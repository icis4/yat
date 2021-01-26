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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Forms;

namespace MKY.Windows.Forms
{
	/// <summary>
	/// <see cref="ToolStripMenuItem"/> utility methods.
	/// </summary>
	/// <remarks>
	/// Dedicated methods are required because <see cref="ToolStripMenuItem"/> is no <see cref="Control"/>.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class ToolStripMenuItemEx
	{
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
			// Same code exists in ControlEx.TagToInt().
			// Changes here must be applied there too.

			var menuItem = (sender as ToolStripMenuItem);
			if (menuItem != null)
			{
				var tag = (menuItem.Tag as string);
				if (tag != null)
				{
					return (int.Parse(tag, NumberStyles.Integer, CultureInfo.InvariantCulture)); // Throw if invalid format.
				}

				throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "Sender has an invalid tag '" + tag + "'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, "sender"));
			}

			throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "'" + sender + "' is no 'ToolStripMenuItem'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, "sender"));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
