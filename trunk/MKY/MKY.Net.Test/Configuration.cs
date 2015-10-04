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

		#region Fields > Configuration
		//------------------------------------------------------------------------------------------
		// Fields > Configuration
		//------------------------------------------------------------------------------------------

		private ConfigurationPropertyCollection properties;

		private ConfigurationProperty ipv4SpecificInterface = new ConfigurationProperty("IPv4SpecificInterface", typeof(string), "TAP-Win32 Adapter");
		private ConfigurationProperty ipv6SpecificInterface = new ConfigurationProperty("IPv6SpecificInterface", typeof(string), "TAP-Win32 Adapter");

		#endregion

		#region Fields > Auxiliary
		//------------------------------------------------------------------------------------------
		// Fields > Auxiliary
		//------------------------------------------------------------------------------------------

		private bool ipv4SpecificInterfaceIsAvailable;
		private bool ipv6SpecificInterfaceIsAvailable;

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
			this.properties = new ConfigurationPropertyCollection();

			this.properties.Add(this.ipv4SpecificInterface);
			this.properties.Add(this.ipv6SpecificInterface);
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
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
		public virtual string IPv4SpecificInterface
		{
			get { return ((string)this["IPv4SpecificInterface"]); }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
		public virtual string IPv6SpecificInterface
		{
			get { return ((string)this["IPv6SpecificInterface"]); }
		}

		#endregion

		#region Properties > Auxiliary
		//------------------------------------------------------------------------------------------
		// Properties > Auxiliary
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
		public virtual bool IPv4SpecificInterfaceIsAvailable
		{
			get { return (this.ipv4SpecificInterfaceIsAvailable); }
			set { this.ipv4SpecificInterfaceIsAvailable = value;  }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
		public virtual bool IPv6SpecificInterfaceIsAvailable
		{
			get { return (this.ipv6SpecificInterfaceIsAvailable); }
			set { this.ipv6SpecificInterfaceIsAvailable = value;  }
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
		public static readonly string ConfigurationGroupName = typeof(ConfigurationConstants).Namespace + ".Configuration";

		/// <summary></summary>
		public static readonly string ConfigurationSectionsGroupName = ConfigurationGroupName + ".Sections";

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
			if (Provider.TryOpenAndMergeConfigurations<ConfigurationSection>(ConfigurationConstants.ConfigurationGroupName, ConfigurationConstants.ConfigurationSectionsGroupName, ConfigurationConstants.SolutionConfigurationFileNameSuffix, ConfigurationConstants.UserConfigurationEnvironmentVariableName, out configuration))
			{
				// Set which physical items are available on the current machine:
				IPNetworkInterfaceCollection inferfaces = new IPNetworkInterfaceCollection();
				inferfaces.FillWithAvailableInterfaces();

				IPNetworkInterface ni;
				if (IPNetworkInterface.TryParse(configuration.IPv4SpecificInterface, out ni))
					configuration.IPv4SpecificInterfaceIsAvailable = inferfaces.Contains(ni);

				if (IPNetworkInterface.TryParse(configuration.IPv6SpecificInterface, out ni))
					configuration.IPv6SpecificInterfaceIsAvailable = inferfaces.Contains(ni);

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

	/// <summary></summary>
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
		public static readonly string IPv4SpecificInterfaceIsAvailable = "IPv4 specific interface";
		//public static readonly string IPv4SpecificInterfaceIsAvailable = "IPv4 specific interface '" + ConfigurationProvider.Configuration.IPv4SpecificInterface + "' is available";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
		public static readonly string IPv6SpecificInterfaceIsAvailable = "IPv6 specific interface";
		//public static readonly string IPv6SpecificInterfaceIsAvailable = "IPv6 specific interface '" + ConfigurationProvider.Configuration.IPv6SpecificInterface + "' is available";
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

	#endregion
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
