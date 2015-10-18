//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
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
using YAT.Settings.Terminal;

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
		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Future test cases may have to implement more logic in the constructor, and anyway, performance isn't an issue here.")]
		static TwoWayTestData()
		{
			PingPongCommand = new Utilities.TestSet(new Types.Command(@"ABC DE F"), 1, new int[] { 2 }, new int[] { 8 }, true);
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
			////Takes several minutes and doesn't reproduce bugs #3284550>#194 and #3480565>#221, therefore disabled.
			}
		}

		private static IEnumerable<TestCaseData> TestCases(Quadruple<Pair<Utilities.TerminalSettingsDelegate<string>, string>, Pair<Utilities.TerminalSettingsDelegate<string>, string>, string, string[]> loopbackSettings)
		{
			foreach (TestCaseData commandData in TestCasesCommandData)
			{
				// Arguments:
				List<object> args = new List<object>(commandData.Arguments);
				args.Insert(0, loopbackSettings.Value1); // Insert the settings delegate at the beginning.
				args.Insert(1, loopbackSettings.Value2); // Insert the settings delegate at the beginning.
				TestCaseData tcd = new TestCaseData(args.ToArray());

				// Category(ies):
				foreach (string cat in loopbackSettings.Value4)
					tcd.SetCategory(cat);

				// Name:
				tcd.SetName(loopbackSettings.Value3 + commandData.TestName);

				yield return (tcd);
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCasesSerialPortLoopbackPairs
		{
			get
			{
				foreach (Quadruple<Pair<Utilities.TerminalSettingsDelegate<string>, string>, Pair<Utilities.TerminalSettingsDelegate<string>, string>, string, string[]> loopbackSettings in Utilities.TransmissionSettings.SerialPortLoopbackPairs)
					yield return (TestCases(loopbackSettings));
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCasesSerialPortLoopbackSelfs
		{
			get
			{
				foreach (Quadruple<Pair<Utilities.TerminalSettingsDelegate<string>, string>, Pair<Utilities.TerminalSettingsDelegate<string>, string>, string, string[]> loopbackSettings in Utilities.TransmissionSettings.SerialPortLoopbackSelfs)
					yield return (TestCases(loopbackSettings));
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCasesIPLoopbacks
		{
			get
			{
				foreach (Quadruple<Pair<Utilities.TerminalSettingsDelegate<string>, string>, Pair<Utilities.TerminalSettingsDelegate<string>, string>, string, string[]> loopbackSettings in Utilities.TransmissionSettings.IPLoopbacks)
					yield return (TestCases(loopbackSettings));
			}
		}

		#endregion
	}

	/// <remarks>
	/// It can be argued that this test would be better located in YAT.Domain.Test. It currently is
	/// located here because line counts and rates are calculated in <see cref="YAT.Model.Terminal"/>
	/// and required when evaluating the test result.
	/// </remarks>
	[TestFixture]
	public class TwoWayTest
	{
		#region Set Up Fixture
		//==========================================================================================
		// Set Up Fixture
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "SetUp", Justification = "Naming according to NUnit.")]
		[TestFixtureSetUp]
		public virtual void TestFixtureSetUp()
		{
			// Create temporary in-memory application settings for this test run.
			ApplicationSettings.Create(ApplicationSettingsFileAccess.None);

			// Prevent auto-save of workspace settings.
			ApplicationSettings.LocalUserSettings.General.AutoSaveWorkspace = false;
		}

		#endregion

		#region Tear Down Fixture
		//==========================================================================================
		// Tear Down Fixture
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TearDown", Justification = "Naming according to NUnit.")]
		[TestFixtureTearDown]
		public virtual void TestFixtureTearDown()
		{
			// Close temporary in-memory application settings.
			ApplicationSettings.Close();
		}

		#endregion

		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		/// <remarks>Separation into multiple tests for easier handling and execution.</remarks>
		[Test, TestCaseSource(typeof(TwoWayTestData), "TestCasesSerialPortLoopbackPairs")]
		public void SerialPortLoopbackPairs(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorA,
		                                    Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorB,
		                                    Utilities.TestSet testSet, int transmissionCount)
		{
			PerformTransmission(settingsDescriptorA, settingsDescriptorB, testSet, transmissionCount);
		}

		/// <remarks>Separation into multiple tests for easier handling and execution.</remarks>
		[Test, TestCaseSource(typeof(TwoWayTestData), "TestCasesSerialPortLoopbackSelfs")]
		public void SerialPortLoopbackSelfs(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorA,
		                                    Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorB,
		                                    Utilities.TestSet testSet, int transmissionCount)
		{
			PerformTransmission(settingsDescriptorA, settingsDescriptorB, testSet, transmissionCount);
		}

		/// <remarks>Separation into multiple tests for easier handling and execution.</remarks>
		[Test, TestCaseSource(typeof(TwoWayTestData), "TestCasesIPLoopbacks")]
		public virtual void IPLoopbacks(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorA,
		                                Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorB,
		                                Utilities.TestSet testSet, int transmissionCount)
		{
			PerformTransmission(settingsDescriptorA, settingsDescriptorB, testSet, transmissionCount);
		}

		private void PerformTransmission(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorA,
		                                 Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorB,
		                                 Utilities.TestSet testSet, int transmissionCount)
		{
			TerminalSettingsRoot settingsA = settingsDescriptorA.Value1(settingsDescriptorA.Value2);
			using (Terminal terminalA = new Terminal(settingsA))
			{
				terminalA.Start();
				Utilities.WaitForConnection(terminalA);

				if (settingsDescriptorB.Value1 != null) // Loopback pair.
				{
					TerminalSettingsRoot settingsB = settingsDescriptorB.Value1(settingsDescriptorB.Value2);
					using (Terminal terminalB = new Terminal(settingsB))
					{
						terminalB.Start();
						Utilities.WaitForConnection(terminalA, terminalB);

						PerformTransmission(terminalA, terminalB, testSet, transmissionCount);
					}
				}
				else // Loopback self.
				{
					PerformTransmission(terminalA, terminalA, testSet, transmissionCount);
				}
			}
		}

		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "There are too many parameters to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "There are too many parameters to verify.")]
		private void PerformTransmission(Terminal terminalA, Terminal terminalB, Utilities.TestSet testSet, int transmissionCount)
		{
			for (int i = 0; i < transmissionCount; i++)
			{
				// Send 'Ping' test command A > B :
				terminalA.SendText(testSet.Command);
				Utilities.WaitForTransmission(terminalA, terminalB, testSet);

				// Verify transmission:
				Utilities.VerifyLines(terminalA.RepositoryToDisplayLines(Domain.RepositoryType.Tx),
				                      terminalB.RepositoryToDisplayLines(Domain.RepositoryType.Rx),
				                      testSet, i + 1);

				// Send 'Pong' test command B > A :
				terminalB.SendText(testSet.Command);
				Utilities.WaitForTransmission(terminalB, terminalA, testSet);

				// Verify transmission:
				Utilities.VerifyLines(terminalB.RepositoryToDisplayLines(Domain.RepositoryType.Tx),
				                      terminalA.RepositoryToDisplayLines(Domain.RepositoryType.Rx),
				                      testSet, i + 1);
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
