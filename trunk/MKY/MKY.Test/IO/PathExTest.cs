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
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Collections;

using NUnit.Framework;

using MKY.IO;

namespace MKY.Test.IO
{
	/// <summary></summary>
	public static class XPathTestData
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
				// ---- Local ----

				//								TS	TC	PATH A								PATH B								RESULT A						RESULT B

				// TS0: Local very-near relation
				yield return (new TestCaseData(0,	0,	@"X:\MyDir",						@"X:\MyDir",						@".",							@"."						));
				yield return (new TestCaseData(0,	1,	@"X:\MyDir",						@"X:\MyDir\MyFile2.abc",			@".\MyFile2.abc",				@"."						));
				yield return (new TestCaseData(0,	2,	@"X:\MyDir\MyFile1.abc",			@"X:\MyDir",						@".",							@".\MyFile1.abc"			));
				yield return (new TestCaseData(0,	3,	@"X:\MyDir\MyFile1.abc",			@"X:\MyDir\MyFile2.abc",			@".\MyFile2.abc",				@".\MyFile1.abc"			));

				// TS1: Local near relation
				yield return (new TestCaseData(1,	0,	@"X:\MyDir",						@"X:\MyDir\MySubDir",				@"MySubDir",					@".."						));
				yield return (new TestCaseData(1,	1,	@"X:\MyDir",						@"X:\MyDir\MySubDir\MyFile2.abc",	@"MySubDir\MyFile2.abc",		@".."						));
				yield return (new TestCaseData(1,	2,	@"X:\MyDir\MyFile1.abc",			@"X:\MyDir\MySubDir",				@"MySubDir",					@"..\MyFile1.abc"			));
				yield return (new TestCaseData(1,	3,	@"X:\MyDir\MyFile1.abc",			@"X:\MyDir\MySubDir\MyFile2.abc",	@"MySubDir\MyFile2.abc",		@"..\MyFile1.abc"			));

				// TS2: Local far relation
				yield return (new TestCaseData(2,	0,	@"X:\MyDir\MySubDir1",				@"X:\MyDir\MySubDir2",				@"..\MySubDir2",				@"..\MySubDir1"				));
				yield return (new TestCaseData(2,	1,	@"X:\MyDir\MySubDir1",				@"X:\MyDir\MySubDir2\MyFile2.abc",	@"..\MySubDir2\MyFile2.abc",	@"..\MySubDir1"				));
				yield return (new TestCaseData(2,	2,	@"X:\MyDir\MySubDir1\MyFile1.abc",	@"X:\MyDir\MySubDir2",				@"..\MySubDir2",				@"..\MySubDir1\MyFile1.abc"	));
				yield return (new TestCaseData(2,	3,	@"X:\MyDir\MySubDir1\MyFile1.abc",	@"X:\MyDir\MySubDir2\MyFile2.abc",	@"..\MySubDir2\MyFile2.abc",	@"..\MySubDir1\MyFile1.abc"	));

