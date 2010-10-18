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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using NUnit.Framework;

using MKY.Types;

namespace MKY.Test.Types
{
	/// <summary></summary>
	[TestFixture]
	public class XUInt64Test
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private struct TestSet
		{
			public readonly ulong Data;

			public readonly int Boundary;
			public readonly ulong Max;
			public readonly bool ExpandNegative;
			public readonly bool UseBigEndian;

			public readonly string BinString;
			public readonly string OctString;
			public readonly string DecString;
			public readonly string HexString;

			public readonly string DecFormat;
			public readonly string HexFormat;

			public readonly byte[] ByteArray;

			public TestSet(ulong data, int boundary, ulong max, bool expandNegative, bool useBigEndian, string binString, string octString, string decString, string hexString, byte[] byteArray)
			{
				Data = data;

				Boundary = boundary;
				Max = max;
				ExpandNegative = expandNegative;
				UseBigEndian = useBigEndian;

				BinString = binString;
				OctString = octString;
				DecString = decString;
				HexString = hexString;

				DecFormat = "D" + DecString.Length;
				HexFormat = "X" + HexString.Length;

				ByteArray = byteArray;
			}
		}

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private readonly TestSet[][] testSets =
		{
			// Automatic boundary
			new TestSet[]
			{
				new TestSet(                    0, 0, byte.MaxValue,   false, true,                                                         "00000000",                    "000",                  "000",               "00", new byte[] { 0x00 } ),
				new TestSet(                    1, 0, byte.MaxValue,   false, true,                                                         "00000001",                    "001",                  "001",               "01", new byte[] { 0x01 } ),
				new TestSet(                  127, 0, byte.MaxValue,   false, true,                                                         "01111111",                    "177",                  "127",               "7F", new byte[] { 0x7F } ),
				new TestSet(                  128, 0, byte.MaxValue,   false, true,                                                         "10000000",                    "200",                  "128",               "80", new byte[] { 0x80 } ),
				new TestSet(                  129, 0, byte.MaxValue,   false, true,                                                         "10000001",                    "201",                  "129",               "81", new byte[] { 0x81 } ),
				new TestSet(                  254, 0, byte.MaxValue,   false, true,                                                         "11111110",                    "376",                  "254",               "FE", new byte[] { 0xFE } ),
				new TestSet(                  255, 0, byte.MaxValue,   false, true,                                                         "11111111",                    "377",                  "255",               "FF", new byte[] { 0xFF } ),

				new TestSet(                  256, 0, ushort.MaxValue, false, true,                                                 "0000000100000000",                 "000400",                "00256",             "0100", new byte[] { 0x01, 0x00 } ),
				new TestSet(                65534, 0, ushort.MaxValue, false, true,                                                 "1111111111111110",                 "177776",                "65534",             "FFFE", new byte[] { 0xFF, 0xFE } ),
				new TestSet(                65535, 0, ushort.MaxValue, false, true,                                                 "1111111111111111",                 "177777",                "65535",             "FFFF", new byte[] { 0xFF, 0xFF } ),

				new TestSet(                65536, 0, uint.MaxValue,   false, true,                                 "00000000000000010000000000000000",            "00000200000",          "00000065536",         "00010000", new byte[] { 0x00, 0x01, 0x00, 0x00 } ),
				new TestSet(                65537, 0, uint.MaxValue,   false, true,                                 "00000000000000010000000000000001",            "00000200001",          "00000065537",         "00010001", new byte[] { 0x00, 0x01, 0x00, 0x01 } ),

				new TestSet( 18446744073709551614, 0, ulong.MaxValue,  false, true, "1111111111111111111111111111111111111111111111111111111111111110", "1777777777777777777776", "18446744073709551614", "FFFFFFFFFFFFFFFE", new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFE } ),
				new TestSet( 18446744073709551615, 0, ulong.MaxValue,  false, true, "1111111111111111111111111111111111111111111111111111111111111111", "1777777777777777777777", "18446744073709551615", "FFFFFFFFFFFFFFFF", new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF } ),
			},

			// Fixed 8 byte boundary.
			new TestSet[]
			{
				new TestSet(                    0, 8, ulong.MaxValue,  false, true, "0000000000000000000000000000000000000000000000000000000000000000", "0000000000000000000000", "00000000000000000000", "0000000000000000", new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 } ),
				new TestSet(                    1, 8, ulong.MaxValue,  false, true, "0000000000000000000000000000000000000000000000000000000000000001", "0000000000000000000001", "00000000000000000001", "0000000000000001", new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 } ),
				new TestSet(                  127, 8, ulong.MaxValue,  false, true, "0000000000000000000000000000000000000000000000000000000001111111", "0000000000000000000177", "00000000000000000127", "000000000000007F", new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7F } ),
				new TestSet(                  128, 8, ulong.MaxValue,  false, true, "0000000000000000000000000000000000000000000000000000000010000000", "0000000000000000000200", "00000000000000000128", "0000000000000080", new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80 } ),
				new TestSet(                  129, 8, ulong.MaxValue,  false, true, "0000000000000000000000000000000000000000000000000000000010000001", "0000000000000000000201", "00000000000000000129", "0000000000000081", new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x81 } ),
				new TestSet(                  254, 8, ulong.MaxValue,  false, true, "0000000000000000000000000000000000000000000000000000000011111110", "0000000000000000000376", "00000000000000000254", "00000000000000FE", new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFE } ),
				new TestSet(                  255, 8, ulong.MaxValue,  false, true, "0000000000000000000000000000000000000000000000000000000011111111", "0000000000000000000377", "00000000000000000255", "00000000000000FF", new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF } ),

				new TestSet(                  256, 8, ulong.MaxValue,  false, true, "0000000000000000000000000000000000000000000000000000000100000000", "0000000000000000000400", "00000000000000000256", "0000000000000100", new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00 } ),
				new TestSet(                65534, 8, ulong.MaxValue,  false, true, "0000000000000000000000000000000000000000000000001111111111111110", "0000000000000000177776", "00000000000000065534", "000000000000FFFE", new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFE } ),
				new TestSet(                65535, 8, ulong.MaxValue,  false, true, "0000000000000000000000000000000000000000000000001111111111111111", "0000000000000000177777", "00000000000000065535", "000000000000FFFF", new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF } ),

				new TestSet(                65536, 8, ulong.MaxValue,  false, true, "0000000000000000000000000000000000000000000000010000000000000000", "0000000000000000200000", "00000000000000065536", "0000000000010000", new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00 } ),
				new TestSet(                65537, 8, ulong.MaxValue,  false, true, "0000000000000000000000000000000000000000000000010000000000000001", "0000000000000000200001", "00000000000000065537", "0000000000010001", new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01 } ),

				new TestSet( 18446744073709551614, 8, ulong.MaxValue,  false, true, "1111111111111111111111111111111111111111111111111111111111111110", "1777777777777777777776", "18446744073709551614", "FFFFFFFFFFFFFFFE", new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFE } ),
				new TestSet( 18446744073709551615, 8, ulong.MaxValue,  false, true, "1111111111111111111111111111111111111111111111111111111111111111", "1777777777777777777777", "18446744073709551615", "FFFFFFFFFFFFFFFF", new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF } ),
			},
		};

		#endregion

		#region Tests
		//==========================================================================================
		// Test
		//==========================================================================================

		#region Tests > ToString()
		//------------------------------------------------------------------------------------------
		// Tests > ToString()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestToString()
		{
			foreach (TestSet[] tsArray in this.testSets)
			{
				foreach (TestSet ts in tsArray)
				{
					ulong data = ts.Data;

					Assert.AreEqual(ts.BinString, XUInt64.ConvertToBinaryString(data, ts.Max));
					Assert.AreEqual(ts.OctString, XUInt64.ConvertToOctalString(data, ts.Max));
					Assert.AreEqual(ts.DecString, data.ToString(ts.DecFormat));
					Assert.AreEqual(ts.HexString, data.ToString(ts.HexFormat));
				}
			}
		}

		#endregion

		#region Tests > ToByteArray()
		//------------------------------------------------------------------------------------------
		// Tests > ToByteArray()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		[Test]
		public virtual void TestToByteArray()
		{
			Exception exceptionToNUnit = null;

			foreach (TestSet[] tsArray in this.testSets)
			{
				foreach (TestSet ts in tsArray)
				{
					byte[] convertedByteArray = new byte[] { };

					try
					{
						convertedByteArray = XUInt64.ConvertToByteArray(ts.Data, ts.Boundary, ts.ExpandNegative, ts.UseBigEndian);
						Assert.AreEqual(ts.ByteArray, convertedByteArray);
					}
					catch (Exception ex)
					{
						// Catch assertion exceptions to ensure that all test sets are run in any case
						//   but keep first exception to signal NUnit that test has failed.
						if (exceptionToNUnit == null)
							exceptionToNUnit = ex;

						Console.WriteLine("Invalid parser output bytes:");
						Console.WriteLine();
						Console.WriteLine("Input data =");
						Console.WriteLine(@"""" + ts.Data + @"""");
						Console.WriteLine();
						Console.WriteLine("Expected bytes =");
						foreach (byte b in ts.ByteArray)
						{
							Console.Write("0x" + b.ToString("X2", CultureInfo.InvariantCulture) + ", ");
						}
						Console.WriteLine();
						Console.WriteLine("Actual converted bytes =");
						foreach (byte b in convertedByteArray)
						{
							Console.Write("0x" + b.ToString("X2", CultureInfo.InvariantCulture) + ", ");
						}
						Console.WriteLine();
					}
				}
			}

			// Re-throw first exception to signal NUnit that test has failed.
			if (exceptionToNUnit != null)
				throw (exceptionToNUnit);
		}

		#endregion

		#region Tests > TryParse()
		//------------------------------------------------------------------------------------------
		// Tests > TryParse()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestTryParse()
		{
			foreach (TestSet[] tsArray in this.testSets)
			{
				foreach (TestSet ts in tsArray)
				{
					ulong data = 0;

					if (!XUInt64.TryParseBinary(ts.BinString, out data))
						Assert.Fail("Failed to parse binary string"      + ts.BinString);
					if (!XUInt64.TryParseOctal (ts.OctString, out data))
						Assert.Fail("Failed to parse octal string"       + ts.OctString);
					if (!UInt64.TryParse       (ts.DecString, out data))
						Assert.Fail("Failed to parse decimal string"     + ts.DecString);
					if (!UInt64.TryParse       (ts.HexString, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out data))
						Assert.Fail("Failed to parse hexadecimal string" + ts.HexString);
				}
			}
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
