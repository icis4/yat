//==================================================================================================
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
using System.IO;
using System.Xml.Serialization;

using NUnit.Framework;

using MKY.Utilities.Diagnostics;
using MKY.Utilities.Recent;

using YAT.Model.Settings;
using YAT.Model.Types;
using YAT.Settings.Terminal;

namespace YAT.Settings.Test
{
	/// <summary></summary>
	[TestFixture]
	public class XmlTest
	{
		#region Tear Down
		//==========================================================================================
		// Tear Down
		//==========================================================================================

		/// <summary></summary>
		[TearDown]
		public virtual void TearDown()
		{
			foreach (string filePath in Directory.GetFiles(MakeTempPath(), MakeTempFileName("*")))
				File.Delete(filePath);
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

			filePath = MakeTempFilePath("Array");
			string[] a = new string[] { "A", "B" };
			TestSerialization(typeof(string[]), a, filePath);

			filePath = MakeTempFilePath("ArrayEmpty");
			string[] ae = new string[] { };
			TestSerialization(typeof(string[]), ae, filePath);

			filePath = MakeTempFilePath("ArrayOfArrays");
			string[][] aa = new string[][]
					{
						new string[] { "A", "AA" },
						new string[] { "B", "BB" }
					};
			TestSerialization(typeof(string[][]), aa, filePath);
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

			filePath = MakeTempFilePath("List");
			List<string> l = new List<string>();
			l.Add("A");
			l.Add("B");
			TestSerialization(typeof(List<string>), l, filePath);

			filePath = MakeTempFilePath("ListEmpty");
			List<string> le = new List<string>();
			TestSerialization(typeof(List<string>), le, filePath);
#if (FALSE)
			// doesn't work
			filePath = MakeTempFilePath("ListOfArrays");
			List<string[]> la = new List<string[]>();
			la.Add(new string[] { "A", "AA" });
			la.Add(new string[] { "B", "BB" });
			Test_Serialization(typeof(List<string>), la, filePath);
#endif
#if (FALSE)
			// doesn't work
			filePath = MakeTempFilePath("ListOfLists");
			List<List<string>> ll = new List<List<string>>();
			ll.Add(l);
			ll.Add(l);
			Test_Serialization(typeof(List<string>), ll, filePath);
#endif
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

			filePath = MakeTempFilePath("EmptyArrayOfCommands");
			Command[] a = new Command[] { };
			TestSerialization(typeof(Command[]), a, filePath);

			filePath = MakeTempFilePath("EmptyListOfCommands");
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

			filePath = MakeTempFilePath("RecentItem");
			RecentItem<string> ri = new RecentItem<string>("RI");
			TestSerialization(typeof(RecentItem<string>), ri, filePath);

			RecentItemCollection<string> ric = new RecentItemCollection<string>();
			ric.Add(ri);
			ric.Add(ri);

			filePath = MakeTempFilePath("RecentFileSettings");
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

			filePath = MakeTempFilePath("PredefinedCommandPage");
			PredefinedCommandPage pcp = new PredefinedCommandPage();
			pcp.Commands.Add(new Command("Hello", "World"));
			pcp.Commands.Add(new Command("Hallo", "W�lt"));
			TestSerialization(typeof(PredefinedCommandPage), pcp, filePath);

			PredefinedCommandPageCollection c = new PredefinedCommandPageCollection();
			c.Add(pcp);
			c.Add(pcp);

			filePath = MakeTempFilePath("PredefinedCommandSettings");
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

			filePath = MakeTempFilePath("ExplicitSettings");
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

			filePath = MakeTempFilePath("ImplicitSettings");
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

		private static string MakeTempPath()
		{
			//return (Path.GetTempPath() + Path.DirectorySeparatorChar + "YAT");
			return ("D:");
		}

		private static string MakeTempFileName(string name)
		{
			return ("YAT-Test-" + name + ".xml");
		}

		private static string MakeTempFilePath(string name)
		{
			return (MakeTempPath() + Path.DirectorySeparatorChar + MakeTempFileName(name));
		}

		private static void TestSerialization(Type type, object obj, string filePath)
		{
			// Save
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

				// Attention: The following call throws an exception, code below it won't be executed
				Assert.Fail("XML serialize error: " + ex.Message);
			}

			// Load
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
