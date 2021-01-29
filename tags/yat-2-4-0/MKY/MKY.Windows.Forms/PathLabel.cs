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

using System.Drawing;
using System.Windows.Forms;

using MKY.Drawing;

namespace MKY.Windows.Forms
{
	/// <summary>
	/// Provides a label that displays path strings with ellipsis.
	/// </summary>
	public class PathLabel : Label
	{
		/// <summary>
		/// Draws string with ellipsis.
		/// </summary>
		protected override void OnPaint(PaintEventArgs e)
		{
			// Try to fit file path with path ellipsis:
			TextFormatFlags pathEllipsisFlags = DrawingEx.ConvertContentAlignmentToTextFormatFlags(TextAlign);

			if (RightToLeft == RightToLeft.Yes)
				pathEllipsisFlags |= TextFormatFlags.RightToLeft;

			pathEllipsisFlags |= TextFormatFlags.SingleLine;
			pathEllipsisFlags |= TextFormatFlags.PathEllipsis;

			Size measuredSize = TextRenderer.MeasureText(e.Graphics, Text, Font, ClientRectangle.Size, pathEllipsisFlags);
			if (measuredSize.Width <= e.ClipRectangle.Width)
			{
				TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, ForeColor, BackColor, pathEllipsisFlags);
			}
			else
			{
				// Path ellipsis, don't fit, draw text with end ellipsis:
				TextFormatFlags endEllipsisFlags = DrawingEx.ConvertContentAlignmentToTextFormatFlags(TextAlign);

				if (RightToLeft == RightToLeft.Yes)
					endEllipsisFlags |= TextFormatFlags.RightToLeft;

				endEllipsisFlags |= TextFormatFlags.SingleLine;
				endEllipsisFlags |= TextFormatFlags.EndEllipsis;

				TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, ForeColor, BackColor, endEllipsisFlags);
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
