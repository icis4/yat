using System;

using MKY.Utilities.Types;

namespace MKY.YAT.Test
{
	public class Test_Utilities
	{
		public static void Test()
		{
			byte data = 85;
			Console.WriteLine("Bin: " + XByte.ConvertToBinaryString(data) + "b");
			Console.WriteLine("Oct: " + XByte.ConvertToOctalString(data) + "o");
			Console.WriteLine("Dec: " + data.ToString("D3") + "d");
			Console.WriteLine("Hex: " + data.ToString("X2") + "h");
		}
	}
}
