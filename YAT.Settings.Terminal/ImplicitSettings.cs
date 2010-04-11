//==================================================================================================
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
	[Serializable]
	public class ImplicitSettings : MKY.Utilities.Settings.Settings, IEquatable<ImplicitSettings>
	{
		private bool terminalIsStarted;
		private bool logIsStarted;

		private Model.Settings.SendCommandSettings sendCommand;
		private Model.Settings.SendFileSettings sendFile;
		private Model.Settings.PredefinedSettings predefined;
		private Model.Settings.WindowSettings window;
		private Model.Settings.LayoutSettings layout;

		public ImplicitSettings()
			: base(MKY.Utilities.Settings.SettingsType.Implicit)
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
			this.terminalIsStarted = rhs.TerminalIsStarted;
			this.logIsStarted      = rhs.LogIsStarted;

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

		[XmlElement("SendCommand")]
		public virtual Model.Settings.SendCommandSettings SendCommand
		{
			get { return (this.sendCommand); }
			set
			{
				if (this.sendCommand == null)
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

		[XmlElement("SendFile")]
		public virtual Model.Settings.SendFileSettings SendFile
		{
			get { return (this.sendFile); }
			set
			{
				if (this.sendFile == null)
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

		[XmlElement("Predefined")]
		public virtual Model.Settings.PredefinedSettings Predefined
		{
			get { return (this.predefined); }
			set
			{
				if (this.predefined == null)
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

		[XmlElement("Window")]
		public virtual Model.Settings.WindowSettings Window
		{
			get { return (this.window); }
			set
			{
				if (this.window == null)
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

		[XmlElement("Layout")]
		public virtual Model.Settings.LayoutSettings Layout
		{
			get { return (this.layout); }
			set
			{
				if (this.layout == null)
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
			if (obj is ImplicitSettings)
				return (Equals((ImplicitSettings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(ImplicitSettings value)
		{
			// Ensure that object.operator!=() is called.
			if ((object)value != null)
			{
				return
					(
					(this.terminalIsStarted == value.terminalIsStarted) &&
					(this.logIsStarted      == value.logIsStarted) &&
					base.Equals((MKY.Utilities.Settings.Settings)value) // Compare all settings nodes.
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
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(ImplicitSettings lhs, ImplicitSettings rhs)
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
		public static bool operator !=(ImplicitSettings lhs, ImplicitSettings rhs)
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
