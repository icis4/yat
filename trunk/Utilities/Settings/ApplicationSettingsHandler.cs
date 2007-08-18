using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;

namespace MKY.Utilities.Settings
{
	/// <summary></summary>
	public class ApplicationSettingsHandler<TCommonSettings, TLocalUserSettings, TRoamingUserSettings>
		where TCommonSettings : new()
		where TLocalUserSettings : new()
		where TRoamingUserSettings : new()
	{
		/// <summary></summary>
		public const string FileNameWithoutExtension = "Settings";
		/// <summary></summary>
		public const string Extension = ".xml";
		/// <summary></summary>
		public const string FileName = FileNameWithoutExtension + Extension;

		private bool _hasCommonSettings = false;
		private bool _hasLocalUserSettings = false;
		private bool _hasRoamingUserSettings = false;

		private string _commonSettingsFilePath = "";
		private string _localUserSettingsFilePath = "";
		private string _roamingUserSettingsFilePath = "";

		private TCommonSettings _commonSettings = default(TCommonSettings);
		private TLocalUserSettings _localUserSettings = default(TLocalUserSettings);
		private TRoamingUserSettings _roamingUserSettings = default(TRoamingUserSettings);

		/// <summary>
		/// Handles common and user settings. Common settings are stored in
		/// <see cref="Application.CommonAppDataPath"/>, local user settings in
		/// <see cref="Application.LocalUserAppDataPath"/>, user settings in
		/// <see cref="Application.UserAppDataPath"/> in a file named
		/// <see cref="FileName"/>
		/// </summary>
		public ApplicationSettingsHandler(bool hasCommonSettings, bool hasLocalUserSettings, bool hasRoamingUserSettings)
		{
			_hasCommonSettings = hasCommonSettings;
			_hasLocalUserSettings = hasLocalUserSettings;
			_hasRoamingUserSettings = hasRoamingUserSettings;

			ResetFileNames();

			if (_hasCommonSettings)
				_commonSettings = CommonSettingsDefault;

			if (_hasLocalUserSettings)
				_localUserSettings = LocalUserSettingsDefault;

			if (_hasRoamingUserSettings)
				_roamingUserSettings = RoamingUserSettingsDefault;
		}

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
		/// Resets filenames to system defaults.
		/// </summary>
		public void ResetFileNames()
		{
			if (_hasCommonSettings)
				_commonSettingsFilePath = Application.CommonAppDataPath + Path.DirectorySeparatorChar + FileName;
			else
				_commonSettingsFilePath = "";

			if (_hasLocalUserSettings)
				_localUserSettingsFilePath = Application.LocalUserAppDataPath + Path.DirectorySeparatorChar + FileName;
			else
				_localUserSettingsFilePath = "";

			if (_hasRoamingUserSettings)
				_roamingUserSettingsFilePath = Application.UserAppDataPath + Path.DirectorySeparatorChar + FileName;
			else
				_roamingUserSettingsFilePath = "";
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
		/// Handler to common settings, if has common settings,
		/// "null" otherwise.
		/// </summary>
		public TCommonSettings CommonSettings
		{
			get { return (_commonSettings); }
		}

		/// <summary>
		/// Handler to local user settings, if has local user settings,
		/// "null" otherwise.
		/// </summary>
		public TLocalUserSettings LocalUserSettings
		{
			get { return (_localUserSettings); }
		}

		/// <summary>
		/// Handler to user settings, if has user settings,
		/// "null" otherwise.
		/// </summary>
		public TRoamingUserSettings RoamingUserSettings
		{
			get { return (_roamingUserSettings); }
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
			bool loadSuccess = true;
			object settings = null;

			if (HasCommonSettings)
			{
				settings = LoadFromFile(typeof(TCommonSettings), _commonSettingsFilePath);
				if (settings == null)
				{
					settings = CommonSettingsDefault;
					loadSuccess = false;
				}
				_commonSettings = (TCommonSettings)settings;
			}

			if (HasLocalUserSettings)
			{
				settings = LoadFromFile(typeof(TLocalUserSettings), _localUserSettingsFilePath);
				if (settings == null)
				{
					settings = LocalUserSettingsDefault;
					loadSuccess = false;
				}
				_localUserSettings = (TLocalUserSettings)settings;
			}

			if (HasRoamingUserSettings)
			{
				settings = LoadFromFile(typeof(TRoamingUserSettings), _roamingUserSettingsFilePath);
				if (settings == null)
				{
					settings = RoamingUserSettingsDefault;
					loadSuccess = false;
				}
				_roamingUserSettings = (TRoamingUserSettings)settings;
			}

			// immediately try to save settings to reflect current version
			try
			{
				Save();
			}
			catch { }

			// return load status
			return (loadSuccess);
		}

		private object LoadFromFile(Type type, string file)
		{
			// try to open existing file of current version
			try
			{
				object settings = null;
				using (FileStream fs = new FileStream(file, FileMode.Open))
				{
					XmlSerializer serializer = new XmlSerializer(type);
					settings = serializer.Deserialize(fs);
				}
				return (settings);
			}
			catch
			{
			}

			// find all valid directories of older versions
			string productSettingsPath = Path.GetDirectoryName(Path.GetDirectoryName(file));
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

			// try to open an existing file of an older version, start with most recent
			string fileName = Path.GetFileName(file);
			oldDirectories.Sort();
			for (int i = oldDirectories.Count - 1; i >= 0; i--)
			{
				try
				{
					object settings = null;
					using (FileStream fs = new FileStream((string)oldDirectories[i] + Path.DirectorySeparatorChar + fileName, FileMode.Open))
					{
						XmlSerializer serializer = new XmlSerializer(type);
						settings = serializer.Deserialize(fs);
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
				if (HasCommonSettings)
					SaveToFile(typeof(TCommonSettings), _commonSettingsFilePath, _commonSettings);
			}
			catch (Exception ex)
			{
				if (result == null)
					result = ex;
			}

			// try to save local user settings
			try
			{
				if (HasLocalUserSettings)
					SaveToFile(typeof(TLocalUserSettings), _localUserSettingsFilePath, _localUserSettings);
			}
			catch (Exception ex)
			{
				if (result == null)
					result = ex;
			}

			// try to save user settings
			try
			{
				if (HasRoamingUserSettings)
					SaveToFile(typeof(TRoamingUserSettings), _roamingUserSettingsFilePath, _roamingUserSettings);
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

		private void SaveToFile(Type type, string file, object settings)
		{
			string backup = file + ".bak";

			try
			{
				if (File.Exists(backup))
					File.Delete(backup);
				if (File.Exists(file))
					File.Move(file, backup);
			}
			catch { }

			try
			{
				XmlSerializer serializer = new XmlSerializer(type);
				using (FileStream fs = new FileStream(file, FileMode.Create))
				{
					serializer.Serialize(fs, settings);
				}
			}
			catch (Exception ex)
			{
				try
				{
					if (File.Exists(backup))
						File.Move(backup, file);
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
	}
}
