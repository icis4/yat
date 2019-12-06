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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using MKY;
using MKY.ComponentModel;
using MKY.Text;
using MKY.Text.RegularExpressions;
using MKY.Windows.Forms;

#endregion

#region Module-level FxCop suppressions
//==================================================================================================
// Module-level FxCop suppressions
//==================================================================================================

[module: SuppressMessage("Microsoft.Naming", "CA1701:ResourceStringCompoundWordsShouldBeCasedCorrectly", Scope = "resource", Target = "YAT.View.Forms.TextTerminalSettings.resources", MessageId = "whitespace", Justification = "'whitespace' is a common term in programming.")]

#endregion

namespace YAT.View.Forms
{
	/// <summary></summary>
	public partial class TextTerminalSettings : Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;

		private Domain.Settings.TextTerminalSettings settings;
		private Domain.Settings.TextTerminalSettings settingsInEdit;

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
		////SetControls() is initially called in the 'Shown' event handler.
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
			this.settingsInEdit = new Domain.Settings.TextTerminalSettings(settings); // Clone to ensure decoupling.
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
		private void TextTerminalSettings_Shown(object sender, EventArgs e)
		{
			SetControls();
		}

		private void TextTerminalSettings_Deactivate(object sender, EventArgs e)
		{
			comboBox_Encoding.OnFormDeactivateWorkaround();
			comboBox_TxEol   .OnFormDeactivateWorkaround();
			comboBox_RxEol   .OnFormDeactivateWorkaround();
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void comboBox_Encoding_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Encoding = (EncodingEx)comboBox_Encoding.SelectedItem;
		}

