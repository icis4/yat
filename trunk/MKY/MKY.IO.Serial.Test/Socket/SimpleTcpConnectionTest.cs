//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.6
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Threading;

using NUnit.Framework;

namespace MKY.IO.Serial.Test.Socket
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
			Utilities.WaitForStart(server, "TCP server could not be started!");
			Utilities.StartTcpClient(out client, serverPort);
			Utilities.WaitForStart(client, "TCP client could not be started!");
			Utilities.WaitForConnect(client, server, "TCP client could not be connected to server!");

			Utilities.StopTcpClient(client);
			Utilities.WaitForDisconnect(server, client, "TCP server is not disconnected!");
			Utilities.WaitForStop(client, "TCP client could not be stopped!");
			Utilities.StopTcpServer(server);
			Utilities.WaitForStop(server, "TCP server could not be stopped!");
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
			Utilities.WaitForStart(server, "TCP server could not be started!");
			Utilities.StartTcpClient(out client, serverPort);
			Utilities.WaitForStart(client, "TCP client could not be started!");
			Utilities.WaitForConnect(client, server, "TCP client could not be connected to server!");

			Utilities.StopTcpServer(server);
			Utilities.WaitForDisconnect(client, server, "TCP client is not disconnected!");
			Utilities.WaitForStop(server, "TCP server could not be stopped!");
			Utilities.StopTcpClient(client);
			Utilities.WaitForStop(client, "TCP client could not be stopped!");
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
			Utilities.WaitForStart(server, "TCP server could not be started!");
			Utilities.StartTcpAutoSocketAsClient(out autoSocket, serverPort);
			Utilities.WaitForStart(autoSocket, "TCP auto socket could not be started!");
			Utilities.WaitForConnect(autoSocket, server, "TCP auto socket could not be connected to server!");

			Utilities.StopTcpAutoSocket(autoSocket);
			Utilities.WaitForDisconnect(server, autoSocket, "TCP server is not disconnected!");
			Utilities.WaitForStop(autoSocket, "TCP auto socket could not be stopped!");
			Utilities.StopTcpServer(server);
			Utilities.WaitForStop(server, "TCP server could not be stopped!");
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
			Utilities.WaitForStart(server, "TCP server could not be started!");
			Utilities.StartTcpAutoSocketAsClient(out autoSocket, serverPort);
			Utilities.WaitForStart(autoSocket, "TCP auto socket could not be started!");
			Utilities.WaitForConnect(autoSocket, server, "TCP auto socket could not be connected to server!");

			Utilities.StopTcpServer(server);
			Utilities.WaitForDisconnect(autoSocket, server, "TCP auto socket is not disconnected!");
			Utilities.WaitForStop(server, "TCP server could not be stopped!");
			Utilities.StopTcpAutoSocket(autoSocket);
			Utilities.WaitForStop(autoSocket, "TCP auto socket could not be stopped!");
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
			Utilities.WaitForStart(autoSocket, "TCP auto socket could not be started!");
			Thread.Sleep(AutoSocketDelay); // Wait a while to let AutoSocket become a server.
			Utilities.StartTcpClient(out client, serverPort);
			Utilities.WaitForStart(client, "TCP client could not be started!");
			Utilities.WaitForConnect(client, autoSocket, "TCP client could not be connected to server!");

			Utilities.StopTcpClient(client);
			Utilities.WaitForDisconnect(autoSocket, client, "TCP auto socket is not disconnected!");
			Utilities.WaitForStop(client, "TCP client could not be stopped!");
			Utilities.StopTcpAutoSocket(autoSocket);
			Utilities.WaitForStop(autoSocket, "TCP auto socket could not be stopped!");
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
			Utilities.WaitForStart(autoSocket, "TCP auto socket could not be started!");
			Thread.Sleep(AutoSocketDelay); // Wait a while to let AutoSocket become a server.
			Utilities.StartTcpClient(out client, serverPort);
			Utilities.WaitForStart(client, "TCP client could not be started!");
			Utilities.WaitForConnect(client, autoSocket, "TCP client could not be connected to server");

			Utilities.StopTcpAutoSocket(autoSocket);
			Utilities.WaitForDisconnect(client, autoSocket, "TCP client is not disconnected!");
			Utilities.WaitForStop(autoSocket, "TCP auto socket could not be stopped!");
			Utilities.StopTcpClient(client);
			Utilities.WaitForStop(client, "TCP client could not be stopped!");
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
			Utilities.WaitForStart(autoSocketA, "TCP auto socket A could not be started!");
			Utilities.StartTcpAutoSocketAsClient(out autoSocketB, serverPort);
			Utilities.WaitForStart(autoSocketB, "TCP auto socket B could not be started!");
			Utilities.WaitForConnect(autoSocketB, autoSocketA, "TCP auto socket B could not be connected to auto socket A!");

			Utilities.StopTcpAutoSocket(autoSocketB);
			Utilities.WaitForDisconnect(autoSocketA, autoSocketB, "TCP auto socket A is not disconnected!");
			Utilities.WaitForStop(autoSocketB, "TCP auto socket B could not be stopped!");
			Utilities.StopTcpAutoSocket(autoSocketA);
			Utilities.WaitForStop(autoSocketA, "TCP auto socket A could not be stopped!");
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
			Utilities.WaitForStart(autoSocketA, "TCP auto socket A could not be started!");
			Utilities.StartTcpAutoSocketAsClient(out autoSocketB, serverPort);
			Utilities.WaitForStart(autoSocketB, "TCP auto socket B could not be started!");
			Utilities.WaitForConnect(autoSocketB, autoSocketA, "TCP auto socket B could not be connected to auto socket A!");

			Utilities.StopTcpAutoSocket(autoSocketA);
			Utilities.WaitForDisconnect(autoSocketB, autoSocketA, "TCP auto socket B is not disconnected!");
			Utilities.WaitForStop(autoSocketA, "TCP auto socket A could not be stopped!");
			Utilities.StopTcpAutoSocket(autoSocketB);
			Utilities.WaitForStop(autoSocketB, "TCP auto socket B could not be stopped!");
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
