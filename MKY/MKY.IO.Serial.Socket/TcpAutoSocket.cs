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
// MKY Version 1.0.25
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2021 Matthias Kläy.
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

	// Enable debugging of state:
////#define DEBUG_STATE

	// Enable debugging of a static list of all sockets:
////#define DEBUG_STATIC_SOCKET_LIST // Attention: Must also be activated in TcpAutoSocket.[Client+Server].cs !!

#endif // DEBUG

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
//// "System.Net" as well as "ALAZ.SystemEx.NetEx" are explicitly used for more obvious distinction.
//// "System.Net.Sockets" as well.
using System.Threading;

using MKY.Contracts;
using MKY.Net;

#endregion

namespace MKY.IO.Serial.Socket
{
	/// <summary>
	/// Implements the <see cref="IIOProvider"/> interface for an automatic TCP/IP client or server.
	/// On startup, an AutoSocket tries to connect to a remote server and run as client. If this
	/// fails, it tries to run as server. Retry cycles and random wait times ensure proper operation
	/// even when multiple AutoSockets try to interconnected to each other.
	/// </summary>
	/// <remarks>
	/// Initially, YAT AutoSockets with the original ALAZ implementation created a deadlock on
	/// shutdown when two AutoSockets that were interconnected with each other. The situation:
	///
	/// 1. The main thread requests stopping all terminals:
	///     => YAT.Model.Workspace.CloseAllTerminals()
	///         => MKY.IO.Serial.TcpAutoSocket.Stop()
	///            => ALAZ.SystemEx.NetEx.SocketsEx.SocketServer.Stop()
	///                => ALAZ.SystemEx.NetEx.SocketsEx.BaseSocketConnection.Active.get()
	///
	/// 2. As a result, the first AutoSocket shuts down, but the second changes from 'Accepted' to
	///    'Listening' and tries to synchronize from the ALAZ socket event to the main thread:
	///     => ALAZ.SystemEx.NetEx.SocketsEx.BaseSocketConnectionHost.FireOnDisconnected()
	///         => MKY.IO.Serial.TcpAutoSocket.OnIOChanged()
	///             => YAT.Model.Terminal.OnIOChanged()
	///                 => Deadlock when synchronizing onto main thread!
	///
	/// The issue has been solved in <see cref="ALAZ.SystemEx.NetEx.SocketsEx.BaseSocketConnection"/>
	/// as well as <see cref="TcpClient"/> or <see cref="TcpServer"/> by invoking Stop() and Dispose()
	/// of socket and connections and thread asynchronously and without firing events. See remarks
	/// of these classes for additional information.
	/// </remarks>
	/// <remarks>
	/// This class is implemented using partial classes separating client/server functionality.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "A type shall spell 'Tcp' like this...")]
	public partial class TcpAutoSocket : DisposableBase, IIOProvider
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private enum SocketState
		{
			Reset,
			StartingConnecting,
			Connecting,
			Connected,
			StartingListening,
			Listening,
			Accepted,
		////Restarting,
			Stopping,
			Error
		}

		private enum SocketUse
		{
			None,
			Client,
			Server
		}

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const int MaxStartCycles = 5;

		private const int MinConnectDelay = 50;
		private const int MaxConnectDelay = 500;

		private const int MinListenDelay = 50;
		private const int MaxListenDelay = 500;

		/// <remarks>
		/// Using the sum of the underlying sockets for two reasons:
		/// <list type="bullet">
		/// <item><description>Best guess of the underlying sockets.</description></item>
		/// <item><description>For sure larger than the underlying time-out.</description></item>
		/// </list>
		/// </remarks>
		private const int SocketStopTimeout = (TcpClient.SocketStopTimeout + TcpServer.SocketStopTimeout);

		/// <remarks>
		/// Can be quite short. A longer interval would delay stopping longer than necessary.
		/// </remarks>
		private const int SocketStopInterval = 10;

