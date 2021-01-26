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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml.Serialization;

using MKY.IO;
using MKY.Test.Xml.Serialization;

using NUnit.Framework;

#endregion

namespace MKY.Test.Settings
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors", Justification = "Only non-static classes are permitted as test data provider for the serialization tests.")]
	public class SettingsTreeTestTypeAndData
	{
		#region IDs
		//==========================================================================================
		// IDs
		//==========================================================================================

		/// <remarks>Haven't cared for a better implementation yet...</remarks>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "A public type is required for NUnit and this type really belongs to the test data only.")]
		public enum TestCase
		{
			/// <summary></summary>
			Root,

			/// <summary></summary>
			Level1,

			/// <summary></summary>
			Level2
		}

		/// <summary></summary>
		public const int SimpleValueEqual = 1;

		/// <summary></summary>
		public const int SimpleValueNotEqual = 2;

		#endregion

		#region Object Types
		//==========================================================================================
		// Object Types
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "A public type is required for NUnit and this type really belongs to the test data only.")]
		[Serializable]
		public class TestTreeChildLevel2 : MKY.Settings.SettingsItem, IEquatable<TestTreeChildLevel2>
		{
			/// <summary></summary>
			public int SimpleValue { get; set; } = SimpleValueEqual;

			/// <summary></summary>
			public TestTreeChildLevel2()
			{
				// Nothing to do.
			}

			/// <summary></summary>
			public TestTreeChildLevel2(int simpleData)
			{
				this.SimpleValue = simpleData;
			}

			#region Object Members
			//==========================================================================================
			// Object Members
			//==========================================================================================

			/// <summary>
			/// Converts the value of this instance to its equivalent string representation.
			/// </summary>
			/// <remarks>
			/// Use properties instead of fields. This ensures that 'intelligent' properties,
			/// i.e. properties with some logic, are also properly handled.
			/// </remarks>
			public override string ToString()
			{
				return (Environment.NewLine + "        2:SimpleValue = " + SimpleValue.ToString(CultureInfo.CurrentCulture));
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
				unchecked
				{
					int hashCode = base.GetHashCode(); // Get hash code of all settings nodes.

					hashCode = (hashCode * 397) ^ SimpleValue;

					return (hashCode);
				}
			}

			/// <summary>
			/// Determines whether this instance and the specified object have reference or value equality.
			/// </summary>
			public override bool Equals(object obj)
			{
				return (Equals(obj as TestTreeChildLevel2));
			}

			/// <summary>
			/// Determines whether this instance and the specified object have reference or value equality.
			/// </summary>
			/// <remarks>
			/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
			/// properties, i.e. properties with some logic, are also properly handled.
			/// </remarks>
			public bool Equals(TestTreeChildLevel2 other)
			{
				if (ReferenceEquals(other, null)) return (false);
				if (ReferenceEquals(this, other)) return (true);
				if (GetType() != other.GetType()) return (false);

				return
				(
					base.Equals(other) && // Compare all settings nodes.

					SimpleValue.Equals(other.SimpleValue)
				);
			}

			/// <summary>
			/// Determines whether the two specified objects have reference or value equality.
			/// </summary>
			public static bool operator ==(TestTreeChildLevel2 lhs, TestTreeChildLevel2 rhs)
			{
				if (ReferenceEquals(lhs, rhs))  return (true);
				if (ReferenceEquals(lhs, null)) return (false);
				if (ReferenceEquals(rhs, null)) return (false);

				object obj = (object)lhs; // Operators are not virtual! Calling object.Equals() ensures
				return (obj.Equals(rhs)); // that a potential virtual <Derived>.Equals() is called.
			}

			/// <summary>
			/// Determines whether the two specified objects have reference and value inequality.
			/// </summary>
			public static bool operator !=(TestTreeChildLevel2 lhs, TestTreeChildLevel2 rhs)
			{
				return (!(lhs == rhs));
			}

			#endregion
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "A public type is required for NUnit and this type really belongs to the test data only.")]
		[Serializable]
		public class TestTreeChildLevel1 : MKY.Settings.SettingsItem, IEquatable<TestTreeChildLevel1>
		{
			/// <summary></summary>
			public int SimpleValue { get; set; } = SimpleValueEqual;

			/// <summary></summary>
			private TestTreeChildLevel2 childLevel2A;

			/// <summary></summary>
			private TestTreeChildLevel2 childLevel2B;

			/// <summary></summary>
			public TestTreeChildLevel1()
			{
				ChildLevel2A = new TestTreeChildLevel2();
				ChildLevel2B = new TestTreeChildLevel2();
			}

			/// <summary></summary>
			public TestTreeChildLevel1(int simpleData)
			{
				this.SimpleValue = simpleData;

				ChildLevel2A = new TestTreeChildLevel2(simpleData);
				ChildLevel2B = new TestTreeChildLevel2(simpleData);
			}

			#region Properties
			//==========================================================================================
			// Properties
			//==========================================================================================

			/// <summary></summary>
			[XmlElement("ChildLevel2A")]
			public virtual TestTreeChildLevel2 ChildLevel2A
			{
				get { return (this.childLevel2A); }
				set
				{
					if (this.childLevel2A != value)
					{
						var oldNode = this.childLevel2A;
						this.childLevel2A = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

						AttachOrReplaceOrDetachNode(oldNode, value);
					}
				}
			}

			/// <summary></summary>
			[XmlElement("ChildLevel2B")]
			public virtual TestTreeChildLevel2 ChildLevel2B
			{
				get { return (this.childLevel2B); }
				set
				{
					if (this.childLevel2B != value)
					{
						var oldNode = this.childLevel2B;
						this.childLevel2B = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

						AttachOrReplaceOrDetachNode(oldNode, value);
					}
				}
			}

			#endregion

			#region Object Members
			//==========================================================================================
			// Object Members
			//==========================================================================================

			/// <summary>
			/// Converts the value of this instance to its equivalent string representation.
			/// </summary>
			/// <remarks>
			/// Use properties instead of fields. This ensures that 'intelligent' properties,
			/// i.e. properties with some logic, are also properly handled.
			/// </remarks>
			public override string ToString()
			{
				return
				(
					Environment.NewLine + "      1:SimpleValue = "  + SimpleValue.ToString(CultureInfo.CurrentCulture) +
					Environment.NewLine + "      1:ChildLevel2A = " + ChildLevel2A.ToString() +
					Environment.NewLine + "      1:ChildLevel2B = " + ChildLevel2B.ToString()
				);
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
				unchecked
				{
					int hashCode = base.GetHashCode(); // Get hash code of all settings nodes.

					hashCode = (hashCode * 397) ^ SimpleValue;

					return (hashCode);
				}
			}

			/// <summary>
			/// Determines whether this instance and the specified object have reference or value equality.
			/// </summary>
			public override bool Equals(object obj)
			{
				return (Equals(obj as TestTreeChildLevel1));
			}

			/// <summary>
			/// Determines whether this instance and the specified object have reference or value equality.
			/// </summary>
			/// <remarks>
			/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
			/// properties, i.e. properties with some logic, are also properly handled.
			/// </remarks>
			public bool Equals(TestTreeChildLevel1 other)
			{
				if (ReferenceEquals(other, null)) return (false);
				if (ReferenceEquals(this, other)) return (true);
				if (GetType() != other.GetType()) return (false);

				return
				(
					base.Equals(other) && // Compare all settings nodes.

					SimpleValue.Equals(other.SimpleValue)
				);
			}

			/// <summary>
			/// Determines whether the two specified objects have reference or value equality.
			/// </summary>
			public static bool operator ==(TestTreeChildLevel1 lhs, TestTreeChildLevel1 rhs)
			{
				if (ReferenceEquals(lhs, rhs))  return (true);
				if (ReferenceEquals(lhs, null)) return (false);
				if (ReferenceEquals(rhs, null)) return (false);

				object obj = (object)lhs; // Operators are not virtual! Calling object.Equals() ensures
				return (obj.Equals(rhs)); // that a potential virtual <Derived>.Equals() is called.
			}

			/// <summary>
			/// Determines whether the two specified objects have reference and value inequality.
			/// </summary>
			public static bool operator !=(TestTreeChildLevel1 lhs, TestTreeChildLevel1 rhs)
			{
				return (!(lhs == rhs));
			}

			#endregion
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "A public type is required for NUnit and this type really belongs to the test data only.")]
		[Serializable]
		[XmlRoot("TestTreeRoot")]
		public class TestTreeRoot : MKY.Settings.SettingsItem, IEquatable<TestTreeRoot>
		{
			/// <summary></summary>
			public int SimpleValue { get; set; } = SimpleValueEqual;

			/// <summary></summary>
			private TestTreeChildLevel1 childLevel1;

			/// <summary></summary>
			public TestTreeRoot()
			{
				ChildLevel1 = new TestTreeChildLevel1();
			}

			/// <summary></summary>
			public TestTreeRoot(int simpleData)
			{
				this.SimpleValue = simpleData;

				ChildLevel1 = new TestTreeChildLevel1(simpleData);
			}

			#region Properties
			//==========================================================================================
			// Properties
			//==========================================================================================

			/// <summary></summary>
			[XmlElement("ChildLevel1")]
			public virtual TestTreeChildLevel1 ChildLevel1
			{
				get { return (this.childLevel1); }
				set
				{
					if (this.childLevel1 != value)
					{
						var oldNode = this.childLevel1;
						this.childLevel1 = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

						AttachOrReplaceOrDetachNode(oldNode, value);
					}
				}
			}

			#endregion

			#region Object Members
			//==========================================================================================
			// Object Members
			//==========================================================================================

			/// <summary>
			/// Converts the value of this instance to its equivalent string representation.
			/// </summary>
			/// <remarks>
			/// Use properties instead of fields. This ensures that 'intelligent' properties,
			/// i.e. properties with some logic, are also properly handled.
			/// </remarks>
			public override string ToString()
			{
				return
				(
					Environment.NewLine + "    0:SimpleValue = " + SimpleValue.ToString(CultureInfo.CurrentCulture) +
					Environment.NewLine + "    0:ChildLevel1 = " + ChildLevel1.ToString()
				);
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
				unchecked
				{
					int hashCode = base.GetHashCode(); // Get hash code of all settings nodes.

					hashCode = (hashCode * 397) ^ SimpleValue;

					return (hashCode);
				}
			}

			/// <summary>
			/// Determines whether this instance and the specified object have reference or value equality.
			/// </summary>
			public override bool Equals(object obj)
			{
				return (Equals(obj as TestTreeRoot));
			}

			/// <summary>
			/// Determines whether this instance and the specified object have reference or value equality.
			/// </summary>
			/// <remarks>
			/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
			/// properties, i.e. properties with some logic, are also properly handled.
			/// </remarks>
			public bool Equals(TestTreeRoot other)
			{
				if (ReferenceEquals(other, null)) return (false);
				if (ReferenceEquals(this, other)) return (true);
				if (GetType() != other.GetType()) return (false);

				return
				(
					base.Equals(other) && // Compare all settings nodes.

					SimpleValue.Equals(other.SimpleValue)
				);
			}

			/// <summary>
			/// Determines whether the two specified objects have reference or value equality.
			/// </summary>
			public static bool operator ==(TestTreeRoot lhs, TestTreeRoot rhs)
			{
				if (ReferenceEquals(lhs, rhs))  return (true);
				if (ReferenceEquals(lhs, null)) return (false);
				if (ReferenceEquals(rhs, null)) return (false);

				object obj = (object)lhs; // Operators are not virtual! Calling object.Equals() ensures
				return (obj.Equals(rhs)); // that a potential virtual <Derived>.Equals() is called.
			}

			/// <summary>
			/// Determines whether the two specified objects have reference and value inequality.
			/// </summary>
			public static bool operator !=(TestTreeRoot lhs, TestTreeRoot rhs)
			{
				return (!(lhs == rhs));
			}

			#endregion
		}

		#endregion
	}

	/// <summary></summary>
	[SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors", Justification = "Only non-static classes are permitted as test data provider for the serialization tests.")]
	public class SettingsEvolutionTestTypeAndData
	{
		#region Test Case Id
		//==========================================================================================
		// Test Case Id
		//==========================================================================================

		/// <remarks>Haven't cared for a better implementation yet...</remarks>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1309:FieldNamesMustNotBeginWithUnderscore", Justification = "Haven't cared for a better implementation yet...")]
		internal static class TestCaseId
		{
			public const int _11 = 11;
			public const int _12 = 12;
			public const int _13 = 13;
			public const int _21 = 21;
			public const int _22 = 22;
			public const int _23 = 23;
			public const int _31 = 31;
			public const int _32 = 32;
			public const int _33 = 33;
		}

		#endregion

		#region Object Types
		//==========================================================================================
		// Object Types
		//==========================================================================================

		/// <summary>
		/// The following classes serve to test what happens if different versions of a type are
		/// serialized/deserialized.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "A public type is required for NUnit and this type really belongs to the test data only.")]
		[Serializable]
		[XmlRoot("TestSettings")]
		public class TestClassV1
		{
			/// <summary></summary>
			public int Data1 { get; set; } = TestCaseId._11;

			/// <summary></summary>
			public int Data2 { get; set; } = TestCaseId._12;
		}

		/// <summary>
		/// V2 adds a third element.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "A public type is required for NUnit and this type really belongs to the test data only.")]
		[Serializable]
		[XmlRoot("TestSettings")]
		public class TestClassV2
		{
			/// <summary></summary>
			public int Data1 { get; set; } = TestCaseId._21;

			/// <summary></summary>
			public int Data2 { get; set; } = TestCaseId._22;

			/// <summary></summary>
			public int Data3 { get; set; } = TestCaseId._23;
		}

		/// <summary>
		/// V3 removes the second element.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "A public type is required for NUnit and this type really belongs to the test data only.")]
		[Serializable]
		[XmlRoot("TestSettings")]
		public class TestClassV3
		{
			/// <summary></summary>
			public int Data1 { get; set; } = TestCaseId._31;

			/// <summary></summary>
			public int Data3 { get; set; } = TestCaseId._33;
		}

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
				yield return (new TestCaseData(TestCaseId._11, "TestObjectV1", typeof(TestClassV1), typeof(TestClassV1)));
				yield return (new TestCaseData(TestCaseId._12, "TestObjectV1", typeof(TestClassV1), typeof(TestClassV2)));
				yield return (new TestCaseData(TestCaseId._13, "TestObjectV1", typeof(TestClassV1), typeof(TestClassV3)));
				yield return (new TestCaseData(TestCaseId._21, "TestObjectV2", typeof(TestClassV2), typeof(TestClassV1)));
				yield return (new TestCaseData(TestCaseId._22, "TestObjectV2", typeof(TestClassV2), typeof(TestClassV2)));
				yield return (new TestCaseData(TestCaseId._23, "TestObjectV2", typeof(TestClassV2), typeof(TestClassV3)));
				yield return (new TestCaseData(TestCaseId._31, "TestObjectV3", typeof(TestClassV3), typeof(TestClassV1)));
				yield return (new TestCaseData(TestCaseId._32, "TestObjectV3", typeof(TestClassV3), typeof(TestClassV2)));
				yield return (new TestCaseData(TestCaseId._33, "TestObjectV3", typeof(TestClassV3), typeof(TestClassV3)));
			}
		}

		#endregion
	}

	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "Too many values to verify.")]
	[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to verify.")]
	[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "Too many values to verify.")]
	[TestFixture]
	public class SettingsTest
	{
		#region TestFixture
		//==========================================================================================
		// TestFixture
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TearDown", Justification = "Naming according to NUnit.")]
		[TestFixtureTearDown]
		public virtual void TestFixtureTearDown()
		{
			Temp.CleanTempPath(GetType());
		}

		#endregion

		#region Tests
		//==========================================================================================
		// Test
		//==========================================================================================

		#region Tests > SettingsTree
		//------------------------------------------------------------------------------------------
		// Tests > SettingsTree
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestSettingsTreeLevel2()
		{
			MKY.Settings.SettingsItem settingsToCompareAgainst = new SettingsTreeTestTypeAndData.TestTreeChildLevel2();
			MKY.Settings.SettingsItem settingsEqual = new SettingsTreeTestTypeAndData.TestTreeChildLevel2();
			MKY.Settings.SettingsItem settingsNotEqual = new SettingsTreeTestTypeAndData.TestTreeChildLevel2(SettingsTreeTestTypeAndData.SimpleValueNotEqual);

			VerifyBase(settingsToCompareAgainst, settingsEqual, settingsNotEqual, -1);

			VerifyLevel2(settingsToCompareAgainst, settingsEqual, settingsNotEqual,
			             (SettingsTreeTestTypeAndData.TestTreeChildLevel2)settingsToCompareAgainst, (SettingsTreeTestTypeAndData.TestTreeChildLevel2)settingsEqual, (SettingsTreeTestTypeAndData.TestTreeChildLevel2)settingsNotEqual);
		}

		/// <summary></summary>
		[Test]
		public virtual void TestSettingsTreeLevel1()
		{
			MKY.Settings.SettingsItem settingsToCompareAgainst = new SettingsTreeTestTypeAndData.TestTreeChildLevel1();
			MKY.Settings.SettingsItem settingsEqual = new SettingsTreeTestTypeAndData.TestTreeChildLevel1();
			MKY.Settings.SettingsItem settingsNotEqual = new SettingsTreeTestTypeAndData.TestTreeChildLevel1(SettingsTreeTestTypeAndData.SimpleValueNotEqual);

			VerifyBase(settingsToCompareAgainst, settingsEqual, settingsNotEqual, -1);

			VerifyLevel1(settingsToCompareAgainst, settingsEqual, settingsNotEqual,
			             (SettingsTreeTestTypeAndData.TestTreeChildLevel1)settingsToCompareAgainst, (SettingsTreeTestTypeAndData.TestTreeChildLevel1)settingsEqual, (SettingsTreeTestTypeAndData.TestTreeChildLevel1)settingsNotEqual);
		}

		/// <summary></summary>
		[Test]
		public virtual void TestSettingsTreeRoot()
		{
			MKY.Settings.SettingsItem settingsToCompareAgainst = new SettingsTreeTestTypeAndData.TestTreeRoot();
			MKY.Settings.SettingsItem settingsEqual = new SettingsTreeTestTypeAndData.TestTreeRoot();
			MKY.Settings.SettingsItem settingsNotEqual = new SettingsTreeTestTypeAndData.TestTreeRoot(SettingsTreeTestTypeAndData.SimpleValueNotEqual);

			VerifyBase(settingsToCompareAgainst, settingsEqual, settingsNotEqual, -1);

			VerifyRoot(settingsToCompareAgainst, settingsEqual, settingsNotEqual,
			           (SettingsTreeTestTypeAndData.TestTreeRoot)settingsToCompareAgainst, (SettingsTreeTestTypeAndData.TestTreeRoot)settingsEqual, (SettingsTreeTestTypeAndData.TestTreeRoot)settingsNotEqual);
		}

		private static void VerifyRoot(MKY.Settings.SettingsItem settingsToCompareAgainst, MKY.Settings.SettingsItem settingsEqual, MKY.Settings.SettingsItem settingsNotEqual,
		                               SettingsTreeTestTypeAndData.TestTreeRoot castedToCompareAgainst, SettingsTreeTestTypeAndData.TestTreeRoot castedEqual, SettingsTreeTestTypeAndData.TestTreeRoot castedNotEqual)
		{
			VerifySimpleValue(castedToCompareAgainst.SimpleValue, castedEqual.SimpleValue, castedNotEqual.SimpleValue);

			Assert.That(settingsEqual,        Is.EqualTo(settingsToCompareAgainst));
			Assert.That(settingsNotEqual, Is.Not.EqualTo(settingsToCompareAgainst));

			VerifyLevel1(settingsToCompareAgainst, settingsEqual, settingsNotEqual,
			             castedToCompareAgainst.ChildLevel1, castedEqual.ChildLevel1, castedNotEqual.ChildLevel1);

			// Modify simple value and verify base again.

			castedEqual.SimpleValue    = SettingsTreeTestTypeAndData.SimpleValueNotEqual;
			castedNotEqual.SimpleValue = SettingsTreeTestTypeAndData.SimpleValueEqual;

			VerifyBase(settingsToCompareAgainst, null, settingsEqual, 0);
			VerifyBase(settingsToCompareAgainst, null, settingsNotEqual, 0);

			// Undo modifications and verify base again.

			castedEqual.SimpleValue    = SettingsTreeTestTypeAndData.SimpleValueEqual;
			castedNotEqual.SimpleValue = SettingsTreeTestTypeAndData.SimpleValueNotEqual;

			VerifyBase(settingsToCompareAgainst, settingsEqual, settingsNotEqual, 0);
		}

		private static void VerifyLevel1(MKY.Settings.SettingsItem settingsToCompareAgainst, MKY.Settings.SettingsItem settingsEqual, MKY.Settings.SettingsItem settingsNotEqual,
		                                 SettingsTreeTestTypeAndData.TestTreeChildLevel1 castedToCompareAgainst, SettingsTreeTestTypeAndData.TestTreeChildLevel1 castedEqual, SettingsTreeTestTypeAndData.TestTreeChildLevel1 castedNotEqual)
		{
			VerifySimpleValue(castedToCompareAgainst.SimpleValue, castedEqual.SimpleValue, castedNotEqual.SimpleValue);

			Assert.That(settingsEqual,        Is.EqualTo(settingsToCompareAgainst));
			Assert.That(settingsNotEqual, Is.Not.EqualTo(settingsToCompareAgainst));

			VerifyLevel2(settingsToCompareAgainst, settingsEqual, settingsNotEqual,
			             castedToCompareAgainst.ChildLevel2A, castedEqual.ChildLevel2A, castedNotEqual.ChildLevel2A);
			VerifyLevel2(settingsToCompareAgainst, settingsEqual, settingsNotEqual,
			             castedToCompareAgainst.ChildLevel2B, castedEqual.ChildLevel2B, castedNotEqual.ChildLevel2B);

			// Modify simple value and verify base again.

			castedEqual.SimpleValue    = SettingsTreeTestTypeAndData.SimpleValueNotEqual;
			castedNotEqual.SimpleValue = SettingsTreeTestTypeAndData.SimpleValueEqual;

			VerifyBase(settingsToCompareAgainst, null, settingsEqual, 1);
			VerifyBase(settingsToCompareAgainst, null, settingsNotEqual, 1);

			// Undo modifications and verify base again.

			castedEqual.SimpleValue    = SettingsTreeTestTypeAndData.SimpleValueEqual;
			castedNotEqual.SimpleValue = SettingsTreeTestTypeAndData.SimpleValueNotEqual;

			VerifyBase(settingsToCompareAgainst, settingsEqual, settingsNotEqual, 1);

			// Replace child nodes and verify base again.
			{
				SettingsTreeTestTypeAndData.TestTreeChildLevel2 replacementEqual    = new SettingsTreeTestTypeAndData.TestTreeChildLevel2(SettingsTreeTestTypeAndData.SimpleValueEqual);
				SettingsTreeTestTypeAndData.TestTreeChildLevel2 replacementNotEqual = new SettingsTreeTestTypeAndData.TestTreeChildLevel2(SettingsTreeTestTypeAndData.SimpleValueNotEqual);

				castedEqual.ChildLevel2B    = replacementNotEqual;
				castedNotEqual.ChildLevel2B = replacementEqual;

				VerifyBase(settingsToCompareAgainst, null, settingsEqual, 1);
				VerifyBase(settingsToCompareAgainst, null, settingsNotEqual, 1);
			}

			// Undo modifications and verify base again.
			{
				SettingsTreeTestTypeAndData.TestTreeChildLevel2 replacementEqual    = new SettingsTreeTestTypeAndData.TestTreeChildLevel2(SettingsTreeTestTypeAndData.SimpleValueEqual);
				SettingsTreeTestTypeAndData.TestTreeChildLevel2 replacementNotEqual = new SettingsTreeTestTypeAndData.TestTreeChildLevel2(SettingsTreeTestTypeAndData.SimpleValueNotEqual);

				castedEqual.ChildLevel2B    = replacementEqual;
				castedNotEqual.ChildLevel2B = replacementNotEqual;

				VerifyBase(settingsToCompareAgainst, settingsEqual, settingsNotEqual, 1);
			}
		}

		private static void VerifyLevel2(MKY.Settings.SettingsItem settingsToCompareAgainst, MKY.Settings.SettingsItem settingsEqual, MKY.Settings.SettingsItem settingsNotEqual,
		                                 SettingsTreeTestTypeAndData.TestTreeChildLevel2 castedToCompareAgainst, SettingsTreeTestTypeAndData.TestTreeChildLevel2 castedEqual, SettingsTreeTestTypeAndData.TestTreeChildLevel2 castedNotEqual)
		{
			VerifySimpleValue(castedToCompareAgainst.SimpleValue, castedEqual.SimpleValue, castedNotEqual.SimpleValue);

			Assert.That(settingsEqual,        Is.EqualTo(settingsToCompareAgainst));
			Assert.That(settingsNotEqual, Is.Not.EqualTo(settingsToCompareAgainst));

			// Modify simple value and verify base again.

			castedEqual.SimpleValue    = SettingsTreeTestTypeAndData.SimpleValueNotEqual;
			castedNotEqual.SimpleValue = SettingsTreeTestTypeAndData.SimpleValueEqual;

			if (settingsToCompareAgainst.GetType() == castedToCompareAgainst.GetType())
			{
				VerifyBase(settingsToCompareAgainst, settingsNotEqual, settingsEqual, 2); // Equal and not-equal are swapped.
			}
			else
			{
				VerifyBase(settingsToCompareAgainst, null, settingsEqual, 2);    // Level 2 simple value has been swapped,
				VerifyBase(settingsToCompareAgainst, null, settingsNotEqual, 2); //   but level 1 / root values have not.
			}

			// Undo modifications and verify base again.

			castedEqual.SimpleValue    = SettingsTreeTestTypeAndData.SimpleValueEqual;
			castedNotEqual.SimpleValue = SettingsTreeTestTypeAndData.SimpleValueNotEqual;

			VerifyBase(settingsToCompareAgainst, settingsEqual, settingsNotEqual, 2);
		}

		private static void VerifySimpleValue(int castedToCompareAgainst, int castedEqual, int castedNotEqual)
		{
			Assert.That(castedEqual,        Is.EqualTo(castedToCompareAgainst));
			Assert.That(castedNotEqual, Is.Not.EqualTo(castedToCompareAgainst));
		}

		private static void VerifyBase(MKY.Settings.SettingsItem settingsToCompareAgainst, MKY.Settings.SettingsItem settingsEqual, MKY.Settings.SettingsItem settingsNotEqual, int level)
		{
			string messagePostfix = "";
			if (level >= 0)
				messagePostfix = " (after modifying level " + level + ")";

			if (settingsEqual != null)
				Assert.That(settingsEqual, Is.EqualTo(settingsToCompareAgainst), "Comparing base for equality failed" + messagePostfix);

			if (settingsNotEqual != null)
				Assert.That(settingsNotEqual, Is.Not.EqualTo(settingsToCompareAgainst), "Comparing base for non-equality failed" + messagePostfix);
		}

		#endregion

		#region Tests > SettingsEvolution
		//------------------------------------------------------------------------------------------
		// Tests > SettingsEvolution
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Performance is not an issue here, readability is...")]
		[Test, TestCaseSource(typeof(SettingsEvolutionTestTypeAndData), "TestCases")]
		public virtual void TestSettingsEvolution(int testCase, string fileName, Type typeToSerialize, Type typeToDeserialize)
		{
			string filePath = Temp.MakeTempFilePath(GetType(), fileName, ".xml");

			object objToSerialize = typeToSerialize.GetConstructor(Type.EmptyTypes).Invoke(null);
			XmlSerializerTest.TestSerializeToFile(typeToSerialize, objToSerialize, filePath);

			object objFromDeserialize = XmlSerializerTest.TestDeserializeFromFile(typeToDeserialize, filePath);

			object objToTestAgainst = typeToDeserialize.GetConstructor(Type.EmptyTypes).Invoke(null);

			SettingsEvolutionTestTypeAndData.TestClassV1 v1a = null;
			SettingsEvolutionTestTypeAndData.TestClassV1 v1b = null;
			SettingsEvolutionTestTypeAndData.TestClassV2 v2a = null;
			SettingsEvolutionTestTypeAndData.TestClassV2 v2b = null;
			SettingsEvolutionTestTypeAndData.TestClassV3 v3a = null;
			SettingsEvolutionTestTypeAndData.TestClassV3 v3b = null;

			switch (testCase)
			{
				case SettingsEvolutionTestTypeAndData.TestCaseId._11:
				{
					v1a = (SettingsEvolutionTestTypeAndData.TestClassV1)objToSerialize;
					v1b = (SettingsEvolutionTestTypeAndData.TestClassV1)objFromDeserialize;
					Assert.That(v1b.Data1, Is.EqualTo(v1a.Data1)); // Data 1 must be the one from V1.
					Assert.That(v1b.Data2, Is.EqualTo(v1a.Data2)); // Data 2 must be the one from V1.
					break;
				}
				case SettingsEvolutionTestTypeAndData.TestCaseId._12:
				{
					v1a = (SettingsEvolutionTestTypeAndData.TestClassV1)objToSerialize;
					v2a = (SettingsEvolutionTestTypeAndData.TestClassV2)objFromDeserialize;
					v2b = (SettingsEvolutionTestTypeAndData.TestClassV2)objToTestAgainst;
					Assert.That(v1a.Data1, Is.EqualTo(v2a.Data1)); // Data 1 must be the one from V1.
					Assert.That(v1a.Data2, Is.EqualTo(v2a.Data2)); // Data 2 must be the one from V1.
					Assert.That(v2b.Data3, Is.EqualTo(v2a.Data3)); // Data 3 can only be the one from V2.
					break;
				}
				case SettingsEvolutionTestTypeAndData.TestCaseId._13:
				{
					v1a = (SettingsEvolutionTestTypeAndData.TestClassV1)objToSerialize;
					v3a = (SettingsEvolutionTestTypeAndData.TestClassV3)objFromDeserialize;
					v3b = (SettingsEvolutionTestTypeAndData.TestClassV3)objToTestAgainst;
					Assert.That(v1a.Data1, Is.EqualTo(v3a.Data1)); // Data 1 must be the one from V1.
					Assert.That(v3b.Data3, Is.EqualTo(v3a.Data3)); // Data 3 can only be the one from V3.
					break;
				}
				case SettingsEvolutionTestTypeAndData.TestCaseId._21:
				{
					v2a = (SettingsEvolutionTestTypeAndData.TestClassV2)objToSerialize;
					v1a = (SettingsEvolutionTestTypeAndData.TestClassV1)objFromDeserialize;
					Assert.That(v2a.Data1, Is.EqualTo(v1a.Data1)); // Data 1 must be the one from V2.
					Assert.That(v2a.Data2, Is.EqualTo(v1a.Data2)); // Data 2 must be the one from V2.
					break;
				}
				case SettingsEvolutionTestTypeAndData.TestCaseId._22:
				{
					v2a = (SettingsEvolutionTestTypeAndData.TestClassV2)objToSerialize;
					v2b = (SettingsEvolutionTestTypeAndData.TestClassV2)objFromDeserialize;
					Assert.That(v2b.Data1, Is.EqualTo(v2a.Data1)); // Data 1 must be the one from V2.
					Assert.That(v2b.Data2, Is.EqualTo(v2a.Data2)); // Data 2 must be the one from V2.
					Assert.That(v2b.Data3, Is.EqualTo(v2a.Data3)); // Data 3 must be the one from V2.
					break;
				}
				case SettingsEvolutionTestTypeAndData.TestCaseId._23:
				{
					v2a = (SettingsEvolutionTestTypeAndData.TestClassV2)objToSerialize;
					v3a = (SettingsEvolutionTestTypeAndData.TestClassV3)objFromDeserialize;
					Assert.That(v2a.Data1, Is.EqualTo(v3a.Data1)); // Data 1 must be the one from V2.
					Assert.That(v2a.Data3, Is.EqualTo(v3a.Data3)); // Data 3 must be the one from V2.
					break;
				}
				case SettingsEvolutionTestTypeAndData.TestCaseId._31:
				{
					v3a = (SettingsEvolutionTestTypeAndData.TestClassV3)objToSerialize;
					v1a = (SettingsEvolutionTestTypeAndData.TestClassV1)objFromDeserialize;
					v1b = (SettingsEvolutionTestTypeAndData.TestClassV1)objToTestAgainst;
					Assert.That(v3a.Data1, Is.EqualTo(v1a.Data1)); // Data 1 must be the one from V3.
					Assert.That(v1b.Data2, Is.EqualTo(v1a.Data2)); // Data 2 can only be the one from V1.
					break;
				}
				case SettingsEvolutionTestTypeAndData.TestCaseId._32:
				{
					v3a = (SettingsEvolutionTestTypeAndData.TestClassV3)objToSerialize;
					v2a = (SettingsEvolutionTestTypeAndData.TestClassV2)objFromDeserialize;
					v2b = (SettingsEvolutionTestTypeAndData.TestClassV2)objToTestAgainst;
					Assert.That(v3a.Data1, Is.EqualTo(v2a.Data1)); // Data 1 must be the one from V3.
					Assert.That(v2b.Data2, Is.EqualTo(v2a.Data2)); // Data 2 can only be the one from V2.
					Assert.That(v3a.Data3, Is.EqualTo(v2a.Data3)); // Data 3 must be the one from V3.
					break;
				}
				case SettingsEvolutionTestTypeAndData.TestCaseId._33:
				{
					v3a = (SettingsEvolutionTestTypeAndData.TestClassV3)objToSerialize;
					v3b = (SettingsEvolutionTestTypeAndData.TestClassV3)objFromDeserialize;
					Assert.That(v3b.Data1, Is.EqualTo(v3a.Data1)); // Data 1 must be the one from V3.
					Assert.That(v3b.Data3, Is.EqualTo(v3a.Data3)); // Data 3 must be the one from V3.
					break;
				}
				default:
				{
					Assert.Fail("Test case " + testCase + " not implemented!");
					break;
				}
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
