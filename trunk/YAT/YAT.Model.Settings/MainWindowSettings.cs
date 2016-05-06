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
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace YAT.Model.Settings
{
	/// <summary></summary>
	[Serializable]
	public class MainWindowSettings : MKY.Settings.SettingsItem
	{
		private FormStartPosition startPosition;
		private FormWindowState windowState;
		private Point location;
		private Size size;

		private bool showTerminalInfo;
		private bool showChrono;

		/// <summary></summary>
		public MainWindowSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public MainWindowSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public MainWindowSettings(MainWindowSettings rhs)
			: base(rhs)
		{
			StartPosition = rhs.StartPosition;
			WindowState   = rhs.WindowState;
			Location      = rhs.Location;
			Size          = rhs.Size;

			ShowTerminalInfo = rhs.ShowTerminalInfo;
			ShowChrono       = rhs.ShowChrono;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			StartPosition = FormStartPosition.WindowsDefaultLocation;
			WindowState   = FormWindowState.Normal;
			Location      = new Point(0, 0);
			Size          = new Size(800, 600);

			ShowTerminalInfo = false;
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
					SetChanged();
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
				if (this.location != value)
				{
					this.location = value;
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
				if (this.size != value)
				{
					this.size = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ShowTerminalInfo")]
		public bool ShowTerminalInfo
		{
			get { return (this.showTerminalInfo); }
			set
			{
				if (this.showTerminalInfo != value)
				{
					this.showTerminalInfo = value;
					SetChanged();
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
					SetChanged();
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

			MainWindowSettings other = (MainWindowSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(StartPosition == other.StartPosition) &&
				(WindowState   == other.WindowState) &&
				(Location      == other.Location) &&
				(Size          == other.Size) &&

				(ShowTerminalInfo == other.ShowTerminalInfo) &&
				(ShowChrono       == other.ShowChrono)
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

				hashCode = (hashCode * 397) ^ StartPosition   .GetHashCode();
				hashCode = (hashCode * 397) ^ WindowState     .GetHashCode();
				hashCode = (hashCode * 397) ^ Location        .GetHashCode();
				hashCode = (hashCode * 397) ^ Size            .GetHashCode();

				hashCode = (hashCode * 397) ^ ShowTerminalInfo.GetHashCode();
				hashCode = (hashCode * 397) ^ ShowChrono      .GetHashCode();

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
