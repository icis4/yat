//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.8
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2013 Matthias Kläy.
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
		[Test, SerialHidDeviceAIsAvailableCategory]
		public virtual void TestConnectDisconnectA()
		{
			if (!SettingsProvider.Settings.SerialHidDeviceAIsAvailable)
				Assert.Ignore();
		}

		/// <summary></summary>
		[Test, SerialHidDeviceBIsAvailableCategory]
		public virtual void TestConnectDisconnectB()
		{
			if (!SettingsProvider.Settings.SerialHidDeviceBIsAvailable)
				Assert.Ignore();
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
