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
// YAT Version 2.0.0
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2018 Matthias Kläy.
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
		public const int WaitTimeoutForStateChange = 3000;

		/// <remarks>
		/// Shorter interval increases debug output.
		/// </remarks>
		public const int WaitIntervalForStateChange = 100;

		/// <remarks>
		/// Timeout of 200 ms is too short for serial COM port at 9600 baud,
		/// especially when debugger is connected:
		///  > SingleLine often takes longer than 200 ms.
		///  > DoubleLine often takes longer than 400 ms.
		///  > TripleLine takes around 500 ms (where timeout would be 600 ms).
		///  > MultiLine takes around 4000..5000 ms (where timeout would be 5200 ms).
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'ms' is the proper abbreviation for milliseconds but StyleCop isn't able to deal with such abbreviations...")]
		public const int WaitTimeoutForLineTransmission = 300;

		/// <remarks>
		/// Shorter interval decreases test.
		/// </remarks>
		public const int WaitIntervalForLineTransmission = 20;

		#endregion

		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

		internal static TerminalSettings GetTextSettings()
		{
			var settings = new TerminalSettings();
			settings.TerminalType = TerminalType.Text;
			settings.TextTerminal.ShowEol = true; // Required for easier test verification (byte count).
			return (settings);
		}

		internal static TerminalSettings GetTcpAutoSocketTextSettings(IPNetworkInterfaceEx networkInterface)
		{
			var settings = GetTextSettings();
			settings.IO.IOType = IOType.TcpAutoSocket;
			settings.IO.Socket.LocalInterface = networkInterface;
			return (settings);
		}

		internal static TerminalSettings GetTcpAutoSocketOnIPv4LoopbackTextSettings()
		{
			return (GetTcpAutoSocketTextSettings(IPNetworkInterface.IPv4Loopback));
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
				Thread.Sleep(WaitIntervalForStateChange);
				waitTime += WaitIntervalForStateChange;

				Console.Out.WriteLine("Waiting for connection, " + waitTime + " ms have passed, timeout is " + WaitTimeoutForStateChange + " ms...");

				if (waitTime >= WaitTimeoutForStateChange) {
					Assert.Fail("Connect timeout!");
				}
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
				Thread.Sleep(WaitIntervalForStateChange);
				waitTime += WaitIntervalForStateChange;

				Console.Out.WriteLine("Waiting for disconnection, " + waitTime + " ms have passed, timeout is " + WaitTimeoutForStateChange + " ms...");

				if (waitTime >= WaitTimeoutForStateChange) {
					Assert.Fail("Disconnect timeout!");
				}
			}

			Console.Out.WriteLine("...done");
		}

		/// <remarks>
		/// There are similar utility methods in 'Model.Test.Utilities'.
		/// Changes here may have to be applied there too.
		/// </remarks>
		internal static void WaitForSending(Terminal terminalTx, int expectedTotalByteCount, int expectedTotalLineCount)
		{
			// Calculate timeout:
			int timeout = WaitTimeoutForLineTransmission;

			int byteCountTx = 0;
			int lineCountTx = 0;
			int waitTime = 0;
			do                         // Initially wait to allow async send,
			{                          //   therefore, use do-while.
				Thread.Sleep(WaitIntervalForLineTransmission);
				waitTime += WaitIntervalForLineTransmission;

				Console.Out.WriteLine("Waiting for sending, " + waitTime + " ms have passed, timeout is " + timeout + " ms...");

				if (waitTime >= timeout) {
					Assert.Fail("Transmission timeout! Not enough data sent within expected interval.");
				}

				byteCountTx = terminalTx.GetRepositoryByteCount(RepositoryType.Tx);
				if (byteCountTx > expectedTotalByteCount) { // Break in case of too much data to improve speed of test.
					Assert.Fail("Transmission error!" +
					            " Number of sent bytes = " + byteCountTx +
					            " mismatches expected = " + expectedTotalByteCount + ".");
				}

				lineCountTx = terminalTx.GetRepositoryLineCount(RepositoryType.Tx);
				if (lineCountTx > expectedTotalLineCount) { // Break in case of too much data to improve speed of test.
					Assert.Fail("Transmission error!" +
					            " Number of sent lines = " + lineCountTx +
					            " mismatches expected = " + expectedTotalLineCount + ".");
				}
			}
			while ((byteCountTx != expectedTotalByteCount) || (lineCountTx != expectedTotalLineCount));

			Console.Out.WriteLine("...done");
		}

		/// <remarks>
		/// There are similar utility methods in 'Model.Test.Utilities'.
		/// Changes here may have to be applied there too.
		/// </remarks>
		internal static void WaitForTransmission(Terminal terminalTx, Terminal terminalRx, int expectedTotalByteCount, int expectedTotalLineCount)
		{
			// Calculate timeout:
			int timeout = WaitTimeoutForLineTransmission;

			int byteCountTx = 0;
			int lineCountTx = 0;
			int byteCountRx = 0;
			int lineCountRx = 0;
			int waitTime = 0;
			do                         // Initially wait to allow async send,
			{                          //   therefore, use do-while.
				Thread.Sleep(WaitIntervalForLineTransmission);
				waitTime += WaitIntervalForLineTransmission;

				Console.Out.WriteLine("Waiting for transmission, " + waitTime + " ms have passed, timeout is " + timeout + " ms...");

				if (waitTime >= timeout) {
					Assert.Fail("Transmission timeout! Not enough data transmitted within expected interval.");
				}

				byteCountTx = terminalTx.GetRepositoryByteCount(RepositoryType.Tx);
				if (byteCountTx > expectedTotalByteCount) { // Break in case of too much data to improve speed of test.
					Assert.Fail("Transmission error!" +
					            " Number of sent bytes = " + byteCountTx +
					            " mismatches expected = " + expectedTotalByteCount + ".");
				}

				lineCountTx = terminalTx.GetRepositoryLineCount(RepositoryType.Tx);
				if (lineCountTx > expectedTotalLineCount) { // Break in case of too much data to improve speed of test.
					Assert.Fail("Transmission error!" +
					            " Number of sent lines = " + lineCountTx +
					            " mismatches expected = " + expectedTotalLineCount + ".");
				}

				byteCountRx = terminalRx.GetRepositoryByteCount(RepositoryType.Rx);
				if (byteCountRx > expectedTotalByteCount) { // Break in case of too much data to improve speed of test.
					Assert.Fail("Transmission error!" +
					            " Number of received bytes = " + byteCountRx +
					            " mismatches expected = " + expectedTotalByteCount + ".");
				}

				lineCountRx = terminalRx.GetRepositoryLineCount(RepositoryType.Rx);
				if (lineCountRx > expectedTotalLineCount) { // Break in case of too much data to improve speed of test.
					Assert.Fail("Transmission error!" +
					            " Number of received lines = " + lineCountRx +
					            " mismatches expected = " + expectedTotalLineCount + ".");
				}
			}
			while ((byteCountTx != expectedTotalByteCount) || (lineCountTx != expectedTotalLineCount) ||
			       (byteCountRx != expectedTotalByteCount) || (lineCountRx != expectedTotalLineCount));

			Console.Out.WriteLine("...done");
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
