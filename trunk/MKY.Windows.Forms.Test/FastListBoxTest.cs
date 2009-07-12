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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MKY.Windows.Forms.Test
{
	public partial class FastListBoxTest : Form
	{
		public FastListBoxTest()
		{
			InitializeComponent();
		}

		private void fastListBox_DrawItem(object sender, DrawItemEventArgs e)
		{
			if (e.Index >= 0)
			{
				e.DrawBackground();

				string s = (string)fastListBox.Items[e.Index];
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
			fastListBox.Items.Add("TESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTEST");
		}

		private void button_AddMany_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < 50; i++)
				fastListBox.Items.Add("TESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTEST");
		}

		private void button_Remove_Click(object sender, EventArgs e)
		{
			fastListBox.Items.RemoveAt(0);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
