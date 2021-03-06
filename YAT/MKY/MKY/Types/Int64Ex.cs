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

// This code is intentionally placed into the MKY namespace even though the file is located in
// MKY.Types for consistency with the System namespace.
namespace MKY
{
	/// <summary>
	/// <see cref="Int64"/>/<see cref="long"/> utility methods.
	/// </summary>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Int64 just *is* 'long'...")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class Int64Ex
	{
		/// <summary>
		/// Determines whether <paramref name="value"/> equals any of the specified <paramref name="values"/>.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="values">The values to compare with.</param>
		/// <returns>true if <paramref name="value"/> matches any of the <paramref name="values"/>; otherwise, false.</returns>
		public static bool EqualsAny(long value, IEnumerable<long> values)
		{
			foreach (long v in values)
			{
				if (v == value)
					return (true); // Equals.
			}

			return (false); // Does not equal any.
		}

		/// <summary>
		/// Limits <paramref name="value"/> to the values specified.
		/// </summary>
		/// <exception cref="ArgumentException">
		/// <paramref name="min"/> is larger than <paramref name="max"/>.
		/// </exception>
		public static long Limit(long value, long min, long max)
		{
			if (min > max)
				throw (new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Precondition is 'min' <= 'max', but 'min' is {0} and 'max' is {1}!", min, max))); // Do not decorate with 'InvalidExecutionPreamble/SubmitBug' as this exception is eligible during normal execution.

			if (value < min)
				return (min);

			if (value > max)
				return (max);

			return (value);
		}

		/// <summary>
		/// Returns whether <paramref name="value"/> is within the values specified (including boundaries).
		/// </summary>
		public static bool IsWithin(long value, long min, long max)
		{
			return ((value >= min) && (value <= max));
		}

		/// <summary>
		/// Returns whether <paramref name="value"/> is even.
		/// </summary>
		public static bool IsEven(long value)
		{
			return ((value % 2) == 0);
		}

		/// <summary>
		/// Returns whether <paramref name="value"/> is odd.
		/// </summary>
		public static bool IsOdd(long value)
		{
			return ((value % 2) != 0);
		}

		/// <summary>
		/// Returns the according numeric suffix like "st", "nd", "rd" or "th".
		/// </summary>
		public static string ToEnglishSuffix(long value)
		{
			switch (value % 10)
			{
				case 1:  return ("st");
				case 2:  return ("nd");
				case 3:  return ("rd");
				default: return ("th");
			}
		}

		/// <summary>
		/// Get the minimum and maximum within <paramref name="collection"/>.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static void GetMinMax(IEnumerable<long> collection, out long min, out long max)
		{
			if (collection.Count() <= 0)
			{
				min = 0; // = default(T)
				max = 0; // = default(T)
			}
			else
			{
				min = int.MaxValue;
				max = int.MinValue;

				foreach (var item in collection)
				{
					if (item < min) { min = item; }
					if (item > max) { max = item; }
				}
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
