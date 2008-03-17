using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using MKY.Utilities.Guid;

namespace YAT.Model.Settings
{
	/// <summary></summary>
	[Serializable]
	public class WorkspaceSettings : MKY.Utilities.Settings.Settings, IEquatable<WorkspaceSettings>
	{
		private GuidList<TerminalSettingsItem> _terminalSettings;

		/// <summary></summary>
		public WorkspaceSettings()
			: base(MKY.Utilities.Settings.SettingsType.Explicit)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public WorkspaceSettings(MKY.Utilities.Settings.SettingsType settingsType)
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
			get { return (_terminalSettings); }
			set
			{
				if (_terminalSettings != value)
				{
					_terminalSettings = value;
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
			if (obj is WorkspaceSettings)
				return (Equals((WorkspaceSettings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(WorkspaceSettings value)
		{
			// ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return
					(
					_terminalSettings.Equals(value._terminalSettings)
					);
			}
			return (false);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(WorkspaceSettings lhs, WorkspaceSettings rhs)
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
		public static bool operator !=(WorkspaceSettings lhs, WorkspaceSettings rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}