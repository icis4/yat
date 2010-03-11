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

using MKY.Utilities.Event;

// The MKY.IO.Serial namespace combines various serial interface infrastructure. This code is
// intentionally placed into the MKY.IO.Serial namespace even though the file is located in
// MKY.IO.Serial\Socket for better separation of the implementation files.
namespace MKY.IO.Serial
{
	/// <summary></summary>
	public class TcpClient : IIOProvider, IDisposable, ALAZ.SystemEx.NetEx.SocketsEx.ISocketService
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private enum SocketState
		{
			Connecting,
			Connected,
			Disconnecting,
			Disconnected,
			WaitingForReconnect,
			Error,
		}

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private static int _instanceCounter = 0;
		private int _instanceId = 0;
		private bool _isDisposed;

		private System.Net.IPAddress _remoteIPAddress;
		private int _remotePort;
		private AutoRetry _autoReconnect;

		private SocketState _state = SocketState.Disconnected;
		private object _stateSyncObj = new object();

		private ALAZ.SystemEx.NetEx.SocketsEx.SocketClient _socket;
		private ALAZ.SystemEx.NetEx.SocketsEx.ISocketConnection _socketConnection;
		private object _socketConnectionSyncObj = new object();

		private Queue<byte> _receiveQueue = new Queue<byte>();

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
		public TcpClient(System.Net.IPAddress remoteIPAddress, int remotePort)
		{
			Initialize(remoteIPAddress, remotePort, new AutoRetry());
		}

		/// <summary></summary>
		public TcpClient(System.Net.IPAddress remoteIPAddress, int remotePort, AutoRetry autoReconnect)
		{
			Initialize(remoteIPAddress, remotePort, autoReconnect);
		}

		private void Initialize(System.Net.IPAddress remoteIPAddress, int remotePort, AutoRetry autoReconnect)
		{
			_instanceId = _instanceCounter++;

			_remoteIPAddress = remoteIPAddress;
			_remotePort = remotePort;
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
#if (DEBUG)
				System.Diagnostics.Debug.WriteLine(GetType() + "     (" + _instanceId + ")(               " + ToShortEndPointString() + "): Disposed");
#endif
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
		public System.Net.IPAddress RemoteIPAddress
		{
			get
			{
				AssertNotDisposed();
				return (_remoteIPAddress);
			}
		}

		/// <summary></summary>
		public int RemotePort
		{
			get
			{
				AssertNotDisposed();
				return (_remotePort);
			}
		}

		/// <summary></summary>
		public AutoRetry AutoReconnect
		{
			get
			{
				AssertNotDisposed();
				return (_autoReconnect);
			}
		}

		/// <summary></summary>
		public bool IsStarted
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

		/// <summary></summary>
		public bool IsOpen
		{
			get { return (IsConnected); }
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
				return (_receiveQueue.Count);
			}
		}

		private bool AutoReconnectEnabledAndAllowed
		{
			get
			{
				return
					(
						!IsDisposed && IsStarted && !IsOpen &&
						_autoReconnect.Enabled
					);
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
				System.Diagnostics.Debug.WriteLine(GetType() + "     (" + _instanceId + ")(               " + ToShortEndPointString() + "): Start() requested but state is " + _state);
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
		public int Receive(out byte[] data)
		{
			AssertNotDisposed();
		
			if (_receiveQueue.Count > 0)
			{
				lock (_receiveQueue)
				{
					int count = _receiveQueue.Count;
					data = new byte[count];
					for (int i = 0; i < count; i++)
						data[i] = _receiveQueue.Dequeue();
				}
			}
			else
			{
				data = new byte[] { };
			}
			return (data.Length);
		}

		/// <summary></summary>
		public void Send(byte[] data)
		{
			AssertNotDisposed();

			if (IsStarted)
			{
				if (_socketConnection != null)
					_socketConnection.BeginSend(data);
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
			System.Diagnostics.Debug.WriteLine(GetType() + "     (" + _instanceId + ")(               " + ToShortEndPointString() + "): State has changed from " + oldState + " to " + _state);
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
			if (_socket != null)
				DisposeSocket();

			SetStateAndNotify(SocketState.Connecting);

			_socket = new ALAZ.SystemEx.NetEx.SocketsEx.SocketClient(System.Net.Sockets.ProtocolType.Tcp,
																	 ALAZ.SystemEx.NetEx.SocketsEx.CallbackThreadType.ctWorkerThread,
																	(ALAZ.SystemEx.NetEx.SocketsEx.ISocketService)this,
																	 ALAZ.SystemEx.NetEx.SocketsEx.DelimiterType.dtNone, null,
																	 SocketDefaults.SocketBufferSize, SocketDefaults.MessageBufferSize,
																	 Timeout.Infinite, Timeout.Infinite);
			_socket.AddConnector("YAT TCP Client Connector", new System.Net.IPEndPoint(_remoteIPAddress, _remotePort));
			_socket.Start(); // The ALAZ socket will be started asynchronously
		}

		private void StopSocket()
		{
			// \remind
			// The ALAZ sockets by default stop synchronously. However, due to some other issues
			//   the ALAZ sockets had to be modified. The modified version stops asynchronously.
			_socket.Stop();
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
			lock (_socketConnectionSyncObj)
				_socketConnection = e.Connection;

			SetStateAndNotify(SocketState.Connected);

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
			lock (_receiveQueue)
			{
				foreach (byte b in e.Buffer)
					_receiveQueue.Enqueue(b);
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
			lock (_socketConnectionSyncObj)
				_socketConnection = null;

			if (AutoReconnectEnabledAndAllowed)
			{
				SetStateAndNotify(SocketState.WaitingForReconnect);
				StartReconnectTimer();
			}
			else
			{
				SetStateAndNotify(SocketState.Disconnected);
			}
		}

		/// <summary>
		/// Fired when exception occurs.
		/// </summary>
		/// <param name="e">
		/// Information about the exception and connection.
		/// </param>
		public void OnException(ALAZ.SystemEx.NetEx.SocketsEx.ExceptionEventArgs e)
		{
			if (AutoReconnectEnabledAndAllowed)
			{
				SetStateAndNotify(SocketState.WaitingForReconnect);
				StartReconnectTimer();
			}
			else
			{
				DisposeSocket();

				lock (_socketConnectionSyncObj)
					_socketConnection = null;

				SetStateAndNotify(SocketState.Error);
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
			if (_reconnectTimer != null)
				StopAndDisposeReconnectTimer();

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
			if (AutoReconnectEnabledAndAllowed)
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
			MKY.Utilities.Unused.PreventCompilerWarning(IOControlChanged);
			throw (new NotSupportedException("Event not in use"));
		}

		/// <summary></summary>
		protected virtual void OnIORequest(IORequestEventArgs e)
		{
			MKY.Utilities.Unused.PreventCompilerWarning(IORequest);
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
			return (_remoteIPAddress + ":" + _remotePort);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
