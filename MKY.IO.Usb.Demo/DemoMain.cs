//==================================================================================================
// $URL: https://y-a-terminal.svn.sourceforge.net/svnroot/y-a-terminal/trunk/MKY.IO.Usb.Test/UsbDeviceIdTest.cs $
// $Author: maettu_this $
// $Date: 2010-03-01 22:31:11 +0100 (Mo, 01 Mrz 2010) $
// $Revision: 261 $
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
                Console.WriteLine(device.SystemPath);
            }
            Console.WriteLine();

            Console.WriteLine("Press <Enter> to exit");
            Console.ReadLine();
        }
    }
}

//==================================================================================================
// End of
// $URL: https://y-a-terminal.svn.sourceforge.net/svnroot/y-a-terminal/trunk/MKY.IO.Usb.Test/UsbDeviceIdTest.cs $
//==================================================================================================
