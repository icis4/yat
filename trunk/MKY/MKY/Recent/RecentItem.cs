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
// MKY Development Version 1.0.8
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml.Serialization;

namespace MKY.Recent
{
	/// <summary>
	/// Item for collections like recent files, encapsulates an item with a time stamp.
	/// </summary>
	/// <typeparam name="T">The underlying type of the recent item.</typeparam>
	[Serializable]
	public class RecentItem<T> : IEquatable<RecentItem<T>>, IComparable<RecentItem<T>>
		where T : IEquatable<T>
	{
		private T item;
		private DateTime timeStamp;

		/// <summary></summary>
		/// <remarks>
		/// Needed for XML serialization.
		/// </remarks>
		public RecentItem()
		{
			this.item = default(T);
			this.timeStamp = DateTime.Now;
		}

		/// <summary></summary>
		public RecentItem(T item)
		{
			this.item = item;
			this.timeStamp = DateTime.Now;
		}

		/// <summary></summary>
		public RecentItem(T item, DateTime timeStamp)
		{
			this.item = item;
			this.timeStamp = timeStamp;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary>
		/// The recent item.
		/// </summary>
		/// <remarks>
		/// Set property needed for XML serialization.
		/// </remarks>
		[XmlElement("Item")]
		public T Item
		{
			get { return (this.item); }
			set { this.item = value;  }
		}

		/// <summary>
		/// The time stamp to the recent item.
		/// </summary>
		/// <remarks>
		/// Set property needed for XML serialization.
		/// </remarks>
		[XmlElement("TimeStamp")]
		public DateTime TimeStamp
		{
			get { return (this.timeStamp); }
			set { this.timeStamp = value;  }
		}

		#endregion

		#region Methods
		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Checks whether the item is valid.
		/// </summary>
		public virtual bool IsValid
		{
			get { return (this.item != null); }
		}

		#endregion

		#region Object Members
		//------------------------------------------------------------------------------------------
		// Object Members
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as RecentItem<T>));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(RecentItem<T> other)
		{
			if (ReferenceEquals(other, null))
				return (false);

			if (GetType() != other.GetType())
				return (false);

			// Attention, default(T) can lead to null, e.g. in case of a string!
			if (Item != null)
				return (Item.Equals(other.Item)); // Do not consider time stamp.
			else
				return (false);
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
			// Attention, default(T) can lead to null, e.g. in case of a string!
			if (Item != null)
				return (Item.GetHashCode()); // Do not consider time stamp.
			else
				return (0);
		}

		/// <summary>
		/// Returns the recent item.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields. This ensures that 'intelligent' properties,
		/// i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override string ToString()
		{
			return (Item.ToString());
		}

		#endregion

		#region IComparable<T> Members
		//------------------------------------------------------------------------------------------
		// IComparable<T> Members
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Compares this instance to a specified object and returns an indication
		/// of their relative values.
		/// </summary>
		public virtual int CompareTo(RecentItem<T> other)
		{
			if (other != null)
				return (-(this.timeStamp.CompareTo(other.timeStamp))); // Sort inverse.
			else
				throw (new ArgumentException("Object is not a RecentItem"));
		}

		#endregion

		#region Comparison Methods
		//------------------------------------------------------------------------------------------
		// Comparison Methods
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "Any better idea to implement Compare() for generic types?")]
		public static int Compare<MT>(RecentItem<MT> otherA, RecentItem<MT> otherB)
			where MT : IEquatable<MT>
		{
			if (ReferenceEquals(otherA, otherB))
				return (0);

			if (otherA != null)
				return (otherA.CompareTo(otherB));

			return (-1);
		}

		#endregion

		#region Comparison Operators
		//------------------------------------------------------------------------------------------
		// Comparison Operators
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(RecentItem<T> lhs, RecentItem<T> rhs)
		{
			// Base reference type implementation of operator ==.
			// See MKY.Test.EqualityTest for details.

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
		public static bool operator !=(RecentItem<T> lhs, RecentItem<T> rhs)
		{
			return (!(lhs == rhs));
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
		//------------------------------------------------------------------------------------------
		// Conversion Operators
		//------------------------------------------------------------------------------------------

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
