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
// MKY Development Version 1.0.18
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2017 Matthias Kläy.
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

		#region Tests > ConnectAndShutdown()
		//------------------------------------------------------------------------------------------
		// Tests > ConnectAndShutdown()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestConnectAndShutdown()
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

			Utilities.StopUdpSocket(pairSocketB);
			Utilities.WaitForStop(pairSocketB, "UDP/IP PairSocket B could not be stopped!");
			Utilities.StopUdpSocket(pairSocketA);
			Utilities.WaitForStop(pairSocketA, "UDP/IP PairSocket A could not be stopped!");
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
