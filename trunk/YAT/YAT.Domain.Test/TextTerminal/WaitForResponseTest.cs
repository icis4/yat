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
	/// <summary></summary>
	[TestFixture]
	public class WaitForResponseTest
	{
		#region TestDefault
		//==========================================================================================
		// TestDefault
		//==========================================================================================

		/// <summary></summary>
		[Test] // Test is mandatory, it shall not be excludable. 'IPv4LoopbackIsAvailable' is probed below.
		public virtual void TestDefault()
		{
			if (!ConfigurationProvider.Configuration.IPv4LoopbackIsAvailable)
				Assert.Ignore("No IPv4 loopback is available, therefore this test is excluded. Ensure that IPv4 loopback is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			const int WaitTimeUntilTimeout = (int)(1.5 * Domain.Settings.TextTerminalSettings.WaitForResponseTimeoutDefault);

			var settingsA = Settings.GetTcpAutoSocketOnIPv4LoopbackSettings(TerminalType.Text);

			var wfr = settingsA.TextTerminal.WaitForResponse;
			wfr.Enabled = true; // For manual test execution, wfr.Timeout must be set to 5000 ms as break is involved.
			settingsA.TextTerminal.WaitForResponse = wfr;

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

							terminalA.SendTextLine(Text); // Line will be retained until time-out.
							Thread.Sleep(WaitTimeUntilTimeout); // Pending line of A will time out.
							expectedTotalByteCountAB += LineByteCount; // 15 bytes
							expectedTotalLineCountAB++;                // 3 lines
							Utilities.WaitForSendingAndAssertCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
							Utilities.WaitForReceivingAndAssertCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);
							Assert.That(terminalA.IsSendingForSomeTime, Is.False); // No need to WaitForIsNoLongerSending() as already waited for completion above.

							// Subsequent pong-pong...
							//              A <= B
							//              A <= B
							terminalB.SendTextLine(Text);
							terminalB.SendTextLine(Text); // No restrictions in this direction.
							expectedTotalByteCountBA += LineByteCount;
							expectedTotalByteCountBA += LineByteCount; // 15 bytes
							expectedTotalLineCountBA++;
							expectedTotalLineCountBA++;                // 3 lines
							Utilities.WaitForSendingAndAssertCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
							Utilities.WaitForReceivingAndAssertCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

							// ...ping-ping...
							//              A => B
							//              A => B
							terminalA.SendTextLine(Text);
							expectedTotalByteCountAB += LineByteCount; // 20 bytes
							expectedTotalLineCountAB++;                // 4 lines
							Utilities.WaitForSendingAndAssertCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
							Utilities.WaitForReceivingAndAssertCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);

							terminalA.SendTextLine(Text); // Line must be retained until time-out again, even in case of pong-pong.
							Thread.Sleep(WaitTimeUntilTimeout); // Pending line of A will time out.
							expectedTotalByteCountAB += LineByteCount; // 25 bytes
							expectedTotalLineCountAB++;                // 5 lines
							Utilities.WaitForSendingAndAssertCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
							Utilities.WaitForReceivingAndAssertCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);
							Assert.That(terminalA.IsSendingForSomeTime, Is.False); // No need to WaitForIsNoLongerSending() as already waited for completion above.

							// Pong to reset clearance to 1 again:
							//              A <= B
							terminalB.SendTextLine(Text);
							expectedTotalByteCountBA += LineByteCount; // 20 bytes
							expectedTotalLineCountBA++;                // 4 lines
							Utilities.WaitForSendingAndAssertCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
							Utilities.WaitForReceivingAndAssertCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

							// Subsequent ping-ping-ping...
							//              A => B
							//              A => B
							//              A => B
							terminalA.SendTextLine(Text);
							expectedTotalByteCountAB += LineByteCount; // 30 bytes
							expectedTotalLineCountAB++;                // 6 lines
							Utilities.WaitForSendingAndAssertCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
							Utilities.WaitForReceivingAndAssertCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);

							terminalA.SendTextLine(Text); // Line must be retained until time-out.
							terminalA.SendTextLine(Text); // Line must be retained until time-out.

							// ...pong the 1st pending...
							//              A <= B
							terminalB.SendTextLine(Text);
							expectedTotalByteCountBA += LineByteCount;
							expectedTotalLineCountBA++;
							Utilities.WaitForSendingAndAssertCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
							Utilities.WaitForReceivingAndAssertCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);
																	 //// 1st retained line of A will have been sent.
							expectedTotalByteCountAB += LineByteCount; // 35 bytes
							expectedTotalLineCountAB++;                // 7 lines
							Utilities.WaitForSendingAndAssertCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
							Utilities.WaitForReceivingAndAssertCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);

							// ...break the 2nd pending line...
							terminalA.ActivateBreak();
							Utilities.WaitForIsNoLongerSending(terminalA);

							// ...and then resume break by pinging again:
							terminalA.SendTextLine(Text);
							expectedTotalByteCountAB += LineByteCount; // 40 bytes
							expectedTotalLineCountAB++;                // 8 lines
							Utilities.WaitForSendingAndAssertCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
							Utilities.WaitForReceivingAndAssertCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);
							Assert.That(terminalA.IsSendingForSomeTime, Is.False); // No need to WaitForIsNoLongerSending() as already waited for completion above.

							// Wait to ensure that no operation is ongoing anymore and verify again:
							Utilities.WaitForReverification();
							Utilities.AssertTxCounts(terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
							Utilities.AssertRxCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);

							// Refresh and verify again:
							terminalA.RefreshRepositories();
							terminalB.RefreshRepositories();
							Utilities.AssertTxCounts(terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
							Utilities.AssertRxCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);
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

		#region TestInfiniteTimeout
		//==========================================================================================
		// TestInfiniteTimeout
		//==========================================================================================

		/// <summary></summary>
		[Test] // Test is mandatory, it shall not be excludable. 'IPv4LoopbackIsAvailable' is probed below.
		public virtual void TestInfiniteTimeout()
		{
			if (!ConfigurationProvider.Configuration.IPv4LoopbackIsAvailable)
				Assert.Ignore("No IPv4 loopback is available, therefore this test is excluded. Ensure that IPv4 loopback is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			var settingsA = Settings.GetTcpAutoSocketOnIPv4LoopbackSettings(TerminalType.Text);

			var wfr = settingsA.TextTerminal.WaitForResponse;
			wfr.Enabled = true;
			wfr.Timeout = Timeout.Infinite;
			settingsA.TextTerminal.WaitForResponse = wfr;

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

							terminalA.SendTextLine(Text); // Line will infinitly be retained.
							Utilities.WaitForIsSendingForSomeTime(terminalA);

							// ...and pong:
							//              A <= B
							terminalB.SendTextLine(Text);
							expectedTotalByteCountBA += LineByteCount; // 10 bytes
							expectedTotalLineCountBA++;                // 2 lines
							Utilities.WaitForSendingAndAssertCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
							Utilities.WaitForReceivingAndAssertCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);
																	 //// Retained line of A will have been sent.
							expectedTotalByteCountAB += LineByteCount; // 15 bytes
							expectedTotalLineCountAB++;                // 3 lines
							Utilities.WaitForSendingAndAssertCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
							Utilities.WaitForReceivingAndAssertCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);
							Assert.That(terminalA.IsSendingForSomeTime, Is.False); // No need to WaitForIsNoLongerSending() as already waited for completion above.

							// Subsequent pong-pong...
							//              A <= B
							//              A <= B
							terminalB.SendTextLine(Text);
							terminalB.SendTextLine(Text); // No restrictions in this direction.
							expectedTotalByteCountBA += LineByteCount; // 15 bytes
							expectedTotalByteCountBA += LineByteCount; // 20 bytes
							expectedTotalLineCountBA++;                // 3 lines
							expectedTotalLineCountBA++;                // 4 lines
							Utilities.WaitForSendingAndAssertCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
							Utilities.WaitForReceivingAndAssertCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

							// ...ping-ping...
							//              A => B
							//              A => B
							terminalA.SendTextLine(Text);
							expectedTotalByteCountAB += LineByteCount; // 20 bytes
							expectedTotalLineCountAB++;                // 4 lines
							Utilities.WaitForSendingAndAssertCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
							Utilities.WaitForReceivingAndAssertCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);

							terminalA.SendTextLine(Text); // Line must infinitly be retained again, even in case of pong-pong.
							Utilities.WaitForIsSendingForSomeTime(terminalA);

							// ...and pong:
							//              A <= B
							terminalB.SendTextLine(Text);
							expectedTotalByteCountBA += LineByteCount; // 25 bytes
							expectedTotalLineCountBA++;                // 5 lines
							Utilities.WaitForSendingAndAssertCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
							Utilities.WaitForReceivingAndAssertCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);
																	 //// Retained line of A will have been sent.
							expectedTotalByteCountAB += LineByteCount; // 25 bytes
							expectedTotalLineCountAB++;                // 5 lines
							Utilities.WaitForSendingAndAssertCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
							Utilities.WaitForReceivingAndAssertCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);
							Assert.That(terminalA.IsSendingForSomeTime, Is.False); // No need to WaitForIsNoLongerSending() as already waited for completion above.

							// Pong to reset clearance to 1 again:
							//              A <= B
							terminalB.SendTextLine(Text);
							expectedTotalByteCountBA += LineByteCount; // 30 bytes
							expectedTotalLineCountBA++;                // 6 lines
							Utilities.WaitForSendingAndAssertCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
							Utilities.WaitForReceivingAndAssertCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

							// Subsequent ping-ping-ping...
							//              A => B
							//              A => B
							//              A => B
							terminalA.SendTextLine(Text);
							expectedTotalByteCountAB += LineByteCount; // 30 bytes
							expectedTotalLineCountAB++;                // 6 lines
							Utilities.WaitForSendingAndAssertCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
							Utilities.WaitForReceivingAndAssertCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);

							terminalA.SendTextLine(Text); // Line must infinitly be retained.
							terminalA.SendTextLine(Text); // Line must infinitly be retained.
							Utilities.WaitForIsSendingForSomeTime(terminalA);

							// ...pong the 1st pending...
							//              A <= B
							terminalB.SendTextLine(Text);
							expectedTotalByteCountBA += LineByteCount; // 35 bytes
							expectedTotalLineCountBA++;                // 7 lines
							Utilities.WaitForSendingAndAssertCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
							Utilities.WaitForReceivingAndAssertCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);
																	 //// 1st retained line of A will have been sent.
							expectedTotalByteCountAB += LineByteCount; // 35 bytes
							expectedTotalLineCountAB++;                // 7 lines
							Utilities.WaitForSendingAndAssertCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
							Utilities.WaitForReceivingAndAssertCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);
							Assert.That(terminalA.IsSendingForSomeTime, Is.True); // No need to WaitForIsSendingForSomeTime() as already waited for completion above.

							// ...break the 2nd pending line...
							terminalA.ActivateBreak();
							Utilities.WaitForIsNoLongerSending(terminalA);

							// ...and then resume break by pinging again:
							terminalA.SendTextLine(Text);
							expectedTotalByteCountAB += LineByteCount; // 40 bytes
							expectedTotalLineCountAB++;                // 8 lines
							Utilities.WaitForSendingAndAssertCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
							Utilities.WaitForReceivingAndAssertCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);
							Assert.That(terminalA.IsSendingForSomeTime, Is.False); // No need to WaitForIsNoLongerSending() as already waited for completion above.

							// Refresh and verify again:
							terminalA.RefreshRepositories();
							terminalB.RefreshRepositories();
							Utilities.AssertTxCounts(terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
							Utilities.AssertRxCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);
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
