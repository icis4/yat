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
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

using MKY.ComponentModel;

#endregion

namespace MKY.Windows.Forms
{
	/// <summary>
	/// Provides a list box including edit buttons to edit a list of strings.
	/// </summary>
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
		public new event EventHandler<StringCancelEventArgs> Validating;

		/// <summary></summary>
		public event EventHandler ListChanged;

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
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Performance is not an issue here, simplicity and ease of use is...")]
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

				if (value != null) // Empty array is OK, but 'null' results in exception.
					listBox_StringList.Items.AddRange(value);

				SetControls();
				OnStringListChanged(EventArgs.Empty);
			}
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void listBox_StringList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			SetControls();
		}

		[ModalBehaviorContract(ModalBehavior.Always)]
		private void listBox_StringList_DoubleClick(object sender, EventArgs e)
		{
			string item = (listBox_StringList.SelectedItem as string);
			if (item != null)
			{
				if (TextInputBox.Show
					(
						this,
						"Item text:",
						"Edit Item",
						item,
						TextInputBox_Validating,
						out item
					)
					== DialogResult.OK)
				{
					listBox_StringList.SelectedItem = item;
					OnStringListChanged(EventArgs.Empty);
				}
			}
		}

		private void TextInputBox_Validating(object sender, StringCancelEventArgs e)
		{
			OnValidating(e);
		}

		[ModalBehaviorContract(ModalBehavior.Always)]
		private void button_Add_Click(object sender, EventArgs e)
		{
			string item;
			if (TextInputBox.Show
				(
					this,
					"Item text:",
					"Add Item",
					"",
					out item
				)
				== DialogResult.OK)
			{
				listBox_StringList.Items.Add(item);
				SetControls();
				OnStringListChanged(EventArgs.Empty);
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
			OnStringListChanged(EventArgs.Empty);
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
				try
				{
					listBox_StringList.Items.RemoveAt(oldIndex);
					listBox_StringList.Items.Insert(newIndex, item);
					listBox_StringList.SelectedIndex = newIndex;
				}
				finally
				{
					this.isSettingControls.Leave();
				}

				SetControls();
				OnStringListChanged(EventArgs.Empty);
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
				try
				{
					listBox_StringList.Items.RemoveAt(oldIndex);
					listBox_StringList.Items.Insert(newIndex, item);
					listBox_StringList.SelectedIndex = newIndex;
				}
				finally
				{
					this.isSettingControls.Leave();
				}

				SetControls();
				OnStringListChanged(EventArgs.Empty);
			}
		}

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		private void SetControls()
		{
			this.isSettingControls.Enter();
			try
			{
				int itemCount = listBox_StringList.Items.Count;
				int selectedItemsCount = listBox_StringList.SelectedIndices.Count;
				int selectedItemIndex  = listBox_StringList.SelectedIndex;

				button_Delete.Enabled   = (selectedItemsCount > 0);
				button_MoveUp.Enabled   = (selectedItemsCount == 1) && (selectedItemIndex > 0);
				button_MoveDown.Enabled = (selectedItemsCount == 1) && (selectedItemIndex < (itemCount - 1));
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		#endregion

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnValidating(StringCancelEventArgs e)
		{
			EventHelper.RaiseSync(Validating, this, e);
		}

		/// <summary></summary>
		protected virtual void OnStringListChanged(EventArgs e)
		{
			EventHelper.RaiseSync(ListChanged, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
