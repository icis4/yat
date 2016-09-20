﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Version 1.99.50
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
						settings.Socket.RemoteHost,
						settings.Socket.RemoteTcpPort,
						settings.Socket.LocalInterface,
						settings.Socket.TcpClientAutoReconnect
						));
				}

				case IOType.TcpServer:
				{
					return (new TcpServer
						(
						settings.Socket.LocalInterface,
						settings.Socket.LocalTcpPort
						));
				}

				case IOType.TcpAutoSocket:
				{
					return (new TcpAutoSocket
						(
						settings.Socket.RemoteHost,
						settings.Socket.RemoteTcpPort,
						settings.Socket.LocalInterface,
						settings.Socket.LocalTcpPort
						));
				}

				case IOType.UdpClient:
				{
					return (new UdpSocket
						(
						settings.Socket.RemoteHost,
						settings.Socket.RemoteUdpPort
						));
				}

				case IOType.UdpServer:
				{
					return (new UdpSocket
						(
						settings.Socket.LocalUdpPort,
						settings.Socket.LocalFilter,
						settings.Socket.UdpServerSendMode
						));
				}

				case IOType.UdpPairSocket:
				{
					return (new UdpSocket
						(
						settings.Socket.RemoteHost,
						settings.Socket.RemoteUdpPort,
						settings.Socket.LocalUdpPort
						));
				}

				case IOType.UsbSerialHid:
				{
					return (new SerialHidDevice(settings.UsbSerialHidDevice));
				}

				default:
				{
					throw (new ArgumentOutOfRangeException("settings", settings, MessageHelper.InvalidExecutionPreamble + "'" + settings + "' is an unknown IO type string!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
