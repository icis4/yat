using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using MKY.Utilities.Event;

namespace MKY.IO.Serial.Socket
{
	/// <summary></summary>
	public class TcpServer : IIOProvider, IDisposable, MKY.Net.Sockets.ISocketService
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private enum SocketState
		{
			Reset,
			Listening,
			Accepted,
			Stopping,
			Error,
		}

		#endregion

		#region Delegates
		//==========================================================================================
		// Delegates
		//==========================================================================================

		/// <summary></summary>
		public delegate void AsyncStopSocketCaller();

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _isDisposed;

		private System.Net.IPAddress _localIPAddress;
		private int _localPort;

		private SocketState _state = SocketState.Reset;
		private object _stateSyncObj = new object();

		private Queue<byte> _receiveBuffer = new Queue<byte>();

		private MKY.Net.Sockets.SocketServer _socket;
		private List<MKY.Net.Sockets.ISocketConnection> _socketConnections = new List<MKY.Net.Sockets.ISocketConnection>();

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
		public TcpServer(System.Net.IPAddress localIPAddress, int localPort)
		{
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
		~TcpServer()
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
		public bool IsConnected
		{
			get
			{
				AssertNotDisposed();
				switch (_state)
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
		public int ConnectedClientCount
		{
			get
			{
				AssertNotDisposed();
				return (_socketConnections.Count);
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
				foreach (MKY.Net.Sockets.ISocketConnection connection in _socketConnections)
					connection.BeginSend(buffer);
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
				_socketConnections.Clear();
			}
		}

		#endregion

		#region Socket Methods
		//==========================================================================================
		// Socket Methods
		//==========================================================================================

		private void StartSocket()
		{
			_socket = new MKY.Net.Sockets.SocketServer((MKY.Net.Sockets.ISocketService)this, null, 2048, 8192, 0, 0, Timeout.Infinite, Timeout.Infinite);
			_socket.OnException += new EventHandler<MKY.Net.Sockets.ExceptionEventArgs>(_socket_OnException);
			_socket.AddListener(new System.Net.IPEndPoint(System.Net.IPAddress.Any, _localPort));
			_socket.Start();

			lock (_stateSyncObj)
				_state = SocketState.Listening;

			OnIOChanged(new EventArgs());
		}

		private void StopSocket()
		{
			lock (_stateSyncObj)
				_state = SocketState.Stopping;

			_socket.Stop();
			DisposeSocket();

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
			lock (_stateSyncObj)
				_state = SocketState.Error;

			OnIOChanged(new EventArgs());
			OnIOError(new IOErrorEventArgs(e.Exception.Message));
		}

		#endregion

		#region ISocketService Members
		//==========================================================================================
		// ISocketService Members
		//==========================================================================================

		/// <summary></summary>
		public void OnConnected(MKY.Net.Sockets.ConnectionEventArgs e)
		{
			lock (_socketConnections)
				_socketConnections.Add(e.Connection);

			lock (_stateSyncObj)
				_state = SocketState.Accepted;

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
				bool isConnected = false;
				lock (_socketConnections)
				{
					_socketConnections.Remove(e.Connection);
					isConnected = (_socketConnections.Count > 0);
				}
				if (!isConnected)
				{
					lock (_stateSyncObj)
						_state = SocketState.Listening;

					OnIOChanged(new EventArgs());
				}
			}
			else
			{
				// exception disconnect
				DisposeSocket();

				lock (_stateSyncObj)
					_state = SocketState.Error;

				OnIOChanged(new EventArgs());
				OnIOError(new IOErrorEventArgs(e.Exception.Message));
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
