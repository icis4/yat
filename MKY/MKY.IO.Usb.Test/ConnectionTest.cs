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
// MKY Version 1.0.29
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Diagnostics.CodeAnalysis;

using MKY.Test.Devices;

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

		/// <remarks>
		/// Test is optional, it can be excluded if either USB Ser/HID device or USB hub is not available.
		/// </remarks>
		/// <remarks>
		/// So far, the USB hub and USB port assignment is hard-coded, could become configurable.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Ser/HID' just happens to contain 'Ser'...")]
		[Test, DeviceAIsAvailableCategory, MKY.Test.UsbHub1IsAvailableCategory]
		public virtual void TestConnectDisconnectA()
		{
			if (!ConfigurationProvider.Configuration.DeviceAIsAvailable)
				Assert.Ignore("'USB Ser/HID Device A' is not available, therefore this test is excluded. Ensure that 'USB Ser/HID Device A' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			if (!MKY.Test.ConfigurationProvider.Configuration.UsbHub1IsAvailable)
				Assert.Ignore(UsbHubControl.ErrorMessage);
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			Assert.Ignore("The USB Ser/HID connect/disconnect test is not yet implemented.");
		}

		/// <remarks>
		/// Test is optional, it can be excluded if either USB Ser/HID device or USB hub is not available.
		/// </remarks>
		/// <remarks>
		/// So far, the USB hub and USB port assignment is hard-coded, could become configurable.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Ser/HID' just happens to contain 'Ser'...")]
		[Test, DeviceBIsAvailableCategory, MKY.Test.UsbHub1IsAvailableCategory]
		public virtual void TestConnectDisconnectB()
		{
			if (!ConfigurationProvider.Configuration.DeviceBIsAvailable)
				Assert.Ignore("'USB Ser/HID Device B' is not available, therefore this test is excluded. Ensure that 'USB Ser/HID Device B' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			if (!MKY.Test.ConfigurationProvider.Configuration.UsbHub1IsAvailable)
				Assert.Ignore(UsbHubControl.ErrorMessage);
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
