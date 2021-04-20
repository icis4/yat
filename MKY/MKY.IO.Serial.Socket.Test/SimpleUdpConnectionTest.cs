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

using NUnit.Framework;
using NUnitEx;

namespace MKY.IO.Serial.Socket.Test
{
	/// <summary></summary>
	[TestFixture]
	public class SimpleUdpConnectionTest
	{
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
			int serverPort = Utilities.GetAvailableLocalUdpPort();

			UdpSocket server;
			Utilities.CreateAndStartAsServer(out server, serverPort);
			using (server)
			{
				Utilities.WaitForStart(server, "UDP/IP server could not be started!");

				UdpSocket client;
				Utilities.CreateAndStartAsClient(out client, serverPort);
				using (client)
				{
					Utilities.WaitForStart(client, "UDP/IP client could not be started!");
					Utilities.AssertStartedAndTransmissive(client);
					Utilities.AssertStartedAndConnected(server); // Only transmissive after client has sent something.

					Utilities.Stop(client);
					Utilities.WaitForStop(client, "UDP/IP client could not be stopped!");
					Utilities.AssertStopped(client);
					Utilities.Stop(server);
					Utilities.WaitForStop(server, "UDP/IP server could not be stopped!");
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
			int serverPort = Utilities.GetAvailableLocalUdpPort();

			UdpSocket server;
			Utilities.CreateAndStartAsServer(out server, serverPort);
			using (server)
			{
				Utilities.WaitForStart(server, "UDP/IP server could not be started!");

				UdpSocket client;
				Utilities.CreateAndStartAsClient(out client, serverPort);
				using (client)
				{
					Utilities.WaitForStart(client, "UDP/IP client could not be started!");
					Utilities.AssertStartedAndTransmissive(client);
					Utilities.AssertStartedAndConnected(server); // Only transmissive after client has sent something.

					Utilities.Stop(server);
					Utilities.WaitForStop(server, "UDP/IP server could not be stopped!");
					Utilities.AssertStopped(server);
					Utilities.Stop(client);
					Utilities.WaitForStop(client, "UDP/IP client could not be stopped!");
					Utilities.AssertStopped(client);
				}
			}
		}

		#endregion

		#region Tests > PairSocketPairSocketConnectAndPairSocketBShutdown()
		//------------------------------------------------------------------------------------------
		// Tests > PairSocketPairSocketConnectAndPairSocketBShutdown()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		[StandardDurationCategory.Second1]
		public virtual void TestPairSocketPairSocketConnectAndPairSocketBShutdown()
		{
			int portA;
			int portB;
			Utilities.GetAvailableLocalUdpPorts(out portA, out portB);

			UdpSocket pairSocketA;
			Utilities.CreateAndStartAsPairSocket(out pairSocketA, portB, portA);
			using (pairSocketA)
			{
				Utilities.WaitForStart(pairSocketA, "UDP/IP PairSocket A could not be started!");

				UdpSocket pairSocketB;
				Utilities.CreateAndStartAsPairSocket(out pairSocketB, portA, portB);
				using (pairSocketB)
				{
					Utilities.WaitForStart(pairSocketB, "UDP/IP PairSocket B could not be started!");
					Utilities.AssertStartedAndTransmissive(pairSocketB);
					Utilities.AssertStartedAndTransmissive(pairSocketA);

					Utilities.Stop(pairSocketB);
					Utilities.WaitForStop(pairSocketB, "UDP/IP PairSocket B could not be stopped!");
					Utilities.AssertStopped(pairSocketB);
					Utilities.Stop(pairSocketA);
					Utilities.WaitForStop(pairSocketA, "UDP/IP PairSocket A could not be stopped!");
					Utilities.AssertStopped(pairSocketA);
				}
			}
		}

		#endregion

		#region Tests > PairSocketPairSocketConnectAndPairSocketAShutdown()
		//------------------------------------------------------------------------------------------
		// Tests > PairSocketPairSocketConnectAndPairSocketAShutdown()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		[StandardDurationCategory.Second1]
		public virtual void TestPairSocketPairSocketConnectAndPairSocketAShutdown()
		{
			int portA;
			int portB;
			Utilities.GetAvailableLocalUdpPorts(out portA, out portB);

			UdpSocket pairSocketA;
			Utilities.CreateAndStartAsPairSocket(out pairSocketA, portB, portA);
			using (pairSocketA)
			{
				Utilities.WaitForStart(pairSocketA, "UDP/IP PairSocket A could not be started!");

				UdpSocket pairSocketB;
				Utilities.CreateAndStartAsPairSocket(out pairSocketB, portA, portB);
				using (pairSocketB)
				{
					Utilities.WaitForStart(pairSocketB, "UDP/IP PairSocket B could not be started!");
					Utilities.AssertStartedAndTransmissive(pairSocketB);
					Utilities.AssertStartedAndTransmissive(pairSocketA);

					Utilities.Stop(pairSocketA);
					Utilities.WaitForStop(pairSocketA, "UDP/IP PairSocket A could not be stopped!");
					Utilities.AssertStopped(pairSocketA);
					Utilities.Stop(pairSocketB);
					Utilities.WaitForStop(pairSocketB, "UDP/IP PairSocket B could not be stopped!");
					Utilities.AssertStopped(pairSocketB);
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
