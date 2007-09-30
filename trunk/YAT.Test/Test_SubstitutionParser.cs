using System;

using YAT.Domain;

namespace YAT.Test
{
	public class Test_SubstitutionParser
	{
		public static void Test()
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
				Console.WriteLine(ex.Message);
			}
		}
	}
}
