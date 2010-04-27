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

using NUnit.Framework;

namespace MKY.IO.Usb.Test
{
	/// <summary></summary>
	public static class Settings
	{
		private const string SettingRoot = "MKY.IO.Usb.Test.Settings";

		private const string SerialHidDeviceAIsAvailableSetting = SettingRoot + ".SerialHidDeviceAIsAvailable";
		private const string SerialHidDeviceBIsAvailableSetting = SettingRoot + ".SerialHidDeviceBIsAvailable";

		private const string SerialHidDeviceASetting = SettingRoot + ".SerialHidDeviceA";
		private const string SerialHidDeviceBSetting = SettingRoot + ".SerialHidDeviceB";

		public static bool SerialHidDeviceAIsAvailable = false;
		public static bool SerialHidDeviceBIsAvailable = false;

		public static string SerialHidDeviceA = "VID:0ABC PID:1234";
		public static string SerialHidDeviceB = "VID:0ABC PID:1234";

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
			MKY.Test.SettingsDefaults.AddSetting(SerialHidDeviceAIsAvailableSetting, SerialHidDeviceAIsAvailable);
			MKY.Test.SettingsDefaults.AddSetting(SerialHidDeviceBIsAvailableSetting, SerialHidDeviceBIsAvailable);

			MKY.Test.SettingsDefaults.AddSetting(SerialHidDeviceASetting, SerialHidDeviceA);
			MKY.Test.SettingsDefaults.AddSetting(SerialHidDeviceBSetting, SerialHidDeviceB);
		}

		private static void TryGetSettings()
		{
			MKY.Test.SettingsProvider.TryGetSetting(SerialHidDeviceAIsAvailableSetting, ref SerialHidDeviceAIsAvailable);
			MKY.Test.SettingsProvider.TryGetSetting(SerialHidDeviceBIsAvailableSetting, ref SerialHidDeviceBIsAvailable);

			MKY.Test.SettingsProvider.TryGetSetting(SerialHidDeviceASetting, ref SerialHidDeviceA);
			MKY.Test.SettingsProvider.TryGetSetting(SerialHidDeviceBSetting, ref SerialHidDeviceB);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
