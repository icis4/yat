using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using HSR.Utilities.Drawing;

namespace HSR.Windows.Forms
{
	/// <summary>
	/// Provides a label that displays path strings with ellipsis.
	/// </summary>
	[DesignerCategory("Windows Forms")]
	public partial class PathLabel : Label
	{
		/// <summary></summary>
		public PathLabel()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Draws string with ellipsis
		/// </summary>
		protected override void OnPaint(PaintEventArgs e)
		{
			// try to fit file path with path ellipsis
			TextFormatFlags pathEllipsisFlags = XDrawing.ConvertContentAlignmentToTextFormatFlags(TextAlign);

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
				// path ellipsis, don't fit, draw text with end ellipsis
				TextFormatFlags endEllipsisFlags = XDrawing.ConvertContentAlignmentToTextFormatFlags(TextAlign);

				if (RightToLeft == RightToLeft.Yes)
					endEllipsisFlags |= TextFormatFlags.RightToLeft;

				endEllipsisFlags |= TextFormatFlags.SingleLine;
				endEllipsisFlags |= TextFormatFlags.EndEllipsis;

				TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, ForeColor, BackColor, endEllipsisFlags);
			}
		}
	}
}
