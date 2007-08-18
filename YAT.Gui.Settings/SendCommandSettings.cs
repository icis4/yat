using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using MKY.Utilities.Recent;

namespace MKY.YAT.Gui.Settings
{
	[Serializable]
	public class SendCommandSettings : Utilities.Settings.Settings, IEquatable<SendCommandSettings>
	{
		public const int MaximumRecentCommands = 24;

		private Command _command;
		private RecentItemCollection<Command> _recentsCommands;

		public SendCommandSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		public SendCommandSettings(Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		public SendCommandSettings(SendCommandSettings rhs)
			: base(rhs)
		{
			Command = new Command(rhs.Command);
			RecentCommands = new RecentItemCollection<Command>(rhs.RecentCommands);
			ClearChanged();
		}

		protected override void SetMyDefaults()
		{
			Command = new Command();
			RecentCommands = new RecentItemCollection<Command>(MaximumRecentCommands);
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

		[XmlElement("RecentCommands")]
		public RecentItemCollection<Command> RecentCommands
		{
			get { return (_recentsCommands); }
			set
			{
				if (_recentsCommands != value)
				{
					_recentsCommands = value;
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
			if (obj is SendCommandSettings)
				return (Equals((SendCommandSettings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(SendCommandSettings value)
		{
			// ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return
					(
					_command.Equals(value._command) &&
					_recentsCommands.Equals(value._recentsCommands)
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
		public static bool operator ==(SendCommandSettings lhs, SendCommandSettings rhs)
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
		public static bool operator !=(SendCommandSettings lhs, SendCommandSettings rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion

		#region Comparision
		//------------------------------------------------------------------------------------------
		// Comparision ;-)
		//------------------------------------------------------------------------------------------

		private const string _EasterEggCommand = "\\easteregg";

		public static bool IsEasterEggCommand(string command)
		{
			return (string.Compare(command, _EasterEggCommand, true) == 0);
		}

		public static string EasterEggCommandText
		{
			get
			{
				return (":-)");
			}
		}

		#endregion
	}
}
