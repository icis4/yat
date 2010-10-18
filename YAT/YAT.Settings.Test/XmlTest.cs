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
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml.Serialization;

using NUnit.Framework;

using MKY.System.Collections;
using MKY.System.Diagnostics;
using MKY.System.Recent;
using MKY.System.Test;

using YAT.Model.Settings;
using YAT.Model.Types;
using YAT.Settings.Terminal;

namespace YAT.Settings.Test
{
	/// <summary></summary>
	[TestFixture]
	public class XmlTest
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		const string FileExtension = ".xml";

		#endregion

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
		// Tests
		//==========================================================================================

		#region Tests > Serialization
		//------------------------------------------------------------------------------------------
		// Tests > Serialization
		//------------------------------------------------------------------------------------------

		#region Tests > Serialization > Array
		//------------------------------------------------------------------------------------------
		// Tests > Serialization > Array
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestArraySerialization()
		{
			string filePath = "";

			filePath = XPath.MakeTempFilePath(this, "Array", FileExtension);
			string[] a = new string[] { "A", "B" };
			TestSerialization(typeof(string[]), a, filePath);

			filePath = XPath.MakeTempFilePath(this, "ArrayEmpty", FileExtension);
			string[] ae = new string[] { };
			TestSerialization(typeof(string[]), ae, filePath);
#if (FALSE)
			// Doesn't work, not supported for serialization.
			filePath = XPath.MakeTempFilePath(this, "MultiArray");
			string[,] ma = new string[,]
					{
						{ "A", "AA" },
						{ "B", "BB" },
					};
			TestSerialization(typeof(string[,]), ma, filePath);
#endif
			filePath = XPath.MakeTempFilePath(this, "ArrayOfArraysOnInit", FileExtension);
			string[][] aai = new string[][]
					{
						new string[] { "A", "AA" },
						new string[] { "B", "BB" },
					};
			TestSerialization(typeof(string[][]), aai, filePath);

			filePath = XPath.MakeTempFilePath(this, "ArrayOfArraysByCreate", FileExtension);
			string[][] aac = (string[][])Array.CreateInstance(typeof(string[]), 2);
			for (int i = 0; i < 2; i++)
			{
				aac[i] = new string[2];
			}
			aac[0][0] = "A";
			aac[0][1] = "AA";
			aac[1][0] = "B";
			aac[1][1] = "BB";
			TestSerialization(typeof(string[][]), aac, filePath);
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

			filePath = XPath.MakeTempFilePath(this, "List", FileExtension);
			List<string> l = new List<string>();
			l.Add("A");
			l.Add("B");
			TestSerialization(typeof(List<string>), l, filePath);

			filePath = XPath.MakeTempFilePath(this, "ListEmpty", FileExtension);
			List<string> le = new List<string>();
			TestSerialization(typeof(List<string>), le, filePath);
#if (FALSE)
			// Doesn't work, not supported for serialization.
			filePath = XPath.MakeTempFilePath(this, "ListOfArrays");
			List<string[]> la = new List<string[]>();
			la.Add(new string[] { "A", "AA" });
			la.Add(new string[] { "B", "BB" });
			Test_Serialization(typeof(List<string>), la, filePath);
#endif
#if (FALSE)
			// Doesn't work, not supported for serialization.
			filePath = XPath.MakeTempFilePath(this, "ListOfLists");
			List<List<string>> ll = new List<List<string>>();
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
			string filePath = "";
#if (FALSE)
			// Doesn't work, not supported for serialization.
			filePath = XPath.MakeTempFilePath(this, "Dictionary");
			Dictionary<string, string> l = new Dictionary<string, string>();
			l.Add("1", "A");
			l.Add("2", "B");
			TestSerialization(typeof(Dictionary<string, string>), l, filePath);
#endif
#if (FALSE)
			// Doesn't work, not supported for serialization.
			filePath = XPath.MakeTempFilePath(this, "DictionaryEmpty");
			Dictionary<string, string> le = new Dictionary<string, string>();
			l.Add("1", "A");
			l.Add("2", "B");
			TestSerialization(typeof(Dictionary<string, string>), le, filePath);
#endif
			filePath = XPath.MakeTempFilePath(this, "DictionaryToArrayOfArrays", FileExtension);
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

			TestSerialization(typeof(string[][]), aa, filePath);
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
			string filePath = XPath.MakeTempFilePath(this, "NamedStringDictionaryToArrayOfArrays", FileExtension);
			NamedStringDictionary nsd = new NamedStringDictionary();
			nsd.Name = "Test";
			nsd.Add("1", "A");
			nsd.Add("2", "B");
			TestSerialization(typeof(NamedStringDictionary), nsd, filePath);
		}

