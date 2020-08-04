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

using MKY;
using MKY.Net.Test;
using MKY.Settings;

using NUnit.Framework;

using YAT.Domain;
using YAT.Settings.Application;

#endregion

namespace YAT.Model.Test
{
	/// <remarks>Located here rather than domain to be able to verify model byte/line counts.</remarks>
	[TestFixture]
	public class LineExceededTest
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
			// Create temporary in-memory application settings for this test run:
			ApplicationSettings.Create(ApplicationSettingsFileAccess.None);

			// Prevent auto-save of workspace settings:
			ApplicationSettings.LocalUserSettings.General.AutoSaveWorkspace = false;
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TearDown", Justification = "Naming according to NUnit.")]
		[TestFixtureTearDown]
		public virtual void TestFixtureTearDown()
		{
			// Close and dispose of temporary in-memory application settings:
			ApplicationSettings.CloseAndDispose();
		}

		#endregion

		#region Test
		//==========================================================================================
		// Test
		//==========================================================================================

		/// <summary></summary>
		[Test] // Test is mandatory, it shall not be excludable. 'IPv4LoopbackIsAvailable' is probed below.
		public virtual void Test()
		{
			if (!ConfigurationProvider.Configuration.IPv4LoopbackIsAvailable)
				Assert.Ignore("No IPv4 loopback is available, therefore this test is excluded. Ensure that IPv4 loopback is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			using (var parser = new Domain.Parser.Parser(Domain.Parser.Mode.Default)) // Defaults are good enough for this test case.
			{                                 // "Ping A => B<CR><LF>" is 13 chars/bytes.
				const int MaxLineLength = 21; // "PingExceeding A => B<CR><LF>" is 22 chars/bytes, result shall be "PingExceeding A => B<CR>[Warning: ..."
				const string MaxLineExceededWarningPattern = @"\[Warning: Maximal number of (characters|bytes) per line exceeded! Check the line break settings in Terminal > Settings > (Text|Binary) or increase the limit in Terminal > Settings > Advanced.\]";

				var settingsA = Settings.GetTcpAutoSocketOnIPv4LoopbackSettings(TerminalType.Text);
				settingsA.Display.MaxLineLength = MaxLineLength;
				using (var terminalA = new Terminal(settingsA))
				{
					Assert.That(terminalA.Start(), Is.True, "Terminal A could not be started!");

					var settingsB = Settings.GetTcpAutoSocketOnIPv4LoopbackSettings(TerminalType.Text);
				////settingsB.Display.MaxLineLength is kept at default of 1000 chars/bytes.
					using (var terminalB = new Terminal(settingsB))
					{
						Assert.That(terminalB.Start(), Is.True, "Terminal B could not be started!");
						Utilities.WaitForConnection(terminalA, terminalB);

						string text;
						const string Cr = "<CR>"; // Related to 'MaxLineLength = 21' above.
						const string Eol = "<CR><LF>";
						const int Repetitions = 3;
						string contentPatternExceeded;
						int expectedTotalByteCountAB = 0;
						int expectedTotalByteCountBA = 0;
						int expectedTotalByteCountOffsetAB = 0;
						int expectedTotalByteCountOffsetBA = 0;
						int expectedTotalLineCountAB = 0;
						int expectedTotalLineCountBA = 0;
						var expectedContentA = new List<string>(); // Applies to bidir only.
						var expectedContentB = new List<string>(); // Applies to bidir only.

						// Initial ping-pong:
						text = "Ping A => B";
						Utilities.TransmitAndAssertCounts(terminalA, terminalB, parser, text, ref expectedTotalByteCountAB, ref expectedTotalLineCountAB);
						Utilities.AddAndAssertBidirContent(terminalA, terminalB, text + Eol, ref expectedContentA, ref expectedContentB);

						text = "Pong B => A";
						Utilities.TransmitAndAssertCounts(terminalB, terminalA, parser, text, ref expectedTotalByteCountBA, ref expectedTotalLineCountBA);
						Utilities.AddAndAssertBidirContent(terminalB, terminalA, text + Eol, ref expectedContentB, ref expectedContentA);

						// Subsequent ping exceeding terminal A, then ping again:
						text = "PingExceeding A => B";
						contentPatternExceeded = StringEx.Left(text, MaxLineLength) + Cr + MaxLineExceededWarningPattern;
						expectedTotalByteCountOffsetAB--; // Only B = Rx will show the complete line.
						Utilities.TransmitAndAssertRxCounts(terminalA, terminalB, parser, text, ref expectedTotalByteCountAB, ref expectedTotalLineCountAB);
						Utilities.AddAndAssertBidirContent(terminalA, terminalB, contentPatternExceeded, text + Eol, ref expectedContentA, ref expectedContentB);

						text = "Ping A => B";
						Utilities.TransmitAndAssertRxCounts(terminalA, terminalB, parser, text, ref expectedTotalByteCountAB, ref expectedTotalLineCountAB);
						Utilities.AddAndAssertBidirContent(terminalA, terminalB, text + Eol, ref expectedContentA, ref expectedContentB);

						// Subsequent multiple pings exceeding terminal A, then ping again:
						text = "PingExceeding A => B";
						contentPatternExceeded = StringEx.Left(text, MaxLineLength) + Cr + MaxLineExceededWarningPattern;
						for (int i = 0; i < Repetitions; i++)
						{
							expectedTotalByteCountOffsetAB--; // Only B = Rx will show the complete line.
							Utilities.TransmitAndAssertRxCounts(terminalA, terminalB, parser, text, ref expectedTotalByteCountAB, ref expectedTotalLineCountAB);
							Utilities.AddAndAssertBidirContent(terminalA, terminalB, contentPatternExceeded, text + Eol, ref expectedContentA, ref expectedContentB);
						}

						text = "Ping A => B";
						Utilities.TransmitAndAssertRxCounts(terminalA, terminalB, parser, text, ref expectedTotalByteCountAB, ref expectedTotalLineCountAB);
						Utilities.AddAndAssertBidirContent(terminalA, terminalB, text + Eol, ref expectedContentA, ref expectedContentB);

						// Subsequent ping exceeding terminal A, then pong:
						text = "PingExceeding A => B";
						contentPatternExceeded = StringEx.Left(text, MaxLineLength) + Cr + MaxLineExceededWarningPattern;
						expectedTotalByteCountOffsetAB--; // Only B = Rx will show the complete line.
						Utilities.TransmitAndAssertRxCounts(terminalA, terminalB, parser, text, ref expectedTotalByteCountAB, ref expectedTotalLineCountAB);
						Utilities.AddAndAssertBidirContent(terminalA, terminalB, contentPatternExceeded, text + Eol, ref expectedContentA, ref expectedContentB);

						text = "Pong B => A";
						Utilities.TransmitAndAssertRxCountsWithOffset(terminalB, terminalA, parser, text, ref expectedTotalByteCountBA, ref expectedTotalLineCountBA, expectedTotalByteCountOffsetBA);
						Utilities.AddAndAssertBidirContent(terminalB, terminalA, text + Eol, ref expectedContentB, ref expectedContentA);

						// Subsequent multiple pings exceeding terminal A, then pong:
						text = "PingExceeding A => B";
						contentPatternExceeded = StringEx.Left(text, MaxLineLength) + Cr + MaxLineExceededWarningPattern;
						for (int i = 0; i < Repetitions; i++)
						{
							expectedTotalByteCountOffsetAB--; // Only B = Rx will show the complete line.
							Utilities.TransmitAndAssertRxCounts(terminalA, terminalB, parser, text, ref expectedTotalByteCountAB, ref expectedTotalLineCountAB);
							Utilities.AddAndAssertBidirContent(terminalA, terminalB, contentPatternExceeded, text + Eol, ref expectedContentA, ref expectedContentB);
						}

						text = "Pong B => A";
						Utilities.TransmitAndAssertRxCountsWithOffset(terminalB, terminalA, parser, text, ref expectedTotalByteCountBA, ref expectedTotalLineCountBA, expectedTotalByteCountOffsetBA);
						Utilities.AddAndAssertBidirContent(terminalB, terminalA, text + Eol, ref expectedContentB, ref expectedContentA);

						// Subsequent pong exceeding terminal A, then pong again:
						text = "PongExceeding B => A";
						contentPatternExceeded = StringEx.Left(text, MaxLineLength) + Cr + MaxLineExceededWarningPattern;
						expectedTotalByteCountOffsetBA--; // Only B = Tx will show the complete line.
						Utilities.TransmitAndAssertTxCounts(terminalB, parser, text, ref expectedTotalByteCountBA, ref expectedTotalLineCountBA);
						Utilities.WaitForReceivingAndAssertCountsWithOffset(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA, expectedTotalByteCountOffsetBA);
						Utilities.AddAndAssertBidirContent(terminalB, terminalA, text + Eol, contentPatternExceeded, ref expectedContentB, ref expectedContentA);

						text = "Pong B => A";
						Utilities.TransmitAndAssertRxCountsWithOffset(terminalB, terminalA, parser, text, ref expectedTotalByteCountBA, ref expectedTotalLineCountBA, expectedTotalByteCountOffsetBA);
						Utilities.AddAndAssertBidirContent(terminalB, terminalA, text + Eol, ref expectedContentB, ref expectedContentA);

						// Subsequent multiple pongs exceeding terminal A, then pong again:
						text = "PongExceeding B => A";
						contentPatternExceeded = StringEx.Left(text, MaxLineLength) + Cr + MaxLineExceededWarningPattern;
						for (int i = 0; i < Repetitions; i++)
						{
							expectedTotalByteCountOffsetBA--; // Only B = Tx will show the complete line.
							Utilities.TransmitAndAssertTxCounts(terminalB, parser, text, ref expectedTotalByteCountBA, ref expectedTotalLineCountBA);
							Utilities.WaitForReceivingAndAssertCountsWithOffset(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA, expectedTotalByteCountOffsetBA);
							Utilities.AddAndAssertBidirContent(terminalB, terminalA, text + Eol, contentPatternExceeded, ref expectedContentB, ref expectedContentA);
						}

						text = "Pong B => A";
						Utilities.TransmitAndAssertRxCountsWithOffset(terminalB, terminalA, parser, text, ref expectedTotalByteCountBA, ref expectedTotalLineCountBA, expectedTotalByteCountOffsetBA);
						Utilities.AddAndAssertBidirContent(terminalB, terminalA, text + Eol, ref expectedContentB, ref expectedContentA);

						// Subsequent pong exceeding terminal A, then ping:
						text = "PongExceeding B => A";
						contentPatternExceeded = StringEx.Left(text, MaxLineLength) + Cr + MaxLineExceededWarningPattern;
						expectedTotalByteCountOffsetBA--; // Only B = Tx will show the complete line.
						Utilities.TransmitAndAssertTxCounts(terminalB, parser, text, ref expectedTotalByteCountBA, ref expectedTotalLineCountBA);
						Utilities.WaitForReceivingAndAssertCountsWithOffset(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA, expectedTotalByteCountOffsetBA);
						Utilities.AddAndAssertBidirContent(terminalB, terminalA, text + Eol, contentPatternExceeded, ref expectedContentB, ref expectedContentA);

						text = "Ping A => B";
						Utilities.TransmitAndAssertRxCounts(terminalA, terminalB, parser, text, ref expectedTotalByteCountAB, ref expectedTotalLineCountAB);
						Utilities.AddAndAssertBidirContent(terminalA, terminalB, text + Eol, ref expectedContentA, ref expectedContentB);

						// Subsequent multiple pongs exceeding terminal A, then ping:
						text = "PongExceeding B => A";
						contentPatternExceeded = StringEx.Left(text, MaxLineLength) + Cr + MaxLineExceededWarningPattern;
						for (int i = 0; i < Repetitions; i++)
						{
							expectedTotalByteCountOffsetBA--; // Only B = Tx will show the complete line.
							Utilities.TransmitAndAssertTxCounts(terminalB, parser, text, ref expectedTotalByteCountBA, ref expectedTotalLineCountBA);
							Utilities.WaitForReceivingAndAssertCountsWithOffset(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA, expectedTotalByteCountOffsetBA);
							Utilities.AddAndAssertBidirContent(terminalB, terminalA, text + Eol, contentPatternExceeded, ref expectedContentB, ref expectedContentA);
						}

						text = "Ping A => B";
						Utilities.TransmitAndAssertRxCounts(terminalA, terminalB, parser, text, ref expectedTotalByteCountAB, ref expectedTotalLineCountAB);
						Utilities.AddAndAssertBidirContent(terminalA, terminalB, text + Eol, ref expectedContentA, ref expectedContentB);

						// Subsequent pongs exceeding terminal A with variation, then ping:
						text = "PongExceeding B => A "; // Additional space will result in <CR> not being shown.
						contentPatternExceeded = StringEx.Left(text, MaxLineLength) + MaxLineExceededWarningPattern;
						expectedTotalByteCountOffsetBA--; // Only B = Tx will show the complete line.
						expectedTotalByteCountOffsetBA--; // Account for missing <CR>.
						Utilities.TransmitAndAssertTxCounts(terminalB, parser, text, ref expectedTotalByteCountBA, ref expectedTotalLineCountBA);
						Utilities.WaitForReceivingAndAssertCountsWithOffset(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA, expectedTotalByteCountOffsetBA);
						Utilities.AddAndAssertBidirContent(terminalB, terminalA, text + Eol, contentPatternExceeded, ref expectedContentB, ref expectedContentA);

						//     "PongExceeding B => A"
						text = "PongNotEx'ng B => A"; // Less a character will result in <CR><LF> being shown.
						Utilities.TransmitAndAssertTxCounts(terminalB, parser, text, ref expectedTotalByteCountBA, ref expectedTotalLineCountBA);
						Utilities.WaitForReceivingAndAssertCountsWithOffset(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA, expectedTotalByteCountOffsetBA);
						Utilities.AddAndAssertBidirContent(terminalB, terminalA, text + Eol, text + Eol, ref expectedContentB, ref expectedContentA);

						text = "Ping A => B";
						Utilities.TransmitAndAssertRxCounts(terminalA, terminalB, parser, text, ref expectedTotalByteCountAB, ref expectedTotalLineCountAB);
						Utilities.AddAndAssertBidirContent(terminalA, terminalB, text + Eol, ref expectedContentA, ref expectedContentB);

						// Wait to ensure that no operation is ongoing anymore and verify again:
						Utilities.WaitForReverification();
						Utilities.AssertTxCountsWithOffset(terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB, expectedTotalByteCountOffsetAB);
						Utilities.AssertRxCounts(          terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);
						Utilities.AssertTxCounts(          terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
						Utilities.AssertRxCountsWithOffset(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA, expectedTotalByteCountOffsetBA);

						Utilities.AssertBidirContent(terminalA, expectedContentA);
						Utilities.AssertBidirContent(terminalB, expectedContentB);

						// Refresh and verify again:
						terminalA.RefreshRepositories();
						terminalB.RefreshRepositories();

						Utilities.AssertTxCountsWithOffset(terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB, expectedTotalByteCountOffsetAB);
						Utilities.AssertRxCounts(          terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);
						Utilities.AssertTxCounts(          terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
						Utilities.AssertRxCountsWithOffset(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA, expectedTotalByteCountOffsetBA);

						Utilities.AssertBidirContent(terminalA, expectedContentA);
						Utilities.AssertBidirContent(terminalB, expectedContentB);

						terminalB.StopIO();
						Utilities.WaitForDisconnection(terminalB);
					} // using (terminalB)

					terminalA.StopIO();
					Utilities.WaitForDisconnection(terminalA);
				} // using (terminalA)
			} // using (parser)
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
