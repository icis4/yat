using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

using MKY.YAT.Settings;
using MKY.YAT.Settings.Application;

namespace MKY.YAT.Log.Settings
{
	[Serializable]
	public class LogSettings : Utilities.Settings.Settings, IEquatable<LogSettings>
	{
		// root
		private string _rootPath;
		private string _rootFileName;

		// raw
		private bool _rawLogTx;
		private bool _rawLogBidir;
		private bool _rawLogRx;
		private string _rawExtension;

		// neat
		private bool _neatLogTx;
		private bool _neatLogBidir;
		private bool _neatLogRx;
		private string _neatExtension;

		// write mode
		private LogFileWriteMode _writeMode;

		// subdirectories
		private bool _subdirectoriesFormat;
		private bool _subdirectoriesChannel;

		// naming
		private bool _nameFormat;
		private bool _nameChannel;
		private bool _nameDate;
		private bool _nameTime;
		private FileNameSeparator _nameSeparator;

		public LogSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		public LogSettings(Utilities.Settings.SettingsType settingsType)
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
			_rootPath     = rhs.RootPath;
			_rootFileName = rhs.RootFileName;

			_rawLogTx     = rhs.RawLogTx;
			_rawLogBidir  = rhs.RawLogBidir;
			_rawLogRx     = rhs.RawLogRx;
			_rawExtension = rhs.RawExtension;

			_neatLogTx     = rhs.NeatLogTx;
			_neatLogBidir  = rhs.NeatLogBidir;
			_neatLogRx     = rhs.NeatLogRx;
			_neatExtension = rhs.NeatExtension;

			_writeMode = rhs.WriteMode;

			_subdirectoriesFormat  = rhs.SubdirectoriesFormat;
			_subdirectoriesChannel = rhs.SubdirectoriesChannel;

			_nameFormat    = rhs.NameFormat;
			_nameChannel   = rhs.NameChannel;
			_nameDate      = rhs.NameDate;
			_nameTime      = rhs.NameTime;
			_nameSeparator = rhs.NameSeparator;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			RootPath = ApplicationSettings.LocalUser.Paths.LogFilesPath;
			RootFileName = "YAT-Log";

			RawLogTx = false;
			RawLogBidir = false;
			RawLogRx = false;
			RawExtension = ExtensionSettings.BinaryFilesDefault;

			NeatLogTx = false;
			NeatLogBidir = true;
			NeatLogRx = false;
			NeatExtension = ExtensionSettings.LogFilesDefault;

			WriteMode = LogFileWriteMode.Create;

			SubdirectoriesFormat = false;
			SubdirectoriesChannel = false;

			NameFormat = false;
			NameChannel = false;
			NameDate = true;
			NameTime = true;
			NameSeparator = FileNameSeparator.DefaultSeparator;
		}

		private string MakeFormat(LogFormat type)
		{
			switch (type)
			{
				case LogFormat.Raw:  return ("Raw");
				case LogFormat.Neat: return ("Neat");
				default: throw (new ArgumentException("LogFormat \"" + type + "\" unknown"));
			}
		}

		private string MakeChannel(LogStreamType type)
		{
			switch (type)
			{
				case LogStreamType.Tx:    return ("Tx");
				case LogStreamType.Bidir: return ("BiDir");
				case LogStreamType.Rx:    return ("Rx");
				default: throw (new ArgumentException("LogChannel \"" + type + "\" unknown"));
			}
		}

