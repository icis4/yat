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

namespace MKY.IO.Ports.Test
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
		/// Initializes a new instance of the <see cref="ConfigurationSection"/> class.
		/// </summary>
		public ConfigurationSection()
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
			if (Provider.TryOpenAndMergeConfigurations<ConfigurationSection>(ConfigurationConstants.ConfigurationGroupName, ConfigurationConstants.ConfigurationSectionsGroupName, ConfigurationConstants.SolutionConfigurationFileNameSuffix, ConfigurationConstants.UserConfigurationEnvironmentVariableName, out configuration))
			{
				// Ensure that the configured physical ports are currently indeed available:

				SerialPortCollection serialPorts = new SerialPortCollection();
				serialPorts.FillWithAvailablePorts(false);

				if (!serialPorts.Contains(configuration.SerialPortA))
					configuration.SerialPortAIsAvailable = false;

				if (!serialPorts.Contains(configuration.SerialPortB))
					configuration.SerialPortBIsAvailable = false;

				if (!serialPorts.Contains(configuration.SerialPortC))
					configuration.SerialPortCIsAvailable = false;

				if (!serialPorts.Contains(configuration.SerialPortD))
					configuration.SerialPortDIsAvailable = false;

				if (!serialPorts.Contains(configuration.SerialPortE))
					configuration.SerialPortEIsAvailable = false;

				if (!serialPorts.Contains(configuration.SerialPortF))
					configuration.SerialPortFIsAvailable = false;

				if (!serialPorts.Contains(configuration.MTSicsDeviceA))
					configuration.MTSicsDeviceAIsConnected = false;

				if (!serialPorts.Contains(configuration.MTSicsDeviceB))
					configuration.MTSicsDeviceBIsConnected = false;

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
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Ports and devices are named with a single letter")]
		public static readonly string SerialPortAIsAvailable = "Serial port " + ConfigurationProvider.Configuration.SerialPortA + " is available";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Ports and devices are named with a single letter")]
		public static readonly string SerialPortBIsAvailable = "Serial port " + ConfigurationProvider.Configuration.SerialPortB + " is available";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "CIs", Justification = "Ports and devices are named with a single letter")]
		public static readonly string SerialPortCIsAvailable = "Serial port " + ConfigurationProvider.Configuration.SerialPortC + " is available";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "DIs", Justification = "Ports and devices are named with a single letter")]
		public static readonly string SerialPortDIsAvailable = "Serial port " + ConfigurationProvider.Configuration.SerialPortD + " is available";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "EIs", Justification = "Ports and devices are named with a single letter")]
		public static readonly string SerialPortEIsAvailable = "Serial port " + ConfigurationProvider.Configuration.SerialPortE + " is available";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "FIs", Justification = "Ports and devices are named with a single letter")]
		public static readonly string SerialPortFIsAvailable = "Serial port " + ConfigurationProvider.Configuration.SerialPortF + " is available";

		/// <summary></summary>
		public static readonly string SerialPortsAreInterconnected = "Serial ports are interconnected";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Ports and devices are named with a single letter")]
		public static readonly string MTSicsDeviceAIsConnected = "MT-SICS device is connected to " + ConfigurationProvider.Configuration.MTSicsDeviceA;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Ports and devices are named with a single letter")]
		public static readonly string MTSicsDeviceBIsConnected = "MT-SICS device is connected to " + ConfigurationProvider.Configuration.MTSicsDeviceA;
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Ports and devices are named with a single letter")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class SerialPortAIsAvailableCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public SerialPortAIsAvailableCategoryAttribute()
			: base(ConfigurationCategoryStrings.SerialPortAIsAvailable)
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
			: base(ConfigurationCategoryStrings.SerialPortBIsAvailable)
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
			: base(ConfigurationCategoryStrings.SerialPortCIsAvailable)
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
			: base(ConfigurationCategoryStrings.SerialPortDIsAvailable)
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
			: base(ConfigurationCategoryStrings.SerialPortEIsAvailable)
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
			: base(ConfigurationCategoryStrings.SerialPortFIsAvailable)
		{
		}
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class SerialPortsAreInterconnectedCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public SerialPortsAreInterconnectedCategoryAttribute()
			: base(ConfigurationCategoryStrings.SerialPortsAreInterconnected)
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
			: base(ConfigurationCategoryStrings.MTSicsDeviceAIsConnected)
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
			: base(ConfigurationCategoryStrings.MTSicsDeviceBIsConnected)
		{
		}
	}

	#endregion
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
