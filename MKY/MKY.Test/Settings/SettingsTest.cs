//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Xml.Serialization;

using NUnit.Framework;

using MKY.Diagnostics;
using MKY.IO;

namespace MKY.Test.Settings
{
	/// <summary></summary>
	public class SettingsTreeTestData
	{
		#region IDs
		//==========================================================================================
		// IDs
		//==========================================================================================

		/// <remarks>Haven't care for a better implementation yet...</remarks>
		public enum TestCase
		{
			/// <summary></summary>
			Root,

			/// <summary></summary>
			Level1,

			/// <summary></summary>
			Level2,
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
		[Serializable]
		public class TestTreeChildLevel2 : MKY.Settings.Settings
		{
			/// <summary></summary>
			public int SimpleValue = SimpleValueEqual;

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

			/// <summary></summary>
			public override string ToString()
			{
				return (Environment.NewLine + "        2:SimpleValue = " + this.SimpleValue.ToString());
			}

			/// <summary>
			/// Determines whether this instance and the specified object have value equality.
			/// </summary>
			public override bool Equals(object obj)
			{
				if (ReferenceEquals(obj, null))
					return (false);

				if (GetType() != obj.GetType())
					return (false);

				TestTreeChildLevel2 other = (TestTreeChildLevel2)obj;
				return
				(
					base.Equals(other) && // Compare all settings nodes.
					(this.SimpleValue == other.SimpleValue)
				);
			}

			/// <summary></summary>
			public override int GetHashCode()
			{
				return
				(
					base.GetHashCode() ^
					this.SimpleValue.GetHashCode()
				);
			}

			#endregion

			#region Comparison Operators

			// Use of base reference type implementation of operators ==/!=.
			// See MKY.Test.EqualityTest for details.

			#endregion
		}

		/// <summary></summary>
		[Serializable]
		public class TestTreeChildLevel1 : MKY.Settings.Settings
		{
			/// <summary></summary>
			public int SimpleValue = SimpleValueEqual;

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
					if (value == null)
					{
						this.childLevel2A = value;
						DetachNode(this.childLevel2A);
					}
					else if (this.childLevel2A == null)
					{
						this.childLevel2A = value;
						AttachNode(this.childLevel2A);
					}
					else if (value != this.childLevel2A)
					{
						TestTreeChildLevel2 old = this.childLevel2A;
						this.childLevel2A = value;
						ReplaceNode(old, this.childLevel2A);
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
					if (value == null)
					{
						this.childLevel2B = value;
						DetachNode(this.childLevel2B);
					}
					else if (this.childLevel2B == null)
					{
						this.childLevel2B = value;
						AttachNode(this.childLevel2B);
					}
					else if (value != this.childLevel2B)
					{
						TestTreeChildLevel2 old = this.childLevel2B;
						this.childLevel2B = value;
						ReplaceNode(old, this.childLevel2B);
					}
				}
			}

			#endregion

			#region Object Members

			/// <summary></summary>
			public override string ToString()
			{
				return
					(
					Environment.NewLine + "      1:SimpleValue = " + SimpleValue.ToString() +
					Environment.NewLine + "      1:ChildLevel2A = " + ChildLevel2A.ToString() +
					Environment.NewLine + "      1:ChildLevel2B = " + ChildLevel2B.ToString()
					);
			}

			/// <summary>
			/// Determines whether this instance and the specified object have value equality.
			/// </summary>
			public override bool Equals(object obj)
			{
				if (ReferenceEquals(obj, null))
					return (false);

				if (GetType() != obj.GetType())
					return (false);

				TestTreeChildLevel1 other = (TestTreeChildLevel1)obj;
				return
				(
					base.Equals(other) && // Compare all settings nodes.
					(this.SimpleValue == other.SimpleValue)
				);
			}

			/// <summary></summary>
			public override int GetHashCode()
			{
				return
				(
					base.GetHashCode() ^
					this.SimpleValue.GetHashCode()
				);
			}

			#endregion

			#region Comparison Operators

			// Use of base reference type implementation of operators ==/!=.
			// See MKY.Test.EqualityTest for details.

			#endregion
		}

