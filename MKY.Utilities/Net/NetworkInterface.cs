//==================================================================================================
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
	#region Enum CommonNetworkInterface

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum CommonNetworkInterface
	{
		Any,
		IPv4Any,
		IPv4Loopback,
		IPv6Any,
		IPv6Loopback,
		Other,
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum XNetworkInterface.
	/// </summary>
	public class XNetworkInterface : XEnum
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

		/// <summary>Default is <see cref="CommonNetworkInterface.Any"/>.</summary>
		public XNetworkInterface()
			: base(CommonNetworkInterface.Any)
		{
		}

		/// <summary></summary>
		public XNetworkInterface(CommonNetworkInterface networkInterface)
			: base(networkInterface)
		{
		}

		/// <summary></summary>
		public XNetworkInterface(IPAddress address, string description)
			: base(CommonNetworkInterface.Other)
		{
			this.otherAddress = address;
			this.otherDescription = description;
		}

		#region Properties

		/// <summary></summary>
		public IPAddress IPAddress
		{
			get
			{
				switch ((CommonNetworkInterface)UnderlyingEnum)
				{
					case CommonNetworkInterface.Any:          return (IPAddress.Any);
					case CommonNetworkInterface.IPv4Any:      return (IPAddress.Any);
					case CommonNetworkInterface.IPv4Loopback: return (IPAddress.Loopback);
					case CommonNetworkInterface.IPv6Any:      return (IPAddress.IPv6Any);
					case CommonNetworkInterface.IPv6Loopback: return (IPAddress.IPv6Loopback);
					case CommonNetworkInterface.Other:        return (this.otherAddress);
				}
				throw (new NotImplementedException(UnderlyingEnum.ToString()));
			}
		}

		#endregion

		#region ToString

		/// <summary></summary>
		public override string ToString()
		{
			switch ((CommonNetworkInterface)UnderlyingEnum)
			{
				case CommonNetworkInterface.Any:          return (Any_stringNice);
				case CommonNetworkInterface.IPv4Any:      return (IPv4Any_string      + " (" + IPAddress.Any + ")");
				case CommonNetworkInterface.IPv4Loopback: return (IPv4Loopback_string + " (" + IPAddress.Loopback + ")");
				case CommonNetworkInterface.IPv6Any:      return (IPv6Any_string      + " (" + IPAddress.IPv6Any + ")");
				case CommonNetworkInterface.IPv6Loopback: return (IPv6Loopback_string + " (" + IPAddress.IPv6Loopback + ")");
				case CommonNetworkInterface.Other:
				{
					if (this.otherDescription != "")
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
							throw (new ArgumentOutOfRangeException("address and description", "IP address and interface description or both undefined"));
					}
				}
			}
			throw (new NotImplementedException(UnderlyingEnum.ToString()));
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static XNetworkInterface[] GetItems()
		{
			List<XNetworkInterface> a = new List<XNetworkInterface>();
			a.Add(new XNetworkInterface(CommonNetworkInterface.Any));
			a.Add(new XNetworkInterface(CommonNetworkInterface.IPv4Any));
			a.Add(new XNetworkInterface(CommonNetworkInterface.IPv4Loopback));
			a.Add(new XNetworkInterface(CommonNetworkInterface.IPv6Any));
			a.Add(new XNetworkInterface(CommonNetworkInterface.IPv6Loopback));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public static XNetworkInterface Parse(string networkInterface)
		{
			XNetworkInterface result;

			if (TryParse(networkInterface, out result))
				return (result);
			else
				throw (new ArgumentOutOfRangeException("networkInterface", networkInterface, "Invalid network interface."));
		}

		/// <summary></summary>
		public static bool TryParse(string networkInterface, out XNetworkInterface result)
		{
			if     ((string.Compare(networkInterface, Any_string, true) == 0) ||
					(string.Compare(networkInterface, Any_stringNice, true) == 0))
			{
				result = new XNetworkInterface(CommonNetworkInterface.Any);
				return (true);
			}
			else if (networkInterface.Contains(IPv4Any_string))
			{
				result = new XNetworkInterface(CommonNetworkInterface.IPv4Any);
				return (true);
			}
			else if (networkInterface.Contains(IPv4Loopback_string))
			{
				result = new XNetworkInterface(CommonNetworkInterface.IPv4Loopback);
				return (true);
			}
			else if (networkInterface.Contains(IPv6Any_string))
			{
				result = new XNetworkInterface(CommonNetworkInterface.IPv6Any);
				return (true);
			}
			else if (networkInterface.Contains(IPv6Loopback_string))
			{
				result = new XNetworkInterface(CommonNetworkInterface.IPv6Loopback);
				return (true);
			}
			else
			{
				IPAddress address;
				if (IPAddress.TryParse(networkInterface, out address))
					result = new XNetworkInterface(address, "");
				else
					result = new XNetworkInterface(IPAddress.None, networkInterface);

				return (true);
			}
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator CommonNetworkInterface(XNetworkInterface networkInterface)
		{
			return ((CommonNetworkInterface)networkInterface.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator XNetworkInterface(CommonNetworkInterface networkInterface)
		{
			return (new XNetworkInterface(networkInterface));
		}

		/// <summary></summary>
		public static implicit operator string(XNetworkInterface networkInterface)
		{
			return (networkInterface.ToString());
		}

		/// <summary></summary>
		public static implicit operator XNetworkInterface(string networkInterface)
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
