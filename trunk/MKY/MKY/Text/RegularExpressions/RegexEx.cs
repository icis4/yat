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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace MKY.Text.RegularExpressions
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class RegexEx
	{
		#region LikelyContains
		//------------------------------------------------------------------------------------------
		// LikelyContains
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Determines whether <paramref name="s"/> likely contains any regular expression typical pattern.
		/// </summary>
		public static bool LikelyContainsAnyPattern(string s)
		{
			if (LikelyContainsCharacterGroupPattern(s))
				return (true);

			if (LikelyContainsBasicCharacterClassPattern(s))
				return (true);

			if (LikelyContainsUnicodeCharacterClassPattern(s))
				return (true);

			if (LikelyContainsAnchorPattern(s))
				return (true);

			if (LikelyContainsOtherPattern(s))
				return (true);

			return (false);
		}

		/// <summary>
		/// Determines whether <paramref name="s"/> likely contains a regular expression character group pattern.
		/// </summary>
		public static bool LikelyContainsCharacterGroupPattern(string s)
		{
			if (Regex.IsMatch(s, @"\[\w+\]")) // Group, e.g. [aeiou].
				return (true);

			if (Regex.IsMatch(s, @"\[\^\w+\]")) // Negation, e.g. [^aeiou].
				return (true);

			if (Regex.IsMatch(s, @"\[\w+-\w+\]")) // Range, e.g. [a-z].
				return (true);

			return (false);
		}

		/// <summary>
		/// Determines whether <paramref name="s"/> likely contains a basic regular expression character class.
		/// </summary>
		public static bool LikelyContainsBasicCharacterClassPattern(string s)
		{
			string[] classes = { @"\w", @"\W", @"\s", @"\S", @"\d", @"\D" };
			return (StringEx.ContainsAny(s, classes));
		}

		/// <summary>
		/// Determines whether <paramref name="s"/> likely contains a regular expression Unicode character class pattern.
		/// </summary>
		public static bool LikelyContainsUnicodeCharacterClassPattern(string s)
		{
			if (Regex.IsMatch(s, @"\\p\{[A-Za-z]+\}")) // Category, e.g. \p{Lu}.
				return (true);

			if (Regex.IsMatch(s, @"\\P\{[A-Za-z]+\}")) // Negation, e.g. \P{Lu}.
				return (true);

			return (false);
		}

		/// <summary>
		/// Determines whether <paramref name="s"/> likely contains a regular expression anchor pattern.
		/// </summary>
		public static bool LikelyContainsAnchorPattern(string s)
		{
			string[] anchors = { "^", "$", @"\A", @"\Z", @"\z", @"\G", @"\b", @"\B" };
			return (StringEx.ContainsAny(s, anchors));
		}

		/// <summary>
		/// Determines whether <paramref name="s"/> likely contains any other regular expression typical pattern.
		/// </summary>
		public static bool LikelyContainsOtherPattern(string s)
		{
			string[] anchors = { "*?", "+?", "??", "}?", "(?" };
			return (StringEx.ContainsAny(s, anchors));
		}

		#endregion

		#region TryValidatePattern
		//------------------------------------------------------------------------------------------
		// TryValidatePattern
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Validates the given regular expression pattern.
		/// </summary>
		public static bool TryValidatePattern(string pattern)
		{
			string messageOnFailure;
			return (TryValidatePattern(pattern, out messageOnFailure));
		}

		/// <summary>
		/// Validates the given regular expression pattern.
		/// </summary>
		public static bool TryValidatePattern(string pattern, out string messageOnFailure)
		{
			try
			{
				var dummyRegexToProbePattern = new Regex(pattern);
				UnusedLocal.PreventAnalysisWarning(dummyRegexToProbePattern, "Dummy variable improves code readability.");

				messageOnFailure = null;
				return (true);
			}
			catch (ArgumentException ex)
			{
				messageOnFailure = ex.Message;
				return (false);
			}
		}

		/// <summary>
		/// Validates the given regular expression pattern.
		/// </summary>
		public static bool TryValidatePattern(string pattern, RegexOptions options)
		{
			string messageOnFailure;
			return (TryValidatePattern(pattern, options, out messageOnFailure));
		}

		/// <summary>
		/// Validates the given regular expression pattern.
		/// </summary>
		public static bool TryValidatePattern(string pattern, RegexOptions options, out string messageOnFailure)
		{
			try
			{
				var dummyRegexToProbePattern = new Regex(pattern, options);
				UnusedLocal.PreventAnalysisWarning(dummyRegexToProbePattern, "Dummy variable improves code readability.");

				messageOnFailure = null;
				return (true);
			}
			catch (ArgumentException ex)
			{
				messageOnFailure = ex.Message;
				return (false);
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
