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
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010 Matthias Kläy.
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
	/// Utilities to provide configuration settings.
	/// </summary>
	/// <remarks>
	/// Debugging this configuration infrastructure may be a bit trickier than normal debugging.
	/// E.g. if the configuration is used to parametrize NUnit test cases, the follow steps need to be taken:
	/// 1. Build the solution
	/// 2. Start NUnit
	/// 3. 'Debug > Attach' Visual Studio to NUnit
	/// 4. Set a breakpoint a the desired location below
	/// 5. Reload the project in NUnit
	///    => Breakpoint is hit
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
