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
// Copyright © 2007-2015 Matthias Kläy.
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
using MKY.Diagnostics;

#endregion

namespace MKY.IO.Serial.Socket
{
	/// <remarks>
	/// In case of YAT with the original ALAZ implementation, a TCP/IP client created a deadlock on
	/// shutdown. The situation:
	/// 
	/// 1. <see cref="Stop()"/> is called from a GUI/main thread
	/// 2. 'ALAZ.SystemEx.NetEx.SocketsEx.BaseSocketConnectionHost.StopConnections()' blocks
	/// 3. The 'OnDisconnected' event is fired
	/// 4. FireOnDisconnected() is blocked when trying to synchronize Invoke() onto the GUI/main
	///    thread and a deadlock happens
	/// 
	/// Further down the calling chain, 'BaseSocketConnection.Active.get()' was also blocking.
	/// 
	/// These two issues could be solved by modifying 'BaseSocketConnection.Active.get()' to be
	/// non-blocking, by calling Stop() asynchronously and by suppressing the 'OnDisconnected' and
	/// 'OnException' events while stopping.
	/// 
	/// These two issues were also reported back to Andre Luis Azevedo. But unfortunately he doesn't
	/// reply and ALAZ seems to have come to a dead end. An alternative to ALAZ might need to be
	/// found in the future.
	/// 
	/// Note that the very same issue existed in <see cref="TcpClient"/>.
	/// 
	/// Also note that a very similar issue existed when stopping two <see cref="TcpAutoSocket"/>
	/// that were interconnected with each other. See remarks of this class for details.
	/// </remarks>
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

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const int DataSentQueueInitialCapacity = 4096;

		private const int ThreadWaitInterval = 1;
		private const int ThreadWaitTimeout = 3000;

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

		private System.Net.IPAddress localIPAddress;
		private int localPort;

		private SocketState state = SocketState.Reset;
		private ReaderWriterLockSlim stateLock = new ReaderWriterLockSlim();

		/// <remarks>
		/// Required to deal with the issues described in the remarks in the header of this class.
		/// </remarks>
		private bool eventHandlingIsSuppressedWhileStopping;
		private ReaderWriterLockSlim eventHandlingIsSuppressedWhileStoppingLock = new ReaderWriterLockSlim();

		private ALAZ.SystemEx.NetEx.SocketsEx.SocketServer socket;
		private List<ALAZ.SystemEx.NetEx.SocketsEx.ISocketConnection> socketConnections = new List<ALAZ.SystemEx.NetEx.SocketsEx.ISocketConnection>();

		/// <remarks>
		/// Async event handling. The capacity is set large enough to reduce the number of resizing
		/// operations while adding elements.
		/// </remarks>
		private Queue<byte> dataSentQueue = new Queue<byte>(DataSentQueueInitialCapacity);

		private bool dataSentThreadRunFlag;
		private AutoResetEvent dataSentThreadEvent;
		private Thread dataSentThread;
		private object dataSentThreadSyncObj = new object();

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
				// Dispose of managed resources if requested:
				if (disposing)
				{
					// In the 'normal' case, the items have already been disposed of, e.g. in Stop().
					SuppressEventsAndThenStopAndDisposeSocket();

					// Do not yet dispose of socket, event lock, thread event and state lock because
					// that may result in null ref exceptions during closing, due to the fact that
					// ALAZ closes/disconnects asynchronously. Further investigation is required
					// in order to further improve the behaviour on Stop()/Dispose().
				}

				// Set state to disposed:
				this.isDisposed = true;

