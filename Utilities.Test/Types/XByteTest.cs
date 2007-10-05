using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using MKY.Utilities.Types;

namespace MKY.Utilities.Test.Types
{
	[TestFixture]
	public class XByteTest
	{

		//------------------------------------------------------------------------------------------
		// Types
		//------------------------------------------------------------------------------------------

		private struct TestSet
		{
			public byte Data;
			public string BinString;
			public string OctString;
			public string DecString;
			public string HexString;

			public TestSet(byte data, string binString, string octString, string decString, string hexString)
			{
				Data = data;
				BinString = binString;
				OctString = octString;
				DecString = decString;
				HexString = hexString;
			}
		};

		//------------------------------------------------------------------------------------------
		// Fields
		//------------------------------------------------------------------------------------------

		private TestSet[] _testSets =
		{
			new TestSet(	  0,	"00000000",	"000",	"000",	"00"	),
			new TestSet(	  1,	"00000001",	"001",	"001",	"01"	),
			new TestSet(	127,	"01111111",	"177",	"127",	"7F"	),
			new TestSet(	128,	"10000000",	"200",	"128",	"80"	),
			new TestSet(	129,	"10000001",	"201",	"129",	"81"	),
			new TestSet(	254,	"11111110",	"376",	"254",	"FE"	),
			new TestSet(	255,	"11111111",	"377",	"255",	"FF"	),
		};

		#region Test ToString()
		//------------------------------------------------------------------------------------------
		// Test ToString()
		//------------------------------------------------------------------------------------------

		[Test]
		public void TestToString()
		{
			byte data = 0;
			string expectedBinString = "";
			string expectedOctString = "";
			string expectedDecString = "";
			string expectedHexString = "";

			foreach (TestSet ts in _testSets)
			{
				data = ts.Data;
				expectedBinString = ts.BinString;
				expectedOctString = ts.OctString;
				expectedDecString = ts.DecString;
				expectedHexString = ts.HexString;

				Assert.AreEqual(expectedBinString, XByte.ConvertToBinaryString(data));
				Assert.AreEqual(expectedOctString, XByte.ConvertToOctalString(data));
				Assert.AreEqual(expectedDecString, data.ToString("D3"));
				Assert.AreEqual(expectedHexString, data.ToString("X2"));
			}
		}

		#endregion
	}
}
