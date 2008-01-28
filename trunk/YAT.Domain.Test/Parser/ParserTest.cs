using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using MKY.Utilities.Diagnostics;

using YAT.Domain;

namespace YAT.Domain.Test.Parser
{
	[TestFixture]
	public class ParserTest
	{
        #region Types
        //==========================================================================================
        // Types
        //==========================================================================================

        private struct TestSet
        {
            public readonly Endianess Endianess;
			public readonly Encoding Encoding;
			public readonly Radix DefaultRadix;
			public readonly string InputString;
			public readonly byte[] OutputBytes;

            public TestSet(string inputString, byte[] outputBytes)
            {
                Endianess = Endianess.LittleEndian;
                Encoding = Encoding.Default;
                DefaultRadix = Radix.String;
                InputString = inputString;
                OutputBytes = outputBytes;
            }

            public TestSet(Endianess endianess, string inputString, byte[] outputBytes)
            {
                Endianess = endianess;
                Encoding = Encoding.Default;
                DefaultRadix = Radix.String;
                InputString = inputString;
                OutputBytes = outputBytes;
            }

            public TestSet(Encoding encoding, string inputString, byte[] outputBytes)
            {
                Endianess = Endianess.LittleEndian;
                Encoding = encoding;
                DefaultRadix = Radix.String;
                InputString = inputString;
                OutputBytes = outputBytes;
            }

            public TestSet(Radix defaultRadix, string inputString, byte[] outputBytes)
            {
                Endianess = Endianess.LittleEndian;
                Encoding = Encoding.Default;
                DefaultRadix = defaultRadix;
                InputString = inputString;
                OutputBytes = outputBytes;
            }
        };

        #endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private TestSet[][] _testSets =
		{
			// mixed
			new TestSet[]
			{
				new TestSet("Hello \\s(Hello \\d(10) Hello) Hello",	new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x0A, 0x20, 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x48, 0x65, 0x6C, 0x6C, 0x6F } ),
			},

			// empty
			new TestSet[]
			{
				new TestSet("",				new byte[] { } ),
			},

			// ascii
			new TestSet[]
			{
				new TestSet("<CR>",			new byte[] { 0x0D } ),
				new TestSet("<LF>",			new byte[] { 0x0A } ),
				new TestSet("<CR><LF>",		new byte[] { 0x0D, 0x0A } ),
				new TestSet("<CR LF>",		new byte[] { 0x0D, 0x0A } ),
				new TestSet("<CR> <LF>",	new byte[] { 0x0D, 0x20, 0x0A } ),
				new TestSet("Empty <>",		new byte[] { 0x45, 0x6D, 0x70, 0x74, 0x79, 0x20 } ),
			},

			// parenthesis and co.
			new TestSet[]
			{
				new TestSet("Hello \\(round\\) and \\<angle\\> brackets",	new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x72, 0x6F, 0x75, 0x6E, 0x64, 0x20, 0x61, 0x6E, 0x64, 0x20, 0x61, 0x6E, 0x67, 0x6C, 0x65, 0x20, 0x62, 0x72, 0x61, 0x63, 0x6B, 0x65, 0x74, 0x73 } ),
			},

