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
// YAT Version 2.2.0 Development
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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using NUnit.Framework;

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
		public const int IgnoreCount = -1;

		/// <summary></summary>
		public const int IgnoreTimeout = 0;

		/// <summary></summary>
		public const int WaitTimeoutForStateChange = 3000;

		/// <remarks>
		/// Note that a shorter interval would increase debug output, spoiling the debug console.
		/// </remarks>
		public const int WaitIntervalForStateChange = 100;

		/// <summary></summary>
		public const int WaitTimeoutForIsSendingForSomeTime = (3 * Domain.Utilities.ForSomeTimeEventHelper.Threshold);

		/// <remarks>
		/// Note that a shorter interval would increase debug output, spoiling the debug console.
		/// </remarks>
		public const int WaitIntervalForIsSendingForSomeTime = WaitIntervalForStateChange;

		/// <summary></summary>
		public const int WaitTimeoutForIsNoLongerSending = (3 * Domain.Utilities.ForSomeTimeEventHelper.Threshold);

		/// <remarks>
		/// Note that a shorter interval would increase debug output, spoiling the debug console.
		/// </remarks>
		public const int WaitIntervalForIsNoLongerSending = WaitIntervalForStateChange;

		/// <remarks>
		/// Timeout of 200 ms is too short for serial COM ports at 9600 baud, especially when
		/// debugger is connected. Measurements:
		///  > TripleLine (where timeout would be 3 * 200 ms = 600 ms) takes around 500 ms.
		///  > MultiLine (where timeout would be 26 * 200 ms = 5200 ms) takes around 5000 ms.
		///     => 300 ms seems defensive enough while still not too long to waste time.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'ms' is the proper abbreviation for milliseconds but StyleCop isn't able to deal with such abbreviations...")]
		public const int WaitTimeoutForLineTransmission = 300; // See remarks above.

		/// <remarks>
		/// Note that a longer interval would increase the wait time, thus increasing the test time.
		/// </remarks>
		public const int WaitIntervalForTransmission = 20;

		#endregion

		#region Regex'es
		//==========================================================================================
		// Regex'es
		//==========================================================================================

		/// <summary>Simple regex pattern matching the default format of "HH:mm:ss.fff" without any value range checks.</summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'ss' amd 'fff' are .NET format strings.")]
		public static readonly string TimeStampRegexPattern = @"\d{2}:\d{2}:\d{2}.\d{3}";

		/// <summary>Simple regex pattern matching the default format of "[d days ][h][h:][m][m:][s]s.fff" reduced to "[s]s.fff" without any value range checks.</summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'fff' is a .NET format string.")]
		public static readonly string DurationRegexPattern = @"\d{1,2}.\d{3}";

		#endregion

		#region Transmit
		//==========================================================================================
		// Transmit
		//==========================================================================================

		/// <summary></summary>
		public static void TransmitAndVerifyTxCounts(Domain.Terminal terminalTx,
		                                             Domain.Parser.Parser parser, string text, int eolByteCount,
		                                             ref int expectedTotalByteCount, ref int expectedTotalLineCount,
		                                             int timeout = WaitTimeoutForLineTransmission)
		{
			byte[] parseResult;
			Assert.That(parser.TryParse(text, out parseResult));

			terminalTx.SendTextLine(text);

			int textByteCount = parseResult.Length;
			expectedTotalByteCount += (textByteCount + eolByteCount);
			expectedTotalLineCount++;
			WaitForSendingAndVerifyCounts(terminalTx, expectedTotalByteCount, expectedTotalLineCount, timeout);
		}

		/// <summary></summary>
		public static void TransmitAndVerifyRxCounts(Domain.Terminal terminalTx, Domain.Terminal terminalRx,
		                                             Domain.Parser.Parser parser, string text, int eolByteCount,
		                                             ref int expectedTotalByteCount, ref int expectedTotalLineCount,
		                                             int timeout = WaitTimeoutForLineTransmission)
		{
			byte[] parseResult;
			Assert.That(parser.TryParse(text, out parseResult));

			terminalTx.SendTextLine(text);

			int textByteCount = parseResult.Length;
			expectedTotalByteCount += (textByteCount + eolByteCount);
			expectedTotalLineCount++;
			WaitForReceivingAndVerifyCounts(terminalTx, expectedTotalByteCount, expectedTotalLineCount, timeout);
		}

		/// <summary></summary>
		public static void TransmitAndVerifyCounts(Domain.Terminal terminalTx, Domain.Terminal terminalRx,
		                                           Domain.Parser.Parser parser, string text, int eolByteCount,
		                                           ref int expectedTotalByteCount, ref int expectedTotalLineCount,
		                                           int timeout = WaitTimeoutForLineTransmission)
		{
			byte[] parseResult;
			Assert.That(parser.TryParse(text, out parseResult));

			terminalTx.SendTextLine(text);

			int textByteCount = parseResult.Length;
			expectedTotalByteCount += (textByteCount + eolByteCount);
			expectedTotalLineCount++;
			WaitForTransmissionAndVerifyCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount, timeout);
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
		public static void WaitForConnection(Domain.Terminal terminalA, Domain.Terminal terminalB)
		{
			int waitTime = 0;

			Trace.WriteLine("Waiting for connection, " + waitTime + " ms have passed, timeout is " + WaitTimeoutForStateChange + " ms...");

			while (!terminalA.IsConnected && !terminalB.IsConnected)
			{
				Thread.Sleep(WaitIntervalForStateChange);
				waitTime += WaitIntervalForStateChange;

				Trace.WriteLine("Waiting for connection, " + waitTime + " ms have passed, timeout is " + WaitTimeoutForStateChange + " ms...");

				if (waitTime >= WaitTimeoutForStateChange)
					Assert.Fail("Connect timeout!");
			}

			Trace.WriteLine("...done, connected");
		}

		/// <remarks>
		/// There are similar utility methods in 'Model.Test.Utilities'.
		/// Changes here may have to be applied there too.
		/// </remarks>
		public static void WaitForDisconnection(Domain.Terminal terminal)
		{
			int waitTime = 0;

			Trace.WriteLine("Waiting for disconnection, " + waitTime + " ms have passed, timeout is " + WaitTimeoutForStateChange + " ms...");

			while (terminal.IsConnected)
			{
				Thread.Sleep(WaitIntervalForStateChange);
				waitTime += WaitIntervalForStateChange;

				Trace.WriteLine("Waiting for disconnection, " + waitTime + " ms have passed, timeout is " + WaitTimeoutForStateChange + " ms...");

				if (waitTime >= WaitTimeoutForStateChange)
					Assert.Fail("Disconnect timeout!");
			}

			Trace.WriteLine("...done, disconnected");
		}

		/// <summary></summary>
		public static void WaitForIsSendingForSomeTime(Domain.Terminal terminal, int timeout = WaitTimeoutForIsSendingForSomeTime)
		{
			int waitTime = 0;

			Trace.WriteLine("Waiting for 'IsSendingForSomeTime', " + waitTime + " ms have passed, timeout is " + timeout + " ms...");

			while (!terminal.IsSendingForSomeTime)
			{
				Thread.Sleep(WaitIntervalForIsSendingForSomeTime);
				waitTime += WaitIntervalForIsSendingForSomeTime;

				Trace.WriteLine("Waiting for 'IsSendingForSomeTime', " + waitTime + " ms have passed, timeout is " + timeout + " ms...");

				if (waitTime >= timeout)
					Assert.Fail("'IsSendingForSomeTime' timeout!");
			}

			Trace.WriteLine("...done, 'IsSendingForSomeTime'");
		}

		/// <summary></summary>
		public static void WaitForIsNoLongerSending(Domain.Terminal terminal, int timeout = WaitTimeoutForIsNoLongerSending)
		{
			int waitTime = 0;

			Trace.WriteLine("Waiting for 'IsNoLongerSending', " + waitTime + " ms have passed, timeout is " + timeout + " ms...");

			while (terminal.IsSending)
			{
				Thread.Sleep(WaitIntervalForIsNoLongerSending);
				waitTime += WaitIntervalForIsNoLongerSending;

				Trace.WriteLine("Waiting for 'IsNoLongerSending', " + waitTime + " ms have passed, timeout is " + timeout + " ms...");

				if (waitTime >= timeout)
					Assert.Fail("'IsNoLongerSending' timeout!");
			}

			Trace.WriteLine("...done, 'IsNoLongerSending'");
		}

		/// <remarks>
		/// There are similar utility methods in 'Model.Test.Utilities'.
		/// Changes here may have to be applied there too.
		/// </remarks>
		public static void WaitForSendingAndVerifyByteCount(Domain.Terminal terminalTx, int expectedTotalByteCount, int timeout = WaitTimeoutForLineTransmission)
		{
			WaitForSendingAndVerifyCounts(terminalTx, expectedTotalByteCount, IgnoreCount, timeout);
		}

		/// <remarks>
		/// There are similar utility methods in 'Model.Test.Utilities'.
		/// Changes here may have to be applied there too.
		/// </remarks>
		/// <remarks>
		/// 'expectedTotalLineCount' will be compared against the number of lines in the view,
		/// i.e. complete as well as incomplete lines.
		/// Comparison against the completed number of lines is not (yet) possible, change #375
		/// "consider to migrate Byte/Line Count/Rate from model to domain" is required for this.
		/// </remarks>
		public static void WaitForSendingAndVerifyCounts(Domain.Terminal terminalTx, int expectedTotalByteCount, int expectedTotalLineCount = IgnoreCount, int timeout = WaitTimeoutForLineTransmission)
		{
			int txByteCount = 0;
			int txLineCount = 0;
			int waitTime = 0;
			bool isFirst = true; // Using do-while, first check state.

			do
			{
				if (!isFirst) {
					Thread.Sleep(WaitIntervalForTransmission);
					waitTime += WaitIntervalForTransmission;
				}

				if (timeout != IgnoreTimeout) {
					Trace.WriteLine("Waiting for sending, " + waitTime + " ms have passed, timeout is " + timeout + " ms...");
				}

				txByteCount = terminalTx.GetRepositoryByteCount(RepositoryType.Tx);
				if (txByteCount > expectedTotalByteCount) { // Break in case of too much data to improve speed of test.
					Assert.Fail("Number of sent bytes = " + txByteCount +
					            " mismatches expected = " + expectedTotalByteCount + ".");
				}

				txLineCount = terminalTx.GetRepositoryLineCount(RepositoryType.Tx);
				if (expectedTotalLineCount != IgnoreCount) {
					if (txLineCount > expectedTotalLineCount) { // Break in case of too much data to improve speed of test.
						Assert.Fail("Number of sent lines = " + txLineCount +
						            " mismatches expected = " + expectedTotalLineCount + ".");
					}
				}

				if ((waitTime >= timeout) && ((timeout != IgnoreTimeout) || !isFirst)) {
					StringBuilder sb;
					if (timeout != IgnoreTimeout) {
						sb = new StringBuilder("Timeout!");
					}
					else {
						sb = new StringBuilder("Mismatch!");
					}

					if (txByteCount < expectedTotalByteCount) {
						sb.Append(" Number of sent bytes = " + txByteCount +
						          " mismatches expected = " + expectedTotalByteCount + ".");
					}

					if (txLineCount < expectedTotalLineCount) {
						sb.Append(" Number of sent lines = " + txLineCount +
						          " mismatches expected = " + expectedTotalLineCount + ".");
					}

					Assert.Fail(sb.ToString());
				}

				if (isFirst) {
					isFirst = false;
				}
			}
			while ((txByteCount != expectedTotalByteCount) || ((txLineCount != expectedTotalLineCount) && (expectedTotalLineCount != IgnoreCount)));

			Debug.WriteLine("Tx of " + txByteCount + " bytes / " + txLineCount + " lines completed");

			if (timeout != IgnoreTimeout) {
				Trace.WriteLine("...done, sent and verified");
			}
			else {
				Trace.WriteLine("Sending verified");
			}
		}

		/// <remarks>
		/// <see cref="WaitForSendingAndVerifyCounts"/> above.
		/// </remarks>
		public static void VerifyTxCounts(Domain.Terminal terminalTx, int expectedTotalByteCount, int expectedTotalLineCount = IgnoreCount)
		{
			WaitForSendingAndVerifyCounts(terminalTx, expectedTotalByteCount, expectedTotalLineCount, IgnoreTimeout);
		}

		/// <remarks>
		/// There are similar utility methods in 'Model.Test.Utilities'.
		/// Changes here may have to be applied there too.
		/// </remarks>
		public static void WaitForReceivingAndVerifyByteCount(Domain.Terminal terminalRx, int expectedTotalByteCount, int timeout = WaitTimeoutForLineTransmission)
		{
			WaitForReceivingAndVerifyCounts(terminalRx, expectedTotalByteCount, IgnoreCount, timeout);
		}

		/// <remarks>
		/// There are similar utility methods in 'Model.Test.Utilities'.
		/// Changes here may have to be applied there too.
		/// </remarks>
		/// <remarks>
		/// 'expectedTotalLineCount' will be compared against the number of lines in the view,
		/// i.e. complete as well as incomplete lines.
		/// Comparison against the completed number of lines is not (yet) possible, change #375
		/// "consider to migrate Byte/Line Count/Rate from model to domain" is required for this.
		/// </remarks>
		public static void WaitForReceivingAndVerifyCounts(Domain.Terminal terminalRx, int expectedTotalByteCount, int expectedTotalLineCount = IgnoreCount, int timeout = WaitTimeoutForLineTransmission)
		{
			int rxByteCount = 0;
			int rxLineCount = 0;
			int waitTime = 0;
			bool isFirst = true; // Using do-while, first check state.

			do
			{
				if (!isFirst) {
					Thread.Sleep(WaitIntervalForTransmission);
					waitTime += WaitIntervalForTransmission;
				}

				if (timeout != IgnoreTimeout) {
					Trace.WriteLine("Waiting for receiving, " + waitTime + " ms have passed, timeout is " + timeout + " ms...");
				}

				rxByteCount = terminalRx.GetRepositoryByteCount(RepositoryType.Rx);
				if (rxByteCount > expectedTotalByteCount) { // Break in case of too much data to improve speed of test.
					Assert.Fail("Number of received bytes = " + rxByteCount +
					            " mismatches expected = " + expectedTotalByteCount + ".");
				}

				rxLineCount = terminalRx.GetRepositoryLineCount(RepositoryType.Rx);
				if (expectedTotalLineCount != IgnoreCount) {
					if (rxLineCount > expectedTotalLineCount) { // Break in case of too much data to improve speed of test.
						Assert.Fail("Number of received lines = " + rxLineCount +
						            " mismatches expected = " + expectedTotalLineCount + ".");
					}
				}

				if ((waitTime >= timeout) && ((timeout != IgnoreTimeout) || !isFirst)) {
					StringBuilder sb;
					if (timeout != IgnoreTimeout) {
						sb = new StringBuilder("Timeout!");
					}
					else {
						sb = new StringBuilder("Mismatch!");
					}

					if (rxByteCount < expectedTotalByteCount) {
						sb.Append(" Number of received bytes = " + rxByteCount +
						          " mismatches expected = " + expectedTotalByteCount + ".");
					}

					if (rxLineCount < expectedTotalLineCount) {
						sb.Append(" Number of received lines = " + rxLineCount +
						          " mismatches expected = " + expectedTotalLineCount + ".");
					}

					Assert.Fail(sb.ToString());
				}

				if (isFirst) {
					isFirst = false;
				}
			}
			while ((rxByteCount != expectedTotalByteCount) || (rxLineCount != expectedTotalLineCount && (expectedTotalLineCount != IgnoreCount)));

			Debug.WriteLine("Rx of " + rxByteCount + " bytes / " + rxLineCount + " lines completed");

			if (timeout != IgnoreTimeout) {
				Trace.WriteLine("...done, received and verified");
			}
			else {
				Trace.WriteLine("Receiving verified");
			}
		}

		/// <remarks>
		/// <see cref="WaitForReceivingAndVerifyCounts"/> above.
		/// </remarks>
		public static void VerifyRxCounts(Domain.Terminal terminalRx, int expectedTotalByteCount, int expectedTotalLineCount = IgnoreCount)
		{
			WaitForReceivingAndVerifyCounts(terminalRx, expectedTotalByteCount, expectedTotalLineCount, IgnoreTimeout);
		}

		/// <remarks>
		/// There are similar utility methods in 'Model.Test.Utilities'.
		/// Changes here may have to be applied there too.
		/// </remarks>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Prepared for future use.")]
		public static void WaitForTransmissionAndVerifyByteCount(Domain.Terminal terminalTx, Domain.Terminal terminalRx, int expectedTotalByteCount, int timeout = WaitTimeoutForLineTransmission)
		{
			WaitForTransmissionAndVerifyCounts(terminalTx, terminalRx, expectedTotalByteCount, IgnoreCount, timeout);
		}

		/// <remarks>
		/// There are similar utility methods in 'Model.Test.Utilities'.
		/// Changes here may have to be applied there too.
		/// </remarks>
		/// <remarks>
		/// 'expectedPerCycleCharCount' does not need to be considered, since bytes are transmitted.
		/// </remarks>
		/// <remarks>
		/// 'expectedTotalLineCount' will be compared against the number of lines in the view,
		/// i.e. complete as well as incomplete lines.
		/// Comparison against the completed number of lines is not (yet) possible, change #375
		/// "consider to migrate Byte/Line Count/Rate from model to domain" is required for this.
		/// </remarks>
		public static void WaitForTransmissionAndVerifyCounts(Domain.Terminal terminalTx, Domain.Terminal terminalRx, int expectedTotalByteCount, int expectedTotalLineCount = IgnoreCount, int timeout = WaitTimeoutForLineTransmission)
		{
			// Attention:
			// Similar code exists in Model.Test.Utilities.WaitForTransmissionAndVerifyCounts().
			// Changes here may have to be applied there too.

			int txByteCount = 0;
			int txLineCount = 0;
			int rxByteCount = 0;
			int rxLineCount = 0;
			int waitTime = 0;
			bool isFirst = true; // Using do-while, first check state.

			do
			{
				if (!isFirst) {
					Thread.Sleep(WaitIntervalForTransmission);
					waitTime += WaitIntervalForTransmission;
				}

				if (timeout != IgnoreTimeout) {
					Trace.WriteLine("Waiting for transmission, " + waitTime + " ms have passed, timeout is " + timeout + " ms...");
				}

				txByteCount = terminalTx.GetRepositoryByteCount(RepositoryType.Tx);
				if (txByteCount > expectedTotalByteCount) { // Break in case of too much data to improve speed of test.
					Assert.Fail("Number of sent bytes = " + txByteCount +
					            " mismatches expected = " + expectedTotalByteCount + ".");
				}

				txLineCount = terminalTx.GetRepositoryLineCount(RepositoryType.Tx);
				if (expectedTotalLineCount != IgnoreCount) {
					if (txLineCount > expectedTotalLineCount) { // Break in case of too much data to improve speed of test.
						Assert.Fail("Number of sent lines = " + txLineCount +
						            " mismatches expected = " + expectedTotalLineCount + ".");
					}
				}

				rxByteCount = terminalRx.GetRepositoryByteCount(RepositoryType.Rx);
				if (rxByteCount > expectedTotalByteCount) { // Break in case of too much data to improve speed of test.
					Assert.Fail("Number of received bytes = " + rxByteCount +
					            " mismatches expected = " + expectedTotalByteCount + ".");
				}

				rxLineCount = terminalRx.GetRepositoryLineCount(RepositoryType.Rx);
				if (expectedTotalLineCount != IgnoreCount) {
					if (rxLineCount > expectedTotalLineCount) { // Break in case of too much data to improve speed of test.
						Assert.Fail("Number of received lines = " + rxLineCount +
						            " mismatches expected = " + expectedTotalLineCount + ".");
					}
				}

				if ((waitTime >= timeout) && ((timeout != IgnoreTimeout) || !isFirst)) {
					StringBuilder sb;
					if (timeout != IgnoreTimeout) {
						sb = new StringBuilder("Timeout!");
					}
					else {
						sb = new StringBuilder("Mismatch!");
					}

					if (txByteCount < expectedTotalByteCount) {
						sb.Append(" Number of sent bytes = " + txByteCount +
						          " mismatches expected = " + expectedTotalByteCount + ".");
					}

					if (txLineCount < expectedTotalLineCount) {
						sb.Append(" Number of sent lines = " + txLineCount +
						          " mismatches expected = " + expectedTotalLineCount + ".");
					}

					if (rxByteCount < expectedTotalByteCount) {
						sb.Append(" Number of received bytes = " + rxByteCount +
						          " mismatches expected = " + expectedTotalByteCount + ".");
					}

					if (rxLineCount < expectedTotalLineCount) {
						sb.Append(" Number of received lines = " + rxLineCount +
						          " mismatches expected = " + expectedTotalLineCount + ".");
					}

					Assert.Fail(sb.ToString());
				}

				if (isFirst) {
					isFirst = false;
				}
			}
			while ((txByteCount != expectedTotalByteCount) || (txLineCount != expectedTotalLineCount && (expectedTotalLineCount != IgnoreCount)) ||
			       (rxByteCount != expectedTotalByteCount) || (rxLineCount != expectedTotalLineCount && (expectedTotalLineCount != IgnoreCount)));

			Debug.WriteLine("Tx of " + txByteCount + " bytes / " + txLineCount + " lines completed");
			Debug.WriteLine("Rx of " + rxByteCount + " bytes / " + rxLineCount + " lines completed");

			if (timeout != IgnoreTimeout) {
				Trace.WriteLine("...done, transmitted and verified");
			}
			else {
				Trace.WriteLine("Transmission verified");
			}
		}

		/// <remarks>
		/// <see cref="WaitForTransmissionAndVerifyCounts"/> above.
		/// </remarks>
		public static void VerifyCounts(Domain.Terminal terminalTx, Domain.Terminal terminalRx, int expectedTotalByteCount, int expectedTotalLineCount = IgnoreCount)
		{
			WaitForTransmissionAndVerifyCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount, IgnoreTimeout);
		}

		/// <remarks>
		/// There are similar utility methods in 'Model.Test.Utilities'.
		/// Changes here may have to be applied there too.
		/// </remarks>
		public static void WaitForReverification()
		{
			Thread.Sleep(2 * WaitTimeoutForLineTransmission);
		}

		#endregion

		#region Verify
		//==========================================================================================
		// Verify
		//==========================================================================================

		/// <summary></summary>
		public static void VerifyTxContent(Domain.Terminal terminal, IEnumerable<string> expectedContentPattern)
		{
			VerifyContent(terminal, RepositoryType.Tx, expectedContentPattern);
		}

		/// <summary></summary>
		public static void VerifyBidirContent(Domain.Terminal terminal, IEnumerable<string> expectedContentPattern)
		{
			VerifyContent(terminal, RepositoryType.Bidir, expectedContentPattern);
		}

		/// <summary></summary>
		public static void VerifyRxContent(Domain.Terminal terminal, IEnumerable<string> expectedContentPattern)
		{
			VerifyContent(terminal, RepositoryType.Rx, expectedContentPattern);
		}

		private static void VerifyContent(Domain.Terminal terminal, RepositoryType repositoryType, IEnumerable<string> expectedContentPattern)
		{
			var displayLines = terminal.RepositoryToDisplayLines(repositoryType);

			var dlEnum = displayLines.GetEnumerator();
			var ecEnum = expectedContentPattern.GetEnumerator();

			var i = 0;
			var previousLineTimeStamp = DateTime.MinValue;

			while (true)
			{
				var dlEnumIsAtEnd = !dlEnum.MoveNext(); // Ensure that both enumerators are located
				var ecEnumIsAtEnd = !ecEnum.MoveNext(); // at the same position! This wouldn't be
				                                        // given in logical || or && condition!
				if (dlEnumIsAtEnd || ecEnumIsAtEnd)
				{
					if (dlEnumIsAtEnd && ecEnumIsAtEnd)
						break;
					else          // Mismatching counts shall be asserted last, i.e. content mismatches shall be asserted first.
						Assert.Fail("Mismatching counts, terminal contains {0} lines but {1} lines are expected!", displayLines.Count, expectedContentPattern.Count());
				}

				i++;

				var input = dlEnum.Current.ToString();
				var pattern = DecoratePattern(ecEnum.Current);
				Assert.That(Regex.IsMatch(input, pattern), Is.True, @"Line {0} is ""{1}"" mismatching expected ""{2}""", i, input, pattern);

				Assert.That(dlEnum.Current.TimeStamp, Is.GreaterThanOrEqualTo(previousLineTimeStamp));
				previousLineTimeStamp = dlEnum.Current.TimeStamp;
			}
		}

		private static string DecoratePattern(string expectedContentPattern)
		{
			expectedContentPattern = expectedContentPattern.Replace("(", @"\(");
			expectedContentPattern = expectedContentPattern.Replace(")", @"\)");

			return ("^" + expectedContentPattern + "$");
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
