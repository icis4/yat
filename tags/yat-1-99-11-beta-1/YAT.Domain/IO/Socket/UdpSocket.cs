using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace HSR.YAT.Domain.IO
{
	public class UdpSocket : IIOProvider, IDisposable, HSR.Net.Sockets.ISocketService
	{
		//------------------------------------------------------------------------------------------
		// Types
		//------------------------------------------------------------------------------------------

		private enum SocketState
		{
			Opening,
			Open,
			Closing,
			Closed,
			Error,
		}

		//------------------------------------------------------------------------------------------
		// Attributes
		//------------------------------------------------------------------------------------------

		private bool _isDisposed = false;

		private System.Net.IPAddress _remoteIPAddress;
		private int _remotePort;
		private System.Net.IPAddress _localIPAddress;
		private int _localPort;

		private SocketState _state = SocketState.Closed;
		private object _stateSyncObj = new object();

		private Queue<byte> _receiveBuffer = new Queue<byte>();

		private HSR.Net.Sockets.SocketClient _socket;
		private HSR.Net.Sockets.ISocketConnection _socketConnection;
		private object _socketConnectionSyncObj = new object();

		//------------------------------------------------------------------------------------------
		// Events
		//------------------------------------------------------------------------------------------

		public event EventHandler IOChanged;
		public event EventHandler IOControlChanged;
		public event EventHandler<IOErrorEventArgs> IOError;
		public event EventHandler DataReceived;
		public event EventHandler DataSent;

		//------------------------------------------------------------------------------------------
		// Object Lifetime
		//------------------------------------------------------------------------------------------

		public UdpSocket(System.Net.IPAddress remoteIPAddress, int remotePort, System.Net.IPAddress localIPAddress, int localPort)
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
					DisposeSocket();
				}
				_isDisposed = true;
			}
		}

		/// <summary></summary>
		~UdpSocket()
		{
			Dispose(false);
		}

		protected bool IsDisposed
		{
			get { return (_isDisposed); }
		}

		protected void AssertNotDisposed()
		{
			if (_isDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed"));
		}

		#endregion

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		public bool HasStarted
		{
			get
			{
				AssertNotDisposed();
				switch (_state)
				{
					case SocketState.Opening:
					case SocketState.Open:
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

		public bool IsConnected
		{
			get
			{
				AssertNotDisposed();
				switch (_state)
				{
					case SocketState.Open:
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

		public int BytesAvailable
		{
			get
			{
				AssertNotDisposed();
				return (_receiveBuffer.Count);
			}
		}

		public object UnderlyingIOInstance
		{
			get
			{
				AssertNotDisposed();
				return (_socket);
			}
		}

		#endregion

		#region Methods
		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------

		public void Start()
		{
			AssertNotDisposed();

			if (!HasStarted)
				StartSocket();
		}

		public void Stop()
		{
			AssertNotDisposed();

			if (HasStarted)
				StopSocket();
		}

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
		//------------------------------------------------------------------------------------------
		// Simple Socket Methods
		//------------------------------------------------------------------------------------------

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
		//------------------------------------------------------------------------------------------
		// Socket Methods
		//------------------------------------------------------------------------------------------

		private void StartSocket()
		{
			_socket = new HSR.Net.Sockets.SocketClient(HSR.Net.Sockets.HostType.Udp, (HSR.Net.Sockets.ISocketService)this, null, 2048, 8192, 0, 0, Timeout.Infinite, Timeout.Infinite);
			_socket.OnException += new EventHandler<HSR.Net.Sockets.ExceptionEventArgs>(_socket_OnException);
			_socket.AddConnector(new System.Net.IPEndPoint(_remoteIPAddress, _remotePort),
								 new System.Net.IPEndPoint(System.Net.IPAddress.Any, _localPort));
			_socket.Start();

			lock (_stateSyncObj)
				_state = SocketState.Opening;
			
			OnIOChanged(new EventArgs());
		}

		private void StopSocket()
		{
			_socket.Stop();
			DisposeSocket();

			lock (_stateSyncObj)
				_state = SocketState.Closing;

			OnIOChanged(new EventArgs());
		}

		private void RestartSocket()
		{
			Stop();
			Start();
		}

		#endregion

		#region Socket Events
		//------------------------------------------------------------------------------------------
		// Socket Events
		//------------------------------------------------------------------------------------------

		private void _socket_OnException(object sender, HSR.Net.Sockets.ExceptionEventArgs e)
		{
			DisposeSocket();

			lock (_stateSyncObj)
				_state = SocketState.Error;

			OnIOChanged(new EventArgs());
			OnIOError(new IOErrorEventArgs(e.Exception.Message));
		}

		#endregion

		#region ISocketService Members
		//------------------------------------------------------------------------------------------
		// ISocketService Members
		//------------------------------------------------------------------------------------------

		public void OnConnected(HSR.Net.Sockets.ConnectionEventArgs e)
		{
			lock (_socketConnectionSyncObj)
				_socketConnection = e.Connection;

			lock (_stateSyncObj)
				_state = SocketState.Open;

			OnIOChanged(new EventArgs());

			// immediately begin receiving data
			e.Connection.BeginReceive();
		}

		public void OnReceived(HSR.Net.Sockets.MessageEventArgs e)
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

		public void OnSent(HSR.Net.Sockets.MessageEventArgs e)
		{
			// nothing to do
		}

		public void OnDisconnected(HSR.Net.Sockets.DisconnectedEventArgs e)
		{
			if (e.Exception == null)
			{
				// normal disconnect
				lock (_socketConnectionSyncObj)
					_socketConnection = null;

				lock (_stateSyncObj)
					_state = SocketState.Closed;

				OnIOChanged(new EventArgs());
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

		#region Event Invoking
		//------------------------------------------------------------------------------------------
		// Event Invoking
		//------------------------------------------------------------------------------------------

		protected virtual void OnIOChanged(EventArgs e)
		{
			Utilities.Event.EventHelper.FireSync(IOChanged, this, e);
		}

		protected virtual void OnIOControlChanged(EventArgs e)
		{
			Utilities.Event.EventHelper.FireSync(IOControlChanged, this, e);
		}

		protected virtual void OnIOError(IOErrorEventArgs e)
		{
			Utilities.Event.EventHelper.FireSync<IOErrorEventArgs>(IOError, this, e);
		}

		protected virtual void OnDataReceived(EventArgs e)
		{
			Utilities.Event.EventHelper.FireSync(DataReceived, this, e);
		}

		protected virtual void OnDataSent(EventArgs e)
		{
			Utilities.Event.EventHelper.FireSync(DataSent, this, e);
		}

		#endregion

	}
}