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
// MKY Version 1.0.12
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
using System.Net.Sockets;

namespace MKY.Net
{
	#region Enum CommonIPHost

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum IPHostType
	{
		Localhost,

		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IPv4 is a common term, and even used by the .NET framework itself.")]
		IPv4Localhost,
		
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IPv6 is a common term, and even used by the .NET framework itself.")]
		IPv6Localhost,
		
		Other,
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum IPHost.
	/// </summary>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[Serializable]
	public class IPHost : EnumEx
	{
		#region String Definitions

		private const string Localhost_string     = "localhost";   // Backward compatibility.
		private const string Localhost_stringNice = "<Localhost>"; // Nicer readable presentation.

		private const string IPv4Localhost_string = "IPv4 localhost";
		private const string IPv6Localhost_string = "IPv6 localhost";

		#endregion

		private IPAddress otherAddress = IPAddress.None;

		/// <summary>Default is <see cref="IPHostType.Localhost"/>.</summary>
		public IPHost()
			: base(IPHostType.Localhost)
		{
		}

		/// <summary></summary>
		public IPHost(IPHostType hostType)
			: base(hostType)
		{
			if (hostType == IPHostType.Other)
				throw (new InvalidOperationException("Program execution should never get here, 'IPHostType.Other' requires an IP address, please report this bug!"));
		}

		/// <summary></summary>
		public IPHost(IPAddress address)
		{
			if      (address == IPAddress.Loopback)     { SetUnderlyingEnum(IPHostType.Localhost);     this.otherAddress = IPAddress.None; }
			else if (address == IPAddress.IPv6Loopback) { SetUnderlyingEnum(IPHostType.IPv6Localhost); this.otherAddress = IPAddress.None; }
			else                                        { SetUnderlyingEnum(IPHostType.Other);         this.otherAddress = address;        }

			// Note that 'IPHostType.IPv4Localhost' cannot be distinguished from 'IPHostType.Localhost' when 'IPAddress.Loopback' is given.
			// Also note that similar but optimized code is found at ParseFromIPAddress() further below.
		}

		#region Properties

		/// <summary></summary>
		public IPAddress IPAddress
		{
			get
			{
				switch ((IPHostType)UnderlyingEnum)
				{
					case IPHostType.Localhost:     return (IPAddress.Loopback);
					case IPHostType.IPv4Localhost: return (IPAddress.Loopback);
					case IPHostType.IPv6Localhost: return (IPAddress.IPv6Loopback);
					case IPHostType.Other:         return (this.otherAddress);
				}
				throw (new InvalidOperationException("Program execution should never get here, item " + UnderlyingEnum.ToString() + " is unknown, please report this bug!"));
			}
		}

		#endregion

		#region ToString

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "The exception indicates a fatal bug that shall be reported.")]
		public override string ToString()
		{
			switch ((IPHostType)UnderlyingEnum)
			{
				case IPHostType.Localhost:     return (Localhost_stringNice);
				case IPHostType.IPv4Localhost: return (IPv4Localhost_string);
				case IPHostType.IPv6Localhost: return (IPv6Localhost_string);
				case IPHostType.Other:         return (this.otherAddress.ToString());
			}
			throw (new InvalidOperationException("Program execution should never get here, item " + UnderlyingEnum.ToString() + " is unknown, please report this bug!"));
		}

		/// <summary>
		/// Returns a <see cref="System.String" /> that e.g. adds [] for IPv6 addresses.
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
		public string ToUrlString()
		{
			switch ((IPHostType)UnderlyingEnum)
			{
				case IPHostType.Localhost:     return (Localhost_stringNice);
				case IPHostType.IPv4Localhost: return (IPv4Localhost_string);
				case IPHostType.IPv6Localhost: return (IPv6Localhost_string);
				case IPHostType.Other:         return (ToUrlString(this.otherAddress.ToString()));
			}
			throw (new InvalidOperationException("Program execution should never get here, item " + UnderlyingEnum.ToString() + " is unknown, please report this bug!"));
		}

		#region ToString > Extensions
		//------------------------------------------------------------------------------------------
		// ToString > Extensions
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Returns a <see cref="System.String" /> that e.g. adds [] for IPv6 addresses.
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
		public static string ToUrlString(IPAddress a)
		{
			switch (a.AddressFamily)
			{
				case AddressFamily.InterNetworkV6: return ("[" + a.ToString() + "]");
				default:                           return (      a.ToString()      );
			}
		}

		/// <summary>
		/// Returns a <see cref="System.String" /> that e.g. adds [] for IPv6 addresses.
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
		public static string ToUrlString(string s)
		{
			IPHost host;
			if (!string.IsNullOrEmpty(s) && IPHost.TryParse(s, out host))
				return (ToUrlString(host.IPAddress));

			return (s);
		}

		#endregion

		#endregion

		#region GetItems

		/// <summary></summary>
		public static IPHost[] GetItems()
		{
			List<IPHost> a = new List<IPHost>();
			a.Add(new IPHost(IPHostType.Localhost));
			a.Add(new IPHost(IPHostType.IPv4Localhost));
			a.Add(new IPHost(IPHostType.IPv6Localhost));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static IPHost Parse(string s)
		{
			IPHost result;

			if (TryParse(s, out result))
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is no valid host string!"));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out IPHost result)
		{
			s = s.Trim();

			IPAddress address;

			if      (StringEx.EqualsOrdinalIgnoreCase(s, Localhost_string) ||
			        (StringEx.EqualsOrdinalIgnoreCase(s, Localhost_stringNice)))
			{	// Note that similar code is found in IPNetworkInterface.TryParse()!
				result = new IPHost(IPHostType.Localhost);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, IPv4Localhost_string))
			{
				result = new IPHost(IPHostType.IPv4Localhost);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, IPv6Localhost_string))
			{
				result = new IPHost(IPHostType.IPv6Localhost);
				return (true);
			}
			else if (IPAddress.TryParse(s, out address))
			{
				result = new IPHost(address);
				return (true);
			}
			else
			{
				result = null;
				return (false);
			}
		}

		/// <summary></summary>
		public static IPHost ParseFromIPAddress(IPAddress address)
		{
			if      (address == IPAddress.Loopback)
				return (new IPHost(IPHostType.Localhost));
			else if (address == IPAddress.IPv6Loopback)
				return (new IPHost(IPHostType.IPv6Localhost));
			else
				return (new IPHost(address));

			// Note that 'IPHostType.IPv4Localhost' cannot be distinguished from 'IPHostType.Localhost' when 'IPAddress.Loopback' is given.
			// Also note that similar but less optimized code is found at IPHost(IPAddress) further above.
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator IPHostType(IPHost host)
		{
			return ((IPHostType)host.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator IPHost(IPHostType hostType)
		{
			return (new IPHost(hostType));
		}

		/// <summary></summary>
		public static implicit operator IPAddress(IPHost address)
		{
			return (address.IPAddress);
		}

		/// <summary></summary>
		public static implicit operator IPHost(IPAddress address)
		{
			return (ParseFromIPAddress(address));
		}

		/// <summary></summary>
		public static implicit operator string(IPHost host)
		{
			return (host.ToString());
		}

		/// <summary></summary>
		public static implicit operator IPHost(string host)
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
