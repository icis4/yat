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

        private readonly string[,] _TestPaths =
		{
			// very-near relation
			{	@"C:\MyDir",						@"C:\MyDir",						@".",							@".",							},
			{	@"C:\MyDir",						@"C:\MyDir\MyFile2.abc",			@".\MyFile2.abc",				@".",							},
			{	@"C:\MyDir\MyFile1.abc",			@"C:\MyDir",						@".",							@".\MyFile1.abc",				},
			{	@"C:\MyDir\MyFile1.abc",			@"C:\MyDir\MyFile2.abc",			@".\MyFile2.abc",				@".\MyFile1.abc",				},

			// near relation
			{	@"C:\MyDir",						@"C:\MyDir\MySubDir",				@"MySubDir",					@"..",							},
			{	@"C:\MyDir",						@"C:\MyDir\MySubDir\MyFile2.abc",	@"MySubDir\MyFile2.abc",		@"..",							},
			{	@"C:\MyDir\MyFile1.abc",			@"C:\MyDir\MySubDir",				@"MySubDir",					@"..\MyFile1.abc",				},
			{	@"C:\MyDir\MyFile1.abc",			@"C:\MyDir\MySubDir\MyFile2.abc",	@"MySubDir\MyFile2.abc",		@"..\MyFile1.abc",				},

			// far relation
			{	@"C:\MyDir\MySubDir1",				@"C:\MyDir\MySubDir2",				@"..\MySubDir2",				@"..\MySubDir1",					},
			{	@"C:\MyDir\MySubDir1",				@"C:\MyDir\MySubDir2\MyFile2.abc",	@"..\MySubDir2\MyFile2.abc",	@"..\MySubDir1",					},
			{	@"C:\MyDir\MySubDir1\MyFile1.abc",	@"C:\MyDir\MySubDir2",				@"..\MySubDir2",				@"..\MySubDir1\MyFile1.abc",		},
			{	@"C:\MyDir\MySubDir1\MyFile1.abc",	@"C:\MyDir\MySubDir2\MyFile2.abc",	@"..\MySubDir2\MyFile2.abc",	@"..\MySubDir1\MyFile1.abc",		},

			// root relation
			{	@"C:\",								@"C:\MyDir\MySubDir",				@"MyDir\MySubDir",				@"..\..",						},
			{	@"C:\",								@"C:\MyDir\MySubDir\MyFile2.abc",	@"MyDir\MySubDir\MyFile2.abc",	@"..\..",						},
			{	@"C:\MyFile1.abc",					@"C:\MyDir\MySubDir",				@"MyDir\MySubDir",				@"..\..\MyFile1.abc",			},
			{	@"C:\MyFile1.abc",					@"C:\MyDir\MySubDir\MyFile2.abc",	@"MyDir\MySubDir\MyFile2.abc",	@"..\..\MyFile1.abc",			},

			// root far relation
			{	@"C:\MyDir1",						@"C:\MyDir2",						@"..\MyDir2",					@"..\MyDir1",					},
			{	@"C:\MyDir1",						@"C:\MyDir2\MyFile2.abc",			@"..\MyDir2\MyFile2.abc",		@"..\MyDir1",					},
			{	@"C:\MyDir1\MyFile1.abc",			@"C:\MyDir2",						@"..\MyDir2",					@"..\MyDir1\MyFile1.abc",		},
			{	@"C:\MyDir1\MyFile1.abc",			@"C:\MyDir2\MyFile2.abc",			@"..\MyDir2\MyFile2.abc",		@"..\MyDir1\MyFile1.abc",		},
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
		public void TestCompare()
		{
			string path1 = "";
			string path2 = "";
			string expected = "";

			XPathCompareResult pcResult;

			for (int i = 0; i < _TestPaths.GetLength(0); i += 4)
			{
				// A compared to B results in A relative
				path1 = _TestPaths[i, 0];
				path2 = _TestPaths[i, 1];
				expected = _TestPaths[i, 2];
				pcResult.RelativePath = "";

				/*if (path1 == "C:\\" &&
					path2 == "C:\\MyDir\\MySubDir")
				{
					System.Diagnostics.Debugger.Break();
				}*/

				try
				{
					switch (i % 4)
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
					Console.WriteLine("A compared to B doesn't result in A relative:");
					Console.WriteLine("Input path1   = " + path1);
					Console.WriteLine("Input path2   = " + path2);
					Console.WriteLine("Output path   = " + pcResult.RelativePath);
					Console.WriteLine("Expected path = " + expected);
					throw (ex);
				}

				// B compared to A results in B relative
				path1 = _TestPaths[i, 1];
				path2 = _TestPaths[i, 0];
				expected = _TestPaths[i, 3];
				pcResult.RelativePath = "";

				/*if (path1 == "C:\\MyDir\\MySubDir" &&
					path2 == "C:\\MyDir")
				{
					System.Diagnostics.Debugger.Break();
				}*/

				try
				{
					switch (i % 4)
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
					Console.WriteLine("B compared to A doesn't result in B relative:");
					Console.WriteLine("Input path1   = " + path1);
					Console.WriteLine("Input path2   = " + path2);
					Console.WriteLine("Output path   = " + pcResult.RelativePath);
					Console.WriteLine("Expected path = " + expected);
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
		public void TestCombine()
		{
			string path1 = "";
			string path2 = "";
			string expected = "";

			string result = "";

			for (int i = 0; i < _TestPaths.GetLength(0); i++)
			{
				// A combined with A relative results in B
				path1 = _TestPaths[i, 0];
				path2 = _TestPaths[i, 2];
				expected = _TestPaths[i, 1];
				result = "";

				/*if (path1 == "C:\\MyDir" &&
					path2 == ".")
				{
					System.Diagnostics.Debugger.Break();
				}*/

				try
				{
					switch (i % 4)
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
					Console.WriteLine("A combined with A relative doesn't result in B:");
					Console.WriteLine("Input path1   = " + path1);
					Console.WriteLine("Input path2   = " + path2);
					Console.WriteLine("Output path   = " + result);
					Console.WriteLine("Expected path = " + expected);
					throw (ex);
				}

				// B combined with B relative results in A
				path1 = _TestPaths[i, 1];
				path2 = _TestPaths[i, 3];
				expected = _TestPaths[i, 0];
				result = "";

				/*if (path1 == "C:\\MyDir\\MySubDir" &&
					path2 == "..\\..\\MyFile1.abc")
				{
					System.Diagnostics.Debugger.Break();
				}*/

				try
				{
					switch (i % 4)
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
					Console.WriteLine("B combined with B relative doesn't result in A:");
					Console.WriteLine("Input path1   = " + path1);
					Console.WriteLine("Input path2   = " + path2);
					Console.WriteLine("Output path   = " + result);
					Console.WriteLine("Expected path = " + expected);
					throw (ex);
				}
			}
		}

		#endregion

		#endregion
	}
}
