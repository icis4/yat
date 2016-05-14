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
// MKY Development Version 1.0.14
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
			IPHostEx ipHost;
			if (IPHostEx.TryParse(remoteHost, out ipHost))
			{
				switch ((IPHost)ipHost)
				{
					case IPHost.Localhost:
					case IPHost.IPv4Localhost:
					case IPHost.IPv6Localhost:
					{
						ipAddress = ipHost.IPAddress;
						return (true);
					}

					case IPHost.Explicit:
					{
						try
						{
							IPAddress[] ipAddresses = Dns.GetHostAddresses(remoteHost);
							ipAddress = ipAddresses[0];
							return (true);
						}
						catch (Exception ex)
						{
							DebugEx.WriteException(typeof(IPResolver), ex, "Failed to get address from DNS!");
							ipAddress = IPAddress.None;
							return (false);
						}
					}

					default:
					{
						throw (new ArgumentOutOfRangeException("remoteHost", (IPHost)ipHost, "Unknown IP host type!"));
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
			IPNetworkInterfaceEx networkInterface;
			if (IPNetworkInterfaceEx.TryParse(localInterface, out networkInterface))
			{
				switch ((IPNetworkInterface)networkInterface)
				{
					case IPNetworkInterface.Any:
					case IPNetworkInterface.IPv4Any:
					case IPNetworkInterface.IPv4Loopback:
					case IPNetworkInterface.IPv6Any:
					case IPNetworkInterface.IPv6Loopback:
					{
						ipAddress = networkInterface.IPAddress;
						return (true);
					}

					case IPNetworkInterface.Explicit:
					{
						try
						{
							IPAddress[] ipAddresses = Dns.GetHostAddresses(localInterface);
							ipAddress = ipAddresses[0];
							return (true);
						}
						catch (Exception ex)
						{
							DebugEx.WriteException(typeof(IPResolver), ex, "Failed to get address from DNS!");
							ipAddress = IPAddress.None;
							return (false);
						}
					}

					default:
					{
						throw (new ArgumentOutOfRangeException("localInterface", (IPNetworkInterface)networkInterface, "Unknown network interface type!"));
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
