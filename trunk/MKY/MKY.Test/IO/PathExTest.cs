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
using System.Collections.Generic;

using NUnit.Framework;

using MKY.IO;

namespace MKY.Test.IO
{
	/// <summary>
	/// \todo
	/// Improve PathEx such that it passes tests for '\' as well as '/'.
	/// Ensure that test vectors potentially run on Windows and Unixoids.
	/// I.e. use of DirectorySeparatorChar must be checked.
	/// </summary>
	public static class PathExTestData
	{
		#region Test Case Data Set
		//==========================================================================================
		// Test Case Data Set
		//==========================================================================================

		private class TestCaseDataSet
		{
			public int TestSet;
			public int TestCase;
			public string PathA;
			public string PathB;
			public string ResultA;
			public string ResultB;

			public TestCaseDataSet(int testSet, int testCase, string pathA, string pathB, string resultA, string resultB)
			{
				TestSet = testSet;
				TestCase = testCase;
				PathA = pathA;
				PathB = pathB;
				ResultA = resultA;
				ResultB = resultB;
			}
		}

		#endregion

		#region Test Cases Base
		//==========================================================================================
		// Test Cases Base
		//==========================================================================================

		/// <summary></summary>
		private static IEnumerable<TestCaseDataSet> TestCasesBase
		{
			get
			{
				// ---- Local ----

				//								TS	TC	PATH A								PATH B								RESULT A	\todo See below			RESULT B

				// TS0: Local very-near relation
				yield return (new TestCaseDataSet(0,	0,	@"X:\MyDir",						@"X:\MyDir",						@".",								@"."						));
				yield return (new TestCaseDataSet(0,	1,	@"X:\MyDir",						@"X:\MyDir\MyFile2.abc",			@".\MyFile2.abc",					@"."						));
				yield return (new TestCaseDataSet(0,	2,	@"X:\MyDir\MyFile1.abc",			@"X:\MyDir",						@".",								@".\MyFile1.abc"			));
				yield return (new TestCaseDataSet(0,	3,	@"X:\MyDir\MyFile1.abc",			@"X:\MyDir\MyFile2.abc",			@".\MyFile2.abc",					@".\MyFile1.abc"			));

				// TS1: Local near relation
				yield return (new TestCaseDataSet(1,	0,	@"X:\MyDir",						@"X:\MyDir\MySubDir",				/*@".\*/@"MySubDir",						@".."						));
				yield return (new TestCaseDataSet(1,	1,	@"X:\MyDir",						@"X:\MyDir\MySubDir\MyFile2.abc",	/*@".\*/@"MySubDir\MyFile2.abc",			@".."						));
				yield return (new TestCaseDataSet(1,	2,	@"X:\MyDir\MyFile1.abc",			@"X:\MyDir\MySubDir",				/*@".\*/@"MySubDir",						@"..\MyFile1.abc"			));
				yield return (new TestCaseDataSet(1,	3,	@"X:\MyDir\MyFile1.abc",			@"X:\MyDir\MySubDir\MyFile2.abc",	/*@".\*/@"MySubDir\MyFile2.abc",			@"..\MyFile1.abc"			));

				// TS2: Local far relation
				yield return (new TestCaseDataSet(2,	0,	@"X:\MyDir\MySubDir1",				@"X:\MyDir\MySubDir2",				@"..\MySubDir2",					@"..\MySubDir1"				));
				yield return (new TestCaseDataSet(2,	1,	@"X:\MyDir\MySubDir1",				@"X:\MyDir\MySubDir2\MyFile2.abc",	@"..\MySubDir2\MyFile2.abc",		@"..\MySubDir1"				));
				yield return (new TestCaseDataSet(2,	2,	@"X:\MyDir\MySubDir1\MyFile1.abc",	@"X:\MyDir\MySubDir2",				@"..\MySubDir2",					@"..\MySubDir1\MyFile1.abc"	));
				yield return (new TestCaseDataSet(2,	3,	@"X:\MyDir\MySubDir1\MyFile1.abc",	@"X:\MyDir\MySubDir2\MyFile2.abc",	@"..\MySubDir2\MyFile2.abc",		@"..\MySubDir1\MyFile1.abc"	));

				// TS3: Local root relation
				yield return (new TestCaseDataSet(3,	0,	@"X:\",								@"X:\MyDir\MySubDir",				/*@".\*/@"MyDir\MySubDir",				@"..\.."					));
				yield return (new TestCaseDataSet(3,	1,	@"X:\",								@"X:\MyDir\MySubDir\MyFile2.abc",	/*@".\*/@"MyDir\MySubDir\MyFile2.abc",	@"..\.."					));
				yield return (new TestCaseDataSet(3,	2,	@"X:\MyFile1.abc",					@"X:\MyDir\MySubDir",				/*@".\*/@"MyDir\MySubDir",				@"..\..\MyFile1.abc"		));
				yield return (new TestCaseDataSet(3,	3,	@"X:\MyFile1.abc",					@"X:\MyDir\MySubDir\MyFile2.abc",	/*@".\*/@"MyDir\MySubDir\MyFile2.abc",	@"..\..\MyFile1.abc"		));

				// TS4: Local root far relation
				yield return (new TestCaseDataSet(4,	0,	@"X:\MyDir1",						@"X:\MyDir2",						@"..\MyDir2",						@"..\MyDir1"				));
				yield return (new TestCaseDataSet(4,	1,	@"X:\MyDir1",						@"X:\MyDir2\MyFile2.abc",			@"..\MyDir2\MyFile2.abc",			@"..\MyDir1"				));
				yield return (new TestCaseDataSet(4,	2,	@"X:\MyDir1\MyFile1.abc",			@"X:\MyDir2",						@"..\MyDir2",						@"..\MyDir1\MyFile1.abc"	));
				yield return (new TestCaseDataSet(4,	3,	@"X:\MyDir1\MyFile1.abc",			@"X:\MyDir2\MyFile2.abc",			@"..\MyDir2\MyFile2.abc",			@"..\MyDir1\MyFile1.abc"	));

				// ---- Network ----

				//								TS	TC	PATH A												PATH B												RESULT A							RESULT B

				// TS5: Network very-near relation
				yield return (new TestCaseDataSet(5,	0,	@"\\MyServer\MyShare\MyDir",						@"\\MyServer\MyShare\MyDir",						@".",								@"."						));
				yield return (new TestCaseDataSet(5,	1,	@"\\MyServer\MyShare\MyDir",						@"\\MyServer\MyShare\MyDir\MyFile2.abc",			@".\MyFile2.abc",					@"."						));
				yield return (new TestCaseDataSet(5,	2,	@"\\MyServer\MyShare\MyDir\MyFile1.abc",			@"\\MyServer\MyShare\MyDir",						@".",								@".\MyFile1.abc"			));
				yield return (new TestCaseDataSet(5,	3,	@"\\MyServer\MyShare\MyDir\MyFile1.abc",			@"\\MyServer\MyShare\MyDir\MyFile2.abc",			@".\MyFile2.abc",					@".\MyFile1.abc"			));

				// TS6: Network near relation
				yield return (new TestCaseDataSet(6,	0,	@"\\MyServer\MyShare\MyDir",						@"\\MyServer\MyShare\MyDir\MySubDir",				/*@".\*/@"MySubDir",						@".."						));
				yield return (new TestCaseDataSet(6,	1,	@"\\MyServer\MyShare\MyDir",						@"\\MyServer\MyShare\MyDir\MySubDir\MyFile2.abc",	/*@".\*/@"MySubDir\MyFile2.abc",			@".."						));
				yield return (new TestCaseDataSet(6,	2,	@"\\MyServer\MyShare\MyDir\MyFile1.abc",			@"\\MyServer\MyShare\MyDir\MySubDir",				/*@".\*/@"MySubDir",						@"..\MyFile1.abc"			));
				yield return (new TestCaseDataSet(6,	3,	@"\\MyServer\MyShare\MyDir\MyFile1.abc",			@"\\MyServer\MyShare\MyDir\MySubDir\MyFile2.abc",	/*@".\*/@"MySubDir\MyFile2.abc",			@"..\MyFile1.abc"			));

				// TS7: Network far relation
				yield return (new TestCaseDataSet(7,	0,	@"\\MyServer\MyShare\MyDir\MySubDir1",				@"\\MyServer\MyShare\MyDir\MySubDir2",				@"..\MySubDir2",					@"..\MySubDir1"				));
				yield return (new TestCaseDataSet(7,	1,	@"\\MyServer\MyShare\MyDir\MySubDir1",				@"\\MyServer\MyShare\MyDir\MySubDir2\MyFile2.abc",	@"..\MySubDir2\MyFile2.abc",		@"..\MySubDir1"				));
				yield return (new TestCaseDataSet(7,	2,	@"\\MyServer\MyShare\MyDir\MySubDir1\MyFile1.abc",	@"\\MyServer\MyShare\MyDir\MySubDir2",				@"..\MySubDir2",					@"..\MySubDir1\MyFile1.abc"	));
				yield return (new TestCaseDataSet(7,	3,	@"\\MyServer\MyShare\MyDir\MySubDir1\MyFile1.abc",	@"\\MyServer\MyShare\MyDir\MySubDir2\MyFile2.abc",	@"..\MySubDir2\MyFile2.abc",		@"..\MySubDir1\MyFile1.abc"	));

				// TS8: Network root relation
				yield return (new TestCaseDataSet(8,	0,	@"\\MyServer\MyShare",								@"\\MyServer\MyShare\MyDir\MySubDir",				/*@".\*/@"MyDir\MySubDir",				@"..\.."					));
				yield return (new TestCaseDataSet(8,	1,	@"\\MyServer\MyShare",								@"\\MyServer\MyShare\MyDir\MySubDir\MyFile2.abc",	/*@".\*/@"MyDir\MySubDir\MyFile2.abc",	@"..\.."					));
				yield return (new TestCaseDataSet(8,	2,	@"\\MyServer\MyShare\MyFile1.abc",					@"\\MyServer\MyShare\MyDir\MySubDir",				/*@".\*/@"MyDir\MySubDir",				@"..\..\MyFile1.abc"		));
				yield return (new TestCaseDataSet(8,	3,	@"\\MyServer\MyShare\MyFile1.abc",					@"\\MyServer\MyShare\MyDir\MySubDir\MyFile2.abc",	/*@".\*/@"MyDir\MySubDir\MyFile2.abc",	@"..\..\MyFile1.abc"		));

				// TS9: Network root far relation
				yield return (new TestCaseDataSet(9,	0,	@"\\MyServer\MyShare\MyDir1",						@"\\MyServer\MyShare\MyDir2",						@"..\MyDir2",						@"..\MyDir1"				));
				yield return (new TestCaseDataSet(9,	1,	@"\\MyServer\MyShare\MyDir1",						@"\\MyServer\MyShare\MyDir2\MyFile2.abc",			@"..\MyDir2\MyFile2.abc",			@"..\MyDir1"				));
				yield return (new TestCaseDataSet(9,	2,	@"\\MyServer\MyShare\MyDir1\MyFile1.abc",			@"\\MyServer\MyShare\MyDir2",						@"..\MyDir2",						@"..\MyDir1\MyFile1.abc"	));
				yield return (new TestCaseDataSet(9,	3,	@"\\MyServer\MyShare\MyDir1\MyFile1.abc",			@"\\MyServer\MyShare\MyDir2\MyFile2.abc",			@"..\MyDir2\MyFile2.abc",			@"..\MyDir1\MyFile1.abc"	));
			}
		}

