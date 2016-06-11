//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace MKY.Windows.Forms
{
	/// <summary></summary>
	public static class SelectionHelper
	{
		/// <summary>
		/// Selects the given item in a <see cref="ListControl"/>.
		/// </summary>
		/// <remarks>
		/// Separate <paramref name="itemText"/> need to selectively chose the way the item is
		/// converted into a string. This can e.g. be an implicit string conversion operator,
		/// or the item's ToString() method, or something else.
		/// </remarks>
		public static void Select(ComboBox control, object item, string itemText)
		{
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
						control.SelectedIndex = ControlEx.InvalidIndex;
						control.Text = itemText;
					}
				}
				else
				{	// Item doesn't exist, use default = first item in the combo box, or none if list is empty.
					control.SelectedIndex = 0;
				}
			}
			else
			{
				control.SelectedIndex = ControlEx.InvalidIndex;
				control.Text = itemText;
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Same type as Select() above.")]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behaviour.")]
		public static void Deselect(ComboBox control, string itemText = null)
		{
			if (string.IsNullOrEmpty(itemText))
			{
				control.Text = null; // Setting the 'Text' property to null or an empty string ("") sets the SelectedIndex to -1.
			}
			else
			{
				control.SelectedIndex = ControlEx.InvalidIndex; // -1.
				control.Text = itemText;
			}
		}

		/// <summary>
		/// Selects the given item in a <see cref="ToolStripComboBox"/>.
		/// </summary>
		/// <remarks>
		/// Separate <paramref name="itemText"/> need to selectively chose the way the item is
		/// converted into a string. This can e.g. be an implicit string conversion operator,
		/// or the item's ToString() method, or something else.
		/// </remarks>
		public static void Select(ToolStripComboBox control, object item, string itemText)
		{
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
						control.SelectedIndex = ControlEx.InvalidIndex;
						control.Text = itemText;
					}
				}
				else
				{	// Item doesn't exist, use default = first item in the combo box, or none if list is empty.
					control.SelectedIndex = 0;
				}
			}
			else
			{
				control.SelectedIndex = ControlEx.InvalidIndex;
				control.Text = itemText;
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behaviour.")]
		public static void Deselect(ToolStripComboBox control, string itemText = null)
		{
			if (string.IsNullOrEmpty(itemText))
			{
				control.Text = null; // Setting the 'Text' property to null or an empty string ("") sets the SelectedIndex to -1.
			}
			else
			{
				control.SelectedIndex = ControlEx.InvalidIndex; // -1.
				control.Text = itemText;
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
