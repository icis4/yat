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
// YAT Version 2.1.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2020 Matthias Kläy.
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
using System.Globalization;
using System.Threading;

using MKY.Collections.Generic;

using NUnit.Framework;

#endregion

namespace YAT.Domain.Test.Terminal
{
	/// <summary></summary>
	public static class KeywordPortTestData
	{
		#region Format
		//==========================================================================================
		// Format
		//==========================================================================================

		/// <param name="loopbackSettings">
		/// Quadruple of...
		/// ...Pair(terminalSettingsDelegateA, terminalSettingsArgumentA)...
		/// ...Pair(terminalSettingsDelegateB, terminalSettingsArgumentB)...
		/// ...string testCaseName...
		/// ...string[] testCaseCategories.
		/// </param>
		private static IEnumerable<TestCaseData> TestCases(Quadruple<Pair<Utilities.TerminalSettingsDelegate<string>, string>, Pair<Utilities.TerminalSettingsDelegate<string>, string>, string, string[]> loopbackSettings)
		{
		////foreach (var data in TestCasesData) // TestCaseData(...) kept for symmetricity with YAT.Model.Test.
			{
				// Arguments:
				var args = new List<object>(2); // data.Arguments);
				args.Add(loopbackSettings.Value1);
				args.Add(loopbackSettings.Value2);
				var tcd = new TestCaseData(args.ToArray()); // TestCaseData(Pair settingsDescriptorA, Pair settingsDescriptorB).

				// Name:
				tcd.SetName(loopbackSettings.Value3); // + data.TestName);

				// Category(ies):
				foreach (string cat in loopbackSettings.Value4)
					tcd.SetCategory(cat);

				yield return (tcd);
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCasesSerialPortLoopbackPairs
		{
			get
			{
				foreach (var loopbackSettings in Utilities.TransmissionSettings.SerialPortLoopbackPairs)
				{
					foreach (var testCase in TestCases(loopbackSettings))
						yield return (testCase);
				}
			}
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class KeywordPortTest
	{
		#region Port
		//==========================================================================================
		// Port
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Too many values to verify.")]
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Don't care, straightforward test implementation.")]
		[Test, TestCaseSource(typeof(KeywordPortTestData), "TestCasesSerialPortLoopbackPairs")]
		public virtual void SerialPortLoopbackPairs(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorA,
		                                            Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorB)
		{
			const int WaitForDisposal = 100;

			using (var parser = new Domain.Parser.Parser(Domain.Parser.Mode.NoEscapes))
			{
				byte[] parseResult;

				var settingsA = settingsDescriptorA.Value1(settingsDescriptorA.Value2);
				using (var terminalA = new Domain.TextTerminal(settingsA))
				{
					Assert.That(terminalA.Start(), Is.True, "Terminal A could not be started");

					var settingsB = settingsDescriptorB.Value1(settingsDescriptorB.Value2);
					using (var terminalB = new Domain.TextTerminal(settingsB))
					{
						Assert.That(terminalB.Start(), Is.True, "Terminal B could not be started");
						Utilities.WaitForConnection(terminalA, terminalB);

						string text;
						int textByteCount;
						int eolByteCount = 2; // Fixed to default of <CR><LF>.
						int expectedTotalByteCount = 0;
						int expectedTotalLineCount = 0;

						// Initial pingpong:
						text = "Ping A>>B";
						terminalA.SendTextLine(text);
						Assert.That(parser.TryParse(text, out parseResult));
						textByteCount = parseResult.Length;
						expectedTotalByteCount += (textByteCount + eolByteCount);
						expectedTotalLineCount++;
						Utilities.WaitForTransmissionAndVerifyCounts(terminalA, terminalB, expectedTotalByteCount, expectedTotalLineCount);

						text = "Pong B>>A";
						terminalB.SendTextLine(text);
						Assert.That(parser.TryParse(text, out parseResult));
						textByteCount = parseResult.Length;
						expectedTotalByteCount += (textByteCount + eolByteCount);
						expectedTotalLineCount++;
						Utilities.WaitForTransmissionAndVerifyCounts(terminalB, terminalA, expectedTotalByteCount, expectedTotalLineCount);

//YAT.Domain.Test.Terminal.KeywordPortTest.SerialPortLoopbackPairs_COM11_COM12:
//Transmission timeout! Not enough data transmitted within expected interval.
//
//YAT.Domain.Test.Terminal.KeywordPortTest.SerialPortLoopbackPairs_COM21_COM22:
//System.IO.IOException : Der Anschluss COM22 ist nicht vorhanden.
//
//YAT.Domain.Test.Terminal.KeywordPortTest.SerialPortLoopbackPairs_COM31_COM32:
//System.IO.IOException : Der Anschluss COM31 ist nicht vorhanden.
//
//YAT.Domain.Test.Terminal.KeywordPortTest.SerialPortLoopbackPairs_COM101_COM102:
//Transmission timeout! Not enough data transmitted within expected interval.

						// PENDING: Swap ports A using keyword, B using settings

						// PENDING: Change settings A using keyword, B using settings

						terminalB.Stop();
						Utilities.WaitForDisconnection(terminalB);
					} // using (terminalB)

					terminalA.Stop();
					Utilities.WaitForDisconnection(terminalA);
				} // using (terminalA)

			} // using (parser)

			Thread.Sleep(WaitForDisposal);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
