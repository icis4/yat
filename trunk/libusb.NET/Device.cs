//==================================================================================================
// $URL: https://y-a-terminal.svn.sourceforge.net/svnroot/y-a-terminal/trunk/MKY.IO.Ports/Properties/AssemblyInfo.cs $
// $Author: maettu_this $
// $Date: 2010-02-23 00:18:29 +0100 (Di, 23 Feb 2010) $
// $Revision: 254 $
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2004 Mike Krüger.
// Copyright © 2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Text;

namespace libusb.NET
{
	public class Device : IDisposable
	{
        #region Static Methods
        //==========================================================================================
        // Static Methods
        //==========================================================================================

        public static Native.libusb_device[] GetAvailableDevices()
        {
            Native.libusb_device[] devices;
            int ret = Native.Functions.libusb_get_device_list(out devices);
            if (ret < 0)
                throw (new UsbNativeMethodCallException("libusb_reset_device", "Couldn't reset device", ret));

            return (devices);
        }

        #endregion

        #region Fields
        //==========================================================================================
        // Fields
        //==========================================================================================

        private bool _isDisposed;

        Native.libusb_device _nativeDevice;
        Native.libusb_device_handle _nativeDeviceHandle;
        Native.libusb_device_descriptor _nativeDeviceDescriptor;
		
		int _timeout = 500;

        #endregion

        #region Object Lifetime
        //==========================================================================================
        // Object Lifetime
        //==========================================================================================

        public Device(int vid, int pid)
        {
            _nativeDeviceHandle = Native.Functions.libusb_open_device_with_vid_pid((ushort)vid, (ushort)pid);

            _nativeDevice = Native.Functions.libusb_get_device(_nativeDeviceHandle);

            _nativeDeviceDescriptor = new Native.libusb_device_descriptor();
            int ret = Native.Functions.libusb_get_device_descriptor(_nativeDevice, ref _nativeDeviceDescriptor);
            if (ret < 0)
                throw (new UsbNativeMethodCallException("libusb_get_device_descriptor", "Couldn't get device descriptor", ret));
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
                    Close();

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

        public int VendorId
        {
            get { return (_nativeDeviceDescriptor.idVendor); }
        }

        public string VendorIdString
        {
            get { return (VendorId.ToString("X4")); }
        }

        public int ProductId
        {
            get { return (_nativeDeviceDescriptor.idProduct); }
        }

        public string ProductIdString
        {
            get { return (ProductId.ToString("X4")); }
        }

        public string Manufacturer
        {
			get
            {
				AssertIsOpen();

				if (_nativeDeviceDescriptor.iManufacturer != 0)
                {
                    string s;
                    int ret = Native.Functions.libusb_get_string_descriptor_ascii(_nativeDeviceHandle, _nativeDeviceDescriptor.iManufacturer, out s);
					if (ret <= 0)
                        throw (new UsbNativeMethodCallException("libusb_get_string_descriptor_ascii", "Unable to retrieve manufacturer string", ret));

					return (s);
				}
				return null;
			}
		}
		
		public string Product
        {
            get
            {
                AssertIsOpen();

                if (_nativeDeviceDescriptor.iProduct != 0)
                {
                    string s;
                    int ret = Native.Functions.libusb_get_string_descriptor_ascii(_nativeDeviceHandle, _nativeDeviceDescriptor.iProduct, out s);
                    if (ret <= 0)
                        throw (new UsbNativeMethodCallException("libusb_get_string_descriptor_ascii", "Unable to retrieve product string", ret));

                    return (s);
                }
                return null;
            }
        }
		
		public string SerialNumber
        {
            get
            {
                AssertIsOpen();

                if (_nativeDeviceDescriptor.iSerialNumber != 0)
                {
                    string s;
                    int ret = Native.Functions.libusb_get_string_descriptor_ascii(_nativeDeviceHandle, _nativeDeviceDescriptor.iSerialNumber, out s);
                    if (ret <= 0)
                        throw (new UsbNativeMethodCallException("libusb_get_string_descriptor_ascii", "Unable to retrieve serial number", ret));

                    return (s);
                }
                return null;
            }
        }
		
        public int Timeout
        {
            get { return (_timeout); }
            set { _timeout = value; }
        }

        #endregion

        #region Public Methods
        //==========================================================================================
        // Public Methods
        //==========================================================================================

        public void ControlRead(RequestType requestType, byte request, int value, int index, ref byte[] data)
		{
			AssertIsOpen();
            Native.Functions.libusb_control_transfer(_nativeDeviceHandle, (byte)requestType, request, (ushort)value, (ushort)index, ref data, _timeout);
		}

		public void ControlReadStandardRequest(StandardRequest request, int value, int index, ref byte[] data)
		{
            ControlRead(RequestType.Standard, (byte)request, value, index, ref data);
		}

        public void BulkWrite(byte endpoint, byte[] data)
        {
            AssertIsOpen();
            int transferred;
            Native.Functions.libusb_bulk_transfer(_nativeDeviceHandle, endpoint, ref data, out transferred, _timeout);
        }

        public void BulkRead(byte endpoint, ref byte[] data)
        {
            AssertIsOpen();
            int transferred;
            Native.Functions.libusb_bulk_transfer(_nativeDeviceHandle, endpoint, ref data, out transferred, _timeout);
        }

        public void InterruptWrite(byte endpoint, byte[] data)
		{
			AssertIsOpen();
            int transferred;
            Native.Functions.libusb_interrupt_transfer(_nativeDeviceHandle, endpoint, ref data, out transferred, _timeout);
		}

        public void InterruptRead(byte endpoint, ref byte[] data)
		{
			AssertIsOpen();
            int transferred;
            Native.Functions.libusb_interrupt_transfer(_nativeDeviceHandle, endpoint, ref data, out transferred, _timeout);
		}

        #endregion

        #region Private Methods
        //==========================================================================================
        // Private Methods
        //==========================================================================================

        private void AssertIsOpen()
        {
            if ((_nativeDeviceHandle == null) || (_nativeDeviceDescriptor == null))
                throw (new UsbDeviceNotOpenException());
        }

        private void Reset()
        {
            if (_nativeDeviceHandle != null)
            {
                int ret = Native.Functions.libusb_reset_device(_nativeDeviceHandle);
                if (ret < 0)
                    throw (new UsbNativeMethodCallException("libusb_reset_device", "Couldn't reset device", ret));
            }
        }

        private void Close()
        {
            if (_nativeDeviceHandle != null)
            {
                Native.Functions.libusb_close(_nativeDeviceHandle);
                _nativeDeviceHandle = null;
            }
        }

        #endregion
    }
}

//==================================================================================================
// End of
// $URL: https://y-a-terminal.svn.sourceforge.net/svnroot/y-a-terminal/trunk/MKY.IO.Ports/Properties/AssemblyInfo.cs $
//==================================================================================================
