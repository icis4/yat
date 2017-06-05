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
// MKY Development Version 1.0.18
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
using System.Collections.Generic;
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

		#region Fields > Configuration
		//------------------------------------------------------------------------------------------
		// Fields > Configuration
		//------------------------------------------------------------------------------------------

		private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

		private ConfigurationProperty portA = new ConfigurationProperty("PortA", typeof(string), "COM1");
		private ConfigurationProperty portB = new ConfigurationProperty("PortB", typeof(string), "COM2");

		private ConfigurationProperty mtSicsDeviceA = new ConfigurationProperty("MTSicsDeviceA", typeof(string), "COM14"); // MCT
		private ConfigurationProperty mtSicsDeviceB = new ConfigurationProperty("MTSicsDeviceB", typeof(string), "COM43"); // MT

		private ConfigurationProperty tiLaunchPadDeviceA = new ConfigurationProperty("TILaunchPadDeviceA", typeof(string), "COM51");
		private ConfigurationProperty tiLaunchPadDeviceB = new ConfigurationProperty("TILaunchPadDeviceB", typeof(string), "COM52");

		private ConfigurationProperty loopbackPairs = new ConfigurationProperty("LoopbackPairs", typeof(SerialPortPairConfigurationElementCollection), null /* DefaultValue doesn't work with a collection => Must be added in constructor */);
		private ConfigurationProperty loopbackSelfs = new ConfigurationProperty("LoopbackSelfs", typeof(SerialPortConfigurationElementCollection),     null /* DefaultValue doesn't work with a collection => Must be added in constructor */);

		#endregion

		#region Fields > Auxiliary
		//------------------------------------------------------------------------------------------
		// Fields > Auxiliary
		//------------------------------------------------------------------------------------------

		private bool portAIsAvailable;
		private bool portBIsAvailable;

		private bool mtSicsDeviceAIsConnected;
		private bool mtSicsDeviceBIsConnected;

		private bool tiLaunchPadDeviceAIsConnected;
		private bool tiLaunchPadDeviceBIsConnected;

		private bool[] loopbackPairIsAvailable;
		private bool[] loopbackSelfIsAvailable;

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
			this.properties.Add(this.portA); // COM1.
			this.properties.Add(this.portB); // COM2.

			this.properties.Add(this.mtSicsDeviceA); // COM41
			this.properties.Add(this.mtSicsDeviceB); // COM42

			this.properties.Add(this.tiLaunchPadDeviceA); // COM51
			this.properties.Add(this.tiLaunchPadDeviceB); // COM52

			this.properties.Add(this.loopbackPairs);
			SerialPortPairConfigurationElementCollection pairs = (SerialPortPairConfigurationElementCollection)this["LoopbackPairs"];
			pairs.Add( "COM1",  "COM2"); // VSPE pair as configured in "\!-Tools".
			pairs.Add("COM11", "COM12"); // MCT
			pairs.Add("COM21", "COM22"); // FTDI
			pairs.Add("COM31", "COM32"); // Prolific

			this.properties.Add(this.loopbackSelfs);
			SerialPortConfigurationElementCollection selfs = (SerialPortConfigurationElementCollection)this["LoopbackSelfs"];
			selfs.Add("COM13"); // MCT
			selfs.Add("COM23"); // FTDI
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
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "MT-SICS is a name.")]
		public virtual string MTSicsDeviceA
		{
			get { return ((string)this["MTSicsDeviceA"]); }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "MT-SICS is a name.")]
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
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		public virtual SerialPortConfigurationElementCollection LoopbackSelfs
		{
			get { return ((SerialPortConfigurationElementCollection)this["LoopbackSelfs"]); }
		}

		/// <summary></summary>
		public virtual int LoopbackCount
		{
			get { return (LoopbackPairs.Count + LoopbackSelfs.Count); }
		}

		#endregion

		#region Properties > Auxiliary
		//------------------------------------------------------------------------------------------
		// Properties > Auxiliary
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Port is named with a single letter.")]
		public virtual bool PortAIsAvailable
		{
			get { return (this.portAIsAvailable); }
			set { this.portAIsAvailable = value;  }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Port is named with a single letter.")]
		public virtual bool PortBIsAvailable
		{
			get { return (this.portBIsAvailable); }
			set { this.portBIsAvailable = value;  }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "MT-SICS is a name.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Port is named with a single letter.")]
		public virtual bool MTSicsDeviceAIsConnected
		{
			get { return (this.mtSicsDeviceAIsConnected); }
			set { this.mtSicsDeviceAIsConnected = value;  }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "MT-SICS is a name.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Port is named with a single letter.")]
		public virtual bool MTSicsDeviceBIsConnected
		{
			get { return (this.mtSicsDeviceBIsConnected); }
			set { this.mtSicsDeviceBIsConnected = value;  }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Port is named with a single letter.")]
		public virtual bool TILaunchPadDeviceAIsConnected
		{
			get { return (this.tiLaunchPadDeviceAIsConnected); }
			set { this.tiLaunchPadDeviceAIsConnected = value;  }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Port is named with a single letter.")]
		public virtual bool TILaunchPadDeviceBIsConnected
		{
			get { return (this.tiLaunchPadDeviceBIsConnected); }
			set { this.tiLaunchPadDeviceBIsConnected = value;  }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Don't care, straightforward test implementation.")]
		public virtual bool[] LoopbackPairIsAvailable
		{
			get { return (this.loopbackPairIsAvailable); }
			set { this.loopbackPairIsAvailable = value; }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Don't care, straightforward test implementation.")]
		public virtual bool[] LoopbackSelfIsAvailable
		{
			get { return (this.loopbackSelfIsAvailable); }
			set { this.loopbackSelfIsAvailable = value; }
		}

		#endregion

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
			if (Provider.TryOpenAndMergeConfigurations<ConfigurationSection>(ConfigurationConstants.SelectionGroupName, ConfigurationConstants.SectionsGroupName, ConfigurationConstants.SolutionConfigurationFileNameSuffix, ConfigurationConstants.UserConfigurationEnvironmentVariableName, out configuration))
			{
				// Set which physical items are available on the current machine:
				SerialPortCollection availablePorts = new SerialPortCollection();
				availablePorts.FillWithAvailablePorts(false); // Explicitly not getting captions, thus faster.

				configuration.PortAIsAvailable = availablePorts.Contains(configuration.PortA);
				configuration.PortBIsAvailable = availablePorts.Contains(configuration.PortB);

				configuration.MTSicsDeviceAIsConnected = availablePorts.Contains(configuration.MTSicsDeviceA);
				configuration.MTSicsDeviceBIsConnected = availablePorts.Contains(configuration.MTSicsDeviceB);

				configuration.TILaunchPadDeviceAIsConnected = availablePorts.Contains(configuration.TILaunchPadDeviceA);
				configuration.TILaunchPadDeviceBIsConnected = availablePorts.Contains(configuration.TILaunchPadDeviceB);

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
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Port is named with a single letter.")]
		public static readonly string PortAIsAvailable = "Serial port A '" + ConfigurationProvider.Configuration.PortA + "' is " + (ConfigurationProvider.Configuration.PortAIsAvailable ? "" : "not ") + "available";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Port is named with a single letter.")]
		public static readonly string PortBIsAvailable = "Serial port B '" + ConfigurationProvider.Configuration.PortB + "' is " + (ConfigurationProvider.Configuration.PortBIsAvailable ? "" : "not ") + "available";

		/// <remarks>"MT-SICS" is no valid NUnit category string as it contains an '-'.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "MT-SICS is a name.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Port is named with a single letter.")]
		public static readonly string MTSicsDeviceAIsConnected = "Serial port MT SICS device A is " + (ConfigurationProvider.Configuration.MTSicsDeviceAIsConnected ? "" : "not ") + "connected to '" + ConfigurationProvider.Configuration.MTSicsDeviceA + "'";

		/// <remarks>"MT-SICS" is no valid NUnit category string as it contains an '-'.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "MT-SICS is a name.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Port is named with a single letter.")]
		public static readonly string MTSicsDeviceBIsConnected = "Serial port MT SICS device B is " + (ConfigurationProvider.Configuration.MTSicsDeviceBIsConnected ? "" : "not ") + "connected to '" + ConfigurationProvider.Configuration.MTSicsDeviceB + "'";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Port is named with a single letter.")]
		public static readonly string TILaunchPadDeviceAIsConnected = "Serial port TI LaunchPad device A is " + (ConfigurationProvider.Configuration.TILaunchPadDeviceAIsConnected ? "" : "not ") + "connected to '" + ConfigurationProvider.Configuration.TILaunchPadDeviceA + "'";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Port is named with a single letter.")]
		public static readonly string TILaunchPadDeviceBIsConnected = "Serial port TI LaunchPad device B is " + (ConfigurationProvider.Configuration.TILaunchPadDeviceBIsConnected ? "" : "not ") + "connected to '" + ConfigurationProvider.Configuration.TILaunchPadDeviceB + "'";

		/// <summary></summary>
		public static readonly string LoopbackPairsAreAvailable = "Serial port loopback pair" + ((ConfigurationProvider.Configuration.LoopbackPairs.Count > 0) ? ((ConfigurationProvider.Configuration.LoopbackPairs.Count > 1) ? "s are " : " is ") : " is not ") + "available";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		public static readonly string LoopbackSelfsAreAvailable = "Serial port loopback self" + ((ConfigurationProvider.Configuration.LoopbackSelfs.Count > 0) ? ((ConfigurationProvider.Configuration.LoopbackSelfs.Count > 1) ? "s are " : " is ") : " is not ") + "available";

		/// <summary></summary>
		public static readonly string LoopbacksAreAvailable = "Serial port loopback" + ((ConfigurationProvider.Configuration.LoopbackCount > 0) ? ((ConfigurationProvider.Configuration.LoopbackCount > 1) ? "s are " : " is ") : " is not ") + "available";
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Port is named with a single letter.")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class PortAIsAvailableCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public PortAIsAvailableCategoryAttribute()
			: base(ConfigurationCategoryStrings.PortAIsAvailable)
		{
		}
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Port is named with a single letter.")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class PortBIsAvailableCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public PortBIsAvailableCategoryAttribute()
			: base(ConfigurationCategoryStrings.PortBIsAvailable)
		{
		}
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "MT-SICS is a name.")]
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Device is named with a single letter")]
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
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "MT-SICS is a name.")]
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Device is named with a single letter")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class MTSicsDeviceBIsConnectedCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public MTSicsDeviceBIsConnectedCategoryAttribute()
			: base(ConfigurationCategoryStrings.MTSicsDeviceBIsConnected)
		{
		}
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "AIs", Justification = "Device is named with a single letter")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class TILaunchPadDeviceAIsConnectedCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public TILaunchPadDeviceAIsConnectedCategoryAttribute()
			: base(ConfigurationCategoryStrings.TILaunchPadDeviceAIsConnected)
		{
		}
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "BIs", Justification = "Device is named with a single letter")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class TILaunchPadDeviceBIsConnectedCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public TILaunchPadDeviceBIsConnectedCategoryAttribute()
			: base(ConfigurationCategoryStrings.TILaunchPadDeviceBIsConnected)
		{
		}
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class LoopbackPairsAreAvailableCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public LoopbackPairsAreAvailableCategoryAttribute()
			: base(ConfigurationCategoryStrings.LoopbackPairsAreAvailable)
		{
		}
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class LoopbackSelfsAreAvailableCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public LoopbackSelfsAreAvailableCategoryAttribute()
			: base(ConfigurationCategoryStrings.LoopbackSelfsAreAvailable)
		{
		}
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class LoopbacksAreAvailableCategoryAttribute : NUnit.Framework.CategoryAttribute
	{
		/// <summary></summary>
		public LoopbacksAreAvailableCategoryAttribute()
			: base(ConfigurationCategoryStrings.LoopbacksAreAvailable)
		{
		}
	}

	#endregion
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
