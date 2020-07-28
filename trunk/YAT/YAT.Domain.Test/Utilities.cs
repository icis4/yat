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
			do                         // Initially wait to allow async send,
			{                          //   therefore, use do-while.
				Thread.Sleep(WaitIntervalForStateChange);
				waitTime += WaitIntervalForStateChange;

				Trace.WriteLine("Waiting for connection, " + waitTime + " ms have passed, timeout is " + WaitTimeoutForStateChange + " ms...");

				if (waitTime >= WaitTimeoutForStateChange) {
					Assert.Fail("Connect timeout!");
				}
			}
			while (!terminalA.IsConnected && !terminalB.IsConnected);

			Trace.WriteLine("...done, connected");
		}

		/// <remarks>
		/// There are similar utility methods in 'Model.Test.Utilities'.
		/// Changes here may have to be applied there too.
		/// </remarks>
		public static void WaitForDisconnection(Domain.Terminal terminal)
		{
			int waitTime = 0;
			while (terminal.IsConnected)
			{
				Thread.Sleep(WaitIntervalForStateChange);
				waitTime += WaitIntervalForStateChange;

				Trace.WriteLine("Waiting for disconnection, " + waitTime + " ms have passed, timeout is " + WaitTimeoutForStateChange + " ms...");

				if (waitTime >= WaitTimeoutForStateChange) {
					Assert.Fail("Disconnect timeout!");
				}
			}

			Trace.WriteLine("...done, disconnected");
		}

		/// <summary></summary>
		public static void WaitForIsSendingForSomeTime(Domain.Terminal terminal, int timeout = WaitTimeoutForIsSendingForSomeTime)
		{
			int waitTime = 0;
			while (!terminal.IsSendingForSomeTime)
			{
				Thread.Sleep(WaitIntervalForIsSendingForSomeTime);
				waitTime += WaitIntervalForIsSendingForSomeTime;

				Trace.WriteLine("Waiting for 'IsSendingForSomeTime', " + waitTime + " ms have passed, timeout is " + timeout + " ms...");

				if (waitTime >= timeout) {
					Assert.Fail("'IsSendingForSomeTime' timeout!");
				}
			}

			Trace.WriteLine("...done, 'IsSendingForSomeTime'");
		}

		/// <summary></summary>
		public static void WaitForIsNoLongerSending(Domain.Terminal terminal, int timeout = WaitTimeoutForIsNoLongerSending)
		{
			int waitTime = 0;
			while (terminal.IsSending)
			{
				Thread.Sleep(WaitIntervalForIsNoLongerSending);
				waitTime += WaitIntervalForIsNoLongerSending;

				Trace.WriteLine("Waiting for 'IsNoLongerSending', " + waitTime + " ms have passed, timeout is " + timeout + " ms...");

				if (waitTime >= timeout) {
					Assert.Fail("'IsNoLongerSending' timeout!");
				}
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
			do                         // Initially wait to allow async send,
			{                          //   therefore, use do-while.
				Thread.Sleep(WaitIntervalForTransmission);
				waitTime += WaitIntervalForTransmission;

				Trace.WriteLine("Waiting for sending, " + waitTime + " ms have passed, timeout is " + timeout + " ms...");

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

				if (waitTime >= timeout) {
					var sb = new StringBuilder("Timeout!");

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
			}
			while ((txByteCount != expectedTotalByteCount) || ((txLineCount != expectedTotalLineCount) && (expectedTotalLineCount != IgnoreCount)));

			Debug.WriteLine("Tx of " + txByteCount + " bytes / " + txLineCount + " lines completed");

			Trace.WriteLine("...done, sent and verified");
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
			do                         // Initially wait to allow async send,
			{                          //   therefore, use do-while.
				Thread.Sleep(WaitIntervalForTransmission);
				waitTime += WaitIntervalForTransmission;

				Trace.WriteLine("Waiting for receiving, " + waitTime + " ms have passed, timeout is " + timeout + " ms...");

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

				if (waitTime >= timeout) {
					var sb = new StringBuilder("Timeout!");

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
			}
			while ((rxByteCount != expectedTotalByteCount) || (rxLineCount != expectedTotalLineCount && (expectedTotalLineCount != IgnoreCount)));

			Debug.WriteLine("Rx of " + rxByteCount + " bytes / " + rxLineCount + " lines completed");

			Trace.WriteLine("...done, received and verified");
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
			do                         // Initially wait to allow async send,
			{                          //   therefore, use do-while.
				Thread.Sleep(WaitIntervalForTransmission);
				waitTime += WaitIntervalForTransmission;

				Trace.WriteLine("Waiting for transmission, " + waitTime + " ms have passed, timeout is " + timeout + " ms...");

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

				if (waitTime >= timeout) {
					var sb = new StringBuilder("Timeout!");

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
			}
			while ((txByteCount != expectedTotalByteCount) || (txLineCount != expectedTotalLineCount && (expectedTotalLineCount != IgnoreCount)) ||
			       (rxByteCount != expectedTotalByteCount) || (rxLineCount != expectedTotalLineCount && (expectedTotalLineCount != IgnoreCount)));

			Debug.WriteLine("Tx of " + txByteCount + " bytes / " + txLineCount + " lines completed");
			Debug.WriteLine("Rx of " + rxByteCount + " bytes / " + rxLineCount + " lines completed");

			Trace.WriteLine("...done, transmitted and verified");
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

		/// <remarks>
		/// There are similar utility methods in 'Model.Test.Utilities'.
		/// Changes here may have to be applied there too.
		/// </remarks>
		public static void VerifyBidirContent(Domain.Terminal terminal, IList<string> contentPattern)
		{
			VerifyContent(terminal, RepositoryType.Bidir, contentPattern);
		}

		/// <remarks>
		/// There are similar utility methods in 'Model.Test.Utilities'.
		/// Changes here may have to be applied there too.
		/// </remarks>
		public static void VerifyContent(Domain.Terminal terminal, RepositoryType repositoryType, IList<string> contentPattern)
		{
			var displayLines = terminal.RepositoryToDisplayLines(repositoryType);
			Assert.That(displayLines.Count, Is.EqualTo(contentPattern.Count));

			var previousLineTimeStamp = DateTime.MinValue;
			for (int i = 0; i < displayLines.Count; i++)
			{
				var dl = displayLines[i];

				var input = dl.ToString();
				var pattern = DecoratePattern(contentPattern[i]);
				Assert.That(Regex.IsMatch(input, pattern), Is.True, @"Line {0} is ""{1}"" mismatching expected ""{2}""", i, input, pattern);

				Assert.That(dl.TimeStamp, Is.GreaterThanOrEqualTo(previousLineTimeStamp));
				previousLineTimeStamp = dl.TimeStamp;
			}
		}

		/// <remarks>
		/// There are similar utility methods in 'Model.Test.Utilities'.
		/// Changes here may have to be applied there too.
		/// </remarks>
		public static string DecoratePattern(string contentPattern)
		{
			contentPattern = contentPattern.Replace("(", @"\(");
			contentPattern = contentPattern.Replace(")", @"\)");

			return ("^" + contentPattern + "$");
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
