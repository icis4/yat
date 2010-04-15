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
using System.Xml.Serialization;

namespace YAT.Model.Types
{
	/// <summary></summary>
	[Serializable]
	public class TextFormat : IEquatable<TextFormat>
	{
		private Color color;
		private FontStyle fontStyle;

		/// <summary></summary>
		public TextFormat()
		{
			this.color = Color.Black;
			this.fontStyle = FontStyle.Regular;
		}

		/// <summary></summary>
		public TextFormat(Color color, bool bold, bool italic, bool underline, bool strikeout)
		{
			this.color = color;

			this.fontStyle = FontStyle.Regular;
			Bold = bold;
			Italic = italic;
			Underline = underline;
			Strikeout = strikeout;
		}

		/// <summary></summary>
		public TextFormat(TextFormat rhs)
		{
			this.color = rhs.color;
			this.fontStyle = rhs.fontStyle;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlIgnore]
		public virtual Color Color
		{
			get { return (this.color); }
			set { this.color = value; }
		}

		/// <summary></summary>
		[XmlElement("Color")]
		public virtual int ColorAsArgb
		{
			get { return (this.color.ToArgb()); }
			set { this.color = Color.FromArgb(value); }
		}

		/// <summary></summary>
		[XmlElement("FontStyle")]
		public virtual FontStyle FontStyle
		{
			get { return (this.fontStyle); }
			set { this.fontStyle = value; }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool Bold
		{
			get { return ((this.fontStyle & FontStyle.Bold) == FontStyle.Bold); }
			set
			{
				if (value)
					this.fontStyle |= FontStyle.Bold;
				else
					this.fontStyle &= ~FontStyle.Bold;
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool Italic
		{
			get { return ((this.fontStyle & FontStyle.Italic) == FontStyle.Italic); }
			set
			{
				if (value)
					this.fontStyle |= FontStyle.Italic;
				else
					this.fontStyle &= ~FontStyle.Italic;
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool Underline
		{
			get { return ((this.fontStyle & FontStyle.Underline) == FontStyle.Underline); }
			set
			{
				if (value)
					this.fontStyle |= FontStyle.Underline;
				else
					this.fontStyle &= ~FontStyle.Underline;
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool Strikeout
		{
			get { return ((this.fontStyle & FontStyle.Strikeout) == FontStyle.Strikeout); }
			set
			{
				if (value)
					this.fontStyle |= FontStyle.Strikeout;
				else
					this.fontStyle &= ~FontStyle.Strikeout;
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
					(this.fontStyle == value.fontStyle) &&
					(this.color     == value.color)
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
