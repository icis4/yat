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
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Drawing;
using System.Windows.Forms;

namespace MKY.Drawing
{
	/// <summary>
	/// Drawing utility methods.
	/// </summary>
	public static class DrawingEx
	{
		/// <summary>
		/// Converts a <see cref="ContentAlignment"/> enum to a <see cref="TextFormatFlags"/> enum.
		/// Can be used for user drawn controls.
		/// </summary>
		/// <param name="contentAlignment">ContentAlignment to be converted.</param>
		/// <returns>Converted TextFormatFlags.</returns>
		public static TextFormatFlags ConvertContentAlignmentToTextFormatFlags(ContentAlignment contentAlignment)
		{
			return (ApplyContentAlignmentToTextFormatFlags(contentAlignment, TextFormatFlags.Default));
		}

		/// <summary>
		/// Applies a <see cref="ContentAlignment"/> enum to a <see cref="TextFormatFlags"/> enum.
		/// Can be used for user drawn controls.
		/// </summary>
		/// <param name="contentAlignment">ContentAlignment to be converted.</param>
		/// <param name="textFormatFlags">TextFormatFlags to be changed.</param>
		/// <returns>Changed TextFormatFlags.</returns>
		public static TextFormatFlags ApplyContentAlignmentToTextFormatFlags(ContentAlignment contentAlignment, TextFormatFlags textFormatFlags)
		{
			// ATTENTION
			// Do not care about TextFormatFlags.Top and TextFormatFlags.Left; their values are 0.

			// Clear alignment related flags.
		////textFormatFlags &= ~TextFormatFlags.Top;
			textFormatFlags &= ~TextFormatFlags.VerticalCenter;
			textFormatFlags &= ~TextFormatFlags.Bottom;

		////textFormatFlags &= ~TextFormatFlags.Left;
			textFormatFlags &= ~TextFormatFlags.HorizontalCenter;
			textFormatFlags &= ~TextFormatFlags.Right;

			// Set alignment related flags.
			switch (contentAlignment)
			{
				case ContentAlignment.TopLeft:
				////textFormatFlags |= TextFormatFlags.Top;
				////textFormatFlags |= TextFormatFlags.Left;
					break;

				case ContentAlignment.MiddleLeft:
					textFormatFlags |= TextFormatFlags.VerticalCenter;
				////textFormatFlags |= TextFormatFlags.Left;
					break;

				case ContentAlignment.BottomLeft:
					textFormatFlags |= TextFormatFlags.Bottom;
				////textFormatFlags |= TextFormatFlags.Left;
					break;

				case ContentAlignment.TopCenter:
				////textFormatFlags |= TextFormatFlags.Top;
					textFormatFlags |= TextFormatFlags.HorizontalCenter;
					break;

				case ContentAlignment.MiddleCenter:
					textFormatFlags |= TextFormatFlags.VerticalCenter;
					textFormatFlags |= TextFormatFlags.HorizontalCenter;
					break;

				case ContentAlignment.BottomCenter:
					textFormatFlags |= TextFormatFlags.Bottom;
					textFormatFlags |= TextFormatFlags.HorizontalCenter;
					break;

				case ContentAlignment.TopRight:
				////textFormatFlags |= TextFormatFlags.Top;
					textFormatFlags |= TextFormatFlags.Right;
					break;

				case ContentAlignment.MiddleRight:
					textFormatFlags |= TextFormatFlags.VerticalCenter;
					textFormatFlags |= TextFormatFlags.Right;
					break;

				case ContentAlignment.BottomRight:
					textFormatFlags |= TextFormatFlags.Bottom;
					textFormatFlags |= TextFormatFlags.Right;
					break;
			}
			return (textFormatFlags);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
