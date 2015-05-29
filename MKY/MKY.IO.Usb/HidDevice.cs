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
// MKY Version 1.0.11
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
using System.Globalization;
using System.Windows.Forms;

using Microsoft.Win32.SafeHandles;

using MKY.Windows.Forms;

#endregion

namespace MKY.IO.Usb
{
	/// <summary>
	/// Extends a USB device with HID capabilities.
	/// </summary>
	public class HidDevice : Device
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

		/// <summary></summary>
		public static new event EventHandler<DeviceEventArgs> DeviceConnected;

		/// <summary></summary>
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
		/// Returns an array of all USB HID devices currently available on the system.
		/// </summary>
		public static new DeviceInfo[] GetDevices()
		{
			return (GetDevicesFromGuid(GetGuidFromDeviceClass(DeviceClass.Hid)));
		}

		/// <summary>
		/// Returns an array of the USB HID devices of the given usage page currently available
		/// on the system.
		/// </summary>
		[CLSCompliant(false)]
		public static DeviceInfo[] GetDevices(HidUsagePage usagePage)
		{
			List<DeviceInfo> l = new List<DeviceInfo>();

			foreach (DeviceInfo di in GetDevices())
			{
				using (HidDevice device = new HidDevice(di))
				{
					if (device.UsagePage == usagePage)
						l.Add(di);
				}
			}

			return (l.ToArray());
		}

