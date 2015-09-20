//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.14
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;

using MKY.Configuration;

namespace MKY.IO.Usb.Test
{
	#region Section
	//==========================================================================================
	// Section
	//==========================================================================================

	/// <summary>
	/// Type representing the configuration section.
	/// </summary>
	public class ConfigurationSection : MergeableConfigurationSection
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

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
		/// Initializes a new instance of the <see cref="ConfigurationSection"/> class.
		/// </summary>
		public ConfigurationSection()
		{
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
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "DeviceA and DeviceB")]
		public virtual bool SerialHidDeviceAIsAvailable
		{
			get { return ((bool)this["SerialHidDeviceAIsAvailable"]); }
			set
			{
				AssertNotReadOnly("SerialHidDeviceAIsAvailable");
				this["SerialHidDeviceAIsAvailable"] = value;
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "DeviceA and DeviceB")]
		public virtual bool SerialHidDeviceBIsAvailable
		{
			get { return ((bool)this["SerialHidDeviceBIsAvailable"]); }
			set
			{
				AssertNotReadOnly("SerialHidDeviceBIsAvailable");
				this["SerialHidDeviceBIsAvailable"] = value;
			}
		}

		/// <summary></summary>
		public virtual string SerialHidDeviceA
		{
			get { return ((string)this["SerialHidDeviceA"]); }
			set
			{
				AssertNotReadOnly("SerialHidDeviceA");
				this["SerialHidDeviceA"] = value;
			}
		}

		/// <summary></summary>
		public virtual string SerialHidDeviceB
		{
			get { return ((string)this["SerialHidDeviceB"]); }
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
				throw (new ConfigurationErrorsException("The property " + propertyName + " is read only!"));
		}

		#endregion
	}

	#endregion

	#region Provider
	//==========================================================================================
	// Provider
	//==========================================================================================

	/// <remarks>
	/// Separate class needed to create the default configuration. To create the defaults, these
	/// constants are needed but the provider below must not be initialized.
	/// </remarks>
	public static class ConfigurationConstants
	{
		/// <summary></summary>
		public static readonly string ConfigurationGroupName = typeof(ConfigurationConstants).Namespace + ".Configuration";

		/// <summary></summary>
		public static readonly string ConfigurationSectionsGroupName = ConfigurationGroupName + ".Sections";

		/// <summary></summary>
		public static readonly string SolutionConfigurationFileNameSuffix = ".Test";

		/// <summary></summary>
		public static readonly string UserConfigurationEnvironmentVariableName = "MKY_IO_USB_TEST_CONFIG_FILE";
	}

	/// <summary></summary>
	public static class ConfigurationProvider
	{
		private static readonly ConfigurationSection StaticConfiguration = new ConfigurationSection();

		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Configuration needs to be read during creation.")]
		static ConfigurationProvider()
		{
			ConfigurationSection configuration;
			if (Provider.TryOpenAndMergeConfigurations<ConfigurationSection>(ConfigurationConstants.ConfigurationGroupName, ConfigurationConstants.ConfigurationSectionsGroupName, ConfigurationConstants.SolutionConfigurationFileNameSuffix, ConfigurationConstants.UserConfigurationEnvironmentVariableName, out configuration))
				StaticConfiguration = configuration;
		}

		/// <summary></summary>
		public static ConfigurationSection Configuration
		{
			get { return (StaticConfiguration); }
		}
	}

	#endregion

	#region Categories
	//==========================================================================================
	// Categories
	//==========================================================================================

	/// <summary></summary>
	public static class ConfigurationCategoryStrings
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "DeviceA and DeviceB")]
		public static readonly string SerialHidDeviceAIsAvailable = "USB Ser/HID " + ConfigurationProvider.Configuration.SerialHidDeviceA + " is available";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "DeviceA and DeviceB")]
		public static readonly string SerialHidDeviceBIsAvailable = "USB Ser/HID " + ConfigurationProvider.Configuration.SerialHidDeviceB + " is available";
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "DeviceA and DeviceB")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class SerialHidDeviceAIsAvailableCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public SerialHidDeviceAIsAvailableCategoryAttribute()
			: base(ConfigurationCategoryStrings.SerialHidDeviceAIsAvailable)
		{
		}
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "DeviceA and DeviceB")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class SerialHidDeviceBIsAvailableCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public SerialHidDeviceBIsAvailableCategoryAttribute()
			: base(ConfigurationCategoryStrings.SerialHidDeviceBIsAvailable)
		{
		}
	}

	#endregion
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
