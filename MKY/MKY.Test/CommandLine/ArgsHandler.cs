//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.6
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using NUnit.Framework;

using MKY.Collections.Generic;

#endregion

namespace MKY.Test.CommandLine
{
	/// <summary></summary>
	public static class OptionFormatsAndHelpTestData
	{
		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		private static IEnumerable<Pair<bool, string[]>> TestCasePairs
		{
			get
			{
				yield return (new Pair<bool, string[]>(true, new string[] { "/?"     }));
				yield return (new Pair<bool, string[]>(true, new string[] { "-?"     }));
				yield return (new Pair<bool, string[]>(true, new string[] { "--?"    }));
				yield return (new Pair<bool, string[]>(true, new string[] { "/h"     }));
				yield return (new Pair<bool, string[]>(true, new string[] { "-h"     }));
				yield return (new Pair<bool, string[]>(true, new string[] { "--h"    }));
				yield return (new Pair<bool, string[]>(true, new string[] { "/H"     }));
				yield return (new Pair<bool, string[]>(true, new string[] { "-H"     }));
				yield return (new Pair<bool, string[]>(true, new string[] { "--H"    }));
				yield return (new Pair<bool, string[]>(true, new string[] { "/help"  }));
				yield return (new Pair<bool, string[]>(true, new string[] { "-help"  }));
				yield return (new Pair<bool, string[]>(true, new string[] { "--help" }));
				yield return (new Pair<bool, string[]>(true, new string[] { "/Help"  }));
				yield return (new Pair<bool, string[]>(true, new string[] { "-Help"  }));
				yield return (new Pair<bool, string[]>(true, new string[] { "--Help" }));
				yield return (new Pair<bool, string[]>(true, new string[] { "/hELp"  }));
				yield return (new Pair<bool, string[]>(true, new string[] { "-HeLp"  }));
				yield return (new Pair<bool, string[]>(true, new string[] { "--HELP" }));
				yield return (new Pair<bool, string[]>(true, new string[] { "--HelpText" }));

				yield return (new Pair<bool, string[]>(true, new string[] { "/?", "-h", "--help", "--HelpText", "-HELP", "--h" }));

				yield return (new Pair<bool, string[]>(false, new string[] { "/??"          }));
				yield return (new Pair<bool, string[]>(false, new string[] { "-hh"          }));
				yield return (new Pair<bool, string[]>(false, new string[] { "--helper"     }));
				yield return (new Pair<bool, string[]>(false, new string[] { "--helptexter" }));
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			get
			{
				foreach (Pair<bool, string[]> pair in TestCasePairs)
					yield return (new TestCaseData(pair.Value1, pair.Value2).SetName(ArrayEx.ElementsToString(pair.Value2)));
			}
		}

		#endregion
	}

	/// <summary></summary>
	public static class ValueArgTestData
	{
		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		private static IEnumerable<Pair<bool, Pair<string, string[]>>> TestCasePairs
		{
			get
			{
				yield return (new Pair<bool, Pair<string, string[]>>(true, new Pair<string, string[]>("ValueArg", new string[] { @"ValueArg" })));
				yield return (new Pair<bool, Pair<string, string[]>>(true, new Pair<string, string[]>("QuotedValueArg", new string[] { @"""QuotedValueArg""" })));
				yield return (new Pair<bool, Pair<string, string[]>>(true, new Pair<string, string[]>("Quoted ValueArg WithSpaces", new string[] { @"""Quoted ValueArg WithSpaces""" })));
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			get
			{
				foreach (Pair<bool, Pair<string, string[]>> pair in TestCasePairs)
					yield return (new TestCaseData(pair.Value1, pair.Value2.Value1, pair.Value2.Value2).SetName(ArrayEx.ElementsToString(pair.Value2.Value2)));
			}
		}

		#endregion
	}

