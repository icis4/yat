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
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

using NUnit;
using NUnit.Framework;

using MKY.IO.Serial;

namespace MKY.IO.Serial.Test.Socket
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
		[StressCategory, MinuteDurationCategory(1)]
		public virtual void StressAutoSocket()
		{
			List<int> serverPorts = new List<int>();
			List<TcpAutoSocket> serverSockets = new List<TcpAutoSocket>();
			List<TcpAutoSocket> clientSockets = new List<TcpAutoSocket>();

			// Create a large number of auto sockets.
			for (int i = 0; i < 100; i++)
			{
				int p;
				TcpAutoSocket s;
				Utilities.StartTcpAutoSocketAsServer(out s, out p);
				Utilities.WaitForStart(s, "TCP auto socket " + i + " could not be started as server!");
				serverSockets.Add(s);
				serverPorts.Add(p);
			}

			// Randomly connect another large numer of auto sockets to the existing sockets.
			Random r = new Random();
			for (int i = 0; i < 500; i++)
			{
				int j = r.Next(99);
				int p = serverPorts[j];
				TcpAutoSocket s = serverSockets[j];
				TcpAutoSocket c;
				Utilities.StartTcpAutoSocketAsClient(out c, p);
				Utilities.WaitForStart(c, "TCP auto socket " + i + " could not be started as client!");
				Utilities.WaitForConnect(c, s, "TCP auto socket " + i + " could not be connected to auto socket " + s);
				clientSockets.Add(c);
			}

			// Shutdown all client sockets.
			foreach (TcpAutoSocket c in clientSockets)
			{
				Utilities.StopTcpAutoSocket(c);
				Utilities.WaitForStop(c, "TCP auto socket as client could not be stopped!");
			}

			// Shutdown all server sockets.
			foreach (TcpAutoSocket s in serverSockets)
			{
				Utilities.StopTcpAutoSocket(s);
				Utilities.WaitForStop(s, "TCP auto socket as client could not be stopped!");
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
