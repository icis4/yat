﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
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
using System.Globalization;

namespace MKY.Text
{
	/// <summary>
	/// Ascii code conversions. Source: http://www.asciitable.com.
	/// </summary>
	public class Ascii
	{
		//------------------------------------------------------------------------------------------
		// Static Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Converts an ascii code into according mnemonic.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if code out of range 0x00 to 0x1F, 0x7F.</exception>
		public static string ConvertToMnemonic(byte code)
		{
			switch (code)
			{
				case 0x00: return ("NUL"); // \0
				case 0x01: return ("SOH");
				case 0x02: return ("STX");
				case 0x03: return ("ETX");
				case 0x04: return ("EOT");
				case 0x05: return ("ENQ");
				case 0x06: return ("ACK");
				case 0x07: return ("BEL"); // \a
				case 0x08: return ("BS");  // \b
				case 0x09: return ("TAB"); // \t
				case 0x0A: return ("LF");  // \n
				case 0x0B: return ("VT");  // \v
				case 0x0C: return ("FF");  // \f
				case 0x0D: return ("CR");  // \r
				case 0x0E: return ("SO");
				case 0x0F: return ("SI");
				case 0x10: return ("DLE");
				case 0x11: return ("DC1/XON");
				case 0x12: return ("DC2");
				case 0x13: return ("DC3/XOFF");
				case 0x14: return ("DC4");
				case 0x15: return ("NAK");
				case 0x16: return ("SYN");
				case 0x17: return ("ETB");
				case 0x18: return ("CAN");
				case 0x19: return ("EM");
				case 0x1A: return ("SUB");
				case 0x1B: return ("ESC");
				case 0x1C: return ("FS");
				case 0x1D: return ("GS");
				case 0x1E: return ("RS");
				case 0x1F: return ("US");
				case 0x7F: return ("DEL");
				default: throw (new ArgumentOutOfRangeException("code", code, "Code hex(" + code.ToString("X2", NumberFormatInfo.InvariantInfo) + ") is no ascii control code."));
			}
		}

		/// <summary>
		/// Converts an ascii code into according description.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if code out of range 0x00 to 0x1F, 0x7F.</exception>
		public static string ConvertToDescription(byte code)
		{
			switch (code)
			{
				case 0x00: return ("Null");
				case 0x01: return ("Start of heading");
				case 0x02: return ("Start of text");
				case 0x03: return ("End Of text");
				case 0x04: return ("End of transmission");
				case 0x05: return ("Enquiry");
				case 0x06: return ("Acknowledge");
				case 0x07: return ("Bell");
				case 0x08: return ("Backspace");
				case 0x09: return ("Horizontal tabulator");
				case 0x0A: return ("Line feed / New line");
				case 0x0B: return ("Vertical tabulator");
				case 0x0C: return ("Form feed / New page");
				case 0x0D: return ("Carriage return");
				case 0x0E: return ("Shift out");
				case 0x0F: return ("Shift in");
				case 0x10: return ("Date link escape");
				case 0x11: return ("Device control 1 / XOn");
				case 0x12: return ("Device control 2");
				case 0x13: return ("Device control 3 / XOff");
				case 0x14: return ("Device control 4");
				case 0x15: return ("Negative acknowledge");
				case 0x16: return ("Synchronous idle");
				case 0x17: return ("End of transmission block");
				case 0x18: return ("Cancel");
				case 0x19: return ("End of medium");
				case 0x1A: return ("Substitute");
				case 0x1B: return ("Escape");
				case 0x1C: return ("File separator");
				case 0x1D: return ("Group separator");
				case 0x1E: return ("Record separator");
				case 0x1F: return ("Unit separator");
				case 0x7F: return ("Delete");
				default: throw (new ArgumentOutOfRangeException("code", code, "Code hex(" + code.ToString("X2", NumberFormatInfo.InvariantInfo) + ") is no ascii control code."));
			}
		}

		/// <summary>
		/// Converts an ascii mnemonic into according code. Case-insensitive.
		/// </summary>
		/// <exception cref="FormatException">Thrown if mnemonic unknown.</exception>
		public static byte Parse(string mnemonic)
		{
			byte result;
			if (!TryParse(mnemonic, out result))
				return (result);
			else
				throw (new FormatException("Mnemonic '" + mnemonic + "' is no ascii control mnemonic"));
		}

		/// <summary>
		/// Converts an ascii mnemonic into according code. Case-insensitive.
		/// </summary>
		public static bool TryParse(string mnemonic, out byte result)
		{
			switch (mnemonic.ToUpper(CultureInfo.InvariantCulture))
			{
				case "NUL": result = 0x00; return (true);
				case "SOH": result = 0x01; return (true);
				case "STX": result = 0x02; return (true);
				case "ETX": result = 0x03; return (true);
				case "EOT": result = 0x04; return (true);
				case "ENQ": result = 0x05; return (true);
				case "ACK": result = 0x06; return (true);
				case "BEL": result = 0x07; return (true);
				case "BS":  result = 0x08; return (true);
				case "TAB": result = 0x09; return (true);
				case "LF":  result = 0x0A; return (true);
				case "VT":  result = 0x0B; return (true);
				case "FF":  result = 0x0C; return (true);
				case "CR":  result = 0x0D; return (true);
				case "SO":  result = 0x0E; return (true);
				case "SI":  result = 0x0F; return (true);
				case "DLE": result = 0x10; return (true);
				case "DC1": result = 0x11; return (true); case "XON":  result = 0x11; return (true);
				case "DC2": result = 0x12; return (true);
				case "DC3": result = 0x13; return (true); case "XOFF": result = 0x13; return (true);
				case "DC4": result = 0x14; return (true);
				case "NAK": result = 0x15; return (true);
				case "SYN": result = 0x16; return (true);
				case "ETB": result = 0x17; return (true);
				case "CAN": result = 0x18; return (true);
				case "EM":  result = 0x19; return (true);
				case "SUB": result = 0x1A; return (true);
				case "ESC": result = 0x1B; return (true);
				case "FS":  result = 0x1C; return (true);
				case "GS":  result = 0x1D; return (true);
				case "RS":  result = 0x1E; return (true);
				case "US":  result = 0x1F; return (true);
				case "DEL": result = 0x7F; return (true);
				default:    result = 0x00; return (false);
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
