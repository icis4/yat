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

#region Configuration
//==================================================================================================
// Configuration
//==================================================================================================

#if (DEBUG)

	// Enable debugging of test case data generation:
////#define DEBUG_TEST_CASE_DATA

#endif // DEBUG

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using MKY;
using MKY.IO.Serial.SerialPort;
using MKY.Text;

using NUnit.Framework;
using NUnitEx;

using YAT.Domain.Settings;
using YAT.Domain.Utilities;

#endregion

namespace YAT.Domain.Test.Terminal
{
	#region Enums

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum SettingsFlavor
	{
		Default,
		Modified
	}

	/// <summary></summary>
	public enum SendMethod
	{
	////Raw is not implemented/used by this test (yet).
		Text,
		TextLine,
		TextLines,
		File
	}

	#pragma warning restore 1591

	#endregion

	/// <summary></summary>
	public static class SendTestData
	{
		#region Test Values
		//==========================================================================================
		// Test Values
		//==========================================================================================

		/// <summary></summary>
		public static IEnumerable<Tuple<StressFile, SendMethod>> TextFilePairs
		{
			get
			{
				var l = new List<Tuple<StressFile, SendMethod>>(8); // Preset the required capacity to improve memory management.
				l.Add(new Tuple<StressFile, SendMethod>(StressFile.Normal,                SendMethod.File));
				l.Add(new Tuple<StressFile, SendMethod>(StressFile.Large,                 SendMethod.File)); // 'Large' is needed for serial COM ports with defaults, 'Huge' would take too long.
				l.Add(new Tuple<StressFile, SendMethod>(StressFile.LargeWithLongLines,    SendMethod.File));
				l.Add(new Tuple<StressFile, SendMethod>(StressFile.Huge,                  SendMethod.File)); // 'Enormous' would take too long.
				l.Add(new Tuple<StressFile, SendMethod>(StressFile.HugeWithVeryLongLines, SendMethod.File));
				l.Add(new Tuple<StressFile, SendMethod>(StressFile.LongLine,              SendMethod.TextLine));
				l.Add(new Tuple<StressFile, SendMethod>(StressFile.VeryLongLine,          SendMethod.TextLine));
				l.Add(new Tuple<StressFile, SendMethod>(StressFile.EnormousLine,          SendMethod.TextLine));
				return (l);

				// Could be extended by 'SendMethod.Text'/'TextLines' and a 'LineCount'.
				// Only to be done when needed, as test would take significantly longer.
			}
		}

		/// <summary></summary>
		public static IEnumerable<Tuple<StressFile, SendMethod>> BinaryFilePairs
		{
			get
			{
				var l = new List<Tuple<StressFile, SendMethod>>(6); // Preset the required capacity to improve memory management.
				l.Add(new Tuple<StressFile, SendMethod>(StressFile.Normal,       SendMethod.File));
				l.Add(new Tuple<StressFile, SendMethod>(StressFile.Large,        SendMethod.File)); // 'Large' is needed for serial COM ports with defaults, 'Huge' would take too long.
				l.Add(new Tuple<StressFile, SendMethod>(StressFile.Huge,         SendMethod.File)); // 'Enormous' would take too long.
				l.Add(new Tuple<StressFile, SendMethod>(StressFile.LongLine,     SendMethod.TextLine));
				l.Add(new Tuple<StressFile, SendMethod>(StressFile.VeryLongLine, SendMethod.TextLine));
				l.Add(new Tuple<StressFile, SendMethod>(StressFile.EnormousLine, SendMethod.TextLine));
				return (l);

				// Could be extended by 'SendMethod.Text'/'TextLines' and a 'LineCount'.
				// Only to be done when needed, as test would take significantly longer.
			}
		}

		#endregion

		#region Test Settings
		//==========================================================================================
		// Test Settings
		//==========================================================================================

