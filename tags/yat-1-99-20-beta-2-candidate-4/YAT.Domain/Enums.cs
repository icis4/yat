using System;
using System.Collections.Generic;
using System.Text;

namespace YAT.Domain
{
	/// <summary></summary>
	public enum IORequest
	{
		/// <summary></summary>
		StartIO = MKY.IO.Serial.IORequest.Open,
		/// <summary></summary>
		StopIO = MKY.IO.Serial.IORequest.Close,
	}
}
