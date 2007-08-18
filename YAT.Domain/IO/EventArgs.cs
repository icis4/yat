using System;
using System.Collections.Generic;
using System.Text;

namespace MKY.YAT.Domain.IO
{
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
}
