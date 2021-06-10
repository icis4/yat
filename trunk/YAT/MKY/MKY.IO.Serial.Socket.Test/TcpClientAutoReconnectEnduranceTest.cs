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
// MKY Version 1.0.30
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using NUnit.Framework;
using NUnitEx;

namespace MKY.IO.Serial.Socket.Test
{
	/// <summary></summary>
	[TestFixture]
	public class TcpClientAutoReconnectEnduranceTest
	{
		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		#region Tests > ConsecutiveDisconnectReconnect()
		//------------------------------------------------------------------------------------------
		// Tests > ConsecutiveDisconnectReconnect()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		[EnduranceCategory, StandardDurationCategory.Minute1]
		public virtual void TestConsecutiveDisconnectReconnectEndurance01Minute()
		{
			TestConsecutiveDisconnectReconnect(1);
		}

		/// <summary></summary>
		[Test]
		[EnduranceCategory, StandardDurationCategory.Minutes15]
		public virtual void TestConsecutiveDisconnectReconnectEndurance15Minutes()
		{
			TestConsecutiveDisconnectReconnect(15);
		}

		/// <summary></summary>
		[Test]
		[EnduranceCategory, InfiniteDurationCategory]
		public virtual void TestConsecutiveDisconnectReconnectEnduranceInfinite()
		{
			TestConsecutiveDisconnectReconnect(int.MaxValue);
		}

		#endregion

		#region Tests > ConsecutiveDisconnectReconnectWithLimitedAllowance()
		//------------------------------------------------------------------------------------------
		// Tests > ConsecutiveDisconnectReconnectWithLimitedAllowance()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		[EnduranceCategory, StandardDurationCategory.Minute1]
		public virtual void TestConsecutiveDisconnectReconnectWithLimitedAllowanceEndurance01Minute()
		{
			TestConsecutiveDisconnectReconnect(1, 1);
		}

		/// <summary></summary>
		[Test]
		[EnduranceCategory, StandardDurationCategory.Minutes15]
		public virtual void TestConsecutiveDisconnectReconnectWithLimitedAllowanceEndurance15Minutes()
		{
			TestConsecutiveDisconnectReconnect(15, 1);
		}

		/// <summary></summary>
		[Test]
		[EnduranceCategory, InfiniteDurationCategory]
		public virtual void TestConsecutiveDisconnectReconnectWithLimitedAllowanceEnduranceInfinite()
		{
			TestConsecutiveDisconnectReconnect(int.MaxValue, 1);
		}

		#endregion

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static void TestConsecutiveDisconnectReconnect(int minutes, int serverConnectionAllowance = TcpServer.ConnectionAllowanceDefault)
		{
			int serverPort;
			TcpServer server;
			Utilities.CreateAndStartAsync(out server, out serverPort, serverConnectionAllowance);
			using (server)
			{
				Utilities.WaitForStart(server, "TCP/IP server could not be started!");

				TcpClient client;
				Utilities.CreateAndStartAsync(out client, serverPort, SocketSettings.TcpClientAutoReconnectDefault);
				using (client)
				{
					Utilities.WaitForStart(client, "TCP/IP client could not be started!");
					Utilities.WaitForConnection(client, server, "TCP/IP client could not be connected to server!");
					Utilities.AssertStartedAndTransmissive(client);
					Utilities.AssertStartedAndTransmissive(server);

					for (int minute = 0; minute < minutes; minute++) // A loop takes around 1 minute.
					{
						if (minutes > 1)
							Trace.WriteLine("Minute = " + minute);

						// Repeat server disconnect several times:
						for (int i = 0; i < 30; i++) {
							Trace.WriteLine("Pattern = A / Iteration = " + i);
							ServerDisconnectReconnect(server, client);
						}

						// Toggle different client/server disconnect patterns several times:
						for (int i = 0; i < 10; i++) {
							Trace.WriteLine("Pattern = B / Iteration = " + i);
							ClientDisconnectReconnect(client, server);
							ServerDisconnectReconnect(server, client);
						}
						for (int i = 0; i < 10; i++) {
							Trace.WriteLine("Pattern = C / Iteration = " + i);
							ClientDisconnectReconnect(client, server);
							ClientDisconnectReconnect(client, server);
							ServerDisconnectReconnect(server, client);
						}
						for (int i = 0; i < 10; i++) {
							Trace.WriteLine("Pattern = D / Iteration = " + i);
							ClientDisconnectReconnect(client, server);
							ServerDisconnectReconnect(server, client);
							ServerDisconnectReconnect(server, client);
						}

						// Repeat client disconnect several times:
						for (int i = 0; i < 30; i++) {
							Trace.WriteLine("Pattern = E / Iteration = " + i);
							ClientDisconnectReconnect(client, server);
						}
					}

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

		/// <summary></summary>
		private static void ServerDisconnectReconnect(TcpServer server, TcpClient client)
		{
			Utilities.StopAsync(server);
			Utilities.WaitForStop(server, "TCP/IP server could not be stopped!");
			Utilities.AssertStopped(server);
			Assert.That(server.ConnectionCount, Is.EqualTo(0));

			Utilities.WaitForDisconnection(client, server, "TCP/IP client is not disconnected from server!");
			Utilities.AssertStartedAndDisconnected(client);

			Utilities.StartAsync(server);
			Utilities.WaitForStart(server, "TCP/IP server could not be started!");
			Utilities.WaitForConnection(server, client, "TCP/IP server could not be reconnected to client!");
			Assert.That(server.ConnectionCount, Is.EqualTo(1));
		}

		/// <summary></summary>
		private static void ClientDisconnectReconnect(TcpClient client, TcpServer server)
		{
			Utilities.StopAsync(client);
			Utilities.WaitForStop(client, "TCP/IP client could not be stopped!");
			Utilities.AssertStopped(client);

			Utilities.WaitForDisconnection(server, client, "TCP/IP server is not disconnected from client!");
			Utilities.AssertStartedAndDisconnected(server);
			Assert.That(server.ConnectionCount, Is.EqualTo(0));

			Utilities.StartAsync(client);
			Utilities.WaitForStart(client, "TCP/IP client could not be started!");
			Utilities.WaitForConnection(client, server, "TCP/IP client could not be reconnected to server!");
			Assert.That(server.ConnectionCount, Is.EqualTo(1));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
