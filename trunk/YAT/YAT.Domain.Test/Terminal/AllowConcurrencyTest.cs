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

using System.Globalization;
using System.Linq;
using System.Threading;

using MKY.Net.Test;

using NUnit.Framework;

#endregion

namespace YAT.Domain.Test.Terminal
{
	/// <summary></summary>
	[TestFixture]
	public class AllowConcurrencyTest
	{
		#region TestNonConcurrentSendRepeating
		//==========================================================================================
		// TestNonConcurrentSendRepeating
		//==========================================================================================

		/// <summary></summary>
		[Test] // Test is mandatory, it shall not be excludable. 'IPv4LoopbackIsAvailable' is probed below.
		public virtual void TestNonConcurrentSendRepeating()
		{
			if (!ConfigurationProvider.Configuration.IPv4LoopbackIsAvailable)
				Assert.Ignore("No IPv4 loopback is available, therefore this test is excluded. Ensure that IPv4 loopback is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			const int WaitForDisposal = 100;

			var settingsTx = Utilities.GetTcpAutoSocketOnIPv4LoopbackTextSettings();
		////settingsTx.Send.AllowConcurrency is false by default.
			settingsTx.TextTerminal.LineSendDelay = new TextLineSendDelaySettingTuple(true, 1, 1); // Delay of 1 ms per line, sending over
			using (var terminalTx = new Domain.TextTerminal(settingsTx))                           // localhost is way too fast otherwise.
			{                                                                                      //  => 300 lines take 300..600 ms, perfect.
				Assert.That(terminalTx.Start(), Is.True, "Terminal A could not be started");

				var settingsRx = Utilities.GetTcpAutoSocketOnIPv4LoopbackTextSettings();
				using (var terminalRx = new Domain.TextTerminal(settingsRx))
				{
					Assert.That(terminalRx.Start(), Is.True, "Terminal B could not be started");
					Utilities.WaitForConnection(terminalTx, terminalRx);

					var repeatingCount = 300;
					var repeating = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
					var repeatingLine = string.Format(CultureInfo.InvariantCulture, @"{0}\!(LineRepeat({1}))", repeating, repeatingCount);
					var repeatingTextExpected = repeating + "<CR><LF>"; // Text settings for testing have 'ShowEOL' = true in order to include EOL in char count.
					var repeatingLengthExpected = (repeating.Length + 2); // Adjust for EOL.
					terminalTx.SendTextLine(repeatingLine);

					var subsequentLine = "0123456789"; // Repeating lines only contain characters.
					var subsequentTextExpected = (subsequentLine + "<CR><LF>"); // Text settings for testing have 'ShowEOL' = true in order to include EOL in char count.
					var subsequentLengthExpected = (subsequentLine.Length + 2); // Adjust EOL.
					terminalTx.SendTextLine(subsequentLine); // Immediately invoke sending of subsequent data.

					var expectedTotalByteCount = ((repeatingLengthExpected * repeatingCount) + subsequentLengthExpected);
					var expectedTotalLineCount = (                           repeatingCount  + 1);                                       // See above, sending takes 300..600 ms.
					Utilities.WaitForTransmissionAndVerifyCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount, 1000);

					// Verify that last line matches subsequently sent line:
					var displayLines = terminalRx.RepositoryToDisplayLines(RepositoryType.Rx);
					var lastLine = displayLines.Last();
					Assert.That(lastLine.Text, Is.EqualTo(subsequentTextExpected));

					terminalRx.Stop();
					Utilities.WaitForDisconnection(terminalRx);
				} // using (terminalB)

				terminalTx.Stop();
				Utilities.WaitForDisconnection(terminalTx);
			} // using (terminalA)

			Thread.Sleep(WaitForDisposal);
		}

		#endregion

		#region TestNonConcurrentSendFile
		//==========================================================================================
		// TestNonConcurrentSendFile
		//==========================================================================================

		/// <summary></summary>
		[Test] // Test is mandatory, it shall not be excludable. 'IPv4LoopbackIsAvailable' is probed below.
		public virtual void TestNonConcurrentSendFile()
		{
			if (!ConfigurationProvider.Configuration.IPv4LoopbackIsAvailable)
				Assert.Ignore("No IPv4 loopback is available, therefore this test is excluded. Ensure that IPv4 loopback is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			const int WaitForDisposal = 100;

			var settingsTx = Utilities.GetTcpAutoSocketOnIPv4LoopbackTextSettings();
		////settingsTx.Send.AllowConcurrency is false by default.
			settingsTx.TextTerminal.LineSendDelay = new TextLineSendDelaySettingTuple(true, 1, 1); // Delay of 1 ms per line, sending over
			using (var terminalTx = new Domain.TextTerminal(settingsTx))                           // localhost is way too fast otherwise.
			{                                                                                      //  => 300 lines of [Stress-1-Normal.txt]
				Assert.That(terminalTx.Start(), Is.True, "Terminal A could not be started");       //     take 300..600 ms, perfect.

				var settingsRx = Utilities.GetTcpAutoSocketOnIPv4LoopbackTextSettings();
				using (var terminalRx = new Domain.TextTerminal(settingsRx))
				{
					Assert.That(terminalRx.Start(), Is.True, "Terminal B could not be started");
					Utilities.WaitForConnection(terminalTx, terminalRx);

					var file = SendFilesProvider.FilePaths_StressText.StressFiles[StressTestCase.Normal];
					terminalTx.SendFile(file.Item1);

					var subsequentLine = "0123456789"; // Stress files only contain characters.
					var subsequentTextExpected = (subsequentLine + "<CR><LF>"); // Text settings for testing have 'ShowEOL' = true in order to include EOL in char count.
					var subsequentLengthExpected = (subsequentLine.Length + 2); // Adjust EOL.
					terminalTx.SendTextLine(subsequentLine); // Immediately invoke sending of subsequent data.
					                                      // Includes EOL.
					var expectedTotalByteCount = (file.Item2 + subsequentLengthExpected);
					var expectedTotalLineCount = (file.Item3 + 1);                                                                       // See above, sending takes 300..600 ms.
					Utilities.WaitForTransmissionAndVerifyCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount, 1000);

					// Verify that last line matches subsequently sent line:
					var displayLines = terminalRx.RepositoryToDisplayLines(RepositoryType.Rx);
					var lastLine = displayLines.Last();
					Assert.That(lastLine.Text, Is.EqualTo(subsequentTextExpected));

					terminalRx.Stop();
					Utilities.WaitForDisconnection(terminalRx);
				} // using (terminalB)

				terminalTx.Stop();
				Utilities.WaitForDisconnection(terminalTx);
			} // using (terminalA)

			Thread.Sleep(WaitForDisposal);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
