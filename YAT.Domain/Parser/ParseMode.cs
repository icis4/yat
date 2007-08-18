using System;
using System.Collections.Generic;
using System.Text;

namespace MKY.YAT.Domain.Parser
{
	/// <summary></summary>
	[Flags]
	public enum ParseMode
	{
		/// <summary></summary>
		Radix = 1,
		/// <summary></summary>
		Ascii = 2,
		/// <summary></summary>
		AllByteArrayResults = 3,

		/// <summary></summary>
		Keywords = 128,

		/// <summary></summary>
		All = 131,
	}
}
