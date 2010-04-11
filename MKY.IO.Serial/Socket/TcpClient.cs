//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright � 2003-2004 HSR Hochschule f�r Technik Rapperswil.
// Copyright � 2003-2010 Matthias Kl�y.
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

		#region Static Fields
		//==========================================================================================
		// Static Fields
		//==========================================================================================

		private static int staticInstanceCounter = 0;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private int instanceId = 0;
		private bool isDisposed;

		private System.Net.IPAddress remoteIPAddress;
		private int remotePort;
		private AutoRetry autoReconnect;

		private SocketState state = SocketState.Disconnected;
		private object stateSyncObj = new object();

		private ALAZ.SystemEx.NetEx.SocketsEx.SocketClient socket;
		private ALAZ.SystemEx.NetEx.SocketsEx.ISocketConnection socketConnection;
		private object socketConnectionSyncObj = new object();

		private Queue<byte> receiveQueue = new Queue<byte>();

		private System.Timers.Timer reconnectTimer;

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
			this.instanceId = staticInstanceCounter++;

			this.remoteIPAddress = remoteIPAddress;
			this.remotePort = remotePort;
			this.autoReconnect = autoReconnect;
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
				if (disposing)
				{
					StopAndDisposeReconnectTimer();
					DisposeSocket();
				}
				this.isDisposed = true;

				Debug.WriteLine(GetType() + "     (" + this.instanceId + ")(               " + ToShortEndPointString() + "): Disposed.");
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
		public virtual AutoRetry AutoReconnect
		{
			get
			{
				AssertNotDisposed();
				return (this.autoReconnect);
			}
		}

		/// <summary></summary>
		public virtual bool IsStarted
		{
			get
			{
				AssertNotDisposed();
				switch (this.state)
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
				switch (this.state)
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
		public virtual bool IsConnected
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

		/// <summary></summary>
		public virtual bool IsOpen
		{
			get { return (IsConnected); }
		}

		/// <summary></summary>
		public virtual int BytesAvailable
		{
			get
			{
				AssertNotDisposed();
				return (this.receiveQueue.Count);
			}
		}

		private bool AutoReconnectEnabledAndAllowed
		{
			get
			{
				return
					(
						!IsDisposed && IsStarted && !IsOpen &&
						this.autoReconnect.Enabled
					);
			}
		}

		/// <summary></summary>
		public virtual object UnderlyingIOInstance
		{
			get
			{
				AssertNotDisposed();
				return (this.socket);
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

			if (!IsStarted)
			{
				StartSocket();
				return (true);
			}
			else
			{
				Debug.WriteLine(GetType() + "     (" + this.instanceId + ")(               " + ToShortEndPointString() + "): Start() requested but state is " + this.state + ".");
				return (false);
			}
		}

		/// <summary></summary>
		public virtual void Stop()
		{
			AssertNotDisposed();

			if (IsStarted)
				StopSocket();
		}

		/// <summary></summary>
		public virtual int Receive(out byte[] data)
		{
			AssertNotDisposed();
		
			if (this.receiveQueue.Count > 0)
			{
				lock (this.receiveQueue)
				{
					int count = this.receiveQueue.Count;
					data = new byte[count];
					for (int i = 0; i < count; i++)
						data[i] = this.receiveQueue.Dequeue();
				}
			}
			else
			{
				data = new byte[] { };
			}
			return (data.Length);
		}

		/// <summary></summary>
		public virtual void Send(byte[] data)
		{
			AssertNotDisposed();

			if (IsStarted)
			{
				if (this.socketConnection != null)
					this.socketConnection.BeginSend(data);
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
			SocketState oldState = this.state;
#endif
			lock (this.stateSyncObj)
				this.state = state;
#if (DEBUG)
			Debug.WriteLine(GetType() + "     (" + this.instanceId + ")(               " + ToShortEndPointString() + "): State has changed from " + oldState + " to " + this.state + ".");
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
			if (this.socket != null)
			{
				this.socket.Stop();
				this.socket.Dispose(); // Attention: ALAZ sockets don't properly stop on Dispose()
				this.socket = null;
				this.socketConnection = null;
			}
		}

		#endregion

		#region Socket Methods
		//==========================================================================================
		// Socket Methods
		//==========================================================================================

		private void StartSocket()
		{
			if (this.socket != null)
				DisposeSocket();

			SetStateAndNotify(SocketState.Connecting);

			this.socket = new ALAZ.SystemEx.NetEx.SocketsEx.SocketClient(System.Net.Sockets.ProtocolType.Tcp,
																	 ALAZ.SystemEx.NetEx.SocketsEx.CallbackThreadType.ctWorkerThread,
																	(ALAZ.SystemEx.NetEx.SocketsEx.ISocketService)this,
																	 ALAZ.SystemEx.NetEx.SocketsEx.DelimiterType.dtNone, null,
																	 SocketDefaults.SocketBufferSize, SocketDefaults.MessageBufferSize,
																	 Timeout.Infinite, Timeout.Infinite);
			this.socket.AddConnector("YAT TCP Client Connector", new System.Net.IPEndPoint(this.remoteIPAddress, this.remotePort));
			this.socket.Start(); // The ALAZ socket will be started asynchronously
		}

		private void StopSocket()
		{
			// \remind
			// The ALAZ sockets by default stop synchronously. However, due to some other issues
			//   the ALAZ sockets had to be modified. The modified version stops asynchronously.
			this.socket.Stop();
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
		public virtual void OnConnected(ALAZ.SystemEx.NetEx.SocketsEx.ConnectionEventArgs e)
		{
			lock (this.socketConnectionSyncObj)
				this.socketConnection = e.Connection;

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
		public virtual void OnReceived(ALAZ.SystemEx.NetEx.SocketsEx.MessageEventArgs e)
		{
			lock (this.receiveQueue)
			{
				foreach (byte b in e.Buffer)
					this.receiveQueue.Enqueue(b);
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
		public virtual void OnSent(ALAZ.SystemEx.NetEx.SocketsEx.MessageEventArgs e)
		{
			// Nothing to do
		}

		/// <summary>
		/// Fired when disconnected.
		/// </summary>
		/// <param name="e">
		/// Information about the connection.
		/// </param>
		public virtual void OnDisconnected(ALAZ.SystemEx.NetEx.SocketsEx.ConnectionEventArgs e)
		{
			lock (this.socketConnectionSyncObj)
				this.socketConnection = null;

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
		public virtual void OnException(ALAZ.SystemEx.NetEx.SocketsEx.ExceptionEventArgs e)
		{
			if (AutoReconnectEnabledAndAllowed)
			{
				SetStateAndNotify(SocketState.WaitingForReconnect);
				StartReconnectTimer();
			}
			else
			{
				DisposeSocket();

				lock (this.socketConnectionSyncObj)
					this.socketConnection = null;

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
			if (this.reconnectTimer != null)
				StopAndDisposeReconnectTimer();

			this.reconnectTimer = new System.Timers.Timer(this.autoReconnect.Interval);
			this.reconnectTimer.AutoReset = false;
			this.reconnectTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.reconnectTimer_Elapsed);
			this.reconnectTimer.Start();
		}

		private void StopAndDisposeReconnectTimer()
		{
			if (this.reconnectTimer != null)
			{
				this.reconnectTimer.Stop();
				this.reconnectTimer.Dispose();
				this.reconnectTimer = null;
			}
		}

		private void reconnectTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
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
			Utilities.Unused.PreventCompilerWarning(IOControlChanged);
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
			Utilities.Unused.PreventCompilerWarning(IORequest);
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
			return (this.remoteIPAddress + ":" + this.remotePort);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
