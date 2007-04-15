using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace HSR.YAT.Settings.Application
{
	[XmlRoot("LocalUserSettings")]
	public class LocalUserSettings
	{
		[XmlElement("Version")]
		public string Version = System.Windows.Forms.Application.ProductVersion;

		[XmlElement("Paths")]
		public Settings.PathSettings Path = new Settings.PathSettings(Utilities.Settings.SettingsType.Explicit);

		[XmlElement("MainWindow")]
		public Gui.Settings.MainWindowSettings MainWindow = new Gui.Settings.MainWindowSettings(Utilities.Settings.SettingsType.Implicit);

		[XmlElement("NewTerminal")]
		public Gui.Settings.NewTerminalSettings NewTerminal = new Gui.Settings.NewTerminalSettings(Utilities.Settings.SettingsType.Implicit);

		[XmlElement("RecentFiles")]
		public Gui.Settings.RecentFileSettings RecentFiles = new Gui.Settings.RecentFileSettings(Utilities.Settings.SettingsType.Implicit);
	}
}
