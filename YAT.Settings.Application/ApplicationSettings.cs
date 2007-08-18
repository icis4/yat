using System;
using System.Collections.Generic;
using System.Text;

using MKY.Utilities.Settings;

namespace MKY.YAT.Settings.Application
{
	public static class ApplicationSettings
	{
		//------------------------------------------------------------------------------------------
		// LocalUserSettings
		//------------------------------------------------------------------------------------------

		private static ApplicationSettingsHandler<object, LocalUserSettingsRoot, object> _settings =
			new ApplicationSettingsHandler<object, LocalUserSettingsRoot, object>(false, true, false);

		public static bool Load()
		{
			return (_settings.Load());
		}

		public static void Save()
		{
			_settings.Save();
		}

		public static LocalUserSettingsRoot LocalUser
		{
			get { return (_settings.LocalUserSettings); }
		}
	}
}
