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
// MKY Development Version 1.0.8
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Drawing;
using System.Windows.Forms;

namespace MKY.Windows.Forms.Test
{
	/// <summary>
	/// Test form for <see cref="FastListBox"/>.
	/// </summary>
	public partial class FastListBoxTest : Form
	{
		/// <summary>
		/// Creates the test form for <see cref="FastListBox"/>.
		/// </summary>
		public FastListBoxTest()
		{
			InitializeComponent();
		}

		private void fastListBox_DrawItem(object sender, DrawItemEventArgs e)
		{
			if (e.Index >= 0)
			{
				e.DrawBackground();

				string s = (string)this.fastListBox.Items[e.Index];
				Brush brush = SystemBrushes.ControlText;
				Font font = SystemFonts.DefaultFont;
				StringFormat sf = StringFormat.GenericTypographic;
				e.Graphics.DrawString(s, font, brush, e.Bounds, sf);
				e.Graphics.MeasureString(s, font, e.Bounds.Size, sf);

				e.DrawFocusRectangle();
			}
		}

		private void button_Add_Click(object sender, EventArgs e)
		{
			FastListBox flb = this.fastListBox;
			flb.BeginUpdate();

			flb.Items.Add("TESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTEST");

			// Scroll list.
			if ((flb.SelectedItems.Count == 0) && (flb.Items.Count > 0))
				flb.TopIndex = flb.Items.Count - 1;

			flb.EndUpdate();
		}

		private void button_AddMany_Click(object sender, EventArgs e)
		{
			FastListBox flb = this.fastListBox;
			flb.BeginUpdate();

			for (int i = 0; i < 50; i++)
				flb.Items.Add("TESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTEST");

			// Scroll list.
			if ((flb.SelectedItems.Count == 0) && (flb.Items.Count > 0))
				flb.TopIndex = flb.Items.Count - 1;

			flb.EndUpdate();
		}

		private void button_Remove_Click(object sender, EventArgs e)
		{
			FastListBox flb = this.fastListBox;
			flb.BeginUpdate();

			if (flb.Items.Count > 0)
				flb.Items.RemoveAt(0);

			// Scroll list.
			if ((flb.SelectedItems.Count == 0) && (flb.Items.Count > 0))
				flb.TopIndex = flb.Items.Count - 1;

			flb.EndUpdate();
		}

		private void button_RemoveMany_Click(object sender, EventArgs e)
		{
			FastListBox flb = this.fastListBox;
			flb.BeginUpdate();

			for (int i = 0; i < 50; i++)
			{
				if (flb.Items.Count > 0)
					flb.Items.RemoveAt(0);
			}

			// Scroll list.
			if ((flb.SelectedItems.Count == 0) && (flb.Items.Count > 0))
				flb.TopIndex = flb.Items.Count - 1;

			flb.EndUpdate();
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
