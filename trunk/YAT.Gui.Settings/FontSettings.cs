using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Drawing;

namespace MKY.YAT.Gui.Settings
{
	[Serializable]
	public class FontSettings : IEquatable<FontSettings>
	{
		private string _name;
		private float _size;
		private FontStyle _style;
		private Font _font;

		public FontSettings()
		{
			_name = "Courier New";
			_size = 8.25f;
			_style = FontStyle.Regular;
			MakeFont();
		}

		public FontSettings(string name, float size, FontStyle style)
		{
			_name = name;
			_size = size;
			_style = style;
			MakeFont();
		}

		public FontSettings(FontSettings rhs)
		{
			_name = rhs._name;
			_size = rhs._size;
			_style = rhs._style;
			MakeFont();
		}

		private void MakeFont()
		{
			_font = new Font(_name, _size, _style);
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		[XmlElement("Name")]
		public string Name
		{
			get { return (_name); }
			set
			{
				_name = value;
				MakeFont();
			}
		}

		[XmlElement("Size")]
		public float Size
		{
			get { return (_size); }
			set
			{
				_size = value;
				MakeFont();
			}
		}

		[XmlElement("Style")]
		public FontStyle Style
		{
			get { return (_style); }
			set
			{
				_style = value;
				MakeFont();
			}
		}

		[XmlIgnore]
		public Font Font
		{
			get { return (_font); }
			set
			{
				_name = value.Name;
				_size = value.Size;
				_style = value.Style;
				MakeFont();
			}
		}

		#endregion

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is FontSettings)
				return (Equals((FontSettings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(FontSettings value)
		{
			// ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return
					(
					_name.Equals(value._name) &&
					_size.Equals(value._size) &&
					_style.Equals(value._style)
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
		public static bool operator ==(FontSettings lhs, FontSettings rhs)
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
		public static bool operator !=(FontSettings lhs, FontSettings rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}
