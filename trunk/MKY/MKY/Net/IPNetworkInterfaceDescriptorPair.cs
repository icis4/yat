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
// Copyright © 2007-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Xml.Serialization;

namespace MKY.Net
{
	/// <summary></summary>
	[Serializable]
	public struct IPNetworkInterfaceDescriptorPair : IEquatable<IPNetworkInterfaceDescriptorPair>
	{
		/// <summary></summary>
		[XmlElement("Description")]
		public string Description { get; set; }

		/// <summary>Interval of reconnect in milliseconds.</summary>
		[XmlElement("Address")]
		public string Address { get; set; }

		/// <summary></summary>
		public IPNetworkInterfaceDescriptorPair(string description, string address)
		{
			Description = description;
			Address     = address;
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
			if (!string.IsNullOrEmpty(Description))
			{
				if (!string.IsNullOrEmpty(Address))
					return (Description + " (" + Address + ")");
				else
					return (Description);
			}
			else
			{
				if (!string.IsNullOrEmpty(Address))
					return (Address);
				else
					return ("");
			}
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

				hashCode =                    (Description != null ? Description.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Address     != null ? Address    .GetHashCode() : 0);

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is IPNetworkInterfaceDescriptorPair)
				return (Equals((IPNetworkInterfaceDescriptorPair)obj));
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
		public bool Equals(IPNetworkInterfaceDescriptorPair other)
		{
			return
			(
				StringEx.EqualsOrdinal(Description, other.Description) &&
				StringEx.EqualsOrdinal(Address,     other.Address)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have value equality.
		/// </summary>
		public static bool operator ==(IPNetworkInterfaceDescriptorPair lhs, IPNetworkInterfaceDescriptorPair rhs)
		{
			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have value inequality.
		/// </summary>
		public static bool operator !=(IPNetworkInterfaceDescriptorPair lhs, IPNetworkInterfaceDescriptorPair rhs)
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
