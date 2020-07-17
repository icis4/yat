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
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;

namespace MKY.Net
{
	/// <summary>
	/// <see cref="IPAddress"/> utility methods.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class IPAddressEx
	{
		/// <summary>
		/// Returns <see cref="IPAddress.Any"/> if <paramref name="address"/> is <see cref="AddressFamily.InterNetwork"/>.
		/// Returns <see cref="IPAddress.IPv6Any"/> if <paramref name="address"/> is <see cref="AddressFamily.InterNetworkV6"/>.
		/// </summary>
		public static IPAddress GetAnyOfFamily(IPAddress address)
		{
			switch (address.AddressFamily)
			{
				case AddressFamily.InterNetwork:   return (IPAddress.Any);
				case AddressFamily.InterNetworkV6: return (IPAddress.IPv6Any);

				default: throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "This method requires 'AddressFamily.InterNetwork' or 'AddressFamily.InterNetworkV6'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary>
		/// Returns <see cref="IPAddress.None"/> if <paramref name="address"/> is <see cref="AddressFamily.InterNetwork"/>.
		/// Returns <see cref="IPAddress.IPv6None"/> if <paramref name="address"/> is <see cref="AddressFamily.InterNetworkV6"/>.
		/// </summary>
		public static IPAddress GetNoneOfFamily(IPAddress address)
		{
			switch (address.AddressFamily)
			{
				case AddressFamily.InterNetwork:   return (IPAddress.None);
				case AddressFamily.InterNetworkV6: return (IPAddress.IPv6None);

				default: throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "This method requires 'AddressFamily.InterNetwork' or 'AddressFamily.InterNetworkV6'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary>
		/// Returns <see cref="IPAddress.Loopback"/> if <paramref name="address"/> is <see cref="AddressFamily.InterNetwork"/>.
		/// Returns <see cref="IPAddress.IPv6Loopback"/> if <paramref name="address"/> is <see cref="AddressFamily.InterNetworkV6"/>.
		/// </summary>
		public static IPAddress GetLoopbackOfFamily(IPAddress address)
		{
			switch (address.AddressFamily)
			{
				case AddressFamily.InterNetwork:   return (IPAddress.Loopback);
				case AddressFamily.InterNetworkV6: return (IPAddress.IPv6Loopback);

				default: throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "This method requires 'AddressFamily.InterNetwork' or 'AddressFamily.InterNetworkV6'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary>
		/// Determines whether the specified <see cref="IPAddress"/> instances are considered equal.
		/// </summary>
		/// <remarks>
		/// Convenience method because <see cref="IPAddress"/> does not override the ==/!= operators, thanks Microsoft guys...
		///
		/// This method is simply a wrapper to <see cref="object.Equals(object, object)"/>.
		/// It can e.g. be used to implement an overloaded Equals() method.
		///
		/// Note the logic behind <see cref="object.Equals(object, object)"/>:
		///  - If both objects represent the same object reference, it returns true.
		///  - If either or object is <c>null</c>, it returns false.
		///  - Otherwise, it calls objA.Equals(objB) and returns the result.
		/// </remarks>
		/// <param name="addressA">The first <see cref="IPAddress"/> to compare.</param>
		/// <param name="addressB">The second <see cref="IPAddress"/> to compare.</param>
		/// <returns><c>true</c> if the instances are equal; otherwise, <c>false</c>.</returns>
		public static bool Equals(IPAddress addressA, IPAddress addressB)
		{
			return (object.Equals(addressA, addressB));
		}

		/// <summary>
		/// Determines whether the specified <paramref name="address"/> has the value of <see cref="IPAddress.None"/>.
		/// </summary>
		/// <remarks>
		/// Convenience method because <see cref="IPAddress"/> does not override the ==/!= operators, thanks Microsoft guys...
		/// </remarks>
		/// <param name="address">Address to evaluate.</param>
		/// <returns><c>true</c> if value is valid; otherwise, <c>false</c>.</returns>
		public static bool EqualsNone(IPAddress address)
		{
			if (address != null) // IPAddress does not override the ==/!= operators, thanks Microsoft guys...
				return (address.Equals(IPAddress.None));
			else
				return (false);
		}

		/// <summary>
		/// Determines whether the specified <paramref name="address"/> has a value other than <see cref="IPAddress.None"/>.
		/// </summary>
		/// <remarks>
		/// Convenience method because <see cref="IPAddress"/> does not override the ==/!= operators, thanks Microsoft guys...
		/// </remarks>
		/// <param name="address">Address to evaluate.</param>
		/// <returns><c>true</c> if value is valid; otherwise, <c>false</c>.</returns>
		public static bool NotEqualsNone(IPAddress address)
		{
			if (address != null) // IPAddress does not override the ==/!= operators, thanks Microsoft guys...
				return (!address.Equals(IPAddress.None));
			else
				return (false);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
