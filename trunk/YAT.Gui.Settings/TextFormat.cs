using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Drawing;

namespace MKY.YAT.Gui.Settings
{
	[Serializable]
	public class TextFormat : IEquatable<TextFormat>
	{
		private Color _color;
		private FontStyle _style;

		public TextFormat()
		{
			_color = Color.Black;
			_style = FontStyle.Regular;
		}

		public TextFormat(Color color, bool bold, bool italic, bool underline, bool strikeout)
		{
			_color = color;

			_style = FontStyle.Regular;
			Bold = bold;
			Italic = italic;
			Underline = underline;
			Strikeout = strikeout;
		}

		public TextFormat(TextFormat rhs)
		{
			_color = rhs._color;
			_style = rhs._style;
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		[XmlIgnore]
		public Color Color
		{
			get { return (_color); }
			set { _color = value; }
		}

		[XmlElement("Color")]
		public int ColorAsArgb
		{
			get { return (_color.ToArgb()); }
			set { _color = Color.FromArgb(value); }
		}

		[XmlElement("FontStyle")]
		public FontStyle Style
		{
			get { return (_style); }
			set { _style = value; }
		}

		[XmlIgnore]
		public bool Bold
		{
			get { return ((_style & FontStyle.Bold) == FontStyle.Bold); }
			set
			{
				if (value)
					_style |= FontStyle.Bold;
				else
					_style &= ~FontStyle.Bold;
			}
		}

		[XmlIgnore]
		public bool Italic
		{
			get { return ((_style & FontStyle.Italic) == FontStyle.Italic); }
			set
			{
				if (value)
					_style |= FontStyle.Italic;
				else
					_style &= ~FontStyle.Italic;
			}
		}

		[XmlIgnore]
		public bool Underline
		{
			get { return ((_style & FontStyle.Underline) == FontStyle.Underline); }
			set
			{
				if (value)
					_style |= FontStyle.Underline;
				else
					_style &= ~FontStyle.Underline;
			}
		}

		[XmlIgnore]
		public bool Strikeout
		{
			get { return ((_style & FontStyle.Strikeout) == FontStyle.Strikeout); }
			set
			{
				if (value)
					_style |= FontStyle.Strikeout;
				else
					_style &= ~FontStyle.Strikeout;
			}
		}

		#endregion

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is TextFormat)
				return (Equals((TextFormat)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(TextFormat value)
		{
			// ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return
					(
					_style.Equals(value._style) &&
					_color.Equals(value._color)
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
		public static bool operator ==(TextFormat lhs, TextFormat rhs)
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
		public static bool operator !=(TextFormat lhs, TextFormat rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}
