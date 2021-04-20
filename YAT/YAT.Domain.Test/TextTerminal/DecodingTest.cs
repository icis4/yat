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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using MKY.Net.Test;
using MKY.Text;

using NUnit.Framework;

using YAT.Domain.Utilities;

#endregion

namespace YAT.Domain.Test.TextTerminal
{
	using DecodingTestDataTuple = Tuple<byte[], Encoding, string, DecodingMismatchBehavior>;

	/// <summary></summary>
	public static class DecodingTestData
	{
		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <remarks>
		/// Separate implementation rather than using <see cref="YAT.Domain.TextTerminal"/> for
		/// separation of tester and testee.
		/// </remarks>
		public static string GetExpectedComprehensiveInvalidBytesWarning(byte[] invalidData, Encoding encoding)
		{
			var invalidDataAsString = ByteHelper.FormatHexString(invalidData, showRadix: true);
			var text = string.Format(@"[Warning: ""{0}"" is an invalid '{1}' byte sequence!]", invalidDataAsString, ((EncodingEx)encoding).DisplayName);
			return (text);
		}

		/// <remarks>
		/// Separate implementation rather than using <see cref="YAT.Domain.TextTerminal"/> for
		/// separation of tester and testee.
		/// </remarks>
		public static string GetExpectedComprehensiveOutsideUnicodePlane0Warning(byte[] invalidData)
		{
			var invalidDataAsString = ByteHelper.FormatHexString(invalidData, showRadix: true);
			var text = string.Format(@"[Warning: ""{0}"" is a byte sequence outside the Unicode basic multilingual plane (plane 0)! Only Unicode plane 0 is supported by .NET Framework and thus YAT (yet).]", invalidDataAsString);
			return (text);
		}

		/// <remarks>
		/// Separate implementation rather than using <see cref="YAT.Domain.TextTerminal"/> for
		/// separation of tester and testee.
		/// </remarks>
		public static string GetExpectedCompactWarning(byte[] invalidData)
		{
			var invalidDataAsString = ByteHelper.FormatHexString(invalidData, showRadix: true);
			var text = string.Format(@"[{0}]", invalidDataAsString);
			return (text);
		}

