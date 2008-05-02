using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;

using MKY.Utilities.Diagnostics;
using MKY.Utilities.Xml;

namespace MKY.Utilities.Settings
{
	/// <summary>
	/// Generic class to handle standard application settings. It covers common, local user
	/// as well as roaming user settings.
	/// </summary>
	public class ApplicationSettingsHandler<TCommonSettings, TLocalUserSettings, TRoamingUserSettings>
		where TCommonSettings : new()
		where TLocalUserSettings : new()
		where TRoamingUserSettings : new()
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const string _Extension = ".xml";
		private const string _CommonFileName = "CommonSettings" + _Extension;
		private const string _LocalUserFileName = "LocalUserSettings" + _Extension;
		private const string _RoamingUserFileName = "RoamingUserSettings" + _Extension;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _hasCommonSettings = false;
		private bool _hasLocalUserSettings = false;
		private bool _hasRoamingUserSettings = false;

		private string _commonSettingsFilePath = "";
		private string _localUserSettingsFilePath = "";
		private string _roamingUserSettingsFilePath = "";

		private TCommonSettings _commonSettings = default(TCommonSettings);
		private TLocalUserSettings _localUserSettings = default(TLocalUserSettings);
		private TRoamingUserSettings _roamingUserSettings = default(TRoamingUserSettings);

		private bool _commonSettingsSuccessfullyLoaded = false;
		private bool _localUserSettingsSuccessfullyLoaded = false;
		private bool _roamingUserSettingsSuccessfullyLoaded = false;

		private bool _allSettingsSuccessfullyLoaded = false;

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
			_hasCommonSettings = hasCommonSettings;
			_hasLocalUserSettings = hasLocalUserSettings;
			_hasRoamingUserSettings = hasRoamingUserSettings;

			ResetFilePaths();

			if (_hasCommonSettings)
				_commonSettings = CommonSettingsDefault;

			if (_hasLocalUserSettings)
				_localUserSettings = LocalUserSettingsDefault;

			if (_hasRoamingUserSettings)
				_roamingUserSettings = RoamingUserSettingsDefault;
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary>
		/// Returns whether this handler has common settings.
		/// </summary>
		public bool HasCommonSettings
		{
			get { return (_hasCommonSettings); }
		}

		/// <summary>
		/// Returns whether this handler has local user settings.
		/// </summary>
		public bool HasLocalUserSettings
		{
			get { return (_hasLocalUserSettings); }
		}

		/// <summary>
		/// Returns whether this handler has user settings.
		/// </summary>
		public bool HasRoamingUserSettings
		{
			get { return (_hasRoamingUserSettings); }
		}

