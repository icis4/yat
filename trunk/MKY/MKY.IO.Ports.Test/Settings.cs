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
// MKY Version 1.0.13
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
		private ConfigurationProperty serialPortCIsAvailable = new ConfigurationProperty("SerialPortCIsAvailable", typeof(bool), false);
		private ConfigurationProperty serialPortDIsAvailable = new ConfigurationProperty("SerialPortDIsAvailable", typeof(bool), false);
		private ConfigurationProperty serialPortEIsAvailable = new ConfigurationProperty("SerialPortEIsAvailable", typeof(bool), false);
		private ConfigurationProperty serialPortFIsAvailable = new ConfigurationProperty("SerialPortFIsAvailable", typeof(bool), false);

		private ConfigurationProperty serialPortA = new ConfigurationProperty("SerialPortA", typeof(string), "COM1");
		private ConfigurationProperty serialPortB = new ConfigurationProperty("SerialPortB", typeof(string), "COM2");
		private ConfigurationProperty serialPortC = new ConfigurationProperty("SerialPortC", typeof(string), "COM11");
		private ConfigurationProperty serialPortD = new ConfigurationProperty("SerialPortD", typeof(string), "COM12");
		private ConfigurationProperty serialPortE = new ConfigurationProperty("SerialPortE", typeof(string), "COM21");
		private ConfigurationProperty serialPortF = new ConfigurationProperty("SerialPortF", typeof(string), "COM22");

		private ConfigurationProperty serialPortsAreInterconnected = new ConfigurationProperty("SerialPortsAreInterconnected", typeof(bool), false);

		private ConfigurationProperty mtSicsDeviceAIsConnected = new ConfigurationProperty("MTSicsDeviceAIsConnected", typeof(bool), false);
		private ConfigurationProperty mtSicsDeviceBIsConnected = new ConfigurationProperty("MTSicsDeviceBIsConnected", typeof(bool), false);

		private ConfigurationProperty mtSicsDeviceA = new ConfigurationProperty("MTSicsDeviceA", typeof(string), "COM11");
		private ConfigurationProperty mtSicsDeviceB = new ConfigurationProperty("MTSicsDeviceB", typeof(string), "COM12");

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
			this.properties.Add(this.serialPortCIsAvailable);
			this.properties.Add(this.serialPortDIsAvailable);
			this.properties.Add(this.serialPortEIsAvailable);
			this.properties.Add(this.serialPortFIsAvailable);

			this.properties.Add(this.serialPortA);
			this.properties.Add(this.serialPortB);
			this.properties.Add(this.serialPortC);
			this.properties.Add(this.serialPortD);
			this.properties.Add(this.serialPortE);
			this.properties.Add(this.serialPortF);

			this.properties.Add(this.serialPortsAreInterconnected);

			this.properties.Add(this.mtSicsDeviceAIsConnected);
			this.properties.Add(this.mtSicsDeviceBIsConnected);

			this.properties.Add(this.mtSicsDeviceA);
			this.properties.Add(this.mtSicsDeviceB);
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
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Ports and devices are named with a single letter")]
		public virtual bool SerialPortAIsAvailable
		{
			get { return ((bool)this["SerialPortAIsAvailable"]); }
			set
			{
				AssertNotReadOnly("SerialPortAIsAvailable");
				this["SerialPortAIsAvailable"] = value;
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Ports and devices are named with a single letter")]
		public virtual bool SerialPortBIsAvailable
		{
			get { return ((bool)this["SerialPortBIsAvailable"]); }
			set
			{
				AssertNotReadOnly("SerialPortBIsAvailable");
				this["SerialPortBIsAvailable"] = value;
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "CIs", Justification = "Ports and devices are named with a single letter")]
		public virtual bool SerialPortCIsAvailable
		{
			get { return ((bool)this["SerialPortCIsAvailable"]); }
			set
			{
				AssertNotReadOnly("SerialPortCIsAvailable");
				this["SerialPortCIsAvailable"] = value;
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "DIs", Justification = "Ports and devices are named with a single letter")]
		public virtual bool SerialPortDIsAvailable
		{
			get { return ((bool)this["SerialPortDIsAvailable"]); }
			set
			{
				AssertNotReadOnly("SerialPortDIsAvailable");
				this["SerialPortDIsAvailable"] = value;
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "EIs", Justification = "Ports and devices are named with a single letter")]
		public virtual bool SerialPortEIsAvailable
		{
			get { return ((bool)this["SerialPortEIsAvailable"]); }
			set
			{
				AssertNotReadOnly("SerialPortEIsAvailable");
				this["SerialPortEIsAvailable"] = value;
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "FIs", Justification = "Ports and devices are named with a single letter")]
		public virtual bool SerialPortFIsAvailable
		{
			get { return ((bool)this["SerialPortFIsAvailable"]); }
			set
			{
				AssertNotReadOnly("SerialPortFIsAvailable");
				this["SerialPortFIsAvailable"] = value;
			}
		}

		/// <summary></summary>
		public virtual string SerialPortA
		{
			get { return ((string)this["SerialPortA"]); }
			set
			{
				AssertNotReadOnly("SerialPortA");
				this["SerialPortA"] = value;
			}
		}

		/// <summary></summary>
		public virtual string SerialPortB
		{
			get { return ((string)this["SerialPortB"]); }
			set
			{
				AssertNotReadOnly("SerialPortB");
				this["SerialPortB"] = value;
			}
		}

		/// <summary></summary>
		public virtual string SerialPortC
		{
			get { return ((string)this["SerialPortC"]); }
			set
			{
				AssertNotReadOnly("SerialPortC");
				this["SerialPortC"] = value;
			}
		}

		/// <summary></summary>
		public virtual string SerialPortD
		{
			get { return ((string)this["SerialPortD"]); }
			set
			{
				AssertNotReadOnly("SerialPortD");
				this["SerialPortD"] = value;
			}
		}

		/// <summary></summary>
		public virtual string SerialPortE
		{
			get { return ((string)this["SerialPortE"]); }
			set
			{
				AssertNotReadOnly("SerialPortE");
				this["SerialPortE"] = value;
			}
		}

		/// <summary></summary>
		public virtual string SerialPortF
		{
			get { return ((string)this["SerialPortF"]); }
			set
			{
				AssertNotReadOnly("SerialPortF");
				this["SerialPortF"] = value;
			}
		}

		/// <summary></summary>
		public virtual bool SerialPortsAreInterconnected
		{
			get
			{
				if (SerialPortAIsAvailable && SerialPortBIsAvailable)
					return ((bool)this["SerialPortsAreInterconnected"]);
				else
					return (false);
			}
			set
			{
				AssertNotReadOnly("SerialPortsAreInterconnected");
				this["SerialPortsAreInterconnected"] = value;
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Ports and devices are named with a single letter")]
		public virtual bool MTSicsDeviceAIsConnected
		{
			get { return ((bool)this["MTSicsDeviceAIsConnected"]); }
			set
			{
				AssertNotReadOnly("MTSicsDeviceAIsConnected");
				this["MTSicsDeviceAIsConnected"] = value;
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Ports and devices are named with a single letter")]
		public virtual bool MTSicsDeviceBIsConnected
		{
			get { return ((bool)this["MTSicsDeviceBIsConnected"]); }
			set
			{
				AssertNotReadOnly("MTSicsDeviceBIsConnected");
				this["MTSicsDeviceBIsConnected"] = value;
			}
		}

		/// <summary></summary>
		public virtual string MTSicsDeviceA
		{
			get { return ((string)this["MTSicsDeviceA"]); }
			set
			{
				AssertNotReadOnly("MTSicsDeviceA");
				this["MTSicsDeviceA"] = value;
			}
		}

		/// <summary></summary>
		public virtual string MTSicsDeviceB
		{
			get { return ((string)this["MTSicsDeviceB"]); }
			set
			{
				AssertNotReadOnly("MTSicsDeviceB");
				this["MTSicsDeviceB"] = value;
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
		private static readonly SettingsSection StaticSettings = new SettingsSection();

		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Settings need to be read during creation.")]
		static SettingsProvider()
		{
			SettingsSection settings;
			if (Provider.TryOpenAndMergeConfigurations<SettingsSection>(SettingsConstants.ConfigurationGroupName, SettingsConstants.ConfigurationsGroupName, SettingsConstants.UserSettingsEnvironmentVariableName, out settings))
			{
				// Ensure that physical ports are not only configured in the settings configuration
				// file but indeed available on the current machine:
				MKY.IO.Ports.SerialPortCollection serialPorts = new MKY.IO.Ports.SerialPortCollection();
				serialPorts.FillWithAvailablePorts(false);

				if (!serialPorts.Contains(settings.SerialPortA))
					settings.SerialPortAIsAvailable = false;

				if (!serialPorts.Contains(settings.SerialPortB))
					settings.SerialPortBIsAvailable = false;

				if (!serialPorts.Contains(settings.SerialPortC))
					settings.SerialPortCIsAvailable = false;

				if (!serialPorts.Contains(settings.SerialPortD))
					settings.SerialPortDIsAvailable = false;

				if (!serialPorts.Contains(settings.SerialPortE))
					settings.SerialPortEIsAvailable = false;

				if (!serialPorts.Contains(settings.SerialPortF))
					settings.SerialPortFIsAvailable = false;

				if (!serialPorts.Contains(settings.MTSicsDeviceA))
					settings.MTSicsDeviceAIsConnected = false;

				if (!serialPorts.Contains(settings.MTSicsDeviceB))
					settings.MTSicsDeviceBIsConnected = false;

				// Activate the effective settings:
				StaticSettings = settings;
			}
		}

		/// <summary></summary>
		public static SettingsSection Settings
		{
			get { return (StaticSettings); }
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
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Ports and devices are named with a single letter")]
		public static readonly string SerialPortAIsAvailable = "Serial port " + SettingsProvider.Settings.SerialPortA + " is available";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Ports and devices are named with a single letter")]
		public static readonly string SerialPortBIsAvailable = "Serial port " + SettingsProvider.Settings.SerialPortB + " is available";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "CIs", Justification = "Ports and devices are named with a single letter")]
		public static readonly string SerialPortCIsAvailable = "Serial port " + SettingsProvider.Settings.SerialPortC + " is available";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "DIs", Justification = "Ports and devices are named with a single letter")]
		public static readonly string SerialPortDIsAvailable = "Serial port " + SettingsProvider.Settings.SerialPortD + " is available";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "EIs", Justification = "Ports and devices are named with a single letter")]
		public static readonly string SerialPortEIsAvailable = "Serial port " + SettingsProvider.Settings.SerialPortE + " is available";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "FIs", Justification = "Ports and devices are named with a single letter")]
		public static readonly string SerialPortFIsAvailable = "Serial port " + SettingsProvider.Settings.SerialPortF + " is available";

		/// <summary></summary>
		public static readonly string SerialPortsAreInterconnected = "Serial ports are interconnected";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Ports and devices are named with a single letter")]
		public static readonly string MTSicsDeviceAIsConnected = "MT-SICS device is connected to " + SettingsProvider.Settings.MTSicsDeviceA;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Ports and devices are named with a single letter")]
		public static readonly string MTSicsDeviceBIsConnected = "MT-SICS device is connected to " + SettingsProvider.Settings.MTSicsDeviceA;
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Ports and devices are named with a single letter")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class SerialPortAIsAvailableCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public SerialPortAIsAvailableCategoryAttribute()
			: base(SettingsCategoryStrings.SerialPortAIsAvailable)
		{
		}
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Ports and devices are named with a single letter")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class SerialPortBIsAvailableCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public SerialPortBIsAvailableCategoryAttribute()
			: base(SettingsCategoryStrings.SerialPortBIsAvailable)
		{
		}
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "CIs", Justification = "Ports and devices are named with a single letter")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class SerialPortCIsAvailableCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public SerialPortCIsAvailableCategoryAttribute()
			: base(SettingsCategoryStrings.SerialPortCIsAvailable)
		{
		}
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "DIs", Justification = "Ports and devices are named with a single letter")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class SerialPortDIsAvailableCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public SerialPortDIsAvailableCategoryAttribute()
			: base(SettingsCategoryStrings.SerialPortDIsAvailable)
		{
		}
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "EIs", Justification = "Ports and devices are named with a single letter")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class SerialPortEIsAvailableCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public SerialPortEIsAvailableCategoryAttribute()
			: base(SettingsCategoryStrings.SerialPortEIsAvailable)
		{
		}
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "FIs", Justification = "Ports and devices are named with a single letter")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class SerialPortFIsAvailableCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public SerialPortFIsAvailableCategoryAttribute()
			: base(SettingsCategoryStrings.SerialPortFIsAvailable)
		{
		}
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class SerialPortsAreInterconnectedCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public SerialPortsAreInterconnectedCategoryAttribute()
			: base(SettingsCategoryStrings.SerialPortsAreInterconnected)
		{
		}
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Ports and devices are named with a single letter")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class MTSicsDeviceAIsConnectedCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public MTSicsDeviceAIsConnectedCategoryAttribute()
			: base(SettingsCategoryStrings.MTSicsDeviceAIsConnected)
		{
		}
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Ports and devices are named with a single letter")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class MTSicsDeviceBIsConnectedCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public MTSicsDeviceBIsConnectedCategoryAttribute()
			: base(SettingsCategoryStrings.MTSicsDeviceBIsConnected)
		{
		}
	}

	#endregion
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
