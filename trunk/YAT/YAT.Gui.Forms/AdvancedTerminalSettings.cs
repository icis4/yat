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
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

using MKY.Windows.Forms;

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

		private SettingControlsHelper isSettingControls;

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

			KeepAndCloneAndAttachSettings(settings);
			InitializeControls();

			// SetControls() is initially called in the 'Paint' event handler.
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

		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

		private void KeepAndCloneAndAttachSettings(Settings.Terminal.ExplicitSettings settings)
		{
			this.settings = settings;
			this.settings_Form = new Settings.Terminal.ExplicitSettings(settings);
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
				this.settings_Form.Terminal.Display.SeparateTxRxRadix = checkBox_SeparateTxRxRadix.Checked;
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
				this.settings_Form.Terminal.Status.ShowConnectTime = checkBox_ShowConnectTime.Checked;
		}

		private void checkBox_ShowCountAndRate_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.Terminal.Status.ShowCountAndRate = checkBox_ShowCountAndRate.Checked;
		}

		private void checkBox_DirectionLineBreak_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.Terminal.Display.DirectionLineBreakEnabled = checkBox_DirectionLineBreak.Checked;
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
				this.settings_Form.Terminal.CharReplace.ReplaceControlChars = checkBox_ReplaceControlCharacters.Checked;
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

		private void checkBox_IndicateBreakStates_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.Terminal.IO.IndicateSerialPortBreakStates = checkBox_IndicateBreakStates.Checked;
		}

		private void checkBox_OutputBreakModifiable_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.Terminal.IO.SerialPortOutputBreakIsModifiable = checkBox_OutputBreakModifiable.Checked;
		}

		private void checkBox_KeepCommand_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.Terminal.Send.KeepCommand = checkBox_KeepCommand.Checked;
		}

		private void checkBox_CopyPredefined_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.Terminal.Send.CopyPredefined = checkBox_CopyPredefined.Checked;
		}

		private void checkBox_SendImmediately_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.Terminal.Send.SendImmediately = checkBox_SendImmediately.Checked;
		}

		private void checkBox_NoSendOnOutputBreak_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.Terminal.IO.SerialPort.NoSendOnOutputBreak = checkBox_NoSendOnOutputBreak.Checked;
		}

		private void checkBox_NoSendOnInputBreak_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.Terminal.IO.SerialPort.NoSendOnInputBreak = checkBox_NoSendOnInputBreak.Checked;
		}

		private void textBox_UserName_TextChanged(object sender, EventArgs e)
		{
			// No need to validate the freely definable name.
			this.settings_Form.UserName = textBox_UserName.Text;
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
				SetDefaults();
			}
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void InitializeControls()
		{
			this.isSettingControls.Enter();

			comboBox_TxRadix.Items.AddRange(Domain.RadixEx.GetItems());
			comboBox_RxRadix.Items.AddRange(Domain.RadixEx.GetItems());
			comboBox_Endianess.Items.AddRange(Domain.EndianessEx.GetItems());

			comboBox_ControlCharacterRadix.Items.Clear();
			comboBox_ControlCharacterRadix.Items.AddRange(Domain.ControlCharRadixEx.GetItems());

			this.isSettingControls.Leave();
		}

		private void SetControls()
		{
			this.isSettingControls.Enter();

			// Radix:
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

			// Display:
			checkBox_ShowRadix.Checked        = this.settings_Form.Terminal.Display.ShowRadix;
			checkBox_ShowTimeStamp.Checked    = this.settings_Form.Terminal.Display.ShowTimeStamp;
			checkBox_ShowLength.Checked       = this.settings_Form.Terminal.Display.ShowLength;
			checkBox_ShowConnectTime.Checked  = this.settings_Form.Terminal.Status.ShowConnectTime;
			checkBox_ShowCountAndRate.Checked = this.settings_Form.Terminal.Status.ShowCountAndRate;

			checkBox_DirectionLineBreak.Checked = this.settings_Form.Terminal.Display.DirectionLineBreakEnabled;
			textBox_MaxLineCount.Text           = this.settings_Form.Terminal.Display.TxMaxLineCount.ToString(NumberFormatInfo.CurrentInfo);

			// Char replace:
			bool replaceControlChars                    = this.settings_Form.Terminal.CharReplace.ReplaceControlChars;
			checkBox_ReplaceControlCharacters.Checked   = replaceControlChars;
			comboBox_ControlCharacterRadix.Enabled      = replaceControlChars;
			comboBox_ControlCharacterRadix.SelectedItem = (Domain.ControlCharRadixEx)this.settings_Form.Terminal.CharReplace.ControlCharRadix;
			checkBox_ReplaceTab.Checked                 = this.settings_Form.Terminal.CharReplace.ReplaceTab;
			checkBox_ReplaceSpace.Checked               = this.settings_Form.Terminal.CharReplace.ReplaceSpace;

			// Communication:
			comboBox_Endianess.SelectedItem        = (Domain.EndianessEx)this.settings_Form.Terminal.IO.Endianess;
			checkBox_IndicateBreakStates.Checked   = this.settings_Form.Terminal.IO.IndicateSerialPortBreakStates;
			checkBox_OutputBreakModifiable.Checked = this.settings_Form.Terminal.IO.SerialPortOutputBreakIsModifiable;

			// Send:
			checkBox_KeepCommand.Checked         = this.settings_Form.Terminal.Send.KeepCommand;
			checkBox_CopyPredefined.Checked      = this.settings_Form.Terminal.Send.CopyPredefined;
			checkBox_SendImmediately.Checked     = this.settings_Form.Terminal.Send.SendImmediately;
			checkBox_NoSendOnOutputBreak.Enabled = (this.settings_Form.Terminal.IO.IOType == Domain.IOType.SerialPort);
			checkBox_NoSendOnOutputBreak.Checked = this.settings_Form.Terminal.IO.SerialPort.NoSendOnOutputBreak;

			// Receive:
			groupBox_ReceiveSettings.Enabled    = (this.settings_Form.Terminal.IO.IOType == Domain.IOType.SerialPort);
			checkBox_NoSendOnInputBreak.Checked = this.settings_Form.Terminal.IO.SerialPort.NoSendOnInputBreak;

			// User:
			textBox_UserName.Text = this.settings_Form.UserName;

			this.isSettingControls.Leave();
		}

		/// <remarks>
		/// The following list must default the same properties as
		/// <see cref="Gui.Forms.TerminalSettings.ShowAdvancedSettings()"/> handles.
		/// </remarks>
		private void SetDefaults()
		{
			this.settings_Form.SuspendChangeEvent();

			// Radix:
			this.settings_Form.Terminal.Display.SeparateTxRxRadix = Domain.Settings.DisplaySettings.SeparateTxRxRadixDefault;
			this.settings_Form.Terminal.Display.TxRadix           = Domain.Settings.DisplaySettings.RadixDefault;
			this.settings_Form.Terminal.Display.RxRadix           = Domain.Settings.DisplaySettings.RadixDefault;

			// Display:
			this.settings_Form.Terminal.Display.ShowRadix       = Domain.Settings.DisplaySettings.ShowRadixDefault;
			this.settings_Form.Terminal.Display.ShowTimeStamp   = Domain.Settings.DisplaySettings.ShowTimeStampDefault;
			this.settings_Form.Terminal.Display.ShowLength      = Domain.Settings.DisplaySettings.ShowLengthDefault;
			this.settings_Form.Terminal.Status.ShowConnectTime  = Domain.Settings.StatusSettings.ShowConnectTimeDefault;
			this.settings_Form.Terminal.Status.ShowCountAndRate = Domain.Settings.StatusSettings.ShowCountAndRateDefault;

			this.settings_Form.Terminal.Display.DirectionLineBreakEnabled = Domain.Settings.DisplaySettings.DirectionLineBreakEnabledDefault;
			this.settings_Form.Terminal.Display.TxMaxLineCount            = Domain.Settings.DisplaySettings.MaxLineCountDefault;
			this.settings_Form.Terminal.Display.RxMaxLineCount            = Domain.Settings.DisplaySettings.MaxLineCountDefault;

			// Char replace:
			this.settings_Form.Terminal.CharReplace.ReplaceControlChars = Domain.Settings.CharReplaceSettings.ReplaceControlCharsDefault;
			this.settings_Form.Terminal.CharReplace.ControlCharRadix    = Domain.Settings.CharReplaceSettings.ControlCharRadixDefault;
			this.settings_Form.Terminal.CharReplace.ReplaceTab          = Domain.Settings.CharReplaceSettings.ReplaceTabDefault;
			this.settings_Form.Terminal.CharReplace.ReplaceSpace        = Domain.Settings.CharReplaceSettings.ReplaceSpaceDefault;

			// Communication:
			this.settings_Form.Terminal.IO.Endianess                         = Domain.Settings.IOSettings.EndianessDefault;
			this.settings_Form.Terminal.IO.IndicateSerialPortBreakStates     = Domain.Settings.IOSettings.IndicateSerialPortBreakStatesDefault;
			this.settings_Form.Terminal.IO.SerialPortOutputBreakIsModifiable = Domain.Settings.IOSettings.SerialPortOutputBreakIsModifiableDefault;

			// Send:
			this.settings_Form.Terminal.Send.KeepCommand                  = Domain.Settings.SendSettings.KeepCommandDefault;
			this.settings_Form.Terminal.Send.CopyPredefined               = Domain.Settings.SendSettings.CopyPredefinedDefault;
			this.settings_Form.Terminal.Send.SendImmediately              = Domain.Settings.SendSettings.SendImmediatelyDefault;
			this.settings_Form.Terminal.IO.SerialPort.NoSendOnOutputBreak = MKY.IO.Serial.SerialPortSettings.NoSendOnOutputBreakDefault;

			// Receive:
			this.settings_Form.Terminal.IO.SerialPort.NoSendOnInputBreak = MKY.IO.Serial.SerialPortSettings.NoSendOnInputBreakDefault;

			// User:
			this.settings_Form.UserName = Settings.Terminal.ExplicitSettings.UserNameDefault;

			this.settings_Form.ResumeChangeEvent();
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
