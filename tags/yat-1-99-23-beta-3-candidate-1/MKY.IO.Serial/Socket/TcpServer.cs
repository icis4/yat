//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright � 2003-2004 HSR Hochschule f�r Technik Rapperswil.
// Copyright � 2003-2009 Matthias Kl�y.
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
	public class TcpServer : IIOProvider, IDisposable, ALAZ.SystemEx.NetEx.SocketsEx.ISocketService
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

		private static int _instanceCounter = 0;
		private int _instanceId = 0;
		private bool _isDisposed;

		private System.Net.IPAddress _localIPAddress;
		private int _localPort;

		private SocketState _state = SocketState.Reset;
		private object _stateSyncObj = new object();

		private Queue<byte> _receiveBuffer = new Queue<byte>();

		private ALAZ.SystemEx.NetEx.SocketsEx.SocketServer _socket;
		private List<ALAZ.SystemEx.NetEx.SocketsEx.ISocketConnection> _socketConnections = new List<ALAZ.SystemEx.NetEx.SocketsEx.ISocketConnection>();

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
		public TcpServer(System.Net.IPAddress localIPAddress, int localPort)
		{
			_instanceId = _instanceCounter++;

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
#if (DEBUG)
				System.Diagnostics.Debug.WriteLine(GetType() + "     (" + _instanceId + ")(" + ToShortEndPointString() + "                  ): Disposed");
#endif
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
		public bool IsStarted
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

			if (!IsStarted)
				StartSocket();
#if (DEBUG)
			else
				System.Diagnostics.Debug.WriteLine(GetType() + "     (" + _instanceId + ")(" + ToShortEndPointString() + "                  ): Start() requested but state is " + _state);
#endif
		}

		/// <summary></summary>
		public void Stop()
		{
			AssertNotDisposed();

			if (IsStarted)
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

			if (IsStarted)
			{
				foreach (ALAZ.SystemEx.NetEx.SocketsEx.ISocketConnection connection in _socketConnections)
					connection.BeginSend(buffer);
			}
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
			System.Diagnostics.Debug.WriteLine(GetType() + "     (" + _instanceId + ")(" + ToShortEndPointString() + "                  ): State has changed from " + oldState + " to " + _state);
#endif
			OnIOChanged(new EventArgs());
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
				_socket.Stop();
				_socket.Dispose(); // Attention: ALAZ sockets don't properly stop on Dispose()
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
			SetStateAndNotify(SocketState.Listening);

			_socket = new ALAZ.SystemEx.NetEx.SocketsEx.SocketServer(System.Net.Sockets.ProtocolType.Tcp,
																	 ALAZ.SystemEx.NetEx.SocketsEx.CallbackThreadType.ctWorkerThread,
																	(ALAZ.SystemEx.NetEx.SocketsEx.ISocketService)this,
																	 ALAZ.SystemEx.NetEx.SocketsEx.DelimiterType.dtNone, null,
																	 SocketDefaults.SocketBufferSize, SocketDefaults.MessageBufferSize,
																	 Timeout.Infinite, Timeout.Infinite);
			_socket.AddListener("YAT TCP Server Listener", new System.Net.IPEndPoint(System.Net.IPAddress.Any, _localPort));
			_socket.Start(); // The ALAZ socket will be started asynchronously
		}

		private void StopSocket()
		{
			// \remind
			// The ALAZ sockets by default stop synchronously. However, due to some other issues
			//   the ALAZ sockets had to be modified. The modified version stops asynchronously.
			_socket.Stop();
		}

		private void RestartSocket()
		{
			Stop();
			Start();
		}

		#endregion

		#region ISocketService Members
		//==========================================================================================
		// ISocketService Members
		//==========================================================================================

		/// <summary>
		/// Fired when connected.
		/// </summary>
		/// <param name="e">
		/// Information about the connection.
		/// </param>
		public void OnConnected(ALAZ.SystemEx.NetEx.SocketsEx.ConnectionEventArgs e)
		{
			lock (_socketConnections)
				_socketConnections.Add(e.Connection);

			SetStateAndNotify(SocketState.Accepted);

			// Immediately begin receiving data
			e.Connection.BeginReceive();
		}

		/// <summary>
		/// Fired when data arrives.
		/// </summary>
		/// <param name="e">
		/// Information about the Message.
		/// </param>
		public void OnReceived(ALAZ.SystemEx.NetEx.SocketsEx.MessageEventArgs e)
		{
			lock (_receiveBuffer)
			{
				foreach (byte b in e.Buffer)
					_receiveBuffer.Enqueue(b);
			}
			OnDataReceived(new EventArgs());

			// Continue receiving data
			e.Connection.BeginReceive();
		}

		/// <summary>
		/// Fired when data is sent.
		/// </summary>
		/// <param name="e">
		/// Information about the Message.
		/// </param>
		public void OnSent(ALAZ.SystemEx.NetEx.SocketsEx.MessageEventArgs e)
		{
			// Nothing to do
		}

		/// <summary>
		/// Fired when disconnected.
		/// </summary>
		/// <param name="e">
		/// Information about the connection.
		/// </param>
		public void OnDisconnected(ALAZ.SystemEx.NetEx.SocketsEx.ConnectionEventArgs e)
		{
			bool isConnected = false;
			lock (_socketConnections)
			{
				_socketConnections.Remove(e.Connection);
				isConnected = (_socketConnections.Count > 0);
			}

			if (!isConnected)
				SetStateAndNotify(SocketState.Listening);
		}

		/// <summary>
		/// Fired when exception occurs.
		/// </summary>
		/// <param name="e">
		/// Information about the exception and connection.
		/// </param>
		public void OnException(ALAZ.SystemEx.NetEx.SocketsEx.ExceptionEventArgs e)
		{
			DisposeSocket();

			SetStateAndNotify(SocketState.Error);
			OnIOError(new IOErrorEventArgs(e.Exception.Message));
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
			return ("Server:" + _localPort);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================