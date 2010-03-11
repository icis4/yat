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
using System.Globalization;
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
			{
				DeviceInfo device = GetDeviceInfoFromSystemPath(systemPath);
				if (device != null)
				{
					if (device.TryValidate())
						devices.Add(device);
				}
			}

			return (devices.ToArray());
		}

		/// <summary>
		/// Returns VID and PID of a given system path.
		/// </summary>
		public static bool GetVidAndPidFromSystemPath(string systemPath, out int vendorId, out int productId)
		{
			SafeFileHandle deviceHandle = Utilities.Win32.FileIO.CreateFile
				(
				systemPath,
				0,
				Utilities.Win32.FileIO.FILE_SHARE_READ | Utilities.Win32.FileIO.FILE_SHARE_WRITE,
				IntPtr.Zero,
				Utilities.Win32.FileIO.OPEN_EXISTING,
				0,
				0
				);

			if (!deviceHandle.IsInvalid)
			{
				try
				{
					// Set the size property of attributes to the number of bytes in the structure.
					Utilities.Win32.Hid.HIDD_ATTRIBUTES attributes = new Utilities.Win32.Hid.HIDD_ATTRIBUTES();
					attributes.Size = Marshal.SizeOf(attributes);

					if (Utilities.Win32.Hid.HidD_GetAttributes(deviceHandle, ref attributes))
					{
						vendorId = attributes.VendorID;
						productId = attributes.ProductID;
						return (true);
					}
				}
				finally
				{
					deviceHandle.Close();
				}
			}

			vendorId = 0;
			productId = 0;
			return (false);
		}

		/// <summary>
		/// Returns the information of the device with the given system path,
		/// or <c>null</c> if no device could be found on the give path.
		/// </summary>
		public static DeviceInfo GetDeviceInfoFromSystemPath(string systemPath)
		{
			int vendorId, productId;
			string manufacturer, product, serialNumber;

			if (GetDeviceInfoFromSystemPath(systemPath, out vendorId, out productId, out manufacturer, out product, out serialNumber))
				return (new DeviceInfo(systemPath, vendorId, productId, manufacturer, product, serialNumber));
			else
				return (null);
		}

		/// <summary>
		/// Returns the information of the device with the given system path.
		/// </summary>
		/// <remarks>
		/// \todo This method currently only works for HID devices. Find a HID independent way to retrieve VID/PID.
		/// </remarks>
		public static bool GetDeviceInfoFromSystemPath(string systemPath, out int vendorId, out int productId, out string manufacturer, out string product, out string serialNumber)
		{
			if (GetVidAndPidFromSystemPath(systemPath, out vendorId, out productId))
			{
				StringDescriptor.TryGetManufacturerString(systemPath, out manufacturer);
				StringDescriptor.TryGetProductString     (systemPath, out product);
				StringDescriptor.TryGetSerialNumberString(systemPath, out serialNumber);
				return (true);
			}

			vendorId = 0;
			productId = 0;
			manufacturer = "";
			product = "";
			serialNumber = "";
			return (false);
		}

		/// <summary>
		/// Returns the information of the device with the given VID and PID,
		/// or <c>null</c> if no device could be found on the give path.
		/// </summary>
		/// <remarks>
		/// If multiple devices with the same VID and PID are connected to the sytem, the first device is returned.
		/// </remarks>
		/// <returns>Retrieved device info, or <c>null</c> if no valable device found.</returns>
		public static DeviceInfo GetDeviceInfoFromVidAndPid(int vendorId, int productId)
		{
			string systemPath, manufacturer, product, serialNumber;

			if (GetDeviceInfoFromVidAndPid(vendorId, productId, out systemPath, out manufacturer, out product, out serialNumber))
				return (new DeviceInfo(systemPath, vendorId, productId, manufacturer, product, serialNumber));
			else
				return (null);
		}

		/// <summary>
		/// Returns the information of the device with the given VID and PID.
		/// </summary>
		/// <remarks>
		/// If multiple devices with the same VID and PID are connected to the sytem, the first device is returned.
		/// </remarks>
		/// <param name="vendorId">Given VID.</param>
		/// <param name="productId">Given PID.</param>
		/// <param name="systemPath">Retrieved system path, or "" if no valable device found.</param>
		/// <param name="manufacturer">Retrieved manufacturer, or "" if no valable device found.</param>
		/// <param name="product">Retrieved product, or "" if no valable device found.</param>
		/// <param name="serialNumber">Retrieved serial number, or "" if no valable device found.</param>
		public static bool GetDeviceInfoFromVidAndPid(int vendorId, int productId, out string systemPath, out string manufacturer, out string product, out string serialNumber)
		{
			DeviceCollection devices = new DeviceCollection();
			devices.FillWithAvailableDevices();

			foreach (DeviceInfo device in GetDevicesFromClass(DeviceClass.Hid))
			{
				if ((device.VendorId == vendorId) && (device.ProductId == productId))
				{
					systemPath = device.SystemPath;
					manufacturer = device.Manufacturer;
					product = device.Product;
					serialNumber = device.SerialNumber;

					return (true);
				}
			}

			systemPath = "";
			manufacturer = "";
			product = "";
			serialNumber = "";

			return (false);
		}

		/// <summary>
		/// Returns the information of the device with the given VID and PID and serial number.
		/// or <c>null</c> if no device could be found on the give path.
		/// </summary>
		/// <returns>Retrieved device info, or <c>null</c> if no valable device found.</returns>
		public static DeviceInfo GetDeviceInfoFromVidAndPidAndSerial(int vendorId, int productId, string serialNumber)
		{
			string systemPath, manufacturer, product;

			if (GetDeviceInfoFromVidAndPidAndSerial(vendorId, productId, serialNumber, out systemPath, out manufacturer, out product))
				return (new DeviceInfo(systemPath, vendorId, productId, manufacturer, product, serialNumber));
			else
				return (null);
		}

		/// <summary>
		/// Returns the information of the device with the given VID and PID and serial number.
		/// </summary>
		/// <param name="vendorId">Given VID.</param>
		/// <param name="productId">Given PID.</param>
		/// <param name="serialNumber">Given serial number.</param>
		/// <param name="systemPath">Retrieved system path, or "" if no valable device found.</param>
		/// <param name="manufacturer">Retrieved manufacturer, or "" if no valable device found.</param>
		/// <param name="product">Retrieved product, or "" if no valable device found.</param>
		public static bool GetDeviceInfoFromVidAndPidAndSerial(int vendorId, int productId, string serialNumber, out string systemPath, out string manufacturer, out string product)
		{
			DeviceCollection devices = new DeviceCollection();
			devices.FillWithAvailableDevices();

			foreach (DeviceInfo device in GetDevicesFromClass(DeviceClass.Hid))
			{
				if ((device.VendorId == vendorId) && (device.ProductId == productId) && (device.SerialNumber == serialNumber))
				{
					systemPath = device.SystemPath;
					manufacturer = device.Manufacturer;
					product = device.Product;

					return (true);
				}
			}

			systemPath = "";
			manufacturer = "";
			product = "";

			return (false);
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
			int vendorId, productId;
			string manufacturer, product, serialNumber;
			Device.GetDeviceInfoFromSystemPath(systemPath, out vendorId, out productId, out manufacturer, out product, out serialNumber);
			_deviceId = new DeviceInfo(systemPath, vendorId, productId, manufacturer, product, serialNumber);
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