		/// <summary></summary>
		[Serializable]
		[XmlRoot("TestTreeRoot")]
		public class TestTreeRoot : MKY.Settings.Settings
		{
			/// <summary></summary>
			public int SimpleValue = SimpleValueEqual;

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
					if (value == null)
					{
						this.childLevel1 = value;
						DetachNode(this.childLevel1);
					}
					else if (this.childLevel1 == null)
					{
						this.childLevel1 = value;
						AttachNode(this.childLevel1);
					}
					else if (value != this.childLevel1)
					{
						TestTreeChildLevel1 old = this.childLevel1;
						this.childLevel1 = value;
						ReplaceNode(old, this.childLevel1);
					}
				}
			}

			#endregion

			#region Object Members

			/// <summary></summary>
			public override string ToString()
			{
				return
					(
					Environment.NewLine + "    0:SimpleValue = " + SimpleValue.ToString() +
					Environment.NewLine + "    0:ChildLevel1 = " + ChildLevel1.ToString()
					);
			}

			/// <summary>
			/// Determines whether this instance and the specified object have value equality.
			/// </summary>
			public override bool Equals(object obj)
			{
				if (ReferenceEquals(obj, null))
					return (false);

				if (GetType() != obj.GetType())
					return (false);

				TestTreeRoot other = (TestTreeRoot)obj;
				return
				(
					base.Equals(other) && // Compare all settings nodes.
					(this.SimpleValue == other.SimpleValue)
				);
			}

			/// <summary></summary>
			public override int GetHashCode()
			{
				return
				(
					base.GetHashCode() ^
					this.SimpleValue.GetHashCode()
				);
			}

			#endregion

			#region Comparison Operators

