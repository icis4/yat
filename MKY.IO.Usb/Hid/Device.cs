//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright � 2003-2004 HSR Hochschule f�r Technik Rapperswil.
// Copyright � 2003-2010 Matthias Kl�y.
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

namespace MKY.IO.Usb.Hid
{
    /// <summary>
    /// A USB HID device.
    /// </summary>
	public class Device : Usb.Device
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
        public Device(int vid, int pid)
            : base(vid, pid)
        {
        }

        /// <summary></summary>
        public Device(int vid, int pid, string serialNumber)
            : base(vid, pid, serialNumber)
        {
        }

        /// <summary></summary>
        public Device(DeviceId deviceId)
            : base(deviceId)
        {
        }

        /// <summary></summary>
        public Device(string path)
            : base(path)
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
