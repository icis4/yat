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
// YAT Version 2.2.0 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2020 Matthias Kläy.
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

			const int WaitTimeUntilTimeout = (int)(1.5 * Settings.TextTerminalSettings.WaitForResponseTimeoutDefault);

			var settingsA = Utilities.GetTcpAutoSocketOnIPv4LoopbackTextSettings();

			var wfr = settingsA.TextTerminal.WaitForResponse;
			wfr.Enabled = true;
			settingsA.TextTerminal.WaitForResponse = wfr;

			using (var terminalA = new Domain.TextTerminal(settingsA))
			{
				Assert.That(terminalA.Start(), Is.True, "Terminal A could not be started");

				var settingsB = Utilities.GetTcpAutoSocketOnIPv4LoopbackTextSettings();
				using (var terminalB = new Domain.TextTerminal(settingsB))
				{
					Assert.That(terminalB.Start(), Is.True, "Terminal B could not be started");
					Utilities.WaitForConnection(terminalA, terminalB);

					string text = "ABC";
					int textByteCount = text.Length;
					int eolByteCount = 2; // Fixed to default of <CR><LF>.
					int lineByteCount = (textByteCount + eolByteCount);
					int expectedTotalByteCountAB = 0;
					int expectedTotalByteCountBA = 0;
					int expectedTotalLineCountAB = 0;
					int expectedTotalLineCountBA = 0;

					// Initial ping-pong:
					//           A => B
					//           A <= B
					terminalA.SendTextLine(text);
					terminalB.SendTextLine(text);
					expectedTotalByteCountAB += lineByteCount;
					expectedTotalByteCountBA += lineByteCount;
					expectedTotalLineCountAB++;
					expectedTotalLineCountBA++;                           // Yet symmetrical, a single verification is yet sufficient.
					Utilities.WaitForTransmissionAndVerifyCounts(terminalA, terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);

					// Subsequent ping-ping...
					//              A => B
					//              A => B
					terminalA.SendTextLine(text);
					expectedTotalByteCountAB += lineByteCount;
					expectedTotalLineCountAB++;
					Utilities.WaitForSendingAndVerifyCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
					Utilities.WaitForReceivingAndVerifyCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);

					terminalA.SendTextLine(text); // Line will be retained until timeout.
					Thread.Sleep(WaitTimeUntilTimeout);
					expectedTotalByteCountAB += lineByteCount; // Pending line of A will have timed out.
					expectedTotalLineCountAB++;
					Utilities.WaitForSendingAndVerifyCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
					Utilities.WaitForReceivingAndVerifyCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);
					Assert.That(terminalA.IsSendingForSomeTime, Is.False); // No need to WaitForIsNoLongerSending() as already waited for completion above.

					// Subsequent pong-pong...
					//              A <= B
					//              A <= B
					terminalB.SendTextLine(text);
					terminalB.SendTextLine(text); // No restrictions in this direction.
					expectedTotalByteCountBA += lineByteCount;
					expectedTotalByteCountBA += lineByteCount;
					expectedTotalLineCountBA++;
					expectedTotalLineCountBA++;
					Utilities.WaitForSendingAndVerifyCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
					Utilities.WaitForReceivingAndVerifyCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

					// ...ping-ping...
					//              A => B
					//              A => B
					terminalA.SendTextLine(text);
					expectedTotalByteCountAB += lineByteCount;
					expectedTotalLineCountAB++;
					Utilities.WaitForSendingAndVerifyCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
					Utilities.WaitForReceivingAndVerifyCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);

					terminalA.SendTextLine(text); // Line must be retained until timeout again, even in case of pong-pong.
					Thread.Sleep(WaitTimeUntilTimeout);
					expectedTotalByteCountAB += lineByteCount; // Pending line of A will have timed out.
					expectedTotalLineCountAB++;
					Utilities.WaitForSendingAndVerifyCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
					Utilities.WaitForReceivingAndVerifyCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);
					Assert.That(terminalA.IsSendingForSomeTime, Is.False); // No need to WaitForIsNoLongerSending() as already waited for completion above.

					// Pong to reset clearance to 1 again:
					//              A <= B
					terminalB.SendTextLine(text);
					expectedTotalByteCountBA += lineByteCount;
					expectedTotalLineCountBA++;
					Utilities.WaitForSendingAndVerifyCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
					Utilities.WaitForReceivingAndVerifyCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

					// Subsequent ping-ping-ping...
					//              A => B
					//              A => B
					//              A => B
					terminalA.SendTextLine(text);
					expectedTotalByteCountAB += lineByteCount;
					expectedTotalLineCountAB++;
					Utilities.WaitForSendingAndVerifyCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
					Utilities.WaitForReceivingAndVerifyCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);

					terminalA.SendTextLine(text); // Line must be retained until timeout.
					terminalA.SendTextLine(text); // Line must be retained until timeout.

					// ...pong the 1st pending...
					//              A <= B
					terminalB.SendTextLine(text);
					expectedTotalByteCountBA += lineByteCount;
					expectedTotalLineCountBA++;
					Utilities.WaitForSendingAndVerifyCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
					Utilities.WaitForReceivingAndVerifyCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

					expectedTotalByteCountAB += lineByteCount; // 1st retained line of A will be sent.
					expectedTotalLineCountAB++;
					Utilities.WaitForSendingAndVerifyCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
					Utilities.WaitForReceivingAndVerifyCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);

					// ...break the 2nd pending line...
					terminalA.Break();
					Utilities.WaitForIsNoLongerSending(terminalA);

					// ...and then resume break by pinging again:
					terminalA.SendTextLine(text);
					expectedTotalByteCountAB += lineByteCount;
					expectedTotalLineCountAB++;
					Utilities.WaitForSendingAndVerifyCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
					Utilities.WaitForReceivingAndVerifyCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);
					Assert.That(terminalA.IsSendingForSomeTime, Is.False); // No need to WaitForIsNoLongerSending() as already waited for completion above.

					// Refresh and verify again:
					terminalA.RefreshRepositories();
					terminalB.RefreshRepositories();
					Utilities.WaitForSendingAndVerifyCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
					Utilities.WaitForReceivingAndVerifyCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);

					terminalB.Stop();
					Utilities.WaitForDisconnection(terminalB);
				} // using (terminalB)

				terminalA.Stop();
				Utilities.WaitForDisconnection(terminalA);
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

			var settingsA = Utilities.GetTcpAutoSocketOnIPv4LoopbackTextSettings();

			var wfr = settingsA.TextTerminal.WaitForResponse;
			wfr.Enabled = true;
			wfr.Timeout = Timeout.Infinite;
			settingsA.TextTerminal.WaitForResponse = wfr;

			using (var terminalA = new Domain.TextTerminal(settingsA))
			{
				Assert.That(terminalA.Start(), Is.True, "Terminal A could not be started");

				var settingsB = Utilities.GetTcpAutoSocketOnIPv4LoopbackTextSettings();
				using (var terminalB = new Domain.TextTerminal(settingsB))
				{
					Assert.That(terminalB.Start(), Is.True, "Terminal B could not be started");
					Utilities.WaitForConnection(terminalA, terminalB);

					string text = "ABC";
					int textByteCount = text.Length;
					int eolByteCount = 2; // Fixed to default of <CR><LF>.
					int lineByteCount = (textByteCount + eolByteCount);
					int expectedTotalByteCountAB = 0;
					int expectedTotalByteCountBA = 0;
					int expectedTotalLineCountAB = 0;
					int expectedTotalLineCountBA = 0;

					// Initial ping-pong:
					//           A => B
					//           A <= B
					terminalA.SendTextLine(text);
					terminalB.SendTextLine(text);
					expectedTotalByteCountAB += lineByteCount;
					expectedTotalByteCountBA += lineByteCount;
					expectedTotalLineCountAB++;
					expectedTotalLineCountBA++;                           // Yet symmetrical, a single verification is yet sufficient.
					Utilities.WaitForTransmissionAndVerifyCounts(terminalA, terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);

					// Subsequent ping-ping...
					//              A => B
					//              A => B
					terminalA.SendTextLine(text);
					expectedTotalByteCountAB += lineByteCount;
					expectedTotalLineCountAB++;
					Utilities.WaitForSendingAndVerifyCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
					Utilities.WaitForReceivingAndVerifyCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);

					terminalA.SendTextLine(text); // Line will infinitly be retained.
					Utilities.WaitForIsSendingForSomeTime(terminalA);

					// ...and pong:
					//              A <= B
					terminalB.SendTextLine(text);
					expectedTotalByteCountBA += lineByteCount;
					expectedTotalLineCountBA++;
					Utilities.WaitForSendingAndVerifyCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
					Utilities.WaitForReceivingAndVerifyCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

					expectedTotalByteCountAB += lineByteCount; // Pending line of A will be sent.
					expectedTotalLineCountAB++;
					Utilities.WaitForSendingAndVerifyCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
					Utilities.WaitForReceivingAndVerifyCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);
					Assert.That(terminalA.IsSendingForSomeTime, Is.False); // No need to WaitForIsNoLongerSending() as already waited for completion above.

					// Subsequent pong-pong...
					//              A <= B
					//              A <= B
					terminalB.SendTextLine(text);
					terminalB.SendTextLine(text); // No restrictions in this direction.
					expectedTotalByteCountBA += lineByteCount;
					expectedTotalByteCountBA += lineByteCount;
					expectedTotalLineCountBA++;
					expectedTotalLineCountBA++;
					Utilities.WaitForSendingAndVerifyCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
					Utilities.WaitForReceivingAndVerifyCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

					// ...ping-ping...
					//              A => B
					//              A => B
					terminalA.SendTextLine(text);
					expectedTotalByteCountAB += lineByteCount;
					expectedTotalLineCountAB++;
					Utilities.WaitForSendingAndVerifyCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
					Utilities.WaitForReceivingAndVerifyCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);

					terminalA.SendTextLine(text); // Line must infinitly be retained again, even in case of pong-pong.
					Utilities.WaitForIsSendingForSomeTime(terminalA);

					// ...and pong:
					//              A <= B
					terminalB.SendTextLine(text);
					expectedTotalByteCountBA += lineByteCount;
					expectedTotalLineCountBA++;
					Utilities.WaitForSendingAndVerifyCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
					Utilities.WaitForReceivingAndVerifyCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

					expectedTotalByteCountAB += lineByteCount; // Pending line of A will be sent.
					expectedTotalLineCountAB++;
					Utilities.WaitForSendingAndVerifyCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
					Utilities.WaitForReceivingAndVerifyCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);
					Assert.That(terminalA.IsSendingForSomeTime, Is.False); // No need to WaitForIsNoLongerSending() as already waited for completion above.

					// Pong to reset clearance to 1 again:
					//              A <= B
					terminalB.SendTextLine(text);
					expectedTotalByteCountBA += lineByteCount;
					expectedTotalLineCountBA++;
					Utilities.WaitForSendingAndVerifyCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
					Utilities.WaitForReceivingAndVerifyCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

					// Subsequent ping-ping-ping...
					//              A => B
					//              A => B
					//              A => B
					terminalA.SendTextLine(text);
					expectedTotalByteCountAB += lineByteCount;
					expectedTotalLineCountAB++;
					Utilities.WaitForSendingAndVerifyCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
					Utilities.WaitForReceivingAndVerifyCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);

					terminalA.SendTextLine(text); // Line must infinitly be retained.
					terminalA.SendTextLine(text); // Line must infinitly be retained.
					Utilities.WaitForIsSendingForSomeTime(terminalA);

					// ...pong the 1st pending...
					//              A <= B
					terminalB.SendTextLine(text);
					expectedTotalByteCountBA += lineByteCount;
					expectedTotalLineCountBA++;
					Utilities.WaitForSendingAndVerifyCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
					Utilities.WaitForReceivingAndVerifyCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

					expectedTotalByteCountAB += lineByteCount; // 1st retained line of A will be sent.
					expectedTotalLineCountAB++;
					Utilities.WaitForSendingAndVerifyCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
					Utilities.WaitForReceivingAndVerifyCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);
					Assert.That(terminalA.IsSendingForSomeTime, Is.True); // No need to WaitForIsSendingForSomeTime() as already waited for completion above.

					// ...break the 2nd pending line...
					terminalA.Break();
					Utilities.WaitForIsNoLongerSending(terminalA);

					// ...and then resume break by pinging again:
					terminalA.SendTextLine(text);
					expectedTotalByteCountAB += lineByteCount;
					expectedTotalLineCountAB++;
					Utilities.WaitForSendingAndVerifyCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
					Utilities.WaitForReceivingAndVerifyCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);
					Assert.That(terminalA.IsSendingForSomeTime, Is.False); // No need to WaitForIsNoLongerSending() as already waited for completion above.

					// Refresh and verify again:
					terminalA.RefreshRepositories();
					terminalB.RefreshRepositories();
					Utilities.WaitForSendingAndVerifyCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
					Utilities.WaitForReceivingAndVerifyCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);

					terminalB.Stop();
					Utilities.WaitForDisconnection(terminalB);
				} // using (terminalB)

				terminalA.Stop();
				Utilities.WaitForDisconnection(terminalA);
			} // using (terminalA)
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
