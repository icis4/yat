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
// MKY Version 1.0.25
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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
//// 'System.Net' as well as 'ALAZ.SystemEx.NetEx' are explicitly used for more obvious distinction.
using System.Threading;

using MKY.Contracts;
using MKY.Diagnostics;
using MKY.Net;

#endregion

namespace MKY.IO.Serial.Socket
{
	/// <summary>
	/// Implements the <see cref="IIOProvider"/> interface for an automatic TCP/IP client or server.
	/// On startup, an AutoSocket tries to connect to a remote server and run as client. If this
	/// fails, it tries to run as server. Retry cycles and random wait times ensure proper operation
	/// even when multiple AutoSockets try to interconnected to each other.
	/// </summary>
	/// <remarks>
	/// Initially, YAT AutoSockets with the original ALAZ implementation created a deadlock on
	/// shutdown when two AutoSockets that were interconnected with each other. The situation:
	///
	/// 1. The main thread requests stopping all terminals:
	///     => YAT.Model.Workspace.CloseAllTerminals()
	///         => MKY.IO.Serial.TcpAutoSocket.Stop()
	///            => ALAZ.SystemEx.NetEx.SocketsEx.SocketServer.Stop()
	///                => ALAZ.SystemEx.NetEx.SocketsEx.BaseSocketConnection.Active.get()
	///
	/// 2. As a result, the first AutoSocket shuts down, but the second changes from 'Accepted' to
	///    'Listening' and tries to synchronize from the ALAZ socket event to the main thread:
	///     => ALAZ.SystemEx.NetEx.SocketsEx.BaseSocketConnectionHost.FireOnDisconnected()
	///         => MKY.IO.Serial.TcpAutoSocket.OnIOChanged()
	///             => YAT.Model.Terminal.OnIOChanged()
	///                 => Deadlock when synchronizing onto main thread !!!
	///
	/// The issue has been solved in <see cref="ALAZ.SystemEx.NetEx.SocketsEx.BaseSocketConnection"/>
	/// as well as <see cref="TcpClient"/> or <see cref="TcpServer"/> by invoking Stop() and Dispose()
	/// of socket and connections and thread asynchronously and without firing events. See remarks
	/// of these classes for additional information.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "A type shall spell 'Tcp' like this...")]
	public class TcpAutoSocket : IIOProvider, IDisposable, IDisposableEx
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private enum SocketState
		{
			Reset,
			Starting,
			Connecting,
			Connected,
			ConnectingFailed,
			StartingListening,
			Listening,
			ListeningFailed,
			Accepted,
		////Restarting,
			Stopping,
			Error
		}

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const int MaxStartCycles = 3;

		private const int MinConnectDelay = 50;
		private const int MaxConnectDelay = 300;

		private const int MinListenDelay = 50;
		private const int MaxListenDelay = 300;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private int instanceId;

		/// <summary>
		/// A dedicated event helper to allow discarding exceptions when object got disposed.
		/// </summary>
		private EventHelper.Item eventHelper = EventHelper.CreateItem(typeof(TcpAutoSocket).FullName);

		private IPHostEx remoteHost;
		private int remotePort;
		private IPNetworkInterfaceEx localInterface;
		private int localPort;

		private SocketState state = SocketState.Reset;
		private object stateSyncObj = new object();

		private int startCycleCounter; // = 0;
		private object startCycleCounterSyncObj = new object();

		private TcpClient client;
		private TcpServer server;
		private object socketSyncObj = new object();

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

