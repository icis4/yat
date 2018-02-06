﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.24 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2018 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;

// This code is intentionally placed into the MKY namespace even though the file is located in
// MKY.Types for consistency with the System namespace.
namespace MKY
{
	/// <summary>
	/// Double utility methods.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class DoubleEx
	{
		/// <summary>
		/// Limits <paramref name="value"/> to the min value specified.
		/// </summary>
		public static double LimitMin(double value, double min)
		{
			if (value < min)
				return (min);

			return (value);
		}

		/// <summary>
		/// Limits <paramref name="value"/> to the max value specified.
		/// </summary>
		public static double LimitMax(double value, double max)
		{
			if (value > max)
				return (max);

			return (value);
		}

		/// <summary>
		/// Limits <paramref name="value"/> to the values specified.
		/// </summary>
		public static double Limit(double value, double min, double max)
		{
			if (value < min)
				return (min);

			if (value > max)
				return (max);

			return (value);
		}

		/// <summary>
		/// Evaluates whether the two given values are almost equal,
		/// taking the given number of digits into account.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="digits"/> is less than 0 or greater than 15.
		/// </exception>
		public static bool AlmostEquals(double lhs, double rhs, int digits)
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
		public static bool RatherNotEquals(double lhs, double rhs, int digits)
		{
			return (!AlmostEquals(lhs, rhs, digits));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
