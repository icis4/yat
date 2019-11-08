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
// MKY Version 1.0.28 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

using MKY.Net;
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

		internal static int AvailableLocalUdpPort
		{
			get
			{
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
						return (port);
				}

				throw (new OverflowException("No local UDP ports available within range of 10000 through " + IPEndPoint.MaxPort + "!"));
			}
		}

		/// <remarks>Separate method needed to ensure that two separate ports are found.</remarks>
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
			localPort = AvailableLocalTcpPort;
			server = new TcpServer(IPNetworkInterface.Any, localPort);
			if (!server.Start()) // <= is initiated async!
				Assert.Fail("TCP/IP server could not be started!");
		}

		internal static void StartTcpClient(out TcpClient client, int remotePort)
		{
			client = new TcpClient(IPHost.Localhost, remotePort, IPNetworkInterface.Any);
			if (!client.Start()) // <= is initiated async!
				Assert.Fail("TCP/IP client could not be started!");
		}

		internal static void StartTcpAutoSocketAsServer(out TcpAutoSocket autoSocket, out int localPort)
		{
			localPort = AvailableLocalTcpPort;
			autoSocket = new TcpAutoSocket(IPHost.Localhost, localPort, IPNetworkInterface.Any, localPort);
			if (!autoSocket.Start()) // <= is initiated async!
				Assert.Fail("TCP/IP AutoSocket could not be started!");
		}

		internal static void StartTcpAutoSocketAsClient(out TcpAutoSocket autoSocket, int remotePort)
		{
			autoSocket = new TcpAutoSocket(IPHost.Localhost, remotePort, IPNetworkInterface.Any, remotePort);
			if (!autoSocket.Start()) // <= is initiated async!
				Assert.Fail("TCP/IP AutoSocket could not be started!");
		}

		internal static void StartUdpServer(out UdpSocket server, int localPort)
		{
			server = new UdpSocket(IPNetworkInterface.Any, localPort);
			if (!server.Start())
				Assert.Fail("UDP/IP server could not be started!");
		}

		internal static void StartUdpClient(out UdpSocket client, int remotePort)
		{
			client = new UdpSocket(IPHost.Localhost, remotePort);
			if (!client.Start())
				Assert.Fail("UDP/IP client could not be started!");
		}

		internal static void StartUdpPairSocket(out UdpSocket pairSocket, int remotePort, int localPort)
		{
			pairSocket = new UdpSocket(IPHost.Localhost, remotePort, IPNetworkInterface.Any, localPort);
			if (!pairSocket.Start())
				Assert.Fail("UDP/IP PairSocket could not be started!");
		}

		internal static void StopTcpServer(TcpServer server)
		{
			server.Stop(); // <= is initiated async!
		}

		internal static void StopTcpClient(TcpClient client)
		{
			client.Stop(); // <= is initiated async!
		}

		internal static void StopTcpAutoSocket(TcpAutoSocket autoSocket)
		{
			autoSocket.Stop(); // <= is initiated async!
		}

		internal static void StopUdpSocket(UdpSocket socket)
		{
			socket.Stop();
		}

		#endregion

		#region Wait
		//==========================================================================================
		// Wait
		//==========================================================================================

		internal static void WaitForStart(IIOProvider io, string message)
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

		internal static void WaitForConnect(IIOProvider ioA, IIOProvider ioB, string message)
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

		internal static void WaitForDisconnect(IIOProvider ioA, IIOProvider ioB, string message)
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

		internal static void WaitForStop(IIOProvider io, string message)
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

		#region Assert
		//==========================================================================================
		// Assert
		//==========================================================================================

		internal static void AssertStartedAndConnected(IIOProvider io)
		{
			Assert.That(io.IsStarted);
			Assert.That(io.IsOpen);
			Assert.That(io.IsConnected);
		}

		internal static void AssertStartedAndTransmissive(IIOProvider io)
		{
			Assert.That(io.IsStarted);
			Assert.That(io.IsOpen);
			Assert.That(io.IsConnected);
			Assert.That(io.IsTransmissive);
		}

		internal static void AssertStopped(IIOProvider io)
		{
			Assert.That( io.IsStopped);
			Assert.That(!io.IsOpen);
			Assert.That(!io.IsConnected);
			Assert.That(!io.IsTransmissive);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
