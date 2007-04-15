using System;
using System.Collections.Generic;
using System.Text;

namespace HSR.YAT.Domain.IO
{
	/// <summary></summary>
	public class IOErrorEventArgs : EventArgs
	{
		public readonly string Message;

		public IOErrorEventArgs(string message)
		{
			Message = message;
		}
	}
}
