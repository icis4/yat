using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace HSR.YAT.Gui
{
	public static class Validation
	{
		public static bool ValidateSequence(IWin32Window owner, string description, string textToValidate)
		{
			return (ValidateSequence(owner, description, textToValidate, Domain.Parser.ParseMode.All));
		}

		public static bool ValidateSequence(IWin32Window owner, string description, string textToValidate, Domain.Parser.ParseMode parseMode)
		{
			string parsedText;
			return (ValidateSequence(owner, description, textToValidate, parseMode, out parsedText));
		}

		public static bool ValidateSequence(IWin32Window owner, string description, string textToValidate, Domain.Parser.ParseMode parseMode, out string parsedText)
		{
			Domain.Parser.Parser p = new Domain.Parser.Parser();
			Domain.Parser.FormatException formatException = new Domain.Parser.FormatException("");
			if (p.TryParse(textToValidate, parseMode, out parsedText, ref formatException))
			{
				return (true);
			}
			else
			{
				StringBuilder sb = new StringBuilder();
				sb.Append(description);
				sb.Append(          " is invalid at position ");
				sb.Append(                                   (parsedText.Length + 1).ToString() + ".");
				if (parsedText.Length > 0)
				{
					sb.Append(Environment.NewLine);
					sb.Append("Only \"");
					sb.Append(        parsedText);
					sb.Append(                "\" is valid");
				}
				if (formatException.Message.Length > 0)
				{
					sb.Append(Environment.NewLine);
					sb.Append(Environment.NewLine);
					sb.Append("Detailed message:");
					sb.Append(Environment.NewLine);
					sb.Append(Environment.NewLine);
					sb.Append(formatException.Message);
				}
				MessageBox.Show
					(
					owner,
					sb.ToString(),
					"Invalid " + description,
					MessageBoxButtons.OK,
					MessageBoxIcon.Exclamation
					);
				return (false);
			}
		}
	}
}
