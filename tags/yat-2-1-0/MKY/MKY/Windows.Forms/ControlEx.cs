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
// MKY Version 1.0.27
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
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Field name is given by the Win32 API.")]
		private const int WM_SETREDRAW = 0x000B;

		/// <summary>
		/// An invalid index is represented by -1 in <see cref="System.Windows.Forms"/> controls.
		/// </summary>
		public const int InvalidIndex = -1;

		/// <summary>
		/// Converts the tag property of <paramref name="sender"/> into an integer value.
		/// </summary>
		/// <param name="sender">Control to retrieve the tag from.</param>
		/// <returns>
		/// Value of the tag property of <paramref name="sender"/>.
		/// </returns>
		/// <exception cref="ArgumentException"><paramref name="sender"/> is not a <see cref="Control"/>.</exception>
		/// <exception cref="ArgumentException">The tag property of <paramref name="sender"/> is not a string.</exception>
		/// <exception cref="OverflowException">The tag property of <paramref name="sender"/> represents a number less than <see cref="int.MinValue"/> or greater than <see cref="int.MaxValue"/>.</exception>
		/// <exception cref="OverflowException">The tag property of <paramref name="sender"/> includes non-zero, fractional digits.</exception>
		public static int TagToInt32(object sender)
		{
			// Attention:
			// Same code exists in ToolStripMenuItemEx.TagToInt().
			// Changes here must be applied there too.

			var control = (sender as Control);
			if (control != null)
			{
				var tag = (control.Tag as string);
				if (tag != null)
				{
					return (int.Parse(tag, NumberStyles.Integer, CultureInfo.InvariantCulture)); // Throw if invalid format.
				}

				throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "Sender has an invalid tag '" + tag + "'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, "sender"));
			}

			throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "'" + sender + "' is no 'Control'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, "sender"));
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

		/// <summary>
		/// Suspends painting of the specified control.
		/// </summary>
		public static void SuspendUpdate(Control control)
		{
			Message msg = Message.Create(control.Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);

			NativeWindow window = NativeWindow.FromHandle(control.Handle);
			window.DefWndProc(ref msg);
		}

		/// <summary>
		/// Resumes painting of the specified control.
		/// </summary>
		public static void ResumeUpdate(Control control)
		{
			IntPtr wparam = new IntPtr(1); // C "true" as an IntPtr.
			Message msg = Message.Create(control.Handle, WM_SETREDRAW, wparam, IntPtr.Zero);

			NativeWindow window = NativeWindow.FromHandle(control.Handle);
			window.DefWndProc(ref msg);

			control.Invalidate();
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
