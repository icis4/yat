//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;

using MKY.Utilities.Event;

// The MKY.IO.Serial namespace combines various serial interface infrastructure. This code is
// intentionally placed into the MKY.IO.Serial namespace even though the file is located in
// MKY.IO.Serial\Socket for better separation of the implementation files.
namespace MKY.IO.Serial
{
	/// <summary></summary>
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

		private const int _MaxStartCycles = 3;

		private const int _MinConnectDelay = 50;
		private const int _MaxConnectDelay = 300;

		private const int _MinListenDelay = 50;
		private const int _MaxListenDelay = 300;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private static int _instanceCounter = 0;
		private static Random _random = new Random();

		private int _instanceId = 0;
		private bool _isDisposed;

		private System.Net.IPAddress _remoteIPAddress;
		private int _remotePort;
		private System.Net.IPAddress _localIPAddress;
		private int _localPort;

		private SocketState _state = SocketState.Reset;
		private object _stateSyncObj = new object();

		private int _startCycleCounter = 0;
		private object _startCycleCounterSyncObj = new object();

		private TcpClient _client;
		private TcpServer _server;

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
		public event EventHandler DataReceived;
		/// <summary></summary>
		public event EventHandler DataSent;
		/// <summary></summary>
		public event EventHandler<IORequestEventArgs> IORequest;
		/// <summary></summary>
		public event EventHandler<IOErrorEventArgs> IOError;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public TcpAutoSocket(System.Net.IPAddress remoteIPAddress, int remotePort, System.Net.IPAddress localIPAddress, int localPort)
		{
			_instanceId = _instanceCounter++;

			_remoteIPAddress = remoteIPAddress;
			_remotePort = remotePort;
			_localIPAddress = localIPAddress;
			_localPort = localPort;
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
			if (!_isDisposed)
			{
				if (disposing)
				{
					DisposeSockets();
				}
				_isDisposed = true;

				Debug.WriteLine(GetType() + " (" + _instanceId + ")(" + ToShortEndPointString() + "): Disposed.");
			}
		}

		/// <summary></summary>
		~TcpAutoSocket()
		{
			Dispose(false);
		}

