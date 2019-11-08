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
#if (DEBUG)
using System.Diagnostics;
#endif
using System.IO;
#if (DEBUG)
using System.Text;
#endif

namespace MKY.Configuration
{
	/// <summary>
	/// For software testing on computers it may be necessary to configure a test case to a given
	/// test environment. For example, a certain test may not be ran on a machine that misses a
	/// certain hardware. Or, a test may need to be configured to access a certain file that is
	/// located at a local path.
	/// This class in conjunction with <see cref="MergeableConfigurationSection"/>, <see cref="SelectionSection"/>
	/// as well as <see cref="Selector"/> provides the infrastructure that is needed to implement
	/// test cases as described above. Basically, this infrastructure features a way to retrieve
	/// settings from a three-tier configuration environment:
	/// (1) Each setting must be hard-coded in a class that derives from <see cref="MergeableConfigurationSection"/>.
	///     This class must contain hard-coded default values for each setting.
	/// (2) The default configuration can be overridden by solution specific defaults in a file
	///     'SolutionName'.config in the solution's root directory. These solution specific defaults
	///     are to be checked in into the source code control system and are the same for all
	///     developers.
	/// (3) The settings may be overridden again by machine specific configuration files
	///     'TestProject'.config that can be located anywhere on the local file system. The location
	///     of each file is resolved via a system variable 'TEST_PROJECT'_CONFIG_FILE.
	///
	/// Example:
	/// 'MKY.IO.Ports.Test.ConfigurationSection' implements settings to select the serial COM ports
	/// used in the test cases. Each setting is implemented as a <see cref="ConfigurationProperty"/>
	/// and consists of the name, the data type and the default value (1).
	/// 'YAT.config' contains a 'MKY.IO.Ports.Test.Configuration' configuration group that defines the
	/// typical values used for YAT testing (2).
	/// 'MKY.IO.Ports.Test.config' located at a certain machine defines which serial COM
	/// ports are available on that machine (3).
	///
	/// Whenever the test suite, e.g. NUnit based, loads the test cases, the configurations are read
	/// and the test cases configured accordingly. The configuration providers may also implement logic
	/// that verifies whether the configuration really make sense on the current machine.
	///
	/// In addition to the mechanism described above, the configuration may contain multiple values
	/// for each setting. Each set of values is collected in a <see cref="ConfigurationSectionGroup"/>.
	/// The desired set can be selected using a <see cref="SelectionSection"/>. It is also possible
	/// that one tier, e.g. (2), defines multiple sets and the next tier selects, e.g. (3), simply
	/// selects out of the sets defined by (2).
	///
	/// Example:
	/// File 'YAT.Test.config' first announces the selector section:
	///     sectionGroup name="MKY.IO.Ports.Test.Configuration" type="System.Configuration.ConfigurationSectionGroup"
	///         section name="Selection" type="MKY.Configuration.SelectionSection, MKY"
	/// Then it lists the available sets of values in a section group:
	///     sectionGroup name="MKY.IO.Ports.Test.Configuration.Configurations" type="System.Configuration.ConfigurationSectionGroup"
	///         section name="NoDevices" type="MKY.IO.Ports.Test.ConfigurationSection, MKY.IO.Ports.Test"
	///         section name="UsingPhysicalDevices" type="MKY.IO.Ports.Test.ConfigurationSection, MKY.IO.Ports.Test"
	///         section name="UsingVirtualDevices" type="MKY.IO.Ports.Test.ConfigurationSection, MKY.IO.Ports.Test"
	/// Finally, the values for each section:
	///     MKY.IO.Ports.Test.Configuration
	///         Selection SelectedConfigurationName="NoDevices"
	///     MKY.IO.Ports.Test.Configuration.Sections
	///         NoDevices PortA="" PortB=""
	///         UsingPhysicalDevices PortA="COM1" PortB="COM2"
	///         UsingVirtualDevices PortA="COM101" PortB="COM102"
	/// These solution defaults may then be partly or completely overridden by the machine specific
	/// configuration file 'MKY.IO.Ports.Test.config':
	///     MKY.IO.Ports.Test.Configuration
	///         Selection SelectedConfigurationName="UsingPhysicalDevices"
	///
	/// Saying hello to StyleCop ;-.
	/// </summary>
	/// <remarks>
	/// Debugging this configuration infrastructure may be a bit trickier than normal debugging.
	/// E.g. if the configuration is used to parameterize NUnit test cases, the follow steps need
	/// to be taken:
	/// 1. Build the solution
	/// 2. Start NUnit
	/// 3. 'Debug > Attach' Visual Studio to NUnit
	/// 4. Set a breakpoint a the desired location below
	/// 5. Reload the project in NUnit
	///    => Breakpoint is hit.
	/// </remarks>
	public static class Provider
	{
		/// <summary>
		/// Tries the open and merge configurations.
		/// </summary>
		/// <typeparam name="T">The type of the section to retrieve.</typeparam>
		/// <param name="selectionGroupName">Name of the selection configuration group.</param>
		/// <param name="sectionsGroupName">Name of the configuration sections group.</param>
		/// <param name="userConfigurationEnvironmentVariableName">Name of the user configuration environment variable.</param>
		/// <param name="resultingConfiguration">The resulting configuration (default, solution, user or merged).</param>
		public static bool TryOpenAndMergeConfigurations<T>(string selectionGroupName, string sectionsGroupName, string userConfigurationEnvironmentVariableName, out T resultingConfiguration)
			where T : MergeableConfigurationSection
		{
			return (TryOpenAndMergeConfigurations(selectionGroupName, sectionsGroupName, null, userConfigurationEnvironmentVariableName, out resultingConfiguration));
		}