		/// <summary>
		/// The typed enumeration.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Don't care, straightforward test implementation.")]
		public static IEnumerable<DecodingTestDataTuple> TestCaseTuples
		{
			get
			{
				var notRelevant = DecodingMismatchBehaviorEx.Default;

				// ASCII:
				var e = Encoding.ASCII;
				var data = new byte[] { 0x61, 0x62, 0x63 };
				yield return (new DecodingTestDataTuple(data, e, "abc", notRelevant));

				// Windows-1252 [1252]:
				e = EncodingEx.GetEncoding(SupportedEncoding.Windows1252);
				data = new byte[] { 0xE4, 0xF6, 0xFC };
				yield return (new DecodingTestDataTuple(data, e, "äöü", notRelevant));

				// UTF-8 [65001]:
				e = Encoding.UTF8;
				data = new byte[] { 0xC3, 0xA4, 0xC3, 0xB6, 0xC3, 0xBC };
				yield return (new DecodingTestDataTuple(data, e, "äöü", notRelevant));
				data = new byte[] { 0xC3, 0xA4, 0xC3, 0xB6, 0xC3, 0xFF };
				var invalidData = new byte[] { 0xC3, 0xFF };
				yield return (new DecodingTestDataTuple(data, e, "äö"  + GetExpectedComprehensiveOutsideUnicodePlane0Warning(invalidData), DecodingMismatchBehavior.ComprehensiveWarning));
				yield return (new DecodingTestDataTuple(data, e, "äö�" + GetExpectedCompactWarning(invalidData),                           DecodingMismatchBehavior.UnicodeReplacementCharacterAndCompactWarning));
				yield return (new DecodingTestDataTuple(data, e, "äö�",                                                                    DecodingMismatchBehavior.UnicodeReplacementCharacter));
				yield return (new DecodingTestDataTuple(data, e, "äö?" + GetExpectedCompactWarning(invalidData),                           DecodingMismatchBehavior.QuestionMarkAndCompactWarning));
				yield return (new DecodingTestDataTuple(data, e, "äö?",                                                                    DecodingMismatchBehavior.QuestionMark));
				yield return (new DecodingTestDataTuple(data, e, "äö",                                                                     DecodingMismatchBehavior.Discard));

				// Makes little sense to also test other Unicode encoding UTF-16 LE/BE, UTF-32 LE/BE,...
				// This test focuses on decoding. Other tests focus on a wider range of encodings.

				// Big5 [950]:
				e = EncodingEx.GetEncoding(SupportedEncoding.Big5);
				data = new byte[] { 0x24, 0xA2, 0x47, 0xA3, 0xE1 }; // 1-2-2 bytes, and ￡ not £!
				yield return (new DecodingTestDataTuple(data, e, "$￡€", notRelevant));
				data = new byte[] { 0x24, 0xA2, 0x47, 0xA3, 0xFF }; // 1-2-2 bytes
				yield return (new DecodingTestDataTuple(data, e, "$￡?", notRelevant));

				// GBK [936]:                                // is GBK!
				e = EncodingEx.GetEncoding(SupportedEncoding.GB2312);
				data = new byte[] { 0xD2, 0xBB, 0xB6, 0xFE, 0xD6, 0xDD };
				yield return (new DecodingTestDataTuple(data, e, "一二州", notRelevant));
				data = new byte[] { 0xD2, 0xBB, 0xB6, 0xFE, 0xD6, 0xFF };
				yield return (new DecodingTestDataTuple(data, e, "一二?", notRelevant));

				// GB2312 (-80) [20936]:                     // is GB2312!
				e = EncodingEx.GetEncoding(SupportedEncoding.X_CP20936);
				data = new byte[] { 0xD2, 0xBB, 0xB6, 0xFE, 0xD6, 0xDD };
				yield return (new DecodingTestDataTuple(data, e, "一二州", notRelevant));
				data = new byte[] { 0xD2, 0xBB, 0xB6, 0xFE, 0xD6, 0xFF };
				yield return (new DecodingTestDataTuple(data, e, "一二?", notRelevant));

				// GB18030 [54936]:
				e = EncodingEx.GetEncoding(SupportedEncoding.GB18030);
				data = new byte[] { 0xD2, 0xBB, 0xB6, 0xFE, 0xD6, 0xDD };
				yield return (new DecodingTestDataTuple(data, e, "一二州", notRelevant));
				data = new byte[] { 0xD2, 0xBB, 0xB6, 0xFE, 0xD6, 0xFF };
				invalidData = new byte[] { 0xD6, 0xFF };
				yield return (new DecodingTestDataTuple(data, e, "一二"  + GetExpectedComprehensiveInvalidBytesWarning(invalidData, e), DecodingMismatchBehavior.ComprehensiveWarning));
				yield return (new DecodingTestDataTuple(data, e, "一二�" + GetExpectedCompactWarning(invalidData),                      DecodingMismatchBehavior.UnicodeReplacementCharacterAndCompactWarning));
				yield return (new DecodingTestDataTuple(data, e, "一二�",                                                               DecodingMismatchBehavior.UnicodeReplacementCharacter));
				yield return (new DecodingTestDataTuple(data, e, "一二?" + GetExpectedCompactWarning(invalidData),                      DecodingMismatchBehavior.QuestionMarkAndCompactWarning));
				yield return (new DecodingTestDataTuple(data, e, "一二?",                                                               DecodingMismatchBehavior.QuestionMark));
				yield return (new DecodingTestDataTuple(data, e, "一二",                                                                DecodingMismatchBehavior.Discard));

				// KSC [949]:
				e = EncodingEx.GetEncoding(SupportedEncoding.KS_C_5601_1987);
				data = new byte[] { 0xEC, 0xE9, 0xEC, 0xA3, 0xF1, 0xB6 };
				yield return (new DecodingTestDataTuple(data, e, "一二州", notRelevant));
				data = new byte[] { 0xEC, 0xE9, 0xEC, 0xA3, 0xF1, 0xFF };
				yield return (new DecodingTestDataTuple(data, e, "一二?", notRelevant));

				// Shift-JIS [932]:
				e = EncodingEx.GetEncoding(SupportedEncoding.Shift_JIS);
				data = new byte[] { 0x88, 0xEA, 0x93, 0xF1, 0x8F, 0x42 };
				yield return (new DecodingTestDataTuple(data, e, "一二州", notRelevant));
				data = new byte[] { 0x88, 0xEA, 0x93, 0xF1, 0xFF, 0x42 };
				invalidData = new byte[] { 0xFF, 0x42 };
				yield return (new DecodingTestDataTuple(data, e, "一二"  + GetExpectedComprehensiveInvalidBytesWarning(invalidData, e), DecodingMismatchBehavior.ComprehensiveWarning));
				yield return (new DecodingTestDataTuple(data, e, "一二�" + GetExpectedCompactWarning(invalidData),                      DecodingMismatchBehavior.UnicodeReplacementCharacterAndCompactWarning));
				yield return (new DecodingTestDataTuple(data, e, "一二�",                                                               DecodingMismatchBehavior.UnicodeReplacementCharacter));
				yield return (new DecodingTestDataTuple(data, e, "一二?" + GetExpectedCompactWarning(invalidData),                      DecodingMismatchBehavior.QuestionMarkAndCompactWarning));
				yield return (new DecodingTestDataTuple(data, e, "一二?",                                                               DecodingMismatchBehavior.QuestionMark));
				yield return (new DecodingTestDataTuple(data, e, "一二",                                                                DecodingMismatchBehavior.Discard));
			}
		}

