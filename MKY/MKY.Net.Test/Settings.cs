//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
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

namespace MKY.Net.Test
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
		/// Initializes a new instance of the <see cref="SettingsSection"/> class.
		/// </summary>
		public SettingsSection()
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
			get { return (bool)this["SpecificIPv4InterfaceIsAvailable"]; }
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
			get { return (bool)this["SpecificIPv6InterfaceIsAvailable"]; }
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
			get { return (string)this["SpecificIPv4Interface"]; }
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
			get { return (string)this["SpecificIPv6Interface"]; }
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
		public static readonly string UserSettingsEnvironmentVariableName = "MKY_NET_TEST_SETTINGS_FILE";
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
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
		public static readonly string IPv4LoopbackIsAvailable = "IPv4 loopback is available";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
		public static readonly string IPv6LoopbackIsAvailable = "IPv6 loopback is available";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
		public static readonly string SpecificIPv4InterfaceIsAvailable = "Specific IPv4 interface '" + SettingsProvider.Settings.SpecificIPv4Interface + "' is available";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
		public static readonly string SpecificIPv6InterfaceIsAvailable = "Specific IPv6 interface '" + SettingsProvider.Settings.SpecificIPv6Interface + "' is available";
	}

	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public class IPv4LoopbackIsAvailableCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public IPv4LoopbackIsAvailableCategoryAttribute()
			: base(SettingsCategoryStrings.IPv4LoopbackIsAvailable)
		{
		}
	}

	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public class IPv6LoopbackIsAvailableCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public IPv6LoopbackIsAvailableCategoryAttribute()
			: base(SettingsCategoryStrings.IPv6LoopbackIsAvailable)
		{
		}
	}

	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public class SpecificIPv4InterfaceIsAvailableCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public SpecificIPv4InterfaceIsAvailableCategoryAttribute()
			: base(SettingsCategoryStrings.SpecificIPv4InterfaceIsAvailable)
		{
		}
	}

	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IP, IPv4, IPv6 are well-known terms.")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public class SpecificIPv6InterfaceIsAvailableCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public SpecificIPv6InterfaceIsAvailableCategoryAttribute()
			: base(SettingsCategoryStrings.SpecificIPv6InterfaceIsAvailable)
		{
		}
	}

	#endregion
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
