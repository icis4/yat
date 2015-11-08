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
// MKY Development Version 1.0.14
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

#region Configuration
//==================================================================================================
// Configuration
//==================================================================================================

#if (DEBUG)

	// Enable debugging of thread state:
////#define DEBUG_THREAD_STATE

#endif // DEBUG

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Microsoft.Win32.SafeHandles;

using MKY.Diagnostics;
using MKY.Windows.Forms;

#endregion

namespace MKY.IO.Usb
{
	/// <summary>
	/// Extends a USB device with serial HID capabilities.
	/// </summary>
	public class SerialHidDevice : HidDevice, ISerial
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private enum State
		{
			Reset,
			Starting,
			Started,
			ConnectedAndClosed,
			ConnectedAndOpened,
			DisconnectedAndWaitingForReopen,
			DisconnectedAndClosed,
			Error,
		}

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <summary>
		/// By default, the USB device is automatically opened as soon as it gets connected to the
		/// computer, given this device object is up and running.
		/// </summary>
		public const bool AutoOpenDefault = true;

		private const int ReceiveQueueInitialCapacity = 4096;

		private const int ThreadWaitTimeout = 200;

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

		#region Static Fields
		//==========================================================================================
		// Static Fields
		//==========================================================================================

		private static Random staticRandom = new Random(RandomEx.NextPseudoRandomSeed());

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
			// \remind MKY 2013-06-08:
			// It is not possible to limit this list/array to true Ser/HID devices, as there is no
			// standardized way to retrieve whether a device is Ser/HID capable. However, this list
			// could be limited to vendor-specific usages. Feature request #195 "USB Ser/HID device
			// list could be limited to vendor-specific usages" deals with this potential feature.
			return (HidDevice.GetDevices());
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
		public static new bool RegisterStaticDeviceNotificationHandler()
		{
			bool result = false;

			lock (staticDeviceNotificationSyncObj)
			{
				// The first call to this method registers the notification.
				if (staticDeviceNotificationCounter == 0)
				{
					if (staticDeviceNotificationHandle != IntPtr.Zero)
						throw (new InvalidOperationException("Invalid state within USB Ser/HID Device object, please report this bug!"));

					if (NativeMessageHandler.MessageSourceIsRegistered)
					{
						staticDeviceNotificationHandler = new NativeMessageHandler(StaticMessageCallback);
						Win32.DeviceManagement.RegisterDeviceNotificationHandle(staticDeviceNotificationHandler.Handle, HidDevice.HidGuid, out staticDeviceNotificationHandle);
						result = true;
					}
				}

				// Keep track of the register/unregister requests.
				if (staticDeviceNotificationCounter < int.MaxValue)
					staticDeviceNotificationCounter++;
			}

			return (result);
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
				if (staticDeviceNotificationCounter > int.MinValue)
					staticDeviceNotificationCounter--;

				// The last call to this method unregisters the notification.
				if (staticDeviceNotificationCounter == 0)
				{
					if (staticDeviceNotificationHandle == IntPtr.Zero)
						throw (new InvalidOperationException("Invalid state within USB Ser/HID Device object, please report this bug!"));

					if (staticDeviceNotificationHandle != null)
					{
						Win32.DeviceManagement.UnregisterDeviceNotificationHandle(staticDeviceNotificationHandle);
						staticDeviceNotificationHandle = IntPtr.Zero;
					}
				}
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

							Debug.WriteLine("USB Ser/HID device connected:");
							Debug.Indent();
							Debug.WriteLine("Path = " + devicePath);
							Debug.WriteLine("Info = " + e.DeviceInfo);
							Debug.Unindent();

							EventHelper.FireAsync(DeviceConnected, typeof(SerialHidDevice), e);
							break;
						}

