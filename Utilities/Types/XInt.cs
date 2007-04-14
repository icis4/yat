using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace HSR.Utilities.Types
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
		public static string ConvertToBinaryString(int value)
		{
			return (ConvertToNumericBaseString(2, value, int.MaxValue));
		}

		/// <summary>
		/// Converts value into octal string (e.g. "024").
		/// </summary>
		public static string ConvertToOctalString(int value)
		{
			return (ConvertToNumericBaseString(8, value, int.MaxValue));
		}

		/// <summary>
		/// Converts value into a string with the given numeric base.
		/// </summary>
		/// <param name="numericBase">Numeric base (0 to 9)</param>
		/// <param name="value">Value to be converted</param>
		/// <param name="max">Maximum value</param>
		public static string ConvertToNumericBaseString(int numericBase, int value, int max)
		{
			StringWriter to = new StringWriter();

			int remainder = value;
			for (int power = (int)Math.Log(max, numericBase); power >= 0; power--)
			{
				int divider = (int)Math.Pow(numericBase, power);
				int data = (int)(remainder / divider);
				to.Write(data);
				remainder %= divider;
			}

			return (to.ToString());
		}

		/// <summary>
		/// Parses a binary string (e.g. "00101011"). String must not contain
		/// other characters than '0' or '1'.
		/// </summary>
		/// <param name="parseString">String to be parsed.</param>
		/// <param name="result">
		/// When this method returns, contains the 32-bit signed integer value equivalent
		/// to the number contained in parseString, if the conversion succeeded, or zero if the
		/// conversion failed. The conversion fails if the parseString parameter is null, is not
		/// of the correct format, or represents a number less than System.Int32.MinValue
		/// or greater than System.Int32.MaxValue. This parameter is passed uninitialized.
		/// </param>
		/// <returns>The corresponding integer value.</returns>
		public static bool TryParseBinary(string parseString, out int result)
		{
			return (TryParseNumericBase(2, parseString, out result));
		}

		/// <summary>
		/// Parses a octal string (e.g. "54"). String must not contain other
		/// characters than '0' to '7'.
		/// </summary>
		/// <param name="parseString">String to be parsed.</param>
		/// <param name="result">
		/// When this method returns, contains the 32-bit signed integer value equivalent
		/// to the number contained in parseString, if the conversion succeeded, or zero if the
		/// conversion failed. The conversion fails if the parseString parameter is null, is not
		/// of the correct format, or represents a number less than System.Int32.MinValue
		/// or greater than System.Int32.MaxValue. This parameter is passed uninitialized.
		/// </param>
		/// <returns>The corresponding integer value.</returns>
		public static bool TryParseOctal(string parseString, out int result)
		{
			return (TryParseNumericBase(8, parseString, out result));
		}

		/// <summary>
		/// Parses a string containing a value in any numeric base. String must not
		/// contain leading or trailing non-numeric characters.
		/// </summary>
		/// <param name="numericBase">Numeric base (0 to 9)</param>
		/// <param name="parseString">String to be parsed.</param>
		/// <param name="result">
		/// When this method returns, contains the 32-bit signed integer value equivalent
		/// to the number contained in parseString, if the conversion succeeded, or zero if the
		/// conversion failed. The conversion fails if the parseString parameter is null, is not
		/// of the correct format, or represents a number less than System.Int32.MinValue
		/// or greater than System.Int32.MaxValue. This parameter is passed uninitialized.
		/// </param>
		/// <returns>The corresponding integer value.</returns>
		public static bool TryParseNumericBase(int numericBase, string parseString, out int result)
		{
			char[] from = parseString.ToCharArray();

			result = 0;
			for (int power = 0; power < from.Length; power++)
			{
				int i = from.Length - power - 1;
				int multiplier;
				if (int.TryParse(from[i].ToString(), out multiplier) && (multiplier < numericBase))
				{
					result += multiplier * (int)Math.Pow(numericBase, power);
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
