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
// MKY Version 1.0.17
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Serialization;

namespace MKY.Time
{
	/// <summary>
	/// Value pair of a time stamp and an item.
	/// </summary>
	/// <typeparam name="T">The type of the time stamped item.</typeparam>
	[Serializable]
	public struct TimeTickItem<T>
	{
		private long timeTick;
		private T item;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TimeTickItem`1"/> struct.
		/// </summary>
		/// <param name="item">The second value.</param>
		public TimeTickItem(T item)
			: this(Stopwatch.GetTimestamp(), item)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TimeTickItem`1"/> struct.
		/// </summary>
		/// <param name="timeTick">The time tick.</param>
		/// <param name="item">The second value.</param>
		public TimeTickItem(long timeTick, T item)
		{
			this.timeTick = timeTick;
			this.item = item;
		}

		/// <summary>
		/// Gets or sets the time tick.
		/// </summary>
		/// <value>The time tick.</value>
		[XmlElement("TimeTick")]
		public long TimeTick
		{
			get { return (this.timeTick); }
			set { this.timeTick = value;  }
		}

		/// <summary>
		/// Gets or sets the item.
		/// </summary>
		/// <value>The item.</value>
		[XmlElement("Item")]
		public T Item
		{
			get { return (this.item); }
			set { this.item = value;  }
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
			string strA = TimeTick.ToString(CultureInfo.InvariantCulture);
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
				int hashCode = TimeTick.GetHashCode();

				// Attention, default(T) can lead to null, e.g. in case of a string!
				hashCode = (hashCode * 397) ^ (Item != null ? Item.GetHashCode() : 0);

				return (hashCode);
			}
		}

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

			TimeTickItem<T> other = (TimeTickItem<T>)obj;

			// Attention, default(T) can lead to null, e.g. in case of a string!
			if (Item == null)
				return (other.Item == null);

			return
			(
				(TimeTick == other.TimeTick) &&
				(Item.Equals(other.Item)) // Attention, <Tx> may not overload the ==/!= operators.
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(TimeTickItem<T> lhs, TimeTickItem<T> rhs)
		{
			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(TimeTickItem<T> lhs, TimeTickItem<T> rhs)
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