						case DeviceEvent.Disconnected:
						{
							DeviceEventArgs e = new DeviceEventArgs(DeviceClass.Hid, new DeviceInfo(devicePath));

							Debug.WriteLine("USB Ser/HID device disconnected:");
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

		private State state = State.Reset;
		private ReaderWriterLockSlim stateLock = new ReaderWriterLockSlim();

		private SerialHidReportFormat reportFormat = new SerialHidReportFormat();
		private SerialHidRxIdUsage    rxIdUsage    = new SerialHidRxIdUsage();

		private bool autoOpen = AutoOpenDefault;

		/// <remarks>
		/// It's just a single stream object, but it contains the basically independent input and
		/// output streams.
		/// </remarks>
		private FileStream stream;

		/// <remarks>
		/// Async receiving. The capacity is set large enough to reduce the number of resizing
		/// operations while adding elements.
		/// </remarks>
		private Queue<byte> receiveQueue = new Queue<byte>(ReceiveQueueInitialCapacity);

		private bool receiveThreadRunFlag;
		private AutoResetEvent receiveThreadEvent;
		private Thread receiveThread;
		private object receiveThreadSyncObj = new object();

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
		public SerialHidDevice(string path)
			: base(path)
		{
			Initialize();
		}

		/// <summary></summary>
		public SerialHidDevice(int vendorId, int productId)
			: base(vendorId, productId)
		{
			Initialize();
		}

		/// <summary></summary>
		public SerialHidDevice(int vendorId, int productId, string serial)
			: base(vendorId, productId, serial)
		{
			Initialize();
		}

		/// <summary></summary>
		public SerialHidDevice(DeviceInfo deviceInfo)
			: base(deviceInfo)
		{
			Initialize();
		}

		/// <remarks>
		/// Base constructor creates device info and therefore also sets system path.
		/// </remarks>
		private void Initialize()
		{
			// Only attach handlers if this is an instance of the USB Ser/HID device class.
			// If this instance is of a derived class, handlers must be attached there.
			if (GetType() == typeof(SerialHidDevice))
				RegisterAndAttachStaticDeviceEventHandlers();
		}

		private void RegisterAndAttachStaticDeviceEventHandlers()
		{
			DeviceConnected    += new EventHandler<DeviceEventArgs>(Device_DeviceConnected);
			DeviceDisconnected += new EventHandler<DeviceEventArgs>(Device_DeviceDisconnected);

			RegisterStaticDeviceNotificationHandler();
		}

		private void DetachAndUnregisterStaticDeviceEventHandlers()
		{
			UnregisterStaticDeviceNotificationHandler();

			DeviceConnected    -= new EventHandler<DeviceEventArgs>(Device_DeviceConnected);
			DeviceDisconnected -= new EventHandler<DeviceEventArgs>(Device_DeviceDisconnected);
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
				Stop();

				// Dispose of managed resources if requested:
				if (disposing)
				{
					// In the 'normal' case, the receive thread will already have been stopped in Close().
					StopReceiveThread();

					this.stateLock.Dispose();
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
		/// Indicates the HID report format of the current device.
		/// </summary>
		public virtual SerialHidReportFormat ReportFormat
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.reportFormat);
			}
			set
			{
				AssertNotDisposed();

				this.reportFormat = value;
			}
		}

