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
// MKY Development Version 1.0.18
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Diagnostics.CodeAnalysis;

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
		/// Limits <paramref name="value"/> to the <paramref name="min"/> value specified.
		/// </summary>
		public static float LimitMin(float value, float min)
		{
			if (value < min)
				return (min);

			return (value);
		}

		/// <summary>
		/// Limits <paramref name="value"/> to the <paramref name="max"/> value specified.
		/// </summary>
		public static float LimitMax(float value, float max)
		{
			if (value > max)
				return (max);

			return (value);
		}

		/// <summary>
		/// Limits <paramref name="value"/> to the values specified.
		/// </summary>
		public static float Limit(float value, float min, float max)
		{
			if (value < min)
				return (min);

			if (value > max)
				return (max);

			return (value);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
