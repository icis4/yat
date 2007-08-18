using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MKY.YAT.Gui.Forms
{
	public partial class AdvancedTerminalSettings : Form
	{
		//------------------------------------------------------------------------------------------
		// Fields
		//------------------------------------------------------------------------------------------

		private bool _isStartingUp = true;
		private bool _isSettingControls = false;

		private Domain.Settings.TerminalSettings _settings;
		private Domain.Settings.TerminalSettings _settings_Form;

		//------------------------------------------------------------------------------------------
		// Constructor
		//------------------------------------------------------------------------------------------

		public AdvancedTerminalSettings(Domain.Settings.TerminalSettings settings)
		{
			InitializeComponent();

			_settings = settings;
			_settings_Form = new Domain.Settings.TerminalSettings(settings);
			InitializeControls();
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		public Domain.Settings.TerminalSettings SettingsResult
		{
			get { return (_settings); }
		}

		#endregion

		#region Form Event Handlers
		//------------------------------------------------------------------------------------------
		// Form Event Handlers
		//------------------------------------------------------------------------------------------

		private void ExtendendTerminalSettings_Paint(object sender, PaintEventArgs e)
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

		private void comboBox_Radix_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				_settings_Form.Display.Radix = (Domain.XRadix)comboBox_Radix.SelectedItem;
		}

		private void checkBox_ShowTimeStamp_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				_settings_Form.Display.ShowTimeStamp = checkBox_ShowTimeStamp.Checked;
		}

		private void checkBox_ShowLength_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				_settings_Form.Display.ShowLength = checkBox_ShowLength.Checked;
		}

		private void checkBox_ShowCounters_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				_settings_Form.Display.ShowCounters = checkBox_ShowCounters.Checked;
		}

		private void textBox_MaximalLineCount_TextChanged(object sender, EventArgs e)
		{
			int lines;
			if (int.TryParse(textBox_MaximalLineCount.Text, out lines) && (Math.Abs(lines) == 1))
				label_MaximalLineCountUnit.Text = "line";
			else
				label_MaximalLineCountUnit.Text = "lines";
		}

		private void textBox_MaximalLineCount_Validating(object sender, CancelEventArgs e)
		{
			if (!_isSettingControls)
			{
				int lineCount;
				if (int.TryParse(textBox_MaximalLineCount.Text, out lineCount) || (lineCount >= 1))
				{
					_settings_Form.Display.TxMaximalLineCount = lineCount;
					_settings_Form.Display.RxMaximalLineCount = lineCount;
					SetControls();
				}
				else
				{
					MessageBox.Show
						(
						this,
						"There must be at least 1 line displayed!",
						"Invalid Input",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
						);
					e.Cancel = true;
				}
			}
		}

		private void comboBox_Endianess_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				_settings_Form.IO.Endianess = (Domain.XEndianess)comboBox_Endianess.SelectedItem;
		}

		private void checkBox_LocalEcho_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				_settings_Form.Transmit.LocalEchoEnabled = checkBox_LocalEcho.Checked;
				SetControls();
			}
		}

		private void textBox_ParityReplacement_Validating(object sender, CancelEventArgs e)
		{
			string replacement = textBox_ParityReplacement.Text;
			if (Validation.ValidateSequence(this, "Replacement", replacement))
			{
				Domain.Parser.Parser p = new Domain.Parser.Parser();
				byte[] bytes = p.Parse(replacement);
				int length = bytes.Length;
				if (length == 1)
				{
					if (!_isSettingControls)
					{
						_settings_Form.IO.SerialPort.ParityErrorReplacement = replacement;
						SetControls();
					}
				}
				else
				{
					string message = "Replacement is too ";
					if (length == 0)
						message += "short, it resolves to no byte.";
					else
						message += "long, it resolves to " + length + " bytes.";

					message += Environment.NewLine;
					message += "Enter a replacement that resolves to 1 byte, e.g. " +
						Domain.Settings.SerialPort.SerialPortSettings.ParityErrorReplacementDefault;

					MessageBox.Show
						(
						this,
						message,
						"Invalid Replacement",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
						);
					e.Cancel = true;
				}

			}
			else
			{
				e.Cancel = true;
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
				"Reset settings to default values?",
				"Defaults?",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button2
				)
				== DialogResult.Yes)
			{
				SetDefaults();
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

			comboBox_Radix.Items.AddRange(Domain.XRadix.GetItems());
			comboBox_Endianess.Items.AddRange(Domain.XEndianess.GetItems());

			_isSettingControls = false;
		}

		private void SetControls()
		{
			comboBox_Radix.SelectedItem = (Domain.XRadix)_settings_Form.Display.Radix;
			checkBox_ShowTimeStamp.Checked = _settings_Form.Display.ShowTimeStamp;
			checkBox_ShowLength.Checked = _settings_Form.Display.ShowLength;
			checkBox_ShowCounters.Checked = _settings_Form.Display.ShowCounters;

			textBox_MaximalLineCount.Text = _settings_Form.Display.TxMaximalLineCount.ToString();

			comboBox_Endianess.SelectedItem = (Domain.XEndianess)_settings_Form.IO.Endianess;
			checkBox_LocalEcho.Checked = _settings_Form.Transmit.LocalEchoEnabled;

			groupBox_ReceiveSettings.Enabled = (_settings_Form.IO.IOType == Domain.IOType.SerialPort);
			textBox_ParityReplacement.Text = _settings_Form.IO.SerialPort.ParityErrorReplacement;
		}

		private void SetDefaults()
		{
			_settings_Form.Display.Radix = Domain.Settings.DisplaySettings.RadixDefault;
			_settings_Form.Display.ShowTimeStamp = Domain.Settings.DisplaySettings.ShowTimeStampDefault;
			_settings_Form.Display.ShowLength = Domain.Settings.DisplaySettings.ShowLengthDefault;
			_settings_Form.Display.ShowCounters = Domain.Settings.DisplaySettings.ShowCountersDefault;

			_settings_Form.Display.TxMaximalLineCount = Domain.Settings.DisplaySettings.MaximalLineCountDefault;
			_settings_Form.Display.RxMaximalLineCount = Domain.Settings.DisplaySettings.MaximalLineCountDefault;

			_settings_Form.IO.Endianess = Domain.Settings.IOSettings.EndianessDefault;
			_settings_Form.Transmit.LocalEchoEnabled = Domain.Settings.TransmitSettings.LocalEchoEnabledDefault;

			_settings_Form.IO.SerialPort.ParityErrorReplacement = Domain.Settings.SerialPort.SerialPortSettings.ParityErrorReplacementDefault;
		}

		#endregion
	}
}