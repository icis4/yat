using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Drawing;

namespace MKY.YAT.Gui.Settings
{
	[Serializable]
	public class MainWindowSettings : Utilities.Settings.Settings, IEquatable<MainWindowSettings>
	{
		private FormStartPosition _startPosition;
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

		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public MainWindowSettings(MainWindowSettings rhs)
			: base(rhs)
		{
			_startPosition = rhs.StartPosition;
			_windowState   = rhs.WindowState;
			_location      = rhs.Location;
			_size          = rhs.Size;

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
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		[XmlElement("StartPosition")]
		public FormStartPosition StartPosition
		{
			get { return (_startPosition); }
			set
			{
				if (_startPosition != value)
				{
					_startPosition = value;
					SetChanged();
				}
			}
		}

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
					_startPosition.Equals(value._startPosition) &&
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
