//==================================================================================================
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

using System;
using System.Collections.Generic;

namespace MKY.IO.Usb
{
	/// <summary>
	/// List containing USB Ser/HID device infos.
	/// </summary>
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
		public override void FillWithAvailableDevices()
		{
			Clear();
			foreach (DeviceInfo di in SerialHidDevice.GetDevices())
				base.Add(di);
			Sort();
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
