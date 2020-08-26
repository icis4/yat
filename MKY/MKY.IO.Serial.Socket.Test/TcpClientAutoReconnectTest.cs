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

using NUnit.Framework;
using NUnitEx;

namespace MKY.IO.Serial.Socket.Test
{
	/// <summary></summary>
	[TestFixture]
	public class TcpClientAutoReconnectTest
	{
		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		#region Tests > ConsecutiveDisconnect()
		//------------------------------------------------------------------------------------------
		// Tests > ConsecutiveDisconnect()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		[EnduranceCategory, StandardDurationCategory.Minute1]
		public virtual void TestConsecutiveDisconnectEndurance01Minute()
		{
			TestConsecutiveDisconnectEndurance(1);
		}

		/// <summary></summary>
		[Test]
		[EnduranceCategory, StandardDurationCategory.Minutes15]
		public virtual void TestConsecutiveDisconnectEndurance15Minutes()
		{
			TestConsecutiveDisconnectEndurance(15);
		}

		/// <summary></summary>
		[Test]
		[EnduranceCategory, InfiniteDurationCategory]
		public virtual void TestConsecutiveDisconnectEnduranceForever()
		{
			TestConsecutiveDisconnectEndurance(int.MaxValue);
		}

		/// <summary></summary>
		public static void TestConsecutiveDisconnectEndurance(int minutes)
		{
			int serverPort;
			TcpServer server;
			TcpClient client;

			Utilities.CreateAndStartAsync(out server, out serverPort);
			Utilities.WaitForStart(server, "TCP/IP server could not be started!");
			Utilities.CreateAndStartAsync(out client, serverPort, SocketSettings.TcpClientAutoReconnectDefault);
			Utilities.WaitForStart(client, "TCP/IP client could not be started!");
			Utilities.WaitForConnect(client, server, "TCP/IP client could not be connected to server!");
			Utilities.AssertStartedAndTransmissive(client);
			Utilities.AssertStartedAndTransmissive(server);

			for (int minute = 0; minute < minutes; minute++) // A loop takes around 1 minute.
			{
				// Repeat server disconnect a few times:
				for (int i = 0; i < 10; i++)
					ServerDisconnectReconnect(server, client);

				// Toggle client/server disconnect a few times:
				ClientDisconnectReconnect(client, server);
				ServerDisconnectReconnect(server, client);
				ClientDisconnectReconnect(client, server);
				ServerDisconnectReconnect(server, client);
				ClientDisconnectReconnect(client, server);
				ServerDisconnectReconnect(server, client);
				ClientDisconnectReconnect(client, server);
				ServerDisconnectReconnect(server, client);
				ClientDisconnectReconnect(client, server);
				ServerDisconnectReconnect(server, client);

				// Repeat client disconnect a few times:
				for (int i = 0; i < 10; i++)
					ClientDisconnectReconnect(client, server);
			}

			Utilities.StopAsync(client);
			Utilities.WaitForDisconnect(server, client, "TCP/IP server is not disconnected!");
			Utilities.WaitForStop(client, "TCP/IP client could not be stopped!");
			Utilities.AssertStopped(client);
			Utilities.StopAsync(server);
			Utilities.WaitForStop(server, "TCP/IP server could not be stopped!");
			Utilities.AssertStopped(server);
		}

		/// <summary></summary>
		private static void ServerDisconnectReconnect(TcpServer server, TcpClient client)
		{
			Utilities.StopAsync(server);
			Utilities.WaitForStop(server, "TCP/IP server could not be stopped!");
			Utilities.AssertStopped(server);
			Assert.That(server.ConnectedClientCount, Is.EqualTo(0));

			Utilities.AssertStartedAndDisconnected(client);

			Utilities.StartAsync(server);
			Utilities.WaitForStart(server, "TCP/IP server could not be started!");
			Utilities.WaitForConnect(client, server, "TCP/IP client could not be connected to server!");
			Assert.That(server.ConnectedClientCount, Is.EqualTo(1));
		}

		/// <summary></summary>
		private static void ClientDisconnectReconnect(TcpClient client, TcpServer server)
		{
			Utilities.StopAsync(client);
			Utilities.WaitForStop(client, "TCP/IP client could not be stopped!");
			Utilities.AssertStopped(client);

			Utilities.AssertStartedAndDisconnected(server);
			Assert.That(server.ConnectedClientCount, Is.EqualTo(0));

			Utilities.StartAsync(client);
			Utilities.WaitForStart(client, "TCP/IP client could not be started!");
			Utilities.WaitForConnect(client, server, "TCP/IP client could not be connected to server!");
			Assert.That(server.ConnectedClientCount, Is.EqualTo(1));
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
