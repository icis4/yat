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
			: this(MKY.Settings.SettingsType.Implicit)
		{
		}

		/// <summary></summary>
		public ImplicitSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
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
				if (this.sendText != value)
				{
					var oldNode = this.sendText;
					AttachOrReplaceOrDetachNode(oldNode, value);
					this.sendText = value;
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
				if (this.sendFile != value)
				{
					var oldNode = this.sendFile;
					AttachOrReplaceOrDetachNode(oldNode, value);
					this.sendFile = value;
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
				if (this.predefined != value)
				{
					var oldNode = this.predefined;
					AttachOrReplaceOrDetachNode(oldNode, value);
					this.predefined = value;
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
					AttachOrReplaceOrDetachNode(oldNode, value);
					this.autoResponse = value;
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
				if (this.window != value)
				{
					var oldNode = this.window;
					AttachOrReplaceOrDetachNode(oldNode, value);
					this.window = value;
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
				if (this.layout != value)
				{
					var oldNode = this.layout;
					AttachOrReplaceOrDetachNode(oldNode, value);
					this.layout = value;
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
				if (this.view != value)
				{
					var oldNode = this.view;
					AttachOrReplaceOrDetachNode(oldNode, value);
					this.view = value;
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
