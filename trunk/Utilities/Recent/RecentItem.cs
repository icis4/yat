using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MKY.Utilities.Recent
{
	/// <summary>
	/// Item for collections like recent files, encapsulates an item with a time stamp.
	/// </summary>
	[Serializable]
	public class RecentItem<T> : IEquatable<RecentItem<T>>, IComparable
	{
		private T _item;
		private DateTime _timeStamp;

		/// <summary></summary>
		/// <remarks>
		/// Needed for XML serialization.
		/// </remarks>
		public RecentItem()
		{
			_item = default(T);
			_timeStamp = DateTime.Now;
		}

		/// <summary></summary>
		public RecentItem(T item)
		{
			_item = item;
			_timeStamp = DateTime.Now;
		}

		/// <summary></summary>
		public RecentItem(T item, DateTime timeStamp)
		{
			_item = item;
			_timeStamp = timeStamp;
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// The recent item.
		/// </summary>
		/// <remarks>
		/// Set property needed for XML serialization.
		/// </remarks>
		[XmlElement("Item")]
		public T Item
		{
			get { return (_item); }
			set { _item = value;  }
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
			get { return (_timeStamp); }
			set { _timeStamp = value;  }
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
			get { return (_item != null); }
		}

		#endregion

		#region Object Members
		//------------------------------------------------------------------------------------------
		// Object Members
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Returns the recent item.
		/// </summary>
		public override string ToString()
		{
			return (_item.ToString());
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is RecentItem<T>)
				return (Equals((RecentItem<T>)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(RecentItem<T> value)
		{
			// ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return
					(
					_item.Equals(value._item)
					// do not compare time stamp
					);
			}
			return (false);
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return (_item.GetHashCode());
		}

		#endregion

		#region IComparable Members
		//------------------------------------------------------------------------------------------
		// IComparable Members
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Compares this instance to a specified object and returns an indication
		/// of their relative values.
		/// </summary>
		public virtual int CompareTo(object obj)
		{
			if (obj == null) return (1);
			if (obj is RecentItem<T>)
			{
				RecentItem<T> ri = (RecentItem<T>)obj;
				return (-(_timeStamp.CompareTo(ri._timeStamp))); // sort inverse
			}
			throw (new ArgumentException("Object is not a RecentItem"));
		}

		#endregion

		#region Comparison Methods

		/// <summary></summary>
		public static int Compare(object objA, object objB)
		{
			if (ReferenceEquals(objA, objB)) return (0);
			if (objA is RecentItem<T>)
			{
				RecentItem<T> casted = (RecentItem<T>)objA;
				return (casted.CompareTo(objB));
			}
			return (-1);
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(RecentItem<T> lhs, RecentItem<T> rhs)
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
		public static bool operator !=(RecentItem<T> lhs, RecentItem<T> rhs)
		{
			return (!(lhs == rhs));
		}

		/// <summary></summary>
		public static bool operator <(RecentItem<T> lhs, RecentItem<T> rhs)
		{
			return (Compare(lhs, rhs) < 0);
		}

		/// <summary></summary>
		public static bool operator >(RecentItem<T> lhs, RecentItem<T> rhs)
		{
			return (Compare(lhs, rhs) > 0);
		}

		/// <summary></summary>
		public static bool operator <=(RecentItem<T> lhs, RecentItem<T> rhs)
		{
			return (Compare(lhs, rhs) <= 0);
		}

		/// <summary></summary>
		public static bool operator >=(RecentItem<T> lhs, RecentItem<T> rhs)
		{
			return (Compare(lhs, rhs) >= 0);
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator T(RecentItem<T> _item)
		{
			return (_item.Item);
		}

		/// <summary></summary>
		public static implicit operator RecentItem<T>(T _item)
		{
			return (new RecentItem<T>(_item));
		}

		#endregion
	}
}
