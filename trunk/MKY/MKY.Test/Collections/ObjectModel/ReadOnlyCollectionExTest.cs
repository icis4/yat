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
using System.Collections.Generic;
using System.Collections.ObjectModel;

using MKY.Collections.ObjectModel;
using MKY.Test.Types;

using NUnit.Framework;

namespace MKY.Test.Collections.ObjectModel
{
	/// <summary></summary>
	public static class ReadOnlyCollectionExTestData
	{
		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		private static ReadOnlyCollection<int> EmptyCollection
		{
			get
			{
				var l = new List<int>(0);
				return (new ReadOnlyCollection<int>(l));
			}
		}

		private static ReadOnlyCollection<int> SingleItem
		{
			get
			{
				var l = new List<int>(1);
				l.Add(1);
				return (new ReadOnlyCollection<int>(l));
			}
		}

		private static ReadOnlyCollection<int>[] IntCollections
		{
			get
			{
				var c = new List<ReadOnlyCollection<int>>();

				foreach (var a in ArrayExTestData.IntArrays)
					c.Add(new ReadOnlyCollection<int>(a));

				return (c.ToArray());
			}
		}

		private static ReadOnlyCollection<EnumType>[] EnumCollections
		{
			get
			{
				var c = new List<ReadOnlyCollection<EnumType>>();

				foreach (var a in ArrayExTestData.EnumArrays)
					c.Add(new ReadOnlyCollection<EnumType>(a));

				return (c.ToArray());
			}
		}

		private static ReadOnlyCollection<EquatableReferenceType>[] ReferenceTypeCollections
		{
			get
			{
				var c = new List<ReadOnlyCollection<EquatableReferenceType>>();

				foreach (var a in ArrayExTestData.ReferenceTypeArrays)
					c.Add(new ReadOnlyCollection<EquatableReferenceType>(a));

				return (c.ToArray());
			}
		}

		private static ReadOnlyCollection<string>[] StringCollections
		{
			get
			{
				var c = new List<ReadOnlyCollection<string>>();

				foreach (var a in ArrayExTestData.StringArrays)
					c.Add(new ReadOnlyCollection<string>(a));

				return (c.ToArray());
			}
		}

		private static ReadOnlyCollection<string>[] CollectionsWithNull
		{
			get
			{
				var c = new List<ReadOnlyCollection<string>>();

				foreach (var a in ArrayExTestData.ArraysWithNull)
					c.Add(new ReadOnlyCollection<string>(a));

				return (c.ToArray());
			}
		}

		private static ReadOnlyCollection<string>[] CollectionsOdd
		{
			get
			{
				var c = new List<ReadOnlyCollection<string>>();

				foreach (var a in ArrayExTestData.ArraysOdd)
					c.Add(new ReadOnlyCollection<string>(a));

				return (c.ToArray());
			}
		}

