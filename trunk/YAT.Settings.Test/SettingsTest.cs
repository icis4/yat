using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;

using NUnit.Framework;

using MKY.Utilities.Recent;

using YAT.Domain.Settings;
using YAT.Gui.Settings;
using YAT.Gui.Types;
using YAT.Settings.Terminal;

namespace YAT.Settings.Test
{
	[TestFixture]
	public class SettingsTest
	{
		//------------------------------------------------------------------------------------------
		// Constants
		//------------------------------------------------------------------------------------------

		private const string _TempPath = "c:\\";
		private const string _TempPrefix = "YAT-Test-";
		private const string _TempExtension = ".txt";

		//------------------------------------------------------------------------------------------
		// Tear Down
		//------------------------------------------------------------------------------------------

		[TearDown]
		public void TearDown()
		{
			string filePath = MakeTempFilePath("*");
			File.Delete(filePath);
		}

		#region Test ArraySerialization()
		//------------------------------------------------------------------------------------------
		// Test ArraySerialization()
		//------------------------------------------------------------------------------------------

		[Test]
		public void TestArraySerialization()
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

		#region Test ListSerialization()
		//------------------------------------------------------------------------------------------
		// Test ListSerialization()
		//------------------------------------------------------------------------------------------

		[Test]
		public void TestListSerialization()
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

		#if false
			// doesn't work
			filePath = MakeTempFilePath("ListOfArrays");
			List<string[]> la = new List<string[]>();
			la.Add(new string[] { "A", "AA" });
			la.Add(new string[] { "B", "BB" });
			Test_Serialization(typeof(List<string>), la, filePath);
		#endif

		#if false
			// doesn't work
			filePath = MakeTempFilePath("ListOfLists");
			List<List<string>> ll = new List<List<string>>();
			ll.Add(l);
			ll.Add(l);
			Test_Serialization(typeof(List<string>), ll, filePath);
		#endif
		}

		#endregion

		#region Test EmptyCommandSerialization()
		//------------------------------------------------------------------------------------------
		// Test EmptyCommandSerialization()
		//------------------------------------------------------------------------------------------

		[Test]
		public void TestEmptyCommandSerialization()
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

		#region Test RecentSerialization()
		//------------------------------------------------------------------------------------------
		// Test RecentSerialization()
		//------------------------------------------------------------------------------------------

		[Test]
		public void TestRecentSerialization()
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

		#region Test PredefinedSerialization()
		//------------------------------------------------------------------------------------------
		// Test PredefinedSerialization()
		//------------------------------------------------------------------------------------------

		[Test]
		public void TestPredefinedSerialization()
		{
			string filePath = "";

			filePath = MakeTempFilePath("PredefinedCommandPage");
			PredefinedCommandPage pcp = new PredefinedCommandPage();
			pcp.Commands.Add(new Command("Hello", "World"));
			pcp.Commands.Add(new Command("Hallo", "Wält"));
			TestSerialization(typeof(PredefinedCommandPage), pcp, filePath);

			List<PredefinedCommandPage> l = new List<PredefinedCommandPage>();
			l.Add(pcp);
			l.Add(pcp);

			filePath = MakeTempFilePath("PredefinedCommandSettings");
			PredefinedCommandSettings pcs = new PredefinedCommandSettings();
			pcs.Pages = l;
			TestSerialization(typeof(PredefinedCommandSettings), pcs, filePath);
		}

		#endregion

		#region Test ExplicitSerialization()
		//------------------------------------------------------------------------------------------
		// Test ExplicitSerialization()
		//------------------------------------------------------------------------------------------

		[Test]
		public void TestExplicitSerialization()
		{
			string filePath = "";

			filePath = MakeTempFilePath("ExplicitSettings");
			ExplicitSettings s = new ExplicitSettings();
			TestSerialization(typeof(ExplicitSettings), s, filePath);
		}

		#endregion

		#region Test ImplicitSerialization()
		//------------------------------------------------------------------------------------------
		// Test ImplicitSerialization()
		//------------------------------------------------------------------------------------------

		[Test]
		public void TestImplicitSerialization()
		{
			string filePath = "";

			filePath = MakeTempFilePath("ImplicitSettings");
			ImplicitSettings s = new ImplicitSettings();
			TestSerialization(typeof(ImplicitSettings), s, filePath);
		}

		#endregion

		#region Private Methods
		//------------------------------------------------------------------------------------------
		// Private Methods
		//------------------------------------------------------------------------------------------

		private static string MakeTempFilePath(string test)
		{
			return (_TempPath + _TempPrefix + test + _TempExtension);
		}

		private static void TestSerialization(Type type, object obj, string filePath)
		{
			FileStream fs = null;
			XmlSerializer serializer = new XmlSerializer(type);

			// save
			try
			{
				fs = new FileStream(filePath, FileMode.Create);
				serializer.Serialize(fs, obj);
				fs.Close();
			}
			catch (Exception ex)
			{
				if (fs != null)
					fs.Close();

				Assert.Fail("XML save error: " + ex.Message);
			}

			// load
			try
			{
				fs = new FileStream(filePath, FileMode.Open);
				obj = serializer.Deserialize(fs);
				fs.Close();
			}
			catch (Exception ex)
			{
				if (fs != null)
					fs.Close();

				Assert.Fail("XML load error: " + ex.Message);
			}
		}

		#endregion
	}
}
