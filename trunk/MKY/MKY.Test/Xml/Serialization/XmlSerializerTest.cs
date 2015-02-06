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
// YAT 2.0 Beta 4 Candidate 3 Development Version 1.99.31
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using MKY.Collections;
using MKY.Diagnostics;
using MKY.IO;
using MKY.Recent;
using MKY.Xml.Serialization;

using NUnit.Framework;

#endregion

namespace MKY.Test.Xml.Serialization
{
	/// <summary></summary>
	[TestFixture]
	public class XmlSerializerTest
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const string FileExtension = ".xml";

		#endregion

		#region Tear Down Fixture
		//==========================================================================================
		// Tear Down Fixture
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
		// Tests
		//==========================================================================================

		#region Tests > Serialization
		//------------------------------------------------------------------------------------------
		// Tests > Serialization
		//------------------------------------------------------------------------------------------

		#region Tests > Serialization > Boolean
		//------------------------------------------------------------------------------------------
		// Tests > Serialization > Boolean
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestBooleanSerialization()
		{
			string filePath = "";
			bool b;

			filePath = Temp.MakeTempFilePath(GetType(), "BooleanFalse", FileExtension);
			b = false;
			TestSerializationChain(filePath, typeof(bool), b);

			filePath = Temp.MakeTempFilePath(GetType(), "BooleanTrue", FileExtension);
			b = true;
			TestSerializationChain(filePath, typeof(bool), b);
		}

		#endregion

		#region Tests > Serialization > Guid
		//------------------------------------------------------------------------------------------
		// Tests > Serialization > Guid
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestGuidSerialization()
		{
			string filePath = "";
			System.Guid guid;

			filePath = Temp.MakeTempFilePath(GetType(), "Guid", FileExtension);
			guid = System.Guid.NewGuid();
			TestSerializationChain(filePath, typeof(System.Guid), guid);

			filePath = Temp.MakeTempFilePath(GetType(), "GuidEmpty", FileExtension);
			guid = System.Guid.Empty;
			TestSerializationChain(filePath, typeof(System.Guid), guid);
		}

		#endregion

		#region Tests > Serialization > Array
		//------------------------------------------------------------------------------------------
		// Tests > Serialization > Array
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestArraySerialization()
		{
			string filePath = "";

			filePath = Temp.MakeTempFilePath(GetType(), "Array", FileExtension);
			string[] a = new string[] { "A", "B" };
			TestSerializationChain(filePath, typeof(string[]), a);

			filePath = Temp.MakeTempFilePath(GetType(), "ArrayEmpty", FileExtension);
			string[] ae = new string[] { };
			TestSerializationChain(filePath, typeof(string[]), ae);
#if (FALSE)
			// Doesn't work, not supported for serialization.
			filePath = Temp.MakeTempFilePath(GetType(), "MultiArray", FileExtension);
			string[,] ma = new string[,]
					{
						{ "A", "AA" },
						{ "B", "BB" },
					};
			TestSerialization(filePath, typeof(string[,]), ma);
#endif
			filePath = Temp.MakeTempFilePath(GetType(), "ArrayOfArraysOnInit", FileExtension);
			string[][] aai = new string[][]
					{
						new string[] { "A", "AA" },
						new string[] { "B", "BB" },
					};
			TestSerializationChain(filePath, typeof(string[][]), aai);

			filePath = Temp.MakeTempFilePath(GetType(), "ArrayOfArraysByCreate", FileExtension);
			string[][] aac = (string[][])Array.CreateInstance(typeof(string[]), 2);
			for (int i = 0; i < 2; i++)
			{
				aac[i] = new string[2];
			}
			aac[0][0] = "A";
			aac[0][1] = "AA";
			aac[1][0] = "B";
			aac[1][1] = "BB";
			TestSerializationChain(filePath, typeof(string[][]), aac);
		}

		#endregion

		#region Tests > Serialization > List
		//------------------------------------------------------------------------------------------
		// Tests > Serialization > List
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestListSerialization()
		{
			string filePath = "";

			filePath = Temp.MakeTempFilePath(GetType(), "List", FileExtension);
			List<string> l = new List<string>();
			l.Add("A");
			l.Add("B");
			TestSerializationChain(filePath, typeof(List<string>), l);

			filePath = Temp.MakeTempFilePath(GetType(), "ListEmpty", FileExtension);
			List<string> le = new List<string>();
			TestSerializationChain(filePath, typeof(List<string>), le);
#if (FALSE)
			// Doesn't work, not supported for serialization.
			filePath = Temp.MakeTempFilePath(GetType(), "ListOfArrays", FileExtension);
			List<string[]> la = new List<string[]>();
			la.Add(new string[] { "A", "AA" });
			la.Add(new string[] { "B", "BB" });
			Test_Serialization(filePath, typeof(List<string>), la);
#endif
#if (FALSE)
			// Doesn't work, not supported for serialization.
			filePath = Temp.MakeTempFilePath(GetType(), "ListOfLists", FileExtension);
			List<List<string>> ll = new List<List<string>>();
			ll.Add(l);
			ll.Add(l);
			Test_Serialization(filePath, typeof(List<string>), ll);
#endif
		}

		#endregion

