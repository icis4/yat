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
// MKY Development Version 1.0.8
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;

using MKY.Contracts;
using MKY.Event;

#endregion

namespace MKY.IO.Serial.Socket
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

		#region Static Fields
		//==========================================================================================
		// Static Fields
		//==========================================================================================

		private static int staticInstanceCounter;

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const int DataSentQueueInitialCapacity = 4096;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private int instanceId;
		private bool isDisposed;

		private System.Net.IPAddress localIPAddress;
		private int localPort;

		private SocketState state = SocketState.Reset;
		private object stateSyncObj = new object();

		private ALAZ.SystemEx.NetEx.SocketsEx.SocketServer socket;
		private List<ALAZ.SystemEx.NetEx.SocketsEx.ISocketConnection> socketConnections = new List<ALAZ.SystemEx.NetEx.SocketsEx.ISocketConnection>();

		/// <summary>
		/// Async event handling. The capacity is set large enough to reduce the number of resizing
		/// operations while adding elements.
		/// </summary>
		private Queue<byte> dataSentQueue = new Queue<byte>(DataSentQueueInitialCapacity);

		private bool dataSentThreadSyncFlag;
		private AutoResetEvent dataSentThreadEvent;
		private Thread dataSentThread;

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
		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true)]
		public event EventHandler<DataReceivedEventArgs> DataReceived;

		/// <summary></summary>
		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true)]
		public event EventHandler<DataSentEventArgs> DataSent;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public TcpServer(System.Net.IPAddress localIPAddress, int localPort)
		{
			this.instanceId = staticInstanceCounter++;

			this.localIPAddress = localIPAddress;
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
				// Finalize managed resources.

				if (disposing)
				{
					// In the 'normal' case, the items have already been disposed of, e.g. in Stop().
					DisposeSocketAndSocketConnectionsAndThreads();
				}

				this.isDisposed = true;

				Debug.WriteLine(GetType() + "     (" + this.instanceId + ")(" + ToShortEndPointString() + "                  ): Disposed.");
			}
		}

		/// <summary></summary>
		~TcpServer()
		{
			Dispose(false);
		}

		/// <summary></summary>
		public bool IsDisposed
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
		public virtual System.Net.IPAddress LocalIPAddress
		{
			get
			{
				AssertNotDisposed();
				return (this.localIPAddress);
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
					case SocketState.Reset:
					case SocketState.Stopping:
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
			get { return (!IsStopped); }
		}

		/// <summary></summary>
		public virtual bool IsConnected
		{
			get
			{
				AssertNotDisposed();
				switch (this.state)
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
		public virtual int ConnectedClientCount
		{
			get
			{
				AssertNotDisposed();
				return (this.socketConnections.Count);
			}
		}

		/// <summary></summary>
		public virtual bool IsOpen
		{
			get { return (IsConnected); }
		}

		/// <summary></summary>
		public virtual bool IsReadyToSend
		{
			get { return (IsConnected); }
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
			// AssertNotDisposed() is called by 'IsStarted' below.

			if (!IsStarted)
			{
				StartSocket();
				return (true);
			}
			else
			{
				Debug.WriteLine(GetType() + "     (" + this.instanceId + ")(" + ToShortEndPointString() + "                  ): Start() requested but state is " + this.state + ".");
				return (false);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "Stop is a common term to start/stop something.")]
		public virtual void Stop()
		{
			// AssertNotDisposed() is called by 'IsStarted' below.

			if (IsStarted)
				StopSocket();
			else
				Debug.WriteLine(GetType() + "     (" + this.instanceId + ")(" + ToShortEndPointString() + "                  ): Stop() requested but state is " + this.state + ".");
		}

		/// <summary></summary>
		public virtual void Send(byte[] data)
		{
			// AssertNotDisposed() is called by 'IsStarted' below.

			if (IsStarted)
			{
				foreach (ALAZ.SystemEx.NetEx.SocketsEx.ISocketConnection connection in this.socketConnections)
					connection.BeginSend(data);
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
			Debug.WriteLine(GetType() + "     (" + this.instanceId + ")(" + ToShortEndPointString() + "                  ): State has changed from " + oldState + " to " + this.state + ".");
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
			SetStateSynchronizedAndNotify(SocketState.Listening);

			this.socket = new ALAZ.SystemEx.NetEx.SocketsEx.SocketServer
				(
				ALAZ.SystemEx.NetEx.SocketsEx.CallbackThreadType.ctWorkerThread,
				(ALAZ.SystemEx.NetEx.SocketsEx.ISocketService)this,
				ALAZ.SystemEx.NetEx.SocketsEx.DelimiterType.dtNone,
				null,
				SocketDefaults.SocketBufferSize,
				SocketDefaults.MessageBufferSize,
				Timeout.Infinite,
				Timeout.Infinite
				);

			this.socket.AddListener("YAT TCP Server Listener", new System.Net.IPEndPoint(System.Net.IPAddress.Any, this.localPort));
			this.socket.Start(); // The ALAZ socket will be started asynchronously.
		}

		private void StopSocket()
		{
			SetStateSynchronizedAndNotify(SocketState.Stopping);

			// \remind:
			// The ALAZ sockets by default stop synchronously. However, due to some other issues
			//   the ALAZ sockets had to be modified. The modified version stops asynchronously.
			this.socket.Stop();

			// \remind:
			// See above
			//SetStateAndNotify(SocketState.Reset);
		}

		private void DisposeSocketAndSocketConnectionsAndThreads()
		{
			if (this.socket != null)
			{
				this.socket.Stop();
				this.socket.Dispose(); // Attention: ALAZ sockets don't properly stop on Dispose().
				this.socket = null;
				this.socketConnections.Clear();
			}
		}

		#endregion

		#region Socket Threads
		//==========================================================================================
		// Socket Threads
		//==========================================================================================

		private void StartDataSentThread()
		{
			// Ensure that thread has stopped after the last stop request.
			while (this.dataSentThread != null)
				Thread.Sleep(1);

			this.dataSentThreadSyncFlag = true;
			this.dataSentThreadEvent = new AutoResetEvent(false);
			this.dataSentThread = new Thread(new ThreadStart(DataSentThread));
			this.dataSentThread.Start();
		}

		/// <remarks>
		/// Just signal the thread, it will stop soon. Do not wait for it (i.e. Join()),
		/// this method could have been called from a thread that also has to handle the receive
		/// events (e.g. the application main thread). Waiting here would lead to deadlocks.
		/// </remarks>
		private void RequestStopDataSentThread()
		{
			this.dataSentThreadSyncFlag = false;
			this.dataSentThreadEvent.Set();
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
			lock (this.socketConnections)
				this.socketConnections.Add(e.Connection);

			SetStateSynchronizedAndNotify(SocketState.Accepted);

			// Immediately begin receiving data.
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
			// This receive callback is always asychronous, thus the event handler can
			// be called directly. It is also ensured that the event handler is called
			// sequential because the 'BeginReceive()' method is only called after
			// the eevent handler has returned.
			OnDataReceived(new DataReceivedEventArgs(e.Buffer));

			// Continue receiving data.
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
			lock (this.dataSentQueue)
			{
				foreach (byte b in e.Buffer)
					this.dataSentQueue.Enqueue(b);
			}

			// Signal receive thread:
			this.dataSentThreadEvent.Set();
		}

		/// <summary>
		/// Asynchronously manage outgoing send requests to ensure that send events are not
		/// invoked on the same thread that triggered the send operation.
		/// Also, the mechanism implemented below reduces the amount of events that are propagated
		/// to the main application. Small chunks of sent data would generate many events in
		/// <see cref="Send(byte[])"/>. However, since <see cref="OnSent"/> synchronously
		/// invokes the event, it will take some time until the send queue is checked again.
		/// During this time, no more new events are invoked, instead, outgoing data is buffered.
		/// </summary>
		/// <remarks>
		/// Will be signaled by <see cref="Send(byte[])"/> method above.
		/// </remarks>
		private void DataSentThread()
		{
			Debug.WriteLine(GetType() + " '" + ToShortEndPointString() + "': SendThread() has started.");

			while (this.dataSentThreadSyncFlag)
			{
				this.dataSentThreadEvent.WaitOne();

				byte[] data;
				lock (this.dataSentQueue)
				{
					if (this.dataSentQueue.Count <= 0)
						continue;

					data = this.dataSentQueue.ToArray();
					this.dataSentQueue.Clear();
				}

				OnDataSent(new DataSentEventArgs(data));
			}

			this.dataSentThread = null;

			// Do not Close() and de-reference the corresponding event as it may be Set() again
			// right now by another thread, e.g. during closing.

			Debug.WriteLine(GetType() + " '" + ToShortEndPointString() + "': SendThread() has terminated.");
		}

		/// <summary>
		/// Fired when disconnected.
		/// </summary>
		/// <param name="e">
		/// Information about the connection.
		/// </param>
		public virtual void OnDisconnected(ALAZ.SystemEx.NetEx.SocketsEx.ConnectionEventArgs e)
		{
			bool isConnected = false;
			lock (this.socketConnections)
			{
				this.socketConnections.Remove(e.Connection);
				isConnected = (this.socketConnections.Count > 0);
			}

			if (!isConnected)
			{
				SocketState state = GetStateSynchronized();
				switch (state)
				{
					case SocketState.Accepted: SetStateSynchronizedAndNotify(SocketState.Listening); break;
					case SocketState.Stopping: SetStateSynchronizedAndNotify(SocketState.Reset);     break;
					default: break; // No state change in all other cases.
				}
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
			DisposeSocketAndSocketConnectionsAndThreads();

			SetStateSynchronizedAndNotify(SocketState.Error);
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
			UnusedEvent.PreventCompilerWarning(IOControlChanged);
			throw (new NotImplementedException("Event 'IOControlChanged' is not in use for TCP servers"));
		}

		/// <summary></summary>
		protected virtual void OnIOError(IOErrorEventArgs e)
		{
			EventHelper.FireSync<IOErrorEventArgs>(IOError, this, e);
		}

		/// <summary></summary>
		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true)]
		protected virtual void OnDataReceived(DataReceivedEventArgs e)
		{
			EventHelper.FireSync<DataReceivedEventArgs>(DataReceived, this, e);
		}

		/// <summary></summary>
		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true)]
		protected virtual void OnDataSent(DataSentEventArgs e)
		{
			EventHelper.FireSync<DataSentEventArgs>(DataSent, this, e);
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary></summary>
		public override string ToString()
		{
			return (ToShortEndPointString());
		}

		/// <summary></summary>
		/// <remarks>
		/// Named according to .NET <see cref="System.Net.IPEndPoint"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "EndPoint", Justification = "Naming according to System.Net.EndPoint.")]
		public virtual string ToShortEndPointString()
		{
			return ("Server:" + this.localPort);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
