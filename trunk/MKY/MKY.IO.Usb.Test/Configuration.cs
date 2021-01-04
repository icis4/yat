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
// MKY Version 1.0.28 Development
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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;

using MKY.Configuration;
using MKY.IO.Usb;

using NUnit.Framework;

#endregion

namespace MKY.IO.Usb.Test
{
	#region Section
	//==============================================================================================
	// Section
	//==============================================================================================

	/// <summary>
	/// Type representing the configuration section.
	/// </summary>
	public class ConfigurationSection : MergeableConfigurationSection
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private ConfigurationPropertyCollection properties; // = null

		private ConfigurationProperty deviceA = new ConfigurationProperty("DeviceA", typeof(string), "VID:0ABC PID:1234 SNR:12345678A");
		private ConfigurationProperty deviceB = new ConfigurationProperty("DeviceB", typeof(string), "VID:0ABC PID:1234 SNR:12345678B");

		private ConfigurationProperty mtSicsDeviceA = new ConfigurationProperty("MTSicsDeviceA", typeof(string), "VID:0EB8 PID:2303 SNR:12345678A");
		private ConfigurationProperty mtSicsDeviceB = new ConfigurationProperty("MTSicsDeviceB", typeof(string), "VID:0EB8 PID:2303 SNR:12345678B");

		private ConfigurationProperty tiLaunchPadDeviceA = new ConfigurationProperty("TILaunchPadDeviceA", typeof(string), "VID:2047 PID:0404 SNR:12345678A");
		private ConfigurationProperty tiLaunchPadDeviceB = new ConfigurationProperty("TILaunchPadDeviceB", typeof(string), "VID:2047 PID:0404 SNR:12345678B");

		#endregion

