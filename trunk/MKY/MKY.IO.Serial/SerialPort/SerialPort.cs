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
// MKY Development Version 1.0.8
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
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
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;

using MKY.Event;

#endregion

// The MKY.IO.Serial namespace combines various serial interface infrastructure. This code is
// intentionally placed into the MKY.IO.Serial namespace even though the file is located in
// MKY.IO.Serial\SerialPort for better separation of the implementation files.
namespace MKY.IO.Serial
{
	/// <summary></summary>
	/// <remarks>
	/// There is a serious deadlock issue in <see cref="System.IO.Ports.SerialPort"/>.
	/// Google for [UnauthorizedAccessException "Access to the port"] for more information and
	/// work-arounds suggestions.
	/// 
	/// ============================================================================================
	/// Source: http://msdn.microsoft.com/en-us/library/system.io.ports.serialport_methods.aspx (3.5)
	/// Author: Dan Randolph
	/// 
	/// There is a deadlock problem with the internal close operation of
	/// <see cref="System.IO.Ports.SerialPort"/>. Use BeginInvoke instead of Invoke from the
	/// serialPort_DataReceived event handler to start the method that reads from the
	/// SerialPort buffer and it will solve the problem. I finally tracked down the problem
	/// to the Close method by putting a start/stop button on the form. Then I was able to
	/// lock up the application and found that Close was the culpret. I'm pretty sure that
	/// components.Dispose() will end up calling the SerialPort Close method if it is open.
	/// 
	/// In my application, the user can change the baud rate and the port. In order to do
	/// this, the SerialPort must be closed fist and this caused a random deadlock in my
	/// application. Microsoft should document this better!
	/// ============================================================================================
	/// 
	/// Use case 1: Open/close a single time from GUI
	/// ---------------------------------------------
	/// 1. Start YAT
	/// 2. Open port
	/// 3. Close port
	/// 4. Exit YAT
	/// 
	/// Use case 2: Close/open multiple times from GUI
	/// ----------------------------------------------
	/// 1. Start YAT
	/// 2. Open port
	/// 3. Close port
	/// 4. Open port
	/// 5. Repeat close/open multiple times
	/// 6. Exit YAT
	/// 
	/// Use case 3: Close/disconnect/reconnect/open multiple times
	/// ----------------------------------------------------------
	/// 1. Start YAT
	/// 2. Open port
	/// 3. Close port
	/// 4. Disconnect USB-to-serial adapter
	/// 5. Reconnect USB-to-serial adapter
	/// 6. Open port
	/// 7. Repeat close/disconnect/reconnect/open multiple times
	/// 8. Exit YAT
	/// 
	/// Use case 4: Disconnect/reconnect multiple times
	/// -----------------------------------------------
	/// 1. Start YAT
	/// 2. Open port
	/// 3. Disconnect USB-to-serial adapter
	/// 4. Reconnect USB-to-serial adapter
	///    => System.UnauthorizedAccssException("Access is denied.")
	///       @ System.IO.Ports.InternalResources.WinIOError(int errorCode, string str)
	///       @ System.IO.Ports.SerialStream.Dispose(bool disposing)
	///       @ System.IO.Ports.SerialStream.Finalize()
	/// 5. Repeat disconnect/reconnect multiple times
	/// 6. Exit YAT
	/// 
	/// ============================================================================================
	/// (from above)
	/// 
	/// Use cases 1 through 3 work fine. But use case 4 results in an exception. Work-arounds tried
	/// in May 2008:
	/// - Async close
	/// - Async DataReceived event
	/// - Immediate async read
	/// - Dispatch of all open/close operations onto Windows.Forms main thread using OnRequest event
	/// - try GC.Collect(Forced) => no exceptions on GC, exception gets fired afterwards
	/// 
	/// --------------------------------------------------------------------------------------------
	/// 
	/// October 2011:
	/// Issue fixed by adding the DisposeBaseStream_SerialPortBugFix() to MKY.IO.Ports.SerialPortEx()
	/// 
	/// (see below)
	/// ============================================================================================
	/// Source: http://msdn.microsoft.com/en-us/library/system.io.ports.serialport_methods.aspx (3.5)
	/// Author: jmatos1
	/// 
	/// I suspect that adding a Dispose() call on the internalSerialStream might be a good change.
	/// ============================================================================================
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Different root namespace.")]
	public class SerialPort : IIOProvider, IXOnXOffHandler, IDisposable
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private enum State
		{
			Reset,
			Closed,
			Opened,
			WaitingForReopen,
			Error,
		}

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const int ReceiveQueueInitialCapacity = 4096;
		private const int SendQueueInitialCapacity    = 4096;

