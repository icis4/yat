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

	// Enable debugging of sending:
////#define DEBUG_SEND // Attention: Must also be activated in TcpClient.Send.cs !!

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
//// 'System.Net.Sockets' including.
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
	/// Note that the very same issue existed in <see cref="TcpClient"/>.
	///
	/// Also note that a very similar issue existed when stopping two <see cref="TcpAutoSocket"/>
	/// that were interconnected with each other. See remarks of this class for details.
	/// </remarks>
	/// <remarks>
	/// This class is implemented using partial classes separating sending functionality.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Azevedo' is a name.")]
	public partial class TcpServer : DisposableBase, IIOProvider, ALAZ.SystemEx.NetEx.SocketsEx.ISocketService
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

		/// <summary>Creates a TCP/IP server socket.</summary>
		/// <exception cref="ArgumentException"><paramref name="localInterface"/> is <see cref="IPNetworkInterface.Explicit"/>.</exception>
		public TcpServer(IPNetworkInterface localInterface, int localPort)
			: this((IPNetworkInterfaceEx)localInterface, localPort)
		{
		}

		/// <summary>Creates a TCP/IP server socket.</summary>
		/// <exception cref="ArgumentNullException"><paramref name="localInterface"/> is <c>null</c>.</exception>
		public TcpServer(IPNetworkInterfaceEx localInterface, int localPort)
			: this(SocketBase.NextInstanceId, localInterface, localPort)
		{
		}

		/// <summary>Creates a TCP/IP server socket.</summary>
		/// <exception cref="ArgumentException"><paramref name="localInterface"/> is <see cref="IPNetworkInterface.Explicit"/>.</exception>
		public TcpServer(int instanceId, IPNetworkInterface localInterface, int localPort)
			: this(instanceId, (IPNetworkInterfaceEx)localInterface, localPort)
		{
		}

		/// <summary>Creates a TCP/IP server socket.</summary>
		/// <exception cref="ArgumentNullException"><paramref name="localInterface"/> is <c>null</c>.</exception>
		public TcpServer(int instanceId, IPNetworkInterfaceEx localInterface, int localPort)
		{
			// Verify by-reference arguments:

			if (localInterface == null) throw (new ArgumentNullException("localInterface"));

			// All arguments are defined!

			this.instanceId     = instanceId;

			this.localInterface = localInterface;
			this.localPort      = localPort;
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
				// In the 'normal' case, the items have already been disposed of, e.g. in Stop().
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
		public virtual IPNetworkInterfaceEx LocalInterface
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				return (this.localInterface);
			}
		}

		/// <summary></summary>
		public virtual int LocalPort
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				return (this.localPort);
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
				AssertUndisposed();

				int count;
				lock (this.socketConnections) // Directly locking the list is OK, it is kept throughout the lifetime of an object.
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
		////AssertUndisposed() is called by 'IsStarted' below.

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

			lock (this.socketConnections) // Directly locking the list is OK, it is kept throughout the lifetime of an object.
				this.socketConnections.Clear();

			// Finally, stop the thread. Must be done AFTER the socket got stopped (and disposed) to
			// ensure that the last socket callbacks 'OnSent' can still be properly processed.
			StopDataSentThread();

			// And don't forget to clear the corresponding queue, its content would reappear in case
			// the socket gets started again.
			lock (this.dataSentQueue) // Lock is required because Queue<T> is not synchronized.
				this.dataSentQueue.Clear();
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
			lock (this.socketConnections) // Directly locking the list is OK, it is kept throughout the lifetime of an object.
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

				DebugSendEnqueue(e.Buffer.Length);

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

			bool isConnected = false;
			lock (this.socketConnections) // Directly locking the list is OK, it is kept throughout the lifetime of an object.
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

				var message = sb.ToString();
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
		protected virtual void OnIOChanged(EventArgs<DateTime> e)
		{
			if (IsUndisposed) // Ensure to not propagate event during closing anymore. This may happen on an async System.Net.Sockets.SocketAsyncEventArgs.CompletionPortCallback.
				this.eventHelper.RaiseSync(IOChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOControlChanged(EventArgs<DateTime> e)
		{
			UnusedEvent.PreventCompilerWarning(IOControlChanged, "See exception message below.");
			throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The event 'IOControlChanged' is not in use for TCP/IP servers!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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

			return ("Server:" + this.localPort);
		}

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		/// <remarks>
		/// Name "DebugWriteLine" would show relation to <see cref="Debug.WriteLine(string)"/>.
		/// However, named "Message" for compactness and more clarity that something will happen
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
