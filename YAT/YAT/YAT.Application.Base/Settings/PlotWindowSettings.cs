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
// YAT Version 2.4.1
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace YAT.Application.Settings
{
	/// <summary></summary>
	public class PlotWindowSettings : WindowSettings, IEquatable<PlotWindowSettings>
	{
		private FormStartPosition startPosition;

		private bool alwaysOnTop;

		/// <summary></summary>
		public PlotWindowSettings()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <summary></summary>
		public PlotWindowSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public PlotWindowSettings(PlotWindowSettings rhs)
			: base(rhs)
		{
			StartPosition = rhs.StartPosition;

			AlwaysOnTop   = rhs.AlwaysOnTop;

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			StartPosition = FormStartPosition.CenterParent; // Window shall appear on 'Main' form.

			State         = FormWindowState.Normal; // Override to intended state of the 'AutoActionPlot' form.
			Size          = new Size(900, 480); // Override to designed 'Size' of the 'AutoActionPlot' form.
			  //// ClientSize = Size(884, 441)
			AlwaysOnTop   = false;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("StartPosition")]
		public FormStartPosition StartPosition
		{
			get { return (this.startPosition); }
			set
			{
				if (this.startPosition != value)
				{
					this.startPosition = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("AlwaysOnTop")]
		public bool AlwaysOnTop
		{
			get { return (this.alwaysOnTop); }
			set
			{
				if (this.alwaysOnTop != value)
				{
					this.alwaysOnTop = value;
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
				int hashCode = base.GetHashCode(); // Get hash code of base including all settings nodes.

				hashCode = (hashCode * 397) ^ StartPosition.GetHashCode();

				hashCode = (hashCode * 397) ^ AlwaysOnTop  .GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as PlotWindowSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(PlotWindowSettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare base including all settings nodes.

				StartPosition.Equals(other.StartPosition) &&

				AlwaysOnTop  .Equals(other.AlwaysOnTop)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(PlotWindowSettings lhs, PlotWindowSettings rhs)
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
		public static bool operator !=(PlotWindowSettings lhs, PlotWindowSettings rhs)
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
