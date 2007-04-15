using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;

using HSR.Utilities.Recent;
using HSR.YAT.Settings.Document;
using HSR.YAT.Domain.Settings;
using HSR.YAT.Gui;
using HSR.YAT.Gui.Settings;

namespace HSR.YAT.Test
{
	public class Test_Xml
	{
		public static void Test()
		{
			string filePath = "";

		#if false
			// array
			{
				filePath = "c:\\YAT-TestXml-Array.txt";
				string[] a = new string[] { "A", "B" };
				Test_Serialization(typeof(string[]), a, filePath);

				filePath = "c:\\YAT-TestXml-ArrayEmpty.txt";
				string[] ae = new string[] { };
				Test_Serialization(typeof(string[]), ae, filePath);

				filePath = "c:\\YAT-TestXml-ArrayOfArrays.txt";
				string[][] aa = new string[][]
					{
						new string[] { "A", "AA" },
						new string[] { "B", "BB" }
					};
				Test_Serialization(typeof(string[][]), aa, filePath);
			}
		#endif

		#if false
			// list
			{
				filePath = "c:\\YAT-TestXml-List.txt";
				List<string> l = new List<string>();
				l.Add("A");
				l.Add("B");
				Test_Serialization(typeof(List<string>), l, filePath);

				filePath = "c:\\YAT-TestXml-ListEmpty.txt";
				List<string> le = new List<string>();
				Test_Serialization(typeof(List<string>), le, filePath);

				/* doesn't work
				filePath = "c:\\YAT-TestXml-ListOfArrays.txt";
				List<string[]> la = new List<string[]>();
				la.Add(new string[] { "A", "AA" });
				la.Add(new string[] { "B", "BB" });
				Test_Serialization(typeof(List<string>), la, filePath);*/

				/* doesn't work
				filePath = "c:\\YAT-TestXml-ListOfLists.txt";
				List<List<string>> ll = new List<List<string>>();
				ll.Add(l);
				ll.Add(l);
				Test_Serialization(typeof(List<string>), ll, filePath);*/
			}
		#endif

		#if false
			// empty commands
			{
				filePath = "c:\\YAT-TestXml-EmptyArrayOfCommands.txt";
				Command[] a = new Command[] { };
				Test_Serialization(typeof(Command[]), a, filePath);

				filePath = "c:\\YAT-TestXml-EmptyListOfCommands.txt";
				List<Command> l = new List<Command>();
				Test_Serialization(typeof(List<Command>), l, filePath);
			}
		#endif

		#if false
			// recent
			{
				filePath = "c:\\YAT-TestXml-RecentItem.txt";
				RecentItem<string> ri = new RecentItem<string>("RI");
				Test_Serialization(typeof(RecentItem<string>), ri, filePath);

				RecentItemCollection<string> ric = new RecentItemCollection<string>();
				ric.Add(ri);
				ric.Add(ri);

				filePath = "c:\\YAT-TestXml-RecentFileSettings.txt";
				RecentFileSettings rfs = new RecentFileSettings();
				rfs.FilePaths = ric;
				Test_Serialization(typeof(RecentFileSettings), rfs, filePath);
			}
		#endif

		#if false
			// predefined
			{
				filePath = "c:\\YAT-TestXml-PredefinedCommandPage.txt";
				PredefinedCommandPage pcp = new PredefinedCommandPage();
				pcp.Commands.Add(new Command("Hello", "World"));
				pcp.Commands.Add(new Command("Hallo", "Wält"));
				Test_Serialization(typeof(PredefinedCommandPage), pcp, filePath);

				List<PredefinedCommandPage> l = new List<PredefinedCommandPage>();
				l.Add(pcp);
				l.Add(pcp);

				filePath = "c:\\YAT-TestXml-PredefinedCommandSettings.txt";
				PredefinedCommandSettings pcs = new PredefinedCommandSettings();
				pcs.Pages = l;
				Test_Serialization(typeof(PredefinedCommandSettings), pcs, filePath);
			}
		#endif

		#if false
			{
				filePath = "c:\\YAT-TestXml-RecentFileSettings.txt";
				RecentFileSettings s = new RecentFileSettings();
				Test_Serialization(typeof(RecentFileSettings), s, filePath);
			}
		#endif

		#if true
			// explicit
			{
				filePath = "c:\\YAT-TestXml-ExplicitSettings.txt";
				ExplicitSettings s = new ExplicitSettings();
				Test_Serialization(typeof(ExplicitSettings), s, filePath);
			}
		#endif

		#if true
			// implicit
			{
				filePath = "c:\\YAT-TestXml-ImplicitSettings.txt";
				ImplicitSettings s = new ImplicitSettings();
				Test_Serialization(typeof(ImplicitSettings), s, filePath);
			}
		#endif
		}

		private static void Test_Serialization(Type type, object obj, string filePath)
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
		
				Console.WriteLine("XML save error: " + ex.Message);
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

				Console.WriteLine("XML load error: " + ex.Message);
			}
		}
	}
}