		#endregion

		#region Tests > Serialization > EmptyCommand
		//------------------------------------------------------------------------------------------
		// Tests > Serialization > EmptyCommand
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestEmptyCommandSerialization()
		{
			string filePath = "";

			filePath = XPath.MakeTempFilePath(this, "EmptyArrayOfCommands", FileExtension);
			Command[] a = new Command[] { };
			TestSerialization(typeof(Command[]), a, filePath);

			filePath = XPath.MakeTempFilePath(this, "EmptyListOfCommands", FileExtension);
			List<Command> l = new List<Command>();
			TestSerialization(typeof(List<Command>), l, filePath);
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

			filePath = XPath.MakeTempFilePath(this, "RecentItem", FileExtension);
			RecentItem<string> ri = new RecentItem<string>("RI");
			TestSerialization(typeof(RecentItem<string>), ri, filePath);

			RecentItemCollection<string> ric = new RecentItemCollection<string>();
			ric.Add(ri);
			ric.Add(ri);

			filePath = XPath.MakeTempFilePath(this, "RecentFileSettings", FileExtension);
			RecentFileSettings rfs = new RecentFileSettings();
			rfs.FilePaths = ric;
			TestSerialization(typeof(RecentFileSettings), rfs, filePath);
		}

		#endregion

		#region Tests > Serialization > Predefined
		//------------------------------------------------------------------------------------------
		// Tests > Serialization > Predefined
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestPredefinedSerialization()
		{
			string filePath = "";

			filePath = XPath.MakeTempFilePath(this, "PredefinedCommandPage", FileExtension);
			PredefinedCommandPage pcp = new PredefinedCommandPage();
			pcp.Commands.Add(new Command("Hello", "World"));
			pcp.Commands.Add(new Command("Hallo", "W�lt"));
			TestSerialization(typeof(PredefinedCommandPage), pcp, filePath);

			PredefinedCommandPageCollection c = new PredefinedCommandPageCollection();
			c.Add(pcp);
			c.Add(pcp);

			filePath = XPath.MakeTempFilePath(this, "PredefinedCommandSettings", FileExtension);
			PredefinedCommandSettings pcs = new PredefinedCommandSettings();
			pcs.Pages = c;
			TestSerialization(typeof(PredefinedCommandSettings), pcs, filePath);
		}

		#endregion

		#region Tests > Serialization > Explicit
		//------------------------------------------------------------------------------------------
		// Tests > Serialization > Explicit
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestExplicitSerialization()
		{
			string filePath = "";

			filePath = XPath.MakeTempFilePath(this, "ExplicitSettings", FileExtension);
			ExplicitSettings s = new ExplicitSettings();
			TestSerialization(typeof(ExplicitSettings), s, filePath);
		}

		#endregion

		#region Tests > Serialization > Implicit
		//------------------------------------------------------------------------------------------
		// Tests > Serialization > Implicit
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestImplicitSerialization()
		{
			string filePath = "";

			filePath = XPath.MakeTempFilePath(this, "ImplicitSettings", FileExtension);
			ImplicitSettings s = new ImplicitSettings();
			TestSerialization(typeof(ImplicitSettings), s, filePath);
		}

		#endregion

		#endregion

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Enusre that really all exceptions get caught.")]
		private static void TestSerialization(Type type, object obj, string filePath)
		{
			// Save.
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
				XConsole.WriteException(typeof(XmlTest), ex);

				// Attention: The following call throws an exception, code below it won't be executed.
				Assert.Fail("XML serialize error: " + ex.Message);
			}

			// Load.
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
				XConsole.WriteException(typeof(XmlTest), ex);

				// Attention: The following call throws an exception, code below it won't be executed.
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