	/// <summary></summary>
	public static class StringValueOptionTestData
	{
		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		private static IEnumerable<Pair<bool, Pair<string, string[]>>> TestCasePairs
		{
			get
			{
				yield return (new Pair<bool, Pair<string, string[]>>(true, new Pair<string, string[]>("MyText", new string[] { "/svo=MyText" })));
				yield return (new Pair<bool, Pair<string, string[]>>(true, new Pair<string, string[]>("MyText", new string[] { "/svo:MyText" })));
				yield return (new Pair<bool, Pair<string, string[]>>(true, new Pair<string, string[]>("MyText", new string[] { "/svo",   "MyText" })));
				yield return (new Pair<bool, Pair<string, string[]>>(true, new Pair<string, string[]>("MyText", new string[] { " /svo",  "MyText" })));
				yield return (new Pair<bool, Pair<string, string[]>>(true, new Pair<string, string[]>("MyText", new string[] { "/svo ",  "MyText" })));
				yield return (new Pair<bool, Pair<string, string[]>>(true, new Pair<string, string[]>("MyText", new string[] { " /svo ", "MyText" })));
				yield return (new Pair<bool, Pair<string, string[]>>(true, new Pair<string, string[]>("MyText", new string[] { " /svo",  " MyText" })));
				yield return (new Pair<bool, Pair<string, string[]>>(true, new Pair<string, string[]>("MyText", new string[] { "/svo ",  "MyText " })));
				yield return (new Pair<bool, Pair<string, string[]>>(true, new Pair<string, string[]>("MyText", new string[] { " /svo ", " MyText " })));

				yield return (new Pair<bool, Pair<string, string[]>>(false, new Pair<string, string[]>(null, new string[] { "/svo" })));

				yield return (new Pair<bool, Pair<string, string[]>>(true, new Pair<string, string[]>("QuotedText", new string[] { @"/svo=""QuotedText""" })));
				yield return (new Pair<bool, Pair<string, string[]>>(true, new Pair<string, string[]>("QuotedText", new string[] { "/svo", @"""QuotedText""" })));
				yield return (new Pair<bool, Pair<string, string[]>>(true, new Pair<string, string[]>("Quoted Text With Spaces", new string[] { @"/svo=""Quoted Text With Spaces""" })));
				yield return (new Pair<bool, Pair<string, string[]>>(true, new Pair<string, string[]>("Quoted Text With Spaces", new string[] { "/svo", @"""Quoted Text With Spaces""" })));
				yield return (new Pair<bool, Pair<string, string[]>>(true, new Pair<string, string[]>("QuotedTextWithLeadingAndTrailing", new string[] { @" /svo=""QuotedTextWithLeadingAndTrailing""  " })));
				yield return (new Pair<bool, Pair<string, string[]>>(true, new Pair<string, string[]>("QuotedTextWithLeadingAndTrailing", new string[] { "/svo", @" ""QuotedTextWithLeadingAndTrailing""  " })));
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			get
			{
				foreach (Pair<bool, Pair<string, string[]>> pair in TestCasePairs)
					yield return (new TestCaseData(pair.Value1, pair.Value2.Value1, pair.Value2.Value2).SetName(ArrayEx.ElementsToString(pair.Value2.Value2)));
			}
		}

		#endregion
	}

	/// <summary></summary>
	public static class SimpleBooleanOptionTestData
	{
		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		private static IEnumerable<Pair<bool, Pair<bool, string[]>>> TestCasePairs
		{
			get
			{
				yield return (new Pair<bool, Pair<bool, string[]>>(true,  new Pair<bool, string[]>(true, new string[] { "/sbo" })));

				yield return (new Pair<bool, Pair<bool, string[]>>(false, new Pair<bool, string[]>(true, new string[] { "/sbo=MyText" })));
				yield return (new Pair<bool, Pair<bool, string[]>>(false, new Pair<bool, string[]>(true, new string[] { "/sbo:MyText" })));
				yield return (new Pair<bool, Pair<bool, string[]>>(true,  new Pair<bool, string[]>(true, new string[] { "/sbo", "MyText" })));
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			get
			{
				foreach (Pair<bool, Pair<bool, string[]>> pair in TestCasePairs)
					yield return (new TestCaseData(pair.Value1, pair.Value2.Value1, pair.Value2.Value2).SetName(ArrayEx.ElementsToString(pair.Value2.Value2)));
			}
		}

		#endregion
	}

