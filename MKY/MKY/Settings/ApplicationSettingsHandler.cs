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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
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
	public class ApplicationSettingsHandler<TCommonSettings, TLocalUserSettings, TRoamingUserSettings> : IDisposable
		where TCommonSettings : new()
		where TLocalUserSettings : new()
		where TRoamingUserSettings : new()
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private class Handler<TSettings> : IDisposable
			where TSettings : new()
		{
			#region Fields
			//==========================================================================================
			// Fields
			//==========================================================================================

			private bool isDisposed;

			private string filePath = "";
			private TSettings settings = default(TSettings);
			private AlternateXmlElement[] alternateXmlElements = null;
			private Mutex mutex;
			private bool successfullyLoaded = false;
			private bool areCurrentlyOwnedByThisInstance = false;

			#endregion

			#region Object Lifetime
			//==========================================================================================
			// Object Lifetime
			//==========================================================================================

			/// <summary></summary>
			public Handler(string name, string filePath)
			{
				this.filePath = filePath;
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
					if (disposing)
					{
						if (this.mutex != null)
						{
							this.mutex.ReleaseMutex();
							this.mutex.Close();
							this.mutex = null;
						}
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
			protected bool IsDisposed
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
			public virtual string FilePath
			{
				get
				{
					AssertNotDisposed();
					return (this.filePath);
				}
				set
				{
					AssertNotDisposed();
					this.filePath = value;
				}
			}

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
			/// Returns whether settings have successfully been loaded, <c>false</c> if
			/// they was no valid settings file and they were set to their defaults.
			/// </summary>
			public virtual bool SuccessfullyLoaded
			{
				get
				{
					AssertNotDisposed();
					return (this.successfullyLoaded);
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
			/// Returns false if either settings could not be loaded from
			/// its file path and have been set to defaults.
			/// </returns>
			public virtual bool Load()
			{
				AssertNotDisposed();

				bool result = true;

				object settings = LoadFromFile(typeof(TSettings), this.filePath, this.alternateXmlElements);
				this.successfullyLoaded = (settings != null);
				if (!this.successfullyLoaded)
				{
					settings = new TSettings();
					result = false;
				}
				this.settings = (TSettings)settings;

				return (result);
			}

			[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
			private object LoadFromFile(Type type, string filePath, AlternateXmlElement[] alternateXmlElements)
			{
				// Try to open existing file of current version.
				object settings = SettingsHandler.LoadFromFile(filePath, type, alternateXmlElements, this.GetType());
				if (settings != null)
					return (settings);

				// Alternatively, try to open an existing file of an older version.
				{
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

					// Iterate through the directories, start with most recent.
					string fileName = Path.GetFileName(filePath);
					oldDirectories.Sort();
					for (int i = oldDirectories.Count - 1; i >= 0; i--)
					{
						string oldFilePath = oldDirectories[i] + Path.DirectorySeparatorChar + fileName;
						settings = SettingsHandler.LoadFromFile(oldFilePath, type, alternateXmlElements, this.GetType());
						if (settings != null)
							return (settings);
					}
				}

				// If nothing found, return <c>null</c>.
				return (null);
			}

			/// <summary>
			/// Tries to save settings to corresponding file path.
			/// </summary>
			/// <exception cref="Exception">
			/// Thrown if settings could not be saved.
			/// </exception>
			public virtual void Save()
			{
				AssertNotDisposed();

				SaveToFile(typeof(TSettings), this.filePath, this.settings);
			}

			[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
			private void SaveToFile(Type type, string filePath, object settings)
			{
				string backup = filePath + IO.FileEx.BackupFileExtension;

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
					using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8))
					{
						XmlSerializer serializer = new XmlSerializer(type);
						serializer.Serialize(sw, settings);
					}
				}
				catch
				{
					try
					{
						if (File.Exists(backup))
							File.Move(backup, filePath);
					}
					catch { }

					throw; // Re-throw!
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
			/// Force that settings are currently owned by this instance, i.e. on the next
			/// save request the settings will indeed be saved.
			/// </summary>
			public virtual void ForceThatSettingsAreCurrentlyOwnedByThisInstance()
			{
				AssertNotDisposed();

				this.areCurrentlyOwnedByThisInstance = true;
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
					Application.CommonAppDataPath + Path.DirectorySeparatorChar + CommonFileName
					);

			if (hasLocalUserSettings)
				this.localUserSettings = new Handler<TLocalUserSettings>
					(
					LocalUserName,
					Application.LocalUserAppDataPath + Path.DirectorySeparatorChar + LocalUserFileName
					);
			
			if (hasRoamingUserSettings)
				this.roamingUserSettings = new Handler<TRoamingUserSettings>
					(
					RoamingUserName,
					Application.UserAppDataPath + Path.DirectorySeparatorChar + RoamingUserFileName
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
				if (disposing)
				{
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
		protected bool IsDisposed
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
				// AssertNotDisposed() is called by 'Has' property below.

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
				// AssertNotDisposed() is called by 'Has' property below.

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
				// AssertNotDisposed() is called by 'Has' property below.

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
				// AssertNotDisposed() is called by 'Has' property below.

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
				// AssertNotDisposed() is called by 'Has' property below.

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
				// AssertNotDisposed() is called by 'Has' property below.
			
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
		/// they was no valid settings file and they were set to their defaults.
		/// </summary>
		public virtual bool CommonSettingsSuccessfullyLoaded
		{
			get
			{
				AssertNotDisposed();
				return (this.commonSettings.SuccessfullyLoaded);
			}
		}

		/// <summary>
		/// Returns whether local user settings have successfully been loaded, <c>false</c> if
		/// they was no valid settings file and they were set to their defaults.
		/// </summary>
		public virtual bool LocalUserSettingsSuccessfullyLoaded
		{
			get
			{
				AssertNotDisposed();
				return (this.localUserSettings.SuccessfullyLoaded);
			}
		}

		/// <summary>
		/// Returns whether roaming user settings have successfully been loaded, <c>false</c> if
		/// they was no valid settings file and they were set to their defaults.
		/// </summary>
		public virtual bool RoamingUserSettingsSuccessfullyLoaded
		{
			get
			{
				AssertNotDisposed();
				return (this.roamingUserSettings.SuccessfullyLoaded);
			}
		}

		/// <summary>
		/// Returns whether all settings have successfully been loaded, <c>false</c> if
		/// they were no valid settings files and they were set to their defaults.
		/// </summary>
		public virtual bool AllSettingsSuccessfullyLoaded
		{
			get
			{
				// AssertNotDisposed() is called by 'Has' properties below.

				if (HasCommonSettings && !CommonSettingsSuccessfullyLoaded)
					return (false);

				if (HasLocalUserSettings && !LocalUserSettingsSuccessfullyLoaded)
					return (false);

				if (HasRoamingUserSettings && !RoamingUserSettingsSuccessfullyLoaded)
					return (false);

				return (true);
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
			// AssertNotDisposed() is called by 'Load' methods below.

			LoadCommonSettings();
			LoadLocalUserSettings();
			LoadRoamingUserSettings();

			// Immediately try to save settings to reflect current version.
			try
			{
				Save();
			}
			catch { }

			// Return load result.
			return (AllSettingsSuccessfullyLoaded);
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
			// AssertNotDisposed() is called by 'Has' property below.

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
			// AssertNotDisposed() is called by 'Has' property below.

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
			// AssertNotDisposed() is called by 'Has' property below.

			if (HasRoamingUserSettings)
				return (this.roamingUserSettings.Load());
			else
				return (true);
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
		/// <exception cref="Exception">
		/// Thrown if settings could not be saved.
		/// </exception>
		public virtual void SaveCommonSettings()
		{
			// AssertNotDisposed() is called by 'Has' property below.

			if (HasCommonSettings && CommonSettingsAreCurrentlyOwnedByThisInstance)
				this.commonSettings.Save();
		}

		/// <summary>
		/// Tries to save settings to <see cref="LocalUserSettingsFilePath"/>.
		/// </summary>
		/// <exception cref="Exception">
		/// Thrown if settings could not be saved.
		/// </exception>
		public virtual void SaveLocalUserSettings()
		{
			// AssertNotDisposed() is called by 'Has' property below.

			if (HasLocalUserSettings && LocalUserSettingsAreCurrentlyOwnedByThisInstance)
				this.localUserSettings.Save();
		}

		/// <summary>
		/// Tries to save settings to <see cref="RoamingUserSettingsFilePath"/>.
		/// </summary>
		/// <exception cref="Exception">
		/// Thrown if settings could not be saved.
		/// </exception>
		public virtual void SaveRoamingUserSettings()
		{
			// AssertNotDisposed() is called by 'Has' property below.

			if (HasRoamingUserSettings && RoamingUserSettingsAreCurrentlyOwnedByThisInstance)
				this.roamingUserSettings.Save();
		}

		/// <summary>
		/// Force that common settings are currently owned by this instance, i.e. on the next
		/// save request the settings will indeed be saved.
		/// </summary>
		public virtual void ForceThatCommonSettingsAreCurrentlyOwnedByThisInstance()
		{
			// AssertNotDisposed() is called by 'Has' property below.

			if (HasCommonSettings)
				this.commonSettings.ForceThatSettingsAreCurrentlyOwnedByThisInstance();
		}

		/// <summary>
		/// Force that local user settings are currently owned by this instance, i.e. on the next
		/// save request the settings will indeed be saved.
		/// </summary>
		public virtual void ForceThatLocalUserSettingsAreCurrentlyOwnedByThisInstance()
		{
			// AssertNotDisposed() is called by 'Has' property below.

			if (HasLocalUserSettings)
				this.localUserSettings.ForceThatSettingsAreCurrentlyOwnedByThisInstance();
		}

		/// <summary>
		/// Force that roaming user settings are currently owned by this instance, i.e. on the next
		/// save request the settings will indeed be saved.
		/// </summary>
		public virtual void ForceThatRoamingUserSettingsAreCurrentlyOwnedByThisInstance()
		{
			// AssertNotDisposed() is called by 'Has' property below.

			if (HasRoamingUserSettings)
				this.roamingUserSettings.ForceThatSettingsAreCurrentlyOwnedByThisInstance();
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
