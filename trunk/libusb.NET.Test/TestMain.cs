//==================================================================================================
// $URL: https://y-a-terminal.svn.sourceforge.net/svnroot/y-a-terminal/trunk/MKY.IO.Ports/Properties/AssemblyInfo.cs $
// $Author: maettu_this $
// $Date: 2010-02-23 00:18:29 +0100 (Di, 23 Feb 2010) $
// $Revision: 254 $
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2004 Mike Krueger. 
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

using libusb.NET;

namespace libusb.NET.Test
{
    public class TestMain
    {
        public static void Main(string[] args)
        {
            foreach (Bus bus in Bus.Busses)
            {
                Console.WriteLine(bus);
                foreach (Descriptor descriptor in bus.Descriptors)
                {
                    Console.WriteLine("\t" + descriptor);
                    try
                    {
                        using (Device device = descriptor.OpenDevice())
                        {
                            Console.WriteLine("\t\t     Product: " + device.Product);
                            Console.WriteLine("\t\tManufacturer: " + device.Manufacturer);
                            Console.WriteLine();
                        }
                    }
                    catch (UsbException e)
                    {
                        Console.WriteLine("Got Exception : " + e);
                    }
                }
            }
        }
    }
}

//==================================================================================================
// End of
// $URL: https://y-a-terminal.svn.sourceforge.net/svnroot/y-a-terminal/trunk/MKY.IO.Ports/Properties/AssemblyInfo.cs $
//==================================================================================================
