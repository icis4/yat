//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
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
// Copyright © 2003-2016 Matthias Kläy.
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
using MKY.Time;

#endregion

namespace MKY.IO.Serial.SerialPort
{
	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop doesn't seem to be able to skip URLs...")]
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

		private const int SendQueueInitialCapacity = 4096;
		private const int ReceiveQueueInitialCapacity = 4096;

		private const int ThreadWaitTimeout = 200;
		private const int AliveInterval = 500;

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

		private State state = State.Reset;
		private ReaderWriterLockSlim stateLock = new ReaderWriterLockSlim();
		
		private SerialPortSettings settings;

		private Ports.ISerialPort port;
		private object portSyncObj = new object(); // Required as port will be disposed on recreated on open/close.

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

		/// <remarks>
		/// In case of manual RFR/CTS + DTR/DSR, RFR is enabled after initialization.
		/// </remarks>
		private bool manualRfrWasEnabled = true;

		/// <remarks>
		/// In case of manual RFR/CTS + DTR/DSR, DTR is disabled after initialization.
		/// </remarks>
		private bool manualDtrWasEnabled; // = false

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

		private int sentXOnCount;
		private int sentXOffCount;
		private int receivedXOnCount;
		private int receivedXOffCount;

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
				// Dispose of managed resources if requested:
				if (disposing)
				{
					// In the 'normal' case, the items have already been disposed of, e.g. in Stop().
					ResetPortAndThreadsAndNotify(false); // Suppress notifications during disposal.

					this.stateLock.Dispose();
				}

