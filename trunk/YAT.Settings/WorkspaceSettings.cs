using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MKY.YAT.Settings
{
	[Serializable]
	public class WorkspaceSettings : Utilities.Settings.Settings, IEquatable<WorkspaceSettings>
	{
		private TerminalSettingsItemCollection _terminalSettingsItems;

		public WorkspaceSettings()
			: base(Utilities.Settings.SettingsType.Explicit)
		{
			SetMyDefaults();
			ClearChanged();
		}

		public WorkspaceSettings(Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		public WorkspaceSettings(WorkspaceSettings rhs)
			: base(rhs)
		{
			_terminalSettingsItems = new TerminalSettingsItemCollection(rhs.TerminalSettingsItems);
			ClearChanged();
		}

		protected override void SetMyDefaults()
		{
			_terminalSettingsItems = new TerminalSettingsItemCollection();
			SetChanged();
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		[XmlElement("TerminalSettingsItems")]
		public TerminalSettingsItemCollection TerminalSettingsItems
		{
			get { return (_terminalSettingsItems); }
			set
			{
				if (_terminalSettingsItems != value)
				{
					_terminalSettingsItems = value;
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
					_terminalSettingsItems.Equals(value._terminalSettingsItems)
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