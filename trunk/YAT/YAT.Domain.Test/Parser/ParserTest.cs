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
using System.Collections;
using System.Collections.Generic;
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
				yield return (new TestCaseData(@"\x1A\h(0A",	new byte[] { 0x1A, 0x0A }).SetName("Subsequent YAT value without closing parentheses"));
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
				yield return (new TestCaseData(@"<CR LF",		new byte[] { 0x0D, 0x0A } ).SetName("ASCII <CR LF> without closing parentheses"));

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
				yield return (new TestCaseData(@"Single char \c(.",					new byte[] { 0x53, 0x69, 0x6E, 0x67, 0x6C, 0x65, 0x20, 0x63, 0x68, 0x61, 0x72, 0x20, 0x2E } ).SetName("Char single '.' without closing parentheses"));
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
				yield return (new TestCaseData(@"\b(00100011 01011101 00100100)",	new byte[] { 0x23, 0x5D, 0x24 } ).SetName("Bin sequence 1"));
				yield return (new TestCaseData(@"\b(001000110101110100100100)",		new byte[] { 0x23, 0x5D, 0x24 } ).SetName("Bin sequence 2"));
				yield return (new TestCaseData(@"\b(0010001101011101001001)",		new byte[] { 0x23, 0x5D, 0x09 } ).SetName("Bin sequence 3"));
				yield return (new TestCaseData(@"\b( 01)",							new byte[] { 0x01 } ).SetName("Bin 01 leading space"));
				yield return (new TestCaseData(@"\b(01 )",							new byte[] { 0x01 } ).SetName("Bin 01 trailing space"));
				yield return (new TestCaseData(@"\b(   01   )",						new byte[] { 0x01 } ).SetName("Bin 01 leading and trailing space"));
				yield return (new TestCaseData(@"\b(   01   ",						new byte[] { 0x01 } ).SetName("Bin 01 leading and trailing space without closing parentheses"));

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
				yield return (new TestCaseData(@"\o(43 135 44)",					new byte[] { 0x23, 0x5D, 0x24 } ).SetName("Oct sequence 1"));
				yield return (new TestCaseData(@"\o(4313544)",						new byte[] { 0x23, 0x5D, 0x24 } ).SetName("Oct sequence 2"));
				yield return (new TestCaseData(@"\o(4313511)",						new byte[] { 0x23, 0x5D, 0x09 } ).SetName("Oct sequence 3"));
				yield return (new TestCaseData(@"\o( 01)",							new byte[] { 0x01 } ).SetName("Oct 01 leading space"));
				yield return (new TestCaseData(@"\o( 01",							new byte[] { 0x01 } ).SetName("Oct 01 leading space without closing parentheses"));
				yield return (new TestCaseData(@"\o(01 )",							new byte[] { 0x01 } ).SetName("Oct 01 trailing space"));
				yield return (new TestCaseData(@"\o(   01   )",						new byte[] { 0x01 } ).SetName("Oct 01 leading and trailing space"));

				// Dec:
				yield return (new TestCaseData(@"\d()",								new byte[] { }).SetName("Dec empty"));
				yield return (new TestCaseData(@"\d()",								new byte[] { }).SetName("Dec empty without closing parentheses"));
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
				yield return (new TestCaseData(@"\d(35 93 36)",						new byte[] { 0x23, 0x5D, 0x24 } ).SetName("Dec sequence 1"));
				yield return (new TestCaseData(@"\d(359336)",						new byte[] { 0x23, 0x5D, 0x24 } ).SetName("Dec sequence 2"));
				yield return (new TestCaseData(@"\d(35939)",						new byte[] { 0x23, 0x5D, 0x09 } ).SetName("Dec sequence 3"));
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
				yield return (new TestCaseData(@"\h(FF",							new byte[] { 0xFF } ).SetName("Hex FF without closing parentheses"));
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

		/// <remarks>
		/// Good online tools:
		/// http://toolswebtop.com/text/process/encode/utf-8 as well as many others; only works with Firefox
		/// http://toolswebtop.com/text/process/decode/utf-8 as well as many others; only works with Firefox
		/// https://www.branah.com/unicode-converter
		/// https://r12a.github.io/app-conversion/
		/// https://r12a.github.io/app-encodings/
		///
		/// Useless online tools:
		/// https://www.motobit.com/util/charset-codepage-conversion.asp (not working for advanced characters)
		/// http://www.njstar.com/cms/unicode-to-dbcs-code-conversion (no access to bytes)
		/// http://codepage-encoding.online-domain-tools.com/ (always encodes 16 bits)
		/// http://string-functions.com/encodedecode.aspx (no access to bytes)
		///
		/// Rather use a simple .NET application:
		/// <![CDATA[
		/// var lines = File.ReadAllLines("Encoding-UTF-8.txt", Encoding.UTF8);
		///
		/// var lookup = new Dictionary<int, string>();
		/// lookup.Add(950,   "Encoding-CJK-Big5-[950].txt");
		/// lookup.Add(20936, "Encoding-CJK-GB2312-[20936].txt");
		/// lookup.Add(54936, "Encoding-CJK-GB18030-[54936].txt");
		/// lookup.Add(936,   "Encoding-CJK-GBK-[936].txt");
		/// lookup.Add(949,   "Encoding-CJK-KSC-[949].txt");
		/// lookup.Add(932,   "Encoding-CJK-ShiftJIS-[932].txt");
		///
		/// foreach (var kvp in lookup)
		/// {
		/// 	var e = Encoding.GetEncoding(kvp.Key);
		///
		/// 	using (var stream = File.Open(kvp.Value, FileMode.Create, FileAccess.Write))
		/// 	{
		/// 		using (var writer = new BinaryWriter(stream))
		/// 		{
		/// 			for (int i = 0; i < lines.Length; i++)
		/// 			{
		/// 				writer.Write(e.GetBytes(lines[i]));
		///
		/// 				if (i < (lines.Length - 1))
		/// 					writer.Write(e.GetBytes(Environment.NewLine));
		/// 			}
		/// 		}
		/// 	}
		/// }
		/// ]]>
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop isn't able to skip URLs...")]
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Why not?")]
		public static IEnumerable<Tuple<Encoding, string, byte[]>> TestCaseTuples
		{
			get
			{
				// ASCII:
				yield return (new Tuple<Encoding, string, byte[]>(Encoding.ASCII, "abc", new byte[] { 0x61, 0x62, 0x63 }));

				// Windows-1252 [1252]:
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.Windows1252), "abc", new byte[] { 0x61, 0x62, 0x63 }));
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.Windows1252), "äöü", new byte[] { 0xE4, 0xF6, 0xFC }));
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.Windows1252), "ÄÖÜ", new byte[] { 0xC4, 0xD6, 0xDC }));
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.Windows1252), "$£€", new byte[] { 0x24, 0xA3, 0x80 }));
				                                                                                                    //// "čěř" not supported
				                                                                                                    //// "一二州" not supported
				                                                                                                    //// "︙" not supported
				                                                                                                    //// "𝄞" not supported
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.Windows1252), @"0\0<CR>1\n2", new byte[] { 0x30, 0x00, 0x0D, 0x31, 0x0A, 0x32 }));

			////// UTF-7 [65000] removed as that encoding belongs to the class of Base64 and Quoted-Printable (FR #407).
			////yield return (new Tuple<Encoding, string, byte[]>(Encoding.UTF7, "abc", new byte[] { 0x61, 0x62, 0x63 }));
			////yield return (new Tuple<Encoding, string, byte[]>(Encoding.UTF7, "äöü", new byte[] { 0x2B, 0x41, 0x4F, 0x51, 0x41, 0x39, 0x67, 0x44, 0x38, 0x2D })); // +AOQA9gD8-
			////yield return (new Tuple<Encoding, string, byte[]>(Encoding.UTF7, "ÄÖÜ", new byte[] { 0x2B, 0x41, 0x4D, 0x51, 0x41, 0x31, 0x67, 0x44, 0x63, 0x2D })); // +AMQA1gDc-
			////yield return (new Tuple<Encoding, string, byte[]>(Encoding.UTF7, "$£€", new byte[] { 0x2B, 0x41, 0x43, 0x51, 0x41, 0x6F, 0x79, 0x43, 0x73, 0x2D })); // +ACQAoyCs-
			////yield return (new Tuple<Encoding, string, byte[]>(Encoding.UTF7, "čěř", new byte[] { 0x2B, 0x41, 0x51, 0x30, 0x42, 0x47, 0x77, 0x46, 0x5A, 0x2D })); // +AQ0BGwFZ-
			////                                                                 //// yi er zhou is U+4E00 U+4E8C U+5DDE
			////yield return (new Tuple<Encoding, string, byte[]>(Encoding.UTF7, "一二州", new byte[] { 0x2B, 0x54, 0x67, 0x42, 0x4F, 0x6A, 0x46, 0x33, 0x65, 0x2D })); // +TgBOjF3e-
			////                                                                 //// 'Vertical Horizontal Ellipsis' is U+FE19
			////yield return (new Tuple<Encoding, string, byte[]>(Encoding.UTF7, "︙", new byte[] { 0x2B, 0x2F, 0x68, 0x6B, 0x2D })); // +/hk-
			////                                                                 //// 'Notenschlüssel' is U+1D11E but U+10000 and above not supported by .NET Framework 4.x (see FR #329 for more information)
			////yield return (new Tuple<Encoding, string, byte[]>(Encoding.UTF7, "𝄞", new byte[] { 0x2B, 0x32, 0x44, 0x54, 0x64, 0x48, 0x67, 0x2D })); // +2DTdHg-
			////
			////yield return (new Tuple<Encoding, string, byte[]>(Encoding.UTF7, @"0\0<CR>1\n2", new byte[] { 0x30, 0x2B, 0x41, 0x41, 0x41, 0x2D, 0x0D, 0x31, 0x0A, 0x32 })); // 0+AAA-\r1\n2

				// UTF-8 [65001]:
				yield return (new Tuple<Encoding, string, byte[]>(Encoding.UTF8, "abc", new byte[] { 0x61, 0x62, 0x63 }));
				yield return (new Tuple<Encoding, string, byte[]>(Encoding.UTF8, "äöü", new byte[] { 0xC3, 0xA4, 0xC3, 0xB6, 0xC3, 0xBC }));
				yield return (new Tuple<Encoding, string, byte[]>(Encoding.UTF8, "ÄÖÜ", new byte[] { 0xC3, 0x84, 0xC3, 0x96, 0xC3, 0x9C }));
				yield return (new Tuple<Encoding, string, byte[]>(Encoding.UTF8, "$£€", new byte[] { 0x24, 0xC2, 0xA3, 0xE2, 0x82, 0xAC })); // 1-2-3 bytes!
				yield return (new Tuple<Encoding, string, byte[]>(Encoding.UTF8, "čěř", new byte[] { 0xC4, 0x8D, 0xC4, 0x9B, 0xC5, 0x99 }));
				                                                                 //// yi er zhou is U+4E00 U+4E8C U+5DDE
				yield return (new Tuple<Encoding, string, byte[]>(Encoding.UTF8, "一二州", new byte[] { 0xE4, 0xB8, 0x80, 0xE4, 0xBA, 0x8C, 0xE5, 0xB7, 0x9E }));
				                                                                 //// 'Vertical Horizontal Ellipsis' is U+FE19
				yield return (new Tuple<Encoding, string, byte[]>(Encoding.UTF8, "︙", new byte[] { 0xEF, 0xB8, 0x99 }));
				                                                                 //// 'Notenschlüssel' is U+1D11E but U+10000 and above not supported by .NET Framework 4.x (see FR #329 for more information)
				yield return (new Tuple<Encoding, string, byte[]>(Encoding.UTF8, "𝄞", new byte[] { 0xF0, 0x9D, 0x84, 0x9E }));

				yield return (new Tuple<Encoding, string, byte[]>(Encoding.UTF8, @"0\0<CR>1\n2", new byte[] { 0x30, 0x00, 0x0D, 0x31, 0x0A, 0x32 }));

				// UTF-16 (little endian, i.e. machine endianness) [1200]:
				yield return (new Tuple<Encoding, string, byte[]>(Encoding.Unicode, "abc", new byte[] { 0x61, 0x00, 0x62, 0x00, 0x63, 0x00 }));
				yield return (new Tuple<Encoding, string, byte[]>(Encoding.Unicode, "äöü", new byte[] { 0xE4, 0x00, 0xF6, 0x00, 0xFC, 0x00 }));
				yield return (new Tuple<Encoding, string, byte[]>(Encoding.Unicode, "ÄÖÜ", new byte[] { 0xC4, 0x00, 0xD6, 0x00, 0xDC, 0x00 }));
				yield return (new Tuple<Encoding, string, byte[]>(Encoding.Unicode, "$£€", new byte[] { 0x24, 0x00, 0xA3, 0x00, 0xAC, 0x20 }));
				yield return (new Tuple<Encoding, string, byte[]>(Encoding.Unicode, "čěř", new byte[] { 0x0D, 0x01, 0x1B, 0x01, 0x59, 0x01 }));
				                                                                    //// yi er zhou is U+4E00 U+4E8C U+5DDE
				yield return (new Tuple<Encoding, string, byte[]>(Encoding.Unicode, "一二州", new byte[] { 0x00, 0x4E, 0x8C, 0x4E, 0xDE, 0x5D }));
				                                                                    //// 'Vertical Horizontal Ellipsis' is U+FE19
				yield return (new Tuple<Encoding, string, byte[]>(Encoding.Unicode, "︙", new byte[] { 0x19, 0xFE }));
				                                                                    //// 'Notenschlüssel' is U+1D11E but U+10000 and above not supported by .NET Framework 4.x (see FR #329 for more information)
				yield return (new Tuple<Encoding, string, byte[]>(Encoding.Unicode, "𝄞", new byte[] { 0x34, 0xD8, 0x1E, 0xDD }));
				                                                                                          //// |           |           |           |           |           |           |
				yield return (new Tuple<Encoding, string, byte[]>(Encoding.Unicode, @"0\0<CR>1\n2", new byte[] { 0x30, 0x00, 0x00, 0x00, 0x0D, 0x00, 0x31, 0x00, 0x0A, 0x00, 0x32, 0x00 }));

				// UTF-16 (big endian, i.e. network endianness) [1201]:
				yield return (new Tuple<Encoding, string, byte[]>(Encoding.BigEndianUnicode, "abc", new byte[] { 0x00, 0x61, 0x00, 0x62, 0x00, 0x63 }));
				yield return (new Tuple<Encoding, string, byte[]>(Encoding.BigEndianUnicode, "äöü", new byte[] { 0x00, 0xE4, 0x00, 0xF6, 0x00, 0xFC }));
				yield return (new Tuple<Encoding, string, byte[]>(Encoding.BigEndianUnicode, "ÄÖÜ", new byte[] { 0x00, 0xC4, 0x00, 0xD6, 0x00, 0xDC }));
				yield return (new Tuple<Encoding, string, byte[]>(Encoding.BigEndianUnicode, "$£€", new byte[] { 0x00, 0x24, 0x00, 0xA3, 0x20, 0xAC }));
				yield return (new Tuple<Encoding, string, byte[]>(Encoding.BigEndianUnicode, "čěř", new byte[] { 0x01, 0x0D, 0x01, 0x1B, 0x01, 0x59 }));
				                                                                             //// yi er zhou is U+4E00 U+4E8C U+5DDE
				yield return (new Tuple<Encoding, string, byte[]>(Encoding.BigEndianUnicode, "一二州", new byte[] { 0x4E, 0x00, 0x4E, 0x8C, 0x5D, 0xDE }));
				                                                                             //// 'Vertical Horizontal Ellipsis' is U+FE19
				yield return (new Tuple<Encoding, string, byte[]>(Encoding.BigEndianUnicode, "︙", new byte[] { 0xFE, 0x19 }));
				                                                                             //// 'Notenschlüssel' is U+1D11E but U+10000 and above not supported by .NET Framework 4.x (see FR #329 for more information)
				yield return (new Tuple<Encoding, string, byte[]>(Encoding.BigEndianUnicode, "𝄞", new byte[] { 0xD8, 0x34, 0xDD, 0x1E }));
				                                                                                                   //// |           |           |           |           |           |           |
				yield return (new Tuple<Encoding, string, byte[]>(Encoding.BigEndianUnicode, @"0\0<CR>1\n2", new byte[] { 0x00, 0x30, 0x00, 0x00, 0x00, 0x0D, 0x00, 0x31, 0x00, 0x0A, 0x00, 0x32 }));

				// UTF-32 (little endian, i.e. machine endianness) [12000]:
				yield return (new Tuple<Encoding, string, byte[]>(Encoding.UTF32, "abc", new byte[] { 0x61, 0x00, 0x00, 0x00, 0x62, 0x00, 0x00, 0x00, 0x63, 0x00, 0x00, 0x00 }));
				yield return (new Tuple<Encoding, string, byte[]>(Encoding.UTF32, "äöü", new byte[] { 0xE4, 0x00, 0x00, 0x00, 0xF6, 0x00, 0x00, 0x00, 0xFC, 0x00, 0x00, 0x00 }));
				yield return (new Tuple<Encoding, string, byte[]>(Encoding.UTF32, "ÄÖÜ", new byte[] { 0xC4, 0x00, 0x00, 0x00, 0xD6, 0x00, 0x00, 0x00, 0xDC, 0x00, 0x00, 0x00 }));
				yield return (new Tuple<Encoding, string, byte[]>(Encoding.UTF32, "$£€", new byte[] { 0x24, 0x00, 0x00, 0x00, 0xA3, 0x00, 0x00, 0x00, 0xAC, 0x20, 0x00, 0x00 }));
				yield return (new Tuple<Encoding, string, byte[]>(Encoding.UTF32, "čěř", new byte[] { 0x0D, 0x01, 0x00, 0x00, 0x1B, 0x01, 0x00, 0x00, 0x59, 0x01, 0x00, 0x00 }));
				                                                                  //// yi er zhou is U+4E00 U+4E8C U+5DDE
				yield return (new Tuple<Encoding, string, byte[]>(Encoding.UTF32, "一二州", new byte[] { 0x00, 0x4E, 0x00, 0x00, 0x8C, 0x4E, 0x00, 0x00, 0xDE, 0x5D, 0x00, 0x00 }));
				                                                                  //// 'Vertical Horizontal Ellipsis' is U+FE19
				yield return (new Tuple<Encoding, string, byte[]>(Encoding.UTF32, "︙", new byte[] { 0x19, 0xFE, 0x00, 0x00 }));
				                                                                  //// 'Notenschlüssel' is U+1D11E but U+10000 and above not supported by .NET Framework 4.x (see FR #329 for more information)
				yield return (new Tuple<Encoding, string, byte[]>(Encoding.UTF32, "𝄞", new byte[] { 0x1E, 0xD1, 0x01, 0x00 }));
				                                                                                        //// |                       |                       |                       |                       |                       |                       |
				yield return (new Tuple<Encoding, string, byte[]>(Encoding.UTF32, @"0\0<CR>1\n2", new byte[] { 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0D, 0x00, 0x00, 0x00, 0x31, 0x00, 0x00, 0x00, 0x0A, 0x00, 0x00, 0x00, 0x32, 0x00, 0x00, 0x00 }));

				// UTF-32 (big endian, i.e. network endianness) [12001]:
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.UTF32BE), "abc", new byte[] { 0x00, 0x00, 0x00, 0x61, 0x00, 0x00, 0x00, 0x62, 0x00, 0x00, 0x00, 0x63 }));
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.UTF32BE), "äöü", new byte[] { 0x00, 0x00, 0x00, 0xE4, 0x00, 0x00, 0x00, 0xF6, 0x00, 0x00, 0x00, 0xFC }));
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.UTF32BE), "ÄÖÜ", new byte[] { 0x00, 0x00, 0x00, 0xC4, 0x00, 0x00, 0x00, 0xD6, 0x00, 0x00, 0x00, 0xDC }));
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.UTF32BE), "$£€", new byte[] { 0x00, 0x00, 0x00, 0x24, 0x00, 0x00, 0x00, 0xA3, 0x00, 0x00, 0x20, 0xAC }));
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.UTF32BE), "čěř", new byte[] { 0x00, 0x00, 0x01, 0x0D, 0x00, 0x00, 0x01, 0x1B, 0x00, 0x00, 0x01, 0x59 }));
				                                                                                                     //// yi er zhou is U+4E00 U+4E8C U+5DDE
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.UTF32BE), "一二州", new byte[] { 0x00, 0x00, 0x4E, 0x00, 0x00, 0x00, 0x4E, 0x8C, 0x00, 0x00, 0x5D, 0xDE }));
				                                                                                                     //// 'Vertical Horizontal Ellipsis' is U+FE19
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.UTF32BE), "︙", new byte[] { 0x00, 0x00, 0xFE, 0x19 }));
				                                                                                                     //// 'Notenschlüssel' is U+1D11E but U+10000 and above not supported by .NET Framework 4.x (see FR #329 for more information)
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.UTF32BE), "𝄞", new byte[] { 0x00, 0x01, 0xD1, 0x1E }));
				                                                                                                                           //// |                       |                       |                       |                       |                       |                       |
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.UTF32BE), @"0\0<CR>1\n2", new byte[] { 0x00, 0x00, 0x00, 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0D, 0x00, 0x00, 0x00, 0x31, 0x00, 0x00, 0x00, 0x0A, 0x00, 0x00, 0x00, 0x32 }));

				// Big5 [950]:
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.Big5), "abc", new byte[] { 0x61, 0x62, 0x63 })); // same as ASCII
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.Big5), "äöü", new byte[] { 0x61, 0x6F, 0x75 })); // Umlaute not supported, i.e. resulting in "aou"
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.Big5), "ÄÖÜ", new byte[] { 0x41, 0x4F, 0x55 })); // Umlaute not supported, i.e. resulting in "aou"
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.Big5), "$£€", new byte[] { 0x24, 0xA2, 0x47, 0xA3, 0xE1 })); // 1-2-2 bytes
				                                                                                             //// "čěř" not supported
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.Big5), "一二州", new byte[] { 0xA4, 0x40, 0xA4, 0x47, 0xA6, 0x7B }));
				                                                                                             //// "︙" not supported
				                                                                                             //// "𝄞" not supported
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.Big5), @"0\0<CR>1\n2", new byte[] { 0x30, 0x00, 0x0D, 0x31, 0x0A, 0x32 })); // same as ASCII

				// GBK [936]:                                                                              // is GBK!
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.GB2312), "abc", new byte[] { 0x61, 0x62, 0x63 })); // same as ASCII
				                                                                                               //// "äöü" not supported
				                                                                                               //// "ÄÖÜ" not supported
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.GB2312), "$£", new byte[] { 0x24, 0xA1, 0xEA })); // € not supported
				                                                                                               //// "čěř" not supported
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.GB2312), "一二州", new byte[] { 0xD2, 0xBB, 0xB6, 0xFE, 0xD6, 0xDD }));
				                                                                                               //// "︙", not supported
				                                                                                               //// "𝄞", not supported
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.GB2312), @"0\0<CR>1\n2", new byte[] { 0x30, 0x00, 0x0D, 0x31, 0x0A, 0x32 })); // same as ASCII

				// GB2312 (-80) [20936]:                                                                   // is GB2312!
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.X_CP20936), "abc", new byte[] { 0x61, 0x62, 0x63 })); // same as ASCII
				                                                                                                  //// "äöü" not supported
				                                                                                                  //// "ÄÖÜ" not supported
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.X_CP20936), "$", new byte[] { 0x24 })); // only $ supported
				                                                                                                  //// "čěř" not supported
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.X_CP20936), "一二州", new byte[] { 0xD2, 0xBB, 0xB6, 0xFE, 0xD6, 0xDD }));
				                                                                                                  //// "︙" not supported
				                                                                                                  //// "𝄞" not supported
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.X_CP20936), @"0\0<CR>1\n2", new byte[] { 0x30, 0x00, 0x0D, 0x31, 0x0A, 0x32 })); // same as ASCII

				// GB18030 [54936]:
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.GB18030), "abc", new byte[] { 0x61, 0x62, 0x63 })); // same as ASCII
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.GB18030),   "ü", new byte[] { 0xA8, 0xB9 })); // only ü supported
				                                                                                                //// "ÄÖÜ" not supported
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.GB18030), "$€",  new byte[] { 0x24, 0xA2, 0xE3 })); // £ not supported
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.GB18030),  "ě",  new byte[] { 0xA8, 0xA7 })); // only ě supported
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.GB18030), "一二州", new byte[] { 0xD2, 0xBB, 0xB6, 0xFE, 0xD6, 0xDD }));
				                                                                                                //// "︙" not supported
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.GB18030), "𝄞", new byte[] { 0x94, 0x32, 0xBE, 0x34 }));

				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.GB18030), @"0\0<CR>1\n2", new byte[] { 0x30, 0x00, 0x0D, 0x31, 0x0A, 0x32 })); // same as ASCII

				// KSC [949]:
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.KS_C_5601_1987), "abc", new byte[] { 0x61, 0x62, 0x63 })); // same as ASCII
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.KS_C_5601_1987), "äöü", new byte[] { 0x61, 0x6F, 0x75 })); // Umlaute not supported, i.e. resulting in "aou"
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.KS_C_5601_1987), "ÄÖÜ", new byte[] { 0x41, 0x4F, 0x55 })); // Umlaute not supported, i.e. resulting in "aou"
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.KS_C_5601_1987), "$£€", new byte[] { 0x24, 0xA1, 0xCC, 0xA2, 0xE6 })); // 1-2-2 bytes
				                                                                                                       //// "čěř" not supported
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.KS_C_5601_1987), "一二州", new byte[] { 0xEC, 0xE9, 0xEC, 0xA3, 0xF1, 0xB6 }));
				                                                                                                       //// "︙" not supported
				                                                                                                       //// "𝄞" not supported
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.KS_C_5601_1987), @"0\0<CR>1\n2", new byte[] { 0x30, 0x00, 0x0D, 0x31, 0x0A, 0x32 })); // same as ASCII

				// Shift-JIS [932]:
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.Shift_JIS), "abc", new byte[] { 0x61, 0x62, 0x63 })); // same as ASCII
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.Shift_JIS), "äöü", new byte[] { 0x61, 0x6F, 0x75 })); // Umlaute not supported, i.e. resulting in "aou"
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.Shift_JIS), "ÄÖÜ", new byte[] { 0x41, 0x4F, 0x55 })); // Umlaute not supported, i.e. resulting in "aou"
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.Shift_JIS), "$£",  new byte[] { 0x24, 0x81, 0x92 })); // only $ and £ supported
				                                                                                                  //// "čěř" not supported
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.Shift_JIS), "一二州", new byte[] { 0x88, 0xEA, 0x93, 0xF1, 0x8F, 0x42 }));
				                                                                                                  //// "︙" not supported
				                                                                                                  //// "𝄞" not supported
				yield return (new Tuple<Encoding, string, byte[]>(EncodingEx.GetEncoding(SupportedEncoding.Shift_JIS), @"0\0<CR>1\n2", new byte[] { 0x30, 0x00, 0x0D, 0x31, 0x0A, 0x32 })); // same as ASCII
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
					var encoding = tuple.Item1;
					var text     = tuple.Item2;
					var data     = tuple.Item3;

					yield return (new TestCaseData(tuple.Item1, tuple.Item2, tuple.Item3).SetName((EncodingEx)encoding + ", " + text));
				}
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
				yield return (new TestCaseData(@"\!(Clear)",					Domain.Parser.Keyword.Clear,				null));
				yield return (new TestCaseData(@"\!(Clear())",					Domain.Parser.Keyword.Clear,				null));
				yield return (new TestCaseData(@"\!(Delay)",					Domain.Parser.Keyword.Delay,				null));
				yield return (new TestCaseData(@"\!(Delay())",					Domain.Parser.Keyword.Delay,				null));
				yield return (new TestCaseData(@"\!(Delay(10))",				Domain.Parser.Keyword.Delay,				new int[] { 10 }));
				yield return (new TestCaseData(@"\!(LineDelay)",				Domain.Parser.Keyword.LineDelay,			null));
				yield return (new TestCaseData(@"\!(LineDelay())",				Domain.Parser.Keyword.LineDelay,			null));
				yield return (new TestCaseData(@"\!(LineDelay(10))",			Domain.Parser.Keyword.LineDelay,			new int[] { 10 }));
				yield return (new TestCaseData(@"\!(LineInterval)",				Domain.Parser.Keyword.LineInterval,			null));
				yield return (new TestCaseData(@"\!(LineInterval())",			Domain.Parser.Keyword.LineInterval,			null));
				yield return (new TestCaseData(@"\!(LineInterval(10))",			Domain.Parser.Keyword.LineInterval,			new int[] { 10 }));
			////yield return (new TestCaseData(@"\!(Repeat)",					Domain.Parser.Keyword.Repeat,				null)); is yet pending (FR #13) and requires parser support for strings (FR #404).
				yield return (new TestCaseData(@"\!(LineRepeat)",				Domain.Parser.Keyword.LineRepeat,			null));
				yield return (new TestCaseData(@"\!(LineRepeat())",				Domain.Parser.Keyword.LineRepeat,			null));
				yield return (new TestCaseData(@"\!(LineRepeat(10))",			Domain.Parser.Keyword.LineRepeat,			new int[] { 10 }));
				yield return (new TestCaseData(@"\!(TimeStamp)",				Domain.Parser.Keyword.TimeStamp,			null));
				yield return (new TestCaseData(@"\!(TimeStamp())",				Domain.Parser.Keyword.TimeStamp,			null));
				yield return (new TestCaseData(@"\!(Eol)",						Domain.Parser.Keyword.Eol,					null));
				yield return (new TestCaseData(@"\!(Eol())",					Domain.Parser.Keyword.Eol,					null));
				yield return (new TestCaseData(@"\!(NoEol)",					Domain.Parser.Keyword.NoEol,				null));
				yield return (new TestCaseData(@"\!(NoEol())",					Domain.Parser.Keyword.NoEol,				null));
				yield return (new TestCaseData(@"\!(Port)",						Domain.Parser.Keyword.Port,					null));
				yield return (new TestCaseData(@"\!(Port())",					Domain.Parser.Keyword.Port,					null));
				yield return (new TestCaseData(@"\!(Port(1))",					Domain.Parser.Keyword.Port,					new int[] { 1 }));
				yield return (new TestCaseData(@"\!(PortSettings)",				Domain.Parser.Keyword.PortSettings,			null));
				yield return (new TestCaseData(@"\!(PortSettings())",			Domain.Parser.Keyword.PortSettings,			null));
				yield return (new TestCaseData(@"\!(PortSettings(9600))",		Domain.Parser.Keyword.PortSettings,			new int[] { 9600 }));
				yield return (new TestCaseData(@"\!(PortSettings(9600,8))",		Domain.Parser.Keyword.PortSettings,			new int[] { 9600, 8 }));
				yield return (new TestCaseData(@"\!(PortSettings(9600;8;0))",	Domain.Parser.Keyword.PortSettings,			new int[] { 9600, 8, 0 })); // \remind (2018-06-13 / MKY) yet limited to parsing integer values (FR #404).
				yield return (new TestCaseData(@"\!(PortSettings(9600|8|0))",	Domain.Parser.Keyword.PortSettings,			new int[] { 9600, 8, 0 })); // \remind (2018-06-13 / MKY) yet limited to parsing integer values (FR #404).
				yield return (new TestCaseData(@"\!(Baud)",						Domain.Parser.Keyword.Baud,					null));
				yield return (new TestCaseData(@"\!(Baud())",					Domain.Parser.Keyword.Baud,					null));
				yield return (new TestCaseData(@"\!(Baud(9600))",				Domain.Parser.Keyword.Baud,					new int[] { 9600 }));
				yield return (new TestCaseData(@"\!(DataBits)",					Domain.Parser.Keyword.DataBits,				null));
				yield return (new TestCaseData(@"\!(DataBits())",				Domain.Parser.Keyword.DataBits,				null));
				yield return (new TestCaseData(@"\!(DataBits(7))",				Domain.Parser.Keyword.DataBits,				new int[] { 7 }));
				yield return (new TestCaseData(@"\!(Parity)",					Domain.Parser.Keyword.Parity,				null));
				yield return (new TestCaseData(@"\!(Parity())",					Domain.Parser.Keyword.Parity,				null));
				yield return (new TestCaseData(@"\!(Parity(0))",				Domain.Parser.Keyword.Parity,				new int[] { 0 })); // \remind (2018-06-13 / MKY) yet limited to parsing integer values (FR #404).
				yield return (new TestCaseData(@"\!(Parity(1))",				Domain.Parser.Keyword.Parity,				new int[] { 1 })); // \remind (2018-06-13 / MKY) yet limited to parsing integer values (FR #404).
				yield return (new TestCaseData(@"\!(Parity(2))",				Domain.Parser.Keyword.Parity,				new int[] { 2 })); // \remind (2018-06-13 / MKY) yet limited to parsing integer values (FR #404).
				yield return (new TestCaseData(@"\!(StopBits)",					Domain.Parser.Keyword.StopBits,				null));
				yield return (new TestCaseData(@"\!(StopBits())",				Domain.Parser.Keyword.StopBits,				null));
				yield return (new TestCaseData(@"\!(StopBits(0))",				Domain.Parser.Keyword.StopBits,				new int[] { 0 })); // \remind (2018-06-13 / MKY) yet limited to parsing integer values (FR #404).
				yield return (new TestCaseData(@"\!(StopBits(1))",				Domain.Parser.Keyword.StopBits,				new int[] { 1 })); // \remind (2018-06-13 / MKY) yet limited to parsing integer values (FR #404).
				yield return (new TestCaseData(@"\!(StopBits(2))",				Domain.Parser.Keyword.StopBits,				new int[] { 2 })); // \remind (2018-06-13 / MKY) yet limited to parsing integer values (FR #404).
				yield return (new TestCaseData(@"\!(FlowControl)",				Domain.Parser.Keyword.FlowControl,			null));
				yield return (new TestCaseData(@"\!(FlowControl())",			Domain.Parser.Keyword.FlowControl,			null));
				yield return (new TestCaseData(@"\!(FlowControl(0))",			Domain.Parser.Keyword.FlowControl,			new int[] { 0 })); // \remind (2018-06-13 / MKY) yet limited to parsing integer values (FR #404).
				yield return (new TestCaseData(@"\!(FlowControl(1))",			Domain.Parser.Keyword.FlowControl,			new int[] { 1 })); // \remind (2018-06-13 / MKY) yet limited to parsing integer values (FR #404).
				yield return (new TestCaseData(@"\!(FlowControl(2))",			Domain.Parser.Keyword.FlowControl,			new int[] { 2 })); // \remind (2018-06-13 / MKY) yet limited to parsing integer values (FR #404).
				yield return (new TestCaseData(@"\!(FlowControl(3))",			Domain.Parser.Keyword.FlowControl,			new int[] { 3 })); // \remind (2018-06-13 / MKY) yet limited to parsing integer values (FR #404).
				yield return (new TestCaseData(@"\!(RTSOn)",					Domain.Parser.Keyword.RtsOn,				null));
				yield return (new TestCaseData(@"\!(RTSOn())",					Domain.Parser.Keyword.RtsOn,				null));
				yield return (new TestCaseData(@"\!(RTSOff)",					Domain.Parser.Keyword.RtsOff,				null));
				yield return (new TestCaseData(@"\!(RTSOff())",					Domain.Parser.Keyword.RtsOff,				null));
				yield return (new TestCaseData(@"\!(RTSToggle)",				Domain.Parser.Keyword.RtsToggle,			null));
				yield return (new TestCaseData(@"\!(RTSToggle())",				Domain.Parser.Keyword.RtsToggle,			null));
				yield return (new TestCaseData(@"\!(DTROn)",					Domain.Parser.Keyword.DtrOn,				null));
				yield return (new TestCaseData(@"\!(DTROn())",					Domain.Parser.Keyword.DtrOn,				null));
				yield return (new TestCaseData(@"\!(DTROff)",					Domain.Parser.Keyword.DtrOff,				null));
				yield return (new TestCaseData(@"\!(DTROff())",					Domain.Parser.Keyword.DtrOff,				null));
				yield return (new TestCaseData(@"\!(DTRToggle)",				Domain.Parser.Keyword.DtrToggle,			null));
				yield return (new TestCaseData(@"\!(DTRToggle())",				Domain.Parser.Keyword.DtrToggle,			null));
				yield return (new TestCaseData(@"\!(OutputBreakOn)",			Domain.Parser.Keyword.OutputBreakOn,		null));
				yield return (new TestCaseData(@"\!(OutputBreakOn())",			Domain.Parser.Keyword.OutputBreakOn,		null));
				yield return (new TestCaseData(@"\!(OutputBreakOff)",			Domain.Parser.Keyword.OutputBreakOff,		null));
				yield return (new TestCaseData(@"\!(OutputBreakOff())",			Domain.Parser.Keyword.OutputBreakOff,		null));
				yield return (new TestCaseData(@"\!(OutputBreakToggle)",		Domain.Parser.Keyword.OutputBreakToggle,	null));
				yield return (new TestCaseData(@"\!(OutputBreakToggle())",		Domain.Parser.Keyword.OutputBreakToggle,	null));
				yield return (new TestCaseData(@"\!(FramingErrorsOn)",			Domain.Parser.Keyword.FramingErrorsOn,		null));
				yield return (new TestCaseData(@"\!(FramingErrorsOn())",		Domain.Parser.Keyword.FramingErrorsOn,		null));
				yield return (new TestCaseData(@"\!(FramingErrorsOff)",			Domain.Parser.Keyword.FramingErrorsOff,		null));
				yield return (new TestCaseData(@"\!(FramingErrorsOff())",		Domain.Parser.Keyword.FramingErrorsOff,		null));
				yield return (new TestCaseData(@"\!(FramingErrorsRestore)",		Domain.Parser.Keyword.FramingErrorsRestore,	null));
				yield return (new TestCaseData(@"\!(FramingErrorsRestore())",	Domain.Parser.Keyword.FramingErrorsRestore,	null));
				yield return (new TestCaseData(@"\!(ReportID)",					Domain.Parser.Keyword.ReportId,				null));
				yield return (new TestCaseData(@"\!(ReportID())",				Domain.Parser.Keyword.ReportId,				null));
				yield return (new TestCaseData(@"\!(ReportID(0))",				Domain.Parser.Keyword.ReportId,				new int[] { 0x00 }));
				yield return (new TestCaseData(@"\!(ReportID(1))",				Domain.Parser.Keyword.ReportId,				new int[] { 0x01 }));
				yield return (new TestCaseData(@"\!(ReportID(0x00))",			Domain.Parser.Keyword.ReportId,				new int[] { 0x00 }));
				yield return (new TestCaseData(@"\!(ReportID(0x3F))",			Domain.Parser.Keyword.ReportId,				new int[] { 0x3F }));
				yield return (new TestCaseData(@"\!(ReportID(0xFF))",			Domain.Parser.Keyword.ReportId,				new int[] { 0xFF }));

				// Whitespace:
				yield return (new TestCaseData(@"\!(Delay )",					Domain.Parser.Keyword.Delay, null));
				yield return (new TestCaseData(@"\!( Delay)",					Domain.Parser.Keyword.Delay, null));
				yield return (new TestCaseData(@"\!( Delay )",					Domain.Parser.Keyword.Delay, null));
				yield return (new TestCaseData(@"\!(   Delay   )",				Domain.Parser.Keyword.Delay, null));
				yield return (new TestCaseData(@"\!(Delay(10) )",				Domain.Parser.Keyword.Delay, new int[] { 10 }));
				yield return (new TestCaseData(@"\!(Delay (10))",				Domain.Parser.Keyword.Delay, new int[] { 10 }));
				yield return (new TestCaseData(@"\!(Delay (10) )",				Domain.Parser.Keyword.Delay, new int[] { 10 }));
				yield return (new TestCaseData(@"\!(Delay   (10)   )",			Domain.Parser.Keyword.Delay, new int[] { 10 }));
				yield return (new TestCaseData(@"\!(Delay(10 ))",				Domain.Parser.Keyword.Delay, new int[] { 10 }));
				yield return (new TestCaseData(@"\!(Delay( 10))",				Domain.Parser.Keyword.Delay, new int[] { 10 }));
				yield return (new TestCaseData(@"\!(Delay( 10 ))",				Domain.Parser.Keyword.Delay, new int[] { 10 }));
				yield return (new TestCaseData(@"\!(Delay(   10   ))",			Domain.Parser.Keyword.Delay, new int[] { 10 }));
				yield return (new TestCaseData(@"\!(Delay(   10   )",			Domain.Parser.Keyword.Delay, new int[] { 10 }));
				yield return (new TestCaseData(@"\!(Delay(   10   ",			Domain.Parser.Keyword.Delay, new int[] { 10 }));

				// Open:
				yield return (new TestCaseData(@"\!(Clear",						Domain.Parser.Keyword.Clear, null));
				yield return (new TestCaseData(@"\!(Clear   ",					Domain.Parser.Keyword.Clear, null));
				yield return (new TestCaseData(@"\!(Clear(",					Domain.Parser.Keyword.Clear, null));
				yield return (new TestCaseData(@"\!(Clear(   ",					Domain.Parser.Keyword.Clear, null));
				yield return (new TestCaseData(@"\!(Clear()",					Domain.Parser.Keyword.Clear, null));
				yield return (new TestCaseData(@"\!(Clear()   ",				Domain.Parser.Keyword.Clear, null));
				yield return (new TestCaseData(@"\!(Delay",						Domain.Parser.Keyword.Delay, null));
				yield return (new TestCaseData(@"\!(Delay   ",					Domain.Parser.Keyword.Delay, null));
				yield return (new TestCaseData(@"\!(Delay(",					Domain.Parser.Keyword.Delay, null));
				yield return (new TestCaseData(@"\!(Delay(   ",					Domain.Parser.Keyword.Delay, null));
				yield return (new TestCaseData(@"\!(Delay()",					Domain.Parser.Keyword.Delay, null));
				yield return (new TestCaseData(@"\!(Delay()   ",				Domain.Parser.Keyword.Delay, null));

				// Empty:
				yield return (new TestCaseData(@"\!(Clear)",					Domain.Parser.Keyword.Clear, null));
				yield return (new TestCaseData(@"\!(Clear())",					Domain.Parser.Keyword.Clear, null));
				yield return (new TestCaseData(@"\!(Clear ( ) )",				Domain.Parser.Keyword.Clear, null));
				yield return (new TestCaseData(@"\!(Clear   (   )   )",			Domain.Parser.Keyword.Clear, null));
				yield return (new TestCaseData(@"\!(Delay)",					Domain.Parser.Keyword.Delay, null));
				yield return (new TestCaseData(@"\!(Delay())",					Domain.Parser.Keyword.Delay, null));
				yield return (new TestCaseData(@"\!(Delay ( ) )",				Domain.Parser.Keyword.Delay, null));
				yield return (new TestCaseData(@"\!(Delay   (   )   )",			Domain.Parser.Keyword.Delay, null));

				// Case:
				yield return (new TestCaseData(@"\!(CLeAr)",					Domain.Parser.Keyword.Clear, null));
				yield return (new TestCaseData(@"\!(cLEAr)",					Domain.Parser.Keyword.Clear, null));
				yield return (new TestCaseData(@"\!(CLEAR)",					Domain.Parser.Keyword.Clear, null));
				yield return (new TestCaseData(@"\!(DeLAy)",					Domain.Parser.Keyword.Delay, null));
				yield return (new TestCaseData(@"\!(deLAY)",					Domain.Parser.Keyword.Delay, null));
				yield return (new TestCaseData(@"\!(DELAY)",					Domain.Parser.Keyword.Delay, null));

				// Single:
				yield return (new TestCaseData(@"\!(ZZZ_FIT(1))",				Domain.Parser.Keyword.ZZZ_FIT, new int[] { 1 }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT ( 1 ) )",			Domain.Parser.Keyword.ZZZ_FIT, new int[] { 1 }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT   (   1   )   )",	Domain.Parser.Keyword.ZZZ_FIT, new int[] { 1 }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT   (   1   )   ",	Domain.Parser.Keyword.ZZZ_FIT, new int[] { 1 }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT   (   1   ",		Domain.Parser.Keyword.ZZZ_FIT, new int[] { 1 }));

				// Multiple:
				yield return (new TestCaseData(@"\!(ZZZ_FIT( 1 , 2 , 3 ))",		Domain.Parser.Keyword.ZZZ_FIT, new int[] { 1, 2, 3 }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(1 , 2 , 3))",		Domain.Parser.Keyword.ZZZ_FIT, new int[] { 1, 2, 3 }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT( 1 ,2 ,3 ))",		Domain.Parser.Keyword.ZZZ_FIT, new int[] { 1, 2, 3 }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(1 ,2 ,3))",			Domain.Parser.Keyword.ZZZ_FIT, new int[] { 1, 2, 3 }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT( 1,2,3 ))",			Domain.Parser.Keyword.ZZZ_FIT, new int[] { 1, 2, 3 }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(1,2,3))",			Domain.Parser.Keyword.ZZZ_FIT, new int[] { 1, 2, 3 }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(0,1,2))",			Domain.Parser.Keyword.ZZZ_FIT, new int[] { 0, 1, 2 }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(0,1,2)",			Domain.Parser.Keyword.ZZZ_FIT, new int[] { 0, 1, 2 }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(0,1,2",				Domain.Parser.Keyword.ZZZ_FIT, new int[] { 0, 1, 2 }));

				// Partial:
				yield return (new TestCaseData(@"\!(ZZZ_FIT(1, 2))",			Domain.Parser.Keyword.ZZZ_FIT, new int[] { 1, 2 }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(0, 1))",			Domain.Parser.Keyword.ZZZ_FIT, new int[] { 0, 1 }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(0, 1)",				Domain.Parser.Keyword.ZZZ_FIT, new int[] { 0, 1 }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(0, 1",				Domain.Parser.Keyword.ZZZ_FIT, new int[] { 0, 1 }));

				// Sign:
				yield return (new TestCaseData(@"\!(ZZZ_FIT(1, -2, 3))",		Domain.Parser.Keyword.ZZZ_FIT, new int[] { 1, -2, 3 }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(1, 2, +3))",		Domain.Parser.Keyword.ZZZ_FIT, new int[] { 1, 2, +3 }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(1, 2, +3)",			Domain.Parser.Keyword.ZZZ_FIT, new int[] { 1, 2, +3 }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(1, 2, +3",			Domain.Parser.Keyword.ZZZ_FIT, new int[] { 1, 2, +3 }));

				// Radix:
				yield return (new TestCaseData(@"\!(ZZZ_FIT(0b0))",									Domain.Parser.Keyword.ZZZ_FIT, new int[] { 0x00 }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(0b1))",									Domain.Parser.Keyword.ZZZ_FIT, new int[] { 0x01 }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(0b01))",								Domain.Parser.Keyword.ZZZ_FIT, new int[] { 0x01 }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(0b11))",								Domain.Parser.Keyword.ZZZ_FIT, new int[] { 0x03 }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(0b00000000000000000000000000000001))",	Domain.Parser.Keyword.ZZZ_FIT, new int[] { 0x01 }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(0b01000000000000000000000000000001))",	Domain.Parser.Keyword.ZZZ_FIT, new int[] { 0x40000001 }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(0b01111111111111111111111111111111))",	Domain.Parser.Keyword.ZZZ_FIT, new int[] { 0x7FFFFFFF }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(00))",									Domain.Parser.Keyword.ZZZ_FIT, new int[] { 0 }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(01))",									Domain.Parser.Keyword.ZZZ_FIT, new int[] { 1 }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(001))",									Domain.Parser.Keyword.ZZZ_FIT, new int[] { 1 }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(012))",									Domain.Parser.Keyword.ZZZ_FIT, new int[] { 10 }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(01234567))",							Domain.Parser.Keyword.ZZZ_FIT, new int[] { 342391 }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(000000000001))",						Domain.Parser.Keyword.ZZZ_FIT, new int[] { 1 }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(010000000001))",						Domain.Parser.Keyword.ZZZ_FIT, new int[] { 0x40000001 }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(017777777777))",						Domain.Parser.Keyword.ZZZ_FIT, new int[] { 0x7FFFFFFF }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(0x0))",									Domain.Parser.Keyword.ZZZ_FIT, new int[] { 0x00 }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(0xA))",									Domain.Parser.Keyword.ZZZ_FIT, new int[] { 0x0A }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(0x0A))",								Domain.Parser.Keyword.ZZZ_FIT, new int[] { 0x0A }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(0x1A))",								Domain.Parser.Keyword.ZZZ_FIT, new int[] { 0x1A }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(0x0123CDEF))",							Domain.Parser.Keyword.ZZZ_FIT, new int[] { 0x0123CDEF }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(0x00000001))",							Domain.Parser.Keyword.ZZZ_FIT, new int[] { 0x01 }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(0x40000001))",							Domain.Parser.Keyword.ZZZ_FIT, new int[] { 0x40000001 }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(0x7FFFFFFF))",							Domain.Parser.Keyword.ZZZ_FIT, new int[] { 0x7FFFFFFF }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(0x7FFFFFFF)",							Domain.Parser.Keyword.ZZZ_FIT, new int[] { 0x7FFFFFFF }));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(0x7FFFFFFF",							Domain.Parser.Keyword.ZZZ_FIT, new int[] { 0x7FFFFFFF }));
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
				yield return (new TestCaseData(@"A<A",			@"A<",			@"""A"" is an invalid ASCII mnemonic.").SetName("Invalid ASCII 1"));
				yield return (new TestCaseData(@"A<AA",			@"A<A",			@"""AA"" is an invalid ASCII mnemonic.").SetName("Invalid ASCII 2"));
				yield return (new TestCaseData(@"A<AAA",		@"A<AA",		@"""AAA"" is an invalid ASCII mnemonic.").SetName("Invalid ASCII 3"));

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

				yield return (new TestCaseData(@"\!(ReportId(-1))",		@"\!(ReportId(-1",		@"""-1"" is no valid 0th argument for keyword 'ReportId'. Argument must be an integer value within 0..255 specifying the report ID."));
				yield return (new TestCaseData(@"\!(ReportId(256))",	@"\!(ReportId(256",		@"""256"" is no valid 0th argument for keyword 'ReportId'. Argument must be an integer value within 0..255 specifying the report ID."));
				yield return (new TestCaseData(@"\!(ReportId(0x100))",	@"\!(ReportId(0x100",	@"""0x100"" is no valid 0th argument for keyword 'ReportId'. Argument must be an integer value within 0..255 specifying the report ID."));

				yield return (new TestCaseData(@"\!(ZZZ_FIT(1 2 3))",		@"\!(ZZZ_FIT(1 ",		@"Closing parenthesis expected instead of character '2' (0x32)."));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(1.2.3))",		@"\!(ZZZ_FIT(1",		@"Character '.' (0x2E) is invalid for decimal values."));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(1 2, 3))",		@"\!(ZZZ_FIT(1 ",		@"Closing parenthesis expected instead of character '2' (0x32)."));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(1.2, 3))",		@"\!(ZZZ_FIT(1",		@"Character '.' (0x2E) is invalid for decimal values."));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(1, 2 3))",		@"\!(ZZZ_FIT(1, 2 ",	@"Closing parenthesis expected instead of character '3' (0x33)."));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(1, 2.3))",		@"\!(ZZZ_FIT(1, 2",		@"Character '.' (0x2E) is invalid for decimal values."));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(1 23))",		@"\!(ZZZ_FIT(1 ",		@"Closing parenthesis expected instead of character '2' (0x32)."));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(1.23))",		@"\!(ZZZ_FIT(1",		@"Character '.' (0x2E) is invalid for decimal values."));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(12 3))",		@"\!(ZZZ_FIT(12 ",		@"Closing parenthesis expected instead of character '3' (0x33)."));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(12.3))",		@"\!(ZZZ_FIT(12",		@"Character '.' (0x2E) is invalid for decimal values."));

				yield return (new TestCaseData(@"\!(ZZZ_FIT(1,))",			@"\!(ZZZ_FIT(1,",		@"Empty arguments are no permitted."));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(1 ,))",			@"\!(ZZZ_FIT(1 ,",		@"Empty arguments are no permitted."));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(1,2,))",		@"\!(ZZZ_FIT(1,2,",		@"Empty arguments are no permitted."));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(1,2 ,))",		@"\!(ZZZ_FIT(1,2 ,",	@"Empty arguments are no permitted."));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(,2,3))",		@"\!(ZZZ_FIT(",			@"Empty arguments are no permitted."));
				yield return (new TestCaseData(@"\!(ZZZ_FIT( ,2,3))",		@"\!(ZZZ_FIT( ",		@"Empty arguments are no permitted."));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(1,2,3,))",		@"\!(ZZZ_FIT(1,2,3",	@"Keyword 'ZZZ_FIT' only supports up to 3 arguments."));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(1,2,3 ,))",		@"\!(ZZZ_FIT(1,2,3 ",	@"Keyword 'ZZZ_FIT' only supports up to 3 arguments."));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(1,2,3,4))",		@"\!(ZZZ_FIT(1,2,3",	@"Keyword 'ZZZ_FIT' only supports up to 3 arguments."));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(1,2,3 ,4))",	@"\!(ZZZ_FIT(1,2,3 ",	@"Keyword 'ZZZ_FIT' only supports up to 3 arguments."));

				yield return (new TestCaseData(@"\!(ZZZ_FIT(bla))",			@"\!(ZZZ_FIT(",			@"Character 'b' (0x62) is invalid for decimal values."));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(1,bla))",		@"\!(ZZZ_FIT(1,",		@"Character 'b' (0x62) is invalid for decimal values."));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(1, bla))",		@"\!(ZZZ_FIT(1, ",		@"Character 'b' (0x62) is invalid for decimal values."));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(1,2,bla))",		@"\!(ZZZ_FIT(1,2,",		@"Character 'b' (0x62) is invalid for decimal values."));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(1,2, bla))",	@"\!(ZZZ_FIT(1,2, ",	@"Character 'b' (0x62) is invalid for decimal values."));

				yield return (new TestCaseData(@"\!(ZZZ_FIT(,))",			@"\!(ZZZ_FIT(",			@"Empty arguments are no permitted."));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(,,))",			@"\!(ZZZ_FIT(",			@"Empty arguments are no permitted."));
				yield return (new TestCaseData(@"\!(ZZZ_FIT(,,,))",			@"\!(ZZZ_FIT(",			@"Empty arguments are no permitted."));
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
			using (var p = new Domain.Parser.Parser(Domain.Parser.Mode.AllEscapes)) // Default encoding of UTF-8 is good enough for this test case.
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
		public virtual void TestParserEncoding(Encoding encoding, string s, byte[] expectedBytes)
		{
			// \remind (2017-12-09 / MKY / bug #400)
			// YAT versions 1.99.70 and 1.99.80 used to take the endianness into account when encoding
			// and decoding multi-byte encoded characters. However, it was always done, but of course e.g.
			// UTF-8 is independent on endianness. The endianness would only have to be applied to single
			// multi-byte values, not multi-byte values split into multiple fragments. However, a .NET
			// 'Encoding' object does not tell whether the encoding is potentially endianness capable or
			// not. Thus, it was decided to again remove the character encoding endianness awareness.

			using (var p = new Domain.Parser.Parser(encoding, Endianness.LittleEndian, Domain.Parser.Mode.RadixAndAsciiEscapes))
			{
				byte[] actualBytes;
				string successfullyParsed;
				Assert.That(p.TryParse(s, out actualBytes, out successfullyParsed), Is.True, @"Failed! Only """ + successfullyParsed + @""" could successfully be parsed!");
				Assert.That(actualBytes, Is.EqualTo(expectedBytes));
			}

			using (var p = new Domain.Parser.Parser(encoding, Endianness.BigEndian, Domain.Parser.Mode.RadixAndAsciiEscapes))
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
			using (var p = new Domain.Parser.Parser(Domain.Parser.Mode.AllEscapes)) // Default encoding of UTF-8 is good enough for this test case.
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
			using (var p = new Domain.Parser.Parser(Domain.Parser.Mode.AllEscapes)) // Default encoding of UTF-8 is good enough for this test case.
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
