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
// MKY Version 1.0.29
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Diagnostics.CodeAnalysis;

namespace MKY.Text.RegularExpressions
{
	/// <summary>
	/// Some common regex patters.
	/// </summary>
	public static class CommonPatternsWithSubCaptures
	{
		/// <summary>
		/// Captures dates separated by spaces, e.g 01 01 2000, always including leading zeros.
		/// </summary>
		public const string DateWithSpacesAscending = @"(0[1-9]|[12]\d|3[01])\s(0[1-9]|1[012])\s((?:19|20)\d\d)";

		/// <summary>
		/// Captures dates separated by spaces, e.g 2000 01 01, always including leading zeros.
		/// </summary>
		public const string DateWithSpacesDescending = @"((?:19|20)\d\d)\s(0[1-9]|1[012])\s(0[1-9]|[12]\d|3[01])";

		/// <summary>
		/// Captures dates separated by spaces, e.g 2000-01-01, always including leading zeros.
		/// </summary>
		public const string DateWithHyphensDescending = @"((?:19|20)\d\d)-(0[1-9]|1[012])-(0[1-9]|[12]\d|3[01])";

		/// <summary>
		/// Captures times separated by spaces, e.g 12 00 00, always including leading zeros.
		/// </summary>
		public const string TimeWithSpaces = @"([01]\d|2[0-3])\s([0-5]\d)\s([0-5]\d)";

		/// <summary>
		/// Captures times separated by colons, e.g 12:00:00, always including leading zeros.
		/// </summary>
		public const string TimeWithColons = @"([01]\d|2[0-3]):([0-5]\d):([0-5]\d)";
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
