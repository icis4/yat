using System;
using System.Collections.Generic;
using System.Text;

namespace HSR.YAT.Domain.Parser
{
	public abstract class Result
	{
	}

	public class ByteArrayResult : Result
	{
		public readonly byte[] ByteArray;

		public ByteArrayResult(byte[] byteArray)
		{
			ByteArray = byteArray;
		}
	}

	public class KeywordResult : Result
	{
		public readonly Keyword Keyword;

		public KeywordResult(Keyword keyword)
		{
			Keyword = keyword;
		}
	}
}
