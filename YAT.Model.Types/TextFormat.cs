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
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Drawing;

namespace YAT.Model.Types
{
	/// <summary></summary>
	[Serializable]
	public class TextFormat : IEquatable<TextFormat>
	{
		private Color _color;
		private FontStyle _fontStyle;

		/// <summary></summary>
		public TextFormat()
		{
			_color = Color.Black;
			_fontStyle = FontStyle.Regular;
		}

		/// <summary></summary>
		public TextFormat(Color color, bool bold, bool italic, bool underline, bool strikeout)
		{
			_color = color;

			_fontStyle = FontStyle.Regular;
			Bold = bold;
			Italic = italic;
			Underline = underline;
			Strikeout = strikeout;
		}

		/// <summary></summary>
		public TextFormat(TextFormat rhs)
		{
			_color = rhs._color;
			_fontStyle = rhs._fontStyle;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlIgnore]
		public virtual Color Color
		{
			get { return (_color); }
			set { _color = value; }
		}

		/// <summary></summary>
		[XmlElement("Color")]
		public virtual int ColorAsArgb
		{
			get { return (_color.ToArgb()); }
			set { _color = Color.FromArgb(value); }
		}

		/// <summary></summary>
		[XmlElement("FontStyle")]
		public virtual FontStyle FontStyle
		{
			get { return (_fontStyle); }
			set { _fontStyle = value; }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool Bold
		{
			get { return ((_fontStyle & FontStyle.Bold) == FontStyle.Bold); }
			set
			{
				if (value)
					_fontStyle |= FontStyle.Bold;
				else
					_fontStyle &= ~FontStyle.Bold;
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool Italic
		{
			get { return ((_fontStyle & FontStyle.Italic) == FontStyle.Italic); }
			set
			{
				if (value)
					_fontStyle |= FontStyle.Italic;
				else
					_fontStyle &= ~FontStyle.Italic;
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool Underline
		{
			get { return ((_fontStyle & FontStyle.Underline) == FontStyle.Underline); }
			set
			{
				if (value)
					_fontStyle |= FontStyle.Underline;
				else
					_fontStyle &= ~FontStyle.Underline;
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool Strikeout
		{
			get { return ((_fontStyle & FontStyle.Strikeout) == FontStyle.Strikeout); }
			set
			{
				if (value)
					_fontStyle |= FontStyle.Strikeout;
				else
					_fontStyle &= ~FontStyle.Strikeout;
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
			// Ensure that object.operator!=() is called.
			if ((object)value != null)
			{
				return
					(
					(_fontStyle == value._fontStyle) &&
					(_color     == value._color)
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

//==================================================================================================
// End of
// $URL$
//==================================================================================================
