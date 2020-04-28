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

using System.Diagnostics.CodeAnalysis;
using System.Threading;

using MKY.Collections.Generic;

using NUnit.Framework;

#endregion

namespace YAT.Domain.Test.Terminal
{
	/// <summary></summary>
	[TestFixture]
	public class KeywordsSerialPortTest
	{
		#region Apply
		//==========================================================================================
		// Apply
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Too many values to verify.")]
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Don't care, straightforward test implementation.")]
		[Test, TestCaseSource(typeof(GenericTestData), "TestCasesSerialPortLoopbackPairs")]
		public virtual void TestApplyToSerialPortLoopbackPairs(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorA,
		                                                       Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorB)
		{
			const int WaitForChange   = 1500; // 1000 ms is not sufficient.
			const int WaitForDisposal =  100;

			using (var parser = new Domain.Parser.Parser(Domain.Parser.Mode.NoEscapes))
			{
				byte[] parseResult;

				var settingsA = settingsDescriptorA.Value1(settingsDescriptorA.Value2);
				var standardPortNumberInitiallyA = settingsA.IO.SerialPort.PortId.StandardPortNumber;
				using (var terminalA = new Domain.TextTerminal(settingsA))
				{
					Assert.That(terminalA.Start(), Is.True, "Terminal A could not be started");

					string keyword;
					string text;
					int textByteCount;
					int eolByteCount = 2; // Fixed to default of <CR><LF>.
					int expectedTotalByteCountAB = 0;
					int expectedTotalByteCountBA = 0;
					int expectedTotalLineCountAB = 0;
					int expectedTotalLineCountBA = 0;

					// Initial pingpong:

					var settingsB = settingsDescriptorB.Value1(settingsDescriptorB.Value2);
					var standardPortNumberInitiallyB = settingsB.IO.SerialPort.PortId.StandardPortNumber;
					using (var terminalB = new Domain.TextTerminal(settingsB))
					{
						Assert.That(terminalB.Start(), Is.True, "Terminal B could not be started");
						Utilities.WaitForConnection(terminalA, terminalB);

						text = "Ping A>>B";
						Assert.That(parser.TryParse(text, out parseResult));
						terminalA.SendTextLine(text);
						textByteCount = parseResult.Length;
						expectedTotalByteCountAB += (textByteCount + eolByteCount);
						expectedTotalLineCountAB++;
						Utilities.WaitForTransmissionAndVerifyCounts(terminalA, terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);

						text = "Pong B>>A";
						Assert.That(parser.TryParse(text, out parseResult));
						terminalB.SendTextLine(text);
						textByteCount = parseResult.Length;
						expectedTotalByteCountBA += (textByteCount + eolByteCount);
						expectedTotalLineCountBA++;
						Utilities.WaitForTransmissionAndVerifyCounts(terminalB, terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

						terminalB.Stop();
						Utilities.WaitForDisconnection(terminalB);
					} // using (terminalB)

					// Swap ports using keyword (A) vs. settings (B):

					keyword = @"\!(Port(" + standardPortNumberInitiallyB + @"))\!(NoEOL())";
					terminalA.SendTextLine(keyword);
					Thread.Sleep(WaitForChange);
					Assert.That(terminalA.TerminalSettings.IO.SerialPort.PortId.StandardPortNumber, Is.EqualTo(standardPortNumberInitiallyB));

					settingsB.IO.SerialPort.PortId = standardPortNumberInitiallyA;
					using (var terminalB = new Domain.TextTerminal(settingsB))
					{
						Assert.That(terminalB.Start(), Is.True, "Terminal B could not be restarted");
						Utilities.WaitForConnection(terminalA, terminalB);

						text = "Swapped A>>B";
						Assert.That(parser.TryParse(text, out parseResult));
						terminalA.SendTextLine(text);
						textByteCount = parseResult.Length;
						expectedTotalByteCountAB += (textByteCount + eolByteCount);
						expectedTotalLineCountAB++;
						Utilities.WaitForSendingAndVerifyCounts(terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);

						text = "Swapped B>>A";
						Assert.That(parser.TryParse(text, out parseResult));
						terminalB.SendTextLine(text);
						textByteCount = parseResult.Length;
						expectedTotalByteCountBA += (textByteCount + eolByteCount);
						expectedTotalLineCountBA++;
						Utilities.WaitForReceivingAndVerifyCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

						terminalB.Stop();
						Utilities.WaitForDisconnection(terminalB);
					} // using (terminalB)

					// Change settings using keyword (A) vs. settings (B):

					keyword = @"\!(Baud(19200))\!(DataBits(7))\!(Parity(2))\!(StopBits(2))\!(NoEOL())"; // Not changing flow control, too difficult to verify here.
					terminalA.SendTextLine(keyword);
					Thread.Sleep(WaitForChange * 4);
					Assert.That(terminalA.TerminalSettings.IO.SerialPort.Communication.BaudRate, Is.EqualTo((int)MKY.IO.Ports.BaudRate.Baud_19200));
					Assert.That(terminalA.TerminalSettings.IO.SerialPort.Communication.DataBits, Is.EqualTo(     MKY.IO.Ports.DataBits.Seven));
					Assert.That(terminalA.TerminalSettings.IO.SerialPort.Communication.Parity,   Is.EqualTo(  System.IO.Ports.Parity  .Even));
					Assert.That(terminalA.TerminalSettings.IO.SerialPort.Communication.StopBits, Is.EqualTo(  System.IO.Ports.StopBits.Two));

					settingsB.IO.SerialPort.Communication.BaudRate = (int)MKY.IO.Ports.BaudRate.Baud_19200;
					settingsB.IO.SerialPort.Communication.DataBits =      MKY.IO.Ports.DataBits.Seven;
					settingsB.IO.SerialPort.Communication.Parity   =   System.IO.Ports.Parity  .Even;
					settingsB.IO.SerialPort.Communication.StopBits =   System.IO.Ports.StopBits.Two;
					using (var terminalB = new Domain.TextTerminal(settingsB))
					{
						Assert.That(terminalB.Start(), Is.True, "Terminal B could not be restarted");
						Utilities.WaitForConnection(terminalA, terminalB);

						text = "Changed dedicated A>>B";
						Assert.That(parser.TryParse(text, out parseResult));
						terminalA.SendTextLine(text);
						textByteCount = parseResult.Length;
						expectedTotalByteCountAB += (textByteCount + eolByteCount);
						expectedTotalLineCountAB++;
						Utilities.WaitForSendingAndVerifyCounts(terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);

						text = "Changed dedicated B>>A";
						Assert.That(parser.TryParse(text, out parseResult));
						terminalB.SendTextLine(text);
						textByteCount = parseResult.Length;
						expectedTotalByteCountBA += (textByteCount + eolByteCount);
						expectedTotalLineCountBA++;
						Utilities.WaitForReceivingAndVerifyCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

						terminalB.Stop();
						Utilities.WaitForDisconnection(terminalB);
					} // using (terminalB)

					// Change settings using keyword (A) vs. settings (B):

					keyword = @"\!(PortSettings(115200, 8, 0, 1))\!(NoEOL())"; // Not changing flow control, too difficult to verify here.
					terminalA.SendTextLine(keyword);
					Thread.Sleep(WaitForChange);
					Assert.That(terminalA.TerminalSettings.IO.SerialPort.Communication.BaudRate, Is.EqualTo((int)MKY.IO.Ports.BaudRate.Baud_115200));
					Assert.That(terminalA.TerminalSettings.IO.SerialPort.Communication.DataBits, Is.EqualTo(     MKY.IO.Ports.DataBits.Eight));
					Assert.That(terminalA.TerminalSettings.IO.SerialPort.Communication.Parity,   Is.EqualTo(  System.IO.Ports.Parity  .None));
					Assert.That(terminalA.TerminalSettings.IO.SerialPort.Communication.StopBits, Is.EqualTo(  System.IO.Ports.StopBits.One));

					settingsB.IO.SerialPort.Communication.BaudRate = (int)MKY.IO.Ports.BaudRate.Baud_115200;
					settingsB.IO.SerialPort.Communication.DataBits =      MKY.IO.Ports.DataBits.Eight;
					settingsB.IO.SerialPort.Communication.Parity   =   System.IO.Ports.Parity  .None;
					settingsB.IO.SerialPort.Communication.StopBits =   System.IO.Ports.StopBits.One;
					using (var terminalB = new Domain.TextTerminal(settingsB))
					{
						Assert.That(terminalB.Start(), Is.True, "Terminal B could not be restarted");
						Utilities.WaitForConnection(terminalA, terminalB);

						text = "Changed combined A>>B";
						Assert.That(parser.TryParse(text, out parseResult));
						terminalA.SendTextLine(text);
						textByteCount = parseResult.Length;
						expectedTotalByteCountAB += (textByteCount + eolByteCount);
						expectedTotalLineCountAB++;
						Utilities.WaitForSendingAndVerifyCounts(terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);

						text = "Changed combined B>>A";
						Assert.That(parser.TryParse(text, out parseResult));
						terminalB.SendTextLine(text);
						textByteCount = parseResult.Length;
						expectedTotalByteCountBA += (textByteCount + eolByteCount);
						expectedTotalLineCountBA++;
						Utilities.WaitForReceivingAndVerifyCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

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
