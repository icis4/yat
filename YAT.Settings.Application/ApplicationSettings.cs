using System;
using System.Collections.Generic;
using System.Text;

using MKY.Utilities.Settings;

namespace YAT.Settings.Application
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

		/// <summary>
		/// Saves local user settings. To improved performance, settings are only saved
		/// if they have changed.
		/// </summary>
		public static void SaveLocalUser()
		{
			if (_settings.LocalUserSettings.HaveChanged)
				_settings.SaveLocalUser();
		}

		public static LocalUserSettingsRoot LocalUser
		{
			get { return (_settings.LocalUserSettings); }
		}
	}
}
