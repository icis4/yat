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

using MKY.Collections.Generic;
using MKY.Settings;

using NUnit.Framework;

using YAT.Settings.Application;

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

		private static IEnumerable<TestCaseData> TestCasesCommandData
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

		/// <param name="loopbackSettings">
		/// Quadruple of...
		/// ...Pair(terminalSettingsDelegateA, terminalSettingsArgumentA)...
		/// ...Pair(terminalSettingsDelegateB, terminalSettingsArgumentB)...
		/// ...string testCaseName...
		/// ...string[] testCaseCategories.
		/// </param>
		private static IEnumerable<TestCaseData> TestCases(Quadruple<Pair<Utilities.TerminalSettingsDelegate<string>, string>, Pair<Utilities.TerminalSettingsDelegate<string>, string>, string, string[]> loopbackSettings)
		{
			foreach (var commandData in TestCasesCommandData) // TestCaseData(Utilities.TestSet command, int transmissionCount).
			{
				// Arguments:
				var args = new List<object>(commandData.Arguments);
				args.Insert(0, loopbackSettings.Value1); // Insert the settings descriptor A at the beginning.
				args.Insert(1, loopbackSettings.Value2); // Insert the settings descriptor B at second.
				var tcd = new TestCaseData(args.ToArray()); // TestCaseData(Pair settingsDescriptorA, Pair settingsDescriptorB, Utilities.TestSet command, int transmissionCount).

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
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Too many values to verify.")]
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Don't care, straightforward test implementation.")]
		[Test, TestCaseSource(typeof(TwoWayTestData), "TestCasesSerialPortLoopbackPairs")]
		public static void SerialPortLoopbackPairs(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorA,
		                                           Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorB,
		                                           Utilities.TestSet testSet, int transmissionCount)
		{
			TransmitAndVerify(settingsDescriptorA, settingsDescriptorB, testSet, transmissionCount);
		}

		/// <remarks>Separation into multiple tests for easier handling and execution.</remarks>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Too many values to verify.")]
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Don't care, straightforward test implementation.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		[Test, TestCaseSource(typeof(TwoWayTestData), "TestCasesSerialPortLoopbackSelfs")]
		public static void SerialPortLoopbackSelfs(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorA,
		                                           Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorB,
		                                           Utilities.TestSet testSet, int transmissionCount)
		{
			TransmitAndVerify(settingsDescriptorA, settingsDescriptorB, testSet, transmissionCount);
		}

		/// <remarks>Separation into multiple tests for easier handling and execution.</remarks>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Too many values to verify.")]
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Don't care, straightforward test implementation.")]
		[Test, TestCaseSource(typeof(TwoWayTestData), "TestCasesIPLoopbackPairs")]
		public virtual void IPLoopbackPairs(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorA,
		                                    Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorB,
		                                    Utilities.TestSet testSet, int transmissionCount)
		{
			TransmitAndVerify(settingsDescriptorA, settingsDescriptorB, testSet, transmissionCount);
		}

		/// <remarks>Separation into multiple tests for easier handling and execution.</remarks>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Too many values to verify.")]
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Don't care, straightforward test implementation.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		[Test, TestCaseSource(typeof(TwoWayTestData), "TestCasesIPLoopbackSelfs")]
		public static void IPLoopbackSelfs(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorA,
		                                   Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorB,
		                                   Utilities.TestSet testSet, int transmissionCount)
		{
			TransmitAndVerify(settingsDescriptorA, settingsDescriptorB, testSet, transmissionCount);
		}

		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Too many values to verify.")]
		private static void TransmitAndVerify(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorA,
		                                      Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorB,
		                                      Utilities.TestSet testSet, int transmissionCount)
		{
			var settingsA = settingsDescriptorA.Value1(settingsDescriptorA.Value2);
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

		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Too many values to verify.")]
		private static void TransmitAndVerify(Terminal terminalA, Terminal terminalB, Utilities.TestSet testSet, int transmissionCount)
		{
			for (int cycle = 1; cycle <= transmissionCount; cycle++)
			{
				var cycleAB   = cycle;
				var cycleABBA = cycle;

				if (terminalA == terminalB) // Loopback self:
				{                                  // Cycle 1, 2, 3,... must result in:
					cycleAB   = ((cycle * 2) - 1); //       1, 3, 5,...
					cycleABBA =  (cycle * 2);      //       2, 4, 6,...
				}

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
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
