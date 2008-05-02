using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace MKY.Utilities.Windows.Forms
{
	/// <summary>
	/// System.Windows.Forms utility methods.
	/// </summary>
	public static class XForm
	{
		/// <summary>
		/// Manual <see cref="FormStartPosition.CenterParent"/> because automatic doesn't work
		/// if not shown as dialog.
		/// </summary>
		/// <param name="parent">Parent form.</param>
		/// <param name="child">Child form to be placed to the center of the parent.</param>
		/// <returns>Center parent location</returns>
		public static Point CalculateManualCenterParentLocation(Form parent, Form child)
		{
			int left = parent.Left + (parent.Width  / 2) - (child.Width  / 2);
			int top  = parent.Top  + (parent.Height / 2) - (child.Height / 2);
			return (new Point(left, top));
		}
	}
}
