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
// YAT 2.0 Gamma 2 Development Version 1.99.35
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
	/// Creates the defaults of the overall test configuration file.
	/// </summary>
	public static class TestSettingsDefaults
	{
		/// <summary></summary>
		[STAThread]
		public static void Main()
		{
			// Create the overall configuration object:

			ExeConfigurationFileMap ecfm = new ExeConfigurationFileMap();
			ecfm.ExeConfigFilename = "YAT.Test";
			Configuration overall = ConfigurationManager.OpenMappedExeConfiguration(ecfm, ConfigurationUserLevel.None);
			overall.Sections.Clear();
			overall.SectionGroups.Clear();

			// Add default sections for each test assembly to the overall configuration,
			// and at the same time create dedicated files for each test assembly too:

			CreateDedicatedFilesAndAddAssemblySections // MKY.IO.Ports.Test
			(
				"MKY.IO.Ports.Test",
				MKY.IO.Ports.Test.ConfigurationConstants.ConfigurationGroupName,
				MKY.IO.Ports.Test.ConfigurationConstants.ConfigurationSectionsGroupName,
				new MKY.IO.Ports.Test.ConfigurationSection(), // Dedicated
				new MKY.IO.Ports.Test.ConfigurationSection(), // Overall
				overall
			);

			// MKY.IO.Serial.SerialPort.Test
			// MKY.IO.Serial.Socket.Test
			// MKY.IO.Serial.Usb.Test
			// Don't provide configuration (yet).

			CreateDedicatedFilesAndAddAssemblySections // MKY.IO.Usb.Test
			(
				"MKY.IO.Usb.Test",
				MKY.IO.Usb.Test.ConfigurationConstants.ConfigurationGroupName,
				MKY.IO.Usb.Test.ConfigurationConstants.ConfigurationSectionsGroupName,
				new MKY.IO.Usb.Test.ConfigurationSection(), // Dedicated
				new MKY.IO.Usb.Test.ConfigurationSection(), // Overall
				overall
			);

			CreateDedicatedFilesAndAddAssemblySections // MKY.Net.Test
			(
				"MKY.Net.Test",
				MKY.Net.Test.ConfigurationConstants.ConfigurationGroupName,
				MKY.Net.Test.ConfigurationConstants.ConfigurationSectionsGroupName,
				new MKY.Net.Test.ConfigurationSection(), // Dedicated
				new MKY.Net.Test.ConfigurationSection(), // Overall
				overall
			);

			// MKY.Test
			// MKY.Win32.Test
			// Doesn't provide configuration (yet).

			// MKY.Windows.Forms.Test
			// Do not include as it is a separate .exe project.

			// YAT.Controller.Test
			// YAT.Domain.Test
			// YAT.Gui.Test
			// YAT.Model.Test
			// YAT.Settings.Test
			// Don't provide configuration (yet).

			// YAT.Test
			// Is this .exe project to generate the defaults of the overall test configuration file.

			// Generate the overall configuration file:
			overall.Save(ConfigurationSaveMode.Full, true);

			// Proceed as follows to generate the configuration files:
			//  1. Build and run this project => The files get created.
			//  2. Go to "\YAT\YAT.Test\bin\Debug" and filter for "*.config".
			//  3. Clean the files from unnecessary information:
			//      a) Remove the version information:
			//          > Groups ", Version=..." >> "" >"
			//          > Sections ", Version=..." >> "" />"
			//      b) Remove the following sections:
			//          > "appSettings"
			//          > "configProtectedData"
			//          > "connectionStrings"
			//          > "system.diagnostics"
			//          > "system.windows.forms"
			//  4. Copy the files to the respective "\ConfigurationTemplate" folder.
			//  5. Compare the new file against the former file.
			//  6. Update the effective solution file ".\YAT.Test.config" as required.
			//  7. Update the effective assembly files in e.g. "..\!-TestConfig" as required.
		}

		private static void CreateDedicatedFilesAndAddAssemblySections(string dedicatedFileName, string groupName, string sectionsGroupName, ConfigurationSection dedicatedSection, ConfigurationSection overallSection, Configuration overallConfiguration)
		{
			// Add default sections for the assembly to the dedicated configuration:
			ExeConfigurationFileMap ecfm = new ExeConfigurationFileMap();
			ecfm.ExeConfigFilename = dedicatedFileName;
			Configuration dedicatedConfiguration = ConfigurationManager.OpenMappedExeConfiguration(ecfm, ConfigurationUserLevel.None);
			dedicatedConfiguration.Sections.Clear();
			dedicatedConfiguration.SectionGroups.Clear();
			AddAssemblySections(groupName, sectionsGroupName, dedicatedSection, dedicatedConfiguration);
			dedicatedConfiguration.Save(ConfigurationSaveMode.Full, true);

			// Add default sections for the assembly to the overall configuration:
			AddAssemblySections(groupName, sectionsGroupName, overallSection, overallConfiguration);
		}

		private static void AddAssemblySections(string groupName, string sectionsGroupName, ConfigurationSection section, Configuration configuration)
		{
			configuration.SectionGroups.Add(groupName, new ConfigurationSectionGroup());
			configuration.SectionGroups[groupName].Sections.Add(SelectionSection.SelectionSectionName, new SelectionSection());

			configuration.SectionGroups.Add(sectionsGroupName, new ConfigurationSectionGroup());
			configuration.SectionGroups[sectionsGroupName].Sections.Add("NoDevices", section);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
