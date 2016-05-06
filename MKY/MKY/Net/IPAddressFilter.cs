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

namespace MKY.Net
{
	#region Enum IPAddressFilterType

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum IPAddressFilter
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

		Other,
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum IPAddressFilter.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Make sure to use the underlying enum for serialization.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	public class IPAddressFilterEx : EnumEx
	{
		#region String Definitions

		private const string Any_string           = "[Any]";
		private const string Any_stringOld2       = "<Any>"; // Backward compatibility.
		private const string Any_stringOld1       =  "any";  // Backward compatibility.

		private const string Localhost_string     = "[Localhost]";
		private const string Localhost_stringOld2 = "<Localhost>"; // Backward compatibility.
		private const string Localhost_stringOld1 =  "localhost";  // Backward compatibility.

		private const string IPv4Any_string       = "IPv4 any";
		private const string IPv4Localhost_string = "IPv4 localhost";

		private const string IPv6Any_string       = "IPv6 any";
		private const string IPv6Localhost_string = "IPv6 localhost";

		#endregion

		private IPAddress otherAddress = IPAddress.None;

		/// <summary>Default is <see cref="IPAddressFilter.Any"/>.</summary>
		public IPAddressFilterEx()
			: this(IPAddressFilter.Any)
		{
		}

		/// <summary></summary>
		public IPAddressFilterEx(IPAddressFilter addressFilter)
			: base(addressFilter)
		{
		}

		/// <summary></summary>
		public IPAddressFilterEx(IPAddress address)
		{
			if      (address == IPAddress.Any)          { SetUnderlyingEnum(IPAddressFilter.Any);     this.otherAddress = IPAddress.None; }
			else if (address == IPAddress.Loopback)     { SetUnderlyingEnum(IPHost.Localhost);        this.otherAddress = IPAddress.None; }
			else if (address == IPAddress.IPv6Any)      { SetUnderlyingEnum(IPAddressFilter.IPv6Any); this.otherAddress = IPAddress.None; }
			else if (address == IPAddress.IPv6Loopback) { SetUnderlyingEnum(IPHost.IPv6Localhost);    this.otherAddress = IPAddress.None; }
			else                                        { SetUnderlyingEnum(IPAddressFilter.Other);   this.otherAddress = address;        }

			// Note that 'IPAddressFilter.IPv4Any|Localhost' cannot be distinguished from 'IPAddressFilter.IPv4Any|Localhost' when 'IPAddress.Any|Loopback' is given.
			// Also note that similar but optimized code is found at ParseFromIPAddress() further below.
		}

		#region Properties

		/// <summary></summary>
		public IPAddress IPAddress
		{
			get
			{
				switch ((IPAddressFilter)UnderlyingEnum)
				{
					case IPAddressFilter.Any:           return (IPAddress.Any);
					case IPAddressFilter.Localhost:     return (IPAddress.Loopback);
					case IPAddressFilter.IPv4Any:       return (IPAddress.Any);
					case IPAddressFilter.IPv4Localhost: return (IPAddress.Loopback);
					case IPAddressFilter.IPv6Any:       return (IPAddress.IPv6Any);
					case IPAddressFilter.IPv6Localhost: return (IPAddress.IPv6Loopback);
					case IPAddressFilter.Other:         return (this.otherAddress);
				}
				throw (new NotSupportedException("Program execution should never get here,'" + UnderlyingEnum.ToString() + "' is an unknown item." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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

			IPAddressFilterEx other = (IPAddressFilterEx)obj;
			if ((IPAddressFilter)UnderlyingEnum == IPAddressFilter.Other)
			{
				return
				(
					base.Equals(other) &&
					(this.otherAddress == other.otherAddress)
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

				if ((IPAddressFilter)UnderlyingEnum == IPAddressFilter.Other)
					hashCode = (hashCode * 397) ^ (this.otherAddress != null ? this.otherAddress.GetHashCode() : 0);

				return (hashCode);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "The exception indicates a fatal bug that shall be reported.")]
		public override string ToString()
		{
			switch ((IPAddressFilter)UnderlyingEnum)
			{
				case IPAddressFilter.Any:           return (Any_string);
				case IPAddressFilter.Localhost:     return (Localhost_string);
				case IPAddressFilter.IPv4Any:       return (IPv4Any_string       + " (" + IPAddress.Any + ")");
				case IPAddressFilter.IPv4Localhost: return (IPv4Localhost_string + " (" + IPAddress.Loopback + ")");
				case IPAddressFilter.IPv6Any:       return (IPv6Any_string       + " (" + IPAddress.IPv6Any + ")");
				case IPAddressFilter.IPv6Localhost: return (IPv6Localhost_string + " (" + IPAddress.IPv6Loopback + ")");
				case IPAddressFilter.Other:         return (this.otherAddress.ToString());
			}
			throw (new NotSupportedException("Program execution should never get here,'" + UnderlyingEnum.ToString() + "' is an unknown item." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static IPAddressFilterEx[] GetItems()
		{
			List<IPAddressFilterEx> a = new List<IPAddressFilterEx>(6); // Preset the required capactiy to improve memory management.
			a.Add(new IPAddressFilterEx(IPAddressFilter.Any));
			a.Add(new IPAddressFilterEx(IPAddressFilter.Localhost));
			a.Add(new IPAddressFilterEx(IPAddressFilter.IPv4Any));
			a.Add(new IPAddressFilterEx(IPAddressFilter.IPv4Localhost));
			a.Add(new IPAddressFilterEx(IPAddressFilter.IPv6Any));
			a.Add(new IPAddressFilterEx(IPAddressFilter.IPv6Localhost));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static IPAddressFilterEx Parse(string s)
		{
			IPAddressFilterEx result;

			if (TryParse(s, out result))
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is no valid address filter string!"));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out IPAddressFilterEx result)
		{
			IPAddressFilter enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				result = enumResult;
				return (true);
			}
			else
			{
				IPAddress address;
				if (IPAddress.TryParse(s, out address)) // Valid other?
				{
					result = new IPAddressFilterEx(address);
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
		public static bool TryParse(string s, out IPAddressFilter result)
		{
			if (s != null)
				s = s.Trim();

			if (string.IsNullOrEmpty(s))
			{
				result = new IPAddressFilterEx(); // Default!
				return (true); // Default silently, could e.g. happen when deserializing an XML.
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Any_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Any_stringOld1) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Any_stringOld2))
			{
				result = new IPAddressFilterEx(IPAddressFilter.Any);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Localhost_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Localhost_stringOld1) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Localhost_stringOld2))
			{
				result = new IPAddressFilterEx(IPAddressFilter.Localhost);
				return (true);
			}
			else if (s.Contains(IPv4Any_string))
			{
				result = new IPAddressFilterEx(IPAddressFilter.IPv4Any);
				return (true);
			}
			else if (s.Contains(IPv4Localhost_string))
			{
				result = new IPAddressFilterEx(IPAddressFilter.IPv4Localhost);
				return (true);
			}
			else if (s.Contains(IPv6Any_string))
			{
				result = new IPAddressFilterEx(IPAddressFilter.IPv6Any);
				return (true);
			}
			else if (s.Contains(IPv6Localhost_string))
			{
				result = new IPAddressFilterEx(IPAddressFilter.IPv6Localhost);
				return (true);
			}
			else // Invalid string!
			{
				result = new IPAddressFilterEx(); // Default!
				return (false);
			}
		}

		/// <summary></summary>
		public static IPAddressFilter ParseFromIPAddress(IPAddress address)
		{
			if      (address == IPAddress.Any)
				return (new IPAddressFilterEx(IPAddressFilter.Any));
			else if (address == IPAddress.Loopback)
				return (new IPAddressFilterEx(IPAddressFilter.Localhost));
			else if (address == IPAddress.IPv6Any)
				return (new IPAddressFilterEx(IPAddressFilter.IPv6Any));
			else if (address == IPAddress.IPv6Loopback)
				return (new IPAddressFilterEx(IPAddressFilter.IPv6Localhost));
			else
				return (new IPAddressFilterEx(address));

			// Note that 'IPAddressFilterType.IPv4Any|Localhost' cannot be distinguished from 'IPAddressFilterType.Any|Localhost' when 'IPAddress.Any|Loopback' is given.
			// Also note that similar but less optimized code is found at IPAddressFilterType(IPAddress) further above.
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator IPAddressFilter(IPAddressFilterEx addressFilter)
		{
			return ((IPAddressFilter)addressFilter.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator IPAddressFilterEx(IPAddressFilter addressFilter)
		{
			return (new IPAddressFilterEx(addressFilter));
		}

		/// <summary></summary>
		public static implicit operator IPAddress(IPAddressFilterEx address)
		{
			return (address.IPAddress);
		}

		/// <summary></summary>
		public static implicit operator IPAddressFilterEx(IPAddress address)
		{
			return (ParseFromIPAddress(address));
		}

		/// <summary></summary>
		public static implicit operator string(IPAddressFilterEx addressFilter)
		{
			return (addressFilter.ToString());
		}

		/// <summary></summary>
		public static implicit operator IPAddressFilterEx(string addressFilter)
		{
			return (Parse(addressFilter));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
