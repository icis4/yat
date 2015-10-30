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
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
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
	/// <summary></summary>
	public class SerialHidDevice : IIOProvider, IDisposable
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const int SendQueueInitialCapacity = 4096;
		private const int ReceiveQueueInitialCapacity = 4096;

		private const int ThreadWaitTimeout = 200;

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

		private bool isDisposed;

		private SerialHidDeviceSettings settings;

		private IO.Usb.SerialHidDevice device;
		private object deviceSyncObj = new object();

		/// <remarks>
		/// Async sending. The capacity is set large enough to reduce the number of resizing
		/// operations while adding elements.
		/// </remarks>
		private Queue<byte> sendQueue = new Queue<byte>(SendQueueInitialCapacity);

		private bool sendThreadRunFlag;
		private AutoResetEvent sendThreadEvent;
		private Thread sendThread;
		private object sendThreadSyncObj = new object();

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
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary></summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!this.isDisposed)
			{
				// Dispose of managed resources if requested:
				if (disposing)
				{
					// In the 'normal' case, the items have already been disposed of, e.g. in Stop().
					DisposeDeviceAndThreads();
				}

				// Set state to disposed:
				this.sendThreadEvent = null;
				this.receiveThreadEvent = null;
				this.isDisposed = true;
			}
		}

		/// <summary></summary>
		~SerialHidDevice()
		{
			Dispose(false);

			WriteDebugMessageLine("The finalizer should have never been called! Ensure to call Dispose()!");
		}

		/// <summary></summary>
		public bool IsDisposed
		{
			get { return (this.isDisposed); }
		}

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (this.isDisposed)
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
				IO.Usb.DeviceInfo di = DeviceInfo;
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
			// AssertNotDisposed() is called by 'IsStarted' below.

			if (IsStopped)
				return (TryCreateAndStartDevice());

			return (true);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "Stop is a common term to start/stop something.")]
		public virtual void Stop()
		{
			// AssertNotDisposed() is called by 'IsStarted' below.

			if (IsStarted)
				DisposeDeviceAndThreads();
		}

		/// <summary></summary>
		public virtual bool Send(byte[] data)
		{
			// AssertNotDisposed() is called by 'IsStarted' below.

			if (IsStarted)
			{
				lock (this.sendQueue)
				{
					foreach (byte b in data)
						this.sendQueue.Enqueue(b);
				}

				// Signal send thread:
				this.sendThreadEvent.Set();

				return (true);
			}
			else
			{
				return (false);
			}
		}

		/// <summary>
		/// Asynchronously manage outgoing send requests to ensure that send events are not
		/// invoked on the same thread that triggered the send operation.
		/// Also, the mechanism implemented below reduces the amount of events that are propagated
		/// to the main application. Small chunks of sent data would generate many events in
		/// <see cref="Send(byte[])"/>. However, since <see cref="OnDataSent"/> synchronously
		/// invokes the event, it will take some time until the send queue is checked again.
		/// During this time, no more new events are invoked, instead, outgoing data is buffered.
		/// </summary>
		/// <remarks>
		/// Will be signaled by <see cref="Send(byte[])"/> method above.
		/// </remarks>
		[SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", MessageId = "System.Threading.WaitHandle.#WaitOne(System.Int32)", Justification = "Installer indeed targets .NET 3.5 SP1.")]
		private void SendThread()
		{
			WriteDebugThreadStateMessageLine("SendThread() has started.");

			// Outer loop, requires another signal.
			while (!IsDisposed && this.sendThreadRunFlag) // Check 'IsDisposed' first!
			{
				try
				{
					// WaitOne() will wait forever if the underlying I/O provider has crashed, or
					// if the overlying client isn't able or forgets to call Stop() or Dispose().
					// Therefore, only wait for a certain period and then poll the run flag again.
					// The period can be quite long, as an event trigger will immediately resume.
					if (!this.sendThreadEvent.WaitOne(staticRandom.Next(50, 200)))
						continue;
				}
				catch (AbandonedMutexException ex)
				{
					// The mutex should never be abandoned, but in case it nevertheless happens,
					// at least output a debug message and gracefully exit the thread.
					DebugEx.WriteException(GetType(), ex, "An 'AbandonedMutexException' occurred in SendThread()!");
					break;
				}

				// Inner loop, runs as long as there is data in the send queue.
				// Ensure not to forward any events during closing anymore.
				while (!IsDisposed && this.sendThreadRunFlag && IsTransmissive) // Check 'IsDisposed' first!
				{
					byte[] data;
					lock (this.sendQueue)
					{
						if (this.sendQueue.Count <= 0)
							break; // Let other threads do their job and wait until signaled again.

						data = this.sendQueue.ToArray();
						this.sendQueue.Clear();
					}

					this.device.Send(data);
					OnDataSent(new DataSentEventArgs(data));

					// Wait for the minimal time possible to allow other threads to execute and
					// to prevent that 'DataSent' events are fired consecutively.
					Thread.Sleep(TimeSpan.Zero);
				}
			}

			WriteDebugThreadStateMessageLine("SendThread() has terminated.");
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
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		private bool TryCreateAndStartDevice()
		{
			try
			{
				if (CreateDevice())
					return (StartDevice());
				else
					return (false);
			}
			catch
			{
				DisposeDeviceAndThreads();
				return (false);
			}
		}

		/// <remarks>
		/// The <see cref="Usb.SerialHidDevice.DataSent"/> event is not used. Instead, the
		/// corresponding event is fired in the <see cref="Send(byte[])"/> method.
		/// </remarks>
		private bool CreateDevice()
		{
			if (this.device != null)
				DisposeDeviceAndThreads();

			IO.Usb.DeviceInfo di = this.settings.DeviceInfo;
			if (di != null)
			{
				StartThreads();

				lock (this.deviceSyncObj)
				{
					// Ensure to create device info from VID/PID/SNR since system path is not saved.
					this.device = new IO.Usb.SerialHidDevice(di.VendorId, di.ProductId, di.Serial);
					this.device.ReportFormat = this.settings.ReportFormat;
					this.device.RxIdUsage    = this.settings.RxIdUsage;
					this.device.AutoOpen     = this.settings.AutoOpen;

					this.device.Connected    += new EventHandler(device_Connected);
					this.device.Disconnected += new EventHandler(device_Disconnected);
					this.device.Opened       += new EventHandler(device_Opened);
					this.device.Closed       += new EventHandler(device_Closed);
					this.device.DataReceived += new EventHandler(device_DataReceived);
				////this.device.DataSent is not used, see remarks above.
					this.device.IOError        += new EventHandler<IO.Usb.ErrorEventArgs>(device_IOError);
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
				return (success);
			}
			else
			{
				return (false);
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		private void DisposeDeviceAndThreads()
		{
			try
			{
				lock (this.deviceSyncObj)
				{
					if (this.device != null)
					{
						this.device.Dispose();
						this.device = null;
					}
				}
			}
			catch { }

			try
			{
				StopThreads();
			}
			catch { }

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
					WriteDebugThreadStateMessageLine("SendThread() gets stopped...");

					// Ensure that send thread has stopped after the stop request:
					try
					{
						int accumulatedTimeout = 0;
						int interval = 0; // Use a relatively short random interval to trigger the thread:
						while (!this.sendThread.Join(interval = staticRandom.Next(5, 20)))
						{
							this.sendThreadEvent.Set();

							accumulatedTimeout += interval;
							if (accumulatedTimeout >= ThreadWaitTimeout)
							{
								WriteDebugThreadStateMessageLine("...failed! Aborting...");
								WriteDebugThreadStateMessageLine("(Abort is likely required due to failed synchronization back the calling thread, which is typically the GUI/main thread.)");
								this.sendThread.Abort();
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

					this.sendThreadEvent.Close();
					this.sendThread = null;

					WriteDebugThreadStateMessageLine("...successfully terminated.");
				}
			}

			lock (this.receiveThreadSyncObj)
			{
				if (this.receiveThread != null)
				{
					WriteDebugThreadStateMessageLine("ReceiveThread() gets stopped...");

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

		/// <remarks>
		/// The <see cref="Usb.SerialHidDevice.DataSent"/> event is not used. Instead, the
		/// corresponding event is triggered in the <see cref="Send(byte[])"/> method.
		/// </remarks>
		private void device_DataReceived(object sender, EventArgs e)
		{
			byte[] data;
			this.device.Receive(out data);

			lock (this.receiveQueue)
			{
				foreach (byte b in data)
					this.receiveQueue.Enqueue(b);
			}

			// Signal receive thread:
			this.receiveThreadEvent.Set();
		}

		/// <summary>
		/// Asynchronously manage incoming events to prevent potential deadlocks if close/dispose
		/// was called from a ISynchronizeInvoke target (i.e. a form) on an event thread.
		/// Also, the mechanism implemented below reduces the amount of events that are propagated
		/// to the main application. Small chunks of received data will generate many events
		/// handled by <see cref="device_DataReceived"/>. However, since <see cref="OnDataReceived"/>
		/// synchronously invokes the event, it will take some time until the send queue is checked
		/// again. During this time, no more new events are invoked, instead, outgoing data is
		/// buffered.
		/// </summary>
		/// <remarks>
		/// Will be signaled by <see cref="device_DataReceived"/> event above.
		/// </remarks>
		[SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", MessageId = "System.Threading.WaitHandle.#WaitOne(System.Int32)", Justification = "Installer indeed targets .NET 3.5 SP1.")]
		private void ReceiveThread()
		{
			WriteDebugThreadStateMessageLine("ReceiveThread() has started.");

			// Outer loop, requires another signal.
			while (!IsDisposed && this.receiveThreadRunFlag) // Check 'IsDisposed' first!
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
				while (!IsDisposed && this.receiveThreadRunFlag && IsOpen) // Check 'IsDisposed' first!
				{
					byte[] data;
					lock (this.receiveQueue)
					{
						if (this.receiveQueue.Count <= 0)
							break; // Let other threads do their job and wait until signaled again.

						data = this.receiveQueue.ToArray();
						this.receiveQueue.Clear();
					}

					OnDataReceived(new DataReceivedEventArgs(data));

					// Wait for the minimal time possible to allow other threads to execute and
					// to prevent that 'DataReceived' events are fired consecutively.
					Thread.Sleep(TimeSpan.Zero);
				}
			}

			WriteDebugThreadStateMessageLine("ReceiveThread() has terminated.");
		}

		private void device_IOError(object sender, IO.Usb.ErrorEventArgs e)
		{
			OnIOError(new IOErrorEventArgs(ErrorSeverity.Severe, e.Message));
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnIOChanged(EventArgs e)
		{
			EventHelper.FireSync(IOChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOControlChanged(EventArgs e)
		{
			UnusedEvent.PreventCompilerWarning(IOControlChanged);
			throw (new NotImplementedException("Program execution should never get here, the event 'IOControlChanged' is not in use for USB Ser/HID devices, please report this bug!"));
		}

		/// <summary></summary>
		protected virtual void OnIOError(IOErrorEventArgs e)
		{
			EventHelper.FireSync<IOErrorEventArgs>(IOError, this, e);
		}

		/// <summary></summary>
		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true)]
		protected virtual void OnDataReceived(DataReceivedEventArgs e)
		{
			EventHelper.FireSync<DataReceivedEventArgs>(DataReceived, this, e);
		}

		/// <summary></summary>
		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true)]
		protected virtual void OnDataSent(DataSentEventArgs e)
		{
			EventHelper.FireSync<DataSentEventArgs>(DataSent, this, e);
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary></summary>
		public override string ToString()
		{
			return (ToDeviceInfoString());
		}

		#region Object Members > Extensions
		//------------------------------------------------------------------------------------------
		// Object Members > Extensions
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual string ToShortString()
		{
			return (ToShortDeviceInfoString());
		}

		/// <summary></summary>
		public virtual string ToDeviceInfoString()
		{
			IO.Usb.DeviceInfo di = DeviceInfo;
			if (di != null)
				return (di.ToString());
			else
				return (base.ToString());
		}

		/// <summary></summary>
		public virtual string ToShortDeviceInfoString()
		{
			IO.Usb.DeviceInfo di = DeviceInfo;
			if (di != null)
				return (di.ToShortString());
			else
				return (base.ToString());
		}

		#endregion

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
					"[" + ToDeviceInfoString() + "]",
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
