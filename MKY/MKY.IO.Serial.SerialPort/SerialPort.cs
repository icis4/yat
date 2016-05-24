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

	// Enable debugging of thread state:
////#define DEBUG_TRANSMISSION

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

		private const int SendQueueFixedCapacity      = 2048; // = default 'WriteBufferSize'
		private const int ReceiveQueueInitialCapacity = 4096; // = default 'ReadBufferSize'

		private const int ThreadWaitTimeout = 200;
		private const int AliveInterval     = 500;

		private const int IOControlChangedTimeout = 47; // Timeout is fixed to 47 ms (a prime number).
		private readonly long IOControlChangedTickInterval = StopwatchEx.TimeToTicks(IOControlChangedTimeout);

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
		private object portSyncObj = new object(); // Required as port will be disposed and recreated on open/close.

		/// <remarks>
		/// Async sending. The capacity is set large enough to reduce the number of resizing
		/// operations while adding elements.
		/// </remarks>
		private Queue<byte> sendQueue = new Queue<byte>(SendQueueFixedCapacity);

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
		/// Only used with <see cref="SerialFlowControl.ManualSoftware"/>
		/// and <see cref="SerialFlowControl.ManualCombined"/>.
		/// </remarks>
		private IXOnXOffHelper iXOnXOffHelper = new IXOnXOffHelper();

		/// <summary>
		/// Alive timer detects port disconnects, i.e. when a USB to serial converter is disconnected.
		/// </summary>
		private System.Timers.Timer aliveTicker;

		private System.Timers.Timer reopenTimeout;

		private object dataEventSyncObj = new object();

		private System.Timers.Timer ioControlEventTimeout;
		private long nextIOControlEventTickStamp; // Ticks as defined by 'Stopwatch'.
		private object nextIOControlEventTickStampSyncObj = new object();

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
					// In the 'normal' case, the items have already been disposed of in e.g. Stop().
					ResetPortAndThreadsAndNotify(false); // Suppress notifications during disposal!

					if (this.stateLock != null)
						this.stateLock.Dispose();
				}

				// Set state to disposed:
				this.stateLock = null;
				this.isDisposed = true;
			}
		}

		/// <remarks>
		/// Microsoft.Design rule CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable requests
		/// "Types that declare disposable members should also implement IDisposable. If the type
		///  does not own any unmanaged resources, do not implement a finalizer on it."
		/// 
		/// Well, true for best performance on finalizing. However, it's not easy to find missing
		/// calls to <see cref="Dispose()"/>. In order to detect such missing calls, the finalizer
		/// is kept, opposing rule CA1001, but getting debug messages indicating missing calls.
		/// 
		/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
		/// </remarks>
		~SerialPort()
		{
			Dispose(false);

			DebugMessage("The finalizer should have never been called! Ensure to call Dispose()!");
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
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.settings);
			}
		}

		/// <summary></summary>
		public virtual Ports.SerialPortId PortId
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.port != null)
					return (this.port.PortId);
				else if (this.settings != null)
					return (this.settings.PortId);
				else
					return (null);
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

				if (this.settings.Communication.FlowControlManagesXOnXOffManually) // Information not available for 'Software' or 'Combined'!
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

				if (this.settings.Communication.FlowControlManagesXOnXOffManually) // Information not available for 'Software' or 'Combined'!
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

				if (this.settings.Communication.FlowControlManagesXOnXOffManually) // Information not available for 'Software' or 'Combined'!
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

				if (this.settings.Communication.FlowControlManagesXOnXOffManually) // Information not available for 'Software' or 'Combined'!
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

				if (this.settings.Communication.FlowControlManagesXOnXOffManually) // Information not available for 'Software' or 'Combined'!
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

				if (this.settings.Communication.FlowControlManagesXOnXOffManually) // Information not available for 'Software' or 'Combined'!
					return (this.iXOnXOffHelper.ReceivedXOffCount);
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
				DebugMessage("Starting...");
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
				DebugMessage("Start() requested but state is " + GetStateSynchronized() + ".");
			}

			return (true); // Return 'true' in any case since port is open in the end.
		}

		/// <summary></summary>
		public virtual void Stop()
		{
			// AssertNotDisposed() is called by 'IsStarted' below.

			if (IsStarted)
			{
				DebugMessage("Stopping...");
				ResetPortAndThreadsAndNotify();
			}
			else
			{
				DebugMessage("Stop() requested but state is " + GetStateSynchronized() + ".");
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

			if (IsTransmissive)
			{
				DebugTransmissionMessage("Enqueuing " + data.Length.ToString(CultureInfo.InvariantCulture) + " byte(s) for sending...");
				foreach (byte b in data)
				{
					// Wait until there is space in the send queue:
					while (this.sendQueue.Count >= SendQueueFixedCapacity) // No lock required, just checking for full.
					{
						if (IsDisposed || !IsTransmissive) // Check 'IsDisposed' first!
							return (false);

						Thread.Sleep(TimeSpan.Zero); // Yield to other threads to allow dequeuing.
					}

					// There is space for at least one byte:
					lock (this.sendQueue) // Lock is required because Queue<T> is not synchronized.
					{
						this.sendQueue.Enqueue(b);
					}
				}
				DebugTransmissionMessage("...enqueuing done");

				// Signal data notification to send thread:
				SignalSendThreadSafely();

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
			Rate sendRate = new Rate(this.settings.MaxSendRate.Interval);

			bool isOutputBreakOldAndErrorHasBeenSignaled = false;
			bool isCtsInactiveOldAndErrorHasBeenSignaled = false;
			bool   isXOffStateOldAndErrorHasBeenSignaled = false;
			bool isUnspecifiedOldAndErrorHasBeenSignaled = false;

			DebugThreadStateMessage("SendThread() has started.");

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
					// Ensure not send and forward events during closing anymore. Check 'IsDisposed' first!
					// Note that 'IsOpen' is used instead of 'IsTransmissive' to allow handling break further below.
					while (!IsDisposed && this.sendThreadRunFlag && IsOpen && (this.sendQueue.Count > 0))
					{                                                      // No lock required, just checking for empty.
						// Initially, yield to other threads before starting to read the queue, since it is very
						// likely that more data is to be enqueued, thus resulting in larger chunks processed.
						// Subsequently, yield to other threads to allow processing the data.
						Thread.Sleep(TimeSpan.Zero);

						bool isWriteTimeout = false;
						bool isOutputBreak  = false;

						// Handle output break state:
						if (this.port.OutputBreak) // No lock required, not modifying anything.
						{
							if (!isOutputBreakOldAndErrorHasBeenSignaled)
							{
								InvokeOutputBreakErrorEvent();
								isOutputBreakOldAndErrorHasBeenSignaled = true;
							}

							break; // Let other threads do their job and wait until signaled again.
						}

						// Handle inactive CTS line:
						if (this.settings.Communication.FlowControlUsesRfrCts && !this.port.CtsHolding) // No lock required, not modifying anything.
						{
							if (!isCtsInactiveOldAndErrorHasBeenSignaled)
							{
								InvokeCtsInactiveErrorEvent();
								isCtsInactiveOldAndErrorHasBeenSignaled = true;
							}

							break; // Let other threads do their job and wait until signaled again.
						}

						// Handle XOff state:
						if (this.settings.Communication.FlowControlUsesXOnXOff && !OutputIsXOn)
						{
							// Attention, XOn/XOff handling is implemented in MKY.IO.Serial.Usb.SerialHidDevice too!
							// Changes here must most likely be applied there too.

							// Control bytes must be sent even in case of XOff! XOn has precedence over XOff.
							if (this.sendQueue.Contains(XOnXOff.XOnByte)) // No lock required, not modifying anything.
							{
								if (TryWriteXOnOrXOffAndNotify(XOnXOff.XOnByte, out isWriteTimeout, out isOutputBreak))
								{
									lock (this.sendQueue) // Lock is required because Queue<T> is not synchronized.
									{
										if (this.sendQueue.Peek() == XOnXOff.XOnByte) // If XOn is upfront...
											this.sendQueue.Dequeue();                 // ...acknowlege it's gone.
									}
									break; // Let other threads do their job and wait until signaled again.
								}
							}
							else if (this.sendQueue.Contains(XOnXOff.XOffByte)) // No lock required, not modifying anything.
							{
								if (TryWriteXOnOrXOffAndNotify(XOnXOff.XOffByte, out isWriteTimeout, out isOutputBreak))
								{
									lock (this.sendQueue) // Lock is required because Queue<T> is not synchronized.
									{
										if (this.sendQueue.Peek() == XOnXOff.XOffByte) // If XOff is upfront...
											this.sendQueue.Dequeue();                  // ...acknowlege it's gone.
									}
									break; // Let other threads do their job and wait until signaled again.
								}
							}
							else
							{
								if (!isXOffStateOldAndErrorHasBeenSignaled)
								{
									InvokeXOffErrorEvent();
									isXOffStateOldAndErrorHasBeenSignaled = true;
								}

								break; // Let other threads do their job and wait until signaled again.
							}
						}

						// --- No break, no inactive CTS, no XOff state, ready to send: ---

						if (!isWriteTimeout && !isOutputBreak)
						{
							// Synchronize the send/receive events to prevent mix-ups at the event
							// sinks, i.e. the send/receive operations shall be synchronized with
							// signaling of them.
							// But attention, do not simply lock() the 'dataSyncObj'. Instead, just
							// try to get the lock or try again later. The thread = direction that
							// get's the lock first, shall also be the one to signal first:

							if (Monitor.TryEnter(this.dataEventSyncObj))
							{
								try
								{
									// By default, stuff as much data as possible into output buffer:
									int maxChunkSize = (this.port.WriteBufferSize - this.port.BytesToWrite);

									// Reduce chunk size if maximum send rate is specified:
									if (this.settings.MaxSendRate.Enabled)
									{
										int remainingSizeInInterval = (this.settings.MaxSendRate.Size - sendRate.Value);
										maxChunkSize = Int32Ex.Limit(maxChunkSize, 0, remainingSizeInInterval);
									}

									// Further reduce chunk size if maximum is specified:
									if (this.settings.MaxChunkSize.Enabled)
									{
										int maxChunkSizeSetting = this.settings.MaxChunkSize.Size;
										maxChunkSize = Int32Ex.Limit(maxChunkSize, 0, maxChunkSizeSetting);
									}

									// Notes on sending:
									//
									// As soon as YAT started to write the maximum chunk size (in Q1/2016 ;-),
									// data got lost even for e.g. a local port loopback pair. All seems to work
									// fine as long as small chunks of e.g. 50 bytes some now an then are transmitted.
									//
									// Tried to use async reading (see port_DataReceived() for details), but there
									// seems to be no difference. Both methods loose the equal amount of data.
									//
									// Weirdly, data loss is much smaller when enabling 'DEBUG_TRANSMISSION',
									// which obviously slows down sending. It seems that the issue is not only
									// about receiving, but also about sending too fast!
									//
									// Suspecting that the issue is actually caused by limitations of the USB/COM
									// converter in use (Prolific)! This suspicion shall be verified using as described
									// in request #263 "Extend SerialPort driver analysis by sending and loopback".
									//
									// Again reducing the chunk size by default to work-around the issue.

									List<byte> effectiveChunkData;
									bool signalIOControlChanged;
									if (TryWriteChunkToPort(maxChunkSize, out effectiveChunkData, out isWriteTimeout, out isOutputBreak, out signalIOControlChanged))
									{
										DebugTransmissionMessage("Signaling " + effectiveChunkData.Count.ToString() + " byte(s) sent...");
										OnDataSent(new SerialDataSentEventArgs(effectiveChunkData.ToArray(), PortId));
										DebugTransmissionMessage("...signaling done");

										// Update the send rate with the effective chunk size:
										if (this.settings.MaxSendRate.Enabled)
											sendRate.Update(effectiveChunkData.Count);

										// Note the Thread.Sleep(TimeSpan.Zero) further above.
									}

									if (signalIOControlChanged)
									{
										OnIOControlChanged(EventArgs.Empty);
									}
								}
								finally
								{
									Monitor.Exit(this.dataEventSyncObj);
								}
							} // Monitor.TryEnter()
						}

						if (isWriteTimeout) // Timeout detected while trying to call System.IO.Ports.SerialPort.Write().
						{                   // May only be partial, some data may have been sent before timeout.
							if (this.settings.Communication.FlowControlUsesRfrCts && !this.port.CtsHolding)
							{
								if (!isCtsInactiveOldAndErrorHasBeenSignaled)
								{
									InvokeCtsInactiveErrorEvent();
									isCtsInactiveOldAndErrorHasBeenSignaled = true;
								}
							}
							else if (this.settings.Communication.FlowControlUsesXOnXOff && !OutputIsXOn)
							{
								if (!isXOffStateOldAndErrorHasBeenSignaled)
								{
									InvokeXOffErrorEvent();
									isXOffStateOldAndErrorHasBeenSignaled = true;
								}
							}
							else
							{
								if (!isUnspecifiedOldAndErrorHasBeenSignaled)
								{
									InvokeUnspecifiedErrorEvent();
									isUnspecifiedOldAndErrorHasBeenSignaled = true;
								}
							}
						}
						else
						{
							isCtsInactiveOldAndErrorHasBeenSignaled = false;
							  isXOffStateOldAndErrorHasBeenSignaled = false;
							isUnspecifiedOldAndErrorHasBeenSignaled = false;
						}

						if (isOutputBreak) // Output break detected *WHILE* trying to call System.IO.Ports.SerialPort.Write().
						{                  // May only be partial, some data may have been sent before break.
							if (!isOutputBreakOldAndErrorHasBeenSignaled)
							{
								InvokeOutputBreakErrorEvent();
								isOutputBreakOldAndErrorHasBeenSignaled = true;
							}
						}
						else
						{
							isOutputBreakOldAndErrorHasBeenSignaled = false;
						}
					} // while (dataAvailable)
				} // while (isRunning)
			}
			catch (IOException ex) // No other way to detect a disconnected device than forcing this exception...
			{
				DebugEx.WriteException(GetType(), ex, "SendThread() has detected shutdown of port.");
				RestartOrResetPortAndThreadsAndNotify();
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(GetType(), ex, "SendThread() has caught an unhandled exception!");
				RestartOrResetPortAndThreadsAndNotify();
			}

			DebugThreadStateMessage("SendThread() has terminated.");
		}

		private bool TryWriteXOnOrXOffAndNotify(byte b, out bool isWriteTimeout, out bool isOutputBreak)
		{
			bool signalIOControlChanged;

			if (TryWriteByteToPort(b, out isWriteTimeout, out isOutputBreak, out signalIOControlChanged))
			{
				OnDataSent(new SerialDataSentEventArgs(b, PortId)); // Skip I/O synchronization for simplicity.

				if (signalIOControlChanged)
					OnIOControlChanged(EventArgs.Empty); // Signal change of XOn/XOff state.

				return (true);
			}

			return (false);
		}

		/// <remarks>
		/// Attention, sending a whole chunk is implemented in <see cref="TryWriteChunkToPort"/> below.
		/// Changes here may have to be applied there too.
		/// </remarks>
		private bool TryWriteByteToPort(byte b, out bool isWriteTimeout, out bool isOutputBreak, out bool signalIOControlChanged)
		{
			isWriteTimeout         = false;
			isOutputBreak          = false;
			signalIOControlChanged = false;

			bool writeSuccess      = false;
			Exception unhandled    = null;

			if (this.settings.Communication.FlowControl == SerialFlowControl.RS485)
			{
				this.port.RfrEnable = true;
			}

			// Note the remark in SerialPort.Write():
			// "If there are too many bytes in the output buffer and 'Handshake' is set to
			//  'XOnXOff' then the 'SerialPort' object may raise a 'TimeoutException' while
			//  it waits for the device to be ready to accept more data."

			try
			{
				DebugTransmissionMessage("Writing 1 byte to port...");

				// Try to write the byte, will raise a 'TimeoutException' if not possible:
				byte[] a = { b };
				this.port.Write(a, 0, 1); // Do not lock, may take some time!
				writeSuccess = true;

				DebugTransmissionMessage("...writing done.");

				// Handle XOn/XOff if required:
				if (this.settings.Communication.FlowControlManagesXOnXOffManually) // Information not available for 'Software' or 'Combined'!
				{
					if (XOnXOff.IsXOnOrXOffByte(b))
					{
						this.iXOnXOffHelper.XOnOrXOffSent(b);
						signalIOControlChanged = true; // XOn/XOff count has changed.
					}
				}
			}
			catch (TimeoutException ex)
			{
				DebugEx.WriteException(GetType(), ex, "Timeout while writing to port!");
				isWriteTimeout = true;
			}
			catch (InvalidOperationException ex)
			{
				DebugEx.WriteException(GetType(), ex, "Invalid operation while writing to port!");
				isOutputBreak = true;
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(GetType(), ex, "Unspecified error while writing to port!");
				unhandled = ex;
			}

			if (this.settings.Communication.FlowControl == SerialFlowControl.RS485)
			{
				this.port.Flush(); // Make sure that data is sent before restoring RFR, including the underlying physical UART.
				Thread.Sleep((int)Math.Ceiling(this.settings.Communication.FrameLength * 1000)); // Single byte/frame.
				this.port.RfrEnable = false;
			}

			if (unhandled != null)
			{
				throw (unhandled); // Re-throw unhandled exception after restoring RFR.
			}

			return (writeSuccess);
		}

		/// <remarks>
		/// Attention, sending a single byte is implemented in <see cref="TryWriteByteToPort"/> above.
		/// Changes here may have to be applied there too.
		/// </remarks>
		private bool TryWriteChunkToPort(int maxChunkSize, out List<byte> effectiveChunkData, out bool isWriteTimeout, out bool isOutputBreak, out bool signalIOControlChanged)
		{
			isWriteTimeout         = false;
			isOutputBreak          = false;
			signalIOControlChanged = false;

			bool writeSuccess      = false;
			Exception unhandled    = null;

			if (this.settings.Communication.FlowControl == SerialFlowControl.RS485)
			{
				this.port.RfrEnable = true;
			}

			// Note the remark in SerialPort.Write():
			// "If there are too many bytes in the output buffer and 'Handshake' is set to
			//  'XOnXOff' then the 'SerialPort' object may raise a 'TimeoutException' while
			//  it waits for the device to be ready to accept more data."

			try
			{
				// Retrieve chunk from the send queue. Retrieve and send as a whole to improve speed.
				// If sending fails, the port is either blocked by XOff or CTS, or closed.

				byte[] a;
				lock (this.sendQueue) // Lock is required because Queue<T> is not synchronized.
				{
					a = this.sendQueue.ToArray();
				}

				int triedChunkSize = Math.Min(maxChunkSize, a.Length);
				effectiveChunkData = new List<byte>(triedChunkSize);

				DebugTransmissionMessage("Writing " + triedChunkSize + " byte(s) to port...");

				// Try to write the chunk, will raise a 'TimeoutException' if not possible:
				this.port.Write(a, 0, triedChunkSize); // Do not lock, may take some time!
				writeSuccess = true;

				DebugTransmissionMessage("...writing done.");

				// Finalize the write operation:
				lock (this.sendQueue) // Lock is required because Queue<T> is not synchronized.
				{
					for (int i = 0; i < triedChunkSize; i++)
					{
						byte b = this.sendQueue.Dequeue(); // Dequeue the chunk to acknowlege it's gone.

						effectiveChunkData.Add(b);

						// Handle XOn/XOff if required:
						if (this.settings.Communication.FlowControlManagesXOnXOffManually) // Information not available for 'Software' or 'Combined'!
						{
							if (XOnXOff.IsXOnOrXOffByte(b))
							{
								this.iXOnXOffHelper.XOnOrXOffSent(b);
								signalIOControlChanged = true; // XOn/XOff count has changed.
							}
						}
					}
				}

				// Ensure not to lock the queue while potentially pending in Write(), that would
				// result in a severe performance drop because enqueuing was no longer possible.
			}
			catch (TimeoutException ex)
			{
				DebugEx.WriteException(GetType(), ex, "Timeout while writing to port!");
				effectiveChunkData = null;
				isWriteTimeout = true;
			}
			catch (InvalidOperationException ex)
			{
				DebugEx.WriteException(GetType(), ex, "Invalid operation while writing to port!");
				effectiveChunkData = null;
				isOutputBreak = true;
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(GetType(), ex, "Unspecified error while writing to port!");
				effectiveChunkData = null;
				unhandled = ex;
			}

			if (this.settings.Communication.FlowControl == SerialFlowControl.RS485)
			{
				int maxFramesInFifo = 0;
				if (effectiveChunkData != null)
					maxFramesInFifo = Math.Min(effectiveChunkData.Count, 16); // Max 16 bytes/frames in FIFO.

				this.port.Flush(); // Make sure that data is sent before restoring RFR, including the underlying physical UART.
				Thread.Sleep((int)Math.Ceiling(this.settings.Communication.FrameLength * 1000 * maxFramesInFifo));
				this.port.RfrEnable = false;
			}

			if (unhandled != null)
			{
				throw (unhandled); // Re-throw unhandled exception after restoring RFR.
			}

			return (writeSuccess);
		}

		private void InvokeOutputBreakErrorEvent()
		{
			OnIOError(new IOErrorEventArgs
				(
					ErrorSeverity.Acceptable,
					Direction.Output,
					"Output break state, retaining data..."
				)
			);
		}

		private void InvokeCtsInactiveErrorEvent()
		{
			OnIOError(new IOErrorEventArgs
				(
					ErrorSeverity.Acceptable,
					Direction.Output,
					"CTS inactive, retaining data..."
				)
			);
		}

		private void InvokeXOffErrorEvent()
		{
			OnIOError(new IOErrorEventArgs
				(
					ErrorSeverity.Acceptable,
					Direction.Output,
					"XOff state, retaining data..."
				)
			);
		}

		private void InvokeUnspecifiedErrorEvent()
		{
			OnIOError(new IOErrorEventArgs
				(
					ErrorSeverity.Acceptable,
					Direction.Output,
					"Inactive, retaining data..."
				)
			);
		}

		// Attention, XOn/XOff handling is implemented in MKY.IO.Serial.Usb.SerialHidDevice too!
		// Changes here must most likely be applied there too.

		/// <summary></summary>
		protected virtual void AssumeOutputXOn()
		{
			this.iXOnXOffHelper.OutputIsXOn = true;

			OnIOControlChanged(EventArgs.Empty);
		}

		/// <summary>
		/// Signals the other communication endpoint that this device is in XOn state.
		/// </summary>
		public virtual void SignalInputXOn()
		{
			AssertNotDisposed();

			Send(XOnXOff.XOnByte);
		}

		/// <summary>
		/// Signals the other communication endpoint that this device is in XOff state.
		/// </summary>
		public virtual void SignalInputXOff()
		{
			AssertNotDisposed();

			Send(XOnXOff.XOffByte);
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
		/// Resets the XOn/XOff signaling count.
		/// </summary>
		public virtual void ResetXOnXOffCount()
		{
			AssertNotDisposed();

			this.iXOnXOffHelper.ResetCounts();

			OnIOControlChanged(EventArgs.Empty);
		}

		/// <summary>
		/// Resets the flow control signaling count.
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

				if (this.settings.OutputBufferSize.Enabled)
					this.port.WriteBufferSize = this.settings.OutputBufferSize.Size;

				SerialCommunicationSettings s = this.settings.Communication;
				this.port.BaudRate  = (MKY.IO.Ports.BaudRateEx)s.BaudRate;
				this.port.DataBits  = (MKY.IO.Ports.DataBitsEx)s.DataBits;
				this.port.Parity    = s.Parity;
				this.port.StopBits  = s.StopBits;
				this.port.Handshake = (SerialFlowControlEx)s.FlowControl;

				switch (s.RfrPin)
				{
					case SerialControlPinState.Automatic: /* Do not access the pin! */ break;
					case SerialControlPinState.Enabled:   this.port.RfrEnable = true;  break;
					case SerialControlPinState.Disabled:  this.port.RfrEnable = false; break;
					default: throw (new NotSupportedException("Program execution should never get here,'" + s.RfrPin.ToString() + "' is an unknown item." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}

				switch (s.DtrPin)
				{
					case SerialControlPinState.Automatic: /* Do not access the pin! */ break;
					case SerialControlPinState.Enabled:   this.port.DtrEnable = true;  break;
					case SerialControlPinState.Disabled:  this.port.DtrEnable = false; break;
					default: throw (new NotSupportedException("Program execution should never get here,'" + s.DtrPin.ToString() + "' is an unknown item." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
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

		private void SetStateSynchronizedAndNotify(State state, bool withNotify = true)
		{
#if (DEBUG)
			State oldState = GetStateSynchronized();
#endif
			this.stateLock.EnterWriteLock();
			this.state = state;
			this.stateLock.ExitWriteLock();

			if (withNotify)
			{
#if (DEBUG)
				if (this.state != oldState)
					DebugMessage("State has changed from " + oldState + " to " + state + ".");
				else
					DebugMessage("State is already " + oldState + ".");
#endif
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
				this.port.WriteTimeout = 50; // By default 'Timeout.Infinite', but that leads to
				// deadlock in case of disabled flow control! Win32 used to default to 500 ms, but
				// that sounds way too long. 1 ms doesn't look like a good idea either, since an
				// exception per ms won't help good performance... 50 ms seems to works fine, still
				// it's just a best guess...

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
				CreatePort();    // Port must be created each time because this.port.Close()
				ApplySettings(); //   disposes the underlying IO instance
				OpenPort();
			}

			StartThreads();
			StartAliveTicker();
			SetStateSynchronizedAndNotify(State.Opened); // Notify outside lock!

			// Handle initial XOn/XOff state:
			if (this.settings.Communication.FlowControlUsesXOnXOff)
			{
				AssumeOutputXOn();

				// Immediately send XOn if software flow control is enabled to ensure that
				//   device gets put into XOn if it was XOff before.
				switch (this.settings.Communication.FlowControl)
				{
					case SerialFlowControl.ManualSoftware:
					case SerialFlowControl.ManualCombined:
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

		private void RestartOrResetPortAndThreadsAndNotify(bool withNotify = true)
		{
			if (this.settings.AutoReopen.Enabled)
			{
				StopAndDisposeAliveTicker();
				StopAndDisposeControlEventTimeout();
				StopThreads();
				CloseAndDisposePort();

				SetStateSynchronizedAndNotify(State.Closed, withNotify); // Notification must succeed here, do not try/catch.

				StartReopenTimeout();

				SetStateSynchronizedAndNotify(State.WaitingForReopen, withNotify); // Notification must succeed here, do not try/catch.
			}
			else
			{
				ResetPortAndThreadsAndNotify();
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		private void ResetPortAndThreadsAndNotify(bool withNotify = true)
		{
			StopAndDisposeReopenTimeout();
			StopAndDisposeAliveTicker();
			StopAndDisposeControlEventTimeout();
			StopThreads();
			CloseAndDisposePort();

			try
			{
				SetStateSynchronizedAndNotify(State.Reset, withNotify);
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(GetType(), ex, "Exception while notifying!");
			}
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

		/// <remarks>
		/// Especially useful during potentially dangerous creation and disposal sequence.
		/// </remarks>
		private void SignalThreadsSafely()
		{
			SignalSendThreadSafely();
			SignalReceiveThreadSafely();
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

		private void StopThreads()
		{
			// First clear both flags to reduce the time to stop the receive thread, it may already
			// be signaled while receiving data or while the send thread is still running.
			lock (this.sendThreadSyncObj)
				this.sendThreadRunFlag = false;
			lock (this.receiveThreadSyncObj)
				this.receiveThreadRunFlag = false;

			lock (this.sendThreadSyncObj)
			{
				if (this.sendThread != null)
				{
					// Attention, this method may also be called from exception handler within SendThread()!
					if (this.sendThread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId)
					{
						DebugThreadStateMessage("SendThread() gets stopped...");

						try
						{
							int accumulatedTimeout = 0;
							int interval = 0; // Use a relatively short random interval to trigger the thread:
							while (!this.sendThread.Join(interval = staticRandom.Next(5, 20)))
							{
								SignalSendThreadSafely();

								accumulatedTimeout += interval;
								if (accumulatedTimeout >= ThreadWaitTimeout)
								{
									DebugThreadStateMessage("...failed! Aborting...");
									DebugThreadStateMessage("(Abort is likely required due to failed synchronization back the calling thread, which is typically the GUI/main thread.)");
									this.sendThread.Abort();
									break;
								}

								DebugThreadStateMessage("...trying to join at " + accumulatedTimeout + " ms...");
							}
						}
						catch (ThreadStateException)
						{
							// Ignore thread state exceptions such as "Thread has not been started" and
							// "Thread cannot be aborted" as it just needs to be ensured that the thread
							// has or will be terminated for sure.

							DebugThreadStateMessage("...failed too but will be exectued as soon as the calling thread gets suspended again.");
						}
						finally
						{
							this.sendThread = null;
						}
					} // Not itself thread.

					DebugThreadStateMessage("...successfully terminated.");
				}

				if (this.sendThreadEvent != null)
				{
					try     { this.sendThreadEvent.Close(); }
					finally { this.sendThreadEvent = null; }
				}
			}

			lock (this.receiveThreadSyncObj)
			{
				if (this.receiveThread != null)
				{
					// Attention, this method may also be called from exception handler within ReceiveThread()!
					if (this.receiveThread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId)
					{
						DebugThreadStateMessage("ReceiveThread() gets stopped...");

						try
						{
							int accumulatedTimeout = 0;
							int interval = 0; // Use a relatively short random interval to trigger the thread:
							while (!this.receiveThread.Join(interval = staticRandom.Next(5, 20)))
							{
								SignalReceiveThreadSafely();

								accumulatedTimeout += interval;
								if (accumulatedTimeout >= ThreadWaitTimeout)
								{
									DebugThreadStateMessage("...failed! Aborting...");
									DebugThreadStateMessage("(Abort is likely required due to failed synchronization back the calling thread, which is typically the GUI/main thread.)");
									this.receiveThread.Abort();
									break;
								}

								DebugThreadStateMessage("...trying to join at " + accumulatedTimeout + " ms...");
							}
						}
						catch (ThreadStateException)
						{
							// Ignore thread state exceptions such as "Thread has not been started" and
							// "Thread cannot be aborted" as it just needs to be ensured that the thread
							// has or will be terminated for sure.

							DebugThreadStateMessage("...failed too but will be exectued as soon as the calling thread gets suspended again.");
						}
						finally
						{
							this.receiveThread = null;
						}
					} // Not itself thread.

					DebugThreadStateMessage("...successfully terminated.");
				}

				if (this.receiveThreadEvent != null)
				{
					try     { this.receiveThreadEvent.Close(); }
					finally { this.receiveThreadEvent = null; }
				}
			}
		}

		#endregion

		#region Port Events
		//==========================================================================================
		// Port Events
		//==========================================================================================

		/// <remarks>
		/// As soon as YAT started to write the maximum chunk size (in Q1/2016 ;-), data got lost
		/// even for e.g. a local port loopback pair. All seems to work fine as long as small chunks
		/// of e.g. 50 bytes some now an then are transmitted. Tried to use async reading instead of
		/// the 'DataReceived' event as suggested by online resources like what Ben Voigt states at
		/// http://www.sparxeng.com/blog/software/must-use-net-system-io-ports-serialport.
		/// 
		/// However, there seems to be no difference whether 'DataReceived' and 'BytesToRead' or
		/// async reading is used. Both loose the equal amount of data, this fact is also supported
		/// be the 'DriverAnalysis'. Also, opposed to what Ben Voigt states, async reading actually
		/// results in smaller chunks, mostly 1 byte reads. Whereas the obvious 'DataReceived' and
		/// 'BytesToRead' mostly result in 1..4 byte reads, even up to 20..30 bytes.
		/// 
		/// Thus, this implementation again uses the 'normal' method. Data loss probably just has
		/// to be expected in case of the maximum rate...
		/// 
		/// Note that there are several solutions to this issue:
		///  - Use of flow control (SW or HW).
		///  - Reduction of the write buffer size (advanced settings).
		///  - Reduction of the write chunk size (advanced settings).
		/// 
		/// Weirdly, data loss is much smaller when enabling 'DEBUG_TRANSMISSION', which obviously
		/// slows down sending. It seems that the issue is not only about receiving, but also about
		/// sending too fast! See comments in <see cref="SendThread"/> for details on sending.
		/// 
		/// 
		/// Additional information on receiving
		/// -----------------------------------
		/// Another improvement suggested by Marco Stroppel on 2011-02-17 doesn't work with YAT.
		/// 
		/// Suggestion: The while(BytesAvailable > 0) fires endless events, because I did not call
		/// the Receive() method. That was, because I receive only the data when the other port to
		/// write the data is opened. So the BytesAvailable got never zero. My idea was (not knowing
		/// if this is good) to do something like:
		/// 
		/// while(BytesAvailable > LastTimeBytesAvailable)
		/// {
		///     LastTimeBytesAvailable = BytesAvailable;
		///     OnDataReceived(EventArgs.Empty);
		/// }
		/// 
		/// This suggestions doesn't work because YAT shall show every single byte as soon as it
		/// get's received. If 3 bytes are received while 5 bytes are taken out of the receive
		/// queue, no more event gets fired. Thus, the 3 bytes do not get shown until new data
		/// arrives. This is not acceptable.
		/// </remarks>
		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true, Rationale = "SerialPort.DataReceived: Only one event handler can execute at a time.")]
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that any exception leads to restart or reset of port.")]
		private void port_DataReceived(object sender, MKY.IO.Ports.SerialDataReceivedEventArgs e)
		{
			// If data has been received, but access to port throws exception, port has been shut
			// down, e.g. a USB to serial converter was disconnected.
			try
			{
				if (!IsDisposed && IsOpen) // Ensure not to perform any operations during closing anymore. Check 'IsDisposed' first!
				{
					// Immediately read data on this thread:
					int bytesToRead;
					byte[] data;
					lock (this.portSyncObj)
					{
						bytesToRead = this.port.BytesToRead;
						data = new byte[bytesToRead];
						this.port.Read(data, 0, bytesToRead);
					}

					// Attention, XOn/XOff handling is implemented in MKY.IO.Serial.Usb.SerialHidDevice too!
					// Changes here must most likely be applied there too.

					bool signalXOnXOff = false;
					bool signalXOnXOffCount = false;

					lock (this.receiveQueue) // Lock is required because Queue<T> is not synchronized.
					{
						DebugTransmissionMessage("Enqueuing " + data.Length.ToString(CultureInfo.InvariantCulture) + " byte(s) for receiving...");
						foreach (byte b in data)
						{
							this.receiveQueue.Enqueue(b);

							// Handle output XOn/XOff state:
							if (this.settings.Communication.FlowControlManagesXOnXOffManually) // Information not available for 'Software' or 'Combined'!
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
						DebugTransmissionMessage("...enqueuing done");
					} // lock (this.receiveQueue)

					// Signal XOn/XOff change to send thread:
					if (signalXOnXOff)
						SignalSendThreadSafely();

					// Signal data notification to receive thread:
					SignalReceiveThreadSafely();

					// Immediately invoke the event, but invoke it asynchronously and NOT on this thread!
					if (signalXOnXOff || signalXOnXOffCount)
						OnIOControlChangedAsync(EventArgs.Empty);
				}
			}
			catch (IOException ex) // No other way to detect a disconnected device than forcing this exception...
			{
				DebugEx.WriteException(GetType(), ex, "Disconnect detected while reading from port.");
				RestartOrResetPortAndThreadsAndNotify();
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(GetType(), ex, "Error while reading from port.");
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
			DebugThreadStateMessage("ReceiveThread() has started.");

			// Outer loop, processes data after a signal was received:
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

				// Inner loop, runs as long as there is data in the receive queue.
				// Ensure not to forward events during disposing anymore. Check 'IsDisposed' first!
				while (!IsDisposed && this.receiveThreadRunFlag && (this.receiveQueue.Count > 0))
				{                                               // No lock required, just checking for empty.
					// Initially, yield to other threads before starting to read the queue, since it is very
					// likely that more data is to be enqueued, thus resulting in larger chunks processed.
					// Subsequently, yield to other threads to allow processing the data.
					Thread.Sleep(TimeSpan.Zero);

					if (Monitor.TryEnter(this.dataEventSyncObj))
					{
						try
						{
							byte[] data;
							lock (this.receiveQueue) // Lock is required because Queue<T> is not synchronized.
							{
								data = this.receiveQueue.ToArray();
								this.receiveQueue.Clear();
							}

							DebugTransmissionMessage("Signaling " + data.Length.ToString() + " byte(s) received...");
							OnDataReceived(new SerialDataReceivedEventArgs(data, PortId));
							DebugTransmissionMessage("...signaling done");

							// Note the Thread.Sleep(TimeSpan.Zero) above.
						}
						finally
						{
							Monitor.Exit(this.dataEventSyncObj);
						}
					} // Monitor.TryEnter()
				} // Inner loop
			} // Outer loop

			DebugThreadStateMessage("ReceiveThread() has terminated.");
		}

		/// <remarks>
		/// Asynchronously invoke incoming events to prevent potential deadlocks if close/dispose
		/// was called from a ISynchronizeInvoke target (i.e. a form) on an event thread.
		/// </remarks>
		private delegate void port_PinChangedDelegate(object sender, MKY.IO.Ports.SerialPinChangedEventArgs e);

		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true, Rationale = "SerialPort.PinChanged: Only one event handler can execute at a time.")]
		private void port_PinChanged(object sender, MKY.IO.Ports.SerialPinChangedEventArgs e)
		{
			// If pin has changed, but access to port throws exception, port has been shut down,
			// e.g. USB to serial converter was disconnected.
			try
			{
				if (!IsDisposed && IsOpen) // Ensure not to forward events during closing anymore.
				{
					// Signal pin change to threads:
					SignalThreadsSafely();

					// Force access to port to check whether the port is still alive:
					int byteToReadDummy = this.port.BytesToRead; // Force e.g. 'IOException', details see alive timer.
					UnusedLocal.PreventAnalysisWarning(byteToReadDummy);

					// Invoke events:
					switch (e.EventType)
					{
						case MKY.IO.Ports.SerialPinChange.InputBreak:
							if (this.settings.NoSendOnInputBreak)
								OnIOChangedAsync(EventArgs.Empty); // Async! See remarks above.
							break;

						case MKY.IO.Ports.SerialPinChange.OutputBreak:
							OnIOChangedAsync(EventArgs.Empty); // Async! See remarks above.
							break;

						default:
							// Do not fire general 'IOChanged' event.
							break;
					}

					long nextTickStamp;
					lock (this.nextIOControlEventTickStampSyncObj)
						nextTickStamp = this.nextIOControlEventTickStamp;

					if (Stopwatch.GetTimestamp() >= nextTickStamp)
					{
						StopAndDisposeControlEventTimeout();
						OnIOControlChangedAsync(EventArgs.Empty); // Async! See remarks above.
					}
					else
					{
						StartControlEventTimeout();
					}

					// Note that the number of events must be limited because certain internal serial
					// COM ports invoke superfluous control pin events. This issue has been reported
					// by UFi/CMe and confirmed by MHe as:
					//  > #271 "Loopback on internal serial interface"
					//  > #277 "Blocking application with internal serial interface"
				}
			}
			catch (IOException ex) // No other way to detect a disconnected device than forcing this exception...
			{
				DebugEx.WriteException(GetType(), ex, "PinChanged() has detected shutdown of port as it is no longer accessible.");
				RestartOrResetPortAndThreadsAndNotify();
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(GetType(), ex, "PinChanged() has caught an unhandled exception!");
				RestartOrResetPortAndThreadsAndNotify();
			}
		}

		private void StartControlEventTimeout()
		{
			if (this.ioControlEventTimeout == null)
			{
				this.ioControlEventTimeout = new System.Timers.Timer(IOControlChangedTimeout * 2); // Synchronous event shall have precedence over timeout.
				this.ioControlEventTimeout.AutoReset = false;
				this.ioControlEventTimeout.Elapsed += new System.Timers.ElapsedEventHandler(this.ioControlEventTimeout_Elapsed);
			}
			this.ioControlEventTimeout.Start();
		}

		private void StopAndDisposeControlEventTimeout()
		{
			if (this.ioControlEventTimeout != null)
			{
				this.ioControlEventTimeout.Stop();
				this.ioControlEventTimeout.Dispose();
				this.ioControlEventTimeout = null;
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		private void ioControlEventTimeout_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			try
			{
				OnIOControlChanged(EventArgs.Empty);
			}
			catch (Exception ex) // Handle any exception, port could e.g. got closed in the meantime.
			{
				DebugEx.WriteException(GetType(), ex, "Exception while invoking 'OnIOControlChanged' event after timeout!");
			}
		}

		/// <summary>
		/// Asynchronously invoke incoming events to prevent potential deadlocks if close/dispose
		/// was called from a ISynchronizeInvoke target (i.e. a form) on an event thread.
		/// </summary>
		private delegate void port_ErrorReceivedDelegate(object sender, MKY.IO.Ports.SerialErrorReceivedEventArgs e);

		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true, Rationale = "SerialPort.ErrorReceived: Only one event handler can execute at a time.")]
		private void port_ErrorReceived(object sender, MKY.IO.Ports.SerialErrorReceivedEventArgs e)
		{
			if (!IsDisposed && IsOpen) // Ensure not to forward events during closing anymore.
			{
				port_ErrorReceivedDelegate asyncInvoker = new port_ErrorReceivedDelegate(port_ErrorReceivedAsync);
				asyncInvoker.BeginInvoke(sender, e, null, null);
			}
		}

		private void port_ErrorReceivedAsync(object sender, MKY.IO.Ports.SerialErrorReceivedEventArgs e)
		{
			if (!IsDisposed && IsOpen) // Ensure not to forward events during closing anymore.
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
		}

		#endregion

		#region Alive Ticker
		//==========================================================================================
		// Alive Ticker
		//==========================================================================================

		[SuppressMessage("Microsoft.Mobility", "CA1601:DoNotUseTimersThatPreventPowerStateChanges", Justification = "Well, any better idea on how to check whether the serial port is still alive?")]
		private void StartAliveTicker()
		{
			if (this.aliveTicker == null)
			{
				this.aliveTicker = new System.Timers.Timer(AliveInterval);
				this.aliveTicker.AutoReset = true;
				this.aliveTicker.Elapsed += new System.Timers.ElapsedEventHandler(this.aliveTicker_Elapsed);
				this.aliveTicker.Start();
			}
		}

		private void StopAndDisposeAliveTicker()
		{
			if (this.aliveTicker != null)
			{
				this.aliveTicker.Stop();
				this.aliveTicker.Dispose();
				this.aliveTicker = null;
			}
		}

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private object aliveTicker_Elapsed_SyncObj = new object();

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		private void aliveTicker_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			// Ensure that only one timer elapsed event thread is active at a time. Because if the
			// execution takes longer than the timer interval, more and more timer threads will pend
			// here, and then be executed after the previous has been executed. This will require
			// more and more resources and lead to a drop in performance.
			if (Monitor.TryEnter(aliveTicker_Elapsed_SyncObj))
			{
				try
				{
					if (!IsDisposed && IsStarted) // Check 'IsDisposed' first!
					{
						try
						{
							// Detect whether port has been shut down, e.g. USB-to-serial-converter
							// device has been disconnected.
							// This is not that straight-forward to achieve, as different devices
							// or their driver behave differently. Used to only check for 'IsOpen',
							// but that is not sufficient for e.g. Prolific driver which will still
							// indicate 'IsOpen' even when device has long been disconnected.
							// With device disconnected, debugger showns exceptions for...
							// ...BytesToRead/BytesToWrite, or...
							// ...CtsHolding/DsrHolding/CDHolding properties.
							// Hardware pin state may again depend on device or driver, thus using
							// one of the pure software properties.

							if (!this.port.IsOpen)
							{
								DebugMessage("AliveTimerElapsed() has detected shutdown of port as it is no longer open.");
								RestartOrResetPortAndThreadsAndNotify();
							}

							int byteToReadDummy = this.port.BytesToRead; // Force e.g. 'IOException', see above.
							UnusedLocal.PreventAnalysisWarning(byteToReadDummy);
						}
						catch (IOException ex) // No other way to detect a disconnected device than forcing this exception...
						{
							DebugEx.WriteException(GetType(), ex, "AliveTimerElapsed() has detected shutdown of port as it is no longer accessible.");
							RestartOrResetPortAndThreadsAndNotify();
						}
						catch (Exception ex)
						{
							DebugEx.WriteException(GetType(), ex, "AliveTimerElapsed() has caught an unhandled exception!");
							RestartOrResetPortAndThreadsAndNotify();
						}
					}
					else
					{
						StopAndDisposeAliveTicker();
					}
				}
				finally
				{
					Monitor.Exit(aliveTicker_Elapsed_SyncObj);
				}
			} // Monitor.TryEnter()
		}

		#endregion

		#region Reopen Timeout
		//==========================================================================================
		// Reopen Timeout
		//==========================================================================================

		private void StartReopenTimeout()
		{
			if (this.reopenTimeout == null)
			{
				this.reopenTimeout = new System.Timers.Timer(this.settings.AutoReopen.Interval);
				this.reopenTimeout.AutoReset = false;
				this.reopenTimeout.Elapsed += new System.Timers.ElapsedEventHandler(this.reopenTimeout_Elapsed);
			}
			this.reopenTimeout.Start();
		}

		private void StopAndDisposeReopenTimeout()
		{
			if (this.reopenTimeout != null)
			{
				this.reopenTimeout.Stop();
				this.reopenTimeout.Dispose();
				this.reopenTimeout = null;
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		private void reopenTimeout_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (AutoReopenEnabledAndAllowed)
			{
				try
				{
					CreateAndOpenPortAndThreadsAndNotify(); // Try to reopen port.
					DebugMessage("ReopenTimerElapsed() successfully reopened the port.");
				}
				catch
				{
					DebugMessage("ReopenTimerElapsed() has failed to reopen the port.");
					RestartOrResetPortAndThreadsAndNotify(false); // Cleanup and restart. No notifications.
				}
			}
			else
			{
				StopAndDisposeReopenTimeout();
			}
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		/// <summary></summary>
		[CallingContract(IsNeverMainThread = true)]
		protected virtual void OnIOChanged(EventArgs e)
		{
			EventHelper.FireSync(IOChanged, this, e);
		}

		/// <summary></summary>
		[CallingContract(IsNeverMainThread = true)]
		protected virtual void OnIOChangedAsync(EventArgs e)
		{
			EventHelper.FireAsync(IOChanged, this, e);
		}

		/// <summary></summary>
		[CallingContract(IsNeverMainThread = true)]
		protected virtual void OnIOControlChanged(EventArgs e)
		{
			EventHelper.FireSync(IOControlChanged, this, e);

			SetNextControlChangedTickStamp();
		}

		/// <summary></summary>
		[CallingContract(IsNeverMainThread = true)]
		protected virtual void OnIOControlChangedAsync(EventArgs e)
		{
			EventHelper.FireAsync(IOControlChanged, this, e);

			SetNextControlChangedTickStamp();
		}

		private void SetNextControlChangedTickStamp()
		{
			lock (this.nextIOControlEventTickStampSyncObj)
				this.nextIOControlEventTickStamp = (Stopwatch.GetTimestamp() + IOControlChangedTickInterval);
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
			Ports.SerialPortId id = PortId;
			if (id != null)
				return (id.ToShortString());
			else
				return (Undefined);
		}

		#endregion

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
		private void DebugThreadStateMessage(string message)
		{
			DebugMessage(message);
		}

		[Conditional("DEBUG_TRANSMISSION")]
		private void DebugTransmissionMessage(string message)
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
