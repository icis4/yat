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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

using MKY.Net.Test;

using NUnit.Framework;

#endregion

namespace YAT.Domain.Test.TextTerminal
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize partial manner of this item.")]
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

			var settingsA = Settings.GetTcpAutoSocketOnIPv4LoopbackSettings(TerminalType.Text);
			settingsA.Display.ShowTimeStamp = true;
			settingsA.Display.ShowDirection = true;
			settingsA.Display.ShowLength    = true;
			settingsA.Display.ShowDuration  = true;

			using (var terminalA = TerminalFactory.CreateTerminal(settingsA))
			{
				try
				{
					Assert.That(terminalA.Start(), Is.True, "Terminal A could not be started!");

					var settingsB = Settings.GetTcpAutoSocketOnIPv4LoopbackSettings(TerminalType.Text);
					settingsB.Display.ShowDirection = true;

					var gcolB = settingsB.TextTerminal.GlueCharsOfLine;
					gcolB.Enabled = false;
					settingsB.TextTerminal.GlueCharsOfLine = gcolB;

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
							expectedTotalByteCountAB += LineByteCount;
							expectedTotalLineCountAB++;
							Utilities.WaitForSendingAndAssertCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
							Utilities.WaitForReceivingAndAssertCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);

							terminalB.SendTextLine(Text);
							expectedTotalByteCountBA += LineByteCount;
							expectedTotalLineCountBA++;
							Utilities.WaitForSendingAndAssertCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
							Utilities.WaitForReceivingAndAssertCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

							var expectedContentPatternA = new List<string>(); // Applies to bidir only.
							expectedContentPatternA.Add(EscapeRegex("(") + Utilities.TimeStampRegexPattern + EscapeRegex(") (<<) ABC<CR><LF> (5) (0.000)"));
							expectedContentPatternA.Add(EscapeRegex("(") + Utilities.TimeStampRegexPattern + EscapeRegex(") (>>) ABC<CR><LF> (5) (0.000)"));
							Utilities.AssertBidirContentPattern(terminalA, expectedContentPatternA);

							var expectedContentPatternB = new List<string>(); // Applies to bidir only.
							expectedContentPatternB.Add(EscapeRegex("(>>) ABC<CR><LF>"));
							expectedContentPatternB.Add(EscapeRegex("(<<) ABC<CR><LF>"));
							Utilities.AssertBidirContentPattern(terminalB, expectedContentPatternB);

							for (int i = 0; i < 2; i++) // ...twice doing...
							{
								// Subsequent ping without EOL...
								//              A => B
								terminalA.SendText(Text);
								expectedTotalByteCountAB += TextByteCount;
								expectedTotalLineCountAB++; // Line not completed though.
								Utilities.WaitForSendingAndAssertCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
								Utilities.WaitForReceivingAndAssertCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);

								expectedContentPatternA.Add(EscapeRegex("(") + Utilities.TimeStampRegexPattern + EscapeRegex(") (<<) ABC"));
								Utilities.AssertBidirContentPattern(terminalA, expectedContentPatternA);

								expectedContentPatternB.Add(EscapeRegex("(>>) ABC"));
								Utilities.AssertBidirContentPattern(terminalB, expectedContentPatternB);

								// ...pong...
								//              A <= B
								terminalB.SendTextLine(Text); // Line from B must be postponed until ping completes with EOL.
								expectedTotalByteCountBA += LineByteCount;
								expectedTotalLineCountBA++;
								Utilities.WaitForSendingAndAssertCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
								Utilities.WaitForReceivingAndAssertCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

							////expectedContentPatternA = same as before as line from B must be postponed until time-out.
								Utilities.AssertBidirContentPattern(terminalA, expectedContentPatternA);

								expectedContentPatternB.Add(EscapeRegex("(<<) ABC<CR><LF>"));
								Utilities.AssertBidirContentPattern(terminalB, expectedContentPatternB);

								// ...and complete ping with EOL:
								//              A => B
								terminalA.SendTextLine("");
								expectedTotalByteCountAB += EolByteCount;
							////expectedTotalLineCountAB++; // Line already had started above.
								Utilities.WaitForSendingAndAssertCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
								Utilities.WaitForReceivingAndAssertCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);
								Utilities.WaitForSendingAndAssertCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
								Utilities.WaitForReceivingAndAssertCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

								var previousIndex = (expectedContentPatternA.Count - 1); // Complete the previous line:
								expectedContentPatternA[previousIndex] +=                                                  EscapeRegex("<CR><LF> (5) (") + Utilities.DurationRegexPattern + EscapeRegex(")");
								expectedContentPatternA.Add(EscapeRegex("(") + Utilities.TimeStampRegexPattern + EscapeRegex(") (>>) ABC<CR><LF> (5) (0.000)"));
								Utilities.AssertBidirContentPattern(terminalA, expectedContentPatternA);

								expectedContentPatternB.Add(EscapeRegex("(>>) <CR><LF>"));
								Utilities.AssertBidirContentPattern(terminalB, expectedContentPatternB);
							}

							// In order to detect erroneous behavior of time-out, wait for twice the time-out before...
							Thread.Sleep(2 * settingsA.TextTerminal.GlueCharsOfLine.Timeout);

							for (int i = 0; i < 2; i++) // ...twice doing...
							{
								// ...subsequent ping without EOL...
								//              A => B
								terminalA.SendText(Text);
								expectedTotalByteCountAB += TextByteCount;

								if (i == 0) { // Account for the fact that non-completed line "ABC" will result in "ABCABC" (Tx) during second iteration.
									expectedTotalLineCountAB++; // Line not completed though.
								}
								Utilities.WaitForSendingAndAssertCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
								Utilities.WaitForReceivingAndAssertCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);

								expectedContentPatternA.Add(EscapeRegex("(") + Utilities.TimeStampRegexPattern + EscapeRegex(") (<<) ABC"));
								Utilities.AssertBidirContentPattern(terminalA, expectedContentPatternA);

								expectedContentPatternB.Add(EscapeRegex("(>>) ABC"));
								Utilities.AssertBidirContentPattern(terminalB, expectedContentPatternB);

								// ...pong...
								//              A <= B
								terminalB.SendTextLine(Text); // Line from B must be postponed until time-out.
								expectedTotalByteCountBA += LineByteCount;
								expectedTotalLineCountBA++;
								Utilities.WaitForSendingAndAssertCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
								Utilities.WaitForReceivingAndAssertCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

							////expectedContentPatternA = same as before as line from B must be postponed until time-out.
								Utilities.AssertBidirContentPattern(terminalA, expectedContentPatternA);

								expectedContentPatternB.Add(EscapeRegex("(<<) ABC<CR><LF>"));
								Utilities.AssertBidirContentPattern(terminalB, expectedContentPatternB);

								// ...and wait for timeout:
								Thread.Sleep(settingsA.TextTerminal.GlueCharsOfLine.Timeout); // No margin needed.

								var previousIndex = (expectedContentPatternA.Count - 1); // Complete the previous line:
								expectedContentPatternA[previousIndex] +=                                                          EscapeRegex(" (3) (") + Utilities.DurationRegexPattern + EscapeRegex(")");
								expectedContentPatternA.Add(EscapeRegex("(") + Utilities.TimeStampRegexPattern + EscapeRegex(") (>>) ABC<CR><LF> (5) (0.000)"));
								Utilities.AssertBidirContentPattern(terminalA, expectedContentPatternA);

							////expectedContentPatternB = same as before.
								Utilities.AssertBidirContentPattern(terminalB, expectedContentPatternB);
							}

							// Refresh and verify again:
							terminalA.RefreshRepositories();
							terminalB.RefreshRepositories();
							Utilities.AssertBidirContentPattern(terminalA, expectedContentPatternA);
							Utilities.AssertBidirContentPattern(terminalB, expectedContentPatternB);
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
			settingsA.Display.ShowTimeStamp = true;
			settingsA.Display.ShowDirection = true;
			settingsA.Display.ShowLength    = true;
			settingsA.Display.ShowDuration  = true;

			var gcolA = settingsA.TextTerminal.GlueCharsOfLine;
			gcolA.Timeout = Timeout.Infinite;
			settingsA.TextTerminal.GlueCharsOfLine = gcolA;

			using (var terminalA = TerminalFactory.CreateTerminal(settingsA))
			{
				try
				{
					Assert.That(terminalA.Start(), Is.True, "Terminal A could not be started!");

					var settingsB = Settings.GetTcpAutoSocketOnIPv4LoopbackSettings(TerminalType.Text);
					settingsB.Display.ShowDirection = true;

					var gcolB = settingsB.TextTerminal.GlueCharsOfLine;
					gcolB.Enabled = false;
					settingsB.TextTerminal.GlueCharsOfLine = gcolB;

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
							int previousIndex;

							// Initial ping-pong:
							//           A => B
							//           A <= B
							terminalA.SendTextLine(Text);
							expectedTotalByteCountAB += LineByteCount;
							expectedTotalLineCountAB++;
							Utilities.WaitForSendingAndAssertCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
							Utilities.WaitForReceivingAndAssertCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);

							terminalB.SendTextLine(Text);
							expectedTotalByteCountBA += LineByteCount;
							expectedTotalLineCountBA++;
							Utilities.WaitForSendingAndAssertCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
							Utilities.WaitForReceivingAndAssertCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

							var expectedContentPatternA = new List<string>(); // Applies to bidir only.
							expectedContentPatternA.Add(EscapeRegex("(") + Utilities.TimeStampRegexPattern + EscapeRegex(") (<<) ABC<CR><LF> (5) (0.000)"));
							expectedContentPatternA.Add(EscapeRegex("(") + Utilities.TimeStampRegexPattern + EscapeRegex(") (>>) ABC<CR><LF> (5) (0.000)"));
							Utilities.AssertBidirContentPattern(terminalA, expectedContentPatternA);

							var expectedContentPatternB = new List<string>(); // Applies to bidir only.
							expectedContentPatternB.Add(EscapeRegex("(>>) ABC<CR><LF>"));
							expectedContentPatternB.Add(EscapeRegex("(<<) ABC<CR><LF>"));
							Utilities.AssertBidirContentPattern(terminalB, expectedContentPatternB);

							for (int i = 0; i < 2; i++) // ...twice doing...
							{
								// Subsequent ping without EOL...
								//              A => B
								terminalA.SendText(Text);
								expectedTotalByteCountAB += TextByteCount;
								expectedTotalLineCountAB++; // Line not completed though.
								Utilities.WaitForSendingAndAssertCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
								Utilities.WaitForReceivingAndAssertCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);

								expectedContentPatternA.Add(EscapeRegex("(") + Utilities.TimeStampRegexPattern + EscapeRegex(") (<<) ABC"));
								Utilities.AssertBidirContentPattern(terminalA, expectedContentPatternA);

								expectedContentPatternB.Add(EscapeRegex("(>>) ABC"));
								Utilities.AssertBidirContentPattern(terminalB, expectedContentPatternB);

								// ...pong...
								//              A <= B
								terminalB.SendTextLine(Text); // Line from B must be postponed until ping completes with EOL.
								expectedTotalByteCountBA += LineByteCount;
								expectedTotalLineCountBA++;
								Utilities.WaitForSendingAndAssertCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
								Utilities.WaitForReceivingAndAssertCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

							////expectedContentPatternA = same as before as line from B must be postponed until line from A has completed (Timeout.Infinite).
								Utilities.AssertBidirContentPattern(terminalA, expectedContentPatternA);

								expectedContentPatternB.Add(EscapeRegex("(<<) ABC<CR><LF>"));
								Utilities.AssertBidirContentPattern(terminalB, expectedContentPatternB);

								// ...and complete ping with EOL:
								//              A => B
								terminalA.SendTextLine("");
								expectedTotalByteCountAB += EolByteCount;
							////expectedTotalLineCountAB++; // Line already had started above.
								Utilities.WaitForSendingAndAssertCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
								Utilities.WaitForReceivingAndAssertCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);
								Utilities.WaitForSendingAndAssertCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
								Utilities.WaitForReceivingAndAssertCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

								previousIndex = (expectedContentPatternA.Count - 1); // Complete the previous line:
								expectedContentPatternA[previousIndex] +=                                                  EscapeRegex("<CR><LF> (5) (") + Utilities.DurationRegexPattern + EscapeRegex(")");
								expectedContentPatternA.Add(EscapeRegex("(") + Utilities.TimeStampRegexPattern + EscapeRegex(") (>>) ABC<CR><LF> (5) (0.000)"));
								Utilities.AssertBidirContentPattern(terminalA, expectedContentPatternA);

								expectedContentPatternB.Add(EscapeRegex("(>>) <CR><LF>"));
								Utilities.AssertBidirContentPattern(terminalB, expectedContentPatternB);
							}

							for (int i = 0; i < 2; i++) // Twice doing...
							{
								// ...subsequent ping without EOL...
								//              A => B
								terminalA.SendText(Text);
								expectedTotalByteCountAB += TextByteCount;

								if (i == 0) { // Account for the fact that non-completed line "ABC" will result in "ABCABC" (Tx) during second iteration.
									expectedTotalLineCountAB++; // Line not completed though.
								}
								Utilities.WaitForSendingAndAssertCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
								Utilities.WaitForReceivingAndAssertCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);

								if (i == 0) { // Account for the fact that non-completed line "ABC" will result in "ABCABC" (Bidir too) during second iteration.
									expectedContentPatternA.Add(EscapeRegex("(") + Utilities.TimeStampRegexPattern + EscapeRegex(") (<<) ABC"));
								}
								else {
									previousIndex = (expectedContentPatternA.Count - 1); // Add to the previous line:
									expectedContentPatternA[previousIndex] +=                                         "ABC";
								}
								Utilities.AssertBidirContentPattern(terminalA, expectedContentPatternA);

								expectedContentPatternB.Add(EscapeRegex("(>>) ABC"));
								Utilities.AssertBidirContentPattern(terminalB, expectedContentPatternB);

								// ...pong...
								//              A <= B
								terminalB.SendTextLine(Text); // Line from B must be postponed until line from A has completed (Timeout.Infinite).
								expectedTotalByteCountBA += LineByteCount;
								expectedTotalLineCountBA++;
								Utilities.WaitForSendingAndAssertCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
								Utilities.WaitForReceivingAndAssertCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

							////expectedContentPatternA = same as before as line from B must be postponed until line from A has completed (Timeout.Infinite).
								Utilities.AssertBidirContentPattern(terminalA, expectedContentPatternA);

								expectedContentPatternB.Add(EscapeRegex("(<<) ABC<CR><LF>"));
								Utilities.AssertBidirContentPattern(terminalB, expectedContentPatternB);

								// ...and wait for at least twice the default timeout:
								Thread.Sleep(2 * Domain.Settings.TextTerminalSettings.GlueCharsOfLineTimeoutDefault); // No margin needed.

							////expectedContentPatternA = same as before as line from B must be postponed until line from A has completed (Timeout.Infinite).
								Utilities.AssertBidirContentPattern(terminalA, expectedContentPatternA);

							////expectedContentPatternB = same as before.
								Utilities.AssertBidirContentPattern(terminalB, expectedContentPatternB);
							}

							// Only then complete the two pings with EOL:
							//              A => B
							terminalA.SendTextLine("");
							expectedTotalByteCountAB += EolByteCount;
						////expectedTotalLineCountAB++; // Line already had started above.

							Utilities.WaitForSendingAndAssertCounts(  terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
							Utilities.WaitForReceivingAndAssertCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);
							Utilities.WaitForSendingAndAssertCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
							Utilities.WaitForReceivingAndAssertCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

							previousIndex = (expectedContentPatternA.Count - 1); // Complete the previous line:
							expectedContentPatternA[previousIndex] +=                                                  EscapeRegex("<CR><LF> (8) (") + Utilities.DurationRegexPattern + EscapeRegex(")");
							expectedContentPatternA.Add(EscapeRegex("(") + Utilities.TimeStampRegexPattern + EscapeRegex(") (>>) ABC<CR><LF> (5) (0.000)"));
							expectedContentPatternA.Add(EscapeRegex("(") + Utilities.TimeStampRegexPattern + EscapeRegex(") (>>) ABC<CR><LF> (5) (0.000)"));
							Utilities.AssertBidirContentPattern(terminalA, expectedContentPatternA);

							expectedContentPatternB.Add(EscapeRegex("(>>) <CR><LF>"));
							Utilities.AssertBidirContentPattern(terminalB, expectedContentPatternB);

							// Refresh and verify again:
							terminalA.RefreshRepositories();
							terminalB.RefreshRepositories();

							Utilities.AssertTxCounts(terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
							Utilities.AssertRxCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);
							Utilities.AssertTxCounts(terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
							Utilities.AssertRxCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

							Utilities.AssertBidirContentPattern(terminalA, expectedContentPatternA);
							Utilities.AssertBidirContentPattern(terminalB, expectedContentPatternB);
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

		#region Utilities
		//==========================================================================================
		// Utilities
		//==========================================================================================

		private static string EscapeRegex(string content)
		{
			return (Utilities.EscapeRegex(content));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
