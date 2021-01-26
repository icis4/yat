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
// MKY Version 1.0.29
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Configuration;
using System.IO;

namespace MKY.Configuration
{
	/// <summary></summary>
	public class MergeableConfigurationSection : ConfigurationSection
	{
		/// <summary>
		/// Merges the configuration with alternative configurations.
		/// </summary>
		/// <param name="configurationsToBeMergedWith">The configurations to be merged with.</param>
		public virtual void MergeWith(MergeableConfigurationSection configurationsToBeMergedWith)
		{
			foreach (ConfigurationProperty cpA in this.Properties)
			{
				foreach (ConfigurationProperty cpB in configurationsToBeMergedWith.Properties)
				{
					if (StringEx.EqualsOrdinalIgnoreCase(cpA.Name, cpB.Name))
					{
						object value = configurationsToBeMergedWith[cpB];
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
