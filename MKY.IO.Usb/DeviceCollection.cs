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

        private DeviceClass _deviceClass = DeviceClass.Any;
        private Guid _classGuid = new Guid();

        /// <summary></summary>
        public DeviceCollection()
        {
        }

        /// <summary></summary>
        public DeviceCollection(DeviceClass deviceClass)
        {
            _deviceClass = deviceClass;

            switch (_deviceClass)
            {
                case DeviceClass.Hid: _classGuid = Utilities.Win32.Hid.GetHidGuid(); break;
            }
        }

        /// <summary></summary>
		public DeviceCollection(IEnumerable<DeviceId> rhs)
			: base(rhs)
		{
            DeviceCollection casted = rhs as DeviceCollection;
            if (casted != null)
            {
                _deviceClass = casted._deviceClass;
                _classGuid   = casted._classGuid;
            }
		}

		/// <summary>
		/// Fills list with all available USB Ser/HID devices.
		/// </summary>
		public void FillWithAvailableDevices()
		{
			Clear();
            foreach (DeviceId id in Device.GetDevicesFromGuid(_classGuid))
                base.Add(id);
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
