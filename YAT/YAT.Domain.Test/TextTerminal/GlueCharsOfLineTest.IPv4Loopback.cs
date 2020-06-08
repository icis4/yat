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
// YAT Version 2.1.1 Development
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

using System.Collections.Generic;
using System.Threading;

using MKY.Net.Test;

using NUnit.Framework;

#endregion

namespace YAT.Domain.Test.TextTerminal
{
	/// <summary></summary>
	[TestFixture]
	public class GlueCharsOfLineTestData_IPv4Loopback
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

			var settingsA = Utilities.GetTcpAutoSocketOnIPv4LoopbackTextSettings();
			settingsA.Display.ShowTimeStamp = true;
			settingsA.Display.ShowDirection = true;
			settingsA.Display.ShowLength    = true;
			settingsA.Display.ShowDuration  = true;

			using (var terminalA = new Domain.TextTerminal(settingsA))
			{
				Assert.That(terminalA.Start(), Is.True, "Terminal A could not be started");

				var settingsB = Utilities.GetTcpAutoSocketOnIPv4LoopbackTextSettings();
				settingsB.Display.ShowDirection = true;

				var gcol = settingsB.TextTerminal.GlueCharsOfLine;
				gcol.Enabled = false;
				settingsB.TextTerminal.GlueCharsOfLine = gcol;

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
					//           A >> B
					//           A << B
					terminalA.SendTextLine(text);
					expectedTotalByteCountAB += lineByteCount;
					expectedTotalLineCountAB++;
					Utilities.WaitForSendingAndVerifyCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
					Utilities.WaitForReceivingAndVerifyCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);

					terminalB.SendTextLine(text);
					expectedTotalByteCountBA += lineByteCount;
					expectedTotalLineCountBA++;
					Utilities.WaitForSendingAndVerifyCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
					Utilities.WaitForReceivingAndVerifyCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

					var contentPatternA = new List<string>();
					contentPatternA.Add("(" + Utilities.TimeStampRegexPattern + ") (<<) ABC<CR><LF> (5) (0.000)");
					contentPatternA.Add("(" + Utilities.TimeStampRegexPattern + ") (>>) ABC<CR><LF> (5) (0.000)");
					Utilities.VerifyContent(terminalA, contentPatternA);

					var contentPatternB = new List<string>();
					contentPatternB.Add("(>>) ABC<CR><LF>");
					contentPatternB.Add("(<<) ABC<CR><LF>");
					Utilities.VerifyContent(terminalB, contentPatternB);

