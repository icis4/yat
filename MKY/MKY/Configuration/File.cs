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
// MKY Version 1.0.26 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Configuration;

namespace MKY.Configuration
{
	/// <summary>
	/// Utilities to deal with a configuration file.
	/// </summary>
	public static class File
	{
		/// <summary>
		/// Creates an empty configuration for the given file.
		/// </summary>
		/// <param name="filePath">The file name or path of the configuration.</param>
		/// <returns>The created empty configuration.</returns>
		public static System.Configuration.Configuration CreateEmpty(string filePath)
		{
			var fm = new ExeConfigurationFileMap();
			fm.ExeConfigFilename = filePath;

			var c = ConfigurationManager.OpenMappedExeConfiguration(fm, ConfigurationUserLevel.None);
			c.Sections.Clear();
			c.SectionGroups.Clear();
			return (c);
		}

		/// <summary>
		/// Creates a configuration for the given sections and saves it to a file.
		/// </summary>
		/// <param name="filePath">The file name or path of the configuration to save.</param>
		/// <param name="selectionGroupName">The name of the selection group.</param>
		/// <param name="sectionsGroupName">The name of the sections group.</param>
		/// <param name="sectionName">The name of the section.</param>
		/// <param name="section">The section object.</param>
		public static void CreateSectionsAndSave(string filePath, string selectionGroupName, string sectionsGroupName, string sectionName, MergeableConfigurationSection section)
		{
			var c = CreateEmpty(filePath);
			AddSectionGroups(c, selectionGroupName, sectionsGroupName, sectionName, section);
			c.Save(ConfigurationSaveMode.Full, true);
		}

		/// <summary>
		/// Adds the given section groups to the given configuration.
		/// </summary>
		/// <param name="configuration">The file name or path of the configuration to save.</param>
		/// <param name="selectionGroupName">The name of the selection group.</param>
		/// <param name="sectionsGroupName">The name of the sections group.</param>
		/// <param name="sectionName">The name of the section.</param>
		/// <param name="section">The section object.</param>
		public static void AddSectionGroups(System.Configuration.Configuration configuration, string selectionGroupName, string sectionsGroupName, string sectionName, MergeableConfigurationSection section)
		{
			configuration.SectionGroups.Add(selectionGroupName, new ConfigurationSectionGroup());
			configuration.SectionGroups[selectionGroupName].Sections.Add(SelectionSection.SelectionSectionName, new SelectionSection());

			configuration.SectionGroups.Add(sectionsGroupName, new ConfigurationSectionGroup());
			configuration.SectionGroups[sectionsGroupName].Sections.Add(sectionName, section);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
