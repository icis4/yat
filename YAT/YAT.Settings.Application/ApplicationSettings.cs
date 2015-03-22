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
// YAT 2.0 Beta 4 Candidate 3 Development Version 1.99.31
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;

using MKY.Settings;

namespace YAT.Settings.Application
{
	/// <remarks>
	/// Should these application settings be static? Well, there's pro and con as so often:
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

		private static ApplicationSettingsHandler<EmptySettingsItem, LocalUserSettingsRoot, EmptySettingsItem> staticSettingsHandler;

		#endregion

		#region Static Properties
		//==========================================================================================
		// Static Properties
		//==========================================================================================

		/// <summary></summary>
		public static bool LocalUserSettingsAreAvailable
		{
			get
			{
				return (staticSettingsHandler != null);
			}
		}

		/// <summary></summary>
		public static LocalUserSettingsRoot LocalUserSettings
		{
			get
			{
				if (staticSettingsHandler != null)
					return (staticSettingsHandler.LocalUserSettings);
				else
					throw (new InvalidOperationException("The settings have to be created before they can be accessed, ensure to call Create() and if needed also Load() before accessing the settings."));
			}
		}

		/// <summary></summary>
		public static string LocalUserSettingsFilePath
		{
			get
			{
				if (staticSettingsHandler != null)
					return (staticSettingsHandler.LocalUserSettingsFilePath);
				else
					return ("");
			}
		}

		/// <summary></summary>
		public static bool LocalUserSettingsSuccessfullyLoadedFromFile
		{
			get
			{
				if (staticSettingsHandler != null)
					return (staticSettingsHandler.LocalUserSettingsSuccessfullyLoadedFromFile);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public static bool LocalUserSettingsAreCurrentlyOwnedByThisInstance
		{
			get
			{
				if (staticSettingsHandler != null)
					return (staticSettingsHandler.LocalUserSettingsAreCurrentlyOwnedByThisInstance);
				else
					return (false);
			}
		}

		#endregion

		#region Static Methods
		//==========================================================================================
		// Static Methods
		//==========================================================================================

		/// <summary>
		/// Create settings.
		/// </summary>
		/// <remarks>
		/// So far, there are only local user settings.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		public static bool Create(ApplicationSettingsFileAccess fileAccess)
		{
			try
			{
				staticSettingsHandler = new ApplicationSettingsHandler<EmptySettingsItem, LocalUserSettingsRoot, EmptySettingsItem>
				(
					ApplicationSettingsFileAccess.None,
					fileAccess,
					ApplicationSettingsFileAccess.None
				);

				return (true);
			}
			catch
			{
				staticSettingsHandler = null;

				return (false);
			}
		}

		/// <summary>
		/// Load settings.
		/// </summary>
		/// <remarks>
		/// So far, there are only local user settings.
		/// </remarks>
		/// <returns>
		/// Returns <c>true</c> if settings could be loaded from the respective file paths,
		/// return <c>false</c> if they could not be loaded and were set to defaults instead.
		/// </returns>
		public static bool Load()
		{
			if (staticSettingsHandler != null)
				return (staticSettingsHandler.LoadLocalUserSettings());
			else
				return (false);
		}

		/// <summary>
		/// Save settings. To improved performance, settings are only saved if they have changed.
		/// </summary>
		/// <remarks>
		/// So far, there are only local user settings.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
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
		/// Close settings.
		/// </summary>
		/// <remarks>
		/// So far, there are only local user settings.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
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
