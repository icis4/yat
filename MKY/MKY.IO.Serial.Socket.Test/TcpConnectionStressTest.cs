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
// Copyright © 2007-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;

using NUnit.Framework;
using NUnitEx;

namespace MKY.IO.Serial.Socket.Test
{
	/// <summary></summary>
	[TestFixture]
	public class TcpConnectionStressTest
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
			List<int> serverPorts = new List<int>(numberOfServers);
			List<TcpAutoSocket> serverSockets = new List<TcpAutoSocket>(numberOfServers); // Preset the required capacity to improve memory management.

			// Create a large number of AutoSockets.
			for (int i = 0; i < numberOfServers; i++)
			{
				int p;
				TcpAutoSocket s;
				Utilities.CreateAndStartAsyncAsServer(out s, out p);
				Utilities.WaitForTcpAutoSocketToBeStartedAsServer(s, "TCP/IP AutoSocket " + i + " could not be started as server!");
				serverSockets.Add(s);
				serverPorts.Add(p);
			}

			// Randomly connect another large numer of AutoSockets to the existing sockets.
			Random r = new Random(RandomEx.NextPseudoRandomSeed());
			List<TcpAutoSocket> clientSockets = new List<TcpAutoSocket>(numberOfClients); // Preset the required capacity to improve memory management.
			for (int i = 0; i < numberOfClients; i++)
			{
				int j = r.Next(0, (numberOfServers - 1));
				int p = serverPorts[j];
				TcpAutoSocket s = serverSockets[j];
				TcpAutoSocket c;
				Utilities.CreateAndStartAsyncAsClient(out c, p);
				Utilities.WaitForStart(c, "TCP/IP AutoSocket " + i + " could not be started!");
				Utilities.WaitForConnect(c, s, "TCP/IP AutoSocket " + i + " could not be connected to AutoSocket " + s);
				clientSockets.Add(c);
			}

			// Shutdown all client sockets.
			foreach (TcpAutoSocket c in clientSockets)
			{
				Utilities.StopAsync(c);
				Utilities.WaitForStop(c, "TCP/IP AutoSocket as client could not be stopped!");
			}

			// Shutdown all server sockets.
			foreach (TcpAutoSocket s in serverSockets)
			{
				Utilities.StopAsync(s);
				Utilities.WaitForStop(s, "TCP/IP AutoSocket as client could not be stopped!");
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
