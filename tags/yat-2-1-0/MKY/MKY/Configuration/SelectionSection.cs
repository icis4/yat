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
// MKY Version 1.0.27
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

namespace MKY.Configuration
{
	/// <summary>
	/// Generic section to handle selection of multiple configurations with a configuration file.
	/// </summary>
	public class SelectionSection : ConfigurationSection
	{
		/// <summary></summary>
		public const string SelectionSectionName = "Selection";

		private ConfigurationPropertyCollection properties;
		private ConfigurationProperty selectedConfigurationName = new ConfigurationProperty("SelectedConfigurationName", typeof(string), "");

		/// <summary>
		/// Initializes a new instance of the <see cref="SelectionSection"/> class.
		/// </summary>
		public SelectionSection()
		{
			this.properties = new ConfigurationPropertyCollection();

			this.properties.Add(this.selectedConfigurationName);
		}

		/// <summary>
		/// Gets the collection of properties.
		/// </summary>
		/// <returns>
		/// The <see cref="T:System.Configuration.ConfigurationPropertyCollection"/> of properties for the element.
		/// </returns>
		protected override ConfigurationPropertyCollection Properties
		{
			get { return (this.properties); }
		}

		/// <summary>
		/// Gets or sets the name of the selected configuration.
		/// </summary>
		/// <value>The name of the selected configuration.</value>
		public virtual string SelectedConfigurationName
		{
			get { return ((string)this["SelectedConfigurationName"]); }
			set
			{
				AssertNotReadOnly("SelectedConfigurationName");
				this["SelectedConfigurationName"] = value;
			}
		}

		private void AssertNotReadOnly(string propertyName)
		{
			if (IsReadOnly())
				throw (new ConfigurationErrorsException("The property " + propertyName + " is read only!"));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
