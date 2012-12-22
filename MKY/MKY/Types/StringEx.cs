//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.8
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

// This code is intentionally placed into the MKY namespace even though the file is located in
// MKY.Types for consistency with the Sytem namespace.
namespace MKY
{
	/// <summary>
	/// String utility methods.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class StringEx
	{
		/// <summary>
		/// An invalid index is represented by -1.
		/// </summary>
		public const int InvalidIndex = -1;

		/// <summary>
		/// Compares two specified <see cref="System.String"/> objects ignoring culture and case.
		/// The method returns an integer that indicates the relationship of the two
		/// <see cref="System.String"/> objects to one another in the sort order.
		/// </summary>
		/// <param name="strA">The first <see cref="System.String"/> object.</param>
		/// <param name="strB">The second <see cref="System.String"/> object.</param>
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
		/// Compares two specified <see cref="System.String"/> objects ignoring culture.
		/// </summary>
		public static bool EqualsOrdinal(string strA, string strB)
		{
			return (string.CompareOrdinal(strA, strB) == 0);
		}

		/// <summary>
		/// Compares two specified string arrays ignoring culture.
		/// </summary>
		public static bool EqualsOrdinal(string[] strA, string[] strB)
		{
			if (strA.Length == strB.Length)
			{
				for (int i = 0; i < strA.Length; i++)
				{
					if (EqualsOrdinal(strA[i], strB[i]))
						return (false);
				}
				return (true);
			}
			else
			{
				return (false);
			}
		}

		/// <summary>
		/// Compares two specified <see cref="System.String"/> objects ignoring culture and case.
		/// </summary>
		public static bool EqualsOrdinalIgnoreCase(string strA, string strB)
		{
			return (string.Compare(strA, strB, StringComparison.OrdinalIgnoreCase) == 0);
		}

		/// <summary>
		/// Compares two specified string arrays ignoring culture and case.
		/// </summary>
		public static bool EqualsOrdinalIgnoreCase(string[] strA, string[] strB)
		{
			if (strA.Length == strB.Length)
			{
				for (int i = 0; i < strA.Length; i++)
				{
					if (EqualsOrdinalIgnoreCase(strA[i], strB[i]))
						return (false);
				}
				return (true);
			}
			else
			{
				return (false);
			}
		}

		/// <summary>
		/// Compares wether the first <see cref="System.String"/> object matches any <see cref="System.String"/> object of the given array.
		/// </summary>
		public static bool EqualsAnyOrdinalIgnoreCase(string strA, string[] strB)
		{
			foreach (string str in strB)
			{
				if (EqualsOrdinalIgnoreCase(strA, str))
					return (true);
			}
			return (false);
		}

		#endregion

		#region Contains
		//------------------------------------------------------------------------------------------
		// Contains
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Returns whether <paramref name="str"/> contains any of the <paramref name="searchChars"/>.
		/// </summary>
		public static bool ContainsAny(string str, char[] searchChars)
		{
			return (str.IndexOfAny(searchChars) >= 0);
		}

		#endregion

		#region Index
		//------------------------------------------------------------------------------------------
		// Index
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Reports the index of the first occurrence of the specified string in <paramref name="str"/>.
		/// Parameters specify the starting search position in the string, the number of characters
		/// in the current string to search, and the type of search to use for the specified string.
		/// </summary>
		public static int IndexOfOutsideDoubleQuotes(string str, string searchString, StringComparison comparisonType)
		{
			return (IndexOfOutsideDoubleQuotes(str, searchString, 0, str.Length, comparisonType));
		}

		/// <summary>
		/// Reports the index of the first occurrence of the specified string in <paramref name="str"/>.
		/// Parameters specify the starting search position in the string, the number of characters
		/// in the current string to search, and the type of search to use for the specified string.
		/// </summary>
		/// <param name="str">The <see cref="System.String"/> object to process.</param>
		/// <param name="searchString">The <see cref="System.String"/> object to seek.</param>
		/// <param name="startIndex">The search starting position.</param>
		/// <param name="count">The number of character positions to examine.</param>
		/// <param name="comparisonType">One of the <see cref="StringComparison"/> values.</param>
		/// <returns>
		/// The zero-based index position of the value parameter if that string is found,
		/// or <see cref="InvalidIndex"/> if it is not. If value is <see cref="String.Empty"/>,
		/// the return value is startIndex.
		/// </returns>
		/// <exception cref="ArgumentNullException">searchString is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">count or startIndex is negative.  -or- count plus startIndex specify a position that is not within this instance.</exception>
		/// <exception cref="ArgumentException">comparisonType is not a valid <see cref="StringComparison"/> value.</exception>
		public static int IndexOfOutsideDoubleQuotes(string str, string searchString, int startIndex, int count, StringComparison comparisonType)
		{
			string substring = str.Substring(startIndex, count); // Crop the string as desired.

			string rep = substring.Replace(@"\""", @""""); // Replace \" by "" to ease processing below.

			int offset = 0;
			List<KeyValuePair<int, string>> l = new List<KeyValuePair<int, string>>();
			foreach (string s in rep.Split('"')) // Split string into chunks between double quotes.
			{
				l.Add(new KeyValuePair<int, string>(offset, s));
				offset += s.Length; // Add the string length to the offset.
				offset++;           // Correct the offset created by the dropped double quotes.
			}

			for (int i = 0; i < l.Count; i += 2) // Check every second chunk for the first occurance of seachString.
			{
				int index = l[i].Value.IndexOf(searchString, comparisonType);
				if (index >= 0)
					return (startIndex + l[i].Key + index);
			}

			return (InvalidIndex);
		}

		#endregion

		#region Count
		//------------------------------------------------------------------------------------------
		// Count
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Counts the number of <paramref name="countChars"/> to the left of <paramref name="str"/>.
		/// </summary>
		public static int CountLeft(string str, params char[] countChars)
		{
			if (str == null)
				throw (new ArgumentNullException("str"));

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
		public static int CountRight(string str, params char[] countChars)
		{
			if (str == null)
				throw (new ArgumentNullException("str"));

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

		#region Left/Mid/Right
		//------------------------------------------------------------------------------------------
		// Left/Mid/Right
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Truncates <paramref name="str"/> to the <paramref name="length"/> leftmost characters.
		/// </summary>
		public static string Left(string str, int length)
		{
			if (length >= str.Length)
				return (str);
			else
				return (str.Substring(0, length));
		}

		/// <summary>
		/// Truncates <paramref name="str"/> from <paramref name="begin"/> to <paramref name="end"/>.
		/// </summary>
		public static string Mid(string str, int begin, int end)
		{
			if (begin >= end)
				return ("");
			else
				return (str.Substring(begin, end - begin + 1));
		}

		/// <summary>
		/// Truncates <paramref name="str"/> to the <paramref name="length"/> rightmost characters.
		/// </summary>
		public static string Right(string str, int length)
		{
			if (length >= str.Length)
				return (str);
			else
				return (str.Substring(str.Length - length, length));
		}

		#endregion

		#region Split
		//------------------------------------------------------------------------------------------
		// Split
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Splits <paramref name="str"/> to the <paramref name="length"/> leftmost characters.
		/// </summary>
		public static string[] SplitLeft(string str, int length)
		{
			string left = Left(str, length);
			string right = Right(str, (str.Length - left.Length));

			List<string> l = new List<string>();
			l.Add(left);
			l.Add(right);
			return (l.ToArray());
		}

		/// <summary>
		/// Splits <paramref name="str"/> to the <paramref name="length"/> rightmost characters.
		/// </summary>
		public static string[] SplitRight(string str, int length)
		{
			string right = Right(str, length);
			string left = Left(str, (str.Length - right.Length));

			List<string> l = new List<string>();
			l.Add(left);
			l.Add(right);
			return (l.ToArray());
		}

		/// <summary>
		/// Splits <paramref name="str"/> into chunks of <paramref name="desiredChunkLength"/>.
		/// </summary>
		public static string[] SplitFixedLength(string str, int desiredChunkLength)
		{
			List<string> l = new List<string>();
			for (int i = 0; i < str.Length; i += desiredChunkLength)
			{
				int effectiveChunkLength = Int32Ex.LimitToBounds(desiredChunkLength, 0, str.Length - i);
				l.Add(str.Substring(i, effectiveChunkLength));
			}
			return (l.ToArray());
		}

		/// <summary>
		/// Splits <paramref name="str"/> into chunks of <paramref name="desiredChunkLength"/>,
		/// taking word boundaries into account.
		/// </summary>
		public static string[] SplitLexically(string str, int desiredChunkLength)
		{
			List<string> chunks = new List<string>();
			string[] newLineSeparators = new string[] { Environment.NewLine, "\n", "\r" };

			foreach (string paragraph in str.Split(newLineSeparators, StringSplitOptions.None))
				chunks.AddRange(SplitLexicallyWithoutTakingNewLineIntoAccount(paragraph, desiredChunkLength));

			return (chunks.ToArray());
		}

		private static string[] SplitLexicallyWithoutTakingNewLineIntoAccount(string str, int desiredChunkLength)
		{
			List<int> spaces = new List<int>();

			// Retrieve all spaces within the string:
			int i = 0;
			while ((i = str.IndexOf(' ', i)) >= 0)
			{
				spaces.Add(i);
				i++;
			}

			// Add an extra space at the end of the string to ensure that the last chunk is split properly:
			spaces.Add(str.Length);

			// Split the string into the desired chunk size taking word boundaries into account:
			int startIndex = 0;
			List<string> chunks = new List<string>();
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
				splitIndex = Int32Ex.LimitToBounds(splitIndex, startIndex, str.Length);
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
		/// of characters specified in an array from the current <see cref="System.String"/> object.
		/// </summary>
		/// <param name="str">The string to trim.</param>
		/// <param name="maxLength">The maximum number of characters to trim at both ends.</param>
		/// <param name="trimChars">An array of Unicode characters to remove or null.</param>
		/// <returns>
		/// The string that remains after all occurrences of the characters in the trimChars
		/// parameter are removed from the start and end of the current <see cref="System.String"/>
		/// object. If <paramref name="trimChars"/> is null, white-space characters are removed
		/// instead.
		/// </returns>
		public static string TrimMaxLength(string str, int maxLength, params char[] trimChars)
		{
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
		/// of characters specified in an array from the current <see cref="System.String"/> object,
		/// but only if they occur to both ends of the string.
		/// </summary>
		/// <param name="str">The string to trim.</param>
		/// <param name="maxLength">The maximum number of characters to trim at both ends.</param>
		/// <param name="trimChars">An array of Unicode characters to remove or null.</param>
		/// <returns>
		/// The string that remains after all occurrences of the characters in the trimChars
		/// parameter are removed from the start and end of the current <see cref="System.String"/>
		/// object. If <paramref name="trimChars"/> is null, white-space characters are removed
		/// instead.
		/// </returns>
		public static string TrimMaxLengthSymmetrical(string str, int maxLength, params char[] trimChars)
		{
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
				return (TrimMaxLength(str, trimMaxLength, trimChars));
			}
			else
			{
				return (str.Trim());
			}
		}

		/// <summary>
		/// Removes all leading and trailing occurrences of a set of characters specified in an
		/// array from the current <see cref="System.String"/> object, but only if they occur to
		/// both ends of the string.
		/// </summary>
		/// <param name="str">The string to trim.</param>
		/// <param name="trimChars">An array of Unicode characters to remove or null.</param>
		/// <returns>
		/// The string that remains after all occurrences of the characters in the trimChars
		/// parameter are removed from the start and end of the current <see cref="System.String"/>
		/// object. If <paramref name="trimChars"/> is null, white-space characters are removed
		/// instead.
		/// </returns>
		public static string TrimSymmetrical(string str, params char[] trimChars)
		{
			if (trimChars != null)
			{
				return (TrimMaxLengthSymmetrical(str, int.MaxValue, trimChars));
			}
			else
			{
				return (str.Trim());
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
