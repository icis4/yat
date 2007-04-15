using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace HSR.YAT.Settings
{
	public class PathSettings : Utilities.Settings.Settings
	{
		[XmlElement("SettingsFilesPath")]
		private string _settingsFilesPath;

		[XmlElement("SendFilesPath")]
		private string _sendFilesPath;

		[XmlElement("LogFilesPath")]
		private string _logFilesPath;

		[XmlElement("MonitorFilesPath")]
		private string _monitorFilesPath;

		public PathSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		public PathSettings(Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		public PathSettings(PathSettings rhs)
			: base(rhs)
		{
			SettingsFilesPath = rhs.SettingsFilesPath;
			SendFilesPath     = rhs.SendFilesPath;
			LogFilesPath      = rhs.LogFilesPath;
			MonitorFilesPath  = rhs.MonitorFilesPath;
			ClearChanged();
		}

		protected override void SetMyDefaults()
		{
			SettingsFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			SendFilesPath     = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			LogFilesPath      = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			MonitorFilesPath  = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		public string SettingsFilesPath
		{
			get { return (_settingsFilesPath); }
			set
			{
				if (_settingsFilesPath != value)
				{
					_settingsFilesPath = value;
					SetChanged();
				}
			}
		}

		public string SendFilesPath
		{
			get { return (_sendFilesPath); }
			set
			{
				if (_sendFilesPath != value)
				{
					_sendFilesPath = value;
					SetChanged();
				}
			}
		}

		public string LogFilesPath
		{
			get { return (_logFilesPath); }
			set
			{
				if (_logFilesPath != value)
				{
					_logFilesPath = value;
					SetChanged();
				}
			}
		}

		public string MonitorFilesPath
		{
			get { return (_monitorFilesPath); }
			set
			{
				if (_monitorFilesPath != value)
				{
					_monitorFilesPath = value;
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
			if (obj is PathSettings)
				return (Equals((PathSettings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(PathSettings value)
		{
			// ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return
					(
					_settingsFilesPath.Equals(value._settingsFilesPath) &&
					_sendFilesPath.Equals(value._sendFilesPath) &&
					_logFilesPath.Equals(value._logFilesPath) &&
					_monitorFilesPath.Equals(value._monitorFilesPath)
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
		/// Determines whether the two specified objects have reference and value equality.
		/// </summary>
		public static bool operator ==(PathSettings lhs, PathSettings rhs)
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
		public static bool operator !=(PathSettings lhs, PathSettings rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}
