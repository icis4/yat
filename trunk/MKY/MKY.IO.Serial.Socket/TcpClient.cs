//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.28 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2020 Matthias Kläy.
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

using MKY.Contracts;
using MKY.Diagnostics;
using MKY.Net;

#endregion

namespace MKY.IO.Serial.Socket
{
	/// <summary>
	/// Implements the <see cref="IIOProvider"/> interface for a TCP/IP client.
	/// </summary>
	/// <remarks>
	/// In case of YAT with the original ALAZ implementation, TCP/IP clients and servers created a
	/// deadlock on shutdown. The situation:
	///
	/// 1. <see cref="Stop()"/> is called from a main thread.
	/// 2. 'ALAZ.SystemEx.NetEx.SocketsEx.BaseSocketConnectionHost.StopConnections()' blocks.
	/// 3. The 'OnDisconnected' event is raised.
	/// 4. FireOnDisconnected() is blocked when trying to synchronize Invoke() onto the main
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
	/// <remarks>
	/// This class is implemented using partial classes separating sending functionality.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Azevedo' is a name.")]
	public partial class TcpClient : DisposableBase, IIOProvider, ALAZ.SystemEx.NetEx.SocketsEx.ISocketService
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
			Error
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
		/// A dedicated event helper to allow discarding exceptions when object got disposed.
		/// </summary>
		private EventHelper.Item eventHelper = EventHelper.CreateItem(typeof(TcpClient).FullName);

		private IPHostEx remoteHost;
		private int remotePort;
		private IPNetworkInterfaceEx localInterface;
		private AutoInterval autoReconnect;

		private SocketState state = SocketState.Reset;
		private object stateSyncObj = new object();

		/// <remarks>
		/// Required to deal with the issues described in the remarks in the header of this class.
		/// </remarks>
		private bool isStoppingAndDisposingSocket;
		private object isStoppingAndDisposingSocketSyncObj = new object();

		private ALAZ.SystemEx.NetEx.SocketsEx.SocketClient socket;
		private object socketSyncObj = new object();

		private ALAZ.SystemEx.NetEx.SocketsEx.ISocketConnection socketConnection;
		private object socketConnectionSyncObj = new object();

		private object dataEventSyncObj = new object();

		/// <remarks>
		/// Async event handling. The capacity is set large enough to reduce the number of resizing
		/// operations while adding items.
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
		public event EventHandler<EventArgs<DateTime>> IOChanged;

		/// <summary></summary>
		public event EventHandler<EventArgs<DateTime>> IOControlChanged;

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
		/// <exception cref="ArgumentException"><paramref name="remoteHost"/> is <see cref="IPHost.Explicit"/>.</exception>
		/// <exception cref="ArgumentException"><paramref name="localInterface"/> is <see cref="IPNetworkInterface.Explicit"/>.</exception>
		public TcpClient(IPHost remoteHost, int remotePort, IPNetworkInterface localInterface)
			: this((IPHostEx)remoteHost, remotePort, (IPNetworkInterfaceEx)localInterface)
		{
		}

		/// <summary>Creates a TCP/IP client socket.</summary>
		/// <exception cref="ArgumentNullException"><paramref name="remoteHost"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="localInterface"/> is <c>null</c>.</exception>
		public TcpClient(IPHostEx remoteHost, int remotePort, IPNetworkInterfaceEx localInterface)
			: this(SocketBase.NextInstanceId, remoteHost, remotePort, localInterface)
		{
		}

		/// <summary>Creates a TCP/IP client socket.</summary>
		/// <exception cref="ArgumentException"><paramref name="remoteHost"/> is <see cref="IPHost.Explicit"/>.</exception>
		/// <exception cref="ArgumentException"><paramref name="localInterface"/> is <see cref="IPNetworkInterface.Explicit"/>.</exception>
		public TcpClient(int instanceId, IPHost remoteHost, int remotePort, IPNetworkInterface localInterface)
			: this(instanceId, (IPHostEx)remoteHost, remotePort, (IPNetworkInterfaceEx)localInterface)
		{
		}

		/// <summary>Creates a TCP/IP client socket.</summary>
		/// <exception cref="ArgumentNullException"><paramref name="remoteHost"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="localInterface"/> is <c>null</c>.</exception>
		public TcpClient(int instanceId, IPHostEx remoteHost, int remotePort, IPNetworkInterfaceEx localInterface)
			: this(instanceId, remoteHost, remotePort, localInterface, new AutoInterval())
		{
		}

		/// <summary>Creates a TCP/IP client socket.</summary>
		/// <exception cref="ArgumentException"><paramref name="remoteHost"/> is <see cref="IPHost.Explicit"/>.</exception>
		/// <exception cref="ArgumentException"><paramref name="localInterface"/> is <see cref="IPNetworkInterface.Explicit"/>.</exception>
		public TcpClient(IPHost remoteHost, int remotePort, IPNetworkInterface localInterface, AutoInterval autoReconnect)
			: this((IPHostEx)remoteHost, remotePort, (IPNetworkInterfaceEx)localInterface, autoReconnect)
		{
		}

