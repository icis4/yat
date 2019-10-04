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
// MKY Version 1.0.27
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2019 Matthias Kläy.
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
using MKY.Collections.Specialized;
using MKY.Diagnostics;
using MKY.IO;
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
			string filePath;
			bool b;

			filePath = Temp.MakeTempFilePath(GetType(), "BooleanFalse", FileExtension);
			b = false;
			TestSerializationChain(typeof(bool), b, filePath);

			filePath = Temp.MakeTempFilePath(GetType(), "BooleanTrue", FileExtension);
			b = true;
			TestSerializationChain(typeof(bool), b, filePath);
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
			TestSerializationChain(typeof(Orientation), e, filePath);

			filePath = Temp.MakeTempFilePath(GetType(), "SimpleEnum_1_Vertical", FileExtension);
			e = Orientation.Vertical;
			TestSerializationChain(typeof(Orientation), e, filePath);
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
			TestSerializationChain(typeof(OrientationEx), x, filePath);

			filePath = Temp.MakeTempFilePath(GetType(), "SimpleEnumEx_1_Vertical", FileExtension);
			x = (OrientationEx)Orientation.Vertical;
			TestSerializationChain(typeof(OrientationEx), x, filePath);
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
			TestSerializationChain(typeof(SupportedEncoding), e, filePath);

			filePath = Temp.MakeTempFilePath(GetType(), "ComplexEnum_Windows1252", FileExtension);
			e = SupportedEncoding.Windows1252;
			TestSerializationChain(typeof(SupportedEncoding), e, filePath);

			filePath = Temp.MakeTempFilePath(GetType(), "ComplexEnum_UTF8_65001", FileExtension);
			e = SupportedEncoding.UTF8;
			TestSerializationChain(typeof(SupportedEncoding), e, filePath);
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
			TestSerializationChain(typeof(EncodingEx), x, filePath);

			filePath = Temp.MakeTempFilePath(GetType(), "ComplexEnumEx_Windows1252", FileExtension);
			x = (EncodingEx)SupportedEncoding.Windows1252;
			TestSerializationChain(typeof(EncodingEx), x, filePath);

			filePath = Temp.MakeTempFilePath(GetType(), "ComplexEnumEx_UTF8_65001", FileExtension);
			x = (EncodingEx)SupportedEncoding.UTF8;
			TestSerializationChain(typeof(EncodingEx), x, filePath);
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
			var a = new string[] { "A", "B" };
			TestSerializationChain(typeof(string[]), a, filePath);

			filePath = Temp.MakeTempFilePath(GetType(), "ArrayEmpty", FileExtension);
			var ae = new string[] { };
			TestSerializationChain(typeof(string[]), ae, filePath);
		#if (FALSE)
			// Doesn't work, not supported for serialization.
			filePath = Temp.MakeTempFilePath(GetType(), "MultiArray", FileExtension);
			var ma = new string[,]
			{
				{ "A", "AA" },
				{ "B", "BB" },
			};
			TestSerialization(typeof(string[,]), ma, filePath);
		#endif
			filePath = Temp.MakeTempFilePath(GetType(), "ArrayOfArraysOnInit", FileExtension);
			var aai = new string[][]
				{
					new string[] { "A", "AA" },
					new string[] { "B", "BB" },
				};
			TestSerializationChain(typeof(string[][]), aai, filePath);

			filePath = Temp.MakeTempFilePath(GetType(), "ArrayOfArraysByCreate", FileExtension);
			var aac = (string[][])Array.CreateInstance(typeof(string[]), 2);
			for (int i = 0; i < 2; i++)
			{
				aac[i] = new string[2];
			}
			aac[0][0] = "A";
			aac[0][1] = "AA";
			aac[1][0] = "B";
			aac[1][1] = "BB";
			TestSerializationChain(typeof(string[][]), aac, filePath);
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
			var l = new List<string>(2); // Preset the required capacity to improve memory management.
			l.Add("A");
			l.Add("B");
			TestSerializationChain(typeof(List<string>), l, filePath);

			filePath = Temp.MakeTempFilePath(GetType(), "ListEmpty", FileExtension);
			var le = new List<string>();
			TestSerializationChain(typeof(List<string>), le, filePath);
		#if (FALSE)
			// Doesn't work, not supported for serialization.
			filePath = Temp.MakeTempFilePath(GetType(), "ListOfArrays", FileExtension);
			var la = new List<string[]>(2); // Preset the required capacity to improve memory management.
			la.Add(new string[] { "A", "AA" });
			la.Add(new string[] { "B", "BB" });
			Test_Serialization(typeof(List<string>), la, filePath);
		#endif
		#if (FALSE)
			// Doesn't work, not supported for serialization.
			filePath = Temp.MakeTempFilePath(GetType(), "ListOfLists", FileExtension);
			var ll = new List<List<string>>(2); // Preset the required capacity to improve memory management.
			ll.Add(l);
			ll.Add(l);
			Test_Serialization(typeof(List<string>), ll, filePath);
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
			var l = new Dictionary<string, string>(2); // Preset the required capacity to improve memory management.
			l.Add("1", "A");
			l.Add("2", "B");
			TestSerialization(typeof(Dictionary<string, string>), l, filePath);
		#endif
		#if (FALSE)
			// Doesn't work, not supported for serialization.
			filePath = Temp.MakeTempFilePath(GetType(), "DictionaryEmpty", FileExtension);
			var le = new Dictionary<string, string>(2); // Preset the required capacity to improve memory management.
			l.Add("1", "A");
			l.Add("2", "B");
			TestSerialization(typeof(Dictionary<string, string>), le, filePath);
		#endif
			filePath = Temp.MakeTempFilePath(GetType(), "DictionaryToArrayOfArrays", FileExtension);
			var l = new Dictionary<string, string>(2); // Preset the required capacity to improve memory management.
			l.Add("1", "A");
			l.Add("2", "B");

			var aa = new string[2][]
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

			TestSerializationChain(typeof(string[][]), aa, filePath);
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
			var nsd = new NamedStringDictionary();
			nsd.Name = "Test";
			nsd.Add("1", "A");
			nsd.Add("2", "B");
			TestSerializationChain(typeof(NamedStringDictionary), nsd, filePath);
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
			var p = new KeyValuePair<int, string>(0, "null");
			TestSerializationChain(typeof(KeyValuePair<int, string>), p, filePath);

			filePath = Temp.MakeTempFilePath(GetType(), "KeyValuePairList", FileExtension);
			var l = new List<KeyValuePair<int, string>>(2); // Preset the required capacity to improve memory management.
			l.Add(new KeyValuePair<int, string>(1, "eins"));
			l.Add(new KeyValuePair<int, string>(2, "zwei"));
			TestSerializationChain(typeof(List<KeyValuePair<int, string>>), l, filePath);

			filePath = Temp.MakeTempFilePath(GetType(), "KeyValuePairArray", FileExtension);
			var a = l.ToArray();
			TestSerializationChain(typeof(KeyValuePair<int, string>[]), a, filePath);
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
			var p1 = new Pair<int, string>(0, "null");
			TestSerializationChain(typeof(Pair<int, string>), p1, filePath);

			filePath = Temp.MakeTempFilePath(GetType(), "Pair2", FileExtension);
			var p2 = new Pair<int[], string>(new int[] { 0, 1, 2 }, "null");
			TestSerializationChain(typeof(Pair<int[], string>), p2, filePath);

			filePath = Temp.MakeTempFilePath(GetType(), "PairList", FileExtension);
			var l = new List<Pair<int, string>>(2); // Preset the required capacity to improve memory management.
			l.Add(new Pair<int, string>(1, "eins"));
			l.Add(new Pair<int, string>(2, "zwei"));
			TestSerializationChain(typeof(List<Pair<int, string>>), l, filePath);

			filePath = Temp.MakeTempFilePath(GetType(), "PairArray", FileExtension);
			var a = l.ToArray();
			TestSerializationChain(typeof(Pair<int, string>[]), a, filePath);
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
			var ri = new RecentItem<string>("RI");
			TestSerializationChain(typeof(RecentItem<string>), ri, filePath);

			filePath = Temp.MakeTempFilePath(GetType(), "RecentItemList", FileExtension);
			var l = new List<RecentItem<string>>(2); // Preset the required capacity to improve memory management.
			l.Add(new RecentItem<string>("RIL1"));
			l.Add(new RecentItem<string>("RIL2"));
			TestSerializationChain(typeof(List<RecentItem<string>>), l, filePath);

			filePath = Temp.MakeTempFilePath(GetType(), "RecentItemArray", FileExtension);
			var a = l.ToArray();
			TestSerializationChain(typeof(RecentItem<string>[]), a, filePath);
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
			TestSerializationChain(typeof(System.Guid), guid, filePath);

			filePath = Temp.MakeTempFilePath(GetType(), "GuidEmpty", FileExtension);
			guid = System.Guid.Empty;
			TestSerializationChain(typeof(System.Guid), guid, filePath);
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
		public static void TestSerializationChain(Type type, object obj, string filePath)
		{
			// Save:
			TestSerializeToFile(type, obj, filePath);

			// Load:
			TestDeserializeFromFile(type, filePath);
			TestTolerantDeserializeFromFile(type, filePath);
			TestAlternateTolerantDeserializeFromFile(type, filePath);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "'obj' is commonly used throughout the .NET framework.")]
		public static void TestSerializeToFile(Type type, object obj, string filePath)
		{
			try
			{
				XmlSerializerEx.SerializeToFile(type, obj, filePath);
			}
			catch (Exception ex)
			{
				TraceEx.WriteException(typeof(XmlSerializerTest), ex);
				Assert.Fail("XML serialize error (using standard serialization): " + ex.Message);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		public static object TestDeserializeFromFile(Type type, string filePath)
		{
			try
			{
				return (XmlSerializerEx.DeserializeFromFile(type, filePath));
			}
			catch (Exception ex)
			{
				TraceEx.WriteException(typeof(XmlSerializerTest), ex);
				Assert.Fail("XML deserialize error (using standard deserialization): " + ex.Message);

				return (null); // The call above throws an exception, still return in order to successfully compile.
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		public static object TestTolerantDeserializeFromFile(Type type, string filePath)
		{
			try
			{
				return (XmlSerializerEx.TolerantDeserializeFromFile(type, filePath));
			}
			catch (Exception ex)
			{
				TraceEx.WriteException(typeof(XmlSerializerTest), ex);
				Assert.Fail("XML deserialize error (using tolerant deserialization): " + ex.Message);

				return (null); // The call above throws an exception, still return in order to successfully compile.
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		public static object TestAlternateTolerantDeserializeFromFile(Type type, string filePath)
		{
			try
			{
				return (XmlSerializerEx.AlternateTolerantDeserializeFromFile(type, null, filePath));
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
