//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2'' Version 1.99.52
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

using MKY;

using YAT.Model.Types;

namespace YAT.Model.Settings
{
	/// <summary></summary>
	public class WorkspaceSettings : MKY.Settings.SettingsItem
	{
		private bool alwaysOnTop;
		private WorkspaceLayout layout;
		private GuidList<TerminalSettingsItem> terminalSettings;

		/// <summary></summary>
		public WorkspaceSettings()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <summary></summary>
		public WorkspaceSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public WorkspaceSettings(WorkspaceSettings rhs)
			: base(rhs)
		{
			AlwaysOnTop      = rhs.AlwaysOnTop;
			Layout           = rhs.Layout;
			TerminalSettings = new GuidList<TerminalSettingsItem>(rhs.TerminalSettings);
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			AlwaysOnTop      = false;
			Layout           = WorkspaceLayout.Automatic;
			TerminalSettings = new GuidList<TerminalSettingsItem>();
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("AlwaysOnTop")]
		public bool AlwaysOnTop
		{
			get { return (this.alwaysOnTop); }
			set
			{
				if (this.alwaysOnTop != value)
				{
					this.alwaysOnTop = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Layout")]
		public WorkspaceLayout Layout
		{
			get { return (this.layout); }
			set
			{
				if (this.layout != value)
				{
					this.layout = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Public getter is required for default XML serialization/deserialization.")]
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Public setter is required for default XML serialization/deserialization.")]
		[XmlElement("TerminalSettings")]
		public GuidList<TerminalSettingsItem> TerminalSettings
		{
			get { return (this.terminalSettings); }
			set
			{
				if (this.terminalSettings != value)
				{
					this.terminalSettings = value;
					SetMyChanged();
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

			WorkspaceSettings other = (WorkspaceSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(AlwaysOnTop      == other.AlwaysOnTop) &&
				(Layout           == other.Layout) &&
				(TerminalSettings == other.TerminalSettings)
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
			unchecked
			{
				int hashCode = base.GetHashCode(); // Get hash code of all settings nodes.

				hashCode = (hashCode * 397) ^  AlwaysOnTop     .GetHashCode();
				hashCode = (hashCode * 397) ^  Layout          .GetHashCode();
				hashCode = (hashCode * 397) ^  TerminalSettings.GetHashCode();

				return (hashCode);
			}
		}

		#endregion

		#region Comparison Operators

		// Use of base reference type implementation of operators ==/!=.
		// See MKY.Test.EqualityAnalysis for details.

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
