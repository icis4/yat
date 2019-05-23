using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using HSR.Utilities.Text;

namespace HSR.YAT.Gui.Forms
{
	public partial class TextTerminalSettings : System.Windows.Forms.Form
	{
		//------------------------------------------------------------------------------------------
		// Attributes
		//------------------------------------------------------------------------------------------

		private bool _isStartingUp = true;
		private bool _isSettingControls = false;

		private Domain.Settings.TextTerminalSettings _settings;
		private Domain.Settings.TextTerminalSettings _settings_Form;

		//------------------------------------------------------------------------------------------
		// Constructor
		//------------------------------------------------------------------------------------------

		public TextTerminalSettings(Domain.Settings.TextTerminalSettings settings)
		{
			InitializeComponent();

			_settings = settings;
			_settings_Form = new Domain.Settings.TextTerminalSettings(settings);
			InitializeControls();
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		public Domain.Settings.TextTerminalSettings SettingsResult
		{
			get { return (_settings); }
		}

		#endregion

		#region Form Event Handlers
		//------------------------------------------------------------------------------------------
		// Form Event Handlers
		//------------------------------------------------------------------------------------------

		private void TextTerminalSettings_Load(object sender, EventArgs e)
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

		private void comboBox_TxEol_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				_settings_Form.TxEol = comboBox_TxEol.Text;
				SetControls();
			}
		}

		private void comboBox_TxEol_Validating(object sender, CancelEventArgs e)
		{
			string eol = comboBox_TxEol.Text;
			if (Validation.ValidateSequence(this, "EOL", eol))
			{
				if (!_isSettingControls)
				{
					_settings_Form.TxEol = eol;
					SetControls();
				}
			}
			else
			{
				e.Cancel = true;
			}
		}

