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
	/// Specifies the type of change that occurred on the MKY.IO.Ports.SerialPort object.
	/// </summary>
	/// <remarks>
	/// This enum adds RTS and DTR pin changes to <see cref="System.IO.Ports.SerialPinChange"/>.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames", Justification = "The name resembles 'System.IO.Ports.SerialPinChange'")]
	[Flags]
	public enum SerialPinChange
	{
		/// <summary>
		/// The RTS/RTR (Request To Send/Ready To Receive) signal changed state.
		/// This signal is used to indicate whether data can be received over the serial port.
		/// </summary>
		/// <remarks>
		/// RTS/RTR is also known as RFR (Ready For Receiving).
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rts", Justification = "'RTS' is a common term for serial ports.")]
		Rts = 2,

		/// <summary>
		/// The CTS (Clear To Send) signal changed state.
		/// This signal is used to indicate whether data can be sent over the serial port.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cts", Justification = "'CTS' is a common term for serial ports.")]
		Cts = System.IO.Ports.SerialPinChange.CtsChanged,

		/// <summary>
		/// The DTR (Data Terminal Ready) signal changed state.
		/// This signal is used to indicate whether this serial port host is ready to operate.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dtr", Justification = "'DTR' is a common term for serial ports.")]
		Dtr = 4,

		/// <summary>
		/// The DSR (Data Set Ready) signal changed state.
		/// This signal is used to indicate whether the device on the serial port is ready to operate.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dsr", Justification = "'DSR' is a common term for serial ports.")]
		Dsr = System.IO.Ports.SerialPinChange.DsrChanged,

		/// <summary>
		/// The DCD (Data Carrier Detect) signal changed state.
		/// This signal is used to indicate whether a modem is connected to a working phone line and a data carrier signal is detected.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dcd", Justification = "'DCD' is a common term for serial ports.")]
		Dcd = System.IO.Ports.SerialPinChange.CDChanged,

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