		private static void ModifySerialPortSettings(TestCaseData tcd)
		{
			// Generated arguments must either be...
			//    settings, fileInfo, sendMethod, timeout
			// ...or...
			//    settingsA, settingsB, fileInfo, sendMethod, timeout

			var settingsA = (tcd.Arguments[0] as TerminalSettings);
			var settingsB = (tcd.Arguments[1] as TerminalSettings);

			// Sending terminal (always A):

			settingsA.IO.SerialPort.BufferMaxBaudRate = false;
			settingsA.IO.SerialPort.MaxChunkSize = new ChunkSize(false, SerialPortSettings.MaxChunkSizeDefault.Size);

			// Both terminals:

			int baudRate = (MKY.IO.Ports.BaudRateEx)MKY.IO.Ports.BaudRate.Baud115200;
			                         settingsA.IO.SerialPort.Communication.BaudRate = baudRate;
			if (settingsB != null) { settingsB.IO.SerialPort.Communication.BaudRate = baudRate; }

			                         settingsA.IO.SerialPort.Communication.FlowControl = SerialFlowControl.Software;   // Required to prevent Rx overruns. Note that hardware
			if (settingsB != null) { settingsB.IO.SerialPort.Communication.FlowControl = SerialFlowControl.Software; } // flow control, cannot be used, MCT doesn't support it!

			                         settingsA.IO.SerialPort.SignalXOnWhenOpened = false;   // Required to match file size. Could alternatively
			if (settingsB != null) { settingsB.IO.SerialPort.SignalXOnWhenOpened = false; } // be handled by adding one byte to the expected.
		}

