// Descriptor.cs
// Copyright (C) 2004 Mike Krueger
// 
// This program is free software. It is dual licensed under GNU GPL and GNU LGPL.
// See COPYING_GPL.txt and COPYING_LGPL.txt for details.
//

using System;
using libusb.NET.Internal;

namespace libusb.NET
{
	/// <summary>
	/// Description of Descriptor.
	/// </summary>
	public class Descriptor
	{
		usb_device nativeDevice;
		
		internal usb_device NativeDevice {
			get {
				return nativeDevice;
			}
		}
		
		public UsbDescriptorType DescriptorType {
			get {
				return (UsbDescriptorType)nativeDevice.descriptor.bDescriptorType;
			}
		}
		
		public UsbClass DeviceClass {
			get {
				return (UsbClass)nativeDevice.descriptor.bDeviceClass;
			}
		}
		
		public string FileName {
			get {
				return nativeDevice.filename;
			}
		}
		
		public int VendorId {
			get {
				return nativeDevice.descriptor.idVendor;
			}
		}
		
		public int ProductId {
			get {
				return nativeDevice.descriptor.idProduct;
			}
		}
		
		internal Descriptor(usb_device nativeDevice)
		{
			this.nativeDevice = nativeDevice;
		}
		
		public Device OpenDevice()
		{
			return new Device(this);
		}
		
		public override string ToString() 
		{
			return String.Format("[Descriptor: DescriptorType = {3}, DeviceClass = {4}, FileName = {0}, VendorId=0x{1:X}, ProductId=0x{2:X}]",
			                     FileName,
			                     VendorId,
			                     ProductId,
			                     DescriptorType,
			                     DeviceClass
			                    );
		}
	}
}