		#endregion

		#region Test Cases Extension
		//==========================================================================================
		// Test Cases Extension
		//==========================================================================================

		/// <summary></summary>
		private static IEnumerable<TestCaseDataSet> TestCasesWithTrailingSeparator
		{
			get
			{
				foreach (TestCaseDataSet s in TestCasesBase)
				{
					// Create normal test case
					yield return (new TestCaseDataSet(s.TestSet, s.TestCase, s.PathA, s.PathB, s.ResultA, s.ResultB));

					// Create extended test case(s) that add a trailing '\' to the given input path(s).
					char dsc = System.IO.Path.DirectorySeparatorChar;
					switch (s.TestCase)
					{
						case 0:  yield return (new TestCaseDataSet(s.TestSet, s.TestCase, s.PathA + dsc, s.PathB,       s.ResultA, s.ResultB));
						         yield return (new TestCaseDataSet(s.TestSet, s.TestCase, s.PathA,       s.PathB + dsc, s.ResultA, s.ResultB));
						         yield return (new TestCaseDataSet(s.TestSet, s.TestCase, s.PathA + dsc, s.PathB + dsc, s.ResultA, s.ResultB)); break;

						case 1:  yield return (new TestCaseDataSet(s.TestSet, s.TestCase, s.PathA + dsc, s.PathB,       s.ResultA, s.ResultB)); break;

						case 2:  yield return (new TestCaseDataSet(s.TestSet, s.TestCase, s.PathA,       s.PathB + dsc, s.ResultA, s.ResultB)); break;

						default: break;
					}
				}
			}
		}

