﻿//==================================================================================================
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

using NUnit;
using NUnit.Framework;

using YAT.Domain.Settings;

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
				l.Add(new Tuple<StressFile, SendMethod>(StressFile.Large,                 SendMethod.File)); // 'Large' is needed for non-socket, 'Huge' would take too long.
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
				l.Add(new Tuple<StressFile, SendMethod>(StressFile.Large,        SendMethod.File)); // 'Large' is needed for non-socket, 'Huge' would take too long.
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
							var tcd = new TestCaseData(tt, fileInfo, sendMethod).SetName(nameSuffix);
							yield return (new Tuple<TerminalType, SettingsFlavor, TestCaseData>(tt, flavor, tcd));
						}

						// Generated arguments must either be...
						//    settings, fileInfo, sendMethod, timeout
						// ...or...
						//    settingsA, settingsB, fileInfo, sendMethod, timeout
						// ...of which the middle three arguments are generated above.
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
					return (Files.SendText.Item[fileKey]);
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
		public static IEnumerable TestCasesSerialPortLoopbackPairs_Text
		{
			get
			{
				foreach (var t in Tests)
				{
					switch (t.Item2)
					{
						case SettingsFlavor.Default:
						{
							foreach (var tc in Data.ToSerialPortLoopbackPairsTestCases(t.Item1, new TestCaseData[] { t.Item3 }))
							{
								yield return (DecorateWithTimeoutAndDurationCategory(tc));
							}

							break;
						}

						case SettingsFlavor.Modified:
						{
							foreach (var tc in Data.ToSerialPortLoopbackPairsTestCases(t.Item1, new TestCaseData[] { t.Item3 }))
							{
								ModifySerialPortSettings(tc);

								yield return (DecorateWithTimeoutAndDurationCategory(tc));
							}

							break;
						}

						default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + t.Item1.ToString() + "' is a flavor that is not (yet) supported!" + System.Environment.NewLine + System.Environment.NewLine + MessageHelper.SubmitBug));
					}
				}
			}
		}

		/// <remarks>Separation into multiple tests for grouping 'by I/O' to ease test development and manual execution.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize variational manner of this item.")]
		public static IEnumerable TestCasesSerialPortLoopbackSelfs_Text
		{
			get
			{
				foreach (var t in Tests)
				{
					switch (t.Item2)
					{
						case SettingsFlavor.Default:
						{
							foreach (var tc in Data.ToSerialPortLoopbackSelfsTestCases(t.Item1, new TestCaseData[] { t.Item3 }))
							{
								yield return (DecorateWithTimeoutAndDurationCategory(tc));
							}

							break;
						}

						case SettingsFlavor.Modified:
						{
							foreach (var tc in Data.ToSerialPortLoopbackSelfsTestCases(t.Item1, new TestCaseData[] { t.Item3 }))
							{
								ModifySerialPortSettings(tc);

								yield return (DecorateWithTimeoutAndDurationCategory(tc));
							}

							break;
						}

						default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + t.Item1.ToString() + "' is a flavor that is not (yet) supported!" + System.Environment.NewLine + System.Environment.NewLine + MessageHelper.SubmitBug));
					}
				}
			}
		}

		/// <remarks>Separation into multiple tests for grouping 'by I/O' to ease test development and manual execution.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize variational manner of this item.")]
		public static IEnumerable TestCasesIPSocketPairs_Text
		{
			get
			{
				foreach (var t in Tests)
				{
					if (t.Item2 != SettingsFlavor.Default)
						continue; // No need to generate non-default settings (yet).

					foreach (var tc in Data.ToIPSocketPairsTestCases(t.Item1, new TestCaseData[] { t.Item3 }))
					{
						yield return (DecorateWithTimeoutAndDurationCategory(tc));
					}
				}
			}
		}

		/// <remarks>Separation into multiple tests for grouping 'by I/O' to ease test development and manual execution.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize variational manner of this item.")]
		public static IEnumerable TestCasesIPSocketSelfs_Text
		{
			get
			{
				foreach (var t in Tests)
				{
					if (t.Item2 != SettingsFlavor.Default)
						continue; // No need to generate non-default settings (yet).

					foreach (var tc in Data.ToIPSocketSelfsTestCases(t.Item1, new TestCaseData[] { t.Item3 }))
					{
						yield return (DecorateWithTimeoutAndDurationCategory(tc));
					}
				}
			}
		}

		/// <remarks>Returning new data is required to modify arguments.</remarks>
		private static TestCaseData DecorateWithTimeoutAndDurationCategory(TestCaseData tc)
		{
			// Generated arguments must either be...
			//    settings, fileInfo, sendMethod, timeout
			// ...or...
			//    settingsA, settingsB, fileInfo, sendMethod, timeout

			var settingsA = (tc.Arguments[0] as TerminalSettings);
			var settingsB = (tc.Arguments[1] as TerminalSettings);

			FileInfo fileInfo;
			if (settingsB == null) {
				fileInfo = (FileInfo)(tc.Arguments[1]);
			}
			else {
				fileInfo = (FileInfo)(tc.Arguments[2]);
			}

			int fileByteCount = fileInfo.ByteCount;

			double estimatedTime;
			if (settingsB == null) {
				estimatedTime = (fileByteCount / settingsA.IO.RoughlyEstimatedMaxBytesPerMillisecond);
			}
			else {
				var estimatedTimeA = (fileByteCount / settingsA.IO.RoughlyEstimatedMaxBytesPerMillisecond);
				var estimatedTimeB = (fileByteCount / settingsB.IO.RoughlyEstimatedMaxBytesPerMillisecond);
				estimatedTime = Math.Max(estimatedTimeA, estimatedTimeB);
			}

			// Append timeout to arguments:
			var args = new List<object>(tc.Arguments.Length + 1);
			args.AddRange(tc.Arguments);
			int timeout = Math.Max((int)(estimatedTime * 2), 1); // 'timeout' must be 1 or above.
			args.Add(timeout);

			// Determine duration category and also estimated time suffix:
			var ts = TimeSpan.FromMilliseconds(estimatedTime);
			var cat = StandardDurationCategory.AttributeFrom(ts).Name;
			var nameSuffix = StandardDurationCategory.NameSuffixFrom(ts);

			return (TestCaseDataEx.ToTestCase(tc, nameSuffix, new string[] { cat }, args));
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
		private const int MaxLineLengthForTest = 10000;

		#endregion

		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		/// <remarks>Separation into multiple tests for grouping 'by I/O' to ease test development and manual execution.</remarks>
		[Test, TestCaseSource(typeof(SendTestData), "TestCasesSerialPortLoopbackPairs_Text"), Combinatorial] // Test is mandatory, it shall not be excludable.
		public virtual void TestSerialPortLoopbackPairs(TerminalSettings settingsA, TerminalSettings settingsB, FileInfo fileInfo, SendMethod sendMethod, int timeout)
		{
			if (!MKY.IO.Ports.Test.ConfigurationProvider.Configuration.LoopbackPairsAreAvailable)
				Assert.Ignore("No serial COM port loopback pairs are available, therefore this test is excluded. Ensure that at least one serial COM port loopback pair is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			SendAndVerify(settingsA, settingsB, fileInfo, sendMethod, timeout);
		}

		/// <remarks>Separation into multiple tests for grouping 'by I/O' to ease test development and manual execution.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		[Test, TestCaseSource(typeof(SendTestData), "TestCasesSerialPortLoopbackSelfs_Text")]
		public virtual void TestSerialPortLoopbackSelfs(TerminalSettings settings, FileInfo fileInfo, SendMethod sendMethod, int timeout)
		{
			if (!MKY.IO.Ports.Test.ConfigurationProvider.Configuration.LoopbackSelfsAreAvailable)
				Assert.Ignore("No serial COM port loopback selfs are available, therefore this test is excluded. Ensure that at least one serial COM port loopback self is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			SendAndVerify(settings, null, fileInfo, sendMethod, timeout);
		}

		/// <remarks>Separation into multiple tests for grouping 'by I/O' to ease test development and manual execution.</remarks>
		[Test, TestCaseSource(typeof(SendTestData), "TestCasesIPSocketPairs_Text")]
		public virtual void TestIPSocketPairs(TerminalSettings settingsA, TerminalSettings settingsB, FileInfo fileInfo, SendMethod sendMethod, int timeout)
		{
			// IPSocketPairs are always made available by 'Utilities', no need to check for this.

			SendAndVerify(settingsA, settingsB, fileInfo, sendMethod, timeout);
		}

		/// <remarks>Separation into multiple tests for grouping 'by I/O' to ease test development and manual execution.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		[Test, TestCaseSource(typeof(SendTestData), "TestCasesIPSocketSelfs_Text")]
		public virtual void TestIPSocketSelfs(TerminalSettings settings, FileInfo fileInfo, SendMethod sendMethod, int timeout)
		{
			// IPSocketSelfs are always made available by 'Utilities', no need to check for this.

			SendAndVerify(settings, null, fileInfo, sendMethod, timeout);
		}

		private static void SendAndVerify(TerminalSettings settingsA, TerminalSettings settingsB, FileInfo fileInfo, SendMethod sendMethod, int timeout)
		{
			// Revert to default behavior expected by this test case:
			Settings.RevertSettingsIfUdpSocket(settingsA);
			Settings.RevertSettingsIfUdpSocket(settingsB);

			// Adjust maximum line length, but only up to 10'000 in order to also test with 'Exceeded' warning message:
			if (fileInfo.LineByteCount > DisplaySettings.MaxLineLengthDefault)
			{
				if (fileInfo.LineByteCount <= MaxLineLengthForTest)
				{
					                         settingsA.Display.MaxLineLength = fileInfo.LineByteCount;
					if (settingsB != null) { settingsB.Display.MaxLineLength = fileInfo.LineByteCount; }
				}
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
			string[] expectedContent;
			GetExpectedContent(terminalTx, sendMethod, fileContentAsBytes, fileContentAsText, fileContentAsLines, out expectedContent);
			Utilities.AssertTxContent(terminalTx, expectedContent);
			Utilities.AssertRxContent(terminalRx, expectedContent);

			// Wait to ensure that no operation is ongoing anymore and verify again:
			Utilities.WaitForReverification();

			Utilities.AssertCounts(terminalTx, terminalRx, fileInfo.ByteCount, fileInfo.LineCount);

			Utilities.AssertTxContent(terminalTx, expectedContent);
			Utilities.AssertRxContent(terminalRx, expectedContent);

			// Refresh and verify again:
			terminalTx.RefreshRepositories();
			terminalRx.RefreshRepositories();

			Utilities.AssertCounts(terminalTx, terminalRx, fileInfo.ByteCount, fileInfo.LineCount);

			Utilities.AssertTxContent(terminalTx, expectedContent);
			Utilities.AssertRxContent(terminalRx, expectedContent);
		}

		private static void ReadFileContent(TerminalType terminalType, FileInfo fileInfo, SendMethod sendMethod, out byte[] fileContentAsBytes, out string fileContentAsText, out string[] fileContentAsLines)
		{
			var supportsText = false;
			switch (sendMethod)
			{
				case SendMethod.Text:
				case SendMethod.TextLine:
				case SendMethod.TextLines:
				{
					supportsText = true;
					break;
				}

				case SendMethod.File:
				{
					if (terminalType == TerminalType.Text)
						supportsText = true;

					break;
				}

				default:
				{
					break; // Nothing to do.
				}
			}

			fileContentAsBytes =                 File.ReadAllBytes(fileInfo.Path); // Applicable to any file.
			fileContentAsText  = (supportsText ? File.ReadAllText( fileInfo.Path) : null);
			fileContentAsLines = (supportsText ? File.ReadAllLines(fileInfo.Path) : null);
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

		private static void GetExpectedContent(Domain.Terminal terminal, SendMethod sendMethod, byte[] fileContentAsBytes, string fileContentAsText, string[] fileContentAsLines, out string[] expectedContent)
		{                                                         // Just one terminal is enough, both terminals are configured the same.
			var terminalType = terminal.TerminalSettings.TerminalType;
			if (terminalType == TerminalType.Text)
				GetExpectedTextContent(  terminal, sendMethod, fileContentAsBytes, fileContentAsText, fileContentAsLines, out expectedContent);
			else
				GetExpectedBinaryContent(terminal, sendMethod, fileContentAsBytes, fileContentAsText, fileContentAsLines, out expectedContent);
		}

		private static void GetExpectedTextContent(Domain.Terminal terminal, SendMethod sendMethod, byte[] fileContentAsBytes, string fileContentAsText, string[] fileContentAsLines, out string[] expectedContent)
		{
			switch (sendMethod)
			{
			////case SendMethod.Raw:       BytesToTextContent(              fileContentAsBytes, out expectedContent);                is not implemented/used by this test (yet).
				case SendMethod.Text:      expectedContent = new string[] { fileContentAsText };                                     break;
				case SendMethod.TextLine:  expectedContent = new string[] { fileContentAsText + Eol };                               break;
				case SendMethod.TextLines:
				case SendMethod.File:      expectedContent =                fileContentAsLines.Select(line => line + Eol).ToArray(); break;

				default: throw (new ArgumentOutOfRangeException("sendMethod", sendMethod, MessageHelper.InvalidExecutionPreamble + "'" + sendMethod + "' is a send method that is not (yet) supported!" + System.Environment.NewLine + System.Environment.NewLine + MessageHelper.SubmitBug));
			}

			// Limit the expected content to the maximum line length in order to also test with 'Exceeded' warning message:
			int maxLineLength = terminal.TerminalSettings.Display.MaxLineLength;
			for (int i = 0; i < expectedContent.Length; i++)
			{
				if (expectedContent[i].Length > maxLineLength)
					expectedContent[i] = StringEx.Left(expectedContent[i], maxLineLength) + Utilities.LineExceededWarningPattern;
			}
		}

		private static void GetExpectedBinaryContent(Domain.Terminal terminal, SendMethod sendMethod, byte[] fileContentAsBytes, string fileContentAsText, string[] fileContentAsLines, out string[] expectedContent)
		{                                                               // Just one terminal is enough, both terminals are configured the same.
			var terminalType = terminal.TerminalSettings.TerminalType;
			Encoding encoding = (EncodingEx)terminal.TerminalSettings.BinaryTerminal.EncodingFixed;

			string formattedLine;
			int expectedLineCount;
			if (!ArrayEx.IsNullOrEmpty(fileContentAsBytes))
			{
				formattedLine = terminal.Format(fileContentAsBytes, IODirection.Tx); // Direction doesn't matter, both directions are configured the same.
				expectedLineCount = 0;
			}
			else if (!ArrayEx.IsNullOrEmpty(fileContentAsLines))
			{
				byte[] firstLineAsBytes = encoding.GetBytes(fileContentAsLines[0]);
				formattedLine = terminal.Format(firstLineAsBytes, IODirection.Tx); // Direction doesn't matter, both directions are configured the same.
				expectedLineCount = fileContentAsLines.Length;
			}
			else
			{
				throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "Argument combination is not (yet) supported!" + System.Environment.NewLine + System.Environment.NewLine + MessageHelper.SubmitBug));
			}

			switch (sendMethod)
			{
			////case SendMethod.Raw:
				case SendMethod.Text:
				case SendMethod.TextLine:  expectedContent = new string[] { formattedLine };                                        return;

				case SendMethod.TextLines:
				case SendMethod.File:      expectedContent = ArrayEx.CreateAndInitializeInstance(expectedLineCount, formattedLine); return;

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
