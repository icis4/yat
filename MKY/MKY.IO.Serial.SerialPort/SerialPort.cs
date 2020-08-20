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
// Copyright © 2003-2020 Matthias Kläy.
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
////#define DEBUG_THREAD_STATE // Attention: Must also be activated in SerialPort.(Receive&Send).cs !!

	// Enable debugging of receiving:
////#define DEBUG_RECEIVE // Attention: Must also be activated in SerialPort.Receive.cs !!

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
	/// <remarks>
	/// This class is implemented using partial classes separating sending/receiving functionality.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop isn't able to skip URLs...")]
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Different root namespace.")]
	public partial class SerialPort : DisposableBase, IIOProvider, IXOnXOffHandler
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

		private const int SendQueueInitialCapacity    = Ports.SerialPortEx.WriteBufferSizeDefault;
		private const int ReceiveQueueInitialCapacity = Ports.SerialPortEx.ReadBufferSizeDefault;

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

		private Queue<byte> sendQueue = new Queue<byte>(SendQueueInitialCapacity);
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

		private object dataEventSyncObj = new object();

		private System.Timers.Timer ioControlEventTimeout; // Ambiguity with 'System.Threading.Timer'.
		private object ioControlEventTimeoutSyncObj = new object();
		private long nextIOControlEventTickStamp; // Ticks as defined by 'Stopwatch'.
		private object nextIOControlEventTickStampSyncObj = new object();

		/// <summary>
		/// Alive timer detects port disconnects, i.e. when a USB to serial converter is disconnected.
		/// </summary>
		private System.Threading.Timer aliveMonitorTimeout; // Explicit for ambiguity with 'System.Timers.Timer'.
		private object aliveMonitorTimeoutSyncObj = new object();

		private System.Threading.Timer reopenTimeout; // Explicit for ambiguity with 'System.Timers.Timer'.
		private object reopenTimeoutSyncObj = new object();

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		public event EventHandler<EventArgs<DateTime>> IOChanged;

		/// <summary></summary>
		public event EventHandler<EventArgs<DateTime>> IOControlChanged;

		/// <summary></summary>
		public event EventHandler<IOWarningEventArgs> IOWarning;

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

		/// <param name="disposing">
		/// <c>true</c> when called from <see cref="Dispose"/>,
		/// <c>false</c> when called from finalizer.
		/// </param>
		[SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "port", Justification = "Is actually disposed of asynchronously in ResetPortAndThreadsAndNotify().")]
		protected override void Dispose(bool disposing)
		{
			this.eventHelper.DiscardAllEventsAndExceptions();

			// Dispose of managed resources:
			if (disposing)
			{
				// In the 'normal' case, the items have already been disposed of in e.g. Stop().
				ResetPortAndThreadsWithoutNotify(false); // Suppress notifications during disposal!
			}
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
			////AssertUndisposed() shall not be called from this simple get-property.

				return (this.settings);
			}
			set
			{
			////AssertUndisposed() is called by 'IsStopped' below.

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
			////AssertUndisposed() shall not be called from this simple get-property.

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
			////AssertUndisposed() shall not be called from this simple get-property.

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
			////AssertUndisposed() shall not be called from this simple get-property.

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
			////AssertUndisposed() shall not be called from this simple get-property.

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
				else // Monitor.TryEnter()
				{
					DebugMessage("IsOpen failed to synchronize access to the port object!");

					return (false);
				}
			}
		}

		/// <summary></summary>
		public virtual bool IsConnected
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

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
				else // Monitor.TryEnter()
				{
					DebugMessage("IsConnected failed to synchronize access to the port object!");

					return (false);
				}
			}
		}

		/// <summary></summary>
		public virtual bool IsTransmissive
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

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
				else // Monitor.TryEnter()
				{
					DebugMessage("IsTransmissive failed to synchronize access to the port object!");

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
				AssertUndisposed();

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
				AssertUndisposed();

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
				AssertUndisposed();

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
				AssertUndisposed();

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
				AssertUndisposed();

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
				AssertUndisposed();

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
				AssertUndisposed();

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
				AssertUndisposed();

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
				AssertUndisposed();

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
				AssertUndisposed();

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
				AssertUndisposed();

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
				AssertUndisposed();

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
		////AssertUndisposed() is called by 'IsStopped' below.

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
		////AssertUndisposed() is called by 'IsStarted' below.

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

		/// <summary>
		/// Resets the XOn/XOff signaling count.
		/// </summary>
		public virtual void ResetXOnXOffCount()
		{
			AssertUndisposed();

			this.iXOnXOffHelper.ResetCounts();

			OnIOControlChanged(new EventArgs<DateTime>(DateTime.Now));
		}

		/// <summary>
		/// Resets the flow control signaling count.
		/// </summary>
		public virtual void ResetFlowControlCount()
		{
		////AssertUndisposed() is called by 'ResetXOnXOffCount()' below.

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
			AssertUndisposed();

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
			State oldState;

			lock (this.stateSyncObj)
			{
				oldState = this.state;
				this.state = state;
			}

			if ((state != oldState) && withNotify)
			{
			#if (DEBUG)
				if (state != oldState)
					DebugMessage("State has changed from " + oldState + " to " + state + ".");
				else
					DebugMessage("State is already " + oldState + ".");
			#endif
				// Notify asynchronously because the state will get changed from asynchronous items
				// such as the reopen timer. In case of that timer, the port needs to be locked to
				// ensure proper operation. In such case, a synchronous notification callback would
				// likely result in a deadlock, in case the callback sink would call any method or
				// property that also locks the port!

				var now = DateTime.Now;
				OnIOChangedAsync(       new EventArgs<DateTime>(now));
				OnIOControlChangedAsync(new EventArgs<DateTime>(now));
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

				if (this.settings.SignalXOnWhenOpened)
				{
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
			DropSendQueueAndNotify();
			DropReceiveQueueAndNotify();
		}

		#endregion

		#region Port Threads
		//==========================================================================================
		// Port Threads
		//==========================================================================================

		/// <remarks>
		/// Measuring timing after introducing the signal related keywords like \!(RTSToggle) in YAT
		/// revealed a limitation with the current implementation:
		///
		/// Sending "A\!(RTSToggle)B+EOL" will result in a significant delay before sending "B+EOL".
		/// The delay is ~20 ms.
		///
		/// The limitation can be made visible by placing debug outputs at various locations in the
		/// following methods:
		/// <see cref="Send(byte[])"/>, <see cref="SignalSendThreadSafely"/>, <see cref="SendThread"/>,
		/// <see cref="TryWriteChunkToPort(int, out List{byte}, out bool, out bool, out bool, out DateTime)"/>
		///
		/// The debug outputs reveal that the two send calls ("A" and "B+EOL") of course result in
		/// scheduling of two threads: The terminal send thread and the I/O send thread.
		///
		/// Scheduling will happen at arbitrary locations, but will (always) result in the second
		/// chunk being significantly delayed.
		///
		/// An attempt to raise the priority of the <see cref="SendThread"/> made the situation even
		/// worse:
		///
		/// Doing \!(RTSToggle) will happen way before "AB+EOL" is sent!
		///
		/// Accepting the current limitation, but keeping in mind, that refactoring to an action
		/// queue combining send data and change signal requests could be done to improve this.
		/// </remarks>
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
			// First, clear both flags to reduce the time to stop the threads, they may already
			// be signaled while receiving data or while the send thread is still running:

			lock (this.sendThreadSyncObj)
				this.sendThreadRunFlag = false;

			lock (this.receiveThreadSyncObj)
				this.receiveThreadRunFlag = false;

			// Then, wait for threads to terminate:

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

		#region Port Events
		//==========================================================================================
		// Port Events
		//==========================================================================================

		/// <remarks>
		/// As soon as YAT started to write the maximum chunk size (in Q1/2016), data got lost even
		/// for e.g. a local port loopback pair. All seems to work fine as long as small chunks of
		/// e.g. 48 bytes some now and then are transmitted.
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
		/// <see cref="System.IO.Ports.SerialPort.BytesToWrite"/>. But that apparently isn't done...
		///
		///
		/// Additional information on receiving
		/// -----------------------------------
		/// Another improvement suggested by Marco Stroppel on 2011-02-17 doesn't work with YAT.
		///
		/// Suggestion: The while (BytesAvailable > 0) raises endless events, because I did not call
		/// the Receive() method. That was, because I receive only the data when the other port to
		/// write the data is opened. So the BytesAvailable got never zero. My idea was (not knowing
		/// if this is good) to do something like:
		///
		/// while (BytesAvailable > LastTimeBytesAvailable)
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
				if (IsUndisposed && IsOpen) // Ensure not to perform any operations during closing anymore. Check disposal state first!
				{
					DebugReceive("'DataReceived' event...");

					// Immediately read data on this thread:
					int bytesToRead;
					byte[] data;
					lock (this.portSyncObj)
					{
						bytesToRead = this.port.BytesToRead;

						DebugReceive("...with {0} bytes...", bytesToRead);

						data = new byte[bytesToRead];
						this.port.Read(data, 0, bytesToRead);
					}

					DebugReceive("...read completed.");

					// Attention:
					// XOn/XOff handling is implemented in MKY.IO.Serial.Usb.SerialHidDevice too!
					// Changes here must most likely be applied there too.

					bool signalXOnXOff = false;
					bool signalXOnXOffCount = false;

					lock (this.receiveQueue) // Lock is required because Queue<T> is not synchronized.
					{
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
					} // lock (this.receiveQueue)

					DebugReceiveEnqueue(data.Length);

					// Signal XOn/XOff change to send thread:
					if (signalXOnXOff)
						SignalSendThreadSafely();

					// Signal data notification to receive thread:
					SignalReceiveThreadSafely();

					// Immediately invoke the event, but invoke it asynchronously and NOT on this thread!
					if (signalXOnXOff || signalXOnXOffCount)
						OnIOControlChangedAsync(new EventArgs<DateTime>(DateTime.Now)); // Async! See remarks above.
				} // if (IsUndisposed && ...)
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
				if (IsUndisposed && IsOpen) // Ensure not to forward events during closing anymore.
				{
					// Signal pin change to threads:
					SignalThreadsSafely();

					// Force access to port to check whether the port is still alive:
					int dummyByteToRead = this.port.BytesToRead; // Force e.g. 'IOException', details see alive timer.
					UnusedLocal.PreventAnalysisWarning(dummyByteToRead, "Dummy variable is required to retrieve a property.");

					// Raise events:
					switch (e.EventType)
					{
						case MKY.IO.Ports.SerialPinChange.InputBreak:
							if (this.settings.NoSendOnInputBreak)
								OnIOChangedAsync(new EventArgs<DateTime>(DateTime.Now)); // Async! See remarks above.
							break;

						case MKY.IO.Ports.SerialPinChange.OutputBreak:
							OnIOChangedAsync(new EventArgs<DateTime>(DateTime.Now)); // Async! See remarks above.
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
						var now = DateTime.Now;
						StopAndDisposeControlEventTimeout();
						OnIOControlChangedAsync(new EventArgs<DateTime>(now)); // Async! See remarks above.
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
			lock (this.ioControlEventTimeoutSyncObj)
			{
				if (this.ioControlEventTimeout == null)
				{
					this.ioControlEventTimeout = new System.Timers.Timer(); // 'Timers.Timer' rather than 'Threading.Timer' because 'e.SignalTime' is needed.
					this.ioControlEventTimeout.Interval = (IOControlChangedTimeout * 2); // Synchronous event shall have precedence over timeout.
					this.ioControlEventTimeout.AutoReset = false; // One-Shot!
					this.ioControlEventTimeout.Elapsed += ioControlEventTimeout_OneShot_Elapsed;
				}
				else
				{
					// Already exists but not necessarily running (AutoReset = false).
				}

				this.ioControlEventTimeout.Start();
			}
		}

		private void StopAndDisposeControlEventTimeout()
		{
			lock (this.ioControlEventTimeoutSyncObj)
			{
				if (this.ioControlEventTimeout != null)
				{
					this.ioControlEventTimeout.Stop();
					this.ioControlEventTimeout.Dispose();
					this.ioControlEventTimeout = null;
				}
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		private void ioControlEventTimeout_OneShot_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			// Non-periodic timer, only a single callback can be active at a time.
			// There is no need to synchronize concurrent callbacks to this event handler.

			lock (this.ioControlEventTimeoutSyncObj)
			{
				if (this.ioControlEventTimeout == null)
					return; // Handle overdue callbacks.
			}

			if (IsUndisposed && IsStarted) // Check disposal state first!
			{
				try
				{
					OnIOControlChanged(new EventArgs<DateTime>(e.SignalTime));
				}
				catch (Exception ex) // Handle any exception, port could e.g. got closed in the meantime.
				{
					DebugEx.WriteException(GetType(), ex, "Exception while invoking 'OnIOControlChanged' event after timeout!");
				}
			}
		}

		/// <remarks>
		/// Asynchronously invoke incoming events to prevent potential deadlocks if close/dispose
		/// was called from a ISynchronizeInvoke target (i.e. a form) on an event thread.
		/// </remarks>
		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true, Rationale = "SerialPort.ErrorReceived: Only one event handler can execute at a time.")]
		private void port_ErrorReceived(object sender, MKY.IO.Ports.SerialErrorReceivedEventArgs e)
		{
			if (IsUndisposed && IsOpen) // Ensure not to forward events during closing anymore.
			{
				ErrorSeverity severity = ErrorSeverity.Severe;
				Direction direction;
				string message;

				switch (e.EventType)
				{
					case System.IO.Ports.SerialError.Frame:    direction = Direction.Input;  message = "Input framing error!";   break;
					case System.IO.Ports.SerialError.Overrun:  direction = Direction.Input;  message = "Input buffer overrun!";  break;
					case System.IO.Ports.SerialError.RXOver:   direction = Direction.Input;  message = "Input buffer overflow!"; break;
					case System.IO.Ports.SerialError.RXParity: direction = Direction.Input;  message = "Input parity error!";    break;
					case System.IO.Ports.SerialError.TXFull:   direction = Direction.Output; message = "Output buffer full!";    break;
					default: severity = ErrorSeverity.Fatal;   direction = Direction.None;   message = "Unknown error!";         break;
				}

				DebugMessage(message); // Output in any case, likely to help debugging tricky cases.

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
			lock (this.aliveMonitorTimeoutSyncObj)
			{
				if (this.aliveMonitorTimeout == null)
				{
					var callback = new TimerCallback(aliveMonitorTimeout_Periodic_Elapsed);
					var dueTime = this.settings.AliveMonitor.Interval;
					var period  = this.settings.AliveMonitor.Interval; // Periodic!

					this.aliveMonitorTimeout = new Timer(callback, null, dueTime, period);
				}
				else
				{
					// Already exists and running (periodic).
				}
			}
		}

		private void StopAndDisposeAliveMonitor()
		{
			lock (this.aliveMonitorTimeoutSyncObj)
			{
				if (this.aliveMonitorTimeout != null)
				{
					this.aliveMonitorTimeout.Dispose();
					this.aliveMonitorTimeout = null;
				}
			}
		}

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private object aliveMonitorTimeout_Periodic_Elapsed_SyncObj = new object();

		private void aliveMonitorTimeout_Periodic_Elapsed(object obj)
		{
			// Ensure that only one timer elapsed event thread is active at a time. Because if the
			// execution takes longer than the timer interval, more and more timer threads will pend
			// here, and then be executed after the previous has been executed. This will require
			// more and more resources and lead to a drop in performance.
			if (Monitor.TryEnter(aliveMonitorTimeout_Periodic_Elapsed_SyncObj))
			{
				try
				{
					if (IsUndisposed && IsStarted) // Check disposal state first!
					{
						if (!Ports.SerialPortCollection.IsAvailable(PortId))
						{
							DebugMessage("aliveMonitorTimeout_Elapsed() has detected shutdown of port as it is no longer available.");
							RestartOrResetPortAndThreadsAfterExceptionAndNotify();
						}
					}
					else
					{
						StopAndDisposeAliveMonitor();
					}
				}
				finally
				{
					Monitor.Exit(aliveMonitorTimeout_Periodic_Elapsed_SyncObj);
				}
			}
			else // Monitor.TryEnter()
			{
				DebugMessage("aliveMonitorTimeout_Elapsed() monitor has timed out, skipping this concurrent event.");
			}
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
				var dueTime = this.settings.AutoReopen.Interval;
				var period = Timeout.Infinite; // One-Shot!

				if (this.reopenTimeout == null)
					this.reopenTimeout = new Timer(new TimerCallback(reopenTimeout_OneShot_Elapsed), null, dueTime, period);
				else
					this.reopenTimeout.Change(dueTime, period);
			}
		}

		private void StopAndDisposeReopenTimeout()
		{
			lock (this.reopenTimeoutSyncObj)
			{
				if (this.reopenTimeout != null)
				{
					this.reopenTimeout.Dispose();
					this.reopenTimeout = null;
				}
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that any exception leads to restart or reset of port.")]
		private void reopenTimeout_OneShot_Elapsed(object obj)
		{
			// Non-periodic timer, only a single callback can be active at a time.
			// There is no need to synchronize concurrent callbacks to this event handler.

			lock (this.reopenTimeoutSyncObj)
			{
				if (this.reopenTimeout == null)
					return; // Handle overdue callbacks.
			}

			if (IsUndisposed && IsStarted && !IsOpen && this.settings.AutoReopen.Enabled) // Check disposal state first!
			{
				if (Ports.SerialPortCollection.IsAvailable(PortId))
				{
					try
					{
						CreateAndOpenPortAndThreadsAndNotify(); // Try to reopen port.
						DebugMessage("reopenTimeout_OneShot_Elapsed() successfully reopened the port.");
					}
					catch // Do not output exception onto debug console, console would get spoilt with useless information.
					{
						DebugMessage("reopenTimeout_OneShot_Elapsed() has failed to reopen the port.");
						RestartOrResetPortAndThreadsAfterExceptionWithoutNotify(); // Cleanup and restart. No notifications.
					}
				}
				else
				{
					StartReopenTimeout();
				}
			}
		}

		#endregion

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

		/// <summary></summary>
		[CallingContract(IsNeverMainThread = true)]
		protected virtual void OnIOChanged(EventArgs<DateTime> e)
		{
			if (IsUndisposed) // Ensure to not propagate event during closing anymore.
				this.eventHelper.RaiseSync(IOChanged, this, e);
		}

		/// <remarks>See remarks on top of MKY.IO.Ports.SerialPort.SerialPortEx why asynchronously is required.</remarks>
		[CallingContract(IsNeverMainThread = true)]
		protected virtual void OnIOChangedAsync(EventArgs<DateTime> e)
		{
			if (IsUndisposed) // Ensure to not propagate event during closing anymore.
				this.eventHelper.RaiseAsync(IOChanged, this, e);
		}

		/// <summary></summary>
		[CallingContract(IsNeverMainThread = true)]
		protected virtual void OnIOControlChanged(EventArgs<DateTime> e)
		{
			if (IsUndisposed) // Ensure to not propagate event during closing anymore.
			{
				this.eventHelper.RaiseSync(IOControlChanged, this, e);

				SetNextControlChangedTickStamp();
			}
		}

		/// <remarks>See remarks on top of MKY.IO.Ports.SerialPort.SerialPortEx why asynchronously is required.</remarks>
		[CallingContract(IsNeverMainThread = true)]
		protected virtual void OnIOControlChangedAsync(EventArgs<DateTime> e)
		{
			if (IsUndisposed) // Ensure to not propagate event during closing anymore.
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
		protected virtual void OnIOWarning(IOWarningEventArgs e)
		{
			if (IsUndisposed) // Ensure to not propagate event during closing anymore.
				this.eventHelper.RaiseSync<IOWarningEventArgs>(IOWarning, this, e);
		}

		/// <summary></summary>
		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true)]
		protected virtual void OnIOError(IOErrorEventArgs e)
		{
			if (IsUndisposed) // Ensure to not propagate event during closing anymore.
				this.eventHelper.RaiseSync<IOErrorEventArgs>(IOError, this, e);
		}

		/// <remarks>See remarks on top of MKY.IO.Ports.SerialPort.SerialPortEx why asynchronously is required.</remarks>
		[CallingContract(IsNeverMainThread = true)]
		protected virtual void OnIOErrorAsync(IOErrorEventArgs e)
		{
			if (IsUndisposed) // Ensure to not propagate event during closing anymore.
				this.eventHelper.RaiseAsync<IOErrorEventArgs>(IOError, this, e);
		}

		/// <summary></summary>
		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true)]
		protected virtual void OnDataReceived(DataReceivedEventArgs e)
		{
			if (IsUndisposed && IsOpen) // Make sure to propagate event only if active.
				this.eventHelper.RaiseSync<DataReceivedEventArgs>(DataReceived, this, e);
		}

		/// <summary></summary>
		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true)]
		protected virtual void OnDataSent(DataSentEventArgs e)
		{
			if (IsUndisposed && IsOpen) // Make sure to propagate event only if active.
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
			// AssertUndisposed() shall not be called from such basic method! Its return value may be needed for debugging. All underlying fields are still valid after disposal.

			return (ToPortName());
		}

		/// <summary></summary>
		public virtual string ToPortName()
		{
			// AssertUndisposed() shall not be called from such basic method! Its return value is needed for debugging! All underlying fields are still valid after disposal.

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

		/// <remarks>
		/// Name "DebugWriteLine" would show relation to <see cref="Debug.WriteLine(string)"/>.
		/// However, named "Message" for compactness and more clarity that something will happen
		/// with <paramref name="message"/>, and rather than e.g. "Common" for comprehensibility.
		/// </remarks>
		[Conditional("DEBUG")]
		protected virtual void DebugMessage(string message)
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

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
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
