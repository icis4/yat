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
// MKY Version 1.0.28 Development
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
using System.Diagnostics.CodeAnalysis;
using System.Text;

using NUnit.Framework;

#endregion

namespace MKY.Test.Types
{
	/// <summary></summary>
	public static class EqualsTestData
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
				yield return (new TestCaseData("abc", "abc", true,  true ));
				yield return (new TestCaseData("abc", "aBc", false, true ));
				yield return (new TestCaseData("aBc", "abc", false, true ));

				yield return (new TestCaseData("abc", "",    false, false));
				yield return (new TestCaseData("",    "abc", false, false));
				yield return (new TestCaseData("",    "",    true,  true ));

				yield return (new TestCaseData("abc", null,  false, false));
				yield return (new TestCaseData(null, "abc",  false, false));
				yield return (new TestCaseData(null, null,   true,  true ));
			}
		}

		#endregion
	}

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

				yield return (new TestCaseData("1234\r1234",   4, new string[] { "1234", "1234" }));
				yield return (new TestCaseData("1234\n1234",   4, new string[] { "1234", "1234" }));
				yield return (new TestCaseData("1234\r\n1234", 4, new string[] { "1234", "1234" }));
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
		public static IEnumerable TestCasesQuote
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
		public static IEnumerable TestCasesEndWhiteSpace
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

		#region Tests > Equals
		//------------------------------------------------------------------------------------------
		// Tests > Equals
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "Why not?")]
		[Test, TestCaseSource(typeof(EqualsTestData), "TestCases")]
		public virtual void EqualsOrdinalIgnoreCase(string strA, string strB, bool expected, bool expectedIgnoreCase)
		{
			bool actual;

			actual = StringEx.EqualsOrdinal(strA, strB);
			Assert.That(actual, Is.EqualTo(expected));

			actual = StringEx.EqualsOrdinalIgnoreCase(strA, strB);
			Assert.That(actual, Is.EqualTo(expectedIgnoreCase));
		}

		#endregion

		#region Tests > Index
		//------------------------------------------------------------------------------------------
		// Tests > Index
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "Why not?")]
		[Test, TestCaseSource(typeof(IndexOfOutsideDoubleQuotesTestData), "TestCases")]
		public virtual void IndexOfOutsideDoubleQuotes(string str, string outString, int outIndex, string inString, int inIndex)
		{
			int index = StringEx.IndexOfOutsideDoubleQuotes(str, outString, StringComparison.Ordinal);
			Assert.That(index, Is.EqualTo(outIndex));
		}

		#endregion

		#region Tests > Count
		//------------------------------------------------------------------------------------------
		// Tests > Count
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "chars", Justification = "Parameter naming as similar string methods.")]
		[Test, TestCaseSource(typeof(CountLeftRightTestData), "TestCasesLeft")]
		public virtual void CountLeft(string str, int expectedCount, char[] countChars)
		{
			int actualCount = StringEx.CountLeft(str, countChars);
			Assert.That(actualCount, Is.EqualTo(expectedCount));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "chars", Justification = "Parameter naming as similar string methods.")]
		[Test, TestCaseSource(typeof(CountLeftRightTestData), "TestCasesRight")]
		public virtual void CountRight(string str, int expectedCount, char[] countChars)
		{
			int actualCount = StringEx.CountRight(str, countChars);
			Assert.That(actualCount, Is.EqualTo(expectedCount));
		}

		#endregion

		#region Tests > Split
		//------------------------------------------------------------------------------------------
		// Tests > Split
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, TestCaseSource(typeof(SplitLeftRightTestData), "TestCasesLeft")]
		public virtual void SplitLeft(string str, int desiredSplitLength, string[] expectedChunks)
		{
			string[] actualChunks = StringEx.SplitLeft(str, desiredSplitLength);

			Assert.That(actualChunks.Length, Is.EqualTo(expectedChunks.Length), "Number of chunks mismatch");

			for (int i = 0; i < expectedChunks.Length; i++)
			{
				Assert.That(actualChunks[i].Length, Is.EqualTo(expectedChunks[i].Length), "Length of chunks mismatch");
				Assert.That(actualChunks[i],        Is.EqualTo(expectedChunks[i]), "Contents of chunks mismatch");
			}
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(SplitLeftRightTestData), "TestCasesRight")]
		public virtual void SplitRight(string str, int desiredSplitLength, string[] expectedChunks)
		{
			string[] actualChunks = StringEx.SplitRight(str, desiredSplitLength);

			Assert.That(actualChunks.Length, Is.EqualTo(expectedChunks.Length), "Number of chunks mismatch");

			for (int i = 0; i < expectedChunks.Length; i++)
			{
				Assert.That(actualChunks[i].Length, Is.EqualTo(expectedChunks[i].Length), "Length of chunks mismatch");
				Assert.That(actualChunks[i],        Is.EqualTo(expectedChunks[i]), "Contents of chunks mismatch");
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "Why not?")]
		[Test, TestCaseSource(typeof(SplitLexicallyTestData), "TestCases")]
		public virtual void SplitLexically(string testString, int desiredChunkLength, string[] expectedChunks)
		{
			string[] actualChunks = StringEx.SplitLexically(testString, desiredChunkLength);

			Assert.That(actualChunks.Length, Is.EqualTo(expectedChunks.Length), "Number of chunks mismatch");

			for (int i = 0; i < expectedChunks.Length; i++)
			{
				Assert.That(actualChunks[i].Length, Is.EqualTo(expectedChunks[i].Length), "Length of chunks mismatch");
				Assert.That(actualChunks[i],        Is.EqualTo(expectedChunks[i]), "Contents of chunks mismatch");
			}
		}

		#endregion

		#region Tests > Trim
		//------------------------------------------------------------------------------------------
		// Tests > Trim
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "chars", Justification = "Parameter naming as similar string methods.")]
		[Test, TestCaseSource(typeof(TrimTestData), "TestCasesQuote")]
		public virtual void TrimOfQuotes(string str, int expectedLength, char[] trimChars)
		{
			var trim = str.Trim(trimChars);

			var sb = new StringBuilder();
			sb.AppendLine("Test string was:");
			sb.AppendLine(str);
			sb.AppendLine("Which was trimmed to:");
			sb.AppendLine(trim);

			Assert.That(trim.Length, Is.EqualTo(expectedLength), sb.ToString());
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "chars", Justification = "Parameter naming as similar string methods.")]
		[Test, TestCaseSource(typeof(TrimTestData), "TestCasesEndWhiteSpace")]
		public virtual void TrimEndOfWhiteSpaces(string str, int expectedLength, char[] trimChars)
		{
			var trim = str.TrimEnd(trimChars);

			var sb = new StringBuilder();
			sb.AppendLine("Test string was:");
			sb.AppendLine(str);
			sb.AppendLine("Which was trimmed to:");
			sb.AppendLine(trim);

			Assert.That(trim.Length, Is.EqualTo(expectedLength), sb.ToString());
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
