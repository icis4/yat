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
// MKY Version 1.0.28 Development
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

using System.Threading;

using NUnit.Framework;
using NUnitEx;

namespace MKY.IO.Serial.Socket.Test
{
	/// <summary></summary>
	[TestFixture]
	public class SimpleTcpConnectionTest
	{
		#region Private Constants
		//==========================================================================================
		// Private Constants
		//==========================================================================================

		private const int AutoSocketDelay = 1500;

		#endregion

		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		#region Tests > ServerClientConnectAndClientShutdown()
		//------------------------------------------------------------------------------------------
		// Tests > ServerClientConnectAndClientShutdown()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		[StandardDurationCategory.Second1]
		public virtual void TestServerClientConnectAndClientShutdown()
		{
			int serverPort;
			TcpServer server;
			Utilities.CreateAndStartAsync(out server, out serverPort);
			using (server)
			{
				Utilities.WaitForStart(server, "TCP/IP server could not be started!");

				TcpClient client;
				Utilities.CreateAndStartAsync(out client, serverPort);
				using (client)
				{
					Utilities.WaitForStart(client, "TCP/IP client could not be started!");
					Utilities.WaitForConnection(client, server, "TCP/IP client could not be connected to server!");
					Utilities.AssertStartedAndTransmissive(client);
					Utilities.AssertStartedAndTransmissive(server);

					Utilities.StopAsync(client);
					Utilities.WaitForDisconnection(server, client, "TCP/IP sockets are not disconnected!");
					Utilities.WaitForStop(client, "TCP/IP client could not be stopped!");
					Utilities.AssertStopped(client);
					Utilities.StopAsync(server);
					Utilities.WaitForStop(server, "TCP/IP server could not be stopped!");
					Utilities.AssertStopped(server);
				}
			}
		}

		#endregion

		#region Tests > ServerClientConnectAndServerShutdown()
		//------------------------------------------------------------------------------------------
		// Tests > ServerClientConnectAndServerShutdown()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		[StandardDurationCategory.Second1]
		public virtual void TestServerClientConnectAndServerShutdown()
		{
			int serverPort;
			TcpServer server;
			Utilities.CreateAndStartAsync(out server, out serverPort);
			using (server)
			{
				Utilities.WaitForStart(server, "TCP/IP server could not be started!");

				TcpClient client;
				Utilities.CreateAndStartAsync(out client, serverPort);
				using (client)
				{
					Utilities.WaitForStart(client, "TCP/IP client could not be started!");
					Utilities.WaitForConnection(client, server, "TCP/IP client could not be connected to server!");
					Utilities.AssertStartedAndTransmissive(client);
					Utilities.AssertStartedAndTransmissive(server);

					Utilities.StopAsync(server);
					Utilities.WaitForDisconnection(client, server, "TCP/IP sockets are not disconnected!");
					Utilities.WaitForStop(server, "TCP/IP server could not be stopped!");
					Utilities.AssertStopped(server);
					Utilities.StopAsync(client);
					Utilities.WaitForStop(client, "TCP/IP client could not be stopped!");
					Utilities.AssertStopped(client);
				}
			}
		}

		#endregion

		#region Tests > ServerAutoSocketConnectAndAutoSocketShutdown()
		//------------------------------------------------------------------------------------------
		// Tests > ServerAutoSocketConnectAndAutoSocketShutdown()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		[StandardDurationCategory.Second1]
		public virtual void TestServerAutoSocketConnectAndAutoSocketShutdown()
		{
			int serverPort;
			TcpServer server;
			Utilities.CreateAndStartAsync(out server, out serverPort);
			using (server)
			{
				Utilities.WaitForStart(server, "TCP/IP server could not be started!");

				TcpAutoSocket autoSocket;
				Utilities.CreateAndStartAsyncAsClient(out autoSocket, serverPort);
				using (autoSocket)
				{
					Utilities.WaitForStart(autoSocket, "TCP/IP AutoSocket could not be started!");
					Utilities.WaitForConnection(autoSocket, server, "TCP/IP AutoSocket could not be connected to server!");
					Utilities.AssertStartedAndTransmissive(autoSocket);
					Utilities.AssertStartedAndTransmissive(server);

					Utilities.StopAsync(autoSocket);
					Utilities.WaitForDisconnection(server, autoSocket, "TCP/IP sockets are not disconnected!");
					Utilities.WaitForStop(autoSocket, "TCP/IP AutoSocket could not be stopped!");
					Utilities.AssertStopped(autoSocket);
					Utilities.StopAsync(server);
					Utilities.WaitForStop(server, "TCP/IP server could not be stopped!");
					Utilities.AssertStopped(server);
				}
			}
		}