		#region Tests > Serialization > Dictionary
		//------------------------------------------------------------------------------------------
		// Tests > Serialization > Dictionary
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestDictionarySerialization()
		{
			int i = 0;
			string filePath = "";
#if (FALSE)
			// Doesn't work, not supported for serialization.
			filePath = Temp.MakeTempFilePath(GetType(), "Dictionary", FileExtension);
			Dictionary<string, string> l = new Dictionary<string, string>();
			l.Add("1", "A");
			l.Add("2", "B");
			TestSerialization(filePath, typeof(Dictionary<string, string>), l);
#endif
#if (FALSE)
			// Doesn't work, not supported for serialization.
			filePath = Temp.MakeTempFilePath(GetType(), "DictionaryEmpty", FileExtension);
			Dictionary<string, string> le = new Dictionary<string, string>();
			l.Add("1", "A");
			l.Add("2", "B");
			TestSerialization(filePath, typeof(Dictionary<string, string>), le);
#endif
			filePath = Temp.MakeTempFilePath(GetType(), "DictionaryToArrayOfArrays", FileExtension);
			Dictionary<string, string> l = new Dictionary<string, string>();
			l.Add("1", "A");
			l.Add("2", "B");

			string[][] aa = new string[2][]
					{
						new string[2],
						new string[2],
					};
			i = 0;
			foreach (string key in l.Keys)
			{
				aa[i][0] = key;
				i++;
			}
			i = 0;
			foreach (string value in l.Values)
			{
				aa[i][1] = value;
				i++;
			}

			TestSerializationChain(filePath, typeof(string[][]), aa);
		}

		#endregion

		#region Tests > Serialization > NamedStringDictionary
		//------------------------------------------------------------------------------------------
		// Tests > Serialization > NamedStringDictionary
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestNamedStringDictionarySerialization()
		{
			string filePath = "";

			filePath = Temp.MakeTempFilePath(GetType(), "NamedStringDictionaryToArrayOfArrays", FileExtension);
			NamedStringDictionary nsd = new NamedStringDictionary();
			nsd.Name = "Test";
			nsd.Add("1", "A");
			nsd.Add("2", "B");
			TestSerializationChain(filePath, typeof(NamedStringDictionary), nsd);
		}

		#endregion

		#region Tests > Serialization > Recent
		//------------------------------------------------------------------------------------------
		// Tests > Serialization > Recent
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestRecentSerialization()
		{
			string filePath = "";

			filePath = Temp.MakeTempFilePath(GetType(), "RecentItem", FileExtension);
			RecentItem<string> ri = new RecentItem<string>("RI");
			TestSerializationChain(filePath, typeof(RecentItem<string>), ri);

			filePath = Temp.MakeTempFilePath(GetType(), "RecentItemList", FileExtension);
			List<RecentItem<string>> ril = new List<RecentItem<string>>();
			ril.Add(new RecentItem<string>("RIL1"));
			ril.Add(new RecentItem<string>("RIL2"));
			TestSerializationChain(filePath, typeof(List<RecentItem<string>>), ril);

			filePath = Temp.MakeTempFilePath(GetType(), "RecentItemArray", FileExtension);
			RecentItem<string>[] ria = ril.ToArray();
			TestSerializationChain(filePath, typeof(RecentItem<string>[]), ria);
		}

		#endregion

		#endregion

		#endregion

		#region Public Test Methods
		//==========================================================================================
		// Public Test Methods
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "'obj' is commonly used throughout the .NET framework.")]
		public static void TestSerializationChain(string filePath, Type type, object obj)
		{
			// Save:
			TestSerializeToFile(filePath, type, obj);

			// Load:
			TestDeserializeFromFile(filePath, type);
			TestTolerantDeserializeFromFile(filePath, type);
			TestAlternateTolerantDeserializeFromFile(filePath, type);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "'obj' is commonly used throughout the .NET framework.")]
		public static void TestSerializeToFile(string filePath, Type type, object obj)
		{
			try
			{
				XmlSerializerEx.SerializeToFile(filePath, type, obj);
			}
			catch (Exception ex)
			{
				TraceEx.WriteException(typeof(XmlSerializerTest), ex);

				// Attention: The following call throws an exception, code after the call will not be executed.
				Assert.Fail("XML serialize error (using standard serialization): " + ex.Message);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		public static object TestDeserializeFromFile(string filePath, Type type)
		{
			try
			{
				return (XmlSerializerEx.DeserializeFromFile(filePath, type));
			}
			catch (Exception ex)
			{
				TraceEx.WriteException(typeof(XmlSerializerTest), ex);

				// Attention: The following call throws an exception, code after the call will not be executed.
				Assert.Fail("XML deserialize error (using standard deserialization): " + ex.Message);

				return (null);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		public static object TestTolerantDeserializeFromFile(string filePath, Type type)
		{
			try
			{
				return (XmlSerializerEx.TolerantDeserializeFromFile(filePath, type));
			}
			catch (Exception ex)
			{
				TraceEx.WriteException(typeof(XmlSerializerTest), ex);

				// Attention: The following call throws an exception, code after the call will not be executed.
				Assert.Fail("XML deserialize error (using tolerant deserialization): " + ex.Message);

				return (null);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		public static object TestAlternateTolerantDeserializeFromFile(string filePath, Type type)
		{
			try
			{
				return (XmlSerializerEx.AlternateTolerantDeserializeFromFile(filePath, type, null));
			}
			catch (Exception ex)
			{
				TraceEx.WriteException(typeof(XmlSerializerTest), ex);

				// Attention: The following call throws an exception, code after the call will not be executed.
				Assert.Fail("XML deserialize error (using alternate tolerant deserialization): " + ex.Message);

				return (null);
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
