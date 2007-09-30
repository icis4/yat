using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MKY.YAT.Settings
{
	[Serializable]
	public class PathSettings : Utilities.Settings.Settings, IEquatable<PathSettings>
	{
		private string _terminalFilesPath;
		private string _workspaceFilesPath;
		private string _sendFilesPath;
		private string _logFilesPath;
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

		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public PathSettings(PathSettings rhs)
			: base(rhs)
		{
			_terminalFilesPath  = rhs.TerminalFilesPath;
			_workspaceFilesPath = rhs.WorkspaceFilesPath;
			_sendFilesPath      = rhs.SendFilesPath;
			_logFilesPath       = rhs.LogFilesPath;
			_monitorFilesPath   = rhs.MonitorFilesPath;
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			TerminalFilesPath  = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			WorkspaceFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			SendFilesPath      = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			LogFilesPath       = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			MonitorFilesPath   = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		[XmlElement("TerminalFilesPath")]
		public string TerminalFilesPath
		{
			get { return (_terminalFilesPath); }
			set
			{
				if (_terminalFilesPath != value)
				{
					_terminalFilesPath = value;
					SetChanged();
				}
			}
		}

		[XmlElement("WorkspaceFilesPath")]
		public string WorkspaceFilesPath
		{
			get { return (_workspaceFilesPath); }
			set
			{
				if (_workspaceFilesPath != value)
				{
					_workspaceFilesPath = value;
					SetChanged();
				}
			}
		}

		[XmlElement("SendFilesPath")]
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

		[XmlElement("LogFilesPath")]
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

		[XmlElement("MonitorFilesPath")]
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
					_terminalFilesPath.Equals(value._terminalFilesPath) &&
					_workspaceFilesPath.Equals(value._workspaceFilesPath) &&
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
		/// Determines whether the two specified objects have reference or value equality.
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
