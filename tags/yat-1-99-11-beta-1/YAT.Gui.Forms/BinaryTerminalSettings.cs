using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace HSR.YAT.Gui.Forms
{
	public partial class BinaryTerminalSettings : System.Windows.Forms.Form
	{
		//------------------------------------------------------------------------------------------
		// Attributes
		//------------------------------------------------------------------------------------------

		private bool _isStartingUp = true;
		private bool _isSettingControls = false;

		private Domain.Settings.BinaryTerminalSettings _settings;
		private Domain.Settings.BinaryTerminalSettings _settings_Form;

		//------------------------------------------------------------------------------------------
		// Constructor
		//------------------------------------------------------------------------------------------

		public BinaryTerminalSettings(Domain.Settings.BinaryTerminalSettings settings)
		{
			InitializeComponent();

			_settings = settings;
			_settings_Form = new Domain.Settings.BinaryTerminalSettings(settings);
			_settings_Form.Changed += new EventHandler<Utilities.Settings.SettingsEventArgs>(_settings_Form_Changed);
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		public Domain.Settings.BinaryTerminalSettings SettingsResult
		{
			get { return (_settings); }
		}

		#endregion

		#region Settings Event Handlers
		//------------------------------------------------------------------------------------------
		// Settings Event Handlers
		//------------------------------------------------------------------------------------------

		private void _settings_Form_Changed(object sender, Utilities.Settings.SettingsEventArgs e)
		{
			SetControls();
		}

		#endregion

		#region Form Event Handlers
		//------------------------------------------------------------------------------------------
		// Form Event Handlers
		//------------------------------------------------------------------------------------------

		private void BinaryTerminalSettings_Paint(object sender, PaintEventArgs e)
		{
			if (_isStartingUp)
			{
				_isStartingUp = false;

				// initially set controls and validate its contents where needed
				SetControls();
			}
		}

		#endregion

		#region Controls Event Handlers
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers
		//------------------------------------------------------------------------------------------

		private void checkBox_DirectionLineBreak_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				_settings_Form.DirectionLineBreakEnabled = checkBox_DirectionLineBreak.Checked;
				SetControls();
			}
		}

		private void binaryTerminalSettingsSet_Tx_SettingsChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				_settings_Form.TxDisplay = binaryTerminalSettingsSet_Tx.Settings;
				SetControls();
			}
		}

		private void checkBox_SeparateTxRxDisplay_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				_settings_Form.SeparateTxRxDisplay = checkBox_SeparateTxRxDisplay.Checked;
				SetControls();
			}
		}

		private void binaryTerminalSettingsSet_Rx_SettingsChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				_settings_Form.RxDisplay = binaryTerminalSettingsSet_Rx.Settings;
				SetControls();
			}
		}

		private void button_OK_Click(object sender, EventArgs e)
		{
			_settings_Form.Changed -= new EventHandler<Utilities.Settings.SettingsEventArgs>(_settings_Form_Changed);
			_settings = _settings_Form;
		}

		private void button_Cancel_Click(object sender, EventArgs e)
		{
			// do nothing
		}

		private void button_Defaults_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show
				(
				this,
				"Reset settings to default values?",
				"Defaults?",
				MessageBoxButtons.YesNoCancel,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button2
				)
				== DialogResult.Yes)
			{
				_settings.SetDefaults();
				SetControls();
			}
		}

		#endregion

		#region Private Methods
		//------------------------------------------------------------------------------------------
		// Private Methods
		//------------------------------------------------------------------------------------------

		private void SetControls()
		{
			_isSettingControls = true;

			checkBox_DirectionLineBreak.Checked = _settings_Form.DirectionLineBreakEnabled;

			if (_settings_Form.SeparateTxRxDisplay)
				groupBox_TxDisplay.Text = "&Tx and Rx";
			else
				groupBox_TxDisplay.Text = "&Tx";
			binaryTerminalSettingsSet_Tx.Settings = _settings_Form.TxDisplay;

			checkBox_SeparateTxRxDisplay.Checked = _settings_Form.SeparateTxRxDisplay;
			groupBox_RxDisplay.Enabled = _settings_Form.SeparateTxRxDisplay;
			binaryTerminalSettingsSet_Rx.Settings = _settings_Form.RxDisplay;

			_isSettingControls = false;
		}

		#endregion
	}
}