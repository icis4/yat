﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.22
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2018 Matthias Kläy.
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
		[Test, DeviceAIsAvailableCategory]
		public virtual void TestConnectDisconnectA()
		{
			if (!ConfigurationProvider.Configuration.DeviceAIsAvailable)
				Assert.Ignore("'USB Ser/HID Device A' is not available, therefore this test is excluded. Ensure that 'USB Ser/HID Device A' is properly configured and available if passing this test is required.");
				//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			Assert.Ignore("The USB Ser/HID connect/disconnect test is not yet implemented.");
		}

		/// <summary></summary>
		[Test, DeviceBIsAvailableCategory]
		public virtual void TestConnectDisconnectB()
		{
			if (!ConfigurationProvider.Configuration.DeviceBIsAvailable)
				Assert.Ignore("'USB Ser/HID Device B' is not available, therefore this test is excluded. Ensure that 'USB Ser/HID Device B' is properly configured and available if passing this test is required.");
				//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			Assert.Ignore("The USB Ser/HID connect/disconnect test is not yet implemented.");
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
