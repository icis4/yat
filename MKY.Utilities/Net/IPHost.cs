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
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Net;

using MKY.Utilities.Types;

namespace MKY.Utilities.Net
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
	/// Extended enum XIPHost.
	/// </summary>
	public class XIPHost : XEnum
	{
		#region String Definitions

		private const string Localhost_string = "localhost";
		private const string Localhost_stringNice = "<Localhost>";
		private const string IPv4Localhost_string = "IPv4 localhost";
		private const string IPv6Localhost_string = "IPv6 localhost";

		#endregion

		private IPAddress otherAddress = IPAddress.None;

		/// <summary>Default is <see cref="IPHostType.Localhost"/>.</summary>
		public XIPHost()
			: base(IPHostType.Localhost)
		{
		}

		/// <summary></summary>
		public XIPHost(IPHostType hostType)
			: base(hostType)
		{
		}

		/// <summary></summary>
		public XIPHost(IPAddress address)
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
		public static XIPHost[] GetItems()
		{
			List<XIPHost> a = new List<XIPHost>();
			a.Add(new XIPHost(IPHostType.Localhost));
			a.Add(new XIPHost(IPHostType.IPv4Localhost));
			a.Add(new XIPHost(IPHostType.IPv6Localhost));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public static XIPHost Parse(string host)
		{
			XIPHost result;

			if (TryParse(host, out result))
				return (result);
			else
				throw (new ArgumentOutOfRangeException("host", host, "Invalid host."));
		}

		/// <summary></summary>
		public static bool TryParse(string host, out XIPHost result)
		{
			IPAddress address;

			if     ((string.Compare(host, Localhost_string, true) == 0) ||
				    (string.Compare(host, Localhost_stringNice, true) == 0))
			{
				result = new XIPHost(IPHostType.Localhost);
				return (true);
			}
			else if (string.Compare(host, IPv4Localhost_string, true) == 0)
			{
				result = new XIPHost(IPHostType.IPv4Localhost);
				return (true);
			}
			else if (string.Compare(host, IPv6Localhost_string, true) == 0)
			{
				result = new XIPHost(IPHostType.IPv6Localhost);
				return (true);
			}
			else if (IPAddress.TryParse(host, out address))
			{
				result = new XIPHost(address);
				return (true);
			}
			else
			{
				result = null;
				return (false);
			}
		}

		/// <summary></summary>
		public static XIPHost ParseFromIPAddress(IPAddress address)
		{
			if (address == IPAddress.Loopback)
				return (new XIPHost(IPHostType.Localhost));
			else if (address == IPAddress.IPv6Loopback)
				return (new XIPHost(IPHostType.IPv6Localhost));
			else
				return (new XIPHost(address));
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator IPHostType(XIPHost host)
		{
			return ((IPHostType)host.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator XIPHost(IPHostType hostType)
		{
			return (new XIPHost(hostType));
		}

		/// <summary></summary>
		public static implicit operator IPAddress(XIPHost address)
		{
			return (address.IPAddress);
		}

		/// <summary></summary>
		public static implicit operator XIPHost(IPAddress address)
		{
			return (ParseFromIPAddress(address));
		}

		/// <summary></summary>
		public static implicit operator string(XIPHost host)
		{
			return (host.ToString());
		}

		/// <summary></summary>
		public static implicit operator XIPHost(string host)
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
