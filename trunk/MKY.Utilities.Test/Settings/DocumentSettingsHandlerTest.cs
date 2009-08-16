//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2009 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;

using NUnit.Framework;

using MKY.Utilities.Diagnostics;
using MKY.Utilities.Settings;

namespace MKY.Utilities.Test.Settings
{
	[TestFixture]
	public class DocumentSettingsHandlerTest
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const int _11 = 11;
		private const int _12 = 12;
		private const int _13 = 13;
		private const int _21 = 21;
		private const int _22 = 22;
		private const int _23 = 23;
		private const int _31 = 31;
		private const int _32 = 32;
		private const int _33 = 33;

		#endregion

		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private struct TestSet
		{
			public int TestCase;
			public string FileName;
			public Type TypeToSerialize;
			public Type TypeToDeserialize;

			public TestSet(int testCase, string fileName, Type typeToSerialize, Type typeToDeserialize)
			{
				TestCase = testCase;
				FileName = fileName;
				TypeToSerialize = typeToSerialize;
				TypeToDeserialize = typeToDeserialize;
			}
		};

		/// <summary>
		/// The following classes serve to test what happens if different versions of a type are
		/// serialized/deserialize.
		/// </summary>
		[Serializable]
		[XmlRoot("TestSettings")]
		public class TestClassV1
		{
			public int Data1 = _11;
			public int Data2 = _12;
		};

		/// <summary>
		/// V2 adds a third element.
		/// </summary>
		[Serializable]
		[XmlRoot("TestSettings")]
		public class TestClassV2
		{
			public int Data1 = _21;
			public int Data2 = _22;
			public int Data3 = _23;
		};

		/// <summary>
		/// V3 removes the second element.
		/// </summary>
		[Serializable]
		[XmlRoot("TestSettings")]
		public class TestClassV3
		{
			public int Data1 = _31;
			public int Data3 = _33;
		};

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private readonly TestSet[] _testSets =
		{
			new TestSet(_11, "TestObjectV1", typeof(TestClassV1), typeof(TestClassV1)),
			new TestSet(_12, "TestObjectV1", typeof(TestClassV1), typeof(TestClassV2)),
			new TestSet(_13, "TestObjectV1", typeof(TestClassV1), typeof(TestClassV3)),
			new TestSet(_21, "TestObjectV2", typeof(TestClassV2), typeof(TestClassV1)),
			new TestSet(_22, "TestObjectV2", typeof(TestClassV2), typeof(TestClassV2)),
			new TestSet(_23, "TestObjectV2", typeof(TestClassV2), typeof(TestClassV3)),
			new TestSet(_31, "TestObjectV3", typeof(TestClassV3), typeof(TestClassV1)),
			new TestSet(_32, "TestObjectV3", typeof(TestClassV3), typeof(TestClassV2)),
			new TestSet(_33, "TestObjectV3", typeof(TestClassV3), typeof(TestClassV3)),
		};

		#endregion

		#region Tear Down
		//==========================================================================================
		// Tear Down
		//==========================================================================================

		[TearDown]
		public void TearDown()
		{
			foreach (string filePath in Directory.GetFiles(MakeTempPath(), MakeTempFileName("*")))
				File.Delete(filePath);
		}

		#endregion

		#region Tests
		//==========================================================================================
		// Test
		//==========================================================================================

		#region Tests > Deserialization
		//------------------------------------------------------------------------------------------
		// Tests > Deserialization
		//------------------------------------------------------------------------------------------

		[Test]
		public void TestDeserialization()
		{
			foreach (TestSet ts in _testSets)
			{
				string filePath = MakeTempFilePath(ts.FileName);

				object objToSerialize = ts.TypeToSerialize.GetConstructor(new System.Type[] { }).Invoke(new object[] { });
				SerializeTestObject(ts.TypeToSerialize, objToSerialize, filePath);

				object objToDeserialize;
				DeserializeTestObject(ts.TypeToDeserialize, out objToDeserialize, filePath);

				VerifyValues(ts, objToSerialize, objToDeserialize);
			}
		}

