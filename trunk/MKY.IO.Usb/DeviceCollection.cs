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
	/// List containing USB device infos.
	/// </summary>
	[Serializable]
	public class DeviceCollection : List<DeviceInfo>
	{
		private DeviceClass deviceClass = DeviceClass.Any;
		private Guid classGuid = Guid.Empty;

		/// <summary></summary>
		public DeviceCollection()
		{
		}

		/// <summary></summary>
		public DeviceCollection(DeviceClass deviceClass)
		{
			this.deviceClass = deviceClass;
			this.classGuid = Device.GetGuidFromDeviceClass(deviceClass);
		}

		/// <summary></summary>
		public DeviceCollection(IEnumerable<DeviceInfo> rhs)
			: base(rhs)
		{
			DeviceCollection casted = rhs as DeviceCollection;
			if (casted != null)
			{
				this.deviceClass = casted.deviceClass;
				this.classGuid   = casted.classGuid;
			}
		}

		/// <summary>
		/// Fills list with the available USB devices.
		/// </summary>
		public virtual void FillWithAvailableDevices()
		{
			Clear();
			foreach (DeviceInfo di in Device.GetDevicesFromGuid(this.classGuid))
				base.Add(di);
			Sort();
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
