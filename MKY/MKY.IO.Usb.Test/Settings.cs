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
// MKY Development Version 1.0.8
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2012 Matthias Kläy.
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
		/// Initializes a new instance of the <see cref="SettingsSection"/> class.
		/// </summary>
		public SettingsSection()
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
			get { return (bool)this["SerialHidDeviceAIsAvailable"]; }
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

		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Settings need to be read during creation.")]
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
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "DeviceA and DeviceB")]
		public static readonly string SerialHidDeviceAIsAvailable = "USB Ser/HID " + SettingsProvider.Settings.SerialHidDeviceA + " is available";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "DeviceA and DeviceB")]
		public static readonly string SerialHidDeviceBIsAvailable = "USB Ser/HID " + SettingsProvider.Settings.SerialHidDeviceB + " is available";
	}

	/// <summary></summary>
	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "DeviceA and DeviceB")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class SerialHidDeviceAIsAvailableCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public SerialHidDeviceAIsAvailableCategoryAttribute()
			: base(SettingsCategoryStrings.SerialHidDeviceAIsAvailable)
		{
		}
	}

	/// <summary></summary>
	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "DeviceA and DeviceB")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class SerialHidDeviceBIsAvailableCategoryAttribute : NUnit.Framework.CategoryAttribute
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
