﻿//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MKY.IO.Usb;

namespace MKY.IO.Usb.Demo
{
	public class DemoMain
	{
		public static void Main(string[] args)
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

			Console.WriteLine("Press <Enter> to exit");
			Console.ReadLine();
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
