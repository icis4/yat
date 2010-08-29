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
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright � 2010 Matthias Kl�y.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Configuration;

using MKY.Utilities;
using MKY.Utilities.Configuration;

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
		public static readonly string IPv4LoopbackIsAvailable = @"IPv4 loopback is available";
		/// <summary></summary>
		public static readonly string IPv6LoopbackIsAvailable = @"IPv6 loopback is available";
		/// <summary></summary>
		public static readonly string SpecificIPv4InterfaceIsAvailable = @"Specific IPv4 interface """ + SettingsProvider.Settings.SpecificIPv4Interface + @""" is available";
		/// <summary></summary>
		public static readonly string SpecificIPv6InterfaceIsAvailable = @"Specific IPv6 interface """ + SettingsProvider.Settings.SpecificIPv6Interface + @""" is available";
	}

	/// <summary></summary>
	public class IPv4LoopbackIsAvailableCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public IPv4LoopbackIsAvailableCategoryAttribute()
			: base(SettingsCategoryStrings.IPv4LoopbackIsAvailable)
		{
		}
	}

	/// <summary></summary>
	public class IPv6LoopbackIsAvailableCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public IPv6LoopbackIsAvailableCategoryAttribute()
			: base(SettingsCategoryStrings.IPv6LoopbackIsAvailable)
		{
		}
	}

	/// <summary></summary>
	public class SpecificIPv4InterfaceIsAvailableCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public SpecificIPv4InterfaceIsAvailableCategoryAttribute()
			: base(SettingsCategoryStrings.SpecificIPv4InterfaceIsAvailable)
		{
		}
	}

	/// <summary></summary>
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
