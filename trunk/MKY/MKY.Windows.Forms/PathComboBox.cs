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
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright � 2003-2004 HSR Hochschule f�r Technik Rapperswil.
// Copyright � 2003-2010 Matthias Kl�y.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MKY.Windows.Forms
{
	/// <summary>
	/// Provides a label that displays path strings with ellipsis.
	/// </summary>
	[DesignerCategory("Windows Forms")]
	public partial class PathComboBox : ComboBox
	{
		/// <summary></summary>
		public PathComboBox()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Draws string with ellipsis.
		/// </summary>
		protected override void OnPaint(PaintEventArgs e)
		{
			// Try to fit file path with path ellipsis.
			TextFormatFlags pathEllipsisFlags = TextFormatFlags.Default;

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
				// Path ellipsis, don't fit, draw text with end ellipsis.
				TextFormatFlags endEllipsisFlags = TextFormatFlags.Default;

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
