//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2009 Matthias Kläy.
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

		[Test]
		public void TestClearCompleteLine()
		{
			// Create terminals from settings and check whether B receives from A
			using (Terminal terminalA = new Terminal(Utilities.GetTextTCPSettings()))
			{
				using (Terminal terminalB = new Terminal(Utilities.GetTextTCPSettings()))
				{
					Utilities.TestSet testSet;

					// Start and open terminals
					terminalA.Start();
					terminalB.Start();
					Utilities.WaitForConnection(terminalA, terminalB);

					// Create test set to verify transmission
					testSet = new Utilities.TestSet(new Types.Command(@"A"), 1, new int[] { 2 }, new int[] { 1 }); // EOL results in one more element

					// Send test command
					terminalA.SendCommand(testSet.Command);
					Utilities.WaitForTransmission(terminalA, terminalB);

					// Verify transmission
					Utilities.VerifyLines(terminalA.RepositoryToDisplayLines(Domain.RepositoryType.Tx),
										  terminalB.RepositoryToDisplayLines(Domain.RepositoryType.Rx),
										  testSet);

					// Create test set to verify clear
					testSet = new Utilities.TestSet(new Types.Command(@""), 0, new int[] { 0 }, new int[] { 0 }); // Empty terminals expected

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

		[Test]
		public void TestClearIncompleteLine()
		{
			// Create terminals from settings and check whether B receives from A
			using (Terminal terminalA = new Terminal(Utilities.GetTextTCPSettings()))
			{
				using (Terminal terminalB = new Terminal(Utilities.GetTextTCPSettings()))
				{
					Utilities.TestSet testSet;
					List<Domain.DisplayLine> lines;

					// Start and open terminals
					terminalA.Start();
					terminalB.Start();
					Utilities.WaitForConnection(terminalA, terminalB);

					// Create test set to verify transmission
					testSet = new Utilities.TestSet(new Types.Command(@"A"), 1, new int[] { 2 }, new int[] { 1 }); // EOL results in one more element

					// Send test command
					terminalA.SendCommand(testSet.Command);
					Utilities.WaitForTransmission(terminalA, terminalB);

					// Verify transmission
					Utilities.VerifyLines(terminalA.RepositoryToDisplayLines(Domain.RepositoryType.Tx),
										  terminalB.RepositoryToDisplayLines(Domain.RepositoryType.Rx),
										  testSet);

					// Send incomplete line command
					terminalA.SendCommand(new Types.Command(@"B\!(NoEOL)"));
					Utilities.WaitForTransmission(terminalA, terminalB);

					// Verify incomplete line
					lines = terminalB.RepositoryToDisplayLines(Domain.RepositoryType.Rx);
					if (lines.Count != 2)
						Assert.Fail("Incomplete line not received");

					// Create test set to verify clear
					testSet = new Utilities.TestSet(new Types.Command(@""), 0, new int[] { 0 }, new int[] { 0 }); // Empty terminals expected

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
