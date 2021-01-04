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
// MKY Version 1.0.28 Development
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
	public static class CommonPatterns
	{
		/// <summary>
		/// Captures integral numbers.
		/// </summary>
		public const string IntegralNumber = @"[-+]?\d+";

		/// <summary>
		/// Captures numbers in normal floating point format.
		/// </summary>
		/// <remarks>
		/// Includes integral values as well as trailing dot representation, i.e. "123", "123." and ".123".
		/// </remarks>
		public const string FloatingPointNormal = @"[-+]?(?:\d+(?:\.\d*)?|\.\d+)";

		/// <summary>
		/// Captures numbers in normal floating point format.
		/// </summary>
		/// <remarks>
		/// Includes integral values as well as trailing dot representation, i.e. "123e9", "123.e9" and ".123e9".
		/// </remarks>
		public const string FloatingPointScientific = @"[-+]?(?:\d+(?:\.\d*)?|\.\d+)[Ee][-+]?\d+";

		/// <summary>
		/// Captures numbers in normal floating point format.
		/// </summary>
		/// <remarks>
		/// Includes integral values as well as trailing dot representation, i.e. "123", "123." and ".123" as well as "123e9", "123.e9" and ".123e9".
		/// </remarks>
		public const string FloatingPointAny = @"[-+]?(?:\d+(?:\.\d*)?|\.\d+)(?:[Ee][-+]?\d+)?";

		/// <summary>
		/// Captures dates separated by spaces, e.g 01 01 2000, always including leading zeros.
		/// </summary>
		public const string DateWithSpacesAscending = @"(?:0[1-9]|[12]\d|3[01])\s(?:0[1-9]|1[012])\s(?:19|20)\d\d";

		/// <summary>
		/// Captures dates separated by spaces, e.g 2000 01 01, always including leading zeros.
		/// </summary>
		public const string DateWithSpacesDescending = @"(?:19|20)\d\d\s(?:0[1-9]|1[012])\s(?:0[1-9]|[12]\d|3[01])";

		/// <summary>
		/// Captures dates separated by spaces, e.g 2000-01-01, always including leading zeros.
		/// </summary>
		public const string DateWithHyphensDescending = @"(?:19|20)\d\d-(?:0[1-9]|1[012])-(?:0[1-9]|[12]\d|3[01])";

		/// <summary>
		/// Captures times separated by spaces, e.g 12 00 00, always including leading zeros.
		/// </summary>
		public const string TimeWithSpaces = @"(?:[01]\d|2[0-3])\s[0-5]\d\s[0-5]\d";

		/// <summary>
		/// Captures times separated by colons, e.g 12:00:00, always including leading zeros.
		/// </summary>
		public const string TimeWithColons = @"(?:[01]\d|2[0-3]):[0-5]\d:[0-5]\d";

		/// <summary>
		/// Captures '"' quoted strings, allowing escaped '\"'.
		/// </summary>
		public const string QuotedString = @"""[^""\\]*(?:\\.[^""\\]*)*""";

		/// <summary>
		/// Positive lookahead that only captures items outside quotes '"', allowing escaped '\"'.
		/// </summary>
		/// <remarks>
		/// Source: https://stackoverflow.com/questions/11502598/how-to-match-something-with-regex-that-is-not-between-two-special-characters.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop isn't able to skip URLs...")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Lookahead", Justification = "'Lookahead' is a common term for regular expressions.")]
		public const string PositiveLookaheadOutsideQuotes = @"(?=(?:(?:\\.|[^""\\])*""(?:\\.|[^""\\])*"")*(?:\\.|[^""\\])*$)";

		// (?=          Only if item is followed by...
		//   (?:
		//     [^"]*"   ...any number of non-quotes, followed by one quote, or...
		//     [^"]*"   ...the same again, ensuring an even number.
		//   )*         Any number of times (0, 2, 4 etc. quotes)...
		//   [^"]*      ...followed by only non-quotes...
		//   $          ...until the end of the string.
		// )
		//
		// Subsituting [^"] by (?:\\.|[^"\\]) to allow escaped quotes:
		//
		//     (?:      Match either...
		//       \\.    ...an escaped character,...
		//       |      ...or,...
		//       [^"\\] ...any character except quote or backslash.
		//     )
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
