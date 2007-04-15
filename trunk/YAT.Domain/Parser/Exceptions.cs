using System;
using System.Collections.Generic;
using System.Text;

namespace HSR.YAT.Domain.Parser
{
	public class FormatException : System.FormatException
	{
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
