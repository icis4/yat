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
using System.ComponentModel;
using System.Windows.Forms;

using YAT.Gui.Utilities;

namespace YAT.Gui.Forms
{
	/// <summary></summary>
	public partial class AdvancedTerminalSettings : Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isSettingControls = false;

		private Settings.Terminal.ExplicitSettings settings;
		private Settings.Terminal.ExplicitSettings settings_Form;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public AdvancedTerminalSettings(Settings.Terminal.ExplicitSettings settings)
		{
			InitializeComponent();

			this.settings = settings;
			this.settings_Form = new Settings.Terminal.ExplicitSettings(settings);
			InitializeControls();
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public Settings.Terminal.ExplicitSettings SettingsResult
		{
			get { return (this.settings); }
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
		private void ExtendendTerminalSettings_Paint(object sender, PaintEventArgs e)
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

		private void checkBox_SeparateTxRxRadix_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				this.settings_Form.Terminal.Display.SeparateTxRxRadix = checkBox_SeparateTxRxRadix.Checked;
				SetControls();
			}
		}

		private void comboBox_TxRadix_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.Terminal.Display.TxRadix = (Domain.RadixEx)comboBox_TxRadix.SelectedItem;
		}

		private void comboBox_RxRadix_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.Terminal.Display.RxRadix = (Domain.RadixEx)comboBox_RxRadix.SelectedItem;
		}

		private void checkBox_ShowRadix_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.Terminal.Display.ShowRadix = checkBox_ShowRadix.Checked;
		}

		private void checkBox_ShowTimeStamp_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.Terminal.Display.ShowTimeStamp = checkBox_ShowTimeStamp.Checked;
		}

		private void checkBox_ShowLength_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.Terminal.Display.ShowLength = checkBox_ShowLength.Checked;
		}

		private void checkBox_ShowConnectTime_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.Terminal.Display.ShowConnectTime = checkBox_ShowConnectTime.Checked;
		}

		private void checkBox_ShowCounters_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.Terminal.Display.ShowCounters = checkBox_ShowCounters.Checked;
		}

		private void checkBox_DirectionLineBreak_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				this.settings_Form.Terminal.Display.DirectionLineBreakEnabled = checkBox_DirectionLineBreak.Checked;
				SetControls();
			}
		}

		private void textBox_MaxLineCount_TextChanged(object sender, EventArgs e)
		{
			int lines;
			if (int.TryParse(textBox_MaxLineCount.Text, out lines) && (Math.Abs(lines) == 1))
				label_MaxLineCountUnit.Text = "line";
			else
				label_MaxLineCountUnit.Text = "lines";
		}

		private void textBox_MaxLineCount_Validating(object sender, CancelEventArgs e)
		{
			if (!this.isSettingControls)
			{
				int lineCount;
				if (int.TryParse(textBox_MaxLineCount.Text, out lineCount) && (lineCount >= 1))
				{
					this.settings_Form.Terminal.Display.TxMaxLineCount = lineCount;
					this.settings_Form.Terminal.Display.RxMaxLineCount = lineCount;
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

		private void checkBox_ReplaceControlCharacters_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				this.settings_Form.Terminal.CharReplace.ReplaceControlChars = checkBox_ReplaceControlCharacters.Checked;
				SetControls();
			}
		}

		private void comboBox_ControlCharacterRadix_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.Terminal.CharReplace.ControlCharRadix = (Domain.ControlCharRadixEx)comboBox_ControlCharacterRadix.SelectedItem;
		}

		private void checkBox_ReplaceTab_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.Terminal.CharReplace.ReplaceTab = checkBox_ReplaceTab.Checked;
		}

		private void checkBox_ReplaceSpace_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.Terminal.CharReplace.ReplaceSpace = checkBox_ReplaceSpace.Checked;
		}

		private void comboBox_Endianess_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.Terminal.IO.Endianess = (Domain.EndianessEx)comboBox_Endianess.SelectedItem;
		}

		private void checkBox_KeepCommand_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				this.settings_Form.Terminal.Send.KeepCommand = checkBox_KeepCommand.Checked;
				SetControls();
			}
		}

		private void checkBox_CopyPredefined_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				this.settings_Form.Terminal.Send.CopyPredefined = checkBox_CopyPredefined.Checked;
				SetControls();
			}
		}

		private void checkBox_SendImmediately_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				this.settings_Form.Terminal.Send.SendImmediately = checkBox_SendImmediately.Checked;
				SetControls();
			}
		}

		private void checkBox_ReplaceParityError_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				this.settings_Form.Terminal.IO.SerialPort.ReplaceParityErrors = checkBox_ReplaceParityError.Checked;
				SetControls();
			}
		}

		private void textBox_ParityReplacement_Validating(object sender, CancelEventArgs e)
		{
			string replacement = textBox_ParityReplacement.Text;
			int invalidTextStart;
			int invalidTextLength;
			if (Validation.ValidateSequence(this, "Replacement", replacement, out invalidTextStart, out invalidTextLength))
			{
				Domain.Parser.Parser p = new Domain.Parser.Parser();
				byte[] bytes = p.Parse(replacement);
				int length = bytes.Length;
				if (length == 1)
				{
					if (!this.isSettingControls)
					{
						this.settings_Form.Terminal.IO.SerialPort.ParityErrorReplacement = bytes[0];
						this.settings_Form.Terminal.IO.SerialParityErrorReplacement = replacement;
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
						Domain.Settings.IOSettings.SerialParityErrorReplacementDefault;

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
				textBox_ParityReplacement.Select(invalidTextStart, invalidTextLength);
				e.Cancel = true;
			}
		}

		private void textBox_UserName_TextChanged(object sender, EventArgs e)
		{
			// No need to validate the freely definable name.
			this.settings_Form.UserName = textBox_UserName.Text;
		}

		private void button_OK_Click(object sender, EventArgs e)
		{
			this.settings = this.settings_Form;
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
				SetDefaults();
				SetControls();
			}
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void InitializeControls()
		{
			this.isSettingControls = true;

			comboBox_TxRadix.Items.AddRange(Domain.RadixEx.GetItems());
			comboBox_RxRadix.Items.AddRange(Domain.RadixEx.GetItems());
			comboBox_Endianess.Items.AddRange(Domain.EndianessEx.GetItems());

			comboBox_ControlCharacterRadix.Items.Clear();
			comboBox_ControlCharacterRadix.Items.AddRange(Domain.ControlCharRadixEx.GetItems());

			this.isSettingControls = false;
		}

		private void SetControls()
		{
			this.isSettingControls = true;

			// Radix.
			bool separateRadix = this.settings_Form.Terminal.Display.SeparateTxRxRadix;
			if (!separateRadix)
				label_TxRadix.Text = "R&adix:";
			else
				label_TxRadix.Text = "&Tx Radix:";

			comboBox_TxRadix.SelectedItem      = (Domain.RadixEx)this.settings_Form.Terminal.Display.TxRadix;
			checkBox_SeparateTxRxRadix.Checked = separateRadix;
			label_RxRadix.Enabled              = separateRadix;
			comboBox_RxRadix.Enabled           = separateRadix;
			comboBox_RxRadix.SelectedItem      = (Domain.RadixEx)this.settings_Form.Terminal.Display.RxRadix;

			// Display.
			checkBox_ShowRadix.Checked       = this.settings_Form.Terminal.Display.ShowRadix;
			checkBox_ShowTimeStamp.Checked   = this.settings_Form.Terminal.Display.ShowTimeStamp;
			checkBox_ShowLength.Checked      = this.settings_Form.Terminal.Display.ShowLength;
			checkBox_ShowConnectTime.Checked = this.settings_Form.Terminal.Display.ShowConnectTime;
			checkBox_ShowCounters.Checked    = this.settings_Form.Terminal.Display.ShowCounters;

			checkBox_DirectionLineBreak.Checked = this.settings_Form.Terminal.Display.DirectionLineBreakEnabled;
			textBox_MaxLineCount.Text           = this.settings_Form.Terminal.Display.TxMaxLineCount.ToString();

			// Char replace.
			bool replaceControlChars                    = this.settings_Form.Terminal.CharReplace.ReplaceControlChars;
			checkBox_ReplaceControlCharacters.Checked   = replaceControlChars;
			comboBox_ControlCharacterRadix.Enabled      = replaceControlChars;
			comboBox_ControlCharacterRadix.SelectedItem = (Domain.ControlCharRadixEx)this.settings_Form.Terminal.CharReplace.ControlCharRadix;
			checkBox_ReplaceTab.Checked                 = this.settings_Form.Terminal.CharReplace.ReplaceTab;
			checkBox_ReplaceSpace.Checked               = this.settings_Form.Terminal.CharReplace.ReplaceSpace;

			// Communication.
			comboBox_Endianess.SelectedItem = (Domain.EndianessEx)this.settings_Form.Terminal.IO.Endianess;

			// Send.
			checkBox_KeepCommand.Checked    = this.settings_Form.Terminal.Send.KeepCommand;
			checkBox_CopyPredefined.Checked = this.settings_Form.Terminal.Send.CopyPredefined;

			// Receive.
			groupBox_ReceiveSettings.Enabled    = (this.settings_Form.Terminal.IO.IOType == Domain.IOType.SerialPort);
			bool replaceParityErrors            = this.settings_Form.Terminal.IO.SerialPort.ReplaceParityErrors;
			checkBox_ReplaceParityError.Checked = replaceParityErrors;
			textBox_ParityReplacement.Enabled   = replaceParityErrors;
			textBox_ParityReplacement.Text      = this.settings_Form.Terminal.IO.SerialParityErrorReplacement;

			// User.
			textBox_UserName.Text = this.settings_Form.UserName;

			this.isSettingControls = false;
		}

		/// <remarks>
		/// The following list must default the same properties as
		/// <see cref="Gui.Forms.TerminalSettings.ShowAdvancedSettings()"/> handles.
		/// </remarks>
		private void SetDefaults()
		{
			this.settings_Form.Terminal.Display.SeparateTxRxRadix = Domain.Settings.DisplaySettings.SeparateTxRxRadixDefault;
			this.settings_Form.Terminal.Display.TxRadix           = Domain.Settings.DisplaySettings.RadixDefault;
			this.settings_Form.Terminal.Display.RxRadix           = Domain.Settings.DisplaySettings.RadixDefault;

			this.settings_Form.Terminal.Display.ShowRadix       = Domain.Settings.DisplaySettings.ShowRadixDefault;
			this.settings_Form.Terminal.Display.ShowTimeStamp   = Domain.Settings.DisplaySettings.ShowTimeStampDefault;
			this.settings_Form.Terminal.Display.ShowLength      = Domain.Settings.DisplaySettings.ShowLengthDefault;
			this.settings_Form.Terminal.Display.ShowConnectTime = Domain.Settings.DisplaySettings.ShowConnectTimeDefault;
			this.settings_Form.Terminal.Display.ShowCounters    = Domain.Settings.DisplaySettings.ShowCountersDefault;

			this.settings_Form.Terminal.Display.DirectionLineBreakEnabled = Domain.Settings.DisplaySettings.DirectionLineBreakEnabledDefault;
			this.settings_Form.Terminal.Display.TxMaxLineCount            = Domain.Settings.DisplaySettings.MaxLineCountDefault;
			this.settings_Form.Terminal.Display.RxMaxLineCount            = Domain.Settings.DisplaySettings.MaxLineCountDefault;

			this.settings_Form.Terminal.CharReplace.ReplaceControlChars = Domain.Settings.CharReplaceSettings.ReplaceControlCharsDefault;
			this.settings_Form.Terminal.CharReplace.ControlCharRadix    = Domain.Settings.CharReplaceSettings.ControlCharRadixDefault;
			this.settings_Form.Terminal.CharReplace.ReplaceTab          = Domain.Settings.CharReplaceSettings.ReplaceTabDefault;
			this.settings_Form.Terminal.CharReplace.ReplaceSpace        = Domain.Settings.CharReplaceSettings.ReplaceSpaceDefault;

			this.settings_Form.Terminal.IO.Endianess = Domain.Settings.IOSettings.EndianessDefault;

			this.settings_Form.Terminal.Send.KeepCommand    = Domain.Settings.SendSettings.KeepCommandDefault;
			this.settings_Form.Terminal.Send.CopyPredefined = Domain.Settings.SendSettings.CopyPredefinedDefault;

			this.settings_Form.Terminal.IO.SerialPort.ReplaceParityErrors    = MKY.IO.Serial.SerialPortSettings.ReplaceParityErrorsDefault;
			this.settings_Form.Terminal.IO.SerialPort.ParityErrorReplacement = MKY.IO.Serial.SerialPortSettings.ParityErrorReplacementDefault;
			this.settings_Form.Terminal.IO.SerialParityErrorReplacement      = Domain.Settings.IOSettings.SerialParityErrorReplacementDefault;

			this.settings_Form.UserName = Settings.Terminal.ExplicitSettings.UserNameDefault;
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
