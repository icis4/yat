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
// MKY Development Version 1.0.14
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

		private Type parentType;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public SettingsFileHandler()
			: this("", null)
		{
		}

		/// <summary></summary>
		public SettingsFileHandler(string filePath)
			: this(filePath, null)
		{
		}

		/// <summary></summary>
		public SettingsFileHandler(Type parentType)
			: this("", parentType)
		{
		}

		/// <summary></summary>
		public SettingsFileHandler(string filePath, Type parentType)
		{
			this.filePath = filePath;
			this.parentType = parentType;
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary>
		/// Complete path to settings file.
		/// </summary>
		public virtual string FilePath
		{
			get { return (this.filePath); }
			set { this.filePath = value;  }
		}

		/// <summary>
		/// Returns whether the settings file path is defined.
		/// </summary>
		public virtual bool FilePathIsDefined
		{
			get { return (PathEx.IsDefined(this.filePath)); }
		}

		/// <summary>
		/// Returns whether the settings file path is valid.
		/// </summary>
		public virtual bool FilePathIsValid
		{
			get { return (PathEx.IsValid(this.filePath)); }
		}

		/// <summary>
		/// Returns whether the settings file exists.
		/// </summary>
		public virtual bool FileExists
		{
			get { return (File.Exists(this.filePath)); }
		}

		/// <summary>
		/// Returns whether the settings were loaded from a file but that doesn't exist anymore.
		/// </summary>
		public virtual bool FileExistsNoMore
		{
			get { return (FileSuccessfullyLoaded && !FileExists); }
		}

		/// <summary>
		/// Returns whether the settings file is up to date.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
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
		/// Returns whether setting file is readable.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		[SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "fi", Justification = "Required to force exception.")]
		public virtual bool FileIsReadable
		{
			get
			{
				// String validation and file existence:
				if (!FileExists)
					return (false);

				return (FileEx.IsReadable(this.filePath));
			}
		}

		/// <summary>
		/// Returns whether setting file is read-only.
		/// </summary>
		public virtual bool FileIsReadOnly
		{
			get
			{
				// String validation and file existence:
				if (!FileExists)
					return (false);

				return (FileEx.IsReadOnly(this.filePath));
			}
		}

		/// <summary>
		/// Returns whether setting file is writable.
		/// </summary>
		public virtual bool FileIsWritable
		{
			get { return (!FileIsReadOnly); }
		}

		/// <summary>
		/// Returns whether setting file has successfully been loaded, <c>false</c> if there was
		/// no valid settings file available.
		/// </summary>
		public virtual bool FileSuccessfullyLoaded
		{
			get { return (this.successfullyLoaded); }
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

		/// <summary></summary>
		public object LoadFromFile(Type type)
		{
			return (LoadFromFile(type, null));
		}

		/// <summary></summary>
		public object LoadFromFile(Type type, AlternateXmlElement[] alternateXmlElements)
		{
			return (LoadFromFile(this.filePath, type, alternateXmlElements));
		}

		/// <summary>
		/// This method loads settings from a file. If the schema of the settings doesn't match,
		/// this method tries to load the settings using tolerant XML interpretation by making use
		/// of <see cref="TolerantXmlSerializer"/> or <see cref="AlternateTolerantXmlSerializer"/>.
		/// </summary>
		/// <remarks>
		/// There are some issues with tolerant XML interpretation in case of lists. See YAT bug
		/// #3537940>#232 "Issues with TolerantXmlSerializer" for details. The following solutions
		/// could fix these issues:
		///  > Fix these issues in <see cref="TolerantXmlSerializer"/>
		///  > Implement a different variant of tolerant XML interpretation
		///     > Use of the default XML serialization to only process parts of the XML tree
		///  > Use of SerializationBinder (feature request #3392369)
		///  > Use of XSLT
		///     > Requires that every setting's schema is archived
		///     > Requires an incremental XSLT chain from every former schema
		///       (Alternatively, an immediate XSLT from every former schema)
		/// 
		/// Decision 2012-06: For the moment, the current solution is kept, rationale:
		///  > Creating an XSLT is time consuming for each release
		///  > Creating an XSLT fully or partly automatically requires an (expensive) tool
		///  > Current solution isn't perfect but good enough and easy to handle (no efforts)
		///  > Current solution also works for other software that makes use of MKY or YAT code
		/// 
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		public object LoadFromFile(string filePath, Type type, AlternateXmlElement[] alternateXmlElements)
		{
			bool success = false;
			object result = null; // If not successful, return <c>null</c>.

			// First check for file to minimize exceptions thrown.
			if (File.Exists(filePath) && FileEx.IsReadable(filePath))
			{
				// First, always try standard deserialization:
				//  > This is the fastest way of deserialization
				//  > Tolerant deserialization has some severe issues (see comments above)
				//
				// However:
				// Standard deserialization might succeed even with an outdated schema! Then,
				// available alternate elements are not considered. But this issue is considered
				// less severe than the issues described above.
				try
				{
					result = XmlSerializerEx.DeserializeFromFile(filePath, type);
					success = true;
				}
				catch
				{
					if (alternateXmlElements == null)
					{
						// Try to open existing file with tolerant deserialization.
						try
						{
							result = XmlSerializerEx.TolerantDeserializeFromFile(filePath, type);
							success = true;
						}
						catch (Exception ex)
						{
							DebugEx.WriteException(this.parentType, ex);
						}
					}
					else
					{
						// Try to open existing file with alternate-tolerant deserialization.
						try
						{
							result = XmlSerializerEx.AlternateTolerantDeserializeFromFile(filePath, type, alternateXmlElements);
							success = true;
						}
						catch (Exception ex)
						{
							DebugEx.WriteException(this.parentType, ex);
						}
					}
				}
			}

			if (success)
			{
				this.successfullyLoaded = true;

				this.lastAccessFilePath = filePath;
				this.lastAccessTimeUtc = File.GetLastAccessTimeUtc(filePath);
			}

			return (result);
		}

		#endregion

		#region Methods > Save
		//------------------------------------------------------------------------------------------
		// Methods > Save
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		public bool SaveToFile(Type type, object settings)
		{
			return (SaveToFile(this.filePath, type, settings));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		public bool SaveToFile(string filePath, Type type, object settings)
		{
			bool success = false;

			if (PathEx.IsValid(filePath) && FileEx.IsWritable(filePath))
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
					XmlSerializerEx.SerializeToFile(filePath, type, settings);

					this.lastAccessFilePath = filePath;
					this.lastAccessTimeUtc = File.GetLastAccessTimeUtc(filePath);

					success = true;
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

			return (success);
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
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
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
