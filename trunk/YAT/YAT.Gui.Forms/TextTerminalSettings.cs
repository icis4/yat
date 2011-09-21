﻿//==================================================================================================
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
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.ComponentModel;
using System.Windows.Forms;

using MKY.Text;
using MKY.Windows.Forms;

using YAT.Gui.Utilities;

namespace YAT.Gui.Forms
{
	/// <summary></summary>
	public partial class TextTerminalSettings : System.Windows.Forms.Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isSettingControls = false;

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

			this.settings = settings;
			this.settings_Form = new Domain.Settings.TextTerminalSettings(settings);
			InitializeControls();
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
		private void TextTerminalSettings_Paint(object sender, PaintEventArgs e)
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

		private void comboBox_Encoding_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.Encoding = (EncodingEx)comboBox_Encoding.SelectedItem;
		}

		private void checkBox_SeparateTxRxEol_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				this.settings_Form.SeparateTxRxEol = checkBox_SeparateTxRxEol.Checked;
				SetControls();
			}
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

				SetControls();
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
			if (!this.isSettingControls)
			{
				Domain.EolEx eol = comboBox_RxEol.SelectedItem as Domain.EolEx;

				if (eol != null)
					this.settings_Form.RxEol = eol.ToSequenceString();
				else
					this.settings_Form.RxEol = comboBox_RxEol.Text;

				SetControls();
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
					{
						this.settings_Form.RxEol = eolString;
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
			if (!this.isSettingControls)
			{
				this.settings_Form.ShowEol = checkBox_ShowEol.Checked;
				SetControls();
			}
		}

		private void checkBox_Delay_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				Domain.TextLineSendDelay lsd = this.settings_Form.LineSendDelay;
				lsd.Enabled = checkBox_Delay.Checked;
				this.settings_Form.LineSendDelay = lsd;
				SetControls();
			}
		}

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
			if (!this.isSettingControls)
			{
				int interval;
				if (int.TryParse(textBox_DelayInterval.Text, out interval) || (interval >= 1))
				{
					Domain.TextLineSendDelay lsd = this.settings_Form.LineSendDelay;
					lsd.LineInterval = interval;
					this.settings_Form.LineSendDelay = lsd;
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
			if (!this.isSettingControls)
			{
				Domain.WaitForResponse wfr = this.settings_Form.WaitForResponse;
				wfr.Enabled = checkBox_WaitForResponse.Checked;
				this.settings_Form.WaitForResponse = wfr;
				SetControls();
			}
		}

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

		private void checkBox_SkipComments_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				this.settings_Form.SkipComments = checkBox_SkipComments.Checked;
				SetControls();
			}
		}

		private void stringListEdit_CommentMarkers_StringListChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.CommentMarkers = (string[])stringListEdit_CommentMarkers.StringList.Clone();
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
				this.settings_Form.SetDefaults();
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

			comboBox_TxEol.Items.Clear();
			comboBox_TxEol.Items.AddRange(Domain.EolEx.GetItems());

			comboBox_RxEol.Items.Clear();
			comboBox_RxEol.Items.AddRange(Domain.EolEx.GetItems());

			comboBox_Encoding.Items.Clear();
			comboBox_Encoding.Items.AddRange(EncodingEx.GetItems());

			this.isSettingControls = false;
		}

		private void SetControls()
		{
			this.isSettingControls = true;

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
			textBox_Delay.Text = this.settings_Form.LineSendDelay.Delay.ToString();
			textBox_DelayInterval.Enabled = delayEnabled;
			textBox_DelayInterval.Text = this.settings_Form.LineSendDelay.LineInterval.ToString();

			bool waitEnabled = this.settings_Form.WaitForResponse.Enabled;
			checkBox_WaitForResponse.Checked = waitEnabled;
			textBox_WaitForResponse.Enabled = waitEnabled;
			textBox_WaitForResponse.Text = this.settings_Form.WaitForResponse.Timeout.ToString();

			switch (this.settings_Form.CharSubstitution)
			{
				case Domain.CharSubstitution.ToUpper: radioButton_SubstituteToUpper.Checked = true; break;
				case Domain.CharSubstitution.ToLower: radioButton_SubstituteToLower.Checked = true; break;
				default:                              radioButton_SubstituteNone.Checked    = true; break;
			}

			stringListEdit_CommentMarkers.Enabled = this.settings_Form.SkipComments;
			stringListEdit_CommentMarkers.StringList = (string[])this.settings_Form.CommentMarkers.Clone();

			this.isSettingControls = false;
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
