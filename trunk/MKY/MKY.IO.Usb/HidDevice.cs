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
// Copyright © 2010-2020 Matthias Kläy.
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
using System.Globalization;
using System.Windows.Forms;

using Microsoft.Win32.SafeHandles;

using MKY.Diagnostics;
using MKY.Windows.Forms;

#endregion

namespace MKY.IO.Usb
{
	/// <summary>
	/// Extends a USB device with HID capabilities.
	/// </summary>
	public class HidDevice : Device, IHidDeviceInfo
	{
		#region Static Constants
		//==========================================================================================
		// Static Constants
		//==========================================================================================

		/// <summary>
		/// Returns the GUID associated with USB HID.
		/// </summary>
		public static readonly Guid HidGuid = Win32.Hid.GetHidGuid();

		#endregion

		#region Static Events
		//==========================================================================================
		// Static Events
		//==========================================================================================

		/// <summary>
		/// Occurs when an USB device is connected to the computer.
		/// </summary>
		public static new event EventHandler<DeviceEventArgs> DeviceConnected;

		/// <summary>
		/// Occurs when an USB device is disconnected from the computer.
		/// </summary>
		public static new event EventHandler<DeviceEventArgs> DeviceDisconnected;

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
		/// Returns an array of the USB HID devices currently available on the system.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static new HidDeviceInfo[] GetDevices(bool retrieveStringsFromDevice = true)
		{
			return (GetDevices(HidDeviceCollection.AnyUsagePage, retrieveStringsFromDevice));
		}

		/// <summary>
		/// Returns an array of the USB HID devices of the given usage page currently available
		/// on the system.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[CLSCompliant(false)]
		public static HidDeviceInfo[] GetDevices(int usagePage = HidDeviceCollection.AnyUsagePage, bool retrieveStringsFromDevice = true)
		{
			return (GetDevices(usagePage, HidDeviceCollection.AnyUsageId, retrieveStringsFromDevice));
		}

