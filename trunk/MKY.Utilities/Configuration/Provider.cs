//==================================================================================================
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

namespace MKY.Utilities.Configuration
{
	/// <summary>
	/// Utilities to provide configuration settings.
	/// </summary>
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
			// The settings section of the selected configuration of the solution configuration file.
			System.Configuration.Configuration solutionConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			if (solutionConfiguration != null)
			{
				T solutionSettings;
				if (Selector.TryGetSelectedConfiguration<T>(solutionConfiguration, configurationGroupName, configurationsGroupName, out solutionSettings))
				{
					StringBuilder sb = new StringBuilder();
					sb.Append("Solution test configuration of ");
					sb.Append(configurationGroupName);
					sb.Append(" successfully loaded from ");
					sb.AppendLine();
					sb.AppendLine(solutionConfiguration.FilePath);
					Debug.Write(sb.ToString());

					// Override/add user settings where applicable.
					string userFilePath;
					if (XEnvironment.TryGetFilePathFromEnvironmentVariableAndVerify(userSettingsEnvironmentVariableName, out userFilePath))
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

								sb.Append("Test configuration of ");
								sb.Append(configurationGroupName);
								sb.Append(" successfully merged with user settings from ");
								sb.AppendLine();
								sb.AppendLine(userConfiguration.FilePath);
								Debug.Write(sb.ToString());
							}
						}
					}
					mergedSettings = solutionSettings;
					return (true);
				}
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
