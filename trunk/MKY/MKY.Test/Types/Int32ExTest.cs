﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.19
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2017 Matthias Kläy.
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

		#region Tests > Limit()
		//------------------------------------------------------------------------------------------
		// Tests > Limit()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Int32ExTestData), "TestCases")]
		public virtual void Limit(int min, int max, int value, bool valueMinimized, bool valueMaximized)
		{
			int limited = Int32Ex.Limit(value, min, max);

			if (valueMinimized)
				Assert.That(limited, Is.EqualTo(min));

			if (valueMaximized)
				Assert.That(limited, Is.EqualTo(max));
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
