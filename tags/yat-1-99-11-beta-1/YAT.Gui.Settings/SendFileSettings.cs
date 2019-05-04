using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace HSR.YAT.Gui.Settings
{
	public class SendFileSettings : Utilities.Settings.Settings
	{
		private Command _command;

		public SendFileSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		public SendFileSettings(Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		public SendFileSettings(SendFileSettings rhs)
			: base(rhs)
		{
			Command = new Command(rhs.Command);
			ClearChanged();
		}

		protected override void SetMyDefaults()
		{
			Command = new Command();
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		[XmlElement("Command")]
		public Command Command
		{
			get { return (_command); }
			set
			{
				if (_command != value)
				{
					_command = value;
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
			if (obj is SendFileSettings)
				return (Equals((SendFileSettings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(SendFileSettings value)
		{
			// ensure that object.operator!=() is called
			if ((object)value != null)
				return (_command.Equals(value._command));

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
		public static bool operator ==(SendFileSettings lhs, SendFileSettings rhs)
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
		public static bool operator !=(SendFileSettings lhs, SendFileSettings rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}