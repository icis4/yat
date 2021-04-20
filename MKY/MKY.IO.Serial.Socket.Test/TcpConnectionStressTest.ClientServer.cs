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
// MKY Version 1.0.30
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

using System;
using System.Collections.Generic;
using System.Diagnostics;

using NUnit.Framework;
using NUnitEx;

namespace MKY.IO.Serial.Socket.Test
{
	/// <remarks>
	/// Implemented as partial class to easy diffing among variants.
	/// </remarks>
	[TestFixture]
	public partial class TcpConnectionStressTest
	{
		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		#region Tests > ClientServerStress()
		//------------------------------------------------------------------------------------------
		// Tests > ClientServerStress()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		[StressCategory, StandardDurationCategory.Seconds4]
		[Combinatorial]
		public virtual void Stress1ServerWith10Clients([Values(false, true)] bool autoReconnectEnabled) // [Values(bool)] to be simplified when upgrading to NUnit 3.x (FR #293).
		{
			StressServersWithClients(1, 10, false);
		}

		/// <summary></summary>
		[Test]
		[StressCategory, StandardDurationCategory.Minute1]
		[Combinatorial]
		public virtual void Stress10ServersWith100Clients([Values(false, true)] bool autoReconnectEnabled) // [Values(bool)] to be simplified when upgrading to NUnit 3.x (FR #293).
		{
			StressServersWithClients(10, 100, false);
		}

		/// <summary></summary>
		[Test]
		[StressCategory, StandardDurationCategory.Minutes15]
		[Combinatorial]
		public virtual void Stress100ServersWith1000Clients([Values(false, true)] bool autoReconnectEnabled) // [Values(bool)] to be simplified when upgrading to NUnit 3.x (FR #293).
		{
			StressServersWithClients(100, 1000, autoReconnectEnabled);
		}

		private static void StressServersWithClients(int numberOfServers, int numberOfClients, bool autoReconnectEnabled)
		{
			var serverSockets = new List<TcpServer>(numberOfServers); // Preset the required capacity to improve memory management.
			var clientSockets = new List<TcpClient>(numberOfClients); // Preset the required capacity to improve memory management.

			try
			{
				Trace.WriteLine("Creating " + numberOfServers + " server(s)...");
				var serverPorts = new List<int>(numberOfServers);
				var portToStartAt = Utilities.StartOfPortRange;
				for (int i = 0; i < numberOfServers; i++)
				{
					int p;
					TcpServer s;
					Utilities.CreateAndStartAsync(out s, out p, portToStartAt);
					serverSockets.Add(s); // Immediately add to ensure disposal even in case of assertion/failure!

					if (p < Utilities.EndOfPortRange)
						portToStartAt = (p + 1);
					else
						throw (new OverflowException("No more local TCP port available within range of " + Utilities.StartOfPortRange + " through " + Utilities.EndOfPortRange + "!"));

					Utilities.WaitForStart(s, "TCP/IP server " + i + " could not be started as server!");
					serverPorts.Add(p);
				}

				Trace.WriteLine("Connecting " + numberOfClients + " client(s) randomly to server(s)...");
				var random = new Random(RandomEx.NextRandomSeed());
				var clientCounts = new int[numberOfServers];
				for (int i = 0; i < numberOfClients; i++)
				{
					int j = random.Next(0, numberOfServers); // Not (numberOfServers - 1) as Next() returns "greater than or equal to minValue and less than maxValue".
					int p = serverPorts[j];
					TcpServer s = serverSockets[j];
					TcpClient c;
					if (!autoReconnectEnabled)
						Utilities.CreateAndStartAsync(out c, p);
					else
						Utilities.CreateAndStartAsync(out c, p, SocketSettings.TcpClientAutoReconnectDefault);

					clientSockets.Add(c); // Immediately add to ensure disposal even in case of assertion/failure!
					clientCounts[j]++;

					Utilities.WaitForStart(c, "TCP/IP client " + i + " could not be started!");
					Utilities.WaitForConnection(c, s, "TCP/IP client " + i + " could not be connected to " + s);
				}

				Trace.WriteLine("Asserting that client(s) are still transmissive...");
				foreach (var c in clientSockets)
				{
					Utilities.AssertStartedAndTransmissive(c);

					Assert.That(c.ConnectionCount, Is.EqualTo(1));
				}

				Trace.WriteLine("Asserting that server(s) are still transmissive...");
				for (int i = 0; i < numberOfServers; i++)
				{
					var s = serverSockets[i];
					Utilities.AssertStartedAndTransmissive(s);

					var clientCount = clientCounts[i];
					Assert.That(s.ConnectionCount, Is.EqualTo(clientCount));
				}

				Trace.WriteLine("Stopping client(s)...");
				foreach (var c in clientSockets)
				{
					Utilities.StopAsync(c);
					Utilities.WaitForStop(c, "TCP/IP client could not be stopped!");
				}

				Trace.WriteLine("Stopping server(s)...");
				foreach (var s in serverSockets)
				{
					Utilities.StopAsync(s);
					Utilities.WaitForStop(s, "TCP/IP server could not be stopped!");
				}
			}
			finally // Ensure to dispose of sockets in any case!
			{
				Trace.WriteLine("Disposing client(s)...");
				foreach (var c in clientSockets)
				{
					c.Dispose();
				}

				Trace.WriteLine("Disposing server(s)...");
				foreach (var s in serverSockets)
				{
					s.Dispose();
				}
			}
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
