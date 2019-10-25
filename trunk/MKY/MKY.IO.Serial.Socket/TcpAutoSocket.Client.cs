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
// Copyright © 2007-2019 Matthias Kläy.
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
//// 'System.Net' as well as 'ALAZ.SystemEx.NetEx' are explicitly used for more obvious distinction.

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

		private void CreateClient(IPHostEx remoteHost, int remotePort, IPNetworkInterfaceEx localInterface)
		{
			DisposeClient();

			lock (this.socketSyncObj)
			{
				this.client = new TcpClient(this.instanceId, remoteHost, remotePort, localInterface);

				this.client.IOChanged    += client_IOChanged;
				this.client.IOError      += client_IOError;
				this.client.DataReceived += client_DataReceived;
				this.client.DataSent     += client_DataSent;
			}
		}

		private void DisposeClient()
		{
			lock (this.socketSyncObj)
			{
				if (this.client != null)
				{
					this.client.IOChanged    -= client_IOChanged;
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

		private void client_IOChanged(object sender, EventArgs e)
		{
			switch (GetStateSynchronized())
			{
				case SocketState.Connecting:
				{
					bool isConnected = false;
					lock (this.socketSyncObj)
					{
						if (this.client != null)
							isConnected = this.client.IsConnected;
					}

					if (isConnected)                  // If I/O changed during startup,
					{                                 //   check for connected and change state.
						SetStateSynchronizedAndNotify(SocketState.Connected);
					}

					break;
				}
				case SocketState.Connected:
				{
					bool isConnected = false;
					lock (this.socketSyncObj)
					{
						if (this.client != null)
							isConnected = this.client.IsConnected;
					}

					if (isConnected)                  // If I/O changed during client operation
					{                                 //   and client is connected to a server,
						OnIOChanged(e);               //   simply forward the event.
					}
					else
					{
						DisposeClient();              // If client lost connection to server,
						StartListening();             //   change to server operation.
					}

					break;
				}
			}
		}

		private void client_IOError(object sender, IOErrorEventArgs e)
		{
			switch (GetStateSynchronized())
			{
				case SocketState.Connecting:
				case SocketState.ConnectingFailed:
				{
					DisposeClient();                // In case of error during startup,
					StartListening();               //   try to start as server.
					break;
				}

				case SocketState.Connected:
				{
					if (e.Severity == ErrorSeverity.Acceptable)
					{
						DisposeClient();            // In case of error during client operation,
						StartConnecting();          //   restart AutoSocket.
					}
					else
					{
						OnIOError(e);
					}
					break;
				}

				default:
				{
					OnIOError(e);
					break;
				}
			}
		}

		private void client_DataReceived(object sender, DataReceivedEventArgs e)
		{
			if (!IsDisposed && IsClient) // Check 'IsDisposed' first!
				OnDataReceived(e);
		}

		private void client_DataSent(object sender, DataSentEventArgs e)
		{
			if (!IsDisposed && IsClient) // Check 'IsDisposed' first!
				OnDataSent(e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
