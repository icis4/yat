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
// YAT Version 2.1.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
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
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

using MKY;
using MKY.Windows.Forms;

using YAT.Settings.Model;

#endregion

namespace YAT.View.Forms
{
	/// <summary></summary>
	public partial class AdvancedTerminalSettings : Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;

		private TerminalExplicitSettings settings;
		private TerminalExplicitSettings settingsInEdit;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public AdvancedTerminalSettings(TerminalExplicitSettings settings)
		{
			InitializeComponent();

			KeepAndCloneAndAttachSettings(settings);
			InitializeControls();
		////SetControls() is initially called in the 'Shown' event handler.
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public TerminalExplicitSettings SettingsResult
		{
			get { return (this.settings); }
		}

		#endregion

		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

		private void KeepAndCloneAndAttachSettings(TerminalExplicitSettings settings)
		{
			this.settings = settings;
			this.settingsInEdit = new TerminalExplicitSettings(settings); // Clone to ensure decoupling.
			this.settingsInEdit.Changed += settings_Form_Changed;
		}

		private void DetachAndAcceptSettings()
		{
			this.settingsInEdit.Changed -= settings_Form_Changed;
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
		/// event can depend on a properly drawn form, as the 'Paint' event of this form and its
		/// child controls has been raised before this 'Shown' event.
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

		private void checkBox_ShowConnectTime_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.Status.ShowConnectTime = checkBox_ShowConnectTime.Checked;
		}

		private void checkBox_ShowCountAndRate_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.Status.ShowCountAndRate = checkBox_ShowCountAndRate.Checked;
		}

