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
using System.Diagnostics.CodeAnalysis;

using NUnit.Framework;

namespace MKY.Test.Types
{
	/// <summary></summary>
	public static class DoubleExTestData
	{
		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			[SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1021:NegativeSignsMustBeSpacedCorrectly", Justification = "Alignment.")]
			[SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1022:PositiveSignsMustBeSpacedCorrectly", Justification = "Alignment.")]
			get
			{
				yield return (new TestCaseData(double.MaxValue, (double.MaxValue - (100 * double.Epsilon)), true));
				yield return (new TestCaseData(           +1.0,            (+1.0 - (100 * double.Epsilon)), true));
				yield return (new TestCaseData(           +1.0,             +1.0,                           true));
				yield return (new TestCaseData(            0.0,             (0.0 + (100 * double.Epsilon)), true));
				yield return (new TestCaseData(            0.0,              0.0,                           true));
				yield return (new TestCaseData(            0.0,             (0.0 - (100 * double.Epsilon)), true));
				yield return (new TestCaseData(           -1.0,             -1.0,                           true));
				yield return (new TestCaseData(           -1.0,            (-1.0 + (100 * double.Epsilon)), true));
				yield return (new TestCaseData(double.MinValue, (double.MinValue + (100 * double.Epsilon)), true));

				yield return (new TestCaseData(0.00000000000012, 0.00000000000013, false));
			}
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class DoubleExTest
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
		[Test, TestCaseSource(typeof(DoubleExTestData), "TestCases")]
		public virtual void Rather(double lhs, double rhs, bool equals)
		{
			if (equals)
			{
				Assert.That(DoubleEx.RatherEquals(   lhs, rhs), Is.True);
				Assert.That(DoubleEx.RatherNotEquals(lhs, rhs), Is.False);
			}
			else
			{
				Assert.That(DoubleEx.RatherEquals(   lhs, rhs), Is.False);
				Assert.That(DoubleEx.RatherNotEquals(lhs, rhs), Is.True);
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
