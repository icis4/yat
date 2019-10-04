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
// MKY Version 1.0.27
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

using System;
using System.Diagnostics.CodeAnalysis;

namespace MKY.IO.Ports
{
	/// <summary>
	/// ISerialPort is based on the .NET standard serial port interface and adds some useful
	/// properties, events and methods.
	/// </summary>
	public interface ISerialPort : ISerialPortDotNet
	{
		/// <summary>
		/// Event raised after port changed.
		/// </summary>
		event EventHandler PortChanged;

		/// <summary>
		/// Event raised after port settings changed.
		/// </summary>
		event EventHandler PortSettingsChanged;

		/// <summary>
		/// Event raised before port is being opened.
		/// </summary>
		event EventHandler Opening;

		/// <summary>
		/// Event raised after port successfully opened.
		/// </summary>
		event EventHandler Opened;

		/// <summary>
		/// Event raised before port is being closed.
		/// </summary>
		event EventHandler Closing;

		/// <summary>
		/// Event raised after port successfully closed.
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
		/// Closes the port according to documentation of <see cref="ISerialPortDotNet.Close"/>.
		/// </summary>
		/// <remarks>
		/// This variant of <see cref="ISerialPortDotNet.Close"/> shall be used when closing
		/// intentionally in a "look-forward" manner. When closing the port after a port related
		/// exception has happened, e.g. a <see cref="System.IO.IOException"/> after a device got
		/// physically disconnected, use <see cref="CloseAfterException"/> instead.
		/// </remarks>
		void CloseNormally();

		/// <summary>
		/// Closes the port according to documentation of <see cref="ISerialPortDotNet.Close"/>.
		/// </summary>
		/// <remarks>
		/// This variant of <see cref="ISerialPortDotNet.Close"/> shall be used when closing the
		/// port after a port related exception has happened, e.g. a <see cref="System.IO.IOException"/>
		/// after a device got physically disconnected. When closing the port intentionally in a
		/// "look-forward" manner, use <see cref="CloseNormally"/> instead.
		/// </remarks>
		void CloseAfterException();

		/// <summary>
		/// Gets or sets a value indicating whether the RTS/RTR (Request To Send/Ready To Receive)
		/// control pin is enabled during serial communication.
		/// </summary>
		/// <remarks>
		/// RTS/RTR is also known as RFR (Ready For Receiving).
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rts", Justification = "'RTS' is a common term for serial ports.")]
		new bool RtsEnable { get; set; }

		/// <summary>
		/// Toggles the RTS/RTR (Request To Send/Ready To Receive) control pin.
		/// </summary>
		/// <remarks>
		/// RTS/RTR is also known as RFR (Ready For Receiving).
		/// </remarks>
		/// <returns>
		/// The new state of the RTS control pin.
		/// </returns>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rts", Justification = "'RTS' is a common term for serial ports.")]
		bool ToggleRts();

		/// <summary>
		/// Toggles the DTR (Data Terminal Ready) control pin.
		/// </summary>
		/// <returns>
		/// The new state of the DTR control pin.
		/// </returns>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dtr", Justification = "'DTR' is a common term for serial ports.")]
		bool ToggleDtr();

		/// <summary>
		/// Serial port control pins.
		/// </summary>
		SerialPortControlPins ControlPins { get; }

		/// <summary>
		/// Serial port control pin counts.
		/// </summary>
		SerialPortControlPinCount ControlPinCount { get; }

		/// <summary>
		/// Resets the control pin counts.
		/// </summary>
		void ResetControlPinCount();

		/// <summary>
		/// Gets or sets whether framing errors shall be ignored.
		/// </summary>
		bool IgnoreFramingErrors { get; set; }

		/// <summary>
		/// Gets the input break state.
		/// </summary>
		bool InputBreak { get; }

		/// <summary>
		/// Gets or sets the output break state.
		/// </summary>
		bool OutputBreak { get; set; }

		/// <summary>
		/// Toggles the output break state.
		/// </summary>
		void ToggleOutputBreak();

		/// <summary>
		/// Returns the number of input breaks.
		/// </summary>
		int InputBreakCount { get; }

		/// <summary>
		/// Returns the number of output breaks.
		/// </summary>
		int OutputBreakCount { get; }

		/// <summary>
		/// Resets the break counts.
		/// </summary>
		void ResetBreakCount();

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
		void WriteByte(byte data);

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
		void WriteChar(char data);

		/// <summary>
		/// Waits for unwritten data to be sent.
		/// </summary>
		void Flush();
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