		/// <summary>Creates a TCP/IP client socket.</summary>
		/// <exception cref="ArgumentNullException"><paramref name="remoteHost"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="localInterface"/> is <c>null</c>.</exception>
		public TcpClient(IPHostEx remoteHost, int remotePort, IPNetworkInterfaceEx localInterface, AutoInterval autoReconnect)
			: this(SocketBase.NextInstanceId, remoteHost, remotePort, localInterface, autoReconnect)
		{
		}

		/// <summary>Creates a TCP/IP client socket.</summary>
		/// <exception cref="ArgumentException"><paramref name="remoteHost"/> is <see cref="IPHost.Explicit"/>.</exception>
		/// <exception cref="ArgumentException"><paramref name="localInterface"/> is <see cref="IPNetworkInterface.Explicit"/>.</exception>
		public TcpClient(int instanceId, IPHost remoteHost, int remotePort, IPNetworkInterface localInterface, AutoInterval autoReconnect)
			: this(instanceId, (IPHostEx)remoteHost, remotePort, (IPNetworkInterfaceEx)localInterface, autoReconnect)
		{
		}

		/// <summary>Creates a TCP/IP client socket.</summary>
		/// <exception cref="ArgumentNullException"><paramref name="remoteHost"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="localInterface"/> is <c>null</c>.</exception>
		public TcpClient(int instanceId, IPHostEx remoteHost, int remotePort, IPNetworkInterfaceEx localInterface, AutoInterval autoReconnect)
		{
			if (remoteHost == null)     throw (new ArgumentNullException("remoteHost"));
			if (localInterface == null) throw (new ArgumentNullException("localInterface"));

			this.instanceId     = instanceId;

			this.remoteHost     = remoteHost;
			this.remotePort     = remotePort;
			this.localInterface = localInterface;
			this.autoReconnect  = autoReconnect;
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <param name="disposing">
		/// <c>true</c> when called from <see cref="Dispose"/>,
		/// <c>false</c> when called from finalizer.
		/// </param>
		[SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "stateLock", Justification = "See comments below.")]
		[SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "dataSentThreadEvent", Justification = "Disposed of asynchronously via SuppressEventsAndThenStopAndDisposeSocketAndConnectionsAndThread().")]
		[SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "socket", Justification = "Disposed of asynchronously via SuppressEventsAndThenStopAndDisposeSocketAndConnectionsAndThread().")]
		[SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "isStoppingAndDisposingLock", Justification = "See comments below.")]
		protected override void Dispose(bool disposing)
		{
			this.eventHelper.DiscardAllEventsAndExceptions();

			DebugMessage("Disposing...");

			// Dispose of managed resources:
			if (disposing)
			{
				// In the 'normal' case, the items have already been disposed of, e.g. OnDisconnected().
				StopAndDisposeReconnectTimer();
				StopAndDisposeSocketAndConnectionsAndThreadAsync();

				// Do not dispose of state and shutdown locks because that will result in null
				// ref exceptions during closing, due to the fact that ALAZ closes/disconnects
				// asynchronously! No better solution has been found to this issue. And, who
				// cares if these two locks don't get disposed (except FxCop ;-).
			}

			DebugMessage("...successfully disposed.");
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
			////AssertUndisposed() shall not be called from this simple get-property.

				return (this.remoteHost);
			}
		}

		/// <summary></summary>
		public virtual int RemotePort
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				return (this.remotePort);
			}
		}

		/// <summary></summary>
		public virtual IPNetworkInterfaceEx LocalInterface
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				return (this.localInterface);
			}
		}

		/// <summary></summary>
		public virtual AutoInterval AutoReconnect
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				return (this.autoReconnect);
			}
		}

		/// <summary></summary>
		public virtual bool IsStopped
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

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
			////AssertUndisposed() shall not be called from this simple get-property.

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
			get { return (IsUndisposed && IsStarted && !IsOpen && AutoReconnect.Enabled); } // Check disposal state first!
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
				AssertUndisposed();

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
		////AssertUndisposed() is called by 'IsStopped' below.

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
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "'Stop' is a common term to start/stop something.")]
		public virtual void Stop()
		{
		////AssertUndisposed() is called by 'IsStarted' below.

			if (IsStarted)
			{
				DebugMessage("Stopping...");
				SetStateSynchronizedAndNotify(SocketState.Stopping);

				StopAndDisposeReconnectTimer();

				// Dispose of ALAZ socket in any case. A new socket will be created on next Start().
				StopAndDisposeSocketAndConnectionsAndThreadAsync();

				SetStateSynchronizedAndNotify(SocketState.Reset);
			}
			else
			{
				DebugMessage("Stop() requested but state is " + GetStateSynchronized() + ".");
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

			OnIOChanged(new EventArgs<DateTime>(DateTime.Now));
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
		/// The Stop() method of the ALAZ socket must not be called on the main thread.
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
						DebugEx.WriteException(GetType(), ex, "Stopping socket of TCP/IP client failed!");
					}

					try
					{
						DebugSocketShutdown("Disposing socket...");
						this.socket.Dispose(); // Attention: ALAZ sockets don't properly stop on Dispose().
						DebugSocketShutdown("...successfully disposed.");
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
			lock (this.socketConnectionSyncObj)
				this.socketConnection = e.Connection;

			SetStateSynchronizedAndNotify(SocketState.Connected);

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
						this.dataSentQueue.Enqueue(b);

					// Note that individual bytes are enqueued, not array of bytes. Analysis has
					// shown that this is faster than enqueuing arrays, since this callback will
					// mostly be called with rather low numbers of bytes.
				}

				SignalDataSentThreadSafely();
			}
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

			// Note that state and shutdown locks do not get disposed in order to be still able to
			// access while asynchronously closing/disconnecting. See Dispose() for more details.
			if (!IsStoppingAndDisposingSocketSynchronized)
			{
				// Dispose of ALAZ socket in any case. A new socket will be created on next Start().
				// Must be called asynchronously! Otherwise, a dead-lock will occur in ALAZ.
				StopAndDisposeSocketAndConnectionsAndThreadAsync();

				// Attention:
				// Ensure that exceptions are only handled if the socket is still active. Otherwise,
				// this event handler must not signal any state anymore, nor does it need to try to
				// reconnect.
				//
				// Attention:
				// Similar code is needed in 'OnException' below.
				// Changes here may have to be applied there too.
				if (IsUndisposed && IsStarted) // Check disposal state first!
				{
					// Signal that socket got disconnected to ensure that auto reconnect is allowed:
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
		/// Event raised when exception occurs.
		/// </summary>
		/// <remarks>
		/// Attention:
		/// This event is also raised on reconnect attempts of auto-reconnect!
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
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

				// Attention:
				// Ensure that exceptions are only handled if the socket is still active. Otherwise,
				// this event handler must not signal any state anymore, nor does it need to try to
				// reconnect.
				//
				// Attention:
				// Similar code is needed in 'OnDisconnected' above.
				// Changes here may have to be applied there too.
				if (IsUndisposed && IsStarted) // Check disposal state first!
				{
					// Signal that socket got disconnected to ensure that auto reconnect is allowed:
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
							var sb = new StringBuilder();
							sb.AppendLine("The socket of this TCP/IP client has thrown an exception!");
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
			this.reconnectTimer.Elapsed += reconnectTimer_Elapsed;
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

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
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

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnIOChanged(EventArgs<DateTime> e)
		{
			if (IsUndisposed) // Ensure to not propagate event during closing anymore. This may happen on an async System.Net.Sockets.SocketAsyncEventArgs.CompletionPortCallback.
				this.eventHelper.RaiseSync(IOChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOControlChanged(EventArgs<DateTime> e)
		{
			UnusedEvent.PreventCompilerWarning(IOControlChanged);
			throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The event 'IOControlChanged' is not in use for TCP/IP clients!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary></summary>
		protected virtual void OnIOError(IOErrorEventArgs e)
		{
			if (IsUndisposed) // Ensure to not propagate event during closing anymore. This may happen on an async System.Net.Sockets.SocketAsyncEventArgs.CompletionPortCallback.
				this.eventHelper.RaiseSync<IOErrorEventArgs>(IOError, this, e);
		}

		/// <summary></summary>
		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true)]
		protected virtual void OnDataReceived(DataReceivedEventArgs e)
		{
			if (IsUndisposed && IsOpen) // Make sure to propagate event only if active.
				this.eventHelper.RaiseSync<DataReceivedEventArgs>(DataReceived, this, e);
		}

		/// <summary></summary>
		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true)]
		protected virtual void OnDataSent(DataSentEventArgs e)
		{
			if (IsUndisposed && IsOpen) // Make sure to propagate event only if active.
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
			// AssertUndisposed() shall not be called from such basic method! Its return value may be needed for debugging. All underlying fields are still valid after disposal.

			return (ToShortEndPointString());
		}

		/// <summary></summary>
		/// <remarks>
		/// Named according to .NET <see cref="System.Net.IPEndPoint"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "EndPoint", Justification = "Naming according to System.Net.EndPoint.")]
		public virtual string ToShortEndPointString()
		{
			// AssertUndisposed() shall not be called from such basic method! Its return value is needed for debugging! All underlying fields are still valid after disposal.

			return (this.remoteHost.ToEndpointAddressString() + ":" + this.remotePort);
		}

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		/// <remarks>
		/// Name "DebugWriteLine" would show relation to <see cref="Debug.WriteLine(string)"/>.
		/// However, named "Message" for compactness and more clarity that somthing will happen
		/// with <paramref name="message"/>, and rather than e.g. "Common" for comprehensibility.
		/// </remarks>
		[Conditional("DEBUG")]
		protected virtual void DebugMessage(string message)
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

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
		[Conditional("DEBUG_THREAD_STATE")]
		private void DebugThreadState(string message)
		{
			DebugMessage(message);
		}

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
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