		#endregion

		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		public static IEnumerable TestCasesInt
		{
			// Attention:
			// Similar code exists in ArrayExTestData.TestCases.
			// Changes here may have to be applied there too.

			get
			{
				yield return (new TestCaseData(EmptyCollection, SingleItem,      false).SetName("1stIsEmpty"));
				yield return (new TestCaseData(SingleItem,      EmptyCollection, false).SetName("2ndIsEmpty"));
				yield return (new TestCaseData(EmptyCollection, EmptyCollection, true).SetName("BothAreEmpty"));

				yield return (new TestCaseData(null,       SingleItem, false).SetName("1stIsNull"));
				yield return (new TestCaseData(SingleItem, null,       false).SetName("2ndIsNull"));
				yield return (new TestCaseData(null,       null,       true).SetName("BothAreNull"));

				yield return (new TestCaseData(IntCollections[0], IntCollections[0], true) .SetName("IntCollections_ReferenceEquals"));
				yield return (new TestCaseData(IntCollections[0], IntCollections[1], true) .SetName("IntCollections_ItemsEqual"));
				yield return (new TestCaseData(IntCollections[0], IntCollections[2], false).SetName("IntCollections_Differs"));
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCasesEnum
		{
			// Attention:
			// Similar code exists in ArrayExTestData.TestCases.
			// Changes here may have to be applied there too.

			get
			{
				yield return (new TestCaseData(EnumCollections[0], EnumCollections[0], true) .SetName("EnumCollections_ReferenceEquals"));
				yield return (new TestCaseData(EnumCollections[0], EnumCollections[1], true) .SetName("EnumCollections_ItemsEqual"));
				yield return (new TestCaseData(EnumCollections[0], EnumCollections[2], false).SetName("EnumCollections_Differs"));
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCasesReferenceType
		{
			// Attention:
			// Similar code exists in ArrayExTestData.TestCases.
			// Changes here may have to be applied there too.

			get
			{
				yield return (new TestCaseData(ReferenceTypeCollections[0], ReferenceTypeCollections[0], true) .SetName("ReferenceTypeCollections_ReferenceEquals"));
				yield return (new TestCaseData(ReferenceTypeCollections[0], ReferenceTypeCollections[1], true) .SetName("ReferenceTypeCollections_ItemsEqual"));
				yield return (new TestCaseData(ReferenceTypeCollections[0], ReferenceTypeCollections[2], false).SetName("ReferenceTypeCollections_Differs"));
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCasesString
		{
			// Attention:
			// Similar code exists in ArrayExTestData.TestCases.
			// Changes here may have to be applied there too.

			get
			{
				yield return (new TestCaseData(StringCollections[0], StringCollections[0], true) .SetName("StringCollections_ReferenceEquals"));
				yield return (new TestCaseData(StringCollections[0], StringCollections[1], true) .SetName("StringCollections_ItemsEqual"));
				yield return (new TestCaseData(StringCollections[0], StringCollections[2], false).SetName("StringCollections_Differs"));

				yield return (new TestCaseData(CollectionsWithNull[0], CollectionsWithNull[0], true) .SetName("CollectionsWithNull_ReferenceEquals"));
				yield return (new TestCaseData(CollectionsWithNull[0], CollectionsWithNull[1], false).SetName("CollectionsWithNull_DiffersInNull"));
				yield return (new TestCaseData(CollectionsWithNull[1], CollectionsWithNull[2], true) .SetName("CollectionsWithNull_EqualsInNull"));
				yield return (new TestCaseData(CollectionsWithNull[0], CollectionsWithNull[3], false).SetName("CollectionsWithNull_DiffersInNull"));
				yield return (new TestCaseData(CollectionsWithNull[1], CollectionsWithNull[3], false).SetName("CollectionsWithNull_DiffersInNull"));
				yield return (new TestCaseData(CollectionsWithNull[3], CollectionsWithNull[4], true) .SetName("CollectionsWithNull_EqualsInNull"));

				yield return (new TestCaseData(CollectionsOdd[1], CollectionsOdd[0], false).SetName("CollectionsOdd_DiffersIn1st"));
				yield return (new TestCaseData(CollectionsOdd[0], CollectionsOdd[1], false).SetName("CollectionsOdd_DiffersIn2nd"));
				yield return (new TestCaseData(CollectionsOdd[0], CollectionsOdd[2], false).SetName("CollectionsOdd_DiffersIn2ndAnd3rd"));
				yield return (new TestCaseData(CollectionsOdd[1], CollectionsOdd[2], false).SetName("CollectionsOdd_DiffersIn3rd"));
			}
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class ReadOnlyCollectionExTest
	{
		/// <summary></summary>
		[Test, TestCaseSource(typeof(ReadOnlyCollectionExTestData), "TestCasesInt")]
		public virtual void TestItemsEqualInt(ReadOnlyCollection<int> collectionA, ReadOnlyCollection<int> collectionB, bool equals)
		{
			Assert.That(ReadOnlyCollectionEx.ItemsEqual(collectionA, collectionB), Is.EqualTo(equals));
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(ReadOnlyCollectionExTestData), "TestCasesEnum")]
		public virtual void TestItemsEqualEnum(ReadOnlyCollection<EnumType> collectionA, ReadOnlyCollection<EnumType> collectionB, bool equals)
		{
			Assert.That(ReadOnlyCollectionEx.ItemsEqual(collectionA, collectionB), Is.EqualTo(equals));
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(ReadOnlyCollectionExTestData), "TestCasesReferenceType")]
		public virtual void TestItemsEqualReferenceType(ReadOnlyCollection<EquatableReferenceType> collectionA, ReadOnlyCollection<EquatableReferenceType> collectionB, bool equals)
		{
			Assert.That(ReadOnlyCollectionEx.ItemsEqual(collectionA, collectionB), Is.EqualTo(equals));
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(ReadOnlyCollectionExTestData), "TestCasesString")]
		public virtual void TestItemsEqualString(ReadOnlyCollection<string> collectionA, ReadOnlyCollection<string> collectionB, bool equals)
		{
			Assert.That(ReadOnlyCollectionEx.ItemsEqual(collectionA, collectionB), Is.EqualTo(equals));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
