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

	// Enable debugging of open/close state:
////#define DEBUG_OPEN_CLOSE

#endif // DEBUG

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Threading;

using MKY.Diagnostics;

#endregion

namespace MKY.IO.Ports
{
	/// <summary>
	/// Serial port component based on <see cref="System.IO.Ports.SerialPort"/>.
	/// </summary>
	/// <remarks>
	/// Some detailed information on .NET implementation can be found at Kim Hamilton's post at
	/// http://www.innovatic.dk/knowledg/SerialCOM/SerialCOM.htm.
	/// 
	/// Note, there is a serious deadlock issue in <see cref="System.IO.Ports.SerialPort"/>.
	/// 
	/// Google for...
	///  > [UnauthorizedAccessException "access to the port"]
	///  > [ObjectDisposedException "safe handle has been closed"]
	/// ...for additional information and suggested workarounds.
	/// 
	/// This issue can be reproduced by 'TestDisconnectReconnectSerialPortExWithContinuousSending'
	/// implemented in 'MKY.IO.Ports.Test.SerialPort.ConnectionTest'. Note this is an 'Explicit'
	/// test as it requires to manually reset the sending device beause it will remain in continuous
	/// mode as well as the port device because it cannot be opened until disconnected/reconnected!
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
	/// Use cases 1 through 3 work fine. But use case 4 results in an exception. Workarounds tried
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
	/// 
	/// Saying hello to StyleCop ;-.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	[ToolboxBitmap(typeof(System.IO.Ports.SerialPort))]
	[DefaultProperty("PortName")]
	public partial class SerialPortEx : System.IO.Ports.SerialPort, ISerialPort
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const string PortSettingsDefault = "9600, 8, None, 1, None";

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isDisposed;

		private SerialPortControlPinCount controlPinCount;

		private bool inputBreak;
		private bool inputBreakSignal;
		private object inputBreakSyncObj = new object();

		private int outputBreakCount;
		private int inputBreakCount;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		//------------------------------------------------------------------------------------------
		// Events > Mapped SerialPort Events
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Represents the method that will handle the data received event
		/// of a <see cref="System.IO.Ports.SerialPort"/> object.
		/// </summary>
		public new event SerialDataReceivedEventHandler DataReceived;

		/// <summary>
		/// Represents the method that handles the error event
		/// of a <see cref="System.IO.Ports.SerialPort"/> object.
		/// </summary>
		public new event SerialErrorReceivedEventHandler ErrorReceived;

		/// <summary>
		/// Represents the method that will handle the serial pin changed event
		/// of a <see cref="System.IO.Ports.SerialPort"/> object.
		/// </summary>
		/// <remarks>
		/// Attention: No event is fired if the RFR or DTR line is changed.
		/// </remarks>
		public new event SerialPinChangedEventHandler PinChanged;

		//------------------------------------------------------------------------------------------
		// Events > Additional Events
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Fired after port changed.
		/// </summary>
		public event EventHandler PortChanged;

		/// <summary>
		/// Fired after port settings changed.
		/// </summary>
		public event EventHandler PortSettingsChanged;

		/// <summary>
		/// Fired before connection is being opened.
		/// </summary>
		public event EventHandler Opening;

		/// <summary>
		/// Fired after connection successfully opened.
		/// </summary>
		public event EventHandler Opened;

		/// <summary>
		/// Fired before connection is being closed.
		/// </summary>
		public event EventHandler Closing;