	/// <summary></summary>
	public static class IntValueOptionTestData
	{
		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		private static IEnumerable<Pair<bool, Pair<int, string[]>>> TestCasePairs
		{
			get
			{
				yield return (new Pair<bool, Pair<int, string[]>>(true, new Pair<int, string[]>(0, new string[] { "/ivo=0" })));
				yield return (new Pair<bool, Pair<int, string[]>>(true, new Pair<int, string[]>(1, new string[] { "/ivo:1" })));
				yield return (new Pair<bool, Pair<int, string[]>>(true, new Pair<int, string[]>(2, new string[] { "/ivo", "2" })));

				yield return (new Pair<bool, Pair<int, string[]>>(true, new Pair<int, string[]>(-1, new string[] { "/ivo=-1" })));
				yield return (new Pair<bool, Pair<int, string[]>>(true, new Pair<int, string[]>(-2, new string[] { "/ivo:-2" })));
				yield return (new Pair<bool, Pair<int, string[]>>(true, new Pair<int, string[]>(-3, new string[] { "/ivo", "-3" })));

				yield return (new Pair<bool, Pair<int, string[]>>(false, new Pair<int, string[]>(0,            new string[] { "/ivo=2147483648" })));
				yield return (new Pair<bool, Pair<int, string[]>>(true,  new Pair<int, string[]>(int.MaxValue, new string[] { "/ivo=2147483647" })));
				yield return (new Pair<bool, Pair<int, string[]>>(true,  new Pair<int, string[]>(int.MinValue, new string[] { "/ivo=-2147483648" })));
				yield return (new Pair<bool, Pair<int, string[]>>(false, new Pair<int, string[]>(0,            new string[] { "/ivo=-2147483649" })));

				yield return (new Pair<bool, Pair<int, string[]>>(false, new Pair<int, string[]>(0, new string[] { "/ivo" })));
				yield return (new Pair<bool, Pair<int, string[]>>(false, new Pair<int, string[]>(0, new string[] { "/ivo=1.5" })));
				yield return (new Pair<bool, Pair<int, string[]>>(false, new Pair<int, string[]>(0, new string[] { "/ivo=ABC" })));
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			get
			{
				foreach (Pair<bool, Pair<int, string[]>> pair in TestCasePairs)
					yield return (new TestCaseData(pair.Value1, pair.Value2.Value1, pair.Value2.Value2).SetName(ArrayEx.ElementsToString(pair.Value2.Value2)));
			}
		}

		#endregion
	}

