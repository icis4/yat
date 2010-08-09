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
// ------------------------------------------------------------------------------------------------
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:maettu_this@users.sourceforge.net.
//==================================================================================================

using System.Configuration;

namespace MKY.Utilities.Configuration
{
	/// <summary>
	/// Utilities to select and return a particular configuration among a set of configurations.
	/// </summary>
	public static class Selector
	{
		/// <summary>
		/// Tries the name of the selected configuration.
		/// </summary>
		/// <param name="c">The configuration.</param>
		/// <param name="groupName">Name of the group.</param>
		/// <param name="selectedConfigurationName">Name of the selected configuration.</param>
		public static bool TryGetSelectedConfigurationName(System.Configuration.Configuration c, string groupName, out string selectedConfigurationName)
		{
			ConfigurationSectionGroup selectionGroup = c.GetSectionGroup(groupName);
			if (selectionGroup != null)
			{
				SelectionSection selection = selectionGroup.Sections[SelectionSection.SelectionSectionName] as SelectionSection;
				if (selection != null)
				{
					selectedConfigurationName = selection.SelectedConfigurationName;
					return (true);
				}
			}
			selectedConfigurationName = "";
			return (false);
		}

		/// <summary>
		/// Tries the get selected configuration section.
		/// </summary>
		/// <typeparam name="T">The type of the section to retrieve.</typeparam>
		/// <param name="c">The c.</param>
		/// <param name="groupName">Name of the group.</param>
		/// <param name="selectedConfigurationName">Name of the selected configuration.</param>
		/// <param name="selectedConfiguration">The selected configuration.</param>
		public static bool TryGetSelectedConfigurationSection<T>(System.Configuration.Configuration c, string groupName, string selectedConfigurationName, out T selectedConfiguration)
			where T : ConfigurationSection
		{
			ConfigurationSectionGroup configurationsGroup = c.GetSectionGroup(groupName);
			if (configurationsGroup != null)
			{
				ConfigurationSection configurationBase = configurationsGroup.Sections[selectedConfigurationName];
				if (configurationBase != null)
				{
					T configuration = configurationBase as T;
					if (configuration != null)
					{
						selectedConfiguration = configuration;
						return (true);
					}
				}
			}
			selectedConfiguration = null;
			return (false);
		}

		/// <summary>
		/// Tries the get selected configuration.
		/// </summary>
		/// <typeparam name="T">The type of the section to retrieve.</typeparam>
		/// <param name="c">The c.</param>
		/// <param name="configurationGroupName">Name of the configuration group.</param>
		/// <param name="configurationsGroupName">Name of the configurations group.</param>
		/// <param name="selectedConfiguration">The selected configuration.</param>
		public static bool TryGetSelectedConfiguration<T>(System.Configuration.Configuration c, string configurationGroupName, string configurationsGroupName, out T selectedConfiguration)
			where T : ConfigurationSection
		{
			string selectedConfigurationName;
			if (TryGetSelectedConfigurationName(c, configurationGroupName, out selectedConfigurationName))
			{
				T configuration;
				if (Selector.TryGetSelectedConfigurationSection<T>(c, configurationsGroupName, selectedConfigurationName, out configuration))
				{
					selectedConfiguration = configuration;
					return (true);
				}
			}
			selectedConfiguration = null;
			return (false);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
