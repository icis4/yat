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
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
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
using System.Threading;

using MKY.Contracts;
using MKY.Diagnostics;

#endregion

namespace MKY.IO.Serial.Usb
{
	/// <summary>
	/// Implements the <see cref="IIOProvider"/> interface for serial COM ports.
	/// </summary>
	/// <remarks>
	/// In addition, this class...
	/// <list type="bullet">
	/// <item><description>...wraps <see cref="IO.Usb.SerialHidDevice"/> with send/receive FIFOs.</description></item>
	/// <item><description>...adds software flow control (XOn/XOff).</description></item>
	/// </list>
	/// </remarks>
	/// <remarks>
	/// This class is implemented using partial classes separating sending/receiving functionality.
	/// </remarks>
	public partial class SerialHidDevice : IIOProvider, IXOnXOffHandler, IDisposable, IDisposableEx
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const int SendQueueFixedCapacity      = 4096;
		private const int ReceiveQueueInitialCapacity = 4096;

		private const int ThreadWaitTimeout = 500; // Enough time to let the threads join...

		private const string Undefined = "<Undefined>";

		#endregion

		#region Static Fields
		//==========================================================================================
		// Static Fields
		//==========================================================================================

		private static Random staticRandom = new Random(RandomEx.NextPseudoRandomSeed());

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		/// <summary>
		/// A dedicated event helper to allow discarding exceptions when object got disposed.
		/// </summary>
		private EventHelper.Item eventHelper = EventHelper.CreateItem(typeof(SerialHidDevice).FullName);

		private SerialHidDeviceSettings settings;

		private IO.Usb.SerialHidDevice device;
		private object deviceSyncObj = new object();
		private object dataEventSyncObj = new object();

		/// <remarks>
		/// Async sending. The capacity is set large enough to reduce the number of resizing
		/// operations while adding items.
		/// </remarks>
		private Queue<byte> sendQueue = new Queue<byte>(SendQueueFixedCapacity);

		private bool sendThreadRunFlag;
		private AutoResetEvent sendThreadEvent;
		private Thread sendThread;
		private object sendThreadSyncObj = new object();

		/// <remarks>
		/// Async receiving. The capacity is set large enough to reduce the number of resizing
		/// operations while adding items.
		/// </remarks>
		private Queue<byte> receiveQueue = new Queue<byte>(ReceiveQueueInitialCapacity);

		private bool receiveThreadRunFlag;
		private AutoResetEvent receiveThreadEvent;
		private Thread receiveThread;
		private object receiveThreadSyncObj = new object();

		/// <remarks>
		/// Only used with <see cref="SerialHidFlowControl.Software"/>
		/// and <see cref="SerialHidFlowControl.ManualSoftware"/>.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Emphasize the existance of the interface in use.")]
		private IXOnXOffHelper iXOnXOffHelper = new IXOnXOffHelper();

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		public event EventHandler IOChanged;

		/// <summary></summary>
		public event EventHandler IOControlChanged;

		/// <summary></summary>
		public event EventHandler<IOErrorEventArgs> IOError;

