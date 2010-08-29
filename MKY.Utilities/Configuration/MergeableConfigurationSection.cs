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

using System;
using System.Configuration;
using System.IO;

namespace MKY.Utilities.Configuration
{
	/// <summary></summary>
	public class MergeableSettingsSection : ConfigurationSection
	{
		/// <summary>
		/// Merges the settings with alternative settings.
		/// </summary>
		/// <param name="settingsToBeMergedWith">The settings to be merged with.</param>
		public virtual void MergeWith(MergeableSettingsSection settingsToBeMergedWith)
		{
			foreach (ConfigurationProperty cpA in this.Properties)
			{
				foreach (ConfigurationProperty cpB in settingsToBeMergedWith.Properties)
				{
					if (cpA.Name == cpB.Name)
					{
						object value = settingsToBeMergedWith[cpB];
						SetPropertyValue(cpA, value, true);
						break;
					}
				}
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
