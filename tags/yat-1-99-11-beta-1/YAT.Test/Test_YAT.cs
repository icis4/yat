//#define TEST_PARSER
//#define TEST_SUBSTITUTION_PARSER
//#define TEST_RAW_TERMINAL
//#define TEST_SETTINGS
//#define TEST_FORMAT
#define TEST_XML

using System;

namespace HSR.YAT.Test
{
	public class Test_YAT
	{
		public Test_YAT()
		{
			Console.WriteLine();
			Console.WriteLine("** YAT TEST BEGIN *********************");
			Console.WriteLine();

			#if (TEST_PARSER)
			Console.WriteLine("-- Parser Test Begin ---------------------");
			Test_Parser.Test();
			Console.WriteLine("-- Parser Test End -----------------------");
			#endif

			#if (TEST_SUBSTITUTION_PARSER)
			Console.WriteLine("-- Substitution Parser Test Begin --------");
			Test_SubstitutionParser.Test();
			Console.WriteLine("-- Substitution Parser Test End ----------");
			#endif

			#if (TEST_RAW_TERMINAL)
			Console.WriteLine("-- RawTerminal Test Begin ----------------");
			Test_RawTerminal.Test();
			Console.WriteLine("-- RawTerminal Test End ------------------");
			#endif

			#if (TEST_SETTINGS)
			Console.WriteLine("-- Settings Test Begin -------------------");
			Test_Settings.Test();
			Console.WriteLine("-- Settings Test End ---------------------");
			#endif

			#if (TEST_FORMAT)
			Console.WriteLine("-- Format Test Begin ---------------------");
			Test_Format.Test();
			Console.WriteLine("-- Format Test End ----------------------");
			#endif

			#if (TEST_XML)
			Console.WriteLine("-- XML Test Begin ---------------------");
			Test_Xml.Test();
			Console.WriteLine("-- XML Test End ----------------------");
			#endif

			Console.WriteLine();
			Console.WriteLine("** YAT TEST END ***********************");
			Console.WriteLine();
			Console.Out.Flush();
		}
	}
}