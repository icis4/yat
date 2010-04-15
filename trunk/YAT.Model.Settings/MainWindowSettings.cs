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
	public class MainWindowSettings : MKY.Utilities.Settings.Settings, IEquatable<MainWindowSettings>
	{
		private FormStartPosition startPosition;
		private FormWindowState windowState;
		private Point location;
		private Size size;

		/// <summary></summary>
		public MainWindowSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public MainWindowSettings(MKY.Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public MainWindowSettings(MainWindowSettings rhs)
			: base(rhs)
		{
			this.startPosition = rhs.StartPosition;
			this.windowState   = rhs.WindowState;
			this.location      = rhs.Location;
			this.size          = rhs.Size;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			StartPosition = FormStartPosition.WindowsDefaultLocation;
			WindowState = FormWindowState.Normal;
			Location = new Point(0, 0);
			Size = new Size(800, 600);
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

		#endregion

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is MainWindowSettings)
				return (Equals((MainWindowSettings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(MainWindowSettings value)
		{
			// Ensure that object.operator!=() is called.
			if ((object)value != null)
			{
				return
					(
					(this.startPosition == value.startPosition) &&
					(this.windowState   == value.windowState) &&
					(this.location      == value.location) &&
					(this.size          == value.size)
					);
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
		public static bool operator ==(MainWindowSettings lhs, MainWindowSettings rhs)
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
