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
// Copyright © 2007-2019 Matthias Kläy.
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
using System.Diagnostics.CodeAnalysis;

using MKY.Time;

using NUnit.Framework;

#endregion

namespace MKY.Test.Time
{
	/// <summary></summary>
	public static class TimeSpanTestData
	{
		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		public static IEnumerable TestCasesFormatInvariantSeconds
		{
			get
			{
				yield return (new TestCaseData(new TimeSpan(     0,  0,  0),                    "0"));
				yield return (new TestCaseData(new TimeSpan(     0,  0,  4),                    "4"));
				yield return (new TestCaseData(new TimeSpan(     0,  3,  4),                 "3:04"));
				yield return (new TestCaseData(new TimeSpan(     2,  3,  4),              "2:03:04"));
				yield return (new TestCaseData(new TimeSpan( 1,  2,  3,  4),        "1 day 2:03:04"));
				yield return (new TestCaseData(new TimeSpan( 1,  2,  3,  4,  5),    "1 day 2:03:04"));
				yield return (new TestCaseData(new TimeSpan(10, 12, 13, 14, 15), "10 days 12:13:14"));
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCasesFormatInvariantThousandths
		{
			get
			{
				yield return (new TestCaseData(new TimeSpan(     0,  0,  0),                    "0.000"));
				yield return (new TestCaseData(new TimeSpan(     0,  0,  4),                    "4.000"));
				yield return (new TestCaseData(new TimeSpan(     0,  3,  4),                 "3:04.000"));
				yield return (new TestCaseData(new TimeSpan(     2,  3,  4),              "2:03:04.000"));
				yield return (new TestCaseData(new TimeSpan( 1,  2,  3,  4),        "1 day 2:03:04.000"));
				yield return (new TestCaseData(new TimeSpan( 1,  2,  3,  4,  5),    "1 day 2:03:04.005"));
				yield return (new TestCaseData(new TimeSpan(10, 12, 13, 14, 15), "10 days 12:13:14.015"));
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCasesFormatInvariantSecondsEnforceMinutes
		{
			get
			{
				yield return (new TestCaseData(new TimeSpan(     0,  0,  0),                 "0:00"));
				yield return (new TestCaseData(new TimeSpan(     0,  0,  4),                 "0:04"));
				yield return (new TestCaseData(new TimeSpan(     0,  3,  4),                 "3:04"));
				yield return (new TestCaseData(new TimeSpan(     2,  3,  4),              "2:03:04"));
				yield return (new TestCaseData(new TimeSpan( 1,  2,  3,  4),        "1 day 2:03:04"));
				yield return (new TestCaseData(new TimeSpan( 1,  2,  3,  4,  5),    "1 day 2:03:04"));
				yield return (new TestCaseData(new TimeSpan(10, 12, 13, 14, 15), "10 days 12:13:14"));
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCasesFormatInvariantThousandthsEnforceMinutes
		{
			get
			{
				yield return (new TestCaseData(new TimeSpan(     0,  0,  0),                 "0:00.000"));
				yield return (new TestCaseData(new TimeSpan(     0,  0,  4),                 "0:04.000"));
				yield return (new TestCaseData(new TimeSpan(     0,  3,  4),                 "3:04.000"));
				yield return (new TestCaseData(new TimeSpan(     2,  3,  4),              "2:03:04.000"));
				yield return (new TestCaseData(new TimeSpan( 1,  2,  3,  4),        "1 day 2:03:04.000"));
				yield return (new TestCaseData(new TimeSpan( 1,  2,  3,  4,  5),    "1 day 2:03:04.005"));
				yield return (new TestCaseData(new TimeSpan(10, 12, 13, 14, 15), "10 days 12:13:14.015"));
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCasesFormatAdditional
		{
			get
			{
				yield return (new TestCaseData(new TimeSpan(0, 0, 0, 0, 0), "^ffffff^",   "000000"));
				yield return (new TestCaseData(new TimeSpan(0, 0, 0, 0, 0), "^sss.sss^", "000.000"));
				yield return (new TestCaseData(new TimeSpan(0, 0, 0, 0, 0), "^mm.mmm^",   "00.000"));
				yield return (new TestCaseData(new TimeSpan(0, 0, 0, 0, 0), "^h.hhh^",     "0.000"));
				yield return (new TestCaseData(new TimeSpan(0, 0, 0, 0, 0), "^d.ddd^",     "0.000"));

				yield return (new TestCaseData(new TimeSpan(10, 12, 13, 14, 15), "^ffffff^",   "907994015"));
				yield return (new TestCaseData(new TimeSpan(10, 12, 13, 14, 15), "^sss.sss^", "907994.015"));
				yield return (new TestCaseData(new TimeSpan(10, 12, 13, 14, 15), "^mm.mmm^",   "15133.234"));
				yield return (new TestCaseData(new TimeSpan(10, 12, 13, 14, 15), "^h.hhh^",      "252.221"));
				yield return (new TestCaseData(new TimeSpan(10, 12, 13, 14, 15), "^d.ddd^",       "10.509"));
			}
		}
		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class TimeSpanTest
	{
		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		/// <summary></summary>
		[Test, TestCaseSource(typeof(TimeSpanTestData), "TestCasesFormatInvariantSeconds")]
		public virtual void TestFormatInvariantSeconds(TimeSpan value, string expected)
		{
			string actual = TimeSpanEx.FormatInvariantSeconds(value);
			Assert.That(actual, Is.EqualTo(expected));
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(TimeSpanTestData), "TestCasesFormatInvariantThousandths")]
		public virtual void TestFormatInvariantThousandths(TimeSpan value, string expected)
		{
			string actual = TimeSpanEx.FormatInvariantThousandths(value);
			Assert.That(actual, Is.EqualTo(expected));
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(TimeSpanTestData), "TestCasesFormatInvariantSecondsEnforceMinutes")]
		public virtual void TestFormatInvariantSecondsEnforceMinutes(TimeSpan value, string expected)
		{
			string actual = TimeSpanEx.FormatInvariantSecondsEnforceMinutes(value);
			Assert.That(actual, Is.EqualTo(expected));
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(TimeSpanTestData), "TestCasesFormatInvariantThousandthsEnforceMinutes")]
		public virtual void TestFormatInvariantThousandthsEnforceMinutes(TimeSpan value, string expected)
		{
			string actual = TimeSpanEx.FormatInvariantThousandthsEnforceMinutes(value);
			Assert.That(actual, Is.EqualTo(expected));
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(TimeSpanTestData), "TestCasesFormatAdditional")]
		public virtual void TestFormatAdditional(TimeSpan value, string additionalFormat, string expected)
		{
			string actual = TimeSpanEx.FormatInvariantThousandths(value, additionalFormat);
			Assert.That(actual, Is.EqualTo(expected));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
