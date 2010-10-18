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
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

using MKY.Xml;

namespace MKY.Settings
{
	/// <summary>
	/// Generic class to handle standard application settings. It covers common, local user
	/// as well as roaming user settings.
	/// </summary>
	/// <typeparam name="TCommonSettings">The type of the common settings.</typeparam>
	/// <typeparam name="TLocalUserSettings">The type of the local user settings.</typeparam>
	/// <typeparam name="TRoamingUserSettings">The type of the roaming user settings.</typeparam>
	public class ApplicationSettingsHandler<TCommonSettings, TLocalUserSettings, TRoamingUserSettings>
		where TCommonSettings : new()
		where TLocalUserSettings : new()
		where TRoamingUserSettings : new()
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const string Extension = ".xml";
		private const string CommonFileName = "CommonSettings" + Extension;
		private const string LocalUserFileName = "LocalUserSettings" + Extension;
		private const string RoamingUserFileName = "RoamingUserSettings" + Extension;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool hasCommonSettings = false;
		private bool hasLocalUserSettings = false;
		private bool hasRoamingUserSettings = false;

		private string commonSettingsFilePath = "";
		private string localUserSettingsFilePath = "";
		private string roamingUserSettingsFilePath = "";

		private TCommonSettings commonSettings = default(TCommonSettings);
		private TLocalUserSettings localUserSettings = default(TLocalUserSettings);
		private TRoamingUserSettings roamingUserSettings = default(TRoamingUserSettings);

		private bool commonSettingsSuccessfullyLoaded = false;
		private bool localUserSettingsSuccessfullyLoaded = false;
		private bool roamingUserSettingsSuccessfullyLoaded = false;

