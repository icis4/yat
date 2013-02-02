//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.9
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2013 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Diagnostics.CodeAnalysis;

// This code is intentionally placed into the MKY namespace even though the file is located in
// MKY.Types for consistency with the Sytem namespace.
namespace MKY
{
	/// <summary>
	/// Single utility methods.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class SingleEx
	{
		/// <summary>
		/// Limits "value" to the boundaries specified.
		/// </summary>
		public static float LimitToLowerBounds(float value, float lower)
		{
			if (value < lower)
				return (lower);

			return (value);
		}

		/// <summary>
		/// Limits "value" to the boundaries specified.
		/// </summary>
		public static float LimitToUpperBounds(float value, float upper)
		{
			if (value > upper)
				return (upper);

			return (value);
		}

		/// <summary>
		/// Limits "value" to the boundaries specified.
		/// </summary>
		public static float LimitToBounds(float value, float lower, float upper)
		{
			if (value < lower)
				return (lower);

			if (value > upper)
				return (upper);

			return (value);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
