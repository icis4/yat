//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System.Collections;
using System.Diagnostics.CodeAnalysis;

using NUnit.Framework;

using MKY.Net.Test;

#endregion

namespace YAT.Domain.Test.TextTerminal
{
	/// <summary></summary>
	public static class EolTestData
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


					=> TestData = verschiedene EOL
						> Beide gleich
						> Unterschiedlich

					=> PingPong mit verschiedenen Texten, welche zum Teil Überschneidungen mit EOL haben
						> <ASCII>
						> Normal
						> Gemischt


				// ToUpper.
				yield return (new TestCaseData(CharSubstitution.ToUpper, @"\c(A)\c(b)CdEfGhIiKlMnOpQrStUvWxYz<Cr><Lf>", new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x49, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x0D, 0x0A } ).SetName("ToUpper"));
			}
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class EolTest
	{
		#region Tests
		//==========================================================================================
		// Test
		//==========================================================================================

		#region Test SubstitutionParser
		//------------------------------------------------------------------------------------------
		// Test SubstitutionParser
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "bytes", Justification = "The naming emphasizes the difference between bytes and other parameters.")]
		[Test, IPv4LoopbackIsAvailableCategory, TestCaseSource(typeof(EolTestData), "TestCases")]
		public virtual void TestSubstitutionParser(string eolAB, string eolBA)
		{
			Settings.TerminalSettings settingsA = Utilities.GetTextTcpAutoSocketOnIPv4LoopbackSettings();
			settingsA.TextTerminal.TxEol = eolAB;
			settingsA.TextTerminal.RxEol = eolBA;
			using (Domain.TextTerminal terminalA = new Domain.TextTerminal(settingsA))
			{
				terminalA.Start();

				Settings.TerminalSettings settingsB = Utilities.GetTextTcpAutoSocketOnIPv4LoopbackSettings();
				settingsB.TextTerminal.TxEol = eolBA;
				settingsB.TextTerminal.RxEol = eolAB;
				using (Domain.TextTerminal terminalB = new Domain.TextTerminal(settingsB))
				{
					terminalB.Start();

					int countA = 0;
					int countB = 0;

					terminalA.SendLine(""); // A#1
					countA++;
					Verify(terminalA, terminalB, eolAB, eolBA, countA);

					terminalA.SendLine("AA"); // A#2
					countA++;
					Verify(terminalA, terminalB, eolAB, eolBA, countA);

					terminalA.SendLine("ABABAB"); // A#3
					countA++;
					Verify(terminalA, terminalB, eolAB, eolBA, countA);

					terminalB.SendLine("<CR>"); // B#1
					countB++;
					Verify(terminalB, terminalA, eolBA, eolAB, countB);

					terminalB.SendLine("<CR><CR>"); // B#2
					countB++;
					Verify(terminalB, terminalA, eolBA, eolAB, countB);

					terminalB.SendLine("<LF><CR>"); // B#3
					countB++;
					Verify(terminalB, terminalA, eolBA, eolAB, countB);

					terminalA.SendLine("<CR>"); // A#4
					countA++;
					Verify(terminalA, terminalB, eolAB, eolBA, countA);

					terminalB.SendLine("BBBB"); // B#4
					countB++;
					Verify(terminalB, terminalA, eolBA, eolAB, countB);
				}
			}
		}

		#endregion

		private void Verify(Domain.TextTerminal terminalA, Domain.TextTerminal terminalB, string eolAB, string eolBA, int count)
		{
			if (eolAB == eolBA)
			{
				Utilities.WaitForTransmission(terminalA, terminalB, count);
				Utilities.VerifyLines(terminalA.RepositoryToDisplayLines(RepositoryType.Tx), terminalB.RepositoryToDisplayLines(RepositoryType.Rx), count);
			}
			else
			{
				Utilities.WaitForTransmission(terminalA, count);
				Utilities.VerifyLines(terminalA.RepositoryToDisplayLines(RepositoryType.Tx), count);
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
