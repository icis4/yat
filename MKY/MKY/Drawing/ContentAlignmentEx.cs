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
// MKY Version 1.0.30
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

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;

namespace MKY.Drawing
{
	/// <summary>
	/// This class provides conversion methods for <see cref="ContentAlignment"/>.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class ContentAlignmentEx
	{
		/// <summary>
		/// Converts a <see cref="ContentAlignment"/> enum to a <see cref="TextFormatFlags"/> enum.
		/// Can be used for user drawn controls.
		/// </summary>
		/// <param name="contentAlignment"><see cref="ContentAlignment"/> to be converted.</param>
		/// <returns>Converted <see cref="TextFormatFlags"/>.</returns>
		public static TextFormatFlags ConvertTo(ContentAlignment contentAlignment)
		{
			return (ApplyTo(contentAlignment));
		}

		/// <summary>
		/// Applies a <see cref="ContentAlignment"/> enum to a <see cref="TextFormatFlags"/> enum.
		/// Can be used for user drawn controls.
		/// </summary>
		/// <param name="contentAlignment"><see cref="ContentAlignment"/> to be converted.</param>
		/// <param name="textFormatFlags"><see cref="TextFormatFlags"/> to be changed.</param>
		/// <returns>Changed <see cref="TextFormatFlags"/>.</returns>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static TextFormatFlags ApplyTo(ContentAlignment contentAlignment, TextFormatFlags textFormatFlags = TextFormatFlags.Default)
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
