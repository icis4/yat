// Device.cs
// Copyright (C) 2004 Mike Krueger
// 
// This program is free software. It is dual licensed under GNU GPL and GNU LGPL.
// See COPYING_GPL.txt and COPYING_LGPL.txt for details.
//

using System;
using System.Text;
using libusb.NET.Internal;

namespace libusb.NET
{
	/// <summary>
	/// Description of Device.
	/// </summary>
	public class Device : System.IDisposable
	{
		Descriptor descriptor;
		IntPtr     deviceHandle = IntPtr.Zero;
		
		int        timeout = 500;
		
		public Descriptor Descriptor {
			get {
				return descriptor;
			}
		}
		
		public int Timeout {
			get {
				return timeout;
			}
			set {
				timeout = value;
			}
		}
		
		public string Manufacturer {
			get {
				CheckDeviceOpen();
				if (descriptor.NativeDevice.descriptor.iManufacturer != 0) {
					StringBuilder str = new StringBuilder(256);
					int ret = NativeMethods.usb_get_string_simple(deviceHandle,  descriptor.NativeDevice.descriptor.iManufacturer, str);
					if (ret <= 0) {
						throw new UsbException("Unable to fetch manufacturer string.");
					}
					return str.ToString();
				}
				return null;
			}
		}
		
		public string Product {
			get {
				CheckDeviceOpen();
				if (descriptor.NativeDevice.descriptor.iProduct != 0) {
					StringBuilder str = new StringBuilder(256);
					int ret = NativeMethods.usb_get_string_simple(deviceHandle,  descriptor.NativeDevice.descriptor.iProduct, str);
					if (ret <= 0) {
						throw new UsbException("Unable to fetch manufacturer string.");
					}
					return str.ToString();
				}
				return null;
			}
		}
		
		public string SerialNumber {
			get {
				CheckDeviceOpen();
				if (descriptor.NativeDevice.descriptor.iSerialNumber != 0) {
					StringBuilder str = new StringBuilder(256);
					int ret = NativeMethods.usb_get_string_simple(deviceHandle,  descriptor.NativeDevice.descriptor.iSerialNumber, str);
					if (ret <= 0) {
						throw new UsbException("Unable to fetch manufacturer string.");
					}
					return str.ToString();
				}
				return null;
			}
		}
		
		internal Device(Descriptor descriptor)
		{
			this.descriptor   = descriptor;
			
			this.deviceHandle = NativeMethods.usb_open(descriptor.NativeDevice);
			if (this.deviceHandle == IntPtr.Zero) {
				throw new UsbException("Can't open device.");
			}
			/*
			int rc = NativeMethods.usb_set_configuration(deviceHandle, 1);
			if (rc < 0) {
				throw new MethodCallUsbException("usb_set_configuration", rc);
			}
			rc = NativeMethods.usb_claim_interface(deviceHandle, 0);
			if (rc < 0) {
				throw new MethodCallUsbException("usb_claim_interface", rc);
			}
			
			rc = NativeMethods.usb_set_altinterface(deviceHandle, 0);
			if (rc < 0) {
				throw new MethodCallUsbException("usb_set_altinterface", rc);
			}*/
			
		}
		
		void CheckDeviceOpen()
		{
			if (deviceHandle == IntPtr.Zero) {
				throw new DeviceNotOpenUsbException();
			}
		}
		
		public void BulkWrite(int endpoint, byte[] bytes)
		{
			CheckDeviceOpen();
			NativeMethods.usb_bulk_write(deviceHandle, endpoint, bytes, timeout);
		}
		
		public void BulkRead(int endpoint, byte[] bytes)
		{
			CheckDeviceOpen();
			NativeMethods.usb_bulk_read(deviceHandle, endpoint, bytes, timeout);
		}
		
		public void InterruptWrite(int endpoint, byte[] bytes)
		{
			CheckDeviceOpen();
			NativeMethods.usb_interrupt_write(deviceHandle, endpoint, bytes, timeout);
		}
		
		public void InterruptRead(int endpoint, byte[] bytes)
		{
			CheckDeviceOpen();
			NativeMethods.usb_interrupt_read(deviceHandle, endpoint, bytes, timeout);
		}
		
		public void SendControlMessage(int requestType, int request, int val, byte[] bytes)
		{
			CheckDeviceOpen();
			int rc = NativeMethods.usb_control_msg(deviceHandle, 
			                                       requestType, 
			                                       request, 
			                                       val, 
			                                       bytes,
			                                       timeout);
			if (rc < 0) {
				throw new MethodCallUsbException("usb_control_msg", rc);
			}
		}
		
		public void Reset()
		{
			CheckDeviceOpen();
			int rc = NativeMethods.usb_reset(deviceHandle);
			if (rc < 0) {
				throw new MethodCallUsbException("usb_reset", rc);
			}
		}
		
		public void Close()
		{
			if (deviceHandle != IntPtr.Zero) {
				int rc = NativeMethods.usb_close(deviceHandle);
				if (rc < 0) {
					throw new MethodCallUsbException("usb_close", rc);
				}
				deviceHandle = IntPtr.Zero;
			}
		}
		
		#region System.IDisposable interface implementation
		public void Dispose() 
		{
			Close();
		}
		#endregion
		
		~Device()
		{
			Dispose();
		}
	}
}
