//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.14
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MKY.IO.Usb
{
	/// <summary>
	/// List containing USB Ser/HID device information.
	/// </summary>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Ser/HID just happens to contain 'Ser'...")]
	[Serializable]
	public class SerialHidDeviceCollection : HidDeviceCollection
	{
		/// <summary></summary>
		public SerialHidDeviceCollection()
		{
		}

		/// <summary></summary>
		public SerialHidDeviceCollection(IEnumerable<DeviceInfo> rhs)
			: base(rhs)
		{
		}

		/// <summary>
		/// Fills list with the available USB Ser/HID devices.
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Ser/HID just happens to contain 'Ser'...")]
		public override void FillWithAvailableDevices(bool retrieveStringsFromDevice = true)
		{
			Clear();

			foreach (DeviceInfo di in SerialHidDevice.GetDevices(retrieveStringsFromDevice))
				Add(di);

			Sort();
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
