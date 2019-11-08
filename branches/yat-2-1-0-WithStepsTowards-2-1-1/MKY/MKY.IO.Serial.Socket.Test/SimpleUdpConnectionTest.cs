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

using NUnit.Framework;

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
		public virtual void TestServerClientConnectAndClientShutdown()
		{
			int serverPort = Utilities.AvailableLocalUdpPort;
			UdpSocket server;
			UdpSocket client;

			Utilities.StartUdpServer(out server, serverPort);
			Utilities.WaitForStart(server, "UDP/IP server could not be started!");
			Utilities.StartUdpClient(out client, serverPort);
			Utilities.WaitForStart(client, "UDP/IP client could not be started!");
			Utilities.AssertStartedAndTransmissive(client);
			Utilities.AssertStartedAndConnected(server); // Only transmissive after client has sent something.

			Utilities.StopUdpSocket(client);
			Utilities.WaitForStop(client, "UDP/IP client could not be stopped!");
			Utilities.AssertStopped(client);
			Utilities.StopUdpSocket(server);
			Utilities.WaitForStop(server, "UDP/IP server could not be stopped!");
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
			int serverPort = Utilities.AvailableLocalUdpPort;
			UdpSocket server;
			UdpSocket client;

			Utilities.StartUdpServer(out server, serverPort);
			Utilities.WaitForStart(server, "UDP/IP server could not be started!");
			Utilities.StartUdpClient(out client, serverPort);
			Utilities.WaitForStart(client, "UDP/IP client could not be started!");
			Utilities.AssertStartedAndTransmissive(client);
			Utilities.AssertStartedAndConnected(server); // Only transmissive after client has sent something.

			Utilities.StopUdpSocket(server);
			Utilities.WaitForStop(server, "UDP/IP server could not be stopped!");
			Utilities.AssertStopped(server);
			Utilities.StopUdpSocket(client);
			Utilities.WaitForStop(client, "UDP/IP client could not be stopped!");
			Utilities.AssertStopped(client);
		}

		#endregion

		#region Tests > PairSocketPairSocketConnectAndPairSocketBShutdown()
		//------------------------------------------------------------------------------------------
		// Tests > PairSocketPairSocketConnectAndPairSocketBShutdown()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestPairSocketPairSocketConnectAndPairSocketBShutdown()
		{
			int portA;
			int portB;
			Utilities.GetAvailableLocalUdpPorts(out portA, out portB);
			UdpSocket pairSocketA;
			UdpSocket pairSocketB;

			Utilities.StartUdpPairSocket(out pairSocketA, portB, portA);
			Utilities.WaitForStart(pairSocketA, "UDP/IP PairSocket A could not be started!");
			Utilities.StartUdpPairSocket(out pairSocketB, portA, portB);
			Utilities.WaitForStart(pairSocketB, "UDP/IP PairSocket B could not be started!");
			Utilities.AssertStartedAndTransmissive(pairSocketB);
			Utilities.AssertStartedAndTransmissive(pairSocketA);

			Utilities.StopUdpSocket(pairSocketB);
			Utilities.WaitForStop(pairSocketB, "UDP/IP PairSocket B could not be stopped!");
			Utilities.AssertStopped(pairSocketB);
			Utilities.StopUdpSocket(pairSocketA);
			Utilities.WaitForStop(pairSocketA, "UDP/IP PairSocket A could not be stopped!");
			Utilities.AssertStopped(pairSocketA);
		}

		#endregion

		#region Tests > PairSocketPairSocketConnectAndPairSocketAShutdown()
		//------------------------------------------------------------------------------------------
		// Tests > PairSocketPairSocketConnectAndPairSocketAShutdown()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestPairSocketPairSocketConnectAndPairSocketAShutdown()
		{
			int portA;
			int portB;
			Utilities.GetAvailableLocalUdpPorts(out portA, out portB);
			UdpSocket pairSocketA;
			UdpSocket pairSocketB;

			Utilities.StartUdpPairSocket(out pairSocketA, portB, portA);
			Utilities.WaitForStart(pairSocketA, "UDP/IP PairSocket A could not be started!");
			Utilities.StartUdpPairSocket(out pairSocketB, portA, portB);
			Utilities.WaitForStart(pairSocketB, "UDP/IP PairSocket B could not be started!");
			Utilities.AssertStartedAndTransmissive(pairSocketB);
			Utilities.AssertStartedAndTransmissive(pairSocketA);

			Utilities.StopUdpSocket(pairSocketA);
			Utilities.WaitForStop(pairSocketA, "UDP/IP PairSocket A could not be stopped!");
			Utilities.AssertStopped(pairSocketA);
			Utilities.StopUdpSocket(pairSocketB);
			Utilities.WaitForStop(pairSocketB, "UDP/IP PairSocket B could not be stopped!");
			Utilities.AssertStopped(pairSocketB);
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
