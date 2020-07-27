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

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using MKY.Settings;

using NUnit.Framework;

using YAT.Domain;
using YAT.Domain.Settings;
using YAT.Settings.Application;
using YAT.Settings.Model;

#endregion

namespace YAT.Model.Test.Transmission
{
	/// <summary></summary>
	public static class TwoWayTestData
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private static readonly Utilities.TestSet PingPongCommand;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Future test cases may have to implement more logic, and anyway, performance isn't an issue here.")]
		static TwoWayTestData()
		{                                                                                          // LineStart + EOL + LineBreak result in three more elements.
			PingPongCommand = new Utilities.TestSet(new Types.Command(@"ABC DE F"), 1, new int[] { 4 }, new int[] { 10 }, true);
		}

		#endregion

		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		private static IEnumerable<TestCaseData> Tests
		{
			get
			{
				yield return (new TestCaseData(PingPongCommand,   1).SetName("_PingPong1"));
				yield return (new TestCaseData(PingPongCommand,  10).SetName("_PingPong10"));

			////yield return (new TestCaseData(PingPongCommand, 100).SetName("_PingPong100"));
			//// Takes several minutes and doesn't reproduce bugs #3284550>#194 and #3480565>#221, therefore disabled.
			//// \ToDo: Add dynamic TimeSpanCategory (same as for MT-SICS tests).
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
					var settingsA = Settings.GetSerialPortSettings(TerminalType.Text, descriptor.PortA);
					var settingsB = Settings.GetSerialPortSettings(TerminalType.Text, descriptor.PortB);

					foreach (var t in Tests)
						yield return (Data.ToTestCase(descriptor, t, settingsA, settingsB, t.Arguments));
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
					var settings = Settings.GetSerialPortSettings(TerminalType.Text, descriptor.Port);

					foreach (var t in Tests)
						yield return (Data.ToTestCase(descriptor, t, settings, t.Arguments));
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
					var settingsA = Settings.GetIPLoopbackSettings(TerminalType.Text, descriptor.SocketTypeA, descriptor.LocalInterface);
					var settingsB = Settings.GetIPLoopbackSettings(TerminalType.Text, descriptor.SocketTypeB, descriptor.LocalInterface);

					foreach (var t in Tests)
						yield return (Data.ToTestCase(descriptor, t, settingsA, settingsB, t.Arguments));
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
					var settings = Settings.GetIPLoopbackSettings(TerminalType.Text, descriptor.SocketType, descriptor.LocalInterface);

					foreach (var t in Tests)
						yield return (Data.ToTestCase(descriptor, t, settings, t.Arguments));
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
	public class TwoWayTest
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
		[Test, TestCaseSource(typeof(TwoWayTestData), "TestCasesSerialPortLoopbackPairs_Text")]
		public static void SerialPortLoopbackPairs(TerminalSettingsRoot settingsA, TerminalSettingsRoot settingsB, Utilities.TestSet testSet, int transmissionCount)
		{
			if (!MKY.IO.Ports.Test.ConfigurationProvider.Configuration.LoopbackPairsAreAvailable)
				Assert.Ignore("No serial COM port loopback pairs are available, therefore this test is excluded. Ensure that at least one serial COM port loopback pair is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			TransmitAndVerify(settingsA, settingsB, testSet, transmissionCount);
		}

		/// <remarks>Separation into multiple tests for easier handling and execution.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		[Test, TestCaseSource(typeof(TwoWayTestData), "TestCasesSerialPortLoopbackSelfs_Text")]
		public static void SerialPortLoopbackSelfs(TerminalSettingsRoot settings, Utilities.TestSet testSet, int transmissionCount)
		{
			if (!MKY.IO.Ports.Test.ConfigurationProvider.Configuration.LoopbackSelfsAreAvailable)
				Assert.Ignore("No serial COM port loopback selfs are available, therefore this test is excluded. Ensure that at least one serial COM port loopback self is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			TransmitAndVerify(settings, null, testSet, transmissionCount);
		}

		/// <remarks>Separation into multiple tests for easier handling and execution.</remarks>
		[Test, TestCaseSource(typeof(TwoWayTestData), "TestCasesIPLoopbackPairs_Text")]
		public virtual void IPLoopbackPairs(TerminalSettingsRoot settingsA, TerminalSettingsRoot settingsB, Utilities.TestSet testSet, int transmissionCount)
		{
			// IPLoopbackPairs are always made available by 'Utilities', no need to check for this.

			TransmitAndVerify(settingsA, settingsB, testSet, transmissionCount);
		}

		/// <remarks>Separation into multiple tests for easier handling and execution.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		[Test, TestCaseSource(typeof(TwoWayTestData), "TestCasesIPLoopbackSelfs_Text")]
		public static void IPLoopbackSelfs(TerminalSettingsRoot settings, Utilities.TestSet testSet, int transmissionCount)
		{
			// IPLoopbackSelfs are always made available by 'Utilities', no need to check for this.

			TransmitAndVerify(settings, null, testSet, transmissionCount);
		}

		private static void TransmitAndVerify(TerminalSettingsRoot settingsA, TerminalSettingsRoot settingsB, Utilities.TestSet testSet, int transmissionCount)
		{
			if (settingsA.IO.IOTypeIsUdpSocket) // Revert to default behavior which is mandatory for this test case.
			{
				settingsA.TextTerminal.TxDisplay.ChunkLineBreakEnabled = false;
				settingsA.TextTerminal.RxDisplay.ChunkLineBreakEnabled = false;

				settingsA.TextTerminal.TxEol = TextTerminalSettings.EolDefault;
				settingsA.TextTerminal.RxEol = TextTerminalSettings.EolDefault;
			}

			using (var terminalA = new Terminal(settingsA))
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
					if (settingsB.IO.IOTypeIsUdpSocket) // Revert to default behavior which is mandatory for this test case.
					{
						settingsB.TextTerminal.TxDisplay.ChunkLineBreakEnabled = false;
						settingsB.TextTerminal.RxDisplay.ChunkLineBreakEnabled = false;

						settingsB.TextTerminal.TxEol = TextTerminalSettings.EolDefault;
						settingsB.TextTerminal.RxEol = TextTerminalSettings.EolDefault;
					}

					using (var terminalB = new Terminal(settingsB))
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

						TransmitAndVerify(terminalA, terminalB, testSet, transmissionCount);
					}
				}
				else // Loopback self:
				{
					TransmitAndVerify(terminalA, terminalA, testSet, transmissionCount);
				}
			}
		}

		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "Too many values to verify.")]
		private static void TransmitAndVerify(Terminal terminalA, Terminal terminalB, Utilities.TestSet testSet, int transmissionCount)
		{
			int cycleAB;
			int cycleABBA;

			for (int cycle = 1; cycle <= transmissionCount; cycle++)
			{
				ToCycles(terminalA, terminalB, cycle, out cycleAB, out cycleABBA);

				// Send 'Ping' test command A => B:
				terminalA.SendText(testSet.Command);
				Utilities.WaitForTransmissionCycleAndVerifyCounts(terminalA, terminalB, testSet, cycleAB);

				// Verify transmission:
				Utilities.VerifyLines(terminalA.RepositoryToDisplayLines(Domain.RepositoryType.Tx),
				                      terminalB.RepositoryToDisplayLines(Domain.RepositoryType.Rx),
				                      testSet, cycleAB);

				// Send 'Pong' test command B => A:
				terminalB.SendText(testSet.Command);
				Utilities.WaitForTransmissionCycleAndVerifyCounts(terminalB, terminalA, testSet, cycleABBA);

				// Verify transmission:
				Utilities.VerifyLines(terminalB.RepositoryToDisplayLines(Domain.RepositoryType.Tx),
				                      terminalA.RepositoryToDisplayLines(Domain.RepositoryType.Rx),
				                      testSet, cycleABBA);
			}

			// Wait to ensure that no operation is ongoing anymore:
			Utilities.WaitForReverification();

			// Verify again:
			ToCycles(terminalA, terminalB, transmissionCount, out cycleAB, out cycleABBA);
			Utilities.VerifyLines(terminalA.RepositoryToDisplayLines(Domain.RepositoryType.Tx),
			                      terminalB.RepositoryToDisplayLines(Domain.RepositoryType.Rx),
			                      testSet, cycleABBA); // ABBA rather than AB as both ways already done.

			Utilities.VerifyLines(terminalB.RepositoryToDisplayLines(Domain.RepositoryType.Tx),
			                      terminalA.RepositoryToDisplayLines(Domain.RepositoryType.Rx),
			                      testSet, cycleABBA);
		}

		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		private static void ToCycles(Terminal terminalA, Terminal terminalB, int cycle, out int cycleAB, out int cycleABBA)
		{
			cycleAB   = cycle;
			cycleABBA = cycle;

			if (terminalA == terminalB)        // Loopback self:
			{                                  // Cycle 1, 2, 3,... must result in:
				cycleAB   = ((cycle * 2) - 1); //       1, 3, 5,...
				cycleABBA =  (cycle * 2);      //       2, 4, 6,...
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
