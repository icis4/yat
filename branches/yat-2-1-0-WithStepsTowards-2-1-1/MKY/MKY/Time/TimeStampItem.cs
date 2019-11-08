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
using System.Globalization;
using System.Xml.Serialization;

namespace MKY.Time
{
	/// <summary>
	/// Value pair of a time stamp and an item.
	/// </summary>
	/// <typeparam name="T">The type of the time stamped item.</typeparam>
	[Serializable]
	public struct TimeStampItem<T> : IEquatable<TimeStampItem<T>>
	{
		/// <summary>
		/// Gets or sets the time stamp.
		/// </summary>
		/// <value>The time stamp.</value>
		[XmlElement("TimeStamp")]
		public DateTime TimeStamp { get; set; }

		/// <summary>
		/// Gets or sets the item.
		/// </summary>
		/// <value>The item.</value>
		[XmlElement("Item")]
		public T Item { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TimeStampItem`1"/> struct.
		/// </summary>
		/// <param name="item">The second value.</param>
		public TimeStampItem(T item)
			: this(DateTime.Now, item)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TimeStampItem`1"/> struct.
		/// </summary>
		/// <param name="timeStamp">The time stamp.</param>
		/// <param name="item">The second value.</param>
		public TimeStampItem(DateTime timeStamp, T item)
		{
			TimeStamp = timeStamp;
			Item      = item;
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
			string strA = TimeStamp.ToString(CultureInfo.CurrentCulture);
			string strB = Item.ToString();

			return (strA + " / " + strB);
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
				int hashCode = TimeStamp.GetHashCode();

				// Attention, default(T) can lead to null, e.g. in case of a string!
				hashCode = (hashCode * 397) ^ (Item != null ? Item.GetHashCode() : 0);

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is TimeStampItem<T>)
				return (Equals((TimeStampItem<T>)obj));
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
		public bool Equals(TimeStampItem<T> other)
		{
			return
			(
				TimeStamp.Equals(     other.TimeStamp) &&
				ObjectEx.Equals(Item, other.Item)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have value equality.
		/// </summary>
		public static bool operator ==(TimeStampItem<T> lhs, TimeStampItem<T> rhs)
		{
			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have value inequality.
		/// </summary>
		public static bool operator !=(TimeStampItem<T> lhs, TimeStampItem<T> rhs)
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
