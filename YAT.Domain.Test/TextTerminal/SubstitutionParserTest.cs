//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using YAT.Domain;

namespace YAT.Domain.Test.TextTerminal
{
	/// <summary></summary>
	[TestFixture]
	public class SubstitutionParserTest
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private struct TestSet
		{
			public CharSubstitution Substitution;
			public string InputString;
			public byte[] OutputBytes;

			public TestSet(CharSubstitution substitution, string inputString, byte[] outputBytes)
			{
				Substitution = substitution;
				InputString = inputString;
				OutputBytes = outputBytes;
			}
		};

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private readonly TestSet[] testSets =
		{
			new TestSet(CharSubstitution.ToUpper, @"\c(A)\c(b)CdEfGhIiKlMnOpQrStUvWxYz<Cr><Lf>",	new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x49, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x0D, 0x0A } ) ,
		};

		#endregion

		#region Tests
		//==========================================================================================
		// Test
		//==========================================================================================

		#region Test SubstitutionParser
		//------------------------------------------------------------------------------------------
		// Test SubstitutionParser
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestSubstitutionParser()
		{
			Exception exceptionToNUnit = null;

			foreach (TestSet ts in this.testSets)
			{
				Domain.Parser.SubstitutionParser parser;
				byte[] outputBytes = new byte[] { };

				try
				{
					parser = new Domain.Parser.SubstitutionParser();
					outputBytes = parser.Parse(ts.InputString, ts.Substitution);
					Assert.AreEqual(ts.OutputBytes, outputBytes);
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
					Console.WriteLine(@"""" + ts.InputString + @"""");
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

			// re-throw first exception to signal NUnit that test has failed
			if (exceptionToNUnit != null)
				throw (exceptionToNUnit);
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
