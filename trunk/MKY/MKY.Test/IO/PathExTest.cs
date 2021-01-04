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

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using MKY.IO;

using NUnit.Framework;

namespace MKY.Test.IO
{
	/// <summary>
	/// \todo:
	/// Improve PathEx such that it passes tests for '\' as well as '/'.
	/// Ensure that test vectors potentially run on Windows and Unixoids.
	/// I.e. use of DirectorySeparatorChar must be checked.
	/// </summary>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Unixoids' is a proper technical term.")]
	public static class PathExTestData
	{
		#region Test Case Data Set
		//==========================================================================================
		// Test Case Data Set
		//==========================================================================================

		private class TestCaseDataSet
		{
			public int TestSet      { get; set; }
			public int TestCase     { get; set; }
			public string AbsoluteA { get; set; }
			public string AbsoluteB { get; set; }
			public string RelativeA { get; set; }
			public string RelativeB { get; set; }

			public TestCaseDataSet(int testSet, int testCase, string absoluteA, string absoluteB, string relativeA, string relativeB)
			{
				TestSet = testSet;
				TestCase = testCase;
				AbsoluteA = absoluteA;
				AbsoluteB = absoluteB;
				RelativeA = relativeA;
				RelativeB = relativeB;
			}
		}

		#endregion

		#region Test Cases Base
		//==========================================================================================
		// Test Cases Base
		//==========================================================================================

		/// <summary></summary>
		private static IEnumerable<TestCaseDataSet> TestCasesEmpty
		{
			get
			{
				// ---- Empty ----
				//									TS	TC	AbsoluteA							AbsoluteB							RelativeA							RelativeB

				// TS0: Empty relative path
				yield return (new TestCaseDataSet(0,	0,	@"X:\MyDir",						@"X:\MyDir",						@"",								@""							));
				yield return (new TestCaseDataSet(0,	1,	@"X:\MyDir",						@"X:\MyDir",						@"",								@""							));
				yield return (new TestCaseDataSet(0,	2,	@"X:\MyDir",						@"X:\MyDir",						@"",								@""							));
				yield return (new TestCaseDataSet(0,	3,	@"X:\MyDir\MyFile.abc",				@"X:\MyDir\MyFile.abc",				@"",								@""							));

				// TS1: Empty absolute path
				yield return (new TestCaseDataSet(1,	0,	@"",								@"",								@".",								@"."						));
				yield return (new TestCaseDataSet(1,	1,	@"",								@"",								@".\MyFile2.abc",					@"."						));
				yield return (new TestCaseDataSet(1,	2,	@"",								@"",								@".",								@".\MyFile1.abc"			));
				yield return (new TestCaseDataSet(1,	3,	@"",								@"",								@".\MyFile2.abc",					@".\MyFile1.abc"			));
			}
		}

