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
	/// List containing USB HID device infos.
	/// </summary>
	[Serializable]
	public class HidDeviceCollection : DeviceCollection
	{
		private HidUsagePage _usagePage = HidUsagePage.Unknown;
		private HidUsage     _usage     = HidUsage.Unknown;

		/// <summary></summary>
		public HidDeviceCollection()
			: base(DeviceClass.Hid)
		{
		}

		/// <summary></summary>
		public HidDeviceCollection(HidUsagePage usagePage)
			: base(DeviceClass.Hid)
		{
			_usagePage = usagePage;
		}

		/// <summary></summary>
		public HidDeviceCollection(HidUsagePage usagePage, HidUsage usage)
			: base(DeviceClass.Hid)
		{
			_usagePage = usagePage;
			_usage     = usage;
		}

		/// <summary></summary>
		public HidDeviceCollection(IEnumerable<DeviceInfo> rhs)
			: base(rhs)
		{
			HidDeviceCollection casted = rhs as HidDeviceCollection;
			if (casted != null)
			{
				_usagePage = casted._usagePage;
				_usage     = casted._usage;
			}
		}

		/// <summary>
		/// Fills list with the available USB HID devices.
		/// </summary>
		public override void FillWithAvailableDevices()
		{
			Clear();
			foreach (DeviceInfo di in HidDevice.GetDevices(_usagePage, _usage))
				base.Add(di);
			Sort();
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
