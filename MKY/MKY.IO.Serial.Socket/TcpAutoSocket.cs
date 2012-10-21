//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.8
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
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
using System.Text;
using System.Threading;

using MKY.Contracts;
using MKY.Event;

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
	/// In case of YAT with the original ALAZ implementation, AutoSockets created a deadlock on
	/// shutdown when two AutoSockets that were interconnected with each other. The situation:
	/// 
	/// a) The GUI/main thread requests stopping all terminals:
	/// 
	/// ALAZ.SystemEx.NetEx.SocketsEx.BaseSocketConnection.Active.get() Line 286
	/// ALAZ.SystemEx.NetEx.SocketsEx.BaseSocketConnectionHost.BeginDisconnect(ALAZ.SystemEx.NetEx.SocketsEx.BaseSocketConnection connection = {ALAZ.SystemEx.NetEx.SocketsEx.ServerSocketConnection}) Line 1446
	/// ALAZ.SystemEx.NetEx.SocketsEx.BaseSocketConnection.BeginDisconnect() Line 558
	/// ALAZ.SystemEx.NetEx.SocketsEx.BaseSocketConnectionHost.StopConnections() Line 351
	/// ALAZ.SystemEx.NetEx.SocketsEx.SocketServer.Stop() Line 206
	/// MKY.IO.Serial.TcpServer.StopSocket() Line 432
	/// MKY.IO.Serial.TcpServer.Stop() Line 329
	/// MKY.IO.Serial.TcpAutoSocket.StopSockets() Line 494
	/// MKY.IO.Serial.TcpAutoSocket.StopAutoSocket() Line 619
	/// MKY.IO.Serial.TcpAutoSocket.Stop() Line 419
	/// YAT.Domain.RawTerminal.Stop() Line 298
	/// YAT.Domain.Terminal.Stop() Line 314
	/// YAT.Model.Terminal.StopIO(bool saveStatus = false) Line 1488
	/// YAT.Model.Terminal.Close(bool isWorkspaceClose = true, bool tryAutoSave = true) Line 1155
	/// YAT.Model.Workspace.CloseAllTerminals(bool isWorkspaceClose = true, bool tryAutoSave = true) Line 1215
	/// YAT.Model.Workspace.Close(bool isMainClose = false) Line 665
	/// YAT.Model.Workspace.Close() Line 625
	/// YAT.Gui.Forms.Main.toolStripMenuItem_MainMenu_File_Workspace_Close_Click(object sender = {System.Windows.Forms.ToolStripMenuItem}, System.EventArgs e = {System.EventArgs}) Line 332
	/// YAT.Controller.Main.Run(bool runWithView = true) Line 327
	/// YAT.Controller.Main.Run() Line 261
	/// YAT.YAT.Main(string[] commandLineArgs = {string[0]}) Line 63
	/// 
	/// b) As a result, the first AutoSocket shuts down, the second changes from 'Accepted' to
	///    'Listening' and tries to sychronize from the ALAZ socket event to the GUI/main thread:
	/// 
	/// MKY.Event.EventHelper.InvokeSynchronized(System.ComponentModel.ISynchronizeInvoke sinkTarget = {YAT.Gui.Forms.Terminal}, System.Delegate sink = {Method = Cannot evaluate expression because the current thread is in a sleep, wait, or join}, object[] args = {object[2]}) Line 319
	/// MKY.Event.EventHelper.FireSync(System.Delegate eventDelegate = {Method = Cannot evaluate expression because the current thread is in a sleep, wait, or join}, object[] args = {object[2]}) Line 163
	/// YAT.Model.Terminal.OnIOChanged(System.EventArgs e = {System.EventArgs}) Line 2195
	/// YAT.Model.Terminal.terminal_IOChanged(object sender = {YAT.Domain.TextTerminal}, System.EventArgs e = {System.EventArgs}) Line 1261
	/// MKY.Event.EventHelper.FireSync(System.Delegate eventDelegate = {Method = Cannot evaluate expression because the current thread is in a sleep, wait, or join}, object[] args = {object[2]}) Line 173
	/// YAT.Domain.Terminal.OnIOChanged(System.EventArgs e = {System.EventArgs}) Line 1049
	/// YAT.Domain.Terminal.rawTerminal_IOChanged(object sender = {YAT.Domain.RawTerminal}, System.EventArgs e = {System.EventArgs}) Line 978
	/// MKY.Event.EventHelper.FireSync(System.Delegate eventDelegate = {Method = Cannot evaluate expression because the current thread is in a sleep, wait, or join}, object[] args = {object[2]}) Line 173
	/// YAT.Domain.RawTerminal.OnIOChanged(System.EventArgs e = {System.EventArgs}) Line 601
	/// YAT.Domain.RawTerminal.io_IOChanged(object sender = {MKY.IO.Serial.TcpAutoSocket}, System.EventArgs e = {System.EventArgs}) Line 557
	/// MKY.Event.EventHelper.FireSync(System.Delegate eventDelegate = {Method = Cannot evaluate expression because the current thread is in a sleep, wait, or join}, object[] args = {object[2]}) Line 173
	/// MKY.IO.Serial.TcpAutoSocket.OnIOChanged(System.EventArgs e = {System.EventArgs}) Line 854
	/// MKY.IO.Serial.TcpAutoSocket.SetStateSynchronizedAndNotify(MKY.IO.Serial.TcpAutoSocket.SocketState state = Listening) Line 476
	/// MKY.IO.Serial.TcpAutoSocket.server_IOChanged(object sender = {MKY.IO.Serial.TcpServer}, System.EventArgs e = {System.EventArgs}) Line 804
	/// MKY.Event.EventHelper.FireSync(System.Delegate eventDelegate = {Method = Cannot evaluate expression because the current thread is in a sleep, wait, or join}, object[] args = {object[2]}) Line 173
	/// MKY.IO.Serial.TcpServer.OnIOChanged(System.EventArgs e = {System.EventArgs}) Line 556
	/// MKY.IO.Serial.TcpServer.SetStateSynchronizedAndNotify(MKY.IO.Serial.TcpServer.SocketState state = Listening) Line 395
	/// MKY.IO.Serial.TcpServer.OnDisconnected(ALAZ.SystemEx.NetEx.SocketsEx.ConnectionEventArgs e = {ALAZ.SystemEx.NetEx.SocketsEx.ConnectionEventArgs}) Line 525
	/// ALAZ.SystemEx.NetEx.SocketsEx.BaseSocketConnectionHost.FireOnDisconnected(ALAZ.SystemEx.NetEx.SocketsEx.BaseSocketConnection connection = {ALAZ.SystemEx.NetEx.SocketsEx.ServerSocketConnection}) Line 535
	/// ALAZ.SystemEx.NetEx.SocketsEx.BaseSocketConnectionHost.BeginDisconnectCallbackAsync(object sender = {System.Net.Sockets.Socket}, System.Net.Sockets.SocketAsyncEventArgs e = null) Line 1501
	/// 
	/// Very simliar issues existed when stopping <see cref="TcpClient"/> or <see cref="TcpServer"/>
	/// objects stand-alone. See remarks of these classes for how all these issues have been solved.
	/// </remarks>
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
			Restarting,
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

		#region Static Fields
		//==========================================================================================
		// Static Fields
		//==========================================================================================

		private static int staticInstanceCounter;
		private static Random staticRandom = new Random();

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

		private int startCycleCounter = 0;
		private object startCycleCounterSyncObj = new object();

		private TcpClient client;
		private TcpServer server;

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
			this.instanceId = staticInstanceCounter++;

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
				// Finalize managed resources.

				if (disposing)
				{
					// In the 'normal' case, the items have already been disposed of.
					DisposeSockets();
				}

				this.isDisposed = true;

				Debug.WriteLine(GetType() + " (" + this.instanceId + ")(" + ToShortEndPointString() + "): Disposed.");
			}
		}

		/// <summary></summary>
		~TcpAutoSocket()
		{
			Dispose(false);
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
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed"));
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
				AssertNotDisposed();
				return (this.remoteIPAddress);
			}
		}

		/// <summary></summary>
		public virtual int RemotePort
		{
			get
			{
				AssertNotDisposed();
				return (this.remotePort);
			}
		}

		/// <summary></summary>
		public virtual System.Net.IPAddress LocalIPAddress
		{
			get
			{
				AssertNotDisposed();
				return (this.localIPAddress);
			}
		}

		/// <summary></summary>
		public virtual int LocalPort
		{
			get
			{
				AssertNotDisposed();
				return (this.localPort);
			}
		}

		/// <summary></summary>
		public virtual bool IsStopped
		{
			get
			{
				AssertNotDisposed();

				switch (this.state)
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
				AssertNotDisposed();

				switch (this.state)
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
		public virtual bool IsReadyToSend
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
				AssertNotDisposed();

				switch (this.state)
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
				AssertNotDisposed();

				switch (this.state)
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

				if (IsClient)
					return (this.client.UnderlyingIOInstance);
				else if (IsServer)
					return (this.server.UnderlyingIOInstance);
				else
					return (null);
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

			switch (GetStateSynchronized())
			{
				case SocketState.Reset:
				case SocketState.Error:
				{
					StartAutoSocket();
					return (true);
				}
				default:
				{
					Debug.WriteLine(GetType() + " (" + this.instanceId + ")(" + ToShortEndPointString() + "): Start() requested but state is " + this.state + ".");
					return (false);
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "Stop is a common term to start/stop something.")]
		public virtual void Stop()
		{
			AssertNotDisposed();

			switch (GetStateSynchronized())
			{
				case SocketState.Reset:
				{
					Debug.WriteLine(GetType() + " (" + this.instanceId + ")(" + ToShortEndPointString() + "): Stop() requested but state is " + this.state + ".");
					return;
				}
				default:
				{
					StopAutoSocket();
					return;
				}
			}
		}

		/// <summary></summary>
		public virtual void Send(byte[] data)
		{
			AssertNotDisposed();

			if (IsReadyToSend)
			{
				if      (IsClient)
					this.client.Send(data);
				else if (IsServer)
					this.server.Send(data);
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
			SocketState oldState = this.state;
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

			Debug.WriteLine(GetType() + " (" + this.instanceId + ")(" + ToShortEndPointString() + "): State has changed from " + oldState + " to " + this.state + ", " + isClientOrServerString + ".");
#endif
			OnIOChanged(new EventArgs());
		}

		#endregion

		#region Socket Methods
		//==========================================================================================
		// Socket Methods
		//==========================================================================================

		private void StopSockets()
		{
			if (this.client != null)
				this.client.Stop();

			if (this.server != null)
				this.server.Stop();
		}

		private void DisposeSockets()
		{
			DestroyClient();
			DestroyServer();
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
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		private void StartConnecting()
		{
			int delay = staticRandom.Next(MinConnectDelay, MaxConnectDelay);
#if (FALSE)
			Debug.WriteLine(GetType() + " (" + this.instanceId + ")(" + ToShortEndPointString() + "): Delaying connecting by " + delay);
#endif
			Thread.Sleep(delay);

			SetStateSynchronizedAndNotify(SocketState.Connecting);
			CreateClient(this.remoteIPAddress, this.remotePort);
			try
			{
				this.client.Start(); // Client will be started asynchronously
			}
			catch
			{
				DestroyClient();
				StartListening();
			}
		}

		/// <summary>
		/// Try to start as server.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		private void StartListening()
		{
			int delay = staticRandom.Next(MinListenDelay, MaxListenDelay);
#if (FALSE)
			Debug.WriteLine(GetType() + " (" + this.instanceId + ")(" + ToShortEndPointString() + "): Delaying listening by " + delay);
#endif
			Thread.Sleep(delay);

			SetStateSynchronizedAndNotify(SocketState.StartingListening);
			CreateServer(this.localIPAddress, this.localPort);
			try
			{
				this.server.Start(); // Server will be started asynchronously
			}
			catch
			{
				DestroyServer();
				RequestTryAgain();
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
				Debug.WriteLine(GetType() + " (" + this.instanceId + ")(" + ToShortEndPointString() + "): Trying connect cycle " + this.startCycleCounter + ".");
				StartConnecting();
			}
			else
			{
				AutoSocketError
					(
					"AutoSocket could neither be started as client nor server," + Environment.NewLine +
					"TCP/IP address/port is not available."
					);
			}
		}

		private void RestartAutoSocket()
		{
			SetStateSynchronizedAndNotify(SocketState.Restarting);

			StopSockets();
			DisposeSockets();

			StartAutoSocket();
		}

		private void StopAutoSocket()
		{
			SetStateSynchronizedAndNotify(SocketState.Stopping);

			StopSockets();
			DisposeSockets();

			SetStateSynchronizedAndNotify(SocketState.Reset);
		}

		private void AutoSocketError(string message)
		{
			DisposeSockets();

			SetStateSynchronizedAndNotify(SocketState.Error);
			OnIOError(new IOErrorEventArgs(message));
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
			this.client = new TcpClient(remoteIPAddress, remotePort);

			this.client.IOChanged    += new EventHandler(this.client_IOChanged);
			this.client.IOError      += new EventHandler<IOErrorEventArgs>(this.client_IOError);
			this.client.DataReceived += new EventHandler<DataReceivedEventArgs>(this.client_DataReceived);
			this.client.DataSent     += new EventHandler<DataSentEventArgs>(this.client_DataSent);
		}

		private void DestroyClient()
		{
			if (this.client != null)
			{
				this.client.IOChanged    -= new EventHandler(this.client_IOChanged);
				this.client.IOError      -= new EventHandler<IOErrorEventArgs>(this.client_IOError);
				this.client.DataReceived -= new EventHandler<DataReceivedEventArgs>(this.client_DataReceived);
				this.client.DataSent     -= new EventHandler<DataSentEventArgs>(this.client_DataSent);

				this.client.Dispose();
				this.client = null;
			}
		}

		#endregion

		#region Client > Events
		//------------------------------------------------------------------------------------------
		// Client > Events
		//------------------------------------------------------------------------------------------

		private void client_IOChanged(object sender, EventArgs e)
		{
			switch (this.state)
			{
				case SocketState.Connecting:
				{
					if (this.client.IsConnected)      // If IO changed during startup,
					{                                 //   check for connected and change state.
						SetStateSynchronizedAndNotify(SocketState.Connected);
					}
					break;
				}
				case SocketState.Connected:
				{
					if (this.client.IsConnected)      // If IO changed during client operation
					{                                 //   and client is connected to a server,
						OnIOChanged(e);               //   simply forward the event.
					}
					else
					{
						DisposeSockets();             // If client lost connection to server,
						StartListening();             //   change to server operation.
					}
					break;
				}
			}
		}

		private void client_IOError(object sender, IOErrorEventArgs e)
		{
			switch (this.state)
			{
				case SocketState.Connecting:
				case SocketState.ConnectingFailed:
				{
					DisposeSockets();                 // In case of error during startup,
					StartListening();                 //   try to start as server.
					break;
				}
				case SocketState.Connected:
				{
					DisposeSockets();                 // In case of error during client operation,
					StartConnecting();                //   restart AutoSocket.
					break;
				}
			}
		}

		private void client_DataReceived(object sender, DataReceivedEventArgs e)
		{
			if (!IsDisposed && IsClient)
				OnDataReceived(e);
		}

		private void client_DataSent(object sender, DataSentEventArgs e)
		{
			if (!IsDisposed && IsClient)
				OnDataSent(e);
		}

		#endregion

		#endregion

		#region Server
		//==========================================================================================
		// Server
		//==========================================================================================

		#region Server > Events
		//------------------------------------------------------------------------------------------
		// Server > Events
		//------------------------------------------------------------------------------------------

		private void CreateServer(System.Net.IPAddress localIPAddress, int localPort)
		{
			this.server = new TcpServer(localIPAddress, localPort);

			this.server.IOChanged    += new EventHandler(this.server_IOChanged);
			this.server.IOError      += new EventHandler<IOErrorEventArgs>(this.server_IOError);
			this.server.DataReceived += new EventHandler<DataReceivedEventArgs>(this.server_DataReceived);
			this.server.DataSent     += new EventHandler<DataSentEventArgs>(this.server_DataSent);
		}

		private void DestroyServer()
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

		#endregion

		#region Server > Events
		//------------------------------------------------------------------------------------------
		// Server > Events
		//------------------------------------------------------------------------------------------

		private void server_IOChanged(object sender, EventArgs e)
		{
			SocketState state = GetStateSynchronized();
			switch (state)
			{
				case SocketState.StartingListening:
				{
					if (this.server.IsStarted)                    // If IO changed during startup,
						SetStateSynchronizedAndNotify(SocketState.Listening); //   check for start and change state
					break;
				}
				case SocketState.Listening:
				{
					if (this.server.ConnectedClientCount > 0)     // If IO changed during listening,
						SetStateSynchronizedAndNotify(SocketState.Accepted);  //   change state to accepted if
					break;                                        //   clients are connected
				}
				case SocketState.Accepted:
				{
					if (this.server.ConnectedClientCount <= 0)    // If IO changed during accepted,
						SetStateSynchronizedAndNotify(SocketState.Listening); //   change state to listening if
					break;                                        //   no clients are connected
				}
			}
		}

		private void server_IOError(object sender, IOErrorEventArgs e)
		{
			switch (this.state)
			{
				case SocketState.StartingListening:   // In case of error during startup,
				{                                     //   increment start cycles and
					RequestTryAgain();                //   continue depending on count
					break;
				}
				case SocketState.Listening:
				case SocketState.Accepted:
				{
					DisposeSockets();                 // In case of error during server operation,
					StartConnecting();                //   restart AutoSocket
					break;
				}
			}
		}

		private void server_DataReceived(object sender, DataReceivedEventArgs e)
		{
			if (!IsDisposed && IsServer)
				OnDataReceived(e);
		}

		private void server_DataSent(object sender, DataSentEventArgs e)
		{
			if (!IsDisposed && IsServer)
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
			throw (new NotImplementedException("Event 'IOControlChanged' is not in use for TCP AutoSockets"));
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

		/// <remarks>
		/// Named according to .NET <see cref="System.Net.IPEndPoint"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "EndPoint", Justification = "Naming according to System.Net.EndPoint.")]
		public virtual string ToShortEndPointString()
		{
			return ("Server:" + this.localPort + " / " + this.remoteIPAddress + ":" + this.remotePort);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
