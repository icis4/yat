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
// MKY Version 1.0.29
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2021 Matthias Kläy.
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
using System.Net;
using System.Net.NetworkInformation;

using MKY.Configuration;

using NUnit.Framework;

#endregion

namespace MKY.Net.Test
{
	#region Section
	//===============================================================================================
	// Section
	//===============================================================================================

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

		private ConfigurationProperty ipv4SpecificInterface = new ConfigurationProperty("IPv4SpecificInterface", typeof(string), "OpenVPN Virtual Ethernet Adapter");
		private ConfigurationProperty ipv6SpecificInterface = new ConfigurationProperty("IPv6SpecificInterface", typeof(string), "OpenVPN Virtual Ethernet Adapter");

		private ConfigurationProperty mtSicsDeviceTcpPort = new ConfigurationProperty("MTSicsDeviceTcpPort", typeof(string), "44400");

		#endregion

		#region Auto-Properties
		//==========================================================================================
		// Auto-Properties
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
		public virtual bool IPv4SpecificInterfaceIsAvailable { get; set; }

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
		public virtual bool IPv6SpecificInterfaceIsAvailable { get; set; }

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "'MT-SICS' is a name.")]
		public virtual bool MTSicsDeviceIsAvailable { get; set; }

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "'MT-SICS' is a name.")]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "int", Justification = "Work around naming conflict.")]
		public virtual int MTSicsDeviceTcpPortAsInt { get; set; }

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

			this.properties.Add(this.ipv4SpecificInterface);
			this.properties.Add(this.ipv6SpecificInterface);

			this.properties.Add(this.mtSicsDeviceTcpPort);
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
		public virtual bool IPv4LoopbackIsAvailable
		{
			get { return (true); } // Assumed to be always on, not configurable so far.
		}

		/// <summary></summary>
		public virtual bool IPv6LoopbackIsAvailable
		{
			get { return (true); } // Assumed to be always on, not configurable so far.
		}

		/// <remarks>Currently limited to a single specific interface.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
		public virtual string IPv4SpecificInterface
		{
			get { return ((string)this["IPv4SpecificInterface"]); }
		}

		/// <remarks>Currently limited to a single specific interface.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
		public virtual string IPv6SpecificInterface
		{
			get { return ((string)this["IPv6SpecificInterface"]); }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "'MT-SICS' is a name.")]
		public virtual string MTSicsDeviceTcpPort
		{
			get { return ((string)this["MTSicsDeviceTcpPort"]); }
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
		public static readonly string UserConfigurationEnvironmentVariableName = "MKY_NET_TEST_CONFIG_FILE";
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
				IPNetworkInterfaceCollection inferfaces = new IPNetworkInterfaceCollection();
				inferfaces.FillWithAvailableLocalInterfaces();

				// Set which physical items are available on the current machine:

				IPNetworkInterfaceEx ni;
				if (IPNetworkInterfaceEx.TryParse(configuration.IPv4SpecificInterface, out ni))
					configuration.IPv4SpecificInterfaceIsAvailable = inferfaces.Contains(ni);

				if (IPNetworkInterfaceEx.TryParse(configuration.IPv6SpecificInterface, out ni))
					configuration.IPv6SpecificInterfaceIsAvailable = inferfaces.Contains(ni);

				// Check whether a port is available at the specified MT-SICS device port:
				int port;
				if (int.TryParse(configuration.MTSicsDeviceTcpPort, out port))
				{
					if (IPEndPointEx.IsValidPort(port))
					{
						foreach (IPEndPoint tcpListener in IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners())
						{
							if (tcpListener.Port == port)
							{
								configuration.MTSicsDeviceIsAvailable = true;
								configuration.MTSicsDeviceTcpPortAsInt = port;
								break;
							}
						}
					}
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
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
		public static readonly string IPv4LoopbackIsAvailable = "IPv4 loopback is available";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
		public static readonly string IPv6LoopbackIsAvailable = "IPv6 loopback is available";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
		public static readonly string IPv4SpecificInterfaceIsAvailable = "IPv4 specific interface '" + ConfigurationProvider.Configuration.IPv4SpecificInterface + "' is " + (ConfigurationProvider.Configuration.IPv4SpecificInterfaceIsAvailable ? "" : "*NOT* ") + "available" + (ConfigurationProvider.Configuration.IPv4SpecificInterfaceIsAvailable ? "" : " => FIX OR EXCLUDE"); // Attention, no '!' allowed in NUnit test category strings!

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
		public static readonly string IPv6SpecificInterfaceIsAvailable = "IPv6 specific interface '" + ConfigurationProvider.Configuration.IPv6SpecificInterface + "' is " + (ConfigurationProvider.Configuration.IPv6SpecificInterfaceIsAvailable ? "" : "*NOT* ") + "available" + (ConfigurationProvider.Configuration.IPv6SpecificInterfaceIsAvailable ? "" : " => FIX OR EXCLUDE"); // Attention, no '!' allowed in NUnit test category strings!

		/// <remarks>
		/// "MT-SICS" would not be a valid NUnit category string as it contains an '-'.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "'MT-SICS' is a name.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Port is named with a single letter.")]
		public static readonly string MTSicsDeviceIsAvailable = "MT SICS device is " + (ConfigurationProvider.Configuration.MTSicsDeviceIsAvailable ? "" : "*NOT* ") + "available on TCP port " + ConfigurationProvider.Configuration.MTSicsDeviceTcpPort + (ConfigurationProvider.Configuration.MTSicsDeviceIsAvailable ? "" : " => FIX OR EXCLUDE"); // Attention, no '!' allowed in NUnit test category strings!
	}

	/// <remarks>
	/// Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class IPv4LoopbackIsAvailableCategoryAttribute : CategoryAttribute
	{
		/// <summary></summary>
		public IPv4LoopbackIsAvailableCategoryAttribute()
			: base(ConfigurationCategoryStrings.IPv4LoopbackIsAvailable)
		{
		}
	}

	/// <remarks>
	/// Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class IPv6LoopbackIsAvailableCategoryAttribute : CategoryAttribute
	{
		/// <summary></summary>
		public IPv6LoopbackIsAvailableCategoryAttribute()
			: base(ConfigurationCategoryStrings.IPv6LoopbackIsAvailable)
		{
		}
	}

	/// <remarks>
	/// Currently limited to a single specific interface.
	/// </remarks>
	/// <remarks>
	/// Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class IPv4SpecificInterfaceIsAvailableCategoryAttribute : CategoryAttribute
	{
		/// <summary></summary>
		public IPv4SpecificInterfaceIsAvailableCategoryAttribute()
			: base(ConfigurationCategoryStrings.IPv4SpecificInterfaceIsAvailable)
		{
		}
	}

	/// <remarks>
	/// Currently limited to a single specific interface.
	/// </remarks>
	/// <remarks>
	/// Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class IPv6SpecificInterfaceIsAvailableCategoryAttribute : CategoryAttribute
	{
		/// <summary></summary>
		public IPv6SpecificInterfaceIsAvailableCategoryAttribute()
			: base(ConfigurationCategoryStrings.IPv6SpecificInterfaceIsAvailable)
		{
		}
	}

	/// <remarks>
	/// Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "'MT-SICS' is a name.")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class MTSicsDeviceIsAvailableCategoryAttribute : CategoryAttribute
	{
		/// <summary></summary>
		public MTSicsDeviceIsAvailableCategoryAttribute()
			: base(ConfigurationCategoryStrings.MTSicsDeviceIsAvailable)
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
		[Test, IPv4LoopbackIsAvailableCategory, IPv6LoopbackIsAvailableCategory]
		public virtual void TestLoopbackCategories()
		{
			if (!ConfigurationProvider.Configuration.IPv4LoopbackIsAvailable)
				Assert.Ignore("No IPv4 loopback is available, therefore this test is excluded. Ensure that IPv4 loopback is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			if (!ConfigurationProvider.Configuration.IPv6LoopbackIsAvailable)
				Assert.Ignore("No IPv6 loopback is available, therefore this test is excluded. Ensure that IPv6 loopback is properly configured and available if passing this test is required.");
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
		[Test, IPv4SpecificInterfaceIsAvailableCategory, IPv6SpecificInterfaceIsAvailableCategory]
		public virtual void TestSpecificInterfaceCategories()
		{
			if (!ConfigurationProvider.Configuration.IPv4SpecificInterfaceIsAvailable)
				Assert.Ignore("No specific IPv4 interface is available, therefore this test is excluded. Ensure that specific IPv4 interface is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			if (!ConfigurationProvider.Configuration.IPv6SpecificInterfaceIsAvailable)
				Assert.Ignore("No specific IPv6 interface is available, therefore this test is excluded. Ensure that specific IPv6 interface is properly configured and available if passing this test is required.");
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
		[Test, MTSicsDeviceIsAvailableCategory]
		public virtual void TestMTSicsDeviceIsAvailableCategory()
		{
			if (!ConfigurationProvider.Configuration.MTSicsDeviceIsAvailable)
				Assert.Ignore("'MTSicsDevice' is not available, therefore this test is excluded. Ensure that 'MTSicsDevice' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.
		}
	}

	#endregion
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
