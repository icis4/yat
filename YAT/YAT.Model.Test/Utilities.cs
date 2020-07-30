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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using MKY;

using NUnit.Framework;

#endregion

namespace YAT.Model.Test
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
	public static class Utilities
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		/// <remarks>
		/// \todo:
		/// This test set struct should be improved such that it can also handle expectations on the
		/// sender side (i.e. terminal A). Rationale: Testing of \!(Clear) behavior.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "This struct really belongs to these test utilities only.")]
		public struct TestSet : IEquatable<TestSet>
		{
			/// <summary>The test command.</summary>
			public Types.Command Command { get; }

			/// <summary>The expected number of completed lines as returned by <see cref="Terminal.RxLineCount"/> and <see cref="Terminal.TxLineCount"/>.</summary>
			public int ExpectedLineCountCompleted { get; }

			/// <summary>The expected number of display elements per display line, including incomplete lines.</summary>
			[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Don't care, straightforward test implementation.")]
			public int[] ExpectedElementCounts { get; }

			/// <summary>The expected number of shown characters per display line, ASCII mnemonics (e.g. &lt;CR&gt;) are considered a single shown character.</summary>
			[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Don't care, straightforward test implementation.")]
			public int[] ExpectedCharCounts { get; }

			/// <summary>The expected number of raw byte content per display line, without hidden EOL or control bytes.</summary>
			[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Don't care, straightforward test implementation.")]
			public int[] ExpectedByteCounts { get; }

			/// <summary>Flag indicating that expected values not only apply to Rx but also Tx.</summary>
			public bool ExpectedAlsoApplyToTx { get; }

			/// <summary>Flag indicating that cleared terminals are expected in the end.</summary>
			public bool ClearedIsExpectedInTheEnd { get; }

			/// <summary></summary>
			/// <param name="command">The test command.</param>
			public TestSet(Types.Command command)
			{
				Command = command;

				ExpectedLineCountCompleted = command.TextLines.Length;

				ExpectedElementCounts = new int[ExpectedLineCountCompleted];
				ExpectedCharCounts    = new int[ExpectedLineCountCompleted];
				ExpectedByteCounts    = new int[ExpectedLineCountCompleted];
				for (int i = 0; i < ExpectedLineCountCompleted; i++)
				{
					ExpectedElementCounts[i] = 4; // LineStart + Data + EOL + LineBreak.
					ExpectedCharCounts[i]    = command.TextLines[i].Length + 2; // Content + EOL.
					ExpectedByteCounts[i]    = command.TextLines[i].Length + 2; // Content + EOL.
				}

				ExpectedAlsoApplyToTx = true;
				ClearedIsExpectedInTheEnd = false;
			}

			/// <summary></summary>
			/// <param name="command">The test command.</param>
			/// <param name="expectedLineCount">The expected number of completed lines as returned by <see cref="Terminal.RxLineCount"/> and <see cref="Terminal.TxLineCount"/>.</param>
			/// <param name="expectedElementCounts">The expected number of display elements per display line, including incomplete lines.</param>
			/// <param name="expectedCharAndByteCounts">
			/// The expected number of shown characters per display line, ASCII mnemonics (e.g. &lt;CR&gt;) are considered a single shown character,
			/// which equals the expected number of raw byte content per display line, without hidden EOL or control bytes.
			/// </param>
			/// <param name="expectedAlsoApplyToTx">Flag indicating that expected values not only apply to Rx but also Tx.</param>
			/// <param name="clearedIsExpectedInTheEnd">Flag indicating that cleared terminals are expected in the end.</param>
			[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
			public TestSet(Types.Command command, int expectedLineCount, int[] expectedElementCounts, int[] expectedCharAndByteCounts, bool expectedAlsoApplyToTx, bool clearedIsExpectedInTheEnd = false)
				: this(command, expectedLineCount, expectedElementCounts, expectedCharAndByteCounts, expectedCharAndByteCounts, expectedAlsoApplyToTx, clearedIsExpectedInTheEnd)
			{
			}

			/// <summary></summary>
			/// <param name="command">The test command.</param>
			/// <param name="expectedLineCount">The expected number of completed lines as returned by <see cref="Terminal.RxLineCount"/> and <see cref="Terminal.TxLineCount"/>.</param>
			/// <param name="expectedElementCounts">The expected number of display elements per display line, including incomplete lines.</param>
			/// <param name="expectedCharCounts">The expected number of shown characters per display line, ASCII mnemonics (e.g. &lt;CR&gt;) are considered a single shown character.</param>
			/// <param name="expectedByteCounts">The expected number of raw byte content per display line, without hidden EOL or control bytes.</param>
			/// <param name="expectedAlsoApplyToTx">Flag indicating that expected values not only apply to Rx but also Tx.</param>
			/// <param name="clearedIsExpectedInTheEnd">Flag indicating that cleared terminals are expected in the end.</param>
			[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
			public TestSet(Types.Command command, int expectedLineCount, int[] expectedElementCounts, int[] expectedCharCounts, int[] expectedByteCounts, bool expectedAlsoApplyToTx, bool clearedIsExpectedInTheEnd = false)
			{
				Command = command;

				ExpectedLineCountCompleted = expectedLineCount;
				ExpectedElementCounts      = expectedElementCounts;
				ExpectedCharCounts         = expectedCharCounts;
				ExpectedByteCounts         = expectedByteCounts;
				ExpectedAlsoApplyToTx      = expectedAlsoApplyToTx;
				ClearedIsExpectedInTheEnd  = clearedIsExpectedInTheEnd;
			}

			/// <summary>The expected number of lines in the display, including incomplete lines.</summary>
			public int ExpectedLineCountDisplayed
			{
				get
				{
					return (ExpectedElementCounts.Length);
				}
			}

			/// <summary>The expected number of display elements in total.</summary>
			public int ExpectedTotalElementCount
			{
				get
				{
					int totalCount = 0;
					foreach (int count in ExpectedElementCounts)
						totalCount += count;

					return (totalCount);
				}
			}

			/// <summary>The expected number of shown characters in total.</summary>
			public int ExpectedTotalCharCount
			{
				get
				{
					int totalCount = 0;
					foreach (int count in ExpectedCharCounts)
						totalCount += count;

					return (totalCount);
				}
			}

			/// <summary>The expected number of raw byte content in total.</summary>
			public int ExpectedTotalByteCount
			{
				get
				{
					int totalCount = 0;
					foreach (int count in ExpectedByteCounts)
						totalCount += count;

					return (totalCount);
				}
			}

			#region Object Members
			//======================================================================================
			// Object Members
			//======================================================================================

			/// <summary>
			/// Serves as a hash function for a particular type.
			/// </summary>
			/// <remarks>
			/// Use properties instead of fields to calculate hash code. This ensures that 'intelligent'
			/// properties, i.e. properties with some logic, are also properly handled.
			/// </remarks>
			public override int GetHashCode()
			{
				unchecked
				{
					int hashCode = (Command != null ? Command.GetHashCode() : 0);

					hashCode = (hashCode * 397) ^  ExpectedLineCountCompleted                           .GetHashCode();
					hashCode = (hashCode * 397) ^ (ExpectedElementCounts != null ? ExpectedElementCounts.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (ExpectedCharCounts    != null ? ExpectedCharCounts   .GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (ExpectedByteCounts    != null ? ExpectedByteCounts   .GetHashCode() : 0);
					hashCode = (hashCode * 397) ^  ExpectedAlsoApplyToTx                                .GetHashCode();

					return (hashCode);
				}
			}

			/// <summary>
			/// Determines whether this instance and the specified object have value equality.
			/// </summary>
			public override bool Equals(object obj)
			{
				if (obj is TestSet)
					return (Equals((TestSet)obj));
				else
					return (false);
			}

			/// <summary>
			/// Determines whether this instance and the specified object have value equality.
			/// </summary>
			/// <remarks>
			/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
			/// properties, i.e. properties with some logic, are also properly handled.
			/// </remarks>
			public bool Equals(TestSet other)
			{
				return
				(
					ObjectEx                  .Equals(Command,               other.Command) &&
					ExpectedLineCountCompleted.Equals(                       other.ExpectedLineCountCompleted) &&
					ArrayEx             .ValuesEqual( ExpectedElementCounts, other.ExpectedElementCounts) &&
					ArrayEx             .ValuesEqual( ExpectedCharCounts,    other.ExpectedCharCounts) &&
					ArrayEx             .ValuesEqual( ExpectedByteCounts,    other.ExpectedByteCounts) &&
					ExpectedAlsoApplyToTx     .Equals(                       other.ExpectedAlsoApplyToTx)
				);
			}

			/// <summary>
			/// Determines whether the two specified objects have value equality.
			/// </summary>
			public static bool operator ==(TestSet lhs, TestSet rhs)
			{
				return (lhs.Equals(rhs));
			}

			/// <summary>
			/// Determines whether the two specified objects have value inequality.
			/// </summary>
			public static bool operator !=(TestSet lhs, TestSet rhs)
			{
				return (!(lhs == rhs));
			}

			#endregion
		}

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <summary></summary>
		public const int IgnoreTimeout = 0;

		                      /// <remarks><see cref="Domain.Test.Utilities.WaitTimeoutForStateChange"/>.</remarks>
		public const int WaitTimeoutForStateChange  = Domain.Test.Utilities.WaitTimeoutForStateChange;

		                      /// <remarks><see cref="Domain.Test.Utilities.WaitIntervalForStateChange"/>.</remarks>
		public const int WaitIntervalForStateChange = Domain.Test.Utilities.WaitIntervalForStateChange;

		                           /// <remarks><see cref="Domain.Test.Utilities.WaitTimeoutForLineTransmission"/>.</remarks>
		public const int WaitTimeoutForLineTransmission  = Domain.Test.Utilities.WaitTimeoutForLineTransmission;

		                       /// <remarks><see cref="Domain.Test.Utilities.WaitIntervalForTransmission"/>.</remarks>
		public const int WaitIntervalForTransmission = Domain.Test.Utilities.WaitIntervalForTransmission;

		#endregion

		#region Wait
		//==========================================================================================
		// Wait
		//==========================================================================================

		/// <remarks>
		/// There are similar utility methods in <see cref="Domain.Test.Utilities"/>.
		/// Changes here may have to be applied there too.
		/// </remarks>
		public static void WaitForStart(Terminal terminal)
		{
			int waitTime = 0;

			Trace.WriteLine("Waiting for start, " + waitTime + " ms have passed, timeout is " + WaitTimeoutForStateChange + " ms...");

			while (!terminal.IsStarted)
			{
				Thread.Sleep(WaitIntervalForStateChange);
				waitTime += WaitIntervalForStateChange;

				Trace.WriteLine("Waiting for start, " + waitTime + " ms have passed, timeout is " + WaitTimeoutForStateChange + " ms...");

				if (waitTime >= WaitTimeoutForStateChange)
					Assert.Fail("Start timeout!");
			}

			Trace.WriteLine("...done, started");
		}

		/// <remarks>
		/// There are similar utility methods in <see cref="Domain.Test.Utilities"/>.
		/// Changes here may have to be applied there too.
		/// </remarks>
		public static void WaitForOpen(Terminal terminal)
		{
			int waitTime = 0;

			Trace.WriteLine("Waiting for open, " + waitTime + " ms have passed, timeout is " + WaitTimeoutForStateChange + " ms...");

			while (!terminal.IsOpen)
			{
				Thread.Sleep(WaitIntervalForStateChange);
				waitTime += WaitIntervalForStateChange;

				Trace.WriteLine("Waiting for open, " + waitTime + " ms have passed, timeout is " + WaitTimeoutForStateChange + " ms...");

				if (waitTime >= WaitTimeoutForStateChange)
					Assert.Fail("Open timeout!");
			}

			Trace.WriteLine("...done, opened");
		}

		/// <remarks>
		/// There are similar utility methods in <see cref="Domain.Test.Utilities"/>.
		/// Changes here may have to be applied there too.
		/// </remarks>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Prepared for future use.")]
		public static void WaitForConnection(Terminal terminal)
		{
			int waitTime = 0;

			Trace.WriteLine("Waiting for connection, " + waitTime + " ms have passed, timeout is " + WaitTimeoutForStateChange + " ms...");

			while (!terminal.IsConnected)
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
		/// There are similar utility methods in <see cref="Domain.Test.Utilities"/>.
		/// Changes here may have to be applied there too.
		/// </remarks>
		public static void WaitForConnection(Terminal terminalA, Terminal terminalB)
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
		/// There are similar utility methods in <see cref="Domain.Test.Utilities"/>.
		/// Changes here may have to be applied there too.
		/// </remarks>
		public static void WaitForClose(Terminal terminal)
		{
			int waitTime = 0;

			Trace.WriteLine("Waiting for close, " + waitTime + " ms have passed, timeout is " + WaitTimeoutForStateChange + " ms...");

			while (terminal.IsOpen)
			{
				Thread.Sleep(WaitIntervalForStateChange);
				waitTime += WaitIntervalForStateChange;

				Trace.WriteLine("Waiting for close, " + waitTime + " ms have passed, timeout is " + WaitTimeoutForStateChange + " ms...");

				if (waitTime >= WaitTimeoutForStateChange)
					Assert.Fail("Close timeout!");
			}

			Trace.WriteLine("...done, closed");
		}

		/// <remarks>
		/// There are similar utility methods in <see cref="Domain.Test.Utilities"/>.
		/// Changes here may have to be applied there too.
		/// </remarks>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Prepared for future use.")]
		public static void WaitForDisconnection(Terminal terminal)
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

		/// <remarks>
		/// There are similar utility methods in <see cref="Domain.Test.Utilities"/>.
		/// Changes here may have to be applied there too.
		/// </remarks>
		/// <remarks>
		/// 'expectedTotalLineCount' will be compared against the number of lines in the view,
		/// i.e. complete as well as incomplete lines, *and* the number of complete lines!
		/// </remarks>
		public static void WaitForReceivingAndVerifyCounts(Terminal terminalRx, int expectedTotalByteCount, int expectedTotalLineCount)
		{
			WaitForReceivingAndVerifyCounts(terminalRx, expectedTotalByteCount, expectedTotalLineCount, expectedTotalLineCount);
		}

		/// <remarks>
		/// There are similar utility methods in <see cref="Domain.Test.Utilities"/>.
		/// Changes here may have to be applied there too.
		/// </remarks>
		public static void WaitForReceivingAndVerifyCounts(Terminal terminalRx, int expectedTotalByteCount, int expectedTotalLineCountDisplayed, int expectedTotalLineCountCompleted, int timeout = WaitTimeoutForLineTransmission)
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

				rxByteCount = terminalRx.GetRepositoryByteCount(Domain.RepositoryType.Rx);
				if (rxByteCount > expectedTotalByteCount) { // Break in case of too much data to improve speed of test.
					Assert.Fail("Number of received bytes = " + rxByteCount +
					            " mismatches expected = " + expectedTotalByteCount + ".");
				}

				rxLineCount = terminalRx.GetRepositoryLineCount(Domain.RepositoryType.Rx);
				if (rxLineCount > expectedTotalLineCountDisplayed) { // Break in case of too much data to improve speed of test.
					Assert.Fail("Number of received lines = " + rxLineCount +
					            " mismatches expected = " + expectedTotalLineCountDisplayed + ".");
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

					if (rxLineCount < expectedTotalLineCountDisplayed) {
						sb.Append(" Number of received lines = " + rxLineCount +
						          " mismatches expected = " + expectedTotalLineCountDisplayed + ".");
					}

					Assert.Fail(sb.ToString());
				}

				if (isFirst) {
					isFirst = false;
				}
			}
			while ((rxByteCount != expectedTotalByteCount) || (rxLineCount != expectedTotalLineCountDisplayed));

			Debug.WriteLine("Rx of " + rxByteCount + " bytes / " + rxLineCount + " lines completed");

			// Also assert count properties:
			Assert.That(terminalRx.RxByteCount, Is.EqualTo(expectedTotalByteCount));
			Assert.That(terminalRx.RxLineCount, Is.EqualTo(expectedTotalLineCountCompleted));

			if (timeout != IgnoreTimeout) {
				Trace.WriteLine("...done, received and verified");
			}
			else {
				Trace.WriteLine("Receiving verified");
			}
		}

		/// <remarks>
		/// There are similar utility methods in <see cref="Domain.Test.Utilities"/>.
		/// Changes here may have to be applied there too.
		/// </remarks>
		/// <remarks>
		/// 'expectedPerCycleCharCount' does not need to be considered, since bytes are transmitted.
		/// </remarks>
		public static void WaitForReceivingCycleAndVerifyCounts(Terminal terminalRx, TestSet testSet, int cycle)
		{
			// Calculate total expected counts at the receiver side:
			int expectedTotalByteCount          = (testSet.ExpectedTotalByteCount     * cycle);
			int expectedTotalLineCountDisplayed = (testSet.ExpectedLineCountDisplayed * cycle);
			int expectedTotalLineCountCompleted = (testSet.ExpectedLineCountCompleted * cycle);

			// Calculate timeout:
			int timeoutFactorPerLine = ((testSet.ExpectedLineCountCompleted > 0) ? (testSet.ExpectedLineCountCompleted) : (1)); // Take cases with 0 lines into account!
			int timeout = (WaitTimeoutForLineTransmission * timeoutFactorPerLine);

			WaitForReceivingAndVerifyCounts(terminalRx, expectedTotalByteCount, expectedTotalLineCountDisplayed, expectedTotalLineCountCompleted, timeout);
		}

		/// <remarks>
		/// <see cref="WaitForReceivingAndVerifyCounts(Terminal, int, int, int, int)"/> above.
		/// </remarks>
		public static void VerifyReceivedCounts(Terminal terminalRx, int expectedTotalByteCount, int expectedTotalLineCountDisplayed, int expectedTotalLineCountCompleted)
		{
			WaitForReceivingAndVerifyCounts(terminalRx, expectedTotalByteCount, expectedTotalLineCountDisplayed, expectedTotalLineCountCompleted, IgnoreTimeout);
		}

		/// <summary></summary>
		public static void WaitForTransmissionAndVerifyCounts(Terminal terminalTx, Terminal terminalRx, TestSet testSet)
		{
			WaitForTransmissionAndVerifyCounts(terminalTx, terminalRx, testSet.ExpectedTotalByteCount, testSet.ExpectedLineCountDisplayed, testSet.ExpectedLineCountCompleted);
		}

		/// <remarks>
		/// There are similar utility methods in <see cref="Domain.Test.Utilities"/>.
		/// Changes here may have to be applied there too.
		/// </remarks>
		/// <remarks>
		/// 'expectedPerCycleCharCount' does not need to be considered, since bytes are transmitted.
		/// </remarks>
		/// <remarks>
		/// 'expectedTotalLineCount' will be compared against the number of lines in the view,
		/// i.e. complete as well as incomplete lines, *and* the number of complete lines!
		/// </remarks>
		public static void WaitForTransmissionAndVerifyCounts(Terminal terminalTx, Terminal terminalRx, int expectedTotalByteCount, int expectedTotalLineCount)
		{
			WaitForTransmissionAndVerifyCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount, expectedTotalLineCount);
		}

		/// <remarks>
		/// There are similar utility methods in <see cref="Domain.Test.Utilities"/>.
		/// Changes here may have to be applied there too.
		/// </remarks>
		/// <remarks>
		/// 'expectedPerCycleCharCount' does not need to be considered, since bytes are transmitted.
		/// </remarks>
		public static void WaitForTransmissionAndVerifyCounts(Terminal terminalTx, Terminal terminalRx, int expectedTotalByteCount, int expectedTotalLineCountDisplayed, int expectedTotalLineCountCompleted, int timeout = WaitTimeoutForLineTransmission)
		{
			// Attention:
			// Similar code exists in Domain.Test.Utilities.WaitForTransmissionAndVerifyCounts().
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

				txByteCount = terminalTx.GetRepositoryByteCount(Domain.RepositoryType.Tx);
				if (txByteCount > expectedTotalByteCount) { // Break in case of too much data to improve speed of test.
					Assert.Fail("Number of sent bytes = " + txByteCount +
					            " mismatches expected = " + expectedTotalByteCount + ".");
				}

				txLineCount = terminalTx.GetRepositoryLineCount(Domain.RepositoryType.Tx);
				if (txLineCount > expectedTotalLineCountDisplayed) { // Break in case of too much data to improve speed of test.
					Assert.Fail("Number of sent lines = " + txLineCount +
					            " mismatches expected = " + expectedTotalLineCountDisplayed + ".");
				}

				rxByteCount = terminalRx.GetRepositoryByteCount(Domain.RepositoryType.Rx);
				if (rxByteCount > expectedTotalByteCount) { // Break in case of too much data to improve speed of test.
					Assert.Fail("Number of received bytes = " + rxByteCount +
					            " mismatches expected = " + expectedTotalByteCount + ".");
				}

				rxLineCount = terminalRx.GetRepositoryLineCount(Domain.RepositoryType.Rx);
				if (rxLineCount > expectedTotalLineCountDisplayed) { // Break in case of too much data to improve speed of test.
					Assert.Fail("Number of received lines = " + rxLineCount +
					            " mismatches expected = " + expectedTotalLineCountDisplayed + ".");
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

					if (txLineCount < expectedTotalLineCountDisplayed) {
						sb.Append(" Number of sent lines = " + txLineCount +
						          " mismatches expected = " + expectedTotalLineCountDisplayed + ".");
					}

					if (rxByteCount < expectedTotalByteCount) {
						sb.Append(" Number of received bytes = " + rxByteCount +
						          " mismatches expected = " + expectedTotalByteCount + ".");
					}

					if (rxLineCount < expectedTotalLineCountDisplayed) {
						sb.Append(" Number of received lines = " + rxLineCount +
						          " mismatches expected = " + expectedTotalLineCountDisplayed + ".");
					}

					Assert.Fail(sb.ToString());
				}

				if (isFirst) {
					isFirst = false;
				}
			}
			while ((txByteCount != expectedTotalByteCount) || (txLineCount != expectedTotalLineCountDisplayed) ||
			       (rxByteCount != expectedTotalByteCount) || (rxLineCount != expectedTotalLineCountDisplayed));

			Debug.WriteLine("Tx of " + txByteCount + " bytes / " + txLineCount + " lines completed");
			Debug.WriteLine("Rx of " + rxByteCount + " bytes / " + rxLineCount + " lines completed");

			// Also assert count properties:
			Assert.That(terminalTx.TxByteCount, Is.EqualTo(expectedTotalByteCount));
			Assert.That(terminalTx.TxLineCount, Is.EqualTo(expectedTotalLineCountCompleted));
			Assert.That(terminalRx.RxByteCount, Is.EqualTo(expectedTotalByteCount));
			Assert.That(terminalRx.RxLineCount, Is.EqualTo(expectedTotalLineCountCompleted));

			if (timeout != IgnoreTimeout) {
				Trace.WriteLine("...done, transmitted and verified");
			}
			else {
				Trace.WriteLine("Transmission verified");
			}
		}

		/// <remarks>
		/// There are similar utility methods in <see cref="Domain.Test.Utilities"/>.
		/// Changes here may have to be applied there too.
		/// </remarks>
		/// <remarks>
		/// 'expectedPerCycleCharCount' does not need to be considered, since bytes are transmitted.
		/// </remarks>
		public static void WaitForTransmissionCycleAndVerifyCounts(Terminal terminalTx, Terminal terminalRx, TestSet testSet, int cycle)
		{
			// Calculate total expected counts at the receiver side:
			int expectedTotalByteCount          = (testSet.ExpectedTotalByteCount     * cycle);
			int expectedTotalLineCountDisplayed = (testSet.ExpectedLineCountDisplayed * cycle);
			int expectedTotalLineCountCompleted = (testSet.ExpectedLineCountCompleted * cycle);

			// Calculate timeout:
			int timeoutFactorPerLine = ((testSet.ExpectedLineCountCompleted > 0) ? (testSet.ExpectedLineCountCompleted) : (1)); // Take cases with 0 lines into account!
			int timeout = (WaitTimeoutForLineTransmission * timeoutFactorPerLine);

			WaitForTransmissionAndVerifyCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCountDisplayed, expectedTotalLineCountCompleted, timeout);
		}

		/// <remarks>
		/// <see cref="WaitForTransmissionAndVerifyCounts(Terminal, Terminal, int, int, int, int)"/> above.
		/// </remarks>
		public static void VerifyCounts(Terminal terminalTx, Terminal terminalRx, int expectedTotalByteCount, int expectedTotalLineCountDisplayed, int expectedTotalLineCountCompleted)
		{
			WaitForTransmissionAndVerifyCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCountDisplayed, expectedTotalLineCountCompleted, IgnoreTimeout);
		}

		/// <remarks>
		/// There are similar utility methods in <see cref="Domain.Test.Utilities"/>.
		/// Changes here may have to be applied there too.
		/// </remarks>
		public static void WaitForReverification()
		{
			Thread.Sleep(2 * WaitTimeoutForLineTransmission);
		}

		#endregion

		#region Verifications
		//==========================================================================================
		// Verifications
		//==========================================================================================

		/// <summary></summary>
		public static void VerifyLines(Terminal terminalTx, Terminal terminalRx, TestSet testSet, int cycle = 1)
		{
			var displayLinesTx = terminalTx.RepositoryToDisplayLines(Domain.RepositoryType.Tx);
			var displayLinesRx = terminalRx.RepositoryToDisplayLines(Domain.RepositoryType.Rx);

			// Attention: Display line count is not always equal to terminal line count!
			//  > Display line count = number of lines in view
			//  > Terminal line count = number of *completed* lines in terminal
			// This function uses display line count for verification!

			// Calculate total expected display line count at the receiver side:
			int expectedTotalDisplayLineCountRx = 0;
			if (testSet.ExpectedElementCounts != null)
				expectedTotalDisplayLineCountRx = (testSet.ExpectedElementCounts.Length * cycle);

			// Compare the expected line count at the receiver side:
			if (displayLinesRx.Count != expectedTotalDisplayLineCountRx)
			{
				var sbRx = new StringBuilder();
				foreach (Domain.DisplayLine displayLineRx in displayLinesRx)
					sbRx.Append(ArrayEx.ValuesToString(displayLineRx.ToArray()));

				Console.Error.Write
				(
					"Rx:" + Environment.NewLine + sbRx + Environment.NewLine
				);

				Assert.Fail
				(
					"Line count mismatches: " + Environment.NewLine +
					"Expected = " + expectedTotalDisplayLineCountRx + " line(s), " +
					"Rx = " + displayLinesRx.Count + " line(s)." + Environment.NewLine +
					@"See ""Output"" for details."
				);
			}

			// If both sides are expected to show the same line count, compare the counts,
			// otherwise, ignore the comparision:
			if (testSet.ExpectedAlsoApplyToTx && !testSet.ClearedIsExpectedInTheEnd)
			{
				if (displayLinesRx.Count == displayLinesTx.Count)
				{
					for (int i = 0; i < displayLinesTx.Count; i++)
					{
						int index                = i % testSet.ExpectedElementCounts.Length;
						int expectedElementCount =     testSet.ExpectedElementCounts[index];
						int expectedCharCount    =     testSet.ExpectedCharCounts[index];
						int expectedByteCount    =     testSet.ExpectedByteCounts[index];

						var displayLineTx = displayLinesTx[i];
						var displayLineRx = displayLinesRx[i];

						if ((displayLineRx.Count     == displayLineTx.Count)     &&
							(displayLineRx.Count     == expectedElementCount)   &&
							(displayLineRx.CharCount == displayLineTx.CharCount) &&
							(displayLineRx.CharCount == expectedCharCount)      &&
							(displayLineRx.ByteCount == displayLineTx.ByteCount) &&
							(displayLineRx.ByteCount == expectedByteCount))
						{
							for (int j = 0; j < displayLineTx.Count; j++)
								Assert.That(displayLineRx[j].Text, Is.EqualTo(displayLineTx[j].Text));
						}
						else
						{
							string strTx = ArrayEx.ValuesToString(displayLineTx.ToArray());
							string strRx = ArrayEx.ValuesToString(displayLineRx.ToArray());

							Console.Error.Write
							(
								"Tx:" + Environment.NewLine + strTx + Environment.NewLine +
								"Rx:" + Environment.NewLine + strRx + Environment.NewLine
							);

							Assert.Fail
							(
								"Length of line " + i + " mismatches:" + Environment.NewLine +
								"Expected = " + expectedElementCount + " element(s), " +
								"Tx = " + displayLineTx.Count + " element(s), " +
								"Rx = " + displayLineRx.Count + " element(s)," + Environment.NewLine +
								"Expected = " + expectedCharCount + " char(s), " +
								"Tx = " + displayLineTx.CharCount + " char(s), " +
								"Rx = " + displayLineRx.CharCount + " char(s)." + Environment.NewLine +
								"Expected = " + expectedByteCount + " byte(s), " +
								"Tx = " + displayLineTx.ByteCount + " byte(s), " +
								"Rx = " + displayLineRx.ByteCount + " byte(s)." + Environment.NewLine +
								@"See ""Output"" for details."
							);
						}
					}
				}
				else
				{
					var sbTx = new StringBuilder();
					foreach (Domain.DisplayLine displayLineTx in displayLinesTx)
						sbTx.Append(ArrayEx.ValuesToString(displayLineTx.ToArray()));

					var sbRx = new StringBuilder();
					foreach (Domain.DisplayLine displayLineRx in displayLinesRx)
						sbRx.Append(ArrayEx.ValuesToString(displayLineRx.ToArray()));

					Console.Error.Write
					(
						"Tx:" + Environment.NewLine + sbTx + Environment.NewLine +
						"Rx:" + Environment.NewLine + sbRx + Environment.NewLine
					);

					Assert.Fail
					(
						"Line count mismatches: " + Environment.NewLine +
						"Expected = " + expectedTotalDisplayLineCountRx + " line(s), " +
						"Tx = " + displayLinesTx.Count + " line(s), " +
						"Rx = " + displayLinesRx.Count + " line(s)." + Environment.NewLine +
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
			if (e.Text.StartsWith("Unable to start terminal", StringComparison.Ordinal)) // 'Ordinal' since YAT is all-English and test is passable with this strict comparison.
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
