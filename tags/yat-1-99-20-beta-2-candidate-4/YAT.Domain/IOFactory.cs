using System;
using System.Collections.Generic;
using System.Text;

using MKY.IO.Serial;

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
						settings.Socket.ResolvedLocalIPAddress,
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
						settings.Socket.ResolvedLocalIPAddress,
						settings.Socket.LocalUdpPort
						));
				}

				default: throw (new NotImplementedException("Unknown IO type"));
			}
		}
	}
}
