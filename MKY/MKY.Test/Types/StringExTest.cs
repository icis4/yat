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
// MKY Development Version 1.0.6
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

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
		[Test, TestCaseSource(typeof(IndexOfOutsideDoubleQuotesTestData), "TestCases")]
		public virtual void TestIndexOfOutsideDoubleQuotes(string testString, string outString, int outIndex, string inString, int inIndex)
		{
			int index = StringEx.IndexOfOutsideDoubleQuotes(testString, outString, StringComparison.InvariantCultureIgnoreCase);
			Assert.AreEqual(outIndex, index);
		}

		#endregion

		#region Tests > SplitLexically()
		//------------------------------------------------------------------------------------------
		// Tests > SplitLexically()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, TestCaseSource(typeof(SplitLexicallyTestData), "TestCases")]
		public virtual void TestSplitLexically(string testString, int desiredChunkSize, string[] expectedChunks)
		{
			string[] actualChunks = StringEx.SplitLexically(testString, desiredChunkSize);
			
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
