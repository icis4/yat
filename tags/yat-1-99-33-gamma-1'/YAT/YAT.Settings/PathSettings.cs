﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 1' Version 1.99.33
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
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
		/// Set fields through properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
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
			base.SetMyDefaults();

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
				if (this.terminalFilesPath != value)
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
				if (this.workspaceFilesPath != value)
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
				if (this.sendFilesPath != value)
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
				if (this.logFilesPath != value)
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
				if (this.monitorFilesPath != value)
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

			PathSettings other = (PathSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				PathEx.Equals(TerminalFilesPath,  other.TerminalFilesPath) &&
				PathEx.Equals(WorkspaceFilesPath, other.WorkspaceFilesPath) &&
				PathEx.Equals(SendFilesPath,      other.SendFilesPath) &&
				PathEx.Equals(LogFilesPath,       other.LogFilesPath) &&
				PathEx.Equals(MonitorFilesPath,   other.MonitorFilesPath)
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
			return
			(
				base.GetHashCode() ^

				this.TerminalFilesPath .GetHashCode() ^
				this.WorkspaceFilesPath.GetHashCode() ^
				this.SendFilesPath     .GetHashCode() ^
				this.LogFilesPath      .GetHashCode() ^
				this.MonitorFilesPath  .GetHashCode()
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