		/// <summary></summary>
		private static IEnumerable<TestCaseDataSet> TestCasesBase
		{
			get
			{
				// ---- Local ----
				//									TS	TC	AbsoluteA							AbsoluteB							RelativeA	\todo: See below		RelativeB

				// TS0: Local very-near relation
				yield return (new TestCaseDataSet(0,	0,	@"X:\MyDir",						@"X:\MyDir",						@".",								@"."						));
				yield return (new TestCaseDataSet(0,	1,	@"X:\MyDir",						@"X:\MyDir\MyFile2.abc",			@".\MyFile2.abc",					@"."						));
				yield return (new TestCaseDataSet(0,	2,	@"X:\MyDir\MyFile1.abc",			@"X:\MyDir",						@".",								@".\MyFile1.abc"			));
				yield return (new TestCaseDataSet(0,	3,	@"X:\MyDir\MyFile1.abc",			@"X:\MyDir\MyFile2.abc",			@".\MyFile2.abc",					@".\MyFile1.abc"			));

				// TS1: Local near relation
				yield return (new TestCaseDataSet(1,	0,	@"X:\MyDir",						@"X:\MyDir\MySubDir",				/*@".\*/@"MySubDir",				@".."						));
				yield return (new TestCaseDataSet(1,	1,	@"X:\MyDir",						@"X:\MyDir\MySubDir\MyFile2.abc",	/*@".\*/@"MySubDir\MyFile2.abc",	@".."						));
				yield return (new TestCaseDataSet(1,	2,	@"X:\MyDir\MyFile1.abc",			@"X:\MyDir\MySubDir",				/*@".\*/@"MySubDir",				@"..\MyFile1.abc"			));
				yield return (new TestCaseDataSet(1,	3,	@"X:\MyDir\MyFile1.abc",			@"X:\MyDir\MySubDir\MyFile2.abc",	/*@".\*/@"MySubDir\MyFile2.abc",	@"..\MyFile1.abc"			));

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
				//									TS	TC	AbsoluteA											AbsoluteB											RelativeA	\todo: See below		RelativeB

				// TS5: Network very-near relation
				yield return (new TestCaseDataSet(5,	0,	@"\\MyServer\MyShare\MyDir",						@"\\MyServer\MyShare\MyDir",						@".",								@"."						));
				yield return (new TestCaseDataSet(5,	1,	@"\\MyServer\MyShare\MyDir",						@"\\MyServer\MyShare\MyDir\MyFile2.abc",			@".\MyFile2.abc",					@"."						));
				yield return (new TestCaseDataSet(5,	2,	@"\\MyServer\MyShare\MyDir\MyFile1.abc",			@"\\MyServer\MyShare\MyDir",						@".",								@".\MyFile1.abc"			));
				yield return (new TestCaseDataSet(5,	3,	@"\\MyServer\MyShare\MyDir\MyFile1.abc",			@"\\MyServer\MyShare\MyDir\MyFile2.abc",			@".\MyFile2.abc",					@".\MyFile1.abc"			));

				// TS6: Network near relation
				yield return (new TestCaseDataSet(6,	0,	@"\\MyServer\MyShare\MyDir",						@"\\MyServer\MyShare\MyDir\MySubDir",				/*@".\*/@"MySubDir",				@".."						));
				yield return (new TestCaseDataSet(6,	1,	@"\\MyServer\MyShare\MyDir",						@"\\MyServer\MyShare\MyDir\MySubDir\MyFile2.abc",	/*@".\*/@"MySubDir\MyFile2.abc",	@".."						));
				yield return (new TestCaseDataSet(6,	2,	@"\\MyServer\MyShare\MyDir\MyFile1.abc",			@"\\MyServer\MyShare\MyDir\MySubDir",				/*@".\*/@"MySubDir",				@"..\MyFile1.abc"			));
				yield return (new TestCaseDataSet(6,	3,	@"\\MyServer\MyShare\MyDir\MyFile1.abc",			@"\\MyServer\MyShare\MyDir\MySubDir\MyFile2.abc",	/*@".\*/@"MySubDir\MyFile2.abc",	@"..\MyFile1.abc"			));

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

		#region Test Cases Extended With Trailing Separator
		//==========================================================================================
		// Test Cases Extended With Trailing Separator
		//==========================================================================================

			// \todo:
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
						case 1:  TestCombine_RunExtended(testSet, testCase, absoluteA, absoluteB, relativeA.Remove(0, 2), relativeB             ); break;

						case 2:  TestCombine_RunExtended(testSet, testCase, absoluteA, absoluteB, relativeA,              relativeB.Remove(0, 2)); break;

						case 3:  TestCombine_RunExtended(testSet, testCase, absoluteA, absoluteB, relativeA.Remove(0, 2), relativeB             );
						         TestCombine_RunExtended(testSet, testCase, absoluteA, absoluteB, relativeA,              relativeB.Remove(0, 2));
						         TestCombine_RunExtended(testSet, testCase, absoluteA, absoluteB, relativeA.Remove(0, 2), relativeB.Remove(0, 2)); break;

						default: break;
					}
					break;
				}

				case 1:
				case 3:
				case 6:
				case 8:
				{
					TestCombine_RunExtended(testSet, testCase, absoluteA, absoluteB, relativeA.Remove(0, 2), relativeB);
					break;
				}

				default: break;
			}*/

