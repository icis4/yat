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
// MKY Version 1.0.28 Development
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
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using MKY.Diagnostics;
using MKY.Xml;

#endregion

namespace MKY.Settings
{
	/// <summary>
	/// Options to control access to the applications settings file(s).
	/// </summary>
	public enum ApplicationSettingsFileAccess
	{
		/// <summary>Do no access the file at all, only use temporary settings.</summary>
		/// <remarks>Can be used to prevent concurrent instances to write the settings file(s).</remarks>
		None = 0,

		/// <summary>Only read the settings.</summary>
		/// <remarks>Can be used to prevent concurrent instances to write the settings file(s).</remarks>
		ReadShared = 1,

		/// <summary>Default, read and write the settings.</summary>
		ReadSharedWriteIfOwned = 3
	}

	/// <summary>
	/// Generic class to handle standard application settings. It covers common, local user
	/// as well as roaming user settings.
	/// </summary>
	/// <typeparam name="TCommonSettings">The type of the common settings.</typeparam>
	/// <typeparam name="TLocalUserSettings">The type of the local user settings.</typeparam>
	/// <typeparam name="TRoamingUserSettings">The type of the roaming user settings.</typeparam>
	/// <remarks>
	/// Pass <see cref="EmptySettingsItem"/> for those settings that shall not be used.
	/// </remarks>
	[SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes", Justification = "Three type parameters are given by the nature of application settings.")]
	public class ApplicationSettingsHandler<TCommonSettings, TLocalUserSettings, TRoamingUserSettings> : IDisposable, IDisposableEx
		where TCommonSettings : SettingsItem, new()
		where TLocalUserSettings : SettingsItem, new()
		where TRoamingUserSettings : SettingsItem, new()
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		[Flags]
		private enum FileAccessFlags
		{
			None =  0,
			Read =  1,
			Write = 2,

			ReadWrite = (Read | Write)
		}

		private class Handler<TSettings> : SettingsFileHandler, IDisposable, IDisposableEx
			where TSettings : SettingsItem, new()
		{
			#region Fields
			//======================================================================================
			// Fields
			//======================================================================================

			private string name;
			private TSettings settings;
			private IEnumerable<AlternateXmlElement> alternateXmlElements;
			private ApplicationSettingsFileAccess desiredFileAccess;
			private FileAccessFlags effectiveFileAccess;
			private Mutex mutex;
			private bool mutexCreatedNew;

			#endregion

			#region Object Lifetime
			//======================================================================================
			// Object Lifetime
			//======================================================================================

			/// <param name="name">The name of the settings.</param>
			/// <param name="filePath">The file path to the settings file.</param>
			/// <param name="desiredFileAccess">The file access of the settings file.</param>
			public Handler(string name, string filePath, ApplicationSettingsFileAccess desiredFileAccess)
				: base(filePath)
			{
				this.name = name;
				this.settings = new TSettings();

				var aep = (this.settings as IAlternateXmlElementProvider);
				if (aep != null)
					this.alternateXmlElements = aep.AlternateXmlElements;

				this.desiredFileAccess = desiredFileAccess;
				switch (this.desiredFileAccess)
				{
					case ApplicationSettingsFileAccess.ReadSharedWriteIfOwned:
					{
						// \remind (2019-08-21 / MKY)
						// The named mutex does include the name (e.g. "LocalUserSettings"), thus
						// will not interfere with other settings of the same application. But it
						// does not include the application version. As a consequence, another of
						// the same application can "block" even though it does not reference the
						// same settings, since they are located in <Version> subfolders. This
						// approach takes cases into account, where a newer setting still references
						// older (sub)settings.

					////                            vvv prepared for FR #37 "clone old auto files and upgrade them"
					////// The name does include the settings name as well as application version
					////// for neither interfering with other settings of the same application nor
					////// other versions of the same application. The latter works fine once a
					////// newer version does have version specific settings, but may cause issues
					////// at the moment an old version is already running and the new version is
					////// started for the first time. This case has to be handled by copying old
					////// (sub)settings to the new version before accessing them.

						var sb = new StringBuilder();
						sb.Append(Application.ProductName);
						sb.Append("."); // e.g. "YAT.LocalUserSettings" same as namespaces.
						sb.Append(this.name);
					////sb.Append(Path.DirectorySeparatorChar); // e.g. "YAT.LocalUserSettings\x.y.z" same
					////sb.Append(Application.ProductVersion);  // as e.g. prefix "Local\" (see Mutex).
					////                            ^^^ prepared for FR #37 "clone old auto files and upgrade them"

						// Create named mutex and try to acquire it. Note that the mutex must be
						// acquired immediately, and once only, and the success is stored in a
						// boolean variable. This mechanism ensures that once an instance 'owns'
						// the settings, it keeps them. And does so until it exits. Then the mutex
						// is released by the finalizer of this class.

						this.mutex = new Mutex(true, sb.ToString(), out this.mutexCreatedNew);

						if (this.mutexCreatedNew)
							this.effectiveFileAccess = FileAccessFlags.ReadWrite;
						else
							this.effectiveFileAccess = FileAccessFlags.Read;

						break;
					}

					case ApplicationSettingsFileAccess.ReadShared:
					{
						this.effectiveFileAccess = FileAccessFlags.Read;
						break;
					}

					default: // Default also covers 'ApplicationSettingsFileAccess.None'.
					{
						this.effectiveFileAccess = FileAccessFlags.None;
						break;
					}
				}
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
					// The mutex must be closed and released by the application because it would
					// be called from the wrong process if it was closed by the garbage collector.

					// Dispose of managed resources if requested:
					if (disposing)
					{
						if (this.mutex != null)
							this.mutex.Close();
					}

					// Set state to disposed:
					this.mutex = null;
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
			~Handler()
			{
				Dispose(false);

				DebugDisposal.DebugNotifyFinalizerInsteadOfDispose(this);
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
			//======================================================================================
			// Properties
			//======================================================================================

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
					// Do not call AssertNotDisposed() in a simple get-property.

					return ((this.effectiveFileAccess & FileAccessFlags.Write) != 0);
				}
			}

			#endregion

			#region Methods
			//======================================================================================
			// Methods
			//======================================================================================

			/// <summary>
			/// Tries to load settings from corresponding file path.
			/// </summary>
			/// <returns>
			/// Returns <c>true</c> if settings could be loaded from the given file path,
			/// returns <c>false</c> if they could not be loaded and were set to defaults instead.
			/// </returns>
			[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
			public virtual bool Load()
			{
				AssertNotDisposed();

				if ((this.effectiveFileAccess & FileAccessFlags.Read) != 0)
				{
					// Try to open existing file of current version:
					try
					{
						var settings = LoadFromFile<TSettings>(this.alternateXmlElements);
						if (settings != null)
						{
							this.settings = settings;
							return (true);
						}
					}
					catch (Exception ex)
					{
						DebugEx.WriteException(GetType(), ex, "Exception while loading current application file!");
					}

					// Alternatively, try to open an existing file of an older version:
					if (FilePathIsDefined)
					{
						// Find all valid directories of older versions:
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
							catch (Exception ex)
							{
								DebugEx.WriteException(GetType(), ex, "Exception while searching through directories!");
							}
						}

						// Iterate through the directories, start with most recent:
						string fileName = Path.GetFileName(FilePath);
						oldDirectories.Sort();
						for (int i = oldDirectories.Count - 1; i >= 0; i--)
						{
							string oldFilePath = oldDirectories[i] + Path.DirectorySeparatorChar + fileName;
							try
							{
								var settings = LoadFromFile<TSettings>(this.alternateXmlElements, oldFilePath);
								if (settings != null)
								{
									this.settings = settings;
									return (true);
								}
							}
							catch (Exception ex)
							{
								DebugEx.WriteException(GetType(), ex, "Exception while loading recent application file" + Environment.NewLine + oldFilePath);
							}
						}
					}
				}

				// Nothing found, return default settings:
				this.settings = new TSettings();
				return (false);
			}

			/// <summary>
			/// Tries to save settings to corresponding file path.
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

				if ((this.effectiveFileAccess & FileAccessFlags.Write) != 0)
				{
					SaveToFile<TSettings>(this.settings);
					this.settings.ClearChanged();
				}
			}

			/// <summary>
			/// Close the settings and release all resources.
			/// </summary>
			public virtual void Close()
			{
				if (IsDisposed)
					return; // Close() shall be callable on a disposed object.

				if (this.mutex != null)
				{
					if (this.mutexCreatedNew)
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
		public ApplicationSettingsHandler(ApplicationSettingsFileAccess commonSettingsFileAccess, ApplicationSettingsFileAccess localUserSettingsFileAccess, ApplicationSettingsFileAccess roamingUserSettingsFileAccess)
		{
			if (typeof(TCommonSettings) != typeof(EmptySettingsItem))
			{
				this.commonSettings = new Handler<TCommonSettings>
				(
					CommonName,
					Application.CommonAppDataPath + Path.DirectorySeparatorChar + CommonFileName,
					commonSettingsFileAccess
				);
			}

			if (typeof(TLocalUserSettings) != typeof(EmptySettingsItem))
			{
				this.localUserSettings = new Handler<TLocalUserSettings>
				(
					LocalUserName,
					Application.LocalUserAppDataPath + Path.DirectorySeparatorChar + LocalUserFileName,
					localUserSettingsFileAccess
				);
			}

			if (typeof(TRoamingUserSettings) != typeof(EmptySettingsItem))
			{
				this.roamingUserSettings = new Handler<TRoamingUserSettings>
				(
					RoamingUserName,
					Application.UserAppDataPath + Path.DirectorySeparatorChar + RoamingUserFileName,
					roamingUserSettingsFileAccess
				);
			}
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
				// Dispose of managed resources if requested:
				if (disposing)
				{
					// In the 'normal' case, all settings have already been closed in Close().
					if (this.commonSettings != null)
						this.commonSettings.Dispose();

					if (this.localUserSettings != null)
						this.localUserSettings.Dispose();

					if (this.roamingUserSettings != null)
						this.roamingUserSettings.Dispose();
				}

				// Set state to disposed:
				this.commonSettings = null;
				this.localUserSettings = null;
				this.roamingUserSettings = null;
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
		~ApplicationSettingsHandler()
		{
			Dispose(false);

			DebugDisposal.DebugNotifyFinalizerInsteadOfDispose(this);
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
		/// Absolute path to common settings file, if has common settings,
		/// <see cref="String.Empty"/> otherwise.
		/// </summary>
		/// <exception cref="NullReferenceException">
		/// Thrown if attempted to set file if this handler has no common settings.
		/// </exception>
		public virtual string CommonSettingsFilePath
		{
			get
			{
				// AssertNotDisposed() is called by 'Has...' below.

				if (HasCommonSettings)
					return (this.commonSettings.FilePath);
				else
					return (string.Empty);
			}
			set
			{
				// AssertNotDisposed() is called by 'Has...' below.

				if (HasCommonSettings)
					this.commonSettings.FilePath = value;
				else
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "This handler has no common settings!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary>
		/// Absolute path to local user settings file, if has local user settings,
		/// <see cref="String.Empty"/> otherwise.
		/// </summary>
		/// <exception cref="NullReferenceException">
		/// Thrown if attempted to set file if this handler has no local user settings.
		/// </exception>
		public virtual string LocalUserSettingsFilePath
		{
			get
			{
				// AssertNotDisposed() is called by 'Has...' below.

				if (HasLocalUserSettings)
					return (this.localUserSettings.FilePath);
				else
					return (string.Empty);
			}
			set
			{
				// AssertNotDisposed() is called by 'Has...' below.

				if (HasLocalUserSettings)
					this.localUserSettings.FilePath = value;
				else
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "This handler has no local user settings!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary>
		/// Absolute path to roaming user settings file, if has roaming user settings,
		/// <see cref="String.Empty"/> otherwise.
		/// </summary>
		/// <exception cref="NullReferenceException">
		/// Thrown if attempted to set file if this handler has no roaming user settings.
		/// </exception>
		public virtual string RoamingUserSettingsFilePath
		{
			get
			{
				// AssertNotDisposed() is called by 'Has...' below.

				if (HasRoamingUserSettings)
					return (this.roamingUserSettings.FilePath);
				else
					return (string.Empty);
			}
			set
			{
				// AssertNotDisposed() is called by 'Has...' below.

				if (HasRoamingUserSettings)
					this.roamingUserSettings.FilePath = value;
				else
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "This handler has no roaming user settings!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary>
		/// Handler to common settings, if has common settings, <c>null</c> otherwise.
		/// </summary>
		public virtual TCommonSettings CommonSettings
		{
			get
			{
				// AssertNotDisposed() is called by 'Has...' below.

				if (HasCommonSettings)
					return (this.commonSettings.Settings);
				else
					return (default(TCommonSettings));
			}
		}

		/// <summary>
		/// Handler to local user settings, if has local user settings, <c>null</c> otherwise.
		/// </summary>
		public virtual TLocalUserSettings LocalUserSettings
		{
			get
			{
				// AssertNotDisposed() is called by 'Has...' below.

				if (HasLocalUserSettings)
					return (this.localUserSettings.Settings);
				else
					return (default(TLocalUserSettings));
			}
		}

		/// <summary>
		/// Handler to roaming user settings, if has roaming user settings, <c>null</c> otherwise.
		/// </summary>
		public virtual TRoamingUserSettings RoamingUserSettings
		{
			get
			{
				// AssertNotDisposed() is called by 'Has...' below.

				if (HasRoamingUserSettings)
					return (this.roamingUserSettings.Settings);
				else
					return (default(TRoamingUserSettings));
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
				// AssertNotDisposed() is called by 'Has...' below.

				if (HasCommonSettings)
					return (this.commonSettings.FileSuccessfullyLoaded);
				else
					return (false);
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
				// AssertNotDisposed() is called by 'Has...' below.

				if (HasLocalUserSettings)
					return (this.localUserSettings.FileSuccessfullyLoaded);
				else
					return (false);
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
				// AssertNotDisposed() is called by 'Has...' below.

				if (HasRoamingUserSettings)
					return (this.roamingUserSettings.FileSuccessfullyLoaded);
				else
					return (false);
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
				// AssertNotDisposed() is called by 'Has...' below.

				if (HasCommonSettings)
					return (this.commonSettings.AreCurrentlyOwnedByThisInstance);
				else
					return (false);
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
				// AssertNotDisposed() is called by 'Has...' below.

				if (HasLocalUserSettings)
					return (this.localUserSettings.AreCurrentlyOwnedByThisInstance);
				else
					return (false);
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
				// AssertNotDisposed() is called by 'Has...' below.

				if (HasRoamingUserSettings)
					return (this.roamingUserSettings.AreCurrentlyOwnedByThisInstance);
				else
					return (false);
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
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		public virtual bool Load()
		{
			// AssertNotDisposed() is called by 'Load()' below.

			bool success = true;

			if (!LoadCommonSettings())
				success = false;

			if (!LoadLocalUserSettings())
				success = false;

			if (!LoadRoamingUserSettings())
				success = false;

			// Immediately try to save settings to reflect current version:
			try
			{
				Save();
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(GetType(), ex, "Exception while initially saving settings!");
			}

			// Return load result:
			return (success);
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
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		/// <exception cref="Exception">
		/// Thrown if settings could not be saved.
		/// </exception>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
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
				throw (result); // Pure re-throw not possible here.
		}

		/// <summary>
		/// Tries to save settings to <see cref="CommonSettingsFilePath"/>.
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
		///
		/// Saying hello to StyleCop ;-.
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
		///
		/// Saying hello to StyleCop ;-.
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
		/// Close the application settings.
		/// </summary>
		public virtual void Close()
		{
			if (IsDisposed)
				return; // Close() shall be callable on a disposed object.

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
