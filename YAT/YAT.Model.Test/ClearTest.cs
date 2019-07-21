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
// YAT Version 2.0.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System.Diagnostics.CodeAnalysis;

using MKY.Settings;

using NUnit.Framework;

using YAT.Settings.Application;

#endregion

namespace YAT.Model.Test
{
	/// <summary></summary>
	[TestFixture]
	public class ClearTest
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

		#region Tests > TestClearCompleteLine
		//------------------------------------------------------------------------------------------
		// Tests > TestClearCompleteLine
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to verify.")]
		[Test]
		public virtual void TestClearCompleteLine()
		{
			// Create terminals from settings and check whether B receives from A:
			using (var terminalA = new Terminal(Utilities.GetStartedTcpAutoSocketOnIPv4LoopbackTextSettings()))
			{
				using (var terminalB = new Terminal(Utilities.GetStartedTcpAutoSocketOnIPv4LoopbackTextSettings()))
				{
					Utilities.TestSet testSet;

					// Start and open terminals:
					Assert.That(terminalA.Start(), Is.True, @"Failed to start """ + terminalA.Caption + @"""");
					Assert.That(terminalB.Start(), Is.True, @"Failed to start """ + terminalB.Caption + @"""");
					Utilities.WaitForConnection(terminalA, terminalB);

					// Create test set to verify transmission:                              // LineStart + EOL + LineBreak result in three more elements.
					testSet = new Utilities.TestSet(new Types.Command(@"A"), 1, new int[] { 4 }, new int[] { 3 }, true);

					// Send test command:
					terminalA.SendText(testSet.Command);
					Utilities.WaitForTransmissionAndVerifyCounts(terminalA, terminalB, testSet);

					// Verify transmission:
					Utilities.VerifyLines(terminalA.RepositoryToDisplayLines(Domain.RepositoryType.Tx),
					                      terminalB.RepositoryToDisplayLines(Domain.RepositoryType.Rx),
					                      testSet);

					// Create test set to verify clear:
					testSet = new Utilities.TestSet(new Types.Command(@""), 0, null, null, true); // Empty terminals expected.

					// Clear data:
					terminalA.ClearRepositories();
					terminalB.ClearRepositories();

					// Verify clear:
					Utilities.VerifyLines(terminalA.RepositoryToDisplayLines(Domain.RepositoryType.Tx),
					                      terminalB.RepositoryToDisplayLines(Domain.RepositoryType.Rx),
					                      testSet);
				}
			}
		}

		#endregion

		#region Tests > TestClearIncompleteLine
		//------------------------------------------------------------------------------------------
		// Tests > TestClearIncompleteLine
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to verify.")]
		[Test]
		public virtual void TestClearIncompleteLine()
		{
			// Create terminals from settings and check whether B receives from A:
			using (var terminalA = new Terminal(Utilities.GetStartedTcpAutoSocketOnIPv4LoopbackTextSettings()))
			{
				using (var terminalB = new Terminal(Utilities.GetStartedTcpAutoSocketOnIPv4LoopbackTextSettings()))
				{
					// Start and open terminals:
					Assert.That(terminalA.Start(), Is.True, @"Failed to start """ + terminalA.Caption + @"""");
					Assert.That(terminalB.Start(), Is.True, @"Failed to start """ + terminalB.Caption + @"""");
					Utilities.WaitForConnection(terminalA, terminalB);

					// Create test set to verify transmission:                                                    // LineStart + EOL + LineBreak result in three more elements.
					var testSetInitial   = new Utilities.TestSet(new Types.Command(@"A"),          1, new int[] { 4 },    new int[] { 3 },    true);
					var testSetContinued = new Utilities.TestSet(new Types.Command(@"B\!(NoEOL)"), 1, new int[] { 4, 2 }, new int[] { 3, 1 }, true);

					// Send test command:
					terminalA.SendText(testSetInitial.Command);
					Utilities.WaitForTransmissionAndVerifyCounts(terminalA, terminalB, testSetInitial);

					// Verify transmission:
					Utilities.VerifyLines(terminalA.RepositoryToDisplayLines(Domain.RepositoryType.Tx),
					                      terminalB.RepositoryToDisplayLines(Domain.RepositoryType.Rx),
					                      testSetInitial);

					// Send incomplete line text:
					terminalA.SendText(testSetContinued.Command);
					Utilities.WaitForTransmissionAndVerifyCounts(terminalA, terminalB, testSetContinued);

					// Verify incomplete line:
					Utilities.VerifyLines(terminalA.RepositoryToDisplayLines(Domain.RepositoryType.Tx),
					                      terminalB.RepositoryToDisplayLines(Domain.RepositoryType.Rx),
					                      testSetContinued);
					var lines = terminalB.RepositoryToDisplayLines(Domain.RepositoryType.Rx);
					if (lines.Count != 2)
						Assert.Fail("Incomplete line not received!");

					// Create test set to verify clear:
					var testSetCleared = new Utilities.TestSet(new Types.Command(@""), 0, null, null, true); // Empty terminals expected.

					// Clear data:
					terminalA.ClearRepositories();
					terminalB.ClearRepositories();

					// Verify clear:
					Utilities.VerifyLines(terminalA.RepositoryToDisplayLines(Domain.RepositoryType.Tx),
					                      terminalB.RepositoryToDisplayLines(Domain.RepositoryType.Rx),
					                      testSetCleared);
				}
			}
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