		/// <remarks>Haven't cared for a better implementation yet...</remarks>
		private void VerifyValues(TestSet ts, object objToSerialize, object objToDeserialize)
		{
			object objToTestAgainst = ts.TypeToDeserialize.GetConstructor(new System.Type[] { }).Invoke(new object[] { });

			TestClassV1 v1a = null;
			TestClassV1 v1b = null;
			TestClassV2 v2a = null;
			TestClassV2 v2b = null;
			TestClassV3 v3a = null;
			TestClassV3 v3b = null;

			switch (ts.TestCase)
			{
				case _11:
				{
					v1a = (TestClassV1)objToSerialize;
					v1b = (TestClassV1)objToDeserialize;
					Assert.AreEqual(v1a.Data1, v1b.Data1); // data 1 must be the one from V1
					Assert.AreEqual(v1a.Data2, v1b.Data2); // data 2 must be the one from V1
					break;
				}
				case _12:
				{
					v1a = (TestClassV1)objToSerialize;
					v2a = (TestClassV2)objToDeserialize;
					v2b = (TestClassV2)objToTestAgainst;
					Assert.AreEqual(v2a.Data1, v1a.Data1); // data 1 must be the one from V1
					Assert.AreEqual(v2a.Data2, v1a.Data2); // data 2 must be the one from V1
					Assert.AreEqual(v2a.Data3, v2b.Data3); // data 3 can only be the one from V2
					break;
				}
				case _13:
				{
					v1a = (TestClassV1)objToSerialize;
					v3a = (TestClassV3)objToDeserialize;
					v3b = (TestClassV3)objToTestAgainst;
					Assert.AreEqual(v3a.Data1, v1a.Data1); // data 1 must be the one from V1
					Assert.AreEqual(v3a.Data3, v3b.Data3); // data 3 can only be the one from V3
					break;
				}
				case _21:
				{
					v2a = (TestClassV2)objToSerialize;
					v1a = (TestClassV1)objToDeserialize;
					Assert.AreEqual(v1a.Data1, v2a.Data1); // data 1 must be the one from V2
					Assert.AreEqual(v1a.Data2, v2a.Data2); // data 2 must be the one from V2
					break;
				}
				case _22:
				{
					v2a = (TestClassV2)objToSerialize;
					v2b = (TestClassV2)objToDeserialize;
					Assert.AreEqual(v2a.Data1, v2b.Data1); // data 1 must be the one from V2
					Assert.AreEqual(v2a.Data2, v2b.Data2); // data 2 must be the one from V2
					Assert.AreEqual(v2a.Data3, v2b.Data3); // data 3 must be the one from V2
					break;
				}
				case _23:
				{
					v2a = (TestClassV2)objToSerialize;
					v3a = (TestClassV3)objToDeserialize;
					Assert.AreEqual(v3a.Data1, v2a.Data1); // data 1 must be the one from V2
					Assert.AreEqual(v3a.Data3, v2a.Data3); // data 3 must be the one from V2
					break;
				}
				case _31:
				{
					v3a = (TestClassV3)objToSerialize;
					v1a = (TestClassV1)objToDeserialize;
					v1b = (TestClassV1)objToTestAgainst;
					Assert.AreEqual(v1a.Data1, v3a.Data1); // data 1 must be the one from V3
					Assert.AreEqual(v1a.Data2, v1b.Data2); // data 2 can only be the one from V1
					break;
				}
				case _32:
				{
					v3a = (TestClassV3)objToSerialize;
					v2a = (TestClassV2)objToDeserialize;
					v2b = (TestClassV2)objToTestAgainst;
					Assert.AreEqual(v2a.Data1, v3a.Data1); // data 1 must be the one from V3
					Assert.AreEqual(v2a.Data2, v2b.Data2); // data 2 can only be the one from V2
					Assert.AreEqual(v2a.Data3, v3a.Data3); // data 3 must be the one from V3
					break;
				}
				case _33:
				{
					v3a = (TestClassV3)objToSerialize;
					v3b = (TestClassV3)objToDeserialize;
					Assert.AreEqual(v3a.Data1, v3b.Data1); // data 1 must be the one from V3
					Assert.AreEqual(v3a.Data3, v3b.Data3); // data 3 must be the one from V3
					break;
				}
				default:
				{
					Assert.Fail("Test case " + ts.TestCase + " not implemented!");
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

		private static string MakeTempPath()
		{
			return (System.Windows.Forms.Application.StartupPath);
		}

		private static string MakeTempFileName(string name)
		{
			return ("YAT-Test-" + name + ".xml");
		}

		private static string MakeTempFilePath(string name)
		{
			return (MakeTempPath() + Path.DirectorySeparatorChar + MakeTempFileName(name));
		}

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