		private bool allSettingsSuccessfullyLoaded = false;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary>
		/// Handles common and user settings. Common settings are stored in
		/// <see cref="Application.CommonAppDataPath"/>, local user settings in
		/// <see cref="Application.LocalUserAppDataPath"/>, user settings in
		/// <see cref="Application.UserAppDataPath"/>.
		/// </summary>
		public ApplicationSettingsHandler(bool hasCommonSettings, bool hasLocalUserSettings, bool hasRoamingUserSettings)
		{
			this.hasCommonSettings = hasCommonSettings;
			this.hasLocalUserSettings = hasLocalUserSettings;
			this.hasRoamingUserSettings = hasRoamingUserSettings;

			ResetFilePaths();

			if (this.hasCommonSettings)
				this.commonSettings = CommonSettingsDefault;

			if (this.hasLocalUserSettings)
				this.localUserSettings = LocalUserSettingsDefault;

			if (this.hasRoamingUserSettings)
				this.roamingUserSettings = RoamingUserSettingsDefault;
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary>
		/// Returns whether this handler has common settings.
		/// </summary>
		public virtual bool HasCommonSettings
		{
			get { return (this.hasCommonSettings); }
		}

		/// <summary>
		/// Returns whether this handler has local user settings.
		/// </summary>
		public virtual bool HasLocalUserSettings
		{
			get { return (this.hasLocalUserSettings); }
		}

		/// <summary>
		/// Returns whether this handler has user settings.
		/// </summary>
		public virtual bool HasRoamingUserSettings
		{
			get { return (this.hasRoamingUserSettings); }
		}

		/// <summary>
		/// Complete path to common settings file, if has common settings,
		/// <see cref="String.Empty"/> otherwise.
		/// </summary>
		/// <exception cref="NullReferenceException">
		/// Thrown if attempted to set file if this handler has no common settings.
		/// </exception>
		public virtual string CommonSettingsFilePath
		{
			get { return (this.commonSettingsFilePath); }
			set
			{
				if (HasCommonSettings)
					this.commonSettingsFilePath = value;
				else
					throw (new NullReferenceException("This handler has no common settings!"));
			}
		}

		/// <summary>
		/// Complete path to local user settings file, if has local user settings,
		/// <see cref="String.Empty"/> otherwise.
		/// </summary>
		/// <exception cref="NullReferenceException">
		/// Thrown if attempted to set file if this handler has no local user settings.
		/// </exception>
		public virtual string LocalUserSettingsFilePath
		{
			get { return (this.localUserSettingsFilePath); }
			set
			{
				if (HasLocalUserSettings)
					this.localUserSettingsFilePath = value;
				else
					throw (new NullReferenceException("This handler has no local user settings!"));
			}
		}

		/// <summary>
		/// Complete path to roaming user settings file, if has roaming user settings,
		/// <see cref="String.Empty"/> otherwise.
		/// </summary>
		/// <exception cref="NullReferenceException">
		/// Thrown if attempted to set file if this handler has no roaming user settings.
		/// </exception>
		public virtual string RoamingUserSettingsFilePath
		{
			get { return (this.roamingUserSettingsFilePath); }
			set
			{
				if (HasRoamingUserSettings)
					this.roamingUserSettingsFilePath = value;
				else
					throw (new NullReferenceException("This handler has no roaming user settings!"));
			}
		}

		/// <summary>
		/// Handler to common settings, if has common settings, <c>null</c> otherwise.
		/// </summary>
		public virtual TCommonSettings CommonSettings
		{
			get { return (this.commonSettings); }
		}

		/// <summary>
		/// Handler to local user settings, if has local user settings, <c>null</c> otherwise.
		/// </summary>
		public virtual TLocalUserSettings LocalUserSettings
		{
			get { return (this.localUserSettings); }
		}

		/// <summary>
		/// Handler to roaming user settings, if has roaming user settings, <c>null</c> otherwise.
		/// </summary>
		public virtual TRoamingUserSettings RoamingUserSettings
		{
			get { return (this.roamingUserSettings); }
		}

		/// <summary>
		/// Returns whether common settings have successfully been loaded, <c>false</c> if
		/// they was no valid settings file and they were set to their defaults.
		/// </summary>
		public virtual bool CommonSettingsSuccessfullyLoaded
		{
			get { return (this.commonSettingsSuccessfullyLoaded); }
		}

		/// <summary>
		/// Returns whether local user settings have successfully been loaded, <c>false</c> if
		/// they was no valid settings file and they were set to their defaults.
		/// </summary>
		public virtual bool LocalUserSettingsSuccessfullyLoaded
		{
			get { return (this.localUserSettingsSuccessfullyLoaded); }
		}

		/// <summary>
		/// Returns whether roaming user settings have successfully been loaded, <c>false</c> if
		/// they was no valid settings file and they were set to their defaults.
		/// </summary>
		public virtual bool RoamingUserSettingsSuccessfullyLoaded
		{
			get { return (this.roamingUserSettingsSuccessfullyLoaded); }
		}

		/// <summary>
		/// Returns whether all settings have successfully been loaded, <c>false</c> if
		/// they were no valid settings files and they were set to their defaults.
		/// </summary>
		public virtual bool AllSettingsSuccessfullyLoaded
		{
			get { return (this.allSettingsSuccessfullyLoaded); }
		}

		/// <summary>
		/// Handler to common settings, if has common settings,
		/// <c>null</c> otherwise.
		/// </summary>
		public virtual TCommonSettings CommonSettingsDefault
		{
			get
			{
				if (HasCommonSettings)
					return (new TCommonSettings());
				else
					return (default(TCommonSettings));
			}
		}

		/// <summary>
		/// Handler to local user settings defaults, if has local user settings,
		/// <c>null</c> otherwise.
		/// </summary>
		public virtual TLocalUserSettings LocalUserSettingsDefault
		{
			get
			{
				if (HasLocalUserSettings)
					return (new TLocalUserSettings());
				else
					return (default(TLocalUserSettings));
			}
		}

		/// <summary>
		/// Handler to user settings defaults, if has user settings,
		/// <c>null</c> otherwise.
		/// </summary>
		public virtual TRoamingUserSettings RoamingUserSettingsDefault
		{
			get
			{
				if (HasRoamingUserSettings)
					return (new TRoamingUserSettings());
				else
					return (default(TRoamingUserSettings));
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary>
		/// Resets file names to system defaults.
		/// </summary>
		public virtual void ResetFilePaths()
		{
			if (this.hasCommonSettings)
				this.commonSettingsFilePath = Application.CommonAppDataPath + Path.DirectorySeparatorChar + CommonFileName;
			else
				this.commonSettingsFilePath = "";

			if (this.hasLocalUserSettings)
				this.localUserSettingsFilePath = Application.LocalUserAppDataPath + Path.DirectorySeparatorChar + LocalUserFileName;
			else
				this.localUserSettingsFilePath = "";

			if (this.hasRoamingUserSettings)
				this.roamingUserSettingsFilePath = Application.UserAppDataPath + Path.DirectorySeparatorChar + RoamingUserFileName;
			else
				this.roamingUserSettingsFilePath = "";
		}

		/// <summary>
		/// Loads settings from <see cref="CommonSettingsFilePath"/>,
		/// <see cref="LocalUserSettingsFilePath"/> and <see cref="RoamingUserSettingsFilePath"/>
		/// or creates default settings if file not found or not readable.
		/// </summary>
		/// <returns>
		/// Returns false if either settings could not be loaded from its file path and have been
		/// set to defaults.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		public virtual bool Load()
		{
			this.allSettingsSuccessfullyLoaded = true;

			if (!LoadCommon())
				this.allSettingsSuccessfullyLoaded = false;

			if (!LoadLocalUser())
				this.allSettingsSuccessfullyLoaded = false;

			if (!LoadRoamingUser())
				this.allSettingsSuccessfullyLoaded = false;

			// Immediately try to save settings to reflect current version.
			try
			{
				Save();
			}
			catch { }

			// Return load result.
			return (this.allSettingsSuccessfullyLoaded);
		}

		/// <summary>
		/// Tries to load settings from <see cref="CommonSettingsFilePath"/>.
		/// </summary>
		/// <returns>
		/// Returns false if either settings could not be loaded from
		/// its file path and have been set to defaults.
		/// </returns>
		public virtual bool LoadCommon()
		{
			bool result = true;
			if (HasCommonSettings)
			{
				object settings = LoadFromFile(typeof(TCommonSettings), this.commonSettingsFilePath);
				this.commonSettingsSuccessfullyLoaded = (settings != null);
				if (!this.commonSettingsSuccessfullyLoaded)
				{
					settings = CommonSettingsDefault;
					result = false;
				}
				this.commonSettings = (TCommonSettings)settings;
			}
			return (result);
		}

		/// <summary>
		/// Tries to load settings from <see cref="LocalUserSettingsFilePath"/>.
		/// </summary>
		/// <returns>
		/// Returns false if either settings could not be loaded from
		/// its file path and have been set to defaults.
		/// </returns>
		public virtual bool LoadLocalUser()
		{
			bool result = true;
			if (HasLocalUserSettings)
			{
				object settings = LoadFromFile(typeof(TLocalUserSettings), this.localUserSettingsFilePath);
				this.localUserSettingsSuccessfullyLoaded = (settings != null);
				if (!this.localUserSettingsSuccessfullyLoaded)
				{
					settings = LocalUserSettingsDefault;
					result = false;
				}
				this.localUserSettings = (TLocalUserSettings)settings;
			}
			return (result);
		}

		/// <summary>
		/// Tries to load settings from <see cref="RoamingUserSettingsFilePath"/>.
		/// </summary>
		/// <returns>
		/// Returns false if either settings could not be loaded from
		/// its file path and have been set to defaults.
		/// </returns>
		public virtual bool LoadRoamingUser()
		{
			bool result = true;
			if (HasRoamingUserSettings)
			{
				object settings = LoadFromFile(typeof(TRoamingUserSettings), this.roamingUserSettingsFilePath);
				this.roamingUserSettingsSuccessfullyLoaded = (settings != null);
				if (!this.roamingUserSettingsSuccessfullyLoaded)
				{
					settings = RoamingUserSettingsDefault;
					result = false;
				}
				this.roamingUserSettings = (TRoamingUserSettings)settings;
			}
			return (result);
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		private object LoadFromFile(Type type, string filePath)
		{
			// Try to open existing file of current version.
			if (File.Exists(filePath)) // First check for file to minimize exceptions thrown.
			{
				// Try to open existing file with default deserialization.
				try
				{
					object settings = null;
					using (StreamReader sr = new StreamReader(filePath))
					{
						XmlSerializer serializer = new XmlSerializer(type);
						settings = serializer.Deserialize(sr);
					}
					return (settings);
				}
				catch { }

				// Try to open existing file with tolerant deserialization.
				try
				{
					object settings = null;
					using (StreamReader sr = new StreamReader(filePath))
					{
						TolerantXmlSerializer serializer = new TolerantXmlSerializer(type);
						settings = serializer.Deserialize(sr);
					}
					return (settings);
				}
				catch { }
			}

			// Find all valid directories of older versions.
			string productSettingsPath = Path.GetDirectoryName(Path.GetDirectoryName(filePath));
			string[] allDirectories = Directory.GetDirectories(productSettingsPath);
			List<string> oldDirectories = new List<string>();
			Version currentVersion = new Version(Application.ProductVersion);
			foreach (string directory in allDirectories)
			{
				try
				{
					Version version = new Version(Path.GetFileName(directory));
					if (version < currentVersion)
						oldDirectories.Add(directory);
				}
				catch { }
			}

			// Try to open an existing file of an older version, start with most recent.
			string fileName = Path.GetFileName(filePath);
			oldDirectories.Sort();
			for (int i = oldDirectories.Count - 1; i >= 0; i--)
			{
				// Try to open existing file with default deserialization.
				try
				{
					object settings = null;
					using (StreamReader sr = new StreamReader((string)oldDirectories[i] + Path.DirectorySeparatorChar + fileName))
					{
						XmlSerializer serializer = new XmlSerializer(type);
						settings = serializer.Deserialize(sr);
					}
					return (settings);
				}
				catch { }

				// Try to open existing file with tolerant deserialization.
				try
				{
					object settings = null;
					using (StreamReader sr = new StreamReader(filePath))
					{
						TolerantXmlSerializer serializer = new TolerantXmlSerializer(type);
						settings = serializer.Deserialize(sr);
					}
					return (settings);
				}
				catch { }
			}

			// If nothing found, return <c>null</c>.
			return (null);
		}

		/// <summary>
		/// Tries to save settings to <see cref="CommonSettingsFilePath"/>,
		/// <see cref="LocalUserSettingsFilePath"/> and <see cref="RoamingUserSettingsFilePath"/>.
		/// </summary>
		/// <exception cref="Exception">
		/// Thrown if settings could not be saved.
		/// </exception>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		public virtual void Save()
		{
			Exception result = null;

			// Try to save common settings.
			try
			{
				SaveCommon();
			}
			catch (Exception ex)
			{
				if (result == null)
					result = ex;
			}

			// Try to save local user settings.
			try
			{
				SaveLocalUser();
			}
			catch (Exception ex)
			{
				if (result == null)
					result = ex;
			}

			// Try to save roaming user settings.
			try
			{
				SaveRoamingUser();
			}
			catch (Exception ex)
			{
				if (result == null)
					result = ex;
			}

			// Throw exeption if either operation failed.
			if (result != null)
				throw (result);
		}

		/// <summary>
		/// Tries to save settings to <see cref="CommonSettingsFilePath"/>.
		/// </summary>
		/// <exception cref="Exception">
		/// Thrown if settings could not be saved.
		/// </exception>
		public virtual void SaveCommon()
		{
			if (HasCommonSettings)
				SaveToFile(typeof(TCommonSettings), this.commonSettingsFilePath, this.commonSettings);
		}

		/// <summary>
		/// Tries to save settings to <see cref="LocalUserSettingsFilePath"/>.
		/// </summary>
		/// <exception cref="Exception">
		/// Thrown if settings could not be saved.
		/// </exception>
		public virtual void SaveLocalUser()
		{
			if (HasLocalUserSettings)
				SaveToFile(typeof(TLocalUserSettings), this.localUserSettingsFilePath, this.localUserSettings);
		}

		/// <summary>
		/// Tries to save settings to <see cref="RoamingUserSettingsFilePath"/>.
		/// </summary>
		/// <exception cref="Exception">
		/// Thrown if settings could not be saved.
		/// </exception>
		public virtual void SaveRoamingUser()
		{
			if (HasRoamingUserSettings)
				SaveToFile(typeof(TRoamingUserSettings), this.roamingUserSettingsFilePath, this.roamingUserSettings);
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		private void SaveToFile(Type type, string filePath, object settings)
		{
			string backup = filePath + IO.XFile.BackupFileExtension;

			try
			{
				if (File.Exists(backup))
					File.Delete(backup);
				if (File.Exists(filePath))
					File.Move(filePath, backup);
			}
			catch { }

			try
			{
				using (StreamWriter sw = new StreamWriter(filePath))
				{
					XmlSerializer serializer = new XmlSerializer(type);
					serializer.Serialize(sw, settings);
				}
			}
			catch (Exception ex)
			{
				try
				{
					if (File.Exists(backup))
						File.Move(backup, filePath);
				}
				catch { }

				throw (ex);
			}
			finally
			{
				try
				{
					if (File.Exists(backup))
						File.Delete(backup);
				}
				catch { }
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
