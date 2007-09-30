using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace YAT.Test
{
	public class Test_Settings
	{
		public static void Test()
		{
			string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + Path.DirectorySeparatorChar + "_Test.xml";

			Test_Serialization(filePath);
		}

		private static void Test_Serialization(string filePath)
		{
			Settings.Terminal.TerminalSettingsRoot settings = new Settings.Terminal.TerminalSettingsRoot();
			//Gui.Settings.FormatSettings settings = new Gui.Settings.FormatSettings();

			XmlTextWriter writer = new XmlTextWriter(filePath, null);
			writer.Formatting = Formatting.Indented;
			XmlSerializer serializer = new XmlSerializer(typeof(Settings.Terminal.TerminalSettingsRoot));
			//XmlSerializer serializer = new XmlSerializer(typeof(Gui.Settings.FormatSettings));
			serializer.Serialize(writer, settings);
			writer.Close();
		}
	}
}
