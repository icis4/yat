using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Drawing;

namespace MKY.YAT.Gui.Settings
{
	[Serializable]
	public class WindowSettings : Utilities.Settings.Settings, IEquatable<WindowSettings>
	{
		private FormWindowState _state;
		private Point _location;
		private Size _size;

		public WindowSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		public WindowSettings(Utilities.Settings.SettingsType settingsType)
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

			_state    = rhs.State;
			_location = rhs.Location;
			_size     = rhs.Size;
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
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		[XmlElement("State")]
		public FormWindowState State
		{
			get { return (_state); }
			set
			{
				if (_state != value)
				{
					_state = value;
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
					if (_state == FormWindowState.Normal)
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
					if (_state == FormWindowState.Normal)
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
			// ensure that object.operator!=() is called
			if ((object)value != null)
			{
				if (_state == FormWindowState.Normal)
				{   // Normal
					return
						(
						_state.Equals(value._state) &&
						_location.Equals(value._location) &&
						_size.Equals(value._size)
						);
				}
				else
				{   // Maximized or Minimized
					return
						(
						_state.Equals(value._state)
						);
				}
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
