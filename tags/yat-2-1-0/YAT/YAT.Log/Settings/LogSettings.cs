﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT Version 2.1.0
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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Serialization;

using MKY;
using MKY.IO;
using MKY.Text;

using YAT.Application.Utilities;
using YAT.Settings.Application;

#endregion

// Placed into a separate '.Settings' namespace for consistency with all other '...Settings' classes.
namespace YAT.Log.Settings
{
	/// <remarks>
	/// Same naming postfix as all other '...Settings' classes.
	/// </remarks>
	public class LogSettings : MKY.Settings.SettingsItem, IEquatable<LogSettings>
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		// Root:
		private string rootPath;
		private string rootFileName;

		// Port:
		private bool portLog;
		private string portExtension;

		// Raw:
		private bool rawLogTx;
		private bool rawLogBidir;
		private bool rawLogRx;
		private string rawExtension;

		// Neat:
		private bool neatLogTx;
		private bool neatLogBidir;
		private bool neatLogRx;
		private string neatExtension;

		// File name:
		private bool nameType;
		private bool nameDirection;
		private bool nameDate;
		private bool nameTime;
		private FileNameSeparatorEx nameSeparator;

		// File folder:
		private bool folderType;
		private bool folderDirection;

		// Write mode:
		private LogFileWriteMode writeMode;

