// Choose whether additional debug output should be written on open/close
//#define WRITE_DEBUG

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using MKY.Utilities.Event;

namespace MKY.IO.Ports
{
	/// <summary>
	/// Serial port component based on <see cref="System.IO.Ports.SerialPort"/>.
	/// </summary>
	[System.Drawing.ToolboxBitmap(typeof(System.IO.Ports.SerialPort))]
	[DefaultProperty("PortName")]
	public partial class SerialPortDotNet : System.IO.Ports.SerialPort, ISerialPort
	{
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

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const int _BaudRateDefault = 9600;
		private const int _DataBitsDefault = 8;
		private const System.IO.Ports.Parity _ParityDefault = System.IO.Ports.Parity.None;
		private const System.IO.Ports.StopBits _StopBitsDefault = System.IO.Ports.StopBits.One;
		private const System.IO.Ports.Handshake _HandshakeDefault = System.IO.Ports.Handshake.None;
		private const string _PortSettingsDefault = "9600, 8, None, 1, None";

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public SerialPortDotNet()
		{
			InitializeComponent();
			Initialize();
		}

		/// <summary></summary>
		public SerialPortDotNet(IContainer container)
		{
			container.Add(this);
			InitializeComponent();
			Initialize();
		}

		private void Initialize()
		{
			base.DataReceived  += new System.IO.Ports.SerialDataReceivedEventHandler(base_DataReceived);
			base.ErrorReceived += new System.IO.Ports.SerialErrorReceivedEventHandler(base_ErrorReceived);
			base.PinChanged    += new System.IO.Ports.SerialPinChangedEventHandler(base_PinChanged);
		}

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
		[DefaultValue(SerialPortId.DefaultPortName)]
		public new string PortName
		{
			get
			{
				AssertNotDisposed();
				return (base.PortName);
			}
			set
			{
				AssertNotDisposed();
				if (base.PortName != value)
				{
					base.PortName = value;
					OnPortChanged(new EventArgs());
				}
			}
		}

		/// <summary>
		/// Communications port (e.g. COM1).
		/// </summary>
		[Category("Port")]
		[Description("Port ID.")]
		[DefaultValue(SerialPortId.DefaultPortNumber)]
		[TypeConverter(typeof(IO.Ports.SerialPortIdConverter))]
		public virtual SerialPortId PortId
		{
			get
			{
				AssertNotDisposed();
				return (new SerialPortId(base.PortName));
			}
			set
			{
				AssertNotDisposed();

				if (base.PortName != value.Name)
				{
					if (base.IsOpen)
						throw (new System.InvalidOperationException("The specified port is open."));

					base.PortName = value.Name;
					OnPortChanged(new EventArgs());
					OnPortSettingsChanged(new EventArgs());
				}
			}
		}

		/// <summary>
		/// Gets or sets the serial baud rate.
		/// </summary>
		[Category("Port")]
		[Description("Baud rate.")]
		[DefaultValue(_BaudRateDefault)]
		public new int BaudRate
		{
			get
			{
				AssertNotDisposed();
				return (base.BaudRate);
			}
			set
			{
				AssertNotDisposed();
				if (base.BaudRate != value)
				{
					base.BaudRate = value;
					OnPortSettingsChanged(new EventArgs());
				}
			}
		}

		/// <summary>
		/// Gets or sets the standard length of data bits per byte.
		/// </summary>
		[Category("Port")]
		[Description("Data bits.")]
		[DefaultValue(_DataBitsDefault)]
		public new int DataBits
		{
			get
			{
				AssertNotDisposed();
				return (base.DataBits);
			}
			set
			{
				AssertNotDisposed();
				if (base.DataBits != value)
				{
					base.DataBits = value;
					OnPortSettingsChanged(new EventArgs());
				}
			}
		}

		/// <summary>
		/// Gets or sets the parity-checking protocol.
		/// </summary>
		[Category("Port")]
		[Description("Parity")]
		[DefaultValue(_ParityDefault)]
		public new System.IO.Ports.Parity Parity
		{
			get
			{
				AssertNotDisposed();
				return (base.Parity);
			}
			set
			{
				AssertNotDisposed();
				if (base.Parity != value)
				{
					base.Parity = value;
					OnPortSettingsChanged(new EventArgs());
				}
			}
		}

