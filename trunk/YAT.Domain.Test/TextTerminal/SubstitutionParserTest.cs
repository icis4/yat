using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using YAT.Domain;

namespace YAT.Domain.Test.TextTerminal
{
	[TestFixture]
	public class SubstitutionParserTest
	{
		#region Tests
		//==========================================================================================
		// Test
		//==========================================================================================

		#region Test SubstitutionParser
		//------------------------------------------------------------------------------------------
		// Test SubstitutionParser
		//------------------------------------------------------------------------------------------

		[Test]
		public void TestSubstitutionParser()
		{
			Domain.Parser.SubstitutionParser p = new Domain.Parser.SubstitutionParser();
			byte[] a;

			try
			{
				a = p.Parse("\\c(A)\\c(b)CdEfGhIiKlMnOpQrStUvWxYz<Cr><Lf>", CharSubstitution.ToUpper);

				Console.WriteLine("String:");
				Console.Write("  ");
				foreach (byte b in a)
				{
					Console.Write((char)b);
				}
				Console.Write("\n");
				Console.WriteLine("Bytes:");
				Console.Write("  ");
				foreach (byte b in a)
				{
					Console.Write(b.ToString() + " ");
				}
				Console.Write("\n");
			}
			catch (Domain.Parser.FormatException ex)
			{
				Assert.Fail(ex.Message);
			}
		}

		#endregion

		#endregion
	}
}