		#region Auto-Properties
		//==========================================================================================
		// Auto-Properties
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Device is named with a single letter.")]
		public virtual bool DeviceAIsAvailable { get; set; }

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Device is named with a single letter.")]
		public virtual bool DeviceBIsAvailable { get; set; }

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "'MT-SICS' is a name.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Port is named with a single letter.")]
		public virtual bool MTSicsDeviceAIsAvailable { get; set; }

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "'MT-SICS' is a name.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Port is named with a single letter.")]
		public virtual bool MTSicsDeviceBIsAvailable { get; set; }

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Port is named with a single letter.")]
		public virtual bool TILaunchPadDeviceAIsAvailable { get; set; }

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Port is named with a single letter.")]
		public virtual bool TILaunchPadDeviceBIsAvailable { get; set; }

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
			this.properties = new ConfigurationPropertyCollection();

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
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "'MT-SICS' is a name.")]
		public virtual string MTSicsDeviceA
		{
			get { return ((string)this["MTSicsDeviceA"]); }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "'MT-SICS' is a name.")]
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
	//===============================================================================================
	// Provider
	//===============================================================================================

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
			if (Provider.TryOpenAndMergeConfigurations(ConfigurationConstants.SelectionGroupName, ConfigurationConstants.SectionsGroupName, ConfigurationConstants.SolutionConfigurationFileNameSuffix, ConfigurationConstants.UserConfigurationEnvironmentVariableName, out configuration))
			{
				var availableDevices = new SerialHidDeviceCollection();
				availableDevices.FillWithAvailableDevices(true); // Retrieve strings from devices in order to get serial strings.

				// Set which physical items are available on the current machine:

				HidDeviceInfo di;
				if (HidDeviceInfo.TryParseVidPidSerial(configuration.DeviceA, out di))
					configuration.DeviceAIsAvailable = availableDevices.ContainsVidPidSerial(di); // Ignore usage, configuration doesn't contain it!

				if (HidDeviceInfo.TryParseVidPidSerial(configuration.DeviceB, out di))
					configuration.DeviceBIsAvailable = availableDevices.ContainsVidPidSerial(di); // Ignore usage, configuration doesn't contain it!

				if (HidDeviceInfo.TryParseVidPidSerial(configuration.MTSicsDeviceA, out di))
					configuration.MTSicsDeviceAIsAvailable = availableDevices.ContainsVidPidSerial(di); // Ignore usage, configuration doesn't contain it!

				if (HidDeviceInfo.TryParseVidPidSerial(configuration.MTSicsDeviceB, out di))
					configuration.MTSicsDeviceBIsAvailable = availableDevices.ContainsVidPidSerial(di); // Ignore usage, configuration doesn't contain it!

				if (HidDeviceInfo.TryParseVidPidSerial(configuration.TILaunchPadDeviceA, out di))
					configuration.TILaunchPadDeviceAIsAvailable = availableDevices.ContainsVidPidSerial(di); // Ignore usage, configuration doesn't contain it!

				if (HidDeviceInfo.TryParseVidPidSerial(configuration.TILaunchPadDeviceB, out di))
					configuration.TILaunchPadDeviceBIsAvailable = availableDevices.ContainsVidPidSerial(di); // Ignore usage, configuration doesn't contain it!

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
	//===============================================================================================
	// Categories
	//===============================================================================================

	/// <remarks>
	/// Note that NUnit category strings may not contain the following characters as specified
	/// by <see cref="CategoryAttribute"/>: ',' '+' '-' and '!'
	///
	/// Saying hello to StyleCop ;-.
	/// </remarks>
	public static class ConfigurationCategoryStrings
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Device is named with a single letter.")]
		public static readonly string DeviceAIsAvailable = "USB Ser/HID device A '" + ConfigurationProvider.Configuration.DeviceA + "' is " + (ConfigurationProvider.Configuration.DeviceAIsAvailable ? "" : "*NOT* ") + "available" + (ConfigurationProvider.Configuration.DeviceAIsAvailable ? "" : " => FIX OR EXCLUDE"); // Attention, no '!' allowed in NUnit test category strings!

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Device is named with a single letter.")]
		public static readonly string DeviceBIsAvailable = "USB Ser/HID device B '" + ConfigurationProvider.Configuration.DeviceB + "' is " + (ConfigurationProvider.Configuration.DeviceBIsAvailable ? "" : "*NOT* ") + "available" + (ConfigurationProvider.Configuration.DeviceBIsAvailable ? "" : " => FIX OR EXCLUDE"); // Attention, no '!' allowed in NUnit test category strings!

		/// <remarks>
		/// "MT-SICS" would not be a valid NUnit category string as it contains an '-'.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "'MT-SICS' is a name.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Port is named with a single letter.")]
		public static readonly string MTSicsDeviceAIsAvailable = "USB Ser/HID MT SICS device A '" + ConfigurationProvider.Configuration.MTSicsDeviceA + "' is " + (ConfigurationProvider.Configuration.MTSicsDeviceAIsAvailable ? "" : "*NOT* ") + "available" + (ConfigurationProvider.Configuration.MTSicsDeviceAIsAvailable ? "" : " => FIX OR EXCLUDE"); // Attention, no '!' allowed in NUnit test category strings!

		/// <remarks>
		/// "MT-SICS" would not be a valid NUnit category string as it contains an '-'.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "'MT-SICS' is a name.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Port is named with a single letter.")]
		public static readonly string MTSicsDeviceBIsAvailable = "USB Ser/HID MT SICS device B '" + ConfigurationProvider.Configuration.MTSicsDeviceB + "' is " + (ConfigurationProvider.Configuration.MTSicsDeviceBIsAvailable ? "" : "*NOT* ") + "available" + (ConfigurationProvider.Configuration.MTSicsDeviceBIsAvailable ? "" : " because typically configured to Ser/CDC => EXCLUDE");
	////public static readonly string MTSicsDeviceBIsAvailable = "USB Ser/HID MT SICS device B '" + ConfigurationProvider.Configuration.MTSicsDeviceB + "' is " + (ConfigurationProvider.Configuration.MTSicsDeviceBIsAvailable ? "" : "*NOT* ") + "available" + (ConfigurationProvider.Configuration.MTSicsDeviceBIsAvailable ? "" : " => FIX OR EXCLUDE"); // Attention, no '!' allowed in NUnit test category strings!

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Port is named with a single letter.")]
		public static readonly string TILaunchPadDeviceAIsAvailable = "USB Ser/HID TI LaunchPad device A '" + ConfigurationProvider.Configuration.TILaunchPadDeviceA + "' is " + (ConfigurationProvider.Configuration.TILaunchPadDeviceAIsAvailable ? "" : "*NOT* ") + "available" + (ConfigurationProvider.Configuration.TILaunchPadDeviceAIsAvailable ? "" : " => FIX OR EXCLUDE"); // Attention, no '!' allowed in NUnit test category strings!

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Port is named with a single letter.")]
		public static readonly string TILaunchPadDeviceBIsAvailable = "USB Ser/HID TI LaunchPad device B '" + ConfigurationProvider.Configuration.TILaunchPadDeviceB + "' is " + (ConfigurationProvider.Configuration.TILaunchPadDeviceBIsAvailable ? "" : "*NOT* ") + "available" + (ConfigurationProvider.Configuration.TILaunchPadDeviceBIsAvailable ? "" : " because such devices don't work concurrently => EXCLUDE"); // Attention, no '!' allowed in NUnit test category strings!
	////public static readonly string TILaunchPadDeviceBIsAvailable = "USB Ser/HID TI LaunchPad device B '" + ConfigurationProvider.Configuration.TILaunchPadDeviceB + "' is " + (ConfigurationProvider.Configuration.TILaunchPadDeviceBIsAvailable ? "" : "*NOT* ") + "available" + (ConfigurationProvider.Configuration.TILaunchPadDeviceBIsAvailable ? "" : " => FIX OR EXCLUDE"); // Attention, no '!' allowed in NUnit test category strings!
	}

	/// <remarks>
	/// Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Device is named with a single letter.")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class DeviceAIsAvailableCategoryAttribute : CategoryAttribute
	{
		/// <summary></summary>
		public DeviceAIsAvailableCategoryAttribute()
			: base(ConfigurationCategoryStrings.DeviceAIsAvailable)
		{
		}
	}

	/// <remarks>
	/// Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Device is named with a single letter.")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class DeviceBIsAvailableCategoryAttribute : CategoryAttribute
	{
		/// <summary></summary>
		public DeviceBIsAvailableCategoryAttribute()
			: base(ConfigurationCategoryStrings.DeviceBIsAvailable)
		{
		}
	}

	/// <remarks>
	/// Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "'MT-SICS' is a name.")]
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Port is named with a single letter.")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class MTSicsDeviceAIsAvailableCategoryAttribute : CategoryAttribute
	{
		/// <summary></summary>
		public MTSicsDeviceAIsAvailableCategoryAttribute()
			: base(ConfigurationCategoryStrings.MTSicsDeviceAIsAvailable)
		{
		}
	}

	/// <remarks>
	/// Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "'MT-SICS' is a name.")]
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Port is named with a single letter.")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class MTSicsDeviceBIsAvailableCategoryAttribute : CategoryAttribute
	{
		/// <summary></summary>
		public MTSicsDeviceBIsAvailableCategoryAttribute()
			: base(ConfigurationCategoryStrings.MTSicsDeviceBIsAvailable)
		{
		}
	}

	/// <remarks>
	/// Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Port is named with a single letter.")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class TILaunchPadDeviceAIsAvailableCategoryAttribute : CategoryAttribute
	{
		/// <summary></summary>
		public TILaunchPadDeviceAIsAvailableCategoryAttribute()
			: base(ConfigurationCategoryStrings.TILaunchPadDeviceAIsAvailable)
		{
		}
	}

	/// <remarks>
	/// Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Port is named with a single letter.")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class TILaunchPadDeviceBIsAvailableCategoryAttribute : CategoryAttribute
	{
		/// <summary></summary>
		public TILaunchPadDeviceBIsAvailableCategoryAttribute()
			: base(ConfigurationCategoryStrings.TILaunchPadDeviceBIsAvailable)
		{
		}
	}

	#endregion

	#region Self-Tests
	//==============================================================================================
	// Self-Tests
	//==============================================================================================

	/// <summary></summary>
	[TestFixture]
	public class CategoryTest
	{
		/// <summary>
		/// This self-test ensures that the given categories are instantiated at least once,
		/// such the tester for sure gets informed in case an infrastructure is not available.
		/// </summary>
		/// <remarks>
		/// The below code also serves as a template for tests that require this infrastructure
		/// and shall probe for it during test execution. Alternatively, tests can apply the
		/// category attribute to the test and can then get excluded by the tester.
		/// </remarks>
		[Test, DeviceAIsAvailableCategory, DeviceBIsAvailableCategory]
		public virtual void TestDeviceIsAvailableCategories()
		{
			if (!ConfigurationProvider.Configuration.DeviceAIsAvailable)
				Assert.Ignore("'DeviceA' is not available, therefore this test is excluded. Ensure that 'DeviceA' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			if (!ConfigurationProvider.Configuration.DeviceBIsAvailable)
				Assert.Ignore("'DeviceB' is not available, therefore this test is excluded. Ensure that 'DeviceB' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.
		}

		/// <summary>
		/// This self-test ensures that the given categories are instantiated at least once,
		/// such the tester for sure gets informed in case an infrastructure is not available.
		/// </summary>
		/// <remarks>
		/// The below code also serves as a template for tests that require this infrastructure
		/// and shall probe for it during test execution. Alternatively, tests can apply the
		/// category attribute to the test and can then get excluded by the tester.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "'MT-SICS' is a name.")]
		[Test, MTSicsDeviceAIsAvailableCategory, MTSicsDeviceBIsAvailableCategory]
		public virtual void TestMTSicsDeviceIsAvailableCategories()
		{
			if (!ConfigurationProvider.Configuration.MTSicsDeviceAIsAvailable)
				Assert.Ignore("'MTSicsDeviceA' is not available, therefore this test is excluded. Ensure that 'MTSicsDeviceA' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			if (!ConfigurationProvider.Configuration.MTSicsDeviceBIsAvailable)
				Assert.Ignore("'MTSicsDeviceB' is not available, therefore this test is excluded. Ensure that 'MTSicsDeviceB' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.
		}

		/// <summary>
		/// This self-test ensures that the given categories are instantiated at least once,
		/// such the tester for sure gets informed in case an infrastructure is not available.
		/// </summary>
		/// <remarks>
		/// The below code also serves as a template for tests that require this infrastructure
		/// and shall probe for it during test execution. Alternatively, tests can apply the
		/// category attribute to the test and can then get excluded by the tester.
		/// </remarks>
		[Test, TILaunchPadDeviceAIsAvailableCategory, TILaunchPadDeviceBIsAvailableCategory]
		public virtual void TestTILaunchPadDeviceIsAvailableCategories()
		{
			if (!ConfigurationProvider.Configuration.TILaunchPadDeviceAIsAvailable)
				Assert.Ignore("'TILaunchPadDeviceA' is not available, therefore this test is excluded. Ensure that 'TILaunchPadDeviceA' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			if (!ConfigurationProvider.Configuration.TILaunchPadDeviceBIsAvailable)
				Assert.Ignore("'TILaunchPadDeviceB' is not available, therefore this test is excluded. Ensure that 'TILaunchPadDeviceB' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.
		}
	}

	#endregion
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
