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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

using MKY.Net.Test;

using NUnit.Framework;

#endregion

namespace YAT.Domain.Test.Terminal
{
	/// <summary></summary>
	public static class KeywordTimeStampTestData
	{
		#region Format
		//==========================================================================================
		// Format
		//==========================================================================================

		/// <summary></summary>
		public static IEnumerable<string> Formats
		{
			get
			{
				foreach (var preset in TimeStampFormatPresetEx.GetItems())
					yield return (preset.ToFormat());

				yield return ("yy MM dd HH mm ss"); // Some user defined format.
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCasesFormats
		{
			get
			{
				foreach (string format in Formats)
				{
					yield return (new TestCaseData(format, false).SetName(@"""" + format + @""" in local time"));
					yield return (new TestCaseData(format, true ).SetName(@"""" + format + @""" in UTC"));
				}
			}
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class KeywordTimeStampTest
	{
		#region Format
		//==========================================================================================
		// Format
		//==========================================================================================

		/// <summary></summary>
		[Test, TestCaseSource(typeof(KeywordTimeStampTestData), "TestCasesFormats")] // Test is mandatory, it shall not be excludable. 'IPv4LoopbackIsAvailable' is probed below.
		public virtual void TestFormat(string format, bool useUtc)
		{
			if (!ConfigurationProvider.Configuration.IPv4LoopbackIsAvailable)
				Assert.Ignore("No IPv4 loopback is available, therefore this test is excluded. Ensure that IPv4 loopback is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			const int WaitForDisposal = 100;

			using (var parser = new Domain.Parser.Parser(Domain.Parser.Mode.NoEscapes))
			{
				byte[] parseResult;

				var settingsTx = Utilities.GetTcpAutoSocketOnIPv4LoopbackTextSettings();
				settingsTx.Display.TimeStampFormat = format;
				settingsTx.Display.TimeStampUseUtc = useUtc;
				using (var terminalTx = new Domain.TextTerminal(settingsTx))
				{
					Assert.That(terminalTx.Start(), Is.True, "Terminal A could not be started");

					var settingsRx = Utilities.GetTcpAutoSocketOnIPv4LoopbackTextSettings();
					settingsRx.Display.TimeStampFormat = format;
					settingsRx.Display.TimeStampUseUtc = useUtc;
					using (var terminalRx = new Domain.TextTerminal(settingsRx))
					{
						Assert.That(terminalRx.Start(), Is.True, "Terminal B could not be started");
						Utilities.WaitForConnection(terminalTx, terminalRx);

						string keyword = @"\!(TimeStamp())";

						DateTime now = DateTime.Now;
						string timeStamp;
						if (useUtc)        // UTC
							timeStamp = now.ToUniversalTime().ToString(format, DateTimeFormatInfo.CurrentInfo);
						else
							timeStamp = now.ToString(format, DateTimeFormatInfo.CurrentInfo);

						string textToSend;
						string textExpected;
						int textByteCount;
						int eolByteCount = 2; // Fixed to default of <CR><LF>.
						int expectedTotalByteCount = 0;
						int expectedTotalLineCount = 0;

						// TimeStamp only:
						textToSend   = keyword;
						textExpected = timeStamp;
						Assert.That(parser.TryParse(textExpected, out parseResult));
						terminalTx.SendTextLine(textToSend);
						textByteCount = parseResult.Length;
						expectedTotalByteCount += (textByteCount + eolByteCount);
						expectedTotalLineCount++;
						Utilities.WaitForTransmissionAndVerifyCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount);

						// Prefix + TimeStamp:
						textToSend   = "AT+DATE=" + keyword;
						textExpected = "AT+DATE=" + timeStamp;
						Assert.That(parser.TryParse(textExpected, out parseResult));
						terminalTx.SendTextLine(textToSend);
						textByteCount = parseResult.Length;
						expectedTotalByteCount += (textByteCount + eolByteCount);
						expectedTotalLineCount++;
						Utilities.WaitForTransmissionAndVerifyCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount);

						// TimeStamp + Postfix:
						textToSend   = keyword   + "=NOW";
						textExpected = timeStamp + "=NOW";
						Assert.That(parser.TryParse(textExpected, out parseResult));
						terminalTx.SendTextLine(textToSend);
						textByteCount = parseResult.Length;
						expectedTotalByteCount += (textByteCount + eolByteCount);
						expectedTotalLineCount++;
						Utilities.WaitForTransmissionAndVerifyCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount);

						// Prefix + TimeStamp + Postfix:
						textToSend   = "AT+DATE=" +  keyword  + "=NOW";
						textExpected = "AT+DATE=" + timeStamp + "=NOW";
						Assert.That(parser.TryParse(textExpected, out parseResult));
						terminalTx.SendTextLine(textToSend);
						textByteCount = parseResult.Length;
						expectedTotalByteCount += (textByteCount + eolByteCount);
						expectedTotalLineCount++;
						Utilities.WaitForTransmissionAndVerifyCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount);

						terminalRx.Stop();
						Utilities.WaitForDisconnection(terminalRx);
					} // using (terminalB)

					terminalTx.Stop();
					Utilities.WaitForDisconnection(terminalTx);
				} // using (terminalA)

			} // using (parser)

			Thread.Sleep(WaitForDisposal);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
