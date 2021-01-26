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
	/// <see cref="Screen"/> utility methods.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class ScreenEx
	{
		/// <summary>
		/// Evaluates whether the given <paramref name="rect"/> is completely located within the
		/// bounds of any of the screens.
		/// </summary>
		/// <param name="rect">The rectangle to be evaluated.</param>
		/// <returns><c>true</c> if within; otherwise, <c>false</c>.</returns>
		public static bool IsWithinAnyBounds(Rectangle rect)
		{
			foreach (var screen in Screen.AllScreens)
			{
				var bounds = screen.Bounds;
				if (bounds.Contains(rect))
					return (true);
			}

			return (false);
		}

		/// <summary>
		/// Evaluates whether the given <paramref name="rect"/> is completely located within the
		/// working area of any of the screens.
		/// </summary>
		/// <param name="rect">The rectangle to be evaluated.</param>
		/// <returns><c>true</c> if within; otherwise, <c>false</c>.</returns>
		public static bool IsWithinAnyWorkingArea(Rectangle rect)
		{
			foreach (var screen in Screen.AllScreens)
			{
				var workingArea = screen.WorkingArea;
				if (workingArea.Contains(rect))
					return (true);
			}

			return (false);
		}

		/// <summary>
		/// Fits the requested rectangle to the screen's working area.
		/// </summary>
		/// <param name="location">The location of the requested rectangle.</param>
		/// <param name="size">The size of the requested rectangle.</param>
		/// <returns>Location that fits screen.</returns>
		public static Point CalculateLocationWithinWorkingArea(Point location, Size size)
		{
			foreach (var screen in Screen.AllScreens)
			{
				var workingArea = screen.WorkingArea;
				if (workingArea.Contains(location))
				{
					var rect = new Rectangle(location, size);
					if (workingArea.Contains(rect))
					{
						return (location); // All fine.
					}
					else
					{
						int x = location.X;
						if ((location.X + size.Width) > workingArea.Right)
							x = (workingArea.Right - size.Width);

						if (x < workingArea.X)
							x = workingArea.X;

						int y = location.Y;
						if ((location.Y + size.Height) > workingArea.Bottom)
							y = (workingArea.Bottom - size.Height);

						if (y < workingArea.Y)
							y = workingArea.Y;

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
