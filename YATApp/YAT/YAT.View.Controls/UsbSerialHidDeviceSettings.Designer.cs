namespace YAT.View.Controls
{
	partial class UsbSerialHidDeviceSettings
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UsbSerialHidDeviceSettings));
			this.checkBox_AutoOpen = new System.Windows.Forms.CheckBox();
			this.linkLabel_Info = new System.Windows.Forms.LinkLabel();
			this.label_ReportFormat = new System.Windows.Forms.Label();
			this.checkBox_UseId = new System.Windows.Forms.CheckBox();
			this.checkBox_PrependPayloadByteLength = new System.Windows.Forms.CheckBox();
			this.checkBox_AppendTerminatingZero = new System.Windows.Forms.CheckBox();
			this.checkBox_FillLastReport = new System.Windows.Forms.CheckBox();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.textBox_Id = new MKY.Windows.Forms.TextBoxEx();
			this.checkBox_SeparateRxId = new System.Windows.Forms.CheckBox();
			this.textBox_RxId = new MKY.Windows.Forms.TextBoxEx();
			this.comboBox_Preset = new System.Windows.Forms.ComboBox();
			this.comboBox_FlowControl = new System.Windows.Forms.ComboBox();
			this.label_Line = new System.Windows.Forms.Label();
			this.label_Preset = new System.Windows.Forms.Label();
			this.label_Preview_1 = new System.Windows.Forms.Label();
			this.label_Preview_2 = new System.Windows.Forms.Label();
			this.label_Preview_3 = new System.Windows.Forms.Label();
			this.label_Preview_4 = new System.Windows.Forms.Label();
			this.label_Preview_5 = new System.Windows.Forms.Label();
			this.label_Preview_6 = new System.Windows.Forms.Label();
			this.label_Preview_7 = new System.Windows.Forms.Label();
			this.label_FlowControl = new System.Windows.Forms.Label();
			this.reportFormatPreview = new YAT.View.Controls.UsbSerialHidReportFormatPreview();
			this.SuspendLayout();
			// 
			// checkBox_AutoOpen
			// 
			this.checkBox_AutoOpen.AutoSize = true;
			this.checkBox_AutoOpen.Location = new System.Drawing.Point(6, 240);
			this.checkBox_AutoOpen.Name = "checkBox_AutoOpen";
			this.checkBox_AutoOpen.Size = new System.Drawing.Size(256, 17);
			this.checkBox_AutoOpen.TabIndex = 22;
			this.checkBox_AutoOpen.Text = "&When device is connected, automatically open it";
			this.checkBox_AutoOpen.UseVisualStyleBackColor = true;
			this.checkBox_AutoOpen.CheckedChanged += new System.EventHandler(this.checkBox_AutoOpen_CheckedChanged);
			// 
			// linkLabel_Info
			// 
			this.linkLabel_Info.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.linkLabel_Info.AutoSize = true;
			this.linkLabel_Info.Location = new System.Drawing.Point(50, 191);
			this.linkLabel_Info.Name = "linkLabel_Info";
			this.linkLabel_Info.Size = new System.Drawing.Size(203, 13);
			this.linkLabel_Info.TabIndex = 18;
			this.linkLabel_Info.TabStop = true;
			this.linkLabel_Info.Text = "TI HID API (Application Report SLAA453)";
			this.linkLabel_Info.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_Info_LinkClicked);
			// 
			// label_ReportFormat
			// 
			this.label_ReportFormat.AutoSize = true;
			this.label_ReportFormat.Location = new System.Drawing.Point(3, 6);
			this.label_ReportFormat.Name = "label_ReportFormat";
			this.label_ReportFormat.Size = new System.Drawing.Size(99, 13);
			this.label_ReportFormat.TabIndex = 0;
			this.label_ReportFormat.Text = "HID Report Format:";
			// 
			// checkBox_UseId
			// 
			this.checkBox_UseId.AutoSize = true;
			this.checkBox_UseId.Location = new System.Drawing.Point(6, 31);
			this.checkBox_UseId.Name = "checkBox_UseId";
			this.checkBox_UseId.Size = new System.Drawing.Size(55, 17);
			this.checkBox_UseId.TabIndex = 1;
			this.checkBox_UseId.Text = "Tx &ID:";
			this.toolTip.SetToolTip(this.checkBox_UseId, "Most devices expect a ID at the beginning of each report. This\r\nID may be used to" +
        " identify multiple logical channels over the\r\nsame HID interface, or multiple ch" +
        "annels to increase the bitrate.");
			this.checkBox_UseId.UseVisualStyleBackColor = true;
			this.checkBox_UseId.CheckedChanged += new System.EventHandler(this.checkBox_UseId_CheckedChanged);
			// 
			// checkBox_PrependPayloadByteLength
			// 
			this.checkBox_PrependPayloadByteLength.AutoSize = true;
			this.checkBox_PrependPayloadByteLength.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBox_PrependPayloadByteLength.Location = new System.Drawing.Point(119, 11);
			this.checkBox_PrependPayloadByteLength.Name = "checkBox_PrependPayloadByteLength";
			this.checkBox_PrependPayloadByteLength.Size = new System.Drawing.Size(138, 17);
			this.checkBox_PrependPayloadByteLength.TabIndex = 5;
			this.checkBox_PrependPayloadByteLength.Text = "Prepend payload &length";
			this.toolTip.SetToolTip(this.checkBox_PrependPayloadByteLength, "Some devices require to indicate the payload length before the payload starts.\r\nT" +
        "he length only relates to the current report, not the total length of all report" +
        "s.");
			this.checkBox_PrependPayloadByteLength.UseVisualStyleBackColor = true;
			this.checkBox_PrependPayloadByteLength.CheckedChanged += new System.EventHandler(this.checkBox_PrependPayloadByteLength_CheckedChanged);
			// 
			// checkBox_AppendTerminatingZero
			// 
			this.checkBox_AppendTerminatingZero.AutoSize = true;
			this.checkBox_AppendTerminatingZero.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBox_AppendTerminatingZero.Location = new System.Drawing.Point(117, 31);
			this.checkBox_AppendTerminatingZero.Name = "checkBox_AppendTerminatingZero";
			this.checkBox_AppendTerminatingZero.Size = new System.Drawing.Size(140, 17);
			this.checkBox_AppendTerminatingZero.TabIndex = 6;
			this.checkBox_AppendTerminatingZero.Text = "Append terminating &zero";
			this.toolTip.SetToolTip(this.checkBox_AppendTerminatingZero, "Append a terminating zero after the last payload byte.\r\nOnly useful for text/stri" +
        "ng based protocols.");
			this.checkBox_AppendTerminatingZero.UseVisualStyleBackColor = true;
			this.checkBox_AppendTerminatingZero.CheckedChanged += new System.EventHandler(this.checkBox_AppendTerminatingZero_CheckedChanged);
			// 
			// checkBox_FillLastReport
			// 
			this.checkBox_FillLastReport.AutoSize = true;
			this.checkBox_FillLastReport.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBox_FillLastReport.Location = new System.Drawing.Point(170, 51);
			this.checkBox_FillLastReport.Name = "checkBox_FillLastReport";
			this.checkBox_FillLastReport.Size = new System.Drawing.Size(87, 17);
			this.checkBox_FillLastReport.TabIndex = 7;
			this.checkBox_FillLastReport.Text = "&Fill last report";
			this.toolTip.SetToolTip(this.checkBox_FillLastReport, resources.GetString("checkBox_FillLastReport.ToolTip"));
			this.checkBox_FillLastReport.UseVisualStyleBackColor = true;
			this.checkBox_FillLastReport.CheckedChanged += new System.EventHandler(this.checkBox_FillLastReport_CheckedChanged);
			// 
			// textBox_Id
			// 
			this.textBox_Id.Location = new System.Drawing.Point(71, 29);
			this.textBox_Id.Name = "textBox_Id";
			this.textBox_Id.Size = new System.Drawing.Size(30, 20);
			this.textBox_Id.TabIndex = 2;
			this.textBox_Id.Text = "255";
			this.textBox_Id.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.toolTip.SetToolTip(this.textBox_Id, resources.GetString("textBox_Id.ToolTip"));
			this.textBox_Id.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_Id_Validating);
			// 
			// checkBox_SeparateRxId
			// 
			this.checkBox_SeparateRxId.AutoSize = true;
			this.checkBox_SeparateRxId.Location = new System.Drawing.Point(6, 51);
			this.checkBox_SeparateRxId.Name = "checkBox_SeparateRxId";
			this.checkBox_SeparateRxId.Size = new System.Drawing.Size(56, 17);
			this.checkBox_SeparateRxId.TabIndex = 3;
			this.checkBox_SeparateRxId.Text = "Rx I&D:";
			this.toolTip.SetToolTip(this.checkBox_SeparateRxId, "Enable this option to accept a different ID than the Tx ID;\r\nor to accept any ID." +
        "");
			this.checkBox_SeparateRxId.UseVisualStyleBackColor = true;
			this.checkBox_SeparateRxId.CheckedChanged += new System.EventHandler(this.checkBox_SeparateRxId_CheckedChanged);
			// 
			// textBox_RxId
			// 
			this.textBox_RxId.Location = new System.Drawing.Point(71, 49);
			this.textBox_RxId.Name = "textBox_RxId";
			this.textBox_RxId.Size = new System.Drawing.Size(30, 20);
			this.textBox_RxId.TabIndex = 4;
			this.textBox_RxId.Text = "255";
			this.textBox_RxId.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.toolTip.SetToolTip(this.textBox_RxId, "The ID accepted for input (Rx) reports. A numeric value within 0..255 or * (aster" +
        "isk) to accept any ID.");
			this.textBox_RxId.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_RxId_Validating);
			// 
			// comboBox_Preset
			// 
			this.comboBox_Preset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_Preset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_Preset.FormattingEnabled = true;
			this.comboBox_Preset.Location = new System.Drawing.Point(49, 168);
			this.comboBox_Preset.Name = "comboBox_Preset";
			this.comboBox_Preset.Size = new System.Drawing.Size(208, 21);
			this.comboBox_Preset.TabIndex = 17;
			this.toolTip.SetToolTip(this.comboBox_Preset, "For the ease of use, select a preset from the list.\r\n\r\nContact YAT via \"Help > Re" +
        "quest Feature\" to request additional presets.");
			this.comboBox_Preset.SelectedIndexChanged += new System.EventHandler(this.comboBox_Preset_SelectedIndexChanged);
			// 
			// comboBox_FlowControl
			// 
			this.comboBox_FlowControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_FlowControl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_FlowControl.Location = new System.Drawing.Point(74, 213);
			this.comboBox_FlowControl.Name = "comboBox_FlowControl";
			this.comboBox_FlowControl.Size = new System.Drawing.Size(183, 21);
			this.comboBox_FlowControl.TabIndex = 21;
			this.comboBox_FlowControl.SelectedIndexChanged += new System.EventHandler(this.comboBox_FlowControl_SelectedIndexChanged);
			// 
			// label_Line
			// 
			this.label_Line.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label_Line.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label_Line.Location = new System.Drawing.Point(3, 208);
			this.label_Line.Name = "label_Line";
			this.label_Line.Size = new System.Drawing.Size(255, 1);
			this.label_Line.TabIndex = 19;
			// 
			// label_Preset
			// 
			this.label_Preset.AutoSize = true;
			this.label_Preset.Location = new System.Drawing.Point(3, 171);
			this.label_Preset.Name = "label_Preset";
			this.label_Preset.Size = new System.Drawing.Size(40, 13);
			this.label_Preset.TabIndex = 16;
			this.label_Preset.Text = "P&reset:";
			// 
			// label_Preview_1
			// 
			this.label_Preview_1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_Preview_1.ForeColor = System.Drawing.SystemColors.GrayText;
			this.label_Preview_1.Location = new System.Drawing.Point(261, 78);
			this.label_Preview_1.Name = "label_Preview_1";
			this.label_Preview_1.Size = new System.Drawing.Size(18, 13);
			this.label_Preview_1.TabIndex = 9;
			this.label_Preview_1.Text = "P";
			this.label_Preview_1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label_Preview_2
			// 
			this.label_Preview_2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_Preview_2.ForeColor = System.Drawing.SystemColors.GrayText;
			this.label_Preview_2.Location = new System.Drawing.Point(261, 90);
			this.label_Preview_2.Name = "label_Preview_2";
			this.label_Preview_2.Size = new System.Drawing.Size(18, 13);
			this.label_Preview_2.TabIndex = 10;
			this.label_Preview_2.Text = "r";
			this.label_Preview_2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label_Preview_3
			// 
			this.label_Preview_3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_Preview_3.ForeColor = System.Drawing.SystemColors.GrayText;
			this.label_Preview_3.Location = new System.Drawing.Point(261, 102);
			this.label_Preview_3.Name = "label_Preview_3";
			this.label_Preview_3.Size = new System.Drawing.Size(18, 13);
			this.label_Preview_3.TabIndex = 11;
			this.label_Preview_3.Text = "e";
			this.label_Preview_3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label_Preview_4
			// 
			this.label_Preview_4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_Preview_4.ForeColor = System.Drawing.SystemColors.GrayText;
			this.label_Preview_4.Location = new System.Drawing.Point(261, 114);
			this.label_Preview_4.Name = "label_Preview_4";
			this.label_Preview_4.Size = new System.Drawing.Size(18, 13);
			this.label_Preview_4.TabIndex = 12;
			this.label_Preview_4.Text = "v";
			this.label_Preview_4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label_Preview_5
			// 
			this.label_Preview_5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_Preview_5.ForeColor = System.Drawing.SystemColors.GrayText;
			this.label_Preview_5.Location = new System.Drawing.Point(261, 126);
			this.label_Preview_5.Name = "label_Preview_5";
			this.label_Preview_5.Size = new System.Drawing.Size(18, 13);
			this.label_Preview_5.TabIndex = 13;
			this.label_Preview_5.Text = "i";
			this.label_Preview_5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label_Preview_6
			// 
			this.label_Preview_6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_Preview_6.ForeColor = System.Drawing.SystemColors.GrayText;
			this.label_Preview_6.Location = new System.Drawing.Point(261, 138);
			this.label_Preview_6.Name = "label_Preview_6";
			this.label_Preview_6.Size = new System.Drawing.Size(18, 13);
			this.label_Preview_6.TabIndex = 14;
			this.label_Preview_6.Text = "e";
			this.label_Preview_6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label_Preview_7
			// 
			this.label_Preview_7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_Preview_7.ForeColor = System.Drawing.SystemColors.GrayText;
			this.label_Preview_7.Location = new System.Drawing.Point(261, 150);
			this.label_Preview_7.Name = "label_Preview_7";
			this.label_Preview_7.Size = new System.Drawing.Size(18, 13);
			this.label_Preview_7.TabIndex = 15;
			this.label_Preview_7.Text = "w";
			this.label_Preview_7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label_FlowControl
			// 
			this.label_FlowControl.AutoSize = true;
			this.label_FlowControl.Location = new System.Drawing.Point(3, 216);
			this.label_FlowControl.Name = "label_FlowControl";
			this.label_FlowControl.Size = new System.Drawing.Size(68, 13);
			this.label_FlowControl.TabIndex = 20;
			this.label_FlowControl.Text = "Flow Control:";
			// 
			// reportFormatPreview
			// 
			this.reportFormatPreview.Location = new System.Drawing.Point(3, 77);
			this.reportFormatPreview.Name = "reportFormatPreview";
			this.reportFormatPreview.Size = new System.Drawing.Size(254, 88);
			this.reportFormatPreview.TabIndex = 8;
			// 
			// UsbSerialHidDeviceSettings
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.Controls.Add(this.comboBox_FlowControl);
			this.Controls.Add(this.label_FlowControl);
			this.Controls.Add(this.label_Preview_7);
			this.Controls.Add(this.label_Preview_6);
			this.Controls.Add(this.label_Preview_5);
			this.Controls.Add(this.label_Preview_4);
			this.Controls.Add(this.label_Preview_3);
			this.Controls.Add(this.label_Preview_2);
			this.Controls.Add(this.label_Preview_1);
			this.Controls.Add(this.textBox_RxId);
			this.Controls.Add(this.textBox_Id);
			this.Controls.Add(this.label_Preset);
			this.Controls.Add(this.comboBox_Preset);
			this.Controls.Add(this.label_Line);
			this.Controls.Add(this.checkBox_SeparateRxId);
			this.Controls.Add(this.checkBox_FillLastReport);
			this.Controls.Add(this.checkBox_AppendTerminatingZero);
			this.Controls.Add(this.checkBox_PrependPayloadByteLength);
			this.Controls.Add(this.checkBox_UseId);
			this.Controls.Add(this.label_ReportFormat);
			this.Controls.Add(this.reportFormatPreview);
			this.Controls.Add(this.linkLabel_Info);
			this.Controls.Add(this.checkBox_AutoOpen);
			this.Name = "UsbSerialHidDeviceSettings";
			this.Size = new System.Drawing.Size(285, 260);
			this.EnabledChanged += new System.EventHandler(this.UsbSerialHidDeviceSettings_EnabledChanged);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.UsbSerialHidPortSettings_Paint);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox checkBox_AutoOpen;
		private System.Windows.Forms.LinkLabel linkLabel_Info;
		private UsbSerialHidReportFormatPreview reportFormatPreview;
		private System.Windows.Forms.Label label_ReportFormat;
		private System.Windows.Forms.CheckBox checkBox_UseId;
		private System.Windows.Forms.CheckBox checkBox_PrependPayloadByteLength;
		private System.Windows.Forms.CheckBox checkBox_AppendTerminatingZero;
		private System.Windows.Forms.CheckBox checkBox_FillLastReport;
		private System.Windows.Forms.ToolTip toolTip;
		private MKY.Windows.Forms.TextBoxEx textBox_Id;
		private System.Windows.Forms.CheckBox checkBox_SeparateRxId;
		private MKY.Windows.Forms.TextBoxEx textBox_RxId;
		private System.Windows.Forms.Label label_Line;
		private System.Windows.Forms.ComboBox comboBox_Preset;
		private System.Windows.Forms.Label label_Preset;
		private System.Windows.Forms.Label label_Preview_1;
		private System.Windows.Forms.Label label_Preview_2;
		private System.Windows.Forms.Label label_Preview_3;
		private System.Windows.Forms.Label label_Preview_4;
		private System.Windows.Forms.Label label_Preview_5;
		private System.Windows.Forms.Label label_Preview_6;
		private System.Windows.Forms.Label label_Preview_7;
		private System.Windows.Forms.Label label_FlowControl;
		private System.Windows.Forms.ComboBox comboBox_FlowControl;
	}
}
