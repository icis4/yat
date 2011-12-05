//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.7
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Configuration;
using System.Diagnostics;
using System.Text;

namespace MKY.Configuration
{
	/// <summary>
	/// For software testing on computers it may be necessary to configure a test case to a given
	/// test environment. For example, a certain test may not be ran on a machine that misses a
	/// certain hardware. Or, a test may need to be configured to access a certain file that is
	/// located at a local path.
	/// This class in conjunction with <see cref="MergeableSettingsSection"/>, <see cref="SelectionSection"/>
	/// as well as <see cref="Selector"/> provides the infrastructure that is needed to implement
	/// test cases as described above. Basically, this infrastructure features a way to retrieve
	/// settings from a three-tier settings environment:
	/// (1) Each setting must be hard-coded in a class that derives from <see cref="MergeableSettingsSection"/>.
	///     This class must contain hard-coded default values for each setting.
	/// (2) The default settings can be overriden by solution specific defaults in a file
	///     'SolutionName'.config in the solution's root directory. These solution specific defaults
	///     are to be checked in into the source code control system and are the same for all developers.
	/// (3) The settings may be overriden again by machine specific configuration files
	///     'TestProject'.config that can be located anywhere on the local file system. The location
	///     of each file is resolved via a system variable 'TEST_PROJECT'_SETTINGS_FILE.
	/// 
	/// Example:
	/// 'MKY.IO.Ports.Test.SettingsSection' implements settings to select the serial COM ports used
	/// in certain test cases. Each setting is implemented as a <see cref="ConfigurationProperty"/>
	/// and consists of the name, the data type and the default value (1).
	/// 'YAT.config' contains a 'MKY.IO.Ports.Test.Settings' configuration group that defines the
	/// typical values used for YAT testing (2).
	/// 'MKY.IO.Ports.Test.Settings.config' located at a certain machine defines which serial COM
	/// ports are available on that machine (3).
	/// 
	/// Whenever the test suite, e.g. NUnit based, loads the test cases, the configurations are read
	/// and the test cases configured accordingly. The settings providers may also implement logic
	/// that verifies whether the settings really make sense on the current machine.
	/// 
	/// In addition to the mechanism described above, the settings may contain multiple values
	/// for each setting. Each set of values is collected in a <see cref="ConfigurationSectionGroup"/>.
	/// The desired set can be selected using a <see cref="SelectionSection"/>. It is also possible
	/// that one tier, e.g. (2), defines multiple sets and the next tier selects, e.g. (3), simply
	/// selects out of the sets defined by (2).
	/// 
	/// Example:
	/// 'YAT.config' first announces the selector section:
	///     sectionGroup name="MKY.IO.Ports.Test.Settings" type="System.Configuration.ConfigurationSectionGroup"
	///         section name="Selection" type="MKY.Configuration.SelectionSection, MKY"
	/// Then it lists the available sets of values in a section group:
	///     sectionGroup name="MKY.IO.Ports.Test.Settings.Configurations" type="System.Configuration.ConfigurationSectionGroup"
	///         section name="NoDevices" type="MKY.IO.Ports.Test.SettingsSection, MKY.IO.Ports.Test"
	///         section name="UsingPhysicalDevices" type="MKY.IO.Ports.Test.SettingsSection, MKY.IO.Ports.Test"
	///         section name="UsingVSPE" type="MKY.IO.Ports.Test.SettingsSection, MKY.IO.Ports.Test"
	/// Finally, the values for each section:
	///     MKY.IO.Ports.Test.Settings
	///         Selection SelectedConfigurationName="NoDevices"
	///     MKY.IO.Ports.Test.Settings.Configurations
	///         NoDevices SerialPortAIsAvailable="false" SerialPortBIsAvailable="false" SerialPortA="COM1" SerialPortB="COM2" SerialPortsAreInterconnected="false"
	///         UsingPhysicalDevices SerialPortAIsAvailable="true" SerialPortBIsAvailable="true" SerialPortA="COM1" SerialPortB="COM2" SerialPortsAreInterconnected="true"
	///         UsingVSPE SerialPortAIsAvailable="true" SerialPortBIsAvailable="true" SerialPortA="COM1" SerialPortB="COM2" SerialPortsAreInterconnected="true"
	/// These solution defaults may then be partly or completely overridden by the machine specific
	/// configuration file 'MKY.IO.Ports.Test.Settings.config':
	///     MKY.IO.Ports.Test.Settings
	///         Selection SelectedConfigurationName="UsingPhysicalDevices"
	/// </summary>
	/// <remarks>
	/// Debugging this configuration infrastructure may be a bit trickier than normal debugging.
	/// E.g. if the configuration is used to parametrize NUnit test cases, the follow steps need to be taken:
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
		/// <param name="configurationGroupName">Name of the configuration group.</param>
		/// <param name="configurationsGroupName">Name of the configurations group.</param>
		/// <param name="userSettingsEnvironmentVariableName">Name of the user settings environment variable.</param>
		/// <param name="mergedSettings">The merged settings.</param>
		public static bool TryOpenAndMergeConfigurations<T>(string configurationGroupName, string configurationsGroupName, string userSettingsEnvironmentVariableName, out T mergedSettings)
			where T : MergeableSettingsSection
		{
#if (DEBUG)
			StringBuilder sb;
#endif
			// The settings section of the selected configuration of the solution configuration file.
			System.Configuration.Configuration solutionConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			if (solutionConfiguration != null)
			{
				T solutionSettings;
				if (Selector.TryGetSelectedConfiguration<T>(solutionConfiguration, configurationGroupName, configurationsGroupName, out solutionSettings))
				{
#if (DEBUG)
					sb = new StringBuilder();
					sb.Append("Solution test configuration of ");
					sb.Append(configurationGroupName);
					sb.Append(" successfully loaded from ");
					sb.AppendLine();
					sb.AppendLine(solutionConfiguration.FilePath);
					Debug.Write(sb.ToString());
#endif
					// Override/add user settings where applicable.
					string userFilePath;
					if (EnvironmentEx.TryGetFilePathFromEnvironmentVariableAndVerify(userSettingsEnvironmentVariableName, out userFilePath))
					{
						ExeConfigurationFileMap ecfm = new ExeConfigurationFileMap();
						ecfm.ExeConfigFilename = userFilePath;
						System.Configuration.Configuration userConfiguration = ConfigurationManager.OpenMappedExeConfiguration(ecfm, ConfigurationUserLevel.None);
						if (userConfiguration != null)
						{
							T userSettings;
							if (Selector.TryGetSelectedConfiguration<T>(userConfiguration, configurationGroupName, configurationsGroupName, out userSettings))
							{
								solutionSettings.MergeWith(userSettings);
#if (DEBUG)
								sb = new StringBuilder();
								sb.Append("Test configuration of ");
								sb.Append(configurationGroupName);
								sb.Append(" successfully merged with user settings from ");
								sb.AppendLine();
								sb.AppendLine(userConfiguration.FilePath);
								Debug.Write(sb.ToString());
#endif
							}
						}
					}
					mergedSettings = solutionSettings;
					return (true);
				}
				else
				{
#if (DEBUG)
					sb = new StringBuilder();
					sb.Append("Failed to load test configuration of ");
					sb.Append(configurationGroupName);
					sb.Append(" from ");
					sb.AppendLine();
					sb.AppendLine(solutionConfiguration.FilePath);
					Debug.Write(sb.ToString());
#endif
				}
			}
			else
			{
#if (DEBUG)
				sb = new StringBuilder();
				sb.Append("Failed to load test configuration of ");
				sb.AppendLine(configurationGroupName);
				Debug.Write(sb.ToString());
#endif
			}
			mergedSettings = null;
			return (false);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
