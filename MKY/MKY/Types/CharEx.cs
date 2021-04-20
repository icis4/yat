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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using MKY.Text;

// This code is intentionally placed into the MKY namespace even though the file is located in
// MKY.Types for consistency with the System namespace.
namespace MKY
{
	/// <summary>
	/// <see cref="Char"/>/<see cref="char"/> utility methods.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class CharEx
	{
		/// <summary>
		/// An invalid char is represented by -1.
		/// </summary>
		/// <remarks>
		/// Value corresponds to the value returned by <see cref="System.IO.StringReader.Read()"/>
		/// and the other read functions if no more characters can be read from the stream.
		/// Value also corresponds to <see cref="IO.StreamEx.EndOfStream"/>.
		/// </remarks>
		public const int InvalidChar = -1;

		/// <summary>
		/// Tries to convert the UTF-16 encoded value into the according ASCII code.
		/// </summary>
		public static bool TryConvertToByte(char value, out byte ascii)
		{
			try
			{
				ascii = Convert.ToByte(value);
				return (true);
			}
			catch (OverflowException)
			{
				ascii = 0x00;
				return (false);
			}
		}

		/// <summary>
		/// Converts the given character to a printable string.
		/// </summary>
		/// <remarks>
		/// If the value is a printable character, the string contains that character.
		/// If the value is a control character, the ASCII mnemonic or Unicode representation is returned.
		/// </remarks>
		public static string ConvertToPrintableString(char value)
		{
			if (!char.IsControl(value))
				return (value.ToString());

			// ASCII control characters:
			byte asciiCode;
			if ((TryConvertToByte(value, out asciiCode)) && (Ascii.IsControl(asciiCode)))
				return ("<" + Ascii.ConvertToMnemonic(asciiCode) + ">");

			// Unicode control characters U+0080..U+009F:
			return (@"\U+" + ((ushort)(value)).ToString("X4", CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Determines whether the given character is valid for Base64.
		/// </summary>
		public static bool IsValidForBase64(char value)
		{
			if ((value >= 'A') && (value <= 'Z'))
				return (true);

			if ((value >= 'a') && (value <= 'z'))
				return (true);

			if ((value >= '0') && (value <= '9'))
				return (true);

			if (value == '+')
				return (true);

			if (value == '/')
				return (true);

			if (value == '=')
				return (true);

			return (false);
		}

		/// <summary>
		/// Determines whether the given character is valid for Base64Url.
		/// </summary>
		public static bool IsValidForBase64Url(char value)
		{
			if ((value >= 'A') && (value <= 'Z'))
				return (true);

			if ((value >= 'a') && (value <= 'z'))
				return (true);

			if ((value >= '0') && (value <= '9'))
				return (true);

			if (value == '-') // '-' instead of '+'
				return (true);

			if (value == '_') // '_' instead of '/'
				return (true);

			if (value == '%') // '%' with numeric value (e.g "%3d") instead of '='
				return (true);

			return (false);
		}

		/// <summary>
		/// Determines whether the given character is valid for UTF-7, as specified
		/// by https://tools.ietf.org/html/rfc2152.
		/// </summary>
		/// <remarks>
		/// Located here instead of <see cref="EncodingEx"/> for closeness to methods above.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "UTF", Justification = "Same spelling as 'Encoding.UTF7'.")]
		public static bool IsValidForUTF7(char value)
		{
			if ((value >= 'A') && (value <= 'Z'))
				return (true);

			if ((value >= 'a') && (value <= 'z'))
				return (true);

			if ((value >= '0') && (value <= '9'))
				return (true);

			switch (value)
			{
				// Mandatory:
				case '\'':
				case '(':
				case ')':
				case ',':
				case '-':
				case '.':
				case '/':
				case ':':
				case '?':

				// Optional:
				case '!':
				case '"':
				case '#':
				case '$':
				case '%':
				case '&':
				case '*':
				case ';':
				case '<':
				case '=':
				case '>':
				case '@':
				case '[':
				case ']':
				case '^':
				case '_':
				case '`':
				case '{':
				case '|':
				case '}':
					return (true);
			}

			return (false);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