		/// <summary>
		/// The NUnit un-typed enumeration.
		/// </summary>
		public static IEnumerable TestCases
		{
			get
			{
				foreach (var tuple in TestCaseTuples)
				{
					var dataTx                 = tuple.Item1;
					var dataTxAsString         = ByteHelper.FormatHexString(dataTx, showRadix: true);
					var encodingRx             = tuple.Item2;
					var expectedContentRx      = tuple.Item3;
					var mismatchBehaviorRx     = tuple.Item4;

					yield return (new TestCaseData(dataTx, encodingRx, expectedContentRx, mismatchBehaviorRx).SetName((EncodingEx)encodingRx + ", " + dataTxAsString + ", " + mismatchBehaviorRx));
				}
			}
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class DecodingTest
	{
		#region Tests
		//==========================================================================================
		// Test
		//==========================================================================================

		/// <summary></summary>
		[Test, TestCaseSource(typeof(DecodingTestData), "TestCases")] // Test is mandatory, it shall not be excludable. 'IPv4LoopbackIsAvailable' is probed below.
		public virtual void TestDecoding(byte[] dataTx, Encoding encodingRx, string expectedContentRx, DecodingMismatchBehavior mismatchBehaviorRx)
		{
			if (!ConfigurationProvider.Configuration.IPv4LoopbackIsAvailable)
				Assert.Ignore("No IPv4 loopback is available, therefore this test is excluded. Ensure that IPv4 loopback is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			var settingsTx = Settings.GetTcpAutoSocketOnIPv4LoopbackSettings(TerminalType.Binary);
			using (var terminalTx = TerminalFactory.CreateTerminal(settingsTx))
			{
				try
				{
					Assert.That(terminalTx.Start(), Is.True, "Terminal Tx could not be started!");

					var settingsRx = Settings.GetTcpAutoSocketOnIPv4LoopbackSettings(TerminalType.Text);
					settingsRx.TextTerminal.Encoding = (EncodingEx)encodingRx;
					settingsRx.TextTerminal.DecodingMismatchBehavior = mismatchBehaviorRx;
					using (var terminalRx = TerminalFactory.CreateTerminal(settingsRx))
					{
						try
						{
							Assert.That(terminalRx.Start(), Is.True, "Terminal Rx could not be started!");
							Utilities.WaitForConnection(terminalTx, terminalRx);

							terminalTx.SendRaw(dataTx);

							Utilities.WaitForSendingAndAssertCounts(  terminalTx, dataTx.Length, 1);
							Utilities.WaitForReceivingAndAssertCounts(terminalRx, dataTx.Length, 1);
							Utilities.AssertRxContent(terminalRx, new string[] { expectedContentRx });
						}
						finally // Properly stop even in case of an exception, e.g. a failed assertion.
						{
							terminalRx.Stop();
							Utilities.WaitForStop(terminalRx);
						}
					} // using (terminalRx)
				}
				finally // Properly stop even in case of an exception, e.g. a failed assertion.
				{
					terminalTx.Stop();
					Utilities.WaitForStop(terminalTx);
				}
			} // using (terminalTx)
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
