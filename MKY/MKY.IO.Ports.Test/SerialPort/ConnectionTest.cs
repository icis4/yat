﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.14
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2015 Matthias Kläy.
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
using System.Threading;
using System.Reflection;

using NUnit;
using NUnit.Framework;

#endregion

namespace MKY.IO.Ports.Test.SerialPort
{
	/// <summary></summary>
	[TestFixture]
	public class ConnectionTest
	{
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
		[Test, MTSicsDeviceAIsConnectedCategory]
		public virtual void TestCloseReopenSerialPortEx()
		{
			TestCloseReopen(new SerialPortEx());
		}

		private void TestCloseReopen(System.IO.Ports.SerialPort port)
		{
			if (!ConfigurationProvider.Configuration.MTSicsDeviceAIsConnected)
				Assert.Ignore("'MTSicsDeviceA' is not connected, therefore this test is excluded. Ensure that 'MTSicsDeviceA' is properly configured and available if passing this test is required.");
				// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

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
			port.WriteLine("SIR"); // \ToDo: Should be upgraded to ECHO as soon as mode 2 is available.
			Thread.Sleep(WaitForOperation);
			port.ReadExisting();

			// Close and reopen port. Expected: No exceptions, port can be closed and reopened.
			Thread.Sleep(WaitForOperation);
			port.Close();
			Assert.IsFalse(port.IsOpen);
			Thread.Sleep(WaitForOperation);
			port.Open();
			Assert.IsTrue(port.IsOpen);

			// Stop continuous data:
			port.WriteLine("SI"); // \ToDo: Should be upgraded to ECHO as soon as mode 2 is available.
			Thread.Sleep(WaitForOperation);
			port.ReadExisting();

			// Close and dispose port. Expected: No exceptions, port can be closed and disposed.
			Thread.Sleep(WaitForOperation);
			port.Close();
			Assert.IsFalse(port.IsOpen);
			port.Dispose();
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

		private void TestDisconnectReconnect(System.IO.Ports.SerialPort port, bool testWithContinuousReceiving)
		{
			// Keep constructor of effective type in order to later recreate a port object of the same type:
			ConstructorInfo ci = port.GetType().GetConstructor(Type.EmptyTypes);

			if (!ConfigurationProvider.Configuration.MTSicsDeviceAIsConnected)
				Assert.Ignore("'MTSicsDeviceA' is not connected, therefore this test is excluded. Ensure that 'MTSicsDeviceA' is properly configured and available if passing this test is required.");
				// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			if (!UsbHubControl.Probe())
				Assert.Ignore(UsbHubControl.ErrorMessage);
				// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			string portName = ConfigurationProvider.Configuration.MTSicsDeviceA;
			UsbHubSetting portOut = UsbHubSetting.Out4;
			const int WaitForOperation = 100;

			// --- Precondition: USB hub is set to its defaults, i.e. all outputs are enabled. -----

			// Configure and open port:
			port.NewLine = "\r\n"; // <CR><LF>
			port.PortName = portName;
			port.Open();
			Assert.IsTrue(port.IsOpen);

			// --- Test: Disconnect/Reconnect without sending. -------------------------------------

			// Disconnect USB/RS-232 converter. Expected: No exceptions, port is closed:
			Assert.IsTrue(UsbHubControl.Set(UsbHubSetting.None), "Failed to modify USB hub!");
			// Disabling all outputs is used to improve speed when enabling single outputs below.
			// See comments in implementation of 'UsbHubControl' for explanation.
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
				port.WriteLine("SIR"); // \ToDo: Should be upgraded to ECHO as soon as mode 2 is available.
				Thread.Sleep(WaitForOperation);
				port.ReadExisting();
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
				port.WriteLine("SI"); // \ToDo: Should be upgraded to ECHO as soon as mode 2 is available.
				Thread.Sleep(WaitForOperation);
				port.ReadExisting();
			}

			// Close and dispose port. Expected: No exceptions, port can be closed and disposed.
			Thread.Sleep(WaitForOperation);
			port.Close();
			Assert.IsFalse(port.IsOpen);
			port.Dispose();

			// --- Postcondition: USB hub is set to its defaults, i.e. all outputs are enabled. ----
			Assert.IsTrue(UsbHubControl.Set(UsbHubSetting.All), "Failed to set USB hub!");
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
