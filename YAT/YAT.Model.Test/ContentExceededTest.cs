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

using MKY;
using MKY.Net.Test;

using NUnit.Framework;

using YAT.Domain;

#endregion

namespace YAT.Model.Test
{
	/// <remarks>Located here rather than domain to be able to verify model byte/line counts.</remarks>
	[TestFixture]
	public class ContentExceededTest
	{
		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		/// <summary></summary>
		[Test] // Test is mandatory, it shall not be excludable. 'IPv4LoopbackIsAvailable' is probed below.
		public virtual void TestContentExceeded()
		{
			if (!ConfigurationProvider.Configuration.IPv4LoopbackIsAvailable)
				Assert.Ignore("No IPv4 loopback is available, therefore this test is excluded. Ensure that IPv4 loopback is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			using (var parser = new Domain.Parser.Parser(Domain.Parser.Mode.NoEscapes)) // Default encoding of UTF-8 is good enough for this test case.
			{
				const int MaxLineLength = 16; // "Ping A => B<CR><LF>" is 13 chars/bytes.
				const string MaxLineExceededWarning = "[Warning: Maximal number of bytes per line exceeded! Check the line break settings or increase the limit in the advanced terminal settings.]";

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
						const string Eol = "<CR><LF>";
						const int EolByteCount = 2; // Fixed to default of <CR><LF>.
						const int Repetitions = 3;
						string contentExceeded;
						int expectedTotalByteCountAB = 0;
						int expectedTotalByteCountBA = 0;
						int expectedTotalLineCountAB = 0;
						int expectedTotalLineCountBA = 0;
						var expectedContentA = new List<string>(); // Applies to bidir only.
						var expectedContentB = new List<string>(); // Applies to bidir only.

						// Initial ping-pong:
						text = "Ping A => B";
						Utilities.TransmitAndVerifyCounts(terminalA, terminalB, parser, text, EolByteCount, ref expectedTotalByteCountAB, ref expectedTotalLineCountAB);
						Utilities.AddAndVerifyBidirContent(terminalA, terminalB, text + Eol, ref expectedContentA, ref expectedContentB);

						text = "Pong B => A";
						Utilities.TransmitAndVerifyCounts(terminalB, terminalA, parser, text, EolByteCount, ref expectedTotalByteCountBA, ref expectedTotalLineCountBA);
						Utilities.AddAndVerifyBidirContent(terminalB, terminalA, text + Eol, ref expectedContentB, ref expectedContentA);

						// Subsequent ping exceeding terminal A, then ping again:
						text = "PingExceeding A => B";
						contentExceeded = StringEx.Left(text, MaxLineLength) + MaxLineExceededWarning;
						Utilities.TransmitAndVerifyCounts(terminalA, terminalB, parser, text, EolByteCount, ref expectedTotalByteCountAB, ref expectedTotalLineCountAB);
						Utilities.AddAndVerifyBidirContent(terminalA, terminalB, contentExceeded, text + Eol, ref expectedContentA, ref expectedContentB);

						text = "Ping A => B";
						Utilities.TransmitAndVerifyCounts(terminalA, terminalB, parser, text, EolByteCount, ref expectedTotalByteCountAB, ref expectedTotalLineCountAB);
						Utilities.AddAndVerifyBidirContent(terminalA, terminalB, text + Eol, ref expectedContentA, ref expectedContentB);

						// Subsequent multiple pings exceeding terminal A, then ping again:
						text = "PingExceeding A => B";
						contentExceeded = StringEx.Left(text, MaxLineLength) + MaxLineExceededWarning;
						for (int i = 0; i < Repetitions; i++)
						{
							Utilities.TransmitAndVerifyCounts(terminalA, terminalB, parser, text, EolByteCount, ref expectedTotalByteCountAB, ref expectedTotalLineCountAB);
							Utilities.AddAndVerifyBidirContent(terminalA, terminalB, contentExceeded, text + Eol, ref expectedContentA, ref expectedContentB);
						}

						text = "Ping A => B";
						Utilities.TransmitAndVerifyCounts(terminalA, terminalB, parser, text, EolByteCount, ref expectedTotalByteCountAB, ref expectedTotalLineCountAB);
						Utilities.AddAndVerifyBidirContent(terminalA, terminalB, text + Eol, ref expectedContentA, ref expectedContentB);

						// Subsequent ping exceeding terminal A, then pong:
						text = "PingExceeding A => B";
						contentExceeded = StringEx.Left(text, MaxLineLength) + MaxLineExceededWarning;
						Utilities.TransmitAndVerifyCounts(terminalA, terminalB, parser, text, EolByteCount, ref expectedTotalByteCountAB, ref expectedTotalLineCountAB);
						Utilities.AddAndVerifyBidirContent(terminalA, terminalB, contentExceeded, text + Eol, ref expectedContentA, ref expectedContentB);

						text = "Pong B => A";
						Utilities.TransmitAndVerifyCounts(terminalB, terminalA, parser, text, EolByteCount, ref expectedTotalByteCountBA, ref expectedTotalLineCountBA);
						Utilities.AddAndVerifyBidirContent(terminalB, terminalA, text + Eol, ref expectedContentB, ref expectedContentA);

						// Subsequent multiple pings exceeding terminal A, then pong:
						text = "PingExceeding A => B";
						contentExceeded = StringEx.Left(text, MaxLineLength) + MaxLineExceededWarning;
						for (int i = 0; i < Repetitions; i++)
						{
							Utilities.TransmitAndVerifyCounts(terminalA, terminalB, parser, text, EolByteCount, ref expectedTotalByteCountAB, ref expectedTotalLineCountAB);
							Utilities.AddAndVerifyBidirContent(terminalA, terminalB, contentExceeded, text + Eol, ref expectedContentA, ref expectedContentB);
						}

						text = "Pong B => A";
						Utilities.TransmitAndVerifyCounts(terminalB, terminalA, parser, text, EolByteCount, ref expectedTotalByteCountBA, ref expectedTotalLineCountBA);
						Utilities.AddAndVerifyBidirContent(terminalB, terminalA, text + Eol, ref expectedContentB, ref expectedContentA);

						// Subsequent pong exceeding terminal A, then pong again:
						text = "PongExceeding B => A";
						contentExceeded = StringEx.Left(text, MaxLineLength) + MaxLineExceededWarning;
						Utilities.TransmitAndVerifyCounts(terminalB, terminalA, parser, text, EolByteCount, ref expectedTotalByteCountBA, ref expectedTotalLineCountBA);
						Utilities.AddAndVerifyBidirContent(terminalB, terminalA, text + Eol, contentExceeded, ref expectedContentB, ref expectedContentA);

						text = "Pong B => A";
						Utilities.TransmitAndVerifyCounts(terminalB, terminalA, parser, text, EolByteCount, ref expectedTotalByteCountBA, ref expectedTotalLineCountBA);
						Utilities.AddAndVerifyBidirContent(terminalB, terminalA, text + Eol, ref expectedContentB, ref expectedContentA);

						// Subsequent multiple pongs exceeding terminal A, then pong again:
						text = "PongExceeding B => A";
						contentExceeded = StringEx.Left(text, MaxLineLength) + MaxLineExceededWarning;
						for (int i = 0; i < Repetitions; i++)
						{
							Utilities.TransmitAndVerifyCounts(terminalB, terminalA, parser, text, EolByteCount, ref expectedTotalByteCountBA, ref expectedTotalLineCountBA);
							Utilities.AddAndVerifyBidirContent(terminalB, terminalA, text + Eol, contentExceeded, ref expectedContentB, ref expectedContentA);
						}

						text = "Pong B => A";
						Utilities.TransmitAndVerifyCounts(terminalB, terminalA, parser, text, EolByteCount, ref expectedTotalByteCountBA, ref expectedTotalLineCountBA);
						Utilities.AddAndVerifyBidirContent(terminalB, terminalA, text + Eol, ref expectedContentB, ref expectedContentA);

						// Subsequent pong exceeding terminal A, then ping:
						text = "PongExceeding B => A";
						contentExceeded = StringEx.Left(text, MaxLineLength) + MaxLineExceededWarning;
						Utilities.TransmitAndVerifyCounts(terminalB, terminalA, parser, text, EolByteCount, ref expectedTotalByteCountBA, ref expectedTotalLineCountBA);
						Utilities.AddAndVerifyBidirContent(terminalB, terminalA, text + Eol, contentExceeded, ref expectedContentB, ref expectedContentA);

						text = "Ping A => B";
						Utilities.TransmitAndVerifyCounts(terminalA, terminalB, parser, text, EolByteCount, ref expectedTotalByteCountAB, ref expectedTotalLineCountAB);
						Utilities.AddAndVerifyBidirContent(terminalA, terminalB, text + Eol, ref expectedContentA, ref expectedContentB);

						// Subsequent multiple pongs exceeding terminal A, then ping:
						text = "PongExceeding B => A";
						contentExceeded = StringEx.Left(text, MaxLineLength) + MaxLineExceededWarning;
						for (int i = 0; i < Repetitions; i++)
						{
							Utilities.TransmitAndVerifyCounts(terminalB, terminalA, parser, text, EolByteCount, ref expectedTotalByteCountBA, ref expectedTotalLineCountBA);
							Utilities.AddAndVerifyBidirContent(terminalB, terminalA, text + Eol, contentExceeded, ref expectedContentB, ref expectedContentA);
						}

						text = "Ping A => B";
						Utilities.TransmitAndVerifyCounts(terminalA, terminalB, parser, text, EolByteCount, ref expectedTotalByteCountAB, ref expectedTotalLineCountAB);
						Utilities.AddAndVerifyBidirContent(terminalA, terminalB, text + Eol, ref expectedContentA, ref expectedContentB);

						// Wait to ensure that no operation is ongoing anymore and verify again:
						Utilities.WaitForReverification();
						Utilities.VerifyTxCounts(terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
						Utilities.VerifyRxCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);
						Utilities.VerifyTxCounts(terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
						Utilities.VerifyRxCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

						Utilities.VerifyBidirContent(terminalA, expectedContentA);
						Utilities.VerifyBidirContent(terminalB, expectedContentB);

						// Refresh and verify again:
						terminalA.RefreshRepositories();
						terminalB.RefreshRepositories();

						Utilities.VerifyTxCounts(terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
						Utilities.VerifyRxCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);
						Utilities.VerifyTxCounts(terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA);
						Utilities.VerifyRxCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

						Utilities.VerifyBidirContent(terminalA, expectedContentA);
						Utilities.VerifyBidirContent(terminalB, expectedContentB);

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
