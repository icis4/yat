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
// Copyright � 2010 Matthias Kl�y.
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
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Windows.Forms;

using Microsoft.Win32.SafeHandles;

using MKY.Diagnostics;
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

		#region Static Lifetime
		//==========================================================================================
		// Static Lifetime
		//==========================================================================================

		static HidDevice()
		{
			RegisterStaticDeviceNotificationHandler();
		}

		// \todo 2010-03-21 / mky
		// Properly unregister without relying on garbage collection
		//
		//static ~HidDevice()
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
		private static IntPtr staticDeviceNotificationHandle = IntPtr.Zero;

		private static void RegisterStaticDeviceNotificationHandler()
		{
			Win32.DeviceManagement.RegisterDeviceNotificationHandle(staticDeviceNotificationWindow.Handle, HidGuid, out staticDeviceNotificationHandle);
		}

		private static void UnregisterStaticDeviceNotificationHandler()
		{
			Win32.DeviceManagement.UnregisterDeviceNotificationHandle(staticDeviceNotificationHandle);
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
					DeviceEventArgs e = new DeviceEventArgs(DeviceClass.Hid, devicePath);
					switch (de)
					{
						case DeviceEvent.Connected:
							Debug.WriteLine("USB HID device " + devicePath + " connected.");
							EventHelper.FireAsync(DeviceConnected, typeof(SerialHidDevice), e);
							break;

						case DeviceEvent.Disconnected:
							Debug.WriteLine("USB HID device " + devicePath + " disconnected.");
							EventHelper.FireAsync(DeviceDisconnected, typeof(SerialHidDevice), e);
							break;
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
		public HidDevice(string systemPath)
			: base(HidGuid, systemPath)
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

		private void Initialize()
		{
			SafeFileHandle deviceHandle;
			if (Win32.Hid.CreateSharedQueryOnlyDeviceHandle(SystemPath, out deviceHandle))
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
			}
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
				// Nothing to do (yet).
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
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================