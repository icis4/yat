﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
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
// Copyright © 2007-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System.Collections.Generic;
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
			// Create temporary in-memory application settings for this test run.
			ApplicationSettings.Create(ApplicationSettingsFileAccess.None);

			// Prevent auto-save of workspace settings.
			ApplicationSettings.LocalUserSettings.General.AutoSaveWorkspace = false;
		}

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

		#region Tests > TestClearCompleteLine
		//------------------------------------------------------------------------------------------
		// Tests > TestClearCompleteLine
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to verify.")]
		[Test]
		public virtual void TestClearCompleteLine()
		{
			// Create terminals from settings and check whether B receives from A
			using (Terminal terminalA = new Terminal(Utilities.GetStartedTextTcpAutoSocketOnIPv4LoopbackSettings()))
			{
				using (Terminal terminalB = new Terminal(Utilities.GetStartedTextTcpAutoSocketOnIPv4LoopbackSettings()))
				{
					Utilities.TestSet testSet;

					// Start and open terminals:
					Assert.IsTrue(terminalA.Start(), @"Failed to start """ + terminalA.Caption + @"""");
					Assert.IsTrue(terminalB.Start(), @"Failed to start """ + terminalB.Caption + @"""");
					Utilities.WaitForConnection(terminalA, terminalB);

					// Create test set to verify transmission:
					testSet = new Utilities.TestSet(new Types.Command(@"A"), 1, new int[] { 2 }, new int[] { 1 }, true); // EOL results in one more element

					// Send test command:
					terminalA.SendText(testSet.Command);
					Utilities.WaitForTransmission(terminalA, terminalB, testSet);

					// Verify transmission:
					Utilities.VerifyLines(terminalA.RepositoryToDisplayLines(Domain.RepositoryType.Tx),
					                      terminalB.RepositoryToDisplayLines(Domain.RepositoryType.Rx),
					                      testSet);

					// Create test set to verify clear:
					testSet = new Utilities.TestSet(new Types.Command(@""), 0, new int[] { 0 }, new int[] { 0 }, true); // Empty terminals expected

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
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to verify.")]
		[Test]
		public virtual void TestClearIncompleteLine()
		{
			// Create terminals from settings and check whether B receives from A
			using (Terminal terminalA = new Terminal(Utilities.GetStartedTextTcpAutoSocketOnIPv4LoopbackSettings()))
			{
				using (Terminal terminalB = new Terminal(Utilities.GetStartedTextTcpAutoSocketOnIPv4LoopbackSettings()))
				{
					Utilities.TestSet testSet;

					// Start and open terminals:
					Assert.IsTrue(terminalA.Start(), @"Failed to start """ + terminalA.Caption + @"""");
					Assert.IsTrue(terminalB.Start(), @"Failed to start """ + terminalB.Caption + @"""");
					Utilities.WaitForConnection(terminalA, terminalB);

					// Create test set to verify transmission:
					testSet = new Utilities.TestSet(new Types.Command(@"A"), 1, new int[] { 2 }, new int[] { 1 }, true); // EOL results in one more element

					// Send test command:
					terminalA.SendText(testSet.Command);
					Utilities.WaitForTransmission(terminalA, terminalB, testSet);

					// Verify transmission:
					Utilities.VerifyLines(terminalA.RepositoryToDisplayLines(Domain.RepositoryType.Tx),
					                      terminalB.RepositoryToDisplayLines(Domain.RepositoryType.Rx),
					                      testSet);

					// Send incomplete line text:
					terminalA.SendText(new Types.Command(@"B\!(NoEOL)"));
					Utilities.WaitForTransmission(terminalA, terminalB, testSet);

					// Verify incomplete line:
					List<Domain.DisplayLine> lines = terminalB.RepositoryToDisplayLines(Domain.RepositoryType.Rx);
					if (lines.Count != 2)
						Assert.Fail("Incomplete line not received!");

					// Create test set to verify clear:
					testSet = new Utilities.TestSet(new Types.Command(@""), 0, new int[] { 0 }, new int[] { 0 }, true); // Empty terminals expected

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

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
