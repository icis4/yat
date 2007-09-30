#define TEST_IO
//#define TEST_TYPE

using System;
using System.IO;

using MKY.Utilities.IO;
using MKY.Utilities.Types;

namespace YAT.Test
{
	public class Test_Utilities
	{
		public static void Test()
		{
			#if (TEST_IO)


			int errorCounter = 0;

			string path1 = "";
			string path2 = "";
			string expected = "";

			XPathCompareResult pcResult;
			string result;

			// Compare()

			// Combine()

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
