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
		#region Formats
		//==========================================================================================
		// Formats
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
		#region Formats
		//==========================================================================================
		// Formats
		//==========================================================================================

		/// <summary></summary>
		[Test, TestCaseSource(typeof(KeywordTimeStampTestData), "TestCasesFormats")] // Test is mandatory, it shall not be excludable. 'IPv4LoopbackIsAvailable' is probed below.
		public virtual void TestFormats(string format, bool useUtc)
		{
			if (!ConfigurationProvider.Configuration.IPv4LoopbackIsAvailable)
				Assert.Ignore("No IPv4 loopback is available, therefore this test is excluded. Ensure that IPv4 loopback is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			using (var parser = new Domain.Parser.Parser(Domain.Parser.Mode.Default)) // Defaults are good enough for this test case.
			{
				byte[] parseResult;

				var settingsTx = Settings.GetTcpAutoSocketOnIPv4LoopbackSettings(TerminalType.Text);
				settingsTx.Display.TimeStampFormat = format;
				settingsTx.Display.TimeStampUseUtc = useUtc;
				using (var terminalTx = TerminalFactory.CreateTerminal(settingsTx))
				{
					try
					{
						Assert.That(terminalTx.Start(), Is.True, "Terminal A could not be started!");

						var settingsRx = Settings.GetTcpAutoSocketOnIPv4LoopbackSettings(TerminalType.Text);
						settingsRx.Display.TimeStampFormat = format;
						settingsRx.Display.TimeStampUseUtc = useUtc;
						using (var terminalRx = TerminalFactory.CreateTerminal(settingsRx))
						{
							try
							{
								Assert.That(terminalRx.Start(), Is.True, "Terminal B could not be started!");
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
								const int EolByteCount = 2; // Fixed to default of <CR><LF>.
								int expectedTotalByteCount = 0;
								int expectedTotalLineCount = 0;

								// TimeStamp only:
								textToSend   = keyword;
								textExpected = timeStamp;
								Assert.That(parser.TryParse(textExpected, out parseResult));
								terminalTx.SendTextLine(textToSend);
								textByteCount = parseResult.Length;
								expectedTotalByteCount += (textByteCount + EolByteCount);
								expectedTotalLineCount++;
								Utilities.WaitForTransmissionAndAssertCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount);

								// Prefix + TimeStamp:
								textToSend   = "AT+DATE=" + keyword;
								textExpected = "AT+DATE=" + timeStamp;
								Assert.That(parser.TryParse(textExpected, out parseResult));
								terminalTx.SendTextLine(textToSend);
								textByteCount = parseResult.Length;
								expectedTotalByteCount += (textByteCount + EolByteCount);
								expectedTotalLineCount++;
								Utilities.WaitForTransmissionAndAssertCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount);

								// TimeStamp + Postfix:
								textToSend   = keyword   + "=NOW";
								textExpected = timeStamp + "=NOW";
								Assert.That(parser.TryParse(textExpected, out parseResult));
								terminalTx.SendTextLine(textToSend);
								textByteCount = parseResult.Length;
								expectedTotalByteCount += (textByteCount + EolByteCount);
								expectedTotalLineCount++;
								Utilities.WaitForTransmissionAndAssertCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount);

								// Prefix + TimeStamp + Postfix:
								textToSend   = "AT+DATE=" +  keyword  + "=NOW";
								textExpected = "AT+DATE=" + timeStamp + "=NOW";
								Assert.That(parser.TryParse(textExpected, out parseResult));
								terminalTx.SendTextLine(textToSend);
								textByteCount = parseResult.Length;
								expectedTotalByteCount += (textByteCount + EolByteCount);
								expectedTotalLineCount++;
								Utilities.WaitForTransmissionAndAssertCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount);
							}
							finally // Properly stop even in case of an exception, e.g. a failed assertion.
							{
								terminalRx.Stop();
								Utilities.WaitForStop(terminalRx);
							}
						} // using (terminalB)
					}
					finally // Properly stop even in case of an exception, e.g. a failed assertion.
					{
						terminalTx.Stop();
						Utilities.WaitForStop(terminalTx);
					}
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