		private void checkBox_SeparateTxRxEol_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				_settings_Form.SeparateTxRxEol = checkBox_SeparateTxRxEol.Checked;
				SetControls();
			}
		}

		private void comboBox_RxEol_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				_settings_Form.RxEol = comboBox_RxEol.Text;
				SetControls();
			}
		}

		private void comboBox_RxEol_Validating(object sender, CancelEventArgs e)
		{
			string eol = comboBox_RxEol.Text;
			if (Validation.ValidateSequence(this, "EOL", eol))
			{
				if (!_isSettingControls)
				{
					_settings_Form.RxEol = eol;
					SetControls();
				}
			}
			else
			{
				e.Cancel = true;
			}
		}

		private void comboBox_Encoding_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				_settings_Form.Encoding = (XEncoding)comboBox_Encoding.SelectedItem;
		}

		private void checkBox_DirectionLineBreak_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				_settings_Form.DirectionLineBreakEnabled = checkBox_DirectionLineBreak.Checked;
				SetControls();
			}
		}

		private void checkBox_ShowEol_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				_settings_Form.ShowEol = checkBox_ShowEol.Checked;
				SetControls();
			}
		}

		private void checkBox_ReplaceControlCharacters_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				_settings_Form.ReplaceControlChars = checkBox_ReplaceControlCharacters.Checked;
				SetControls();
			}
		}

		private void comboBox_ControlCharacterRadix_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				_settings_Form.ControlCharRadix = (Domain.XControlCharRadix)comboBox_ControlCharacterRadix.SelectedItem;
		}

		private void checkBox_Delay_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				Domain.TextLineSendDelay lsd = _settings_Form.LineSendDelay;
				lsd.Enabled = checkBox_Delay.Checked;
				_settings_Form.LineSendDelay = lsd;
				SetControls();
			}
		}

		private void textBox_Delay_Validating(object sender, CancelEventArgs e)
		{
			if (!_isSettingControls)
			{
				int delay;
				if (int.TryParse(textBox_Delay.Text, out delay) && (delay >= 1))
				{
					Domain.TextLineSendDelay lsd = _settings_Form.LineSendDelay;
					lsd.Delay = delay;
					_settings_Form.LineSendDelay = lsd;
					SetControls();
				}
				else
				{
					MessageBox.Show
						(
						this,
						"Delay must be at least 1 ms!",
						"Invalid Input",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
						);
					e.Cancel = true;
				}
			}
		}

		private void textBox_DelayInterval_TextChanged(object sender, EventArgs e)
		{
			int interval;
			if (int.TryParse(textBox_DelayInterval.Text, out interval) && (Math.Abs(interval) == 1))
				label_DelayIntervalUnit.Text = "line";
			else
				label_DelayIntervalUnit.Text = "lines";
		}

		private void textBox_DelayInterval_Validating(object sender, CancelEventArgs e)
		{
			if (!_isSettingControls)
			{
				int interval;
				if (int.TryParse(textBox_DelayInterval.Text, out interval) || (interval >= 1))
				{
					Domain.TextLineSendDelay lsd = _settings_Form.LineSendDelay;
					lsd.LineInterval = interval;
					_settings_Form.LineSendDelay = lsd;
					SetControls();
				}
				else
				{
					MessageBox.Show
						(
						this,
						"Line interval must be at least 1 line!",
						"Invalid Input",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
						);
					e.Cancel = true;
				}
			}
		}

		private void checkBox_WaitForResponse_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				Domain.WaitForResponse wfr = _settings_Form.WaitForResponse;
				wfr.Enabled = checkBox_WaitForResponse.Checked;
				_settings_Form.WaitForResponse = wfr;
				SetControls();
			}
		}

		private void textBox_WaitForResponse_Validating(object sender, CancelEventArgs e)
		{
			if (!_isSettingControls)
			{
				int timeout;
				if (int.TryParse(textBox_WaitForResponse.Text, out timeout) && (timeout >= 1))
				{
					Domain.WaitForResponse wfr = _settings_Form.WaitForResponse;
					wfr.Timeout = timeout;
					_settings_Form.WaitForResponse = wfr;
					SetControls();
				}
				else
				{
					MessageBox.Show
						(
						this,
						"Timeout must be at least 1 ms!",
						"Invalid Input",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
						);
					e.Cancel = true;
				}
			}
		}

		private void radioButton_SubstituteNone_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls && radioButton_SubstituteNone.Checked)
				_settings_Form.CharSubstitution = Domain.CharSubstitution.None;
		}

		private void radioButton_SubstituteToUpper_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls && radioButton_SubstituteToUpper.Checked)
				_settings_Form.CharSubstitution = Domain.CharSubstitution.ToUpper;
		}

		private void radioButton_SubstituteToLower_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls && radioButton_SubstituteToLower.Checked)
				_settings_Form.CharSubstitution = Domain.CharSubstitution.ToLower;
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
				"Reset settings to default values?",
				"Defaults?",
				MessageBoxButtons.YesNoCancel,
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

			comboBox_TxEol.Items.Clear();
			comboBox_TxEol.Items.AddRange(Domain.XEol.GetItems());

			comboBox_RxEol.Items.Clear();
			comboBox_RxEol.Items.AddRange(Domain.XEol.GetItems());

			comboBox_Encoding.Items.Clear();
			comboBox_Encoding.Items.AddRange(XEncoding.GetItems());

			comboBox_ControlCharacterRadix.Items.Clear();
			comboBox_ControlCharacterRadix.Items.AddRange(Domain.XControlCharRadix.GetItems());

			_isSettingControls = false;
		}

		private void SetControls()
		{
			_isSettingControls = true;

			comboBox_TxEol.Text = _settings_Form.TxEol;
			comboBox_RxEol.Text = _settings_Form.RxEol;

			bool separateEol = _settings_Form.SeparateTxRxEol;
			checkBox_SeparateTxRxEol.Checked = separateEol;
			label_RxEol.Enabled = separateEol;
			comboBox_RxEol.Enabled = separateEol;

			comboBox_Encoding.SelectedItem = (XEncoding)_settings_Form.Encoding;

			checkBox_DirectionLineBreak.Checked = _settings_Form.DirectionLineBreakEnabled;

			checkBox_ShowEol.Checked = _settings_Form.ShowEol;

			bool replaceEnabled = _settings_Form.ReplaceControlChars;
			checkBox_ReplaceControlCharacters.Checked = replaceEnabled;
			comboBox_ControlCharacterRadix.Enabled = replaceEnabled;
			comboBox_ControlCharacterRadix.SelectedItem = (Domain.XControlCharRadix)_settings_Form.ControlCharRadix;

			bool delayEnabled = _settings_Form.LineSendDelay.Enabled;
			checkBox_Delay.Checked = delayEnabled;
			textBox_Delay.Enabled = delayEnabled;
			textBox_Delay.Text = _settings_Form.LineSendDelay.Delay.ToString();
			textBox_DelayInterval.Enabled = delayEnabled;
			textBox_DelayInterval.Text = _settings_Form.LineSendDelay.LineInterval.ToString();

			bool waitEnabled = _settings_Form.WaitForResponse.Enabled;
			checkBox_WaitForResponse.Checked = waitEnabled;
			textBox_WaitForResponse.Enabled = waitEnabled;
			textBox_WaitForResponse.Text = _settings_Form.WaitForResponse.Timeout.ToString();

			switch (_settings_Form.CharSubstitution)
			{
				case Domain.CharSubstitution.ToUpper: radioButton_SubstituteToUpper.Checked = true; break;
				case Domain.CharSubstitution.ToLower: radioButton_SubstituteToLower.Checked = true; break;
				default:                              radioButton_SubstituteNone.Checked    = true; break;
			}

			_isSettingControls = false;
		}

		#endregion
	}
}