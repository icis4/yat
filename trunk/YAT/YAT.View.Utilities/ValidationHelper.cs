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
// YAT Version 2.0.1 Development
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
		public static bool ValidateTextSilently(string textToValidate, Domain.Radix defaultRadix = Domain.Parser.Parser.DefaultRadixDefault, Domain.Parser.Modes modes = Domain.Parser.Modes.AllEscapesExceptKeywords)
		{
			int invalidTextLength;
			int invalidTextStart;
			string errorMessage;                                  // Empty 'description' as error message will be ignored anyway.
			return (Model.Utilities.ValidationHelper.ValidateText("", textToValidate, out invalidTextStart, out invalidTextLength, out errorMessage, defaultRadix, modes));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static bool ValidateText(IWin32Window owner, string description, string textToValidate, out int invalidTextStart, out int invalidTextLength, Domain.Radix defaultRadix = Domain.Parser.Parser.DefaultRadixDefault, Domain.Parser.Modes modes = Domain.Parser.Modes.AllEscapesExceptKeywords)
		{
			string errorMessage;
			if (Model.Utilities.ValidationHelper.ValidateText(description, textToValidate, out invalidTextStart, out invalidTextLength, out errorMessage, defaultRadix, modes))
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
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static bool ValidateRadix(IWin32Window owner, string description, string textToValidate, Domain.Radix defaultRadix = Domain.Parser.Parser.DefaultRadixDefault, Domain.Parser.Modes modes = Domain.Parser.Modes.AllEscapesExceptKeywords)
		{
			if (Model.Utilities.ValidationHelper.ValidateText(description, textToValidate, defaultRadix, modes))
			{
				return (true);
			}
			else
			{
				var errorMessage = "This " + description + " is not valid with the current command. Clear or change the command before changing the " + description + ".";
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
