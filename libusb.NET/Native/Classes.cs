//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2004 Mike Krüger.
// Copyright © 2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Text;
using System.Security;
using System.Runtime.InteropServices;

/// <summary>
/// This namespace declares the native libusb elements.
/// Elements aren't documented for consistency/maintenance reasons. See libusb for documentation.
/// </summary>
namespace libusb.NET.Native
{
	/// <remarks>
	/// Must be a class to allow two levels of indirection when calling the native function.
	/// For details, see "Passing Structures" in the MSDN.
	/// </remarks>
	[StructLayout(LayoutKind.Sequential)]
	public class libusb_device 
	{
	    public IntPtr refcnt_lock;
	    public int refcnt;

	    public IntPtr ctx;

	    public byte bus_number;
	    public byte device_address;
	    public byte num_configurations;

		public IntPtr prev = IntPtr.Zero;
		public IntPtr next = IntPtr.Zero;

	    public ulong session_data;
	    public byte[] os_priv;
	}

	/// <remarks>
	/// Must be a class to allow two levels of indirection when calling the native function.
	/// For details, see "Passing Structures" in the MSDN.
	/// </remarks>
	[StructLayout(LayoutKind.Sequential)]
	public class libusb_device_handle
	{
	    public IntPtr claimed_interfaces_lock;
	    public ulong claimed_interfaces;

		public IntPtr prev = IntPtr.Zero;
		public IntPtr next = IntPtr.Zero;

	    public libusb_device dev;
	    public byte[] os_priv;
	}

	/// <remarks>
	/// Must be a class to allow two levels of indirection when calling the native function.
	/// For details, see "Passing Structures" in the MSDN.
	/// </remarks>
	[StructLayout(LayoutKind.Sequential)]
	public class libusb_iso_packet_descriptor
	{
	    uint length;
	    uint actual_length;
	    libusb_transfer_status status;
	}

	/// <remarks>
	/// Must be a class to allow two levels of indirection when calling the native function.
	/// For details, see "Passing Structures" in the MSDN.
	/// </remarks>
	[StructLayout(LayoutKind.Sequential)]
	public class libusb_transfer
	{
		public libusb_device_handle dev_handle;
	    public byte flags;
	    public byte endpoint;
	    public byte type;
	    public uint timeout;
	    public libusb_transfer_status status;
	    public int length;
	    public int actual_length;
	    public libusb_transfer_cb_fn callback;
	    public object user_data;
	    public byte[] buffer;
	    public int num_iso_packets;
	    public libusb_iso_packet_descriptor[] iso_packet_desc;
	}

	/// <remarks>
	/// Must be a class to allow two levels of indirection when calling the native function.
	/// For details, see "Passing Structures" in the MSDN.
	/// </remarks>
	[StructLayout(LayoutKind.Sequential)]
	public class libusb_pollfd
	{
		public int fd;
		public short events;
	}

	/// <remarks>
	/// Must be a class to allow two levels of indirection when calling the native function.
	/// For details, see "Passing Structures" in the MSDN.
	/// </remarks>
	[StructLayout(LayoutKind.Sequential)]
	public class libusb_device_descriptor
	{
		public byte   bLength;
		public byte   bDescriptorType;
		public ushort bcdUSB;
		public byte   bDeviceClass;
		public byte   bDeviceSubClass;
		public byte   bDeviceProtocol;
		public byte   bMaxPacketSize0;
		public ushort idVendor;
		public ushort idProduct;
		public ushort bcdDevice;
		public byte   iManufacturer;
		public byte   iProduct;
		public byte   iSerialNumber;
		public byte   bNumConfigurations;
	}

	/// <remarks>
	/// Must be a class to allow two levels of indirection when calling the native function.
	/// For details, see "Passing Structures" in the MSDN.
	/// </remarks>
	[StructLayout(LayoutKind.Sequential)]
	public class libusb_endpoint_descriptor
	{
		public byte   bLength;
		public byte   bDescriptorType;
		public byte   bEndpointAddress;
		public byte   bmAttributes;
		public byte   wMaxPacketSize;
		public byte   bInterval;
		public byte   bRefresh;
		public byte   bSynchAddress;
		public string extra;
		public int    extra_length;
	}

	/// <remarks>
	/// Must be a class to allow two levels of indirection when calling the native function.
	/// For details, see "Passing Structures" in the MSDN.
	/// </remarks>
	[StructLayout(LayoutKind.Sequential)]
	public class libusb_interface_descriptor
	{
	    public byte   bLength;
	    public byte   bDescriptorType;
	    public byte   bInterfaceNumber;
	    public byte   bAlternateSetting;
	    public byte   bNumEndpoints;
	    public byte   bInterfaceClass;
	    public byte   bInterfaceSubClass;
	    public byte   bInterfaceProtocol;
	    public byte   iInterface;
	    public libusb_endpoint_descriptor endpoint;
	    public string extra;
	    public int    extra_length;
	}

	/// <remarks>
	/// Must be a class to allow two levels of indirection when calling the native function.
	/// For details, see "Passing Structures" in the MSDN.
	/// </remarks>
	[StructLayout(LayoutKind.Sequential)]
	public class libusb_interface
	{
	    libusb_interface_descriptor altsetting;
	    int num_altsetting;
	}

	/// <remarks>
	/// Must be a class to allow two levels of indirection when calling the native function.
	/// For details, see "Passing Structures" in the MSDN.
	/// </remarks>
	[StructLayout(LayoutKind.Sequential)]
	public class libusb_config_descriptor
	{
	    public byte  bLength;
	    public byte  bDescriptorType;
	    public ushort wTotalLength;
	    public byte  bNumInterfaces;
	    public byte  bConfigurationValue;
	    public byte  iConfiguration;
	    public byte  bmAttributes;
	    public byte  MaxPower;
	    public libusb_interface iface;
	    public byte[] extra;
	    public int extra_length;
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
