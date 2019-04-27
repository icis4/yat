﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.26 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2019 Matthias Kläy.
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

	// Enable debugging of thread state:
////#define DEBUG_THREAD_STATE

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
//// 'System.Net' as well as 'ALAZ.SystemEx.NetEx' are explicitly used for more obvious distinction.
using System.Text;
using System.Threading;

using MKY.Collections.Generic;
using MKY.Contracts;
using MKY.Diagnostics;
using MKY.Net;

#endregion

namespace MKY.IO.Serial.Socket
{
	/// <summary>
	/// Implements the <see cref="IIOProvider"/> interface for a TCP/IP server.
	/// </summary>
	/// <remarks>
	/// In case of YAT with the original ALAZ implementation, TCP/IP clients and servers created a
	/// deadlock on shutdown. The situation:
	/// 
	/// 1. <see cref="Stop()"/> is called from a GUI/main thread.
	/// 2. 'ALAZ.SystemEx.NetEx.SocketsEx.BaseSocketConnectionHost.StopConnections()' blocks.
	/// 3. The 'OnDisconnected' event is raised.
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
	/// Note that the very same issue existed in <see cref="TcpClient"/>.
	/// 
	/// Also note that a very similar issue existed when stopping two <see cref="TcpAutoSocket"/>
	/// that were interconnected with each other. See remarks of this class for details.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Azevedo' is a name.")]
	public class TcpServer : IIOProvider, IDisposable, ALAZ.SystemEx.NetEx.SocketsEx.ISocketService
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

