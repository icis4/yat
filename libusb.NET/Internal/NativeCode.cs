// NaticeCode.cs
// Copyright (C) 2004 Mike Krueger
// 
// This program is free software. It is dual licensed under GNU GPL and GNU LGPL.
// See COPYING_GPL.txt and COPYING_LGPL.txt for details.
//

using System;
using System.Text;
using System.Security;
using System.Runtime.InteropServices;

namespace libusb.NET.Internal
{
	/// <summary>
	/// Descriptor sizes per descriptor type.
	/// </summary>
	public enum UsbDescriptorTypeSize
	{
		Device        = 18,
		Config        = 9,
		Interface     = 9,
		EndPoint      = 7,
		EndPointAudio = 9, // Audio extension
		HUBNonVar     = 7
	}
	
	/// <summary>
	/// All standard descriptors have these 2 fields in common
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct usb_descriptor_header 
	{
		public byte bLength;
		public byte bDescriptorType;
		
		public override string ToString()
		{
			return String.Format("[usb_descriptor_header: bDescriptorType = {0}, bLength = {1}]",
			                     bDescriptorType,
			                     bLength);
		}
	};
	
	/// <summary>
	/// String descriptor
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct usb_string_descriptor
	{
		public byte  bLength;
		public byte  bDescriptorType;
		public ushort wData0;
		
		public override string ToString()
		{
			return String.Format("[usb_string_descriptor: bDescriptorType = {0}, bLength = {1}, wData0 = {2}]",
			                     bDescriptorType,
			                     bLength,
			                     wData0);
		}
	};
	
	/// <summary>
	/// HID descriptor
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct usb_hid_descriptor 
	{
		public byte  bLength;
		public byte  bDescriptorType;
		public ushort bcdHID;
		public byte  bCountryCode;
		public byte  bNumDescriptors;
		/* byte  bReportDescriptorType; */
		/* ushort wDescriptorLength; */
		/* ... */
		
		public override string ToString()
		{
			return String.Format("[usb_hid_descriptor: bcdHID = {0}, bCountryCode = {1}, bDescriptorType = {2}, bLength = {3}, bNumDescriptors = {4}]",
			                     bcdHID,
			                     bCountryCode,
			                     bDescriptorType,
			                     bLength,
			                     bNumDescriptors);
		}
	};
	
	/// <summary>
	/// Endpoint descriptor
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct usb_endpoint_descriptor 
	{
		public const byte USB_MAXENDPOINTS = 32;
		
		public byte  bLength;
		public byte  bDescriptorType;
		public byte  bEndpointAddress;
		public byte  bmAttributes;
		public ushort wMaxPacketSize;
		public byte  bInterval;
		public byte  bRefresh;
		public byte  bSynchAddress;
		
		public IntPtr extra;	// Extra descriptors 
		public int extralen;
		
		public const byte USB_ENDPOINT_ADDRESS_MASK = 0x0f; // in bEndpointAddress
		public const byte USB_ENDPOINT_DIR_MASK	    = 0x80;

		public const byte USB_ENDPOINT_TYPE_MASK        = 0x03; // in bmAttributes
		public const byte USB_ENDPOINT_TYPE_CONTROL     = 0;
		public const byte USB_ENDPOINT_TYPE_ISOCHRONOUS = 1;
		public const byte USB_ENDPOINT_TYPE_BULK        = 2;
		public const byte USB_ENDPOINT_TYPE_INTERRUPT   = 3;
		
		public override string ToString() {
			return String.Format("[usb_endpoint_descriptor: bDescriptorType = {0}, bEndpointAddress = {1}, bInterval = {2}, bLength = {3}, bmAttributes = {4}, bRefresh = {5}, bSynchAddress = {6}, extra = {7}, extralen = {8}, wMaxPacketSize = {9}]",
			                     bDescriptorType,
			                     bEndpointAddress,
			                     bInterval,
			                     bLength,
			                     bmAttributes,
			                     bRefresh,
			                     bSynchAddress,
			                     extra,
			                     extralen,
			                     wMaxPacketSize);
		}
	};
	
