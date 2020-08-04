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
using System.Threading;
using System.Windows.Forms;

using MKY;

using NUnit.Framework;

using YAT.Domain;

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

		#region Transmit
		//==========================================================================================
		// Transmit
		//==========================================================================================

		/// <summary></summary>
		public static void TransmitAndAssertTxCounts(Terminal terminalTx,
		                                             Domain.Parser.Parser parser, string text,
		                                             ref int expectedTotalByteCount, ref int expectedTotalLineCount,
		                                             int timeout = WaitTimeoutForLineTransmission)
		{
			TransmitAndAssertTxCountsWithOffset(terminalTx, parser, text, ref expectedTotalByteCount, ref expectedTotalLineCount, 0, timeout);
		}

		/// <summary></summary>
		public static void TransmitAndAssertTxCountsWithOffset(Terminal terminalTx,
		                                                       Domain.Parser.Parser parser, string text,
		                                                       ref int expectedTotalByteCount, ref int expectedTotalLineCount,
		                                                       int expectedTotalByteCountOffset,
		                                                       int timeout = WaitTimeoutForLineTransmission)
		{
			Domain.Test.Utilities.TransmitAndAssertTxCountsWithOffset(terminalTx.UnderlyingDomain_ForTestOnly,
			                                                         parser, text,
			                                                         ref expectedTotalByteCount, ref expectedTotalLineCount,
			                                                         expectedTotalByteCountOffset,
			                                                         timeout);
		}

		/// <summary></summary>
		public static void TransmitAndAssertRxCounts(Terminal terminalTx, Terminal terminalRx,
		                                             Domain.Parser.Parser parser, string text,
		                                             ref int expectedTotalByteCount, ref int expectedTotalLineCount,
		                                             int timeout = WaitTimeoutForLineTransmission)
		{
			TransmitAndAssertRxCountsWithOffset(terminalTx, terminalRx, parser, text, ref expectedTotalByteCount, ref expectedTotalLineCount, 0, timeout);
		}

		/// <summary></summary>
		public static void TransmitAndAssertRxCountsWithOffset(Terminal terminalTx, Terminal terminalRx,
		                                                       Domain.Parser.Parser parser, string text,
		                                                       ref int expectedTotalByteCount, ref int expectedTotalLineCount,
		                                                       int expectedTotalByteCountOffset,
		                                                       int timeout = WaitTimeoutForLineTransmission)
		{
			Domain.Test.Utilities.TransmitAndAssertRxCountsWithOffset(terminalTx.UnderlyingDomain_ForTestOnly, terminalRx.UnderlyingDomain_ForTestOnly,
			                                                          parser, text,
			                                                          ref expectedTotalByteCount, ref expectedTotalLineCount,
			                                                          expectedTotalByteCountOffset,
			                                                          timeout);
		}

		/// <summary></summary>
		public static void TransmitAndAssertCounts(Terminal terminalTx, Terminal terminalRx,
		                                           Domain.Parser.Parser parser, string text,
		                                           ref int expectedTotalByteCount, ref int expectedTotalLineCount,
		                                           int timeout = WaitTimeoutForLineTransmission)
		{
			TransmitAndAssertCountsWithOffset(terminalTx, terminalRx, parser, text, ref expectedTotalByteCount, ref expectedTotalLineCount, 0, timeout);
		}

		/// <summary></summary>
		public static void TransmitAndAssertCountsWithOffset(Terminal terminalTx, Terminal terminalRx,
		                                                     Domain.Parser.Parser parser, string text,
		                                                     ref int expectedTotalByteCount, ref int expectedTotalLineCount,
		                                                     int expectedTotalByteCountOffset,
		                                                     int timeout = WaitTimeoutForLineTransmission)
		{
			Domain.Test.Utilities.TransmitAndAssertCountsWithOffset(terminalTx.UnderlyingDomain_ForTestOnly, terminalRx.UnderlyingDomain_ForTestOnly,
			                                                        parser, text,
			                                                        ref expectedTotalByteCount, ref expectedTotalLineCount,
			                                                        expectedTotalByteCountOffset,
			                                                        timeout);
		}

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
		/// 'expectedTotalLineCount' will be compared against the number of lines in the view,
		/// i.e. complete as well as incomplete lines, *and* the number of complete lines!
		/// </remarks>
		public static void WaitForSendingAndAssertCounts(Terminal terminalTx, int expectedTotalByteCount, int expectedTotalLineCount, int timeout = WaitTimeoutForLineTransmission)
		{
			WaitForSendingAndAssertCounts(terminalTx, expectedTotalByteCount, expectedTotalLineCount, expectedTotalLineCount, timeout);
		}

		/// <summary></summary>
		public static void WaitForSendingAndAssertCounts(Terminal terminalTx, int expectedTotalByteCount, int expectedTotalLineCountDisplayed, int expectedTotalLineCountCompleted, int timeout = WaitTimeoutForLineTransmission)
		{
			WaitForSendingAndAssertCountsWithOffset(terminalTx, expectedTotalByteCount, expectedTotalLineCountDisplayed, expectedTotalLineCountCompleted, 0, timeout);
		}

		/// <remarks>
		/// 'expectedTotalLineCount' will be compared against the number of lines in the view,
		/// i.e. complete as well as incomplete lines, *and* the number of complete lines!
		/// </remarks>
		public static void WaitForSendingAndAssertCountsWithOffset(Terminal terminalTx, int expectedTotalByteCount, int expectedTotalLineCount, int expectedTotalByteCountOffset, int timeout = WaitTimeoutForLineTransmission)
		{
			WaitForSendingAndAssertCountsWithOffset(terminalTx, expectedTotalByteCount, expectedTotalLineCount, expectedTotalLineCount, expectedTotalByteCountOffset, timeout);
		}

		/// <summary></summary>
		public static void WaitForSendingAndAssertCountsWithOffset(Terminal terminalTx, int expectedTotalByteCount, int expectedTotalLineCountDisplayed, int expectedTotalLineCountCompleted, int expectedTotalByteCountOffset, int timeout = WaitTimeoutForLineTransmission)
		{
			Domain.Test.Utilities.WaitForSendingAndAssertCounts(terminalTx.UnderlyingDomain_ForTestOnly, (expectedTotalByteCount + expectedTotalByteCountOffset), expectedTotalLineCountDisplayed, timeout);

			Assert.That(terminalTx.TxByteCount, Is.EqualTo(expectedTotalByteCount));
			Assert.That(terminalTx.TxLineCount, Is.EqualTo(expectedTotalLineCountCompleted));
		}

		/// <remarks>
		/// 'expectedPerCycleCharCount' does not need to be considered, since bytes are transmitted.
		/// </remarks>
		public static void WaitForSendingCycleAndAssertCounts(Terminal terminalRx, TestSet testSet, int cycle)
		{
			int expectedTotalByteCount;
			int expectedTotalLineCountDisplayed;
			int expectedTotalLineCountCompleted;
			int timeout;
			CalculateValues(terminalRx, testSet, cycle, out expectedTotalByteCount, out expectedTotalLineCountDisplayed, out expectedTotalLineCountCompleted, out timeout);

			WaitForSendingAndAssertCounts(terminalRx, expectedTotalByteCount, expectedTotalLineCountDisplayed, expectedTotalLineCountCompleted, timeout);
		}

		/// <remarks>
		/// 'expectedTotalLineCount' will be compared against the number of lines in the view,
		/// i.e. complete as well as incomplete lines, *and* the number of complete lines!
		/// </remarks>
		public static void WaitForReceivingAndAssertCounts(Terminal terminalRx, int expectedTotalByteCount, int expectedTotalLineCount, int timeout = WaitTimeoutForLineTransmission)
		{
			WaitForReceivingAndAssertCounts(terminalRx, expectedTotalByteCount, expectedTotalLineCount, expectedTotalLineCount, timeout);
		}

		/// <summary></summary>
		public static void WaitForReceivingAndAssertCounts(Terminal terminalRx, int expectedTotalByteCount, int expectedTotalLineCountDisplayed, int expectedTotalLineCountCompleted, int timeout = WaitTimeoutForLineTransmission)
		{
			WaitForReceivingAndAssertCountsWithOffset(terminalRx, expectedTotalByteCount, expectedTotalLineCountDisplayed, expectedTotalLineCountCompleted, 0, timeout);
		}

		/// <remarks>
		/// 'expectedTotalLineCount' will be compared against the number of lines in the view,
		/// i.e. complete as well as incomplete lines, *and* the number of complete lines!
		/// </remarks>
		public static void WaitForReceivingAndAssertCountsWithOffset(Terminal terminalRx, int expectedTotalByteCount, int expectedTotalLineCount, int expectedTotalByteCountOffset, int timeout = WaitTimeoutForLineTransmission)
		{
			WaitForReceivingAndAssertCountsWithOffset(terminalRx, expectedTotalByteCount, expectedTotalLineCount, expectedTotalLineCount, expectedTotalByteCountOffset, timeout);
		}

		/// <summary></summary>
		public static void WaitForReceivingAndAssertCountsWithOffset(Terminal terminalRx, int expectedTotalByteCount, int expectedTotalLineCountDisplayed, int expectedTotalLineCountCompleted, int expectedTotalByteCountOffset, int timeout = WaitTimeoutForLineTransmission)
		{
			Domain.Test.Utilities.WaitForReceivingAndAssertCounts(terminalRx.UnderlyingDomain_ForTestOnly, (expectedTotalByteCount + expectedTotalByteCountOffset), expectedTotalLineCountDisplayed, timeout);

			Assert.That(terminalRx.RxByteCount, Is.EqualTo(expectedTotalByteCount));
			Assert.That(terminalRx.RxLineCount, Is.EqualTo(expectedTotalLineCountCompleted));
		}

		/// <remarks>
		/// 'expectedPerCycleCharCount' does not need to be considered, since bytes are transmitted.
		/// </remarks>
		public static void WaitForReceivingCycleAndAssertCounts(Terminal terminalRx, TestSet testSet, int cycle)
		{
			int expectedTotalByteCount;
			int expectedTotalLineCountDisplayed;
			int expectedTotalLineCountCompleted;
			int timeout;
			CalculateValues(terminalRx, testSet, cycle, out expectedTotalByteCount, out expectedTotalLineCountDisplayed, out expectedTotalLineCountCompleted, out timeout);

			WaitForReceivingAndAssertCounts(terminalRx, expectedTotalByteCount, expectedTotalLineCountDisplayed, expectedTotalLineCountCompleted, timeout);
		}

		/// <summary></summary>
		public static void WaitForTransmissionAndAssertCounts(Terminal terminalTx, Terminal terminalRx, TestSet testSet)
		{
			WaitForTransmissionAndAssertCounts(terminalTx, terminalRx, testSet.ExpectedTotalByteCount, testSet.ExpectedLineCountDisplayed, testSet.ExpectedLineCountCompleted);
		}

		/// <remarks>
		/// 'expectedPerCycleCharCount' does not need to be considered, since bytes are transmitted.
		/// </remarks>
		/// <remarks>
		/// 'expectedTotalLineCount' will be compared against the number of lines in the view,
		/// i.e. complete as well as incomplete lines, *and* the number of complete lines!
		/// </remarks>
		public static void WaitForTransmissionAndAssertCounts(Terminal terminalTx, Terminal terminalRx, int expectedTotalByteCount, int expectedTotalLineCount, int timeout = WaitTimeoutForLineTransmission)
		{
			WaitForTransmissionAndAssertCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount, expectedTotalLineCount, timeout);
		}

		/// <remarks>
		/// 'expectedPerCycleCharCount' does not need to be considered, since bytes are transmitted.
		/// </remarks>
		public static void WaitForTransmissionAndAssertCounts(Terminal terminalTx, Terminal terminalRx, int expectedTotalByteCount, int expectedTotalLineCountDisplayed, int expectedTotalLineCountCompleted, int timeout = WaitTimeoutForLineTransmission)
		{
			Domain.Test.Utilities.WaitForTransmissionAndAssertCounts(terminalTx.UnderlyingDomain_ForTestOnly, terminalRx.UnderlyingDomain_ForTestOnly, expectedTotalByteCount, expectedTotalLineCountDisplayed, timeout);

			Assert.That(terminalTx.TxByteCount, Is.EqualTo(expectedTotalByteCount));
			Assert.That(terminalTx.TxLineCount, Is.EqualTo(expectedTotalLineCountCompleted));
			Assert.That(terminalRx.RxByteCount, Is.EqualTo(expectedTotalByteCount));
			Assert.That(terminalRx.RxLineCount, Is.EqualTo(expectedTotalLineCountCompleted));
		}

		/// <remarks>
		/// 'expectedPerCycleCharCount' does not need to be considered, since bytes are transmitted.
		/// </remarks>
		public static void WaitForTransmissionCycleAndAssertCounts(Terminal terminalTx, Terminal terminalRx, TestSet testSet, int cycle)
		{
			int expectedTotalByteCount;
			int expectedTotalLineCountDisplayed;
			int expectedTotalLineCountCompleted;
			int timeout;
			CalculateValues(terminalRx, testSet, cycle, out expectedTotalByteCount, out expectedTotalLineCountDisplayed, out expectedTotalLineCountCompleted, out timeout);

			WaitForTransmissionAndAssertCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCountDisplayed, expectedTotalLineCountCompleted, timeout);
		}

		private static void CalculateValues(Terminal terminalRx, TestSet testSet, int cycle, out int expectedTotalByteCount, out int expectedTotalLineCountDisplayed, out int expectedTotalLineCountCompleted, out int timeout)
		{
			// Calculate total expected counts:
			expectedTotalByteCount          = (testSet.ExpectedTotalByteCount     * cycle);
			expectedTotalLineCountDisplayed = (testSet.ExpectedLineCountDisplayed * cycle);
			expectedTotalLineCountCompleted = (testSet.ExpectedLineCountCompleted * cycle);

			// Calculate timeout:
			int timeoutFactorPerLine = ((testSet.ExpectedLineCountCompleted > 0) ? (testSet.ExpectedLineCountCompleted) : (1)); // Take cases with 0 lines into account!
			timeout = (WaitTimeoutForLineTransmission * timeoutFactorPerLine);
		}

		/// <summary></summary>
		public static void WaitForReverification()
		{
			Domain.Test.Utilities.WaitForReverification();
		}

		#endregion

		#region Assert
		//==========================================================================================
		// Assert
		//==========================================================================================

		/// <summary></summary>
		public static void AssertMatchingParserSettingsForSendText(Terminal terminalA, Terminal terminalB, out Encoding parserEncoding, out Endianness parserEndianness, out Domain.Parser.Mode parserMode)
		{
			Domain.Test.Utilities.AssertMatchingParserSettingsForSendText(terminalA.SettingsRoot.Terminal, terminalB.SettingsRoot.Terminal, out parserEncoding, out parserEndianness, out parserMode);
		}

		/// <remarks>
		/// <see cref="WaitForSendingAndAssertCounts(Terminal, int, int, int, int)"/> further above.
		/// </remarks>
		public static void AssertTxCounts(Terminal terminalTx, int expectedTotalByteCount, int expectedTotalLineCountDisplayed)
		{
			WaitForSendingAndAssertCounts(terminalTx, expectedTotalByteCount, expectedTotalLineCountDisplayed, expectedTotalLineCountDisplayed, IgnoreTimeout); // Simply forward (yet).
		}

		/// <remarks>
		/// <see cref="WaitForSendingAndAssertCountsWithOffset(Terminal, int, int, int, int)"/> further above.
		/// </remarks>
		public static void AssertTxCountsWithOffset(Terminal terminalTx, int expectedTotalByteCount, int expectedTotalLineCountDisplayed, int expectedTotalByteCountOffset)
		{
			WaitForSendingAndAssertCountsWithOffset(terminalTx, expectedTotalByteCount, expectedTotalLineCountDisplayed, expectedTotalByteCountOffset, IgnoreTimeout); // Simply forward (yet).
		}

		/// <remarks>
		/// <see cref="WaitForReceivingAndAssertCounts(Terminal, int, int, int, int)"/> further above.
		/// </remarks>
		public static void AssertRxCounts(Terminal terminalRx, int expectedTotalByteCount, int expectedTotalLineCountDisplayed)
		{
			WaitForReceivingAndAssertCounts(terminalRx, expectedTotalByteCount, expectedTotalLineCountDisplayed, expectedTotalLineCountDisplayed, IgnoreTimeout); // Simply forward (yet).
		}

		/// <remarks>
		/// <see cref="WaitForReceivingAndAssertCountsWithOffset(Terminal, int, int, int, int)"/> further above.
		/// </remarks>
		public static void AssertRxCountsWithOffset(Terminal terminalTx, int expectedTotalByteCount, int expectedTotalLineCountDisplayed, int expectedTotalByteCountOffset)
		{
			WaitForReceivingAndAssertCountsWithOffset(terminalTx, expectedTotalByteCount, expectedTotalLineCountDisplayed, expectedTotalByteCountOffset, IgnoreTimeout); // Simply forward (yet).
		}

		/// <remarks>
		/// <see cref="WaitForTransmissionAndAssertCounts(Terminal, Terminal, int, int, int, int)"/> further above.
		/// </remarks>
		public static void AssertCounts(Terminal terminalTx, Terminal terminalRx, int expectedTotalByteCount, int expectedTotalLineCountDisplayed)
		{
			WaitForTransmissionAndAssertCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCountDisplayed, expectedTotalLineCountDisplayed, IgnoreTimeout); // Simply forward (yet).
		}

		/// <summary></summary>
		public static void AddAndAssertTxContent(Terminal terminal, string contentPatternToAdd, ref List<string> expectedContentPattern)
		{
			Domain.Test.Utilities.AddAndAssertTxContent(terminal.UnderlyingDomain_ForTestOnly, contentPatternToAdd, ref expectedContentPattern);
		}

		/// <summary></summary>
		public static void AddAndAssertBidirContent(Terminal terminal, string contentPatternToAdd, ref List<string> expectedContentPattern)
		{
			Domain.Test.Utilities.AddAndAssertBidirContent(terminal.UnderlyingDomain_ForTestOnly, contentPatternToAdd, ref expectedContentPattern);
		}

		/// <remarks>
		/// Same sequence of arguments as other verify methods.
		/// </remarks>
		/// <remarks>
		/// Using "1" / "2" since neither related to "A" / "B" nor "Tx" / "Rx" terminology.
		/// </remarks>
		public static void AddAndAssertBidirContent(Terminal terminal1, Terminal terminal2, string contentPatternToAdd, ref List<string> expectedContentPattern1, ref List<string> expectedContentPattern2)
		{
			AddAndAssertBidirContent(terminal1, terminal2, contentPatternToAdd, contentPatternToAdd, ref expectedContentPattern1, ref expectedContentPattern2);
		}

		/// <remarks>
		/// Same sequence of arguments as other verify methods.
		/// </remarks>
		/// <remarks>
		/// Using "1" / "2" since neither related to "A" / "B" nor "Tx" / "Rx" terminology.
		/// </remarks>
		public static void AddAndAssertBidirContent(Terminal terminal1, Terminal terminal2, string contentPatternToAdd1, string contentPatternToAdd2, ref List<string> expectedContentPattern1, ref List<string> expectedContentPattern2)
		{
			AddAndAssertBidirContent(terminal1, contentPatternToAdd1, ref expectedContentPattern1);
			AddAndAssertBidirContent(terminal2, contentPatternToAdd2, ref expectedContentPattern2);
		}

		/// <summary></summary>
		public static void AddAndAssertRxContent(Terminal terminal, string contentPatternToAdd, ref List<string> expectedContentPattern)
		{
			Domain.Test.Utilities.AddAndAssertRxContent(terminal.UnderlyingDomain_ForTestOnly, contentPatternToAdd, ref expectedContentPattern);
		}

		/// <summary></summary>
		public static void AssertTxContent(Terminal terminal, IEnumerable<string> expectedContentPattern)
		{
			Domain.Test.Utilities.AssertTxContent(terminal.UnderlyingDomain_ForTestOnly, expectedContentPattern);
		}

		/// <summary></summary>
		public static void AssertBidirContent(Terminal terminal, IEnumerable<string> expectedContentPattern)
		{
			Domain.Test.Utilities.AssertBidirContent(terminal.UnderlyingDomain_ForTestOnly, expectedContentPattern);
		}

		/// <remarks>
		/// Same sequence of arguments as other verify methods.
		/// </remarks>
		/// <remarks>
		/// Using "1" / "2" since neither related to "A" / "B" nor "Tx" / "Rx" terminology.
		/// </remarks>
		public static void AssertBidirContent(Terminal terminal1, Terminal terminal2, IEnumerable<string> expectedContentPattern1, IEnumerable<string> expectedContentPattern2)
		{
			AssertBidirContent(terminal1, expectedContentPattern1);
			AssertBidirContent(terminal2, expectedContentPattern2);
		}

		/// <summary></summary>
		public static void AssertRxContent(Terminal terminal, IEnumerable<string> expectedContentPattern)
		{
			Domain.Test.Utilities.AssertRxContent(terminal.UnderlyingDomain_ForTestOnly, expectedContentPattern);
		}

		/// <summary></summary>
		public static void AssertLines(Terminal terminalTx, Terminal terminalRx, TestSet testSet, int cycle = 1)
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
