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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using MKY;
using MKY.Text;

using NUnit.Framework;

//// 'YAT.Domain'        is not used due to ambiguity with 'YAT.Domain.Test.Terminal'.
//// 'YAT.Domain.Parser' is not used due to ambiguity with 'YAT.Domain.Test.Parser'.
using YAT.Domain.Settings;

#endregion

namespace YAT.Domain.Test
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
	public static class Utilities
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private enum ComparisonType
		{
			String,
			Regex
		}

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <summary></summary>
		public const int IgnoreCount = -1;

		/// <summary></summary>
		public const int IgnoreTimeout = 0;

		/// <remarks>
		/// State changes on a <see cref="MKY.IO.Serial.Socket.TcpAutoSocket"/> are the slowest,
		/// due to the nature of the AutoSocket to try this and that.
		/// </remarks>
		public const int WaitTimeoutForStateChange = 5000; // Same as 'MKY.IO.Serial.Socket.Test.Utilities.WaitTimeoutForStateChange'.

		/// <remarks>
		/// Note that a shorter interval would increase debug output, spoiling the debug console.
		/// </remarks>
		public const int WaitIntervalForStateChange = 100;

		/// <remarks>
		/// Timeout of 200 ms is too short for serial COM ports at 9600 baud, especially when
		/// debugger is connected. Measurements:
		///  > TripleLine (where timeout would be 3 * 200 ms = 600 ms) takes around 500 ms.
		///  > MultiLine (where timeout would be 26 * 200 ms = 5200 ms) takes around 5000 ms.
		///     => 300 ms seems defensive enough while still not too long to waste time.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'ms' is the proper abbreviation for milliseconds but StyleCop isn't able to deal with such abbreviations...")]
		public const int WaitTimeoutForLineTransmission = 300; // See remarks above.

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

		/// <summary></summary>
		public const string LineExceededWarningPattern = @"\[Warning: Maximal number of (characters|bytes) per line exceeded! Check the line break settings in Terminal > Settings > (Text|Binary) or increase the limit in Terminal > Settings > Advanced.\]";

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

		#region Estimate
		//==========================================================================================
		// Estimate
		//==========================================================================================

		/// <summary>
		/// The roughly estimated transmission time in milliseconds.
		/// </summary>
		/// <remarks>
		/// Value is approximate! It may be off by a factor of 2 or 3, depending on the settings.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "byte", Justification = "'Byte' not only is a type, it also tells the semantic.")]
		public static double GetRoughlyEstimatedTransmissionTime(TerminalSettings settings, int byteCount, int lineByteCount)
		{
			// Analysis and measurements 2020-08-11..17:
			//
			// 9600 baud:                                                (theoretically 960 bytes/s)
			//   >   9.5 s for     8'400 bytes (Normal)             @ NUnit/Debug =>   ~888 bytes/s
			//   >  94   s for    82'500 bytes (Large)              @ NUnit/Debug =>   ~875 bytes/s
			//
			// 115.2 kbaud:                                            (theoretically 11520 bytes/s)
			//   >   0.8 s for     8'400 bytes (Normal)             @ NUnit/Debug => ~10500 bytes/s
			//   >   7.5 s for    82'500 bytes (Large)              @ NUnit/Debug => ~11000 bytes/s
			//   >  94   s for 1'090'000 bytes (Huge)               @ NUnit/Debug => ~11500 bytes/s
			//
			// TCP/IP:
			//   >   0.3 s for     8'400 bytes (Normal)             @ NUnit/Debug => ~28000 bytes/s
			//   >   4.2 s for    97'300 bytes (LargeWithLongLines) @ NUnit/Debug => ~23000 bytes/s [Text]
			//   >   3.6 s for    82'500 bytes (Large)              @ NUnit/Debug => ~23000 bytes/s [Binary]
			//   >  42   s for 1'090'000 bytes (Huge)               @ NUnit/Debug => ~26000 bytes/s [Text]
			//   >  46   s for 1'089'792 bytes (Huge)               @ NUnit/Debug => ~23500 bytes/s [Binary]
			//
			// UDP/IP [Text]:
			//   >   0.4 s for     8'400 bytes (Normal)             @ NUnit/Debug => ~21000 bytes/s
			//   >   4.0 s for    82'500 bytes (Large)              @ NUnit/Debug => ~20500 bytes/s
			//   >  46   s for 1'090'000 bytes (Huge)               @ NUnit/Debug => ~23500 bytes/s

			var transmissionTime = (byteCount / settings.IO.ApproximateTypicalNumberOfBytesPerMillisecond);

			// Account for the longer initial delay when transmitting long lines:
			transmissionTime += (Math.Log10(lineByteCount) * 100); // Results in += ~300 ms for 'Long' and += ~400 ms for 'VeryLong'.

			// [Binary] takes a bit longer because formatting hex values is more time consuming:
			if (settings.TerminalType == TerminalType.Binary)
				transmissionTime *= 1.1;

			return (transmissionTime);
		}

		/// <summary>
		/// The roughly estimated overhead (initialization, verification, reverification,...) time in milliseconds.
		/// </summary>
		/// <remarks>
		/// Value is approximate! It may be off by a factor of 2 or 3, depending on the settings.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'reverification' is a correct English term.")]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "byte", Justification = "'Byte' not only is a type, it also tells the semantic.")]
		public static double GetRoughlyEstimatedOverheadTime(TerminalSettings settings, int byteCount)
		{
			var overheadBase = 1000; // Typical time to initialize and finalize test case.

			switch (settings.IO.IOType)
			{
				case IOType.TcpClient:                       // Typical time to establish connection.
				case IOType.TcpServer:     overheadBase +=  500; break;
				                                           //// Time needed to determined whether client or server.
				case IOType.TcpAutoSocket: overheadBase += 2000; break;

				default:                   /* Nothing to add. */ break;
			}

			// Analysis and measurement 2020-08-11:
			//
			// 9600 baud:                           strictly calculated:           estimated:
			//   >   1.4 s for     8'400 bytes (Normal) => ~167 us/byte => 1 s + ~47 us/byte
			//   >   4.5 s for    82'500 bytes (Large)  =>  ~55 us/byte => 1 s + ~42 us/byte
			//
			// 115.2 kbaud:                         strictly calculated:           estimated:
			//   >   1.4 s for     8'400 bytes (Normal) => ~167 us/byte => 1 s + ~47 us/byte
			//   >   4.5 s for    82'500 bytes (Large)  =>  ~55 us/byte => 1 s + ~42 us/byte
			//   >  48   s for 1'090'000 bytes (Huge)   =>  ~44 us/byte => 1 s + ~43 us/byte
			//
			// Analysis and measurement 2020-08-13:
			//
			// TCP/IP:
			//   >  10   s total for    82'500 bytes (Large) where base ~3 s and transmission  ~3 s =>  ~4 s => ~48 us/byte
			//   >  97   s total for 1'090'000 bytes (Huge)  where base ~3 s and transmission ~43 s => ~51 s => ~47 us/byte

			double verificationTime = (45E-3 * byteCount);

			// Note consequence of restriction of FR #412 "Make raw buffer size user configurable":
			// "Refresh and verify again" does not 100% work for transmission above 65536 bytes, thus skipped.

			return (overheadBase + verificationTime);
		}

		#endregion

		#region Transmit
		//==========================================================================================
		// Transmit
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "There are too many parameters to pass.")]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "3#", Justification = "Required for modifying the total count.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "4#", Justification = "Required for modifying the total count.")]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "byte", Justification = "'Byte' not only is a type, it also tells the semantic.")]
		public static void TransmitAndAssertTxCounts(Domain.Terminal terminalTx,
		                                             Domain.Parser.Parser parser, string text,
		                                             ref int expectedTotalByteCount, ref int expectedTotalLineCount,
		                                             int timeout = WaitTimeoutForLineTransmission)
		{
			TransmitAndAssertTxCountsWithOffset(terminalTx, parser, text, ref expectedTotalByteCount, ref expectedTotalLineCount, 0, timeout);
		}

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "There are too many parameters to pass.")]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "3#", Justification = "Required for modifying the total count.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "4#", Justification = "Required for modifying the total count.")]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "byte", Justification = "'Byte' not only is a type, it also tells the semantic.")]
		public static void TransmitAndAssertTxCountsWithOffset(Domain.Terminal terminalTx,
		                                                       Domain.Parser.Parser parser, string text,
		                                                       ref int expectedTotalByteCount, ref int expectedTotalLineCount,
		                                                       int expectedTotalByteCountOffset,
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
			WaitForSendingAndAssertCounts(terminalTx, (expectedTotalByteCount + expectedTotalByteCountOffset), expectedTotalLineCount, timeout);
		}

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "There are too many parameters to pass.")]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "4#", Justification = "Required for modifying the total count.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "5#", Justification = "Required for modifying the total count.")]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "byte", Justification = "'Byte' not only is a type, it also tells the semantic.")]
		public static void TransmitAndAssertRxCounts(Domain.Terminal terminalTx, Domain.Terminal terminalRx,
		                                             Domain.Parser.Parser parser, string text,
		                                             ref int expectedTotalByteCount, ref int expectedTotalLineCount,
		                                             int timeout = WaitTimeoutForLineTransmission)
		{
			TransmitAndAssertRxCountsWithOffset(terminalTx, terminalRx, parser, text, ref expectedTotalByteCount, ref expectedTotalLineCount, 0, timeout);
		}

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "There are too many parameters to pass.")]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "4#", Justification = "Required for modifying the total count.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "5#", Justification = "Required for modifying the total count.")]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "byte", Justification = "'Byte' not only is a type, it also tells the semantic.")]
		public static void TransmitAndAssertRxCountsWithOffset(Domain.Terminal terminalTx, Domain.Terminal terminalRx,
		                                                       Domain.Parser.Parser parser, string text,
		                                                       ref int expectedTotalByteCount, ref int expectedTotalLineCount,
		                                                       int expectedTotalByteCountOffset,
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
			WaitForReceivingAndAssertCounts(terminalRx, (expectedTotalByteCount + expectedTotalByteCountOffset), expectedTotalLineCount, timeout);
		}

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "There are too many parameters to pass.")]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "4#", Justification = "Required for modifying the total count.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "5#", Justification = "Required for modifying the total count.")]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "byte", Justification = "'Byte' not only is a type, it also tells the semantic.")]
		public static void TransmitAndAssertCounts(Domain.Terminal terminalTx, Domain.Terminal terminalRx,
		                                           Domain.Parser.Parser parser, string text,
		                                           ref int expectedTotalByteCount, ref int expectedTotalLineCount,
		                                           int timeout = WaitTimeoutForLineTransmission)
		{
			TransmitAndAssertCountsWithOffset(terminalTx, terminalRx, parser, text, ref expectedTotalByteCount, ref expectedTotalLineCount, 0, timeout);
		}

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "There are too many parameters to pass.")]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "4#", Justification = "Required for modifying the total count.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "5#", Justification = "Required for modifying the total count.")]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "byte", Justification = "'Byte' not only is a type, it also tells the semantic.")]
		public static void TransmitAndAssertCountsWithOffset(Domain.Terminal terminalTx, Domain.Terminal terminalRx,
		                                                     Domain.Parser.Parser parser, string text,
		                                                     ref int expectedTotalByteCount, ref int expectedTotalLineCount,
		                                                     int expectedTotalByteCountOffset,
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
			WaitForTransmissionAndAssertCounts(terminalTx, terminalRx, (expectedTotalByteCount + expectedTotalByteCountOffset), expectedTotalLineCount, timeout);
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

			Trace.WriteLine("Waiting for connection, 0 ms have passed, timeout is " + WaitTimeoutForStateChange + " ms...");

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

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static void WaitForIsSendingForSomeTime(Domain.Terminal terminal, int timeout = WaitTimeoutForIsSendingForSomeTime)
		{
			int waitTime = 0;

			Trace.WriteLine("Waiting for 'IsSendingForSomeTime', 0 ms have passed, timeout is " + timeout + " ms...");

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
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static void WaitForIsNoLongerSending(Domain.Terminal terminal, int timeout = WaitTimeoutForIsNoLongerSending)
		{
			int waitTime = 0;

			Trace.WriteLine("Waiting for 'IsNoLongerSending', 0 ms have passed, timeout is " + timeout + " ms...");

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
		public static void WaitForStop(Domain.Terminal terminal)
		{
			int waitTime = 0;

			Trace.WriteLine("Waiting for disconnection, 0 ms have passed, timeout is " + WaitTimeoutForStateChange + " ms...");

			while (!terminal.IsStopped)
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
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "byte", Justification = "'Byte' not only is a type, it also tells the semantic.")]
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
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "byte", Justification = "'Byte' not only is a type, it also tells the semantic.")]
		public static void WaitForSendingAndAssertCounts(Domain.Terminal terminalTx, int expectedTotalByteCount, int expectedTotalLineCount = IgnoreCount, int timeout = WaitTimeoutForLineTransmission)
		{
			bool isFirst = true; // Using do-while, first check state.
			int waitTime = 0;
			int waitIntervalForTransmission = TimeoutToInterval(timeout);
			int txByteCount = 0;
			int txLineCount = 0;
			StringBuilder sb;

			do
			{
				if (!isFirst) {
					Thread.Sleep(waitIntervalForTransmission);
					waitTime += waitIntervalForTransmission;
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
					if (timeout != IgnoreTimeout) {
						sb = new StringBuilder("Timeout! (" + timeout + " ms)");
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

				sb = new StringBuilder("Waiting for sending, ");
				sb.AppendFormat(CultureInfo.CurrentCulture, "{0}/{1} bytes/lines expected, {2}/{3} sent, ", expectedTotalByteCount, expectedTotalLineCount, txByteCount, txLineCount);
				if (timeout != IgnoreTimeout) {
					sb.AppendFormat(CultureInfo.CurrentCulture, "{0} ms have passed, timeout is {1} ms...", waitTime, timeout);
				}
				Trace.WriteLine(sb.ToString());

				if (isFirst) {
					isFirst = false;
				}
			}
			while ((txByteCount != expectedTotalByteCount) || ((txLineCount != expectedTotalLineCount) && (expectedTotalLineCount != IgnoreCount)));

			if (timeout != IgnoreTimeout) {
				Trace.WriteLine("...done, sent and verified");
			}
			else {
				Trace.WriteLine("Sending verified");
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "byte", Justification = "'Byte' not only is a type, it also tells the semantic.")]
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
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "byte", Justification = "'Byte' not only is a type, it also tells the semantic.")]
		public static void WaitForReceivingAndAssertCounts(Domain.Terminal terminalRx, int expectedTotalByteCount, int expectedTotalLineCount = IgnoreCount, int timeout = WaitTimeoutForLineTransmission)
		{
			bool isFirst = true; // Using do-while, first check state.
			int waitTime = 0;
			int waitIntervalForTransmission = TimeoutToInterval(timeout);
			int rxByteCount = 0;
			int rxLineCount = 0;
			StringBuilder sb;

			do
			{
				if (!isFirst) {
					Thread.Sleep(waitIntervalForTransmission);
					waitTime += waitIntervalForTransmission;
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
					if (timeout != IgnoreTimeout) {
						sb = new StringBuilder("Timeout! (" + timeout + " ms)");
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

				sb = new StringBuilder("Waiting for receiving, ");
				sb.AppendFormat(CultureInfo.CurrentCulture, "{0}/{1} bytes/lines expected, {2}/{3} received, ", expectedTotalByteCount, expectedTotalLineCount, rxByteCount, rxLineCount);
				if (timeout != IgnoreTimeout) {
					sb.AppendFormat(CultureInfo.CurrentCulture, "{0} ms have passed, timeout is {1} ms...", waitTime, timeout);
				}
				Trace.WriteLine(sb.ToString());

				if (isFirst) {
					isFirst = false;
				}
			}
			while ((rxByteCount != expectedTotalByteCount) || ((rxLineCount != expectedTotalLineCount) && (expectedTotalLineCount != IgnoreCount)));

			if (timeout != IgnoreTimeout) {
				Trace.WriteLine("...done, received and verified");
			}
			else {
				Trace.WriteLine("Receiving verified");
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Prepared for future use.")]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "byte", Justification = "'Byte' not only is a type, it also tells the semantic.")]
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
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "byte", Justification = "'Byte' not only is a type, it also tells the semantic.")]
		public static void WaitForTransmissionAndAssertCounts(Domain.Terminal terminalTx, Domain.Terminal terminalRx, int expectedTotalByteCount, int expectedTotalLineCount = IgnoreCount, int timeout = WaitTimeoutForLineTransmission)
		{
			bool isFirst = true; // Using do-while, first check state.
			int waitTime = 0;
			int waitIntervalForTransmission = TimeoutToInterval(timeout);
			int txByteCount = 0;
			int txLineCount = 0;
			int rxByteCount = 0;
			int rxLineCount = 0;
			StringBuilder sb;

			do
			{
				if (!isFirst) {
					Thread.Sleep(waitIntervalForTransmission);
					waitTime += waitIntervalForTransmission;
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
					if (timeout != IgnoreTimeout) {
						sb = new StringBuilder("Timeout! (" + timeout + " ms)");
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

				sb = new StringBuilder("Waiting for transmission, ");
				sb.AppendFormat(CultureInfo.CurrentCulture, "{0}/{1} bytes/lines expected, {2}/{3} sent, {4}/{5} received, ", expectedTotalByteCount, expectedTotalLineCount, txByteCount, txLineCount, rxByteCount, rxLineCount);
				if (timeout != IgnoreTimeout) {
					sb.AppendFormat(CultureInfo.CurrentCulture, "{0} ms have passed, timeout is {1} ms...", waitTime, timeout);
				}
				Trace.WriteLine(sb.ToString());

				if (isFirst) {
					isFirst = false;
				}
			}
			while ((txByteCount != expectedTotalByteCount) || ((txLineCount != expectedTotalLineCount) && (expectedTotalLineCount != IgnoreCount)) ||
			       (rxByteCount != expectedTotalByteCount) || ((rxLineCount != expectedTotalLineCount) && (expectedTotalLineCount != IgnoreCount)));

			if (timeout != IgnoreTimeout) {
				Trace.WriteLine("...done, transmitted and verified");
			}
			else {
				Trace.WriteLine("Transmission verified");
			}
		}

		private static int TimeoutToInterval(int timeout)
		{
			// Note that a longer minimum interval would increase the wait time for short tests, thus
			// increasing the test time. But also note that such short interval increases the overhead
			// of the test, thus increasing the test time of long tests. Adaption is the solution.
			const int MinimumWaitIntervalForTransmission = 20;

			// There shall be at least a trace output every now and then.
			const int MaximumWaitIntervalForTransmission = 600; // Same as 2 x WaitTimeoutForLineTransmission.

			// The standard timeout shall result in a typical number of updates.
			const int TypicalNumberOfUpdates = (WaitTimeoutForLineTransmission / MinimumWaitIntervalForTransmission); // 15.

			int interval = (timeout / TypicalNumberOfUpdates); // No need for higher accuracy (float or double), value will be limited anyway.
			return (Int32Ex.Limit(interval, MinimumWaitIntervalForTransmission, MaximumWaitIntervalForTransmission));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Reverification", Justification = "'Reverification' is a correct English term.")]
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
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "endianness", Justification = "'Endianness' is a correct English term.")]
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
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "byte", Justification = "'Byte' not only is a type, it also tells the semantic.")]
		public static void AssertTxCounts(Domain.Terminal terminalTx, int expectedTotalByteCount, int expectedTotalLineCount = IgnoreCount)
		{
			WaitForSendingAndAssertCounts(terminalTx, expectedTotalByteCount, expectedTotalLineCount, IgnoreTimeout); // Simply forward (yet).
		}

		/// <remarks>
		/// <see cref="WaitForReceivingAndAssertCounts"/> further above.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "byte", Justification = "'Byte' not only is a type, it also tells the semantic.")]
		public static void AssertRxCounts(Domain.Terminal terminalRx, int expectedTotalByteCount, int expectedTotalLineCount = IgnoreCount)
		{
			WaitForReceivingAndAssertCounts(terminalRx, expectedTotalByteCount, expectedTotalLineCount, IgnoreTimeout); // Simply forward (yet).
		}

		/// <remarks>
		/// <see cref="WaitForTransmissionAndAssertCounts"/> further above.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "byte", Justification = "'Byte' not only is a type, it also tells the semantic.")]
		public static void AssertCounts(Domain.Terminal terminalTx, Domain.Terminal terminalRx, int expectedTotalByteCount, int expectedTotalLineCount = IgnoreCount)
		{
			WaitForTransmissionAndAssertCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount, IgnoreTimeout); // Simply forward (yet).
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "Required for modifying the total content.")]
		public static void AddAndAssertTxContent(Domain.Terminal terminal, string contentToAdd, ref List<string> expectedContent)
		{
			expectedContent.Add(contentToAdd);

			AssertTxContent(terminal, expectedContent);
		}

		/// <remarks>
		/// Assertion will be based on <see cref="Regex"/> patterns.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "Required for modifying the total content.")]
		public static void AddAndAssertTxContentPattern(Domain.Terminal terminal, string contentPatternToAdd, ref List<string> expectedContentPattern)
		{
			expectedContentPattern.Add(contentPatternToAdd);

			AssertTxContentPattern(terminal, expectedContentPattern);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "Required for modifying the total content.")]
		public static void AddAndAssertBidirContent(Domain.Terminal terminal, string contentToAdd, ref List<string> expectedContent)
		{
			expectedContent.Add(contentToAdd);

			AssertBidirContent(terminal, expectedContent);
		}

		/// <remarks>
		/// Same sequence of arguments as other verify methods.
		/// </remarks>
		/// <remarks>
		/// Using "1" / "2" since neither related to "A" / "B" nor "Tx" / "Rx" terminology.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "3#", Justification = "Required for modifying the total content.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "4#", Justification = "Required for modifying the total content.")]
		public static void AddAndAssertBidirContent(Domain.Terminal terminal1, Domain.Terminal terminal2, string contentToAdd, ref List<string> expectedContent1, ref List<string> expectedContent2)
		{
			AddAndAssertBidirContent(terminal1, terminal2, contentToAdd, contentToAdd, ref expectedContent1, ref expectedContent2);
		}

		/// <remarks>
		/// Same sequence of arguments as other verify methods.
		/// </remarks>
		/// <remarks>
		/// Using "1" / "2" since neither related to "A" / "B" nor "Tx" / "Rx" terminology.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "4#", Justification = "Required for modifying the total content.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "5#", Justification = "Required for modifying the total content.")]
		public static void AddAndAssertBidirContent(Domain.Terminal terminal1, Domain.Terminal terminal2, string contentToAdd1, string contentToAdd2, ref List<string> expectedContent1, ref List<string> expectedContent2)
		{
			AddAndAssertBidirContent(terminal1, contentToAdd1, ref expectedContent1);
			AddAndAssertBidirContent(terminal2, contentToAdd2, ref expectedContent2);
		}

		/// <remarks>
		/// Assertion will be based on <see cref="Regex"/> patterns.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "Required for modifying the total content.")]
		public static void AddAndAssertBidirContentPattern(Domain.Terminal terminal, string contentPatternToAdd, ref List<string> expectedContentPattern)
		{
			expectedContentPattern.Add(contentPatternToAdd);

			AssertBidirContentPattern(terminal, expectedContentPattern);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "Required for modifying the total content.")]
		public static void AddAndAssertRxContent(Domain.Terminal terminal, string contentToAdd, ref List<string> expectedContent)
		{
			expectedContent.Add(contentToAdd);

			AssertRxContent(terminal, expectedContent);
		}

		/// <remarks>
		/// Assertion will be based on <see cref="Regex"/> patterns.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "Required for modifying the total content.")]
		public static void AddAndAssertRxContentPattern(Domain.Terminal terminal, string contentPatternToAdd, ref List<string> expectedContentPattern)
		{
			expectedContentPattern.Add(contentPatternToAdd);

			AssertRxContentPattern(terminal, expectedContentPattern);
		}

		/// <summary></summary>
		public static void AssertTxContent(Domain.Terminal terminal, IEnumerable<string> expectedContent)
		{
			AssertContent(terminal, RepositoryType.Tx, expectedContent, ComparisonType.String);
		}

		/// <remarks>
		/// Assertion will be based on <see cref="Regex"/> patterns.
		/// </remarks>
		public static void AssertTxContentPattern(Domain.Terminal terminal, IEnumerable<string> expectedContentPattern)
		{
			AssertContent(terminal, RepositoryType.Tx, expectedContentPattern, ComparisonType.Regex);
		}

		/// <summary></summary>
		public static void AssertBidirContent(Domain.Terminal terminal, IEnumerable<string> expectedContent)
		{
			AssertContent(terminal, RepositoryType.Bidir, expectedContent, ComparisonType.String);
		}

		/// <remarks>
		/// Same sequence of arguments as other verify methods.
		/// </remarks>
		/// <remarks>
		/// Using "1" / "2" since neither related to "A" / "B" nor "Tx" / "Rx" terminology.
		/// </remarks>
		public static void AssertBidirContent(Domain.Terminal terminal1, Domain.Terminal terminal2, IEnumerable<string> expectedContent1, IEnumerable<string> expectedContent2)
		{
			AssertBidirContent(terminal1, expectedContent1);
			AssertBidirContent(terminal2, expectedContent2);
		}

		/// <remarks>
		/// Assertion will be based on <see cref="Regex"/> patterns.
		/// </remarks>
		public static void AssertBidirContentPattern(Domain.Terminal terminal, IEnumerable<string> expectedContentPattern)
		{
			AssertContent(terminal, RepositoryType.Bidir, expectedContentPattern, ComparisonType.Regex);
		}

		/// <summary></summary>
		public static void AssertRxContent(Domain.Terminal terminal, IEnumerable<string> expectedContentPattern)
		{
			AssertContent(terminal, RepositoryType.Rx, expectedContentPattern, ComparisonType.String);
		}

		/// <remarks>
		/// Assertion will be based on <see cref="Regex"/> patterns.
		/// </remarks>
		public static void AssertRxContentPattern(Domain.Terminal terminal, IEnumerable<string> expectedContentPattern)
		{
			AssertContent(terminal, RepositoryType.Rx, expectedContentPattern, ComparisonType.Regex);
		}

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "'dl'/'ec' = DisplayLine/ExpectedContent.")]
		private static void AssertContent(Domain.Terminal terminal, RepositoryType repositoryType, IEnumerable<string> expectedContentOrPattern, ComparisonType comparisonType)
		{
			var displayLines = terminal.RepositoryToDisplayLines(repositoryType);

			var dlEnum = displayLines.GetEnumerator();
			var ecEnum = expectedContentOrPattern.GetEnumerator();

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
						Assert.Fail("Mismatching counts, terminal contains {0} lines but {1} lines are expected!", displayLines.Count, expectedContentOrPattern.Count());
				}

				i++;

				switch (comparisonType)
				{
					case ComparisonType.String:
					{
						var actual = dlEnum.Current.ToString();
						var content = ecEnum.Current;
						Assert.That(actual.Equals(content, StringComparison.Ordinal), Is.True, @"Line {0} is ""{1}"" mismatching expected ""{2}""", i, actual, content);
						break;
					}

					case ComparisonType.Regex:
					{
						var actual = dlEnum.Current.ToString();
						var pattern = ("^" + ecEnum.Current + "$");
						Assert.That(Regex.IsMatch(actual, pattern), Is.True, @"Line {0} is ""{1}"" mismatching expected ""{2}""", i, actual, pattern);
						break;
					}

					default:
					{
						throw (new ArgumentOutOfRangeException("comparisonType", comparisonType, MessageHelper.InvalidExecutionPreamble + "'" + comparisonType.ToString() + "' is a comparison type that is not (yet) supported!" + System.Environment.NewLine + System.Environment.NewLine + MessageHelper.SubmitBug));
					}
				}

				Assert.That(dlEnum.Current.TimeStamp, Is.GreaterThanOrEqualTo(previousLineTimeStamp));
				previousLineTimeStamp = dlEnum.Current.TimeStamp;
			}
		}

		/// <summary></summary>
		public static string EscapeRegex(string content)
		{
			return (EscapeParenthesis(content)); // Nothing else to escape needed yet.
		}

		/// <summary></summary>
		public static string EscapeParenthesis(string content)
		{
			string contentPattern = content;

			contentPattern = contentPattern.Replace("(", @"\(");
			contentPattern = contentPattern.Replace(")", @"\)");

			return (contentPattern);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
