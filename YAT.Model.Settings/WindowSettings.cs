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
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace YAT.Model.Settings
{
	/// <summary></summary>
	[Serializable]
	public class WindowSettings : MKY.Utilities.Settings.Settings, IEquatable<WindowSettings>
	{
		private FormWindowState state;
		private Point location;
		private Size size;

		/// <summary></summary>
		public WindowSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public WindowSettings(MKY.Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public WindowSettings(WindowSettings rhs)
			: base(rhs)
		{
			this.state    = rhs.State;
			this.location = rhs.Location;
			this.size     = rhs.Size;
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			State    = FormWindowState.Maximized;
			Location = new Point(0, 0);
			Size     = new Size(800, 600);
		}

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
				if (value != this.state)
				{
					this.state = value;
					SetChanged();
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
				if (value != this.location)
				{
					this.location = value;
					if (this.state == FormWindowState.Normal)
						SetChanged();
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
				if (value != this.size)
				{
					this.size = value;
					if (this.state == FormWindowState.Normal)
						SetChanged();
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
			if (obj is WindowSettings)
				return (Equals((WindowSettings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(WindowSettings value)
		{
			// Ensure that object.operator!=() is called.
			if ((object)value != null)
			{
				if (this.state == FormWindowState.Normal)
				{   // Normal
					return
						(
						(this.state    == value.state) &&
						(this.location == value.location) &&
						(this.size     == value.size)
						);
				}
				else
				{   // Maximized or Minimized
					return
						(
						(this.state    == value.state)
						);
				}
			}
			return (false);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(WindowSettings lhs, WindowSettings rhs)
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