		/// <summary>
		/// Fired after connection successfully closed.
		/// </summary>
		public event EventHandler Closed;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary>
		/// Initializes a new instance of the <see cref="SerialPortEx"/> class.
		/// </summary>
		public SerialPortEx()
		{
			InitializeComponent();
			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SerialPortEx"/> class using the specified
		/// <see cref="IContainer"/> object.
		/// </summary>
		public SerialPortEx(IContainer container)
			: base(container)
		{
			InitializeComponent();
			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SerialPortEx"/> class using the specified
		/// port name.
		/// </summary>
		/// <param name="portName">
		/// The port to use (for example, COM1).
		/// </param>
		public SerialPortEx(string portName)
		{
			PortName = portName;
			InitializeComponent();
			Initialize();
		}

		private void Initialize()
		{
			base.DataReceived  += new System.IO.Ports.SerialDataReceivedEventHandler (base_DataReceived);
			base.ErrorReceived += new System.IO.Ports.SerialErrorReceivedEventHandler(base_ErrorReceived);
			base.PinChanged    += new System.IO.Ports.SerialPinChangedEventHandler   (base_PinChanged);
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

#if (DEBUG)

		/// <remarks>
		/// Microsoft.Design rule CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable requests
		/// "Types that declare disposable members should also implement IDisposable. If the type
		///  does not own any unmanaged resources, do not implement a finalizer on it."
		/// 
		/// Well, true for best performance on finalizing. However, it's not easy to find missing
		/// calls to <see cref="Component.Dispose()"/>. In order to detect such missing calls, the
		/// finalizer is kept for DEBUG, indicating missing calls.
		/// 
		/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
		/// </remarks>
		~SerialPortEx()
		{
			EventManagementHelper.DebugNotifyAllEventRemains(this);

			Dispose(false);

			DisposalHelper.DebugNotifyFinalizerInsteadOfDispose(this);
		}

#endif // DEBUG

		/// <summary>
		/// Returns whether the object has already been disposed.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

		/// <summary>
		/// Gets or sets the port for communications, including but not limited to all available COM ports.
		/// </summary>
		[Category("Port")]
		[Description(@"Port name, e.g. ""COM1"".")]
		[DefaultValue(SerialPortId.FirstStandardPortName)]
		public new string PortName
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (base.PortName);
			}
			set
			{
				AssertNotDisposed();

				if (base.PortName != value)
				{
					base.PortName = value;
					OnPortChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Communications port (e.g. COM1).
		/// </summary>
		[Category("Port")]
		[Description("Port ID.")]
		[DefaultValue(SerialPortId.FirstStandardPortNumber)]
		[TypeConverter(typeof(SerialPortIdConverter))]
		public virtual SerialPortId PortId
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (new SerialPortId(base.PortName));
			}
			set
			{
				AssertNotDisposed();

				if (base.PortName != value.Name)
				{
					if (IsOpen)
						throw (new InvalidOperationException("The serial COM port is already open, it must be stopped before changing the port ID!"));

					base.PortName = value.Name;
					OnPortChanged(EventArgs.Empty);
					OnPortSettingsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the serial baud rate.
		/// </summary>
		[Category("Port")]
		[Description("Baud rate.")]
		[DefaultValue(BaudRateEx.Default)]
		public new int BaudRate
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (base.BaudRate);
			}
			set
			{
				AssertNotDisposed();

				if (base.BaudRate != value)
				{
					base.BaudRate = value;
					OnPortSettingsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the standard length of data bits per byte.
		/// </summary>
		[Category("Port")]
		[Description("Data bits.")]
		[DefaultValue(DataBitsEx.Default)]
		public new int DataBits
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (base.DataBits);
			}
			set
			{
				AssertNotDisposed();

				if (base.DataBits != value)
				{
					base.DataBits = value;
					OnPortSettingsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the parity-checking protocol.
		/// </summary>
		[Category("Port")]
		[Description("Parity")]
		[DefaultValue(ParityEx.Default)]
		public new System.IO.Ports.Parity Parity
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (base.Parity);
			}
			set
			{
				AssertNotDisposed();

				if (base.Parity != value)
				{
					base.Parity = value;
					OnPortSettingsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the standard number of stop bits per byte.
		/// </summary>
		[Category("Port")]
		[Description("Stop bits.")]
		[DefaultValue(StopBitsEx.Default)]
		public new System.IO.Ports.StopBits StopBits
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (base.StopBits);
			}
			set
			{
				AssertNotDisposed();

				if (base.StopBits != value)
				{
					base.StopBits = value;
					OnPortSettingsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the handshaking protocol for serial port transmission of data.
		/// </summary>
		[Category("Port")]
		[Description("Handshake.")]
		[DefaultValue(HandshakeEx.Default)]
		public new System.IO.Ports.Handshake Handshake
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (base.Handshake);
			}
			set
			{
				AssertNotDisposed();

				if (base.Handshake != value)
				{
					base.Handshake = value;
					OnPortSettingsChanged(EventArgs.Empty);
					OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.Rfr));
					OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.Dtr));
				}
			}
		}

		private bool HandshakeIsNotUsingRts
		{
			get
			{
				return ((Handshake != System.IO.Ports.Handshake.RequestToSend) &&
						(Handshake != System.IO.Ports.Handshake.RequestToSendXOnXOff));
			}
		}

		/// <summary>
		/// Communications port settings.
		/// </summary>
		[Category("Port")]
		[Description("Port settings. Default settings are '" + PortSettingsDefault + "'.")]
		[DefaultValue(PortSettingsDefault)]
		[TypeConverter(typeof(SerialPortSettingsConverter))]
		public virtual SerialPortSettings PortSettings
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				SerialPortSettings settings = new SerialPortSettings();
				settings.BaudRate  = (BaudRateEx) base.BaudRate;
				settings.DataBits  = (DataBitsEx) base.DataBits;
				settings.Parity    = (ParityEx)   base.Parity;
				settings.StopBits  = (StopBitsEx) base.StopBits;
				settings.Handshake = (HandshakeEx)base.Handshake;

				return (settings);
			}
			set
			{
				AssertNotDisposed();

				base.BaudRate  = (BaudRateEx) value.BaudRate;
				base.DataBits  = (DataBitsEx) value.DataBits;
				base.Parity    = (ParityEx)   value.Parity;
				base.StopBits  = (StopBitsEx) value.StopBits;
				base.Handshake = (HandshakeEx)value.Handshake;

				OnPortSettingsChanged(EventArgs.Empty);
				OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.Rfr));
				OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.Dtr));
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the RTS (Request To Send) signal
		/// is enabled during serial communication.
		/// </summary>
		/// <remarks>
		/// Attention:
		/// Different than <see cref="System.IO.Ports.SerialPort.RtsEnable()"/> this
		/// property fires an <see cref="PinChanged"/> event if the value changes.
		/// 
		/// Also note that there is a synonym <see cref="RfrEnable"/> which now is
		/// the official name of the RTS signal.
		/// </remarks>
		public new bool RtsEnable
		{
			get
			{
				AssertNotDisposed();

				// Needed to prevent System.InvalidOperationException in case RTS is in use.
				if (HandshakeIsNotUsingRts)
					return (base.RtsEnable);
				else
					return (true);
			}
			set
			{
				AssertNotDisposed();

				// Needed to prevent System.InvalidOperationException in case RTS is in use.
				if (HandshakeIsNotUsingRts)
				{
					if (base.RtsEnable != value)
					{
						base.RtsEnable = value;

						if (!base.RtsEnable)
							Interlocked.Increment(ref this.controlPinCount.RfrDisableCount);

						OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.Rfr));
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the RFR (Ready For Receiving) signal
		/// is enabled during serial communication.
		/// </summary>
		/// <remarks>
		/// Note that there is a synonym <see cref="RtsEnable"/> which is the former
		/// name of the RFR signal.
		/// </remarks>
		public bool RfrEnable
		{
			get { return (RtsEnable); }
			set { RtsEnable = value;  }
		}

		/// <summary>
		/// Toggles the RFR (Ready For Receiving) control pin. This pin was formerly called RTS (Request To Send).
		/// </summary>
		/// <returns>
		/// The new state of the RFR control pin.
		/// </returns>
		public virtual bool ToggleRfr()
		{
			AssertNotDisposed();

			return (this.RfrEnable = !this.RfrEnable);
		}

		/// <summary>
		/// Gets or sets a value that enables the Data Terminal Ready (DTR) signal
		/// during serial communication.
		/// </summary>
		/// <remarks>
		/// Attention: Different than <see cref="System.IO.Ports.SerialPort.DtrEnable()"/>
		/// this property fires an <see cref="PinChanged"/> event if the value changes.
		/// </remarks>
		public new bool DtrEnable
		{
			get
			{
				AssertNotDisposed();

				return (base.DtrEnable);
			}
			set
			{
				AssertNotDisposed();

				if (base.DtrEnable != value)
				{
					base.DtrEnable = value;

					if (!base.DtrEnable)
						Interlocked.Increment(ref this.controlPinCount.DtrDisableCount);

					OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.Dtr));
				}
			}
		}

		/// <summary>
		/// Toggles the DTR (Data Terminal Ready) control pin.
		/// </summary>
		/// <returns>
		/// The new state of the DTR control pin.
		/// </returns>
		public virtual bool ToggleDtr()
		{
			AssertNotDisposed();

			return (this.DtrEnable = !this.DtrEnable);
		}

		/// <summary>
		/// Serial port control pins.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		public virtual SerialPortControlPins ControlPins
		{
			get
			{
				AssertNotDisposed();

				SerialPortControlPins pins = new SerialPortControlPins();

				try
				{
					if (HandshakeIsNotUsingRts)
						pins.Rfr = RfrEnable; // 'RfrEnable' must not be accessed if it is used by the base class!
					else
						pins.Rfr = true;

					pins.Dtr = DtrEnable;

					if (IsOpen) // Ensure that incoming signals are only retrieved while port is open.
					{
						pins.Cts = CtsHolding;
						pins.Dsr = DsrHolding;
						pins.Dcd = CDHolding;
					}
				}
				catch (Exception ex)
				{
					DebugEx.WriteException(GetType(), ex, "Exception while accessing pins!");
				}

				return (pins);
			}
		}

		/// <summary>
		/// Serial port control pin counts.
		/// </summary>
		public virtual SerialPortControlPinCount ControlPinCount
		{
			get
			{
				AssertNotDisposed();

				return (this.controlPinCount);
			}
		}

		/// <summary>
		/// Resets the control pin counts.
		/// </summary>
		public virtual void ResetControlPinCount()
		{
			AssertNotDisposed();

			this.controlPinCount.Reset();

			// Fire the event even though just the count changed.
			OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.Rfr));
			OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.Cts));
			OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.Dtr));
			OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.Dsr));
			OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.Dcd));
		}

		/// <summary>
		/// Gets the input break state.
		/// </summary>
		public virtual bool InputBreak
		{
			get
			{
				AssertNotDisposed();

				lock (this.inputBreakSyncObj)
					return (this.inputBreak);
			}
		}

		/// <summary>
		/// Gets or sets the output break state.
		/// </summary>
		public virtual bool OutputBreak
		{
			get
			{
				AssertNotDisposed();

				return (BreakState);
			}
			set
			{
				AssertNotDisposed();

				if (BreakState != value)
				{
					BreakState = value;

					if (!BreakState)
						Interlocked.Increment(ref this.outputBreakCount);

					OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.OutputBreak));
				}
			}
		}

		/// <summary>
		/// Toggles the output break state.
		/// </summary>
		public virtual void ToggleOutputBreak()
		{
			AssertNotDisposed();

			OutputBreak = !OutputBreak;
		}

		/// <summary>
		/// Returns the number of output breaks.
		/// </summary>
		public virtual int OutputBreakCount
		{
			get
			{
				AssertNotDisposed();

				return (this.outputBreakCount);
			}
		}

		/// <summary>
		/// Returns the number of input breaks.
		/// </summary>
		public virtual int InputBreakCount
		{
			get
			{
				AssertNotDisposed();

				return (this.inputBreakCount);
			}
		}

		/// <summary>
		/// Resets the break counts.
		/// </summary>
		public virtual void ResetBreakCount()
		{
			AssertNotDisposed();

			Interlocked.Exchange(ref this.outputBreakCount, 0);
			Interlocked.Exchange(ref this.inputBreakCount, 0);

			// Fire the event even though just the count changed.
			OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.OutputBreak));
			OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.InputBreak));
		}

		#endregion

		#region Open/Close
		//==========================================================================================
		// Open/Close
		//==========================================================================================

		/// <summary>
		/// Opens a new serial port connection.
		/// </summary>
		/// <exception cref="System.UnauthorizedAccessException">
		/// Access is denied to the port.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// The port name does not begin with "COM". - or - The file type of the port
		/// is not supported.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// The port is in an invalid state. - or - An attempt to set the state of
		/// the underlying port failed. For example, the parameters passed from this
		/// <see cref="System.IO.Ports.SerialPort"/> object were invalid.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// One or more of the properties for this instance are invalid. For example,
		/// the <see cref="System.IO.Ports.SerialPort.Parity"/>,
		/// <see cref="System.IO.Ports.SerialPort.DataBits"/>, or
		/// <see cref="System.IO.Ports.SerialPort.Handshake"/> properties are not valid
		/// values; the <see cref="System.IO.Ports.SerialPort.BaudRate"/> is less than or
		/// equal to zero; the <see cref="System.IO.Ports.SerialPort.ReadTimeout"/> or
		/// <see cref="System.IO.Ports.SerialPort.WriteTimeout"/> property is less than
		/// zero and is not <see cref="System.IO.Ports.SerialPort.InfiniteTimeout"/>.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The specified port is open.
		/// </exception>
		public new void Open()
		{
			AssertNotDisposed();

			if (!IsOpen)
			{
				OnOpening(EventArgs.Empty);

#if (DEBUG_OPEN_CLOSE)
				try
				{
					DebugWrite("Trying base.Open()...", true);
					base.Open();
					DebugWrite("...OK.");
				}
				catch (Exception ex)
				{
					DebugWrite("...exception!");
					DebugWrite(ex.Message);
					throw; // Re-throw!
				}
#else
				base.Open();
#endif
				// This is a fix to the 'ObjectDisposedException' issue described in several locations:
				// http://stackoverflow.com/questions/3808885/net-4-serial-port-objectdisposedexception-on-windows-7-only
				// http://stackoverflow.com/questions/3230311/problem-with-serialport
				//
				// Details on this exception:
				// Type: System.ObjectDisposedException
				// Message: Safe handle has been closed
				// Source: mscorlib
				// Stack:
				//    at System.StubHelpers.StubHelpers.SafeHandleC2NHelper(Object pThis, IntPtr pCleanupWorkList)
				//    at Microsoft.Win32.UnsafeNativeMethods.GetOverlappedResult(SafeFileHandle hFile, NativeOverlapped * lpOverlapped, Int32 & lpNumberOfBytesTransferred, Boolean bWait)
				//    at System.IO.Ports.SerialStream.EventLoopRunner.WaitForCommEvent()
				//    at System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state)
				//    at System.Threading.ThreadHelper.ThreadStart()
				//
				// It suppresses finalization of the underlying base stream while the port is open.
				// It will later safely be re-registered by Close().
				GC.SuppressFinalize(this.BaseStream);

				// Invoke the additional events implemented by this class:
				OnOpened(EventArgs.Empty);
				OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.Rfr));
				OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.Dtr));
				OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.OutputBreak));

				// Immediately check whether there is already data pending:
				if (BytesToRead > 0)
					OnDataReceived(new SerialDataReceivedEventArgs(System.IO.Ports.SerialData.Chars));
			}
		}

		/// <summary>
		/// Waits for unwritten data to be sent.
		/// </summary>
		public virtual void Flush()
		{
			AssertNotDisposed();

			if (IsOpen)
			{
				try
				{
					while (BytesToWrite > 0)
						Thread.Sleep(TimeSpan.Zero);

					// Alternatively, base.BaseStream.Flush() could be called.
					// But that approach doesn't offer flexibility, so no-go.
				}
				catch (Exception ex)
				{
					throw (new System.IO.IOException(ex.Message));
				}
			}
		}

		/// <summary>
		/// Closes the port connection, sets the
		/// <see cref="System.IO.Ports.SerialPort.IsOpen"/> property to false,
		/// and disposes of the internal <see cref="System.IO.Stream"/> object.
		/// </summary>
		/// <remarks>
		/// Immediately closes the connection. Call <see cref="Flush"/> prior to this
		/// method to make sure unwritten data is sent.
		/// </remarks>
		/// <exception cref="System.InvalidOperationException">
		/// The specified port is not open.
		/// </exception>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		public new void Close()
		{
			AssertNotDisposed();

			if (IsOpen)
			{
				OnClosing(EventArgs.Empty);

				// This is a fix to the 'ObjectDisposedException' issue described in several locations:
				// http://stackoverflow.com/questions/3808885/net-4-serial-port-objectdisposedexception-on-windows-7-only
				// http://stackoverflow.com/questions/3230311/problem-with-serialport
				//
				// Details on this exception:
				// Type: System.ObjectDisposedException
				// Message: Safe handle has been closed
				// Source: mscorlib
				// Stack:
				//    at System.StubHelpers.StubHelpers.SafeHandleC2NHelper(Object pThis, IntPtr pCleanupWorkList)
				//    at Microsoft.Win32.UnsafeNativeMethods.GetOverlappedResult(SafeFileHandle hFile, NativeOverlapped * lpOverlapped, Int32 & lpNumberOfBytesTransferred, Boolean bWait)
				//    at System.IO.Ports.SerialStream.EventLoopRunner.WaitForCommEvent()
				//    at System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state)
				//    at System.Threading.ThreadHelper.ThreadStart()
				//
				// It suppresses finalization of the underlying base stream while the port is open.
				// It is here safely re-registered.
				try
				{
					GC.ReRegisterForFinalize(this.BaseStream);
				}
				catch (Exception ex)
				{
					DebugEx.WriteException(GetType(), ex);
				}

#if (DEBUG_OPEN_CLOSE)
				try
				{
					DebugWrite("Trying base.Close()...", true);
					base.Close();
					DebugWrite("...OK.");
				}
				catch (Exception ex)
				{
					DebugWrite("...exception!");
					DebugWrite(ex.Message);
					throw; // Re-throw!
				}
#else
				base.Close();
#endif
				// Invoke the additional events implemented by this class:
				OnClosed(EventArgs.Empty);
				OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.Rfr));
				OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.Dtr));
				OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.OutputBreak));
			}
		}

		#endregion

		#region Write
		//==========================================================================================
		// Write
		//==========================================================================================

		/// <summary>
		/// Writes the specified byte to an output buffer at the specified offset.
		/// </summary>
		/// <param name="data">The byte to write the output to.</param>
		/// <exception cref="System.TimeoutException">
		/// The operation did not complete before the time-out period ended.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The specified port is not open.
		/// </exception>
		public virtual void WriteByte(byte data)
		{
			Write(new byte[] { data }, 0, 1);
		}

		/// <summary>
		/// Writes the specified character to an output buffer at the specified offset.
		/// </summary>
		/// <param name="data">The byte to write the output to.</param>
		/// <exception cref="System.TimeoutException">
		/// The operation did not complete before the time-out period ended.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The specified port is not open.
		/// </exception>
		public virtual void WriteChar(char data)
		{
			Write(new char[] { data }, 0, 1);
		}

		#endregion

		#region Base Event Handling
		//==========================================================================================
		// Base Event Handling
		//==========================================================================================

		private void base_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
		{
			// Improve performance by only locking if input break is active.
			if (this.inputBreak)
			{
				lock (this.inputBreakSyncObj)
				{
					if (this.inputBreakSignal)
						this.inputBreakSignal = false; // Signal input break once and then restore signal.
					else
						this.inputBreak = false; // Restore input break if data has been received successfully.
				}
				OnPinChanged(new SerialPinChangedEventArgs((SerialPinChange)e.EventType));
			}

			OnDataReceived(new SerialDataReceivedEventArgs(e.EventType));
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that any exception leads to restart or reset of port.")]
		private void base_PinChanged(object sender, System.IO.Ports.SerialPinChangedEventArgs e)
		{
			try // Access to pins may lead to exceptions when port is about to be closed.
			{
				switch (e.EventType)
				{
					case System.IO.Ports.SerialPinChange.CtsChanged:
					{
						if (IsOpen && !CtsHolding) // Count inactive signals!
							Interlocked.Increment(ref this.controlPinCount.CtsDisableCount);

						break;
					}

					case System.IO.Ports.SerialPinChange.DsrChanged:
					{
						if (IsOpen && !DsrHolding) // Count inactive signals!
							Interlocked.Increment(ref this.controlPinCount.DsrDisableCount);

						break;
					}

					case System.IO.Ports.SerialPinChange.CDChanged:
					{
						if (IsOpen && CDHolding)
							Interlocked.Increment(ref this.controlPinCount.DcdCount);

						break;
					}

					case System.IO.Ports.SerialPinChange.Break:
					{
						lock (this.inputBreakSyncObj)
						{
							this.inputBreak = true;
							this.inputBreakSignal = true;
						}

						Interlocked.Increment(ref this.inputBreakCount);

						break;
					}
				}
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(GetType(), ex, "Exception while accessing pins!");
			}

			OnPinChanged(new SerialPinChangedEventArgs((SerialPinChange)e.EventType));
		}

		private void base_ErrorReceived(object sender, System.IO.Ports.SerialErrorReceivedEventArgs e)
		{
			OnErrorReceived(new SerialErrorReceivedEventArgs(e.EventType));
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		/// <summary>
		/// Invokes "PortChanged" event.
		/// </summary>
		protected virtual void OnPortChanged(EventArgs e)
		{
			EventHelper.FireSync(PortChanged, this, e);
		}

		/// <summary>
		/// Invokes "PortSettingsChanged" event.
		/// </summary>
		protected virtual void OnPortSettingsChanged(EventArgs e)
		{
			EventHelper.FireSync(PortSettingsChanged, this, e);
		}

		/// <summary>
		/// Invokes "Opening" event.
		/// </summary>
		protected virtual void OnOpening(EventArgs e)
		{
			EventHelper.FireSync(Opening, this, e);
		}

		/// <summary>
		/// Invokes "Opened" event.
		/// </summary>
		protected virtual void OnOpened(EventArgs e)
		{
			EventHelper.FireSync(Opened, this, e);
		}

		/// <summary>
		/// Invokes "Closing" event.
		/// </summary>
		protected virtual void OnClosing(EventArgs e)
		{
			EventHelper.FireSync(Closing, this, e);
		}

		/// <summary>
		/// Invokes "Closed" event.
		/// </summary>
		protected virtual void OnClosed(EventArgs e)
		{
			EventHelper.FireSync(Closed, this, e);
		}

		/// <summary>
		/// Invokes "DataReceived" event.
		/// </summary>
		protected virtual void OnDataReceived(SerialDataReceivedEventArgs e)
		{
			if (!IsDisposed && IsOpen) // Make sure to propagate event only if port is active.
				EventHelper.FireSync<SerialDataReceivedEventArgs, SerialDataReceivedEventHandler>(DataReceived, this, e);
		}

		/// <summary>
		/// Invokes "ErrorReceived" event.
		/// </summary>
		protected virtual void OnErrorReceived(SerialErrorReceivedEventArgs e)
		{
			if (!IsDisposed && IsOpen) // Make sure to propagate event only if port is active.
				EventHelper.FireSync<SerialErrorReceivedEventArgs, SerialErrorReceivedEventHandler>(ErrorReceived, this, e);
		}

		/// <summary>
		/// Invokes "PinChanged" event.
		/// </summary>
		protected virtual void OnPinChanged(SerialPinChangedEventArgs e)
		{
			if (!IsDisposed && IsOpen) // Make sure to propagate event only if port is active.
				EventHelper.FireSync<SerialPinChangedEventArgs, SerialPinChangedEventHandler>(PinChanged, this, e);
		}

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

#if (DEBUG_OPEN_CLOSE)

		private string DebugWrite_portName = "";

		private void DebugWrite(string message)
		{
			DebugWrite(message, false);
		}

		private void DebugWrite(string message, bool writeStack)
		{
			if (DebugWrite_portName.Length == 0)
				DebugWrite_portName = PortName;

			string completeMessage = DebugWrite_portName + " " + Environment.TickCount + " " + message;

			if (writeStack)
				DebugEx.WriteStack(this, completeMessage);
			else
				DebugEx.WriteLine(this, completeMessage);
		}

#endif // DEBUG_OPEN_CLOSE

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
