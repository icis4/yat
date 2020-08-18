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
// Copyright © 2007-2020 Matthias Kläy.
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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
//// 'System.Net' as well as 'ALAZ.SystemEx.NetEx' are explicitly used for more obvious distinction.
//// 'System.Net.Sockets' including.
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
			Starting,
			Connecting,
			Connected,
			ConnectingFailed,
			StartingListening,
			Listening,
			ListeningFailed,
			Accepted,
		////Restarting,
			Stopping,
			Error
		}

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const int MaxStartCycles = 3;

		private const int MinConnectDelay = 50;
		private const int MaxConnectDelay = 300;

		private const int MinListenDelay = 50;
		private const int MaxListenDelay = 300;

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

		private int startCycleCounter; // = 0;
		private object startCycleCounterSyncObj = new object();

		private TcpClient client;
		private TcpServer server;
		private object socketSyncObj = new object();

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
			// Verify by-reference arguments:

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
			this.eventHelper.DiscardAllEventsAndExceptions();

			DebugMessage("Disposing...");

			// Dispose of managed resources:
			if (disposing)
			{
				// In the 'normal' case, the items have already been disposed of, e.g. in Stop().
				DisposeSockets();
			}

			DebugMessage("...successfully disposed.");
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

				lock (this.socketSyncObj)
				{
					if (IsClient && (this.client != null))
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
					DebugMessage("Start() requested but state is already " + state + ".");
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
					DebugMessage("Stop() requested but state is " + state + ".");
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
				lock (this.socketSyncObj)
				{
					if (IsClient && (this.client != null))
						return (this.client.Send(data));
					else if (IsServer && (this.server != null))
						return (this.server.Send(data));
				}
			}

			return (false);
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

		private void SetStateSynchronizedAndNotify(SocketState state)
		{
			SocketState oldState;

			lock (this.stateSyncObj)
			{
				oldState = this.state;
				this.state = state;
			}

		#if (DEBUG)
			string isClientOrServerString;
			if      (IsClient && IsConnected) // 'Doppel-moppel', but keep it as a check during development and debugging
				isClientOrServerString = "is connected as client";
			else if (IsServer && IsConnected)
				isClientOrServerString = "is connected as server";
			else if (IsServer && !IsConnected)
				isClientOrServerString = "is server and listening";
			else
				isClientOrServerString = "is neither client nor server";

			if (state != oldState)
				DebugMessage("State has changed from " + oldState + " to " + state + ", " + isClientOrServerString + ".");
			else
				DebugMessage("State is already " + oldState + ".");
		#endif

			if (state != oldState)
				OnIOChanged(new EventArgs<DateTime>(DateTime.Now));
		}

		#endregion

		#region Socket Methods
		//==========================================================================================
		// Socket Methods
		//==========================================================================================

		/// <remarks>
		/// Note that the underlying sockets perform stopping synchronously, opposed to starting
		/// that is done asynchronously. This difference is due to the issue described in the header
		/// of this class.
		/// </remarks>
		private void StopAndDisposeSockets()
		{
			StopSockets();
			DisposeSockets();
		}

		/// <remarks>
		/// Note that the underlying sockets perform stopping synchronously, opposed to starting
		/// that is done asynchronously. This difference is due to the issue described in the header
		/// of this class.
		/// </remarks>
		private void StopSockets()
		{
			DebugMessage("Stopping sockets...");

			lock (this.socketSyncObj)
			{
				if (this.client != null)
					this.client.Stop();

				if (this.server != null)
					this.server.Stop();
			}

			DebugMessage("...sucessfully stopped.");
		}

		private void DisposeSockets()
		{
			DisposeClient();
			DisposeServer();
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

			SetStateSynchronizedAndNotify(SocketState.Starting);
			StartConnecting();
		}

		/// <summary>
		/// Try to start as client.
		/// </summary>
		private void StartConnecting()
		{
			int randomDelay = SocketBase.Random.Next(MinConnectDelay, MaxConnectDelay);
			DebugMessage("Delaying connecting by random value of " + randomDelay + " ms.");
			Thread.Sleep(randomDelay);

			// Only continue if socket is still up and running after the delay! Required because
			// re-connecting happens automatically when connection gets lost. If two AutoSockets
			// are interconnected, and both are shut down, the sequence of the operation is not
			// defined. It is likely that Stop() is called while the thread is delayed above. In
			// such case, neither a client nor a server shall be created nor started.
			if (IsUndisposed && IsStarted) // Check disposal state first!
			{
				SetStateSynchronizedAndNotify(SocketState.Connecting);
				CreateClient(this.remoteHost, this.remotePort, this.localInterface);

				bool startIsOngoing = false;
				try
				{
					lock (this.socketSyncObj)
					{
						if (this.client != null)
							startIsOngoing = this.client.Start();
					}
				}
				finally
				{
					if (!startIsOngoing)
					{
						DisposeClient();
						StartListening();
					}
				}
			}
		}

		/// <summary>
		/// Try to start as server.
		/// </summary>
		private void StartListening()
		{
			int randomDelay = SocketBase.Random.Next(MinListenDelay, MaxListenDelay);
			DebugMessage("Delaying listening by random value of " + randomDelay + " ms.");
			Thread.Sleep(randomDelay);

			// Only continue if socket is still up and running after the delay! Required because
			// re-connecting happens automatically when connection gets lost. If two AutoSockets
			// are interconnected, and both are shut down, the sequence of the operation is not
			// defined. It is likely that Stop() is called while the thread is delayed above. In
			// such case, neither a client nor a server shall be created nor started.
			if (IsUndisposed && IsStarted) // Check disposal state first!
			{
				SetStateSynchronizedAndNotify(SocketState.StartingListening);
				CreateServer(this.localInterface, this.localPort);

				bool startIsOngoing = false;
				try
				{
					lock (this.socketSyncObj)
					{
						if (this.server != null)
							startIsOngoing = this.server.Start(); // Server will be started asynchronously.
					}
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
				finally
				{
					if (!startIsOngoing)
					{
						DisposeServer();
						RequestTryAgain();
					}
				}
			}
		}

		private void RequestTryAgain()
		{
			bool tryAgain = false;
			lock (this.startCycleCounterSyncObj)
			{
				this.startCycleCounter++;
				if (this.startCycleCounter <= MaxStartCycles)
					tryAgain = true;
			}

			if (tryAgain)
			{
				DebugMessage("Trying connect cycle #" + this.startCycleCounter + ".");

				StartConnecting();
			}
			else
			{
				string message =
					"AutoSocket could neither be started as client nor server," + Environment.NewLine +
					"TCP/IP address/port is not available.";

				AutoSocketError
				(
					ErrorSeverity.Severe,
					message
				);
			}
		}

	////private void RestartAutoSocket()
	////{
	////	SetStateSynchronizedAndNotify(SocketState.Restarting);
	////	StopAndDisposeSockets();
	////	StartAutoSocket();
	////}

		private void StopAutoSocket()
		{
			SetStateSynchronizedAndNotify(SocketState.Stopping);
			StopAndDisposeSockets();
			SetStateSynchronizedAndNotify(SocketState.Reset);
		}

		private void AutoSocketError(ErrorSeverity severity, string message)
		{
			DebugMessage(severity + " error in AutoSocket: " + Environment.NewLine + message);

			DisposeSockets();

			SetStateSynchronizedAndNotify(SocketState.Error);
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

			var remoteHostEndpoint = ((this.remoteHost != null) ? (this.remoteHost.ToEndpointAddressString()) : "[none]"); // Required to always be available.
			return ("Server:" + this.localPort + " / " + remoteHostEndpoint + ":" + this.remotePort);
		}

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

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

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
