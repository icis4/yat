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
// MKY Version 1.0.27
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

using System.Diagnostics.CodeAnalysis;

namespace MKY.Text.RegularExpressions
{
	/// <summary>
	/// Some common regex patters.
	/// </summary>
	public static class CommonPatterns
	{
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
