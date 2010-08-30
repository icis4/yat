//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

using NUnit.Framework;

namespace MKY.IO.Serial.Test.Socket
{
	/// <summary></summary>
	public static class Utilities
	{
		#region Private Constants
		//==========================================================================================
		// Private Constants
		//==========================================================================================

		private const int Interval = 100;
		private const int Timeout = 10000;

		#endregion

		#region Private Properties
		//==========================================================================================
		// Private Properties
		//==========================================================================================

		static private int AvailableLocalTcpPort
		{
			get
			{
				IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
				IPEndPoint[] listeners = properties.GetActiveTcpListeners();

				int port;
				for (port = 10000; port <= 65535; port++)
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

				throw (new OverflowException("No local TCP port available within range of 10000 through 65535"));
			}
		}

		#endregion

		#region Start/Stop
		//==========================================================================================
		// Start/Stop
		//==========================================================================================

		internal static void StartServer(out TcpServer server, out int localPort)
		{
			// Create server and initiate asych start.
			localPort = AvailableLocalTcpPort;
			server = new TcpServer(IPAddress.Loopback, localPort);
			if (!server.Start())
				Assert.Fail("TCP server could not be started");
		}

		internal static void StartClientAndConnect(out TcpClient client, int remotePort)
		{
			// Create client and initiate asych start.
			client = new TcpClient(IPAddress.Loopback, remotePort);
			if (!client.Start())
				Assert.Fail("TCP client could not be started");
		}

		internal static void StartAutoSocketAsServer(out TcpAutoSocket autoSocket, out int localPort)
		{
			// Create auto socket and initiate asych start.
			localPort = AvailableLocalTcpPort;
			autoSocket = new TcpAutoSocket(IPAddress.Loopback, localPort, IPAddress.Any, localPort);
			if (!autoSocket.Start())
				Assert.Fail("TCP auto socket could not be started");
		}

		internal static void StartAutoSocketAsClient(out TcpAutoSocket autoSocket, int remotePort)
		{
			// Create auto socket and initiate asych start.
			autoSocket = new TcpAutoSocket(IPAddress.Loopback, remotePort, IPAddress.Any, remotePort);
			if (!autoSocket.Start())
				Assert.Fail("TCP auto socket could not be started");
		}

		internal static void StopServer(TcpServer server)
		{
			// Initiate async stop.
			server.Stop();
		}

		internal static void StopClient(TcpClient client)
		{
			// Initiate async stop.
			client.Stop();
		}

		internal static void StopAutoSocket(TcpAutoSocket autoSocket)
		{
			// Initiate async stop.
			autoSocket.Stop();
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
				Thread.Sleep(Interval);
				timeout += Interval;

				if (timeout >= Timeout)
					Assert.Fail(message);
			}
			while (!io.IsStarted);
		}

		internal static void WaitForConnect(IO.Serial.IIOProvider ioA, IO.Serial.IIOProvider ioB, string message)
		{
			int timeout = 0;
			do
			{
				Thread.Sleep(Interval);
				timeout += Interval;

				if (timeout >= Timeout)
					Assert.Fail(message);
			}
			while (!ioA.IsConnected && !ioB.IsConnected);
		}

		internal static void WaitForDisconnect(IO.Serial.IIOProvider ioA, IO.Serial.IIOProvider ioB, string message)
		{
			int timeout = 0;
			do
			{
				Thread.Sleep(Interval);
				timeout += Interval;

				if (timeout >= Timeout)
					Assert.Fail(message);
			}
			while (ioA.IsConnected || ioB.IsConnected);
		}

		internal static void WaitForStop(IO.Serial.IIOProvider io, string message)
		{
			int timeout = 0;
			do
			{
				Thread.Sleep(Interval);
				timeout += Interval;

				if (timeout >= Timeout)
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
