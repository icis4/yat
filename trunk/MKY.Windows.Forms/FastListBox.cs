//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2009 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MKY.Windows.Forms
{
	/// <summary>
	/// Fast implementation of a list box. The original <see cref="ListBox"/> is rather slow if
	/// there are many consequent updates/adds.
	/// </summary>
	[DesignerCategory("Windows Forms")]
	public class FastListBox : ListBox
	{
		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public FastListBox()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.UserPaint, true);
		}

		#endregion

		#region Overridden Methods
		//==========================================================================================
		// Overridden Methods
		//==========================================================================================

		/// <summary></summary>
		protected override void OnPaintBackground(PaintEventArgs e)
		{
			base.OnPaintBackground(e);
		}

		/// <summary></summary>
		protected override void OnPaint(PaintEventArgs e)
		{
			int maxVisibleItems = (int)Math.Ceiling((float)Height / (float)ItemHeight);
			int maxBottomIndex = TopIndex + maxVisibleItems;
			int bottomIndex = Math.Min(maxBottomIndex, Items.Count);

			for (int i = TopIndex; i < bottomIndex; i++)
			{
				int offset = i - TopIndex;
				Rectangle dr = new Rectangle(e.ClipRectangle.X, offset * ItemHeight,
											 e.ClipRectangle.Width, ItemHeight);

				//System.Diagnostics.Debug.WriteLine(offset + " :: CR :: " + e.ClipRectangle);
				//Rectangle ir = GetItemRectangle(i);
				//System.Diagnostics.Debug.WriteLine(offset + " :: IR :: " + ir);

				//Rectangle dr;
				/*if (e.ClipRectangle.X == 0) // moving right
				{
					if (e.ClipRectangle.Width == Width)
						dr = new Rectangle(e.ClipRectangle.Width - ir.Width, offset * ItemHeight, ir.Width, ItemHeight);
					else
						dr = new Rectangle(e.ClipRectangle.X, offset * ItemHeight, ir.Width, ItemHeight);
				}
				else                        // moving left
				{
					if (e.ClipRectangle.Width == Width)
						dr = new Rectangle(e.ClipRectangle.Width - ir.Width, offset * ItemHeight, ir.Width, ItemHeight);
					else
						dr = new Rectangle(e.ClipRectangle.X, offset * ItemHeight, ir.Width, ItemHeight);
				}*/
				//dr = new Rectangle(e.ClipRectangle.Width - ir.Width, offset * ItemHeight, e.ClipRectangle.Width, ItemHeight);
				//System.Diagnostics.Debug.WriteLine(offset + " :: DR :: " + dr);

				DrawItemState state = DrawItemState.Default;
				foreach (int index in SelectedIndices)
				{
					if (index == i)
					{
						state = DrawItemState.Selected;
						break;
					}
				}

				base.OnDrawItem(new DrawItemEventArgs(e.Graphics, Font, dr, i, state));
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
