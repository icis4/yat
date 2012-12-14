//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.8
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2012 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;

namespace MKY.IO.Usb.Demo
{
	/// <summary></summary>
	public static class ConsoleProgram
	{
		/// <summary></summary>
		[STAThread]
		public static void Main()
		{
			DeviceCollection devices = new DeviceCollection(DeviceClass.Hid);
			devices.FillWithAvailableDevices();

			Console.WriteLine();
			Console.WriteLine("USB HID Devices");
			foreach (DeviceInfo device in devices)
			{
				Console.Write(" + ");
				Console.WriteLine(device.ToString());
				Console.Write("   ");
				Console.WriteLine(device.Path);
			}
			Console.WriteLine();

			Console.WriteLine("Press <Enter> to exit.");
			Console.ReadLine();
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
