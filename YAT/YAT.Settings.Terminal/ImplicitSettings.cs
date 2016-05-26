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

namespace YAT.Settings.Terminal
{
	/// <summary></summary>
	[Serializable]
	public class ImplicitSettings : MKY.Settings.SettingsItem
	{
		/// <summary></summary>
		public const bool TerminalIsStartedDefault = false;

		/// <summary></summary>
		public const bool LogIsOnDefault = false;

		private bool terminalIsStarted;
		private bool logIsOn;

		private Model.Settings.SendTextSettings sendText;
		private Model.Settings.SendFileSettings sendFile;
		private Model.Settings.PredefinedSettings predefined;
		private Model.Settings.AutoResponseSettings autoResponse;
		private Model.Settings.WindowSettings window;
		private Model.Settings.LayoutSettings layout;
		private Model.Settings.ViewSettings view;

		/// <summary></summary>
		public ImplicitSettings()
			: base(MKY.Settings.SettingsType.Implicit)
		{
			SetMyDefaults();

			SendText     = new Model.Settings.SendTextSettings(SettingsType);
			SendFile     = new Model.Settings.SendFileSettings(SettingsType);
			Predefined   = new Model.Settings.PredefinedSettings(SettingsType);
			AutoResponse = new Model.Settings.AutoResponseSettings(SettingsType);
			Window       = new Model.Settings.WindowSettings(SettingsType);
			Layout       = new Model.Settings.LayoutSettings(SettingsType);
			View         = new Model.Settings.ViewSettings(SettingsType);

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public ImplicitSettings(ImplicitSettings rhs)
			: base(rhs)
		{
			TerminalIsStarted = rhs.TerminalIsStarted;
			LogIsOn           = rhs.LogIsOn;

			SendText     = new Model.Settings.SendTextSettings(rhs.SendText);
			SendFile     = new Model.Settings.SendFileSettings(rhs.SendFile);
			Predefined   = new Model.Settings.PredefinedSettings(rhs.Predefined);
			AutoResponse = new Model.Settings.AutoResponseSettings(rhs.AutoResponse);
			Window       = new Model.Settings.WindowSettings(rhs.Window);
			Layout       = new Model.Settings.LayoutSettings(rhs.Layout);
			View         = new Model.Settings.ViewSettings(rhs.View);

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
					SetChanged();
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
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SendText")]
		public virtual Model.Settings.SendTextSettings SendText
		{
			get { return (this.sendText); }
			set
			{
				if (value == null)
				{
					DetachNode(this.sendText);
					this.sendText = null;
				}
				else if (this.sendText == null)
				{
					this.sendText = value;
					AttachNode(this.sendText);
				}
				else if (this.sendText != value)
				{
					Model.Settings.SendTextSettings old = this.sendText;
					this.sendText = value;
					ReplaceNode(old, this.sendText);
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
					DetachNode(this.sendFile);
					this.sendFile = null;
				}
				else if (this.sendFile == null)
				{
					this.sendFile = value;
					AttachNode(this.sendFile);
				}
				else if (this.sendFile != value)
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
					DetachNode(this.predefined);
					this.predefined = null;
				}
				else if (this.predefined == null)
				{
					this.predefined = value;
					AttachNode(this.predefined);
				}
				else if (this.predefined != value)
				{
					Model.Settings.PredefinedSettings old = this.predefined;
					this.predefined = value;
					ReplaceNode(old, this.predefined);
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
				if (value == null)
				{
					DetachNode(this.autoResponse);
					this.autoResponse = null;
				}
				else if (this.autoResponse == null)
				{
					this.autoResponse = value;
					AttachNode(this.autoResponse);
				}
				else if (this.autoResponse != value)
				{
					Model.Settings.AutoResponseSettings old = this.autoResponse;
					this.autoResponse = value;
					ReplaceNode(old, this.autoResponse);
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
					DetachNode(this.window);
					this.window = null;
				}
				else if (this.window == null)
				{
					this.window = value;
					AttachNode(this.window);
				}
				else if (this.window != value)
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
					DetachNode(this.layout);
					this.layout = null;
				}
				else if (this.layout == null)
				{
					this.layout = value;
					AttachNode(this.layout);
				}
				else if (this.layout != value)
				{
					Model.Settings.LayoutSettings old = this.layout;
					this.layout = value;
					ReplaceNode(old, this.layout);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("View")]
		public virtual Model.Settings.ViewSettings View
		{
			get { return (this.view); }
			set
			{
				if (value == null)
				{
					DetachNode(this.view);
					this.view = null;
				}
				else if (this.view == null)
				{
					this.view = value;
					AttachNode(this.view);
				}
				else if (this.view != value)
				{
					Model.Settings.ViewSettings old = this.view;
					this.view = value;
					ReplaceNode(old, this.view);
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

			ImplicitSettings other = (ImplicitSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(TerminalIsStarted == other.TerminalIsStarted) &&
				(LogIsOn           == other.LogIsOn)
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
			unchecked
			{
				int hashCode = base.GetHashCode(); // Get hash code of all settings nodes.

				hashCode = (hashCode * 397) ^ TerminalIsStarted.GetHashCode();
				hashCode = (hashCode * 397) ^ LogIsOn          .GetHashCode();

				return (hashCode);
			}
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
