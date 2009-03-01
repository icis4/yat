using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;

using MKY.Utilities.Diagnostics;
using MKY.Utilities.Xml;

namespace MKY.Utilities.Settings
{
	/// <summary>
	/// Generic class to handle any kind of document settings, e.g. MDI application settings.
	/// </summary>
	public class DocumentSettingsHandler<TDocumentSettings>
		where TDocumentSettings : Settings, new()
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private string _settingsFilePath = "";
		private string _accessedSettingsFilePath = "";

		private TDocumentSettings _settings = default(TDocumentSettings);

		private AlternateXmlElement[] _alternateXmlElements = null;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary>
		/// Handles document settings. Settings are stored in filePath
		/// <see cref="SettingsFilePath"/>
		/// </summary>
		public DocumentSettingsHandler()
		{
			Initialize(SettingsDefault);
		}

		/// <summary>
		/// Handles document settings. Settings are stored in filePath
		/// <see cref="SettingsFilePath"/>
		/// </summary>
		public DocumentSettingsHandler(TDocumentSettings settings)
		{
			Initialize(settings);
		}

		private void Initialize(TDocumentSettings settings)
		{
			_settings = settings;

			IAlternateXmlElementProvider aep = settings	as IAlternateXmlElementProvider;
			if (aep != null)
				_alternateXmlElements = aep.AlternateXmlElements;
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary>
		/// Complete path to document settings filePath.
		/// </summary>
		public string SettingsFilePath
		{
			get { return (_settingsFilePath); }
			set { _settingsFilePath = value; }
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
		/// Returns whether the settings file exists.
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
		/// Returns whether the settings file is up to date.
		/// </summary>
		public bool SettingsFileIsUpToDate
		{
			get
			{
				if (_settingsFilePath == null)
					return (false);
				if (_settingsFilePath == "")
					return (false);
				if (!System.IO.File.Exists(_settingsFilePath))
					return (false);

				// return whether current settings file path is still the same as the last access
				return (_settingsFilePath == _accessedSettingsFilePath);
			}
		}

		/// <summary>
		/// Handler to settings.
		/// </summary>
		public TDocumentSettings Settings
		{
			get { return (_settings); }
		}

		/// <summary>
		/// Handler to settings default.
		/// </summary>
		public TDocumentSettings SettingsDefault
		{
			get { return (new TDocumentSettings()); }
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary>
		/// Loads settings from <see cref="SettingsFilePath"/>
		/// or creates default settings if file path not found or not readable.
		/// </summary>
		/// <returns>
		/// Returns false if settings could not be loaded from
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
			else
			{
				_accessedSettingsFilePath = _settingsFilePath;
			}
			_settings = (TDocumentSettings)settings;
			_settings.ClearChanged();

			// return load status
			return (loadSuccess);
		}

		private object LoadFromFile(Type type, string filePath)
		{
			if (File.Exists(filePath)) // first check for file to minimize exceptions thrown
			{
				// Try to open existing file with default deserialization
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

				// Try to open existing file with tolerant & alternate-tolerant deserialization
				try
				{
					object settings = null;
					using (StreamReader sr = new StreamReader(filePath))
					{
						AlternateTolerantXmlSerializer serializer = new AlternateTolerantXmlSerializer(type, _alternateXmlElements);
						settings = serializer.Deserialize(sr);
					}
					return (settings);
				}
				catch (Exception ex)
				{
					XDebug.WriteException(this, ex);
				}
			}

			// If not successful, return <c>null</c>
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
				_accessedSettingsFilePath = _settingsFilePath;
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

		/// <summary>
		/// Tries to delete file <see cref="SettingsFilePath"/>.
		/// </summary>
		/// <returns>
		/// Returns true if file successfully saved.
		/// </returns>
		public bool Delete()
		{
			try
			{
				File.Delete(_settingsFilePath);
				return (true);
			}
			catch
			{
				return (false);
			}
		}

		#endregion
	}
}
