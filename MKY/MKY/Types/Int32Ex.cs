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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

// This code is intentionally placed into the MKY namespace even though the file is located in
// MKY.Types for consistency with the System namespace.
namespace MKY
{
	/// <summary>
	/// Int32/int utility methods.
	/// </summary>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Int32 just *is* 'int'...")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class Int32Ex
	{
		/// <summary>
		/// Limits <paramref name="value"/> to the values specified.
		/// </summary>
		/// <exception cref="ArgumentException">
		/// <paramref name="min"/> is larger than <paramref name="max"/>.
		/// </exception>
		public static int Limit(int value, int min, int max)
		{
			if (min > max)
				throw (new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Precondition is 'min' <= 'max', but 'min' is {0} and 'max' is {1}!", min, max))); // Do not append 'MessageHelper.InvalidExecutionPreamble' as caller could rely on this exception text.

			if (value < min)
				return (min);

			if (value > max)
				return (max);

			return (value);
		}

		/// <summary>
		/// Returns whether <paramref name="value"/> is within the values specified (including boundaries).
		/// </summary>
		public static bool IsWithin(int value, int min, int max)
		{
			return ((value >= min) && (value <= max));
		}

		/// <summary>
		/// Returns whether <paramref name="value"/> is even.
		/// </summary>
		public static bool IsEven(int value)
		{
			return ((value % 2) == 0);
		}

		/// <summary>
		/// Returns whether <paramref name="value"/> is odd.
		/// </summary>
		public static bool IsOdd(int value)
		{
			return ((value % 2) != 0);
		}

		/// <summary>
		/// Returns the according numeric suffix like "st", "nd", "rd" or "th".
		/// </summary>
		public static string ToEnglishSuffix(int value)
		{
			switch (value % 10)
			{
				case 1:  return ("st");
				case 2:  return ("nd");
				case 3:  return ("rd");
				default: return ("th");
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