		private void checkBox_SeparateTxRxRadix_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.Display.SeparateTxRxRadix = checkBox_SeparateTxRxRadix.Checked;
		}

		private void comboBox_TxRadix_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.Display.TxRadix = (Domain.RadixEx)comboBox_TxRadix.SelectedItem;
		}

		private void comboBox_RxRadix_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.Display.RxRadix = (Domain.RadixEx)comboBox_RxRadix.SelectedItem;
		}

		private void checkBox_ShowRadix_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.Display.ShowRadix = checkBox_ShowRadix.Checked;
		}

		private void checkBox_ShowLineNumbers_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.Display.ShowLineNumbers = checkBox_ShowLineNumbers.Checked;
		}

		private void comboBox_LineNumberSelection_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.Display.LineNumberSelection = (Domain.Utilities.LineNumberSelectionEx)(comboBox_LineNumberSelection.SelectedItem);
		}

		private void checkBox_ShowTimeStamp_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.Display.ShowTimeStamp = checkBox_ShowTimeStamp.Checked;
		}

		private void checkBox_ShowTimeSpan_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.Display.ShowTimeSpan = checkBox_ShowTimeSpan.Checked;
		}

		private void checkBox_ShowTimeDelta_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.Display.ShowTimeDelta = checkBox_ShowTimeDelta.Checked;
		}

		private void checkBox_ShowDevice_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			// Attention:
			// Similar code exists in 'View.Forms.Terminal.toolStripMenuItem_TerminalMenu_View_ShowDevice_Click()'!
			// Changes here must most likely be applied there too.

			if (checkBox_ShowDevice.Checked && !this.settingsInEdit.Terminal.Display.DeviceLineBreakEnabled)
			{
				var isServerSocket = this.settingsInEdit.Terminal.IO.IOTypeIsServerSocket;
				if (isServerSocket) // Attention: This 'isServerSocket' restriction is also implemented at other locations!
				{
					var dr = MessageBoxEx.Show
					(
						this,
						"To enable this setting, lines must be broken when I/O device changes.",
						"Incompatible Setting",
						MessageBoxButtons.OKCancel,
						MessageBoxIcon.Information
					);

					if (dr == DialogResult.OK)
					{
						this.settingsInEdit.Terminal.Display.ShowDevice             = true;
						this.settingsInEdit.Terminal.Display.DeviceLineBreakEnabled = true;
					}
				}
				else
				{
					// Silently make setting compatible:
					this.settingsInEdit.Terminal.Display.ShowDevice             = true;
					this.settingsInEdit.Terminal.Display.DeviceLineBreakEnabled = true;
				}
			}
			else
			{
				this.settingsInEdit.Terminal.Display.ShowDevice = checkBox_ShowDevice.Checked;
			}
		}

		private void checkBox_ShowDirection_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.Display.ShowDirection = checkBox_ShowDirection.Checked;
		}

		private void checkBox_ShowLength_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.Display.ShowLength = checkBox_ShowLength.Checked;
		}

		private void comboBox_LengthSelection_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.Display.LengthSelection = (Domain.Utilities.LengthSelectionEx)(comboBox_LengthSelection.SelectedItem);
		}

		private void checkBox_ShowDuration_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.Display.ShowDuration = checkBox_ShowDuration.Checked;
		}

		private void checkBox_IncludeIOControl_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.Display.IncludeIOControl = checkBox_IncludeIOControl.Checked;
		}

		/// <remarks>
		/// Located before <see cref="checkBox_DeviceLineBreak"/> because the direction line
		/// break is something more important (not only for TCP/IP and UDP/IP servers).
		/// </remarks>
		private void checkBox_DirectionLineBreak_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.Display.DirectionLineBreakEnabled = checkBox_DirectionLineBreak.Checked;
		}

		/// <remarks>
		/// Located after <see cref="checkBox_DirectionLineBreak"/> because the device line
		/// break is something less important (only for TCP/IP and UDP/IP servers).
		/// </remarks>
		private void checkBox_DeviceLineBreak_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			if (!checkBox_DeviceLineBreak.Checked && this.settingsInEdit.Terminal.Display.ShowDevice)
			{
				// No need for 'isServerSocket' restriction as this check box is already restricted.

				var dr = MessageBoxEx.Show
				(
					this,
					"To disable this setting, I/O device can no longer be shown.",
					"Incompatible Setting",
					MessageBoxButtons.OKCancel,
					MessageBoxIcon.Information
				);

				if (dr == DialogResult.OK)
				{
					this.settingsInEdit.Terminal.Display.ShowDevice             = false;
					this.settingsInEdit.Terminal.Display.DeviceLineBreakEnabled = false;
				}
			}
			else
			{
				this.settingsInEdit.Terminal.Display.DeviceLineBreakEnabled = checkBox_DeviceLineBreak.Checked;
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

		[ModalBehaviorContract(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void textBox_MaxLineCount_Validating(object sender, CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			int count;
			if (int.TryParse(textBox_MaxLineCount.Text, out count) && (count >= 1))
			{
				this.settingsInEdit.Terminal.Display.MaxLineCount = count;
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

		private void textBox_MaxLineLength_TextChanged(object sender, EventArgs e)
		{
			bool isText = (this.settingsInEdit.Terminal.TerminalType == Domain.TerminalType.Text);

			int bytes;
			if (int.TryParse(textBox_MaxLineLength.Text, out bytes) && (Math.Abs(bytes) == 1))
				label_MaxLineLengthUnit.Text = (isText ? "character" : "byte") + " per line";
			else
				label_MaxLineLengthUnit.Text = (isText ? "characters" : "bytes") + " per line";
		}

		[ModalBehaviorContract(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void textBox_MaxLineLength_Validating(object sender, CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			int count;
			if (int.TryParse(textBox_MaxLineLength.Text, out count) && (count >= 1))
			{
				this.settingsInEdit.Terminal.Display.MaxLineLength = count;
			}
			else
			{
				MessageBoxEx.Show
				(
					this,
					"There must be at least 1 byte displayed!",
					"Invalid Input",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);

				e.Cancel = true;
			}
		}

		private void checkBox_ShowCopyOfActiveLine_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.Display.ShowCopyOfActiveLine = checkBox_ShowCopyOfActiveLine.Checked;
		}

		private void checkBox_ReplaceControlCharacters_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.CharReplace.ReplaceControlChars = checkBox_ReplaceControlCharacters.Checked;
		}

		private void comboBox_ControlCharacterRadix_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.CharReplace.ControlCharRadix = (Domain.ControlCharRadixEx)comboBox_ControlCharacterRadix.SelectedItem;
		}

		private void checkBox_ReplaceBackspace_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.CharReplace.ReplaceBackspace = checkBox_ReplaceBackspace.Checked;
		}

		private void checkBox_ReplaceTab_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.CharReplace.ReplaceTab = checkBox_ReplaceTab.Checked;
		}

		private void checkBox_HideXOnXOff_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.CharHide.HideXOnXOff = checkBox_HideXOnXOff.Checked;
		}

		private void checkBox_BeepOnBell_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.CharAction.BeepOnBell = checkBox_BeepOnBell.Checked;
		}

		private void checkBox_ReplaceSpace_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.CharReplace.ReplaceSpace = checkBox_ReplaceSpace.Checked;
		}

		private void checkBox_Hide0x00_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.CharHide.Hide0x00 = checkBox_Hide0x00.Checked;
		}

		private void checkBox_Hide0xFF_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.CharHide.Hide0xFF = checkBox_Hide0xFF.Checked;
		}

		private void checkBox_IncludeNonPayloadData_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.IO.UsbSerialHidDevice.IncludeNonPayloadData = checkBox_IncludeNonPayloadData.Checked;
		}

		private void comboBox_Endianness_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

		////this.settingsInEdit.Terminal.IO.Endianness = (Domain.EndiannessEx)comboBox_Endianness.SelectedItem;

			var endianness = (Domain.EndiannessEx)comboBox_Endianness.SelectedItem;
			if (endianness != this.settingsInEdit.Terminal.IO.Endianness)
			{
				string message =
					"The endianness is currently fixed to 'Big-Endian (Network, Motorola)'. " +
					"It was used by former versions of YAT but is currently not used anymore. " +
					"Still, the setting is kept for future enhancements as documented in bug #343.";

				MessageBoxEx.Show
				(
					this,
					message,
					"Limitation",
					MessageBoxButtons.OK,
					MessageBoxIcon.Information
				);

				SetControls();
			}
		}

		private void checkBox_IgnoreFramingErrors_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.IO.SerialPort.IgnoreFramingErrors = checkBox_IgnoreFramingErrors.Checked;
		}

		private void checkBox_IndicateBreakStates_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.IO.IndicateSerialPortBreakStates = checkBox_IndicateBreakStates.Checked;
		}

		private void checkBox_OutputBreakModifiable_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.IO.SerialPortOutputBreakIsModifiable = checkBox_OutputBreakModifiable.Checked;
		}

		private void checkBox_ShowFlowControlCount_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.Status.ShowFlowControlCount = checkBox_ShowFlowControlCount.Checked;
		}

		private void checkBox_ShowBreakCount_CheckedChanged(object sender, EventArgs e)
		{
			 if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.Status.ShowBreakCount = checkBox_ShowBreakCount.Checked;
		}

		private void checkBox_UseExplicitDefaultRadix_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.Send.UseExplicitDefaultRadix = checkBox_UseExplicitDefaultRadix.Checked;
		}

		private void checkBox_KeepSendText_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.Send.Text.KeepSendText = checkBox_KeepSendText.Checked;
		}

		private void checkBox_SendImmediately_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.Send.Text.SendImmediately = checkBox_SendImmediately.Checked;
		}

		private void checkBox_SkipEmptyLines_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.Send.File.SkipEmptyLines = checkBox_SkipEmptyLines.Checked;
		}

		private void checkBox_CopyPredefined_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.Send.CopyPredefined = checkBox_CopyPredefined.Checked;
		}

		private void checkBox_OutputBufferSize_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var lob = this.settingsInEdit.Terminal.IO.SerialPort.OutputBufferSize;
			lob.Enabled = checkBox_OutputBufferSize.Checked;
			this.settingsInEdit.Terminal.IO.SerialPort.OutputBufferSize = lob;
		}

		[ModalBehaviorContract(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void textBox_OutputBufferSize_Validating(object sender, CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			int size;
			if (int.TryParse(textBox_OutputBufferSize.Text, out size) && Int32Ex.IsEven(size) && (size >= 2))
			{
				var lob = this.settingsInEdit.Terminal.IO.SerialPort.OutputBufferSize;
				lob.Size = size;
				this.settingsInEdit.Terminal.IO.SerialPort.OutputBufferSize = lob;
			}
			else
			{
				string message =
					"Size must be an even value and at least 2 bytes!" + Environment.NewLine + Environment.NewLine +
					"Recommended values are powers of two (e.g. 64, 512, 4096). " + Environment.NewLine +
					"The system's default value is 2048.";

				MessageBoxEx.Show
				(
					this,
					message,
					"Invalid Input",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);

				e.Cancel = true;
			}
		}

		private void checkBox_BufferMaxBaudRate_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.IO.SerialPort.BufferMaxBaudRate = checkBox_BufferMaxBaudRate.Checked;
		}

		private void checkBox_MaxChunkSizeEnable_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var cs = this.settingsInEdit.Terminal.IO.SerialPort.MaxChunkSize;
			cs.Enabled = checkBox_MaxChunkSizeEnable.Checked;
			this.settingsInEdit.Terminal.IO.SerialPort.MaxChunkSize = cs;
		}

		private void textBox_MaxChunkSize_TextChanged(object sender, EventArgs e)
		{
			int bytes;
			if (int.TryParse(textBox_MaxChunkSize.Text, out bytes) && (Math.Abs(bytes) == 1))
				label_MaxChunkSizeUnit.Text = "byte";
			else
				label_MaxChunkSizeUnit.Text = "bytes";
		}

		[ModalBehaviorContract(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void textBox_MaxChunkSize_Validating(object sender, CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			int maxSize = this.settingsInEdit.Terminal.IO.SerialPort.MaxChunkSizeMaxSize;
			int size;
			if (int.TryParse(textBox_MaxChunkSize.Text, out size) && (size >= 1) && (size <= maxSize))
			{
				MKY.IO.Serial.SerialPort.ChunkSize sr = this.settingsInEdit.Terminal.IO.SerialPort.MaxChunkSize;
				sr.Size = size;
				this.settingsInEdit.Terminal.IO.SerialPort.MaxChunkSize = sr;
			}
			else
			{
				MessageBoxEx.Show
				(
					this,
					"Size must be between 1 and " + maxSize + " bytes!",
					"Invalid Input",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);

				e.Cancel = true;
			}
		}

		private void checkBox_MaxSendRateEnable_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var sr = this.settingsInEdit.Terminal.IO.SerialPort.MaxSendRate;
			sr.Enabled = checkBox_MaxSendRateEnable.Checked;
			this.settingsInEdit.Terminal.IO.SerialPort.MaxSendRate = sr;
		}

		private void textBox_MaxSendRateSize_TextChanged(object sender, EventArgs e)
		{
			int bytes;
			if (int.TryParse(textBox_MaxSendRateSize.Text, out bytes) && (Math.Abs(bytes) == 1))
				label_MaxSendRateIntervalUnit1.Text = "byte per";
			else
				label_MaxSendRateIntervalUnit1.Text = "bytes per";
		}

		[ModalBehaviorContract(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void textBox_MaxSendRateSize_Validating(object sender, CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			int size;
			if (int.TryParse(textBox_MaxSendRateSize.Text, out size) && (size >= 1))
			{
				var sr = this.settingsInEdit.Terminal.IO.SerialPort.MaxSendRate;
				sr.Size = size;
				this.settingsInEdit.Terminal.IO.SerialPort.MaxSendRate = sr;
			}
			else
			{
				MessageBoxEx.Show
				(
					this,
					"Rate must be at least 1 byte per interval!",
					"Invalid Input",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);

				e.Cancel = true;
			}
		}

		[ModalBehaviorContract(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void textBox_MaxSendRateInterval_Validating(object sender, CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			const int MaxInterval = MKY.IO.Serial.SerialPort.SerialPortSettings.SendRateMaxInterval;

			int interval;
			if (int.TryParse(textBox_MaxSendRateInterval.Text, out interval) &&
				(interval >= 1) && (interval <= MaxInterval))
			{
				var sr = this.settingsInEdit.Terminal.IO.SerialPort.MaxSendRate;
				sr.Interval = interval;
				this.settingsInEdit.Terminal.IO.SerialPort.MaxSendRate = sr;
			}
			else
			{
				MessageBoxEx.Show
				(
					this,
					"Interval must be from 1 to " + MaxInterval.ToString(CultureInfo.CurrentCulture) + " ms!",
					"Invalid Input",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);

				e.Cancel = true;
			}
		}

		private void checkBox_SignalXOnBeforeEachTransmission_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.Send.SignalXOnBeforeEachTransmission = checkBox_SignalXOnBeforeEachTransmission.Checked;
		}

		private void checkBox_SignalXOnPeriodicallyEnable_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var ps = this.settingsInEdit.Terminal.Send.SignalXOnPeriodically;
			ps.Enabled = checkBox_SignalXOnPeriodicallyEnable.Checked;
			this.settingsInEdit.Terminal.Send.SignalXOnPeriodically = ps;
		}

		private void textBox_SignalXOnPeriodicallyInterval_Validating(object sender, CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			int interval;
			if (int.TryParse(textBox_SignalXOnPeriodicallyInterval.Text, out interval) && (interval >= 1))
			{
				var ps = this.settingsInEdit.Terminal.Send.SignalXOnPeriodically;
				ps.Interval = interval;
				this.settingsInEdit.Terminal.Send.SignalXOnPeriodically = ps;
			}
			else
			{
				MessageBoxEx.Show
				(
					this,
					"Interval must be at least 1 ms!",
					"Invalid Input",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);

				e.Cancel = true;
			}
		}

		private void checkBox_NoSendOnOutputBreak_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.IO.SerialPort.NoSendOnOutputBreak = checkBox_NoSendOnOutputBreak.Checked;
		}

		private void checkBox_NoSendOnInputBreak_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.IO.SerialPort.NoSendOnInputBreak = checkBox_NoSendOnInputBreak.Checked;
		}

		private void checkBox_EnableEscapesForText_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.Send.Text.EnableEscapes = checkBox_EnableEscapesForText.Checked;
		}

		private void checkBox_EnableEscapesForFile_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.Send.File.EnableEscapes = checkBox_EnableEscapesForFile.Checked;
		}

		[ModalBehaviorContract(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void textBox_DefaultDelay_Validating(object sender, CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			int delay;
			if (int.TryParse(textBox_DefaultDelay.Text, out delay) && (delay >= 1)) // Attention, a similar validation exists in 'Domain.Parser.KeywordEx'. Changes here may have to be applied there too.
			{
				this.settingsInEdit.Terminal.Send.DefaultDelay = delay;
			}
			else
			{
				MessageBoxEx.Show
				(
					this,
					"Delay must be at least 1 ms!", // Attention, a similar message exists in 'Domain.Parser.KeywordEx'. Changes here may have to be applied there too.
					"Invalid Input",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);

				e.Cancel = true;
			}
		}

		[ModalBehaviorContract(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void textBox_DefaultLineDelay_Validating(object sender, CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			int delay;
			if (int.TryParse(textBox_DefaultLineDelay.Text, out delay) && (delay >= 1)) // Attention, a similar validation exists in 'Domain.Parser.KeywordEx'. Changes here may have to be applied there too.
			{
				this.settingsInEdit.Terminal.Send.DefaultLineDelay = delay;
			}
			else
			{
				MessageBoxEx.Show
				(
					this,
					"Delay must be at least 1 ms!", // Attention, a similar message exists in 'Domain.Parser.KeywordEx'. Changes here may have to be applied there too.
					"Invalid Input",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);

				e.Cancel = true;
			}
		}

		[ModalBehaviorContract(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void textBox_DefaultLineInterval_Validating(object sender, CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			int interval;
			if (int.TryParse(textBox_DefaultLineInterval.Text, out interval) && (interval >= 1)) // Attention, a similar validation exists in 'Domain.Parser.KeywordEx'. Changes here may have to be applied there too.
			{
				this.settingsInEdit.Terminal.Send.DefaultLineInterval = interval;
			}
			else
			{
				MessageBoxEx.Show
				(
					this,
					"Interval must be at least 1 ms!", // Attention, a similar message exists in 'Domain.Parser.KeywordEx'. Changes here may have to be applied there too.
					"Invalid Input",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);

				e.Cancel = true;
			}
		}

		private void textBox_DefaultLineRepeat_TextChanged(object sender, EventArgs e)
		{
			int repeat;
			if (int.TryParse(textBox_DefaultLineRepeat.Text, out repeat) && (repeat < 0))
				label_DefaultLineRepeatUnit.Text = "= infinite";
			else if (repeat == 0)
				label_DefaultLineRepeatUnit.Text = "= never";
			else if (repeat == 1)
				label_DefaultLineRepeatUnit.Text = "= once";
			else if (repeat == 2)
				label_DefaultLineRepeatUnit.Text = "= twice";
			else
				label_DefaultLineRepeatUnit.Text = "times";
		}

		[ModalBehaviorContract(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void textBox_DefaultLineRepeat_Validating(object sender, CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			const int RepeatInfinite = Domain.Settings.SendSettings.LineRepeatInfinite;

			int repeat;
			if (int.TryParse(textBox_DefaultLineRepeat.Text, out repeat) && ((repeat >= 1) || (repeat == RepeatInfinite))) // Attention, a similar validation exists in 'Domain.Parser.KeywordEx'. Changes here may have to be applied there too.
			{
				this.settingsInEdit.Terminal.Send.DefaultLineRepeat = repeat;
			}
			else
			{
				MessageBoxEx.Show
				(
					this,
					"Repeat must be 1 or more! Or " + RepeatInfinite + " for infinite repeating.", // Attention, a similar message exists in 'Domain.Parser.KeywordEx'. Changes here may have to be applied there too.
					"Invalid Input",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);

				e.Cancel = true;
			}
		}

		/// <remarks>
		/// No need to validate the freely definable name.
		/// </remarks>
		private void textBox_UserName_TextChanged(object sender, EventArgs e)
		{
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
				SetDefaults();
			}
		}

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		private void InitializeControls()
		{
			this.isSettingControls.Enter();
			try
			{
				comboBox_TxRadix              .Items.AddRange(Domain.RadixEx                        .GetItems());
				comboBox_RxRadix              .Items.AddRange(Domain.RadixEx                        .GetItems());
				comboBox_LineNumberSelection  .Items.AddRange(Domain.Utilities.LineNumberSelectionEx.GetItems());
				comboBox_LengthSelection      .Items.AddRange(Domain.Utilities.LengthSelectionEx    .GetItems());
				comboBox_Endianness           .Items.AddRange(Domain.EndiannessEx                   .GetItems());
				comboBox_ControlCharacterRadix.Items.AddRange(Domain.ControlCharRadixEx             .GetItems());

				if (this.settingsInEdit.Terminal.TerminalType == Domain.TerminalType.Text)
					this.toolTip.SetToolTip(this.textBox_MaxLineLength, "The maximal number of characters per line is limited to improve performance.");
				else                   // incl. (TerminalType == Domain.TerminalType.Binary)
					this.toolTip.SetToolTip(this.textBox_MaxLineLength, "The maximal number of bytes per line is limited to improve performance.");
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void SetControls()
		{
			bool isText   = (this.settingsInEdit.Terminal.TerminalType == Domain.TerminalType.Text);
			bool isBinary = (this.settingsInEdit.Terminal.TerminalType == Domain.TerminalType.Binary);

			bool isSerialPort   = (this.settingsInEdit.Terminal.IO.IOType == Domain.IOType.SerialPort);
			bool isUsbSerialHid = (this.settingsInEdit.Terminal.IO.IOType == Domain.IOType.UsbSerialHid);

			bool isServerSocket = this.settingsInEdit.Terminal.IO.IOTypeIsServerSocket;

			this.isSettingControls.Enter();
			try
			{
				// Status:
				checkBox_ShowConnectTime.Checked  = this.settingsInEdit.Terminal.Status.ShowConnectTime;
				checkBox_ShowCountAndRate.Checked = this.settingsInEdit.Terminal.Status.ShowCountAndRate;

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

				bool isShowable = ((this.settingsInEdit.Terminal.Display.TxRadixIsShowable) ||
				                   (this.settingsInEdit.Terminal.Display.RxRadixIsShowable));
				checkBox_ShowRadix.Enabled =  isShowable; // Attention: This 'isShowable' restriction also exists in two locations in 'View.Forms.Terminal'.
				checkBox_ShowRadix.Checked = (isShowable && this.settingsInEdit.Terminal.Display.ShowRadix);

				// Display:
				checkBox_ShowLineNumbers.Checked    = this.settingsInEdit.Terminal.Display.ShowLineNumbers;
				ComboBoxHelper.Select(comboBox_LineNumberSelection, (Domain.Utilities.LineNumberSelectionEx)this.settingsInEdit.Terminal.Display.LineNumberSelection);
				checkBox_ShowTimeStamp.Checked      = this.settingsInEdit.Terminal.Display.ShowTimeStamp;
				checkBox_ShowTimeSpan.Checked       = this.settingsInEdit.Terminal.Display.ShowTimeSpan;
				checkBox_ShowTimeDelta.Checked      = this.settingsInEdit.Terminal.Display.ShowTimeDelta;
				checkBox_ShowDevice.Checked         = this.settingsInEdit.Terminal.Display.ShowDevice;
				checkBox_ShowDirection.Checked      = this.settingsInEdit.Terminal.Display.ShowDirection;
				checkBox_ShowLength.Checked         = this.settingsInEdit.Terminal.Display.ShowLength;

				if (!isBinary) {
					comboBox_LengthSelection.Enabled = true;
					ComboBoxHelper.Select(comboBox_LengthSelection, (Domain.Utilities.LengthSelectionEx)this.settingsInEdit.Terminal.Display.LengthSelection);
				}
				else { // isBinary => preset and disable:
					ComboBoxHelper.Select(comboBox_LengthSelection, (Domain.Utilities.LengthSelectionEx)Domain.Utilities.LengthSelection.ByteCount);
					comboBox_LengthSelection.Enabled = false;
				}

				checkBox_ShowDuration.Checked     = this.settingsInEdit.Terminal.Display.ShowDuration;
				checkBox_IncludeIOControl.Enabled = (isSerialPort || isUsbSerialHid);
				checkBox_IncludeIOControl.Checked = this.settingsInEdit.Terminal.Display.IncludeIOControl;

				checkBox_DirectionLineBreak.Checked =  this.settingsInEdit.Terminal.Display.DirectionLineBreakEnabled;
				checkBox_DeviceLineBreak.Enabled    =  isServerSocket; // Attention: This 'isServerSocket' restriction is also implemented at other locations!
				checkBox_DeviceLineBreak.Checked    = (isServerSocket && this.settingsInEdit.Terminal.Display.DeviceLineBreakEnabled);
				label_LineBreakRemark.Text          = "Also see" + Environment.NewLine + "[" + (!isBinary ? "Text" : "Binary") + " Settings...]";

				textBox_MaxLineCount.Text             = this.settingsInEdit.Terminal.Display.MaxLineCount .ToString(CultureInfo.CurrentCulture);
				textBox_MaxLineLength.Text            = this.settingsInEdit.Terminal.Display.MaxLineLength.ToString(CultureInfo.CurrentCulture);
				checkBox_ShowCopyOfActiveLine.Checked = this.settingsInEdit.Terminal.Display.ShowCopyOfActiveLine;

				// Char replace:
				bool replaceControlChars                    = this.settingsInEdit.Terminal.CharReplace.ReplaceControlChars;
				checkBox_ReplaceControlCharacters.Checked   = replaceControlChars;
				comboBox_ControlCharacterRadix.Enabled      = replaceControlChars;
				comboBox_ControlCharacterRadix.SelectedItem = (Domain.ControlCharRadixEx)this.settingsInEdit.Terminal.CharReplace.ControlCharRadix;

				bool replaceBackspaceEnabled                = (isText && replaceControlChars);
				checkBox_ReplaceBackspace.Enabled           =  replaceBackspaceEnabled;
				checkBox_ReplaceBackspace.Checked           = (replaceBackspaceEnabled && this.settingsInEdit.Terminal.CharReplace.ReplaceBackspace);
				label_ReplaceBackspace.Enabled              =  replaceBackspaceEnabled;

				bool replaceTabEnabled                      = (isText && replaceControlChars);
				checkBox_ReplaceTab.Enabled                 =  replaceTabEnabled;
				checkBox_ReplaceTab.Checked                 = (replaceTabEnabled && this.settingsInEdit.Terminal.CharReplace.ReplaceTab);
				label_ReplaceTab.Enabled                    =  replaceTabEnabled;

				checkBox_HideXOnXOff.Enabled                = this.settingsInEdit.Terminal.IO.FlowControlUsesXOnXOff;
				checkBox_HideXOnXOff.Checked                = this.settingsInEdit.Terminal.CharHide.HideXOnXOff;
				checkBox_ReplaceSpace.Checked               = this.settingsInEdit.Terminal.CharReplace.ReplaceSpace;
				checkBox_Hide0x00.Checked                   = this.settingsInEdit.Terminal.CharHide.Hide0x00;
				checkBox_Hide0xFF.Enabled                   = this.settingsInEdit.Terminal.SupportsHide0xFF;
				checkBox_Hide0xFF.Checked                   = this.settingsInEdit.Terminal.CharHide.Hide0xFF;

				bool beepOnBellEnabled                      =  isText;
				checkBox_BeepOnBell.Enabled                 =  beepOnBellEnabled;
				checkBox_BeepOnBell.Checked                 = (beepOnBellEnabled && this.settingsInEdit.Terminal.CharAction.BeepOnBell);

				groupBox_Display_UsbSerialHid.Enabled       = isUsbSerialHid;
				checkBox_IncludeNonPayloadData.Checked      = this.settingsInEdit.Terminal.IO.UsbSerialHidDevice.IncludeNonPayloadData;

				// Communication:
				comboBox_Endianness.SelectedItem = (Domain.EndiannessEx)this.settingsInEdit.Terminal.IO.Endianness;

				groupBox_Communication_SerialPorts.Enabled = isSerialPort;
				checkBox_IgnoreFramingErrors.Checked       = this.settingsInEdit.Terminal.IO.SerialPort.IgnoreFramingErrors;
				checkBox_IndicateBreakStates.Checked       = this.settingsInEdit.Terminal.IO.IndicateSerialPortBreakStates;
				checkBox_OutputBreakModifiable.Checked     = this.settingsInEdit.Terminal.IO.SerialPortOutputBreakIsModifiable;

				checkBox_ShowBreakCount.Enabled            = this.settingsInEdit.Terminal.IO.IndicateSerialPortBreakStates;
				checkBox_ShowBreakCount.Checked            = this.settingsInEdit.Terminal.Status.ShowBreakCount;
				checkBox_ShowFlowControlCount.Enabled      = this.settingsInEdit.Terminal.IO.FlowControlIsInUse;
				checkBox_ShowFlowControlCount.Checked      = this.settingsInEdit.Terminal.Status.ShowFlowControlCount;

				// Send:
				checkBox_UseExplicitDefaultRadix.Checked =  this.settingsInEdit.Terminal.Send.UseExplicitDefaultRadix;
				checkBox_KeepSendText.Enabled            = !this.settingsInEdit.Terminal.Send.Text.SendImmediately;
				checkBox_KeepSendText.Checked            = !this.settingsInEdit.Terminal.Send.Text.SendImmediately && this.settingsInEdit.Terminal.Send.Text.KeepSendText;
				checkBox_SendImmediately.Checked         =  this.settingsInEdit.Terminal.Send.Text.SendImmediately;
				checkBox_SkipEmptyLines.Checked          =  this.settingsInEdit.Terminal.Send.File.SkipEmptyLines;
				checkBox_CopyPredefined.Checked          =  this.settingsInEdit.Terminal.Send.CopyPredefined;

				checkBox_SignalXOnBeforeEachTransmission.Enabled = this.settingsInEdit.Terminal.IO.FlowControlUsesXOnXOff;
				checkBox_SignalXOnBeforeEachTransmission.Checked = this.settingsInEdit.Terminal.Send.SignalXOnBeforeEachTransmission;
				checkBox_SignalXOnPeriodicallyEnable.Enabled     = this.settingsInEdit.Terminal.IO.FlowControlUsesXOnXOff;
				checkBox_SignalXOnPeriodicallyEnable.Checked     = this.settingsInEdit.Terminal.Send.SignalXOnPeriodically.Enabled;
				textBox_SignalXOnPeriodicallyInterval.Enabled    = this.settingsInEdit.Terminal.IO.FlowControlUsesXOnXOff;
				textBox_SignalXOnPeriodicallyInterval.Text       = this.settingsInEdit.Terminal.Send.SignalXOnPeriodically.Interval.ToString(CultureInfo.CurrentCulture);
				label_SignalXOnPeriodicallyIntervalUnit.Enabled  = this.settingsInEdit.Terminal.IO.FlowControlUsesXOnXOff;

				groupBox_Send_SerialPorts.Enabled    = isSerialPort;
				checkBox_OutputBufferSize.Checked    = this.settingsInEdit.Terminal.IO.SerialPort.OutputBufferSize.Enabled;
				textBox_OutputBufferSize.Enabled     = this.settingsInEdit.Terminal.IO.SerialPort.OutputBufferSize.Enabled;
				textBox_OutputBufferSize.Text        = this.settingsInEdit.Terminal.IO.SerialPort.OutputBufferSize.Size.ToString(CultureInfo.CurrentCulture);
				checkBox_BufferMaxBaudRate.Checked   = this.settingsInEdit.Terminal.IO.SerialPort.BufferMaxBaudRate;
				checkBox_MaxChunkSizeEnable.Checked  = this.settingsInEdit.Terminal.IO.SerialPort.MaxChunkSize.Enabled;
				textBox_MaxChunkSize.Enabled         = this.settingsInEdit.Terminal.IO.SerialPort.MaxChunkSize.Enabled;
				textBox_MaxChunkSize.Text            = this.settingsInEdit.Terminal.IO.SerialPort.MaxChunkSize.Size.ToString(CultureInfo.CurrentCulture);
				checkBox_MaxSendRateEnable.Checked   = this.settingsInEdit.Terminal.IO.SerialPort.MaxSendRate.Enabled;
				textBox_MaxSendRateSize.Enabled      = this.settingsInEdit.Terminal.IO.SerialPort.MaxSendRate.Enabled;
				textBox_MaxSendRateSize.Text         = this.settingsInEdit.Terminal.IO.SerialPort.MaxSendRate.Size.ToString(CultureInfo.CurrentCulture);
				textBox_MaxSendRateInterval.Enabled  = this.settingsInEdit.Terminal.IO.SerialPort.MaxSendRate.Enabled;
				textBox_MaxSendRateInterval.Text     = this.settingsInEdit.Terminal.IO.SerialPort.MaxSendRate.Interval.ToString(CultureInfo.CurrentCulture);
				checkBox_NoSendOnOutputBreak.Checked = this.settingsInEdit.Terminal.IO.SerialPort.NoSendOnOutputBreak;
				checkBox_NoSendOnInputBreak.Checked  = this.settingsInEdit.Terminal.IO.SerialPort.NoSendOnInputBreak;

				checkBox_EnableEscapesForText.Checked = this.settingsInEdit.Terminal.Send.Text.EnableEscapes;
				checkBox_EnableEscapesForFile.Checked = this.settingsInEdit.Terminal.Send.File.EnableEscapes;

				groupBox_Send_Keywords.Enabled   = (this.settingsInEdit.Terminal.Send.Text.EnableEscapes || this.settingsInEdit.Terminal.Send.File.EnableEscapes);
				textBox_DefaultDelay.Text        =  this.settingsInEdit.Terminal.Send.DefaultDelay       .ToString(CultureInfo.CurrentCulture);
				textBox_DefaultLineDelay.Text    =  this.settingsInEdit.Terminal.Send.DefaultLineDelay   .ToString(CultureInfo.CurrentCulture);
				textBox_DefaultLineInterval.Text =  this.settingsInEdit.Terminal.Send.DefaultLineInterval.ToString(CultureInfo.CurrentCulture);
				textBox_DefaultLineRepeat.Text   =  this.settingsInEdit.Terminal.Send.DefaultLineRepeat  .ToString(CultureInfo.CurrentCulture);

				// User:
				textBox_UserName.Text = this.settingsInEdit.UserName;

				// Remark:
				label_TextSettingsRemark.Visible = isText;
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		/// <remarks>
		/// The following list must default the same properties as
		/// <see cref="TerminalSettings.ShowAdvancedSettings()"/> handles.
		/// </remarks>
		/// <remarks>
		/// The following list must default the same values as
		/// <see cref="Domain.Settings.TerminalSettings.TerminalType"/> handles.
		/// </remarks>
		private void SetDefaults()
		{
			this.settingsInEdit.SuspendChangeEvent();
			try
			{
				// Status:
				this.settingsInEdit.Terminal.Status.ShowConnectTime  = Domain.Settings.StatusSettings.ShowConnectTimeDefault;
				this.settingsInEdit.Terminal.Status.ShowCountAndRate = Domain.Settings.StatusSettings.ShowCountAndRateDefault;

				// Radix:
				this.settingsInEdit.Terminal.Display.SeparateTxRxRadix = Domain.Settings.DisplaySettings.SeparateTxRxRadixDefault;
				this.settingsInEdit.Terminal.Display.TxRadix           = Domain.Settings.DisplaySettings.RadixDefault;
				this.settingsInEdit.Terminal.Display.RxRadix           = Domain.Settings.DisplaySettings.RadixDefault;

				// Display:
				this.settingsInEdit.Terminal.Display.ShowRadix           = Domain.Settings.DisplaySettings.ShowRadixDefault;
				this.settingsInEdit.Terminal.Display.ShowLineNumbers     = Domain.Settings.DisplaySettings.ShowLineNumbersDefault;
				this.settingsInEdit.Terminal.Display.LineNumberSelection = Domain.Settings.DisplaySettings.LineNumberSelectionDefault;
				this.settingsInEdit.Terminal.Display.ShowTimeStamp       = Domain.Settings.DisplaySettings.ShowTimeStampDefault;
				this.settingsInEdit.Terminal.Display.ShowTimeSpan        = Domain.Settings.DisplaySettings.ShowTimeSpanDefault;
				this.settingsInEdit.Terminal.Display.ShowTimeDelta       = Domain.Settings.DisplaySettings.ShowTimeDeltaDefault;
				this.settingsInEdit.Terminal.Display.ShowDevice          = Domain.Settings.DisplaySettings.ShowDeviceDefault;
				this.settingsInEdit.Terminal.Display.ShowDirection       = Domain.Settings.DisplaySettings.ShowDirectionDefault;
				this.settingsInEdit.Terminal.Display.ShowLength          = Domain.Settings.DisplaySettings.ShowLengthDefault;
				this.settingsInEdit.Terminal.Display.LengthSelection     = Domain.Settings.DisplaySettings.LengthSelectionDefault;
				this.settingsInEdit.Terminal.Display.ShowDuration        = Domain.Settings.DisplaySettings.ShowDurationDefault;
				this.settingsInEdit.Terminal.Display.IncludeIOControl    = Domain.Settings.DisplaySettings.IncludeIOControlDefault;

				this.settingsInEdit.Terminal.Display.DirectionLineBreakEnabled = Domain.Settings.DisplaySettings.DirectionLineBreakEnabledDefault;
				this.settingsInEdit.Terminal.Display.DeviceLineBreakEnabled    = Domain.Settings.DisplaySettings.DeviceLineBreakEnabledDefault;
				this.settingsInEdit.Terminal.Display.MaxLineCount              = Domain.Settings.DisplaySettings.MaxLineCountDefault;
				this.settingsInEdit.Terminal.Display.MaxLineLength             = Domain.Settings.DisplaySettings.MaxLineLengthDefault;

				// Char replace/hide:
				this.settingsInEdit.Terminal.CharReplace.ReplaceControlChars = Domain.Settings.CharReplaceSettings.ReplaceControlCharsDefault;
				this.settingsInEdit.Terminal.CharReplace.ControlCharRadix    = Domain.Settings.CharReplaceSettings.ControlCharRadixDefault;
				this.settingsInEdit.Terminal.CharReplace.ReplaceBackspace    = Domain.Settings.CharReplaceSettings.ReplaceBackspaceDefault;
				this.settingsInEdit.Terminal.CharReplace.ReplaceTab          = Domain.Settings.CharReplaceSettings.ReplaceTabDefault;
				this.settingsInEdit.Terminal.CharHide.HideXOnXOff            = Domain.Settings.CharHideSettings.HideXOnXOffDefault;
				this.settingsInEdit.Terminal.CharAction.BeepOnBell           = Domain.Settings.CharActionSettings.BeepOnBellDefault;
				this.settingsInEdit.Terminal.CharReplace.ReplaceSpace        = Domain.Settings.CharReplaceSettings.ReplaceSpaceDefault;
				this.settingsInEdit.Terminal.CharHide.Hide0x00               = Domain.Settings.CharHideSettings.Hide0x00Default;
				this.settingsInEdit.Terminal.CharHide.Hide0xFF               = Domain.Settings.CharHideSettings.Hide0xFFDefault;

				// USB Ser/HID:
				this.settingsInEdit.Terminal.IO.UsbSerialHidDevice.IncludeNonPayloadData = MKY.IO.Serial.Usb.SerialHidDeviceSettings.IncludeNonPayloadDataDefault;

				// Communication:
				this.settingsInEdit.Terminal.IO.Endianness                        = Domain.Settings.IOSettings.EndiannessDefault;
				this.settingsInEdit.Terminal.IO.SerialPort.IgnoreFramingErrors    = MKY.IO.Serial.SerialPort.SerialPortSettings.IgnoreFramingErrorsDefault;
				this.settingsInEdit.Terminal.IO.IndicateSerialPortBreakStates     = Domain.Settings.IOSettings.IndicateSerialPortBreakStatesDefault;
				this.settingsInEdit.Terminal.IO.SerialPortOutputBreakIsModifiable = Domain.Settings.IOSettings.SerialPortOutputBreakIsModifiableDefault;

				this.settingsInEdit.Terminal.Status.ShowFlowControlCount          = Domain.Settings.StatusSettings.ShowFlowControlCountDefault;
				this.settingsInEdit.Terminal.Status.ShowBreakCount                = Domain.Settings.StatusSettings.ShowBreakCountDefault;

				// Send:
				this.settingsInEdit.Terminal.Send.UseExplicitDefaultRadix         = Domain.Settings.SendSettings.UseExplicitDefaultRadixDefault;
				this.settingsInEdit.Terminal.Send.CopyPredefined                  = Domain.Settings.SendSettings.CopyPredefinedDefault;
				this.settingsInEdit.Terminal.Send.Text.KeepSendText               = Domain.Settings.SendSettingsText.KeepSendTextDefault;
				this.settingsInEdit.Terminal.Send.Text.SendImmediately            = Domain.Settings.SendSettingsText.SendImmediatelyDefault;

				this.settingsInEdit.Terminal.Send.SignalXOnBeforeEachTransmission = Domain.Settings.SendSettings.SignalXOnBeforeEachTransmissionDefault;
				this.settingsInEdit.Terminal.Send.SignalXOnPeriodically           = Domain.Settings.SendSettings.SignalXOnPeriodicallyDefault;
				this.settingsInEdit.Terminal.IO.SerialPort.OutputBufferSize       = MKY.IO.Serial.SerialPort.SerialPortSettings.OutputBufferSizeDefault;
				this.settingsInEdit.Terminal.IO.SerialPort.BufferMaxBaudRate      = MKY.IO.Serial.SerialPort.SerialPortSettings.BufferMaxBaudRateDefault;
				this.settingsInEdit.Terminal.IO.SerialPort.MaxChunkSize           = MKY.IO.Serial.SerialPort.SerialPortSettings.MaxChunkSizeDefault;
				this.settingsInEdit.Terminal.IO.SerialPort.MaxSendRate            = MKY.IO.Serial.SerialPort.SerialPortSettings.MaxSendRateDefault;
				this.settingsInEdit.Terminal.IO.SerialPort.NoSendOnOutputBreak    = MKY.IO.Serial.SerialPort.SerialPortSettings.NoSendOnOutputBreakDefault;
				this.settingsInEdit.Terminal.IO.SerialPort.NoSendOnInputBreak     = MKY.IO.Serial.SerialPort.SerialPortSettings.NoSendOnInputBreakDefault;

				this.settingsInEdit.Terminal.Send.Text.EnableEscapes              = Domain.Settings.SendSettingsText.EnableEscapesDefault;
				this.settingsInEdit.Terminal.Send.File.EnableEscapes              = Domain.Settings.SendSettingsFile.EnableEscapesDefault;

				this.settingsInEdit.Terminal.Send.DefaultDelay                    = Domain.Settings.SendSettings.DefaultDelayDefault;
				this.settingsInEdit.Terminal.Send.DefaultLineDelay                = Domain.Settings.SendSettings.DefaultLineDelayDefault;
				this.settingsInEdit.Terminal.Send.DefaultLineInterval             = Domain.Settings.SendSettings.DefaultLineIntervalDefault;
				this.settingsInEdit.Terminal.Send.DefaultLineRepeat               = Domain.Settings.SendSettings.DefaultLineRepeatDefault;

				// User:
				this.settingsInEdit.UserName = TerminalExplicitSettings.UserNameDefault;

				// Update dependent settings:
				this.settingsInEdit.Terminal.UpdateAllDependentSettings();
			}
			finally
			{
				this.settingsInEdit.ResumeChangeEvent(true); // Force event.
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
