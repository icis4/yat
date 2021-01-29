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
// MKY Version 1.0.29
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using NUnit.Framework;

namespace MKY.Test.Types
{
	/// <summary></summary>
	public static class UInt64ExTestData
	{
		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			get
			{
				// Automatic boundary.
				yield return (new TestCaseData((ulong)                   0, 0, byte.MaxValue,   false, true,                                                         "00000000",                    "000",                  "000",               "00", new byte[] { 0x00 } ));
				yield return (new TestCaseData((ulong)                   1, 0, byte.MaxValue,   false, true,                                                         "00000001",                    "001",                  "001",               "01", new byte[] { 0x01 } ));
				yield return (new TestCaseData((ulong)                 127, 0, byte.MaxValue,   false, true,                                                         "01111111",                    "177",                  "127",               "7F", new byte[] { 0x7F } ));
				yield return (new TestCaseData((ulong)                 128, 0, byte.MaxValue,   false, true,                                                         "10000000",                    "200",                  "128",               "80", new byte[] { 0x80 } ));
				yield return (new TestCaseData((ulong)                 129, 0, byte.MaxValue,   false, true,                                                         "10000001",                    "201",                  "129",               "81", new byte[] { 0x81 } ));
				yield return (new TestCaseData((ulong)                 254, 0, byte.MaxValue,   false, true,                                                         "11111110",                    "376",                  "254",               "FE", new byte[] { 0xFE } ));
				yield return (new TestCaseData((ulong)                 255, 0, byte.MaxValue,   false, true,                                                         "11111111",                    "377",                  "255",               "FF", new byte[] { 0xFF } ));

				yield return (new TestCaseData((ulong)                 256, 0, ushort.MaxValue, false, true,                                                 "0000000100000000",                 "000400",                "00256",             "0100", new byte[] { 0x01, 0x00 } ));
				yield return (new TestCaseData((ulong)               65534, 0, ushort.MaxValue, false, true,                                                 "1111111111111110",                 "177776",                "65534",             "FFFE", new byte[] { 0xFF, 0xFE } ));
				yield return (new TestCaseData((ulong)               65535, 0, ushort.MaxValue, false, true,                                                 "1111111111111111",                 "177777",                "65535",             "FFFF", new byte[] { 0xFF, 0xFF } ));

				yield return (new TestCaseData((ulong)               65536, 0, uint.MaxValue,   false, true,                                 "00000000000000010000000000000000",            "00000200000",          "00000065536",         "00010000", new byte[] { 0x00, 0x01, 0x00, 0x00 } ));
				yield return (new TestCaseData((ulong)               65537, 0, uint.MaxValue,   false, true,                                 "00000000000000010000000000000001",            "00000200001",          "00000065537",         "00010001", new byte[] { 0x00, 0x01, 0x00, 0x01 } ));

				yield return (new TestCaseData((ulong)18446744073709551614, 0, ulong.MaxValue,  false, true, "1111111111111111111111111111111111111111111111111111111111111110", "1777777777777777777776", "18446744073709551614", "FFFFFFFFFFFFFFFE", new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFE } ));
				yield return (new TestCaseData((ulong)18446744073709551615, 0, ulong.MaxValue,  false, true, "1111111111111111111111111111111111111111111111111111111111111111", "1777777777777777777777", "18446744073709551615", "FFFFFFFFFFFFFFFF", new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF } ));

