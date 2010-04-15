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
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using Microsoft.Win32.SafeHandles;

using MKY.Utilities.Event;
using MKY.Utilities.Windows.Forms;

#endregion

namespace MKY.IO.Usb
{
	/// <summary>
	/// Encapsulates functions and properties that are common to all USB devices.
	/// </summary>
	public abstract class Device : IDisposable, IDeviceInfo, IDevice
	{
		#region Static Events
		//==========================================================================================
		// Static Events
		//==========================================================================================

		/// <summary></summary>
		public static event EventHandler<DeviceEventArgs> DeviceConnected;

		/// <summary></summary>
		public static event EventHandler<DeviceEventArgs> DeviceDisconnected;

		#endregion

		#region Static Lifetime
		//==========================================================================================
		// Static Lifetime
		//==========================================================================================

		static Device()
		{
			RegisterStaticDeviceNotificationHandler();
		}

		// \todo 2010-03-21 / mky
		// Properly unregister without relying on garbage collection
		//
		//static ~Device()
		//{
		//	UnregisterStaticDeviceNotificationHandler();
		//}

		#endregion

		#region Static Methods
		//==========================================================================================
		// Static Methods
		//==========================================================================================

		#region Static Methods > Device Enummeration
		//------------------------------------------------------------------------------------------
		// Static Methods > Device Enummeration
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Returns the GUID for the given device class.
		/// </summary>
		/// <param name="deviceClass">USB device class.</param>
		public static Guid GetGuidFromDeviceClass(DeviceClass deviceClass)
		{
			switch (deviceClass)
			{
				case DeviceClass.Hid: return (SerialHidDevice.HidGuid);
				default:              return (Guid.Empty);
			}
		}

		/// <summary>
		/// Returns an array of all USB devices of the given class currently available on the system.
		/// </summary>
		/// <remarks>
		/// \todo This method currently only works for HID devices. Find a HID independent way to retrieve VID/PID.
		/// </remarks>
		public static DeviceInfo[] GetDevices()
		{
			return (GetDevicesFromGuid(GetGuidFromDeviceClass(DeviceClass.Any)));
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
			List<DeviceInfo> l = new List<DeviceInfo>();

			foreach (string path in Win32.DeviceManagement.GetDevicesFromGuid(classGuid))
			{
				DeviceInfo device = GetDeviceInfoFromPath(path);
				if (device != null)
					l.Add(device);
			}

			return (l.ToArray());
		}

		#endregion

