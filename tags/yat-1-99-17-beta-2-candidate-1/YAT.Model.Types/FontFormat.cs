using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Drawing;

namespace YAT.Model.Types
{
	/// <summary></summary>
	[Serializable]
	public class FontFormat : IEquatable<FontFormat>
	{
		private string _name;
		private float _size;
		private FontStyle _style;
		private Font _font;

		/// <summary></summary>
		public FontFormat()
		{
			_name = "Courier New";
			_size = 8.25f;
			_style = FontStyle.Regular;
			MakeFont();
		}

		/// <summary></summary>
		public FontFormat(string name, float size, FontStyle style)
		{
			_name = name;
			_size = size;
			_style = style;
			MakeFont();
		}

		/// <summary></summary>
		public FontFormat(FontFormat rhs)
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

		/// <summary></summary>
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

		/// <summary></summary>
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

		/// <summary></summary>
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

		/// <summary></summary>
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
			if (obj is FontFormat)
				return (Equals((FontFormat)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(FontFormat value)
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
		public static bool operator ==(FontFormat lhs, FontFormat rhs)
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
		public static bool operator !=(FontFormat lhs, FontFormat rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}
