using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using YAT.Domain;

namespace YAT.Domain.Test.Parser
{
	[TestFixture]
	public class ParserTest
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

		[Test]
		public void TestParser()
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
				Assert.Fail(ex.Message);
			}
		}
	}
}
