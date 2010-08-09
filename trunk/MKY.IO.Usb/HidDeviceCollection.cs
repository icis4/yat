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
// ------------------------------------------------------------------------------------------------
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:maettu_this@users.sourceforge.net.
//==================================================================================================

using System;
using System.Collections.Generic;

namespace MKY.IO.Usb
{
	/// <summary>
	/// List containing USB HID device infos.
	/// </summary>
	[Serializable]
	public class HidDeviceCollection : DeviceCollection
	{
		private HidUsagePage usagePage = HidUsagePage.Unknown;
		private HidUsage     usage     = HidUsage.Unknown;

		/// <summary></summary>
		public HidDeviceCollection()
			: base(DeviceClass.Hid)
		{
		}

		/// <summary></summary>
		public HidDeviceCollection(HidUsagePage usagePage)
			: base(DeviceClass.Hid)
		{
			this.usagePage = usagePage;
		}

		/// <summary></summary>
		public HidDeviceCollection(HidUsagePage usagePage, HidUsage usage)
			: base(DeviceClass.Hid)
		{
			this.usagePage = usagePage;
			this.usage     = usage;
		}

		/// <summary></summary>
		public HidDeviceCollection(IEnumerable<DeviceInfo> rhs)
			: base(rhs)
		{
			HidDeviceCollection casted = rhs as HidDeviceCollection;
			if (casted != null)
			{
				this.usagePage = casted.usagePage;
				this.usage     = casted.usage;
			}
		}

		/// <summary>
		/// Fills list with the available USB HID devices.
		/// </summary>
		public override void FillWithAvailableDevices()
		{
			Clear();
			foreach (DeviceInfo di in HidDevice.GetDevices(this.usagePage, this.usage))
				Add(di);
			Sort();
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
