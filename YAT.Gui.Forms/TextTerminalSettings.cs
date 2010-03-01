//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using MKY.Utilities.Text;

using YAT.Gui.Utilities;

namespace YAT.Gui.Forms
{
	public partial class TextTerminalSettings : System.Windows.Forms.Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _isSettingControls = false;

		private Domain.Settings.TextTerminalSettings _settings;
		private Domain.Settings.TextTerminalSettings _settings_Form;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		public TextTerminalSettings(Domain.Settings.TextTerminalSettings settings)
		{
			InitializeComponent();

			_settings = settings;
			_settings_Form = new Domain.Settings.TextTerminalSettings(settings);
			InitializeControls();
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		public Domain.Settings.TextTerminalSettings SettingsResult
		{
			get { return (_settings); }
		}

		#endregion

		#region Form Event Handlers
		//==========================================================================================
		// Form Event Handlers
		//==========================================================================================

        /// <summary>
        /// Startup flag only used in the following event handler.
        /// </summary>
        private bool _isStartingUp = true;

        /// <summary>
        /// Initially set controls and validate its contents where needed.
        /// </summary>
        private void TextTerminalSettings_Paint(object sender, PaintEventArgs e)
		{
			if (_isStartingUp)
			{
				_isStartingUp = false;
				SetControls();
			}
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void comboBox_Encoding_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				_settings_Form.Encoding = (XEncoding)comboBox_Encoding.SelectedItem;
		}

		private void checkBox_SeparateTxRxEol_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				_settings_Form.SeparateTxRxEol = checkBox_SeparateTxRxEol.Checked;
				SetControls();
			}
		}

		private void comboBox_TxEol_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				Domain.XEol eol = comboBox_TxEol.SelectedItem as Domain.XEol;

				if (eol != null)
					_settings_Form.TxEol = eol.ToSequenceString();
				else
					_settings_Form.TxEol = comboBox_TxEol.Text;

				SetControls();
			}
		}

		private void comboBox_TxEol_Validating(object sender, CancelEventArgs e)
		{
			Domain.XEol eol;
			string eolString = comboBox_TxEol.Text;

			if (Domain.XEol.TryParse(eolString, out eol))
			{
				_settings_Form.TxEol = eol.ToSequenceString();
			}
			else
			{
				int invalidTextStart;
				int invalidTextLength;
				if (Validation.ValidateSequence(this, "EOL", eolString, out invalidTextStart, out invalidTextLength))
				{
					if (!_isSettingControls)
					{
						_settings_Form.TxEol = eolString;
						SetControls();
					}
				}
				else
				{
					comboBox_TxEol.Select(invalidTextStart, invalidTextLength);
					e.Cancel = true;
				}
			}
		}

		private void comboBox_RxEol_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				Domain.XEol eol = comboBox_RxEol.SelectedItem as Domain.XEol;

				if (eol != null)
					_settings_Form.RxEol = eol.ToSequenceString();
				else
					_settings_Form.RxEol = comboBox_RxEol.Text;

				SetControls();
			}
		}

		private void comboBox_RxEol_Validating(object sender, CancelEventArgs e)
		{
			Domain.XEol eol;
			string eolString = comboBox_RxEol.Text;

			if (Domain.XEol.TryParse(eolString, out eol))
			{
				_settings_Form.RxEol = eol.ToSequenceString();
			}
			else
			{
				int invalidTextStart;
				int invalidTextLength;
				if (Validation.ValidateSequence(this, "EOL", eolString, out invalidTextStart, out invalidTextLength))
				{
					if (!_isSettingControls)
					{
						_settings_Form.RxEol = eolString;
						SetControls();
					}
				}
				else
				{
					comboBox_RxEol.Select(invalidTextStart, invalidTextLength);
					e.Cancel = true;
				}
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
			_settings_Form.SetDefaults();
			SetControls();
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void InitializeControls()
		{
			_isSettingControls = true;

			comboBox_TxEol.Items.Clear();
			comboBox_TxEol.Items.AddRange(Domain.XEol.GetItems());

			comboBox_RxEol.Items.Clear();
			comboBox_RxEol.Items.AddRange(Domain.XEol.GetItems());

			comboBox_Encoding.Items.Clear();
			comboBox_Encoding.Items.AddRange(XEncoding.GetItems());

			_isSettingControls = false;
		}

		private void SetControls()
		{
			_isSettingControls = true;

			// encoding
			comboBox_Encoding.SelectedItem = (XEncoding)_settings_Form.Encoding;

			// EOL
			bool separateEol = _settings_Form.SeparateTxRxEol;
			if (!separateEol)
				label_TxEol.Text = "E&OL sequence:";
			else
				label_TxEol.Text = "&Tx EOL sequence:";
			checkBox_SeparateTxRxEol.Checked = separateEol;
			label_RxEol.Enabled = separateEol;
			comboBox_RxEol.Enabled = separateEol;

			Domain.XEol eol;
			if (Domain.XEol.TryParse(_settings_Form.TxEol, out eol))
				comboBox_TxEol.SelectedItem = eol;
			else
				comboBox_TxEol.Text = _settings_Form.TxEol;
			if (Domain.XEol.TryParse(_settings_Form.RxEol, out eol))
				comboBox_RxEol.SelectedItem = eol;
			else
				comboBox_RxEol.Text = _settings_Form.RxEol;

			// display
			checkBox_ShowEol.Checked = _settings_Form.ShowEol;

			// transmit
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

//==================================================================================================
// End of
// $URL$
//==================================================================================================
