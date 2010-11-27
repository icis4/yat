//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
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
using System.Windows.Forms;

using Microsoft.Win32.SafeHandles;

using MKY.Event;
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
		public static DeviceInfo[] GetDevices(HidUsagePage usagePage, HidUsage usage)
		{
			List<DeviceInfo> l = new List<DeviceInfo>();

			foreach (DeviceInfo di in GetDevices())
			{
				using (HidDevice device = new HidDevice(di))
				{
					if (device.UsagePage == usagePage)
						if (device.Usage == usage)
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

		private static NativeMessageHandler staticDeviceNotificationWindow = new NativeMessageHandler(StaticDeviceNotificationHandler);
		private static int    staticDeviceNotificationCounter = 0;
		private static IntPtr staticDeviceNotificationHandle = IntPtr.Zero;
		private static object staticDeviceNotificationSyncObj = new object();

		/// <remarks>
		/// \attention This function also exists in the other USB classes. Changes here must also be applied there.
		/// </remarks>
		public static new void RegisterStaticDeviceNotificationHandler()
		{
			lock (staticDeviceNotificationSyncObj)
			{
				// The first call to this method registers the notification.
				if (staticDeviceNotificationCounter == 0)
				{
					if (staticDeviceNotificationHandle == IntPtr.Zero)
						Win32.DeviceManagement.RegisterDeviceNotificationHandle(staticDeviceNotificationWindow.Handle, HidGuid, out staticDeviceNotificationHandle);
					else
						throw (new InvalidOperationException("Invalid state within USB HID Device object"));
				}

				// Keep track of the register/unregister requests.
				staticDeviceNotificationCounter++;
			}
		}

		/// <remarks>
		/// \attention This function also exists in the other USB classes. Changes here must also be applied there.
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
					if (staticDeviceNotificationHandle != IntPtr.Zero)
						Win32.DeviceManagement.UnregisterDeviceNotificationHandle(staticDeviceNotificationHandle);
					else
						throw (new InvalidOperationException("Invalid state within USB HID Device object"));

					staticDeviceNotificationHandle = IntPtr.Zero;
				}

				// Ensure that decrement never results in negative values.
				if (staticDeviceNotificationCounter < 0)
					staticDeviceNotificationCounter = 0;
			}
		}

		/// <remarks>
		/// \attention This function also exists in the other USB classes. Changes here must also be applied there.
		/// </remarks>
		private static void StaticDeviceNotificationHandler(ref Message m)
		{
			DeviceEvent de = MessageToDeviceEvent(ref m);

			if ((de == DeviceEvent.Connected) ||
				(de == DeviceEvent.Disconnected))
			{
				string devicePath;
				if (Win32.DeviceManagement.DeviceChangeMessageToDevicePath(m, out devicePath))
				{
					DeviceEventArgs e = new DeviceEventArgs(DeviceClass.Hid, new DeviceInfo(devicePath));
					switch (de)
					{
						case DeviceEvent.Connected:
						{
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
							Debug.WriteLine("USB HID device disconnected:");
							Debug.Indent();
							Debug.WriteLine("Path = " + devicePath);
							Debug.WriteLine("Info = " + e.DeviceInfo);
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
		private HidUsage usage;

		private int inputReportLength;
		private int outputReportLength;
		private int featureReportLength;

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
		public HidDevice(int vendorId, int productId, string serialNumber)
			: base(HidGuid, vendorId, productId, serialNumber)
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
			SafeFileHandle deviceHandle;
			if (Win32.Hid.CreateSharedQueryOnlyDeviceHandle(Path, out deviceHandle))
			{
				try
				{
					Win32.Hid.NativeTypes.HIDP_CAPS caps = Win32.Hid.GetDeviceCapabilities(deviceHandle);

					this.usagePage = (HidUsagePageEx)caps.UsagePage;
					this.usage     = (HidUsageEx)caps.Usage;

					this.inputReportLength   = caps.InputReportByteLength;
					this.outputReportLength  = caps.OutputReportByteLength;
					this.featureReportLength = caps.FeatureReportByteLength;

					this.linkCollectionNodes = caps.NumberLinkCollectionNodes;
					this.inputButtonCaps     = caps.NumberInputButtonCaps;
					this.inputValueCaps      = caps.NumberInputValueCaps;
					this.inputDataIndices    = caps.NumberInputDataIndices;
					this.outputButtonCaps    = caps.NumberOutputButtonCaps;
					this.outputValueCaps     = caps.NumberOutputValueCaps;
					this.outputDataIndices   = caps.NumberOutputDataIndices;
					this.featureButtonCaps   = caps.NumberFeatureButtonCaps;
					this.featureValueCaps    = caps.NumberFeatureValueCaps;
					this.featureDataIndices  = caps.NumberFeatureDataIndices;
				}
				finally
				{
					deviceHandle.Close();
				}

				// Only attach handlers if this is an instance of the USB HID device class.
				// If this instance is e.g. an Ser/HID device, handlers must be attached there.
				if (this is HidDevice)
					RegisterAndAttachStaticDeviceEventHandlers();
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
			if (disposing)
			{
				DetachAndUnregisterStaticDeviceEventHandlers();
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
		public virtual HidUsagePage UsagePage
		{
			get { return (this.usagePage); }
		}

		/// <summary></summary>
		public virtual HidUsage Usage
		{
			get { return (this.usage); }
		}

		/// <summary></summary>
		public virtual int InputReportLength
		{
			get { return (this.inputReportLength); }
		}

		/// <summary></summary>
		public virtual int OutputReportLength
		{
			get { return (this.outputReportLength); }
		}

		/// <summary></summary>
		public virtual int FeatureReportLength
		{
			get { return (this.featureReportLength); }
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
		public virtual int FeatureDataIndices
		{
			get { return (this.featureDataIndices); }
		}

		#endregion

		#region Event Handling
		//==========================================================================================
		// Event Handling
		//==========================================================================================

		private void Device_DeviceConnected(object sender, DeviceEventArgs e)
		{
			if (Info == e.DeviceInfo)
				OnConnected(new EventArgs());
		}

		private void Device_DeviceDisconnected(object sender, DeviceEventArgs e)
		{
			if (Info == e.DeviceInfo)
				OnDisconnected(new EventArgs());
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