		private string MakeSubdirectory(LogFormat format, LogStreamType channelType)
		{
			StringBuilder subdirectories = new StringBuilder();

			if (_subdirectoriesFormat)
			{
				subdirectories.Append(MakeFormat(format));
				subdirectories.Append(Path.DirectorySeparatorChar);
			}
			if (_subdirectoriesChannel)
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

			if (_nameFormat)
			{
				postFix.Append(_nameSeparator.Separator);
				postFix.Append(MakeFormat(format));
			}
			if (_nameChannel)
			{
				postFix.Append(_nameSeparator.Separator);
				postFix.Append(MakeChannel(channelType));
			}
			if (_nameDate)
			{
				postFix.Append(_nameSeparator.Separator);
				postFix.Append(now.Year.ToString("D4"));
				postFix.Append(now.Month.ToString("D2"));
				postFix.Append(now.Day.ToString("D2"));
			}
			if (_nameTime)
			{
				postFix.Append(_nameSeparator.Separator);
				postFix.Append(now.Hour.ToString("D2"));
				postFix.Append(now.Minute.ToString("D2"));
				postFix.Append(now.Second.ToString("D2"));
			}
			return (postFix.ToString());
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		[XmlElement("RootPath")]
		public string RootPath
		{
			get { return (_rootPath); }
			set
			{
				if (_rootPath != value)
				{
					_rootPath = value;
					SetChanged();
				}
			}
		}

		[XmlElement("RootFileName")]
		public string RootFileName
		{
			get { return (_rootFileName); }
			set
			{
				if (_rootFileName != value)
				{
					_rootFileName = value;
					SetChanged();
				}
			}
		}

		[XmlIgnore]
		public string RootFilePath
		{
			get { return (_rootPath + Path.DirectorySeparatorChar + _rootFileName); }
		}

		//- Raw -------------------------------------------------------------------
		[XmlElement("RawLogTx")]
		public bool RawLogTx
		{
			get { return (_rawLogTx); }
			set
			{
				if (_rawLogTx != value)
				{
					_rawLogTx = value;
					SetChanged();
				}
			}
		}

		[XmlIgnore]
		public string RawTxRootRelativeFilePath
		{
			get
			{
				return (MakeSubdirectory(LogFormat.Raw, LogStreamType.Tx) +
						_rootFileName +
						MakeFileNamePostFix(LogFormat.Raw, LogStreamType.Tx) +
						_rawExtension);
			}
		}

		[XmlIgnore]
		public string RawTxFilePath
		{
			get { return (_rootPath + Path.DirectorySeparatorChar + RawTxRootRelativeFilePath); }
		}

		[XmlElement("RawLogBidir")]
		public bool RawLogBidir
		{
			get { return (_rawLogBidir); }
			set
			{
				if (_rawLogBidir != value)
				{
					_rawLogBidir = value;
					SetChanged();
				}
			}
		}

		[XmlIgnore]
		public string RawBidirRootRelativeFilePath
		{
			get
			{
				return (MakeSubdirectory(LogFormat.Raw, LogStreamType.Bidir) +
						_rootFileName +
						MakeFileNamePostFix(LogFormat.Raw, LogStreamType.Bidir) +
						_rawExtension);
			}
		}

		[XmlIgnore]
		public string RawBidirFilePath
		{
			get { return (_rootPath + Path.DirectorySeparatorChar + RawBidirRootRelativeFilePath); }
		}

		[XmlElement("RawLogRx")]
		public bool RawLogRx
		{
			get { return (_rawLogRx); }
			set
			{
				if (_rawLogRx != value)
				{
					_rawLogRx = value;
					SetChanged();
				}
			}
		}

		[XmlIgnore]
		public string RawRxRootRelativeFilePath
		{
			get
			{
				return (MakeSubdirectory(LogFormat.Raw, LogStreamType.Rx) +
						_rootFileName +
						MakeFileNamePostFix(LogFormat.Raw, LogStreamType.Rx) +
						_rawExtension);
			}
		}

		[XmlIgnore]
		public string RawRxFilePath
		{
			get { return (_rootPath + Path.DirectorySeparatorChar + RawRxRootRelativeFilePath); }
		}

		[XmlElement("RawExtension")]
		public string RawExtension
		{
			get { return (_rawExtension); }
			set
			{
				if (_rawExtension != value)
				{
					_rawExtension = value;
					SetChanged();
				}
			}
		}

		//- Neat ------------------------------------------------------------------
		[XmlElement("NeatLogTx")]
		public bool NeatLogTx
		{
			get { return (_neatLogTx); }
			set
			{
				if (_neatLogTx != value)
				{
					_neatLogTx = value;
					SetChanged();
				}
			}
		}

		[XmlIgnore]
		public string NeatTxRootRelativeFilePath
		{
			get
			{
				return (MakeSubdirectory(LogFormat.Neat, LogStreamType.Tx) +
						_rootFileName +
						MakeFileNamePostFix(LogFormat.Neat, LogStreamType.Tx) +
						_neatExtension);
			}
		}

		[XmlIgnore]
		public string NeatTxFilePath
		{
			get { return (_rootPath + Path.DirectorySeparatorChar + NeatTxRootRelativeFilePath); }
		}

		[XmlElement("NeatLogBidir")]
		public bool NeatLogBidir
		{
			get { return (_neatLogBidir); }
			set
			{
				if (_neatLogBidir != value)
				{
					_neatLogBidir = value;
					SetChanged();
				}
			}
		}

		[XmlIgnore]
		public string NeatBidirRootRelativeFilePath
		{
			get
			{
				return (MakeSubdirectory(LogFormat.Neat, LogStreamType.Bidir) +
						_rootFileName +
						MakeFileNamePostFix(LogFormat.Neat, LogStreamType.Bidir) +
						_neatExtension);
			}
		}

		[XmlIgnore]
		public string NeatBidirFilePath
		{
			get { return (_rootPath + Path.DirectorySeparatorChar + NeatBidirRootRelativeFilePath); }
		}

		[XmlElement("NeatLogRx")]
		public bool NeatLogRx
		{
			get { return (_neatLogRx); }
			set
			{
				if (_neatLogRx != value)
				{
					_neatLogRx = value;
					SetChanged();
				}
			}
		}

		[XmlIgnore]
		public string NeatRxRootRelativeFilePath
		{
			get
			{
				return (MakeSubdirectory(LogFormat.Neat, LogStreamType.Rx) +
						_rootFileName +
						MakeFileNamePostFix(LogFormat.Neat, LogStreamType.Rx) +
						_neatExtension);
			}
		}

		[XmlIgnore]
		public string NeatRxFilePath
		{
			get { return (_rootPath + Path.DirectorySeparatorChar + NeatRxRootRelativeFilePath); }
		}

		[XmlElement("NeatExtension")]
		public string NeatExtension
		{
			get { return (_neatExtension); }
			set
			{
				if (_neatExtension != value)
				{
					_neatExtension = value;
					SetChanged();
				}
			}
		}

		//- Combinations ----------------------------------------------------------
		[XmlIgnore]
		public bool AnyRaw
		{
			get { return (RawLogTx || RawLogRx); }
		}

		[XmlIgnore]
		public bool MultipleRaw
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

		[XmlIgnore]
		public bool AnyNeat
		{
			get { return (NeatLogTx || NeatLogBidir || NeatLogRx); }
		}

		[XmlIgnore]
		public bool MultipleNeat
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

		[XmlIgnore]
		public bool AnyRawOrNeat
		{
			get { return (AnyRaw || AnyNeat); }
		}

		[XmlIgnore]
		public bool BothRawAndNeat
		{
			get
			{
				return (AnyRaw && AnyNeat);
			}
		}

		[XmlIgnore]
		public bool SameRawAndNeat
		{
			get { return ((RawLogTx && NeatLogTx) || (RawLogBidir && NeatLogBidir) || (RawLogRx && NeatLogRx)); }
		}

		//- WriteMode -------------------------------------------------------------
		[XmlElement("WriteMode")]
		public LogFileWriteMode WriteMode
		{
			get { return (_writeMode); }
			set
			{
				if (_writeMode != value)
				{
					_writeMode = value;
					SetChanged();
				}
			}
		}

		//- Subdirectories --------------------------------------------------------
		[XmlElement("SubdirectoriesFormat")]
		public bool SubdirectoriesFormat
		{
			get { return (_subdirectoriesFormat); }
			set
			{
				if (_subdirectoriesFormat != value)
				{
					_subdirectoriesFormat = value;
					SetChanged();
				}
			}
		}

		[XmlElement("SubdirectoriesChannel")]
		public bool SubdirectoriesChannel
		{
			get { return (_subdirectoriesChannel); }
			set
			{
				if (_subdirectoriesChannel != value)
				{
					_subdirectoriesChannel = value;
					SetChanged();
				}
			}
		}

		//- Naming ----------------------------------------------------------------
		[XmlElement("NameFormat")]
		public bool NameFormat
		{
			get { return (_nameFormat); }
			set
			{
				if (_nameFormat != value)
				{
					_nameFormat = value;
					SetChanged();
				}
			}
		}

		[XmlElement("NameChannel")]
		public bool NameChannel
		{
			get { return (_nameChannel); }
			set
			{
				if (_nameChannel != value)
				{
					_nameChannel = value;
					SetChanged();
				}
			}
		}

		[XmlElement("NameDate")]
		public bool NameDate
		{
			get { return (_nameDate); }
			set
			{
				if (_nameDate != value)
				{
					_nameDate = value;
					SetChanged();
				}
			}
		}

		[XmlElement("NameTime")]
		public bool NameTime
		{
			get { return (_nameTime); }
			set
			{
				if (_nameTime != value)
				{
					_nameTime = value;
					SetChanged();
				}
			}
		}

		[XmlElement("NameSeparator")]
		public FileNameSeparator NameSeparator
		{
			get { return (_nameSeparator); }
			set
			{
				if (_nameSeparator != value)
				{
					_nameSeparator = value;
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
			if (obj is LogSettings)
				return (Equals((LogSettings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(LogSettings value)
		{
			// ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return
					(
					_rootPath.Equals        (value._rootPath) &&
					_rootFileName.Equals             (value._rootFileName) &&
					_rawLogTx.Equals             (value._rawLogTx) &&
					_rawLogBidir.Equals          (value._rawLogBidir) &&
					_rawLogRx.Equals             (value._rawLogRx) &&
					_rawExtension.Equals         (value._rawExtension) &&
					_neatLogTx.Equals            (value._neatLogTx) &&
					_neatLogBidir.Equals         (value._neatLogBidir) &&
					_neatLogRx.Equals            (value._neatLogRx) &&
					_neatExtension.Equals        (value._neatExtension) &&
					_writeMode.Equals            (value._writeMode) &&
					_subdirectoriesFormat.Equals (value._subdirectoriesFormat) &&
					_subdirectoriesChannel.Equals(value._subdirectoriesChannel) &&
					_nameFormat.Equals           (value._nameFormat) &&
					_nameChannel.Equals          (value._nameChannel) &&
					_nameDate.Equals             (value._nameDate) &&
					_nameTime.Equals             (value._nameTime) &&
					_nameSeparator.Equals        (value._nameSeparator)
					);
			}
			return (false);
		}

		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(LogSettings lhs, LogSettings rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return (true);

			if ((object)lhs != null)
				return (lhs.Equals(rhs));
			
			return (false);
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
