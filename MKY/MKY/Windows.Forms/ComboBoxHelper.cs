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

using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace MKY.Windows.Forms
{
	/// <summary></summary>
	public static class ComboBoxHelper
	{
		#region CursorAndSelection
		//==========================================================================================
		// CursorAndSelection
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Emphasize that this type belongs to the 'ComboBoxHelper'.")]
		public class CursorAndSelection
		{
			/// <summary>
			/// Gets or sets whether the previous cursor position is at the end of the editable portion of the combo box.
			/// </summary>
			public bool PreviousCursorIsAtEnd { get; protected set; } // = false;

			/// <summary>
			/// Gets or sets the previous starting index of text selected in the combo box.
			/// </summary>
			public int  PreviousSelectionStart { get; protected set; } = ControlEx.InvalidIndex;

			/// <summary>
			/// Gets or sets the previous number of characters selected in the editable portion of the combo box.
			/// </summary>
			public int  PreviousSelectionLength { get; protected set; } // = 0;

			/// <summary>
			/// Gets or sets whether the previous selection spans to the end of the editable portion of the combo box.
			/// </summary>
			public bool PreviousSelectionSpansEnd { get; protected set; } // = false;

			/// <summary>
			/// Remembers the current cursor position and text selection of the specified control.
			/// </summary>
			public void Remember(ComboBox control)
			{
				PreviousCursorIsAtEnd = (control.SelectionStart == control.Text.Length);

				PreviousSelectionStart  = control.SelectionStart;
				PreviousSelectionLength = control.SelectionLength;

				if (control.SelectionLength == 0)
					PreviousSelectionSpansEnd = (control.SelectionLength == (control.Text.Length - control.SelectionStart));
				else
					PreviousSelectionSpansEnd = false;
			}

			/// <summary>
			/// Restores the previously remembered cursor position and text selection on the specified control.
			/// </summary>
			public void Restore(ComboBox control)
			{
				if (PreviousSelectionStart != ControlEx.InvalidIndex)
				{
					if (PreviousSelectionLength == 0)
					{
						if (!PreviousCursorIsAtEnd) // Simply restore selection:
						{
							control.SelectionStart  = PreviousSelectionStart;
							control.SelectionLength = 0;
						}
						else // PreviousCursorIsAtEnd => Re-calculate position:
						{
							control.SelectionStart  = control.Text.Length;
							control.SelectionLength = 0;
						}
					}
					else
					{
						if (!PreviousSelectionSpansEnd) // Simply restore selection:
						{
							control.SelectionStart  = PreviousSelectionStart;
							control.SelectionLength = PreviousSelectionLength;
						}
						else // PreviousSelectionSpansEnd => Re-calculate selection:
						{
							control.SelectionStart  = PreviousSelectionStart;
							control.SelectionLength = (control.Text.Length - control.SelectionStart);
						}

						// \remind (2018-01-02 / MKY) minor issue (bug #403):
						// If selection was done in reverse direction, i.e. cursor is located to the
						// left of the selection, e.g. Iabc, Restore() again reverts this, e.g. abcI.
					}
				}
			}
		}

		#endregion

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
		/// <remarks>
		/// Attention, <see cref="ComboBox"/> objects have a limitation regarding case sensitivity:
		/// If <see cref="ComboBox.Text"/> is e.g. set to "aa" while <see cref="ComboBox.Items"/>
		/// contain "AA", that item is wrongly selected.
		///
		/// Issue is documented as YAT bug #347. Issue shall again check after upgrade to .NET 4+.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static void Select(ComboBox control, object item, string fallbackText = null)
		{
			var cursorAndSelection = new CursorAndSelection();

			if (control.DropDownStyle != ComboBoxStyle.DropDownList)
			{
				cursorAndSelection.Remember(control);
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

						// Note taken from MSDN:
						// "Setting the Text property to 'null' or an empty string ("") sets the SelectedIndex to -1."
					}
				}
				else
				{
					if (!string.IsNullOrEmpty(fallbackText))
					{	// Applies if explicitly setting the fallback text.
						control.SelectedIndex = ControlEx.InvalidIndex; // Explicitly set the SelectedIndex to -1.
						control.Text = fallbackText;
					}
					else
					{	// Neither item nor fallback is given, use default = first item in the combo box.
						control.SelectedIndex = 0;
					}
				}
			}
			else
			{
				control.SelectedIndex = ControlEx.InvalidIndex; // Explicitly set the SelectedIndex to -1.
				control.Text = fallbackText;

				// Note taken from MSDN:
				// "Setting the Text property to 'null' or an empty string ("") sets the SelectedIndex to -1."
			}

			if (control.DropDownStyle != ComboBoxStyle.DropDownList)
			{
				cursorAndSelection.Restore(control);
			}
		}

		/// <remarks>
		/// <see cref="ComboBox.SelectedIndex"/> is set to <see cref="ControlEx.InvalidIndex"/>.
		/// </remarks>
		/// <remarks>
		/// Provided for symmetricity with <see cref="Select(ComboBox, object, string)"/> above.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Symmetricity with Select() above.")]
		public static void Deselect(ComboBox control)
		{
			control.SelectedIndex = ControlEx.InvalidIndex; // Explicitly set the SelectedIndex to -1.
			control.Text = "";                              // Wouldn't be necessary, see note below.

			// Note taken from MSDN:
			// "Setting the Text property to 'null' or an empty string ("") sets the SelectedIndex to -1."
		}

		/// <summary>
		/// Updates the text of the <see cref="ComboBox"/> while staying in edit,
		/// i.e. cursor location and text selection is kept.
		/// </summary>
		/// <remarks>
		/// Attention, <see cref="ComboBox"/> objects have a limitation regarding case sensitivity:
		/// If <see cref="ComboBox.Text"/> is e.g. set to "abc" while <see cref="ComboBox.Items"/>
		/// contain "ABC", that item is wrongly selected.
		///
		/// Issue is documented as YAT bug #347. Issue shall again check after upgrade to .NET 4+.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Come on, 'abc'...")]
		public static void UpdateTextKeepingCursorAndSelection(ComboBox control, string text)
		{
			var cursorAndSelection = new CursorAndSelection();

			if (control.DropDownStyle != ComboBoxStyle.DropDownList)
				cursorAndSelection.Remember(control);

			control.Text = text;

			if (control.DropDownStyle != ComboBoxStyle.DropDownList)
				cursorAndSelection.Restore(control);
		}

		/// <summary>
		/// Updates the items of the <see cref="ComboBox"/> while staying in edit,
		/// i.e. cursor location and text selection is kept.
		/// </summary>
		/// <remarks>
		/// Attention, <see cref="ComboBox"/> objects have a limitation regarding case sensitivity:
		/// If <see cref="ComboBox.Text"/> is e.g. set to "abc" while <see cref="ComboBox.Items"/>
		/// contain "ABC", that item is wrongly selected.
		///
		/// Issue is documented as YAT bug #347. Issue shall again check after upgrade to .NET 4+.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Come on, 'abc'...")]
		public static void UpdateItemsKeepingCursorAndSelection(ComboBox control, object[] items)
		{
			var cursorAndSelection = new CursorAndSelection();

			if (control.DropDownStyle != ComboBoxStyle.DropDownList)
				cursorAndSelection.Remember(control);

			// Also remember text field because Items.Clear() will reset this:
			string text = control.Text;

			control.Items.Clear();

			if (items != null) // Empty array is OK, but 'null' results in exception.
				control.Items.AddRange(items);

			// First restore text field:
			control.Text = text;

			if (control.DropDownStyle != ComboBoxStyle.DropDownList)
				cursorAndSelection.Restore(control);
		}

		/// <summary>
		/// Clears the items of the <see cref="ComboBox"/> while staying in edit,
		/// i.e. cursor location and text selection is kept.
		/// </summary>
		public static void ClearItemsKeepingCursorAndSelection(ComboBox control)
		{
			UpdateItemsKeepingCursorAndSelection(control, null);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
