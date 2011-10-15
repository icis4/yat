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
// YAT 2.0 Beta 4 Candidate 1 Development Version 1.99.27
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.IO;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

using MKY.IO;

using YAT.Settings;
using YAT.Settings.Application;

namespace YAT.Log.Settings
{
	/// <summary></summary>
	[Serializable]
	public class LogSettings : MKY.Settings.SettingsItem
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		// root
		private string rootPath;
		private string rootFileName;

		// raw
		private bool rawLogTx;
		private bool rawLogBidir;
		private bool rawLogRx;
		private string rawExtension;

		// neat
		private bool neatLogTx;
		private bool neatLogBidir;
		private bool neatLogRx;
		private string neatExtension;

		// write mode
		private LogFileWriteMode writeMode;

		// subdirectories
		private bool subdirectoriesFormat;
		private bool subdirectoriesChannel;

		// naming
		private bool nameFormat;
		private bool nameChannel;
		private bool nameDate;
		private bool nameTime;
		private FileNameSeparator nameSeparator;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public LogSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public LogSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public LogSettings(LogSettings rhs)
			: base(rhs)
		{
			this.rootPath     = rhs.RootPath;
			this.rootFileName = rhs.RootFileName;

			this.rawLogTx     = rhs.RawLogTx;
			this.rawLogBidir  = rhs.RawLogBidir;
			this.rawLogRx     = rhs.RawLogRx;
			this.rawExtension = rhs.RawExtension;

			this.neatLogTx     = rhs.NeatLogTx;
			this.neatLogBidir  = rhs.NeatLogBidir;
			this.neatLogRx     = rhs.NeatLogRx;
			this.neatExtension = rhs.NeatExtension;

			this.writeMode = rhs.WriteMode;

			this.subdirectoriesFormat  = rhs.SubdirectoriesFormat;
			this.subdirectoriesChannel = rhs.SubdirectoriesChannel;

			this.nameFormat    = rhs.NameFormat;
			this.nameChannel   = rhs.NameChannel;
			this.nameDate      = rhs.NameDate;
			this.nameTime      = rhs.NameTime;
			this.nameSeparator = rhs.NameSeparator;

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

			RootPath     = ApplicationSettings.LocalUser.Paths.LogFilesPath;
			RootFileName = "YAT-Log";

			RawLogTx     = false;
			RawLogBidir  = false;
			RawLogRx     = false;
			RawExtension = ExtensionSettings.BinaryFilesDefault;

			NeatLogTx     = false;
			NeatLogBidir  = true;
			NeatLogRx     = false;
			NeatExtension = ExtensionSettings.LogFilesDefault;

			WriteMode = LogFileWriteMode.Create;

			SubdirectoriesFormat  = false;
			SubdirectoriesChannel = false;

			NameFormat    = false;
			NameChannel   = false;
			NameDate      = true;
			NameTime      = true;
			NameSeparator = FileNameSeparator.DefaultSeparator;
		}

		private string MakeFormat(LogFormat type)
		{
			switch (type)
			{
				case LogFormat.Raw:  return ("Raw");
				case LogFormat.Neat: return ("Neat");
				default: throw (new ArgumentException("LogFormat '" + type + "' unknown"));
			}
		}

		private string MakeChannel(LogStreamType type)
		{
			switch (type)
			{
				case LogStreamType.Tx:    return ("Tx");
				case LogStreamType.Bidir: return ("BiDir");
				case LogStreamType.Rx:    return ("Rx");
				default: throw (new ArgumentException("LogChannel '" + type + "' unknown"));
			}
		}

		private string MakeSubdirectory(LogFormat format, LogStreamType channelType)
		{
			StringBuilder subdirectories = new StringBuilder();

			if (this.subdirectoriesFormat)
			{
				subdirectories.Append(MakeFormat(format));
				subdirectories.Append(Path.DirectorySeparatorChar);
			}
			if (this.subdirectoriesChannel)
			{
				subdirectories.Append(MakeChannel(channelType));
				subdirectories.Append(Path.DirectorySeparatorChar);
			}
			return (subdirectories.ToString());
		}

