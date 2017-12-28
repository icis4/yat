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
// MKY Version 1.0.22
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

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace MKY.Windows.Forms
{
	/// <summary></summary>
	public static class ComboBoxHelper
	{
		/// <summary>
		/// Selects the given item in a <see cref="ComboBox"/>.
		/// </summary>
		/// <remarks>
		/// If <paramref name="item"/> is not contained, <see cref="ComboBox.SelectedIndex"/>
		/// is set to <see cref="ControlEx.InvalidIndex"/>. Optionally, the text is set to the
		/// given <paramref name="fallbackText"/>.
		/// </remarks>
		/// <remarks>
		/// Separate <paramref name="fallbackText"/> need to selectively chose the way the item is
		/// converted into a string. This can e.g. be an implicit string conversion operator,
		/// or the item's ToString() method, or something else.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behavior.")]
		public static void Select(ComboBox control, object item, string fallbackText = null)
		{
			int selectionStart  = 0;
			int selectionLength = 0;

			if (control.DropDownStyle != ComboBoxStyle.DropDownList)
			{
				// Keep cursor position and text selection:
				selectionStart  = control.SelectionStart;
				selectionLength = control.SelectionLength;
			}

			if (control.Items.Count > 0)
			{
				if (item != null)
				{
					if (control.Items.Contains(item))
					{	// Applies if an item of the combo box is selected.
						control.SelectedItem = item;
					}
					else
					{	// Applies if an item that is not in the combo box is selected.
						control.SelectedIndex = ControlEx.InvalidIndex; // Explicitly set the SelectedIndex to -1.
						control.Text = fallbackText;
					}
				}
				else
				{	// Item doesn't exist, use default = first item in the combo box.
					control.SelectedIndex = 0;
				}
			}
			else
			{
				control.SelectedIndex = ControlEx.InvalidIndex; // Explicitly set the SelectedIndex to -1.
				control.Text = fallbackText;
			}

			if (control.DropDownStyle != ComboBoxStyle.DropDownList)
			{
				// Restore cursor position and text selection (as possible):
				control.SelectionStart  = selectionStart;
				control.SelectionLength = selectionLength;
			}
		}

		/// <remarks>
		/// <see cref="ComboBox.SelectedIndex"/> is set to <see cref="ControlEx.InvalidIndex"/>.
		/// </remarks>
		/// <remarks>
		/// Provided for symmetricity with <see cref="Select(ComboBox, object, string)"/> above.
		/// </remarks>
		public static void Deselect(ComboBox control)
		{
			control.SelectedIndex = ControlEx.InvalidIndex;
			control.Text = "";
		}

		/// <summary>
		/// Updates the text of the <see cref="ComboBox"/> while staying in edit,
		/// i.e. cursor location and text selection is kept.
		/// </summary>
		public static void UpdateTextWhileInEdit(ComboBox control, string text)
		{
			int selectionStart  = 0;
			int selectionLength = 0;

			if (control.DropDownStyle != ComboBoxStyle.DropDownList)
			{
				// Keep cursor position and text selection:
				selectionStart  = control.SelectionStart;
				selectionLength = control.SelectionLength;
			}

			control.Text = text;

			if (control.DropDownStyle != ComboBoxStyle.DropDownList)
			{
				// Restore cursor position and text selection (as possible):
				control.SelectionStart  = selectionStart;
				control.SelectionLength = selectionLength;
			}
		}

		/// <summary>
		/// Updates the items of the <see cref="ComboBox"/> while staying in edit,
		/// i.e. cursor location and text selection is kept.
		/// </summary>
		public static void UpdateItemsWhileInEdit(ComboBox control, object[] items)
		{
			// Keep text field because Items.Clear() will reset this:
			string text         = control.Text;
			int selectionStart  = control.SelectionStart;
			int selectionLength = control.SelectionLength;

			control.Items.Clear();

			if (items != null) // Empty array is OK, but 'null' results in exception.
				control.Items.AddRange(items);

			// Restore text field:
			control.Text            = text;
			control.SelectionStart  = selectionStart;
			control.SelectionLength = selectionLength;
		}

		/// <summary>
		/// Clears the items of the <see cref="ComboBox"/> while staying in edit,
		/// i.e. cursor location and text selection is kept.
		/// </summary>
		public static void ClearItemsWhileInEdit(ComboBox control)
		{
			UpdateItemsWhileInEdit(control, null);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
