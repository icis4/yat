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

using System;
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
	public static class OneWayTestData
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private static readonly string[] TestCommandLines = new string[]
			{
				@"1a2a3a4a5a6a7a8a9a0",
				@"1b2b3b4b5b6b7b8b9b0",
				@"1c2c3c4c5c6c7c8c9c0",
				@"1d2d3d4d5d6d7d8d9d0",
				@"1e2e3e4e5e6e7e8e9e0",
				@"1f2f3f4f5f6f7f8f9f0",
				@"1g2g3g4g5g6g7g8g9g0",
				@"1h2h3h4h5h6h7h8h9h0",
				@"1i2i3i4i5i6i7i8i9i0",
				@"1j2j3j4j5j6j7j8j9j0",
			};

		private static readonly Utilities.TestSet SingleLineCommand;
		private static readonly Utilities.TestSet DoubleLineCommand;
		private static readonly Utilities.TestSet TripleLineCommand;
		private static readonly Utilities.TestSet MultiLineCommand;

		private static readonly Utilities.TestSet MultiEolCommand;
		private static readonly Utilities.TestSet MixedEolCommand;

		private static readonly Utilities.TestSet EolPartsCommand;
		private static readonly Utilities.TestSet EolOnlyCommand;

		private static readonly Utilities.TestSet SingleNoEolCommand;
		private static readonly Utilities.TestSet DoubleNoEolCommand;
		private static readonly Utilities.TestSet StillEolCommand1;
		private static readonly Utilities.TestSet StillEolCommand2;
		private static readonly Utilities.TestSet StillEolCommand3;

		private static readonly Utilities.TestSet ControlCharCommand1;
		private static readonly Utilities.TestSet ControlCharCommand2;
		private static readonly Utilities.TestSet ControlCharCommand3;

		private static readonly Utilities.TestSet ClearCommand;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Future test cases may have to implement more logic in the constructor, and anyway, performance isn't an issue here.")]
		static OneWayTestData()
		{
			SingleLineCommand   = new Utilities.TestSet(new Types.Command(TestCommandLines[0]));
			DoubleLineCommand   = new Utilities.TestSet(new Types.Command(new string[] { TestCommandLines[0], TestCommandLines[1] } ));
			TripleLineCommand   = new Utilities.TestSet(new Types.Command(new string[] { TestCommandLines[0], TestCommandLines[1], TestCommandLines[2] }));
			MultiLineCommand    = new Utilities.TestSet(new Types.Command(TestCommandLines));

			MultiEolCommand     = new Utilities.TestSet(new Types.Command(@"A\!(EOL)B<CR><LF>C<CR><LF>D"), 4, new int[] { 2, 2, 2, 2 }, new int[] { 1, 1, 1, 1 }, true); // Eol results in one element since ShowEol is switched off.
			MixedEolCommand     = new Utilities.TestSet(new Types.Command(@"A\!(EOL)BC<CR><LF>D"),         3, new int[] { 2, 2, 2    }, new int[] { 1, 2, 1    }, true); // Eol results in one element since ShowEol is switched off.

			EolPartsCommand     = new Utilities.TestSet(new Types.Command(@"A<CR><CR><LF>B<CR><LF><LF>C<CR><LF>D<CR>E<LF>F"), 4, new int[] { 3, 2, 3, 6 }, new int[] { 2, 1, 2, 5 }, true);
			EolOnlyCommand      = new Utilities.TestSet(new Types.Command(new string[] { "A", "B", "", "C" }),                4, new int[] { 2, 2, 1, 2 }, new int[] { 1, 1, 0, 1 }, true);

			SingleNoEolCommand  = new Utilities.TestSet(new Types.Command(@"A\!(NoEOL)"),                                 1, new int[] { 1 },    new int[] { 1 },    true); // There is always 1 line.
			DoubleNoEolCommand  = new Utilities.TestSet(new Types.Command(new string[] { @"A\!(NoEOL)", @"B\!(NoEOL)" }), 1, new int[] { 1 },    new int[] { 2 },    true); // There is always 1 line.
			StillEolCommand1    = new Utilities.TestSet(new Types.Command(@"<CR><LF>\!(NoEOL)"),                          1, new int[] { 1 },    new int[] { 0 },    true);
			StillEolCommand2    = new Utilities.TestSet(new Types.Command(@"A<CR><LF>\!(NoEOL)"),                         1, new int[] { 2 },    new int[] { 1 },    true);
			StillEolCommand3    = new Utilities.TestSet(new Types.Command(@"A<CR><LF>B\!(NoEOL)"),                        2, new int[] { 2, 1 }, new int[] { 1, 1 }, true);

			ControlCharCommand1 = new Utilities.TestSet(new Types.Command(@"\h(00)<CR><LF>\h(00)A<CR><LF>A\h(00)<CR><LF>A\h(00)A"), 4, new int[] { 2, 3, 3, 4 }, new int[] { 1, 2, 2, 3 }, true);
			ControlCharCommand2 = new Utilities.TestSet(new Types.Command(@"\h(7F)<CR><LF>\h(7F)A<CR><LF>A\h(7F)<CR><LF>A\h(7F)A"), 4, new int[] { 2, 3, 3, 4 }, new int[] { 1, 2, 2, 3 }, true);
			ControlCharCommand3 = new Utilities.TestSet(new Types.Command(@"\h(FF)<CR><LF>\h(FF)A<CR><LF>A\h(FF)<CR><LF>A\h(FF)A"), 4, new int[] { 2, 2, 2, 2 }, new int[] { 1, 2, 2, 3 }, true); // A non-breaking space isn't a control character.

			ClearCommand        = new Utilities.TestSet(new Types.Command(@"A<CR><LF>B<CR><LF>C\!(Clear)\!(NoEOL)"), 3, new int[] { 1, 1, 1 }, new int[] { 0 }, false);
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
				yield return (new TestCaseData(SingleLineCommand,   1).SetName("_SingleLineTransmission"));
				yield return (new TestCaseData(DoubleLineCommand,   1).SetName("_DoubleLineTransmission"));
				yield return (new TestCaseData(DoubleLineCommand,   2).SetName("_DoubleLineDoubleTransmission"));

				yield return (new TestCaseData(TripleLineCommand,   1).SetName("_TripleLineTransmission"));
				yield return (new TestCaseData(TripleLineCommand,   3).SetName("_TripleLineTripleTransmission"));

				yield return (new TestCaseData(MultiLineCommand,    1).SetName("_MultiLineTransmission"));
				yield return (new TestCaseData(MultiLineCommand, TestCommandLines.Length).SetName("_MultiLineMultiTransmission"));

				yield return (new TestCaseData(MultiEolCommand,     1).SetName("_MultiEolTransmission"));
				yield return (new TestCaseData(MixedEolCommand,     1).SetName("_MixedEolTransmission"));

				yield return (new TestCaseData(EolPartsCommand,     1).SetName("_EolPartsTransmission"));
				yield return (new TestCaseData(EolOnlyCommand,      1).SetName("_EolOnlyTransmission"));

				yield return (new TestCaseData(SingleNoEolCommand,  1).SetName("_SingleNoEolTransmission"));
				yield return (new TestCaseData(DoubleNoEolCommand,  1).SetName("_DoubleNoEolTransmission"));
				yield return (new TestCaseData(StillEolCommand1,    1).SetName("_StillEolCommand1"));
				yield return (new TestCaseData(StillEolCommand2,    1).SetName("_StillEolCommand2"));
				yield return (new TestCaseData(StillEolCommand3,    1).SetName("_StillEolCommand3"));

				yield return (new TestCaseData(ControlCharCommand1, 1).SetName("_ControlCharCommand1"));
				yield return (new TestCaseData(ControlCharCommand2, 1).SetName("_ControlCharCommand2"));
				yield return (new TestCaseData(ControlCharCommand3, 1).SetName("_ControlCharCommand3"));

				yield return (new TestCaseData(ClearCommand,        1).SetName("_ClearCommand"));
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
			foreach (TestCaseData commandData in TestCasesCommandData) // TestCaseData(Utilities.TestSet command, int transmissionCount).
			{
				// Arguments:
				List<object> args = new List<object>(commandData.Arguments);
				args.Insert(0, loopbackSettings.Value1); // Insert the settings descriptor A at the beginning.
				args.Insert(1, loopbackSettings.Value2); // Insert the settings descriptor B at second.
				TestCaseData tcd = new TestCaseData(args.ToArray()); // TestCaseData(Pair settingsDescriptorA, Pair settingsDescriptorB, Utilities.TestSet command, int transmissionCount).

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
				foreach (Quadruple<Pair<Utilities.TerminalSettingsDelegate<string>, string>, Pair<Utilities.TerminalSettingsDelegate<string>, string>, string, string[]> loopbackSettings in Utilities.TransmissionSettings.SerialPortLoopbackPairs)
				{
					foreach (TestCaseData testCase in TestCases(loopbackSettings))
						yield return (testCase);
				}
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCasesSerialPortLoopbackSelfs
		{
			get
			{
				foreach (Quadruple<Pair<Utilities.TerminalSettingsDelegate<string>, string>, Pair<Utilities.TerminalSettingsDelegate<string>, string>, string, string[]> loopbackSettings in Utilities.TransmissionSettings.SerialPortLoopbackSelfs)
				{
					foreach (TestCaseData testCase in TestCases(loopbackSettings))
						yield return (testCase);
				}
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCasesIPLoopbacks
		{
			get
			{
				foreach (Quadruple<Pair<Utilities.TerminalSettingsDelegate<string>, string>, Pair<Utilities.TerminalSettingsDelegate<string>, string>, string, string[]> loopbackSettings in Utilities.TransmissionSettings.IPLoopbacks)
				{
					foreach (TestCaseData testCase in TestCases(loopbackSettings))
						yield return (testCase);
				}
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
	public class OneWayTest
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
		[Test, TestCaseSource(typeof(OneWayTestData), "TestCasesSerialPortLoopbackPairs")]
		public virtual void SerialPortLoopbackPairs(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorA,
		                                            Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorB,
		                                            Utilities.TestSet testSet, int transmissionCount)
		{
			PerformTransmission(settingsDescriptorA, settingsDescriptorB, testSet, transmissionCount);
		}

		/// <remarks>Separation into multiple tests for easier handling and execution.</remarks>
		[Test, TestCaseSource(typeof(OneWayTestData), "TestCasesSerialPortLoopbackSelfs")]
		public virtual void SerialPortLoopbackSelfs(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorA,
		                                            Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorB,
		                                            Utilities.TestSet testSet, int transmissionCount)
		{
			PerformTransmission(settingsDescriptorA, settingsDescriptorB, testSet, transmissionCount);
		}

		/// <remarks>Separation into multiple tests for easier handling and execution.</remarks>
		[Test, TestCaseSource(typeof(OneWayTestData), "TestCasesIPLoopbacks")]
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
				terminalA.MessageInputRequest += new EventHandler<MessageInputEventArgs>(PerformTransmission_terminal_MessageInputRequest);
				Assert.IsTrue(terminalA.Start(), @"Failed to start """ + terminalA.Caption + @"""");
				Utilities.WaitForConnection(terminalA);

				if (settingsDescriptorB.Value1 != null) // Loopback pair.
				{
					TerminalSettingsRoot settingsB = settingsDescriptorB.Value1(settingsDescriptorB.Value2);
					using (Terminal terminalB = new Terminal(settingsB))
					{
						terminalB.MessageInputRequest += new EventHandler<MessageInputEventArgs>(PerformTransmission_terminal_MessageInputRequest);
						Assert.IsTrue(terminalB.Start(), @"Failed to start """ + terminalB.Caption + @"""");
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
			for (int cycle = 1; cycle <= transmissionCount; cycle++)
			{
				// Send test command:
				terminalA.SendText(testSet.Command);
				Utilities.WaitForTransmission(terminalA, terminalB, testSet.ExpectedLineCount * cycle);

				// Verify transmission:
				Utilities.VerifyLines(terminalA.RepositoryToDisplayLines(Domain.RepositoryType.Tx),
				                      terminalB.RepositoryToDisplayLines(Domain.RepositoryType.Rx),
				                      testSet, testSet.ExpectedLineCount * cycle);
			}
		}

		private static void PerformTransmission_terminal_MessageInputRequest(object sender, MessageInputEventArgs e)
		{
			Assert.Fail
			(
				"Unexpected message input request:" + Environment.NewLine + Environment.NewLine +
				e.Caption + Environment.NewLine + Environment.NewLine +
				e.Text
			);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
