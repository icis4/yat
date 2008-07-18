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
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private struct TestSet
		{
			public readonly byte Data;

			public readonly string BinString;
			public readonly string OctString;
			public readonly string DecString;
			public readonly string HexString;

			public TestSet(byte data, string binString, string octString, string decString, string hexString)
			{
				Data = data;

				BinString = binString;
				OctString = octString;
				DecString = decString;
				HexString = hexString;
			}
		};

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private readonly TestSet[] _testSets =
		{
			new TestSet(	  0, "00000000", "000", "000", "00" ),
			new TestSet(	  1, "00000001", "001", "001", "01" ),
			new TestSet(	127, "01111111", "177", "127", "7F" ),
			new TestSet(	128, "10000000", "200", "128", "80" ),
			new TestSet(	129, "10000001", "201", "129", "81" ),
			new TestSet(	254, "11111110", "376", "254", "FE" ),
			new TestSet(	255, "11111111", "377", "255", "FF" ),
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

		[Test]
		public void TestToString()
		{
			foreach (TestSet ts in _testSets)
			{
				byte data = ts.Data;

				Assert.AreEqual(ts.BinString, XByte.ConvertToBinaryString(data));
				Assert.AreEqual(ts.OctString, XByte.ConvertToOctalString(data));
				Assert.AreEqual(ts.DecString, data.ToString("D3"));
				Assert.AreEqual(ts.HexString, data.ToString("X2"));
			}
		}

		#endregion

		#endregion
	}
}