			// char
			new TestSet[]
			{
				new TestSet("Single char \\c(9)",							new byte[] { 0x53, 0x69, 0x6E, 0x67, 0x6C, 0x65, 0x20, 0x63, 0x68, 0x61, 0x72, 0x20, 0x39 } ),
				new TestSet("Single char \\c(.)",							new byte[] { 0x53, 0x69, 0x6E, 0x67, 0x6C, 0x65, 0x20, 0x63, 0x68, 0x61, 0x72, 0x20, 0x2E } ),
				new TestSet("Hello \\c(()round\\c()) brackets",				new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x28, 0x72, 0x6F, 0x75, 0x6E, 0x64, 0x29, 0x20, 0x62, 0x72, 0x61, 0x63, 0x6B, 0x65, 0x74, 0x73 } ),
				new TestSet("\\c(H)\\c(e)llo \\c(()round\\c()) brackets",	new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x28, 0x72, 0x6F, 0x75, 0x6E, 0x64, 0x29, 0x20, 0x62, 0x72, 0x61, 0x63, 0x6B, 0x65, 0x74, 0x73 } ),
				new TestSet("Empty \\c()",									new byte[] { 0x45, 0x6D, 0x70, 0x74, 0x79, 0x20 } ),
			},

			// hex
			new TestSet[]
			{
				new TestSet("\\h()",							new byte[] { } ),
				new TestSet("\\h(00)",							new byte[] { 0x00 } ),
				new TestSet("\\h(01)",							new byte[] { 0x01 } ),
				new TestSet("\\h(20)",							new byte[] { 0x20 } ),
				new TestSet("\\h(7F)",							new byte[] { 0x7F } ),
				new TestSet("\\h(80)",							new byte[] { 0x80 } ),
				new TestSet("\\h(81)",							new byte[] { 0x81 } ),
				new TestSet("\\h(AA)",							new byte[] { 0xAA } ),
				new TestSet("\\h(FE)",							new byte[] { 0xFE } ),
				new TestSet("\\h(FF)",							new byte[] { 0xFF } ),
				new TestSet("\\h(00 01 20 7F 80 81 AA FE FF)",	new byte[] { 0x00, 0x01, 0x20, 0x7F, 0x80, 0x81, 0xAA, 0xFE, 0xFF } ),
				new TestSet("\\h(00 00)",						new byte[] { 0x00, 0x00 } ),
				new TestSet("\\h(FF FF)",						new byte[] { 0xFF, 0xFF } ),
				new TestSet("\\h(23 5D 24 81 20 A5)",			new byte[] { 0x23, 0x5D, 0x24, 0x81, 0x20, 0xA5 } ),
				new TestSet("\\h(00 \\h(FF) 00)",				new byte[] { 0x00, 0xFF, 0x00 } ),
			},
		};

		#endregion

		#region Tests
		//==========================================================================================
		// Test
		//==========================================================================================

		#region Tests > Parser
		//------------------------------------------------------------------------------------------
		// Tests > Parser
		//------------------------------------------------------------------------------------------

		[Test]
		public void TestParser()
		{
			Exception exceptionToNUnit = null;

			foreach (TestSet[] tsArray in _testSets)
			{
				foreach (TestSet ts in tsArray)
				{
					Domain.Parser.Parser parser;
					byte[] outputBytes = new byte[] { };

					try
					{
						parser = new Domain.Parser.Parser(ts.Encoding);
						outputBytes = parser.Parse(ts.InputString);
						Assert.AreEqual(outputBytes, ts.OutputBytes);
					}
					catch (Exception ex)
					{
						// catch assertion exceptions to ensure that all test sets are run in any case
						//   but keep first exception to signal NUnit that test has failed
						if (exceptionToNUnit == null)
							exceptionToNUnit = ex;

						Console.WriteLine("Invalid parser output bytes:");
						Console.WriteLine();
						Console.WriteLine("Input string =");
						Console.WriteLine("\"" + ts.InputString + "\"");
						Console.WriteLine();
						Console.WriteLine("Expected output bytes =");
						foreach (byte b in ts.OutputBytes)
						{
							Console.Write("0x" + b.ToString("X2") + ", ");
						}
						Console.WriteLine();
						Console.WriteLine("Actual output bytes =");
						foreach (byte b in outputBytes)
						{
							Console.Write("0x" + b.ToString("X2") + ", ");
						}
						Console.WriteLine();
					}
				}
			}

			// re-throw first exception to signal NUnit that test has failed
			if (exceptionToNUnit != null)
				throw (exceptionToNUnit);
		}

		#endregion

		#endregion
	}
}
