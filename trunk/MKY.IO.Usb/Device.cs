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
        /// Returns an array of all USB devices of the given class currently available on the system.
        /// </summary>
        /// <param name="classGuid">GUID of a class of devices.</param>
        public static DeviceId[] GetDevicesFromGuid(Guid classGuid)
        {
            List<DeviceId> devices = new List<DeviceId>();

            foreach (string path in Utilities.Win32.DeviceManagement.GetDevicesFromGuid(classGuid))
            {
                SafeFileHandle hidHandle = Utilities.Win32.FileIO.CreateFile(path, 0,
                    Utilities.Win32.FileIO.FILE_SHARE_READ | Utilities.Win32.FileIO.FILE_SHARE_WRITE,
                    IntPtr.Zero, Utilities.Win32.FileIO.OPEN_EXISTING, 0, 0);

                if (!hidHandle.IsInvalid)
                {
                    // Set the size property of attributes to the number of bytes in the structure.
                    Utilities.Win32.Hid.HIDD_ATTRIBUTES attributes = new Utilities.Win32.Hid.HIDD_ATTRIBUTES();
                    attributes.Size = Marshal.SizeOf(attributes);

                    if (Utilities.Win32.Hid.HidD_GetAttributes(hidHandle, ref attributes))
                        devices.Add(new DeviceId(attributes.VendorID, attributes.ProductID));

                    hidHandle.Close();
                }
            }

            return (devices.ToArray());
        }

		#endregion

        #region Fields
        //==========================================================================================
        // Fields
        //==========================================================================================

        private bool _isDisposed;

        private DeviceId _deviceId;

        #endregion

        #region Object Lifetime
        //==========================================================================================
        // Object Lifetime
        //==========================================================================================

        /// <summary></summary>
        public Device(int vendorId, int productId)
        {
            _deviceId = new DeviceId(vendorId, productId);
        }

        /// <summary></summary>
        public Device(int vendorId, int productId, string serialNumber)
        {
            _deviceId = new DeviceId(vendorId, productId, serialNumber);
        }

        /// <summary></summary>
        public Device(DeviceId rhs)
        {
            _deviceId = new DeviceId(rhs);
        }

        /// <summary></summary>
        public Device(string path)
        {
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
        public string ManufacturerName
        {
            get { return (_deviceId.ManufacturerName); }
        }

        /// <summary></summary>
        public string ProductName
        {
            get { return (_deviceId.ProductName); }
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
