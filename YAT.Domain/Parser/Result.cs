using System;
using System.Collections.Generic;
using System.Text;

namespace MKY.YAT.Domain.Parser
{
	/// <summary></summary>
	public abstract class Result
	{
	}

	/// <summary></summary>
	public class ByteArrayResult : Result
	{
		/// <summary></summary>
		public readonly byte[] ByteArray;

		/// <summary></summary>
		public ByteArrayResult(byte[] byteArray)
		{
			ByteArray = byteArray;
		}
	}

	/// <summary></summary>
	public class KeywordResult : Result
	{
		/// <summary></summary>
		public readonly Keyword Keyword;

		/// <summary></summary>
		public KeywordResult(Keyword keyword)
		{
			Keyword = keyword;
		}
	}
}
