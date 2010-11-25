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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Microsoft.Win32.SafeHandles;

using MKY.Diagnostics;
using MKY.Event;
using MKY.Windows.Forms;

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
		/// Returns an array of all USB Ser/HID devices currently available on the system.
		/// </summary>
		public static new DeviceInfo[] GetDevices()
		{
			// \fixme 2010-04-02 mky
			// Ser/HID should be Generic/Undefined
			//return (HidDevice.GetDevices(HidUsagePage.GenericDesktopControls, HidUsage.Undefined));
			return (HidDevice.GetDevices());
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
						{
							Debug.WriteLine("USB HID device connected:");
							Debug.Indent();
							Debug.WriteLine("Path = " + devicePath);
							Debug.Unindent();

							EventHelper.FireAsync(DeviceConnected, typeof(SerialHidDevice), e);
							break;
						}

						case DeviceEvent.Disconnected:
						{
							Debug.WriteLine("USB HID device disconnected:");
							Debug.Indent();
							Debug.WriteLine("Path = " + devicePath);
							Debug.Unindent();

							EventHelper.FireAsync(DeviceDisconnected, typeof(SerialHidDevice), e);
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

		/// <summary>
		/// It just a single stream object, but it contains the basically independent input and
		/// output streams.
		/// </summary>
		private FileStream stream;

		/// <summary>
		/// Async receiving.
		/// </summary>
		private Queue<byte> receiveQueue = new Queue<byte>();

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
		/// <c>true</c> if the serial communication port is open; otherwise, <c>false</c>.
		/// </returns>
		public virtual bool IsOpen
		{
			get
			{
				// \attention
				// Do not call AssertNotDisposed() since IsOpen is used by AsyncReadCompleted()
				// to detect devices that are just being closed or have already been closed.

				// \attention
				// CloseStream() intentionally sets this.stream to null to ensure that this
				// property also works during closing.

				if (this.stream != null)
					return ((this.stream.CanRead) && (this.stream.CanWrite));

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
				lock (this.receiveQueue)
				{
					bytesAvailable = this.receiveQueue.Count;
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
				OnOpened(new EventArgs());
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
				lock (this.receiveQueue)
				{
					bytesReceived = this.receiveQueue.Count;
					data = new byte[bytesReceived];
					for (int i = 0; i < bytesReceived; i++)
						data[i] = this.receiveQueue.Dequeue();
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
			if (!Win32.Hid.CreateSharedReadWriteHandle(SystemPath, out readWriteHandle))
				return (false);

			if (!Win32.Hid.FlushQueue(readWriteHandle))
				return (false);

			this.stream = new FileStream(readWriteHandle, FileAccess.Read | FileAccess.Write, InputReportLength, true);
			return (true);
		}

		private void BeginAsyncRead()
		{
			byte[] inputReportBuffer = new byte[InputReportLength];
			this.stream.BeginRead(inputReportBuffer, 0, InputReportLength, new AsyncCallback(AsyncReadCompleted), inputReportBuffer);
		}

		/// <summary>
		/// Asynchronously invoke incoming events to prevent potential dead-locks if close/dispose
		/// was called from a ISynchronizeInvoke target (i.e. a form) on an event thread.
		/// Also, the mechanism implemented below reduces the amount of events that are propagated
		/// to the main application. Small chunks of received data will generate many events
		/// handled by <see cref="AsyncReadCompleted"/>. However, since <see cref="OnDataReceived"/>
		/// synchronously invokes the event, it will take some time until the monitor is released
		/// again. During this time, no more new events are invoked, instead, incoming data is
		/// buffered.
		/// </summary>
		private delegate void AsyncReadCompletedDelegate();

		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "This field strictly belows to the method below and is therefore prefixed with the method's name.")]
		private object AsyncReadCompletedSyncObj = new object();

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		private void AsyncReadCompleted(IAsyncResult result)
		{
			if (!IsDisposed && IsOpen) // Ensure not to perform any operations during closing anymore.
			{
				try
				{
					// Immediately read data on this thread.

					// Retrieve the read data and finalize read. In case of an exception
					// during the read, the call of EndRead() throws it.
					byte[] inputReportBuffer = (byte[])result.AsyncState; 
					this.stream.EndRead(result);

					// Convert the input report into usable data.
					HidInputReportContainer input = new HidInputReportContainer(this);
					input.CreateDataFromReport(inputReportBuffer);

					// Don't care about report ID, Ser/HID only supports report 0.

					// Read data on this thread.
					lock (this.receiveQueue)
					{
						foreach (byte b in input.Data)
							this.receiveQueue.Enqueue(b);
					}

					// Ensure that only one data received event thread is active at the same time.
					// Without this exclusivity, two receive threads could create a race condition.
					if (Monitor.TryEnter(AsyncReadCompletedSyncObj))
					{
						try
						{
							AsyncReadCompletedDelegate asyncInvoker = new AsyncReadCompletedDelegate(AsyncReadCompletedAsync);
							asyncInvoker.BeginInvoke(null, null);
						}
						finally
						{
							Monitor.Exit(AsyncReadCompletedSyncObj);
						}
					}

					// Trigger the next async read.
					BeginAsyncRead();
				}
				catch (Exception ex)
				{
					DebugEx.WriteException(this.GetType(), ex);
					OnError(new ErrorEventArgs("Error while reading an input report from the USB HID device" + Environment.NewLine + ToString()));
				}
			}
		}

		private void AsyncReadCompletedAsync()
		{
			// Ensure that only one data received event thread is active at the same time.
			// Without this exclusivity, two receive threads could create a race condition.
			Monitor.Enter(AsyncReadCompletedSyncObj);
			try
			{
				// Fire events until there is no more data available. Must be done to ensure
				// that events are fired even for data that was enqueued above while the sync
				// obj was busy.
				while (BytesAvailable > 0)
					OnDataReceived(new EventArgs());
			}
			finally
			{
				Monitor.Exit(AsyncReadCompletedSyncObj);
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		private void Write(byte[] data)
		{
			try
			{
				// Report ID is fixed, Ser/HID only supports report 0.
				byte reportId = 0;

				HidOutputReportContainer output = new HidOutputReportContainer(this);
				output.CreateReportsFromData(reportId, data);

				foreach (byte[] report in output.Reports)
					this.stream.Write(report, 0, report.Length);
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(this.GetType(), ex);
				OnError(new ErrorEventArgs("Error while writing an output report to the USB HID device" + Environment.NewLine + ToString()));
			}
		}

		private void CloseStream()
		{
			bool wasOpen = false;
			if (IsOpen)
				wasOpen = true;

			if (this.stream != null)
			{
				// \attention
				// Set this.stream to null before initiating Close() to ensure that the IsOpen
				// property returns false during closing. AsyncReadCompleted() will be called
				// when Close() is initiated. AsyncReadCompleted() will check IsOpen.

				FileStream fs = this.stream;
				this.stream = null;
				fs.Close();
			}

			if (wasOpen)
				OnClosed(new EventArgs());
		}

		#endregion

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		/// <summary></summary>
		protected override void OnConnected(EventArgs e)
		{
			base.OnConnected(e);
		}

		/// <summary></summary>
		protected override void OnDisconnected(EventArgs e)
		{
			CloseStream();
			base.OnDisconnected(e);
		}

		/// <summary></summary>
		protected override void OnError(ErrorEventArgs e)
		{
			CloseStream();
			base.OnError(e);
		}

		/// <summary></summary>
		protected virtual void OnOpened(EventArgs e)
		{
			EventHelper.FireSync(Opened, this, e);
		}

		/// <summary></summary>
		protected virtual void OnClosed(EventArgs e)
		{
			EventHelper.FireSync(Closed, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDataReceived(EventArgs e)
		{
			EventHelper.FireSync(DataReceived, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDataSent(EventArgs e)
		{
			EventHelper.FireSync(DataSent, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
