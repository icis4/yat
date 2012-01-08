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
// YAT 2.0 Beta 4 Candidate 2 Development Version 1.99.29
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
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

namespace YAT.Gui.Forms
{
	/// <summary></summary>
	public partial class BinaryTerminalSettings : System.Windows.Forms.Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;

		private Domain.Settings.BinaryTerminalSettings settings;
		private Domain.Settings.BinaryTerminalSettings settings_Form;

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

			// SetControls() is initially called in the 'Paint' event handler.
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
			this.settings_Form = new Domain.Settings.BinaryTerminalSettings(settings);
			this.settings_Form.Changed += new EventHandler<MKY.Settings.SettingsEventArgs>(settings_Form_Changed);
		}

		private void DetachAndAcceptSettings()
		{
			this.settings_Form.Changed -= new EventHandler<MKY.Settings.SettingsEventArgs>(settings_Form_Changed);
			this.settings = this.settings_Form;
		}

		private void settings_Form_Changed(object sender, MKY.Settings.SettingsEventArgs e)
		{
			SetControls();
		}

		#endregion

		#region Form Event Handlers
		//==========================================================================================
		// Form Event Handlers
		//==========================================================================================

		/// <summary>
		/// Startup flag only used in the following event handler.
		/// </summary>
		private bool isStartingUp = true;

		/// <summary>
		/// Initially set controls and validate its contents where needed.
		/// </summary>
		private void BinaryTerminalSettings_Paint(object sender, PaintEventArgs e)
		{
			if (this.isStartingUp)
			{
				this.isStartingUp = false;
				SetControls();
			}
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void binaryTerminalSettingsSet_Tx_SettingsChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.TxDisplay = binaryTerminalSettingsSet_Tx.Settings;
		}

		private void checkBox_SeparateTxRxDisplay_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.SeparateTxRxDisplay = checkBox_SeparateTxRxDisplay.Checked;
		}

		private void binaryTerminalSettingsSet_Rx_SettingsChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.RxDisplay = binaryTerminalSettingsSet_Rx.Settings;
		}

		private void button_OK_Click(object sender, EventArgs e)
		{
			DetachAndAcceptSettings();
		}

		private void button_Cancel_Click(object sender, EventArgs e)
		{
			// Do nothing.
		}

		private void button_Defaults_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show
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
				this.settings_Form.SetDefaults();
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

			if (this.settings_Form.SeparateTxRxDisplay)
				groupBox_TxDisplay.Text = "&Tx and Rx";
			else
				groupBox_TxDisplay.Text = "&Tx";
			binaryTerminalSettingsSet_Tx.Settings = this.settings_Form.TxDisplay;

			checkBox_SeparateTxRxDisplay.Checked = this.settings_Form.SeparateTxRxDisplay;
			groupBox_RxDisplay.Enabled = this.settings_Form.SeparateTxRxDisplay;
			binaryTerminalSettingsSet_Rx.Settings = this.settings_Form.RxDisplay;

			this.isSettingControls.Leave();
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
