//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2009 Matthias Kläy.
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
using System.Windows.Forms;

namespace YAT.Settings
{
	[Serializable]
	public class GeneralSettings : MKY.Utilities.Settings.Settings, IEquatable<GeneralSettings>
	{
		/// <summary></summary>
		public static readonly string AutoSaveRoot = Application.LocalUserAppDataPath;
		/// <summary></summary>
		public const string AutoSaveTerminalFileNamePrefix = "Terminal-";
		/// <summary></summary>
		public const string AutoSaveWorkspaceFileNamePrefix = "Workspace-";

		private bool _autoOpenWorkspace;
		private bool _autoSaveWorkspace;
		private bool _useRelativePaths;
		private string _workspaceFilePath;
		private bool _detectSerialPortsInUse;

		public GeneralSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		public GeneralSettings(MKY.Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public GeneralSettings(GeneralSettings rhs)
			: base(rhs)
		{
			_autoOpenWorkspace      = rhs.AutoOpenWorkspace;
			_autoSaveWorkspace      = rhs.AutoSaveWorkspace;
			_useRelativePaths       = rhs.UseRelativePaths;
			_workspaceFilePath      = rhs.WorkspaceFilePath;
			_detectSerialPortsInUse = rhs.DetectSerialPortsInUse;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			AutoOpenWorkspace      = true;
			AutoSaveWorkspace      = true;
			UseRelativePaths       = true;
			WorkspaceFilePath      = "";
			DetectSerialPortsInUse = true;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

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

		[XmlElement("UseRelativePaths")]
		public bool UseRelativePaths
		{
			get { return (_useRelativePaths); }
			set
			{
				if (_useRelativePaths != value)
				{
					_useRelativePaths = value;
					SetChanged();
				}
			}
		}

		[XmlElement("WorkspaceFilePath")]
		public string WorkspaceFilePath
		{
			get { return (_workspaceFilePath); }
			set
			{
				if (_workspaceFilePath != value)
				{
					_workspaceFilePath = value;
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
					_autoOpenWorkspace.     Equals(value._autoOpenWorkspace) &&
					_autoSaveWorkspace.     Equals(value._autoSaveWorkspace) &&
					_useRelativePaths.      Equals(value._useRelativePaths) &&
					_workspaceFilePath.     Equals(value._workspaceFilePath) &&
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
		/// Determines whether the two specified objects have reference or value equality.
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

//==================================================================================================
// End of
// $URL$
//==================================================================================================
