using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using MKY.Utilities.Event;

namespace YAT.Domain.IO
{
	/// <summary></summary>
	public class TcpClient : IIOProvider, IDisposable, MKY.Net.Sockets.ISocketService
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private enum SocketState
		{
			Disconnected,
			Connecting,
			Connected,
			WaitingForReconnect,
			Error,
		}

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _isDisposed;

		private System.Net.IPAddress _remoteIPAddress;
		private int _remotePort;
		private System.Net.IPAddress _localIPAddress;
		private AutoRetry _autoReconnect;

		private SocketState _state = SocketState.Disconnected;
		private object _stateSyncObj = new object();

		private Queue<byte> _receiveBuffer = new Queue<byte>();

		private MKY.Net.Sockets.SocketClient _socket;
		private MKY.Net.Sockets.ISocketConnection _socketConnection;
		private object _socketConnectionSyncObj = new object();

		private System.Timers.Timer _reconnectTimer;

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
		public TcpClient(System.Net.IPAddress remoteIPAddress, int remotePort, System.Net.IPAddress localIPAddress)
		{
			_remoteIPAddress = remoteIPAddress;
			_remotePort = remotePort;
			_localIPAddress = localIPAddress;
			_autoReconnect = new AutoRetry();
		}

		/// <summary></summary>
		public TcpClient(System.Net.IPAddress remoteIPAddress, int remotePort, System.Net.IPAddress localIPAddress, AutoRetry autoReconnect)
		{
			_remoteIPAddress = remoteIPAddress;
			_remotePort = remotePort;
			_localIPAddress = localIPAddress;
			_autoReconnect = autoReconnect;
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
					StopAndDisposeReconnectTimer();
					DisposeSocket();
				}
				_isDisposed = true;
			}
		}

		/// <summary></summary>
		~TcpClient()
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
					case SocketState.Connecting:
					case SocketState.Connected:
					case SocketState.WaitingForReconnect:
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

		private bool IsConnectedOrConnecting
		{
			get
			{
				AssertNotDisposed();
				switch (_state)
				{
					case SocketState.Connecting:
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
		public bool IsConnected
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
		public int BytesAvailable
		{
			get
			{
				AssertNotDisposed();
				return (_receiveBuffer.Count);
			}
		}

		/// <summary></summary>
		public object UnderlyingIOInstance
		{
			get
			{
				AssertNotDisposed();
				return (_socket);
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

			if (!HasStarted)
				StartSocket();
		}

		/// <summary></summary>
		public void Stop()
		{
			AssertNotDisposed();

			if (HasStarted)
				StopSocket();
		}

		/// <summary></summary>
		public int Receive(out byte[] buffer)
		{
			AssertNotDisposed();
		
			if (_receiveBuffer.Count > 0)
			{
				lock (_receiveBuffer)
				{
					int count = _receiveBuffer.Count;
					buffer = new byte[count];
					for (int i = 0; i < count; i++)
						buffer[i] = _receiveBuffer.Dequeue();
				}
			}
			else
			{
				buffer = new byte[] { };
			}
			return (buffer.Length);
		}

		/// <summary></summary>
		public void Send(byte[] buffer)
		{
			AssertNotDisposed();

			if (HasStarted)
			{
				if (_socketConnection != null)
					_socketConnection.BeginSend(buffer);
			}
		}

		#endregion

		#region Simple Socket Methods
		//==========================================================================================
		// Simple Socket Methods
		//==========================================================================================

		private void DisposeSocket()
		{
			if (_socket != null)
			{
				_socket.Dispose();
				_socket = null;
				_socketConnection = null;
			}
		}

		#endregion

		#region Socket Methods
		//==========================================================================================
		// Socket Methods
		//==========================================================================================

		private void StartSocket()
		{
			_socket = new MKY.Net.Sockets.SocketClient((MKY.Net.Sockets.ISocketService)this, null, 2048, 8192, 0, 0, Timeout.Infinite, Timeout.Infinite);
			_socket.OnException += new EventHandler<MKY.Net.Sockets.ExceptionEventArgs>(_socket_OnException);
			_socket.AddConnector(new System.Net.IPEndPoint(_remoteIPAddress, _remotePort));
			_socket.Start();
			
			lock (_stateSyncObj)
				_state = SocketState.Connecting;
			
			OnIOChanged(new EventArgs());
		}

		private void StopSocket()
		{
			_socket.Stop();
			DisposeSocket();
			
			lock (_stateSyncObj)
				_state = SocketState.Disconnected;
			
			OnIOChanged(new EventArgs());
		}

		private void RestartSocket()
		{
			Stop();
			Start();
		}

		#endregion

		#region Socket Events
		//==========================================================================================
		// Socket Events
		//==========================================================================================

		private void _socket_OnException(object sender, MKY.Net.Sockets.ExceptionEventArgs e)
		{
			if (_autoReconnect.Enabled)
			{
				lock (_stateSyncObj)
					_state = SocketState.WaitingForReconnect;
			
				OnIOChanged(new EventArgs());

				StartReconnectTimer();
			}
			else
			{
				DisposeSocket();

				lock (_stateSyncObj)
					_state = SocketState.Error;
				
				OnIOChanged(new EventArgs());
				OnIOError(new IOErrorEventArgs(e.Exception.Message));
			}
		}

		#endregion

		#region ISocketService Members
		//==========================================================================================
		// ISocketService Members
		//==========================================================================================

		/// <summary></summary>
		public void OnConnected(MKY.Net.Sockets.ConnectionEventArgs e)
		{
			lock (_socketConnectionSyncObj)
				_socketConnection = e.Connection;
			
			lock (_stateSyncObj)
				_state = SocketState.Connected;

			OnIOChanged(new EventArgs());

			// immediately begin receiving data
			e.Connection.BeginReceive();
		}

		/// <summary></summary>
		public void OnReceived(MKY.Net.Sockets.MessageEventArgs e)
		{
			lock (_receiveBuffer)
			{
				foreach (byte b in e.Buffer)
					_receiveBuffer.Enqueue(b);
			}
			OnDataReceived(new EventArgs());

			// continue receiving data
			e.Connection.BeginReceive();
		}

		/// <summary></summary>
		public void OnSent(MKY.Net.Sockets.MessageEventArgs e)
		{
			// nothing to do
		}

		/// <summary></summary>
		public void OnDisconnected(MKY.Net.Sockets.DisconnectedEventArgs e)
		{
			if (e.Exception == null)
			{
				// normal disconnect
				lock (_socketConnectionSyncObj)
					_socketConnection = null;

				if (_autoReconnect.Enabled)
				{
					lock (_stateSyncObj)
						_state = SocketState.WaitingForReconnect;

					OnIOChanged(new EventArgs());

					StartReconnectTimer();
				}
				else
				{
					lock (_stateSyncObj)
						_state = SocketState.Disconnected;

					OnIOChanged(new EventArgs());
				}
			}
			else
			{
				// exception disconnect
				DisposeSocket();

				lock (_socketConnectionSyncObj)
					_socketConnection = null;

				lock (_stateSyncObj)
					_state = SocketState.Error;

				OnIOChanged(new EventArgs());
				OnIOError(new IOErrorEventArgs(e.Exception.Message));
			}
		}

		#endregion

		#region Reconnect Timer
		//==========================================================================================
		// Reconnect Timer
		//==========================================================================================

		private void StartReconnectTimer()
		{
			_reconnectTimer = new System.Timers.Timer(_autoReconnect.Interval);
			_reconnectTimer.AutoReset = false;
			_reconnectTimer.Elapsed += new System.Timers.ElapsedEventHandler(_reconnectTimer_Elapsed);
			_reconnectTimer.Start();
		}

		private void StopAndDisposeReconnectTimer()
		{
			if (_reconnectTimer != null)
			{
				_reconnectTimer.Stop();
				_reconnectTimer.Dispose();
				_reconnectTimer = null;
			}
		}

		private void _reconnectTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (!IsDisposed && HasStarted && !IsConnectedOrConnecting)
			{
				try
				{
					StartSocket();
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
