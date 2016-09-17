﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.15
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Configuration
//==================================================================================================
// Configuration
//==================================================================================================

#if (DEBUG)

	// Enable debugging of ALAZ socket shutdown:
////#define DEBUG_SOCKET_SHUTDOWN

#endif // DEBUG

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Threading;

using MKY.Contracts;
using MKY.Diagnostics;
using MKY.Net;

#endregion

namespace MKY.IO.Serial.Socket
{
	/// <remarks>
	/// In case of YAT with the original ALAZ implementation, TCP/IP clients and servers created a
	/// deadlock on shutdown. The situation:
	/// 
	/// 1. <see cref="Stop()"/> is called from a GUI/main thread.
	/// 2. 'ALAZ.SystemEx.NetEx.SocketsEx.BaseSocketConnectionHost.StopConnections()' blocks.
	/// 3. The 'OnDisconnected' event is fired.
	/// 4. FireOnDisconnected() is blocked when trying to synchronize Invoke() onto the GUI/main
	///    thread and a deadlock happens.
	/// 
	/// Further down the calling chain, 'BaseSocketConnection.Active.get()' was also blocking.
	/// 
	/// These two issues could be solved by modifying 'BaseSocketConnection.Active.get()' to be
	/// non-blocking, by calling Stop() asynchronously and by suppressing the 'OnDisconnected' and
	/// 'OnException' events while stopping.
	/// 
	/// These two issues were also reported back to Andre Luis Azevedo. But unfortunately ALAZ seems
	/// to have come to a dead end. An alternative to ALAZ might need to be found in the future.
	/// 
	/// Note that the very same issue existed in <see cref="TcpServer"/>.
	/// 
	/// Also note that a very similar issue existed when stopping two <see cref="TcpAutoSocket"/>
	/// that were interconnected with each other. See remarks of this class for details.
	/// </remarks>
	public class TcpClient : IIOProvider, IDisposable, ALAZ.SystemEx.NetEx.SocketsEx.ISocketService
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private enum SocketState
		{
			/// <summary>
			/// The reset state is necessary to differentiate between not yet started or stopped
			/// sockets versus started but disconnected sockets.
			/// </summary>
			Reset,

			Connecting,
			Connected,
			Disconnecting,
			Disconnected,
			WaitingForReconnect,
			Stopping,
			Error,
		}

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const int DataSentQueueInitialCapacity = 4096;

		private const int ThreadWaitTimeout = 200;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private int instanceId;
		private bool isDisposed;

		private IPHostEx remoteHost;
		private int remotePort;
		private IPNetworkInterfaceEx localInterface;
		private AutoInterval autoReconnect;

		private SocketState state = SocketState.Reset;
		private ReaderWriterLockSlim stateLock = new ReaderWriterLockSlim();

		/// <remarks>
		/// Required to deal with the issues described in the remarks in the header of this class.
		/// </remarks>
		private bool isStoppingAndDisposing;
		private ReaderWriterLockSlim isStoppingAndDisposingLock = new ReaderWriterLockSlim();

		private ALAZ.SystemEx.NetEx.SocketsEx.SocketClient socket;
		private object socketSyncObj = new object();

		private ALAZ.SystemEx.NetEx.SocketsEx.ISocketConnection socketConnection;
		private object socketConnectionSyncObj = new object();

		private object dataEventSyncObj = new object();

		/// <remarks>
		/// Async event handling. The capacity is set large enough to reduce the number of resizing
		/// operations while adding elements.
		/// </remarks>
		private Queue<byte> dataSentQueue = new Queue<byte>(DataSentQueueInitialCapacity);

		private bool dataSentThreadRunFlag;
		private AutoResetEvent dataSentThreadEvent;
		private Thread dataSentThread;
		private object dataSentThreadSyncObj = new object();

		private System.Timers.Timer reconnectTimer;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		public event EventHandler IOChanged;

		/// <summary></summary>
		public event EventHandler IOControlChanged;

		/// <summary></summary>
		public event EventHandler<IOErrorEventArgs> IOError;

