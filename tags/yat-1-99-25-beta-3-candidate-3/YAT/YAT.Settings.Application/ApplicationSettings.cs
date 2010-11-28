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
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

using MKY.Settings;

namespace YAT.Settings.Application
{
	/// <summary></summary>
	public static class ApplicationSettings
	{
		//------------------------------------------------------------------------------------------
		// LocalUserSettings
		//------------------------------------------------------------------------------------------

		private static ApplicationSettingsHandler<object, LocalUserSettingsRoot, object> settingsHandler =
			new ApplicationSettingsHandler<object, LocalUserSettingsRoot, object>(false, true, false);

		/// <summary>
		/// Loads settings. So far, there are only local user settings.
		/// </summary>
		public static bool Load()
		{
			return (settingsHandler.LoadLocalUser());
		}

		/// <summary>
		/// Saves settings. So far, there are only local user settings.
		/// To improved performance, settings are only saved if they have changed.
		/// </summary>
		public static void Save()
		{
			if (settingsHandler.LocalUserSettings.HaveChanged)
				settingsHandler.SaveLocalUser();
		}

		/// <summary></summary>
		public static LocalUserSettingsRoot LocalUser
		{
			get { return (settingsHandler.LocalUserSettings); }
		}

		/// <summary></summary>
		public static bool LocalUserSettingsSuccessfullyLoaded
		{
			get { return (settingsHandler.LocalUserSettingsSuccessfullyLoaded); }
		}

		/// <summary></summary>
		public static string LocalUserSettingsFilePath
		{
			get { return (settingsHandler.LocalUserSettingsFilePath); }
		}

		/// <summary></summary>
		public static bool SettingsSuccessfullyLoaded
		{
			get { return (settingsHandler.LocalUserSettingsSuccessfullyLoaded); }
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
