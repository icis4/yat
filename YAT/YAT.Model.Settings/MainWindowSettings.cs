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
// YAT Version 2.3.90 Development
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
	public class MainWindowSettings : MKY.Settings.SettingsItem, IEquatable<MainWindowSettings>
	{
		private FormStartPosition startPosition;
		private FormWindowState windowState;
		private Point location;
		private Size size;
		private bool alwaysOnTop;

		private bool showTerminalInfo;
		private bool showTime;
		private bool showChrono;

		/// <summary></summary>
		public MainWindowSettings()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <summary></summary>
		public MainWindowSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public MainWindowSettings(MainWindowSettings rhs)
			: base(rhs)
		{
			StartPosition = rhs.StartPosition;
			WindowState   = rhs.WindowState;
			Location      = rhs.Location;
			Size          = rhs.Size;
			AlwaysOnTop   = rhs.AlwaysOnTop;

			ShowTerminalInfo = rhs.ShowTerminalInfo;
			ShowTime         = rhs.ShowTime;
			ShowChrono       = rhs.ShowChrono;

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			StartPosition = FormStartPosition.WindowsDefaultLocation;
			WindowState   = FormWindowState.Normal;
			Location      = new Point(0, 0);
			Size          = new Size(912, 684); // Equals designed 'Size' of the 'View.Main' form.
			AlwaysOnTop   = false;

			ShowTerminalInfo = false;
			ShowTime         = false;
			ShowChrono       = true;
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
		[XmlElement("WindowState")]
		public FormWindowState WindowState
		{
			get { return (this.windowState); }
			set
			{
				if (this.windowState != value)
				{
					this.windowState = value;
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

		/// <remarks>
		/// Using term 'Info' since the info contains name and IDs.
		/// </remarks>
		[XmlElement("ShowTerminalInfo")]
		public bool ShowTerminalInfo
		{
			get { return (this.showTerminalInfo); }
			set
			{
				if (this.showTerminalInfo != value)
				{
					this.showTerminalInfo = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ShowTime")]
		public bool ShowTime
		{
			get { return (this.showTime); }
			set
			{
				if (this.showTime != value)
				{
					this.showTime = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ShowChrono")]
		public bool ShowChrono
		{
			get { return (this.showChrono); }
			set
			{
				if (this.showChrono != value)
				{
					this.showChrono = value;
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

				hashCode = (hashCode * 397) ^ StartPosition.GetHashCode();
				hashCode = (hashCode * 397) ^ WindowState  .GetHashCode();
				hashCode = (hashCode * 397) ^ Location     .GetHashCode();
				hashCode = (hashCode * 397) ^ Size         .GetHashCode();
				hashCode = (hashCode * 397) ^ AlwaysOnTop  .GetHashCode();

				hashCode = (hashCode * 397) ^ ShowTerminalInfo.GetHashCode();
				hashCode = (hashCode * 397) ^ ShowTime        .GetHashCode();
				hashCode = (hashCode * 397) ^ ShowChrono      .GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as MainWindowSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(MainWindowSettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				StartPosition.Equals(other.StartPosition) &&
				WindowState  .Equals(other.WindowState)   &&
				Location     .Equals(other.Location)      &&
				Size         .Equals(other.Size)          &&
				AlwaysOnTop  .Equals(other.AlwaysOnTop)   &&

				ShowTerminalInfo.Equals(other.ShowTerminalInfo) &&
				ShowTime        .Equals(other.ShowTime)         &&
				ShowChrono      .Equals(other.ShowChrono)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(MainWindowSettings lhs, MainWindowSettings rhs)
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
		public static bool operator !=(MainWindowSettings lhs, MainWindowSettings rhs)
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
