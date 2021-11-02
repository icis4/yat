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
// YAT Version 2.4.1
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System.Threading;

using MKY.Net.Test;

using NUnit.Framework;

#endregion

namespace YAT.Domain.Test.TextTerminal
{
	/// <remarks>
	/// <see cref="TestDefault"/> and <see cref="TestInfiniteTimeout"/> are 90% equal, but do have
	/// some significant diff. Thus, decided to compy-paste implement twice rather than having to
	/// implement obscure logic. Still, separated into partial class files to ease diffing.
	/// </remarks>
	[TestFixture]
	public partial class WaitForResponseTest
	{
		#region TestOpenClose
		//==========================================================================================
		// TestOpenClose
		//==========================================================================================

		/// <summary></summary>
		[Test] // Test is mandatory, it shall not be excludable. 'IPv4LoopbackIsAvailable' is probed below.
		public virtual void TestOpenClose()
		{
			if (!ConfigurationProvider.Configuration.IPv4LoopbackIsAvailable)
				Assert.Ignore("No IPv4 loopback is available, therefore this test is excluded. Ensure that IPv4 loopback is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			var settingsA = Settings.GetTcpAutoSocketOnIPv4LoopbackSettings(TerminalType.Text);

			var wfr = settingsA.TextTerminal.WaitForResponse;
			wfr.Enabled = true;
			wfr.Timeout = Timeout.Infinite;
			settingsA.TextTerminal.WaitForResponse = wfr;

			// Depending on timing, [Warning: 5 bytes not sent anymore due to break.] may be indicated
			// on a separate line by "DisplayRepository.ToLines()" (see bug #352), or the same line.
			// This suboptimality does not allow for reliable "expectedTotalLineCountAB", thus:
			settingsA.Display.IncludeIOWarnings = false;

			using (var terminalA = TerminalFactory.CreateTerminal(settingsA))
			{
				try
				{
					Assert.That(terminalA.Start(), Is.True, "Terminal A could not be started!");

					var settingsB = Settings.GetTcpAutoSocketOnIPv4LoopbackSettings(TerminalType.Text);
					using (var terminalB = TerminalFactory.CreateTerminal(settingsB))
					{
						try
						{
							Assert.That(terminalB.Start(), Is.True, "Terminal B could not be started!");
							Utilities.WaitForConnection(terminalA, terminalB);

							const string Text = "ABC";
							const int TextByteCount = 3; // Fixed to "ABC".
							const int EolByteCount = 2; // Fixed to default of <CR><LF>.
							const int LineByteCount = (TextByteCount + EolByteCount);
							int expectedTotalByteCountAB = 0;
							int expectedTotalByteCountBA = 0;
							int expectedTotalLineCountAB = 0;
							int expectedTotalLineCountBA = 0;

							// Initial ping-pong:
							//           A => B
							//           A <= B
							terminalA.SendTextLine(Text);
							terminalB.SendTextLine(Text);
							expectedTotalByteCountAB += LineByteCount; // 5 bytes
							expectedTotalByteCountBA += LineByteCount; // 5 bytes
							expectedTotalLineCountAB++;                // 1 lines
							expectedTotalLineCountBA++;                // 1 lines  // Yet symmetrical, a single verification is yet sufficient.
							Utilities.WaitForTransmissionAndAssertCounts(terminalA, terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);

							// Subsequent ping-ping...
							//              A => B
							//              A => B
							terminalA.SendTextLine(Text);
							expectedTotalByteCountAB += LineByteCount; // 10 bytes
							expectedTotalLineCountAB++;                // 2 lines
							Utilities.WaitForSendingAndAssertCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
							Utilities.WaitForReceivingAndAssertCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);

							terminalA.SendTextLine(Text); // 2nd line will infinitely be retained.
							Utilities.WaitForIsSendingForSomeTime(terminalA);

							// ...then stop/restart terminal to reset clearance
							terminalA.Stop();
							Utilities.WaitForStop(terminalA);
							Assert.That(terminalA.IsStarted, Is.False);
							Assert.That(terminalA.IsStopped, Is.True);
							Utilities.AssertTxCounts(terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
							Utilities.AssertRxCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

							terminalA.Start();
							Utilities.WaitForConnection(terminalA, terminalB);
							Assert.That(terminalA.IsStopped, Is.False);
							Assert.That(terminalA.IsStarted, Is.True);

							// Subsequent ping-ping...
							//              A => B
							//              A => B
							terminalA.SendTextLine(Text);
							expectedTotalByteCountAB += LineByteCount; // 15 bytes
							expectedTotalLineCountAB++;                // 3 lines
							Utilities.WaitForSendingAndAssertCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
							Utilities.WaitForReceivingAndAssertCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);

							terminalA.SendTextLine(Text); // 2nd line will infinitely be retained.
							Utilities.WaitForIsSendingForSomeTime(terminalA);
						}
						finally // Properly stop even in case of an exception, e.g. a failed assertion.
						{
							terminalB.Stop();
							Utilities.WaitForStop(terminalB);
						}
					} // using (terminalB)
				}
				finally // Properly stop even in case of an exception, e.g. a failed assertion.
				{
					terminalA.Stop();
					Utilities.WaitForStop(terminalA);
				}
			} // using (terminalA)
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
