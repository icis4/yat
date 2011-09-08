//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL: https://y-a-terminal.svn.sourceforge.net/svnroot/y-a-terminal/trunk/MKY/MKY/Types/DoubleEx.cs $
// $Author: klaey-1 $
// $Date: 2011/08/24 13:38:39MESZ $
// $Revision: 1.1 $
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

// This code is intentionally placed into the MKY namespace even though the file is located in
// MKY.Types for consistency with the Sytem namespace.
namespace MKY
{
	/// <summary>
	/// Single utility methods.
	/// </summary>
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
// $URL: https://y-a-terminal.svn.sourceforge.net/svnroot/y-a-terminal/trunk/MKY/MKY/Types/DoubleEx.cs $
//==================================================================================================
