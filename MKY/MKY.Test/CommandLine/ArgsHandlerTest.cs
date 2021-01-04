//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.28 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2021 Matthias Kläy.
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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Text;

using MKY.Collections.Generic;
using MKY.CommandLine;

using NUnit.Framework;
using NUnitEx;

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
					yield return (new TestCaseData(pair.Value1, pair.Value2).SetName(ArrayEx.ValuesToString(pair.Value2)));
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
					yield return (new TestCaseData(pair.Value1, pair.Value2.Value1, pair.Value2.Value2).SetName(ArrayEx.ValuesToString(pair.Value2.Value2)));
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
				yield return (new Pair<bool, Pair<string, string[]>>(false, new Pair<string, string[]>(null, new string[] { "/svo=" })));
				yield return (new Pair<bool, Pair<string, string[]>>(false, new Pair<string, string[]>(null, new string[] { "/svo:" })));

				yield return (new Pair<bool, Pair<string, string[]>>(true, new Pair<string, string[]>("QuotedText", new string[] { @"/svo=""QuotedText""" })));
				yield return (new Pair<bool, Pair<string, string[]>>(true, new Pair<string, string[]>("QuotedText", new string[] { "/svo", @"""QuotedText""" })));
				yield return (new Pair<bool, Pair<string, string[]>>(true, new Pair<string, string[]>("Quoted Text With Spaces", new string[] { @"/svo=""Quoted Text With Spaces""" })));
				yield return (new Pair<bool, Pair<string, string[]>>(true, new Pair<string, string[]>("Quoted Text With Spaces", new string[] { "/svo", @"""Quoted Text With Spaces""" })));
				yield return (new Pair<bool, Pair<string, string[]>>(true, new Pair<string, string[]>("QuotedTextWithLeadingAndTrailing", new string[] { @" /svo=""QuotedTextWithLeadingAndTrailing""  " })));
				yield return (new Pair<bool, Pair<string, string[]>>(true, new Pair<string, string[]>("QuotedTextWithLeadingAndTrailing", new string[] { "/svo", @" ""QuotedTextWithLeadingAndTrailing""  " })));

				yield return (new Pair<bool, Pair<string, string[]>>(true, new Pair<string, string[]>(@"QuotedText\\""WithQuotes\\""", new string[] { @"/svo=""QuotedText\\""WithQuotes\\""""" })));
				yield return (new Pair<bool, Pair<string, string[]>>(true, new Pair<string, string[]>(@"QuotedText\\""WithQuotes\\""", new string[] { "/svo", @"""QuotedText\\""WithQuotes\\""""" })));

				// Quoted text that looks like another option:
				yield return (new Pair<bool, Pair<string, string[]>>(true, new Pair<string, string[]>("/BooleanOption",         new string[] { @"/svo=""/BooleanOption""" })));
				yield return (new Pair<bool, Pair<string, string[]>>(true, new Pair<string, string[]>("/IntValueOption=1",      new string[] { @"/svo=""/IntValueOption=1""" })));
				yield return (new Pair<bool, Pair<string, string[]>>(true, new Pair<string, string[]>("/DoubleValueOption:2.3", new string[] { @"/svo=""/DoubleValueOption:2.3""" })));
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			get
			{
				foreach (Pair<bool, Pair<string, string[]>> pair in TestCasePairs)
					yield return (new TestCaseData(pair.Value1, pair.Value2.Value1, pair.Value2.Value2).SetName(ArrayEx.ValuesToString(pair.Value2.Value2)));
			}
		}

		#endregion
	}

	/// <summary></summary>
	public static class BooleanOptionTestData
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
				yield return (new Pair<bool, Pair<bool, string[]>>(true,  new Pair<bool, string[]>(true, new string[] { "/bo" })));

				yield return (new Pair<bool, Pair<bool, string[]>>(false, new Pair<bool, string[]>(true, new string[] { "/bo=MyText" })));
				yield return (new Pair<bool, Pair<bool, string[]>>(false, new Pair<bool, string[]>(true, new string[] { "/bo:MyText" })));
				yield return (new Pair<bool, Pair<bool, string[]>>(true,  new Pair<bool, string[]>(true, new string[] { "/bo", "MyText" })));
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			get
			{
				foreach (Pair<bool, Pair<bool, string[]>> pair in TestCasePairs)
					yield return (new TestCaseData(pair.Value1, pair.Value2.Value1, pair.Value2.Value2).SetName(ArrayEx.ValuesToString(pair.Value2.Value2)));
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
				yield return (new Pair<bool, Pair<int, string[]>>(false, new Pair<int, string[]>(0, new string[] { "/ivo=" })));
				yield return (new Pair<bool, Pair<int, string[]>>(false, new Pair<int, string[]>(0, new string[] { "/ivo:" })));
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
					yield return (new TestCaseData(pair.Value1, pair.Value2.Value1, pair.Value2.Value2).SetName(ArrayEx.ValuesToString(pair.Value2.Value2)));
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
				yield return (new Pair<bool, Pair<double, string[]>>(false, new Pair<double, string[]>(0, new string[] { @"/dvo=" })));
				yield return (new Pair<bool, Pair<double, string[]>>(false, new Pair<double, string[]>(0, new string[] { @"/dvo:" })));
				yield return (new Pair<bool, Pair<double, string[]>>(false, new Pair<double, string[]>(0, new string[] { @"/dvo=ABC" })));
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			get
			{
				foreach (Pair<bool, Pair<double, string[]>> pair in TestCasePairs)
					yield return (new TestCaseData(pair.Value1, pair.Value2.Value1, pair.Value2.Value2).SetName(ArrayEx.ValuesToString(pair.Value2.Value2)));
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
				yield return (new Pair<bool, Pair<CommandLineEnum, string[]>>(true, new Pair<CommandLineEnum, string[]>(CommandLineEnum.Bb,  new string[] { @"/evo:Bb" })));
				yield return (new Pair<bool, Pair<CommandLineEnum, string[]>>(true, new Pair<CommandLineEnum, string[]>(CommandLineEnum.Ccc, new string[] { @"/evo", "cCc" })));

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
					yield return (new TestCaseData(pair.Value1, pair.Value2.Value1, pair.Value2.Value2).SetName(ArrayEx.ValuesToString(pair.Value2.Value2)));
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
				yield return (new Pair<bool, Pair<int, string[]>>(true, new Pair<int, string[]>(2, new string[] { "/svo=ABC", "/bo" })));
				yield return (new Pair<bool, Pair<int, string[]>>(true, new Pair<int, string[]>(5, new string[] { "/svo=ABC", "/bo", "/ivo=1", "/dvo=2.2", "/evo=A" })));
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			get
			{
				foreach (Pair<bool, Pair<int, string[]>> pair in TestCasePairs)
					yield return (new TestCaseData(pair.Value1, pair.Value2.Value1, pair.Value2.Value2).SetName(ArrayEx.ValuesToString(pair.Value2.Value2)));
			}
		}

		#endregion
	}

	/// <summary></summary>
	public static class StringArrayOptionTestData
	{
		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		private static IEnumerable<Pair<bool, Pair<string[], string[]>>> TestCasePairs
		{
			get
			{
				yield return (new Pair<bool, Pair<string[], string[]>>(true, new Pair<string[], string[]>(new string[] { "A" },              new string[] { "/sao=A" })));
				yield return (new Pair<bool, Pair<string[], string[]>>(true, new Pair<string[], string[]>(new string[] { "A", "BB", "CCC" }, new string[] { "/sao=A", "BB", "CCC" })));

				// Quoted text that looks like another option:
				yield return (new Pair<bool, Pair<string[], string[]>>(true, new Pair<string[], string[]>(new string[] { "A", "BB", @"""/BooleanOption""" },         new string[] { "/sao=A", "BB", @"""/BooleanOption""" })));
				yield return (new Pair<bool, Pair<string[], string[]>>(true, new Pair<string[], string[]>(new string[] { "A", "BB", @"""/IntValueOption=1""" },      new string[] { "/sao=A", "BB", @"""/IntValueOption=1""" })));
				yield return (new Pair<bool, Pair<string[], string[]>>(true, new Pair<string[], string[]>(new string[] { "A", "BB", @"""/DoubleValueOption:2.3""" }, new string[] { "/sao=A", "BB", @"""/DoubleValueOption:2.3""" })));
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			get
			{
				foreach (Pair<bool, Pair<string[], string[]>> pair in TestCasePairs)
					yield return (new TestCaseData(pair.Value1, pair.Value2.Value1, pair.Value2.Value2).SetName(ArrayEx.ValuesToString(pair.Value2.Value2)));
			}
		}

		#endregion
	}

	/// <summary></summary>
	public static class IntArrayOptionTestData
	{
		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		private static IEnumerable<Pair<bool, Pair<int[], string[]>>> TestCasePairs
		{
			get
			{
				yield return (new Pair<bool, Pair<int[], string[]>>(true, new Pair<int[], string[]>(new int[] { 1 },       new string[] { "/iao=1" })));
				yield return (new Pair<bool, Pair<int[], string[]>>(true, new Pair<int[], string[]>(new int[] { 1, 2, 3 }, new string[] { "/iao=1", "2", "3" })));
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			get
			{
				foreach (Pair<bool, Pair<int[], string[]>> pair in TestCasePairs)
					yield return (new TestCaseData(pair.Value1, pair.Value2.Value1, pair.Value2.Value2).SetName(ArrayEx.ValuesToString(pair.Value2.Value2)));
			}
		}

		#endregion
	}

	/// <summary></summary>
	public static class CombinedTestData
	{
		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		private static IEnumerable<Pair<bool, Pair<Pair<int, int>, string[]>>> TestCasePairs
		{
			get
			{
				yield return (new Pair<bool, Pair<Pair<int, int>, string[]>>(true, new Pair<Pair<int, int>, string[]>(new Pair<int, int>(2, 1), new string[] { "/svo=ABC", "/bo", "/sao=A", "BB", "CCC" })));
				yield return (new Pair<bool, Pair<Pair<int, int>, string[]>>(true, new Pair<Pair<int, int>, string[]>(new Pair<int, int>(5, 2), new string[] { "/svo=ABC", "/bo", "/sao=A", "BB", "CCC", "/ivo=1", "/dvo=2.2", "/evo=A", "/iao=1", "2", "3" })));
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			get
			{
				foreach (Pair<bool, Pair<Pair<int, int>, string[]>> pair in TestCasePairs)
					yield return (new TestCaseData(pair.Value1, pair.Value2.Value1.Value1, pair.Value2.Value1.Value2, pair.Value2.Value2).SetName(ArrayEx.ValuesToString(pair.Value2.Value2)));
			}
		}

		#endregion
	}

	/// <summary></summary>
	public static class DevelopmentValidationTestData
	{
		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			get
			{
				yield return (new TestCaseData(true, typeof(CommandLineArgs)));

				yield return (new TestCaseData(false, typeof(InvalidCommandLineArgs1)));
				yield return (new TestCaseData(false, typeof(InvalidCommandLineArgs2)));
				yield return (new TestCaseData(false, typeof(InvalidCommandLineArgs3)));
				yield return (new TestCaseData(false, typeof(InvalidCommandLineArgs4)));
				yield return (new TestCaseData(false, typeof(InvalidCommandLineArgs5)));
				yield return (new TestCaseData(false, typeof(InvalidCommandLineArgs6)));
				yield return (new TestCaseData(false, typeof(InvalidCommandLineArgs7)));
				yield return (new TestCaseData(false, typeof(InvalidCommandLineArgs8)));
				yield return (new TestCaseData(false, typeof(InvalidCommandLineArgs9)));
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

			Assert.That(cla.ProcessAndValidate(), Is.True);
			Assert.That(cla.IsValid,              Is.True);
			Assert.That(cla.HelpIsRequested,      Is.False);

			Assert.That(cla.PureValueArg,           Is.Null.Or.Empty);
			Assert.That(cla.CombinedValueOptionArg, Is.Null.Or.Empty);
			Assert.That(cla.StringValueOption,      Is.Null.Or.Empty);
			Assert.That(cla.BooleanOption,          Is.False);
			Assert.That(cla.IntValueOption,       Is.EqualTo(0));
			Assert.That(cla.DoubleValueOption,    Is.EqualTo(0.0));
			Assert.That((int)cla.EnumValueOption, Is.EqualTo(0));
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

			Assert.That(cla.ProcessAndValidate(), Is.EqualTo(isValid));
			Assert.That(cla.IsValid,              Is.EqualTo(isValid));
			Assert.That(cla.HelpIsRequested,      Is.EqualTo(isValid));
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

			Assert.That(cla.ProcessAndValidate(), Is.EqualTo(isValid));
			Assert.That(cla.IsValid,              Is.EqualTo(isValid));
			Assert.That(cla.HelpIsRequested,      Is.False);

			if (isValid)
			{
				Assert.That(cla.ValueArgsCount, Is.EqualTo(1));
				Assert.That(cla.ValueArgs[0],   Is.EqualTo(commandLineArgs[0]));
				Assert.That(cla.PureValueArg,   Is.EqualTo(expectedValue));

				Assert.That(cla.OptionArgsCount, Is.EqualTo(0));
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

			Assert.That(cla.ProcessAndValidate(), Is.EqualTo(isValid));
			Assert.That(cla.IsValid,              Is.EqualTo(isValid));
			Assert.That(cla.HelpIsRequested,      Is.False);

			if (isValid)
			{
				Assert.That(cla.OptionIsGiven("svo"), Is.True);
				Assert.That(cla.StringValueOption,    Is.EqualTo(expectedValue));
			}
		}

		#endregion

		#region Tests > BooleanOption
		//------------------------------------------------------------------------------------------
		// Tests > BooleanOption
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, TestCaseSource(typeof(BooleanOptionTestData), "TestCases")]
		public virtual void TestBooleanOption(bool isValid, bool expectedValue, params string[] commandLineArgs)
		{
			CommandLineArgs cla = new CommandLineArgs(commandLineArgs);

			Assert.That(cla.ProcessAndValidate(), Is.EqualTo(isValid));
			Assert.That(cla.IsValid,              Is.EqualTo(isValid));
			Assert.That(cla.HelpIsRequested,      Is.False);

			if (isValid)
			{
				Assert.That(cla.OptionIsGiven("bo"), Is.True);
				Assert.That(cla.BooleanOption,       Is.EqualTo(expectedValue));
			}
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

			Assert.That(cla.ProcessAndValidate(), Is.EqualTo(isValid));
			Assert.That(cla.IsValid,              Is.EqualTo(isValid));
			Assert.That(cla.HelpIsRequested,      Is.False);

			if (isValid)
			{
				Assert.That(cla.OptionIsGiven("ivo"), Is.True);
				Assert.That(cla.IntValueOption,       Is.EqualTo(expectedValue));
			}
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

			Assert.That(cla.ProcessAndValidate(), Is.EqualTo(isValid));
			Assert.That(cla.IsValid,              Is.EqualTo(isValid));
			Assert.That(cla.HelpIsRequested,      Is.False);

			if (isValid)
			{
				Assert.That(cla.OptionIsGiven("dvo"), Is.True);
				Assert.That(cla.DoubleValueOption,    Is.EqualTo(expectedValue));
			}
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

			Assert.That(cla.ProcessAndValidate(), Is.EqualTo(isValid));
			Assert.That(cla.IsValid,              Is.EqualTo(isValid));
			Assert.That(cla.HelpIsRequested,      Is.False);

			if (isValid)
			{
				Assert.That(cla.OptionIsGiven("evo"), Is.True);
				Assert.That(cla.EnumValueOption,      Is.EqualTo(expectedValue));
			}
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

			Assert.That(cla.ProcessAndValidate(), Is.EqualTo(isValid));
			Assert.That(cla.IsValid,              Is.EqualTo(isValid));
			Assert.That(cla.HelpIsRequested,      Is.False);

			if (isValid)
				Assert.That(cla.OptionArgsCount, Is.EqualTo(expectedNumberOfOptions));
		}

		#endregion

		#region Tests > StringArrayOption
		//------------------------------------------------------------------------------------------
		// Tests > StringArrayOption
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, TestCaseSource(typeof(StringArrayOptionTestData), "TestCases")]
		public virtual void TestArrayOption(bool isValid, string[] expectedArrayOptionArgs, params string[] commandLineArgs)
		{
			CommandLineArgs cla = new CommandLineArgs(commandLineArgs);

			Assert.That(cla.ProcessAndValidate(), Is.EqualTo(isValid));
			Assert.That(cla.IsValid,              Is.EqualTo(isValid));
			Assert.That(cla.HelpIsRequested,      Is.False);

			if (isValid)
			{
				Assert.That(cla.OptionIsGiven("sao"));
				Assert.That(ArrayEx.ValuesEqual(expectedArrayOptionArgs, cla.StringArrayOption));
			}
		}

		#endregion

		#region Tests > IntArrayOption
		//------------------------------------------------------------------------------------------
		// Tests > IntArrayOption
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, TestCaseSource(typeof(IntArrayOptionTestData), "TestCases")]
		public virtual void TestArrayOption(bool isValid, int[] expectedArrayOptionArgs, params string[] commandLineArgs)
		{
			CommandLineArgs cla = new CommandLineArgs(commandLineArgs);

			Assert.That(cla.ProcessAndValidate(), Is.EqualTo(isValid));
			Assert.That(cla.IsValid,              Is.EqualTo(isValid));
			Assert.That(cla.HelpIsRequested,      Is.False);

			if (isValid)
			{
				Assert.That(cla.OptionIsGiven("iao"));
				Assert.That(ArrayEx.ValuesEqual(expectedArrayOptionArgs, cla.IntArrayOption));
			}
		}

		#endregion

		#region Tests > Combined
		//------------------------------------------------------------------------------------------
		// Tests > Combined
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, TestCaseSource(typeof(CombinedTestData), "TestCases")]
		public virtual void TestCombined(bool isValid, int expectedNumberOfOptions, int expectedNumberOfArrayOptions, params string[] commandLineArgs)
		{
			CommandLineArgs cla = new CommandLineArgs(commandLineArgs);

			Assert.That(cla.ProcessAndValidate(), Is.EqualTo(isValid));
			Assert.That(cla.IsValid,              Is.EqualTo(isValid));
			Assert.That(cla.HelpIsRequested,      Is.False);

			if (isValid)
			{
				Assert.That(cla.OptionArgsCount,      Is.EqualTo(expectedNumberOfOptions));
				Assert.That(cla.ArrayOptionArgsCount, Is.EqualTo(expectedNumberOfArrayOptions));
			}
		}

		#endregion

		#region Tests > Override
		//------------------------------------------------------------------------------------------
		// Tests > Override
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestValueArgOverride()
		{
			string arg = "ABC";
			string argOverride = "XYZ";

			// Normal case:
			CommandLineArgs cla = new CommandLineArgs(new string[] { arg });
			Assert.That(cla.ProcessAndValidate(), Is.True);
			Assert.That(cla.IsValid, Is.True);

			Assert.That(cla.ValueArgsCount, Is.EqualTo(1));
			Assert.That(cla.ValueArgs[0],   Is.EqualTo(arg));
			Assert.That(cla.PureValueArg,   Is.EqualTo(arg));

			// Override:
			cla.Override("PureValueArg", argOverride);
			Assert.That(cla.ProcessAndValidate(), Is.True);
			Assert.That(cla.IsValid,              Is.True);

			Assert.That(cla.ValueArgsCount, Is.EqualTo(1));
			Assert.That(cla.ValueArgs[0],   Is.EqualTo(arg)); // Arg array must still contain the original arg.
			Assert.That(cla.PureValueArg,   Is.EqualTo(argOverride)); // Arg field must contain the override.
		}

		/// <summary></summary>
		[Test]
		public virtual void TestOptionArgOverride()
		{
			string argValue = "ABC";
			string arg = "/svo=" + argValue;
			string argValueOverride = "XYZ";

			// Normal case:
			CommandLineArgs cla = new CommandLineArgs(new string[] { arg });
			Assert.That(cla.ProcessAndValidate(), Is.True);
			Assert.That(cla.IsValid, Is.True);

			Assert.That(cla.OptionArgsCount,   Is.EqualTo(1));
			Assert.That(cla.OptionArgs[0],     Is.EqualTo(arg));
			Assert.That(cla.StringValueOption, Is.EqualTo(argValue));

			// Override:
			cla.Override("StringValueOption", argValueOverride);
			Assert.That(cla.ProcessAndValidate(), Is.True);
			Assert.That(cla.IsValid,              Is.True);

			Assert.That(cla.OptionArgsCount,   Is.EqualTo(1));
			Assert.That(cla.OptionArgs[0],     Is.EqualTo(arg)); // Arg array must still contain the original arg.
			Assert.That(cla.StringValueOption, Is.EqualTo(argValueOverride)); // Arg field must contain the override.
		}

		/// <summary></summary>
		[Test]
		public virtual void TestCombinedArgOverride()
		{
			string argValue = "ABC";
			string arg = "/cvoa=" + argValue;
			string argValueOverride = "XYZ";

			// Normal case:
			CommandLineArgs cla = new CommandLineArgs(new string[] { arg });
			Assert.That(cla.ProcessAndValidate(), Is.True);
			Assert.That(cla.IsValid, Is.True);

			Assert.That(cla.OptionArgsCount,        Is.EqualTo(1));
			Assert.That(cla.OptionArgs[0],          Is.EqualTo(arg));
			Assert.That(cla.CombinedValueOptionArg, Is.EqualTo(argValue));

			// Override:
			cla.Override("CombinedValueOptionArg", argValueOverride);
			Assert.That(cla.ProcessAndValidate(), Is.True);
			Assert.That(cla.IsValid,              Is.True);

			Assert.That(cla.OptionArgsCount,        Is.EqualTo(1));
			Assert.That(cla.OptionArgs[0],          Is.EqualTo(arg)); // Arg array must still contain the original arg.
			Assert.That(cla.CombinedValueOptionArg, Is.EqualTo(argValueOverride)); // Arg field must contain the override.
		}

		#endregion

		#region Tests > DevelopmentValidation
		//------------------------------------------------------------------------------------------
		// Tests > DevelopmentValidation
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, TestCaseSource(typeof(DevelopmentValidationTestData), "TestCases")]
		[AssertThrowsCategory]
		public virtual void TestDevelopmentValidation(bool isValid, Type type)
		{
			TestDevelopmentValidation_type = type;

			// Must not throw an exception:
			Assert.That(new TestDelegate(TestDevelopmentValidation_GetConstructorAndCreateObject), Throws.Nothing);

			// Must throw an exception in case of invalid arguments:
			if (isValid)
			{
				Assert.That(new TestDelegate(TestDevelopmentValidation_ProcessAndValidate), Throws.Nothing);
			}
		#if (DEBUG)
			else
			{
				Assert.That(new TestDelegate(TestDevelopmentValidation_ProcessAndValidate), Throws.TypeOf<ArgsHandler.DevelopmentValidationException>());
			}
		#endif
		}

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "'type' does start with a lower case letter.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private Type TestDevelopmentValidation_type;

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "'args' does start with a lower case letter.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private ArgsHandler TestDevelopmentValidation_args;

		private void TestDevelopmentValidation_GetConstructorAndCreateObject()
		{
			ConstructorInfo ci = TestDevelopmentValidation_type.GetConstructor(new Type[] { typeof(string[]) });
			TestDevelopmentValidation_args = (ArgsHandler)ci.Invoke(new object[] { new string[] { "" } });
		}

		private void TestDevelopmentValidation_ProcessAndValidate()
		{
			TestDevelopmentValidation_args.ProcessAndValidate();
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
			var di = new DirectoryInfo(Environment.CurrentDirectory);
			for (int i = 0; i < 4; i++)
				di = di.Parent;

			// Set path to "<Root>\MKY\MKY.Test\CommandLine\":
			string fullNamespace     = GetType().Namespace;
			string packageName       = StringEx.Left(fullNamespace, fullNamespace.IndexOf('.'));
			string assemblyName      = GetType().Assembly.GetName().Name;
			string relativeNamespace = StringEx.Right(fullNamespace, (fullNamespace.Length - assemblyName.Length)).TrimStart('.');

			var sb = new StringBuilder(di.FullName); // <Root>

			if (!string.IsNullOrEmpty(packageName))
				sb.Append(Path.DirectorySeparatorChar + packageName); // MKY

			if (!string.IsNullOrEmpty(assemblyName))
				sb.Append(Path.DirectorySeparatorChar + assemblyName); // MKY.Test

			if (!string.IsNullOrEmpty(relativeNamespace))
				sb.Append(Path.DirectorySeparatorChar + relativeNamespace); // CommandLine

			sb.Append(Path.DirectorySeparatorChar + "ExpectedHelpText.txt");

			// Get the file with the expected help text:
			string expected;
			using (var sr = new StreamReader(sb.ToString()))
			{
				expected = sr.ReadToEnd();
			}

			// Get the help text and compare it to the file:
			var cla = new CommandLineArgs(null);
			string actual = cla.GetHelpText(80);
			Assert.That(actual, Is.EqualTo(expected));
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
