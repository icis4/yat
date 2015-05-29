﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.11
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2015 Matthias Kläy.
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

			UdpSocket socketA;
			UdpSocket socketB;

			Utilities.StartUdpSocket(out socketA, portB, portA);
			Utilities.WaitForStart(socketA, "UDP socket A could not be started!");
			Utilities.StartUdpSocket(out socketB, portA, portB);
			Utilities.WaitForStart(socketB, "UDP socket B could not be started!");

			Utilities.StopUdpSocket(socketB);
			Utilities.WaitForStop(socketB, "UDP socket B could not be stopped!");
			Utilities.StopUdpSocket(socketA);
			Utilities.WaitForStop(socketA, "UDP socket A could not be stopped!");
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
