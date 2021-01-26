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

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;

namespace MKY.Windows.Forms
{
	/// <summary>
	/// <see cref="Form"/> utility methods.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class FormEx
	{
		/// <summary>
		/// Manual <see cref="FormStartPosition.CenterParent"/> because automatic doesn't work
		/// if not shown as dialog.
		/// </summary>
		/// <param name="parent">Parent form.</param>
		/// <param name="child">Child form to be placed to the center of the parent.</param>
		/// <returns>Center parent location.</returns>
		public static Point CalculateManualCenterParentLocation(Control parent, Control child)
		{
			int left = parent.Left + (parent.Width  / 2) - (child.Width  / 2);
			int top  = parent.Top  + (parent.Height / 2) - (child.Height / 2);
			return (new Point(left, top));
		}

		/// <summary>
		/// Fits the requested rectangle to the screen.
		/// </summary>
		/// <param name="location">The location of the requested rectangle.</param>
		/// <param name="size">The size of the requested rectangle.</param>
		/// <returns>Location that fits screen.</returns>
		public static Point CalculateWithinScreenBoundsLocation(Point location, Size size)
		{
			foreach (var screen in Screen.AllScreens)
			{
				if (screen.Bounds.Contains(location))
				{
					var rect = new Rectangle(location, size);
					if (screen.Bounds.Contains(rect))
					{
						return (location); // All fine.
					}
					else
					{
						int x = location.X;
						if ((location.X + size.Width) > screen.Bounds.Width)
							x = (screen.Bounds.Width - size.Width);

						int y = location.Y;
						if ((location.Y + size.Height) > screen.Bounds.Height)
							y = (screen.Bounds.Height - size.Height);

						return (new Point(x, y));
					}
				}
			}

			return (new Point(0, 0)); // Fallback.
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