				WriteDebugMessageLine("Disposed.");
			}
		}

		/// <summary></summary>
		~TcpServer()
		{
			Dispose(false);

			System.Diagnostics.Debug.WriteLine("The finalizer of '" + GetType().FullName + "' should have never been called! Ensure to call Dispose()!");
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
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.localIPAddress);
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
				// Do not call AssertNotDisposed() in a simple get-property.

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

				int count;
				lock (this.socketConnections)
					count = this.socketConnections.Count;

				return (count);
			}
		}

		/// <summary></summary>
		public virtual bool IsOpen
		{
			get { return (IsConnected); }
		}

		/// <summary></summary>
		public virtual bool IsTransmissive
		{
			get { return (IsConnected); }
		}

		private bool EventHandlingIsSuppressedWhileStoppingSynchronized
		{
			get
			{
				bool isSuppressed;

				this.eventHandlingIsSuppressedWhileStoppingLock.EnterReadLock();
				isSuppressed = this.eventHandlingIsSuppressedWhileStopping;
				this.eventHandlingIsSuppressedWhileStoppingLock.ExitReadLock();

				return (isSuppressed);
			}

			set
			{
				this.eventHandlingIsSuppressedWhileStoppingLock.EnterWriteLock();
				this.eventHandlingIsSuppressedWhileStopping = value;
				this.eventHandlingIsSuppressedWhileStoppingLock.ExitWriteLock();
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
			// AssertNotDisposed() is called by 'IsStarted' below.

			if (IsStopped)
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

				// Dispose of ALAZ socket in any case. A new socket will be created on next Start().
				StopAndDisposeSocketWithoutSuppressingEvents();
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
				lock (this.socketConnections)
				{
					foreach (ALAZ.SystemEx.NetEx.SocketsEx.ISocketConnection connection in this.socketConnections)
						connection.BeginSend(data);
				}
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
			OnIOChanged(EventArgs.Empty);
		}

		#endregion

		#region Socket Methods
		//==========================================================================================
		// Socket Methods
		//==========================================================================================

		private void StartSocket()
		{
			EventHandlingIsSuppressedWhileStoppingSynchronized = false;

			SetStateSynchronizedAndNotify(SocketState.Listening);

			StartDataSentThread();

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

			this.socket.AddListener("MKY TCP/IP Server Listener", new System.Net.IPEndPoint(System.Net.IPAddress.Any, this.localPort));
			this.socket.Start(); // The ALAZ socket will be started asynchronously.
		}

		/// <remarks>
		/// Dispose of ALAZ socket in any case. A new socket will be created on next Start().
		/// 
		/// \attention:
		/// The Stop() method of the ALAZ socket must not be called on the GUI/main thread.
		/// See remarks of the header of this class for details.
		/// </remarks>
		private void StopAndDisposeSocketWithoutSuppressingEvents()
		{
			SetStateSynchronizedAndNotify(SocketState.Stopping);

			VoidDelegateVoid asyncInvoker = new VoidDelegateVoid(StopAndDisposeSocketAndConnectionsAndThreadWithoutFiringEvents);
			asyncInvoker.BeginInvoke(null, null);
		}

		/// <remarks>
		/// Dispose of ALAZ socket in any case. A new socket will be created on next Start().
		/// 
		/// \attention:
		/// The Stop() method of the ALAZ socket must not be called on the GUI/main thread.
		/// See remarks of the header of this class for details.
		/// </remarks>
		private void SuppressEventsAndThenStopAndDisposeSocket()
		{
			EventHandlingIsSuppressedWhileStoppingSynchronized = true;

			VoidDelegateVoid asyncInvoker = new VoidDelegateVoid(StopAndDisposeSocketAndConnectionsAndThreadWithoutFiringEvents);
			asyncInvoker.BeginInvoke(null, null);
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		private void StopAndDisposeSocketAndConnectionsAndThreadWithoutFiringEvents()
		{
			if (this.socket != null)
			{
				try
				{
					this.socket.Stop(); // Attention: ALAZ sockets don't properly stop on Dispose().
				}
				catch (Exception ex)
				{
					DebugEx.WriteException(GetType(), ex);
				}
			}

			if (this.socket != null) // If everything works as intended, the socket will already
			{                        // have gotten disposed while handling the 'Disconnected'
				try                  // event that is raised after the call to Stop() above.
				{
					this.socket.Dispose(); // Attention: ALAZ sockets don't properly stop on Dispose().
				}
				catch (Exception ex)
				{
					DebugEx.WriteException(GetType(), ex);
				}

				this.socket = null;
			}

			lock (this.socketConnections)
				this.socketConnections.Clear();

			// Finally, stop the thread. Must be done AFTER the socket got disposed to ensure that
			// the last socket callbacks can still be properly processed.
			StopDataSentThread();
		}

		#endregion

		#region Socket Threads
		//==========================================================================================
		// Socket Threads
		//==========================================================================================

		private void StartDataSentThread()
		{
			lock (this.dataSentThreadSyncObj)
			{
				if (this.dataSentThread == null)
				{
					// Start thread:
					this.dataSentThreadRunFlag = true;
					this.dataSentThreadEvent = new AutoResetEvent(false);
					this.dataSentThread = new Thread(new ThreadStart(DataSentThread));
					this.dataSentThread.Name = ToShortEndPointString() + " DataSent Thread";
					this.dataSentThread.Start();
				}
			}
		}

		private void StopDataSentThread()
		{
			lock (this.dataSentThreadSyncObj)
			{
				if (this.dataSentThread != null)
				{
					this.dataSentThreadRunFlag = false;

					// Ensure that thread has stopped after the stop request:
					try
					{
						int timeoutCounter = 0;
						while (!this.dataSentThread.Join(ThreadWaitInterval))
						{
							this.dataSentThreadEvent.Set();
							if (++timeoutCounter >= (ThreadWaitTimeout / ThreadWaitInterval))
								throw (new TimeoutException("Data sent thread hasn't properly stopped"));
						}
					}
					catch (ThreadStateException)
					{
						// Ignore thread state exceptions such as "Thread has not been started"
						// since the thread needs to be shut down anyway.
					}

					this.dataSentThreadEvent.Close();
					this.dataSentThread = null;
				}
			}
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
		/// Fired when data has been sent.
		/// </summary>
		/// <param name="e">
		/// Information about the data that has been sent.
		/// <remarks>
		/// Note that the original ALAZ implementation always keeps 'e.Buffer' at
		/// <c>null</c> whereas the modified version contains a filled data buffer.
		/// </remarks>
		/// </param>
		public virtual void OnSent(ALAZ.SystemEx.NetEx.SocketsEx.MessageEventArgs e)
		{
			// No clue why the 'Sent' event fires once before actual data is being sent...
			if (e.Buffer != null)
			{
				lock (this.dataSentQueue)
				{
					foreach (byte b in e.Buffer)
						this.dataSentQueue.Enqueue(b);
				}

				// Signal receive thread:
				this.dataSentThreadEvent.Set();
			}
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
			WriteDebugMessageLine("SendThread() has started.");

			// Outer loop, requires another signal.
			while (this.dataSentThreadRunFlag && !IsDisposed)
			{
				try
				{
					// WaitOne() might wait forever in case the underlying I/O provider crashes,
					// or if the overlying client isn't able or forgets to call Stop() or Dispose(),
					// therefore, only wait for a certain period and then poll the run flag again.
					if (!this.dataSentThreadEvent.WaitOne(staticRandom.Next(50, 200)))
						continue;
				}
				catch (AbandonedMutexException ex)
				{
					// The mutex should never be abandoned, but in case it nevertheless happens,
					// at least output a debug message and gracefully exit the thread.
					DebugEx.WriteException(GetType(), ex, "An 'AbandonedMutexException' occurred in DataSentThread()");
					break;
				}

				// Inner loop, runs as long as there is data to be handled. Must be done to
				// ensure that events are fired even for data that was enqueued above while the
				// 'OnDataReceived' event was being handled.
				// 
				// Ensure not to forward any events during closing anymore.
				while (IsOpen && this.dataSentThreadRunFlag && !IsDisposed)
				{
					byte[] data;
					lock (this.dataSentQueue)
					{
						if (this.dataSentQueue.Count <= 0)
							break; // Let other threads do their job and wait until signaled again.

						data = this.dataSentQueue.ToArray();
						this.dataSentQueue.Clear();
					}

					OnDataSent(new DataSentEventArgs(data));

					// Wait for the minimal time possible to allow other threads to execute and
					// to prevent that 'DataSent' events are fired consecutively.
					Thread.Sleep(TimeSpan.Zero);
				}
			}

			WriteDebugMessageLine("SendThread() has terminated.");
		}

		/// <summary>
		/// Fired when disconnected.
		/// </summary>
		/// <param name="e">
		/// Information about the connection.
		/// </param>
		public virtual void OnDisconnected(ALAZ.SystemEx.NetEx.SocketsEx.ConnectionEventArgs e)
		{
			if (!EventHandlingIsSuppressedWhileStoppingSynchronized)
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
		}

		/// <summary>
		/// Fired when exception occurs.
		/// </summary>
		/// <param name="e">
		/// Information about the exception and connection.
		/// </param>
		public virtual void OnException(ALAZ.SystemEx.NetEx.SocketsEx.ExceptionEventArgs e)
		{
			if (!EventHandlingIsSuppressedWhileStoppingSynchronized)
			{
				// Dispose of ALAZ socket in any case. A new socket will be created on next Start().
				SuppressEventsAndThenStopAndDisposeSocket();

				SetStateSynchronizedAndNotify(SocketState.Error);

				StringBuilder sb = new StringBuilder();
				sb.AppendLine("The socket of this TCP/IP Server has fired an exception!");
				sb.AppendLine();
				sb.AppendLine("Exception type:");
				sb.AppendLine(e.Exception.GetType().Name);
				sb.AppendLine();
				sb.AppendLine("Exception error message:");
				sb.AppendLine(e.Exception.Message);
				string message = sb.ToString();
				WriteDebugMessageLine(message);

				OnIOError(new IOErrorEventArgs(ErrorSeverity.Severe, message));
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
			throw (new NotImplementedException("Event 'IOControlChanged' is not in use for TCP/IP Servers"));
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
			return ("Server:" + this.localPort);
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
			Debug.WriteLine(GetType() + "     (" + this.instanceId.ToString("D2", System.Globalization.CultureInfo.InvariantCulture) + ")(" + ToShortEndPointString() + "                  ): " + message);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