	/// <summary>
	/// Interface descriptor
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct usb_interface_descriptor 
	{
		public const int USB_MAXINTERFACES = 32;
		
		public byte  bLength;
		public byte  bDescriptorType;
		public byte  bInterfaceNumber;
		public byte  bAlternateSetting;
		public byte  bNumEndpoints;
		public byte  bInterfaceClass;
		public byte  bInterfaceSubClass;
		public byte  bInterfaceProtocol;
		public byte  iInterface;
		
		public IntPtr endpoint;
		
		public IntPtr extra;	/* Extra descriptors */
		public int extralen;
		
		public usb_endpoint_descriptor Endpoint {
			get {
				if (endpoint == IntPtr.Zero) {
					return new usb_endpoint_descriptor();
				}
				return (usb_endpoint_descriptor)Marshal.PtrToStructure(endpoint, typeof(usb_endpoint_descriptor));
			}
		}
		
		public override string ToString()
		{
			return String.Format("[usb_interface_descriptor: bAlternateSetting = {0}, bDescriptorType = {1}, bInterfaceClass = {2}, bInterfaceNumber = {3}, bInterfaceProtocol = {4}, bInterfaceSubClass = {5}, bLength = {6}, bNumEndpoints = {7}, endpoint = {8}, extra = {9}, extralen = {10}, iInterface = {11}]",
			                     bAlternateSetting,
			                     bDescriptorType,
			                     bInterfaceClass,
			                     bInterfaceNumber,
			                     bInterfaceProtocol,
			                     bInterfaceSubClass,
			                     bLength,
			                     bNumEndpoints,
			                     endpoint,
			                     extra,
			                     extralen,
			                     iInterface);
		}
	};
	
	[StructLayout(LayoutKind.Sequential)]
	internal struct usb_interface 
	{
		public const int USB_MAXALTSETTING = 128;	/* Hard limit */
		public IntPtr altsetting;
		
		public int num_altsetting;
		
		public usb_interface_descriptor Altsetting {
			get {
				if (altsetting == IntPtr.Zero) {
					return new usb_interface_descriptor();
				}
				
				return (usb_interface_descriptor)Marshal.PtrToStructure(altsetting, typeof(usb_interface_descriptor));
			}
		}
		public override string ToString() 
		{
			return String.Format("[usb_interface: altsetting = {0}, num_altsetting = {1}]",
			                     altsetting,
			                     num_altsetting);
		}
		
	};
	
	/* Configuration descriptor information.. */
	[StructLayout(LayoutKind.Sequential)]
	internal struct usb_config_descriptor 
	{
		public const int USB_MAXCONFIG = 8;
		
		public byte   bLength;
		public byte   bDescriptorType;
		public ushort wTotalLength;
		public byte   bNumInterfaces;
		public byte   bConfigurationValue;
		public byte   iConfiguration;
		public byte   bmAttributes;
		public byte   MaxPower;
		
		public IntPtr iface;
		
		public IntPtr extra;	/* Extra descriptors */
		public int    extralen;
		
		public usb_interface Interface {
			get {
				if (iface == IntPtr.Zero) {
					return new usb_interface();
				}
				
				return (usb_interface)Marshal.PtrToStructure(iface, typeof(usb_interface));
			}
		}
		
		public override string ToString()
		{
			return String.Format("[usb_config_descriptor: bConfigurationValue = {0}, bDescriptorType = {1}, bLength = {2}, bmAttributes = {3}, bNumInterfaces = {4}, extra = {5}, extralen = {6}, iConfiguration = {7}, iface = {8}, MaxPower = {9}, wTotalLength = {10}]",
			                     bConfigurationValue,
			                     bDescriptorType,
			                     bLength,
			                     bmAttributes,
			                     bNumInterfaces,
			                     extra,
			                     extralen,
			                     iConfiguration,
			                     iface,
			                     MaxPower,
			                     wTotalLength);
		}
	};
	
