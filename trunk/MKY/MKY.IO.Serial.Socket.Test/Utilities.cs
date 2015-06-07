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
// MKY Development Version 1.0.13
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

using NUnit.Framework;

namespace MKY.IO.Serial.Socket.Test
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
	public static class Utilities
	{
		#region Private Constants
		//==========================================================================================
		// Private Constants
		//==========================================================================================

		private const int WaitInterval = 100;
		private const int WaitTimeout = 10000;

		#endregion

		#region Available Ports
		//==========================================================================================
		// Available Ports
		//==========================================================================================

		internal static int AvailableLocalTcpPort
		{
			get
			{
				IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
				IPEndPoint[] listeners = properties.GetActiveTcpListeners();

				int port;
				for (port = 10000; port <= IPEndPoint.MaxPort; port++)
				{
					bool found = false;
					foreach (IPEndPoint ep in listeners)
					{
						if (ep.Port == port)
						{
							found = true;
							break;
						}
					}

					if (!found)
						return (port);
				}

				throw (new OverflowException("No local TCP port available within range of 10000 through " + IPEndPoint.MaxPort + "!"));
			}
		}

		internal static void GetAvailableLocalUdpPorts(out int portA, out int portB)
		{
			portA = 0;
			portB = 0;

			IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
			IPEndPoint[] listeners = properties.GetActiveUdpListeners();

			int port;
			for (port = 10000; port <= IPEndPoint.MaxPort; port++)
			{
				bool found = false;
				foreach (IPEndPoint ep in listeners)
				{
					if (ep.Port == port)
					{
						found = true;
						break;
					}
				}

				if (!found)
				{
					if (portA == 0)
					{
						portA = port;
					}
					else if (portB == 0)
					{
						portB = port;
						return;
					}
				}
			}

			throw (new OverflowException("No local UDP ports available within range of 10000 through " + IPEndPoint.MaxPort + "!"));
		}

		#endregion

		#region Start/Stop
		//==========================================================================================
		// Start/Stop
		//==========================================================================================

		internal static void StartTcpServer(out TcpServer server, out int localPort)
		{
			// Create server and initiate asych start.
			localPort = AvailableLocalTcpPort;
			server = new TcpServer(IPAddress.Loopback, localPort);
			if (!server.Start())
				Assert.Fail("TCP/IP Server could not be started!");
		}

		internal static void StartTcpClient(out TcpClient client, int remotePort)
		{
			// Create client and initiate asych start.
			client = new TcpClient(IPAddress.Loopback, remotePort);
			if (!client.Start())
				Assert.Fail("TCP/IP Client could not be started!");
		}

		internal static void StartTcpAutoSocketAsServer(out TcpAutoSocket autoSocket, out int localPort)
		{
			// Create auto socket and initiate asych start.
			localPort = AvailableLocalTcpPort;
			autoSocket = new TcpAutoSocket(IPAddress.Loopback, localPort, IPAddress.Any, localPort);
			if (!autoSocket.Start())
				Assert.Fail("TCP auto socket could not be started!");
		}

		internal static void StartTcpAutoSocketAsClient(out TcpAutoSocket autoSocket, int remotePort)
		{
			// Create auto socket and initiate asych start.
			autoSocket = new TcpAutoSocket(IPAddress.Loopback, remotePort, IPAddress.Any, remotePort);
			if (!autoSocket.Start())
				Assert.Fail("TCP auto socket could not be started!");
		}

		internal static void StartUdpSocket(out UdpSocket socket, int remotePort, int localPort)
		{
			// Create socket and initiate asych start.
			socket = new UdpSocket(IPAddress.Loopback, remotePort, localPort);
			if (!socket.Start())
				Assert.Fail("UDP socket could not be started!");
		}

		internal static void StopTcpServer(TcpServer server)
		{
			// Initiate async stop.
			server.Stop();
		}

		internal static void StopTcpClient(TcpClient client)
		{
			// Initiate async stop.
			client.Stop();
		}

		internal static void StopTcpAutoSocket(TcpAutoSocket autoSocket)
		{
			// Initiate async stop.
			autoSocket.Stop();
		}

		internal static void StopUdpSocket(UdpSocket socket)
		{
			// Initiate async stop.
			socket.Stop();
		}

		#endregion

		#region Wait
		//==========================================================================================
		// Wait
		//==========================================================================================

		internal static void WaitForStart(IO.Serial.IIOProvider io, string message)
		{
			int timeout = 0;
			do
			{
				Thread.Sleep(WaitInterval);
				timeout += WaitInterval;

				if (timeout >= WaitTimeout)
					Assert.Fail(message);
			}
			while (!io.IsStarted);
		}

		internal static void WaitForTcpAutoSocketToBeStartedAsServer(TcpAutoSocket io, string message)
		{
			int timeout = 0;
			do
			{
				Thread.Sleep(WaitInterval);
				timeout += WaitInterval;

				if (timeout >= WaitTimeout)
					Assert.Fail(message);
			}
			while (!(io.IsStarted && io.IsServer));
		}

		internal static void WaitForConnect(IO.Serial.IIOProvider ioA, IO.Serial.IIOProvider ioB, string message)
		{
			int timeout = 0;
			do
			{
				Thread.Sleep(WaitInterval);
				timeout += WaitInterval;

				if (timeout >= WaitTimeout)
					Assert.Fail(message);
			}
			while (!ioA.IsConnected && !ioB.IsConnected);
		}

		internal static void WaitForDisconnect(IO.Serial.IIOProvider ioA, IO.Serial.IIOProvider ioB, string message)
		{
			int timeout = 0;
			do
			{
				Thread.Sleep(WaitInterval);
				timeout += WaitInterval;

				if (timeout >= WaitTimeout)
					Assert.Fail(message);
			}
			while (ioA.IsConnected || ioB.IsConnected);
		}

		internal static void WaitForStop(IO.Serial.IIOProvider io, string message)
		{
			int timeout = 0;
			do
			{
				Thread.Sleep(WaitInterval);
				timeout += WaitInterval;

				if (timeout >= WaitTimeout)
					Assert.Fail(message);
			}
			while (io.IsStarted);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