			Listening,
			Accepted,
			Stopping,
			Error,
		}

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const int DataSentQueueInitialCapacity = 4096;

		private const int ThreadWaitTimeout = 500; // Enough time to let the threads join...

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private int instanceId;

		/// <summary>
		/// A dedicated event helper to allow autonomously ignoring exceptions when disposed.
		/// </summary>
		private EventHelper.Item eventHelper = EventHelper.CreateItem(typeof(TcpServer).FullName);

		private IPNetworkInterfaceEx localInterface;
		private int localPort;

		private SocketState state = SocketState.Reset;
		private object stateSyncObj = new object();

		/// <remarks>
		/// Required to deal with the issues described in the remarks in the header of this class.
		/// </remarks>
		private bool isStoppingAndDisposingSocket;
		private object isStoppingAndDisposingSocketSyncObj = new object();

		private ALAZ.SystemEx.NetEx.SocketsEx.SocketServer socket;
		private object socketSyncObj = new object();

		private List<ALAZ.SystemEx.NetEx.SocketsEx.ISocketConnection> socketConnections = new List<ALAZ.SystemEx.NetEx.SocketsEx.ISocketConnection>();

		private object dataEventSyncObj = new object();

		/// <remarks>
		/// Async event handling. The capacity is set large enough to reduce the number of resizing
		/// operations while adding items.
		/// </remarks>
		private Queue<Pair<byte, System.Net.IPEndPoint>> dataSentQueue = new Queue<Pair<byte, System.Net.IPEndPoint>>(DataSentQueueInitialCapacity);

		private bool dataSentThreadRunFlag;
		private AutoResetEvent dataSentThreadEvent;
		private Thread dataSentThread;
		private object dataSentThreadSyncObj = new object();

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

		/// <summary>Creates a TCP/IP server socket.</summary>
		/// <exception cref="ArgumentNullException"><paramref name="localInterface"/> is is <c>null</c>.</exception>
		public TcpServer(IPNetworkInterfaceEx localInterface, int localPort)
			: this(SocketBase.NextInstanceId, localInterface, localPort)
		{
		}

		/// <summary>Creates a TCP/IP server socket.</summary>
		/// <exception cref="ArgumentNullException"><paramref name="localInterface"/> is is <c>null</c>.</exception>
		public TcpServer(int instanceId, IPNetworkInterfaceEx localInterface, int localPort)
		{
			if (localInterface == null) throw (new ArgumentNullException("localInterface"));

			this.instanceId     = instanceId;

			this.localInterface = localInterface;
			this.localPort      = localPort;
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public bool IsDisposed { get; protected set; }

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
			if (!IsDisposed)
			{
				DebugEventManagement.DebugWriteAllEventRemains(this);
				this.eventHelper.DiscardAllEventsAndExceptions();

				DebugMessage("Disposing...");

				// Dispose of managed resources if requested:
				if (disposing)
				{
					// In the 'normal' case, the items have already been disposed of, e.g. in Stop().
					StopAndDisposeSocketAndConnectionsAndThreadAsync();

					// Do not dispose of state and shutdown locks because that will result in null
					// ref exceptions during closing, due to the fact that ALAZ closes/disconnects
					// asynchronously! No better solution has been found to this issue. And, who
					// cares if these two locks don't get disposed (except FxCop ;-).
				}

				// Set state to disposed:
				IsDisposed = true;

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
		~TcpServer()
		{
			Dispose(false);

			DebugDisposal.DebugNotifyFinalizerInsteadOfDispose(this);
		}
	#endif // DEBUG

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (IsDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed!"));
		}

		#endregion

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

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
		public virtual int LocalPort
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.localPort);
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
					case SocketState.Accepted:
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
		public virtual int ConnectedClientCount
		{
			get
			{
				AssertNotDisposed();

				int count;
				lock (this.socketConnections)
					count = this.socketConnections.Count;

				return (count);
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

		private bool IsStoppingAndDisposingSocketSynchronized
		{
			get
			{
				bool value;

				lock (this.isStoppingAndDisposingSocketSyncObj)
					value = this.isStoppingAndDisposingSocket;

				return (value);
			}

			set
			{
				lock (this.isStoppingAndDisposingSocketSyncObj)
					this.isStoppingAndDisposingSocket = value;
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
				DebugMessage("Starting...");
				StartSocketAndThread();
				return (true);
			}
			else
			{
				DebugMessage("Start() requested but state is already " + GetStateSynchronized() + ".");
				return (true); // Return 'true' since socket is already started.
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "'Stop' is a common term to start/stop something.")]
		public virtual void Stop()
		{
			// AssertNotDisposed() is called by 'IsStarted' below.

			if (IsStarted)
			{
				DebugMessage("Stopping...");
				SetStateSynchronizedAndNotify(SocketState.Stopping);

				// Dispose of ALAZ socket in any case. A new socket will be created on next Start().
				StopAndDisposeSocketAndConnectionsAndThreadAsync();

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
				lock (this.socketConnections)
				{
					foreach (var connection in this.socketConnections)
						connection.BeginSend(data);
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

			lock (this.stateSyncObj)
				state = this.state;

			return (state);
		}

		private void SetStateSynchronizedAndNotify(SocketState state)
		{
#if (DEBUG)
			SocketState oldState = this.state;
#endif
			lock (this.stateSyncObj)
				this.state = state;
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
			IsStoppingAndDisposingSocketSynchronized = false;

			SetStateSynchronizedAndNotify(SocketState.Listening);

			StartDataSentThread();

			lock (this.socketSyncObj)
			{
				this.socket = new ALAZ.SystemEx.NetEx.SocketsEx.SocketServer
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

				this.socket.AddListener("MKY.IO.Serial.Socket.TcpServer", new System.Net.IPEndPoint(this.localInterface.Address, this.localPort));
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
		private void StopAndDisposeSocketAndConnectionsAndThreadAsync()
		{
			IsStoppingAndDisposingSocketSynchronized = true;

			var asyncInvoker = new Action(StopAndDisposeSocketAndConnectionsAndThread);
			asyncInvoker.BeginInvoke(null, null);
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		private void StopAndDisposeSocketAndConnectionsAndThread()
		{
			lock (this.socketSyncObj)
			{
				if (this.socket != null)
				{
					try
					{
						DebugSocketShutdown("Stopping socket...");
						this.socket.Stop(); // Attention: ALAZ sockets don't properly stop on Dispose().
						DebugSocketShutdown("...successfully stopped.");
					}
					catch (Exception ex)
					{
						DebugEx.WriteException(GetType(), ex, "Stopping socket of TCP/IP server failed!");
					}

					try
					{
						DebugSocketShutdown("Disposing socket...");
						this.socket.Dispose(); // Attention: ALAZ sockets don't properly stop on Dispose().
						DebugSocketShutdown("...successfully disposed.");
					}
					catch (Exception ex)
					{
						DebugEx.WriteException(GetType(), ex, "Disposing socket of TCP/IP server failed!");
					}
					finally
					{
						this.socket = null;
					}
				}
			}

			lock (this.socketConnections)
				this.socketConnections.Clear(); // Note: List is kept throughout lifetime of this object.

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
					DebugThreadState("DataSentThread() gets stopped...");

					this.dataSentThreadRunFlag = false;

					// Ensure that thread has stopped after the stop request:
					try
					{
						Debug.Assert(this.dataSentThread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId, "Attention: Tried to join itself!");

						bool isAborting = false;
						int accumulatedTimeout = 0;
						int interval = 0; // Use a relatively short random interval to trigger the thread:
						while (!this.dataSentThread.Join(interval = SocketBase.Random.Next(5, 20)))
						{
							SignalDataSentThreadSafely();

							accumulatedTimeout += interval;
							if (accumulatedTimeout >= ThreadWaitTimeout)
							{
								DebugThreadState("...failed! Aborting...");
								DebugThreadState("(Abort is likely required due to failed synchronization back the calling thread, which is typically the GUI/main thread.)");

								isAborting = true;           // Thread.Abort() must not be used whenever possible!
								this.dataSentThread.Abort(); // This is only the fall-back in case joining fails for too long.
								break;
							}

							DebugThreadState("...trying to join at " + accumulatedTimeout + " ms...");
						}

						if (!isAborting)
							DebugThreadState("...successfully stopped.");
					}
					catch (ThreadStateException)
					{
						// Ignore thread state exceptions such as "Thread has not been started" and
						// "Thread cannot be aborted" as it just needs to be ensured that the thread
						// has or will be terminated for sure.

						DebugThreadState("...failed too but will be exectued as soon as the calling thread gets suspended again.");
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
		/// Event raised when connected.
		/// </summary>
		/// <param name="e">
		/// Information about the connection.
		/// </param>
		public virtual void OnConnected(ALAZ.SystemEx.NetEx.SocketsEx.ConnectionEventArgs e)
		{
			lock (this.socketConnections)
				this.socketConnections.Add(e.Connection);

			SetStateSynchronizedAndNotify(SocketState.Accepted);

			// Immediately begin receiving data:
			e.Connection.BeginReceive();
		}

		/// <summary>
		/// Event raised when data arrives.
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
		/// Event raised when data has been sent.
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
			// No clue why the 'Sent' event is raised once before actual data is being sent...
			if ((e.Buffer != null) && (e.Buffer.Length > 0))
			{
				lock (this.dataSentQueue) // Lock is required because Queue<T> is not synchronized.
				{
					foreach (byte b in e.Buffer)
						this.dataSentQueue.Enqueue(new Pair<byte, System.Net.IPEndPoint>(b, e.Connection.RemoteEndPoint));

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
			DebugMessage("SendThread() has started.");

			try
			{
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

						if (Monitor.TryEnter(this.dataEventSyncObj, 10)) // Allow a short time to enter, as receiving
						{                                                // could be busy mostly locking the object.
							try
							{
								System.Net.IPEndPoint remoteEndPoint = null;
								List<byte> data;

								lock (this.dataSentQueue) // Lock is required because Queue<T> is not synchronized.
								{
									data = new List<byte>(this.dataSentQueue.Count); // Preset the required capacity to improve memory management.

									while (this.dataSentQueue.Count > 0)
									{
										Pair<byte, System.Net.IPEndPoint> item;

										// First, peek to check whether data refers to a different end point:
										item = this.dataSentQueue.Peek();

										if (remoteEndPoint == null)
											remoteEndPoint = item.Value2;
										else if (remoteEndPoint != item.Value2)
											break; // Break as soon as data of a different end point is available.

										// If still the same end point, dequeue the item to acknowledge it's gone:
										item = this.dataSentQueue.Dequeue();
										data.Add(item.Value1);
									}
								}

								OnDataSent(new SocketDataSentEventArgs(data.ToArray(), remoteEndPoint));
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
			}
			catch (ThreadAbortException ex)
			{
				DebugEx.WriteException(GetType(), ex, "DataSentThread() has been aborted! Confirming the abort, i.e. Thread.ResetAbort() will be called...");

				// Should only happen when failing to 'friendly' join the thread on stopping!
				// Don't try to set and notify a state change, or even restart the socket!

				// But reset the abort request, as 'ThreadAbortException' is a special exception
				// that would be rethrown at the end of the catch block otherwise!

				Thread.ResetAbort();
			}

			DebugMessage("SendThread() has terminated.");
		}

		/// <summary>
		/// Event raised when disconnected.
		/// </summary>
		/// <param name="e">
		/// Information about the connection.
		/// </param>
		public virtual void OnDisconnected(ALAZ.SystemEx.NetEx.SocketsEx.ConnectionEventArgs e)
		{
			DebugSocketShutdown("Socket 'OnDisconnected' event!");

			bool isConnected = false;
			lock (this.socketConnections)
			{
				this.socketConnections.Remove(e.Connection);
				isConnected = (this.socketConnections.Count > 0);
			}

			// Note that state and shutdown locks do not get disposed in order to be still able to
			// access while asynchronously closing/disconnecting. See Dispose() for more details.
			if (!isConnected && !IsStoppingAndDisposingSocketSynchronized)
			{
				if (GetStateSynchronized() == SocketState.Accepted)
					SetStateSynchronizedAndNotify(SocketState.Listening);
			}
		}

		/// <summary>
		/// Event raised when exception occurs.
		/// </summary>
		/// <param name="e">
		/// Information about the exception and connection.
		/// </param>
		public virtual void OnException(ALAZ.SystemEx.NetEx.SocketsEx.ExceptionEventArgs e)
		{
			DebugSocketShutdown("Socket 'OnException' event!");

			// Note that state and shutdown locks do not get disposed in order to be still able to
			// access while asynchronously closing/disconnecting. See Dispose() for more details.
			if (!IsStoppingAndDisposingSocketSynchronized)
			{
				// Dispose of ALAZ socket in any case. A new socket will be created on next Start().
				// Must be called asynchronously! Otherwise, a dead-lock will occur in ALAZ.
				StopAndDisposeSocketAndConnectionsAndThreadAsync();

				SetStateSynchronizedAndNotify(SocketState.Error);

				var sb = new StringBuilder();
				sb.AppendLine("The socket of this TCP/IP server has thrown an exception!");
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

		#endregion

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnIOChanged(EventArgs e)
		{
			this.eventHelper.RaiseSync(IOChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOControlChanged(EventArgs e)
		{
			UnusedEvent.PreventCompilerWarning(IOControlChanged);
			throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The event 'IOControlChanged' is not in use for TCP/IP servers!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary></summary>
		protected virtual void OnIOError(IOErrorEventArgs e)
		{
			this.eventHelper.RaiseSync<IOErrorEventArgs>(IOError, this, e);
		}

		/// <summary></summary>
		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true)]
		protected virtual void OnDataReceived(DataReceivedEventArgs e)
		{
			if (IsOpen) // Make sure to propagate event only if active.
				this.eventHelper.RaiseSync<DataReceivedEventArgs>(DataReceived, this, e);
		}

		/// <summary></summary>
		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true)]
		protected virtual void OnDataSent(DataSentEventArgs e)
		{
			if (IsOpen) // Make sure to propagate event only if active.
				this.eventHelper.RaiseSync<DataSentEventArgs>(DataSent, this, e);
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		public override string ToString()
		{
			// See below why AssertNotDisposed() is not called on such basic method!

			return (ToShortEndPointString());
		}

		/// <summary></summary>
		/// <remarks>
		/// Named according to .NET <see cref="System.Net.IPEndPoint"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "EndPoint", Justification = "Naming according to System.Net.EndPoint.")]
		public virtual string ToShortEndPointString()
		{
			if (IsDisposed)
				return (base.ToString()); // Do not call AssertNotDisposed() on such basic method!

			return ("Server:" + this.localPort);
		}

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
					CultureInfo.CurrentCulture,
					" @ {0} @ Thread #{1} : {2,36} {3,3} {4,-38} : {5}",
					DateTime.Now.ToString("HH:mm:ss.fff", DateTimeFormatInfo.CurrentInfo),
					Thread.CurrentThread.ManagedThreadId.ToString("D3", CultureInfo.CurrentCulture),
					GetType(),
					"#" + this.instanceId.ToString("D2", CultureInfo.CurrentCulture),
					"[" + ToShortEndPointString() + "]",
					message
				)
			);
		}

		[Conditional("DEBUG_THREAD_STATE")]
		private void DebugThreadState(string message)
		{
			DebugMessage(message);
		}

		[Conditional("DEBUG_SOCKET_SHUTDOWN")]
		private void DebugSocketShutdown(string message)
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