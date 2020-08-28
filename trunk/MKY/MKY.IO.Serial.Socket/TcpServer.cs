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
////#define DEBUG_THREAD_STATE // Attention: Must also be activated in TcpServer.Send.cs !!

	// Enable debugging of sending:
////#define DEBUG_SEND // Attention: Must also be activated in TcpServer.Send.cs !!

	// Enable debugging of the ALAZ socket connection(s):
////#define DEBUG_SOCKET_CONNECTIONS

	// Enable debugging of the ALAZ socket shutdown:
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
	/// non-blocking, by calling Stop() asynchronously and by carefully handle the 'OnDisconnected'
	/// and 'OnException' events while stopping.
	///
	/// These two issues were also reported back to Andre Luis Azevedo. But unfortunately ALAZ seems
	/// to have come to a dead end. An alternative to ALAZ might need to be found in the future.
	///
	/// Note that the very same issue existed in <see cref="TcpClient"/>.
	///
	/// Also note that a very similar issue existed when stopping two <see cref="TcpAutoSocket"/>
	/// that were interconnected with each other. See remarks of that class for details.
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

		/// <summary>
		/// The maximum possible connections are allowed by default.
		/// </summary>
		public const int ConnectionAllowanceDefault = int.MaxValue;

		private const int SendQueueFixedCapacity       = ALAZEx.MessageBufferSizeDefault;
		private const int DataSentQueueInitialCapacity = ALAZEx.MessageBufferSizeDefault;

		private const int ThreadWaitTimeout = 500; // Enough time to let the threads join...

		#endregion

		#region Static Fields
		//==========================================================================================
		// Static Fields
		//==========================================================================================

		private static Random staticRandom = new Random(RandomEx.NextPseudoRandomSeed());

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
		private int connectionAllowance;

		private SocketState state = SocketState.Reset;
		private object stateSyncObj = new object();

		private ALAZ.SystemEx.NetEx.SocketsEx.SocketServer socket;
		private object socketSyncObj = new object();

		private List<ALAZ.SystemEx.NetEx.SocketsEx.ISocketConnection> socketConnections = new List<ALAZ.SystemEx.NetEx.SocketsEx.ISocketConnection>();

		private object dataEventSyncObj = new object();

		private Queue<byte> sendQueue = new Queue<byte>(SendQueueFixedCapacity);
		private bool sendThreadRunFlag;
		private AutoResetEvent sendThreadEvent;
		private Thread sendThread;
		private object sendThreadSyncObj = new object();

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
		public event EventHandler<IOWarningEventArgs> IOWarning;

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
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public TcpServer(IPNetworkInterface localInterface, int localPort, int connectionAllowance = ConnectionAllowanceDefault)
			: this((IPNetworkInterfaceEx)localInterface, localPort, connectionAllowance)
		{
		}

		/// <summary>Creates a TCP/IP server socket.</summary>
		/// <exception cref="ArgumentNullException"><paramref name="localInterface"/> is <c>null</c>.</exception>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public TcpServer(IPNetworkInterfaceEx localInterface, int localPort, int connectionAllowance = ConnectionAllowanceDefault)
			: this(SocketBase.NextInstanceId, localInterface, localPort, connectionAllowance)
		{
		}

		/// <summary>Creates a TCP/IP server socket.</summary>
		/// <exception cref="ArgumentException"><paramref name="localInterface"/> is <see cref="IPNetworkInterface.Explicit"/>.</exception>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public TcpServer(int instanceId, IPNetworkInterface localInterface, int localPort, int connectionAllowance = ConnectionAllowanceDefault)
			: this(instanceId, (IPNetworkInterfaceEx)localInterface, localPort, connectionAllowance)
		{
		}

		/// <summary>Creates a TCP/IP server socket.</summary>
		/// <exception cref="ArgumentNullException"><paramref name="localInterface"/> is <c>null</c>.</exception>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public TcpServer(int instanceId, IPNetworkInterfaceEx localInterface, int localPort, int connectionAllowance = ConnectionAllowanceDefault)
		{
			// Verify by-reference arguments:

			if (localInterface == null) throw (new ArgumentNullException("localInterface"));

			// All arguments are defined!

			this.instanceId          = instanceId;

			this.localInterface      = localInterface;
			this.localPort           = localPort;
			this.connectionAllowance = connectionAllowance;
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
				// In the 'normal' case, the items have already been disposed of, e.g. in Stop() or OnDisconnected().
				StopAndDisposeSocketAndConnectionsAndThreadAsyncWithoutNotify(); // Must by async! See remarks of the header of this class for details.

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
		public virtual int ConnectionAllowance
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				return (this.connectionAllowance);
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
				////case SocketState.Stopping shall not be considered 'Stopped' even though it will eventually result to that.
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
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				switch (GetStateSynchronized())
				{
					case SocketState.Reset:
					case SocketState.Stopping: // Shall not be considered 'Started' as it is already on the way to 'Stopped'.
					case SocketState.Error:
					{
						return (false);
					}
					default:
					{
						return (true);
					}
				}
			}
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
				CreateAndStartSocketAndThreadsAndNotify();
				return (true);
			}
			else
			{
				DebugMessage("Start() requested but state is already {0}.", GetStateSynchronized());
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
				StopAndDisposeSocketAndConnectionsAndThreadAsyncAndNotify(); // Must by async when called from main thread or ALAZ event callback! See remarks of the header of this class for details.
			}
			else
			{
				DebugMessage("Stop() requested but state already is {0}.", GetStateSynchronized());
			}
		}

		/// <summary></summary>
		protected virtual void StopAndDisposeSocketAndConnectionsAndThreadAsyncAndNotify()
		{
			StopAndDisposeSocketAndConnectionsAndThreadAsync(true);
		}

		/// <summary></summary>
		protected virtual void StopAndDisposeSocketAndConnectionsAndThreadAsyncWithoutNotify()
		{
			StopAndDisposeSocketAndConnectionsAndThreadAsync(false);
		}

		private void StopAndDisposeSocketAndConnectionsAndThreadAsync(bool notify)
		{
			SetStateSynchronized(SocketState.Stopping, notify);

			// Dispose of ALAZ socket in any case. A new socket will be created on next Start().
			StopAndDisposeSocketAndConnectionsAndThreadAsync(); // Must by async when called from main thread or ALAZ event callback! See remarks of the header of this class for details.

			SetStateSynchronized(SocketState.Reset, notify);
		}

		#endregion

		#region State Methods
		//==========================================================================================
		// State Methods
		//==========================================================================================

		private SocketState GetStateSynchronized()
		{
			lock (this.stateSyncObj)
				return (this.state);
		}

		private bool SetStateSynchronized(SocketState state, bool notify)
		{
			SocketState oldState;

			lock (this.stateSyncObj)
			{
				oldState = this.state;
				this.state = state;
			}

		#if (DEBUG)
			if (state != oldState)
				DebugMessage("State has changed from {0} to {1}.", oldState, state);
			else
				DebugMessage("State already is {0}.", oldState);
		#endif

			if (notify && (state != oldState))
				NotifyStateHasChanged();

			return (state != oldState);
		}

		private void NotifyStateHasChanged()
		{
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
		private void CreateAndStartSocketAndThreadsAndNotify()
		{
			SetStateSynchronized(SocketState.Listening, notify: true);

			StartThreads();

			lock (this.socketSyncObj)
			{
				this.socket = new ALAZ.SystemEx.NetEx.SocketsEx.SocketServer
				(
					ALAZ.SystemEx.NetEx.SocketsEx.CallbackThreadType.ctWorkerThread,
					this,
					ALAZ.SystemEx.NetEx.SocketsEx.DelimiterType.dtNone,
					null,
					ALAZEx.SocketBufferSizeDefault,
					ALAZEx.MessageBufferSizeDefault,
					Timeout.Infinite,
					Timeout.Infinite
				);

				this.socket.AddListener("MKY.IO.Serial.Socket.TcpServer", new System.Net.IPEndPoint(this.localInterface.Address, this.localPort));
				this.socket.Start(); // The ALAZ socket will be started asynchronously.
			}
		}

		/// <remarks>
		/// See remarks of the header of this class for details.
		/// </remarks>
		private void StopAndDisposeSocketAndConnectionsAndThreadAsync()
		{
			var asyncInvoker = new Action(StopAndDisposeSocketAndConnectionsAndThread);
			asyncInvoker.BeginInvoke(null, null);

			Thread.Sleep(0); // Actively yield to other threads to prioritize async stopping.
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
			{
				if (this.socketConnections.Count > 0)
				{
					this.socketConnections.Clear();

					DebugSocketConnections("Connections reset to 0.");
				}
			}

			// Finally, stop the thread. Must be done AFTER the socket got stopped (and disposed)
			// to ensure that the last socket callbacks 'OnSent' can still be properly processed.
			StopThreads();

			// And don't forget to clear the corresponding queues, its content would reappear in
			// case the socket gets started again.
			DropQueuesAndNotify();
		}

		private void DropQueuesAndNotify()
		{
			DropSendQueueAndNotify();
			DropDataSentQueueAndNotify();
		}

		private int DropSendQueueAndNotify()
		{
			int droppedCount = DropSendQueue();
			if (droppedCount > 0)
				NotifySendQueueDropped(droppedCount);

			return (droppedCount);
		}

		private int DropSendQueue()
		{
			int droppedCount;
			lock (this.sendQueue) // Lock is required because Queue<T> is not synchronized.
			{
				droppedCount = this.sendQueue.Count;
				this.sendQueue.Clear();
			}

			return (droppedCount);
		}

		private void NotifySendQueueDropped(int droppedCount)
		{
			string message;
			if (droppedCount <= 1)
				message = droppedCount + " byte not sent anymore.";  // Using "byte" rather than "octet" as that is more common, and .NET uses "byte" as well.
			else                                                     // Reason cannot be stated, could be "disconnected" or "stopped/closed"
				message = droppedCount + " bytes not sent anymore."; // Using "byte" rather than "octet" as that is more common, and .NET uses "byte" as well.

			OnIOWarning(new IOWarningEventArgs(Direction.Output, message));
		}

		private int DropDataSentQueueAndNotify()
		{
			int droppedCount = DropDataSentQueue();
			if (droppedCount > 0)
				NotifyDataSentQueueDropped(droppedCount);

			return (droppedCount);
		}

		private int DropDataSentQueue()
		{
			int droppedCount;
			lock (this.dataSentQueue) // Lock is required because Queue<T> is not synchronized.
			{
				droppedCount = this.dataSentQueue.Count;
				this.dataSentQueue.Clear();
			}

			return (droppedCount);
		}

		private void NotifyDataSentQueueDropped(int droppedCount)
		{
			string message;
			if (droppedCount <= 1)
				message = droppedCount + " sent byte dropped.";  // Using "byte" rather than "octet" as that is more common, and .NET uses "byte" as well.
			else                                                 // Reason cannot be stated, could be "disconnected" or "stopped/closed"
				message = droppedCount + " sent bytes dropped."; // Using "byte" rather than "octet" as that is more common, and .NET uses "byte" as well.

			OnIOWarning(new IOWarningEventArgs(Direction.Output, message));
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
			if (!IsUndisposed) // Ignore async callbacks during closing.
				return;

			bool acceptConnection = false;
			bool stateHasChanged = false;

			lock (this.stateSyncObj) // Ensure state is handled atomically.
			{
				lock (this.socketConnections) // Directly locking the list is OK, it is kept throughout the lifetime of an object.
				{
					var state = GetStateSynchronized();
					if ((state == SocketState.Listening) || (state == SocketState.Accepted)) // Only handle when expected.
					{
						if (this.socketConnections.Count < this.connectionAllowance)
						{
							if (!this.socketConnections.Contains(e.Connection))
							{
								this.socketConnections.Add(e.Connection);
								acceptConnection = true;
								stateHasChanged = SetStateSynchronized(SocketState.Accepted, notify: false); // Notify outside lock!

								DebugSocketConnections("Connection (ID = {0}, remote endpoint = {1}) added, now {2} connection(s).", e.Connection.ConnectionId, e.Connection.RemoteEndPoint, this.socketConnections.Count);
							}
							else // Stray event callbacks may e.g. happen on AutoReconnect of a TCP/IP Client.
							{
								DebugMessage("Ignoring stray 'OnConnected' event callback (connection ID = {0}, remote endpoint = {1}) as connection is already contained.", e.Connection.ConnectionId, e.Connection.RemoteEndPoint);
							}
						}
						else
						{
							DebugMessage("Rejecting additional connection (ID = {0}, remote endpoint = {1}) as allowance of {2} has already been reached.", e.Connection.ConnectionId, e.Connection.RemoteEndPoint, this.connectionAllowance);
						}
					}
					else // Such stray event callbacks should never happen.
					{
						DebugMessage("Ignoring stray 'OnConnected' event callback (connection ID = {0}, remote endpoint = {1}) while state was neither 'Listening' nor 'Accepted'.", e.Connection.ConnectionId, e.Connection.RemoteEndPoint);
					}
				}
			}

			if (acceptConnection)
				e.Connection.BeginReceive(); // Immediately begin receiving.
			else
				e.Connection.BeginDisconnect(); // Immediately begin disconnecting.
			                                  //// Silently, i.e. no OnIOWarning().
			if (stateHasChanged)
				NotifyStateHasChanged(); // Notify outside lock!
		}

		/// <summary>
		/// Event raised when data arrives.
		/// </summary>
		/// <param name="e">
		/// Information about the Message.
		/// </param>
		public virtual void OnReceived(ALAZ.SystemEx.NetEx.SocketsEx.MessageEventArgs e)
		{
			if (!IsUndisposed) // Ignore async callbacks during closing.
				return;

			// Synchronize the send/receive events to prevent mix-ups at the event
			// sinks, i.e. the send/receive operations shall be synchronized with
			// signaling of them. The thread = direction that get's the lock first,
			// shall also be the one to signal first:

			lock (this.socketConnections) // Directly locking the list is OK, it is kept throughout the lifetime of an object.
			{
				if (!this.socketConnections.Contains(e.Connection)) // Stray event callbacks may e.g. happen on AutoReconnect of a TCP/IP Client.
				{
					DebugMessage("Ignoring stray 'OnReceived' event callback (connection ID = {0}, remote endpoint = {1}).", e.Connection.ConnectionId, e.Connection.RemoteEndPoint);
					return;
				}
			}

			lock (this.dataEventSyncObj)
			{
				// This receive callback is always asychronous, thus the event handler can
				// be called directly. It is also ensured that the event handler is called
				// sequentially because the 'BeginReceive()' method is only called after
				// the event handler has returned.
				OnDataReceived(new SocketDataReceivedEventArgs((byte[])e.Buffer.Clone(), e.Connection.RemoteEndPoint));
			}

			e.Connection.BeginReceive(); // Continue receiving.
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
			if (!IsUndisposed) // Ignore async callbacks during closing.
				return;

			lock (this.socketConnections) // Directly locking the list is OK, it is kept throughout the lifetime of an object.
			{
				if (!this.socketConnections.Contains(e.Connection)) // Stray event callbacks may e.g. happen on AutoReconnect of a TCP/IP Client.
				{
					DebugMessage("Ignoring stray 'OnSent' event callback (connection ID = {0}, remote endpoint = {1}).", e.Connection.ConnectionId, e.Connection.RemoteEndPoint);
					return;
				}
			}

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

				DebugDataSentEnqueue(e.Buffer.Length);

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
			if (!IsUndisposed) // Ignore async callbacks during closing.
				return;

			DebugSocketShutdown("Socket 'OnDisconnected' event!");

			bool isStillConnected = false;
			bool stateHasChanged = false;
			int droppedSendCount = 0;
			int droppedDataSentCount = 0;

			lock (this.stateSyncObj) // Ensure state is handled atomically.
			{
				lock (this.socketConnections) // Directly locking the list is OK, it is kept throughout the lifetime of an object.
				{
					if (this.socketConnections.Contains(e.Connection))
					{
						this.socketConnections.Remove(e.Connection);
						isStillConnected = (this.socketConnections.Count > 0);

						DebugSocketConnections("Connection removed (ID = {0}, remote endpoint = {1}), is {2} connected.", e.Connection.ConnectionId, e.Connection.RemoteEndPoint, (isStillConnected ? "still" : "no longer"));
					}
					else // Stray event callbacks may e.g. happen on AutoReconnect of a TCP/IP Client.
					{
						DebugMessage("Ignoring stray 'OnDisconnected' event callback (connection ID = {0}, remote endpoint = {1}).", e.Connection.ConnectionId, e.Connection.RemoteEndPoint);
					}
				}

				if (!isStillConnected) // Drop inside but notify outside lock!
				{
					droppedSendCount     = DropSendQueue();
					droppedDataSentCount = DropDataSentQueue();

					if (GetStateSynchronized() == SocketState.Accepted)
						stateHasChanged = SetStateSynchronized(SocketState.Listening, notify: false); // Notify outside lock!
				}
			}

			if (droppedSendCount > 0)
				NotifySendQueueDropped(droppedSendCount); // Notify outside lock!

			if (droppedDataSentCount > 0)
				NotifyDataSentQueueDropped(droppedDataSentCount); // Notify outside lock!

			if (stateHasChanged)
				NotifyStateHasChanged(); // Notify outside lock!
		}

		/// <summary>
		/// Event raised when exception occurs.
		/// </summary>
		/// <param name="e">
		/// Information about the exception and connection.
		/// </param>
		public virtual void OnException(ALAZ.SystemEx.NetEx.SocketsEx.ExceptionEventArgs e)
		{
			if (!IsUndisposed) // Ignore async callbacks during closing.
				return;

			DebugSocketShutdown("Socket 'OnException' event!");

			lock (this.socketConnections) // Directly locking the list is OK, it is kept throughout the lifetime of an object.
			{
				if (this.socketConnections.Contains(e.Connection)) // Stray event callbacks may e.g. happen on AutoReconnect of a TCP/IP Client.
				{
					DebugMessage("Ignoring stray 'OnException' event callback (connection ID = {0}, remote endpoint = {1}).", e.Connection.ConnectionId, e.Connection.RemoteEndPoint);
					return;
				}
			}

			bool stateHasChanged = false;
			bool notifyError = false;

			lock (this.stateSyncObj) // Ensure state is handled atomically.
			{
				var state = GetStateSynchronized();
				if (!((state == SocketState.Error) || (state == SocketState.Reset))) // Don't handle when no longer active.
				{
					stateHasChanged = SetStateSynchronized(SocketState.Error, notify: false); // Notify outside lock!

					// Dispose of ALAZ socket in any case. A new socket will be created on next Start().
					StopAndDisposeSocketAndConnectionsAndThreadAsync(); // Must by async when called from main thread or ALAZ event callback! See remarks of the header of this class for details.

					notifyError = true;
				}
				else // (Error || Reset) => don't handle when no longer active.
				{
					DebugEx.WriteException(GetType(), e.Exception, "'OnException' event callback is ignored as socket is no longer active.");
				}
			}

			if (notifyError) // Notify outside lock!
			{
				// Attention, the error event must be raised before changing the state,
				// because e.g. in case of an AutoSocket, the state change to 'Error'
				// will trigger disposal of this Server, thus a subsequent event would
				// get discarded and no error would be raised anymore. Note that the same
				// applies to the Client implementation.

				var sb = new StringBuilder();
				sb.AppendLine("The socket of this TCP/IP server has thrown an exception!");
				sb.AppendLine();
				sb.AppendLine("Exception type:");
				sb.AppendLine(e.Exception.GetType().FullName);
				sb.AppendLine();
				sb.AppendLine("Exception error message:");
				sb.AppendLine(e.Exception.Message);

				var message = sb.ToString();
				DebugMessage(message);
				OnIOError(new IOErrorEventArgs(ErrorSeverity.Fatal, message)); // Notify outside lock!
			}

			if (stateHasChanged)
				NotifyStateHasChanged(); // Notify outside lock!
		}

		#endregion

		#region Socket Threads
		//==========================================================================================
		// Socket Threads
		//==========================================================================================

		private void StartThreads()
		{
			lock (this.sendThreadSyncObj)
			{
				if (this.sendThread == null)
				{
					this.sendThreadRunFlag = true;
					this.sendThreadEvent = new AutoResetEvent(false);
					this.sendThread = new Thread(new ThreadStart(SendThread));
					this.sendThread.Name = ToShortEndPointString() + " Send Thread";
					this.sendThread.Start();
				}
			}

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
		/// Using 'Stop' instead of 'Terminate' to emphasize graceful termination, i.e. trying
		/// to join first, then abort if not successfully joined.
		/// </remarks>
		private void StopThreads()
		{
			// First, clear both flags to reduce the time to stop the threads, they may already
			// be signaled while receiving data or while the send thread is still running:

			lock (this.sendThreadSyncObj)
				this.sendThreadRunFlag = false;

			lock (this.dataSentThreadSyncObj)
					this.dataSentThreadRunFlag = false;

			// Then, wait for threads to terminate:

			lock (this.sendThreadSyncObj)
			{
				if (this.sendThread != null)
				{
					Debug.Assert(this.sendThread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId, "Attention: Tried to join itself!");

					DebugThreadState("SendThread() gets stopped...");

					// Ensure that thread has stopped after the stop request:
					try
					{
						bool isAborting = false;
						int accumulatedTimeout = 0;
						int interval = 0; // Use a relatively short random interval to trigger the thread:
						while (!this.sendThread.Join(interval = staticRandom.Next(5, 20)))
						{
							SignalSendThreadSafely();

							accumulatedTimeout += interval;
							if (accumulatedTimeout >= ThreadWaitTimeout)
							{
								DebugThreadState("...failed! Aborting...");
								DebugThreadState("(Abort is likely required due to failed synchronization back the calling thread, which is typically the main thread.)");

								isAborting = true;       // Thread.Abort() must not be used whenever possible!
								this.sendThread.Abort(); // This is only the fall-back in case joining fails for too long.
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

					this.sendThread = null;
				}

				if (this.sendThreadEvent != null)
				{
					try     { this.sendThreadEvent.Close(); }
					finally { this.sendThreadEvent = null; }
				}
			} // lock (sendThreadSyncObj)

			lock (this.dataSentThreadSyncObj)
			{
				if (this.dataSentThread != null)
				{
					Debug.Assert(this.dataSentThread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId, "Attention: Tried to join itself!");

					DebugThreadState("DataSentThread() gets stopped...");

					// Ensure that thread has stopped after the stop request:
					try
					{
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
								DebugThreadState("(Abort is likely required due to failed synchronization back the calling thread, which is typically the main thread.)");

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

		/// <remarks>
		/// Especially useful during potentially dangerous creation and disposal sequence.
		/// </remarks>
		private void SignalSendThreadSafely()
		{
			try
			{
				if (this.sendThreadEvent != null)
					this.sendThreadEvent.Set();
			}
			catch (ObjectDisposedException ex) { DebugEx.WriteException(GetType(), ex, "Unsafe thread signaling caught"); }
			catch (NullReferenceException ex)  { DebugEx.WriteException(GetType(), ex, "Unsafe thread signaling caught"); }

			// Catch 'NullReferenceException' for the unlikely case that the event has just been
			// disposed after the if-check. This way, the event doesn't need to be locked (which
			// is a relatively time-consuming operation). Still keep the if-check for the normal
			// cases.
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
		protected virtual void OnIOWarning(IOWarningEventArgs e)
		{
			if (IsUndisposed) // Ensure to not propagate event during closing anymore. This may happen on an async System.Net.Sockets.SocketAsyncEventArgs.CompletionPortCallback.
				this.eventHelper.RaiseSync<IOWarningEventArgs>(IOWarning, this, e);
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

		/// <summary></summary>
		[Conditional("DEBUG")]
		protected void DebugMessage(string format, params object[] args)
		{
			DebugMessage(string.Format(CultureInfo.CurrentCulture, format, args));
		}

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
		[Conditional("DEBUG_SOCKET_CONNECTIONS")]
		private void DebugSocketConnections(string format, params object[] args)
		{
			DebugMessage(format, args);
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
