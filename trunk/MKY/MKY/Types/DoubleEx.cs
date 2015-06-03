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
// MKY Version 1.0.12
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
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
	/// Double utility methods.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class DoubleEx
	{
		/// <summary>
		/// Limits "value" to the boundaries specified.
		/// </summary>
		public static double LimitToLowerBounds(double value, double lower)
		{
			if (value < lower)
				return (lower);

			return (value);
		}

		/// <summary>
		/// Limits "value" to the boundaries specified.
		/// </summary>
		public static double LimitToUpperBounds(double value, double upper)
		{
			if (value > upper)
				return (upper);

			return (value);
		}

		/// <summary>
		/// Limits "value" to the boundaries specified.
		/// </summary>
		public static double LimitToBounds(double value, double lower, double upper)
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
