//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright � 2003-2004 HSR Hochschule f�r Technik Rapperswil.
// Copyright � 2003-2010 Matthias Kl�y.
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
	public class FontFormat : IEquatable<FontFormat>
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <summary></summary>
		public const string NameDefault = "DejaVu Sans Mono";
		/// <summary></summary>
		public const float SizeDefault = 8.25f;
		/// <summary></summary>
		public const FontStyle StyleDefault = FontStyle.Regular;

		#endregion

		private string name;
		private float size;
		private FontStyle style;
		private Font font;

		/// <summary></summary>
		public FontFormat()
		{
			this.name = NameDefault;
			this.size = SizeDefault;
			this.style = StyleDefault;
			MakeFont();
		}

		/// <summary></summary>
		public FontFormat(string name, float size, FontStyle style)
		{
			this.name = name;
			this.size = size;
			this.style = style;
			MakeFont();
		}

		/// <summary></summary>
		public FontFormat(FontFormat rhs)
		{
			this.name = rhs.name;
			this.size = rhs.size;
			this.style = rhs.style;
			MakeFont();
		}

		private void MakeFont()
		{
			this.font = new Font(this.name, this.size, this.style);
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("Name")]
		public virtual string Name
		{
			get { return (this.name); }
			set
			{
				this.name = value;
				MakeFont();
			}
		}

		/// <summary></summary>
		[XmlElement("Size")]
		public virtual float Size
		{
			get { return (this.size); }
			set
			{
				this.size = value;
				MakeFont();
			}
		}

		/// <summary></summary>
		[XmlElement("Style")]
		public virtual FontStyle Style
		{
			get { return (this.style); }
			set
			{
				this.style = value;
				MakeFont();
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual Font Font
		{
			get { return (this.font); }
			set
			{
				this.name = value.Name;
				this.size = value.Size;
				this.style = value.Style;
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
			// Ensure that object.operator!=() is called.
			if ((object)value != null)
			{
				return
					(
					(this.name  == value.name) &&
					(this.size  == value.size) &&
					(this.style == value.style)
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

//==================================================================================================
// End of
// $URL$
//==================================================================================================
