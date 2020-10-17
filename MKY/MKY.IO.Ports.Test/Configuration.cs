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
// Copyright © 2007-2020 Matthias Kläy.
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
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;

using MKY.Configuration;

using NUnit.Framework;

#endregion

namespace MKY.IO.Ports.Test
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

		private ConfigurationPropertyCollection properties; // = null;

		private ConfigurationProperty portA = new ConfigurationProperty("PortA", typeof(string), "COM1");
		private ConfigurationProperty portB = new ConfigurationProperty("PortB", typeof(string), "COM2");

		private ConfigurationProperty mtSicsDeviceA = new ConfigurationProperty("MTSicsDeviceA", typeof(string), "COM11");
		private ConfigurationProperty mtSicsDeviceB = new ConfigurationProperty("MTSicsDeviceB", typeof(string), "COM12");

		private ConfigurationProperty tiLaunchPadDeviceA = new ConfigurationProperty("TILaunchPadDeviceA", typeof(string), "COM21");
		private ConfigurationProperty tiLaunchPadDeviceB = new ConfigurationProperty("TILaunchPadDeviceB", typeof(string), "COM22");

		private ConfigurationProperty loopbackPairs = new ConfigurationProperty("LoopbackPairs", typeof(SerialPortPairConfigurationElementCollection), null /* DefaultValue doesn't work with a collection => Must be added in constructor */);
		private ConfigurationProperty loopbackSelfs = new ConfigurationProperty("LoopbackSelfs", typeof(SerialPortConfigurationElementCollection),     null /* DefaultValue doesn't work with a collection => Must be added in constructor */);

		#endregion

		#region Auto-Properties
		//==========================================================================================
		// Auto-Properties
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Port is named with a single letter.")]
		public bool PortAIsAvailable { get; set; }

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Port is named with a single letter.")]
		public bool PortBIsAvailable { get; set; }

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "'MT-SICS' is a name.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Port is named with a single letter.")]
		public bool MTSicsDeviceAIsAvailable { get; set; }

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "'MT-SICS' is a name.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Port is named with a single letter.")]
		public bool MTSicsDeviceBIsAvailable { get; set; }

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Port is named with a single letter.")]
		public bool TILaunchPadDeviceAIsAvailable { get; set; }

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Port is named with a single letter.")]
		public bool TILaunchPadDeviceBIsAvailable { get; set; }

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Don't care, straightforward test implementation.")]
		public bool[] LoopbackPairIsAvailable { get; set; }

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Don't care, straightforward test implementation.")]
		public bool[] LoopbackSelfIsAvailable { get; set; }

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

			this.properties.Add(this.portA);
			this.properties.Add(this.portB);

			this.properties.Add(this.mtSicsDeviceA);
			this.properties.Add(this.mtSicsDeviceB);

			this.properties.Add(this.tiLaunchPadDeviceA);
			this.properties.Add(this.tiLaunchPadDeviceB);

			this.properties.Add(this.loopbackPairs);
			var pairs = (SerialPortPairConfigurationElementCollection)this["LoopbackPairs"];
			pairs.Add("COM1", "COM2");

			this.properties.Add(this.loopbackSelfs);
			var selfs = (SerialPortConfigurationElementCollection)this["LoopbackSelfs"];
			selfs.Add("COM3");
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
		public virtual string PortA
		{
			get { return ((string)this["PortA"]); }
		}

		/// <summary></summary>
		public virtual string PortB
		{
			get { return ((string)this["PortB"]); }
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

		/// <summary></summary>
		public virtual SerialPortPairConfigurationElementCollection LoopbackPairs
		{
			get { return ((SerialPortPairConfigurationElementCollection)this["LoopbackPairs"]); }
		}

		/// <summary></summary>
		public virtual bool LoopbackPairsAreAvailable
		{
			get { return (LoopbackPairs.Count > 0); }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		public virtual SerialPortConfigurationElementCollection LoopbackSelfs
		{
			get { return ((SerialPortConfigurationElementCollection)this["LoopbackSelfs"]); }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		public virtual bool LoopbackSelfsAreAvailable
		{
			get { return (LoopbackSelfs.Count > 0); }
		}

		/// <summary></summary>
		public virtual int LoopbackCount
		{
			get { return (LoopbackPairs.Count + LoopbackSelfs.Count); }
		}

		/// <summary></summary>
		public virtual bool LoopbacksAreAvailable
		{
			get { return (LoopbackCount > 0); }
		}

		#endregion
	}

	#endregion

	#region Provider
	//==============================================================================================
	// Provider
	//==============================================================================================

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
		public static readonly string UserConfigurationEnvironmentVariableName = "MKY_IO_PORTS_TEST_CONFIG_FILE";
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
				var availablePorts = new SerialPortCollection();
				availablePorts.FillWithAvailablePorts(false); // Explicitly not getting captions, thus faster.

				// Set which physical items are available on the current machine:

				configuration.PortAIsAvailable = availablePorts.Contains(configuration.PortA);
				configuration.PortBIsAvailable = availablePorts.Contains(configuration.PortB);

				configuration.MTSicsDeviceAIsAvailable = availablePorts.Contains(configuration.MTSicsDeviceA);
				configuration.MTSicsDeviceBIsAvailable = availablePorts.Contains(configuration.MTSicsDeviceB);

				configuration.TILaunchPadDeviceAIsAvailable = availablePorts.Contains(configuration.TILaunchPadDeviceA);
				configuration.TILaunchPadDeviceBIsAvailable = availablePorts.Contains(configuration.TILaunchPadDeviceB);

				List<bool> l;

				l = new List<bool>(configuration.LoopbackPairs.Count); // Preset the required capacity to improve memory management.
				foreach (SerialPortPairConfigurationElement item in configuration.LoopbackPairs)
				{
					l.Add(availablePorts.Contains(item.PortA) && availablePorts.Contains(item.PortB));
				}
				configuration.LoopbackPairIsAvailable = l.ToArray();

				l = new List<bool>(configuration.LoopbackSelfs.Count); // Preset the required capacity to improve memory management.
				foreach (SerialPortConfigurationElement item in configuration.LoopbackSelfs)
				{
					l.Add(availablePorts.Contains(item.Port));
				}
				configuration.LoopbackSelfIsAvailable = l.ToArray();

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
	//==============================================================================================
	// Categories
	//==============================================================================================

	/// <remarks>
	/// Note that NUnit category strings may not contain the following characters as specified
	/// by <see cref="CategoryAttribute"/>: ',' '+' '-' and '!'
	///
	/// Saying hello to StyleCop ;-.
	/// </remarks>
	public static class ConfigurationCategoryStrings
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Port is named with a single letter.")]
		public static readonly string PortAIsAvailable = "Serial port A '" + ConfigurationProvider.Configuration.PortA + "' is " + (ConfigurationProvider.Configuration.PortAIsAvailable ? "" : "*NOT* ") + "available" + (ConfigurationProvider.Configuration.PortAIsAvailable ? "" : " => FIX OR EXCLUDE"); // Attention, no '!' allowed in NUnit test category strings!

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Port is named with a single letter.")]
		public static readonly string PortBIsAvailable = "Serial port B '" + ConfigurationProvider.Configuration.PortB + "' is " + (ConfigurationProvider.Configuration.PortBIsAvailable ? "" : "*NOT* ") + "available" + (ConfigurationProvider.Configuration.PortBIsAvailable ? "" : " => FIX OR EXCLUDE"); // Attention, no '!' allowed in NUnit test category strings!

		/// <remarks>
		/// "MT-SICS" is no valid NUnit category string as it contains an '-'.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "'MT-SICS' is a name.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Port is named with a single letter.")]
		public static readonly string MTSicsDeviceAIsAvailable = "Serial port MT SICS device A is " + (ConfigurationProvider.Configuration.MTSicsDeviceAIsAvailable ? "" : "*NOT* ") + "available on '" + ConfigurationProvider.Configuration.MTSicsDeviceA + "'" + (ConfigurationProvider.Configuration.MTSicsDeviceAIsAvailable ? "" : " => FIX OR EXCLUDE"); // Attention, no '!' allowed in NUnit test category strings!

		/// <remarks>
		/// "MT-SICS" is no valid NUnit category string as it contains an '-'.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "'MT-SICS' is a name.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Port is named with a single letter.")]
		public static readonly string MTSicsDeviceBIsAvailable = "Serial port MT SICS device B is " + (ConfigurationProvider.Configuration.MTSicsDeviceBIsAvailable ? "" : "*NOT* ") + "available on '" + ConfigurationProvider.Configuration.MTSicsDeviceB + "'" + (ConfigurationProvider.Configuration.MTSicsDeviceBIsAvailable ? "" : " => FIX OR EXCLUDE"); // Attention, no '!' allowed in NUnit test category strings!

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Port is named with a single letter.")]
		public static readonly string TILaunchPadDeviceAIsAvailable = "Serial port TI LaunchPad device A is " + (ConfigurationProvider.Configuration.TILaunchPadDeviceAIsAvailable ? "" : "*NOT* ") + "available on '" + ConfigurationProvider.Configuration.TILaunchPadDeviceA + "'" + (ConfigurationProvider.Configuration.TILaunchPadDeviceAIsAvailable ? "" : " => FIX OR EXCLUDE"); // Attention, no '!' allowed in NUnit test category strings!

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Port is named with a single letter.")]
		public static readonly string TILaunchPadDeviceBIsAvailable = "Serial port TI LaunchPad device B is " + (ConfigurationProvider.Configuration.TILaunchPadDeviceBIsAvailable ? "" : "*NOT* ") + "available on '" + ConfigurationProvider.Configuration.TILaunchPadDeviceB + "'" + (ConfigurationProvider.Configuration.TILaunchPadDeviceBIsAvailable ? "" : " because such devices don't work concurrently => EXCLUDE"); // Attention, no '!' allowed in NUnit test category strings!
	////public static readonly string TILaunchPadDeviceBIsAvailable = "Serial port TI LaunchPad device B is " + (ConfigurationProvider.Configuration.TILaunchPadDeviceBIsAvailable ? "" : "*NOT* ") + "available on '" + ConfigurationProvider.Configuration.TILaunchPadDeviceB + "'" + (ConfigurationProvider.Configuration.TILaunchPadDeviceBIsAvailable ? "" : " => FIX OR EXCLUDE"); // Attention, no '!' allowed in NUnit test category strings!

		/// <summary></summary>
		public static readonly string LoopbackPairsAreAvailable = "Serial port loopback pair" + ((ConfigurationProvider.Configuration.LoopbackPairsAreAvailable) ? ((ConfigurationProvider.Configuration.LoopbackPairs.Count > 1) ? "s are " : " is ") : "*NOT* ") + "available" + ((ConfigurationProvider.Configuration.LoopbackPairsAreAvailable) ? "" : " => FIX OR EXCLUDE"); // Attention, no '!' allowed in NUnit test category strings!

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		public static readonly string LoopbackSelfsAreAvailable = "Serial port loopback self" + ((ConfigurationProvider.Configuration.LoopbackSelfsAreAvailable) ? ((ConfigurationProvider.Configuration.LoopbackSelfs.Count > 1) ? "s are " : " is ") : "*NOT* ") + "available" + ((ConfigurationProvider.Configuration.LoopbackSelfsAreAvailable) ? "" : " => FIX OR EXCLUDE"); // Attention, no '!' allowed in NUnit test category strings!

		/// <summary></summary>
		public static readonly string LoopbacksAreAvailable = "Serial port loopback" + ((ConfigurationProvider.Configuration.LoopbacksAreAvailable) ? ((ConfigurationProvider.Configuration.LoopbackCount > 1) ? "s are " : " is ") : "*NOT* ") + "available" + ((ConfigurationProvider.Configuration.LoopbacksAreAvailable) ? "" : " => FIX OR EXCLUDE"); // Attention, no '!' allowed in NUnit test category strings!
	}

	/// <remarks>
	/// Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Port is named with a single letter.")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class PortAIsAvailableCategoryAttribute : CategoryAttribute
	{
		/// <summary></summary>
		public PortAIsAvailableCategoryAttribute()
			: base(ConfigurationCategoryStrings.PortAIsAvailable)
		{
		}
	}

	/// <remarks>
	/// Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Port is named with a single letter.")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class PortBIsAvailableCategoryAttribute : CategoryAttribute
	{
		/// <summary></summary>
		public PortBIsAvailableCategoryAttribute()
			: base(ConfigurationCategoryStrings.PortBIsAvailable)
		{
		}
	}

	/// <remarks>
	/// Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "'MT-SICS' is a name.")]
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Device is named with a single letter.")]
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
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Device is named with a single letter.")]
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
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Device is named with a single letter.")]
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
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Device is named with a single letter.")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class TILaunchPadDeviceBIsAvailableCategoryAttribute : CategoryAttribute
	{
		/// <summary></summary>
		public TILaunchPadDeviceBIsAvailableCategoryAttribute()
			: base(ConfigurationCategoryStrings.TILaunchPadDeviceBIsAvailable)
		{
		}
	}

	/// <remarks>
	/// Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class LoopbackPairsAreAvailableCategoryAttribute : CategoryAttribute
	{
		/// <summary></summary>
		public LoopbackPairsAreAvailableCategoryAttribute()
			: base(ConfigurationCategoryStrings.LoopbackPairsAreAvailable)
		{
		}
	}

	/// <remarks>
	/// Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class LoopbackSelfsAreAvailableCategoryAttribute : CategoryAttribute
	{
		/// <summary></summary>
		public LoopbackSelfsAreAvailableCategoryAttribute()
			: base(ConfigurationCategoryStrings.LoopbackSelfsAreAvailable)
		{
		}
	}

	/// <remarks>
	/// Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class LoopbacksAreAvailableCategoryAttribute : CategoryAttribute
	{
		/// <summary></summary>
		public LoopbacksAreAvailableCategoryAttribute()
			: base(ConfigurationCategoryStrings.LoopbacksAreAvailable)
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
		[Test, PortAIsAvailableCategory, PortBIsAvailableCategory]
		public virtual void TestPortIsAvailableCategories()
		{
			if (!ConfigurationProvider.Configuration.PortAIsAvailable)
				Assert.Ignore("'PortA' is not available, therefore this test is excluded. Ensure that 'PortA' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			if (!ConfigurationProvider.Configuration.PortBIsAvailable)
				Assert.Ignore("'PortB' is not available, therefore this test is excluded. Ensure that 'PortB' is properly configured and available if passing this test is required.");
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
		[Test, TILaunchPadDeviceAIsAvailableCategory] // Device B excluded because TI LaunchPad devices don't work concurrently.
	////[Test, TILaunchPadDeviceAIsAvailableCategory, TILaunchPadDeviceBIsAvailableCategory]
		public virtual void TestTILaunchPadDeviceIsAvailableCategories()
		{
			if (!ConfigurationProvider.Configuration.TILaunchPadDeviceAIsAvailable)
				Assert.Ignore("'TILaunchPadDeviceA' is not available, therefore this test is excluded. Ensure that 'TILaunchPadDeviceA' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

		////if (!ConfigurationProvider.Configuration.TILaunchPadDeviceBIsAvailable) Device B excluded because TI LaunchPad devices don't work concurrently.
		////	Assert.Ignore("'TILaunchPadDeviceB' is not available, therefore this test is excluded. Ensure that 'TILaunchPadDeviceB' is properly configured and available if passing this test is required.");
		//////// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.
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
		[Test, LoopbackPairsAreAvailableCategory, LoopbackSelfsAreAvailableCategory, LoopbacksAreAvailableCategory]
		public virtual void TestLoopbackCategories()
		{
			if (!ConfigurationProvider.Configuration.LoopbackPairsAreAvailable)
				Assert.Ignore("No loopback pairs are available, therefore this test is excluded. Ensure that loopback pairs are properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.
		}
	}

	#endregion
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
