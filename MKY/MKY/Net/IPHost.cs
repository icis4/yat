//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.7
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Net;

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
		IPv4Localhost,
		IPv6Localhost,
		Other,
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum IPHost.
	/// </summary>
	public class IPHost : EnumEx
	{
		#region String Definitions

		private const string Localhost_string = "localhost";
		private const string Localhost_stringNice = "<Localhost>";
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
		}

		/// <summary></summary>
		public IPHost(IPAddress address)
		{
			if      (address == IPAddress.Loopback)     { SetUnderlyingEnum(IPHostType.Localhost);     this.otherAddress = IPAddress.None; }
			else if (address == IPAddress.Loopback)     { SetUnderlyingEnum(IPHostType.IPv4Localhost); this.otherAddress = IPAddress.None; }
			else if (address == IPAddress.IPv6Loopback) { SetUnderlyingEnum(IPHostType.IPv6Localhost); this.otherAddress = IPAddress.None; }
			else                                        { SetUnderlyingEnum(IPHostType.Other);         this.otherAddress = address;        }
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
				throw (new NotImplementedException(UnderlyingEnum.ToString()));
			}
		}

		#endregion

		#region ToString

		/// <summary></summary>
		public override string ToString()
		{
			switch ((IPHostType)UnderlyingEnum)
			{
				case IPHostType.Localhost:     return (Localhost_string);
				case IPHostType.IPv4Localhost: return (IPv4Localhost_string);
				case IPHostType.IPv6Localhost: return (IPv6Localhost_string);
				case IPHostType.Other:         return (this.otherAddress.ToString());
			}
			throw (new NotImplementedException(UnderlyingEnum.ToString()));
		}

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

		/// <summary></summary>
		public static IPHost Parse(string host)
		{
			IPHost result;

			if (TryParse(host, out result))
				return (result);
			else
				throw (new ArgumentOutOfRangeException("host", host, "Invalid host."));
		}

		/// <summary></summary>
		public static bool TryParse(string host, out IPHost result)
		{
			IPAddress address;

			if      (StringEx.EqualsOrdinalIgnoreCase(host, Localhost_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(host, Localhost_stringNice))
			{
				result = new IPHost(IPHostType.Localhost);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(host, IPv4Localhost_string))
			{
				result = new IPHost(IPHostType.IPv4Localhost);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(host, IPv6Localhost_string))
			{
				result = new IPHost(IPHostType.IPv6Localhost);
				return (true);
			}
			else if (IPAddress.TryParse(host, out address))
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
			if (address == IPAddress.Loopback)
				return (new IPHost(IPHostType.Localhost));
			else if (address == IPAddress.IPv6Loopback)
				return (new IPHost(IPHostType.IPv6Localhost));
			else
				return (new IPHost(address));
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
