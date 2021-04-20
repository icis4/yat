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
// MKY Version 1.0.30
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
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
	/// Escape code conversions.
	/// </summary>
	public static class Escape
	{
		//------------------------------------------------------------------------------------------
		// Static Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Converts a character code into according escape sequence.
		/// </summary>
		public static string ConvertToEscapeSequence(int code)
		{
			switch (code)
			{
				case 0x00: return (@"\0");
				case 0x07: return (@"\a");
				case 0x08: return (@"\b");
				case 0x09: return (@"\t");
				case 0x0A: return (@"\n");
				case 0x0B: return (@"\v");
				case 0x0C: return (@"\f");
				case 0x0D: return (@"\r");

				default:
				{
					if      (code <= 0xFF)
						return (@"\x" + code.ToString("X2", CultureInfo.InvariantCulture));
					else if (code <= 0xFFFF)
						return (@"\x" + code.ToString("X4", CultureInfo.InvariantCulture));
					else
						return (@"\x" + code.ToString("X8", CultureInfo.InvariantCulture));
				}
			}
		}

		/// <summary>
		/// Converts an escape sequence into according character code. Case-insensitive.
		/// </summary>
		/// <exception cref="FormatException">Thrown if escape sequence unknown.</exception>
		/// <remarks>
		/// Opposed to the convention of the .NET framework, whitespace is NOT
		/// trimmed from <paramref name="s"/> as all escapes are whitespaces.
		/// </remarks>
		public static byte Parse(string s)
		{
			byte result;
			if (TryParse(s, out result))
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid escape sequence! String must a valid C-style escape sequence."));
		}

		/// <summary>
		/// Converts an escape sequence into according character code. Case-insensitive.
		/// </summary>
		/// <remarks>
		/// Opposed to the convention of the .NET framework, whitespace is NOT
		/// trimmed from <paramref name="s"/> as all escapes are whitespaces.
		/// </remarks>
		public static bool TryParse(string s, out byte result)
		{
			if (s == null)
			{
				result = 0x00;
				return (false);
			}

			// Do not s = s.Trim(); due to reason described above.

			switch (s.ToUpper(CultureInfo.InvariantCulture))
			{
				case @"\0": result = 0x00; return (true);
				case @"\a": result = 0x07; return (true);
				case @"\b": result = 0x08; return (true);
				case @"\t": result = 0x09; return (true);
				case @"\n": result = 0x0A; return (true);
				case @"\v": result = 0x0B; return (true);
				case @"\f": result = 0x0C; return (true);
				case @"\r": result = 0x0D; return (true);
				default:    result = 0x00; return (false);
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
