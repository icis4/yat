using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace MKY.Utilities.Settings
{
	/// <summary></summary>
	public class DocumentSettingsHandler<TDocumentSettings>
		where TDocumentSettings : Settings, new()
	{
		private string _settingsFilePath = "";
		private TDocumentSettings _settings = default(TDocumentSettings);

		/// <summary>
		/// Handles document settings. Settings are stored in filePath
		/// <see cref="SettingsFilePath"/>
		/// </summary>
		public DocumentSettingsHandler()
		{
			_settings = SettingsDefault;
		}

		/// <summary>
		/// Handles document settings. Settings are stored in filePath
		/// <see cref="SettingsFilePath"/>
		/// </summary>
		public DocumentSettingsHandler(TDocumentSettings settings)
		{
			_settings = settings;
		}

		/// <summary>
		/// Complete path to document settings filePath.
		/// </summary>
		public string SettingsFilePath
		{
			get { return (_settingsFilePath); }
			set { _settingsFilePath = value; }
		}

		/// <summary>
		/// Returns whether the settings file path exists.
		/// </summary>
		public bool SettingsFileExists
		{
			get
			{
				if (_settingsFilePath == null)
					return (false);
				if (_settingsFilePath == "")
					return (false);
				return (System.IO.File.Exists(_settingsFilePath));
			}
		}

		/// <summary>
		/// Returns whether the settings file path is valid.
		/// </summary>
		public bool SettingsFilePathIsValid
		{
			get
			{
				if (_settingsFilePath == null)
					return (false);
				if (_settingsFilePath == "")
					return (false);
				if (System.IO.Path.GetFullPath(_settingsFilePath) == "")
					return (false);
				return (true);
			}
		}

		/// <summary>
		/// Handler to user settings, if has user settings,
		/// "null" otherwise.
		/// </summary>
		public TDocumentSettings Settings
		{
			get { return (_settings); }
		}

		/// <summary>
		/// Handler to common settings, if has common settings,
		/// "null" otherwise.
		/// </summary>
		public TDocumentSettings SettingsDefault
		{
			get { return (new TDocumentSettings()); }
		}

		/// <summary>
		/// Loads settings from <see cref="SettingsFilePath"/>
		/// or creates default settings if file path not found or not readable.
		/// </summary>
		/// <returns>
		/// Returns false if either settings could not be loaded from
		/// <see cref="SettingsFilePath"/> and have been set to defaults.
		/// </returns>
		/// <exception cref="Exception">
		/// Thrown if settings could not be created.
		/// </exception>
		public bool Load()
		{
			bool loadSuccess = true;
			object settings = null;

			settings = LoadFromFile(_settings.GetType(), _settingsFilePath);
			if (settings == null)
			{
				settings = SettingsDefault;
				loadSuccess = false;
			}
			_settings = (TDocumentSettings)settings;
			_settings.ClearChanged();

			// return load status
			return (loadSuccess);
		}

		private object LoadFromFile(Type type, string filePath)
		{
			XmlSerializer serializer = new XmlSerializer(type);

			// try to open filePath
			try
			{
				object settings = null;
				using (FileStream fs = new FileStream(filePath, FileMode.Open))
				{
					settings = serializer.Deserialize(fs);
				}
				return (settings);
			}
			catch
			{
			}

			// if nothing found, return null
			return (null);
		}

		/// <summary>
		/// Tries to save settings to <see cref="SettingsFilePath"/>.
		/// </summary>
		/// <exception cref="Exception">
		/// Thrown if settings could not be saved.
		/// </exception>
		public void Save()
		{
			Exception result = null;

			try
			{
				SaveToFile(_settings.GetType(), _settingsFilePath, _settings);
				_settings.ClearChanged();
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
				using (FileStream fs = new FileStream(filePath, FileMode.Create))
				{
					XmlSerializer serializer = new XmlSerializer(type);
					serializer.Serialize(fs, settings);
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
	}
}
