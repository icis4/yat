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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;

#endregion

namespace MKY.IO.Usb
{
    /// <summary>
    /// A USB HID device.
    /// </summary>
	public class HidDevice : Device
	{
		#region Static Methods
		//==========================================================================================
		// Static Methods
		//==========================================================================================

        /// <summary>
        /// Returns an array of all USB HID devices currently available on the system.
        /// </summary>
        public static string[] GetDevices()
        {
            Guid hidGuid = Utilities.Win32.Hid.GetHidGuid();
            return (Utilities.Win32.DeviceManagement.GetDevicesFromGuid(hidGuid));
        }

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

        /// <summary></summary>
        public HidDevice(string systemPath)
            : base(systemPath)
        {
        }

        /// <summary></summary>
        public HidDevice(int vendorId, int productId)
            : base(vendorId, productId)
        {
        }

        /// <summary></summary>
        public HidDevice(int vendorId, int productId, string serialNumber)
            : base(vendorId, productId, serialNumber)
        {
        }

        /// <summary></summary>
        public HidDevice(DeviceInfo deviceId)
            : base(deviceId)
        {
        }

        #endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
