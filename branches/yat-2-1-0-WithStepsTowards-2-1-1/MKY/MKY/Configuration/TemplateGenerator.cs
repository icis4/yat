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
// MKY Version 1.0.28 Development
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
using System.Diagnostics.CodeAnalysis;

namespace MKY.Configuration
{
	/// <summary>
	/// Utilities to deal with a configuration file.
	/// </summary>
	public static class TemplateGenerator
	{
		/// <summary>
		/// Returns an array of strings containing default instructions to generate template and configuration files.
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "As always, there are exceptions to the rules...")]
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "through", Justification = "As always, there are exceptions to the rules...")]
		[SuppressMessage("Microsoft.Security", "CA2105:ArrayFieldsShouldNotBeReadOnly", Justification = "Nobody will modify this array, don't worry...")]
		public static readonly string[] DefaultInstructions_1through7 =
		{
			@"Proceed as follows to generate template as well as effective configuration files:",
			@"",
			@" 1. Activate and build ""Debug Test"" on ""YAT.Test"".",
			@" 2. Start NUnit and explicitly execute this test case.",
			@"     => Template file gets created.",
			@" 3. Go to ""<Project>\bin\Debug"" and filter for ""*.config"".",
			@"        Note: File may instead be located in",
			@"                  ""<RelatedProject>\bin\Debug"", is that case",
			@"                  go to ""\MKY"" and filter for ""*.config"".",
			@"                  Why? No clue...",
			@" 4. Clean template files from unnecessary information:",
			@"     a) Remove the following sections:",
			@"         > ""appSettings""",
			@"         > ""configProtectedData""",
			@"         > ""connectionStrings""",
			@"         > ""system.diagnostics""",
			@"         > ""system.windows.forms""",
			@"     b) Remove the version information:",
			@"         > In all ""<sectionGroup..."" remove all content from",
			@"            "", System.Configuration, Version=...""",
			@"            up to the closing quote.",
			@"            Note: Files including the assembly information",
			@"                      ""System.Configuration"" result in",
			@"                      'TypeLoadException'! Why? No clue...",
			@"         > In all ""<section..."" remove all content from",
			@"            "", Version=..."" up to the very last closing quote.",
			@" 5. Move template file to ""<Project>\ConfigurationTemplate"".",
			@" 6. Compare the new against the former template file.",
			@" 7. Merge the commentary information into the new file."
		};

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
