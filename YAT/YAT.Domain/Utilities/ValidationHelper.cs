//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT Version 2.1.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
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

using MKY.Windows.Forms;

#endregion

namespace YAT.Domain.Utilities
{
	/// <summary></summary>
	public static class ValidationHelper
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static bool ValidateText(string description, string textToValidate, Parser.Modes modes, Radix defaultRadix = Radix.String)
		{
			string errorMessage;
			return (ValidateText(description, textToValidate, out errorMessage, modes, defaultRadix));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static bool ValidateText(string description, string textToValidate, out string errorMessage, Parser.Modes modes, Radix defaultRadix = Radix.String)
		{
			int invalidTextStart;
			int invalidTextLength;
			return (ValidateText(description, textToValidate, out invalidTextStart, out invalidTextLength, out errorMessage, modes, defaultRadix));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static bool ValidateText(string description, string textToValidate, out int invalidTextStart, out int invalidTextLength, out string errorMessage, Parser.Modes modes, Radix defaultRadix = Radix.String)
		{
			string successfullyParsed;
			if (ValidateText(description, textToValidate, out successfullyParsed, out errorMessage, modes, defaultRadix))
			{
				invalidTextStart = -1;
				invalidTextLength = 0;
				return (true);
			}
			else
			{
				invalidTextStart = successfullyParsed.Length;
				invalidTextLength = textToValidate.Length - invalidTextStart;
				return (false);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[ModalBehaviorContract(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		public static bool ValidateText(string description, string textToValidate, out string successfullyParsed, out string errorMessage, Parser.Modes modes, Radix defaultRadix = Radix.String)
		{
			bool hasSucceeded;
			var formatException = new Parser.FormatException("");

			using (var p = new Parser.Parser(modes))
				hasSucceeded = p.TryParse(textToValidate, out successfullyParsed, ref formatException, defaultRadix);

			if (hasSucceeded)
			{
				errorMessage = null;
				return (true);
			}
			else
			{
				var sb = new StringBuilder();
				sb.Append("The ");
				sb.Append(     description);
				sb.Append(              @" """);
				sb.Append(                   textToValidate);
				sb.Append(                               @"""");
				if (successfullyParsed != null)
				{
					if (successfullyParsed.Length <= textToValidate.Length)
					{
						sb.Append(                          " is incomplete");
					}
					else
					{
						sb.Append(                          " is invalid at position ");
						sb.Append(                                                  (successfullyParsed.Length + 1).ToString(CultureInfo.CurrentCulture));
						sb.Append(                                                                                                                     ".");
						if (successfullyParsed.Length > 0)
						{
							sb.Append(Environment.NewLine);
							sb.Append(@"Only """);
							sb.Append(         successfullyParsed);
							sb.Append(                         @""" is valid.");
						}
					}
				}
				else
				{
					sb.Append(                              " is invalid.");
				}

				if (!string.IsNullOrEmpty(formatException.Message))
				{
					sb.Append(Environment.NewLine);
					sb.Append(formatException.Message);
				}

				errorMessage = sb.ToString();
				return (false);
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
