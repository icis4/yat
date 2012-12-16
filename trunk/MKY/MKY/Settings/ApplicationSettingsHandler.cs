//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;

using MKY.Xml;

#endregion

namespace MKY.Settings
{
	/// <summary>
	/// Generic class to handle standard application settings. It covers common, local user
	/// as well as roaming user settings.
	/// </summary>
	/// <typeparam name="TCommonSettings">The type of the common settings.</typeparam>
	/// <typeparam name="TLocalUserSettings">The type of the local user settings.</typeparam>
	/// <typeparam name="TRoamingUserSettings">The type of the roaming user settings.</typeparam>
	public class ApplicationSettingsHandler<TCommonSettings, TLocalUserSettings, TRoamingUserSettings> : IDisposable
		where TCommonSettings : SettingsItem, new()
		where TLocalUserSettings : SettingsItem, new()
		where TRoamingUserSettings : SettingsItem, new()
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private class Handler<TSettings> : SettingsFileHandler, IDisposable
			where TSettings : SettingsItem, new()
		{
			#region Fields
			//==========================================================================================
			// Fields
			//==========================================================================================

			private bool isDisposed;

			private TSettings settings = default(TSettings);
			private AlternateXmlElement[] alternateXmlElements = null;
			private Mutex mutex;
			private bool areCurrentlyOwnedByThisInstance = false;

			#endregion

			#region Object Lifetime
			//==========================================================================================
			// Object Lifetime
			//==========================================================================================

			/// <summary></summary>
			public Handler(string name, string filePath, Type parentType)
				: base(filePath, parentType)
			{
				this.settings = new TSettings();

				IAlternateXmlElementProvider aep = this.settings as IAlternateXmlElementProvider;
				if (aep != null)
					this.alternateXmlElements = aep.AlternateXmlElements;

				// Create named mutex and try to acquire it. Note that the mutex must be acquired
				// immediately, and once only, and the success is stored in a boolean variable. This
				// mechanism ensures that once an instance 'owns' the settings, it keeps it. And does
				// so until it exits. Then the mutex is released by the destructor of this class.
				this.mutex = new Mutex(false, Application.ProductName + "." + name);
				this.areCurrentlyOwnedByThisInstance = this.mutex.WaitOne(TimeSpan.Zero);
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
					// The mutex must be closed and released by the application because it would
					// be called from the wrong process if it was closed by the garbage collector.

					if (disposing)
					{
						// Dispose of unmanaged resources.
					}

					this.isDisposed = true;
				}
			}

			/// <summary></summary>
			~Handler()
			{
				Dispose(false);
			}

			/// <summary></summary>
			public bool IsDisposed
			{
				get { return (this.isDisposed); }
			}

			/// <summary></summary>
			protected void AssertNotDisposed()
			{
				if (this.isDisposed)
					throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed"));
			}

			#endregion

			#endregion

			#region Properties
			//==========================================================================================
			// Properties
			//==========================================================================================

			/// <summary></summary>
			public virtual TSettings Settings
			{
				get
				{
					AssertNotDisposed();
					return (this.settings);
				}
			}

			/// <summary>
			/// Handler to settings default.
			/// </summary>
			public virtual TSettings SettingsDefault
			{
				get
				{
					AssertNotDisposed();
					return (new TSettings());
				}
			}

			/// <summary>
			/// Returns whether settings are currently owned by the current instance,
			/// <c>false</c> if not.
			/// </summary>
			/// <remarks>
			/// This class uses a named mutex to prevent that concurrent instances of the application
			/// (i.e. if the application has been started multiple times in parallel) mess with the
			/// common settings.
			/// </remarks>
			public virtual bool AreCurrentlyOwnedByThisInstance
			{
				get
				{
					AssertNotDisposed();
					return (this.areCurrentlyOwnedByThisInstance);
				}
			}

			#endregion

			#region Methods
			//==========================================================================================
			// Methods
			//==========================================================================================