		private const int AliveInterval = 500;

		private const string Undefined = "<Undefined>";

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isDisposed;

		private State state = State.Reset;
		private object stateSyncObj = new object();
		
		private SerialPortSettings settings;

		/// <summary>
		/// Separate string containing the port name. Used for diagnostics/debug purposes.
		/// </summary>
		private string portName;

		private Ports.ISerialPort port;

		/// <summary>
		/// Async receiving. The capacity is set large enough to reduce the number of resizing
		/// operations while adding elements.
		/// </summary>
		private Queue<byte> receiveQueue = new Queue<byte>(ReceiveQueueInitialCapacity);

		/// <summary>
		/// Async sending. The capacity is set large enough to reduce the number of resizing
		/// operations while adding elements.
		/// </summary>
		private Queue<byte> sendQueue = new Queue<byte>(SendQueueInitialCapacity);

		/// <remarks>
		/// In case of manual RTS/CTS + DTR/DSR, RTS is enabled after initialization.
		/// </remarks>
		private bool manualRtsWasEnabled = true;

		/// <remarks>
		/// In case of manual RTS/CTS + DTR/DSR, DTR is disabled after initialization.
		/// </remarks>
		private bool manualDtrWasEnabled = false;

		/// <summary>
		/// Input XOn/XOff reflects the XOn/XOff state of this serial port itself, i.e. this computer.
		/// </summary>
		/// <remarks>
		/// Only applies in case of <see cref="SerialFlowControl.ManualSoftware"/> or <see cref="SerialFlowControl.ManualCombined"/>.
		/// </remarks>
		private bool inputIsXOn;
		private object inputIsXOnSyncObj = new object();

		/// <summary>
		/// Output XOn/XOff reflects the XOn/XOff state of the communication counterpart, i.e. a device.
		/// </summary>
		/// <remarks>
		/// Only applies in case of <see cref="SerialFlowControl.ManualSoftware"/> or <see cref="SerialFlowControl.ManualCombined"/>.
		/// </remarks>
		private bool outputIsXOn;
		private object outputIsXOnSyncObj = new object();

		/// <remarks>
		/// In case of manual XOn/XOff, input is initialized to XOn.
		/// </remarks>
		private bool manualInputWasXOn = true;
		private object manualInputWasXOnSyncObj = new object();

		/// <summary>
		/// Alive timer detects port disconnects, i.e. when a USB to serial converter is disconnected.
		/// </summary>
		private System.Timers.Timer aliveTimer;

		private System.Timers.Timer reopenTimer;

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
		public event EventHandler DataReceived;

		/// <summary></summary>
		public event EventHandler DataSent;

