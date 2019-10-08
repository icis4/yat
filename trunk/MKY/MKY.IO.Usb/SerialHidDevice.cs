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
// Copyright © 2010-2019 Matthias Kläy.
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
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Order according to meaning.")]
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
			Stopping,
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
		/// By default, the USB device serial string must match the connected device.
		/// </summary>
		public const bool MatchSerialDefault = true;

		/// <remarks>
		/// None shall be the preset, the report format and filter usage defaults still result in
		/// the <see cref="SerialHidDeviceSettingsPreset.Common"/> default.
		/// </remarks>
		public const SerialHidDeviceSettingsPreset PresetDefault = SerialHidDeviceSettingsPreset.None;

		/// <summary></summary>
		public static readonly SerialHidReportFormat ReportFormatDefault = new SerialHidReportFormat
		(
			SerialHidReportFormat.UseIdDefault,
			SerialHidReportFormat.IdDefault,
			SerialHidReportFormat.PrependPayloadByteLengthDefault,
			SerialHidReportFormat.AppendTerminatingZeroDefault,
			SerialHidReportFormat.FillLastReportDefault
		);

		/// <summary></summary>
		public static readonly SerialHidRxFilterUsage RxFilterUsageDefault = new SerialHidRxFilterUsage
		(
			SerialHidRxFilterUsage.SeparateRxIdDefault,
			SerialHidRxFilterUsage.AnyRxIdDefault,
			SerialHidRxFilterUsage.RxIdDefault
		);

		/// <summary>
		/// By default, the USB device is automatically opened as soon as it gets connected to the
		/// computer, given this device object is up and running.
		/// </summary>
		public const bool AutoOpenDefault = true;

		/// <summary>
		/// By default, the USB device hides non-payload data such as the report ID, the optional
		/// payload length, the optional terminating zero or the filler bytes.
		/// </summary>
		public const bool IncludeNonPayloadDataDefault = false;

		private const int ReceiveQueueInitialCapacity = 4096;

		private const int ThreadWaitTimeout = 500; // Enough time to let the threads join...

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
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static new DeviceInfo[] GetDevices(bool retrieveStringsFromDevice = true)
		{
			// \remind (MKY 2013-06-08)
			// It is not possible to limit this list/array to true Ser/HID devices, as there is no
			// standardized way to retrieve whether a device is Ser/HID capable. However, this list
			// could be limited to vendor-specific usages. Feature request #195 "USB Ser/HID device
			// list could be limited to vendor-specific usages" deals with this potential feature.
			return (HidDevice.GetDevices(retrieveStringsFromDevice));
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
		public static new bool RegisterStaticDeviceNotificationHandler()
		{
			bool success = false;

			lock (staticDeviceNotificationSyncObj)
			{
				// The first call to this method registers the notification:
				if (staticDeviceNotificationCounter == 0)
				{
					if (staticDeviceNotificationHandle != IntPtr.Zero)
						throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "Invalid state within USB Ser/HID device object!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

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
					throw (new OverflowException("Too many USB Ser/HID device notification registrations! It is required to restart the application!"));
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
							var e = new DeviceEventArgs(DeviceClass.Hid, new DeviceInfo(devicePath));

							Debug.WriteLine("USB Ser/HID device connected:");
							Debug.Indent();
							Debug.WriteLine("Path = " + devicePath);
							Debug.WriteLine("Info = " + e.DeviceInfo);
							Debug.Unindent();

							EventHelper.RaiseAsync(DeviceConnected, typeof(SerialHidDevice), e);
							break;
						}

						case DeviceEvent.Disconnected:
						{
							var e = new DeviceEventArgs(DeviceClass.Hid, new DeviceInfo(devicePath));

							Debug.WriteLine("USB Ser/HID device disconnected:");
							Debug.Indent();
							Debug.WriteLine("Path = " + devicePath);
							Debug.Unindent();

							EventHelper.RaiseAsync(DeviceDisconnected, typeof(SerialHidDevice), e);
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
		/// A dedicated event helper to allow discarding exceptions when object got disposed.
		/// </summary>
		private EventHelper.Item eventHelper = EventHelper.CreateItem(typeof(SerialHidDevice).FullName);

		private State state = State.Reset;
		private object stateSyncObj = new object();

		private bool matchSerial = MatchSerialDefault;

		private SerialHidReportFormat reportFormat = ReportFormatDefault;
		private SerialHidRxFilterUsage rxFilterUsage = RxFilterUsageDefault;

		private bool autoOpen = AutoOpenDefault;

		private bool includeNonPayloadData = IncludeNonPayloadDataDefault;

		/// <remarks>
		/// It's just a single stream object, but it contains the basically independent input and
		/// output streams.
		/// </remarks>
		private FileStream stream;

		/// <remarks>
		/// Async receiving. The capacity is set large enough to reduce the number of resizing
		/// operations while adding items.
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
		/// Event raised after port successfully opened.
		/// </summary>
		public event EventHandler Opened;

		/// <summary>
		/// Event raised after port successfully closed.
		/// </summary>
		public event EventHandler Closed;

		/// <summary>
		/// Event raised after data has been received from the device.
		/// </summary>
		public event EventHandler DataReceived;

		/// <summary>
		/// Event raised after data has completely be sent to the device.
		/// </summary>
		public event EventHandler<DataEventArgs> DataSent;

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
				AttachAndRegisterStaticDeviceEventHandlers();
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
				DebugEventManagement.DebugWriteAllEventRemains(this);
				this.eventHelper.DiscardAllEventsAndExceptions();

				UnregisterAndDetachStaticDeviceEventHandlers();

				// Dispose of managed resources if requested:
				if (disposing)
				{
					// In the 'normal' case, the items have already been disposed of in Close().
					StopReceiveThread();
					CloseStream();
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
		/// Indicates whether the configured USB serial string must match the current device.
		/// </summary>
		public virtual bool MatchSerial
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.matchSerial);
			}
			set
			{
				AssertNotDisposed();

				this.matchSerial = value;
			}
		}

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

		/// <summary></summary>
		public virtual byte ActiveReportId
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.reportFormat.Id);
			}
			set
			{
				AssertNotDisposed();

				this.reportFormat.Id = value;
			}
		}

		/// <summary>
		/// Indicates how the ID is used while receiving.
		/// </summary>
		public virtual SerialHidRxFilterUsage RxFilterUsage
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.rxFilterUsage);
			}
			set
			{
				AssertNotDisposed();

				this.rxFilterUsage = value;
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
		/// Indicates whether the device hides non-payload data.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the device hides non-payload data; otherwise, <c>false</c>.
		/// </returns>
		public virtual bool IncludeNonPayloadData
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.includeNonPayloadData);
			}
			set
			{
				AssertNotDisposed();

				this.includeNonPayloadData = value;
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
				// Attention:
				// Do not call AssertNotDisposed() since IsOpen is used by AsyncReadCompleted()
				// to detect devices that are just being closed or have already been closed.

				switch (this.state)
				{
					case State.ConnectedAndClosed:
					case State.ConnectedAndOpened:
					case State.DisconnectedAndWaitingForReopen:
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
				// Attention:
				// Do not call AssertNotDisposed() since IsOpen is used by AsyncReadCompleted()
				// to detect devices that are just being closed or have already been closed.

				// Attention:
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

			DebugMessage("Starting...");
			SetStateSynchronized(State.Starting);
			return (Open()); // Resulting state will be set in Open().
		}

		/// <summary>
		/// Stops the device.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "'Stop' is a common term to start/stop something.")]
		public virtual void Stop()
		{
			AssertNotDisposed();

			DebugMessage("Stopping...");
			SetStateSynchronized(State.Stopping);
			Close(); // Resulting state will be set in Close().
		}

		/// <summary>
		/// Opens the serial communication to the device.
		/// </summary>
		public virtual bool Open()
		{
			AssertNotDisposed();

			// The stream may already exist and be ready.
			if (IsOpen)
			{
				SetStateSynchronized(State.ConnectedAndOpened);
				return (true);
			}

			DebugMessage("Opening...");
			CreateAndStartReceiveThread();

			// Create a new stream and begin to read data from the device.
			if (CreateStream())
			{
				SetStateSynchronized(State.ConnectedAndOpened);
				OnOpened(EventArgs.Empty);
				return (true);
			}
			else if (AutoOpen)
			{
				SetStateSynchronized(State.DisconnectedAndWaitingForReopen);
				return (true);
			}
			else
			{
				SetStateSynchronized(State.DisconnectedAndClosed);
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

			DebugMessage("Closing...");
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
		/// An array of type <see cref="Byte"/> that is the storage location for the received data.
		/// </param>
		/// <returns>The number of bytes received.</returns>
		public virtual int Receive(out byte[] data)
		{
			AssertNotDisposed();

			// OnDataReceived has been called before.

			int bytesReceived = 0;
			if (IsOpen)
			{
				lock (this.receiveQueue) // Lock is required because Queue<T> is not synchronized.
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
		/// An item of type <see cref="Byte"/> that contains the data to be sent.
		/// </param>
		public virtual void Send(byte data)
		{
			Send(new byte[] { data });
		}

		/// <summary>
		/// Sends data to the device.
		/// </summary>
		/// <param name="data">
		/// An array of type <see cref="Byte"/> that contains the data to be sent.
		/// </param>
		public virtual void Send(byte[] data)
		{
			AssertNotDisposed();

			// OnDataSent is called by 'Write()' below.

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

		/// <remarks>
		/// Especially useful during potentially dangerous creation and disposal sequence.
		/// </remarks>
		private void SignalReceiveThreadSafely()
		{
			try
			{
				if (this.receiveThreadEvent != null)
					this.receiveThreadEvent.Set();
			}
			catch (ObjectDisposedException ex) { DebugEx.WriteException(GetType(), ex, "Unsafe thread signaling caught"); }
			catch (NullReferenceException ex)  { DebugEx.WriteException(GetType(), ex, "Unsafe thread signaling caught"); }

			// Catch 'NullReferenceException' for the unlikely case that the event has just been
			// disposed after the if-check. This way, the event doesn't need to be locked (which
			// is a relatively time-consuming operation). Still keep the if-check for the normal
			// cases.
		}

		/// <remarks>
		/// Using 'Stop' instead of 'Terminate' to emphasize graceful termination, i.e. trying
		/// to join first, then abort if not successfully joined.
		/// </remarks>
		private void StopReceiveThread()
		{
			lock (this.receiveThreadSyncObj)
			{
				if (this.receiveThread != null)
				{
					DebugThreadState("ReceiveThread() gets stopped...");

					this.receiveThreadRunFlag = false;

					// Ensure that receive thread has stopped after the stop request:
					try
					{
						Debug.Assert(this.receiveThread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId, "Attention: Tried to join itself!");

						bool isAborting = false;
						int accumulatedTimeout = 0;
						int interval = 0; // Use a relatively short random interval to trigger the thread:
						while (!this.receiveThread.Join(interval = staticRandom.Next(5, 20)))
						{
							SignalReceiveThreadSafely();

							accumulatedTimeout += interval;
							if (accumulatedTimeout >= ThreadWaitTimeout)
							{
								DebugThreadState("...failed! Aborting...");
								DebugThreadState("(Abort is likely required due to failed synchronization back the calling thread, which is typically the GUI/main thread.)");

								isAborting = true;          // Thread.Abort() must not be used whenever possible!
								this.receiveThread.Abort(); // This is only the fall-back in case joining fails for too long.
								break;
							}

							DebugThreadState("...trying to join at " + accumulatedTimeout + " ms...");
						}

						if (!isAborting)
							DebugThreadState("...successfully stopped.");
					}
					catch (ThreadStateException)
					{
						// Ignore thread state exceptions such as "Thread has not been started" and
						// "Thread cannot be aborted" as it just needs to be ensured that the thread
						// has or will be terminated for sure.

						DebugThreadState("...failed too but will be exectued as soon as the calling thread gets suspended again.");
					}

					this.receiveThread = null;
				}

				if (this.receiveThreadEvent != null)
				{
					try     { this.receiveThreadEvent.Close(); }
					finally { this.receiveThreadEvent = null; }
				}
			} // lock (receiveThreadSyncObj)
		}

		#endregion

		#region Methods > Stream
		//------------------------------------------------------------------------------------------
		// Methods > Stream
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// Currently, this implementation is fixed to true Ser/HID devices. Attempted to create a
		/// <see cref="Win32.Hid.CreateSharedReadHandle"/> to check whether accessing a true HID
		/// device (e.g. a mouse) would also work on 2018-02-23. It doesn't, both calls (read/write
		/// as well as read-only) result in "API call returned code '5' meaning 'access denied'".
		/// Looks like the HID driver enforces exclusive device access.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Well, 'Ser/HID'... And adding 'Ser/HID' to 'CustomDictionary.xml' doesn't help either...")]
		private bool CreateStream()
		{
			SafeFileHandle readWriteHandle;
			if (!Win32.Hid.CreateSharedReadWriteHandle(Path, out readWriteHandle))
				return (false);

			if (!Win32.Hid.FlushQueue(readWriteHandle))
				return (false);

			this.stream = new FileStream(readWriteHandle, FileAccess.Read | FileAccess.Write, InputReportByteLength, true);

			// Immediately start reading:
			BeginAsyncRead();

			return (true);
		}

		private void BeginAsyncRead()
		{
			byte[] inputReportBuffer = new byte[InputReportByteLength];
			this.stream.BeginRead(inputReportBuffer, 0, InputReportByteLength, new AsyncCallback(AsyncReadCompleted), inputReportBuffer);
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		private void AsyncReadCompleted(IAsyncResult ar)
		{
			try
			{
				if (!IsDisposed && IsOpen) // Ensure not to perform any operations during closing anymore. Check 'IsDisposed' first!
				{
					// Immediately read data on this thread:

					// Finalize read and retrieve the data. In case of an exception during the read,
					// the call of EndRead() throws it. If this happens, e.g. due to disconnect,
					// exception is caught further down and stream is closed:
					int actualLength = this.stream.EndRead(ar);
					byte[] inputReportBuffer = new byte[actualLength];
					Buffer.BlockCopy((byte[])ar.AsyncState, 0, inputReportBuffer, 0, actualLength);

					// Convert the input report into usable data:
					SerialHidInputReportContainer input = new SerialHidInputReportContainer(this);
					input.ProcessReport(this.reportFormat, inputReportBuffer);

					// Evaluate whether report can be accepted, based on ID and configuration:
					bool acceptReport = true;
					if (this.reportFormat.UseId)
					{
						if (!this.rxFilterUsage.SeparateRxId) // Common case = Same ID for Tx and Rx.
						{
							acceptReport = (input.Id == this.reportFormat.Id);
						}
						else // Special case = Separate ID for Rx.
						{
							if (this.rxFilterUsage.AnyRxId)
								acceptReport = true;
							else
								acceptReport = (input.Id == this.rxFilterUsage.RxId);
						}
					}

					if (acceptReport)
					{
						// Read data on this thread:
						lock (this.receiveQueue) // Lock is required because Queue<T> is not synchronized.
						{
							if (this.includeNonPayloadData)
							{
								foreach (byte b in inputReportBuffer) // Include the whole report.
									this.receiveQueue.Enqueue(b);
							}
							else
							{
								foreach (byte b in input.Payload) // Only enqueue the retrieved payload.
									this.receiveQueue.Enqueue(b);
							}
						}

						// Signal thread:
						SignalReceiveThreadSafely();
					}

					// Continue receiving:
					BeginAsyncRead();
				} // if (active)
			}
			catch (IOException ex) // Includes Close().
			{
				string message = "Disconnect detected while reading from USB Ser/HID device.";
				DebugEx.WriteException(GetType(), ex, message);
				OnDisconnected(EventArgs.Empty);
			}
			catch (Exception ex)
			{
				var sb = new StringBuilder();
				sb.Append    (@"Error while reading an input report from USB Ser/HID device """);
				sb.Append    (ToString());
				sb.AppendLine(@""":");
				sb.AppendLine();

				if (ex.Message.EndsWith(Environment.NewLine, StringComparison.OrdinalIgnoreCase))
					sb.Append    (ex.Message);
				else
					sb.AppendLine(ex.Message);

				sb.AppendLine();
				sb.AppendLine("This may be caused by mismatching report format settings.");
				sb.AppendLine();
				sb.Append    ("Change the settings, then close and reopen the terminal and try again.");

				string message = sb.ToString();
				DebugEx.WriteException(GetType(), ex, message);
				OnIOError(new ErrorEventArgs(message));
			}
		}

		/// <summary>
		/// Asynchronously manage incoming events to prevent potential deadlocks if close/dispose
		/// was called from a ISynchronizeInvoke target (i.e. a form) on an event thread.
		/// Also, the mechanism implemented below reduces the amount of events that are propagated
		/// to the main application. Small chunks of received data will generate many events
		/// handled by <see cref="AsyncReadCompleted"/>. However, since <see cref="OnDataReceived"/>
		/// synchronously invokes the event, it will take some time until the send queue is checked
		/// again. During this time, no more new events are invoked, instead, incoming data is
		/// buffered.
		/// </summary>
		/// <remarks>
		/// Will be signaled by <see cref="AsyncReadCompleted"/> event above, or by XOn/XOff while
		/// sending.
		/// </remarks>
		[SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", MessageId = "System.Threading.WaitHandle.#WaitOne(System.Int32)", Justification = "Installer indeed targets .NET 3.5 SP1.")]
		private void ReceiveThread()
		{
			DebugThreadState("ReceiveThread() has started.");

			try
			{
				// Outer loop, processes data after a signal was received:
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

					// Inner loop, runs as long as there is data in the receive queue.
					// Ensure not to forward events during disposing anymore. Check 'IsDisposed' first!
					while (!IsDisposed && this.receiveThreadRunFlag && (BytesAvailable > 0))
					{
						// Initially, yield to other threads before starting to read the queue, since it is very
						// likely that more data is to be enqueued, thus resulting in larger chunks processed.
						// Subsequently, yield to other threads to allow processing the data.
						Thread.Sleep(TimeSpan.Zero);

						OnDataReceived(EventArgs.Empty);

						// Note the Thread.Sleep(TimeSpan.Zero) above.
					} // Inner loop
				} // Outer loop
			}
			catch (ThreadAbortException ex)
			{
				DebugEx.WriteException(GetType(), ex, "SendThread() has been aborted! Confirming the abort, i.e. Thread.ResetAbort() will be called...");

				// Should only happen when failing to 'friendly' join the thread on stopping!
				// Don't try to set and notify a state change, or even restart the device!

				// But reset the abort request, as 'ThreadAbortException' is a special exception
				// that would be rethrown at the end of the catch block otherwise!

				Thread.ResetAbort();
			}

			DebugThreadState("ReceiveThread() has terminated.");
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		private void Write(byte[] payload)
		{
			try
			{
				var output = new SerialHidOutputReportContainer(this);
				output.CreateReports(this.reportFormat, payload);

				foreach (byte[] report in output.Reports)
				{
					this.stream.Write(report, 0, report.Length);

					if (this.includeNonPayloadData)
						OnDataSent(new DataEventArgs(report)); // Include the whole report.
					else
						OnDataSent(new DataEventArgs(payload)); // Only include the payload.
				}
			}
			catch (Exception ex)
			{
				var sb = new StringBuilder();
				sb.Append    (@"Error while writing an output report to USB Ser/HID device """);
				sb.Append    (ToString());
				sb.AppendLine(@""":");
				sb.AppendLine();

				if (ex.Message.EndsWith(Environment.NewLine, StringComparison.OrdinalIgnoreCase))
					sb.Append    (ex.Message);
				else
					sb.AppendLine(ex.Message);

				sb.AppendLine();

				if (this.reportFormat.UseId)
				{
					sb.Append("Error may be caused by a mismatching report ID, i.e. the device may not support ID " + this.reportFormat.Id.ToString(CultureInfo.InvariantCulture) + ". ");
					sb.Append("Change the settings to a matching report ID or use the \\!(ReportID(<id>)) keyword to set a matching ID, then close and reopen the terminal and try again.");
				}
				else
				{
					sb.Append("Error may be caused by mismatching report format settings. ");
					sb.Append("Change the settings, then close and reopen the terminal and try again.");
				}

				string message = sb.ToString();
				DebugEx.WriteException(GetType(), ex, message);
				OnIOError(new ErrorEventArgs(message));
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		private void CloseStream()
		{
			if (this.stream != null)
			{
				try
				{
					// Attention:
					// Set this.stream to null before initiating Close() to ensure that the IsOpen
					// property returns false during closing. AsyncReadCompleted() will be called
					// when Close() is initiated. AsyncReadCompleted() will check IsOpen.

					FileStream fs = this.stream;
					this.stream = null;
					fs.Close();
				}
				catch (Exception ex)
				{
					DebugEx.WriteException(GetType(), ex, "Exception while closing stream!");
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

			lock (this.stateSyncObj)
				state = this.state;

			return (state);
		}

		private void SetStateSynchronized(State state)
		{
#if (DEBUG)
			State oldState = this.state;
#endif
			lock (this.stateSyncObj)
				this.state = state;
#if (DEBUG)
			if (this.state != oldState)
				DebugMessage("State has changed from " + oldState + " to " + this.state + ".");
			else
				DebugMessage("State is already " + oldState + ".");
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
		/// Attention:
		/// This function similarly exists in the other USB classes.
		/// Changes here may have to be applied there too.
		/// </remarks>
		private void Device_DeviceConnected(object sender, DeviceEventArgs e)
		{
			bool isMatch = false;

			if (this.matchSerial)
				isMatch = Info.Equals(e.DeviceInfo);
			else
				isMatch = Info.EqualsVidPid(e.DeviceInfo);

			if (isMatch)
			{
				// Force reinitialize with new device info:
				Reinitialize(e.DeviceInfo);

				OnConnected(EventArgs.Empty);

				if (AutoOpen)
					Open();
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
				OnDisconnected(EventArgs.Empty); // Includes Close().
		}

		#endregion

		#region Event Raising
		//==========================================================================================
		// Event Raising
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
			this.eventHelper.RaiseSync(Opened, this, e);
		}

		/// <summary></summary>
		protected virtual void OnClosed(EventArgs e)
		{
			this.eventHelper.RaiseSync(Closed, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDataReceived(EventArgs e)
		{
			if (IsOpen) // Make sure to propagate event only if active.
				this.eventHelper.RaiseSync(DataReceived, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDataSent(DataEventArgs e)
		{
			if (IsOpen) // Make sure to propagate event only if active.
				this.eventHelper.RaiseSync(DataSent, this, e);
		}

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		[Conditional("DEBUG")]
		private void DebugMessage(string message)
		{
			Debug.WriteLine
			(
				string.Format
				(
					CultureInfo.CurrentCulture,
					" @ {0} @ Thread #{1} : {2,36} {3,3} {4,-38} : {5}",
					DateTime.Now.ToString("HH:mm:ss.fff", DateTimeFormatInfo.CurrentInfo),
					Thread.CurrentThread.ManagedThreadId.ToString("D3", CultureInfo.CurrentCulture),
					GetType(),
					"",
					"[" + ToString() + "]",
					message
				)
			);
		}

		[Conditional("DEBUG_THREAD_STATE")]
		private void DebugThreadState(string message)
		{
			DebugMessage(message);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
