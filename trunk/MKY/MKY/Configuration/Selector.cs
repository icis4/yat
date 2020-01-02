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
// Copyright © 2010-2020 Matthias Kläy.
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
	/// Utilities to select and return a particular configuration among a set of configurations.
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
	public static class Selector
	{
		/// <summary>
		/// Tries the name of the selected configuration.
		/// </summary>
		/// <param name="c">The configuration.</param>
		/// <param name="groupName">Name of the group.</param>
		/// <param name="selectedConfigurationName">Name of the selected configuration.</param>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "c", Justification = "Short and obvious.")]
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
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "c", Justification = "Short and obvious.")]
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
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "c", Justification = "Short and obvious.")]
		public static bool TryGetSelectedConfiguration<T>(System.Configuration.Configuration c, string configurationGroupName, string configurationsGroupName, out T selectedConfiguration)
			where T : ConfigurationSection
		{
			string selectedConfigurationName;
			if (TryGetSelectedConfigurationName(c, configurationGroupName, out selectedConfigurationName))
			{
				T configuration;
				if (TryGetSelectedConfigurationSection<T>(c, configurationsGroupName, selectedConfigurationName, out configuration))
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
