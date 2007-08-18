using System;
using System.Collections.Generic;
using System.Text;

namespace MKY.YAT.Domain.Parser
{
	/// <summary></summary>
	public class FormatException : System.FormatException
	{
		/// <summary></summary>
		public FormatException(string message)
			: base
			(
			message + Environment.NewLine + Environment.NewLine +
			Parser.FormatHelp + Environment.NewLine + Environment.NewLine +
			Parser.KeywordHelp + Environment.NewLine + Environment.NewLine
			)
		{
		}
	}
}
