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
// YAT 2.0 Delta Version 1.99.80
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2017 Matthias Kläy.
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
using System.Threading;

using MKY.Collections.Generic;
using MKY.Settings;

using NUnit.Framework;

using YAT.Settings.Application;

#endregion

namespace YAT.Model.Test.Connection
{
	/// <summary></summary>
	public static class DisconnectTestData
	{
		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <param name="loopbackSettings">
		/// Quadruple of...
		/// ...Pair(terminalSettingsDelegateA, terminalSettingsArgumentA)...
		/// ...Pair(terminalSettingsDelegateB, terminalSettingsArgumentB)...
		/// ...string testCaseName...
		/// ...string[] testCaseCategories.
		/// </param>
		private static IEnumerable<TestCaseData> TestCases(Quadruple<Pair<Utilities.TerminalSettingsDelegate<string>, string>, Pair<Utilities.TerminalSettingsDelegate<string>, string>, string, string[]> loopbackSettings)
		{
			for (char disconnectIdentifier = 'A'; disconnectIdentifier <= 'B'; disconnectIdentifier++)
			{
				// Arguments:
				var tcd = new TestCaseData(loopbackSettings.Value1, loopbackSettings.Value2, disconnectIdentifier); // TestCaseData(Pair settingsDescriptorA, Pair settingsDescriptorB, char disconnectIdentifier).

				// Name:
				tcd.SetName(loopbackSettings.Value3 + "_Disconnect" + disconnectIdentifier);

				// Category(ies):
				foreach (string cat in loopbackSettings.Value4)
					tcd.SetCategory(cat);

				yield return (tcd);
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCasesIPLoopbackPairs
		{
			get
			{
				foreach (var loopbackSettings in Utilities.TransmissionSettings.IPLoopbackPairs)
				{
					foreach (var testCase in TestCases(loopbackSettings))
						yield return (testCase);
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		public static IEnumerable TestCasesIPLoopbackSelfs
		{
			get
			{
				foreach (var loopbackSettings in Utilities.TransmissionSettings.IPLoopbackSelfs)
				{
					foreach (var testCase in TestCases(loopbackSettings))
						yield return (testCase);
				}
			}
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class DisconnectTest
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
		[Test, TestCaseSource(typeof(DisconnectTestData), "TestCasesIPLoopbackPairs")]
		public virtual void IPLoopbackPairs(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorA,
		                                    Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorB,
		                                    char disconnectIdentifier)
		{
			PerformTransmissionAndDisconnect(settingsDescriptorA, settingsDescriptorB, disconnectIdentifier);
		}

		/// <remarks>Separation into multiple tests for easier handling and execution.</remarks>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Too many values to verify.")]
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Don't care, straightforward test implementation.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		[Test, TestCaseSource(typeof(DisconnectTestData), "TestCasesIPLoopbackSelfs")]
		public static void IPLoopbackSelfs(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorA,
		                                   Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorB,
		                                   char disconnectIdentifier)
		{
			PerformTransmissionAndDisconnect(settingsDescriptorA, settingsDescriptorB, disconnectIdentifier);
		}

		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Too many values to verify.")]
		private static void PerformTransmissionAndDisconnect(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorA,
		                                                     Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorB,
		                                                     char disconnectIdentifier)
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

				if (settingsDescriptorB.Value1 != null) // Loopback pair.
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

						PerformTransmissionAndDisconnect(terminalA, terminalB, disconnectIdentifier, 1);
					}
				}
				else // Loopback self.
				{
					PerformTransmissionAndDisconnect(terminalA, terminalA, disconnectIdentifier, 2);
				}
			}
		}

		private static void PerformTransmissionAndDisconnect(Terminal terminalA, Terminal terminalB, char disconnectIdentifier, int lineCountPerTransmission)
		{
			// Send initial A >> B:
			terminalA.SendText("A >> B");
			Utilities.WaitForTransmission(terminalA, terminalB, lineCountPerTransmission);

			// Send response B >> A:
			terminalB.SendText("B >> A");
			Utilities.WaitForTransmission(terminalB, terminalA, lineCountPerTransmission);

			if (disconnectIdentifier == 'A')
			{
				// Send repeating B >> A then close A:
				terminalB.SendText(@"B >> A\!(LineRepeat)");
				Thread.Sleep(333); // = approx. 30 lines.
				terminalA.StopIO();
				Utilities.WaitForDisconnection(terminalA);

				terminalA.ClearRepositories();
				terminalB.ClearRepositories();
				terminalA.ResetIOCountAndRate();
				terminalB.ResetIOCountAndRate();

				terminalA.StartIO();
				Utilities.WaitForConnection(terminalA, terminalB);
			}
			else
			{
				// Send repeating A >> B then close B:
				terminalA.SendText(@"A >> B\!(LineRepeat)");
				Thread.Sleep(333); // = approx. 30 lines.
				terminalB.StopIO();
				Utilities.WaitForDisconnection(terminalB);

				terminalB.ClearRepositories();
				terminalA.ClearRepositories();
				terminalB.ResetIOCountAndRate();
				terminalA.ResetIOCountAndRate();

				terminalB.StartIO();
				Utilities.WaitForConnection(terminalB, terminalA);
			}

			// Send subsequent A >> B:
			terminalA.SendText("A >> B");
			Utilities.WaitForTransmission(terminalA, terminalB, lineCountPerTransmission);

			// Send response B >> A:
			terminalB.SendText("B >> A");
			Utilities.WaitForTransmission(terminalB, terminalA, lineCountPerTransmission);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
