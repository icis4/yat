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
	/// <see cref="Single"/>/<see cref="float"/> utility methods.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class SingleEx
	{
		/// <summary>
		/// Limits <paramref name="value"/> to the values specified.
		/// </summary>
		/// <exception cref="ArgumentException">
		/// <paramref name="min"/> is larger than <paramref name="max"/>.
		/// </exception>
		public static float Limit(float value, float min, float max)
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
		public static bool IsWithin(float value, float min, float max)
		{
			return ((value >= min) && (value <= max));
		}

		/// <summary>
		/// Evaluates whether the two given values are almost equal integral values.
		/// </summary>
		/// <remarks>
		/// Same as <code>AlmostEquals(lhs, rhs, 0)</code>.
		/// </remarks>
		public static bool AlmostEquals(float lhs, float rhs)
		{
			return (AlmostEquals(lhs, rhs, 0));
		}

		/// <summary>
		/// Evaluates whether the two given values are almost equal,
		/// taking the given number of digits into account.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="digits"/> is less than 0 or greater than 7.
		/// </exception>
		/// <remarks>
		/// Using term "digits" same as e.g. <see cref="Math.Round(double, int)"/>.
		/// </remarks>
		public static bool AlmostEquals(float lhs, float rhs, int digits)
		{
			float diff = Math.Abs(lhs - rhs);

			switch (digits)
			{
				case  0: return (diff < 1.0f);
				case  1: return (diff < 0.1f);
				case  2: return (diff < 0.01f);
				case  3: return (diff < 0.001f);
				case  4: return (diff < 0.0001f);
				case  5: return (diff < 0.00001f);
				case  6: return (diff < 0.000001f);
				case  7: return (diff < 0.0000001f);

				default: throw (new ArgumentOutOfRangeException("digits", digits, MessageHelper.InvalidExecutionPreamble + "Value must be from 0 to 7 but is " + digits + "!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary>
		/// Evaluates whether the two given values are rather not equal,
		/// taking the given number of digits into account.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="digits"/> is less than 0 or greater than 7.
		/// </exception>
		/// <remarks>
		/// Using term "digits" same as e.g. <see cref="Math.Round(double, int)"/>.
		/// </remarks>
		public static bool RatherNotEquals(float lhs, float rhs, int digits)
		{
			return (!AlmostEquals(lhs, rhs, digits));
		}

		/// <summary>
		/// Evaluates whether the two given values are rather not equal integral values.
		/// </summary>
		/// <remarks>
		/// Same as <code>RatherNotEquals(lhs, rhs, 0)</code>.
		/// </remarks>
		public static bool RatherNotEquals(float lhs, float rhs)
		{
			return (RatherNotEquals(lhs, rhs, 0));
		}

		/// <summary>
		/// Get the minimum and maximum within <paramref name="collection"/>.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static void GetMinMax(IEnumerable<float> collection, out float min, out float max)
		{
			if (collection.Count() <= 0)
			{
				min = 0.0f; // = default(T)
				max = 0.0f; // = default(T)
			}
			else
			{
				min = float.MaxValue;
				max = float.MinValue;

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