		#endregion

		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <remarks>
		/// Test cases are generated by code rather than using <see cref="ValuesAttribute"/> for
		/// being able to name the test cases in a human readable way.
		/// </remarks>
		/// <remarks>
		/// For proper ordering (e.g. defaults first, modified after), all combinations are
		/// generated here and then some are removed by the I/O dependent generation further below.
		/// again.
		/// </remarks>
		private static IEnumerable<Tuple<TerminalType, SettingsFlavor, TestCaseData>> Tests
		{
			get
			{
				foreach (var tt in (TerminalType[])(Enum.GetValues(typeof(TerminalType))))
				{
					foreach (var flavor in (SettingsFlavor[])(Enum.GetValues(typeof(SettingsFlavor))))
					{
						IEnumerable<Tuple<StressFile, SendMethod>> filePairs;

						switch (tt)
						{
							case TerminalType.Text:   filePairs = TextFilePairs;   break;
							case TerminalType.Binary: filePairs = BinaryFilePairs; break;

							default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + tt + "' is a terminal type that is not (yet) supported!" + System.Environment.NewLine + System.Environment.NewLine + MessageHelper.SubmitBug));
						}

						foreach (var fp in filePairs)
						{
							var fileKey = fp.Item1;
							var sendMethod = fp.Item2;
							var fileInfo = GetFileInfo(tt, fileKey, sendMethod);

							var nameSuffix = string.Format(CultureInfo.CurrentCulture, "_{0}_{1}_{2}", tt, flavor, fileKey);
							var tcd = new TestCaseData(fileInfo, sendMethod).SetName(nameSuffix);
							yield return (new Tuple<TerminalType, SettingsFlavor, TestCaseData>(tt, flavor, tcd));
						}

						// Generated arguments must either be...
						//    settings, fileInfo, sendMethod, timeout
						// ...or...
						//    settingsA, settingsB, fileInfo, sendMethod, timeout
						// ...of which the middle two arguments are generated above.
					}
				}
			}
		}

		private static FileInfo GetFileInfo(TerminalType terminalType, StressFile fileKey, SendMethod sendMethod)
		{
			switch (sendMethod)
			{
			////case SendMethod.RawData:
			////{
			////	return (Files.SendRaw.Item[fileKey];
			////}

				case SendMethod.Text:
				case SendMethod.TextLine:
				case SendMethod.TextLines:
				{
					switch (terminalType)
					{
						case TerminalType.Text:   return (Files.TextSendText  .Item[fileKey]);
						case TerminalType.Binary: return (Files.BinarySendText.Item[fileKey]);

						default: throw (new ArgumentOutOfRangeException("terminalType", terminalType, MessageHelper.InvalidExecutionPreamble + "'" + terminalType + "' is a terminal type that is not (yet) supported!" + System.Environment.NewLine + System.Environment.NewLine + MessageHelper.SubmitBug));
					}
				}

				case SendMethod.File:
				{
					switch (terminalType)
					{
						case TerminalType.Text:   return (Files.TextSendFile  .Item[fileKey]);
						case TerminalType.Binary: return (Files.BinarySendFile.Item[fileKey]);

						default: throw (new ArgumentOutOfRangeException("terminalType", terminalType, MessageHelper.InvalidExecutionPreamble + "'" + terminalType + "' is a terminal type that is not (yet) supported!" + System.Environment.NewLine + System.Environment.NewLine + MessageHelper.SubmitBug));
					}
				}

				default: throw (new ArgumentOutOfRangeException("sendMethod", sendMethod, MessageHelper.InvalidExecutionPreamble + "'" + sendMethod + "' is a send method that is not (yet) supported!" + System.Environment.NewLine + System.Environment.NewLine + MessageHelper.SubmitBug));
			}
		}


		/// <remarks>Separation into multiple tests for grouping 'by I/O' to ease test development and manual execution.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize variational manner of this item.")]
		public static IEnumerable TestCasesSerialPortLoopbackPairs
		{
			get
			{
				foreach (var t in Tests)
				{
					foreach (var tc in Data.ToSerialPortLoopbackPairsTestCases(t.Item1, new TestCaseData[] { t.Item3 }))
						yield return (ComplementTestCase(t, tc));
				}
			}
		}

		/// <remarks>Separation into multiple tests for grouping 'by I/O' to ease test development and manual execution.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize variational manner of this item.")]
		public static IEnumerable TestCasesSerialPortLoopbackSelfs
		{
			get
			{
				foreach (var t in Tests)
				{
					foreach (var tc in Data.ToSerialPortLoopbackSelfsTestCases(t.Item1, new TestCaseData[] { t.Item3 }))
						yield return (ComplementTestCase(t, tc));
				}
			}
		}

		/// <remarks>Separation into multiple tests for grouping 'by I/O' to ease test development and manual execution.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize variational manner of this item.")]
		public static IEnumerable TestCasesIPSocketPairs
		{
			get
			{
				foreach (var t in Tests)
				{
					if (t.Item2 != SettingsFlavor.Default)
						continue; // No need to generate non-default settings (yet).

					foreach (var tc in Data.ToIPSocketPairsTestCases(t.Item1, new TestCaseData[] { t.Item3 }))
						yield return (ComplementTestCase(t, tc));
				}
			}
		}

		/// <remarks>Separation into multiple tests for grouping 'by I/O' to ease test development and manual execution.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize variational manner of this item.")]
		public static IEnumerable TestCasesIPSocketSelfs
		{
			get
			{
				foreach (var t in Tests)
				{
					if (t.Item2 != SettingsFlavor.Default)
						continue; // No need to generate non-default settings (yet).

					foreach (var tc in Data.ToIPSocketSelfsTestCases(t.Item1, new TestCaseData[] { t.Item3 }))
						yield return (ComplementTestCase(t, tc));
				}
			}
		}

		private static TestCaseData ComplementTestCase(Tuple<TerminalType, SettingsFlavor, TestCaseData> t, TestCaseData tc)
		{
			// Generated arguments must either be...
			//    settings, fileInfo, sendMethod, timeout
			// ...or...
			//    settingsA, settingsB, fileInfo, sendMethod, timeout

			var settingsA = (tc.Arguments[0] as TerminalSettings);
			var settingsB = (tc.Arguments[1] as TerminalSettings);

			if (settingsA.IO.IOType == IOType.SerialPort) // Ignore settingsB (yet).
			{
				if (t.Item2 == SettingsFlavor.Modified)
					ModifySerialPortSettings(tc);
			}

			return (DecorateWithTimeoutAndDuration(tc));
		}

		/// <remarks>Returning new data is required to modify arguments.</remarks>
		private static TestCaseData DecorateWithTimeoutAndDuration(TestCaseData tc)
		{
		#if (DEBUG_TEST_CASE_DATA)
			TestCaseDataHelper.WriteToTempFile(typeof(SendTestData), tc);
		#endif

			// Generated arguments must either be...
			//    settings, fileInfo, sendMethod, timeout
			// ...or...
			//    settingsA, settingsB, fileInfo, sendMethod, timeout

			var settingsA = (tc.Arguments[0] as TerminalSettings);
			var settingsB = (tc.Arguments[1] as TerminalSettings);

			FileInfo fileInfo;
			if (settingsB == null) { fileInfo = (FileInfo)(tc.Arguments[1]); }
			else                   { fileInfo = (FileInfo)(tc.Arguments[2]); }

			var fileByteCount = fileInfo.ByteCount;                             // settingsB are ignored (yet).
			var estimatedTransmissionTime = Utilities.GetEstimatedTransmissionTime(settingsA, fileByteCount);

			// Workaround to two issues:
			//  > YAT: Becomes really slow in case of such long lines. With FR #375 (see further below) this can be fixed for this test.
			//  > NUnit: Memory consumption increases such RAM becomes full and execution takes forever, maybe due to reason above.
			string workaround = "";
			if (fileInfo.Path.Contains("HugeWith") ||
			    fileInfo.Path.Contains("EnormousLine"))
			{
				estimatedTransmissionTime *= 999; // Will result in a "high" enough duration category to easily let it exclude.
				workaround = " WORKAROUND FR #375";
			}

			// Timeout:
			var args = new List<object>(tc.Arguments.Length + 1);
			args.AddRange(tc.Arguments);                            // 50% margin to account for temporary congestion and the amount of tests (RAM consumption).
			var timeout = Math.Max((int)(estimatedTransmissionTime * 1.5), Utilities.WaitTimeoutForLineTransmission); // 'timeout' must always be at least
			args.Add(timeout);                                                                                        // the standard line timeout.
			var timeoutCaption = StandardDurationCategory.CaptionFrom(TimeSpan.FromMilliseconds(timeout));

			// Estimated time and duration category:                    // settingsB are ignored (yet).
			var estimatedOverheadTime = Utilities.GetEstimatedOverheadTime(settingsA, fileByteCount);
			var estimated = TimeSpan.FromMilliseconds(estimatedTransmissionTime + estimatedOverheadTime);
			var estimatedCaption = StandardDurationCategory.CaptionFrom(estimated);
			var cat = StandardDurationCategory.AttributeFrom(estimated).Name;

			var nameSuffix = " (" + estimatedCaption + " estimated total; " + timeoutCaption +" Tx/Rx timeout)" + workaround;
			var result = TestCaseDataEx.ToTestCase(tc, nameSuffix, new string[] { cat }, args.ToArray());
		#if (DEBUG_TEST_CASE_DATA)
			TestCaseDataHelper.WriteToTempFile(typeof(SendTestData), result); // No need to append to file, file name will differ due to suffix.
		#endif
			return (result);
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class SendTest
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const string Eol = "<CR><LF>"; // Fixed to default.
		private const int EolByteLength = 2;

	////private const int MaxLineLengthForTest = 10000;

		#endregion

		#region TestFixture
		//==========================================================================================
		// TestFixture
		//==========================================================================================

		#if (DEBUG_TEST_CASE_DATA)

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "SetUp", Justification = "Naming according to NUnit.")]
		[TestFixtureSetUp]
		public virtual void TestFixtureSetUp()
		{
			var path = Temp.MakeTempPath(GetType(), outputPathToDebug: true);

			Exception ex;
			if (!DirectoryEx.TryBrowse(path, out ex))
				DebugEx.WriteException(this.GetType(), ex, "Exception when trying the browse folder with temporary debug log files!");
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TearDown", Justification = "Naming according to NUnit.")]
		[TestFixtureTearDown]
		public virtual void TestFixtureTearDown()
		{
			Temp.CleanTempPath(GetType(), outputPathToDebug: true);
		}

		#endif // DEBUG_TEST_CASE_DATA

		#endregion

		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		/// <remarks>Separation into multiple tests for grouping 'by I/O' to ease test development and manual execution.</remarks>
		[Test, TestCaseSource(typeof(SendTestData), "TestCasesSerialPortLoopbackPairs"), Combinatorial] // Test is mandatory, it shall not be excludable.
		public virtual void TestSerialPortLoopbackPairs(TerminalSettings settingsA, TerminalSettings settingsB, FileInfo fileInfo, SendMethod sendMethod, int timeout)
		{
			if (!MKY.IO.Ports.Test.ConfigurationProvider.Configuration.LoopbackPairsAreAvailable)
				Assert.Ignore("No serial COM port loopback pairs are available, therefore this test is excluded. Ensure that at least one serial COM port loopback pair is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			SendAndVerify(settingsA, settingsB, fileInfo, sendMethod, timeout);
		}

		/// <remarks>Separation into multiple tests for grouping 'by I/O' to ease test development and manual execution.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		[Test, TestCaseSource(typeof(SendTestData), "TestCasesSerialPortLoopbackSelfs")]
		public virtual void TestSerialPortLoopbackSelfs(TerminalSettings settings, FileInfo fileInfo, SendMethod sendMethod, int timeout)
		{
			if (!MKY.IO.Ports.Test.ConfigurationProvider.Configuration.LoopbackSelfsAreAvailable)
				Assert.Ignore("No serial COM port loopback selfs are available, therefore this test is excluded. Ensure that at least one serial COM port loopback self is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			SendAndVerify(settings, null, fileInfo, sendMethod, timeout);
		}

		/// <remarks>Separation into multiple tests for grouping 'by I/O' to ease test development and manual execution.</remarks>
		[Test, TestCaseSource(typeof(SendTestData), "TestCasesIPSocketPairs")]
		public virtual void TestIPSocketPairs(TerminalSettings settingsA, TerminalSettings settingsB, FileInfo fileInfo, SendMethod sendMethod, int timeout)
		{
			// IPSocketPairs are always made available by 'Utilities', no need to check for this.

			SendAndVerify(settingsA, settingsB, fileInfo, sendMethod, timeout);
		}

		/// <remarks>Separation into multiple tests for grouping 'by I/O' to ease test development and manual execution.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		[Test, TestCaseSource(typeof(SendTestData), "TestCasesIPSocketSelfs")]
		public virtual void TestIPSocketSelfs(TerminalSettings settings, FileInfo fileInfo, SendMethod sendMethod, int timeout)
		{
			// IPSocketSelfs are always made available by 'Utilities', no need to check for this.

			SendAndVerify(settings, null, fileInfo, sendMethod, timeout);
		}

		private static void SendAndVerify(TerminalSettings settingsA, TerminalSettings settingsB, FileInfo fileInfo, SendMethod sendMethod, int timeout)
		{
			// Configure binary terminal behavior expected by this test case::
			Settings.ConfigureSettingsIfBinary(settingsA, fileInfo.LineByteCount);
			Settings.ConfigureSettingsIfBinary(settingsB, fileInfo.LineByteCount);

			// Revert UDP/IP to default behavior expected by this test case:
			Settings.RevertSettingsIfUdpSocket(settingsA);
			Settings.RevertSettingsIfUdpSocket(settingsB);

			// Adjust maximum line length:
			if (fileInfo.LineByteCount > settingsA.Display.MaxLineLength) // Potentially differing 'settingsB' will result in assertion on verification.
			{
			////if (fileInfo.LineByteCount <= MaxLineLengthForTest) // Only adjust up to 10'000 in order to also test with 'Exceeded' warning message:
				{
					                         settingsA.Display.MaxLineLength = fileInfo.LineByteCount;
					if (settingsB != null) { settingsB.Display.MaxLineLength = fileInfo.LineByteCount; }
				}
			////else // Requires FR #375 "...migrate Byte/Line Count/Rate from model to domain..." because verification is yet fixed to repositories.
			////{
			////	                         settingsA.Display.MaxLineLength = MaxLineLengthForTest;
			////	if (settingsB != null) { settingsB.Display.MaxLineLength = MaxLineLengthForTest; }
			////}
			}

			// Adjust maximum number of lines:
			if (fileInfo.LineCount > settingsA.Display.MaxLineCount) // Potentially differing 'settingsB' will result in assertion on verification.
			{
				                         settingsA.Display.MaxLineCount = fileInfo.LineCount;
				if (settingsB != null) { settingsB.Display.MaxLineCount = fileInfo.LineCount; }
			}

			// Ready to send and verify:
			if (settingsB != null)
				SendAndVerifyPair(settingsA, settingsB, fileInfo, sendMethod, timeout);
			else
				SendAndVerifySelf(settingsA,            fileInfo, sendMethod, timeout);
		}

		private static void SendAndVerifyPair(TerminalSettings settingsTx, TerminalSettings settingsRx, FileInfo fileInfo, SendMethod sendMethod, int timeout)
		{
			using (var terminalTx = TerminalFactory.CreateTerminal(settingsTx))
			{
				using (var terminalRx = TerminalFactory.CreateTerminal(settingsRx))
				{
					Assert.That(terminalTx.Start(), Is.True, "Terminal Tx could not be started!");
					Assert.That(terminalRx.Start(), Is.True, "Terminal Rx could not be started!");
					Utilities.WaitForConnection(terminalTx, terminalRx);

					SendAndVerify(terminalTx, terminalRx, fileInfo, sendMethod, timeout);

					terminalRx.Stop();
					Utilities.WaitForDisconnection(terminalRx);
				}

				terminalTx.Stop();
				Utilities.WaitForDisconnection(terminalTx);
			}
		}

		private static void SendAndVerifySelf(TerminalSettings settingsTxRx, FileInfo fileInfo, SendMethod sendMethod, int timeout)
		{
			using (var terminalTxRx = TerminalFactory.CreateTerminal(settingsTxRx))
			{
				Assert.That(terminalTxRx.Start(), Is.True, "Terminal Tx/Rx could not be started!");
				Utilities.WaitForConnection(terminalTxRx, terminalTxRx);

				SendAndVerify(terminalTxRx, terminalTxRx, fileInfo, sendMethod, timeout);

				terminalTxRx.Stop();
				Utilities.WaitForDisconnection(terminalTxRx);
			}
		}

		private static void SendAndVerify(Domain.Terminal terminalTx, Domain.Terminal terminalRx, FileInfo fileInfo, SendMethod sendMethod, int timeout)
		{
			var terminalType = terminalTx.TerminalSettings.TerminalType;

			// Read file content:
			byte[] fileContentAsBytes = null;
			string fileContentAsText = null;
			string[] fileContentAsLines = null;
			ReadFileContent(terminalType, fileInfo, sendMethod, out fileContentAsBytes, out fileContentAsText, out fileContentAsLines);

			// Send and verify counts:
			Send(terminalTx, fileInfo, sendMethod, fileContentAsBytes, fileContentAsText, fileContentAsLines);
			Utilities.WaitForTransmissionAndAssertCounts(terminalTx, terminalRx, fileInfo.ByteCount, fileInfo.LineCount, timeout);

			// Verify content:
			string[] expectedContentPattern;
			GetExpectedContent(terminalTx, fileInfo, sendMethod, fileContentAsBytes, fileContentAsText, fileContentAsLines, out expectedContentPattern);
			Utilities.AssertTxContentPattern(terminalTx, expectedContentPattern);
			Utilities.AssertRxContentPattern(terminalRx, expectedContentPattern);

			// Wait to ensure that no operation is ongoing anymore and verify again:
			Utilities.WaitForReverification();

			Utilities.AssertCounts(terminalTx, terminalRx, fileInfo.ByteCount, fileInfo.LineCount);

			Utilities.AssertTxContentPattern(terminalTx, expectedContentPattern);
			Utilities.AssertRxContentPattern(terminalRx, expectedContentPattern);

			// Refresh and verify again:
			terminalTx.RefreshRepositories();
			terminalRx.RefreshRepositories();

			Utilities.AssertCounts(terminalTx, terminalRx, fileInfo.ByteCount, fileInfo.LineCount);

			Utilities.AssertTxContentPattern(terminalTx, expectedContentPattern);
			Utilities.AssertRxContentPattern(terminalRx, expectedContentPattern);
		}

		private static void ReadFileContent(TerminalType terminalType, FileInfo fileInfo, SendMethod sendMethod, out byte[] fileContentAsBytes, out string fileContentAsText, out string[] fileContentAsLines)
		{
			fileContentAsBytes = ((fileInfo.Type == FileType.Binary) ? File.ReadAllBytes(fileInfo.Path) : null);
			fileContentAsText  = ((fileInfo.Type == FileType.Text)   ? File.ReadAllText( fileInfo.Path) : null);
			fileContentAsLines = ((fileInfo.Type == FileType.Text)   ? File.ReadAllLines(fileInfo.Path) : null);
		}

		private static void Send(Domain.Terminal terminalTx, FileInfo fileInfo, SendMethod sendMethod, byte[] fileContentAsBytes, string fileContentAsText, string[] fileContentAsLines)
		{
			switch (sendMethod)
			{
			////case SendMethod.Raw:       terminalTx.SendRaw(      fileContentAsBytes); return;
				case SendMethod.Text:      terminalTx.SendText(     fileContentAsText ); return;
				case SendMethod.TextLine:  terminalTx.SendTextLine( fileContentAsText ); return;
				case SendMethod.TextLines: terminalTx.SendTextLines(fileContentAsLines); return;
				case SendMethod.File:      terminalTx.SendFile(     fileInfo.Path);      return;

				default: throw (new ArgumentOutOfRangeException("sendMethod", sendMethod, MessageHelper.InvalidExecutionPreamble + "'" + sendMethod + "' is a send method that is not (yet) supported!" + System.Environment.NewLine + System.Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		private static void GetExpectedContent(Domain.Terminal terminal, FileInfo fileInfo, SendMethod sendMethod, byte[] fileContentAsBytes, string fileContentAsText, string[] fileContentAsLines, out string[] expectedContentPattern)
		{                                                         // Just one terminal is enough, both terminals are configured the same.
			var terminalType = terminal.TerminalSettings.TerminalType;
			if (terminalType == TerminalType.Text)
				GetExpectedTextContent(  terminal,           sendMethod,                     fileContentAsText, fileContentAsLines, out expectedContentPattern);
			else
				GetExpectedBinaryContent(terminal, fileInfo, sendMethod, fileContentAsBytes, fileContentAsText, fileContentAsLines, out expectedContentPattern);
		}

		private static void GetExpectedTextContent(Domain.Terminal terminal, SendMethod sendMethod, string fileContentAsText, string[] fileContentAsLines, out string[] expectedContentPattern)
		{
			switch (sendMethod)
			{
			////case SendMethod.Raw: is not implemented/used by this test (yet).

				case SendMethod.Text:      expectedContentPattern = new string[] { fileContentAsText };                                     break;
				case SendMethod.TextLine:  expectedContentPattern = new string[] { fileContentAsText + Eol };                               break;
				case SendMethod.TextLines:
				case SendMethod.File:      expectedContentPattern =                fileContentAsLines.Select(line => line + Eol).ToArray(); break;

				default: throw (new ArgumentOutOfRangeException("sendMethod", sendMethod, MessageHelper.InvalidExecutionPreamble + "'" + sendMethod + "' is a send method that is not (yet) supported!" + System.Environment.NewLine + System.Environment.NewLine + MessageHelper.SubmitBug));
			}

			// Limit the expected content to the maximum line length in order to also test with 'Exceeded' warning message:
			int maxLineLength = terminal.TerminalSettings.Display.MaxLineLength;
			for (int i = 0; i < expectedContentPattern.Length; i++)
			{
				var expectedContentLength = (expectedContentPattern[i].Length - Eol.Length + EolByteLength); // Adjust value by length "<CR><LF>" and the corresponding byte length.
				if (expectedContentLength > maxLineLength)                                            // Works as long there are neither control nor hidden characters in the file.
					expectedContentPattern[i] = StringEx.Left(expectedContentPattern[i], maxLineLength) + Utilities.LineExceededWarningPattern;
			}
		}

		private static void GetExpectedBinaryContent(Domain.Terminal terminal, FileInfo fileInfo, SendMethod sendMethod, byte[] fileContentAsBytes, string fileContentAsText, string[] fileContentAsLines, out string[] expectedContentPattern)
		{                                                               // Just one terminal is enough, both terminals are configured the same.
			var terminalType = terminal.TerminalSettings.TerminalType;
			Encoding encoding = (EncodingEx)terminal.TerminalSettings.BinaryTerminal.EncodingFixed;

			byte[] firstLineAsBytes;
			int expectedLineCount;
			if (!ArrayEx.IsNullOrEmpty(fileContentAsBytes))
			{
				firstLineAsBytes = new byte[fileInfo.LineByteCount];
				Array.Copy(fileContentAsBytes, firstLineAsBytes, fileInfo.LineByteCount);
				expectedLineCount = fileInfo.LineCount;
			}
			else if (!ArrayEx.IsNullOrEmpty(fileContentAsLines))
			{
				firstLineAsBytes = encoding.GetBytes(fileContentAsLines[0]);
				expectedLineCount = fileContentAsLines.Length;
			}
			else
			{
				throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "Argument combination is not (yet) supported!" + System.Environment.NewLine + System.Environment.NewLine + MessageHelper.SubmitBug));
			}

			string formattedLine;
			if (firstLineAsBytes.Length <= terminal.TerminalSettings.Display.MaxLineLength)
			{
				formattedLine = ByteHelper.FormatHexString(firstLineAsBytes);
			}
			else
			{
				formattedLine = ByteHelper.FormatHexString(firstLineAsBytes.Take(terminal.TerminalSettings.Display.MaxLineLength));
				formattedLine += Utilities.LineExceededWarningPattern;
			}

			// Yet limited to files where each resulting line has the same content.

			switch (sendMethod)
			{
			////case SendMethod.Raw:
				case SendMethod.Text:
				case SendMethod.TextLine:  expectedContentPattern = new string[] { formattedLine };                                        return;

				case SendMethod.TextLines:
				case SendMethod.File:      expectedContentPattern = ArrayEx.CreateAndInitializeInstance(expectedLineCount, formattedLine); return;

				default: throw (new ArgumentOutOfRangeException("sendMethod", sendMethod, MessageHelper.InvalidExecutionPreamble + "'" + sendMethod + "' is a send method that is not (yet) supported!" + System.Environment.NewLine + System.Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
