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
// YAT 2.0 Beta 4 Candidate 1 Version 1.99.28
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
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
				if (value != this.startPosition)
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
				if (value != this.windowState)
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
				if (value != this.location)
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
				if (value != this.size)
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
				if (value != this.showTerminalInfo)
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
				if (value != this.showChrono)
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

				(this.startPosition == other.startPosition) &&
				(this.windowState   == other.windowState) &&
				(this.location      == other.location) &&
				(this.size          == other.size) &&

				(this.showTerminalInfo == other.showTerminalInfo) &&
				(this.showChrono       == other.showChrono)
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return
			(
				base.GetHashCode() ^

				this.startPosition.GetHashCode() ^
				this.windowState  .GetHashCode() ^
				this.location     .GetHashCode() ^
				this.size         .GetHashCode() ^

				this.showTerminalInfo.GetHashCode() ^
				this.showChrono      .GetHashCode()
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
