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

#region Configuration
//==================================================================================================
// Configuration
//==================================================================================================

#if (DEBUG)

	// Enable debugging of a static list of all sockets:
////#define DEBUG_STATIC_SOCKET_LIST // Attention: Must also be activated in TcpAutoSocket.[Client].cs !!

#endif // DEBUG

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
//// 'System.Net' as well as 'ALAZ.SystemEx.NetEx' are explicitly used for more obvious distinction.
//// 'System.Net.Sockets' including.
using System.Threading;

using MKY.Net;

#endregion

namespace MKY.IO.Serial.Socket
{
	/// <remarks>
	/// This partial class implements the server part of <see cref="TcpAutoSocket"/>.
	/// </remarks>
	public partial class TcpAutoSocket
	{
		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		private void CreateServerSynchronized(IPNetworkInterfaceEx localInterface, int localPort)
		{
			lock (this.stateSyncObj)
			{
				this.server = new TcpServer(this.instanceId, localInterface, localPort);

			#if (DEBUG_STATIC_SOCKET_LIST)
				staticSocketList.Add(this.server);
			#endif

				AttachServerSynchronized();
			}
		}

		private void AttachServerSynchronized()
		{
			lock (this.stateSyncObj)
			{
				if (this.server != null)
				{
					this.server.IOChanged    += server_IOChanged;
					this.server.IOWarning    += server_IOWarning;
					this.server.IOError      += server_IOError;
					this.server.DataReceived += server_DataReceived;
					this.server.DataSent     += server_DataSent;
				}
			}
		}

		private void DetachServerSynchronized()
		{
			lock (this.stateSyncObj)
			{
				if (this.server != null)
				{
					this.server.IOChanged    -= server_IOChanged;
					this.server.IOWarning    -= server_IOWarning;
					this.server.IOError      -= server_IOError;
					this.server.DataReceived -= server_DataReceived;
					this.server.DataSent     -= server_DataSent;
				}
			}
		}

		private void DisposeServerSynchronized()
		{
			lock (this.stateSyncObj)
			{
				if (this.server != null)
				{
					DetachServerSynchronized();

					this.server.Dispose();
					this.server = null;
				}
			}
		}

		/// <remarks>
		/// Opposed to the ALAZ sockets, the MKY sockets stop asynchronously. However, ALAZ sockets
		/// sometimes deadlock on Stop(). Therefore, this method waits until the server indeed is
		/// stopped and then disposes of it. A timeout in the MKY sockets guarantee stopping.
		/// </remarks>
		private void StopServerSyncAndDisposeSynchronized()
		{
			lock (this.stateSyncObj)
			{
				if (this.server != null)
				{
					DetachServerSynchronized(); // Stop() may invoke event callbacks, potentially leading to a deadlock!

					if (this.server.IsStarted)
						this.server.Stop();

					int waitTime = 0;
					while (!this.server.IsStopped)
					{
						Thread.Sleep(SocketStopInterval);
						waitTime += SocketStopInterval;

						if (waitTime >= SocketStopTimeout)
							break;
					}

					this.server.Dispose();
					this.server = null;
				}
			}
		}

		#endregion

		#region Event Handling
		//==========================================================================================
		// Event Handling
		//==========================================================================================

