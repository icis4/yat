using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MKY.YAT.Settings.Application
{
	[Serializable]
	[XmlRoot("LocalUserSettings")]
	public class LocalUserSettingsRoot
	{
		[XmlElement("Version")]
		public string Version = System.Windows.Forms.Application.ProductVersion;

		[XmlElement("General")]
		public Settings.GeneralSettings General = new Settings.GeneralSettings(Utilities.Settings.SettingsType.Explicit);

		[XmlElement("Paths")]
		public Settings.PathSettings Paths = new Settings.PathSettings(Utilities.Settings.SettingsType.Explicit);

		[XmlElement("MainWindow")]
		public Gui.Settings.MainWindowSettings MainWindow = new Gui.Settings.MainWindowSettings(Utilities.Settings.SettingsType.Implicit);

		[XmlElement("NewTerminal")]
		public Gui.Settings.NewTerminalSettings NewTerminal = new Gui.Settings.NewTerminalSettings(Utilities.Settings.SettingsType.Implicit);

		[XmlElement("RecentFiles")]
		public Gui.Settings.RecentFileSettings RecentFiles = new Gui.Settings.RecentFileSettings(Utilities.Settings.SettingsType.Implicit);
	}
}