		/// <summary></summary>
		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true)]
		public event EventHandler<DataReceivedEventArgs> DataReceived;

		/// <summary></summary>
		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true)]
		public event EventHandler<DataSentEventArgs> DataSent;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public SerialHidDevice(SerialHidDeviceSettings settings)
		{
			this.settings = settings;
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public bool IsDisposed { get; protected set; }

		/// <summary></summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary></summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				DebugEventManagement.DebugWriteAllEventRemains(this);
				this.eventHelper.DiscardAllEventsAndExceptions();

				// Dispose of managed resources if requested:
				if (disposing)
				{
					// In the 'normal' case, the items have already been disposed of, e.g. in Stop().
					DisposeDeviceAndThreads();
				}

				// Set state to disposed:
				this.sendThreadEvent = null;
				this.receiveThreadEvent = null;
				IsDisposed = true;
			}
		}

	#if (DEBUG)
		/// <remarks>
		/// Microsoft.Design rule CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable requests
		/// "Types that declare disposable members should also implement IDisposable. If the type
		///  does not own any unmanaged resources, do not implement a finalizer on it."
		///
		/// Well, true for best performance on finalizing. However, it's not easy to find missing
		/// calls to <see cref="Dispose()"/>. In order to detect such missing calls, the finalizer
		/// is kept for DEBUG, indicating missing calls.
		///
		/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
		/// </remarks>
		~SerialHidDevice()
		{
			Dispose(false);

			DebugDisposal.DebugNotifyFinalizerInsteadOfDispose(this);
		}
	#endif // DEBUG

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (IsDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed!"));
		}

		#endregion

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public virtual SerialHidDeviceSettings Settings
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.settings);
			}
		}

		/// <summary></summary>
		public virtual IO.Usb.DeviceInfo DeviceInfo
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.device != null)
					return (this.device.Info);
				else if (this.settings != null)
					return (this.settings.DeviceInfo);
				else
					return (null);
			}
		}

		/// <summary></summary>
		public virtual string DeviceInfoString
		{
			get
			{
				var di = DeviceInfo;
				if (di != null)
					return (di.ToString());
				else
					return (Undefined);
			}
		}

		/// <summary></summary>
		public virtual bool IsStopped
		{
			get
			{
				return (!IsStarted);
			}
		}

		/// <summary></summary>
		public virtual bool IsStarted
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.device != null);
			}
		}

		/// <summary></summary>
		public virtual bool IsConnected
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.device != null)
					return (this.device.IsConnected);

				return (false);
			}
		}

		/// <summary></summary>
		public virtual bool IsOpen
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.device != null)
					return (this.device.IsOpen);

				return (false);
			}
		}

		/// <summary></summary>
		public virtual bool IsTransmissive
		{
			get { return (IsOpen); }
		}

		/// <summary>
		/// Returns <c>true</c> if XOn/XOff is in use, i.e. if one or the other kind of XOn/XOff
		/// flow control is active.
		/// </summary>
		public virtual bool XOnXOffIsInUse
		{
			get
			{
				AssertNotDisposed();

				return (this.settings.FlowControlUsesXOnXOff);
			}
		}

		/// <summary>
		/// Gets the input XOn/XOff state.
		/// </summary>
		public virtual bool InputIsXOn
		{
			get
			{
				AssertNotDisposed();

				if (this.settings.FlowControlUsesXOnXOff)
					return (this.iXOnXOffHelper.InputIsXOn);
				else
					return (true);
			}
		}

		/// <summary>
		/// Gets the output XOn/XOff state.
		/// </summary>
		public virtual bool OutputIsXOn
		{
			get
			{
				AssertNotDisposed();

				if (this.settings.FlowControlUsesXOnXOff)
					return (this.iXOnXOffHelper.OutputIsXOn);
				else
					return (true);
			}
		}

		/// <summary>
		/// Returns the number of sent XOn bytes, i.e. the count of input XOn/XOff signaling.
		/// </summary>
		public virtual int SentXOnCount
		{
			get
			{
				AssertNotDisposed();

				if (this.settings.FlowControlUsesXOnXOff)
					return (this.iXOnXOffHelper.SentXOnCount);
				else
					return (0);
			}
		}

		/// <summary>
		/// Returns the number of sent XOff bytes, i.e. the count of input XOn/XOff signaling.
		/// </summary>
		public virtual int SentXOffCount
		{
			get
			{
				AssertNotDisposed();

				if (this.settings.FlowControlUsesXOnXOff)
					return (this.iXOnXOffHelper.SentXOffCount);
				else
					return (0);
			}
		}

		/// <summary>
		/// Returns the number of received XOn bytes, i.e. the count of output XOn/XOff signaling.
		/// </summary>
		public virtual int ReceivedXOnCount
		{
			get
			{
				AssertNotDisposed();

				if (this.settings.FlowControlUsesXOnXOff)
					return (this.iXOnXOffHelper.ReceivedXOnCount);
				else
					return (0);
			}
		}

		/// <summary>
		/// Returns the number of received XOff bytes, i.e. the count of output XOn/XOff signaling.
		/// </summary>
		public virtual int ReceivedXOffCount
		{
			get
			{
				AssertNotDisposed();

				if (this.settings.FlowControlUsesXOnXOff)
					return (this.iXOnXOffHelper.ReceivedXOffCount);
				else
					return (0);
			}
		}

		/// <summary></summary>
		public virtual object UnderlyingIOInstance
		{
			get
			{
				AssertNotDisposed();

				return (this.device);
			}
		}

		#endregion

		#region Public Methods
		//==========================================================================================
		// Public Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual bool Start()
		{
			// AssertNotDisposed() is called by 'IsStopped' below.

			if (IsStopped)
				return (TryCreateAndStartDevice());

			return (true); // Return 'true' since device is started in any case.
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "'Stop' is a common term to start/stop something.")]
		public virtual void Stop()
		{
			// AssertNotDisposed() is called by 'IsStarted' below.

			if (IsStarted)
				DisposeDeviceAndThreads();
		}

		/// <summary></summary>
		public int CalculateTotalReportByteLength(byte[] payload)
		{
			return (this.device.CalculateTotalReportByteLength(payload));
		}

		#endregion

		#region Device Methods
		//==========================================================================================
		// Device Methods
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		private bool TryCreateAndStartDevice()
		{
			try
			{
				if (CreateDevice())
					return (StartDevice());
				else
					return (false);
			}
			catch (Exception ex)
			{
				string message = "Error while creating and starting the USB Ser/HID device" + Environment.NewLine + ToString();
				DebugEx.WriteException(GetType(), ex, message);

				DisposeDeviceAndThreads();
				return (false);
			}
		}

		private bool CreateDevice()
		{
			if (this.device != null)
				DisposeDeviceAndThreads();

			var di = this.settings.DeviceInfo;
			if (di != null)
			{
				StartThreads();

				lock (this.deviceSyncObj)
				{
					// Ensure to create device info from VID/PID/SNR since system path is not saved.
					this.device = new IO.Usb.SerialHidDevice(di.VendorId, di.ProductId, di.Serial);
					this.device.MatchSerial           = this.settings.MatchSerial;
				////                                    this.settings.Preset does not to be considered when creating a device.
					this.device.ReportFormat          = this.settings.ReportFormat;
					this.device.RxFilterUsage         = this.settings.RxFilterUsage;
					this.device.AutoOpen              = this.settings.AutoOpen;
					this.device.IncludeNonPayloadData = this.settings.IncludeNonPayloadData;

					this.device.Connected    += device_Connected;
					this.device.Disconnected += device_Disconnected;
					this.device.Opened       += device_Opened;
					this.device.Closed       += device_Closed;
					this.device.DataReceived += device_DataReceived;
					this.device.DataSent     += Device_DataSent;
					this.device.IOError      += device_IOError;
				}

				return (true);
			}
			else
			{
				return (false);
			}
		}

		private bool StartDevice()
		{
			if (this.device != null)
			{
				bool success = this.device.Start();

				OnIOChanged(EventArgs.Empty);

				if (success)
				{
					// Handle initial XOn/XOff state:
					if (this.settings.FlowControlUsesXOnXOff)
					{
						AssumeOutputXOn();

						// Immediately send XOn if software flow control is enabled to ensure that
						//   device gets put into XOn if it was XOff before.
						switch (this.settings.FlowControl)
						{
							case SerialHidFlowControl.ManualSoftware:
							{
								if (this.iXOnXOffHelper.ManualInputWasXOn)
									SignalInputXOn();

								break;
							}

							default:
							{
								SignalInputXOn();
								break;
							}
						}
					}
				}

				return (success);
			}
			else
			{
				return (false);
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		private void DisposeDeviceAndThreads()
		{
			lock (this.deviceSyncObj)
			{
				try
				{
					if (this.device != null)
						this.device.Dispose();
				}
				catch (Exception ex)
				{
					DebugEx.WriteException(GetType(), ex, "Exception while disposing device!");
				}
				finally
				{
					this.device = null;
				}
			}

			try
			{
				StopThreads();
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(GetType(), ex, "Exception while stopping threads!");
			}

			OnIOChanged(EventArgs.Empty);
		}

		#endregion

		#region Device Threads
		//==========================================================================================
		// Device Threads
		//==========================================================================================

		private void StartThreads()
		{
			lock (this.sendThreadSyncObj)
			{
				if (this.sendThread == null)
				{
					this.sendThreadRunFlag = true;
					this.sendThreadEvent = new AutoResetEvent(false);
					this.sendThread = new Thread(new ThreadStart(SendThread));
					this.sendThread.Name = ToDeviceInfoString() + " Send Thread";
					this.sendThread.Start();
				}
			}

			lock (this.receiveThreadSyncObj)
			{
				if (this.receiveThread == null)
				{
					this.receiveThreadRunFlag = true;
					this.receiveThreadEvent = new AutoResetEvent(false);
					this.receiveThread = new Thread(new ThreadStart(ReceiveThread));
					this.receiveThread.Name = ToDeviceInfoString() + " Receive Thread";
					this.receiveThread.Start();
				}
			}
		}

		/// <remarks>
		/// Especially useful during potentially dangerous creation and disposal sequence.
		/// </remarks>
		private void SignalSendThreadSafely()
		{
			try
			{
				if (this.sendThreadEvent != null)
					this.sendThreadEvent.Set();
			}
			catch (ObjectDisposedException ex) { DebugEx.WriteException(GetType(), ex, "Unsafe thread signaling caught"); }
			catch (NullReferenceException ex)  { DebugEx.WriteException(GetType(), ex, "Unsafe thread signaling caught"); }

			// Catch 'NullReferenceException' for the unlikely case that the event has just been
			// disposed after the if-check. This way, the event doesn't need to be locked (which
			// is a relatively time-consuming operation). Still keep the if-check for the normal
			// cases.
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
		private void StopThreads()
		{
			// First clear both flags to reduce the time to stop the receive thread, it may already
			// be signaled while receiving data while the send thread is still running.
			lock (this.sendThreadSyncObj)
				this.sendThreadRunFlag = false;
			lock (this.receiveThreadSyncObj)
				this.receiveThreadRunFlag = false;

			lock (this.sendThreadSyncObj)
			{
				if (this.sendThread != null)
				{
					DebugThreadState("SendThread() gets stopped...");

					// Ensure that send thread has stopped after the stop request:
					try
					{
						Debug.Assert(this.sendThread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId, "Attention: Tried to join itself!");

						bool isAborting = false;
						int accumulatedTimeout = 0;
						int interval = 0; // Use a relatively short random interval to trigger the thread:
						while (!this.sendThread.Join(interval = staticRandom.Next(5, 20)))
						{
							SignalSendThreadSafely();

							accumulatedTimeout += interval;
							if (accumulatedTimeout >= ThreadWaitTimeout)
							{
								DebugThreadState("...failed! Aborting...");
								DebugThreadState("(Abort is likely required due to failed synchronization back the calling thread, which is typically the main thread.)");

								isAborting = true;       // Thread.Abort() must not be used whenever possible!
								this.sendThread.Abort(); // This is only the fall-back in case joining fails for too long.
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

					this.sendThread = null;
				}

				if (this.sendThreadEvent != null)
				{
					try     { this.sendThreadEvent.Close(); }
					finally { this.sendThreadEvent = null; }
				}
			} // lock (sendThreadSyncObj)

			lock (this.receiveThreadSyncObj)
			{
				if (this.receiveThread != null)
				{
					DebugThreadState("ReceiveThread() gets stopped...");

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
								DebugThreadState("(Abort is likely required due to failed synchronization back the calling thread, which is typically the main thread.)");

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

		#region Device Events
		//==========================================================================================
		// Device Events
		//==========================================================================================

		private void device_Connected(object sender, EventArgs e)
		{
			OnIOChanged(e);
		}

		private void device_Disconnected(object sender, EventArgs e)
		{
			OnIOChanged(e);
		}

		private void device_Opened(object sender, EventArgs e)
		{
			OnIOChanged(e);
		}

		private void device_Closed(object sender, EventArgs e)
		{
			OnIOChanged(e);
		}

		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true, Rationale = "Usb.SerialHidDevice uses a 'ReceiveThread' to invoke this event.")]
		private void device_DataReceived(object sender, EventArgs e)
		{
			if (!IsDisposed && IsOpen) // Ensure not to perform any operations during closing anymore. Check 'IsDisposed' first!
			{
				byte[] data;
				this.device.Receive(out data);

				// Attention, XOn/XOff handling is implemented in MKY.IO.Serial.SerialPort.SerialPort too!
				// Changes here must most likely be applied there too.

				bool signalXOnXOff = false;
				bool signalXOnXOffCount = false;

				lock (this.receiveQueue) // Lock is required because Queue<T> is not synchronized.
				{
					foreach (byte b in data)
					{
						this.receiveQueue.Enqueue(b);

						// Handle XOn/XOff state:
						if (this.settings.FlowControlUsesXOnXOff)
						{
							if (b == XOnXOff.XOnByte)
							{
								if (this.iXOnXOffHelper.XOnReceived())
									signalXOnXOff = true;

								signalXOnXOffCount = true;
							}
							else if (b == XOnXOff.XOffByte)
							{
								if (this.iXOnXOffHelper.XOffReceived())
									signalXOnXOff = true;

								signalXOnXOffCount = true;
							}
						}
					} // foreach (byte b in data)
				} // lock (this.receiveQueue)

				// Signal XOn/XOff change to send thread:
				if (signalXOnXOff)
					SignalSendThreadSafely();

				// Signal data notification to receive thread:
				SignalReceiveThreadSafely();

				// Immediately invoke the event, but invoke it asynchronously and NOT on this thread!
				if (signalXOnXOff || signalXOnXOffCount)
					OnIOControlChangedAsync(EventArgs.Empty);
			} // if (!IsDisposed && ...)
		}

		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true, Rationale = "Usb.SerialHidDevice uses asynchronous 'Write' to invoke this event.")]
		private void Device_DataSent(object sender, IO.Usb.DataEventArgs e)
		{
			OnDataSent(new SerialDataSentEventArgs(e.Data, e.TimeStamp, DeviceInfo, this.device.ReportFormat.UseId, this.device.ActiveReportId));
		}

		private void device_IOError(object sender, IO.Usb.ErrorEventArgs e)
		{
			OnIOError(new IOErrorEventArgs(ErrorSeverity.Severe, e.Message, e.TimeStamp));
		}

		#endregion

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

		/// <summary></summary>
		[CallingContract(IsNeverMainThread = true)]
		protected virtual void OnIOChanged(EventArgs e)
		{
			if (!IsDisposed) // Make sure to propagate event only if not already disposed.
				this.eventHelper.RaiseSync(IOChanged, this, e);
		}

		/// <summary></summary>
		[CallingContract(IsNeverMainThread = true)]
		protected virtual void OnIOControlChanged(EventArgs e)
		{
			if (!IsDisposed) // Make sure to propagate event only if not already disposed.
				this.eventHelper.RaiseSync(IOControlChanged, this, e);
		}

		/// <summary></summary>
		[CallingContract(IsNeverMainThread = true)]
		protected virtual void OnIOControlChangedAsync(EventArgs e)
		{
			if (!IsDisposed) // Make sure to propagate event only if not already disposed.
				this.eventHelper.RaiseAsync(IOControlChanged, this, e);
		}

		/// <summary></summary>
		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true)]
		protected virtual void OnIOError(IOErrorEventArgs e)
		{
			if (!IsDisposed) // Make sure to propagate event only if not already disposed.
				this.eventHelper.RaiseSync<IOErrorEventArgs>(IOError, this, e);
		}

		/// <summary></summary>
		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true)]
		protected virtual void OnDataReceived(DataReceivedEventArgs e)
		{
			if (IsOpen) // Make sure to propagate event only if active.
				this.eventHelper.RaiseSync<DataReceivedEventArgs>(DataReceived, this, e);
		}

		/// <summary></summary>
		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true)]
		protected virtual void OnDataSent(DataSentEventArgs e)
		{
			if (IsOpen) // Make sure to propagate event only if active.
				this.eventHelper.RaiseSync<DataSentEventArgs>(DataSent, this, e);
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		public override string ToString()
		{
			// Do not call AssertNotDisposed() on such basic method! Its return value may be needed for debugging. All underlying fields are still valid after disposal.

			return (ToDeviceInfoString());
		}

		/// <summary></summary>
		public virtual string ToShortString()
		{
			// Do not call AssertNotDisposed() on such basic method! Its return value may be needed for debugging. All underlying fields are still valid after disposal.

			return (ToShortDeviceInfoString());
		}

		/// <summary></summary>
		public virtual string ToDeviceInfoString()
		{
			// Do not call AssertNotDisposed() on such basic method! Its return value is needed for debugging! All underlying fields are still valid after disposal.

			var di = DeviceInfo;
			if (di != null)
				return (di.ToString());
			else
				return (Undefined);
		}

		/// <summary></summary>
		public virtual string ToShortDeviceInfoString()
		{
			// Do not call AssertNotDisposed() on such basic method! Its return value may be needed for debugging. All underlying fields are still valid after disposal.

			var di = DeviceInfo;
			if (di != null)
				return (di.ToShortString());
			else
				return (Undefined);
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
					"[" + ToDeviceInfoString() + "]",
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
