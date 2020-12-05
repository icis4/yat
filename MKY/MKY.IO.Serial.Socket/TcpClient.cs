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

	// Enable debugging of state:
////#define DEBUG_STATE

	// Enable debugging of thread:
////#define DEBUG_THREADS // Attention: Must also be activated in TcpClient.Send.cs !!

	// Enable debugging of sending:
////#define DEBUG_SEND // Attention: Must also be activated in TcpClient.Send.cs !!

	// Enable debugging of a static list of all sockets:
////#define DEBUG_STATIC_SOCKET_LIST

	// Enable debugging of the ALAZ socket connection:
////#define DEBUG_SOCKET_CONNECTION

	// Enable debugging of the ALAZ socket shutdown:
////#define DEBUG_SOCKET_SHUTDOWN

	// Enable debugging of the AutoReconnect feature:
////#define DEBUG_AUTO_RECONNECT

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
using System.Threading.Tasks;

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
	/// non-blocking, by calling Stop() asynchronously and by carefully handle the 'OnDisconnected'
	/// and 'OnException' events while stopping.
	///
	/// These two issues were also reported back to Andre Luis Azevedo. But unfortunately ALAZ seems
	/// to have come to a dead end. An alternative to ALAZ might need to be found in the future.
	///
	/// Note that the very same issue existed in <see cref="TcpServer"/>.
	///
	/// Also note that a very similar issue existed when stopping two <see cref="TcpAutoSocket"/>
	/// that were interconnected with each other. See remarks of that class for details.
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
			WaitingForOrTryingReconnecting, // Intentionally using a single state, there shall not be two 'IOChanged' events each reconnect interval.
			Connected,
			Disconnected,
			Stopping,
			Error
		}

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const int SendQueueFixedCapacity       = ALAZEx.MessageBufferSizeDefault;
		private const int DataSentQueueInitialCapacity = ALAZEx.MessageBufferSizeDefault;

		private const int DefaultConnectingTimeout = 5000; // Best guess... Same as 'MKY.IO.Serial.Socket.Test.Utilities.WaitTimeoutForStateChange'.

		internal const int SocketStopTimeout = 1000; // Best guess... It just applies in case of a deadlock inside the ALAZ socket.
		private const int ThreadWaitTimeout = 500; // Enough time to let the threads join...

		private const string Undefined = "(undefined)"; // Lower case same as "localhost".

		#endregion

		#region Static Fields
		//==========================================================================================
		// Static Fields
		//==========================================================================================

		private static Random staticRandom = new Random(RandomEx.NextRandomSeed());

	#if (DEBUG_STATIC_SOCKET_LIST)
		private static List<ALAZ.SystemEx.NetEx.SocketsEx.SocketClient> staticSocketList = new List<ALAZ.SystemEx.NetEx.SocketsEx.SocketClient>();
	#endif

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
		private IntervalSettingTuple autoReconnect;

		private SocketState state = SocketState.Reset;
		private int stateCount; // = 0;
		private bool stateTokenForStopAndDispose; // = false;
		private SocketState stateIntendedAfterStopAndDispose;
		private object stateSyncObj = new object();

		private ALAZ.SystemEx.NetEx.SocketsEx.SocketClient socket;
		private object socketSyncObj = new object();

		private ALAZ.SystemEx.NetEx.SocketsEx.ISocketConnection socketConnection;
		private object socketConnectionSyncObj = new object();

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
		private Queue<byte> dataSentQueue = new Queue<byte>(DataSentQueueInitialCapacity);
		private bool dataSentThreadRunFlag;
		private AutoResetEvent dataSentThreadEvent;
		private Thread dataSentThread;
		private object dataSentThreadSyncObj = new object();

		private Timer connectingTimeout;
		private object connectingTimeoutSyncObj = new object();

		private Timer reconnectDelay;
		private object reconnectDelaySyncObj = new object();

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

		/// <summary>Creates a TCP/IP client.</summary>
		/// <remarks>This overload creates a TCP/IP client with <see cref="AutoReconnect"/> disabled.</remarks>
		/// <exception cref="ArgumentException"><paramref name="remoteHost"/> is <see cref="IPHost.Explicit"/>.</exception>
		/// <exception cref="ArgumentException"><paramref name="localInterface"/> is <see cref="IPNetworkInterface.Explicit"/>.</exception>
		/// <exception cref="ArgumentException">Mismatching <see cref="System.Net.Sockets.AddressFamily"/> of <paramref name="remoteHost"/> and <paramref name="localInterface"/>.</exception>
		public TcpClient(IPHost remoteHost, int remotePort, IPNetworkInterface localInterface)
			: this((IPHostEx)remoteHost, remotePort, (IPNetworkInterfaceEx)localInterface)
		{
		}

		/// <summary>Creates a TCP/IP client.</summary>
		/// <remarks>This overload creates a TCP/IP client with <see cref="AutoReconnect"/> disabled.</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="remoteHost"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="localInterface"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException">Mismatching <see cref="System.Net.Sockets.AddressFamily"/> of <paramref name="remoteHost"/> and <paramref name="localInterface"/>.</exception>
		public TcpClient(IPHostEx remoteHost, int remotePort, IPNetworkInterfaceEx localInterface)
			: this(SocketBase.NextInstanceId, remoteHost, remotePort, localInterface)
		{
		}

		/// <summary>Creates a TCP/IP client.</summary>
		/// <remarks>This overload creates a TCP/IP client with <see cref="AutoReconnect"/> disabled.</remarks>
		/// <exception cref="ArgumentException"><paramref name="remoteHost"/> is <see cref="IPHost.Explicit"/>.</exception>
		/// <exception cref="ArgumentException"><paramref name="localInterface"/> is <see cref="IPNetworkInterface.Explicit"/>.</exception>
		/// <exception cref="ArgumentException">Mismatching <see cref="System.Net.Sockets.AddressFamily"/> of <paramref name="remoteHost"/> and <paramref name="localInterface"/>.</exception>
		public TcpClient(int instanceId, IPHost remoteHost, int remotePort, IPNetworkInterface localInterface)
			: this(instanceId, (IPHostEx)remoteHost, remotePort, (IPNetworkInterfaceEx)localInterface)
		{
		}

		/// <summary>Creates a TCP/IP client.</summary>
		/// <remarks>This overload creates a TCP/IP client with <see cref="AutoReconnect"/> disabled.</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="remoteHost"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="localInterface"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException">Mismatching <see cref="System.Net.Sockets.AddressFamily"/> of <paramref name="remoteHost"/> and <paramref name="localInterface"/>.</exception>
		public TcpClient(int instanceId, IPHostEx remoteHost, int remotePort, IPNetworkInterfaceEx localInterface)
			: this(instanceId, remoteHost, remotePort, localInterface, new IntervalSettingTuple())
		{
		}

		/// <summary>Creates a TCP/IP client.</summary>
		/// <exception cref="ArgumentException"><paramref name="remoteHost"/> is <see cref="IPHost.Explicit"/>.</exception>
		/// <exception cref="ArgumentException"><paramref name="localInterface"/> is <see cref="IPNetworkInterface.Explicit"/>.</exception>
		/// <exception cref="ArgumentException">Mismatching <see cref="System.Net.Sockets.AddressFamily"/> of <paramref name="remoteHost"/> and <paramref name="localInterface"/>.</exception>
		public TcpClient(IPHost remoteHost, int remotePort, IPNetworkInterface localInterface, IntervalSettingTuple autoReconnect)
			: this((IPHostEx)remoteHost, remotePort, (IPNetworkInterfaceEx)localInterface, autoReconnect)
		{
		}

		/// <summary>Creates a TCP/IP client.</summary>
		/// <exception cref="ArgumentNullException"><paramref name="remoteHost"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="localInterface"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException">Mismatching <see cref="System.Net.Sockets.AddressFamily"/> of <paramref name="remoteHost"/> and <paramref name="localInterface"/>.</exception>
		public TcpClient(IPHostEx remoteHost, int remotePort, IPNetworkInterfaceEx localInterface, IntervalSettingTuple autoReconnect)
			: this(SocketBase.NextInstanceId, remoteHost, remotePort, localInterface, autoReconnect)
		{
		}

		/// <summary>Creates a TCP/IP client.</summary>
		/// <exception cref="ArgumentException"><paramref name="remoteHost"/> is <see cref="IPHost.Explicit"/>.</exception>
		/// <exception cref="ArgumentException"><paramref name="localInterface"/> is <see cref="IPNetworkInterface.Explicit"/>.</exception>
		/// <exception cref="ArgumentException">Mismatching <see cref="System.Net.Sockets.AddressFamily"/> of <paramref name="remoteHost"/> and <paramref name="localInterface"/>.</exception>
		public TcpClient(int instanceId, IPHost remoteHost, int remotePort, IPNetworkInterface localInterface, IntervalSettingTuple autoReconnect)
			: this(instanceId, (IPHostEx)remoteHost, remotePort, (IPNetworkInterfaceEx)localInterface, autoReconnect)
		{
		}

		/// <summary>Creates a TCP/IP client.</summary>
		/// <exception cref="ArgumentNullException"><paramref name="remoteHost"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="localInterface"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException">Mismatching <see cref="System.Net.Sockets.AddressFamily"/> of <paramref name="remoteHost"/> and <paramref name="localInterface"/>.</exception>
		public TcpClient(int instanceId, IPHostEx remoteHost, int remotePort, IPNetworkInterfaceEx localInterface, IntervalSettingTuple autoReconnect)
		{
			// Assert by-reference arguments:

			if (remoteHost     == null) throw (new ArgumentNullException("remoteHost",     MessageHelper.InvalidExecutionPreamble + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			if (localInterface == null) throw (new ArgumentNullException("localInterface", MessageHelper.InvalidExecutionPreamble + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			// All arguments are defined!

			if (remoteHost.Address.AddressFamily != localInterface.Address.AddressFamily) // Do not prepend/append 'SubmitBug' as an application could rely and the error message.
				throw (new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Mismatching address families! Remote host is {0} while local interface is {1}.", remoteHost.Address.AddressFamily, localInterface.Address.AddressFamily)));

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
		[SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "sendThreadEvent", Justification = "Disposed of asynchronously via 'DoStopAndDisposeAsync()'.")]
		[SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "dataSentThreadEvent", Justification = "Disposed of asynchronously via 'DoStopAndDisposeAsync()'.")]
		[SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "socket", Justification = "Disposed of asynchronously via 'DoStopAndDisposeAsync()'.")]
		[SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "isStoppingAndDisposingLock", Justification = "See comments below.")]
		protected override void Dispose(bool disposing)
		{
			if (this.eventHelper != null) // Possible when called by finalizer (non-deterministic).
				this.eventHelper.DiscardAllEventsAndExceptions();

			// Dispose of managed resources:
			if (disposing)
			{
				DebugMessage("Disposing...");

				// In the 'normal' case, the items have already been disposed of, e.g. in Stop() or OnDisconnected().
				DoDispose();

				// Do not dispose of state and shutdown locks because that will result in null
				// ref exceptions during closing, due to the fact that ALAZ closes/disconnects
				// asynchronously! No better solution has been found to this issue. And, who
				// cares if these two locks don't get disposed (except FxCop ;-).

				DebugMessage("...successfully disposed.");
			}

		////base.Dispose(disposing) of 'DisposableBase' doesn't need and cannot be called since abstract.
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

		/// <remarks>Convenience method, always returns a valid value, at least "(undefined)".</remarks>
		protected virtual string RemoteHostEndpointString
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				return ((RemoteHost != null) ? (RemoteHost.ToEndpointAddressString()) : Undefined);
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
		public virtual IntervalSettingTuple AutoReconnect
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
					case SocketState.Stopping: // Shall not be considered 'Started' as it is already on the way to 'Stopped'. This logic is also required for 'AutoSocket.StopClientSyncAndDisposeSynchronized()'!
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
		public virtual int ConnectionCount
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				int count;

				lock (this.socketConnectionSyncObj) // Directly locking the list is OK, it is kept throughout the lifetime of an object.
				{
					count = ((socketConnection != null) ? (1) : (0));
				}

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

		/// <remarks>
		/// The MKY sockets start synchronously, but the underlying ALAZ sockets will start
		/// asynchronously. Thus, after return of this method, the underlying ALAZ socket is yet
		/// about starting.
		/// </remarks>
		public virtual bool Start()
		{
		////AssertUndisposed() is called by 'IsStopped' below.
			                                  // Ensure state is handled atomically. Not using
			Monitor.Enter(this.stateSyncObj); // lock() for being able to selectively release.
			if (IsStopped)
			{
				DebugMessage("Resolving host address...");
				if (this.remoteHost.TryResolve())
				{
					DateTime timeStamp;
					SetStateSynchronized(SocketState.Connecting, out timeStamp, notify: false); // Notify outside lock!
					Monitor.Exit(this.stateSyncObj);
					NotifyStateHasChanged(timeStamp); // Notify outside lock! Has changed for sure.

					DebugMessage("...starting...");
					return (DoStart());
				}
				else
				{
					DateTime timeStamp;
					SetStateSynchronized(SocketState.Error, out timeStamp, notify: false); // Notify outside lock!
					Monitor.Exit(this.stateSyncObj);
					NotifyStateHasChanged(timeStamp); // Notify outside lock! Has changed for sure.

					DebugMessage("...failed");
					OnIOError(new IOErrorEventArgs(ErrorSeverity.Severe, "Failed to resolve host address of " + this.remoteHost.ToString() + "!"));
					return (false);
				}
			}
			else
			{
				DebugMessage("Start() requested but state already is {0}.", GetStateSynchronized());
				Monitor.Exit(this.stateSyncObj);
				return (true); // Return 'true' since socket is already started.
			}
		}

		private bool DoStart()
		{
			if (AutoReconnectIsEnabledAndAllowed)
			{
				StartConnecting();
				return (true);
			}
			else
			{
				if (TryCreateAndStartSocketAndThreads())
				{
					return (true);
				}
				else
				{
					SetStateSynchronized(SocketState.Error, notify: true);
					return (false);
				}
			}
		}

		private void StartConnecting()
		{
			if (!TryCreateAndStartSocketAndThreads())
				StartConnectingTimeout(); // Will try again after timeout.
		}

		/// <remarks>
		/// Opposed to the ALAZ sockets, the MKY sockets stop asynchronously, for the reason
		/// described in the header of this class. Thus, after return of this method, this
		/// socket as well as the underlying ALAZ socket is yet about stopping.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "'Stop' is a common term to start/stop something.")]
		public virtual void Stop()
		{
		////AssertUndisposed() is called by 'IsStarted' below.
			                                  // Ensure state is handled atomically. Not using
			Monitor.Enter(this.stateSyncObj); // lock() for being able to selectively release.
			if (IsStarted)
			{
				// Check whether token is available:
				if (!this.stateTokenForStopAndDispose)
				{
					DateTime timeStamp;
					int stateCountExpectedOnAsyncCallback;
					SetStateSynchronized(SocketState.Stopping, out timeStamp, out stateCountExpectedOnAsyncCallback, notify: false); // Notify outside lock!
					this.stateTokenForStopAndDispose = true; // Take token inside lock!
					Monitor.Exit(this.stateSyncObj);
					NotifyStateHasChanged(timeStamp); // Notify outside lock! Has changed for sure.

					DebugMessage("Invoking Stop()..."); // Not outputting "Stopping..." same as not outputting "Disposing...".
					DoStopAndDisposeAsync(stateCountExpectedOnAsyncCallback, SocketState.Reset, notify: true); // Must by async when called from main thread or ALAZ event callback! See remarks of the header of this class for details.
				}
				else
				{
					DebugMessage("Stop() requested while shutdown is already ongoing.");
					Monitor.Exit(this.stateSyncObj);
				}
			}
			else
			{
				DebugMessage("Stop() requested but state already is {0}.", GetStateSynchronized());
				Monitor.Exit(this.stateSyncObj);
			}
		}

		/// <summary></summary>
		protected virtual void DoDispose()
		{                                     // Ensure state is handled atomically. Not using
			Monitor.Enter(this.stateSyncObj); // lock() for being able to selectively release.
			if (!IsStopped) // No need for async disposal if socket is already stopped.
			{
				// Check whether token is available:
				if (!this.stateTokenForStopAndDispose)
				{
					DateTime timeStamp;
					int stateCountExpectedOnAsyncCallback;
					SetStateSynchronized(SocketState.Stopping, out timeStamp, out stateCountExpectedOnAsyncCallback, notify: false); // Without notify!
					this.stateTokenForStopAndDispose = true; // Take token inside lock!
					Monitor.Exit(this.stateSyncObj);

					DebugMessage("Invoking Dispose()..."); // Not outputting "Disposing..." as that message will be output by Dispose(bool).
					DoStopAndDisposeAsync(stateCountExpectedOnAsyncCallback, SocketState.Reset, notify: false); // Without notify! Must by async when called from main thread or ALAZ event callback! See remarks of the header of this class for details.
				}
				else
				{
					DebugMessage("Dispose() requested while shutdown is already ongoing.");
					Monitor.Exit(this.stateSyncObj);
				}
			}
			else
			{
				DebugMessage("Dispose() requested but is not needed anymore as state already is " + GetStateSynchronized() + ".");
				Monitor.Exit(this.stateSyncObj);
			}
		}

		private void DoStopAndDisposeAsync(int stateCountExpectedOnAsyncCallback, SocketState stateIntendedAfterStopAndDispose, bool notify)
		{
			DebugSocketShutdown("Stopping timers...");
			StopConnectingTimeout();
			StopReconnectDelay();

			// Stop and dispose of ALAZ socket in any case. A new socket will be created on next Start().
			DebugSocketShutdown("...and socket and threads async...");
			StopAndDisposeSocketAndThreadsAsync(stateCountExpectedOnAsyncCallback, stateIntendedAfterStopAndDispose, notify); // Must by async when called from main thread or ALAZ event callback! See remarks of the header of this class for details.
		}

		private void DoStopAndDisposeSync(SocketState stateIntendedAfterStopAndDispose, bool notify)
		{
			DebugSocketShutdown("Stopping timers...");
			StopConnectingTimeout();
			StopReconnectDelay();

			// Stop and dispose of ALAZ socket in any case. A new socket will be created on next Start().
			DebugSocketShutdown("...and socket and threads sync...");
			StopAndDisposeSocketAndThreadsSync(stateIntendedAfterStopAndDispose, notify);
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
			DateTime timeStamp;
			return (SetStateSynchronized(state, out timeStamp, notify));
		}

		private bool SetStateSynchronized(SocketState state, out DateTime timeStamp, bool notify)
		{
			int stateCount;
			return (SetStateSynchronized(state, out timeStamp, out stateCount, notify));
		}

		private bool SetStateSynchronized(SocketState state, out DateTime timeStamp, out int stateCount, bool notify)
		{
			SocketState oldState;

			lock (this.stateSyncObj)
			{
				oldState = this.state;
				this.state = state;
				timeStamp = DateTime.Now; // Inside lock for accuracy.

				unchecked {
					this.stateCount++; // Loop-around is OK.
				}
				stateCount = this.stateCount;

			#if (DEBUG) // Inside lock to prevent potential mixup in debug output.
				if (state != oldState)
					DebugMessage("State has changed from {0} to {1}.", oldState, state);
				else
					DebugState("State already is {0}.", oldState); // State non-changes shall only be output when explicitly activated.
			#endif
			}

			if (notify && (state != oldState)) // Outside lock is OK, only stating change, not state.
				NotifyStateHasChanged(timeStamp);

			return (state != oldState);
		}

		private void NotifyStateHasChanged(DateTime timeStamp)
		{
			OnIOChanged(new EventArgs<DateTime>(timeStamp));
		}

		#endregion

		#region Socket Methods
		//==========================================================================================
		// Socket Methods
		//==========================================================================================

		/// <remarks>
		/// Note that ALAZ sockets start asynchronously, same as stopping.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		private bool TryCreateAndStartSocketAndThreads()
		{
			StartThreads();

			try
			{
				lock (this.socketSyncObj)
				{
					this.socket = new ALAZ.SystemEx.NetEx.SocketsEx.SocketClient
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

				#if (DEBUG_STATIC_SOCKET_LIST)
					staticSocketList.Add(this.socket);
				#endif

					this.socket.AddConnector("MKY.IO.Serial.Socket.TcpClient", new System.Net.IPEndPoint(this.remoteHost.Address, this.remotePort), new System.Net.IPEndPoint(this.localInterface.Address, 0));
					this.socket.Start(); // The ALAZ socket will be started asynchronously.
				}
			}
			catch (Exception ex)
			{
				StopThreads(); // Unwind.

				var message = "Failed to start TCP/IP client " + ToShortEndPointString() + "! Socket error message: " + ex.Message;
			#if (DEBUG_STATE)
				DebugEx.WriteException(GetType(), ex, message); // This exception is expected and handled, thus shall only be written if configured.
			#endif
				OnIOError(new IOErrorEventArgs(ErrorSeverity.Severe, message));
				return (false);
			}

			StartConnectingTimeout(); // Good enough to start timer after invoking Start().

			return (true);
		}

		/// <remarks>
		/// See remarks of the header of this class for details.
		/// </remarks>
		private void StopAndDisposeSocketAndThreadsAsync(int stateCountExpectedOnAsyncCallback, SocketState stateIntendedAfterStopAndDispose, bool notify)
		{
			var asyncInvoker = new Action<int, SocketState, bool>(StopAndDisposeSocketAndThreadsAsyncCallback);
			asyncInvoker.BeginInvoke(stateCountExpectedOnAsyncCallback, stateIntendedAfterStopAndDispose, notify, null, null);

			Thread.Sleep(0); // Actively yield to other threads to prioritize async stopping.
		}

		private void StopAndDisposeSocketAndThreadsAsyncCallback(int stateCountExpectedOnAsyncCallback, SocketState stateIntendedAfterStopAndDispose, bool notify)
		{
		////if (!IsUndisposed) must not be checked for, as this async call will also be invoked by Dispose()!
		////	return;

			lock (this.stateSyncObj) // Ensure state is handled atomically.
			{
				// Skip if state no longer matches:
				if (!this.stateTokenForStopAndDispose)
				{
					DebugSocketShutdown("StopAndDisposeSocketAndThreadsAsyncCallback() determined to not stop and dispose because token is no longer signalled.");
					return;
				}

				// Skip if state count no longer matches:
				if (this.stateCount != stateCountExpectedOnAsyncCallback)
				{
					DebugSocketShutdown("StopAndDisposeSocketAndThreadsAsyncCallback() determined to not stop and dispose because state count does not match.");
					return;
				}

				this.stateIntendedAfterStopAndDispose = stateIntendedAfterStopAndDispose;
			}

			// Outside lock! Stop() may invoke event callbacks, potentially resulting in a deadlock!
			StopSocketAndThreadsAndDisposeSocketAndTriggerAutoReconnectOrNotifyErrorAsGiven(notify, reconnectWithDelay: true, notifyErrorIfNoReconnect: false); // OnDisconnected() and OnException() shall not immediately reconnect.
		}

		private void StopAndDisposeSocketAndThreadsSync(SocketState stateIntendedAfterStopAndDispose, bool notify)
		{
			lock (this.stateSyncObj) // Ensure state is handled atomically.
			{
				this.stateIntendedAfterStopAndDispose = stateIntendedAfterStopAndDispose;
			}

			// Outside lock! Stop() may invoke event callbacks, potentially resulting in a deadlock!
			StopSocketAndThreadsAndDisposeSocketAndTriggerAutoReconnectOrNotifyErrorAsGiven(notify, reconnectWithDelay: false, notifyErrorIfNoReconnect: true); // connectingTimeout_OneShot_Elapsed() has already delayed.
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		private void StopSocketAndThreadsAndDisposeSocketAndTriggerAutoReconnectOrNotifyErrorAsGiven(bool notify, bool reconnectWithDelay, bool notifyErrorIfNoReconnect)
		{
			lock (this.socketSyncObj)
			{
				if (this.socket != null)
				{
					// socket.Stop() sometimes deadlock at:
					//
					//  > ALAZ.SystemEx.NetEx.SocketsEx.SocketClient.Stop()
					//  > ALAZ.SystemEx.NetEx.SocketsEx.BaseSocketConnectionHost.StopConnections()
					//     > FWaitConnectionsDisposing.WaitOne(Timeout.Infinite, false);
					//
					// The root cause is (yet) unknown. Working around this issue by a timeout:

					DebugSocketShutdown("...stopping socket...");

					var stopTask = Task.Factory.StartNew(() =>
					{
						try
						{                       // Attention:
							this.socket.Stop(); // ALAZ sockets stop synchronously and may deadlock (see above)!
						}                       // ALAZ sockets don't stop on Dispose()!
						catch (Exception ex)
						{
							DebugEx.WriteException(GetType(), ex, "Stopping socket of TCP/IP client failed!");
						}
					});

					try
					{
						if (stopTask.Wait(SocketStopTimeout))
							DebugSocketShutdown("...disposing socket...");
						else
							DebugSocketShutdown("...timeout! Disposing socket...");
					}
					catch (AggregateException ex)
					{
						DebugEx.WriteException(GetType(), ex, "Stopping socket of TCP/IP client failed!");
					}

					try
					{                          // Attention:
						this.socket.Dispose(); // ALAZ sockets don't stop on Dispose()!
					}
					catch (Exception ex)
					{
						DebugEx.WriteException(GetType(), ex, "Disposing socket of TCP/IP client failed!");
					}
				}
			}

			// Finally, stop the threads. Must be done AFTER the socket got stopped (and disposed)
			// to ensure that the last socket callbacks 'OnSent' can still be properly processed.
			StopThreads();

			// And don't forget to clear the corresponding queues, its content would reappear in
			// case the socket gets started again.
			DropQueues(notify);

			// Then, decide on how to continue:
			bool stateHasChanged = false;
			DateTime timeStamp;
			bool tryReconnect = false;

			lock (this.stateSyncObj) // Ensure state is handled atomically.
			{
				// Skip if state no longer matches:
				if (!this.stateTokenForStopAndDispose)
				{
					DebugSocketShutdown("StopSocketAndThreadsAndDisposeSocketAndTriggerAutoReconnectOrNotifyErrorAsGiven() determined to break disposal because token is no longer signalled.");
					return;
				}

				this.stateTokenForStopAndDispose = false; // Release token inside lock!
				DebugSocketShutdown("...completed.");

				stateHasChanged = SetStateSynchronized(this.stateIntendedAfterStopAndDispose, out timeStamp, notify: false); // Notify outside lock!

				if (this.stateIntendedAfterStopAndDispose == SocketState.WaitingForOrTryingReconnecting)
					tryReconnect = true;
			}

			if (notify && stateHasChanged)
				NotifyStateHasChanged(timeStamp); // Notify outside lock!

			if (tryReconnect)
			{
				if (reconnectWithDelay)
					StartReconnectDelay();
				else
					StartConnecting(); // Try immediately, but outside lock!
			}
			else if (notifyErrorIfNoReconnect)
			{
				OnIOError(new IOErrorEventArgs(ErrorSeverity.Severe, "Failed to connect to " + ToShortEndPointString() + "!"));
			}
		}

		private void DropQueues(bool notify)
		{
			DropSendQueue(notify);
			DropDataSentQueue(notify);
		}

		private void DropQueues(out int droppedSendCount, out int droppedDataSentCount)
		{
			droppedSendCount     = DropSendQueue();
			droppedDataSentCount = DropDataSentQueue();
		}

		private int DropSendQueueAndNotify()
		{
			return (DropSendQueue(notify: true));
		}

		private int DropSendQueue(bool notify)
		{
			int droppedCount = DropSendQueue();
			if (notify && (droppedCount > 0))
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

		private int DropDataSentQueue(bool notify)
		{
			int droppedCount = DropDataSentQueue();
			if (notify && (droppedCount > 0))
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
			bool rejectConnection = false;
			bool stateHasChanged = false;
			DateTime timeStamp = DateTime.MinValue;

			lock (this.stateSyncObj) // Ensure state is handled atomically.
			{
				// Ensure shutdown token is not signalled:
				if (!this.stateTokenForStopAndDispose)
				{
					lock (this.socketConnectionSyncObj)
					{
						var state = GetStateSynchronized();
						if ((state == SocketState.Connecting) || (state == SocketState.WaitingForOrTryingReconnecting)) // Only handle when expected.
						{
							StopConnectingTimeout();

							if (this.socketConnection == null)
							{
								this.socketConnection = e.Connection;
								acceptConnection = true;
								stateHasChanged = SetStateSynchronized(SocketState.Connected, out timeStamp, notify: false); // Notify outside lock!

								DebugSocketConnection("Connection (ID = {0}, remote endpoint = {1}, local endpoint = {2}) set.", e.Connection.ConnectionId, ToDebugString(e.Connection.RemoteEndPoint), ToDebugString(e.Connection.LocalEndPoint));
							}
							else // Such stray event callbacks may e.g. happen on 'AutoReconnect'.
							{
								DebugMessage("Ignoring stray 'OnConnected' event callback (connection ID = {0}, remote endpoint = {1}, local endpoint = {2}) as connection has already been accepted.", e.Connection.ConnectionId, ToDebugString(e.Connection.RemoteEndPoint), ToDebugString(e.Connection.LocalEndPoint));
							}
						}
						else // Such stray event callbacks should never happen.
						{
							rejectConnection = true;

							DebugMessage("Rejecting stray 'OnConnected' event callback (connection ID = {0}, remote endpoint = {1}, local endpoint = {2}) while state was neither 'Connecting' nor 'WaitingForOrTryingReconnecting'.", e.Connection.ConnectionId, ToDebugString(e.Connection.RemoteEndPoint), ToDebugString(e.Connection.LocalEndPoint));
						}
					}
				}
				else // Such event callbacks e.g. have to be expected on 'AutoReconnect' due to the async timers.
				{
					DebugMessage("Ignoring 'OnConnected' event callback (connection ID = {0}, remote endpoint = {1}, local endpoint = {2}) as shutdown is ongoing.", e.Connection.ConnectionId, ToDebugString(e.Connection.RemoteEndPoint));
				}
			}

			if (acceptConnection)
				e.Connection.BeginReceive(); // Immediately begin receiving, outside lock!

			if (rejectConnection)
				e.Connection.BeginDisconnect(); // Immediately begin disconnecting, outside lock!

			if (stateHasChanged)
				NotifyStateHasChanged(timeStamp); // Notify outside lock!
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

			lock (this.stateSyncObj)
			{
				if (this.stateTokenForStopAndDispose) // Such event callbacks e.g. have to be expected on 'AutoReconnect' due to the async timers.
				{
					DebugMessage("Ignoring 'OnReceived' event callback (connection ID = {0}, remote endpoint = {1}, local endpoint = {2}) as shutdown is ongoing.", e.Connection.ConnectionId, ToDebugString(e.Connection.RemoteEndPoint), ToDebugString(e.Connection.LocalEndPoint));
					return;
				}
			}

			lock (this.socketConnectionSyncObj)
			{
				if (e.Connection != this.socketConnection) // Such stray event callbacks may e.g. happen on 'AutoReconnect'.
				{
					DebugMessage("Ignoring stray 'OnReceived' event callback (connection ID = {0}, remote endpoint = {1}, local endpoint = {2}).", e.Connection.ConnectionId, ToDebugString(e.Connection.RemoteEndPoint), ToDebugString(e.Connection.LocalEndPoint));
					return;
				}
			}

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

			lock (this.stateSyncObj)
			{
				if (this.stateTokenForStopAndDispose) // Such event callbacks e.g. have to be expected on 'AutoReconnect' due to the async timers.
				{
					DebugMessage("Ignoring 'OnSent' event callback (connection ID = {0}, remote endpoint = {1}, local endpoint = {2}) as shutdown is ongoing.", e.Connection.ConnectionId, ToDebugString(e.Connection.RemoteEndPoint), ToDebugString(e.Connection.LocalEndPoint));
					return;
				}
			}

			lock (this.socketConnectionSyncObj)
			{
				if (e.Connection != this.socketConnection) // Such stray event callbacks may e.g. happen on 'AutoReconnect'.
				{
					DebugMessage("Ignoring stray 'OnSent' event callback (connection ID = {0}, remote endpoint = {1}, local endpoint = {2}).", e.Connection.ConnectionId, ToDebugString(e.Connection.RemoteEndPoint), ToDebugString(e.Connection.LocalEndPoint));
					return;
				}
			}

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

			lock (this.socketConnectionSyncObj)
			{
				if (e.Connection != this.socketConnection) // Such stray event callbacks may e.g. happen on 'AutoReconnect'.
				{
					DebugMessage("Ignoring stray 'OnDisconnected' event callback (connection ID = {0}, remote endpoint = {1}, local endpoint = {2}).", e.Connection.ConnectionId, ToDebugString(e.Connection.RemoteEndPoint), ToDebugString(e.Connection.LocalEndPoint));
					return;
				}

				if (this.socketConnection != null)
				{
					this.socketConnection = null;

					DebugSocketConnection("Connection reset (connection ID = {0}, remote endpoint = {1}, local endpoint = {2}).", e.Connection.ConnectionId, ToDebugString(e.Connection.RemoteEndPoint), ToDebugString(e.Connection.LocalEndPoint));
				}
			}

			// Check whether token is available. Ensure state is handled atomically. Not using
			Monitor.Enter(this.stateSyncObj); // lock() for being able to selectively release.
			if (!this.stateTokenForStopAndDispose)
			{
				DebugSocketShutdown("'OnDisconnected' event event callback determined to stop and dispose" + (AutoReconnectIsEnabledAndAllowed ? " and then try reconnecting." : "."));

				// Ensure to set the temporary and intended state based on current state, i.e. before potentially setting 'Disconnected' below:
				var stateIntended  = (AutoReconnectIsEnabledAndAllowed ? SocketState.WaitingForOrTryingReconnecting : SocketState.Reset);
				var stateTemporary = (AutoReconnectIsEnabledAndAllowed ? SocketState.WaitingForOrTryingReconnecting : SocketState.Disconnected); // Temporary until Stop() has completed.
				                  // No need to change state twice, immediately set 'WaitingForOrTryingReconnecting'.
				DateTime timeStamp;
				int stateCountExpectedOnAsyncCallback;
				var stateHasChanged = SetStateSynchronized(stateTemporary, out timeStamp, out stateCountExpectedOnAsyncCallback, notify: false); // Notify outside lock!
				this.stateTokenForStopAndDispose = true; // Take token inside lock!
				Monitor.Exit(this.stateSyncObj);
				if (stateHasChanged)
					NotifyStateHasChanged(timeStamp); // Notify outside lock!

				// Stop and dispose in any case. A new socket will be created on 'AutoReconnect' or next Start().
				DoStopAndDisposeAsync(stateCountExpectedOnAsyncCallback, stateIntended, notify: true); // Must by async when called from main thread or ALAZ event callback! See remarks of the header of this class for details.
			}                                                                        // Notify is OK as async and no longer inside lock.
			else
			{
				DebugSocketShutdown("Ignoring 'OnDisconnected' event event callback as shutdown is already ongoing.");

				Monitor.Exit(this.stateSyncObj);
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
			if (!IsUndisposed) // Ignore async callbacks during closing.
				return;

			lock (this.socketConnectionSyncObj)
			{
				if (e.Connection != this.socketConnection) // Such stray event callbacks may e.g. happen on 'AutoReconnect'.
				{
					DebugMessage("Ignoring stray 'OnException' event callback (connection ID = {0}, remote endpoint = {1}, local endpoint = {2}).", e.Connection.ConnectionId, ToDebugString(e.Connection.RemoteEndPoint), ToDebugString(e.Connection.LocalEndPoint));
					return;
				}

				if (this.socketConnection != null)
				{
					this.socketConnection = null;

					DebugSocketConnection("Connection reset.");
				}
			}

			// Check whether token is available. Ensure state is handled atomically. Not using
			Monitor.Enter(this.stateSyncObj); // lock() for being able to selectively release.
			if (!this.stateTokenForStopAndDispose)
			{
				DebugSocketShutdown("'OnException' event event callback determined to stop and dispose" + (AutoReconnectIsEnabledAndAllowed ? " and then try reconnecting." : "."));

				// Ensure to set the intended state based on current state, i.e. before potentially setting 'Stopping' below:
				var stateIntended  = (AutoReconnectIsEnabledAndAllowed ? SocketState.WaitingForOrTryingReconnecting : SocketState.Error);
				var stateTemporary = (AutoReconnectIsEnabledAndAllowed ? SocketState.WaitingForOrTryingReconnecting : SocketState.Stopping); // Temporary until Stop() has completed.
				                  // No need to change state twice, immediately set 'WaitingForOrTryingReconnecting'.
				DateTime timeStamp;
				int stateCountExpectedOnAsyncCallback;
				var stateHasChanged = SetStateSynchronized(stateTemporary, out timeStamp, out stateCountExpectedOnAsyncCallback, notify: false); // Notify outside lock!
				this.stateTokenForStopAndDispose = true; // Take token inside lock!
				Monitor.Exit(this.stateSyncObj);
				if (stateHasChanged)
					NotifyStateHasChanged(timeStamp); // Notify outside lock!

				// Stop and dispose in any case. A new socket will be created on 'AutoReconnect' or next Start().
				DoStopAndDisposeAsync(stateCountExpectedOnAsyncCallback, stateIntended, notify: true); // Must by async when called from main thread or ALAZ event callback! See remarks of the header of this class for details.
				                                                                   //// Notify is OK as async and no longer inside lock.
				if (stateIntended != SocketState.WaitingForOrTryingReconnecting) // No need to notify errors that are expected.
					NotifyError(e.Exception); // Notify after triggering 'StopAndDispose'! This sequence is required for not deadlocking in 'AutoSocket.StopClientSyncAndDisposeSynchronized()'!
			}
			else
			{
				DebugSocketShutdown("Ignoring 'OnException' event event callback as shutdown is already ongoing.");

				Monitor.Exit(this.stateSyncObj);
			}
		}

		private void NotifyError(Exception ex)
		{
			if (ex is ALAZ.SystemEx.NetEx.SocketsEx.ReconnectAttemptException)
			{
				var sb = new StringBuilder();
				sb.Append("Could not connect to TCP/IP server ");
				sb.Append(this.remoteHost.Address);
				sb.Append(":");
				sb.Append(this.remotePort);
				sb.Append("."); // Not "!" as this is 'Acceptable'.

				// Appending "Socket error message: " + ex.Message makes no sense as it always is "Reconnect attempt".

				if (ex.InnerException != null)
				{
					var socketException = (ex.InnerException as System.Net.Sockets.SocketException);
					if (socketException != null)
					{
						sb.Append(" Socket error code: ");
						sb.Append(socketException.NativeErrorCode);
					}
				}

				var message = sb.ToString();              // Acceptable as this is an expected situation when a server is not available.
				OnIOError(new IOErrorEventArgs(ErrorSeverity.Acceptable, message));
			}
			else
			{
				var sb = new StringBuilder();
				sb.AppendLine("The socket of this TCP/IP client has thrown an exception!");
				sb.AppendLine();
				sb.AppendLine("Exception type:");
				sb.AppendLine(ex.GetType().FullName);
				sb.AppendLine();
				sb.AppendLine("Exception error message:");
				sb.AppendLine(ex.Message);

				var message = sb.ToString();
				DebugMessage(message);
				OnIOError(new IOErrorEventArgs(ErrorSeverity.Fatal, message));
			}
		}

		/// <remarks>
		/// There must not be an additional <code>&amp;&amp; !IsOpen</code> check because 'state'
		/// will still be 'Connected' when this property gets checked by the callbacks above. And
		/// just setting 'state' to 'Disconnected' in the callbacks above makes no sense as it would
		/// result in an additional unnecessary state change events.
		/// </remarks>
		private bool AutoReconnectIsEnabledAndAllowed
		{
			get { return (IsUndisposed && IsStarted && AutoReconnect.Enabled); } // Check disposal state first!
		}

		#endregion

		#region Socket Threads
		//==========================================================================================
		// Socket Threads
		//==========================================================================================

		private void StartThreads()
		{
			DebugThreads("SendThread() gets started...");

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

			DebugThreads("DataSentThread() gets started...");

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

					DebugThreads("...SendThread() gets stopped...");

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
								DebugThreads("...failed! Aborting...");
								DebugThreads("(Abort is likely required due to failed synchronization back the calling thread, which is typically the main thread.)");

								isAborting = true;       // Thread.Abort() must not be used whenever possible!
								this.sendThread.Abort(); // This is only the fall-back in case joining fails for too long.
								break;
							}

							DebugThreads("...trying to join at " + accumulatedTimeout + " ms...");
						}

						if (!isAborting)
							DebugThreads("...successfully stopped.");
					}
					catch (ThreadStateException)
					{
						// Ignore thread state exceptions such as "Thread has not been started" and
						// "Thread cannot be aborted" as it just needs to be ensured that the thread
						// has or will be terminated for sure.

						DebugThreads("...failed too but will be exectued as soon as the calling thread gets suspended again.");
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

					DebugThreads("DataSentThread() gets stopped...");

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
								DebugThreads("...failed! Aborting...");
								DebugThreads("(Abort is likely required due to failed synchronization back the calling thread, which is typically the main thread.)");

								isAborting = true;           // Thread.Abort() must not be used whenever possible!
								this.dataSentThread.Abort(); // This is only the fall-back in case joining fails for too long.
								break;
							}

							DebugThreads("...trying to join at " + accumulatedTimeout + " ms...");
						}

						if (!isAborting)
							DebugThreads("...successfully stopped.");
					}
					catch (ThreadStateException)
					{
						// Ignore thread state exceptions such as "Thread has not been started" and
						// "Thread cannot be aborted" as it just needs to be ensured that the thread
						// has or will be terminated for sure.

						DebugThreads("...failed too but will be exectued as soon as the calling thread gets suspended again.");
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

		#region Connecting/Reconnect Timers
		//==========================================================================================
		// Connecting/Reconnect Timers
		//==========================================================================================

		/// <remarks>
		/// BSc revealed an issue with 'AutoReconnect' (bug #487). Manually closing/opening a
		/// connection did work fine. But 'AutoReconnect' only reconnected after approx. 20 s.
		/// Wireshark traces revealed the details:
		///
		/// 1. 500 ms after connection loss, YAT tries to reconnect:
		///    <![CDATA[Server (Device) <= SYN <= Client (YAT)]]>
		/// 2. For whatever reason, the device acknowledges but at the same time resets the connection:
		///    <![CDATA[Server (Device) => RST|ACK => Client (YAT)]]>
		/// 3. YAT's socket then correctly retransmits the paket:
		///    <![CDATA[Server (Device) <= SYN <= Client (YAT)]]>
		/// 4. Then, nothing happens for 11 seconds anymore:
		///    YAT's socket waits for acknowledge or timeout.
		///    The device does nothing anymore.
		///    <![CDATA[Server (Device) <= SYN <= Client (YAT)]]>
		/// 5. After timeout, YAT tries again:
		///    <![CDATA[Server (Device) <= SYN <= Client (YAT)]]>
		/// 6. Approx. 12 seconds later, YAT tries again:
		///    <![CDATA[Server (Device) <= SYN <= Client (YAT)]]>
		/// 7. Then device responses and YAT confirms:
		///    <![CDATA[Server (Device) => SYN|ACK => Client (YAT)]]>
		///    <![CDATA[Server (Device) <= ACK <= Client (YAT)]]>
		///
		/// However, YAT is configured to try every 500 ms again, this does not happen.
		/// The timer must check the connection after 500 ms. Now:
		///
		/// 1. 500 ms after connection loss, YAT tries to reconnect:
		///    <![CDATA[Server (Device) <= SYN <= Client (YAT)]]>
		/// 2. For whatever reason, the device acknowledges but at the same time resets the connection:
		///    <![CDATA[Server (Device) => RST|ACK => Client (YAT)]]>
		/// 3. YAT's socket then correctly retransmits the paket:
		///    <![CDATA[Server (Device) <= SYN <= Client (YAT)]]> @ port e.g. 54659
		/// 4. Now, after another 500 ms, YAT checks the state and the connection is not established yet.
		///    YAT then closes the already reset socket, i.e. nothing is sent anymore.
		///    And tries to reconnect again and again:
		///    <![CDATA[Server (Device) <= SYN <= Client (YAT)]]> @ port e.g. 54660
		///    <![CDATA[Server (Device) <= SYN <= Client (YAT)]]> @ port e.g. 54661
		///    <![CDATA[Server (Device) <= SYN <= Client (YAT)]]> @ port e.g. 54662
		///    ...
		///    <![CDATA[Server (Device) <= SYN <= Client (YAT)]]> @ port e.g. 54670
		/// 5. Finally the device responses and YAT confirms:
		///    <![CDATA[Server (Device) => SYN|ACK => Client (YAT)]]>
		///    <![CDATA[Server (Device) <= ACK <= Client (YAT)]]>
		///
		/// But attention, there are some tricky things here, again revealed by BSc and Wireshark:
		///
		/// a) While the above SYN|ACK|SYN|ACK is happening at socket level, this class may not yet
		///    have received the 'OnConnected' event callback and therefore closes the socket and
		///    tries again:
		///    <![CDATA[Server (Device) <= SYN <= Client (YAT)]]>
		///    But the device actually has received the subsequent SYN and does response:
		///    <![CDATA[Server (Device) => SYN|ACK => Client (YAT)]]>
		///    Or the device is limited to a single connection, thus rejects it:
		///    <![CDATA[Server (Device) => RST|ACK => Client (YAT)]]>
		///    In these cases, this class will receive "stray" 'OnConnected' or 'OnDisconnected'
		///    events. This class must be able to deal with such corner-cases.
		/// b) As all the above operations are async, there is uncertainty about the moment of the
		///    event callbacks.
		///
		/// Note that reconnecting is implemented by creating a new socket each time, resulting in
		/// using a new backport each time (e.g. 54659, 54660, 54661,...). If this ever becomes an
		/// issue, e.g. because a (really simple) server expects the same backport, the behavior
		/// could be made configurable, e.g. [use same socket] / [use new socket].
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		/// <remarks>
		/// Note <see cref="ALAZ.SystemEx.NetEx.SocketsEx.ClientSocketConnection.BeginReconnect"/>
		/// exists but cannot be configured. Also note the existence of this method indicates there
		/// is no such functionality in the underlying <see cref="System.Net.Sockets.Socket"/>. No
		/// such functionality found by searching MSDN nor Internet.
		/// </remarks>
		private void StartConnectingTimeout()
		{
			var dueTime = ((GetStateSynchronized() == SocketState.Connecting) ? DefaultConnectingTimeout : AutoReconnect.Interval);
			var period = Timeout.Infinite; // One-Shot!

			lock (this.connectingTimeoutSyncObj)
			{
				if (this.connectingTimeout == null)
					this.connectingTimeout = new Timer(new TimerCallback(connectingTimeout_OneShot_Elapsed), null, dueTime, period);
				else
					this.connectingTimeout.Change(dueTime, period);
			}
		}

		private void StopConnectingTimeout()
		{
			lock (this.connectingTimeoutSyncObj)
			{
				if (this.connectingTimeout != null)
				{
					this.connectingTimeout.Dispose(); // Simply Dispose(), no need to Change() first.
					this.connectingTimeout = null;
				}
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		private void connectingTimeout_OneShot_Elapsed(object obj)
		{
			if (!IsUndisposed) // Ignore async callbacks during closing.
				return;

			// Non-periodic timer, only a single callback can be active at a time.
			// There is no need to synchronize concurrent callbacks to this event handler.

			lock (this.connectingTimeoutSyncObj)
			{
				if (this.connectingTimeout == null) // Handle overdue callbacks:
					return;
			}
			                                //// Ensure state is handled atomically. Not using
			Monitor.Enter(this.stateSyncObj); // lock() for being able to selectively release.
			if (IsStarted && !IsConnected)
			{
				var state = GetStateSynchronized();
				if ((state == SocketState.Connecting) || (state == SocketState.WaitingForOrTryingReconnecting))
				{
					// Check whether token is available:
					if (!this.stateTokenForStopAndDispose)
					{
						DebugAutoReconnect("connectingTimeout_OneShot_Elapsed() determined to stop and dispose" + (AutoReconnectIsEnabledAndAllowed ? " and then try reconnecting." : "."));

						// Ensure to set the intended state based on current state, i.e. before setting 'Reset' below:
						var stateIntended  = (AutoReconnectIsEnabledAndAllowed ? SocketState.WaitingForOrTryingReconnecting : SocketState.Reset);
						var stateTemporary = (AutoReconnectIsEnabledAndAllowed ? SocketState.WaitingForOrTryingReconnecting : SocketState.Disconnected); // Temporary until Stop() has completed.
						                 // No need to change state twice, immediately set 'WaitingForOrTryingReconnecting'.
						DateTime timeStamp;
						var stateHasChanged = SetStateSynchronized(stateTemporary, out timeStamp, notify: false); // Notify outside lock!
						this.stateTokenForStopAndDispose = true; // Take token inside lock!
						Monitor.Exit(this.stateSyncObj);
						if (stateHasChanged)
							NotifyStateHasChanged(timeStamp); // Notify outside lock!

						// Outside lock! Stop() may invoke event callbacks, potentially resulting in a deadlock!
						DoStopAndDisposeSync(stateIntended, notify: true); // Sync is OK from this timer callback.
					}
					else
					{
						DebugMessage("Ignoring 'connectingTimeout_OneShot_Elapsed' callback as shutdown already is ongoing.");
						Monitor.Exit(this.stateSyncObj);
					}
				}
				else
				{
					DebugAutoReconnect("connectingTimeout_OneShot_Elapsed() determined to not stop and dispose and then try reconnecting because state no longer is 'Connecting' or 'WaitingForOrTryingReconnecting'.");
					Monitor.Exit(this.stateSyncObj);
				}
			}
			else
			{
				Monitor.Exit(this.stateSyncObj);
			}
		}

		private void StartReconnectDelay()
		{
			var dueTime = AutoReconnect.Interval;
			var period = Timeout.Infinite; // One-Shot!

			lock (this.reconnectDelaySyncObj)
			{
				if (this.reconnectDelay == null)
					this.reconnectDelay = new Timer(new TimerCallback(reconnectDelay_OneShot_Elapsed), null, dueTime, period);
				else
					this.reconnectDelay.Change(dueTime, period);
			}
		}

		private void StopReconnectDelay()
		{
			lock (this.reconnectDelaySyncObj)
			{
				if (this.reconnectDelay != null)
				{
					this.reconnectDelay.Dispose(); // Simply Dispose(), no need to Change() first.
					this.reconnectDelay = null;
				}
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		private void reconnectDelay_OneShot_Elapsed(object obj)
		{
			if (!IsUndisposed) // Ignore async callbacks during closing.
				return;

			// Non-periodic timer, only a single callback can be active at a time.
			// There is no need to synchronize concurrent callbacks to this event handler.

			lock (this.reconnectDelaySyncObj)
			{
				if (this.reconnectDelay == null) // Handle overdue callbacks:
					return;
			}
			                                //// Ensure state is handled atomically. Not using
			Monitor.Enter(this.stateSyncObj); // lock() for being able to selectively release.
			if (IsStarted && !IsConnected && AutoReconnectIsEnabledAndAllowed)
			{
				DebugSocketShutdown("reconnectDelay_OneShot_Elapsed() determined to try reconnecting.");
				Monitor.Exit(this.stateSyncObj);

				StartConnecting();
			}
			else
			{
				Monitor.Exit(this.stateSyncObj);
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
			throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The event 'IOControlChanged' is not in use for TCP/IP clients!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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

			return (RemoteHostEndpointString + ":" + RemotePort);
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
		/// Name 'DebugWriteLine' would show relation to <see cref="Debug.WriteLine(string)"/>.
		/// However, named 'Message' for compactness and more clarity that something will happen
		/// with <paramref name="message"/>, and rather than e.g. 'Common' for comprehensibility.
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
		[Conditional("DEBUG_STATE")]
		private void DebugState(string format, params object[] args)
		{
			DebugMessage(format, args);
		}

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
		[Conditional("DEBUG_SOCKET_CONNECTION")]
		private void DebugSocketConnection(string format, params object[] args)
		{
			DebugMessage(format, args);
		}

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
		/// <remarks>
		/// Named 'Shutdown' for separating from terms 'Stop' and 'Dispose' as well as compactness.
		/// </remarks>
		[Conditional("DEBUG_SOCKET_SHUTDOWN")]
		private void DebugSocketShutdown(string message)
		{
			DebugMessage(message);
		}

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
		[Conditional("DEBUG_AUTO_RECONNECT")]
		private void DebugAutoReconnect(string message)
		{
			DebugMessage(message);
		}

		/// <summary></summary>
		protected static string ToDebugString(System.Net.IPEndPoint ep)
		{
			return ((ep != null) ? (ep.ToString()) : "<undefined>");
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
