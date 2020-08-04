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

using MKY.Text;

using NUnit.Framework;

///// YAT.Domain.Parser is not used due to ambiguity with 'YAT.Domain.Test.Parser'.
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
		public static void TransmitAndAssertTxCounts(Domain.Terminal terminalTx,
		                                             Domain.Parser.Parser parser, string text,
		                                             ref int expectedTotalByteCount, ref int expectedTotalLineCount,
		                                             int timeout = WaitTimeoutForLineTransmission)
		{
			byte[] parseResult;
			Assert.That(parser.TryParse(text, out parseResult)); // Verify text before sending, as result will be needed anyway.
			int textByteCount = parseResult.Length;

			terminalTx.SendTextLine(text); // Always send a "line", no matter what terminal type is being used.

			int eolByteCount = 0;
			if (terminalTx.TerminalSettings.TerminalType == TerminalType.Text)
			{
				var txEol = terminalTx.TerminalSettings.TextTerminal.TxEol;
				Assert.That(parser.TryParse(txEol, out parseResult));
				eolByteCount = parseResult.Length;
			}

			expectedTotalByteCount += (textByteCount + eolByteCount);
			expectedTotalLineCount++;
			WaitForSendingAndAssertCounts(terminalTx, expectedTotalByteCount, expectedTotalLineCount, timeout);
		}

		/// <summary></summary>
		public static void TransmitAndAssertRxCounts(Domain.Terminal terminalTx, Domain.Terminal terminalRx,
		                                             Domain.Parser.Parser parser, string text,
		                                             ref int expectedTotalByteCount, ref int expectedTotalLineCount,
		                                             int timeout = WaitTimeoutForLineTransmission)
		{
			byte[] parseResult;
			Assert.That(parser.TryParse(text, out parseResult)); // Verify text before sending, as result will be needed anyway.
			int textByteCount = parseResult.Length;

			terminalTx.SendTextLine(text); // Always send a "line", no matter what terminal type is being used.

			int eolByteCount = 0;
			if (terminalRx.TerminalSettings.TerminalType == TerminalType.Text)
			{
				var rxEol = terminalRx.TerminalSettings.TextTerminal.RxEol;
				Assert.That(parser.TryParse(rxEol, out parseResult));
				eolByteCount = parseResult.Length;
			}

			expectedTotalByteCount += (textByteCount + eolByteCount);
			expectedTotalLineCount++;
			WaitForReceivingAndAssertCounts(terminalTx, expectedTotalByteCount, expectedTotalLineCount, timeout);
		}

		/// <summary></summary>
		public static void TransmitAndAssertCounts(Domain.Terminal terminalTx, Domain.Terminal terminalRx,
		                                           Domain.Parser.Parser parser, string text,
		                                           ref int expectedTotalByteCount, ref int expectedTotalLineCount,
		                                           int timeout = WaitTimeoutForLineTransmission)
		{
			byte[] parseResult;
			Assert.That(parser.TryParse(text, out parseResult)); // Verify text before sending, as result will be needed anyway.
			int textByteCount = parseResult.Length;

			terminalTx.SendTextLine(text); // Always send a "line", no matter what terminal type is being used.

			int eolByteCount = 0;
			var terminalTxTerminalType = terminalTx.TerminalSettings.TerminalType;
			var terminalRxTerminalType = terminalRx.TerminalSettings.TerminalType;
			Assert.That(terminalTxTerminalType, Is.EqualTo(terminalRxTerminalType), "This verification requires that both terminals are of the same type!");
			if (terminalTxTerminalType == TerminalType.Text)
			{
				var txEol = terminalTx.TerminalSettings.TextTerminal.TxEol;
				Assert.That(parser.TryParse(txEol, out parseResult));
				var txEolBytes = parseResult;

				var rxEol = terminalRx.TerminalSettings.TextTerminal.RxEol;
				Assert.That(parser.TryParse(rxEol, out parseResult));
				var rxEolBytes = parseResult;

				Assert.That(txEolBytes, Is.EqualTo(rxEolBytes), "For [Text] terminals, this verification requires that both terminals use the same EOL sequence!");
				eolByteCount = txEolBytes.Length;
			}

			expectedTotalByteCount += (textByteCount + eolByteCount);
			expectedTotalLineCount++;
			WaitForTransmissionAndAssertCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount, timeout);
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

		/// <summary></summary>
		public static void WaitForSendingAndAssertByteCount(Domain.Terminal terminalTx, int expectedTotalByteCount, int timeout = WaitTimeoutForLineTransmission)
		{
			WaitForSendingAndAssertCounts(terminalTx, expectedTotalByteCount, IgnoreCount, timeout);
		}

		/// <remarks>
		/// 'expectedTotalLineCount' will be compared against the number of lines in the view,
		/// i.e. complete as well as incomplete lines.
		/// Comparison against the completed number of lines is not (yet) possible, change #375
		/// "consider to migrate Byte/Line Count/Rate from model to domain" is required for this.
		/// </remarks>
		public static void WaitForSendingAndAssertCounts(Domain.Terminal terminalTx, int expectedTotalByteCount, int expectedTotalLineCount = IgnoreCount, int timeout = WaitTimeoutForLineTransmission)
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

		/// <summary></summary>
		public static void WaitForReceivingAndAssertByteCount(Domain.Terminal terminalRx, int expectedTotalByteCount, int timeout = WaitTimeoutForLineTransmission)
		{
			WaitForReceivingAndAssertCounts(terminalRx, expectedTotalByteCount, IgnoreCount, timeout);
		}

		/// <remarks>
		/// 'expectedTotalLineCount' will be compared against the number of lines in the view,
		/// i.e. complete as well as incomplete lines.
		/// Comparison against the completed number of lines is not (yet) possible, change #375
		/// "consider to migrate Byte/Line Count/Rate from model to domain" is required for this.
		/// </remarks>
		public static void WaitForReceivingAndAssertCounts(Domain.Terminal terminalRx, int expectedTotalByteCount, int expectedTotalLineCount = IgnoreCount, int timeout = WaitTimeoutForLineTransmission)
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

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Prepared for future use.")]
		public static void WaitForTransmissionAndAssertByteCount(Domain.Terminal terminalTx, Domain.Terminal terminalRx, int expectedTotalByteCount, int timeout = WaitTimeoutForLineTransmission)
		{
			WaitForTransmissionAndAssertCounts(terminalTx, terminalRx, expectedTotalByteCount, IgnoreCount, timeout);
		}

		/// <remarks>
		/// 'expectedPerCycleCharCount' does not need to be considered, since bytes are transmitted.
		/// </remarks>
		/// <remarks>
		/// 'expectedTotalLineCount' will be compared against the number of lines in the view,
		/// i.e. complete as well as incomplete lines.
		/// Comparison against the completed number of lines is not (yet) possible, change #375
		/// "consider to migrate Byte/Line Count/Rate from model to domain" is required for this.
		/// </remarks>
		public static void WaitForTransmissionAndAssertCounts(Domain.Terminal terminalTx, Domain.Terminal terminalRx, int expectedTotalByteCount, int expectedTotalLineCount = IgnoreCount, int timeout = WaitTimeoutForLineTransmission)
		{
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

		/// <summary></summary>
		public static void WaitForReverification()
		{
			Thread.Sleep(2 * WaitTimeoutForLineTransmission);
		}

		#endregion

		#region Assert
		//==========================================================================================
		// Assert
		//==========================================================================================

		/// <summary></summary>
		public static void AssertMatchingParserSettingsForSendText(TerminalSettings settingsA, TerminalSettings settingsB, out Encoding encoding, out Endianness endianness, out Domain.Parser.Mode mode)
		{
			var terminalTypeA = settingsA.TerminalType;
			var terminalTypeB = settingsB.TerminalType;
			Assert.That(terminalTypeA, Is.EqualTo(terminalTypeB));

			Encoding encodingA;
			Encoding encodingB;
			if (settingsA.TerminalType == TerminalType.Text)
			{
				encodingA = ((EncodingEx)settingsA.TextTerminal.Encoding).Encoding;
				encodingB = ((EncodingEx)settingsB.TextTerminal.Encoding).Encoding;
			}
			else
			{
				encodingA = ((EncodingEx)settingsA.BinaryTerminal.EncodingFixed).Encoding;
				encodingB = ((EncodingEx)settingsB.BinaryTerminal.EncodingFixed).Encoding;
			}
			Assert.That(encodingA, Is.EqualTo(encodingB));

			var endiannessA = settingsA.IO.Endianness;
			var endiannessB = settingsB.IO.Endianness;
			Assert.That(endiannessA, Is.EqualTo(endiannessB));

			var modeA = settingsA.Send.Text.ToParseMode();
			var modeB = settingsB.Send.Text.ToParseMode();
			Assert.That(modeA, Is.EqualTo(modeB));

			encoding = encodingA;
			endianness = endiannessA;
			mode = modeA;
		}

		/// <remarks>
		/// <see cref="WaitForSendingAndAssertCounts"/> further above.
		/// </remarks>
		public static void AssertTxCounts(Domain.Terminal terminalTx, int expectedTotalByteCount, int expectedTotalLineCount = IgnoreCount)
		{
			WaitForSendingAndAssertCounts(terminalTx, expectedTotalByteCount, expectedTotalLineCount, IgnoreTimeout); // Simply forward (yet).
		}

		/// <remarks>
		/// <see cref="WaitForReceivingAndAssertCounts"/> further above.
		/// </remarks>
		public static void AssertRxCounts(Domain.Terminal terminalRx, int expectedTotalByteCount, int expectedTotalLineCount = IgnoreCount)
		{
			WaitForReceivingAndAssertCounts(terminalRx, expectedTotalByteCount, expectedTotalLineCount, IgnoreTimeout); // Simply forward (yet).
		}

		/// <remarks>
		/// <see cref="WaitForTransmissionAndAssertCounts"/> further above.
		/// </remarks>
		public static void AssertCounts(Domain.Terminal terminalTx, Domain.Terminal terminalRx, int expectedTotalByteCount, int expectedTotalLineCount = IgnoreCount)
		{
			WaitForTransmissionAndAssertCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount, IgnoreTimeout); // Simply forward (yet).
		}

		/// <summary></summary>
		public static void AddAndAssertTxContent(Domain.Terminal terminal, string contentPatternToAdd, ref List<string> expectedContentPattern)
		{
			expectedContentPattern.Add(contentPatternToAdd);

			AssertTxContent(terminal, expectedContentPattern);
		}

		/// <summary></summary>
		public static void AddAndAssertBidirContent(Domain.Terminal terminal, string contentPatternToAdd, ref List<string> expectedContentPattern)
		{
			expectedContentPattern.Add(contentPatternToAdd);

			AssertBidirContent(terminal, expectedContentPattern);
		}

		/// <remarks>
		/// Same sequence of arguments as other verify methods.
		/// </remarks>
		/// <remarks>
		/// Using "1" / "2" since neither related to "A" / "B" nor "Tx" / "Rx" terminology.
		/// </remarks>
		public static void AddAndAssertBidirContent(Domain.Terminal terminal1, Domain.Terminal terminal2, string contentPatternToAdd, ref List<string> expectedContentPattern1, ref List<string> expectedContentPattern2)
		{
			AddAndAssertBidirContent(terminal1, terminal2, contentPatternToAdd, contentPatternToAdd, ref expectedContentPattern1, ref expectedContentPattern2);
		}

		/// <remarks>
		/// Same sequence of arguments as other verify methods.
		/// </remarks>
		/// <remarks>
		/// Using "1" / "2" since neither related to "A" / "B" nor "Tx" / "Rx" terminology.
		/// </remarks>
		public static void AddAndAssertBidirContent(Domain.Terminal terminal1, Domain.Terminal terminal2, string contentPatternToAdd1, string contentPatternToAdd2, ref List<string> expectedContentPattern1, ref List<string> expectedContentPattern2)
		{
			AddAndAssertBidirContent(terminal1, contentPatternToAdd1, ref expectedContentPattern1);
			AddAndAssertBidirContent(terminal2, contentPatternToAdd2, ref expectedContentPattern2);
		}

		/// <summary></summary>
		public static void AddAndAssertRxContent(Domain.Terminal terminal, string contentPatternToAdd, ref List<string> expectedContentPattern)
		{
			expectedContentPattern.Add(contentPatternToAdd);

			AssertRxContent(terminal, expectedContentPattern);
		}

		private static void AddAndAssertContent(Domain.Terminal terminal, RepositoryType repositoryType, string contentPatternToAdd, ref List<string> expectedContentPattern)
		{
			expectedContentPattern.Add(contentPatternToAdd);

			AssertContent(terminal, repositoryType, expectedContentPattern);
		}

		/// <summary></summary>
		public static void AssertTxContent(Domain.Terminal terminal, IEnumerable<string> expectedContentPattern)
		{
			AssertContent(terminal, RepositoryType.Tx, expectedContentPattern);
		}

		/// <summary></summary>
		public static void AssertBidirContent(Domain.Terminal terminal, IEnumerable<string> expectedContentPattern)
		{
			AssertContent(terminal, RepositoryType.Bidir, expectedContentPattern);
		}

		/// <remarks>
		/// Same sequence of arguments as other verify methods.
		/// </remarks>
		/// <remarks>
		/// Using "1" / "2" since neither related to "A" / "B" nor "Tx" / "Rx" terminology.
		/// </remarks>
		public static void AssertBidirContent(Domain.Terminal terminal1, Domain.Terminal terminal2, IEnumerable<string> expectedContentPattern1, IEnumerable<string> expectedContentPattern2)
		{
			AssertBidirContent(terminal1, expectedContentPattern1);
			AssertBidirContent(terminal2, expectedContentPattern2);
		}

		/// <summary></summary>
		public static void AssertRxContent(Domain.Terminal terminal, IEnumerable<string> expectedContentPattern)
		{
			AssertContent(terminal, RepositoryType.Rx, expectedContentPattern);
		}

		private static void AssertContent(Domain.Terminal terminal, RepositoryType repositoryType, IEnumerable<string> expectedContentPattern)
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
