#define TEST_IO
//#define TEST_TYPE

using System;
using System.IO;

using MKY.Utilities.IO;
using MKY.Utilities.Types;

namespace MKY.YAT.Test
{
	public class Test_Utilities
	{
		public static void Test()
		{
			#if (TEST_IO)

			string[,] testPaths =
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

			int errorCounter = 0;

			string path1 = "";
			string path2 = "";
			string expected = "";

			XPathCompareResult pcResult;
			string result;

			// Compare()
			for (int i = 0; i < testPaths.GetLength(0); i += 4)
			{
				// A compared to B results in A relative
				path1 = testPaths[i, 0];
				path2 = testPaths[i, 1];
				expected = testPaths[i, 2];

				switch (i % 4)
				{
					case 0:  pcResult = XPath.CompareDirectoryPaths(path1, path2);        break;
					case 1:  pcResult = XPath.CompareDirectoryAndFilePaths(path1, path2); break;
					case 2:  pcResult = XPath.CompareFileAndDirectoryPaths(path1, path2); break;
					default: pcResult = XPath.CompareFilePaths(path1, path2);             break;
				}

				Console.WriteLine("Input path1: " + path1);
				Console.WriteLine("Input path2: " + path2);
				Console.WriteLine("Output path: " + pcResult.RelativePath);
				if (pcResult.RelativePath != expected)
				{
					errorCounter++;
					Console.WriteLine("ERROR, expected: " + expected);
				}
				Console.WriteLine();

				// B compared to A results in B relative
				path1 = testPaths[i, 1];
				path2 = testPaths[i, 0];
				expected = testPaths[i, 3];

				switch (i % 4)
				{
					case 0:  pcResult = XPath.CompareDirectoryPaths(path1, path2);        break;
					case 1:  pcResult = XPath.CompareFileAndDirectoryPaths(path1, path2); break;
					case 2:  pcResult = XPath.CompareDirectoryAndFilePaths(path1, path2); break;
					default: pcResult = XPath.CompareFilePaths(path1, path2);             break;
				}

				Console.WriteLine("Input path1: " + path1);
				Console.WriteLine("Input path2: " + path2);
				Console.WriteLine("Output path: " + pcResult.RelativePath);
				if (pcResult.RelativePath != expected)
				{
					errorCounter++;
					Console.WriteLine("ERROR, expected: " + expected);
				}
				Console.WriteLine();
			}

			// Combine()
			for (int i = 0; i < testPaths.GetLength(0); i++)
			{
				// A combined with A relative results in B
				path1 = testPaths[i, 0];
				path2 = testPaths[i, 2];
				expected = testPaths[i, 1];

				switch (i % 4)
				{
					case 0:  result = XPath.CombineDirectoryPaths(path1, path2);        break;
					case 1:  result = XPath.CombineDirectoryAndFilePaths(path1, path2); break;
					case 2:  result = XPath.CombineFileAndDirectoryPaths(path1, path2); break;
					default: result = XPath.CombineFilePaths(path1, path2);             break;
				}

				Console.WriteLine("Input path1: " + path1);
				Console.WriteLine("Input path2: " + path2);
				Console.WriteLine("Output path: " + result);
				if (result != expected)
				{
					errorCounter++;
					Console.WriteLine("ERROR, expected: " + expected);
				}
				Console.WriteLine();

				// B combined with B relative results in A
				path1 = testPaths[i, 1];
				path2 = testPaths[i, 3];
				expected = testPaths[i, 0];

				switch (i % 4)
				{
					case 0:  result = XPath.CombineDirectoryPaths(path1, path2);        break;
					case 1:  result = XPath.CombineFileAndDirectoryPaths(path1, path2); break;
					case 2:  result = XPath.CombineDirectoryAndFilePaths(path1, path2); break;
					default: result = XPath.CombineFilePaths(path1, path2);             break;
				}

				Console.WriteLine("Input path1: " + path1);
				Console.WriteLine("Input path2: " + path2);
				Console.WriteLine("Output path: " + result);
				if (result != expected)
				{
					errorCounter++;
					Console.WriteLine("ERROR, expected: " + expected);
				}
				Console.WriteLine();
			}

			if (errorCounter == 0)
				Console.WriteLine("Successful, no errors");
			else
				Console.WriteLine("Failed, {0} errors", errorCounter);

			#endif

			#if (TEST_TYPE)
			byte data = 85;
			Console.WriteLine("Bin: " + XByte.ConvertToBinaryString(data) + "b");
			Console.WriteLine("Oct: " + XByte.ConvertToOctalString(data) + "o");
			Console.WriteLine("Dec: " + data.ToString("D3") + "d");
			Console.WriteLine("Hex: " + data.ToString("X2") + "h");
			#endif
		}
	}
}
