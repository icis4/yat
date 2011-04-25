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
// Copyright © 2010-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;

using MKY.Configuration;

namespace MKY.IO.Ports.Test
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

		private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

		private ConfigurationProperty serialPortAIsAvailable = new ConfigurationProperty("SerialPortAIsAvailable", typeof(bool), false);
		private ConfigurationProperty serialPortBIsAvailable = new ConfigurationProperty("SerialPortBIsAvailable", typeof(bool), false);

		private ConfigurationProperty serialPortA = new ConfigurationProperty("SerialPortA", typeof(string), "COM1");
		private ConfigurationProperty serialPortB = new ConfigurationProperty("SerialPortB", typeof(string), "COM2");

		private ConfigurationProperty serialPortsAreInterconnected = new ConfigurationProperty("SerialPortsAreInterconnected", typeof(bool), false);

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
			this.properties.Add(this.serialPortAIsAvailable);
			this.properties.Add(this.serialPortBIsAvailable);

			this.properties.Add(this.serialPortA);
			this.properties.Add(this.serialPortB);

			this.properties.Add(this.serialPortsAreInterconnected);
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
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "PortA and PortB")]
		public virtual bool SerialPortAIsAvailable
		{
			get { return (bool)this["SerialPortAIsAvailable"]; }
			set
			{
				AssertNotReadOnly("SerialPortAIsAvailable");
				this["SerialPortAIsAvailable"] = value;
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "PortA and PortB")]
		public virtual bool SerialPortBIsAvailable
		{
			get { return (bool)this["SerialPortBIsAvailable"]; }
			set
			{
				AssertNotReadOnly("SerialPortBIsAvailable");
				this["SerialPortBIsAvailable"] = value;
			}
		}

		/// <summary></summary>
		public virtual string SerialPortA
		{
			get { return (string)this["SerialPortA"]; }
			set
			{
				AssertNotReadOnly("SerialPortA");
				this["SerialPortA"] = value;
			}
		}

		/// <summary></summary>
		public virtual string SerialPortB
		{
			get { return (string)this["SerialPortB"]; }
			set
			{
				AssertNotReadOnly("SerialPortB");
				this["SerialPortB"] = value;
			}
		}

		/// <summary></summary>
		public virtual bool SerialPortsAreInterconnected
		{
			get
			{
				if (SerialPortAIsAvailable && SerialPortBIsAvailable)
					return (bool)this["SerialPortsAreInterconnected"];
				else
					return (false);
			}
			set
			{
				AssertNotReadOnly("SerialPortsAreInterconnected");
				this["SerialPortsAreInterconnected"] = value;
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
		public static readonly string UserSettingsEnvironmentVariableName = "MKY_IO_PORTS_TEST_SETTINGS_FILE";
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
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "PortA and PortB")]
		public static readonly string SerialPortAIsAvailable = "Serial port " + SettingsProvider.Settings.SerialPortA + " is available";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "PortA and PortB")]
		public static readonly string SerialPortBIsAvailable = "Serial port " + SettingsProvider.Settings.SerialPortB + " is available";

		/// <summary></summary>
		public static readonly string SerialPortsAreInterconnected = "Serial ports are interconnected";
	}

	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "PortA and PortB")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public class SerialPortAIsAvailableCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public SerialPortAIsAvailableCategoryAttribute()
			: base(SettingsCategoryStrings.SerialPortAIsAvailable)
		{
		}
	}

	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "PortA and PortB")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public class SerialPortBIsAvailableCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public SerialPortBIsAvailableCategoryAttribute()
			: base(SettingsCategoryStrings.SerialPortBIsAvailable)
		{
		}
	}

	/// <summary></summary>
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public class SerialPortsAreInterconnectedCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public SerialPortsAreInterconnectedCategoryAttribute()
			: base(SettingsCategoryStrings.SerialPortsAreInterconnected)
		{
		}
	}

	#endregion
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
