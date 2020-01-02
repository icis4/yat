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
// Copyright © 2010-2020 Matthias Kläy.
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
using MKY.Test.Devices;

using NUnit.Framework;

#endregion

namespace MKY.Test
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

		private ConfigurationProperty usbHub1 = new ConfigurationProperty("USBHub1", typeof(string), "ABCD1234");
		private ConfigurationProperty usbHub2 = new ConfigurationProperty("USBHub2", typeof(string), "ABCD1234");

		#endregion

		#region Auto-Properties
		//==========================================================================================
		// Auto-Properties
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "1Is", Justification = "Device is named with a single digit.")]
		public virtual bool UsbHub1IsAvailable { get; set; }

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "2Is", Justification = "Device is named with a single digit.")]
		public virtual bool UsbHub2IsAvailable { get; set; }

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

			this.properties.Add(this.usbHub1);
			this.properties.Add(this.usbHub2);
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
		public virtual string UsbHub1
		{
			get { return ((string)this["USBHub1"]); }
		}

		/// <summary></summary>
		public virtual string UsbHub2
		{
			get { return ((string)this["USBHub2"]); }
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
		public static readonly string UserConfigurationEnvironmentVariableName = "MKY_TEST_CONFIG_FILE";
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
				// Set which physical items are available on the current machine:
				if (UsbHubControl.ExecutableIsAvailable)
				{
					configuration.UsbHub1IsAvailable = UsbHubControl.Probe(configuration.UsbHub1);
					configuration.UsbHub2IsAvailable = UsbHubControl.Probe(configuration.UsbHub2);
				}

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
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "1Is", Justification = "Device is named with a single digit.")]
		public static readonly string UsbHub1IsAvailable = "USB hub 1 '" + ConfigurationProvider.Configuration.UsbHub1 + "' is " + (ConfigurationProvider.Configuration.UsbHub1IsAvailable ? "" : "*NOT* ") + "available" + (ConfigurationProvider.Configuration.UsbHub1IsAvailable ? "" : " => FIX OR EXCLUDE"); // Attention, no '!' allowed in NUnit test category strings!

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "2Is", Justification = "Device is named with a single digit.")]
		public static readonly string UsbHub2IsAvailable = "USB hub 2 '" + ConfigurationProvider.Configuration.UsbHub2 + "' is " + (ConfigurationProvider.Configuration.UsbHub2IsAvailable ? "" : "*NOT* ") + "available" + (ConfigurationProvider.Configuration.UsbHub2IsAvailable ? "" : " => FIX OR EXCLUDE"); // Attention, no '!' allowed in NUnit test category strings!
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "1Is", Justification = "Device is named with a single digit.")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class UsbHub1IsAvailableCategoryAttribute : CategoryAttribute
	{
		/// <summary></summary>
		public UsbHub1IsAvailableCategoryAttribute()
			: base(ConfigurationCategoryStrings.UsbHub1IsAvailable)
		{
		}
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "2Is", Justification = "Device is named with a single digit.")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class UsbHub2IsAvailableCategoryAttribute : CategoryAttribute
	{
		/// <summary></summary>
		public UsbHub2IsAvailableCategoryAttribute()
			: base(ConfigurationCategoryStrings.UsbHub2IsAvailable)
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
		[Test, UsbHub1IsAvailableCategory, UsbHub2IsAvailableCategory]
		public virtual void TestUsbHubIsAvailableCategories()
		{
			if (!ConfigurationProvider.Configuration.UsbHub1IsAvailable)
				Assert.Ignore("'USBHub1' is not available, therefore this test is excluded. Ensure that 'USBHub1' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			if (!ConfigurationProvider.Configuration.UsbHub2IsAvailable)
				Assert.Ignore("'USBHub2' is not available, therefore this test is excluded. Ensure that 'USBHub2' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.
		}
	}

	#endregion
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
