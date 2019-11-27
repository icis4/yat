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

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;

namespace MKY.Windows.Forms
{
	/// <summary>
	/// <see cref="Screen"/> utility methods.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class ScreenEx
	{
		/// <summary>
		/// Evaluates whether the given <paramref name="bounds"/> are within the bounds of any of the screens.
		/// </summary>
		/// <param name="bounds">The bounds to be evaluated.</param>
		/// <returns><c>true</c> if within; otherwise, <c>false</c>.</returns>
		public static bool IsWithinAnyBounds(Rectangle bounds)
		{
			foreach (var screen in Screen.AllScreens)
			{
				if (screen.Bounds.Contains(bounds))
					return (true);
			}

			return (false);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
