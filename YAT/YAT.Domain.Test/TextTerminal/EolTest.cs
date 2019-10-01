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
// YAT Version 2.1.0
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;

using MKY.Net.Test;
using MKY.Text;

using NUnit.Framework;

#endregion

namespace YAT.Domain.Test.TextTerminal
{
	/// <summary></summary>
	public static class ByteSequenceTestData
	{
		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		public static IEnumerable<string> SingleByteSequences
		{
			get
			{
				// Control characters:
				yield return ("<CR><LF>");
				yield return ("<LF>"); // Note that <LF><CR> and <CR> cannot be used because they are used in the test implementation further below.
				yield return ("<TAB>");
				yield return ("<NUL>");

				// Visible characters:
				yield return ("ABC");
				yield return ("CR");
				yield return ("X");

				// Mixed:
				yield return ("AB<CR>");
				yield return ("CR<LF>");
				yield return ("X<NUL>");

				// Space:
				yield return (" ");
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "What's wrong with 'MultiByte'?")]
		public static IEnumerable<string> MultiByteSequences
		{
			get
			{
				yield return ("ä");
				yield return ("€");
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			get
			{
				foreach (string eolAB in SingleByteSequences)
				{
					foreach (string eolBA in SingleByteSequences)
					{
						if (eolAB == eolBA)
							yield return (new TestCaseData(Encoding.Default, eolAB, eolBA).SetName(@"Symmetric """ + eolAB + @""""));
						else
							yield return (new TestCaseData(Encoding.Default, eolAB, eolBA).SetName(@"Asymmetric """ + eolAB + @""" | """ + eolBA + @""""));
					}
				}

				foreach (string eolAB in MultiByteSequences)
				{
					foreach (string eolBA in MultiByteSequences)
					{
						if (eolAB == eolBA)
							yield return (new TestCaseData(Encoding.UTF8, eolAB, eolBA).SetName(@"Symmetric """ + eolAB + @""""));
						else
							yield return (new TestCaseData(Encoding.UTF8, eolAB, eolBA).SetName(@"Asymmetric """ + eolAB + @""" | """ + eolBA + @""""));
					}
				}
			}
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class EolTest
	{
		#region Tests
		//==========================================================================================
		// Test
		//==========================================================================================

		/// <summary></summary>
		[Test, TestCaseSource(typeof(ByteSequenceTestData), "TestCases")] // Test is mandatory, it shall not be excludable. 'IPv4LoopbackIsAvailable' is probed below.
		public virtual void TestByteSequence(Encoding encoding, string eolAB, string eolBA)
		{
			if (!ConfigurationProvider.Configuration.IPv4LoopbackIsAvailable)
				Assert.Ignore("No IPv4 loopback is available, therefore this test is excluded. Ensure that IPv4 loopback is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			const int WaitForDisposal = 100;

			using (var parser = new Domain.Parser.Parser(encoding, Domain.Parser.Modes.RadixAndAsciiEscapes))
			{
				byte[] parseResult;

				Assert.That(parser.TryParse(eolAB, out parseResult));
				int eolByteCountAB = parseResult.Length;

				Assert.That(parser.TryParse(eolBA, out parseResult));
				int eolByteCountBA = parseResult.Length;

				var settingsA = Utilities.GetTcpAutoSocketOnIPv4LoopbackTextSettings();
				settingsA.TextTerminal.Encoding = (EncodingEx)encoding;
				settingsA.TextTerminal.TxEol = eolAB;
				settingsA.TextTerminal.RxEol = eolBA;
				using (var terminalA = new Domain.TextTerminal(settingsA))
				{
					Assert.That(terminalA.Start(), Is.True, "Terminal A could not be started");

					var settingsB = Utilities.GetTcpAutoSocketOnIPv4LoopbackTextSettings();
					settingsB.TextTerminal.Encoding = (EncodingEx)encoding;
					settingsB.TextTerminal.TxEol = eolBA;
					settingsB.TextTerminal.RxEol = eolAB;
					using (var terminalB = new Domain.TextTerminal(settingsB))
					{
						Assert.That(terminalB.Start(), Is.True, "Terminal B could not be started");
						Utilities.WaitForConnection(terminalA, terminalB);

						var eolIsSymmetric = (eolAB == eolBA);
						string text;
						int textByteCount;
						int expectedTotalByteCountA = 0;
						int expectedTotalByteCountB = 0;
						int expectedTotalLineCountA = 0;
						int expectedTotalLineCountB = 0;

						text = ""; // A#1
						terminalA.SendTextLine(text);
						Assert.That(parser.TryParse(text, out parseResult));
						textByteCount = parseResult.Length;
						expectedTotalByteCountA += (textByteCount + eolByteCountAB);
						expectedTotalLineCountA++;
						WaitForTransmissionEolAware(terminalA, terminalB, eolIsSymmetric, expectedTotalByteCountA, expectedTotalLineCountA);

						text = "AA"; // A#2
						terminalA.SendTextLine(text);
						Assert.That(parser.TryParse(text, out parseResult));
						textByteCount = parseResult.Length;
						expectedTotalByteCountA += (textByteCount + eolByteCountAB);
						expectedTotalLineCountA++;
						WaitForTransmissionEolAware(terminalA, terminalB, eolIsSymmetric, expectedTotalByteCountA, expectedTotalLineCountA);

						text = "ABABAB"; // A#3
						terminalA.SendTextLine(text);
						Assert.That(parser.TryParse(text, out parseResult));
						textByteCount = parseResult.Length;
						expectedTotalByteCountA += (textByteCount + eolByteCountAB);
						expectedTotalLineCountA++;
						WaitForTransmissionEolAware(terminalA, terminalB, eolIsSymmetric, expectedTotalByteCountA, expectedTotalLineCountA);

						text = "<CR>"; // B#1
						terminalB.SendTextLine(text);
						Assert.That(parser.TryParse(text, out parseResult));
						textByteCount = parseResult.Length;
						expectedTotalByteCountB += (textByteCount + eolByteCountBA);
						expectedTotalLineCountB++;
						WaitForTransmissionEolAware(terminalB, terminalA, eolIsSymmetric, expectedTotalByteCountB, expectedTotalLineCountB);

						text = "<CR><CR>"; // B#2
						terminalB.SendTextLine(text);
						Assert.That(parser.TryParse(text, out parseResult));
						textByteCount = parseResult.Length;
						expectedTotalByteCountB += (textByteCount + eolByteCountBA);
						expectedTotalLineCountB++;
						WaitForTransmissionEolAware(terminalB, terminalA, eolIsSymmetric, expectedTotalByteCountB, expectedTotalLineCountB);

						text = "<CR><CR><ESC>"; // B#3
						terminalB.SendTextLine(text);
						Assert.That(parser.TryParse(text, out parseResult));
						textByteCount = parseResult.Length;
						expectedTotalByteCountB += (textByteCount + eolByteCountBA);
						expectedTotalLineCountB++;
						WaitForTransmissionEolAware(terminalB, terminalA, eolIsSymmetric, expectedTotalByteCountB, expectedTotalLineCountB);

						text = "<ESC>"; // A#4
						terminalA.SendTextLine(text);
						Assert.That(parser.TryParse(text, out parseResult));
						textByteCount = parseResult.Length;
						expectedTotalByteCountA += (textByteCount + eolByteCountAB);
						expectedTotalLineCountA++;
						WaitForTransmissionEolAware(terminalA, terminalB, eolIsSymmetric, expectedTotalByteCountA, expectedTotalLineCountA);

						text = "BBBB"; // B#4
						terminalB.SendTextLine(text);
						Assert.That(parser.TryParse(text, out parseResult));
						textByteCount = parseResult.Length;
						expectedTotalByteCountB += (textByteCount + eolByteCountBA);
						expectedTotalLineCountB++;
						WaitForTransmissionEolAware(terminalB, terminalA, eolIsSymmetric, expectedTotalByteCountB, expectedTotalLineCountB);

						terminalB.Stop();
						Utilities.WaitForDisconnection(terminalB);
					} // using (terminalB)

					terminalA.Stop();
					Utilities.WaitForDisconnection(terminalA);
				} // using (terminalA)
			} // using (parser)

			Thread.Sleep(WaitForDisposal);
		}

		/// <remarks>
		/// 'expectedTotalLineCount' will be compared against the number of lines in the view,
		/// i.e. complete as well as incomplete lines.
		/// </remarks>
		/// <remarks>
		/// Comparison against the completed number of lines is not (yet) possible, change #375
		/// "consider to migrate Byte/Line Count/Rate from model to domain" is required for this.
		/// </remarks>
		private static void WaitForTransmissionEolAware(Domain.TextTerminal terminalTx, Domain.TextTerminal terminalRx, bool eolIsSymmetric, int expectedTotalByteCount, int expectedTotalLineCount)
		{
			if (eolIsSymmetric)
				Utilities.WaitForTransmissionAndVerifyCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount);
			else
				Utilities.WaitForSendingAndVerifyCounts(terminalTx, expectedTotalByteCount, expectedTotalLineCount);
		}

		/// <summary></summary>
		[Test] // Test is mandatory, it shall not be excludable. 'IPv4LoopbackIsAvailable' is probed below.
		public virtual void TestEolNone()
		{
			if (!ConfigurationProvider.Configuration.IPv4LoopbackIsAvailable)
				Assert.Ignore("No IPv4 loopback is available, therefore this test is excluded. Ensure that IPv4 loopback is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			const int WaitForOperation = 100;
			const int WaitForDisposal = 100;

			var settingsA = Utilities.GetTcpAutoSocketOnIPv4LoopbackTextSettings();
			settingsA.TextTerminal.TxEol = "";
			settingsA.TextTerminal.RxEol = "";
			using (var terminalA = new Domain.TextTerminal(settingsA))
			{
				Assert.That(terminalA.Start(), Is.True, "Terminal A could not be started");

				var settingsB = Utilities.GetTcpAutoSocketOnIPv4LoopbackTextSettings();
				settingsB.TextTerminal.TxEol = "";
				settingsB.TextTerminal.RxEol = "";
				using (var terminalB = new Domain.TextTerminal(settingsB))
				{
					Assert.That(terminalB.Start(), Is.True, "Terminal B could not be started");
					Utilities.WaitForConnection(terminalA, terminalB);

					terminalA.SendTextLine("A"); // Line #1 A >> B, must not result in line break.
					Thread.Sleep(WaitForOperation);
					VerifyLineCount(terminalA, terminalB, 1);

					terminalB.SendTextLine("BB"); // Line #2 B >> A, due to direction line break.
					Thread.Sleep(WaitForOperation);
					VerifyLineCount(terminalB, terminalA, 2);

					terminalB.SendTextLine("BB"); // Still line #2 B >> A, must not result in additional line break.
					Thread.Sleep(WaitForOperation);
					VerifyLineCount(terminalB, terminalA, 2);

					terminalB.SendTextLine("BB"); // Still line #2 B >> A, must not result in additional line break.
					Thread.Sleep(WaitForOperation);
					VerifyLineCount(terminalB, terminalA, 2);

					terminalA.SendTextLine("AAA"); // Line #3 A >> B, due to direction line break.
					Thread.Sleep(WaitForOperation);
					VerifyLineCount(terminalA, terminalB, 3);

					terminalA.SendTextLine("AAA"); // Still line #3 A >> B, must not result in additional line break.
					Thread.Sleep(WaitForOperation);
					VerifyLineCount(terminalA, terminalB, 3);

					terminalB.Stop();
					Utilities.WaitForDisconnection(terminalB);
				}

				terminalA.Stop();
				Utilities.WaitForDisconnection(terminalA);
			}

			Thread.Sleep(WaitForDisposal);
		}

		/// <remarks>
		/// 'expectedTotalLineCount' will be compared against the number of lines in the view,
		/// i.e. complete as well as incomplete lines.
		/// </remarks>
		/// <remarks>
		/// Comparison against the completed number of lines is not (yet) possible, change #375
		/// "consider to migrate Byte/Line Count/Rate from model to domain" is required for this.
		/// </remarks>
		private static void VerifyLineCount(Domain.TextTerminal terminalTx, Domain.TextTerminal terminalRx, int expectedTotalLineCount)
		{
			int txTotalLineCount = terminalTx.GetRepositoryLineCount(RepositoryType.Bidir);
			int rxTotalLineCount = terminalRx.GetRepositoryLineCount(RepositoryType.Bidir);

			string txMessage = "Error!" +
			                   " Number of total lines = " + txTotalLineCount +
			                   " mismatches expected = " + expectedTotalLineCount + ".";

			string rxMessage = "Error!" +
			                   " Number of total lines = " + rxTotalLineCount +
			                   " mismatches expected = " + expectedTotalLineCount + ".";

			Assert.That(txTotalLineCount, Is.EqualTo(expectedTotalLineCount), txMessage);
			Assert.That(rxTotalLineCount, Is.EqualTo(expectedTotalLineCount), rxMessage);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
