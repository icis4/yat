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
// YAT 2.0 Beta 4 Candidate 2 Development Version 1.99.29
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
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
	/// <remarks>
	/// Should these application settings be static? Well, there's pro and con as often:
	/// Pro:
	///  > Easier to access throughout the application.
	///  > Easier to load, and they can be loaded before any application 'Main' is created.
	///  > Easier for those test cases where the settings have to be changed before the test is run.
	/// Con:
	///  > Static shall be avoided whenever possible, especially if the class hold data.
	///  > Hardly possible to automatically test concurrent applications by NUnit test cases:
	///    > The easy way would simply be to create two 'Main' objects and run them in parallel but
	///      then both objects share the same static application settings object which isn't the
	///      case if they're loaded in separate processes.
	///    > The alternative is to do these test cases manually...
	/// </remarks>
	public static class ApplicationSettings
	{
		#region Static Fields
		//==========================================================================================
		// Static Fields
		//==========================================================================================

		private static ApplicationSettingsHandler<object, LocalUserSettingsRoot, object> staticSettingsHandler =
			new ApplicationSettingsHandler<object, LocalUserSettingsRoot, object>(false, true, false);

		#endregion

		#region Static Properties
		//==========================================================================================
		// Static Properties
		//==========================================================================================

		/// <summary></summary>
		public static LocalUserSettingsRoot LocalUserSettings
		{
			get { return (staticSettingsHandler.LocalUserSettings); }
		}

		/// <summary></summary>
		public static bool LocalUserSettingsSuccessfullyLoaded
		{
			get { return (staticSettingsHandler.LocalUserSettingsSuccessfullyLoaded); }
		}

		/// <summary></summary>
		public static bool LocalUserSettingsAreCurrentlyOwnedByThisInstance
		{
			get { return (staticSettingsHandler.LocalUserSettingsAreCurrentlyOwnedByThisInstance); }
		}

		/// <summary></summary>
		public static string LocalUserSettingsFilePath
		{
			get { return (staticSettingsHandler.LocalUserSettingsFilePath); }
		}

		/// <summary></summary>
		public static bool SettingsSuccessfullyLoaded
		{
			get { return (staticSettingsHandler.LocalUserSettingsSuccessfullyLoaded); }
		}

		#endregion

		#region Static Methods
		//==========================================================================================
		// Static Methods
		//==========================================================================================

		/// <summary>
		/// Create settings. So far, there are only local user settings.
		/// </summary>
		public static bool Create()
		{
			try
			{
				staticSettingsHandler = new ApplicationSettingsHandler<object, LocalUserSettingsRoot, object>(false, true, false);
				return (true);
			}
			catch
			{
				staticSettingsHandler = null;
				return (false);
			}
		}

		/// <summary>
		/// Load settings. So far, there are only local user settings.
		/// </summary>
		public static bool Load()
		{
			if (staticSettingsHandler != null)
				return (staticSettingsHandler.LoadLocalUserSettings());
			else
				return (false);
		}

		/// <summary>
		/// Save settings. So far, there are only local user settings.
		/// To improved performance, settings are only saved if they have changed.
		/// </summary>
		public static bool Save()
		{
			if (staticSettingsHandler != null)
			{
				try
				{
					if (staticSettingsHandler.LocalUserSettings.HaveChanged)
						staticSettingsHandler.SaveLocalUserSettings();

					return (true);
				}
				catch
				{
					return (false);
				}
			}
			else
			{
				return (false);
			}
		}

		/// <summary>
		/// Close settings. So far, there are only local user settings.
		/// </summary>
		public static bool Close()
		{
			if (staticSettingsHandler != null)
			{
				try
				{
					staticSettingsHandler.Close();
					return (true);
				}
				catch
				{
					return (false);
				}
			}
			else
			{
				return (false);
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
