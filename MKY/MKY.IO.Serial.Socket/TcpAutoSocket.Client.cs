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
////#define DEBUG_STATIC_SOCKET_LIST // Attention: Must also be activated in TcpAutoSocket.[Server].cs !!

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
	/// This partial class implements the client part of <see cref="TcpAutoSocket"/>.
	/// </remarks>
	public partial class TcpAutoSocket
	{
		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		private void CreateClientSynchronized(IPHostEx remoteHost, int remotePort, IPNetworkInterfaceEx localInterface)
		{
			lock (this.stateSyncObj)
			{
				DisposeClientSynchronized();

				this.client = new TcpClient(this.instanceId, remoteHost, remotePort, localInterface);

			#if DEBUG_STATIC_SOCKET_LIST
				staticSocketList.Add(this.client);
			#endif

				this.client.IOChanged    += client_IOChanged;
				this.client.IOWarning    += client_IOWarning;
				this.client.IOError      += client_IOError;
				this.client.DataReceived += client_DataReceived;
				this.client.DataSent     += client_DataSent;
			}
		}

		private void DisposeClientSynchronized()
		{
			lock (this.stateSyncObj)
			{
				if (this.client != null)
				{
					this.client.IOChanged    -= client_IOChanged;
					this.client.IOWarning    -= client_IOWarning;
					this.client.IOError      -= client_IOError;
					this.client.DataReceived -= client_DataReceived;
					this.client.DataSent     -= client_DataSent;

					this.client.Dispose();
					this.client = null;
				}
			}
		}

		#endregion

		#region Event Handling
		//==========================================================================================
		// Event Handling
		//==========================================================================================

		private void client_IOChanged(object sender, EventArgs<DateTime> e)
		{
			if (IsInDisposal) // Ensure to not handle event during closing anymore.
				return;

			Monitor.Enter(this.stateSyncObj); // Ensure state is handled atomically. Not using
			switch (GetStateSynchronized())   // lock() for being able to selectively release.
			{
				case SocketState.Connecting:
				{
					bool isConnected = false;
					if (this.client != null)
						isConnected = this.client.IsConnected;

					// If I/O changed during connecting, change state if connected:

					if (isConnected)
					{
						DateTime timeStamp;
						var stateHasChanged = SetStateSynchronized(SocketState.Connected, out timeStamp, notify: false); // Notify outside lock!
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

				case SocketState.Connected:
				{
					bool isConnected = false;
					if (this.client != null)
						isConnected = this.client.IsConnected;

					// If I/O changed during client operation, simply forward or change state if disconnected:

					if (isConnected)
					{
						Monitor.Exit(this.stateSyncObj);

						OnIOChanged(e); // Raise outside lock!
					}
					else
					{
						DisposeClientSynchronized();

						DateTime timeStamp;
						var stateHasChanged = SetStateSynchronized(SocketState.StartingListening, out timeStamp, notify: false); // Notify outside lock!
						Monitor.Exit(this.stateSyncObj);
						if (stateHasChanged)
							NotifyStateHasChanged(timeStamp); // Notify outside lock!

						// Try to restart as server:
						StartListening(); // Invoke outside lock!
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

		private void client_IOWarning(object sender, IOWarningEventArgs e)
		{
			if (IsUndisposed && IsClient) // Check disposal state first!
				OnIOWarning(e);
		}

		private void client_IOError(object sender, IOErrorEventArgs e)
		{
			if (IsInDisposal) // Ensure to not handle event during closing anymore.
				return;

			Monitor.Enter(this.stateSyncObj); // Ensure state is handled atomically. Not using
			switch (GetStateSynchronized())   // lock() for being able to selectively release.
			{
				case SocketState.Connecting:
				{
					DisposeClientSynchronized();

					DateTime timeStamp;
					var stateHasChanged = SetStateSynchronized(SocketState.StartingListening, out timeStamp, notify: false); // Notify outside lock!
					Monitor.Exit(this.stateSyncObj);
					if (stateHasChanged)
						NotifyStateHasChanged(timeStamp); // Notify outside lock!

					// Try to restart as server:
					StartListening(); // Invoke outside lock!
					break;
				}

				case SocketState.Connected:
				{
					if (e.Severity == ErrorSeverity.Acceptable)
					{
						DisposeClientSynchronized();

						DateTime timeStamp;
						var stateHasChanged = SetStateSynchronized(SocketState.StartingConnecting, out timeStamp, notify: false); // Notify outside lock!
						Monitor.Exit(this.stateSyncObj);
						if (stateHasChanged)
							NotifyStateHasChanged(timeStamp); // Notify outside lock!

						// Restart AutoSocket:
						StartConnecting(); // Invoke outside lock!
					}
					else
					{
						Monitor.Exit(this.stateSyncObj);

						OnIOError(e); // Raise outside lock!
					}

					break;
				}

				default:
				{
					Monitor.Exit(this.stateSyncObj);

					OnIOError(e); // Raise outside lock!

					break;
				}
			}
		}

		private void client_DataReceived(object sender, DataReceivedEventArgs e)
		{
			if (IsUndisposed && IsClient) // Check disposal state first!
				OnDataReceived(e);
		}

		private void client_DataSent(object sender, DataSentEventArgs e)
		{
			if (IsUndisposed && IsClient) // Check disposal state first!
				OnDataSent(e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
