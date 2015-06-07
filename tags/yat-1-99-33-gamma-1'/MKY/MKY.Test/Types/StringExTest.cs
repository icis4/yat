//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.12
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

using NUnit.Framework;

namespace MKY.Test.Types
{
	/// <summary></summary>
	public static class IndexOfOutsideDoubleQuotesTestData
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
				yield return (new TestCaseData(@"out",                "out",  0, "in", -1));
				yield return (new TestCaseData(@"out""in",            "out",  0, "in",  4));
				yield return (new TestCaseData(@"out""in""",          "out",  0, "in",  4));
				yield return (new TestCaseData(@"out""in""out",       "out",  0, "in",  4));
				yield return (new TestCaseData(@"""in""out""in""",    "out",  4, "in",  1));
				yield return (new TestCaseData(@"""in""out""in""out", "out",  4, "in",  1));
				yield return (new TestCaseData(@"""in""""in""out",    "out",  8, "in",  1));
				yield return (new TestCaseData(@"""in""""in""",       "out", -1, "in",  1));

				yield return (new TestCaseData(@"out\""in""out",      "out",  0, "in",  4));
				yield return (new TestCaseData(@"""in\""out""in""",   "out",  4, "in",  0));
				yield return (new TestCaseData(@"out""in\""out",      "out",  0, "in",  4));
				yield return (new TestCaseData(@"""in""out\""in""",   "out",  4, "in",  0));
				yield return (new TestCaseData(@"out\""in\""out",     "out",  0, "in",  4));
				yield return (new TestCaseData(@"""in\""out\""in""",  "out",  4, "in",  0));
			}
		}

		#endregion
	}

	/// <summary></summary>
	public static class CountLeftRightTestData
	{
		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		public static IEnumerable TestCasesLeft
		{
			get
			{
				yield return (new TestCaseData("aaa", 3, new char[] { 'a' }));
				yield return (new TestCaseData("aab", 2, new char[] { 'a' }));
				yield return (new TestCaseData("abc", 1, new char[] { 'a' }));
				yield return (new TestCaseData("bcd", 0, new char[] { 'a' }));
				yield return (new TestCaseData("",    0, new char[] { 'a' }));

				yield return (new TestCaseData("abc", 2, new char[] { 'a', 'b' }));
				yield return (new TestCaseData("bac", 2, new char[] { 'a', 'b' }));
				yield return (new TestCaseData("acb", 1, new char[] { 'a', 'b' }));
				yield return (new TestCaseData("cba", 0, new char[] { 'a', 'b' }));
				yield return (new TestCaseData("",    0, new char[] { 'a', 'b' }));

				yield return (new TestCaseData(@"""",   1, new char[] { '"' }));
				yield return (new TestCaseData(@"""""", 2, new char[] { '"' }));
				yield return (new TestCaseData(@"\\""", 0, new char[] { '"' }));
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCasesRight
		{
			get
			{
				yield return (new TestCaseData("aaa", 3, new char[] { 'a' }));
				yield return (new TestCaseData("baa", 2, new char[] { 'a' }));
				yield return (new TestCaseData("cba", 1, new char[] { 'a' }));
				yield return (new TestCaseData("dcb", 0, new char[] { 'a' }));
				yield return (new TestCaseData("",    0, new char[] { 'a' }));

				yield return (new TestCaseData("cba", 2, new char[] { 'a', 'b' }));
				yield return (new TestCaseData("cab", 2, new char[] { 'a', 'b' }));
				yield return (new TestCaseData("bca", 1, new char[] { 'a', 'b' }));
				yield return (new TestCaseData("abc", 0, new char[] { 'a', 'b' }));
				yield return (new TestCaseData("",    0, new char[] { 'a', 'b' }));

				yield return (new TestCaseData(@"""",   1, new char[] { '"' }));
				yield return (new TestCaseData(@"""""", 2, new char[] { '"' }));
				yield return (new TestCaseData(@"\\""", 1, new char[] { '"' }));
			}
		}

		#endregion
	}

	/// <summary></summary>
	public static class SplitLeftRightTestData
	{
		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		public static IEnumerable TestCasesLeft
		{
			get
			{
				yield return (new TestCaseData("abc", 0, new string[] { "", "abc" }));
				yield return (new TestCaseData("abc", 1, new string[] { "a", "bc" }));
				yield return (new TestCaseData("abc", 2, new string[] { "ab", "c" }));
				yield return (new TestCaseData("abc", 3, new string[] { "abc", "" }));
				yield return (new TestCaseData("abc", 4, new string[] { "abc", "" }));
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCasesRight
		{
			get
			{
				yield return (new TestCaseData("abc", 0, new string[] { "abc", "" }));
				yield return (new TestCaseData("abc", 1, new string[] { "ab", "c" }));
				yield return (new TestCaseData("abc", 2, new string[] { "a", "bc" }));
				yield return (new TestCaseData("abc", 3, new string[] { "", "abc" }));
				yield return (new TestCaseData("abc", 4, new string[] { "", "abc" }));
			}
		}

		#endregion
	}

	/// <summary></summary>
	public static class SplitLexicallyTestData
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
				yield return (new TestCaseData("a bb ccc dddd eeeee", 10, new string[] { "a bb ccc", "dddd eeeee" }));
				yield return (new TestCaseData("a bb ccc dddd eeeee",  5, new string[] { "a bb", "ccc", "dddd", "eeeee" }));
				yield return (new TestCaseData("a bb ccc dddd eeeee",  4, new string[] { "a bb", "ccc", "dddd", "eeee", "e" }));
				yield return (new TestCaseData("a bb ccc dddd eeeee",  3, new string[] { "a", "bb", "ccc", "ddd", "d", "eee", "ee" }));

				yield return (new TestCaseData("1", 4, new string[] { "1" }));
				yield return (new TestCaseData("1" + Environment.NewLine + "123456", 4, new string[] { "1", "1234", "56" }));
				yield return (new TestCaseData("1" + Environment.NewLine + "123456" + Environment.NewLine + "78", 4, new string[] { "1", "1234", "56", "78" }));

				yield return (new TestCaseData("1234\n1234",   4, new string[] { "1234", "1234" }));
				yield return (new TestCaseData("1234\r1234",   4, new string[] { "1234", "1234" }));
				yield return (new TestCaseData("1234\n\r1234", 4, new string[] { "1234", "1234" }));
				yield return (new TestCaseData("1234" + Environment.NewLine + "1234", 4, new string[] { "1234", "1234" }));
			}
		}

		#endregion
	}

	/// <summary></summary>
	public static class TrimTestData
	{
		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		public static IEnumerable QuoteTestCases
		{
			get
			{
				yield return (new TestCaseData(@"""", 0, new char[] { '"' }));
				yield return (new TestCaseData(@"""a", 1, new char[] { '"' }));
				yield return (new TestCaseData(@"""a""", 1, new char[] { '"' }));
				yield return (new TestCaseData(@"""""a""", 1, new char[] { '"' }));
				yield return (new TestCaseData(@"""""a""""", 1, new char[] { '"' }));

			////yield return (new TestCaseData(@"\""", 0, new char[] { '"' }));
			////yield return (new TestCaseData(@"\""a", 1, new char[] { '"' }));
			////yield return (new TestCaseData(@"\""a""", 1, new char[] { '"' }));
			////yield return (new TestCaseData(@"\""""a""", 1, new char[] { '"' }));
			////yield return (new TestCaseData(@"\""""a""""", 1, new char[] { '"' }));
			////yield return (new TestCaseData(@"\""\""a""""", 1, new char[] { '"' }));

				yield return (new TestCaseData(@"""", 0, new char[] { '"' }));
				yield return (new TestCaseData(@"""a", 1, new char[] { '"' }));
			////yield return (new TestCaseData(@"""a\""", 1, new char[] { '"' }));
			////yield return (new TestCaseData(@"""""a\""", 1, new char[] { '"' }));
			////yield return (new TestCaseData(@"""""a""\""", 1, new char[] { '"' }));
			////yield return (new TestCaseData(@"""""a\""\""", 1, new char[] { '"' }));
			}
		}

		/// <summary></summary>
		public static IEnumerable EndWhiteSpaceTestCases
		{
			get
			{
				yield return (new TestCaseData("a",    1, new char[] { ' ' }));
				yield return (new TestCaseData("a ",   1, new char[] { ' ' }));
				yield return (new TestCaseData("a \t", 3, new char[] { ' ' }));
				yield return (new TestCaseData("a\t",  2, new char[] { ' ' }));
				yield return (new TestCaseData("a\t ", 2, new char[] { ' ' }));
				yield return (new TestCaseData("a a",  3, new char[] { ' ' }));

				yield return (new TestCaseData("a",    1, new char[] { '\t' }));
				yield return (new TestCaseData("a ",   2, new char[] { '\t' }));
				yield return (new TestCaseData("a \t", 2, new char[] { '\t' }));
				yield return (new TestCaseData("a\t",  1, new char[] { '\t' }));
				yield return (new TestCaseData("a\t ", 3, new char[] { '\t' }));
				yield return (new TestCaseData("a a",  3, new char[] { '\t' }));

				yield return (new TestCaseData("a",    1, new char[] { ' ', '\t' }));
				yield return (new TestCaseData("a ",   1, new char[] { ' ', '\t' }));
				yield return (new TestCaseData("a \t", 1, new char[] { ' ', '\t' }));
				yield return (new TestCaseData("a\t",  1, new char[] { ' ', '\t' }));
				yield return (new TestCaseData("a\t ", 1, new char[] { ' ', '\t' }));
				yield return (new TestCaseData("a a",  3, new char[] { ' ', '\t' }));

				yield return (new TestCaseData("a",    1, null));
				yield return (new TestCaseData("a ",   1, null));
				yield return (new TestCaseData("a \t", 1, null));
				yield return (new TestCaseData("a\t",  1, null));
				yield return (new TestCaseData("a\t ", 1, null));
				yield return (new TestCaseData("a a",  3, null));
			}
		}
		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class StringExTest
	{
		#region Tests
		//==========================================================================================
		// Test
		//==========================================================================================

		#region Tests > IndexOfOutsideDoubleQuotes()
		//------------------------------------------------------------------------------------------
		// Tests > IndexOfOutsideDoubleQuotes()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "Why not?")]
		[Test, TestCaseSource(typeof(IndexOfOutsideDoubleQuotesTestData), "TestCases")]
		public virtual void TestIndexOfOutsideDoubleQuotes(string str, string outString, int outIndex, string inString, int inIndex)
		{
			int index = StringEx.IndexOfOutsideDoubleQuotes(str, outString, StringComparison.Ordinal);
			Assert.AreEqual(outIndex, index);
		}

		#endregion

		#region Tests > CountLeft()
		//------------------------------------------------------------------------------------------
		// Tests > CountLeft()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "chars", Justification = "Parameter naming as similar string methods.")]
		[Test, TestCaseSource(typeof(CountLeftRightTestData), "TestCasesLeft")]
		public virtual void CountLeft(string str, int expectedCount, char[] countChars)
		{
			int actualCount = StringEx.CountLeft(str, countChars);
			Assert.AreEqual(expectedCount, actualCount);
		}

		#endregion

		#region Tests > CountRight()
		//------------------------------------------------------------------------------------------
		// Tests > CountRight()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "chars", Justification = "Parameter naming as similar string methods.")]
		[Test, TestCaseSource(typeof(CountLeftRightTestData), "TestCasesRight")]
		public virtual void CountRight(string str, int expectedCount, char[] countChars)
		{
			int actualCount = StringEx.CountRight(str, countChars);
			Assert.AreEqual(expectedCount, actualCount);
		}

		#endregion

		#region Tests > SplitLeft()
		//------------------------------------------------------------------------------------------
		// Tests > SplitLeft()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, TestCaseSource(typeof(SplitLeftRightTestData), "TestCasesLeft")]
		public virtual void SplitLeft(string str, int desiredSplitLength, string[] expectedChunks)
		{
			string[] actualChunks = StringEx.SplitLeft(str, desiredSplitLength);

			Assert.AreEqual(expectedChunks.Length, actualChunks.Length, "Number of chunks mismatch");

			for (int i = 0; i < expectedChunks.Length; i++)
			{
				Assert.AreEqual(expectedChunks[i].Length, actualChunks[i].Length, "Length of chunks mismatch");
				Assert.AreEqual(expectedChunks[i], actualChunks[i], "Contents of chunks mismatch");
			}
		}

		#endregion

		#region Tests > SplitRight()
		//------------------------------------------------------------------------------------------
		// Tests > SplitRight()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, TestCaseSource(typeof(SplitLeftRightTestData), "TestCasesRight")]
		public virtual void SplitRight(string str, int desiredSplitLength, string[] expectedChunks)
		{
			string[] actualChunks = StringEx.SplitRight(str, desiredSplitLength);

			Assert.AreEqual(expectedChunks.Length, actualChunks.Length, "Number of chunks mismatch");

			for (int i = 0; i < expectedChunks.Length; i++)
			{
				Assert.AreEqual(expectedChunks[i].Length, actualChunks[i].Length, "Length of chunks mismatch");
				Assert.AreEqual(expectedChunks[i], actualChunks[i], "Contents of chunks mismatch");
			}
		}

		#endregion

		#region Tests > Trim()
		//------------------------------------------------------------------------------------------
		// Tests > Trim()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "chars", Justification = "Parameter naming as similar string methods.")]
		[Test, TestCaseSource(typeof(TrimTestData), "QuoteTestCases")]
		public virtual void TestTrimOfQuotes(string str, int expectedLength, char[] trimChars)
		{
			string trim = str.Trim(trimChars);

			StringBuilder sb = new StringBuilder();
			sb.AppendLine("Test string was:");
			sb.AppendLine(str);
			sb.AppendLine("Which was trimmed to:");
			sb.AppendLine(trim);

			Assert.AreEqual(expectedLength, trim.Length, sb.ToString());
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "chars", Justification = "Parameter naming as similar string methods.")]
		[Test, TestCaseSource(typeof(TrimTestData), "EndWhiteSpaceTestCases")]
		public virtual void TestTrimEndOfWhiteSpaces(string str, int expectedLength, char[] trimChars)
		{
			string trim = str.TrimEnd(trimChars);

			StringBuilder sb = new StringBuilder();
			sb.AppendLine("Test string was:");
			sb.AppendLine(str);
			sb.AppendLine("Which was trimmed to:");
			sb.AppendLine(trim);

			Assert.AreEqual(expectedLength, trim.Length, sb.ToString());
		}

		#endregion

		#region Tests > SplitLexically()
		//------------------------------------------------------------------------------------------
		// Tests > SplitLexically()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "Why not?")]
		[Test, TestCaseSource(typeof(SplitLexicallyTestData), "TestCases")]
		public virtual void TestSplitLexically(string testString, int desiredChunkLength, string[] expectedChunks)
		{
			string[] actualChunks = StringEx.SplitLexically(testString, desiredChunkLength);
			
			Assert.AreEqual(expectedChunks.Length, actualChunks.Length, "Number of chunks mismatch");

			for (int i = 0; i < expectedChunks.Length; i++)
			{
				Assert.AreEqual(expectedChunks[i].Length, actualChunks[i].Length, "Length of chunks mismatch");
				Assert.AreEqual(expectedChunks[i], actualChunks[i], "Contents of chunks mismatch");
			}
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
