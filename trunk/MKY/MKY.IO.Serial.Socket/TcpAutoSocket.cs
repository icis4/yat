//==================================================================================================
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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using MKY.Contracts;
using MKY.Net;

#endregion

namespace MKY.IO.Serial.Socket
{
	/// <summary>
	/// This TCP/IP AutoSocket automatically determines whether to run as client or server. On start,
	/// it tries to connect to a remote server and run as client. If this fails, it tries to run as
	/// server. Retry cycles and random wait times ensure proper operation even when two AutoSockets
	/// are interconnected to each other.
	/// </summary>
	/// <remarks>
	/// Initially, YAT AutoSockets with the original ALAZ implementation created a deadlock on
	/// shutdown when two AutoSockets that were interconnected with each other. The situation:
	/// 
	/// 1. The GUI/main thread requests stopping all terminals:
	///     => YAT.Model.Workspace.CloseAllTerminals()
	///         => MKY.IO.Serial.TcpAutoSocket.Stop()
	///            => ALAZ.SystemEx.NetEx.SocketsEx.SocketServer.Stop()
	///                => ALAZ.SystemEx.NetEx.SocketsEx.BaseSocketConnection.Active.get()
	/// 
	/// 2. As a result, the first AutoSocket shuts down, but the second changes from 'Accepted' to
	///    'Listening' and tries to synchronize from the ALAZ socket event to the GUI/main thread:
	///     => ALAZ.SystemEx.NetEx.SocketsEx.BaseSocketConnectionHost.FireOnDisconnected()
	///         => MKY.IO.Serial.TcpAutoSocket.OnIOChanged()
	///             => YAT.Model.Terminal.OnIOChanged()
	///                 => Deadlock when synchronizing onto GUI/main thread !!!
	/// 
	/// The issue has been solved in <see cref="ALAZ.SystemEx.NetEx.SocketsEx.BaseSocketConnection"/>
	/// as well as <see cref="TcpClient"/> or <see cref="TcpServer"/> by invoking Stop() and Dispose()
	/// of socket and connections and thread asynchronously and without firing events. See remarks
	/// of these classes for additional information.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "A type shall spell 'Tcp' like this...")]
	public class TcpAutoSocket : IIOProvider, IDisposable
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
			Error,
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
		private bool isDisposed;

		private System.Net.IPAddress remoteIPAddress;
		private int remotePort;
		private System.Net.IPAddress localIPAddress;
		private int localPort;

		private SocketState state = SocketState.Reset;
		private ReaderWriterLockSlim stateLock = new ReaderWriterLockSlim();

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

