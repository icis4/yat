﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.17
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2016 Matthias Kläy.
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
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading;

using NUnit;
using NUnit.Framework;

#endregion

namespace MKY.IO.Ports.Test.SerialPort
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
			TestCloseReopen(new System.IO.Ports.SerialPort());
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
		[Test, MTSicsDeviceAIsConnectedCategory]
		public virtual void TestCloseReopenSerialPortEx()
		{
			TestCloseReopen(new SerialPortEx());
		}

		private static void TestCloseReopen(System.IO.Ports.SerialPort port)
		{
			if (!ConfigurationProvider.Configuration.MTSicsDeviceAIsConnected)
				Assert.Ignore("'MTSicsDeviceA' is not connected, therefore this test is excluded. Ensure that 'MTSicsDeviceA' is properly configured and available if passing this test is required.");
				//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			string portName = ConfigurationProvider.Configuration.MTSicsDeviceA;
			const int WaitForOperation = 100;

			// Configure and open port:
			port.NewLine = "\r\n"; // <CR><LF>
			port.PortName = portName;
			port.Open();
			Assert.IsTrue(port.IsOpen);

			// --- Test: Close/Reopen without sending. ---------------------------------------------

			// Close and reopen port. Expected: No exceptions, port can be closed and reopened.
			Thread.Sleep(WaitForOperation);
			port.Close();
			Assert.IsFalse(port.IsOpen);
			Thread.Sleep(WaitForOperation);
			port.Open();
			Assert.IsTrue(port.IsOpen);

			// --- Test: Close/Reopen with previous and subsequent sending. ------------------------

			// Send something and verify response:
			port.WriteLine(""); // Perform empty request.
			Thread.Sleep(WaitForOperation);
			Assert.AreEqual("ES", port.ReadLine()); // Verify empty request.

			// Close and reopen port. Expected: No exceptions, port can be closed and reopened.
			Thread.Sleep(WaitForOperation);
			port.Close();
			Assert.IsFalse(port.IsOpen);
			Thread.Sleep(WaitForOperation);
			port.Open();
			Assert.IsTrue(port.IsOpen);

			// Send something and verify response:
			port.WriteLine(""); // Perform empty request.
			Thread.Sleep(WaitForOperation);
			Assert.AreEqual("ES", port.ReadLine()); // Verify empty request.

			// --- Test: Close/Reopen while continuous receiving. ----------------------------------

			// Request continuous data:
			port.WriteLine("ECHO 2"); // Activate continuous echo mode.
			Thread.Sleep(WaitForOperation);
			Assert.AreEqual("ECHO C", port.ReadLine(), "Failed to initiate ECHO mode 2!");
			port.WriteLine(CommandToEcho); // Request continuous echo.

			// Close and reopen port. Expected: No exceptions, port can be closed and reopened.
			Thread.Sleep(WaitForOperation);
			port.Close();
			Assert.IsFalse(port.IsOpen);
			Thread.Sleep(WaitForOperation);
			port.Open();
			Assert.IsTrue(port.IsOpen);

			// Stop continuous data:
			port.Write(new byte[] { 0x1B }, 0, 1); // <ESC> to quit ECHO mode.
			Thread.Sleep(WaitForOperation);
			port.ReadExisting();

			// Close and dispose port. Expected: No exceptions, port can be closed and disposed.
			Thread.Sleep(WaitForOperation);
			port.Close();
			Assert.IsFalse(port.IsOpen);
			port.Dispose();
			Thread.Sleep(WaitForOperation); // Wait to prevent issues in subsequent test cases.
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
			TestDisconnectReconnect(new System.IO.Ports.SerialPort(), false); // See comments in TestDisconnectReconnect().
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
		[Test, MTSicsDeviceAIsConnectedCategory, MinuteDurationCategory(1)]
		public virtual void TestDisconnectReconnectSerialPortEx()
		{
			TestDisconnectReconnect(new SerialPortEx(), false); // See comments in TestDisconnectReconnect().
		}

		/// <summary></summary>
		[Test, MTSicsDeviceAIsConnectedCategory, MinuteDurationCategory(1), Explicit("This test requires to manually reset the sending device beause it will remain in continuous mode as well as the port device because it cannot be opened until disconnected/reconnected!")]
		public virtual void TestDisconnectReconnectSerialPortExWithContinuousReceiving()
		{
			TestDisconnectReconnect(new SerialPortEx(), true); // See comments in TestDisconnectReconnect().
		}

		/// <remarks>
		/// There is a similar method in 'YAT.Model.Test.ConnectionTest'.
		/// Changes here may have to be applied there too.
		/// 
		/// Tests implemented below:
		///  - Disconnect/Reconnect without sending.
		///  - Disconnect/Reconnect with previous and subsequent sending.
		///  - Disconnect/Reconnect while continuous receiving.
		/// </remarks>
		private static void TestDisconnectReconnect(System.IO.Ports.SerialPort port, bool testWithContinuousReceiving)
		{
			// Keep constructor of effective type in order to later recreate a port object of the same type:
			ConstructorInfo ci = port.GetType().GetConstructor(Type.EmptyTypes);

			if (!ConfigurationProvider.Configuration.MTSicsDeviceAIsConnected)
				Assert.Ignore("'MTSicsDeviceA' is not connected, therefore this test is excluded. Ensure that 'MTSicsDeviceA' is properly configured and available if passing this test is required.");
				//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			if (!UsbHubControl.Probe())
				Assert.Ignore(UsbHubControl.ErrorMessage);
				//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			string portName = ConfigurationProvider.Configuration.MTSicsDeviceA;
			UsbHubSettings portOut = UsbHubSettings.Out4;
			const int WaitForOperation = 100;

			// --- Precondition: USB hub is set to its defaults, i.e. all outputs are enabled. -----

			// Configure and open port:
			port.NewLine = "\r\n"; // <CR><LF>
			port.PortName = portName;
			port.Open();
			Assert.IsTrue(port.IsOpen);

			// --- Test: Disconnect/Reconnect without sending. -------------------------------------

			// Disconnect USB/RS-232 converter. Expected: No exceptions, port is closed:
			Assert.IsTrue(UsbHubControl.Set(UsbHubSettings.None), "Failed to modify USB hub!"); // Disabling all outputs is used to improve speed when enabling single outputs below. See comments in implementation of 'UsbHubControl' for explanation.
			Assert.IsFalse(port.IsOpen);

			// Reconnect USB/RS-232 converter. Expected: No exceptions, port can be reopened.
			Assert.IsTrue(UsbHubControl.Enable(portOut), "Failed to modify USB hub!");
			port.Open();
			Assert.IsTrue(port.IsOpen);

			// Close and reopen port. Expected: No exceptions, port can be closed and reopened.
			Thread.Sleep(WaitForOperation);
			port.Close();
			Assert.IsFalse(port.IsOpen);
			Thread.Sleep(WaitForOperation);
			port.Open();
			Assert.IsTrue(port.IsOpen);

			// --- Test: Disconnect/Reconnect with previous and subsequent sending. ----------------

			// Send something and verify response:
			port.WriteLine(""); // Perform empty request.
			Thread.Sleep(WaitForOperation);
			Assert.AreEqual("ES", port.ReadLine()); // Verify empty request.

			// Disconnect USB/RS-232 converter. Expected: No exceptions, port is closed:
			Assert.IsTrue(UsbHubControl.Disable(portOut), "Failed to modify USB hub!");
			Assert.IsFalse(port.IsOpen);

			// Reconnect USB/RS-232 converter. Expected: No exceptions, port can be reopened.
			Assert.IsTrue(UsbHubControl.Enable(portOut), "Failed to modify USB hub!");
			port.Open();
			Assert.IsTrue(port.IsOpen);

			// Send something and verify response:
			port.WriteLine(""); // Perform empty request.
			Thread.Sleep(WaitForOperation);
			Assert.AreEqual("ES", port.ReadLine()); // Verify empty request.

			// Close and reopen port. Expected: No exceptions, port can be closed and reopened.
			Thread.Sleep(WaitForOperation);
			port.Close();
			Assert.IsFalse(port.IsOpen);
			Thread.Sleep(WaitForOperation);
			port.Open();
			Assert.IsTrue(port.IsOpen);

			// Send something and verify response:
			port.WriteLine(""); // Perform empty request.
			Thread.Sleep(WaitForOperation);
			Assert.AreEqual("ES", port.ReadLine()); // Verify empty request.

			// --- Test: Disconnect/Reconnect while continuous receiving. --------------------------

			if (testWithContinuousReceiving) // See block of comments further below.
			{
				// Request continuous data:
				port.WriteLine("ECHO 2"); // Activate continuous echo mode.
				Thread.Sleep(WaitForOperation);
				Assert.AreEqual("ECHO C", port.ReadLine(), "Failed to initiate ECHO mode 2!");
				port.WriteLine(CommandToEcho); // Request continuous echo.
				Thread.Sleep(WaitForOperation);

				// Disconnect USB/RS-232 converter. Expected: No exceptions, port is closed:
				Assert.IsTrue(UsbHubControl.Disable(portOut), "Failed to modify USB hub!");
			////Assert.IsFalse(port.IsOpen);

				// \Remind: The port should be closed here. However, this doesn't work due to the 
				// issue documented in the header of 'SerialPortEx'. The assertion above actually
				// results in an ObjectDisposedException "The SafeHandle has been closed" which
				// would...
				// ...actually be handled in a Application.ThreadException => EventHandler(HandleThreadException)...
				// ...and result in an additional UnauthorizedAccessException when calling port.Dispose()...
				// ...and port cannot be recreated even after all devices have been reset.
				// Instead, this test can only be executed with an improved 'SerialPortEx' object,
				// Dispose() must be called here, and a new object must be created.

				port.Dispose();
				Thread.Sleep(WaitForOperation);
				port = (System.IO.Ports.SerialPort)ci.Invoke(null); // Recreate a port object.
				port.NewLine = "\r\n"; // <CR><LF>
				port.PortName = portName;

				// Reconnect USB/RS-232 converter. Expected: No exceptions, port can be reopened.
				Assert.IsTrue(UsbHubControl.Enable(portOut), "Failed to modify USB hub!");
				port.Open();
				Assert.IsTrue(port.IsOpen);

				// Stop continuous data:
				port.Write(new byte[] { 0x1B }, 0, 1); // <ESC> to quit ECHO mode.
				Thread.Sleep(WaitForOperation);
				port.ReadExisting();
			}

			// Close and dispose port. Expected: No exceptions, port can be closed and disposed.
			Thread.Sleep(WaitForOperation);
			port.Close();
			Assert.IsFalse(port.IsOpen);
			port.Dispose();
			Thread.Sleep(WaitForOperation); // Wait to prevent issues in subsequent test cases.

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