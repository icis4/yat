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
// MKY Development Version 1.0.6
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

//==================================================================================================
// Configuration
//==================================================================================================

// Choose whether additional debug output should be written on open/close:
// - Uncomment to debug
// - Comment out for normal operation
//#define DEBUG_OPEN_CLOSE

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;

using MKY.Event;

#endregion

namespace MKY.IO.Ports
{
	/// <summary>
	/// Serial port component based on <see cref="System.IO.Ports.SerialPort"/>.
	/// </summary>
	[System.Drawing.ToolboxBitmap(typeof(System.IO.Ports.SerialPort))]
	[DefaultProperty("PortName")]
	public partial class SerialPortEx : System.IO.Ports.SerialPort, ISerialPort, IDisposableEx
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const int BaudRateDefault = 9600;
		private const int DataBitsDefault = 8;
		private const System.IO.Ports.Parity ParityDefault       = System.IO.Ports.Parity.None;
		private const System.IO.Ports.StopBits StopBitsDefault   = System.IO.Ports.StopBits.One;
		private const System.IO.Ports.Handshake HandshakeDefault = System.IO.Ports.Handshake.None;
		private const string PortSettingsDefault = "9600, 8, None, 1, None";

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool inputBreak;
		private bool inputBreakSignal;
		private object inputBreakSyncObj = new object();

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

		/// <summary></summary>
		public SerialPortEx()
		{
			InitializeComponent();
			Initialize();
		}

		/// <summary></summary>
		public SerialPortEx(IContainer container)
		{
			container.Add(this);
			InitializeComponent();
			Initialize();
		}

		private void Initialize()
		{
			base.DataReceived  += new System.IO.Ports.SerialDataReceivedEventHandler (base_DataReceived);
			base.ErrorReceived += new System.IO.Ports.SerialErrorReceivedEventHandler(base_ErrorReceived);
			base.PinChanged    += new System.IO.Ports.SerialPinChangedEventHandler   (base_PinChanged);
		}

		/// <remarks>
		/// This dispose method fixes the deadlock issue described in MKY.IO.Serial.SerialPort.
		/// </remarks>
		private void DisposeBaseStream_SerialPortBugFix()
		{
			if (this.BaseStream != null)
			{
				this.BaseStream.Flush();
				this.BaseStream.Close();
				this.BaseStream.Dispose();
			}
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
		[DefaultValue(SerialPortId.FirstStandardPortName)]
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
				if (value != base.PortName)
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
		[DefaultValue(SerialPortId.FirstStandardPortNumber)]
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
					if (IsOpen)
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
		[DefaultValue(BaudRateDefault)]
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
				if (value != base.BaudRate)
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
		[DefaultValue(DataBitsDefault)]
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
				if (value != base.DataBits)
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
		[DefaultValue(ParityDefault)]
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
				if (value != base.Parity)
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
		[DefaultValue(StopBitsDefault)]
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
				if (value != base.StopBits)
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
		[DefaultValue(HandshakeDefault)]
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
				if (value != base.Handshake)
				{
					base.Handshake = value;
					OnPortSettingsChanged(new EventArgs());
					OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.Rts));
					OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.Dtr));
				}
			}
		}

		private bool HandshakeIsNotUsingRequestToSend
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
		[TypeConverter(typeof(IO.Ports.SerialPortSettingsConverter))]
		public virtual SerialPortSettings PortSettings
		{
			get
			{
				AssertNotDisposed();

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

				OnPortSettingsChanged(new EventArgs());
				OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.Rts));
				OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.Dtr));
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

				// Needed to prevent System.InvalidOperationException in case RTS is in use.
				if (HandshakeIsNotUsingRequestToSend)
					return (base.RtsEnable);
				else
					return (true);
			}
			set
			{
				AssertNotDisposed();

				// Needed to prevent System.InvalidOperationException in case RTS is in use.
				if (HandshakeIsNotUsingRequestToSend)
				{
					if (value != base.RtsEnable)
					{
						base.RtsEnable = value;
						OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.Rts));
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
			this.RtsEnable = !this.RtsEnable;
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
				if (value != base.DtrEnable)
				{
					base.DtrEnable = value;
					OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.Dtr));
				}
			}
		}

		/// <summary>
		/// Toggles the DTR (Data Terminal Ready) control line.
		/// </summary>
		public virtual void ToggleDtr()
		{
			AssertNotDisposed();
			this.DtrEnable = !this.DtrEnable;
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
					pins.Rts = RtsEnable; // 'RtsEnable' must not be accessed if it is used by the base class!
				else
					pins.Rts = true;

				pins.Cts = CtsHolding;
				pins.Dtr = DtrEnable;
				pins.Dsr = DsrHolding;
				pins.Dcd = CDHolding;

				System.Diagnostics.Debug.WriteLine(pins.ToString());

				return (pins);
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
				if (value != BreakState)
				{
					BreakState = value;
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
				OnOpening(new EventArgs());
#if (DEBUG && DEBUG_OPEN_CLOSE)
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
					throw (ex);
				}
#else
				base.Open();
#endif
				OnOpened(new EventArgs());
				OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.Rts));
				OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.Dtr));
				OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.OutputBreak));

				// Immediately check whether there is already data pending
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
					// Flush() can throw System.Exception
					while (BytesToWrite > 0)
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
			if (IsOpen)
			{
				OnClosing(new EventArgs());
#if (DEBUG && DEBUG_OPEN_CLOSE)
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
					throw (ex);
				}
#else
				base.Close();
#endif
				OnClosed(new EventArgs());
				OnPinChanged(new SerialPinChangedEventArgs(SerialPinChange.Rts));
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

		private void base_PinChanged(object sender, System.IO.Ports.SerialPinChangedEventArgs e)
		{
			if (e.EventType == System.IO.Ports.SerialPinChange.Break)
			{
				lock (this.inputBreakSyncObj)
				{
					this.inputBreak = true;
					this.inputBreakSignal = true;
				}
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
			if (!IsDisposed && IsOpen)      // Make sure to propagate event only if port is active.
				EventHelper.FireSync<SerialDataReceivedEventArgs, SerialDataReceivedEventHandler>(DataReceived, this, e);
		}

		/// <summary>
		/// Invokes "ErrorReceived" event.
		/// </summary>
		protected virtual void OnErrorReceived(SerialErrorReceivedEventArgs e)
		{
			if (!IsDisposed && IsOpen)      // Make sure to propagate event only if port is active.
				EventHelper.FireSync<SerialErrorReceivedEventArgs, SerialErrorReceivedEventHandler>(ErrorReceived, this, e);
		}

		/// <summary>
		/// Invokes "PinChanged" event.
		/// </summary>
		protected virtual void OnPinChanged(SerialPinChangedEventArgs e)
		{
			if (!IsDisposed && IsOpen)      // Make sure to propagate event only if port is active.
				EventHelper.FireSync<SerialPinChangedEventArgs, SerialPinChangedEventHandler>(PinChanged, this, e);
		}

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

#if (DEBUG && DEBUG_OPEN_CLOSE)

		private string DebugWrite_portName = "";

		private void DebugWrite(string message)
		{
			DebugWrite(message, false);
		}

		private void DebugWrite(string message, bool writeStack)
		{
			if (DebugWrite_portName.Length == 0)
				DebugWrite_portName = PortName;

			Debug.WriteLine(DebugWrite_portName + " " + Environment.TickCount + " " + message);

			if (writeStack)
				MKY.Diagnostics.XDebug.WriteStack(this, "");
		}

#endif // DEBUG && DEBUG_OPEN_CLOSE

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