		/// <summary></summary>
		protected bool IsDisposed
		{
			get { return (_isDisposed); }
		}

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (_isDisposed)
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
				return (_remoteIPAddress);
			}
		}

		/// <summary></summary>
		public virtual int RemotePort
		{
			get
			{
				AssertNotDisposed();
				return (_remotePort);
			}
		}

		/// <summary></summary>
		public virtual System.Net.IPAddress LocalIPAddress
		{
			get
			{
				AssertNotDisposed();
				return (_localIPAddress);
			}
		}

		/// <summary></summary>
		public virtual int LocalPort
		{
			get
			{
				AssertNotDisposed();
				return (_localPort);
			}
		}

		/// <summary></summary>
		public virtual bool IsStarted
		{
			get
			{
				AssertNotDisposed();
				switch (_state)
				{
					case SocketState.Starting:
					case SocketState.Connecting:
					case SocketState.ConnectingFailed:
					case SocketState.Connected:
					case SocketState.StartingListening:
					case SocketState.Listening:
					case SocketState.ListeningFailed:
					case SocketState.Accepted:
					case SocketState.Restarting:
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
				AssertNotDisposed();
				switch (_state)
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
		public virtual bool IsClient
		{
			get
			{
				AssertNotDisposed();
				switch (_state)
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
		public virtual bool IsServer
		{
			get
			{
				AssertNotDisposed();
				switch (_state)
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
		public virtual int BytesAvailable
		{
			get
			{
				AssertNotDisposed();

				if (IsClient)
					return (_client.BytesAvailable);
				else if (IsServer)
					return (_server.BytesAvailable);
				else
					return (0);
			}
		}

		/// <summary></summary>
		public virtual object UnderlyingIOInstance
		{
			get
			{
				AssertNotDisposed();

				if (IsClient)
					return (_client.UnderlyingIOInstance);
				else if (IsServer)
					return (_server.UnderlyingIOInstance);
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
			switch (_state)
			{
				case SocketState.Reset:
				case SocketState.Error:
				{
					StartAutoSocket();
					return (true);
				}
				default:
				{
					Debug.WriteLine(GetType() + " (" + _instanceId + ")(" + ToShortEndPointString() + "): Start() requested but state is " + _state + ".");
					return (false);
				}
			}
		}

		/// <summary></summary>
		public virtual void Stop()
		{
			AssertNotDisposed();
			StopAutoSocket();
		}

		/// <summary></summary>
		public virtual int Receive(out byte[] data)
		{
			AssertNotDisposed();

			if (IsClient)
				return (_client.Receive(out data));
			else if (IsServer)
				return (_server.Receive(out data));
			else
			{
				data = new byte[] { };
				return (0);
			}
		}

		/// <summary></summary>
		public virtual void Send(byte[] data)
		{
			AssertNotDisposed();

			if (IsClient)
				_client.Send(data);
			else if (IsServer)
				_server.Send(data);
		}

		#endregion

		#region State Methods
		//==========================================================================================
		// State Methods
		//==========================================================================================

		private void SetStateAndNotify(SocketState state)
		{
#if (DEBUG)
			SocketState oldState = _state;
#endif
			lock (_stateSyncObj)
				_state = state;
#if (DEBUG)
			Debug.WriteLine(GetType() + " (" + _instanceId + ")(" + ToShortEndPointString() + "): State has changed from " + oldState + " to " + _state + ".");
#endif
			OnIOChanged(new EventArgs());
		}

		#endregion

		#region Simple Socket Methods
		//==========================================================================================
		// Simple Socket Methods
		//==========================================================================================

		private void StopSockets()
		{
			// \remind
			// The ALAZ sockets by default stop synchronously. However, due to some other issues
			//   the ALAZ sockets had to be modified. The modified version stops asynchronously.
			if (_client != null)
				_client.Stop();
			if (_server != null)
				_server.Stop();
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
			lock (_startCycleCounterSyncObj)
				_startCycleCounter = 1;

			SetStateAndNotify(SocketState.Starting);
			StartConnecting();
		}

		// Try to start as client
		private void StartConnecting()
		{
			int delay = _random.Next(_MinConnectDelay, _MaxConnectDelay);
#if (FALSE)
			Debug.WriteLine(GetType() + " (" + _instanceId + ")(" + ToShortEndPointString() + "): Delaying connecting by " + delay);
#endif
			Thread.Sleep(delay);

			SetStateAndNotify(SocketState.Connecting);
			CreateClient(_remoteIPAddress, _remotePort);
			try
			{
				_client.Start(); // Client will be started asynchronously
			}
			catch
			{
				_client.Dispose();
				_client = null;

				StartListening();
			}
		}

		// try to start as server
		private void StartListening()
		{
			int delay = _random.Next(_MinListenDelay, _MaxListenDelay);
#if (FALSE)
			Debug.WriteLine(GetType() + " (" + _instanceId + ")(" + ToShortEndPointString() + "): Delaying listening by " + delay);
#endif
			Thread.Sleep(delay);

			SetStateAndNotify(SocketState.StartingListening);
			CreateServer(_localIPAddress, _localPort);
			try
			{
				_server.Start(); // Server will be started asynchronously
			}
			catch
			{
				_server.Dispose();
				_server = null;

				RequestTryAgain();
			}
		}

		private void RequestTryAgain()
		{
			bool tryAgain = false;
			lock (_startCycleCounterSyncObj)
			{
				_startCycleCounter++;
				if (_startCycleCounter <= _MaxStartCycles)
					tryAgain = true;
			}
			if (tryAgain)
			{
				Debug.WriteLine(GetType() + " (" + _instanceId + ")(" + ToShortEndPointString() + "): Trying connect cycle " + _startCycleCounter + ".");
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
			SetStateAndNotify(SocketState.Restarting);

			// \remind
			// The ALAZ sockets by default stop synchronously. However, due to some other issues
			//   the ALAZ sockets had to be modified. The modified version stops asynchronously.
			StopSockets();
			// \remind
			//DisposeSockets();

			StartAutoSocket();
		}

		private void StopAutoSocket()
		{
			SetStateAndNotify(SocketState.Stopping);

			// \remind
			// The ALAZ sockets by default stop synchronously. However, due to some other issues
			//   the ALAZ sockets had to be modified. The modified version stops asynchronously.
			StopSockets();
			// \remind
			//DisposeSockets();

			SetStateAndNotify(SocketState.Reset);
		}

		private void AutoSocketError(string message)
		{
			DisposeSockets();

			SetStateAndNotify(SocketState.Error);
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
			_client = new TcpClient(_remoteIPAddress, _remotePort);

			_client.IOChanged    += new EventHandler(_client_IOChanged);
			_client.IOError      += new EventHandler<IOErrorEventArgs>(_client_IOError);
			_client.DataReceived += new EventHandler(_client_DataReceived);
			_client.DataSent     += new EventHandler(_client_DataSent);
		}

		private void DestroyClient()
		{
			if (_client != null)
			{
				_client.IOChanged    -= new EventHandler(_client_IOChanged);
				_client.IOError      -= new EventHandler<IOErrorEventArgs>(_client_IOError);
				_client.DataReceived -= new EventHandler(_client_DataReceived);
				_client.DataSent     -= new EventHandler(_client_DataSent);

				_client.Dispose();
				_client = null;
			}
		}

		#endregion

		#region Client > Events
		//------------------------------------------------------------------------------------------
		// Client > Events
		//------------------------------------------------------------------------------------------

		private void _client_IOChanged(object sender, EventArgs e)
		{
			switch (_state)
			{
				case SocketState.Connecting:
				{
					if (_client.IsConnected)          // If IO changed during startup,
					{                                 //   check for connected and change state
						SetStateAndNotify(SocketState.Connected);
					}
					break;
				}
				case SocketState.Connected:
				{
					if (_client.IsConnected)          // If IO changed during client operation
					{                                 //   and client is connected to a server,
						OnIOChanged(e);               //   simply forward the event
					}
					else
					{
						DisposeSockets();             // If client lost connection to server,
						StartListening();             //   change to server operation
					}
					break;
				}
			}
		}

		private void _client_IOError(object sender, IOErrorEventArgs e)
		{
			switch (_state)
			{
				case SocketState.Connecting:
				case SocketState.ConnectingFailed:
				{
					DisposeSockets();                 // In case of error during startup,
					StartListening();                 //   try to start as server
					break;
				}
				case SocketState.Connected:
				{
					DisposeSockets();                 // In case of error during client operation,
					StartConnecting();                //   restart AutoSocket
					break;
				}
			}
		}

		private void _client_DataReceived(object sender, EventArgs e)
		{
			if (IsClient)
				OnDataReceived(e);
		}

		private void _client_DataSent(object sender, EventArgs e)
		{
			if (IsClient)
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
			_server = new TcpServer(_localIPAddress, _localPort);

			_server.IOChanged    += new EventHandler(_server_IOChanged);
			_server.IOError      += new EventHandler<IOErrorEventArgs>(_server_IOError);
			_server.DataReceived += new EventHandler(_server_DataReceived);
			_server.DataSent     += new EventHandler(_server_DataSent);
		}

		private void DestroyServer()
		{
			if (_server != null)
			{
				_server.IOChanged    -= new EventHandler(_server_IOChanged);
				_server.IOError      -= new EventHandler<IOErrorEventArgs>(_server_IOError);
				_server.DataReceived -= new EventHandler(_server_DataReceived);
				_server.DataSent     -= new EventHandler(_server_DataSent);

				_server.Dispose();
				_server = null;
			}
		}

		#endregion

		#region Server > Events
		//------------------------------------------------------------------------------------------
		// Server > Events
		//------------------------------------------------------------------------------------------

		private void _server_IOChanged(object sender, EventArgs e)
		{
			switch (_state)
			{
				case SocketState.StartingListening:
				{
					if (_server.IsStarted)                        // If IO changed during startup,
						SetStateAndNotify(SocketState.Listening); //   check for start and change state
					break;
				}
				case SocketState.Listening:
				{
					if (_server.ConnectedClientCount > 0)         // If IO changed during listening,
						SetStateAndNotify(SocketState.Accepted);  //   change state to accepted if
					break;                                        //   clients are connected
				}
				case SocketState.Accepted:
				{
					if (_server.ConnectedClientCount <= 0)        // If IO changed during accepted,
						SetStateAndNotify(SocketState.Listening); //   change state to listening if
					break;                                        //   no clients are connected
				}
			}
		}

		private void _server_IOError(object sender, IOErrorEventArgs e)
		{
			switch (_state)
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

		private void _server_DataReceived(object sender, EventArgs e)
		{
			if (IsServer)
				OnDataReceived(e);
		}

		private void _server_DataSent(object sender, EventArgs e)
		{
			if (IsServer)
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
			EventHelper.FireSync(IOControlChanged, this, e);
			throw (new NotSupportedException("Event not in use"));
		}

		/// <summary></summary>
		protected virtual void OnDataReceived(EventArgs e)
		{
			EventHelper.FireSync(DataReceived, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDataSent(EventArgs e)
		{
			EventHelper.FireSync(DataSent, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIORequest(IORequestEventArgs e)
		{
			EventHelper.FireSync(IORequest, this, e);
			throw (new NotSupportedException("Event not in use"));
		}

		/// <summary></summary>
		protected virtual void OnIOError(IOErrorEventArgs e)
		{
			EventHelper.FireSync<IOErrorEventArgs>(IOError, this, e);
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary></summary>
		public virtual string ToShortEndPointString()
		{
			return ("Server:" + _localPort + " / " + _remoteIPAddress + ":" + _remotePort);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
