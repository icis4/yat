using System;
using System.Collections.Generic;
using System.Text;

using HSR.Utilities.Settings;

namespace HSR.YAT.Settings.Application
{
	public static class ApplicationSettings
	{
		//------------------------------------------------------------------------------------------
		// LocalUserSettings
		//------------------------------------------------------------------------------------------

		private static ApplicationSettingsHandler<object, LocalUserSettings, object> _settings =
			new ApplicationSettingsHandler<object, LocalUserSettings, object>(false, true, false);

		public static bool Load()
		{
			return (_settings.Load());
		}

		public static void Save()
		{
			_settings.Save();
		}

		public static LocalUserSettings LocalUser
		{
			get { return (_settings.LocalUserSettings); }
		}

		//------------------------------------------------------------------------------------------
		// ExtensionSettings
		//------------------------------------------------------------------------------------------

		private static ExtensionSettings _extensions = new ExtensionSettings();

		public static ExtensionSettings Extensions
		{
			get { return (_extensions); }
		}
	}
}
