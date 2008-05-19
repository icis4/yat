using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using MKY.Utilities.Event;

namespace YAT.Domain.IO
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
			StartingListing,
			Listing,
			ListingFailed,
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

		private const int _MaximalStartCycles = 3;
		private const int _MinimalRestartWaitTimeInMs = 50;
		private const int _MaximalRestartWaitTimeInMs = 300;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _isDisposed;

		private System.Net.IPAddress _remoteIPAddress;
		private int _remotePort;
		private System.Net.IPAddress _localIPAddress;
		private int _localPort;

		private SocketState _state = SocketState.Reset;
		private object _stateSyncObj = new object();

		private int _startCycleCounter = 0;
		private object _startCycleCounterSyncObj = new object();
		private Random _waitRandom = new Random();

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
		public event EventHandler<IOErrorEventArgs> IOError;
		/// <summary></summary>
		public event EventHandler DataReceived;
		/// <summary></summary>
		public event EventHandler DataSent;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public TcpAutoSocket(System.Net.IPAddress remoteIPAddress, int remotePort, System.Net.IPAddress localIPAddress, int localPort)
		{
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
		public bool HasStarted
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
					case SocketState.StartingListing:
					case SocketState.Listing:
					case SocketState.ListingFailed:
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
		public bool IsConnected
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
		public bool IsClient
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
		public bool IsServer
		{
			get
			{
				AssertNotDisposed();
				switch (_state)
				{
					case SocketState.Listing:
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
		public int BytesAvailable
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
		public object UnderlyingIOInstance
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
		public void Start()
		{
			AssertNotDisposed();
			switch (_state)
			{
				case SocketState.Reset:
				case SocketState.Error:
				{
					StartAutoSocket();
					break;
				}
			}
		}

		/// <summary></summary>
		public void Stop()
		{
			AssertNotDisposed();
			StopAutoSocket();
		}

		/// <summary></summary>
		public int Receive(out byte[] buffer)
		{
			AssertNotDisposed();

			if (IsClient)
				return (_client.Receive(out buffer));
			else if (IsServer)
				return (_server.Receive(out buffer));
			else
			{
				buffer = new byte[] { };
				return (0);
			}
		}

		/// <summary></summary>
		public void Send(byte[] buffer)
		{
			AssertNotDisposed();

			if (IsClient)
				_client.Send(buffer);
			else if (IsServer)
				_server.Send(buffer);
		}

		#endregion

		#region Simple Socket Methods
		//==========================================================================================
		// Simple Socket Methods
		//==========================================================================================

		private void StopSockets()
		{
			if (IsClient)
				_client.Stop();
			else if (IsServer)
				_server.Stop();
		}

		private void DisposeSockets()
		{
			if (_client != null)
			{
				_client.Dispose();
				_client = null;
			}
			if (_server != null)
			{
				_server.Dispose();
				_server = null;
			}
		}

		#endregion

		#region AutoSocket State Transitions
		//==========================================================================================
		// AutoSocket State Transitions
		//==========================================================================================

		private void StartAutoSocket()
		{
			lock (_startCycleCounterSyncObj)
				_startCycleCounter = 0;

			_state = SocketState.Starting;
			OnIOChanged(new EventArgs());

			// immediately start connecting
			StartConnecting();
		}

		// try to start as client
		private void StartConnecting()
		{
			lock (_stateSyncObj)
				_state = SocketState.Connecting;

			OnIOChanged(new EventArgs());

			_client = new TcpClient(_remoteIPAddress, _remotePort, _localIPAddress);
			_client.IOChanged += new EventHandler(_client_IOChanged);
			_client.IOControlChanged += new EventHandler(_client_IOControlChanged);
			_client.IOError += new EventHandler<IOErrorEventArgs>(_client_IOError);
			_client.DataReceived += new EventHandler(_client_DataReceived);
			_client.DataSent += new EventHandler(_client_DataSent);

			try
			{
				_client.Start();
			}
			catch
			{
				if (_client != null)
					_client.Dispose();
				_client = null;

				lock (_stateSyncObj)
					_state = SocketState.Error;

				OnIOChanged(new EventArgs());
				OnIOError(new IOErrorEventArgs("AutoSocket client could not be created."));
			}
		}

		// try to start as server
		private void StartListening()
		{
			lock (_stateSyncObj)
				_state = SocketState.StartingListing;

			OnIOChanged(new EventArgs());

			_server = new TcpServer(_localIPAddress, _localPort);
			_server.IOChanged += new EventHandler(_server_IOChanged);
			_server.IOControlChanged += new EventHandler(_server_IOControlChanged);
			_server.IOError += new EventHandler<IOErrorEventArgs>(_server_IOError);
			_server.DataReceived += new EventHandler(_server_DataReceived);
			_server.DataSent += new EventHandler(_server_DataSent);

			try
			{
				_server.Start();
			}
			catch
			{
				_server.Dispose();
				_server = null;

				lock (_stateSyncObj)
					_state = SocketState.Error;

				OnIOChanged(new EventArgs());
				OnIOError(new IOErrorEventArgs("AutoSocket server could not be created."));
			}
		}

		private void RestartAutoSocket()
		{
			lock (_stateSyncObj)
				_state = SocketState.Restarting;

			OnIOChanged(new EventArgs());

			StopSockets();
			DisposeSockets();

			StartAutoSocket();
		}

		private void StopAutoSocket()
		{
			lock (_stateSyncObj)
				_state = SocketState.Stopping;

			OnIOChanged(new EventArgs());

			StopSockets();
			DisposeSockets();

			lock (_stateSyncObj)
				_state = SocketState.Reset;

			OnIOChanged(new EventArgs());
		}

		private void AutoSocketError(string message)
		{
			DisposeSockets();

			lock (_stateSyncObj)
				_state = SocketState.Error;

			OnIOChanged(new EventArgs());
			OnIOError(new IOErrorEventArgs(message));
		}

		#endregion

		#region Client Events
		//==========================================================================================
		// Client Events
		//==========================================================================================

		private void _client_IOChanged(object sender, EventArgs e)
		{
			switch (_state)
			{
				case SocketState.Connecting:
				{
					if (_client.IsConnected)               // if IO changed during startup,
					{                                      //   check for connected and change state
						lock (_stateSyncObj)
							_state = SocketState.Connected;

						OnIOChanged(e);
					}
					break;
				}
				case SocketState.Connected:
				{
					if (_client.IsConnected)               // if IO changed during client operation
					{                                      //   and client is connected to a server,
						OnIOChanged(e);                    //   simply forward the event
					}
					else
					{
						DisposeSockets();                  // if client lost connection to server,
						StartListening();                  //   change to server operation
					}
					break;
				}
			}
		}

		private void _client_IOControlChanged(object sender, EventArgs e)
		{
			if (IsClient)
				OnIOControlChanged(e);
		}

		private void _client_IOError(object sender, IOErrorEventArgs e)
		{
			switch (_state)
			{
				case SocketState.Connecting:
				case SocketState.ConnectingFailed:
				{
					DisposeSockets();                      // in case of error during startup,
					StartListening();                      //   try to start as server
					break;
				}
				case SocketState.Connected:
				{
					DisposeSockets();                      // in case of error during client operation,
					StartConnecting();                     //   restart AutoSocket
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

		#region Server Events
		//==========================================================================================
		// Server Events
		//==========================================================================================

		private void _server_IOChanged(object sender, EventArgs e)
		{
			switch (_state)
			{
				case SocketState.StartingListing:
				{
					if (_server.HasStarted)                // if IO changed during startup,
					{                                      //   check for start and change state
						lock (_stateSyncObj)
							_state = SocketState.Listing;

						OnIOChanged(e);
					}
					break;
				}
				case SocketState.Listing:
				{
					if (_server.ConnectedClientCount > 0)  // if IO changed during listening,
					{                                      //   change state to accepted if
						lock (_stateSyncObj)               //   clients are connected
							_state = SocketState.Accepted;

						OnIOChanged(e);
					}
					break;
				}
				case SocketState.Accepted:
				{
					if (_server.ConnectedClientCount <= 0) // if IO changed during accepted,
					{                                      //   change state to listening if
						lock (_stateSyncObj)               //   no clients are connected
							_state = SocketState.Listing;

						OnIOChanged(e);
					}
					break;
				}
			}
		}

		private void _server_IOControlChanged(object sender, EventArgs e)
		{
			if (IsServer)
				OnIOControlChanged(e);
		}

		private void _server_IOError(object sender, IOErrorEventArgs e)
		{
			switch (_state)
			{
				case SocketState.StartingListing:          // in case of error during startup,
				{                                          //   increment start cycles and
					bool tryAgain = false;                 //   continue depending on count
					lock (_startCycleCounterSyncObj)
					{
						_startCycleCounter++;
						if (_startCycleCounter < (_MaximalStartCycles - 1))
							tryAgain = true;
					}
					if (tryAgain)
					{
						Thread.Sleep(_waitRandom.Next(_MinimalRestartWaitTimeInMs, _MaximalRestartWaitTimeInMs));
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
					break;
				}
				case SocketState.Listing:
				case SocketState.Accepted:
				{
					DisposeSockets();                      // in case of error during server operation,
					StartConnecting();                     //   restart AutoSocket
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
		}

		/// <summary></summary>
		protected virtual void OnIOError(IOErrorEventArgs e)
		{
			EventHelper.FireSync<IOErrorEventArgs>(IOError, this, e);
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

		#endregion
	}
}
