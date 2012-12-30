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
// Copyright © 2010-2013 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace MKY.Time
{
	/// <summary>
	/// Value pair.
	/// </summary>
	/// <typeparam name="T">The type of the time stamped item.</typeparam>
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

			TimeStampItem<T> other = (TimeStampItem<T>)obj;
			return
			(
				(TimeStamp.Equals(other.TimeStamp)) &&
				(Item.Equals(other.Item))
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
				TimeStamp.GetHashCode() ^
				Item.GetHashCode()
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

			sb.Append(TimeStamp.ToString(CultureInfo.InvariantCulture));
			sb.Append(" / ");
			sb.Append(Item.ToString());

			return (sb.ToString());
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

			if (ReferenceEquals(lhs, rhs))  return (true);
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
