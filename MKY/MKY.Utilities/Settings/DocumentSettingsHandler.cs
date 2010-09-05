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
// Copyright � 2003-2004 HSR Hochschule f�r Technik Rapperswil.
// Copyright � 2003-2010 Matthias Kl�y.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml.Serialization;

using MKY.Utilities.Diagnostics;
using MKY.Utilities.Xml;

namespace MKY.Utilities.Settings
{
	/// <summary>
	/// Generic class to handle any kind of document settings, e.g. MDI application settings.
	/// </summary>
	/// <typeparam name="TDocumentSettings">The type of the settings.</typeparam>
	public class DocumentSettingsHandler<TDocumentSettings>
		where TDocumentSettings : Settings, new()
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private string settingsFilePath = "";
		private string accessedSettingsFilePath = "";

		private TDocumentSettings settings = default(TDocumentSettings);

		private AlternateXmlElement[] alternateXmlElements = null;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary>
		/// Handles document settings. Settings are stored in filePath.
		/// <see cref="SettingsFilePath"/>
		/// </summary>
		public DocumentSettingsHandler()
		{
			Initialize(SettingsDefault);
		}

		/// <summary>
		/// Handles document settings. Settings are stored in filePath.
		/// <see cref="SettingsFilePath"/>
		/// </summary>
		public DocumentSettingsHandler(TDocumentSettings settings)
		{
			Initialize(settings);
		}

		private void Initialize(TDocumentSettings settings)
		{
			this.settings = settings;

			IAlternateXmlElementProvider aep = settings	as IAlternateXmlElementProvider;
			if (aep != null)
				this.alternateXmlElements = aep.AlternateXmlElements;
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary>
		/// Complete path to document settings filePath.
		/// </summary>
		public virtual string SettingsFilePath
		{
			get { return (this.settingsFilePath); }
			set { this.settingsFilePath = value; }
		}

		/// <summary>
		/// Returns whether the settings file path is valid.
		/// </summary>
		public virtual bool SettingsFilePathIsValid
		{
			get
			{
				if (this.settingsFilePath == null)
					return (false);
				if (this.settingsFilePath.Length == 0)
					return (false);
				if (System.IO.Path.GetFullPath(this.settingsFilePath).Length == 0)
					return (false);
				return (true);
			}
		}

		/// <summary>
		/// Returns whether the settings file exists.
		/// </summary>
		public virtual bool SettingsFileExists
		{
			get
			{
				if (this.settingsFilePath == null)
					return (false);
				if (this.settingsFilePath.Length == 0)
					return (false);
				return (System.IO.File.Exists(this.settingsFilePath));
			}
		}

		/// <summary>
		/// Returns whether the settings file is up to date.
		/// </summary>
		public virtual bool SettingsFileIsUpToDate
		{
			get
			{
				if (this.settingsFilePath == null)
					return (false);
				if (this.settingsFilePath.Length == 0)
					return (false);
				if (!System.IO.File.Exists(this.settingsFilePath))
					return (false);

				// Return whether current settings file path is still the same as the last access.
				return (this.settingsFilePath == this.accessedSettingsFilePath);
			}
		}

		/// <summary>
		/// Handler to settings.
		/// </summary>
		public virtual TDocumentSettings Settings
		{
			get { return (this.settings); }
		}

		/// <summary>
		/// Handler to settings default.
		/// </summary>
		public virtual TDocumentSettings SettingsDefault
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
		public virtual bool Load()
		{
			bool loadSuccess = true;
			object settings = null;

			settings = LoadFromFile(this.settings.GetType(), this.settingsFilePath);
			if (settings == null)
			{
				settings = SettingsDefault;
				loadSuccess = false;
			}
			else
			{
				this.accessedSettingsFilePath = this.settingsFilePath;
			}
			this.settings = (TDocumentSettings)settings;
			this.settings.ClearChanged();

			// Return load status.
			return (loadSuccess);
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		private object LoadFromFile(Type type, string filePath)
		{
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

				// Try to open existing file with tolerant & alternate-tolerant deserialization.
				try
				{
					object settings = null;
					using (StreamReader sr = new StreamReader(filePath))
					{
						AlternateTolerantXmlSerializer serializer = new AlternateTolerantXmlSerializer(type, this.alternateXmlElements);
						settings = serializer.Deserialize(sr);
					}
					return (settings);
				}
				catch (Exception ex)
				{
					XDebug.WriteException(this, ex);
				}
			}

			// If not successful, return <c>null</c>.
			return (null);
		}

		/// <summary>
		/// Tries to save settings to <see cref="SettingsFilePath"/>.
		/// </summary>
		/// <exception cref="Exception">
		/// Thrown if settings could not be saved.
		/// </exception>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		public virtual void Save()
		{
			Exception result = null;

			try
			{
				SaveToFile(this.settings.GetType(), this.settingsFilePath, this.settings);
				this.accessedSettingsFilePath = this.settingsFilePath;
				this.settings.ClearChanged();
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

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
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
		/// Returns <c>true</c> if file successfully saved.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		public virtual bool TryDelete()
		{
			try
			{
				File.Delete(this.settingsFilePath);
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

//==================================================================================================
// End of
// $URL$
//==================================================================================================
