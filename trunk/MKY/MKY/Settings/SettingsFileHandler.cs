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
// Copyright © 2003-2020 Matthias Kläy.
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

using MKY.Diagnostics;
using MKY.IO;
using MKY.Xml;
using MKY.Xml.Serialization;

#endregion

namespace MKY.Settings
{
	/// <summary>
	/// Utility class to provide basic settings file handling methods.
	/// </summary>
	public class SettingsFileHandler
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private string filePath;

		private bool successfullyLoaded;

		private string lastAccessFilePath;
		private DateTime lastAccessTimeUtc;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public SettingsFileHandler()
			: this(null)
		{
		}

		/// <summary></summary>
		public SettingsFileHandler(string filePath)
		{
			this.filePath = filePath;
		}

	#if (DEBUG)

		/// <remarks>
		/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
		/// </remarks>
		[SuppressMessage("Microsoft.Performance", "CA1821:RemoveEmptyFinalizers", Justification = "See remarks.")]
		~SettingsFileHandler()
		{
			DebugFinalization.DebugNotifyFinalizerAndCheckWhetherOverdue(this);
		}

	#endif // DEBUG

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary>
		/// Absolute path to settings file.
		/// </summary>
		public virtual string FilePath
		{
			get { return (this.filePath); }
			set { this.filePath = value;  }
		}

		/// <summary>
		/// Determines whether the settings file path is defined.
		/// </summary>
		public virtual bool FilePathIsDefined
		{
			get { return (!string.IsNullOrEmpty(this.filePath)); }
		}

		/// <summary>
		/// Determines whether the settings file path is valid.
		/// </summary>
		public virtual bool FilePathIsValid
		{
			get { return (PathEx.IsValid(this.filePath)); }
		}

		/// <summary>
		/// Determines whether the settings file exists.
		/// </summary>
		public virtual bool FileExists
		{
			get { return (File.Exists(this.filePath)); }
		}

		/// <summary>
		/// Determines whether the settings were loaded from a file but that doesn't exist anymore.
		/// </summary>
		public virtual bool FileExistsNoMore
		{
			get { return (FileSuccessfullyLoaded && !FileExists); }
		}

		/// <summary>
		/// Determines whether the settings file is up to date.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		public virtual bool FileIsUpToDate
		{
			get
			{
				// String validation and file existence:
				if (!FileExists)
					return (false);

				// Return whether current settings file is still the same as at the last access.
				if (!PathEx.Equals(this.filePath, this.lastAccessFilePath))
					return (false);

				try
				{
					DateTime lastAccessTimeUtc = File.GetLastAccessTimeUtc(this.filePath);
					return (lastAccessTimeUtc == this.lastAccessTimeUtc);
				}
				catch
				{
					return (false);
				}
			}
		}

		/// <summary>
		/// Determines whether the settings file is readable, i.e. exists and can be accessed.
		/// </summary>
		public virtual bool FileIsReadable
		{
			get { return (FileEx.IsReadable(this.filePath)); }
		}

		/// <summary>
		/// Determines whether the settings file is read-only, i.e. exists and is read-only.
		/// </summary>
		public virtual bool FileIsReadOnly
		{
			get { return (FileEx.IsReadOnly(this.filePath)); }
		}

		/// <summary>
		/// Determines whether the settings file path is writeable, i.e. is not read-only or the file doesn't exist yet.
		/// </summary>
		public virtual bool FileIsWritable
		{
			get { return (FileEx.IsWritable(this.filePath)); }
		}

		/// <summary>
		/// Determines whether the settings file has successfully been loaded, <c>false</c> if there was
		/// no valid settings file available.
		/// </summary>
		public virtual bool FileSuccessfullyLoaded
		{
			get { return (this.successfullyLoaded); }
		}

