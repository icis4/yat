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
// MKY Version 1.0.28 Development
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
using System.Xml.Serialization;

namespace MKY.Collections
{
	/// <summary>
	/// Serializable string key/value-pair.
	/// </summary>
	/// <remarks>
	/// Required because <see cref="T:KeyValuePair`1"/> doesn't properly serialize
	/// its contents even though it is marked <see cref="SerializableAttribute"/>.
	/// </remarks>
	[Serializable]
	public struct StringKeyValuePair : IEquatable<StringKeyValuePair>
	{
		/// <summary>
		/// Gets or sets the key.
		/// </summary>
		/// <value>The key.</value>
		[XmlElement("Key")]
		public string Key { get; set; }

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		[XmlElement("Value")]
		public string Value { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="StringKeyValuePair"/> struct.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		public StringKeyValuePair(string key, string value)
		{
			Key = key;
			Value = value;
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
			return (Key + " / " + Value);
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

				hashCode =                    (Key   != null ? Key  .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Value != null ? Value.GetHashCode() : 0);

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is StringKeyValuePair)
				return (Equals((StringKeyValuePair)obj));
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
		public bool Equals(StringKeyValuePair other)
		{
			return
			(
				StringEx.EqualsOrdinal(Key,   other.Key) &&
				StringEx.EqualsOrdinal(Value, other.Value)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have value equality.
		/// </summary>
		public static bool operator ==(StringKeyValuePair lhs, StringKeyValuePair rhs)
		{
			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have value inequality.
		/// </summary>
		public static bool operator !=(StringKeyValuePair lhs, StringKeyValuePair rhs)
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
