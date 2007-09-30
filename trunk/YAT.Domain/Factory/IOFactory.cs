using System;
using System.Collections.Generic;
using System.Text;

using YAT.Domain.IO;

namespace YAT.Domain.Factory
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
						settings.Socket.RemotePort,
						settings.Socket.ResolvedLocalIPAddress,
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
						settings.Socket.RemotePort,
						settings.Socket.ResolvedLocalIPAddress,
						settings.Socket.LocalTcpPort
						));
				}

				case IOType.Udp:
				{
					return (new UdpSocket
						(
						settings.Socket.ResolvedRemoteIPAddress,
						settings.Socket.RemotePort,
						settings.Socket.ResolvedLocalIPAddress,
						settings.Socket.LocalUdpPort
						));
				}

				default: throw (new NotImplementedException("Unknown IO type"));
			}
		}
	}
}
