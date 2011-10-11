﻿//==================================================================================================
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
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using MKY.IO;

namespace YAT.Settings
{
	/// <summary></summary>
	[Serializable]
	public class PathSettings : MKY.Settings.SettingsItem
	{
		private string terminalFilesPath;
		private string workspaceFilesPath;
		private string sendFilesPath;
		private string logFilesPath;
		private string monitorFilesPath;

		/// <summary></summary>
		public PathSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public PathSettings(MKY.Settings.SettingsType settingsType)
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
			TerminalFilesPath  = rhs.TerminalFilesPath;
			WorkspaceFilesPath = rhs.WorkspaceFilesPath;
			SendFilesPath      = rhs.SendFilesPath;
			LogFilesPath       = rhs.LogFilesPath;
			MonitorFilesPath   = rhs.MonitorFilesPath;

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

		/// <summary></summary>
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

		/// <summary></summary>
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

		/// <summary></summary>
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

		/// <summary></summary>
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

		/// <summary></summary>
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
			if (ReferenceEquals(obj, null))
				return (false);

			if (GetType() != obj.GetType())
				return (false);

			PathSettings other = (PathSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				PathEx.Equals(this.terminalFilesPath,  other.terminalFilesPath) &&
				PathEx.Equals(this.workspaceFilesPath, other.workspaceFilesPath) &&
				PathEx.Equals(this.sendFilesPath,      other.sendFilesPath) &&
				PathEx.Equals(this.logFilesPath,       other.logFilesPath) &&
				PathEx.Equals(this.monitorFilesPath,   other.monitorFilesPath)
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return
			(
				base.GetHashCode() ^

				this.terminalFilesPath .GetHashCode() ^
				this.workspaceFilesPath.GetHashCode() ^
				this.sendFilesPath     .GetHashCode() ^
				this.logFilesPath      .GetHashCode() ^
				this.monitorFilesPath  .GetHashCode()
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
