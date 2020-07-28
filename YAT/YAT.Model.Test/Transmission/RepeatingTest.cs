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
using System.Threading;

using MKY;
using MKY.Settings;

using NUnit.Framework;

using YAT.Domain;
using YAT.Domain.Settings;
using YAT.Settings.Application;

#endregion

namespace YAT.Model.Test.Transmission
{
	/// <summary></summary>
	public static class RepeatingTestData
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <summary></summary>
		public const string TestString = "Hello World";

		/// <summary></summary>
		public const string TestCommand = TestString + @"\!(LineRepeat)\!(LineDelay)";

		#endregion

		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		private static IEnumerable<TestCaseData> Tests
		{
			get
			{
				yield return (new TestCaseData( 1, false, false).SetName("_Repeat01OneWay"));
				yield return (new TestCaseData( 1, true,  false).SetName("_Repeat01TwoWay"));
				yield return (new TestCaseData( 2, false, false).SetName("_Repeat02OneWay"));
				yield return (new TestCaseData( 2, true,  false).SetName("_Repeat02TwoWay"));
				yield return (new TestCaseData(10, false, false).SetName("_Repeat10OneWay"));
				yield return (new TestCaseData(10, true,  false).SetName("_Repeat10TwoWay"));

				yield return (new TestCaseData(SendSettings.LineRepeatInfinite, false, true ).SetName("_RepeatRandomOneWayAndBreak"));
				yield return (new TestCaseData(SendSettings.LineRepeatInfinite, false, false).SetName("_RepeatRandomOneWayUntilExit"));
				yield return (new TestCaseData(SendSettings.LineRepeatInfinite, true,  true ).SetName("_RepeatRandomTwoWayAndBreak"));
				yield return (new TestCaseData(SendSettings.LineRepeatInfinite, true,  false).SetName("_RepeatRandomTwoWayUntilExit"));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize variational manner of this item.")]
		public static IEnumerable TestCasesSerialPortLoopbackPairs_Text
		{
			get
			{
				foreach (var descriptor in Domain.Test.Environment.SerialPortLoopbackPairs)
				{
					var settingsA = Domain.Test.Settings.GetSerialPortSettings(TerminalType.Text, descriptor.PortA);
					var settingsB = Domain.Test.Settings.GetSerialPortSettings(TerminalType.Text, descriptor.PortB);

					foreach (var t in Tests)
						yield return (Domain.Test.Data.ToTestCase(descriptor, t, settingsA, settingsB, t.Arguments));
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize variational manner of this item.")]
		public static IEnumerable TestCasesSerialPortLoopbackSelfs_Text
		{
			get
			{
				foreach (var descriptor in Domain.Test.Environment.SerialPortLoopbackSelfs)
				{
					var settings = Domain.Test.Settings.GetSerialPortSettings(TerminalType.Text, descriptor.Port);

					foreach (var t in Tests)
						yield return (Domain.Test.Data.ToTestCase(descriptor, t, settings, t.Arguments));
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize variational manner of this item.")]
		public static IEnumerable TestCasesIPLoopbackPairs_Text
		{
			get
			{
				foreach (var descriptor in Domain.Test.Environment.IPLoopbackPairs)
				{
					var settingsA = Domain.Test.Settings.GetIPLoopbackSettings(TerminalType.Text, descriptor.SocketTypeA, descriptor.LocalInterface);
					var settingsB = Domain.Test.Settings.GetIPLoopbackSettings(TerminalType.Text, descriptor.SocketTypeB, descriptor.LocalInterface);

					foreach (var t in Tests)
						yield return (Domain.Test.Data.ToTestCase(descriptor, t, settingsA, settingsB, t.Arguments));
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize variational manner of this item.")]
		public static IEnumerable TestCasesIPLoopbackSelfs_Text
		{
			get
			{
				foreach (var descriptor in Domain.Test.Environment.IPLoopbackSelfs)
				{
					var settings = Domain.Test.Settings.GetIPLoopbackSettings(TerminalType.Text, descriptor.SocketType, descriptor.LocalInterface);

					foreach (var t in Tests)
						yield return (Domain.Test.Data.ToTestCase(descriptor, t, settings, t.Arguments));
				}
			}
		}

		#endregion
	}

	/// <remarks>
	/// It can be argued that this test would be better located in YAT.Domain.Test. It currently is
	/// located here because line counts and rates are calculated in <see cref="Terminal"/>
	/// and required when evaluating the test result.
	/// </remarks>
	[TestFixture]
	public class RepeatingTest
	{
		#region TestFixture
		//==========================================================================================
		// TestFixture
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "SetUp", Justification = "Naming according to NUnit.")]
		[TestFixtureSetUp]
		public virtual void TestFixtureSetUp()
		{
			// Create temporary in-memory application settings for this test run:
			ApplicationSettings.Create(ApplicationSettingsFileAccess.None);

			// Prevent auto-save of workspace settings:
			ApplicationSettings.LocalUserSettings.General.AutoSaveWorkspace = false;
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TearDown", Justification = "Naming according to NUnit.")]
		[TestFixtureTearDown]
		public virtual void TestFixtureTearDown()
		{
			// Close and dispose of temporary in-memory application settings:
			ApplicationSettings.CloseAndDispose();
		}

		#endregion

		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		/// <remarks>Separation into multiple tests for easier handling and execution.</remarks>
		[Test, TestCaseSource(typeof(RepeatingTestData), "TestCasesSerialPortLoopbackPairs_Text")]
		public virtual void SerialPortLoopbackPairs(TerminalSettings settingsA, TerminalSettings settingsB, int repeatCount, bool doTwoWay, bool executeBreak)
		{
			if (!MKY.IO.Ports.Test.ConfigurationProvider.Configuration.LoopbackPairsAreAvailable)
				Assert.Ignore("No serial COM port loopback pairs are available, therefore this test is excluded. Ensure that at least one serial COM port loopback pair is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			TransmitAndVerify(settingsA, settingsB, repeatCount, doTwoWay, executeBreak);
		}

		/// <remarks>Separation into multiple tests for easier handling and execution.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		[Test, TestCaseSource(typeof(RepeatingTestData), "TestCasesSerialPortLoopbackSelfs_Text")]
		public virtual void SerialPortLoopbackSelfs(TerminalSettings settings, int repeatCount, bool doTwoWay, bool executeBreak)
		{
			if (!MKY.IO.Ports.Test.ConfigurationProvider.Configuration.LoopbackSelfsAreAvailable)
				Assert.Ignore("No serial COM port loopback selfs are available, therefore this test is excluded. Ensure that at least one serial COM port loopback self is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			TransmitAndVerify(settings, null, repeatCount, doTwoWay, executeBreak);
		}

		/// <remarks>Separation into multiple tests for easier handling and execution.</remarks>
		[Test, TestCaseSource(typeof(RepeatingTestData), "TestCasesIPLoopbackPairs_Text")]
		public virtual void IPLoopbackPairs(TerminalSettings settingsA, TerminalSettings settingsB, int repeatCount, bool doTwoWay, bool executeBreak)
		{
			// IPLoopbackPairs are always made available by 'Utilities', no need to check for this.

			TransmitAndVerify(settingsA, settingsB, repeatCount, doTwoWay, executeBreak);
		}

		/// <remarks>Separation into multiple tests for easier handling and execution.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		[Test, TestCaseSource(typeof(RepeatingTestData), "TestCasesIPLoopbackSelfs_Text")]
		public static void IPLoopbackSelfs(TerminalSettings settings, int repeatCount, bool doTwoWay, bool executeBreak)
		{
			// IPLoopbackSelfs are always made available by 'Utilities', no need to check for this.

			TransmitAndVerify(settings, null, repeatCount, doTwoWay, executeBreak);
		}

		private static void TransmitAndVerify(TerminalSettings settingsA, TerminalSettings settingsB, int repeatCount, bool doTwoWay, bool executeBreak)
		{
			settingsA.Send.DefaultLineRepeat = repeatCount; // Set settings to the desired repeat count.

			if (settingsA.IO.IOTypeIsUdpSocket) // Revert to default behavior which is mandatory for this test case.
			{
				settingsA.TextTerminal.TxDisplay.ChunkLineBreakEnabled = false;
				settingsA.TextTerminal.RxDisplay.ChunkLineBreakEnabled = false;

				settingsA.TextTerminal.TxEol = TextTerminalSettings.EolDefault;
				settingsA.TextTerminal.RxEol = TextTerminalSettings.EolDefault;

				if (doTwoWay)
					settingsA.Send.DefaultLineRepeat--; // Initial ping-pong needed.
			}

			using (var terminalA = new Terminal(Settings.Create(settingsA)))
			{
				terminalA.MessageInputRequest += Utilities.TerminalMessageInputRequest;
				if (!terminalA.Start())
				{
					if (Utilities.TerminalMessageInputRequestResultsInExclude) {
						Assert.Ignore(Utilities.TerminalMessageInputRequestResultsInExcludeText);
					//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.
					}
					else {
						Assert.Fail(@"Failed to start """ + terminalA.Caption + @"""");
					}
				}
				Utilities.WaitForStart(terminalA);

				if (settingsB != null) // Loopback pair:
				{
					settingsB.Send.DefaultLineRepeat = repeatCount; // Set settings to the desired repeat count.

					if (settingsB.IO.IOTypeIsUdpSocket) // Revert to default behavior which is mandatory for this test case.
					{
						settingsB.TextTerminal.TxDisplay.ChunkLineBreakEnabled = false;
						settingsB.TextTerminal.RxDisplay.ChunkLineBreakEnabled = false;

						settingsB.TextTerminal.TxEol = TextTerminalSettings.EolDefault;
						settingsB.TextTerminal.RxEol = TextTerminalSettings.EolDefault;

						if (doTwoWay)
							settingsB.Send.DefaultLineRepeat--; // Initial ping-pong needed.
					}

					using (var terminalB = new Terminal(Settings.Create(settingsB)))
					{
						terminalB.MessageInputRequest += Utilities.TerminalMessageInputRequest;
						if (!terminalB.Start())
						{
							if (Utilities.TerminalMessageInputRequestResultsInExclude) {
								Assert.Ignore(Utilities.TerminalMessageInputRequestResultsInExcludeText);
							//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.
							}
							else {
								Assert.Fail(@"Failed to start """ + terminalB.Caption + @"""");
							}
						}
						Utilities.WaitForConnection(terminalA, terminalB);

						TransmitAndVerify(terminalA, terminalB, repeatCount, doTwoWay, executeBreak);
					}
				}
				else // Loopback self:
				{
					TransmitAndVerify(terminalA, terminalA, repeatCount, doTwoWay, executeBreak);
				}
			}
		}

		private static void TransmitAndVerify(Terminal terminalA, Terminal terminalB, int repeatCount, bool doTwoWay, bool executeBreak)
		{
			if (repeatCount >= 0)
				TransmitAndVerifySpecific(terminalA, terminalB, repeatCount, doTwoWay, executeBreak);
			else
				TransmitAndVerifyRandom(terminalA, terminalB, executeBreak); // Yet limited to one-way.
		}

		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "Too many values to verify.")]
		private static void TransmitAndVerifySpecific(Terminal terminalA, Terminal terminalB, int repeatCount, bool doTwoWay, bool executeBreak)
		{
			var repeatCommand = new Types.Command(RepeatingTestData.TestCommand);

			var expectedLineCount = repeatCount;
			if (doTwoWay && (terminalA == terminalB)) // Loopback self:
				expectedLineCount *= 2;               // Twice the number of lines.

			var expectedLineByteCount = RepeatingTestData.TestString.Length + 2; // Content + EOL.
			var expectedTotalByteCount = (expectedLineCount * expectedLineByteCount);

			var testSet = new Utilities.TestSet
			(
				repeatCommand, expectedLineCount,
				ArrayEx.CreateAndInitializeInstance(expectedLineCount, 2), // 2 Elements: Content + EOL.
				ArrayEx.CreateAndInitializeInstance(expectedLineCount, expectedLineByteCount),
				false
			);

			// Two-way UDP/IP requires an initial ping-pong to tell server where to respond to:
			var requiresInitialPingPong = (doTwoWay && terminalA.SettingsRoot.IO.IOTypeIsUdpSocket);
			if (requiresInitialPingPong)
			{
				var singleCommand = new Types.Command(RepeatingTestData.TestString);

				var expectedInitialPingByteCount = expectedLineByteCount;
				var expectedInitialPongByteCount = expectedLineByteCount;
				if (doTwoWay && (terminalA == terminalB)) // Loopback self:
					expectedInitialPongByteCount *= 2;    // Twice the number of bytes.

				var expectedInitialPingLineCount = 1;
				var expectedInitialPongLineCount = 1;
				if (doTwoWay && (terminalA == terminalB)) // Loopback self:
					expectedInitialPongLineCount *= 2;    // Twice the number of lines.

				terminalA.SendText(singleCommand);
				Utilities.WaitForTransmissionAndVerifyCounts(terminalA, terminalB,
				                                             expectedTotalByteCount:          expectedInitialPingByteCount,
				                                             expectedTotalLineCountDisplayed: expectedInitialPingLineCount,
				                                             expectedTotalLineCountCompleted: expectedInitialPingLineCount,
				                                             timeout: (1 * Utilities.WaitTimeoutForLineTransmission));

				terminalB.SendText(singleCommand);
				Utilities.WaitForTransmissionAndVerifyCounts(terminalB, terminalA,
				                                             expectedTotalByteCount:          expectedInitialPongByteCount,
				                                             expectedTotalLineCountDisplayed: expectedInitialPongLineCount,
				                                             expectedTotalLineCountCompleted: expectedInitialPongLineCount,
				                                             timeout: (1 * Utilities.WaitTimeoutForLineTransmission));
			}

			// Send repeating simplex or duplex (for even better test coverage):
			var requiresInitialPingPongAndIsJustOne = (requiresInitialPingPong && (repeatCount == 1));
			if (!requiresInitialPingPongAndIsJustOne)
			{
				terminalA.SendText(repeatCommand);
				if (doTwoWay) {
					terminalB.SendText(repeatCommand);
				}
			}

			Utilities.WaitForTransmissionAndVerifyCounts(terminalA, terminalB,
			                                             expectedTotalByteCount:          expectedTotalByteCount,
			                                             expectedTotalLineCountDisplayed: expectedLineCount,
			                                             expectedTotalLineCountCompleted: expectedLineCount,
			                                             timeout: (expectedLineCount * Utilities.WaitTimeoutForLineTransmission));
			if (doTwoWay) {
				Utilities.WaitForTransmissionAndVerifyCounts(terminalB, terminalA,
				                                             expectedTotalByteCount:          expectedTotalByteCount,
				                                             expectedTotalLineCountDisplayed: expectedLineCount,
				                                             expectedTotalLineCountCompleted: expectedLineCount,
				                                             timeout: (expectedLineCount * Utilities.WaitTimeoutForLineTransmission));
			}

			// Wait to ensure that no operation is ongoing anymore:
			Utilities.WaitForReverification();

			// Verify transmission:
			Utilities.VerifyLines(terminalA.RepositoryToDisplayLines(Domain.RepositoryType.Tx),
			                      terminalB.RepositoryToDisplayLines(Domain.RepositoryType.Rx),
			                      testSet);
			if (doTwoWay) {
				Utilities.VerifyLines(terminalB.RepositoryToDisplayLines(Domain.RepositoryType.Tx),
				                      terminalA.RepositoryToDisplayLines(Domain.RepositoryType.Rx),
				                      testSet);
			}
		}

		/// <remarks>
		/// Yet limited to one-way.
		/// </remarks>
		private static void TransmitAndVerifyRandom(Terminal terminalA, Terminal terminalB, bool executeBreak)
		{
			var repeatCommand = new Types.Command(RepeatingTestData.TestCommand);

			terminalA.SendText(repeatCommand);

			var random = new Random(RandomEx.NextPseudoRandomSeed());
			Thread.Sleep(random.Next(100, 10000)); // Something between 0.1..10 seconds to keep test execution fast.

			// Break or stop:
			if (executeBreak)
			{
				terminalA.Break();
				terminalB.Break();
			}
			else
			{
				terminalA.StopIO();
				terminalB.StopIO();
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
