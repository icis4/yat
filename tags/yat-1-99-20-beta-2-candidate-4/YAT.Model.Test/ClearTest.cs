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
			// create terminals from settings and check whether B receives from A
			using (Terminal terminalA = new Terminal(Utilities.GetTextTCPSettings()))
			{
				using (Terminal terminalB = new Terminal(Utilities.GetTextTCPSettings()))
				{
					Utilities.TestSet testSet;

					// start and open terminals
					terminalA.Start();
					terminalB.Start();
					Utilities.WaitForConnection(terminalA, terminalB);

					// create test set to verify transmission
					testSet = new Utilities.TestSet(new Types.Command(@"A"), 1, new int[] { 2 }); // EOL results in one more element

					// send test command
					terminalA.SendCommand(testSet.Command);
					Utilities.WaitForTransmission(terminalA, terminalB);

					// verify transmission
					Utilities.VerifyLines(terminalA.RepositoryToDisplayLines(Domain.RepositoryType.Tx),
										  terminalB.RepositoryToDisplayLines(Domain.RepositoryType.Rx),
										  testSet);

					// create test set to verify clear
					testSet = new Utilities.TestSet(new Types.Command(@""), 0, new int[] { 0 }); // Empty terminals expected

					// clear data
					terminalA.ClearRepositories();
					terminalB.ClearRepositories();

					// verify clear
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
			// create terminals from settings and check whether B receives from A
			using (Terminal terminalA = new Terminal(Utilities.GetTextTCPSettings()))
			{
				using (Terminal terminalB = new Terminal(Utilities.GetTextTCPSettings()))
				{
					Utilities.TestSet testSet;
					List<List<Domain.DisplayElement>> lines;

					// start and open terminals
					terminalA.Start();
					terminalB.Start();
					Utilities.WaitForConnection(terminalA, terminalB);

					// create test set to verify transmission
					testSet = new Utilities.TestSet(new Types.Command(@"A"), 1, new int[] { 2 }); // EOL results in one more element

					// send test command
					terminalA.SendCommand(testSet.Command);
					Utilities.WaitForTransmission(terminalA, terminalB);

					// verify transmission
					Utilities.VerifyLines(terminalA.RepositoryToDisplayLines(Domain.RepositoryType.Tx),
										  terminalB.RepositoryToDisplayLines(Domain.RepositoryType.Rx),
										  testSet);

					// send incomplete line command
					terminalA.SendCommand(new Types.Command(@"B\!(NoEOL)"));
					Utilities.WaitForTransmission(terminalA, terminalB);

					// verify incomplete line
					lines = terminalB.RepositoryToDisplayLines(Domain.RepositoryType.Rx);
					if (lines.Count != 2)
						Assert.Fail("Incomplete line not received");

					// create test set to verify clear
					testSet = new Utilities.TestSet(new Types.Command(@""), 0, new int[] { 0 }); // Empty terminals expected

					// clear data
					terminalA.ClearRepositories();
					terminalB.ClearRepositories();

					// verify clear
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
