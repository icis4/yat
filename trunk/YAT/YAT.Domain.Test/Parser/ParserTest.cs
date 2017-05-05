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
// YAT 2.0 Gamma 3 Development Version 1.99.53
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

using System;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using MKY.Text;

using NUnit.Framework;

#endregion

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
				// Mixed:
				yield return (new TestCaseData(@"A\s(B \h(88) <CR>)<LF>", new byte[] { 0x41, 0x42, 0x20, 0x88, 0x20, 0x0D, 0x0A } ).SetName("Mixed"));

				// Empty:
				yield return (new TestCaseData(@"", new byte[] { }).SetName("Empty"));

				// Whitespace (note these escapes are already interpreted by the C# compiler):
				yield return (new TestCaseData("\0",	new byte[] { 0x00 } ).SetName("Whitespace Null"));
				yield return (new TestCaseData("\a",	new byte[] { 0x07 } ).SetName("Whitespace Bell"));
				yield return (new TestCaseData("\b",	new byte[] { 0x08 } ).SetName("Whitespace Backspace"));
				yield return (new TestCaseData("	",	new byte[] { 0x09 } ).SetName("Whitespace Tab"));

				// C-style escape:
				yield return (new TestCaseData(@"\0",		new byte[] { 0x00 } ).SetName("C-style <NUL>"));
				yield return (new TestCaseData(@"\0ABC",	new byte[] { 0x00, 0x41, 0x42, 0x43 }).SetName("C-style <NUL> with following characters"));
				yield return (new TestCaseData(@"\a",		new byte[] { 0x07 } ).SetName("C-style <BEL>"));
				yield return (new TestCaseData(@"\A",		new byte[] { 0x07 } ).SetName("C-style capital <BEL>"));
				yield return (new TestCaseData(@"\b",		new byte[] { 0x08 } ).SetName("C-style <BS>"));
				yield return (new TestCaseData(@"\bABC",	new byte[] { 0x08, 0x41, 0x42, 0x43 }).SetName("C-style <BS> with following characters"));
				yield return (new TestCaseData(@"\t",		new byte[] { 0x09 } ).SetName("C-style <TAB>"));
				yield return (new TestCaseData(@"\v",		new byte[] { 0x0B } ).SetName("C-style <VT>"));
				yield return (new TestCaseData(@"\r",		new byte[] { 0x0D } ).SetName("C-style <CR>"));
				yield return (new TestCaseData(@"\n",		new byte[] { 0x0A } ).SetName("C-style <LF>"));
				yield return (new TestCaseData(@"\r\n",		new byte[] { 0x0D, 0x0A } ).SetName("C-style <CR><LF>"));
				yield return (new TestCaseData(@"\r \n",	new byte[] { 0x0D, 0x20, 0x0A }).SetName("C-style <CR> <LF>"));
				yield return (new TestCaseData(@"\R\N",		new byte[] { 0x0D, 0x0A }).SetName("C-style <CR><LF> capital"));
				yield return (new TestCaseData(@"\f",		new byte[] { 0x0C } ).SetName("C-style <FF>"));
				yield return (new TestCaseData(@"C-style\r\nis like\tthis", new byte[] { 0x43, 0x2D, 0x73, 0x74, 0x79, 0x6C, 0x65, 0x0D, 0x0A, 0x69, 0x73, 0x20, 0x6C, 0x69, 0x6B, 0x65, 0x09, 0x74, 0x68, 0x69, 0x73 }).SetName("C-style string"));
				yield return (new TestCaseData(@"\0b",			new byte[] {      }).SetName("C-style binary value empty"));
				yield return (new TestCaseData(@"\0b1",			new byte[] { 0x01 }).SetName("C-style binary value 1 digit"));
				yield return (new TestCaseData(@"\0b01",		new byte[] { 0x01 }).SetName("C-style binary value 1 digit with additional 0"));
				yield return (new TestCaseData(@"\0b11",		new byte[] { 0x03 }).SetName("C-style binary value 2 digits"));
				yield return (new TestCaseData(@"\0b10101010",	new byte[] { 0xAA }).SetName("C-style binary value 8 digits"));
				yield return (new TestCaseData(@"\0b1010101000110011", new byte[] { 0xAA, 0x33 }).SetName("C-style binary value 16 digits"));
				yield return (new TestCaseData(@"\0",			new byte[] { 0x00 }).SetName("C-style octal value empty")); // Must result in zero as this is same as C-style <NUL>!
				yield return (new TestCaseData(@"\01",			new byte[] {    1 }).SetName("C-style octal value 1 digit")); // Note that C# doesn't support 0... notation!
				yield return (new TestCaseData(@"\001",			new byte[] {    1 }).SetName("C-style octal value 1 digit with additional 0"));
				yield return (new TestCaseData(@"\012",			new byte[] {   10 }).SetName("C-style octal value 2 digits"));
				yield return (new TestCaseData(@"\0123",		new byte[] {   83 }).SetName("C-style octal value 3 digits"));
				yield return (new TestCaseData(@"\01234",		new byte[] {   83, 4 }).SetName("C-style octal value 4 digits"));
				yield return (new TestCaseData(@"\012345",		new byte[] {   83, 37 }).SetName("C-style octal value 5 digits"));
				yield return (new TestCaseData(@"\0123456",		new byte[] {   83, 37, 6 }).SetName("C-style octal value 6 digits"));
				yield return (new TestCaseData(@"\1",			new byte[] {    1 }).SetName("C-style decimal value 1 digit"));
				yield return (new TestCaseData(@"\12",			new byte[] {   12 }).SetName("C-style decimal value 2 digits"));
				yield return (new TestCaseData(@"\123",			new byte[] {  123 }).SetName("C-style decimal value 3 digits"));
				yield return (new TestCaseData(@"\1234",		new byte[] {  123, 4 }).SetName("C-style decimal value 4 digits"));
				yield return (new TestCaseData(@"\12345",		new byte[] {  123, 45 }).SetName("C-style decimal value 5 digits"));
				yield return (new TestCaseData(@"\123456",		new byte[] {  123, 45, 6 }).SetName("C-style decimal value 6 digits"));
				yield return (new TestCaseData(@"\0x",			new byte[] {      }).SetName("C-style hexadecimal value 0x empty"));
				yield return (new TestCaseData(@"\0xA",			new byte[] { 0x0A }).SetName("C-style hexadecimal value 0xA"));
				yield return (new TestCaseData(@"\0x0A",		new byte[] { 0x0A }).SetName("C-style hexadecimal value 0xA with additional 0"));
				yield return (new TestCaseData(@"\0x1A",		new byte[] { 0x1A }).SetName("C-style hexadecimal value 0x1A"));
				yield return (new TestCaseData(@"\x",			new byte[] {      }).SetName("C-style hexadecimal value x empty"));
				yield return (new TestCaseData(@"\x1A",			new byte[] { 0x1A }).SetName("C-style hexadecimal value x1A"));
				yield return (new TestCaseData(@"\x1A2B",		new byte[] { 0x1A, 0x2B }).SetName("C-style hexadecimal value x 16 bits"));
				yield return (new TestCaseData(@"\x1A2",		new byte[] { 0x1A, 0x2  }).SetName("C-style hexadecimal value x 16 bits odd"));
				yield return (new TestCaseData(@"\x1A2B3C",		new byte[] { 0x1A, 0x2B, 0x3C }).SetName("C-style hexadecimal value x 24 bits"));
				yield return (new TestCaseData(@"\x1A2B3",		new byte[] { 0x1A, 0x2B, 0x3  }).SetName("C-style hexadecimal value x 24 bits odd"));
				yield return (new TestCaseData(@"\x1A2B3C4D",	new byte[] { 0x1A, 0x2B, 0x3C, 0x4D }).SetName("C-style hexadecimal value x 32 bits"));
				yield return (new TestCaseData(@"\x1A2B3C4",	new byte[] { 0x1A, 0x2B, 0x3C, 0x4  }).SetName("C-style hexadecimal value x 32 bits odd"));
				yield return (new TestCaseData(@"\x1A\x1B",		new byte[] { 0x1A, 0x1B }).SetName("Subsequent C-style value"));
				yield return (new TestCaseData(@"\x1A\n",		new byte[] { 0x1A, 0x0A }).SetName("Subsequent C-style escape"));
				yield return (new TestCaseData(@"\x1A<LF>",		new byte[] { 0x1A, 0x0A }).SetName("Subsequent ASCII value"));
				yield return (new TestCaseData(@"\x1A\h(0A)",	new byte[] { 0x1A, 0x0A }).SetName("Subsequent YAT value"));
				yield return (new TestCaseData(@"\xFFFE",		new byte[] { 0xFF, 0xFE }).SetName("C-style hexadecimal Unicode BOM"));

				// ASCII:
				yield return (new TestCaseData(@"<NUL>",		new byte[] { 0x00 } ).SetName("ASCII <NUL>"));
				yield return (new TestCaseData(@"<BEL>",		new byte[] { 0x07 } ).SetName("ASCII <BEL>"));
				yield return (new TestCaseData(@"<BS>",			new byte[] { 0x08 } ).SetName("ASCII <BS>"));
				yield return (new TestCaseData(@"<TAB>",		new byte[] { 0x09 } ).SetName("ASCII <TAB>"));
				yield return (new TestCaseData(@"<CR>",			new byte[] { 0x0D } ).SetName("ASCII <CR>"));
				yield return (new TestCaseData(@"<LF>",			new byte[] { 0x0A } ).SetName("ASCII <LF>"));
				yield return (new TestCaseData(@"<CR><LF>",		new byte[] { 0x0D, 0x0A } ).SetName("ASCII <CR><LF>"));
				yield return (new TestCaseData(@"<CR LF>",		new byte[] { 0x0D, 0x0A } ).SetName("ASCII <CR LF>"));
				yield return (new TestCaseData(@"<CRLF>",		new byte[] { 0x0D, 0x0A } ).SetName("ASCII <CRLF>"));
				yield return (new TestCaseData(@"<CR> <LF>",	new byte[] { 0x0D, 0x20, 0x0A } ).SetName("ASCII <CR> <LF>"));
				yield return (new TestCaseData(@"Empty <>",		new byte[] { 0x45, 0x6D, 0x70, 0x74, 0x79, 0x20 } ).SetName("ASCII Empty <>"));
				yield return (new TestCaseData(@"<XOn>",		new byte[] { 0x11 } ).SetName("ASCII <XOn>"));
				yield return (new TestCaseData(@"<XOff>",		new byte[] { 0x13 } ).SetName("ASCII <XOff>"));
				yield return (new TestCaseData(@"<ESC>",		new byte[] { 0x1B } ).SetName("ASCII <ESC>"));
				yield return (new TestCaseData(@"<cr><lf>",		new byte[] { 0x0D, 0x0A } ).SetName("ASCII <cr><lf>"));
				yield return (new TestCaseData(@"<cR><Lf>",		new byte[] { 0x0D, 0x0A } ).SetName("ASCII <cR><Lf>"));

				// Parentheses & Co:
				yield return (new TestCaseData(@"Hello (par)",				new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x28, 0x70, 0x61, 0x72, 0x29 } ).SetName("Parentheses"));
				yield return (new TestCaseData(@"\s(\(par\))",				new byte[] { 0x28, 0x70, 0x61, 0x72, 0x29 }             ).SetName("Parentheses in string short 1"));
				yield return (new TestCaseData(@"\s( \(par\) )",			new byte[] { 0x20, 0x28, 0x70, 0x61, 0x72, 0x29, 0x20 } ).SetName("Parentheses in string short 2"));
				yield return (new TestCaseData(@"\s(\( par \))",			new byte[] { 0x28, 0x20, 0x70, 0x61, 0x72, 0x20, 0x29 } ).SetName("Parentheses in string short 3"));
				yield return (new TestCaseData(@"Hello \<angle\> brackets",	new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x3C, 0x61, 0x6E, 0x67, 0x6C, 0x65, 0x3E, 0x20, 0x62, 0x72, 0x61, 0x63, 0x6B, 0x65, 0x74, 0x73 } ).SetName("Angle brackets"));
				yield return (new TestCaseData(@"\s(\<angle\>)",			new byte[] { 0x3C, 0x61, 0x6E, 0x67, 0x6C, 0x65, 0x3E }             ).SetName("Angle brackets in string short 1"));
				yield return (new TestCaseData(@"\s( \<angle\> )",			new byte[] { 0x20, 0x3C, 0x61, 0x6E, 0x67, 0x6C, 0x65, 0x3E, 0x20 } ).SetName("Angle brackets in string short 2"));
				yield return (new TestCaseData(@"\s(\< angle \>)",			new byte[] { 0x3C, 0x20, 0x61, 0x6E, 0x67, 0x6C, 0x65, 0x20, 0x3E } ).SetName("Angle brackets in string short 3"));

				// Backslashes:
				yield return (new TestCaseData(@"\\",										new byte[] { 0x5C } ).SetName("Backslash"));
				yield return (new TestCaseData(@"\\\\",										new byte[] { 0x5C, 0x5C } ).SetName("Backslash double"));
				yield return (new TestCaseData(@"Hello \\back\\ slashes",					new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x5C, 0x62, 0x61, 0x63, 0x6B, 0x5C, 0x20, 0x73, 0x6C, 0x61, 0x73, 0x68, 0x65, 0x73 } ).SetName("Backslashes"));
				yield return (new TestCaseData(@"Hello \92back\92 slashes",					new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x5C, 0x62, 0x61, 0x63, 0x6B, 0x5C, 0x20, 0x73, 0x6C, 0x61, 0x73, 0x68, 0x65, 0x73 } ).SetName("Backslashes C-style"));
				yield return (new TestCaseData(@"Hello \\\\double back\\\\ slashes",		new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x5C, 0x5C, 0x64, 0x6F, 0x75, 0x62, 0x6C, 0x65, 0x20, 0x62, 0x61, 0x63, 0x6B, 0x5C, 0x5C, 0x20, 0x73, 0x6C, 0x61, 0x73, 0x68, 0x65, 0x73 } ).SetName("Backslashes double"));
 				yield return (new TestCaseData(@"Hello \92\92double back\92\92 slashes",	new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x5C, 0x5C, 0x64, 0x6F, 0x75, 0x62, 0x6C, 0x65, 0x20, 0x62, 0x61, 0x63, 0x6B, 0x5C, 0x5C, 0x20, 0x73, 0x6C, 0x61, 0x73, 0x68, 0x65, 0x73 } ).SetName("Backslashes double C-style"));
				yield return (new TestCaseData(@"Hello \\\92double back\92\\ slashes",		new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x5C, 0x5C, 0x64, 0x6F, 0x75, 0x62, 0x6C, 0x65, 0x20, 0x62, 0x61, 0x63, 0x6B, 0x5C, 0x5C, 0x20, 0x73, 0x6C, 0x61, 0x73, 0x68, 0x65, 0x73 } ).SetName("Backslashes double combined"));

				// Char:
				yield return (new TestCaseData(@"Single char \c(9)",				new byte[] { 0x53, 0x69, 0x6E, 0x67, 0x6C, 0x65, 0x20, 0x63, 0x68, 0x61, 0x72, 0x20, 0x39 } ).SetName("Char single '9'"));
				yield return (new TestCaseData(@"Single char \c(.)",				new byte[] { 0x53, 0x69, 0x6E, 0x67, 0x6C, 0x65, 0x20, 0x63, 0x68, 0x61, 0x72, 0x20, 0x2E } ).SetName("Char single '.'"));
				yield return (new TestCaseData(@"Hello \c(()par\c()) OK",			new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x28, 0x70, 0x61, 0x72, 0x29, 0x20, 0x4F, 0x4B } ).SetName("Char parenthesis 1"));
				yield return (new TestCaseData(@"\c(H)\c(e)llo \c(()par\c()) OK",	new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x28, 0x70, 0x61, 0x72, 0x29, 0x20, 0x4F, 0x4B } ).SetName("Char parenthesis 2"));
				yield return (new TestCaseData(@"Empty \c()",						new byte[] { 0x45, 0x6D, 0x70, 0x74, 0x79, 0x20 } ).SetName("Char empty"));

				// Bin:
				yield return (new TestCaseData(@"\b()",								new byte[] { }).SetName("Bin empty"));
				yield return (new TestCaseData(@"\b(   )",							new byte[] { }).SetName("Bin still empty"));
				yield return (new TestCaseData(@"\b(00)",							new byte[] { 0x00 } ).SetName("Bin 00"));
				yield return (new TestCaseData(@"\b(01)",							new byte[] { 0x01 } ).SetName("Bin 01"));
				yield return (new TestCaseData(@"\b(00000000)",						new byte[] { 0x00 } ).SetName("Bin 00000000"));
				yield return (new TestCaseData(@"\b(00000001)",						new byte[] { 0x01 } ).SetName("Bin 00000001"));
				yield return (new TestCaseData(@"\b(11111111)",						new byte[] { 0xFF } ).SetName("Bin 11111111"));
				yield return (new TestCaseData(@"\b(0000000000000000)",				new byte[] { 0x00, 0x00 } ).SetName("Bin 0000000000000000"));
				yield return (new TestCaseData(@"\b(00000000 00000000)",			new byte[] { 0x00, 0x00 } ).SetName("Bin 00000000 00000000"));
				yield return (new TestCaseData(@"\b(0000000000000001)",				new byte[] { 0x00, 0x01 } ).SetName("Bin 0000000000000001"));
				yield return (new TestCaseData(@"\b(00000000 00000001)",			new byte[] { 0x00, 0x01 } ).SetName("Bin 00000000 00000001"));
				yield return (new TestCaseData(@"\b(0000000100000000)",				new byte[] { 0x01, 0x00 } ).SetName("Bin 0000000100000000"));
				yield return (new TestCaseData(@"\b(00000001 00000000)",			new byte[] { 0x01, 0x00 } ).SetName("Bin 00000001 00000000"));
				yield return (new TestCaseData(@"\b(1111111111111111)",				new byte[] { 0xFF, 0xFF } ).SetName("Bin 1111111111111111"));
				yield return (new TestCaseData(@"\b(11111111 11111111)",			new byte[] { 0xFF, 0xFF } ).SetName("Bin 11111111 11111111"));
				yield return (new TestCaseData(@"\b(00100011 01011101 00100100)",	new byte[] { 0x23, 0x5D, 0x24, } ).SetName("Bin sequence 1"));
				yield return (new TestCaseData(@"\b(001000110101110100100100)",		new byte[] { 0x23, 0x5D, 0x24, } ).SetName("Bin sequence 2"));
				yield return (new TestCaseData(@"\b(0010001101011101001001)",		new byte[] { 0x23, 0x5D, 0x09, } ).SetName("Bin sequence 3"));
				yield return (new TestCaseData(@"\b( 01)",							new byte[] { 0x01 } ).SetName("Bin 01 leading space"));
				yield return (new TestCaseData(@"\b(01 )",							new byte[] { 0x01 } ).SetName("Bin 01 trailing space"));
				yield return (new TestCaseData(@"\b(   01   )",						new byte[] { 0x01 } ).SetName("Bin 01 leading and trailing space"));

				// Oct:
				yield return (new TestCaseData(@"\o()",								new byte[] { }).SetName("Oct empty"));
				yield return (new TestCaseData(@"\o(   )",							new byte[] { }).SetName("Oct still empty"));
				yield return (new TestCaseData(@"\o(0)",							new byte[] { 0x00 } ).SetName("Oct 0"));
				yield return (new TestCaseData(@"\o(1)",							new byte[] { 0x01 } ).SetName("Oct 1"));
				yield return (new TestCaseData(@"\o(00)",							new byte[] { 0x00 } ).SetName("Oct 00"));
				yield return (new TestCaseData(@"\o(01)",							new byte[] { 0x01 } ).SetName("Oct 01"));
				yield return (new TestCaseData(@"\o(377)",							new byte[] { 0xFF } ).SetName("Oct 377"));
				yield return (new TestCaseData(@"\o(000000)",						new byte[] { 0x00, 0x00 } ).SetName("Oct 000000"));
				yield return (new TestCaseData(@"\o(000 000)",						new byte[] { 0x00, 0x00 } ).SetName("Oct 000 000"));
				yield return (new TestCaseData(@"\o(000001)",						new byte[] { 0x00, 0x01 } ).SetName("Oct 000001"));
				yield return (new TestCaseData(@"\o(000 001)",						new byte[] { 0x00, 0x01 } ).SetName("Oct 000 001"));
				yield return (new TestCaseData(@"\o(001000)",						new byte[] { 0x01, 0x00 } ).SetName("Oct 001000"));
				yield return (new TestCaseData(@"\o(001 000)",						new byte[] { 0x01, 0x00 } ).SetName("Oct 001 000"));
				yield return (new TestCaseData(@"\o(377377)",						new byte[] { 0xFF, 0xFF } ).SetName("Oct 377377"));
				yield return (new TestCaseData(@"\o(377 377)",						new byte[] { 0xFF, 0xFF } ).SetName("Oct 377 377"));
				yield return (new TestCaseData(@"\o(43 135 44)",					new byte[] { 0x23, 0x5D, 0x24, } ).SetName("Oct sequence 1"));
				yield return (new TestCaseData(@"\o(4313544)",						new byte[] { 0x23, 0x5D, 0x24, } ).SetName("Oct sequence 2"));
				yield return (new TestCaseData(@"\o(4313511)",						new byte[] { 0x23, 0x5D, 0x09, } ).SetName("Oct sequence 3"));
				yield return (new TestCaseData(@"\o( 01)",							new byte[] { 0x01 } ).SetName("Oct 01 leading space"));
				yield return (new TestCaseData(@"\o(01 )",							new byte[] { 0x01 } ).SetName("Oct 01 trailing space"));
				yield return (new TestCaseData(@"\o(   01   )",						new byte[] { 0x01 } ).SetName("Oct 01 leading and trailing space"));

				// Dec:
				yield return (new TestCaseData(@"\d()",								new byte[] { }).SetName("Dec empty"));
				yield return (new TestCaseData(@"\d(   )",							new byte[] { }).SetName("Dec still empty"));
				yield return (new TestCaseData(@"\d(0)",							new byte[] { 0x00 } ).SetName("Dec 0"));
				yield return (new TestCaseData(@"\d(1)",							new byte[] { 0x01 } ).SetName("Dec 1"));
				yield return (new TestCaseData(@"\d(00)",							new byte[] { 0x00 } ).SetName("Dec 00"));
				yield return (new TestCaseData(@"\d(01)",							new byte[] { 0x01 } ).SetName("Dec 01"));
				yield return (new TestCaseData(@"\d(255)",							new byte[] { 0xFF } ).SetName("Dec 255"));
				yield return (new TestCaseData(@"\d(000000)",						new byte[] { 0x00, 0x00 } ).SetName("Dec 000000"));
				yield return (new TestCaseData(@"\d(000 000)",						new byte[] { 0x00, 0x00 } ).SetName("Dec 000 000"));
				yield return (new TestCaseData(@"\d(000001)",						new byte[] { 0x00, 0x01 } ).SetName("Dec 000001"));
				yield return (new TestCaseData(@"\d(000 001)",						new byte[] { 0x00, 0x01 } ).SetName("Dec 000 001"));
				yield return (new TestCaseData(@"\d(001000)",						new byte[] { 0x01, 0x00 } ).SetName("Dec 001000"));
				yield return (new TestCaseData(@"\d(001 000)",						new byte[] { 0x01, 0x00 } ).SetName("Dec 001 000"));
				yield return (new TestCaseData(@"\d(255255)",						new byte[] { 0xFF, 0xFF } ).SetName("Dec 255255"));
				yield return (new TestCaseData(@"\d(255 255)",						new byte[] { 0xFF, 0xFF } ).SetName("Dec 255 255"));
				yield return (new TestCaseData(@"\d(35 93 36)",						new byte[] { 0x23, 0x5D, 0x24, } ).SetName("Dec sequence 1"));
				yield return (new TestCaseData(@"\d(359336)",						new byte[] { 0x23, 0x5D, 0x24, } ).SetName("Dec sequence 2"));
				yield return (new TestCaseData(@"\d(35939)",						new byte[] { 0x23, 0x5D, 0x09, } ).SetName("Dec sequence 3"));
				yield return (new TestCaseData(@"\d( 01)",							new byte[] { 0x01 } ).SetName("Dec 01 leading space"));
				yield return (new TestCaseData(@"\d(01 )",							new byte[] { 0x01 } ).SetName("Dec 01 trailing space"));
				yield return (new TestCaseData(@"\d(   01   )",						new byte[] { 0x01 } ).SetName("Dec 01 leading and trailing space"));

				// Hex:
				yield return (new TestCaseData(@"\h()",								new byte[] { } ).SetName("Hex empty"));
				yield return (new TestCaseData(@"\h(   )",							new byte[] { } ).SetName("Hex still empty"));
				yield return (new TestCaseData(@"\h(00)",							new byte[] { 0x00 } ).SetName("Hex 00"));
				yield return (new TestCaseData(@"\h(01)",							new byte[] { 0x01 } ).SetName("Hex 01"));
				yield return (new TestCaseData(@"\h(0A)",							new byte[] { 0x0A } ).SetName("Hex 0A"));
				yield return (new TestCaseData(@"\h(A)",							new byte[] { 0x0A } ).SetName("Hex A without leading 0"));
				yield return (new TestCaseData(@"\h(20)",							new byte[] { 0x20 } ).SetName("Hex 20"));
				yield return (new TestCaseData(@"\h(7F)",							new byte[] { 0x7F } ).SetName("Hex 7F"));
				yield return (new TestCaseData(@"\h(80)",							new byte[] { 0x80 } ).SetName("Hex 80"));
				yield return (new TestCaseData(@"\h(81)",							new byte[] { 0x81 } ).SetName("Hex 81"));
				yield return (new TestCaseData(@"\h(AA)",							new byte[] { 0xAA } ).SetName("Hex AA"));
				yield return (new TestCaseData(@"\h(FE)",							new byte[] { 0xFE } ).SetName("Hex FE"));
				yield return (new TestCaseData(@"\h(FF)",							new byte[] { 0xFF } ).SetName("Hex FF"));
				yield return (new TestCaseData(@"\h(0000)",							new byte[] { 0x00, 0x00 } ).SetName("Hex 0000"));
				yield return (new TestCaseData(@"\h(00 00)",						new byte[] { 0x00, 0x00 } ).SetName("Hex 00 00"));
				yield return (new TestCaseData(@"\h(0001)",							new byte[] { 0x00, 0x01 } ).SetName("Hex 0001"));
				yield return (new TestCaseData(@"\h(00 01)",						new byte[] { 0x00, 0x01 } ).SetName("Hex 00 01"));
				yield return (new TestCaseData(@"\h(0100)",							new byte[] { 0x01, 0x00 } ).SetName("Hex 0100"));
				yield return (new TestCaseData(@"\h(01 00)",						new byte[] { 0x01, 0x00 } ).SetName("Hex 01 00"));
				yield return (new TestCaseData(@"\h(FFFF)",							new byte[] { 0xFF, 0xFF } ).SetName("Hex FFFF"));
				yield return (new TestCaseData(@"\h(FF FF)",						new byte[] { 0xFF, 0xFF } ).SetName("Hex FF FF"));
				yield return (new TestCaseData(@"\h(00 01 20 7F 80 81 AA FE FF)",	new byte[] { 0x00, 0x01, 0x20, 0x7F, 0x80, 0x81, 0xAA, 0xFE, 0xFF } ).SetName("Hex sequence 1"));
				yield return (new TestCaseData(@"\h(23 5D 24 81 20 A5)",			new byte[] { 0x23, 0x5D, 0x24, 0x81, 0x20, 0xA5 } ).SetName("Hex sequence 2"));
				yield return (new TestCaseData(@"\h(235D248120A5)",					new byte[] { 0x23, 0x5D, 0x24, 0x81, 0x20, 0xA5 } ).SetName("Hex sequence 3"));
				yield return (new TestCaseData(@"\h(235D2)",						new byte[] { 0x23, 0x5D, 0x02 } ).SetName("Hex sequence 4"));
				yield return (new TestCaseData(@"\h(00 \h(FF) 00)",					new byte[] { 0x00, 0xFF, 0x00 } ).SetName("Hex nested"));
				yield return (new TestCaseData(@"\h( 01)",							new byte[] { 0x01 } ).SetName("Hex 01 leading space"));
				yield return (new TestCaseData(@"\h(01 )",							new byte[] { 0x01 } ).SetName("Hex 01 trailing space"));
				yield return (new TestCaseData(@"\h(   01   )",						new byte[] { 0x01 } ).SetName("Hex 01 leading and trailing space"));

				// EOL:
				yield return (new TestCaseData(@"A<CR><CR><LF>B<CR><LF><LF>C<CR><LF>D<CR>E<LF>F", new byte[] { 0x41, 0x0D, 0x0D, 0x0A, 0x42, 0x0D, 0x0A, 0x0A, 0x43, 0x0D, 0x0A, 0x44, 0x0D, 0x45, 0x0A, 0x46 } ).SetName("Partial EOL"));
			}
		}

		#endregion
	}

	/// <summary></summary>
	public static class EncodingTestData
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
				// ASCII:
				yield return (new TestCaseData(Encoding.ASCII, Endianness.LittleEndian, "abc", new byte[] { 0x61, 0x62, 0x63 }));
				yield return (new TestCaseData(Encoding.ASCII, Endianness.BigEndian,    "abc", new byte[] { 0x61, 0x62, 0x63 }));

				// UTF-8:
				yield return (new TestCaseData(Encoding.UTF8, Endianness.LittleEndian, "abc", new byte[] { 0x61, 0x62, 0x63 }));
				yield return (new TestCaseData(Encoding.UTF8, Endianness.LittleEndian, "äöü", new byte[] { 0xC3, 0xA4, 0xC3, 0xB6, 0xC3, 0xBC }));
				yield return (new TestCaseData(Encoding.UTF8, Endianness.LittleEndian, "ÄÖÜ", new byte[] { 0xC3, 0x84, 0xC3, 0x96, 0xC3, 0x9C }));
				yield return (new TestCaseData(Encoding.UTF8, Endianness.LittleEndian, "$£€", new byte[] { 0x24, 0xC2, 0xA3, 0xE2, 0x82, 0xAC })); // 1-2-3 bytes !!!
				yield return (new TestCaseData(Encoding.UTF8, Endianness.LittleEndian, "čěř", new byte[] { 0xC4, 0x8D, 0xC4, 0x9B, 0xC5, 0x99 }));
				yield return (new TestCaseData(Encoding.UTF8, Endianness.BigEndian,    "abc", new byte[] { 0x61, 0x62, 0x63 }));
				yield return (new TestCaseData(Encoding.UTF8, Endianness.BigEndian,    "äöü", new byte[] { 0xA4, 0xC3, 0xB6, 0xC3, 0xBC, 0xC3 }));
				yield return (new TestCaseData(Encoding.UTF8, Endianness.BigEndian,    "ÄÖÜ", new byte[] { 0x84, 0xC3, 0x96, 0xC3, 0x9C, 0xC3 }));
				yield return (new TestCaseData(Encoding.UTF8, Endianness.BigEndian,    "$£€", new byte[] { 0x24, 0xA3, 0xC2, 0xAC, 0x82, 0xE2 })); // 1-2-3 bytes !!!
				yield return (new TestCaseData(Encoding.UTF8, Endianness.BigEndian,    "čěř", new byte[] { 0x8D, 0xC4, 0x9B, 0xC4, 0x99, 0xC5 }));

				yield return (new TestCaseData(Encoding.UTF8, Endianness.LittleEndian, @"\0<CR>\n", new byte[] { 0x00, 0x0D, 0x0A }));
				yield return (new TestCaseData(Encoding.UTF8, Endianness.BigEndian,    @"\0<CR>\n", new byte[] { 0x00, 0x0D, 0x0A }));

				// UTF-16 (little endian, i.e. machine endianness):
				yield return (new TestCaseData(Encoding.Unicode, Endianness.LittleEndian, "abc", new byte[] { 0x61, 0x00, 0x62, 0x00, 0x63, 0x00 }));
				yield return (new TestCaseData(Encoding.Unicode, Endianness.LittleEndian, "äöü", new byte[] { 0xE4, 0x00, 0xF6, 0x00, 0xFC, 0x00 }));
				yield return (new TestCaseData(Encoding.Unicode, Endianness.LittleEndian, "ÄÖÜ", new byte[] { 0xC4, 0x00, 0xD6, 0x00, 0xDC, 0x00 }));
				yield return (new TestCaseData(Encoding.Unicode, Endianness.LittleEndian, "$£€", new byte[] { 0x24, 0x00, 0xA3, 0x00, 0xAC, 0x20 }));
				yield return (new TestCaseData(Encoding.Unicode, Endianness.LittleEndian, "čěř", new byte[] { 0x0D, 0x01, 0x1B, 0x01, 0x59, 0x01 }));
				yield return (new TestCaseData(Encoding.Unicode, Endianness.BigEndian,    "abc", new byte[] { 0x00, 0x61, 0x00, 0x62, 0x00, 0x63 }));
				yield return (new TestCaseData(Encoding.Unicode, Endianness.BigEndian,    "äöü", new byte[] { 0x00, 0xE4, 0x00, 0xF6, 0x00, 0xFC }));
				yield return (new TestCaseData(Encoding.Unicode, Endianness.BigEndian,    "ÄÖÜ", new byte[] { 0x00, 0xC4, 0x00, 0xD6, 0x00, 0xDC }));
				yield return (new TestCaseData(Encoding.Unicode, Endianness.BigEndian,    "$£€", new byte[] { 0x00, 0x24, 0x00, 0xA3, 0x20, 0xAC }));
				yield return (new TestCaseData(Encoding.Unicode, Endianness.BigEndian,    "čěř", new byte[] { 0x01, 0x0D, 0x01, 0x1B, 0x01, 0x59 }));

				yield return (new TestCaseData(Encoding.Unicode, Endianness.LittleEndian, @"\0<CR>\n", new byte[] { 0x00, 0x00, 0x0D, 0x00, 0x0A, 0x00 }));
				yield return (new TestCaseData(Encoding.Unicode, Endianness.BigEndian,    @"\0<CR>\n", new byte[] { 0x00, 0x00, 0x00, 0x0D, 0x00, 0x0A }));

				// UTF-16 (big endian, i.e. network endianness):
				yield return (new TestCaseData(Encoding.BigEndianUnicode, Endianness.LittleEndian, "abc", new byte[] { 0x00, 0x61, 0x00, 0x62, 0x00, 0x63 }));
				yield return (new TestCaseData(Encoding.BigEndianUnicode, Endianness.LittleEndian, "äöü", new byte[] { 0x00, 0xE4, 0x00, 0xF6, 0x00, 0xFC }));
				yield return (new TestCaseData(Encoding.BigEndianUnicode, Endianness.LittleEndian, "ÄÖÜ", new byte[] { 0x00, 0xC4, 0x00, 0xD6, 0x00, 0xDC }));
				yield return (new TestCaseData(Encoding.BigEndianUnicode, Endianness.LittleEndian, "$£€", new byte[] { 0x00, 0x24, 0x00, 0xA3, 0x20, 0xAC }));
				yield return (new TestCaseData(Encoding.BigEndianUnicode, Endianness.LittleEndian, "čěř", new byte[] { 0x01, 0x0D, 0x01, 0x1B, 0x01, 0x59 }));
				yield return (new TestCaseData(Encoding.BigEndianUnicode, Endianness.BigEndian,    "abc", new byte[] { 0x61, 0x00, 0x62, 0x00, 0x63, 0x00 }));
				yield return (new TestCaseData(Encoding.BigEndianUnicode, Endianness.BigEndian,    "äöü", new byte[] { 0xE4, 0x00, 0xF6, 0x00, 0xFC, 0x00 }));
				yield return (new TestCaseData(Encoding.BigEndianUnicode, Endianness.BigEndian,    "ÄÖÜ", new byte[] { 0xC4, 0x00, 0xD6, 0x00, 0xDC, 0x00 }));
				yield return (new TestCaseData(Encoding.BigEndianUnicode, Endianness.BigEndian,    "$£€", new byte[] { 0x24, 0x00, 0xA3, 0x00, 0xAC, 0x20 }));
				yield return (new TestCaseData(Encoding.BigEndianUnicode, Endianness.BigEndian,    "čěř", new byte[] { 0x0D, 0x01, 0x1B, 0x01, 0x59, 0x01 }));

				yield return (new TestCaseData(Encoding.BigEndianUnicode, Endianness.LittleEndian, @"\0<CR>\n", new byte[] { 0x00, 0x00, 0x00, 0x0D, 0x00, 0x0A }));
				yield return (new TestCaseData(Encoding.BigEndianUnicode, Endianness.BigEndian,    @"\0<CR>\n", new byte[] { 0x00, 0x00, 0x0D, 0x00, 0x0A, 0x00 }));

				// UTF-32 (little endian, i.e. machine endianness):
				yield return (new TestCaseData(Encoding.UTF32, Endianness.LittleEndian, "abc", new byte[] { 0x61, 0x00, 0x00, 0x00, 0x62, 0x00, 0x00, 0x00, 0x63, 0x00, 0x00, 0x00 }));
				yield return (new TestCaseData(Encoding.UTF32, Endianness.LittleEndian, "äöü", new byte[] { 0xE4, 0x00, 0x00, 0x00, 0xF6, 0x00, 0x00, 0x00, 0xFC, 0x00, 0x00, 0x00 }));
				yield return (new TestCaseData(Encoding.UTF32, Endianness.LittleEndian, "ÄÖÜ", new byte[] { 0xC4, 0x00, 0x00, 0x00, 0xD6, 0x00, 0x00, 0x00, 0xDC, 0x00, 0x00, 0x00 }));
				yield return (new TestCaseData(Encoding.UTF32, Endianness.LittleEndian, "$£€", new byte[] { 0x24, 0x00, 0x00, 0x00, 0xA3, 0x00, 0x00, 0x00, 0xAC, 0x20, 0x00, 0x00 }));
				yield return (new TestCaseData(Encoding.UTF32, Endianness.LittleEndian, "čěř", new byte[] { 0x0D, 0x01, 0x00, 0x00, 0x1B, 0x01, 0x00, 0x00, 0x59, 0x01, 0x00, 0x00 }));
				yield return (new TestCaseData(Encoding.UTF32, Endianness.BigEndian,    "abc", new byte[] { 0x00, 0x00, 0x00, 0x61, 0x00, 0x00, 0x00, 0x62, 0x00, 0x00, 0x00, 0x63 }));
				yield return (new TestCaseData(Encoding.UTF32, Endianness.BigEndian,    "äöü", new byte[] { 0x00, 0x00, 0x00, 0xE4, 0x00, 0x00, 0x00, 0xF6, 0x00, 0x00, 0x00, 0xFC }));
				yield return (new TestCaseData(Encoding.UTF32, Endianness.BigEndian,    "ÄÖÜ", new byte[] { 0x00, 0x00, 0x00, 0xC4, 0x00, 0x00, 0x00, 0xD6, 0x00, 0x00, 0x00, 0xDC }));
				yield return (new TestCaseData(Encoding.UTF32, Endianness.BigEndian,    "$£€", new byte[] { 0x00, 0x00, 0x00, 0x24, 0x00, 0x00, 0x00, 0xA3, 0x00, 0x00, 0x20, 0xAC }));
				yield return (new TestCaseData(Encoding.UTF32, Endianness.BigEndian,    "čěř", new byte[] { 0x00, 0x00, 0x01, 0x0D, 0x00, 0x00, 0x01, 0x1B, 0x00, 0x00, 0x01, 0x59 }));

				yield return (new TestCaseData(Encoding.UTF32, Endianness.LittleEndian, @"\0<CR>\n", new byte[] { 0x00, 0x00, 0x00, 0x00, 0x0D, 0x00, 0x00, 0x00, 0x0A, 0x00, 0x00, 0x00 }));
				yield return (new TestCaseData(Encoding.UTF32, Endianness.BigEndian,    @"\0<CR>\n", new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0D, 0x00, 0x00, 0x00, 0x0A }));

				// UTF-32 (big endian, i.e. network endianness):
				yield return (new TestCaseData(EncodingEx.GetEncoding(SupportedEncoding.UTF32BE), Endianness.LittleEndian, "abc", new byte[] { 0x00, 0x00, 0x00, 0x61, 0x00, 0x00, 0x00, 0x62, 0x00, 0x00, 0x00, 0x63 }));
				yield return (new TestCaseData(EncodingEx.GetEncoding(SupportedEncoding.UTF32BE), Endianness.LittleEndian, "äöü", new byte[] { 0x00, 0x00, 0x00, 0xE4, 0x00, 0x00, 0x00, 0xF6, 0x00, 0x00, 0x00, 0xFC }));
				yield return (new TestCaseData(EncodingEx.GetEncoding(SupportedEncoding.UTF32BE), Endianness.LittleEndian, "ÄÖÜ", new byte[] { 0x00, 0x00, 0x00, 0xC4, 0x00, 0x00, 0x00, 0xD6, 0x00, 0x00, 0x00, 0xDC }));
				yield return (new TestCaseData(EncodingEx.GetEncoding(SupportedEncoding.UTF32BE), Endianness.LittleEndian, "$£€", new byte[] { 0x00, 0x00, 0x00, 0x24, 0x00, 0x00, 0x00, 0xA3, 0x00, 0x00, 0x20, 0xAC }));
				yield return (new TestCaseData(EncodingEx.GetEncoding(SupportedEncoding.UTF32BE), Endianness.LittleEndian, "čěř", new byte[] { 0x00, 0x00, 0x01, 0x0D, 0x00, 0x00, 0x01, 0x1B, 0x00, 0x00, 0x01, 0x59 }));
				yield return (new TestCaseData(EncodingEx.GetEncoding(SupportedEncoding.UTF32BE), Endianness.BigEndian,    "abc", new byte[] { 0x61, 0x00, 0x00, 0x00, 0x62, 0x00, 0x00, 0x00, 0x63, 0x00, 0x00, 0x00 }));
				yield return (new TestCaseData(EncodingEx.GetEncoding(SupportedEncoding.UTF32BE), Endianness.BigEndian,    "äöü", new byte[] { 0xE4, 0x00, 0x00, 0x00, 0xF6, 0x00, 0x00, 0x00, 0xFC, 0x00, 0x00, 0x00 }));
				yield return (new TestCaseData(EncodingEx.GetEncoding(SupportedEncoding.UTF32BE), Endianness.BigEndian,    "ÄÖÜ", new byte[] { 0xC4, 0x00, 0x00, 0x00, 0xD6, 0x00, 0x00, 0x00, 0xDC, 0x00, 0x00, 0x00 }));
				yield return (new TestCaseData(EncodingEx.GetEncoding(SupportedEncoding.UTF32BE), Endianness.BigEndian,    "$£€", new byte[] { 0x24, 0x00, 0x00, 0x00, 0xA3, 0x00, 0x00, 0x00, 0xAC, 0x20, 0x00, 0x00 }));
				yield return (new TestCaseData(EncodingEx.GetEncoding(SupportedEncoding.UTF32BE), Endianness.BigEndian,    "čěř", new byte[] { 0x0D, 0x01, 0x00, 0x00, 0x1B, 0x01, 0x00, 0x00, 0x59, 0x01, 0x00, 0x00 }));

				yield return (new TestCaseData(EncodingEx.GetEncoding(SupportedEncoding.UTF32BE), Endianness.LittleEndian, @"\0<CR>\n", new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0D, 0x00, 0x00, 0x00, 0x0A }));
				yield return (new TestCaseData(EncodingEx.GetEncoding(SupportedEncoding.UTF32BE), Endianness.BigEndian,    @"\0<CR>\n", new byte[] { 0x00, 0x00, 0x00, 0x00, 0x0D, 0x00, 0x00, 0x00, 0x0A, 0x00, 0x00, 0x00 }));
			}
		}

		#endregion
	}

	/// <summary></summary>
	public static class KeywordTestData
	{
		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <remarks>
		/// So far, only integer arguments are supported.
		/// See <see cref="Domain.Parser.KeywordArgState"/> for more details.
		/// </remarks>
		private static IEnumerable TestCasesWithoutName
		{
			get
			{
				// Keywords:
				yield return (new TestCaseData(@"\!(Clear)",               Domain.Parser.Keyword.Clear,             null));
				yield return (new TestCaseData(@"\!(Clear())",             Domain.Parser.Keyword.Clear,             null));
				yield return (new TestCaseData(@"\!(Delay)",               Domain.Parser.Keyword.Delay,             null));
				yield return (new TestCaseData(@"\!(Delay())",             Domain.Parser.Keyword.Delay,             null));
				yield return (new TestCaseData(@"\!(Delay(10))",           Domain.Parser.Keyword.Delay,             new int[] { 10 }));
				yield return (new TestCaseData(@"\!(LineDelay)",           Domain.Parser.Keyword.LineDelay,         null));
				yield return (new TestCaseData(@"\!(LineDelay())",         Domain.Parser.Keyword.LineDelay,         null));
				yield return (new TestCaseData(@"\!(LineDelay(10))",       Domain.Parser.Keyword.LineDelay,         new int[] { 10 }));
				yield return (new TestCaseData(@"\!(LineInterval)",        Domain.Parser.Keyword.LineInterval,      null));
				yield return (new TestCaseData(@"\!(LineInterval())",      Domain.Parser.Keyword.LineInterval,      null));
				yield return (new TestCaseData(@"\!(LineInterval(10))",    Domain.Parser.Keyword.LineInterval,      new int[] { 10 }));
				yield return (new TestCaseData(@"\!(LineRepeat)",          Domain.Parser.Keyword.LineRepeat,        null));
				yield return (new TestCaseData(@"\!(LineRepeat())",        Domain.Parser.Keyword.LineRepeat,        null));
				yield return (new TestCaseData(@"\!(LineRepeat(10))",      Domain.Parser.Keyword.LineRepeat,        new int[] { 10 }));
				yield return (new TestCaseData(@"\!(Eol)",                 Domain.Parser.Keyword.Eol,               null));
				yield return (new TestCaseData(@"\!(Eol())",               Domain.Parser.Keyword.Eol,               null));
				yield return (new TestCaseData(@"\!(NoEol)",               Domain.Parser.Keyword.NoEol,             null));
				yield return (new TestCaseData(@"\!(NoEol())",             Domain.Parser.Keyword.NoEol,             null));
				yield return (new TestCaseData(@"\!(OutputBreakOn)",       Domain.Parser.Keyword.OutputBreakOn,     null));
				yield return (new TestCaseData(@"\!(OutputBreakOn())",     Domain.Parser.Keyword.OutputBreakOn,     null));
				yield return (new TestCaseData(@"\!(OutputBreakOff)",      Domain.Parser.Keyword.OutputBreakOff,    null));
				yield return (new TestCaseData(@"\!(OutputBreakOff())",    Domain.Parser.Keyword.OutputBreakOff,    null));
				yield return (new TestCaseData(@"\!(OutputBreakToggle)",   Domain.Parser.Keyword.OutputBreakToggle, null));
				yield return (new TestCaseData(@"\!(OutputBreakToggle())", Domain.Parser.Keyword.OutputBreakToggle, null));
				yield return (new TestCaseData(@"\!(ReportID)",            Domain.Parser.Keyword.ReportId,          null));
				yield return (new TestCaseData(@"\!(ReportID())",          Domain.Parser.Keyword.ReportId,          null));
				yield return (new TestCaseData(@"\!(ReportID(0xAA))",      Domain.Parser.Keyword.ReportId,          new int[] { 0xAA }));

				// Whitespace:
				yield return (new TestCaseData(@"\!( Delay)",            Domain.Parser.Keyword.Delay, null));
				yield return (new TestCaseData(@"\!(Delay )",            Domain.Parser.Keyword.Delay, null));
				yield return (new TestCaseData(@"\!( Delay )",           Domain.Parser.Keyword.Delay, null));
				yield return (new TestCaseData(@"\!(   Delay   )",       Domain.Parser.Keyword.Delay, null));
				yield return (new TestCaseData(@"\!(Delay (10))",        Domain.Parser.Keyword.Delay, new int[] { 10 }));
				yield return (new TestCaseData(@"\!(Delay(10) )",        Domain.Parser.Keyword.Delay, new int[] { 10 }));
				yield return (new TestCaseData(@"\!(Delay (10) )",       Domain.Parser.Keyword.Delay, new int[] { 10 }));
				yield return (new TestCaseData(@"\!(Delay   (10)   )",   Domain.Parser.Keyword.Delay, new int[] { 10 }));
				yield return (new TestCaseData(@"\!(Delay( 10))",        Domain.Parser.Keyword.Delay, new int[] { 10 }));
				yield return (new TestCaseData(@"\!(Delay(10 ))",        Domain.Parser.Keyword.Delay, new int[] { 10 }));
				yield return (new TestCaseData(@"\!(Delay( 10 ))",       Domain.Parser.Keyword.Delay, new int[] { 10 }));
				yield return (new TestCaseData(@"\!(Delay(   10   ))",   Domain.Parser.Keyword.Delay, new int[] { 10 }));

				// Empty:
				yield return (new TestCaseData(@"\!(Clear())",          Domain.Parser.Keyword.Clear, null));
				yield return (new TestCaseData(@"\!(Clear ( ) )",       Domain.Parser.Keyword.Clear, null));
				yield return (new TestCaseData(@"\!(Clear   (   )   )", Domain.Parser.Keyword.Clear, null));
				yield return (new TestCaseData(@"\!(Delay())",          Domain.Parser.Keyword.Delay, null));
				yield return (new TestCaseData(@"\!(Delay ( ) )",       Domain.Parser.Keyword.Delay, null));
				yield return (new TestCaseData(@"\!(Delay   (   )   )", Domain.Parser.Keyword.Delay, null));

				// Single:
				yield return (new TestCaseData(@"\!(Z_FIT(1))",             Domain.Parser.Keyword.Z_FIT, new int[] { 1 }));
				yield return (new TestCaseData(@"\!(Z_FIT ( 1 ) )",         Domain.Parser.Keyword.Z_FIT, new int[] { 1 }));
				yield return (new TestCaseData(@"\!(Z_FIT   (   1   )   )", Domain.Parser.Keyword.Z_FIT, new int[] { 1 }));

				// Multiple:
				yield return (new TestCaseData(@"\!(Z_FIT( 1 , 2 , 3 ))", Domain.Parser.Keyword.Z_FIT, new int[] { 1, 2, 3 }));
				yield return (new TestCaseData(@"\!(Z_FIT(1 , 2 , 3))",   Domain.Parser.Keyword.Z_FIT, new int[] { 1, 2, 3 }));
				yield return (new TestCaseData(@"\!(Z_FIT( 1 ,2 ,3 ))",   Domain.Parser.Keyword.Z_FIT, new int[] { 1, 2, 3 }));
				yield return (new TestCaseData(@"\!(Z_FIT(1 ,2 ,3))",     Domain.Parser.Keyword.Z_FIT, new int[] { 1, 2, 3 }));
				yield return (new TestCaseData(@"\!(Z_FIT( 1,2,3 ))",     Domain.Parser.Keyword.Z_FIT, new int[] { 1, 2, 3 }));
				yield return (new TestCaseData(@"\!(Z_FIT(1,2,3))",       Domain.Parser.Keyword.Z_FIT, new int[] { 1, 2, 3 }));

				// Sign:
				yield return (new TestCaseData(@"\!(Z_FIT(1, -2, 3))", Domain.Parser.Keyword.Z_FIT, new int[] { 1, -2, 3 }));
				yield return (new TestCaseData(@"\!(Z_FIT(1, 2, +3))", Domain.Parser.Keyword.Z_FIT, new int[] { 1, 2, +3 }));

				// Radix:
				yield return (new TestCaseData(@"\!(Z_FIT(0b0))",                                Domain.Parser.Keyword.Z_FIT, new int[] { 0x00 }));
				yield return (new TestCaseData(@"\!(Z_FIT(0b1))",                                Domain.Parser.Keyword.Z_FIT, new int[] { 0x01 }));
				yield return (new TestCaseData(@"\!(Z_FIT(0b01))",                               Domain.Parser.Keyword.Z_FIT, new int[] { 0x01 }));
				yield return (new TestCaseData(@"\!(Z_FIT(0b11))",                               Domain.Parser.Keyword.Z_FIT, new int[] { 0x03 }));
				yield return (new TestCaseData(@"\!(Z_FIT(0b00000000000000000000000000000001))", Domain.Parser.Keyword.Z_FIT, new int[] { 0x01 }));
				yield return (new TestCaseData(@"\!(Z_FIT(0b01000000000000000000000000000001))", Domain.Parser.Keyword.Z_FIT, new int[] { 0x40000001 }));
				yield return (new TestCaseData(@"\!(Z_FIT(0b01111111111111111111111111111111))", Domain.Parser.Keyword.Z_FIT, new int[] { 0x7FFFFFFF }));
				yield return (new TestCaseData(@"\!(Z_FIT(00))",                                 Domain.Parser.Keyword.Z_FIT, new int[] { 0 }));
				yield return (new TestCaseData(@"\!(Z_FIT(01))",                                 Domain.Parser.Keyword.Z_FIT, new int[] { 1 }));
				yield return (new TestCaseData(@"\!(Z_FIT(001))",                                Domain.Parser.Keyword.Z_FIT, new int[] { 1 }));
				yield return (new TestCaseData(@"\!(Z_FIT(012))",                                Domain.Parser.Keyword.Z_FIT, new int[] { 10 }));
				yield return (new TestCaseData(@"\!(Z_FIT(01234567))",                           Domain.Parser.Keyword.Z_FIT, new int[] { 342391 }));
				yield return (new TestCaseData(@"\!(Z_FIT(000000000001))",                       Domain.Parser.Keyword.Z_FIT, new int[] { 1 }));
				yield return (new TestCaseData(@"\!(Z_FIT(010000000001))",                       Domain.Parser.Keyword.Z_FIT, new int[] { 0x40000001 }));
				yield return (new TestCaseData(@"\!(Z_FIT(017777777777))",                       Domain.Parser.Keyword.Z_FIT, new int[] { 0x7FFFFFFF }));
				yield return (new TestCaseData(@"\!(Z_FIT(0x0))",                                Domain.Parser.Keyword.Z_FIT, new int[] { 0x00 }));
				yield return (new TestCaseData(@"\!(Z_FIT(0xA))",                                Domain.Parser.Keyword.Z_FIT, new int[] { 0x0A }));
				yield return (new TestCaseData(@"\!(Z_FIT(0x0A))",                               Domain.Parser.Keyword.Z_FIT, new int[] { 0x0A }));
				yield return (new TestCaseData(@"\!(Z_FIT(0x1A))",                               Domain.Parser.Keyword.Z_FIT, new int[] { 0x1A }));
				yield return (new TestCaseData(@"\!(Z_FIT(0x0123CDEF))",                         Domain.Parser.Keyword.Z_FIT, new int[] { 0x0123CDEF }));
				yield return (new TestCaseData(@"\!(Z_FIT(0x00000001))",                         Domain.Parser.Keyword.Z_FIT, new int[] { 0x01 }));
				yield return (new TestCaseData(@"\!(Z_FIT(0x40000001))",                         Domain.Parser.Keyword.Z_FIT, new int[] { 0x40000001 }));
				yield return (new TestCaseData(@"\!(Z_FIT(0x7FFFFFFF))",                         Domain.Parser.Keyword.Z_FIT, new int[] { 0x7FFFFFFF }));
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			get
			{
				foreach (TestCaseData tcd in TestCasesWithoutName)
				{
					yield return (tcd.SetName("Keyword escape " + (string)tcd.Arguments[0]));
				}
			}
		}

		#endregion
	}

	/// <summary></summary>
	public static class ErrorTestData
	{
		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		private static IEnumerable TestCasesPartlyWithoutName
		{
			get							// Erroneous input		Expected substring		Expected message
			{
				yield return (new TestCaseData(@"A<AAA",		@"A<AA",		@"""AAA"" is an invalid ASCII mnemonic.").SetName("Invalid ASCII"));

				yield return (new TestCaseData(@"A\d(A7)",		@"A\d(",		@"""A"" is an invalid decimal value.").SetName("Invalid decimal value 1"));
				yield return (new TestCaseData(@"A\d(A77)",		@"A\d(",		@"""A"" is an invalid decimal value.").SetName("Invalid decimal value 2"));
				yield return (new TestCaseData(@"A\d(7A)",		@"A\d(7",		@"""A"" of ""7A"" is an invalid decimal value.").SetName("Invalid decimal digit 1"));
				yield return (new TestCaseData(@"A\d(7AA)",		@"A\d(7",		@"""A"" of ""7A"" is an invalid decimal value.").SetName("Invalid decimal digit 2"));

				yield return (new TestCaseData(@"A\e(AA)",		@"A\",			@"Character 'e' (0x65) is an invalid escape character.").SetName("Invalid escape character"));

				yield return (new TestCaseData(@"A\",			@"A",			@"Incomplete escape sequence.").SetName("Incomplete escape sequence 1"));
				yield return (new TestCaseData(@"A\\\",			@"A\\",			@"Incomplete escape sequence.").SetName("Incomplete escape sequence 2"));

				yield return (new TestCaseData(@"\!(Clear(10))",				@"\!(Clear(",				@"Keyword 'Clear' does not support arguments.").SetName("Keyword 'Clear' with arguments"));
				yield return (new TestCaseData(@"\!(Eol(10))",					@"\!(Eol(",					@"Keyword 'Eol' does not support arguments.").SetName("Keyword 'Eol' with arguments"));
				yield return (new TestCaseData(@"\!(NoEol(10))",				@"\!(NoEol(",				@"Keyword 'NoEol' does not support arguments.").SetName("Keyword 'NoEol' with arguments"));
				yield return (new TestCaseData(@"\!(OutputBreakOn(10))",		@"\!(OutputBreakOn(",		@"Keyword 'OutputBreakOn' does not support arguments.").SetName("Keyword 'OutputBreakOn' with arguments"));
				yield return (new TestCaseData(@"\!(OutputBreakOff(10))",		@"\!(OutputBreakOff(",		@"Keyword 'OutputBreakOff' does not support arguments.").SetName("Keyword 'OutputBreakOff' with arguments"));
				yield return (new TestCaseData(@"\!(OutputBreakToggle(10))",	@"\!(OutputBreakToggle(",	@"Keyword 'OutputBreakToggle' does not support arguments.").SetName("Keyword 'OutputBreakToggle' with arguments"));

				yield return (new TestCaseData(@"\!(Z_FIT(1 2 3))",		@"\!(Z_FIT(1 ",		@"Closing parenthesis expected instead of character '2' (0x32)."));
				yield return (new TestCaseData(@"\!(Z_FIT(1.2.3))",		@"\!(Z_FIT(1",		@"Character '.' (0x2E) is invalid for decimal values."));
				yield return (new TestCaseData(@"\!(Z_FIT(1 2, 3))",	@"\!(Z_FIT(1 ",		@"Closing parenthesis expected instead of character '2' (0x32)."));
				yield return (new TestCaseData(@"\!(Z_FIT(1.2, 3))",	@"\!(Z_FIT(1",		@"Character '.' (0x2E) is invalid for decimal values."));
				yield return (new TestCaseData(@"\!(Z_FIT(1, 2 3))",	@"\!(Z_FIT(1, 2 ",	@"Closing parenthesis expected instead of character '3' (0x33)."));
				yield return (new TestCaseData(@"\!(Z_FIT(1, 2.3))",	@"\!(Z_FIT(1, 2",	@"Character '.' (0x2E) is invalid for decimal values."));
				yield return (new TestCaseData(@"\!(Z_FIT(1 23))",		@"\!(Z_FIT(1 ",		@"Closing parenthesis expected instead of character '2' (0x32)."));
				yield return (new TestCaseData(@"\!(Z_FIT(1.23))",		@"\!(Z_FIT(1",		@"Character '.' (0x2E) is invalid for decimal values."));
				yield return (new TestCaseData(@"\!(Z_FIT(12 3))",		@"\!(Z_FIT(12 ",	@"Closing parenthesis expected instead of character '3' (0x33)."));
				yield return (new TestCaseData(@"\!(Z_FIT(12.3))",		@"\!(Z_FIT(12",		@"Character '.' (0x2E) is invalid for decimal values."));

				yield return (new TestCaseData(@"\!(Z_FIT(1,))",		@"\!(Z_FIT(1,",		@"Empty arguments are no permitted."));
				yield return (new TestCaseData(@"\!(Z_FIT(1 ,))",		@"\!(Z_FIT(1 ,",	@"Empty arguments are no permitted."));
				yield return (new TestCaseData(@"\!(Z_FIT(1,2,))",		@"\!(Z_FIT(1,2,",	@"Empty arguments are no permitted."));
				yield return (new TestCaseData(@"\!(Z_FIT(1,2 ,))",		@"\!(Z_FIT(1,2 ,",	@"Empty arguments are no permitted."));
				yield return (new TestCaseData(@"\!(Z_FIT(,2,3))",		@"\!(Z_FIT(",		@"Empty arguments are no permitted."));
				yield return (new TestCaseData(@"\!(Z_FIT( ,2,3))",		@"\!(Z_FIT( ",		@"Empty arguments are no permitted."));
				yield return (new TestCaseData(@"\!(Z_FIT(1,2,3,))",	@"\!(Z_FIT(1,2,3",	@"Keyword 'Z_FIT' only supports up to 3 arguments."));
				yield return (new TestCaseData(@"\!(Z_FIT(1,2,3 ,))",	@"\!(Z_FIT(1,2,3 ",	@"Keyword 'Z_FIT' only supports up to 3 arguments."));
				yield return (new TestCaseData(@"\!(Z_FIT(1,2,3,4))",	@"\!(Z_FIT(1,2,3",	@"Keyword 'Z_FIT' only supports up to 3 arguments."));
				yield return (new TestCaseData(@"\!(Z_FIT(1,2,3 ,4))",	@"\!(Z_FIT(1,2,3 ",	@"Keyword 'Z_FIT' only supports up to 3 arguments."));

				yield return (new TestCaseData(@"\!(Z_FIT(bla))",		@"\!(Z_FIT(",		@"Character 'b' (0x62) is invalid for decimal values."));
				yield return (new TestCaseData(@"\!(Z_FIT(1,bla))",		@"\!(Z_FIT(1,",		@"Character 'b' (0x62) is invalid for decimal values."));
				yield return (new TestCaseData(@"\!(Z_FIT(1, bla))",	@"\!(Z_FIT(1, ",	@"Character 'b' (0x62) is invalid for decimal values."));
				yield return (new TestCaseData(@"\!(Z_FIT(1,2,bla))",	@"\!(Z_FIT(1,2,",	@"Character 'b' (0x62) is invalid for decimal values."));
				yield return (new TestCaseData(@"\!(Z_FIT(1,2, bla))",	@"\!(Z_FIT(1,2, ",	@"Character 'b' (0x62) is invalid for decimal values."));

				yield return (new TestCaseData(@"\!(Z_FIT(,))",			@"\!(Z_FIT(",		@"Empty arguments are no permitted."));
				yield return (new TestCaseData(@"\!(Z_FIT(,,))",		@"\!(Z_FIT(",		@"Empty arguments are no permitted."));
				yield return (new TestCaseData(@"\!(Z_FIT(,,,))",		@"\!(Z_FIT(",		@"Empty arguments are no permitted."));
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			get
			{
				foreach (TestCaseData tcd in TestCasesPartlyWithoutName)
				{
					if (tcd.TestName != null)
						yield return (tcd);
					else
						yield return (tcd.SetName("Keyword escape " + (string)tcd.Arguments[0]));
				}
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

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "bytes", Justification = "The naming emphasizes the difference between bytes and other parameters.")]
		[Test, TestCaseSource(typeof(ParserTestData), "TestCases")]
		public virtual void TestParser(string s, byte[] expectedBytes)
		{
			using (Domain.Parser.Parser p = new Domain.Parser.Parser())
			{
				byte[] actualBytes;
				string successfullyParsed;
				Assert.That(p.TryParse(s, out actualBytes, out successfullyParsed), Is.True, @"Failed! Only """ + successfullyParsed + @""" could successfully be parsed!");
				Assert.That(actualBytes, Is.EqualTo(expectedBytes));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "bytes", Justification = "The naming emphasizes the difference between bytes and other parameters.")]
		[Test, TestCaseSource(typeof(EncodingTestData), "TestCases")]
		public virtual void TestParserEncoding(Encoding encoding, Endianness endianness, string s, byte[] expectedBytes)
		{
			using (Domain.Parser.Parser p = new Domain.Parser.Parser(encoding, endianness))
			{
				byte[] actualBytes;
				string successfullyParsed;
				Assert.That(p.TryParse(s, out actualBytes, out successfullyParsed), Is.True, @"Failed! Only """ + successfullyParsed + @""" could successfully be parsed!");
				Assert.That(actualBytes, Is.EqualTo(expectedBytes));
			}
		}

		/// <remarks>
		/// So far, only integer arguments are supported.
		/// See <see cref="Domain.Parser.KeywordArgState"/> for more details.
		/// </remarks>
		[Test, TestCaseSource(typeof(KeywordTestData), "TestCases")]
		public virtual void TestParserKeyword(string s, Domain.Parser.Keyword expectedKeyword, int[] expectedArgs)
		{
			using (Domain.Parser.Parser p = new Domain.Parser.Parser())
			{
				Domain.Parser.Result[] results;
				string successfullyParsed;
				Assert.That(p.TryParse(s, out results, out successfullyParsed), Is.True, @"Failed! Only """ + successfullyParsed + @""" could successfully be parsed!");
				Assert.That(results.Length, Is.EqualTo(1));
				Assert.That(results[0], Is.TypeOf(typeof(Domain.Parser.KeywordResult)));

				var actualKeyword = results[0] as Domain.Parser.KeywordResult;
				Assert.That(actualKeyword.Keyword, Is.EqualTo(expectedKeyword));
				Assert.That(actualKeyword.Args,    Is.EqualTo(expectedArgs));
			}
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(ErrorTestData), "TestCases")]
		public virtual void TestParserError(string s, string expectedParsed, string expectedMessage)
		{
			using (Domain.Parser.Parser p = new Domain.Parser.Parser())
			{
				byte[] actualBytes;
				string actualParsed;

				Domain.Parser.FormatException formatException = new Domain.Parser.FormatException("");
				Assert.That(p.TryParse(s, out actualBytes, out actualParsed, ref formatException), Is.False);

				string actualMessage;
				if (formatException.Message.Contains(Environment.NewLine))
					actualMessage = formatException.Message.Substring(0, formatException.Message.IndexOf(Environment.NewLine, StringComparison.CurrentCultureIgnoreCase));
				else
					actualMessage = formatException.Message;

				if (!expectedParsed.Equals(actualParsed))
				{
					Trace.WriteLine("Input    " + s);
					Trace.WriteLine("Expected " + expectedParsed);
					Trace.WriteLine("Actual   " + actualParsed);
					Trace.WriteLine("Message  " + actualMessage);

					Assert.That(actualParsed, Is.EqualTo(expectedParsed));
				}

				if (!expectedMessage.Equals(actualMessage))
				{
					Trace.WriteLine("Input    " + s);
					Trace.WriteLine("Expected " + expectedParsed);
					Trace.WriteLine("Actual   " + actualParsed);
					Trace.WriteLine("Message  " + actualMessage);

					Assert.That(actualMessage, Is.EqualTo(expectedMessage));
				}
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
