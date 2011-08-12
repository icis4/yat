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
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Net;

using MKY.Diagnostics;

#endregion

namespace MKY.Net
{
	/// <summary></summary>
	static public class IPResolver
	{
		/// <summary></summary>
		static public IPAddress ResolveRemoteHost(string remoteHost)
		{
			IPHost ipHost;
			if (IPHost.TryParse(remoteHost, out ipHost))
			{
				switch ((IPHostType)ipHost)
				{
					case IPHostType.Localhost:
					case IPHostType.IPv4Localhost:
					case IPHostType.IPv6Localhost:
					{
						return (ipHost.IPAddress);
					}

					case IPHostType.Other:
					{
						try
						{
							IPAddress[] ipAddresses = System.Net.Dns.GetHostAddresses(remoteHost);
							return (ipAddresses[0]);
						}
						catch (Exception ex)
						{
							DebugEx.WriteException(typeof(IPResolver), ex);
							return (IPAddress.None);
						}
					}

					default:
					{
						throw (new ArgumentOutOfRangeException("value", (IPHostType)ipHost, "Unknown IP host type"));
					}
				}
			}
			else
			{
				return (IPAddress.None);
			}
		}

		/// <summary></summary>
		static public IPAddress ResolveLocalInterface(string localInterface)
		{
			IPNetworkInterface networkInterface;
			if (IPNetworkInterface.TryParse(localInterface, out networkInterface))
			{
				switch ((IPNetworkInterfaceType)networkInterface)
				{
					case IPNetworkInterfaceType.Any:
					case IPNetworkInterfaceType.IPv4Any:
					case IPNetworkInterfaceType.IPv4Loopback:
					case IPNetworkInterfaceType.IPv6Any:
					case IPNetworkInterfaceType.IPv6Loopback:
					{
						return (networkInterface.IPAddress);
					}

					case IPNetworkInterfaceType.Other:
					{
						try
						{
							IPAddress[] ipAddresses = Dns.GetHostAddresses(localInterface);
							return (ipAddresses[0]);
						}
						catch (Exception ex)
						{
							DebugEx.WriteException(typeof(IPResolver), ex);
							return (IPAddress.None);
						}
					}

					default:
					{
						throw (new ArgumentOutOfRangeException("value", (IPNetworkInterfaceType)networkInterface, "Unknown network interface type"));
					}
				}
			}
			else
			{
				return (IPAddress.None);
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
