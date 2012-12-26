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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

using MKY.Text;
using MKY.Windows.Forms;

using YAT.Gui.Utilities;

#endregion

namespace YAT.Gui.Forms
{
	/// <summary></summary>
	public partial class TextTerminalSettings : System.Windows.Forms.Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;

		private Domain.Settings.TextTerminalSettings settings;
		private Domain.Settings.TextTerminalSettings settings_Form;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public TextTerminalSettings(Domain.Settings.TextTerminalSettings settings)
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
		public Domain.Settings.TextTerminalSettings SettingsResult
		{
			get { return (this.settings); }
		}

		#endregion

		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

		private void KeepAndCloneAndAttachSettings(Domain.Settings.TextTerminalSettings settings)
		{
			this.settings = settings;
			this.settings_Form = new Domain.Settings.TextTerminalSettings(settings);
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
		private void TextTerminalSettings_Shown(object sender, EventArgs e)
		{
			SetControls();
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void comboBox_Encoding_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.Encoding = (EncodingEx)comboBox_Encoding.SelectedItem;
		}

		private void checkBox_SeparateTxRxEol_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.SeparateTxRxEol = checkBox_SeparateTxRxEol.Checked;
		}

		private void comboBox_TxEol_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				Domain.EolEx eol = comboBox_TxEol.SelectedItem as Domain.EolEx;

				if (eol != null)
					this.settings_Form.TxEol = eol.ToSequenceString();
				else
					this.settings_Form.TxEol = comboBox_TxEol.Text;

