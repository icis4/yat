//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.7
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Drawing;
using System.Windows.Forms;

namespace MKY.Windows.Forms
{
	/// <summary>
	/// <see cref="System.Windows.Forms"/> utility methods.
	/// </summary>
	public static class ControlEx
	{
		/// <summary>
		/// An invalid index is represented by -1 in <see cref="System.Windows.Forms"/> controls.
		/// </summary>
		public const int InvalidIndex = -1;

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
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
