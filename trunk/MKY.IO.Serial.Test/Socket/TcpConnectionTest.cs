//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

using NUnit.Framework;

using MKY.IO.Serial;

namespace MKY.IO.Serial.Test.Socket
{
	/// <summary></summary>
	[TestFixture]
	public class TcpConnectionTest
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const int Interval = 100;
		private const int Timeout = 10000;

		#endregion

		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		#region Tests > ClientShutdown()
		//------------------------------------------------------------------------------------------
		// Tests > ClientShutdown()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestClientShutdown()
		{
			int serverPort;
			TcpServer server;
			TcpClient client;
			
			StartServer(out serverPort, out server);
			Utilities.WaitForStart(server, "TCP server could not be started");
			StartClientAndConnect(out client, serverPort);
			Utilities.WaitForStart(client, "TCP client could not be started");
			Utilities.WaitForConnect(client, server, "TCP client could not be connected to server");

			StopClient(client);
			Utilities.WaitForDisconnect(client, server, "TCP client is not disconnected");
			Utilities.WaitForStop(client, "TCP client could not be stopped");
			StopServer(server);
			Utilities.WaitForStop(server, "TCP server could not be stopped");
		}

		/// <summary></summary>
		[Test]
		public virtual void TestServerShutdown()
		{
			int serverPort;
			TcpServer server;
			TcpClient client;

			StartServer(out serverPort, out server);
			Utilities.WaitForStart(server, "TCP server could not be started");
			StartClientAndConnect(out client, serverPort);
			Utilities.WaitForStart(client, "TCP client could not be started");
			Utilities.WaitForConnect(client, server, "TCP client could not be connected to server");

			StopServer(server);
			Utilities.WaitForDisconnect(client, server, "TCP client is not disconnected");
			Utilities.WaitForStop(server, "TCP server could not be stopped");
			StopClient(client);
			Utilities.WaitForStop(client, "TCP client could not be stopped");
		}

		#endregion

		#endregion

		#region Private Properties
		//==========================================================================================
		// Private Properties
		//==========================================================================================

		int AvailableLocalTcpPort
		{
			get
			{
				IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
				IPEndPoint[] listeners = properties.GetActiveTcpListeners();

				int port;
				for (port = 10000; port <= 65535; port++)
				{
					bool found = false;
					foreach (IPEndPoint ep in listeners)
					{
						if (ep.Port == port)
						{
							found = true;
							break;
						}
					}

					if (!found)
						return (port);
				}

				throw (new OverflowException("No local TCP port available within range of 10000 through 65535"));
			}
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void StartServer(out int serverPort, out TcpServer server)
		{
			// Create server and initiate asych start
			serverPort = AvailableLocalTcpPort;
			server = new TcpServer(IPAddress.Loopback, serverPort);
			if (!server.Start())
				Assert.Fail("TCP server could not be started");
		}

		private void StartClientAndConnect(out TcpClient client, int serverPort)
		{
			// Create client and initiate asych start
			client = new TcpClient(IPAddress.Loopback, serverPort);
			if (!client.Start())
				Assert.Fail("TCP client could not be started");
		}

		private void StopServer(TcpServer server)
		{
			// Initiate async stop
			server.Stop();
		}

		private void StopClient(TcpClient client)
		{
			// Initiate async stop
			client.Stop();
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
