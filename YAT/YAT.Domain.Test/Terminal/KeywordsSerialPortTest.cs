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
// YAT Version 2.2.0 Development
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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;

using MKY.IO.Ports.Test;
using MKY.IO.Serial.SerialPort;

using NUnit.Framework;

//// 'YAT.Domain.Parser' is not used due to ambiguity with 'YAT.Domain.Test.Parser'.
using YAT.Domain.Settings;

#endregion

namespace YAT.Domain.Test.Terminal
{
	/// <summary></summary>
	[TestFixture]
	public class KeywordsSerialPortTest
	{
		#region TestSettings
		//==========================================================================================
		// TestSettings
		//==========================================================================================

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data.Generic), "TestCasesSerialPortLoopbackPairs_Text")]
		public virtual void TestSettings(TerminalSettings settingsA, TerminalSettings settingsB)
		{
			if (!ConfigurationProvider.Configuration.LoopbackPairsAreAvailable)
				Assert.Ignore("No serial COM port loopback pairs are available, therefore this test is excluded. Ensure that at least one serial COM port loopback pairs is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			const int WaitForApply = 1500; // 1000 ms is not sufficient.

			Encoding parserEncoding;
			Endianness parserEndianness;
			Domain.Parser.Mode parserMode;
			Utilities.AssertMatchingParserSettingsForSendText(settingsA, settingsB, out parserEncoding, out parserEndianness, out parserMode);
			using (var parser = new Domain.Parser.Parser(parserEncoding, parserEndianness, parserMode))
			{
				var standardPortNumberInitiallyA = settingsA.IO.SerialPort.PortId.StandardPortNumber;
				using (var terminalA = TerminalFactory.CreateTerminal(settingsA))
				{
					Assert.That(terminalA.Start(), Is.True, "Terminal A could not be started!");

					string keyword;
					int expectedTotalByteCountAB = 0;
					int expectedTotalByteCountBA = 0;
					int expectedTotalLineCountAB = 0;
					int expectedTotalLineCountBA = 0;

					// Initial ping-pong:

					var standardPortNumberInitiallyB = settingsB.IO.SerialPort.PortId.StandardPortNumber;
					using (var terminalB = TerminalFactory.CreateTerminal(settingsB))
					{
						Assert.That(terminalB.Start(), Is.True, "Terminal B could not be started!");
						Utilities.WaitForConnection(terminalA, terminalB);

						Utilities.TransmitAndAssertCounts(terminalA, terminalB, parser, "Ping A => B", ref expectedTotalByteCountAB, ref expectedTotalLineCountAB);
						Utilities.TransmitAndAssertCounts(terminalB, terminalA, parser, "Pong B => A", ref expectedTotalByteCountBA, ref expectedTotalLineCountBA);

						// Wait to ensure that no operation is ongoing anymore and verify again:
						Utilities.WaitForReverification();
						Utilities.AssertCounts(terminalB, terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

						// Refresh and verify again:
						terminalA.RefreshRepositories();
						terminalB.RefreshRepositories();
						Utilities.AssertCounts(terminalB, terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

						terminalB.Stop();
						Utilities.WaitForStop(terminalB);
					} // using (terminalB)

					// Swap ports using keyword (A) vs. settings (B):

					keyword = @"\!(Port(" + standardPortNumberInitiallyB + @"))\!(NoEOL)";
					terminalA.SendTextLine(keyword); // Intentionally using Line() with \!(NoEOL) as this will be required for a keyword-only predefined command.
					Thread.Sleep(WaitForApply);
					Assert.That(terminalA.TerminalSettings.IO.SerialPort.PortId.StandardPortNumber, Is.EqualTo(standardPortNumberInitiallyB));

					settingsB.IO.SerialPort.PortId = standardPortNumberInitiallyA;
					using (var terminalB = TerminalFactory.CreateTerminal(settingsB))
					{
						Assert.That(terminalB.Start(), Is.True, "Terminal B could not be restarted");
						Utilities.WaitForConnection(terminalA, terminalB);

						Utilities.TransmitAndAssertTxCounts(terminalA,            parser, "Swapped A => B", ref expectedTotalByteCountAB, ref expectedTotalLineCountAB);
						Utilities.TransmitAndAssertRxCounts(terminalB, terminalA, parser, "Swapped B => A", ref expectedTotalByteCountBA, ref expectedTotalLineCountBA);

						// Wait to ensure that no operation is ongoing anymore and verify again:
						Utilities.WaitForReverification();
						Utilities.AssertRxCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

						// Refresh and verify again:
						terminalA.RefreshRepositories();
						Utilities.AssertRxCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

						terminalB.Stop();
						Utilities.WaitForStop(terminalB);
					} // using (terminalB)

					// Change settings using keyword (A) vs. settings (B):

					keyword = @"\!(Baud(19200))\!(DataBits(7))\!(Parity(2))\!(StopBits(2))\!(NoEOL)"; // Not changing flow control, too difficult to verify here.
					terminalA.SendTextLine(keyword); // Intentionally using Line() with \!(NoEOL) as this will be required for a keyword-only predefined command.
					Thread.Sleep(WaitForApply * 4);
					Assert.That(terminalA.TerminalSettings.IO.SerialPort.Communication.BaudRate, Is.EqualTo((int)MKY.IO.Ports.BaudRate.Baud19200));
					Assert.That(terminalA.TerminalSettings.IO.SerialPort.Communication.DataBits, Is.EqualTo(     MKY.IO.Ports.DataBits.Seven));
					Assert.That(terminalA.TerminalSettings.IO.SerialPort.Communication.Parity,   Is.EqualTo(  System.IO.Ports.Parity  .Even));
					Assert.That(terminalA.TerminalSettings.IO.SerialPort.Communication.StopBits, Is.EqualTo(  System.IO.Ports.StopBits.Two));

					settingsB.IO.SerialPort.Communication.BaudRate = (int)MKY.IO.Ports.BaudRate.Baud19200;
					settingsB.IO.SerialPort.Communication.DataBits =      MKY.IO.Ports.DataBits.Seven;
					settingsB.IO.SerialPort.Communication.Parity   =   System.IO.Ports.Parity  .Even;
					settingsB.IO.SerialPort.Communication.StopBits =   System.IO.Ports.StopBits.Two;
					using (var terminalB = TerminalFactory.CreateTerminal(settingsB))
					{
						Assert.That(terminalB.Start(), Is.True, "Terminal B could not be restarted");
						Utilities.WaitForConnection(terminalA, terminalB);

						Utilities.TransmitAndAssertTxCounts(terminalA,            parser, "Changed dedicated A => B", ref expectedTotalByteCountAB, ref expectedTotalLineCountAB);
						Utilities.TransmitAndAssertRxCounts(terminalB, terminalA, parser, "Changed dedicated B => A", ref expectedTotalByteCountBA, ref expectedTotalLineCountBA);

						// Wait to ensure that no operation is ongoing anymore and verify again:
						Utilities.WaitForReverification();
						Utilities.AssertRxCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

						// Refresh and verify again:
						terminalA.RefreshRepositories();
						Utilities.AssertRxCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

						terminalB.Stop();
						Utilities.WaitForStop(terminalB);
					} // using (terminalB)

					// Change settings using keyword (A) vs. settings (B):

					keyword = @"\!(PortSettings(115200, 8, 0, 1))\!(NoEOL)"; // Not changing flow control, too difficult to verify here.
					terminalA.SendTextLine(keyword); // Intentionally using Line() with \!(NoEOL) as this will be required for a keyword-only predefined command.
					Thread.Sleep(WaitForApply);
					Assert.That(terminalA.TerminalSettings.IO.SerialPort.Communication.BaudRate, Is.EqualTo((int)MKY.IO.Ports.BaudRate.Baud115200));
					Assert.That(terminalA.TerminalSettings.IO.SerialPort.Communication.DataBits, Is.EqualTo(     MKY.IO.Ports.DataBits.Eight));
					Assert.That(terminalA.TerminalSettings.IO.SerialPort.Communication.Parity,   Is.EqualTo(  System.IO.Ports.Parity  .None));
					Assert.That(terminalA.TerminalSettings.IO.SerialPort.Communication.StopBits, Is.EqualTo(  System.IO.Ports.StopBits.One));

					settingsB.IO.SerialPort.Communication.BaudRate = (int)MKY.IO.Ports.BaudRate.Baud115200;
					settingsB.IO.SerialPort.Communication.DataBits =      MKY.IO.Ports.DataBits.Eight;
					settingsB.IO.SerialPort.Communication.Parity   =   System.IO.Ports.Parity  .None;
					settingsB.IO.SerialPort.Communication.StopBits =   System.IO.Ports.StopBits.One;
					using (var terminalB = TerminalFactory.CreateTerminal(settingsB))
					{
						Assert.That(terminalB.Start(), Is.True, "Terminal B could not be restarted");
						Utilities.WaitForConnection(terminalA, terminalB);

						Utilities.TransmitAndAssertTxCounts(terminalA,            parser, "Changed combined A => B", ref expectedTotalByteCountAB, ref expectedTotalLineCountAB);
						Utilities.TransmitAndAssertRxCounts(terminalB, terminalA, parser, "Changed combined B => A", ref expectedTotalByteCountBA, ref expectedTotalLineCountBA);

						// Wait to ensure that no operation is ongoing anymore and verify again:
						Utilities.WaitForReverification();
						Utilities.AssertRxCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

						// Refresh and verify again:
						terminalA.RefreshRepositories();
						Utilities.AssertRxCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

						terminalB.Stop();
						Utilities.WaitForStop(terminalB);
					} // using (terminalB)

					terminalA.Stop();
					Utilities.WaitForStop(terminalA);
				} // using (terminalA)
			} // using (parser)
		}

		#endregion

		#region TestSignals
		//==========================================================================================
		// TestSignals
		//==========================================================================================

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data.Generic), "TestCasesSerialPortLoopbackPairs_Text")]
		public virtual void TestSignals(TerminalSettings settingsA, TerminalSettings settingsB)
		{
			if (!ConfigurationProvider.Configuration.LoopbackPairsAreAvailable)
				Assert.Ignore("No serial COM port loopback pairs are available, therefore this test is excluded. Ensure that at least one serial COM port loopback pairs is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			const int WaitForApply = 100; // Happens async.

			Encoding parserEncoding;
			Endianness parserEndianness;
			Domain.Parser.Mode parserMode;
			Utilities.AssertMatchingParserSettingsForSendText(settingsA, settingsB, out parserEncoding, out parserEndianness, out parserMode);
			using (var parser = new Domain.Parser.Parser(parserEncoding, parserEndianness, parserMode))
			{
				settingsA.IO.SerialPort.Communication.FlowControl = SerialFlowControl.ManualHardware;

				if (TestIsExcludedForMCT(settingsA, new string[] { "COM11", "COM12", "COM13", "COM14" }))
					return; // Green bar.

				using (var terminalA = TerminalFactory.CreateTerminal(settingsA))
				{
					Assert.That(terminalA.Start(), Is.True, "Terminal A could not be started!");

					settingsB.IO.SerialPort.Communication.FlowControl = SerialFlowControl.Hardware; // Automatic.

					using (var terminalB = TerminalFactory.CreateTerminal(settingsB))
					{
						Assert.That(terminalB.Start(), Is.True, "Terminal B could not be started!");
						Utilities.WaitForConnection(terminalA, terminalB);

						string keyword;
						string text;
						const string Eol = "<CR><LF>";
						int expectedTotalByteCountAB = 0;
						int expectedTotalByteCountBA = 0;
						int expectedTotalLineCountAB = 0;
						int expectedTotalLineCountBA = 0;
						var expectedContentA = new List<string>(); // Applies to bidir only.
						var expectedContentB = new List<string>(); // Applies to bidir only.

						// Initial connection:
						AssertThatPinsAreTrue(terminalA.SerialPortControlPins);
						AssertThatPinsAreTrue(terminalB.SerialPortControlPins);

						// Initial ping-pong:
						text = "Ping A => B";
						Utilities.TransmitAndAssertCounts( terminalA, terminalB, parser, text, ref expectedTotalByteCountAB, ref expectedTotalLineCountAB);
						Utilities.AddAndAssertBidirContent(terminalA, terminalB, text + Eol, ref expectedContentA, ref expectedContentB);

						text = "Pong B => A";
						Utilities.TransmitAndAssertCounts( terminalB, terminalA, parser, text, ref expectedTotalByteCountBA, ref expectedTotalLineCountBA);
						Utilities.AddAndAssertBidirContent(terminalB, terminalA, text + Eol, ref expectedContentB, ref expectedContentA);

						// Switch RTS off:
						keyword = @"\!(RTSOff)\!(NoEOL)";
						terminalA.SendTextLine(keyword); // Intentionally using Line() with \!(NoEOL) as this will be required for a keyword-only predefined command.
						Thread.Sleep(WaitForApply);
						AssertThatRtsPinIsFalse(terminalA.SerialPortControlPins);
						AssertThatCtsPinIsFalse(terminalB.SerialPortControlPins);

						Utilities.WaitForTransmissionAndAssertCounts(terminalB, terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);
						Utilities.AssertBidirContent(                terminalB, terminalA, expectedContentB, expectedContentA);

						// Switch RTS on:
						keyword = @"\!(RTSOn)\!(NoEOL)";
						terminalA.SendTextLine(keyword); // Intentionally using Line() with \!(NoEOL) as this will be required for a keyword-only predefined command.
						Thread.Sleep(WaitForApply);
						AssertThatPinsAreTrue(terminalA.SerialPortControlPins);
						AssertThatPinsAreTrue(terminalB.SerialPortControlPins);

						Utilities.WaitForTransmissionAndAssertCounts(terminalB, terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);
						Utilities.AssertBidirContent(                terminalB, terminalA, expectedContentB, expectedContentA);

						// Switch RTS off:
						keyword = @"\!(RTSOff)\!(NoEOL)";
						terminalA.SendTextLine(keyword); // Intentionally using Line() with \!(NoEOL) as this will be required for a keyword-only predefined command.
						Thread.Sleep(WaitForApply);
						AssertThatRtsPinIsFalse(terminalA.SerialPortControlPins);
						AssertThatCtsPinIsFalse(terminalB.SerialPortControlPins);

						Utilities.WaitForTransmissionAndAssertCounts(terminalB, terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);
						Utilities.AssertBidirContent(                terminalB, terminalA, expectedContentB, expectedContentA);

						// Toggle RTS on:
						keyword = @"\!(RTSToggle)\!(NoEOL)";
						terminalA.SendTextLine(keyword); // Intentionally using Line() with \!(NoEOL) as this will be required for a keyword-only predefined command.
						Thread.Sleep(WaitForApply);
						AssertThatPinsAreTrue(terminalA.SerialPortControlPins);
						AssertThatPinsAreTrue(terminalB.SerialPortControlPins);

						Utilities.WaitForTransmissionAndAssertCounts(terminalB, terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);
						Utilities.AssertBidirContent(                terminalB, terminalA, expectedContentB, expectedContentA);

						// Subsequent ping-pong:
						text = "Ping A => B";
						Utilities.TransmitAndAssertCounts( terminalA, terminalB, parser, text, ref expectedTotalByteCountAB, ref expectedTotalLineCountAB);
						Utilities.AddAndAssertBidirContent(terminalA, terminalB, text + Eol, ref expectedContentA, ref expectedContentB);

						text = "Pong B => A";
						Utilities.TransmitAndAssertCounts( terminalB, terminalA, parser, text, ref expectedTotalByteCountBA, ref expectedTotalLineCountBA);
						Utilities.AddAndAssertBidirContent(terminalB, terminalA, text + Eol, ref expectedContentB, ref expectedContentA);

						// Switch DTR off:
						keyword = @"\!(DTROff)\!(NoEOL)";
						terminalA.SendTextLine(keyword); // Intentionally using Line() with \!(NoEOL) as this will be required for a keyword-only predefined command.
						Thread.Sleep(WaitForApply);
						AssertThatDtrPinIsFalse(terminalA.SerialPortControlPins);
						AssertThatDsrPinIsFalse(terminalB.SerialPortControlPins);

						Utilities.WaitForTransmissionAndAssertCounts(terminalB, terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);
						Utilities.AssertBidirContent(                terminalB, terminalA, expectedContentB, expectedContentA);

						// Switch DTR on:
						keyword = @"\!(DTROn)\!(NoEOL)";
						terminalA.SendTextLine(keyword); // Intentionally using Line() with \!(NoEOL) as this will be required for a keyword-only predefined command.
						Thread.Sleep(WaitForApply);
						AssertThatPinsAreTrue(terminalA.SerialPortControlPins);
						AssertThatPinsAreTrue(terminalB.SerialPortControlPins);

						Utilities.WaitForTransmissionAndAssertCounts(terminalB, terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);
						Utilities.AssertBidirContent(                terminalB, terminalA, expectedContentB, expectedContentA);

						// Switch DTR off:
						keyword = @"\!(DTROff)\!(NoEOL)";
						terminalA.SendTextLine(keyword); // Intentionally using Line() with \!(NoEOL) as this will be required for a keyword-only predefined command.
						Thread.Sleep(WaitForApply);
						AssertThatDtrPinIsFalse(terminalA.SerialPortControlPins);
						AssertThatDsrPinIsFalse(terminalB.SerialPortControlPins);

						Utilities.WaitForTransmissionAndAssertCounts(terminalB, terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);
						Utilities.AssertBidirContent(                terminalB, terminalA, expectedContentB, expectedContentA);

						// Toggle DTR on:
						keyword = @"\!(DTRToggle)\!(NoEOL)";
						terminalA.SendTextLine(keyword); // Intentionally using Line() with \!(NoEOL) as this will be required for a keyword-only predefined command.
						Thread.Sleep(WaitForApply);
						AssertThatPinsAreTrue(terminalA.SerialPortControlPins);
						AssertThatPinsAreTrue(terminalB.SerialPortControlPins);

						Utilities.WaitForTransmissionAndAssertCounts(terminalB, terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);
						Utilities.AssertBidirContent(                terminalB, terminalA, expectedContentB, expectedContentA);

						// Toggle DTR off in other terminal:
						keyword = @"\!(DTRToggle)\!(NoEOL)";
						terminalB.SendTextLine(keyword); // Intentionally using Line() with \!(NoEOL) as this will be required for a keyword-only predefined command.
						Thread.Sleep(WaitForApply);
						AssertThatDtrPinIsFalse(terminalB.SerialPortControlPins);
						AssertThatDsrPinIsFalse(terminalA.SerialPortControlPins);

						Utilities.WaitForTransmissionAndAssertCounts(terminalB, terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);
						Utilities.AssertBidirContent(                terminalB, terminalA, expectedContentB, expectedContentA);

						// Switch DTR on again in other terminal:
						keyword = @"\!(DTROn)\!(NoEOL)";
						terminalB.SendTextLine(keyword); // Intentionally using Line() with \!(NoEOL) as this will be required for a keyword-only predefined command.
						Thread.Sleep(WaitForApply);
						AssertThatPinsAreTrue(terminalA.SerialPortControlPins);
						AssertThatPinsAreTrue(terminalB.SerialPortControlPins);

						Utilities.WaitForTransmissionAndAssertCounts(terminalB, terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);
						Utilities.AssertBidirContent(                terminalB, terminalA, expectedContentB, expectedContentA);

						// Try to switch RTS off in other terminal:
						keyword = @"\!(RTSOff)\!(NoEOL)";
						terminalB.SendTextLine(keyword); // Intentionally using Line() with \!(NoEOL) as this will be required for a keyword-only predefined command.
						Thread.Sleep(WaitForApply);
						AssertThatPinsAreTrue(terminalA.SerialPortControlPins); // Change must be rejected as automatic flow control is active.
						AssertThatPinsAreTrue(terminalB.SerialPortControlPins);
						                       //// Escape the [ for Regex.
						var warningMessage = @"[Warning: Modifying the RTS signal is not possible when automatic hardware or RS-485 flow control is active.]";
						expectedContentB.Add(warningMessage);
						                                                                                                        //// Adjust for warning message.
						Utilities.WaitForSendingAndAssertCounts(  terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA + 1);
						Utilities.WaitForReceivingAndAssertCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

						Utilities.AssertBidirContent(terminalB, expectedContentB);
						Utilities.AssertBidirContent(terminalA, expectedContentA);

						// Wait to ensure that no operation is ongoing anymore and verify again:
						Utilities.WaitForReverification();
						Utilities.AssertTxCounts(terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
						Utilities.AssertRxCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);
						Utilities.AssertTxCounts(terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA + 1); // See above.
						Utilities.AssertRxCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

						Utilities.AssertBidirContent(terminalB, expectedContentB);
						Utilities.AssertBidirContent(terminalA, expectedContentA);

						// Refresh and verify again:
						terminalA.RefreshRepositories();
						terminalB.RefreshRepositories();

						Utilities.AssertTxCounts(terminalA, expectedTotalByteCountAB, expectedTotalLineCountAB);
						Utilities.AssertRxCounts(terminalB, expectedTotalByteCountAB, expectedTotalLineCountAB);
						Utilities.AssertTxCounts(terminalB, expectedTotalByteCountBA, expectedTotalLineCountBA); // Warning will disappear (pending fix of bug #211 "...indications disappear").
						Utilities.AssertRxCounts(terminalA, expectedTotalByteCountBA, expectedTotalLineCountBA);

						expectedContentB.Remove(warningMessage); // Warning will disappear (pending fix of bug #211 "...indications disappear").

						Utilities.AssertBidirContent(terminalB, expectedContentB);
						Utilities.AssertBidirContent(terminalA, expectedContentA);

						terminalB.Stop();
						Utilities.WaitForStop(terminalB);
					} // using (terminalB)

					terminalA.Stop();
					Utilities.WaitForStop(terminalA);
				} // using (terminalA)
			} // using (parser)
		}

		/// <summary></summary>
		protected virtual void AssertThatPinsAreTrue(MKY.IO.Ports.SerialPortControlPins pins)
		{
			Assert.That(pins.Rts, Is.True);
			Assert.That(pins.Cts, Is.True);
			Assert.That(pins.Dtr, Is.True);
			Assert.That(pins.Dsr, Is.True);
			Assert.That(pins.Dcd, Is.True); // = DSR
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rts", Justification = "'RTS' is a common term for serial ports.")]
		protected virtual void AssertThatRtsPinIsFalse(MKY.IO.Ports.SerialPortControlPins pins)
		{
			Assert.That(pins.Rts, Is.False);
			Assert.That(pins.Cts, Is.True);
			Assert.That(pins.Dtr, Is.True);
			Assert.That(pins.Dsr, Is.True);
			Assert.That(pins.Dcd, Is.True); // = DSR
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cts", Justification = "'CTS' is a common term for serial ports.")]
		protected virtual void AssertThatCtsPinIsFalse(MKY.IO.Ports.SerialPortControlPins pins)
		{
			Assert.That(pins.Rts, Is.True);
			Assert.That(pins.Cts, Is.False);
			Assert.That(pins.Dtr, Is.True);
			Assert.That(pins.Dsr, Is.True);
			Assert.That(pins.Dcd, Is.True); // = DSR
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dtr", Justification = "'DTR' is a common term for serial ports.")]
		protected virtual void AssertThatDtrPinIsFalse(MKY.IO.Ports.SerialPortControlPins pins)
		{
			Assert.That(pins.Rts, Is.True);
			Assert.That(pins.Cts, Is.True);
			Assert.That(pins.Dtr, Is.False);
			Assert.That(pins.Dsr, Is.True);
			Assert.That(pins.Dcd, Is.True); // = DSR
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dsr", Justification = "'DSR' is a common term for serial ports.")]
		protected virtual void AssertThatDsrPinIsFalse(MKY.IO.Ports.SerialPortControlPins pins)
		{
			Assert.That(pins.Rts, Is.True);
			Assert.That(pins.Cts, Is.True);
			Assert.That(pins.Dtr, Is.True);
			Assert.That(pins.Dsr, Is.False);
			Assert.That(pins.Dcd, Is.False); // = DSR
		}

		/// <remarks>
		/// Workaround to bug #354 "Automatic hardware flow control is not supported by MCT"
		/// for MCT based converters loopback COM1x in YAT TestLab.
		/// </remarks>
		protected virtual bool TestIsExcludedForMCT(TerminalSettings settings, IEnumerable<string> mctPortNamesInQuestion)
		{
			var mctPortCaptionInQuestion = MKY.IO.Ports.SerialPortId.CaptionMCT;

			foreach (var mctPortNameInQuestion in mctPortNamesInQuestion)
			{
				if (settings.IO.SerialPort.PortId == mctPortNameInQuestion)
				{
					var availablePorts = new MKY.IO.Ports.SerialPortCollection();
					availablePorts.FillWithAvailablePorts(true);

					var predicate = new MKY.IO.Ports.EqualsPortName<MKY.IO.Ports.SerialPortId>(mctPortNameInQuestion);
					var mctPortIdInQuestion = availablePorts.Find(predicate.Match);
					if (mctPortIdInQuestion.EqualsCaption(mctPortCaptionInQuestion))
					{
						System.Diagnostics.Trace.WriteLine(@"Test is exculded to work around bug #354 ""Automatic hardware flow control is not supported by MCT"".");
						return (true);
					}
				}
			}

			return (false);
		}


		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
