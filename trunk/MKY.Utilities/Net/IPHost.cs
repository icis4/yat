//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2009 Matthias Kläy.
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

	/// <summary></summary>
	public enum CommonIPHost
	{
		/// <summary></summary>
		Localhost,
		/// <summary></summary>
		IPv4Localhost,
		/// <summary></summary>
		IPv6Localhost,
		/// <summary></summary>
		Other,
	}

	#endregion

	/// <summary>
	/// Extended enum XIPHost.
	/// </summary>
	[Serializable]
	public class XIPHost : XEnum
	{
		#region String Definitions

		private const string Localhost_string = "localhost";
		private const string Localhost_stringNice = "<Localhost>";
		private const string IPv4Localhost_string = "IPv4 localhost";
		private const string IPv6Localhost_string = "IPv6 localhost";

		#endregion

		private IPAddress _otherAddress = IPAddress.None;

		/// <summary>Default is <see cref="CommonIPHost.Localhost"/></summary>
		public XIPHost()
			: base(CommonIPHost.Localhost)
		{
		}

		/// <summary></summary>
		public XIPHost(CommonIPHost host)
			: base(host)
		{
		}

		/// <summary></summary>
		public XIPHost(IPAddress address)
			: base(CommonIPHost.Other)
		{
			_otherAddress = address;
		}

		#region Properties

		/// <summary></summary>
		public IPAddress IPAddress
		{
			get
			{
				switch ((CommonIPHost)UnderlyingEnum)
				{
					case CommonIPHost.Localhost:     return (IPAddress.Loopback);
					case CommonIPHost.IPv4Localhost: return (IPAddress.Loopback);
					case CommonIPHost.IPv6Localhost: return (IPAddress.IPv6Loopback);
					case CommonIPHost.Other:         return (_otherAddress);
				}
				throw (new NotImplementedException(UnderlyingEnum.ToString()));
			}
		}

		#endregion

		#region ToString

		/// <summary></summary>
		public override string ToString()
		{
			switch ((CommonIPHost)UnderlyingEnum)
			{
				case CommonIPHost.Localhost:     return (Localhost_stringNice);
				case CommonIPHost.IPv4Localhost: return (IPv4Localhost_string);
				case CommonIPHost.IPv6Localhost: return (IPv6Localhost_string);
				case CommonIPHost.Other:         return (_otherAddress.ToString());
			}
			throw (new NotImplementedException(UnderlyingEnum.ToString()));
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static XIPHost[] GetItems()
		{
			List<XIPHost> a = new List<XIPHost>();
			a.Add(new XIPHost(CommonIPHost.Localhost));
			a.Add(new XIPHost(CommonIPHost.IPv4Localhost));
			a.Add(new XIPHost(CommonIPHost.IPv6Localhost));
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
				result = new XIPHost(CommonIPHost.Localhost);
				return (true);
			}
			else if (string.Compare(host, IPv4Localhost_string, true) == 0)
			{
				result = new XIPHost(CommonIPHost.IPv4Localhost);
				return (true);
			}
			else if (string.Compare(host, IPv6Localhost_string, true) == 0)
			{
				result = new XIPHost(CommonIPHost.IPv6Localhost);
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
				return (new XIPHost(CommonIPHost.Localhost));
			else if (address == IPAddress.IPv6Loopback)
				return (new XIPHost(CommonIPHost.IPv6Localhost));
			else
				return (new XIPHost(address));
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator CommonIPHost(XIPHost host)
		{
			return ((CommonIPHost)host.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator XIPHost(CommonIPHost host)
		{
			return (new XIPHost(host));
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
