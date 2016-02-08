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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using MKY;
using MKY.Collections.Generic;
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
			return (GetTextTcpAutoSocketSettings(IPNetworkInterfaceType.IPv4Loopback));
		}

		internal static TerminalSettings GetTextTcpAutoSocketSettings(IPNetworkInterface networkInterface)
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

		/// <remarks>Using 'A' and 'B' instead of 'Tx' and 'Rx' as some tests perform two-way-transmission.</remarks>
		internal static void WaitForTransmission(Terminal terminalA, Terminal terminalB, int expectedPerCycleLineCountRx)
		{
			WaitForTransmission(terminalA, terminalB, expectedPerCycleLineCountRx, 1); // Single cycle.
		}

		/// <remarks>Using 'A' and 'B' instead of 'Tx' and 'Rx' as some tests perform two-way-transmission.</remarks>
		internal static void WaitForTransmission(Terminal terminalA, Terminal terminalB, int expectedPerCycleLineCountRx, int cycle)
		{
			// Calculate total expected line count at the receiver side:
			int expectedTotalLineCountRx = (expectedPerCycleLineCountRx * cycle);

			// Calculate timeout factor per line, taking cases with 0 lines into account:
			int timeoutFactorPerLine = ((expectedPerCycleLineCountRx > 0) ? expectedPerCycleLineCountRx : 1);

			int timeout = 0;
			do                         // Initially wait to allow async send,
			{                          //   therefore, use do-while.
				Thread.Sleep(WaitInterval);
				timeout += WaitInterval;

				if (timeout >= (WaitTimeoutForLineTransmission * timeoutFactorPerLine))
					Assert.Fail("Transmission timeout! Try to re-run test case.");

				if (terminalB.RxLineCount > expectedTotalLineCountRx) // Break in case of too much data to improve speed of test.
					Assert.Fail("Transmission error!" +
						" Number of received lines = " + terminalB.RxLineCount +
						" mismatches expected = " + expectedTotalLineCountRx + ".");
			}
			while ((terminalB.RxLineCount != expectedTotalLineCountRx) ||
			       (terminalB.RxLineCount != terminalA.TxLineCount) ||
			       (terminalB.RxByteCount != terminalA.TxByteCount));

			// Attention: Terminal line count is not always equal to display line count!
			//  > Terminal line count = number of *completed* lines in terminal
			//  > Display line count = number of lines in view
			// This function uses terminal line count for verification!
		}

		internal static void WaitForTransmission(Terminal terminal, int expectedTotalLineCountRx, int expectedTotalByteCountRx)
		{
			int timeout = 0;
			do                         // Initially wait to allow async send,
			{                          //   therefore, use do-while.
				Thread.Sleep(WaitInterval);
				timeout += WaitInterval;

				if (timeout >= (WaitTimeoutForLineTransmission * expectedTotalLineCountRx))
					Assert.Fail("Transmission timeout! Try to re-run test case.");

				if ((terminal.RxLineCount > expectedTotalLineCountRx) ||
					(terminal.RxByteCount > expectedTotalByteCountRx)) // Break in case of too much data to improve speed of test.
					Assert.Fail("Transmission error!" +
						" Number of received lines = " + terminal.RxLineCount + " / bytes = " + terminal.RxByteCount +
						" mismatches expected = " + expectedTotalLineCountRx + " / " + expectedTotalByteCountRx + ".");
			}
			while ((terminal.RxLineCount != expectedTotalLineCountRx) ||
			       (terminal.RxByteCount != expectedTotalByteCountRx));

			// Attention: Terminal line count is not always equal to display line count!
			//  > Terminal line count = number of *completed* lines in terminal
			//  > Display line count = number of lines in view
			// This function uses terminal line count for verification!
		}

		#endregion

		#region Verifications
		//==========================================================================================
		// Verifications
		//==========================================================================================

		/// <remarks>Using 'A' and 'B' instead of 'Tx' and 'Rx' as some tests perform two-way-transmission.</remarks>
		internal static void VerifyLines(List<DisplayLine> displayLinesA, List<DisplayLine> displayLinesB, TestSet testSet)
		{
			VerifyLines(displayLinesA, displayLinesB, testSet, 1); // Single cycle.
		}

		/// <remarks>Using 'A' and 'B' instead of 'Tx' and 'Rx' as some tests perform two-way-transmission.</remarks>
		internal static void VerifyLines(List<DisplayLine> displayLinesA, List<DisplayLine> displayLinesB, TestSet testSet, int cycle)
		{
			// Attention: Display line count is not always equal to terminal line count!
			//  > Display line count = number of lines in view
			//  > Terminal line count = number of *completed* lines in terminal
			// This function uses display line count for verification!

			// Calculate total expected dispaly line count at the receiver side:
			int expectedTotalDisplayLineCountB = (testSet.ExpectedElementCounts.Length * cycle);

			// Compare the expected line count at the receiver side:
			if (displayLinesB.Count != expectedTotalDisplayLineCountB)
			{
				StringBuilder sbB = new StringBuilder();
				foreach (DisplayLine displayLineB in displayLinesB)
					sbB.Append(ArrayEx.ElementsToString(displayLineB.ToArray()));

				Console.Error.Write
				(
					"B:" + Environment.NewLine + sbB + Environment.NewLine
				);

				Assert.Fail
				(
					"Line count mismatches: " + Environment.NewLine +
					"Expected = " + expectedTotalDisplayLineCountB + " line(s), " +
					"B = " + displayLinesB.Count + " line(s)." + Environment.NewLine +
					@"See ""Output"" for details."
				);
			}

			// If both sides are expected to show the same line count, compare the counts,
			// otherwise, ignore the comparision:
			if (testSet.ExpectedAlsoApplyToA)
			{
				if (displayLinesB.Count == displayLinesA.Count)
				{
					for (int i = 0; i < displayLinesA.Count; i++)
					{
						int index                = i % testSet.ExpectedElementCounts.Length;
						int expectedElementCount =     testSet.ExpectedElementCounts[index];
						int expectedDataCount    =     testSet.ExpectedDataCounts[index];

						DisplayLine displayLineA = displayLinesA[i];
						DisplayLine displayLineB = displayLinesB[i];

						if ((displayLineB.Count     == displayLineA.Count) &&
							(displayLineB.Count     == expectedElementCount) &&
							(displayLineB.DataCount == displayLineA.DataCount) &&
							(displayLineB.DataCount == expectedDataCount))
						{
							for (int j = 0; j < displayLineA.Count; j++)
								Assert.AreEqual(displayLineA[j].Text, displayLineB[j].Text);
						}
						else
						{
							string strA = ArrayEx.ElementsToString(displayLineA.ToArray());
							string strB = ArrayEx.ElementsToString(displayLineB.ToArray());

							Console.Error.Write
							(
								"A:" + Environment.NewLine + strA + Environment.NewLine +
								"B:" + Environment.NewLine + strB + Environment.NewLine
							);

							Assert.Fail
							(
								"Length of line " + i + " mismatches:" + Environment.NewLine +
								"Expected = " + expectedElementCount + " element(s), " +
								"A = " + displayLineA.Count + " element(s), " +
								"B = " + displayLineB.Count + " element(s)," + Environment.NewLine +
								"Expected = " + expectedDataCount + " data, " +
								"A = " + displayLineA.DataCount + " data, " +
								"B = " + displayLineB.DataCount + " data." + Environment.NewLine +
								@"See ""Output"" for details."
							);
						}
					}
				}
				else
				{
					StringBuilder sbA = new StringBuilder();
					foreach (DisplayLine displayLineA in displayLinesA)
						sbA.Append(ArrayEx.ElementsToString(displayLineA.ToArray()));

					StringBuilder sbB = new StringBuilder();
					foreach (DisplayLine displayLineB in displayLinesB)
						sbB.Append(ArrayEx.ElementsToString(displayLineB.ToArray()));

					Console.Error.Write
					(
						"A:" + Environment.NewLine + sbA + Environment.NewLine +
						"B:" + Environment.NewLine + sbB + Environment.NewLine
					);

					Assert.Fail
					(
						"Line count mismatches: " + Environment.NewLine +
						"Expected = " + expectedTotalDisplayLineCountB + " line(s), " +
						"A = " + displayLinesA.Count + " line(s), " +
						"B = " + displayLinesB.Count + " line(s)." + Environment.NewLine +
						@"See ""Output"" for details."
					);
				}
			}
		}

		#endregion

		#region Helpers
		//==========================================================================================
		// Helpers
		//==========================================================================================

		private static bool staticTerminalMessageInputRequestResultsInExclude = false;
		private static string staticTerminalMessageInputRequestResultsInExcludeText = "";

		/// <summary></summary>
		public static bool TerminalMessageInputRequestResultsInExclude
		{
			get { return (staticTerminalMessageInputRequestResultsInExclude); }
		}

		/// <summary></summary>
		public static string TerminalMessageInputRequestResultsInExcludeText
		{
			get { return (staticTerminalMessageInputRequestResultsInExcludeText); }
		}

		/// <summary></summary>
		public static void TerminalMessageInputRequest(object sender, MessageInputEventArgs e)
		{
			// No assertion = exception can be invoked here as it might be handled by the calling event handler.
			// Therefore, simply confirm...
			e.Result = DialogResult.OK;

			// ...and signal exclusion via a flag:
			if (e.Text.StartsWith("Unable to start terminal!"))
			{
				staticTerminalMessageInputRequestResultsInExclude = true;
				staticTerminalMessageInputRequestResultsInExcludeText = e.Text;
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
