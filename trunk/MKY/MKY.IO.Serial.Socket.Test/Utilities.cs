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
// Copyright © 2007-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

using MKY.Net;

using NUnit.Framework;

#endregion

namespace MKY.IO.Serial.Socket.Test
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
	public static class Utilities
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <remarks>
		/// The start default value of 10000 corresponds to the YAT's default value.
		/// </remarks>
		internal const int StartOfPortRange = 10000;

		/// <remarks>
		/// The end default value of 49151 is the last port before the dynamic port range defined
		/// e.g. at https://docs.microsoft.com/en-us/troubleshoot/windows-server/networking/service-overview-and-network-port-requirements.
		/// </remarks>
		internal const int EndOfPortRange = 49151;

		/// <remarks>
		/// State changes on a <see cref="TcpAutoSocket"/> are the slowest, due
		/// to the nature of the <see cref="TcpAutoSocket"/> to try this and that.
		/// </remarks>                                      // Same as 'MKY.IO.Serial.Socket.TcpClient.DefaultConnectingTimeout'.
		private const int WaitTimeoutForStateChange = 5000; // Same as 'YAT.Domain.Test.Utilities.WaitTimeoutForStateChange'.

		/// <remarks>
		/// Note that a shorter interval would increase debug output, spoiling the debug console.
		/// </remarks>
		private const int WaitIntervalForStateChange = 100; // Same as 'YAT.Domain.Test.Utilities.WaitIntervalForStateChange'.

		#endregion

		#region Available Ports
		//==========================================================================================
		// Available Ports
		//==========================================================================================

		/// <summary>
		/// Gets an available local TCP port in the given range.
		/// </summary>
		/// <exception cref="OverflowException">
		/// No local TCP port available within given range.
		/// </exception>
		internal static int GetAvailableLocalTcpPort(int startAt = StartOfPortRange, int endAt = EndOfPortRange)
		{
			IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
			IPEndPoint[] listeners = properties.GetActiveTcpListeners();

			int port;
			for (port = startAt; port <= endAt; port++)
			{
				bool isActive = false;
				foreach (IPEndPoint ep in listeners)
				{
					if (ep.Port == port)
					{
						isActive = true;
						break;
					}
				}

				if (!isActive)
					return (port);
			}

			throw (new OverflowException("No local TCP port available within range of " + startAt + " through " + endAt + "!"));
		}

		/// <summary>
		/// Gets an available local TCP port in the given range.
		/// </summary>
		/// <exception cref="OverflowException">
		/// No local TCP port available within given range.
		/// </exception>
		internal static int GetAvailableLocalUdpPort(int startAt = StartOfPortRange, int endAt = EndOfPortRange)
		{
			IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
			IPEndPoint[] listeners = properties.GetActiveUdpListeners();

			int port;
			for (port = startAt; port <= endAt; port++)
			{
				bool isActive = false;
				foreach (IPEndPoint ep in listeners)
				{
					if (ep.Port == port)
					{
						isActive = true;
						break; // foreach (listeners)
					}
				}

				if (!isActive)
					return (port);
			}

			throw (new OverflowException("No local UDP port available within range of " + startAt + " through " + endAt + "!"));
		}

		/// <remarks>Separate method needed to ensure that two independent ports are found.</remarks>
		internal static void GetAvailableLocalUdpPorts(out int portA, out int portB, int startAt = StartOfPortRange, int endAt = EndOfPortRange)
		{
			portA = 0;
			portB = 0;

			IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
			IPEndPoint[] listeners = properties.GetActiveUdpListeners();

			int port;
			for (port = startAt; port <= endAt; port++)
			{
				bool isActive = false;
				foreach (IPEndPoint ep in listeners)
				{
					if (ep.Port == port)
					{
						isActive = true;
						break; // foreach (listeners)
					}
				}

				if (!isActive)
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

			throw (new OverflowException("No local UDP ports available within range of " + startAt + " through " + endAt + "!"));
		}

		#endregion

		#region Start/Stop
		//==========================================================================================
		// Start/Stop
		//==========================================================================================

		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		internal static void CreateAndStartAsync(out TcpServer server, out int localPort, int connectionAllowance = TcpServer.ConnectionAllowanceDefault, int startAt = StartOfPortRange, int endAt = EndOfPortRange)
		{
			localPort = GetAvailableLocalTcpPort(startAt, endAt);
			server = new TcpServer(IPNetworkInterface.Any, localPort, connectionAllowance);

			StartAsync(server);
		}

		internal static void StartAsync(TcpServer server)
		{
			if (!server.Start()) // <= is initiated async!
				Assert.Fail("TCP/IP server could not be started!");
		}

		internal static void CreateAndStartAsync(out TcpClient client, int remotePort)
		{
			client = new TcpClient(IPHost.Localhost, remotePort, IPNetworkInterface.Any);

			StartAsync(client);
		}

		internal static void CreateAndStartAsync(out TcpClient client, int remotePort, IntervalSettingTuple autoReconnect)
		{
			client = new TcpClient(IPHost.Localhost, remotePort, IPNetworkInterface.Any, autoReconnect);

			StartAsync(client);
		}

		internal static void StartAsync(TcpClient client)
		{
			if (!client.Start()) // <= is initiated async!
				Assert.Fail("TCP/IP client could not be started!");
		}

		internal static void CreateAndStartAsyncAsServer(out TcpAutoSocket autoSocket, out int localPort, int startAt = StartOfPortRange, int endAt = EndOfPortRange)
		{
			localPort = GetAvailableLocalTcpPort(startAt, endAt);
			autoSocket = new TcpAutoSocket(IPHost.Localhost, localPort, IPNetworkInterface.Any, localPort);

			StartAsyncAsServer(autoSocket);
		}

		internal static void StartAsyncAsServer(TcpAutoSocket autoSocket)
		{
			if (!autoSocket.Start()) // <= is initiated async!
				Assert.Fail("TCP/IP AutoSocket could not be started!");
		}

		internal static void CreateAndStartAsyncAsClient(out TcpAutoSocket autoSocket, int remotePort)
		{
			autoSocket = new TcpAutoSocket(IPHost.Localhost, remotePort, IPNetworkInterface.Any, remotePort);

			StartAsyncAsClient(autoSocket);
		}

		internal static void StartAsyncAsClient(TcpAutoSocket autoSocket)
		{
			if (!autoSocket.Start()) // <= is initiated async!
				Assert.Fail("TCP/IP AutoSocket could not be started!");
		}

		internal static void CreateAndStartAsServer(out UdpSocket server, int localPort)
		{
			server = new UdpSocket(IPNetworkInterface.Any, localPort);

			Start(server);
		}

		internal static void CreateAndStartAsClient(out UdpSocket client, int remotePort)
		{
			client = new UdpSocket(IPHost.Localhost, remotePort);

			Start(client);
		}

		internal static void CreateAndStartAsPairSocket(out UdpSocket pairSocket, int remotePort, int localPort)
		{
			pairSocket = new UdpSocket(IPHost.Localhost, remotePort, IPNetworkInterface.Any, localPort);

			Start(pairSocket);
		}

		internal static void Start(UdpSocket socket)
		{
			if (!socket.Start())
				Assert.Fail("UDP/IP socket could not be started!");
		}

		internal static void StopAsync(TcpServer server)
		{
			server.Stop(); // <= is initiated async!
		}

		internal static void StopAsync(TcpClient client)
		{
			client.Stop(); // <= is initiated async!
		}

		internal static void StopAsync(TcpAutoSocket autoSocket)
		{
			autoSocket.Stop(); // <= is initiated async!
		}

		internal static void Stop(UdpSocket socket)
		{
			socket.Stop();
		}

		#endregion

		#region Wait
		//==========================================================================================
		// Wait
		//==========================================================================================

		/// <remarks>
		/// There are similar utility methods in
		/// 'YAT.Domain.Test.Utilities' and
		/// 'YAT.Model.Test.Utilities'.
		/// Changes here may have to be applied there too.
		/// </remarks>
		internal static void WaitForStart(IIOProvider io, string message)
		{
			int waitTime = 0;

			Trace.WriteLine("Waiting for start, 0 ms have passed, time-out is " + WaitTimeoutForStateChange + " ms...");

			while (!io.IsStarted)
			{
				Thread.Sleep(WaitIntervalForStateChange);
				waitTime += WaitIntervalForStateChange;

				Trace.WriteLine("Waiting for start, " + waitTime + " ms have passed, time-out is " + WaitTimeoutForStateChange + " ms...");

				if (waitTime >= WaitTimeoutForStateChange)
					Assert.Fail("Start timeout! " + message);
			}

			Trace.WriteLine("...done, started");
		}

		internal static void WaitForTcpAutoSocketToBeStartedAsServer(TcpAutoSocket io, string message)
		{
			int waitTime = 0;

			Trace.WriteLine("Waiting for start as server, 0 ms have passed, time-out is " + WaitTimeoutForStateChange + " ms...");

			while (!(io.IsStarted && io.IsServer))
			{
				Thread.Sleep(WaitIntervalForStateChange);
				waitTime += WaitIntervalForStateChange;

				Trace.WriteLine("Waiting for start as server, " + waitTime + " ms have passed, time-out is " + WaitTimeoutForStateChange + " ms...");

				if (waitTime >= WaitTimeoutForStateChange)
					Assert.Fail("Start timeout! " + message);
			}

			Trace.WriteLine("...done, started as server");
		}

		/// <remarks>
		/// There are similar utility methods in
		/// 'YAT.Domain.Test.Utilities' and
		/// 'YAT.Model.Test.Utilities'.
		/// Changes here may have to be applied there too.
		/// </remarks>
		internal static void WaitForConnection(IIOProvider ioA, IIOProvider ioB, string message)
		{
			int waitTime = 0;

			Trace.WriteLine("Waiting for connection, 0 ms have passed, time-out is " + WaitTimeoutForStateChange + " ms...");

			while (!(ioA.IsConnected && ioB.IsConnected))
			{
				Thread.Sleep(WaitIntervalForStateChange);
				waitTime += WaitIntervalForStateChange;

				Trace.WriteLine("Waiting for connection, " + waitTime + " ms have passed, time-out is " + WaitTimeoutForStateChange + " ms...");

				if (waitTime >= WaitTimeoutForStateChange)
					Assert.Fail("Connect timeout! " + message);
			}

			Trace.WriteLine("...done, connected");
		}

		/// <remarks>
		/// There are similar utility methods in
		/// 'YAT.Domain.Test.Utilities' and
		/// 'YAT.Model.Test.Utilities'.
		/// Changes here may have to be applied there too.
		/// </remarks>
		internal static void WaitForDisconnection(IIOProvider ioA, IIOProvider ioB, string message)
		{
			int waitTime = 0;

			Trace.WriteLine("Waiting for disconnection, 0 ms have passed, time-out is " + WaitTimeoutForStateChange + " ms...");

			while (ioA.IsConnected || ioB.IsConnected)
			{
				Thread.Sleep(WaitIntervalForStateChange);
				waitTime += WaitIntervalForStateChange;

				Trace.WriteLine("Waiting for disconnection, " + waitTime + " ms have passed, time-out is " + WaitTimeoutForStateChange + " ms...");

				if (waitTime >= WaitTimeoutForStateChange)
					Assert.Fail("Disconnect timeout! " + message);
			}

			Trace.WriteLine("...done, disconnected");
		}

		/// <remarks>
		/// There are similar utility methods in
		/// 'YAT.Domain.Test.Utilities' and
		/// 'YAT.Model.Test.Utilities'.
		/// Changes here may have to be applied there too.
		/// </remarks>
		internal static void WaitForStop(IIOProvider io, string message)
		{
			int waitTime = 0;

			Trace.WriteLine("Waiting for stop, 0 ms have passed, time-out is " + WaitTimeoutForStateChange + " ms...");

			while (!io.IsStopped)
			{
				Thread.Sleep(WaitIntervalForStateChange);
				waitTime += WaitIntervalForStateChange;

				Trace.WriteLine("Waiting for stop, " + waitTime + " ms have passed, time-out is " + WaitTimeoutForStateChange + " ms...");

				if (waitTime >= WaitTimeoutForStateChange)
					Assert.Fail("Stop timeout! " + message);
			}

			Trace.WriteLine("...done, stopped");
		}

		#endregion

		#region Assert
		//==========================================================================================
		// Assert
		//==========================================================================================

		internal static void AssertStartedAndDisconnected(IIOProvider io)
		{
			Assert.That(io.IsStarted);
			Assert.That(io.IsOpen,         Is.False);
			Assert.That(io.IsConnected,    Is.False);
			Assert.That(io.IsTransmissive, Is.False);
		}

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
			Assert.That(io.IsStopped);
			Assert.That(io.IsOpen,         Is.False);
			Assert.That(io.IsConnected,    Is.False);
			Assert.That(io.IsTransmissive, Is.False);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
