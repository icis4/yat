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

using System.Globalization;

using NUnit.Framework;

using MKY.Utilities.Types;

namespace MKY.Utilities.Test.Types
{
	/// <summary></summary>
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
		}

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private readonly TestSet[] testSets =
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

		/// <summary></summary>
		[Test]
		public virtual void TestToString()
		{
			foreach (TestSet ts in this.testSets)
			{
				byte data = ts.Data;

				Assert.AreEqual(ts.BinString, XByte.ConvertToBinaryString(data));
				Assert.AreEqual(ts.OctString, XByte.ConvertToOctalString(data));
				Assert.AreEqual(ts.DecString, data.ToString("D3", CultureInfo.InvariantCulture));
				Assert.AreEqual(ts.HexString, data.ToString("X2", CultureInfo.InvariantCulture));
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
