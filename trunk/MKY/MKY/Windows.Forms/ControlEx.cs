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
// MKY Version 1.0.25
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2018 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace MKY.Windows.Forms
{
	/// <summary>
	/// <see cref="Control"/> utility methods.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class ControlEx
	{
		/// <summary>
		/// An invalid index is represented by -1 in <see cref="System.Windows.Forms"/> controls.
		/// </summary>
		public const int InvalidIndex = -1;

		/// <summary>
		/// Converts the tag property of <paramref name="sender"/> into an index value.
		/// </summary>
		/// <param name="sender">Control to retrieve the tag from.</param>
		/// <returns>
		/// Index value if the tag property of <paramref name="sender"/> is a valid value;
		/// <see cref="InvalidIndex"/> otherwise.
		/// </returns>
		/// <remarks>
		/// Attention:
		/// This function also exists in <see cref="ToolStripMenuItemEx"/>.
		/// Changes here must be applied there too.
		/// </remarks>
		public static int TagToIndex(object sender)
		{
			var control = sender as Control;
			if (control != null)
			{
				var tag = control.Tag as string;
				if (tag != null)
				{
					int index;
					if (int.TryParse(tag, NumberStyles.Integer, CultureInfo.InvariantCulture, out index))
						return (index);
				}

				throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "Sender has an invalid tag '" + tag + "'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, "sender"));
			}

			throw (new ArgumentOutOfRangeException("sender", sender, MessageHelper.InvalidExecutionPreamble + "'" + sender + "' is no control!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

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
		/// Set focus to <paramref name="sender"/> if that can have the focus; otherwise, focus is
		/// forwarded to the next possible control.
		/// </summary>
		/// <param name="sender">Control to set or forward focus.</param>
		/// <returns><c>true</c> if focus was set or forwarded, <c>false</c> if failed.</returns>
		public static bool SetOrForwardFocus(object sender)
		{
			var control = sender as Control;
			if (control != null)
			{
				if (control.Focus())
					return (true);

				if (control.SelectNextControl(control, true, true, true, true))
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