				// Fixed 8 byte boundary.
				yield return (new TestCaseData((ulong)                   0, 8, ulong.MaxValue,  false, true, "0000000000000000000000000000000000000000000000000000000000000000", "0000000000000000000000", "00000000000000000000", "0000000000000000", new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 } ));
				yield return (new TestCaseData((ulong)                   1, 8, ulong.MaxValue,  false, true, "0000000000000000000000000000000000000000000000000000000000000001", "0000000000000000000001", "00000000000000000001", "0000000000000001", new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 } ));
				yield return (new TestCaseData((ulong)                 127, 8, ulong.MaxValue,  false, true, "0000000000000000000000000000000000000000000000000000000001111111", "0000000000000000000177", "00000000000000000127", "000000000000007F", new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7F } ));
				yield return (new TestCaseData((ulong)                 128, 8, ulong.MaxValue,  false, true, "0000000000000000000000000000000000000000000000000000000010000000", "0000000000000000000200", "00000000000000000128", "0000000000000080", new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80 } ));
				yield return (new TestCaseData((ulong)                 129, 8, ulong.MaxValue,  false, true, "0000000000000000000000000000000000000000000000000000000010000001", "0000000000000000000201", "00000000000000000129", "0000000000000081", new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x81 } ));
				yield return (new TestCaseData((ulong)                 254, 8, ulong.MaxValue,  false, true, "0000000000000000000000000000000000000000000000000000000011111110", "0000000000000000000376", "00000000000000000254", "00000000000000FE", new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFE } ));
				yield return (new TestCaseData((ulong)                 255, 8, ulong.MaxValue,  false, true, "0000000000000000000000000000000000000000000000000000000011111111", "0000000000000000000377", "00000000000000000255", "00000000000000FF", new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF } ));

				yield return (new TestCaseData((ulong)                 256, 8, ulong.MaxValue,  false, true, "0000000000000000000000000000000000000000000000000000000100000000", "0000000000000000000400", "00000000000000000256", "0000000000000100", new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00 } ));
				yield return (new TestCaseData((ulong)               65534, 8, ulong.MaxValue,  false, true, "0000000000000000000000000000000000000000000000001111111111111110", "0000000000000000177776", "00000000000000065534", "000000000000FFFE", new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFE } ));
				yield return (new TestCaseData((ulong)               65535, 8, ulong.MaxValue,  false, true, "0000000000000000000000000000000000000000000000001111111111111111", "0000000000000000177777", "00000000000000065535", "000000000000FFFF", new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF } ));

				yield return (new TestCaseData((ulong)               65536, 8, ulong.MaxValue,  false, true, "0000000000000000000000000000000000000000000000010000000000000000", "0000000000000000200000", "00000000000000065536", "0000000000010000", new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00 } ));
				yield return (new TestCaseData((ulong)               65537, 8, ulong.MaxValue,  false, true, "0000000000000000000000000000000000000000000000010000000000000001", "0000000000000000200001", "00000000000000065537", "0000000000010001", new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01 } ));

				yield return (new TestCaseData((ulong)18446744073709551614, 8, ulong.MaxValue,  false, true, "1111111111111111111111111111111111111111111111111111111111111110", "1777777777777777777776", "18446744073709551614", "FFFFFFFFFFFFFFFE", new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFE } ));
				yield return (new TestCaseData((ulong)18446744073709551615, 8, ulong.MaxValue,  false, true, "1111111111111111111111111111111111111111111111111111111111111111", "1777777777777777777777", "18446744073709551615", "FFFFFFFFFFFFFFFF", new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF } ));
			}
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class UInt64ExTest
	{
		#region Tests
		//==========================================================================================
		// Test
		//==========================================================================================

		#region Tests > ToString()
		//------------------------------------------------------------------------------------------
		// Tests > ToString()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "The naming emphasizes the difference between string and number parameters.")]
		[CLSCompliant(false)]
		[Test, TestCaseSource(typeof(UInt64ExTestData), "TestCases")]
		public virtual void TestToString(ulong data, int boundary, ulong max, bool expandNegative, bool useBigEndian, string binString, string octString, string decString, string hexString, byte[] byteArray)
		{
			string decFormat = "D" + decString.Length;
			string hexFormat = "X" + hexString.Length;

			Assert.That(UInt64Ex.ConvertToBinaryString(data, max),              Is.EqualTo(binString));
			Assert.That(UInt64Ex.ConvertToOctalString (data, max),              Is.EqualTo(octString));
			Assert.That(data.ToString(decFormat, CultureInfo.InvariantCulture), Is.EqualTo(decString));
			Assert.That(data.ToString(hexFormat, CultureInfo.InvariantCulture), Is.EqualTo(hexString));
		}

		#endregion

		#region Tests > ToByteArray()
		//------------------------------------------------------------------------------------------
		// Tests > ToByteArray()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "The naming emphasizes the difference between string and number parameters.")]
		[CLSCompliant(false)]
		[Test, TestCaseSource(typeof(UInt64ExTestData), "TestCases")]
		public virtual void TestToByteArray(ulong data, int boundary, ulong max, bool expandNegative, bool useBigEndian, string binString, string octString, string decString, string hexString, byte[] byteArray)
		{
			byte[] convertedByteArray = UInt64Ex.ConvertToByteArray(data, boundary, expandNegative, useBigEndian);
			Assert.That(ArrayEx.ValuesEqual(convertedByteArray, byteArray));
		}

		#endregion

		#region Tests > TryParse()
		//------------------------------------------------------------------------------------------
		// Tests > TryParse()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Using UInt64 for orthogonality of test and testee identifiers.")]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "The naming emphasizes the difference between string and number parameters.")]
		[CLSCompliant(false)]
		[Test, TestCaseSource(typeof(UInt64ExTestData), "TestCases")]
		public virtual void TestTryParse(ulong data, int boundary, ulong max, bool expandNegative, bool useBigEndian, string binString, string octString, string decString, string hexString, byte[] byteArray)
		{
			if (!UInt64Ex.TryParseBinary(binString, out data))
				Assert.Fail("Failed to parse binary string"      + binString);
			if (!UInt64Ex.TryParseOctal (octString, out data))
				Assert.Fail("Failed to parse octal string"       + octString);
			if (!UInt64.TryParse        (decString, out data))
				Assert.Fail("Failed to parse decimal string"     + decString);
			if (!UInt64.TryParse        (hexString, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out data))
				Assert.Fail("Failed to parse hexadecimal string" + hexString);
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
