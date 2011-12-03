//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.7
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;

// This code is intentionally placed into the MKY namespace even though the file is located in
// MKY.Types for consistency with the Sytem namespace.
namespace MKY
{
	/// <summary>
	/// String utility methods.
	/// </summary>
	public static class StringEx
	{
		/// <summary>
		/// An invalid index is represented by -1.
		/// </summary>
		public const int InvalidIndex = -1;
	
		/// <summary>
		/// Compares two specified <see cref="System.String"/> objects ignoring culture.
		/// </summary>
		public static bool EqualsOrdinal(string strA, string strB)
		{
			return (string.Compare(strA, strB, StringComparison.Ordinal) == 0);
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

		/// <summary>
		/// Truncates "truncateString" to the "length" leftmost characters.
		/// </summary>
		public static string Left(string truncateString, int length)
		{
			if (length >= truncateString.Length)
				return (truncateString);
			else
				return (truncateString.Substring(0, length));
		}

		/// <summary>
		/// Truncates "truncateString" from "begin" to "end".
		/// </summary>
		public static string Mid(string truncateString, int begin, int end)
		{
			if (begin >= end)
				return ("");
			else
				return (truncateString.Substring(begin, end - begin + 1));
		}

		/// <summary>
		/// Truncates "truncateString" to the "length" rightmost characters.
		/// </summary>
		public static string Right(string truncateString, int length)
		{
			if (length >= truncateString.Length)
				return (truncateString);
			else
				return (truncateString.Substring(truncateString.Length - length, length));
		}

		/// <summary>
		/// Returns whether "str" contains any of the "searchChars".
		/// </summary>
		public static bool ContainsAny(string str, char[] searchChars)
		{
			return (str.IndexOfAny(searchChars) >= 0);
		}

		/// <summary>
		/// Reports the index of the first occurrence of the specified string in "str".
		/// Parameters specify the starting search position in the string, the number of characters
		/// in the current string to search, and the type of search to use for the specified string.
		/// </summary>
		public static int IndexOfOutsideDoubleQuotes(string str, string searchString, StringComparison comparisonType)
		{
			return (IndexOfOutsideDoubleQuotes(str, searchString, 0, str.Length, comparisonType));
		}

		/// <summary>
		/// Reports the index of the first occurrence of the specified string in "str".
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
				int index = l[i].Value.IndexOf(searchString);
				if (index >= 0)
					return (startIndex + l[i].Key + index);
			}

			return (InvalidIndex);
		}

		/// <summary>
		/// Splits "str" into chunks of "desiredChunkSize".
		/// </summary>
		public static string[] Split(string str, int desiredChunkSize)
		{
			List<string> l = new List<string>();
			for (int i = 0; i < str.Length; i += desiredChunkSize)
			{
				int effectiveChunkSize = Int32Ex.LimitToBounds(desiredChunkSize, 0, str.Length - i);
				l.Add(str.Substring(i, effectiveChunkSize));
			}
			return (l.ToArray());
		}

		/// <summary>
		/// Splits "str" into chunks of "desiredChunkSize" taking word boundaries into account.
		/// </summary>
		public static string[] SplitLexically(string str, int desiredChunkSize)
		{
			List<string> chunks = new List<string>();
			string[] newLineSeparators = new string[] { Environment.NewLine, "\n", "\r" };

			foreach (string paragraph in str.Split(newLineSeparators, StringSplitOptions.None))
				chunks.AddRange(SplitLexicallyWithoutTakingNewLineIntoAccount(paragraph, desiredChunkSize));

			return (chunks.ToArray());
		}

		private static string[] SplitLexicallyWithoutTakingNewLineIntoAccount(string str, int desiredChunkSize)
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
				Predicate<int> p = new Predicate<int>(value => (value <= (startIndex + desiredChunkSize)));
				int spaceIndex = spaces.FindLastIndex(p);
				
				int splitIndex;
				if (spaceIndex >= 0)
					splitIndex = spaces[spaceIndex];
				else
					splitIndex = (startIndex + desiredChunkSize);

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
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
