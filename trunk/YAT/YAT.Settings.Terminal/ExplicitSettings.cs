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
using System.Xml.Serialization;

using MKY;

namespace YAT.Settings.Terminal
{
	/// <summary></summary>
	public class ExplicitSettings : MKY.Settings.SettingsItem, IEquatable<ExplicitSettings>
	{
		/// <summary></summary>
		public const bool TerminalIsStartedDefault = true;

		/// <summary></summary>
		public const bool LogIsOnDefault = false;

		/// <summary></summary>
		public const string UserNameDefault = "";

		private bool terminalIsStarted;
		private bool logIsOn;
		private string userName;

		private Domain.Settings.TerminalSettings terminal;
		private Model.Settings.PredefinedCommandSettings predefinedCommand;
		private Model.Settings.AutoResponseSettings autoResponse;
		private Model.Settings.FormatSettings format;
		private Log.Settings.LogSettings log;

		/// <summary></summary>
		public ExplicitSettings()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <summary></summary>
		public ExplicitSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();

			Terminal          = new Domain.Settings.TerminalSettings(SettingsType);
			PredefinedCommand = new Model.Settings.PredefinedCommandSettings(SettingsType);
			AutoResponse      = new Model.Settings.AutoResponseSettings(SettingsType);
			Format            = new Model.Settings.FormatSettings(SettingsType);
			Log               = new Log.Settings.LogSettings(SettingsType);

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public ExplicitSettings(ExplicitSettings rhs)
			: base(rhs)
		{
			TerminalIsStarted = rhs.TerminalIsStarted;
			LogIsOn           = rhs.LogIsOn;
			UserName          = rhs.UserName;

			Terminal          = new Domain.Settings.TerminalSettings(rhs.Terminal);
			PredefinedCommand = new Model.Settings.PredefinedCommandSettings(rhs.PredefinedCommand);
			AutoResponse      = new Model.Settings.AutoResponseSettings(rhs.AutoResponse);
			Format            = new Model.Settings.FormatSettings(rhs.Format);
			Log               = new Log.Settings.LogSettings(rhs.Log);

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			TerminalIsStarted = TerminalIsStartedDefault;
			LogIsOn           = LogIsOnDefault;
			UserName          = UserNameDefault;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("TerminalIsStarted")]
		public virtual bool TerminalIsStarted
		{
			get { return (this.terminalIsStarted); }
			set
			{
				if (this.terminalIsStarted != value)
				{
					this.terminalIsStarted = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
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
		public Model.Settings.PredefinedCommandSettings PredefinedCommand
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
		[XmlElement("AutoResponse")]
		public virtual Model.Settings.AutoResponseSettings AutoResponse
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
		public Model.Settings.FormatSettings Format
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

				hashCode = (hashCode * 397) ^  TerminalIsStarted          .GetHashCode();
				hashCode = (hashCode * 397) ^  LogIsOn                    .GetHashCode();
				hashCode = (hashCode * 397) ^ (UserName != null ? UserName.GetHashCode() : 0);

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as ExplicitSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(ExplicitSettings other)
		{
			if (ReferenceEquals(other, null))
				return (false);

			if (ReferenceEquals(this, other))
				return (true);

			if (this.GetType() != other.GetType())
				return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				TerminalIsStarted.Equals(other.TerminalIsStarted) &&
				LogIsOn          .Equals(other.LogIsOn)           &&
				StringEx.EqualsOrdinalIgnoreCase(UserName, other.UserName)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(ExplicitSettings lhs, ExplicitSettings rhs)
		{
			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(ExplicitSettings lhs, ExplicitSettings rhs)
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
