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
// MKY Version 1.0.28 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics;

namespace MKY.IO.Usb.Demo
{
	/// <summary></summary>
	public static class ConsoleProgram
	{
		/// <summary></summary>
		[STAThread]
		public static void Main()
		{
			var devices = new HidDeviceCollection();
			devices.FillWithAvailableDevices(true); // Retrieve strings from devices in order to get serial strings.

			Console.Out.WriteLine();
			Console.Out.WriteLine("USB HID Devices");
			foreach (var device in devices)
			{
				Console.Out.Write(" + ");
				Console.Out.WriteLine(device.ToString());
				Console.Out.Write("   ");
				Console.Out.WriteLine(device.Path);

				using (var listener = new ConsoleTraceListener())
				{
					Debug.Listeners.Add(listener);
					HidDevice.DebugWriteDeviceCapabilities(device.Path);
				}
			}
			Console.Out.WriteLine();

			Console.Out.WriteLine("Press <Enter> to exit.");
			Console.In.ReadLine();
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