		/// <summary>
		/// Returns an array of the USB HID devices of the given usage page and usage currently
		/// available on the system.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static HidDeviceInfo[] GetDevices(int usagePage = HidDeviceCollection.AnyUsagePage, int usageId = HidDeviceCollection.AnyUsageId, bool retrieveStringsFromDevice = true)
		{
			var dis = GetDevicesFromGuid(GetGuidFromDeviceClass(DeviceClass.Hid), retrieveStringsFromDevice);
			var l = new List<HidDeviceInfo>(dis.Length); // Preset the required capacity to improve memory management.

			foreach (var di in dis)
			{
				if (usagePage != HidDeviceCollection.AnyUsagePage)
				{
					if (di.UsagePage != usagePage)
						continue; // Filter devices with non-matching usage pages.
				}

				if (usageId != HidDeviceCollection.AnyUsageId)
				{
					if (di.UsageId != usageId)
						continue; // Filter devices with non-matching usage pages.
				}

				l.Add(di);
			}

			return (l.ToArray());
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
		public static new HidDeviceInfo[] GetDevicesFromClass(DeviceClass deviceClass, bool retrieveStringsFromDevice = true)
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
		public static new HidDeviceInfo[] GetDevicesFromGuid(Guid classGuid, bool retrieveStringsFromDevice = true, bool ignoreDuplicates = true)
		{
			var dis = Device.GetDevicesFromGuid(classGuid, retrieveStringsFromDevice, ignoreDuplicates);
			var l = new List<HidDeviceInfo>(dis.Length); // Preset the required capacity to improve memory management.

			foreach (var di in dis)
			{
				Win32.Hid.NativeTypes.HIDP_CAPS capabilities;
				if (GetDeviceCapabilities(di.Path, out capabilities))
				{                        // The Win32 HIDP_CAPS structure uses term 'Usage', not 'UsageId'.
					l.Add(new HidDeviceInfo(di, capabilities.UsagePage, capabilities.Usage));
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
		/// Returns the information of the device with the given path.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "6#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "7#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static bool GetDeviceInfoFromPath(string path, out int vendorId, out int productId, out string manufacturer, out string product, out string serial, out int usagePage, out int usageId)
		{
			if (GetDeviceInfoFromPath(path, out vendorId, out productId, out manufacturer, out product, out serial))
			{
				Win32.Hid.NativeTypes.HIDP_CAPS capabilities;
				if (GetDeviceCapabilities(path, out capabilities))
				{
					usagePage = capabilities.UsagePage;
					usageId   = capabilities.Usage; // The Win32 HIDP_CAPS structure uses term 'Usage', not 'UsageId'.
					return (true);
				}
			}

			usagePage = HidDeviceInfo.AnyUsagePage;
			usageId   = HidDeviceInfo.AnyUsageId;
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
		public static HidDeviceInfo GetDeviceInfoFromVidPidUsage(int vendorId, int productId, int usagePage, int usageId)
		{
			string path, manufacturer, product, serial;

			if (GetDeviceInfoFromVidPidUsage(vendorId, productId, usagePage, usageId, out path, out manufacturer, out product, out serial))
				return (new HidDeviceInfo(path, vendorId, productId, manufacturer, product, serial, usagePage, usageId));
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
		/// <param name="usagePage">Given usage page.</param>
		/// <param name="usageId">Given usage ID.</param>
		/// <param name="path">Retrieved system path, or "" if no appropriate device was found.</param>
		/// <param name="manufacturer">Retrieved manufacturer, or "" if no appropriate device was found.</param>
		/// <param name="product">Retrieved product, or "" if no appropriate device was found.</param>
		/// <param name="serial">Retrieved serial string, or "" if no appropriate device was found.</param>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "6#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "7#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static bool GetDeviceInfoFromVidPidUsage(int vendorId, int productId, int usagePage, int usageId, out string path, out string manufacturer, out string product, out string serial)
		{
			foreach (var di in GetDevicesFromClass(DeviceClass.Hid))
			{
				if (di.EqualsVidPidUsage(vendorId, productId, usagePage, usageId))
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
		public static HidDeviceInfo GetDeviceInfoFromVidPidSerialUsage(int vendorId, int productId, string serial, int usagePage, int usageId)
		{
			string path, manufacturer, product;

			if (GetDeviceInfoFromVidPidSerialUsage(vendorId, productId, serial, usagePage, usageId, out path, out manufacturer, out product))
				return (new HidDeviceInfo(path, vendorId, productId, manufacturer, product, serial, usagePage, usageId));
			else
				return (null);
		}

		/// <summary>
		/// Returns the information of the device with the given VID and PID and serial string.
		/// </summary>
		/// <param name="vendorId">Given VID.</param>
		/// <param name="productId">Given PID.</param>
		/// <param name="serial">Given serial string.</param>
		/// <param name="usagePage">Given usage page.</param>
		/// <param name="usageId">Given usage ID.</param>
		/// <param name="path">Retrieved system path, or "" if no appropriate device was found.</param>
		/// <param name="manufacturer">Retrieved manufacturer, or "" if no appropriate device was found.</param>
		/// <param name="product">Retrieved product, or "" if no appropriate device was found.</param>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "6#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "7#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static bool GetDeviceInfoFromVidPidSerialUsage(int vendorId, int productId, string serial, int usagePage, int usageId, out string path, out string manufacturer, out string product)
		{
			foreach (var di in GetDevicesFromClass(DeviceClass.Hid))
			{
				if (di.EqualsVidPidSerialUsage(vendorId, productId, serial, usagePage, usageId))
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
		/// Attention:
		/// This function also exists in the other USB classes.
		/// Changes here must be applied there too.
		/// </remarks>
		public static bool RegisterStaticDeviceNotificationHandler()
		{
			bool success = false;

			lock (staticDeviceNotificationSyncObj)
			{
				// The first call to this method registers the notification:
				if (staticDeviceNotificationCounter == 0)
				{
					if (staticDeviceNotificationHandle != IntPtr.Zero)
						throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "Invalid state within USB HID device object!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

					if (NativeMessageHandler.MessageSourceIsRegistered)
					{
						staticDeviceNotificationHandler = new NativeMessageHandler(StaticMessageCallback);
						Win32.DeviceManagement.RegisterDeviceNotificationHandle(staticDeviceNotificationHandler.Handle, HidGuid, out staticDeviceNotificationHandle);
						success = true;
					}
				}

				// Keep track of the register/unregister requests:
				if (staticDeviceNotificationCounter < int.MaxValue)
					staticDeviceNotificationCounter++;
				else
					throw (new OverflowException("Too many USB HID device notification registrations! It is required to restart the application!"));
			}

			return (success);
		}

		/// <remarks>
		/// Attention:
		/// This function also exists in the other USB classes.
		/// Changes here must be applied there too.
		/// </remarks>
		public static new void UnregisterStaticDeviceNotificationHandler()
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
							var e = new DeviceEventArgs(DeviceClass.Hid, new HidDeviceInfo(devicePath));

							Debug.WriteLine("USB HID device connected:");
							Debug.Indent();
							Debug.WriteLine("Path = " + devicePath);
							Debug.WriteLine("Info = " + e.DeviceInfo);
							Debug.Unindent();

							EventHelper.RaiseAsync(DeviceConnected, typeof(HidDevice), e);
							break;
						}

						case DeviceEvent.Disconnected:
						{
							var e = new DeviceEventArgs(DeviceClass.Hid, new HidDeviceInfo(devicePath));

							Debug.WriteLine("USB HID device disconnected:");
							Debug.Indent();
							Debug.WriteLine("Path = " + devicePath);
							Debug.Unindent();

							EventHelper.RaiseAsync(DeviceDisconnected, typeof(HidDevice), e);
							break;
						}
					}
				}
			}
		}

		#endregion

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private HidDeviceInfo hidDeviceInfo;

		private int inputReportByteLength;
		private int outputReportByteLength;
		private int featureReportByteLength;

		private int linkCollectionNodes;
		private int inputButtonCaps;
		private int inputValueCaps;
		private int inputDataIndices;
		private int outputButtonCaps;
		private int outputValueCaps;
		private int outputDataIndices;
		private int featureButtonCaps;
		private int featureValueCaps;
		private int featureDataIndices;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public HidDevice(string path)
			: base(HidGuid, path) // Path already is correct in respect to usage.
		{
			Initialize();
		}

		/// <summary></summary>
		public HidDevice(int vendorId, int productId, int usagePage, int usageId)
			: base(HidGuid, GetPathUsageAware(vendorId, productId, usagePage, usageId))
		{
			Initialize();
		}

		/// <summary></summary>
		public HidDevice(int vendorId, int productId, string serial, int usagePage, int usageId)
			: base(HidGuid, GetPathUsageAware(vendorId, productId, serial, usagePage, usageId))
		{
			Initialize();
		}

		/// <summary></summary>
		public HidDevice(HidDeviceInfo deviceInfo)
			: base(HidGuid, (DeviceInfo)deviceInfo) // Path already is correct in respect to usage.
		{
			Initialize();
		}

		private static string GetPathUsageAware(int vendorId, int productId, int usagePage, int usageId)
		{
			string path, manufacturer, product, serial;
			if (GetDeviceInfoFromVidPidUsage(vendorId, productId, usagePage, usageId, out path, out manufacturer, out product, out serial))
				return (path);

			return (null);
		}

		private static string GetPathUsageAware(int vendorId, int productId, string serial, int usagePage, int usageId)
		{
			string path, manufacturer, product;
			if (GetDeviceInfoFromVidPidSerialUsage(vendorId, productId, serial, usagePage, usageId, out path, out manufacturer, out product))
				return (path);

			return (null);
		}

		/// <remarks>
		/// Base constructor creates device info and therefore also sets system path.
		/// </remarks>
		private void Initialize()
		{
			GetDeviceCapabilities();

			// Only attach handlers if this is an instance of the USB HID device class.
			// If this instance is e.g. an Ser/HID device, handlers must be attached there.
			if (GetType() == typeof(HidDevice))
				AttachAndRegisterStaticDeviceEventHandlers();
		}

		/// <summary>
		/// Used to reinitialize the device in case of a reconnect.
		/// </summary>
		protected override void Reinitialize()
		{
			base.Reinitialize();

			GetDeviceCapabilities();
		}

		/// <remarks>
		/// This method shall not be overridden as it accesses private members.
		/// </remarks>
		private void GetDeviceCapabilities()
		{
			Win32.Hid.NativeTypes.HIDP_CAPS capabilities;
			if (GetDeviceCapabilities(Path, out capabilities))
			{                                              // The Win32 HIDP_CAPS structure uses term 'Usage', not 'UsageId'.
				this.hidDeviceInfo = new HidDeviceInfo(base.Info, capabilities.UsagePage, capabilities.Usage);

				this.inputReportByteLength   = capabilities.InputReportByteLength;
				this.outputReportByteLength  = capabilities.OutputReportByteLength;
				this.featureReportByteLength = capabilities.FeatureReportByteLength;

				this.linkCollectionNodes = capabilities.NumberLinkCollectionNodes;
				this.inputButtonCaps     = capabilities.NumberInputButtonCaps;
				this.inputValueCaps      = capabilities.NumberInputValueCaps;
				this.inputDataIndices    = capabilities.NumberInputDataIndices;
				this.outputButtonCaps    = capabilities.NumberOutputButtonCaps;
				this.outputValueCaps     = capabilities.NumberOutputValueCaps;
				this.outputDataIndices   = capabilities.NumberOutputDataIndices;
				this.featureButtonCaps   = capabilities.NumberFeatureButtonCaps;
				this.featureValueCaps    = capabilities.NumberFeatureValueCaps;
				this.featureDataIndices  = capabilities.NumberFeatureDataIndices;

				// Output user-friendly usage information:
				string usagePageValue = "0x" + Info.UsagePage.ToString("X4", CultureInfo.InvariantCulture);
				string usageIdValue   = "0x" + Info.UsageId  .ToString("X4", CultureInfo.InvariantCulture);

				string usagePageName = "<Unknown>";
				try
				{
					HidUsagePage page = (HidUsagePageEx)(Info.UsagePage);
					string name = Enum.GetName(typeof(HidUsagePage), page);
					if (!string.IsNullOrEmpty(name))
						usagePageName = name;
					else if ((Info.UsagePage >= (int)(HidUsagePage.VendorDefined_First)) &&
						     (Info.UsagePage <= (int)(HidUsagePage.VendorDefined_Last)))
						usagePageName = "VendorDefined";
				}
				catch (ArgumentException ex) // "...is not an Enum" or type mismatches.
				{
					DebugEx.WriteException(GetType(), ex, "Exception while retrieving usage page!");
				}

				string usageIdName = "<Unknown>";
				try
				{
					HidUsageId id = (HidUsageIdEx)(Info.UsageId);
					string name = Enum.GetName(typeof(HidUsageId), id);
					if (!string.IsNullOrEmpty(name))
						usageIdName = name;
					else if ((Info.UsageId >= (int)(HidUsageId.PageDefined_First)) &&
						     (Info.UsageId <= (int)(HidUsageId.PageDefined_Last)))
						usageIdName = "PageDefined";
					else if ((Info.UsageId >= (int)(HidUsageId.VendorDefined_First)) &&
						     (Info.UsageId <= (int)(HidUsageId.VendorDefined_Last)))
						usageIdName = "VendorDefined";
				}
				catch (ArgumentException ex) // "...is not an Enum" or type mismatches.
				{
					DebugEx.WriteException(GetType(), ex, "Exception while retrieving usage ID!");
				}

				Debug.WriteLine("USB HID device usage information:");
				Debug.Indent(); // Terms "Usage page" and "Usage ID" are given by https://www.usb.org/sites/default/files/documents/hut1_12v2.pdf section 3.1 [HID Usage Table Conventions].
				Debug.WriteLine("Usage page " + usagePageValue + " corresponds to '" + usagePageName + "'");
				Debug.WriteLine("Usage ID   " + usageIdValue   + " corresponds to '" + usageIdName   + "'");
				Debug.Unindent();
			}
		}

		/// <remarks>
		/// Private for not having to expose native Win32 types.
		/// </remarks>
		private static bool GetDeviceCapabilities(string path, out Win32.Hid.NativeTypes.HIDP_CAPS capabilities)
		{
			SafeFileHandle deviceHandle;
			if (!string.IsNullOrEmpty(path) && Win32.Hid.CreateSharedQueryOnlyDeviceHandle(path, out deviceHandle))
			{
				try
				{
					capabilities = new Win32.Hid.NativeTypes.HIDP_CAPS();
					return (Win32.Hid.GetDeviceCapabilities(deviceHandle, ref capabilities));
				}
				finally
				{
					deviceHandle.Close();
				}
			}

			capabilities = new Win32.Hid.NativeTypes.HIDP_CAPS();
			return (false);
		}

		/// <summary>
		/// Gets the device capabilities for diagnostics/debug purposes.
		/// </summary>
		[Conditional("DEBUG")]
		public static void DebugWriteDeviceCapabilities(string path)
		{
			Win32.Hid.NativeTypes.HIDP_CAPS capabilities;
			GetDeviceCapabilities(path, out capabilities);
		}

		private void AttachAndRegisterStaticDeviceEventHandlers()
		{
			DeviceConnected    += Device_DeviceConnected;
			DeviceDisconnected += Device_DeviceDisconnected;

			RegisterStaticDeviceNotificationHandler();
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
		protected override void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				UnregisterAndDetachStaticDeviceEventHandlers();

				// Dispose of managed resources if requested:
				if (disposing)
				{
				}
			}

			base.Dispose(disposing);
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
		/// According to the class description, an <see cref="HidDeviceInfo"/> shall be treated as
		/// an immutable object. While not ideal, it is considered acceptable to return such object.
		/// Split into mutable settings tuple and immutable runtime container should be done.
		/// </remarks>
		public new HidDeviceInfo Info
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.hidDeviceInfo);
			}
		}

		#region Properties > IHidDeviceInfo
		//------------------------------------------------------------------------------------------
		// Properties > IHidDeviceInfo
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual int UsagePage
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (Info.UsagePage);
			}
		}

