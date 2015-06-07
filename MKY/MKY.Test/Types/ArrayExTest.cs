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
// MKY Development Version 1.0.13
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2015 Matthias Kläy.
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

#endregion

namespace MKY.Test.Types
{
	/// <summary></summary>
	public static class ArrayExTestData
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private enum SimpleEnum
		{
			A,
			B,
			C,
			D,
		}

		private class SimpleType
		{
			/// <summary></summary>
			public readonly int A;

			/// <summary></summary>
			public readonly double B;

			/// <summary></summary>
			public SimpleType(int a, double b)
			{
				A = a;
				B = b;
			}

			/// <summary>
			/// Determines whether this instance and the specified object have value equality.
			/// </summary>
			/// <remarks>
			/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
			/// properties, i.e. properties with some logic, are also properly handled.
			/// </remarks>
			public override bool Equals(object obj)
			{
				if (ReferenceEquals(obj, null))
					return (false);

				if (GetType() != obj.GetType())
					return (false);

				SimpleType other = (SimpleType)obj;
				return ((A == other.A) && (B == other.B));
			}

			/// <summary>
			/// Serves as a hash function for a particular type.
			/// </summary>
			/// <remarks>
			/// Use properties instead of fields to calculate hash code. This ensures that 'intelligent'
			/// properties, i.e. properties with some logic, are also properly handled.
			/// </remarks>
			public override int GetHashCode()
			{
				return (A.GetHashCode() ^ B.GetHashCode());
			}
		}

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private static readonly int[][] IntArrays = new int[][]
			{
				new int[] { 0, 1, 2 },
				new int[] { 0, 1, 2 },
				new int[] { 1, 2, 3 },
			};

		private static readonly string[][] StringArrays = new string[][]
			{
				new string[] { "AAA", "BBB", "CCC" },
				new string[] { "AAA", "BBB", "CCC" },
				new string[] { "BBB", "CCC", "DDD" },
			};

		private static readonly SimpleEnum[][] EnumArrays = new SimpleEnum[][]
			{
				new SimpleEnum[] { SimpleEnum.A, SimpleEnum.B, SimpleEnum.C },
				new SimpleEnum[] { SimpleEnum.A, SimpleEnum.B, SimpleEnum.C },
				new SimpleEnum[] { SimpleEnum.B, SimpleEnum.C, SimpleEnum.D },
			};

		private static readonly SimpleType[][] ObjectArrays = new SimpleType[][]
			{
				new SimpleType[] { new SimpleType(0, 1.0), new SimpleType(1, 1.1), new SimpleType(2, 1.2) },
				new SimpleType[] { new SimpleType(0, 1.0), new SimpleType(1, 1.1), new SimpleType(2, 1.2) },
				new SimpleType[] { new SimpleType(1, 1.1), new SimpleType(2, 1.2), new SimpleType(3, 1.3) },
			};

		private static readonly string[][] ArraysWithNull = new string[][]
			{
				new string[] { "AAA", "BBB" },
				new string[] { "AAA", null },
				new string[] { "AAA", null },
				new string[] { null, null },
				new string[] { null, null },
			};

		private static readonly string[][] ArraysOdd = new string[][]
			{
				new string[] { "AAA" },
				new string[] { "AAA", "BBB" },
				new string[] { "AAA", "BBB", "CCC" },
			};

		#endregion

		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			get
			{
				yield return (new TestCaseData(IntArrays[0], IntArrays[0], true) .SetName("IntArrays_ReferenceEquals"));
				yield return (new TestCaseData(IntArrays[0], IntArrays[1], true) .SetName("IntArrays_ValueEquals"));
				yield return (new TestCaseData(IntArrays[0], IntArrays[2], false).SetName("IntArrays_Differs"));

				yield return (new TestCaseData(StringArrays[0], StringArrays[0], true) .SetName("StringArrays_ReferenceEquals"));
				yield return (new TestCaseData(StringArrays[0], StringArrays[1], true) .SetName("StringArrays_ValueEquals"));
				yield return (new TestCaseData(StringArrays[0], StringArrays[2], false).SetName("StringArrays_Differs"));

				yield return (new TestCaseData(EnumArrays[0], EnumArrays[0], true) .SetName("EnumArrays_ReferenceEquals"));
				yield return (new TestCaseData(EnumArrays[0], EnumArrays[1], true) .SetName("EnumArrays_ValueEquals"));
				yield return (new TestCaseData(EnumArrays[0], EnumArrays[2], false).SetName("EnumArrays_Differs"));

				yield return (new TestCaseData(ObjectArrays[0], ObjectArrays[0], true) .SetName("ObjectArrays_ReferenceEquals"));
				yield return (new TestCaseData(ObjectArrays[0], ObjectArrays[1], true) .SetName("ObjectArrays_ValueEquals"));
				yield return (new TestCaseData(ObjectArrays[0], ObjectArrays[2], false).SetName("ObjectArrays_Differs"));

				yield return (new TestCaseData(ArraysWithNull[0], ArraysWithNull[0], true) .SetName("ArraysWithNull_ReferenceEquals"));
				yield return (new TestCaseData(ArraysWithNull[0], ArraysWithNull[1], false).SetName("ArraysWithNull_DiffersInNull"));
				yield return (new TestCaseData(ArraysWithNull[1], ArraysWithNull[2], true) .SetName("ArraysWithNull_EqualsInNull"));
				yield return (new TestCaseData(ArraysWithNull[0], ArraysWithNull[3], false).SetName("ArraysWithNull_DiffersInNull"));
				yield return (new TestCaseData(ArraysWithNull[1], ArraysWithNull[3], false).SetName("ArraysWithNull_DiffersInNull"));
				yield return (new TestCaseData(ArraysWithNull[3], ArraysWithNull[4], true) .SetName("ArraysWithNull_EqualsInNull"));

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
			Assert.AreEqual(ArrayEx.ValuesEqual(objA, objB), equals);
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
