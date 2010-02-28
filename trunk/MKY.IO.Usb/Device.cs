//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
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
    /// A USB device.
    /// </summary>
	public class Device
	{
		#region Static Methods
		//==========================================================================================
		// Static Methods
		//==========================================================================================

        /// <summary>
        /// Returns an array of all USB devices of the given class currently available on the system.
        /// </summary>
        /// <param name="classGuid">GUID of a class of devices.</param>
        public static string[] GetDevicesFromGuid(Guid classGuid)
        {
            return (Utilities.Win32.DeviceManagement.GetDevicesFromGuid(classGuid));
        }

		#endregion

        #region Fields
        //==========================================================================================
        // Fields
        //==========================================================================================

        private DeviceId _deviceId;

        #endregion

        #region Object Lifetime
        //==========================================================================================
        // Object Lifetime
        //==========================================================================================

        /// <summary></summary>
        public Device(int vid, int pid)
        {
        }

        /// <summary></summary>
        public Device(int vid, int pid, string serialNumber)
        {
        }

        /// <summary></summary>
        public Device(DeviceId deviceId)
        {
        }

        /// <summary></summary>
        public Device(string path)
        {
        }

        #endregion

		#region Properties
		//==========================================================================================
		// Properties
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
