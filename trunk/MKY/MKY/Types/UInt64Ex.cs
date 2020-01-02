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
// Copyright © 2003-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;

// This code is intentionally placed into the MKY namespace even though the file is located in
// MKY.Types for consistency with the System namespace.
namespace MKY
{
	/// <summary>
	/// UInt64/ulong utility methods.
	/// </summary>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "UInt64 just *is* 'ulong'...")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class UInt64Ex
	{
		/// <summary>
		/// Converts the value into a binary string (e.g. "0000000000000000000000000000000000000000000000000000000000010100").
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1631:DocumentationMustMeetCharacterPercentage", Justification = "Sorry, 64 bits are that long...")]
		[CLSCompliant(false)]
		public static string ConvertToBinaryString(ulong value)
		{
			return (ConvertToBinaryString(value, ulong.MaxValue));
		}

		/// <summary>
		/// Converts the value into a binary string (e.g. "0000000000000000000000000000000000000000000000000000000000010100").
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1631:DocumentationMustMeetCharacterPercentage", Justification = "Sorry, 64 bits are that long...")]
		[CLSCompliant(false)]
		public static string ConvertToBinaryString(ulong value, ulong max)
		{
			return (ConvertToNumericBaseString(2, value, max));
		}

		/// <summary>
		/// Converts value into octal string (e.g. "000000000000000000024").
		/// </summary>
		[CLSCompliant(false)]
		public static string ConvertToOctalString(ulong value)
		{
			return (ConvertToOctalString(value, ulong.MaxValue));
		}

		/// <summary>
		/// Converts value into octal string (e.g. "000000000000000000024").
		/// </summary>
		[CLSCompliant(false)]
		public static string ConvertToOctalString(ulong value, ulong max)
		{
			return (ConvertToNumericBaseString(8, value, max));
		}

		/// <summary>
		/// Converts value into a string with the given numeric base.
		/// </summary>
		/// <param name="numericBase">Numeric base (e.g. 4 or 8).</param>
		/// <param name="value">Value to be converted.</param>
		/// <param name="max">Maximum value.</param>
		[CLSCompliant(false)]
		public static string ConvertToNumericBaseString(int numericBase, ulong value, ulong max)
		{
			using (var to = new StringWriter(CultureInfo.InvariantCulture))
			{
				ulong remainder = value; // Cast to double to prevent overflow on ulong.MaxValue.
				double exactPower = System.Math.Log((double)max + 1, numericBase);
				for (int power = (int)(System.Math.Ceiling(exactPower)) - 1; power >= 0; power--)
				{
					ulong divider = (ulong)(System.Math.Pow(numericBase, power));
					ulong data = remainder / divider;
					to.Write(data);
					remainder %= divider;
				}

				return (to.ToString());
			}
		}

		/// <summary>
		/// Converts value into a byte array. Negative numbers can optionally be converted,
		/// they are aligned to the specified boundary.
		/// </summary>
		/// <param name="value">Value to convert.</param>
		/// <returns>Converted byte array.</returns>
		[CLSCompliant(false)]
		public static byte[] ConvertToByteArray(ulong value)
		{
			return (ConvertToByteArray(value, 0, false, false));
		}

		/// <summary>
		/// Converts value into a byte array. Negative numbers can optionally be converted,
		/// they are aligned to the specified boundary.
		/// </summary>
		/// <param name="value">Value to convert.</param>
		/// <param name="boundary">
		/// byte boundary (e.g. 1/2/4/8 bytes). Set to 0 for
		/// automatic expansion to next 1/2/4/8 byte boundary.
		/// </param>
		/// <param name="useBigEndian">Use big endian instead of little endian.</param>
		/// <returns>Converted byte array.</returns>
		[CLSCompliant(false)]
		public static byte[] ConvertToByteArray(ulong value, int boundary, bool useBigEndian)
		{
			return (ConvertToByteArray(value, boundary, false, useBigEndian));
		}

		/// <summary>
		/// Converts value into a byte array. Negative numbers can optionally be converted,
		/// they are aligned to the specified boundary.
		/// </summary>
		/// <param name="value">Value to convert.</param>
		/// <param name="expandNegative">True to expand negative values.</param>
		/// <param name="useBigEndian">Use big endian instead of little endian.</param>
		/// <returns>Converted byte array.</returns>
		[CLSCompliant(false)]
		public static byte[] ConvertToByteArray(ulong value, bool expandNegative, bool useBigEndian)
		{
			return (ConvertToByteArray(value, 0, expandNegative, useBigEndian));
		}

		/// <summary>
		/// Converts value into a byte array. Negative numbers can optionally be converted,
		/// they are aligned to the specified boundary.
		/// </summary>
		/// <param name="value">Value to convert.</param>
		/// <param name="boundary">
		/// byte boundary (e.g. 1/2/4/8 bytes). Set to 0 for
		/// automatic expansion to next 1/2/4/8 byte boundary.
		/// </param>
		/// <param name="expandNegative">True to expand negative values.</param>
		/// <param name="useBigEndian">Use big endian instead of little endian.</param>
		/// <returns>Converted byte array.</returns>
		[CLSCompliant(false)]
		public static byte[] ConvertToByteArray(ulong value, int boundary, bool expandNegative, bool useBigEndian)
		{
			int bytesSignificant = 0;
			if (boundary > 0)
			{
				bytesSignificant = boundary;
			}
			else
			{
				if (value > 0)
				{	// Cast to double to prevent overflow on ulong.MaxValue.
					double base2Log = System.Math.Log((double)value + 1, 2);
					double bitsSignificant = System.Math.Ceiling(base2Log);
					bytesSignificant = (int)(System.Math.Ceiling(bitsSignificant / 8));
				}
			}

			// Make sure there's a significant byte.
			if (bytesSignificant <= 0)
				bytesSignificant = 1;

			// Limit range to UInt64.
			if (bytesSignificant > 8)
				bytesSignificant = 8;

			int bytesNeeded = bytesSignificant;
			if (boundary > 0)
			{
				bytesNeeded = boundary;
			}
			else
			{
				// Auto-adjust boundary.
				if      (bytesNeeded > 4) bytesNeeded = 8;
				else if (bytesNeeded > 2) bytesNeeded = 4;
			}

			// Limit range to UInt64.
			if (bytesNeeded > 8)
				bytesNeeded = 8;

			byte[] result = new byte[bytesNeeded];

			// For negative values, fill leading bytes with 0xFF.
			if (expandNegative)
			{
				for (int i = bytesNeeded; i > bytesSignificant; i--)
					result[i - 1] = 0xFF;
			}

			ulong l = 0;

			// Convert most significant byte.
			if (bytesSignificant < 8)
				l = value % (ulong)(System.Math.Pow(2, bytesSignificant * 8));
			else
				l = value;

			byte b = (byte)(l >> ((bytesSignificant - 1) * 8));

			// For negative values, expand most significant byte.
			if (expandNegative)
			{
				if      (b < 0x01) b |= 0xFF;
				else if (b < 0x02) b |= 0xFE;
				else if (b < 0x04) b |= 0xFC;
				else if (b < 0x08) b |= 0xF8;
				else if (b < 0x10) b |= 0xF0;
				else if (b < 0x20) b |= 0xE0;
				else if (b < 0x40) b |= 0xC0;
				else if (b < 0x80) b |= 0x80;
			}

			// Write most significant.
			result[bytesSignificant - 1] = b;

			// Convert and write remaining bytes.
			for (int i = (bytesSignificant - 1); i > 0; i--)
			{
				l = value % (ulong)(System.Math.Pow(2, i * 8));
				b = (byte)(l >> ((i - 1) * 8));
				result[i - 1] = b;
			}

			// Swap endianness if needed.
			if (useBigEndian)
			{
				int i = 0;
				int j = 0;
				for (i = 0, j = (result.Length - 1); i < (int)(result.Length / 2); i++, j--)
				{
					b = result[i];
					result[i] = result[j];
					result[j] = b;
				}
			}

			return (result);
		}

		/// <summary>
		/// Parses a binary string (e.g. "00101011"). String must not contain
		/// other characters than '0' or '1'.
		/// </summary>
		/// <param name="s">String to be parsed.</param>
		/// <param name="result">
		/// When this method returns, contains the 64-bit unsigned value value equivalent to the
		/// number contained in <paramref name="s"/>, if the conversion succeeded, or zero if
		/// the conversion failed. The conversion fails if the <paramref name="s"/> parameter
		/// is <c>null</c>, is not of the correct format, or represents a number less than
		/// <see cref="UInt64.MinValue"/> or greater than <see cref="UInt64.MaxValue"/>.
		/// This parameter is passed uninitialized.
		/// </param>
		/// <returns>The corresponding integer value.</returns>
		[CLSCompliant(false)]
		public static bool TryParseBinary(string s, out ulong result)
		{
			return (TryParseNumericBase(2, s, out result));
		}

		/// <summary>
		/// Parses a octal string (e.g. "54"). String must not contain other
		/// characters than '0' to '7'.
		/// </summary>
		/// <param name="s">String to be parsed.</param>
		/// <param name="result">
		/// When this method returns, contains the 64-bit unsigned value value equivalent to the
		/// number contained in <paramref name="s"/>, if the conversion succeeded, or zero if
		/// the conversion failed. The conversion fails if the <paramref name="s"/> parameter
		/// is <c>null</c>, is not of the correct format, or represents a number less than
		/// <see cref="UInt64.MinValue"/> or greater than <see cref="UInt64.MaxValue"/>.
		/// This parameter is passed uninitialized.
		/// </param>
		/// <returns>The corresponding integer value.</returns>
		[CLSCompliant(false)]
		public static bool TryParseOctal(string s, out ulong result)
		{
			return (TryParseNumericBase(8, s, out result));
		}

		/// <summary>
		/// Parses a string containing a value in any numeric base. String must not
		/// contain leading or trailing non-numeric characters.
		/// </summary>
		/// <param name="numericBase">Numeric base (e.g. 4 or 8).</param>
		/// <param name="s">String to be parsed.</param>
		/// <param name="result">
		/// When this method returns, contains the 64-bit unsigned value value equivalent to the
		/// number contained in <paramref name="s"/>, if the conversion succeeded, or zero if
		/// the conversion failed. The conversion fails if the <paramref name="s"/> parameter
		/// is <c>null</c>, is not of the correct format, or represents a number less than
		/// <see cref="UInt64.MinValue"/> or greater than <see cref="UInt64.MaxValue"/>.
		/// This parameter is passed uninitialized.
		/// </param>
		/// <returns>The corresponding integer value.</returns>
		[CLSCompliant(false)]
		public static bool TryParseNumericBase(int numericBase, string s, out ulong result)
		{
			char[] from = s.ToCharArray();
			result = 0;
			for (int power = 0; power < from.Length; power++)
			{
				int i = from.Length - power - 1;
				int multiplier;
				if (int.TryParse(from[i].ToString(CultureInfo.InvariantCulture), out multiplier) && (multiplier < numericBase))
				{
					result += (ulong)multiplier * (ulong)(System.Math.Pow(numericBase, power));
				}
				else
				{
					result = 0;
					return (false);
				}
			}
			return (true);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
