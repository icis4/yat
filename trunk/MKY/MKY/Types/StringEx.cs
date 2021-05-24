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
// MKY Version 1.0.30
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;

// This code is intentionally placed into the MKY namespace even though the file is located in
// MKY.Types for consistency with the System namespace.
namespace MKY
{
	/// <summary>
	/// <see cref="String"/>/<see cref="string"/> utility methods.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class StringEx
	{
		/// <summary>
		/// An invalid index is represented by -1.
		/// </summary>
		public const int InvalidIndex = -1;

		/// <summary>
		/// Text ellipsis which are "..." by definition.
		/// </summary>
		public const string Ellipsis = "...";

		/// <remarks>
		/// 16 is an arbitrary value.
		/// </remarks>
		private const int ListInitialCapacityDefault = 16;

		/// <summary>
		/// Compares two specified <see cref="string"/> objects ignoring culture and case.
		/// The method returns an integer that indicates the relationship of the two
		/// <see cref="string"/> objects to one another in the sort order.
		/// </summary>
		/// <param name="strA">The first <see cref="string"/> object.</param>
		/// <param name="strB">The second <see cref="string"/> object.</param>
		/// <returns>
		/// A 32-bit signed integer indicating the lexical relationship between the two comparands.
		/// Value Condition:
		///  - Less than zero <paramref name="strA"/> is less than <paramref name="strB"/>.
		///  - Zero <paramref name="strA"/> equals <paramref name="strB"/>.
		///  - Greater than zero <paramref name="strA"/> is greater than <paramref name="strB"/>.
		/// </returns>
		public static int CompareOrdinalIgnoreCase(string strA, string strB)
		{
			return (string.Compare(strA, strB, StringComparison.OrdinalIgnoreCase));
		}

		#region Equals
		//------------------------------------------------------------------------------------------
		// Equals
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Compares two specified <see cref="string"/> objects ignoring culture.
		/// </summary>
		public static bool EqualsOrdinal(string strA, string strB)
		{
			return (string.CompareOrdinal(strA, strB) == 0);
		}

		/// <summary>
		/// Compares two specified <see cref="string"/> arrays ignoring culture.
		/// </summary>
		/// <remarks>
		/// There are overloads for <see cref="string"/> arrays as well as
		/// <see cref="IEnumerable{T}"/> for optimal performance.
		/// </remarks>
		public static bool ValuesEqualOrdinal(string[] strA, string[] strB)
		{
			if (ReferenceEquals(strA, strB)) return (true);
			if (ReferenceEquals(strA, null)) return (false);
			if (ReferenceEquals(strB, null)) return (false);

			if (strA.Length == strB.Length)
			{
				for (int i = 0; i < strA.Length; i++)
				{
					if (!EqualsOrdinal(strA[i], strB[i]))
						return (false); // No match.
				}

				return (true); // Match.
			}
			else
			{
				return (false); // No match.
			}
		}

		/// <summary>
		/// Compares two specified <see cref="string"/> enumerables ignoring culture.
		/// </summary>
		/// <remarks>
		/// There are overloads for <see cref="string"/> arrays as well as
		/// <see cref="IEnumerable{T}"/> for optimal performance.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'enumerables' is a correct English term for multiple 'enumerable', right?")]
		public static bool ItemsEqualOrdinal(IEnumerable<string> strA, IEnumerable<string> strB)
		{
			if (ReferenceEquals(strA, strB)) return (true);
			if (ReferenceEquals(strA, null)) return (false);
			if (ReferenceEquals(strB, null)) return (false);

			if (strA.Count() == strB.Count())
			{
				for (int i = 0; i < strA.Count(); i++)
				{
					if (!EqualsOrdinal(strA.ElementAt(i), strB.ElementAt(i)))
						return (false); // No match.
				}

				return (true); // Match.
			}
			else
			{
				return (false); // No match.
			}
		}

		/// <summary>
		/// Compares two specified <see cref="string"/> objects ignoring culture and case.
		/// </summary>
		public static bool EqualsOrdinalIgnoreCase(string strA, string strB)
		{
			return (string.Compare(strA, strB, StringComparison.OrdinalIgnoreCase) == 0);
		}

		/// <summary>
		/// Compares two specified <see cref="string"/> arrays ignoring culture and case.
		/// </summary>
		/// <remarks>
		/// There are overloads for <see cref="string"/> arrays as well as
		/// <see cref="IEnumerable{T}"/> for optimal performance.
		/// </remarks>
		public static bool ValuesEqualOrdinalIgnoreCase(string[] strA, string[] strB)
		{
			if (ReferenceEquals(strA, strB)) return (true);
			if (ReferenceEquals(strA, null)) return (false);
			if (ReferenceEquals(strB, null)) return (false);

			if (strA.Length == strB.Length)
			{
				for (int i = 0; i < strA.Length; i++)
				{
					if (!EqualsOrdinalIgnoreCase(strA[i], strB[i]))
						return (false); // No match.
				}

				return (true); // Match.
			}
			else
			{
				return (false); // No match.
			}
		}

		/// <summary>
		/// Compares two specified <see cref="string"/> enumerables ignoring culture and case.
		/// </summary>
		/// <remarks>
		/// There are overloads for <see cref="string"/> arrays as well as
		/// <see cref="IEnumerable{T}"/> for optimal performance.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'enumerables' is a correct English term for multiple 'enumerable', right?")]
		public static bool ItemsEqualOrdinalIgnoreCase(IEnumerable<string> strA, IEnumerable<string> strB)
		{
			if (ReferenceEquals(strA, strB)) return (true);
			if (ReferenceEquals(strA, null)) return (false);
			if (ReferenceEquals(strB, null)) return (false);

			if (strA.Count() == strB.Count())
			{
				for (int i = 0; i < strA.Count(); i++)
				{
					if (!EqualsOrdinalIgnoreCase(strA.ElementAt(i), strB.ElementAt(i)))
						return (false); // No match.
				}

				return (true); // Match.
			}
			else
			{
				return (false); // No match.
			}
		}

		/// <summary>
		/// Compares whether <paramref name="strA"/> matches any <paramref name="strB"/>.
		/// </summary>
		public static bool EqualsAnyOrdinalIgnoreCase(string strA, params string[] strB)
		{
			return (EqualsAnyOrdinalIgnoreCase(strA, (IEnumerable<string>)strB));
		}

		/// <summary>
		/// Compares whether <paramref name="strA"/> matches any <paramref name="strB"/>.
		/// </summary>
		public static bool EqualsAnyOrdinalIgnoreCase(string strA, IEnumerable<string> strB)
		{
			foreach (string str in strB)
			{
				if (EqualsOrdinalIgnoreCase(strA, str))
					return (true); // Match.
			}

			return (false); // No match.
		}

		#endregion

		#region Count
		//------------------------------------------------------------------------------------------
		// Count
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Counts the number of <paramref name="countChars"/> to the left of <paramref name="str"/>.
		/// </summary>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "chars", Justification = "Parameter naming as similar string methods.")]
		public static int CountLeft(string str, params char[] countChars)
		{
			if (str == null)
				throw (new ArgumentNullException(nameof(str)));

			if (countChars != null)
			{
				int i = 0;
				while (i < str.Length)
				{
					bool found = false;
					foreach (char c in countChars)
					{
						if (str[i] == c)
						{
							found = true;
							break; // Immediately break once a matching character has been found.
						}
					}

					if (found)
						i++;
					else
						break;
				}

				return (i);
			}
			else
			{
				return (0);
			}
		}

		/// <summary>
		/// Counts the number of <paramref name="countChars"/> to the right of <paramref name="str"/>.
		/// </summary>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "chars", Justification = "Parameter naming as similar string methods.")]
		public static int CountRight(string str, params char[] countChars)
		{
			if (str == null)
				throw (new ArgumentNullException(nameof(str)));

			if (countChars != null)
			{
				int i = str.Length;
				while (i > 0)
				{
					bool found = false;
					foreach (char c in countChars)
					{
						if (str[i - 1] == c)
						{
							found = true;
							break; // Immediately break once a matching character has been found.
						}
					}

					if (found)
						i--;
					else
						break;
				}

				return (str.Length - i);
			}
			else
			{
				return (0);
			}
		}

		#endregion

		#region Contains
		//------------------------------------------------------------------------------------------
		// Contains
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Determines how many occurrences of <paramref name="value"/> are contained in <paramref name="str"/>.
		/// </summary>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
		public static int ContainingCount(string str, string value, StringComparison comparisonType)
		{
			if (str == null)
				throw (new ArgumentNullException(nameof(str)));

			int count = 0;

			int i = 0;
			while ((i = str.IndexOf(value, i, comparisonType)) >= 0)
			{
				i += value.Length;
				count++;
			}

			return (count);
		}

		/// <summary>
		/// Determines how many occurrences of <paramref name="value"/> are contained in <paramref name="str"/>.
		/// </summary>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
		public static int ContainingWholeWordCount(string str, string value, StringComparison comparisonType)
		{
			if (str == null)
				throw (new ArgumentNullException(nameof(str)));

			int count = 0;

			int i = 0;
			while ((i = IndexOfWholeWord(str, value, i, comparisonType)) >= 0)
			{
				i += value.Length;
				count++;
			}

			return (count);
		}

		/// <summary>
		/// Determines whether <paramref name="str"/> contains any of the <paramref name="anyOf"/>.
		/// </summary>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		public static bool ContainsAny(string str, char[] anyOf)
		{
			if (str == null)
				throw (new ArgumentNullException(nameof(str)));

			return (str.IndexOfAny(anyOf) >= 0);
		}

		/// <summary>
		/// Determines whether <paramref name="str"/> matches one of the specified <paramref name="values"/>.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <param name="values">The strings to compare with.</param>
		/// <returns>true if <paramref name="str"/> matches the beginning of a comparing string; otherwise, false.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		/// <exception cref="ArgumentNullException">One of the <paramref name="values"/> is null.</exception>
		public static bool ContainsAny(string str, params string[] values)
		{
			return (ContainsAny(str, (IEnumerable<string>)values));
		}

		/// <summary>
		/// Determines whether <paramref name="str"/> matches one of the specified <paramref name="values"/>.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <param name="values">The strings to compare with.</param>
		/// <returns>true if <paramref name="str"/> matches the beginning of a comparing string; otherwise, false.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		/// <exception cref="ArgumentNullException">One of the <paramref name="values"/> is null.</exception>
		public static bool ContainsAny(string str, IEnumerable<string> values)
		{
			if (str == null)
				throw (new ArgumentNullException(nameof(str)));

			foreach (string v in values)
			{
				if (str.Contains(v))
					return (true); // Match.
			}

			return (false); // No match.
		}

		#endregion

		#region StartsWith/EndsWith
		//------------------------------------------------------------------------------------------
		// StartsWith/EndsWith
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Determines whether the beginning of <paramref name="str"/> matches the specified <paramref name="value"/>.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <param name="value">The string to compare with.</param>
		/// <returns>true if <paramref name="str"/> matches the beginning of the comparing string; otherwise, false.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
		public static bool StartsWithOrdinal(string str, string value)
		{
			if (str == null)
				throw (new ArgumentNullException(nameof(str)));

			return (str.StartsWith(value, StringComparison.Ordinal));
		}

		/// <summary>
		/// Determines whether the beginning of <paramref name="str"/> matches the specified <paramref name="value"/>.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <param name="value">The string to compare with.</param>
		/// <returns>true if <paramref name="str"/> matches the beginning of the comparing string; otherwise, false.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
		public static bool StartsWithOrdinalIgnoreCase(string str, string value)
		{
			if (str == null)
				throw (new ArgumentNullException(nameof(str)));

			return (str.StartsWith(value, StringComparison.OrdinalIgnoreCase));
		}

		/// <summary>
		/// Determines whether the beginning of <paramref name="str"/> matches one of the specified <paramref name="values"/>.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <param name="values">The strings to compare with.</param>
		/// <returns>true if <paramref name="str"/> matches the beginning of a comparing string; otherwise, false.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		/// <exception cref="ArgumentNullException">One of the <paramref name="values"/> is null.</exception>
		public static bool StartsWithAny(string str, params string[] values)
		{
			return (StartsWithAny(str, values, false, null));
		}

		/// <summary>
		/// Determines whether the beginning of <paramref name="str"/> matches one of the specified <paramref name="values"/>.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <param name="values">The strings to compare with.</param>
		/// <returns>true if <paramref name="str"/> matches the beginning of a comparing string; otherwise, false.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		/// <exception cref="ArgumentNullException">One of the <paramref name="values"/> is null.</exception>
		public static bool StartsWithAny(string str, IEnumerable<string> values)
		{
			return (StartsWithAny(str, values, false, null));
		}

		/// <summary>
		/// Determines whether the beginning of <paramref name="str"/> matches one of the specified <paramref name="values"/>
		/// when compared using the specified culture.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <param name="values">The strings to compare with.</param>
		/// <param name="ignoreCase">true to ignore case when comparing this string and value; otherwise, false.</param>
		/// <param name="culture">Cultural information that determines how this string and value are compared. If culture is null, the current culture is used.</param>
		/// <returns>true if <paramref name="str"/> matches the beginning of a comparing string; otherwise, false.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		/// <exception cref="ArgumentNullException">One of the <paramref name="values"/> is null.</exception>
		public static bool StartsWithAny(string str, IEnumerable<string> values, bool ignoreCase, CultureInfo culture)
		{
			if (str == null)
				throw (new ArgumentNullException(nameof(str)));

			foreach (string v in values)
			{
				if (str.StartsWith(v, ignoreCase, culture))
					return (true); // Match.
			}

			return (false); // No match.
		}

		/// <summary>
		/// Determines whether the beginning of <paramref name="str"/> matches one of the specified <paramref name="values"/>
		/// when compared using the specified comparison type.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <param name="values">The strings to compare with.</param>
		/// <param name="comparisonType">One of the <see cref="StringComparison"/> values that determines how the strings and the value are compared.</param>
		/// <returns>true if <paramref name="str"/> matches the beginning of a comparing string; otherwise, false.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		/// <exception cref="ArgumentNullException">One of the <paramref name="values"/> is null.</exception>
		/// <exception cref="ArgumentException">comparisonType is not a <see cref="StringComparison"/> value.</exception>
		public static bool StartsWithAny(string str, IEnumerable<string> values, StringComparison comparisonType)
		{
			if (str == null)
				throw (new ArgumentNullException(nameof(str)));

			foreach (string v in values)
			{
				if (str.StartsWith(v, comparisonType))
					return (true); // Match.
			}

			return (false); // No match.
		}

		/// <summary>
		/// Determines whether the beginning of <paramref name="str"/> matches one of the specified <paramref name="values"/>.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <param name="values">The strings to compare with.</param>
		/// <returns>true if <paramref name="str"/> matches the beginning of a comparing string; otherwise, false.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		/// <exception cref="ArgumentNullException">One of the <paramref name="values"/> is null.</exception>
		public static bool StartsWithAnyOrdinalIgnoreCase(string str, IEnumerable<string> values)
		{
			return (StartsWithAny(str, values, StringComparison.OrdinalIgnoreCase));
		}

		/// <summary>
		/// Determines whether the beginning of <paramref name="str"/> matches one of the specified <paramref name="values"/>.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <param name="values">The strings to compare with.</param>
		/// <returns>true if <paramref name="str"/> matches the beginning of a comparing string; otherwise, false.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		/// <exception cref="ArgumentNullException">One of the <paramref name="values"/> is null.</exception>
		public static bool StartsWithAnyOrdinalIgnoreCase(string str, params string[] values)
		{
			return (StartsWithAnyOrdinalIgnoreCase(str, (IEnumerable<string>)values));
		}

		/// <summary>
		/// Determines whether the end of <paramref name="str"/> matches the specified <paramref name="value"/>.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <param name="value">The string to compare with.</param>
		/// <returns>true if <paramref name="str"/> matches the end of the comparing string; otherwise, false.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
		public static bool EndsWithOrdinalIgnoreCase(string str, string value)
		{
			if (str == null)
				throw (new ArgumentNullException(nameof(str)));

			return (str.EndsWith(value, StringComparison.OrdinalIgnoreCase));
		}

		#endregion

		#region Left/Mid/Right
		//------------------------------------------------------------------------------------------
		// Left/Mid/Right
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Truncates <paramref name="str"/> to the <paramref name="length"/> leftmost characters.
		/// </summary>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		public static string Left(string str, int length)
		{
			if (str == null)
				throw (new ArgumentNullException(nameof(str)));

			if (length >= str.Length)
				return (str);
			else if (length <= 0)
				return ("");
			else
				return (str.Substring(0, length));
		}

		/// <summary>
		/// Truncates <paramref name="str"/> from <paramref name="begin"/> to <paramref name="end"/>.
		/// </summary>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		public static string Mid(string str, int begin, int end)
		{
			if (str == null)
				throw (new ArgumentNullException(nameof(str)));

			if (begin >= end)
				return ("");
			else if ((begin < 0) || (begin >= str.Length) || (end < 0))
				return ("");
			else if (end >= str.Length)
				return (str.Substring(begin));
			else
				return (str.Substring(begin, end - begin + 1));
		}

		/// <summary>
		/// Truncates <paramref name="str"/> to the <paramref name="length"/> rightmost characters.
		/// </summary>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		public static string Right(string str, int length)
		{
			if (str == null)
				throw (new ArgumentNullException(nameof(str)));

			if (length >= str.Length)
				return (str);
			else if (length <= 0)
				return ("");
			else
				return (str.Substring(str.Length - length, length));
		}

		#endregion

		#region Limit

		/// <summary>
		/// Limits the give string to the specified max length. If the string exceed the max length,
		/// <see cref="Ellipsis"/> are appended.
		/// </summary>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		public static string Limit(string str, int length)
		{
			if (str == null)
				throw (new ArgumentNullException(nameof(str)));

			if (str.Length <= length)
				return (str);
			else
				return (Left(str, (length - Ellipsis.Length)) + Ellipsis);
		}

		#endregion

		#region Split
		//------------------------------------------------------------------------------------------
		// Split
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Splits <paramref name="str"/> to the <paramref name="length"/> leftmost characters.
		/// </summary>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		public static string[] SplitLeft(string str, int length)
		{
			if (str == null)
				throw (new ArgumentNullException(nameof(str)));

			string left = Left(str, length);
			string right = Right(str, (str.Length - left.Length));

			var l = new List<string>(2); // Preset the required capacity to improve memory management.
			l.Add(left);
			l.Add(right);
			return (l.ToArray());
		}

		/// <summary>
		/// Splits <paramref name="str"/> to the <paramref name="length"/> rightmost characters.
		/// </summary>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		public static string[] SplitRight(string str, int length)
		{
			if (str == null)
				throw (new ArgumentNullException(nameof(str)));

			string right = Right(str, length);
			string left = Left(str, (str.Length - right.Length));

			var l = new List<string>(2) // Preset the required capacity to improve memory management.
			{
				left,
				right
			};
			return (l.ToArray());
		}

		/// <summary>
		/// Splits <paramref name="str"/> into chunks of <paramref name="desiredChunkLength"/>.
		/// </summary>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		public static string[] SplitFixedLength(string str, int desiredChunkLength)
		{
			if (str == null)
				throw (new ArgumentNullException(nameof(str)));

			var l = new List<string>(str.Length); // Preset the required capacity to improve memory management.
			for (int i = 0; i < str.Length; i += desiredChunkLength)
			{
				int effectiveChunkLength = Int32Ex.Limit(desiredChunkLength, 0, (str.Length - i)); // Remaining length is always above 0.
				l.Add(str.Substring(i, effectiveChunkLength));
			}
			return (l.ToArray());
		}

		/// <summary>
		/// Splits <paramref name="str"/> into chunks of <paramref name="desiredChunkLength"/>,
		/// taking word boundaries into account.
		/// </summary>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		public static string[] SplitLexically(string str, int desiredChunkLength)
		{
			if (str == null)
				throw (new ArgumentNullException(nameof(str)));

			List<string> chunks = new List<string>(ListInitialCapacityDefault); // Preset the initial capacity to improve memory management.
			string[] newLineSeparators = new string[] { Environment.NewLine, "\n", "\r" };

			foreach (string paragraph in str.Split(newLineSeparators, StringSplitOptions.None))
				chunks.AddRange(SplitLexicallyWithoutTakingNewLineIntoAccount(paragraph, desiredChunkLength));

			return (chunks.ToArray());
		}

		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		private static string[] SplitLexicallyWithoutTakingNewLineIntoAccount(string str, int desiredChunkLength)
		{
			if (str == null)
				throw (new ArgumentNullException(nameof(str)));

			var spaces = new List<int>(ListInitialCapacityDefault); // Preset the initial capacity to improve memory management.

			// Retrieve all spaces within the string:
			int i = 0;
			while ((i = str.IndexOf(' ', i)) >= 0)
			{
				spaces.Add(i);
				i++;
			}

			// Add an extra spaces at the end of the string to ensure that the last chunk is split properly:
			spaces.Add(str.Length);

			// Split the string into the desired chunk size taking word boundaries into account:
			int startIndex = 0;
			var chunks = new List<string>(ListInitialCapacityDefault); // Preset the initial capacity to improve memory management.
			while (startIndex < str.Length)
			{
				// Find the furthermost split position:
				int spaceIndex = spaces.FindLastIndex(value => (value <= (startIndex + desiredChunkLength)));

				int splitIndex;
				if (spaceIndex >= 0)
					splitIndex = spaces[spaceIndex];
				else
					splitIndex = (startIndex + desiredChunkLength);

				// Limit to split within the string and execute the split:
				splitIndex = Int32Ex.Limit(splitIndex, startIndex, str.Length); // Length is always above start index.
				int length = (splitIndex - startIndex);
				chunks.Add(str.Substring(startIndex, length));
				startIndex += length;

				// Remove the already used spaces from the collection to ensure those spaces are not used again:
				if (spaceIndex >= 0)
				{
					spaces.RemoveRange(0, (spaceIndex + 1));
					startIndex++; // Advance an extra character to compensate the space.
				}
			}

			return (chunks.ToArray());
		}

		#endregion

		#region Trim
		//------------------------------------------------------------------------------------------
		// Trim
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Removes up to <paramref name="maxLength"/> leading and trailing occurrences of a set
		/// of characters specified in an array from the current <see cref="string"/> object.
		/// </summary>
		/// <param name="str">The string to trim.</param>
		/// <param name="maxLength">The maximum number of characters to trim at both ends.</param>
		/// <param name="trimChars">An array of Unicode characters to remove or null.</param>
		/// <returns>
		/// The string that remains after all occurrences of the characters in the trimChars
		/// parameter are removed from the start and end of the current <see cref="string"/>
		/// object. If <paramref name="trimChars"/> is null, white-space characters are removed
		/// instead.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "chars", Justification = "Parameter naming as similar string methods.")]
		public static string Trim(string str, int maxLength, params char[] trimChars)
		{
			if (str == null)
				throw (new ArgumentNullException(nameof(str)));

			if (trimChars != null)
			{
				// Left:
				string[] sl = SplitLeft(str, maxLength);
				string left = sl[0];
				string rest = sl[1];
				left = left.Trim(trimChars);

				// Right:
				string[] sr = SplitRight(rest, maxLength);
				string mid = sr[0];
				string right = sr[1];
				right = right.Trim(trimChars);

				// Re-compose:
				return (left + mid + right);
			}
			else
			{
				return (str.Trim());
			}
		}

		/// <summary>
		/// Removes up to <paramref name="maxLength"/> leading and trailing occurrences of a set
		/// of characters specified in an array from the current <see cref="string"/> object,
		/// but only if they occur to both ends of the string.
		/// </summary>
		/// <param name="str">The string to trim.</param>
		/// <param name="maxLength">The maximum number of characters to trim at both ends.</param>
		/// <param name="trimChars">An array of Unicode characters to remove or null.</param>
		/// <returns>
		/// The string that remains after all occurrences of the characters in the trimChars
		/// parameter are removed from the start and end of the current <see cref="string"/>
		/// object. If <paramref name="trimChars"/> is null, white-space characters are removed
		/// instead.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "chars", Justification = "Parameter naming as similar string methods.")]
		public static string TrimSymmetrical(string str, int maxLength, params char[] trimChars)
		{
			if (str == null)
				throw (new ArgumentNullException(nameof(str)));

			if (trimChars != null)
			{
				// Count the number of trim characters at both ends:
				int countLeft = CountLeft(str, trimChars);
				int countRight = CountRight(str, trimChars);

				// Limit the number of trim characters at both ends:
				countLeft = Math.Min(countLeft, maxLength);
				countRight = Math.Min(countRight, maxLength);

				// Evaluate the symmetrical portion and trim it:
				int trimMaxLength = Math.Min(countLeft, countRight);
				return (Trim(str, trimMaxLength, trimChars));
			}
			else
			{
				return (str.Trim());
			}
		}

		/// <summary>
		/// Removes all leading and trailing occurrences of a set of characters specified in an
		/// array from the current <see cref="string"/> object, but only if they occur to
		/// both ends of the string.
		/// </summary>
		/// <param name="str">The string to trim.</param>
		/// <param name="trimChars">An array of Unicode characters to remove or null.</param>
		/// <returns>
		/// The string that remains after all occurrences of the characters in the trimChars
		/// parameter are removed from the start and end of the current <see cref="string"/>
		/// object. If <paramref name="trimChars"/> is null, white-space characters are removed
		/// instead.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "chars", Justification = "Parameter naming as similar string methods.")]
		public static string TrimSymmetrical(string str, params char[] trimChars)
		{
			if (str == null)
				throw (new ArgumentNullException(nameof(str)));

			if (trimChars != null)
			{
				return (TrimSymmetrical(str, int.MaxValue, trimChars));
			}
			else
			{
				return (str.Trim());
			}
		}

		#endregion

		#region Space
		//------------------------------------------------------------------------------------------
		// Space
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Spaces all characters of a <see cref="string"/> object.
		/// </summary>
		/// <param name="str">The string to space.</param>
		/// <returns>
		/// A new string that contains the spaced version of <paramref name="str"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		public static string Space(string str)
		{
			return (Space(str, ' '));
		}

		/// <summary>
		/// Spaces all characters of a <see cref="string"/> object.
		/// </summary>
		/// <param name="str">The string to space.</param>
		/// <param name="space">The character to use as space.</param>
		/// <returns>
		/// A new string that contains the spaced version of <paramref name="str"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		[SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "1#", Justification = "Intentionally using the same parameter name to emphasize purpose.")]
		public static string Space(string str, char space)
		{
			if (str == null)
				throw (new ArgumentNullException(nameof(str)));

			var sb = new StringBuilder((str.Length * 2) - 1);

			bool isFirst = true;
			foreach (char c in str)
			{
				if (isFirst)
					isFirst = false;
				else
					sb.Append(space);

				sb.Append(c);
			}

			return (sb.ToString());
		}

		#endregion

		#region Index
		//------------------------------------------------------------------------------------------
		// Index
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Reports the index of the first occurrence of the same character class in <paramref name="str"/>.
		/// </summary>
		/// <param name="str">The <see cref="string"/> object to process.</param>
		/// <param name="startIndex">The search starting position.</param>
		/// <returns>
		/// The zero-based index position of the start of the same character class in the string.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		public static int IndexOfSameCharacterClass(string str, int startIndex)
		{
			if (str == null)
				throw (new ArgumentNullException(nameof(str)));

			int i = startIndex;

			if (char.IsWhiteSpace(str, i)) // Includes 'IsSeparator' (Unicode space/line/paragraph
			{                              // separators) as well as 'IsControl' (<CR>, <LF>,...).
				for (/* i */; i >= 0; i--)
				{
					if (!char.IsWhiteSpace(str, i))
						return (i + 1);
				}
			}
			else if (char.IsPunctuation(str, i))
			{
				for (/* i */; i >= 0; i--)
				{
					if (!char.IsPunctuation(str, i))
						return (i + 1);
				}
			}
			else if (char.IsSymbol(str, i))
			{
				for (/* i */; i >= 0; i--)
				{
					if (!char.IsSymbol(str, i))
						return (i + 1);
				}
			}
			else
			{
				for (/* i */; i >= 0; i--)
				{
					if (char.IsWhiteSpace(str, i) || char.IsPunctuation(str, i) || char.IsSymbol(str, i))
						return (i + 1);
				}
			}

			return (0);
		}

		/// <summary>
		/// Reports the index of the first occurrence of the specified word in <paramref name="str"/>.
		/// </summary>
		/// <param name="str">The <see cref="string"/> object to process.</param>
		/// <param name="value">The word to search for.</param>
		/// <param name="comparisonType">One of the <see cref="StringComparison"/> values.</param>
		/// <returns>
		/// The zero-based index position of the start of the same character class in the string.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		public static int IndexOfWholeWord(string str, string value, StringComparison comparisonType)
		{
			return (IndexOfWholeWord(str, value, 0, comparisonType));
		}

		/// <summary>
		/// Reports the index of the first occurrence of the specified word in <paramref name="str"/>.
		/// </summary>
		/// <param name="str">The <see cref="string"/> object to process.</param>
		/// <param name="value">The word to search for.</param>
		/// <param name="startIndex">The search starting position.</param>
		/// <param name="comparisonType">One of the <see cref="StringComparison"/> values.</param>
		/// <returns>
		/// The zero-based index position of the start of the same character class in the string.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		public static int IndexOfWholeWord(string str, string value, int startIndex, StringComparison comparisonType)
		{
			if (str == null)
				throw (new ArgumentNullException(nameof(str)));

			int i = startIndex;  // Using string.IndexOf() because string.Contains() does not allow controlling culture and case.
			while ((i < str.Length) && ((i = str.IndexOf(value, i, comparisonType)) != InvalidIndex))
			{
				bool isStartBoundary;
				int wordStart = i;
				if (wordStart <= 0)
					isStartBoundary = true;
				else
					isStartBoundary = !char.IsLetterOrDigit(str[wordStart - 1]);

				bool isEndBoundary;
				int wordEnd = (i + value.Length - 1);
				if (wordEnd >= (str.Length - 1))
					isEndBoundary = true;
				else
					isEndBoundary = !char.IsLetterOrDigit(str[wordEnd + 1]);

				if (isStartBoundary && isEndBoundary)
					return (i);

				i++;
			}

			return (InvalidIndex);
		}

		/// <summary>
		/// Reports the index of the first occurrence of the specified string in <paramref name="str"/>.
		/// Parameters specify the starting search position in the string, the number of characters
		/// in the current string to search, and the type of search to use for the specified string.
		/// </summary>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "Parameter naming as similar string methods.")]
		public static int IndexOfOutsideDoubleQuotes(string str, string searchString, StringComparison comparisonType)
		{
			return (IndexOfOutsideDoubleQuotes(str, searchString, 0, str.Length, comparisonType));
		}

		/// <summary>
		/// Reports the index of the first occurrence of the specified string in <paramref name="str"/>.
		/// Parameters specify the starting search position in the string, the number of characters
		/// in the current string to search, and the type of search to use for the specified string.
		/// </summary>
		/// <param name="str">The <see cref="string"/> object to process.</param>
		/// <param name="searchString">The <see cref="string"/> object to seek.</param>
		/// <param name="startIndex">The search starting position.</param>
		/// <param name="count">The number of character positions to examine.</param>
		/// <param name="comparisonType">One of the <see cref="StringComparison"/> values.</param>
		/// <returns>
		/// The zero-based index position of the value parameter if that string is found,
		/// or <see cref="InvalidIndex"/> if it is not. If value is <see cref="string.Empty"/>,
		/// the return value is <paramref name="startIndex"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="searchString"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> or <paramref name="startIndex"/> is negative.  -or- <paramref name="count"/> plus <paramref name="startIndex"/> specify a position that is not within this instance.</exception>
		/// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a valid <see cref="StringComparison"/> value.</exception>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "Parameter naming as similar string methods.")]
		public static int IndexOfOutsideDoubleQuotes(string str, string searchString, int startIndex, int count, StringComparison comparisonType)
		{
			if (str == null)
				throw (new ArgumentNullException(nameof(str)));

			string substring = str.Substring(startIndex, count); // Crop the string as desired.

			string rep = substring.Replace(@"\""", @""""); // Replace \" by "" to ease processing below.

			int offset = 0;
			var l = new List<KeyValuePair<int, string>>(ListInitialCapacityDefault); // Preset the initial capacity to improve memory management.
			foreach (string s in rep.Split('"')) // Split string into chunks between double quotes.
			{
				l.Add(new KeyValuePair<int, string>(offset, s));
				offset += s.Length; // Add the string length to the offset.
				offset++;           // Correct the offset created by the dropped double quotes.
			}

			for (int i = 0; i < l.Count; i += 2) // Check every second chunk for the first occurrence of 'seachString'.
			{
				int index = l[i].Value.IndexOf(searchString, comparisonType);
				if (index >= 0)
					return (startIndex + l[i].Key + index);
			}

			return (InvalidIndex);
		}

		#endregion

		#region ToString
		//------------------------------------------------------------------------------------------
		// ToString
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Converts the given value to a printable string.
		/// </summary>
		/// <remarks>
		/// Printable characters are kept, control characters are converted into the ASCII mnemonic
		/// or Unicode representation as required.
		/// </remarks>
		public static string ConvertToPrintableString(string value)
		{
			var sb = new StringBuilder();

			foreach (char c in value)
				sb.Append(CharEx.ConvertToPrintableString(c));

			return (sb.ToString());
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