	/// <summary></summary>
	public static class DoubleValueOptionTestData
	{
		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		private static IEnumerable<Pair<bool, Pair<double, string[]>>> TestCasePairs
		{
			get
			{
				yield return (new Pair<bool, Pair<double, string[]>>(true, new Pair<double, string[]>(0.0, new string[] { @"/dvo=0.0" })));
				yield return (new Pair<bool, Pair<double, string[]>>(true, new Pair<double, string[]>(1.5, new string[] { @"/dvo:1.5" })));
				yield return (new Pair<bool, Pair<double, string[]>>(true, new Pair<double, string[]>(2.0, new string[] { @"/dvo:2  " })));
				yield return (new Pair<bool, Pair<double, string[]>>(true, new Pair<double, string[]>(3.3, new string[] { @"/dvo", "3.3" })));
				yield return (new Pair<bool, Pair<double, string[]>>(true, new Pair<double, string[]>(5.0, new string[] { @"/dvo", "5  " })));

				yield return (new Pair<bool, Pair<double, string[]>>(true, new Pair<double, string[]>(-1.01, new string[] { @"/dvo=-1.01" })));
				yield return (new Pair<bool, Pair<double, string[]>>(true, new Pair<double, string[]>(-2.99, new string[] { @"/dvo:-2.99" })));
				yield return (new Pair<bool, Pair<double, string[]>>(true, new Pair<double, string[]>(-3.0,  new string[] { @"/dvo:-3   " })));
				yield return (new Pair<bool, Pair<double, string[]>>(true, new Pair<double, string[]>(-10.1, new string[] { @"/dvo", "-10.1" })));
				yield return (new Pair<bool, Pair<double, string[]>>(true, new Pair<double, string[]>(-22.0, new string[] { @"/dvo", "-22  " })));

				yield return (new Pair<bool, Pair<double, string[]>>(false, new Pair<double, string[]>(0,             new string[] { @"/dvo=1.0+310" })));
				yield return (new Pair<bool, Pair<double, string[]>>(true,  new Pair<double, string[]>( 1.79769e+308, new string[] { @"/dvo=1.79769e+308" })));
				yield return (new Pair<bool, Pair<double, string[]>>(true,  new Pair<double, string[]>(-1.79769e+308, new string[] { @"/dvo=-1.79769e+308" })));
				yield return (new Pair<bool, Pair<double, string[]>>(false, new Pair<double, string[]>(0,             new string[] { @"/dvo=-1.0+310" })));

				yield return (new Pair<bool, Pair<double, string[]>>(false, new Pair<double, string[]>(0, new string[] { @"/dvo" })));
				yield return (new Pair<bool, Pair<double, string[]>>(false, new Pair<double, string[]>(0, new string[] { @"/dvo=ABC" })));
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			get
			{
				foreach (Pair<bool, Pair<double, string[]>> pair in TestCasePairs)
					yield return (new TestCaseData(pair.Value1, pair.Value2.Value1, pair.Value2.Value2).SetName(ArrayEx.ElementsToString(pair.Value2.Value2)));
			}
		}

		#endregion
	}

	/// <summary></summary>
	public static class EnumValueOptionTestData
	{
		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		private static IEnumerable<Pair<bool, Pair<CommandLineEnum, string[]>>> TestCasePairs
		{
			get
			{
				yield return (new Pair<bool, Pair<CommandLineEnum, string[]>>(true, new Pair<CommandLineEnum, string[]>(CommandLineEnum.A,   new string[] { @"/evo=A" })));
				yield return (new Pair<bool, Pair<CommandLineEnum, string[]>>(true, new Pair<CommandLineEnum, string[]>(CommandLineEnum.Be,  new string[] { @"/evo:BE" })));
				yield return (new Pair<bool, Pair<CommandLineEnum, string[]>>(true, new Pair<CommandLineEnum, string[]>(CommandLineEnum.Cee, new string[] { @"/evo", "cEe" })));

				yield return (new Pair<bool, Pair<CommandLineEnum, string[]>>(false, new Pair<CommandLineEnum, string[]>(0, new string[] { @"/evo" })));
				yield return (new Pair<bool, Pair<CommandLineEnum, string[]>>(false, new Pair<CommandLineEnum, string[]>(0, new string[] { @"/evo=ABC" })));
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			get
			{
				foreach (Pair<bool, Pair<CommandLineEnum, string[]>> pair in TestCasePairs)
					yield return (new TestCaseData(pair.Value1, pair.Value2.Value1, pair.Value2.Value2).SetName(ArrayEx.ElementsToString(pair.Value2.Value2)));
			}
		}

		#endregion
	}

