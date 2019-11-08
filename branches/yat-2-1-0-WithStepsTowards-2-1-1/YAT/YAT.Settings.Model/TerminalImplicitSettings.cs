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
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Xml.Serialization;

namespace YAT.Settings.Model
{
	/// <summary></summary>
	public class TerminalImplicitSettings : MKY.Settings.SettingsItem, IEquatable<TerminalImplicitSettings>
	{
		/// <summary></summary>
		public const bool TerminalIsStartedDefault = true;

		private bool terminalIsStarted;

		private YAT.Model.Settings.SendTextSettings sendText;
		private YAT.Model.Settings.SendFileSettings sendFile;
		private YAT.Model.Settings.PredefinedSettings predefined;
		private YAT.Model.Settings.WindowSettings window;
		private YAT.Model.Settings.LayoutSettings layout;

		/// <summary></summary>
		public TerminalImplicitSettings()
			: this(MKY.Settings.SettingsType.Implicit)
		{
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public TerminalImplicitSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();

			SendText   = new YAT.Model.Settings.SendTextSettings(SettingsType);
			SendFile   = new YAT.Model.Settings.SendFileSettings(SettingsType);
			Predefined = new YAT.Model.Settings.PredefinedSettings(SettingsType);
			Window     = new YAT.Model.Settings.WindowSettings(SettingsType);
			Layout     = new YAT.Model.Settings.LayoutSettings(SettingsType);

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public TerminalImplicitSettings(TerminalImplicitSettings rhs)
			: base(rhs)
		{
			TerminalIsStarted = rhs.TerminalIsStarted;

			SendText   = new YAT.Model.Settings.SendTextSettings(rhs.SendText);
			SendFile   = new YAT.Model.Settings.SendFileSettings(rhs.SendFile);
			Predefined = new YAT.Model.Settings.PredefinedSettings(rhs.Predefined);
			Window     = new YAT.Model.Settings.WindowSettings(rhs.Window);
			Layout     = new YAT.Model.Settings.LayoutSettings(rhs.Layout);

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			TerminalIsStarted = TerminalIsStartedDefault;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <remarks>
		/// This property is intentionally located in 'implicit' for the following reasons:
		///  > The action of starting/stopping or opening/closing is something temporary. A user may
		///    do this several times during a session. Such changes shall not be indicated.
		///  > A port that has previously been available may not be available right now. Such change
		///    shall neither be indicated.
		///
		/// Note that this setting as well as <see cref="TerminalExplicitSettings.LogIsOn"/> both used to
		/// be 'implicit' up to 1.99.34 and then got moved to 'explicit' for 1.99.50/51/52. But,
		/// this just leads to too many occasions where a user is asked for "Save Terminal?" where
		/// it just doesn't make much sense. So it was decided to revert that change for 1.99.70+.
		/// </remarks>
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
		[XmlElement("SendText")]
		public virtual YAT.Model.Settings.SendTextSettings SendText
		{
			get { return (this.sendText); }
			set
			{
				if (this.sendText != value)
				{
					var oldNode = this.sendText;
					this.sendText = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SendFile")]
		public virtual YAT.Model.Settings.SendFileSettings SendFile
		{
			get { return (this.sendFile); }
			set
			{
				if (this.sendFile != value)
				{
					var oldNode = this.sendFile;
					this.sendFile = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Predefined")]
		public virtual YAT.Model.Settings.PredefinedSettings Predefined
		{
			get { return (this.predefined); }
			set
			{
				if (this.predefined != value)
				{
					var oldNode = this.predefined;
					this.predefined = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Window")]
		public virtual YAT.Model.Settings.WindowSettings Window
		{
			get { return (this.window); }
			set
			{
				if (this.window != value)
				{
					var oldNode = this.window;
					this.window = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Layout")]
		public virtual YAT.Model.Settings.LayoutSettings Layout
		{
			get { return (this.layout); }
			set
			{
				if (this.layout != value)
				{
					var oldNode = this.layout;
					this.layout = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

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

				hashCode = (hashCode * 397) ^ TerminalIsStarted .GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as TerminalImplicitSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(TerminalImplicitSettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				TerminalIsStarted.Equals(other.TerminalIsStarted)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(TerminalImplicitSettings lhs, TerminalImplicitSettings rhs)
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
		public static bool operator !=(TerminalImplicitSettings lhs, TerminalImplicitSettings rhs)
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
