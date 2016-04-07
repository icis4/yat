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
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;

using MKY.IO.Serial;
using MKY.IO.Serial.SerialPort;
using MKY.IO.Serial.Socket;
using MKY.IO.Serial.Usb;

namespace YAT.Domain
{
	internal static class IOFactory
	{
		public static IIOProvider CreateIO(Settings.IOSettings settings)
		{
			switch (settings.IOType)
			{
				case IOType.SerialPort:
				{
					return (new SerialPort(settings.SerialPort));
				}

				case IOType.TcpClient:
				{
					return (new TcpClient
						(
						settings.Socket.ResolvedRemoteIPAddress,
						settings.Socket.RemoteTcpPort,
						settings.Socket.TcpClientAutoReconnect
						));
				}

				case IOType.TcpServer:
				{
					return (new TcpServer
						(
						settings.Socket.ResolvedLocalIPAddress,
						settings.Socket.LocalTcpPort
						));
				}

				case IOType.TcpAutoSocket:
				{
					return (new TcpAutoSocket
						(
						settings.Socket.ResolvedRemoteIPAddress,
						settings.Socket.RemoteTcpPort,
						settings.Socket.ResolvedLocalIPAddress,
						settings.Socket.LocalTcpPort
						));
				}

				case IOType.UdpClient:
				{
					return (new UdpSocket
						(
						settings.Socket.ResolvedRemoteIPAddress,
						settings.Socket.RemoteUdpPort
						));
				}

				case IOType.UdpServer:
				{
					return (new UdpSocket
						(
						settings.Socket.LocalUdpPort,
						settings.Socket.ResolvedLocalIPAddressFilter
						));
				}

				case IOType.UdpPairSocket:
				{
					return (new UdpSocket
						(
						settings.Socket.ResolvedRemoteIPAddress,
						settings.Socket.RemoteUdpPort,
						settings.Socket.LocalUdpPort,
						settings.Socket.ResolvedLocalIPAddressFilter
						));
				}

				case IOType.UsbSerialHid:
				{
					return (new SerialHidDevice(settings.UsbSerialHidDevice));
				}

				default:
				{
					throw (new ArgumentOutOfRangeException("settings", settings, "'" + settings + "' is an unknown IO type string"));
				}
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
