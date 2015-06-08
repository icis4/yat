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
// YAT 2.0 Gamma 1'' Version 1.99.34
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

using MKY.IO.Serial;
using MKY.IO.Serial.SerialPort;
using MKY.IO.Serial.Socket;
using MKY.IO.Serial.Usb;

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
						settings.Socket.RemoteTcpPort,
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
						settings.Socket.RemoteTcpPort,
						settings.Socket.ResolvedLocalIPAddress,
						settings.Socket.LocalTcpPort
						));
				}

				case Domain.IOType.Udp:
				{
					return (new UdpSocket
						(
						settings.Socket.ResolvedRemoteIPAddress,
						settings.Socket.RemoteUdpPort,
						settings.Socket.LocalUdpPort
						));
				}

				case Domain.IOType.UsbSerialHid:
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