	/// <summary></summary>
	public static class MultipleOptionsTestData
	{
		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		private static IEnumerable<Pair<bool, Pair<int, string[]>>> TestCasePairs
		{
			get
			{
				yield return (new Pair<bool, Pair<int, string[]>>(true, new Pair<int, string[]>(2, new string[] { "/cvoa=AA", "/svo=ABC" })));
				yield return (new Pair<bool, Pair<int, string[]>>(true, new Pair<int, string[]>(2, new string[] { "/svo=ABC", "/sbo" })));
				yield return (new Pair<bool, Pair<int, string[]>>(true, new Pair<int, string[]>(5, new string[] { "/svo=ABC", "/sbo", "/ivo=1", "/dvo=2.2", "/evo=A" })));
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			get
			{
				foreach (Pair<bool, Pair<int, string[]>> pair in TestCasePairs)
					yield return (new TestCaseData(pair.Value1, pair.Value2.Value1, pair.Value2.Value2).SetName(ArrayEx.ElementsToString(pair.Value2.Value2)));
			}
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class ArgsHandlerTest
	{
		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		#region Tests > EmptyCommandLine
		//------------------------------------------------------------------------------------------
		// Tests > EmptyCommandLine
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[CLSCompliant(false)]
		[Test]
		public virtual void TestEmptyCommandLine([Values(null, new string[] { "" })] string[] commandLineArgs)
		{
			CommandLineArgs cla = new CommandLineArgs(commandLineArgs);

			Assert.IsTrue(cla.IsValid);
			Assert.IsFalse(cla.HelpIsRequested);

			Assert.IsTrue(string.IsNullOrEmpty(cla.PureValueArg));
			Assert.IsTrue(string.IsNullOrEmpty(cla.CombinedValueOptionArg));
			Assert.IsTrue(string.IsNullOrEmpty(cla.StringValueOption));
			Assert.IsFalse(cla.SimpleBooleanOption);
			Assert.AreEqual(0,      cla.IntValueOption);
			Assert.AreEqual(0.0,    cla.DoubleValueOption);
			Assert.AreEqual(0, (int)cla.EnumValueOption);
		}

		#endregion

		#region Tests > OptionFormatsAndHelp
		//------------------------------------------------------------------------------------------
		// Tests > OptionFormatsAndHelp
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, TestCaseSource(typeof(OptionFormatsAndHelpTestData), "TestCases")]
		public virtual void TestOptionFormatsAndHelp(bool isValid, params string[] commandLineArgs)
		{
			CommandLineArgs cla = new CommandLineArgs(commandLineArgs);

			Assert.AreEqual(isValid, cla.IsValid);
			Assert.AreEqual(isValid, cla.HelpIsRequested);
		}

		#endregion

		#region Tests > ValueArg
		//------------------------------------------------------------------------------------------
		// Tests > ValueArg
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, TestCaseSource(typeof(ValueArgTestData), "TestCases")]
		public virtual void TestValueArg(bool isValid, string expectedValue, params string[] commandLineArgs)
		{
			CommandLineArgs cla = new CommandLineArgs(commandLineArgs);

			Assert.AreEqual(isValid, cla.IsValid);
			Assert.IsFalse(cla.HelpIsRequested);

			if (isValid)
			{
				Assert.AreEqual(1, cla.ValueArgsCount);
				Assert.AreEqual(commandLineArgs[0], cla.ValueArgs[0]);
				Assert.AreEqual(expectedValue, cla.PureValueArg);

				Assert.AreEqual(0, cla.OptionArgsCount);
			}
		}

		#endregion

		#region Tests > StringValueOption
		//------------------------------------------------------------------------------------------
		// Tests > StringValueOption
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, TestCaseSource(typeof(StringValueOptionTestData), "TestCases")]
		public virtual void TestStringValueOption(bool isValid, string expectedValue, params string[] commandLineArgs)
		{
			CommandLineArgs cla = new CommandLineArgs(commandLineArgs);

			Assert.AreEqual(isValid, cla.IsValid);
			Assert.IsFalse(cla.HelpIsRequested);

			if (isValid)
				Assert.AreEqual(expectedValue, cla.StringValueOption);
		}

		#endregion

		#region Tests > SimpleBooleanOption
		//------------------------------------------------------------------------------------------
		// Tests > SimpleBooleanOption
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, TestCaseSource(typeof(SimpleBooleanOptionTestData), "TestCases")]
		public virtual void TestSimpleBooleanOption(bool isValid, bool expectedValue, params string[] commandLineArgs)
		{
			CommandLineArgs cla = new CommandLineArgs(commandLineArgs);

			Assert.AreEqual(isValid, cla.IsValid);
			Assert.IsFalse(cla.HelpIsRequested);

			if (isValid)
				Assert.AreEqual(expectedValue, cla.SimpleBooleanOption);
		}

