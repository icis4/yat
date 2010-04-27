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

namespace MKY.Test
{
	/// <summary>
	/// This static class contains the mode how test settings should be handled.
	/// Used to either create default settings or load settings from existing files.
	/// </summary>
	public class SettingsSection : ConfigurationSection
	{
		private ConfigurationPropertyCollection properties;

		private ConfigurationProperty settings = new ConfigurationProperty("settings", typeof(SettingsCollection));

		public SettingsSection()
		{
			this.properties = new ConfigurationPropertyCollection();
			this.properties.Add(this.settings);
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get { return (base.Properties); }
		}

		public override bool IsReadOnly()
		{
			return base.IsReadOnly();
		}

		/// <summary>
		/// Gets or sets the settings mode.
		/// </summary>
		/// <value>The settings mode.</value>
		public static SettingsMode Mode
		{
			get { return (staticMode); }
			set { staticMode = value;  }
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