		/// <summary>
		/// Complete path to common settings file, if has common settings,
		/// <see cref="String.Empty"/> otherwise.
		/// </summary>
		/// <exception cref="NullReferenceException">
		/// Thrown if attempted to set file if this handler has no common settings.
		/// </exception>
		public string CommonSettingsFilePath
		{
			get { return (_commonSettingsFilePath); }
			set
			{
				if (HasCommonSettings)
					_commonSettingsFilePath = value;
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
		public string LocalUserSettingsFilePath
		{
			get { return (_localUserSettingsFilePath); }
			set
			{
				if (HasLocalUserSettings)
					_localUserSettingsFilePath = value;
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
		public string RoamingUserSettingsFilePath
		{
			get { return (_roamingUserSettingsFilePath); }
			set
			{
				if (HasRoamingUserSettings)
					_roamingUserSettingsFilePath = value;
				else
					throw (new NullReferenceException("This handler has no roaming user settings!"));
			}
		}

		/// <summary>
		/// Handler to common settings, if has common settings, "null" otherwise.
		/// </summary>
		public TCommonSettings CommonSettings
		{
			get { return (_commonSettings); }
		}

		/// <summary>
		/// Handler to local user settings, if has local user settings, "null" otherwise.
		/// </summary>
		public TLocalUserSettings LocalUserSettings
		{
			get { return (_localUserSettings); }
		}

		/// <summary>
		/// Handler to roaming user settings, if has roaming user settings, "null" otherwise.
		/// </summary>
		public TRoamingUserSettings RoamingUserSettings
		{
			get { return (_roamingUserSettings); }
		}

		/// <summary>
		/// Returns whether common settings have successfully been loaded, "false" if
		/// they was no valid settings file and they were set to their defaults.
		/// </summary>
		public bool CommonSettingsSuccessfullyLoaded
		{
			get { return (_commonSettingsSuccessfullyLoaded); }
		}

		/// <summary>
		/// Returns whether local user settings have successfully been loaded, "false" if
		/// they was no valid settings file and they were set to their defaults.
		/// </summary>
		public bool LocalUserSettingsSuccessfullyLoaded
		{
			get { return (_localUserSettingsSuccessfullyLoaded); }
		}

		/// <summary>
		/// Returns whether roaming user settings have successfully been loaded, "false" if
		/// they was no valid settings file and they were set to their defaults.
		/// </summary>
		public bool RoamingUserSettingsSuccessfullyLoaded
		{
			get { return (_roamingUserSettingsSuccessfullyLoaded); }
		}

		/// <summary>
		/// Returns whether all settings have successfully been loaded, "false" if
		/// they were no valid settings files and they were set to their defaults.
		/// </summary>
		public bool AllSettingsSuccessfullyLoaded
		{
			get { return (_allSettingsSuccessfullyLoaded); }
		}

		/// <summary>
		/// Handler to common settings, if has common settings,
		/// "null" otherwise.
		/// </summary>
		public TCommonSettings CommonSettingsDefault
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
		/// "null" otherwise.
		/// </summary>
		public TLocalUserSettings LocalUserSettingsDefault
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
		/// "null" otherwise.
		/// </summary>
		public TRoamingUserSettings RoamingUserSettingsDefault
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
		/// Resets filenames to system defaults.
		/// </summary>
		public void ResetFilePaths()
		{
			if (_hasCommonSettings)
				_commonSettingsFilePath = Application.CommonAppDataPath + Path.DirectorySeparatorChar + _CommonFileName;
			else
				_commonSettingsFilePath = "";

			if (_hasLocalUserSettings)
				_localUserSettingsFilePath = Application.LocalUserAppDataPath + Path.DirectorySeparatorChar + _LocalUserFileName;
			else
				_localUserSettingsFilePath = "";

			if (_hasRoamingUserSettings)
				_roamingUserSettingsFilePath = Application.UserAppDataPath + Path.DirectorySeparatorChar + _RoamingUserFileName;
			else
				_roamingUserSettingsFilePath = "";
		}

		/// <summary>
		/// Loads settings from <see cref="CommonSettingsFilePath"/>,
		/// <see cref="LocalUserSettingsFilePath"/> and <see cref="RoamingUserSettingsFilePath"/>
		/// or creates default settings if file not found or not readable.
		/// </summary>
		/// <returns>
		/// Returns false if either settings could not be loaded from
		/// its file path and have been set to defaults.
		/// </returns>
		/// <exception cref="Exception">
		/// Thrown if settings could not be created.
		/// </exception>
		public bool Load()
		{
			object settings = null;

			_allSettingsSuccessfullyLoaded = true;

			if (HasCommonSettings)
			{
				settings = LoadFromFile(typeof(TCommonSettings), _commonSettingsFilePath);
				_commonSettingsSuccessfullyLoaded = (settings != null);
				if (!_commonSettingsSuccessfullyLoaded)
				{
					settings = CommonSettingsDefault;
					_allSettingsSuccessfullyLoaded = false;
				}
				_commonSettings = (TCommonSettings)settings;
			}

			if (HasLocalUserSettings)
			{
				settings = LoadFromFile(typeof(TLocalUserSettings), _localUserSettingsFilePath);
				_localUserSettingsSuccessfullyLoaded = (settings != null);
				if (!_localUserSettingsSuccessfullyLoaded)
				{
					settings = LocalUserSettingsDefault;
					_allSettingsSuccessfullyLoaded = false;
				}
				_localUserSettings = (TLocalUserSettings)settings;
			}

			if (HasRoamingUserSettings)
			{
				settings = LoadFromFile(typeof(TRoamingUserSettings), _roamingUserSettingsFilePath);
				_roamingUserSettingsSuccessfullyLoaded = (settings != null);
				if (!_roamingUserSettingsSuccessfullyLoaded)
				{
					settings = RoamingUserSettingsDefault;
					_allSettingsSuccessfullyLoaded = false;
				}
				_roamingUserSettings = (TRoamingUserSettings)settings;
			}

			// immediately try to save settings to reflect current version
			try
			{
				Save();
			}
			catch
			{
			}

			// return load status
			return (_allSettingsSuccessfullyLoaded);
		}

		private object LoadFromFile(Type type, string filePath)
		{
			// try to open existing file of current version
			if (File.Exists(filePath)) // first check for file to minimize exceptions thrown
			{
				// try to open existing file with default deserialization
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
				catch
				{
				}

				// try to open existing file with tolerant deserialization
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
				catch
				{
				}
			}

			// find all valid directories of older versions
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
				catch
				{
				}
			}

			// try to open an existing file of an older version, start with most recent
			string fileName = Path.GetFileName(filePath);
			oldDirectories.Sort();
			for (int i = oldDirectories.Count - 1; i >= 0; i--)
			{
				// try to open existing file with default deserialization
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
				catch
				{
				}

				// try to open existing file with tolerant deserialization
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
				catch
				{
				}
			}

			// if nothing found, return null
			return (null);
		}

		/// <summary>
		/// Tries to save settings to <see cref="CommonSettingsFilePath"/>,
		/// <see cref="LocalUserSettingsFilePath"/> and <see cref="RoamingUserSettingsFilePath"/>.
		/// </summary>
		/// <exception cref="Exception">
		/// Thrown if settings could not be saved.
		/// </exception>
		public void Save()
		{
			Exception result = null;

			// try to save common settings
			try
			{
				SaveCommon();
			}
			catch (Exception ex)
			{
				if (result == null)
					result = ex;
			}

			// try to save local user settings
			try
			{
				SaveLocalUser();
			}
			catch (Exception ex)
			{
				if (result == null)
					result = ex;
			}

			// try to save roaming user settings
			try
			{
				SaveRoamingUser();
			}
			catch (Exception ex)
			{
				if (result == null)
					result = ex;
			}

			// throw exeption if either operation failed
			if (result != null)
				throw (result);
		}

		/// <summary>
		/// Tries to save settings to <see cref="CommonSettingsFilePath"/>.
		/// </summary>
		/// <exception cref="Exception">
		/// Thrown if settings could not be saved.
		/// </exception>
		public void SaveCommon()
		{
			if (HasCommonSettings)
				SaveToFile(typeof(TCommonSettings), _commonSettingsFilePath, _commonSettings);
		}

		/// <summary>
		/// Tries to save settings to <see cref="LocalUserSettingsFilePath"/>.
		/// </summary>
		/// <exception cref="Exception">
		/// Thrown if settings could not be saved.
		/// </exception>
		public void SaveLocalUser()
		{
			if (HasLocalUserSettings)
				SaveToFile(typeof(TLocalUserSettings), _localUserSettingsFilePath, _localUserSettings);
		}

		/// <summary>
		/// Tries to save settings to <see cref="RoamingUserSettingsFilePath"/>.
		/// </summary>
		/// <exception cref="Exception">
		/// Thrown if settings could not be saved.
		/// </exception>
		public void SaveRoamingUser()
		{
			if (HasRoamingUserSettings)
				SaveToFile(typeof(TRoamingUserSettings), _roamingUserSettingsFilePath, _roamingUserSettings);
		}

		private void SaveToFile(Type type, string filePath, object settings)
		{
			string backup = filePath + ".bak";

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