				// TS3: Local root relation
				yield return (new TestCaseData(3,	0,	@"X:\",								@"X:\MyDir\MySubDir",				@"MyDir\MySubDir",				@"..\.."					));
				yield return (new TestCaseData(3,	1,	@"X:\",								@"X:\MyDir\MySubDir\MyFile2.abc",	@"MyDir\MySubDir\MyFile2.abc",	@"..\.."					));
				yield return (new TestCaseData(3,	2,	@"X:\MyFile1.abc",					@"X:\MyDir\MySubDir",				@"MyDir\MySubDir",				@"..\..\MyFile1.abc"		));
				yield return (new TestCaseData(3,	3,	@"X:\MyFile1.abc",					@"X:\MyDir\MySubDir\MyFile2.abc",	@"MyDir\MySubDir\MyFile2.abc",	@"..\..\MyFile1.abc"		));

				// TS4: Local root far relation
				yield return (new TestCaseData(4,	0,	@"X:\MyDir1",						@"X:\MyDir2",						@"..\MyDir2",					@"..\MyDir1"				));
				yield return (new TestCaseData(4,	1,	@"X:\MyDir1",						@"X:\MyDir2\MyFile2.abc",			@"..\MyDir2\MyFile2.abc",		@"..\MyDir1"				));
				yield return (new TestCaseData(4,	2,	@"X:\MyDir1\MyFile1.abc",			@"X:\MyDir2",						@"..\MyDir2",					@"..\MyDir1\MyFile1.abc"	));
				yield return (new TestCaseData(4,	3,	@"X:\MyDir1\MyFile1.abc",			@"X:\MyDir2\MyFile2.abc",			@"..\MyDir2\MyFile2.abc",		@"..\MyDir1\MyFile1.abc"	));

				// ---- Network ----

				//								TS	TC	PATH A												PATH B												RESULT A						RESULT B

				// TS5: Network very-near relation
				yield return (new TestCaseData(5,	0,	@"\\MyServer\MyShare\MyDir",						@"\\MyServer\MyShare\MyDir",						@".",							@"."						));
				yield return (new TestCaseData(5,	1,	@"\\MyServer\MyShare\MyDir",						@"\\MyServer\MyShare\MyDir\MyFile2.abc",			@".\MyFile2.abc",				@"."						));
				yield return (new TestCaseData(5,	2,	@"\\MyServer\MyShare\MyDir\MyFile1.abc",			@"\\MyServer\MyShare\MyDir",						@".",							@".\MyFile1.abc"			));
				yield return (new TestCaseData(5,	3,	@"\\MyServer\MyShare\MyDir\MyFile1.abc",			@"\\MyServer\MyShare\MyDir\MyFile2.abc",			@".\MyFile2.abc",				@".\MyFile1.abc"			));

				// TS6: Network near relation
				yield return (new TestCaseData(6,	0,	@"\\MyServer\MyShare\MyDir",						@"\\MyServer\MyShare\MyDir\MySubDir",				@"MySubDir",					@".."						));
				yield return (new TestCaseData(6,	1,	@"\\MyServer\MyShare\MyDir",						@"\\MyServer\MyShare\MyDir\MySubDir\MyFile2.abc",	@"MySubDir\MyFile2.abc",		@".."						));
				yield return (new TestCaseData(6,	2,	@"\\MyServer\MyShare\MyDir\MyFile1.abc",			@"\\MyServer\MyShare\MyDir\MySubDir",				@"MySubDir",					@"..\MyFile1.abc"			));
				yield return (new TestCaseData(6,	3,	@"\\MyServer\MyShare\MyDir\MyFile1.abc",			@"\\MyServer\MyShare\MyDir\MySubDir\MyFile2.abc",	@"MySubDir\MyFile2.abc",		@"..\MyFile1.abc"			));

				// TS7: Network far relation
				yield return (new TestCaseData(7,	0,	@"\\MyServer\MyShare\MyDir\MySubDir1",				@"\\MyServer\MyShare\MyDir\MySubDir2",				@"..\MySubDir2",				@"..\MySubDir1"				));
				yield return (new TestCaseData(7,	1,	@"\\MyServer\MyShare\MyDir\MySubDir1",				@"\\MyServer\MyShare\MyDir\MySubDir2\MyFile2.abc",	@"..\MySubDir2\MyFile2.abc",	@"..\MySubDir1"				));
				yield return (new TestCaseData(7,	2,	@"\\MyServer\MyShare\MyDir\MySubDir1\MyFile1.abc",	@"\\MyServer\MyShare\MyDir\MySubDir2",				@"..\MySubDir2",				@"..\MySubDir1\MyFile1.abc"	));
				yield return (new TestCaseData(7,	3,	@"\\MyServer\MyShare\MyDir\MySubDir1\MyFile1.abc",	@"\\MyServer\MyShare\MyDir\MySubDir2\MyFile2.abc",	@"..\MySubDir2\MyFile2.abc",	@"..\MySubDir1\MyFile1.abc"	));

				// TS8: Network root relation
				yield return (new TestCaseData(8,	0,	@"\\MyServer\MyShare",								@"\\MyServer\MyShare\MyDir\MySubDir",				@"MyDir\MySubDir",				@"..\.."					));
				yield return (new TestCaseData(8,	1,	@"\\MyServer\MyShare",								@"\\MyServer\MyShare\MyDir\MySubDir\MyFile2.abc",	@"MyDir\MySubDir\MyFile2.abc",	@"..\.."					));
				yield return (new TestCaseData(8,	2,	@"\\MyServer\MyShare\MyFile1.abc",					@"\\MyServer\MyShare\MyDir\MySubDir",				@"MyDir\MySubDir",				@"..\..\MyFile1.abc"		));
				yield return (new TestCaseData(8,	3,	@"\\MyServer\MyShare\MyFile1.abc",					@"\\MyServer\MyShare\MyDir\MySubDir\MyFile2.abc",	@"MyDir\MySubDir\MyFile2.abc",	@"..\..\MyFile1.abc"		));

				// TS9: Network root far relation
				yield return (new TestCaseData(9,	0,	@"\\MyServer\MyShare\MyDir1",						@"\\MyServer\MyShare\MyDir2",						@"..\MyDir2",					@"..\MyDir1"				));
				yield return (new TestCaseData(9,	1,	@"\\MyServer\MyShare\MyDir1",						@"\\MyServer\MyShare\MyDir2\MyFile2.abc",			@"..\MyDir2\MyFile2.abc",		@"..\MyDir1"				));
				yield return (new TestCaseData(9,	2,	@"\\MyServer\MyShare\MyDir1\MyFile1.abc",			@"\\MyServer\MyShare\MyDir2",						@"..\MyDir2",					@"..\MyDir1\MyFile1.abc"	));
				yield return (new TestCaseData(9,	3,	@"\\MyServer\MyShare\MyDir1\MyFile1.abc",			@"\\MyServer\MyShare\MyDir2\MyFile2.abc",			@"..\MyDir2\MyFile2.abc",		@"..\MyDir1\MyFile1.abc"	));
			}
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class PathExTest
	{
		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		#region Tests > Compare()
		//------------------------------------------------------------------------------------------
		// Tests > Compare()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, TestCaseSource(typeof(XPathTestData), "TestCases")]
		public virtual void TestCompare(int testSet, int testCase, string pathA, string pathB, string expectedA, string expectedB)
		{
			// Test set is given as additional argument to ease searching for errornous test cases.
			UnusedArg.PreventAnalysisWarning(testSet);

			PathCompareResult result;

			// A compared to B results in A relative.
			switch (testCase)
			{
				case 0:  result = PathEx.CompareDirectoryPaths       (pathA, pathB); break;
				case 1:  result = PathEx.CompareDirectoryAndFilePaths(pathA, pathB); break; // DIR vs FILE.
				case 2:  result = PathEx.CompareFileAndDirectoryPaths(pathA, pathB); break; // FILE vs DIR.
				default: result = PathEx.CompareFilePaths            (pathA, pathB); break;
			}
			Assert.AreEqual(expectedA, result.RelativePath, "A compared to B doesn't result in A relative");

			// B compared to A results in B relative.
			switch (testCase)
			{
				case 0:  result = PathEx.CompareDirectoryPaths       (pathB, pathA); break;
				case 1:  result = PathEx.CompareFileAndDirectoryPaths(pathB, pathA); break; // FILE vs DIR.
				case 2:  result = PathEx.CompareDirectoryAndFilePaths(pathB, pathA); break; // DIR vs FILE.
				default: result = PathEx.CompareFilePaths            (pathB, pathA); break;
			}
			Assert.AreEqual(expectedB, result.RelativePath, "B compared to A doesn't result in B relative");
		}

		#endregion

		#region Tests > Combine()
		//------------------------------------------------------------------------------------------
		// Tests > Combine()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, TestCaseSource(typeof(XPathTestData), "TestCases")]
		public virtual void TestCombine(int testSet, int testCase, string pathA, string pathB, string expectedA, string expectedB)
		{
			// Test set is given as additional argument to ease searching for errornous test cases.
			UnusedArg.PreventAnalysisWarning(testSet);

			string result = "";

			// A combined with A relative results in B.
			switch (testCase)
			{
				case 0:  result = PathEx.CombineDirectoryPaths       (pathA, expectedA); break;
				case 1:  result = PathEx.CombineDirectoryAndFilePaths(pathA, expectedA); break; // DIR vs FILE.
				case 2:  result = PathEx.CombineFileAndDirectoryPaths(pathA, expectedA); break; // FILE vs DIR.
				default: result = PathEx.CombineFilePaths            (pathA, expectedA); break;
			}
			Assert.AreEqual(pathB, result, "A combined with A relative doesn't result in B");

			// B combined with B relative results in A.
			switch (testCase)
			{
				case 0:  result = PathEx.CombineDirectoryPaths       (pathB, expectedB); break;
				case 1:  result = PathEx.CombineFileAndDirectoryPaths(pathB, expectedB); break; // FILE vs DIR.
				case 2:  result = PathEx.CombineDirectoryAndFilePaths(pathB, expectedB); break; // DIR vs FILE.
				default: result = PathEx.CombineFilePaths            (pathB, expectedB); break;
			}
			Assert.AreEqual(pathA, result, "B combined with B relative doesn't result in A");
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
