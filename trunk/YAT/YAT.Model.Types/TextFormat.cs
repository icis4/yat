//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 2 Version 1.99.30
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2013 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
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
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Argb", Justification = "ARGB is a common term, and even used by the .NET framework itself.")]
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
			return (Equals(obj as TextFormat));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(TextFormat other)
		{
			if (ReferenceEquals(other, null))
				return (false);

			if (GetType() != other.GetType())
				return (false);

			return
			(
				(FontStyle == other.FontStyle) &&
				(Color     == other.Color)
			);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to calculate hash code. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override int GetHashCode()
		{
			return
			(
				FontStyle.GetHashCode() ^
				Color    .GetHashCode()
			);
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(TextFormat lhs, TextFormat rhs)
		{
			// Base reference type implementation of operator ==.
			// See MKY.Test.EqualityTest for details.

			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			// Ensure that potiential <Derived>.Equals() is called.
			// Thus, ensure that object.Equals() is called.
			object obj = (object)lhs;
			return (obj.Equals(rhs));
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
