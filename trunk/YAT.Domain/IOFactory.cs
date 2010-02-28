//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright � 2003-2004 HSR Hochschule f�r Technik Rapperswil.
// Copyright � 2003-2010 Matthias Kl�y.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

using MKY.IO.Serial;

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\RawTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	internal static class IOFactory
	{
		public static IIOProvider CreateIO(Settings.IOSettings settings)
		{
			switch (settings.IOType)
			{
				case Domain.IOType.SerialPort:
				{
					return (new SerialPort(settings.SerialPort));
				}

				case Domain.IOType.TcpClient:
				{
					return (new TcpClient
						(
						settings.Socket.ResolvedRemoteIPAddress,
						settings.Socket.RemotePort,
						settings.Socket.TcpClientAutoReconnect
						));
				}

				case Domain.IOType.TcpServer:
				{
					return (new TcpServer
						(
						settings.Socket.ResolvedLocalIPAddress,
						settings.Socket.LocalTcpPort
						));
				}

				case Domain.IOType.TcpAutoSocket:
				{
					return (new TcpAutoSocket
						(
						settings.Socket.ResolvedRemoteIPAddress,
						settings.Socket.RemotePort,
						settings.Socket.ResolvedLocalIPAddress,
						settings.Socket.LocalTcpPort
						));
				}

				case Domain.IOType.Udp:
				{
					return (new UdpSocket
						(
						settings.Socket.ResolvedRemoteIPAddress,
						settings.Socket.RemotePort,
						settings.Socket.LocalUdpPort
						));
				}

                case Domain.IOType.UsbHid:
                {
                    return (new UsbHidDevice
                        (
                        settings.UsbHidPort.DeviceId,
                        settings.UsbHidPort.AutoReconnect
                        ));
                }

				default: throw (new NotImplementedException("Unknown IO type"));
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
