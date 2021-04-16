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
		public static bool IsWithin(decimal value, decimal min, decimal max)
		{
			return ((value >= min) && (value <= max));
		}

		/// <summary>
		/// Evaluates whether the integral part of the two given values is equal or almost equal.
		/// </summary>
		/// <remarks>
		/// Same as <code>RatherEquals(lhs, rhs, 0)</code>.
		/// </remarks>
		public static bool RatherEqualsIntegral(decimal lhs, decimal rhs)
		{
			return (RatherEquals(lhs, rhs, 0));
		}

		/// <summary>
		/// Evaluates whether the two given values are equal or almost equal floating point values.
		/// </summary>
		/// <remarks>
		/// Same as <code>RatherEquals(lhs, rhs, 28)</code>.
		/// </remarks>
		public static bool RatherEquals(decimal lhs, decimal rhs)
		{
			return (RatherEquals(lhs, rhs, 28));
		}

		/// <summary>
		/// Evaluates whether the two given values are equal or almost equal,
		/// taking the given number of decimals into account.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="decimals"/> is less than 0 or greater than 28.
		/// </exception>
		/// <remarks>
		/// Using term "decimals" same as e.g. <see cref="Math.Round(decimal, int)"/>.
		/// </remarks>
		public static bool RatherEquals(decimal lhs, decimal rhs, int decimals)
		{
			decimal diff = Math.Abs(lhs - rhs);

			switch (decimals)
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

				default: throw (new ArgumentOutOfRangeException("decimals", decimals, MessageHelper.InvalidExecutionPreamble + "Value must be from 0 to 28 but is " + decimals + "!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary>
		/// Evaluates whether the two given values are rather not equal,
		/// taking the given number of decimals into account.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="decimals"/> is less than 0 or greater than 28.
		/// </exception>
		/// <remarks>
		/// Using term "decimals" same as e.g. <see cref="Math.Round(decimal, int)"/>.
		/// </remarks>
		public static bool RatherNotEquals(decimal lhs, decimal rhs, int decimals)
		{
			return (!RatherEquals(lhs, rhs, decimals));
		}

		/// <summary>
		/// Evaluates whether the two given values are rather not equal floating point values.
		/// </summary>
		/// <remarks>
		/// Same as <code>RatherNotEquals(lhs, rhs, 28)</code>.
		/// </remarks>
		public static bool RatherNotEquals(decimal lhs, decimal rhs)
		{
			return (RatherNotEquals(lhs, rhs, 28));
		}

		/// <summary>
		/// Evaluates whether the integral part of the two given values is rather not equal.
		/// </summary>
		/// <remarks>
		/// Same as <code>RatherNotEquals(lhs, rhs, 0)</code>.
		/// </remarks>
		public static bool RatherNotEqualsIntegral(decimal lhs, decimal rhs)
		{
			return (RatherNotEquals(lhs, rhs, 0));
		}

		/// <summary>
		/// Get the minimum and maximum within <paramref name="collection"/>.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static void GetMinMax(IEnumerable<decimal> collection, out decimal min, out decimal max)
		{
			if (collection.Count() <= 0)
			{
				min = 0; // = default(T)
				max = 0; // = default(T)
			}
			else
			{
				min = decimal.MaxValue;
				max = decimal.MinValue;

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
