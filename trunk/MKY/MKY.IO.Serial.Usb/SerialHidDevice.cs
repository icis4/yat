﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.8
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;

using MKY.Contracts;
using MKY.Event;

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

		private const int SendQueueInitialCapacity    = 4096;
		private const int ReceiveQueueInitialCapacity = 4096;

		private const string Undefined = "<Undefined>";

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isDisposed;

		private SerialHidDeviceSettings settings;

		private IO.Usb.SerialHidDevice device;
		private object deviceSyncObj = new object();

		/// <summary>
		/// Async sending. The capacity is set large enough to reduce the number of resizing
		/// operations while adding elements.
		/// </summary>
		private Queue<byte> sendQueue = new Queue<byte>(SendQueueInitialCapacity);

		private bool sendThreadSyncFlag;
		private AutoResetEvent sendThreadEvent;
		private Thread sendThread;

		/// <summary>
		/// Async receiving. The capacity is set large enough to reduce the number of resizing
		/// operations while adding elements.
		/// </summary>
		private Queue<byte> receiveQueue = new Queue<byte>(ReceiveQueueInitialCapacity);

		private bool receiveThreadSyncFlag;
		private AutoResetEvent receiveThreadEvent;
		private Thread receiveThread;

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
				// Finalize managed resources.

				if (disposing)
				{
					// In the 'normal' case, the items have already been disposed of, e.g. in Stop().
					DisposeDeviceAndThreads();
				}

				this.isDisposed = true;
			}
		}

		/// <summary></summary>
		~SerialHidDevice()
		{
			Dispose(false);
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
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed"));
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
				AssertNotDisposed();
				return (this.settings);
			}
		}

		/// <summary></summary>
		public virtual IO.Usb.DeviceInfo DeviceInfo
		{
			get
			{
				AssertNotDisposed();

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
				AssertNotDisposed();
				return (this.device != null);
			}
		}

		/// <summary></summary>
		public virtual bool IsConnected
		{
			get
			{
				AssertNotDisposed();

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
				AssertNotDisposed();

				if (this.device != null)
					return (this.device.IsOpen);

				return (false);
			}
		}

		/// <summary></summary>
		public virtual bool IsReadyToSend
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

			if (!IsStarted)
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
		public virtual void Send(byte[] data)
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
		private void SendThread()
		{
			Debug.WriteLine(GetType() + " '" + ToDeviceInfoString() + "': SendThread() has started.");

			while (this.sendThreadSyncFlag)
			{
				this.sendThreadEvent.WaitOne();

				// Read items from queue until there are no more.
				while (true)
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
				}
			}

			this.sendThread = null;

			// Do not Close() and de-reference the corresponding event as it may be Set() again
			// right now by another thread, e.g. during closing.

			Debug.WriteLine(GetType() + " '" + ToDeviceInfoString() + "': SendThread() has terminated.");
		}

		#endregion

		#region Device Methods
		//==========================================================================================
		// Device Methods
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
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

			StartThreads();

			IO.Usb.DeviceInfo di = this.settings.DeviceInfo;
			if (di != null)
			{
				lock (this.deviceSyncObj)
				{
					// Ensure to create device info from VID/PID/SNR since system path is not saved.
					this.device = new IO.Usb.SerialHidDevice(di.VendorId, di.ProductId, di.SerialNumber);
					this.device.AutoOpen = this.settings.AutoOpen;

					this.device.Connected    += new EventHandler(device_Connected);
					this.device.Disconnected += new EventHandler(device_Disconnected);
					this.device.Opened       += new EventHandler(device_Opened);
					this.device.Closed       += new EventHandler(device_Closed);
					this.device.DataReceived += new EventHandler(device_DataReceived);
				//	this.device.DataSent is not used, see remarks above.
					this.device.Error        += new EventHandler<IO.Usb.ErrorEventArgs>(device_Error);
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
				OnIOChanged(new EventArgs());
				return (success);
			}
			else
			{
				return (false);
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		private void DisposeDeviceAndThreads()
		{
			if (this.device != null)
			{
				try
				{
					RequestStopThreads();

					lock (this.deviceSyncObj)
					{
						this.device.Dispose();
						this.device = null;
					}
					OnIOChanged(new EventArgs());
				}
				catch { }
			}
		}

		#endregion

		#region Device Threads
		//==========================================================================================
		// Device Threads
		//==========================================================================================

		private void StartThreads()
		{
			// Ensure that threads have stopped after the last stop request.
			while ((this.receiveThread != null) && (this.sendThread != null))
				Thread.Sleep(1);

			this.sendThreadSyncFlag = true;
			this.sendThreadEvent = new AutoResetEvent(false);
			this.sendThread = new Thread(new ThreadStart(SendThread));
			this.sendThread.Start();

			this.receiveThreadSyncFlag = true;
			this.receiveThreadEvent = new AutoResetEvent(false);
			this.receiveThread = new Thread(new ThreadStart(ReceiveThread));
			this.receiveThread.Start();
		}

		/// <remarks>
		/// Just signal the threads, they will stop soon. Do not wait for them (i.e. Join()),
		/// this method could have been called from a thread that also has to handle the receive
		/// events (e.g. the application main thread). Waiting here would lead to deadlocks.
		/// </remarks>
		private void RequestStopThreads()
		{
			this.sendThreadSyncFlag = false;
			this.sendThreadEvent.Set();

			this.receiveThreadSyncFlag = false;
			this.receiveThreadEvent.Set();
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
		/// Asynchronously manage incoming events to prevent potential dead-locks if close/dispose
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
		private void ReceiveThread()
		{
			Debug.WriteLine(GetType() + " '" + ToDeviceInfoString() + "': ReceiveThread() has started.");

			while (this.receiveThreadSyncFlag)
			{
				this.receiveThreadEvent.WaitOne();

				// Fire events until there is no more data. Must be done to ensure that events
				// are fired even for data that was enqueued above while the 'OnDataReceived'
				// event was being handled. In addition, wait for the minimal time possible to
				// allow other threads to execute and to prevent that 'OnDataReceived' events
				// are fired consecutively.

				// Ensure not to forward any events during closing anymore.
				while (this.receiveThreadSyncFlag && IsOpen)
				{
					byte[] data;
					lock (this.receiveQueue)
					{
						if (this.receiveQueue.Count <= 0)
							break;

						data = this.receiveQueue.ToArray();
						this.receiveQueue.Clear();
					}

					OnDataReceived(new DataReceivedEventArgs(data));
					Thread.Sleep(0);
				}
			}

			this.receiveThread = null;

			// Do not Close() and de-reference the corresponding event as it may be Set() again
			// right now by another thread, e.g. during closing.

			Debug.WriteLine(GetType() + " '" + ToDeviceInfoString() + "': ReceiveThread() has terminated.");
		}

		private void device_Error(object sender, IO.Usb.ErrorEventArgs e)
		{
			OnIOError(new IOErrorEventArgs(e.Message));
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
			throw (new NotImplementedException("Event 'IOControlChanged' is not in use for USB Ser/HID devices"));
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
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
