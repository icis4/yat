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
// YAT 2.0 Beta 4 Candidate 2 Development Version 1.99.29
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
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
	public class WindowSettings : MKY.Settings.SettingsItem
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
		public WindowSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties even though changed flag will be cleared anyway.
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
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

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
			if (ReferenceEquals(obj, null))
				return (false);

			if (GetType() != obj.GetType())
				return (false);

			WindowSettings other = (WindowSettings)obj;
			if (this.state == FormWindowState.Normal)
			{   // Normal
				return
				(
					base.Equals(other) && // Compare all settings nodes.

					(this.state    == other.state) &&
					(this.location == other.location) &&
					(this.size     == other.size)
				);
			}
			else
			{   // Maximized or Minimized
				return
				(
					base.Equals(other) && // Compare all settings nodes.
					(this.state == other.state)
				);
			}
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return
			(
				base.GetHashCode() ^
				this.state.GetHashCode()
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