		/// <summary>
		/// Indicates how the ID is used while receiving.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Rx", Justification = "'Rx' is a common term in serial communication.")]
		public virtual SerialHidRxIdUsage RxIdUsage
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.rxIdUsage);
			}
			set
			{
				AssertNotDisposed();

				this.rxIdUsage = value;
			}
		}

		/// <summary>
		/// Indicates whether the device automatically tries to open.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the device automatically tries to open; otherwise, <c>false</c>.
		/// </returns>
		public virtual bool AutoOpen
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.autoOpen);
			}
			set
			{
				AssertNotDisposed();

				this.autoOpen = value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the device has been started.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the device has been started; otherwise, <c>false</c>.
		/// </returns>
		public virtual bool IsStarted
		{
			get
			{
				// \attention:
				// Do not call AssertNotDisposed() since IsOpen is used by AsyncReadCompleted()
				// to detect devices that are just being closed or have already been closed.

				switch (this.state)
				{
					case State.Started:
					case State.ConnectedAndClosed:
					case State.ConnectedAndOpened:
					case State.DisconnectedAndWaitingForReopen:
					case State.DisconnectedAndClosed:
						return (true);

					default:
						return (false);
				}
			}
		}

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
				// \attention:
				// Do not call AssertNotDisposed() since IsOpen is used by AsyncReadCompleted()
				// to detect devices that are just being closed or have already been closed.

				// \attention:
				// CloseStream() intentionally sets this.stream to null to ensure that this
				// property also works during closing.

				if (IsStarted && (this.stream != null))
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
		/// Starts the device.
		/// </summary>
		public virtual bool Start()
		{
			AssertNotDisposed();

			WriteDebugMessageLine("Starting...");
			SetStateSynchronized(State.Starting);
			if (Open())
			{
				SetStateSynchronized(State.Started);
				return (true);
			}
			else
			{
				SetStateSynchronized(State.Reset);
				return (false);
			}
		}

		/// <summary>
		/// Stops the device.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "Stop is a common term to start/stop something.")]
		public virtual void Stop()
		{
			AssertNotDisposed();

			WriteDebugMessageLine("Stopping...");
			Close();
			SetStateSynchronized(State.Reset);
		}

		/// <summary>
		/// Opens the serial communication to the device.
		/// </summary>
		public virtual bool Open()
		{
			AssertNotDisposed();

			// The stream may already exist and be ready.
			if (IsOpen)
				return (true);

			WriteDebugMessageLine("Opening...");
			CreateAndStartReceiveThread();

			// Create a new stream and begin to read data from the device.
			if (CreateStream())
			{
				SetStateSynchronized(State.ConnectedAndOpened);
				OnOpened(EventArgs.Empty);
				return (true);
			}
			else
			{
				return (false);
			}
		}

		/// <summary>
		/// Closes the serial communication to the device.
		/// </summary>
		public virtual void Close()
		{
			AssertNotDisposed();

			// The stream may already be closed.
			if (!IsOpen)
				return;

			WriteDebugMessageLine("Closing...");
			StopReceiveThread();
			CloseStream();

			if (IsConnected)
				SetStateSynchronized(State.ConnectedAndClosed);
			else if (AutoOpen)
				SetStateSynchronized(State.DisconnectedAndWaitingForReopen);
			else
				SetStateSynchronized(State.DisconnectedAndClosed);

			OnClosed(EventArgs.Empty);
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
			AssertNotDisposed();

			// OnDataReceived has been fired before.

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
			AssertNotDisposed();

			// OnDataSent is fired by Write.

			if (IsOpen)
				Write(data);
		}

		#endregion

		#region Methods > Threads
		//------------------------------------------------------------------------------------------
		// Methods > Threads
		//------------------------------------------------------------------------------------------

		private void CreateAndStartReceiveThread()
		{
			lock (this.receiveThreadSyncObj)
			{
				if (this.receiveThread == null)
				{
					this.receiveThreadRunFlag = true;
					this.receiveThreadEvent = new AutoResetEvent(false);
					this.receiveThread = new Thread(new ThreadStart(ReceiveThread));
					this.receiveThread.Name = ToString() + " Receive Thread";
					this.receiveThread.Start();
				}
			}
		}

		private void StopReceiveThread()
		{
			lock (this.receiveThreadSyncObj)
			{
				if (this.receiveThread != null)
				{
					WriteDebugThreadStateMessageLine("ReceiveThread() gets stopped...");

					this.receiveThreadRunFlag = false;

					// Ensure that receive thread has stopped after the stop request:
					try
					{
						int accumulatedTimeout = 0;
						int interval = 0; // Use a relatively short random interval to trigger the thread:
						while (!this.receiveThread.Join(interval = staticRandom.Next(5, 20)))
						{
							this.receiveThreadEvent.Set();

							accumulatedTimeout += interval;
							if (accumulatedTimeout >= ThreadWaitTimeout)
							{
								WriteDebugThreadStateMessageLine("...failed! Aborting...");
								WriteDebugThreadStateMessageLine("(Abort is likely required due to failed synchronization back the calling thread, which is typically the GUI/main thread.)");
								this.receiveThread.Abort();
								break;
							}

							WriteDebugThreadStateMessageLine("...trying to join at " + accumulatedTimeout + " ms...");
						}
					}
					catch (ThreadStateException)
					{
						// Ignore thread state exceptions such as "Thread has not been started" and
						// "Thread cannot be aborted" as it just needs to be ensured that the thread
						// has or will be terminated for sure.

						WriteDebugThreadStateMessageLine("...failed too but will be exectued as soon as the calling thread gets suspended again.");
					}

					this.receiveThreadEvent.Close();
					this.receiveThread = null;

					WriteDebugThreadStateMessageLine("...successfully terminated.");
				}
			}
		}

		#endregion

		#region Methods > Stream
		//------------------------------------------------------------------------------------------
		// Methods > Stream
		//------------------------------------------------------------------------------------------

		private bool CreateStream()
		{
			SafeFileHandle readWriteHandle;
			if (!Win32.Hid.CreateSharedReadWriteHandle(Path, out readWriteHandle))
				return (false);

			if (!Win32.Hid.FlushQueue(readWriteHandle))
				return (false);

			this.stream = new FileStream(readWriteHandle, FileAccess.Read | FileAccess.Write, InputReportByteLength, true);

			// Immediately start reading.
			BeginAsyncRead();

			return (true);
		}

		private void BeginAsyncRead()
		{
			byte[] inputReportBuffer = new byte[InputReportByteLength];
			this.stream.BeginRead(inputReportBuffer, 0, InputReportByteLength, new AsyncCallback(AsyncReadCompleted), inputReportBuffer);
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		private void AsyncReadCompleted(IAsyncResult result)
		{
			if (!IsDisposed && IsOpen) // Ensure not to perform any operations during closing anymore. Check 'IsDisposed' first!
			{
				try
				{
					// Immediately read data on this thread.

					// Retrieve the read data and finalize read. In case of an exception during
					// the read, the call of EndRead() throws it. If this happens, e.g. due to
					// disconnect, exception is caught further down and stream is closed:
					byte[] inputReportBuffer = (byte[])result.AsyncState;
					this.stream.EndRead(result);

					// Convert the input report into usable data:
					SerialHidInputReportContainer input = new SerialHidInputReportContainer(this);
					input.ProcessReport(this.reportFormat, inputReportBuffer);

					// Evaluate whether report can be accepted, based on ID and configuration:
					bool acceptReport = true;
					if (this.reportFormat.UseId)
					{
						if (!this.rxIdUsage.SeparateRxId) // Common case = Same ID for Tx and Rx.
						{
							acceptReport = (input.Id == this.reportFormat.Id);
						}
						else // Special case = Separate ID for Rx.
						{
							if (this.rxIdUsage.AnyRxId)
								acceptReport = true;
							else
								acceptReport = (input.Id == this.rxIdUsage.RxId);
						}
					}

					if (acceptReport)
					{
						// Read data on this thread:
						lock (this.receiveQueue)
						{
							foreach (byte b in input.Payload)
								this.receiveQueue.Enqueue(b);
						}

						// Signal receive thread:
						this.receiveThreadEvent.Set();
					}

					// Trigger the next async read.
					BeginAsyncRead();
				}
				catch (IOException ex) // Includes Close().
				{
					string message = "Disconnect detected while reading from device.";
					DebugEx.WriteException(GetType(), ex, message);
					OnDisconnected(EventArgs.Empty);
				}
				catch (Exception ex)
				{
					string message = "Error while reading an input report from the USB Ser/HID device" + Environment.NewLine + ToString();
					DebugEx.WriteException(GetType(), ex, message);
					OnIOError(new ErrorEventArgs(message));
				}
			}
		}

		/// <summary>
		/// Asynchronously manage incoming events to prevent potential deadlocks if close/dispose
		/// was called from a ISynchronizeInvoke target (i.e. a form) on an event thread.
		/// Also, the mechanism implemented below reduces the amount of events that are propagated
		/// to the main application. Small chunks of received data will generate many events
		/// handled by <see cref="AsyncReadCompleted"/>. However, since <see cref="OnDataReceived"/>
		/// synchronously invokes the event, it will take some time until the send queue is checked
		/// again. During this time, no more new events are invoked, instead, outgoing data is
		/// buffered.
		/// </summary>
		/// <remarks>
		/// Will be signaled by <see cref="AsyncReadCompleted"/> event above.
		/// </remarks>
		[SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", MessageId = "System.Threading.WaitHandle.#WaitOne(System.Int32)", Justification = "Installer indeed targets .NET 3.5 SP1.")]
		private void ReceiveThread()
		{
			WriteDebugThreadStateMessageLine("ReceiveThread() has started.");

			// Outer loop, requires another signal.
			while (this.receiveThreadRunFlag && !IsDisposed) // Check 'IsDisposed' first!
			{
				try
				{
					// WaitOne() will wait forever if the underlying I/O provider has crashed, or
					// if the overlying client isn't able or forgets to call Stop() or Dispose().
					// Therefore, only wait for a certain period and then poll the run flag again.
					// The period can be quite long, as an event trigger will immediately resume.
					if (!this.receiveThreadEvent.WaitOne(staticRandom.Next(50, 200)))
						continue;
				}
				catch (AbandonedMutexException ex)
				{
					// The mutex should never be abandoned, but in case it nevertheless happens,
					// at least output a debug message and gracefully exit the thread.
					DebugEx.WriteException(GetType(), ex, "An 'AbandonedMutexException' occurred in ReceiveThread()!");
					break;
				}

				// Inner loop, runs as long as there is data to be received. Must be done to
				// ensure that events are fired even for data that was enqueued above while the
				// 'OnDataReceived' event was being handled.
				// 
				// Ensure not to forward any events during closing anymore.
				while (!IsDisposed && this.receiveThreadRunFlag && IsOpen && (BytesAvailable > 0)) // Check 'IsDisposed' first!
				{
					OnDataReceived(EventArgs.Empty);

					// Wait for the minimal time possible to allow other threads to execute and
					// to prevent that 'DataReceived' events are fired consecutively.
					Thread.Sleep(TimeSpan.Zero);
				}
			}

			WriteDebugThreadStateMessageLine("ReceiveThread() has terminated.");
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		private void Write(byte[] payload)
		{
			try
			{
				SerialHidOutputReportContainer output = new SerialHidOutputReportContainer(this);
				output.CreateReports(this.reportFormat, payload);

				foreach (byte[] report in output.Reports)
					this.stream.Write(report, 0, report.Length);

				OnDataSent(EventArgs.Empty);
			}
			catch (Exception ex)
			{
				string message = "Error while writing an output report to the USB Ser/HID device" + Environment.NewLine + ToString();
				DebugEx.WriteException(GetType(), ex);
				OnIOError(new ErrorEventArgs(message));
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		private void CloseStream()
		{
			if (this.stream != null)
			{
				try
				{
					// \attention:
					// Set this.stream to null before initiating Close() to ensure that the IsOpen
					// property returns false during closing. AsyncReadCompleted() will be called
					// when Close() is initiated. AsyncReadCompleted() will check IsOpen.

					FileStream fs = this.stream;
					this.stream = null;
					fs.Close();
				}
				catch (Exception ex)
				{
					DebugEx.WriteException(GetType(), ex);
				}
			}
		}

		#endregion

		#region Methods > State
		//------------------------------------------------------------------------------------------
		// Methods > State
		//------------------------------------------------------------------------------------------

		private State GetStateSynchronized()
		{
			State state;

			this.stateLock.EnterReadLock();
			state = this.state;
			this.stateLock.ExitReadLock();

			return (state);
		}

		private void SetStateSynchronized(State state)
		{
#if (DEBUG)
			State oldState = this.state;
#endif
			this.stateLock.EnterWriteLock();
			this.state = state;
			this.stateLock.ExitWriteLock();
#if (DEBUG)
			if (this.state != oldState)
				WriteDebugMessageLine("State has changed from " + oldState + " to " + this.state + ".");
			else
				WriteDebugMessageLine("State is already " + oldState + ".");
#endif
		}

		#endregion

		#region Methods > Reports
		//------------------------------------------------------------------------------------------
		// Methods > Reports
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public int CalculateTotalReportByteLength(byte[] payload)
		{
			SerialHidOutputReportContainer dummy = new SerialHidOutputReportContainer(this);
			dummy.CreateReports(this.reportFormat, payload);

			int byteLength = 0;
			foreach (byte[] report in dummy.Reports)
				byteLength += report.Length;

			return (byteLength);
		}

		#endregion
		
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

				if (AutoOpen)
					Open();
			}
		}

		/// <remarks>
		/// \attention:
		/// This function similarly exists in the other USB classes. Changes here may also be applied there.
		/// </remarks>
		private void Device_DeviceDisconnected(object sender, DeviceEventArgs e)
		{
			if (Info == e.DeviceInfo)
				OnDisconnected(EventArgs.Empty); // Includes Close().
		}

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
			Close();
			base.OnDisconnected(e);
		}

		/// <summary></summary>
		protected override void OnIOError(ErrorEventArgs e)
		{
			Close();
			base.OnIOError(e);
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

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		[Conditional("DEBUG")]
		private void WriteDebugMessageLine(string message)
		{
			Debug.WriteLine
			(
				string.Format
				(
					CultureInfo.InvariantCulture,
					" @ {0} @ Thread #{1} : {2,36} {3,3} {4,-38} : {5}",
					DateTime.Now.ToString("HH:mm:ss.fff", DateTimeFormatInfo.InvariantInfo),
					Thread.CurrentThread.ManagedThreadId.ToString("D3", CultureInfo.InvariantCulture),
					GetType(),
					"",
					"[" + ToString() + "]",
					message
				)
			);
		}

		[Conditional("DEBUG_THREAD_STATE")]
		private void WriteDebugThreadStateMessageLine(string message)
		{
			WriteDebugMessageLine(message);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
