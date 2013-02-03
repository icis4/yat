//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.10
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2013 Matthias Kläy.
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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

using MKY.Contracts;
using MKY.Diagnostics;

#endregion

namespace MKY.IO.Serial.Socket
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

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const int SendQueueInitialCapacity = 4096;

		#endregion

		#region Static Fields
		//==========================================================================================
		// Static Fields
		//==========================================================================================

		private static int staticInstanceCounter;
		private static Random staticRandom = new Random(RandomEx.NextPseudoRandomSeed());

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
		private ReaderWriterLockSlim stateLock = new ReaderWriterLockSlim();

		private System.Net.IPEndPoint endPoint;
		private System.Net.Sockets.UdpClient socket;

		/// <remarks>
		/// Async sending. The capacity is set large enough to reduce the number of resizing
		/// operations while adding elements.
		/// </remarks>
		private Queue<byte> sendQueue = new Queue<byte>(SendQueueInitialCapacity);

		private bool sendThreadRunFlag;
		private AutoResetEvent sendThreadEvent;
		private Thread sendThread;

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
				// Dispose of managed resources if requested:
				if (disposing)
				{
					// In the 'normal' case, the items have already been disposed of, e.g. in Stop().
					DisposeSocketAndThreads();

					// Do not yet dispose of thread event and state lock because that may result
					// in null ref exceptions during closing. Further investigation is required
					// in order to further improve the behaviour on Stop()/Dispose().
				}

				// Set state to disposed:
				this.isDisposed = true;

				WriteDebugMessageLine("Disposed.");
			}
		}

		/// <summary></summary>
		~UdpSocket()
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
		public virtual System.Net.IPAddress RemoteIPAddress
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.remoteIPAddress);
			}
		}

		/// <summary></summary>
		public virtual int RemotePort
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.remotePort);
			}
		}

		/// <summary></summary>
		public virtual int LocalPort
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.localPort);
			}
		}

		/// <summary></summary>
		public virtual bool IsStopped
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

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
				// Do not call AssertNotDisposed() in a simple get-property.

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
				// Do not call AssertNotDisposed() in a simple get-property.

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
				WriteDebugMessageLine("Starting...");
				StartSocket();
				return (true);
			}
			else
			{
				WriteDebugMessageLine("Start() requested but state is " + this.state + ".");
				return (false);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "Stop is a common term to start/stop something.")]
		public virtual void Stop()
		{
			// AssertNotDisposed() is called by 'IsStarted' below.

			if (IsStarted)
			{
				WriteDebugMessageLine("Stopping...");
				StopSocket();
			}
			else
			{
				WriteDebugMessageLine("Stop() requested but state is " + this.state + ".");
			}
		}

		/// <summary></summary>
		public virtual void Send(byte[] data)
		{
			// AssertNotDisposed() is called by 'IsStarted' below.

			if (IsStarted)
			{
				lock (this.sendQueue)
				{
					foreach (byte b in data)
						this.sendQueue.Enqueue(b);
				}

				// Signal send thread:
				this.sendThreadEvent.Set();
			}
		}

		/// <summary>
		/// Asynchronously manage outgoing send requests to ensure that send events are not
		/// invoked on the same thread that triggered the send operation.
		/// Also, the mechanism implemented below reduces the amount of events that are propagated
		/// to the main application. Small chunks of sent data would generate many events in
		/// <see cref="Send(byte[])"/>. However, since <see cref="OnDataSent"/> synchronously
		/// invokes the event, it will take some time until the send queue is checked again.
		/// During this time, no more new events are invoked, instead, outgoing data is buffered.
		/// </summary>
		/// <remarks>
		/// Will be signaled by <see cref="Send(byte[])"/> method above.
		/// </remarks>
		private void SendThread()
		{
			WriteDebugMessageLine("SendThread() has started.");

			// Outer loop, requires another signal.
			while (this.sendThreadRunFlag && !IsDisposed)
			{
				try
				{
					// WaitOne() might wait forever in case the underlying I/O provider crashes,
					// or if the overlying client isn't able or forgets to call Stop() or Dispose(),
					// therefore, only wait for a certain period and then poll the run flag again.
					if (!this.sendThreadEvent.WaitOne(staticRandom.Next(50, 200)))
						continue;
				}
				catch (AbandonedMutexException ex)
				{
					// The mutex should never be abandoned, but in case it nevertheless happens,
					// at least output a debug message and gracefully exit the thread.
					DebugEx.WriteException(GetType(), ex, "An 'AbandonedMutexException' occurred in SendThread()");
					break;
				}

				// Inner loop, runs as long as there is data in the send queue.
				// Ensure not to forward any events during closing anymore.
				while (this.sendThreadRunFlag && IsReadyToSend && !IsDisposed)
				{
					byte[] data;
					lock (this.sendQueue)
					{
						if (this.sendQueue.Count <= 0)
							break; // Let other threads do their job and wait until signaled again.

						data = this.sendQueue.ToArray();
						this.sendQueue.Clear();
					}

					this.socket.Send(data, data.Length);
					OnDataSent(new DataSentEventArgs(new ReadOnlyCollection<byte>(data)));

					// Wait for the minimal time possible to allow other threads to execute and
					// to prevent that 'DataSent' events are fired consecutively.
					Thread.Sleep(TimeSpan.Zero);
				}
			}

			this.sendThread = null;

			// Do not Close() and de-reference the corresponding event as it may be Set() again
			// right now by another thread, e.g. during closing.

			WriteDebugMessageLine("SendThread() has terminated.");
		}

		#endregion

		#region State Methods
		//==========================================================================================
		// State Methods
		//==========================================================================================

		private SocketState GetStateSynchronized()
		{
			SocketState state;

			this.stateLock.EnterReadLock();
			state = this.state;
			this.stateLock.ExitReadLock();

			return (state);
		}

		private void SetStateSynchronizedAndNotify(SocketState state)
		{
#if (DEBUG)
			SocketState oldState = this.state;
#endif
			this.stateLock.EnterWriteLock();
			this.state = state;
			this.stateLock.ExitWriteLock();
#if (DEBUG)
			if (this.state != oldState)
				WriteDebugMessageLine("State has changed from " + oldState + " to " + this.state + ".");
			else
				WriteDebugMessageLine("State is still " + oldState + ".");
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
			CreateAndStartSendThread();

			SetStateSynchronizedAndNotify(SocketState.Opening);

			this.endPoint = new System.Net.IPEndPoint(this.remoteIPAddress, this.remotePort);
			this.socket = new System.Net.Sockets.UdpClient(this.localPort);
			this.socket.Connect(this.endPoint);

			SetStateSynchronizedAndNotify(SocketState.Opened);

			// Immediately begin receiving data.
			BeginReceive();
		}

		private void CreateAndStartSendThread()
		{
			// Ensure that thread has stopped after the last stop request:
			int timeoutCounter = 0;
			while (this.sendThread != null)
			{
				Thread.Sleep(1);

				if (++timeoutCounter >= 3000)
					throw (new TimeoutException("Thread hasn't properly stopped"));
			}

			// Do not yet enforce that thread events have been disposed because that may result in
			// deadlock. Further investigation is required in order to further improve the behaviour
			// on Stop()/Dispose().

			// Start thread:
			this.sendThreadRunFlag = true;
			this.sendThreadEvent = new AutoResetEvent(false);
			this.sendThread = new Thread(new ThreadStart(SendThread));
			this.sendThread.Start();
		}

		private void StopSocket()
		{
			SetStateSynchronizedAndNotify(SocketState.Closing);
			DisposeSocketAndThreads();
			SetStateSynchronizedAndNotify(SocketState.Closed);
		}

		private void DisposeSocketAndThreads()
		{
			this.socket.Close();
			this.socket = null;

			// Finally, stop the thread. Must be done AFTER the socket got closed to ensure that
			// the last socket callbacks can still be properly processed.
			this.sendThreadRunFlag = false;

			// Ensure that thread has stopped after the stop request:
			int timeoutCounter = 0;
			while (this.sendThread != null)
			{
				this.sendThreadEvent.Set();

				Thread.Sleep(1);

				if (++timeoutCounter >= 3000)
					throw (new TimeoutException("Thread hasn't properly stopped"));
			}
		}

		private void SocketError()
		{
			DisposeSocketAndThreads();
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
				byte[] data;
				try
				{
					data = socket.EndReceive(ar, ref endPoint);
				}
				catch (System.Net.Sockets.SocketException ex)
				{
					data = null;
					SocketError();
					OnIOError(new IOErrorEventArgs(ErrorSeverity.Severe, ex.Message));
				}

				if (data != null)
				{
					// This receive callback is always asychronous, thus the event handler can
					// be called directly. It is also ensured that the event handler is called
					// sequential because the 'BeginReceive()' method is only called after
					// the eevent handler has returned.
					OnDataReceived(new DataReceivedEventArgs(data));

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

		#region Object Members > Extensions
		//------------------------------------------------------------------------------------------
		// Object Members > Extensions
		//------------------------------------------------------------------------------------------

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

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		/// <summary></summary>
		[Conditional("DEBUG")]
		private void WriteDebugMessageLine(string message)
		{
			Debug.WriteLine(GetType() + "     (" + this.instanceId.ToString("D2", System.Globalization.CultureInfo.InvariantCulture) + ")(" + ToShortEndPointString() + "): " + message);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