				if (!this.settings_Form.SeparateTxRxEol)
					this.settings_Form.RxEol = this.settings_Form.TxEol;
			}
		}

		private void comboBox_TxEol_Validating(object sender, CancelEventArgs e)
		{
			Domain.EolEx eol;
			string eolString = comboBox_TxEol.Text;

			if (Domain.EolEx.TryParse(eolString, out eol))
			{
				this.settings_Form.TxEol = eol.ToSequenceString();

				if (!this.settings_Form.SeparateTxRxEol)
					this.settings_Form.RxEol = this.settings_Form.TxEol;
			}
			else
			{
				string description;
				if (!this.settings_Form.SeparateTxRxEol)
					description = "Tx EOL";
				else
					description = "EOL";

				int invalidTextStart;
				int invalidTextLength;
				if (Validation.ValidateSequence(this, description, eolString, out invalidTextStart, out invalidTextLength))
				{
					if (!this.isSettingControls)
					{
						this.settings_Form.TxEol = eolString;

						if (!this.settings_Form.SeparateTxRxEol)
							this.settings_Form.RxEol = this.settings_Form.TxEol;
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
			if (!this.isSettingControls)
			{
				Domain.EolEx eol = comboBox_RxEol.SelectedItem as Domain.EolEx;

				if (eol != null)
					this.settings_Form.RxEol = eol.ToSequenceString();
				else
					this.settings_Form.RxEol = comboBox_RxEol.Text;
			}
		}

		private void comboBox_RxEol_Validating(object sender, CancelEventArgs e)
		{
			Domain.EolEx eol;
			string eolString = comboBox_RxEol.Text;

			if (Domain.EolEx.TryParse(eolString, out eol))
			{
				this.settings_Form.RxEol = eol.ToSequenceString();
			}
			else
			{
				int invalidTextStart;
				int invalidTextLength;
				if (Validation.ValidateSequence(this, "Rx EOL", eolString, out invalidTextStart, out invalidTextLength))
				{
					if (!this.isSettingControls)
						this.settings_Form.RxEol = eolString;
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
			if (!this.isSettingControls)
				this.settings_Form.ShowEol = checkBox_ShowEol.Checked;
		}

		private void checkBox_Delay_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				Domain.TextLineSendDelay lsd = this.settings_Form.LineSendDelay;
				lsd.Enabled = checkBox_Delay.Checked;
				this.settings_Form.LineSendDelay = lsd;
			}
		}

		[ModalBehavior(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void textBox_Delay_Validating(object sender, CancelEventArgs e)
		{
			if (!this.isSettingControls)
			{
				int delay;
				if (int.TryParse(textBox_Delay.Text, out delay) && (delay >= 1))
				{
					Domain.TextLineSendDelay lsd = this.settings_Form.LineSendDelay;
					lsd.Delay = delay;
					this.settings_Form.LineSendDelay = lsd;
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

		private void textBox_DelayInterval_TextChanged(object sender, EventArgs e)
		{
			int interval;
			if (int.TryParse(textBox_DelayInterval.Text, out interval) && (Math.Abs(interval) == 1))
				label_DelayIntervalUnit.Text = "line";
			else
				label_DelayIntervalUnit.Text = "lines";
		}

		[ModalBehavior(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void textBox_DelayInterval_Validating(object sender, CancelEventArgs e)
		{
			if (!this.isSettingControls)
			{
				int interval;
				if (int.TryParse(textBox_DelayInterval.Text, out interval) || (interval >= 1))
				{
					Domain.TextLineSendDelay lsd = this.settings_Form.LineSendDelay;
					lsd.LineInterval = interval;
					this.settings_Form.LineSendDelay = lsd;
				}
				else
				{
					MessageBoxEx.Show
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
			if (!this.isSettingControls)
			{
				Domain.WaitForResponse wfr = this.settings_Form.WaitForResponse;
				wfr.Enabled = checkBox_WaitForResponse.Checked;
				this.settings_Form.WaitForResponse = wfr;
			}
		}

		[ModalBehavior(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void textBox_WaitForResponse_Validating(object sender, CancelEventArgs e)
		{
			if (!this.isSettingControls)
			{
				int timeout;
				if (int.TryParse(textBox_WaitForResponse.Text, out timeout) && (timeout >= 1))
				{
					Domain.WaitForResponse wfr = this.settings_Form.WaitForResponse;
					wfr.Timeout = timeout;
					this.settings_Form.WaitForResponse = wfr;
				}
				else
				{
					MessageBoxEx.Show
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
			if (!this.isSettingControls && radioButton_SubstituteNone.Checked)
				this.settings_Form.CharSubstitution = Domain.CharSubstitution.None;
		}

		private void radioButton_SubstituteToUpper_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls && radioButton_SubstituteToUpper.Checked)
				this.settings_Form.CharSubstitution = Domain.CharSubstitution.ToUpper;
		}

		private void radioButton_SubstituteToLower_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls && radioButton_SubstituteToLower.Checked)
				this.settings_Form.CharSubstitution = Domain.CharSubstitution.ToLower;
		}

		private void checkBox_SkipEolComment_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.EolComment.SkipComment = checkBox_SkipEolComment.Checked;
		}

		private void stringListEdit_EolCommentIndicators_StringListChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.EolComment.Indicators = new List<string>(stringListEdit_EolCommentIndicators.StringList);
		}

		private void checkBox_SkipEolCommentWhiteSpace_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.EolComment.SkipWhiteSpace = checkBox_SkipEolCommentWhiteSpace.Checked;
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
				this.settings_Form.SetDefaults();
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

			comboBox_TxEol.Items.Clear();
			comboBox_TxEol.Items.AddRange(Domain.EolEx.GetItems());

			comboBox_RxEol.Items.Clear();
			comboBox_RxEol.Items.AddRange(Domain.EolEx.GetItems());

			comboBox_Encoding.Items.Clear();
			comboBox_Encoding.Items.AddRange(EncodingEx.GetItems());

			this.isSettingControls.Leave();
		}

		private void SetControls()
		{
			this.isSettingControls.Enter();

			// Encoding.
			comboBox_Encoding.SelectedItem = (EncodingEx)this.settings_Form.Encoding;

			// EOL.
			bool separateEol = this.settings_Form.SeparateTxRxEol;
			if (!separateEol)
				label_TxEol.Text = "E&OL sequence:";
			else
				label_TxEol.Text = "&Tx EOL sequence:";
			checkBox_SeparateTxRxEol.Checked = separateEol;
			label_RxEol.Enabled = separateEol;
			comboBox_RxEol.Enabled = separateEol;

			Domain.EolEx eol;
			if (Domain.EolEx.TryParse(this.settings_Form.TxEol, out eol))
				comboBox_TxEol.SelectedItem = eol;
			else
				comboBox_TxEol.Text = this.settings_Form.TxEol;
			if (Domain.EolEx.TryParse(this.settings_Form.RxEol, out eol))
				comboBox_RxEol.SelectedItem = eol;
			else
				comboBox_RxEol.Text = this.settings_Form.RxEol;

			// Display.
			checkBox_ShowEol.Checked = this.settings_Form.ShowEol;

			// Transmit.
			bool delayEnabled = this.settings_Form.LineSendDelay.Enabled;
			checkBox_Delay.Checked = delayEnabled;
			textBox_Delay.Enabled = delayEnabled;
			textBox_Delay.Text = this.settings_Form.LineSendDelay.Delay.ToString(NumberFormatInfo.CurrentInfo);
			textBox_DelayInterval.Enabled = delayEnabled;
			textBox_DelayInterval.Text = this.settings_Form.LineSendDelay.LineInterval.ToString(NumberFormatInfo.CurrentInfo);

			bool waitEnabled = this.settings_Form.WaitForResponse.Enabled;
			checkBox_WaitForResponse.Checked = waitEnabled;
			textBox_WaitForResponse.Enabled = waitEnabled;
			textBox_WaitForResponse.Text = this.settings_Form.WaitForResponse.Timeout.ToString(NumberFormatInfo.CurrentInfo);

			switch (this.settings_Form.CharSubstitution)
			{
				case Domain.CharSubstitution.ToUpper: radioButton_SubstituteToUpper.Checked = true; break;
				case Domain.CharSubstitution.ToLower: radioButton_SubstituteToLower.Checked = true; break;
				default:                              radioButton_SubstituteNone.Checked    = true; break;
			}

			bool doSkip = this.settings_Form.EolComment.SkipComment;
			checkBox_SkipEolComment.Checked                = doSkip;
			stringListEdit_EolCommentIndicators.Enabled    = doSkip;
			stringListEdit_EolCommentIndicators.StringList = this.settings_Form.EolComment.Indicators.ToArray();
			checkBox_SkipEolCommentWhiteSpace.Enabled      = doSkip;
			checkBox_SkipEolCommentWhiteSpace.Checked      = this.settings_Form.EolComment.SkipWhiteSpace;

			this.isSettingControls.Leave();
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
