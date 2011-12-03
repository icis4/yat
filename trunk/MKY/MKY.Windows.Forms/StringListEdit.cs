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
// MKY Version 1.0.7
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
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
using System.ComponentModel;
using System.Windows.Forms;

using MKY.Event;

#endregion

namespace MKY.Windows.Forms
{
	/// <summary>
	/// Provides a list box including edit buttons to eidt a list of strings.
	/// </summary>
	[DesignerCategory("Windows Forms")]
	[DefaultEvent("StringListChanged")]
	public partial class StringListEdit : UserControl
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		public event EventHandler StringListChanged;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public StringListEdit()
		{
			InitializeComponent();
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public string[] StringList
		{
			get
			{
				object[] a = new object[listBox_StringList.Items.Count];
				listBox_StringList.Items.CopyTo(a, 0);
				return (Array.ConvertAll<object, string>(a, new Converter<object, string>(Convert.ToString)));
			}
			set
			{
				listBox_StringList.Items.Clear();
				listBox_StringList.Items.AddRange(value);
				SetControls();
				OnStringListChanged(new EventArgs());
			}
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void listBox_StringList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				SetControls();
		}

		private void listBox_StringList_DoubleClick(object sender, EventArgs e)
		{
			string item = (listBox_StringList.SelectedItem as string);
			if (item != null)
			{
				if (TextInputBox.Show
					(
					this,
					"Edit item:",
					"Edit Item",
					item,
					out item
					)
					== DialogResult.OK)
				{
					listBox_StringList.SelectedItem = item;
					OnStringListChanged(new EventArgs());
				}
			}
		}

		private void button_Add_Click(object sender, EventArgs e)
		{
			string item;
			if (TextInputBox.Show
				(
				this,
				"Enter the items text:",
				"Add Item",
				"",
				out item
				)
				== DialogResult.OK)
			{
				listBox_StringList.Items.Add(item);
				SetControls();
				OnStringListChanged(new EventArgs());
			}
		}

		private void button_Delete_Click(object sender, EventArgs e)
		{
			int count = listBox_StringList.SelectedIndices.Count;
			int[] selectedIndices = new int[count];
			listBox_StringList.SelectedIndices.CopyTo(selectedIndices, 0);

			// Remove items reverse to ensure that indices stay valid throughout the operation.
			for (int i = (count - 1); i >= 0; i--)
				listBox_StringList.Items.RemoveAt(selectedIndices[i]);

			SetControls();
			OnStringListChanged(new EventArgs());
		}

		private void button_MoveUp_Click(object sender, EventArgs e)
		{
			// Preconditions:
			// > Button is only enabled if a single item is selected.
			// > Button is only enabled if selected item is not the first item.
			int oldIndex = listBox_StringList.SelectedIndex;
			int newIndex = oldIndex - 1;
			string item = (listBox_StringList.SelectedItem as string);
			if ((oldIndex != ControlEx.InvalidIndex) && (item != null))
			{
				this.isSettingControls.Enter();
				listBox_StringList.Items.RemoveAt(oldIndex);
				listBox_StringList.Items.Insert(newIndex, item);
				listBox_StringList.SelectedIndex = newIndex;
				this.isSettingControls.Leave();

				SetControls();
				OnStringListChanged(new EventArgs());
			}
		}

		private void button_MoveDown_Click(object sender, EventArgs e)
		{
			// Preconditions:
			// > Button is only enabled if a single item is selected.
			// > Button is only enabled if selected item is not the last item.
			int oldIndex = listBox_StringList.SelectedIndex;
			int newIndex = oldIndex + 1;
			string item = (listBox_StringList.SelectedItem as string);
			if ((oldIndex != ControlEx.InvalidIndex) && (item != null))
			{
				this.isSettingControls.Enter();
				listBox_StringList.Items.RemoveAt(oldIndex);
				listBox_StringList.Items.Insert(newIndex, item);
				listBox_StringList.SelectedIndex = newIndex;
				this.isSettingControls.Leave();

				SetControls();
				OnStringListChanged(new EventArgs());
			}
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void SetControls()
		{
			this.isSettingControls.Enter();

			int itemCount = listBox_StringList.Items.Count;
			int selectedItemsCount = listBox_StringList.SelectedIndices.Count;
			int selectedItemIndex  = listBox_StringList.SelectedIndex;

			button_Delete.Enabled   = (selectedItemsCount > 0);
			button_MoveUp.Enabled   = (selectedItemsCount == 1) && (selectedItemIndex > 0);
			button_MoveDown.Enabled = (selectedItemsCount == 1) && (selectedItemIndex < (itemCount - 1));

			this.isSettingControls.Leave();
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnStringListChanged(EventArgs e)
		{
			EventHelper.FireSync(StringListChanged, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
