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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;

using MKY;
using MKY.IO.Serial.SerialPort;

using NUnit.Framework;

using YAT.Domain.Settings;

#endregion

namespace YAT.Domain.Test.Terminal
{
	#region Enum SendMethod

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum SendMethod
	{
		Raw,
		Text,
		TextLine,
		TextLines,
		File
	}

	#pragma warning restore 1591

	#endregion

	/// <summary></summary>
	public static class SendFileTestTestData
	{
		#region Test Values
		//==========================================================================================
		// Test Values
		//==========================================================================================

		/// <summary></summary>
		public static IEnumerable<Tuple<bool, string>> SettingsFlavors
		{
			get
			{
				var l = new List<Tuple<bool, string>>(2); // Preset the required capacity to improve memory management.
				l.Add(new Tuple<bool, string>(false, "Default"));
				l.Add(new Tuple<bool, string>(true, "Modified"));
				return (l);
			}
		}

		/// <summary></summary>
		public static IEnumerable<Tuple<StressFile, SendMethod>> TextFileItems
		{
			get
			{
				var l = new List<Tuple<StressFile, SendMethod>>(6); // Preset the required capacity to improve memory management.
				l.Add(new Tuple<StressFile, SendMethod>(StressFile.Normal,                SendMethod.File)); // 'Large*' does not increase test coverage.
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
		public static IEnumerable<Tuple<StressFile, SendMethod>> BinaryFileItems
		{
			get
			{
				var l = new List<Tuple<StressFile, SendMethod>>(5); // Preset the required capacity to improve memory management.
				l.Add(new Tuple<StressFile, SendMethod>(StressFile.Normal,            SendMethod.File)); // 'Large*' does not increase test coverage.
				l.Add(new Tuple<StressFile, SendMethod>(StressFile.Huge,              SendMethod.File)); // 'Enormous' would take too long.
				l.Add(new Tuple<StressFile, SendMethod>(StressFile.LongLine,          SendMethod.TextLine));
				l.Add(new Tuple<StressFile, SendMethod>(StressFile.VeryLongLine,      SendMethod.TextLine));
				l.Add(new Tuple<StressFile, SendMethod>(StressFile.EnormousLine,      SendMethod.TextLine));
				return (l);

				// Could be extended by 'SendMethod.Text'/'TextLines' and a 'LineCount'.
				// Only to be done when needed, as test would take significantly longer.
			}
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
		private static IEnumerable<TestCaseData> Tests
		{
			get
			{
				foreach (var tt in (TerminalType[])(Enum.GetValues(typeof(TerminalType))))
				{
					foreach (var flavor in SettingsFlavors)
					{
						IEnumerable<Tuple<StressFile, SendMethod>> fileItems;

						switch (tt)
						{
							case TerminalType.Text:   fileItems = TextFileItems;   break;
							case TerminalType.Binary: fileItems = BinaryFileItems; break;

							default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + tt + "' is a terminal type that is not (yet) supported!" + System.Environment.NewLine + System.Environment.NewLine + MessageHelper.SubmitBug));
						}

						foreach (var fi in fileItems)
						{
							var nameSuffix = string.Format(CultureInfo.CurrentCulture, "_{0}_{1}_{2}", tt, flavor.Item2, fi.Item1);
							yield return (new TestCaseData(tt, flavor.Item1, fi.Item1, fi.Item2).SetName(nameSuffix));
						}
					}
				}
			}
		}

		/// <remarks>Separation into multiple tests for grouping 'by I/O' to ease test development and manual execution.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize variational manner of this item.")]
		public static IEnumerable TestCasesSerialPortLoopbackPairs_Text
		{
			get
			{
				foreach (var tc in Data.ToSerialPortLoopbackPairsTestCases_Text(Tests))
					yield return (tc);
			}
		}

		/// <remarks>Separation into multiple tests for grouping 'by I/O' to ease test development and manual execution.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize variational manner of this item.")]
		public static IEnumerable TestCasesSerialPortLoopbackSelfs_Text
		{
			get
			{
				foreach (var tc in Data.ToSerialPortLoopbackSelfsTestCases_Text(Tests))
					yield return (tc);
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
					var modifySettings = (bool)(t.Arguments[3]);
					if (modifySettings)
						continue; // No need to generate these settings for IP sockets (yet).

					foreach (var tc in Data.ToIPSocketPairsTestCases_Text(new TestCaseData[] { t }))
						yield return (tc);
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
					var modifySettings = (bool)(t.Arguments[3]);
					if (modifySettings)
						continue; // No need to generate these settings for IP sockets (yet).

					foreach (var tc in Data.ToIPSocketSelfsTestCases_Text(new TestCaseData[] { t }))
						yield return (tc);
				}
			}
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class SendTest
	{
		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		/// <remarks>Separation into multiple tests for grouping 'by I/O' to ease test development and manual execution.</remarks>
		[Test, TestCaseSource(typeof(SendFileTestTestData), "TestCasesSerialPortLoopbackPairs_Text"), Combinatorial] // Test is mandatory, it shall not be excludable.
		public virtual void TestSerialPortLoopbackPairs(TerminalSettings settingsA, TerminalSettings settingsB, TerminalType terminalType, bool modifySettings, StressFile fileItem, SendMethod sendMethod)
		{
			if (!MKY.IO.Ports.Test.ConfigurationProvider.Configuration.LoopbackPairsAreAvailable)
				Assert.Ignore("No serial COM port loopback pairs are available, therefore this test is excluded. Ensure that at least one serial COM port loopback pair is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			SendAndVerify(settingsA, settingsB, terminalType, modifySettings, fileItem, sendMethod);
		}

		/// <remarks>Separation into multiple tests for grouping 'by I/O' to ease test development and manual execution.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		[Test, TestCaseSource(typeof(SendFileTestTestData), "TestCasesSerialPortLoopbackSelfs_Text")]
		public virtual void TestSerialPortLoopbackSelfs(TerminalSettings settings, TerminalType terminalType, bool modifySettings, StressFile fileItem, SendMethod sendMethod)
		{
			if (!MKY.IO.Ports.Test.ConfigurationProvider.Configuration.LoopbackSelfsAreAvailable)
				Assert.Ignore("No serial COM port loopback selfs are available, therefore this test is excluded. Ensure that at least one serial COM port loopback self is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			SendAndVerify(settings, null, terminalType, modifySettings, fileItem, sendMethod);
		}

		/// <remarks>Separation into multiple tests for grouping 'by I/O' to ease test development and manual execution.</remarks>
		[Test, TestCaseSource(typeof(SendFileTestTestData), "TestCasesIPSocketPairs_Text")]
		public virtual void TestIPSocketPairs(TerminalSettings settingsA, TerminalSettings settingsB, TerminalType terminalType, bool modifySettings, StressFile fileItem, SendMethod sendMethod)
		{
			// IPSocketPairs are always made available by 'Utilities', no need to check for this.

			SendAndVerify(settingsA, settingsB, terminalType, modifySettings, fileItem, sendMethod);
		}

		/// <remarks>Separation into multiple tests for grouping 'by I/O' to ease test development and manual execution.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		[Test, TestCaseSource(typeof(SendFileTestTestData), "TestCasesIPSocketSelfs_Text")]
		public virtual void TestIPSocketSelfs(TerminalSettings settings, TerminalType terminalType, bool modifySettings, StressFile fileItem, SendMethod sendMethod)
		{
			// IPSocketSelfs are always made available by 'Utilities', no need to check for this.

			SendAndVerify(settings, null, terminalType, modifySettings, fileItem, sendMethod);
		}

		private static void SendAndVerify(TerminalSettings settingsA, TerminalSettings settingsB, TerminalType terminalType, bool modifySettings, StressFile fileItem, SendMethod sendMethod)
		{
			// Revert to default behavior expected by this test case:
			Settings.RevertSettingsIfUdpSocket(settingsA);
			Settings.RevertSettingsIfUdpSocket(settingsB);

			// Get the file to be used for this test case:
			FileInfo fi;
			switch (sendMethod)
			{
			////case SendMethod.RawData:
			////{
			////	fi = Files.SendRaw.Item[fileItem];
			////	break;
			////}

				case SendMethod.Text:
				case SendMethod.TextLine:
				case SendMethod.TextLines:
				{
					fi = Files.SendText.Item[fileItem];
					break;
				}

				case SendMethod.File:
				{
					switch (terminalType)
					{
						case TerminalType.Text:   fi = Files.TextSendFile  .Item[fileItem]; break;
						case TerminalType.Binary: fi = Files.BinarySendFile.Item[fileItem]; break;

						default: throw (new ArgumentOutOfRangeException("terminalType", terminalType, MessageHelper.InvalidExecutionPreamble + "'" + terminalType + "' is a terminal type that is not (yet) supported!" + System.Environment.NewLine + System.Environment.NewLine + MessageHelper.SubmitBug));
					}
					break;
				}

				default: throw (new ArgumentOutOfRangeException("sendMethod", sendMethod, MessageHelper.InvalidExecutionPreamble + "'" + sendMethod + "' is a send method that is not (yet) supported!" + System.Environment.NewLine + System.Environment.NewLine + MessageHelper.SubmitBug));
			}

			// Adjust maximum line length:
			if (fi.LineByteCount > DisplaySettings.MaxLineLengthDefault)
			{
				                         settingsA.Display.MaxLineLength = fi.LineByteCount;
				if (settingsB != null) { settingsB.Display.MaxLineLength = fi.LineByteCount; }
			}

			// Adjust serial port send settings for sending terminal (always A):
			if (modifySettings)
			{
				settingsA.IO.SerialPort.BufferMaxBaudRate = false;
				settingsA.IO.SerialPort.MaxChunkSize = new ChunkSize(false, SerialPortSettings.MaxChunkSizeDefault.Size);

				// Attention, in order to also be able to modify socket or USB settings, test case generation has to be adjusted.
			}

			// Set terminal type:
			                         settingsA.TerminalType = terminalType;
			if (settingsB != null) { settingsB.TerminalType = terminalType; }

			// Ready to send and verify:
			if (settingsB != null)
				SendAndVerifyPair(settingsA, settingsB, fi, sendMethod);
			else
				SendAndVerifySelf(settingsA,            fi, sendMethod);
		}

		private static void SendAndVerifyPair(TerminalSettings settingsTx, TerminalSettings settingsRx, FileInfo fi, SendMethod sendMethod)
		{
			using (var terminalTx = TerminalFactory.CreateTerminal(settingsTx))
			{
				using (var terminalRx = TerminalFactory.CreateTerminal(settingsRx))
				{
					Assert.That(terminalTx.Start(), Is.True, "Terminal Tx could not be started!");
					Assert.That(terminalRx.Start(), Is.True, "Terminal Rx could not be started!");
					Utilities.WaitForConnection(terminalTx, terminalRx);

					SendAndVerify(terminalTx, terminalRx, fi, sendMethod);

					terminalRx.Stop();
					Utilities.WaitForDisconnection(terminalRx);
				}

				terminalTx.Stop();
				Utilities.WaitForDisconnection(terminalTx);
			}
		}

		private static void SendAndVerifySelf(TerminalSettings settingsTxRx, FileInfo fi, SendMethod sendMethod)
		{
			using (var terminalTxRx = TerminalFactory.CreateTerminal(settingsTxRx))
			{
				Assert.That(terminalTxRx.Start(), Is.True, "Terminal Tx/Rx could not be started!");
				Utilities.WaitForConnection(terminalTxRx, terminalTxRx);

				SendAndVerify(terminalTxRx, terminalTxRx, fi, sendMethod);

				terminalTxRx.Stop();
				Utilities.WaitForDisconnection(terminalTxRx);
			}
		}

		private static void SendAndVerify(Domain.Terminal terminalTx, Domain.Terminal terminalRx, FileInfo fi, SendMethod sendMethod)
		{
			// Read file content:
			byte[] fileContentAsBytes = null;
			string fileContentAsText = null;
			string[] fileContentAsLines = null;
			ReadFileContent(terminalTx.TerminalSettings.TerminalType, fi, sendMethod, out fileContentAsBytes, out fileContentAsText, out fileContentAsLines);

			// Send and verify counts:
			Send(terminalTx, fi, sendMethod, fileContentAsBytes, fileContentAsText, fileContentAsLines);
			Utilities.WaitForTransmissionAndVerifyCounts(terminalTx, terminalRx, fi.ByteCount, fi.LineCount, fi.Timeout);

			// Verify content:
			Utilities.VerifyContent(terminalTx, RepositoryType.Tx, contentPattern);
			Utilities.VerifyBidirContent(terminalTx,               contentPattern);
			Utilities.VerifyBidirContent(terminalRx,               contentPattern);
			Utilities.VerifyContent(terminalRx, RepositoryType.Rx, contentPattern);

			// Wait to ensure that no operation is ongoing anymore and verify again:
			Utilities.WaitForReverification();

			Utilities.VerifyCounts(terminalTx, terminalRx, fi.ByteCount, fi.LineCount);

			Utilities.VerifyContent(terminalTx, RepositoryType.Tx, contentPattern);
			Utilities.VerifyBidirContent(terminalTx,               contentPattern);
			Utilities.VerifyBidirContent(terminalRx,               contentPattern);
			Utilities.VerifyContent(terminalRx, RepositoryType.Rx, contentPattern);

			// Refresh and verify again:
			terminalTx.RefreshRepositories();
			terminalRx.RefreshRepositories();

			Utilities.VerifyCounts(terminalTx, terminalRx, fi.ByteCount, fi.LineCount);

			Utilities.VerifyContent(terminalTx, RepositoryType.Tx, contentPattern);
			Utilities.VerifyBidirContent(terminalTx,               contentPattern);
			Utilities.VerifyBidirContent(terminalRx,               contentPattern);
			Utilities.VerifyContent(terminalRx, RepositoryType.Rx, contentPattern);
		}

		private static void ReadFileContent(TerminalType terminalType, FileInfo fi, SendMethod sendMethod, out byte[] fileContentAsBytes, out string fileContentAsText, out string[] fileContentAsLines)
		{
			fileContentAsBytes = null;
			fileContentAsText  = null;
			fileContentAsLines = null;

			switch (sendMethod)
			{
				case SendMethod.Raw:       fileContentAsBytes = File.ReadAllBytes(fi.Path); return;

				case SendMethod.Text:
				case SendMethod.TextLine:  fileContentAsText  = File.ReadAllText( fi.Path); return;

				case SendMethod.TextLines: fileContentAsLines = File.ReadAllLines(fi.Path); return;

				case SendMethod.File:
				{
					if (terminalType == TerminalType.Text) {
						                   fileContentAsLines = File.ReadAllLines(fi.Path); return;
					}
					else {
						                   fileContentAsBytes = File.ReadAllBytes(fi.Path); return;
					}
				}

				default: throw (new ArgumentOutOfRangeException("sendMethod", sendMethod, MessageHelper.InvalidExecutionPreamble + "'" + sendMethod + "' is a send method that is not (yet) supported!" + System.Environment.NewLine + System.Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		private static void Send(Domain.Terminal terminalTx, FileInfo fi, SendMethod sendMethod, byte[] fileContentAsBytes, string fileContentAsText, string[] fileContentAsLines)
		{
			switch (sendMethod)
			{
			////case SendMethod.Raw:       terminalTx.SendRaw(      fileContentAsBytes); return;

				case SendMethod.Text:      terminalTx.SendText(     fileContentAsText);  return;
				case SendMethod.TextLine:  terminalTx.SendTextLine( fileContentAsText);  return;

			////case SendMethod.TextLines: terminalTx.SendTextLines(fileContentAsLines); return;

				case SendMethod.File:      terminalTx.SendFile(     fi.Path);            return;

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
