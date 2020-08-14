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
//// 'System.Net' as well as 'ALAZ.SystemEx.NetEx' are explicitly used for more obvious distinction.
//// 'System.Net.Sockets' including.

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

		private void CreateServer(IPNetworkInterfaceEx localInterface, int localPort)
		{
			DisposeServer();

			lock (this.socketSyncObj)
			{
				this.server = new TcpServer(this.instanceId, localInterface, localPort);

				this.server.IOChanged    += server_IOChanged;
				this.server.IOWarning    += server_IOWarning;
				this.server.IOError      += server_IOError;
				this.server.DataReceived += server_DataReceived;
				this.server.DataSent     += server_DataSent;
			}
		}

		private void DisposeServer()
		{
			lock (this.socketSyncObj)
			{
				if (this.server != null)
				{
					this.server.IOChanged    -= server_IOChanged;
					this.server.IOWarning    -= server_IOWarning;
					this.server.IOError      -= server_IOError;
					this.server.DataReceived -= server_DataReceived;
					this.server.DataSent     -= server_DataSent;

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
			switch (GetStateSynchronized())
			{
				case SocketState.StartingListening:
				{
					bool isStarted = false;
					lock (this.socketSyncObj)
					{
						if (this.server != null)
							isStarted = this.server.IsStarted;
					}

					if (isStarted)                    // If I/O changed during startup,
					{                                 //   check for start and change state.
						SetStateSynchronizedAndNotify(SocketState.Listening);
					}

					break;
				}
				case SocketState.Listening:
				{
					int connectedClientCount = 0;
					lock (this.socketSyncObj)
					{
						if (this.server != null)
							connectedClientCount = this.server.ConnectedClientCount;
					}

					if (connectedClientCount > 0)     // If I/O changed during listening, change state
					{                                 //   to accepted if clients are connected.
						SetStateSynchronizedAndNotify(SocketState.Accepted);
					}

					break;
				}
				case SocketState.Accepted:
				{
					int connectedClientCount = 0;
					lock (this.socketSyncObj)
					{
						if (this.server != null)
							connectedClientCount = this.server.ConnectedClientCount;
					}

					if (connectedClientCount <= 0)    // If I/O changed during accepted, change state
					{                                 //   to listening if no clients are connected.
						SetStateSynchronizedAndNotify(SocketState.Listening);
					}

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
			switch (GetStateSynchronized())
			{
				case SocketState.StartingListening: // In case of error during startup,
				{                                   //   increment start cycles and
					RequestTryAgain();              //   continue depending on count
					break;
				}

				case SocketState.Listening:
				case SocketState.Accepted:
				{
					if (e.Severity == ErrorSeverity.Acceptable)
					{
						DisposeServer();            // In case of error during server operation,
						StartConnecting();          //   restart AutoSocket
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
