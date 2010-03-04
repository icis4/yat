// UsbException.cs
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
	/// Description of UsbException.
	/// </summary>
	[Serializable]
	public class UsbException : System.Exception
	{
		public UsbException()
		{
		}
		
		public UsbException(string msg) : base(msg)
		{
		}
		
		public override string ToString()
		{
			return "UsbException: " + base.Message;
		}
	}
	
	[Serializable]
	public class DeviceAlreadyOpenUsbException : UsbException
	{
		public DeviceAlreadyOpenUsbException() : base("USB device is already open.")
		{
		}
	}
	
	[Serializable]
	public class DeviceNotOpenUsbException : UsbException
	{
		public DeviceNotOpenUsbException() : base("USB device is not open.")
		{
		}
	}
	
	[Serializable]
	public class MethodCallUsbException : UsbException
	{
		string method;
		int    returnCode;
		
		public string Method {
			get {
				return method;
			}
		}
		
		public int ReturnCode {
			get {
				return returnCode;
			}
		}
		
		public MethodCallUsbException() : this(String.Empty, 0)
		{
		}
		
		public MethodCallUsbException(string method, int returnCode) : base(NativeMethods.usb_strerror())
		{
			this.method     = method;
			this.returnCode = returnCode;
		}
			
		public override string ToString()
		{
			return "Got exception : " + base.Message + " while calling " + method +" return code: " + returnCode;
		}
	}
}
