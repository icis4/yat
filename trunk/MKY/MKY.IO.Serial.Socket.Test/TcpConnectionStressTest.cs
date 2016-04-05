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
// MKY Development Version 1.0.14
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;

using NUnit;
using NUnit.Framework;

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
		[StressCategory]
		public virtual void Stress1AutoSocketServerWith10AutoSocketClients()
		{
			StressAutoSocketServersWithAutoSocketClients(1, 10);
		}

		/// <summary></summary>
		[Test]
		[StressCategory, MinuteDurationCategory(1)]
		public virtual void Stress10AutoSocketServersWith100AutoSocketClients()
		{
			StressAutoSocketServersWithAutoSocketClients(10, 100);
		}

		/// <summary></summary>
		[Test]
		[StressCategory, MinuteDurationCategory(10)]
		public virtual void Stress100AutoSocketServersWith1000AutoSocketClients()
		{
			StressAutoSocketServersWithAutoSocketClients(100, 1000);
		}

		private static void StressAutoSocketServersWithAutoSocketClients(int numberOfServers, int numberOfClients)
		{
			List<int> serverPorts = new List<int>();
			List<TcpAutoSocket> serverSockets = new List<TcpAutoSocket>();
			List<TcpAutoSocket> clientSockets = new List<TcpAutoSocket>();

			// Create a large number of AutoSockets.
			for (int i = 0; i < numberOfServers; i++)
			{
				int p;
				TcpAutoSocket s;
				Utilities.StartTcpAutoSocketAsServer(out s, out p);
				Utilities.WaitForTcpAutoSocketToBeStartedAsServer(s, "TCP/IP AutoSocket " + i + " could not be started as server!");
				serverSockets.Add(s);
				serverPorts.Add(p);
			}

			// Randomly connect another large numer of AutoSockets to the existing sockets.
			Random r = new Random(RandomEx.NextPseudoRandomSeed());
			for (int i = 0; i < numberOfClients; i++)
			{
				int j = r.Next(0, (numberOfServers - 1));
				int p = serverPorts[j];
				TcpAutoSocket s = serverSockets[j];
				TcpAutoSocket c;
				Utilities.StartTcpAutoSocketAsClient(out c, p);
				Utilities.WaitForStart(c, "TCP/IP AutoSocket " + i + " could not be started!");
				Utilities.WaitForConnect(c, s, "TCP/IP AutoSocket " + i + " could not be connected to AutoSocket " + s);
				clientSockets.Add(c);
			}

			// Shutdown all client sockets.
			foreach (TcpAutoSocket c in clientSockets)
			{
				Utilities.StopTcpAutoSocket(c);
				Utilities.WaitForStop(c, "TCP/IP AutoSocket as client could not be stopped!");
			}

			// Shutdown all server sockets.
			foreach (TcpAutoSocket s in serverSockets)
			{
				Utilities.StopTcpAutoSocket(s);
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
