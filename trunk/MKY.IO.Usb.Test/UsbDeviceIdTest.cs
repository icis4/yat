//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using MKY.IO.Usb;

namespace MKY.IO.Usb.Test
{
	[TestFixture]
	public class UsbDeviceIdTest
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private struct TestSet
		{
            public readonly bool IsValid;
			public readonly int VendorId;
			public readonly int ProductId;
			public readonly string[] Descriptions;

			public TestSet(bool isValid, int vendorId, int productId, string[] descriptions)
			{
                IsValid      = isValid;
				VendorId     = vendorId;
				ProductId    = productId;
				Descriptions = descriptions;
			}
		};

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private readonly TestSet[] _testSets =
		{
			new TestSet(true,   0x0ABC,  0x1234, new string[] { "VID:0ABC PID:1234", "vid:0ABC pid:1234"} ),
			new TestSet(true,   0x0ABC,  0x1234, new string[] { "VID_0ABC PID_1234", "vid_0ABC pid_1234"} ),
			new TestSet(true,   0x0ABC,  0x1234, new string[] { "VID_0ABC&PID_1234", "vid_0ABC&pid_1234"} ),
			new TestSet(true,   0x0ABC,  0x1234, new string[] { "Company (VID:0ABC) Product (PID:1234) Generic USB Hub"} ),
			new TestSet(false,  0x0000,  0x0000, new string[] { " VID:0000  PID:0000" } ),
			new TestSet(false,  0x0000,  0x0001, new string[] { " VID:0000  PID:0001" } ),
			new TestSet(false,  0x0001,  0x0000, new string[] { " VID:0001  PID:0000" } ),
			new TestSet(false,  0x0001, 0x10000, new string[] { " VID:0001 PID:10000" } ),
			new TestSet(false, 0x10000,  0x0001, new string[] { "VID:10000  PID:0001" } ),
			new TestSet(false, 0x10000, 0x10000, new string[] { "VID:10000 PID:10000" } ),
		};

		#endregion

		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		#region Tests > Constructor/Parse()
		//------------------------------------------------------------------------------------------
		// Tests > Constructor/Parse()
		//------------------------------------------------------------------------------------------

		[Test]
		public void TestConstructorAndParse()
		{
			DeviceId id;

			foreach (TestSet ts in _testSets)
			{
                if (ts.IsValid)
                {
                    id = new DeviceId(ts.VendorId, ts.ProductId);
                    Assert.AreEqual(ts.VendorId,  id.VendorId);
                    Assert.AreEqual(ts.ProductId, id.ProductId);

                    foreach (string description in ts.Descriptions)
                    {
                        id = DeviceId.Parse(description);
                        Assert.AreEqual(ts.VendorId,  id.VendorId);
                        Assert.AreEqual(ts.ProductId, id.ProductId);
                    }
                }
                else
                {
                    try
                    {
                        id = new DeviceId(ts.VendorId, ts.ProductId);
                        Assert.Fail
                            (
                            "Invalid ID pair " + ts.VendorId + "/" + ts.ProductId +
                            " wasn't properly handled"
                            );
                    }
                    catch
                    {
                        // Invalid input must throw an exception
                    }

                    foreach (string description in ts.Descriptions)
                    {
                        try
                        {
                            id = DeviceId.Parse(description);
                            Assert.Fail
                                (
                                "Invalid descripton " + description +
                                " wasn't properly handled"
                                );
                        }
                        catch
                        {
                            // Invalid input must throw an exception
                        }
                    }
                }
			}
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
