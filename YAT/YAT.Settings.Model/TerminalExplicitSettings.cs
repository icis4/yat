//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT Version 2.1.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Xml.Serialization;

using MKY;

namespace YAT.Settings.Model
{
	/// <summary></summary>
	public class TerminalExplicitSettings : MKY.Settings.SettingsItem, IEquatable<TerminalExplicitSettings>
	{
		/// <summary></summary>
		public const bool LogIsOnDefault = false;

		/// <summary></summary>
		public const string UserNameDefault = "";

		private bool logIsOn;
		private string userName;

		private Domain.Settings.TerminalSettings terminal;
		private YAT.Model.Settings.PredefinedCommandSettings predefinedCommand;
		private YAT.Model.Settings.AutoActionSettings autoAction;
		private YAT.Model.Settings.AutoResponseSettings autoResponse;
		private Format.Settings.FormatSettings format;
		private Log.Settings.LogSettings log;

		/// <summary></summary>
		public TerminalExplicitSettings()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public TerminalExplicitSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();

			Terminal          = new Domain.Settings.TerminalSettings(SettingsType);
			PredefinedCommand = new YAT.Model.Settings.PredefinedCommandSettings(SettingsType);
			AutoAction        = new YAT.Model.Settings.AutoActionSettings(SettingsType);
			AutoResponse      = new YAT.Model.Settings.AutoResponseSettings(SettingsType);
			Format            = new Format.Settings.FormatSettings(SettingsType);
			Log               = new Log.Settings.LogSettings(SettingsType);

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public TerminalExplicitSettings(TerminalExplicitSettings rhs)
			: base(rhs)
		{
			LogIsOn  = rhs.LogIsOn;
			UserName = rhs.UserName;

			Terminal          = new Domain.Settings.TerminalSettings(rhs.Terminal);
			PredefinedCommand = new YAT.Model.Settings.PredefinedCommandSettings(rhs.PredefinedCommand);
			AutoAction        = new YAT.Model.Settings.AutoActionSettings(rhs.AutoAction);
			AutoResponse      = new YAT.Model.Settings.AutoResponseSettings(rhs.AutoResponse);
			Format            = new Format.Settings.FormatSettings(rhs.Format);
			Log               = new Log.Settings.LogSettings(rhs.Log);

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			LogIsOn  = LogIsOnDefault;
			UserName = UserNameDefault;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <remarks>
		/// This property is intentionally located in 'explicit' for the following reasons:
		///  > The decision whether or not to log something is something explicit.
		///  > Opposed to <see cref="TerminalImplicitSettings.TerminalIsStarted"/>, logging produces data.
		///  > Logging is coupled to the terminal state, so it will only become active then the
		///    terminal is started.
		///
		/// Note that this setting as well as <see cref="TerminalImplicitSettings.TerminalIsStarted"/> both
		/// used to be 'implicit' up to 1.99.34 and then got moved to 'explicit' for 1.99.50/51/52.
		/// But, as described in <see cref="TerminalImplicitSettings.TerminalIsStarted"/>, that setting got
		/// reverted for 1.99.70+ while this settings is kept here for the above stated reasons.
		/// </remarks>
		[XmlElement("LogIsOn")]
		public virtual bool LogIsOn
		{
			get { return (this.logIsOn); }
			set
			{
				if (this.logIsOn != value)
				{
					this.logIsOn = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("UserName")]
		public virtual string UserName
		{
			get { return (this.userName); }
			set
			{
				if (this.userName != value)
				{
					this.userName = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Terminal")]
		public Domain.Settings.TerminalSettings Terminal
		{
			get { return (this.terminal); }
			set
			{
				if (this.terminal != value)
				{
					var oldNode = this.terminal;
					this.terminal = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("PredefinedCommand")]
		public YAT.Model.Settings.PredefinedCommandSettings PredefinedCommand
		{
			get { return (this.predefinedCommand); }
			set
			{
				if (this.predefinedCommand != value)
				{
					var oldNode = this.predefinedCommand;
					this.predefinedCommand = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("AutoAction")]
		public virtual YAT.Model.Settings.AutoActionSettings AutoAction
		{
			get { return (this.autoAction); }
			set
			{
				if (this.autoAction != value)
				{
					var oldNode = this.autoAction;
					this.autoAction = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("AutoResponse")]
		public virtual YAT.Model.Settings.AutoResponseSettings AutoResponse
		{
			get { return (this.autoResponse); }
			set
			{
				if (this.autoResponse != value)
				{
					var oldNode = this.autoResponse;
					this.autoResponse = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Format")]
		public Format.Settings.FormatSettings Format
		{
			get { return (this.format); }
			set
			{
				if (this.format != value)
				{
					var oldNode = this.format;
					this.format = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Log")]
		public Log.Settings.LogSettings Log
		{
			get { return (this.log); }
			set
			{
				if (this.log != value)
				{
					var oldNode = this.log;
					this.log = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
				}
			}
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

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

				hashCode = (hashCode * 397) ^  LogIsOn                    .GetHashCode();
				hashCode = (hashCode * 397) ^ (UserName != null ? UserName.GetHashCode() : 0);

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as TerminalExplicitSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(TerminalExplicitSettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				LogIsOn .Equals                           (other.LogIsOn) &&
				StringEx.EqualsOrdinalIgnoreCase(UserName, other.UserName)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(TerminalExplicitSettings lhs, TerminalExplicitSettings rhs)
		{
			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			object obj = (object)lhs; // Operators are not virtual! Calling object.Equals() ensures
			return (obj.Equals(rhs)); // that a potential virtual <Derived>.Equals() is called.
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(TerminalExplicitSettings lhs, TerminalExplicitSettings rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
