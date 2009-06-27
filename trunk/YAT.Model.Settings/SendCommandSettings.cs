//==================================================================================================
// URL       : $URL$
// Author    : $Author$
// Date      : $Date$
// Revision  : $Rev$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2009 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using MKY.Utilities.Recent;

using YAT.Model.Types;

namespace YAT.Model.Settings
{
	/// <summary></summary>
	[Serializable]
	public class SendCommandSettings : MKY.Utilities.Settings.Settings, IEquatable<SendCommandSettings>
	{
		/// <summary></summary>
		public const int MaximumRecentCommands = 24;

		private Command _command;
		private RecentItemCollection<Command> _recentsCommands;

		/// <summary></summary>
		public SendCommandSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public SendCommandSettings(MKY.Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public SendCommandSettings(SendCommandSettings rhs)
			: base(rhs)
		{
			Command = new Command(rhs.Command);
			RecentCommands = new RecentItemCollection<Command>(rhs.RecentCommands);
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			Command = new Command();
			RecentCommands = new RecentItemCollection<Command>(MaximumRecentCommands);
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
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

		/// <summary></summary>
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

		private const string _EasterEggCommand = @"\easteregg";

		/// <summary></summary>
		public static bool IsEasterEggCommand(string command)
		{
			return (string.Compare(command, _EasterEggCommand, true) == 0);
		}

		/// <summary></summary>
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

//==================================================================================================
// End of $URL$
//==================================================================================================
