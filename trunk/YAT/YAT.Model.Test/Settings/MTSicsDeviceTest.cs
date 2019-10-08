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
// YAT Version 2.3.90 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2019 Matthias Kläy.
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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Windows.Forms;

using MKY.Collections.Generic;
using MKY.Settings;
using MKY.Windows.Forms;

using NUnit.Framework;

using YAT.Settings.Application;

#endregion

namespace YAT.Model.Test.Settings
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "MT-SICS is a name.")]
	public static class MTSicsDeviceTestData
	{
		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		public static IEnumerable SerialPortTestCases
		{
			get
			{
				foreach (var dev in Utilities.MTSicsSerialPortDevices)
				{
					// Arguments:
					var tcd = new TestCaseData(dev.Value1); // The settings descriptor delegate.

					// Category:
					tcd.SetCategory(dev.Value2); // Set device category.

					// Name:
					tcd.SetName(dev.Value3); // Set device indicator.

					yield return (tcd);
				}
			}
		}

		#endregion
	}

	/// <remarks>
	/// It can be argued that this test would be better located in YAT.Domain.Test. It currently is
	/// located here same as the similar <see cref="Transmission.MTSicsDeviceTest"/>.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "MT-SICS is a name.")]
	[TestFixture]
	public class MTSicsDeviceTest
	{
		#region TestFixture
		//==========================================================================================
		// TestFixture
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Don't care, it's for debugging only...")]
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "SetUp", Justification = "Naming according to NUnit.")]
		[TestFixtureSetUp]
		public virtual void TestFixtureSetUp()
		{
			// \remind (2016-05-26 / MKY) should be guarded by if (isRunningFromGui) to prevent the message box in case of automatic test runs.
			// \remind (2017-10-09 / MKY) even better to be replaced by a runtime check for availability of a weighing capable MT-SICS device.
			var dr = MessageBoxEx.Show
			(
				"This test requires a weighing MT-SICS device:" + Environment.NewLine +
				" > Serial COM Port: Start WeightSim if no load cell connected." + Environment.NewLine +
				" > TCP/IP Socket: Start device simulation." + Environment.NewLine +
				" > USB Ser/HID: Start WeightSim if no load cell connected.",
				"Precondition",
				MessageBoxButtons.OKCancel,
				MessageBoxIcon.Information
			);

			if (dr != DialogResult.OK)
				Assert.Ignore("Tester has canceled");

			// Create temporary in-memory application settings for this test run:
			ApplicationSettings.Create(ApplicationSettingsFileAccess.None);

			// Prevent auto-save of workspace settings:
			ApplicationSettings.LocalUserSettings.General.AutoSaveWorkspace = false;
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TearDown", Justification = "Naming according to NUnit.")]
		[TestFixtureTearDown]
		public virtual void TestFixtureTearDown()
		{
			// Close and dispose of temporary in-memory application settings:
			ApplicationSettings.CloseAndDispose();
		}

		#endregion

		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Don't care, straightforward test implementation.")]
		[Test, TestCaseSource(typeof(MTSicsDeviceTestData), "SerialPortTestCases")]
		[Ignore("Test sequence in principle works, but verification of expected lengths doesn't due to XOn/XOff and timing restrictions => to be done when integrating scripting.")]
		public virtual void SerialPortCommunicationSettings(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptor)
		{
			var settings = settingsDescriptor.Value1(settingsDescriptor.Value2);

			// Create terminals from settings:
			using (var terminal = new Terminal(settings))
			{
				terminal.MessageInputRequest += Utilities.TerminalMessageInputRequest;
				if (!terminal.Start())
				{
					if (Utilities.TerminalMessageInputRequestResultsInExclude) {
						Assert.Ignore(Utilities.TerminalMessageInputRequestResultsInExcludeText);
					//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.
					}
					else {
						Assert.Fail(@"Failed to start """ + terminal.Caption + @"""");
					}
				}
				Utilities.WaitForOpen(terminal);

				// Execute test sequence:
				int expectedTotalByteCount = 0;
				int expectedTotalLineCount = 0;

				var send     = "COM 0"; // Ensure that device is using default settings!
				var expected = "COM A 0 6 3 1";

				Trace.WriteLine(@">> """ + send + @"""");
				terminal.SendText(send);                      // EOL
				expectedTotalByteCount += expected.Length + 2;
				expectedTotalLineCount += 1;
				Utilities.WaitForReceivingAndVerifyCounts(terminal, expectedTotalByteCount, expectedTotalLineCount);

				send     = @"COM 0 7 3 1"; // Request change to 19200 baud.
				expected =  "COM A";       // Still expected at 9600 baud.

				Trace.WriteLine(@">> """ + send + @"""");
				terminal.SendText(send);                      // EOL
				expectedTotalByteCount += expected.Length + 2 + 1; // \remind (2018-07-28 / MKY) additional <XOn> is received on MCT, though that should be consumed/hidden as XOn/XOff is active...
				expectedTotalLineCount += 1;
				Utilities.WaitForReceivingAndVerifyCounts(terminal, expectedTotalByteCount, expectedTotalLineCount);

				Thread.Sleep(500);

				send     = @"\!(Baud(19200))COM 0"; // \remind (2018-06-20 / MKY) can be migrated to -1 after upgrade of test boards
				expected =  "COM A 0 7 3 1";        // Now expected at 19200 baud.

				Trace.WriteLine(@">> """ + send + @"""");
				terminal.SendText(send);                      // EOL
				expectedTotalByteCount += expected.Length + 2;
				expectedTotalLineCount += 1;
				Utilities.WaitForReceivingAndVerifyCounts(terminal, expectedTotalByteCount, expectedTotalLineCount);

				send     = @"COM 0 6 0 1"; // Request change to 9600/7/E.
				expected =  "COM A";       // Still expected at 19200 baud.

				Trace.WriteLine(@">> """ + send + @"""");
				terminal.SendText(send);                      // EOL
				expectedTotalByteCount += expected.Length + 2 + 1; // \remind (2018-07-28 / MKY) additional <XOn> is received on MCT, though that should be consumed/hidden as XOn/XOff is active...
				expectedTotalLineCount += 1;
				Utilities.WaitForReceivingAndVerifyCounts(terminal, expectedTotalByteCount, expectedTotalLineCount);

				Thread.Sleep(500);
				                                    //// 2 = Even
				send     = @"\!(PortSettings(9600|7|2))COM 0"; // \remind (2018-06-20 / MKY) can be migrated to -1 after upgrade of test boards
				expected =  "COM A 0 6 0 1";                   // Now expected at 9600/7/E.

				Trace.WriteLine(@">> """ + send + @"""");
				terminal.SendText(send);                      // EOL
				expectedTotalByteCount += expected.Length + 2;
				expectedTotalLineCount += 1;
				Utilities.WaitForReceivingAndVerifyCounts(terminal, expectedTotalByteCount, expectedTotalLineCount);

				send     = @"COM 0 6 3 1"; // Request revert to defaults of 9600/8/N.
				expected =  "COM A";       // Still expected at 9600/7/E.

				Trace.WriteLine(@">> """ + send + @"""");
				terminal.SendText(send);                      // EOL
				expectedTotalByteCount += expected.Length + 2 + 1; // \remind (2018-07-28 / MKY) additional <XOn> is received on MCT, though that should be consumed/hidden as XOn/XOff is active...
				expectedTotalLineCount += 1;
				Utilities.WaitForReceivingAndVerifyCounts(terminal, expectedTotalByteCount, expectedTotalLineCount);

				Thread.Sleep(500);
				                                      //// 0 = None
				send     = @"\!(DataBits(8))\!(Parity(0))COM 0"; // \remind (2018-06-20 / MKY) can be migrated to -1 after upgrade of test boards
				expected =  "COM A 0 6 3 1";                     // Now expected at 9600/8/N.

				Trace.WriteLine(@">> """ + send + @"""");
				terminal.SendText(send);                      // EOL
				expectedTotalByteCount += expected.Length + 2;
				expectedTotalLineCount += 2;
				Utilities.WaitForReceivingAndVerifyCounts(terminal, expectedTotalByteCount, expectedTotalLineCount);
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
