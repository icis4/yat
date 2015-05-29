//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.11
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
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
	#region Enum IPNetworkInterfaceType

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum IPNetworkInterfaceType
	{
		Any,
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IPv4 is a common term, and even used by the .NET framework itself.")]
		IPv4Any,
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IPv4 is a common term, and even used by the .NET framework itself.")]
		IPv4Loopback,
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IPv6 is a common term, and even used by the .NET framework itself.")]
		IPv6Any,
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IPv6 is a common term, and even used by the .NET framework itself.")]
		IPv6Loopback,
		Other,
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum IPNetworkInterface.
	/// </summary>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[Serializable]
	public class IPNetworkInterface : EnumEx
	{
		#region String Definitions

		private const string Any_string          = "any";
		private const string Any_stringNice      = "<Any>";
		private const string IPv4Any_string      = "IPv4 any";
		private const string IPv4Loopback_string = "IPv4 loopback";
		private const string IPv6Any_string      = "IPv6 any";
		private const string IPv6Loopback_string = "IPv6 loopback";

		#endregion

		private IPAddress otherAddress = IPAddress.None;
		private string otherDescription = "";

		/// <summary>Default is <see cref="IPNetworkInterfaceType.Any"/>.</summary>
		public IPNetworkInterface()
			: base(IPNetworkInterfaceType.Any)
		{
		}

		/// <summary></summary>
		public IPNetworkInterface(IPNetworkInterfaceType networkInterface)
			: base(networkInterface)
		{
		}

		/// <summary></summary>
		public IPNetworkInterface(IPAddress address, string description)
		{
			if      (address == IPAddress.Any)          { SetUnderlyingEnum(IPNetworkInterfaceType.Any);          this.otherAddress = IPAddress.None; }
			else if (address == IPAddress.Any)          { SetUnderlyingEnum(IPNetworkInterfaceType.IPv4Any);      this.otherAddress = IPAddress.None; }
			else if (address == IPAddress.Loopback)     { SetUnderlyingEnum(IPNetworkInterfaceType.IPv4Loopback); this.otherAddress = IPAddress.None; }
			else if (address == IPAddress.IPv6Any)      { SetUnderlyingEnum(IPNetworkInterfaceType.IPv6Any);      this.otherAddress = IPAddress.None; }
			else if (address == IPAddress.IPv6Loopback) { SetUnderlyingEnum(IPNetworkInterfaceType.IPv6Loopback); this.otherAddress = IPAddress.None; }
			else                                        { SetUnderlyingEnum(IPNetworkInterfaceType.Other);        this.otherAddress = address;        }

			this.otherDescription = description;
		}

		#region Properties

		/// <summary></summary>
		public IPAddress IPAddress
		{
			get
			{
				switch ((IPNetworkInterfaceType)UnderlyingEnum)
				{
					case IPNetworkInterfaceType.Any:          return (IPAddress.Any);
					case IPNetworkInterfaceType.IPv4Any:      return (IPAddress.Any);
					case IPNetworkInterfaceType.IPv4Loopback: return (IPAddress.Loopback);
					case IPNetworkInterfaceType.IPv6Any:      return (IPAddress.IPv6Any);
					case IPNetworkInterfaceType.IPv6Loopback: return (IPAddress.IPv6Loopback);
					case IPNetworkInterfaceType.Other:        return (this.otherAddress);
				}
				throw (new InvalidOperationException("Program execution should never get here, item " + UnderlyingEnum.ToString() + " is unknown, please report this bug!"));
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

			IPNetworkInterface other = (IPNetworkInterface)obj;
			if ((IPNetworkInterfaceType)UnderlyingEnum == IPNetworkInterfaceType.Other)
			{
				return
				(
					base.Equals(other) &&

					(this.otherAddress == other.otherAddress) &&
					StringEx.EqualsOrdinalIgnoreCase(this.otherDescription, other.otherDescription)
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
			if ((IPNetworkInterfaceType)UnderlyingEnum == IPNetworkInterfaceType.Other)
			{
				return
				(
					base.GetHashCode() ^

					this.otherAddress    .GetHashCode() ^
					this.otherDescription.GetHashCode()
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
			switch ((IPNetworkInterfaceType)UnderlyingEnum)
			{
				case IPNetworkInterfaceType.Any:          return (Any_stringNice);
				case IPNetworkInterfaceType.IPv4Any:      return (IPv4Any_string      + " (" + IPAddress.Any + ")");
				case IPNetworkInterfaceType.IPv4Loopback: return (IPv4Loopback_string + " (" + IPAddress.Loopback + ")");
				case IPNetworkInterfaceType.IPv6Any:      return (IPv6Any_string      + " (" + IPAddress.IPv6Any + ")");
				case IPNetworkInterfaceType.IPv6Loopback: return (IPv6Loopback_string + " (" + IPAddress.IPv6Loopback + ")");
				case IPNetworkInterfaceType.Other:
				{
					if (this.otherDescription.Length > 0)
					{
						if (this.otherAddress != IPAddress.None)
							return (this.otherDescription + " (" + this.otherAddress + ")");
						else
							return (this.otherDescription);
					}
					else
					{
						if (this.otherAddress != IPAddress.None)
							return (this.otherAddress.ToString());
						else
							throw (new InvalidOperationException("IP address and interface description or both are undefined!"));
					}
				}

				default:
				{
					throw (new InvalidOperationException("Program execution should never get here, item " + UnderlyingEnum.ToString() + " is unknown, please report this bug!"));
				}
			}
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static IPNetworkInterface[] GetItems()
		{
			List<IPNetworkInterface> a = new List<IPNetworkInterface>();
			a.Add(new IPNetworkInterface(IPNetworkInterfaceType.Any));
			a.Add(new IPNetworkInterface(IPNetworkInterfaceType.IPv4Any));
			a.Add(new IPNetworkInterface(IPNetworkInterfaceType.IPv4Loopback));
			a.Add(new IPNetworkInterface(IPNetworkInterfaceType.IPv6Any));
			a.Add(new IPNetworkInterface(IPNetworkInterfaceType.IPv6Loopback));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static IPNetworkInterface Parse(string s)
		{
			IPNetworkInterface result;

			if (TryParse(s, out result))
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is no valid network interface string!"));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out IPNetworkInterface result)
		{
			s = s.Trim();

			if      (StringEx.EqualsOrdinalIgnoreCase(s, Any_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Any_stringNice))
			{
				result = new IPNetworkInterface(IPNetworkInterfaceType.Any);
				return (true);
			}
			else if (s.Contains(IPv4Any_string))
			{
				result = new IPNetworkInterface(IPNetworkInterfaceType.IPv4Any);
				return (true);
			}
			else if (s.Contains(IPv4Loopback_string))
			{
				result = new IPNetworkInterface(IPNetworkInterfaceType.IPv4Loopback);
				return (true);
			}
			else if (s.Contains(IPv6Any_string))
			{
				result = new IPNetworkInterface(IPNetworkInterfaceType.IPv6Any);
				return (true);
			}
			else if (s.Contains(IPv6Loopback_string))
			{
				result = new IPNetworkInterface(IPNetworkInterfaceType.IPv6Loopback);
				return (true);
			}
			else
			{
				IPAddress address;
				if (IPAddress.TryParse(s, out address))
					result = new IPNetworkInterface(address, "");
				else
					result = new IPNetworkInterface(IPAddress.None, s);

				return (true);
			}
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator IPNetworkInterfaceType(IPNetworkInterface networkInterface)
		{
			return ((IPNetworkInterfaceType)networkInterface.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator IPNetworkInterface(IPNetworkInterfaceType networkInterface)
		{
			return (new IPNetworkInterface(networkInterface));
		}

		/// <summary></summary>
		public static implicit operator string(IPNetworkInterface networkInterface)
		{
			return (networkInterface.ToString());
		}

		/// <summary></summary>
		public static implicit operator IPNetworkInterface(string networkInterface)
		{
			return (Parse(networkInterface));
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
