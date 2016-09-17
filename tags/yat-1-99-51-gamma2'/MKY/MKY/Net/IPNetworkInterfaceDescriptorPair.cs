//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author: stroppel-1$
// $Date: Donnerstag, 5. September 2013 11:02:41$
// $Revision: 2$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.9
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace MKY.Net
{
	/// <summary></summary>
	[Serializable]
	public struct IPNetworkInterfaceDescriptorPair
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
		[XmlElement("Description")]
		public string Description;

		/// <summary>Interval of reconnect in milliseconds.</summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
		[XmlElement("Address")]
		public string Address;

		/// <summary></summary>
		public IPNetworkInterfaceDescriptorPair(string description, string address)
		{
			Description = description;
			Address     = address;
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

			IPNetworkInterfaceDescriptorPair other = (IPNetworkInterfaceDescriptorPair)obj;
			return
			(
				(Description  == other.Description) &&
				(Address      == other.Address)
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
			unchecked
			{
				int hashCode;

				hashCode =                    (Description != null ? Description.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Address     != null ? Address    .GetHashCode() : 0);

				return (hashCode);
			}
		}

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

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(IPNetworkInterfaceDescriptorPair lhs, IPNetworkInterfaceDescriptorPair rhs)
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