		/// <summary></summary>
		public TcpAutoSocket(System.Net.IPAddress remoteIPAddress, int remotePort, System.Net.IPAddress localIPAddress, int localPort)
		{
			this.instanceId = SocketBase.NextInstanceId;

			this.remoteIPAddress = remoteIPAddress;
			this.remotePort = remotePort;
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
		protected virtual void Dispose(bool disposing)
		{
			if (!this.isDisposed)
			{
				WriteDebugMessageLine("Disposing...");

				// Dispose of managed resources if requested:
				if (disposing)
				{
					// In the 'normal' case, the items have already been disposed of, e.g. in Stop().
					DisposeSockets();

					if (this.stateLock != null)
						this.stateLock.Dispose();
				}

				// Set state to disposed:
				this.isDisposed = true;

				WriteDebugMessageLine("...successfully disposed.");
			}
		}

		/// <summary></summary>
		~TcpAutoSocket()
		{
			Dispose(false);

			WriteDebugMessageLine("The finalizer should have never been called! Ensure to call Dispose()!");
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
		public virtual System.Net.IPAddress RemoteIPAddress
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.remoteIPAddress);
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
					WriteDebugMessageLine("Starting...");
					StartAutoSocket();
					return (true);
				}
				default:
				{
					WriteDebugMessageLine("Start() requested but state is " + state + ".");
					return (false);
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "Stop is a common term to start/stop something.")]
		public virtual void Stop()
		{
			AssertNotDisposed();

			SocketState state = GetStateSynchronized();
			switch (state)
			{
				case SocketState.Reset:
				{
					WriteDebugMessageLine("Stop() requested but state is " + state + ".");
					return;
				}
				default:
				{
					WriteDebugMessageLine("Stopping...");
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
				WriteDebugMessageLine("State has changed from " + oldState + " to " + state + ", " + isClientOrServerString + ".");
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
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		private void StartConnecting()
		{
			int randomDelay = SocketBase.Random.Next(MinConnectDelay, MaxConnectDelay);
			WriteDebugMessageLine("Delaying connecting by random value of " + randomDelay + " ms.");
			Thread.Sleep(randomDelay);

			// Only continue if socket is still up and running after the delay! Required because
			// re-connecting happens automatically when connection gets lost. If two AutoSockets
			// are interconnected, and both are shut down, the sequence of the operation is not
			// defined. It is likely that Stop() is called while the thread is delayed above. In
			// such case, neither a client nor a server shall be created nor started.
			if (!IsDisposed && IsStarted) // Check 'IsDisposed' first!
			{
				SetStateSynchronizedAndNotify(SocketState.Connecting);
				CreateClient(this.remoteIPAddress, this.remotePort);

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
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		private void StartListening()
		{
			int randomDelay = SocketBase.Random.Next(MinListenDelay, MaxListenDelay);
			WriteDebugMessageLine("Delaying listening by random value of " + randomDelay + " ms.");
			Thread.Sleep(randomDelay);

			// Only continue if socket is still up and running after the delay! Required because
			// re-connecting happens automatically when connection gets lost. If two AutoSockets
			// are interconnected, and both are shut down, the sequence of the operation is not
			// defined. It is likely that Stop() is called while the thread is delayed above. In
			// such case, neither a client nor a server shall be created nor started.
			if (!IsDisposed && IsStarted) // Check 'IsDisposed' first!
			{
				SetStateSynchronizedAndNotify(SocketState.StartingListening);
				CreateServer(this.localIPAddress, this.localPort);

				bool startIsOngoing = false;
				try
				{
					lock (this.socketSyncObj)
					{
						if (this.server != null)
							startIsOngoing = this.server.Start(); // Server will be started asynchronously.
					}
				}
				catch (SocketException)
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
				WriteDebugMessageLine("Trying connect cycle #" + this.startCycleCounter + ".");

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
			WriteDebugMessageLine(severity + " error in AutoSocket: " + Environment.NewLine + message);

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

		private void CreateClient(System.Net.IPAddress remoteIPAddress, int remotePort)
		{
			DisposeClient();

			lock (this.socketSyncObj)
			{
				this.client = new TcpClient(this.instanceId, remoteIPAddress, remotePort);

				this.client.IOChanged    += new EventHandler                       (this.client_IOChanged);
				this.client.IOError      += new EventHandler<IOErrorEventArgs>     (this.client_IOError);
				this.client.DataReceived += new EventHandler<DataReceivedEventArgs>(this.client_DataReceived);
				this.client.DataSent     += new EventHandler<DataSentEventArgs>    (this.client_DataSent);
			}
		}

		private void DisposeClient()
		{
			lock (this.socketSyncObj)
			{
				if (this.client != null)
				{
					this.client.IOChanged    -= new EventHandler                       (this.client_IOChanged);
					this.client.IOError      -= new EventHandler<IOErrorEventArgs>     (this.client_IOError);
					this.client.DataReceived -= new EventHandler<DataReceivedEventArgs>(this.client_DataReceived);
					this.client.DataSent     -= new EventHandler<DataSentEventArgs>    (this.client_DataSent);

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
					DisposeClient();                  // In case of error during startup,
					StartListening();                 //   try to start as server.
					break;
				}
				case SocketState.Connected:
				{
					DisposeClient();                  // In case of error during client operation,
					StartConnecting();                //   restart AutoSocket.
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

		private void CreateServer(System.Net.IPAddress localIPAddress, int localPort)
		{
			DisposeServer();

			lock (this.socketSyncObj)
			{
				this.server = new TcpServer(this.instanceId, localIPAddress, localPort);

				this.server.IOChanged    += new EventHandler(this.server_IOChanged);
				this.server.IOError      += new EventHandler<IOErrorEventArgs>(this.server_IOError);
				this.server.DataReceived += new EventHandler<DataReceivedEventArgs>(this.server_DataReceived);
				this.server.DataSent     += new EventHandler<DataSentEventArgs>(this.server_DataSent);
			}
		}

		private void DisposeServer()
		{
			lock (this.socketSyncObj)
			{
				if (this.server != null)
				{
					this.server.IOChanged    -= new EventHandler(this.server_IOChanged);
					this.server.IOError      -= new EventHandler<IOErrorEventArgs>(this.server_IOError);
					this.server.DataReceived -= new EventHandler<DataReceivedEventArgs>(this.server_DataReceived);
					this.server.DataSent     -= new EventHandler<DataSentEventArgs>(this.server_DataSent);

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
				case SocketState.StartingListening:   // In case of error during startup,
				{                                     //   increment start cycles and
					RequestTryAgain();                //   continue depending on count
					break;
				}
				case SocketState.Listening:
				case SocketState.Accepted:
				{
					DisposeServer();                  // In case of error during server operation,
					StartConnecting();                //   restart AutoSocket
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
			throw (new InvalidOperationException("Program execution should never get here, the event 'IOControlChanged' is not in use for TCP/IP AutoSockets!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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

		/// <remarks>
		/// Named according to .NET <see cref="System.Net.IPEndPoint"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "EndPoint", Justification = "Naming according to System.Net.EndPoint.")]
		public virtual string ToShortEndPointString()
		{
			return ("Server:" + this.localPort + " / " + IPHostEx.ToEndpointAdressString(this.remoteIPAddress) + ":" + this.remotePort);
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

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
