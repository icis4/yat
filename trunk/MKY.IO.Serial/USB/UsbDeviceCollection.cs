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
	/// List containing USB Ser/HID port IDs.
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
		/// Fills list with all available USH Ser/HID devices.
		/// </summary>
		public void FillWithAvailableDevices()
		{
			Clear();
			foreach (string portName in System.IO.Ports.SerialPort.GetPortNames())
			{
                // \todo
				//base.Add(new UsbDeviceId(portName));
			}
			Sort();
		}

        /// <summary>
        /// Queries the USB device for user readable strings like vendor or product name.
        /// </summary>
        /// <remarks>
        /// Query is never done automatically because it takes quite some time.
        /// </remarks>
        public void GetInformationFromDevices()
		{
			foreach (UsbDeviceId deviceId in this)
                deviceId.GetInformationFromDevice();
		}

        /// <summary>
        /// Checks all ports whether they are currently in use and marks them.
        /// </summary>
        /// <remarks>
        /// In .NET 2.0, no class provides a method to retrieve whether a port is currently
        /// in use or not. Therefore, this method actively tries to open every port. This
        /// takes some time.
        /// </remarks>
        public void MarkDevicesInUse()
        {
            MarkDevicesInUse(null);
        }

        /// <summary>
        /// Checks all ports whether they are currently in use and marks them.
        /// </summary>
        /// <remarks>
        /// In .NET 2.0, no class provides a method to retrieve whether a port is currently
        /// in use or not. Therefore, this method actively tries to open every port. This
        /// takes some time.
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

                // \todo
                //System.IO.Ports.SerialPort p = new System.IO.Ports.SerialPort(deviceId);
                try
                {
                    //p.Open();
                    //p.Close();
                    deviceId.IsInUse = false;
                }
                catch
                {
                    deviceId.IsInUse = true;
                }
                finally
                {
                    //p.Dispose();
                }
            }
        }
    }
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
