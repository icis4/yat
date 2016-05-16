//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
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

namespace YAT.Model.Utilities
{
	/// <summary></summary>
	public static class ValidationHelper
	{
		/// <summary>Validation using <see cref="Domain.Radix.String"/> and <see cref="Domain.Parser.Modes.AllByteArrayResults"/>.</summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static bool ValidateText(string description, string textToValidate, out string errorMessage)
		{
			int invalidTextStart;
			return (ValidateText(description, textToValidate, out invalidTextStart, out errorMessage));
		}

		/// <summary>Validation using <see cref="Domain.Radix.String"/> and <see cref="Domain.Parser.Modes.AllByteArrayResults"/>.</summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static bool ValidateText(string description, string textToValidate, out int invalidTextStart, out string errorMessage)
		{
			int invalidTextLength;
			return (ValidateText(description, textToValidate, out invalidTextStart, out invalidTextLength, out errorMessage));
		}

		/// <summary>Validation using <see cref="Domain.Radix.String"/> and <see cref="Domain.Parser.Modes.AllByteArrayResults"/>.</summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static bool ValidateText(string description, string textToValidate, out int invalidTextStart, out int invalidTextLength, out string errorMessage)
		{
			return (ValidateText(description, textToValidate, Domain.Radix.String, Domain.Parser.Modes.AllByteArrayResults, out invalidTextStart, out invalidTextLength, out errorMessage));
		}

		/// <summary>Validation using <see cref="Domain.Radix.String"/>.</summary>
		/// <remarks>\ToDo: Remove after FR#238 has been implemented.</remarks>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static bool ValidateText(string description, string textToValidate, Domain.Parser.Modes modes, out int invalidTextStart, out int invalidTextLength, out string errorMessage)
		{
			return (ValidateText(description, textToValidate, Domain.Radix.String, modes, out invalidTextStart, out invalidTextLength, out errorMessage));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "6#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static bool ValidateText(string description, string textToValidate, Domain.Radix defaultRadix, Domain.Parser.Modes modes, out int invalidTextStart, out int invalidTextLength, out string errorMessage)
		{
			string successfullyParsed;
			if (ValidateText(description, textToValidate, defaultRadix, modes, out successfullyParsed, out errorMessage))
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
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[ModalBehavior(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		public static bool ValidateText(string description, string textToValidate, Domain.Radix defaultRadix, Domain.Parser.Modes modes, out string successfullyParsed, out string errorMessage)
		{
			bool hasSucceeded;
			Domain.Parser.FormatException formatException = new Domain.Parser.FormatException("");

			using (Domain.Parser.Parser p = new Domain.Parser.Parser(defaultRadix))
				hasSucceeded = p.TryParse(textToValidate, modes, out successfullyParsed, ref formatException);

			if (hasSucceeded)
			{
				errorMessage = null;
				return (true);
			}
			else
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("The ");
				sb.Append(     description);
				sb.Append(              @" """);
				sb.Append(                   textToValidate);
				sb.Append(                               @"""");
				if (successfullyParsed != null)
				{
					sb.Append(                              " is invalid at position ");
					sb.Append(                                                      (successfullyParsed.Length + 1).ToString(CultureInfo.CurrentCulture) + ".");
					if (successfullyParsed.Length > 0)
					{
						sb.Append(Environment.NewLine);
						sb.Append(@"Only """);
						sb.Append(         successfullyParsed);
						sb.Append(                  @""" is valid.");
					}
				}
				else
				{
					sb.Append(                         " is invalid.");
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