		#endregion

		#region Tests > IntValueOption
		//------------------------------------------------------------------------------------------
		// Tests > IntValueOption
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, TestCaseSource(typeof(IntValueOptionTestData), "TestCases")]
		public virtual void TestIntValueOption(bool isValid, int expectedValue, params string[] commandLineArgs)
		{
			CommandLineArgs cla = new CommandLineArgs(commandLineArgs);

			Assert.AreEqual(isValid, cla.IsValid);
			Assert.IsFalse(cla.HelpIsRequested);

			if (isValid)
				Assert.AreEqual(expectedValue, cla.IntValueOption);
		}

		#endregion

		#region Tests > DoubleValueOption
		//------------------------------------------------------------------------------------------
		// Tests > DoubleValueOption
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, TestCaseSource(typeof(DoubleValueOptionTestData), "TestCases")]
		public virtual void TestDoubleValueOption(bool isValid, double expectedValue, params string[] commandLineArgs)
		{
			CommandLineArgs cla = new CommandLineArgs(commandLineArgs);

			Assert.AreEqual(isValid, cla.IsValid);
			Assert.IsFalse(cla.HelpIsRequested);

			if (isValid)
				Assert.AreEqual(expectedValue, cla.DoubleValueOption);
		}

		#endregion

		#region Tests > EnumValueOption
		//------------------------------------------------------------------------------------------
		// Tests > EnumValueOption
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, TestCaseSource(typeof(EnumValueOptionTestData), "TestCases")]
		public virtual void TestEnumValueOption(bool isValid, CommandLineEnum expectedValue, params string[] commandLineArgs)
		{
			CommandLineArgs cla = new CommandLineArgs(commandLineArgs);

			Assert.AreEqual(isValid, cla.IsValid);
			Assert.IsFalse(cla.HelpIsRequested);

			if (isValid)
				Assert.AreEqual(expectedValue, cla.EnumValueOption);
		}

		#endregion

		#region Tests > MultipleOptions
		//------------------------------------------------------------------------------------------
		// Tests > MultipleOptions
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, TestCaseSource(typeof(MultipleOptionsTestData), "TestCases")]
		public virtual void TestMultipleOptions(bool isValid, int expectedNumberOfOptions, params string[] commandLineArgs)
		{
			CommandLineArgs cla = new CommandLineArgs(commandLineArgs);

			Assert.AreEqual(isValid, cla.IsValid);
			Assert.IsFalse(cla.HelpIsRequested);

			if (isValid)
				Assert.AreEqual(expectedNumberOfOptions, cla.OptionArgsCount);
		}

		#endregion

		#region Tests > HelpText
		//------------------------------------------------------------------------------------------
		// Tests > HelpText
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestHelpText()
		{
			// Traverse path from "<Root>\YAT\YAT\bin\[Debug|Release]\YAT.exe" to "<Root>":
			DirectoryInfo di = new DirectoryInfo(Environment.CurrentDirectory);
			for (int i = 0; i < 4; i++)
				di = di.Parent;

			// Set path to "<Root>\MKY\!-Settings\<Directory>\":
			string assemblyFullName = this.GetType().Assembly.FullName;
			string namespaceName    = this.GetType().Namespace;

			string filePath = di.FullName + Path.DirectorySeparatorChar +
				"MKY" + Path.DirectorySeparatorChar +
				StringEx.Left(assemblyFullName, (assemblyFullName.IndexOf(','))) + Path.DirectorySeparatorChar + 
				StringEx.Right(namespaceName, (namespaceName.Length - namespaceName.LastIndexOf('.') - 1)) + Path.DirectorySeparatorChar + 
				"ExpectedHelpText.txt";

			// Get the file with the expected help text:
			string expected;
			using (StreamReader sr = new StreamReader(filePath))
			{
				expected = sr.ReadToEnd();
				sr.Close();
			}

			// Get the help text and compare it to the file:
			CommandLineArgs cla = new CommandLineArgs(null);
			string actual = cla.GetHelpText(80);
			Assert.AreEqual(expected, actual);
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