		#endregion

		#region Tests > ServerAutoSocketConnectAndServerShutdown()
		//------------------------------------------------------------------------------------------
		// Tests > ServerAutoSocketConnectAndServerShutdown()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		[StandardDurationCategory.Second1]
		public virtual void TestServerAutoSocketConnectAndServerShutdown()
		{
			int serverPort;
			TcpServer server;
			Utilities.CreateAndStartAsync(out server, out serverPort);
			using (server)
			{
				Utilities.WaitForStart(server, "TCP/IP server could not be started!");

				TcpAutoSocket autoSocket;
				Utilities.CreateAndStartAsyncAsClient(out autoSocket, serverPort);
				using (autoSocket)
				{
					Utilities.WaitForStart(autoSocket, "TCP/IP AutoSocket could not be started!");
					Utilities.WaitForConnection(autoSocket, server, "TCP/IP AutoSocket could not be connected to server!");
					Utilities.AssertStartedAndTransmissive(autoSocket);
					Utilities.AssertStartedAndTransmissive(server);

					Utilities.StopAsync(server);
					Utilities.WaitForDisconnection(autoSocket, server, "TCP/IP sockets are not disconnected!");
					Utilities.WaitForStop(server, "TCP/IP server could not be stopped!");
					Utilities.AssertStopped(server);
					Utilities.StopAsync(autoSocket);
					Utilities.WaitForStop(autoSocket, "TCP/IP AutoSocket could not be stopped!");
					Utilities.AssertStopped(autoSocket);
				}
			}
		}

		#endregion

		#region Tests > AutoSocketClientConnectAndClientShutdown()
		//------------------------------------------------------------------------------------------
		// Tests > AutoSocketClientConnectAndClientShutdown()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		[StandardDurationCategory.Seconds4]
		public virtual void TestAutoSocketClientConnectAndClientShutdown()
		{
			int serverPort;
			TcpAutoSocket autoSocket;
			Utilities.CreateAndStartAsyncAsServer(out autoSocket, out serverPort);
			using (autoSocket)
			{
				Utilities.WaitForStart(autoSocket, "TCP/IP AutoSocket could not be started!");
				Thread.Sleep(AutoSocketDelay); // Wait a while to let AutoSocket become a server.

				TcpClient client;
				Utilities.CreateAndStartAsync(out client, serverPort);
				using (client)
				{
					Utilities.WaitForStart(client, "TCP/IP client could not be started!");
					Utilities.WaitForConnection(client, autoSocket, "TCP/IP client could not be connected to server!");
					Utilities.AssertStartedAndTransmissive(client);
					Utilities.AssertStartedAndTransmissive(autoSocket);

					Utilities.StopAsync(client);
					Utilities.WaitForDisconnection(autoSocket, client, "TCP/IP sockets are not disconnected!");
					Utilities.WaitForStop(client, "TCP/IP client could not be stopped!");
					Utilities.AssertStopped(client);
					Utilities.StopAsync(autoSocket);
					Utilities.WaitForStop(autoSocket, "TCP/IP AutoSocket could not be stopped!");
					Utilities.AssertStopped(autoSocket);
				}
			}
		}

		#endregion

		#region Tests > AutoSocketClientConnectAndAutoSocketShutdown()
		//------------------------------------------------------------------------------------------
		// Tests > AutoSocketClientConnectAndAutoSocketShutdown()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		[StandardDurationCategory.Seconds4]
		public virtual void TestAutoSocketClientConnectAndAutoSocketShutdown()
		{
			int serverPort;
			TcpAutoSocket autoSocket;
			Utilities.CreateAndStartAsyncAsServer(out autoSocket, out serverPort);
			using (autoSocket)
			{
				Utilities.WaitForStart(autoSocket, "TCP/IP AutoSocket could not be started!");
				Thread.Sleep(AutoSocketDelay); // Wait a while to let AutoSocket become a server.

				TcpClient client;
				Utilities.CreateAndStartAsync(out client, serverPort);
				using (client)
				{
					Utilities.WaitForStart(client, "TCP/IP client could not be started!");
					Utilities.WaitForConnection(client, autoSocket, "TCP/IP client could not be connected to server");
					Utilities.AssertStartedAndTransmissive(client);
					Utilities.AssertStartedAndTransmissive(autoSocket);

					Utilities.StopAsync(autoSocket);
					Utilities.WaitForDisconnection(client, autoSocket, "TCP/IP sockets are not disconnected!");
					Utilities.WaitForStop(autoSocket, "TCP/IP AutoSocket could not be stopped!");
					Utilities.AssertStopped(autoSocket);
					Utilities.StopAsync(client);
					Utilities.WaitForStop(client, "TCP/IP client could not be stopped!");
					Utilities.AssertStopped(client);
				}
			}
		}

