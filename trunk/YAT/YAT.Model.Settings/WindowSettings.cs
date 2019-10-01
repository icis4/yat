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
// YAT Version 2.1.0
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
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace YAT.Model.Settings
{
	/// <remarks>
	/// \remind (2017-11-19 / MKY), (2019-08-02 / MKY)
	/// Should be migrated to a separate 'YAT.Application.Settings' project. Not easily possible
	/// because of dependencies among 'YAT.*' and 'YAT.Application', e.g. 'ExtensionSettings'.
	/// Requires slight refactoring of project dependencies. Could be done when refactoring the
	/// projects on integration with Albatros.
	/// </remarks>
	public class WindowSettings : MKY.Settings.SettingsItem, IEquatable<WindowSettings>
	{
		private FormWindowState state;
		private Point location;
		private Size size;

		/// <summary></summary>
		public WindowSettings()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <summary></summary>
		public WindowSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public WindowSettings(WindowSettings rhs)
			: base(rhs)
		{
			State    = rhs.State;
			Location = rhs.Location;
			Size     = rhs.Size;

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			State    = FormWindowState.Maximized;
			Location = new Point(0, 0);
			Size     = new Size(900, 675); // Relates to 'Size' of the 'YAT.View.Main' form as
		}                                  // well as the 'YAT.Model.Settings.MainWindowSettings'.

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("State")]
		public FormWindowState State
		{
			get { return (this.state); }
			set
			{
				if (this.state != value)
				{
					this.state = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Location")]
		public Point Location
		{
			get { return (this.location); }
			set
			{
				if (this.location != value)
				{
					this.location = value;
					if (this.state == FormWindowState.Normal)
						SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Size")]
		public Size Size
		{
			get { return (this.size); }
			set
			{
				if (this.size != value)
				{
					this.size = value;
					if (this.state == FormWindowState.Normal)
						SetMyChanged();
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

				hashCode = (hashCode * 397) ^ State.GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as WindowSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(WindowSettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			if (this.state == FormWindowState.Normal)
			{   // Normal
				return
				(
					base.Equals(other) && // Compare all settings nodes.

					State   .Equals(other.State)    &&
					Location.Equals(other.Location) &&
					Size    .Equals(other.Size)
				);
			}
			else
			{   // Maximized or Minimized
				return
				(
					base.Equals(other) && // Compare all settings nodes.

					State.Equals(other.State)
				);
			}
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(WindowSettings lhs, WindowSettings rhs)
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
		public static bool operator !=(WindowSettings lhs, WindowSettings rhs)
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