		/// <summary></summary>
		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true)]
		public event EventHandler<DataReceivedEventArgs> DataReceived;

		/// <summary></summary>
		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true)]
		public event EventHandler<DataSentEventArgs> DataSent;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary>Creates a TCP/IP client socket.</summary>
		/// <exception cref="ArgumentNullException"><paramref name="remoteHost"/> is is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="localInterface"/> is is <c>null</c>.</exception>
		public TcpClient(IPHostEx remoteHost, int remotePort, IPNetworkInterfaceEx localInterface)
			: this(SocketBase.NextInstanceId, remoteHost, remotePort, localInterface)
		{
		}

		/// <summary>Creates a TCP/IP client socket.</summary>
		/// <exception cref="ArgumentNullException"><paramref name="remoteHost"/> is is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="localInterface"/> is is <c>null</c>.</exception>
		public TcpClient(int instanceId, IPHostEx remoteHost, int remotePort, IPNetworkInterfaceEx localInterface)
			: this(instanceId, remoteHost, remotePort, localInterface, new AutoInterval())
		{
		}

		/// <summary>Creates a TCP/IP client socket.</summary>
		/// <exception cref="ArgumentNullException"><paramref name="remoteHost"/> is is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="localInterface"/> is is <c>null</c>.</exception>
		public TcpClient(IPHostEx remoteHost, int remotePort, IPNetworkInterfaceEx localInterface, AutoInterval autoReconnect)
			: this(SocketBase.NextInstanceId, remoteHost, remotePort, localInterface, autoReconnect)
		{
		}

		/// <summary>Creates a TCP/IP client socket.</summary>
		/// <exception cref="ArgumentNullException"><paramref name="remoteHost"/> is is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="localInterface"/> is is <c>null</c>.</exception>
		public TcpClient(int instanceId, IPHostEx remoteHost, int remotePort, IPNetworkInterfaceEx localInterface, AutoInterval autoReconnect)
		{
			if (remoteHost == null)     throw (new ArgumentNullException("remoteHost"));
			if (localInterface == null) throw (new ArgumentNullException("localInterface"));

			this.instanceId = instanceId;

			this.remoteHost     = remoteHost;
			this.remotePort     = remotePort;
			this.localInterface = localInterface;
			this.autoReconnect  = autoReconnect;
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "stateLock", Justification = "See comments below.")]
		[SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "dataSentThreadEvent", Justification = "Disposed of asynchronously via SuppressEventsAndThenStopAndDisposeSocketAndConnectionsAndThread().")]
		[SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "socket", Justification = "Disposed of asynchronously via SuppressEventsAndThenStopAndDisposeSocketAndConnectionsAndThread().")]
		[SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "isStoppingAndDisposingLock", Justification = "See comments below.")]
		protected virtual void Dispose(bool disposing)
		{
			DebugEventManagement.DebugNotifyAllEventRemains(this);

			if (!this.isDisposed)
			{
				DebugMessage("Disposing...");

				// Dispose of managed resources if requested:
				if (disposing)
				{
					// In the 'normal' case, the items have already been disposed of, e.g. OnDisconnected().
					StopAndDisposeReconnectTimer();
					SuppressEventsAndThenStopAndDisposeSocketAndConnectionsAndThreadAsync();

					// Do not dispose of state and shutdown locks because that will result in null
					// ref exceptions during closing, due to the fact that ALAZ closes/disconnects
					// asynchronously! No better solution has been found to this issue. And, who
					// cares if these two locks don't get disposed (except FxCop ;-).
				}

				// Set state to disposed:
				this.isDisposed = true;

				DebugMessage("...successfully disposed.");
			}
		}

#if (DEBUG)

		/// <remarks>
		/// Microsoft.Design rule CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable requests
		/// "Types that declare disposable members should also implement IDisposable. If the type
		///  does not own any unmanaged resources, do not implement a finalizer on it."
		/// 
		/// Well, true for best performance on finalizing. However, it's not easy to find missing
		/// calls to <see cref="Dispose()"/>. In order to detect such missing calls, the finalizer
		/// is kept for DEBUG, indicating missing calls.
		/// 
		/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
		/// </remarks>
		~TcpClient()
		{
			Dispose(false);

			DebugDisposal.DebugNotifyFinalizerInsteadOfDispose(this);
		}

