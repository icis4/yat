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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;

using MKY.Contracts;
using MKY.Diagnostics;

#endregion

namespace MKY.IO.Ports
{
	/// <summary>
	/// Serial port component based on <see cref="System.IO.Ports.SerialPort"/>.
	/// </summary>
	/// <remarks>
	/// There are several issues with <see cref="System.IO.Ports.SerialPort"/>:
	/// <list type="bullet">
	/// <item><description>IOException issue</description></item>
	/// <item><description>ObjectDisposedException issue</description></item>
	/// <item><description>UnauthorizedAccessException and deadlock issue</description></item>
	/// </list>
	/// See ".\!-Doc\*.txt" for details.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Order according to meaning.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	[ToolboxBitmap(typeof(System.IO.Ports.SerialPort))]
	[DefaultProperty("PortName")]
	public partial class SerialPortEx : System.IO.Ports.SerialPort, ISerialPort
	{
		#region Static Fields
		//==========================================================================================
		// Static Fields
		//==========================================================================================

		private static Random staticRandom = new Random(RandomEx.NextPseudoRandomSeed());

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const string Undefined = "<Undefined>";

		/// <remarks>Must be constant (and not a readonly) to be usable as attribute argument.</remarks>
		private const string PortNameDefault = SerialPortId.FirstStandardPortName;
		private const int    PortIdDefault   = SerialPortId.FirstStandardPortNumber;

		private const string PortSettingsDefault = "9600, 8, None, 1, None";

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		/// <summary>
		/// A dedicated event helper to allow discarding exceptions when object got disposed.
		/// </summary>
		private EventHelper.Item eventHelper = EventHelper.CreateItem(typeof(SerialPortEx).FullName);

		/// <remarks>
		/// Required for patches to the 'ObjectDisposedException' issue described in <see cref="Close"/>.
		/// </remarks>
		private Stream baseStreamReferenceForCloseSafely;

		private SerialPortControlPinCount controlPinCount;

		private bool ignoreFramingErrors; // = false

		private bool inputBreak;       // = false
		private bool inputBreakSignal; // = false
		private object inputBreakSyncObj = new object();

		private int inputBreakCount;  // = 0
		private int outputBreakCount; // = 0

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
		/// Attention: No event is fired if the RTS or DTR line is changed.
		/// </remarks>
		public new event SerialPinChangedEventHandler PinChanged;

		//------------------------------------------------------------------------------------------
		// Events > Additional Events
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Event raised after port changed.
		/// </summary>
		public event EventHandler PortChanged;

		/// <summary>
		/// Event raised after port settings changed.
		/// </summary>
		public event EventHandler PortSettingsChanged;

		/// <summary>
		/// Event raised before connection is being opened.
		/// </summary>
		public event EventHandler Opening;

		/// <summary>
		/// Event raised after connection successfully opened.
		/// </summary>
		public event EventHandler Opened;

		/// <summary>
		/// Event raised before connection is being closed.
		/// </summary>
		public event EventHandler Closing;

