//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 3 Development Version 1.99.31
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2013 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

using MKY.Windows.Forms;

#endregion

namespace YAT.Gui.Utilities
{
	/// <summary></summary>
	public static class Validation
	{
		/// <summary></summary>
		public static bool ValidateSequence(IWin32Window owner, string description, string textToValidate, Domain.Parser.Modes modes)
		{
			string parsedText;
			return (ValidateSequence(owner, description, textToValidate, modes, out parsedText));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static bool ValidateSequence(IWin32Window owner, string description, string textToValidate, Domain.Parser.Modes modes, out int invalidTextStart, out int invalidTextLength)
		{
			string parsedText;
			if (ValidateSequence(owner, description, textToValidate, modes, out parsedText))
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
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[ModalBehavior(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		public static bool ValidateSequence(IWin32Window owner, string description, string textToValidate, Domain.Parser.Modes modes, out string parsedText)
		{
			Domain.Parser.Parser p = new Domain.Parser.Parser();
			Domain.Parser.FormatException formatException = new Domain.Parser.FormatException("");
			if (p.TryParse(textToValidate, modes, out parsedText, ref formatException))
			{
				return (true);
			}
			else
			{
				StringBuilder sb = new StringBuilder();
				sb.Append(description);
				sb.Append(         @" """);
				sb.Append(              textToValidate);
				sb.Append(                          @"""");
				if (parsedText != null)
				{
					sb.Append(                         " is invalid at position ");
					sb.Append(                                                 (parsedText.Length + 1).ToString(CultureInfo.InvariantCulture) + ".");
					if (parsedText.Length > 0)
					{
						sb.Append(Environment.NewLine);
						sb.Append(@"Only """);
						sb.Append(         parsedText);
						sb.Append(                  @""" is valid.");
					}
				}
				else
				{
					sb.Append(                         " is invalid.");
				}

				if (formatException.Message.Length > 0)
				{
					sb.Append(Environment.NewLine);
					sb.Append(formatException.Message);
				}

				MessageBoxEx.Show
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
