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

	// Enable debugging of thread state (send and receive threads):
////#define DEBUG_THREAD_STATE

	// Enable debugging of send request:
////#define DEBUG_SEND_REQUEST

	// Enable debugging of data transmission to/from port:
////#define DEBUG_TRANSMISSION

	// Enable debugging of receive request:
////#define DEBUG_RECEIVE_REQUEST

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
	/// <summary>
	/// Implements the <see cref="IIOProvider"/> interface for serial COM ports.
	/// </summary>
	/// <remarks>
	/// In addition, this class...
	/// <list type="bullet">
	/// <item><description>...wraps <see cref="Ports.SerialPortEx"/> with send/receive FIFOs.</description></item>
	/// <item><description>...adds advanced connection management (alive ticker, reopen timer).</description></item>
	/// <item><description>...adds advanced flow control management (manual flow control).</description></item>
	/// <item><description>...adds advanced break management.</description></item>
	/// </list>
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop isn't able to skip URLs...")]
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Different root namespace.")]
	public class SerialPort : IIOProvider, IXOnXOffHandler, IDisposable, IDisposableEx
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
			Error
		}

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const string Undefined = "<Undefined>";

		private const int SendQueueFixedCapacity      = 2048; // = default 'WriteBufferSize'
		private const int ReceiveQueueInitialCapacity = 4096; // = default 'ReadBufferSize'

		private const int ThreadWaitTimeout = 500; // Enough time to let the threads join...
		private const int AliveInterval     = 500;

		private const int IOControlChangedTimeout = 47; // Timeout is fixed to 47 ms (a prime number).

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "This is a 'readonly', thus meant to be constant.")]
		private readonly long IOControlChangedTickInterval = StopwatchEx.TimeToTicks(IOControlChangedTimeout);

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
		private EventHelper.Item eventHelper = EventHelper.CreateItem(typeof(SerialPort).FullName);

		private State state = State.Reset;
		private object stateSyncObj = new object(); // Required as port will be disposed and recreated on open/close.

		private SerialPortSettings settings;

		private MKY.IO.Ports.ISerialPort port;
		private object portSyncObj = new object(); // Required as port will be disposed and recreated on open/close.

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
		/// Only used with <see cref="SerialFlowControl.ManualSoftware"/>
		/// and <see cref="SerialFlowControl.ManualCombined"/>.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Emphasize the existance of the interface in use.")]
		private IXOnXOffHelper iXOnXOffHelper = new IXOnXOffHelper();

		/// <summary>
		/// Alive timer detects port disconnects, i.e. when a USB to serial converter is disconnected.
		/// </summary>
		private System.Timers.Timer aliveMonitor;
		private object aliveMonitorSyncObj = new object();

		private System.Timers.Timer reopenTimeout;
		private object reopenTimeoutSyncObj = new object();

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

		/// <summary>
		/// Initializes a new instance of the <see cref="SerialPort"/> class with
		/// default values of the <see cref="SerialPortSettings"/>.
		/// </summary>
		public SerialPort()
			: this(new SerialPortSettings())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SerialPort"/> class with
		/// the given <see cref="SerialPortSettings"/>.
		/// </summary>
		public SerialPort(SerialPortSettings settings)
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
		[SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "port", Justification = "Is actually disposed of asynchronously in ResetPortAndThreadsAndNotify().")]
		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				DebugEventManagement.DebugWriteAllEventRemains(this);

				this.eventHelper.DiscardAllEventsAndExceptions();

				// Dispose of managed resources if requested:
				if (disposing)
				{
					// In the 'normal' case, the items have already been disposed of in e.g. Stop().
					ResetPortAndThreadsWithoutNotify(false); // Suppress notifications during disposal!
				}

				// Set state to disposed:
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
		~SerialPort()
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
		public virtual SerialPortSettings Settings
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.settings);
			}
			set
			{
				// AssertNotDisposed() is called by 'IsStopped' below.

				if (IsStopped)
				{
					if (value != null)
						this.settings = value;
					else
						throw (new ArgumentNullException("value", "Settings cannot be changed to 'null'!"));
				}
				else
				{
					throw (new NotSupportedException("Settings cannot be changed while the port is open!"));
				}
			}
		}

		/// <summary></summary>
		public virtual MKY.IO.Ports.SerialPortId PortId
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

		/// <summary></summary>
		public virtual bool IsOpen
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				// Do not lock() infinitly in a simple get-property.

				if (Monitor.TryEnter(this.portSyncObj, ThreadWaitTimeout))
				{
					try
					{
						if (this.port != null)
							return (this.port.IsOpen);
						else
							return (false);
					}
					finally
					{
						Monitor.Exit(this.portSyncObj);
					}
				}
				else
				{
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

				// Do not lock() infinitly in a simple get-property.

				if (Monitor.TryEnter(this.portSyncObj, ThreadWaitTimeout))
				{
					try
					{
						if (IsOpen)
							return (!this.port.OutputBreak && !this.port.InputBreak);
						else
							return (false);
					}
					finally
					{
						Monitor.Exit(this.portSyncObj);
					}
				}
				else
				{
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

				// Do not lock() infinitly in a simple get-property.

				if (Monitor.TryEnter(this.portSyncObj, ThreadWaitTimeout))
				{
					try
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
					finally
					{
						Monitor.Exit(this.portSyncObj);
					}
				}
				else
				{
					return (false);
				}
			}
		}

		/// <summary>
		/// Serial port control pins.
		/// </summary>
		public virtual MKY.IO.Ports.SerialPortControlPins ControlPins
		{
			get
			{
				AssertNotDisposed();

				lock (this.portSyncObj)
				{
					if (this.port != null)
						return (this.port.ControlPins);
					else
						return (new MKY.IO.Ports.SerialPortControlPins());
				}
			}
		}

		/// <summary>
		/// Serial port control pin counts.
		/// </summary>
		public virtual MKY.IO.Ports.SerialPortControlPinCount ControlPinCount
		{
			get
			{
				AssertNotDisposed();

				lock (this.portSyncObj)
				{
					if (this.port != null)
						return (this.port.ControlPinCount);
					else
						return (new MKY.IO.Ports.SerialPortControlPinCount());
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

				if (this.settings.Communication.FlowControlManagesXOnXOffManually) // XOn/XOff information is not available for 'Software' or 'Combined'!
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

				if (this.settings.Communication.FlowControlManagesXOnXOffManually) // XOn/XOff information is not available for 'Software' or 'Combined'!
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

				if (this.settings.Communication.FlowControlManagesXOnXOffManually) // XOn/XOff information is not available for 'Software' or 'Combined'!
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

				if (this.settings.Communication.FlowControlManagesXOnXOffManually) // XOn/XOff information is not available for 'Software' or 'Combined'!
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

				if (this.settings.Communication.FlowControlManagesXOnXOffManually) // XOn/XOff information is not available for 'Software' or 'Combined'!
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

				if (this.settings.Communication.FlowControlManagesXOnXOffManually) // XOn/XOff information is not available for 'Software' or 'Combined'!
					return (this.iXOnXOffHelper.ReceivedXOffCount);
				else
					return (0);
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
			// AssertNotDisposed() is called by 'IsStopped' below.

			if (IsStopped)
			{
				DebugMessage("Starting...");
				try
				{
					CreateAndOpenPortAndThreadsAndNotify();
					return (true); // Return 'true' whether port open or not, since port is started in any case.
				}
				catch
				{
					ResetPortAndThreadsAndNotify(true);
					throw; // Rethrow!
				}
			}
			else
			{
				DebugMessage("Start() requested but state is already " + GetStateSynchronized() + ".");
				return (true); // Return 'true' since port is already open.
			}
		}

		/// <summary></summary>
		public virtual void Stop()
		{
			// AssertNotDisposed() is called by 'IsStarted' below.

			if (IsStarted)
			{
				DebugMessage("Stopping...");
				ResetPortAndThreadsAndNotify(false);
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
				DebugSendRequest("Enqueuing " + data.Length.ToString(CultureInfo.CurrentCulture) + " byte(s) for sending...");
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
				DebugSendRequest("...enqueuing done");

				// Signal data notification to send thread:
				SignalSendThreadSafely();

				return (true);
			}
			else
			{
				return (false);
			}
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

				this.port.IgnoreFramingErrors = this.settings.IgnoreFramingErrors;

				if (this.settings.OutputBufferSize.Enabled)
					this.port.WriteBufferSize = this.settings.OutputBufferSize.Size;

				SerialCommunicationSettings scs = this.settings.Communication;
				this.port.BaudRate  = (MKY.IO.Ports.BaudRateEx)scs.BaudRate;
				this.port.DataBits  = (MKY.IO.Ports.DataBitsEx)scs.DataBits;
				this.port.Parity    = scs.Parity;
				this.port.StopBits  = scs.StopBits;
				this.port.Handshake = (SerialFlowControlEx)scs.FlowControl;

				switch (scs.RtsPin)
				{
					case SerialControlPinState.Automatic: /* Do not access the pin! */ break;
					case SerialControlPinState.Enabled:   this.port.RtsEnable = true;  break;
					case SerialControlPinState.Disabled:  this.port.RtsEnable = false; break;

					default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + scs.RtsPin.ToString() + "' is an item that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}

				switch (scs.DtrPin)
				{
					case SerialControlPinState.Automatic: /* Do not access the pin! */ break;
					case SerialControlPinState.Enabled:   this.port.DtrEnable = true;  break;
					case SerialControlPinState.Disabled:  this.port.DtrEnable = false; break;

					default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + scs.DtrPin.ToString() + "' is an item that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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
			lock (this.stateSyncObj)
				return (this.state);
		}

		private void SetStateSynchronizedAndNotify(State state, bool withNotify = true)
		{
		#if (DEBUG)
			State oldState = GetStateSynchronized();
		#endif
			lock (this.stateSyncObj)
				this.state = state;

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
					CloseAndDisposePort(false);

				this.port = new MKY.IO.Ports.SerialPortEx();
				this.port.WriteTimeout = 50; // By default 'Timeout.Infinite', but that leads to
				// deadlock in case of disabled flow control! Win32 used to default to 500 ms, but
				// that sounds way too long. 1 ms doesn't look like a good idea either, since an
				// exception per ms won't help good performance... 50 ms seems to works fine, still
				// it's just a best guess...

				this.port.DataReceived  += new MKY.IO.Ports.SerialDataReceivedEventHandler (port_DataReceived);
				this.port.PinChanged    += new MKY.IO.Ports.SerialPinChangedEventHandler   (port_PinChanged);
				this.port.ErrorReceived += new MKY.IO.Ports.SerialErrorReceivedEventHandler(port_ErrorReceived);
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

		private void CloseAndDisposePort(bool isAfterException)
		{
			lock (this.portSyncObj)
			{
				if (this.port != null)
				{
					try
					{
						if (isAfterException)
							this.port.CloseAfterException();
						else if (this.port.IsOpen)
							this.port.CloseNormally();
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

			if (this.settings.AliveMonitor.Enabled)
				StartAliveMonitor();

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

		private void RestartOrResetPortAndThreadsAfterExceptionAndNotify()
		{
			DoRestartOrResetPortAndThreadsAfterException(true);
		}

		private void RestartOrResetPortAndThreadsAfterExceptionWithoutNotify()
		{
			DoRestartOrResetPortAndThreadsAfterException(false);
		}

		private void DoRestartOrResetPortAndThreadsAfterException(bool withNotify)
		{
			if (this.settings.AutoReopen.Enabled)
			{
				StopAndDisposeReopenTimeout();
				StopAndDisposeAliveMonitor();
				StopAndDisposeControlEventTimeout();
				StopThreads();

				// Used to invoke the code below via an async worker, in order to prevent deadlocks on
				// closing, working around the issue described in MKY.IO.Ports.SerialPort.SerialPortEx.
				// This worked well when closing a port during execution of the application, but again
				// lead to 'ObjectDisposedException' when exiting the application, each time! The async
				// implementation was added in SVN revision #1063 and again removed in #1101.

				CloseAndDisposePort(true); // This method is always called 'AfterException'.
				ClearQueues();

				SetStateSynchronizedAndNotify(State.Closed, withNotify); // Notification must succeed here, do not try/catch.

				StartReopenTimeout();

				SetStateSynchronizedAndNotify(State.WaitingForReopen, withNotify); // Notification must succeed here, do not try/catch.
			}
			else
			{
				ResetPortAndThreadsAndNotify(true); // This method is always called 'AfterException'.
			}
		}

		private void ResetPortAndThreadsAndNotify(bool isAfterException)
		{
			DoResetPortAndThreads(isAfterException, true);
		}

		private void ResetPortAndThreadsWithoutNotify(bool isAfterException)
		{
			DoResetPortAndThreads(isAfterException, false);
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		private void DoResetPortAndThreads(bool isAfterException, bool withNotify)
		{
			StopAndDisposeReopenTimeout();
			StopAndDisposeAliveMonitor();
			StopAndDisposeControlEventTimeout();
			StopThreads();

			// Used to invoke the code below via an async worker, in order to prevent deadlocks on
			// closing, working around the issue described in MKY.IO.Ports.SerialPort.SerialPortEx.
			// This worked well when closing a port during execution of the application, but again
			// lead to 'ObjectDisposedException' when exiting the application, each time! The async
			// implementation was added in SVN revision #1063 and again removed in #1101.

			CloseAndDisposePort(isAfterException);
			ClearQueues();

			try
			{
				SetStateSynchronizedAndNotify(State.Reset, withNotify);
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(GetType(), ex, "Exception while notifying!");
			}
		}

		private void ClearQueues()
		{
			lock (this.sendQueue) // Lock is required because Queue<T> is not synchronized.
				this.sendQueue.Clear();

			lock (this.receiveQueue) // Lock is required because Queue<T> is not synchronized.
				this.receiveQueue.Clear();
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
					this.sendThread.Name = ToPortName() + " Send Thread";
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
					this.receiveThread.Name = ToPortName() + " Receive Thread";
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

		/// <remarks>
		/// Using 'Stop' instead of 'Terminate' to emphasize graceful termination, i.e. trying
		/// to join first, then abort if not successfully joined.
		/// </remarks>
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
						DebugThreadState("SendThread() gets stopped...");

						try
						{
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
					else // Called by thread itself!
					{
						this.sendThread = null; // Simply drop the reference to this thread, to be prepar
					}
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
					// Attention, this method may also be called from exception handler within ReceiveThread()!
					if (this.receiveThread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId)
					{
						DebugThreadState("ReceiveThread() gets stopped...");

						try
						{
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
					else // Called by thread itself!
					{
						this.receiveThread = null; // Simply drop the reference to this thread, to be prepar
					}
				}

				if (this.receiveThreadEvent != null)
				{
					try     { this.receiveThreadEvent.Close(); }
					finally { this.receiveThreadEvent = null; }
				}
			} // lock (receiveThreadSyncObj)
		}

		#endregion

		#region Send Thread
		//==========================================================================================
		// Send Thread
		//==========================================================================================

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
			// Calculate maximum baud defined send rate:
			double frameTime   = this.settings.Communication.FrameTime;
			int    frameTime10 = (int)Math.Ceiling(frameTime * 10);

			int interval = 50;          // Interval shall be rather narrow to ensure being inside
			if (interval < frameTime10) // the limits, but ensure that interval is at least 10 times
				interval = frameTime10; // the frame time.

			Rate maxBaudRatePerInterval = new Rate(interval);
			int maxFramesPerInterval = (int)Math.Ceiling(((1.0 / frameTime) * interval * 0.75)); // 25% safety margin.

			// Calculate maximum user defined send rate:
			Rate maxSendRate = new Rate(this.settings.MaxSendRate.Interval);

			bool isOutputBreakOldAndErrorHasBeenSignaled = false;
			bool isCtsInactiveOldAndErrorHasBeenSignaled = false;
			bool   isXOffStateOldAndErrorHasBeenSignaled = false;

			DebugThreadState("SendThread() has started.");

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
						if (this.settings.Communication.FlowControlUsesRtsCts && !this.port.CtsHolding) // No lock required, not modifying anything.
						{
							if (!isCtsInactiveOldAndErrorHasBeenSignaled)
							{
								InvokeCtsInactiveErrorEvent();
								isCtsInactiveOldAndErrorHasBeenSignaled = true;
							}

							break; // Let other threads do their job and wait until signaled again.
						}

						// Handle XOff state:
						if (this.settings.Communication.FlowControlManagesXOnXOffManually && !OutputIsXOn) // XOn/XOff information is not available for 'Software' or 'Combined'!
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

							if (Monitor.TryEnter(this.dataEventSyncObj, 10)) // Allow a short time to enter, as receiving
							{                                                // could be busy mostly locking the object.
								try
								{
									// By default, stuff as much data as possible into output buffer:
									int maxChunkSize = (this.port.WriteBufferSize - this.port.BytesToWrite);

									// Notes on sending:
									//
									// As soon as YAT started to write the maximum chunk size (in Q1/2016 ;-), data got lost
									// even for e.g. a local port loopback pair. All seems to work fine as long as small chunks
									// of e.g. 48 bytes some now and then are transmitted.
									//
									// For a while, I assumed data loss happens in the receive path. Therefore, I tried to use
									// async reading instead of the 'DataReceived' event, as suggested by online resources like
									// http://www.sparxeng.com/blog/software/must-use-net-system-io-ports-serialport written by
									// Ben Voigt.
									//
									// See 'port_DataReceived()' for more details on receiving.
									//
									// Finally (MKy/SSt/ATo in Q3/2016), the root cause for the data loss could be tracked down
									// to the physical limitations of the USB/COM and SPI/COM converter: If more data is sent
									// than the baud rate permits forwarding, the converter simply discards supernumerous data!
									// Of course, what else could it do... Actually, it could propagate the information back to
									// 'System.IO.Ports.SerialPort.BytesToWrite'. But that obviously isn't done...
									//
									// Solution: Limit output writing to baud rate :-)

									// Reduce chunk size if maximum is limited to baud rate:
									if (this.settings.BufferMaxBaudRate)
									{
										int remainingSizeInInterval = (maxFramesPerInterval - maxBaudRatePerInterval.Value);
										maxChunkSize = Int32Ex.Limit(maxChunkSize, 0, Math.Max(remainingSizeInInterval, 0)); // 'max' must be 0 or above.
									}

									// Reduce chunk size if maximum send rate is specified:
									if (this.settings.MaxSendRate.Enabled)
									{
										int remainingSizeInInterval = (this.settings.MaxSendRate.Size - maxSendRate.Value);
										maxChunkSize = Int32Ex.Limit(maxChunkSize, 0, Math.Max(remainingSizeInInterval, 0)); // 'max' must be 0 or above.
									}

									// Further reduce chunk size if maximum is specified:
									if (this.settings.MaxChunkSize.Enabled)
									{
										int maxChunkSizeSetting = this.settings.MaxChunkSize.Size;
										maxChunkSize = Int32Ex.Limit(maxChunkSize, 0, maxChunkSizeSetting); // 'Setting' is always above 0.
									}

									int effectiveChunkDataCount = 0;
									if (maxChunkSize > 0)
									{
										List<byte> effectiveChunkData;
										bool signalIOControlChanged;
										if (TryWriteChunkToPort(maxChunkSize, out effectiveChunkData, out isWriteTimeout, out isOutputBreak, out signalIOControlChanged))
										{
											DebugSendRequest("Signaling " + effectiveChunkData.Count.ToString() + " byte(s) sent...");
											OnDataSent(new SerialDataSentEventArgs(effectiveChunkData.ToArray(), PortId));
											DebugSendRequest("...signaling done");

											effectiveChunkDataCount = effectiveChunkData.Count;
										}

										if (signalIOControlChanged)
										{
											OnIOControlChanged(EventArgs.Empty);
										}
									}

									// Update the send rates with the effective chunk size of the current interval.
									// This must be done no matter whether writing to port has succeeded or not!
									// Otherwise, on overload, a rate value may "get stuck" at the limit!

									if (this.settings.BufferMaxBaudRate)
										maxBaudRatePerInterval.Update(effectiveChunkDataCount);

									if (this.settings.MaxSendRate.Enabled)
										maxSendRate.Update(effectiveChunkDataCount);

									// Note the Thread.Sleep(TimeSpan.Zero) further above.
								}
								finally
								{
									Monitor.Exit(this.dataEventSyncObj);
								}
							} // Monitor.TryEnter()
						}

						if (isWriteTimeout) // Timeout detected while trying to call System.IO.Ports.SerialPort.Write().
						{                   // May only be partial, some data may have been sent before timeout.
							if (this.settings.Communication.FlowControlUsesRtsCts && !this.port.CtsHolding)
							{
								if (!isCtsInactiveOldAndErrorHasBeenSignaled)
								{
									InvokeCtsInactiveErrorEvent();
									isCtsInactiveOldAndErrorHasBeenSignaled = true;
								}
							}
							else if (this.settings.Communication.FlowControlManagesXOnXOffManually && !OutputIsXOn)
							{
								if (!isXOffStateOldAndErrorHasBeenSignaled)
								{
									InvokeXOffErrorEvent();
									isXOffStateOldAndErrorHasBeenSignaled = true;
								}
							}
							else if (this.settings.Communication.FlowControlUsesXOnXOff) // Handle independent on '!OutputIsXOn' because XOn/XOff
							{                                                            // information not available for 'Software' or 'Combined'!
								if (!isXOffStateOldAndErrorHasBeenSignaled)
								{
									InvokeXOffErrorEvent();
									isXOffStateOldAndErrorHasBeenSignaled = true;
								}
							}
							else
							{
								// Do not output a warning in case of unspecified timeouts.
								// Such may happen when too much data is sent too quickly.
							}
						}
						else // !isWriteTimeout
						{
							isCtsInactiveOldAndErrorHasBeenSignaled = false;
							  isXOffStateOldAndErrorHasBeenSignaled = false;
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
			catch (ThreadAbortException ex)
			{
				DebugEx.WriteException(GetType(), ex, "SendThread() has been aborted! Confirming the abort, i.e. Thread.ResetAbort() will be called...");

				// Should only happen when failing to 'friendly' join the thread on stopping!
				// Don't try to set and notify a state change, or even restart the port!

				// But reset the abort request, as 'ThreadAbortException' is a special exception
				// that would be rethrown at the end of the catch block otherwise!

				Thread.ResetAbort();
			}
			catch (IOException ex) // The best way to detect a disconnected device is handling this exception...
			{
				DebugEx.WriteException(GetType(), ex, "SendThread() has detected shutdown of port.");
				RestartOrResetPortAndThreadsAfterExceptionAndNotify();
			}
		#if (!DEBUG)
			catch (Exception ex) // This best-effort approach shall only be done for RELEASE, in order to better detect and analyze the issue during DEBUG.
			{
				DebugEx.WriteException(GetType(), ex, "SendThread() has caught an unexpected exception! Restarting the port to try fixing the issue...");
				RestartOrResetPortAndThreadsAfterExceptionAndNotify();
			}
		#endif

			DebugThreadState("SendThread() has terminated.");
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
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		private bool TryWriteByteToPort(byte b, out bool isWriteTimeout, out bool isOutputBreak, out bool signalIOControlChanged)
		{
			isWriteTimeout         = false;
			isOutputBreak          = false;
			signalIOControlChanged = false;

			bool writeSuccess      = false;
			Exception unhandled    = null;

			if (this.settings.Communication.FlowControl == SerialFlowControl.RS485)
			{
				this.port.RtsEnable = true;
			}

			// Note the remark in SerialPort.Write():
			// "If there are too many bytes in the output buffer and 'Handshake' is set to
			//  'XOnXOff' then the 'SerialPort' object may raise a 'TimeoutException' while
			//  it waits for the device to be ready to accept more data."

			try
			{
				DebugTransmission("Writing 1 byte to port...");

				// Try to write the byte, will raise a 'TimeoutException' if not possible:
				byte[] a = { b };
				this.port.Write(a, 0, 1); // Do not lock, may take some time!
				writeSuccess = true;

				DebugTransmission("...writing done.");

				// Handle XOn/XOff if required:
				if (this.settings.Communication.FlowControlManagesXOnXOffManually) // XOn/XOff information is not available for 'Software' or 'Combined'!
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

				if (this.settings.Communication.FlowControlManagesXOnXOffManually) // XOn/XOff information is not available for 'Software' or 'Combined'!
					this.iXOnXOffHelper.OutputIsXOn = false;
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
				this.port.Flush(); // Make sure that data is sent before restoring RTS, including the underlying physical UART.
				Thread.Sleep((int)Math.Ceiling(this.settings.Communication.FrameTime)); // Single byte/frame.
				this.port.RtsEnable = false;
			}

			if (unhandled != null)
			{
				throw (unhandled); // Re-throw unhandled exception after restoring RTS.
			}

			return (writeSuccess);
		}

		/// <remarks>
		/// Attention, sending a single byte is implemented in <see cref="TryWriteByteToPort"/> above.
		/// Changes here may have to be applied there too.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		private bool TryWriteChunkToPort(int maxChunkSize, out List<byte> effectiveChunkData, out bool isWriteTimeout, out bool isOutputBreak, out bool signalIOControlChanged)
		{
			isWriteTimeout         = false;
			isOutputBreak          = false;
			signalIOControlChanged = false;

			bool writeSuccess      = false;
			Exception unhandled    = null;

			if (this.settings.Communication.FlowControl == SerialFlowControl.RS485)
			{
				this.port.RtsEnable = true;
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

				DebugTransmission("Writing " + triedChunkSize + " byte(s) to port...");

				// Try to write the chunk, will raise a 'TimeoutException' if not possible:
				this.port.Write(a, 0, triedChunkSize); // Do not lock, may take some time!
				writeSuccess = true;

				DebugTransmission("...writing done.");

				// Finalize the write operation:
				lock (this.sendQueue) // Lock is required because Queue<T> is not synchronized.
				{
					for (int i = 0; i < triedChunkSize; i++)
					{
						byte b = this.sendQueue.Dequeue(); // Dequeue the chunk to acknowlege it's gone.

						effectiveChunkData.Add(b);

						// Handle XOn/XOff if required:
						if (this.settings.Communication.FlowControlManagesXOnXOffManually) // XOn/XOff information is not available for 'Software' or 'Combined'!
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

				if (this.settings.Communication.FlowControlManagesXOnXOffManually) // XOn/XOff information is not available for 'Software' or 'Combined'!
					this.iXOnXOffHelper.OutputIsXOn = false;
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

				this.port.Flush(); // Make sure that data is sent before restoring RTS, including the underlying physical UART.
				Thread.Sleep((int)Math.Ceiling(this.settings.Communication.FrameTime * maxFramesInFifo));
				this.port.RtsEnable = false;
			}

			if (unhandled != null)
			{
				throw (unhandled); // Re-throw unhandled exception after restoring RTS.
			}

			return (writeSuccess);
		}

		private void InvokeOutputBreakErrorEvent()
		{
			OnIOError
			(
				new IOErrorEventArgs
				(
					ErrorSeverity.Acceptable,
					Direction.Output,
					"Output break state, retaining data..."
				)
			);
		}

		private void InvokeCtsInactiveErrorEvent()
		{
			OnIOError
			(
				new IOErrorEventArgs
				(
					ErrorSeverity.Acceptable,
					Direction.Output,
					"CTS inactive, retaining data..."
				)
			);
		}

		private void InvokeXOffErrorEvent()
		{
			OnIOError
			(
				new IOErrorEventArgs
				(
					ErrorSeverity.Acceptable,
					Direction.Output,
					"XOff state, retaining data..."
				)
			);
		}

		#endregion

		#region Port Events
		//==========================================================================================
		// Port Events
		//==========================================================================================

		/// <remarks>
		/// As soon as YAT started to write the maximum chunk size (in Q1/2016 ;-), data got lost
		/// even for e.g. a local port loopback pair. All seems to work fine as long as small chunks
		/// of e.g. 48 bytes some now and then are transmitted.
		///
		/// For a while, I assumed data loss happens in the receive path. Therefore, I tried to use
		/// async reading instead of the 'DataReceived' event, as suggested by online resources like
		/// http://www.sparxeng.com/blog/software/must-use-net-system-io-ports-serialport written by
		/// Ben Voigt.
		///
		/// However, there seems to be no difference whether 'DataReceived' and 'BytesToRead' or
		/// async reading is used. Both loose the equal amount of data, this fact is also supported
		/// be the 'DriverAnalysis'. Also, opposed to what Ben Voigt states, async reading actually
		/// results in smaller chunks, mostly 1 byte reads. Whereas the obvious 'DataReceived' and
		/// 'BytesToRead' mostly result in 1..4 byte reads, even up to 20..30 bytes. Thus, this
		/// implementation again uses the 'normal' method.
		///
		/// Finally (MKy/SSt/ATo in Q3/2016), the root cause for the data loss could be tracked down
		/// to the physical limitations of the USB/COM and SPI/COM converter: If more data is sent
		/// than the baud rate permits forwarding, the converter simply discards supernumerous data!
		/// Of course, what else could it do... Actually, it could propagate the information back to
		/// <see cref="System.IO.Ports.SerialPort.BytesToWrite"/>. But that obviously isn't done...
		///
		///
		/// Additional information on receiving
		/// -----------------------------------
		/// Another improvement suggested by Marco Stroppel on 2011-02-17 doesn't work with YAT.
		///
		/// Suggestion: The while(BytesAvailable > 0) raises endless events, because I did not call
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
		/// queue, no more event gets raised. Thus, the 3 bytes do not get shown until new data
		/// arrives. This is not acceptable.
		/// </remarks>
		/// <remarks>
		/// Asynchronously manage incoming events to prevent potential deadlocks if close/dispose
		/// was called from a ISynchronizeInvoke target (i.e. a form) on an event thread.
		/// Also, the mechanism implemented below reduces the amount of events that are propagated
		/// to the main application. Small chunks of received data will generate many events
		/// handled by <see cref="port_DataReceived"/>. However, since <see cref="OnDataReceived"/>
		/// synchronously invokes the event, it will take some time until the send queue is checked
		/// again. During this time, no more new events are invoked, instead, incoming data is
		/// buffered.
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
						DebugTransmission("Enqueuing " + data.Length.ToString(CultureInfo.CurrentCulture) + " byte(s) for receiving...");
						foreach (byte b in data)
						{
							this.receiveQueue.Enqueue(b);

							// Handle output XOn/XOff state:
							if (this.settings.Communication.FlowControlManagesXOnXOffManually) // XOn/XOff information is not available for 'Software' or 'Combined'!
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
						DebugTransmission("...enqueuing done");
					} // lock (this.receiveQueue)

					// Signal XOn/XOff change to send thread:
					if (signalXOnXOff)
						SignalSendThreadSafely();

					// Signal data notification to receive thread:
					SignalReceiveThreadSafely();

					// Immediately invoke the event, but invoke it asynchronously and NOT on this thread!
					if (signalXOnXOff || signalXOnXOffCount)
						OnIOControlChangedAsync(EventArgs.Empty); // Async! See remarks above.
				} // if (!IsDisposed && ...)
			}
			catch (IOException ex) // The best way to detect a disconnected device is handling this exception...
			{
				DebugEx.WriteException(GetType(), ex, "DataReceived() has detected shutdown of port as it is no longer accessible.");
				RestartOrResetPortAndThreadsAfterExceptionAndNotify();
			}
		#if (!DEBUG)
			catch (Exception ex) // This best-effort approach shall only be done for RELEASE, in order to better detect and analyze the issue during DEBUG.
			{
				DebugEx.WriteException(GetType(), ex, "DataReceived() has has caught an unexpected exception! Restarting the port to try fixing the issue...");
				RestartOrResetPortAndThreadsAfterExceptionAndNotify();
			}
		#endif
		}

		/// <remarks>
		/// Will be signaled by <see cref="port_DataReceived"/> event above, or by XOn/XOff while
		/// sending.
		/// </remarks>
		[SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", MessageId = "System.Threading.WaitHandle.#WaitOne(System.Int32)", Justification = "Installer indeed targets .NET 3.5 SP1.")]
		private void ReceiveThread()
		{
			DebugThreadState("ReceiveThread() has started.");

			try
			{
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

						if (Monitor.TryEnter(this.dataEventSyncObj, 10)) // Allow a short time to enter, as sending
						{                                                // could be busy mostly locking the object.
							try
							{
								byte[] data;
								lock (this.receiveQueue) // Lock is required because Queue<T> is not synchronized.
								{
									data = this.receiveQueue.ToArray();
									this.receiveQueue.Clear();
								}

								DebugReceiveRequest("Signaling " + data.Length.ToString() + " byte(s) received...");
								OnDataReceived(new SerialDataReceivedEventArgs(data, PortId));
								DebugReceiveRequest("...signaling done");

								// Note the Thread.Sleep(TimeSpan.Zero) above.
							}
							finally
							{
								Monitor.Exit(this.dataEventSyncObj);
							}
						} // Monitor.TryEnter()
					} // Inner loop
				} // Outer loop
			}
			catch (ThreadAbortException ex)
			{
				DebugEx.WriteException(GetType(), ex, "ReceiveThread() has been aborted! Confirming the abort, i.e. Thread.ResetAbort() will be called...");

				// Should only happen when failing to 'friendly' join the thread on stopping!
				// Don't try to set and notify a state change, or even restart the port!

				// But reset the abort request, as 'ThreadAbortException' is a special exception
				// that would be rethrown at the end of the catch block otherwise!

				Thread.ResetAbort();
			}

			DebugThreadState("ReceiveThread() has terminated.");
		}

		/// <remarks>
		/// Asynchronously invoke incoming events to prevent potential deadlocks if close/dispose
		/// was called from a ISynchronizeInvoke target (i.e. a form) on an event thread.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that any exception leads to restart or reset of port.")]
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

					// Raise events:
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
							// Do not raise general 'IOChanged' event.
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
			catch (IOException ex) // The best way to detect a disconnected device is handling this exception...
			{
				DebugEx.WriteException(GetType(), ex, "PinChanged() has detected shutdown of port as it is no longer accessible.");
				RestartOrResetPortAndThreadsAfterExceptionAndNotify();
			}
		#if (!DEBUG)
			catch (Exception ex) // This best-effort approach shall only be done for RELEASE, in order to better detect and analyze the issue during DEBUG.
			{
				DebugEx.WriteException(GetType(), ex, "PinChanged() has caught an unexpected exception! Restarting the port to try fixing the issue...");
				RestartOrResetPortAndThreadsAfterExceptionAndNotify();
			}
		#endif
		}

		[SuppressMessage("Microsoft.Mobility", "CA1601:DoNotUseTimersThatPreventPowerStateChanges", Justification = "The timer just invokes a single-shot callback.")]
		private void StartControlEventTimeout()
		{
			if (this.ioControlEventTimeout == null)
			{
				this.ioControlEventTimeout = new System.Timers.Timer(IOControlChangedTimeout * 2); // Synchronous event shall have precedence over timeout.
				this.ioControlEventTimeout.AutoReset = false;
				this.ioControlEventTimeout.Elapsed += ioControlEventTimeout_Elapsed;
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

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private object ioControlEventTimeout_Elapsed_SyncObj = new object();

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		private void ioControlEventTimeout_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			// Ensure that only one timer elapsed event thread is active at a time. Because if the
			// execution takes longer than the timer interval, more and more timer threads will pend
			// here, and then be executed after the previous has been executed. This will require
			// more and more resources and lead to a drop in performance.
			if (Monitor.TryEnter(ioControlEventTimeout_Elapsed_SyncObj))
			{
				try
				{
					if (!IsDisposed && IsStarted) // Check 'IsDisposed' first!
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
				}
				finally
				{
					Monitor.Exit(ioControlEventTimeout_Elapsed_SyncObj);
				}
			} // Monitor.TryEnter()
		}

		/// <remarks>
		/// Asynchronously invoke incoming events to prevent potential deadlocks if close/dispose
		/// was called from a ISynchronizeInvoke target (i.e. a form) on an event thread.
		/// </remarks>
		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true, Rationale = "SerialPort.ErrorReceived: Only one event handler can execute at a time.")]
		private void port_ErrorReceived(object sender, MKY.IO.Ports.SerialErrorReceivedEventArgs e)
		{
			if (!IsDisposed && IsOpen) // Ensure not to forward events during closing anymore.
			{
				ErrorSeverity severity = ErrorSeverity.Severe;
				Direction direction;
				string message;

				switch (e.EventType)
				{
					case System.IO.Ports.SerialError.Frame:    direction = Direction.Input;  message = "Input framing error!";            break;
					case System.IO.Ports.SerialError.Overrun:  direction = Direction.Input;  message = "Input character buffer overrun!"; break;
					case System.IO.Ports.SerialError.RXOver:   direction = Direction.Input;  message = "Input buffer overflow!";          break;
					case System.IO.Ports.SerialError.RXParity: direction = Direction.Input;  message = "Input parity error!";             break;
					case System.IO.Ports.SerialError.TXFull:   direction = Direction.Output; message = "Output buffer full!";             break;
					default:  severity = ErrorSeverity.Fatal;  direction = Direction.None;   message = "Unknown error!";                  break;
				}

				OnIOErrorAsync(new SerialPortErrorEventArgs(severity, direction, message, e.EventType)); // Async! See remarks above.
			}
		}

		#endregion

		#region Alive Ticker
		//==========================================================================================
		// Alive Ticker
		//==========================================================================================

		/// <remarks>
		/// Note that this monitor is active even during sending and receiving data. Restarting the
		/// timer on each write and read operation to the port, i.e. temporarily disable the timer,
		/// is too time consuming. It's way better performing by simply letting the monitor run.
		/// </remarks>
		[SuppressMessage("Microsoft.Mobility", "CA1601:DoNotUseTimersThatPreventPowerStateChanges", Justification = "Well, any better idea on how to check whether the serial port is still alive?")]
		private void StartAliveMonitor()
		{
			lock (this.aliveMonitorSyncObj)
			{
				if (this.aliveMonitor == null)
				{
					this.aliveMonitor = new System.Timers.Timer(this.settings.AliveMonitor.Interval);
					this.aliveMonitor.AutoReset = true;
					this.aliveMonitor.Elapsed += aliveMonitor_Elapsed;
					this.aliveMonitor.Start();
				}
				else
				{
					// Already exists and running (AutoReset = true).
				}
			}
		}

		private void StopAndDisposeAliveMonitor()
		{
			lock (this.aliveMonitorSyncObj)
			{
				if (this.aliveMonitor != null)
				{
					this.aliveMonitor.Stop();
					this.aliveMonitor.Dispose();
					this.aliveMonitor = null;
				}
			}
		}

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private object aliveMonitor_Elapsed_SyncObj = new object();

		private void aliveMonitor_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			// Ensure that only one timer elapsed event thread is active at a time. Because if the
			// execution takes longer than the timer interval, more and more timer threads will pend
			// here, and then be executed after the previous has been executed. This will require
			// more and more resources and lead to a drop in performance.
			if (Monitor.TryEnter(aliveMonitor_Elapsed_SyncObj))
			{
				try
				{
					if (!IsDisposed && IsStarted) // Check 'IsDisposed' first!
					{
						if (!Ports.SerialPortCollection.IsAvailable(PortId))
						{
							DebugMessage("AliveMonitorElapsed() has detected shutdown of port as it is no longer available.");
							RestartOrResetPortAndThreadsAfterExceptionAndNotify();
						}

						// Note that the AliveMonitor is AutoReset = true.
					}
					else
					{
						StopAndDisposeAliveMonitor();
					}
				}
				finally
				{
					Monitor.Exit(aliveMonitor_Elapsed_SyncObj);
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
			lock (this.reopenTimeoutSyncObj)
			{
				if (this.reopenTimeout == null)
				{
					this.reopenTimeout = new System.Timers.Timer(this.settings.AutoReopen.Interval);
					this.reopenTimeout.AutoReset = false;
					this.reopenTimeout.Elapsed += this.reopenTimeout_Elapsed;
				}
				else
				{
					// Already exists but not necessarily running (AutoReset = false).
				}

				this.reopenTimeout.Start();
			}
		}

		private void StopAndDisposeReopenTimeout()
		{
			lock (this.reopenTimeoutSyncObj)
			{
				if (this.reopenTimeout != null)
				{
					this.reopenTimeout.Stop();
					this.reopenTimeout.Dispose();
					this.reopenTimeout = null;
				}
			}
		}

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private object reopenTimeout_Elapsed_SyncObj = new object();

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that any exception leads to restart or reset of port.")]
		private void reopenTimeout_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			// Ensure that only one timer elapsed event thread is active at a time. Because if the
			// execution takes longer than the timer interval, more and more timer threads will pend
			// here, and then be executed after the previous has been executed. This will require
			// more and more resources and lead to a drop in performance.
			if (Monitor.TryEnter(reopenTimeout_Elapsed_SyncObj))
			{
				try
				{
					if (!IsDisposed && IsStarted && !IsOpen && this.settings.AutoReopen.Enabled) // Check 'IsDisposed' first!
					{
						if (Ports.SerialPortCollection.IsAvailable(PortId))
						{
							try
							{
								CreateAndOpenPortAndThreadsAndNotify(); // Try to reopen port.
								DebugMessage("ReopenTimerElapsed() successfully reopened the port.");
							}
							catch // Do not output exception onto debug console, console would get spoilt with useless information.
							{
								DebugMessage("ReopenTimerElapsed() has failed to reopen the port.");
								RestartOrResetPortAndThreadsAfterExceptionWithoutNotify(); // Cleanup and restart. No notifications.
							}
						}
						else
						{
							StartReopenTimeout();
						}

						// Note that the ReopenTimeout is AutoReset = false.
					}
					else
					{
						StopAndDisposeReopenTimeout();
					}
				}
				finally
				{
					Monitor.Exit(reopenTimeout_Elapsed_SyncObj);
				}
			} // Monitor.TryEnter()
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

		/// <remarks>See remarks on top of MKY.IO.Ports.SerialPort.SerialPortEx why asynchronously is required.</remarks>
		[CallingContract(IsNeverMainThread = true)]
		protected virtual void OnIOChangedAsync(EventArgs e)
		{
			if (!IsDisposed) // Make sure to propagate event only if not already disposed.
				this.eventHelper.RaiseAsync(IOChanged, this, e);
		}

		/// <summary></summary>
		[CallingContract(IsNeverMainThread = true)]
		protected virtual void OnIOControlChanged(EventArgs e)
		{
			if (!IsDisposed) // Make sure to propagate event only if not already disposed.
			{
				this.eventHelper.RaiseSync(IOControlChanged, this, e);

				SetNextControlChangedTickStamp();
			}
		}

		/// <remarks>See remarks on top of MKY.IO.Ports.SerialPort.SerialPortEx why asynchronously is required.</remarks>
		[CallingContract(IsNeverMainThread = true)]
		protected virtual void OnIOControlChangedAsync(EventArgs e)
		{
			if (!IsDisposed) // Make sure to propagate event only if not already disposed.
			{
				this.eventHelper.RaiseAsync(IOControlChanged, this, e);

				SetNextControlChangedTickStamp();
			}
		}

		private void SetNextControlChangedTickStamp()
		{
			lock (this.nextIOControlEventTickStampSyncObj)
			{
				unchecked
				{
					this.nextIOControlEventTickStamp = (Stopwatch.GetTimestamp() + IOControlChangedTickInterval); // Loop-around is OK.
				}
			}
		}

		/// <summary></summary>
		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true)]
		protected virtual void OnIOError(IOErrorEventArgs e)
		{
			if (!IsDisposed) // Make sure to propagate event only if not already disposed.
				this.eventHelper.RaiseSync<IOErrorEventArgs>(IOError, this, e);
		}

		/// <remarks>See remarks on top of MKY.IO.Ports.SerialPort.SerialPortEx why asynchronously is required.</remarks>
		[CallingContract(IsNeverMainThread = true)]
		protected virtual void OnIOErrorAsync(IOErrorEventArgs e)
		{
			if (!IsDisposed) // Make sure to propagate event only if not already disposed.
				this.eventHelper.RaiseAsync<IOErrorEventArgs>(IOError, this, e);
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

			return (ToPortName());
		}

		/// <summary></summary>
		public virtual string ToPortName()
		{
			// Do not call AssertNotDisposed() on such basic method! Its return value is needed for debugging! All underlying fields are still valid after disposal.

			var id = PortId;
			if (id != null)
				return (id.Name);
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
					"[" + ToPortName() + "]",
					message
				)
			);
		}

		[Conditional("DEBUG_THREAD_STATE")]
		private void DebugThreadState(string message)
		{
			DebugMessage(message);
		}

		[Conditional("DEBUG_SEND_REQUEST")]
		private void DebugSendRequest(string message)
		{
			DebugMessage(message);
		}

		[Conditional("DEBUG_TRANSMISSION")]
		private void DebugTransmission(string message)
		{
			DebugMessage(message);
		}

		[Conditional("DEBUG_RECEIVE_REQUEST")]
		private void DebugReceiveRequest(string message)
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
