//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Text;
using System.Xml.Serialization;

namespace MKY.Time
{
	/// <summary>
	/// Value pair.
	/// </summary>
	[Serializable]
	public struct TimeStampItem<T>
	{
		private DateTime timeStamp;
		private T item;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TimeStampItem`1"/> struct.
		/// </summary>
		/// <param name="item">The second value.</param>
		public TimeStampItem(T item)
		{
			this.timeStamp = DateTime.Now;
			this.item = item;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TimeStampItem`1"/> struct.
		/// </summary>
		/// <param name="timeStamp">The time stamp.</param>
		/// <param name="item">The second value.</param>
		public TimeStampItem(DateTime timeStamp, T item)
		{
			this.timeStamp = timeStamp;
			this.item = item;
		}

		/// <summary>
		/// Gets or sets the time stamp.
		/// </summary>
		/// <value>The time stamp.</value>
		[XmlElement("TimeStamp")]
		public DateTime TimeStamp
		{
			get { return (this.timeStamp); }
			set { this.timeStamp = value; }
		}

		/// <summary>
		/// Gets or sets the item.
		/// </summary>
		/// <value>The item.</value>
		[XmlElement("Item")]
		public T Item
		{
			get { return (this.item); }
			set { this.item = value; }
		}

		#region Object Members

		/// <summary>
		/// Standard ToString method returning the element contents only.
		/// </summary>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(this.timeStamp.ToString());
			sb.Append(" / ");
			sb.Append(this.item.ToString());

			return (sb.ToString());
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(obj, null))
				return (false);

			if (GetType() != obj.GetType())
				return (false);

			TimeStampItem<T> other = (TimeStampItem<T>)obj;
			return
			(
				(this.timeStamp.Equals(other.timeStamp)) &&
				(this.item.Equals(other.item))
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return
			(
				this.timeStamp.GetHashCode() ^
				this.item.GetHashCode()
			);
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(TimeStampItem<T> lhs, TimeStampItem<T> rhs)
		{
			// Value type implementation of operator ==.
			// See MKY.Test.EqualityTest for details.

			if (ReferenceEquals(lhs, rhs)) return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
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
