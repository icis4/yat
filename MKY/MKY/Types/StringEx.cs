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
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;

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
		public static bool Contains(string str, char[] searchChars)
		{
			foreach (char c in searchChars)
			{
				if (str.Contains(c.ToString()))
					return (true);
			}
			return (false);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