		private void server_IOChanged(object sender, EventArgs<DateTime> e)
		{
			if (IsInDisposal) // Ensure to not handle event during closing anymore.
				return;

			Monitor.Enter(this.stateSyncObj); // Ensure state is handled atomically. Not using
			switch (GetStateSynchronized())   // lock() for being able to selectively release.
			{
				case SocketState.StartingListening:
				{
					bool isListening = false;
					if (this.server != null)
						isListening = this.server.IsListening;

					// If I/O changed during starting, change state if started:

					if (isListening)
					{
						DateTime timeStamp;
						var stateHasChanged = SetStateSynchronized(SocketState.Listening, out timeStamp, notify: false); // Notify outside lock!
						Monitor.Exit(this.stateSyncObj);
						if (stateHasChanged)
							NotifyStateHasChanged(timeStamp); // Notify outside lock!
					}
					else
					{
						Monitor.Exit(this.stateSyncObj);
					}

					break;
				}

				case SocketState.Listening:
				{
					bool isConnected = false;
					if (this.server != null)
						isConnected = this.server.IsConnected;

					// If I/O changed during listening, change state if connected:

					if (isConnected)
					{
						DateTime timeStamp;
						var stateHasChanged = SetStateSynchronized(SocketState.Accepted, out timeStamp, notify: false); // Notify outside lock!
						Monitor.Exit(this.stateSyncObj);
						if (stateHasChanged)
							NotifyStateHasChanged(timeStamp); // Notify outside lock!
					}
					else
					{
						Monitor.Exit(this.stateSyncObj);

						// Note that state change on error is handled further below.
					}

					break;
				}

				case SocketState.Accepted:
				{
					bool isConnected = false;
					if (this.server != null)
						isConnected = this.server.IsConnected;

					// If I/O changed during server operation, simply forward or change state if disconnected:

					if (isConnected)
					{
						Monitor.Exit(this.stateSyncObj);

						OnIOChanged(e); // Raise outside lock!
					}
					else
					{
						DateTime timeStamp;
						var stateHasChanged = SetStateSynchronized(SocketState.Listening, out timeStamp, notify: false); // Notify outside lock!
						Monitor.Exit(this.stateSyncObj);
						if (stateHasChanged)
							NotifyStateHasChanged(timeStamp); // Notify outside lock!

						// Note that state change on error is handled further below.
					}

					break;
				}

				default:
				{
					Monitor.Exit(this.stateSyncObj);

					break;
				}
			}
		}

		private void server_IOWarning(object sender, IOWarningEventArgs e)
		{
			if (IsUndisposed && IsServer) // Check disposal state first!
				OnIOWarning(e);
		}

		private void server_IOError(object sender, IOErrorEventArgs e)
		{
			if (IsInDisposal) // Ensure to not handle event during closing anymore.
				return;

			Monitor.Enter(this.stateSyncObj); // Ensure state is handled atomically. Not using
			switch (GetStateSynchronized())   // lock() for being able to selectively release.
			{
				case SocketState.StartingListening:
				{                                           // Even though yet 'StartingListening', the underlying socket may already have started,
					StopServerSyncAndDisposeSynchronized(); // thus always doing two-stage stop-dispose for reason described in remarks of method.

					DateTime timeStamp;
					var stateHasChanged = SetStateSynchronized(SocketState.StartingConnecting, out timeStamp, notify: false); // Notify outside lock!
					Monitor.Exit(this.stateSyncObj);
					if (stateHasChanged)
						NotifyStateHasChanged(timeStamp); // Notify outside lock!

					// Continue by trying again as client:
					DelayAndStartConnectingIfCyclesPermit(); // Invoke outside lock!

					break;
				}

				case SocketState.Listening:
				case SocketState.Accepted:
				{
					StopServerSyncAndDisposeSynchronized(); // Doing two-stage stop-dispose for reason described in remarks of method.

					DateTime timeStamp;
					var stateHasChanged = SetStateSynchronized(SocketState.StartingConnecting, out timeStamp, notify: false); // Notify outside lock!
					Monitor.Exit(this.stateSyncObj);
					if (stateHasChanged)
						NotifyStateHasChanged(timeStamp); // Notify outside lock!

					// Restart AutoSocket, first try as client:
					RestartWithDelayAndStartConnectingAsync(); // Invoke outside lock!

					break;
				}

				default: // incl. e.g. SocketState.Stopping:
				{
					Monitor.Exit(this.stateSyncObj);

					OnIOError(e); // Raise outside lock!

					break;
				}
			}
		}

		private void server_DataReceived(object sender, DataReceivedEventArgs e)
		{
			if (IsUndisposed && IsServer) // Check disposal state first!
				OnDataReceived(e);
		}

		private void server_DataSent(object sender, DataSentEventArgs e)
		{
			if (IsUndisposed && IsServer) // Check disposal state first!
				OnDataSent(e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
