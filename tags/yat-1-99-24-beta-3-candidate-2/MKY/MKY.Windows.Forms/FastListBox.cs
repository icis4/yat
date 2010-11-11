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
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
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
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool normal_UserPaint;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public FastListBox()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

			StoreNormalPaintStyle();
			SetUserPaintStyle();
		}

		#endregion

		#region Overridden Properties
		//==========================================================================================
		// Overridden Properties
		//==========================================================================================

		/// <summary></summary>
		public override DrawMode DrawMode
		{
			get { return base.DrawMode; }
			set
			{
				if (value == DrawMode.Normal)
					RestoreNormalPaintStyle();
				else
					SetUserPaintStyle();

				base.DrawMode = value;
			}
		}

		#endregion

		#region Overridden Methods
		//==========================================================================================
		// Overridden Methods
		//==========================================================================================

		/// <remarks>
		/// Is only called if draw mode is <see cref="System.Windows.Forms.DrawMode.OwnerDrawFixed"/>
		/// or <see cref="System.Windows.Forms.DrawMode.OwnerDrawVariable"/>.
		/// If draw mode is <see cref="System.Windows.Forms.DrawMode.Normal"/>,
		/// only ListBox.OnDrawItem() is called.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "Inconsequent naming in .NET.")]
		protected override void OnPaintBackground(PaintEventArgs e)
		{
			base.OnPaintBackground(e);
		}

		/// <remarks>
		/// Is only called if draw mode is <see cref="System.Windows.Forms.DrawMode.OwnerDrawFixed"/>
		/// or <see cref="System.Windows.Forms.DrawMode.OwnerDrawVariable"/>.
		/// If draw mode is <see cref="System.Windows.Forms.DrawMode.Normal"/>,
		/// only ListBox.OnDrawItem() is called.
		/// </remarks>
		protected override void OnPaint(PaintEventArgs e)
		{
			int maxVisibleItems = (int)Math.Ceiling((float)Height / (float)ItemHeight);
			int maxBottomIndex = TopIndex + maxVisibleItems;
			int bottomIndex = Math.Min(maxBottomIndex, Items.Count);

			for (int i = TopIndex; i < bottomIndex; i++)
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

				// Set normal/selected state.
				DrawItemState state = DrawItemState.Default;
				foreach (int index in SelectedIndices)
				{
					if (index == i)
					{
						state = DrawItemState.Selected;
						break;
					}
				}

				// Request drawing of item.
				OnDrawItem(new DrawItemEventArgs(e.Graphics, Font, bounds, i, state));
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary>
		/// Scroll list to bottom if no items are selected.
		/// </summary>
		public virtual void ScrollToBottomIfNoItemsSelected()
		{
			if ((SelectedItems.Count == 0) && (Items.Count > 0))
				TopIndex = Items.Count - 1;
		}

		#endregion

		#region Protected Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		/// <summary></summary>
		protected virtual void StoreNormalPaintStyle()
		{
			this.normal_UserPaint = GetStyle(ControlStyles.UserPaint);
		}

		/// <summary></summary>
		protected virtual void RestoreNormalPaintStyle()
		{
			SetStyle(ControlStyles.UserPaint, this.normal_UserPaint);
		}

		/// <summary></summary>
		protected virtual void SetUserPaintStyle()
		{
			SetStyle(ControlStyles.UserPaint, true);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
