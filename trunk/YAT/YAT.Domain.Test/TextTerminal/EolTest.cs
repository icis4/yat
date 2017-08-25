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
// YAT 2.0 Delta Version 1.99.80
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2017 Matthias Kläy.
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
		[Test, IPv4LoopbackIsAvailableCategory, TestCaseSource(typeof(ByteSequenceTestData), "TestCases")]
		public virtual void TestByteSequence(Encoding encoding, string eolAB, string eolBA)
		{
			const int WaitForDisposal = 100;

			var eolByteCountAB = encoding.GetByteCount(eolAB);
			var eolByteCountBA = encoding.GetByteCount(eolBA);
			var eolIsSymmetric = (eolAB == eolBA);

			var settingsA = Utilities.GetTextTcpAutoSocketOnIPv4LoopbackSettings();
			settingsA.TextTerminal.Encoding = (EncodingEx)encoding;
			settingsA.TextTerminal.TxEol = eolAB;
			settingsA.TextTerminal.RxEol = eolBA;
			using (var terminalA = new Domain.TextTerminal(settingsA))
			{
				Assert.That(terminalA.Start(), Is.True, "Terminal A could not be started");

				var settingsB = Utilities.GetTextTcpAutoSocketOnIPv4LoopbackSettings();
				settingsB.TextTerminal.Encoding = (EncodingEx)encoding;
				settingsB.TextTerminal.TxEol = eolBA;
				settingsB.TextTerminal.RxEol = eolAB;
				using (var terminalB = new Domain.TextTerminal(settingsB))
				{
					Assert.That(terminalB.Start(), Is.True, "Terminal B could not be started");
					Utilities.WaitForConnection(terminalA, terminalB);

					string text;
					int expectedTotalByteCountA = 0;
					int expectedTotalByteCountB = 0;
					int expectedTotalLineCountA = 0;
					int expectedTotalLineCountB = 0;

					text = ""; // A#1
					terminalA.SendLine(text);
					expectedTotalByteCountA += (encoding.GetByteCount(text) + eolByteCountAB);
					expectedTotalLineCountA++;
					Verify(terminalA, terminalB, eolIsSymmetric, expectedTotalByteCountA, expectedTotalLineCountA);

					text = "AA"; // A#2
					terminalA.SendLine(text);
					expectedTotalByteCountA += (encoding.GetByteCount(text) + eolByteCountAB);
					expectedTotalLineCountA++;
					Verify(terminalA, terminalB, eolIsSymmetric, expectedTotalByteCountA, expectedTotalLineCountA);

					text = "ABABAB"; // A#3
					terminalA.SendLine(text);
					expectedTotalByteCountA += (encoding.GetByteCount(text) + eolByteCountAB);
					expectedTotalLineCountA++;
					Verify(terminalA, terminalB, eolIsSymmetric, expectedTotalByteCountA, expectedTotalLineCountA);

					text = "<CR>"; // B#1
					terminalB.SendLine(text);
					expectedTotalByteCountB += (encoding.GetByteCount(text) + eolByteCountBA);
					expectedTotalLineCountB++;
					Verify(terminalB, terminalA, eolIsSymmetric, expectedTotalByteCountB, expectedTotalLineCountB);

					text = "<CR><CR>"; // B#2
					terminalB.SendLine(text);
					expectedTotalByteCountB += (encoding.GetByteCount(text) + eolByteCountBA);
					expectedTotalLineCountB++;
					Verify(terminalB, terminalA, eolIsSymmetric, expectedTotalByteCountB, expectedTotalLineCountB);

					text = "<CR><CR><ESC>"; // B#3
					terminalB.SendLine(text);
					expectedTotalByteCountB += (encoding.GetByteCount(text) + eolByteCountBA);
					expectedTotalLineCountB++;
					Verify(terminalB, terminalA, eolIsSymmetric, expectedTotalByteCountB, expectedTotalLineCountB);

					text = "<ESC>"; // A#4
					terminalA.SendLine(text);
					expectedTotalByteCountA += (encoding.GetByteCount(text) + eolByteCountAB);
					expectedTotalLineCountA++;
					Verify(terminalA, terminalB, eolIsSymmetric, expectedTotalByteCountA, expectedTotalLineCountA);

					text = "BBBB"; // B#4
					terminalB.SendLine(text);
					expectedTotalByteCountB += (encoding.GetByteCount(text) + eolByteCountBA);
					expectedTotalLineCountB++;
					Verify(terminalB, terminalA, eolIsSymmetric, expectedTotalByteCountB, expectedTotalLineCountB);

					terminalB.Stop();
					Utilities.WaitForDisconnection(terminalB);
				}

				terminalA.Stop();
				Utilities.WaitForDisconnection(terminalA);
			}

			Thread.Sleep(WaitForDisposal);
		}

		private static void Verify(Domain.TextTerminal terminalTx, Domain.TextTerminal terminalRx, bool eolIsSymmetric, int expectedTotalByteCount, int expectedTotalLineCount)
		{
			if (eolIsSymmetric)
				Utilities.WaitForTransmission(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount);
			else
				Utilities.WaitForSending(terminalTx, expectedTotalByteCount, expectedTotalLineCount);
		}

		/// <summary></summary>
		[Test, IPv4LoopbackIsAvailableCategory]
		public virtual void TestEolSpace()
		{
			const int WaitForOperation = 100;
			const int WaitForDisposal = 100;

			var settingsA = Utilities.GetTextTcpAutoSocketOnIPv4LoopbackSettings();
			settingsA.TextTerminal.TxEol = "";
			settingsA.TextTerminal.RxEol = "";
			using (var terminalA = new Domain.TextTerminal(settingsA))
			{
				Assert.That(terminalA.Start(), Is.True, "Terminal A could not be started");

				var settingsB = Utilities.GetTextTcpAutoSocketOnIPv4LoopbackSettings();
				settingsB.TextTerminal.TxEol = "";
				settingsB.TextTerminal.RxEol = "";
				using (var terminalB = new Domain.TextTerminal(settingsB))
				{
					Assert.That(terminalB.Start(), Is.True, "Terminal B could not be started");
					Utilities.WaitForConnection(terminalA, terminalB);

					terminalA.SendLine("A"); // Line #1 A >> B, must not result in line break.
					Thread.Sleep(WaitForOperation);
					Verify(terminalA, terminalB, 1);

					terminalB.SendLine("BB"); // Line #2 B >> A, due to direction line break.
					Thread.Sleep(WaitForOperation);
					Verify(terminalB, terminalA, 2);

					terminalB.SendLine("BB"); // Still line #2 B >> A, must not result in additional line break.
					Thread.Sleep(WaitForOperation);
					Verify(terminalB, terminalA, 2);

					terminalB.SendLine("BB"); // Still line #2 B >> A, must not result in additional line break.
					Thread.Sleep(WaitForOperation);
					Verify(terminalB, terminalA, 2);

					terminalA.SendLine("AAA"); // Line #3 A >> B, due to direction line break.
					Thread.Sleep(WaitForOperation);
					Verify(terminalA, terminalB, 3);

					terminalA.SendLine("AAA"); // Still line #3 A >> B, must not result in additional line break.
					Thread.Sleep(WaitForOperation);
					Verify(terminalA, terminalB, 3);

					terminalB.Stop();
					Utilities.WaitForDisconnection(terminalB);
				}

				terminalA.Stop();
				Utilities.WaitForDisconnection(terminalA);
			}

			Thread.Sleep(WaitForDisposal);
		}

		private static void Verify(Domain.TextTerminal terminalTx, Domain.TextTerminal terminalRx, int expectedTotalLineCount)
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
