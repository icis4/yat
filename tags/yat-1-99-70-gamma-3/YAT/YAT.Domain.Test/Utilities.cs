﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 3 Version 1.99.70
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2017 Matthias Kläy.
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
using System.Diagnostics.CodeAnalysis;
using System.Threading;

using MKY.Net;

using NUnit.Framework;

using YAT.Domain.Settings;

#endregion

namespace YAT.Domain.Test
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
	public static class Utilities
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <summary></summary>
		public const int WaitTimeoutForConnectionChange = 3000;

		/// <remarks>
		/// Timeout of 200 ms is a bit too short, especially when debugger is connected, e.g.
		///  > SingleLine often takes longer than 200 ms.
		///  > DoubleLine often takes longer than 400 ms.
		///  > TripleLine takes around 500 ms (where timeout is 600 ms).
		///  > MultiLine takes around 4000..5000 ms (where timeout is 5200 ms).
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'ms' is the proper abbreviation for milliseconds but StyleCop isn't able to deal with such abbreviations...")]
		public const int WaitTimeoutForLineTransmission = 300;

		/// <summary></summary>
		public const int WaitInterval = 100;

		#endregion

		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

		internal static TerminalSettings GetTextTcpAutoSocketOnIPv4LoopbackSettings()
		{
			return (GetTextTcpAutoSocketSettings(IPNetworkInterface.IPv4Loopback));
		}

		internal static TerminalSettings GetTextTcpAutoSocketSettings(IPNetworkInterfaceEx networkInterface)
		{
			TerminalSettings settings = new TerminalSettings();
			settings.TerminalType = TerminalType.Text;
			settings.IO.IOType = IOType.TcpAutoSocket;
			settings.IO.Socket.LocalInterface = networkInterface;
			return (settings);
		}

		#endregion

		#region Wait
		//==========================================================================================
		// Wait
		//==========================================================================================

		/// <remarks>
		/// There are similar utility methods in 'Model.Test.Utilities'.
		/// Changes here may have to be applied there too.
		/// </remarks>
		internal static void WaitForConnection(Terminal terminalA, Terminal terminalB)
		{
			int waitTime = 0;
			do                         // Initially wait to allow async send,
			{                          //   therefore, use do-while.
				Thread.Sleep(WaitInterval);
				waitTime += WaitInterval;

				Console.Out.WriteLine("Waiting for connection, " + waitTime + " ms have passed, timeout is " + WaitTimeoutForConnectionChange + " ms...");

				if (waitTime >= WaitTimeoutForConnectionChange)
					Assert.Fail("Connect timeout!");
			}
			while (!terminalA.IsConnected && !terminalB.IsConnected);

			Console.Out.WriteLine("...done");
		}

		/// <remarks>
		/// There are similar utility methods in 'Model.Test.Utilities'.
		/// Changes here may have to be applied there too.
		/// </remarks>
		internal static void WaitForDisconnection(Terminal terminal)
		{
			int waitTime = 0;
			while (terminal.IsConnected)
			{
				Thread.Sleep(WaitInterval);
				waitTime += WaitInterval;

				Console.Out.WriteLine("Waiting for disconnection, " + waitTime + " ms have passed, timeout is " + WaitTimeoutForConnectionChange + " ms...");

				if (waitTime >= WaitTimeoutForConnectionChange)
					Assert.Fail("Disconnect timeout!");
			}

			Console.Out.WriteLine("...done");
		}

		/// <remarks>
		/// There are similar utility methods in 'Model.Test.Utilities'.
		/// Changes here may have to be applied there too.
		/// </remarks>
		internal static void WaitForTransmission(Terminal terminalTx, Terminal terminalRx, int currentLineCount, int expectedTotalLineCount)
		{
			// Calculate timeout:
			int timeout = (WaitTimeoutForLineTransmission * currentLineCount);

			int txLineCount = 0;
			int rxLineCount = 0;
			int waitTime = 0;
			do                         // Initially wait to allow async send,
			{                          //   therefore, use do-while.
				Thread.Sleep(WaitInterval);
				waitTime += WaitInterval;

				Console.Out.WriteLine("Waiting for transmission, " + waitTime + " ms have passed, timeout is " + timeout + " ms...");

				if (waitTime >= timeout)
					Assert.Fail("Transmission timeout! Not enough lines received within expected interval.");

				txLineCount = terminalTx.GetRepositoryLineCount(RepositoryType.Tx);
				if (txLineCount > expectedTotalLineCount) // Break in case of too much data to improve speed of test.
					Assert.Fail("Transmission error!" +
						" Number of received lines = " + txLineCount +
						" mismatches expected = " + expectedTotalLineCount + ".");

				rxLineCount = terminalRx.GetRepositoryLineCount(RepositoryType.Rx);
				if (rxLineCount > expectedTotalLineCount) // Break in case of too much data to improve speed of test.
					Assert.Fail("Transmission error!" +
						" Number of received lines = " + rxLineCount +
						" mismatches expected = " + expectedTotalLineCount + ".");
			}
			while ((txLineCount != expectedTotalLineCount) &&
			       (rxLineCount != expectedTotalLineCount));

			Console.Out.WriteLine("...done");
		}

		/// <remarks>
		/// There are similar utility methods in 'Model.Test.Utilities'.
		/// Changes here may have to be applied there too.
		/// </remarks>
		internal static void WaitForSending(Terminal terminalTx, int currentLineCount, int expectedTotalLineCount)
		{
			// Calculate timeout:
			int timeout = (WaitTimeoutForLineTransmission * currentLineCount);

			int lineCountTx = 0;
			int waitTime = 0;
			do                         // Initially wait to allow async send,
			{                          //   therefore, use do-while.
				Thread.Sleep(WaitInterval);
				waitTime += WaitInterval;

				Console.Out.WriteLine("Waiting for transmission, " + waitTime + " ms have passed, timeout is " + timeout + " ms...");

				if (waitTime >= timeout)
					Assert.Fail("Transmission timeout! Not enough lines received within expected interval.");

				lineCountTx = terminalTx.GetRepositoryLineCount(RepositoryType.Tx);
				if (lineCountTx > expectedTotalLineCount) // Break in case of too much data to improve speed of test.
					Assert.Fail("Transmission error!" +
						" Number of sent lines = " + lineCountTx +
						" mismatches expected = " + expectedTotalLineCount + ".");
			}
			while (lineCountTx != expectedTotalLineCount);

			Console.Out.WriteLine("...done");
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================