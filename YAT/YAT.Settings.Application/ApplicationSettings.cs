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
// YAT Version 2.1.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;

using MKY;
using MKY.Diagnostics;
using MKY.Settings;

#endregion

namespace YAT.Settings.Application
{
	/// <remarks>
	/// Should these application settings be static? Well, there's pro and con as so often:
	/// Pro:
	///  > Easier to access throughout the application.
	///  > Easier to load, and they can be loaded before any application 'Main' is created.
	///  > Easier for those test cases where the settings have to be changed before the test is run.
	/// Con:
	///  > Static shall be avoided whenever possible, especially if a class holds data.
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

		private static ApplicationSettingsHandler<EmptySettingsItem, LocalUserSettingsRoot, RoamingUserSettingsRoot> staticSettingsHandler;

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
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The settings have to be created before they can be accessed, ensure to call Create() and if needed also Load() before accessing the settings!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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

		/// <summary></summary>
		public static bool RoamingUserSettingsAreAvailable
		{
			get
			{
				return (staticSettingsHandler != null);
			}
		}

		/// <summary></summary>
		public static RoamingUserSettingsRoot RoamingUserSettings
		{
			get
			{
				if (staticSettingsHandler != null)
					return (staticSettingsHandler.RoamingUserSettings);
				else
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The settings have to be created before they can be accessed, ensure to call Create() and if needed also Load() before accessing the settings!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		public static string RoamingUserSettingsFilePath
		{
			get
			{
				if (staticSettingsHandler != null)
					return (staticSettingsHandler.RoamingUserSettingsFilePath);
				else
					return ("");
			}
		}

		/// <summary></summary>
		public static bool RoamingUserSettingsSuccessfullyLoadedFromFile
		{
			get
			{
				if (staticSettingsHandler != null)
					return (staticSettingsHandler.RoamingUserSettingsSuccessfullyLoadedFromFile);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public static bool RoamingUserSettingsAreCurrentlyOwnedByThisInstance
		{
			get
			{
				if (staticSettingsHandler != null)
					return (staticSettingsHandler.RoamingUserSettingsAreCurrentlyOwnedByThisInstance);
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
		/// So far, there are local and roaming user settings.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		public static bool Create(ApplicationSettingsFileAccess fileAccess)
		{
			try
			{
				staticSettingsHandler = new ApplicationSettingsHandler<EmptySettingsItem, LocalUserSettingsRoot, RoamingUserSettingsRoot>
				(
					ApplicationSettingsFileAccess.None,
					fileAccess,
					fileAccess
				);

				return (true);
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(typeof(ApplicationSettings), ex, "Exception while creating the settings handler!");

				staticSettingsHandler = null;

				return (false);
			}
		}

		/// <summary>
		/// Load settings.
		/// </summary>
		/// <remarks>
		/// So far, there are local and roaming user settings.
		/// </remarks>
		/// <returns>
		/// Returns <c>true</c> if settings could be loaded from the respective file paths,
		/// returns <c>false</c> if they could not be loaded and were set to defaults instead.
		/// </returns>
		public static bool Load()
		{
			if (staticSettingsHandler != null)
			{
				bool success;

				success  = staticSettingsHandler.LoadLocalUserSettings();
				success |= staticSettingsHandler.LoadRoamingUserSettings();

				return (success);
			}
			else
			{
				return (false);
			}
		}

		/// <summary>
		/// Save settings. To improved performance, settings shall only be saved if they have changed.
		/// </summary>
		/// <remarks>
		/// So far, there are local and roaming user settings.
		/// </remarks>
		public static bool Save()
		{
			bool success;

			success  = SaveLocalUserSettings();
			success |= SaveRoamingUserSettings();

			return (success);
		}

		/// <summary>
		/// Save local user settings. To improved performance, settings shall only be saved if they have changed.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		public static bool SaveLocalUserSettings()
		{
			if (staticSettingsHandler != null)
			{
				try
				{
					if (staticSettingsHandler.LocalUserSettings.HaveChanged)
						staticSettingsHandler.SaveLocalUserSettings();

					return (true);
				}
				catch (Exception ex)
				{
					DebugEx.WriteException(typeof(ApplicationSettings), ex, "Exception while saving the settings!");

					return (false);
				}
			}
			else
			{
				return (false);
			}
		}

		/// <summary>
		/// Save roaming user settings. To improved performance, settings shall only be saved if they have changed.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		public static bool SaveRoamingUserSettings()
		{
			if (staticSettingsHandler != null)
			{
				try
				{
					if (staticSettingsHandler.RoamingUserSettings.HaveChanged)
						staticSettingsHandler.SaveRoamingUserSettings();

					return (true);
				}
				catch (Exception ex)
				{
					DebugEx.WriteException(typeof(ApplicationSettings), ex, "Exception while saving the settings!");

					return (false);
				}
			}
			else
			{
				return (false);
			}
		}

		/// <summary>
		/// Close the application settings and release all resources.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		public static bool CloseAndDispose()
		{
			if (staticSettingsHandler != null)
			{
				bool success = true;

				try
				{
					staticSettingsHandler.Close();
				}
				catch (Exception ex)
				{
					DebugEx.WriteException(typeof(ApplicationSettings), ex, "Exception while closing the settings handler!");

					success = false;
				}

				try
				{
					staticSettingsHandler.Dispose();
				}
				catch (Exception ex)
				{
					DebugEx.WriteException(typeof(ApplicationSettings), ex, "Exception while disposing the settings handler!");

					success = false;
				}

				return (success);
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
