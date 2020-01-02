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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;

using MKY.Diagnostics;

#endregion

namespace MKY.Net
{
	#region Enum IPHost

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum IPHost
	{
		Localhost,

		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IPv4 is a common term, and even used by the .NET framework itself.")]
		IPv4Localhost,

		Broadcast,

		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IPv4 is a common term, and even used by the .NET framework itself.")]
		IPv4Broadcast,

		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IPv6 is a common term, and even used by the .NET framework itself.")]
		IPv6Localhost,

		Explicit
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum IPHostEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public class IPHostEx : EnumEx, IEquatable<IPHostEx>
	{
		#region String Definitions

		/// <remarks>Explicitly using the more common spelling "[localhost]" instead of "[Localhost]".</remarks>
		private const string Localhost_string     = "[localhost]";
		private const string Localhost_stringOld2 = "<localhost>"; // Backward compatibility.
		private const string Localhost_stringOld1 =  "localhost";  // Backward compatibility.

		private const string IPv4Localhost_string = "IPv4 localhost";
		private const string Broadcast_string     = "[broadcast]";
		private const string IPv4Broadcast_string = "IPv4 broadcast";
		private const string IPv6Localhost_string = "IPv6 localhost";

		#endregion

		private string    explicitName; // = null;
		private IPAddress explicitAddress  = IPAddress.None;

		/// <summary>Default is <see cref="IPHost.Localhost"/>.</summary>
		public const IPHost Default = IPHost.Localhost;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public IPHostEx()
			: this(Default)
		{
		}

		/// <remarks>
		/// Do not use with <see cref="IPHost.Explicit"/> because that selection requires
		/// an IP address or host name. Use <see cref="IPHostEx(IPAddress)"/> or
		/// or <see cref="IPHostEx(string, IPAddress)"/> instead.
		/// </remarks>
		public IPHostEx(IPHost host)
			: base(host)
		{
			Debug.Assert((host != IPHost.Explicit), "'IPHost.Explicit' requires an IP address or host name, use 'IPHostEx(IPAddress)' or 'IPHostEx(string, IPAddress)' instead!");
		}

		/// <summary></summary>
		public IPHostEx(IPAddress address)
		{
			if (address == null)
				throw (new ArgumentNullException("address", "An IP address is required!"));

			                 // IPAddress does not override the ==/!= operators, thanks Microsoft guys...
			if      (address.Equals(IPAddress.Loopback))     { SetUnderlyingEnum(IPHost.Localhost);     this.explicitAddress = IPAddress.None; }
			else if (address.Equals(IPAddress.Broadcast))    { SetUnderlyingEnum(IPHost.Broadcast);     this.explicitAddress = IPAddress.None; }
			else if (address.Equals(IPAddress.IPv6Loopback)) { SetUnderlyingEnum(IPHost.IPv6Localhost); this.explicitAddress = IPAddress.None; }
			else                                             { SetUnderlyingEnum(IPHost.Explicit);      this.explicitAddress = address;        }

			// Note that 'IPHost.IPv4Localhost' cannot be distinguished from 'IPHost.Localhost' when 'IPAddress.Loopback' is given.
		}

		/// <summary>
		/// Creates an explicit <see cref="IPHostEx"/> object, using the provided host name and optional address.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public IPHostEx(string name, IPAddress address = null)
		{
			SetUnderlyingEnum(IPHost.Explicit);
			this.explicitName = name;

			if (address != null) // Keep 'IPAddress.None' otherwise.
				this.explicitAddress = address;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public string Name
		{
			get
			{
				switch ((IPHost)UnderlyingEnum)
				{
					case IPHost.Localhost:     return ("localhost");
					case IPHost.IPv4Localhost: return ("localhost");
					case IPHost.Broadcast:     return ("broadcast");
					case IPHost.IPv4Broadcast: return ("broadcast");
					case IPHost.IPv6Localhost: return ("localhost");
					case IPHost.Explicit:
					{
						if (!string.IsNullOrEmpty(this.explicitName))
							return (this.explicitName);
						else
							return (this.explicitAddress.ToString()); // Explicit address is always given, at least 'IPAdress.None'.
					}

					default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		public IPAddress Address
		{
			get
			{
				switch ((IPHost)UnderlyingEnum)
				{
					case IPHost.Localhost:     return (IPAddress.Loopback);
					case IPHost.IPv4Localhost: return (IPAddress.Loopback);
					case IPHost.Broadcast:     return (IPAddress.Broadcast);
					case IPHost.IPv4Broadcast: return (IPAddress.Broadcast);
					case IPHost.IPv6Localhost: return (IPAddress.IPv6Loopback);
					case IPHost.Explicit:      return (this.explicitAddress);

					default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		public bool IsLocalhost
		{
			get
			{
				switch ((IPHost)UnderlyingEnum)
				{
					case IPHost.Localhost:
					case IPHost.IPv4Localhost:
					case IPHost.IPv6Localhost:
						return (true);

					default:
						return (false);
				}
			}
		}

		/// <summary></summary>
		public bool IsBroadcast
		{
			get
			{
				switch ((IPHost)UnderlyingEnum)
				{
					case IPHost.Broadcast:
					case IPHost.IPv4Broadcast:
						return (true);

					default:
						return (false);
				}
			}
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Indication of a fatal bug that shall be reported but cannot be easily handled with 'Debug|Trace.Assert()'.")]
		public override string ToString()
		{
			switch ((IPHost)UnderlyingEnum)
			{
				case IPHost.Localhost:     return (Localhost_string);
				case IPHost.IPv4Localhost: return (IPv4Localhost_string + " (" + IPAddress.Loopback + ")");
				case IPHost.Broadcast:     return (Broadcast_string);
				case IPHost.IPv4Broadcast: return (IPv4Broadcast_string + " (" + IPAddress.Broadcast + ")");
				case IPHost.IPv6Localhost: return (IPv6Localhost_string + " (" + IPAddress.IPv6Loopback + ")");
				case IPHost.Explicit:
				{
					if (!string.IsNullOrEmpty(this.explicitName))
						return (this.explicitName); // Do not add address when explicit name is given, as user may input either name -OR- address.
					else
						return (this.explicitAddress.ToString()); // Explicit address is always given, at least 'IPAdress.None'.
				}

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary>
		/// Returns a compact string representation:
		/// - For predefined hosts, the predefined string is returned.
		/// - For explicit hosts, the host name or address is returned.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Indication of a fatal bug that shall be reported but cannot be easily handled with 'Debug|Trace.Assert()'.")]
		public string ToCompactString()
		{
			switch ((IPHost)UnderlyingEnum)
			{
				case IPHost.Localhost:     return (Localhost_string);
				case IPHost.IPv4Localhost: return (IPv4Localhost_string);
				case IPHost.Broadcast:     return (Broadcast_string);
				case IPHost.IPv4Broadcast: return (IPv4Broadcast_string);
				case IPHost.IPv6Localhost: return (IPv6Localhost_string);
				case IPHost.Explicit:
				{
					if (!string.IsNullOrEmpty(this.explicitName))
						return (this.explicitName); // Do not add address when explicit name is given, as user may input either name -OR- address.
					else
						return (this.explicitAddress.ToString()); // Explicit address is always given, at least 'IPAdress.None'.
				}

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary>
		/// Returns a <see cref="string" /> that e.g. adds [] for IPv6 addresses.
		/// </summary>
		/// <remarks>
		/// It is recommended to use this function instead of <see cref="ToString"/> in cases where
		/// the IPv6 address is immediately followed the port number, separated by a colon.
		/// Compare readability:
		/// "1:2:3:4:5:6:7:8:8080"
		/// "[1:2:3:4:5:6:7:8]:8080"
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Well, 'Pv' is just a part of IPv6...")]
		[SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "What's wrong with a variant of ToString() ?!?")]
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Indication of a fatal bug that shall be reported but cannot be easily handled with 'Debug|Trace.Assert()'.")]
		public string ToEndpointAddressString()
		{
			switch ((IPHost)UnderlyingEnum)
			{
				case IPHost.Localhost:     return (Localhost_string);
				case IPHost.IPv4Localhost: return (IPv4Localhost_string);
				case IPHost.Broadcast:     return (Broadcast_string);
				case IPHost.IPv4Broadcast: return (IPv4Broadcast_string);
				case IPHost.IPv6Localhost: return (IPv6Localhost_string);
				case IPHost.Explicit:
				{
					if (!string.IsNullOrEmpty(this.explicitName))
						return (this.explicitName);
					else
						return (ToEndpointAddressString(this.explicitAddress)); // Explicit address is always given, at least 'IPAdress.None'.
				}

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary>
		/// Returns a <see cref="string" /> that e.g. adds [] for IPv6 addresses.
		/// </summary>
		/// <remarks>
		/// It is recommended to use this function instead of <see cref="ToString"/> in cases where
		/// the IPv6 address is immediately followed the port number, separated by a colon.
		/// Compare readability:
		/// "1:2:3:4:5:6:7:8:8080"
		/// "[1:2:3:4:5:6:7:8]:8080"
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Well, 'Pv' is just a part of IPv6...")]
		[SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "What's wrong with a variant of ToString() ?!?")]
		public static string ToEndpointAddressString(IPAddress address)
		{
			switch (address.AddressFamily)
			{
				case AddressFamily.InterNetworkV6: return ("[" + address.ToString() + "]");
				default:                           return (      address.ToString()      );
			}
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = base.GetHashCode();

				if ((IPHost)UnderlyingEnum == IPHost.Explicit)
				{
					if (!string.IsNullOrEmpty(this.explicitName))
						hashCode = (hashCode * 397) ^ this.explicitName   .GetHashCode();
					else
						hashCode = (hashCode * 397) ^ this.explicitAddress.GetHashCode(); // Explicit address is always given, at least 'IPAdress.None'.
				}

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as IPHostEx));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public bool Equals(IPHostEx other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			if ((IPHost)UnderlyingEnum == IPHost.Explicit)
			{
				if (!string.IsNullOrEmpty(this.explicitName))
				{
					return
					(
						base.Equals(other) &&
						StringEx.EqualsOrdinalIgnoreCase(this.explicitName, other.explicitName)
					////Ignore this.explicitAddress, it shall be resolved when required.
					);
				}
				else
				{
					return
					(
						base.Equals(other) &&
					////this.explicitName is not given.
						IPAddressEx.Equals(this.explicitAddress, other.explicitAddress)
					);
				}
			}
			else
			{
				return (base.Equals(other));
			}
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(IPHostEx lhs, IPHostEx rhs)
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
		public static bool operator !=(IPHostEx lhs, IPHostEx rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion

		#region Items
		//==========================================================================================
		// Items
		//==========================================================================================

		/// <remarks>
		/// The list of items of this extended enum, depending on whether broadcast shall be included or not.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Symmetricity with Enum.GetNames() and Enum.GetValues().")]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static ReadOnlyCollection<IPHostEx> GetItems(bool includeBroadcast = true)
		{
			var items = new List<IPHostEx>(includeBroadcast ? 5 : 3); // Preset the required capacity to improve memory management.

			items.Add(new IPHostEx(IPHost.Localhost));
			items.Add(new IPHostEx(IPHost.IPv4Localhost));

			if (includeBroadcast)
			{
				items.Add(new IPHostEx(IPHost.Broadcast));
				items.Add(new IPHostEx(IPHost.IPv4Broadcast));
			}

			items.Add(new IPHostEx(IPHost.IPv6Localhost));

			// The shall only contain the fixed items, 'Explicit' is not added therefore.

			return (items.AsReadOnly());
		}

		/// <summary>
		/// Determines whether the enumeration covers the specified item.
		/// </summary>
		public static bool HasItem(IPHostEx item)
		{
			return (GetItems().Contains(item));
		}

		#endregion

		#region Parse
		//==========================================================================================
		// Parse
		//==========================================================================================

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static IPHostEx Parse(string s)
		{
			IPHostEx result;

			if (TryParse(s, out result))
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid host string! String must be an IP host name (URL), an IP address, or one of the predefined hosts."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out IPHostEx result)
		{
			IPHost enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				result = new IPHostEx(enumResult);
				return (true);
			}
			else
			{
				if (s != null) // IPAddress.TryParse() does not support 'null', thanks Microsoft guys...
				{
					IPAddress address;
					if (IPAddress.TryParse(s, out address)) // Valid explicit?
					{
						result = new IPHostEx(address);
						return (true);
					}
				}

				if (Uri.IsWellFormedUriString(s, UriKind.RelativeOrAbsolute))
				{
					result = new IPHostEx(s);
					return (true);
				}
				else // Invalid string!
				{
					result = null;
					return (false);
				}
			}
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out IPHost result)
		{
			if (s != null)
				s = s.Trim();

			if      (StringEx.EqualsOrdinalIgnoreCase(s, Localhost_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Localhost_stringOld2) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Localhost_stringOld1))
			{
				result = new IPHostEx(IPHost.Localhost);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, IPv4Localhost_string))
			{
				result = new IPHostEx(IPHost.IPv4Localhost);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Broadcast_string))
			{
				result = new IPHostEx(IPHost.Broadcast);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, IPv4Broadcast_string))
			{
				result = new IPHostEx(IPHost.IPv4Broadcast);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, IPv6Localhost_string))
			{
				result = new IPHostEx(IPHost.IPv6Localhost);
				return (true);
			}
			else // Invalid string!
			{
				result = new IPHostEx(); // Default!
				return (false);
			}
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParseAndResolve(string s, out IPHostEx result)
		{
			if (TryParse(s, out result))
			{
				return (true);
			}
			else
			{
				IPAddress address;
				if (TryResolve(s, out address))
				{
					result = new IPHostEx(s, address);
					return (true);
				}
				else // Invalid string!
				{
					result = null;
					return (false);
				}
			}
		}

		#endregion

		#region Resolve
		//==========================================================================================
		// Resolve
		//==========================================================================================

		/// <summary>
		/// Tries to resolve an IP address from DNS.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		public static bool TryResolve(string s, out IPAddress result)
		{
			if (!string.IsNullOrEmpty(s))
			{
				try
				{
					var addressesFromDns = Dns.GetHostAddresses(s);
					foreach (var addressFromDns in addressesFromDns)
					{
						if (addressFromDns.AddressFamily == AddressFamily.InterNetwork) // IPv4 has precedence for compatibility reasons.
						{
							result = addressFromDns;
							return (true);
						}
					}

					if (addressesFromDns.Length > 0)
					{
						result = addressesFromDns[0];
						return (true);
					}
				}
				catch (Exception ex)
				{
					DebugEx.WriteException(typeof(IPHostEx), ex, "Failed to get address from DNS!");
				}
			}

			result = IPAddress.None;
			return (false);
		}

		/// <summary>
		/// Tries to resolve an IP address from DNS.
		/// </summary>
		public virtual bool TryResolve()
		{
			if ((IPHost)UnderlyingEnum == IPHost.Explicit) // Only non-predefined addresses need to be resolved.
			{
				if (!string.IsNullOrEmpty(this.explicitName))
				{
					IPAddress result;
					if (TryResolve(this.explicitName, out result))
						this.explicitAddress = result;
					else
						return (false);
				}
				else
				{
					return (IPAddressEx.NotEqualsNone(this.explicitAddress)); // Explicit address is always given, at least 'IPAdress.None'.
				}
			}

			return (true);
		}

		/// <summary>
		/// Returns whether IP has been resolved from DNS.
		/// </summary>
		public virtual bool HasBeenResolved
		{
			get
			{
				return (IPAddressEx.NotEqualsNone(this.explicitAddress)); // Explicit address is always given, at least 'IPAdress.None'.
			}
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

		/// <summary></summary>
		public static implicit operator IPHost(IPHostEx host)
		{
			return ((IPHost)host.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator IPHostEx(IPHost host)
		{
			return (new IPHostEx(host));
		}

		/// <summary></summary>
		public static implicit operator IPAddress(IPHostEx address)
		{
			return (address.Address);
		}

		/// <summary></summary>
		public static implicit operator IPHostEx(IPAddress address)
		{
			return (new IPHostEx(address));
		}

		/// <summary></summary>
		public static implicit operator string(IPHostEx host)
		{
			return (host.ToString());
		}

		/// <summary></summary>
		public static implicit operator IPHostEx(string host)
		{
			return (Parse(host));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
