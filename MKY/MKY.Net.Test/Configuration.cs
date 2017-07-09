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
// MKY Version 1.0.20
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
using System.Net;
using System.Net.NetworkInformation;

using MKY.Configuration;

namespace MKY.Net.Test
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

		private ConfigurationPropertyCollection properties;

		private ConfigurationProperty ipv4SpecificInterface = new ConfigurationProperty("IPv4SpecificInterface", typeof(string), "TAP-Win32 Adapter");
		private ConfigurationProperty ipv6SpecificInterface = new ConfigurationProperty("IPv6SpecificInterface", typeof(string), "TAP-Win32 Adapter");

		private ConfigurationProperty mtSicsDeviceTcpPort = new ConfigurationProperty("MTSicsDeviceTcpPort", typeof(string), "55600"); // ExBal

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
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "MT-SICS is a name.")]
		public virtual bool MTSicsDeviceIsAvailable { get; set; }

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "MT-SICS is a name.")]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "int", Justification = "Work arond naming conflict.")]
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
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "MT-SICS is a name.")]
		public virtual string MTSicsDeviceTcpPort
		{
			get { return ((string)this["MTSicsDeviceTcpPort"]); }
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
			if (Provider.TryOpenAndMergeConfigurations<ConfigurationSection>(ConfigurationConstants.SelectionGroupName, ConfigurationConstants.SectionsGroupName, ConfigurationConstants.SolutionConfigurationFileNameSuffix, ConfigurationConstants.UserConfigurationEnvironmentVariableName, out configuration))
			{
				// Set which physical items are available on the current machine:
				IPNetworkInterfaceCollection inferfaces = new IPNetworkInterfaceCollection();
				inferfaces.FillWithAvailableLocalInterfaces();

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

		/// <remarks>"MT-SICS" is no valid NUnit category string as it contains an '-'.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "MT-SICS is a name.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Port is named with a single letter.")]
		public static readonly string MTSicsDeviceIsAvailable = "MT SICS device is " + (ConfigurationProvider.Configuration.MTSicsDeviceIsAvailable ? "" : "*NOT* ") + "available on TCP port " + ConfigurationProvider.Configuration.MTSicsDeviceTcpPort + (ConfigurationProvider.Configuration.MTSicsDeviceIsAvailable ? "" : " => FIX OR EXCLUDE"); // Attention, no '!' allowed in NUnit test category strings!
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class IPv4LoopbackIsAvailableCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public IPv4LoopbackIsAvailableCategoryAttribute()
			: base(ConfigurationCategoryStrings.IPv4LoopbackIsAvailable)
		{
		}
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class IPv6LoopbackIsAvailableCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public IPv6LoopbackIsAvailableCategoryAttribute()
			: base(ConfigurationCategoryStrings.IPv6LoopbackIsAvailable)
		{
		}
	}

	/// <remarks>Currently limited to a single specific interface.</remarks>
	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class IPv4SpecificInterfaceIsAvailableCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public IPv4SpecificInterfaceIsAvailableCategoryAttribute()
			: base(ConfigurationCategoryStrings.IPv4SpecificInterfaceIsAvailable)
		{
		}
	}

	/// <remarks>Currently limited to a single specific interface.</remarks>
	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class IPv6SpecificInterfaceIsAvailableCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public IPv6SpecificInterfaceIsAvailableCategoryAttribute()
			: base(ConfigurationCategoryStrings.IPv6SpecificInterfaceIsAvailable)
		{
		}
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "MT-SICS is a name.")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class MTSicsDeviceIsAvailableCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public MTSicsDeviceIsAvailableCategoryAttribute()
			: base(ConfigurationCategoryStrings.MTSicsDeviceIsAvailable)
		{
		}
	}

	#endregion
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
