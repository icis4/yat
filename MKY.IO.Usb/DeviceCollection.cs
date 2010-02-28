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

using System;
using System.Collections.Generic;

namespace MKY.IO.Usb
{
	/// <summary>
	/// List containing USB device IDs.
	/// </summary>
	[Serializable]
	public class DeviceCollection : List<DeviceId>
	{
        /// <summary></summary>
        public class DeviceChangedAndCancelEventArgs : EventArgs
        {
            /// <summary></summary>
            public readonly DeviceId Device;

            /// <summary></summary>
            public bool Cancel = false;

            /// <summary></summary>
            public DeviceChangedAndCancelEventArgs(DeviceId device)
            {
                Device = device;
            }
        }

        private Guid _classGuid = new Guid();

        /// <summary></summary>
        public DeviceCollection()
        {
        }

        /// <summary></summary>
        public DeviceCollection(Guid classGuid)
        {
            _classGuid = classGuid;
        }

        /// <summary></summary>
		public DeviceCollection(IEnumerable<DeviceId> rhs)
			: base(rhs)
		{
            DeviceCollection casted = rhs as DeviceCollection;
            if (casted != null)
                _classGuid = casted._classGuid;
            else
                _classGuid = new Guid();
		}

		/// <summary>
		/// Fills list with all available USB Ser/HID devices.
		/// </summary>
		public void FillWithAvailableDevices()
		{
			Clear();
            foreach (string path in Utilities.Win32.DeviceManagement.GetDevicesFromGuid(_classGuid))
			{
                DeviceId id;
                if (DeviceId.TryParse(path, out id))
                {
                    id.GetInformationFromDevice();
                    base.Add(id);
                }
			}
			Sort();
		}

        /// <summary>
        /// Checks all ports whether they are currently in use and marks them.
        /// </summary>
        public void MarkDevicesInUse()
        {
            MarkDevicesInUse(null);
        }

        /// <summary>
        /// Checks all ports whether they are currently in use and marks them.
        /// </summary>
        /// <param name="deviceChangedCallback">
        /// Callback delegate, can be used to get an event each time a new port is being
        /// tried to be opened. Set the <see cref="DeviceChangedAndCancelEventArgs.Cancel"/>
        /// property the true to cancel port scanning.
        /// </param>
        public void MarkDevicesInUse(EventHandler<DeviceChangedAndCancelEventArgs> deviceChangedCallback)
        {
            foreach (DeviceId deviceId in this)
            {
                if (deviceChangedCallback != null)
                {
                    DeviceChangedAndCancelEventArgs args = new DeviceChangedAndCancelEventArgs(deviceId);
                    deviceChangedCallback.Invoke(this, args);
                    if (args.Cancel)
                        break;
                }

                // \remind
                // See remarks above.
                deviceId.IsInUse = false;
            }
        }
    }
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
