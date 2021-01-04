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
// YAT Version 2.2.0 Development
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
using System.Windows.Forms;

using MKY.Windows.Forms;

#endregion

namespace YAT.View.Forms
{
	/// <summary></summary>
	public partial class BinaryTerminalSettings : Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;

		private Domain.Settings.BinaryTerminalSettings settings;
		private Domain.Settings.BinaryTerminalSettings settingsInEdit;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public BinaryTerminalSettings(Domain.Settings.BinaryTerminalSettings settings)
		{
			InitializeComponent();

			KeepAndCloneAndAttachSettings(settings);

			// SetControls() is initially called in the 'Shown' event handler.
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public Domain.Settings.BinaryTerminalSettings SettingsResult
		{
			get { return (this.settings); }
		}

		#endregion

		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

		private void KeepAndCloneAndAttachSettings(Domain.Settings.BinaryTerminalSettings settings)
		{
			this.settings = settings;
			this.settingsInEdit = new Domain.Settings.BinaryTerminalSettings(settings); // Clone to ensure decoupling.
			this.settingsInEdit.Changed += settingsInEdit_Changed;
		}

		private void DetachAndAcceptSettings()
		{
			this.settingsInEdit.Changed -= settingsInEdit_Changed;
			this.settings = this.settingsInEdit;
		}

		private void settingsInEdit_Changed(object sender, MKY.Settings.SettingsEventArgs e)
		{
			SetControls();
		}

		#endregion

		#region Form Event Handlers
		//==========================================================================================
		// Form Event Handlers
		//==========================================================================================

		/// <summary>
		/// Initially set controls and validate its contents where needed.
		/// </summary>
		/// <remarks>
		/// The 'Shown' event is only raised the first time a form is displayed; subsequently
		/// minimizing, maximizing, restoring, hiding, showing, or invalidating and repainting will
		/// not raise this event again.
		/// Note that the 'Shown' event is raised after the 'Load' event and will also be raised if
		/// the application is started minimized. Also note that operations called in the 'Shown'
		/// event can depend on a properly drawn form, as the 'Paint' event of this form and its
		/// child controls has been raised before this 'Shown' event.
		/// </remarks>
		private void BinaryTerminalSettings_Shown(object sender, EventArgs e)
		{
			SetControls();
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void binaryTerminalSettingsSet_Tx_SettingsChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.TxDisplay = binaryTerminalSettingsSet_Tx.Settings;
		}

		private void checkBox_SeparateTxRxDisplay_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.SeparateTxRxDisplay = checkBox_SeparateTxRxDisplay.Checked;
		}

		private void binaryTerminalSettingsSet_Rx_SettingsChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.RxDisplay = binaryTerminalSettingsSet_Rx.Settings;
		}

		private void button_OK_Click(object sender, EventArgs e)
		{
			DetachAndAcceptSettings();
		}

		private void button_Cancel_Click(object sender, EventArgs e)
		{
			// Do nothing.
		}

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void button_Defaults_Click(object sender, EventArgs e)
		{
			if (MessageBoxEx.Show
				(
				this,
				"Reset all settings to default values?",
				"Defaults?",
				MessageBoxButtons.YesNoCancel,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button3
				)
				== DialogResult.Yes)
			{
				this.settingsInEdit.SetDefaults();
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
				if (this.settingsInEdit.SeparateTxRxDisplay)
					groupBox_TxDisplay.Text = "&Tx";
				else
					groupBox_TxDisplay.Text = "&Tx and Rx";

				binaryTerminalSettingsSet_Tx.Settings = this.settingsInEdit.TxDisplay;

				checkBox_SeparateTxRxDisplay.Checked  = this.settingsInEdit.SeparateTxRxDisplay;
				groupBox_RxDisplay.Enabled            = this.settingsInEdit.SeparateTxRxDisplay;
				binaryTerminalSettingsSet_Rx.Settings = this.settingsInEdit.RxDisplay;
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