			/// <summary>
			/// Tries to load settings from corresponding file path.
			/// </summary>
			/// <returns>
			/// Returns <c>true</c> if settings could be loaded from the given file path,
			/// return <c>false</c> if they could not be loaded and were set to defaults instead.
			/// </returns>
			public virtual bool Load()
			{
				AssertNotDisposed();

				// Try to open existing file of current version.
				object settings = LoadFromFile(typeof(TSettings), this.alternateXmlElements);
				if (settings != null)
				{
					this.settings = (TSettings)settings;
					return (true);
				}

				// Alternatively, try to open an existing file of an older version.
				{
					// Find all valid directories of older versions.
					string productSettingsPath = Path.GetDirectoryName(Path.GetDirectoryName(FilePath));
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

					// Iterate through the directories, start with most recent.
					string fileName = Path.GetFileName(FilePath);
					oldDirectories.Sort();
					for (int i = oldDirectories.Count - 1; i >= 0; i--)
					{
						string oldFilePath = oldDirectories[i] + Path.DirectorySeparatorChar + fileName;
						settings = LoadFromFile(oldFilePath, typeof(TSettings), this.alternateXmlElements);
						if (settings != null)
						{
							this.settings = (TSettings)settings;
							return (true);
						}
					}
				}

				// Nothing found, return default settings:
				this.settings = SettingsDefault;
				return (false);
			}

			/// <summary>
			/// Tries to save settings to corresponding file path.
			/// </summary>
			/// <remarks>
			/// Use of exception instead of boolean return value to ease handling of errors:
			///  - Exception will contain the reason for the failure
			///  - 'good-weather' case be easier implemented, kind of scripted
			/// </remarks>
			/// <exception cref="Exception">
			/// Thrown if settings could not be saved.
			/// </exception>
			public virtual void Save()
			{
				AssertNotDisposed();

				SaveToFile(typeof(TSettings), this.settings);
				this.settings.ClearChanged();
			}

			/// <summary>
			/// Force that settings are currently owned by this instance, i.e. on the next
			/// save request the settings will indeed be saved.
			/// </summary>
			public virtual void ForceThatSettingsAreCurrentlyOwnedByThisInstance()
			{
				AssertNotDisposed();

				this.areCurrentlyOwnedByThisInstance = true;
			}

			/// <summary>
			/// Close the settings and release all resources.
			/// </summary>
			public virtual void Close()
			{
				AssertNotDisposed();

				if (this.mutex != null)
				{
					this.mutex.ReleaseMutex();
					this.mutex.Close();
					this.mutex = null;
				}
			}

