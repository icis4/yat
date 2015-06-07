//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.34
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Configuration;

using MKY.Configuration;

namespace YAT.Test
{
	/// <summary>
	/// Creates the defaults of the test settings in the solution directory.
	/// </summary>
	public static class TestSettingsDefaults
	{
		/// <summary></summary>
		[STAThread]
		public static void Main()
		{
			// Open and reset current configuration.
			Configuration c = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			c.Sections.Clear();
			c.SectionGroups.Clear();
			c.AppSettings.Settings.Clear();
			c.ConnectionStrings.ConnectionStrings.Clear();

			// Add default sections for each test assembly.

			// MKY.IO.Ports.Test
			CreateAssemblySections
			(
				c,
				MKY.IO.Ports.Test.SettingsConstants.ConfigurationGroupName,
				MKY.IO.Ports.Test.SettingsConstants.ConfigurationsGroupName,
				new MKY.IO.Ports.Test.SettingsSection()
			);

			// MKY.IO.Serial.Test

			// MKY.IO.Usb.Test
			CreateAssemblySections
			(
				c,
				MKY.IO.Usb.Test.SettingsConstants.ConfigurationGroupName,
				MKY.IO.Usb.Test.SettingsConstants.ConfigurationsGroupName,
				new MKY.IO.Usb.Test.SettingsSection()
			);

			// MKY.Net.Test
			CreateAssemblySections
			(
				c,
				MKY.Net.Test.SettingsConstants.ConfigurationGroupName,
				MKY.Net.Test.SettingsConstants.ConfigurationsGroupName,
				new MKY.Net.Test.SettingsSection()
			);

			// MKY.Test
			// Do not include MKY.Windows.Forms.Test as it is a separate .exe project.

			// YAT.Controller.Test
			// YAT.Domain.Test
			// YAT.Model.Test
			// YAT.Settings.Test

			c.Save(ConfigurationSaveMode.Full, true);
		}

		private static void CreateAssemblySections(Configuration c, string configurationGroupName, string configurationsGroupName, ConfigurationSection settingsSection)
		{
			c.SectionGroups.Add(configurationGroupName, new ConfigurationSectionGroup());
			c.SectionGroups[configurationGroupName].Sections.Add(SelectionSection.SelectionSectionName, new SelectionSection());

			c.SectionGroups.Add(configurationsGroupName, new ConfigurationSectionGroup());
			c.SectionGroups[configurationsGroupName].Sections.Add("Default", settingsSection);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
