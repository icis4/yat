﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 2 Development Version 1.99.29
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

using MKY.Windows.Forms;

namespace YAT.Gui.Utilities
{
	/// <summary></summary>
	public static class Validation
	{
		/// <summary></summary>
		public static bool ValidateSequence(IWin32Window owner, string description, string textToValidate)
		{
			return (ValidateSequence(owner, description, textToValidate, Domain.Parser.ParseMode.All));
		}

		/// <summary></summary>
		public static bool ValidateSequence(IWin32Window owner, string description, string textToValidate, out int invalidTextStart, out int invalidTextLength)
		{
			string parsedText;
			if (ValidateSequence(owner, description, textToValidate, out parsedText))
			{
				invalidTextStart = -1;
				invalidTextLength = 0;
				return (true);
			}
			else
			{
				invalidTextStart = parsedText.Length;
				invalidTextLength = textToValidate.Length - invalidTextStart;
				return (false);
			}
		}

		/// <summary></summary>
		public static bool ValidateSequence(IWin32Window owner, string description, string textToValidate, out string parsedText)
		{
			return (ValidateSequence(owner, description, textToValidate, Domain.Parser.ParseMode.All, out parsedText));
		}

		/// <summary></summary>
		public static bool ValidateSequence(IWin32Window owner, string description, string textToValidate, Domain.Parser.ParseMode parseMode)
		{
			string parsedText;
			return (ValidateSequence(owner, description, textToValidate, parseMode, out parsedText));
		}

		/// <summary></summary>
		[ModalBehavior(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
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
				sb.Append(                                   (parsedText.Length + 1).ToString(CultureInfo.InvariantCulture) + ".");
				if (parsedText.Length > 0)
				{
					sb.Append(Environment.NewLine);
					sb.Append(@"Only """);
					sb.Append(         parsedText);
					sb.Append(                 @""" is valid.");
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

//==================================================================================================
// End of
// $URL$
//==================================================================================================