				// Set state to disposed:
				this.isDisposed = true;
			}
		}

		/// <summary></summary>
		~SerialPort()
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
				// Do not call AssertNotDisposed() in a simple get-property.

				switch (GetStateSynchronized())
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
				// Do not call AssertNotDisposed() in a simple get-property.

				switch (GetStateSynchronized())
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
				return (!IsDisposed && IsStarted && !IsOpen && this.settings.AutoReopen.Enabled); // Check 'IsDisposed' first!
			}
		}

		/// <summary></summary>
		public virtual bool IsOpen
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				lock (this.portSyncObj)
				{
					if (this.port != null)
						return (this.port.IsOpen);
					else
						return (false);
				}
			}
		}

		/// <summary></summary>
		public virtual bool IsConnected
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				lock (this.portSyncObj)
				{
					if (IsOpen)
						return (!this.port.OutputBreak && !this.port.InputBreak);
					else
						return (false);
				}
			}
		}

		/// <summary></summary>
		public virtual bool IsTransmissive
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				lock (this.portSyncObj)
				{
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
		}

		/// <summary>
		/// Serial port control pins.
		/// </summary>
		public virtual Ports.SerialPortControlPins ControlPins
		{
			get
			{
				AssertNotDisposed();

				lock (this.portSyncObj)
				{
					if (this.port != null)
						return (this.port.ControlPins);
					else
						return (new Ports.SerialPortControlPins());
				}
			}
		}

		/// <summary>
		/// Serial port control pin counts.
		/// </summary>
		public virtual Ports.SerialPortControlPinCount ControlPinCount
		{
			get
			{
				AssertNotDisposed();

				lock (this.portSyncObj)
				{
					if (this.port != null)
						return (this.port.ControlPinCount);
					else
						return (new Ports.SerialPortControlPinCount());
				}
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

		/// <summary>
		/// Returns the number of sent XOn characters, i.e. the count of input XOn/XOff signalling.
		/// </summary>
		public virtual int SentXOnCount
		{
			get
			{
				AssertNotDisposed();

				if (this.settings.Communication.FlowControlManagesXOnXOffManually)
					return (this.sentXOnCount);
				else
					return (0);
			}
		}

		/// <summary>
		/// Returns the number of sent XOff characters, i.e. the count of input XOn/XOff signalling.
		/// </summary>
		public virtual int SentXOffCount
		{
			get
			{
				AssertNotDisposed();

				if (this.settings.Communication.FlowControlManagesXOnXOffManually)
					return (this.sentXOffCount);
				else
					return (0);
			}
		}

		/// <summary>
		/// Returns the number of received XOn characters, i.e. the count of output XOn/XOff signalling.
		/// </summary>
		public virtual int ReceivedXOnCount
		{
			get
			{
				AssertNotDisposed();

				if (this.settings.Communication.FlowControlManagesXOnXOffManually)
					return (this.receivedXOnCount);
				else
					return (0);
			}
		}

		/// <summary>
		/// Returns the number of received XOff characters, i.e. the count of output XOn/XOff signalling.
		/// </summary>
		public virtual int ReceivedXOffCount
		{
			get
			{
				AssertNotDisposed();

				if (this.settings.Communication.FlowControlManagesXOnXOffManually)
					return (this.receivedXOffCount);
				else
					return (0);
			}
		}

		/// <summary></summary>
		public virtual int OutputBreakCount
		{
			get
			{
				AssertNotDisposed();

				lock (this.portSyncObj)
				{
					if (this.port != null)
						return (this.port.OutputBreakCount);
					else
						return (0);
				}
			}
		}

		/// <summary></summary>
		public virtual int InputBreakCount
		{
			get
			{
				AssertNotDisposed();

				lock (this.portSyncObj)
				{
					if (this.port != null)
						return (this.port.InputBreakCount);
					else
						return (0);
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
			// AssertNotDisposed() is called by 'IsStarted' below.

			if (IsStopped)
			{
				WriteDebugMessageLine("Starting...");
				try
				{
					CreateAndOpenPortAndThreadsAndNotify();
				}
				catch
				{
					ResetPortAndThreadsAndNotify();
					throw; // Re-throw!
				}
			}
			else
			{
				WriteDebugMessageLine("Start() requested but state is " + GetStateSynchronized() + ".");
			}

			return (true); // Return 'true' in any case since port is open in the end.
		}

		/// <summary></summary>
		public virtual void Stop()
		{
			// AssertNotDisposed() is called by 'IsStarted' below.

			if (IsStarted)
			{
				WriteDebugMessageLine("Stopping...");
				ResetPortAndThreadsAndNotify();
			}
			else
			{
				WriteDebugMessageLine("Stop() requested but state is " + GetStateSynchronized() + ".");
			}
		}

		/// <summary></summary>
		protected virtual bool Send(byte data)
		{
			// AssertNotDisposed() is called by 'Send()' below.

			return (Send(new byte[] { data }));
		}

		/// <summary></summary>
		public virtual bool Send(byte[] data)
		{
			// AssertNotDisposed() is called by 'IsStarted' below.

			if (IsStarted)
			{
				bool signalXOnXOff = false;
				bool signalXOnXOffCount = false;

				lock (this.sendQueue)
				{
					foreach (byte b in data)
					{
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

								Interlocked.Increment(ref this.sentXOnCount);
								signalXOnXOffCount = true;
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

								Interlocked.Increment(ref this.sentXOffCount);
								signalXOnXOffCount = true;
							}
						}
					}
				}

				// Signal XOn/XOff change to receive thread:
				if (signalXOnXOff)
					this.receiveThreadEvent.Set();

				// Immediately invoke the event:
				if (signalXOnXOff || signalXOnXOffCount)
					OnIOControlChanged(EventArgs.Empty);

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
		/// Asynchronously manage outgoing send requests to ensure that software and/or hardware
		/// flow control is properly buffered and suspended if the communication counterpart
		/// requests so.
		/// Also, the mechanism implemented below reduces the amount of events that are propagated
		/// to the main application. Small chunks of sent data would generate many events in
		/// <see cref="Send(byte[])"/>. However, since <see cref="OnDataSent"/> synchronously
		/// invokes the event, it will take some time until the send queue is checked again.
		/// During this time, no more new events are invoked, instead, outgoing data is buffered.
		/// </summary>
		/// <remarks>
		/// Will be signaled by <see cref="Send(byte[])"/> method above, or by XOn/XOff while receiving.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that any exception leads to restart or reset of port.")]
		[SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", MessageId = "System.Threading.WaitHandle.#WaitOne(System.Int32)", Justification = "Installer indeed targets .NET 3.5 SP1.")]
		private void SendThread()
		{
			RateHelper sendRate = new RateHelper(this.settings.MaxSendRate.Interval);

			bool isOutputBreakOldAndErrorHasBeenSignaled = false;

			WriteDebugThreadStateMessageLine("SendThread() has started.");

			// If access to port throws exception, port has been shut down, e.g. USB to serial converter
			// was disconnected.
			try
			{
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
						// Handle output break state. System.IO.Ports.SerialPort.Write() will raise
						// an exception when trying to write while in output break!
						bool isOutputBreak;
						lock (this.portSyncObj)
							isOutputBreak = this.port.OutputBreak;

						if (!isOutputBreak)
						{
							// Reset the flag, once the output break has been reset:
							isOutputBreakOldAndErrorHasBeenSignaled = false;

							// In case of XOff:
							if (XOnXOffIsInUse && !OutputIsXOn)
								break; // Let other threads do their job and wait until signaled again.

							// In case of disabled CTS line:
							if (this.settings.Communication.FlowControlUsesRfrCts)
							{
								bool isClearToSend;
								lock (this.portSyncObj)
									isClearToSend = this.port.CtsHolding;

								if (!isClearToSend)
									break; // Let other threads do their job and wait until signaled again.
							}

							// No break, no XOff, no CTS disable, ready to send.

							// Something to send?
							lock (this.sendQueue)
							{
								if (this.sendQueue.Count <= 0)
									break; // Let other threads do their job and wait until signaled again.
							}

							// Send it!
							byte[] chunkData;
							lock (this.portSyncObj)
							{
								if (this.settings.Communication.FlowControl == SerialFlowControl.RS485)
									this.port.RfrEnable = true;

								// By default, stuff as much data as possible into output buffer:
								int chunkSize = (this.port.WriteBufferSize - this.port.BytesToWrite);

								// Reduce the chunk size if maximum send rate is specified:
								if (this.settings.MaxSendRate.Enabled)
								{
									int remainingSizeInInterval = (this.settings.MaxSendRate.Size - sendRate.Value);
									chunkSize = Int32Ex.LimitToBounds(chunkSize, 0, remainingSizeInInterval);
								}

								// Retrieve the chunk from the send queue:
								List<byte> chunkList = new List<byte>(chunkSize);
								lock (this.sendQueue)
								{
									for (int i = 0; (i < chunkSize) && (this.sendQueue.Count > 0); i++)
										chunkList.Add(this.sendQueue.Dequeue());
								}
								chunkData = chunkList.ToArray();

								// Update the send rate with the effective chunk size:
								if (this.settings.MaxSendRate.Enabled)
									sendRate.Update(chunkData.Length);

								try
								{
									this.port.Write(chunkData, 0, chunkData.Length);
									this.port.Flush(); // Make sure that data is sent before continuing.
								}
								catch (TimeoutException ex)
								{
									// \remind (2012-09-19 / mky)
									// This try-catch works around YAT issue #255 "Manual software
									// flow control may lead to data loss in case of too long XOff"
									if (this.settings.Communication.FlowControlUsesXOnXOff)
										DebugEx.WriteException(GetType(), ex, "SendThread() has provoked a timeout while writing to the port, ignoring the issue since it got caused due to XOff.");
									else
										throw; // Re-throw!
								}

								if (this.settings.Communication.FlowControl == SerialFlowControl.RS485)
									this.port.RfrEnable = false;
							}

							OnDataSent(new DataSentEventArgs(chunkData));

							// Wait for the minimal time possible to allow other threads to execute and
							// to prevent that 'DataSent' events are fired consecutively.
							Thread.Sleep(TimeSpan.Zero);
						}
						else
						{
							// If data is intended to be sent, and the output has changed to break,
							// write an error message onto the terminal:
							if ((this.sendQueue.Count > 0) && isOutputBreak && !isOutputBreakOldAndErrorHasBeenSignaled)
							{
								OnIOError(new IOErrorEventArgs
								(
									ErrorSeverity.Acceptable,
									Direction.Output,
									"No data can be sent while port is in output break state")
								);

								isOutputBreakOldAndErrorHasBeenSignaled = true;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(GetType(), ex, "SendThread() has detected shutdown of port.");
				RestartOrResetPortAndThreadsAndNotify();
			}

			WriteDebugThreadStateMessageLine("SendThread() has terminated.");
		}

		/// <summary></summary>
		protected virtual void AssumeOutputXOn()
		{
			lock (this.outputIsXOnSyncObj)
				this.outputIsXOn = true;

			OnIOControlChanged(EventArgs.Empty);
		}

		/// <summary>
		/// Signals the other communication endpoint that this device is in XOn state.
		/// </summary>
		public virtual void SignalInputXOn()
		{
			AssertNotDisposed();

			Send(SerialPortSettings.XOnByte);
		}

		/// <summary>
		/// Signals the other communication endpoint that this device is in XOff state.
		/// </summary>
		public virtual void SignalInputXOff()
		{
			AssertNotDisposed();

			Send(SerialPortSettings.XOffByte);
		}

		/// <summary>
		/// Toggles the input XOn/XOff state.
		/// </summary>
		public virtual void ToggleInputXOnXOff()
		{
			// AssertNotDisposed() and FlowControlManagesXOnXOffManually { get; } are called by the
			// 'InputIsXOn' property.

			if (InputIsXOn)
				SignalInputXOff();
			else
				SignalInputXOn();
		}

		/// <summary>
		/// Resets the XOn/XOff signalling count.
		/// </summary>
		public virtual void ResetXOnXOffCount()
		{
			AssertNotDisposed();

			Interlocked.Exchange(ref this.sentXOnCount, 0);
			Interlocked.Exchange(ref this.sentXOffCount, 0);
			Interlocked.Exchange(ref this.receivedXOnCount, 0);
			Interlocked.Exchange(ref this.receivedXOffCount, 0);

			OnIOControlChanged(EventArgs.Empty);
		}

		/// <summary>
		/// Resets the flow control signalling count.
		/// </summary>
		public virtual void ResetFlowControlCount()
		{
			// AssertNotDisposed() is called by 'ResetXOnXOffCount()' below.

			lock (this.portSyncObj)
			{
				ResetXOnXOffCount();

				if (this.port != null)
					this.port.ResetControlPinCount();
			}
		}

		/// <summary>
		/// Resets the break count.
		/// </summary>
		public virtual void ResetBreakCount()
		{
			AssertNotDisposed();

			lock (this.portSyncObj)
			{
				if (this.port != null)
					this.port.ResetBreakCount();
			}
		}

		#endregion

		#region Settings Methods
		//==========================================================================================
		// Settings Methods
		//==========================================================================================

		private void ApplySettings()
		{
			lock (this.portSyncObj)
			{
				if (this.port == null)
					return;

				this.port.PortId = this.settings.PortId;

				if (this.settings.LimitOutputBuffer.Enabled)
					this.port.WriteBufferSize = this.settings.LimitOutputBuffer.Size;

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

			this.stateLock.EnterReadLock();
			state = this.state;
			this.stateLock.ExitReadLock();

			return (state);
		}

		private void SetStateSynchronizedAndNotify(State state)
		{
			SetStateSynchronizedAndNotify(state, true);
		}

		private void SetStateSynchronizedAndNotify(State state, bool withNotify)
		{
#if (DEBUG)
			State oldState = GetStateSynchronized();
#endif
			this.stateLock.EnterWriteLock();
			this.state = state;
			this.stateLock.ExitWriteLock();
#if (DEBUG)
			if (this.state != oldState)
				WriteDebugMessageLine("State has changed from " + oldState + " to " + state + ".");
			else
				WriteDebugMessageLine("State is already " + oldState + ".");
#endif
			if (withNotify)
			{
				// Notify asynchronously because the state will get changed from asynchronous items
				// such as the reopen timer. In case of that timer, the port needs to be locked to
				// ensure proper operation. In such case, a synchronous notification callback would
				// likely result in a deadlock, in case the callback sink would call any method or
				// property that also locks the port!

				OnIOChangedAsync(EventArgs.Empty);
				OnIOControlChangedAsync(EventArgs.Empty);
			}
		}

		#endregion

		#region Simple Port Methods
		//==========================================================================================
		// Simple Port Methods
		//==========================================================================================

		private void CreatePort()
		{
			lock (this.portSyncObj)
			{
				if (this.port != null)
					CloseAndDisposePort();

				this.port = new Ports.SerialPortEx();
				this.port.DataReceived  += new Ports.SerialDataReceivedEventHandler (port_DataReceived);
				this.port.PinChanged    += new Ports.SerialPinChangedEventHandler   (port_PinChanged);
				this.port.ErrorReceived += new Ports.SerialErrorReceivedEventHandler(port_ErrorReceived);
			}
		}

		private void OpenPort()
		{
			lock (this.portSyncObj)
			{
				if (this.port != null)
				{
					if (!this.port.IsOpen)
						this.port.Open();
				}
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		private void CloseAndDisposePort()
		{
			lock (this.portSyncObj)
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
		}

		#endregion

		#region Complex Port Methods
		//==========================================================================================
		// Complex Port Methods
		//==========================================================================================

		/// <summary></summary>
		private void CreateAndOpenPortAndThreadsAndNotify()
		{
			lock (this.portSyncObj) // Ensure that whole operation is performed at once!
			{
				CreatePort();       // Port must be created each time because this.port.Close()
				ApplySettings();    //   disposes the underlying IO instance

				// RFR (formerly RTS)
				switch (this.settings.Communication.FlowControl)
				{
					case SerialFlowControl.Hardware:
					case SerialFlowControl.Combined:
						// Do nothing, RFR is handled by the underlying serial port object.
						break;

					case SerialFlowControl.RS485:
						this.port.RfrEnable = false;
						break;

					case SerialFlowControl.ManualHardware:
					case SerialFlowControl.ManualCombined:
						this.port.RfrEnable = this.manualRfrWasEnabled;
						break;

					default:
						this.port.RfrEnable = false;
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

				StartThreads();
				StartAliveTimer();
				OpenPort();
				SetStateSynchronizedAndNotify(State.Opened);
			} // lock (this.portSyncObj)

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
					{
						bool wasXOn = false;
						lock (this.manualInputWasXOnSyncObj)
							wasXOn = this.manualInputWasXOn;

						if (wasXOn)
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

		/// <summary></summary>
		private void RestartOrResetPortAndThreadsAndNotify()
		{
			RestartOrResetPortAndThreadsAndNotify(true);
		}

		/// <summary></summary>
		private void RestartOrResetPortAndThreadsAndNotify(bool withNotify)
		{
			if (this.settings.AutoReopen.Enabled)
			{
				StopThreads();
				StopAndDisposeAliveTimer();
				CloseAndDisposePort();

				SetStateSynchronizedAndNotify(State.Closed, withNotify);

				StartReopenTimer();

				SetStateSynchronizedAndNotify(State.WaitingForReopen, withNotify);
			}
			else
			{
				ResetPortAndThreadsAndNotify();
			}
		}

		private void ResetPortAndThreadsAndNotify()
		{
			ResetPortAndThreadsAndNotify(true);
		}

		private void ResetPortAndThreadsAndNotify(bool withNotify)
		{
			StopThreads();
			StopAndDisposeAliveTimer();
			StopAndDisposeReopenTimer();
			CloseAndDisposePort();

			SetStateSynchronizedAndNotify(State.Reset, withNotify);
		}

		#endregion

		#region Port Threads
		//==========================================================================================
		// Port Threads
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
					this.sendThread.Name = ToShortPortString() + " Send Thread";
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
					this.receiveThread.Name = ToShortPortString() + " Receive Thread";
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
						Debug.Assert(this.sendThread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId, "Attention: Tried to join itself!");

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
						Debug.Assert(this.receiveThread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId, "Attention: Tried to join itself!");

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

		#region Port Events
		//==========================================================================================
		// Port Events
		//==========================================================================================

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that any exception leads to restart or reset of port.")]
		private void port_DataReceived(object sender, MKY.IO.Ports.SerialDataReceivedEventArgs e)
		{
			// If data has been received, but access to port throws exception, port has been shut
			// down, e.g. USB to serial converter was disconnected.
			try
			{
				if (IsOpen) // Ensure not to forward any events during closing anymore.
				{
					// Immediately read data on this thread.
					int bytesToRead;
					byte[] data;
					lock (this.portSyncObj)
					{
						bytesToRead = this.port.BytesToRead;
						data = new byte[bytesToRead];
						this.port.Read(data, 0, bytesToRead);
					}

					bool signalXOnXOff = false;
					bool signalXOnXOffCount = false;

					lock (this.receiveQueue)
					{
						foreach (byte b in data)
						{
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

									Interlocked.Increment(ref this.receivedXOnCount);
									signalXOnXOffCount = true;
								}
								else if (b == SerialPortSettings.XOffByte)
								{
									lock (this.outputIsXOnSyncObj)
									{
										if (BooleanEx.ClearIfSet(ref this.outputIsXOn))
											signalXOnXOff = true;
									}

									Interlocked.Increment(ref this.receivedXOffCount);
									signalXOnXOffCount = true;
								}
							}
						}
					}

					// Signal XOn/XOff change to send thread:
					if (signalXOnXOff)
						this.sendThreadEvent.Set();

					// Immediately invoke the event, but invoke it asynchronously!
					if (signalXOnXOff || signalXOnXOffCount)
						OnIOControlChangedAsync(EventArgs.Empty);

					// Signal receive thread:
					this.receiveThreadEvent.Set();
				}
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(GetType(), ex, "DataReceived() has detected shutdown of port.");
				RestartOrResetPortAndThreadsAndNotify();
			}
		}

		/// <summary>
		/// Asynchronously manage incoming events to prevent potential deadlocks if close/dispose
		/// was called from a ISynchronizeInvoke target (i.e. a form) on an event thread.
		/// Also, the mechanism implemented below reduces the amount of events that are propagated
		/// to the main application. Small chunks of received data will generate many events
		/// handled by <see cref="port_DataReceived"/>. However, since <see cref="OnDataReceived"/>
		/// synchronously invokes the event, it will take some time until the send queue is checked
		/// again. During this time, no more new events are invoked, instead, outgoing data is
		/// buffered.
		/// </summary>
		/// <remarks>
		/// Will be signaled by <see cref="port_DataReceived"/> event above, or by XOn/XOff while
		/// sending.
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
				// Measurements 2011-04-24 on an Intel Core 2 Duo running Win7 at 2.4 GHz and 3.5 GB of RAM:
				// > 0.0% CPU load in idle
				// > Up to an short-term-average of 20% CPU load while sending a large chuck of text
				//   (\YAT\!-SendFiles\Stress-2-Large.txt, 106 kB)
				// This is considered an acceptable CPU load.
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
		//       OnDataReceived(EventArgs.Empty);
		//   }
		// 
		// This suggestions doesn't work because YAT shall show every single byte as soon as it get's received.
		// If 3 bytes are received while 5 bytes are taken out of the receive queue, no more event gets fired.
		// Thus, the 3 bytes do not get shown until new data arrives. This is not acceptable.

		/// <summary>
		/// Asynchronously invoke incoming events to prevent potential deadlocks if close/dispose
		/// was called from a ISynchronizeInvoke target (i.e. a form) on an event thread.
		/// </summary>
		private delegate void port_PinChangedDelegate(object sender, MKY.IO.Ports.SerialPinChangedEventArgs e);

		private void port_PinChanged(object sender, MKY.IO.Ports.SerialPinChangedEventArgs e)
		{
			if (IsOpen) // Ensure not to forward any events during closing anymore.
			{
				port_PinChangedDelegate asyncInvoker = new port_PinChangedDelegate(port_PinChangedAsync);
				asyncInvoker.BeginInvoke(sender, e, null, null);
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		private void port_PinChangedAsync(object sender, MKY.IO.Ports.SerialPinChangedEventArgs e)
		{
			// If pin has changed, but access to port throws exception, port has been shut down,
			// e.g. USB to serial converter was disconnected.
			try
			{
				// Force access to port to check whether the port is still alive:
				bool cts = this.port.CtsHolding;
				UnusedLocal.PreventAnalysisWarning(cts);

				if (IsOpen) // Ensure not to forward any events during closing anymore.
				{
					if (this.settings.Communication.FlowControlManagesRfrCtsDtrDsrManually)
					{
						this.manualRfrWasEnabled = this.port.RfrEnable;
						this.manualDtrWasEnabled = this.port.DtrEnable;
					}

					// Signal pin change to threads:
					this.sendThreadEvent.Set();
					this.receiveThreadEvent.Set();

					switch (e.EventType)
					{
						case MKY.IO.Ports.SerialPinChange.InputBreak:
							if (this.settings.NoSendOnInputBreak)
								OnIOChanged(EventArgs.Empty);
							break;

						case MKY.IO.Ports.SerialPinChange.OutputBreak:
							OnIOChanged(EventArgs.Empty);
							break;

						default:
							// Do not fire general event, I/O control event is fired below.
							break;
					}
					OnIOControlChanged(EventArgs.Empty);
				}
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(GetType(), ex, "PinChangedAsync() has detected shutdown of port.");
				RestartOrResetPortAndThreadsAndNotify();
			}
		}

		/// <summary>
		/// Asynchronously invoke incoming events to prevent potential deadlocks if close/dispose
		/// was called from a ISynchronizeInvoke target (i.e. a form) on an event thread.
		/// </summary>
		private delegate void port_ErrorReceivedDelegate(object sender, MKY.IO.Ports.SerialErrorReceivedEventArgs e);

		private void port_ErrorReceived(object sender, MKY.IO.Ports.SerialErrorReceivedEventArgs e)
		{
			if (IsOpen) // Ensure not to forward any events during closing anymore.
			{
				port_ErrorReceivedDelegate asyncInvoker = new port_ErrorReceivedDelegate(port_ErrorReceivedAsync);
				asyncInvoker.BeginInvoke(sender, e, null, null);
			}
		}

		private void port_ErrorReceivedAsync(object sender, MKY.IO.Ports.SerialErrorReceivedEventArgs e)
		{
			ErrorSeverity severity = ErrorSeverity.Severe;
			Direction direction;
			string message;
			switch (e.EventType)
			{
				case System.IO.Ports.SerialError.Frame:    direction = Direction.Input;  message = "Serial port input framing error!";            break;
				case System.IO.Ports.SerialError.Overrun:  direction = Direction.Input;  message = "Serial port input character buffer overrun!"; break;
				case System.IO.Ports.SerialError.RXOver:   direction = Direction.Input;  message = "Serial port input buffer overflow!";          break;
				case System.IO.Ports.SerialError.RXParity: direction = Direction.Input;  message = "Serial port input parity error!";             break;
				case System.IO.Ports.SerialError.TXFull:   direction = Direction.Output; message = "Serial port output buffer full!";             break;
				default:   severity = ErrorSeverity.Fatal; direction = Direction.None;   message = "Unknown serial port error!";                  break;
			}
			OnIOError(new SerialPortErrorEventArgs(severity, direction, message, e.EventType));
		}

		#endregion

		#region Alive Timer
		//==========================================================================================
		// Alive Timer
		//==========================================================================================

		[SuppressMessage("Microsoft.Mobility", "CA1601:DoNotUseTimersThatPreventPowerStateChanges", Justification = "Well, any better idea on how to check whether the serial port is still alive?")]
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

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		private void aliveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			lock (this.portSyncObj) // Ensure that whole operation is performed at once!
			{
				if (!IsDisposed && IsStarted) // Check 'IsDisposed' first!
				{
					try
					{
						// If port isn't open anymore, or access to port throws exception,
						//   port has been shut down, e.g. USB to serial converter disconnected.
						if (!this.port.IsOpen)
						{
							WriteDebugMessageLine("AliveTimerElapsed() has detected shutdown of port.");
							RestartOrResetPortAndThreadsAndNotify();
						}
					}
					catch
					{
						WriteDebugMessageLine("AliveTimerElapsed() has detected shutdown of port.");
						RestartOrResetPortAndThreadsAndNotify();
					}
				}
				else
				{
					StopAndDisposeAliveTimer();
				}
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

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		private void reopenTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			lock (this.portSyncObj) // Ensure that whole operation is performed at once!
			{
				if (AutoReopenEnabledAndAllowed)
				{
					try
					{
						CreateAndOpenPortAndThreadsAndNotify(); // Try to reopen port.
						WriteDebugMessageLine("ReopenTimerElapsed() successfully reopened the port.");
					}
					catch
					{
						WriteDebugMessageLine("ReopenTimerElapsed() has failed to reopen the port.");
						RestartOrResetPortAndThreadsAndNotify(false); // Cleanup and restart. Suppress notifications.
					}
				}
				else
				{
					StopAndDisposeReopenTimer();
				}
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
		protected virtual void OnIOChangedAsync(EventArgs e)
		{
			EventHelper.FireAsync(IOChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOControlChanged(EventArgs e)
		{
			EventHelper.FireSync(IOControlChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOControlChangedAsync(EventArgs e)
		{
			EventHelper.FireAsync(IOControlChanged, this, e);
		}

		/// <summary></summary>
		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true)]
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
			return (ToShortPortString());
		}

		#region Object Members > Extensions
		//------------------------------------------------------------------------------------------
		// Object Members > Extensions
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual string ToShortPortString()
		{
			lock (this.portSyncObj)
			{
				if      (this.port != null)
					return (this.port.PortId);
				else if (this.settings != null)
					return (this.settings.PortId);
				else
					return (Undefined);
			}
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
					"[" + ToShortPortString() + "]",
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
