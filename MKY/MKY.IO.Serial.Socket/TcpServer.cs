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
// MKY Development Version 1.0.14
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
using System.Text;
using System.Threading;

using MKY.Collections.Generic;
using MKY.Contracts;
using MKY.Diagnostics;

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
	/// Note that the very same issue existed in <see cref="TcpClient"/>.
	/// 
	/// Also note that a very similar issue existed when stopping two <see cref="TcpAutoSocket"/>
	/// that were interconnected with each other. See remarks of this class for details.
	/// </remarks>
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

		private const int ThreadWaitTimeout = 200;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private int instanceId;
		private bool isDisposed;

		private System.Net.IPAddress localIPAddress;
		private int localPort;

		private SocketState state = SocketState.Reset;
		private ReaderWriterLockSlim stateLock = new ReaderWriterLockSlim();

		/// <remarks>
		/// Required to deal with the issues described in the remarks in the header of this class.
		/// </remarks>
		private bool isStoppingAndDisposing;
		private ReaderWriterLockSlim isStoppingAndDisposingLock = new ReaderWriterLockSlim();

		private ALAZ.SystemEx.NetEx.SocketsEx.SocketServer socket;
		private object socketSyncObj = new object();

		private List<ALAZ.SystemEx.NetEx.SocketsEx.ISocketConnection> socketConnections = new List<ALAZ.SystemEx.NetEx.SocketsEx.ISocketConnection>();

		/// <remarks>
		/// Async event handling. The capacity is set large enough to reduce the number of resizing
		/// operations while adding elements.
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

		/// <summary></summary>
		public TcpServer(System.Net.IPAddress localIPAddress, int localPort)
			: this(SocketBase.NextInstanceId, localIPAddress, localPort)
		{
		}

		/// <summary></summary>
		public TcpServer(int instanceId, System.Net.IPAddress localIPAddress, int localPort)
		{
			this.instanceId = instanceId;

			this.localIPAddress = localIPAddress;
			this.localPort = localPort;
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
			if (!this.isDisposed)
			{
				WriteDebugMessageLine("Disposing...");

				// Dispose of managed resources if requested:
				if (disposing)
				{
					// In the 'normal' case, the items have already been disposed of, e.g. in Stop().
					SuppressEventsAndThenStopAndDisposeSocketAndConnectionsAndThreadAsync();

					// Do not dispose of state and shutdown locks because that will result in null
					// ref exceptions during closing, due to the fact that ALAZ closes/disconnects
					// asynchronously! No better solution has been found to this issue. And, who
					// cares if these two locks don't get disposed (except FxCop ;-).
				}

				// Set state to disposed:
				this.isDisposed = true;

				WriteDebugMessageLine("...successfully disposed.");
			}
		}

		/// <summary></summary>
		~TcpServer()
		{
			Dispose(false);

			WriteDebugSocketShutdownMessageLine("The finalizer should have never been called! Ensure to call Dispose()!");
		}

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
		public virtual System.Net.IPAddress LocalIPAddress
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.localIPAddress);
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
			// AssertNotDisposed() is called by 'IsStarted' below.

			if (IsStopped)
			{
				WriteDebugMessageLine("Starting...");
				StartSocketAndThread();
				return (true);
			}
			else
			{
				WriteDebugMessageLine("Start() requested but state is " + GetStateSynchronized() + ".");
				return (false);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "Stop is a common term to start/stop something.")]
		public virtual void Stop()
		{
			// AssertNotDisposed() is called by 'IsStarted' below.

			if (IsStarted)
			{
				WriteDebugMessageLine("Stopping...");
				SetStateSynchronizedAndNotify(SocketState.Stopping);

				// Dispose of ALAZ socket in any case. A new socket will be created on next Start().
				SuppressEventsAndThenStopAndDisposeSocketAndConnectionsAndThreadAsync();

				SetStateSynchronizedAndNotify(SocketState.Reset);
			}
			else
			{
				WriteDebugMessageLine("Stop() requested but state is " + GetStateSynchronized() + ".");
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
					foreach (ALAZ.SystemEx.NetEx.SocketsEx.ISocketConnection connection in this.socketConnections)
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
				WriteDebugMessageLine("State has changed from " + oldState + " to " + state + ".");
			else
				WriteDebugMessageLine("State is already " + oldState + ".");
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

			SetStateSynchronizedAndNotify(SocketState.Listening);

			StartDataSentThread();

			lock (this.socketSyncObj)
			{
				this.socket = new ALAZ.SystemEx.NetEx.SocketsEx.SocketServer
				(
					ALAZ.SystemEx.NetEx.SocketsEx.CallbackThreadType.ctWorkerThread,
					(ALAZ.SystemEx.NetEx.SocketsEx.ISocketService)this,
					ALAZ.SystemEx.NetEx.SocketsEx.DelimiterType.dtNone,
					null,
					SocketDefaults.SocketBufferSize,
					SocketDefaults.MessageBufferSize,
					Timeout.Infinite,
					Timeout.Infinite
				);

				this.socket.AddListener("MKY.IO.Serial.Socket.TcpServer", new System.Net.IPEndPoint(System.Net.IPAddress.Any, this.localPort));
				this.socket.Start(); // The ALAZ socket will be started asynchronously.
			}
		}

		/// <remarks>
		/// Note that ALAZ sockets stop asynchronously, same as starting.
		/// 
		/// \attention:
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
						WriteDebugSocketShutdownMessageLine("Stopping socket...");
						this.socket.Stop(); // Attention: ALAZ sockets don't properly stop on Dispose().
						WriteDebugSocketShutdownMessageLine("...successfully stopped.");
					}
					catch (Exception ex)
					{
						DebugEx.WriteException(GetType(), ex, "Stopping socket of TCP/IP server failed!");
					}

					try
					{
						WriteDebugSocketShutdownMessageLine("Disposing socket...");
						this.socket.Dispose(); // Attention: ALAZ sockets don't properly stop on Dispose().
						WriteDebugSocketShutdownMessageLine("...successfully disposed.");
					}
					catch (Exception ex)
					{
						DebugEx.WriteException(GetType(), ex, "Disposing socket of TCP/IP server failed!");
					}

					this.socket = null;
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

		private void StopDataSentThread()
		{
			lock (this.dataSentThreadSyncObj)
			{
				if (this.dataSentThread != null)
				{
					WriteDebugThreadStateMessageLine("DataSentThread() gets stopped...");

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
								WriteDebugThreadStateMessageLine("...failed! Aborting...");
								WriteDebugThreadStateMessageLine("(Abort is likely required due to failed synchronization back the calling thread, which is typically the GUI/main thread.)");
								this.dataSentThread.Abort();
								break;
							}

							WriteDebugThreadStateMessageLine("...trying to join at " + accumulatedTimeout + " ms...");
						}
					}
					catch (ThreadStateException)
					{
						// Ignore thread state exceptions such as "Thread has not been started" and
						// "Thread cannot be aborted" as it just needs to be ensured that the thread
						// has or will be terminated for sure.

						WriteDebugThreadStateMessageLine("...failed too but will be exectued as soon as the calling thread gets suspended again.");
					}

					this.dataSentThreadEvent.Close();
					this.dataSentThreadEvent = null;
					this.dataSentThread = null;

					WriteDebugThreadStateMessageLine("...successfully terminated.");
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
			lock (this.socketConnections)
				this.socketConnections.Add(e.Connection);

			SetStateSynchronizedAndNotify(SocketState.Accepted);

			// Immediately begin receiving data.
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
			// This receive callback is always asychronous, thus the event handler can
			// be called directly. It is also ensured that the event handler is called
			// sequential because the 'BeginReceive()' method is only called after
			// the event handler has returned.
			OnDataReceived(new SocketDataReceivedEventArgs((byte[])e.Buffer.Clone(), e.Connection.RemoteEndPoint));

			// Continue receiving data.
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
						this.dataSentQueue.Enqueue(new Pair<byte, System.Net.IPEndPoint>(b, e.Connection.RemoteEndPoint));
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
			WriteDebugMessageLine("SendThread() has started.");

			// Outer loop, requires another signal.
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

					List<byte> data = new List<byte>();
					System.Net.IPEndPoint remoteEndPoint = null;
					lock (this.dataSentQueue) // Lock is required because Queue<T> is not synchronized.
					{
						while (this.dataSentQueue.Count > 0)
						{
							Pair<byte, System.Net.IPEndPoint> item = this.dataSentQueue.Dequeue();

							data.Add(item.Value1);

							if (remoteEndPoint == null)
								remoteEndPoint = item.Value2;
							else if (remoteEndPoint != item.Value2)
								break; // Break as soon as data of a different end point is available.
						}
					}

					OnDataSent(new SocketDataSentEventArgs(data.ToArray(), remoteEndPoint));

					// Note the Thread.Sleep(TimeSpan.Zero) above.
				}
			}

			WriteDebugMessageLine("SendThread() has terminated.");
		}

		/// <summary>
		/// Fired when disconnected.
		/// </summary>
		/// <param name="e">
		/// Information about the connection.
		/// </param>
		public virtual void OnDisconnected(ALAZ.SystemEx.NetEx.SocketsEx.ConnectionEventArgs e)
		{
			WriteDebugSocketShutdownMessageLine("Socket 'OnDisconnected' event!");

			bool isConnected = false;
			lock (this.socketConnections)
			{
				this.socketConnections.Remove(e.Connection);
				isConnected = (this.socketConnections.Count > 0);
			}

			// Note that state and shutdown locks do not get disposed in order to be still able to
			// access while asynchronously closing/disconnecting. See Dispose() for more details.
			if (!isConnected && !IsStoppingAndDisposingSynchronized)
			{
				if (GetStateSynchronized() == SocketState.Accepted)
					SetStateSynchronizedAndNotify(SocketState.Listening);
			}
		}

		/// <summary>
		/// Fired when exception occurs.
		/// </summary>
		/// <param name="e">
		/// Information about the exception and connection.
		/// </param>
		public virtual void OnException(ALAZ.SystemEx.NetEx.SocketsEx.ExceptionEventArgs e)
		{
			WriteDebugSocketShutdownMessageLine("Socket 'OnException' event!");

			// Note that state and shutdown locks do not get disposed in order to be still able to
			// access while asynchronously closing/disconnecting. See Dispose() for more details.
			if (!IsStoppingAndDisposingSynchronized)
			{
				// Dispose of ALAZ socket in any case. A new socket will be created on next Start().
				// Must be called asynchronously! Otherwise, a dead-lock will occur in ALAZ.
				SuppressEventsAndThenStopAndDisposeSocketAndConnectionsAndThreadAsync();

				SetStateSynchronizedAndNotify(SocketState.Error);

				StringBuilder sb = new StringBuilder();
				sb.AppendLine("The socket of this TCP/IP server has fired an exception!");
				sb.AppendLine();
				sb.AppendLine("Exception type:");
				sb.AppendLine(e.Exception.GetType().Name);
				sb.AppendLine();
				sb.AppendLine("Exception error message:");
				sb.AppendLine(e.Exception.Message);
				string message = sb.ToString();
				WriteDebugMessageLine(message);

				OnIOError(new IOErrorEventArgs(ErrorSeverity.Fatal, message));
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
			throw (new NotImplementedException("Program execution should never get here, the event 'IOControlChanged' is not in use for TCP/IP servers." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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
			return ("Server:" + this.localPort);
		}

		#endregion

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		[Conditional("DEBUG")]
		private void WriteDebugMessageLine(string message)
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
		private void WriteDebugThreadStateMessageLine(string message)
		{
			WriteDebugMessageLine(message);
		}

		[Conditional("DEBUG_SOCKET_SHUTDOWN")]
		private void WriteDebugSocketShutdownMessageLine(string message)
		{
			WriteDebugMessageLine(message);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
