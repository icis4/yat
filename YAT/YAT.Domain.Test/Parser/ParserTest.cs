//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 2 Development Version 1.99.29
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using NUnit.Framework;

namespace YAT.Domain.Test.Parser
{
	/// <summary></summary>
	public static class ParserTestData
	{
		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			get
			{
				// Mixed.
				yield return (new TestCaseData(@"Hello \s(Hello \d(10) Hello) Hello", new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x0A, 0x20, 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x48, 0x65, 0x6C, 0x6C, 0x6F } ).SetName("Mixed"));

				// Empty.
				yield return (new TestCaseData("", new byte[] { }).SetName("Empty"));

				// Whitespace.
				yield return (new TestCaseData("\0",   new byte[] { 0x00 } ).SetName("Whitespace Null"));
				yield return (new TestCaseData("\a",   new byte[] { 0x07 } ).SetName("Whitespace Bell"));
				yield return (new TestCaseData("\b",   new byte[] { 0x08 } ).SetName("Whitespace Backspace"));
				yield return (new TestCaseData("	", new byte[] { 0x09 } ).SetName("Whitespace Tab"));

				// C-style escape.
				yield return (new TestCaseData("\\0",     new byte[] { 0x00 } ).SetName("C-stype <NUL>"));
				yield return (new TestCaseData("\\a",     new byte[] { 0x07 } ).SetName("C-stype <BEL>"));
				yield return (new TestCaseData("\\A",     new byte[] { 0x07 } ).SetName("C-stype capital <BEL>"));
				yield return (new TestCaseData("\\b",     new byte[] { 0x08 } ).SetName("C-stype <BS>"));
				yield return (new TestCaseData("\\bABC",  new byte[] { 0x08, 0x41, 0x42, 0x43 }).SetName("C-stype <BS> with following characters"));
				yield return (new TestCaseData("\\t",     new byte[] { 0x09 } ).SetName("C-stype <TAB>"));
				yield return (new TestCaseData("\\v",     new byte[] { 0x0B } ).SetName("C-stype <VT>"));
				yield return (new TestCaseData("\\r",     new byte[] { 0x0D } ).SetName("C-stype <CR>"));
				yield return (new TestCaseData("\\n",     new byte[] { 0x0A } ).SetName("C-style <LF>"));
				yield return (new TestCaseData("\\r\\n",  new byte[] { 0x0D, 0x0A } ).SetName("C-style <CR><LF>"));
				yield return (new TestCaseData("\\r \\n", new byte[] { 0x0D, 0x20, 0x0A }).SetName("C-style <CR> <LF>"));
				yield return (new TestCaseData("\\f",     new byte[] { 0x0C } ).SetName("C-style <FF>"));
				yield return (new TestCaseData("C-style\\r\\nis like\\tthis", new byte[] { 0x43, 0x2D, 0x73, 0x74, 0x79, 0x6C, 0x65, 0x0D, 0x0A, 0x69, 0x73, 0x20, 0x6C, 0x69, 0x6B, 0x65, 0x09, 0x74, 0x68, 0x69, 0x73 }).SetName("C-style string"));
				yield return (new TestCaseData("\\01",    new byte[] {   01 }).SetName("C-style octal value"));
				yield return (new TestCaseData("\\12",    new byte[] {   12 }).SetName("C-style decimal value"));
				yield return (new TestCaseData("\\0x1A",  new byte[] { 0x1A }).SetName("C-style hexadecimal value 0x"));
				yield return (new TestCaseData("\\x1A",   new byte[] { 0x1A }).SetName("C-style hexadecimal value x only"));
				yield return (new TestCaseData("\\x1A2B", new byte[] { 0x1A, 0x2B }).SetName("C-style hexadecimal value x only 16 bits"));
				yield return (new TestCaseData("\\x1A2",  new byte[] { 0x1A, 0x2  }).SetName("C-style hexadecimal value x only 16 bits odd"));

				// ASCII.
				yield return (new TestCaseData("<NUL>",     new byte[] { 0x00 } ).SetName("ASCII <NUL>"));
				yield return (new TestCaseData("<BEL>",     new byte[] { 0x07 } ).SetName("ASCII <BEL>"));
				yield return (new TestCaseData("<BS>",      new byte[] { 0x08 } ).SetName("ASCII <BS>"));
				yield return (new TestCaseData("<TAB>",     new byte[] { 0x09 } ).SetName("ASCII <TAB>"));
				yield return (new TestCaseData("<CR>",      new byte[] { 0x0D } ).SetName("ASCII <CR>"));
				yield return (new TestCaseData("<LF>",      new byte[] { 0x0A } ).SetName("ASCII <LF>"));
				yield return (new TestCaseData("<CR><LF>",  new byte[] { 0x0D, 0x0A } ).SetName("ASCII <CR><LF>"));
				yield return (new TestCaseData("<CR LF>",   new byte[] { 0x0D, 0x0A } ).SetName("ASCII <CR LF>"));
				yield return (new TestCaseData("<CR> <LF>", new byte[] { 0x0D, 0x20, 0x0A } ).SetName("ASCII <CR> <LF>"));
				yield return (new TestCaseData("Empty <>",  new byte[] { 0x45, 0x6D, 0x70, 0x74, 0x79, 0x20 } ).SetName("ASCII Empty <>"));
				yield return (new TestCaseData("<XOn>",     new byte[] { 0x11 } ).SetName("ASCII <XOn>"));
				yield return (new TestCaseData("<XOff>",    new byte[] { 0x13 } ).SetName("ASCII <XOff>"));

