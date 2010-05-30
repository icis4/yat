//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright � 2010 Matthias Kl�y.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Configuration;

using MKY.Utilities;
using MKY.Utilities.Configuration;

namespace MKY.IO.Usb.Test
{
	#region Settings
	//==========================================================================================
	// Settings
	//==========================================================================================

	/// <summary>
	/// Type representing the configuration settings section.
	/// </summary>
	public class SettingsSection : MergeableSettingsSection
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private ConfigurationPropertyCollection properties;

		private ConfigurationProperty serialHidDeviceAIsAvailable = new ConfigurationProperty("SerialHidDeviceAIsAvailable", typeof(bool), false);
		private ConfigurationProperty serialHidDeviceBIsAvailable = new ConfigurationProperty("SerialHidDeviceBIsAvailable", typeof(bool), false);

		private ConfigurationProperty serialHidDeviceA = new ConfigurationProperty("SerialHidDeviceA", typeof(string), "VID:0ABC PID:1234");
		private ConfigurationProperty serialHidDeviceB = new ConfigurationProperty("SerialHidDeviceB", typeof(string), "VID:0ABC PID:1234");

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary>
		/// Initializes a new instance of the <see cref="SettingsSection"/> class.
		/// </summary>
		public SettingsSection()
		{
			this.properties = new ConfigurationPropertyCollection();

			this.properties.Add(this.serialHidDeviceAIsAvailable);
			this.properties.Add(this.serialHidDeviceBIsAvailable);

			this.properties.Add(this.serialHidDeviceA);
			this.properties.Add(this.serialHidDeviceB);
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		protected override ConfigurationPropertyCollection Properties
		{
			get { return (this.properties); }
		}

		/// <summary></summary>
		public virtual bool SerialHidDeviceAIsAvailable
		{
			get { return (bool)this["SerialHidDeviceAIsAvailable"]; }
			set
			{
				AssertNotReadOnly("SerialHidDeviceAIsAvailable");
				this["SerialHidDeviceAIsAvailable"] = value;
			}
		}

		/// <summary></summary>
		public virtual bool SerialHidDeviceBIsAvailable
		{
			get { return (bool)this["SerialHidDeviceBIsAvailable"]; }
			set
			{
				AssertNotReadOnly("SerialHidDeviceBIsAvailable");
				this["SerialHidDeviceBIsAvailable"] = value;
			}
		}

		/// <summary></summary>
		public virtual string SerialHidDeviceA
		{
			get { return (string)this["SerialHidDeviceA"]; }
			set
			{
				AssertNotReadOnly("SerialHidDeviceA");
				this["SerialHidDeviceA"] = value;
			}
		}

		/// <summary></summary>
		public virtual string SerialHidDeviceB
		{
			get { return (string)this["SerialHidDeviceB"]; }
			set
			{
				AssertNotReadOnly("SerialHidDeviceB");
				this["SerialHidDeviceB"] = value;
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

	#endregion

	#region Provider
	//==========================================================================================
	// Provider
	//==========================================================================================

	/// <summary></summary>
	/// <remarks>
	/// Separate class needed to create default settings. To create the defaults, these constants
	/// are needed but the provider below must not be initialized.
	/// </remarks>
	public static class SettingsConstants
	{
		/// <summary></summary>
		public static readonly string ConfigurationGroupName = typeof(SettingsConstants).Namespace + ".Settings";
		/// <summary></summary>
		public static readonly string ConfigurationsGroupName = ConfigurationGroupName + ".Configurations";
		/// <summary></summary>
		public static readonly string UserSettingsEnvironmentVariableName = "MKY_IO_USB_TEST_SETTINGS_FILE";
	}

	/// <summary></summary>
	public static class SettingsProvider
	{
		private static readonly SettingsSection staticSettings = new SettingsSection();

		static SettingsProvider()
		{
			SettingsSection settings;
			if (Provider.TryOpenAndMergeConfigurations<SettingsSection>(SettingsConstants.ConfigurationGroupName, SettingsConstants.ConfigurationsGroupName, SettingsConstants.UserSettingsEnvironmentVariableName, out settings))
				staticSettings = settings;
		}

		/// <summary></summary>
		public static SettingsSection Settings
		{
			get { return (staticSettings); }
		}
	}

	#endregion

	#region Categories
	//==========================================================================================
	// Categories
	//==========================================================================================

	/// <summary></summary>
	public static class SettingsCategoryStrings
	{
		/// <summary></summary>
		public static readonly string SerialHidDeviceAIsAvailable = "USB Ser/HID " + SettingsProvider.Settings.SerialHidDeviceA + " is available";
		/// <summary></summary>
		public static readonly string SerialHidDeviceBIsAvailable = "USB Ser/HID " + SettingsProvider.Settings.SerialHidDeviceB + " is available";
	}

	/// <summary></summary>
	public class SerialHidDeviceAIsAvailableCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public SerialHidDeviceAIsAvailableCategoryAttribute()
			: base(SettingsCategoryStrings.SerialHidDeviceAIsAvailable)
		{
		}
	}

	/// <summary></summary>
	public class SerialHidDeviceBIsAvailableCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public SerialHidDeviceBIsAvailableCategoryAttribute()
			: base(SettingsCategoryStrings.SerialHidDeviceBIsAvailable)
		{
		}
	}

	#endregion
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