		#region Static Methods > Device Info
		//------------------------------------------------------------------------------------------
		// Static Methods > Device Info
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Returns VID and PID of a given path.
		/// </summary>
		public static bool GetVidAndPidFromPath(string path, out int vendorId, out int productId)
		{
			SafeFileHandle deviceHandle;
			if (Win32.Hid.CreateSharedQueryOnlyDeviceHandle(path, out deviceHandle))
			{
				try
				{
					if (GetVidAndPidFromHandle(deviceHandle, out vendorId, out productId))
						return (true);
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

		private static bool GetVidAndPidFromHandle(SafeFileHandle deviceHandle, out int vendorId, out int productId)
		{
			// Set the size property of attributes to the number of bytes in the structure.
			Win32.Hid.Native.HIDD_ATTRIBUTES attributes = new Win32.Hid.Native.HIDD_ATTRIBUTES();
			attributes.Size = Marshal.SizeOf(attributes);

			if (Win32.Hid.Native.HidD_GetAttributes(deviceHandle, ref attributes))
			{
				vendorId = attributes.VendorID;
				productId = attributes.ProductID;
				return (true);
			}

			vendorId = 0;
			productId = 0;
			return (false);
		}

		/// <summary>
		/// Returns manufacturer, product and serial number strings of a given path.
		/// </summary>
		public static bool GetStringsFromPath(string path, out string manufacturer, out string product, out string serialNumber)
		{
			SafeFileHandle deviceHandle;
			if (Win32.Hid.CreateSharedQueryOnlyDeviceHandle(path, out deviceHandle))
			{
				try
				{
					if (GetStringsFromHandle(deviceHandle, out manufacturer, out product, out serialNumber))
						return (true);
				}
				finally
				{
					deviceHandle.Close();
				}
			}

			manufacturer = "";
			product = "";
			serialNumber = "";
			return (false);
		}

		private static bool GetStringsFromHandle(SafeFileHandle deviceHandle, out string manufacturer, out string product, out string serialNumber)
		{
			Win32.Hid.GetManufacturerString(deviceHandle, out manufacturer);
			Win32.Hid.GetProductString(deviceHandle, out product);
			Win32.Hid.GetSerialNumberString(deviceHandle, out serialNumber);
			return (true);
		}

		/// <summary>
		/// Returns the information of the device with the given path,
		/// or <c>null</c> if no device could be found on the given path.
		/// </summary>
		public static DeviceInfo GetDeviceInfoFromPath(string path)
		{
			int vendorId, productId;
			string manufacturer, product, serialNumber;

			if (GetDeviceInfoFromPath(path, out vendorId, out productId, out manufacturer, out product, out serialNumber))
				return (new DeviceInfo(path, vendorId, productId, manufacturer, product, serialNumber));
			else
				return (null);
		}

		/// <summary>
		/// Returns the information of the device with the given path.
		/// </summary>
		/// <remarks>
		/// \todo This method currently only works for HID devices. Find a HID independent way to retrieve VID/PID.
		/// </remarks>
		public static bool GetDeviceInfoFromPath(string path, out int vendorId, out int productId, out string manufacturer, out string product, out string serialNumber)
		{
			SafeFileHandle deviceHandle;
			if (Win32.Hid.CreateSharedQueryOnlyDeviceHandle(path, out deviceHandle))
			{
				try
				{
					if (GetVidAndPidFromHandle(deviceHandle, out vendorId, out productId))
						if (GetStringsFromHandle(deviceHandle, out manufacturer, out product, out serialNumber))
							return (true);
				}
				finally
				{
					deviceHandle.Close();
				}
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
		/// or <c>null</c> if no device could be found on the given path.
		/// </summary>
		/// <remarks>
		/// If multiple devices with the same VID and PID are connected to the sytem, the first device is returned.
		/// </remarks>
		/// <returns>Retrieved device info, or <c>null</c> if no valable device found.</returns>
		public static DeviceInfo GetDeviceInfoFromVidAndPid(int vendorId, int productId)
		{
			string path, manufacturer, product, serialNumber;

			if (GetDeviceInfoFromVidAndPid(vendorId, productId, out path, out manufacturer, out product, out serialNumber))
				return (new DeviceInfo(path, vendorId, productId, manufacturer, product, serialNumber));
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
		/// <param name="path">Retrieved system path, or "" if no valable device found.</param>
		/// <param name="manufacturer">Retrieved manufacturer, or "" if no valable device found.</param>
		/// <param name="product">Retrieved product, or "" if no valable device found.</param>
		/// <param name="serialNumber">Retrieved serial number, or "" if no valable device found.</param>
		public static bool GetDeviceInfoFromVidAndPid(int vendorId, int productId, out string path, out string manufacturer, out string product, out string serialNumber)
		{
			DeviceCollection devices = new DeviceCollection();
			devices.FillWithAvailableDevices();

			foreach (DeviceInfo device in GetDevicesFromClass(DeviceClass.Hid))
			{
				if ((device.VendorId == vendorId) && (device.ProductId == productId))
				{
					path = device.Path;
					manufacturer = device.Manufacturer;
					product = device.Product;
					serialNumber = device.SerialNumber;

					return (true);
				}
			}

			path = "";
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
			string path, manufacturer, product;

			if (GetDeviceInfoFromVidAndPidAndSerial(vendorId, productId, serialNumber, out path, out manufacturer, out product))
				return (new DeviceInfo(path, vendorId, productId, manufacturer, product, serialNumber));
			else
				return (null);
		}

		/// <summary>
		/// Returns the information of the device with the given VID and PID and serial number.
		/// </summary>
		/// <param name="vendorId">Given VID.</param>
		/// <param name="productId">Given PID.</param>
		/// <param name="serialNumber">Given serial number.</param>
		/// <param name="path">Retrieved system path, or "" if no valable device found.</param>
		/// <param name="manufacturer">Retrieved manufacturer, or "" if no valable device found.</param>
		/// <param name="product">Retrieved product, or "" if no valable device found.</param>
		public static bool GetDeviceInfoFromVidAndPidAndSerial(int vendorId, int productId, string serialNumber, out string path, out string manufacturer, out string product)
		{
			DeviceCollection devices = new DeviceCollection();
			devices.FillWithAvailableDevices();

			foreach (DeviceInfo device in GetDevicesFromClass(DeviceClass.Hid))
			{
				if ((device.VendorId == vendorId) && (device.ProductId == productId) && (device.SerialNumber == serialNumber))
				{
					path = device.Path;
					manufacturer = device.Manufacturer;
					product = device.Product;

					return (true);
				}
			}

			path = "";
			manufacturer = "";
			product = "";

			return (false);
		}

		#endregion

		#region Static Methods > Device Notification
		//------------------------------------------------------------------------------------------
		// Static Methods > Device Notification
		//------------------------------------------------------------------------------------------

		//private static NativeMessageHandler staticDeviceNotificationWindow = new NativeMessageHandler(StaticDeviceNotificationHandler);
		//private static IntPtr staticDeviceNotificationHandle = IntPtr.Zero;

		/// <remarks>
		/// \todo Don't know how to retrieve the GUID for any USB device class. So only HID devices are detected.
		/// </remarks>
		private static void RegisterStaticDeviceNotificationHandler()
		{
			//Win32.DeviceManagement.RegisterDeviceNotificationHandle(staticDeviceNotificationWindow.Handle, HidDevice.HidGuid, ref staticDeviceNotificationHandle);
		}

		private static void UnregisterStaticDeviceNotificationHandler()
		{
			//Win32.DeviceManagement.UnregisterDeviceNotificationHandle(staticDeviceNotificationHandle);
		}

		private static void StaticDeviceNotificationHandler(ref Message m)
		{
			DeviceEvent de = MessageToDeviceEvent(ref m);

			if ((de == DeviceEvent.Connected) ||
				(de == DeviceEvent.Disconnected))
			{
				string devicePath;
				if (Win32.DeviceManagement.DeviceChangeMessageToDevicePath(m, out devicePath))
				{
					DeviceEventArgs e = new DeviceEventArgs(DeviceClass.Any, devicePath);
					switch (de)
					{
						case DeviceEvent.Connected:
							Debug.WriteLine("USB device connected: " + devicePath + ".");
							EventHelper.FireAsync(DeviceConnected, typeof(Device), e);
							break;

						case DeviceEvent.Disconnected:
							Debug.WriteLine("USB device disconnected: " + devicePath + ".");
							EventHelper.FireAsync(DeviceDisconnected, typeof(Device), e);
							break;
					}
				}
			}
		}

		internal static DeviceEvent MessageToDeviceEvent(ref Message m)
		{
			if (m.Msg == (int)Win32.DeviceManagement.Native.WM_DEVICECHANGE)
			{
				Win32.DeviceManagement.Native.DBT e = (Win32.DeviceManagement.Native.DBT)m.WParam.ToInt32();
				switch (e)
				{
					case Win32.DeviceManagement.Native.DBT.DEVICEARRIVAL:
						return (DeviceEvent.Connected);

					case Win32.DeviceManagement.Native.DBT.DEVICEREMOVECOMPLETE:
						return (DeviceEvent.Disconnected);
				}
			}
			return (DeviceEvent.None);
		}

		#endregion

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isDisposed;

		private DeviceInfo deviceInfo;

		private bool isConnected;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary>
		/// Fired after the device has been connected or reconnected.
		/// </summary>
		public event EventHandler Connected;

		/// <summary>
		/// Fired after the device has been disconnected.
		/// </summary>
		public event EventHandler Disconnected;

		/// <summary>
		/// Fired after an error has occured.
		/// </summary>
		public event EventHandler<ErrorEventArgs> Error;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public Device(Guid classGuid, string path)
		{
			int vendorId, productId;
			string manufacturer, product, serialNumber;
			GetDeviceInfoFromPath(path, out vendorId, out productId, out manufacturer, out product, out serialNumber);
			this.deviceInfo = new DeviceInfo(path, vendorId, productId, manufacturer, product, serialNumber);
			Initialize();
		}

		/// <summary></summary>
		public Device(Guid classGuid, int vendorId, int productId)
		{
			this.deviceInfo = new DeviceInfo(vendorId, productId);
			Initialize();
		}

		/// <summary></summary>
		public Device(Guid classGuid, int vendorId, int productId, string serialNumber)
		{
			this.deviceInfo = new DeviceInfo(vendorId, productId, serialNumber);
			Initialize();
		}

		/// <summary></summary>
		public Device(Guid classGuid, DeviceInfo deviceInfo)
		{
			this.deviceInfo = new DeviceInfo(deviceInfo);
			Initialize();
		}

		private void Initialize()
		{
			SafeFileHandle deviceHandle;
			if (Win32.Hid.CreateSharedQueryOnlyDeviceHandle(SystemPath, out deviceHandle))
			{
				deviceHandle.Close();

				// Getting a handle means that the device is connected to the computer.
				this.isConnected = true;
			}

			AttachEventHandlers();
		}

		private void AttachEventHandlers()
		{
			DeviceConnected    += new EventHandler<DeviceEventArgs>(Device_DeviceConnected);
			DeviceDisconnected += new EventHandler<DeviceEventArgs>(Device_DeviceDisconnected);
		}

		private void DetachEventHandlers()
		{
			DeviceConnected    -= new EventHandler<DeviceEventArgs>(Device_DeviceConnected);
			DeviceDisconnected -= new EventHandler<DeviceEventArgs>(Device_DeviceDisconnected);
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
			if (!this.isDisposed)
			{
				if (disposing)
				{
					DetachEventHandlers();
				}
				this.isDisposed = true;
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
			get { return (this.isDisposed); }
		}

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (this.isDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed"));
		}

		#endregion

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		protected virtual string SystemPath
		{
			get { return (this.deviceInfo.Path); }
		}

		#region Properties > IDeviceInfo
		//------------------------------------------------------------------------------------------
		// Properties > IDeviceInfo
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Returns the complete device info. To read a specific device property, use the property
		/// members below.
		/// </summary>
		public virtual DeviceInfo Info
		{
			get { return (this.deviceInfo); }
		}

		/// <summary></summary>
		public virtual int VendorId
		{
			get { return (this.deviceInfo.VendorId); }
		}

		/// <summary></summary>
		public virtual string VendorIdString
		{
			get { return (this.deviceInfo.VendorIdString); }
		}

		/// <summary></summary>
		public virtual int ProductId
		{
			get { return (this.deviceInfo.ProductId); }
		}

		/// <summary></summary>
		public virtual string ProductIdString
		{
			get { return (this.deviceInfo.ProductIdString); }
		}

		/// <summary></summary>
		public virtual string Manufacturer
		{
			get { return (this.deviceInfo.Manufacturer); }
		}

		/// <summary></summary>
		public virtual string Product
		{
			get { return (this.deviceInfo.Product); }
		}

		/// <summary></summary>
		public virtual string SerialNumber
		{
			get { return (this.deviceInfo.SerialNumber); }
		}

		#endregion

		#region Properties > IDevice
		//------------------------------------------------------------------------------------------
		// Properties > IDevice
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Indicates whether the device is connected to the computer.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the device is connected to the computer; otherwise, <c>false</c>.
		/// </returns>
		public bool IsConnected
		{
			get { return (this.isConnected); }
		}

		#endregion

		#endregion

		#region Event Handling
		//==========================================================================================
		// Event Handling
		//==========================================================================================

		private void Device_DeviceConnected(object sender, DeviceEventArgs e)
		{
			if (this.deviceInfo.Path == e.DevicePath)
				OnConnected(new EventArgs());
		}

		private void Device_DeviceDisconnected(object sender, DeviceEventArgs e)
		{
			if (this.deviceInfo.Path == e.DevicePath)
				OnDisconnected(new EventArgs());
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnConnected(EventArgs e)
		{
			this.isConnected = true;
			EventHelper.FireSync(Connected, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisconnected(EventArgs e)
		{
			this.isConnected = false;
			EventHelper.FireSync(Disconnected, this, e);
		}

		/// <summary></summary>
		protected virtual void OnError(ErrorEventArgs e)
		{
			EventHelper.FireSync<ErrorEventArgs>(Error, this, e);
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Returns a string describing the USB device as accurate as possible.
		/// </summary>
		public override string ToString()
		{
			return (this.deviceInfo.ToString());
		}

		/// <summary>
		/// Returns a string describing the USB device in a short form.
		/// </summary>
		public virtual string ToShortString()
		{
			return (this.deviceInfo.ToShortString());
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
