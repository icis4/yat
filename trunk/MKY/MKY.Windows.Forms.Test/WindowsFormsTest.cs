//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.10
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;

namespace MKY.Windows.Forms.Test
{
	/// <summary>
	/// Test form for <see cref="FastListBox"/>.
	/// </summary>
	public partial class WindowsFormsTest : Form
	{
		/// <summary>
		/// Creates the test form for <see cref="FastListBox"/>.
		/// </summary>
		public WindowsFormsTest()
		{
			InitializeComponent();
		}

		private void listBoxEx_HorizontalScrolled(object sender, ScrollEventArgs e)
		{
			label_ListBoxEx_HorizontalScrollPositionOld.Text = e.OldValue.ToString();
			label_ListBoxEx_HorizontalScrollPositionNew.Text = e.NewValue.ToString();
			label_ListBoxEx_HorizontalScrollType.Text = e.Type.ToString();
		}

		private void listBoxEx_VerticalScrolled(object sender, ScrollEventArgs e)
		{
			label_ListBoxEx_VerticalScrollPositionOld.Text = e.OldValue.ToString();
			label_ListBoxEx_VerticalScrollPositionNew.Text = e.NewValue.ToString();
			label_ListBoxEx_VerticalScrollType.Text = e.Type.ToString();
		}

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "'increment' does start with a lower case letter.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private int button_ListBoxEx_AddChar_Click_increment;

		private void button_ListBoxEx_AddChar_Click(object sender, EventArgs e)
		{
			ListBoxEx lbe = listBoxEx;
			lbe.BeginUpdate();

			string append = button_ListBoxEx_AddChar_Click_increment.ToString();
			
			button_ListBoxEx_AddChar_Click_increment++;
			if (button_ListBoxEx_AddChar_Click_increment >= 10)
				button_ListBoxEx_AddChar_Click_increment = 0;

			if (lbe.Items.Count > 0)
			{
				string item = lbe.Items[lbe.Items.Count - 1] as string;
				if (item != null)
				{
					item += append;
					lbe.Items[lbe.Items.Count - 1] = item;
				}
				else
				{
					lbe.Items[lbe.Items.Count - 1] = append;
				}
			}
			else
			{
				lbe.Items.Add(append);
			}

			lbe.EndUpdate();

			// \remind MKY 2013-05-25 (related to feature request #163)
			// No feasible way to implement horizontal auto scroll found. There are Win32 API
			// functions to move the position of the scroll bar itself, and to scroll rectangles,
			// but it is not feasible to do the whole translation from .NET Windows.Forms to Win32.
			// Giving up.
		////lbe.HorizontalScrollToEnd();
		}

		private void button_ListBoxEx_AddSeveralChars_Click(object sender, EventArgs e)
		{
			ListBoxEx lbe = listBoxEx;
			lbe.BeginUpdate();

			if (lbe.Items.Count > 0)
			{
				string item = lbe.Items[lbe.Items.Count - 1] as string;
				if (item != null)
				{
					item += "0123456789";
					lbe.Items[lbe.Items.Count - 1] = item;
				}
				else
				{
					lbe.Items[lbe.Items.Count - 1] = "0123456789";
				}
			}
			else
			{
				lbe.Items.Add("0123456789");
			}

			lbe.EndUpdate();

			// \remind MKY 2013-05-25 (related to feature request #163)
			// No feasible way to implement horizontal auto scroll found. There are Win32 API
			// functions to move the position of the scroll bar itself, and to scroll rectangles,
			// but it is not feasible to do the whole translation from .NET Windows.Forms to Win32.
			// Giving up.
		////lbe.HorizontalScrollToEnd();
		}

		private void button_ListBoxEx_AddLine_Click(object sender, EventArgs e)
		{
			ListBoxEx lbe = listBoxEx;
			lbe.BeginUpdate();

			lbe.Items.Add("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");

			lbe.VerticalScrollToBottom();
			lbe.EndUpdate();
		}

		private void button_ListBoxEx_AddManyLines_Click(object sender, EventArgs e)
		{
			ListBoxEx lbe = listBoxEx;
			lbe.BeginUpdate();

			for (int i = 0; i < 50; i++)
				lbe.Items.Add("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");

			lbe.VerticalScrollToBottom();
			lbe.EndUpdate();
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

		private void button_FastListBox_AddLine_Click(object sender, EventArgs e)
		{
			FastListBox flb = fastListBox;
			flb.BeginUpdate();

			flb.Items.Add("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");

			flb.VerticalScrollToBottom();
			flb.EndUpdate();
		}

		private void button_FastListBox_AddManyLines_Click(object sender, EventArgs e)
		{
			FastListBox flb = fastListBox;
			flb.BeginUpdate();

			for (int i = 0; i < 50; i++)
				flb.Items.Add("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");

			flb.VerticalScrollToBottom();
			flb.EndUpdate();
		}

		private void button_FastListBox_RemoveLine_Click(object sender, EventArgs e)
		{
			FastListBox flb = fastListBox;
			flb.BeginUpdate();

			if (flb.Items.Count > 0)
				flb.Items.RemoveAt(0);

			flb.VerticalScrollToBottom();
			flb.EndUpdate();
		}

		private void button_FastListBox_RemoveManyLines_Click(object sender, EventArgs e)
		{
			FastListBox flb = fastListBox;
			flb.BeginUpdate();

			for (int i = 0; i < 50; i++)
			{
				if (flb.Items.Count > 0)
					flb.Items.RemoveAt(0);
			}

			flb.VerticalScrollToBottom();
			flb.EndUpdate();
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
