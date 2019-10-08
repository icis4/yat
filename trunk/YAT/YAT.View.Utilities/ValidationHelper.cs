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
// YAT Version 2.3.90 Development
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

using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Windows.Forms;

using MKY.Windows.Forms;

#endregion

namespace YAT.View.Utilities
{
	/// <summary></summary>
	public static class ValidationHelper
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static bool ValidateTextSilently(string textToValidate, Domain.Parser.Modes modes, Domain.Radix defaultRadix = Domain.Parser.Parser.DefaultRadixDefault)
		{
			int invalidTextLength;
			int invalidTextStart;
			string errorMessage;                                  // Empty 'description' as error message will be ignored anyway.
			return (Domain.Utilities.ValidationHelper.ValidateText("", textToValidate, out invalidTextStart, out invalidTextLength, out errorMessage, modes, defaultRadix));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static bool ValidateText(IWin32Window owner, string description, string textToValidate, out int invalidTextStart, out int invalidTextLength, Domain.Parser.Modes modes, Domain.Radix defaultRadix = Domain.Parser.Parser.DefaultRadixDefault)
		{
			string errorMessage;
			if (Domain.Utilities.ValidationHelper.ValidateText(description, textToValidate, out invalidTextStart, out invalidTextLength, out errorMessage, modes, defaultRadix))
			{
				return (true);
			}
			else
			{
				var errorCaption = "Invalid " + description;
				return (ShowErrorMessageAndReturnFalse(owner, errorMessage, errorCaption));
			}
		}

		/// <summary></summary>
		public static bool ValidateRadix(IWin32Window owner, string description, string textToValidate, Domain.Parser.Modes modes, Domain.Radix radix)
		{
			if (Domain.Utilities.ValidationHelper.ValidateText(description, textToValidate, modes, radix))
			{
				return (true);
			}
			else
			{
				var radixName = ((Domain.RadixEx)radix).ToString();

				var sb = new StringBuilder();           // The below error text must cover the following cases:
				sb.Append("The ");                      //  > User changes the explicit default radix.
				sb.Append(     description);            //  > User activates or deactivates the explicit default radix.
				sb.Append(               " [");         //  > User changes the parse mode, e.g. re-enables escapes.
				sb.Append(                  radixName); // The error text must suit all these cases.
				sb.Append(                         @"] is not valid with """);
				sb.Append(                                            textToValidate);
				sb.Append(                                                        @""". ");
				sb.Append("Clear or change the command text to fit a ");
				sb.Append(                                           description);
				sb.Append(                                                     " of [");
				sb.Append(                                                           radixName);
				sb.Append(                                                                   "].");

				var errorMessage = sb.ToString();
				var errorCaption = "Invalid " + description;
				return (ShowErrorMessageAndReturnFalse(owner, errorMessage, errorCaption));
			}
		}

		private static bool ShowErrorMessageAndReturnFalse(IWin32Window owner, string errorMessage, string errorCaption)
		{
			MessageBoxEx.Show
			(
				owner,
				errorMessage,
				errorCaption,
				MessageBoxButtons.OK,
				MessageBoxIcon.Exclamation
			);

			return (false);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
