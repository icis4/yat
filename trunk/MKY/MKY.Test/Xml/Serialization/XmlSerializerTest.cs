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
// YAT 2.0 Beta 4 Candidate 2 Development Version 1.99.29
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
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
using System.IO;
using System.Text;
using System.Xml.Serialization;

using NUnit.Framework;

using MKY.Collections;
using MKY.Diagnostics;
using MKY.IO;
using MKY.Recent;
using MKY.Xml;
using MKY.Xml.Serialization;

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
			Temp.CleanTempPath(this.GetType());
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
			System.Boolean b;

			filePath = Temp.MakeTempFilePath(this.GetType(), "BooleanFalse", FileExtension);
			b = false;
			TestSerialization(filePath, typeof(bool), b);

			filePath = Temp.MakeTempFilePath(this.GetType(), "BooleanTrue", FileExtension);
			b = true;
			TestSerialization(filePath, typeof(bool), b);
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

			filePath = Temp.MakeTempFilePath(this.GetType(), "Guid", FileExtension);
			guid = System.Guid.NewGuid();
			TestSerialization(filePath, typeof(System.Guid), guid);

			filePath = Temp.MakeTempFilePath(this.GetType(), "GuidEmpty", FileExtension);
			guid = System.Guid.Empty;
			TestSerialization(filePath, typeof(System.Guid), guid);
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

			filePath = Temp.MakeTempFilePath(this.GetType(), "Array", FileExtension);
			string[] a = new string[] { "A", "B" };
			TestSerialization(filePath, typeof(string[]), a);

			filePath = Temp.MakeTempFilePath(this.GetType(), "ArrayEmpty", FileExtension);
			string[] ae = new string[] { };
			TestSerialization(filePath, typeof(string[]), ae);
#if (FALSE)
			// Doesn't work, not supported for serialization.
			filePath = Temp.MakeTempFilePath(this.GetType(), "MultiArray", FileExtension);
			string[,] ma = new string[,]
					{
						{ "A", "AA" },
						{ "B", "BB" },
					};
			TestSerialization(filePath, typeof(string[,]), ma);
#endif
			filePath = Temp.MakeTempFilePath(this.GetType(), "ArrayOfArraysOnInit", FileExtension);
			string[][] aai = new string[][]
					{
						new string[] { "A", "AA" },
						new string[] { "B", "BB" },
					};
			TestSerialization(filePath, typeof(string[][]), aai);

			filePath = Temp.MakeTempFilePath(this.GetType(), "ArrayOfArraysByCreate", FileExtension);
			string[][] aac = (string[][])Array.CreateInstance(typeof(string[]), 2);
			for (int i = 0; i < 2; i++)
			{
				aac[i] = new string[2];
			}
			aac[0][0] = "A";
			aac[0][1] = "AA";
			aac[1][0] = "B";
			aac[1][1] = "BB";
			TestSerialization(filePath, typeof(string[][]), aac);
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

			filePath = Temp.MakeTempFilePath(this.GetType(), "List", FileExtension);
			List<string> l = new List<string>();
			l.Add("A");
			l.Add("B");
			TestSerialization(filePath, typeof(List<string>), l);

			filePath = Temp.MakeTempFilePath(this.GetType(), "ListEmpty", FileExtension);
			List<string> le = new List<string>();
			TestSerialization(filePath, typeof(List<string>), le);
#if (FALSE)
			// Doesn't work, not supported for serialization.
			filePath = Temp.MakeTempFilePath(this.GetType(), "ListOfArrays", FileExtension);
			List<string[]> la = new List<string[]>();
			la.Add(new string[] { "A", "AA" });
			la.Add(new string[] { "B", "BB" });
			Test_Serialization(filePath, typeof(List<string>), la);
#endif
#if (FALSE)
			// Doesn't work, not supported for serialization.
			filePath = Temp.MakeTempFilePath(this.GetType(), "ListOfLists", FileExtension);
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
			filePath = Temp.MakeTempFilePath(this.GetType(), "Dictionary", FileExtension);
			Dictionary<string, string> l = new Dictionary<string, string>();
			l.Add("1", "A");
			l.Add("2", "B");
			TestSerialization(filePath, typeof(Dictionary<string, string>), l);
#endif
#if (FALSE)
			// Doesn't work, not supported for serialization.
			filePath = Temp.MakeTempFilePath(this.GetType(), "DictionaryEmpty", FileExtension);
			Dictionary<string, string> le = new Dictionary<string, string>();
			l.Add("1", "A");
			l.Add("2", "B");
			TestSerialization(filePath, typeof(Dictionary<string, string>), le);
