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

namespace MKY.IO.Usb
{
	/// <summary>
	/// List containing USB HID device information.
	/// </summary>
	[Serializable]
	public class HidDeviceCollection : DeviceCollection
	{
		private HidUsagePage usagePage = HidUsagePage.Undefined;
		private HidUsageId   usageId   = HidUsageId.Undefined;

		/// <summary></summary>
		public HidDeviceCollection()
			: base(DeviceClass.Hid)
		{
		}

		/// <summary></summary>
		[CLSCompliant(false)]
		public HidDeviceCollection(HidUsagePage usagePage)
			: base(DeviceClass.Hid)
		{
			this.usagePage = usagePage;
		}

		/// <summary></summary>
		[CLSCompliant(false)]
		public HidDeviceCollection(HidUsagePage usagePage, HidUsageId usageId)
			: base(DeviceClass.Hid)
		{
			this.usagePage = usagePage;
			this.usageId   = usageId;
		}

		/// <summary></summary>
		public HidDeviceCollection(IEnumerable<DeviceInfo> rhs)
			: base(rhs)
		{
			var casted = (rhs as HidDeviceCollection);
			if (casted != null)
			{
				this.usagePage = casted.usagePage;
				this.usageId   = casted.usageId;
			}
		}

		/// <summary>
		/// Fills list with the available USB HID devices.
		/// </summary>
		public override void FillWithAvailableDevices(bool retrieveStringsFromDevice = true)
		{
			Clear();

			foreach (DeviceInfo di in HidDevice.GetDevices(this.usagePage, this.usageId, retrieveStringsFromDevice))
				Add(di);

			Sort();
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
