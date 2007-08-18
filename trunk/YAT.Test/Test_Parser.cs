using System;
using System.Collections.Generic;
using System.Text;

namespace MKY.YAT.Test
{
	public class Test_Parser
	{
		private static readonly string[] _TestStrings =
		{
			// mixed
			"Hello \\s(Hello \\d(10) Hello) Hello",

			// ascii
			"<CR><LF>",
			"<CR LF>",
			"<CR> <LF>",
			"Empty <>",

			// parenthesis and co.
			"Hello \\(round\\) and \\<angle\\> brackets",

			// char
			"Single char \\c(9)",
			"Single char \\c())",
			"Hello \\c(()round\\c()) brackets",
			"\\c(H)\\c(e)llo \\c(()round\\c()) brackets",
			"Empty \\c()",
		};

		public static void Test()
		{

			byte[] bytes;
			Domain.Parser.Parser parser = new Domain.Parser.Parser();

			try
			{
				foreach (string testString in _TestStrings)
				{
					bytes = parser.Parse(testString);

					Console.WriteLine();
					Console.WriteLine("Test String: \"" + testString + "\"");
					Console.WriteLine();

					Console.WriteLine("String:");
					foreach (byte b in bytes)
					{
						Console.Write((char)b);
					}
					Console.WriteLine();
					Console.WriteLine();

					Console.WriteLine("Bytes:");
					foreach (byte b in bytes)
					{
						Console.Write(b.ToString() + " ");
					}
					Console.WriteLine();
					Console.WriteLine();
				}
			}
			catch (Domain.Parser.FormatException ex)
			{
				Console.WriteLine(ex.Message);
			}
		}
	}
}