		/// <summary>Creates a TCP/IP AutoSocket.</summary>
		/// <exception cref="ArgumentNullException"><paramref name="remoteHost"/> is is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="localInterface"/> is is <c>null</c>.</exception>
		public TcpAutoSocket(IPHostEx remoteHost, int remotePort, IPNetworkInterfaceEx localInterface, int localPort)
		{
			if (remoteHost == null)     throw (new ArgumentNullException("remoteHost"));
			if (localInterface == null) throw (new ArgumentNullException("localInterface"));

			this.instanceId = SocketBase.NextInstanceId;

			this.remoteHost     = remoteHost;
			this.remotePort     = remotePort;
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
					DisposeSockets();
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
		~TcpAutoSocket()
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
		public virtual bool IsListening
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				switch (GetStateSynchronized())
				{
					case SocketState.Listening:
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
		public virtual bool IsConnected
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				switch (GetStateSynchronized())
				{
					case SocketState.Connected:
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
		public virtual bool IsOpen
		{
			get { return (IsConnected); }
		}

		/// <summary></summary>
		public virtual bool IsTransmissive
		{
			get { return (IsConnected); }
		}

		/// <remarks>
		/// The <see cref="IsClient"/> and <see cref="IsServer"/> properties only return <c>true</c>
		/// if it is defined whether the AutoSocket indeed behaves as client or server. If it is not
		/// yet defined, both flags are set <c>false</c>.
		/// </remarks>
		public virtual bool IsClient
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

		/// <remarks>
		/// The <see cref="IsClient"/> and <see cref="IsServer"/> properties only return <c>true</c>
		/// if it is defined whether the AutoSocket indeed behaves as client or server. If it is not
		/// yet defined, both flags are set <c>false</c>.
		/// </remarks>
		public virtual bool IsServer
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				switch (GetStateSynchronized())
				{
					case SocketState.Listening:
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

		/// <remarks>
		/// Convenience property to combine the <see cref="IsClient"/> and <see cref="IsServer"/>
		/// properties. It returns <c>true</c> if it is defined whether the AutoSocket indeed is
		/// a client or server. If it is not yet defined, it returns <c>false</c>.
		/// </remarks>
		public virtual bool IsClientOrServer
		{
			get { return (IsClient || IsServer); }
		}

		/// <summary></summary>
		public virtual object UnderlyingIOInstance
		{
			get
			{
				AssertNotDisposed();

				lock (this.socketSyncObj)
				{
					if (IsClient && (this.client != null))
						return (this.client.UnderlyingIOInstance);
					else if (IsServer && (this.server != null))
						return (this.server.UnderlyingIOInstance);
					else
						return (null);
				}
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
			AssertNotDisposed();

			SocketState state = GetStateSynchronized();
			switch (state)
			{
				case SocketState.Reset:
				case SocketState.Error:
				{
					DebugMessage("Starting...");
					StartAutoSocket();
					return (true);
				}
				default:
				{
					DebugMessage("Start() requested but state is already " + state + ".");
					return (true); // Return 'true' since socket is already started.
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "'Stop' is a common term to start/stop something.")]
		public virtual void Stop()
		{
			AssertNotDisposed();

			SocketState state = GetStateSynchronized();
			switch (state)
			{
				case SocketState.Reset:
				{
					DebugMessage("Stop() requested but state is " + state + ".");
					return;
				}
				default:
				{
					DebugMessage("Stopping...");
					StopAutoSocket();
					return;
				}
			}
		}

		/// <summary></summary>
		public virtual bool Send(byte[] data)
		{
			AssertNotDisposed();

			if (IsTransmissive)
			{
				lock (this.socketSyncObj)
				{
					if (IsClient && (this.client != null))
						return (this.client.Send(data));
					else if (IsServer && (this.server != null))
						return (this.server.Send(data));
				}
			}

			return (false);
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
			string isClientOrServerString;
			if      (IsClient && IsConnected) // 'Doppel-moppel', but keep it as a check during development and debugging
				isClientOrServerString = "is connected as client";
			else if (IsServer && IsConnected)
				isClientOrServerString = "is connected as server";
			else if (IsServer && !IsConnected)
				isClientOrServerString = "is server and listening";
			else
				isClientOrServerString = "is neither client nor server";

			if (this.state != oldState)
				DebugMessage("State has changed from " + oldState + " to " + state + ", " + isClientOrServerString + ".");
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
		/// Note that the underlying sockets perform stopping synchronously, opposed to starting
		/// that is done asynchronously. This difference is due to the issue described in the header
		/// of this class.
		/// </remarks>
		private void StopAndDisposeSockets()
		{
			StopSockets();
			DisposeSockets();
		}

		/// <remarks>
		/// Note that the underlying sockets perform stopping synchronously, opposed to starting
		/// that is done asynchronously. This difference is due to the issue described in the header
		/// of this class.
		/// </remarks>
		private void StopSockets()
		{
			lock (this.socketSyncObj)
			{
				if (this.client != null)
					this.client.Stop();

				if (this.server != null)
					this.server.Stop();
			}
		}

		private void DisposeSockets()
		{
			DisposeClient();
			DisposeServer();
		}

		#endregion

		#region AutoSocket State Transitions
		//==========================================================================================
		// AutoSocket State Transitions
		//==========================================================================================

		private void StartAutoSocket()
		{
			lock (this.startCycleCounterSyncObj)
				this.startCycleCounter = 1;

			SetStateSynchronizedAndNotify(SocketState.Starting);
			StartConnecting();
		}

		/// <summary>
		/// Try to start as client.
		/// </summary>
		private void StartConnecting()
		{
			int randomDelay = SocketBase.Random.Next(MinConnectDelay, MaxConnectDelay);
			DebugMessage("Delaying connecting by random value of " + randomDelay + " ms.");
			Thread.Sleep(randomDelay);

			// Only continue if socket is still up and running after the delay! Required because
			// re-connecting happens automatically when connection gets lost. If two AutoSockets
			// are interconnected, and both are shut down, the sequence of the operation is not
			// defined. It is likely that Stop() is called while the thread is delayed above. In
			// such case, neither a client nor a server shall be created nor started.
			if (!IsDisposed && IsStarted) // Check 'IsDisposed' first!
			{
				SetStateSynchronizedAndNotify(SocketState.Connecting);
				CreateClient(this.remoteHost, this.remotePort, this.localInterface);

				bool startIsOngoing = false;
				try
				{
					lock (this.socketSyncObj)
					{
						if (this.client != null)
							startIsOngoing = this.client.Start();
					}
				}
				finally
				{
					if (!startIsOngoing)
					{
						DisposeClient();
						StartListening();
					}
				}
			}
		}

		/// <summary>
		/// Try to start as server.
		/// </summary>
		private void StartListening()
		{
			int randomDelay = SocketBase.Random.Next(MinListenDelay, MaxListenDelay);
			DebugMessage("Delaying listening by random value of " + randomDelay + " ms.");
			Thread.Sleep(randomDelay);

			// Only continue if socket is still up and running after the delay! Required because
			// re-connecting happens automatically when connection gets lost. If two AutoSockets
			// are interconnected, and both are shut down, the sequence of the operation is not
			// defined. It is likely that Stop() is called while the thread is delayed above. In
			// such case, neither a client nor a server shall be created nor started.
			if (!IsDisposed && IsStarted) // Check 'IsDisposed' first!
			{
				SetStateSynchronizedAndNotify(SocketState.StartingListening);
				CreateServer(this.localInterface, this.localPort);

				bool startIsOngoing = false;
				try
				{
					lock (this.socketSyncObj)
					{
						if (this.server != null)
							startIsOngoing = this.server.Start(); // Server will be started asynchronously.
					}
				}
				catch (System.Net.Sockets.SocketException)
				{
					// A 'SocketException' can occur in the call stack shown below, in case another
					// AutoSocket or server tries to bind the given end point at the same time. In
					// such case, ignore the exception and try again later. This procedure ensures
					// that multiple AutoSockets eventually get connected to each other.
					//
					// Stack trace:
					//  > MKY.IO.Serial.Socket.TcpServer.Start() <= Called above
					//  > MKY.IO.Serial.Socket.TcpServer.StartSocketAndThread()
					//  > ALAZ.SystemEx.NetEx.SocketsEx.BaseSocketConnectionHost.Start()
					//  > ALAZ.SystemEx.NetEx.SocketsEx.SocketListener.Start()
					//  > System.Net.Sockets.Socket.Bind(EndPoint localEP)
					//  > System.Net.Sockets.Socket.DoBind(EndPoint endPointSnapshot, SocketAddress socketAddress)
				}
				finally
				{
					if (!startIsOngoing)
					{
						DisposeServer();
						RequestTryAgain();
					}
				}
			}
		}

		private void RequestTryAgain()
		{
			bool tryAgain = false;
			lock (this.startCycleCounterSyncObj)
			{
				this.startCycleCounter++;
				if (this.startCycleCounter <= MaxStartCycles)
					tryAgain = true;
			}

			if (tryAgain)
			{
				DebugMessage("Trying connect cycle #" + this.startCycleCounter + ".");

				StartConnecting();
			}
			else
			{
				string message =
					"AutoSocket could neither be started as client nor server," + Environment.NewLine +
					"TCP/IP address/port is not available.";

				AutoSocketError
				(
					ErrorSeverity.Acceptable,
					message
				);
			}
		}

	////private void RestartAutoSocket()
	////{
	////	SetStateSynchronizedAndNotify(SocketState.Restarting);
	////	StopAndDisposeSockets();
	////	StartAutoSocket();
	////}

		private void StopAutoSocket()
		{
			SetStateSynchronizedAndNotify(SocketState.Stopping);
			StopAndDisposeSockets();
			SetStateSynchronizedAndNotify(SocketState.Reset);
		}

		private void AutoSocketError(ErrorSeverity severity, string message)
		{
			DebugMessage(severity + " error in AutoSocket: " + Environment.NewLine + message);

			DisposeSockets();

			SetStateSynchronizedAndNotify(SocketState.Error);
			OnIOError(new IOErrorEventArgs(severity, message));
		}

		#endregion

		#region Client
		//==========================================================================================
		// Client
		//==========================================================================================

		#region Client > Lifetime
		//------------------------------------------------------------------------------------------
		// Client > Lifetime
		//------------------------------------------------------------------------------------------

		private void CreateClient(IPHostEx remoteHost, int remotePort, IPNetworkInterfaceEx localInterface)
		{
			DisposeClient();

			lock (this.socketSyncObj)
			{
				this.client = new TcpClient(this.instanceId, remoteHost, remotePort, localInterface);

				this.client.IOChanged    += client_IOChanged;
				this.client.IOError      += client_IOError;
				this.client.DataReceived += client_DataReceived;
				this.client.DataSent     += client_DataSent;
			}
		}

		private void DisposeClient()
		{
			lock (this.socketSyncObj)
			{
				if (this.client != null)
				{
					this.client.IOChanged    -= client_IOChanged;
					this.client.IOError      -= client_IOError;
					this.client.DataReceived -= client_DataReceived;
					this.client.DataSent     -= client_DataSent;

					this.client.Dispose();
					this.client = null;
				}
			}
		}

		#endregion

		#region Client > Events
		//------------------------------------------------------------------------------------------
		// Client > Events
		//------------------------------------------------------------------------------------------

		private void client_IOChanged(object sender, EventArgs e)
		{
			switch (GetStateSynchronized())
			{
				case SocketState.Connecting:
				{
					bool isConnected = false;
					lock (this.socketSyncObj)
					{
						if (this.client != null)
							isConnected = this.client.IsConnected;
					}

					if (isConnected)                  // If I/O changed during startup,
					{                                 //   check for connected and change state.
						SetStateSynchronizedAndNotify(SocketState.Connected);
					}

					break;
				}
				case SocketState.Connected:
				{
					bool isConnected = false;
					lock (this.socketSyncObj)
					{
						if (this.client != null)
							isConnected = this.client.IsConnected;
					}

					if (isConnected)                  // If I/O changed during client operation
					{                                 //   and client is connected to a server,
						OnIOChanged(e);               //   simply forward the event.
					}
					else
					{
						DisposeClient();              // If client lost connection to server,
						StartListening();             //   change to server operation.
					}

					break;
				}
			}
		}

		private void client_IOError(object sender, IOErrorEventArgs e)
		{
			switch (GetStateSynchronized())
			{
				case SocketState.Connecting:
				case SocketState.ConnectingFailed:
				{
					DisposeClient();                // In case of error during startup,
					StartListening();               //   try to start as server.
					break;
				}

				case SocketState.Connected:
				{
					if (e.Severity == ErrorSeverity.Acceptable)
					{
						DisposeClient();            // In case of error during client operation,
						StartConnecting();          //   restart AutoSocket.
					}
					else
					{
						OnIOError(e);
					}
					break;
				}

				default:
				{
					OnIOError(e);
					break;
				}
			}
		}

		private void client_DataReceived(object sender, DataReceivedEventArgs e)
		{
			if (!IsDisposed && IsClient) // Check 'IsDisposed' first!
				OnDataReceived(e);
		}

		private void client_DataSent(object sender, DataSentEventArgs e)
		{
			if (!IsDisposed && IsClient) // Check 'IsDisposed' first!
				OnDataSent(e);
		}

		#endregion

		#endregion

		#region Server
		//==========================================================================================
		// Server
		//==========================================================================================

		#region Server > Lifetime
		//------------------------------------------------------------------------------------------
		// Server > Lifetime
		//------------------------------------------------------------------------------------------

		private void CreateServer(IPNetworkInterfaceEx localInterface, int localPort)
		{
			DisposeServer();

			lock (this.socketSyncObj)
			{
				this.server = new TcpServer(this.instanceId, localInterface, localPort);

				this.server.IOChanged    += server_IOChanged;
				this.server.IOError      += server_IOError;
				this.server.DataReceived += server_DataReceived;
				this.server.DataSent     += server_DataSent;
			}
		}

		private void DisposeServer()
		{
			lock (this.socketSyncObj)
			{
				if (this.server != null)
				{
					this.server.IOChanged    -= server_IOChanged;
					this.server.IOError      -= server_IOError;
					this.server.DataReceived -= server_DataReceived;
					this.server.DataSent     -= server_DataSent;

					this.server.Dispose();
					this.server = null;
				}
			}
		}

		#endregion

		#region Server > Events
		//------------------------------------------------------------------------------------------
		// Server > Events
		//------------------------------------------------------------------------------------------

		private void server_IOChanged(object sender, EventArgs e)
		{
			switch (GetStateSynchronized())
			{
				case SocketState.StartingListening:
				{
					bool isStarted = false;
					lock (this.socketSyncObj)
					{
						if (this.server != null)
							isStarted = this.server.IsStarted;
					}

					if (isStarted)                    // If I/O changed during startup,
					{                                 //   check for start and change state.
						SetStateSynchronizedAndNotify(SocketState.Listening);
					}

					break;
				}
				case SocketState.Listening:
				{
					int connectedClientCount = 0;
					lock (this.socketSyncObj)
					{
						if (this.server != null)
							connectedClientCount = this.server.ConnectedClientCount;
					}

					if (connectedClientCount > 0)     // If I/O changed during listening, change state
					{                                 //   to accepted if clients are connected.
						SetStateSynchronizedAndNotify(SocketState.Accepted);
					}

					break;
				}
				case SocketState.Accepted:
				{
					int connectedClientCount = 0;
					lock (this.socketSyncObj)
					{
						if (this.server != null)
							connectedClientCount = this.server.ConnectedClientCount;
					}

					if (connectedClientCount <= 0)    // If I/O changed during accepted, change state
					{                                 //   to listening if no clients are connected.
						SetStateSynchronizedAndNotify(SocketState.Listening);
					}

					break;
				}
			}
		}

		private void server_IOError(object sender, IOErrorEventArgs e)
		{
			switch (GetStateSynchronized())
			{
				case SocketState.StartingListening: // In case of error during startup,
				{                                   //   increment start cycles and
					RequestTryAgain();              //   continue depending on count
					break;
				}

				case SocketState.Listening:
				case SocketState.Accepted:
				{
					if (e.Severity == ErrorSeverity.Acceptable)
					{
						DisposeServer();            // In case of error during server operation,
						StartConnecting();          //   restart AutoSocket
					}
					else
					{
						OnIOError(e);
					}
					break;
				}

				default:
				{
					OnIOError(e);
					break;
				}
			}
		}

		private void server_DataReceived(object sender, DataReceivedEventArgs e)
		{
			if (!IsDisposed && IsServer) // Check 'IsDisposed' first!
				OnDataReceived(e);
		}

		private void server_DataSent(object sender, DataSentEventArgs e)
		{
			if (!IsDisposed && IsServer) // Check 'IsDisposed' first!
				OnDataSent(e);
		}

		#endregion

		#endregion

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnIOChanged(EventArgs e)
		{
			if (!IsDisposed) // Make sure to propagate event only if not already disposed. This may happen on an async System.Net.Sockets.SocketAsyncEventArgs.CompletionPortCallback.
				this.eventHelper.RaiseSync(IOChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOControlChanged(EventArgs e)
		{
			UnusedEvent.PreventCompilerWarning(IOControlChanged);
			throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The event 'IOControlChanged' is not in use for TCP/IP AutoSockets!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary></summary>
		protected virtual void OnIOError(IOErrorEventArgs e)
		{
			if (!IsDisposed) // Make sure to propagate event only if not already disposed. This may happen on an async System.Net.Sockets.SocketAsyncEventArgs.CompletionPortCallback.
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
			// Do not call AssertNotDisposed() on such basic method! Its return value may be needed for debugging. All underlying fields are still valid after disposal.

			return (ToShortEndPointString());
		}

		/// <remarks>
		/// Named according to .NET <see cref="System.Net.IPEndPoint"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "EndPoint", Justification = "Naming according to System.Net.EndPoint.")]
		public virtual string ToShortEndPointString()
		{
			// Do not call AssertNotDisposed() on such basic method! Its return value is needed for debugging! All underlying fields are still valid after disposal.

			return ("Server:" + this.localPort + " / " + this.remoteHost.ToEndpointAddressString() + ":" + this.remotePort);
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

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
