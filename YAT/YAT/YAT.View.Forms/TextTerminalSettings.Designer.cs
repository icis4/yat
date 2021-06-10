using System;
using System.Collections.Generic;
using System.Text;

namespace YAT.View.Forms
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextTerminalSettings));
			this.button_OK = new System.Windows.Forms.Button();
			this.button_Cancel = new System.Windows.Forms.Button();
			this.groupBox_Settings = new System.Windows.Forms.GroupBox();
			this.label_DecodingMismatchBehavior = new System.Windows.Forms.Label();
			this.comboBox_DecodingMismatchBehavior = new System.Windows.Forms.ComboBox();
			this.groupBox_Display = new System.Windows.Forms.GroupBox();
			this.groupBox_RxDisplay = new System.Windows.Forms.GroupBox();
			this.textTerminalSettingsSet_Rx = new YAT.View.Controls.TextDisplaySettingsSet();
			this.groupBox_TxDisplay = new System.Windows.Forms.GroupBox();
			this.textTerminalSettingsSet_Tx = new YAT.View.Controls.TextDisplaySettingsSet();
			this.checkBox_SeparateTxRxDisplay = new System.Windows.Forms.CheckBox();
			this.groupBox_Eol = new System.Windows.Forms.GroupBox();
			this.checkBox_ShowEol = new System.Windows.Forms.CheckBox();
			this.checkBox_SeparateTxRxEol = new System.Windows.Forms.CheckBox();
			this.comboBox_RxEol = new MKY.Windows.Forms.ComboBoxEx();
			this.label_RxEol = new System.Windows.Forms.Label();
			this.comboBox_TxEol = new MKY.Windows.Forms.ComboBoxEx();
			this.label_TxEol = new System.Windows.Forms.Label();
			this.comboBox_Encoding = new MKY.Windows.Forms.ComboBoxEx();
			this.groupBox_Send = new System.Windows.Forms.GroupBox();
			this.textBox_WaitForResponseTimeout = new MKY.Windows.Forms.TextBoxEx();
			this.label_WaitForResponseTimeout = new System.Windows.Forms.Label();
			this.textBox_WaitForResponseNext = new MKY.Windows.Forms.TextBoxEx();
			this.label_WaitForResponseNext = new System.Windows.Forms.Label();
			this.label_WaitForResponseOf = new System.Windows.Forms.Label();
			this.textBox_WaitForResponseOf = new MKY.Windows.Forms.TextBoxEx();
			this.groupBox_Exclude = new System.Windows.Forms.GroupBox();
			this.linkLabel_Regex = new System.Windows.Forms.LinkLabel();
			this.stringListEdit_ExcludePatterns = new MKY.Windows.Forms.StringListEdit();
			this.checkBox_Exclude = new System.Windows.Forms.CheckBox();
			this.groupBox_Substitute = new System.Windows.Forms.GroupBox();
			this.radioButton_SubstituteToLower = new System.Windows.Forms.RadioButton();
			this.radioButton_SubstituteToUpper = new System.Windows.Forms.RadioButton();
			this.radioButton_SubstituteNone = new System.Windows.Forms.RadioButton();
			this.checkBox_WaitForResponse = new System.Windows.Forms.CheckBox();
			this.textBox_DelayInterval = new MKY.Windows.Forms.TextBoxEx();
			this.label_WaitForResponseNextUnit = new System.Windows.Forms.Label();
			this.label_WaitForResponseTimeoutUnit = new System.Windows.Forms.Label();
			this.label_DelayUnit = new System.Windows.Forms.Label();
			this.textBox_Delay = new MKY.Windows.Forms.TextBoxEx();
			this.label_DelayIntervalUnit = new System.Windows.Forms.Label();
			this.checkBox_Delay = new System.Windows.Forms.CheckBox();
			this.label_Encoding = new System.Windows.Forms.Label();
			this.button_Defaults = new System.Windows.Forms.Button();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.label_AdvancedSettingsRemark = new System.Windows.Forms.Label();
			this.groupBox_Settings.SuspendLayout();
			this.groupBox_Display.SuspendLayout();
			this.groupBox_RxDisplay.SuspendLayout();
			this.groupBox_TxDisplay.SuspendLayout();
			this.groupBox_Eol.SuspendLayout();
			this.groupBox_Send.SuspendLayout();
			this.groupBox_Exclude.SuspendLayout();
			this.groupBox_Substitute.SuspendLayout();
			this.SuspendLayout();
			// 
			// button_OK
			// 
			this.button_OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button_OK.Location = new System.Drawing.Point(615, 42);
			this.button_OK.Name = "button_OK";
			this.button_OK.Size = new System.Drawing.Size(75, 23);
			this.button_OK.TabIndex = 1;
			this.button_OK.Text = "OK";
			this.button_OK.UseVisualStyleBackColor = true;
			this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
			// 
			// button_Cancel
			// 
			this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button_Cancel.Location = new System.Drawing.Point(615, 71);
			this.button_Cancel.Name = "button_Cancel";
			this.button_Cancel.Size = new System.Drawing.Size(75, 23);
			this.button_Cancel.TabIndex = 2;
			this.button_Cancel.Text = "Cancel";
			this.button_Cancel.UseVisualStyleBackColor = true;
			this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
			// 
			// groupBox_Settings
			// 
			this.groupBox_Settings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Settings.Controls.Add(this.label_DecodingMismatchBehavior);
			this.groupBox_Settings.Controls.Add(this.comboBox_DecodingMismatchBehavior);
			this.groupBox_Settings.Controls.Add(this.groupBox_Display);
			this.groupBox_Settings.Controls.Add(this.groupBox_Eol);
			this.groupBox_Settings.Controls.Add(this.comboBox_Encoding);
			this.groupBox_Settings.Controls.Add(this.groupBox_Send);
			this.groupBox_Settings.Controls.Add(this.label_Encoding);
			this.groupBox_Settings.Location = new System.Drawing.Point(12, 12);
			this.groupBox_Settings.Name = "groupBox_Settings";
			this.groupBox_Settings.Size = new System.Drawing.Size(587, 484);
			this.groupBox_Settings.TabIndex = 0;
			this.groupBox_Settings.TabStop = false;
			// 
			// label_DecodingMismatchBehavior
			// 
			this.label_DecodingMismatchBehavior.AutoSize = true;
			this.label_DecodingMismatchBehavior.Location = new System.Drawing.Point(15, 61);
			this.label_DecodingMismatchBehavior.Name = "label_DecodingMismatchBehavior";
			this.label_DecodingMismatchBehavior.Size = new System.Drawing.Size(161, 13);
			this.label_DecodingMismatchBehavior.TabIndex = 2;
			this.label_DecodingMismatchBehavior.Text = "Behavior on decoding &mismatch:";
			this.toolTip.SetToolTip(this.label_DecodingMismatchBehavior, "Applies to multi-byte character sets.");
			// 
			// comboBox_DecodingMismatchBehavior
			// 
			this.comboBox_DecodingMismatchBehavior.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_DecodingMismatchBehavior.FormattingEnabled = true;
			this.comboBox_DecodingMismatchBehavior.Location = new System.Drawing.Point(119, 77);
			this.comboBox_DecodingMismatchBehavior.Name = "comboBox_DecodingMismatchBehavior";
			this.comboBox_DecodingMismatchBehavior.Size = new System.Drawing.Size(153, 21);
			this.comboBox_DecodingMismatchBehavior.TabIndex = 3;
			this.toolTip.SetToolTip(this.comboBox_DecodingMismatchBehavior, resources.GetString("comboBox_DecodingMismatchBehavior.ToolTip"));
			this.comboBox_DecodingMismatchBehavior.SelectedIndexChanged += new System.EventHandler(this.comboBox_DecodingMismatchBehavior_SelectedIndexChanged);
			// 
			// groupBox_Display
			// 
			this.groupBox_Display.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox_Display.Controls.Add(this.groupBox_RxDisplay);
			this.groupBox_Display.Controls.Add(this.groupBox_TxDisplay);
			this.groupBox_Display.Controls.Add(this.checkBox_SeparateTxRxDisplay);
			this.groupBox_Display.Location = new System.Drawing.Point(6, 229);
			this.groupBox_Display.Name = "groupBox_Display";
			this.groupBox_Display.Size = new System.Drawing.Size(272, 249);
			this.groupBox_Display.TabIndex = 5;
			this.groupBox_Display.TabStop = false;
			this.groupBox_Display.Text = "Display Settings";
			// 
			// groupBox_RxDisplay
			// 
			this.groupBox_RxDisplay.Controls.Add(this.textTerminalSettingsSet_Rx);
			this.groupBox_RxDisplay.Location = new System.Drawing.Point(6, 147);
			this.groupBox_RxDisplay.Name = "groupBox_RxDisplay";
			this.groupBox_RxDisplay.Size = new System.Drawing.Size(260, 96);
			this.groupBox_RxDisplay.TabIndex = 1;
			this.groupBox_RxDisplay.TabStop = false;
			this.groupBox_RxDisplay.Text = "Rx Line Break";
			// 
			// textTerminalSettingsSet_Rx
			// 
			this.textTerminalSettingsSet_Rx.Location = new System.Drawing.Point(3, 19);
			this.textTerminalSettingsSet_Rx.Name = "textTerminalSettingsSet_Rx";
			this.textTerminalSettingsSet_Rx.Size = new System.Drawing.Size(251, 68);
			this.textTerminalSettingsSet_Rx.TabIndex = 0;
			this.textTerminalSettingsSet_Rx.SettingsChanged += new System.EventHandler(this.textTerminalSettingsSet_Rx_SettingsChanged);
			// 
			// groupBox_TxDisplay
			// 
			this.groupBox_TxDisplay.Controls.Add(this.textTerminalSettingsSet_Tx);
			this.groupBox_TxDisplay.Location = new System.Drawing.Point(6, 19);
			this.groupBox_TxDisplay.Name = "groupBox_TxDisplay";
			this.groupBox_TxDisplay.Size = new System.Drawing.Size(260, 96);
			this.groupBox_TxDisplay.TabIndex = 0;
			this.groupBox_TxDisplay.TabStop = false;
			this.groupBox_TxDisplay.Text = "&Line Break";
			// 
			// textTerminalSettingsSet_Tx
			// 
			this.textTerminalSettingsSet_Tx.Location = new System.Drawing.Point(3, 19);
			this.textTerminalSettingsSet_Tx.Name = "textTerminalSettingsSet_Tx";
			this.textTerminalSettingsSet_Tx.Size = new System.Drawing.Size(251, 68);
			this.textTerminalSettingsSet_Tx.TabIndex = 0;
			this.textTerminalSettingsSet_Tx.SettingsChanged += new System.EventHandler(this.textTerminalSettingsSet_Tx_SettingsChanged);
			// 
			// checkBox_SeparateTxRxDisplay
			// 
			this.checkBox_SeparateTxRxDisplay.AutoSize = true;
			this.checkBox_SeparateTxRxDisplay.Location = new System.Drawing.Point(12, 124);
			this.checkBox_SeparateTxRxDisplay.Name = "checkBox_SeparateTxRxDisplay";
			this.checkBox_SeparateTxRxDisplay.Size = new System.Drawing.Size(175, 17);
			this.checkBox_SeparateTxRxDisplay.TabIndex = 0;
			this.checkBox_SeparateTxRxDisplay.Text = "&Separate settings for Tx and Rx";
			this.checkBox_SeparateTxRxDisplay.UseVisualStyleBackColor = true;
			this.checkBox_SeparateTxRxDisplay.CheckedChanged += new System.EventHandler(this.checkBox_SeparateTxRxDisplay_CheckedChanged);
			// 
			// groupBox_Eol
			// 
			this.groupBox_Eol.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox_Eol.Controls.Add(this.checkBox_ShowEol);
			this.groupBox_Eol.Controls.Add(this.checkBox_SeparateTxRxEol);
			this.groupBox_Eol.Controls.Add(this.comboBox_RxEol);
			this.groupBox_Eol.Controls.Add(this.label_RxEol);
			this.groupBox_Eol.Controls.Add(this.comboBox_TxEol);
			this.groupBox_Eol.Controls.Add(this.label_TxEol);
			this.groupBox_Eol.Location = new System.Drawing.Point(6, 104);
			this.groupBox_Eol.Name = "groupBox_Eol";
			this.groupBox_Eol.Size = new System.Drawing.Size(272, 119);
			this.groupBox_Eol.TabIndex = 4;
			this.groupBox_Eol.TabStop = false;
			this.groupBox_Eol.Text = "EOL (End-Of-Line)";
			// 
			// checkBox_ShowEol
			// 
			this.checkBox_ShowEol.AutoSize = true;
			this.checkBox_ShowEol.Location = new System.Drawing.Point(12, 93);
			this.checkBox_ShowEol.Name = "checkBox_ShowEol";
			this.checkBox_ShowEol.Size = new System.Drawing.Size(127, 17);
			this.checkBox_ShowEol.TabIndex = 5;
			this.checkBox_ShowEol.Text = "Show EOL se&quence";
			this.checkBox_ShowEol.UseVisualStyleBackColor = true;
			this.checkBox_ShowEol.CheckedChanged += new System.EventHandler(this.checkBox_ShowEol_CheckedChanged);
			// 
			// checkBox_SeparateTxRxEol
			// 
			this.checkBox_SeparateTxRxEol.AutoSize = true;
			this.checkBox_SeparateTxRxEol.Location = new System.Drawing.Point(12, 45);
			this.checkBox_SeparateTxRxEol.Name = "checkBox_SeparateTxRxEol";
			this.checkBox_SeparateTxRxEol.Size = new System.Drawing.Size(215, 17);
			this.checkBox_SeparateTxRxEol.TabIndex = 2;
			this.checkBox_SeparateTxRxEol.Text = "Separate EOL sequences for Tx and Rx";
			this.checkBox_SeparateTxRxEol.UseVisualStyleBackColor = true;
			this.checkBox_SeparateTxRxEol.CheckedChanged += new System.EventHandler(this.checkBox_SeparateTxRxEol_CheckedChanged);
			// 
			// comboBox_RxEol
			// 
			this.comboBox_RxEol.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_RxEol.Enabled = false;
			this.comboBox_RxEol.Location = new System.Drawing.Point(113, 67);
			this.comboBox_RxEol.Name = "comboBox_RxEol";
			this.comboBox_RxEol.Size = new System.Drawing.Size(153, 21);
			this.comboBox_RxEol.TabIndex = 4;
			this.toolTip.SetToolTip(this.comboBox_RxEol, resources.GetString("comboBox_RxEol.ToolTip"));
			this.comboBox_RxEol.Validating += new System.ComponentModel.CancelEventHandler(this.comboBox_RxEol_Validating);
			// 
			// label_RxEol
			// 
			this.label_RxEol.AutoSize = true;
			this.label_RxEol.Enabled = false;
			this.label_RxEol.Location = new System.Drawing.Point(9, 70);
			this.label_RxEol.Name = "label_RxEol";
			this.label_RxEol.Size = new System.Drawing.Size(97, 13);
			this.label_RxEol.TabIndex = 3;
			this.label_RxEol.Text = "&Rx EOL sequence:";
			this.toolTip.SetToolTip(this.label_RxEol, resources.GetString("label_RxEol.ToolTip"));
			// 
			// comboBox_TxEol
			// 
			this.comboBox_TxEol.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_TxEol.Location = new System.Drawing.Point(113, 19);
			this.comboBox_TxEol.Name = "comboBox_TxEol";
			this.comboBox_TxEol.Size = new System.Drawing.Size(153, 21);
			this.comboBox_TxEol.TabIndex = 1;
			this.toolTip.SetToolTip(this.comboBox_TxEol, resources.GetString("comboBox_TxEol.ToolTip"));
			this.comboBox_TxEol.Validating += new System.ComponentModel.CancelEventHandler(this.comboBox_TxEol_Validating);
			// 
			// label_TxEol
			// 
			this.label_TxEol.AutoSize = true;
			this.label_TxEol.Location = new System.Drawing.Point(9, 22);
			this.label_TxEol.Name = "label_TxEol";
			this.label_TxEol.Size = new System.Drawing.Size(81, 13);
			this.label_TxEol.TabIndex = 0;
			this.label_TxEol.Text = "E&OL sequence:";
			this.toolTip.SetToolTip(this.label_TxEol, resources.GetString("label_TxEol.ToolTip"));
			// 
			// comboBox_Encoding
			// 
			this.comboBox_Encoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_Encoding.FormattingEnabled = true;
			this.comboBox_Encoding.Location = new System.Drawing.Point(17, 32);
			this.comboBox_Encoding.Name = "comboBox_Encoding";
			this.comboBox_Encoding.Size = new System.Drawing.Size(255, 21);
			this.comboBox_Encoding.TabIndex = 1;
			this.comboBox_Encoding.SelectedIndexChanged += new System.EventHandler(this.comboBox_Encoding_SelectedIndexChanged);
			// 
			// groupBox_Send
			// 
			this.groupBox_Send.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Send.Controls.Add(this.textBox_WaitForResponseTimeout);
			this.groupBox_Send.Controls.Add(this.label_WaitForResponseTimeout);
			this.groupBox_Send.Controls.Add(this.textBox_WaitForResponseNext);
			this.groupBox_Send.Controls.Add(this.label_WaitForResponseNext);
			this.groupBox_Send.Controls.Add(this.label_WaitForResponseOf);
			this.groupBox_Send.Controls.Add(this.textBox_WaitForResponseOf);
			this.groupBox_Send.Controls.Add(this.groupBox_Exclude);
			this.groupBox_Send.Controls.Add(this.groupBox_Substitute);
			this.groupBox_Send.Controls.Add(this.checkBox_WaitForResponse);
			this.groupBox_Send.Controls.Add(this.textBox_DelayInterval);
			this.groupBox_Send.Controls.Add(this.label_WaitForResponseNextUnit);
			this.groupBox_Send.Controls.Add(this.label_WaitForResponseTimeoutUnit);
			this.groupBox_Send.Controls.Add(this.label_DelayUnit);
			this.groupBox_Send.Controls.Add(this.textBox_Delay);
			this.groupBox_Send.Controls.Add(this.label_DelayIntervalUnit);
			this.groupBox_Send.Controls.Add(this.checkBox_Delay);
			this.groupBox_Send.Location = new System.Drawing.Point(284, 16);
			this.groupBox_Send.Name = "groupBox_Send";
			this.groupBox_Send.Size = new System.Drawing.Size(297, 462);
			this.groupBox_Send.TabIndex = 6;
			this.groupBox_Send.TabStop = false;
			this.groupBox_Send.Text = "Send Settings";
			// 
			// textBox_WaitForResponseTimeout
			// 
			this.textBox_WaitForResponseTimeout.Location = new System.Drawing.Point(194, 62);
			this.textBox_WaitForResponseTimeout.Name = "textBox_WaitForResponseTimeout";
			this.textBox_WaitForResponseTimeout.Size = new System.Drawing.Size(48, 20);
			this.textBox_WaitForResponseTimeout.TabIndex = 12;
			this.textBox_WaitForResponseTimeout.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.toolTip.SetToolTip(this.textBox_WaitForResponseTimeout, "When too few lines are received within this time-out,\r\nongoing send requests will" +
        " break and new requests\r\nwill be allowed again.\r\n\r\nSet to -1 for infinite waitin" +
        "g for response.");
			this.textBox_WaitForResponseTimeout.TextChanged += new System.EventHandler(this.textBox_WaitForResponseTimeout_TextChanged);
			this.textBox_WaitForResponseTimeout.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_WaitForResponseTimeout_Validating);
			// 
			// label_WaitForResponseTimeout
			// 
			this.label_WaitForResponseTimeout.AutoSize = true;
			this.label_WaitForResponseTimeout.Location = new System.Drawing.Point(126, 65);
			this.label_WaitForResponseTimeout.Name = "label_WaitForResponseTimeout";
			this.label_WaitForResponseTimeout.Size = new System.Drawing.Size(68, 13);
			this.label_WaitForResponseTimeout.TabIndex = 11;
			this.label_WaitForResponseTimeout.Text = "time-out after";
			this.label_WaitForResponseTimeout.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox_WaitForResponseNext
			// 
			this.textBox_WaitForResponseNext.Location = new System.Drawing.Point(56, 62);
			this.textBox_WaitForResponseNext.Name = "textBox_WaitForResponseNext";
			this.textBox_WaitForResponseNext.Size = new System.Drawing.Size(40, 20);
			this.textBox_WaitForResponseNext.TabIndex = 9;
			this.textBox_WaitForResponseNext.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBox_WaitForResponseNext.TextChanged += new System.EventHandler(this.textBox_WaitForResponseNext_TextChanged);
			this.textBox_WaitForResponseNext.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_WaitForResponseNext_Validating);
			// 
			// label_WaitForResponseNext
			// 
			this.label_WaitForResponseNext.AutoSize = true;
			this.label_WaitForResponseNext.Location = new System.Drawing.Point(29, 65);
			this.label_WaitForResponseNext.Name = "label_WaitForResponseNext";
			this.label_WaitForResponseNext.Size = new System.Drawing.Size(27, 13);
			this.label_WaitForResponseNext.TabIndex = 8;
			this.label_WaitForResponseNext.Text = "next";
			this.label_WaitForResponseNext.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label_WaitForResponseOf
			// 
			this.label_WaitForResponseOf.AutoSize = true;
			this.label_WaitForResponseOf.Location = new System.Drawing.Point(172, 44);
			this.label_WaitForResponseOf.Name = "label_WaitForResponseOf";
			this.label_WaitForResponseOf.Size = new System.Drawing.Size(114, 13);
			this.label_WaitForResponseOf.TabIndex = 7;
			this.label_WaitForResponseOf.Text = "line before sending the";
			this.label_WaitForResponseOf.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox_WaitForResponseOf
			// 
			this.textBox_WaitForResponseOf.Location = new System.Drawing.Point(131, 41);
			this.textBox_WaitForResponseOf.Name = "textBox_WaitForResponseOf";
			this.textBox_WaitForResponseOf.Size = new System.Drawing.Size(40, 20);
			this.textBox_WaitForResponseOf.TabIndex = 6;
			this.textBox_WaitForResponseOf.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBox_WaitForResponseOf.TextChanged += new System.EventHandler(this.textBox_WaitForResponseOf_TextChanged);
			this.textBox_WaitForResponseOf.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_WaitForResponseOf_Validating);
			// 
			// groupBox_Exclude
			// 
			this.groupBox_Exclude.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Exclude.Controls.Add(this.linkLabel_Regex);
			this.groupBox_Exclude.Controls.Add(this.stringListEdit_ExcludePatterns);
			this.groupBox_Exclude.Controls.Add(this.checkBox_Exclude);
			this.groupBox_Exclude.Location = new System.Drawing.Point(6, 189);
			this.groupBox_Exclude.Name = "groupBox_Exclude";
			this.groupBox_Exclude.Size = new System.Drawing.Size(285, 267);
			this.groupBox_Exclude.TabIndex = 15;
			this.groupBox_Exclude.TabStop = false;
			this.groupBox_Exclude.Text = "Text E&xclusion";
			this.toolTip.SetToolTip(this.groupBox_Exclude, resources.GetString("groupBox_Exclude.ToolTip"));
			// 
			// linkLabel_Regex
			// 
			this.linkLabel_Regex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.linkLabel_Regex.AutoSize = true;
			this.linkLabel_Regex.Location = new System.Drawing.Point(30, 244);
			this.linkLabel_Regex.Name = "linkLabel_Regex";
			this.linkLabel_Regex.Size = new System.Drawing.Size(150, 13);
			this.linkLabel_Regex.TabIndex = 2;
			this.linkLabel_Regex.TabStop = true;
			this.linkLabel_Regex.Text = ".NET Regex Quick Reference";
			this.linkLabel_Regex.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_Regex_LinkClicked);
			// 
			// stringListEdit_ExcludePatterns
			// 
			this.stringListEdit_ExcludePatterns.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.stringListEdit_ExcludePatterns.Location = new System.Drawing.Point(3, 40);
			this.stringListEdit_ExcludePatterns.MinimumSize = new System.Drawing.Size(132, 116);
			this.stringListEdit_ExcludePatterns.Name = "stringListEdit_ExcludePatterns";
			this.stringListEdit_ExcludePatterns.Size = new System.Drawing.Size(279, 201);
			this.stringListEdit_ExcludePatterns.StringList = new string[0];
			this.stringListEdit_ExcludePatterns.TabIndex = 1;
			this.stringListEdit_ExcludePatterns.Validating += new System.EventHandler<MKY.ComponentModel.CancelEventArgs<string>>(this.stringListEdit_ExcludePatterns_Validating);
			this.stringListEdit_ExcludePatterns.ListChanged += new System.EventHandler(this.stringListEdit_ExcludePatterns_ListChanged);
			// 
			// checkBox_Exclude
			// 
			this.checkBox_Exclude.AutoSize = true;
			this.checkBox_Exclude.Location = new System.Drawing.Point(6, 19);
			this.checkBox_Exclude.Name = "checkBox_Exclude";
			this.checkBox_Exclude.Size = new System.Drawing.Size(232, 17);
			this.checkBox_Exclude.TabIndex = 0;
			this.checkBox_Exclude.Text = "Do not send text with the following patterns:";
			this.checkBox_Exclude.UseVisualStyleBackColor = true;
			this.checkBox_Exclude.CheckedChanged += new System.EventHandler(this.checkBox_Exclude_CheckedChanged);
			// 
			// groupBox_Substitute
			// 
			this.groupBox_Substitute.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Substitute.Controls.Add(this.radioButton_SubstituteToLower);
			this.groupBox_Substitute.Controls.Add(this.radioButton_SubstituteToUpper);
			this.groupBox_Substitute.Controls.Add(this.radioButton_SubstituteNone);
			this.groupBox_Substitute.Location = new System.Drawing.Point(6, 88);
			this.groupBox_Substitute.Name = "groupBox_Substitute";
			this.groupBox_Substitute.Size = new System.Drawing.Size(285, 95);
			this.groupBox_Substitute.TabIndex = 14;
			this.groupBox_Substitute.TabStop = false;
			this.groupBox_Substitute.Text = "&Character Substitution";
			// 
			// radioButton_SubstituteToLower
			// 
			this.radioButton_SubstituteToLower.AutoSize = true;
			this.radioButton_SubstituteToLower.Location = new System.Drawing.Point(6, 65);
			this.radioButton_SubstituteToLower.Name = "radioButton_SubstituteToLower";
			this.radioButton_SubstituteToLower.Size = new System.Drawing.Size(182, 17);
			this.radioButton_SubstituteToLower.TabIndex = 2;
			this.radioButton_SubstituteToLower.Text = "Lower case (e.g. \"Abc\" -> \"abc\")";
			this.radioButton_SubstituteToLower.UseVisualStyleBackColor = true;
			this.radioButton_SubstituteToLower.CheckedChanged += new System.EventHandler(this.radioButton_SubstituteToLower_CheckedChanged);
			// 
			// radioButton_SubstituteToUpper
			// 
			this.radioButton_SubstituteToUpper.AutoSize = true;
			this.radioButton_SubstituteToUpper.Location = new System.Drawing.Point(6, 42);
			this.radioButton_SubstituteToUpper.Name = "radioButton_SubstituteToUpper";
			this.radioButton_SubstituteToUpper.Size = new System.Drawing.Size(185, 17);
			this.radioButton_SubstituteToUpper.TabIndex = 1;
			this.radioButton_SubstituteToUpper.Text = "Upper case (e.g. \"Abc\" -> \"ABC\")";
			this.radioButton_SubstituteToUpper.UseVisualStyleBackColor = true;
			this.radioButton_SubstituteToUpper.CheckedChanged += new System.EventHandler(this.radioButton_SubstituteToUpper_CheckedChanged);
			// 
			// radioButton_SubstituteNone
			// 
			this.radioButton_SubstituteNone.AutoSize = true;
			this.radioButton_SubstituteNone.Location = new System.Drawing.Point(6, 19);
			this.radioButton_SubstituteNone.Name = "radioButton_SubstituteNone";
			this.radioButton_SubstituteNone.Size = new System.Drawing.Size(149, 17);
			this.radioButton_SubstituteNone.TabIndex = 0;
			this.radioButton_SubstituteNone.Text = "None (e.g. \"Abc -> \"Abc\")";
			this.radioButton_SubstituteNone.UseVisualStyleBackColor = true;
			this.radioButton_SubstituteNone.CheckedChanged += new System.EventHandler(this.radioButton_SubstituteNone_CheckedChanged);
			// 
			// checkBox_WaitForResponse
			// 
			this.checkBox_WaitForResponse.AutoSize = true;
			this.checkBox_WaitForResponse.Location = new System.Drawing.Point(12, 43);
			this.checkBox_WaitForResponse.Name = "checkBox_WaitForResponse";
			this.checkBox_WaitForResponse.Size = new System.Drawing.Size(121, 17);
			this.checkBox_WaitForResponse.TabIndex = 5;
			this.checkBox_WaitForResponse.Text = "&Wait for response of";
			this.toolTip.SetToolTip(this.checkBox_WaitForResponse, resources.GetString("checkBox_WaitForResponse.ToolTip"));
			this.checkBox_WaitForResponse.UseVisualStyleBackColor = true;
			this.checkBox_WaitForResponse.CheckedChanged += new System.EventHandler(this.checkBox_WaitForResponse_CheckedChanged);
			// 
			// textBox_DelayInterval
			// 
			this.textBox_DelayInterval.Location = new System.Drawing.Point(194, 17);
			this.textBox_DelayInterval.Name = "textBox_DelayInterval";
			this.textBox_DelayInterval.Size = new System.Drawing.Size(40, 20);
			this.textBox_DelayInterval.TabIndex = 3;
			this.textBox_DelayInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBox_DelayInterval.TextChanged += new System.EventHandler(this.textBox_DelayInterval_TextChanged);
			this.textBox_DelayInterval.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_DelayInterval_Validating);
			// 
			// label_WaitForResponseNextUnit
			// 
			this.label_WaitForResponseNextUnit.AutoSize = true;
			this.label_WaitForResponseNextUnit.Location = new System.Drawing.Point(97, 65);
			this.label_WaitForResponseNextUnit.Name = "label_WaitForResponseNextUnit";
			this.label_WaitForResponseNextUnit.Size = new System.Drawing.Size(26, 13);
			this.label_WaitForResponseNextUnit.TabIndex = 10;
			this.label_WaitForResponseNextUnit.Text = "line;";
			this.label_WaitForResponseNextUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label_WaitForResponseTimeoutUnit
			// 
			this.label_WaitForResponseTimeoutUnit.AutoSize = true;
			this.label_WaitForResponseTimeoutUnit.Location = new System.Drawing.Point(243, 65);
			this.label_WaitForResponseTimeoutUnit.Name = "label_WaitForResponseTimeoutUnit";
			this.label_WaitForResponseTimeoutUnit.Size = new System.Drawing.Size(20, 13);
			this.label_WaitForResponseTimeoutUnit.TabIndex = 13;
			this.label_WaitForResponseTimeoutUnit.Text = "ms";
			this.label_WaitForResponseTimeoutUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label_DelayUnit
			// 
			this.label_DelayUnit.AutoSize = true;
			this.label_DelayUnit.Location = new System.Drawing.Point(147, 20);
			this.label_DelayUnit.Name = "label_DelayUnit";
			this.label_DelayUnit.Size = new System.Drawing.Size(47, 13);
			this.label_DelayUnit.TabIndex = 2;
			this.label_DelayUnit.Text = "ms each";
			this.label_DelayUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox_Delay
			// 
			this.textBox_Delay.Location = new System.Drawing.Point(95, 17);
			this.textBox_Delay.Name = "textBox_Delay";
			this.textBox_Delay.Size = new System.Drawing.Size(51, 20);
			this.textBox_Delay.TabIndex = 1;
			this.textBox_Delay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBox_Delay.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_Delay_Validating);
			// 
			// label_DelayIntervalUnit
			// 
			this.label_DelayIntervalUnit.AutoSize = true;
			this.label_DelayIntervalUnit.Location = new System.Drawing.Point(235, 20);
			this.label_DelayIntervalUnit.Name = "label_DelayIntervalUnit";
			this.label_DelayIntervalUnit.Size = new System.Drawing.Size(23, 13);
			this.label_DelayIntervalUnit.TabIndex = 4;
			this.label_DelayIntervalUnit.Text = "line";
			this.label_DelayIntervalUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkBox_Delay
			// 
			this.checkBox_Delay.AutoSize = true;
			this.checkBox_Delay.Location = new System.Drawing.Point(12, 19);
			this.checkBox_Delay.Name = "checkBox_Delay";
			this.checkBox_Delay.Size = new System.Drawing.Size(85, 17);
			this.checkBox_Delay.TabIndex = 0;
			this.checkBox_Delay.Text = "Add dela&y of";
			this.toolTip.SetToolTip(this.checkBox_Delay, resources.GetString("checkBox_Delay.ToolTip"));
			this.checkBox_Delay.UseVisualStyleBackColor = true;
			this.checkBox_Delay.CheckedChanged += new System.EventHandler(this.checkBox_Delay_CheckedChanged);
			// 
			// label_Encoding
			// 
			this.label_Encoding.AutoSize = true;
			this.label_Encoding.Location = new System.Drawing.Point(15, 16);
			this.label_Encoding.Name = "label_Encoding";
			this.label_Encoding.Size = new System.Drawing.Size(55, 13);
			this.label_Encoding.TabIndex = 0;
			this.label_Encoding.Text = "&Encoding:";
			// 
			// button_Defaults
			// 
			this.button_Defaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Defaults.Location = new System.Drawing.Point(615, 155);
			this.button_Defaults.Name = "button_Defaults";
			this.button_Defaults.Size = new System.Drawing.Size(75, 23);
			this.button_Defaults.TabIndex = 3;
			this.button_Defaults.Text = "&Defaults...";
			this.button_Defaults.UseVisualStyleBackColor = true;
			this.button_Defaults.Click += new System.EventHandler(this.button_Defaults_Click);
			// 
			// label_AdvancedSettingsRemark
			// 
			this.label_AdvancedSettingsRemark.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label_AdvancedSettingsRemark.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_AdvancedSettingsRemark.ForeColor = System.Drawing.SystemColors.GrayText;
			this.label_AdvancedSettingsRemark.Location = new System.Drawing.Point(599, 215);
			this.label_AdvancedSettingsRemark.Name = "label_AdvancedSettingsRemark";
			this.label_AdvancedSettingsRemark.Size = new System.Drawing.Size(102, 281);
			this.label_AdvancedSettingsRemark.TabIndex = 4;
			this.label_AdvancedSettingsRemark.Text = "Also see\r\n[Advanced Settings...]";
			this.label_AdvancedSettingsRemark.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// TextTerminalSettings
			// 
			this.AcceptButton = this.button_OK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button_Cancel;
			this.ClientSize = new System.Drawing.Size(702, 508);
			this.Controls.Add(this.button_Defaults);
			this.Controls.Add(this.groupBox_Settings);
			this.Controls.Add(this.button_Cancel);
			this.Controls.Add(this.button_OK);
			this.Controls.Add(this.label_AdvancedSettingsRemark);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TextTerminalSettings";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Text Terminal Settings";
			this.Deactivate += new System.EventHandler(this.TextTerminalSettings_Deactivate);
			this.Shown += new System.EventHandler(this.TextTerminalSettings_Shown);
			this.groupBox_Settings.ResumeLayout(false);
			this.groupBox_Settings.PerformLayout();
			this.groupBox_Display.ResumeLayout(false);
			this.groupBox_Display.PerformLayout();
			this.groupBox_RxDisplay.ResumeLayout(false);
			this.groupBox_TxDisplay.ResumeLayout(false);
			this.groupBox_Eol.ResumeLayout(false);
			this.groupBox_Eol.PerformLayout();
			this.groupBox_Send.ResumeLayout(false);
			this.groupBox_Send.PerformLayout();
			this.groupBox_Exclude.ResumeLayout(false);
			this.groupBox_Exclude.PerformLayout();
			this.groupBox_Substitute.ResumeLayout(false);
			this.groupBox_Substitute.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox_Settings;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.Button button_Cancel;
		private System.Windows.Forms.Button button_Defaults;
		private System.Windows.Forms.Label label_Encoding;
		private MKY.Windows.Forms.ComboBoxEx comboBox_Encoding;
		private System.Windows.Forms.GroupBox groupBox_Send;
		private System.Windows.Forms.GroupBox groupBox_Substitute;
		private System.Windows.Forms.RadioButton radioButton_SubstituteToLower;
		private System.Windows.Forms.RadioButton radioButton_SubstituteToUpper;
		private System.Windows.Forms.RadioButton radioButton_SubstituteNone;
		private System.Windows.Forms.CheckBox checkBox_WaitForResponse;
		private MKY.Windows.Forms.TextBoxEx textBox_DelayInterval;
		private System.Windows.Forms.Label label_DelayUnit;
		private MKY.Windows.Forms.TextBoxEx textBox_Delay;
		private System.Windows.Forms.Label label_DelayIntervalUnit;
		private System.Windows.Forms.CheckBox checkBox_Delay;
		private System.Windows.Forms.Label label_WaitForResponseNextUnit;
		private System.Windows.Forms.Label label_WaitForResponseTimeoutUnit;
		private MKY.Windows.Forms.TextBoxEx textBox_WaitForResponseTimeout;
		private System.Windows.Forms.GroupBox groupBox_Eol;
		private System.Windows.Forms.CheckBox checkBox_ShowEol;
		private System.Windows.Forms.CheckBox checkBox_SeparateTxRxEol;
		private MKY.Windows.Forms.ComboBoxEx comboBox_RxEol;
		private System.Windows.Forms.Label label_RxEol;
		private MKY.Windows.Forms.ComboBoxEx comboBox_TxEol;
		private System.Windows.Forms.Label label_TxEol;
		private System.Windows.Forms.GroupBox groupBox_Exclude;
		private System.Windows.Forms.CheckBox checkBox_Exclude;
		private MKY.Windows.Forms.StringListEdit stringListEdit_ExcludePatterns;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.LinkLabel linkLabel_Regex;
		private System.Windows.Forms.GroupBox groupBox_Display;
		private System.Windows.Forms.CheckBox checkBox_SeparateTxRxDisplay;
		private System.Windows.Forms.GroupBox groupBox_RxDisplay;
		private YAT.View.Controls.TextDisplaySettingsSet textTerminalSettingsSet_Rx;
		private System.Windows.Forms.GroupBox groupBox_TxDisplay;
		private YAT.View.Controls.TextDisplaySettingsSet textTerminalSettingsSet_Tx;
		private System.Windows.Forms.Label label_AdvancedSettingsRemark;
		private System.Windows.Forms.Label label_WaitForResponseTimeout;
		private MKY.Windows.Forms.TextBoxEx textBox_WaitForResponseNext;
		private System.Windows.Forms.Label label_WaitForResponseNext;
		private System.Windows.Forms.Label label_WaitForResponseOf;
		private MKY.Windows.Forms.TextBoxEx textBox_WaitForResponseOf;
		private System.Windows.Forms.Label label_DecodingMismatchBehavior;
		private System.Windows.Forms.ComboBox comboBox_DecodingMismatchBehavior;
	}
}
