// Bus.cs
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
	/// Description of Bus.
	/// </summary>
	public class Bus
	{
		usb_bus              nativeBus;
		DescriptorCollection descriptors;
		
		public static BusCollection Busses {
			get {
				BusCollection busCollection = new BusCollection();
				NativeMethods.usb_init();
				int rc = NativeMethods.usb_find_busses();
				if (rc <= 0) {
					throw new MethodCallUsbException("usb_find_busses", rc);
				}
				
				rc = NativeMethods.usb_find_devices();
				if (rc <= 0) {
					throw new MethodCallUsbException("usb_find_devices", rc);
				}
				
				for (usb_bus bus = NativeMethods.usb_get_busses(); bus != null; bus = bus.Next) {
					busCollection.Add(new Bus(bus));
				}
				return busCollection;
			}
		}
		
		public string DirectoryName {
			get {
				return nativeBus.dirname;
			}
		}
		
		public DescriptorCollection Descriptors {
			get {
				return descriptors;
			}
		}
		
		internal Bus(usb_bus nativeBus)
		{
			this.nativeBus = nativeBus;
			this.descriptors = new DescriptorCollection();
		
			
			for (usb_device dev = nativeBus.Devices; dev != null; dev = dev.Next) {
				descriptors.Add(new Descriptor(dev));
			}
		}
		
		public override string ToString() 
		{
			return String.Format("[Bus: DirectoryName = {0}]", DirectoryName);
		}
	}
}
