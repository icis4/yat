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
// MKY Version 1.0.30
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

using System.Collections;

using NUnit.Framework;

namespace MKY.Test.Types
{
	/// <summary></summary>
	public static class DecimalExTestData
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
				yield return (new TestCaseData(decimal.MaxValue, (decimal.MaxValue - 0.00000000000000000000000000001m), true));
				yield return (new TestCaseData(           +1.0m,            (+1.0m - 0.00000000000000000000000000001m), true));
				yield return (new TestCaseData(           +1.0m,             +1.0m,                                     true));
				yield return (new TestCaseData(            0.0m,             (0.0m + 0.00000000000000000000000000001m), true));
				yield return (new TestCaseData(            0.0m,              0.0m,                                     true));
				yield return (new TestCaseData(            0.0m,             (0.0m - 0.00000000000000000000000000001m), true));
				yield return (new TestCaseData(           -1.0m,             -1.0m,                                     true));
				yield return (new TestCaseData(           -1.0m,            (-1.0m + 0.00000000000000000000000000001m), true));
				yield return (new TestCaseData(decimal.MinValue, (decimal.MinValue + 0.00000000000000000000000000001m), true));

				yield return (new TestCaseData(0.000000000000000000000000012m, 0.000000000000000000000000013m, false));
			}
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class DecimalExTest
	{
		#region Tests
		//==========================================================================================
		// Test
		//==========================================================================================

		#region Tests > Rather*()
		//------------------------------------------------------------------------------------------
		// Tests > Rather*()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, TestCaseSource(typeof(DecimalExTestData), "TestCases")]
		public virtual void Rather(decimal lhs, decimal rhs, bool equals)
		{
			if (equals)
			{
				Assert.That(DecimalEx.RatherEquals(   lhs, rhs), Is.True);
				Assert.That(DecimalEx.RatherNotEquals(lhs, rhs), Is.False);
			}
			else
			{
				Assert.That(DecimalEx.RatherEquals(   lhs, rhs), Is.False);
				Assert.That(DecimalEx.RatherNotEquals(lhs, rhs), Is.True);
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
