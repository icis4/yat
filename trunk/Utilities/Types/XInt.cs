using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MKY.Utilities.Types
{
	/// <summary>
	/// Int utility methods.
	/// </summary>
	/// <remarks>
	/// Possible extensions:
	/// - ParseInside: get integer values inside strings (e.g. "COM5 (Device123B)" returns {5;123})
	/// </remarks>
	public static class XInt
	{
		/// <summary>
		/// Returns the lesser of the two values.
		/// </summary>
		public static int Min(int value1, int value2)
		{
			if (value1 <= value2)
				return (value1);
			else
				return (value2);
		}

		/// <summary>
		/// Returns the larger of the two values.
		/// </summary>
		public static int Max(int value1, int value2)
		{
			if (value1 >= value2)
				return (value1);
			else
				return (value2);
		}

		/// <summary>
		/// Limits "value" to the boundaries specified.
		/// </summary>
		public static int LimitToBounds(int value, int lower, int upper)
		{
			if (value < lower)
				return (lower);
			if (value > upper)
				return (upper);
			return (value);
		}

		/// <summary>
		/// Converts value into binary string (e.g. "00010100").
		/// </summary>
		[CLSCompliant(false)]
		public static string ConvertToBinaryString(ulong value)
		{
			return (ConvertToNumericBaseString(2, value, ulong.MaxValue));
		}

		/// <summary>
		/// Converts value into octal string (e.g. "024").
		/// </summary>
		[CLSCompliant(false)]
		public static string ConvertToOctalString(ulong value)
		{
			return (ConvertToNumericBaseString(8, value, ulong.MaxValue));
		}

		/// <summary>
		/// Converts value into a string with the given numeric base.
		/// </summary>
		/// <param name="numericBase">Numeric base (0 to 9)</param>
		/// <param name="value">Value to be converted</param>
		/// <param name="max">Maximum value</param>
		[CLSCompliant(false)]
		public static string ConvertToNumericBaseString(int numericBase, ulong value, ulong max)
		{
			StringWriter to = new StringWriter();

			ulong remainder = value;
			for (int power = (int)Math.Log(max, numericBase); power >= 0; power--)
			{
				ulong divider = (ulong)Math.Pow(numericBase, power);
				ulong data = remainder / divider;
				to.Write(data);
				remainder %= divider;
			}

			return (to.ToString());
		}

		/// <summary>
		/// Converts value into a byte array. Negative numbers can optionally be converted,
		/// they are aligned to the specified boundary.
		/// </summary>
		/// <param name="value">Value to convert</param>
		/// <returns>Converted byte array</returns>
		[CLSCompliant(false)]
		public static byte[] ConvertToByteArray(ulong value)
		{
			return (ConvertToByteArray(value, 0, false, false));
		}

		/// <summary>
		/// Converts value into a byte array. Negative numbers can optionally be converted,
		/// they are aligned to the specified boundary.
		/// </summary>
		/// <param name="value">Value to convert</param>
		/// <param name="boundary">
		/// Byte boundary (e.g. 1/2/4/8 bytes). Set to 0 for
		/// automatic expansion to next 1/2/4/8 byte boundary.
		/// </param>
		/// <param name="useBigEndian">Use big endian instead of little endian</param>
		/// <returns>Converted byte array</returns>
		[CLSCompliant(false)]
		public static byte[] ConvertToByteArray(ulong value, int boundary, bool useBigEndian)
		{
			return (ConvertToByteArray(value, boundary, false, useBigEndian));
		}

		/// <summary>
		/// Converts value into a byte array. Negative numbers can optionally be converted,
		/// they are aligned to the specified boundary.
		/// </summary>
		/// <param name="value">Value to convert</param>
		/// <param name="expandNegative">True to expand negative values.</param>
		/// <param name="useBigEndian">Use big endian instead of little endian</param>
		/// <returns>Converted byte array</returns>
		[CLSCompliant(false)]
		public static byte[] ConvertToByteArray(ulong value, bool expandNegative, bool useBigEndian)
		{
			return (ConvertToByteArray(value, 0, expandNegative, useBigEndian));
		}

		/// <summary>
		/// Converts value into a byte array. Negative numbers can optionally be converted,
		/// they are aligned to the specified boundary.
		/// </summary>
		/// <param name="value">Value to convert</param>
		/// <param name="boundary">
		/// Byte boundary (e.g. 1/2/4/8 bytes). Set to 0 for
		/// automatic expansion to next 1/2/4/8 byte boundary.
		/// </param>
		/// <param name="expandNegative">True to expand negative values.</param>
		/// <param name="useBigEndian">Use big endian instead of little endian</param>
		/// <returns>Converted byte array</returns>
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
				{
					double base2Log = Math.Log(value, 2);
					double bitsSignificant = base2Log + 1;
					bytesSignificant = (int)Math.Ceiling(bitsSignificant / 8);
				}
			}

			// make sure there's a significant byte
			if (bytesSignificant <= 0)
				bytesSignificant = 1;

			// limit range to UInt64
			if (bytesSignificant > 8)
				bytesSignificant = 8;

			int bytesNeeded = bytesSignificant;
			if (expandNegative)
			{
				if (boundary > 0)
				{
					bytesNeeded = boundary;
				}
				else
				{
					if      (bytesNeeded > 4) bytesNeeded = 8;
					else if (bytesNeeded > 2) bytesNeeded = 4;
				}
			}

			// limit range to UInt64
			if (bytesNeeded > 8)
				bytesNeeded = 8;

			int i = 0;
			ulong l = 0;
			byte b = 0;
			byte[] result = new byte[bytesNeeded];

			// for negative values, fill leading bytes with 0xFF
			if (expandNegative)
			{
				for (i = bytesNeeded; i > bytesSignificant; i--)
					result[i - 1] = 0xFF;
			}

			// convert most significant byte
			if (bytesSignificant < 8)
				l = value % (ulong)Math.Pow(2, bytesSignificant * 8);
			else
				l = value;

			b = (byte)(l >> ((bytesSignificant - 1) * 8));

			// for negative values, expand most significant byte
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

			// write most significant
			result[bytesSignificant - 1] = b;

			// convert and write remaining bytes
			for (i = (bytesSignificant - 1); i > 0; i--)
			{
				l = value % (ulong)Math.Pow(2, i * 8);
				b = (byte)(l >> ((i - 1) * 8));
				result[i - 1] = b;
			}

			// swap endianess if needed
			if (useBigEndian)
			{
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
		/// <param name="parseString">String to be parsed.</param>
		/// <param name="isBigEndian">String is in big endian instead of little endian.</param>
		/// <param name="result">
		/// When this method returns, contains the 64-bit unsigned value value equivalent
		/// to the number contained in parseString, if the conversion succeeded, or zero if the
		/// conversion failed. The conversion fails if the parseString parameter is null, is not
		/// of the correct format, or represents a number less than System.Int32.MinValue
		/// or greater than System.Int32.MaxValue. This parameter is passed uninitialized.
		/// </param>
		/// <returns>The corresponding integer value.</returns>
		[CLSCompliant(false)]
		public static bool TryParseBinary(string parseString, bool isBigEndian, out ulong result)
		{
			return (TryParseNumericBase(2, parseString, isBigEndian, out result));
		}

		/// <summary>
		/// Parses a octal string (e.g. "54"). String must not contain other
		/// characters than '0' to '7'.
		/// </summary>
		/// <param name="parseString">String to be parsed.</param>
		/// <param name="isBigEndian">String is in big endian instead of little endian.</param>
		/// <param name="result">
		/// When this method returns, contains the 64-bit unsigned value equivalent
		/// to the number contained in parseString, if the conversion succeeded, or zero if the
		/// conversion failed. The conversion fails if the parseString parameter is null, is not
		/// of the correct format, or represents a number less than System.Int32.MinValue
		/// or greater than System.Int32.MaxValue. This parameter is passed uninitialized.
		/// </param>
		/// <returns>The corresponding integer value.</returns>
		[CLSCompliant(false)]
		public static bool TryParseOctal(string parseString, bool isBigEndian, out ulong result)
		{
			return (TryParseNumericBase(8, parseString, isBigEndian, out result));
		}

		/// <summary>
		/// Parses a string containing a value in any numeric base. String must not
		/// contain leading or trailing non-numeric characters.
		/// </summary>
		/// <param name="numericBase">Numeric base (0 to 9)</param>
		/// <param name="parseString">String to be parsed.</param>
		/// <param name="isBigEndian">String is in big endian instead of little endian.</param>
		/// <param name="result">
		/// When this method returns, contains the 64-bit unsigned value equivalent
		/// to the number contained in parseString, if the conversion succeeded, or zero if the
		/// conversion failed. The conversion fails if the parseString parameter is null, is not
		/// of the correct format, or represents a number less than System.Decimal.MinValue
		/// or greater than System.Decimal.MaxValue. This parameter is passed uninitialized.
		/// </param>
		/// <returns>The corresponding integer value.</returns>
		[CLSCompliant(false)]
		public static bool TryParseNumericBase(int numericBase, string parseString, bool isBigEndian, out ulong result)
		{
			char[] from = parseString.ToCharArray();

			// swap endianess if needed
			if (isBigEndian)
			{
				int i = 0;
				int j = 0;
				char c = '\0';
				for (i = 0, j = (from.Length - 1); i < (int)(from.Length / 2); i++, j--)
				{
					c = from[i];
					from[i] = from[j];
					from[j] = c;
				}
			}

			result = 0;
			for (int power = 0; power < from.Length; power++)
			{
				int i = from.Length - power - 1;
				int multiplier;
				if (int.TryParse(from[i].ToString(), out multiplier) && (multiplier < numericBase))
				{
					result += (ulong)(multiplier * (int)Math.Pow(numericBase, power));
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
