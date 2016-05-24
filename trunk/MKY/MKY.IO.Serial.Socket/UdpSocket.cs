﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.14
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Configuration
//==================================================================================================
// Configuration
//==================================================================================================

#if (DEBUG)

	// Enable debugging of thread state:
////#define DEBUG_THREAD_STATE

#endif // DEBUG

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;

using MKY.Contracts;
using MKY.Diagnostics;
using MKY.Net;

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
			public readonly System.Net.IPEndPoint LocalFilterEndPoint;
			public readonly System.Net.Sockets.UdpClient Socket;

			public AsyncReceiveState(System.Net.IPEndPoint localFilterEndPoint, System.Net.Sockets.UdpClient socket)
			{
				this.LocalFilterEndPoint = localFilterEndPoint;
				this.Socket = socket;
			}
		}

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const int SendQueueFixedCapacity = 4096;

		private const int ThreadWaitTimeout = 200;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isDisposed;

		private int instanceId;
		private UdpSocketType socketType;

		private IPHostEx remoteHost;
		private int remotePort;
		private int localPort;
		private IPFilterEx localFilter;

		private UdpServerSendMode serverSendMode;

		private SocketState state = SocketState.Closed;
		private ReaderWriterLockSlim stateLock = new ReaderWriterLockSlim();

		private System.Net.Sockets.UdpClient socket;
		private object socketSyncObj = new object();
		private object dataEventSyncObj = new object();

		/// <remarks>
		/// Async sending. The capacity is set large enough to reduce the number of resizing
		/// operations while adding elements.
		/// </remarks>
		private Queue<byte> sendQueue = new Queue<byte>(SendQueueFixedCapacity);

		private bool sendThreadRunFlag;
		private AutoResetEvent sendThreadEvent;
		private Thread sendThread;
		private object sendThreadSyncObj = new object();

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

		/// <summary>Creates a UDP/IP socket of type <see cref="UdpSocketType.Client"/>.</summary>
		/// <remarks>The local IP address filter is set to the remote IP address.</remarks>
		public UdpSocket(System.Net.IPAddress remoteIPAddress, int remotePort)
			: this(SocketBase.NextInstanceId, remoteIPAddress, remotePort)
		{
		}

		/// <summary>Creates a UDP/IP socket of type <see cref="UdpSocketType.Client"/>.</summary>
		/// <remarks>The local IP address filter is set to the remote IP address.</remarks>
		public UdpSocket(int instanceId, System.Net.IPAddress remoteIPAddress, int remotePort)
			: this(instanceId, UdpSocketType.Client, remoteIPAddress, remotePort, 0, remoteIPAddress)
		{
		}

		/// <summary>Creates a UDP/IP socket of type <see cref="UdpSocketType.Server"/>.</summary>
		/// <remarks>The local IP address filter is defaulted to <see cref="System.Net.IPAddress.Any"/>.</remarks>
		public UdpSocket(int localPort, UdpServerSendMode serverSendMode)
			: this(SocketBase.NextInstanceId, localPort, System.Net.IPAddress.Any, serverSendMode)
		{
		}

		/// <summary>Creates a UDP/IP socket of type <see cref="UdpSocketType.Server"/>.</summary>
		/// <remarks>The local IP address filter is defaulted to <see cref="System.Net.IPAddress.Any"/>.</remarks>
		public UdpSocket(int instanceId, int localPort, UdpServerSendMode serverSendMode)
			: this(instanceId, UdpSocketType.Server, System.Net.IPAddress.None, 0, localPort, System.Net.IPAddress.Any, serverSendMode)
		{
		}

		/// <summary>Creates a UDP/IP socket of type <see cref="UdpSocketType.Server"/>.</summary>
		/// <exception cref="ArgumentNullException"><paramref name="localFilter"/> is is <c>null</c>.</exception>
		public UdpSocket(int localPort, IPFilterEx localFilter, UdpServerSendMode serverSendMode)
			: this(SocketBase.NextInstanceId, localPort, localFilter, serverSendMode)
		{
		}

		/// <summary>Creates a UDP/IP socket of type <see cref="UdpSocketType.Server"/>.</summary>
		/// <exception cref="ArgumentNullException"><paramref name="localFilter"/> is is <c>null</c>.</exception>
		public UdpSocket(int instanceId, int localPort, IPFilterEx localFilter, UdpServerSendMode serverSendMode)
			: this(instanceId, UdpSocketType.Server, System.Net.IPAddress.None, 0, localPort, localFilter, serverSendMode)
		{
		}

		/// <summary>Creates a UDP/IP socket of type <see cref="UdpSocketType.PairSocket"/>.</summary>
		/// <remarks>The local IP address filter is set to the remote IP address.</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="remoteHost"/> is is <c>null</c>.</exception>
		public UdpSocket(IPHostEx remoteHost, int remotePort, int localPort)
			: this(SocketBase.NextInstanceId, remoteHost, remotePort, localPort)
		{
		}

		/// <summary>Creates a UDP/IP socket of type <see cref="UdpSocketType.PairSocket"/>.</summary>
		/// <remarks>The local IP address filter is set to the remote IP address.</remarks>
		public UdpSocket(int instanceId, System.Net.IPAddress remoteIPAddress, int remotePort, int localPort)
			: this(instanceId, UdpSocketType.PairSocket, remoteIPAddress, remotePort, localPort, remoteIPAddress)
		{
		}

		/// <summary>Creates a UDP/IP socket of the given type.</summary>
		/// <remarks>The local IP address filter is defaulted to <see cref="System.Net.IPAddress.Any"/>.</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="remoteHost"/> is is <c>null</c>.</exception>
		public UdpSocket(UdpSocketType socketType, IPHostEx remoteHost, int remotePort, int localPort)
			: this(SocketBase.NextInstanceId, socketType, remoteHost, remotePort, localPort, System.Net.IPAddress.Any, UdpServerSendMode.None)
		{
		}

		/// <summary>Creates a UDP/IP socket of the given type.</summary>
		/// <remarks>The local IP address filter is defaulted to <see cref="System.Net.IPAddress.Any"/>.</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="remoteHost"/> is is <c>null</c>.</exception>
		public UdpSocket(int instanceId, UdpSocketType socketType, IPHostEx remoteHost, int remotePort, int localPort)
			: this(instanceId, socketType, remoteHost, remotePort, localPort, System.Net.IPAddress.Any)
		{
		}

		/// <summary>Creates a UDP/IP socket of the given type.</summary>
		/// <exception cref="ArgumentNullException"><paramref name="remoteHost"/> is is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="localFilter"/> is is <c>null</c>.</exception>
		public UdpSocket(UdpSocketType socketType, IPHostEx remoteHost, int remotePort, int localPort, IPFilterEx localFilter, UdpServerSendMode serverSendMode)
			: this(SocketBase.NextInstanceId, socketType, remoteHost, remotePort, localPort, localFilter, serverSendMode)
		{
		}

		/// <summary>Creates a UDP/IP socket of the given type.</summary>
		/// <exception cref="ArgumentNullException"><paramref name="remoteHost"/> is is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="localFilter"/> is is <c>null</c>.</exception>
		public UdpSocket(int instanceId, UdpSocketType socketType, IPHostEx remoteHost, int remotePort, int localPort, IPFilterEx localFilter)
			: this(instanceId, socketType, remoteHost, remotePort, localPort, localFilter, UdpServerSendMode.None)
		{
		}

		/// <summary>Creates a UDP/IP socket of the given type.</summary>
		/// <exception cref="ArgumentNullException"><paramref name="remoteHost"/> is is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="localFilter"/> is is <c>null</c>.</exception>
		public UdpSocket(int instanceId, UdpSocketType socketType, IPHostEx remoteHost, int remotePort, int localPort, IPFilterEx localFilter, UdpServerSendMode serverSendMode)
		{
			if (remoteHost == null)  throw (new ArgumentNullException("remoteHost"));
			if (localFilter == null) throw (new ArgumentNullException("localFilter"));

			this.instanceId = instanceId;
			this.socketType = socketType;

			this.remoteHost  = remoteHost;
			this.remotePort  = remotePort;
			this.localPort   = localPort;
			this.localFilter = localFilter;

			this.serverSendMode = serverSendMode;
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
				DebugMessage("Disposing...");

				// Dispose of managed resources if requested:
				if (disposing)
				{
					// In the 'normal' case, the items have already been disposed of, e.g. in Stop().
					DisposeSocketAndThread();

					if (this.stateLock != null)
						this.stateLock.Dispose();
				}

				// Set state to disposed:
				this.stateLock = null;
				this.isDisposed = true;

				DebugMessage("...successfully disposed.");
			}
		}

		/// <remarks>
		/// Microsoft.Design rule CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable requests
		/// "Types that declare disposable members should also implement IDisposable. If the type
		///  does not own any unmanaged resources, do not implement a finalizer on it."
		/// 
		/// Well, true for best performance on finalizing. However, it's not easy to find missing
		/// calls to <see cref="Dispose()"/>. In order to detect such missing calls, the finalizer
		/// is kept, opposing rule CA1001, but getting debug messages indicating missing calls.
		/// 
		/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
		/// </remarks>
		~UdpSocket()
		{
			Dispose(false);

			DebugMessage("The finalizer should have never been called! Ensure to call Dispose()!");
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
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed!"));
		}

		#endregion

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public virtual UdpSocketType SocketType
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.socketType);
			}
		}

		/// <summary></summary>
		public virtual IPHostEx RemoteHost
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				lock (this.socketSyncObj)
					return (this.remoteHost);
			}
		}

		/// <summary></summary>
		public virtual bool RemoteIPAddressIsDefined
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				lock (this.socketSyncObj)
					return (!this.remoteHost.Address.Equals(System.Net.IPAddress.None)); // IPAddress does not override the ==/!= operators, thanks Microsoft guys...
			}
		}

		/// <summary></summary>
		public virtual int RemotePort
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				lock (this.socketSyncObj)
					return (this.remotePort);
			}
		}

		/// <summary></summary>
		public virtual System.Net.IPEndPoint RemoteEndPoint
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				lock (this.socketSyncObj)
					return (new System.Net.IPEndPoint(this.remoteHost, this.remotePort));
			}
		}

		/// <summary></summary>
		public virtual int LocalPort
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				lock (this.socketSyncObj)
					return (this.localPort);
			}
		}

		/// <summary></summary>
		public virtual IPFilterEx LocalFilter
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				lock (this.socketSyncObj)
					return (this.localFilter);
			}
		}

		/// <summary></summary>
		public virtual UdpServerSendMode ServerSendMode
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				lock (this.socketSyncObj)
					return (this.serverSendMode);
			}
		}

		/// <summary></summary>
		public virtual bool IsStopped
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				switch (GetStateSynchronized())
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

				switch (GetStateSynchronized())
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

				switch (GetStateSynchronized())
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
		public virtual bool IsTransmissive
		{
			get
			{
				if ((this.socketType == UdpSocketType.Client) ||
					(this.socketType == UdpSocketType.PairSocket)) // Remote endpoint has been defaulted on Create().
				{
					return (IsOpen);
				}
				else // Server => Remote endpoint is the sender of the first or most recently received data.
				{
					return (IsOpen && RemoteIPAddressIsDefined);
				}
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
				if ((this.socketType == UdpSocketType.Client) ||
					(this.socketType == UdpSocketType.PairSocket))
				{
					DebugMessage("Resolving host address...");
					if (!this.remoteHost.TryResolve())
					{
						DebugMessage("...failed");
						return (false);
					}
				}

				if ((this.socketType == UdpSocketType.Server) ||
					(this.socketType == UdpSocketType.PairSocket))
				{
					DebugMessage("Resolving local filter addres...");
					if (!this.localFilter.TryResolve())
					{
						DebugMessage("...failed");
						return (false);
					}
				}

				DebugMessage("Starting...");
				StartSocket();
				return (true);
			}
			else
			{
				DebugMessage("Start() requested but state is " + GetStateSynchronized() + ".");
				return (false);
			}
		}

		private void StartSocket()
		{
			SetStateSynchronizedAndNotify(SocketState.Opening);
			CreateThreadAndSocket();
			SetStateSynchronizedAndNotify(SocketState.Opened);

			// Immediately begin receiving data:
			BeginReceiveIfEnabled();
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "Stop is a common term to start/stop something.")]
		public virtual void Stop()
		{
			// AssertNotDisposed() is called by 'IsStarted' below.

			if (IsStarted)
			{
				DebugMessage("Stopping...");
				StopSocket();
			}
			else
			{
				DebugMessage("Stop() requested but state is " + GetStateSynchronized() + ".");
			}
		}

		private void StopSocket()
		{
			SetStateSynchronizedAndNotify(SocketState.Closing);
			DisposeSocketAndThread();
			SetStateSynchronizedAndNotify(SocketState.Closed);
		}

		/// <summary></summary>
		public virtual bool Send(byte[] data)
		{
			// AssertNotDisposed() is called by 'IsStarted' below.

			if (IsTransmissive)
			{
				foreach (byte b in data)
				{
					// Wait until there is space in the send queue:
					while (this.sendQueue.Count >= SendQueueFixedCapacity) // No lock required, just checking for full.
					{
						if (IsDisposed || !IsTransmissive) // Check 'IsDisposed' first!
							return (false);

						Thread.Sleep(TimeSpan.Zero); // Yield to other threads to allow dequeuing.
					}

					// There is space for at least one byte:
					lock (this.sendQueue) // Lock is required because Queue<T> is not synchronized.
					{
						this.sendQueue.Enqueue(b);
					}
				}

				// Signal send thread:
				SignalSendThreadSafely();

				return (true);
			}
			else
			{
				return (false);
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
		[SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", MessageId = "System.Threading.WaitHandle.#WaitOne(System.Int32)", Justification = "Installer indeed targets .NET 3.5 SP1.")]
		private void SendThread()
		{
			System.Net.IPEndPoint remoteEndPoint;
			lock (this.socketSyncObj)
				remoteEndPoint = new System.Net.IPEndPoint(this.remoteHost, this.remotePort);

			DebugThreadStateMessage("SendThread() has started.");

			// Outer loop, processes data after a signal was received:
			while (!IsDisposed && this.sendThreadRunFlag) // Check 'IsDisposed' first!
			{
				try
				{
					// WaitOne() will wait forever if the underlying I/O provider has crashed, or
					// if the overlying client isn't able or forgets to call Stop() or Dispose().
					// Therefore, only wait for a certain period and then poll the run flag again.
					// The period can be quite long, as an event trigger will immediately resume.
					if (!this.sendThreadEvent.WaitOne(SocketBase.Random.Next(50, 200)))
						continue;
				}
				catch (AbandonedMutexException ex)
				{
					// The mutex should never be abandoned, but in case it nevertheless happens,
					// at least output a debug message and gracefully exit the thread.
					DebugEx.WriteException(GetType(), ex, "An 'AbandonedMutexException' occurred in SendThread()!");
					break;
				}

				// Inner loop, runs as long as there is data in the send queue.
				// Ensure not to send and forward events during closing anymore. Check 'IsDisposed' first!
				while (!IsDisposed && this.sendThreadRunFlag && IsTransmissive && (this.sendQueue.Count > 0))
				{                                                              // No lock required, just checking for empty.
					// Initially, yield to other threads before starting to read the queue, since it is very
					// likely that more data is to be enqueued, thus resulting in larger chunks processed.
					// Subsequently, yield to other threads to allow processing the data.
					Thread.Sleep(TimeSpan.Zero);

					// Synchronize the send/receive events to prevent mix-ups at the event
					// sinks, i.e. the send/receive operations shall be synchronized with
					// signaling of them.
					// But attention, do not simply lock() the 'dataSyncObj'. Instead, just
					// try to get the lock or try again later. The thread = direction that
					// get's the lock first, shall also be the one to signal first:

					if (Monitor.TryEnter(this.dataEventSyncObj))
					{
						try
						{
							byte[] data;
							lock (this.sendQueue) // Lock is required because Queue<T> is not synchronized.
							{
								data = this.sendQueue.ToArray();
								this.sendQueue.Clear();
							}

							if ((this.socketType == UdpSocketType.Client) ||
								(this.socketType == UdpSocketType.PairSocket)) // Remote endpoint has been defaulted on Create().
							{
								this.socket.Send(data, data.Length);
							}
							else // Server => Remote endpoint is the sender of the first or most recently received data.
							{
								lock (this.socketSyncObj)
									remoteEndPoint = new System.Net.IPEndPoint(this.remoteHost, this.remotePort);

								this.socket.Send(data, data.Length, remoteEndPoint);
							}

							OnDataSent(new SocketDataSentEventArgs(data, remoteEndPoint));

							// Note the Thread.Sleep(TimeSpan.Zero) above.
						}
						finally
						{
							Monitor.Exit(this.dataEventSyncObj);
						}
					} // Monitor.TryEnter()

					if (this.socketType == UdpSocketType.Client)
					{
						// Get the currently used local port:

						bool hasChanged = false;

						lock (this.socketSyncObj)
						{
							System.Net.IPEndPoint localEndPoint = (System.Net.IPEndPoint)this.socket.Client.LocalEndPoint;
							if (this.localPort != localEndPoint.Port) {
								this.localPort = localEndPoint.Port;
								hasChanged = true;
							}
						}

						if (hasChanged)
							OnIOChanged(EventArgs.Empty);
					} // Client
				} // Inner loop
			} // Outer loop

			DebugThreadStateMessage("SendThread() has terminated.");
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
			SocketState oldState = GetStateSynchronized();
#endif
			this.stateLock.EnterWriteLock();
			this.state = state;
			this.stateLock.ExitWriteLock();
#if (DEBUG)
			if (this.state != oldState)
				DebugMessage("State has changed from " + oldState + " to " + state + ".");
			else
				DebugMessage("State is already " + oldState + ".");
#endif
			OnIOChanged(EventArgs.Empty);
		}

		#endregion

		#region Socket Methods
		//==========================================================================================
		// Socket Methods
		//==========================================================================================

		private void CreateThreadAndSocket()
		{
			// Create and start thread:
			lock (this.sendThreadSyncObj)
			{
				if (this.sendThread == null)
				{
					this.sendThreadRunFlag = true;
					this.sendThreadEvent = new AutoResetEvent(false);
					this.sendThread = new Thread(new ThreadStart(SendThread));
					this.sendThread.Name = ToShortEndPointString() + " Send Thread";
					this.sendThread.Start();
				}
			}

			// Create socket:
			lock (this.socketSyncObj)
			{
				// Neither local nor remote endpoint must be set in constructor!
				// Otherwise, options cannot be changed below!
				this.socket = new System.Net.Sockets.UdpClient();

				if ((this.socketType == UdpSocketType.Server) ||
					(this.socketType == UdpSocketType.PairSocket)) // Configure and bind the server/listener port:
				{
					this.socket.ExclusiveAddressUse = false;
					this.socket.Client.SetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket, System.Net.Sockets.SocketOptionName.ReuseAddress, true);
					this.socket.Client.Bind(new System.Net.IPEndPoint(this.localFilter, this.localPort));
				}
				else // Client
				{
					// The socket is not bound to a local port.
				}

				if ((this.socketType == UdpSocketType.Client) ||
					(this.socketType == UdpSocketType.PairSocket)) // Connect the client port:
				{
					this.socket.Connect(this.remoteHost.Address, this.remotePort);
				}
				else // Server
				{
					// The remote address will be set to the sender of the first or most recently received data.
				}
			}
		}

		/// <remarks>
		/// Especially useful during potentially dangerous creation and disposal sequence.
		/// </remarks>
		private void SignalSendThreadSafely()
		{
			try
			{
				if (this.sendThreadEvent != null)
					this.sendThreadEvent.Set();
			}
			catch (ObjectDisposedException ex) { DebugEx.WriteException(GetType(), ex, "Unsafe thread signaling caught"); }
			catch (NullReferenceException ex)  { DebugEx.WriteException(GetType(), ex, "Unsafe thread signaling caught"); }

			// Catch 'NullReferenceException' for the unlikely case that the event has just been
			// disposed after the if-check. This way, the event doesn't need to be locked (which
			// is a relatively time-consuming operation). Still keep the if-check for the normal
			// cases.
		}

		private void DisposeSocketAndThread()
		{
			// Close and dispose socket:
			lock (this.socketSyncObj)
			{
				if (this.socket != null)
				{
					this.socket.Close();
					this.socket = null;
				}
			}

			// Stop and dispose thread:
			lock (this.sendThreadSyncObj)
			{
				if (this.sendThread != null)
				{
					DebugThreadStateMessage("SendThread() gets stopped...");

					// Stop the thread. Must be done AFTER the socket got closed to ensure that
					// the last socket callbacks can still be properly processed.
					this.sendThreadRunFlag = false;

					// Ensure that send thread has stopped after the stop request:
					try
					{
						Debug.Assert(this.sendThread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId, "Attention: Tried to join itself!");

						int accumulatedTimeout = 0;
						int interval = 0; // Use a relatively short random interval to trigger the thread:
						while (!this.sendThread.Join(interval = SocketBase.Random.Next(5, 20)))
						{
							SignalSendThreadSafely();

							accumulatedTimeout += interval;
							if (accumulatedTimeout >= ThreadWaitTimeout)
							{
								DebugThreadStateMessage("...failed! Aborting...");
								DebugThreadStateMessage("(Abort is likely required due to failed synchronization back the calling thread, which is typically the GUI/main thread.)");
								this.sendThread.Abort();
								break;
							}

							DebugThreadStateMessage("...trying to join at " + accumulatedTimeout + " ms...");
						}
					}
					catch (ThreadStateException)
					{
						// Ignore thread state exceptions such as "Thread has not been started" and
						// "Thread cannot be aborted" as it just needs to be ensured that the thread
						// has or will be terminated for sure.

						DebugThreadStateMessage("...failed too but will be exectued as soon as the calling thread gets suspended again.");
					}
					finally
					{
						this.sendThread = null;
					}

					DebugThreadStateMessage("...successfully terminated.");
				}

				if (this.sendThreadEvent != null)
				{
					try     { this.sendThreadEvent.Close(); }
					finally { this.sendThreadEvent = null; }
				}
			}
		}

		#endregion

		#region Async Receive
		//==========================================================================================
		// Async Receive
		//==========================================================================================

		private void BeginReceiveIfEnabled()
		{
			lock (this.socketSyncObj)
			{
				System.Net.IPEndPoint localFilterEndPoint = new System.Net.IPEndPoint(this.localFilter, this.localPort);

				AsyncReceiveState state = new AsyncReceiveState(localFilterEndPoint, this.socket);
				this.socket.BeginReceive(new AsyncCallback(ReceiveCallback), state);
			}
		}

		private void ReceiveCallback(IAsyncResult ar)
		{
			AsyncReceiveState state = (AsyncReceiveState)(ar.AsyncState);

			// Ensure that async receive is discarded after close/dispose:
			if (!IsDisposed && (state.Socket != null) && (GetStateSynchronized() == SocketState.Opened)) // Check 'IsDisposed' first!
			{
				if (Monitor.TryEnter(this.dataEventSyncObj))
				{
					try
					{
						System.Net.IPEndPoint remoteEndPoint = state.LocalFilterEndPoint;
						byte[] data;
						try
						{
							data = state.Socket.EndReceive(ar, ref remoteEndPoint);
						}
						catch (System.Net.Sockets.SocketException ex)
						{
							data = null;
							SocketError();
							OnIOError(new IOErrorEventArgs(ErrorSeverity.Fatal, ex.Message));
						}

						if (data != null)
						{
							if (this.socketType == UdpSocketType.Server)
							{
								// Set the remote end point to the sender of the first or most recently received data, depending on send mode:

								bool hasChanged = false;

								lock (this.socketSyncObj)
								{
									bool updateRemoteEndPoint = false;

									switch (this.serverSendMode)
									{
										case UdpServerSendMode.None:                                                                       /* Do nothing. */           break;
										case UdpServerSendMode.First:      if (this.remoteHost.Address.Equals(System.Net.IPAddress.None)) updateRemoteEndPoint = true; break; // IPAddress does not override the ==/!= operators, thanks Microsoft guys...
										case UdpServerSendMode.MostRecent:                                                                updateRemoteEndPoint = true; break;
										default: throw (new InvalidOperationException("Program execution should never get here,'" + this.serverSendMode.ToString() + "' is an unknown UDP/IP server send mode!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
									}

									if (updateRemoteEndPoint)
									{
										if (!this.remoteHost.Equals(remoteEndPoint.Address)) // IPAddress does not override the ==/!= operators, thanks Microsoft guys...
										{
											this.remoteHost = remoteEndPoint.Address;
											hasChanged = true;
										}

										if (this.remotePort != remoteEndPoint.Port)
										{
											this.remotePort = remoteEndPoint.Port;
											hasChanged = true;
										}
									}
								}

								if (hasChanged)
									OnIOChanged(EventArgs.Empty);
							} // Server

							// This receive callback is always asychronous, thus the event handler can
							// be called directly. It is also ensured that the event handler is called
							// sequential because the 'BeginReceive()' method is only called after
							// the event handler has returned.
							OnDataReceived(new SocketDataReceivedEventArgs(data, remoteEndPoint));

							// Continue receiving data:
							BeginReceiveIfEnabled();
						}
					}
					finally
					{
						Monitor.Exit(this.dataEventSyncObj);
					}
				} // Monitor.TryEnter()
			} // if (!IsDisposed && ...)
		}

		private void SocketError()
		{
			DisposeSocketAndThread();
			SetStateSynchronizedAndNotify(SocketState.Error);
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
			throw (new InvalidOperationException("Program execution should never get here, the event 'IOControlChanged' is not in use for UDP/IP sockets!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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
			return ("Receive:" + this.localPort + " / " + this.remoteHost.ToEndpointAdressString() + ":" + this.remotePort);
		}

		#endregion

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		[Conditional("DEBUG")]
		private void DebugMessage(string message)
		{
			Debug.WriteLine
			(
				string.Format
				(
					CultureInfo.InvariantCulture,
					" @ {0} @ Thread #{1} : {2,36} {3,3} {4,-38} : {5}",
					DateTime.Now.ToString("HH:mm:ss.fff", DateTimeFormatInfo.InvariantInfo),
					Thread.CurrentThread.ManagedThreadId.ToString("D3", CultureInfo.InvariantCulture),
					GetType(),
					"#" + this.instanceId.ToString("D2", CultureInfo.InvariantCulture),
					"[" + ToShortEndPointString() + "]",
					message
				)
			);
		}

		[Conditional("DEBUG_THREAD_STATE")]
		private void DebugThreadStateMessage(string message)
		{
			DebugMessage(message);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
