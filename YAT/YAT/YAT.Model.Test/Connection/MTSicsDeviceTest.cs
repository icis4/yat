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
// YAT Version 2.4.1
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2021 Matthias Kläy.
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
using System.Threading;

using MKY.Settings;
using MKY.Test.Devices;

using NUnit.Framework;
using NUnitEx;

using YAT.Settings.Application;

#endregion

namespace YAT.Model.Test.Connection
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "'MT-SICS' is a name.")]
	[TestFixture]
	public class MTSicsDeviceTest
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const string CommandToEcho = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwyxyz0123456789";

		#endregion

		#region TestFixture
		//==========================================================================================
		// TestFixture
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "SetUp", Justification = "Naming according to NUnit.")]
		[TestFixtureSetUp]
		public virtual void TestFixtureSetUp()
		{
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

		#region Tests > Close/Reopen
		//------------------------------------------------------------------------------------------
		// Tests > Close/Reopen
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// Test is optional, it can be excluded if no MT-SICS device is available.
		/// </remarks>
		[Test, MKY.IO.Ports.Test.MTSicsDeviceAIsAvailableCategory]
		public virtual void TestCloseReopen()
		{
			if (!MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceAIsAvailable)
				Assert.Ignore("'MTSicsDeviceA' is not available, therefore this test is excluded. Ensure that 'MTSicsDeviceA' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			var settings = Settings.GetMTSicsSerialPortDeviceASettings();

			// Create terminals from settings:
			using (var terminal = new Terminal(settings))
			{
				terminal.MessageInputRequest += Utilities.TerminalMessageInputRequest;
				if (!terminal.Start())
				{
					if (Utilities.TerminalMessageInputRequestResultsInExclude)
					{
						Assert.Ignore(Utilities.TerminalMessageInputRequestResultsInExcludeText);
					//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.
					}
					else
					{
						Assert.Fail(@"Failed to start """ + terminal.Caption + @"""");
					}
				}
				Utilities.WaitForOpen(terminal);

				const int WaitForOperation = 100;

				// --- Test: Close/Reopen without sending. -----------------------------------------

				// Close and reopen terminal. Expected: No exceptions, terminal can be closed and reopened.
				Assert.That(terminal.Stop(),         Is.True);
				Assert.That(terminal.IsStarted,      Is.False);
				Assert.That(terminal.IsOpen,         Is.False);
				Assert.That(terminal.IsTransmissive, Is.False);
				Thread.Sleep(WaitForOperation);
				Assert.That(terminal.Start(),        Is.True);
				Assert.That(terminal.IsStarted,      Is.True);
				Assert.That(terminal.IsOpen,         Is.True);
				Assert.That(terminal.IsTransmissive, Is.True);

				// --- Test: Close/Reopen with previous and subsequent sending. --------------------

				// Prepare stimulus and expected:
				Types.Command emptyCommand = new Types.Command("");
				var l = new List<byte>(4) // Preset the required capacity to improve memory management.
				{
					0x45, // 'E'
					0x53, // 'S'
					0x0D, // <CR>
					0x0A  // <LF>
				};
				byte[] emptyCommandExpected = l.ToArray();
				int expectedTotalLineCount = 0;
				int expectedTotalByteCount = 0;

				// Send something and verify response:
				terminal.SendText(emptyCommand);
				expectedTotalLineCount++;
				expectedTotalByteCount += emptyCommandExpected.Length;
				Utilities.WaitForReceivingAndAssertCounts(terminal, expectedTotalByteCount, expectedTotalLineCount);

				// Close and reopen terminal. Expected: No exceptions, terminal can be closed and reopened.
				Assert.That(terminal.Stop(),         Is.True);
				Assert.That(terminal.IsStarted,      Is.False);
				Assert.That(terminal.IsOpen,         Is.False);
				Assert.That(terminal.IsTransmissive, Is.False);
				Thread.Sleep(WaitForOperation);
				Assert.That(terminal.Start(),        Is.True);
				Assert.That(terminal.IsStarted,      Is.True);
				Assert.That(terminal.IsOpen,         Is.True);
				Assert.That(terminal.IsTransmissive, Is.True);

				// Send something and verify response:
				terminal.SendText(emptyCommand);
				expectedTotalLineCount++;
				expectedTotalByteCount += emptyCommandExpected.Length;
				Utilities.WaitForReceivingAndAssertCounts(terminal, expectedTotalByteCount, expectedTotalLineCount);

				// --- Test: Close/Reopen while continuous receiving. ----------------------------------

				// Request continuous data:
				terminal.SendText("ECHO 2"); // Activate continuous echo mode.
				Thread.Sleep(WaitForOperation);
				terminal.SendText(CommandToEcho); // Request continuous echo.
				Thread.Sleep(WaitForOperation);

				// Close and reopen terminal. Expected: No exceptions, terminal can be closed and reopened.
				Assert.That(terminal.Stop(),         Is.True);
				Assert.That(terminal.IsStarted,      Is.False);
				Assert.That(terminal.IsOpen,         Is.False);
				Assert.That(terminal.IsTransmissive, Is.False);
				Thread.Sleep(WaitForOperation);
				Assert.That(terminal.Start(),        Is.True);
				Assert.That(terminal.IsStarted,      Is.True);
				Assert.That(terminal.IsOpen,         Is.True);
				Assert.That(terminal.IsTransmissive, Is.True);

				// Stop continuous data:
				terminal.SendText("<ESC>"); // <ESC> to quit ECHO mode.
				Thread.Sleep(WaitForOperation);

				// Close terminal. Expected: No exceptions, terminal can be closed.
				Assert.That(terminal.Stop(),         Is.True);
				Assert.That(terminal.IsStarted,      Is.False);
				Assert.That(terminal.IsOpen,         Is.False);
				Assert.That(terminal.IsTransmissive, Is.False);
			} // Expected: No exceptions, terminal can be disposed.
		}

		#endregion

		#region Tests > Disconnect/Reconnect
		//------------------------------------------------------------------------------------------
		// Tests > Disconnect/Reconnect
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// Test is optional, it can be excluded if either MT-SICS device or USB hub is not available.
		/// </remarks>
		/// <remarks>
		/// So far, the USB hub and USB port assignment is hard-coded, could become configurable.
		/// </remarks>
		[Test, MKY.IO.Ports.Test.MTSicsDeviceAIsAvailableCategory, MKY.Test.UsbHub2IsAvailableCategory, StandardDurationCategory.Minute1]
		public virtual void TestDisconnectReconnect()
		{
			TestDisconnectReconnect(false); // See comments in MKY.IO.Ports.Test.TestDisconnectReconnect().
		}

		/// <remarks>
		/// Test is optional, it can be excluded if either MT-SICS device or USB hub is not available.
		/// </remarks>
		/// <remarks>
		/// So far, the USB hub and USB port assignment is hard-coded, could become configurable.
		/// </remarks>
		[Test, MKY.IO.Ports.Test.MTSicsDeviceAIsAvailableCategory, MKY.Test.UsbHub2IsAvailableCategory, StandardDurationCategory.Minute1Attribute, Explicit("This test requires to manually reset the sending device beause it will remain in continuous mode as well as the port device because it cannot be opened until disconnected/reconnected!")]
		public virtual void TestDisconnectReconnectWithContinuousReceiving()
		{
			TestDisconnectReconnect(true); // See comments in MKY.IO.Ports.Test.TestDisconnectReconnect().
		}

		/// <remarks>
		/// There is a similar method in 'MKY.IO.Ports.Test.SerialPort.ConnectionTest'.
		/// Changes here may have to be applied there too.
		///
		/// Tests implemented below:
		///  - Disconnect/Reconnect without sending.
		///  - Disconnect/Reconnect with previous and subsequent sending.
		///  - Disconnect/Reconnect while continuous receiving.
		///  - Disconnect, then manually close.
		///
		/// Tests that should be added in addition:
		///  - Disconnect, then try to send something.
		///     => Either, terminal must detect disconnect (FTDI, MCT, Prolific, ToriLogic/Thesycon).
		///     => Or, terminal must properly handle the exception that happens when sending on no longer available port (Microsoft, Microchip).
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Thesycon' is a brand name.")]
		private static void TestDisconnectReconnect(bool testWithContinuousReceiving)
		{
			if (!MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceAIsAvailable)
				Assert.Ignore("'MTSicsDeviceA' is not available, therefore this test is excluded. Ensure that 'MTSicsDeviceA' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			if (!MKY.Test.ConfigurationProvider.Configuration.UsbHub2IsAvailable)
				Assert.Ignore(UsbHubControl.ErrorMessage);
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			var portOut = UsbHubSettings.Out4;

			// --- Precondition: USB hub is set to its defaults, i.e. all outputs are enabled. -----

			var settings = Settings.GetMTSicsSerialPortDeviceASettings();

			// Create terminals from settings:
			using (var terminal = new Terminal(settings))
			{
				terminal.MessageInputRequest += Utilities.TerminalMessageInputRequest;
				if (!terminal.Start())
				{
					if (Utilities.TerminalMessageInputRequestResultsInExclude)
					{
						Assert.Ignore(Utilities.TerminalMessageInputRequestResultsInExcludeText);
					//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.
					}
					else
					{
						Assert.Fail(@"Failed to start """ + terminal.Caption + @"""");
					}
				}
				Utilities.WaitForOpen(terminal);

				const int WaitForOperation = 100;

				// --- Test: Disconnect/Reconnect without sending. ---------------------------------

				// Disconnect USB/RS-232 converter. Expected: No exceptions, terminal is closed:
				Assert.That(UsbHubControl.Set(UsbHubDevices.Hub2, UsbHubSettings.None), Is.True, "Failed to change USB hub configuration!"); // Disabling all outputs is used to improve speed when enabling single outputs below. See comments in implementation of 'UsbHubControl' for explanation.
				Assert.That(terminal.IsStarted, Is.True); // Terminal still started, and must automatically close!
				Utilities.WaitForClose(terminal);
				Assert.That(terminal.IsOpen,         Is.False);
				Assert.That(terminal.IsTransmissive, Is.False);

				// Reconnect USB/RS-232 converter. Expected: No exceptions, terminal can be reopened.
				Assert.That(UsbHubControl.Enable(UsbHubDevices.Hub2, portOut), Is.True, "Failed to change USB hub configuration!");
				Assert.That(terminal.IsStarted, Is.True); // Terminal still started, and must automatically reopen!
				Utilities.WaitForOpen(terminal);
				Assert.That(terminal.IsOpen,         Is.True);
				Assert.That(terminal.IsTransmissive, Is.True);

				// Verify that subsequently calling Start() also works:
				Assert.That(terminal.Start(),        Is.True);
				Assert.That(terminal.IsStarted,      Is.True);
				Assert.That(terminal.IsOpen,         Is.True);
				Assert.That(terminal.IsTransmissive, Is.True);

				// Close and reopen terminal. Expected: No exceptions, terminal can be closed and reopened.
				Thread.Sleep(WaitForOperation);
				Assert.That(terminal.Stop(),         Is.True);
				Assert.That(terminal.IsStarted,      Is.False);
				Assert.That(terminal.IsOpen,         Is.False);
				Assert.That(terminal.IsTransmissive, Is.False);

				Thread.Sleep(WaitForOperation);
				Assert.That(terminal.Start(),        Is.True);
				Assert.That(terminal.IsStarted,      Is.True);
				Assert.That(terminal.IsOpen,         Is.True);
				Assert.That(terminal.IsTransmissive, Is.True);

				// --- Test: Disconnect/Reconnect with previous and subsequent sending. ------------

				// Prepare stimulus and expected:
				var emptyCommand = new Types.Command("");
				var l = new List<byte>(4) // Preset the required capacity to improve memory management.
				{
					0x45, // 'E'
					0x53, // 'S'
					0x0D, // <CR>
					0x0A  // <LF>
				};
				var emptyCommandExpected = l.ToArray();
				int expectedTotalRxLineCount = 0;
				int expectedTotalRxByteCount = 0;

				// Send something and verify response:
				terminal.SendText(emptyCommand);
				expectedTotalRxLineCount++;
				expectedTotalRxByteCount += emptyCommandExpected.Length;
				Utilities.WaitForReceivingAndAssertCounts(terminal, expectedTotalRxByteCount, expectedTotalRxLineCount);

				// Disconnect USB/RS-232 converter. Expected: No exceptions, terminal is closed:
				Assert.That(UsbHubControl.Disable(UsbHubDevices.Hub2, portOut), Is.True, "Failed to change USB hub configuration!");
				Assert.That(terminal.IsStarted, Is.True); // Terminal still started, and must automatically close!
				Utilities.WaitForClose(terminal);
				Assert.That(terminal.IsOpen,         Is.False);
				Assert.That(terminal.IsTransmissive, Is.False);

				// Reconnect USB/RS-232 converter. Expected: No exceptions, terminal can be reopened.
				Assert.That(UsbHubControl.Enable(UsbHubDevices.Hub2, portOut), Is.True, "Failed to change USB hub configuration!");
				Assert.That(terminal.IsStarted, Is.True); // Terminal still started, and must automatically reopen!
				Utilities.WaitForOpen(terminal);
				Assert.That(terminal.IsOpen,         Is.True);
				Assert.That(terminal.IsTransmissive, Is.True);

				// Send something and verify response:
				terminal.SendText(emptyCommand);
				expectedTotalRxLineCount++;
				expectedTotalRxByteCount += emptyCommandExpected.Length;
				Utilities.WaitForReceivingAndAssertCounts(terminal, expectedTotalRxByteCount, expectedTotalRxLineCount);

				// Verify that subsequently calling Start() also works:
				Assert.That(terminal.Start(),        Is.True);
				Assert.That(terminal.IsStarted,      Is.True);
				Assert.That(terminal.IsOpen,         Is.True);
				Assert.That(terminal.IsTransmissive, Is.True);

				// Send something and verify response:
				terminal.SendText(emptyCommand);
				expectedTotalRxLineCount++;
				expectedTotalRxByteCount += emptyCommandExpected.Length;
				Utilities.WaitForReceivingAndAssertCounts(terminal, expectedTotalRxByteCount, expectedTotalRxLineCount);

				// Close and reopen terminal. Expected: No exceptions, terminal can be closed and reopened.
				Assert.That(terminal.Stop(),         Is.True);
				Assert.That(terminal.IsStarted,      Is.False);
				Assert.That(terminal.IsOpen,         Is.False);
				Assert.That(terminal.IsTransmissive, Is.False);
				Thread.Sleep(WaitForOperation);

				Assert.That(terminal.Start(),        Is.True);
				Assert.That(terminal.IsStarted,      Is.True);
				Assert.That(terminal.IsOpen,         Is.True);
				Assert.That(terminal.IsTransmissive, Is.True);
				Thread.Sleep(WaitForOperation);

				// Send something and verify response:
				terminal.SendText(emptyCommand);
				expectedTotalRxLineCount++;
				expectedTotalRxByteCount += emptyCommandExpected.Length;
				Utilities.WaitForReceivingAndAssertCounts(terminal, expectedTotalRxByteCount, expectedTotalRxLineCount);

				// --- Test: Disconnect/Reconnect while continuous receiving. ----------------------

				if (testWithContinuousReceiving) // See comments in MKY.IO.Ports.Test.TestDisconnectReconnect().
				{
					// Request continuous data:
					terminal.SendText("ECHO 2"); // Activate continuous echo mode.
					Thread.Sleep(WaitForOperation);
					terminal.SendText(CommandToEcho); // Request continuous echo.
					Thread.Sleep(WaitForOperation);

					// Disconnect USB/RS-232 converter. Expected: No exceptions, terminal is closed:
					Assert.That(UsbHubControl.Disable(UsbHubDevices.Hub2, portOut), Is.True, "Failed to change USB hub configuration!");
					Assert.That(terminal.IsStarted, Is.True); // Terminal still started, and must automatically close!
					Utilities.WaitForClose(terminal);
					Assert.That(terminal.IsOpen,         Is.False);
					Assert.That(terminal.IsTransmissive, Is.False);

					// \remind: The underlying port should be closed here. However, this doesn't
					// work due to the issue documented in the header of 'SerialPortEx'. Still, a
					// YAT terminal shall handle this situation without any exceptions!

					// Reconnect USB/RS-232 converter. Expected: No exceptions, terminal can be reopened.
					Assert.That(UsbHubControl.Enable(UsbHubDevices.Hub2, portOut), Is.True, "Failed to change USB hub configuration!");
					Assert.That(terminal.IsStarted, Is.True); // Terminal still started, and must automatically reopen!
					Utilities.WaitForOpen(terminal);
					Assert.That(terminal.IsOpen,         Is.True);
					Assert.That(terminal.IsTransmissive, Is.True);

					// Stop continuous data:
					terminal.SendText("<ESC>"); // <ESC> to quit ECHO mode.
					Thread.Sleep(WaitForOperation);
				}

				// --- Test: Disconnect, then manually close. --------------------------------------

				// Disconnect USB/RS-232 converter. Expected: No exceptions, terminal is closed:
				Assert.That(UsbHubControl.Disable(UsbHubDevices.Hub2, portOut), Is.True, "Failed to change USB hub configuration!");
				Assert.That(terminal.IsStarted, Is.True); // Terminal still started, and must automatically close!
				Utilities.WaitForClose(terminal);
				Assert.That(terminal.IsOpen,         Is.False);
				Assert.That(terminal.IsTransmissive, Is.False);

				// Manually close terminal. Expected: No exceptions, terminal can be closed.
				Assert.That(terminal.Stop(),         Is.True);
				Assert.That(terminal.IsStarted,      Is.False);
				Assert.That(terminal.IsOpen,         Is.False);
				Assert.That(terminal.IsTransmissive, Is.False);

				// Reconnect USB/RS-232 converter. Expected: No exceptions, terminal can be reopened.
				Assert.That(UsbHubControl.Enable(UsbHubDevices.Hub2, portOut), Is.True, "Failed to change USB hub configuration!");
				Assert.That(terminal.IsStarted,      Is.False);

				// Manually open terminal again. Expected: No exceptions, terminal can be opened.
				Thread.Sleep(WaitForOperation);
				Assert.That(terminal.Start(),        Is.True);
				Utilities.WaitForOpen(terminal);
				Assert.That(terminal.IsStarted,      Is.True);
				Assert.That(terminal.IsOpen,         Is.True);
				Assert.That(terminal.IsTransmissive, Is.True);

				// Close terminal. Expected: No exceptions, terminal can be closed.
				Thread.Sleep(WaitForOperation);
				Assert.That(terminal.Stop(),         Is.True);
				Assert.That(terminal.IsStarted,      Is.False);
				Assert.That(terminal.IsOpen,         Is.False);
				Assert.That(terminal.IsTransmissive, Is.False);
			} // Expected: No exceptions, terminal can be disposed.

			// --- Postcondition: USB hub is set to its defaults, i.e. all outputs are enabled. ----

			Assert.That(UsbHubControl.Set(UsbHubDevices.Hub2, UsbHubSettings.All), Is.True, "Failed to set USB hub!");
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
