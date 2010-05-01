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

namespace MKY.Utilities.Configuration
{
	/// <summary>
	/// Generic section to handle selection of multiple configurations with a configuration file.
	/// </summary>
	public class SelectionSection : ConfigurationSection
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <summary></summary>
		public const string SelectionSectionName = "Selection";

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private ConfigurationPropertyCollection properties;

		private ConfigurationProperty selectedConfigurationName = new ConfigurationProperty("SelectedConfigurationName", typeof(string), "");

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary>
		/// Initializes a new instance of the <see cref="SelectionSection"/> class.
		/// </summary>
		public SelectionSection()
		{
			this.properties = new ConfigurationPropertyCollection();

			this.properties.Add(this.selectedConfigurationName);
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

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
			get { return (string)this["SelectedConfigurationName"]; }
			set
			{
				AssertNotReadOnly("SelectedConfigurationName");
				this["SelectedConfigurationName"] = value;
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		private void AssertNotReadOnly(string propertyName)
		{
			if (IsReadOnly())
				throw (new ConfigurationErrorsException("The property " + propertyName + " is read only."));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