		/// <summary>
		/// Tries the open and merge configurations.
		/// </summary>
		/// <typeparam name="T">The type of the section to retrieve.</typeparam>
		/// <param name="selectionGroupName">Name of the selection configuration group.</param>
		/// <param name="sectionsGroupName">Name of the configuration sections group.</param>
		/// <param name="solutionConfigurationFileNameSuffix">An optional suffix to select a dedicated file (e.g. ".Test" for a test configuration file).</param>
		/// <param name="userConfigurationEnvironmentVariableName">Name of the user configuration environment variable.</param>
		/// <param name="resultingConfiguration">The resulting configuration (default, solution, user or merged).</param>
		public static bool TryOpenAndMergeConfigurations<T>(string selectionGroupName, string sectionsGroupName, string solutionConfigurationFileNameSuffix, string userConfigurationEnvironmentVariableName, out T resultingConfiguration)
			where T : MergeableConfigurationSection
		{
			string solutionFilePath = System.AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
		#if (DEBUG)
			StringBuilder sb;
		#endif
			// The configuration section of the selected configuration of the solution configuration file:
			System.Configuration.Configuration solutionConfiguration;
			if (string.IsNullOrEmpty(solutionConfigurationFileNameSuffix))
			{
				// Use the default file:
				solutionConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			}
			else
			{
				// Use of a dedicated file (e.g. dedicated for test configuration):
				string extension = Path.GetExtension(solutionFilePath);
				int indexBeforeExtension = solutionFilePath.Length - extension.Length;
				solutionFilePath = solutionFilePath.Insert(indexBeforeExtension, solutionConfigurationFileNameSuffix);
				ExeConfigurationFileMap ecfm = new ExeConfigurationFileMap();
				ecfm.ExeConfigFilename = solutionFilePath;
				solutionConfiguration = ConfigurationManager.OpenMappedExeConfiguration(ecfm, ConfigurationUserLevel.None);
			}

			if (solutionConfiguration != null)
			{
				T selectedSolutionConfiguration;
				if (Selector.TryGetSelectedConfiguration<T>(solutionConfiguration, selectionGroupName, sectionsGroupName, out selectedSolutionConfiguration))
				{
				#if (DEBUG)
					sb = new StringBuilder();
					sb.Append("Solution test configuration of ");
					sb.Append(selectionGroupName);
					sb.Append(" successfully loaded from ");
					sb.AppendLine();
					sb.AppendLine(solutionConfiguration.FilePath);
					Debug.Write(sb.ToString());
				#endif
					// Override with and/or add user configuration where requested:
					string userFilePath;
					if (EnvironmentEx.TryGetFilePathFromEnvironmentVariableAndVerify(userConfigurationEnvironmentVariableName, out userFilePath))
					{
						ExeConfigurationFileMap ecfm = new ExeConfigurationFileMap();
						ecfm.ExeConfigFilename = userFilePath;
						System.Configuration.Configuration userConfiguration = ConfigurationManager.OpenMappedExeConfiguration(ecfm, ConfigurationUserLevel.None);
						if (userConfiguration != null)
						{
							T selectedUserConfiguration;
							if (Selector.TryGetSelectedConfiguration<T>(userConfiguration, selectionGroupName, sectionsGroupName, out selectedUserConfiguration))
							{
								selectedSolutionConfiguration.MergeWith(selectedUserConfiguration);
							#if (DEBUG)
								sb = new StringBuilder();
								sb.Append("Configuration of ");
								sb.Append(selectionGroupName);
								sb.Append(" successfully merged with user configuration from ");
								sb.AppendLine();
								sb.AppendLine(userConfiguration.FilePath);
								Debug.Write(sb.ToString());
							#endif
							}
						}
					#if (DEBUG)
						else
						{
							sb = new StringBuilder();
							sb.Append("Failed to load user configuration from ");
							sb.AppendLine();
							sb.AppendLine(userFilePath);
							Debug.Write(sb.ToString());
						}
					#endif
					}
					resultingConfiguration = selectedSolutionConfiguration;
					return (true);
				}
			#if (DEBUG)
				else
				{
					sb = new StringBuilder();
					sb.Append("Failed to load solution configuration of ");
					sb.Append(selectionGroupName);
					sb.Append(" from ");
					sb.AppendLine();
					sb.AppendLine(solutionConfiguration.FilePath);
					Debug.Write(sb.ToString());
				}
			#endif
			}
		#if (DEBUG)
			else
			{
				sb = new StringBuilder();
				sb.Append("Failed to load solution configuration from ");
				sb.AppendLine();
				sb.AppendLine(solutionFilePath);
				Debug.Write(sb.ToString());
			}
		#endif
			resultingConfiguration = null;
			return (false);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
