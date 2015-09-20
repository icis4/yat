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

		private ConfigurationPropertyCollection properties;

		private ConfigurationProperty specificIPv4InterfaceIsAvailable = new ConfigurationProperty("SpecificIPv4InterfaceIsAvailable", typeof(bool), false);
		private ConfigurationProperty specificIPv6InterfaceIsAvailable = new ConfigurationProperty("SpecificIPv6InterfaceIsAvailable", typeof(bool), false);

		private ConfigurationProperty specificIPv4Interface = new ConfigurationProperty("SpecificIPv4Interface", typeof(string), "TAP-Win32 Adapter");
		private ConfigurationProperty specificIPv6Interface = new ConfigurationProperty("SpecificIPv6Interface", typeof(string), "TAP-Win32 Adapter");

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

			this.properties.Add(this.specificIPv4InterfaceIsAvailable);
			this.properties.Add(this.specificIPv6InterfaceIsAvailable);

			this.properties.Add(this.specificIPv4Interface);
			this.properties.Add(this.specificIPv6Interface);
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
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
		public virtual bool SpecificIPv4InterfaceIsAvailable
		{
			get { return ((bool)this["SpecificIPv4InterfaceIsAvailable"]); }
			set
			{
				AssertNotReadOnly("SpecificIPv4InterfaceIsAvailable");
				this["SpecificIPv4InterfaceIsAvailable"] = value;
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
		public virtual bool SpecificIPv6InterfaceIsAvailable
		{
			get { return ((bool)this["SpecificIPv6InterfaceIsAvailable"]); }
			set
			{
				AssertNotReadOnly("SpecificIPv6InterfaceIsAvailable");
				this["SpecificIPv6InterfaceIsAvailable"] = value;
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
		public virtual string SpecificIPv4Interface
		{
			get { return ((string)this["SpecificIPv4Interface"]); }
			set
			{
				AssertNotReadOnly("SpecificIPv4Interface");
				this["SpecificIPv4Interface"] = value;
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
		public virtual string SpecificIPv6Interface
		{
			get { return ((string)this["SpecificIPv6Interface"]); }
			set
			{
				AssertNotReadOnly("SpecificIPv6Interface");
				this["SpecificIPv6Interface"] = value;
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
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
		public static readonly string IPv4LoopbackIsAvailable = "IPv4 loopback is available";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
		public static readonly string IPv6LoopbackIsAvailable = "IPv6 loopback is available";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
		public static readonly string SpecificIPv4InterfaceIsAvailable = "Specific IPv4 interface '" + ConfigurationProvider.Configuration.SpecificIPv4Interface + "' is available";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
		public static readonly string SpecificIPv6InterfaceIsAvailable = "Specific IPv6 interface '" + ConfigurationProvider.Configuration.SpecificIPv6Interface + "' is available";
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
	public sealed class SpecificIPv4InterfaceIsAvailableCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public SpecificIPv4InterfaceIsAvailableCategoryAttribute()
			: base(ConfigurationCategoryStrings.SpecificIPv4InterfaceIsAvailable)
		{
		}
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class SpecificIPv6InterfaceIsAvailableCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public SpecificIPv6InterfaceIsAvailableCategoryAttribute()
			: base(ConfigurationCategoryStrings.SpecificIPv6InterfaceIsAvailable)
		{
		}
	}

	#endregion
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
