using System;
using System.Collections.Generic;
using System.Text;

namespace HSR.IO.Ports
{
	/// <summary>
	/// Specifies the type of change that occurred on the HSR.IO.Ports.SerialPort object.
	/// </summary>
	/// <remarks>
	/// This enum adds RTS and DTR pin changes to <see cref="System.IO.Ports.SerialPinChange"/>.
	/// </remarks>
	public enum SerialPinChange
	{
		/// <summary>
		/// Indicates that it's unknown which pin changed it's state.
		/// </summary>
		Unknown = 0,

		/// <summary>
		/// The Ready to Send (RTS) signal changed state. This signal is used to indicate
		/// whether data can be received over the serial port.
		/// </summary>
		RtsChanged = 2,

		/// <summary>
		/// The Clear to Send (CTS) signal changed state. This signal is used to indicate
		/// whether data can be sent over the serial port.
		/// </summary>
		CtsChanged = System.IO.Ports.SerialPinChange.CtsChanged,

		/// <summary>
		/// The Data Terminal Ready (DTR) signal changed state. This signal is used to indicate
		/// whether this serial port host is ready to operate.
		/// </summary>
		DtrChanged = 4,

		/// <summary>
		/// The Data Set Ready (DSR) signal changed state. This signal is used to indicate
		/// whether the device on the serial port is ready to operate.
		/// </summary>
		DsrChanged = System.IO.Ports.SerialPinChange.DsrChanged,

		/// <summary>
		/// The Carrier Detect (CD) signal changed state. This signal is used to indicate
		/// whether a modem is connected to a working phone line and a data carrier signal
		/// is detected.
		/// </summary>
		CDChanged = System.IO.Ports.SerialPinChange.CDChanged,

		/// <summary>
		/// A break was detected on input.
		/// </summary>
		Break = System.IO.Ports.SerialPinChange.Break,

		/// <summary>
		/// A ring indicator was detected.
		/// </summary>
		Ring = System.IO.Ports.SerialPinChange.Ring,
	}
}