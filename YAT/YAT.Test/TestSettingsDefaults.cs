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
// YAT 2.0 Gamma 3 Development Version 1.99.53
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
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
			ecfm.ExeConfigFilename = "YAT.Test.config";
			Configuration overall = ConfigurationManager.OpenMappedExeConfiguration(ecfm, ConfigurationUserLevel.None);
			overall.Sections.Clear();
			overall.SectionGroups.Clear();

			// Add default sections for each test assembly to the overall configuration,
			// and at the same time create dedicated files for each test assembly too:

			CreateDedicatedFilesAndAddAssemblySections // MKY.IO.Ports.Test
			(
				"MKY.IO.Ports.Test.config",
				MKY.IO.Ports.Test.ConfigurationConstants.SelectionGroupName,
				MKY.IO.Ports.Test.ConfigurationConstants.SectionsGroupName,
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
				"MKY.IO.Usb.Test.config",
				MKY.IO.Usb.Test.ConfigurationConstants.SelectionGroupName,
				MKY.IO.Usb.Test.ConfigurationConstants.SectionsGroupName,
				new MKY.IO.Usb.Test.ConfigurationSection(), // Dedicated
				new MKY.IO.Usb.Test.ConfigurationSection(), // Overall
				overall
			);

			CreateDedicatedFilesAndAddAssemblySections // MKY.Net.Test
			(
				"MKY.Net.Test.config",
				MKY.Net.Test.ConfigurationConstants.SelectionGroupName,
				MKY.Net.Test.ConfigurationConstants.SectionsGroupName,
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
			// YAT.View.Test
			// YAT.Model.Test
			// YAT.Settings.Test
			// Don't provide configuration (yet).

			// YAT.Test
			// Is this .exe project to generate the defaults of the overall test configuration file.

			// Generate the overall configuration file:
			overall.Save(ConfigurationSaveMode.Full, true);

			// Proceed as follows to generate the configuration files:
			//  1. Active the "Debug Test" configuration.
			//  2. Build and run this project => Template files get created.
			//  3. Go to "\YAT\YAT.Test\bin\Debug" and filter for "*.config".
			//  4. Clean template files from unnecessary information:
			//      a) Remove the following sections:
			//          > "appSettings"
			//          > "configProtectedData"
			//          > "connectionStrings"
			//          > "system.diagnostics"
			//          > "system.windows.forms"
			//      b) Remove the version information:
			//          > In all "<sectionGroup..." remove all content from ", System.Configuration, Version=..." up to the closing quote.
			//            Attention: Files including the assembly information "System.Configuration" result in TypeLoadException's! Why? No clue...
			//          > In all "<section..." remove all content from ", Version=..." up to the very last closing quote.
			//  5. Move template files to the respective "\ConfigurationTemplate" directory.
			//  6. Compare the new template file against the former template file.
			//  7. Update the effective solution file ".\YAT.Test.config" as required. (This is the generic base configuration.)
			//  8. Update the effective assembly files in e.g. "..\!-TestConfig" as required. (This is the user/machine dependent configuration to be merged with.)
		}

		private static void CreateDedicatedFilesAndAddAssemblySections(string dedicatedFileName, string selectionGroupName, string sectionsGroupName, ConfigurationSection dedicatedSection, ConfigurationSection overallSection, Configuration overallConfiguration)
		{
			// Add default sections for the assembly to the dedicated configuration:
			ExeConfigurationFileMap ecfm = new ExeConfigurationFileMap();
			ecfm.ExeConfigFilename = dedicatedFileName;
			Configuration dedicatedConfiguration = ConfigurationManager.OpenMappedExeConfiguration(ecfm, ConfigurationUserLevel.None);
			dedicatedConfiguration.Sections.Clear();
			dedicatedConfiguration.SectionGroups.Clear();
			AddAssemblySections(selectionGroupName, sectionsGroupName, dedicatedSection, dedicatedConfiguration);
			dedicatedConfiguration.Save(ConfigurationSaveMode.Full, true);

			// Add default sections for the assembly to the overall configuration:
			AddAssemblySections(selectionGroupName, sectionsGroupName, overallSection, overallConfiguration);
		}

		private static void AddAssemblySections(string selectionGroupName, string sectionsGroupName, ConfigurationSection section, Configuration configuration)
		{
			configuration.SectionGroups.Add(selectionGroupName, new ConfigurationSectionGroup());
			configuration.SectionGroups[selectionGroupName].Sections.Add(SelectionSection.SelectionSectionName, new SelectionSection());

			configuration.SectionGroups.Add(sectionsGroupName, new ConfigurationSectionGroup());
			configuration.SectionGroups[sectionsGroupName].Sections.Add("Template", section);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
