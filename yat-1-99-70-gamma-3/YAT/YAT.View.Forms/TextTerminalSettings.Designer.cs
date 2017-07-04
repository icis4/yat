﻿using System;
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
			this.button_OK = new System.Windows.Forms.Button();
			this.button_Cancel = new System.Windows.Forms.Button();
			this.groupBox_Settings = new System.Windows.Forms.GroupBox();
			this.groupBox_Eol = new System.Windows.Forms.GroupBox();
			this.checkBox_ShowEol = new System.Windows.Forms.CheckBox();
			this.checkBox_SeparateTxRxEol = new System.Windows.Forms.CheckBox();
			this.comboBox_RxEol = new System.Windows.Forms.ComboBox();
			this.label_RxEol = new System.Windows.Forms.Label();
			this.comboBox_TxEol = new System.Windows.Forms.ComboBox();
			this.label_TxEol = new System.Windows.Forms.Label();
			this.comboBox_Encoding = new System.Windows.Forms.ComboBox();
			this.groupBox_Send = new System.Windows.Forms.GroupBox();
			this.groupBox_Comments = new System.Windows.Forms.GroupBox();
			this.checkBox_SkipEolCommentWhiteSpace = new System.Windows.Forms.CheckBox();
			this.stringListEdit_EolCommentIndicators = new MKY.Windows.Forms.StringListEdit();
			this.label_CommentIndicators = new System.Windows.Forms.Label();
			this.checkBox_SkipEolComment = new System.Windows.Forms.CheckBox();
			this.groupBox_Substitute = new System.Windows.Forms.GroupBox();
			this.radioButton_SubstituteToLower = new System.Windows.Forms.RadioButton();
			this.radioButton_SubstituteToUpper = new System.Windows.Forms.RadioButton();
			this.radioButton_SubstituteNone = new System.Windows.Forms.RadioButton();
			this.checkBox_WaitForResponse = new System.Windows.Forms.CheckBox();
			this.textBox_DelayInterval = new System.Windows.Forms.TextBox();
			this.label_WaitForResponse = new System.Windows.Forms.Label();
			this.label_WaitForResponseUnit = new System.Windows.Forms.Label();
			this.label_DelayUnit = new System.Windows.Forms.Label();
			this.textBox_WaitForResponse = new System.Windows.Forms.TextBox();
			this.textBox_Delay = new System.Windows.Forms.TextBox();
			this.label_DelayIntervalUnit = new System.Windows.Forms.Label();
			this.checkBox_Delay = new System.Windows.Forms.CheckBox();
			this.label_Encoding = new System.Windows.Forms.Label();
			this.button_Defaults = new System.Windows.Forms.Button();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.checkBox_SkipEmptyLines = new System.Windows.Forms.CheckBox();
			this.groupBox_Settings.SuspendLayout();
			this.groupBox_Eol.SuspendLayout();
			this.groupBox_Send.SuspendLayout();
			this.groupBox_Comments.SuspendLayout();
			this.groupBox_Substitute.SuspendLayout();
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
			this.groupBox_Settings.Controls.Add(this.groupBox_Eol);
			this.groupBox_Settings.Controls.Add(this.comboBox_Encoding);
			this.groupBox_Settings.Controls.Add(this.groupBox_Send);
			this.groupBox_Settings.Controls.Add(this.label_Encoding);
			this.groupBox_Settings.Location = new System.Drawing.Point(12, 12);
			this.groupBox_Settings.Name = "groupBox_Settings";
			this.groupBox_Settings.Size = new System.Drawing.Size(284, 592);
			this.groupBox_Settings.TabIndex = 0;
			this.groupBox_Settings.TabStop = false;
			// 
			// groupBox_Eol
			// 
			this.groupBox_Eol.Controls.Add(this.checkBox_ShowEol);
			this.groupBox_Eol.Controls.Add(this.checkBox_SeparateTxRxEol);
			this.groupBox_Eol.Controls.Add(this.comboBox_RxEol);
			this.groupBox_Eol.Controls.Add(this.label_RxEol);
			this.groupBox_Eol.Controls.Add(this.comboBox_TxEol);
			this.groupBox_Eol.Controls.Add(this.label_TxEol);
			this.groupBox_Eol.Location = new System.Drawing.Point(6, 59);
			this.groupBox_Eol.Name = "groupBox_Eol";
			this.groupBox_Eol.Size = new System.Drawing.Size(272, 122);
			this.groupBox_Eol.TabIndex = 2;
			this.groupBox_Eol.TabStop = false;
			this.groupBox_Eol.Text = "EOL (end-of-line) sequence";
			// 
			// checkBox_ShowEol
			// 
			this.checkBox_ShowEol.AutoSize = true;
			this.checkBox_ShowEol.Location = new System.Drawing.Point(11, 96);
			this.checkBox_ShowEol.Name = "checkBox_ShowEol";
			this.checkBox_ShowEol.Size = new System.Drawing.Size(155, 17);
			this.checkBox_ShowEol.TabIndex = 5;
			this.checkBox_ShowEol.Text = "&Show end-of-line sequence";
			this.checkBox_ShowEol.CheckedChanged += new System.EventHandler(this.checkBox_ShowEol_CheckedChanged);
			// 
			// checkBox_SeparateTxRxEol
			// 
			this.checkBox_SeparateTxRxEol.AutoSize = true;
			this.checkBox_SeparateTxRxEol.Location = new System.Drawing.Point(11, 46);
			this.checkBox_SeparateTxRxEol.Name = "checkBox_SeparateTxRxEol";
			this.checkBox_SeparateTxRxEol.Size = new System.Drawing.Size(217, 17);
			this.checkBox_SeparateTxRxEol.TabIndex = 2;
			this.checkBox_SeparateTxRxEol.Text = "&Separate EOL sequencies for Tx and Rx";
			this.checkBox_SeparateTxRxEol.UseVisualStyleBackColor = true;
			this.checkBox_SeparateTxRxEol.CheckedChanged += new System.EventHandler(this.checkBox_SeparateTxRxEol_CheckedChanged);
			// 
			// comboBox_RxEol
			// 
			this.comboBox_RxEol.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_RxEol.Enabled = false;
			this.comboBox_RxEol.Location = new System.Drawing.Point(113, 69);
			this.comboBox_RxEol.Name = "comboBox_RxEol";
			this.comboBox_RxEol.Size = new System.Drawing.Size(153, 21);
			this.comboBox_RxEol.TabIndex = 4;
			this.toolTip.SetToolTip(this.comboBox_RxEol, "Either select a preset from the list, or fill in any sequence of bytes, e.g. <ETB" +
        "><NUL> or \\h(17 00).\r\n\r\nContact YAT via \"Help > Request Feature\" to request addi" +
        "tional presets.");
			this.comboBox_RxEol.SelectedIndexChanged += new System.EventHandler(this.comboBox_RxEol_SelectedIndexChanged);
			this.comboBox_RxEol.Validating += new System.ComponentModel.CancelEventHandler(this.comboBox_RxEol_Validating);
			// 
			// label_RxEol
			// 
			this.label_RxEol.AutoSize = true;
			this.label_RxEol.Enabled = false;
			this.label_RxEol.Location = new System.Drawing.Point(8, 72);
			this.label_RxEol.Name = "label_RxEol";
			this.label_RxEol.Size = new System.Drawing.Size(97, 13);
			this.label_RxEol.TabIndex = 3;
			this.label_RxEol.Text = "&Rx EOL sequence:";
			// 
			// comboBox_TxEol
			// 
			this.comboBox_TxEol.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_TxEol.Location = new System.Drawing.Point(113, 19);
			this.comboBox_TxEol.Name = "comboBox_TxEol";
			this.comboBox_TxEol.Size = new System.Drawing.Size(153, 21);
			this.comboBox_TxEol.TabIndex = 1;
			this.toolTip.SetToolTip(this.comboBox_TxEol, "Either select a preset from the list, or fill in any sequence of bytes, e.g. <ETB" +
        "><NUL> or \\h(17 00).\r\n\r\nContact YAT via \"Help > Request Feature\" to request addi" +
        "tional presets.");
			this.comboBox_TxEol.SelectedIndexChanged += new System.EventHandler(this.comboBox_TxEol_SelectedIndexChanged);
			this.comboBox_TxEol.Validating += new System.ComponentModel.CancelEventHandler(this.comboBox_TxEol_Validating);
			// 
			// label_TxEol
			// 
			this.label_TxEol.AutoSize = true;
			this.label_TxEol.Location = new System.Drawing.Point(8, 22);
			this.label_TxEol.Name = "label_TxEol";
			this.label_TxEol.Size = new System.Drawing.Size(81, 13);
			this.label_TxEol.TabIndex = 0;
			this.label_TxEol.Text = "E&OL sequence:";
			// 
			// comboBox_Encoding
			// 
			this.comboBox_Encoding.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
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
			this.groupBox_Send.Controls.Add(this.checkBox_SkipEmptyLines);
			this.groupBox_Send.Controls.Add(this.groupBox_Comments);
			this.groupBox_Send.Controls.Add(this.groupBox_Substitute);
			this.groupBox_Send.Controls.Add(this.checkBox_WaitForResponse);
			this.groupBox_Send.Controls.Add(this.textBox_DelayInterval);
			this.groupBox_Send.Controls.Add(this.label_WaitForResponse);
			this.groupBox_Send.Controls.Add(this.label_WaitForResponseUnit);
			this.groupBox_Send.Controls.Add(this.label_DelayUnit);
			this.groupBox_Send.Controls.Add(this.textBox_WaitForResponse);
			this.groupBox_Send.Controls.Add(this.textBox_Delay);
			this.groupBox_Send.Controls.Add(this.label_DelayIntervalUnit);
			this.groupBox_Send.Controls.Add(this.checkBox_Delay);
			this.groupBox_Send.Location = new System.Drawing.Point(6, 187);
			this.groupBox_Send.Name = "groupBox_Send";
			this.groupBox_Send.Size = new System.Drawing.Size(272, 398);
			this.groupBox_Send.TabIndex = 3;
			this.groupBox_Send.TabStop = false;
			this.groupBox_Send.Text = "Send Settings";
			// 
			// groupBox_Comments
			// 
			this.groupBox_Comments.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Comments.Controls.Add(this.checkBox_SkipEolCommentWhiteSpace);
			this.groupBox_Comments.Controls.Add(this.stringListEdit_EolCommentIndicators);
			this.groupBox_Comments.Controls.Add(this.label_CommentIndicators);
			this.groupBox_Comments.Controls.Add(this.checkBox_SkipEolComment);
			this.groupBox_Comments.Location = new System.Drawing.Point(6, 209);
			this.groupBox_Comments.Name = "groupBox_Comments";
			this.groupBox_Comments.Size = new System.Drawing.Size(259, 183);
			this.groupBox_Comments.TabIndex = 11;
			this.groupBox_Comments.TabStop = false;
			this.groupBox_Comments.Text = "Comments";
			// 
			// checkBox_SkipEolCommentWhiteSpace
			// 
			this.checkBox_SkipEolCommentWhiteSpace.AutoSize = true;
			this.checkBox_SkipEolCommentWhiteSpace.Location = new System.Drawing.Point(10, 160);
			this.checkBox_SkipEolCommentWhiteSpace.Name = "checkBox_SkipEolCommentWhiteSpace";
			this.checkBox_SkipEolCommentWhiteSpace.Size = new System.Drawing.Size(190, 17);
			this.checkBox_SkipEolCommentWhiteSpace.TabIndex = 3;
			this.checkBox_SkipEolCommentWhiteSpace.Text = "Omit w&hitespaces before indicators";
			this.checkBox_SkipEolCommentWhiteSpace.CheckedChanged += new System.EventHandler(this.checkBox_SkipEolCommentWhiteSpace_CheckedChanged);
			// 
			// stringListEdit_EolCommentIndicators
			// 
			this.stringListEdit_EolCommentIndicators.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.stringListEdit_EolCommentIndicators.Location = new System.Drawing.Point(107, 38);
			this.stringListEdit_EolCommentIndicators.MinimumSize = new System.Drawing.Size(132, 116);
			this.stringListEdit_EolCommentIndicators.Name = "stringListEdit_EolCommentIndicators";
			this.stringListEdit_EolCommentIndicators.Size = new System.Drawing.Size(146, 116);
			this.stringListEdit_EolCommentIndicators.StringList = new string[0];
			this.stringListEdit_EolCommentIndicators.TabIndex = 2;
			this.stringListEdit_EolCommentIndicators.StringListChanged += new System.EventHandler(this.stringListEdit_EolCommentIndicators_StringListChanged);
			// 
			// label_CommentIndicators
			// 
			this.label_CommentIndicators.AutoSize = true;
			this.label_CommentIndicators.Location = new System.Drawing.Point(7, 42);
			this.label_CommentIndicators.Name = "label_CommentIndicators";
			this.label_CommentIndicators.Size = new System.Drawing.Size(102, 13);
			this.label_CommentIndicators.TabIndex = 1;
			this.label_CommentIndicators.Text = "Comment &indicators:";
			// 
			// checkBox_SkipEolComment
			// 
			this.checkBox_SkipEolComment.AutoSize = true;
			this.checkBox_SkipEolComment.Location = new System.Drawing.Point(10, 19);
			this.checkBox_SkipEolComment.Name = "checkBox_SkipEolComment";
			this.checkBox_SkipEolComment.Size = new System.Drawing.Size(233, 17);
			this.checkBox_SkipEolComment.TabIndex = 0;
			this.checkBox_SkipEolComment.Text = "Do not send text that is marked as &comment";
			this.checkBox_SkipEolComment.CheckedChanged += new System.EventHandler(this.checkBox_SkipEolComment_CheckedChanged);
			// 
			// groupBox_Substitute
			// 
			this.groupBox_Substitute.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Substitute.Controls.Add(this.radioButton_SubstituteToLower);
			this.groupBox_Substitute.Controls.Add(this.radioButton_SubstituteToUpper);
			this.groupBox_Substitute.Controls.Add(this.radioButton_SubstituteNone);
			this.groupBox_Substitute.Location = new System.Drawing.Point(6, 115);
			this.groupBox_Substitute.Name = "groupBox_Substitute";
			this.groupBox_Substitute.Size = new System.Drawing.Size(260, 88);
			this.groupBox_Substitute.TabIndex = 10;
			this.groupBox_Substitute.TabStop = false;
			this.groupBox_Substitute.Text = "Substitute Characters";
			// 
			// radioButton_SubstituteToLower
			// 
			this.radioButton_SubstituteToLower.AutoSize = true;
			this.radioButton_SubstituteToLower.Location = new System.Drawing.Point(10, 65);
			this.radioButton_SubstituteToLower.Name = "radioButton_SubstituteToLower";
			this.radioButton_SubstituteToLower.Size = new System.Drawing.Size(182, 17);
			this.radioButton_SubstituteToLower.TabIndex = 2;
			this.radioButton_SubstituteToLower.Text = "&Lower case (e.g. \"Abc\" -> \"abc\")";
			this.radioButton_SubstituteToLower.CheckedChanged += new System.EventHandler(this.radioButton_SubstituteToLower_CheckedChanged);
			// 
			// radioButton_SubstituteToUpper
			// 
			this.radioButton_SubstituteToUpper.AutoSize = true;
			this.radioButton_SubstituteToUpper.Location = new System.Drawing.Point(10, 42);
			this.radioButton_SubstituteToUpper.Name = "radioButton_SubstituteToUpper";
			this.radioButton_SubstituteToUpper.Size = new System.Drawing.Size(185, 17);
			this.radioButton_SubstituteToUpper.TabIndex = 1;
			this.radioButton_SubstituteToUpper.Text = "&Upper case (e.g. \"Abc\" -> \"ABC\")";
			this.radioButton_SubstituteToUpper.CheckedChanged += new System.EventHandler(this.radioButton_SubstituteToUpper_CheckedChanged);
			// 
			// radioButton_SubstituteNone
			// 
			this.radioButton_SubstituteNone.AutoSize = true;
			this.radioButton_SubstituteNone.Location = new System.Drawing.Point(10, 19);
			this.radioButton_SubstituteNone.Name = "radioButton_SubstituteNone";
			this.radioButton_SubstituteNone.Size = new System.Drawing.Size(149, 17);
			this.radioButton_SubstituteNone.TabIndex = 0;
			this.radioButton_SubstituteNone.Text = "&None (e.g. \"Abc -> \"Abc\")";
			this.radioButton_SubstituteNone.CheckedChanged += new System.EventHandler(this.radioButton_SubstituteNone_CheckedChanged);
			// 
			// checkBox_WaitForResponse
			// 
			this.checkBox_WaitForResponse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkBox_WaitForResponse.AutoSize = true;
			this.checkBox_WaitForResponse.Location = new System.Drawing.Point(12, 69);
			this.checkBox_WaitForResponse.Name = "checkBox_WaitForResponse";
			this.checkBox_WaitForResponse.Size = new System.Drawing.Size(242, 17);
			this.checkBox_WaitForResponse.TabIndex = 6;
			this.checkBox_WaitForResponse.Text = "&Wait for response before sending the next line";
			this.toolTip.SetToolTip(this.checkBox_WaitForResponse, "Not implemented yet. See feature request #19 and bug #176 in online trackers for " +
        "details.");
			this.checkBox_WaitForResponse.UseVisualStyleBackColor = true;
			this.checkBox_WaitForResponse.CheckedChanged += new System.EventHandler(this.checkBox_WaitForResponse_CheckedChanged);
			// 
			// textBox_DelayInterval
			// 
			this.textBox_DelayInterval.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textBox_DelayInterval.Location = new System.Drawing.Point(195, 43);
			this.textBox_DelayInterval.Name = "textBox_DelayInterval";
			this.textBox_DelayInterval.Size = new System.Drawing.Size(40, 20);
			this.textBox_DelayInterval.TabIndex = 4;
			this.textBox_DelayInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBox_DelayInterval.TextChanged += new System.EventHandler(this.textBox_DelayInterval_TextChanged);
			this.textBox_DelayInterval.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_DelayInterval_Validating);
			// 
			// label_WaitForResponse
			// 
			this.label_WaitForResponse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label_WaitForResponse.AutoSize = true;
			this.label_WaitForResponse.Location = new System.Drawing.Point(27, 89);
			this.label_WaitForResponse.Name = "label_WaitForResponse";
			this.label_WaitForResponse.Size = new System.Drawing.Size(84, 13);
			this.label_WaitForResponse.TabIndex = 7;
			this.label_WaitForResponse.Text = "with a timeout of";
			this.label_WaitForResponse.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label_WaitForResponseUnit
			// 
			this.label_WaitForResponseUnit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label_WaitForResponseUnit.AutoSize = true;
			this.label_WaitForResponseUnit.Location = new System.Drawing.Point(162, 89);
			this.label_WaitForResponseUnit.Name = "label_WaitForResponseUnit";
			this.label_WaitForResponseUnit.Size = new System.Drawing.Size(20, 13);
			this.label_WaitForResponseUnit.TabIndex = 9;
			this.label_WaitForResponseUnit.Text = "ms";
			this.label_WaitForResponseUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label_DelayUnit
			// 
			this.label_DelayUnit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label_DelayUnit.AutoSize = true;
			this.label_DelayUnit.Location = new System.Drawing.Point(148, 46);
			this.label_DelayUnit.Name = "label_DelayUnit";
			this.label_DelayUnit.Size = new System.Drawing.Size(47, 13);
			this.label_DelayUnit.TabIndex = 3;
			this.label_DelayUnit.Text = "ms each";
			this.label_DelayUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox_WaitForResponse
			// 
			this.textBox_WaitForResponse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textBox_WaitForResponse.Location = new System.Drawing.Point(111, 86);
			this.textBox_WaitForResponse.Name = "textBox_WaitForResponse";
			this.textBox_WaitForResponse.Size = new System.Drawing.Size(51, 20);
			this.textBox_WaitForResponse.TabIndex = 8;
			this.textBox_WaitForResponse.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBox_WaitForResponse.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_WaitForResponse_Validating);
			// 
			// textBox_Delay
			// 
			this.textBox_Delay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textBox_Delay.Location = new System.Drawing.Point(97, 43);
			this.textBox_Delay.Name = "textBox_Delay";
			this.textBox_Delay.Size = new System.Drawing.Size(51, 20);
			this.textBox_Delay.TabIndex = 2;
			this.textBox_Delay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBox_Delay.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_Delay_Validating);
			// 
			// label_DelayIntervalUnit
			// 
			this.label_DelayIntervalUnit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label_DelayIntervalUnit.AutoSize = true;
			this.label_DelayIntervalUnit.Location = new System.Drawing.Point(235, 46);
			this.label_DelayIntervalUnit.Name = "label_DelayIntervalUnit";
			this.label_DelayIntervalUnit.Size = new System.Drawing.Size(23, 13);
			this.label_DelayIntervalUnit.TabIndex = 5;
			this.label_DelayIntervalUnit.Text = "line";
			this.label_DelayIntervalUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkBox_Delay
			// 
			this.checkBox_Delay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkBox_Delay.AutoSize = true;
			this.checkBox_Delay.Location = new System.Drawing.Point(12, 45);
			this.checkBox_Delay.Name = "checkBox_Delay";
			this.checkBox_Delay.Size = new System.Drawing.Size(85, 17);
			this.checkBox_Delay.TabIndex = 1;
			this.checkBox_Delay.Text = "Add dela&y of";
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
			this.button_Defaults.Location = new System.Drawing.Point(312, 123);
			this.button_Defaults.Name = "button_Defaults";
			this.button_Defaults.Size = new System.Drawing.Size(75, 23);
			this.button_Defaults.TabIndex = 3;
			this.button_Defaults.Text = "&Defaults...";
			this.button_Defaults.Click += new System.EventHandler(this.button_Defaults_Click);
			// 
			// checkBox_SkipEmptyLines
			// 
			this.checkBox_SkipEmptyLines.AutoSize = true;
			this.checkBox_SkipEmptyLines.Location = new System.Drawing.Point(12, 21);
			this.checkBox_SkipEmptyLines.Name = "checkBox_SkipEmptyLines";
			this.checkBox_SkipEmptyLines.Size = new System.Drawing.Size(192, 17);
			this.checkBox_SkipEmptyLines.TabIndex = 0;
			this.checkBox_SkipEmptyLines.Text = "&Skip empty lines when sending files";
			this.checkBox_SkipEmptyLines.UseVisualStyleBackColor = true;
			this.checkBox_SkipEmptyLines.CheckedChanged += new System.EventHandler(this.checkBox_SkipEmptyLines_CheckedChanged);
			// 
			// TextTerminalSettings
			// 
			this.AcceptButton = this.button_OK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button_Cancel;
			this.ClientSize = new System.Drawing.Size(399, 616);
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
			this.Shown += new System.EventHandler(this.TextTerminalSettings_Shown);
			this.groupBox_Settings.ResumeLayout(false);
			this.groupBox_Settings.PerformLayout();
			this.groupBox_Eol.ResumeLayout(false);
			this.groupBox_Eol.PerformLayout();
			this.groupBox_Send.ResumeLayout(false);
			this.groupBox_Send.PerformLayout();
			this.groupBox_Comments.ResumeLayout(false);
			this.groupBox_Comments.PerformLayout();
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
		private System.Windows.Forms.ComboBox comboBox_Encoding;
		private System.Windows.Forms.GroupBox groupBox_Send;
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
		private System.Windows.Forms.Label label_WaitForResponse;
		private System.Windows.Forms.Label label_WaitForResponseUnit;
		private System.Windows.Forms.TextBox textBox_WaitForResponse;
		private System.Windows.Forms.GroupBox groupBox_Eol;
		private System.Windows.Forms.CheckBox checkBox_ShowEol;
		private System.Windows.Forms.CheckBox checkBox_SeparateTxRxEol;
		private System.Windows.Forms.ComboBox comboBox_RxEol;
		private System.Windows.Forms.Label label_RxEol;
		private System.Windows.Forms.ComboBox comboBox_TxEol;
		private System.Windows.Forms.Label label_TxEol;
		private System.Windows.Forms.GroupBox groupBox_Comments;
		private System.Windows.Forms.Label label_CommentIndicators;
		private System.Windows.Forms.CheckBox checkBox_SkipEolComment;
		private MKY.Windows.Forms.StringListEdit stringListEdit_EolCommentIndicators;
		private System.Windows.Forms.CheckBox checkBox_SkipEolCommentWhiteSpace;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.CheckBox checkBox_SkipEmptyLines;
	}
}
