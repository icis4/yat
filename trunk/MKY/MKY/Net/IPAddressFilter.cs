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
	public enum IPAddressFilterType
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
	public class IPAddressFilter : EnumEx
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

		/// <summary>Default is <see cref="IPAddressFilterType.Any"/>.</summary>
		public IPAddressFilter()
			: base(IPAddressFilterType.Any)
		{
		}

		/// <summary></summary>
		public IPAddressFilter(IPAddressFilterType addressFilter)
			: base(addressFilter)
		{
		}

		/// <summary></summary>
		public IPAddressFilter(IPAddress address)
		{
			if      (address == IPAddress.Any)          { SetUnderlyingEnum(IPAddressFilterType.Any);     this.otherAddress = IPAddress.None; }
			else if (address == IPAddress.Loopback)     { SetUnderlyingEnum(IPHostType.Localhost);        this.otherAddress = IPAddress.None; }
			else if (address == IPAddress.IPv6Any)      { SetUnderlyingEnum(IPAddressFilterType.IPv6Any); this.otherAddress = IPAddress.None; }
			else if (address == IPAddress.IPv6Loopback) { SetUnderlyingEnum(IPHostType.IPv6Localhost);    this.otherAddress = IPAddress.None; }
			else                                        { SetUnderlyingEnum(IPAddressFilterType.Other);   this.otherAddress = address;        }

			// Note that 'IPAddressFilter.IPv4Any|Localhost' cannot be distinguished from 'IPAddressFilter.IPv4Any|Localhost' when 'IPAddress.Any|Loopback' is given.
			// Also note that similar but optimized code is found at ParseFromIPAddress() further below.
		}

		#region Properties

		/// <summary></summary>
		public IPAddress IPAddress
		{
			get
			{
				switch ((IPAddressFilterType)UnderlyingEnum)
				{
					case IPAddressFilterType.Any:           return (IPAddress.Any);
					case IPAddressFilterType.Localhost:     return (IPAddress.Loopback);
					case IPAddressFilterType.IPv4Any:       return (IPAddress.Any);
					case IPAddressFilterType.IPv4Localhost: return (IPAddress.Loopback);
					case IPAddressFilterType.IPv6Any:       return (IPAddress.IPv6Any);
					case IPAddressFilterType.IPv6Localhost: return (IPAddress.IPv6Loopback);
					case IPAddressFilterType.Other:         return (this.otherAddress);
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

			IPAddressFilter other = (IPAddressFilter)obj;
			if ((IPAddressFilterType)UnderlyingEnum == IPAddressFilterType.Other)
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
			if ((IPAddressFilterType)UnderlyingEnum == IPAddressFilterType.Other)
			{
				return
				(
					base.GetHashCode() ^
					this.otherAddress.GetHashCode()
				);
			}
			else
			{
				return (base.GetHashCode());
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "The exception indicates a fatal bug that shall be reported.")]
		public override string ToString()
		{
			switch ((IPAddressFilterType)UnderlyingEnum)
			{
				case IPAddressFilterType.Any:           return (Any_string);
				case IPAddressFilterType.Localhost:     return (Localhost_string);
				case IPAddressFilterType.IPv4Any:       return (IPv4Any_string       + " (" + IPAddress.Any + ")");
				case IPAddressFilterType.IPv4Localhost: return (IPv4Localhost_string + " (" + IPAddress.Loopback + ")");
				case IPAddressFilterType.IPv6Any:       return (IPv6Any_string       + " (" + IPAddress.IPv6Any + ")");
				case IPAddressFilterType.IPv6Localhost: return (IPv6Localhost_string + " (" + IPAddress.IPv6Loopback + ")");
				case IPAddressFilterType.Other:         return (this.otherAddress.ToString());
			}
			throw (new NotSupportedException("Program execution should never get here,'" + UnderlyingEnum.ToString() + "' is an unknown item." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static IPAddressFilter[] GetItems()
		{
			List<IPAddressFilter> a = new List<IPAddressFilter>();
			a.Add(new IPAddressFilter(IPAddressFilterType.Any));
			a.Add(new IPAddressFilter(IPAddressFilterType.Localhost));
			a.Add(new IPAddressFilter(IPAddressFilterType.IPv4Any));
			a.Add(new IPAddressFilter(IPAddressFilterType.IPv4Localhost));
			a.Add(new IPAddressFilter(IPAddressFilterType.IPv6Any));
			a.Add(new IPAddressFilter(IPAddressFilterType.IPv6Localhost));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static IPAddressFilter Parse(string s)
		{
			IPAddressFilter result;

			if (TryParse(s, out result))
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is no valid address filter string!"));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out IPAddressFilter result)
		{
			s = s.Trim();

			if      (StringEx.EqualsOrdinalIgnoreCase(s, Any_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Any_stringOld1) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Any_stringOld2))
			{
				result = new IPAddressFilter(IPAddressFilterType.Any);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Localhost_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Localhost_stringOld1) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Localhost_stringOld2))
			{
				result = new IPAddressFilter(IPAddressFilterType.Localhost);
				return (true);
			}
			else if (s.Contains(IPv4Any_string))
			{
				result = new IPAddressFilter(IPAddressFilterType.IPv4Any);
				return (true);
			}
			else if (s.Contains(IPv4Localhost_string))
			{
				result = new IPAddressFilter(IPAddressFilterType.IPv4Localhost);
				return (true);
			}
			else if (s.Contains(IPv6Any_string))
			{
				result = new IPAddressFilter(IPAddressFilterType.IPv6Any);
				return (true);
			}
			else if (s.Contains(IPv6Localhost_string))
			{
				result = new IPAddressFilter(IPAddressFilterType.IPv6Localhost);
				return (true);
			}
			else
			{
				IPAddress address;
				if (IPAddress.TryParse(s, out address)) // IP address!
				{
					result = new IPAddressFilter(address);
					return (true);
				}
				else if (string.IsNullOrEmpty(s)) // Default!
				{
					result = new IPAddressFilter();
					return (true);
				}
				else
				{
					result = null;
					return (false);
				}
			}
		}

		/// <summary></summary>
		public static IPAddressFilterType ParseFromIPAddress(IPAddress address)
		{
			if      (address == IPAddress.Any)
				return (new IPAddressFilter(IPAddressFilterType.Any));
			else if (address == IPAddress.Loopback)
				return (new IPAddressFilter(IPAddressFilterType.Localhost));
			else if (address == IPAddress.IPv6Any)
				return (new IPAddressFilter(IPAddressFilterType.IPv6Any));
			else if (address == IPAddress.IPv6Loopback)
				return (new IPAddressFilter(IPAddressFilterType.IPv6Localhost));
			else
				return (new IPAddressFilter(address));

			// Note that 'IPAddressFilterType.IPv4Any|Localhost' cannot be distinguished from 'IPAddressFilterType.Any|Localhost' when 'IPAddress.Any|Loopback' is given.
			// Also note that similar but less optimized code is found at IPAddressFilterType(IPAddress) further above.
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator IPAddressFilterType(IPAddressFilter addressFilter)
		{
			return ((IPAddressFilterType)addressFilter.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator IPAddressFilter(IPAddressFilterType addressFilter)
		{
			return (new IPAddressFilter(addressFilter));
		}

		/// <summary></summary>
		public static implicit operator IPAddress(IPAddressFilter address)
		{
			return (address.IPAddress);
		}

		/// <summary></summary>
		public static implicit operator IPAddressFilter(IPAddress address)
		{
			return (ParseFromIPAddress(address));
		}

		/// <summary></summary>
		public static implicit operator string(IPAddressFilter addressFilter)
		{
			return (addressFilter.ToString());
		}

		/// <summary></summary>
		public static implicit operator IPAddressFilter(string addressFilter)
		{
			return (Parse(addressFilter));
		}

		#endregion

		#region Comparison Operators

		// Use of base reference type implementation of operators ==/!=.
		// See MKY.Test.EqualityTest for details.

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
