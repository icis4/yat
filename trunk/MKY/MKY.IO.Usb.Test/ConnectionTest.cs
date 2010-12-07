﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using NUnit.Framework;

namespace MKY.IO.Usb.Test
{
	/// <summary></summary>
	[TestFixture]
	public class ConnectionTest
	{
		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		#region Tests > Connect/Disconnect()
		//------------------------------------------------------------------------------------------
		// Tests > Connect/Disconnect()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestConnectDisconnect()
		{
			if (!SettingsProvider.Settings.SerialHidDeviceAIsAvailable)
				Assert.Fail();
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