			/// <summary>
			/// Determines whether the two specified objects have reference or value equality.
			/// </summary>
			public static bool operator ==(TestTreeRoot lhs, TestTreeRoot rhs)
			{
				// Base reference type implementation of operator ==.
				// See MKY.Test.EqualityTest for details.

				if (ReferenceEquals(lhs, rhs)) return (true);
				if (ReferenceEquals(lhs, null)) return (false);
				if (ReferenceEquals(rhs, null)) return (false);

				// Ensure that object.Equals() is called.
				// Thus, ensure that potential <Derived>.Equals() is called.
				object obj = (object)lhs;
				return (obj.Equals(rhs));
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
	public class SettingsEvolutionTestData
	{
		#region Test Case Id
		//==========================================================================================
		// Test Case Id
		//==========================================================================================

		/// <remarks>Haven't care for a better implementation yet...</remarks>
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
		/// serialized/deserialize.
		/// </summary>
		[Serializable]
		[XmlRoot("TestSettings")]
		public class TestClassV1
		{
			/// <summary></summary>
			public int Data1 = TestCaseId._11;
			/// <summary></summary>
			public int Data2 = TestCaseId._12;
		}

		/// <summary>
		/// V2 adds a third element.
		/// </summary>
		[Serializable]
		[XmlRoot("TestSettings")]
		public class TestClassV2
		{
			/// <summary></summary>
			public int Data1 = TestCaseId._21;
			/// <summary></summary>
			public int Data2 = TestCaseId._22;
			/// <summary></summary>
			public int Data3 = TestCaseId._23;
		}

		/// <summary>
		/// V3 removes the second element.
		/// </summary>
		[Serializable]
		[XmlRoot("TestSettings")]
		public class TestClassV3
		{
			/// <summary></summary>
			public int Data1 = TestCaseId._31;
			/// <summary></summary>
			public int Data3 = TestCaseId._33;
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
	[TestFixture]
	public class SettingsTest
	{
		#region Tear Down Fixture
		//==========================================================================================
		// Tear Down Fixture
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TearDown", Justification = "Naming according to NUnit.")]
		[TestFixtureTearDown]
		public virtual void TestFixtureTearDown()
		{
			Temp.CleanTempPath(this.GetType());
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
			MKY.Settings.Settings settingsToCompareAgainst = new SettingsTreeTestData.TestTreeChildLevel2();
			MKY.Settings.Settings settingsEqual = new SettingsTreeTestData.TestTreeChildLevel2();
			MKY.Settings.Settings settingsNotEqual = new SettingsTreeTestData.TestTreeChildLevel2(SettingsTreeTestData.SimpleValueNotEqual);

			VerifyBase(settingsToCompareAgainst, settingsEqual, settingsNotEqual, -1);

			VerifyLevel2(settingsToCompareAgainst, settingsEqual, settingsNotEqual,
			             (SettingsTreeTestData.TestTreeChildLevel2)settingsToCompareAgainst, (SettingsTreeTestData.TestTreeChildLevel2)settingsEqual, (SettingsTreeTestData.TestTreeChildLevel2)settingsNotEqual);
		}

		/// <summary></summary>
		[Test]
		public virtual void TestSettingsTreeLevel1()
		{
			MKY.Settings.Settings settingsToCompareAgainst = new SettingsTreeTestData.TestTreeChildLevel1();
			MKY.Settings.Settings settingsEqual = new SettingsTreeTestData.TestTreeChildLevel1();
			MKY.Settings.Settings settingsNotEqual = new SettingsTreeTestData.TestTreeChildLevel1(SettingsTreeTestData.SimpleValueNotEqual);

			VerifyBase(settingsToCompareAgainst, settingsEqual, settingsNotEqual, -1);

			VerifyLevel1(settingsToCompareAgainst, settingsEqual, settingsNotEqual,
			             (SettingsTreeTestData.TestTreeChildLevel1)settingsToCompareAgainst, (SettingsTreeTestData.TestTreeChildLevel1)settingsEqual, (SettingsTreeTestData.TestTreeChildLevel1)settingsNotEqual);
		}

		/// <summary></summary>
		[Test]
		public virtual void TestSettingsTreeRoot()
		{
			MKY.Settings.Settings settingsToCompareAgainst = new SettingsTreeTestData.TestTreeRoot();
			MKY.Settings.Settings settingsEqual = new SettingsTreeTestData.TestTreeRoot();
			MKY.Settings.Settings settingsNotEqual = new SettingsTreeTestData.TestTreeRoot(SettingsTreeTestData.SimpleValueNotEqual);

			VerifyBase(settingsToCompareAgainst, settingsEqual, settingsNotEqual, -1);

			VerifyRoot(settingsToCompareAgainst, settingsEqual, settingsNotEqual,
			           (SettingsTreeTestData.TestTreeRoot)settingsToCompareAgainst, (SettingsTreeTestData.TestTreeRoot)settingsEqual, (SettingsTreeTestData.TestTreeRoot)settingsNotEqual);
		}

		private void VerifyRoot(MKY.Settings.Settings settingsToCompareAgainst, MKY.Settings.Settings settingsEqual, MKY.Settings.Settings settingsNotEqual,
		                        SettingsTreeTestData.TestTreeRoot castedToCompareAgainst, SettingsTreeTestData.TestTreeRoot castedEqual, SettingsTreeTestData.TestTreeRoot castedNotEqual)
		{
			VerifySimpleValue(castedToCompareAgainst.SimpleValue, castedEqual.SimpleValue, castedNotEqual.SimpleValue);

			Assert.AreEqual   (settingsToCompareAgainst, settingsEqual);
			Assert.AreNotEqual(settingsToCompareAgainst, settingsNotEqual);

			VerifyLevel1(settingsToCompareAgainst, settingsEqual, settingsNotEqual,
			             castedToCompareAgainst.ChildLevel1, castedEqual.ChildLevel1, castedNotEqual.ChildLevel1);

			// Modify simple value and verify base again.

			castedEqual.SimpleValue    = SettingsTreeTestData.SimpleValueNotEqual;
			castedNotEqual.SimpleValue = SettingsTreeTestData.SimpleValueEqual;

			VerifyBase(settingsToCompareAgainst, null, settingsEqual, 0);
			VerifyBase(settingsToCompareAgainst, null, settingsNotEqual, 0);

			// Undo modifications and verify base again.

			castedEqual.SimpleValue    = SettingsTreeTestData.SimpleValueEqual;
			castedNotEqual.SimpleValue = SettingsTreeTestData.SimpleValueNotEqual;

			VerifyBase(settingsToCompareAgainst, settingsEqual, settingsNotEqual, 0);
		}

		private void VerifyLevel1(MKY.Settings.Settings settingsToCompareAgainst, MKY.Settings.Settings settingsEqual, MKY.Settings.Settings settingsNotEqual,
		                          SettingsTreeTestData.TestTreeChildLevel1 castedToCompareAgainst, SettingsTreeTestData.TestTreeChildLevel1 castedEqual, SettingsTreeTestData.TestTreeChildLevel1 castedNotEqual)
		{
			VerifySimpleValue(castedToCompareAgainst.SimpleValue, castedEqual.SimpleValue, castedNotEqual.SimpleValue);

			Assert.AreEqual   (settingsToCompareAgainst, settingsEqual);
			Assert.AreNotEqual(settingsToCompareAgainst, settingsNotEqual);

			VerifyLevel2(settingsToCompareAgainst, settingsEqual, settingsNotEqual,
			             castedToCompareAgainst.ChildLevel2A, castedEqual.ChildLevel2A, castedNotEqual.ChildLevel2A);
			VerifyLevel2(settingsToCompareAgainst, settingsEqual, settingsNotEqual,
			             castedToCompareAgainst.ChildLevel2B, castedEqual.ChildLevel2B, castedNotEqual.ChildLevel2B);

			// Modify simple value and verify base again.

			castedEqual.SimpleValue    = SettingsTreeTestData.SimpleValueNotEqual;
			castedNotEqual.SimpleValue = SettingsTreeTestData.SimpleValueEqual;

			VerifyBase(settingsToCompareAgainst, null, settingsEqual, 1);
			VerifyBase(settingsToCompareAgainst, null, settingsNotEqual, 1);

			// Undo modifications and verify base again.

			castedEqual.SimpleValue    = SettingsTreeTestData.SimpleValueEqual;
			castedNotEqual.SimpleValue = SettingsTreeTestData.SimpleValueNotEqual;

			VerifyBase(settingsToCompareAgainst, settingsEqual, settingsNotEqual, 1);

			// Replace child nodes and verify base again.
			{
				SettingsTreeTestData.TestTreeChildLevel2 replacementEqual    = new SettingsTreeTestData.TestTreeChildLevel2(SettingsTreeTestData.SimpleValueEqual);
				SettingsTreeTestData.TestTreeChildLevel2 replacementNotEqual = new SettingsTreeTestData.TestTreeChildLevel2(SettingsTreeTestData.SimpleValueNotEqual);

				castedEqual.ChildLevel2B    = replacementNotEqual;
				castedNotEqual.ChildLevel2B = replacementEqual;

				VerifyBase(settingsToCompareAgainst, null, settingsEqual, 1);
				VerifyBase(settingsToCompareAgainst, null, settingsNotEqual, 1);
			}

			// Undo modifications and verify base again.
			{
				SettingsTreeTestData.TestTreeChildLevel2 replacementEqual    = new SettingsTreeTestData.TestTreeChildLevel2(SettingsTreeTestData.SimpleValueEqual);
				SettingsTreeTestData.TestTreeChildLevel2 replacementNotEqual = new SettingsTreeTestData.TestTreeChildLevel2(SettingsTreeTestData.SimpleValueNotEqual);

				castedEqual.ChildLevel2B    = replacementEqual;
				castedNotEqual.ChildLevel2B = replacementNotEqual;

				VerifyBase(settingsToCompareAgainst, settingsEqual, settingsNotEqual, 1);
			}
		}

		private void VerifyLevel2(MKY.Settings.Settings settingsToCompareAgainst, MKY.Settings.Settings settingsEqual, MKY.Settings.Settings settingsNotEqual,
		                          SettingsTreeTestData.TestTreeChildLevel2 castedToCompareAgainst, SettingsTreeTestData.TestTreeChildLevel2 castedEqual, SettingsTreeTestData.TestTreeChildLevel2 castedNotEqual)
		{
			VerifySimpleValue(castedToCompareAgainst.SimpleValue, castedEqual.SimpleValue, castedNotEqual.SimpleValue);

			Assert.AreEqual   (settingsToCompareAgainst, settingsEqual);
			Assert.AreNotEqual(settingsToCompareAgainst, settingsNotEqual);

			// Modify simple value and verify base again.

			castedEqual.SimpleValue    = SettingsTreeTestData.SimpleValueNotEqual;
			castedNotEqual.SimpleValue = SettingsTreeTestData.SimpleValueEqual;

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

			castedEqual.SimpleValue    = SettingsTreeTestData.SimpleValueEqual;
			castedNotEqual.SimpleValue = SettingsTreeTestData.SimpleValueNotEqual;

			VerifyBase(settingsToCompareAgainst, settingsEqual, settingsNotEqual, 2);
		}

		private void VerifySimpleValue(int castedToCompareAgainst, int castedEqual, int castedNotEqual)
		{
			Assert.AreEqual   (castedToCompareAgainst, castedEqual);
			Assert.AreNotEqual(castedToCompareAgainst, castedNotEqual);
		}

		private void VerifyBase(MKY.Settings.Settings settingsToCompareAgainst, MKY.Settings.Settings settingsEqual, MKY.Settings.Settings settingsNotEqual, int level)
		{
			string messagePostfix = "";
			if (level >= 0)
				messagePostfix = " (after modifying level " + level + ")";

			if (settingsEqual != null)
				Assert.AreEqual   (settingsToCompareAgainst, settingsEqual,    "Comparing base for equality failed"     + messagePostfix);

			if (settingsNotEqual != null)
				Assert.AreNotEqual(settingsToCompareAgainst, settingsNotEqual, "Comparing base for non-equality failed" + messagePostfix);
		}

		#endregion

		#region Tests > SettingsEvolution
		//------------------------------------------------------------------------------------------
		// Tests > SettingsEvolution
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, TestCaseSource(typeof(SettingsEvolutionTestData), "TestCases")]
		public virtual void TestSettingsEvolution(int testCase, string fileName, Type typeToSerialize, Type typeToDeserialize)
		{
			string filePath = Temp.MakeTempFilePath(this.GetType(), fileName, ".xml");

			object objToSerialize = typeToSerialize.GetConstructor(new System.Type[] { }).Invoke(new object[] { });
			SerializeTestObject(typeToSerialize, objToSerialize, filePath);

			object objToDeserialize;
			DeserializeTestObject(typeToDeserialize, out objToDeserialize, filePath);

			object objToTestAgainst = typeToDeserialize.GetConstructor(new System.Type[] { }).Invoke(new object[] { });

			SettingsEvolutionTestData.TestClassV1 v1a = null;
			SettingsEvolutionTestData.TestClassV1 v1b = null;
			SettingsEvolutionTestData.TestClassV2 v2a = null;
			SettingsEvolutionTestData.TestClassV2 v2b = null;
			SettingsEvolutionTestData.TestClassV3 v3a = null;
			SettingsEvolutionTestData.TestClassV3 v3b = null;

			switch (testCase)
			{
				case SettingsEvolutionTestData.TestCaseId._11:
				{
					v1a = (SettingsEvolutionTestData.TestClassV1)objToSerialize;
					v1b = (SettingsEvolutionTestData.TestClassV1)objToDeserialize;
					Assert.AreEqual(v1a.Data1, v1b.Data1); // Data 1 must be the one from V1.
					Assert.AreEqual(v1a.Data2, v1b.Data2); // Data 2 must be the one from V1.
					break;
				}
				case SettingsEvolutionTestData.TestCaseId._12:
				{
					v1a = (SettingsEvolutionTestData.TestClassV1)objToSerialize;
					v2a = (SettingsEvolutionTestData.TestClassV2)objToDeserialize;
					v2b = (SettingsEvolutionTestData.TestClassV2)objToTestAgainst;
					Assert.AreEqual(v2a.Data1, v1a.Data1); // Data 1 must be the one from V1.
					Assert.AreEqual(v2a.Data2, v1a.Data2); // Data 2 must be the one from V1.
					Assert.AreEqual(v2a.Data3, v2b.Data3); // Data 3 can only be the one from V2.
					break;
				}
				case SettingsEvolutionTestData.TestCaseId._13:
				{
					v1a = (SettingsEvolutionTestData.TestClassV1)objToSerialize;
					v3a = (SettingsEvolutionTestData.TestClassV3)objToDeserialize;
					v3b = (SettingsEvolutionTestData.TestClassV3)objToTestAgainst;
					Assert.AreEqual(v3a.Data1, v1a.Data1); // Data 1 must be the one from V1.
					Assert.AreEqual(v3a.Data3, v3b.Data3); // Data 3 can only be the one from V3.
					break;
				}
				case SettingsEvolutionTestData.TestCaseId._21:
				{
					v2a = (SettingsEvolutionTestData.TestClassV2)objToSerialize;
					v1a = (SettingsEvolutionTestData.TestClassV1)objToDeserialize;
					Assert.AreEqual(v1a.Data1, v2a.Data1); // Data 1 must be the one from V2.
					Assert.AreEqual(v1a.Data2, v2a.Data2); // Data 2 must be the one from V2.
					break;
				}
				case SettingsEvolutionTestData.TestCaseId._22:
				{
					v2a = (SettingsEvolutionTestData.TestClassV2)objToSerialize;
					v2b = (SettingsEvolutionTestData.TestClassV2)objToDeserialize;
					Assert.AreEqual(v2a.Data1, v2b.Data1); // Data 1 must be the one from V2.
					Assert.AreEqual(v2a.Data2, v2b.Data2); // Data 2 must be the one from V2.
					Assert.AreEqual(v2a.Data3, v2b.Data3); // Data 3 must be the one from V2.
					break;
				}
				case SettingsEvolutionTestData.TestCaseId._23:
				{
					v2a = (SettingsEvolutionTestData.TestClassV2)objToSerialize;
					v3a = (SettingsEvolutionTestData.TestClassV3)objToDeserialize;
					Assert.AreEqual(v3a.Data1, v2a.Data1); // Data 1 must be the one from V2.
					Assert.AreEqual(v3a.Data3, v2a.Data3); // Data 3 must be the one from V2.
					break;
				}
				case SettingsEvolutionTestData.TestCaseId._31:
				{
					v3a = (SettingsEvolutionTestData.TestClassV3)objToSerialize;
					v1a = (SettingsEvolutionTestData.TestClassV1)objToDeserialize;
					v1b = (SettingsEvolutionTestData.TestClassV1)objToTestAgainst;
					Assert.AreEqual(v1a.Data1, v3a.Data1); // Data 1 must be the one from V3.
					Assert.AreEqual(v1a.Data2, v1b.Data2); // Data 2 can only be the one from V1.
					break;
				}
				case SettingsEvolutionTestData.TestCaseId._32:
				{
					v3a = (SettingsEvolutionTestData.TestClassV3)objToSerialize;
					v2a = (SettingsEvolutionTestData.TestClassV2)objToDeserialize;
					v2b = (SettingsEvolutionTestData.TestClassV2)objToTestAgainst;
					Assert.AreEqual(v2a.Data1, v3a.Data1); // Data 1 must be the one from V3.
					Assert.AreEqual(v2a.Data2, v2b.Data2); // Data 2 can only be the one from V2.
					Assert.AreEqual(v2a.Data3, v3a.Data3); // Data 3 must be the one from V3.
					break;
				}
				case SettingsEvolutionTestData.TestCaseId._33:
				{
					v3a = (SettingsEvolutionTestData.TestClassV3)objToSerialize;
					v3b = (SettingsEvolutionTestData.TestClassV3)objToDeserialize;
					Assert.AreEqual(v3a.Data1, v3b.Data1); // Data 1 must be the one from V3.
					Assert.AreEqual(v3a.Data3, v3b.Data3); // Data 3 must be the one from V3.
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

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		private static void SerializeTestObject(Type type, object obj, string filePath)
		{
			try
			{
				using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8))
				{
					XmlSerializer serializer = new XmlSerializer(type);
					serializer.Serialize(sw, obj);
				}
			}
			catch (Exception ex)
			{
				TraceEx.WriteException(typeof(SettingsTest), ex);

				// Attention: The following call throws an exception, code below it won't be executed
				Assert.Fail("XML serialize error: " + ex.Message);
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		private static void DeserializeTestObject(Type type, out object obj, string filePath)
		{
			try
			{
				using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8, true))
				{
					XmlSerializer serializer = new XmlSerializer(type);
					obj = serializer.Deserialize(sr);
				}
			}
			catch (Exception ex)
			{
				obj = null;
				TraceEx.WriteException(typeof(SettingsTest), ex);

				// Attention: The following call throws an exception, code below it won't be executed
				Assert.Fail("XML deserialize error: " + ex.Message);
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
