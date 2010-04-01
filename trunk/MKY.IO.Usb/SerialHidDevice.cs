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
using System.Text;
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
	/// Extends a USB device with serial HID capabilities.
	/// </summary>
	public class SerialHidDevice : HidDevice, ISerial
	{
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

		static SerialHidDevice()
		{
			RegisterStaticDeviceNotificationHandler();
		}

		// \todo 2010-03-21 / mky
		// Properly unregister without relying on garbage collection
		//
		//static ~SerialHidDevice()
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

		/// <summary>
		/// It just a single stream object, but it contains the basically independent input and
		/// output streams.
		/// </summary>
		private FileStream _stream;

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
		/// Fired after port successfully opened.
		/// </summary>
		public event EventHandler Opened;

		/// <summary>
		/// Fired after port successfully closed.
		/// </summary>
		public event EventHandler Closed;

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
		public SerialHidDevice(string systemPath)
			: base(systemPath)
		{
		}

		/// <summary></summary>
		public SerialHidDevice(int vendorId, int productId)
			: base(vendorId, productId)
		{
		}

		/// <summary></summary>
		public SerialHidDevice(int vendorId, int productId, string serialNumber)
			: base(vendorId, productId, serialNumber)
		{
		}

		/// <summary></summary>
		public SerialHidDevice(DeviceInfo deviceInfo)
			: base(deviceInfo)
		{
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
				
				if (_stream != null)
					return ((_stream.CanRead) && (_stream.CanWrite));

				return (false);
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
		/// Opens the serial communication to the device.
		/// </summary>
		public virtual bool Open()
		{
			// The file may already exist and be ready.
			if (IsOpen)
				return (true);

			// Create a new file and begin to read data from the device.
			if (CreateStream())
			{
				BeginAsyncRead();
				OnOpened_Sync(new EventArgs());
				return (true);
			}
			else
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendLine("Couldn't open serial communication to USB HID device");
				sb.AppendLine(ToString());
				throw (new UsbException(sb.ToString()));
			}
		}

		/// <summary>
		/// Closes the serial communication port to the device.
		/// </summary>
		public virtual void Close()
		{
			CloseStream();
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
				Write(data);
		}

		#endregion

		#region Methods > Stream
		//------------------------------------------------------------------------------------------
		// Methods > Stream
		//------------------------------------------------------------------------------------------

		private bool CreateStream()
		{
			SafeFileHandle readWriteHandle;
			if (!Utilities.Win32.Hid.CreateExclusiveReadWriteHandle(SystemPath, out readWriteHandle))
				return (false);

			if (!Utilities.Win32.Hid.FlushQueue(readWriteHandle))
				return (false);

			_stream = new FileStream(readWriteHandle, FileAccess.Read | FileAccess.Write, InputReportLength, true);
			return (true);
		}

		private void BeginAsyncRead()
		{
			byte[] inputReportBuffer = new byte[InputReportLength];
			_stream.BeginRead(inputReportBuffer, 0, InputReportLength, new AsyncCallback(AsyncReadCompleted), inputReportBuffer);
		}

		private void AsyncReadCompleted(IAsyncResult result)
		{
			if (IsOpen) // Ensure not to perform any operations during closing anymore.
			{
				try
				{
					// Retrieve the read data and finalize read. In case of an exception
					// during the read, the call of EndRead() throws it.
					byte[] inputReportBuffer = (byte[])result.AsyncState; 
					_stream.EndRead(result);

					// Convert the input report into usable data.
					HidInputReportContainer input = new HidInputReportContainer(this);
					input.CreateDataFromReport(inputReportBuffer);

					// Don't care about report ID, Ser/HID only supports report 0.

					// Read data on this thread.
					lock (_receiveQueue)
					{
						foreach (byte b in input.Data)
							_receiveQueue.Enqueue(b);
					}

					// Asynchronously signal that data has been received.
					OnDataReceived_Async(new EventArgs());

					// Trigger the next async read.
					BeginAsyncRead();
				}
				catch (Exception ex)
				{
					XDebug.WriteException(this, ex);
					OnError_Sync(new ErrorEventArgs("Error while reading an input report from the USB HID device" + Environment.NewLine + ToString()));
				}
			}
		}

		private void Write(byte[] data)
		{
			try
			{
				// Report ID is fixed, Ser/HID only supports report 0.
				byte reportId = 0;

				HidOutputReportContainer output = new HidOutputReportContainer(this);
				output.CreateReportsFromData(reportId, data);

				foreach (byte[] report in output.Reports)
					_stream.Write(report, 0, report.Length);
			}
			catch (Exception ex)
			{
				XDebug.WriteException(this, ex);
				OnError_Sync(new ErrorEventArgs("Error while writing an output report to the USB HID device" + Environment.NewLine + ToString()));
			}
		}

		private void CloseStream()
		{
			CloseStream(false);
		}

		private void CloseStream(bool async)
		{
			bool wasOpen = false;
			if (IsOpen)
				wasOpen = true;

			if (_stream != null)
			{
				_stream.Close();
				_stream = null;
			}

			if (wasOpen)
			{
				if (async)
					OnClosed_Async(new EventArgs());
				else
					OnClosed_Sync(new EventArgs());
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
			CloseStream(true);
			base.OnDisconnected_Async(e);
		}

		/// <summary></summary>
		protected override void OnDisconnected_Sync(EventArgs e)
		{
			CloseStream();
			base.OnDisconnected_Sync(e);
		}

		/// <summary></summary>
		protected override void OnError_Sync(ErrorEventArgs e)
		{
			CloseStream();
			base.OnError_Sync(e);
		}

		/// <summary></summary>
		protected virtual void OnOpened_Sync(EventArgs e)
		{
			EventHelper.FireSync(Opened, this, e);
		}

		/// <summary></summary>
		protected virtual void OnClosed_Sync(EventArgs e)
		{
			EventHelper.FireSync(Closed, this, e);
		}

		/// <summary></summary>
		protected virtual void OnClosed_Async(EventArgs e)
		{
			EventHelper.FireAsync(Closed, this, e);
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
