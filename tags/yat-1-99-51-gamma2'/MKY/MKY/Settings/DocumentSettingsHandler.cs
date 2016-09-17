//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.15
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
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
using System.IO;

using MKY.IO;
using MKY.Xml;

#endregion

namespace MKY.Settings
{
	/// <summary>
	/// Generic class to handle any kind of document settings, e.g. MDI application settings.
	/// </summary>
	/// <typeparam name="TSettings">The type of the settings.</typeparam>
	public class DocumentSettingsHandler<TSettings> : IDisposable
		where TSettings : SettingsItem, new()
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isDisposed;

		private SettingsFileHandler fileHandler;

		private TSettings settings; // = null;
		private AlternateXmlElement[] alternateXmlElements; // = null;

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
			this.fileHandler = new SettingsFileHandler(GetType());

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
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary></summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!this.isDisposed)
			{
				// Dispose of managed resources:
				if (disposing)
				{
				}

				// Set state to disposed:
				this.isDisposed = true;
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
		public bool IsDisposed
		{
			get { return (this.isDisposed); }
		}

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (this.isDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed!"));
		}

		#endregion

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
			get { AssertNotDisposed(); return (this.fileHandler.FilePath); }
			set { AssertNotDisposed(); this.fileHandler.FilePath = value;  }
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
		/// Returns whether setting file is readable.
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
		/// Returns whether setting file is writable.
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
		/// Returns whether setting file is read-only.
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
		/// Returns whether setting file has successfully been loaded, <c>false</c> if there was
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
		/// return <c>false</c> if they could not be loaded and were set to defaults instead.
		/// </returns>
		/// <exception cref="Exception">
		/// Thrown if settings could not be created.
		/// </exception>
		public virtual bool Load()
		{
			AssertNotDisposed();

			// Try to open existing file of current version.
			var settings = (TSettings)this.fileHandler.LoadFromFile(this.settings.GetType(), this.alternateXmlElements);
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
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		public virtual void Save()
		{
			AssertNotDisposed();

			this.fileHandler.SaveToFile(typeof(TSettings), this.settings);
			this.settings.ClearChanged();
		}

		/// <summary>
		/// Tries to delete file <see cref="SettingsFilePath"/>.
		/// </summary>
		/// <returns>
		/// Returns <c>true</c> if settings file successfully deleted.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
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
