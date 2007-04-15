using System;
using System.Collections.Generic;
using System.Text;

namespace HSR.YAT.Gui.Forms
{
	partial class TextTerminalSettings
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.button_OK = new System.Windows.Forms.Button();
			this.button_Cancel = new System.Windows.Forms.Button();
			this.groupBox_Settings = new System.Windows.Forms.GroupBox();
			this.groupBox_SendSettings = new System.Windows.Forms.GroupBox();
			this.groupBox_Substitute = new System.Windows.Forms.GroupBox();
			this.radioButton_SubstituteToLower = new System.Windows.Forms.RadioButton();
			this.radioButton_SubstituteToUpper = new System.Windows.Forms.RadioButton();
			this.radioButton_SubstituteNone = new System.Windows.Forms.RadioButton();
			this.checkBox_WaitForResponse = new System.Windows.Forms.CheckBox();
			this.textBox_DelayInterval = new System.Windows.Forms.TextBox();
			this.label_DelayUnit = new System.Windows.Forms.Label();
			this.textBox_Delay = new System.Windows.Forms.TextBox();
			this.label_DelayIntervalUnit = new System.Windows.Forms.Label();
			this.checkBox_Delay = new System.Windows.Forms.CheckBox();
			this.label_Encoding = new System.Windows.Forms.Label();
			this.comboBox_Encoding = new System.Windows.Forms.ComboBox();
			this.checkBox_SeparateTxRxEol = new System.Windows.Forms.CheckBox();
			this.comboBox_RxEol = new System.Windows.Forms.ComboBox();
			this.label_RxEol = new System.Windows.Forms.Label();
			this.comboBox_TxEol = new System.Windows.Forms.ComboBox();
			this.label_TxEol = new System.Windows.Forms.Label();
			this.groupBox_DisplaySettings = new System.Windows.Forms.GroupBox();
			this.checkBox_DirectionLineBreak = new System.Windows.Forms.CheckBox();
			this.comboBox_ControlCharacterRadix = new System.Windows.Forms.ComboBox();
			this.label_ControlCharacterRadix = new System.Windows.Forms.Label();
			this.checkBox_ShowEol = new System.Windows.Forms.CheckBox();
			this.checkBox_ReplaceControlCharacters = new System.Windows.Forms.CheckBox();
			this.button_Defaults = new System.Windows.Forms.Button();
			this.label_WaitForResponse = new System.Windows.Forms.Label();
			this.textBox_WaitForResponse = new System.Windows.Forms.TextBox();
			this.label_WaitForResponseUnit = new System.Windows.Forms.Label();
			this.groupBox_Settings.SuspendLayout();
			this.groupBox_SendSettings.SuspendLayout();
			this.groupBox_Substitute.SuspendLayout();
			this.groupBox_DisplaySettings.SuspendLayout();
			this.SuspendLayout();
			// 
			// button_OK
			// 
			this.button_OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button_OK.Location = new System.Drawing.Point(312, 26);
			this.button_OK.Name = "button_OK";
			this.button_OK.Size = new System.Drawing.Size(75, 23);
			this.button_OK.TabIndex = 1;
			this.button_OK.Text = "OK";
			this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
			// 
			// button_Cancel
			// 
			this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button_Cancel.Location = new System.Drawing.Point(312, 55);
			this.button_Cancel.Name = "button_Cancel";
			this.button_Cancel.Size = new System.Drawing.Size(75, 23);
			this.button_Cancel.TabIndex = 2;
			this.button_Cancel.Text = "Cancel";
			this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
			// 
			// groupBox_Settings
			// 
			this.groupBox_Settings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Settings.Controls.Add(this.groupBox_SendSettings);
			this.groupBox_Settings.Controls.Add(this.label_Encoding);
			this.groupBox_Settings.Controls.Add(this.comboBox_Encoding);
			this.groupBox_Settings.Controls.Add(this.checkBox_SeparateTxRxEol);
			this.groupBox_Settings.Controls.Add(this.comboBox_RxEol);
			this.groupBox_Settings.Controls.Add(this.label_RxEol);
			this.groupBox_Settings.Controls.Add(this.comboBox_TxEol);
			this.groupBox_Settings.Controls.Add(this.label_TxEol);
			this.groupBox_Settings.Controls.Add(this.groupBox_DisplaySettings);
			this.groupBox_Settings.Location = new System.Drawing.Point(12, 12);
			this.groupBox_Settings.Name = "groupBox_Settings";
			this.groupBox_Settings.Size = new System.Drawing.Size(284, 457);
			this.groupBox_Settings.TabIndex = 0;
			this.groupBox_Settings.TabStop = false;
			// 
			// groupBox_SendSettings
			// 
			this.groupBox_SendSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_SendSettings.Controls.Add(this.groupBox_Substitute);
			this.groupBox_SendSettings.Controls.Add(this.checkBox_WaitForResponse);
			this.groupBox_SendSettings.Controls.Add(this.textBox_DelayInterval);
			this.groupBox_SendSettings.Controls.Add(this.label_WaitForResponse);
			this.groupBox_SendSettings.Controls.Add(this.label_WaitForResponseUnit);
			this.groupBox_SendSettings.Controls.Add(this.label_DelayUnit);
			this.groupBox_SendSettings.Controls.Add(this.textBox_WaitForResponse);
			this.groupBox_SendSettings.Controls.Add(this.textBox_Delay);
			this.groupBox_SendSettings.Controls.Add(this.label_DelayIntervalUnit);
			this.groupBox_SendSettings.Controls.Add(this.checkBox_Delay);
			this.groupBox_SendSettings.Location = new System.Drawing.Point(6, 265);
			this.groupBox_SendSettings.Name = "groupBox_SendSettings";
			this.groupBox_SendSettings.Size = new System.Drawing.Size(272, 185);
			this.groupBox_SendSettings.TabIndex = 8;
			this.groupBox_SendSettings.TabStop = false;
			this.groupBox_SendSettings.Text = "Transmit Settings";
			// 
			// groupBox_Substitute
			// 
			this.groupBox_Substitute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox_Substitute.Controls.Add(this.radioButton_SubstituteToLower);
			this.groupBox_Substitute.Controls.Add(this.radioButton_SubstituteToUpper);
			this.groupBox_Substitute.Controls.Add(this.radioButton_SubstituteNone);
			this.groupBox_Substitute.Location = new System.Drawing.Point(6, 91);
			this.groupBox_Substitute.Name = "groupBox_Substitute";
			this.groupBox_Substitute.Size = new System.Drawing.Size(260, 88);
			this.groupBox_Substitute.TabIndex = 9;
			this.groupBox_Substitute.TabStop = false;
			this.groupBox_Substitute.Text = "Substitute Characters";
			// 
			// radioButton_SubstituteToLower
			// 
			this.radioButton_SubstituteToLower.AutoSize = true;
			this.radioButton_SubstituteToLower.Location = new System.Drawing.Point(10, 65);
			this.radioButton_SubstituteToLower.Name = "radioButton_SubstituteToLower";
			this.radioButton_SubstituteToLower.Size = new System.Drawing.Size(172, 17);
			this.radioButton_SubstituteToLower.TabIndex = 2;
			this.radioButton_SubstituteToLower.Text = "&Lower case (e.g. \"Rst\" -> \"rst\")";
			this.radioButton_SubstituteToLower.CheckedChanged += new System.EventHandler(this.radioButton_SubstituteToLower_CheckedChanged);
			// 
			// radioButton_SubstituteToUpper
			// 
			this.radioButton_SubstituteToUpper.AutoSize = true;
			this.radioButton_SubstituteToUpper.Location = new System.Drawing.Point(10, 42);
			this.radioButton_SubstituteToUpper.Name = "radioButton_SubstituteToUpper";
			this.radioButton_SubstituteToUpper.Size = new System.Drawing.Size(183, 17);
			this.radioButton_SubstituteToUpper.TabIndex = 1;
			this.radioButton_SubstituteToUpper.Text = "&Upper case (e.g. \"Rst\" -> \"RST\")";
			this.radioButton_SubstituteToUpper.CheckedChanged += new System.EventHandler(this.radioButton_SubstituteToUpper_CheckedChanged);
			// 
			// radioButton_SubstituteNone
			// 
			this.radioButton_SubstituteNone.AutoSize = true;
			this.radioButton_SubstituteNone.Location = new System.Drawing.Point(10, 19);
			this.radioButton_SubstituteNone.Name = "radioButton_SubstituteNone";
			this.radioButton_SubstituteNone.Size = new System.Drawing.Size(143, 17);
			this.radioButton_SubstituteNone.TabIndex = 0;
			this.radioButton_SubstituteNone.Text = "&None (e.g. \"Rst -> \"Rst\")";
			this.radioButton_SubstituteNone.CheckedChanged += new System.EventHandler(this.radioButton_SubstituteNone_CheckedChanged);
			// 
			// checkBox_WaitForResponse
			// 
			this.checkBox_WaitForResponse.AutoSize = true;
			this.checkBox_WaitForResponse.Location = new System.Drawing.Point(12, 45);
			this.checkBox_WaitForResponse.Name = "checkBox_WaitForResponse";
			this.checkBox_WaitForResponse.Size = new System.Drawing.Size(242, 17);
			this.checkBox_WaitForResponse.TabIndex = 5;
			this.checkBox_WaitForResponse.Text = "&Wait for response before sending the next line";
			this.checkBox_WaitForResponse.UseVisualStyleBackColor = true;
			this.checkBox_WaitForResponse.CheckedChanged += new System.EventHandler(this.checkBox_WaitForResponse_CheckedChanged);
			// 
			// textBox_DelayInterval
			// 
			this.textBox_DelayInterval.Location = new System.Drawing.Point(195, 19);
			this.textBox_DelayInterval.Name = "textBox_DelayInterval";
			this.textBox_DelayInterval.Size = new System.Drawing.Size(40, 20);
			this.textBox_DelayInterval.TabIndex = 3;
			this.textBox_DelayInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBox_DelayInterval.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_DelayInterval_Validating);
			this.textBox_DelayInterval.TextChanged += new System.EventHandler(this.textBox_DelayInterval_TextChanged);
			// 
			// label_DelayUnit
			// 
			this.label_DelayUnit.AutoSize = true;
			this.label_DelayUnit.Location = new System.Drawing.Point(148, 22);
			this.label_DelayUnit.Name = "label_DelayUnit";
			this.label_DelayUnit.Size = new System.Drawing.Size(47, 13);
			this.label_DelayUnit.TabIndex = 2;
			this.label_DelayUnit.Text = "ms each";
			this.label_DelayUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox_Delay
			// 
			this.textBox_Delay.Location = new System.Drawing.Point(97, 19);
			this.textBox_Delay.Name = "textBox_Delay";
			this.textBox_Delay.Size = new System.Drawing.Size(51, 20);
			this.textBox_Delay.TabIndex = 1;
			this.textBox_Delay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBox_Delay.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_Delay_Validating);
			// 
			// label_DelayIntervalUnit
			// 
			this.label_DelayIntervalUnit.AutoSize = true;
			this.label_DelayIntervalUnit.Location = new System.Drawing.Point(235, 22);
			this.label_DelayIntervalUnit.Name = "label_DelayIntervalUnit";
			this.label_DelayIntervalUnit.Size = new System.Drawing.Size(23, 13);
			this.label_DelayIntervalUnit.TabIndex = 4;
			this.label_DelayIntervalUnit.Text = "line";
			this.label_DelayIntervalUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkBox_Delay
			// 
			this.checkBox_Delay.AutoSize = true;
			this.checkBox_Delay.Location = new System.Drawing.Point(12, 21);
			this.checkBox_Delay.Name = "checkBox_Delay";
			this.checkBox_Delay.Size = new System.Drawing.Size(85, 17);
			this.checkBox_Delay.TabIndex = 0;
			this.checkBox_Delay.Text = "Add &delay of";
			this.checkBox_Delay.CheckedChanged += new System.EventHandler(this.checkBox_Delay_CheckedChanged);
			// 
			// label_Encoding
			// 
			this.label_Encoding.AutoSize = true;
			this.label_Encoding.Location = new System.Drawing.Point(15, 97);
			this.label_Encoding.Name = "label_Encoding";
			this.label_Encoding.Size = new System.Drawing.Size(55, 13);
			this.label_Encoding.TabIndex = 5;
			this.label_Encoding.Text = "&Encoding:";
			// 
			// comboBox_Encoding
			// 
			this.comboBox_Encoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_Encoding.FormattingEnabled = true;
			this.comboBox_Encoding.Location = new System.Drawing.Point(18, 113);
			this.comboBox_Encoding.Name = "comboBox_Encoding";
			this.comboBox_Encoding.Size = new System.Drawing.Size(255, 21);
			this.comboBox_Encoding.TabIndex = 6;
			this.comboBox_Encoding.SelectedIndexChanged += new System.EventHandler(this.comboBox_Encoding_SelectedIndexChanged);
			// 
			// checkBox_SeparateTxRxEol
			// 
			this.checkBox_SeparateTxRxEol.AutoSize = true;
			this.checkBox_SeparateTxRxEol.Location = new System.Drawing.Point(18, 42);
			this.checkBox_SeparateTxRxEol.Name = "checkBox_SeparateTxRxEol";
			this.checkBox_SeparateTxRxEol.Size = new System.Drawing.Size(245, 17);
			this.checkBox_SeparateTxRxEol.TabIndex = 2;
			this.checkBox_SeparateTxRxEol.Text = "&Separate end of line sequencies for Tx and Rx";
			this.checkBox_SeparateTxRxEol.UseVisualStyleBackColor = true;
			this.checkBox_SeparateTxRxEol.CheckedChanged += new System.EventHandler(this.checkBox_SeparateTxRxEol_CheckedChanged);
			// 
			// comboBox_RxEol
			// 
			this.comboBox_RxEol.Enabled = false;
			this.comboBox_RxEol.Location = new System.Drawing.Point(152, 65);
			this.comboBox_RxEol.Name = "comboBox_RxEol";
			this.comboBox_RxEol.Size = new System.Drawing.Size(121, 21);
			this.comboBox_RxEol.TabIndex = 4;
			this.comboBox_RxEol.Validating += new System.ComponentModel.CancelEventHandler(this.comboBox_RxEol_Validating);
			this.comboBox_RxEol.SelectedIndexChanged += new System.EventHandler(this.comboBox_RxEol_SelectedIndexChanged);
			// 
			// label_RxEol
			// 
			this.label_RxEol.AutoSize = true;
			this.label_RxEol.Enabled = false;
			this.label_RxEol.Location = new System.Drawing.Point(15, 68);
			this.label_RxEol.Name = "label_RxEol";
			this.label_RxEol.Size = new System.Drawing.Size(125, 13);
			this.label_RxEol.TabIndex = 3;
			this.label_RxEol.Text = "&Rx end of line sequence:";
			// 
			// comboBox_TxEol
			// 
			this.comboBox_TxEol.Location = new System.Drawing.Point(152, 15);
			this.comboBox_TxEol.Name = "comboBox_TxEol";
			this.comboBox_TxEol.Size = new System.Drawing.Size(121, 21);
			this.comboBox_TxEol.TabIndex = 1;
			this.comboBox_TxEol.Validating += new System.ComponentModel.CancelEventHandler(this.comboBox_TxEol_Validating);
			this.comboBox_TxEol.SelectedIndexChanged += new System.EventHandler(this.comboBox_TxEol_SelectedIndexChanged);
			// 
			// label_TxEol
			// 
			this.label_TxEol.AutoSize = true;
			this.label_TxEol.Location = new System.Drawing.Point(15, 18);
			this.label_TxEol.Name = "label_TxEol";
			this.label_TxEol.Size = new System.Drawing.Size(110, 13);
			this.label_TxEol.TabIndex = 0;
			this.label_TxEol.Text = "End &of line sequence:";
			// 
			// groupBox_DisplaySettings
			// 
			this.groupBox_DisplaySettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_DisplaySettings.Controls.Add(this.checkBox_DirectionLineBreak);
			this.groupBox_DisplaySettings.Controls.Add(this.comboBox_ControlCharacterRadix);
			this.groupBox_DisplaySettings.Controls.Add(this.label_ControlCharacterRadix);
			this.groupBox_DisplaySettings.Controls.Add(this.checkBox_ShowEol);
			this.groupBox_DisplaySettings.Controls.Add(this.checkBox_ReplaceControlCharacters);
			this.groupBox_DisplaySettings.Location = new System.Drawing.Point(6, 142);
			this.groupBox_DisplaySettings.Name = "groupBox_DisplaySettings";
			this.groupBox_DisplaySettings.Size = new System.Drawing.Size(272, 117);
			this.groupBox_DisplaySettings.TabIndex = 7;
			this.groupBox_DisplaySettings.TabStop = false;
			this.groupBox_DisplaySettings.Text = "Display Settings";
			// 
			// checkBox_DirectionLineBreak
			// 
			this.checkBox_DirectionLineBreak.AutoSize = true;
			this.checkBox_DirectionLineBreak.Checked = true;
			this.checkBox_DirectionLineBreak.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBox_DirectionLineBreak.Location = new System.Drawing.Point(12, 21);
			this.checkBox_DirectionLineBreak.Name = "checkBox_DirectionLineBreak";
			this.checkBox_DirectionLineBreak.Size = new System.Drawing.Size(194, 17);
			this.checkBox_DirectionLineBreak.TabIndex = 4;
			this.checkBox_DirectionLineBreak.Text = "Break lines when direction changes";
			this.checkBox_DirectionLineBreak.CheckedChanged += new System.EventHandler(this.checkBox_DirectionLineBreak_CheckedChanged);
			// 
			// comboBox_ControlCharacterRadix
			// 
			this.comboBox_ControlCharacterRadix.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_ControlCharacterRadix.Location = new System.Drawing.Point(146, 90);
			this.comboBox_ControlCharacterRadix.Name = "comboBox_ControlCharacterRadix";
			this.comboBox_ControlCharacterRadix.Size = new System.Drawing.Size(121, 21);
			this.comboBox_ControlCharacterRadix.TabIndex = 3;
			this.comboBox_ControlCharacterRadix.SelectedIndexChanged += new System.EventHandler(this.comboBox_ControlCharacterRadix_SelectedIndexChanged);
			// 
			// label_ControlCharacterRadix
			// 
			this.label_ControlCharacterRadix.AutoSize = true;
			this.label_ControlCharacterRadix.Location = new System.Drawing.Point(9, 93);
			this.label_ControlCharacterRadix.Name = "label_ControlCharacterRadix";
			this.label_ControlCharacterRadix.Size = new System.Drawing.Size(116, 13);
			this.label_ControlCharacterRadix.TabIndex = 2;
			this.label_ControlCharacterRadix.Text = "Control character radix:";
			// 
			// checkBox_ShowEol
			// 
			this.checkBox_ShowEol.AutoSize = true;
			this.checkBox_ShowEol.Location = new System.Drawing.Point(12, 44);
			this.checkBox_ShowEol.Name = "checkBox_ShowEol";
			this.checkBox_ShowEol.Size = new System.Drawing.Size(155, 17);
			this.checkBox_ShowEol.TabIndex = 0;
			this.checkBox_ShowEol.Text = "&Show end of line sequence";
			this.checkBox_ShowEol.CheckedChanged += new System.EventHandler(this.checkBox_ShowEol_CheckedChanged);
			// 
			// checkBox_ReplaceControlCharacters
			// 
			this.checkBox_ReplaceControlCharacters.AutoSize = true;
			this.checkBox_ReplaceControlCharacters.Location = new System.Drawing.Point(12, 67);
			this.checkBox_ReplaceControlCharacters.Name = "checkBox_ReplaceControlCharacters";
			this.checkBox_ReplaceControlCharacters.Size = new System.Drawing.Size(218, 17);
			this.checkBox_ReplaceControlCharacters.TabIndex = 1;
			this.checkBox_ReplaceControlCharacters.Text = "&Replace control characters (0x00 - 0x1F)";
			this.checkBox_ReplaceControlCharacters.CheckedChanged += new System.EventHandler(this.checkBox_ReplaceControlCharacters_CheckedChanged);
			// 
			// button_Defaults
			// 
			this.button_Defaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Defaults.Location = new System.Drawing.Point(312, 123);
			this.button_Defaults.Name = "button_Defaults";
			this.button_Defaults.Size = new System.Drawing.Size(75, 23);
			this.button_Defaults.TabIndex = 3;
			this.button_Defaults.Text = "&Defaults...";
			this.button_Defaults.Click += new System.EventHandler(this.button_Defaults_Click);
			// 
			// label_WaitForResponse
			// 
			this.label_WaitForResponse.AutoSize = true;
			this.label_WaitForResponse.Location = new System.Drawing.Point(27, 65);
			this.label_WaitForResponse.Name = "label_WaitForResponse";
			this.label_WaitForResponse.Size = new System.Drawing.Size(84, 13);
			this.label_WaitForResponse.TabIndex = 6;
			this.label_WaitForResponse.Text = "with a timeout of";
			this.label_WaitForResponse.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox_WaitForResponse
			// 
			this.textBox_WaitForResponse.Location = new System.Drawing.Point(113, 62);
			this.textBox_WaitForResponse.Name = "textBox_WaitForResponse";
			this.textBox_WaitForResponse.Size = new System.Drawing.Size(51, 20);
			this.textBox_WaitForResponse.TabIndex = 7;
			this.textBox_WaitForResponse.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBox_WaitForResponse.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_WaitForResponse_Validating);
			// 
			// label_WaitForResponseUnit
			// 
			this.label_WaitForResponseUnit.AutoSize = true;
			this.label_WaitForResponseUnit.Location = new System.Drawing.Point(164, 65);
			this.label_WaitForResponseUnit.Name = "label_WaitForResponseUnit";
			this.label_WaitForResponseUnit.Size = new System.Drawing.Size(20, 13);
			this.label_WaitForResponseUnit.TabIndex = 8;
			this.label_WaitForResponseUnit.Text = "ms";
			this.label_WaitForResponseUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// TextTerminalSettings
			// 
			this.AcceptButton = this.button_OK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.button_Cancel;
			this.ClientSize = new System.Drawing.Size(399, 481);
			this.Controls.Add(this.button_Defaults);
			this.Controls.Add(this.groupBox_Settings);
			this.Controls.Add(this.button_Cancel);
			this.Controls.Add(this.button_OK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TextTerminalSettings";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Text Terminal Settings";
			this.Load += new System.EventHandler(this.TextTerminalSettings_Load);
			this.groupBox_Settings.ResumeLayout(false);
			this.groupBox_Settings.PerformLayout();
			this.groupBox_SendSettings.ResumeLayout(false);
			this.groupBox_SendSettings.PerformLayout();
			this.groupBox_Substitute.ResumeLayout(false);
			this.groupBox_Substitute.PerformLayout();
			this.groupBox_DisplaySettings.ResumeLayout(false);
			this.groupBox_DisplaySettings.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox_Settings;
		private System.Windows.Forms.GroupBox groupBox_DisplaySettings;
		private System.Windows.Forms.Label label_TxEol;
		private System.Windows.Forms.ComboBox comboBox_TxEol;
		private System.Windows.Forms.CheckBox checkBox_ReplaceControlCharacters;
		private System.Windows.Forms.Label label_ControlCharacterRadix;
		private System.Windows.Forms.ComboBox comboBox_ControlCharacterRadix;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.Button button_Cancel;
		private System.Windows.Forms.Button button_Defaults;
		private System.Windows.Forms.CheckBox checkBox_SeparateTxRxEol;
		private System.Windows.Forms.ComboBox comboBox_RxEol;
		private System.Windows.Forms.Label label_RxEol;
		private System.Windows.Forms.Label label_Encoding;
		private System.Windows.Forms.ComboBox comboBox_Encoding;
		private System.Windows.Forms.GroupBox groupBox_SendSettings;
		private System.Windows.Forms.GroupBox groupBox_Substitute;
		private System.Windows.Forms.RadioButton radioButton_SubstituteToLower;
		private System.Windows.Forms.RadioButton radioButton_SubstituteToUpper;
		private System.Windows.Forms.RadioButton radioButton_SubstituteNone;
		private System.Windows.Forms.CheckBox checkBox_WaitForResponse;
		private System.Windows.Forms.TextBox textBox_DelayInterval;
		private System.Windows.Forms.Label label_DelayUnit;
		private System.Windows.Forms.TextBox textBox_Delay;
		private System.Windows.Forms.Label label_DelayIntervalUnit;
		private System.Windows.Forms.CheckBox checkBox_Delay;
		private System.Windows.Forms.CheckBox checkBox_ShowEol;
		private System.Windows.Forms.CheckBox checkBox_DirectionLineBreak;
		private System.Windows.Forms.Label label_WaitForResponse;
		private System.Windows.Forms.Label label_WaitForResponseUnit;
		private System.Windows.Forms.TextBox textBox_WaitForResponse;
	}
}
