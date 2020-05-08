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
// MKY Version 1.0.28 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Configuration
//==================================================================================================
// Configuration
//==================================================================================================

#if (DEBUG)

	// Attention, requires to be enabled in multiple files!
////#define DEBUG_COUNT_AND_INDICES // The 'DebugEnabled' property must also be set!

#endif // DEBUG

#endregion

using System.Drawing;
using System.Windows.Forms;

namespace MKY.Windows.Forms
{
	/// <summary>
	/// Fast implementation of a list box. The original <see cref="ListBox"/> is rather slow if
	/// there are many consecutive updates/adds. The difference among the different variants is
	/// demonstrated in the 'MKY.Windows.Forms.Test' test application:
	///  > The <see cref="ListBox"/> and <see cref="ListBoxEx"/> flicker on vertical scrolling.
	///  > This <see cref="FastListBox"/> doesn't flicker on vertical scrolling.
	/// </summary>
	public class FastListBox : ListBoxEx
	{
		private bool defaultUserPaintStyle;

		/// <summary></summary>
		public FastListBox()
		{
			this.defaultUserPaintStyle = GetStyle(ControlStyles.UserPaint); // Store default setting.

			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
		}

		/// <summary></summary>
		public override DrawMode DrawMode
		{
			get { return (base.DrawMode); }
			set
			{
				if (value == DrawMode.Normal)
					SetStyle(ControlStyles.UserPaint, this.defaultUserPaintStyle); // Restore default setting.
				else
					SetStyle(ControlStyles.UserPaint, true);

				base.DrawMode = value;
			}
		}

		/// <remarks>
		/// Is only called if draw mode is <see cref="System.Windows.Forms.DrawMode.OwnerDrawFixed"/>
		/// or <see cref="System.Windows.Forms.DrawMode.OwnerDrawVariable"/>.
		/// If draw mode is <see cref="System.Windows.Forms.DrawMode.Normal"/>,
		/// only ListBox.OnDrawItem() is called.
		/// </remarks>
		protected override void OnPaint(PaintEventArgs e)
		{
			if (Items.Count > 0) // Attention, initially 'TopIndex' = 'BottomIndex' = 0!
			{
				DebugCountAndIndices("Indices are going to be retrieved");

				for (int i = TopIndex; i <= BottomIndex; i++) // Note that indices work fine here.
				{
					// Calculate bounding box taking the beginning of the item rectangle into account:
					// e.g. scroll position somewhere in between...
					//         ------XX------
					// ...results in a complete rectangle (HorizontalExtent) of...
					//    ------------------------
					// ...and a text position of...
					//    ABCDEFGHIJ
					// ...thus the beginning of the text (x) is a negative value left of the visible rectangle...
					//    (x)
					int offset = i - TopIndex;
					Rectangle ir = GetItemRectangle(i);
					int x = DisplayRectangle.Width - ir.Width; // (x).
					Rectangle bounds = new Rectangle(x, offset * ItemHeight, ir.Width, ItemHeight);

					// Set normal/selected state:
					DrawItemState state;
					if (SelectedIndices.Contains(i))
						state = DrawItemState.Selected;
					else
						state = DrawItemState.Default;

					// Request drawing of item:
					OnDrawItem(new DrawItemEventArgs(e.Graphics, Font, bounds, i, state));
				}
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
