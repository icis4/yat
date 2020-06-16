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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

using MKY.Net.Test;

using NUnit.Framework;

#endregion

namespace YAT.Domain.Test.TextTerminal
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize partial manner of this test class.")]
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
					//           A => B
					//           A <= B
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

					var expectedContentPatternA = new List<string>(); // Applies to bidir only.
					expectedContentPatternA.Add("(" + Utilities.TimeStampRegexPattern + ") (<<) ABC<CR><LF> (5) (0.000)");
					expectedContentPatternA.Add("(" + Utilities.TimeStampRegexPattern + ") (>>) ABC<CR><LF> (5) (0.000)");
					Utilities.VerifyBidirContent(terminalA, expectedContentPatternA);

					var expectedContentPatternB = new List<string>(); // Applies to bidir only.
					expectedContentPatternB.Add("(>>) ABC<CR><LF>");
					expectedContentPatternB.Add("(<<) ABC<CR><LF>");
					Utilities.VerifyBidirContent(terminalB, expectedContentPatternB);

					for (int i = 0; i < 2; i++) // ...twice doing...
					{
						// Subsequent ping without EOL...
						//              A => B
						terminalA.SendText(text);
						expectedTotalByteCountAB += textByteCount;
						expectedTotalLineCountAB++; // Line not completed though.
						Utilities.WaitForSendingAndVerifyCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
						Utilities.WaitForReceivingAndVerifyCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);

						expectedContentPatternA.Add("(" + Utilities.TimeStampRegexPattern + ") (<<) ABC");
						Utilities.VerifyBidirContent(terminalA, expectedContentPatternA);

						expectedContentPatternB.Add("(>>) ABC");
						Utilities.VerifyBidirContent(terminalB, expectedContentPatternB);

						// ...pong...
						//              A <= B
						lineByteCount = (text.Length + eolByteCount);
						terminalB.SendTextLine(text); // Line from B must be postponed until ping completes with EOL.
						expectedTotalByteCountBA += lineByteCount;
						expectedTotalLineCountBA++;
						Utilities.WaitForSendingAndVerifyCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
						Utilities.WaitForReceivingAndVerifyCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

					////expectedContentPatternA = same as before as line from B must be postponed until timeout.
						Utilities.VerifyBidirContent(terminalA, expectedContentPatternA);

						expectedContentPatternB.Add("(<<) ABC<CR><LF>");
						Utilities.VerifyBidirContent(terminalB, expectedContentPatternB);

						// ...and complete ping with EOL:
						//              A => B
						terminalA.SendTextLine("");
						expectedTotalByteCountAB += eolByteCount;
					////expectedTotalLineCountAB++; // Line already had started above.
						Utilities.WaitForSendingAndVerifyCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
						Utilities.WaitForReceivingAndVerifyCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);
						Utilities.WaitForSendingAndVerifyCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
						Utilities.WaitForReceivingAndVerifyCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

						var previousIndex = (expectedContentPatternA.Count - 1); // Complete the previous line:
						expectedContentPatternA[previousIndex] +=                                     "<CR><LF> (5) (" + Utilities.DurationRegexPattern + ")";
						expectedContentPatternA.Add("(" + Utilities.TimeStampRegexPattern + ") (>>) ABC<CR><LF> (5) (0.000)");
						Utilities.VerifyBidirContent(terminalA, expectedContentPatternA);

						expectedContentPatternB.Add("(>>) <CR><LF>");
						Utilities.VerifyBidirContent(terminalB, expectedContentPatternB);
					}

					// In order to detect erroneous behavior of timeout, wait for twice the timeout before...
					Thread.Sleep(2 * settingsA.TextTerminal.GlueCharsOfLine.Timeout);

					for (int i = 0; i < 2; i++) // ...twice doing...
					{
						// ...subsequent ping without EOL...
						//              A => B
						terminalA.SendText(text);
						expectedTotalByteCountAB += textByteCount;

						if (i == 0) { // Account for the fact that non-completed line "ABC" will result in "ABCABC" (Tx) during second iteration.
							expectedTotalLineCountAB++; // Line not completed though.
						}
						Utilities.WaitForSendingAndVerifyCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
						Utilities.WaitForReceivingAndVerifyCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);

						expectedContentPatternA.Add("(" + Utilities.TimeStampRegexPattern + ") (<<) ABC");
						Utilities.VerifyBidirContent(terminalA, expectedContentPatternA);

						expectedContentPatternB.Add("(>>) ABC");
						Utilities.VerifyBidirContent(terminalB, expectedContentPatternB);

						// ...pong...
						//              A <= B
						lineByteCount = (text.Length + eolByteCount);
						terminalB.SendTextLine(text); // Line from B must be postponed until timeout.
						expectedTotalByteCountBA += lineByteCount;
						expectedTotalLineCountBA++;
						Utilities.WaitForSendingAndVerifyCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
						Utilities.WaitForReceivingAndVerifyCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

					////expectedContentPatternA = same as before as line from B must be postponed until timeout.
						Utilities.VerifyBidirContent(terminalA, expectedContentPatternA);

						expectedContentPatternB.Add("(<<) ABC<CR><LF>");
						Utilities.VerifyBidirContent(terminalB, expectedContentPatternB);

						// ...and wait for timeout:
						Thread.Sleep(settingsA.TextTerminal.GlueCharsOfLine.Timeout); // No margin needed.

						var previousIndex = (expectedContentPatternA.Count - 1); // Complete the previous line:
						expectedContentPatternA[previousIndex] +=                                             " (3) (" + Utilities.DurationRegexPattern + ")";
						expectedContentPatternA.Add("(" + Utilities.TimeStampRegexPattern + ") (>>) ABC<CR><LF> (5) (0.000)");
						Utilities.VerifyBidirContent(terminalA, expectedContentPatternA);

					////expectedContentPatternB = same as before.
						Utilities.VerifyBidirContent(terminalB, expectedContentPatternB);
					}

					// Refresh and verify again:
					terminalA.RefreshRepositories();
					terminalB.RefreshRepositories();
					Utilities.VerifyBidirContent(terminalA, expectedContentPatternA);
					Utilities.VerifyBidirContent(terminalB, expectedContentPatternB);

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
			settingsA.Display.ShowLength    = true;
			settingsA.Display.ShowDuration  = true;

			var gcol = settingsA.TextTerminal.GlueCharsOfLine;
			gcol.Timeout = Timeout.Infinite;
			settingsA.TextTerminal.GlueCharsOfLine = gcol;

			using (var terminalA = new Domain.TextTerminal(settingsA))
			{
				Assert.That(terminalA.Start(), Is.True, "Terminal A could not be started");

				var settingsB = Utilities.GetTcpAutoSocketOnIPv4LoopbackTextSettings();
				settingsB.Display.ShowDirection = true;

				gcol = settingsB.TextTerminal.GlueCharsOfLine;
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
					int previousIndex;

					// Initial ping-pong:
					//           A => B
					//           A <= B
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

					var expectedContentPatternA = new List<string>(); // Applies to bidir only.
					expectedContentPatternA.Add("(" + Utilities.TimeStampRegexPattern + ") (<<) ABC<CR><LF> (5) (0.000)");
					expectedContentPatternA.Add("(" + Utilities.TimeStampRegexPattern + ") (>>) ABC<CR><LF> (5) (0.000)");
					Utilities.VerifyBidirContent(terminalA, expectedContentPatternA);

					var expectedContentPatternB = new List<string>(); // Applies to bidir only.
					expectedContentPatternB.Add("(>>) ABC<CR><LF>");
					expectedContentPatternB.Add("(<<) ABC<CR><LF>");
					Utilities.VerifyBidirContent(terminalB, expectedContentPatternB);

					for (int i = 0; i < 2; i++) // ...twice doing...
					{
						// Subsequent ping without EOL...
						//              A => B
						terminalA.SendText(text);
						expectedTotalByteCountAB += textByteCount;
						expectedTotalLineCountAB++; // Line not completed though.
						Utilities.WaitForSendingAndVerifyCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
						Utilities.WaitForReceivingAndVerifyCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);

						expectedContentPatternA.Add("(" + Utilities.TimeStampRegexPattern + ") (<<) ABC");
						Utilities.VerifyBidirContent(terminalA, expectedContentPatternA);

						expectedContentPatternB.Add("(>>) ABC");
						Utilities.VerifyBidirContent(terminalB, expectedContentPatternB);

						// ...pong...
						//              A <= B
						lineByteCount = (text.Length + eolByteCount);
						terminalB.SendTextLine(text); // Line from B must be postponed until ping completes with EOL.
						expectedTotalByteCountBA += lineByteCount;
						expectedTotalLineCountBA++;
						Utilities.WaitForSendingAndVerifyCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
						Utilities.WaitForReceivingAndVerifyCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

					////expectedContentPatternA = same as before as line from B must be postponed until line from A has completed (timeout = forever).
						Utilities.VerifyBidirContent(terminalA, expectedContentPatternA);

						expectedContentPatternB.Add("(<<) ABC<CR><LF>");
						Utilities.VerifyBidirContent(terminalB, expectedContentPatternB);

						// ...and complete ping with EOL:
						//              A => B
						terminalA.SendTextLine("");
						expectedTotalByteCountAB += eolByteCount;
					////expectedTotalLineCountAB++; // Line already had started above.
						Utilities.WaitForSendingAndVerifyCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
						Utilities.WaitForReceivingAndVerifyCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);
						Utilities.WaitForSendingAndVerifyCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
						Utilities.WaitForReceivingAndVerifyCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

						previousIndex = (expectedContentPatternA.Count - 1); // Complete the previous line:
						expectedContentPatternA[previousIndex] +=                                     "<CR><LF> (5) (" + Utilities.DurationRegexPattern + ")";
						expectedContentPatternA.Add("(" + Utilities.TimeStampRegexPattern + ") (>>) ABC<CR><LF> (5) (0.000)");
						Utilities.VerifyBidirContent(terminalA, expectedContentPatternA);

						expectedContentPatternB.Add("(>>) <CR><LF>");
						Utilities.VerifyBidirContent(terminalB, expectedContentPatternB);
					}

					for (int i = 0; i < 2; i++) // Twice doing...
					{
						// ...subsequent ping without EOL...
						//              A => B
						terminalA.SendText(text);
						expectedTotalByteCountAB += textByteCount;

						if (i == 0) { // Account for the fact that non-completed line "ABC" will result in "ABCABC" (Tx) during second iteration.
							expectedTotalLineCountAB++; // Line not completed though.
						}
						Utilities.WaitForSendingAndVerifyCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
						Utilities.WaitForReceivingAndVerifyCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);

						if (i == 0) { // Account for the fact that non-completed line "ABC" will result in "ABCABC" (Bidir too) during second iteration.
							expectedContentPatternA.Add("(" + Utilities.TimeStampRegexPattern + ") (<<) ABC");
						}
						else {
							previousIndex = (expectedContentPatternA.Count - 1); // Add to the previous line:
							expectedContentPatternA[previousIndex] +=                                     "ABC";
						}
						Utilities.VerifyBidirContent(terminalA, expectedContentPatternA);

						expectedContentPatternB.Add("(>>) ABC");
						Utilities.VerifyBidirContent(terminalB, expectedContentPatternB);

						// ...pong...
						//              A <= B
						lineByteCount = (text.Length + eolByteCount);
						terminalB.SendTextLine(text); // Line from B must be postponed until line from A has completed (timeout = forever).
						expectedTotalByteCountBA += lineByteCount;
						expectedTotalLineCountBA++;
						Utilities.WaitForSendingAndVerifyCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
						Utilities.WaitForReceivingAndVerifyCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

					////expectedContentPatternA = same as before as line from B must be postponed until line from A has completed (timeout = forever).
						Utilities.VerifyBidirContent(terminalA, expectedContentPatternA);

						expectedContentPatternB.Add("(<<) ABC<CR><LF>");
						Utilities.VerifyBidirContent(terminalB, expectedContentPatternB);

						// ...and wait for at least twice the default timeout:
						Thread.Sleep(2 * Settings.TextTerminalSettings.GlueCharsOfLineTimeoutDefault); // No margin needed.

					////expectedContentPatternA = same as before as line from B must be postponed until line from A has completed (timeout = forever).
						Utilities.VerifyBidirContent(terminalA, expectedContentPatternA);

					////expectedContentPatternB = same as before.
						Utilities.VerifyBidirContent(terminalB, expectedContentPatternB);
					}

					// Only then complete the two pings with EOL:
					//              A => B
					terminalA.SendTextLine("");
					expectedTotalByteCountAB += eolByteCount;
				////expectedTotalLineCountAB++; // Line already had started above.

					Utilities.WaitForSendingAndVerifyCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
					Utilities.WaitForReceivingAndVerifyCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);
					Utilities.WaitForSendingAndVerifyCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
					Utilities.WaitForReceivingAndVerifyCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

					previousIndex = (expectedContentPatternA.Count - 1); // Complete the previous line:
					expectedContentPatternA[previousIndex] +=                                     "<CR><LF> (8) (" + Utilities.DurationRegexPattern + ")";
					expectedContentPatternA.Add("(" + Utilities.TimeStampRegexPattern + ") (>>) ABC<CR><LF> (5) (0.000)");
					expectedContentPatternA.Add("(" + Utilities.TimeStampRegexPattern + ") (>>) ABC<CR><LF> (5) (0.000)");
					Utilities.VerifyBidirContent(terminalA, expectedContentPatternA);

					expectedContentPatternB.Add("(>>) <CR><LF>");
					Utilities.VerifyBidirContent(terminalB, expectedContentPatternB);

					// Refresh and verify again:
					terminalA.RefreshRepositories();
					terminalB.RefreshRepositories();

					Utilities.WaitForSendingAndVerifyCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
					Utilities.WaitForReceivingAndVerifyCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);
					Utilities.WaitForSendingAndVerifyCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
					Utilities.WaitForReceivingAndVerifyCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

					Utilities.VerifyBidirContent(terminalA, expectedContentPatternA);
					Utilities.VerifyBidirContent(terminalB, expectedContentPatternB);

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
