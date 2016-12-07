//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2'' Version 1.99.52
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Xml.Serialization;

using MKY;
using MKY.Drawing;

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
			this.color     = Color.Black;
			this.fontStyle = FontStyle.Regular;
		}

		/// <summary></summary>
		public TextFormat(Color color, bool bold, bool italic, bool underline, bool strikeout)
		{
			this.color     = color;
			this.fontStyle = FontStyle.Regular;

			Bold      = bold;
			Italic    = italic;
			Underline = underline;
			Strikeout = strikeout;
		}

		/// <summary></summary>
		public TextFormat(TextFormat rhs)
		{
			this.color     = rhs.color;
			this.fontStyle = rhs.fontStyle;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <remarks>
		/// <see cref="Color"/> cannot be serialized, thus, the helper below is used for serialization.
		/// </remarks>
		[XmlIgnore]
		public virtual Color Color
		{
			get { return (this.color); }
			set { this.color = value;  }
		}

		/// <remarks>
		/// Using string because <see cref="Color"/> cannot be serialized.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize the purpose.")]
		[XmlElement("Color")]
		public virtual string Color_ForSerialization
		{
			get { return (ColorTranslator.ToHtml(Color));           }
			set { Color = ColorTranslatorEx.FromHtmlOrWin32(value); } // Also allow Win32 for backward compatibility!
		}

		/// <summary></summary>
		[XmlElement("FontStyle")]
		public virtual FontStyle FontStyle
		{
			get { return (this.fontStyle); }
			set { this.fontStyle = value;  }
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
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to calculate hash code. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = (Color_ForSerialization != null ? Color_ForSerialization.GetHashCode() : 0);

				hashCode = (hashCode * 397) ^ FontStyle.GetHashCode();

				return (hashCode);
			}
		}

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
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				StringEx.EqualsOrdinalIgnoreCase(Color_ForSerialization, other.Color_ForSerialization) &&
				FontStyle.Equals(other.FontStyle)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(TextFormat lhs, TextFormat rhs)
		{
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
