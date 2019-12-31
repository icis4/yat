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
	/// Single utility methods.
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
				throw (new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Precondition is 'min' <= 'max', but 'min' is {0} and 'max' is {1}!", min, max))); // Do not append 'MessageHelper.InvalidExecutionPreamble' as caller could rely on this exception text.

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
		/// <paramref name="digits"/> is less than 0 or greater than 7.
		/// </exception>
		public static bool AlmostEquals(float lhs, float rhs, int digits)
		{
			float diff = System.Math.Abs(lhs - rhs);

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
		public static bool RatherNotEquals(float lhs, float rhs, int digits)
		{
			return (!AlmostEquals(lhs, rhs, digits));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