	/* Device descriptor */
	[StructLayout(LayoutKind.Sequential)]
	internal struct usb_device_descriptor 
	{
		public byte  bLength;
		public byte  bDescriptorType;
		public ushort bcdUSB;
		public byte  bDeviceClass;
		public byte  bDeviceSubClass;
		public byte  bDeviceProtocol;
		public byte  bMaxPacketSize0;
		public ushort idVendor;
		public ushort idProduct;
		public ushort bcdDevice;
		public byte  iManufacturer;
		public byte  iProduct;
		public byte  iSerialNumber;
		public byte  bNumConfigurations;
		
		public override string ToString() 
		{
			return String.Format("[usb_device_descriptor: bcdDevice = {0}, bcdUSB = {1}, bDescriptorType = {2}, bDeviceClass = {3}, bDeviceProtocol = {4}, bDeviceSubClass = {5}, bLength = {6}, bMaxPacketSize0 = {7}, bNumConfigurations = {8}, idProduct = {9}, idVendor = {10}, iManufacturer = {11}, iProduct = {12}, iSerialNumber = {13}]",
			                     bcdDevice,
			                     bcdUSB,
			                     bDescriptorType,
			                     bDeviceClass,
			                     bDeviceProtocol,
			                     bDeviceSubClass,
			                     bLength,
			                     bMaxPacketSize0,
			                     bNumConfigurations,
			                     idProduct,
			                     idVendor,
			                     iManufacturer,
			                     iProduct,
			                     iSerialNumber);
		}
		
	};
	
	[StructLayout(LayoutKind.Sequential)]
	internal struct usb_ctrl_setup 
	{
		public byte  bRequestType;
		public byte  bRequest;
		public ushort wValue;
		public ushort wIndex;
		public ushort wLength;
		
		public override string ToString() 
		{
			return String.Format("[usb_ctrl_setup: bRequest = {0}, bRequestType = {1}, wIndex = {2}, wLength = {3}, wValue = {4}]",
			                     bRequest,
			                     bRequestType,
			                     wIndex,
			                     wLength,
			                     wValue);
		}
	};
	
	[StructLayout(LayoutKind.Sequential)]
	internal class usb_device 
	{
		public IntPtr next = IntPtr.Zero;
		public IntPtr prev = IntPtr.Zero;
		
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = NativeMethods.LIBUSB_PATH_MAX)]
		public string filename = String.Empty;
		
		public IntPtr bus    = IntPtr.Zero;
		
		public usb_device_descriptor descriptor = new usb_device_descriptor();
		public IntPtr config = IntPtr.Zero;
		
		public IntPtr dev    = IntPtr.Zero;		// Darwin support
		
		public override string ToString()
		{
			return String.Format("[usb_device: filename={0}, next={1}, prev={2}, bus={3}, descriptor={4}, config={5}, dev={6}]",
			                     filename,
			                     next,
			                     prev,
			                     bus,
			                     descriptor,
			                     config,
			                     dev);
		}
		
		public usb_device Next {
			get {
				if (next == IntPtr.Zero) {
					return null;
				}
				return (usb_device)Marshal.PtrToStructure(next, typeof(usb_device));
			}
		}
		
		public usb_device Prev {
			get {
				if (prev == IntPtr.Zero) {
					return null;
				}
				return (usb_device)Marshal.PtrToStructure(prev, typeof(usb_device));
			}
		}
		
		public usb_bus Bus {
			get {
				if (bus == IntPtr.Zero) {
					return null;
				}
				return (usb_bus)Marshal.PtrToStructure(bus, typeof(usb_bus));
			}
		}
		
