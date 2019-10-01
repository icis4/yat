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
// MKY Version 1.0.27
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace MKY.Collections.Generic
{
	/// <summary>
	/// Value quadruple.
	/// </summary>
	/// <typeparam name="T1">The first type of the quadruple.</typeparam>
	/// <typeparam name="T2">The second type of the quadruple.</typeparam>
	/// <typeparam name="T3">The third type of the quadruple.</typeparam>
	/// <typeparam name="T4">The forth type of the quadruple.</typeparam>
	[SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes", Justification = "Well, that's what a quadruple is meant to be...")]
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "1", Justification = "T1 relates to Value1.")]
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "2", Justification = "T2 relates to Value2.")]
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "3", Justification = "T3 relates to Value3.")]
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "4", Justification = "T4 relates to Value4.")]
	[Serializable]
	public struct Quadruple<T1, T2, T3, T4> : IEquatable<Quadruple<T1, T2, T3, T4>>
	{
		/// <summary>
		/// Gets or sets the first value.
		/// </summary>
		/// <value>The first value.</value>
		[XmlElement("Value1")]
		public T1 Value1 { get; set; }

		/// <summary>
		/// Gets or sets the second value.
		/// </summary>
		/// <value>The second value.</value>
		[XmlElement("Value2")]
		public T2 Value2 { get; set; }

		/// <summary>
		/// Gets or sets the third value.
		/// </summary>
		/// <value>The third value.</value>
		[XmlElement("Value3")]
		public T3 Value3 { get; set; }

		/// <summary>
		/// Gets or sets the forth value.
		/// </summary>
		/// <value>The forth value.</value>
		[XmlElement("Value4")]
		public T4 Value4 { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ValueQuadruple`1"/> struct.
		/// </summary>
		/// <param name="value1">The first value.</param>
		/// <param name="value2">The second value.</param>
		/// <param name="value3">The third value.</param>
		/// <param name="value4">The forth value.</param>
		public Quadruple(T1 value1, T2 value2, T3 value3, T4 value4)
		{
			Value1 = value1;
			Value2 = value2;
			Value3 = value3;
			Value4 = value4;
		}

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields. This ensures that 'intelligent' properties,
		/// i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override string ToString()
		{
			return (Value1 + " / " + Value2 + " / " + Value3 + " / " + Value4);
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
				int hashCode;

				hashCode =                    Value1.GetHashCode();
				hashCode = (hashCode * 397) ^ Value2.GetHashCode();
				hashCode = (hashCode * 397) ^ Value3.GetHashCode();
				hashCode = (hashCode * 397) ^ Value4.GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is Quadruple<T1, T2, T3, T4>)
				return (Equals((Quadruple<T1, T2, T3, T4>)obj));
			else
				return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(Quadruple<T1, T2, T3, T4> other)
		{
			return
			(
				ObjectEx.Equals(Value1, other.Value1) &&
				ObjectEx.Equals(Value2, other.Value2) &&
				ObjectEx.Equals(Value3, other.Value3) &&
				ObjectEx.Equals(Value4, other.Value4)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have value equality.
		/// </summary>
		public static bool operator ==(Quadruple<T1, T2, T3, T4> lhs, Quadruple<T1, T2, T3, T4> rhs)
		{
			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have value inequality.
		/// </summary>
		public static bool operator !=(Quadruple<T1, T2, T3, T4> lhs, Quadruple<T1, T2, T3, T4> rhs)
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
