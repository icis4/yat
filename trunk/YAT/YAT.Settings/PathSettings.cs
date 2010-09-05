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

namespace YAT.Settings
{
	[Serializable]
	public class PathSettings : MKY.Utilities.Settings.Settings, IEquatable<PathSettings>
	{
		private string terminalFilesPath;
		private string workspaceFilesPath;
		private string sendFilesPath;
		private string logFilesPath;
		private string monitorFilesPath;

		public PathSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		public PathSettings(MKY.Utilities.Settings.SettingsType settingsType)
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
			this.terminalFilesPath  = rhs.TerminalFilesPath;
			this.workspaceFilesPath = rhs.WorkspaceFilesPath;
			this.sendFilesPath      = rhs.SendFilesPath;
			this.logFilesPath       = rhs.LogFilesPath;
			this.monitorFilesPath   = rhs.MonitorFilesPath;
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
		//==========================================================================================
		// Properties
		//==========================================================================================

		[XmlElement("TerminalFilesPath")]
		public virtual string TerminalFilesPath
		{
			get { return (this.terminalFilesPath); }
			set
			{
				if (value != this.terminalFilesPath)
				{
					this.terminalFilesPath = value;
					SetChanged();
				}
			}
		}

		[XmlElement("WorkspaceFilesPath")]
		public virtual string WorkspaceFilesPath
		{
			get { return (this.workspaceFilesPath); }
			set
			{
				if (value != this.workspaceFilesPath)
				{
					this.workspaceFilesPath = value;
					SetChanged();
				}
			}
		}

		[XmlElement("SendFilesPath")]
		public virtual string SendFilesPath
		{
			get { return (this.sendFilesPath); }
			set
			{
				if (value != this.sendFilesPath)
				{
					this.sendFilesPath = value;
					SetChanged();
				}
			}
		}

		[XmlElement("LogFilesPath")]
		public virtual string LogFilesPath
		{
			get { return (this.logFilesPath); }
			set
			{
				if (value != this.logFilesPath)
				{
					this.logFilesPath = value;
					SetChanged();
				}
			}
		}

		[XmlElement("MonitorFilesPath")]
		public virtual string MonitorFilesPath
		{
			get { return (this.monitorFilesPath); }
			set
			{
				if (value != this.monitorFilesPath)
				{
					this.monitorFilesPath = value;
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
			if (obj == null)
				return (false);

			PathSettings casted = obj as PathSettings;
			if (casted == null)
				return (false);

			return (Equals(casted));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(PathSettings other)
		{
			// Ensure that object.operator==() is called.
			if ((object)other == null)
				return (false);

			return
			(
				base.Equals((MKY.Utilities.Settings.Settings)other) && // Compare all settings nodes.

				(this.terminalFilesPath  == other.terminalFilesPath) &&
				(this.workspaceFilesPath == other.workspaceFilesPath) &&
				(this.sendFilesPath      == other.sendFilesPath) &&
				(this.logFilesPath       == other.logFilesPath) &&
				(this.monitorFilesPath   == other.monitorFilesPath)
			);
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

//==================================================================================================
// End of
// $URL$
//==================================================================================================
