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
// MKY Version 1.0.29
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
	public static class SingleExTestData
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
				yield return (new TestCaseData(float.MaxValue, (float.MaxValue - (100 * float.Epsilon)), true));
				yield return (new TestCaseData(         +1.0f,          (+1.0f - (100 * float.Epsilon)), true));
				yield return (new TestCaseData(         +1.0f,           +1.0f,                          true));
				yield return (new TestCaseData(          0.0f,           (0.0f + (100 * float.Epsilon)), true));
				yield return (new TestCaseData(          0.0f,            0.0f,                          true));
				yield return (new TestCaseData(          0.0f,           (0.0f - (100 * float.Epsilon)), true));
				yield return (new TestCaseData(         -1.0f,           -1.0f,                          true));
				yield return (new TestCaseData(         -1.0f,          (-1.0f + (100 * float.Epsilon)), true));
				yield return (new TestCaseData(float.MinValue, (float.MinValue + (100 * float.Epsilon)), true));

				yield return (new TestCaseData(0.000012f, 0.000013f, false));
			}
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class SingleExTest
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
		[Test, TestCaseSource(typeof(SingleExTestData), "TestCases")]
		public virtual void Rather(float lhs, float rhs, bool equals)
		{
			if (equals)
			{
				Assert.That(SingleEx.RatherEquals(   lhs, rhs), Is.True);
				Assert.That(SingleEx.RatherNotEquals(lhs, rhs), Is.False);
			}
			else
			{
				Assert.That(SingleEx.RatherEquals(   lhs, rhs), Is.False);
				Assert.That(SingleEx.RatherNotEquals(lhs, rhs), Is.True);
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
