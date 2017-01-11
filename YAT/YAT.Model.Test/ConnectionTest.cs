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
// YAT 2.0 Gamma 2'' Version 1.99.52
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2017 Matthias Kläy.
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

using MKY.IO.Ports.Test;
using MKY.Settings;

using NUnit;
using NUnit.Framework;

using YAT.Settings.Application;
using YAT.Settings.Terminal;

#endregion

namespace YAT.Model.Test
{
	/// <summary></summary>
	[TestFixture]
	public class ConnectionTest
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

		/// <summary></summary>
		[Test, MTSicsDeviceAIsConnectedCategory]
		public virtual void TestCloseReopenSerialPort()
		{
			if (!ConfigurationProvider.Configuration.MTSicsDeviceAIsConnected)
				Assert.Ignore("'MTSicsDeviceA' is not connected, therefore this test is excluded. Ensure that 'MTSicsDeviceA' is properly configured and available if passing this test is required.");
				//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			TerminalSettingsRoot settings = Utilities.GetStartedTextSerialPortMTSicsDeviceASettings();

			// Create terminals from settings:
			using (Terminal terminal = new Terminal(settings))
			{
				terminal.MessageInputRequest += Utilities.TerminalMessageInputRequest;
				if (!terminal.StartIO())
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
				Assert.IsTrue(terminal.StopIO());
				Assert.IsFalse(terminal.IsStarted);
				Assert.IsFalse(terminal.IsOpen);
				Assert.IsFalse(terminal.IsReadyToSend);
				Thread.Sleep(WaitForOperation);
				Assert.IsTrue(terminal.StartIO());
				Assert.IsTrue(terminal.IsStarted);
				Assert.IsTrue(terminal.IsOpen);
				Assert.IsTrue(terminal.IsReadyToSend);

				// --- Test: Close/Reopen with previous and subsequent sending. --------------------

				// Prepare stimulus and expected:
				Types.Command emptyCommand = new Types.Command("");
				List<byte> l = new List<byte>(4); // Preset the required capacity to improve memory management.
				l.Add(0x45); // 'E'
				l.Add(0x53); // 'S'
				l.Add(0x0D); // <CR>
				l.Add(0x0A); // <LF>
				byte[] emptyCommandExpected = l.ToArray();
				int expectedTotalLineCount = 0;
				int expectedTotalByteCount = 0;

				// Send something and verify response:
				terminal.SendText(emptyCommand);
				expectedTotalLineCount++;
				expectedTotalByteCount += emptyCommandExpected.Length;
				Utilities.WaitForReceiving(terminal, expectedTotalLineCount, expectedTotalByteCount);

				// Close and reopen terminal. Expected: No exceptions, terminal can be closed and reopened.
				Assert.IsTrue(terminal.StopIO());
				Assert.IsFalse(terminal.IsStarted);
				Assert.IsFalse(terminal.IsOpen);
				Assert.IsFalse(terminal.IsReadyToSend);
				Thread.Sleep(WaitForOperation);
				Assert.IsTrue(terminal.StartIO());
				Assert.IsTrue(terminal.IsStarted);
				Assert.IsTrue(terminal.IsOpen);
				Assert.IsTrue(terminal.IsReadyToSend);

				// Send something and verify response:
				terminal.SendText(emptyCommand);
				expectedTotalLineCount++;
				expectedTotalByteCount += emptyCommandExpected.Length;
				Utilities.WaitForReceiving(terminal, expectedTotalLineCount, expectedTotalByteCount);

				// --- Test: Close/Reopen while continuous receiving. ----------------------------------

				// Request continuous data:
				terminal.SendText(new Types.Command("ECHO 2")); // Activate continuous echo mode.
				Thread.Sleep(WaitForOperation);
				terminal.SendText(new Types.Command(CommandToEcho)); // Request continuous echo.
				Thread.Sleep(WaitForOperation);

				// Close and reopen terminal. Expected: No exceptions, terminal can be closed and reopened.
				Assert.IsTrue(terminal.StopIO());
				Assert.IsFalse(terminal.IsStarted);
				Assert.IsFalse(terminal.IsOpen);
				Assert.IsFalse(terminal.IsReadyToSend);
				Thread.Sleep(WaitForOperation);
				Assert.IsTrue(terminal.StartIO());
				Assert.IsTrue(terminal.IsStarted);
				Assert.IsTrue(terminal.IsOpen);
				Assert.IsTrue(terminal.IsReadyToSend);

				// Stop continuous data:
				terminal.SendText(new Types.Command("<ESC>")); // <ESC> to quit ECHO mode.
				Thread.Sleep(WaitForOperation);

				// Close terminal. Expected: No exceptions, terminal can be closed.
				Assert.IsTrue(terminal.StopIO());
				Assert.IsFalse(terminal.IsStarted);
				Assert.IsFalse(terminal.IsOpen);
				Assert.IsFalse(terminal.IsReadyToSend);
			} // Expected: No exceptions, terminal can be disposed.
		}

