using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace YAT.Gui.Forms
{
	public partial class Preferences : Form
	{
		//------------------------------------------------------------------------------------------
		// Fields
		//------------------------------------------------------------------------------------------

		private bool _isStartingUp = true;
		private bool _isSettingControls = false;

		private YAT.Settings.GeneralSettings _settings;
		private YAT.Settings.GeneralSettings _settings_Form;

		//------------------------------------------------------------------------------------------
		// Constructor
		//------------------------------------------------------------------------------------------

		public Preferences(YAT.Settings.GeneralSettings settings)
		{
			InitializeComponent();

			_settings = settings;
			_settings_Form = new YAT.Settings.GeneralSettings(settings);
			InitializeControls();
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		public YAT.Settings.GeneralSettings SettingsResult
		{
			get { return (_settings); }
		}

		#endregion

		#region Form Event Handlers
		//------------------------------------------------------------------------------------------
		// Form Event Handlers
		//------------------------------------------------------------------------------------------

		private void Preferences_Paint(object sender, PaintEventArgs e)
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

		private void checkBox_AutoOpenWorkspace_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				_settings_Form.AutoOpenWorkspace = checkBox_AutoOpenWorkspace.Checked;
				SetControls();
			}
		}

		private void checkBox_AutoSaveWorkspace_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				_settings_Form.AutoSaveWorkspace = checkBox_AutoSaveWorkspace.Checked;
				SetControls();
			}
		}

		private void checkBox_UseRelativePaths_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				_settings_Form.UseRelativePaths = checkBox_UseRelativePaths.Checked;
				SetControls();
			}
		}

		private void checkBox_DetectSerialPortsInUse_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				_settings_Form.DetectSerialPortsInUse = checkBox_DetectSerialPortsInUse.Checked;
				SetControls();
			}
		}

		private void button_OK_Click(object sender, EventArgs e)
		{
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
				"Reset preferences to default values?",
				"Defaults?",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button2
				)
				== DialogResult.Yes)
			{
				_settings_Form.SetDefaults();
				SetControls();
			}
		}

		#endregion

		#region Private Methods
		//------------------------------------------------------------------------------------------
		// Private Methods
		//------------------------------------------------------------------------------------------

		private void InitializeControls()
		{
			_isSettingControls = true;

			// nothing to do yet

			_isSettingControls = false;
		}

		private void SetControls()
		{
			_isSettingControls = true;

			checkBox_AutoOpenWorkspace.Checked = _settings_Form.AutoOpenWorkspace;
			checkBox_AutoSaveWorkspace.Checked = _settings_Form.AutoSaveWorkspace;
			checkBox_UseRelativePaths.Checked  = _settings_Form.UseRelativePaths;

			checkBox_DetectSerialPortsInUse.Checked = _settings_Form.DetectSerialPortsInUse;

			_isSettingControls = false;
		}

		#endregion
	}
}