			#endregion
		}

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const string Extension = ".xml";

		private const string CommonName      = "CommonSettings";
		private const string LocalUserName   = "LocalUserSettings";
		private const string RoamingUserName = "RoamingUserSettings";

		private const string CommonFileName      = CommonName      + Extension;
		private const string LocalUserFileName   = LocalUserName   + Extension;
		private const string RoamingUserFileName = RoamingUserName + Extension;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isDisposed;

		private Handler<TCommonSettings> commonSettings;
		private Handler<TLocalUserSettings> localUserSettings;
		private Handler<TRoamingUserSettings> roamingUserSettings;

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
			if (hasCommonSettings)
				this.commonSettings = new Handler<TCommonSettings>
					(
					CommonName,
					Application.CommonAppDataPath + Path.DirectorySeparatorChar + CommonFileName,
					GetType()
					);

			if (hasLocalUserSettings)
				this.localUserSettings = new Handler<TLocalUserSettings>
					(
					LocalUserName,
					Application.LocalUserAppDataPath + Path.DirectorySeparatorChar + LocalUserFileName,
					GetType()
					);
			
			if (hasRoamingUserSettings)
				this.roamingUserSettings = new Handler<TRoamingUserSettings>
					(
					RoamingUserName,
					Application.UserAppDataPath + Path.DirectorySeparatorChar + RoamingUserFileName,
					GetType()
					);
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
				// Finalize managed resources.

				if (disposing)
				{
					// In the 'normal' case, all settings have already been closed in Close().
					if (this.commonSettings != null)
					{
						this.commonSettings.Dispose();
						this.commonSettings = null;
					}
					if (this.localUserSettings != null)
					{
						this.localUserSettings.Dispose();
						this.localUserSettings = null;
					}
					if (this.roamingUserSettings != null)
					{
						this.roamingUserSettings.Dispose();
						this.roamingUserSettings = null;
					}
				}

				this.isDisposed = true;
			}
		}

		/// <summary></summary>
		~ApplicationSettingsHandler()
		{
			Dispose(false);
		}

		/// <summary></summary>
		public bool IsDisposed
		{
			get { return (this.isDisposed); }
		}

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (this.isDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed"));
		}

		#endregion

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
			get
			{
				AssertNotDisposed();
				return (this.commonSettings != null);
			}
		}

		/// <summary>
		/// Returns whether this handler has local user settings.
		/// </summary>
		public virtual bool HasLocalUserSettings
		{
			get
			{
				AssertNotDisposed();
				return (this.localUserSettings != null);
			}
		}

		/// <summary>
		/// Returns whether this handler has user settings.
		/// </summary>
		public virtual bool HasRoamingUserSettings
		{
			get
			{
				AssertNotDisposed();
				return (this.roamingUserSettings != null);
			}
		}

		/// <summary>
		/// Handler to common settings, if has common settings,
		/// <c>null</c> otherwise.
		/// </summary>
		public virtual TCommonSettings CommonSettingsDefault
		{
			get
			{
				// AssertNotDisposed() is called by 'Has...' below.

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
				// AssertNotDisposed() is called by 'Has...' below.

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
				// AssertNotDisposed() is called by 'Has...' below.

				if (HasRoamingUserSettings)
					return (new TRoamingUserSettings());
				else
					return (default(TRoamingUserSettings));
			}
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
			get
			{
				AssertNotDisposed();
				return (this.commonSettings.FilePath);
			}
			set
			{
				// AssertNotDisposed() is called by 'Has...' below.

				if (HasCommonSettings)
					this.commonSettings.FilePath = value;
				else
					throw (new InvalidOperationException("This handler has no common settings!"));
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
			get
			{
				AssertNotDisposed();
				return (this.localUserSettings.FilePath);
			}
			set
			{
				// AssertNotDisposed() is called by 'Has...' below.

				if (HasLocalUserSettings)
					this.localUserSettings.FilePath = value;
				else
					throw (new InvalidOperationException("This handler has no local user settings!"));
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
			get
			{
				AssertNotDisposed();
				return (this.roamingUserSettings.FilePath);
			}
			set
			{
				// AssertNotDisposed() is called by 'Has...' below.
			
				if (HasRoamingUserSettings)
					this.roamingUserSettings.FilePath = value;
				else
					throw (new InvalidOperationException("This handler has no roaming user settings!"));
			}
		}

		/// <summary>
		/// Handler to common settings, if has common settings, <c>null</c> otherwise.
		/// </summary>
		public virtual TCommonSettings CommonSettings
		{
			get
			{
				AssertNotDisposed();
				return (this.commonSettings.Settings);
			}
		}

		/// <summary>
		/// Handler to local user settings, if has local user settings, <c>null</c> otherwise.
		/// </summary>
		public virtual TLocalUserSettings LocalUserSettings
		{
			get
			{
				AssertNotDisposed();
				return (this.localUserSettings.Settings);
			}
		}

		/// <summary>
		/// Handler to roaming user settings, if has roaming user settings, <c>null</c> otherwise.
		/// </summary>
		public virtual TRoamingUserSettings RoamingUserSettings
		{
			get
			{
				AssertNotDisposed();
				return (this.roamingUserSettings.Settings);
			}
		}

		/// <summary>
		/// Returns whether common settings have successfully been loaded, <c>false</c> if
		/// there was no valid settings file and they were set to their defaults.
		/// </summary>
		public virtual bool CommonSettingsSuccessfullyLoadedFromFile
		{
			get
			{
				AssertNotDisposed();
				return (this.commonSettings.FileSuccessfullyLoaded);
			}
		}

		/// <summary>
		/// Returns whether local user settings have successfully been loaded, <c>false</c> if
		/// there was no valid settings file and they were set to their defaults.
		/// </summary>
		public virtual bool LocalUserSettingsSuccessfullyLoadedFromFile
		{
			get
			{
				AssertNotDisposed();
				return (this.localUserSettings.FileSuccessfullyLoaded);
			}
		}

		/// <summary>
		/// Returns whether roaming user settings have successfully been loaded, <c>false</c> if
		/// there was no valid settings file and they were set to their defaults.
		/// </summary>
		public virtual bool RoamingUserSettingsSuccessfullyLoadedFromFile
		{
			get
			{
				AssertNotDisposed();
				return (this.roamingUserSettings.FileSuccessfullyLoaded);
			}
		}

		/// <summary>
		/// Returns whether common settings are currently owned by the current instance,
		/// <c>false</c> if not.
		/// </summary>
		/// <remarks>
		/// This class uses a named mutex to prevent that concurrent instances of the application
		/// (i.e. if the application has been started multiple times in parallel) mess with the
		/// common settings.
		/// </remarks>
		public virtual bool CommonSettingsAreCurrentlyOwnedByThisInstance
		{
			get
			{
				AssertNotDisposed();
				return (this.commonSettings.AreCurrentlyOwnedByThisInstance);
			}
		}

		/// <summary>
		/// Returns whether local user settings are currently owned by the current instance,
		/// <c>false</c> if not.
		/// </summary>
		/// <remarks>
		/// This class uses a named mutex to prevent that concurrent instances of the application
		/// (i.e. if the application has been started multiple times in parallel) mess with the
		/// local user settings.
		/// </remarks>
		public virtual bool LocalUserSettingsAreCurrentlyOwnedByThisInstance
		{
			get
			{
				AssertNotDisposed();
				return (this.localUserSettings.AreCurrentlyOwnedByThisInstance);
			}
		}

		/// <summary>
		/// Returns whether roaming user settings are currently owned by the current instance,
		/// <c>false</c> if not.
		/// </summary>
		/// <remarks>
		/// This class uses a named mutex to prevent that concurrent instances of the application
		/// (i.e. if the application has been started multiple times in parallel) mess with the
		/// roaming user settings.
		/// </remarks>
		public virtual bool RoamingUserSettingsAreCurrentlyOwnedByThisInstance
		{
			get
			{
				AssertNotDisposed();
				return (this.roamingUserSettings.AreCurrentlyOwnedByThisInstance);
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

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
			// AssertNotDisposed() is called by 'Load()' below.

			bool result = true;

			if (!LoadCommonSettings())
				result = false;

			if (!LoadLocalUserSettings())
				result = false;

			if (!LoadRoamingUserSettings())
				result = false;

			// Immediately try to save settings to reflect current version.
			try
			{
				Save();
			}
			catch { }

			// Return load result.
			return (result);
		}

		/// <summary>
		/// Tries to load settings from <see cref="CommonSettingsFilePath"/>.
		/// </summary>
		/// <returns>
		/// Returns false if either settings could not be loaded from
		/// its file path and have been set to defaults.
		/// </returns>
		public virtual bool LoadCommonSettings()
		{
			// AssertNotDisposed() is called by 'Has...' below.

			if (HasCommonSettings)
				return (this.commonSettings.Load());
			else
				return (true);
		}

		/// <summary>
		/// Tries to load settings from <see cref="LocalUserSettingsFilePath"/>.
		/// </summary>
		/// <returns>
		/// Returns false if either settings could not be loaded from
		/// its file path and have been set to defaults.
		/// </returns>
		public virtual bool LoadLocalUserSettings()
		{
			// AssertNotDisposed() is called by 'Has...' below.

			if (HasLocalUserSettings)
				return (this.localUserSettings.Load());
			else
				return (true);
		}

		/// <summary>
		/// Tries to load settings from <see cref="RoamingUserSettingsFilePath"/>.
		/// </summary>
		/// <returns>
		/// Returns false if either settings could not be loaded from
		/// its file path and have been set to defaults.
		/// </returns>
		public virtual bool LoadRoamingUserSettings()
		{
			// AssertNotDisposed() is called by 'Has...' below.

			if (HasRoamingUserSettings)
				return (this.roamingUserSettings.Load());
			else
				return (true);
		}

		/// <summary>
		/// Tries to save settings to <see cref="CommonSettingsFilePath"/>,
		/// <see cref="LocalUserSettingsFilePath"/> and <see cref="RoamingUserSettingsFilePath"/>.
		/// </summary>
		/// <remarks>
		/// Use of exception instead of boolean return value to ease handling of errors:
		///  - Exception will contain the reason for the failure
		///  - 'good-weather' case be easier implemented, kind of scripted
		/// </remarks>
		/// <exception cref="Exception">
		/// Thrown if settings could not be saved.
		/// </exception>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		public virtual void Save()
		{
			AssertNotDisposed();

			Exception result = null;

			// Try to save common settings.
			try
			{
				SaveCommonSettings();
			}
			catch (Exception ex)
			{
				if (result == null)
					result = ex;
			}

			// Try to save local user settings.
			try
			{
				SaveLocalUserSettings();
			}
			catch (Exception ex)
			{
				if (result == null)
					result = ex;
			}

			// Try to save roaming user settings.
			try
			{
				SaveRoamingUserSettings();
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
		/// Tries to save settings to <see cref="CommonSettingsFilePath"/>.
		/// </summary>
		/// <remarks>
		/// Use of exception instead of boolean return value to ease handling of errors:
		///  - Exception will contain the reason for the failure
		///  - 'good-weather' case be easier implemented, kind of scripted
		/// </remarks>
		/// <exception cref="Exception">
		/// Thrown if settings could not be saved.
		/// </exception>
		public virtual void SaveCommonSettings()
		{
			// AssertNotDisposed() is called by 'Has...' below.

			if (HasCommonSettings && CommonSettingsAreCurrentlyOwnedByThisInstance)
				this.commonSettings.Save();
		}

		/// <summary>
		/// Tries to save settings to <see cref="LocalUserSettingsFilePath"/>.
		/// </summary>
		/// <remarks>
		/// Use of exception instead of boolean return value to ease handling of errors:
		///  - Exception will contain the reason for the failure
		///  - 'good-weather' case be easier implemented, kind of scripted
		/// </remarks>
		/// <exception cref="Exception">
		/// Thrown if settings could not be saved.
		/// </exception>
		public virtual void SaveLocalUserSettings()
		{
			// AssertNotDisposed() is called by 'Has...' below.

			if (HasLocalUserSettings && LocalUserSettingsAreCurrentlyOwnedByThisInstance)
				this.localUserSettings.Save();
		}

		/// <summary>
		/// Tries to save settings to <see cref="RoamingUserSettingsFilePath"/>.
		/// </summary>
		/// <remarks>
		/// Use of exception instead of boolean return value to ease handling of errors:
		///  - Exception will contain the reason for the failure
		///  - 'good-weather' case be easier implemented, kind of scripted
		/// </remarks>
		/// <exception cref="Exception">
		/// Thrown if settings could not be saved.
		/// </exception>
		public virtual void SaveRoamingUserSettings()
		{
			// AssertNotDisposed() is called by 'Has...' below.

			if (HasRoamingUserSettings && RoamingUserSettingsAreCurrentlyOwnedByThisInstance)
				this.roamingUserSettings.Save();
		}

		/// <summary>
		/// Force that common settings are currently owned by this instance, i.e. on the next
		/// save request the settings will indeed be saved.
		/// </summary>
		public virtual void ForceThatCommonSettingsAreCurrentlyOwnedByThisInstance()
		{
			// AssertNotDisposed() is called by 'Has...' below.

			if (HasCommonSettings)
				this.commonSettings.ForceThatSettingsAreCurrentlyOwnedByThisInstance();
		}

		/// <summary>
		/// Force that local user settings are currently owned by this instance, i.e. on the next
		/// save request the settings will indeed be saved.
		/// </summary>
		public virtual void ForceThatLocalUserSettingsAreCurrentlyOwnedByThisInstance()
		{
			// AssertNotDisposed() is called by 'Has...' below.

			if (HasLocalUserSettings)
				this.localUserSettings.ForceThatSettingsAreCurrentlyOwnedByThisInstance();
		}

		/// <summary>
		/// Force that roaming user settings are currently owned by this instance, i.e. on the next
		/// save request the settings will indeed be saved.
		/// </summary>
		public virtual void ForceThatRoamingUserSettingsAreCurrentlyOwnedByThisInstance()
		{
			// AssertNotDisposed() is called by 'Has...' below.

			if (HasRoamingUserSettings)
				this.roamingUserSettings.ForceThatSettingsAreCurrentlyOwnedByThisInstance();
		}

		/// <summary>
		/// Close the application settings and release all resources.
		/// </summary>
		public virtual void Close()
		{
			if (HasCommonSettings)
				this.commonSettings.Close();

			if (HasLocalUserSettings)
				this.localUserSettings.Close();

			if (HasRoamingUserSettings)
				this.roamingUserSettings.Close();
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
