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

using System.Windows.Forms;

namespace MKY.Windows.Forms
{
	/// <summary></summary>
	public static class ToolStripComboBoxHelper
	{
		/// <summary>
		/// Selects the given item in a <see cref="ToolStripComboBox"/>.
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
		public static void Select(ToolStripComboBox control, object item, string fallbackText = null)
		{
			ComboBoxHelper.Select(control.ComboBox, item, fallbackText);
		}

		/// <remarks>
		/// <see cref="ToolStripComboBox.SelectedIndex"/> is set to <see cref="ControlEx.InvalidIndex"/>.
		/// </remarks>
		/// <remarks>
		/// Provided for symmetricity with <see cref="Select(ToolStripComboBox, object, string)"/> above.
		/// </remarks>
		public static void Deselect(ToolStripComboBox control)
		{
			ComboBoxHelper.Deselect(control.ComboBox);
		}

		/// <summary>
		/// Updates the text of the <see cref="ToolStripComboBox"/> while staying in edit,
		/// i.e. cursor location and text selection is kept.
		/// </summary>
		public static void UpdateTextWhileInEdit(ToolStripComboBox control, string text)
		{
			ComboBoxHelper.UpdateTextWhileInEdit(control.ComboBox, text);
		}

		/// <summary>
		/// Updates the items of the <see cref="ToolStripComboBox"/> while staying in edit,
		/// i.e. cursor location and text selection is kept.
		/// </summary>
		public static void UpdateItemsWhileInEdit(ToolStripComboBox control, object[] items)
		{
			ComboBoxHelper.UpdateItemsWhileInEdit(control.ComboBox, items);
		}

		/// <summary>
		/// Clears the items of the <see cref="ToolStripComboBox"/> while staying in edit,
		/// i.e. cursor location and text selection is kept.
		/// </summary>
		public static void ClearItemsWhileInEdit(ToolStripComboBox control)
		{
			ComboBoxHelper.ClearItemsWhileInEdit(control.ComboBox);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
