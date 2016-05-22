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

namespace YAT.View.Utilities
{
	/// <summary></summary>
	public static class ValidationHelper
	{
		/// <summary>Validation using <see cref="Domain.Radix.String"/> and <see cref="Domain.Parser.Modes.AllByteArrayResults"/>.</summary>
		public static bool ValidateText(IWin32Window owner, string description, string textToValidate)
		{
			int invalidTextStart;
			return (ValidateText(owner, description, textToValidate, out invalidTextStart));
		}

		/// <summary>Validation using <see cref="Domain.Radix.String"/> and <see cref="Domain.Parser.Modes.AllByteArrayResults"/>.</summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static bool ValidateText(IWin32Window owner, string description, string textToValidate, out int invalidTextStart)
		{
			int invalidTextLength;
			return (ValidateText(owner, description, textToValidate, out invalidTextStart, out invalidTextLength));
		}

		/// <summary>Validation using <see cref="Domain.Radix.String"/> and <see cref="Domain.Parser.Modes.AllByteArrayResults"/>.</summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static bool ValidateText(IWin32Window owner, string description, string textToValidate, out int invalidTextStart, out int invalidTextLength)
		{
			return (ValidateText(owner, description, textToValidate, Domain.Radix.String, Domain.Parser.Modes.AllByteArrayResults, out invalidTextStart, out invalidTextLength));
		}

		/// <summary>Validation using <see cref="Domain.Radix.String"/>.</summary>
		/// <remarks>\ToDo: Remove after FR#238 has been implemented.</remarks>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static bool ValidateText(IWin32Window owner, string description, string textToValidate, Domain.Parser.Modes modes, out int invalidTextStart, out int invalidTextLength)
		{
			return (ValidateText(owner, description, textToValidate, Domain.Radix.String, modes, out invalidTextStart, out invalidTextLength));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "6#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static bool ValidateText(IWin32Window owner, string description, string textToValidate, Domain.Radix defaultRadix, Domain.Parser.Modes modes, out int invalidTextStart, out int invalidTextLength)
		{
			string errorMessage;
			if (Model.Utilities.ValidationHelper.ValidateText(description, textToValidate, defaultRadix, modes, out invalidTextStart, out invalidTextLength, out errorMessage))
			{
				return (true);
			}
			else
			{
				MessageBoxEx.Show
				(
					owner,
					errorMessage.ToString(),
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
