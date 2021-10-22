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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;

using System.Collections;

using NUnit.Framework;
using NUnit.Framework.Constraints;

#endregion

namespace MKY.Test.Time
{
	/// <summary></summary>
	public static class DateTimeTestData
	{
		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		public static IEnumerable TestCasesEqualsUptoSeconds
		{
			get
			{
				yield return (new TestCaseData(new DateTime(0),                       new DateTime(0),                       Is.True));
				yield return (new TestCaseData(new DateTime(637688726423181188),      new DateTime(637688726420000000),      Is.True));
				yield return (new TestCaseData(new DateTime(DateTime.MinValue.Ticks), new DateTime(DateTime.MaxValue.Ticks), Is.False));

				yield return (new TestCaseData(new DateTime(2000, 1, 1, 12, 34, 56, 0), new DateTime(2000, 1, 1, 12, 34, 55, 999), Is.False));
				yield return (new TestCaseData(new DateTime(2000, 1, 1, 12, 34, 56, 0), new DateTime(2000, 1, 1, 12, 34, 56, 0),   Is.True));
				yield return (new TestCaseData(new DateTime(2000, 1, 1, 12, 34, 56, 0), new DateTime(2000, 1, 1, 12, 34, 56, 999), Is.True));
				yield return (new TestCaseData(new DateTime(2000, 1, 1, 12, 34, 56, 0), new DateTime(2000, 1, 1, 12, 34, 57, 0),   Is.False));
			}
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class DateTimeTest
	{
		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		/// <summary></summary>
		[Test, TestCaseSource(typeof(DateTimeTestData), "TestCasesEqualsUptoSeconds")]
		public virtual void TestEqualsUptoSeconds(DateTime valueA, DateTime valueB, IResolveConstraint constraint)
		{
			Assert.That(DateTimeEx.EqualsUptoSeconds(valueA, valueB), constraint);
			Assert.That(DateTimeEx.EqualsUptoSeconds(valueB, valueA), constraint);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
