//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.18
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2017 Matthias Kläy.
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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
//// 'System.Net' as well as 'ALAZ.SystemEx.NetEx' are explicitly used for more obvious distinction.
using System.Threading;

using MKY.Collections.Generic;
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
				LocalFilterEndPoint = localFilterEndPoint;
				Socket              = socket;
			}
		}

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const int SendQueueFixedCapacity      = 8192; // = default 'Socket.SendBufferSize'
		private const int ReceiveQueueInitialCapacity = 8192; // = default 'Socket.ReceiveBufferSize'

		private const int ThreadWaitTimeout = 200;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		/// <summary>
		/// A dedicated event helper to allow autonomously ignoring exceptions when disposed.
		/// </summary>
		private EventHelper.Item eventHelper = EventHelper.CreateItem();

		private int instanceId;
		private UdpSocketType socketType;

		private IPHostEx remoteHost;
		private int remotePort;
		private IPNetworkInterfaceEx localInterface;
		private int localPort;
		private IPFilterEx localFilter;

		private UdpServerSendMode serverSendMode;

		private SocketState state = SocketState.Closed;
		private object stateSyncObj = new object();

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

		/// <remarks>
		/// Async receiving. The capacity is set large enough to reduce the number of resizing
		/// operations while adding elements.
		/// </remarks>
		private Queue<Pair<byte, System.Net.IPEndPoint>> receiveQueue = new Queue<Pair<byte, System.Net.IPEndPoint>>(ReceiveQueueInitialCapacity);

		private bool receiveThreadRunFlag;
		private AutoResetEvent receiveThreadEvent;
		private Thread receiveThread;
		private object receiveThreadSyncObj = new object();

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
		/// <exception cref="ArgumentNullException"><paramref name="remoteHost"/> is is <c>null</c>.</exception>
		public UdpSocket(IPHostEx remoteHost, int remotePort)
			: this(SocketBase.NextInstanceId, remoteHost, remotePort)
		{
		}

		/// <summary>Creates a UDP/IP socket of type <see cref="UdpSocketType.Client"/>.</summary>
		/// <remarks>The local IP address filter is set to the remote IP address.</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="remoteHost"/> is is <c>null</c>.</exception>
		public UdpSocket(int instanceId, IPHostEx remoteHost, int remotePort)
			: this(instanceId, UdpSocketType.Client, remoteHost, remotePort, System.Net.IPAddress.Any, 0, (System.Net.IPAddress)remoteHost)
		{
		}

		/// <summary>Creates a UDP/IP socket of type <see cref="UdpSocketType.Server"/>.</summary>
		/// <remarks>The local IP network interface is defaulted to <see cref="System.Net.IPAddress.Any"/>.</remarks>
		/// <remarks>The local IP address filter is defaulted to <see cref="System.Net.IPAddress.Any"/>.</remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behavior.")]
		public UdpSocket(int localPort, UdpServerSendMode serverSendMode = UdpServerSendMode.MostRecent)
			: this(SocketBase.NextInstanceId, localPort, serverSendMode)
		{
		}

		/// <summary>Creates a UDP/IP socket of type <see cref="UdpSocketType.Server"/>.</summary>
		/// <remarks>The local IP network interface is defaulted to <see cref="System.Net.IPAddress.Any"/>.</remarks>
		/// <remarks>The local IP address filter is defaulted to <see cref="System.Net.IPAddress.Any"/>.</remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behavior.")]
		public UdpSocket(int instanceId, int localPort, UdpServerSendMode serverSendMode = UdpServerSendMode.MostRecent)
			: this(instanceId, UdpSocketType.Server, System.Net.IPAddress.None, 0, System.Net.IPAddress.Any, localPort, System.Net.IPAddress.Any, serverSendMode)
		{
		}

		/// <summary>Creates a UDP/IP socket of type <see cref="UdpSocketType.Server"/>.</summary>
		/// <exception cref="ArgumentNullException"><paramref name="localInterface"/> is is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="localFilter"/> is is <c>null</c>.</exception>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behavior.")]
		public UdpSocket(IPNetworkInterfaceEx localInterface, int localPort, IPFilterEx localFilter, UdpServerSendMode serverSendMode = UdpServerSendMode.MostRecent)
			: this(SocketBase.NextInstanceId, localInterface, localPort, localFilter, serverSendMode)
		{
		}

		/// <summary>Creates a UDP/IP socket of type <see cref="UdpSocketType.Server"/>.</summary>
		/// <exception cref="ArgumentNullException"><paramref name="localInterface"/> is is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="localFilter"/> is is <c>null</c>.</exception>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behavior.")]
		public UdpSocket(int instanceId, IPNetworkInterfaceEx localInterface, int localPort, IPFilterEx localFilter, UdpServerSendMode serverSendMode = UdpServerSendMode.MostRecent)
			: this(instanceId, UdpSocketType.Server, System.Net.IPAddress.None, 0, localInterface, localPort, localFilter, serverSendMode)
		{
		}

		/// <summary>Creates a UDP/IP socket of type <see cref="UdpSocketType.PairSocket"/>.</summary>
		/// <remarks>The local IP address filter is set to the remote IP address.</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="remoteHost"/> is is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="localInterface"/> is is <c>null</c>.</exception>
		public UdpSocket(IPHostEx remoteHost, int remotePort, IPNetworkInterfaceEx localInterface, int localPort)
			: this(SocketBase.NextInstanceId, remoteHost, remotePort, localInterface, localPort)
		{
		}

		/// <summary>Creates a UDP/IP socket of type <see cref="UdpSocketType.PairSocket"/>.</summary>
		/// <remarks>The local IP address filter is set to the remote IP address.</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="remoteHost"/> is is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="localInterface"/> is is <c>null</c>.</exception>
		public UdpSocket(int instanceId, IPHostEx remoteHost, int remotePort, IPNetworkInterfaceEx localInterface, int localPort)
			: this(instanceId, UdpSocketType.PairSocket, remoteHost, remotePort, localInterface, localPort, (System.Net.IPAddress)remoteHost)
		{
		}

		/// <summary>Creates a UDP/IP socket of the given type.</summary>
		/// <remarks>The local IP address filter is defaulted to <see cref="System.Net.IPAddress.Any"/>.</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="remoteHost"/> is is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="localInterface"/> is is <c>null</c>.</exception>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behavior.")]
		public UdpSocket(UdpSocketType socketType, IPHostEx remoteHost, int remotePort, IPNetworkInterfaceEx localInterface, int localPort, UdpServerSendMode serverSendMode = UdpServerSendMode.MostRecent)
			: this(SocketBase.NextInstanceId, socketType, remoteHost, remotePort, localInterface, localPort, serverSendMode)
		{
		}

		/// <summary>Creates a UDP/IP socket of the given type.</summary>
		/// <remarks>The local IP address filter is defaulted to <see cref="System.Net.IPAddress.Any"/>.</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="remoteHost"/> is is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="localInterface"/> is is <c>null</c>.</exception>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behavior.")]
		public UdpSocket(int instanceId, UdpSocketType socketType, IPHostEx remoteHost, int remotePort, IPNetworkInterfaceEx localInterface, int localPort, UdpServerSendMode serverSendMode = UdpServerSendMode.MostRecent)
			: this(instanceId, socketType, remoteHost, remotePort, localInterface, localPort, System.Net.IPAddress.Any, serverSendMode)
		{
		}

		/// <summary>Creates a UDP/IP socket of the given type.</summary>
		/// <exception cref="ArgumentNullException"><paramref name="remoteHost"/> is is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="localInterface"/> is is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="localFilter"/> is is <c>null</c>.</exception>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behavior.")]
		public UdpSocket(UdpSocketType socketType, IPHostEx remoteHost, int remotePort, IPNetworkInterfaceEx localInterface, int localPort, IPFilterEx localFilter, UdpServerSendMode serverSendMode = UdpServerSendMode.MostRecent)
			: this(SocketBase.NextInstanceId, socketType, remoteHost, remotePort, localInterface, localPort, localFilter, serverSendMode)
		{
		}

		/// <summary>Creates a UDP/IP socket of the given type.</summary>
		/// <exception cref="ArgumentNullException"><paramref name="remoteHost"/> is is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="localInterface"/> is is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="localFilter"/> is is <c>null</c>.</exception>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behavior.")]
		public UdpSocket(int instanceId, UdpSocketType socketType, IPHostEx remoteHost, int remotePort, IPNetworkInterfaceEx localInterface, int localPort, IPFilterEx localFilter, UdpServerSendMode serverSendMode = UdpServerSendMode.MostRecent)
		{
			// Verify reference arguments:

			if (remoteHost     == null) throw (new ArgumentNullException("remoteHost"));
			if (localInterface == null) throw (new ArgumentNullException("localInterface"));
			if (localFilter    == null) throw (new ArgumentNullException("localFilter"));

			// Arguments are defined:

			this.instanceId     = instanceId;
			this.socketType     = socketType;

			this.remoteHost     = remoteHost;
			this.remotePort     = remotePort;
			this.localInterface = localInterface;
			this.localPort      = localPort;
			this.localFilter    = localFilter;

			this.serverSendMode = serverSendMode;
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public bool IsDisposed { get; protected set; }

		/// <summary></summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary></summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				DebugEventManagement.DebugWriteAllEventRemains(this);
				this.eventHelper.DiscardAllEventsAndExceptions();

				DebugMessage("Disposing...");

				// Dispose of managed resources if requested:
				if (disposing)
				{
					// In the 'normal' case, the items have already been disposed of, e.g. in Stop().
					DisposeSocketAndThreads();
				}

				// Set state to disposed:
				IsDisposed = true;

				DebugMessage("...successfully disposed.");
			}
		}

	#if (DEBUG)

		/// <remarks>
		/// Microsoft.Design rule CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable requests
		/// "Types that declare disposable members should also implement IDisposable. If the type
		///  does not own any unmanaged resources, do not implement a finalizer on it."
		/// 
		/// Well, true for best performance on finalizing. However, it's not easy to find missing
		/// calls to <see cref="Dispose()"/>. In order to detect such missing calls, the finalizer
		/// is kept for DEBUG, indicating missing calls.
		/// 
		/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
		/// </remarks>
		~UdpSocket()
		{
			Dispose(false);

			DebugDisposal.DebugNotifyFinalizerInsteadOfDispose(this);
		}

	#endif // DEBUG

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (IsDisposed)
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
					return (IPAddressEx.NotEqualsNone(this.remoteHost.Address));
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
		public virtual IPNetworkInterfaceEx LocalInterface
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.localInterface);
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
			// AssertNotDisposed() is called by 'IsStopped' below.

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
				DebugMessage("Start() requested but state is already " + GetStateSynchronized() + ".");
				return (true); // Return 'true' since socket is already started.
			}
		}

		private void StartSocket()
		{
			SetStateSynchronizedAndNotify(SocketState.Opening);
			CreateSocketAndThreads();
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
			DisposeSocketAndThreads();
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
					// But attention, do not simply lock() the sync obj. Instead, just try
					// to get the lock or try again later. The thread = direction that get's
					// the lock first, shall also be the one to signal first:

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
						}
						finally
						{
							Monitor.Exit(this.dataEventSyncObj);
						}
					} // Monitor.TryEnter()

					// Note the Thread.Sleep(TimeSpan.Zero) above.

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

		private void CreateSocketAndThreads()
		{
			// First, create and start receive thread to be ready when socket first receives data:
			lock (this.receiveThreadSyncObj)
			{
				if (this.receiveThread == null)
				{
					this.receiveThreadRunFlag = true;
					this.receiveThreadEvent = new AutoResetEvent(false);
					this.receiveThread = new Thread(new ThreadStart(ReceiveThread));
					this.receiveThread.Name = ToShortEndPointString() + " Receive Thread";
					this.receiveThread.Start();
				}
			}

			// Then, create socket:
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
					this.socket.Client.Bind(new System.Net.IPEndPoint(this.localInterface.Address, this.localPort));
				}
				else // Client
				{
					// The socket is not bound to a local port, neither limited to a specific interface.
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

			// Finally, create and start send thread:
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

		/// <remarks>
		/// Especially useful during potentially dangerous creation and disposal sequence.
		/// </remarks>
		private void SignalReceiveThreadSafely()
		{
			try
			{
				if (this.receiveThreadEvent != null)
					this.receiveThreadEvent.Set();
			}
			catch (ObjectDisposedException ex) { DebugEx.WriteException(GetType(), ex, "Unsafe thread signaling caught"); }
			catch (NullReferenceException ex)  { DebugEx.WriteException(GetType(), ex, "Unsafe thread signaling caught"); }

			// Catch 'NullReferenceException' for the unlikely case that the event has just been
			// disposed after the if-check. This way, the event doesn't need to be locked (which
			// is a relatively time-consuming operation). Still keep the if-check for the normal
			// cases.
		}

		private void DisposeSocketAndThreads()
		{
			// First clear both flags to reduce the time to stop the receive thread, it may already
			// be signaled while receiving data while the send thread is still running.
			lock (this.sendThreadSyncObj)
				this.sendThreadRunFlag = false;
			lock (this.receiveThreadSyncObj)
				this.receiveThreadRunFlag = false;

			// First, stop and dispose send thread to prevent more data being forwarded to socket:
			lock (this.sendThreadSyncObj)
			{
				if (this.sendThread != null)
				{
					DebugThreadStateMessage("SendThread() gets stopped...");

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

						DebugThreadStateMessage("...successfully stopped.");
					}
					catch (ThreadStateException)
					{
						// Ignore thread state exceptions such as "Thread has not been started" and
						// "Thread cannot be aborted" as it just needs to be ensured that the thread
						// has or will be terminated for sure.

						DebugThreadStateMessage("...failed too but will be exectued as soon as the calling thread gets suspended again.");
					}

					this.sendThread = null;
				}

				if (this.sendThreadEvent != null)
				{
					try     { this.sendThreadEvent.Close(); }
					finally { this.sendThreadEvent = null; }
				}
			}

			// Then, close and dispose socket:
			lock (this.socketSyncObj)
			{
				if (this.socket != null)
				{
					this.socket.Close();
					this.socket = null;
				}
			}

			// Finally, stop and dispose receive thread:
			lock (this.receiveThreadSyncObj)
			{
				if (this.receiveThread != null)
				{
					DebugThreadStateMessage("ReceiveThread() gets stopped...");

					// Ensure that receive thread has stopped after the stop request:
					try
					{
						Debug.Assert(this.receiveThread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId, "Attention: Tried to join itself!");

						int accumulatedTimeout = 0;
						int interval = 0; // Use a relatively short random interval to trigger the thread:
						while (!this.receiveThread.Join(interval = SocketBase.Random.Next(5, 20)))
						{
							SignalReceiveThreadSafely();

							accumulatedTimeout += interval;
							if (accumulatedTimeout >= ThreadWaitTimeout)
							{
								DebugThreadStateMessage("...failed! Aborting...");
								DebugThreadStateMessage("(Abort is likely required due to failed synchronization back the calling thread, which is typically the GUI/main thread.)");
								this.receiveThread.Abort();
								break;
							}

							DebugThreadStateMessage("...trying to join at " + accumulatedTimeout + " ms...");
						}

						DebugThreadStateMessage("...successfully stopped.");
					}
					catch (ThreadStateException)
					{
						// Ignore thread state exceptions such as "Thread has not been started" and
						// "Thread cannot be aborted" as it just needs to be ensured that the thread
						// has or will be terminated for sure.

						DebugThreadStateMessage("...failed too but will be exectued as soon as the calling thread gets suspended again.");
					}

					this.receiveThread = null;
				}

				if (this.receiveThreadEvent != null)
				{
					try     { this.receiveThreadEvent.Close(); }
					finally { this.receiveThreadEvent = null; }
				}
			} // lock (sendThreadSyncObj)
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
				// Ensure that async receive is no longer initiated after close/dispose:
				if (!IsDisposed && (GetStateSynchronized() == SocketState.Opened)) // Check 'IsDisposed' first!
				{
					System.Net.IPEndPoint localFilterEndPoint = new System.Net.IPEndPoint(this.localFilter, this.localPort);

					AsyncReceiveState state = new AsyncReceiveState(localFilterEndPoint, this.socket);
					this.socket.BeginReceive(new AsyncCallback(ReceiveCallback), state);
				}
			}
		}

		private void ReceiveCallback(IAsyncResult ar)
		{
			AsyncReceiveState state = (AsyncReceiveState)(ar.AsyncState);

			// Ensure that async receive is discarded after close/dispose:
			if (!IsDisposed && (state.Socket != null) && (GetStateSynchronized() == SocketState.Opened)) // Check 'IsDisposed' first!
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
					// Handle data:
					lock (this.receiveQueue) // Lock is required because Queue<T> is not synchronized.
					{
						foreach (byte b in data)
							this.receiveQueue.Enqueue(new Pair<byte, System.Net.IPEndPoint>(b, remoteEndPoint));

						// Note that individual bytes are enqueued, not array of bytes. Analysis has
						// shown that this is faster than enqueuing arrays, since this callback will
						// mostly be called with rather low numbers of bytes.
					}

					// Handle connection:
					if (this.socketType == UdpSocketType.Server)
					{
						// Set the remote end point to the sender of the first or most recently received data, depending on send mode:

						bool hasChanged = false;

						lock (this.socketSyncObj)
						{
							bool updateRemoteEndPoint = false;

							switch (this.serverSendMode)
							{
								case UdpServerSendMode.None:                                                             /* Do nothing. */           break;
								case UdpServerSendMode.First:      if (IPAddressEx.EqualsNone(this.remoteHost.Address)) updateRemoteEndPoint = true; break;
								case UdpServerSendMode.MostRecent:                                                      updateRemoteEndPoint = true; break;
								default: throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "'" + this.serverSendMode.ToString() + "' is an unknown UDP/IP server send mode!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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

					// Signal data notification to receive thread:
					SignalReceiveThreadSafely();
				} // if (data != null)

				// Continue receiving:
				BeginReceiveIfEnabled();
			} // if (!IsDisposed && ...)
		}

		/// <summary>
		/// Asynchronously manage incoming events to prevent potential deadlocks if close/dispose
		/// was called from a ISynchronizeInvoke target (i.e. a form) on an event thread.
		/// Also, the mechanism implemented below reduces the amount of events that are propagated
		/// to the main application. Small chunks of received data will generate many events
		/// handled by <see cref="ReceiveCallback"/>. However, since <see cref="OnDataReceived"/>
		/// synchronously invokes the event, it will take some time until the send queue is checked
		/// again. During this time, no more new events are invoked, instead, incoming data is
		/// buffered.
		/// </summary>
		/// <remarks>
		/// Will be signaled by <see cref="ReceiveCallback"/> event above.
		/// </remarks>
		[SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", MessageId = "System.Threading.WaitHandle.#WaitOne(System.Int32)", Justification = "Installer indeed targets .NET 3.5 SP1.")]
		private void ReceiveThread()
		{
			DebugThreadStateMessage("ReceiveThread() has started.");

			// Outer loop, processes data after a signal was received:
			while (!IsDisposed && this.receiveThreadRunFlag) // Check 'IsDisposed' first!
			{
				try
				{
					// WaitOne() will wait forever if the underlying I/O provider has crashed, or
					// if the overlying client isn't able or forgets to call Stop() or Dispose().
					// Therefore, only wait for a certain period and then poll the run flag again.
					// The period can be quite long, as an event trigger will immediately resume.
					if (!this.receiveThreadEvent.WaitOne(SocketBase.Random.Next(50, 200)))
						continue;
				}
				catch (AbandonedMutexException ex)
				{
					// The mutex should never be abandoned, but in case it nevertheless happens,
					// at least output a debug message and gracefully exit the thread.
					DebugEx.WriteException(GetType(), ex, "An 'AbandonedMutexException' occurred in ReceiveThread()!");
					break;
				}

				// Inner loop, runs as long as there is data in the receive queue.
				// Ensure not to forward events during disposing anymore. Check 'IsDisposed' first!
				while (!IsDisposed && this.receiveThreadRunFlag && (this.receiveQueue.Count > 0))
				{                                               // No lock required, just checking for empty.
					// Initially, yield to other threads before starting to read the queue, since it is very
					// likely that more data is to be enqueued, thus resulting in larger chunks processed.
					// Subsequently, yield to other threads to allow processing the data.
					Thread.Sleep(TimeSpan.Zero);

					if (Monitor.TryEnter(this.dataEventSyncObj))
					{
						try
						{
							System.Net.IPEndPoint remoteEndPoint = null;
							List<byte> data;

							lock (this.receiveQueue) // Lock is required because Queue<T> is not synchronized.
							{
								data = new List<byte>(this.receiveQueue.Count); // Preset the initial capacity to improve memory management.

								while (this.receiveQueue.Count > 0)
								{
									Pair<byte, System.Net.IPEndPoint> item;

									// First, peek to check whether data refers to a different end point:
									item = this.receiveQueue.Peek();

									if (remoteEndPoint == null)
										remoteEndPoint = item.Value2;
									else if (remoteEndPoint != item.Value2)
										break; // Break as soon as data of a different end point is available.

									// If still the same end point, dequeue the item to acknowledge it's gone:
									item = this.receiveQueue.Dequeue();
									data.Add(item.Value1);
								}
							}

							OnDataReceived(new SocketDataReceivedEventArgs(data.ToArray(), remoteEndPoint));
						}
						finally
						{
							Monitor.Exit(this.dataEventSyncObj);
						}
					} // Monitor.TryEnter()

					// Note the Thread.Sleep(TimeSpan.Zero) above.

					// Saying hello to StyleCop ;-.
				} // Inner loop
			} // Outer loop

			DebugThreadStateMessage("ReceiveThread() has terminated.");
		}

		private void SocketError()
		{
			DisposeSocketAndThreads();
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
			this.eventHelper.FireSync(IOChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOControlChanged(EventArgs e)
		{
			UnusedEvent.PreventCompilerWarning(IOControlChanged);
			throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The event 'IOControlChanged' is not in use for UDP/IP sockets!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary></summary>
		protected virtual void OnIOError(IOErrorEventArgs e)
		{
			this.eventHelper.FireSync<IOErrorEventArgs>(IOError, this, e);
		}

		/// <summary></summary>
		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true)]
		protected virtual void OnDataReceived(DataReceivedEventArgs e)
		{
			if (IsOpen) // Make sure to propagate event only if active.
				this.eventHelper.FireSync<DataReceivedEventArgs>(DataReceived, this, e);
		}

		/// <summary></summary>
		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true)]
		protected virtual void OnDataSent(DataSentEventArgs e)
		{
			if (IsOpen) // Make sure to propagate event only if active.
				this.eventHelper.FireSync<DataSentEventArgs>(DataSent, this, e);
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
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
			return ("Receive:" + this.localPort + " / " + this.remoteHost.ToEndpointAddressString() + ":" + this.remotePort);
		}

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