		private const string Undefined = "(undefined)"; // Lower case same as "localhost".

		#endregion

		#region Static Fields
		//==========================================================================================
		// Static Fields
		//==========================================================================================

	#if (DEBUG_STATIC_SOCKET_LIST)
		private static System.Collections.Generic.List<IIOProvider> staticSocketList = new System.Collections.Generic.List<IIOProvider>();
	#endif

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private int instanceId;

		/// <summary>
		/// A dedicated event helper to allow discarding exceptions when object got disposed.
		/// </summary>
		private EventHelper.Item eventHelper = EventHelper.CreateItem(typeof(TcpAutoSocket).FullName);

		private IPHostEx remoteHost;
		private int remotePort;
		private IPNetworkInterfaceEx localInterface;
		private int localPort;

		private SocketState state = SocketState.Reset;
		private object stateSyncObj = new object();

		private TcpClient client;
		private TcpServer server;

		private int startCycleCounter; // = 0;
		private object startCycleCounterSyncObj = new object();

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		public event EventHandler<EventArgs<DateTime>> IOChanged;

		/// <summary></summary>
		public event EventHandler<EventArgs<DateTime>> IOControlChanged;

		/// <summary></summary>
		public event EventHandler<IOWarningEventArgs> IOWarning;

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

		/// <summary>Creates a TCP/IP AutoSocket.</summary>
		/// <exception cref="ArgumentException"><paramref name="remoteHost"/> is <see cref="IPHost.Explicit"/>.</exception>
		/// <exception cref="ArgumentException"><paramref name="localInterface"/> is <see cref="IPNetworkInterface.Explicit"/>.</exception>
		/// <exception cref="ArgumentException">Mismatching <see cref="System.Net.Sockets.AddressFamily"/> of <paramref name="remoteHost"/> and <paramref name="localInterface"/>.</exception>
		public TcpAutoSocket(IPHost remoteHost, int remotePort, IPNetworkInterface localInterface, int localPort)
			: this((IPHostEx)remoteHost, remotePort, (IPNetworkInterfaceEx)localInterface, localPort)
		{
		}

		/// <summary>Creates a TCP/IP AutoSocket.</summary>
		/// <exception cref="ArgumentNullException"><paramref name="remoteHost"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="localInterface"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException">Mismatching <see cref="System.Net.Sockets.AddressFamily"/> of <paramref name="remoteHost"/> and <paramref name="localInterface"/>.</exception>
		public TcpAutoSocket(IPHostEx remoteHost, int remotePort, IPNetworkInterfaceEx localInterface, int localPort)
		{
			// Assert by-reference arguments:

			if (remoteHost     == null) throw (new ArgumentNullException("remoteHost",     MessageHelper.InvalidExecutionPreamble + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			if (localInterface == null) throw (new ArgumentNullException("localInterface", MessageHelper.InvalidExecutionPreamble + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			// All arguments are defined!

			if (remoteHost.Address.AddressFamily != localInterface.Address.AddressFamily) // Do not prepend/append 'SubmitBug' as an application could rely and the error message.
				throw (new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Mismatching address families! Remote host is {0} while local interface is {1}.", remoteHost.Address.AddressFamily, localInterface.Address.AddressFamily)));

			this.instanceId = SocketBase.NextInstanceId;

			this.remoteHost     = remoteHost;
			this.remotePort     = remotePort;
			this.localInterface = localInterface;
			this.localPort      = localPort;
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <param name="disposing">
		/// <c>true</c> when called from <see cref="Dispose"/>,
		/// <c>false</c> when called from finalizer.
		/// </param>
		protected override void Dispose(bool disposing)
		{
			if (this.eventHelper != null) // Possible when called by finalizer (non-deterministic).
				this.eventHelper.DiscardAllEventsAndExceptions();

			// Dispose of managed resources:
			if (disposing)
			{
				DebugMessage("Disposing...");

				// In the 'normal' case, the items have already been disposed of, e.g. in Stop().
				DisposeSocketSynchronized();

				DebugMessage("...successfully disposed.");
			}

		////base.Dispose(disposing) of 'DisposableBase' doesn't need and cannot be called since abstract.
		}

		#endregion

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public virtual IPHostEx RemoteHost
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				return (this.remoteHost);
			}
		}

		/// <remarks>Convenience method, always returns a valid value, at least "(undefined)".</remarks>
		protected virtual string RemoteHostEndpointString
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				return ((RemoteHost != null) ? (RemoteHost.ToEndpointAddressString()) : Undefined);
			}
		}

		/// <summary></summary>
		public virtual int RemotePort
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				return (this.remotePort);
			}
		}