		/// <summary>
		/// Returns an array of the USB HID devices of the given usage page and usage currently
		/// available on the system.
		/// </summary>
		[CLSCompliant(false)]
		public static DeviceInfo[] GetDevices(HidUsagePage usagePage, HidUsageId usageId)
		{
			List<DeviceInfo> l = new List<DeviceInfo>();

			foreach (DeviceInfo di in GetDevices())
			{
				using (HidDevice device = new HidDevice(di))
				{
					if (device.UsagePage == usagePage)
						if (device.UsageId == usageId)
							l.Add(di);
				}
			}

			return (l.ToArray());
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
		/// \attention:
		/// This function also exists in the other USB classes. Changes here must also be applied there.
		/// </remarks>
		public static void RegisterStaticDeviceNotificationHandler()
		{
			lock (staticDeviceNotificationSyncObj)
			{
				// The first call to this method registers the notification.
				if (staticDeviceNotificationCounter == 0)
				{
					if (staticDeviceNotificationHandle != IntPtr.Zero)
						throw (new InvalidOperationException("Invalid state within USB HID Device object, please report this bug!"));

					staticDeviceNotificationHandler = new NativeMessageHandler(StaticMessageCallback);
					Win32.DeviceManagement.RegisterDeviceNotificationHandle(staticDeviceNotificationHandler.Handle, HidGuid, out staticDeviceNotificationHandle);
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
		public static new void UnregisterStaticDeviceNotificationHandler()
		{
			lock (staticDeviceNotificationSyncObj)
			{
				// Keep track of the register/unregister requests.
				staticDeviceNotificationCounter--;

				// The last call to this method unregisters the notification.
				if (staticDeviceNotificationCounter == 0)
				{
					if (staticDeviceNotificationHandle == IntPtr.Zero)
						throw (new InvalidOperationException("Invalid state within USB HID Device object, please report this bug!"));

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
							DeviceEventArgs e = new DeviceEventArgs(DeviceClass.Hid, new DeviceInfo(devicePath));

							Debug.WriteLine("USB HID device connected:");
							Debug.Indent();
							Debug.WriteLine("Path = " + devicePath);
							Debug.WriteLine("Info = " + e.DeviceInfo);
							Debug.Unindent();

							EventHelper.FireAsync(DeviceConnected, typeof(HidDevice), e);
							break;
						}

						case DeviceEvent.Disconnected:
						{
							DeviceEventArgs e = new DeviceEventArgs(DeviceClass.Hid, new DeviceInfo(devicePath));

							Debug.WriteLine("USB HID device disconnected:");
							Debug.Indent();
							Debug.WriteLine("Path = " + devicePath);
							Debug.Unindent();

							EventHelper.FireAsync(DeviceDisconnected, typeof(HidDevice), e);
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

		private HidUsagePage usagePage;
		private HidUsageId   usageId;

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
			: base(HidGuid, path)
		{
			Initialize();
		}

		/// <summary></summary>
		public HidDevice(int vendorId, int productId)
			: base(HidGuid, vendorId, productId)
		{
			Initialize();
		}

		/// <summary></summary>
		public HidDevice(int vendorId, int productId, string serial)
			: base(HidGuid, vendorId, productId, serial)
		{
			Initialize();
		}

		/// <summary></summary>
		public HidDevice(DeviceInfo deviceInfo)
			: base(HidGuid, deviceInfo)
		{
			Initialize();
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
				RegisterAndAttachStaticDeviceEventHandlers();
		}

		/// <summary>
		/// Used to reinitialize the device in case of a reconnect.
		/// </summary>
		protected override void Reinitialize()
		{
			base.Reinitialize();
			GetDeviceCapabilities();
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		private void GetDeviceCapabilities()
		{
			SafeFileHandle deviceHandle;
			if (!string.IsNullOrEmpty(Path) && Win32.Hid.CreateSharedQueryOnlyDeviceHandle(Path, out deviceHandle))
			{
				try
				{
					Win32.Hid.NativeTypes.HIDP_CAPS capabilities = new Win32.Hid.NativeTypes.HIDP_CAPS();
					if (Win32.Hid.GetDeviceCapabilities(deviceHandle, ref capabilities))
					{
						this.usagePage = (HidUsagePageEx)capabilities.UsagePage;
						this.usageId   = (HidUsageIdEx)  capabilities.Usage; // The Win32 HIDP_CAPS structure is named 'Usage', not 'UsageId'.

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
						string usagePageValue = "0x" + this.usagePage.GetHashCode().ToString("X4", CultureInfo.InvariantCulture);
						string usageIdValue   = "0x" + this.usageId  .GetHashCode().ToString("X4", CultureInfo.InvariantCulture);

						string usagePageName = "<Unknown>";
						try
						{
							string name = Enum.GetName(typeof(HidUsagePage), this.usagePage);
							if (!string.IsNullOrEmpty(name))
								usagePageName = name;
							else if (this.usagePage.GetHashCode() >= 0xFF00) // Vendor-defined usage page.
								usagePageName = "VendorDefined";
						}
						catch { }

						string usageIdName = "<Unknown>";
						try
						{
							string name = Enum.GetName(typeof(HidUsageId), this.usageId);
							if (!string.IsNullOrEmpty(name))
								usageIdName = name;
							else if (this.usagePage.GetHashCode() >= 0xFF00) // Vendor-defined usage page also
								usageIdName = "VendorDefined";               //   results in vendor-defined usage.
						}
						catch { }

						System.Diagnostics.Debug.WriteLine("USB HID device usage information:");
						System.Diagnostics.Debug.Indent();
						System.Diagnostics.Debug.WriteLine("Usage page " + usagePageValue + " corresponds to '" + usagePageName + "'");
						System.Diagnostics.Debug.WriteLine("Usage ID   " + usageIdValue   + " corresponds to '" + usageIdName   + "'");
						System.Diagnostics.Debug.Unindent();
					}
				}
				finally
				{
					deviceHandle.Close();
				}
			}
		}

		private void RegisterAndAttachStaticDeviceEventHandlers()
		{
			RegisterStaticDeviceNotificationHandler();
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
		protected override void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				DetachAndUnregisterStaticDeviceEventHandlers();

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

		/// <summary></summary>
		[CLSCompliant(false)]
		public virtual HidUsagePage UsagePage
		{
			get { return (this.usagePage); }
		}

		/// <summary></summary>
		[CLSCompliant(false)]
		public virtual HidUsageId UsageId
		{
			get { return (this.usageId); }
		}

		/// <summary></summary>
		public virtual int InputReportByteLength
		{
			get { return (this.inputReportByteLength); }
		}

		/// <summary></summary>
		public virtual int OutputReportByteLength
		{
			get { return (this.outputReportByteLength); }
		}

		/// <summary></summary>
		public virtual int FeatureReportByteLength
		{
			get { return (this.featureReportByteLength); }
		}

		/// <summary></summary>
		public virtual int LinkCollectionNodes
		{
			get { return (this.linkCollectionNodes); }
		}

		/// <summary></summary>
		public virtual int InputButtonCaps
		{
			get { return (this.inputButtonCaps); }
		}

		/// <summary></summary>
		public virtual int InputValueCaps
		{
			get { return (this.inputValueCaps); }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Indices", Justification = "'Indices' is a correct English term and used throughout the .NET framework.")]
		public virtual int InputDataIndices
		{
			get { return (this.inputDataIndices); }
		}

		/// <summary></summary>
		public virtual int OutputButtonCaps
		{
			get { return (this.outputButtonCaps); }
		}

		/// <summary></summary>
		public virtual int OutputValueCaps
		{
			get { return (this.outputValueCaps); }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Indices", Justification = "'Indices' is a correct English term and used throughout the .NET framework.")]
		public virtual int OutputDataIndices
		{
			get { return (this.outputDataIndices); }
		}

		/// <summary></summary>
		public virtual int FeatureButtonCaps
		{
			get { return (this.featureButtonCaps); }
		}

		/// <summary></summary>
		public virtual int FeatureValueCaps
		{
			get { return (this.featureValueCaps); }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Indices", Justification = "'Indices' is a correct English term and used throughout the .NET framework.")]
		public virtual int FeatureDataIndices
		{
			get { return (this.featureDataIndices); }
		}

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
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
