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
	public class ImplicitSettings : MKY.Settings.Settings
	{
		private bool terminalIsStarted;
		private bool logIsStarted;

		private Model.Settings.SendCommandSettings sendCommand;
		private Model.Settings.SendFileSettings sendFile;
		private Model.Settings.PredefinedSettings predefined;
		private Model.Settings.WindowSettings window;
		private Model.Settings.LayoutSettings layout;

		/// <summary></summary>
		public ImplicitSettings()
			: base(MKY.Settings.SettingsType.Implicit)
		{
			SetMyDefaults();

			SendCommand = new Model.Settings.SendCommandSettings(SettingsType);
			SendFile    = new Model.Settings.SendFileSettings(SettingsType);
			Predefined  = new Model.Settings.PredefinedSettings(SettingsType);
			Window      = new Model.Settings.WindowSettings(SettingsType);
			Layout      = new Model.Settings.LayoutSettings(SettingsType);

			ClearChanged();
		}

		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public ImplicitSettings(ImplicitSettings rhs)
			: base(rhs)
		{
			TerminalIsStarted = rhs.TerminalIsStarted;
			LogIsStarted      = rhs.LogIsStarted;

			SendCommand = new Model.Settings.SendCommandSettings(rhs.SendCommand);
			SendFile    = new Model.Settings.SendFileSettings(rhs.SendFile);
			Predefined  = new Model.Settings.PredefinedSettings(rhs.Predefined);
			Window      = new Model.Settings.WindowSettings(rhs.Window);
			Layout      = new Model.Settings.LayoutSettings(rhs.Layout);

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			TerminalIsStarted = true;
			LogIsStarted = false;
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
				if (value != this.terminalIsStarted)
				{
					this.terminalIsStarted = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("LogIsStarted")]
		public virtual bool LogIsStarted
		{
			get { return (this.logIsStarted); }
			set
			{
				if (value != this.logIsStarted)
				{
					this.logIsStarted = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SendCommand")]
		public virtual Model.Settings.SendCommandSettings SendCommand
		{
			get { return (this.sendCommand); }
			set
			{
				if (value == null)
				{
					this.sendCommand = value;
					DetachNode(this.sendCommand);
				}
				else if (this.sendCommand == null)
				{
					this.sendCommand = value;
					AttachNode(this.sendCommand);
				}
				else if (value != this.sendCommand)
				{
					Model.Settings.SendCommandSettings old = this.sendCommand;
					this.sendCommand = value;
					ReplaceNode(old, this.sendCommand);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SendFile")]
		public virtual Model.Settings.SendFileSettings SendFile
		{
			get { return (this.sendFile); }
			set
			{
				if (value == null)
				{
					this.sendFile = value;
					DetachNode(this.sendFile);
				}
				else if (this.sendFile == null)
				{
					this.sendFile = value;
					AttachNode(this.sendFile);
				}
				else if (value != this.sendFile)
				{
					Model.Settings.SendFileSettings old = this.sendFile;
					this.sendFile = value;
					ReplaceNode(old, this.sendFile);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Predefined")]
		public virtual Model.Settings.PredefinedSettings Predefined
		{
			get { return (this.predefined); }
			set
			{
				if (value == null)
				{
					this.predefined = value;
					DetachNode(this.predefined);
				}
				else if (this.predefined == null)
				{
					this.predefined = value;
					AttachNode(this.predefined);
				}
				else if (value != this.predefined)
				{
					Model.Settings.PredefinedSettings old = this.predefined;
					this.predefined = value;
					ReplaceNode(old, this.predefined);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Window")]
		public virtual Model.Settings.WindowSettings Window
		{
			get { return (this.window); }
			set
			{
				if (value == null)
				{
					this.window = value;
					DetachNode(this.window);
				}
				else if (this.window == null)
				{
					this.window = value;
					AttachNode(this.window);
				}
				else if (value != this.window)
				{
					Model.Settings.WindowSettings old = this.window;
					this.window = value;
					ReplaceNode(old, this.window);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Layout")]
		public virtual Model.Settings.LayoutSettings Layout
		{
			get { return (this.layout); }
			set
			{
				if (value == null)
				{
					this.layout = value;
					DetachNode(this.layout);
				}
				else if (this.layout == null)
				{
					this.layout = value;
					AttachNode(this.layout);
				}
				else if (value != this.layout)
				{
					Model.Settings.LayoutSettings old = this.layout;
					this.layout = value;
					ReplaceNode(old, this.layout);
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

			ImplicitSettings other = (ImplicitSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(this.terminalIsStarted == other.terminalIsStarted) &&
				(this.logIsStarted      == other.logIsStarted)
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return
			(
				base.GetHashCode() ^

				this.terminalIsStarted.GetHashCode() ^
				this.logIsStarted     .GetHashCode()
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
