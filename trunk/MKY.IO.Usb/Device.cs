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
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

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
        /// Returns the GUID for the given device class.
        /// </summary>
        /// <param name="deviceClass">USB device class</param>
        public static Guid GetGuidFromDeviceClass(DeviceClass deviceClass)
        {
            switch (deviceClass)
            {
                case DeviceClass.Hid: return (Utilities.Win32.Hid.GetHidGuid());
                default:              return (Guid.Empty);
            }
        }

        /// <summary>
        /// Returns an array of all USB devices of the given class currently available on the system.
        /// </summary>
        /// <remarks>
        /// \todo This method currently only works for HID devices. Find a HID independent way to retrieve VID/PID.
        /// </remarks>
        /// <param name="deviceClass">USB device class.</param>
        public static DeviceInfo[] GetDevicesFromClass(DeviceClass deviceClass)
        {
            return (GetDevicesFromGuid(GetGuidFromDeviceClass(deviceClass)));
        }

        /// <summary>
        /// Returns an array of all USB devices of the given class currently available on the system.
        /// </summary>
        /// <remarks>
        /// \todo This method currently only works for HID devices. Find a HID independent way to retrieve VID/PID.
        /// </remarks>
        /// <param name="classGuid">GUID of a class of devices.</param>
        public static DeviceInfo[] GetDevicesFromGuid(Guid classGuid)
        {
            List<DeviceInfo> devices = new List<DeviceInfo>();

            foreach (string systemPath in Utilities.Win32.DeviceManagement.GetDevicesFromGuid(classGuid))
                devices.Add(GetDeviceIdFromSystemPath(systemPath));

            return (devices.ToArray());
        }

        /// <summary>
        /// Returns the VID and PID of the device with the given system path.
        /// </summary>
        public static DeviceInfo GetDeviceIdFromSystemPath(string systemPath)
        {
            int vendorId, productId;
            string manufacturer, product, serialNumber;
            GetDeviceInfoFromSystemPath(systemPath, out vendorId, out productId, out manufacturer, out product, out serialNumber);
            return (new DeviceInfo(systemPath, vendorId, productId, manufacturer, product, serialNumber));
        }

        /// <summary>
        /// Returns the VID and PID of the device with the given system path.
        /// </summary>
        /// <remarks>
        /// \todo This method currently only works for HID devices. Find a HID independent way to retrieve VID/PID.
        /// </remarks>
        public static void GetDeviceInfoFromSystemPath(string systemPath, out int vendorId, out int productId, out string manufacturer, out string product, out string serialNumber)
        {
            vendorId = 0;
            productId = 0;
            manufacturer = "";
            product = "";
            serialNumber = "";

            SafeFileHandle hidHandle = Utilities.Win32.FileIO.CreateFile(systemPath, 0,
                Utilities.Win32.FileIO.FILE_SHARE_READ | Utilities.Win32.FileIO.FILE_SHARE_WRITE,
                IntPtr.Zero, Utilities.Win32.FileIO.OPEN_EXISTING, 0, 0);

            if (!hidHandle.IsInvalid)
            {
                try
                {
                    // Set the size property of attributes to the number of bytes in the structure.
                    Utilities.Win32.Hid.HIDD_ATTRIBUTES attributes = new Utilities.Win32.Hid.HIDD_ATTRIBUTES();
                    attributes.Size = Marshal.SizeOf(attributes);

                    if (Utilities.Win32.Hid.HidD_GetAttributes(hidHandle, ref attributes))
                    {
                        vendorId = attributes.VendorID;
                        productId = attributes.ProductID;

                        if (Utilities.Win32.Hid.HidD_GetManufacturerString(hidHandle, out manufacturer))
                        {
                            if (Utilities.Win32.Hid.HidD_GetProductString(hidHandle, out product))
                            {
                                if (Utilities.Win32.Hid.HidD_GetSerialNumberString(hidHandle, out serialNumber))
                                {
                                }
                            }
                        }
                    }
                }
                finally
                {
                    hidHandle.Close();
                }
            }
        }

        /// <summary>
        /// Returns the system path of a device with the given VID and PID.
        /// </summary>
        /// <remarks>
        /// If multiple devices with the same VID andPID are connected to the sytem, the first device is returned.
        /// </remarks>
        public static void GetSystemPathFromVidAndPid(int vendorId, int productId, out string systemPath)
        {
            systemPath = "";

            DeviceCollection devices = new DeviceCollection();
            devices.FillWithAvailableDevices();

            foreach (DeviceInfo device in GetDevicesFromClass(DeviceClass.Hid))
            {
                if ((device.VendorId == vendorId) && (device.ProductId == productId))
                    systemPath = device.SystemPath;
            }
        }

		#endregion

        #region Fields
        //==========================================================================================
        // Fields
        //==========================================================================================

        private bool _isDisposed;

        private DeviceInfo _deviceId;

        #endregion

        #region Object Lifetime
        //==========================================================================================
        // Object Lifetime
        //==========================================================================================

        /// <summary></summary>
        public Device(string systemPath)
        {
            _deviceId = new DeviceInfo(systemPath);
        }

        /// <summary></summary>
        public Device(int vendorId, int productId)
        {
            _deviceId = new DeviceInfo(vendorId, productId);
        }

        /// <summary></summary>
        public Device(int vendorId, int productId, string serialNumber)
        {
            _deviceId = new DeviceInfo(vendorId, productId, serialNumber);
        }

        /// <summary></summary>
        public Device(DeviceInfo deviceId)
        {
            _deviceId = new DeviceInfo(deviceId);
        }

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary></summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!_isDisposed)
			{
				if (disposing)
				{
                    // \fixme
					// Nothing yet
				}
				_isDisposed = true;
			}
		}

		/// <summary></summary>
        ~Device()
		{
			Dispose(false);
		}

		/// <summary></summary>
		protected bool IsDisposed
		{
			get { return (_isDisposed); }
		}

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (_isDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed"));
		}

		#endregion

        #endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

        /// <summary></summary>
        public int VendorId
        {
            get { return (_deviceId.VendorId); }
        }

        /// <summary></summary>
        public string VendorIdString
        {
            get { return (_deviceId.VendorIdString); }
        }

        /// <summary></summary>
        public int ProductId
        {
            get { return (_deviceId.ProductId); }
        }

        /// <summary></summary>
        public string ProductIdString
        {
            get { return (_deviceId.ProductIdString); }
        }

        /// <summary></summary>
        public string Manufacturer
        {
            get { return (_deviceId.Manufacturer); }
        }

        /// <summary></summary>
        public string Product
        {
            get { return (_deviceId.Product); }
        }

        /// <summary></summary>
        public string SerialNumber
        {
            get { return (_deviceId.SerialNumber); }
        }

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
