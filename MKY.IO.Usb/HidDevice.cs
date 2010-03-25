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
	public class HidDevice : Device, ISerial
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
							Debug.WriteLine("USB HID device " + devicePath + " connected");
							EventHelper.FireAsync(DeviceConnected, typeof(HidDevice), e);
							break;

						case DeviceEvent.Disconnected:
							Debug.WriteLine("USB HID device " + devicePath + " disconnected");
							EventHelper.FireAsync(DeviceDisconnected, typeof(HidDevice), e);
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

		private int _inputReportLength;
		private int _outputReportLength;

		private HidUsage _usage;

		private FileStream _file;

		/// <summary>
		/// Async receiving.
		/// </summary>
		private Queue<byte> _receiveQueue = new Queue<byte>();

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary>
		/// Fired after data has been received from the device.
		/// </summary>
		public event EventHandler DataReceived;

		/// <summary>
		/// Fired after data has completely be sent to the device.
		/// </summary>
		public event EventHandler DataSent;

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
			Utilities.Win32.Hid.HIDP_CAPS caps = Utilities.Win32.Hid.GetDeviceCapabilities(DeviceHandle);

			_inputReportLength = caps.InputReportByteLength;
			_outputReportLength = caps.OutputReportByteLength;

			_usage = (XHidUsage)Utilities.Win32.Hid.GetHidUsage(caps);

			Utilities.Win32.Hid.FlushQueue(DeviceHandle);

			//RegisterForDeviceNotifications();
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
		public virtual HidUsage Usage
		{
			get { return (_usage); }
		}

		/// <summary>
		/// Indicates whether the serial communication port to the device is open.
		/// </summary>
		/// <returns>
		/// true if the serial communication port is open; otherwise, false.
		/// </returns>
		public virtual bool IsOpen
		{
			get
			{
				AssertNotDisposed();

				return (Utilities.IO.XFile.IsReady(_file));
			}
		}

		/// <summary>
		/// Gets the amount of data received from the device that is available to read.
		/// </summary>
		/// <returns>
		/// The number of bytes of data received from the device.
		/// </returns>
		public virtual int BytesAvailable
		{
			get
			{
				AssertNotDisposed();

				int bytesAvailable = 0;
				lock (_receiveQueue)
				{
					bytesAvailable = _receiveQueue.Count;
				}
				return (bytesAvailable);
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		#region Methods > ISerial
		//------------------------------------------------------------------------------------------
		// Methods > ISerial
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Opens the serial communication port to the device.
		/// </summary>
		public virtual bool Open()
		{
			// The file may already exist and be ready.
			if (IsOpen)
				return (true);

			// Create a new file and begin to read data from the device.
			try
			{
				CreateFile();
				BeginAsyncRead();
				return (true);
			}
			catch (Exception lowLevelEx)
			{
				CloseFile();

				UsbException ex = new UsbException(lowLevelEx.Message);
				XDebug.WriteException(this, ex);
				throw (ex);
			}
		}

		/// <summary>
		/// Closes the serial communication port to the device.
		/// </summary>
		public virtual void Close()
		{
			CloseFile();
		}

		/// <summary>
		/// Receives data from the device into a receive buffer.
		/// </summary>
		/// <param name="data">
		/// An array of type System.Byte that is the storage location for the received data.
		/// </param>
		/// <returns>The number of bytes received.</returns>
		public virtual int Receive(out byte[] data)
		{
			// AssertNotDisposed() is called by IsOpen
			// OnDataReceived has been fired before

			int bytesReceived = 0;
			if (IsOpen)
			{
				lock (_receiveQueue)
				{
					bytesReceived = _receiveQueue.Count;
					data = new byte[bytesReceived];
					for (int i = 0; i < bytesReceived; i++)
						data[i] = _receiveQueue.Dequeue();
				}
			}
			else
			{
				data = new byte[] { };
			}
			return (bytesReceived);
		}

		/// <summary>
		/// Sends data to the device.
		/// </summary>
		/// <param name="data">
		/// An array of type System.Byte that contains the data to be sent.
		/// </param>
		public virtual void Send(byte[] data)
		{
			if (IsOpen)
			{
				try
				{
					_file.Write(data, 0, data.Length);
				}
				catch (Exception lowLevelEx)
				{
					CloseFile();

					UsbException ex = new UsbException(lowLevelEx.Message);
					XDebug.WriteException(this, ex);
					throw (ex);
				}
			}
		}

		#endregion

		#region Methods > File
		//------------------------------------------------------------------------------------------
		// Methods > File
		//------------------------------------------------------------------------------------------

		private void CreateFile()
		{
			_file = new FileStream(new SafeFileHandle(DeviceHandle.DangerousGetHandle(), false), FileAccess.Read | FileAccess.Write, _inputReportLength, true);
		}

		private void BeginAsyncRead()
		{
			byte[] inputReportBuffer = new byte[_inputReportLength];
			_file.BeginRead(inputReportBuffer, 0, _inputReportLength, new AsyncCallback(AsyncReadCompleted), inputReportBuffer);
		}

		private void AsyncReadCompleted(IAsyncResult result)
		{
			if (IsOpen) // Ensure not to forward any events during closing anymore.
			{
				byte[] inputReportBuffer = (byte[])result.AsyncState; // Retrieve the read data.
				try
				{
					// Finalize read. In case of an exception during read, this call throws it.
					_file.EndRead(result);

					// Read data on this thread.
					lock (_receiveQueue)
					{
						foreach (byte b in inputReportBuffer)
							_receiveQueue.Enqueue(b);
					}

					// Trigger the next async read.
					BeginAsyncRead();
				}
				catch
				{
					OnDisconnected_Sync(new EventArgs());
					OnError_Sync(new ErrorEventArgs("Error while reading an input report from the device"));
				}
			}
		}

		private void CloseFile()
		{
			if (_file != null)
			{
				_file.Dispose();
				_file = null;
			}
		}

		#endregion

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		/// <summary></summary>
		protected override void OnConnected_Async(EventArgs e)
		{
			base.OnConnected_Async(e);
		}

		/// <summary></summary>
		protected override void OnDisconnected_Async(EventArgs e)
		{
			CloseFile();
			base.OnDisconnected_Async(e);
		}

		/// <summary></summary>
		protected override void OnDisconnected_Sync(EventArgs e)
		{
			CloseFile();
			base.OnDisconnected_Sync(e);
		}

		/// <summary></summary>
		protected override void OnError_Sync(ErrorEventArgs e)
		{
			base.OnError_Sync(e);
		}

		/// <remarks>
		/// Asynchronously fire this event to prevent potential race conditions on closing.
		/// </remarks>
		protected virtual void OnDataReceived_Async(EventArgs e)
		{
			EventHelper.FireAsync(DataReceived, this, e);
		}

		/// <remarks>
		/// Asynchronously fire this event to prevent potential race conditions on closing.
		/// </remarks>
		protected virtual void OnDataSent_Async(EventArgs e)
		{
			EventHelper.FireAsync(DataSent, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