#endif
			filePath = Temp.MakeTempFilePath(this.GetType(), "DictionaryToArrayOfArrays", FileExtension);
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

			TestSerialization(filePath, typeof(string[][]), aa);
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

			filePath = Temp.MakeTempFilePath(this.GetType(), "NamedStringDictionaryToArrayOfArrays", FileExtension);
			NamedStringDictionary nsd = new NamedStringDictionary();
			nsd.Name = "Test";
			nsd.Add("1", "A");
			nsd.Add("2", "B");
			TestSerialization(filePath, typeof(NamedStringDictionary), nsd);
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

			filePath = Temp.MakeTempFilePath(this.GetType(), "RecentItem", FileExtension);
			RecentItem<string> ri = new RecentItem<string>("RI");
			TestSerialization(filePath, typeof(RecentItem<string>), ri);

			filePath = Temp.MakeTempFilePath(this.GetType(), "RecentItemList", FileExtension);
			List<RecentItem<string>> ril = new List<RecentItem<string>>();
			ril.Add(new RecentItem<string>("RIL1"));
			ril.Add(new RecentItem<string>("RIL2"));
			TestSerialization(filePath, typeof(List<RecentItem<string>>), ril);

			filePath = Temp.MakeTempFilePath(this.GetType(), "RecentItemArray", FileExtension);
			RecentItem<string>[] ria = ril.ToArray();
			TestSerialization(filePath, typeof(RecentItem<string>[]), ria);
		}

		#endregion

		#endregion

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Enusre that really all exceptions get caught.")]
		private static void TestSerialization(string filePath, Type type, object obj)
		{
			// Save.
			try
			{
				SerializeToFile(filePath, type, obj);
			}
			catch (Exception ex)
			{
				TraceEx.WriteException(typeof(XmlSerializerTest), ex);

				// Attention: The following call throws an exception, code below that call won't be executed.
				Assert.Fail("XML serialize error: " + ex.Message);
			}

			// Load.
			try
			{
				DeserializeFromFile(filePath, type);
			}
			catch (Exception ex)
			{
				TraceEx.WriteException(typeof(XmlSerializerTest), ex);

				// Attention: The following call throws an exception, code below that call won't be executed.
				Assert.Fail("XML deserialize error: " + ex.Message);
			}

			try
			{
				TolerantDeserializeFromFile(filePath, type);
			}
			catch (Exception ex)
			{
				TraceEx.WriteException(typeof(XmlSerializerTest), ex);

				// Attention: The following call throws an exception, code below that call won't be executed.
				Assert.Fail("XML deserialize error: " + ex.Message);
			}

			try
			{
				AlternateTolerantDeserializeFromFile(filePath, type, null);
			}
			catch (Exception ex)
			{
				TraceEx.WriteException(typeof(XmlSerializerTest), ex);

				// Attention: The following call throws an exception, code below that call won't be executed.
				Assert.Fail("XML deserialize error: " + ex.Message);
			}
		}

		#endregion

		#region Static Methods
		//==========================================================================================
		// Static Methods
		//==========================================================================================

		#region Static Methods > Serialize
		//------------------------------------------------------------------------------------------
		// Static Methods > Serialize
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public static void SerializeToFile(string filePath, Type type, object settings)
		{
			using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8))
			{
				XmlSerializer serializer = new XmlSerializer(type);
				serializer.Serialize(sw, settings);
			}
		}

		#endregion

		#region Static Methods > Deserialize
		//------------------------------------------------------------------------------------------
		// Static Methods > Deserialize
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public static object DeserializeFromFile(string filePath, Type type)
		{
			object settings = null;
			using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8, true))
			{
				XmlSerializer serializer = new XmlSerializer(type);
				settings = serializer.Deserialize(sr);
			}
			return (settings);
		}

		/// <summary></summary>
		public static object TolerantDeserializeFromFile(string filePath, Type type)
		{
			object settings = null;
			using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8, true))
			{
				TolerantXmlSerializer serializer = new TolerantXmlSerializer(type);
				settings = serializer.Deserialize(sr);
			}
			return (settings);
		}

		/// <summary></summary>
		public static object AlternateTolerantDeserializeFromFile(string filePath, Type type, AlternateXmlElement[] alternateXmlElements)
		{
			object settings = null;
			using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8, true))
			{
				AlternateTolerantXmlSerializer serializer = new AlternateTolerantXmlSerializer(type, alternateXmlElements);
				settings = serializer.Deserialize(sr);
			}
			return (settings);
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
