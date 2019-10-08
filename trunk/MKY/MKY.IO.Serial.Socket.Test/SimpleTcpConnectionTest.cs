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
// Copyright © 2007-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Threading;

using NUnit.Framework;

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
		public virtual void TestServerClientConnectAndClientShutdown()
		{
			int serverPort;
			TcpServer server;
			TcpClient client;

			Utilities.StartTcpServer(out server, out serverPort);
			Utilities.WaitForStart(server, "TCP/IP server could not be started!");
			Utilities.StartTcpClient(out client, serverPort);
			Utilities.WaitForStart(client, "TCP/IP client could not be started!");
			Utilities.WaitForConnect(client, server, "TCP/IP client could not be connected to server!");
			Utilities.AssertStartedAndTransmissive(client);
			Utilities.AssertStartedAndTransmissive(server);

			Utilities.StopTcpClient(client);
			Utilities.WaitForDisconnect(server, client, "TCP/IP server is not disconnected!");
			Utilities.WaitForStop(client, "TCP/IP client could not be stopped!");
			Utilities.AssertStopped(client);
			Utilities.StopTcpServer(server);
			Utilities.WaitForStop(server, "TCP/IP server could not be stopped!");
			Utilities.AssertStopped(server);
		}

		#endregion

		#region Tests > ServerClientConnectAndServerShutdown()
		//------------------------------------------------------------------------------------------
		// Tests > ServerClientConnectAndServerShutdown()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestServerClientConnectAndServerShutdown()
		{
			int serverPort;
			TcpServer server;
			TcpClient client;

			Utilities.StartTcpServer(out server, out serverPort);
			Utilities.WaitForStart(server, "TCP/IP server could not be started!");
			Utilities.StartTcpClient(out client, serverPort);
			Utilities.WaitForStart(client, "TCP/IP client could not be started!");
			Utilities.WaitForConnect(client, server, "TCP/IP client could not be connected to server!");
			Utilities.AssertStartedAndTransmissive(client);
			Utilities.AssertStartedAndTransmissive(server);

			Utilities.StopTcpServer(server);
			Utilities.WaitForDisconnect(client, server, "TCP/IP client is not disconnected!");
			Utilities.WaitForStop(server, "TCP/IP server could not be stopped!");
			Utilities.AssertStopped(server);
			Utilities.StopTcpClient(client);
			Utilities.WaitForStop(client, "TCP/IP client could not be stopped!");
			Utilities.AssertStopped(client);
		}

		#endregion

		#region Tests > ServerAutoSocketConnectAndAutoSocketShutdown()
		//------------------------------------------------------------------------------------------
		// Tests > ServerAutoSocketConnectAndAutoSocketShutdown()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestServerAutoSocketConnectAndAutoSocketShutdown()
		{
			int serverPort;
			TcpServer server;
			TcpAutoSocket autoSocket;

			Utilities.StartTcpServer(out server, out serverPort);
			Utilities.WaitForStart(server, "TCP/IP server could not be started!");
			Utilities.StartTcpAutoSocketAsClient(out autoSocket, serverPort);
			Utilities.WaitForStart(autoSocket, "TCP/IP AutoSocket could not be started!");
			Utilities.WaitForConnect(autoSocket, server, "TCP/IP AutoSocket could not be connected to server!");
			Utilities.AssertStartedAndTransmissive(autoSocket);
			Utilities.AssertStartedAndTransmissive(server);

			Utilities.StopTcpAutoSocket(autoSocket);
			Utilities.WaitForDisconnect(server, autoSocket, "TCP/IP server is not disconnected!");
			Utilities.WaitForStop(autoSocket, "TCP/IP AutoSocket could not be stopped!");
			Utilities.AssertStopped(autoSocket);
			Utilities.StopTcpServer(server);
			Utilities.WaitForStop(server, "TCP/IP server could not be stopped!");
			Utilities.AssertStopped(server);
		}

		#endregion

		#region Tests > ServerAutoSocketConnectAndServerShutdown()
		//------------------------------------------------------------------------------------------
		// Tests > ServerAutoSocketConnectAndServerShutdown()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestServerAutoSocketConnectAndServerShutdown()
		{
			int serverPort;
			TcpServer server;
			TcpAutoSocket autoSocket;

			Utilities.StartTcpServer(out server, out serverPort);
			Utilities.WaitForStart(server, "TCP/IP server could not be started!");
			Utilities.StartTcpAutoSocketAsClient(out autoSocket, serverPort);
			Utilities.WaitForStart(autoSocket, "TCP/IP AutoSocket could not be started!");
			Utilities.WaitForConnect(autoSocket, server, "TCP/IP AutoSocket could not be connected to server!");
			Utilities.AssertStartedAndTransmissive(autoSocket);
			Utilities.AssertStartedAndTransmissive(server);

			Utilities.StopTcpServer(server);
			Utilities.WaitForDisconnect(autoSocket, server, "TCP/IP AutoSocket is not disconnected!");
			Utilities.WaitForStop(server, "TCP/IP server could not be stopped!");
			Utilities.AssertStopped(server);
			Utilities.StopTcpAutoSocket(autoSocket);
			Utilities.WaitForStop(autoSocket, "TCP/IP AutoSocket could not be stopped!");
			Utilities.AssertStopped(autoSocket);
		}

		#endregion

		#region Tests > AutoSocketClientConnectAndClientShutdown()
		//------------------------------------------------------------------------------------------
		// Tests > AutoSocketClientConnectAndClientShutdown()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestAutoSocketClientConnectAndClientShutdown()
		{
			int serverPort;
			TcpAutoSocket autoSocket;
			TcpClient client;

			Utilities.StartTcpAutoSocketAsServer(out autoSocket, out serverPort);
			Utilities.WaitForStart(autoSocket, "TCP/IP AutoSocket could not be started!");
			Thread.Sleep(AutoSocketDelay); // Wait a while to let AutoSocket become a server.
			Utilities.StartTcpClient(out client, serverPort);
			Utilities.WaitForStart(client, "TCP/IP client could not be started!");
			Utilities.WaitForConnect(client, autoSocket, "TCP/IP client could not be connected to server!");
			Utilities.AssertStartedAndTransmissive(client);
			Utilities.AssertStartedAndTransmissive(autoSocket);

			Utilities.StopTcpClient(client);
			Utilities.WaitForDisconnect(autoSocket, client, "TCP/IP AutoSocket is not disconnected!");
			Utilities.WaitForStop(client, "TCP/IP client could not be stopped!");
			Utilities.AssertStopped(client);
			Utilities.StopTcpAutoSocket(autoSocket);
			Utilities.WaitForStop(autoSocket, "TCP/IP AutoSocket could not be stopped!");
			Utilities.AssertStopped(autoSocket);
		}

		#endregion

		#region Tests > AutoSocketClientConnectAndAutoSocketShutdown()
		//------------------------------------------------------------------------------------------
		// Tests > AutoSocketClientConnectAndAutoSocketShutdown()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestAutoSocketClientConnectAndAutoSocketShutdown()
		{
			int serverPort;
			TcpAutoSocket autoSocket;
			TcpClient client;

			Utilities.StartTcpAutoSocketAsServer(out autoSocket, out serverPort);
			Utilities.WaitForStart(autoSocket, "TCP/IP AutoSocket could not be started!");
			Thread.Sleep(AutoSocketDelay); // Wait a while to let AutoSocket become a server.
			Utilities.StartTcpClient(out client, serverPort);
			Utilities.WaitForStart(client, "TCP/IP client could not be started!");
			Utilities.WaitForConnect(client, autoSocket, "TCP/IP client could not be connected to server");
			Utilities.AssertStartedAndTransmissive(client);
			Utilities.AssertStartedAndTransmissive(autoSocket);

			Utilities.StopTcpAutoSocket(autoSocket);
			Utilities.WaitForDisconnect(client, autoSocket, "TCP/IP client is not disconnected!");
			Utilities.WaitForStop(autoSocket, "TCP/IP AutoSocket could not be stopped!");
			Utilities.AssertStopped(autoSocket);
			Utilities.StopTcpClient(client);
			Utilities.WaitForStop(client, "TCP/IP client could not be stopped!");
			Utilities.AssertStopped(client);
		}

		#endregion

		#region Tests > AutoSocketAutoSocketConnectAndAutoSocketBShutdown()
		//------------------------------------------------------------------------------------------
		// Tests > AutoSocketAutoSocketConnectAndAutoSocketBShutdown()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestAutoSocketAutoSocketConnectAndAutoSocketBShutdown()
		{
			int serverPort;
			TcpAutoSocket autoSocketA;
			TcpAutoSocket autoSocketB;

			Utilities.StartTcpAutoSocketAsServer(out autoSocketA, out serverPort);
			Utilities.WaitForStart(autoSocketA, "TCP/IP AutoSocket A could not be started!");
			Utilities.StartTcpAutoSocketAsClient(out autoSocketB, serverPort);
			Utilities.WaitForStart(autoSocketB, "TCP/IP AutoSocket B could not be started!");
			Utilities.WaitForConnect(autoSocketB, autoSocketA, "TCP/IP AutoSocket B could not be connected to AutoSocket A!");
			Utilities.AssertStartedAndTransmissive(autoSocketB);
			Utilities.AssertStartedAndTransmissive(autoSocketA);

			Utilities.StopTcpAutoSocket(autoSocketB);
			Utilities.WaitForDisconnect(autoSocketA, autoSocketB, "TCP/IP AutoSocket A is not disconnected!");
			Utilities.WaitForStop(autoSocketB, "TCP/IP AutoSocket B could not be stopped!");
			Utilities.AssertStopped(autoSocketB);
			Utilities.StopTcpAutoSocket(autoSocketA);
			Utilities.WaitForStop(autoSocketA, "TCP/IP AutoSocket A could not be stopped!");
			Utilities.AssertStopped(autoSocketA);
		}

		#endregion

		#region Tests > AutoSocketAutoSocketConnectAndAutoSocketAShutdown()
		//------------------------------------------------------------------------------------------
		// Tests > AutoSocketAutoSocketConnectAndAutoSocketAShutdown()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestAutoSocketAutoSocketConnectAndAutoSocketAShutdown()
		{
			int serverPort;
			TcpAutoSocket autoSocketA;
			TcpAutoSocket autoSocketB;

			Utilities.StartTcpAutoSocketAsServer(out autoSocketA, out serverPort);
			Utilities.WaitForStart(autoSocketA, "TCP/IP AutoSocket A could not be started!");
			Utilities.StartTcpAutoSocketAsClient(out autoSocketB, serverPort);
			Utilities.WaitForStart(autoSocketB, "TCP/IP AutoSocket B could not be started!");
			Utilities.WaitForConnect(autoSocketB, autoSocketA, "TCP/IP AutoSocket B could not be connected to AutoSocket A!");
			Utilities.AssertStartedAndTransmissive(autoSocketB);
			Utilities.AssertStartedAndTransmissive(autoSocketA);

			Utilities.StopTcpAutoSocket(autoSocketA);
			Utilities.WaitForDisconnect(autoSocketB, autoSocketA, "TCP/IP AutoSocket B is not disconnected!");
			Utilities.WaitForStop(autoSocketA, "TCP/IP AutoSocket A could not be stopped!");
			Utilities.AssertStopped(autoSocketA);
			Utilities.StopTcpAutoSocket(autoSocketB);
			Utilities.WaitForStop(autoSocketB, "TCP/IP AutoSocket B could not be stopped!");
			Utilities.AssertStopped(autoSocketB);
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