		/// <summary></summary>
		private static IEnumerable<TestCaseDataSet> TestCasesWithTrailingSeparatorAndAltDirectorySeparatorChar
		{
			get
			{
				foreach (TestCaseDataSet s in TestCasesWithTrailingSeparator)
				{
					// Using '\'.
					yield return (new TestCaseDataSet(s.TestSet, s.TestCase, s.PathA, s.PathB, s.ResultA, s.ResultB));

					// Using '/', in different combinations.
					string altA = s.PathA.Replace(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
					string altB = s.PathB.Replace(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
					yield return (new TestCaseDataSet(s.TestSet, s.TestCase, altA,    s.PathB, s.ResultA, s.ResultB));
					yield return (new TestCaseDataSet(s.TestSet, s.TestCase, s.PathA, altB,    s.ResultA, s.ResultB));
					yield return (new TestCaseDataSet(s.TestSet, s.TestCase, altA,    altB,    s.ResultA, s.ResultB));
				}
			}
		}

		#endregion

		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		public static IEnumerable TestCasesCompare
		{
			get
			{
				foreach (TestCaseDataSet s in TestCasesWithTrailingSeparatorAndAltDirectorySeparatorChar)
				{
					yield return (new TestCaseData(s.TestSet, s.TestCase, s.PathA, s.PathB, s.ResultA, s.ResultB));
				}
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCasesCombine
		{
			get
			{
				foreach (TestCaseDataSet s in TestCasesWithTrailingSeparatorAndAltDirectorySeparatorChar)
				{
					yield return (new TestCaseData(s.TestSet, s.TestCase, s.PathA, s.PathB, s.ResultA, s.ResultB));
				}
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
		[Test, TestCaseSource(typeof(PathExTestData), "TestCasesCompare")]
		public virtual void TestCompare(int testSet, int testCase, string pathA, string pathB, string expectedA, string expectedB)
		{
			// Test set is given as additional argument for potential special treatment of test cases.
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
		[Test, TestCaseSource(typeof(PathExTestData), "TestCasesCombine")]
		public virtual void TestCombine(int testSet, int testCase, string pathA, string pathB, string expectedA, string expectedB)
		{
			// Run normal test case.
			TestCombine_DoCallSetA(testSet, testCase, pathA, pathB, expectedA);
			TestCombine_DoCallSetB(testSet, testCase, pathA, pathB, expectedB);

			// \todo
			// Improve PathEx such that these tests pass.
			/*
			// Run extended test case(s) that remove the '.\' at the beginning of the given input path(s).
			switch (testSet)
			{
				case 0:
				case 5:
				{
					switch (testCase)
					{
						case 1:  TestCombine_RunExtended(testSet, testCase, pathA, pathB, expectedA.Remove(0, 2), expectedB             ); break;

						case 2:  TestCombine_RunExtended(testSet, testCase, pathA, pathB, expectedA,              expectedB.Remove(0, 2)); break;

						case 3:  TestCombine_RunExtended(testSet, testCase, pathA, pathB, expectedA.Remove(0, 2), expectedB             );
						         TestCombine_RunExtended(testSet, testCase, pathA, pathB, expectedA,              expectedB.Remove(0, 2));
						         TestCombine_RunExtended(testSet, testCase, pathA, pathB, expectedA.Remove(0, 2), expectedB.Remove(0, 2)); break;

						default: break;
					}
					break;
				}

				case 1:
				case 3:
				case 6:
				case 8:
				{
					TestCombine_RunExtended(testSet, testCase, pathA, pathB, expectedA.Remove(0, 2), expectedB);
					break;
				}

				default: break;
			}*/
		}

		private void TestCombine_RunExtended(int testSet, int testCase, string pathA, string pathB, string expectedA, string expectedB)
		{
			// Run extended test case(s) that add an additional '\' to the given input path(s).
			switch (testCase)
			{
				case 0:  TestCombine_DoCallSetA(testSet, testCase, pathA + "\\", pathB,        expectedA       );
				         TestCombine_DoCallSetA(testSet, testCase, pathA,        pathB,        expectedA + "\\");
				         TestCombine_DoCallSetA(testSet, testCase, pathA + "\\", pathB,        expectedA + "\\");
				         TestCombine_DoCallSetB(testSet, testCase, pathA,        pathB + "\\", expectedB + "\\");
				         TestCombine_DoCallSetB(testSet, testCase, pathA,        pathB,        expectedB + "\\");
				         TestCombine_DoCallSetB(testSet, testCase, pathA,        pathB + "\\", expectedB       ); break;

				case 1:  TestCombine_DoCallSetA(testSet, testCase, pathA + "\\", pathB,        expectedA       );
				         TestCombine_DoCallSetB(testSet, testCase, pathA,        pathB,        expectedB + "\\"); break;

				case 2:  TestCombine_DoCallSetA(testSet, testCase, pathA,        pathB,        expectedA + "\\");
				         TestCombine_DoCallSetB(testSet, testCase, pathA,        pathB + "\\", expectedB       ); break;

				default: break;
			}
		}

		private void TestCombine_DoCallSetA(int testSet, int testCase, string pathA, string pathB, string expectedA)
		{
			// Using '\'.
			TestCombine_DoCallsA(testSet, testCase, pathA, pathB, expectedA);

			// Using '/'.
			TestCombine_DoCallsA(testSet, testCase, pathA.Replace('\\', '/'), pathB, expectedA.Replace('\\', '/'));
		}

		private void TestCombine_DoCallSetB(int testSet, int testCase, string pathA, string pathB, string expectedB)
		{
			// Using '\'.
			TestCombine_DoCallsB(testSet, testCase, pathA, pathB, expectedB);

			// Using '/'.
			TestCombine_DoCallsB(testSet, testCase, pathA, pathB.Replace('\\', '/'), expectedB.Replace('\\', '/'));
		}

		private void TestCombine_DoCallsA(int testSet, int testCase, string pathA, string pathB, string expectedA)
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
		}

		private void TestCombine_DoCallsB(int testSet, int testCase, string pathA, string pathB, string expectedB)
		{
			// Test set is given as additional argument to ease searching for errornous test cases.
			UnusedArg.PreventAnalysisWarning(testSet);

			string result = "";

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
