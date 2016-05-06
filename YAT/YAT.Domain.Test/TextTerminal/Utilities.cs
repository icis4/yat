//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

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

		private const int WaitTimeoutForConnectionChange = 5000;
		private const int WaitTimeoutForLineTransmission = 1000;
		private const int WaitInterval = 100;

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
			// Create settings:
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

		internal static void WaitForConnection(Terminal terminal)
		{
			int timeout = 0;
			do                         // Initially wait to allow async send,
			{                          //   therefore, use do-while.
				Thread.Sleep(WaitInterval);
				timeout += WaitInterval;

				if (timeout >= WaitTimeoutForConnectionChange)
					Assert.Fail("Connect timeout!");
			}
			while (!terminal.IsConnected);
		}

		internal static void WaitForConnection(Terminal terminalA, Terminal terminalB)
		{
			int timeout = 0;
			do                         // Initially wait to allow async send,
			{                          //   therefore, use do-while.
				Thread.Sleep(WaitInterval);
				timeout += WaitInterval;

				if (timeout >= WaitTimeoutForConnectionChange)
					Assert.Fail("Connect timeout!");
			}
			while (!terminalA.IsConnected && !terminalB.IsConnected);
		}

		internal static void WaitForDisconnection(Terminal terminal)
		{
			int timeout = 0;
			while (terminal.IsConnected)
			{
				Thread.Sleep(WaitInterval);
				timeout += WaitInterval;

				if (timeout >= WaitTimeoutForConnectionChange)
					Assert.Fail("Disconnect timeout!");
			}
		}

		internal static void WaitForDisconnection(Terminal terminalA, Terminal terminalB)
		{
			int timeout = 0;
			while (terminalA.IsConnected || terminalB.IsConnected)
			{
				Thread.Sleep(WaitInterval);
				timeout += WaitInterval;

				if (timeout >= WaitTimeoutForConnectionChange)
					Assert.Fail("Disconnect timeout!");
			}
		}

		internal static void WaitForTransmission(Terminal terminalTx, Terminal terminalRx, int currentLineCount, int expectedTotalLineCount)
		{
			int txLineCount = 0;
			int rxLineCount = 0;
			int timeout = 0;
			do                         // Initially wait to allow async send,
			{                          //   therefore, use do-while.
				Thread.Sleep(WaitInterval);
				timeout += WaitInterval;

				if (timeout >= (WaitTimeoutForLineTransmission * currentLineCount))
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
		}

		internal static void WaitForSending(Terminal terminalTx, int currentLineCount, int expectedTotalLineCount)
		{
			int lineCountTx = 0;
			int timeout = 0;
			do                         // Initially wait to allow async send,
			{                          //   therefore, use do-while.
				Thread.Sleep(WaitInterval);
				timeout += WaitInterval;

				if (timeout >= (WaitTimeoutForLineTransmission * currentLineCount))
					Assert.Fail("Transmission timeout! Not enough lines received within expected interval.");

				lineCountTx = terminalTx.GetRepositoryLineCount(RepositoryType.Tx);
				if (lineCountTx > expectedTotalLineCount) // Break in case of too much data to improve speed of test.
					Assert.Fail("Transmission error!" +
						" Number of sent lines = " + lineCountTx +
						" mismatches expected = " + expectedTotalLineCount + ".");
			}
			while (lineCountTx != expectedTotalLineCount);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
