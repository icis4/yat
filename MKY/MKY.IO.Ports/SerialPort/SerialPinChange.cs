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
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

namespace MKY.IO.Ports
{
	/// <summary>
	/// Specifies the type of change that occurred on the MKY.IO.Ports.SerialPort object.
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
		Rts = 2,

		/// <summary>
		/// The Clear to Send (CTS) signal changed state. This signal is used to indicate
		/// whether data can be sent over the serial port.
		/// </summary>
		Cts = System.IO.Ports.SerialPinChange.CtsChanged,

		/// <summary>
		/// The Data Terminal Ready (DTR) signal changed state. This signal is used to indicate
		/// whether this serial port host is ready to operate.
		/// </summary>
		Dtr = 4,

		/// <summary>
		/// The Data Set Ready (DSR) signal changed state. This signal is used to indicate
		/// whether the device on the serial port is ready to operate.
		/// </summary>
		Dsr = System.IO.Ports.SerialPinChange.DsrChanged,

		/// <summary>
		/// The Carrier Detect (CD) signal changed state. This signal is used to indicate
		/// whether a modem is connected to a working phone line and a data carrier signal
		/// is detected.
		/// </summary>
		CD = System.IO.Ports.SerialPinChange.CDChanged,

		/// <summary>
		/// A break was detected on input.
		/// </summary>
		InputBreak = System.IO.Ports.SerialPinChange.Break,

		/// <summary>
		/// A break was set on output.
		/// </summary>
		OutputBreak = 128,

		/// <summary>
		/// A ring indicator was detected.
		/// </summary>
		Ring = System.IO.Ports.SerialPinChange.Ring,
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
