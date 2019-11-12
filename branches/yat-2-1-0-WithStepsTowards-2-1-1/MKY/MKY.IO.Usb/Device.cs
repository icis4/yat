//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.28 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2019 Matthias Kläy.
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

using MKY.Diagnostics;
using MKY.Windows.Forms;

#endregion

namespace MKY.IO.Usb
{
	/// <summary>
	/// Encapsulates functions and properties that are common to all USB devices.
	/// </summary>
	public abstract class Device : IDevice, IDeviceInfo, IDisposable, IDisposableEx
	{
		#region Static Events
		//==========================================================================================
		// Static Events
		//==========================================================================================

		/// <summary>
		/// Occurs when an USB device is connected to the computer.
		/// </summary>
		public static event EventHandler<DeviceEventArgs> DeviceConnected;

		/// <summary>
		/// Occurs when an USB device is disconnected from the computer.
		/// </summary>
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
				case DeviceClass.Hid: return (HidDevice.HidGuid);
				default:              return (Guid.Empty);
			}
		}

		/// <summary>
		/// Returns an array of all USB devices of the given class currently available on the system.
		/// </summary>
		/// <remarks>
		/// \todo: This method currently only works for HID devices. Find a HID independent way to retrieve VID/PID.
		/// </remarks>
		/// <param name="retrieveStringsFromDevice">Enable or disable string retrieval from device.</param>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static DeviceInfo[] GetDevices(bool retrieveStringsFromDevice = true)
		{
			return (GetDevicesFromGuid(GetGuidFromDeviceClass(DeviceClass.Any), retrieveStringsFromDevice));
		}

		/// <summary>
		/// Returns an array of all USB devices of the given class currently available on the system.
		/// </summary>
		/// <remarks>
		/// \todo: This method currently only works for HID devices. Find a HID independent way to retrieve VID/PID.
		/// </remarks>
		/// <param name="deviceClass">USB device class.</param>
		/// <param name="retrieveStringsFromDevice">Enable or disable string retrieval from device.</param>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static DeviceInfo[] GetDevicesFromClass(DeviceClass deviceClass, bool retrieveStringsFromDevice = true)
		{
			return (GetDevicesFromGuid(GetGuidFromDeviceClass(deviceClass), retrieveStringsFromDevice));
		}

		/// <summary>
		/// Returns an array of all USB devices of the given class currently available on the system.
		/// </summary>
		/// <remarks>
		/// \todo: This method currently only works for HID devices. Find a HID independent way to retrieve VID/PID.
		/// </remarks>
		/// <param name="classGuid">GUID of a class of devices.</param>
		/// <param name="retrieveStringsFromDevice">Enable or disable string retrieval from device.</param>
		/// <param name="ignoreDuplicates">Ignore duplicated entries in WMI.</param>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid", Justification = "'ClassGuid' is the official term, even WMI uses it.")]
		public static DeviceInfo[] GetDevicesFromGuid(Guid classGuid, bool retrieveStringsFromDevice = true, bool ignoreDuplicates = true)
		{
			var paths = Win32.DeviceManagement.GetDevicesFromGuid(classGuid);
			var l = new List<DeviceInfo>(paths.Length); // Preset the required capacity to improve memory management.

			foreach (var path in paths)
			{
				var device = GetDeviceInfoFromPath(path, retrieveStringsFromDevice);
				if (device != null)
				{
					if (ignoreDuplicates)
					{
						var isAlreadyContained = false;

						foreach (var item in l)
						{
							if (item.EqualsVidPidManufacturerProductSerial(device))
							{
								isAlreadyContained = true;
								break;
							}
						}

						if (!isAlreadyContained) // Prevent duplicates since WMI may contain the same device multiple times on different "cols".
							l.Add(device);
					}
					else
					{
						l.Add(device);
					}
				}
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
		public static bool GetVidPidFromPath(string path, out int vendorId, out int productId)
		{
			SafeFileHandle deviceHandle;
			if (Win32.Hid.CreateSharedQueryOnlyDeviceHandle(path, out deviceHandle))
			{
				try
				{
					if (GetVidPidFromHandle(deviceHandle, out vendorId, out productId))
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

		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Using explicit types to emphasize the type declared by the native element.")]
		private static bool GetVidPidFromHandle(SafeFileHandle deviceHandle, out int vendorId, out int productId)
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
					GetStringsFromHandle(deviceHandle, out manufacturer, out product, out serial);
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

		/// <remarks>
		/// A device may not define any or some strings (e.g. no serial string).
		/// Therefore, simply try to retrieve the strings in best-effort manner.
		/// </remarks>
		private static void GetStringsFromHandle(SafeFileHandle deviceHandle, out string manufacturer, out string product, out string serial)
		{
			Win32.Hid.GetManufacturerString(deviceHandle, out manufacturer);
			Win32.Hid.GetProductString(deviceHandle, out product);
			Win32.Hid.GetSerialString(deviceHandle, out serial);
		}

		/// <summary>
		/// Returns the information of the device with the given path;
		/// or <c>null</c> if no device could be found on the given path.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static DeviceInfo GetDeviceInfoFromPath(string path, bool retrieveStringsFromDevice = true)
		{
			if (retrieveStringsFromDevice)
			{
				int vendorId, productId;
				string manufacturer, product, serial;

				if (GetDeviceInfoFromPath(path, out vendorId, out productId, out manufacturer, out product, out serial))
					return (new DeviceInfo(path, vendorId, productId, manufacturer, product, serial));
				else
					return (null);
			}
			else
			{
				int vendorId, productId;

				if (GetDeviceInfoFromPath(path, out vendorId, out productId))
					return (new DeviceInfo(path, vendorId, productId));
				else
					return (null);
			}
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
		public static bool GetDeviceInfoFromPath(string path, out int vendorId, out int productId)
		{
			string manufacturer, product, serial;
			return (GetDeviceInfoFromPath(path, false, out vendorId, out productId, out manufacturer, out product, out serial));
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
			return (GetDeviceInfoFromPath(path, true, out vendorId, out productId, out manufacturer, out product, out serial));
		}

		/// <summary>
		/// Returns the information of the device with the given path.
		/// </summary>
		/// <remarks>
		/// \todo: This method currently only works for HID devices. Find a HID independent way to retrieve VID/PID.
		/// </remarks>
		private static bool GetDeviceInfoFromPath(string path, bool retrieveStringsFromDevice, out int vendorId, out int productId, out string manufacturer, out string product, out string serial)
		{
			SafeFileHandle deviceHandle;
			if (Win32.Hid.CreateSharedQueryOnlyDeviceHandle(path, out deviceHandle))
			{
				try
				{
					if (GetVidPidFromHandle(deviceHandle, out vendorId, out productId))
					{
						if (retrieveStringsFromDevice)
						{
							GetStringsFromHandle(deviceHandle, out manufacturer, out product, out serial);
							return (true);
						}
						else
						{
							manufacturer = "";
							product      = "";
							serial       = "";
							return (true);
						}
					}
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
		/// Returns the information of the device with the given VID and PID;
		/// or <c>null</c> if no device could be found on the given path.
		/// </summary>
		/// <remarks>
		/// If multiple devices with the same VID and PID are connected to the system, the first device is returned.
		/// </remarks>
		/// <returns>Retrieved device info; or <c>null</c> if no appropriate device was found.</returns>
		public static DeviceInfo GetDeviceInfoFromVidPid(int vendorId, int productId)
		{
			string path, manufacturer, product, serial;

			if (GetDeviceInfoFromVidPid(vendorId, productId, out path, out manufacturer, out product, out serial))
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
		public static bool GetDeviceInfoFromVidPid(int vendorId, int productId, out string path, out string manufacturer, out string product, out string serial)
		{
			foreach (var di in GetDevicesFromClass(DeviceClass.Hid))
			{
				if (di.EqualsVidPid(vendorId, productId))
				{
					path         = di.Path;
					manufacturer = di.Manufacturer;
					product      = di.Product;
					serial       = di.Serial;

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
		/// Returns the information of the device with the given VID and PID and serial string;
		/// or <c>null</c> if no device could be found on the give path.
		/// </summary>
		/// <returns>Retrieved device info; or <c>null</c> if no appropriate device was found.</returns>
		public static DeviceInfo GetDeviceInfoFromVidPidSerial(int vendorId, int productId, string serial)
		{
			string path, manufacturer, product;

			if (GetDeviceInfoFromVidPidSerial(vendorId, productId, serial, out path, out manufacturer, out product))
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
		public static bool GetDeviceInfoFromVidPidSerial(int vendorId, int productId, string serial, out string path, out string manufacturer, out string product)
		{
			foreach (var di in GetDevicesFromClass(DeviceClass.Hid))
			{
				if (di.EqualsVidPidSerial(vendorId, productId, serial))
				{
					path         = di.Path;
					manufacturer = di.Manufacturer;
					product      = di.Product;

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
		/// Attention:
		/// This function also exists in the other USB classes.
		/// Changes here must be applied there too.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid", Justification = "'ClassGuid' is the official term, even WMI uses it.")]
		public static bool RegisterStaticDeviceNotificationHandler(Guid classGuid)
		{
			bool success = false;

			lock (staticDeviceNotificationSyncObj)
			{
				// The first call to this method registers the notification:
				if (staticDeviceNotificationCounter == 0)
				{
					if (staticDeviceNotificationHandle != IntPtr.Zero)
						throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "Invalid state within USB device object!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

					if (NativeMessageHandler.MessageSourceIsRegistered)
					{
						staticDeviceNotificationHandler = new NativeMessageHandler(StaticMessageCallback);
						Win32.DeviceManagement.RegisterDeviceNotificationHandle(staticDeviceNotificationHandler.Handle, classGuid, out staticDeviceNotificationHandle);
						success = true;
					}
				}

				// Keep track of the register/unregister requests:
				if (staticDeviceNotificationCounter < int.MaxValue)
					staticDeviceNotificationCounter++;
				else
					throw (new OverflowException("Too many USB device notification registrations! It is required to restart the application!"));
			}

			return (success);
		}

		/// <remarks>
		/// Attention:
		/// This function also exists in the other USB classes.
		/// Changes here must be applied there too.
		/// </remarks>
		public static void UnregisterStaticDeviceNotificationHandler()
		{
			lock (staticDeviceNotificationSyncObj)
			{
				// Keep track of the register/unregister requests:
				if (staticDeviceNotificationCounter > 0)
					staticDeviceNotificationCounter--;

				// The last call to this method unregisters the notification:
				if (staticDeviceNotificationCounter == 0)
				{
					// Check whether unregistration is still required, as Dispose() may be called multiple times!
					if (staticDeviceNotificationHandle != IntPtr.Zero)
					{
						Win32.DeviceManagement.UnregisterDeviceNotificationHandle(staticDeviceNotificationHandle);
						staticDeviceNotificationHandle = IntPtr.Zero;

						staticDeviceNotificationHandler.Close();
						staticDeviceNotificationHandler = null;
					}
				}
			}
		}

		/// <remarks>
		/// Attention:
		/// This function also exists in the other USB classes.
		/// Changes here must be applied there too.
		/// </remarks>
		private static void StaticMessageCallback(ref Message m)
		{
			var de = MessageToDeviceEvent(ref m);

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
							var e = new DeviceEventArgs(DeviceClass.Any, new DeviceInfo(devicePath));

							Debug.WriteLine("USB device connected:");
							Debug.Indent();
							Debug.WriteLine("Path = " + devicePath);
							Debug.WriteLine("Info = " + e.DeviceInfo);
							Debug.Unindent();

							EventHelper.RaiseAsync(DeviceConnected, typeof(Device), e);
							break;
						}

						case DeviceEvent.Disconnected:
						{
							var e = new DeviceEventArgs(DeviceClass.Any, new DeviceInfo(devicePath));

							Debug.WriteLine("USB device disconnected:");
							Debug.Indent();
							Debug.WriteLine("Path = " + devicePath);
							Debug.Unindent();

							EventHelper.RaiseAsync(DeviceDisconnected, typeof(Device), e);
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

		/// <summary>
		/// A dedicated event helper to allow discarding exceptions when object got disposed.
		/// </summary>
		private EventHelper.Item eventHelper = EventHelper.CreateItem(typeof(Device).FullName);

		private Guid classGuid;
		private DeviceInfo deviceInfo;
		private bool isConnected;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary>
		/// Event raised after the device has been connected or reconnected.
		/// </summary>
		public event EventHandler Connected;

		/// <summary>
		/// Event raised after the device has been disconnected.
		/// </summary>
		public event EventHandler Disconnected;

		/// <summary>
		/// Event raised after an I/O error has occurred.
		/// </summary>
		public event EventHandler<ErrorEventArgs> IOError;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid", Justification = "'ClassGuid' is the official term, even WMI uses it.")]
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
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid", Justification = "'ClassGuid' is the official term, even WMI uses it.")]
		protected Device(Guid classGuid, int vendorId, int productId)
		{
			this.classGuid = classGuid; // The USB class GUID arg is forseen for future use.

			this.deviceInfo = new DeviceInfo(vendorId, productId);
			Initialize();
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid", Justification = "'ClassGuid' is the official term, even WMI uses it.")]
		protected Device(Guid classGuid, int vendorId, int productId, string serial)
		{
			this.classGuid = classGuid; // The USB class GUID arg is forseen for future use.

			this.deviceInfo = new DeviceInfo(vendorId, productId, serial);
			Initialize();
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid", Justification = "'ClassGuid' is the official term, even WMI uses it.")]
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
				AttachAndRegisterStaticDeviceEventHandlers(this.classGuid);
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
				// Getting a handle means the device is connected to the computer:
				deviceHandle.Close();
				this.isConnected = true;
			}
		}

		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid", Justification = "'ClassGuid' is the official term, even WMI uses it.")]
		private void AttachAndRegisterStaticDeviceEventHandlers(Guid classGuid)
		{
			DeviceConnected    += Device_DeviceConnected;
			DeviceDisconnected += Device_DeviceDisconnected;

			RegisterStaticDeviceNotificationHandler(classGuid);
		}

		private void UnregisterAndDetachStaticDeviceEventHandlers()
		{
			UnregisterStaticDeviceNotificationHandler();

			DeviceConnected    -= Device_DeviceConnected;
			DeviceDisconnected -= Device_DeviceDisconnected;
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public bool IsDisposed { get; protected set; }

		/// <summary></summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary></summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				DebugEventManagement.DebugWriteAllEventRemains(this);
				this.eventHelper.DiscardAllEventsAndExceptions();

				// In any case, ensure that the static event handlers get detached:
				UnregisterAndDetachStaticDeviceEventHandlers();

				// Dispose of managed resources if requested:
				if (disposing)
				{
				}

				// Set state to disposed:
				IsDisposed = true;
			}
		}

	#if (DEBUG)
		/// <remarks>
		/// Microsoft.Design rule CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable requests
		/// "Types that declare disposable members should also implement IDisposable. If the type
		///  does not own any unmanaged resources, do not implement a finalizer on it."
		///
		/// Well, true for best performance on finalizing. However, it's not easy to find missing
		/// calls to <see cref="Dispose()"/>. In order to detect such missing calls, the finalizer
		/// is kept for DEBUG, indicating missing calls.
		///
		/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
		/// </remarks>
		~Device()
		{
			Dispose(false);

			DebugDisposal.DebugNotifyFinalizerInsteadOfDispose(this);
		}
	#endif // DEBUG

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (IsDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed!"));
		}

		#endregion

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary>
		/// Returns the complete device info. To read a specific device property, use the property
		/// members below.
		/// </summary>
		/// <remarks>
		/// \remind (2019-11-10 / MKY)
		/// According to the class description, a <see cref="DeviceInfo"/> shall be treated as an
		/// an immutable object. While not ideal, it is considered acceptable to return such object.
		/// Split into mutable settings tuple and immutable runtime container should be done.
		/// </remarks>
		public virtual DeviceInfo Info
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.deviceInfo);
			}
		}

		/// <remarks>
		/// Example path:
		/// <![CDATA[
		/// "\\\\?\\hid#vid_0eb8&pid_2303#8&26d7e5e6&0&0000#{4d1e55b2-f16f-11cf-88cb-001111000030}"
		/// ]]>
		///
		/// Only VID/PID is contained as string, not the serial.
		/// </remarks>
		protected virtual string Path
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (Info.Path);
			}
		}

		#region Properties > IDeviceInfo
		//------------------------------------------------------------------------------------------
		// Properties > IDeviceInfo
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual int VendorId
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (Info.VendorId);
			}
		}

		/// <summary></summary>
		public virtual string VendorIdString
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (Info.VendorIdString);
			}
		}

		/// <summary></summary>
		public virtual int ProductId
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (Info.ProductId);
			}
		}

		/// <summary></summary>
		public virtual string ProductIdString
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (Info.ProductIdString);
			}
		}

		/// <summary></summary>
		public virtual string Manufacturer
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (Info.Manufacturer);
			}
		}

		/// <summary></summary>
		public virtual string Product
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (Info.Product);
			}
		}

		/// <summary></summary>
		public virtual string Serial
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (Info.Serial);
			}
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
		/// The Win32 API 'RegisterDeviceNotification' raises 'Connected' or 'Disconnected' events
		/// in case a device is attached or removed from the computer. The <see cref="IsConnected"/>
		/// property relates to the state of these events.
		/// </remarks>
		/// <returns>
		/// <c>true</c> if the device is connected to the computer; otherwise, <c>false</c>.
		/// </returns>
		public bool IsConnected
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.isConnected);
			}
		}

		#endregion

		#endregion

		#region Event Handling
		//==========================================================================================
		// Event Handling
		//==========================================================================================

		/// <remarks>
		/// Attention:
		/// This function similarly exists in the other USB classes.
		/// Changes here may have to be applied there too.
		/// </remarks>
		private void Device_DeviceConnected(object sender, DeviceEventArgs e)
		{
			if (Info == e.DeviceInfo)
			{
				// Force reinitialize with new device info:
				Reinitialize(e.DeviceInfo);

				OnConnected(EventArgs.Empty);
			}
		}

		/// <remarks>
		/// Attention:
		/// This function similarly exists in the other USB classes.
		/// Changes here may have to be applied there too.
		/// </remarks>
		private void Device_DeviceDisconnected(object sender, DeviceEventArgs e)
		{
			if (Info == e.DeviceInfo)
			{
				OnDisconnected(EventArgs.Empty);
			}
		}

		#endregion

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnConnected(EventArgs e)
		{
			this.isConnected = true;
			this.eventHelper.RaiseSync(Connected, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisconnected(EventArgs e)
		{
			this.isConnected = false;
			this.eventHelper.RaiseSync(Disconnected, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOError(ErrorEventArgs e)
		{
			this.eventHelper.RaiseSync<ErrorEventArgs>(IOError, this, e);
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation,
		/// which is a string describing the USB device as accurately as possible.
		/// </summary>
		public override string ToString()
		{
			// Do not call AssertNotDisposed() on such basic method! Its return value may be needed for debugging. All underlying fields are still valid after disposal.

			return (Info.ToString());
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation,
		/// which is a string describing the USB device as accurately as possible.
		/// </summary>
		public virtual string ToString(bool insertIds)
		{
			// Do not call AssertNotDisposed() on such basic method! Its return value may be needed for debugging. All underlying fields are still valid after disposal.

			return (Info.ToString(insertIds));
		}

		/// <summary>
		/// Returns a string describing the USB device in a short form.
		/// </summary>
		public virtual string ToShortString()
		{
			// Do not call AssertNotDisposed() on such basic method! Its return value may be needed for debugging. All underlying fields are still valid after disposal.

			return (Info.ToShortString());
		}

		/// <summary>
		/// Returns a string describing the USB device in a long form.
		/// </summary>
		public virtual string ToLongString()
		{
			// Do not call AssertNotDisposed() on such basic method! Its return value may be needed for debugging. All underlying fields are still valid after disposal.

			return (Info.ToLongString());
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