#endif // DEBUG

		/// <summary></summary>
		public bool IsDisposed
		{
			get { return (this.isDisposed); }
		}

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (this.isDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed!"));
		}

		#endregion

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public virtual IPHostEx RemoteHost
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.remoteHost);
			}
		}

		/// <summary></summary>
		public virtual int RemotePort
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.remotePort);
			}
		}

		/// <summary></summary>
		public virtual IPNetworkInterfaceEx LocalInterface
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.localInterface);
			}
		}

		/// <summary></summary>
		public virtual AutoInterval AutoReconnect
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.autoReconnect);
			}
		}

		/// <summary></summary>
		public virtual bool IsStopped
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				switch (GetStateSynchronized())
				{
					case SocketState.Reset:
					case SocketState.Stopping: // Stopping is considered 'Stopped' because it will eventually result to that.
					case SocketState.Error:
					{
						return (true);
					}
					default:
					{
						return (false);
					}
				}
			}
		}

		/// <summary></summary>
		public virtual bool IsStarted
		{
			get { return (!IsStopped); }
		}

		/// <summary></summary>
		public virtual bool IsConnected
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				switch (GetStateSynchronized())
				{
					case SocketState.Connected:
					{
						return (true);
					}
					default:
					{
						return (false);
					}
				}
			}
		}

		/// <summary></summary>
		public virtual bool IsOpen
		{
			get { return (IsConnected); }
		}

		/// <summary></summary>
		public virtual bool IsTransmissive
		{
			get { return (IsConnected); }
		}

		private bool AutoReconnectEnabledAndAllowed
		{
			get { return (!IsDisposed && IsStarted && !IsOpen && AutoReconnect.Enabled); } // Check 'IsDisposed' first!
		}

		private bool IsStoppingAndDisposingSynchronized
		{
			get
			{
				bool value;

				this.isStoppingAndDisposingLock.EnterReadLock();
				value = this.isStoppingAndDisposing;
				this.isStoppingAndDisposingLock.ExitReadLock();

				return (value);
			}

			set
			{
				this.isStoppingAndDisposingLock.EnterWriteLock();
				this.isStoppingAndDisposing = value;
				this.isStoppingAndDisposingLock.ExitWriteLock();
			}
		}

		/// <summary></summary>
		public virtual object UnderlyingIOInstance
		{
			get
			{
				AssertNotDisposed();

				lock (this.socketSyncObj)
					return (this.socket);
			}
		}

		#endregion

		#region Public Methods
		//==========================================================================================
		// Public Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual bool Start()
		{
			// AssertNotDisposed() is called by 'IsStopped' below.

			if (IsStopped)
			{
				DebugMessage("Resolving host address...");
				if (this.remoteHost.TryResolve())
				{
					DebugMessage("Starting...");
					StartSocketAndThread();
					return (true);
				}

				DebugMessage("...failed");
				return (false);
			}
			else
			{
				DebugMessage("Start() requested but state is already " + GetStateSynchronized() + ".");
				return (true); // Return 'true' since socket is already started.
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "Stop is a common term to start/stop something.")]
		public virtual void Stop()
		{
			// AssertNotDisposed() is called by 'IsStarted' below.

			if (IsStarted)
			{
				DebugMessage("Stopping...");
				SetStateSynchronizedAndNotify(SocketState.Stopping);

				StopAndDisposeReconnectTimer();

				// Dispose of ALAZ socket in any case. A new socket will be created on next Start().
				SuppressEventsAndThenStopAndDisposeSocketAndConnectionsAndThreadAsync();

				SetStateSynchronizedAndNotify(SocketState.Reset);
			}
			else
			{
				DebugMessage("Stop() requested but state is " + GetStateSynchronized() + ".");
			}
		}

		/// <summary></summary>
		public virtual bool Send(byte[] data)
		{
			// AssertNotDisposed() is called by 'IsStarted' below.

			if (IsStarted)
			{
				lock (this.socketConnectionSyncObj)
				{
					if (this.socketConnection != null)
						this.socketConnection.BeginSend(data);
				}

				return (true);
			}
			else
			{
				return (false);
			}
		}

		#endregion

		#region State Methods
		//==========================================================================================
		// State Methods
		//==========================================================================================

		private SocketState GetStateSynchronized()
		{
			SocketState state;

			this.stateLock.EnterReadLock();
			state = this.state;
			this.stateLock.ExitReadLock();

			return (state);
		}

		private void SetStateSynchronizedAndNotify(SocketState state)
		{
#if (DEBUG)
			SocketState oldState = GetStateSynchronized();
#endif
			this.stateLock.EnterWriteLock();
			this.state = state;
			this.stateLock.ExitWriteLock();
#if (DEBUG)
			if (this.state != oldState)
				DebugMessage("State has changed from " + oldState + " to " + state + ".");
			else
				DebugMessage("State is already " + oldState + ".");
#endif
			OnIOChanged(EventArgs.Empty);
		}

		#endregion

		#region Socket Methods
		//==========================================================================================
		// Socket Methods
		//==========================================================================================

		/// <remarks>
		/// Note that ALAZ sockets start asynchronously, same as stopping.
		/// </remarks>
		private void StartSocketAndThread()
		{
			IsStoppingAndDisposingSynchronized = false;

			SetStateSynchronizedAndNotify(SocketState.Connecting);

			StartDataSentThread();

			lock (this.socketSyncObj)
			{
				this.socket = new ALAZ.SystemEx.NetEx.SocketsEx.SocketClient
				(
					ALAZ.SystemEx.NetEx.SocketsEx.CallbackThreadType.ctWorkerThread,
					this,
					ALAZ.SystemEx.NetEx.SocketsEx.DelimiterType.dtNone,
					null,
					SocketDefaults.SocketBufferSize,
					SocketDefaults.MessageBufferSize,
					Timeout.Infinite,
					Timeout.Infinite
				);

				this.socket.AddConnector("MKY.IO.Serial.Socket.TcpClient", new System.Net.IPEndPoint(this.remoteHost.Address, this.remotePort), new System.Net.IPEndPoint(this.localInterface.Address, 0));
				this.socket.Start(); // The ALAZ socket will be started asynchronously.
			}
		}

		/// <remarks>
		/// Note that ALAZ sockets stop asynchronously, same as starting.
		/// 
		/// Attention:
		/// The Stop() method of the ALAZ socket must not be called on the GUI/main thread.
		/// See remarks of the header of this class for details.
		/// </remarks>
		private void SuppressEventsAndThenStopAndDisposeSocketAndConnectionsAndThreadAsync()
		{
			IsStoppingAndDisposingSynchronized = true;

			VoidDelegateVoid asyncInvoker = new VoidDelegateVoid(StopAndDisposeSocketAndConnectionsAndThread);
			asyncInvoker.BeginInvoke(null, null);
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		private void StopAndDisposeSocketAndConnectionsAndThread()
		{
			lock (this.socketSyncObj)
			{
				if (this.socket != null)
				{
					try
					{
						DebugSocketShutdownMessage("Stopping socket...");
						this.socket.Stop(); // Attention: ALAZ sockets don't properly stop on Dispose().
						DebugSocketShutdownMessage("...successfully stopped.");
					}
					catch (Exception ex)
					{
						DebugEx.WriteException(GetType(), ex, "Stopping socket of TCP/IP client failed!");
					}

					try
					{
						DebugSocketShutdownMessage("Disposing socket...");
						this.socket.Dispose(); // Attention: ALAZ sockets don't properly stop on Dispose().
						DebugSocketShutdownMessage("...successfully disposed.");
					}
					catch (Exception ex)
					{
						DebugEx.WriteException(GetType(), ex, "Disposing socket of TCP/IP client failed!");
					}
					finally
					{
						this.socket = null;
					}
				}
			}

			lock (this.socketConnectionSyncObj)
				this.socketConnection = null;

			// Finally, stop the thread. Must be done AFTER the socket got stopped (and disposed) to
			// ensure that the last socket callbacks 'OnSent' can still be properly processed.
			StopDataSentThread();
		}

		#endregion

		#region Socket Threads
		//==========================================================================================
		// Socket Threads
		//==========================================================================================

		private void StartDataSentThread()
		{
			lock (this.dataSentThreadSyncObj)
			{
				if (this.dataSentThread == null)
				{
					this.dataSentThreadRunFlag = true;
					this.dataSentThreadEvent = new AutoResetEvent(false);
					this.dataSentThread = new Thread(new ThreadStart(DataSentThread));
					this.dataSentThread.Name = ToShortEndPointString() + " DataSent Thread";
					this.dataSentThread.Start();
				}
			}
		}

		/// <remarks>
		/// Especially useful during potentially dangerous creation and disposal sequence.
		/// </remarks>
		private void SignalDataSentThreadSafely()
		{
			try
			{
				if (this.dataSentThreadEvent != null)
					this.dataSentThreadEvent.Set();
			}
			catch (ObjectDisposedException ex) { DebugEx.WriteException(GetType(), ex, "Unsafe thread signaling caught"); }
			catch (NullReferenceException ex)  { DebugEx.WriteException(GetType(), ex, "Unsafe thread signaling caught"); }

			// Catch 'NullReferenceException' for the unlikely case that the event has just been
			// disposed after the if-check. This way, the event doesn't need to be locked (which
			// is a relatively time-consuming operation). Still keep the if-check for the normal
			// cases.
		}

		/// <remarks>
		/// Using 'Stop' instead of 'Terminate' to emphasize graceful termination, i.e. trying
		/// to join first, then abort if not successfully joined.
		/// </remarks>
		private void StopDataSentThread()
		{
			lock (this.dataSentThreadSyncObj)
			{
				if (this.dataSentThread != null)
				{
					DebugThreadStateMessage("DataSentThread() gets stopped...");

					this.dataSentThreadRunFlag = false;

					// Ensure that thread has stopped after the stop request:
					try
					{
						Debug.Assert(this.dataSentThread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId, "Attention: Tried to join itself!");

						int accumulatedTimeout = 0;
						int interval = 0; // Use a relatively short random interval to trigger the thread:
						while (!this.dataSentThread.Join(interval = SocketBase.Random.Next(5, 20)))
						{
							SignalDataSentThreadSafely();

							accumulatedTimeout += interval;
							if (accumulatedTimeout >= ThreadWaitTimeout)
							{
								DebugThreadStateMessage("...failed! Aborting...");
								DebugThreadStateMessage("(Abort is likely required due to failed synchronization back the calling thread, which is typically the GUI/main thread.)");
								this.dataSentThread.Abort();
								break;
							}

							DebugThreadStateMessage("...trying to join at " + accumulatedTimeout + " ms...");
						}

						DebugThreadStateMessage("...successfully stopped.");
					}
					catch (ThreadStateException)
					{
						// Ignore thread state exceptions such as "Thread has not been started" and
						// "Thread cannot be aborted" as it just needs to be ensured that the thread
						// has or will be terminated for sure.

						DebugThreadStateMessage("...failed too but will be exectued as soon as the calling thread gets suspended again.");
					}

					this.dataSentThread = null;
				}

				if (this.dataSentThreadEvent != null)
				{
					try     { this.dataSentThreadEvent.Close(); }
					finally { this.dataSentThreadEvent = null; }
				}
			}
		}

		#endregion

		#region ISocketService Members
		//==========================================================================================
		// ISocketService Members
		//==========================================================================================

		/// <summary>
		/// Fired when connected.
		/// </summary>
		/// <param name="e">
		/// Information about the connection.
		/// </param>
		public virtual void OnConnected(ALAZ.SystemEx.NetEx.SocketsEx.ConnectionEventArgs e)
		{
			lock (this.socketConnectionSyncObj)
				this.socketConnection = e.Connection;

			SetStateSynchronizedAndNotify(SocketState.Connected);

			// Immediately begin receiving data:
			e.Connection.BeginReceive();
		}

		/// <summary>
		/// Fired when data arrives.
		/// </summary>
		/// <param name="e">
		/// Information about the Message.
		/// </param>
		public virtual void OnReceived(ALAZ.SystemEx.NetEx.SocketsEx.MessageEventArgs e)
		{
			// Synchronize the send/receive events to prevent mix-ups at the event
			// sinks, i.e. the send/receive operations shall be synchronized with
			// signaling of them. The thread = direction that get's the lock first,
			// shall also be the one to signal first:

			lock (this.dataEventSyncObj)
			{
				// This receive callback is always asychronous, thus the event handler can
				// be called directly. It is also ensured that the event handler is called
				// sequentially because the 'BeginReceive()' method is only called after
				// the event handler has returned.
				OnDataReceived(new SocketDataReceivedEventArgs((byte[])e.Buffer.Clone(), e.Connection.RemoteEndPoint));
			}

			// Continue receiving:
			e.Connection.BeginReceive();
		}

		/// <summary>
		/// Fired when data has been sent.
		/// </summary>
		/// <param name="e">
		/// Information about the data that has been sent.
		/// <remarks>
		/// Note that the original ALAZ implementation always keeps 'e.Buffer' at
		/// <c>null</c> whereas the modified version contains a filled data buffer.
		/// </remarks>
		/// </param>
		public virtual void OnSent(ALAZ.SystemEx.NetEx.SocketsEx.MessageEventArgs e)
		{
			// No clue why the 'Sent' event fires once before actual data is being sent...
			if ((e.Buffer != null) && (e.Buffer.Length > 0))
			{
				lock (this.dataSentQueue) // Lock is required because Queue<T> is not synchronized.
				{
					foreach (byte b in e.Buffer)
						this.dataSentQueue.Enqueue(b);

					// Note that individual bytes are enqueued, not array of bytes. Analysis has
					// shown that this is faster than enqueuing arrays, since this callback will
					// mostly be called with rather low numbers of bytes.
				}

				SignalDataSentThreadSafely();
			}
		}

		/// <summary>
		/// Asynchronously manage outgoing send requests to ensure that send events are not
		/// invoked on the same thread that triggered the send operation.
		/// Also, the mechanism implemented below reduces the amount of events that are propagated
		/// to the main application. Small chunks of sent data would generate many events in
		/// <see cref="Send(byte[])"/>. However, since <see cref="OnSent"/> synchronously
		/// invokes the event, it will take some time until the send queue is checked again.
		/// During this time, no more new events are invoked, instead, outgoing data is buffered.
		/// </summary>
		/// <remarks>
		/// Will be signaled by <see cref="Send(byte[])"/> method above.
		/// </remarks>
		[SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", MessageId = "System.Threading.WaitHandle.#WaitOne(System.Int32)", Justification = "Installer indeed targets .NET 3.5 SP1.")]
		private void DataSentThread()
		{
			System.Net.IPEndPoint remoteEndPoint;
			lock (this.socketSyncObj) // Remote end point is fixed for a TCP/IP client object.
				remoteEndPoint = new System.Net.IPEndPoint(this.remoteHost.Address, this.remotePort);

			DebugThreadStateMessage("SendThread() has started.");

			// Outer loop, processes data after a signal was received:
			while (!IsDisposed && this.dataSentThreadRunFlag) // Check 'IsDisposed' first!
			{
				try
				{
					// WaitOne() will wait forever if the underlying I/O provider has crashed, or
					// if the overlying client isn't able or forgets to call Stop() or Dispose().
					// Therefore, only wait for a certain period and then poll the run flag again.
					// The period can be quite long, as an event trigger will immediately resume.
					if (!this.dataSentThreadEvent.WaitOne(SocketBase.Random.Next(50, 200)))
						continue;
				}
				catch (AbandonedMutexException ex)
				{
					// The mutex should never be abandoned, but in case it nevertheless happens,
					// at least output a debug message and gracefully exit the thread.
					DebugEx.WriteException(GetType(), ex, "An 'AbandonedMutexException' occurred in DataSentThread()!");
					break;
				}

				// Inner loop, runs as long as there is data to be handled.
				// Ensure not to forward events during disposing anymore. Check 'IsDisposed' first!
				while (!IsDisposed && this.dataSentThreadRunFlag && (this.dataSentQueue.Count > 0))
				{                                                // No lock required, just checking for empty.
					// Initially, yield to other threads before starting to read the queue, since it is very
					// likely that more data is to be enqueued, thus resulting in larger chunks processed.
					// Subsequently, yield to other threads to allow processing the data.
					Thread.Sleep(TimeSpan.Zero);

					// Synchronize the send/receive events to prevent mix-ups at the event
					// sinks, i.e. the send/receive operations shall be synchronized with
					// signaling of them.
					// But attention, do not simply lock() the sync obj. Instead, just try
					// to get the lock or try again later. The thread = direction that get's
					// the lock first, shall also be the one to signal first:

					if (Monitor.TryEnter(this.dataEventSyncObj))
					{
						try
						{
							byte[] data;
							lock (this.dataSentQueue) // Lock is required because Queue<T> is not synchronized.
							{
								data = this.dataSentQueue.ToArray();
								this.dataSentQueue.Clear();
							}

							OnDataSent(new SocketDataSentEventArgs(data, remoteEndPoint));
						}
						finally
						{
							Monitor.Exit(this.dataEventSyncObj);
						}
					} // Monitor.TryEnter()

					// Note the Thread.Sleep(TimeSpan.Zero) above.

					// Saying hello to StyleCop ;-.
				} // Inner loop
			} // Outer loop

			DebugThreadStateMessage("SendThread() has terminated.");
		}

		/// <summary>
		/// Fired when disconnected.
		/// </summary>
		/// <param name="e">
		/// Information about the connection.
		/// </param>
		public virtual void OnDisconnected(ALAZ.SystemEx.NetEx.SocketsEx.ConnectionEventArgs e)
		{
			DebugSocketShutdownMessage("Socket 'OnDisconnected' event!");

			// Note that state and shutdown locks do not get disposed in order to be still able to
			// access while asynchronously closing/disconnecting. See Dispose() for more details.
			if (!IsStoppingAndDisposingSynchronized)
			{
				// Dispose of ALAZ socket in any case. A new socket will be created on next Start().
				// Must be called asynchronously! Otherwise, a dead-lock will occur in ALAZ.
				SuppressEventsAndThenStopAndDisposeSocketAndConnectionsAndThreadAsync();

				// Attention:
				// Ensure that exceptions are only handled if the socket is still active. Otherwise,
				// this event handler must not signal any state anymore, nor does it need to try to
				// reconnect.
				// 
				// Attention:
				// Similar code is needed in 'OnException' below.
				// Changes here may have to be applied there too.
				if (!IsDisposed && IsStarted) // Check 'IsDisposed' first!
				{
					// Signal that socket got disconnected to ensure that auto reconnect is allowed.
					SetStateSynchronizedAndNotify(SocketState.Disconnected);

					if (AutoReconnectEnabledAndAllowed)
					{
						SetStateSynchronizedAndNotify(SocketState.WaitingForReconnect);
						StartReconnectTimer();
					}
					else
					{
						SetStateSynchronizedAndNotify(SocketState.Reset);
					}
				}
			}
		}

		/// <summary>
		/// Fired when exception occurs.
		/// </summary>
		/// <remarks>
		/// Attention:
		/// This event is also fired on reconnect attempts of auto-reconnect!
		/// 
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		/// <param name="e">
		/// Information about the exception and connection.
		/// </param>
		public virtual void OnException(ALAZ.SystemEx.NetEx.SocketsEx.ExceptionEventArgs e)
		{
			DebugSocketShutdownMessage("Socket 'OnException' event!");

			// Note that state and shutdown locks do not get disposed in order to be still able to
			// access while asynchronously closing/disconnecting. See Dispose() for more details.
			if (!IsStoppingAndDisposingSynchronized)
			{
				// Dispose of ALAZ socket in any case. A new socket will be created on next Start().
				// Must be called asynchronously! Otherwise, a dead-lock will occur in ALAZ.
				SuppressEventsAndThenStopAndDisposeSocketAndConnectionsAndThreadAsync();

				// Attention:
				// Ensure that exceptions are only handled if the socket is still active. Otherwise,
				// this event handler must not signal any state anymore, nor does it need to try to
				// reconnect.
				// 
				// Attention:
				// Similar code is needed in 'OnDisconnected' above.
				// Changes here may have to be applied there too.
				if (!IsDisposed && IsStarted) // Check 'IsDisposed' first!
				{
					// Signal that socket got disconnected to ensure that auto reconnect is allowed.
					SetStateSynchronizedAndNotify(SocketState.Disconnected);

					if (AutoReconnectEnabledAndAllowed)
					{
						SetStateSynchronizedAndNotify(SocketState.WaitingForReconnect);
						StartReconnectTimer();
					}
					else
					{
						SetStateSynchronizedAndNotify(SocketState.Error);

						if (e.Exception is ALAZ.SystemEx.NetEx.SocketsEx.ReconnectAttemptException)
						{
							OnIOError(new IOErrorEventArgs(ErrorSeverity.Acceptable, "Failed to connect to TCP/IP server " + this.remoteHost.Address + ":" + this.remotePort));
						}
						else
						{
							StringBuilder sb = new StringBuilder();
							sb.AppendLine("The socket of this TCP/IP client has fired an exception!");
							sb.AppendLine();
							sb.AppendLine("Exception type:");
							sb.AppendLine(e.Exception.GetType().Name);
							sb.AppendLine();
							sb.AppendLine("Exception error message:");
							sb.AppendLine(e.Exception.Message);
							string message = sb.ToString();
							DebugMessage(message);

							OnIOError(new IOErrorEventArgs(ErrorSeverity.Fatal, message));
						}
					}
				}
			}
		}

		#endregion

		#region Reconnect Timer
		//==========================================================================================
		// Reconnect Timer
		//==========================================================================================

		private void StartReconnectTimer()
		{
			if (this.reconnectTimer != null)
				StopAndDisposeReconnectTimer();

			this.reconnectTimer = new System.Timers.Timer(this.autoReconnect.Interval);
			this.reconnectTimer.AutoReset = false;
			this.reconnectTimer.Elapsed += new System.Timers.ElapsedEventHandler(reconnectTimer_Elapsed);
			this.reconnectTimer.Start();
		}

		private void StopAndDisposeReconnectTimer()
		{
			if (this.reconnectTimer != null)
			{
				this.reconnectTimer.Stop();
				this.reconnectTimer.Dispose();
				this.reconnectTimer = null;
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		private void reconnectTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (AutoReconnectEnabledAndAllowed)
			{
				try
				{
					StartSocketAndThread();
				}
				catch
				{
					StartReconnectTimer();
				}
			}
			else
			{
				StopAndDisposeReconnectTimer();
			}
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnIOChanged(EventArgs e)
		{
			EventHelper.FireSync(IOChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOControlChanged(EventArgs e)
		{
			UnusedEvent.PreventCompilerWarning(IOControlChanged);
			throw (new InvalidOperationException("Program execution should never get here, the event 'IOControlChanged' is not in use for TCP/IP clients!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary></summary>
		protected virtual void OnIOError(IOErrorEventArgs e)
		{
			EventHelper.FireSync<IOErrorEventArgs>(IOError, this, e);
		}

		/// <summary></summary>
		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true)]
		protected virtual void OnDataReceived(DataReceivedEventArgs e)
		{
			EventHelper.FireSync<DataReceivedEventArgs>(DataReceived, this, e);
		}

		/// <summary></summary>
		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true)]
		protected virtual void OnDataSent(DataSentEventArgs e)
		{
			EventHelper.FireSync<DataSentEventArgs>(DataSent, this, e);
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary></summary>
		public override string ToString()
		{
			return (ToShortEndPointString());
		}

		#region Object Members > Extensions
		//------------------------------------------------------------------------------------------
		// Object Members > Extensions
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		/// <remarks>
		/// Named according to .NET <see cref="System.Net.IPEndPoint"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "EndPoint", Justification = "Naming according to System.Net.EndPoint.")]
		public virtual string ToShortEndPointString()
		{
			return (this.remoteHost.ToEndpointAddressString() + ":" + this.remotePort);
		}

		#endregion

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		[Conditional("DEBUG")]
		private void DebugMessage(string message)
		{
			Debug.WriteLine
			(
				string.Format
				(
					CultureInfo.InvariantCulture,
					" @ {0} @ Thread #{1} : {2,36} {3,3} {4,-38} : {5}",
					DateTime.Now.ToString("HH:mm:ss.fff", DateTimeFormatInfo.InvariantInfo),
					Thread.CurrentThread.ManagedThreadId.ToString("D3", CultureInfo.InvariantCulture),
					GetType(),
					"#" + this.instanceId.ToString("D2", CultureInfo.InvariantCulture),
					"[" + ToShortEndPointString() + "]",
					message
				)
			);
		}

		[Conditional("DEBUG_THREAD_STATE")]
		private void DebugThreadStateMessage(string message)
		{
			DebugMessage(message);
		}

		[Conditional("DEBUG_SOCKET_SHUTDOWN")]
		private void DebugSocketShutdownMessage(string message)
		{
			DebugMessage(message);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