		/// <summary>
		/// AbsoluteA combined with RelativeA results in AbsoluteB.
		/// </summary>
		private static IEnumerable<TestCaseDataSet> TestCasesCombineAWithTrailingSeparator
		{
			get
			{
				foreach (TestCaseDataSet s in TestCasesBase)
				{
					// Create normal test case.
					yield return (new TestCaseDataSet(s.TestSet, s.TestCase, s.AbsoluteA, s.AbsoluteB, s.RelativeA, null));

					// Create extended test case(s) that add a trailing '\' to the given input path(s).
					char dsc = System.IO.Path.DirectorySeparatorChar;
					switch (s.TestCase)
					{
						case 0:  yield return (new TestCaseDataSet(s.TestSet, s.TestCase, s.AbsoluteA + dsc, s.AbsoluteB, s.RelativeA,       null));
						         yield return (new TestCaseDataSet(s.TestSet, s.TestCase, s.AbsoluteA,       s.AbsoluteB, s.RelativeA + dsc, null));
						         yield return (new TestCaseDataSet(s.TestSet, s.TestCase, s.AbsoluteA + dsc, s.AbsoluteB, s.RelativeA + dsc, null)); break;

						case 1:  yield return (new TestCaseDataSet(s.TestSet, s.TestCase, s.AbsoluteA + dsc, s.AbsoluteB, s.RelativeA,       null)); break;

						case 2:  yield return (new TestCaseDataSet(s.TestSet, s.TestCase, s.AbsoluteA,       s.AbsoluteB, s.RelativeA + dsc, null)); break;

						default: break;
					}
				}
			}
		}

		/// <summary>
		/// AbsoluteB combined with RelativeB results in AbsoluteA.
		/// </summary>
		private static IEnumerable<TestCaseDataSet> TestCasesCombineBWithTrailingSeparator
		{
			get
			{
				foreach (TestCaseDataSet s in TestCasesBase)
				{
					// Create normal test case.
					yield return (new TestCaseDataSet(s.TestSet, s.TestCase, s.AbsoluteA, s.AbsoluteB, null, s.RelativeB));

					// Create extended test case(s) that add a trailing '\' to the given input path(s).
					char dsc = System.IO.Path.DirectorySeparatorChar;
					switch (s.TestCase)
					{
						case 0:  yield return (new TestCaseDataSet(s.TestSet, s.TestCase, s.AbsoluteA, s.AbsoluteB + dsc, null, s.RelativeB + dsc));
						         yield return (new TestCaseDataSet(s.TestSet, s.TestCase, s.AbsoluteA, s.AbsoluteB,       null, s.RelativeB + dsc));
						         yield return (new TestCaseDataSet(s.TestSet, s.TestCase, s.AbsoluteA, s.AbsoluteB + dsc, null, s.RelativeB      )); break;

						case 1:  yield return (new TestCaseDataSet(s.TestSet, s.TestCase, s.AbsoluteA, s.AbsoluteB,       null, s.RelativeB + dsc)); break;

						case 2:  yield return (new TestCaseDataSet(s.TestSet, s.TestCase, s.AbsoluteA, s.AbsoluteB + dsc, null, s.RelativeB      )); break;

						default: break;
					}
				}
			}
		}

		/// <summary>
		/// AbsoluteA combined with AbsoluteB results in RelativeA.
		/// AbsoluteB combined with AbsoluteA results in RelativeB.
		/// </summary>
		private static IEnumerable<TestCaseDataSet> TestCasesCompareWithTrailingSeparator
		{
			get
			{
				foreach (TestCaseDataSet s in TestCasesBase)
				{
					// Create normal test case.
					yield return (new TestCaseDataSet(s.TestSet, s.TestCase, s.AbsoluteA, s.AbsoluteB, s.RelativeA, s.RelativeB));

					// Create extended test case(s) that add a trailing '\' to the given input path(s).
					char dsc = System.IO.Path.DirectorySeparatorChar;
					switch (s.TestCase)
					{
						case 0:  yield return (new TestCaseDataSet(s.TestSet, s.TestCase, s.AbsoluteA + dsc, s.AbsoluteB,       s.RelativeA, s.RelativeB));
						         yield return (new TestCaseDataSet(s.TestSet, s.TestCase, s.AbsoluteA,       s.AbsoluteB + dsc, s.RelativeA, s.RelativeB));
						         yield return (new TestCaseDataSet(s.TestSet, s.TestCase, s.AbsoluteA + dsc, s.AbsoluteB + dsc, s.RelativeA, s.RelativeB)); break;

						case 1:  yield return (new TestCaseDataSet(s.TestSet, s.TestCase, s.AbsoluteA + dsc, s.AbsoluteB,       s.RelativeA, s.RelativeB)); break;

						case 2:  yield return (new TestCaseDataSet(s.TestSet, s.TestCase, s.AbsoluteA,       s.AbsoluteB + dsc, s.RelativeA, s.RelativeB)); break;

						default: break;
					}
				}
			}
		}

