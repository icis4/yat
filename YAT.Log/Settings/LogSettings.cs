//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

using YAT.Settings;
using YAT.Settings.Application;

namespace YAT.Log.Settings
{
	/// <summary></summary>
	[Serializable]
	public class LogSettings : MKY.Utilities.Settings.Settings, IEquatable<LogSettings>
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

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
		public LogSettings(MKY.Utilities.Settings.SettingsType settingsType)
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
				default: throw (new ArgumentException(@"LogFormat """ + type + @""" unknown"));
			}
		}

		private string MakeChannel(LogStreamType type)
		{
			switch (type)
			{
				case LogStreamType.Tx:    return ("Tx");
				case LogStreamType.Bidir: return ("BiDir");
				case LogStreamType.Rx:    return ("Rx");
				default: throw (new ArgumentException(@"LogChannel """ + type + @""" unknown"));
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

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("RootPath")]
		public virtual string RootPath
		{
			get { return (_rootPath); }
			set
			{
				if (value != _rootPath)
				{
					_rootPath = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("RootFileName")]
		public virtual string RootFileName
		{
			get { return (_rootFileName); }
			set
			{
				if (value != _rootFileName)
				{
					_rootFileName = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual string RootFilePath
		{
			get { return (_rootPath + Path.DirectorySeparatorChar + _rootFileName); }
		}

		//- Raw -------------------------------------------------------------------

		/// <summary></summary>
		[XmlElement("RawLogTx")]
		public virtual bool RawLogTx
		{
			get { return (_rawLogTx); }
			set
			{
				if (value != _rawLogTx)
				{
					_rawLogTx = value;
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
						_rootFileName +
						MakeFileNamePostFix(LogFormat.Raw, LogStreamType.Tx) +
						_rawExtension);
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual string RawTxFilePath
		{
			get { return (_rootPath + Path.DirectorySeparatorChar + RawTxRootRelativeFilePath); }
		}

		/// <summary></summary>
		[XmlElement("RawLogBidir")]
		public virtual bool RawLogBidir
		{
			get { return (_rawLogBidir); }
			set
			{
				if (value != _rawLogBidir)
				{
					_rawLogBidir = value;
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
						_rootFileName +
						MakeFileNamePostFix(LogFormat.Raw, LogStreamType.Bidir) +
						_rawExtension);
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual string RawBidirFilePath
		{
			get { return (_rootPath + Path.DirectorySeparatorChar + RawBidirRootRelativeFilePath); }
		}

		/// <summary></summary>
		[XmlElement("RawLogRx")]
		public virtual bool RawLogRx
		{
			get { return (_rawLogRx); }
			set
			{
				if (value != _rawLogRx)
				{
					_rawLogRx = value;
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
						_rootFileName +
						MakeFileNamePostFix(LogFormat.Raw, LogStreamType.Rx) +
						_rawExtension);
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual string RawRxFilePath
		{
			get { return (_rootPath + Path.DirectorySeparatorChar + RawRxRootRelativeFilePath); }
		}

		/// <summary></summary>
		[XmlElement("RawExtension")]
		public virtual string RawExtension
		{
			get { return (_rawExtension); }
			set
			{
				if (value != _rawExtension)
				{
					_rawExtension = value;
					SetChanged();
				}
			}
		}

		//- Neat ------------------------------------------------------------------

		/// <summary></summary>
		[XmlElement("NeatLogTx")]
		public virtual bool NeatLogTx
		{
			get { return (_neatLogTx); }
			set
			{
				if (value != _neatLogTx)
				{
					_neatLogTx = value;
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
						_rootFileName +
						MakeFileNamePostFix(LogFormat.Neat, LogStreamType.Tx) +
						_neatExtension);
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual string NeatTxFilePath
		{
			get { return (_rootPath + Path.DirectorySeparatorChar + NeatTxRootRelativeFilePath); }
		}

		/// <summary></summary>
		[XmlElement("NeatLogBidir")]
		public virtual bool NeatLogBidir
		{
			get { return (_neatLogBidir); }
			set
			{
				if (value != _neatLogBidir)
				{
					_neatLogBidir = value;
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
						_rootFileName +
						MakeFileNamePostFix(LogFormat.Neat, LogStreamType.Bidir) +
						_neatExtension);
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual string NeatBidirFilePath
		{
			get { return (_rootPath + Path.DirectorySeparatorChar + NeatBidirRootRelativeFilePath); }
		}

		/// <summary></summary>
		[XmlElement("NeatLogRx")]
		public virtual bool NeatLogRx
		{
			get { return (_neatLogRx); }
			set
			{
				if (value != _neatLogRx)
				{
					_neatLogRx = value;
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
						_rootFileName +
						MakeFileNamePostFix(LogFormat.Neat, LogStreamType.Rx) +
						_neatExtension);
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual string NeatRxFilePath
		{
			get { return (_rootPath + Path.DirectorySeparatorChar + NeatRxRootRelativeFilePath); }
		}

		/// <summary></summary>
		[XmlElement("NeatExtension")]
		public virtual string NeatExtension
		{
			get { return (_neatExtension); }
			set
			{
				if (value != _neatExtension)
				{
					_neatExtension = value;
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
			get { return (_writeMode); }
			set
			{
				if (value != _writeMode)
				{
					_writeMode = value;
					SetChanged();
				}
			}
		}

		//- Subdirectories --------------------------------------------------------

		/// <summary></summary>
		[XmlElement("SubdirectoriesFormat")]
		public virtual bool SubdirectoriesFormat
		{
			get { return (_subdirectoriesFormat); }
			set
			{
				if (value != _subdirectoriesFormat)
				{
					_subdirectoriesFormat = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SubdirectoriesChannel")]
		public virtual bool SubdirectoriesChannel
		{
			get { return (_subdirectoriesChannel); }
			set
			{
				if (value != _subdirectoriesChannel)
				{
					_subdirectoriesChannel = value;
					SetChanged();
				}
			}
		}

		//- Naming ----------------------------------------------------------------

		/// <summary></summary>
		[XmlElement("NameFormat")]
		public virtual bool NameFormat
		{
			get { return (_nameFormat); }
			set
			{
				if (value != _nameFormat)
				{
					_nameFormat = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("NameChannel")]
		public virtual bool NameChannel
		{
			get { return (_nameChannel); }
			set
			{
				if (value != _nameChannel)
				{
					_nameChannel = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("NameDate")]
		public virtual bool NameDate
		{
			get { return (_nameDate); }
			set
			{
				if (value != _nameDate)
				{
					_nameDate = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("NameTime")]
		public virtual bool NameTime
		{
			get { return (_nameTime); }
			set
			{
				if (value != _nameTime)
				{
					_nameTime = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("NameSeparator")]
		public virtual FileNameSeparator NameSeparator
		{
			get { return (_nameSeparator); }
			set
			{
				if (value != _nameSeparator)
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
			// Ensure that object.operator!=() is called.
			if ((object)value != null)
			{
				return
					(
					(_rootPath              == value._rootPath) &&
					(_rootFileName          == value._rootFileName) &&
					(_rawLogTx              == value._rawLogTx) &&
					(_rawLogBidir           == value._rawLogBidir) &&
					(_rawLogRx              == value._rawLogRx) &&
					(_rawExtension          == value._rawExtension) &&
					(_neatLogTx             == value._neatLogTx) &&
					(_neatLogBidir          == value._neatLogBidir) &&
					(_neatLogRx             == value._neatLogRx) &&
					(_neatExtension         == value._neatExtension) &&
					(_writeMode             == value._writeMode) &&
					(_subdirectoriesFormat  == value._subdirectoriesFormat) &&
					(_subdirectoriesChannel == value._subdirectoriesChannel) &&
					(_nameFormat            == value._nameFormat) &&
					(_nameChannel           == value._nameChannel) &&
					(_nameDate              == value._nameDate) &&
					(_nameTime              == value._nameTime) &&
					(_nameSeparator         == value._nameSeparator)
					);
			}
			return (false);
		}

		/// <summary></summary>
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

//==================================================================================================
// End of
// $URL$
//==================================================================================================
