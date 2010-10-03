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
using System.Windows.Forms;
using System.Xml.Serialization;

namespace YAT.Settings
{
	/// <summary></summary>
	[Serializable]
	public class GeneralSettings : MKY.Utilities.Settings.Settings
	{
		/// <summary></summary>
		public static readonly string AutoSaveRoot = Application.LocalUserAppDataPath;

		/// <summary></summary>
		public const string AutoSaveTerminalFileNamePrefix = "Terminal-";

		/// <summary></summary>
		public const string AutoSaveWorkspaceFileNamePrefix = "Workspace-";

		private bool autoOpenWorkspace;
		private bool autoSaveWorkspace;
		private bool useRelativePaths;
		private bool detectSerialPortsInUse;

		/// <summary></summary>
		public GeneralSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
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
			AutoOpenWorkspace      = rhs.AutoOpenWorkspace;
			AutoSaveWorkspace      = rhs.AutoSaveWorkspace;
			UseRelativePaths       = rhs.UseRelativePaths;
			DetectSerialPortsInUse = rhs.DetectSerialPortsInUse;

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
			DetectSerialPortsInUse = true;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("AutoOpenWorkspace")]
		public virtual bool AutoOpenWorkspace
		{
			get { return (this.autoOpenWorkspace); }
			set
			{
				if (value != this.autoOpenWorkspace)
				{
					this.autoOpenWorkspace = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("AutoSaveWorkspace")]
		public virtual bool AutoSaveWorkspace
		{
			get { return (this.autoSaveWorkspace); }
			set
			{
				if (value != this.autoSaveWorkspace)
				{
					this.autoSaveWorkspace = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("UseRelativePaths")]
		public virtual bool UseRelativePaths
		{
			get { return (this.useRelativePaths); }
			set
			{
				if (value != this.useRelativePaths)
				{
					this.useRelativePaths = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("DetectSerialPortsInUse")]
		public virtual bool DetectSerialPortsInUse
		{
			get { return (this.detectSerialPortsInUse); }
			set
			{
				if (value != this.detectSerialPortsInUse)
				{
					this.detectSerialPortsInUse = value;
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

			GeneralSettings other = (GeneralSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(this.autoOpenWorkspace      == other.autoOpenWorkspace) &&
				(this.autoSaveWorkspace      == other.autoSaveWorkspace) &&
				(this.useRelativePaths       == other.useRelativePaths) &&
				(this.detectSerialPortsInUse == other.detectSerialPortsInUse)
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return
			(
				base.GetHashCode() ^

				this.autoOpenWorkspace     .GetHashCode() ^
				this.autoSaveWorkspace     .GetHashCode() ^
				this.useRelativePaths      .GetHashCode() ^
				this.detectSerialPortsInUse.GetHashCode()
			);
		}

		#endregion

		#region Comparison Operators

		// Use of base reference type implementation of operators ==/!=.
		// See MKY.Utilities.Test.EqualityTest for details.

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
