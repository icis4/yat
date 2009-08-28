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
using System.Text;
using System.Net;
using System.Net.NetworkInformation;

using MKY.Utilities.Types;

namespace MKY.Utilities.Net
{
	#region Enum CommonNetworkInterface

	/// <summary></summary>
	public enum CommonNetworkInterface
	{
		/// <summary></summary>
		Any,
		/// <summary></summary>
		IPv4Any,
		/// <summary></summary>
		IPv4Loopback,
		/// <summary></summary>
		IPv6Any,
		/// <summary></summary>
		IPv6Loopback,
		/// <summary></summary>
		Other,
	}

	#endregion

	/// <summary>
	/// Extended enum XNetworkInterface.
	/// </summary>
	[Serializable]
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

		private IPAddress _otherAddress = IPAddress.None;
		private string _otherDescription = "";

		/// <summary>Default is <see cref="CommonNetworkInterface.Any"/></summary>
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
			_otherAddress = address;
			_otherDescription = description;
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
					case CommonNetworkInterface.Other:        return (_otherAddress);
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
					if (_otherDescription != "")
					{
						if (_otherAddress != IPAddress.None)
							return (_otherDescription + " (" + _otherAddress + ")");
						else
							return (_otherDescription);
					}
					else
					{
						if (_otherAddress != IPAddress.None)
							return (_otherAddress.ToString());
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
