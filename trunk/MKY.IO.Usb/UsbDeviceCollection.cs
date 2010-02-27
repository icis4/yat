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

namespace MKY.IO.Serial
{
	/// <summary>
	/// List containing USB Ser/HID device IDs.
	/// </summary>
	[Serializable]
	public class UsbDeviceCollection : List<UsbDeviceId>
	{
        /// <summary></summary>
        public class DeviceChangedAndCancelEventArgs : EventArgs
        {
            /// <summary></summary>
            public readonly UsbDeviceId Device;

            /// <summary></summary>
            public bool Cancel = false;

            /// <summary></summary>
            public DeviceChangedAndCancelEventArgs(UsbDeviceId device)
            {
                Device = device;
            }
        }

        /// <summary></summary>
        public UsbDeviceCollection()
        {
        }

        /// <summary></summary>
		public UsbDeviceCollection(IEnumerable<UsbDeviceId> rhs)
			: base(rhs)
		{
		}

		/// <summary>
		/// Fills list with all available USB Ser/HID devices.
		/// </summary>
		public void FillWithAvailableDevices()
		{
			Clear();
            foreach (UsbLibrary.HIDDevice device in UsbLibrary.AvailableDevice.FindAvailableDevices())
			{
                UsbDeviceId id;
                if (UsbDeviceId.TryParse(device.StrPath, out id))
                {
                    id.GetInformationFromDevice();
                    base.Add(id);
                }

                // \remind
                // Find a better way to free the available devices
                device.Dispose();
			}
			Sort();
		}

        /// <summary>
        /// Checks all ports whether they are currently in use and marks them.
        /// </summary>
        /// <remarks>
        /// <see cref="UsbLibrary"/> doesn't provide a method to retrieve whether a device
        /// is currently in use or not. Therefore, this method always marks devices as not
        /// in use.
        /// </remarks>
        public void MarkDevicesInUse()
        {
            MarkDevicesInUse(null);
        }

        /// <summary>
        /// Checks all ports whether they are currently in use and marks them.
        /// </summary>
        /// <remarks>
        /// <see cref="UsbLibrary"/> doesn't provide a method to retrieve whether a device
        /// is currently in use or not. Therefore, this method always marks devices as not
        /// in use.
        /// </remarks>
        /// <param name="deviceChangedCallback">
        /// Callback delegate, can be used to get an event each time a new port is being
        /// tried to be opened. Set the <see cref="DeviceChangedAndCancelEventArgs.Cancel"/>
        /// property the true to cancel port scanning.
        /// </param>
        public void MarkDevicesInUse(EventHandler<DeviceChangedAndCancelEventArgs> deviceChangedCallback)
        {
            foreach (UsbDeviceId deviceId in this)
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
