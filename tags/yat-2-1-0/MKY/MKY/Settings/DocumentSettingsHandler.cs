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
// MKY Version 1.0.27
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using MKY.Xml;

#endregion

namespace MKY.Settings
{
	/// <summary>
	/// Generic class to handle any kind of document settings, e.g. MDI application settings.
	/// </summary>
	/// <typeparam name="TSettings">The type of the settings.</typeparam>
	public class DocumentSettingsHandler<TSettings> : IDisposable, IDisposableEx
		where TSettings : SettingsItem, new()
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingsFileHandler fileHandler;

		private TSettings settings; // = null;
		private IEnumerable<AlternateXmlElement> alternateXmlElements; // = null;

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
			Initialize(new TSettings());
		}

		/// <summary>
		/// Handles document settings. Settings are stored in <see cref="SettingsFilePath"/>.
		/// </summary>
		public DocumentSettingsHandler(TSettings settings)
		{
			Initialize(settings);
		}

		private void Initialize(TSettings settings)
		{
			this.fileHandler = new SettingsFileHandler();

			this.settings = settings;

			var aep = (this.settings as IAlternateXmlElementProvider);
			if (aep != null)
				this.alternateXmlElements = aep.AlternateXmlElements;
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public bool IsDisposed { get; protected set; }

		/// <summary></summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary></summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				// Dispose of managed resources:
				if (disposing)
				{
				}

				// Set state to disposed:
				IsDisposed = true;
			}
		}

	#if (DEBUG)
		/// <remarks>
		/// Microsoft.Design rule CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable requests
		/// "Types that declare disposable members should also implement IDisposable. If the type
		///  does not own any unmanaged resources, do not implement a finalizer on it."
		///
		/// Well, true for best performance on finalizing. However, it's not easy to find missing
		/// calls to <see cref="Dispose()"/>. In order to detect such missing calls, the finalizer
		/// is kept for DEBUG, indicating missing calls.
		///
		/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
		/// </remarks>
		~DocumentSettingsHandler()
		{
			Dispose(false);

			Diagnostics.DebugDisposal.DebugNotifyFinalizerInsteadOfDispose(this);
		}
	#endif // DEBUG

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (IsDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed!"));
		}

		#endregion

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary>
		/// Absolute path to the settings file.
		/// </summary>
		public virtual string SettingsFilePath
		{
			get { AssertNotDisposed(); return (this.fileHandler.FilePath);        }
			set { AssertNotDisposed();         this.fileHandler.FilePath = value; }
		}

		/// <summary></summary>
		public virtual void ResetSettingsFilePath()
		{
			AssertNotDisposed();

			SettingsFilePath = null;
		}

		/// <summary>
		/// Returns whether the settings file path is defined.
		/// </summary>
		public virtual bool SettingsFilePathIsDefined
		{
			get
			{
				AssertNotDisposed();

				return (this.fileHandler.FilePathIsDefined);
			}
		}

		/// <summary>
		/// Returns whether the settings file path is valid.
		/// </summary>
		public virtual bool SettingsFilePathIsValid
		{
			get
			{
				AssertNotDisposed();

				return (this.fileHandler.FilePathIsValid);
			}
		}

		/// <summary>
		/// Returns whether the settings file exists.
		/// </summary>
		public virtual bool SettingsFileExists
		{
			get
			{
				AssertNotDisposed();

				return (this.fileHandler.FileExists);
			}
		}

		/// <summary>
		/// Returns whether the settings were loaded from a file but that doesn't exist anymore.
		/// </summary>
		public virtual bool SettingsFileExistsNoMore
		{
			get
			{
				AssertNotDisposed();

				return (this.fileHandler.FileExistsNoMore);
			}
		}

		/// <summary>
		/// Returns whether the settings file is up to date.
		/// </summary>
		public virtual bool SettingsFileIsUpToDate
		{
			get
			{
				AssertNotDisposed();

				return (this.fileHandler.FileIsUpToDate);
			}
		}

		/// <summary>
		/// Returns whether the settings file is readable.
		/// </summary>
		public virtual bool SettingsFileIsReadable
		{
			get
			{
				AssertNotDisposed();

				return (this.fileHandler.FileIsReadable);
			}
		}

		/// <summary>
		/// Returns whether the settings file is read-only.
		/// </summary>
		public virtual bool SettingsFileIsReadOnly
		{
			get
			{
				AssertNotDisposed();

				return (this.fileHandler.FileIsReadOnly);
			}
		}

		/// <summary>
		/// Returns whether the settings file is writable.
		/// </summary>
		public virtual bool SettingsFileIsWritable
		{
			get
			{
				AssertNotDisposed();

				return (this.fileHandler.FileIsWritable);
			}
		}

		/// <summary>
		/// Returns whether the settings file has successfully been loaded, <c>false</c> if there was
		/// no valid settings file available.
		/// </summary>
		public virtual bool SettingsFileSuccessfullyLoaded
		{
			get
			{
				AssertNotDisposed();

				return (this.fileHandler.FileSuccessfullyLoaded);
			}
		}

		/// <summary>
		/// Handler to settings.
		/// </summary>
		/// <remarks>
		/// Public getter, protected setter.
		/// </remarks>
		public virtual TSettings Settings
		{
			get
			{
				AssertNotDisposed();

				return (this.settings);
			}
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
		/// Returns <c>true</c> if settings could be loaded from <see cref="SettingsFilePath"/>,
		/// returns <c>false</c> if they could not be loaded and were set to defaults instead.
		/// </returns>
		/// <exception cref="Exception">
		/// Thrown if settings could not be created.
		/// </exception>
		public virtual bool Load()
		{
			AssertNotDisposed();

			// Try to open existing file of current version.
			var settings = this.fileHandler.LoadFromFile<TSettings>(this.alternateXmlElements);
			if (settings != null)
			{
				this.settings = settings;
				return (true);
			}

			// Nothing found, return default settings:
			this.settings = new TSettings();
			return (false);
		}

		/// <summary>
		/// Tries to save settings to <see cref="SettingsFilePath"/>.
		/// </summary>
		/// <remarks>
		/// Use of exception instead of boolean return value to ease handling of errors:
		///  - Exception will contain the reason for the failure
		///  - 'good-weather' case be easier implemented, kind of scripted
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		/// <exception cref="Exception">
		/// Thrown if settings could not be saved.
		/// </exception>
		public virtual void Save()
		{
			AssertNotDisposed();

			this.fileHandler.SaveToFile<TSettings>(this.settings);
			this.settings.ClearChanged();
		}

		/// <summary>
		/// Tries to delete file <see cref="SettingsFilePath"/>.
		/// </summary>
		/// <returns>
		/// Returns <c>true</c> if settings file successfully deleted.
		/// </returns>
		public virtual bool TryDelete()
		{
			AssertNotDisposed();

			return (this.fileHandler.TryDelete());
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
