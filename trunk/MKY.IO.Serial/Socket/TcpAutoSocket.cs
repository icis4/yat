//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2009 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using MKY.Utilities.Event;

// The MKY.IO.Serial namespace combines serial port and socket infrastructure. This code is
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
		private const int _MinRestartWaitTime = 50;
		private const int _MaxRestartWaitTime = 300;

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
		public event EventHandler<IORequestEventArgs> IORequest;
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
		public bool IsStarted
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
		public bool IsOpen
		{
			get { return (IsConnected); }
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
			System.Diagnostics.Debug.WriteLine(GetType() + " (" + ToShortEndPointString() + "): State has changed from " + oldState + " to " + _state);
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

			SetStateAndNotify(SocketState.Starting);

			// immediately start connecting
			StartConnecting();
		}

		// try to start as client
		private void StartConnecting()
		{
			SetStateAndNotify(SocketState.Connecting);

			_client = new TcpClient(_remoteIPAddress, _remotePort);
			_client.IOChanged += new EventHandler(_client_IOChanged);
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

				SetStateAndNotify(SocketState.Error);
				OnIOError(new IOErrorEventArgs("AutoSocket client could not be created."));
			}
		}

		// try to start as server
		private void StartListening()
		{
			SetStateAndNotify(SocketState.StartingListening);

			_server = new TcpServer(_localIPAddress, _localPort);
			_server.IOChanged += new EventHandler(_server_IOChanged);
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

				SetStateAndNotify(SocketState.Error);
				OnIOError(new IOErrorEventArgs("AutoSocket server could not be created."));
			}
		}

		private void RestartAutoSocket()
		{
			SetStateAndNotify(SocketState.Restarting);

			StopSockets();
			DisposeSockets();

			StartAutoSocket();
		}

		private void StopAutoSocket()
		{
			SetStateAndNotify(SocketState.Stopping);

			StopSockets();
			DisposeSockets();

			SetStateAndNotify(SocketState.Reset);
		}

		private void AutoSocketError(string message)
		{
			DisposeSockets();

			SetStateAndNotify(SocketState.Error);
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

		#region Server Events
		//==========================================================================================
		// Server Events
		//==========================================================================================

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
					bool tryAgain = false;            //   continue depending on count
					lock (_startCycleCounterSyncObj)
					{
						_startCycleCounter++;
						if (_startCycleCounter < (_MaxStartCycles - 1))
							tryAgain = true;
					}
					if (tryAgain)
					{
						Thread.Sleep(_waitRandom.Next(_MinRestartWaitTime, _MaxRestartWaitTime));
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

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary></summary>
		public string ToShortEndPointString()
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
