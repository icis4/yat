//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 2 Version 1.99.30
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2013 Matthias Kläy.
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
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

using MKY.Windows.Forms;

using YAT.Gui.Utilities;

#endregion

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
		private Settings.Terminal.ExplicitSettings settingsInEdit;

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
			this.settingsInEdit = new Settings.Terminal.ExplicitSettings(settings);
			this.settingsInEdit.Changed += new EventHandler<MKY.Settings.SettingsEventArgs>(settings_Form_Changed);
		}

		private void DetachAndAcceptSettings()
		{
			this.settingsInEdit.Changed -= new EventHandler<MKY.Settings.SettingsEventArgs>(settings_Form_Changed);
			this.settings = this.settingsInEdit;
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
		/// Initially set controls and validate its contents where needed.
		/// </summary>
		/// <remarks>
		/// The 'Shown' event is only raised the first time a form is displayed; subsequently
		/// minimizing, maximizing, restoring, hiding, showing, or invalidating and repainting will
		/// not raise this event again.
		/// Note that the 'Shown' event is raised after the 'Load' event and will also be raised if
		/// the application is started minimized. Also note that operations called in the 'Shown'
		/// event can depend on a properly drawn form, even when a modal dialog (e.g. a message box)
		/// is shown. This is due to the fact that the 'Paint' event will happen right after this
		/// 'Shown' event and will somehow be processed asynchronously.
		/// </remarks>
		private void AdvancedTerminalSettings_Shown(object sender, EventArgs e)
		{
				SetControls();
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void checkBox_SeparateTxRxRadix_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsInEdit.Terminal.Display.SeparateTxRxRadix = checkBox_SeparateTxRxRadix.Checked;
		}

		private void comboBox_TxRadix_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsInEdit.Terminal.Display.TxRadix = (Domain.RadixEx)comboBox_TxRadix.SelectedItem;
		}

		private void comboBox_RxRadix_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsInEdit.Terminal.Display.RxRadix = (Domain.RadixEx)comboBox_RxRadix.SelectedItem;
		}

		private void checkBox_ShowRadix_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsInEdit.Terminal.Display.ShowRadix = checkBox_ShowRadix.Checked;
		}

		private void checkBox_ShowLineNumbers_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsInEdit.Terminal.Display.ShowLineNumbers = checkBox_ShowLineNumbers.Checked;
		}

		private void checkBox_ShowTimeStamp_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsInEdit.Terminal.Display.ShowTimeStamp = checkBox_ShowTimeStamp.Checked;
		}

		private void checkBox_ShowLength_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsInEdit.Terminal.Display.ShowLength = checkBox_ShowLength.Checked;
		}

		private void checkBox_ShowConnectTime_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsInEdit.Terminal.Status.ShowConnectTime = checkBox_ShowConnectTime.Checked;
		}

		private void checkBox_ShowCountAndRate_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsInEdit.Terminal.Status.ShowCountAndRate = checkBox_ShowCountAndRate.Checked;
		}

		private void checkBox_DirectionLineBreak_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsInEdit.Terminal.Display.DirectionLineBreakEnabled = checkBox_DirectionLineBreak.Checked;
		}

		private void textBox_MaxLineCount_TextChanged(object sender, EventArgs e)
		{
			int lines;
			if (int.TryParse(textBox_MaxLineCount.Text, out lines) && (Math.Abs(lines) == 1))
				label_MaxLineCountUnit.Text = "line";
			else
				label_MaxLineCountUnit.Text = "lines";
		}

		[ModalBehavior(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void textBox_MaxLineCount_Validating(object sender, CancelEventArgs e)
		{
			if (!this.isSettingControls)
			{
				int lineCount;
				if (int.TryParse(textBox_MaxLineCount.Text, out lineCount) && (lineCount >= 1))
				{
					this.settingsInEdit.Terminal.Display.TxMaxLineCount = lineCount;
					this.settingsInEdit.Terminal.Display.RxMaxLineCount = lineCount;
				}
				else
				{
					MessageBoxEx.Show
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
				this.settingsInEdit.Terminal.CharReplace.ReplaceControlChars = checkBox_ReplaceControlCharacters.Checked;
		}

		private void comboBox_ControlCharacterRadix_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsInEdit.Terminal.CharReplace.ControlCharRadix = (Domain.ControlCharRadixEx)comboBox_ControlCharacterRadix.SelectedItem;
		}

		private void checkBox_ReplaceTab_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsInEdit.Terminal.CharReplace.ReplaceTab = checkBox_ReplaceTab.Checked;
		}

		private void checkBox_ReplaceSpace_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsInEdit.Terminal.CharReplace.ReplaceSpace = checkBox_ReplaceSpace.Checked;
		}

		private void comboBox_Endianness_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsInEdit.Terminal.IO.Endianness = (Domain.EndiannessEx)comboBox_Endianness.SelectedItem;
		}

		private void checkBox_IndicateBreakStates_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsInEdit.Terminal.IO.IndicateSerialPortBreakStates = checkBox_IndicateBreakStates.Checked;
		}

		private void checkBox_OutputBreakModifiable_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsInEdit.Terminal.IO.SerialPortOutputBreakIsModifiable = checkBox_OutputBreakModifiable.Checked;
		}

		private void checkBox_KeepCommand_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsInEdit.Terminal.Send.KeepCommand = checkBox_KeepCommand.Checked;
		}

		private void checkBox_CopyPredefined_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsInEdit.Terminal.Send.CopyPredefined = checkBox_CopyPredefined.Checked;
		}

		private void textBox_MaxSendChunkSize_TextChanged(object sender, EventArgs e)
		{
			int bytes;
			if (int.TryParse(textBox_MaxSendChunkSize.Text, out bytes) && (Math.Abs(bytes) == 1))
				label_MaxSendChunkSizeUnit.Text = "byte";
			else
				label_MaxSendChunkSizeUnit.Text = "bytes";
		}

		[ModalBehavior(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void textBox_MaxSendChunkSize_Validating(object sender, CancelEventArgs e)
		{
			if (!this.isSettingControls)
			{
				int maxSendChunkSize;
				if (int.TryParse(textBox_MaxSendChunkSize.Text, out maxSendChunkSize) && (maxSendChunkSize >= 1))
				{
					this.settingsInEdit.Terminal.IO.SerialPort.MaxSendChunkSize = maxSendChunkSize;
				}
				else
				{
					MessageBoxEx.Show
						(
						this,
						"Chunk size must be at least 1 byte!",
						"Invalid Input",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
						);

					e.Cancel = true;
				}
			}
		}

		[ModalBehavior(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void textBox_DefaultDelay_Validating(object sender, CancelEventArgs e)
		{
			if (!this.isSettingControls)
			{
				int delay;
				if (int.TryParse(textBox_DefaultDelay.Text, out delay) && (delay >= 1))
				{
					this.settingsInEdit.Terminal.Send.DefaultDelay = delay;
				}
				else
				{
					MessageBoxEx.Show
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

		[ModalBehavior(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void textBox_DefaultLineDelay_Validating(object sender, CancelEventArgs e)
		{
			if (!this.isSettingControls)
			{
				int delay;
				if (int.TryParse(textBox_DefaultLineDelay.Text, out delay) && (delay >= 1))
				{
					this.settingsInEdit.Terminal.Send.DefaultLineDelay = delay;
				}
				else
				{
					MessageBoxEx.Show
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

		private void checkBox_SendImmediately_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsInEdit.Terminal.Send.SendImmediately = checkBox_SendImmediately.Checked;
		}

		private void checkBox_NoSendOnOutputBreak_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsInEdit.Terminal.IO.SerialPort.NoSendOnOutputBreak = checkBox_NoSendOnOutputBreak.Checked;
		}

		private void checkBox_NoSendOnInputBreak_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsInEdit.Terminal.IO.SerialPort.NoSendOnInputBreak = checkBox_NoSendOnInputBreak.Checked;
		}

		private void textBox_UserName_TextChanged(object sender, EventArgs e)
		{
			// No need to validate the freely definable name.
			this.settingsInEdit.UserName = textBox_UserName.Text;
		}

		private void button_OK_Click(object sender, EventArgs e)
		{
			DetachAndAcceptSettings();
		}

		private void button_Cancel_Click(object sender, EventArgs e)
		{
			// Do nothing.
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
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
			comboBox_Endianness.Items.AddRange(Domain.EndiannessEx.GetItems());

			comboBox_ControlCharacterRadix.Items.Clear();
			comboBox_ControlCharacterRadix.Items.AddRange(Domain.ControlCharRadixEx.GetItems());

			this.isSettingControls.Leave();
		}

		private void SetControls()
		{
			bool isSerialPort = (this.settingsInEdit.Terminal.IO.IOType == Domain.IOType.SerialPort);

			this.isSettingControls.Enter();

			// Radix:
			bool separateRadix = this.settingsInEdit.Terminal.Display.SeparateTxRxRadix;
			if (!separateRadix)
				label_TxRadix.Text = "R&adix:";
			else
				label_TxRadix.Text = "&Tx Radix:";

			comboBox_TxRadix.SelectedItem      = (Domain.RadixEx)this.settingsInEdit.Terminal.Display.TxRadix;
			checkBox_SeparateTxRxRadix.Checked = separateRadix;
			label_RxRadix.Enabled              = separateRadix;
			comboBox_RxRadix.Enabled           = separateRadix;
			comboBox_RxRadix.SelectedItem      = (Domain.RadixEx)this.settingsInEdit.Terminal.Display.RxRadix;

			// Display:
			checkBox_ShowRadix.Checked        = this.settingsInEdit.Terminal.Display.ShowRadix;
			checkBox_ShowLineNumbers.Checked  = this.settingsInEdit.Terminal.Display.ShowLineNumbers;
			checkBox_ShowTimeStamp.Checked    = this.settingsInEdit.Terminal.Display.ShowTimeStamp;
			checkBox_ShowLength.Checked       = this.settingsInEdit.Terminal.Display.ShowLength;
			checkBox_ShowConnectTime.Checked  = this.settingsInEdit.Terminal.Status.ShowConnectTime;
			checkBox_ShowCountAndRate.Checked = this.settingsInEdit.Terminal.Status.ShowCountAndRate;

			checkBox_DirectionLineBreak.Checked = this.settingsInEdit.Terminal.Display.DirectionLineBreakEnabled;
			textBox_MaxLineCount.Text           = this.settingsInEdit.Terminal.Display.TxMaxLineCount.ToString(NumberFormatInfo.CurrentInfo);

			// Char replace:
			bool replaceControlChars                    = this.settingsInEdit.Terminal.CharReplace.ReplaceControlChars;
			checkBox_ReplaceControlCharacters.Checked   = replaceControlChars;
			comboBox_ControlCharacterRadix.Enabled      = replaceControlChars;
			comboBox_ControlCharacterRadix.SelectedItem = (Domain.ControlCharRadixEx)this.settingsInEdit.Terminal.CharReplace.ControlCharRadix;
			checkBox_ReplaceTab.Checked                 = this.settingsInEdit.Terminal.CharReplace.ReplaceTab;
			checkBox_ReplaceSpace.Checked               = this.settingsInEdit.Terminal.CharReplace.ReplaceSpace;

			// Communication:
			comboBox_Endianness.SelectedItem        = (Domain.EndiannessEx)this.settingsInEdit.Terminal.IO.Endianness;
			checkBox_IndicateBreakStates.Checked   = this.settingsInEdit.Terminal.IO.IndicateSerialPortBreakStates;
			checkBox_OutputBreakModifiable.Checked = this.settingsInEdit.Terminal.IO.SerialPortOutputBreakIsModifiable;

			// Send:
			checkBox_KeepCommand.Checked     = this.settingsInEdit.Terminal.Send.KeepCommand;
			checkBox_CopyPredefined.Checked  = this.settingsInEdit.Terminal.Send.CopyPredefined;
			checkBox_SendImmediately.Checked = this.settingsInEdit.Terminal.Send.SendImmediately;

			label_MaxSendChunkSizeRemark.Enabled = isSerialPort;
			label_MaxSendChunkSizeUnit.Enabled   = isSerialPort;
			label_MaxSendChunkSize.Enabled       = isSerialPort;
			textBox_MaxSendChunkSize.Enabled     = isSerialPort;
			textBox_MaxSendChunkSize.Text        = this.settingsInEdit.Terminal.IO.SerialPort.MaxSendChunkSize.ToString(CultureInfo.InvariantCulture);

			textBox_DefaultDelay.Text     = this.settingsInEdit.Terminal.Send.DefaultDelay.ToString(CultureInfo.InvariantCulture);
			textBox_DefaultLineDelay.Text = this.settingsInEdit.Terminal.Send.DefaultLineDelay.ToString(CultureInfo.InvariantCulture);

			checkBox_NoSendOnOutputBreak.Enabled = isSerialPort;
			checkBox_NoSendOnOutputBreak.Checked = this.settingsInEdit.Terminal.IO.SerialPort.NoSendOnOutputBreak;

			// Receive:
			groupBox_ReceiveSettings.Enabled    = isSerialPort;
			checkBox_NoSendOnInputBreak.Checked = this.settingsInEdit.Terminal.IO.SerialPort.NoSendOnInputBreak;

			// User:
			textBox_UserName.Text = this.settingsInEdit.UserName;

			this.isSettingControls.Leave();
		}

		/// <remarks>
		/// The following list must default the same properties as
		/// <see cref="Gui.Forms.TerminalSettings.ShowAdvancedSettings()"/> handles.
		/// </remarks>
		private void SetDefaults()
		{
			this.settingsInEdit.SuspendChangeEvent();

			// Radix:
			this.settingsInEdit.Terminal.Display.SeparateTxRxRadix = Domain.Settings.DisplaySettings.SeparateTxRxRadixDefault;
			this.settingsInEdit.Terminal.Display.TxRadix           = Domain.Settings.DisplaySettings.RadixDefault;
			this.settingsInEdit.Terminal.Display.RxRadix           = Domain.Settings.DisplaySettings.RadixDefault;

			// Display:
			this.settingsInEdit.Terminal.Display.ShowRadix       = Domain.Settings.DisplaySettings.ShowRadixDefault;
			this.settingsInEdit.Terminal.Display.ShowTimeStamp   = Domain.Settings.DisplaySettings.ShowTimeStampDefault;
			this.settingsInEdit.Terminal.Display.ShowLength      = Domain.Settings.DisplaySettings.ShowLengthDefault;
			this.settingsInEdit.Terminal.Display.ShowLineNumbers = Domain.Settings.DisplaySettings.ShowLineNumbersDefault;
			this.settingsInEdit.Terminal.Status.ShowConnectTime  = Domain.Settings.StatusSettings.ShowConnectTimeDefault;
			this.settingsInEdit.Terminal.Status.ShowCountAndRate = Domain.Settings.StatusSettings.ShowCountAndRateDefault;

			this.settingsInEdit.Terminal.Display.DirectionLineBreakEnabled = Domain.Settings.DisplaySettings.DirectionLineBreakEnabledDefault;
			this.settingsInEdit.Terminal.Display.TxMaxLineCount            = Domain.Settings.DisplaySettings.MaxLineCountDefault;
			this.settingsInEdit.Terminal.Display.RxMaxLineCount            = Domain.Settings.DisplaySettings.MaxLineCountDefault;

			// Char replace:
			this.settingsInEdit.Terminal.CharReplace.ReplaceControlChars = Domain.Settings.CharReplaceSettings.ReplaceControlCharsDefault;
			this.settingsInEdit.Terminal.CharReplace.ControlCharRadix    = Domain.Settings.CharReplaceSettings.ControlCharRadixDefault;
			this.settingsInEdit.Terminal.CharReplace.ReplaceTab          = Domain.Settings.CharReplaceSettings.ReplaceTabDefault;
			this.settingsInEdit.Terminal.CharReplace.ReplaceSpace        = Domain.Settings.CharReplaceSettings.ReplaceSpaceDefault;

			// Communication:
			this.settingsInEdit.Terminal.IO.Endianness                         = Domain.Settings.IOSettings.EndiannessDefault;
			this.settingsInEdit.Terminal.IO.IndicateSerialPortBreakStates     = Domain.Settings.IOSettings.IndicateSerialPortBreakStatesDefault;
			this.settingsInEdit.Terminal.IO.SerialPortOutputBreakIsModifiable = Domain.Settings.IOSettings.SerialPortOutputBreakIsModifiableDefault;

			// Send:
			this.settingsInEdit.Terminal.Send.KeepCommand                  = Domain.Settings.SendSettings.KeepCommandDefault;
			this.settingsInEdit.Terminal.Send.CopyPredefined               = Domain.Settings.SendSettings.CopyPredefinedDefault;
			this.settingsInEdit.Terminal.Send.SendImmediately              = Domain.Settings.SendSettings.SendImmediatelyDefault;
			this.settingsInEdit.Terminal.IO.SerialPort.MaxSendChunkSize    = MKY.IO.Serial.SerialPort.SerialPortSettings.MaxSendChunkSizeDefault;
			this.settingsInEdit.Terminal.Send.DefaultDelay                 = Domain.Settings.SendSettings.DefaultDelayDefault;
			this.settingsInEdit.Terminal.Send.DefaultLineDelay             = Domain.Settings.SendSettings.DefaultLineDelayDefault;
			this.settingsInEdit.Terminal.IO.SerialPort.NoSendOnOutputBreak = MKY.IO.Serial.SerialPort.SerialPortSettings.NoSendOnOutputBreakDefault;

			// Receive:
			this.settingsInEdit.Terminal.IO.SerialPort.NoSendOnInputBreak = MKY.IO.Serial.SerialPort.SerialPortSettings.NoSendOnInputBreakDefault;

			// User:
			this.settingsInEdit.UserName = Settings.Terminal.ExplicitSettings.UserNameDefault;

			this.settingsInEdit.ResumeChangeEvent();
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
