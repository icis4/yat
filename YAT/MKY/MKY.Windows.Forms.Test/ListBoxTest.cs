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
// Copyright © 2007-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Forms;

#endregion

#region Module-level StyleCop suppressions
//==================================================================================================
// Module-level StyleCop suppressions
//==================================================================================================

[module: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1404:CodeAnalysisSuppressionMustHaveJustification", Justification = "Large blocks of module-level FxCop suppressions which were copy-pasted out of FxCop.")]

#endregion

#region Module-level FxCop suppressions
//==================================================================================================
// Module-level FxCop suppressions
//==================================================================================================

// Justification = "Used in case of 'ENABLE_HORIZONTAL_AUTO_SCROLL'. Not #if-able as this method is created by the designer."
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "MKY.Windows.Forms.Test.ListBoxTest.#listBoxEx_HorizontalScrolled(System.Object,System.Windows.Forms.ScrollEventArgs)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "MKY.Windows.Forms.Test.ListBoxTest.#listBoxEx_VerticalScrolled(System.Object,System.Windows.Forms.ScrollEventArgs)")]

#endregion

namespace MKY.Windows.Forms.Test
{
	/// <summary>
	/// Test form for <see cref="ListBoxEx"/> and <see cref="FastListBox"/>.
	/// </summary>
	public partial class ListBoxTest : Form
	{
		private List<ListBox> listBoxes;
		private TextFormatFlags formatFlags;

		/// <summary>
		/// Initializes a new instance of the <see cref="ListBoxTest"/> class which
		/// tests variants of <see cref="ListBox"/>.
		/// </summary>
		public ListBoxTest()
		{
			InitializeComponent();

			this.listBoxes = new List<ListBox>(6) // Preset the required capacity to improve memory management.
			{
				listBox_Normal,
				listBox_OwnerDrawFixed,
				listBoxEx_Normal,
				listBoxEx_OwnerDrawFixed,
				fastListBox_Normal,
				fastListBox_OwnerDrawFixed
			};

			this.formatFlags  = TextFormatFlags.Default;
			this.formatFlags |= TextFormatFlags.SingleLine;
			this.formatFlags |= TextFormatFlags.ExpandTabs;
			this.formatFlags |= TextFormatFlags.NoPadding;
			this.formatFlags |= TextFormatFlags.NoPrefix;
		}

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "'counter' does start with a lower case letter.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private int listBox_OwnerDrawFixed_DrawItem_counter;

		private void listBox_OwnerDrawFixed_DrawItem(object sender, DrawItemEventArgs e)
		{
			unchecked
			{
				if (e.Index >= 0)
					DrawAndMeasureAndSetHorizontalExtent(listBox_OwnerDrawFixed, e);

				listBox_OwnerDrawFixed_DrawItem_counter++;
				label_ListBox_OwnerDrawFixed_Count.Text = listBox_OwnerDrawFixed_DrawItem_counter.ToString();
			}
		}

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "'counter' does start with a lower case letter.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private int listBoxEx_OwnerDrawFixed_DrawItem_counter;

		private void listBoxEx_OwnerDrawFixed_DrawItem(object sender, DrawItemEventArgs e)
		{
			unchecked
			{
				if (e.Index >= 0)
					DrawAndMeasureAndSetHorizontalExtent(listBoxEx_OwnerDrawFixed, e);

				listBoxEx_OwnerDrawFixed_DrawItem_counter++;
				label_ListBoxEx_OwnerDrawFixed_Count.Text = listBoxEx_OwnerDrawFixed_DrawItem_counter.ToString();
			}
		}

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "'counter' does start with a lower case letter.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private int fastListBox_OwnerDrawFixed_DrawItem_counter;

		private void fastListBox_OwnerDrawFixed_DrawItem(object sender, DrawItemEventArgs e)
		{
			unchecked
			{
				if (e.Index >= 0)
					DrawAndMeasureAndSetHorizontalExtent(fastListBox_OwnerDrawFixed, e);

				fastListBox_OwnerDrawFixed_DrawItem_counter++;
				label_FastListBox_OwnerDrawFixed_Count.Text = fastListBox_OwnerDrawFixed_DrawItem_counter.ToString();
			}
		}

		private void DrawAndMeasureAndSetHorizontalExtent(ListBox lb, DrawItemEventArgs e)
		{
			e.DrawBackground();

			var text = lb.Items[e.Index] as string;
			TextRenderer.DrawText(e.Graphics, text, e.Font, e.Bounds, e.ForeColor, e.BackColor, this.formatFlags);

			int requestedWidth = TextRenderer.MeasureText(e.Graphics, text, e.Font, e.Bounds.Size, this.formatFlags).Width;
			if ((requestedWidth > 0) && (requestedWidth > lb.HorizontalExtent))
				lb.HorizontalExtent = requestedWidth;

			e.DrawFocusRectangle();
		}

		// \remind MKY 2013-05-25 (related to feature request #163)
		// No feasible way to implement horizontal auto scroll found. There are Win32 API
		// functions to move the position of the scroll bar itself, and to scroll rectangles,
		// but it is not feasible to do the whole translation from .NET Windows.Forms to Win32.
		// Giving up.
	////private void listBoxEx_HorizontalScrolled(object sender, ScrollEventArgs e)
	////{
	////	label_ListBoxEx_HorizontalScrollPositionOld.Text = e.OldValue.ToString(CultureInfo.CurrentCulture);
	////	label_ListBoxEx_HorizontalScrollPositionNew.Text = e.NewValue.ToString(CultureInfo.CurrentCulture);
	////	label_ListBoxEx_HorizontalScrollType.Text = e.Type.ToString();
	////}
	////
	////private void listBoxEx_VerticalScrolled(object sender, ScrollEventArgs e)
	////{
	////	label_ListBoxEx_VerticalScrollPositionOld.Text = e.OldValue.ToString(CultureInfo.CurrentCulture);
	////	label_ListBoxEx_VerticalScrollPositionNew.Text = e.NewValue.ToString(CultureInfo.CurrentCulture);
	////	label_ListBoxEx_VerticalScrollType.Text = e.Type.ToString();
	////}

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "'increment' does start with a lower case letter.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private int button_AddChar_Click_increment;

		private void button_AddChar_Click(object sender, EventArgs e)
		{
			foreach (var lb in this.listBoxes)
			{
				lb.BeginUpdate();

				string append = button_AddChar_Click_increment.ToString(CultureInfo.CurrentCulture);

				button_AddChar_Click_increment++;
				if (button_AddChar_Click_increment >= 10)
					button_AddChar_Click_increment = 0;

				if (lb.Items.Count > 0)
				{
					string item = (lb.Items[lb.Items.Count - 1] as string);
					if (item != null)
					{
						item += append;
						lb.Items[lb.Items.Count - 1] = item;
					}
					else
					{
						lb.Items[lb.Items.Count - 1] = append;
					}
				}
				else
				{
					lb.Items.Add(append);
				}

				// \remind MKY 2013-05-25 (related to feature request #163)
				// No feasible way to implement horizontal auto scroll found. There are Win32 API
				// functions to move the position of the scroll bar itself, and to scroll rectangles,
				// but it is not feasible to do the whole translation from .NET Windows.Forms to Win32.
				// Giving up.
			////lb.HorizontalScrollToEnd();

				lb.EndUpdate();
			}
		}

		private void button_AddSomeChars_Click(object sender, EventArgs e)
		{
			foreach (var lb in this.listBoxes)
			{
				lb.BeginUpdate();

				if (lb.Items.Count > 0)
				{
					string item = (lb.Items[lb.Items.Count - 1] as string);
					if (item != null)
					{
						item += "0123456789";
						lb.Items[lb.Items.Count - 1] = item;
					}
					else
					{
						lb.Items[lb.Items.Count - 1] = "0123456789";
					}
				}
				else
				{
					lb.Items.Add("0123456789");
				}

				// \remind MKY 2013-05-25 (related to feature request #163)
				// No feasible way to implement horizontal auto scroll found. There are Win32 API
				// functions to move the position of the scroll bar itself, and to scroll rectangles,
				// but it is not feasible to do the whole translation from .NET Windows.Forms to Win32.
				// Giving up.
			////lb.HorizontalScrollToEnd();

				lb.EndUpdate();
			}
		}

		private void button_AddLine_Click(object sender, EventArgs e)
		{
			foreach (var lb in this.listBoxes)
			{
				lb.BeginUpdate();

				lb.Items.Add("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");

				var lbex = lb as ListBoxEx;
				if (lbex != null)
					lbex.VerticalScrollToBottom();

				lb.EndUpdate();
			}
		}

		private void button_AddSomeLines_Click(object sender, EventArgs e)
		{
			foreach (var lb in this.listBoxes)
			{
				lb.BeginUpdate();

				for (int i = 0; i < 10; i++)
					lb.Items.Add("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");

				var lbex = lb as ListBoxEx;
				if (lbex != null)
					lbex.VerticalScrollToBottom();

				lb.EndUpdate();
			}
		}

		private void button_AddManyLines_Click(object sender, EventArgs e)
		{
			foreach (var lb in this.listBoxes)
			{
				lb.BeginUpdate();

				for (int i = 0; i < 1000; i++)
					lb.Items.Add("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");

				var lbex = lb as ListBoxEx;
				if (lbex != null)
					lbex.VerticalScrollToBottom();

				lb.EndUpdate();
			}
		}

		private void button_RemoveLine_Click(object sender, EventArgs e)
		{
			foreach (var lb in this.listBoxes)
			{
				lb.BeginUpdate();

				if (lb.Items.Count > 0)
					lb.Items.RemoveAt(0);

				var lbex = lb as ListBoxEx;
				if (lbex != null)
					lbex.VerticalScrollToBottom();

				lb.EndUpdate();
			}
		}

		private void button_RemoveManyLines_Click(object sender, EventArgs e)
		{
			foreach (var lb in this.listBoxes)
			{
				lb.BeginUpdate();

				for (int i = 0; i < 50; i++)
				{
					if (lb.Items.Count > 0)
						lb.Items.RemoveAt(0);
				}

				var lbex = lb as ListBoxEx;
				if (lbex != null)
					lbex.VerticalScrollToBottom();

				lb.EndUpdate();
			}
		}

		private void button_RemoveAllLines_Click(object sender, EventArgs e)
		{
			foreach (var lb in this.listBoxes)
			{
				lb.BeginUpdate();

				lb.Items.Clear();

				lb.EndUpdate();
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
