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
// MKY Version 1.0.17
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
	#region Enum IPNetworkInterface

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
	/// Extended enum IPNetworkInterfaceEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
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

		private string    explicitDescription = null;
		private IPAddress explicitAddress     = IPAddress.None;

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
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "'IPNetworkInterface.Explicit' requires an IP address or interface description, use IPNetworkInterfaceEx(IPAddress, string) instead!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behaviour.")]
		public IPNetworkInterfaceEx(IPAddress address, string description = null)
		{
			if (string.IsNullOrEmpty(description)) // Defined by address only.
			{
				if (address == null)
					throw (new ArgumentNullException("address", "An IP address is required when interface description is not given!"));

				                   // IPAddress does not override the ==/!= operators, thanks Microsoft guys...
				if      (address.Equals(IPAddress.Any))          { SetUnderlyingEnum(IPNetworkInterface.Any);          this.explicitAddress = IPAddress.None; }
				else if (address.Equals(IPAddress.Loopback))     { SetUnderlyingEnum(IPNetworkInterface.Loopback);     this.explicitAddress = IPAddress.None; }
				else if (address.Equals(IPAddress.IPv6Any))      { SetUnderlyingEnum(IPNetworkInterface.IPv6Any);      this.explicitAddress = IPAddress.None; }
				else if (address.Equals(IPAddress.IPv6Loopback)) { SetUnderlyingEnum(IPNetworkInterface.IPv6Loopback); this.explicitAddress = IPAddress.None; }
				else                                             { SetUnderlyingEnum(IPNetworkInterface.Explicit);     this.explicitAddress = address;        }

				// Note that 'IPNetworkInterface.IPv4Loopback' cannot be distinguished from 'IPNetworkInterface.Loopback' when 'IPAddress.Loopback' is given.
			}
			else // Defined by description (and optional address).
			{
				IPNetworkInterface enumResult;
				if (TryParse(description, out enumResult)) // Predefined.
				{
					SetUnderlyingEnum(enumResult);
				}
				else // Explicitly defined, may also cover variants of predefined interfaces, e.g. interface specific loopback!
				{
					if ((address == null) || address.Equals(IPAddress.None))
						throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "'IPNetworkInterface.Explicit' requires an IP address!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

					SetUnderlyingEnum(IPNetworkInterface.Explicit);

					this.explicitDescription = description;
					this.explicitAddress     = address;
				}
			}
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

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
				throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an unknown item!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		public string Description
		{
			get
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
						if (!string.IsNullOrEmpty(this.explicitDescription))
							return (this.explicitDescription);
						else
							return ("");
					}
				}
				throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an unknown item!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		public bool IsLocalhost
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

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public IPNetworkInterfaceDescriptorPair ToDescriptorPair()
		{
			return (new IPNetworkInterfaceDescriptorPair(Description, Address.ToString()));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality,
		/// ignoring <see cref="Address"/>.
		/// </summary>
		public bool EqualsDescription(IPNetworkInterfaceEx other)
		{
			if (ReferenceEquals(other, null))
				return (false);

			if (GetType() != other.GetType())
				return (false);

			if ((IPNetworkInterface)UnderlyingEnum == IPNetworkInterface.Explicit)
			{
				return
				(
					base.Equals(other) &&
					(this.explicitDescription == other.explicitDescription)
				);
			}
			else
			{
				return
				(
					base.Equals(other)
				////this.explicitDescription is not given.
				);
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
						return (this.explicitDescription + " (" + this.explicitAddress.ToString() + ")"); // Explicit address is always given, at least 'IPAdress.None'.
					else
						return (this.explicitAddress.ToString()); // Explicit address is always given, at least 'IPAdress.None'.
				}
			}
			throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an unknown item!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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
					if (!string.IsNullOrEmpty(this.explicitDescription))
						hashCode = (hashCode * 397) ^ this.explicitDescription.GetHashCode();

					hashCode = (hashCode * 397) ^ this.explicitAddress.GetHashCode(); // Explicit address is always given, at least 'IPAdress.None'.
				}

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as IPNetworkInterfaceEx));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public virtual bool Equals(IPNetworkInterfaceEx other)
		{
			if (ReferenceEquals(other, null))
				return (false);

			if (GetType() != other.GetType())
				return (false);

			if ((IPNetworkInterface)UnderlyingEnum == IPNetworkInterface.Explicit)
			{
				return
				(
					base.Equals(other) &&
					(this.explicitDescription == other.explicitDescription) &&
					this.explicitAddress.Equals(other.explicitAddress) // Explicit address is always given, at least 'IPAdress.None'.
				);                         // IPAddress does not override the ==/!= operators, thanks Microsoft guys...
			}
			else
			{
				return (base.Equals(other));
			}
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(IPNetworkInterfaceEx lhs, IPNetworkInterfaceEx rhs)
		{
			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(IPNetworkInterfaceEx lhs, IPNetworkInterfaceEx rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion

		#region GetItems
		//==========================================================================================
		// GetItems
		//==========================================================================================

		/// <remarks>
		/// An array of extended enum items is returned for more versatile use, e.g. UI controls lists.
		/// </remarks>
		public static IPNetworkInterfaceEx[] GetItems()
		{
			List<IPNetworkInterfaceEx> a = new List<IPNetworkInterfaceEx>(6); // Preset the required capacity to improve memory management.
			a.Add(new IPNetworkInterfaceEx(IPNetworkInterface.Any));
			a.Add(new IPNetworkInterfaceEx(IPNetworkInterface.Loopback));
			a.Add(new IPNetworkInterfaceEx(IPNetworkInterface.IPv4Any));
			a.Add(new IPNetworkInterfaceEx(IPNetworkInterface.IPv4Loopback));
			a.Add(new IPNetworkInterfaceEx(IPNetworkInterface.IPv6Any));
			a.Add(new IPNetworkInterfaceEx(IPNetworkInterface.IPv6Loopback));
			return (a.ToArray());
		}

		#endregion

		#region Parse
		//==========================================================================================
		// Parse
		//==========================================================================================

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
				if (s != null) // IPAddress.TryParse() does not support 'null', thanks Microsoft guys...
				{
					IPAddress address;
					if (IPAddress.TryParse(s, out address)) // Valid explicit?
					{
						result = new IPNetworkInterfaceEx(address);
						return (true);
					}
				}

				// Invalid string!
				result = null;
				return (false);
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

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

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
		public static implicit operator IPNetworkInterfaceDescriptorPair(IPNetworkInterfaceEx networkInterface)
		{
			return (networkInterface.ToDescriptorPair());
		}

		/// <summary></summary>
		public static implicit operator IPNetworkInterfaceEx(IPNetworkInterfaceDescriptorPair networkInterface)
		{
			if (networkInterface.Address != null) // IPAddress.TryParse() does not support 'null', thanks Microsoft guys...
			{
				IPAddress address;
				if (IPAddress.TryParse(networkInterface.Address, out address))
					return (new IPNetworkInterfaceEx(address, networkInterface.Description));
			}

			// Use default = [Any] in case o invalid interface (required for backward compatibility with old settings).
			return (new IPNetworkInterfaceEx(IPAddress.Any, networkInterface.Description));
		}

		/// <summary></summary>
		public static implicit operator IPAddress(IPNetworkInterfaceEx networkInterface)
		{
			return (networkInterface.Address);
		}

		/// <summary></summary>
		public static implicit operator IPNetworkInterfaceEx(IPAddress address)
		{
			return (new IPNetworkInterfaceEx(address));
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
