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
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Serialization;

using MKY;
using MKY.IO;

using YAT.Application.Utilities;
using YAT.Settings.Application;

#endregion

namespace YAT.Log.Settings
{
	/// <summary></summary>
	[Serializable]
	public class LogSettings : MKY.Settings.SettingsItem
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <summary></summary>
		public readonly Encoding DefaultTextEncoding = System.Text.Encoding.UTF8;

		#endregion

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
		private LogFileEncoding textEncoding;

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

			RootPath     = ApplicationSettings.LocalUserSettings.Paths.LogFiles;
			RootFileName = ApplicationEx.ProductName + "-Log";

			RawLogTx     = false;
			RawLogBidir  = false;
			RawLogRx     = false;
			RawExtension = ApplicationSettings.LocalUserSettings.Extensions.RawLogFiles;

			NeatLogTx     = false;
			NeatLogBidir  = true;
			NeatLogRx     = false;
			NeatExtension = ApplicationSettings.LocalUserSettings.Extensions.NeatLogFiles;

			NameFormat    = false;
			NameChannel   = false;
			NameDate      = true;
			NameTime      = true;
			NameSeparator = FileNameSeparator.Dash;

			FolderFormat  = false;
			FolderChannel = false;

			WriteMode     = LogFileWriteMode.Create;

			TextEncoding  = LogFileEncoding.UTF8;
		}

		private static string ToFormatString(LogFormat format)
		{
			switch (format)
			{
				case LogFormat.Raw:  return ("Raw");
				case LogFormat.Neat: return ("Neat");
			}
			throw (new ArgumentException("LogFormat '" + format + "' unknown!"));
		}

		private static string ToChannelString(LogChannelType channelType)
		{
			switch (channelType)
			{
				case LogChannelType.Tx:    return ("Tx");
				case LogChannelType.Bidir: return ("Bidir");
				case LogChannelType.Rx:    return ("Rx");
			}
			throw (new ArgumentException("LogChannelType '" + channelType + "' unknown!"));
		}

		private string ToFileNamePostFixString(LogFormat format, LogChannelType channelType)
		{
			DateTime now = DateTime.Now;
			StringBuilder postFix = new StringBuilder();

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
			StringBuilder folders = new StringBuilder();

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
				case LogFileEncoding.Terminal:
					return (textTerminalEncoding);

				case LogFileEncoding.UTF8:
				default:
					return (DefaultTextEncoding);
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
					SetChanged();
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
					SetChanged();
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
					SetChanged();
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

		/// <summary></summary>
		public virtual string MakeRawTxFilePath()
		{
			return (this.rootPath + Path.DirectorySeparatorChar + RawTxRootRelativeFilePath);
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
					SetChanged();
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

		/// <summary></summary>
		public virtual string MakeRawBidirFilePath()
		{
			return (this.rootPath + Path.DirectorySeparatorChar + RawBidirRootRelativeFilePath);
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
					SetChanged();
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

		/// <summary></summary>
		public virtual string MakeRawRxFilePath()
		{
			return (this.rootPath + Path.DirectorySeparatorChar + RawRxRootRelativeFilePath);
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
					SetChanged();
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
					SetChanged();
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

		/// <summary></summary>
		public virtual string MakeNeatTxFilePath()
		{
			return (this.rootPath + Path.DirectorySeparatorChar + NeatTxRootRelativeFilePath);
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
					SetChanged();
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

		/// <summary></summary>
		public virtual string MakeNeatBidirFilePath()
		{
			return (this.rootPath + Path.DirectorySeparatorChar + NeatBidirRootRelativeFilePath);
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
					SetChanged();
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

		/// <summary></summary>
		public virtual string MakeNeatRxFilePath()
		{
			return (this.rootPath + Path.DirectorySeparatorChar + NeatRxRootRelativeFilePath);
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
					SetChanged();
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
					SetChanged();
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
					SetChanged();
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
					SetChanged();
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
					SetChanged();
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
					SetChanged();
				}
			}
		}

		/// <remarks>
		/// Must be string because an 'EnumEx' cannot be serialized.
		/// </remarks>
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
					SetChanged();
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
					SetChanged();
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
					SetChanged();
				}
			}
		}

		//- Encoding -------------------------------------------------------------

		/// <summary></summary>
		[XmlElement("TextEncoding")]
		public virtual LogFileEncoding TextEncoding
		{
			get { return (this.textEncoding); }
			set
			{
				if (this.textEncoding != value)
				{
					this.textEncoding = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		public bool TextEncodingIsSupported()
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

		#endregion

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(obj, null))
				return (false);

			if (GetType() != obj.GetType())
				return (false);

			LogSettings other = (LogSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				PathEx.Equals(RootPath,     other.RootPath) &&
				PathEx.Equals(RootFileName, other.RootFileName) &&
				(RawLogTx                == other.RawLogTx) &&
				(RawLogBidir             == other.RawLogBidir) &&
				(RawLogRx                == other.RawLogRx) &&
				(RawExtension            == other.RawExtension) &&
				(NeatLogTx               == other.NeatLogTx) &&
				(NeatLogBidir            == other.NeatLogBidir) &&
				(NeatLogRx               == other.NeatLogRx) &&
				(NeatExtension           == other.NeatExtension) &&
				(NameFormat              == other.NameFormat) &&
				(NameChannel             == other.NameChannel) &&
				(NameDate                == other.NameDate) &&
				(NameTime                == other.NameTime) &&
				StringEx.EqualsOrdinalIgnoreCase(NameSeparator_ForSerialization, other.NameSeparator_ForSerialization) &&
				(FolderFormat            == other.FolderFormat) &&
				(FolderChannel           == other.FolderChannel) &&
				(WriteMode               == other.WriteMode) &&
				(TextEncoding            == other.TextEncoding)
			);
		}

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

				return (hashCode);
			}
		}

		#endregion

		#region Comparison Operators

		// Use of base reference type implementation of operators ==/!=.
		// See MKY.Test.EqualityTest for details.

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
