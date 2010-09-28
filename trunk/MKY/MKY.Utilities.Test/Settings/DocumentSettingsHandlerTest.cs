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
// Copyright � 2003-2004 HSR Hochschule f�r Technik Rapperswil.
// Copyright � 2003-2010 Matthias Kl�y.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml.Serialization;

using NUnit.Framework;

using MKY.Utilities.Diagnostics;

namespace MKY.Utilities.Test.Settings
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
		public class TestTreeChildLevel2 : Utilities.Settings.Settings, IEquatable<TestTreeChildLevel2>
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
				if (obj == null)
					return (false);

				TestTreeChildLevel2 casted = obj as TestTreeChildLevel2;
				if (casted == null)
					return (false);

				return (Equals(casted));
			}

			/// <summary>
			/// Determines whether this instance and the specified object have value equality.
			/// </summary>
			public bool Equals(TestTreeChildLevel2 other)
			{
				// Ensure that object.operator==() is called.
				if ((object)other == null)
					return (false);

				return
				(
					base.Equals((Utilities.Settings.Settings)other) && // Compare all settings nodes.
					(this.SimpleValue == other.SimpleValue)
				);
			}

			/// <summary></summary>
			public override int GetHashCode()
			{
				return (base.GetHashCode());
			}

			#endregion

			#region Comparison Operators

			/// <summary>
			/// Determines whether the two specified objects have reference or value equality.
			/// </summary>
			public static bool operator ==(TestTreeChildLevel2 lhs, TestTreeChildLevel2 rhs)
			{
				if (ReferenceEquals(lhs, rhs))
					return (true);

				if ((object)lhs != null)
					return (lhs.Equals(rhs));

				return (false);
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
		[Serializable]
		public class TestTreeChildLevel1 : Utilities.Settings.Settings, IEquatable<TestTreeChildLevel1>
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
					if (this.childLevel2A == null)
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
					if (this.childLevel2B == null)
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
				if (obj == null)
					return (false);

				TestTreeChildLevel1 casted = obj as TestTreeChildLevel1;
				if (casted == null)
					return (false);

				return (Equals(casted));
			}

			/// <summary>
			/// Determines whether this instance and the specified object have value equality.
			/// </summary>
			public bool Equals(TestTreeChildLevel1 other)
			{
				// Ensure that object.operator==() is called.
				if ((object)other == null)
					return (false);

				return
				(
					base.Equals((Utilities.Settings.Settings)other) && // Compare all settings nodes.
					(this.SimpleValue == other.SimpleValue)
				);
			}

			/// <summary></summary>
			public override int GetHashCode()
			{
				return (base.GetHashCode());
			}

			#endregion

			#region Comparison Operators

			/// <summary>
			/// Determines whether the two specified objects have reference or value equality.
			/// </summary>
			public static bool operator ==(TestTreeChildLevel1 lhs, TestTreeChildLevel1 rhs)
			{
				if (ReferenceEquals(lhs, rhs))
					return (true);

				if ((object)lhs != null)
					return (lhs.Equals(rhs));

				return (false);
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
		[Serializable]
		[XmlRoot("TestTreeRoot")]
		public class TestTreeRoot : Utilities.Settings.Settings, IEquatable<TestTreeRoot>
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
					if (this.childLevel1 == null)
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
				if (obj == null)
					return (false);

				TestTreeRoot casted = obj as TestTreeRoot;
				if (casted == null)
					return (false);

				return (Equals(casted));
			}

			/// <summary>
			/// Determines whether this instance and the specified object have value equality.
			/// </summary>
			public bool Equals(TestTreeRoot other)
			{
				// Ensure that object.operator==() is called.
				if ((object)other == null)
					return (false);

				return
				(
					base.Equals((Utilities.Settings.Settings)other) && // Compare all settings nodes.
					(this.SimpleValue == other.SimpleValue)
				);
			}

			/// <summary></summary>
			public override int GetHashCode()
			{
				return (base.GetHashCode());
			}

			#endregion

			#region Comparison Operators

			/// <summary>
			/// Determines whether the two specified objects have reference or value equality.
			/// </summary>
			public static bool operator ==(TestTreeRoot lhs, TestTreeRoot rhs)
			{
				if (ReferenceEquals(lhs, rhs))
					return (true);

				if ((object)lhs != null)
					return (lhs.Equals(rhs));

				return (false);
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

		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			get
			{
				yield return (new TestCaseData(TestCase.Level2, new TestTreeChildLevel2(), new TestTreeChildLevel2(), new TestTreeChildLevel2(SimpleValueNotEqual)).SetName("Level2"));
				yield return (new TestCaseData(TestCase.Level1, new TestTreeChildLevel1(), new TestTreeChildLevel1(), new TestTreeChildLevel1(SimpleValueNotEqual)).SetName("Level1"));
				yield return (new TestCaseData(TestCase.Root,   new TestTreeRoot(),        new TestTreeRoot(),        new TestTreeRoot(SimpleValueNotEqual))       .SetName("Root"));
			}
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
	public class DocumentSettingsHandlerTest
	{
		#region Tear Down
		//==========================================================================================
		// Tear Down
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TearDown", Justification = "Naming according to NUnit.")]
		[TearDown]
		public virtual void TearDown()
		{
			XPath.CleanTempPath(this);
		}

		#endregion

		#region Tests
		//==========================================================================================
		// Test
		//==========================================================================================

		#region Tests > SettingsEvolution
		//------------------------------------------------------------------------------------------
		// Tests > SettingsEvolution
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, TestCaseSource(typeof(SettingsEvolutionTestData), "TestCases")]
		public virtual void TestSettingsEvolution(int testCase, string fileName, Type typeToSerialize, Type typeToDeserialize)
		{
			string filePath = XPath.MakeTempFilePath(this, fileName, ".xml");

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

		#region Tests > SettingsTree
		//------------------------------------------------------------------------------------------
		// Tests > SettingsTree
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, TestCaseSource(typeof(SettingsTreeTestData), "TestCases")]
		public virtual void TestSettingsTree(SettingsTreeTestData.TestCase testCase, Utilities.Settings.Settings settingsToCompareAgainst, Utilities.Settings.Settings settingsEqual, Utilities.Settings.Settings settingsNotEqual)
		{
			VerifyBase(settingsToCompareAgainst, settingsEqual, settingsNotEqual, -1);

			switch (testCase)
			{
				case SettingsTreeTestData.TestCase.Level2:
					VerifyLevel2(settingsToCompareAgainst, settingsEqual, settingsNotEqual,
					             (SettingsTreeTestData.TestTreeChildLevel2)settingsToCompareAgainst, (SettingsTreeTestData.TestTreeChildLevel2)settingsEqual, (SettingsTreeTestData.TestTreeChildLevel2)settingsNotEqual);
					break;

				case SettingsTreeTestData.TestCase.Level1:
					VerifyLevel1(settingsToCompareAgainst, settingsEqual, settingsNotEqual,
					             (SettingsTreeTestData.TestTreeChildLevel1)settingsToCompareAgainst, (SettingsTreeTestData.TestTreeChildLevel1)settingsEqual, (SettingsTreeTestData.TestTreeChildLevel1)settingsNotEqual);
					break;

				case SettingsTreeTestData.TestCase.Root:
				default:
					VerifyRoot(settingsToCompareAgainst, settingsEqual, settingsNotEqual,
					           (SettingsTreeTestData.TestTreeRoot)settingsToCompareAgainst, (SettingsTreeTestData.TestTreeRoot)settingsEqual, (SettingsTreeTestData.TestTreeRoot)settingsNotEqual);
					break;
			}
		}

		private void VerifyRoot(Utilities.Settings.Settings settingsToCompareAgainst, Utilities.Settings.Settings settingsEqual, Utilities.Settings.Settings settingsNotEqual,
		                        SettingsTreeTestData.TestTreeRoot castedToCompareAgainst, SettingsTreeTestData.TestTreeRoot castedEqual, SettingsTreeTestData.TestTreeRoot castedNotEqual)
		{
			VerifySimpleValue(castedToCompareAgainst.SimpleValue, castedEqual.SimpleValue, castedNotEqual.SimpleValue);

			Assert.AreEqual(settingsToCompareAgainst, settingsEqual);
			Assert.AreNotEqual(settingsToCompareAgainst, settingsNotEqual);

			VerifyLevel1(settingsToCompareAgainst, settingsEqual, settingsNotEqual,
			             castedToCompareAgainst.ChildLevel1, castedEqual.ChildLevel1, castedNotEqual.ChildLevel1);

			// Modify simple value and verify base again.

			castedEqual.SimpleValue = SettingsTreeTestData.SimpleValueNotEqual;
			castedNotEqual.SimpleValue = SettingsTreeTestData.SimpleValueEqual;

			VerifyBase(settingsToCompareAgainst, settingsEqual, settingsNotEqual, 0);
		}

		private void VerifyLevel1(Utilities.Settings.Settings settingsToCompareAgainst, Utilities.Settings.Settings settingsEqual, Utilities.Settings.Settings settingsNotEqual,
		                          SettingsTreeTestData.TestTreeChildLevel1 castedToCompareAgainst, SettingsTreeTestData.TestTreeChildLevel1 castedEqual, SettingsTreeTestData.TestTreeChildLevel1 castedNotEqual)
		{
			VerifySimpleValue(castedToCompareAgainst.SimpleValue, castedEqual.SimpleValue, castedNotEqual.SimpleValue);

			Assert.AreEqual(settingsToCompareAgainst, settingsEqual);
			Assert.AreNotEqual(settingsToCompareAgainst, settingsNotEqual);

			VerifyLevel2(settingsToCompareAgainst, settingsEqual, settingsNotEqual,
			             castedToCompareAgainst.ChildLevel2A, castedEqual.ChildLevel2A, castedNotEqual.ChildLevel2A);
			VerifyLevel2(settingsToCompareAgainst, settingsEqual, settingsNotEqual,
			             castedToCompareAgainst.ChildLevel2B, castedEqual.ChildLevel2B, castedNotEqual.ChildLevel2B);

			// Modify simple value and verify base again.

			castedEqual.SimpleValue = SettingsTreeTestData.SimpleValueNotEqual;
			castedNotEqual.SimpleValue = SettingsTreeTestData.SimpleValueEqual;

			VerifyBase(settingsToCompareAgainst, settingsEqual, settingsNotEqual, 1);
		}

		private void VerifyLevel2(Utilities.Settings.Settings settingsToCompareAgainst, Utilities.Settings.Settings settingsEqual, Utilities.Settings.Settings settingsNotEqual,
		                          SettingsTreeTestData.TestTreeChildLevel2 castedToCompareAgainst, SettingsTreeTestData.TestTreeChildLevel2 castedEqual, SettingsTreeTestData.TestTreeChildLevel2 castedNotEqual)
		{
			VerifySimpleValue(castedToCompareAgainst.SimpleValue, castedEqual.SimpleValue, castedNotEqual.SimpleValue);

			Assert.AreEqual(settingsToCompareAgainst, settingsEqual);
			Assert.AreNotEqual(settingsToCompareAgainst, settingsNotEqual);

			// Modify simple value and verify base again.

			castedEqual.SimpleValue = SettingsTreeTestData.SimpleValueNotEqual;
			castedNotEqual.SimpleValue = SettingsTreeTestData.SimpleValueEqual;

			VerifyBase(settingsToCompareAgainst, settingsEqual, settingsNotEqual, 2);
		}

		private void VerifySimpleValue(int castedToCompareAgainst, int castedEqual, int castedNotEqual)
		{
			Assert.AreEqual(castedToCompareAgainst, castedEqual);
			Assert.AreNotEqual(castedToCompareAgainst, castedNotEqual);
		}

		private void VerifyBase(Utilities.Settings.Settings settingsToCompareAgainst, Utilities.Settings.Settings settingsEqual, Utilities.Settings.Settings settingsNotEqual, int level)
		{
			string messagePostfix = "";
			if (level >= 0)
				messagePostfix = "(after modifying level " + level + ")";

			Assert.AreEqual   (settingsToCompareAgainst, settingsEqual,    "Comparing base for equality failed"     + messagePostfix);
			Assert.AreNotEqual(settingsToCompareAgainst, settingsNotEqual, "Comparing base for non-equality failed" + messagePostfix);
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
				using (StreamWriter sw = new StreamWriter(filePath))
				{
					XmlSerializer serializer = new XmlSerializer(type);
					serializer.Serialize(sw, obj);
				}
			}
			catch (Exception ex)
			{
				XConsole.WriteException(typeof(DocumentSettingsHandlerTest), ex);

				// Attention: The following call throws an exception, code below it won't be executed
				Assert.Fail("XML serialize error: " + ex.Message);
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		private static void DeserializeTestObject(Type type, out object obj, string filePath)
		{
			try
			{
				using (StreamReader sr = new StreamReader(filePath))
				{
					XmlSerializer serializer = new XmlSerializer(type);
					obj = serializer.Deserialize(sr);
				}
			}
			catch (Exception ex)
			{
				obj = null;
				XConsole.WriteException(typeof(DocumentSettingsHandlerTest), ex);

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
