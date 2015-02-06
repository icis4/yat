//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.10
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Collections;

using NUnit.Framework;

namespace MKY.Test.Types
{
	/// <summary></summary>
	public static class Int32ExTestData
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
				yield return (new TestCaseData( 0,  0, -1,  true,  true));
				yield return (new TestCaseData( 0,  0,  0,  true,  true));
				yield return (new TestCaseData( 0,  0,  1,  true,  true));

				yield return (new TestCaseData( 0,  1, -1,  true, false));
				yield return (new TestCaseData( 0,  1,  0,  true, false));
				yield return (new TestCaseData( 0,  1,  1, false,  true));
				yield return (new TestCaseData( 0,  1,  2, false,  true));

				yield return (new TestCaseData(-1,  0, -2,  true, false));
				yield return (new TestCaseData(-1,  0, -1,  true, false));
				yield return (new TestCaseData(-1,  0,  0, false,  true));
				yield return (new TestCaseData(-1,  0,  1, false,  true));

				yield return (new TestCaseData(-1,  1, -2,  true, false));
				yield return (new TestCaseData(-1,  1, -1,  true, false));
				yield return (new TestCaseData(-1,  1,  0, false, false));
				yield return (new TestCaseData(-1,  1,  1, false,  true));
				yield return (new TestCaseData(-1,  1,  2, false,  true));

				yield return (new TestCaseData(int.MinValue, int.MaxValue,            0, false, false));
				yield return (new TestCaseData(int.MinValue, int.MaxValue, int.MinValue,  true, false));
				yield return (new TestCaseData(int.MinValue, int.MaxValue, int.MaxValue, false,  true));

				yield return (new TestCaseData(int.MinValue, int.MaxValue, int.MinValue + 1, false, false));
				yield return (new TestCaseData(int.MinValue, int.MaxValue, int.MaxValue - 1, false, false));

				yield return (new TestCaseData(int.MinValue + 1, int.MaxValue - 1, int.MinValue,      true, false));
				yield return (new TestCaseData(int.MinValue + 1, int.MaxValue - 1, int.MinValue + 1,  true, false));
				yield return (new TestCaseData(int.MinValue + 1, int.MaxValue - 1, int.MinValue + 2, false, false));
				yield return (new TestCaseData(int.MinValue + 1, int.MaxValue - 1, int.MaxValue - 2, false, false));
				yield return (new TestCaseData(int.MinValue + 1, int.MaxValue - 1, int.MaxValue - 1, false,  true));
				yield return (new TestCaseData(int.MinValue + 1, int.MaxValue - 1, int.MaxValue,     false,  true));
			}
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class Int32ExTest
	{
		#region Tests
		//==========================================================================================
		// Test
		//==========================================================================================

		#region Tests > LimitToBounds()
		//------------------------------------------------------------------------------------------
		// Tests > LimitToBounds()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Int32ExTestData), "TestCases")]
		public virtual void TestLimitToBounds(int min, int max, int value, bool valueMinimized, bool valueMaximized)
		{
			int limited = Int32Ex.LimitToBounds(value, min, max);

			if (valueMinimized)
				Assert.AreEqual(min, limited);

			if (valueMaximized)
				Assert.AreEqual(max, limited);
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
