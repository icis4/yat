using System;
using System.Collections.Generic;
using System.Text;

namespace MKY.IO.Serial
{
	/// <summary></summary>
	public class IORequestEventArgs : EventArgs
	{
		/// <summary></summary>
		public readonly IORequest Request;

		/// <summary></summary>
		public IORequestEventArgs(IORequest request)
		{
			Request = request;
		}
	}

	/// <summary></summary>
	public class IOErrorEventArgs : EventArgs
	{
		/// <summary></summary>
		public readonly string Message;

		/// <summary></summary>
		public IOErrorEventArgs(string message)
		{
			Message = message;
		}
	}

	/// <summary></summary>
	public class SerialPortIOErrorEventArgs : IOErrorEventArgs
	{
		/// <summary></summary>
		public readonly System.IO.Ports.SerialError SerialPortError;

		/// <summary></summary>
		public SerialPortIOErrorEventArgs(string message, System.IO.Ports.SerialError serialPortError)
			: base(message)
		{
			SerialPortError = serialPortError;
		}
	}
}
