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
using MKY.Collections.Generic;
using MKY.Settings;

using NUnit.Framework;

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

		private static IEnumerable<TestCaseData> TestCasesCommandData
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

		/// <param name="loopbackSettings">
		/// Quadruple of...
		/// ...Pair(terminalSettingsDelegateA, terminalSettingsArgumentA)...
		/// ...Pair(terminalSettingsDelegateB, terminalSettingsArgumentB)...
		/// ...string testCaseName...
		/// ...string[] testCaseCategories.
		/// </param>
		private static IEnumerable<TestCaseData> TestCases(Quadruple<Pair<Utilities.TerminalSettingsDelegate<string>, string>, Pair<Utilities.TerminalSettingsDelegate<string>, string>, string, string[]> loopbackSettings)
		{
			foreach (var commandData in TestCasesCommandData) // TestCaseData(int repeatCount, bool doTwoWay, bool executeBreak).
			{
				// Arguments:
				var args = new List<object>(commandData.Arguments);
				args.Insert(0, loopbackSettings.Value1); // Insert the settings descriptor A at the beginning.
				args.Insert(1, loopbackSettings.Value2); // Insert the settings descriptor B at second.
				var tcd = new TestCaseData(args.ToArray()); // TestCaseData(Pair settingsDescriptorA, Pair settingsDescriptorB, int repeatCount, bool doTwoWay, bool executeBreak).

				// Name:
				tcd.SetName(loopbackSettings.Value3 + commandData.TestName);

				// Category(ies):
				foreach (string cat in loopbackSettings.Value4)
					tcd.SetCategory(cat);

				yield return (tcd);
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCasesSerialPortLoopbackPairs
		{
			get
			{
				foreach (var lp in Utilities.TransmissionSettings.SerialPortLoopbackPairs)
				{
					foreach (var tc in TestCases(lp))
						yield return (tc);
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		public static IEnumerable TestCasesSerialPortLoopbackSelfs
		{
			get
			{
				foreach (var ls in Utilities.TransmissionSettings.SerialPortLoopbackSelfs)
				{
					foreach (var tc in TestCases(ls))
						yield return (tc);
				}
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCasesIPLoopbackPairs
		{
			get
			{
				foreach (var lp in Utilities.TransmissionSettings.IPLoopbackPairs)
				{
					foreach (var tc in TestCases(lp))
						yield return (tc);
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		public static IEnumerable TestCasesIPLoopbackSelfs
		{
			get
			{
				foreach (var ls in Utilities.TransmissionSettings.IPLoopbackSelfs)
				{
					foreach (var tc in TestCases(ls))
						yield return (tc);
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
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "Too many values to verify.")]
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Don't care, straightforward test implementation.")]
		[Test, TestCaseSource(typeof(RepeatingTestData), "TestCasesSerialPortLoopbackPairs")]
		public virtual void SerialPortLoopbackPairs(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorA,
		                                            Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorB,
		                                            int repeatCount, bool doTwoWay, bool executeBreak)
		{
			TransmitAndVerify(settingsDescriptorA, settingsDescriptorB, repeatCount, doTwoWay, executeBreak);
		}

		/// <remarks>Separation into multiple tests for easier handling and execution.</remarks>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "Too many values to verify.")]
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Don't care, straightforward test implementation.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		[Test, TestCaseSource(typeof(RepeatingTestData), "TestCasesSerialPortLoopbackSelfs")]
		public virtual void SerialPortLoopbackSelfs(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorA,
		                                            Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorB,
		                                            int repeatCount, bool doTwoWay, bool executeBreak)
		{
			TransmitAndVerify(settingsDescriptorA, settingsDescriptorB, repeatCount, doTwoWay, executeBreak);
		}

		/// <remarks>Separation into multiple tests for easier handling and execution.</remarks>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "Too many values to verify.")]
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Don't care, straightforward test implementation.")]
		[Test, TestCaseSource(typeof(RepeatingTestData), "TestCasesIPLoopbackPairs")]
		public virtual void IPLoopbackPairs(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorA,
		                                    Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorB,
		                                    int repeatCount, bool doTwoWay, bool executeBreak)
		{
			TransmitAndVerify(settingsDescriptorA, settingsDescriptorB, repeatCount, doTwoWay, executeBreak);
		}

		/// <remarks>Separation into multiple tests for easier handling and execution.</remarks>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "Too many values to verify.")]
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Don't care, straightforward test implementation.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		[Test, TestCaseSource(typeof(RepeatingTestData), "TestCasesIPLoopbackSelfs")]
		public static void IPLoopbackSelfs(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorA,
		                                   Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorB,
		                                   int repeatCount, bool doTwoWay, bool executeBreak)
		{
			TransmitAndVerify(settingsDescriptorA, settingsDescriptorB, repeatCount, doTwoWay, executeBreak);
		}

		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "Too many values to verify.")]
		private static void TransmitAndVerify(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorA,
		                                      Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorB,
		                                      int repeatCount, bool doTwoWay, bool executeBreak)
		{
			var settingsA = settingsDescriptorA.Value1(settingsDescriptorA.Value2);
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

				if (settingsDescriptorB.Value1 != null) // Loopback pair:
				{
					var settingsB = settingsDescriptorB.Value1(settingsDescriptorB.Value2);
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
