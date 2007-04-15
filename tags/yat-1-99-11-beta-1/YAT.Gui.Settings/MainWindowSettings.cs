using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Drawing;

namespace HSR.YAT.Gui.Settings
{
	public class MainWindowSettings : Utilities.Settings.Settings
	{
		private FormWindowState _windowState;
		private Point _location;
		private Size _size;

		public MainWindowSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		public MainWindowSettings(Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		public MainWindowSettings(MainWindowSettings rhs)
			: base(rhs)
		{
			WindowState = rhs.WindowState;
			Location    = rhs.Location;
			Size        = rhs.Size;
			ClearChanged();
		}

		protected override void SetMyDefaults()
		{
			WindowState = FormWindowState.Normal;
			Location = new Point(0, 0);
			Size = new Size(720, 540);
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		[XmlElement("WindowState")]
		public FormWindowState WindowState
		{
			get { return (_windowState); }
			set
			{
				if (_windowState != value)
				{
					_windowState = value;
					SetChanged();
				}
			}
		}

		[XmlElement("Location")]
		public Point Location
		{
			get { return (_location); }
			set
			{
				if (_location != value)
				{
					_location = value;
					SetChanged();
				}
			}
		}

		[XmlElement("Size")]
		public Size Size
		{
			get { return (_size); }
			set
			{
				if (_size != value)
				{
					_size = value;
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
			// ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return
					(
					_windowState.Equals(value._windowState) &&
					_location.Equals(value._location) &&
					_size.Equals(value._size)
					);
			}
			return (false);
		}

		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference and value equality.
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