		#endregion

		#region Test Cases Extended With Trailing Separator And Alternative Separator
		//==========================================================================================
		// Test Cases Extended With Trailing Separator And Alternative Separator
		//==========================================================================================

		/// <summary>
		/// AbsoluteA combined with RelativeA results in AbsoluteB.
		/// </summary>
		private static IEnumerable<TestCaseDataSet> TestCasesCombineAWithTrailingSeparatorAndAlternativeSeparator
		{
			get
			{
				foreach (TestCaseDataSet s in TestCasesCombineAWithTrailingSeparator)
				{
					// Using '\'.
					yield return (new TestCaseDataSet(s.TestSet, s.TestCase, s.AbsoluteA, s.AbsoluteB, s.RelativeA, null));

					// Using '/', in different combinations.
					string replaceAA = s.AbsoluteA.Replace(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
					string replaceRA = s.RelativeA.Replace(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
					yield return (new TestCaseDataSet(s.TestSet, s.TestCase, replaceAA,   s.AbsoluteB, s.RelativeA, null));
					yield return (new TestCaseDataSet(s.TestSet, s.TestCase, s.AbsoluteA, s.AbsoluteB, replaceRA,   null));
					yield return (new TestCaseDataSet(s.TestSet, s.TestCase, replaceAA,   s.AbsoluteB, replaceRA,   null));
				}
			}
		}

		/// <summary>
		/// AbsoluteB combined with RelativeB results in AbsoluteA.
		/// </summary>
		private static IEnumerable<TestCaseDataSet> TestCasesCombineBWithTrailingSeparatorAndAlternativeSeparator
		{
			get
			{
				foreach (TestCaseDataSet s in TestCasesCombineBWithTrailingSeparator)
				{
					// Using '\'.
					yield return (new TestCaseDataSet(s.TestSet, s.TestCase, s.AbsoluteA, s.AbsoluteB, null, s.RelativeB));

					// Using '/', in different combinations.
					string replaceAB = s.AbsoluteB.Replace(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
					string replaceRB = s.RelativeB.Replace(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
					yield return (new TestCaseDataSet(s.TestSet, s.TestCase, s.AbsoluteA, replaceAB,   null, s.RelativeB));
					yield return (new TestCaseDataSet(s.TestSet, s.TestCase, s.AbsoluteA, s.AbsoluteB, null, replaceRB  ));
					yield return (new TestCaseDataSet(s.TestSet, s.TestCase, s.AbsoluteA, replaceAB,   null, replaceRB  ));
				}
			}
		}

		/// <summary>
		/// AbsoluteA combined with AbsoluteB results in RelativeA.
		/// AbsoluteB combined with AbsoluteA results in RelativeB.
		/// </summary>
		private static IEnumerable<TestCaseDataSet> TestCasesCompareWithTrailingSeparatorAndAlternativeSeparator
		{
			get
			{
				foreach (TestCaseDataSet s in TestCasesCompareWithTrailingSeparator)
				{
					// Using '\'.
					yield return (new TestCaseDataSet(s.TestSet, s.TestCase, s.AbsoluteA, s.AbsoluteB, s.RelativeA, s.RelativeB));

					// Using '/', in different combinations.
					string replaceA = s.AbsoluteA.Replace(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
					string replaceB = s.AbsoluteB.Replace(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
					yield return (new TestCaseDataSet(s.TestSet, s.TestCase, replaceA,    s.AbsoluteB, s.RelativeA, s.RelativeB));
					yield return (new TestCaseDataSet(s.TestSet, s.TestCase, s.AbsoluteA, replaceB,    s.RelativeA, s.RelativeB));
					yield return (new TestCaseDataSet(s.TestSet, s.TestCase, replaceA,    replaceB,    s.RelativeA, s.RelativeB));
				}
			}
		}

		#endregion

		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary>
		/// AbsoluteA combined with RelativeA results in AbsoluteB.
		/// </summary>
		public static IEnumerable TestCasesCombineA
		{
			get
			{
				foreach (TestCaseDataSet s in TestCasesCombineAWithTrailingSeparatorAndAlternativeSeparator)
				{
					yield return (new TestCaseData(s.TestSet, s.TestCase, s.AbsoluteA, s.AbsoluteB, s.RelativeA, null));
				}
			}
		}

		/// <summary>
		/// AbsoluteA combined with RelativeA results in AbsoluteB.
		/// </summary>
		public static IEnumerable TestCasesCombineAEmpty
		{
			get
			{
				foreach (TestCaseDataSet s in TestCasesEmpty)
				{
					yield return (new TestCaseData(s.TestSet, s.TestCase, s.AbsoluteA, s.AbsoluteB, s.RelativeA, null));
				}
			}
		}

		/// <summary>
		/// AbsoluteB combined with RelativeB results in AbsoluteA.
		/// </summary>
		public static IEnumerable TestCasesCombineB
		{
			get
			{
				foreach (TestCaseDataSet s in TestCasesCombineBWithTrailingSeparatorAndAlternativeSeparator)
				{
					yield return (new TestCaseData(s.TestSet, s.TestCase, s.AbsoluteA, s.AbsoluteB, null, s.RelativeB));
				}
			}
		}

		/// <summary>
		/// AbsoluteB combined with RelativeB results in AbsoluteA.
		/// </summary>
		public static IEnumerable TestCasesCombineBEmpty
		{
			get
			{
				foreach (TestCaseDataSet s in TestCasesEmpty)
				{
					yield return (new TestCaseData(s.TestSet, s.TestCase, s.AbsoluteA, s.AbsoluteB, null, s.RelativeB));
				}
			}
		}

		/// <summary>
		/// AbsoluteA combined with AbsoluteB results in RelativeA.
		/// AbsoluteB combined with AbsoluteA results in RelativeB.
		/// </summary>
		public static IEnumerable TestCasesCompare
		{
			get
			{
				foreach (TestCaseDataSet s in TestCasesCompareWithTrailingSeparatorAndAlternativeSeparator)
				{
					yield return (new TestCaseData(s.TestSet, s.TestCase, s.AbsoluteA, s.AbsoluteB, s.RelativeA, s.RelativeB));
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

		#region Tests > Append...()
		//------------------------------------------------------------------------------------------
		// Tests > Append...()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestAppendPostfixToFileName()
		{
			string postfix             =                 "_1";
			string filePath            = @"X:\MyDir\MyFile.abc";
			string filePathWithPostfix = @"X:\MyDir\MyFile_1.abc";

			filePath = PathEx.AppendPostfixToFileName(filePath, postfix);

			Assert.That(filePath, Is.EqualTo(filePathWithPostfix), "AppendPostfixToFileName() has failed");
		}

		#endregion

		#region Tests > Combine...()
		//------------------------------------------------------------------------------------------
		// Tests > Combine...()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, TestCaseSource(typeof(PathExTestData), "TestCasesCombineA")]
		public virtual void TestCombineA(int testSet, int testCase, string absoluteA, string absoluteB, string relativeA, string relativeB)
		{
			UnusedArg.PreventAnalysisWarning(testSet, "Argument is given to ease searching for erroneous test cases.");

			string result = "";

			// A combined with A relative results in B.
			switch (testCase)
			{
				case 0:  result = PathEx.CombineDirectoryPaths       (absoluteA, relativeA); break;
				case 1:  result = PathEx.CombineDirectoryAndFilePaths(absoluteA, relativeA); break; // DIR vs. FILE.
				case 2:  result = PathEx.CombineFileAndDirectoryPaths(absoluteA, relativeA); break; // FILE vs. DIR.
				default: result = PathEx.CombineFilePaths            (absoluteA, relativeA); break;
			}
			Assert.That(result, Is.EqualTo(absoluteB), "A absolute combined with A relative doesn't result in B absolute");
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(PathExTestData), "TestCasesCombineAEmpty")]
		public virtual void TestCombineAEmpty(int testSet, int testCase, string absoluteA, string absoluteB, string relativeA, string relativeB)
		{
			// Same test as above, but using a different test source, and performing it with both "" and null.
			if (string.IsNullOrEmpty(absoluteA) && string.IsNullOrEmpty(absoluteB))
			{
				TestCombineA(testSet, testCase,   "", null, relativeA, relativeB);
				TestCombineA(testSet, testCase, null, null, relativeA, relativeB);
			}
			else
			{
				TestCombineA(testSet, testCase, absoluteA, absoluteB, relativeA, relativeB);
			}
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(PathExTestData), "TestCasesCombineB")]
		public virtual void TestCombineB(int testSet, int testCase, string absoluteA, string absoluteB, string relativeA, string relativeB)
		{
			UnusedArg.PreventAnalysisWarning(testSet, "Argument is given to ease searching for erroneous test cases.");

			string result = "";

			// B combined with B relative results in A.
			switch (testCase)
			{
				case 0:  result = PathEx.CombineDirectoryPaths       (absoluteB, relativeB); break;
				case 1:  result = PathEx.CombineFileAndDirectoryPaths(absoluteB, relativeB); break; // FILE vs. DIR.
				case 2:  result = PathEx.CombineDirectoryAndFilePaths(absoluteB, relativeB); break; // DIR vs. FILE.
				default: result = PathEx.CombineFilePaths            (absoluteB, relativeB); break;
			}
			Assert.That(result, Is.EqualTo(absoluteA), "B absolute combined with B relative doesn't result in A absolute");
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(PathExTestData), "TestCasesCombineBEmpty")]
		public virtual void TestCombineBEmpty(int testSet, int testCase, string absoluteA, string absoluteB, string relativeA, string relativeB)
		{
			// Same test as above, but using a different test source, and performing it with both "" and null.
			if (string.IsNullOrEmpty(absoluteA) && string.IsNullOrEmpty(absoluteB))
			{
				TestCombineB(testSet, testCase, null,   "", relativeA, relativeB);
				TestCombineB(testSet, testCase, null, null, relativeA, relativeB);
			}
			else
			{
				TestCombineB(testSet, testCase, absoluteA, absoluteB, relativeA, relativeB);
			}
		}

		#endregion

		#region Tests > Compare...()
		//------------------------------------------------------------------------------------------
		// Tests > Compare...()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, TestCaseSource(typeof(PathExTestData), "TestCasesCompare")]
		public virtual void TestCompare(int testSet, int testCase, string absoluteA, string absoluteB, string relativeA, string relativeB)
		{
			UnusedArg.PreventAnalysisWarning(testSet, "Argument is given to ease searching for erroneous test cases.");

			PathCompareResult result;

			// A compared to B results in A relative.
			switch (testCase)
			{
				case 0:  result = PathEx.CompareDirectoryPaths       (absoluteA, absoluteB); break;
				case 1:  result = PathEx.CompareDirectoryAndFilePaths(absoluteA, absoluteB); break; // DIR vs. FILE.
				case 2:  result = PathEx.CompareFileAndDirectoryPaths(absoluteA, absoluteB); break; // FILE vs. DIR.
				default: result = PathEx.CompareFilePaths            (absoluteA, absoluteB); break;
			}
			Assert.That(result.RelativePath, Is.EqualTo(relativeA), "A absolute compared to B absolute doesn't result in A relative");

			// B compared to A results in B relative.
			switch (testCase)
			{
				case 0:  result = PathEx.CompareDirectoryPaths       (absoluteB, absoluteA); break;
				case 1:  result = PathEx.CompareFileAndDirectoryPaths(absoluteB, absoluteA); break; // FILE vs. DIR.
				case 2:  result = PathEx.CompareDirectoryAndFilePaths(absoluteB, absoluteA); break; // DIR vs. FILE.
				default: result = PathEx.CompareFilePaths            (absoluteB, absoluteA); break;
			}
			Assert.That(result.RelativePath, Is.EqualTo(relativeB), "B absolute compared to A absolute doesn't result in B relative");
		}

		#endregion

		#region Tests > Distinct...()
		//------------------------------------------------------------------------------------------
		// Tests > Distinct...()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestDistinct()
		{
			string[] singleDistinctPath =
			{
				@"C:\Test",
				@"C:\test",
				null,
				"",
				@"c:\Test",
				@"c:\test"
			};

			string[] fourDistinctPaths =
			{
				@"C:\ABC",
				@"C:\ABCD",
				null,
				"",
				@"c:\ABCDe",
				@"c:\abcdef"
			};

			string[] nullAndEmpty =
			{
				null,
				""
			};

			var distinctPaths = PathEx.Distinct(singleDistinctPath, fourDistinctPaths, nullAndEmpty);
			Assert.That(distinctPaths.Count(), Is.EqualTo(5));
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
