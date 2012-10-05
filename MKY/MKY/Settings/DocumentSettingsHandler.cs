﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.8
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using MKY.IO;
using MKY.Xml;

namespace MKY.Settings
{
	/// <summary>
	/// Generic class to handle any kind of document settings, e.g. MDI application settings.
	/// </summary>
	/// <typeparam name="TDocumentSettings">The type of the settings.</typeparam>
	public class DocumentSettingsHandler<TDocumentSettings>
		where TDocumentSettings : SettingsItem, new()
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
		/// Handles document settings. Settings are stored in <see cref="SettingsFilePath"/>.
		/// </summary>
		public DocumentSettingsHandler()
		{
			Initialize(SettingsDefault);
		}

		/// <summary>
		/// Handles document settings. Settings are stored in <see cref="SettingsFilePath"/>.
		/// </summary>
		public DocumentSettingsHandler(TDocumentSettings settings)
		{
			Initialize(settings);
		}

		private void Initialize(TDocumentSettings settings)
		{
			this.settings = settings;

			IAlternateXmlElementProvider aep = this.settings as IAlternateXmlElementProvider;
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
				if (string.IsNullOrEmpty(this.settingsFilePath))
					return (false);

				if (Path.GetFullPath(this.settingsFilePath).Length == 0)
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
				if (string.IsNullOrEmpty(this.settingsFilePath))
					return (false);

				return (File.Exists(this.settingsFilePath));
			}
		}

		/// <summary>
		/// Returns whether the settings file is up to date.
		/// </summary>
		public virtual bool SettingsFileIsUpToDate
		{
			get
			{
				if (string.IsNullOrEmpty(this.settingsFilePath))
					return (false);

				if (!File.Exists(this.settingsFilePath))
					return (false);

				// Return whether current settings file path is still the same as the last access.
				return (PathEx.Equals(this.settingsFilePath, this.accessedSettingsFilePath));
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
			object settings = SettingsFileHandler.LoadFromFile(this.settingsFilePath, this.settings.GetType(), this.alternateXmlElements, GetType());
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
				SettingsFileHandler.SaveToFile(this.settingsFilePath, this.settings.GetType(), this.settings, GetType());
				this.accessedSettingsFilePath = this.settingsFilePath;
				this.settings.ClearChanged();
			}
			catch (Exception ex)
			{
				if (result == null)
					result = ex;
			}

			// Throw exception if either operation failed.
			if (result != null)
				throw (result);
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
