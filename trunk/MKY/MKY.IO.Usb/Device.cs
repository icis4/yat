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
// MKY Version 1.0.30
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2021 Matthias Kläy.
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
	public abstract class Device : DisposableBase, IDevice, IDeviceInfo
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
		/// Tries to return VID and PID of a given path.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static bool TryGetVidPidFromPath(string path, out int vendorId, out int productId)
		{
			SafeFileHandle deviceHandle;
			if (Win32.Hid.CreateSharedQueryOnlyDeviceHandle(path, out deviceHandle))
			{
				try
				{
					if (TryGetVidPidFromHandle(deviceHandle, out vendorId, out productId))
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
		private static bool TryGetVidPidFromHandle(SafeFileHandle deviceHandle, out int vendorId, out int productId)
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
		/// Tries to return manufacturer, product and serial strings of a given path.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static bool TryGetStringsFromPath(string path, out string manufacturer, out string product, out string serial)
		{
			SafeFileHandle deviceHandle;
			if (Win32.Hid.CreateSharedQueryOnlyDeviceHandle(path, out deviceHandle))
			{
				try
				{
					// A device may not define any or some strings (e.g. no serial string).
					// Therefore, simply try to retrieve the strings in best-effort manner.
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
		/// Therefore, simply try to retrieve the strings in best-effort manner; otherwise, return "".
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

				if (TryGetDeviceInfoFromPath(path, out vendorId, out productId, out manufacturer, out product, out serial))
					return (new DeviceInfo(path, vendorId, productId, manufacturer, product, serial));
				else
					return (null);
			}
			else
			{
				int vendorId, productId;

				if (TryGetDeviceInfoFromPath(path, out vendorId, out productId))
					return (new DeviceInfo(path, vendorId, productId));
				else
					return (null);
			}
		}

		/// <summary>
		/// Tries to return the information of the device with the given path.
		/// </summary>
		/// <remarks>
		/// \todo: This method currently only works for HID devices. Find a HID independent way to retrieve VID/PID.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static bool TryGetDeviceInfoFromPath(string path, out int vendorId, out int productId)
		{
			string manufacturer, product, serial;
			return (TryGetDeviceInfoFromPath(path, false, out vendorId, out productId, out manufacturer, out product, out serial));
		}

		/// <summary>
		/// Tries to return the information of the device with the given path.
		/// </summary>
		/// <remarks>
		/// \todo: This method currently only works for HID devices. Find a HID independent way to retrieve VID/PID.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static bool TryGetDeviceInfoFromPath(string path, out int vendorId, out int productId, out string manufacturer, out string product, out string serial)
		{
			return (TryGetDeviceInfoFromPath(path, true, out vendorId, out productId, out manufacturer, out product, out serial));
		}

		/// <summary>
		/// Tries to return the information of the device with the given path.
		/// </summary>
		/// <remarks>
		/// \todo: This method currently only works for HID devices. Find a HID independent way to retrieve VID/PID.
		/// </remarks>
		private static bool TryGetDeviceInfoFromPath(string path, bool retrieveStringsFromDevice, out int vendorId, out int productId, out string manufacturer, out string product, out string serial)
		{
			SafeFileHandle deviceHandle;
			if (Win32.Hid.CreateSharedQueryOnlyDeviceHandle(path, out deviceHandle))
			{
				try
				{
					if (TryGetVidPidFromHandle(deviceHandle, out vendorId, out productId))
					{
						if (retrieveStringsFromDevice)
						{
							// A device may not define any or some strings (e.g. no serial string).
							// Therefore, simply try to retrieve the strings in best-effort manner.
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

			if (TryGetDeviceInfoFromVidPid(vendorId, productId, out path, out manufacturer, out product, out serial))
				return (new DeviceInfo(path, vendorId, productId, manufacturer, product, serial));
			else
				return (null);
		}

		/// <summary>
		/// Tries to return the information of the device with the given VID and PID.
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
		public static bool TryGetDeviceInfoFromVidPid(int vendorId, int productId, out string path, out string manufacturer, out string product, out string serial)
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

			if (TryGetDeviceInfoFromVidPidSerial(vendorId, productId, serial, out path, out manufacturer, out product))
				return (new DeviceInfo(path, vendorId, productId, manufacturer, product, serial));
			else
				return (null);
		}

		/// <summary>
		/// Tries to return the information of the device with the given VID and PID and serial string.
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
		public static bool TryGetDeviceInfoFromVidPidSerial(int vendorId, int productId, string serial, out string path, out string manufacturer, out string product)
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
			{                                                                               // DBT is uint, i.e. UInt32.
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
		private DeviceInfo info;
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

		/// <remarks>
		/// <paramref name="path"/> must be valid; otherwise, an exception is thrown.
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="path"/> is null.</exception>
		/// <exception cref="ArgumentException">No device could be created for the given <paramref name="path"/>.</exception>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid", Justification = "'ClassGuid' is the official term, even WMI uses it.")]
		protected Device(Guid classGuid, string path)
		{
			if (path == null)
				throw (new ArgumentNullException(nameof(path), "A path is required for this constructor! Without a path, use one of the other constructors."));

			this.classGuid = classGuid; // The USB class GUID arg is forseen for future use.

			int vendorId, productId;
			string manufacturer, product, serial;
			if (TryGetDeviceInfoFromPath(path, out vendorId, out productId, out manufacturer, out product, out serial))
			{
				this.info = new DeviceInfo(path, vendorId, productId, manufacturer, product, serial);
				Initialize();
			}
			else // Ensure that 'Info' is always defined!
			{
				throw (new ArgumentException("No device could be created for the given path! Make sure the path is valid or use one of the other constructors.", nameof(path)));
			}
		}

		/// <remarks>
		/// If <paramref name="path"/> is valid, <see cref="Info"/> is created from it; otherwise,
		/// <see cref="Info"/> is created from <paramref name="vendorId"/> and <paramref name="productId"/>.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"> if a value is invalid.</exception>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid", Justification = "'ClassGuid' is the official term, even WMI uses it.")]
		protected Device(Guid classGuid, string path, int vendorId, int productId)
		{
			this.classGuid = classGuid; // The USB class GUID arg is forseen for future use.

			string manufacturer, product, serial;
			if (!string.IsNullOrEmpty(path) && TryGetDeviceInfoFromPath(path, out vendorId, out productId, out manufacturer, out product, out serial))
			{
				this.info = new DeviceInfo(path, vendorId, productId, manufacturer, product, serial);
				Initialize();
			}
			else
			{
				this.info = new DeviceInfo(vendorId, productId);
				Initialize();
			}
		}

		/// <remarks>
		/// If <paramref name="path"/> is valid, <see cref="Info"/> is created from it; otherwise,
		/// <see cref="Info"/> is created from <paramref name="vendorId"/>, <paramref name="productId"/> and <paramref name="serial"/>.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"> if a value is invalid.</exception>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid", Justification = "'ClassGuid' is the official term, even WMI uses it.")]
		protected Device(Guid classGuid, string path, int vendorId, int productId, string serial)
		{
			this.classGuid = classGuid; // The USB class GUID arg is forseen for future use.

			string manufacturer, product;
			if (!string.IsNullOrEmpty(path) && TryGetDeviceInfoFromPath(path, out vendorId, out productId, out manufacturer, out product, out serial))
			{
				this.info = new DeviceInfo(path, vendorId, productId, manufacturer, product, serial);
				Initialize();
			}
			else
			{
				this.info = new DeviceInfo(vendorId, productId, serial);
				Initialize();
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid", Justification = "'ClassGuid' is the official term, even WMI uses it.")]
		protected Device(Guid classGuid, DeviceInfo deviceInfo)
		{
			this.classGuid = classGuid; // The USB class GUID arg is forseen for future use.

			this.info = new DeviceInfo(deviceInfo);
			Initialize();
		}

		/// <remarks>
		/// Constructor creates <see cref="Info"/>.
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
		protected virtual void Reinitialize(DeviceInfo deviceInfo)
		{
			this.info = new DeviceInfo(deviceInfo);
			Reinitialize();
		}

		/// <summary>
		/// Used to reinitialize the device in case of a reconnect.
		/// </summary>
		private void Reinitialize()
		{
			GetDeviceConnect();
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

		/// <param name="disposing">
		/// <c>true</c> when called from <see cref="Dispose"/>,
		/// <c>false</c> when called from finalizer.
		/// </param>
		protected override void Dispose(bool disposing)
		{
			if (this.eventHelper != null) // Possible when called by finalizer (non-deterministic).
				this.eventHelper.DiscardAllEventsAndExceptions();

			// In any case, ensure that the static event handlers get detached:
			UnregisterAndDetachStaticDeviceEventHandlers();

			// Dispose of managed resources:
			if (disposing)
			{
				// Nothing else to do (yet).
			}
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
		/// The info is always defined, i.e. not <c>null</c>, for a <see cref="Device"/> object.
		/// </remarks>
		/// <remarks>
		/// \remind (2019-11-10 / MKY)
		/// According to the class description, a <see cref="DeviceInfo"/> shall be treated as an
		/// an immutable object. While not ideal, it is considered acceptable to return such object.
		/// Split into mutable settings tuple and immutable runtime container could be done.
		/// </remarks>
		public virtual DeviceInfo Info
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				return (this.info);
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
			////AssertUndisposed() shall not be called from this simple get-property.

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
			////AssertUndisposed() shall not be called from this simple get-property.

				return (Info.VendorId);
			}
		}

		/// <summary></summary>
		public virtual string VendorIdString
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				return (Info.VendorIdString);
			}
		}

		/// <summary></summary>
		public virtual int ProductId
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				return (Info.ProductId);
			}
		}

		/// <summary></summary>
		public virtual string ProductIdString
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				return (Info.ProductIdString);
			}
		}

		/// <summary></summary>
		public virtual string Manufacturer
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				return (Info.Manufacturer);
			}
		}

		/// <summary></summary>
		public virtual string Product
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				return (Info.Product);
			}
		}

		/// <summary></summary>
		public virtual string Serial
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

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
			////AssertUndisposed() shall not be called from this simple get-property.

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
			// AssertUndisposed() shall not be called from such basic method! Its return value may be needed for debugging. All underlying fields are still valid after disposal.

			return (Info.ToString());
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation,
		/// which is a string describing the USB device as accurately as possible.
		/// </summary>
		public virtual string ToString(bool insertVidPid)
		{
			// AssertUndisposed() shall not be called from such basic method! Its return value may be needed for debugging. All underlying fields are still valid after disposal.

			return (Info.ToString(insertVidPid));
		}

		/// <summary>
		/// Returns a string describing the USB device in a short form.
		/// </summary>
		public virtual string ToShortString()
		{
			// AssertUndisposed() shall not be called from such basic method! Its return value may be needed for debugging. All underlying fields are still valid after disposal.

			return (Info.ToShortString());
		}

		/// <summary>
		/// Returns a string describing the USB device in a long form.
		/// </summary>
		public virtual string ToLongString()
		{
			// AssertUndisposed() shall not be called from such basic method! Its return value may be needed for debugging. All underlying fields are still valid after disposal.

			return (Info.ToLongString());
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
