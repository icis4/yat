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

		#region Fields > Configuration
		//------------------------------------------------------------------------------------------
		// Fields > Configuration
		//------------------------------------------------------------------------------------------

		private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

		private ConfigurationProperty deviceA = new ConfigurationProperty("DeviceA", typeof(string), "VID:0ABC PID:1234");
		private ConfigurationProperty deviceB = new ConfigurationProperty("DeviceB", typeof(string), "VID:0ABC PID:1234");

		private ConfigurationProperty mtSicsDeviceA = new ConfigurationProperty("MTSicsDeviceA", typeof(string), "VID:0ABC PID:1234");
		private ConfigurationProperty mtSicsDeviceB = new ConfigurationProperty("MTSicsDeviceB", typeof(string), "VID:0ABC PID:1234");

		private ConfigurationProperty tiLaunchPadDeviceA = new ConfigurationProperty("TILaunchPadDeviceA", typeof(string), "VID:0ABC PID:1234");
		private ConfigurationProperty tiLaunchPadDeviceB = new ConfigurationProperty("TILaunchPadDeviceB", typeof(string), "VID:0ABC PID:1234");

		#endregion

		#region Fields > Auxiliary
		//------------------------------------------------------------------------------------------
		// Fields > Auxiliary
		//------------------------------------------------------------------------------------------

		private bool deviceAIsAvailable;
		private bool deviceBIsAvailable;

		private bool mtSicsDeviceAIsConnected;
		private bool mtSicsDeviceBIsConnected;

		private bool tiLaunchPadDeviceAIsConnected;
		private bool tiLaunchPadDeviceBIsConnected;

		#endregion

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

		#region Properties > Configuration
		//------------------------------------------------------------------------------------------
		// Properties > Configuration
		//------------------------------------------------------------------------------------------

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
		public virtual string MTSicsDeviceA
		{
			get { return ((string)this["MTSicsDeviceA"]); }
		}

		/// <summary></summary>
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

		#region Properties > Auxiliary
		//------------------------------------------------------------------------------------------
		// Properties > Auxiliary
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Device is named with a single letter")]
		public virtual bool DeviceAIsAvailable
		{
			get { return (this.deviceAIsAvailable); }
			set { this.deviceAIsAvailable = value;  }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Device is named with a single letter")]
		public virtual bool DeviceBIsAvailable
		{
			get { return (this.deviceBIsAvailable); }
			set { this.deviceBIsAvailable = value;  }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Port is named with a single letter")]
		public virtual bool MTSicsDeviceAIsConnected
		{
			get { return (this.mtSicsDeviceAIsConnected); }
			set { this.mtSicsDeviceAIsConnected = value; }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Port is named with a single letter")]
		public virtual bool MTSicsDeviceBIsConnected
		{
			get { return (this.mtSicsDeviceBIsConnected); }
			set { this.mtSicsDeviceBIsConnected = value; }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Port is named with a single letter")]
		public virtual bool TILaunchPadDeviceAIsConnected
		{
			get { return (this.tiLaunchPadDeviceAIsConnected); }
			set { this.tiLaunchPadDeviceAIsConnected = value; }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Port is named with a single letter")]
		public virtual bool TILaunchPadDeviceBIsConnected
		{
			get { return (this.tiLaunchPadDeviceBIsConnected); }
			set { this.tiLaunchPadDeviceBIsConnected = value; }
		}

		#endregion

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
				availableDevices.FillWithAvailableDevices();

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
		public static readonly string DeviceAIsAvailable = "USB Ser/HID device A '" + ConfigurationProvider.Configuration.DeviceA + "' is " + (ConfigurationProvider.Configuration.DeviceAIsAvailable ? "" : "not ") + "available";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Device is named with a single letter")]
		public static readonly string DeviceBIsAvailable = "USB Ser/HID device B '" + ConfigurationProvider.Configuration.DeviceB + "' is " + (ConfigurationProvider.Configuration.DeviceBIsAvailable ? "" : "not ") + "available";

		/// <remarks>"MT-SICS" is no valid NUnit category string as it contains an '-'.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Port is named with a single letter")]
		public static readonly string MTSicsDeviceAIsConnected = "USB Ser/HID MT SICS device A '" + ConfigurationProvider.Configuration.MTSicsDeviceA + "' is " + (ConfigurationProvider.Configuration.MTSicsDeviceAIsConnected ? "" : "not ") + "connected";

		/// <remarks>"MT-SICS" is no valid NUnit category string as it contains an '-'.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Port is named with a single letter")]
		public static readonly string MTSicsDeviceBIsConnected = "USB Ser/HID MT SICS device B '" + ConfigurationProvider.Configuration.MTSicsDeviceB + "' is " + (ConfigurationProvider.Configuration.MTSicsDeviceBIsConnected ? "" : "not ") + "connected";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Port is named with a single letter")]
		public static readonly string TILaunchPadDeviceAIsConnected = "USB Ser/HID TI LaunchPad device A '" + ConfigurationProvider.Configuration.TILaunchPadDeviceA + "' is " + (ConfigurationProvider.Configuration.TILaunchPadDeviceAIsConnected ? "" : "not ") + "connected";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Port is named with a single letter")]
		public static readonly string TILaunchPadDeviceBIsConnected = "USB Ser/HID TI LaunchPad device B '" + ConfigurationProvider.Configuration.TILaunchPadDeviceB + "' is " + (ConfigurationProvider.Configuration.TILaunchPadDeviceBIsConnected ? "" : "not ") + "connected";
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
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Port is named with a single letter")]
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
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Port is named with a single letter")]
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
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Port is named with a single letter")]
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
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Port is named with a single letter")]
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
