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
// MKY Version 1.0.30
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
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
		/// Determines whether the specified address is an IP localhost address.
		/// </summary>
		public static bool IsLocalhost(IPAddress address)
		{
			switch (address.AddressFamily)
			{
				case AddressFamily.InterNetwork:   return (IsIPv4Localhost(address));
				case AddressFamily.InterNetworkV6: return (IsIPv6Localhost(address));

				default: throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "This method requires 'AddressFamily.InterNetwork' or 'AddressFamily.InterNetworkV6'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

	////public static bool IsLocalhost(string ipString) makes no sense, it would not take e.g. IPHostEx "[localhost]" into account.

		/// <summary>
		/// Determines whether the specified address is the IPv4 localhost address.
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Well, 'Pv' is just a part of IPv4...")]
		public static bool IsIPv4Localhost(IPAddress address)
		{                   // IPAddress does not override the ==/!= operators, thanks Microsoft guys...
			return (address.Equals(IPAddress.Loopback));
		}

	////public static bool IsIPv4Localhost(string ipString) makes no sense, it would not take e.g. IPHostEx "IPv4 localhost" into account.

		/// <summary>
		/// Determines whether the specified address is the IPv6 localhost address.
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Well, 'Pv' is just a part of IPv6...")]
		public static bool IsIPv6Localhost(IPAddress address)
		{                   // IPAddress does not override the ==/!= operators, thanks Microsoft guys...
			return (address.Equals(IPAddress.IPv6Loopback));
		}

	////public static bool IsIPv6Localhost(string ipString) makes no sense, it would not take e.g. IPHostEx "IPv6 localhost" into account.

		/// <summary>
		/// Determines whether the specified address is an IP broadcast address.
		/// </summary>
		public static bool IsBroadcast(IPAddress address)
		{                   // IPAddress does not override the ==/!= operators, thanks Microsoft guys...
			return (address.Equals(IPAddress.Broadcast));
		}

	////public static bool IsBroadcast(string ipString) makes no sense, it would not take e.g. IPHostEx "[broadcast]" into account.

		/// <summary>
		/// Determines whether the specified address is an IP multicast address.
		/// </summary>
		public static bool IsMulticast(IPAddress address)
		{
			switch (address.AddressFamily)
			{
				case AddressFamily.InterNetwork:   return (IsIPv4Multicast(address));
				case AddressFamily.InterNetworkV6: return (address.IsIPv6Multicast);

				default: throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "This method requires 'AddressFamily.InterNetwork' or 'AddressFamily.InterNetworkV6'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

	////public static bool IsMulticast(string ipString) makes no sense, it would not take e.g. a potential IPHostEx "[multicast]" into account.

		/// <summary>
		/// Determines whether the specified address is an IPv4 multicast address.
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Well, 'Pv' is just a part of IPv4...")]
		public static bool IsIPv4Multicast(IPAddress address)
		{
			var addressBytes = address.GetAddressBytes();
			var octet1 = addressBytes[0];
			return ((octet1 >= 224) && (octet1 <= 239));
		}

	////public static bool IsIPv4Multicast(string ipString) makes no sense, it would not take e.g. a potential IPHostEx "IPv4 multicast" into account.

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
		/// Determines whether the specified <paramref name="address"/> has the value of <see cref="IPAddress.None"/> or <see cref="IPAddress.IPv6None"/>.
		/// </summary>
		/// <remarks>
		/// Convenience method because <see cref="IPAddress"/> does not override the ==/!= operators, thanks Microsoft guys...
		/// </remarks>
		/// <param name="address">Address to evaluate.</param>
		/// <returns><c>true</c> if value is valid; otherwise, <c>false</c>.</returns>
		public static bool EqualsNone(IPAddress address)
		{
			if (address != null) // IPAddress does not override the ==/!= operators, thanks Microsoft guys...
				return (address.Equals(IPAddress.None) || address.Equals(IPAddress.IPv6None));
			else
				return (false);
		}

		/// <summary>
		/// Determines whether the specified <paramref name="address"/> has a value other than <see cref="IPAddress.None"/> or <see cref="IPAddress.IPv6None"/>.
		/// </summary>
		/// <remarks>
		/// Convenience method because <see cref="IPAddress"/> does not override the ==/!= operators, thanks Microsoft guys...
		/// </remarks>
		/// <param name="address">Address to evaluate.</param>
		/// <returns><c>true</c> if value is valid; otherwise, <c>false</c>.</returns>
		public static bool NotEqualsNone(IPAddress address)
		{
			return (!EqualsNone(address));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