				// Parenthesis and co.
				yield return (new TestCaseData(@"Hello \(round\) and \<angle\> brackets", new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x28, 0x72, 0x6F, 0x75, 0x6E, 0x64, 0x29, 0x20, 0x61, 0x6E, 0x64, 0x20, 0x3C, 0x61, 0x6E, 0x67, 0x6C, 0x65, 0x3E, 0x20, 0x62, 0x72, 0x61, 0x63, 0x6B, 0x65, 0x74, 0x73 } ).SetName("Parenthesis and co."));

				// Backslashes
				yield return (new TestCaseData(@"Hello \\back\\ slashes", new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x5C, 0x62, 0x61, 0x63, 0x6B, 0x5C, 0x20, 0x73, 0x6C, 0x61, 0x73, 0x68, 0x65, 0x73 } ).SetName("Backslashes"));

				// Char.
				yield return (new TestCaseData(@"Single char \c(9)",						new byte[] { 0x53, 0x69, 0x6E, 0x67, 0x6C, 0x65, 0x20, 0x63, 0x68, 0x61, 0x72, 0x20, 0x39 } ).SetName("Char single '9'"));
				yield return (new TestCaseData(@"Single char \c(.)",						new byte[] { 0x53, 0x69, 0x6E, 0x67, 0x6C, 0x65, 0x20, 0x63, 0x68, 0x61, 0x72, 0x20, 0x2E } ).SetName("Char single '.'"));
				yield return (new TestCaseData(@"Hello \c(()round\c()) brackets",			new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x28, 0x72, 0x6F, 0x75, 0x6E, 0x64, 0x29, 0x20, 0x62, 0x72, 0x61, 0x63, 0x6B, 0x65, 0x74, 0x73 } ).SetName("Char round brackets 1"));
				yield return (new TestCaseData(@"\c(H)\c(e)llo \c(()round\c()) brackets",	new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x28, 0x72, 0x6F, 0x75, 0x6E, 0x64, 0x29, 0x20, 0x62, 0x72, 0x61, 0x63, 0x6B, 0x65, 0x74, 0x73 } ).SetName("Char round brackets 2"));
				yield return (new TestCaseData(@"Empty \c()",								new byte[] { 0x45, 0x6D, 0x70, 0x74, 0x79, 0x20 } ).SetName("Char empty"));

				// Hex.
				yield return (new TestCaseData(@"\h()",								new byte[] { } ).SetName("Hex empty"));
				yield return (new TestCaseData(@"\h(00)",							new byte[] { 0x00 } ).SetName("Hex 00"));
				yield return (new TestCaseData(@"\h(01)",							new byte[] { 0x01 } ).SetName("Hex 01"));
				yield return (new TestCaseData(@"\h(20)",							new byte[] { 0x20 } ).SetName("Hex 20"));
				yield return (new TestCaseData(@"\h(7F)",							new byte[] { 0x7F } ).SetName("Hex 7F"));
				yield return (new TestCaseData(@"\h(80)",							new byte[] { 0x80 } ).SetName("Hex 80"));
				yield return (new TestCaseData(@"\h(81)",							new byte[] { 0x81 } ).SetName("Hex 81"));
				yield return (new TestCaseData(@"\h(AA)",							new byte[] { 0xAA } ).SetName("Hex AA"));
				yield return (new TestCaseData(@"\h(FE)",							new byte[] { 0xFE } ).SetName("Hex FE"));
				yield return (new TestCaseData(@"\h(FF)",							new byte[] { 0xFF } ).SetName("Hex FF"));
				yield return (new TestCaseData(@"\h(00 01 20 7F 80 81 AA FE FF)",	new byte[] { 0x00, 0x01, 0x20, 0x7F, 0x80, 0x81, 0xAA, 0xFE, 0xFF } ).SetName("Hex sequence 1"));
				yield return (new TestCaseData(@"\h(00 00)",						new byte[] { 0x00, 0x00 } ).SetName("Hex 00 00"));
				yield return (new TestCaseData(@"\h(FF FF)",						new byte[] { 0xFF, 0xFF } ).SetName("Hex FF FF"));
				yield return (new TestCaseData(@"\h(23 5D 24 81 20 A5)",			new byte[] { 0x23, 0x5D, 0x24, 0x81, 0x20, 0xA5 } ).SetName("Hex sequence 2"));
				yield return (new TestCaseData(@"\h(00 \h(FF) 00)",					new byte[] { 0x00, 0xFF, 0x00 } ).SetName("Hex nested"));

				// EOL.
				yield return (new TestCaseData(@"A<CR><CR><LF>B<CR><LF><LF>C<CR><LF>D<CR>E<LF>F", new byte[] { 0x41, 0x0D, 0x0D, 0x0A, 0x42, 0x0D, 0x0A, 0x0A, 0x43, 0x0D, 0x0A, 0x44, 0x0D, 0x45, 0x0A, 0x46 }).SetName("Partial EOL"));
			}
		}

		#endregion

		#region Test Cases Encoding
		//==========================================================================================
		// Test Cases Encoding
		//==========================================================================================

		/// <summary></summary>
		public static IEnumerable TestCasesEncoding
		{
			get
			{
				// ASCII.
				yield return (new TestCaseData(Encoding.ASCII, "abc", new byte[] { 0x61, 0x62, 0x63 }));

				// UTF-8.
				yield return (new TestCaseData(Encoding.UTF8, "abc", new byte[] { 0x61, 0x62, 0x63 }));
				yield return (new TestCaseData(Encoding.UTF8, "äöü", new byte[] { 0xC3, 0xA4, 0xC3, 0xB6, 0xC3, 0xBC }));
				yield return (new TestCaseData(Encoding.UTF8, "ÄÖÜ", new byte[] { 0xC3, 0x84, 0xC3, 0x96, 0xC3, 0x9C }));
				yield return (new TestCaseData(Encoding.UTF8, "$£€", new byte[] { 0x24, 0xC2, 0xA3, 0xE2, 0x82, 0xAC }));
				yield return (new TestCaseData(Encoding.UTF8, "čěř", new byte[] { 0xC4, 0x8D, 0xC4, 0x9B, 0xC5, 0x99 }));

				// UTF-16 (little endian).
				yield return (new TestCaseData(Encoding.Unicode, "abc", new byte[] { 0x61, 0x00, 0x62, 0x00, 0x63, 0x00 }));
				yield return (new TestCaseData(Encoding.Unicode, "äöü", new byte[] { 0xE4, 0x00, 0xF6, 0x00, 0xFC, 0x00 }));
				yield return (new TestCaseData(Encoding.Unicode, "ÄÖÜ", new byte[] { 0xC4, 0x00, 0xD6, 0x00, 0xDC, 0x00 }));
				yield return (new TestCaseData(Encoding.Unicode, "$£€", new byte[] { 0x24, 0x00, 0xA3, 0x00, 0xAC, 0x20 }));
				yield return (new TestCaseData(Encoding.Unicode, "čěř", new byte[] { 0x0D, 0x01, 0x1B, 0x01, 0x59, 0x01 }));

				// UTF-32 (little endian).
				yield return (new TestCaseData(Encoding.UTF32, "abc", new byte[] { 0x61, 0x00, 0x00, 0x00, 0x62, 0x00, 0x00, 0x00, 0x63, 0x00, 0x00, 0x00 }));
				yield return (new TestCaseData(Encoding.UTF32, "äöü", new byte[] { 0xE4, 0x00, 0x00, 0x00, 0xF6, 0x00, 0x00, 0x00, 0xFC, 0x00, 0x00, 0x00 }));
				yield return (new TestCaseData(Encoding.UTF32, "ÄÖÜ", new byte[] { 0xC4, 0x00, 0x00, 0x00, 0xD6, 0x00, 0x00, 0x00, 0xDC, 0x00, 0x00, 0x00 }));
				yield return (new TestCaseData(Encoding.UTF32, "$£€", new byte[] { 0x24, 0x00, 0x00, 0x00, 0xA3, 0x00, 0x00, 0x00, 0xAC, 0x20, 0x00, 0x00 }));
				yield return (new TestCaseData(Encoding.UTF32, "čěř", new byte[] { 0x0D, 0x01, 0x00, 0x00, 0x1B, 0x01, 0x00, 0x00, 0x59, 0x01, 0x00, 0x00 }));
			}
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class ParserTest
	{
		#region Tests
		//==========================================================================================
		// Test
		//==========================================================================================

		#region Tests > Parser
		//------------------------------------------------------------------------------------------
		// Tests > Parser
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, TestCaseSource(typeof(ParserTestData), "TestCases")]
		public virtual void TestParser(string inputString, byte[] expectedBytes)
		{
			Domain.Parser.Parser parser = new Domain.Parser.Parser();
			byte[] actualBytes = parser.Parse(inputString);
			Assert.AreEqual(expectedBytes, actualBytes);
		}

		#endregion

		#region Tests > Encoding
		//------------------------------------------------------------------------------------------
		// Tests > Encoding
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, TestCaseSource(typeof(ParserTestData), "TestCasesEncoding")]
		public virtual void TestParserEncoding(Encoding encoding, string inputString, byte[] expectedBytes)
		{
			Domain.Parser.Parser parser = new Domain.Parser.Parser(encoding);
			byte[] actualBytes = parser.Parse(inputString);
			Assert.AreEqual(expectedBytes, actualBytes);
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
