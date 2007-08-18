using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;

namespace MKY.YAT.Settings
{
	[Serializable]
	public class GeneralSettings : Utilities.Settings.Settings, IEquatable<GeneralSettings>
	{
		private bool _autoOpenWorkspace;
		private bool _autoSaveWorkspace;
		private string _currentWorkspaceFile;
		private bool _detectSerialPortsInUse;

		public GeneralSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		public GeneralSettings(Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		public GeneralSettings(GeneralSettings rhs)
			: base(rhs)
		{
			AutoOpenWorkspace      = rhs.AutoOpenWorkspace;
			AutoSaveWorkspace      = rhs.AutoSaveWorkspace;
			CurrentWorkspaceFile   = rhs.CurrentWorkspaceFile;
			DetectSerialPortsInUse = rhs.DetectSerialPortsInUse;

			ClearChanged();
		}

		protected override void SetMyDefaults()
		{
			AutoOpenWorkspace      = true;
			AutoSaveWorkspace      = true;
			CurrentWorkspaceFile   = Application.LocalUserAppDataPath + Path.DirectorySeparatorChar + "DefaultWorkspace" + ExtensionSettings.WorkspaceFiles;
			DetectSerialPortsInUse = true;
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		[XmlElement("AutoOpenWorkspace")]
		public bool AutoOpenWorkspace
		{
			get { return (_autoOpenWorkspace); }
			set
			{
				if (_autoOpenWorkspace != value)
				{
					_autoOpenWorkspace = value;
					SetChanged();
				}
			}
		}

		[XmlElement("AutoSaveWorkspace")]
		public bool AutoSaveWorkspace
		{
			get { return (_autoSaveWorkspace); }
			set
			{
				if (_autoSaveWorkspace != value)
				{
					_autoSaveWorkspace = value;
					SetChanged();
				}
			}
		}

		[XmlElement("CurrentWorkspaceFile")]
		public string CurrentWorkspaceFile
		{
			get { return (_currentWorkspaceFile); }
			set
			{
				if (_currentWorkspaceFile != value)
				{
					_currentWorkspaceFile = value;
					SetChanged();
				}
			}
		}

		[XmlElement("DetectSerialPortsInUse")]
		public bool DetectSerialPortsInUse
		{
			get { return (_detectSerialPortsInUse); }
			set
			{
				if (_detectSerialPortsInUse != value)
				{
					_detectSerialPortsInUse = value;
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
			if (obj is GeneralSettings)
				return (Equals((GeneralSettings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(GeneralSettings value)
		{
			// ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return
					(
					_autoOpenWorkspace.Equals(value._autoOpenWorkspace) &&
					_autoSaveWorkspace.Equals(value._autoSaveWorkspace) &&
					_currentWorkspaceFile.Equals(value._currentWorkspaceFile) &&
					_detectSerialPortsInUse.Equals(value._detectSerialPortsInUse)
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
		public static bool operator ==(GeneralSettings lhs, GeneralSettings rhs)
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
		public static bool operator !=(GeneralSettings lhs, GeneralSettings rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}
