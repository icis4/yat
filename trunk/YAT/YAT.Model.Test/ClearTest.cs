//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 2 Development Version 1.99.29
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using YAT.Settings.Terminal;

namespace YAT.Model.Test
{
	/// <summary></summary>
	[TestFixture]
	public class ClearTest
	{
		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		#region Tests > TestClearCompleteLine
		//------------------------------------------------------------------------------------------
		// Tests > TestClearCompleteLine
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestClearCompleteLine()
		{
			// Create terminals from settings and check whether B receives from A
			using (Terminal terminalA = new Terminal(Utilities.GetStartedTextTcpAutoSocketOnIPv4LoopbackSettings()))
			{
				using (Terminal terminalB = new Terminal(Utilities.GetStartedTextTcpAutoSocketOnIPv4LoopbackSettings()))
				{
					Utilities.TestSet testSet;

					// Start and open terminals
					terminalA.Start();
					terminalB.Start();
					Utilities.WaitForConnection(terminalA, terminalB);

					// Create test set to verify transmission
					testSet = new Utilities.TestSet(new Types.Command(@"A"), 1, new int[] { 2 }, new int[] { 1 }, true); // EOL results in one more element

					// Send test command
					terminalA.SendText(testSet.Command);
					Utilities.WaitForTransmission(terminalA, terminalB, testSet);

					// Verify transmission
					Utilities.VerifyLines(terminalA.RepositoryToDisplayLines(Domain.RepositoryType.Tx),
					                      terminalB.RepositoryToDisplayLines(Domain.RepositoryType.Rx),
					                      testSet);

					// Create test set to verify clear
					testSet = new Utilities.TestSet(new Types.Command(@""), 0, new int[] { 0 }, new int[] { 0 }, true); // Empty terminals expected

					// Clear data
					terminalA.ClearRepositories();
					terminalB.ClearRepositories();

					// Verify clear
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
		[Test]
		public virtual void TestClearIncompleteLine()
		{
			// Create terminals from settings and check whether B receives from A
			using (Terminal terminalA = new Terminal(Utilities.GetStartedTextTcpAutoSocketOnIPv4LoopbackSettings()))
			{
				using (Terminal terminalB = new Terminal(Utilities.GetStartedTextTcpAutoSocketOnIPv4LoopbackSettings()))
				{
					Utilities.TestSet testSet;
					List<Domain.DisplayLine> lines;

					// Start and open terminals
					terminalA.Start();
					terminalB.Start();
					Utilities.WaitForConnection(terminalA, terminalB);

					// Create test set to verify transmission
					testSet = new Utilities.TestSet(new Types.Command(@"A"), 1, new int[] { 2 }, new int[] { 1 }, true); // EOL results in one more element

					// Send test command
					terminalA.SendText(testSet.Command);
					Utilities.WaitForTransmission(terminalA, terminalB, testSet);

					// Verify transmission
					Utilities.VerifyLines(terminalA.RepositoryToDisplayLines(Domain.RepositoryType.Tx),
					                      terminalB.RepositoryToDisplayLines(Domain.RepositoryType.Rx),
					                      testSet);

					// Send incomplete line command
					terminalA.SendText(new Types.Command(@"B\!(NoEOL)"));
					Utilities.WaitForTransmission(terminalA, terminalB, testSet);

					// Verify incomplete line
					lines = terminalB.RepositoryToDisplayLines(Domain.RepositoryType.Rx);
					if (lines.Count != 2)
						Assert.Fail("Incomplete line not received!");

					// Create test set to verify clear
					testSet = new Utilities.TestSet(new Types.Command(@""), 0, new int[] { 0 }, new int[] { 0 }, true); // Empty terminals expected

					// clear data
					terminalA.ClearRepositories();
					terminalB.ClearRepositories();

					// Verify clear
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
