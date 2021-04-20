//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT Version 2.4.1
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
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

namespace YAT.Format.Types
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Only used to hold font settings.")]
	[Serializable]
	public class FontFormat : IEquatable<FontFormat>
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <summary></summary>
		public const string DirectoryNameDefault = "DejaVu Font";

		/// <summary></summary>
		public const string FileNameDefault = "DejaVuSansMono.ttf";

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
			this.name  = NameDefault;
			this.size  = SizeDefault;
			this.style = StyleDefault;
			MakeFont();
		}

		/// <summary></summary>
		public FontFormat(string name, float size, FontStyle style)
		{
			this.name  = name;
			this.size  = size;
			this.style = style;
			MakeFont();
		}

		/// <summary></summary>
		public FontFormat(FontFormat rhs)
		{
			this.name  = rhs.name;
			this.size  = rhs.size;
			this.style = rhs.style;
			MakeFont();
		}

		private void MakeFont()
		{
			if (this.font != null)
				this.font.Dispose();

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
				int hashCode;

				hashCode =                   (Name != null ? Name.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ Size               .GetHashCode();
				hashCode = (hashCode * 397) ^ Style              .GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as FontFormat));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(FontFormat other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
			////base.Equals(other) is not required when deriving from 'object'.

				StringEx.EqualsOrdinalIgnoreCase(Name, other.Name) &&
				Size .Equals(other.Size) &&
				Style.Equals(other.Style)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(FontFormat lhs, FontFormat rhs)
		{
			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			object obj = (object)lhs; // Operators are not virtual! Calling object.Equals() ensures
			return (obj.Equals(rhs)); // that a potential virtual <Derived>.Equals() is called.
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
