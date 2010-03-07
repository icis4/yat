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

namespace libusb.NET
{
    [Serializable]
    public class UsbException : Exception
    {
        public UsbException()
        {
        }

        public UsbException(string message)
            : base(message)
        {
        }
    }

    [Serializable]
    public class UsbDeviceAlreadyOpenException : UsbException
	{
        public UsbDeviceAlreadyOpenException()
        {
		}
	}

	[Serializable]
    public class UsbDeviceNotOpenException : UsbException
	{
        public UsbDeviceNotOpenException()
        {
		}
	}

	[Serializable]
    public class UsbNativeMethodCallException : UsbException
	{
		public readonly string Method;
        public readonly int    ReturnCode;
        public readonly string NativeMessage;

        public UsbNativeMethodCallException(string method)
            : this(method, "", 0)
        {
        }

        public UsbNativeMethodCallException(string method, string message)
            : this(method, message, 0)
        {
        }

        public UsbNativeMethodCallException(string method, string message, int returnCode)
            : base(message)
		{
            Method = method;
			ReturnCode = returnCode;
            NativeMessage = Native.Functions.libusb_strerror((Native.libusb_error)returnCode);
		}
			
		public override string ToString()
		{
            StringBuilder sb = new StringBuilder();

            sb.Append("USB exception while calling ");
            sb.AppendLine(Method);

            sb.Append("  Message : ");
            sb.AppendLine(Message);

            sb.Append("  Return  : ");
            sb.AppendLine(ReturnCode.ToString());

            sb.Append("  Native  : ");
            sb.AppendLine(NativeMessage);

            return (sb.ToString());
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
