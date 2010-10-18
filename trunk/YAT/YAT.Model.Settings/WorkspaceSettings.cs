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

using MKY.System.Guid;

namespace YAT.Model.Settings
{
	/// <summary></summary>
	[Serializable]
	public class WorkspaceSettings : MKY.System.Settings.Settings
	{
		private GuidList<TerminalSettingsItem> terminalSettings;

		/// <summary></summary>
		public WorkspaceSettings()
			: base(MKY.System.Settings.SettingsType.Explicit)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public WorkspaceSettings(MKY.System.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public WorkspaceSettings(WorkspaceSettings rhs)
			: base(rhs)
		{
			TerminalSettings = new GuidList<TerminalSettingsItem>(rhs.TerminalSettings);
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			TerminalSettings = new GuidList<TerminalSettingsItem>();
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("TerminalSettings")]
		public GuidList<TerminalSettingsItem> TerminalSettings
		{
			get { return (this.terminalSettings); }
			set
			{
				if (value != this.terminalSettings)
				{
					this.terminalSettings = value;
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

			WorkspaceSettings other = (WorkspaceSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.
				(this.terminalSettings == other.terminalSettings)
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return
			(
				base.GetHashCode() ^
				this.terminalSettings.GetHashCode()
			);
		}

		#endregion

		#region Comparison Operators

		// Use of base reference type implementation of operators ==/!=.
		// See MKY.System.Test.EqualityTest for details.

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