		/// <summary>
		/// Gets or sets the standard number of stopbits per byte.
		/// </summary>
		[Category("Port")]
		[Description("Stop bits.")]
		[DefaultValue(_StopBitsDefault)]
		public new System.IO.Ports.StopBits StopBits
		{
			get
			{
				AssertNotDisposed();
				return (base.StopBits);
			}
			set
			{
				AssertNotDisposed();
				if (base.StopBits != value)
				{
					base.StopBits = value;
					OnPortSettingsChanged(new EventArgs());
				}
			}
		}

		/// <summary>
		/// Gets or sets the handshaking protocol for serial port transmission of data.
		/// </summary>
		[Category("Port")]
		[Description("Handshake.")]
		[DefaultValue(_HandshakeDefault)]
		public new System.IO.Ports.Handshake Handshake
		{
			get
			{
				AssertNotDisposed();
				return (base.Handshake);
			}
			set
			{
				AssertNotDisposed();
				if (base.Handshake != value)
				{
					base.Handshake = value;
					OnPortSettingsChanged(new EventArgs());
					OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.RtsChanged));
					OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.DtrChanged));
				}
			}
		}

		private bool HandshakeIsNotUsingRequestToSend
		{
			get
			{
				return ((base.Handshake != System.IO.Ports.Handshake.RequestToSend) &&
						(base.Handshake != System.IO.Ports.Handshake.RequestToSendXOnXOff));
			}
		}

		/// <summary>
		/// Communications port settings.
		/// </summary>
		[Category("Port")]
		[Description(@"Port settings. Default settings are """ + _PortSettingsDefault + @""".")]
		[DefaultValue(_PortSettingsDefault)]
		[TypeConverter(typeof(IO.Ports.SerialPortSettingsConverter))]
		public virtual SerialPortSettings PortSettings
		{
			get
			{
				AssertNotDisposed();

				SerialPortSettings settings = new SerialPortSettings();
				settings.BaudRate = (XBaudRate)base.BaudRate;
				settings.DataBits = (XDataBits)base.DataBits;
				settings.Parity = (XParity)base.Parity;
				settings.StopBits = (XStopBits)base.StopBits;
				settings.Handshake = (XHandshake)base.Handshake;

				return (settings);
			}
			set
			{
				AssertNotDisposed();

				base.BaudRate = (XBaudRate)value.BaudRate;
				base.DataBits = (XDataBits)value.DataBits;
				base.Parity = (XParity)value.Parity;
				base.StopBits = (XStopBits)value.StopBits;
				base.Handshake = (XHandshake)value.Handshake;

				OnPortSettingsChanged(new EventArgs());
				OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.RtsChanged));
				OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.DtrChanged));
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the Request to Send (RTS) signal
		/// is enabled during serial communication.
		/// </summary>
		/// <remarks>
		/// Attention: Different than <see cref="System.IO.Ports.SerialPort.RtsEnable()"/>
		/// this property fires an <see cref="PinChanged"/> event if the value changes.
		/// </remarks>
		public new bool RtsEnable
		{
			get
			{
				AssertNotDisposed();

				if (HandshakeIsNotUsingRequestToSend)
					return (base.RtsEnable);
				else
					return (true);
			}
			set
			{
				AssertNotDisposed();

				if (HandshakeIsNotUsingRequestToSend)
				{
					if (base.RtsEnable != value)
					{
						base.RtsEnable = value;
						OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.RtsChanged));
					}
				}
			}
		}

		/// <summary>
		/// Toggles the RTS (Request To Send) control line.
		/// </summary>
		public virtual void ToggleRts()
		{
			AssertNotDisposed();

			if (HandshakeIsNotUsingRequestToSend)
			{
				this.RtsEnable = !this.RtsEnable;
				OnPinChanged(new SerialPinChangedEventArgs());
			}
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
					OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.DtrChanged));
				}
			}
		}

		/// <summary>
		/// Toggles the DTR (Data Terminal Ready) control line.
		/// </summary>
		public virtual void ToggleDtr()
		{
			AssertNotDisposed();
			DtrEnable = !DtrEnable;
		}

		/// <summary>
		/// Serial port control pins.
		/// </summary>
		public virtual SerialPortControlPins ControlPins
		{
			get
			{
				AssertNotDisposed();

				SerialPortControlPins pins = new SerialPortControlPins();

				if (HandshakeIsNotUsingRequestToSend)
					pins.Rts = base.RtsEnable;
				else
					pins.Rts = true;

				pins.Cts = base.CtsHolding;
				pins.Dtr = base.DtrEnable;
				pins.Dsr = base.DsrHolding;
				pins.Cd  = base.CDHolding;

				return (pins);
			}
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
			if (!base.IsOpen)
			{
				OnOpening(new EventArgs());

			#if (DEBUG && WRITE_DEBUG)
				try
				{
					DebugWrite("Trying base.Open()", true);
					base.Open();
					DebugWrite("base.Open() OK");
				}
				catch (Exception ex)
				{
					DebugWrite("base.Open() exception");
					DebugWrite(ex.Message);
					throw (ex);
				}
			#else
				base.Open();
			#endif

				// immediately send XOn if software flow control enabled to ensure that
                //   device gets put into XOn if it was XOff before
				if ((Handshake == System.IO.Ports.Handshake.XOnXOff) ||
					(Handshake == System.IO.Ports.Handshake.RequestToSendXOnXOff))
				{
					byte[] xOn = { SerialPortSettings.XOnByte };
					base.Write(xOn, 0, 1);
				}

				OnOpened(new EventArgs());
				OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.RtsChanged));
				OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.DtrChanged));
			}
		}

		/// <summary>
		/// Waits for unwritten data to be sent.
		/// </summary>
		public virtual void Flush()
		{
			AssertNotDisposed();
			if (base.IsOpen)
			{
				try
				{
					// Flush() can throw System.Exception
					while (base.BytesToWrite > 0)
					{
						System.Threading.Thread.Sleep(0);
					}
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
		public new void Close()
		{
			AssertNotDisposed();
			if (base.IsOpen)
			{
				OnClosing(new EventArgs());
			#if (DEBUG && WRITE_DEBUG)
				try
				{
					DebugWrite("Trying base.Close()", true);
					base.Close();
					DebugWrite("base.Close() OK");
				}
				catch (Exception ex)
				{
					DebugWrite("base.Close() exception");
					DebugWrite(ex.Message);
					throw (ex);
				}
			#else
				base.Close();
			#endif
				OnClosed(new EventArgs());
				OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.RtsChanged));
				OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.DtrChanged));
			}
		}

		#endregion

		#region SerialPort Event Handling
		//==========================================================================================
		// SerialPort Event Handling
		//==========================================================================================

		private void base_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
		{
			OnDataReceived(new SerialDataReceivedEventArgs(e.EventType));
		}

		private void base_ErrorReceived(object sender, System.IO.Ports.SerialErrorReceivedEventArgs e)
		{
			OnErrorReceived(new SerialErrorReceivedEventArgs(e.EventType));
		}

		private void base_PinChanged(object sender, System.IO.Ports.SerialPinChangedEventArgs e)
		{
			OnPinChanged(new SerialPinChangedEventArgs((SerialPinChange)e.EventType));
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
			if (!IsDisposed && base.IsOpen)      // make sure to propagate event only if port active
				EventHelper.FireSync<SerialDataReceivedEventArgs, SerialDataReceivedEventHandler>(DataReceived, this, e);
		}

		/// <summary>
		/// Invokes "ErrorReceived" event.
		/// </summary>
		protected virtual void OnErrorReceived(SerialErrorReceivedEventArgs e)
		{
			if (!IsDisposed && base.IsOpen)      // make sure to propagate event only if port active
				EventHelper.FireSync<SerialErrorReceivedEventArgs, SerialErrorReceivedEventHandler>(ErrorReceived, this, e);
		}

		/// <summary>
		/// Invokes "PinChanged" event.
		/// </summary>
		protected virtual void OnPinChanged(SerialPinChangedEventArgs e)
		{
			if (!IsDisposed && base.IsOpen)      // make sure to propagate event only if port active
				EventHelper.FireSync<SerialPinChangedEventArgs, SerialPinChangedEventHandler>(PinChanged, this, e);
		}

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

	#if (DEBUG && WRITE_DEBUG)

		private string DebugWrite_portName = "";
		private void DebugWrite(string message)
		{
			DebugWrite(message, false);
		}
		private void DebugWrite(string message, bool writeStack)
		{
			if (DebugWrite_portName == "")
				DebugWrite_portName = PortName;

			System.Diagnostics.Debug.WriteLine(DebugWrite_portName + " " + Environment.TickCount.ToString() + " " + message);

			if (writeStack)
				MKY.Utilities.Diagnostics.XDebug.WriteStack(this, "");
		}

	#endif // DEBUG && WRITE_DEBUG

		#endregion
	}
}
