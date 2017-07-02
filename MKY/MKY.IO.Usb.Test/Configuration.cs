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
// MKY Version 1.0.19
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2017 Matthias Kläy.
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

		private ConfigurationProperty deviceA = new ConfigurationProperty("DeviceA", typeof(string), "VID:0ABC PID:1234 SNR:XYZ");
		private ConfigurationProperty deviceB = new ConfigurationProperty("DeviceB", typeof(string), "VID:0ABC PID:1234 SNR:XYZ");

		private ConfigurationProperty mtSicsDeviceA = new ConfigurationProperty("MTSicsDeviceA", typeof(string), "VID:0ABC PID:1234 SNR:XYZ");
		private ConfigurationProperty mtSicsDeviceB = new ConfigurationProperty("MTSicsDeviceB", typeof(string), "VID:0ABC PID:1234 SNR:XYZ");

		private ConfigurationProperty tiLaunchPadDeviceA = new ConfigurationProperty("TILaunchPadDeviceA", typeof(string), "VID:0ABC PID:1234 SNR:XYZ");
		private ConfigurationProperty tiLaunchPadDeviceB = new ConfigurationProperty("TILaunchPadDeviceB", typeof(string), "VID:0ABC PID:1234 SNR:XYZ");

		#endregion

		#region Auto-Properties
		//==========================================================================================
		// Auto-Properties
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Device is named with a single letter")]
		public virtual bool DeviceAIsAvailable { get; set; }

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Device is named with a single letter")]
		public virtual bool DeviceBIsAvailable { get; set; }

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "MT-SICS is a name.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Port is named with a single letter.")]
		public virtual bool MTSicsDeviceAIsConnected { get; set; }

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "MT-SICS is a name.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Port is named with a single letter.")]
		public virtual bool MTSicsDeviceBIsConnected { get; set; }

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Port is named with a single letter.")]
		public virtual bool TILaunchPadDeviceAIsConnected { get; set; }

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Port is named with a single letter.")]
		public virtual bool TILaunchPadDeviceBIsConnected { get; set; }

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
			this.properties.Add(this.deviceA);
			this.properties.Add(this.deviceB);

			this.properties.Add(this.mtSicsDeviceA);
			this.properties.Add(this.mtSicsDeviceB);

			this.properties.Add(this.tiLaunchPadDeviceA);
			this.properties.Add(this.tiLaunchPadDeviceB);
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
		public virtual string DeviceA
		{
			get { return ((string)this["DeviceA"]); }
		}

		/// <summary></summary>
		public virtual string DeviceB
		{
			get { return ((string)this["DeviceB"]); }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "MT-SICS is a name.")]
		public virtual string MTSicsDeviceA
		{
			get { return ((string)this["MTSicsDeviceA"]); }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "MT-SICS is a name.")]
		public virtual string MTSicsDeviceB
		{
			get { return ((string)this["MTSicsDeviceB"]); }
		}

		/// <summary></summary>
		public virtual string TILaunchPadDeviceA
		{
			get { return ((string)this["TILaunchPadDeviceA"]); }
		}

		/// <summary></summary>
		public virtual string TILaunchPadDeviceB
		{
			get { return ((string)this["TILaunchPadDeviceB"]); }
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
		public static readonly string SelectionGroupName = typeof(ConfigurationConstants).Namespace + ".Configuration"; // Simply use the generic <Namespace>.Configuration identifier.

		/// <summary></summary>
		public static readonly string SectionsGroupName = SelectionGroupName + ".Sections"; // Just add to the generic identifier from above.

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
			if (Provider.TryOpenAndMergeConfigurations<ConfigurationSection>(ConfigurationConstants.SelectionGroupName, ConfigurationConstants.SectionsGroupName, ConfigurationConstants.SolutionConfigurationFileNameSuffix, ConfigurationConstants.UserConfigurationEnvironmentVariableName, out configuration))
			{
				// Set which physical items are available on the current machine:
				SerialHidDeviceCollection availableDevices = new SerialHidDeviceCollection();
				availableDevices.FillWithAvailableDevices(true); // Retrieve strings from devices in order to get serial strings.

				DeviceInfo di;
				if (DeviceInfo.TryParseFromVidAndPidAndSerial(configuration.DeviceA, out di))
					configuration.DeviceAIsAvailable = availableDevices.Contains(di);

				if (DeviceInfo.TryParseFromVidAndPidAndSerial(configuration.DeviceB, out di))
					configuration.DeviceBIsAvailable = availableDevices.Contains(di);

				if (DeviceInfo.TryParseFromVidAndPidAndSerial(configuration.MTSicsDeviceA, out di))
					configuration.MTSicsDeviceAIsConnected = availableDevices.Contains(di);

				if (DeviceInfo.TryParseFromVidAndPidAndSerial(configuration.MTSicsDeviceB, out di))
					configuration.MTSicsDeviceBIsConnected = availableDevices.Contains(di);

				if (DeviceInfo.TryParseFromVidAndPidAndSerial(configuration.TILaunchPadDeviceA, out di))
					configuration.TILaunchPadDeviceAIsConnected = availableDevices.Contains(di);

				if (DeviceInfo.TryParseFromVidAndPidAndSerial(configuration.TILaunchPadDeviceB, out di))
					configuration.TILaunchPadDeviceBIsConnected = availableDevices.Contains(di);

				// Activate the effective configuration:
				StaticConfiguration = configuration;
			}
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

	/// <remarks>
	/// Note that NUnit category strings may not contain the following characters as specified
	/// by <see cref="NUnit.Framework.CategoryAttribute"/>: ',' '+' '-' and '!'
	/// 
	/// Saying hello to StyleCop ;-.
	/// </remarks>
	public static class ConfigurationCategoryStrings
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Device is named with a single letter")]
		public static readonly string DeviceAIsAvailable = "USB Ser/HID device A '" + ConfigurationProvider.Configuration.DeviceA + "' is " + (ConfigurationProvider.Configuration.DeviceAIsAvailable ? "" : "*NOT* ") + "available" + (ConfigurationProvider.Configuration.DeviceAIsAvailable ? "" : " => FIX OR EXCLUDE"); // Attention, no '!' allowed in NUnit test category strings!

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Device is named with a single letter")]
		public static readonly string DeviceBIsAvailable = "USB Ser/HID device B '" + ConfigurationProvider.Configuration.DeviceB + "' is " + (ConfigurationProvider.Configuration.DeviceBIsAvailable ? "" : "*NOT* ") + "available" + (ConfigurationProvider.Configuration.DeviceBIsAvailable ? "" : " => FIX OR EXCLUDE"); // Attention, no '!' allowed in NUnit test category strings!

		/// <remarks>"MT-SICS" is no valid NUnit category string as it contains an '-'.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "MT-SICS is a name.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Port is named with a single letter.")]
		public static readonly string MTSicsDeviceAIsConnected = "USB Ser/HID MT SICS device A '" + ConfigurationProvider.Configuration.MTSicsDeviceA + "' is " + (ConfigurationProvider.Configuration.MTSicsDeviceAIsConnected ? "" : "*NOT* ") + "connected" + (ConfigurationProvider.Configuration.MTSicsDeviceAIsConnected ? "" : " => FIX OR EXCLUDE"); // Attention, no '!' allowed in NUnit test category strings!

		/// <remarks>"MT-SICS" is no valid NUnit category string as it contains an '-'.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "MT-SICS is a name.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Port is named with a single letter.")]
		public static readonly string MTSicsDeviceBIsConnected = "USB Ser/HID MT SICS device B '" + ConfigurationProvider.Configuration.MTSicsDeviceB + "' is " + (ConfigurationProvider.Configuration.MTSicsDeviceBIsConnected ? "" : "*NOT* ") + "connected" + (ConfigurationProvider.Configuration.MTSicsDeviceBIsConnected ? "" : " => FIX OR EXCLUDE"); // Attention, no '!' allowed in NUnit test category strings!

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Port is named with a single letter.")]
		public static readonly string TILaunchPadDeviceAIsConnected = "USB Ser/HID TI LaunchPad device A '" + ConfigurationProvider.Configuration.TILaunchPadDeviceA + "' is " + (ConfigurationProvider.Configuration.TILaunchPadDeviceAIsConnected ? "" : "*NOT* ") + "connected" + (ConfigurationProvider.Configuration.TILaunchPadDeviceAIsConnected ? "" : " => FIX OR EXCLUDE"); // Attention, no '!' allowed in NUnit test category strings!

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Port is named with a single letter.")]
		public static readonly string TILaunchPadDeviceBIsConnected = "USB Ser/HID TI LaunchPad device B '" + ConfigurationProvider.Configuration.TILaunchPadDeviceB + "' is " + (ConfigurationProvider.Configuration.TILaunchPadDeviceBIsConnected ? "" : "*NOT* ") + "connected" + (ConfigurationProvider.Configuration.TILaunchPadDeviceBIsConnected ? "" : " => FIX OR EXCLUDE"); // Attention, no '!' allowed in NUnit test category strings!
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Device is named with a single letter")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class DeviceAIsAvailableCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public DeviceAIsAvailableCategoryAttribute()
			: base(ConfigurationCategoryStrings.DeviceAIsAvailable)
		{
		}
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Device is named with a single letter")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class DeviceBIsAvailableCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public DeviceBIsAvailableCategoryAttribute()
			: base(ConfigurationCategoryStrings.DeviceBIsAvailable)
		{
		}
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "MT-SICS is a name.")]
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Port is named with a single letter.")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class MTSicsDeviceAIsConnectedCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public MTSicsDeviceAIsConnectedCategoryAttribute()
			: base(ConfigurationCategoryStrings.MTSicsDeviceAIsConnected)
		{
		}
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "MT-SICS is a name.")]
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Port is named with a single letter.")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class MTSicsDeviceBIsConnectedCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public MTSicsDeviceBIsConnectedCategoryAttribute()
			: base(ConfigurationCategoryStrings.MTSicsDeviceBIsConnected)
		{
		}
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Port is named with a single letter.")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class TILaunchPadDeviceAIsConnectedCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public TILaunchPadDeviceAIsConnectedCategoryAttribute()
			: base(ConfigurationCategoryStrings.TILaunchPadDeviceAIsConnected)
		{
		}
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Port is named with a single letter.")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class TILaunchPadDeviceBIsConnectedCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public TILaunchPadDeviceBIsConnectedCategoryAttribute()
			: base(ConfigurationCategoryStrings.TILaunchPadDeviceBIsConnected)
		{
		}
	}

	#endregion
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