		private void checkBox_SeparateTxRxEol_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.SeparateTxRxEol = checkBox_SeparateTxRxEol.Checked;
		}

	////private void comboBox_TxEol_SelectedIndexChanged(object sender, EventArgs e)
	////is not required since   "  _Validating() below gets called anyway.

		[ModalBehaviorContract(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void comboBox_TxEol_Validating(object sender, CancelEventArgs e)
		{
			string eolString = comboBox_TxEol.Text;

			Domain.EolEx eol;
			if (Domain.EolEx.TryParse(eolString, out eol))
			{
				this.settingsInEdit.TxEol = eol.ToSequenceString();

				if (!this.settingsInEdit.SeparateTxRxEol)
					this.settingsInEdit.RxEol = this.settingsInEdit.TxEol;
			}
			else
			{
				string description;
				if (!this.settingsInEdit.SeparateTxRxEol)
					description = "EOL";
				else
					description = "Tx EOL";

				int invalidTextStart;
				int invalidTextLength;
				if (Utilities.ValidationHelper.ValidateText(this, description, eolString, out invalidTextStart, out invalidTextLength, Domain.Parser.Modes.RadixAndAsciiEscapes))
				{
					if (!this.isSettingControls)
					{
						this.settingsInEdit.TxEol = eolString;

						if (!this.settingsInEdit.SeparateTxRxEol)
							this.settingsInEdit.RxEol = this.settingsInEdit.TxEol;
					}
				}
				else
				{
					comboBox_TxEol.Select(invalidTextStart, invalidTextLength);
					e.Cancel = true;
				}
			}
		}

	////private void comboBox_RxEol_SelectedIndexChanged(object sender, EventArgs e)
	////is not required since   "  _Validating() below gets called anyway.

		[ModalBehaviorContract(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void comboBox_RxEol_Validating(object sender, CancelEventArgs e)
		{
			string eolString = comboBox_RxEol.Text;

			Domain.EolEx eol;
			if (Domain.EolEx.TryParse(eolString, out eol))
			{
				this.settingsInEdit.RxEol = eol.ToSequenceString();
			}
			else
			{
				string description = "Rx EOL";
				int invalidTextStart;
				int invalidTextLength;
				if (Utilities.ValidationHelper.ValidateText(this, description, eolString, out invalidTextStart, out invalidTextLength, Domain.Parser.Modes.RadixAndAsciiEscapes))
				{
					if (!this.isSettingControls)
						this.settingsInEdit.RxEol = eolString;
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
			if (this.isSettingControls)
				return;

			this.settingsInEdit.ShowEol = checkBox_ShowEol.Checked;
		}

		private void textTerminalSettingsSet_Tx_SettingsChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.TxDisplay = textTerminalSettingsSet_Tx.Settings;
		}

		private void checkBox_SeparateTxRxDisplay_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.SeparateTxRxDisplay = checkBox_SeparateTxRxDisplay.Checked;
		}

		private void textTerminalSettingsSet_Rx_SettingsChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.RxDisplay = textTerminalSettingsSet_Rx.Settings;
		}

		private void checkBox_Delay_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var lsd = this.settingsInEdit.LineSendDelay;
			lsd.Enabled = checkBox_Delay.Checked;
			this.settingsInEdit.LineSendDelay = lsd; // Settings member must be changed to let the changed event be raised!
		}

		[ModalBehaviorContract(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void textBox_Delay_Validating(object sender, CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			int delay;
			if (int.TryParse(textBox_Delay.Text, out delay) && (delay >= 1))
			{
				var lsd = this.settingsInEdit.LineSendDelay;
				lsd.Delay = delay;
				this.settingsInEdit.LineSendDelay = lsd; // Settings member must be changed to let the changed event be raised!
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

		private void textBox_DelayInterval_TextChanged(object sender, EventArgs e)
		{
			int interval;
			if (int.TryParse(textBox_DelayInterval.Text, out interval) && (Math.Abs(interval) == 1))
				label_DelayIntervalUnit.Text = "line";
			else
				label_DelayIntervalUnit.Text = "lines";
		}

		[ModalBehaviorContract(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void textBox_DelayInterval_Validating(object sender, CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			int interval;
			if (int.TryParse(textBox_DelayInterval.Text, out interval) || (interval >= 1))
			{
				var lsd = this.settingsInEdit.LineSendDelay;
				lsd.LineInterval = interval;
				this.settingsInEdit.LineSendDelay = lsd; // Settings member must be changed to let the changed event be raised!
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

		private void checkBox_WaitForResponse_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var wfr = this.settingsInEdit.WaitForResponse;
			wfr.Enabled = checkBox_WaitForResponse.Checked;
			this.settingsInEdit.WaitForResponse = wfr; // Settings member must be changed to let the changed event be raised!
		}

		private void textBox_WaitForResponseOf_TextChanged(object sender, EventArgs e)
		{
			int interval;
			if (int.TryParse(textBox_WaitForResponseOf.Text, out interval) && (Math.Abs(interval) == 1))
				label_WaitForResponseOf.Text = "line before sending the";
			else
				label_WaitForResponseOf.Text = "lines before sending the";
		}

		[ModalBehaviorContract(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void textBox_WaitForResponseOf_Validating(object sender, CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			int count;
			if (int.TryParse(textBox_WaitForResponseOf.Text, out count) || (count >= 1))
			{
				var wfr = this.settingsInEdit.WaitForResponse;
				wfr.ResponseLineCount = count;
				this.settingsInEdit.WaitForResponse = wfr; // Settings member must be changed to let the changed event be raised!
			}
			else
			{
				MessageBoxEx.Show
				(
					this,
					"Line count must be at least 1 line!",
					"Invalid Input",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);

				e.Cancel = true;
			}
		}

		private void textBox_WaitForResponseNext_TextChanged(object sender, EventArgs e)
		{
			int interval;
			if (int.TryParse(textBox_WaitForResponseNext.Text, out interval) && (Math.Abs(interval) == 1))
				label_WaitForResponseNextUnit.Text = "line;";
			else
				label_WaitForResponseNextUnit.Text = "lines;";
		}

		[ModalBehaviorContract(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void textBox_WaitForResponseNext_Validating(object sender, CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			int count;
			if (int.TryParse(textBox_WaitForResponseNext.Text, out count) || (count >= 1))
			{
				var wfr = this.settingsInEdit.WaitForResponse;
				wfr.ClearanceLineCount = count;
				this.settingsInEdit.WaitForResponse = wfr; // Settings member must be changed to let the changed event be raised!
			}
			else
			{
				MessageBoxEx.Show
				(
					this,
					"Line count must be at least 1 line!",
					"Invalid Input",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);

				e.Cancel = true;
			}
		}

		[ModalBehaviorContract(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void textBox_WaitForResponseTimeout_Validating(object sender, CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			int timeout;
			if (int.TryParse(textBox_WaitForResponseTimeout.Text, out timeout) && (timeout >= 1))
			{
				var wfr = this.settingsInEdit.WaitForResponse;
				wfr.Timeout = timeout;
				this.settingsInEdit.WaitForResponse = wfr; // Settings member must be changed to let the changed event be raised!
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

		private void radioButton_SubstituteNone_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			if (radioButton_SubstituteNone.Checked)
				this.settingsInEdit.CharSubstitution = Domain.CharSubstitution.None;
		}

		private void radioButton_SubstituteToUpper_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			if (radioButton_SubstituteToUpper.Checked)
				this.settingsInEdit.CharSubstitution = Domain.CharSubstitution.ToUpper;
		}

		private void radioButton_SubstituteToLower_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			if (radioButton_SubstituteToLower.Checked)
				this.settingsInEdit.CharSubstitution = Domain.CharSubstitution.ToLower;
		}

		private void checkBox_Exclude_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.TextExclusion.Enabled = checkBox_Exclude.Checked;
		}

		private void stringListEdit_ExcludePatterns_Validating(object sender, StringCancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			string errorMessage;
			if (!RegexEx.TryValidatePattern(e.Value, out errorMessage))
			{
				MessageBoxEx.Show
				(
					this,
					errorMessage,
					"Invalid Regex",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
				e.Cancel = true;
			}
		}

		private void stringListEdit_ExcludePatterns_ListChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.TextExclusion.Patterns = new List<string>(stringListEdit_ExcludePatterns.StringList);
		}

		private void linkLabel_Regex_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			LinkHelper.TryBrowseUriAndShowErrorIfItFails(Parent, e);
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
				this.settingsInEdit.SetDefaults();
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
				comboBox_TxEol.Items.Clear();
				comboBox_TxEol.Items.AddRange(Domain.EolEx.GetItems());

				comboBox_RxEol.Items.Clear();
				comboBox_RxEol.Items.AddRange(Domain.EolEx.GetItems());

				comboBox_Encoding.Items.Clear();
				comboBox_Encoding.Items.AddRange(EncodingEx.GetItems());

				var linkText = ".NET Regex Quick Reference";
				var linkUri = @"https://docs.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference";
				linkLabel_Regex.Links.Clear();
				linkLabel_Regex.Text = linkText;
				linkLabel_Regex.Links.Add(0, linkText.Length, linkUri);
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void SetControls()
		{
			this.isSettingControls.Enter();
			try
			{
				// Encoding:
				comboBox_Encoding.SelectedItem = (EncodingEx)this.settingsInEdit.Encoding;

				// EOL:
				bool separateEol = this.settingsInEdit.SeparateTxRxEol;
				if (!separateEol)
				{
					label_TxEol.Text = "E&OL sequence:";
					label_RxEol.Text = "EOL sequence:";
				}
				else
				{
					label_TxEol.Text = "&Tx EOL sequence:";
					label_RxEol.Text = "&Rx EOL sequence:";
				}

				checkBox_SeparateTxRxEol.Checked = separateEol;
				label_RxEol.Enabled              = separateEol;
				comboBox_RxEol.Enabled           = separateEol;

				Domain.EolEx eol;

				if (Domain.EolEx.TryParse(this.settingsInEdit.TxEol, out eol))
					comboBox_TxEol.SelectedItem = eol;
				else
					comboBox_TxEol.Text = this.settingsInEdit.TxEol;

				if (Domain.EolEx.TryParse(this.settingsInEdit.RxEol, out eol))
					comboBox_RxEol.SelectedItem = eol;
				else
					comboBox_RxEol.Text = this.settingsInEdit.RxEol;

				checkBox_ShowEol.Checked = this.settingsInEdit.ShowEol;

				// Display:
				if (this.settingsInEdit.SeparateTxRxDisplay)
					groupBox_TxDisplay.Text = "&Tx";
				else
					groupBox_TxDisplay.Text = "&Tx and Rx";

				textTerminalSettingsSet_Tx.Settings  = this.settingsInEdit.TxDisplay;

				checkBox_SeparateTxRxDisplay.Checked = this.settingsInEdit.SeparateTxRxDisplay;
				groupBox_RxDisplay.Enabled           = this.settingsInEdit.SeparateTxRxDisplay;
				textTerminalSettingsSet_Rx.Settings  = this.settingsInEdit.RxDisplay;

				// Send:
				bool delayEnabled             = this.settingsInEdit.LineSendDelay.Enabled;
				checkBox_Delay.Checked        = delayEnabled;
				textBox_Delay.Enabled         = delayEnabled;
				textBox_Delay.Text            = this.settingsInEdit.LineSendDelay.Delay.ToString(CultureInfo.CurrentCulture);
				textBox_DelayInterval.Enabled = delayEnabled;
				textBox_DelayInterval.Text    = this.settingsInEdit.LineSendDelay.LineInterval.ToString(CultureInfo.CurrentCulture);

				bool waitEnabled                       = this.settingsInEdit.WaitForResponse.Enabled;
				checkBox_WaitForResponse.Checked       = waitEnabled;
				textBox_WaitForResponseOf.Enabled      = waitEnabled;
				textBox_WaitForResponseOf.Text         = this.settingsInEdit.WaitForResponse.ResponseLineCount.ToString(CultureInfo.CurrentCulture);
				textBox_WaitForResponseNext.Enabled    = waitEnabled;
				textBox_WaitForResponseNext.Text       = this.settingsInEdit.WaitForResponse.ClearanceLineCount.ToString(CultureInfo.CurrentCulture);
				textBox_WaitForResponseTimeout.Enabled = waitEnabled;
				textBox_WaitForResponseTimeout.Text    = this.settingsInEdit.WaitForResponse.Timeout.ToString(CultureInfo.CurrentCulture);

				switch (this.settingsInEdit.CharSubstitution)
				{
					case Domain.CharSubstitution.ToUpper: radioButton_SubstituteToUpper.Checked = true; break;
					case Domain.CharSubstitution.ToLower: radioButton_SubstituteToLower.Checked = true; break;
					default:                              radioButton_SubstituteNone.Checked    = true; break;
				}

				bool enabled = this.settingsInEdit.TextExclusion.Enabled;
				checkBox_Exclude.Checked                  = enabled;
				stringListEdit_ExcludePatterns.Enabled    = enabled;
				stringListEdit_ExcludePatterns.StringList = this.settingsInEdit.TextExclusion.Patterns.ToArray();
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
