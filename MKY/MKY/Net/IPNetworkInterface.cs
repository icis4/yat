//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.14
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;

using MKY.Diagnostics;

namespace MKY.Net
{
	#region Enum IPNetworkInterfaceType

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum IPNetworkInterface
	{
		Any,

		Loopback,

		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IPv4 is a common term, and even used by the .NET framework itself.")]
		IPv4Any,

		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IPv4 is a common term, and even used by the .NET framework itself.")]
		IPv4Loopback,

		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IPv6 is a common term, and even used by the .NET framework itself.")]
		IPv6Any,

		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IPv6 is a common term, and even used by the .NET framework itself.")]
		IPv6Loopback,

		Explicit
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum IPNetworkInterface.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	public class IPNetworkInterfaceEx : EnumEx
	{
		#region String Definitions

		private const string Any_string          = "[Any]";
		private const string Any_stringOld2      = "<Any>"; // Backward compatibility.
		private const string Any_stringOld1      =  "any";  // Backward compatibility.

		private const string Loopback_string     = "[Loopback]";
		private const string Loopback_stringOld2 = "<Loopback>"; // Backward compatibility.
		private const string Loopback_stringOld1 =  "loopback";  // Backward compatibility.

		private const string IPv4Any_string      = "IPv4 any";
		private const string IPv4Loopback_string = "IPv4 loopback";
		private const string IPv6Any_string      = "IPv6 any";
		private const string IPv6Loopback_string = "IPv6 loopback";

		#endregion

		private IPAddress explicitAddress     = IPAddress.None;
		private string    explicitDescription = null;

		/// <summary>Default is <see cref="IPNetworkInterface.Any"/>.</summary>
		public const IPNetworkInterface Default = IPNetworkInterface.Any;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public IPNetworkInterfaceEx()
			: this(Default)
		{
		}

		/// <summary></summary>
		public IPNetworkInterfaceEx(IPNetworkInterface networkInterface)
			: base(networkInterface)
		{
			if (networkInterface == IPNetworkInterface.Explicit)
				throw (new InvalidOperationException("'IPNetworkInterface.Explicit' requires an IP address, use IPNetworkInterfaceEx(IPAddress, string) instead!"));
		}

		/// <summary></summary>
		public IPNetworkInterfaceEx(IPAddress address, string description = null)
		{
			if      (address == IPAddress.Any)          { SetUnderlyingEnum(IPNetworkInterface.Any);          this.explicitAddress = IPAddress.None; }
			else if (address == IPAddress.Loopback)     { SetUnderlyingEnum(IPNetworkInterface.Loopback);     this.explicitAddress = IPAddress.None; }
			else if (address == IPAddress.IPv6Any)      { SetUnderlyingEnum(IPNetworkInterface.IPv6Any);      this.explicitAddress = IPAddress.None; }
			else if (address == IPAddress.IPv6Loopback) { SetUnderlyingEnum(IPNetworkInterface.IPv6Loopback); this.explicitAddress = IPAddress.None; }
			else                                        { SetUnderlyingEnum(IPNetworkInterface.Explicit);     this.explicitAddress = address;        }

			this.explicitDescription = description;

			// Note that 'IPHostType.IPv4Localhost' cannot be distinguished from 'IPHostType.Localhost' when 'IPAddress.Loopback' is given.
			// Also note that similar but optimized code is found at FromIPAddress() further below.
		}

		#region Properties

		/// <summary></summary>
		public IPAddress Address
		{
			get
			{
				switch ((IPNetworkInterface)UnderlyingEnum)
				{
					case IPNetworkInterface.Any:          return (IPAddress.Any);
					case IPNetworkInterface.Loopback:     return (IPAddress.Loopback);
					case IPNetworkInterface.IPv4Any:      return (IPAddress.Any);
					case IPNetworkInterface.IPv4Loopback: return (IPAddress.Loopback);
					case IPNetworkInterface.IPv6Any:      return (IPAddress.IPv6Any);
					case IPNetworkInterface.IPv6Loopback: return (IPAddress.IPv6Loopback);
					case IPNetworkInterface.Explicit:     return (this.explicitAddress);
				}
				throw (new NotSupportedException("Program execution should never get here,'" + UnderlyingEnum.ToString() + "' is an unknown item." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		public string Description
		{
			get
			{
				if (!string.IsNullOrEmpty(this.explicitDescription))
					return (this.explicitDescription);
				else if (this.explicitAddress != null)
					return (this.explicitAddress.ToString());
				else
					return ("");
			}
		}

		/// <summary></summary>
		public bool IsLocalHost
		{
			get
			{
				switch ((IPNetworkInterface)UnderlyingEnum)
				{
					case IPNetworkInterface.Loopback:
					case IPNetworkInterface.IPv4Loopback:
					case IPNetworkInterface.IPv6Loopback:
						return (true);

					default:
						return (false);
				}
			}
		}

		#endregion

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(obj, null))
				return (false);

			if (GetType() != obj.GetType())
				return (false);

			IPNetworkInterfaceEx other = (IPNetworkInterfaceEx)obj;
			if ((IPNetworkInterface)UnderlyingEnum == IPNetworkInterface.Explicit)
			{
				return
				(
					base.Equals(other) &&
					(this.explicitAddress == other.explicitAddress)
					// Ignore 'explicitDescription'
				);
			}
			else
			{
				return (base.Equals(other));
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

				if ((IPNetworkInterface)UnderlyingEnum == IPNetworkInterface.Explicit)
				{
					hashCode = (hashCode * 397) ^ (this.explicitAddress != null ? this.explicitAddress.GetHashCode() : 0); // Ignore 'otherDescription'
					// Ignore 'explicitDescription'
				}

				return (hashCode);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "The exception indicates a fatal bug that shall be reported.")]
		public override string ToString()
		{
			switch ((IPNetworkInterface)UnderlyingEnum)
			{
				case IPNetworkInterface.Any:          return (Any_string);
				case IPNetworkInterface.Loopback:     return (Loopback_string);
				case IPNetworkInterface.IPv4Any:      return (IPv4Any_string      + " (" + IPAddress.Any + ")");
				case IPNetworkInterface.IPv4Loopback: return (IPv4Loopback_string + " (" + IPAddress.Loopback + ")");
				case IPNetworkInterface.IPv6Any:      return (IPv6Any_string      + " (" + IPAddress.IPv6Any + ")");
				case IPNetworkInterface.IPv6Loopback: return (IPv6Loopback_string + " (" + IPAddress.IPv6Loopback + ")");
				case IPNetworkInterface.Explicit:
				{
					if (!string.IsNullOrEmpty(this.explicitDescription))
					{
						if (this.explicitAddress != IPAddress.None)
							return (this.explicitDescription + " (" + this.explicitAddress + ")");
						else
							return (this.explicitDescription);
					}
					else
					{
						if (this.explicitAddress != IPAddress.None)
							return (this.explicitAddress.ToString());
						else
							return ("");
					}
				}
			}
			throw (new NotSupportedException("Program execution should never get here,'" + UnderlyingEnum.ToString() + "' is an unknown item." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary>
		/// Returns a compact string representation:
		/// - For predefined interfaces, the predefined string is returned.
		/// - For explicit interfaces, the resolved IP address is returned.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "The exception indicates a fatal bug that shall be reported.")]
		public string ToCompactString()
		{
			switch ((IPNetworkInterface)UnderlyingEnum)
			{
				case IPNetworkInterface.Any:          return (Any_string);
				case IPNetworkInterface.Loopback:     return (Loopback_string);
				case IPNetworkInterface.IPv4Any:      return (IPv4Any_string);
				case IPNetworkInterface.IPv4Loopback: return (IPv4Loopback_string);
				case IPNetworkInterface.IPv6Any:      return (IPv6Any_string);
				case IPNetworkInterface.IPv6Loopback: return (IPv6Loopback_string);
				case IPNetworkInterface.Explicit:
				{
					if (this.explicitAddress != null)
						return (this.explicitAddress.ToString());
					else
						return ("");
				}
			}
			throw (new NotSupportedException("Program execution should never get here,'" + UnderlyingEnum.ToString() + "' is an unknown item." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static IPNetworkInterfaceEx[] GetItems()
		{
			List<IPNetworkInterfaceEx> a = new List<IPNetworkInterfaceEx>(6); // Preset the required capactiy to improve memory management.
			a.Add(new IPNetworkInterfaceEx(IPNetworkInterface.Any));
			a.Add(new IPNetworkInterfaceEx(IPNetworkInterface.Loopback));
			a.Add(new IPNetworkInterfaceEx(IPNetworkInterface.IPv4Any));
			a.Add(new IPNetworkInterfaceEx(IPNetworkInterface.IPv4Loopback));
			a.Add(new IPNetworkInterfaceEx(IPNetworkInterface.IPv6Any));
			a.Add(new IPNetworkInterfaceEx(IPNetworkInterface.IPv6Loopback));
			return (a.ToArray());
		}

		#endregion

		#region Parse/From

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static IPNetworkInterfaceEx Parse(string s)
		{
			IPNetworkInterfaceEx result;

			if (TryParse(s, out result))
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid network interface string! String must be an IP address, or one of the predefined interfaces."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out IPNetworkInterfaceEx result)
		{
			IPNetworkInterface enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				result = enumResult;
				return (true);
			}
			else
			{
				IPAddress address;
				if (IPAddress.TryParse(s, out address)) // Valid explicit?
				{
					result = new IPNetworkInterfaceEx(address, null);
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
		public static bool TryParse(string s, out IPNetworkInterface result)
		{
			if (s != null)
				s = s.Trim();

			if (string.IsNullOrEmpty(s))
			{
				result = new IPNetworkInterfaceEx(); // Default!
				return (true); // Default silently, could e.g. happen when deserializing an XML.
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Any_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Any_stringOld2) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Any_stringOld1))
			{
				result = new IPNetworkInterfaceEx(IPNetworkInterface.Any);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Loopback_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Loopback_stringOld2) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Loopback_stringOld1))
			{
				result = new IPNetworkInterfaceEx(IPNetworkInterface.Loopback);
				return (true);
			}
			else if (s.Contains(IPv4Any_string))
			{
				result = new IPNetworkInterfaceEx(IPNetworkInterface.IPv4Any);
				return (true);
			}
			else if (s.Contains(IPv4Loopback_string))
			{
				result = new IPNetworkInterfaceEx(IPNetworkInterface.IPv4Loopback);
				return (true);
			}
			else if (s.Contains(IPv6Any_string))
			{
				result = new IPNetworkInterfaceEx(IPNetworkInterface.IPv6Any);
				return (true);
			}
			else if (s.Contains(IPv6Loopback_string))
			{
				result = new IPNetworkInterfaceEx(IPNetworkInterface.IPv6Loopback);
				return (true);
			}
			else // Invalid string!
			{
				result = new IPNetworkInterfaceEx(); // Default!
				return (false);
			}
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParseAndResolve(string s, out IPNetworkInterfaceEx result)
		{
			if (TryParse(s, out result))
			{
				return (true);
			}
			else
			{
				try
				{
					IPAddress[] addressesFromDns = Dns.GetHostAddresses(s);
					foreach (IPAddress addressFromDns in addressesFromDns)
					{
						if (addressFromDns.AddressFamily == AddressFamily.InterNetwork) // IPv4 has precedence for compatibility reasons.
						{
							result = new IPNetworkInterfaceEx(addressFromDns);
							return (true);
						}
					}

					if (addressesFromDns.Length > 0)
					{
						result = new IPNetworkInterfaceEx(addressesFromDns[0]);
						return (true);
					}
				}
				catch (Exception ex)
				{
					DebugEx.WriteException(typeof(IPNetworkInterfaceEx), ex, "Failed to get address from DNS!");
				}

				// Invalid string!
				result = null;
				return (false);
			}
		}

		/// <summary></summary>
		public static IPNetworkInterfaceEx FromIPAddress(IPAddress address)
		{
			if      (address == IPAddress.Any)
				return (new IPNetworkInterfaceEx(IPNetworkInterface.Any));
			else if (address == IPAddress.Loopback)
				return (new IPNetworkInterfaceEx(IPNetworkInterface.Loopback));
			else if (address == IPAddress.IPv6Any)
				return (new IPNetworkInterfaceEx(IPNetworkInterface.IPv6Any));
			else if (address == IPAddress.IPv6Loopback)
				return (new IPNetworkInterfaceEx(IPNetworkInterface.IPv6Loopback));
			else
				return (new IPNetworkInterfaceEx(address, ""));

			// Note that 'IPNetworkInterfaceType.IPv4Any|Loopback' cannot be distinguished from 'IPNetworkInterfaceType.Any|Loopback' when 'IPAddress.Any|Loopback' is given.
			// Also note that similar but less optimized code is found at IPNetworkInterfaceType(IPAddress) further above.
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator IPNetworkInterface(IPNetworkInterfaceEx networkInterface)
		{
			return ((IPNetworkInterface)networkInterface.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator IPNetworkInterfaceEx(IPNetworkInterface networkInterface)
		{
			return (new IPNetworkInterfaceEx(networkInterface));
		}

		/// <summary></summary>
		public static implicit operator IPAddress(IPNetworkInterfaceEx address)
		{
			return (address.Address);
		}

		/// <summary></summary>
		public static implicit operator IPNetworkInterfaceEx(IPAddress address)
		{
			return (FromIPAddress(address));
		}

		/// <summary></summary>
		public static implicit operator string(IPNetworkInterfaceEx networkInterface)
		{
			return (networkInterface.ToString());
		}

		/// <summary></summary>
		public static implicit operator IPNetworkInterfaceEx(string networkInterface)
		{
			return (Parse(networkInterface));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
