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
// MKY Development Version 1.0.14
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml.Serialization;

namespace MKY.Collections.Generic
{
	/// <summary>
	/// Value triple.
	/// </summary>
	/// <typeparam name="T1">The first type of the triple.</typeparam>
	/// <typeparam name="T2">The second type of the triple.</typeparam>
	/// <typeparam name="T3">The third type of the triple.</typeparam>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "1", Justification = "T1 relates to Value1.")]
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "2", Justification = "T2 relates to Value2.")]
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "3", Justification = "T3 relates to Value3.")]
	[Serializable]
	public struct Triple<T1, T2, T3>
	{
		private T1 value1;
		private T2 value2;
		private T3 value3;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ValueTriple`1"/> struct.
		/// </summary>
		/// <param name="value1">The first value.</param>
		/// <param name="value2">The second value.</param>
		/// <param name="value3">The third value.</param>
		public Triple(T1 value1, T2 value2, T3 value3)
		{
			this.value1 = value1;
			this.value2 = value2;
			this.value3 = value3;
		}

		/// <summary>
		/// Gets or sets the first value.
		/// </summary>
		/// <value>The first value.</value>
		[XmlElement("Value1")]
		public T1 Value1
		{
			get { return (this.value1); }
			set { this.value1 = value;  }
		}

		/// <summary>
		/// Gets or sets the second value.
		/// </summary>
		/// <value>The second value.</value>
		[XmlElement("Value2")]
		public T2 Value2
		{
			get { return (this.value2); }
			set { this.value2 = value;  }
		}

		/// <summary>
		/// Gets or sets the third value.
		/// </summary>
		/// <value>The third value.</value>
		[XmlElement("Value3")]
		public T3 Value3
		{
			get { return (this.value3); }
			set { this.value3 = value;  }
		}

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(obj, null))
				return (false);

			if (GetType() != obj.GetType())
				return (false);

			Triple<T1, T2, T3> other = (Triple<T1, T2, T3>)obj;
			return
			(
				(Value1.Equals(other.Value1)) &&
				(Value2.Equals(other.Value2)) &&
				(Value3.Equals(other.Value3))
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
				Value1.GetHashCode() ^
				Value2.GetHashCode() ^
				Value3.GetHashCode()
			);
		}

		/// <summary>
		/// Standard ToString method returning the element contents only.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields. This ensures that 'intelligent' properties,
		/// i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(Value1);
			sb.Append(" / ");
			sb.Append(Value2);
			sb.Append(" / ");
			sb.Append(Value3);

			return (sb.ToString());
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(Triple<T1, T2, T3> lhs, Triple<T1, T2, T3> rhs)
		{
			// Value type implementation of operator ==.
			// See MKY.Test.EqualityTest for details.

			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(Triple<T1, T2, T3> lhs, Triple<T1, T2, T3> rhs)
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
