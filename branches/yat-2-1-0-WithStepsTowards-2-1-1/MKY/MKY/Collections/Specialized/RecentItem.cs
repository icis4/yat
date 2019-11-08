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
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace MKY.Collections.Specialized
{
	/// <summary>
	/// Item for collections like recent files, encapsulates an item with a time stamp.
	/// </summary>
	/// <typeparam name="T">The underlying type of the recent item.</typeparam>
	[Serializable]
	public class RecentItem<T> : IEquatable<RecentItem<T>>, IComparable<RecentItem<T>>
	{
		/// <summary>
		/// The recent item.
		/// </summary>
		/// <remarks>
		/// Set is needed for XML serialization.
		/// </remarks>
		[XmlElement("Item")]
		public T Item { get; set; }

		/// <summary>
		/// The time stamp to the recent item.
		/// </summary>
		/// <remarks>
		/// Set is needed for XML serialization.
		/// </remarks>
		[XmlElement("TimeStamp")]
		public DateTime TimeStamp { get; set; }

		/// <remarks>
		/// Needed for XML serialization.
		/// </remarks>
		public RecentItem()
			: this(default(T))
		{
		}

		/// <summary></summary>
		public RecentItem(T item)
			: this(item, DateTime.Now)
		{
		}

		/// <summary></summary>
		public RecentItem(T item, DateTime timeStamp)
		{
			Item = item;
			TimeStamp = timeStamp;
		}

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary>
		/// Checks whether the item is valid.
		/// </summary>
		public virtual bool IsValid
		{
			get { return (Item != null); }
		}

		#endregion

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
			return (Item.ToString());
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
				// Attention, default(T) can lead to null, e.g. in case of a string!
				if (Item != null)
					return (Item.GetHashCode()); // Do not consider time stamp.
				else
					return (0);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as RecentItem<T>));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(RecentItem<T> other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
			////base.Equals(other) is not required when deriving from 'object'.

				ObjectEx.Equals(Item, other.Item) // Do not consider time stamp.
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(RecentItem<T> lhs, RecentItem<T> rhs)
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
		public static bool operator !=(RecentItem<T> lhs, RecentItem<T> rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion

		#region IComparable Members / Comparison Methods and Operators
		//==========================================================================================
		// IComparable Members / Comparison Methods and Operators
		//==========================================================================================

		/// <summary>
		/// Compares this instance to a specified object and returns an indication
		/// of their relative values.
		/// </summary>
		public virtual int CompareTo(RecentItem<T> other)
		{
			return (-(TimeStamp.CompareTo(other.TimeStamp))); // Sort inverse.
		}

		/// <summary></summary>
		/// <typeparam name="MT">The underlying type of the recent item.</typeparam>
		[SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "Any better idea to implement Compare() for generic types?")]
		[SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "T", Justification = "MT is used to clearly show that this type only applies to this method.")]
		public static int Compare<MT>(RecentItem<MT> otherA, RecentItem<MT> otherB)
		{
			if (ReferenceEquals(otherA, otherB))
				return (0);

			if (otherA != null)
				return (otherA.CompareTo(otherB));

			return (ObjectEx.InvalidComparisonResult);
		}

		/// <summary></summary>
		public static bool operator <(RecentItem<T> lhs, RecentItem<T> rhs)
		{
			return (Compare<T>(lhs, rhs) < 0);
		}

		/// <summary></summary>
		public static bool operator >(RecentItem<T> lhs, RecentItem<T> rhs)
		{
			return (Compare<T>(lhs, rhs) > 0);
		}

		/// <summary></summary>
		public static bool operator <=(RecentItem<T> lhs, RecentItem<T> rhs)
		{
			return (Compare<T>(lhs, rhs) <= 0);
		}

		/// <summary></summary>
		public static bool operator >=(RecentItem<T> lhs, RecentItem<T> rhs)
		{
			return (Compare<T>(lhs, rhs) >= 0);
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

		/// <summary></summary>
		public static implicit operator T(RecentItem<T> item)
		{
			return (item.Item);
		}

		/// <summary></summary>
		public static implicit operator RecentItem<T>(T item)
		{
			return (new RecentItem<T>(item));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
