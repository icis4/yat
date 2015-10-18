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

using MKY;
using MKY.Collections.Generic;
using MKY.Settings;

using NUnit.Framework;

using YAT.Settings.Application;
using YAT.Settings.Terminal;

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

				yield return (new TestCaseData(Domain.Settings.SendSettings.LineRepeatInfinite, false, true ).SetName("_RepeatInfiniteOneWayAndBreak"));
				yield return (new TestCaseData(Domain.Settings.SendSettings.LineRepeatInfinite, false, false).SetName("_RepeatInfiniteOneWayUntilExit"));
				yield return (new TestCaseData(Domain.Settings.SendSettings.LineRepeatInfinite, true,  true ).SetName("_RepeatInfiniteTwoWayAndBreak"));
				yield return (new TestCaseData(Domain.Settings.SendSettings.LineRepeatInfinite, true,  false).SetName("_RepeatInfiniteTwoWayUntilExit"));
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
	public class RepeatingTest
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
		[Test, TestCaseSource(typeof(RepeatingTestData), "TestCasesSerialPortLoopbackPairs")]
		public virtual void SerialPortLoopbackPairs(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorA,
		                                            Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorB,
		                                            int repeatCount, bool doTwoWay, bool executeBreak)
		{
			PerformTransmission(settingsDescriptorA, settingsDescriptorB, repeatCount, doTwoWay, executeBreak);
		}

		/// <remarks>Separation into multiple tests for easier handling and execution.</remarks>
		[Test, TestCaseSource(typeof(RepeatingTestData), "TestCasesSerialPortLoopbackSelfs")]
		public virtual void SerialPortLoopbackSelfs(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorA,
		                                            Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorB,
		                                            int repeatCount, bool doTwoWay, bool executeBreak)
		{
			PerformTransmission(settingsDescriptorA, settingsDescriptorB, repeatCount, doTwoWay, executeBreak);
		}

		/// <remarks>Separation into multiple tests for easier handling and execution.</remarks>
		[Test, TestCaseSource(typeof(RepeatingTestData), "TestCasesIPLoopbacks")]
		public virtual void IPLoopbacks(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorA,
		                                Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorB,
		                                int repeatCount, bool doTwoWay, bool executeBreak)
		{
			PerformTransmission(settingsDescriptorA, settingsDescriptorB, repeatCount, doTwoWay, executeBreak);
		}

		private void PerformTransmission(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorA,
		                                 Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorB,
		                                 int repeatCount, bool doTwoWay, bool executeBreak)
		{
			TerminalSettingsRoot settingsA = settingsDescriptorA.Value1(settingsDescriptorA.Value2);
			settingsA.Send.DefaultLineRepeat = repeatCount; // Set settings to the desired repeat count.
			using (Terminal terminalA = new Terminal(settingsA))
			{
				terminalA.Start();
				Utilities.WaitForConnection(terminalA);

				if (settingsDescriptorB.Value1 != null) // Loopback pair.
				{
					TerminalSettingsRoot settingsB = settingsDescriptorB.Value1(settingsDescriptorB.Value2);
					settingsB.Send.DefaultLineRepeat = repeatCount; // Set settings to the desired repeat count.
					using (Terminal terminalB = new Terminal(settingsB))
					{
						terminalB.Start();
						Utilities.WaitForConnection(terminalA, terminalB);

						PerformTransmission(terminalA, terminalB, repeatCount, doTwoWay, executeBreak);
					}
				}
				else // Loopback self.
				{
					PerformTransmission(terminalA, terminalA, repeatCount, doTwoWay, executeBreak);
				}
			}
		}

		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "There are too many parameters to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "There are too many parameters to verify.")]
		private void PerformTransmission(Terminal terminalA, Terminal terminalB, int repeatCount, bool doTwoWay, bool executeBreak)
		{
			Types.Command command = new Types.Command(RepeatingTestData.TestCommand);

			terminalA.SendText(command);
			if (doTwoWay)
				terminalB.SendText(command);

			if (repeatCount >= 0) // Finite count.
			{
				Utilities.TestSet testSet = new Utilities.TestSet
					(
					command, repeatCount,
					ArrayEx.CreateAndInitializeInstance<int>(repeatCount, 2), // Data + EOL
					ArrayEx.CreateAndInitializeInstance<int>(repeatCount, RepeatingTestData.TestString.Length),
					false
					);

				Utilities.WaitForTransmission(terminalA, terminalB, repeatCount);
				if (doTwoWay)
					Utilities.WaitForTransmission(terminalB, terminalA, repeatCount);

				// Verify transmission:
				Utilities.VerifyLines(terminalA.RepositoryToDisplayLines(Domain.RepositoryType.Tx),
				                      terminalB.RepositoryToDisplayLines(Domain.RepositoryType.Rx),
				                      testSet);
				if (doTwoWay)
					Utilities.VerifyLines(terminalB.RepositoryToDisplayLines(Domain.RepositoryType.Tx),
					                      terminalA.RepositoryToDisplayLines(Domain.RepositoryType.Rx),
					                      testSet);
			}
			else // Infinite count.
			{
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
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
