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
// Copyright © 2003-2020 Matthias Kläy.
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
	/// <see cref="Decimal"/>/<see cref="decimal"/> utility methods.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class DecimalEx
	{
		/// <summary>
		/// Limits <paramref name="value"/> to the values specified.
		/// </summary>
		/// <exception cref="ArgumentException">
		/// <paramref name="min"/> is larger than <paramref name="max"/>.
		/// </exception>
		public static decimal Limit(decimal value, decimal min, decimal max)
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
		public static bool IsWithin(decimal value, decimal min, decimal max)
		{
			return ((value >= min) && (value <= max));
		}


		/// <summary>
		/// Evaluates whether the two given values are almost equal,
		/// taking the given number of digits into account.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="digits"/> is less than 0 or greater than 28.
		/// </exception>
		public static bool AlmostEquals(decimal lhs, decimal rhs, int digits)
		{
			decimal diff = Math.Abs(lhs - rhs);

			switch (digits)
			{
				case  0: return (diff < 1.0m);
				case  1: return (diff < 0.1m);
				case  2: return (diff < 0.01m);
				case  3: return (diff < 0.001m);
				case  4: return (diff < 0.0001m);
				case  5: return (diff < 0.00001m);
				case  6: return (diff < 0.000001m);
				case  7: return (diff < 0.0000001m);
				case  8: return (diff < 0.00000001m);
				case  9: return (diff < 0.000000001m);
				case 10: return (diff < 0.0000000001m);
				case 11: return (diff < 0.00000000001m);
				case 12: return (diff < 0.000000000001m);
				case 13: return (diff < 0.0000000000001m);
				case 14: return (diff < 0.00000000000001m);
				case 15: return (diff < 0.000000000000001m);
				case 16: return (diff < 0.0000000000000001m);
				case 17: return (diff < 0.00000000000000001m);
				case 18: return (diff < 0.000000000000000001m);
				case 19: return (diff < 0.0000000000000000001m);
				case 20: return (diff < 0.00000000000000000001m);
				case 21: return (diff < 0.000000000000000000001m);
				case 22: return (diff < 0.0000000000000000000001m);
				case 23: return (diff < 0.00000000000000000000001m);
				case 24: return (diff < 0.000000000000000000000001m);
				case 25: return (diff < 0.0000000000000000000000001m);
				case 26: return (diff < 0.00000000000000000000000001m);
				case 27: return (diff < 0.000000000000000000000000001m);
				case 28: return (diff < 0.0000000000000000000000000001m);

				default: throw (new ArgumentOutOfRangeException("digits", digits, MessageHelper.InvalidExecutionPreamble + "Value must be from 0 to 28 but is " + digits + "!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary>
		/// Evaluates whether the two given values are rather not equal,
		/// taking the given number of digits into account.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="digits"/> is less than 0 or greater than 28.
		/// </exception>
		public static bool RatherNotEquals(decimal lhs, decimal rhs, int digits)
		{
			return (!AlmostEquals(lhs, rhs, digits));
		}

		/// <summary>
		/// Get the minimum and maximum within <paramref name="collection"/>.
		/// </summary>
		public static void GetMinMax(IEnumerable<decimal> collection, out decimal min, out decimal max)
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
