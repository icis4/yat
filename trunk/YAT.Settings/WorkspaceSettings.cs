using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MKY.YAT.Settings
{
	[Serializable]
	public class WorkspaceSettings : Utilities.Settings.Settings, IEquatable<WorkspaceSettings>
	{
		private TerminalSettingsItemCollection _terminalSettings;

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

		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public WorkspaceSettings(WorkspaceSettings rhs)
			: base(rhs)
		{
			TerminalSettings = new TerminalSettingsItemCollection(rhs.TerminalSettings);
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			TerminalSettings = new TerminalSettingsItemCollection();
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		[XmlElement("TerminalSettings")]
		public TerminalSettingsItemCollection TerminalSettings
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