		#endregion

		#region Tests > Disconnect/Reconnect
		//------------------------------------------------------------------------------------------
		// Tests > Disconnect/Reconnect
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, MTSicsDeviceAIsConnectedCategory, MinuteDurationCategory(1)]
		public virtual void TestDisconnectReconnectSerialPort()
		{
			TestDisconnectReconnect(false); // See comments in MKY.IO.Ports.Test.TestDisconnectReconnect().
		}

		/// <summary></summary>
		[Test, MTSicsDeviceAIsConnectedCategory, MinuteDurationCategory(1), Explicit("This test requires to manually reset the sending device beause it will remain in continuous mode as well as the port device because it cannot be opened until disconnected/reconnected!")]
		public virtual void TestDisconnectReconnectSerialPortWithContinuousReceiving()
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
			if (!ConfigurationProvider.Configuration.MTSicsDeviceAIsConnected)
				Assert.Ignore("'MTSicsDeviceA' is not connected, therefore this test is excluded. Ensure that 'MTSicsDeviceA' is properly configured and available if passing this test is required.");
				//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			if (!UsbHubControl.Probe())
				Assert.Ignore(UsbHubControl.ErrorMessage);
				//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			UsbHubSettings portOut = UsbHubSettings.Out4;

			// --- Precondition: USB hub is set to its defaults, i.e. all outputs are enabled. -----

			TerminalSettingsRoot settings = Utilities.GetStartedTextSerialPortMTSicsDeviceASettings();

			// Create terminals from settings:
			using (Terminal terminal = new Terminal(settings))
			{
				terminal.MessageInputRequest += Utilities.TerminalMessageInputRequest;
				if (!terminal.StartIO())
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
				Assert.IsTrue(UsbHubControl.Set(UsbHubSettings.None), "Failed to modify USB hub!"); // Disabling all outputs is used to improve speed when enabling single outputs below. See comments in implementation of 'UsbHubControl' for explanation.
				Assert.IsTrue(terminal.IsStarted); // Terminal still started, and must automatically close!
				Utilities.WaitForClose(terminal);
				Assert.IsFalse(terminal.IsOpen);
				Assert.IsFalse(terminal.IsReadyToSend);

				// Reconnect USB/RS-232 converter. Expected: No exceptions, terminal can be reopened.
				Assert.IsTrue(UsbHubControl.Enable(portOut), "Failed to modify USB hub!");
				Assert.IsTrue(terminal.IsStarted); // Terminal still started, and must automatically reopen!
				Utilities.WaitForOpen(terminal);
				Assert.IsTrue(terminal.IsOpen);
				Assert.IsTrue(terminal.IsReadyToSend);

				// Verify that subsequently calling StartIO() also works:
				Assert.IsTrue(terminal.StartIO());
				Assert.IsTrue(terminal.IsStarted);
				Assert.IsTrue(terminal.IsOpen);
				Assert.IsTrue(terminal.IsReadyToSend);

				// Close and reopen terminal. Expected: No exceptions, terminal can be closed and reopened.
				Thread.Sleep(WaitForOperation);
				Assert.IsTrue(terminal.StopIO());
				Assert.IsFalse(terminal.IsStarted);
				Assert.IsFalse(terminal.IsOpen);
				Assert.IsFalse(terminal.IsReadyToSend);

				Thread.Sleep(WaitForOperation);
				Assert.IsTrue(terminal.StartIO());
				Assert.IsTrue(terminal.IsStarted);
				Assert.IsTrue(terminal.IsOpen);
				Assert.IsTrue(terminal.IsReadyToSend);

				// --- Test: Disconnect/Reconnect with previous and subsequent sending. ------------

				// Prepare stimulus and expected:
				Types.Command emptyCommand = new Types.Command("");
				List<byte> l = new List<byte>(4); // Preset the required capacity to improve memory management.
				l.Add(0x45); // 'E'
				l.Add(0x53); // 'S'
				l.Add(0x0D); // <CR>
				l.Add(0x0A); // <LF>
				byte[] emptyCommandExpected = l.ToArray();
				int expectedTotalLineCount = 0;
				int expectedTotalByteCount = 0;

				// Send something and verify response:
				terminal.SendText(emptyCommand);
				expectedTotalLineCount++;
				expectedTotalByteCount += emptyCommandExpected.Length;
				Utilities.WaitForReceiving(terminal, expectedTotalLineCount, expectedTotalByteCount);

				// Disconnect USB/RS-232 converter. Expected: No exceptions, terminal is closed:
				Assert.IsTrue(UsbHubControl.Disable(portOut), "Failed to modify USB hub!");
				Assert.IsTrue(terminal.IsStarted); // Terminal still started, and must automatically close!
				Utilities.WaitForClose(terminal);
				Assert.IsFalse(terminal.IsOpen);
				Assert.IsFalse(terminal.IsReadyToSend);

				// Reconnect USB/RS-232 converter. Expected: No exceptions, terminal can be reopened.
				Assert.IsTrue(UsbHubControl.Enable(portOut), "Failed to modify USB hub!");
				Assert.IsTrue(terminal.IsStarted); // Terminal still started, and must automatically reopen!
				Utilities.WaitForOpen(terminal);
				Assert.IsTrue(terminal.IsOpen);
				Assert.IsTrue(terminal.IsReadyToSend);

				// Send something and verify response:
				terminal.SendText(emptyCommand);
				expectedTotalLineCount++;
				expectedTotalByteCount += emptyCommandExpected.Length;
				Utilities.WaitForReceiving(terminal, expectedTotalLineCount, expectedTotalByteCount);

				// Verify that subsequently calling StartIO() also works:
				Assert.IsTrue(terminal.StartIO());
				Assert.IsTrue(terminal.IsStarted);
				Assert.IsTrue(terminal.IsOpen);
				Assert.IsTrue(terminal.IsReadyToSend);

				// Send something and verify response:
				terminal.SendText(emptyCommand);
				expectedTotalLineCount++;
				expectedTotalByteCount += emptyCommandExpected.Length;
				Utilities.WaitForReceiving(terminal, expectedTotalLineCount, expectedTotalByteCount);

				// Close and reopen terminal. Expected: No exceptions, terminal can be closed and reopened.
				Assert.IsTrue(terminal.StopIO());
				Assert.IsFalse(terminal.IsStarted);
				Assert.IsFalse(terminal.IsOpen);
				Assert.IsFalse(terminal.IsReadyToSend);
				Thread.Sleep(WaitForOperation);

				Assert.IsTrue(terminal.StartIO());
				Assert.IsTrue(terminal.IsStarted);
				Assert.IsTrue(terminal.IsOpen);
				Assert.IsTrue(terminal.IsReadyToSend);
				Thread.Sleep(WaitForOperation);

				// Send something and verify response:
				terminal.SendText(emptyCommand);
				expectedTotalLineCount++;
				expectedTotalByteCount += emptyCommandExpected.Length;
				Utilities.WaitForReceiving(terminal, expectedTotalLineCount, expectedTotalByteCount);

				// --- Test: Disconnect/Reconnect while continuous receiving. ----------------------

				if (testWithContinuousReceiving) // See comments in MKY.IO.Ports.Test.TestDisconnectReconnect().
				{
					// Request continuous data:
					terminal.SendText(new Types.Command("ECHO 2")); // Activate continuous echo mode.
					Thread.Sleep(WaitForOperation);
					terminal.SendText(new Types.Command(CommandToEcho)); // Request continuous echo.
					Thread.Sleep(WaitForOperation);

					// Disconnect USB/RS-232 converter. Expected: No exceptions, terminal is closed:
					Assert.IsTrue(UsbHubControl.Disable(portOut), "Failed to modify USB hub!");
					Assert.IsTrue(terminal.IsStarted); // Terminal still started, and must automatically close!
					Utilities.WaitForClose(terminal);
					Assert.IsFalse(terminal.IsOpen);
					Assert.IsFalse(terminal.IsReadyToSend);

					// \Remind: The underlying port should be closed here. However, this doesn't
					// work due to the issue documented in the header of 'SerialPortEx'. Still, a
					// YAT terminal shall handle this situation without any exceptions!

					// Reconnect USB/RS-232 converter. Expected: No exceptions, terminal can be reopened.
					Assert.IsTrue(UsbHubControl.Enable(portOut), "Failed to modify USB hub!");
					Assert.IsTrue(terminal.IsStarted); // Terminal still started, and must automatically reopen!
					Utilities.WaitForOpen(terminal);
					Assert.IsTrue(terminal.IsOpen);
					Assert.IsTrue(terminal.IsReadyToSend);

					// Stop continuous data:
					terminal.SendText(new Types.Command("<ESC>")); // <ESC> to quit ECHO mode.
					Thread.Sleep(WaitForOperation);
				}

				// --- Test: Disconnect, then manually close. --------------------------------------

				// Disconnect USB/RS-232 converter. Expected: No exceptions, terminal is closed:
				Assert.IsTrue(UsbHubControl.Disable(portOut), "Failed to modify USB hub!");
				Assert.IsTrue(terminal.IsStarted); // Terminal still started, and must automatically close!
				Utilities.WaitForClose(terminal);
				Assert.IsFalse(terminal.IsOpen);
				Assert.IsFalse(terminal.IsReadyToSend);

				// Manually close terminal. Expected: No exceptions, terminal can be closed.
				Assert.IsTrue(terminal.StopIO());
				Assert.IsFalse(terminal.IsStarted);
				Assert.IsFalse(terminal.IsOpen);
				Assert.IsFalse(terminal.IsReadyToSend);

				// Reconnect USB/RS-232 converter. Expected: No exceptions, terminal can be reopened.
				Assert.IsTrue(UsbHubControl.Enable(portOut), "Failed to modify USB hub!");
				Assert.IsFalse(terminal.IsStarted);

				// Manually open terminal again. Expected: No exceptions, terminal can be opened.
				Thread.Sleep(WaitForOperation);
				Assert.IsTrue(terminal.StartIO());
				Utilities.WaitForOpen(terminal);
				Assert.IsTrue(terminal.IsStarted);
				Assert.IsTrue(terminal.IsOpen);
				Assert.IsTrue(terminal.IsReadyToSend);

				// Close terminal. Expected: No exceptions, terminal can be closed.
				Thread.Sleep(WaitForOperation);
				Assert.IsTrue(terminal.StopIO());
				Assert.IsFalse(terminal.IsStarted);
				Assert.IsFalse(terminal.IsOpen);
				Assert.IsFalse(terminal.IsReadyToSend);
			} // Expected: No exceptions, terminal can be disposed.

			// --- Postcondition: USB hub is set to its defaults, i.e. all outputs are enabled. ----

			Assert.IsTrue(UsbHubControl.Set(UsbHubSettings.All), "Failed to set USB hub!");
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