		private string MakeFileNamePostFix(LogFormat format, LogStreamType channelType)
		{
			DateTime now = DateTime.Now;
			StringBuilder postFix = new StringBuilder();

			if (this.nameFormat)
			{
				postFix.Append(this.nameSeparator.Separator);
				postFix.Append(MakeFormat(format));
			}
			if (this.nameChannel)
			{
				postFix.Append(this.nameSeparator.Separator);
				postFix.Append(MakeChannel(channelType));
			}
			if (this.nameDate)
			{
				postFix.Append(this.nameSeparator.Separator);
				postFix.Append(now.Year.ToString ("D4", CultureInfo.InvariantCulture));
				postFix.Append(now.Month.ToString("D2", CultureInfo.InvariantCulture));
				postFix.Append(now.Day.ToString  ("D2", CultureInfo.InvariantCulture));
			}
			if (this.nameTime)
			{
				postFix.Append(this.nameSeparator.Separator);
				postFix.Append(now.Hour.ToString  ("D2", CultureInfo.InvariantCulture));
				postFix.Append(now.Minute.ToString("D2", CultureInfo.InvariantCulture));
				postFix.Append(now.Second.ToString("D2", CultureInfo.InvariantCulture));
			}
			return (postFix.ToString());
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
				if (value != this.rootPath)
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
				if (value != this.rootFileName)
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
				if (value != this.rawLogTx)
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
				return (MakeSubdirectory(LogFormat.Raw, LogStreamType.Tx) +
						this.rootFileName +
						MakeFileNamePostFix(LogFormat.Raw, LogStreamType.Tx) +
						this.rawExtension);
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual string RawTxFilePath
		{
			get { return (this.rootPath + Path.DirectorySeparatorChar + RawTxRootRelativeFilePath); }
		}

		/// <summary></summary>
		[XmlElement("RawLogBidir")]
		public virtual bool RawLogBidir
		{
			get { return (this.rawLogBidir); }
			set
			{
				if (value != this.rawLogBidir)
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
				return (MakeSubdirectory(LogFormat.Raw, LogStreamType.Bidir) +
						this.rootFileName +
						MakeFileNamePostFix(LogFormat.Raw, LogStreamType.Bidir) +
						this.rawExtension);
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual string RawBidirFilePath
		{
			get { return (this.rootPath + Path.DirectorySeparatorChar + RawBidirRootRelativeFilePath); }
		}

		/// <summary></summary>
		[XmlElement("RawLogRx")]
		public virtual bool RawLogRx
		{
			get { return (this.rawLogRx); }
			set
			{
				if (value != this.rawLogRx)
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
				return (MakeSubdirectory(LogFormat.Raw, LogStreamType.Rx) +
						this.rootFileName +
						MakeFileNamePostFix(LogFormat.Raw, LogStreamType.Rx) +
						this.rawExtension);
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual string RawRxFilePath
		{
			get { return (this.rootPath + Path.DirectorySeparatorChar + RawRxRootRelativeFilePath); }
		}

		/// <summary></summary>
		[XmlElement("RawExtension")]
		public virtual string RawExtension
		{
			get { return (this.rawExtension); }
			set
			{
				if (value != this.rawExtension)
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
				if (value != this.neatLogTx)
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
				return (MakeSubdirectory(LogFormat.Neat, LogStreamType.Tx) +
						this.rootFileName +
						MakeFileNamePostFix(LogFormat.Neat, LogStreamType.Tx) +
						this.neatExtension);
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual string NeatTxFilePath
		{
			get { return (this.rootPath + Path.DirectorySeparatorChar + NeatTxRootRelativeFilePath); }
		}

		/// <summary></summary>
		[XmlElement("NeatLogBidir")]
		public virtual bool NeatLogBidir
		{
			get { return (this.neatLogBidir); }
			set
			{
				if (value != this.neatLogBidir)
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
				return (MakeSubdirectory(LogFormat.Neat, LogStreamType.Bidir) +
						this.rootFileName +
						MakeFileNamePostFix(LogFormat.Neat, LogStreamType.Bidir) +
						this.neatExtension);
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual string NeatBidirFilePath
		{
			get { return (this.rootPath + Path.DirectorySeparatorChar + NeatBidirRootRelativeFilePath); }
		}

		/// <summary></summary>
		[XmlElement("NeatLogRx")]
		public virtual bool NeatLogRx
		{
			get { return (this.neatLogRx); }
			set
			{
				if (value != this.neatLogRx)
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
				return (MakeSubdirectory(LogFormat.Neat, LogStreamType.Rx) +
						this.rootFileName +
						MakeFileNamePostFix(LogFormat.Neat, LogStreamType.Rx) +
						this.neatExtension);
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual string NeatRxFilePath
		{
			get { return (this.rootPath + Path.DirectorySeparatorChar + NeatRxRootRelativeFilePath); }
		}

		/// <summary></summary>
		[XmlElement("NeatExtension")]
		public virtual string NeatExtension
		{
			get { return (this.neatExtension); }
			set
			{
				if (value != this.neatExtension)
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
		public virtual bool MultipleRaw
		{
			get
			{
				int count = 0;
				if (RawLogTx) count++;
				if (RawLogBidir) count++;
				if (RawLogRx) count++;
				return (count >= 2);
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool AnyNeat
		{
			get { return (NeatLogTx || NeatLogBidir || NeatLogRx); }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool MultipleNeat
		{
			get
			{
				int count = 0;
				if (NeatLogTx) count++;
				if (NeatLogBidir) count++;
				if (NeatLogRx) count++;
				return (count >= 2);
			}
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
			get
			{
				return (AnyRaw && AnyNeat);
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool SameRawAndNeat
		{
			get { return ((RawLogTx && NeatLogTx) || (RawLogBidir && NeatLogBidir) || (RawLogRx && NeatLogRx)); }
		}

		//- WriteMode -------------------------------------------------------------

		/// <summary></summary>
		[XmlElement("WriteMode")]
		public virtual LogFileWriteMode WriteMode
		{
			get { return (this.writeMode); }
			set
			{
				if (value != this.writeMode)
				{
					this.writeMode = value;
					SetChanged();
				}
			}
		}

		//- Subdirectories --------------------------------------------------------

		/// <summary></summary>
		[XmlElement("SubdirectoriesFormat")]
		public virtual bool SubdirectoriesFormat
		{
			get { return (this.subdirectoriesFormat); }
			set
			{
				if (value != this.subdirectoriesFormat)
				{
					this.subdirectoriesFormat = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SubdirectoriesChannel")]
		public virtual bool SubdirectoriesChannel
		{
			get { return (this.subdirectoriesChannel); }
			set
			{
				if (value != this.subdirectoriesChannel)
				{
					this.subdirectoriesChannel = value;
					SetChanged();
				}
			}
		}

		//- Naming ----------------------------------------------------------------

		/// <summary></summary>
		[XmlElement("NameFormat")]
		public virtual bool NameFormat
		{
			get { return (this.nameFormat); }
			set
			{
				if (value != this.nameFormat)
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
				if (value != this.nameChannel)
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
				if (value != this.nameDate)
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
				if (value != this.nameTime)
				{
					this.nameTime = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("NameSeparator")]
		public virtual FileNameSeparator NameSeparator
		{
			get { return (this.nameSeparator); }
			set
			{
				if (value != this.nameSeparator)
				{
					this.nameSeparator = value;
					SetChanged();
				}
			}
		}

		#endregion

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
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

				PathEx.Equals(this.rootPath,     other.rootPath) &&
				PathEx.Equals(this.rootFileName, other.rootFileName) &&
				(this.rawLogTx              == other.rawLogTx) &&
				(this.rawLogBidir           == other.rawLogBidir) &&
				(this.rawLogRx              == other.rawLogRx) &&
				(this.rawExtension          == other.rawExtension) &&
				(this.neatLogTx             == other.neatLogTx) &&
				(this.neatLogBidir          == other.neatLogBidir) &&
				(this.neatLogRx             == other.neatLogRx) &&
				(this.neatExtension         == other.neatExtension) &&
				(this.writeMode             == other.writeMode) &&
				(this.subdirectoriesFormat  == other.subdirectoriesFormat) &&
				(this.subdirectoriesChannel == other.subdirectoriesChannel) &&
				(this.nameFormat            == other.nameFormat) &&
				(this.nameChannel           == other.nameChannel) &&
				(this.nameDate              == other.nameDate) &&
				(this.nameTime              == other.nameTime) &&
				(this.nameSeparator         == other.nameSeparator)
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return
			(
				base.GetHashCode() ^

				this.rootPath             .GetHashCode() ^
				this.rootFileName         .GetHashCode() ^
				this.rawLogTx             .GetHashCode() ^
				this.rawLogBidir          .GetHashCode() ^
				this.rawLogRx             .GetHashCode() ^
				this.rawExtension         .GetHashCode() ^
				this.neatLogTx            .GetHashCode() ^
				this.neatLogBidir         .GetHashCode() ^
				this.neatLogRx            .GetHashCode() ^
				this.neatExtension        .GetHashCode() ^
				this.writeMode            .GetHashCode() ^
				this.subdirectoriesFormat .GetHashCode() ^
				this.subdirectoriesChannel.GetHashCode() ^
				this.nameFormat           .GetHashCode() ^
				this.nameChannel          .GetHashCode() ^
				this.nameDate             .GetHashCode() ^
				this.nameTime             .GetHashCode() ^
				this.nameSeparator        .GetHashCode()
			);
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
