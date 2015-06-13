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
// MKY Version 1.0.13
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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;

using MKY.Diagnostics;

#endregion

namespace MKY.Net
{
	/// <summary></summary>
	public static class IPResolver
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		public static bool TryResolveRemoteHost(string remoteHost, out IPAddress ipAddress)
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
						ipAddress = ipHost.IPAddress;
						return (true);
					}

					case IPHostType.Other:
					{
						try
						{
							IPAddress[] ipAddresses = Dns.GetHostAddresses(remoteHost);
							ipAddress = ipAddresses[0];
							return (true);
						}
						catch (Exception ex)
						{
							DebugEx.WriteException(typeof(IPResolver), ex);
							ipAddress = IPAddress.None;
							return (false);
						}
					}

					default:
					{
						throw (new ArgumentOutOfRangeException("remoteHost", (IPHostType)ipHost, "Unknown IP host type!"));
					}
				}
			}
			else
			{
				ipAddress = IPAddress.None;
				return (false);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		public static bool TryResolveLocalInterface(string localInterface, out IPAddress ipAddress)
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
						ipAddress = networkInterface.IPAddress;
						return (true);
					}

					case IPNetworkInterfaceType.Other:
					{
						try
						{
							IPAddress[] ipAddresses = Dns.GetHostAddresses(localInterface);
							ipAddress = ipAddresses[0];
							return (true);
						}
						catch (Exception ex)
						{
							DebugEx.WriteException(typeof(IPResolver), ex);
							ipAddress = IPAddress.None;
							return (false);
						}
					}

					default:
					{
						throw (new ArgumentOutOfRangeException("localInterface", (IPNetworkInterfaceType)networkInterface, "Unknown network interface type!"));
					}
				}
			}
			else
			{
				ipAddress = IPAddress.None;
				return (false);
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
