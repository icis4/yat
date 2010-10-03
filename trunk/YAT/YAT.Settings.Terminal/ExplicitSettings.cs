//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace YAT.Settings.Terminal
{
	/// <summary></summary>
	[Serializable]
	public class ExplicitSettings : MKY.Utilities.Settings.Settings
	{
		private Domain.Settings.TerminalSettings terminal;
		private Model.Settings.PredefinedCommandSettings predefinedCommand;
		private Model.Settings.FormatSettings format;
		private Domain.Settings.CharReplaceSettings charReplace;
		private Log.Settings.LogSettings log;

		/// <summary></summary>
		public ExplicitSettings()
			: base(MKY.Utilities.Settings.SettingsType.Explicit)
		{
			SetMyDefaults();

			Terminal          = new Domain.Settings.TerminalSettings(SettingsType);
			PredefinedCommand = new Model.Settings.PredefinedCommandSettings(SettingsType);
			Format            = new Model.Settings.FormatSettings(SettingsType);
			CharReplace       = new Domain.Settings.CharReplaceSettings(SettingsType);
			Log               = new Log.Settings.LogSettings(SettingsType);

			ClearChanged();
		}

		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public ExplicitSettings(ExplicitSettings rhs)
			: base(rhs)
		{
			Terminal          = new Domain.Settings.TerminalSettings(rhs.Terminal);
			PredefinedCommand = new Model.Settings.PredefinedCommandSettings(rhs.PredefinedCommand);
			Format            = new Model.Settings.FormatSettings(rhs.Format);
			CharReplace       = new Domain.Settings.CharReplaceSettings(rhs.CharReplace);
			Log               = new Log.Settings.LogSettings(rhs.Log);

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			// Nothing to do.
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("Terminal")]
		public Domain.Settings.TerminalSettings Terminal
		{
			get { return (this.terminal); }
			set
			{
				if (value == null)
				{
					this.terminal = value;
					DetachNode(this.terminal);
				}
				else if (this.terminal == null)
				{
					this.terminal = value;
					AttachNode(this.terminal);
				}
				else if (value != this.terminal)
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
					this.predefinedCommand = value;
					DetachNode(this.predefinedCommand);
				}
				else if (this.predefinedCommand == null)
				{
					this.predefinedCommand = value;
					AttachNode(this.predefinedCommand);
				}
				else if (value != this.predefinedCommand)
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
					this.format = value;
					DetachNode(this.format);
				}
				else if (this.format == null)
				{
					this.format = value;
					AttachNode(this.format);
				}
				else if (value != this.format)
				{
					Model.Settings.FormatSettings old = this.format;
					this.format = value;
					ReplaceNode(old, this.format);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("CharReplace")]
		public Domain.Settings.CharReplaceSettings CharReplace
		{
			get { return (this.charReplace); }
			set
			{
				if (value == null)
				{
					this.charReplace = value;
					DetachNode(this.charReplace);
				}
				else if (this.charReplace == null)
				{
					this.charReplace = value;
					AttachNode(this.charReplace);
				}
				else if (value != this.charReplace)
				{
					Domain.Settings.CharReplaceSettings old = this.charReplace;
					this.charReplace = value;
					ReplaceNode(old, this.charReplace);
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
					this.log = value;
					DetachNode(this.log);
				}
				else if (this.log == null)
				{
					this.log = value;
					AttachNode(this.log);
				}
				else if (value != this.log)
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
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(obj, null))
				return (false);

			if (GetType() != obj.GetType())
				return (false);

			ExplicitSettings other = (ExplicitSettings)obj;
			return (base.Equals(other)); // Compare all settings nodes.
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		#endregion

		#region Comparison Operators

		// Use of base reference type implementation of operators ==/!=.
		// See MKY.Utilities.Test.EqualityTest for details.

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
