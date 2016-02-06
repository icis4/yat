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
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Xml.Serialization;

using MKY;

namespace YAT.Settings.Terminal
{
	/// <summary></summary>
	[Serializable]
	public class ExplicitSettings : MKY.Settings.SettingsItem
	{
		/// <summary></summary>
		public const string UserNameDefault = "";

		private string userName;

		private Domain.Settings.TerminalSettings terminal;
		private Model.Settings.PredefinedCommandSettings predefinedCommand;
		private Model.Settings.FormatSettings format;
		private Log.Settings.LogSettings log;

		/// <summary></summary>
		public ExplicitSettings()
			: base(MKY.Settings.SettingsType.Explicit)
		{
			SetMyDefaults();

			Terminal          = new Domain.Settings.TerminalSettings(SettingsType);
			PredefinedCommand = new Model.Settings.PredefinedCommandSettings(SettingsType);
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
			UserName          = rhs.UserName;

			Terminal          = new Domain.Settings.TerminalSettings(rhs.Terminal);
			PredefinedCommand = new Model.Settings.PredefinedCommandSettings(rhs.PredefinedCommand);
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

			UserName = UserNameDefault;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

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
					SetChanged();
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
				if (value == null)
				{
					DetachNode(this.terminal);
					this.terminal = null;
				}
				else if (this.terminal == null)
				{
					this.terminal = value;
					AttachNode(this.terminal);
				}
				else if (this.terminal != value)
				{
					Domain.Settings.TerminalSettings old = this.terminal;
					this.terminal = value;
					ReplaceNode(old, this.terminal);
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
				if (value == null)
				{
					DetachNode(this.predefinedCommand);
					this.predefinedCommand = null;
				}
				else if (this.predefinedCommand == null)
				{
					this.predefinedCommand = value;
					AttachNode(this.predefinedCommand);
				}
				else if (this.predefinedCommand != value)
				{
					Model.Settings.PredefinedCommandSettings old = this.predefinedCommand;
					this.predefinedCommand = value;
					ReplaceNode(old, this.predefinedCommand);
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
				if (value == null)
				{
					DetachNode(this.format);
					this.format = null;
				}
				else if (this.format == null)
				{
					this.format = value;
					AttachNode(this.format);
				}
				else if (this.format != value)
				{
					Model.Settings.FormatSettings old = this.format;
					this.format = value;
					ReplaceNode(old, this.format);
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
				if (value == null)
				{
					DetachNode(this.log);
					this.log = null;
				}
				else if (this.log == null)
				{
					this.log = value;
					AttachNode(this.log);
				}
				else if (this.log != value)
				{
					Log.Settings.LogSettings old = this.log;
					this.log = value;
					ReplaceNode(old, this.log);
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

			ExplicitSettings other = (ExplicitSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				StringEx.EqualsOrdinal(UserName, other.UserName)
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
			return
			(
				base.GetHashCode() ^

				UserName.GetHashCode()
			);
		}

		#endregion

		#region Comparison Operators

		// Use of base reference type implementation of operators ==/!=.
		// See MKY.Test.EqualityTest for details.

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
