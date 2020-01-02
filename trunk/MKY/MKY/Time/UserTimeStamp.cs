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
// Copyright © 2003-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Xml.Serialization;

namespace MKY.Time
{
	/// <summary></summary>
	[Serializable]
	public struct UserTimeStamp : IEquatable<UserTimeStamp>
	{
		/// <summary></summary>
		[XmlElement("TimeStamp")]
		public DateTime TimeStamp { get; set; }

		/// <summary></summary>
		[XmlElement("UserName")]
		public string UserName { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:UserTimeStamp`1"/> struct.
		/// </summary>
		/// <param name="userName">The user name.</param>
		public UserTimeStamp(string userName)
			: this(DateTime.Now, userName)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:UserTimeStamp`1"/> struct.
		/// </summary>
		/// <param name="timeStamp">The time stamp.</param>
		/// <param name="userName">The user name.</param>
		public UserTimeStamp(DateTime timeStamp, string userName)
		{
			TimeStamp = timeStamp;
			UserName  = userName;
		}

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

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

				hashCode = (hashCode * 397) ^ (UserName != null ? UserName.GetHashCode() : 0);

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is UserTimeStamp)
				return (Equals((UserTimeStamp)obj));
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
		public bool Equals(UserTimeStamp other)
		{
			return
			(
				TimeStamp.Equals(                          other.TimeStamp) &&
				StringEx.EqualsOrdinalIgnoreCase(UserName, other.UserName) // Ignore case of user names.
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have value equality.
		/// </summary>
		public static bool operator ==(UserTimeStamp lhs, UserTimeStamp rhs)
		{
			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have value inequality.
		/// </summary>
		public static bool operator !=(UserTimeStamp lhs, UserTimeStamp rhs)
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