		/// <summary>
		/// Event raised after connection successfully closed.
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
			base.DataReceived  += base_DataReceived;
			base.ErrorReceived += base_ErrorReceived;
			base.PinChanged    += base_PinChanged;
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public bool IsDisposed { get; protected set; }

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
		[SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "See remarks.")]
		~SerialPortEx()
		{
			DebugEventManagement.DebugWriteAllEventRemains(this);

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

		/// <summary>
		/// Gets or sets the port for communications, including but not limited to all available COM ports.
		/// </summary>
		[Category("Port")]
		[Description(@"Port name, e.g. ""COM1"".")]
		[DefaultValue(PortNameDefault)]
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
		/// Gets or sets the port for communications, including but not limited to all available COM ports.
		/// </summary>
		/// <exception cref="InvalidOperationException"> if ID is attempted to be changed while the port is open.</exception>
		[Category("Port")]
		[Description(@"Port ID, i.e. either port name (e.g. ""COM1"") or port number (e.g. ""1"").")]
		[DefaultValue(PortIdDefault)]
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
						throw (new InvalidOperationException("The serial COM port is already open, it must be stopped before changing the port ID!")); // Do not append 'MessageHelper.InvalidExecutionPreamble' as caller could rely on this exception text.

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
					OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.Rts));
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

				var settings = new SerialPortSettings();
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
				OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.Rts));
				OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.Dtr));
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the RTS/RTR (Request To Send/Ready To Receive)
		/// control pin is enabled during serial communication.
		/// </summary>
		/// <remarks>
		/// RTS/RTR is also known as RFR (Ready For Receiving).
		/// </remarks>
		/// <remarks>
		/// Attention:
		/// Different than <see cref="System.IO.Ports.SerialPort.RtsEnable()"/> this
		/// property fires an <see cref="PinChanged"/> event if the value changes.
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
							Interlocked.Increment(ref this.controlPinCount.RtsDisableCount);

						OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.Rts));
					}
				}
			}
		}

		/// <summary>
		/// Toggles the RTS/RTR (Request To Send/Ready To Receive) control pin.
		/// </summary>
		/// <remarks>
		/// RTS/RTR is also known as RFR (Ready For Receiving).
		/// </remarks>
		/// <returns>
		/// The new state of the RTS control pin.
		/// </returns>
		public virtual bool ToggleRts()
		{
			AssertNotDisposed();

			return (RtsEnable = !RtsEnable);
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

			return (DtrEnable = !DtrEnable);
		}

		/// <summary>
		/// Serial port control pins.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		public virtual SerialPortControlPins ControlPins
		{
			get
			{
				AssertNotDisposed();

				var pins = new SerialPortControlPins();

				try
				{
					if (HandshakeIsNotUsingRts)
						pins.Rts = RtsEnable; // 'RtsEnable' must not be accessed if it is used by the base class!
					else
						pins.Rts = true;

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
			OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.Rts));
			OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.Cts));
			OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.Dtr));
			OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.Dsr));
			OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.Dcd));
		}

		/// <summary>
		/// Gets or sets whether framing errors shall be ignored.
		/// </summary>
		public virtual bool IgnoreFramingErrors
		{
			get
			{
				AssertNotDisposed();

				return (this.ignoreFramingErrors);
			}
			set
			{
				AssertNotDisposed();

				this.ignoreFramingErrors = value;
			}
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
		/// Resets the break counts.
		/// </summary>
		public virtual void ResetBreakCount()
		{
			AssertNotDisposed();

			Interlocked.Exchange(ref this.inputBreakCount, 0);
			Interlocked.Exchange(ref this.outputBreakCount, 0);

			// Fire the event even though just the count changed.
			OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.InputBreak));
			OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.OutputBreak));
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
		[SuppressMessage("Microsoft.Usage", "CA1816:CallGCSuppressFinalizeCorrectly", Justification = "See comments...")]
		public new void Open()
		{
			AssertNotDisposed();

			if (!IsOpen)
			{
				OnOpening(EventArgs.Empty);

				try
				{
					DebugOpenClose("Trying base.Open()...");
					base.Open();
					DebugOpenClose("...done");
				}
				catch (Exception ex)
				{
					DebugEx.WriteException(GetType(), ex, "...failed!");
					throw; // Rethrow!
				}

				// --------------------------------------------------------------------------------
				// Patches to the 'ObjectDisposedException' issue described in the ".\!-Doc\*.txt".
				// --------------------------------------------------------------------------------

				// Immediately try to access the underlying stream:
				try
				{
					this.baseStreamReferenceForCloseSafely = BaseStream;

					DiscardInBuffer();
					DiscardOutBuffer();
				}
				catch (Exception ex)
				{
					if (this.baseStreamReferenceForCloseSafely == null)
					{
						var field = typeof(System.IO.Ports.SerialPort).GetField("internalSerialStream", BindingFlags.Instance | BindingFlags.NonPublic);
						if (field == null) // This will happen if the SerialPort class is changed in future versions of the .NET Framework.
						{
							DebugEx.WriteException(GetType(), ex, "Serious issue when trying to open the port! No longer able to retrieve internal stream using reflection!");
						}
						else
						{
							DebugEx.WriteException(GetType(), ex, "Failed to access the port! Safely disposing the internal stream using reflection.");
							TryToApplyEventLoopHandlerPatchAndCloseBaseStreamSafely((Stream)field.GetValue(this));
						////this.baseStreamReferenceForCloseSafely is already null.
						}
					}
					else
					{
						DebugEx.WriteException(GetType(), ex, "Failed to access the port! Safely disposing the internal stream.");
						TryToApplyEventLoopHandlerPatchAndCloseBaseStreamSafely(this.baseStreamReferenceForCloseSafely);
						this.baseStreamReferenceForCloseSafely = null;
					}

					throw; // Rethrow!
				}

				// Port successfully opened. Still suppress finalization of the underlying stream
				// while the port is open. It will later safely be re-registered by Close().
				GC.SuppressFinalize(BaseStream);

				// --------------------------------------------------------------------------------
				// End of patches to the 'ObjectDisposedException' issue.
				// --------------------------------------------------------------------------------

				// Invoke the additional events implemented by this class:
				OnOpened(EventArgs.Empty);
				OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.Rts));
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
		[Obsolete("This standard variant of Close() doesn't know the cirumstances. Use CloseNormally() or CloseAfterException() instead.")]
		public new void Close()
		{
			// AssertNotDisposed() is called by 'CloseAfterException()' below.

			CloseAfterException();
		}

		/// <summary>
		/// Closes the port according to documentation of <see cref="Close"/>.
		/// </summary>
		/// <remarks>
		/// This variant of <see cref="Close"/> shall be used when closing the port after a port
		/// related exception has happened, e.g. a <see cref="System.IO.IOException"/> after a
		/// device got physically disconnected. When closing the port intentionally in a
		/// "look-forward" manner, use <see cref="CloseNormally"/> instead.
		/// </remarks>
		public void CloseAfterException()
		{
			// AssertNotDisposed() is called by 'DoClose()' below.

			DoClose(true);
		}

		/// <summary>
		/// Closes the port according to documentation of <see cref="Close"/>.
		/// </summary>
		/// <remarks>
		/// This variant of <see cref="Close"/> shall be used when closing intentionally in a
		/// "look-forward" manner. When closing the port after a port related exception has
		/// happened, e.g. a <see cref="System.IO.IOException"/> after a device got physically
		/// disconnected, use <see cref="CloseAfterException"/> instead.
		/// </remarks>
		public void CloseNormally()
		{
			// AssertNotDisposed() is called by 'DoClose()' below.

			DoClose(false);
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		private void DoClose(bool isAfterException)
		{
			AssertNotDisposed();

			if (isAfterException || IsOpen) // Check 'isAfterException' first because the underlying port may have crashed and thus can no longer be accessed with 'IsOpen'.
			{
				OnClosing(EventArgs.Empty);

				// --------------------------------------------------------------------------------
				// Patches to the 'ObjectDisposedException' issue described in the ".\!-Doc\*.txt".
				// --------------------------------------------------------------------------------

				// Safely re-register finalization of the underlying stream:
				try
				{
					GC.ReRegisterForFinalize(BaseStream);
				}
				catch (Exception ex)
				{
					DebugEx.WriteException(GetType(), ex);
				}

				// Apply patch:
				TryToApplyEventLoopHandlerPatchAndCloseBaseStreamSafely(this.baseStreamReferenceForCloseSafely);
				this.baseStreamReferenceForCloseSafely = null;

				// Note that the patch must be applied in any case. Because with e.g. the Microsoft
				// driver, a device disconnect is not detected by .NET/YAT, and the user will have
				// to manually = intentionally close the port.
				// Corresponds to verification # 3.2. and 4.3. in ".\!-Doc\*.txt".

				// --------------------------------------------------------------------------------
				// End of patches to the 'ObjectDisposedException' issue.
				// --------------------------------------------------------------------------------

				try
				{
					DebugOpenClose("Trying base.Close()...");
					base.Close();
					DebugOpenClose("...done");
				}
				catch (Exception ex) // May be 'IOException' or 'ObjectDisposedException' or ...
				{
					DebugEx.WriteException(GetType(), ex, "...failed!");

					if (!isAfterException) // CloseNormally() shall be notified about exceptions.
						throw; // Rethrow!
					else
						Debug.WriteLine("Suppressing exception as 'isAfterException' has been signaled.");
				}

				// Invoke the additional events implemented by this class:
				OnClosed(EventArgs.Empty);
				OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.Rts));
				OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.Dtr));
				OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.OutputBreak));
			}
		}

		/// <summary>
		/// Safely disposes of the underlying stream even if a USB serial interface was physically
		/// removed from the system in a reliable manner based on https://pastebin.com/KmKEVzR8.
		/// </summary>
		/// <remarks>
		/// The <see cref="System.IO.Ports.SerialPort"/> class has 3 different problems in disposal
		/// in case of a USB serial device that is physically removed:
		///
		/// 1. The eventLoopRunner is asked to stop and <see cref="System.IO.Ports.SerialPort.IsOpen"/>
		/// returns false. Upon disposal this property is checked and closing  the internal serial
		/// stream is skipped, thus keeping the original handle open indefinitely (until the finalizer
		/// runs which leads to the next problem).
		///
		/// The solution for this one is to manually close the internal serial stream. We can get
		/// its reference by <see cref="System.IO.Ports.SerialPort.BaseStream" /> before the
		/// exception has happened or by reflection and getting the "internalSerialStream" field.
		///
		/// 2. Closing the internal serial stream throws an exception and closes the internal handle
		/// without waiting for its eventLoopRunner thread to finish, causing an uncatchable
		/// ObjectDisposedException from it later on when the finalizer runs (which oddly avoids
		/// throwing the exception but still fails to wait for the eventLoopRunner).
		///
		/// The solution is to manually ask the event loop runner thread to shutdown
		/// (via reflection) and waiting for it before closing the internal serial stream.
		///
		/// 3. Since Dispose throws exceptions, the finalizer is not suppressed.
		///
		/// The solution is to suppress their finalizers at the beginning.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop isn't able to skip URLs...")]
		protected static void TryToApplyEventLoopHandlerPatchAndCloseBaseStreamSafely(Stream baseStreamReference)
		{
			TryToShutdownBaseStreamEventLoopHandler(baseStreamReference);

			CloseBaseStreamSafely(baseStreamReference);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		protected static void CloseBaseStreamSafely(Stream baseStreamReference)
		{
			try
			{
				// Attention, the base stream is only available while the port is open!

				if (baseStreamReference != null)
				{
					// Attention, do not call Flush() !!!
					// It will block if the device is no longer available !!!

					baseStreamReference.Close();

					// Attention, do not call Dispose() !!!
					// It can throw after a call to Close() !!!
				}
			}
			catch (Exception ex) // May be 'IOException' or 'ObjectDisposedException' or ...
			{
				DebugEx.WriteException(typeof(SerialPortEx), ex, "Failed to close the underlying stream!");
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		protected static void TryToShutdownBaseStreamEventLoopHandler(Stream baseStreamReference)
		{
			if (baseStreamReference != null)
			{
				try
				{
					var eventRunnerField = baseStreamReference.GetType().GetField("eventRunner", BindingFlags.NonPublic | BindingFlags.Instance);
					if (eventRunnerField == null)
					{
						Debug.WriteLine("'System.IO.Ports.SerialPort' workaround has failed! Application may crash due to an 'ObjectDisposedException' in 'mscorlib'!");
					}
					else
					{
						var eventRunner = eventRunnerField.GetValue(baseStreamReference);
						var eventRunnerType = eventRunner.GetType();

						var endEventLoopFieldInfo            = eventRunnerType.GetField("endEventLoop", BindingFlags.Instance | BindingFlags.NonPublic);
						var eventLoopEndedSignalFieldInfo    = eventRunnerType.GetField("eventLoopEndedSignal", BindingFlags.Instance | BindingFlags.NonPublic);
						var waitCommEventWaitHandleFieldInfo = eventRunnerType.GetField("waitCommEventWaitHandle", BindingFlags.Instance | BindingFlags.NonPublic);

						if ((endEventLoopFieldInfo == null) ||
							(eventLoopEndedSignalFieldInfo == null) ||
							(waitCommEventWaitHandleFieldInfo == null))
						{
							Debug.WriteLine("'System.IO.Ports.SerialPort' workaround has failed! Unable to retrieve the 'EventLoopRunner'! Application may crash due to an 'ObjectDisposedException' in 'mscorlib'!");
						}
						else
						{
							var eventLoopEndedWaitHandle = (WaitHandle)eventLoopEndedSignalFieldInfo.GetValue(eventRunner);
							var waitCommEventWaitHandle = (ManualResetEvent)waitCommEventWaitHandleFieldInfo.GetValue(eventRunner);
							endEventLoopFieldInfo.SetValue(eventRunner, true);

							// Wait for the EventLoopRunner thread to finish. But sometimes the handler
							// resets the wait handle before exiting the loop and hangs (in case of USB
							// disconnect). In case it takes too long, brute-force it out of its wait by
							// setting the handle again and again:
							do
							{
								waitCommEventWaitHandle.Set();
							}
							while (!eventLoopEndedWaitHandle.WaitOne(staticRandom.Next(50, 200)));
						}
					}
				}
				catch (Exception ex)
				{
					DebugEx.WriteException(typeof(SerialPortEx), ex, "'System.IO.Ports.SerialPort' workaround has failed! Application may crash due to an 'ObjectDisposedException' in 'mscorlib'!");
				}
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

		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true, Rationale = "SerialPort.DataReceived: Only one event handler can execute at a time.")]
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

				// Note that this event is raised synchronously even though there is the deadlock
				// issue documented on top. Reason: This SerialPortEx shall behave the same as the
				// base implementation SerialPort. To work around the issue, a client class shall
				// ensure that these event are handled asynchronously.
			}

			OnDataReceived(new SerialDataReceivedEventArgs(e.EventType));

			// Note that this event is raised synchronously even though there is the deadlock
			// issue documented on top. Reason: This SerialPortEx shall behave the same as the
			// base implementation SerialPort. To work around the issue, a client class shall
			// ensure that these event are handled asynchronously.
		}

		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true, Rationale = "SerialPort.PinChanged: Only one event handler can execute at a time.")]
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

			// Note that this event is raised synchronously even though there is the deadlock
			// issue documented on top. Reason: This SerialPortEx shall behave the same as the
			// base implementation SerialPort. To work around the issue, a client class shall
			// ensure that these event are handled asynchronously.
		}

		private void base_ErrorReceived(object sender, System.IO.Ports.SerialErrorReceivedEventArgs e)
		{
			OnErrorReceived(new SerialErrorReceivedEventArgs(e.EventType));

			// Note that this event is raised synchronously even though there is the deadlock
			// issue documented on top. Reason: This SerialPortEx shall behave the same as the
			// base implementation SerialPort. To work around the issue, a client class shall
			// ensure that these event are handled asynchronously.
		}

		#endregion

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

		/// <summary>
		/// Invokes "PortChanged" event.
		/// </summary>
		protected virtual void OnPortChanged(EventArgs e)
		{
			if (!IsDisposed) // Make sure to propagate event only if not already disposed.
				this.eventHelper.RaiseSync(PortChanged, this, e);
		}

		/// <summary>
		/// Invokes "PortSettingsChanged" event.
		/// </summary>
		protected virtual void OnPortSettingsChanged(EventArgs e)
		{
			if (!IsDisposed) // Make sure to propagate event only if not already disposed.
				this.eventHelper.RaiseSync(PortSettingsChanged, this, e);
		}

		/// <summary>
		/// Invokes "Opening" event.
		/// </summary>
		protected virtual void OnOpening(EventArgs e)
		{
			if (!IsDisposed) // Make sure to propagate event only if not already disposed.
				this.eventHelper.RaiseSync(Opening, this, e);
		}

		/// <summary>
		/// Invokes "Opened" event.
		/// </summary>
		protected virtual void OnOpened(EventArgs e)
		{
			if (!IsDisposed) // Make sure to propagate event only if not already disposed.
				this.eventHelper.RaiseSync(Opened, this, e);
		}

		/// <summary>
		/// Invokes "Closing" event.
		/// </summary>
		protected virtual void OnClosing(EventArgs e)
		{
			if (!IsDisposed) // Make sure to propagate event only if not already disposed.
				this.eventHelper.RaiseSync(Closing, this, e);
		}

		/// <summary>
		/// Invokes "Closed" event.
		/// </summary>
		protected virtual void OnClosed(EventArgs e)
		{
			if (!IsDisposed) // Make sure to propagate event only if not already disposed.
				this.eventHelper.RaiseSync(Closed, this, e);
		}

		/// <summary>
		/// Invokes "DataReceived" event.
		/// </summary>
		protected virtual void OnDataReceived(SerialDataReceivedEventArgs e)
		{
			if (IsOpen) // Make sure to propagate event only if active!
				this.eventHelper.RaiseSync<SerialDataReceivedEventArgs, SerialDataReceivedEventHandler>(DataReceived, this, e);
		}

		/// <summary>
		/// Invokes "ErrorReceived" event.
		/// </summary>
		protected virtual void OnErrorReceived(SerialErrorReceivedEventArgs e)
		{
			if (IsOpen) // Make sure to propagate event only if active!
			{
				if ((e.EventType == System.IO.Ports.SerialError.Frame) && IgnoreFramingErrors)
				{
					// Ignore!
				}
				else
				{
					this.eventHelper.RaiseSync<SerialErrorReceivedEventArgs, SerialErrorReceivedEventHandler>(ErrorReceived, this, e);
				}
			}
		}

		/// <summary>
		/// Invokes "PinChanged" event.
		/// </summary>
		protected virtual void OnPinChanged(SerialPinChangedEventArgs e)
		{
			if (IsOpen) // Make sure to propagate event only if active!
				this.eventHelper.RaiseSync<SerialPinChangedEventArgs, SerialPinChangedEventHandler>(PinChanged, this, e);
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

		[Conditional("DEBUG_OPEN_CLOSE")]
		private void DebugOpenClose(string message)
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
