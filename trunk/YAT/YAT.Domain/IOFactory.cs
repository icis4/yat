//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT Version 2.3.90 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;

using MKY;
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
			var clone = new Settings.IOSettings(settings); // Clone settings to ensure decoupling of domain and I/O instance.

			switch (settings.IOType)
			{
				case IOType.SerialPort:
				{
					return (new SerialPort(clone.SerialPort));
				}

				case IOType.TcpClient:
				{
					return (new TcpClient
					(
						clone.Socket.RemoteHost,
						clone.Socket.RemoteTcpPort,
						clone.Socket.LocalInterface,
						clone.Socket.TcpClientAutoReconnect
					));
				}

				case IOType.TcpServer:
				{
					return (new TcpServer
					(
						clone.Socket.LocalInterface,
						clone.Socket.LocalTcpPort
					));
				}

				case IOType.TcpAutoSocket:
				{
					return (new TcpAutoSocket
					(
						clone.Socket.RemoteHost,
						clone.Socket.RemoteTcpPort,
						clone.Socket.LocalInterface,
						clone.Socket.LocalTcpPort
					));
				}

				case IOType.UdpClient:
				{
					return (new UdpSocket
					(
						clone.Socket.RemoteHost,
						clone.Socket.RemoteUdpPort,
						clone.Socket.LocalInterface
					));
				}

				case IOType.UdpServer:
				{
					return (new UdpSocket
					(
						clone.Socket.LocalInterface,
						clone.Socket.LocalUdpPort,
						clone.Socket.LocalFilter,
						clone.Socket.UdpServerSendMode
					));
				}

				case IOType.UdpPairSocket:
				{
					return (new UdpSocket
					(
						clone.Socket.RemoteHost,
						clone.Socket.RemoteUdpPort,
						clone.Socket.LocalInterface,
						clone.Socket.LocalUdpPort
					));
				}

				case IOType.UsbSerialHid:
				{
					return (new SerialHidDevice(clone.UsbSerialHidDevice));
				}

				default:
				{
					throw (new ArgumentOutOfRangeException("settings", settings, MessageHelper.InvalidExecutionPreamble + "'" + settings + "' is an I/O type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