		/// <summary></summary>
		public virtual string UsagePageString
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (Info.UsagePageString);
			}
		}

		/// <summary></summary>
		public virtual int UsageId
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (Info.UsageId);
			}
		}

		/// <summary></summary>
		public virtual string UsageIdString
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (Info.UsageIdString);
			}
		}

		#endregion

		/// <summary></summary>
		public virtual int InputReportByteLength
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.inputReportByteLength);
			}
		}

		/// <summary></summary>
		public virtual int OutputReportByteLength
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.outputReportByteLength);
			}
		}

		/// <summary></summary>
		public virtual int FeatureReportByteLength
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.featureReportByteLength);
			}
		}

		/// <summary></summary>
		public virtual int LinkCollectionNodes
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.linkCollectionNodes);
			}
		}

		/// <summary></summary>
		public virtual int InputButtonCaps
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.inputButtonCaps);
			}
		}

		/// <summary></summary>
		public virtual int InputValueCaps
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.inputValueCaps);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Indices", Justification = "'Indices' is a correct English term and used throughout the .NET framework.")]
		public virtual int InputDataIndices
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.inputDataIndices);
			}
		}

		/// <summary></summary>
		public virtual int OutputButtonCaps
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.outputButtonCaps);
			}
		}

		/// <summary></summary>
		public virtual int OutputValueCaps
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.outputValueCaps);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Indices", Justification = "'Indices' is a correct English term and used throughout the .NET framework.")]
		public virtual int OutputDataIndices
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.outputDataIndices);
			}
		}

		/// <summary></summary>
		public virtual int FeatureButtonCaps
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.featureButtonCaps);
			}
		}

		/// <summary></summary>
		public virtual int FeatureValueCaps
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.featureValueCaps);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Indices", Justification = "'Indices' is a correct English term and used throughout the .NET framework.")]
		public virtual int FeatureDataIndices
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.featureDataIndices);
			}
		}

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
		public override string ToString(bool insertVidPid)
		{
			// Do not call AssertNotDisposed() on such basic method! Its return value may be needed for debugging. All underlying fields are still valid after disposal.

			return (Info.ToString(insertVidPid));
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation,
		/// which is a string describing the USB device as accurately as possible.
		/// </summary>
		public virtual string ToString(bool insertVidPid, bool appendUsage)
		{
			// Do not call AssertNotDisposed() on such basic method! Its return value may be needed for debugging. All underlying fields are still valid after disposal.

			return (Info.ToString(insertVidPid, appendUsage));
		}

		/// <summary>
		/// Returns a string describing the USB device in a short form.
		/// </summary>
		public override string ToShortString()
		{
			// Do not call AssertNotDisposed() on such basic method! Its return value may be needed for debugging. All underlying fields are still valid after disposal.

			return (Info.ToShortString());
		}

		/// <summary>
		/// Returns a string describing the USB device in a long form.
		/// </summary>
		public override string ToLongString()
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
