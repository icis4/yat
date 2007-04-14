using System;
using System.ComponentModel;

namespace HSR.IO.Ports
{
	/// <summary>
	/// ISerialPort is based on the .NET standart serial port interface and adds some useful
	/// properties, events and methods.
	/// </summary>
	public interface ISerialPort : ISerialPortDotNet
	{
		/// <summary>
		/// Fired after port changed.
		/// </summary>
		event EventHandler PortChanged;

		/// <summary>
		/// Fired after port settings changed.
		/// </summary>
		event EventHandler PortSettingsChanged;

		/// <summary>
		/// Fired before connection is being opened.
		/// </summary>
		event EventHandler Opening;

		/// <summary>
		/// Fired after connection successfully opened.
		/// </summary>
		event EventHandler Opened;

		/// <summary>
		/// Fired before connection is being closed.
		/// </summary>
		event EventHandler Closing;

		/// <summary>
		/// Fired after connection successfully closed.
		/// </summary>
		event EventHandler Closed;

		/// <summary>
		/// Communications port (e.g. COM1).
		/// </summary>
		SerialPortId PortId { get; set; }

		/// <summary>
		/// Communications port settings.
		/// </summary>
		SerialPortSettings PortSettings { get; set; }

		/// <summary>
		/// Toggles the RTS (Request To Send) control line.
		/// </summary>
		void ToggleRts();

		/// <summary>
		/// Toggles the DTR (Data Terminal Ready) control line.
		/// </summary>
		void ToggleDtr();

		/// <summary>
		/// Control pins.
		/// </summary>
		SerialPortControlPins ControlPins { get; }

		/// <summary>
		/// Waits for unwritten data to be sent.
		/// </summary>
		void Flush();
	}
}