		// Encoding:
		private TextEncoding textEncoding;
		private bool emitEncodingPreamble;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public LogSettings()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <summary></summary>
		public LogSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public LogSettings(LogSettings rhs)
			: base(rhs)
		{
			RootPath        = rhs.RootPath;
			RootFileName    = rhs.RootFileName;

			PortLog         = rhs.PortLog;
			PortExtension   = rhs.PortExtension;

			RawLogTx        = rhs.RawLogTx;
			RawLogBidir     = rhs.RawLogBidir;
			RawLogRx        = rhs.RawLogRx;
			RawExtension    = rhs.RawExtension;

			NeatLogTx       = rhs.NeatLogTx;
			NeatLogBidir    = rhs.NeatLogBidir;
			NeatLogRx       = rhs.NeatLogRx;
			NeatExtension   = rhs.NeatExtension;

			NameType        = rhs.NameType;
			NameDirection   = rhs.NameDirection;
			NameDate        = rhs.NameDate;
			NameTime        = rhs.NameTime;
			NameSeparator   = rhs.NameSeparator;

			FolderType      = rhs.FolderType;
			FolderDirection = rhs.FolderDirection;

			WriteMode       = rhs.WriteMode;

			TextEncoding    = rhs.TextEncoding;
			EmitEncodingPreamble = rhs.EmitEncodingPreamble;

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			ApplicationSettings.LocalUserSettings.Paths.LogFiles = Application.Settings.PathSettings.LogFilesDefault;

			RootPath     = ApplicationSettings.LocalUserSettings.Paths.LogFiles;
			RootFileName = ApplicationEx.ProductName + "-Log"; // File location shall be separate for "YAT" and "YATConsole".

			PortLog         = false;
			PortExtension   = ApplicationSettings.RoamingUserSettings.Extensions.PortLogFiles;

			RawLogTx        = false;
			RawLogBidir     = false;
			RawLogRx        = false;
			RawExtension    = ApplicationSettings.RoamingUserSettings.Extensions.RawLogFiles;

			NeatLogTx       = false;
			NeatLogBidir    = true;
			NeatLogRx       = false;
			NeatExtension   = ApplicationSettings.RoamingUserSettings.Extensions.NeatLogFiles;

			NameType        = false;
			NameDirection   = false;
			NameDate        = true;
			NameTime        = true;
			NameSeparator   = FileNameSeparator.Dash;

			FolderType      = false;
			FolderDirection = false;

			WriteMode       = LogFileWriteMode.Create;

			TextEncoding         = TextEncoding.UTF8;
			EmitEncodingPreamble = EncodingEx.EnvironmentRecommendsByteOrderMarks;
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("RootPath")]
		public virtual string RootPath
		{
			get { return (this.rootPath); }
			set
			{
				if (this.rootPath != value)
				{
					this.rootPath = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("RootFileName")]
		public virtual string RootFileName
		{
			get { return (this.rootFileName); }
			set
			{
				if (this.rootFileName != value)
				{
					this.rootFileName = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual string RootFilePath
		{
			get { return (this.rootPath + Path.DirectorySeparatorChar + this.rootFileName); }
		}

		//- Port ------------------------------------------------------------------

		/// <summary></summary>
		[XmlElement("PortLog")]
		public virtual bool PortLog
		{
			get { return (this.portLog); }
			set
			{
				if (this.portLog != value)
				{
					this.portLog = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public string PortRootRelativeFilePath
		{
			get
			{
				return (ToDirectoryString(LogType.Port, LogDirection.None) +
						this.rootFileName +
						ToFileNamePostFixString(LogType.Port, LogDirection.None) +
						this.portExtension);
			}
		}

		/// <summary>
		/// Builds the absolute or relative path to the log file, expanding environment variables.
		/// </summary>
		public virtual string MakePortFilePath()
		{
			return (Environment.ExpandEnvironmentVariables(this.rootPath + Path.DirectorySeparatorChar + PortRootRelativeFilePath));
		}

		/// <summary></summary>
		[XmlElement("PortExtension")]
		public virtual string PortExtension
		{
			get { return (this.portExtension); }
			set
			{
				if (this.portExtension != value)
				{
					this.portExtension = value;
					SetMyChanged();
				}
			}
		}

		//- Raw -------------------------------------------------------------------

		/// <summary></summary>
		[XmlElement("RawLogTx")]
		public virtual bool RawLogTx
		{
			get { return (this.rawLogTx); }
			set
			{
				if (this.rawLogTx != value)
				{
					this.rawLogTx = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public string RawTxRootRelativeFilePath
		{
			get
			{
				return (ToDirectoryString(LogType.Raw, LogDirection.Tx) +
						this.rootFileName +
						ToFileNamePostFixString(LogType.Raw, LogDirection.Tx) +
						this.rawExtension);
			}
		}

		/// <summary>
		/// Builds the absolute or relative path to the log file, expanding environment variables.
		/// </summary>
		public virtual string MakeRawTxFilePath()
		{
			return (Environment.ExpandEnvironmentVariables(this.rootPath + Path.DirectorySeparatorChar + RawTxRootRelativeFilePath));
		}

		/// <summary></summary>
		[XmlElement("RawLogBidir")]
		public virtual bool RawLogBidir
		{
			get { return (this.rawLogBidir); }
			set
			{
				if (this.rawLogBidir != value)
				{
					this.rawLogBidir = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public string RawBidirRootRelativeFilePath
		{
			get
			{
				return (ToDirectoryString(LogType.Raw, LogDirection.Bidir) +
						this.rootFileName +
						ToFileNamePostFixString(LogType.Raw, LogDirection.Bidir) +
						this.rawExtension);
			}
		}

		/// <summary>
		/// Builds the absolute or relative path to the log file, expanding environment variables.
		/// </summary>
		public virtual string MakeRawBidirFilePath()
		{
			return (Environment.ExpandEnvironmentVariables(this.rootPath + Path.DirectorySeparatorChar + RawBidirRootRelativeFilePath));
		}

		/// <summary></summary>
		[XmlElement("RawLogRx")]
		public virtual bool RawLogRx
		{
			get { return (this.rawLogRx); }
			set
			{
				if (this.rawLogRx != value)
				{
					this.rawLogRx = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public string RawRxRootRelativeFilePath
		{
			get
			{
				return (ToDirectoryString(LogType.Raw, LogDirection.Rx) +
						this.rootFileName +
						ToFileNamePostFixString(LogType.Raw, LogDirection.Rx) +
						this.rawExtension);
			}
		}

		/// <summary>
		/// Builds the absolute or relative path to the log file, expanding environment variables.
		/// </summary>
		public virtual string MakeRawRxFilePath()
		{
			return (Environment.ExpandEnvironmentVariables(this.rootPath + Path.DirectorySeparatorChar + RawRxRootRelativeFilePath));
		}

		/// <summary></summary>
		[XmlElement("RawExtension")]
		public virtual string RawExtension
		{
			get { return (this.rawExtension); }
			set
			{
				if (this.rawExtension != value)
				{
					this.rawExtension = value;
					SetMyChanged();
				}
			}
		}

		//- Neat ------------------------------------------------------------------

		/// <summary></summary>
		[XmlElement("NeatLogTx")]
		public virtual bool NeatLogTx
		{
			get { return (this.neatLogTx); }
			set
			{
				if (this.neatLogTx != value)
				{
					this.neatLogTx = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual string NeatTxRootRelativeFilePath
		{
			get
			{
				return (ToDirectoryString(LogType.Neat, LogDirection.Tx) +
						this.rootFileName +
						ToFileNamePostFixString(LogType.Neat, LogDirection.Tx) +
						this.neatExtension);
			}
		}

		/// <summary>
		/// Builds the absolute or relative path to the log file, expanding environment variables.
		/// </summary>
		public virtual string MakeNeatTxFilePath()
		{
			return (Environment.ExpandEnvironmentVariables(this.rootPath + Path.DirectorySeparatorChar + NeatTxRootRelativeFilePath));
		}

		/// <summary></summary>
		[XmlElement("NeatLogBidir")]
		public virtual bool NeatLogBidir
		{
			get { return (this.neatLogBidir); }
			set
			{
				if (this.neatLogBidir != value)
				{
					this.neatLogBidir = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual string NeatBidirRootRelativeFilePath
		{
			get
			{
				return (ToDirectoryString(LogType.Neat, LogDirection.Bidir) +
						this.rootFileName +
						ToFileNamePostFixString(LogType.Neat, LogDirection.Bidir) +
						this.neatExtension);
			}
		}

		/// <summary>
		/// Builds the absolute or relative path to the log file, expanding environment variables.
		/// </summary>
		public virtual string MakeNeatBidirFilePath()
		{
			return (Environment.ExpandEnvironmentVariables(this.rootPath + Path.DirectorySeparatorChar + NeatBidirRootRelativeFilePath));
		}

		/// <summary></summary>
		[XmlElement("NeatLogRx")]
		public virtual bool NeatLogRx
		{
			get { return (this.neatLogRx); }
			set
			{
				if (this.neatLogRx != value)
				{
					this.neatLogRx = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual string NeatRxRootRelativeFilePath
		{
			get
			{
				return (ToDirectoryString(LogType.Neat, LogDirection.Rx) +
						this.rootFileName +
						ToFileNamePostFixString(LogType.Neat, LogDirection.Rx) +
						this.neatExtension);
			}
		}

		/// <summary>
		/// Builds the absolute or relative path to the log file, expanding environment variables.
		/// </summary>
		public virtual string MakeNeatRxFilePath()
		{
			return (Environment.ExpandEnvironmentVariables(this.rootPath + Path.DirectorySeparatorChar + NeatRxRootRelativeFilePath));
		}

		/// <summary></summary>
		[XmlElement("NeatExtension")]
		public virtual string NeatExtension
		{
			get { return (this.neatExtension); }
			set
			{
				if (this.neatExtension != value)
				{
					this.neatExtension = value;
					SetMyChanged();
				}
			}
		}

		//- Combinations ----------------------------------------------------------

		/// <summary></summary>
		[XmlIgnore]
		public virtual int PortCount
		{
			get
			{
				int count = 0;

				if (PortLog) count++;

				return (count);
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool AnyPort
		{
			get { return (PortCount >= 1); }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool MultiplePort
		{
			get { return (PortCount >= 2); }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual int RawCount
		{
			get
			{
				int count = 0;

				if (RawLogTx)    count++;
				if (RawLogBidir) count++;
				if (RawLogRx)    count++;

				return (count);
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool AnyRaw
		{
			get { return (RawCount >= 1); }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool MultipleRaw
		{
			get { return (RawCount >= 2); }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual int NeatCount
		{
			get
			{
				int count = 0;

				if (NeatLogTx)    count++;
				if (NeatLogBidir) count++;
				if (NeatLogRx)    count++;

				return (count);
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool AnyNeat
		{
			get { return (NeatCount >= 1); }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool MultipleNeat
		{
			get { return (NeatCount >= 2); }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool Any
		{
			get { return (AnyRaw || AnyNeat); }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool Multiple
		{
			get { return ((AnyPort && AnyRaw) || (AnyPort && AnyNeat) || (AnyRaw && AnyNeat) || (AnyPort && AnyRaw && AnyNeat)); }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool SameType
		{
			get { return (MultiplePort || MultipleRaw || MultipleNeat); }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool SameDirection
		{
			get
			{
				bool sameTx    = (RawLogTx    && NeatLogTx);
				bool sameBidir = (RawLogBidir && NeatLogBidir);
				bool sameRx    = (RawLogRx    && NeatLogRx);

				return (sameTx || sameBidir || sameRx);
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool SameExtension
		{
			get
			{
				bool samePR = (AnyPort && AnyRaw  && StringEx.EqualsOrdinalIgnoreCase(PortExtension, RawExtension));
				bool samePN = (AnyPort && AnyNeat && StringEx.EqualsOrdinalIgnoreCase(PortExtension, NeatExtension));
				bool sameRN = (AnyRaw  && AnyNeat && StringEx.EqualsOrdinalIgnoreCase(RawExtension,  NeatExtension));

				return (samePR || samePN || sameRN);
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual int Count
		{
			get { return (PortCount + RawCount + NeatCount); }
		}

		//- Naming ----------------------------------------------------------------

		/// <summary></summary>
		[XmlElement("NameType")]
		public virtual bool NameType
		{
			get { return (this.nameType); }
			set
			{
				if (this.nameType != value)
				{
					this.nameType = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("NameDirection")]
		public virtual bool NameDirection
		{
			get { return (this.nameDirection); }
			set
			{
				if (this.nameDirection != value)
				{
					this.nameDirection = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("NameDate")]
		public virtual bool NameDate
		{
			get { return (this.nameDate); }
			set
			{
				if (this.nameDate != value)
				{
					this.nameDate = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("NameTime")]
		public virtual bool NameTime
		{
			get { return (this.nameTime); }
			set
			{
				if (this.nameTime != value)
				{
					this.nameTime = value;
					SetMyChanged();
				}
			}
		}

		/// <remarks>
		/// This 'EnumEx' cannot be serialized, thus, the helper below is used for serialization.
		/// Still, this settings object shall provide an 'EnumEx' for full control of the setting.
		/// </remarks>
		[XmlIgnore]
		public virtual FileNameSeparatorEx NameSeparator
		{
			get { return (this.nameSeparator); }
			set
			{
				if (this.nameSeparator != value)
				{
					this.nameSeparator = value;
					SetMyChanged();
				}
			}
		}

		/// <remarks>
		/// Must be string because an 'EnumEx' cannot be serialized.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize the purpose.")]
		[XmlElement("NameSeparator")]
		public virtual string NameSeparator_ForSerialization
		{
			get { return (NameSeparator.ToSeparator()); } // Use separator string only!
			set { NameSeparator = value;                }
		}

		//- Folders --------------------------------------------------------------------------------

		/// <remarks>
		/// Using "Folder" instead of "Directory" as today's users are more used to that term.
		/// All other identifiers in code shall use "Directory" (same as .NET uses "Directory").
		/// </remarks>
		[XmlElement("FolderType")]
		public virtual bool FolderType
		{
			get { return (this.folderType); }
			set
			{
				if (this.folderType != value)
				{
					this.folderType = value;
					SetMyChanged();
				}
			}
		}

		/// <remarks>
		/// Using "Folder" instead of "Directory" as today's users are more used to that term.
		/// All other identifiers in code shall use "Directory" (same as .NET uses "Directory").
		/// </remarks>
		[XmlElement("FolderDirection")]
		public virtual bool FolderDirection
		{
			get { return (this.folderDirection); }
			set
			{
				if (this.folderDirection != value)
				{
					this.folderDirection = value;
					SetMyChanged();
				}
			}
		}

		//- WriteMode -------------------------------------------------------------

		/// <summary></summary>
		[XmlElement("WriteMode")]
		public virtual LogFileWriteMode WriteMode
		{
			get { return (this.writeMode); }
			set
			{
				if (this.writeMode != value)
				{
					this.writeMode = value;
					SetMyChanged();
				}
			}
		}

		//- Encoding -------------------------------------------------------------

		/// <summary></summary>
		[XmlIgnore]
		public bool TextEncodingIsSupported
		{
			get
			{
				if (AnyPort || AnyNeat)
				{                                                         // RTF is limited to ANSI/ASCII. YAT always uses UTF-8 for XML.
					bool portExtensionSupportsTextEncoding = (!ExtensionHelper.IsRtfFile(PortExtension) && !ExtensionHelper.IsXmlFile(PortExtension));
					bool neatExtensionSupportsTextEncoding = (!ExtensionHelper.IsRtfFile(NeatExtension) && !ExtensionHelper.IsXmlFile(NeatExtension));
					return (portExtensionSupportsTextEncoding || neatExtensionSupportsTextEncoding);
				}
				else // RawOnly => Not supported.
				{
					return (false);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("TextEncoding")]
		public virtual TextEncoding TextEncoding
		{
			get { return (this.textEncoding); }
			set
			{
				if (this.textEncoding != value)
				{
					this.textEncoding = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("EmitEncodingPreamble")]
		public virtual bool EmitEncodingPreamble
		{
			get { return (this.emitEncodingPreamble); }
			set
			{
				if (this.emitEncodingPreamble != value)
				{
					this.emitEncodingPreamble = value;
					SetMyChanged();
				}
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		private static string ToTypeString(LogType type)
		{
			switch (type)
			{
				case LogType.Port: return ("Port");
				case LogType.Raw:  return ("Raw");
				case LogType.Neat: return ("Neat");
			}
			throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "Log type '" + type + "' unknown!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, "type"));
		}

		private static string ToDirectionString(LogDirection direction)
		{
			switch (direction)
			{
				case LogDirection.Tx:    return ("Tx");
				case LogDirection.Bidir: return ("Bidir");
				case LogDirection.Rx:    return ("Rx");
			}
			throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "Log direction '" + direction + "' unknown!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, "direction"));
		}

		private string ToFileNamePostFixString(LogType type, LogDirection direction)
		{
			var now = DateTime.Now;
			var postFix = new StringBuilder();

			if (this.nameType)
			{
				postFix.Append(this.nameSeparator.ToSeparator());
				postFix.Append(ToTypeString(type));
			}

			if (this.nameDirection && (direction != LogDirection.None))
			{
				postFix.Append(this.nameSeparator.ToSeparator());
				postFix.Append(ToDirectionString(direction));
			}

			if (this.nameDate)
			{
				postFix.Append(this.nameSeparator.ToSeparator());
				postFix.Append(now.Year.ToString ("D4", CultureInfo.InvariantCulture));
				postFix.Append(now.Month.ToString("D2", CultureInfo.InvariantCulture));
				postFix.Append(now.Day.ToString  ("D2", CultureInfo.InvariantCulture));
			}

			if (this.nameTime)
			{
				postFix.Append(this.nameSeparator.ToSeparator());
				postFix.Append(now.Hour.ToString  ("D2", CultureInfo.InvariantCulture));
				postFix.Append(now.Minute.ToString("D2", CultureInfo.InvariantCulture));
				postFix.Append(now.Second.ToString("D2", CultureInfo.InvariantCulture));
			}

			return (postFix.ToString());
		}

		private string ToDirectoryString(LogType type, LogDirection direction)
		{
			var folders = new StringBuilder();

			if (this.folderType)
			{
				folders.Append(ToTypeString(type));
				folders.Append(Path.DirectorySeparatorChar);
			}

			if (this.folderDirection && (direction != LogDirection.None))
			{
				folders.Append(ToDirectionString(direction));
				folders.Append(Path.DirectorySeparatorChar);
			}

			return (folders.ToString());
		}

		/// <summary></summary>
		public Encoding ToTextEncoding(Encoding textTerminalEncoding)
		{
			switch (this.textEncoding)
			{
				case TextEncoding.Terminal:
				{
					return (textTerminalEncoding);
				}

				case TextEncoding.UTF8:
				default:
				{
					if (this.emitEncodingPreamble)
						return (Encoding.UTF8); // = UTF8Encoding(true, false);
					else
						return (new UTF8Encoding(false));
				}
			}
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to calculate hash code. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = base.GetHashCode(); // Get hash code of all settings nodes.

				hashCode = (hashCode * 397) ^ (RootPath      != null ? RootPath     .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (RootFileName  != null ? RootFileName .GetHashCode() : 0);

				hashCode = (hashCode * 397) ^  PortLog                              .GetHashCode();
				hashCode = (hashCode * 397) ^ (PortExtension != null ? PortExtension.GetHashCode() : 0);

				hashCode = (hashCode * 397) ^  RawLogTx                             .GetHashCode();
				hashCode = (hashCode * 397) ^  RawLogBidir                          .GetHashCode();
				hashCode = (hashCode * 397) ^  RawLogRx                             .GetHashCode();
				hashCode = (hashCode * 397) ^ (RawExtension != null ? RawExtension  .GetHashCode() : 0);

				hashCode = (hashCode * 397) ^  NeatLogTx                            .GetHashCode();
				hashCode = (hashCode * 397) ^  NeatLogBidir                         .GetHashCode();
				hashCode = (hashCode * 397) ^  NeatLogRx                            .GetHashCode();
				hashCode = (hashCode * 397) ^ (NeatExtension != null ? NeatExtension.GetHashCode() : 0);

				hashCode = (hashCode * 397) ^  NameType                             .GetHashCode();
				hashCode = (hashCode * 397) ^  NameDirection                        .GetHashCode();
				hashCode = (hashCode * 397) ^  NameDate                             .GetHashCode();
				hashCode = (hashCode * 397) ^  NameTime                             .GetHashCode();
				hashCode = (hashCode * 397) ^ (NameSeparator_ForSerialization != null ? NameSeparator_ForSerialization.GetHashCode() : 0);

				hashCode = (hashCode * 397) ^  FolderType                           .GetHashCode();
				hashCode = (hashCode * 397) ^  FolderDirection                      .GetHashCode();

				hashCode = (hashCode * 397) ^  WriteMode                            .GetHashCode();

				hashCode = (hashCode * 397) ^  TextEncoding                         .GetHashCode();
				hashCode = (hashCode * 397) ^  EmitEncodingPreamble                 .GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as LogSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(LogSettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				PathEx.Equals(RootPath,                         other.RootPath)        &&
				PathEx.Equals(RootFileName,                     other.RootFileName)    &&

				PortLog                                 .Equals(other.PortLog)         &&
				StringEx.EqualsOrdinalIgnoreCase(PortExtension, other.PortExtension)   &&

				RawLogTx                                .Equals(other.RawLogTx)        &&
				RawLogBidir                             .Equals(other.RawLogBidir)     &&
				RawLogRx                                .Equals(other.RawLogRx)        &&
				StringEx.EqualsOrdinalIgnoreCase(RawExtension,  other.RawExtension)    &&

				NeatLogTx                               .Equals(other.NeatLogTx)       &&
				NeatLogBidir                            .Equals(other.NeatLogBidir)    &&
				NeatLogRx                               .Equals(other.NeatLogRx)       &&
				StringEx.EqualsOrdinalIgnoreCase(NeatExtension, other.NeatExtension)   &&

				NameType                                .Equals(other.NameType)        &&
				NameDirection                           .Equals(other.NameDirection)   &&
				NameDate                                .Equals(other.NameDate)        &&
				NameTime                                .Equals(other.NameTime)        &&
				StringEx.EqualsOrdinalIgnoreCase(NameSeparator_ForSerialization, other.NameSeparator_ForSerialization) &&

				FolderType                              .Equals(other.FolderType)      &&
				FolderDirection                         .Equals(other.FolderDirection) &&

				WriteMode                               .Equals(other.WriteMode)       &&

				TextEncoding                            .Equals(other.TextEncoding)    &&
				EmitEncodingPreamble                    .Equals(other.EmitEncodingPreamble)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(LogSettings lhs, LogSettings rhs)
		{
			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			object obj = (object)lhs; // Operators are not virtual! Calling object.Equals() ensures
			return (obj.Equals(rhs)); // that a potential virtual <Derived>.Equals() is called.
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(LogSettings lhs, LogSettings rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
