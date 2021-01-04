﻿//==================================================================================================
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
using System.Globalization;
using System.Text;

using MKY;
using MKY.Text;

using NUnit.Framework;

#endregion

namespace YAT.Domain.Test.BinaryTerminal
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
			UnusedArg.PreventAnalysisWarning(encoding, "Not needed (yet) but already here for orthogonality with TextTerminal.FormatTest.ToExpectedText()");

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
					{                 // Binary terminals are limited to single-byte encodings.
						if ((encoding == Encoding.ASCII) || (encoding == Encoding.Default))
						{
							var expectedText = ToExpectedText(data, text, encoding, radix);
							if (expectedText != null)
								yield return (new TestCaseData(data, radix, expectedText).SetName((EncodingEx)encoding + ", " + radix + ", " + expectedText));
						}
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
		public virtual void Test(byte[] data, Radix radix, string expectedText)
		{
			// Attention:
			// This test is a limited implementation of TextTerminal.FormatTest.Test().
			// Changes here likely also have to be applied to that full test.

			var settings = Settings.GetTcpAutoSocketOnIPv4LoopbackSettings(TerminalType.Binary);
			settings.Display.TxRadix = radix;
			settings.Display.RxRadix = radix;
			settings.Display.ShowRadix = false;
			using (var terminal = new Domain.BinaryTerminal(settings))
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
