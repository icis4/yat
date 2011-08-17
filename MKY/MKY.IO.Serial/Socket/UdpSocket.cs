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
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;

using MKY.Event;

// The MKY.IO.Serial namespace combines various serial interface infrastructure. This code is
// intentionally placed into the MKY.IO.Serial namespace even though the file is located in
// MKY.IO.Serial\Socket for better separation of the implementation files.
namespace MKY.IO.Serial
{
	/// <summary></summary>
	public class UdpSocket : IIOProvider, IDisposable
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private enum SocketState
		{
			Opening,
			Opened,
			Closing,
			Closed,
			Error,
		}

		private class AsyncReceiveState
		{
			public readonly System.Net.IPEndPoint EndPoint;
			public readonly System.Net.Sockets.UdpClient Socket;

			public AsyncReceiveState(System.Net.IPEndPoint endPoint, System.Net.Sockets.UdpClient socket)
			{
				this.EndPoint = endPoint;
				this.Socket = socket;
			}
		}

		#endregion

		#region Static Fields
		//==========================================================================================
		// Static Fields
		//==========================================================================================

		private static int staticInstanceCounter;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private int instanceId;
		private bool isDisposed;

		private System.Net.IPAddress remoteIPAddress;
		private int remotePort;
		private int localPort;

		private SocketState state = SocketState.Closed;
		private object stateSyncObj = new object();

		private System.Net.IPEndPoint endPoint;
		private System.Net.Sockets.UdpClient socket;

		private Queue<byte> receiveQueue = new Queue<byte>();

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
		public UdpSocket(System.Net.IPAddress remoteIPAddress, int remotePort, int localPort)
		{
			this.instanceId = staticInstanceCounter++;

			this.remoteIPAddress = remoteIPAddress;
			this.remotePort = remotePort;
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
				if (disposing)
				{
					CloseAndDisposeSocket();
				}
				this.isDisposed = true;

				Debug.WriteLine(GetType() + "     (" + this.instanceId + ")(" + ToShortEndPointString() + "): Disposed.");
			}
		}

		/// <summary></summary>
		~UdpSocket()
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
					case SocketState.Closed:
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
			get
			{
				AssertNotDisposed();
				switch (this.state)
				{
					case SocketState.Opening:
					case SocketState.Opened:
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
			get
			{
				AssertNotDisposed();
				switch (this.state)
				{
					case SocketState.Opened:
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
			get { return (IsOpen); }
		}

		/// <summary></summary>
		public virtual bool IsReadyToSend
		{
			get { return (IsOpen); }
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
				Debug.WriteLine(GetType() + "     (" + this.instanceId + ")(" + ToShortEndPointString() + "): Start() requested but state is " + this.state + ".");
				return (false);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "Stop is a common term to start/stop something.")]
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
				this.socket.Send(data, data.Length);
				OnDataSent(new EventArgs());
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

			lock (this.stateSyncObj)
				state = this.state;

			return (state);
		}

		private void SetStateSynchronizedAndNotify(SocketState state)
		{
#if (DEBUG)
			SocketState oldState = this.state;
#endif
			lock (this.stateSyncObj)
				this.state = state;
#if (DEBUG)
			Debug.WriteLine(GetType() + "     (" + this.instanceId + ")(" + ToShortEndPointString() + "): State has changed from " + oldState + " to " + this.state + ".");
#endif
			OnIOChanged(new EventArgs());
		}

		#endregion

		#region Socket Methods
		//==========================================================================================
		// Socket Methods
		//==========================================================================================

		private void StartSocket()
		{
			SetStateSynchronizedAndNotify(SocketState.Opening);

			this.endPoint = new System.Net.IPEndPoint(this.remoteIPAddress, this.remotePort);
			this.socket = new System.Net.Sockets.UdpClient(this.localPort);
			this.socket.Connect(this.endPoint);

			SetStateSynchronizedAndNotify(SocketState.Opened);

			// Immediately begin receiving data.
			BeginReceive();
		}

		private void StopSocket()
		{
			SetStateSynchronizedAndNotify(SocketState.Closing);
			CloseAndDisposeSocket();
			SetStateSynchronizedAndNotify(SocketState.Closed);
		}

		private void CloseAndDisposeSocket()
		{
			this.socket.Close();
			this.socket = null;
		}

		private void SocketError()
		{
			CloseAndDisposeSocket();
			SetStateSynchronizedAndNotify(SocketState.Error);
		}

		#endregion

		#region Async Receive
		//==========================================================================================
		// Async Receive
		//==========================================================================================

		private void BeginReceive()
		{
			AsyncReceiveState state = new AsyncReceiveState(this.endPoint, this.socket);
			this.socket.BeginReceive(new AsyncCallback(ReceiveCallback), state);
		}

		private void ReceiveCallback(IAsyncResult ar)
		{
			AsyncReceiveState state = (AsyncReceiveState)(ar.AsyncState);
			System.Net.IPEndPoint endPoint = state.EndPoint;
			System.Net.Sockets.UdpClient socket = state.Socket;

			// Ensure that async receive is discarded after close/dispose.
			if (!isDisposed && (socket != null) && (this.state == SocketState.Opened))
			{
				byte[] buffer;
				try
				{
					buffer = socket.EndReceive(ar, ref endPoint);
				}
				catch (System.Net.Sockets.SocketException ex)
				{
					buffer = null;
					SocketError();
					OnIOError(new IOErrorEventArgs(IOErrorSeverity.Severe, ex.Message));
				}

				if (buffer != null)
				{
					lock (this.receiveQueue)
					{
						foreach (byte b in buffer)
							this.receiveQueue.Enqueue(b);
					}

					OnDataReceived(new EventArgs());

					// Continue receiving data.
					BeginReceive();
				}
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
			UnusedEvent.PreventCompilerWarning(IOControlChanged);
			throw (new NotImplementedException("Event 'IOControlChanged' is not in use for UDP sockets"));
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
			UnusedEvent.PreventCompilerWarning(IORequest);
			throw (new NotImplementedException("Event 'IORequest' is not in use for UDP sockets"));
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
		/// <remarks>
		/// Named according to .NET <see cref="System.Net.IPEndPoint"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "EndPoint", Justification = "Naming according to System.Net.EndPoint.")]
		public virtual string ToShortEndPointString()
		{
			return ("Receive:" + this.localPort + " / " + this.remoteIPAddress + ":" + this.remotePort);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
