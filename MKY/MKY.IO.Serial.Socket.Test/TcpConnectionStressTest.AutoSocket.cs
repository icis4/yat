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

		#region Tests > StressAutoSocket()
		//------------------------------------------------------------------------------------------
		// Tests > StressAutoSocket()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		[StressCategory, StandardDurationCategory.Seconds4]
		public virtual void Stress1AutoSocketServerWith10AutoSocketClients()
		{
			StressAutoSocketServersWithAutoSocketClients(1, 10);
		}

		/// <summary></summary>
		[Test]
		[StressCategory, StandardDurationCategory.Minute1]
		public virtual void Stress10AutoSocketServersWith100AutoSocketClients()
		{
			StressAutoSocketServersWithAutoSocketClients(10, 100);
		}

		/// <summary></summary>
		[Test]
		[StressCategory, StandardDurationCategory.Minutes15]
		public virtual void Stress100AutoSocketServersWith1000AutoSocketClients()
		{
			StressAutoSocketServersWithAutoSocketClients(100, 1000);
		}

		private static void StressAutoSocketServersWithAutoSocketClients(int numberOfServers, int numberOfClients)
		{
			var serverSockets = new List<TcpAutoSocket>(numberOfServers); // Preset the required capacity to improve memory management.
			var clientSockets = new List<TcpAutoSocket>(numberOfClients); // Preset the required capacity to improve memory management.

			try
			{
				Trace.WriteLine("Creating " + numberOfServers + " AutoSocket(s) as server...");
				var serverPorts = new List<int>(numberOfServers);
				var portToStartAt = Utilities.StartOfPortRange;
				for (int i = 0; i < numberOfServers; i++)
				{
					int p;
					TcpAutoSocket s;
					Utilities.CreateAndStartAsyncAsServer(out s, out p, portToStartAt);
					serverSockets.Add(s); // Immediately add to ensure disposal even in case of assertion/failure!

					if (p < Utilities.EndOfPortRange)
						portToStartAt = (p + 1);
					else
						throw (new OverflowException("No more local TCP port available within range of " + Utilities.StartOfPortRange + " through " + Utilities.EndOfPortRange + "!"));

					Utilities.WaitForTcpAutoSocketToBeStartedAsServer(s, "TCP/IP AutoSocket " + i + " could not be started as server!");
					serverPorts.Add(p);
				}

				Trace.WriteLine("Connecting " + numberOfClients + " AutoSocket(s) randomly to server(s)...");
				var random = new Random(RandomEx.NextRandomSeed());
				var clientCounts = new int[numberOfServers];
				for (int i = 0; i < numberOfClients; i++)
				{
					int j = random.Next(0, numberOfServers); // Not (numberOfServers - 1) as Next() returns "greater than or equal to minValue and less than maxValue".
					int p = serverPorts[j];
					TcpAutoSocket s = serverSockets[j];
					TcpAutoSocket c;
					Utilities.CreateAndStartAsyncAsClient(out c, p);

					clientSockets.Add(c); // Immediately add to ensure disposal even in case of assertion/failure!
					clientCounts[j]++;

					Utilities.WaitForStart(c, "TCP/IP AutoSocket " + i + " could not be started!");
					Utilities.WaitForConnection(c, s, "TCP/IP AutoSocket " + i + " could not be connected to AutoSocket " + s);
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
					Utilities.WaitForStop(c, "TCP/IP AutoSocket as client could not be stopped!");
				}

				Trace.WriteLine("Stopping server(s)...");
				foreach (var s in serverSockets)
				{
					Utilities.StopAsync(s);
					Utilities.WaitForStop(s, "TCP/IP AutoSocket as server could not be stopped!");
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
