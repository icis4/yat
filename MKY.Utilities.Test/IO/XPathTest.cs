//==================================================================================================
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

using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using MKY.Utilities.IO;

namespace MKY.Utilities.Test.IO
{
	[TestFixture]
	public class XPathTest
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const int TestCasesPerTestSet = 4;

		private readonly string[,] TestPaths =
		{
			// ---- Local ----

			//	PATH A								PATH B								RESULT A						RESULT B

			// TS0: Local very-near relation
			{	@"X:\MyDir",						@"X:\MyDir",						@".",							@".",							},
			{	@"X:\MyDir",						@"X:\MyDir\MyFile2.abc",			@".\MyFile2.abc",				@".",							},
			{	@"X:\MyDir\MyFile1.abc",			@"X:\MyDir",						@".",							@".\MyFile1.abc",				},
			{	@"X:\MyDir\MyFile1.abc",			@"X:\MyDir\MyFile2.abc",			@".\MyFile2.abc",				@".\MyFile1.abc",				},

			// TS1: Local near relation
			{	@"X:\MyDir",						@"X:\MyDir\MySubDir",				@"MySubDir",					@"..",							},
			{	@"X:\MyDir",						@"X:\MyDir\MySubDir\MyFile2.abc",	@"MySubDir\MyFile2.abc",		@"..",							},
			{	@"X:\MyDir\MyFile1.abc",			@"X:\MyDir\MySubDir",				@"MySubDir",					@"..\MyFile1.abc",				},
			{	@"X:\MyDir\MyFile1.abc",			@"X:\MyDir\MySubDir\MyFile2.abc",	@"MySubDir\MyFile2.abc",		@"..\MyFile1.abc",				},

			// TS2: Local far relation
			{	@"X:\MyDir\MySubDir1",				@"X:\MyDir\MySubDir2",				@"..\MySubDir2",				@"..\MySubDir1",				},
			{	@"X:\MyDir\MySubDir1",				@"X:\MyDir\MySubDir2\MyFile2.abc",	@"..\MySubDir2\MyFile2.abc",	@"..\MySubDir1",				},
			{	@"X:\MyDir\MySubDir1\MyFile1.abc",	@"X:\MyDir\MySubDir2",				@"..\MySubDir2",				@"..\MySubDir1\MyFile1.abc",	},
			{	@"X:\MyDir\MySubDir1\MyFile1.abc",	@"X:\MyDir\MySubDir2\MyFile2.abc",	@"..\MySubDir2\MyFile2.abc",	@"..\MySubDir1\MyFile1.abc",	},

			// TS3: Local root relation
			{	@"X:\",								@"X:\MyDir\MySubDir",				@"MyDir\MySubDir",				@"..\..",						},
			{	@"X:\",								@"X:\MyDir\MySubDir\MyFile2.abc",	@"MyDir\MySubDir\MyFile2.abc",	@"..\..",						},
			{	@"X:\MyFile1.abc",					@"X:\MyDir\MySubDir",				@"MyDir\MySubDir",				@"..\..\MyFile1.abc",			},
			{	@"X:\MyFile1.abc",					@"X:\MyDir\MySubDir\MyFile2.abc",	@"MyDir\MySubDir\MyFile2.abc",	@"..\..\MyFile1.abc",			},

			// TS4: Local root far relation
			{	@"X:\MyDir1",						@"X:\MyDir2",						@"..\MyDir2",					@"..\MyDir1",					},
			{	@"X:\MyDir1",						@"X:\MyDir2\MyFile2.abc",			@"..\MyDir2\MyFile2.abc",		@"..\MyDir1",					},
			{	@"X:\MyDir1\MyFile1.abc",			@"X:\MyDir2",						@"..\MyDir2",					@"..\MyDir1\MyFile1.abc",		},
			{	@"X:\MyDir1\MyFile1.abc",			@"X:\MyDir2\MyFile2.abc",			@"..\MyDir2\MyFile2.abc",		@"..\MyDir1\MyFile1.abc",		},


			// ---- Network ----

			//	PATH A												PATH B												RESULT A						RESULT B

			// TS5: Network very-near relation
			{	@"\\MyServer\MyShare\MyDir",						@"\\MyServer\MyShare\MyDir",						@".",							@".",							},
			{	@"\\MyServer\MyShare\MyDir",						@"\\MyServer\MyShare\MyDir\MyFile2.abc",			@".\MyFile2.abc",				@".",							},
			{	@"\\MyServer\MyShare\MyDir\MyFile1.abc",			@"\\MyServer\MyShare\MyDir",						@".",							@".\MyFile1.abc",				},
			{	@"\\MyServer\MyShare\MyDir\MyFile1.abc",			@"\\MyServer\MyShare\MyDir\MyFile2.abc",			@".\MyFile2.abc",				@".\MyFile1.abc",				},

			// TS6: Network near relation
			{	@"\\MyServer\MyShare\MyDir",						@"\\MyServer\MyShare\MyDir\MySubDir",				@"MySubDir",					@"..",							},
			{	@"\\MyServer\MyShare\MyDir",						@"\\MyServer\MyShare\MyDir\MySubDir\MyFile2.abc",	@"MySubDir\MyFile2.abc",		@"..",							},
			{	@"\\MyServer\MyShare\MyDir\MyFile1.abc",			@"\\MyServer\MyShare\MyDir\MySubDir",				@"MySubDir",					@"..\MyFile1.abc",				},
			{	@"\\MyServer\MyShare\MyDir\MyFile1.abc",			@"\\MyServer\MyShare\MyDir\MySubDir\MyFile2.abc",	@"MySubDir\MyFile2.abc",		@"..\MyFile1.abc",				},

			// TS7: Network far relation
			{	@"\\MyServer\MyShare\MyDir\MySubDir1",				@"\\MyServer\MyShare\MyDir\MySubDir2",				@"..\MySubDir2",				@"..\MySubDir1",				},
			{	@"\\MyServer\MyShare\MyDir\MySubDir1",				@"\\MyServer\MyShare\MyDir\MySubDir2\MyFile2.abc",	@"..\MySubDir2\MyFile2.abc",	@"..\MySubDir1",				},
			{	@"\\MyServer\MyShare\MyDir\MySubDir1\MyFile1.abc",	@"\\MyServer\MyShare\MyDir\MySubDir2",				@"..\MySubDir2",				@"..\MySubDir1\MyFile1.abc",	},
			{	@"\\MyServer\MyShare\MyDir\MySubDir1\MyFile1.abc",	@"\\MyServer\MyShare\MyDir\MySubDir2\MyFile2.abc",	@"..\MySubDir2\MyFile2.abc",	@"..\MySubDir1\MyFile1.abc",	},

			// TS8: Network root relation
			{	@"\\MyServer\MyShare",								@"\\MyServer\MyShare\MyDir\MySubDir",				@"MyDir\MySubDir",				@"..\..",						},
			{	@"\\MyServer\MyShare",								@"\\MyServer\MyShare\MyDir\MySubDir\MyFile2.abc",	@"MyDir\MySubDir\MyFile2.abc",	@"..\..",						},
			{	@"\\MyServer\MyShare\MyFile1.abc",					@"\\MyServer\MyShare\MyDir\MySubDir",				@"MyDir\MySubDir",				@"..\..\MyFile1.abc",			},
			{	@"\\MyServer\MyShare\MyFile1.abc",					@"\\MyServer\MyShare\MyDir\MySubDir\MyFile2.abc",	@"MyDir\MySubDir\MyFile2.abc",	@"..\..\MyFile1.abc",			},

			// TS9: Network root far relation
			{	@"\\MyServer\MyShare\MyDir1",						@"\\MyServer\MyShare\MyDir2",						@"..\MyDir2",					@"..\MyDir1",					},
			{	@"\\MyServer\MyShare\MyDir1",						@"\\MyServer\MyShare\MyDir2\MyFile2.abc",			@"..\MyDir2\MyFile2.abc",		@"..\MyDir1",					},
			{	@"\\MyServer\MyShare\MyDir1\MyFile1.abc",			@"\\MyServer\MyShare\MyDir2",						@"..\MyDir2",					@"..\MyDir1\MyFile1.abc",		},
			{	@"\\MyServer\MyShare\MyDir1\MyFile1.abc",			@"\\MyServer\MyShare\MyDir2\MyFile2.abc",			@"..\MyDir2\MyFile2.abc",		@"..\MyDir1\MyFile1.abc",		},
		};

		#endregion

		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		#region Tests > Compare()
		//------------------------------------------------------------------------------------------
		// Tests > Compare()
		//------------------------------------------------------------------------------------------

		[Test]
		public virtual void TestCompare()
		{
			string path1 = "";
			string path2 = "";
			string expected = "";
			XPathCompareResult pcResult;

			for (int i = 0; i < TestPaths.GetLength(0); i++)
			{
				int testSet  = i / TestCasesPerTestSet;
				int testCase = i % TestCasesPerTestSet;

				// A compared to B results in A relative
				path1    = TestPaths[i, 0];
				path2    = TestPaths[i, 1];
				expected = TestPaths[i, 2];
				pcResult.RelativePath = "";

				try
				{
					switch (testCase)
					{
						case 0:  pcResult = XPath.CompareDirectoryPaths       (path1, path2); break;
						case 1:  pcResult = XPath.CompareDirectoryAndFilePaths(path1, path2); break;
						case 2:  pcResult = XPath.CompareFileAndDirectoryPaths(path1, path2); break;
						default: pcResult = XPath.CompareFilePaths            (path1, path2); break;
					}
					Assert.AreEqual(expected, pcResult.RelativePath);
				}
				catch (Exception ex)
				{
					WriteResults("A compared to B doesn't result in A relative:",
								 testSet, testCase, path1, path2, pcResult.RelativePath, expected);
					throw (ex);
				}

				// B compared to A results in B relative
				path1    = TestPaths[i, 1];
				path2    = TestPaths[i, 0];
				expected = TestPaths[i, 3];
				pcResult.RelativePath = "";

				try
				{
					switch (testCase)
					{
						case 0:  pcResult = XPath.CompareDirectoryPaths       (path1, path2); break;
						case 1:  pcResult = XPath.CompareFileAndDirectoryPaths(path1, path2); break;
						case 2:  pcResult = XPath.CompareDirectoryAndFilePaths(path1, path2); break;
						default: pcResult = XPath.CompareFilePaths            (path1, path2); break;
					}
					Assert.AreEqual(expected, pcResult.RelativePath);
				}
				catch (Exception ex)
				{
					WriteResults("B compared to A doesn't result in B relative:",
								 testSet, testCase, path1, path2, pcResult.RelativePath, expected);
					throw (ex);
				}
			}
		}

		#endregion

		#region Tests > Combine()
		//------------------------------------------------------------------------------------------
		// Tests > Combine()
		//------------------------------------------------------------------------------------------

		[Test]
		public virtual void TestCombine()
		{
			string path1 = "";
			string path2 = "";
			string expected = "";
			string result = "";

			for (int i = 0; i < TestPaths.GetLength(0); i++)
			{
				int testSet  = i / TestCasesPerTestSet;
				int testCase = i % TestCasesPerTestSet;

				//if ((testSet == 6) && (testCase == 0))
				//	System.Diagnostics.Debugger.Break();

				// A combined with A relative results in B
				path1    = TestPaths[i, 0];
				path2    = TestPaths[i, 2];
				expected = TestPaths[i, 1];
				result   = "";

				try
				{
					switch (testCase)
					{
						case 0:  result = XPath.CombineDirectoryPaths       (path1, path2); break;
						case 1:  result = XPath.CombineDirectoryAndFilePaths(path1, path2); break;
						case 2:  result = XPath.CombineFileAndDirectoryPaths(path1, path2); break;
						default: result = XPath.CombineFilePaths            (path1, path2); break;
					}
					Assert.AreEqual(expected, result);
				}
				catch (Exception ex)
				{
					WriteResults("A combined with A relative doesn't result in B:",
								 testSet, testCase, path1, path2, result, expected);
					throw (ex);
				}

				// B combined with B relative results in A
				path1    = TestPaths[i, 1];
				path2    = TestPaths[i, 3];
				expected = TestPaths[i, 0];
				result = "";

				try
				{
					switch (testCase)
					{
						case 0:  result = XPath.CombineDirectoryPaths       (path1, path2); break;
						case 1:  result = XPath.CombineFileAndDirectoryPaths(path1, path2); break;
						case 2:  result = XPath.CombineDirectoryAndFilePaths(path1, path2); break;
						default: result = XPath.CombineFilePaths            (path1, path2); break;
					}
					Assert.AreEqual(expected, result);
				}
				catch (Exception ex)
				{
					WriteResults("B combined with B relative doesn't result in A:",
								 testSet, testCase, path1, path2, result, expected);
					throw (ex);
				}
			}
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void WriteResults(string title, int testSet, int testCase, string path1, string path2, string result, string expected)
		{
			Console.WriteLine(title);
			Console.WriteLine("Test set      = " + testSet);
			Console.WriteLine("Test case     = " + testCase);
			Console.WriteLine("Input path1   = " + path1);
			Console.WriteLine("Input path2   = " + path2);
			Console.WriteLine("Output path   = " + result);
			Console.WriteLine("Expected path = " + expected);
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
