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
// MKY Development Version 1.0.18
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2017 Matthias Kläy.
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

using MKY.Collections;
using MKY.Collections.Generic;
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

		#region TestFixture
		//==========================================================================================
		// TestFixture
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TearDown", Justification = "Naming according to NUnit.")]
		[OneTimeTearDown]
		public virtual void OneTimeTearDown()
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
			string filePath;
			bool b;

			filePath = Temp.MakeTempFilePath(GetType(), "BooleanFalse", FileExtension);
			b = false;
			TestSerializationChain(filePath, typeof(bool), b);

			filePath = Temp.MakeTempFilePath(GetType(), "BooleanTrue", FileExtension);
			b = true;
			TestSerializationChain(filePath, typeof(bool), b);
		}

		#endregion

		#region Tests > Serialization > SimpleEnum
		//------------------------------------------------------------------------------------------
		// Tests > Serialization > SimpleEnum
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// A simple enum 0, 1, 2,... can easy be serialized. However, serialization results in an
		/// XML elemnt that contains the enum's string, e.g. "Horizontal". This may be intentional,
		/// but can also be difficult to understand.
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "What's wrong with 'enum's'?")]
		[Test]
		public virtual void TestSimpleEnumSerialization()
		{
			string filePath;
			Orientation e;

			filePath = Temp.MakeTempFilePath(GetType(), "SimpleEnum_0_Horizontal", FileExtension);
			e = Orientation.Horizontal;
			TestSerializationChain(filePath, typeof(Orientation), e);

			filePath = Temp.MakeTempFilePath(GetType(), "SimpleEnum_1_Vertical", FileExtension);
			e = Orientation.Vertical;
			TestSerializationChain(filePath, typeof(Orientation), e);
		}

		#endregion

		#region Tests > Serialization > SimpleEnumEx
		//------------------------------------------------------------------------------------------
		// Tests > Serialization > SimpleEnumEx
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// <see cref="EnumEx"/> based types are not serializable because <see cref="Enum"/> isn't.
		/// </summary>
		[Test]
		public virtual void TestSimpleEnumExSerialization()
		{
#if (FALSE) // See 'summary' above.
			string filePath;
			OrientationEx x;

			filePath = Temp.MakeTempFilePath(GetType(), "SimpleEnumEx_0_Horizontal", FileExtension);
			x = (OrientationEx)Orientation.Horizontal;
			TestSerializationChain(filePath, typeof(OrientationEx), x);

			filePath = Temp.MakeTempFilePath(GetType(), "SimpleEnumEx_1_Vertical", FileExtension);
			x = (OrientationEx)Orientation.Vertical;
			TestSerializationChain(filePath, typeof(OrientationEx), x);
#endif
		}

		#endregion

		#region Tests > Serialization > ComplexEnum
		//------------------------------------------------------------------------------------------
		// Tests > Serialization > ComplexEnum
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Default serialization also works for a complex enum as shown below. However, the
		/// <see cref="TolerantXmlSerializer"/> is not able to deserialize such enum, because it
		/// does not provide an element 0. This issue has been remarked at bug #232 "Issues with
		/// TolerantXmlSerializer".
		/// </summary>
		[Test]
		public virtual void TestComplexEnumSerialization()
		{
#if (FALSE) // See 'summary' above.
			string filePath;
			SupportedEncoding e;

			filePath = Temp.MakeTempFilePath(GetType(), "ComplexEnum_ASCII_20127", FileExtension);
			e = SupportedEncoding.ASCII;
			TestSerializationChain(filePath, typeof(SupportedEncoding), e);

			filePath = Temp.MakeTempFilePath(GetType(), "ComplexEnum_Windows1252", FileExtension);
			e = SupportedEncoding.Windows1252;
			TestSerializationChain(filePath, typeof(SupportedEncoding), e);

			filePath = Temp.MakeTempFilePath(GetType(), "ComplexEnum_UTF8_65001", FileExtension);
			e = SupportedEncoding.UTF8;
			TestSerializationChain(filePath, typeof(SupportedEncoding), e);
#endif
		}

		#endregion

		#region Tests > Serialization > ComplexEnumEx
		//------------------------------------------------------------------------------------------
		// Tests > Serialization > ComplexEnumEx
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// <see cref="EnumEx"/> based types are not serializable because <see cref="Enum"/> isn't.
		/// </summary>
		[Test]
		public virtual void TestComplexEnumExSerialization()
		{
#if (FALSE) // See 'summary' above.
			string filePath;
			EncodingEx x;

			filePath = Temp.MakeTempFilePath(GetType(), "ComplexEnumEx_ASCII_20127", FileExtension);
			x = (EncodingEx)SupportedEncoding.ASCII;
			TestSerializationChain(filePath, typeof(EncodingEx), x);

			filePath = Temp.MakeTempFilePath(GetType(), "ComplexEnumEx_Windows1252", FileExtension);
			x = (EncodingEx)SupportedEncoding.Windows1252;
			TestSerializationChain(filePath, typeof(EncodingEx), x);

			filePath = Temp.MakeTempFilePath(GetType(), "ComplexEnumEx_UTF8_65001", FileExtension);
			x = (EncodingEx)SupportedEncoding.UTF8;
			TestSerializationChain(filePath, typeof(EncodingEx), x);
#endif
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
			string filePath;

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
			string filePath;

			filePath = Temp.MakeTempFilePath(GetType(), "List", FileExtension);
			List<string> l = new List<string>(2); // Preset the required capacity to improve memory management.
			l.Add("A");
			l.Add("B");
			TestSerializationChain(filePath, typeof(List<string>), l);

			filePath = Temp.MakeTempFilePath(GetType(), "ListEmpty", FileExtension);
			List<string> le = new List<string>();
			TestSerializationChain(filePath, typeof(List<string>), le);
#if (FALSE)
			// Doesn't work, not supported for serialization.
			filePath = Temp.MakeTempFilePath(GetType(), "ListOfArrays", FileExtension);
			List<string[]> la = new List<string[]>(2); // Preset the required capacity to improve memory management.
			la.Add(new string[] { "A", "AA" });
			la.Add(new string[] { "B", "BB" });
			Test_Serialization(filePath, typeof(List<string>), la);
#endif
#if (FALSE)
			// Doesn't work, not supported for serialization.
			filePath = Temp.MakeTempFilePath(GetType(), "ListOfLists", FileExtension);
			List<List<string>> ll = new List<List<string>>(2); // Preset the required capacity to improve memory management.
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
			string filePath;
#if (FALSE)
			// Doesn't work, not supported for serialization.
			filePath = Temp.MakeTempFilePath(GetType(), "Dictionary", FileExtension);
			Dictionary<string, string> l = new Dictionary<string, string>(2); // Preset the required capacity to improve memory management.
			l.Add("1", "A");
			l.Add("2", "B");
			TestSerialization(filePath, typeof(Dictionary<string, string>), l);
#endif
#if (FALSE)
			// Doesn't work, not supported for serialization.
			filePath = Temp.MakeTempFilePath(GetType(), "DictionaryEmpty", FileExtension);
			Dictionary<string, string> le = new Dictionary<string, string>(2); // Preset the required capacity to improve memory management.
			l.Add("1", "A");
			l.Add("2", "B");
			TestSerialization(filePath, typeof(Dictionary<string, string>), le);
#endif
			filePath = Temp.MakeTempFilePath(GetType(), "DictionaryToArrayOfArrays", FileExtension);
			Dictionary<string, string> l = new Dictionary<string, string>(2); // Preset the required capacity to improve memory management.
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
			string filePath = Temp.MakeTempFilePath(GetType(), "NamedStringDictionaryToArrayOfArrays", FileExtension);
			NamedStringDictionary nsd = new NamedStringDictionary();
			nsd.Name = "Test";
			nsd.Add("1", "A");
			nsd.Add("2", "B");
			TestSerializationChain(filePath, typeof(NamedStringDictionary), nsd);
		}

		#endregion

		#region Tests > Serialization > KeyValuePair
		//------------------------------------------------------------------------------------------
		// Tests > Serialization > KeyValuePair
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestKeyValuePairSerialization()
		{
			string filePath;

			filePath = Temp.MakeTempFilePath(GetType(), "KeyValuePair", FileExtension);
			KeyValuePair<int, string> p = new KeyValuePair<int, string>(0, "null");
			TestSerializationChain(filePath, typeof(KeyValuePair<int, string>), p);

			filePath = Temp.MakeTempFilePath(GetType(), "KeyValuePairList", FileExtension);
			List<KeyValuePair<int, string>> l = new List<KeyValuePair<int, string>>(2); // Preset the required capacity to improve memory management.
			l.Add(new KeyValuePair<int, string>(1, "eins"));
			l.Add(new KeyValuePair<int, string>(2, "zwei"));
			TestSerializationChain(filePath, typeof(List<KeyValuePair<int, string>>), l);

			filePath = Temp.MakeTempFilePath(GetType(), "KeyValuePairArray", FileExtension);
			KeyValuePair<int, string>[] a = l.ToArray();
			TestSerializationChain(filePath, typeof(KeyValuePair<int, string>[]), a);
		}

		#endregion

		#region Tests > Serialization > Pair
		//------------------------------------------------------------------------------------------
		// Tests > Serialization > Pair
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestPairSerialization()
		{
			string filePath;

			filePath = Temp.MakeTempFilePath(GetType(), "Pair1", FileExtension);
			Pair<int, string> p1 = new Pair<int, string>(0, "null");
			TestSerializationChain(filePath, typeof(Pair<int, string>), p1);

			filePath = Temp.MakeTempFilePath(GetType(), "Pair2", FileExtension);
			Pair<int[], string> p2 = new Pair<int[], string>(new int[] { 0, 1, 2 }, "null");
			TestSerializationChain(filePath, typeof(Pair<int[], string>), p2);

			filePath = Temp.MakeTempFilePath(GetType(), "PairList", FileExtension);
			List<Pair<int, string>> l = new List<Pair<int, string>>(2); // Preset the required capacity to improve memory management.
			l.Add(new Pair<int, string>(1, "eins"));
			l.Add(new Pair<int, string>(2, "zwei"));
			TestSerializationChain(filePath, typeof(List<Pair<int, string>>), l);

			filePath = Temp.MakeTempFilePath(GetType(), "PairArray", FileExtension);
			Pair<int, string>[] a = l.ToArray();
			TestSerializationChain(filePath, typeof(Pair<int, string>[]), a);
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
			string filePath;

			filePath = Temp.MakeTempFilePath(GetType(), "RecentItem", FileExtension);
			RecentItem<string> ri = new RecentItem<string>("RI");
			TestSerializationChain(filePath, typeof(RecentItem<string>), ri);

			filePath = Temp.MakeTempFilePath(GetType(), "RecentItemList", FileExtension);
			List<RecentItem<string>> l = new List<RecentItem<string>>(2); // Preset the required capacity to improve memory management.
			l.Add(new RecentItem<string>("RIL1"));
			l.Add(new RecentItem<string>("RIL2"));
			TestSerializationChain(filePath, typeof(List<RecentItem<string>>), l);

			filePath = Temp.MakeTempFilePath(GetType(), "RecentItemArray", FileExtension);
			RecentItem<string>[] a = l.ToArray();
			TestSerializationChain(filePath, typeof(RecentItem<string>[]), a);
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
			string filePath;
			System.Guid guid;

			filePath = Temp.MakeTempFilePath(GetType(), "Guid", FileExtension);
			guid = System.Guid.NewGuid();
			TestSerializationChain(filePath, typeof(System.Guid), guid);

			filePath = Temp.MakeTempFilePath(GetType(), "GuidEmpty", FileExtension);
			guid = System.Guid.Empty;
			TestSerializationChain(filePath, typeof(System.Guid), guid);
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
				Assert.Fail("XML deserialize error (using standard deserialization): " + ex.Message);

				return (null); // The call above throws an exception, still return in order to successfully compile.
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
				Assert.Fail("XML deserialize error (using tolerant deserialization): " + ex.Message);

				return (null); // The call above throws an exception, still return in order to successfully compile.
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
				Assert.Fail("XML deserialize error (using alternate tolerant deserialization): " + ex.Message);

				return (null); // The call above throws an exception, still return in order to successfully compile.
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
