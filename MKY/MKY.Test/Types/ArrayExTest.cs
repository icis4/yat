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

using System;
using System.Collections;

using NUnit.Framework;

namespace MKY.Test.Types
{
	/// <summary></summary>
	public static class ArrayExTestData
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <summary></summary>
		private static readonly int[] EmptyArray = new int[] { };

		/// <summary></summary>
		private static readonly int[] SingleItem = new int[] { 1 };

		/// <summary></summary>
		internal static readonly int[][] IntArrays = new int[][]
		{
			new int[] { 0, 1, 2 },
			new int[] { 0, 1, 2 },
			new int[] { 1, 2, 3 }
		};

		/// <summary></summary>
		internal static readonly EnumType[][] EnumArrays = new EnumType[][]
		{
			new EnumType[] { EnumType.A, EnumType.B, EnumType.C },
			new EnumType[] { EnumType.A, EnumType.B, EnumType.C },
			new EnumType[] { EnumType.B, EnumType.C, EnumType.D }
		};

		/// <summary></summary>
		internal static readonly EquatableReferenceType[][] ReferenceTypeArrays = new EquatableReferenceType[][]
		{
			new EquatableReferenceType[] { new EquatableReferenceType(0, 1.0), new EquatableReferenceType(1, 1.1), new EquatableReferenceType(2, 1.2) },
			new EquatableReferenceType[] { new EquatableReferenceType(0, 1.0), new EquatableReferenceType(1, 1.1), new EquatableReferenceType(2, 1.2) },
			new EquatableReferenceType[] { new EquatableReferenceType(1, 1.1), new EquatableReferenceType(2, 1.2), new EquatableReferenceType(3, 1.3) }
		};

		/// <summary></summary>
		internal static readonly string[][] StringArrays = new string[][]
		{
			new string[] { "AAA", "BBB", "CCC" },
			new string[] { "AAA", "BBB", "CCC" },
			new string[] { "BBB", "CCC", "DDD" }
		};

		/// <summary></summary>
		internal static readonly string[][] ArraysWithNull = new string[][]
		{
			new string[] { "AAA", "BBB" },
			new string[] { "AAA", null },
			new string[] { "AAA", null },
			new string[] { null, null },
			new string[] { null, null }
		};

		/// <summary></summary>
		internal static readonly string[][] ArraysOdd = new string[][]
		{
			new string[] { "AAA" },
			new string[] { "AAA", "BBB" },
			new string[] { "AAA", "BBB", "CCC" }
		};

		#endregion

		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			// Attention:
			// Similar code exists in Collections.ObjectModel.ReadOnlyCollectionExTestData.
			// Changes here may have to be applied there too.

			get
			{
				yield return (new TestCaseData(EmptyArray, SingleItem, false).SetName("1stIsEmpty"));
				yield return (new TestCaseData(SingleItem, EmptyArray, false).SetName("2ndIsEmpty"));
				yield return (new TestCaseData(EmptyArray, EmptyArray, true).SetName("BothAreEmpty"));

				yield return (new TestCaseData(null,       SingleItem, false).SetName("1stIsNull"));
				yield return (new TestCaseData(SingleItem, null,       false).SetName("2ndIsNull"));
				yield return (new TestCaseData(null,       null,       true).SetName("BothAreNull"));

				yield return (new TestCaseData(IntArrays[0], IntArrays[0], true) .SetName("IntArrays_ReferenceEquals"));
				yield return (new TestCaseData(IntArrays[0], IntArrays[1], true) .SetName("IntArrays_ValuesEqual"));
				yield return (new TestCaseData(IntArrays[0], IntArrays[2], false).SetName("IntArrays_Differs"));

				yield return (new TestCaseData(EnumArrays[0], EnumArrays[0], true) .SetName("EnumArrays_ReferenceEquals"));
				yield return (new TestCaseData(EnumArrays[0], EnumArrays[1], true) .SetName("EnumArrays_ValuesEqual"));
				yield return (new TestCaseData(EnumArrays[0], EnumArrays[2], false).SetName("EnumArrays_Differs"));

				yield return (new TestCaseData(ReferenceTypeArrays[0], ReferenceTypeArrays[0], true) .SetName("ReferenceTypeArrays_ReferenceEquals"));
				yield return (new TestCaseData(ReferenceTypeArrays[0], ReferenceTypeArrays[1], true) .SetName("ReferenceTypeArrays_ValuesEqual"));
				yield return (new TestCaseData(ReferenceTypeArrays[0], ReferenceTypeArrays[2], false).SetName("ReferenceTypeArrays_Differs"));

				yield return (new TestCaseData(StringArrays[0], StringArrays[0], true) .SetName("StringArrays_ReferenceEquals"));
				yield return (new TestCaseData(StringArrays[0], StringArrays[1], true) .SetName("StringArrays_ValuesEqual"));
				yield return (new TestCaseData(StringArrays[0], StringArrays[2], false).SetName("StringArrays_Differs"));

				yield return (new TestCaseData(ArraysWithNull[0], ArraysWithNull[0], true) .SetName("ArraysWithNull_ReferenceEquals"));
				yield return (new TestCaseData(ArraysWithNull[0], ArraysWithNull[1], false).SetName("ArraysWithNull_DiffersInNull"));
				yield return (new TestCaseData(ArraysWithNull[1], ArraysWithNull[2], true) .SetName("ArraysWithNull_EqualsInNull"));
				yield return (new TestCaseData(ArraysWithNull[0], ArraysWithNull[3], false).SetName("ArraysWithNull_DiffersInNull"));
				yield return (new TestCaseData(ArraysWithNull[1], ArraysWithNull[3], false).SetName("ArraysWithNull_DiffersInNull"));
				yield return (new TestCaseData(ArraysWithNull[3], ArraysWithNull[4], true) .SetName("ArraysWithNull_EqualsInNull"));

				yield return (new TestCaseData(ArraysOdd[1], ArraysOdd[0], false).SetName("ArraysOdd_DiffersIn1st"));
				yield return (new TestCaseData(ArraysOdd[0], ArraysOdd[1], false).SetName("ArraysOdd_DiffersIn2nd"));
				yield return (new TestCaseData(ArraysOdd[0], ArraysOdd[2], false).SetName("ArraysOdd_DiffersIn2ndAnd3rd"));
				yield return (new TestCaseData(ArraysOdd[1], ArraysOdd[2], false).SetName("ArraysOdd_DiffersIn3rd"));
			}
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class ArrayExTest
	{
		#region Tests
		//==========================================================================================
		// Test
		//==========================================================================================

		#region Tests > ValuesEqual()
		//------------------------------------------------------------------------------------------
		// Tests > ValuesEqual()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, TestCaseSource(typeof(ArrayExTestData), "TestCases")]
		public virtual void TestValuesEqual(Array objA, Array objB, bool equals)
		{
			Assert.That(ArrayEx.ValuesEqual(objA, objB), Is.EqualTo(equals));
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