		/// <summary></summary>
		public virtual IPNetworkInterfaceEx LocalInterface
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				return (this.localInterface);
			}
		}

		/// <summary></summary>
		public virtual int LocalPort
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				return (this.localPort);
			}
		}

		/// <summary></summary>
		public virtual bool IsStopped
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				switch (GetStateSynchronized())
				{
					case SocketState.Reset:
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
		public virtual bool IsListening
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				switch (GetStateSynchronized())
				{
					case SocketState.Listening:
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
			////AssertUndisposed() shall not be called from this simple get-property.

				switch (GetStateSynchronized())
				{
					case SocketState.Connected:
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
		public virtual int ConnectionCount
		{
			get
			{
				AssertUndisposed();

				lock (this.stateSyncObj) // Directly locking the state is OK, 'ConnectionCount' cannot result in a state related deadlock.
				{
					if      (IsClient && (this.client != null))
						return (this.client.ConnectionCount);
					else if (IsServer && (this.server != null))
						return (this.server.ConnectionCount);
					else
						return (0);
				}
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

		/// <remarks>
		/// The value of this property only reflects the state of the send queue.
		/// <para>
		/// The state of the underlying <see cref="ALAZ.SystemEx.NetEx.SocketsEx.ISocketConnection"/>
		/// (i.e. calls to <see cref="ALAZ.SystemEx.NetEx.SocketsEx.ISocketConnection.BeginSend"/>
		/// and their callbacks to <see cref="ALAZ.SystemEx.NetEx.SocketsEx.ISocketService.OnSent"/>)
		/// is not taken into account because keeping track of ongoing send requests and callbacks is
		/// not feasible to implement in a solid way. E.g. incrementing the number of requested bytes
		/// and decrementing them in the callback would be susceptible to inconsistencies, e.g. in
		/// case of connection state related exceptions.
		/// </para><para>
		/// Neither is the state of the underlying operating system socket taken into account, as
		/// its state cannot be retrieved from within this .NET implementation by common means.
		/// </para></remarks>
		public virtual bool IsSending
		{
			get
			{
				AssertUndisposed();

				lock (this.stateSyncObj) // Directly locking the state is OK, 'IsSending' cannot result in a state related deadlock.
				{
					if      (IsClient && (this.client != null))
						return (this.client.IsSending);
					else if (IsServer && (this.server != null))
						return (this.server.IsSending);
					else
						return (false);
				}
			}
		}

		/// <remarks>
		/// The <see cref="IsClient"/> and <see cref="IsServer"/> properties only return <c>true</c>
		/// if it is defined whether the AutoSocket indeed behaves as client or server. If it is not
		/// yet defined, both flags are set <c>false</c>.
		/// </remarks>
		public virtual bool IsClient
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				switch (GetStateSynchronized())
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

		/// <remarks>
		/// The <see cref="IsClient"/> and <see cref="IsServer"/> properties only return <c>true</c>
		/// if it is defined whether the AutoSocket indeed behaves as client or server. If it is not
		/// yet defined, both flags are set <c>false</c>.
		/// </remarks>
		public virtual bool IsServer
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				switch (GetStateSynchronized())
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

		/// <remarks>
		/// Convenience property to combine the <see cref="IsClient"/> and <see cref="IsServer"/>
		/// properties. It returns <c>true</c> if it is defined whether the AutoSocket indeed is
		/// a client or server. If it is not yet defined, it returns <c>false</c>.
		/// </remarks>
		public virtual bool IsClientOrServer
		{
			get { return (IsClient || IsServer); }
		}

		/// <summary></summary>
		public virtual object UnderlyingIOInstance
		{
			get
			{
				AssertUndisposed();

				lock (this.stateSyncObj) // Directly locking the state is OK, 'UnderlyingIOInstance' cannot result in a state related deadlock.
				{
					if      (IsClient && (this.client != null))
						return (this.client.UnderlyingIOInstance);
					else if (IsServer && (this.server != null))
						return (this.server.UnderlyingIOInstance);
					else
						return (null);
				}
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
			AssertUndisposed();

			SocketState state = GetStateSynchronized();
			switch (state)
			{
				case SocketState.Reset:
				case SocketState.Error:
				{
					DebugMessage("Starting...");
					StartAutoSocket();
					return (true);
				}
				default:
				{
					DebugMessage("Start() requested but state already is {0}.", state);
					return (true); // Return 'true' since socket is already started.
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "'Stop' is a common term to start/stop something.")]
		public virtual void Stop()
		{
			AssertUndisposed();

			SocketState state = GetStateSynchronized();
			switch (state)
			{
				case SocketState.Reset:
				{
					DebugMessage("Stop() requested but state is {0}.", state);
					return;
				}
				default:
				{
					DebugMessage("Stopping...");
					StopAutoSocket();
					return;
				}
			}
		}

		/// <summary></summary>
		public virtual bool Send(byte[] data)
		{
			AssertUndisposed();

			if (IsTransmissive)
			{
				try
				{
					var socketUse = GetSocketUseSynchronized();    // Send() will notify for sure! Thus,
					switch (socketUse)                             // to prevent potential state deadlocks,
					{                                              // it must not be called within a lock!
						case SocketUse.Client: return (this.client.Send(data));
						case SocketUse.Server: return (this.server.Send(data));
						case SocketUse.None:   return (false);

						default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + socketUse + "' is an item that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					}
				}
				catch (NullReferenceException) // Handle the unlikely but possible case where the socket
				{                              // changes right between selecting but before accessing it.
					return (false);
				}
			}

			return (false);
		}

		/// <summary>
		/// Clears the send buffer(s) immediately.
		/// </summary>
		public virtual int ClearSendBuffer()
		{
			AssertUndisposed();

			if (IsStarted) // Clearing the buffer shall be executed also when not 'Connected'/'Open'/'Transmissive'.
			{
				try
				{
					var socketUse = GetSocketUseSynchronized();    // ClearSendBuffer() may notify! Thus,
					switch (socketUse)                             // to prevent potential state deadlocks,
					{                                              // it must not be called within a lock!
						case SocketUse.Client: return (this.client.ClearSendBuffer());
						case SocketUse.Server: return (this.server.ClearSendBuffer());
						case SocketUse.None:   return (0);

						default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + socketUse + "' is an item that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					}
				}
				catch (NullReferenceException) // Handle the unlikely but possible case where the socket
				{                              // changes right between selecting but before accessing it.
					return (0);
				}
			}

			return (0);
		}

		#endregion

		#region State Methods
		//==========================================================================================
		// State Methods
		//==========================================================================================

		private SocketState GetStateSynchronized()
		{
			lock (this.stateSyncObj)
				return (this.state);
		}

		private bool SetStateSynchronized(SocketState state, bool notify)
		{
			DateTime timeStamp;
			return (SetStateSynchronized(state, out timeStamp, notify));
		}

		private bool SetStateSynchronized(SocketState state, out DateTime timeStamp, bool notify)
		{
			SocketState oldState;

			lock (this.stateSyncObj)
			{
				oldState = this.state;
				this.state = state;
				timeStamp = DateTime.Now; // Inside lock for accuracy.

			#if (DEBUG) // Inside lock to prevent potential mixup in debug output.
				string isClientOrServerString;
				if      (IsClient && IsConnected) // "Doppel-moppel", but keep it as a check during development and debugging
					isClientOrServerString = "connected as client";
				else if (IsServer && IsConnected)
					isClientOrServerString = "connected as server";
				else if (IsServer && !IsConnected)
					isClientOrServerString = "server and listening";
				else
					isClientOrServerString = "neither client nor server";

				if (state != oldState)
					DebugMessage("State has changed from {0} to {1}, is {2}.", oldState, state, isClientOrServerString);
				else
					DebugState("State already is {0}.", oldState); // State non-changes shall only be output when explicitly activated.
			#endif
			}

			if (notify && (state != oldState)) // Outside lock is OK, only stating change, not state.
				NotifyStateHasChanged(timeStamp);

			return (state != oldState);
		}

		private void NotifyStateHasChanged(DateTime timeStamp)
		{
			OnIOChanged(new EventArgs<DateTime>(timeStamp));
		}

		#endregion

		#region Socket Methods
		//==========================================================================================
		// Socket Methods
		//==========================================================================================

		private SocketUse GetSocketUseSynchronized()
		{
			lock (this.stateSyncObj)
			{
				if      (IsClient && (this.client != null))
					return (SocketUse.Client);
				else if (IsServer && (this.server != null))
					return (SocketUse.Server);
				else
					return (SocketUse.None);
			}
		}

		/// <remarks>
		/// See remarks of called methods.
		/// </remarks>
		private void StopSocketSyncAndDisposeSynchronized() // Only either client or server.
		{
			lock (this.stateSyncObj)
			{
				DebugMessage("Stopping socket..."); // Only either client or server.

				StopClientSyncAndDisposeSynchronized();
				StopServerSyncAndDisposeSynchronized();

				DebugMessage("...completed.");
			}
		}

		/// <remarks>
		/// Opposed to the ALAZ sockets, the MKY sockets do stop on Dispose(), but both is done asynchronously!
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		private void DisposeSocketSynchronized() // Only either client or server.
		{
			lock (this.stateSyncObj)
			{
				DebugMessage("Disposing socket..."); // Only either client or server.

				DisposeClientSynchronized();
				DisposeServerSynchronized();

				DebugMessage("...completed.");
			}
		}

		#endregion

		#region AutoSocket State Transitions
		//==========================================================================================
		// AutoSocket State Transitions
		//==========================================================================================

		private void StartAutoSocket()
		{
			lock (this.startCycleCounterSyncObj)
				this.startCycleCounter = 1;

			SetStateSynchronized(SocketState.StartingConnecting, notify: true);
			StartConnecting();
		}

		private void RestartWithDelayAndStartConnectingAsync()
		{
			lock (this.startCycleCounterSyncObj)
				this.startCycleCounter = 1;

			DelayAndStartConnectingAsync();
		}

		private void DelayAndStartConnectingAsync()
		{
			var asyncInvoker = new Action(DelayAndStartConnectingAsyncCallback);
			asyncInvoker.BeginInvoke(null, null);
		}

		private void DelayAndStartConnectingAsyncCallback()
		{
			int randomDelay = SocketBase.Random.Next(MinConnectDelay, MaxConnectDelay);
			DebugMessage("Delaying connecting by random value of {0} ms.", randomDelay);
			Thread.Sleep(randomDelay);

			StartConnecting();
		}

		private void StartConnecting()
		{
			// Only continue if still in expected state after the delay. Required because the
			// sequence of the operation of an 'AutoSocket' is not defined. There are several
			// situations where more than one trigger invokes this method, e.g.:
			//  > StartListening() fails at Start().
			//  > server_IOError() in case 'StartingListening'.
			// The 'AutoSocket' must be able to deal with such situations.

			Monitor.Enter(this.stateSyncObj);                             // Ensure state is handled atomically. Not using
			if (GetStateSynchronized() == SocketState.StartingConnecting) // lock() for being able to selectively release.
			{
				DateTime timeStamp;
				var stateHasChanged = SetStateSynchronized(SocketState.Connecting, out timeStamp, notify: false); // Notify outside lock!

				CreateClientSynchronized(this.remoteHost, this.remotePort, this.localInterface);
				Monitor.Exit(this.stateSyncObj);

				bool isStarting = false;
				try
				{
					isStarting = this.client.Start(); // Will be started asynchronously but may notify! Thus outside lock!
				}
				catch (NullReferenceException)
				{
					// A 'NullReferenceException' can occur in the unlikely case where the client has just gotten disposed of.
				}
				finally
				{
					if (!isStarting)
					{
						lock (this.stateSyncObj) // Ensure state is handled atomically.
						{
							DisposeClientSynchronized(); // Not started at all, no need for two-stage stop-dispose.

							stateHasChanged = SetStateSynchronized(SocketState.StartingListening, out timeStamp, notify: false); // Notify outside lock!
						}

						if (stateHasChanged)
							NotifyStateHasChanged(timeStamp); // Notify outside lock!

						// Continue starting AutoSocket by trying again as server:
						DelayAndStartListeningAsync(); // Invoke outside lock!
					}
					else
					{
						if (stateHasChanged)
							NotifyStateHasChanged(timeStamp); // Notify outside lock!
					}
				}
			}
			else
			{
				Monitor.Exit(this.stateSyncObj);
			}
		}

		private void RestartWithDelayAndStartListeningAsync()
		{
			lock (this.startCycleCounterSyncObj)
				this.startCycleCounter = 1;

			DelayAndStartListeningAsync();
		}

		private void DelayAndStartListeningAsync()
		{
			var asyncInvoker = new Action(DelayAndStartListeningAsyncCallback);
			asyncInvoker.BeginInvoke(null, null);
		}

		private void DelayAndStartListeningAsyncCallback()
		{
			int randomDelay = SocketBase.Random.Next(MinListenDelay, MaxListenDelay);
			DebugMessage("Delaying listening by random value of {0} ms.", randomDelay);
			Thread.Sleep(randomDelay);

			StartListening();
		}

		private void StartListening()
		{
			// Only continue if still in expected state after the delay. Required because the
			// sequence of the operation of an 'AutoSocket' is not defined. There are several
			// situations where more than one trigger invokes this method, e.g.:
			//  > StartConnecting() fails at Start().
			//  > client_IOError() in case 'Connecting'.
			// The 'AutoSocket' must be able to deal with such situations.

			Monitor.Enter(this.stateSyncObj);                            // Ensure state is handled atomically. Not using
			if (GetStateSynchronized() == SocketState.StartingListening) // lock() for being able to selectively release.
			{
				CreateServerSynchronized(this.localInterface, this.localPort);
				Monitor.Exit(this.stateSyncObj);

				bool isStarting = false;
				try
				{
					isStarting = this.server.Start(); // Will be started asynchronously but may notify! Thus outside lock!
				}
				catch (System.Net.Sockets.SocketException)
				{
					// A 'SocketException' can occur in the call stack shown below, in case another
					// AutoSocket or server tries to bind the given end point at the same time. In
					// such case, ignore the exception and try again later. This procedure ensures
					// that multiple AutoSockets eventually get connected to each other.
					//
					// Stack trace:
					//  > MKY.IO.Serial.Socket.TcpServer.Start() <= Called above
					//  > MKY.IO.Serial.Socket.TcpServer.StartSocketAndThread()
					//  > ALAZ.SystemEx.NetEx.SocketsEx.BaseSocketConnectionHost.Start()
					//  > ALAZ.SystemEx.NetEx.SocketsEx.SocketListener.Start()
					//  > System.Net.Sockets.Socket.Bind(EndPoint localEP)
					//  > System.Net.Sockets.Socket.DoBind(EndPoint endPointSnapshot, SocketAddress socketAddress)
				}
				catch (NullReferenceException)
				{
					// A 'NullReferenceException' can occur in the unlikely case where the server has just gotten disposed of.
				}
				finally
				{
					if (!isStarting)
					{
						DateTime timeStamp;
						bool stateHasChanged;

						lock (this.stateSyncObj) // Ensure state is handled atomically.
						{
							DisposeServerSynchronized(); // Not started at all, no need for two-stage stop-dispose.

							stateHasChanged = SetStateSynchronized(SocketState.StartingConnecting, out timeStamp, notify: false); // Notify outside lock!
						}

						if (stateHasChanged)
							NotifyStateHasChanged(timeStamp); // Notify outside lock!

						// Continue by trying again as client:
						DelayAndStartConnectingIfCyclesPermit(); // Invoke outside lock!
					}
				}
			}
			else
			{
				Monitor.Exit(this.stateSyncObj);
			}
		}

		private void DelayAndStartConnectingIfCyclesPermit()
		{
			bool cyclesPermit = false;
			lock (this.startCycleCounterSyncObj)
			{
				this.startCycleCounter++;
				if (this.startCycleCounter <= MaxStartCycles)
				{
					DebugMessage("Trying cycle #{0}.", this.startCycleCounter);
					cyclesPermit = true;
				}
			}

			if (cyclesPermit)
			{
				DelayAndStartConnectingAsync();
			}
			else
			{
				string message = "AutoSocket could neither be started as client nor server.";

				AutoSocketError
				(
					ErrorSeverity.Severe,
					message
				);
			}
		}

		private void StopAutoSocket()
		{
			// 1st step:
			{
				bool stateHasChanged;
				DateTime timeStamp;

				lock (this.stateSyncObj) // Ensure state is handled atomically.
				{
					if (!IsStarted)
					{
						DebugMessage("Stop() requested but state already is " + GetStateSynchronized() + ".");
						return;
					}

					stateHasChanged = SetStateSynchronized(SocketState.Stopping, out timeStamp, notify: false); // Notify outside lock!
				}

				if (stateHasChanged)
					NotifyStateHasChanged(timeStamp); // Notify outside lock!
			}

			// 2nd step:
			Monitor.Enter(this.stateSyncObj);                   // Ensure state is handled atomically. Not using
			if (GetStateSynchronized() == SocketState.Stopping) // lock() for being able to selectively release.
			{
				StopSocketSyncAndDisposeSynchronized(); // Opposed to the ALAZ sockets, the MKY sockets stop asynchronously.

				DateTime timeStamp;
				var stateHasChanged = SetStateSynchronized(SocketState.Reset, out timeStamp, notify: false); // Notify outside lock!
				Monitor.Exit(this.stateSyncObj);
				if (stateHasChanged)
					NotifyStateHasChanged(timeStamp); // Notify outside lock!
			}
			else
			{
				DebugMessage("Stop() requested but state already is " + GetStateSynchronized() + ".");
				Monitor.Exit(this.stateSyncObj);
			}
		}

		private void AutoSocketError(ErrorSeverity severity, string message)
		{
			DebugMessage(severity + " error in AutoSocket: " + Environment.NewLine + message);

			bool stateHasChanged;
			DateTime timeStamp;

			lock (this.stateSyncObj)
			{
				StopSocketSyncAndDisposeSynchronized(); // Opposed to the ALAZ sockets, the MKY sockets stop asynchronously.

				stateHasChanged = SetStateSynchronized(SocketState.Error, out timeStamp, notify: false); // Notify outside lock!
			}

			if (stateHasChanged)
				NotifyStateHasChanged(timeStamp); // Notify outside lock!

			OnIOError(new IOErrorEventArgs(severity, message));
		}

		#endregion

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnIOChanged(EventArgs<DateTime> e)
		{
			if (IsUndisposed) // Ensure to not propagate event during closing anymore. This may happen on an async System.Net.Sockets.SocketAsyncEventArgs.CompletionPortCallback.
				this.eventHelper.RaiseSync(IOChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOControlChanged(EventArgs<DateTime> e)
		{
			UnusedEvent.PreventCompilerWarning(IOControlChanged, "See exception message below.");
			throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The event 'IOControlChanged' is not in use for TCP/IP AutoSockets!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary></summary>
		protected virtual void OnIOWarning(IOWarningEventArgs e)
		{
			if (IsUndisposed) // Ensure to not propagate event during closing anymore. This may happen on an async System.Net.Sockets.SocketAsyncEventArgs.CompletionPortCallback.
				this.eventHelper.RaiseSync<IOWarningEventArgs>(IOWarning, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOError(IOErrorEventArgs e)
		{
			if (IsUndisposed) // Ensure to not propagate event during closing anymore. This may happen on an async System.Net.Sockets.SocketAsyncEventArgs.CompletionPortCallback.
				this.eventHelper.RaiseSync<IOErrorEventArgs>(IOError, this, e);
		}

		/// <summary></summary>
		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true)]
		protected virtual void OnDataReceived(DataReceivedEventArgs e)
		{
			if (IsUndisposed && IsOpen) // Make sure to propagate event only if active.
				this.eventHelper.RaiseSync<DataReceivedEventArgs>(DataReceived, this, e);
		}

		/// <summary></summary>
		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true)]
		protected virtual void OnDataSent(DataSentEventArgs e)
		{
			if (IsUndisposed && IsOpen) // Make sure to propagate event only if active.
				this.eventHelper.RaiseSync<DataSentEventArgs>(DataSent, this, e);
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
			// AssertUndisposed() shall not be called from such basic method! Its return value may be needed for debugging. All underlying fields are still valid after disposal.

			return (ToShortEndPointString());
		}

		/// <remarks>
		/// Named according to .NET <see cref="System.Net.IPEndPoint"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "EndPoint", Justification = "Naming according to System.Net.EndPoint.")]
		public virtual string ToShortEndPointString()
		{
			// AssertUndisposed() shall not be called from such basic method! Its return value is needed for debugging! All underlying fields are still valid after disposal.

			return ("Server:" + LocalPort + " / " + RemoteHostEndpointString + ":" + RemotePort);
		}

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		/// <summary></summary>
		[Conditional("DEBUG")]
		protected void DebugMessage(string format, params object[] args)
		{
			DebugMessage(string.Format(CultureInfo.CurrentCulture, format, args));
		}

		/// <remarks>
		/// Name "DebugWriteLine" would show relation to <see cref="Debug.WriteLine(string)"/>.
		/// However, named "Message" for compactness and more clarity that something will happen
		/// with <paramref name="message"/>, and rather than e.g. "Common" for comprehensibility.
		/// </remarks>
		[Conditional("DEBUG")]
		protected virtual void DebugMessage(string message)
		{
			Debug.WriteLine
			(
				string.Format
				(
					CultureInfo.CurrentCulture,
					" @ {0} @ Thread #{1} : {2,36} {3,3} {4,-38} : {5}",
					DateTime.Now.ToString("HH:mm:ss.fff", DateTimeFormatInfo.CurrentInfo),
					Thread.CurrentThread.ManagedThreadId.ToString("D3", CultureInfo.CurrentCulture),
					GetType(),
					"#" + this.instanceId.ToString("D2", CultureInfo.CurrentCulture),
					"[" + ToShortEndPointString() + "]",
					message
				)
			);
		}

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
		[Conditional("DEBUG_STATE")]
		private void DebugState(string format, params object[] args)
		{
			DebugMessage(format, args);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
