//==================================================================================================
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
// ------------------------------------------------------------------------------------------------
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:maettu_this@users.sourceforge.net.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

namespace MKY.Utilities.Types
{
	/// <summary>
	/// String utility methods.
	/// </summary>
	public static class XString
	{
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
