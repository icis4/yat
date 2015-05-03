//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.10
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2015 Matthias Kläy.
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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Microsoft.Win32.SafeHandles;

using MKY.Windows.Forms;

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
		/// \todo: This method currently only works for HID devices. Find a HID independent way to retrieve VID/PID.
		/// </remarks>
		public static DeviceInfo[] GetDevices()
		{
			return (GetDevicesFromGuid(GetGuidFromDeviceClass(DeviceClass.Any)));
		}

		/// <summary>
		/// Returns an array of all USB devices of the given class currently available on the system.
		/// </summary>
		/// <remarks>
		/// \todo: This method currently only works for HID devices. Find a HID independent way to retrieve VID/PID.
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
		/// \todo: This method currently only works for HID devices. Find a HID independent way to retrieve VID/PID.
		/// </remarks>
		/// <param name="classGuid">GUID of a class of devices.</param>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid", Justification = "Why not? 'Guid' not only is a type, but also emphasizes a purpose.")]
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
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vid", Justification = "'VID' is a common term in USB.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pid", Justification = "'PID' is a common term in USB.")]
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

			vendorId  = 0;
			productId = 0;
			return (false);
		}

		private static bool GetVidAndPidFromHandle(SafeFileHandle deviceHandle, out int vendorId, out int productId)
		{
			// Set the size property of attributes to the number of bytes in the structure.
			Win32.Hid.NativeTypes.HIDD_ATTRIBUTES attributes = new Win32.Hid.NativeTypes.HIDD_ATTRIBUTES();
			attributes.Size = (UInt32)Marshal.SizeOf(attributes);

			if (Win32.Hid.NativeMethods.HidD_GetAttributes(deviceHandle, ref attributes))
			{
				vendorId  = attributes.VendorID;
				productId = attributes.ProductID;
				return (true);
			}

			vendorId  = 0;
			productId = 0;
			return (false);
		}

		/// <summary>
		/// Returns manufacturer, product and serial strings of a given path.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static bool GetStringsFromPath(string path, out string manufacturer, out string product, out string serial)
		{
			SafeFileHandle deviceHandle;
			if (Win32.Hid.CreateSharedQueryOnlyDeviceHandle(path, out deviceHandle))
			{
				try
				{
					if (GetStringsFromHandle(deviceHandle, out manufacturer, out product, out serial))
						return (true);
				}
				finally
				{
					deviceHandle.Close();
				}
			}

			manufacturer = "";
			product      = "";
			serial       = "";
			return (false);
		}

		private static bool GetStringsFromHandle(SafeFileHandle deviceHandle, out string manufacturer, out string product, out string serial)
		{
			Win32.Hid.GetManufacturerString(deviceHandle, out manufacturer);
			Win32.Hid.GetProductString(deviceHandle, out product);
			Win32.Hid.GetSerialString(deviceHandle, out serial);
			return (true);
		}

		/// <summary>
		/// Returns the information of the device with the given path,
		/// or <c>null</c> if no device could be found on the given path.
		/// </summary>
		public static DeviceInfo GetDeviceInfoFromPath(string path)
		{
			int vendorId, productId;
			string manufacturer, product, serial;

			if (GetDeviceInfoFromPath(path, out vendorId, out productId, out manufacturer, out product, out serial))
				return (new DeviceInfo(path, vendorId, productId, manufacturer, product, serial));
			else
				return (null);
		}

		/// <summary>
		/// Returns the information of the device with the given path.
		/// </summary>
		/// <remarks>
		/// \todo: This method currently only works for HID devices. Find a HID independent way to retrieve VID/PID.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static bool GetDeviceInfoFromPath(string path, out int vendorId, out int productId, out string manufacturer, out string product, out string serial)
		{
			SafeFileHandle deviceHandle;
			if (Win32.Hid.CreateSharedQueryOnlyDeviceHandle(path, out deviceHandle))
			{
				try
				{
					if (GetVidAndPidFromHandle(deviceHandle, out vendorId, out productId))
						if (GetStringsFromHandle(deviceHandle, out manufacturer, out product, out serial))
							return (true);
				}
				finally
				{
					deviceHandle.Close();
				}
			}

			vendorId     = 0;
			productId    = 0;
			manufacturer = "";
			product      = "";
			serial       = "";
			return (false);
		}

		/// <summary>
		/// Returns the information of the device with the given VID and PID,
		/// or <c>null</c> if no device could be found on the given path.
		/// </summary>
		/// <remarks>
		/// If multiple devices with the same VID and PID are connected to the system, the first device is returned.
		/// </remarks>
		/// <returns>Retrieved device info, or <c>null</c> if no appropriate device was found.</returns>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vid", Justification = "'VID' is a common term in USB.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pid", Justification = "'PID' is a common term in USB.")]
		public static DeviceInfo GetDeviceInfoFromVidAndPid(int vendorId, int productId)
		{
			string path, manufacturer, product, serial;

			if (GetDeviceInfoFromVidAndPid(vendorId, productId, out path, out manufacturer, out product, out serial))
				return (new DeviceInfo(path, vendorId, productId, manufacturer, product, serial));
			else
				return (null);
		}

		/// <summary>
		/// Returns the information of the device with the given VID and PID.
		/// </summary>
		/// <remarks>
		/// If multiple devices with the same VID and PID are connected to the system, the first device is returned.
		/// </remarks>
		/// <param name="vendorId">Given VID.</param>
		/// <param name="productId">Given PID.</param>
		/// <param name="path">Retrieved system path, or "" if no appropriate device was found.</param>
		/// <param name="manufacturer">Retrieved manufacturer, or "" if no appropriate device was found.</param>
		/// <param name="product">Retrieved product, or "" if no appropriate device was found.</param>
		/// <param name="serial">Retrieved serial string, or "" if no appropriate device was found.</param>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vid", Justification = "'VID' is a common term in USB.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pid", Justification = "'PID' is a common term in USB.")]
		public static bool GetDeviceInfoFromVidAndPid(int vendorId, int productId, out string path, out string manufacturer, out string product, out string serial)
		{
			foreach (DeviceInfo device in GetDevicesFromClass(DeviceClass.Hid))
			{
				if ((device.VendorId == vendorId) && (device.ProductId == productId))
				{
					path         = device.Path;
					manufacturer = device.Manufacturer;
					product      = device.Product;
					serial       = device.Serial;

					return (true);
				}
			}

			path         = "";
			manufacturer = "";
			product      = "";
			serial       = "";

			return (false);
		}

		/// <summary>
		/// Returns the information of the device with the given VID and PID and serial string.
		/// or <c>null</c> if no device could be found on the give path.
		/// </summary>
		/// <returns>Retrieved device info, or <c>null</c> if no appropriate device was found.</returns>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vid", Justification = "'VID' is a common term in USB.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pid", Justification = "'PID' is a common term in USB.")]
		public static DeviceInfo GetDeviceInfoFromVidAndPidAndSerial(int vendorId, int productId, string serial)
		{
			string path, manufacturer, product;

			if (GetDeviceInfoFromVidAndPidAndSerial(vendorId, productId, serial, out path, out manufacturer, out product))
				return (new DeviceInfo(path, vendorId, productId, manufacturer, product, serial));
			else
				return (null);
		}

		/// <summary>
		/// Returns the information of the device with the given VID and PID and serial string.
		/// </summary>
		/// <param name="vendorId">Given VID.</param>
		/// <param name="productId">Given PID.</param>
		/// <param name="serial">Given serial string.</param>
		/// <param name="path">Retrieved system path, or "" if no appropriate device was found.</param>
		/// <param name="manufacturer">Retrieved manufacturer, or "" if no appropriate device was found.</param>
		/// <param name="product">Retrieved product, or "" if no appropriate device was found.</param>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vid", Justification = "'VID' is a common term in USB.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pid", Justification = "'PID' is a common term in USB.")]
		public static bool GetDeviceInfoFromVidAndPidAndSerial(int vendorId, int productId, string serial, out string path, out string manufacturer, out string product)
		{
			foreach (DeviceInfo device in GetDevicesFromClass(DeviceClass.Hid))
			{
				if ((device.VendorId == vendorId) &&
					(device.ProductId == productId) &&
					(StringEx.EqualsOrdinalIgnoreCase(device.Serial, serial))) // Case-insensitive (i.e. Windows behaviour).
				{
					path         = device.Path;
					manufacturer = device.Manufacturer;
					product      = device.Product;

					return (true);
				}
			}

			path         = "";
			manufacturer = "";
			product      = "";

			return (false);
		}

		#endregion

		#region Static Methods > Device Notification
		//------------------------------------------------------------------------------------------
		// Static Methods > Device Notification
		//------------------------------------------------------------------------------------------

		private static NativeMessageHandler staticDeviceNotificationHandler;

		private static int    staticDeviceNotificationCounter; // = 0;
		private static IntPtr staticDeviceNotificationHandle = IntPtr.Zero;
		private static object staticDeviceNotificationSyncObj = new object();

		/// <summary>
		/// This method registers for static device notifications. These notifications will report
		/// whenever a device is physically connected or disconnected to the computer. Only one
		/// handler for these notifications is needed, therefore, only the first call to this
		/// method does have any effect.
		/// </summary>
		/// <remarks>
		/// \todo: Don't know the GUID for any USB device class. So only HID devices are detected.
		/// 
		/// \attention:
		/// This function also exists in the other USB classes. Changes here must also be applied there.
		/// </remarks>
		public static void RegisterStaticDeviceNotificationHandler(Guid classGuid)
		{
			lock (staticDeviceNotificationSyncObj)
			{
				// The first call to this method registers the notification.
				if (staticDeviceNotificationCounter == 0)
				{
					if (staticDeviceNotificationHandle != IntPtr.Zero)
						throw (new InvalidOperationException("Invalid state within USB Device object"));

					staticDeviceNotificationHandler = new NativeMessageHandler(StaticMessageCallback);
					Win32.DeviceManagement.RegisterDeviceNotificationHandle(staticDeviceNotificationHandler.Handle, classGuid, out staticDeviceNotificationHandle);
				}

				// Keep track of the register/unregister requests.
				if (staticDeviceNotificationCounter < int.MaxValue)
					staticDeviceNotificationCounter++;
			}
		}

		/// <remarks>
		/// \attention:
		/// This function also exists in the other USB classes. Changes here must also be applied there.
		/// </remarks>
		public static void UnregisterStaticDeviceNotificationHandler()
		{
			lock (staticDeviceNotificationSyncObj)
			{
				// Keep track of the register/unregister requests.
				staticDeviceNotificationCounter--;

				// The last call to this method unregisters the notification.
				if (staticDeviceNotificationCounter == 0)
				{
					if (staticDeviceNotificationHandle == IntPtr.Zero)
						throw (new InvalidOperationException("Invalid state within USB Device object"));

					Win32.DeviceManagement.UnregisterDeviceNotificationHandle(staticDeviceNotificationHandle);
					staticDeviceNotificationHandle = IntPtr.Zero;
				}

				// Ensure that decrement never results in negative values.
				if (staticDeviceNotificationCounter < 0)
					staticDeviceNotificationCounter = 0;
			}
		}

		/// <remarks>
		/// \attention:
		/// This function also exists in the other USB classes. Changes here must also be applied there.
		/// </remarks>
		private static void StaticMessageCallback(ref Message m)
		{
			DeviceEvent de = MessageToDeviceEvent(ref m);

			if ((de == DeviceEvent.Connected) ||
				(de == DeviceEvent.Disconnected))
			{
				string devicePath;
				if (Win32.DeviceManagement.DeviceChangeMessageToDevicePath(m, out devicePath))
				{
					switch (de)
					{
						case DeviceEvent.Connected:
						{
							DeviceEventArgs e = new DeviceEventArgs(DeviceClass.Any, new DeviceInfo(devicePath));

							Debug.WriteLine("USB device connected:");
							Debug.Indent();
							Debug.WriteLine("Path = " + devicePath);
							Debug.WriteLine("Info = " + e.DeviceInfo);
							Debug.Unindent();

							EventHelper.FireAsync(DeviceConnected, typeof(Device), e);
							break;
						}

						case DeviceEvent.Disconnected:
						{
							DeviceEventArgs e = new DeviceEventArgs(DeviceClass.Any, new DeviceInfo(devicePath, false));

							Debug.WriteLine("USB device disconnected:");
							Debug.Indent();
							Debug.WriteLine("Path = " + devicePath);
							Debug.Unindent();

							EventHelper.FireAsync(DeviceDisconnected, typeof(Device), e);
							break;
						}
					}
				}
			}
		}

		internal static DeviceEvent MessageToDeviceEvent(ref Message m)
		{
			if (m.Msg == (int)Win32.DeviceManagement.NativeConstants.WM_DEVICECHANGE)
			{
				Win32.DeviceManagement.NativeTypes.DBT e = (Win32.DeviceManagement.NativeTypes.DBT)m.WParam.ToInt32();
				switch (e)
				{
					case Win32.DeviceManagement.NativeTypes.DBT.DEVICEARRIVAL:
						return (DeviceEvent.Connected);

					case Win32.DeviceManagement.NativeTypes.DBT.DEVICEREMOVECOMPLETE:
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
		private Guid classGuid;
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
		/// Fired after an I/O error has occurred.
		/// </summary>
		public event EventHandler<ErrorEventArgs> IOError;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid", Justification = "Why not? 'Guid' not only is a type, but also emphasizes a purpose.")]
		protected Device(Guid classGuid, string path)
		{
			this.classGuid = classGuid; // The USB class GUID arg is forseen for future use.

			int vendorId, productId;
			string manufacturer, product, serial;
			if (GetDeviceInfoFromPath(path, out vendorId, out productId, out manufacturer, out product, out serial))
			{
				this.deviceInfo = new DeviceInfo(path, vendorId, productId, manufacturer, product, serial);
				Initialize();
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid", Justification = "Why not? 'Guid' not only is a type, but also emphasizes a purpose.")]
		protected Device(Guid classGuid, int vendorId, int productId)
		{
			this.classGuid = classGuid; // The USB class GUID arg is forseen for future use.

			this.deviceInfo = new DeviceInfo(vendorId, productId);
			Initialize();
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid", Justification = "Why not? 'Guid' not only is a type, but also emphasizes a purpose.")]
		protected Device(Guid classGuid, int vendorId, int productId, string serial)
		{
			this.classGuid = classGuid; // The USB class GUID arg is forseen for future use.

			this.deviceInfo = new DeviceInfo(vendorId, productId, serial);
			Initialize();
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid", Justification = "Why not? 'Guid' not only is a type, but also emphasizes a purpose.")]
		protected Device(Guid classGuid, DeviceInfo deviceInfo)
		{
			this.classGuid = classGuid; // The USB class GUID arg is forseen for future use.

			this.deviceInfo = new DeviceInfo(deviceInfo);
			Initialize();
		}

		/// <remarks>
		/// Constructor above creates device info and therefore also sets system path.
		/// </remarks>
		private void Initialize()
		{
			GetDeviceConnect();

			// Only attach handlers if this is an instance of the general USB device class.
			// If this instance is e.g. an HID device, handlers must be attached there.
			if (GetType() == typeof(Device))
				RegisterAndAttachStaticDeviceEventHandlers(this.classGuid);
		}

		/// <summary>
		/// Used to reinitialize the device in case of a reconnect.
		/// </summary>
		protected virtual void Reinitialize()
		{
			GetDeviceConnect();
		}

		/// <summary>
		/// Used to reinitialize the device in case of a reconnect.
		/// </summary>
		protected void Reinitialize(DeviceInfo deviceInfo)
		{
			this.deviceInfo = new DeviceInfo(deviceInfo);
			Reinitialize();
		}

		private void GetDeviceConnect()
		{
			SafeFileHandle deviceHandle;
			if (!string.IsNullOrEmpty(Path) && Win32.Hid.CreateSharedQueryOnlyDeviceHandle(Path, out deviceHandle))
			{
				// Getting a handle means the device is connected to the computer.
				deviceHandle.Close();
				this.isConnected = true;
			}
		}

		private void RegisterAndAttachStaticDeviceEventHandlers(Guid classGuid)
		{
			RegisterStaticDeviceNotificationHandler(classGuid);
			DeviceConnected    += new EventHandler<DeviceEventArgs>(Device_DeviceConnected);
			DeviceDisconnected += new EventHandler<DeviceEventArgs>(Device_DeviceDisconnected);
		}

		private void DetachAndUnregisterStaticDeviceEventHandlers()
		{
			DeviceConnected    -= new EventHandler<DeviceEventArgs>(Device_DeviceConnected);
			DeviceDisconnected -= new EventHandler<DeviceEventArgs>(Device_DeviceDisconnected);
			UnregisterStaticDeviceNotificationHandler();
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
				// In any case, ensure that the static event handlers get detached:
				DetachAndUnregisterStaticDeviceEventHandlers();

				// Dispose of managed resources if requested:
				if (disposing)
				{
				}

				// Set state to disposed:
				this.isDisposed = true;
			}
		}

		/// <summary></summary>
		~Device()
		{
			Dispose(false);

			System.Diagnostics.Debug.WriteLine("The finalizer of '" + GetType().FullName + "' should have never been called! Ensure to call Dispose()!");
		}

		/// <summary></summary>
		public bool IsDisposed
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
		protected virtual string Path
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

		/// <summary>
		/// Returns the complete device info. To read a specific device property, use the property
		/// members below.
		/// </summary>
		public virtual string InfoString
		{
			get { return (this.deviceInfo.ToString()); }
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
		public virtual string Serial
		{
			get { return (this.deviceInfo.Serial); }
		}

		#endregion

		#region Properties > IDevice
		//------------------------------------------------------------------------------------------
		// Properties > IDevice
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Indicates whether the device is physically connected to the computer, i.e. the USB cable
		/// is connected to both computer and device.
		/// </summary>
		/// <remarks>
		/// The Win32 API 'RegisterDeviceNotification' fires 'Connected' or 'Disconnected' events
		/// in case a device is attached or removed from the computer. The <see cref="IsConnected"/>
		/// property relates to the state of these events.
		/// </remarks>
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

		/// <remarks>
		/// \attention:
		/// This function similarly exists in the other USB classes. Changes here may also be applied there.
		/// </remarks>
		private void Device_DeviceConnected(object sender, DeviceEventArgs e)
		{
			if (Info == e.DeviceInfo)
			{
				// Force reinitialize with new device info.
				Reinitialize(e.DeviceInfo);

				OnConnected(EventArgs.Empty);
			}
		}

		/// <remarks>
		/// \attention:
		/// This function similarly exists in the other USB classes. Changes here may also be applied there.
		/// </remarks>
		private void Device_DeviceDisconnected(object sender, DeviceEventArgs e)
		{
			if (Info == e.DeviceInfo)
				OnDisconnected(EventArgs.Empty);
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
		protected virtual void OnIOError(ErrorEventArgs e)
		{
			EventHelper.FireSync<ErrorEventArgs>(IOError, this, e);
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

		#region Object Members > Extensions
		//------------------------------------------------------------------------------------------
		// Object Members > Extensions
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Returns a string describing the USB device in a short form.
		/// </summary>
		public virtual string ToShortString()
		{
			return (this.deviceInfo.ToShortString());
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
