//==================================================================================================
// URL       : $URL$
// Author    : $Author$
// Date      : $Date$
// Revision  : $Rev$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2009 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

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

		private static ApplicationSettingsHandler<object, LocalUserSettingsRoot, object> _settingsHandler =
			new ApplicationSettingsHandler<object, LocalUserSettingsRoot, object>(false, true, false);

		public static bool Load()
		{
			return (_settingsHandler.Load());
		}

		/// <summary>
		/// Saves local user settings. To improved performance, settings are only saved
		/// if they have changed.
		/// </summary>
		public static void SaveLocalUser()
		{
			if (_settingsHandler.LocalUserSettings.HaveChanged)
				_settingsHandler.SaveLocalUser();
		}

		public static LocalUserSettingsRoot LocalUser
		{
			get { return (_settingsHandler.LocalUserSettings); }
		}

		public static bool LocalUserSettingsSuccessfullyLoaded
		{
			get { return (_settingsHandler.LocalUserSettingsSuccessfullyLoaded); }
		}

		public static string LocalUserSettingsFilePath
		{
			get { return (_settingsHandler.LocalUserSettingsFilePath); }
		}

		public static bool AllSettingsSuccessfullyLoaded
		{
			get { return (_settingsHandler.AllSettingsSuccessfullyLoaded); }
		}
	}
}

//==================================================================================================
// End of $URL$
//==================================================================================================