		/// <summary>
		/// Determines whether settings are feasible to be saved to the settings file.
		/// </summary>
		public virtual bool SaveIsFeasible
		{
			get
			{
				// Save is not feasible for files without valid file path:
				if (!FilePathIsValid)
					return (false);

				// Save is not feasible for write-protected files:
				if (!FileIsWritable)
					return (false);

				return (true);
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		#region Methods > Load
		//------------------------------------------------------------------------------------------
		// Methods > Load
		//------------------------------------------------------------------------------------------

		/// <typeparam name="T">The settings type.</typeparam>
		/// <exception cref="Exception">
		/// Thrown if settings could not be created.
		/// </exception>
		public T LoadFromFile<T>()
		{
			return (LoadFromFile<T>(null));
		}

		/// <typeparam name="T">The settings type.</typeparam>
		/// <exception cref="Exception">
		/// Thrown if settings could not be created.
		/// </exception>
		public T LoadFromFile<T>(IEnumerable<AlternateXmlElement> alternateXmlElements)
		{
			return (LoadFromFile<T>(alternateXmlElements, this.filePath));
		}

		/// <summary>
		/// This method loads settings from a file. If the schema of the settings doesn't match,
		/// this method tries to load the settings using tolerant XML interpretation by making use
		/// of <see cref="TolerantXmlSerializer"/> or <see cref="AlternateTolerantXmlSerializer"/>.
		/// </summary>
		/// <typeparam name="T">The settings type.</typeparam>
		/// <exception cref="Exception">
		/// Thrown if settings could not be created.
		/// </exception>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		public T LoadFromFile<T>(IEnumerable<AlternateXmlElement> alternateXmlElements, string filePath)
		{
			bool success = false;
			object result = null; // If not successful, return <c>null</c>.

			// First check for file to minimize exceptions thrown:
			if (FileEx.IsReadable(filePath))
			{
				try
				{
					result = XmlSerializerEx.DeserializeFromFileInsisting(typeof(T), alternateXmlElements, filePath);
					success = true;
				}
				catch (Exception)
				{
					throw; // Rethrow!
				}
			}

			if (success)
			{
				this.successfullyLoaded = true;

				this.lastAccessFilePath = filePath;
				this.lastAccessTimeUtc = File.GetLastAccessTimeUtc(filePath);
			}

			return ((T)result);
		}

		#endregion

		#region Methods > Save
		//------------------------------------------------------------------------------------------
		// Methods > Save
		//------------------------------------------------------------------------------------------

		/// <typeparam name="T">The settings type.</typeparam>
		/// <exception cref="Exception">
		/// Thrown if settings could not be saved.
		/// </exception>
		public void SaveToFile<T>(T settings)
		{
			SaveToFile(settings, this.filePath);
		}

		/// <typeparam name="T">The settings type.</typeparam>
		/// <exception cref="Exception">
		/// Thrown if settings could not be saved.
		/// </exception>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		public void SaveToFile<T>(T settings, string filePath)
		{
			string backup = PathEx.GetUniqueTempPath(); // Backup file can be located anywhere.

			try
			{
				if (File.Exists(filePath))
					File.Move(filePath, backup);
			}
			catch (Exception exBackup)
			{
				DebugEx.WriteException(GetType(), exBackup, "Exception while backing file up!");
			}

			try
			{
				XmlSerializerEx.SerializeToFile(typeof(T), settings, filePath);

				this.lastAccessFilePath = filePath;
				this.lastAccessTimeUtc = File.GetLastAccessTimeUtc(filePath);
			}
			catch (Exception exPrimary)
			{
				DebugEx.WriteException(GetType(), exPrimary, "Exception while saving file!");

				try
				{
					if (File.Exists(backup))
						File.Move(backup, filePath);
				}
				catch (Exception exRestore)
				{
					DebugEx.WriteException(GetType(), exRestore, "Exception while restoring backup file!");
				}

				throw; // Rethrow!
			}
			finally
			{
				try
				{
					if (File.Exists(backup))
						File.Delete(backup);
				}
				catch (Exception exCleanup)
				{
					DebugEx.WriteException(GetType(), exCleanup, "Exception while removing backup file!");
				}
			}
		}

		#endregion

		#region Methods > Delete
		//------------------------------------------------------------------------------------------
		// Methods > Delete
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Tries to delete file <see cref="FilePath"/>.
		/// </summary>
		/// <returns>
		/// Returns <c>true</c> if file successfully saved.
		/// </returns>
		public virtual bool TryDelete()
		{
			return (FileEx.TryDelete(this.filePath));
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
