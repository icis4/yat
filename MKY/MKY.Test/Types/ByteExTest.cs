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
// Copyright © 2007-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Collections;
using System.Globalization;

using NUnit.Framework;

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
		public virtual void TestToString(byte data, string bin, string oct, string dec, string hex)
		{
			Assert.That(ByteEx.ConvertToBinaryString(data),                Is.EqualTo(bin));
			Assert.That(ByteEx.ConvertToOctalString(data),                 Is.EqualTo(oct));
			Assert.That(data.ToString("D3", CultureInfo.InvariantCulture), Is.EqualTo(dec));
			Assert.That(data.ToString("X2", CultureInfo.InvariantCulture), Is.EqualTo(hex));
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
