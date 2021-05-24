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
	#region Enum IPFilter

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum IPFilter
	{
		Any,

		Localhost,

		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IPv4 is a common term, and even used by the .NET framework itself.")]
		IPv4Any,

		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IPv4 is a common term, and even used by the .NET framework itself.")]
		IPv4Localhost,

		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IPv6 is a common term, and even used by the .NET framework itself.")]
		IPv6Any,

		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IPv6 is a common term, and even used by the .NET framework itself.")]
		IPv6Localhost,

		Explicit
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum IPFilterEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public class IPFilterEx : EnumEx, IEquatable<IPFilterEx>
	{
		#region String Definitions

		/// <remarks>Explicitly using "[any]" instead of "[Any]" same as "[localhost]" and "[loopback]".</remarks>
		private const string Any_string           = "[any]";
		private const string Any_stringOld2       = "<any>"; // Backward compatibility.
		private const string Any_stringOld1       =  "any";  // Backward compatibility.

		/// <remarks>Explicitly using the more common spelling "[localhost]" instead of "[Localhost]".</remarks>
		private const string Localhost_string     = "[localhost]";
		private const string Localhost_stringOld2 = "<localhost>"; // Backward compatibility.
		private const string Localhost_stringOld1 =  "localhost";  // Backward compatibility.

		private const string IPv4Any_string       = "IPv4 any";
		private const string IPv4Localhost_string = "IPv4 localhost";

		private const string IPv6Any_string       = "IPv6 any";
		private const string IPv6Localhost_string = "IPv6 localhost";

		#endregion

		/// <remarks>
		/// The list of fixed items of this extended enum.
		/// </remarks>
		private static List<IPFilterEx> staticItems;

		private uint ipv4MaskBytes; // = 0;

		private string    explicitName; // = null;
		private IPAddress explicitAddress  = IPAddress.None;

		/// <summary>Default is <see cref="IPFilter.Any"/>.</summary>
		public const IPFilter Default = IPFilter.Any;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public IPFilterEx()
			: this(Default)
		{
		}

		/// <remarks>
		/// Do not use with <see cref="IPFilter.Explicit"/> because that selection requires
		/// an IP address or host name. Use <see cref="IPFilterEx(IPAddress)"/> or
		/// <see cref="IPFilterEx(string, IPAddress)"/> instead.
		/// </remarks>
		/// <exception cref="ArgumentException">
		/// <paramref name="address"/> is <see cref="IPFilter.Explicit"/>. Use <see cref="IPFilterEx(IPAddress)"/> instead.
		/// </exception>
		public IPFilterEx(IPFilter address)
			: base(address)
		{
			if (address == IPFilter.Explicit)
				throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "'IPFilter.Explicit' requires an IP address or host name, use 'IPFilterEx(IPAddress)' or 'IPFilterEx(string, IPAddress)' instead!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			if      (address == IPFilter.Any)           { this.ipv4MaskBytes = 0xFFFFFFFF; }
			else if (address == IPFilter.Localhost)     { this.ipv4MaskBytes = 0x01FFFF7F; }
			else if (address == IPFilter.IPv4Any)       { this.ipv4MaskBytes = 0xFFFFFFFF; }
			else if (address == IPFilter.IPv4Localhost) { this.ipv4MaskBytes = 0x01FFFF7F; }
			else if (address == IPFilter.IPv6Any)       { this.ipv4MaskBytes = 0xFFFFFFFF; }
			else if (address == IPFilter.IPv6Localhost) { this.ipv4MaskBytes = 0x01FFFF7F; }
		}

		/// <summary></summary>
		public IPFilterEx(IPAddress address)
		{
			if (address == null)
				throw (new ArgumentNullException(nameof(address), "An IP address is required!"));

			                 // IPAddress does not override the ==/!= operators, thanks Microsoft guys...
			if      (address.Equals(IPAddress.Any))          { SetUnderlyingEnum(IPFilter.Any);           this.ipv4MaskBytes = 0xFFFFFFFF;                                                      }
			else if (address.Equals(IPAddress.Loopback))     { SetUnderlyingEnum(IPFilter.Localhost);     this.ipv4MaskBytes = 0x01FFFF7F;                                                      }
			else if (address.Equals(IPAddress.IPv6Any))      { SetUnderlyingEnum(IPFilter.IPv6Any);       this.ipv4MaskBytes = 0xFFFFFFFF;                                                      }
			else if (address.Equals(IPAddress.IPv6Loopback)) { SetUnderlyingEnum(IPFilter.IPv6Localhost); this.ipv4MaskBytes = 0x01FFFF7F;                                                      }
			else                                             { SetUnderlyingEnum(IPFilter.Explicit);      this.ipv4MaskBytes = ConvertToIPv4MaskBytes(address); this.explicitAddress = address; }

			// Note that 'IPAddressFilter.IPv4Any|Localhost' cannot be distinguished from 'IPAddressFilter.IPv4Any|Localhost' when 'IPAddress.Any|Loopback' is given.
		}

		/// <summary>
		/// Creates an explicit <see cref="IPFilterEx"/> object, using the provided host name and optional address.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public IPFilterEx(string name, IPAddress address = null)
		{
			SetUnderlyingEnum(IPFilter.Explicit);
			this.explicitName = name;

			if (address != null) // Keep 'IPAddress.None' otherwise.
			{
				this.ipv4MaskBytes = ConvertToIPv4MaskBytes(address);
				this.explicitAddress = address;
			}
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
				switch ((IPFilter)UnderlyingEnum)
				{
					case IPFilter.Any:           return (IPAddress.Any.ToString());
					case IPFilter.Localhost:     return ("localhost");
					case IPFilter.IPv4Any:       return (IPAddress.Any.ToString());
					case IPFilter.IPv4Localhost: return ("localhost");
					case IPFilter.IPv6Any:       return (IPAddress.IPv6Any.ToString());
					case IPFilter.IPv6Localhost: return ("localhost");
					case IPFilter.Explicit:
					{
						if (!string.IsNullOrEmpty(this.explicitName))
							return (this.explicitName);
						else
							return (this.explicitAddress.ToString()); // Explicit address is always given, at least 'IPAdress.None'.
					}

					default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		public IPAddress Address
		{
			get
			{
				switch ((IPFilter)UnderlyingEnum)
				{
					case IPFilter.Any:           return (IPAddress.Any);
					case IPFilter.Localhost:     return (IPAddress.Loopback);
					case IPFilter.IPv4Any:       return (IPAddress.Any);
					case IPFilter.IPv4Localhost: return (IPAddress.Loopback);
					case IPFilter.IPv6Any:       return (IPAddress.IPv6Any);
					case IPFilter.IPv6Localhost: return (IPAddress.IPv6Loopback);
					case IPFilter.Explicit:      return (this.explicitAddress);

					default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		public bool IsAny
		{
			get
			{
				switch ((IPFilter)UnderlyingEnum)
				{
					case IPFilter.Any:
					case IPFilter.IPv4Any:
					case IPFilter.IPv6Any:
						return (true);

					default:
						return (false);
				}
			}
		}

		/// <summary></summary>
		public bool IsLocalhost
		{
			get
			{
				switch ((IPFilter)UnderlyingEnum)
				{
					case IPFilter.Localhost:
					case IPFilter.IPv4Localhost:
					case IPFilter.IPv6Localhost:
						return (true);

					default:
						return (IPAddressEx.IsLocalhost(Address));
				}
			}
		}

		/// <summary></summary>
		public bool IsExplicit
		{
			get { return ((IPFilter)UnderlyingEnum == IPFilter.Explicit); }
		}

		/// <summary></summary>
		[CLSCompliant(false)]
		public uint IPv4MaskBytes
		{
			get { return (this.ipv4MaskBytes); }
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary>
		/// Converts the given address to filter mask bytes.
		/// </summary>
		[CLSCompliant(false)]
		public virtual uint ConvertToIPv4MaskBytes()
		{
			return (ConvertToIPv4MaskBytes(Address));
		}

		/// <summary>
		/// Converts the given address to filter mask bytes.
		/// </summary>
		[CLSCompliant(false)]
		public static uint ConvertToIPv4MaskBytes(IPAddress address)
		{
			if (address.AddressFamily == AddressFamily.InterNetwork) // IPv4
			{
				var addressBytes = address.GetAddressBytes();

				for (int i = 0; i < addressBytes.Length; i++)
				{
					if (addressBytes[i] == 0)
						addressBytes[i] = 255;
				}

				return (BitConverter.ToUInt32(addressBytes, 0));
			}
			else
			{
				return (0);
			}
		}

		/// <summary>
		/// Determines whether the given address is accepted by the filter.
		/// </summary>
		public virtual bool IsAccepted(IPAddress address)
		{
			switch (address.AddressFamily)
			{
				case AddressFamily.InterNetwork: // IPv4
					return (IsIPv4Accepted(IPv4MaskBytes, address));

				default:
					throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + address.AddressFamily.ToString() + "' is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary>
		/// Determines whether the given address is accepted by the given filter.
		/// </summary>
		public static bool IsAccepted(IPAddress addressOfFilter, IPAddress addressToProbe)
		{
			switch (addressToProbe.AddressFamily)
			{
				case AddressFamily.InterNetwork: // IPv4
					return (IsIPv4Accepted(ConvertToIPv4MaskBytes(addressOfFilter), addressToProbe));

				default:
					throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + addressToProbe.AddressFamily.ToString() + "' is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary>
		/// Determines whether the given address is accepted by the given filter.
		/// </summary>
		[CLSCompliant(false)]
		public static bool IsIPv4Accepted(uint ipv4FilterMaskBytes, IPAddress addressToProbe)
		{
			uint probeBytes = BitConverter.ToUInt32(addressToProbe.GetAddressBytes(), 0);

			// e.g. (192.20.0.1 &   192.255.255.255  ) ==  192.20.0.1
			return ((probeBytes & ipv4FilterMaskBytes) == probeBytes);
		}

		/// <summary>
		/// Determines whether the given address is refused by the filter.
		/// </summary>
		public virtual bool IsRefused(IPAddress address)
		{
			return (!IsAccepted(address));
		}

		/// <summary>
		/// Determines whether the given address is refused by the given filter.
		/// </summary>
		public static bool IsRefused(IPAddress addressOfFilter, IPAddress addressToProbe)
		{
			return (!IsAccepted(addressOfFilter, addressToProbe));
		}

		/// <summary>
		/// Determines whether the given address is refused by the given filter.
		/// </summary>
		[CLSCompliant(false)]
		public static bool IsIPv4Refused(uint ipv4FilterMaskBytes, IPAddress addressToProbe)
		{
			return (!IsIPv4Accepted(ipv4FilterMaskBytes, addressToProbe));
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
			switch ((IPFilter)UnderlyingEnum)
			{
				case IPFilter.Any:           return (Any_string);
				case IPFilter.Localhost:     return (Localhost_string);
				case IPFilter.IPv4Any:       return (IPv4Any_string       + " (" + IPAddress.Any + ")");
				case IPFilter.IPv4Localhost: return (IPv4Localhost_string + " (" + IPAddress.Loopback + ")");
				case IPFilter.IPv6Any:       return (IPv6Any_string       + " (" + IPAddress.IPv6Any + ")");
				case IPFilter.IPv6Localhost: return (IPv6Localhost_string + " (" + IPAddress.IPv6Loopback + ")");
				case IPFilter.Explicit:
				{
					if (!string.IsNullOrEmpty(this.explicitName))
						return (this.explicitName); // Do not add address when explicit name is given, as user may input either name -OR- address.
					else
						return (this.explicitAddress.ToString()); // Explicit address is always given, at least 'IPAdress.None'.
				}

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary>
		/// Returns a compact string representation:
		/// - For predefined filters, the predefined string is returned.
		/// - For explicit filters, the host name or address is returned.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Indication of a fatal bug that shall be reported but cannot be easily handled with 'Debug|Trace.Assert()'.")]
		public string ToCompactString()
		{
			switch ((IPFilter)UnderlyingEnum)
			{
				case IPFilter.Any:           return (Any_string);
				case IPFilter.Localhost:     return (Localhost_string);
				case IPFilter.IPv4Any:       return (IPv4Any_string);
				case IPFilter.IPv4Localhost: return (IPv4Localhost_string);
				case IPFilter.IPv6Any:       return (IPv6Any_string);
				case IPFilter.IPv6Localhost: return (IPv6Localhost_string);
				case IPFilter.Explicit:
				{
					if (!string.IsNullOrEmpty(this.explicitName))
						return (this.explicitName); // Do not add address when explicit name is given, as user may input either name -OR- address.
					else
						return (this.explicitAddress.ToString()); // Explicit address is always given, at least 'IPAdress.None'.
				}

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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
			switch ((IPFilter)UnderlyingEnum)
			{
				case IPFilter.Localhost:     return (Localhost_string);
				case IPFilter.IPv4Localhost: return (IPv4Localhost_string);
				case IPFilter.IPv6Localhost: return (IPv6Localhost_string);
				case IPFilter.Explicit:
				{
					if (!string.IsNullOrEmpty(this.explicitName))
						return (this.explicitName);
					else
						return (ToEndpointAddressString(this.explicitAddress)); // Explicit address is always given, at least 'IPAdress.None'.
				}

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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

				if ((IPFilter)UnderlyingEnum == IPFilter.Explicit)
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
			return (Equals(obj as IPFilterEx));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public bool Equals(IPFilterEx other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			if ((IPFilter)UnderlyingEnum == IPFilter.Explicit)
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
		public static bool operator ==(IPFilterEx lhs, IPFilterEx rhs)
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
		public static bool operator !=(IPFilterEx lhs, IPFilterEx rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion

		#region Items
		//==========================================================================================
		// Items
		//==========================================================================================

		/// <remarks>
		/// The list of fixed items of this extended enum.
		/// <para>
		/// Method instead of property for orthogonality with <see cref="Enum.GetNames(Type)"/> and
		/// <see cref="Enum.GetValues(Type)"/>.
		/// </para></remarks>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "See remarks.")]
		public static ReadOnlyCollection<IPFilterEx> GetItems()
		{
			if (staticItems == null)
			{
				staticItems = new List<IPFilterEx>(6) // Preset the required capacity to improve memory management.
				{
					new IPFilterEx(IPFilter.Any),
					new IPFilterEx(IPFilter.Localhost),
					new IPFilterEx(IPFilter.IPv4Any),
					new IPFilterEx(IPFilter.IPv4Localhost),
					new IPFilterEx(IPFilter.IPv6Any),
					new IPFilterEx(IPFilter.IPv6Localhost)

					// This method shall only return the fixed items, 'Explicit' is therefore not added.
				};
			}

			return (staticItems.AsReadOnly());
		}

		/// <summary>
		/// Determines whether the enumeration contains the specified item.
		/// </summary>
		public static bool Contains(IPFilterEx item)
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
		public static IPFilterEx Parse(string s)
		{
			IPFilterEx result;

			if (TryParse(s, out result))
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid address filter string! String must be an IP host name (URL), an IP address, or one of the predefined filters."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out IPFilterEx result)
		{
			IPFilter enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				result = new IPFilterEx(enumResult);
				return (true);
			}
			else
			{
				if (s != null) // IPAddress.TryParse() does not support 'null', thanks Microsoft guys...
				{
					IPAddress address;
					if (IPAddress.TryParse(s, out address)) // Valid explicit?
					{
						result = new IPFilterEx(s, address);
						return (true);
					}
				}

				if (Uri.IsWellFormedUriString(s, UriKind.RelativeOrAbsolute))
				{
					result = new IPFilterEx(s);
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
		public static bool TryParse(string s, out IPFilter result)
		{
			if (s != null)
				s = s.Trim();

			if      (StringEx.EqualsOrdinalIgnoreCase(s, Any_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Any_stringOld1) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Any_stringOld2))
			{
				result = new IPFilterEx(IPFilter.Any);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Localhost_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Localhost_stringOld1) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Localhost_stringOld2))
			{
				result = new IPFilterEx(IPFilter.Localhost);
				return (true);
			}
			else if (StringEx.StartsWithOrdinalIgnoreCase(s, IPv4Any_string)) // Explicit IPv4, i.e. adress is appended in parentheses.
			{
				result = new IPFilterEx(IPFilter.IPv4Any);
				return (true);
			}
			else if (StringEx.StartsWithOrdinalIgnoreCase(s, IPv4Localhost_string)) // Explicit IPv4, i.e. adress is appended in parentheses.
			{
				result = new IPFilterEx(IPFilter.IPv4Localhost);
				return (true);
			}
			else if (StringEx.StartsWithOrdinalIgnoreCase(s, IPv6Any_string)) // Explicit IPv6, i.e. adress is appended in parentheses.
			{
				result = new IPFilterEx(IPFilter.IPv6Any);
				return (true);
			}
			else if (StringEx.StartsWithOrdinalIgnoreCase(s, IPv6Localhost_string)) // Explicit IPv6, i.e. adress is appended in parentheses.
			{
				result = new IPFilterEx(IPFilter.IPv6Localhost);
				return (true);
			}
			else // Invalid string!
			{
				result = new IPFilterEx(); // Default!
				return (false);
			}
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParseAndResolve(string s, out IPFilterEx result)
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
					result = new IPFilterEx(s, address);
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
					IPAddress[] addressesFromDns = Dns.GetHostAddresses(s);
					foreach (IPAddress addressFromDns in addressesFromDns)
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
					DebugEx.WriteException(typeof(IPFilterEx), ex, "Failed to get address from DNS!");
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
			if ((IPFilter)UnderlyingEnum == IPFilter.Explicit) // Only non-predefined addresses need to be resolved.
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
		public static implicit operator IPFilter(IPFilterEx address)
		{
			return ((IPFilter)address.UnderlyingEnum);
		}

		/// <remarks>
		/// Explicit because cast doesn't work for <see cref="IPFilter.Explicit"/>.
		/// </remarks>
		/// <exception cref="ArgumentException">
		/// <paramref name="address"/> is <see cref="IPFilter.Explicit"/>.
		/// </exception>
		public static explicit operator IPFilterEx(IPFilter address)
		{
			return (new IPFilterEx(address));
		}

		/// <summary></summary>
		public static implicit operator IPAddress(IPFilterEx address)
		{
			return (address.Address);
		}

		/// <summary></summary>
		public static implicit operator IPFilterEx(IPAddress address)
		{
			return (new IPFilterEx(address));
		}

		/// <summary></summary>
		public static implicit operator string(IPFilterEx address)
		{
			return (address.ToString());
		}

		/// <summary></summary>
		public static implicit operator IPFilterEx(string address)
		{
			return (Parse(address));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
