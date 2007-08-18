using System;
using System.Collections.Generic;
using System.Text;

namespace MKY.Utilities.Types
{
	/// <summary>
	/// Byte utility methods.
	/// </summary>
	public class XByte
	{
		/// <summary>
		/// Converts value into binary string (e.g. "00010100").
		/// </summary>
		public static string ConvertToBinaryString(byte value)
		{
			return (XInt.ConvertToNumericBaseString(2, value, byte.MaxValue));
		}

		/// <summary>
		/// Converts value into octal string (e.g. "024").
		/// </summary>
		public static string ConvertToOctalString(byte value)
		{
			return (XInt.ConvertToNumericBaseString(8, value, byte.MaxValue));
		}
	}
}
