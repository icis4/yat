//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Collections.Generic;
using System.Configuration;

using NUnit.Framework;

namespace MKY.IO.Ports.Test
{
	public class SettingsSection : ConfigurationSection
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private ConfigurationPropertyCollection properties;

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

		public SettingsSection()
		{
			this.properties = new ConfigurationPropertyCollection();

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

		protected override ConfigurationPropertyCollection Properties
		{
			get { return (this.properties); }
		}

		public bool SerialPortAIsAvailable
		{
			get { return (bool)this["SerialPortAIsAvailable"]; }
			set
			{
				AssertNotReadOnly("SerialPortAIsAvailable");
				this["SerialPortAIsAvailable"] = value;
			}
		}

		public bool SerialPortBIsAvailable
		{
			get { return (bool)this["SerialPortBIsAvailable"]; }
			set
			{
				AssertNotReadOnly("SerialPortBIsAvailable");
				this["SerialPortBIsAvailable"] = value;
			}
		}

		public string SerialPortA
		{
			get { return (string)this["SerialPortA"]; }
			set
			{
				AssertNotReadOnly("SerialPortA");
				this["SerialPortA"] = value;
			}
		}

		public string SerialPortB
		{
			get { return (string)this["SerialPortB"]; }
			set
			{
				AssertNotReadOnly("SerialPortB");
				this["SerialPortB"] = value;
			}
		}

		public bool SerialPortsAreInterconnected
		{
			get { return (bool)this["SerialPortsAreInterconnected"]; }
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

	/// <summary></summary>
	public static class Settings
	{
		private const string SettingRoot = "MKY.IO.Ports.Test.Settings";

		private const string SerialPortAIsAvailableSetting = SettingRoot + ".SerialPortAIsAvailable";
		private const string SerialPortBIsAvailableSetting = SettingRoot + ".SerialPortBIsAvailable";

		private const string SerialPortASetting = SettingRoot + ".SerialPortA";
		private const string SerialPortBSetting = SettingRoot + ".SerialPortB";

		private const string SerialPortsAreInterconnectedSetting = SettingRoot + ".SerialPortsAreInterconnected";

		public static bool SerialPortAIsAvailable = false;
		public static bool SerialPortBIsAvailable = false;

		public static string SerialPortA = "COM1";
		public static string SerialPortB = "COM2";

		public static bool SerialPortsAreInterconnected = false;

		static Settings()
		{
			if (MKY.Test.SettingsModeProvider.Mode == MKY.Test.SettingsMode.CreateDefaultSolutionFile)
				InitializeDefaults();
			else
				TryGetSettings();
		}

		public static void ForceStaticInitialization()
		{
			// Do nothing, call will force initialization of static fields and call of static constructor.
		}

		private static void InitializeDefaults()
		{
			MKY.Test.SettingsDefaults.AddSetting(SerialPortAIsAvailableSetting, SerialPortAIsAvailable);
			MKY.Test.SettingsDefaults.AddSetting(SerialPortBIsAvailableSetting, SerialPortBIsAvailable);

			MKY.Test.SettingsDefaults.AddSetting(SerialPortASetting, SerialPortA);
			MKY.Test.SettingsDefaults.AddSetting(SerialPortBSetting, SerialPortB);

			MKY.Test.SettingsDefaults.AddSetting(SerialPortsAreInterconnectedSetting, SerialPortsAreInterconnected);
		}

		private static void TryGetSettings()
		{
			MKY.Test.SettingsProvider.TryGetSetting(SerialPortAIsAvailableSetting, ref SerialPortAIsAvailable);
			MKY.Test.SettingsProvider.TryGetSetting(SerialPortBIsAvailableSetting, ref SerialPortBIsAvailable);

			MKY.Test.SettingsProvider.TryGetSetting(SerialPortASetting, ref SerialPortA);
			MKY.Test.SettingsProvider.TryGetSetting(SerialPortBSetting, ref SerialPortB);

			MKY.Test.SettingsProvider.TryGetSetting(SerialPortsAreInterconnectedSetting, ref SerialPortsAreInterconnected);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
