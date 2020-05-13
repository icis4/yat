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
using System.Globalization;
using System.Text;

using MKY;
using MKY.Text;

using NUnit.Framework;

#endregion

namespace YAT.Domain.Test.TextTerminal
{
	/// <summary></summary>
	public static class FormatTestData
	{
		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		public static string ToExpectedText(byte[] data, Func<byte, string> converter)
		{
			var sb = new StringBuilder();

			bool isFirst = true;
			foreach (var b in data)
			{
				if (isFirst)
					isFirst = false;
				else
					sb.Append(' ');

				sb.Append(converter(b));
			}

			return (sb.ToString());
		}

		/// <summary></summary>
		public static string ToExpectedText(byte[] data, string text, Encoding encoding, Radix radix)
		{
			SupportedEncoding supportedEncoding = (EncodingEx)encoding;
			switch (supportedEncoding)
			{
				case SupportedEncoding.Big5:
				case SupportedEncoding.GB2312: // is GBK !!!
				case SupportedEncoding.X_CP20936: // is GB2312 !!!
				case SupportedEncoding.KS_C_5601_1987:
				case SupportedEncoding.Shift_JIS:
					text = text.Replace("ä", "a");
					text = text.Replace("ö", "o");
					text = text.Replace("ü", "u");
					text = text.Replace("Ä", "A");
					text = text.Replace("Ö", "O");
					text = text.Replace("Ü", "U");
					text = text.Replace("£", "￡"); // Formatting results in 0xEF 0xBF 0xA1
					break;
			}

			switch (radix)
			{
				case Radix.Bin: return (ToExpectedText(data, ByteEx.ConvertToBinaryString));
				case Radix.Oct: return (ToExpectedText(data, ByteEx.ConvertToOctalString));
				case Radix.Dec: return (ToExpectedText(data, (b => b.ToString("D3", CultureInfo.InvariantCulture))));
				case Radix.Hex: return (ToExpectedText(data, (b => b.ToString("X2", CultureInfo.InvariantCulture))));

				case Radix.Char:
				{
					if (text.Contains(@"\")) // The "0\0<CR>1\n2" test cases will not work here, the spaced string
						return (null);       // would become "0 \ 0 < C R > 1 \ n 2", thus ignored (yet).
					else
						return (StringEx.Space(text));
				}

				case Radix.String:
				{
					text = text.Replace(@"\0", "<NUL>"); // Handle the "0\0<CR>1\n2" test cases.
					text = text.Replace(@"\n", "<LF>");
					return (text);
				}

				case Radix.Unicode: return (null); // Not covered by this test case (yet), because the test case tuples not (yet) contain the Unicode.

				default: Assert.Fail("{0} is a radix that is not yet covered by this test case!", radix); return (null);
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			get
			{
				foreach (var tuple in Parser.EncodingTestData.TestCaseTuples)
				{
					var encoding = tuple.Item1;
					var text     = tuple.Item2;
					var data     = tuple.Item3;

					foreach (Radix radix in RadixEx.GetItems())
					{                                                        // TextTerminal.ByteToElement() has two severe limitations for UTF-7, see comments and FR #407 "Support for Base64 and Quoted-Printable" for details.
						if ((encoding == Encoding.UTF7) && ((radix == Radix.Char) || (radix == Radix.String)))
							continue;                                                                  // The text given by 'TestCaseTuples' does not work for UTF-7, thus ignored (yet).
						             //// 'Notenschlüssel' is U+1D11E but U+10000 and above is not supported by .NET Framework 4.x (see FR #329 for more information)
						if ((text == "𝄞") && ((radix == Radix.Char) || (radix == Radix.String)))
							continue;

						var expectedText = ToExpectedText(data, text, encoding, radix);
						if (expectedText != null)
							yield return (new TestCaseData(data, encoding, radix, expectedText).SetName((EncodingEx)encoding + ", " + radix + ", " + expectedText));
					}
				}
			}
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class FormatTest
	{
		#region Test
		//==========================================================================================
		// Test
		//==========================================================================================

		/// <summary></summary>
		[Test, TestCaseSource(typeof(FormatTestData), "TestCases")]
		public virtual void Test(byte[] data, Encoding encoding, Radix radix, string expectedText)
		{
			// Attention:
			// A limited implementation of this test can be found in BinaryTerminal.FormatTest.Test().
			// Changes here may also have to be applied to that limited test.

			var settings = Utilities.GetTcpAutoSocketOnIPv4LoopbackTextSettings();
			settings.TextTerminal.Encoding = (EncodingEx)encoding;
			settings.Display.TxRadix = radix;
			settings.Display.RxRadix = radix;
			settings.Display.ShowRadix = false;
			using (var terminal = new Domain.TextTerminal(settings))
			{                                     // All must result in same text.
				Assert.That(terminal.Format(data, radix),          Is.EqualTo(expectedText));
				Assert.That(terminal.Format(data, IODirection.Tx), Is.EqualTo(expectedText));
				Assert.That(terminal.Format(data, IODirection.Rx), Is.EqualTo(expectedText));
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
