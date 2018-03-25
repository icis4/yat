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
// YAT Version 2.0.0
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2018 Matthias Kläy.
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

		// Naming:
		private bool nameFormat;
		private bool nameChannel;
		private bool nameDate;
		private bool nameTime;
		private FileNameSeparatorEx nameSeparator;

		// Folders:
		private bool folderFormat;
		private bool folderChannel;

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
		/// Set fields through properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public LogSettings(LogSettings rhs)
			: base(rhs)
		{
			RootPath     = rhs.RootPath;
			RootFileName = rhs.RootFileName;

			RawLogTx     = rhs.RawLogTx;
			RawLogBidir  = rhs.RawLogBidir;
			RawLogRx     = rhs.RawLogRx;
			RawExtension = rhs.RawExtension;

			NeatLogTx     = rhs.NeatLogTx;
			NeatLogBidir  = rhs.NeatLogBidir;
			NeatLogRx     = rhs.NeatLogRx;
			NeatExtension = rhs.NeatExtension;

			NameFormat    = rhs.NameFormat;
			NameChannel   = rhs.NameChannel;
			NameDate      = rhs.NameDate;
			NameTime      = rhs.NameTime;
			NameSeparator = rhs.NameSeparator;

			FolderFormat  = rhs.FolderFormat;
			FolderChannel = rhs.FolderChannel;

			WriteMode     = rhs.WriteMode;

			TextEncoding  = rhs.TextEncoding;
			EmitEncodingPreamble = rhs.EmitEncodingPreamble;

			ClearChanged();
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			ApplicationSettings.LocalUserSettings.Paths.LogFiles = Application.Settings.PathSettings.LogFilesDefault;

			RootPath     = ApplicationSettings.LocalUserSettings.Paths.LogFiles;
			RootFileName = ApplicationEx.ProductName + "-Log";

			RawLogTx     = false;
			RawLogBidir  = false;
			RawLogRx     = false;
			RawExtension = ApplicationSettings.RoamingUserSettings.Extensions.RawLogFiles;

			NeatLogTx     = false;
			NeatLogBidir  = true;
			NeatLogRx     = false;
			NeatExtension = ApplicationSettings.RoamingUserSettings.Extensions.NeatLogFiles;

			NameFormat    = false;
			NameChannel   = false;
			NameDate      = true;
			NameTime      = true;
			NameSeparator = FileNameSeparator.Dash;

			FolderFormat  = false;
			FolderChannel = false;

			WriteMode     = LogFileWriteMode.Create;

			TextEncoding  = TextEncoding.UTF8;
			EmitEncodingPreamble = true;
		}

		private static string ToFormatString(LogFormat format)
		{
			switch (format)
			{
				case LogFormat.Raw:  return ("Raw");
				case LogFormat.Neat: return ("Neat");
			}
			throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "Log format '" + format + "' unknown!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, "format"));
		}

		private static string ToChannelString(LogChannelType channelType)
		{
			switch (channelType)
			{
				case LogChannelType.Tx:    return ("Tx");
				case LogChannelType.Bidir: return ("Bidir");
				case LogChannelType.Rx:    return ("Rx");
			}
			throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "Log channel type '" + channelType + "' unknown!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, "channelType"));
		}

		private string ToFileNamePostFixString(LogFormat format, LogChannelType channelType)
		{
			var now = DateTime.Now;
			var postFix = new StringBuilder();

			if (this.nameFormat)
			{
				postFix.Append(this.nameSeparator.ToSeparator());
				postFix.Append(ToFormatString(format));
			}
			if (this.nameChannel)
			{
				postFix.Append(this.nameSeparator.ToSeparator());
				postFix.Append(ToChannelString(channelType));
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

		private string ToDirectoryString(LogFormat format, LogChannelType channelType)
		{
			var folders = new StringBuilder();

			if (this.folderFormat)
			{
				folders.Append(ToFormatString(format));
				folders.Append(Path.DirectorySeparatorChar);
			}
			if (this.folderChannel)
			{
				folders.Append(ToChannelString(channelType));
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
				return (ToDirectoryString(LogFormat.Raw, LogChannelType.Tx) +
						this.rootFileName +
						ToFileNamePostFixString(LogFormat.Raw, LogChannelType.Tx) +
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
				return (ToDirectoryString(LogFormat.Raw, LogChannelType.Bidir) +
						this.rootFileName +
						ToFileNamePostFixString(LogFormat.Raw, LogChannelType.Bidir) +
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
				return (ToDirectoryString(LogFormat.Raw, LogChannelType.Rx) +
						this.rootFileName +
						ToFileNamePostFixString(LogFormat.Raw, LogChannelType.Rx) +
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
				return (ToDirectoryString(LogFormat.Neat, LogChannelType.Tx) +
						this.rootFileName +
						ToFileNamePostFixString(LogFormat.Neat, LogChannelType.Tx) +
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
				return (ToDirectoryString(LogFormat.Neat, LogChannelType.Bidir) +
						this.rootFileName +
						ToFileNamePostFixString(LogFormat.Neat, LogChannelType.Bidir) +
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
				return (ToDirectoryString(LogFormat.Neat, LogChannelType.Rx) +
						this.rootFileName +
						ToFileNamePostFixString(LogFormat.Neat, LogChannelType.Rx) +
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
		public virtual bool AnyRaw
		{
			get { return (RawLogTx || RawLogBidir || RawLogRx); }
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
		public virtual bool MultipleRaw
		{
			get { return (RawCount >= 2); }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool AnyNeat
		{
			get { return (NeatLogTx || NeatLogBidir || NeatLogRx); }
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
		public virtual bool MultipleNeat
		{
			get { return (NeatCount >= 2); }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool AnyRawOrNeat
		{
			get { return (AnyRaw || AnyNeat); }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool BothRawAndNeat
		{
			get { return (AnyRaw && AnyNeat); }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool SameRawAndNeat
		{
			get { return ((RawLogTx && NeatLogTx) || (RawLogBidir && NeatLogBidir) || (RawLogRx && NeatLogRx)); }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual int Count
		{
			get { return (RawCount + NeatCount); }
		}

		//- Naming ----------------------------------------------------------------

		/// <summary></summary>
		[XmlElement("NameFormat")]
		public virtual bool NameFormat
		{
			get { return (this.nameFormat); }
			set
			{
				if (this.nameFormat != value)
				{
					this.nameFormat = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("NameChannel")]
		public virtual bool NameChannel
		{
			get { return (this.nameChannel); }
			set
			{
				if (this.nameChannel != value)
				{
					this.nameChannel = value;
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
		[XmlElement("FolderFormat")]
		public virtual bool FolderFormat
		{
			get { return (this.folderFormat); }
			set
			{
				if (this.folderFormat != value)
				{
					this.folderFormat = value;
					SetMyChanged();
				}
			}
		}

		/// <remarks>
		/// Using "Folder" instead of "Directory" as today's users are more used to that term.
		/// All other identifiers in code shall use "Directory" (same as .NET uses "Directory").
		/// </remarks>
		[XmlElement("FolderChannel")]
		public virtual bool FolderChannel
		{
			get { return (this.folderChannel); }
			set
			{
				if (this.folderChannel != value)
				{
					this.folderChannel = value;
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
				if (AnyNeat)
				{
					if      (ExtensionHelper.IsRtfFile(NeatExtension))
						return (false); // RTF is limited to ANSI/ASCII.
					else if (ExtensionHelper.IsXmlFile(NeatExtension))
						return (false); // YAT always uses UTF-8 for XML.
					else
						return (true);
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

				hashCode = (hashCode * 397) ^  RawLogTx                             .GetHashCode();
				hashCode = (hashCode * 397) ^  RawLogBidir                          .GetHashCode();
				hashCode = (hashCode * 397) ^  RawLogRx                             .GetHashCode();
				hashCode = (hashCode * 397) ^  RawExtension                         .GetHashCode();

				hashCode = (hashCode * 397) ^  NeatLogTx                            .GetHashCode();
				hashCode = (hashCode * 397) ^  NeatLogBidir                         .GetHashCode();
				hashCode = (hashCode * 397) ^  NeatLogRx                            .GetHashCode();
				hashCode = (hashCode * 397) ^  NeatExtension                        .GetHashCode();

				hashCode = (hashCode * 397) ^  NameFormat                           .GetHashCode();
				hashCode = (hashCode * 397) ^  NameChannel                          .GetHashCode();
				hashCode = (hashCode * 397) ^  NameDate                             .GetHashCode();
				hashCode = (hashCode * 397) ^  NameTime                             .GetHashCode();
				hashCode = (hashCode * 397) ^ (NameSeparator_ForSerialization != null ? NameSeparator_ForSerialization.GetHashCode() : 0);

				hashCode = (hashCode * 397) ^  FolderFormat                         .GetHashCode();
				hashCode = (hashCode * 397) ^  FolderChannel                        .GetHashCode();

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

				PathEx.Equals(RootPath,     other.RootPath)      &&
				PathEx.Equals(RootFileName, other.RootFileName)  &&
				RawLogTx            .Equals(other.RawLogTx)      &&
				RawLogBidir         .Equals(other.RawLogBidir)   &&
				RawLogRx            .Equals(other.RawLogRx)      &&
				RawExtension        .Equals(other.RawExtension)  &&

				NeatLogTx           .Equals(other.NeatLogTx)     &&
				NeatLogBidir        .Equals(other.NeatLogBidir)  &&
				NeatLogRx           .Equals(other.NeatLogRx)     &&
				NeatExtension       .Equals(other.NeatExtension) &&

				NameFormat          .Equals(other.NameFormat)    &&
				NameChannel         .Equals(other.NameChannel)   &&
				NameDate            .Equals(other.NameDate)      &&
				NameTime            .Equals(other.NameTime)      &&
				StringEx.EqualsOrdinalIgnoreCase(NameSeparator_ForSerialization, other.NameSeparator_ForSerialization) &&

				FolderFormat        .Equals(other.FolderFormat)  &&
				FolderChannel       .Equals(other.FolderChannel) &&

				WriteMode           .Equals(other.WriteMode)     &&

				TextEncoding        .Equals(other.TextEncoding)  &&
				EmitEncodingPreamble.Equals(other.EmitEncodingPreamble)
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