//		public usb_device_descriptor Descriptor {
//			get {
//				return (usb_device_descriptor)Marshal.PtrToStructure(bus, typeof(usb_device_descriptor));
//			}
//		}
//		
		public usb_config_descriptor GetConfig(int number)
		{
			if (config == IntPtr.Zero) {
				return new usb_config_descriptor();
			}
			IntPtr configPtr = new IntPtr(config.ToInt64() + number * Marshal.SizeOf(typeof(usb_config_descriptor)));
			return (usb_config_descriptor)Marshal.PtrToStructure(configPtr, typeof(usb_config_descriptor));
		}
	};
	
	[StructLayout(LayoutKind.Sequential)]
	internal class usb_bus 
	{
		public IntPtr next = IntPtr.Zero;
		public IntPtr prev = IntPtr.Zero;
		
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = NativeMethods.LIBUSB_PATH_MAX)]
		public string dirname = String.Empty;
		
		public IntPtr devices= IntPtr.Zero;
		public uint location = 0;
		
		public usb_bus Next {
			get {
				if (next == IntPtr.Zero) {
					return null;
				}
				return (usb_bus)Marshal.PtrToStructure(next, typeof(usb_bus));
			}
		}
		public usb_bus Prev {
			get {
				if (prev == IntPtr.Zero) {
					return null;
				}
				return (usb_bus)Marshal.PtrToStructure(prev, typeof(usb_bus));
			}
		}
		
		public usb_device Devices {
			get {
				if (devices == IntPtr.Zero) {
					return null;
				}
				return (usb_device)Marshal.PtrToStructure(devices, typeof(usb_device));
			}
		}
		
		public override string ToString()
		{
			return String.Format("[usb_bus: dirname={0}, next={1}, prev={2}, Devices={3}, location={4}]",
			                     dirname,
			                     next,
			                     prev,
			                     Devices,
			                     location);
		}
	};
	
	internal sealed class NativeMethods
	{
		// Standard requests
		public const int USB_REQ_GET_STATUS	   = 0x00;
		public const int USB_REQ_CLEAR_FEATURE = 0x01;
		
		// 0x02 is reserved 
		public const int USB_REQ_SET_FEATURE = 0x03;
		
		// 0x04 is reserved
		public const int USB_REQ_SET_ADDRESS       = 0x05;
		public const int USB_REQ_GET_DESCRIPTOR    = 0x06;
		public const int USB_REQ_SET_DESCRIPTOR    = 0x07;
		public const int USB_REQ_GET_CONFIGURATION = 0x08;
		public const int USB_REQ_SET_CONFIGURATION = 0x09;
		public const int USB_REQ_GET_INTERFACE     = 0x0A;
		public const int USB_REQ_SET_INTERFACE     = 0x0B;
		public const int USB_REQ_SYNCH_FRAME       = 0x0C;
		
		public const int USB_TYPE_STANDARD = (0x00 << 5);
		public const int USB_TYPE_CLASS    = (0x01 << 5);
		public const int USB_TYPE_VENDOR   = (0x02 << 5);
		public const int USB_TYPE_RESERVED = (0x03 << 5);
		
		public const int USB_RECIP_DEVICE    = 0x00;
		public const int USB_RECIP_INTERFACE = 0x01;
		public const int USB_RECIP_ENDPOINT  = 0x02;
		public const int USB_RECIP_OTHER     = 0x03;
		
		// Various libusb API related stuff
		public const int USB_ENDPOINT_IN  = 0x80;
		public const int USB_ENDPOINT_OUT = 0x00;
		
		// Error codes 
		public const int USB_ERROR_BEGIN = 500000;
		
		const CallingConvention CALLING_CONVENTION    =  CallingConvention.Cdecl;

#if WIN32
    #if !DEBUG
		const string            LIBUSB_NATIVE_LIBRARY = "libusb-1.0_Win32.dll";
    #else
		const string            LIBUSB_NATIVE_LIBRARY = "libusb-1.0_Win32_Debug.dll";
    #endif
        public const int        LIBUSB_PATH_MAX = 512;
#elif WIN64
    #if !DEBUG
		const string            LIBUSB_NATIVE_LIBRARY = "libusb-1.0_x64.dll";
    #else
		const string            LIBUSB_NATIVE_LIBRARY = "libusb-1.0_x64_Debug.dll";
    #endif
        public const int        LIBUSB_PATH_MAX = 512;
#else // Linux
		const string            LIBUSB_NATIVE_LIBRARY = "libusb";
		public const int        LIBUSB_PATH_MAX = 4096 + 1;
#endif

        // usb.c
		[DllImport(LIBUSB_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern IntPtr usb_open(usb_device dev);
		
		[DllImport(LIBUSB_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int usb_close(IntPtr dev);
		
		[DllImport(LIBUSB_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int usb_get_string(IntPtr dev, int index, int langid, StringBuilder buf, int buflen);
		public static int usb_get_string(IntPtr dev, int index, int langid, StringBuilder buf)
		{
			return usb_get_string(dev, index, langid, buf, buf == null ? 0 : buf.Capacity);
		}
		
		[DllImport(LIBUSB_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int usb_get_string_simple(IntPtr dev, int index, StringBuilder buf, int buflen);
		public static int usb_get_string_simple(IntPtr dev, int index, StringBuilder buf)
		{
			return usb_get_string_simple(dev, index, buf, buf == null ? 0 : buf.Capacity);
		}
		
		// descriptors.c 
		[DllImport(LIBUSB_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int usb_get_descriptor_by_endpoint(IntPtr udev, int ep, byte type, byte index, IntPtr buf, int size);
		
		[DllImport(LIBUSB_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int usb_get_descriptor(IntPtr udev, byte type, byte index, IntPtr buf, int size);
		
		// <arch>.c 
		[DllImport(LIBUSB_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int usb_bulk_write(IntPtr dev, int ep, byte[] bytes, int size, int timeout);
		public static int usb_bulk_write(IntPtr dev, int ep, byte[] bytes, int timeout)
		{
			return usb_bulk_write(dev, ep, bytes, bytes == null ? 0 : bytes.Length, timeout);
		}
		
		[DllImport(LIBUSB_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int usb_bulk_read(IntPtr dev, int ep, byte[] bytes, int size, int timeout);
		public static int usb_bulk_read(IntPtr dev, int ep, byte[] bytes, int timeout)
		{
			return usb_bulk_read(dev, ep, bytes, bytes == null ? 0 : bytes.Length, timeout);
		}
		
		[DllImport(LIBUSB_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int usb_interrupt_write(IntPtr dev, int ep, byte[] bytes, int size, int timeout);
		public static int usb_interrupt_write(IntPtr dev, int ep, byte[] bytes, int timeout)
		{
			return usb_interrupt_write(dev, ep, bytes, bytes == null ? 0 : bytes.Length, timeout);
		}
		
		[DllImport(LIBUSB_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int usb_interrupt_read(IntPtr dev, int ep, byte[] bytes, int size, int timeout);
		public static int usb_interrupt_read(IntPtr dev, int ep, byte[] bytes, int timeout)
		{
			return usb_interrupt_read(dev, ep, bytes, bytes == null ? 0 : bytes.Length, timeout);
		}
		
		[DllImport(LIBUSB_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int usb_control_msg(IntPtr dev, int requesttype, int request, int value, int index, byte[] bytes, int size, int timeout);
		public static int usb_control_msg(IntPtr dev, int requesttype, int request, int value, byte[] bytes, int timeout)
		{
			return usb_control_msg(dev, requesttype, request, value, 0, bytes, bytes == null ? 0 : bytes.Length, timeout);
		}
		
		[DllImport(LIBUSB_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int usb_set_configuration(IntPtr dev, int configuration);
		
		[DllImport(LIBUSB_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int usb_claim_interface(IntPtr dev, int interfaceNum);
		
		[DllImport(LIBUSB_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int usb_release_interface(IntPtr dev, int interfaceNum);
		
		[DllImport(LIBUSB_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int usb_set_altinterface(IntPtr dev, int alternate);
		
		[DllImport(LIBUSB_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int usb_resetep(IntPtr dev, uint ep);
		
		[DllImport(LIBUSB_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int usb_clear_halt(IntPtr dev, uint ep);
		
		[DllImport(LIBUSB_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int usb_reset(IntPtr dev);
		
		[DllImport(LIBUSB_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern string usb_strerror();
		
		[DllImport(LIBUSB_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern void usb_init();
		
		[DllImport(LIBUSB_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern void usb_set_debug(int level);
		
		[DllImport(LIBUSB_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int usb_find_busses();
		
		[DllImport(LIBUSB_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int usb_find_devices();
		
		[DllImport(LIBUSB_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern usb_device usb_device(IntPtr dev);
		
		[DllImport(LIBUSB_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern usb_bus usb_get_busses();
	}
}