					// Subsequent ping without EOL...
					//              A >> B
					terminalA.SendText(text);
					expectedTotalByteCountAB += textByteCount;
					expectedTotalLineCountAB++; // Line not completed though.
					Utilities.WaitForSendingAndVerifyCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
					Utilities.WaitForReceivingAndVerifyCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);

					contentPatternA.Add("(" + Utilities.TimeStampRegexPattern + ") (<<) ABC");
					Utilities.VerifyContent(terminalA, contentPatternA);

					contentPatternB.Add("(>>) ABC");
					Utilities.VerifyContent(terminalB, contentPatternB);

					// ...pong...
					//              A << B
					lineByteCount = (text.Length + eolByteCount);
					terminalB.SendTextLine(text); // Line from B must be postponed until ping completes with EOL.
					expectedTotalByteCountBA += lineByteCount;
					expectedTotalLineCountBA++;
					Utilities.WaitForSendingAndVerifyCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
					Utilities.WaitForReceivingAndVerifyCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

				////contentPatternA = same as before as line from B must be postponed until timeout.
					Utilities.VerifyContent(terminalA, contentPatternA);

					contentPatternB.Add("(<<) ABC<CR><LF>");
					Utilities.VerifyContent(terminalB, contentPatternB);

					// ...and complete ping with EOL:
					//              A >> B
					terminalA.SendTextLine("");
					expectedTotalByteCountAB += eolByteCount;
				////expectedTotalLineCountAB++; // Line already had started above.
					Utilities.WaitForSendingAndVerifyCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
					Utilities.WaitForReceivingAndVerifyCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);
					Utilities.WaitForSendingAndVerifyCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
					Utilities.WaitForReceivingAndVerifyCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

					var previousIndex = (contentPatternA.Count - 1); // Complete the previous line.
					contentPatternA[previousIndex] +=                                     "<CR><LF> (5) (" + Utilities.DurationRegexPattern + ")";
					contentPatternA.Add("(" + Utilities.TimeStampRegexPattern + ") (>>) ABC<CR><LF> (5) (0.000)");
					Utilities.VerifyContent(terminalA, contentPatternA);

					contentPatternB.Add("(>>) <CR><LF>");
					Utilities.VerifyContent(terminalB, contentPatternB);

					// In order to detect errenuos behavior of timeout, wait for twice the timeout before...
					Thread.Sleep(2 * settingsA.TextTerminal.GlueCharsOfLine.Timeout);

					// ...subsequent ping without EOL...
					//              A >> B
					terminalA.SendText(text);
					expectedTotalByteCountAB += textByteCount;
					expectedTotalLineCountAB++; // Line not completed though.
					Utilities.WaitForSendingAndVerifyCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
					Utilities.WaitForReceivingAndVerifyCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);

					contentPatternA.Add("(" + Utilities.TimeStampRegexPattern + ") (<<) ABC");
					Utilities.VerifyContent(terminalA, contentPatternA);

					contentPatternB.Add("(>>) ABC");
					Utilities.VerifyContent(terminalB, contentPatternB);

					// ...pong...
					//              A << B
					lineByteCount = (text.Length + eolByteCount);
					terminalB.SendTextLine(text); // Line from B must be postponed until timeout.
					expectedTotalByteCountBA += lineByteCount;
					expectedTotalLineCountBA++;
					Utilities.WaitForSendingAndVerifyCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
					Utilities.WaitForReceivingAndVerifyCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

				////contentPatternA = same as before as line from B must be postponed until timeout.
					Utilities.VerifyContent(terminalA, contentPatternA);

					contentPatternB.Add("(<<) ABC<CR><LF>");
					Utilities.VerifyContent(terminalB, contentPatternB);

					// ...and wait for timeout:
					Thread.Sleep(settingsA.TextTerminal.GlueCharsOfLine.Timeout); // No margin needed.

					previousIndex = (contentPatternA.Count - 1); // Complete the previous line.
					contentPatternA[previousIndex] +=                                             " (3) (" + Utilities.DurationRegexPattern + ")";
					contentPatternA.Add("(" + Utilities.TimeStampRegexPattern + ") (>>) ABC<CR><LF> (5) (0.000)");
					Utilities.VerifyContent(terminalA, contentPatternA);

				////contentPatternB = same as before.
					Utilities.VerifyContent(terminalB, contentPatternB);

					// Refresh and verify again:
					terminalA.RefreshRepositories();
					terminalB.RefreshRepositories();
					Utilities.VerifyContent(terminalA, contentPatternA);
					Utilities.VerifyContent(terminalB, contentPatternB);

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
			settingsA.Display.ShowTimeStamp = true;
			settingsA.Display.ShowDirection = true;
			settingsA.Display.ShowDuration = true;

			var gcol = settingsA.TextTerminal.GlueCharsOfLine;
			gcol.Timeout = Timeout.Infinite;
			settingsA.TextTerminal.GlueCharsOfLine = gcol;

			using (var terminalA = new Domain.TextTerminal(settingsA))
			{
				Assert.That(terminalA.Start(), Is.True, "Terminal A could not be started");

				var settingsB = Utilities.GetTcpAutoSocketOnIPv4LoopbackTextSettings();
				using (var terminalB = new Domain.TextTerminal(settingsB))
				{
					Assert.That(terminalB.Start(), Is.True, "Terminal B could not be started");
					Utilities.WaitForConnection(terminalA, terminalB);

					// PENDING

					// Refresh and verify again:
					// PENDING

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
