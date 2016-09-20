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
// MKY Version 1.0.15
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
	public class IPFilterEx : EnumEx
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

		private string    explicitName = null;
		private IPAddress explicitAddress  = IPAddress.None;

		/// <summary>Default is <see cref="IPFilter.Any"/>.</summary>
		public const IPFilter Default = IPFilter.Any;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public IPFilterEx()
			: this(Default)
		{
		}

		/// <summary></summary>
		public IPFilterEx(IPFilter addressFilter)
			: base(addressFilter)
		{
			if (addressFilter == IPFilter.Explicit)
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "'IPFilter.Explicit' requires an IP address or host name, use IPFilterEx(IPAddress) instead!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary></summary>
		public IPFilterEx(IPAddress address)
		{                        // IPAddress does not override the ==/!= operators, thanks Microsoft guys...
			if      (address.Equals(IPAddress.Any))          { SetUnderlyingEnum(IPFilter.Any);           this.explicitAddress = IPAddress.None; }
			else if (address.Equals(IPAddress.Loopback))     { SetUnderlyingEnum(IPFilter.Localhost);     this.explicitAddress = IPAddress.None; }
			else if (address.Equals(IPAddress.IPv6Any))      { SetUnderlyingEnum(IPFilter.IPv6Any);       this.explicitAddress = IPAddress.None; }
			else if (address.Equals(IPAddress.IPv6Loopback)) { SetUnderlyingEnum(IPFilter.IPv6Localhost); this.explicitAddress = IPAddress.None; }
			else
			{
				if (address == null)
					throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "'IPFilter.Explicit' requires an IP address or host name!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, "address"));

				SetUnderlyingEnum(IPFilter.Explicit);

				this.explicitAddress = address;
			}

			// Note that 'IPAddressFilter.IPv4Any|Localhost' cannot be distinguished from 'IPAddressFilter.IPv4Any|Localhost' when 'IPAddress.Any|Loopback' is given.
		}

		/// <summary>
		/// Creates an explicit <see cref="IPFilterEx"/> object, using the provided host name.
		/// </summary>
		public IPFilterEx(string name)
		{
			SetUnderlyingEnum(IPFilter.Explicit);

			this.explicitName = name;
		}

		/// <summary>
		/// Creates an explicit <see cref="IPFilterEx"/> object, using the provided host name and address.
		/// </summary>
		public IPFilterEx(string name, IPAddress address)
		{
			SetUnderlyingEnum(IPFilter.Explicit);

			this.explicitName = name;

			if (address != null) // Keep 'IPAddress.None' otherwise.
				this.explicitAddress = address;
		}

		#region Properties

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
				}
				throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an unknown item!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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
				}
				throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an unknown item!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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
			return (Equals(obj as IPFilterEx));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public virtual bool Equals(IPFilterEx other)
		{
			if (ReferenceEquals(other, null))
				return (false);

			if (GetType() != other.GetType())
				return (false);

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
						this.explicitAddress.Equals(other.explicitAddress) // Explicit address is always given, at least 'IPAdress.None'.
					);                         // IPAddress does not override the ==/!= operators, thanks Microsoft guys...
				}
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

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "The exception indicates a fatal bug that shall be reported.")]
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
			}
			throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an unknown item!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary>
		/// Returns a compact string representation:
		/// - For predefined filters, the predefined string is returned.
		/// - For explicit filters, the host name or address is returned.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "The exception indicates a fatal bug that shall be reported.")]
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
			}
			throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an unknown item!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		#region ToString > Extensions
		//------------------------------------------------------------------------------------------
		// ToString > Extensions
		//------------------------------------------------------------------------------------------

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
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "The exception indicates a fatal bug that shall be reported.")]
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
			}
			throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an unknown item!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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
		public static string ToEndpointAddressString(IPAddress a)
		{
			switch (a.AddressFamily)
			{
				case AddressFamily.InterNetworkV6: return ("[" + a.ToString() + "]");
				default:                           return (      a.ToString()      );
			}
		}

		#endregion

		#endregion

		#region GetItems

		/// <summary></summary>
		public static IPFilterEx[] GetItems()
		{
			List<IPFilterEx> a = new List<IPFilterEx>(6); // Preset the required capacity to improve memory management.
			a.Add(new IPFilterEx(IPFilter.Any));
			a.Add(new IPFilterEx(IPFilter.Localhost));
			a.Add(new IPFilterEx(IPFilter.IPv4Any));
			a.Add(new IPFilterEx(IPFilter.IPv4Localhost));
			a.Add(new IPFilterEx(IPFilter.IPv6Any));
			a.Add(new IPFilterEx(IPFilter.IPv6Localhost));
			return (a.ToArray());
		}

		#endregion

		#region Parse

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
						result = new IPFilterEx(address);
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

			if (string.IsNullOrEmpty(s))
			{
				result = new IPFilterEx(); // Default!
				return (true); // Default silently, could e.g. happen when deserializing an XML.
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Any_string) ||
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
			else if (s.Contains(IPv4Any_string))
			{
				result = new IPFilterEx(IPFilter.IPv4Any);
				return (true);
			}
			else if (s.Contains(IPv4Localhost_string))
			{
				result = new IPFilterEx(IPFilter.IPv4Localhost);
				return (true);
			}
			else if (s.Contains(IPv6Any_string))
			{
				result = new IPFilterEx(IPFilter.IPv6Any);
				return (true);
			}
			else if (s.Contains(IPv6Localhost_string))
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

		/// <summary>
		/// Tries to resolve an IP address from DNS.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
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
					return (!this.explicitAddress.Equals(IPAddress.None)); // Explicit address is always given, at least 'IPAdress.None'.
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
				return (!this.explicitAddress.Equals(IPAddress.None));
			}
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator IPFilter(IPFilterEx addressFilter)
		{
			return ((IPFilter)addressFilter.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator IPFilterEx(IPFilter addressFilter)
		{
			return (new IPFilterEx(addressFilter));
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
		public static implicit operator string(IPFilterEx addressFilter)
		{
			return (addressFilter.ToString());
		}

		/// <summary></summary>
		public static implicit operator IPFilterEx(string addressFilter)
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
