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
// MKY Development Version 1.0.18
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
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
		/// This function also exists in <see cref="ControlEx"/>.
		/// Changes here must be applied there too.
		/// </remarks>
		public static int TagToIndex(object sender)
		{
			ToolStripMenuItem control = sender as ToolStripMenuItem;
			if (control != null)
			{
				string tag = control.Tag as string;
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
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
