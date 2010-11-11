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

using System.Collections;
using System.Globalization;

using NUnit.Framework;

using MKY.Types;

namespace MKY.Test.Types
{
	/// <summary></summary>
	public static class ByteExTestData
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
				yield return (new TestCaseData((byte)  0, "00000000", "000", "000", "00"));
				yield return (new TestCaseData((byte)  1, "00000001", "001", "001", "01"));
				yield return (new TestCaseData((byte)127, "01111111", "177", "127", "7F"));
				yield return (new TestCaseData((byte)128, "10000000", "200", "128", "80"));
				yield return (new TestCaseData((byte)129, "10000001", "201", "129", "81"));
				yield return (new TestCaseData((byte)254, "11111110", "376", "254", "FE"));
				yield return (new TestCaseData((byte)255, "11111111", "377", "255", "FF"));
			}
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class ByteExTest
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
		[Test, TestCaseSource(typeof(ByteExTestData), "TestCases")]
		public virtual void TestToString(byte data, string binString, string octString, string decString, string hexString)
		{
			Assert.AreEqual(binString, ByteEx.ConvertToBinaryString(data));
			Assert.AreEqual(octString, ByteEx.ConvertToOctalString(data));
			Assert.AreEqual(decString, data.ToString("D3", CultureInfo.InvariantCulture));
			Assert.AreEqual(hexString, data.ToString("X2", CultureInfo.InvariantCulture));
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
