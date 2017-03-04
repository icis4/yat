﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 3 Development Version 1.99.53
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Xml.Serialization;

namespace YAT.Settings.Terminal
{
	/// <summary></summary>
	public class ImplicitSettings : MKY.Settings.SettingsItem, IEquatable<ImplicitSettings>
	{
		private Model.Settings.SendTextSettings sendText;
		private Model.Settings.SendFileSettings sendFile;
		private Model.Settings.PredefinedSettings predefined;
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
			SendText     = new Model.Settings.SendTextSettings(rhs.SendText);
			SendFile     = new Model.Settings.SendFileSettings(rhs.SendFile);
			Predefined   = new Model.Settings.PredefinedSettings(rhs.Predefined);
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
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

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
					this.sendText = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
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
					this.sendFile = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
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
					this.predefined = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
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
					this.window = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
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
					this.layout = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
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
					this.view = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

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
				return (base.GetHashCode()); // Get hash code of all settings nodes.
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as ImplicitSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(ImplicitSettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) // Compare all settings nodes.
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(ImplicitSettings lhs, ImplicitSettings rhs)
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
