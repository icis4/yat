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
	/// <see cref="Double"/>/<see cref="double"/> utility methods.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class DoubleEx
	{
		/// <summary>
		/// Limits <paramref name="value"/> to the values specified.
		/// </summary>
		/// <exception cref="ArgumentException">
		/// <paramref name="min"/> is larger than <paramref name="max"/>.
		/// </exception>
		public static double Limit(double value, double min, double max)
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
		public static bool IsWithin(double value, double min, double max)
		{
			return ((value >= min) && (value <= max));
		}

		/// <summary>
		/// Evaluates whether the integral part of the two given values is equal or almost equal.
		/// </summary>
		/// <remarks>
		/// Same as <code>RatherEquals(lhs, rhs, 0)</code>.
		/// </remarks>
		public static bool RatherEqualsIntegral(double lhs, double rhs)
		{
			return (RatherEquals(lhs, rhs, 0));
		}

		/// <summary>
		/// Evaluates whether the two given values are equal or almost equal floating point values.
		/// </summary>
		/// <remarks>
		/// Same as <code>RatherEquals(lhs, rhs, 15)</code>.
		/// </remarks>
		public static bool RatherEquals(double lhs, double rhs)
		{
			return (RatherEquals(lhs, rhs, 15));
		}

		/// <summary>
		/// Evaluates whether the two given values are equal or almost equal,
		/// taking the given number of digits into account.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="digits"/> is less than 0 or greater than 15.
		/// </exception>
		/// <remarks>
		/// Using term "digits" same as e.g. <see cref="Math.Round(double, int)"/>.
		/// </remarks>
		public static bool RatherEquals(double lhs, double rhs, int digits)
		{
			double diff = Math.Abs(lhs - rhs);

			switch (digits)
			{
				case  0: return (diff < 1.0);
				case  1: return (diff < 0.1);
				case  2: return (diff < 0.01);
				case  3: return (diff < 0.001);
				case  4: return (diff < 0.0001);
				case  5: return (diff < 0.00001);
				case  6: return (diff < 0.000001);
				case  7: return (diff < 0.0000001);
				case  8: return (diff < 0.00000001);
				case  9: return (diff < 0.000000001);
				case 10: return (diff < 0.0000000001);
				case 11: return (diff < 0.00000000001);
				case 12: return (diff < 0.000000000001);
				case 13: return (diff < 0.0000000000001);
				case 14: return (diff < 0.00000000000001);
				case 15: return (diff < 0.000000000000001);

				default: throw (new ArgumentOutOfRangeException("digits", digits, MessageHelper.InvalidExecutionPreamble + "Value must be from 0 to 15 but is " + digits + "!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary>
		/// Evaluates whether the two given values are rather not equal,
		/// taking the given number of digits into account.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="digits"/> is less than 0 or greater than 15.
		/// </exception>
		/// <remarks>
		/// Using term "digits" same as e.g. <see cref="Math.Round(double, int)"/>.
		/// </remarks>
		public static bool RatherNotEquals(double lhs, double rhs, int digits)
		{
			return (!RatherEquals(lhs, rhs, digits));
		}

		/// <summary>
		/// Evaluates whether the two given values are rather not equal floating point values.
		/// </summary>
		/// <remarks>
		/// Same as <code>RatherNotEquals(lhs, rhs, 15)</code>.
		/// </remarks>
		public static bool RatherNotEquals(double lhs, double rhs)
		{
			return (RatherNotEquals(lhs, rhs, 15));
		}

		/// <summary>
		/// Evaluates whether the integral part of the two given values is rather not equal.
		/// </summary>
		/// <remarks>
		/// Same as <code>RatherNotEquals(lhs, rhs, 0)</code>.
		/// </remarks>
		public static bool RatherNotEqualsIntegral(double lhs, double rhs)
		{
			return (RatherNotEquals(lhs, rhs, 0));
		}

		/// <summary>
		/// Get the minimum and maximum within <paramref name="collection"/>.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static void GetMinMax(IEnumerable<double> collection, out double min, out double max)
		{
			if (collection.Count() <= 0)
			{
				min = 0.0; // = default(T)
				max = 0.0; // = default(T)
			}
			else
			{
				min = double.MaxValue;
				max = double.MinValue;

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
