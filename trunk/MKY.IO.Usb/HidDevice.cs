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
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;

using Microsoft.Win32.SafeHandles;

using MKY.Utilities.Event;
using MKY.Utilities.Windows.Forms;
using MKY.Utilities.Diagnostics;

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
		public static readonly Guid HidGuid = Utilities.Win32.Hid.GetHidGuid();

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
		public static new string[] GetDevices()
		{
			return (Utilities.Win32.DeviceManagement.GetDevicesFromGuid(HidGuid));
		}

		#endregion

		#region Static Methods > Device Notification
		//------------------------------------------------------------------------------------------
		// Static Methods > Device Notification
		//------------------------------------------------------------------------------------------

		private static NativeMessageHandler _staticDeviceNotificationWindow = new NativeMessageHandler(StaticDeviceNotificationHandler);
		private static IntPtr _staticDeviceNotificationHandle = IntPtr.Zero;

		private static void RegisterStaticDeviceNotificationHandler()
		{
			Utilities.Win32.DeviceManagement.RegisterDeviceNotificationHandle(_staticDeviceNotificationWindow.Handle, HidGuid, out _staticDeviceNotificationHandle);
		}

		private static void UnregisterStaticDeviceNotificationHandler()
		{
			Utilities.Win32.DeviceManagement.UnregisterDeviceNotificationHandle(_staticDeviceNotificationHandle);
		}

		private static void StaticDeviceNotificationHandler(ref Message m)
		{
			DeviceEvent de = MessageToDeviceEvent(ref m);

			if ((de == DeviceEvent.Connected) ||
				(de == DeviceEvent.Disconnected))
			{
				string devicePath;
				if (Utilities.Win32.DeviceManagement.DeviceChangeMessageToDevicePath(m, out devicePath))
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

		private HidUsage _usage;
		private int _usagePage;

		private int _inputReportLength;
		private int _outputReportLength;
		private int _featureReportLength;

		private int _linkCollectionNodes;
		private int _inputButtonCaps;
		private int _inputValueCaps;
		private int _inputDataIndices;
		private int _outputButtonCaps;
		private int _outputValueCaps;
		private int _outputDataIndices;
		private int _featureButtonCaps;
		private int _featureValueCaps;
		private int _featureDataIndices;

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
			if (Utilities.Win32.Hid.CreateSharedQueryOnlyDeviceHandle(SystemPath, out deviceHandle))
			{
				try
				{
					Utilities.Win32.Hid.HIDP_CAPS caps = Utilities.Win32.Hid.GetDeviceCapabilities(deviceHandle);

					_usage = (XHidUsage)Utilities.Win32.Hid.GetHidUsage(caps);
					_usagePage = caps.UsagePage;

					_inputReportLength   = caps.InputReportByteLength;
					_outputReportLength  = caps.OutputReportByteLength;
					_featureReportLength = caps.FeatureReportByteLength;

					_linkCollectionNodes = caps.NumberLinkCollectionNodes;
					_inputButtonCaps     = caps.NumberInputButtonCaps;
					_inputValueCaps      = caps.NumberInputValueCaps;
					_inputDataIndices    = caps.NumberInputDataIndices;
					_outputButtonCaps    = caps.NumberOutputButtonCaps;
					_outputValueCaps     = caps.NumberOutputValueCaps;
					_outputDataIndices   = caps.NumberOutputDataIndices;
					_featureButtonCaps   = caps.NumberFeatureButtonCaps;
					_featureValueCaps    = caps.NumberFeatureValueCaps;
					_featureDataIndices  = caps.NumberFeatureDataIndices;
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
		public virtual HidUsage Usage
		{
			get { return (_usage); }
		}

		/// <summary></summary>
		public virtual int UsagePage
		{
			get { return (_usagePage); }
		}

		/// <summary></summary>
		public virtual int InputReportLength
		{
			get { return (_inputReportLength); }
		}

		/// <summary></summary>
		public virtual int OutputReportLength
		{
			get { return (_outputReportLength); }
		}

		/// <summary></summary>
		public virtual int FeatureReportLength
		{
			get { return (_featureReportLength); }
		}

		/// <summary></summary>
		public virtual int LinkCollectionNodes
		{
			get { return (_linkCollectionNodes); }
		}

		/// <summary></summary>
		public virtual int InputButtonCaps
		{
			get { return (_inputButtonCaps); }
		}

		/// <summary></summary>
		public virtual int InputValueCaps
		{
			get { return (_inputValueCaps); }
		}

		/// <summary></summary>
		public virtual int InputDataIndices
		{
			get { return (_inputDataIndices); }
		}

		/// <summary></summary>
		public virtual int OutputButtonCaps
		{
			get { return (_outputButtonCaps); }
		}

		/// <summary></summary>
		public virtual int OutputValueCaps
		{
			get { return (_outputValueCaps); }
		}

		/// <summary></summary>
		public virtual int OutputDataIndices
		{
			get { return (_outputDataIndices); }
		}

		/// <summary></summary>
		public virtual int FeatureButtonCaps
		{
			get { return (_featureButtonCaps); }
		}

		/// <summary></summary>
		public virtual int FeatureValueCaps
		{
			get { return (_featureValueCaps); }
		}

		/// <summary></summary>
		public virtual int FeatureDataIndices
		{
			get { return (_featureDataIndices); }
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