		/// <summary></summary>
		public event EventHandler<IOErrorEventArgs> IOError;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public SerialPort(SerialPortSettings settings)
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
				if (disposing)
				{
					// Ensure to reset state during Dispose().
					ResetPort();
				}
				this.isDisposed = true;
			}
		}

		/// <summary></summary>
		~SerialPort()
		{
			Dispose(false);
		}

		/// <summary></summary>
		protected bool IsDisposed
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
		public virtual SerialPortSettings Settings
		{
			get
			{
				AssertNotDisposed();
				return (this.settings);
			}
		}

		/// <summary></summary>
		public virtual bool IsStopped
		{
			get
			{
				AssertNotDisposed();

				switch (this.state)
				{
					case State.Reset:
					case State.Error:
					{
						return (true);
					}
					default:
					{
						return (false);
					}
				}
			}
		}

		/// <summary></summary>
		public virtual bool IsStarted
		{
			get
			{
				AssertNotDisposed();

				switch (this.state)
				{
					case State.Closed:
					case State.Opened:
					case State.WaitingForReopen:
					{
						return (true);
					}
					default:
					{
						return (false);
					}
				}
			}
		}

		private bool AutoReopenEnabledAndAllowed
		{
			get
			{
				return
					(
						!IsDisposed && IsStarted && !IsOpen &&
						this.settings.AutoReopen.Enabled
					);
			}
		}

		/// <summary></summary>
		public virtual bool IsOpen
		{
			get
			{
				AssertNotDisposed();

				if (this.port != null)
					return (this.port.IsOpen);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool IsConnected
		{
			get
			{
				AssertNotDisposed();

				if (IsOpen)
					return (!this.port.OutputBreak && !this.port.InputBreak);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool IsReadyToSend
		{
			get
			{
				AssertNotDisposed();

				if (IsOpen)
				{
					bool outputBreak = (this.settings.NoSendOnOutputBreak && this.port.OutputBreak);
					bool inputBreak  = (this.settings.NoSendOnInputBreak  && this.port.InputBreak);
					return (!outputBreak && !inputBreak);
				}
				else
				{
					return (false);
				}
			}
		}

		/// <summary></summary>
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

		/// <summary>
		/// Returns <c>true</c> if XOn/XOff is in use, i.e. if one or the other kind of XOn/XOff
		/// flow control is active.
		/// </summary>
		public virtual bool XOnXOffIsInUse
		{
			get
			{
				AssertNotDisposed();
				return (this.settings.Communication.FlowControlUsesXOnXOff);
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

				if (this.settings.Communication.FlowControlManagesXOnXOffManually)
				{
					lock (this.inputIsXOnSyncObj)
						return (this.inputIsXOn);
				}
				else
				{
					return (true);
				}
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

				if (this.settings.Communication.FlowControlManagesXOnXOffManually)
				{
					lock (this.outputIsXOnSyncObj)
						return (this.outputIsXOn);
				}
				else
				{
					return (true);
				}
			}
		}

		/// <summary></summary>
		public virtual object UnderlyingIOInstance
		{
			get
			{
				AssertNotDisposed();
				return (this.port);
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
			// AssertNotDisposed() is called by IsStarted.

			if (!IsStarted)
				CreateAndOpenPort();

			return (true);
		}

		/// <summary></summary>
		public virtual void Stop()
		{
			// AssertNotDisposed() is called by IsStarted.

			if (IsStarted)
				ResetPort();
		}

		/// <remarks>
		/// Typically, 'OnDataReceived' has been fired before this method is called. However, this
		/// method can also be called after the port got closed to retrieve the remaining data.
		/// </remarks>
		public virtual int Receive(out byte[] data)
		{
			AssertNotDisposed();

			// Don't care whether the port actually is open. It shall also be possible to retrieve
			// remaining data after the port got closed.
			int bytesReceived = 0;
			lock (this.receiveQueue)
			{
				bytesReceived = this.receiveQueue.Count;
				data = new byte[bytesReceived];
				for (int i = 0; i < bytesReceived; i++)
					data[i] = this.receiveQueue.Dequeue();
			}

			return (bytesReceived);
		}

		/// <summary>
		/// Asynchronously invoke outgoing send requests to ensure that software and/or hardware
		/// flow control is properley buffered and suspended if the communication counterpart
		/// requests so.
		/// Also, the mechanism implemented below reduces the amount of events that are propagated
		/// to the main application. Small chunks of sent data will would generate many events in
		/// <see cref="Send(byte[])"/>. However, since <see cref="OnDataSent"/> synchronously
		/// invokes the event, it will take some time until the monitor is released again. During
		/// this time, no more new events are invoked, instead, outgoing data is buffered.
		/// </summary>
		private delegate void SendDelegate();
		private object SendSyncObj = new object();

		/// <summary></summary>
		protected virtual void Send(byte data)
		{
			Send(new byte[] { data });
		}

		/// <summary></summary>
		public virtual void Send(byte[] data)
		{
			AssertNotDisposed();

			bool signalXOnXOff = false;

			lock (this.sendQueue)
			{
				foreach (byte b in data)
				{
					// Receive data into queue.
					this.sendQueue.Enqueue(b);

					// Handle input XOn/XOff.
					if (this.settings.Communication.FlowControlManagesXOnXOffManually)
					{
						if (b == SerialPortSettings.XOnByte)
						{
							lock (this.inputIsXOnSyncObj)
							{
								if (BooleanEx.SetIfCleared(ref this.inputIsXOn))
									signalXOnXOff = true;

								lock (this.manualInputWasXOnSyncObj)
									this.manualInputWasXOn = true;
							}
						}
						else if (b == SerialPortSettings.XOffByte)
						{
							lock (this.inputIsXOnSyncObj)
							{
								if (BooleanEx.ClearIfSet(ref this.inputIsXOn))
									signalXOnXOff = true;

								lock (this.manualInputWasXOnSyncObj)
									this.manualInputWasXOn = false;
							}
						}
					}
				}
			}

			if (signalXOnXOff)
				OnIOControlChanged(new EventArgs());

			// Ensure that only one data send thread is active at a time.
			// Without this exclusivity, two send threads could create a race condition.
			if (Monitor.TryEnter(this.SendSyncObj))
			{
				try
				{
					SendDelegate asyncInvoker = new SendDelegate(SendAsynch);
					asyncInvoker.BeginInvoke(null, null);
				}
				finally
				{
					Monitor.Exit(this.SendSyncObj);
				}
			}
		}

		private void SendAsynch()
		{
			// Ensure that only one data send thread is active at a time.
			// Without this exclusivity, two receive threads could create a race condition.
			Monitor.Enter(this.SendSyncObj);
			try
			{
				// Fire events until there is no more data. Must be done to ensure that events
				// are fired even for data that was enqueued above while the sync obj was busy.
				// In addition, wait for the minimal time possible to allow other threads to
				// execute and to prevent that 'OnDataSent' events are fired consecutively.
				while (true)
				{
					// Handle output break state.
					bool isOutputBreak;
					lock (this.port)
						isOutputBreak = this.port.OutputBreak;

					if (!isOutputBreak)
					{
						// In case of XOff, let other threads do their job and then try again.
						if (XOnXOffIsInUse && !OutputIsXOn)
						{
							Thread.Sleep(0);
							continue;
						}

						// In case of disabled CTS line, let other threads do their job and then try again.
						if (this.settings.Communication.FlowControlUsesRtsCts)
						{
							bool isClearToSend;
							lock (this.port)
								isClearToSend = this.port.CtsHolding;

							if (!isClearToSend)
							{
								Thread.Sleep(0);
								continue;
							}
						}

						// No break, no XOff, no CTS disable, ready to send.
						byte[] buffer;
						lock (this.sendQueue)
						{
							if (this.sendQueue.Count <= 0)
								break;

							buffer = this.sendQueue.ToArray();
							this.sendQueue.Clear();
						}
						lock (this.port)
						{
							if (this.settings.Communication.FlowControl == SerialFlowControl.RS485)
								this.port.RtsEnable = true;

							this.port.Write(buffer, 0, buffer.Length);

							if (this.settings.Communication.FlowControl == SerialFlowControl.RS485)
								this.port.RtsEnable = false;
						}

						OnDataSent(new EventArgs());
						Thread.Sleep(0);
					}
					else
					{
						OnIOError(new IOErrorEventArgs(IOErrorSeverity.Acceptable, IODirection.Output, "No data can be sent while port is in output break state"));
						break;
					}
				}
			}
			finally
			{
				Monitor.Exit(this.SendSyncObj);
			}
		}

		/// <summary></summary>
		protected virtual void AssumeOutputXOn()
		{
			lock (this.outputIsXOnSyncObj)
				this.outputIsXOn = true;

			OnIOControlChanged(new EventArgs());
		}

		/// <summary>
		/// Signals the other communication endpoint that this device is in XOn state.
		/// </summary>
		public virtual void SetInputXOn()
		{
			AssertNotDisposed();

			Send(SerialPortSettings.XOnByte);
		}

		/// <summary>
		/// Signals the other communication endpoint that this device is in XOff state.
		/// </summary>
		public virtual void SetInputXOff()
		{
			AssertNotDisposed();

			Send(SerialPortSettings.XOffByte);
		}

		/// <summary>
		/// Toggles the input XOn/XOff state.
		/// </summary>
		public virtual void ToggleInputXOnXOff()
		{
			// AssertNotDisposed() and HandshakeIsNotUsingXOnXOff { get; } is called in functions below.

			if (InputIsXOn)
				SetInputXOff();
			else
				SetInputXOn();
		}

		#endregion

		#region Settings Methods
		//==========================================================================================
		// Settings Methods
		//==========================================================================================

		private void ApplySettings()
		{
			if (this.port == null)
				return;

			lock (this.port)
			{
				// Keep port name for diagnostics/debug purposes:
				this.portName    = this.settings.PortId;
				this.port.PortId = this.settings.PortId;

				SerialCommunicationSettings s = this.settings.Communication;
				this.port.BaudRate  = (MKY.IO.Ports.BaudRateEx)s.BaudRate;
				this.port.DataBits  = (MKY.IO.Ports.DataBitsEx)s.DataBits;
				this.port.Parity    = s.Parity;
				this.port.StopBits  = s.StopBits;
				this.port.Handshake = (SerialFlowControlEx)s.FlowControl;
			}
		}

		#endregion

		#region State Methods
		//==========================================================================================
		// State Methods
		//==========================================================================================

		private State GetStateSynchronized()
		{
			State state;

			lock (this.stateSyncObj)
				state = this.state;

			return (state);
		}

		private void SetStateSynchronizedAndNotify(State state)
		{
#if (DEBUG)
			State oldState = this.state;
#endif
			lock (this.stateSyncObj)
				this.state = state;
#if (DEBUG)
			Debug.WriteLine(GetType() + " '" + ToShortPortString() + "': State has changed from " + oldState + " to " + this.state + ".");
#endif
			OnIOChanged(new EventArgs());
			OnIOControlChanged(new EventArgs());
		}

		#endregion

		#region Simple Port Methods
		//==========================================================================================
		// Simple Port Methods
		//==========================================================================================

		private void CreatePort()
		{
			if (this.port != null)
				CloseAndDisposePort();

			this.port = new Ports.SerialPortEx();
			this.port.DataReceived  += new Ports.SerialDataReceivedEventHandler (port_DataReceived);
			this.port.PinChanged    += new Ports.SerialPinChangedEventHandler   (port_PinChanged);
			this.port.ErrorReceived += new Ports.SerialErrorReceivedEventHandler(port_ErrorReceived);
		}

		private void OpenPort()
		{
			if (this.port != null)
			{
				lock (this.port)
				{
					if (!this.port.IsOpen)
						this.port.Open();
				}
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		private void CloseAndDisposePort()
		{
			if (this.port != null)
			{
				try
				{
					if (this.port.IsOpen)
						this.port.Close();
				}
				finally
				{
					this.port.Dispose();
					this.port = null;
				}
			}
		}

		#endregion

		#region Port Methods
		//==========================================================================================
		// Port Methods
		//==========================================================================================

		/// <summary></summary>
		private void CreateAndOpenPort()
		{
			CreatePort();          // Port must be created each time because this.port.Close()
			ApplySettings();       //   disposes the underlying IO instance

			lock (this.port)
			{
				// RTS
				switch (this.settings.Communication.FlowControl)
				{
					case SerialFlowControl.Hardware:
					case SerialFlowControl.Combined:
						// Do nothing, RTS is handled by the underlying serial port object.
						break;

					case SerialFlowControl.RS485:
						this.port.RtsEnable = false;
						break;

					case SerialFlowControl.ManualHardware:
					case SerialFlowControl.ManualCombined:
						this.port.RtsEnable = this.manualRtsWasEnabled;
						break;

					default:
						this.port.RtsEnable = false;
						break;
				}

				// DTR
				switch (this.settings.Communication.FlowControl)
				{
					case SerialFlowControl.ManualHardware:
					case SerialFlowControl.ManualCombined:
						this.port.DtrEnable = this.manualDtrWasEnabled;
						break;

					default:
						this.port.DtrEnable = false;
						break;
				}
			} // lock (this.portSyncObj)

			OpenPort();
			StartAliveTimer();
			SetStateSynchronizedAndNotify(State.Opened);

			// Handle XOn/XOff
			if (XOnXOffIsInUse)
			{
				// Assume XOn on input.
				AssumeOutputXOn();

				// Immediately send XOn if software flow control is enabled to ensure that
				//   device gets put into XOn if it was XOff before.
				switch (this.settings.Communication.FlowControl)
				{
					case SerialFlowControl.ManualSoftware:
					case SerialFlowControl.ManualCombined:
						bool wasXOn = false;
						lock (this.manualInputWasXOnSyncObj)
						{
							wasXOn = this.manualInputWasXOn;
						}
						if (wasXOn)
							SetInputXOn();
						break;

					default:
						SetInputXOn();
						break;
				}
			}
		}

		/// <summary></summary>
		private void StopOrClosePort()
		{
			if (this.settings.AutoReopen.Enabled)
			{
				StopAndDisposeAliveTimer();
				CloseAndDisposePort();
				SetStateSynchronizedAndNotify(State.Closed);
				OnIOControlChanged(new EventArgs());

				StartReopenTimer();
				SetStateSynchronizedAndNotify(State.WaitingForReopen);
			}
			else
			{
				Stop();
			}
		}

		/// <summary></summary>
		private void ResetPort()
		{
			StopAndDisposeAliveTimer();
			StopAndDisposeReopenTimer();
			CloseAndDisposePort();
			SetStateSynchronizedAndNotify(State.Reset);
		}

		#endregion

		#region Port Events
		//==========================================================================================
		// Port Events
		//==========================================================================================

		/// <summary>
		/// Asynchronously invoke incoming events to prevent potential dead-locks if close/dispose
		/// was called from a ISynchronizeInvoke target (i.e. a form) on an event thread.
		/// Also, the mechanism implemented below reduces the amount of events that are propagated
		/// to the main application. Small chunks of received data will generate many events
		/// handled by <see cref="port_DataReceived"/>. However, since <see cref="OnDataReceived"/>
		/// synchronously invokes the event, it will take some time until the monitor is released
		/// again. During this time, no more new events are invoked, instead, incoming data is
		/// buffered.
		/// </summary>
		private delegate void port_DataReceivedDelegate(object sender, MKY.IO.Ports.SerialDataReceivedEventArgs e);
		private object port_DataReceivedSyncObj = new object();

		private void port_DataReceived(object sender, MKY.IO.Ports.SerialDataReceivedEventArgs e)
		{
			if (this.state == State.Opened) // Ensure not to forward any events during closing anymore.
			{
				// Immediately read data on this thread.
				int bytesToRead;
				byte[] buffer;
				lock (this.port)
				{
					bytesToRead = this.port.BytesToRead;
					buffer = new byte[bytesToRead];
					this.port.Read(buffer, 0, bytesToRead);
				}

				bool signalXOnXOff = false;

				lock (this.receiveQueue)
				{
					foreach (byte b in buffer)
					{
						// Receive data into queue.
						this.receiveQueue.Enqueue(b);

						// Handle output XOn/XOff.
						if (this.settings.Communication.FlowControlManagesXOnXOffManually)
						{
							if (b == SerialPortSettings.XOnByte)
							{
								lock (this.outputIsXOnSyncObj)
								{
									if (BooleanEx.SetIfCleared(ref this.outputIsXOn))
										signalXOnXOff = true;
								}
							}
							else if (b == SerialPortSettings.XOffByte)
							{
								lock (this.outputIsXOnSyncObj)
								{
									if (BooleanEx.ClearIfSet(ref this.outputIsXOn))
										signalXOnXOff = true;
								}
							}
						}
					}
				}

				// Immediately invoke the event, but invoke it asynchronously!
				if (signalXOnXOff)
					OnIOControlChangedAsync(new EventArgs());

				// Ensure that only one data received event thread is active at a time.
				// Without this exclusivity, two receive threads could create a race condition.
				if (Monitor.TryEnter(this.port_DataReceivedSyncObj))
				{
					try
					{
						port_DataReceivedDelegate asyncInvoker = new port_DataReceivedDelegate(port_DataReceivedAsync);
						asyncInvoker.BeginInvoke(sender, e, null, null);
					}
					finally
					{
						Monitor.Exit(this.port_DataReceivedSyncObj);
					}
				}
			}
		}

		private void port_DataReceivedAsync(object sender, MKY.IO.Ports.SerialDataReceivedEventArgs e)
		{
			// Ensure that only one data received event thread is active at a time.
			// Without this exclusivity, two receive threads could create a race condition.
			Monitor.Enter(this.port_DataReceivedSyncObj);
			try
			{
				// Fire events until there is no more data. Must be done to ensure that events
				// are fired even for data that was enqueued above while the sync obj was busy.
				// In addition, wait for the minimal time possible to allow other threads to
				// execute and to prevent that 'OnDataReceived' events are fired consecutively.
				//
				// Measurements 2011-04-24 on an Intel Core 2 Duo running Win7 at 2.4 GHz and 3 GB of RAM:
				// > 0.0% CPU load in idle
				// > Up to an short-term-average of 20% CPU load while sending a large chuck of text (\YAT\!-SendFiles\Stress-2-Large.txt, 106 kB)
				// This is an acceptable CPU load.
				while ((this.state == State.Opened) && (BytesAvailable > 0)) // Ensure not to forward any events during closing anymore.
				{
					OnDataReceived(new EventArgs());
					Thread.Sleep(0);
				}
			}
			finally
			{
				Monitor.Exit(this.port_DataReceivedSyncObj);
			}
		}

		// Additional information to the 'DataReceived' event
		// --------------------------------------------------
		// An improvement suggested by Marco Stroppel on 2011-02-17 doesn't work in case of YAT. Suggestion:
		// 
		//   The while(BytesAvailable > 0) fires endless events, because I did not call the Receive() method.
		//   That was, because I receive only the data when the other port to write the data is opened. So the
		//   BytesAvailable got never zero. My idea was (not knowing if this is good) to do something like:
		//   
		//   while(BytesAvailable > LastTimeBytesAvailable)
		//   {
		//       LastTimeBytesAvailable = BytesAvailable;
		//       OnDataReceived(new EventArgs());
		//   }
		// 
		// This suggestions doesn't work because YAT shall show every single byte as soon as it get's received.
		// If 3 bytes are received while 5 bytes are taken out of the receive queue, no more event gets fired.
		// Thus, the 3 bytes do not get shown until new data arrives. This is not acceptable.

		/// <summary>
		/// Asynchronously invoke incoming events to prevent potential dead-locks if close/dispose
		/// was called from a ISynchronizeInvoke target (i.e. a form) on an event thread.
		/// </summary>
		private delegate void port_PinChangedDelegate(object sender, MKY.IO.Ports.SerialPinChangedEventArgs e);

		private void port_PinChanged(object sender, MKY.IO.Ports.SerialPinChangedEventArgs e)
		{
			if (this.state == State.Opened) // Ensure not to forward any events during closing anymore.
			{
				port_PinChangedDelegate asyncInvoker = new port_PinChangedDelegate(port_PinChangedAsync);
				asyncInvoker.BeginInvoke(sender, e, null, null);
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		private void port_PinChangedAsync(object sender, MKY.IO.Ports.SerialPinChangedEventArgs e)
		{
			// If pin has changed, but access to port throws exception, port has been shut down,
			//   e.g. USB to serial converter disconnected.
			try
			{
				// Force access to port to check whether it's still alive.
				bool cts = this.port.CtsHolding;

				if (this.state == State.Opened) // Ensure not to forward any events during closing anymore.
				{
					if (this.settings.Communication.FlowControlManagesRtsCtsDtrDsrManually)
					{
						this.manualRtsWasEnabled = this.port.RtsEnable;
						this.manualDtrWasEnabled = this.port.DtrEnable;
					}

					switch (e.EventType)
					{
						case MKY.IO.Ports.SerialPinChange.InputBreak:
							if (this.settings.NoSendOnInputBreak)
								OnIOChanged(new EventArgs());
							break;

						case MKY.IO.Ports.SerialPinChange.OutputBreak:
							OnIOChanged(new EventArgs());
							break;

						default:
							// Do not fire general event, I/O control event is fired below.
							break;
					}
					OnIOControlChanged(new EventArgs());
				}
			}
			catch
			{
				StopOrClosePort();
			}
		}

		/// <summary>
		/// Asynchronously invoke incoming events to prevent potential dead-locks if close/dispose
		/// was called from a ISynchronizeInvoke target (i.e. a form) on an event thread.
		/// </summary>
		private delegate void port_ErrorReceivedDelegate(object sender, MKY.IO.Ports.SerialErrorReceivedEventArgs e);

		private void port_ErrorReceived(object sender, MKY.IO.Ports.SerialErrorReceivedEventArgs e)
		{
			if (this.state == State.Opened) // Ensure not to forward any events during closing anymore.
			{
				port_ErrorReceivedDelegate asyncInvoker = new port_ErrorReceivedDelegate(port_ErrorReceivedAsync);
				asyncInvoker.BeginInvoke(sender, e, null, null);
			}
		}

		private void port_ErrorReceivedAsync(object sender, MKY.IO.Ports.SerialErrorReceivedEventArgs e)
		{
			string message;
			switch (e.EventType)
			{
				case System.IO.Ports.SerialError.Frame:    message = "Serial port input framing error!";            break;
				case System.IO.Ports.SerialError.Overrun:  message = "Serial port input character buffer overrun!"; break;
				case System.IO.Ports.SerialError.RXOver:   message = "Serial port input buffer overflow!";          break;
				case System.IO.Ports.SerialError.RXParity: message = "Serial port input parity error!";             break;
				case System.IO.Ports.SerialError.TXFull:   message = "Serial port output buffer full!";             break;
				default:                                   message = "Unknown serial port error!";                  break;
			}
			OnIOError(new SerialPortIOErrorEventArgs(message, e.EventType));
		}

		#endregion

		#region Alive Timer
		//==========================================================================================
		// Alive Timer
		//==========================================================================================

		private void StartAliveTimer()
		{
			if (this.aliveTimer == null)
			{
				this.aliveTimer = new System.Timers.Timer(AliveInterval);
				this.aliveTimer.AutoReset = true;
				this.aliveTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.aliveTimer_Elapsed);
				this.aliveTimer.Start();
			}
		}

		private void StopAndDisposeAliveTimer()
		{
			if (this.aliveTimer != null)
			{
				this.aliveTimer.Stop();
				this.aliveTimer.Dispose();
				this.aliveTimer = null;
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		private void aliveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (!IsDisposed && IsStarted)
			{
				try
				{
					// If port isn't open anymore, or access to port throws exception,
					//   port has been shut down, e.g. USB to serial converter disconnected.
					if (!this.port.IsOpen)
						StopOrClosePort();
				}
				catch
				{
					StopOrClosePort();
				}
			}
			else
			{
				StopAndDisposeAliveTimer();
			}
		}

		#endregion

		#region Reopen Timer
		//==========================================================================================
		// Reopen Timer
		//==========================================================================================

		private void StartReopenTimer()
		{
			if (this.reopenTimer == null)
			{
				this.reopenTimer = new System.Timers.Timer(this.settings.AutoReopen.Interval);
				this.reopenTimer.AutoReset = false;
				this.reopenTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.reopenTimer_Elapsed);
			}
			this.reopenTimer.Start();
		}

		private void StopAndDisposeReopenTimer()
		{
			if (this.reopenTimer != null)
			{
				this.reopenTimer.Stop();
				this.reopenTimer.Dispose();
				this.reopenTimer = null;
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		private void reopenTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (AutoReopenEnabledAndAllowed)
			{
				try
				{
					// Try to re-open port.
					CreateAndOpenPort();
				}
				catch
				{
					// Re-open failed, cleanup and restart.
					CloseAndDisposePort();
					StartReopenTimer();
				}
			}
			else
			{
				StopAndDisposeReopenTimer();
			}
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
			EventHelper.FireSync(IOControlChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOControlChangedAsync(EventArgs e)
		{
			EventHelper.FireSync(IOControlChanged, this, e);
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

		/// <summary></summary>
		protected virtual void OnIOError(IOErrorEventArgs e)
		{
			EventHelper.FireSync<IOErrorEventArgs>(IOError, this, e);
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary></summary>
		public virtual string ToShortPortString()
		{
			if (this.port != null)
				return (this.port.PortId);
			else if (!string.IsNullOrEmpty(this.portName))
				return (this.portName);
			else
				return (Undefined);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
