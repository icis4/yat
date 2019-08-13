﻿//==================================================================================================
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
	public class BackFormat : IEquatable<BackFormat>
	{
		private Color color;

		/// <summary></summary>
		public BackFormat()
		{
			this.color = Color.Black;
		}

		/// <summary></summary>
		public BackFormat(Color color)
		{
			this.color = color;
		}

		/// <summary></summary>
		public BackFormat(BackFormat rhs)
		{
			this.color = rhs.color;
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
			get { return (ColorTranslator.ToHtml(Color));  }
			set { Color = ColorTranslator.FromHtml(value); }
		}

		#endregion

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as BackFormat));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public virtual bool Equals(BackFormat other)
		{
			if (ReferenceEquals(other, null))
				return (false);

			if (GetType() != other.GetType())
				return (false);

			return
			(
				StringEx.EqualsOrdinalIgnoreCase(Color_ForSerialization, other.Color_ForSerialization)
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
			unchecked
			{
				return (Color_ForSerialization != null ? Color_ForSerialization.GetHashCode() : 0);
			}
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(BackFormat lhs, BackFormat rhs)
		{
			// Base reference type implementation of operator ==.
			// See MKY.Test.EqualityAnalysis for details.

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
		public static bool operator !=(BackFormat lhs, BackFormat rhs)
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