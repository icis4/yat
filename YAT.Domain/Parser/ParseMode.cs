using System;
using System.Collections.Generic;
using System.Text;

namespace HSR.YAT.Domain.Parser
{
	[Flags]
	public enum ParseMode
	{
		Radix = 1,
		Ascii = 2,
		AllByteArrayResults = 3,

		Keywords = 128,

		All = 131,
	}
}
