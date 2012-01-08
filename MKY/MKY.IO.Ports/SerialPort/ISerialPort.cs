﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.8
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;

namespace MKY.IO.Ports
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
		/// Fired before port is being opened.
		/// </summary>
		event EventHandler Opening;

		/// <summary>
		/// Fired after port successfully opened.
		/// </summary>
		event EventHandler Opened;

		/// <summary>
		/// Fired before port is being closed.
		/// </summary>
		event EventHandler Closing;

		/// <summary>
		/// Fired after port successfully closed.
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