		#endregion

		#region Tests > AutoSocketAutoSocketConnectAndAutoSocketBShutdown()
		//------------------------------------------------------------------------------------------
		// Tests > AutoSocketAutoSocketConnectAndAutoSocketBShutdown()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		[StandardDurationCategory.Seconds4]
		public virtual void TestAutoSocketAutoSocketConnectAndAutoSocketBShutdown()
		{
			int serverPort;
			TcpAutoSocket autoSocketA;
			Utilities.CreateAndStartAsyncAsServer(out autoSocketA, out serverPort);
			using (autoSocketA)
			{
				Utilities.WaitForStart(autoSocketA, "TCP/IP AutoSocket A could not be started!");

				TcpAutoSocket autoSocketB;
				Utilities.CreateAndStartAsyncAsClient(out autoSocketB, serverPort);
				using (autoSocketB)
				{
					Utilities.WaitForStart(autoSocketB, "TCP/IP AutoSocket B could not be started!");
					Utilities.WaitForConnection(autoSocketB, autoSocketA, "TCP/IP AutoSocket B could not be connected to AutoSocket A!");
					Utilities.AssertStartedAndTransmissive(autoSocketB);
					Utilities.AssertStartedAndTransmissive(autoSocketA);

					Utilities.StopAsync(autoSocketB);
					Utilities.WaitForDisconnection(autoSocketA, autoSocketB, "TCP/IP sockets are not disconnected!");
					Utilities.WaitForStop(autoSocketB, "TCP/IP AutoSocket B could not be stopped!");
					Utilities.AssertStopped(autoSocketB);
					Utilities.StopAsync(autoSocketA);
					Utilities.WaitForStop(autoSocketA, "TCP/IP AutoSocket A could not be stopped!");
					Utilities.AssertStopped(autoSocketA);
				}
			}
		}

		#endregion

		#region Tests > AutoSocketAutoSocketConnectAndAutoSocketAShutdown()
		//------------------------------------------------------------------------------------------
		// Tests > AutoSocketAutoSocketConnectAndAutoSocketAShutdown()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		[StandardDurationCategory.Seconds4]
		public virtual void TestAutoSocketAutoSocketConnectAndAutoSocketAShutdown()
		{
			int serverPort;
			TcpAutoSocket autoSocketA;
			Utilities.CreateAndStartAsyncAsServer(out autoSocketA, out serverPort);
			using (autoSocketA)
			{
				Utilities.WaitForStart(autoSocketA, "TCP/IP AutoSocket A could not be started!");

				TcpAutoSocket autoSocketB;
				Utilities.CreateAndStartAsyncAsClient(out autoSocketB, serverPort);
				using (autoSocketB)
				{
					Utilities.WaitForStart(autoSocketB, "TCP/IP AutoSocket B could not be started!");
					Utilities.WaitForConnection(autoSocketB, autoSocketA, "TCP/IP AutoSocket B could not be connected to AutoSocket A!");
					Utilities.AssertStartedAndTransmissive(autoSocketB);
					Utilities.AssertStartedAndTransmissive(autoSocketA);

					Utilities.StopAsync(autoSocketA);
					Utilities.WaitForDisconnection(autoSocketB, autoSocketA, "TCP/IP sockets are not disconnected!");
					Utilities.WaitForStop(autoSocketA, "TCP/IP AutoSocket A could not be stopped!");
					Utilities.AssertStopped(autoSocketA);
					Utilities.StopAsync(autoSocketB);
					Utilities.WaitForStop(autoSocketB, "TCP/IP AutoSocket B could not be stopped!");
					Utilities.AssertStopped(autoSocketB);
				}
			}
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
