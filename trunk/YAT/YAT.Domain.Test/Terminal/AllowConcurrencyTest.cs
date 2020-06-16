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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

using MKY;
using MKY.Net.Test;

using NUnit.Framework;

using YAT.Domain.Settings;

#endregion

namespace YAT.Domain.Test.Terminal
{
	/// <summary></summary>
	[TestFixture]
	public class AllowConcurrencyTest
	{
		#region Enums
		//==========================================================================================
		// Enums
		//==========================================================================================

		#pragma warning disable 1591

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "A public type is required for NUnit and this type really belongs to the test data only.")]
		public enum Stimulus
		{
			SendTextRepeating,
			SendFile
		}

		/// <remarks>
		/// Named "subsequence" rather than "subsequency" as that is the correct English term.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "A public type is required for NUnit and this type really belongs to the test data only.")]
		public enum Subsequence
		{
			One,
			Random
		}

		#pragma warning restore 1591

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <summary></summary>
		protected const int SendLineCount = 300; // Must correspond to 300 lines of [Stress-1-Normal.txt].

		#endregion

		#region TestCombinatorial
		//==========================================================================================
		// TestCombinatorial
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Well... Better? Really?")]
		[Test, Combinatorial] // Test is mandatory, it shall not be excludable. 'IPv4LoopbackIsAvailable' is probed below.
		public virtual void TestCombinatorial([Values(false, true)]bool allowConcurrency,
		                                      [Values(Stimulus.SendTextRepeating, Stimulus.SendFile)]Stimulus stimulus,
		                                      [Values(Subsequence.One, Subsequence.Random)]Subsequence subsequence)
		{
			if (!ConfigurationProvider.Configuration.IPv4LoopbackIsAvailable)
				Assert.Ignore("No IPv4 loopback is available, therefore this test is excluded. Ensure that IPv4 loopback is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			var settingsTx = Utilities.GetTcpAutoSocketOnIPv4LoopbackTextSettings();
			settingsTx.Send.AllowConcurrency = allowConcurrency;
			settingsTx.TextTerminal.LineSendDelay = new TextLineSendDelaySettingTuple(true, 1, 1); // Delay of 1 ms per line, sending over
			using (var terminalTx = new Domain.TextTerminal(settingsTx))                           // localhost is way too fast otherwise.
			{                                                                                      //  => 300 lines take 300..600 ms, perfect.
				Assert.That(terminalTx.Start(), Is.True, "Terminal A could not be started");

				var settingsRx = Utilities.GetTcpAutoSocketOnIPv4LoopbackTextSettings();
				using (var terminalRx = new Domain.TextTerminal(settingsRx))
				{
					Assert.That(terminalRx.Start(), Is.True, "Terminal B could not be started");
					Utilities.WaitForConnection(terminalTx, terminalRx);

					int subsequentLineCount;
					switch (subsequence)
					{
						case Subsequence.One:
							subsequentLineCount = 1; // Ensures that single line = most likely use case works.
							break;

						case Subsequence.Random:
							var random = new Random(RandomEx.NextPseudoRandomSeed());
							var minValue = (SendLineCount / 100); // 1%
							var maxValue = (SendLineCount / 10); // 10%
							subsequentLineCount = random.Next(minValue, maxValue);
							break;

						default:
							throw (new ArgumentOutOfRangeException("stimulus", stimulus, "'" + stimulus + "' is a stimulus that is not (yet) supported by this test implementation!"));
					}

					var subsequentLineText = "0123456789"; // Repeating lines only contain characters.
					var subsequentLineTextExpected = (subsequentLineText + "<CR><LF>"); // Text settings for testing have 'ShowEOL' = true in order to include EOL in char count.
					switch (stimulus)
					{
						case Stimulus.SendTextRepeating: SendTextRepeating(terminalTx, terminalRx, subsequentLineCount, subsequentLineText); break;
						case Stimulus.SendFile:          SendFile(         terminalTx, terminalRx, subsequentLineCount, subsequentLineText); break;

						default: throw (new ArgumentOutOfRangeException("stimulus", stimulus, "'" + stimulus + "' is a stimulus that is not (yet) supported by this test implementation!"));
					}

					if (allowConcurrency)
						VerifyConcurrent(terminalRx, subsequentLineCount, subsequentLineTextExpected);
					else
						VerifyNonConcurrent(terminalRx, subsequentLineCount, subsequentLineTextExpected);

					// Refresh and verify again:
					terminalRx.RefreshRepositories();

					if (allowConcurrency)
						VerifyConcurrent(terminalRx, subsequentLineCount, subsequentLineTextExpected);
					else
						VerifyNonConcurrent(terminalRx, subsequentLineCount, subsequentLineTextExpected);

					terminalRx.Stop();
					Utilities.WaitForDisconnection(terminalRx);
				} // using (terminalB)

				terminalTx.Stop();
				Utilities.WaitForDisconnection(terminalTx);
			} // using (terminalA)
		}

		/// <summary></summary>
		protected virtual void SendTextRepeating(Domain.TextTerminal terminalTx, Domain.TextTerminal terminalRx, int subsequentLineCount, string subsequentLineText)
		{
			var repeating = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			var repeatingLine = string.Format(CultureInfo.InvariantCulture, @"{0}\!(LineRepeat({1}))", repeating, SendLineCount);
			var repeatingLengthExpected = (repeating.Length + 2); // Adjust for EOL.
			terminalTx.SendTextLine(repeatingLine);

			var subsequentLengthExpected = (subsequentLineText.Length + 2); // Adjust EOL.
			for (int i = 0; i < subsequentLineCount; i++)
				terminalTx.SendTextLine(subsequentLineText); // Immediately invoke sending of subsequent data.

			var expectedTotalByteCount = ((repeatingLengthExpected * SendLineCount) + (subsequentLengthExpected * subsequentLineCount));
			var expectedTotalLineCount = (                           SendLineCount  +                             subsequentLineCount);
			Utilities.WaitForTransmissionAndVerifyCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount, 1000); // See further above, sending takes 300..600 ms.
		}

		/// <summary></summary>
		protected virtual void SendFile(Domain.TextTerminal terminalTx, Domain.TextTerminal terminalRx, int subsequentLineCount, string subsequentLineText)
		{
			var file = SendFilesProvider.FilePaths_StressText.StressFiles[StressTestCase.Normal];
			var message = string.Format(CultureInfo.InvariantCulture, "Precondition: File line count must equal {0} but is {1}!", SendLineCount, file.Item3);
			Assert.That(file.Item3, Is.EqualTo(SendLineCount), message);
			terminalTx.SendFile(file.Item1);

			var subsequentLengthExpected = (subsequentLineText.Length + 2); // Adjust EOL.
			for (int i = 0; i < subsequentLineCount; i++)
				terminalTx.SendTextLine(subsequentLineText); // Immediately invoke sending of subsequent data.
			                                     // Includes EOL.
			var expectedTotalByteCount = (file.Item2 + (subsequentLengthExpected * subsequentLineCount));
			var expectedTotalLineCount = (file.Item3 +                             subsequentLineCount);
			Utilities.WaitForTransmissionAndVerifyCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount, 1000); // See further above, sending takes 300..600 ms.
		}

		/// <summary>Verify that number of lines matches subsequently sent lines and they are found inbetween.</summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'inbetween' is a correct English term.")]
		protected virtual void VerifyConcurrent(Domain.TextTerminal terminalRx, int subsequentLineCountExpected, string subsequentLineTextExpected)
		{
			var displayLines = terminalRx.RepositoryToDisplayLines(RepositoryType.Rx);

			Assert.That(displayLines.Last().Text, Is.Not.EqualTo(subsequentLineTextExpected), "All subsequently sent lines must be found inbetween!");

			int foundCount = 0;
			foreach (var line in displayLines)
			{
				if (line.Text.Equals(subsequentLineTextExpected))
					foundCount++;
			}
			Assert.That(foundCount, Is.EqualTo(subsequentLineCountExpected));
		}

		/// <summary>Verify that last lines match subsequently sent lines.</summary>
		protected virtual void VerifyNonConcurrent(Domain.TextTerminal terminalRx, int subsequentLineCountExpected, string subsequentLineTextExpected)
		{
			var displayLines = terminalRx.RepositoryToDisplayLines(RepositoryType.Rx);

			for (int i = 0; i < subsequentLineCountExpected; i++)
			{
				var j = (displayLines.Count - 1 - i);
				var line = displayLines[j];
				Assert.That(line.Text, Is.EqualTo(subsequentLineTextExpected));
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
