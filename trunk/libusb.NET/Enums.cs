// Enums.cs
// Copyright (C) 2004 Mike Krueger
// 
// This program is free software. It is dual licensed under GNU GPL and GNU LGPL.
// See COPYING_GPL.txt and COPYING_LGPL.txt for details.
//

using System;

namespace libusb.NET
{
	/// <summary>
	/// Device and/or Interface Class codes.
	/// </summary>
	public enum UsbClass
	{
		PerInterface = 0, // for DeviceClass
		Audio        = 1,
		Comm         = 2,
		HID          = 3,
		Printer      = 7,
		MassStorage  = 8,
		HUB          = 9,
		Data         = 10,
		VendorSpec   = 0xF
	}
	
	/// <summary>
	/// Descriptor types.
	/// </summary>
	public enum UsbDescriptorType
	{
		Device    = 0x01,
		Config    = 0x02,
		String    = 0x03,
		Interface = 0x04,
		EndPoint  = 0x05,
		HID       = 0x21,
		Report    = 0x22,
		Physical  = 0x23,
		HUB       = 0x29
	}	
}
