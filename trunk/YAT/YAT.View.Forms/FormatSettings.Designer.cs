using System;
using System.Collections.Generic;
using System.Text;

namespace YAT.View.Forms
{
	partial class FormatSettings
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
			this.button_Cancel = new System.Windows.Forms.Button();
			this.button_OK = new System.Windows.Forms.Button();
			this.groupBox_Elements = new System.Windows.Forms.GroupBox();
			this.monitor_Warning = new YAT.View.Controls.Monitor();
			this.label_Warning = new System.Windows.Forms.Label();
			this.textFormat_Warning = new YAT.View.Controls.TextFormat();
			this.monitor_Separator = new YAT.View.Controls.Monitor();
			this.label_Separator = new System.Windows.Forms.Label();
			this.textFormat_Separator = new YAT.View.Controls.TextFormat();
			this.monitor_IOControl = new YAT.View.Controls.Monitor();
			this.label_IOControl = new System.Windows.Forms.Label();
			this.textFormat_IOControl = new YAT.View.Controls.TextFormat();
			this.label_Remark3 = new System.Windows.Forms.Label();
			this.checkBox_EnableFormatting = new System.Windows.Forms.CheckBox();
			this.textFormat_Device = new YAT.View.Controls.TextFormat();
			this.monitor_Device = new YAT.View.Controls.Monitor();
			this.label_Device = new System.Windows.Forms.Label();
			this.label_Remark2 = new System.Windows.Forms.Label();
			this.label_Remark1 = new System.Windows.Forms.Label();
			this.button_Background = new System.Windows.Forms.Button();
			this.textFormat_TimeStamp = new YAT.View.Controls.TextFormat();
			this.monitor_TimeStamp = new YAT.View.Controls.Monitor();
			this.label_Date = new System.Windows.Forms.Label();
			this.textFormat_Direction = new YAT.View.Controls.TextFormat();
			this.monitor_Direction = new YAT.View.Controls.Monitor();
			this.label_Direction = new System.Windows.Forms.Label();
			this.monitor_Error = new YAT.View.Controls.Monitor();
			this.monitor_Length = new YAT.View.Controls.Monitor();
			this.monitor_TimeSpan = new YAT.View.Controls.Monitor();
			this.monitor_TimeDelta = new YAT.View.Controls.Monitor();
			this.monitor_TimeDuration = new YAT.View.Controls.Monitor();
			this.monitor_RxControl = new YAT.View.Controls.Monitor();
			this.monitor_RxData = new YAT.View.Controls.Monitor();
			this.monitor_TxControl = new YAT.View.Controls.Monitor();
			this.monitor_TxData = new YAT.View.Controls.Monitor();
			this.label_Error = new System.Windows.Forms.Label();
			this.label_Length = new System.Windows.Forms.Label();
			this.label_TimeSpan = new System.Windows.Forms.Label();
			this.label_TimeDelta = new System.Windows.Forms.Label();
			this.label_TimeDuration = new System.Windows.Forms.Label();
			this.label_RxControl = new System.Windows.Forms.Label();
			this.label_RxData = new System.Windows.Forms.Label();
			this.label_TxControl = new System.Windows.Forms.Label();
			this.label_TxData = new System.Windows.Forms.Label();
			this.textFormat_Error = new YAT.View.Controls.TextFormat();
			this.textFormat_Length = new YAT.View.Controls.TextFormat();
			this.textFormat_TimeSpan = new YAT.View.Controls.TextFormat();
			this.textFormat_TimeDelta = new YAT.View.Controls.TextFormat();
			this.textFormat_TimeDuration = new YAT.View.Controls.TextFormat();
			this.textFormat_RxControl = new YAT.View.Controls.TextFormat();
			this.textFormat_RxData = new YAT.View.Controls.TextFormat();
			this.textFormat_TxControl = new YAT.View.Controls.TextFormat();
			this.textFormat_TxData = new YAT.View.Controls.TextFormat();
			this.button_Font = new System.Windows.Forms.Button();
			this.button_Defaults = new System.Windows.Forms.Button();
			this.label_Example = new System.Windows.Forms.Label();
			this.groupBox_Options = new System.Windows.Forms.GroupBox();
			this.comboBox_ContentSeparator = new MKY.Windows.Forms.ComboBoxEx();
			this.label_ContentSeparator = new System.Windows.Forms.Label();
			this.comboBox_InfoSeparator = new MKY.Windows.Forms.ComboBoxEx();
			this.comboBox_InfoEnclosure = new MKY.Windows.Forms.ComboBoxEx();
			this.textBox_TimeDurationFormat = new MKY.Windows.Forms.TextBoxEx();
			this.checkBox_TimeStampUseUtc = new System.Windows.Forms.CheckBox();
			this.label_Reference = new System.Windows.Forms.Label();
			this.label_Presets = new System.Windows.Forms.Label();
			this.linkLabel_TimeSpanFormat = new System.Windows.Forms.LinkLabel();
			this.linkLabel_DateTimeFormat = new System.Windows.Forms.LinkLabel();
			this.label_TimeStampFormat = new System.Windows.Forms.Label();
			this.comboBox_TimeStampFormatPreset = new System.Windows.Forms.ComboBox();
			this.textBox_TimeStampFormat = new MKY.Windows.Forms.TextBoxEx();
			this.label_TimeSpanFormat = new System.Windows.Forms.Label();
			this.comboBox_TimeSpanFormatPreset = new System.Windows.Forms.ComboBox();
			this.textBox_TimeSpanFormat = new MKY.Windows.Forms.TextBoxEx();
			this.label_TimeDeltaFormat = new System.Windows.Forms.Label();
			this.comboBox_TimeDeltaFormatPreset = new System.Windows.Forms.ComboBox();
			this.textBox_TimeDeltaFormat = new MKY.Windows.Forms.TextBoxEx();
			this.label_TimeDurationFormat = new System.Windows.Forms.Label();
			this.comboBox_TimeDurationFormatPreset = new System.Windows.Forms.ComboBox();
			this.label_InfoEnclosure = new System.Windows.Forms.Label();
			this.label_InfoSeparator = new System.Windows.Forms.Label();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.colorDialog = new System.Windows.Forms.ColorDialog();
			this.monitor_Example = new YAT.View.Controls.Monitor();
			this.groupBox_Elements.SuspendLayout();
			this.groupBox_Options.SuspendLayout();
			this.SuspendLayout();
			// 
			// button_Cancel
			// 
			this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button_Cancel.Location = new System.Drawing.Point(765, 85);
			this.button_Cancel.Name = "button_Cancel";
			this.button_Cancel.Size = new System.Drawing.Size(75, 23);
			this.button_Cancel.TabIndex = 5;
			this.button_Cancel.Text = "Cancel";
			this.button_Cancel.UseVisualStyleBackColor = true;
			this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
			// 
			// button_OK
			// 
			this.button_OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button_OK.Location = new System.Drawing.Point(765, 58);
			this.button_OK.Name = "button_OK";
			this.button_OK.Size = new System.Drawing.Size(75, 23);
			this.button_OK.TabIndex = 4;
			this.button_OK.Text = "OK";
			this.button_OK.UseVisualStyleBackColor = true;
			this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
			// 
			// groupBox_Elements
			// 
			this.groupBox_Elements.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Elements.Controls.Add(this.monitor_Warning);
			this.groupBox_Elements.Controls.Add(this.label_Warning);
			this.groupBox_Elements.Controls.Add(this.textFormat_Warning);
			this.groupBox_Elements.Controls.Add(this.monitor_Separator);
			this.groupBox_Elements.Controls.Add(this.label_Separator);
			this.groupBox_Elements.Controls.Add(this.textFormat_Separator);
			this.groupBox_Elements.Controls.Add(this.monitor_IOControl);
			this.groupBox_Elements.Controls.Add(this.label_IOControl);
			this.groupBox_Elements.Controls.Add(this.textFormat_IOControl);
			this.groupBox_Elements.Controls.Add(this.label_Remark3);
			this.groupBox_Elements.Controls.Add(this.checkBox_EnableFormatting);
			this.groupBox_Elements.Controls.Add(this.textFormat_Device);
			this.groupBox_Elements.Controls.Add(this.monitor_Device);
			this.groupBox_Elements.Controls.Add(this.label_Device);
			this.groupBox_Elements.Controls.Add(this.label_Remark2);
			this.groupBox_Elements.Controls.Add(this.label_Remark1);
			this.groupBox_Elements.Controls.Add(this.button_Background);
			this.groupBox_Elements.Controls.Add(this.textFormat_TimeStamp);
			this.groupBox_Elements.Controls.Add(this.monitor_TimeStamp);
			this.groupBox_Elements.Controls.Add(this.label_Date);
			this.groupBox_Elements.Controls.Add(this.textFormat_Direction);
			this.groupBox_Elements.Controls.Add(this.monitor_Direction);
			this.groupBox_Elements.Controls.Add(this.label_Direction);
			this.groupBox_Elements.Controls.Add(this.monitor_Error);
			this.groupBox_Elements.Controls.Add(this.monitor_Length);
			this.groupBox_Elements.Controls.Add(this.monitor_TimeSpan);
			this.groupBox_Elements.Controls.Add(this.monitor_TimeDelta);
			this.groupBox_Elements.Controls.Add(this.monitor_TimeDuration);
			this.groupBox_Elements.Controls.Add(this.monitor_RxControl);
			this.groupBox_Elements.Controls.Add(this.monitor_RxData);
			this.groupBox_Elements.Controls.Add(this.monitor_TxControl);
			this.groupBox_Elements.Controls.Add(this.monitor_TxData);
			this.groupBox_Elements.Controls.Add(this.label_Error);
			this.groupBox_Elements.Controls.Add(this.label_Length);
			this.groupBox_Elements.Controls.Add(this.label_TimeSpan);
			this.groupBox_Elements.Controls.Add(this.label_TimeDelta);
			this.groupBox_Elements.Controls.Add(this.label_TimeDuration);
			this.groupBox_Elements.Controls.Add(this.label_RxControl);
			this.groupBox_Elements.Controls.Add(this.label_RxData);
			this.groupBox_Elements.Controls.Add(this.label_TxControl);
			this.groupBox_Elements.Controls.Add(this.label_TxData);
			this.groupBox_Elements.Controls.Add(this.textFormat_Error);
			this.groupBox_Elements.Controls.Add(this.textFormat_Length);
			this.groupBox_Elements.Controls.Add(this.textFormat_TimeSpan);
			this.groupBox_Elements.Controls.Add(this.textFormat_TimeDelta);
			this.groupBox_Elements.Controls.Add(this.textFormat_TimeDuration);
			this.groupBox_Elements.Controls.Add(this.textFormat_RxControl);
			this.groupBox_Elements.Controls.Add(this.textFormat_RxData);
			this.groupBox_Elements.Controls.Add(this.textFormat_TxControl);
			this.groupBox_Elements.Controls.Add(this.textFormat_TxData);
			this.groupBox_Elements.Controls.Add(this.button_Font);
			this.groupBox_Elements.Location = new System.Drawing.Point(12, 12);
			this.groupBox_Elements.Name = "groupBox_Elements";
			this.groupBox_Elements.Size = new System.Drawing.Size(736, 484);
			this.groupBox_Elements.TabIndex = 0;
			this.groupBox_Elements.TabStop = false;
			this.groupBox_Elements.Text = "Elements";
			// 
			// monitor_Warning
			// 
			this.monitor_Warning.ActiveConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_Warning.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.monitor_Warning.Location = new System.Drawing.Point(111, 370);
			this.monitor_Warning.Name = "monitor_Warning";
			this.monitor_Warning.ShowStatusPanel = false;
			this.monitor_Warning.Size = new System.Drawing.Size(275, 23);
			this.monitor_Warning.TabIndex = 38;
			this.monitor_Warning.TabStop = false;
			this.monitor_Warning.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			// 
			// label_Warning
			// 
			this.label_Warning.AutoSize = true;
			this.label_Warning.Location = new System.Drawing.Point(12, 375);
			this.label_Warning.Name = "label_Warning";
			this.label_Warning.Size = new System.Drawing.Size(50, 13);
			this.label_Warning.TabIndex = 37;
			this.label_Warning.Text = "&Warning:";
			// 
			// textFormat_Warning
			// 
			this.textFormat_Warning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textFormat_Warning.FormatColor = System.Drawing.Color.Black;
			this.textFormat_Warning.FormatFont = new System.Drawing.Font("DejaVu Sans Mono", 8.25F);
			this.textFormat_Warning.Location = new System.Drawing.Point(396, 370);
			this.textFormat_Warning.Name = "textFormat_Warning";
			this.textFormat_Warning.Size = new System.Drawing.Size(232, 23);
			this.textFormat_Warning.TabIndex = 39;
			this.textFormat_Warning.Tag = "12";
			this.textFormat_Warning.FormatChanged += new System.EventHandler(this.textFormat_FormatChanged);
			this.textFormat_Warning.CustomColorsChanged += new System.EventHandler(this.textFormat_CustomColorsChanged);
			// 
			// monitor_Separator
			// 
			this.monitor_Separator.ActiveConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_Separator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.monitor_Separator.Location = new System.Drawing.Point(111, 424);
			this.monitor_Separator.Name = "monitor_Separator";
			this.monitor_Separator.ShowStatusPanel = false;
			this.monitor_Separator.Size = new System.Drawing.Size(275, 23);
			this.monitor_Separator.TabIndex = 44;
			this.monitor_Separator.TabStop = false;
			this.monitor_Separator.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			// 
			// label_Separator
			// 
			this.label_Separator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label_Separator.AutoSize = true;
			this.label_Separator.Location = new System.Drawing.Point(12, 430);
			this.label_Separator.Name = "label_Separator";
			this.label_Separator.Size = new System.Drawing.Size(61, 13);
			this.label_Separator.TabIndex = 43;
			this.label_Separator.Text = "&Separator:";
			// 
			// textFormat_Separator
			// 
			this.textFormat_Separator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textFormat_Separator.FormatColor = System.Drawing.Color.Black;
			this.textFormat_Separator.FormatFont = new System.Drawing.Font("DejaVu Sans Mono", 8.25F);
			this.textFormat_Separator.Location = new System.Drawing.Point(396, 424);
			this.textFormat_Separator.Name = "textFormat_Separator";
			this.textFormat_Separator.Size = new System.Drawing.Size(232, 23);
			this.textFormat_Separator.TabIndex = 45;
			this.textFormat_Separator.Tag = "14";
			this.textFormat_Separator.FormatChanged += new System.EventHandler(this.textFormat_FormatChanged);
			this.textFormat_Separator.CustomColorsChanged += new System.EventHandler(this.textFormat_CustomColorsChanged);
			// 
			// monitor_IOControl
			// 
			this.monitor_IOControl.ActiveConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_IOControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.monitor_IOControl.Location = new System.Drawing.Point(111, 343);
			this.monitor_IOControl.Name = "monitor_IOControl";
			this.monitor_IOControl.ShowStatusPanel = false;
			this.monitor_IOControl.Size = new System.Drawing.Size(275, 23);
			this.monitor_IOControl.TabIndex = 35;
			this.monitor_IOControl.TabStop = false;
			this.monitor_IOControl.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			// 
			// label_IOControl
			// 
			this.label_IOControl.AutoSize = true;
			this.label_IOControl.Location = new System.Drawing.Point(12, 348);
			this.label_IOControl.Name = "label_IOControl";
			this.label_IOControl.Size = new System.Drawing.Size(62, 13);
			this.label_IOControl.TabIndex = 34;
			this.label_IOControl.Text = "I/O &Control:";
			// 
			// textFormat_IOControl
			// 
			this.textFormat_IOControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textFormat_IOControl.FormatColor = System.Drawing.Color.Black;
			this.textFormat_IOControl.FormatFont = new System.Drawing.Font("DejaVu Sans Mono", 8.25F);
			this.textFormat_IOControl.Location = new System.Drawing.Point(396, 343);
			this.textFormat_IOControl.Name = "textFormat_IOControl";
			this.textFormat_IOControl.Size = new System.Drawing.Size(232, 23);
			this.textFormat_IOControl.TabIndex = 36;
			this.textFormat_IOControl.Tag = "11";
			this.textFormat_IOControl.FormatChanged += new System.EventHandler(this.textFormat_FormatChanged);
			this.textFormat_IOControl.CustomColorsChanged += new System.EventHandler(this.textFormat_CustomColorsChanged);
			// 
			// label_Remark3
			// 
			this.label_Remark3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label_Remark3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_Remark3.ForeColor = System.Drawing.SystemColors.GrayText;
			this.label_Remark3.Location = new System.Drawing.Point(634, 450);
			this.label_Remark3.Name = "label_Remark3";
			this.label_Remark3.Size = new System.Drawing.Size(96, 26);
			this.label_Remark3.TabIndex = 47;
			this.label_Remark3.Text = "Background\r\nfor all elements.";
			this.label_Remark3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// checkBox_EnableFormatting
			// 
			this.checkBox_EnableFormatting.AutoSize = true;
			this.checkBox_EnableFormatting.Location = new System.Drawing.Point(15, 21);
			this.checkBox_EnableFormatting.Name = "checkBox_EnableFormatting";
			this.checkBox_EnableFormatting.Size = new System.Drawing.Size(111, 17);
			this.checkBox_EnableFormatting.TabIndex = 0;
			this.checkBox_EnableFormatting.Text = "Enable Formattin&g";
			this.checkBox_EnableFormatting.UseVisualStyleBackColor = true;
			this.checkBox_EnableFormatting.CheckedChanged += new System.EventHandler(this.checkBox_EnableFormatting_CheckedChanged);
			// 
			// textFormat_Device
			// 
			this.textFormat_Device.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textFormat_Device.FormatColor = System.Drawing.Color.Black;
			this.textFormat_Device.FormatFont = new System.Drawing.Font("DejaVu Sans Mono", 8.25F);
			this.textFormat_Device.Location = new System.Drawing.Point(396, 262);
			this.textFormat_Device.Name = "textFormat_Device";
			this.textFormat_Device.Size = new System.Drawing.Size(232, 23);
			this.textFormat_Device.TabIndex = 27;
			this.textFormat_Device.Tag = "8";
			this.textFormat_Device.FormatChanged += new System.EventHandler(this.textFormat_FormatChanged);
			this.textFormat_Device.CustomColorsChanged += new System.EventHandler(this.textFormat_CustomColorsChanged);
			// 
			// monitor_Device
			// 
			this.monitor_Device.ActiveConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_Device.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.monitor_Device.Location = new System.Drawing.Point(111, 262);
			this.monitor_Device.Name = "monitor_Device";
			this.monitor_Device.ShowStatusPanel = false;
			this.monitor_Device.Size = new System.Drawing.Size(275, 23);
			this.monitor_Device.TabIndex = 26;
			this.monitor_Device.TabStop = false;
			this.monitor_Device.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			// 
			// label_Device
			// 
			this.label_Device.AutoSize = true;
			this.label_Device.Location = new System.Drawing.Point(12, 267);
			this.label_Device.Name = "label_Device";
			this.label_Device.Size = new System.Drawing.Size(63, 13);
			this.label_Device.TabIndex = 25;
			this.label_Device.Text = "I/O De&vice:";
			// 
			// label_Remark2
			// 
			this.label_Remark2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label_Remark2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_Remark2.ForeColor = System.Drawing.SystemColors.GrayText;
			this.label_Remark2.Location = new System.Drawing.Point(634, 294);
			this.label_Remark2.Name = "label_Remark2";
			this.label_Remark2.Size = new System.Drawing.Size(96, 156);
			this.label_Remark2.TabIndex = 48;
			this.label_Remark2.Text = "Style and color\r\nper element type.";
			this.label_Remark2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label_Remark1
			// 
			this.label_Remark1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label_Remark1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_Remark1.ForeColor = System.Drawing.SystemColors.GrayText;
			this.label_Remark1.Location = new System.Drawing.Point(634, 264);
			this.label_Remark1.Name = "label_Remark1";
			this.label_Remark1.Size = new System.Drawing.Size(96, 26);
			this.label_Remark1.TabIndex = 49;
			this.label_Remark1.Text = "Font and size\r\nfor all elements.";
			this.label_Remark1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// button_Background
			// 
			this.button_Background.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Background.Location = new System.Drawing.Point(492, 451);
			this.button_Background.Name = "button_Background";
			this.button_Background.Size = new System.Drawing.Size(136, 23);
			this.button_Background.TabIndex = 46;
			this.button_Background.Text = "&Background Color...";
			this.button_Background.UseVisualStyleBackColor = true;
			this.button_Background.Click += new System.EventHandler(this.button_Background_Click);
			// 
			// textFormat_TimeStamp
			// 
			this.textFormat_TimeStamp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textFormat_TimeStamp.FormatColor = System.Drawing.Color.Black;
			this.textFormat_TimeStamp.FormatFont = new System.Drawing.Font("DejaVu Sans Mono", 8.25F);
			this.textFormat_TimeStamp.Location = new System.Drawing.Point(396, 154);
			this.textFormat_TimeStamp.Name = "textFormat_TimeStamp";
			this.textFormat_TimeStamp.Size = new System.Drawing.Size(232, 23);
			this.textFormat_TimeStamp.TabIndex = 15;
			this.textFormat_TimeStamp.Tag = "4";
			this.textFormat_TimeStamp.FormatChanged += new System.EventHandler(this.textFormat_FormatChanged);
			this.textFormat_TimeStamp.CustomColorsChanged += new System.EventHandler(this.textFormat_CustomColorsChanged);
			// 
			// monitor_TimeStamp
			// 
			this.monitor_TimeStamp.ActiveConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_TimeStamp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.monitor_TimeStamp.Location = new System.Drawing.Point(111, 154);
			this.monitor_TimeStamp.Name = "monitor_TimeStamp";
			this.monitor_TimeStamp.ShowStatusPanel = false;
			this.monitor_TimeStamp.Size = new System.Drawing.Size(275, 23);
			this.monitor_TimeStamp.TabIndex = 14;
			this.monitor_TimeStamp.TabStop = false;
			this.monitor_TimeStamp.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			// 
			// label_Date
			// 
			this.label_Date.AutoSize = true;
			this.label_Date.Location = new System.Drawing.Point(12, 159);
			this.label_Date.Name = "label_Date";
			this.label_Date.Size = new System.Drawing.Size(66, 13);
			this.label_Date.TabIndex = 13;
			this.label_Date.Text = "T&ime Stamp:";
			// 
			// textFormat_Direction
			// 
			this.textFormat_Direction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textFormat_Direction.FormatColor = System.Drawing.Color.Black;
			this.textFormat_Direction.FormatFont = new System.Drawing.Font("DejaVu Sans Mono", 8.25F);
			this.textFormat_Direction.Location = new System.Drawing.Point(396, 289);
			this.textFormat_Direction.Name = "textFormat_Direction";
			this.textFormat_Direction.Size = new System.Drawing.Size(232, 23);
			this.textFormat_Direction.TabIndex = 30;
			this.textFormat_Direction.Tag = "9";
			this.textFormat_Direction.FormatChanged += new System.EventHandler(this.textFormat_FormatChanged);
			this.textFormat_Direction.CustomColorsChanged += new System.EventHandler(this.textFormat_CustomColorsChanged);
			// 
			// monitor_Direction
			// 
			this.monitor_Direction.ActiveConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_Direction.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.monitor_Direction.Location = new System.Drawing.Point(111, 289);
			this.monitor_Direction.Name = "monitor_Direction";
			this.monitor_Direction.ShowStatusPanel = false;
			this.monitor_Direction.Size = new System.Drawing.Size(275, 23);
			this.monitor_Direction.TabIndex = 29;
			this.monitor_Direction.TabStop = false;
			this.monitor_Direction.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			// 
			// label_Direction
			// 
			this.label_Direction.AutoSize = true;
			this.label_Direction.Location = new System.Drawing.Point(12, 294);
			this.label_Direction.Name = "label_Direction";
			this.label_Direction.Size = new System.Drawing.Size(52, 13);
			this.label_Direction.TabIndex = 28;
			this.label_Direction.Text = "&Direction:";
			// 
			// monitor_Error
			// 
			this.monitor_Error.ActiveConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_Error.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.monitor_Error.Location = new System.Drawing.Point(111, 397);
			this.monitor_Error.Name = "monitor_Error";
			this.monitor_Error.ShowStatusPanel = false;
			this.monitor_Error.Size = new System.Drawing.Size(275, 23);
			this.monitor_Error.TabIndex = 41;
			this.monitor_Error.TabStop = false;
			this.monitor_Error.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			// 
			// monitor_Length
			// 
			this.monitor_Length.ActiveConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_Length.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.monitor_Length.Location = new System.Drawing.Point(111, 316);
			this.monitor_Length.Name = "monitor_Length";
			this.monitor_Length.ShowStatusPanel = false;
			this.monitor_Length.Size = new System.Drawing.Size(275, 23);
			this.monitor_Length.TabIndex = 32;
			this.monitor_Length.TabStop = false;
			this.monitor_Length.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			// 
			// monitor_TimeSpan
			// 
			this.monitor_TimeSpan.ActiveConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_TimeSpan.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.monitor_TimeSpan.Location = new System.Drawing.Point(111, 181);
			this.monitor_TimeSpan.Name = "monitor_TimeSpan";
			this.monitor_TimeSpan.ShowStatusPanel = false;
			this.monitor_TimeSpan.Size = new System.Drawing.Size(275, 23);
			this.monitor_TimeSpan.TabIndex = 17;
			this.monitor_TimeSpan.TabStop = false;
			this.monitor_TimeSpan.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			// 
			// monitor_TimeDelta
			// 
			this.monitor_TimeDelta.ActiveConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_TimeDelta.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.monitor_TimeDelta.Location = new System.Drawing.Point(111, 208);
			this.monitor_TimeDelta.Name = "monitor_TimeDelta";
			this.monitor_TimeDelta.ShowStatusPanel = false;
			this.monitor_TimeDelta.Size = new System.Drawing.Size(275, 23);
			this.monitor_TimeDelta.TabIndex = 20;
			this.monitor_TimeDelta.TabStop = false;
			this.monitor_TimeDelta.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			// 
			// monitor_TimeDuration
			// 
			this.monitor_TimeDuration.ActiveConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_TimeDuration.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.monitor_TimeDuration.Location = new System.Drawing.Point(111, 235);
			this.monitor_TimeDuration.Name = "monitor_TimeDuration";
			this.monitor_TimeDuration.ShowStatusPanel = false;
			this.monitor_TimeDuration.Size = new System.Drawing.Size(275, 23);
			this.monitor_TimeDuration.TabIndex = 23;
			this.monitor_TimeDuration.TabStop = false;
			this.monitor_TimeDuration.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			// 
			// monitor_RxControl
			// 
			this.monitor_RxControl.ActiveConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_RxControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.monitor_RxControl.Location = new System.Drawing.Point(111, 127);
			this.monitor_RxControl.Name = "monitor_RxControl";
			this.monitor_RxControl.ShowStatusPanel = false;
			this.monitor_RxControl.Size = new System.Drawing.Size(275, 23);
			this.monitor_RxControl.TabIndex = 11;
			this.monitor_RxControl.TabStop = false;
			this.monitor_RxControl.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			// 
			// monitor_RxData
			// 
			this.monitor_RxData.ActiveConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_RxData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.monitor_RxData.Location = new System.Drawing.Point(111, 100);
			this.monitor_RxData.Name = "monitor_RxData";
			this.monitor_RxData.ShowStatusPanel = false;
			this.monitor_RxData.Size = new System.Drawing.Size(275, 23);
			this.monitor_RxData.TabIndex = 8;
			this.monitor_RxData.TabStop = false;
			this.monitor_RxData.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			// 
			// monitor_TxControl
			// 
			this.monitor_TxControl.ActiveConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_TxControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.monitor_TxControl.Location = new System.Drawing.Point(111, 73);
			this.monitor_TxControl.Name = "monitor_TxControl";
			this.monitor_TxControl.ShowStatusPanel = false;
			this.monitor_TxControl.Size = new System.Drawing.Size(275, 23);
			this.monitor_TxControl.TabIndex = 5;
			this.monitor_TxControl.TabStop = false;
			this.monitor_TxControl.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			// 
			// monitor_TxData
			// 
			this.monitor_TxData.ActiveConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_TxData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.monitor_TxData.Location = new System.Drawing.Point(111, 46);
			this.monitor_TxData.Name = "monitor_TxData";
			this.monitor_TxData.ShowStatusPanel = false;
			this.monitor_TxData.Size = new System.Drawing.Size(275, 23);
			this.monitor_TxData.TabIndex = 2;
			this.monitor_TxData.TabStop = false;
			this.monitor_TxData.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			// 
			// label_Error
			// 
			this.label_Error.AutoSize = true;
			this.label_Error.Location = new System.Drawing.Point(12, 402);
			this.label_Error.Name = "label_Error";
			this.label_Error.Size = new System.Drawing.Size(32, 13);
			this.label_Error.TabIndex = 40;
			this.label_Error.Text = "&Error:";
			// 
			// label_Length
			// 
			this.label_Length.AutoSize = true;
			this.label_Length.Location = new System.Drawing.Point(12, 321);
			this.label_Length.Name = "label_Length";
			this.label_Length.Size = new System.Drawing.Size(43, 13);
			this.label_Length.TabIndex = 31;
			this.label_Length.Text = "&Length:";
			// 
			// label_TimeSpan
			// 
			this.label_TimeSpan.AutoSize = true;
			this.label_TimeSpan.Location = new System.Drawing.Point(12, 186);
			this.label_TimeSpan.Name = "label_TimeSpan";
			this.label_TimeSpan.Size = new System.Drawing.Size(61, 13);
			this.label_TimeSpan.TabIndex = 16;
			this.label_TimeSpan.Text = "Ti&me Span:";
			// 
			// label_TimeDelta
			// 
			this.label_TimeDelta.AutoSize = true;
			this.label_TimeDelta.Location = new System.Drawing.Point(12, 213);
			this.label_TimeDelta.Name = "label_TimeDelta";
			this.label_TimeDelta.Size = new System.Drawing.Size(61, 13);
			this.label_TimeDelta.TabIndex = 19;
			this.label_TimeDelta.Text = "Tim&e Delta:";
			// 
			// label_TimeDuration
			// 
			this.label_TimeDuration.AutoSize = true;
			this.label_TimeDuration.Location = new System.Drawing.Point(12, 240);
			this.label_TimeDuration.Name = "label_TimeDuration";
			this.label_TimeDuration.Size = new System.Drawing.Size(76, 13);
			this.label_TimeDuration.TabIndex = 22;
			this.label_TimeDuration.Text = "Time D&uration:";
			// 
			// label_RxControl
			// 
			this.label_RxControl.AutoSize = true;
			this.label_RxControl.Location = new System.Drawing.Point(12, 132);
			this.label_RxControl.Name = "label_RxControl";
			this.label_RxControl.Size = new System.Drawing.Size(59, 13);
			this.label_RxControl.TabIndex = 10;
			this.label_RxControl.Text = "Rx C&ontrol:";
			// 
			// label_RxData
			// 
			this.label_RxData.AutoSize = true;
			this.label_RxData.Location = new System.Drawing.Point(12, 105);
			this.label_RxData.Name = "label_RxData";
			this.label_RxData.Size = new System.Drawing.Size(49, 13);
			this.label_RxData.TabIndex = 7;
			this.label_RxData.Text = "&Rx Data:";
			// 
			// label_TxControl
			// 
			this.label_TxControl.AutoSize = true;
			this.label_TxControl.Location = new System.Drawing.Point(12, 78);
			this.label_TxControl.Name = "label_TxControl";
			this.label_TxControl.Size = new System.Drawing.Size(58, 13);
			this.label_TxControl.TabIndex = 4;
			this.label_TxControl.Text = "Tx &Control:";
			// 
			// label_TxData
			// 
			this.label_TxData.AutoSize = true;
			this.label_TxData.Location = new System.Drawing.Point(12, 51);
			this.label_TxData.Name = "label_TxData";
			this.label_TxData.Size = new System.Drawing.Size(48, 13);
			this.label_TxData.TabIndex = 1;
			this.label_TxData.Text = "&Tx Data:";
			// 
			// textFormat_Error
			// 
			this.textFormat_Error.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textFormat_Error.FormatColor = System.Drawing.Color.Black;
			this.textFormat_Error.FormatFont = new System.Drawing.Font("DejaVu Sans Mono", 8.25F);
			this.textFormat_Error.Location = new System.Drawing.Point(396, 397);
			this.textFormat_Error.Name = "textFormat_Error";
			this.textFormat_Error.Size = new System.Drawing.Size(232, 23);
			this.textFormat_Error.TabIndex = 42;
			this.textFormat_Error.Tag = "13";
			this.textFormat_Error.FormatChanged += new System.EventHandler(this.textFormat_FormatChanged);
			this.textFormat_Error.CustomColorsChanged += new System.EventHandler(this.textFormat_CustomColorsChanged);
			// 
			// textFormat_Length
			// 
			this.textFormat_Length.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textFormat_Length.FormatColor = System.Drawing.Color.Black;
			this.textFormat_Length.FormatFont = new System.Drawing.Font("DejaVu Sans Mono", 8.25F);
			this.textFormat_Length.Location = new System.Drawing.Point(396, 316);
			this.textFormat_Length.Name = "textFormat_Length";
			this.textFormat_Length.Size = new System.Drawing.Size(232, 23);
			this.textFormat_Length.TabIndex = 33;
			this.textFormat_Length.Tag = "10";
			this.textFormat_Length.FormatChanged += new System.EventHandler(this.textFormat_FormatChanged);
			this.textFormat_Length.CustomColorsChanged += new System.EventHandler(this.textFormat_CustomColorsChanged);
			// 
			// textFormat_TimeSpan
			// 
			this.textFormat_TimeSpan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textFormat_TimeSpan.FormatColor = System.Drawing.Color.Black;
			this.textFormat_TimeSpan.FormatFont = new System.Drawing.Font("DejaVu Sans Mono", 8.25F);
			this.textFormat_TimeSpan.Location = new System.Drawing.Point(396, 181);
			this.textFormat_TimeSpan.Name = "textFormat_TimeSpan";
			this.textFormat_TimeSpan.Size = new System.Drawing.Size(232, 23);
			this.textFormat_TimeSpan.TabIndex = 18;
			this.textFormat_TimeSpan.Tag = "5";
			this.textFormat_TimeSpan.FormatChanged += new System.EventHandler(this.textFormat_FormatChanged);
			this.textFormat_TimeSpan.CustomColorsChanged += new System.EventHandler(this.textFormat_CustomColorsChanged);
			// 
			// textFormat_TimeDelta
			// 
			this.textFormat_TimeDelta.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textFormat_TimeDelta.FormatColor = System.Drawing.Color.Black;
			this.textFormat_TimeDelta.FormatFont = new System.Drawing.Font("DejaVu Sans Mono", 8.25F);
			this.textFormat_TimeDelta.Location = new System.Drawing.Point(396, 208);
			this.textFormat_TimeDelta.Name = "textFormat_TimeDelta";
			this.textFormat_TimeDelta.Size = new System.Drawing.Size(232, 23);
			this.textFormat_TimeDelta.TabIndex = 21;
			this.textFormat_TimeDelta.Tag = "6";
			this.textFormat_TimeDelta.FormatChanged += new System.EventHandler(this.textFormat_FormatChanged);
			this.textFormat_TimeDelta.CustomColorsChanged += new System.EventHandler(this.textFormat_CustomColorsChanged);
			// 
			// textFormat_TimeDuration
			// 
			this.textFormat_TimeDuration.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textFormat_TimeDuration.FormatColor = System.Drawing.Color.Black;
			this.textFormat_TimeDuration.FormatFont = new System.Drawing.Font("DejaVu Sans Mono", 8.25F);
			this.textFormat_TimeDuration.Location = new System.Drawing.Point(396, 235);
			this.textFormat_TimeDuration.Name = "textFormat_TimeDuration";
			this.textFormat_TimeDuration.Size = new System.Drawing.Size(232, 23);
			this.textFormat_TimeDuration.TabIndex = 24;
			this.textFormat_TimeDuration.Tag = "7";
			this.textFormat_TimeDuration.FormatChanged += new System.EventHandler(this.textFormat_FormatChanged);
			this.textFormat_TimeDuration.CustomColorsChanged += new System.EventHandler(this.textFormat_CustomColorsChanged);
			// 
			// textFormat_RxControl
			// 
			this.textFormat_RxControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textFormat_RxControl.FormatColor = System.Drawing.Color.Black;
			this.textFormat_RxControl.FormatFont = new System.Drawing.Font("DejaVu Sans Mono", 8.25F);
			this.textFormat_RxControl.Location = new System.Drawing.Point(396, 127);
			this.textFormat_RxControl.Name = "textFormat_RxControl";
			this.textFormat_RxControl.Size = new System.Drawing.Size(232, 23);
			this.textFormat_RxControl.TabIndex = 12;
			this.textFormat_RxControl.Tag = "3";
			this.textFormat_RxControl.FormatChanged += new System.EventHandler(this.textFormat_FormatChanged);
			this.textFormat_RxControl.CustomColorsChanged += new System.EventHandler(this.textFormat_CustomColorsChanged);
			// 
			// textFormat_RxData
			// 
			this.textFormat_RxData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textFormat_RxData.FormatColor = System.Drawing.Color.Black;
			this.textFormat_RxData.FormatFont = new System.Drawing.Font("DejaVu Sans Mono", 8.25F);
			this.textFormat_RxData.Location = new System.Drawing.Point(396, 100);
			this.textFormat_RxData.Name = "textFormat_RxData";
			this.textFormat_RxData.Size = new System.Drawing.Size(232, 23);
			this.textFormat_RxData.TabIndex = 9;
			this.textFormat_RxData.Tag = "2";
			this.textFormat_RxData.FormatChanged += new System.EventHandler(this.textFormat_FormatChanged);
			this.textFormat_RxData.CustomColorsChanged += new System.EventHandler(this.textFormat_CustomColorsChanged);
			// 
			// textFormat_TxControl
			// 
			this.textFormat_TxControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textFormat_TxControl.FormatColor = System.Drawing.Color.Black;
			this.textFormat_TxControl.FormatFont = new System.Drawing.Font("DejaVu Sans Mono", 8.25F);
			this.textFormat_TxControl.Location = new System.Drawing.Point(396, 73);
			this.textFormat_TxControl.Name = "textFormat_TxControl";
			this.textFormat_TxControl.Size = new System.Drawing.Size(232, 23);
			this.textFormat_TxControl.TabIndex = 6;
			this.textFormat_TxControl.Tag = "1";
			this.textFormat_TxControl.FormatChanged += new System.EventHandler(this.textFormat_FormatChanged);
			this.textFormat_TxControl.CustomColorsChanged += new System.EventHandler(this.textFormat_CustomColorsChanged);
			// 
			// textFormat_TxData
			// 
			this.textFormat_TxData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textFormat_TxData.FormatColor = System.Drawing.Color.Black;
			this.textFormat_TxData.FormatFont = new System.Drawing.Font("DejaVu Sans Mono", 8.25F);
			this.textFormat_TxData.Location = new System.Drawing.Point(396, 46);
			this.textFormat_TxData.Name = "textFormat_TxData";
			this.textFormat_TxData.Size = new System.Drawing.Size(232, 23);
			this.textFormat_TxData.TabIndex = 3;
			this.textFormat_TxData.Tag = "0";
			this.textFormat_TxData.FormatChanged += new System.EventHandler(this.textFormat_FormatChanged);
			this.textFormat_TxData.CustomColorsChanged += new System.EventHandler(this.textFormat_CustomColorsChanged);
			// 
			// button_Font
			// 
			this.button_Font.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Font.Location = new System.Drawing.Point(643, 235);
			this.button_Font.Name = "button_Font";
			this.button_Font.Size = new System.Drawing.Size(75, 23);
			this.button_Font.TabIndex = 50;
			this.button_Font.Text = "&Font...";
			this.button_Font.UseVisualStyleBackColor = true;
			this.button_Font.Click += new System.EventHandler(this.button_Font_Click);
			// 
			// button_Defaults
			// 
			this.button_Defaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Defaults.Location = new System.Drawing.Point(765, 247);
			this.button_Defaults.Name = "button_Defaults";
			this.button_Defaults.Size = new System.Drawing.Size(75, 23);
			this.button_Defaults.TabIndex = 6;
			this.button_Defaults.Text = "Defa&ults...";
			this.button_Defaults.UseVisualStyleBackColor = true;
			this.button_Defaults.Click += new System.EventHandler(this.button_Defaults_Click);
			// 
			// label_Example
			// 
			this.label_Example.AutoSize = true;
			this.label_Example.Location = new System.Drawing.Point(24, 738);
			this.label_Example.Name = "label_Example";
			this.label_Example.Size = new System.Drawing.Size(50, 13);
			this.label_Example.TabIndex = 2;
			this.label_Example.Text = "Example:";
			// 
			// groupBox_Options
			// 
			this.groupBox_Options.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Options.Controls.Add(this.comboBox_ContentSeparator);
			this.groupBox_Options.Controls.Add(this.label_ContentSeparator);
			this.groupBox_Options.Controls.Add(this.comboBox_InfoSeparator);
			this.groupBox_Options.Controls.Add(this.comboBox_InfoEnclosure);
			this.groupBox_Options.Controls.Add(this.textBox_TimeDurationFormat);
			this.groupBox_Options.Controls.Add(this.checkBox_TimeStampUseUtc);
			this.groupBox_Options.Controls.Add(this.label_Reference);
			this.groupBox_Options.Controls.Add(this.label_Presets);
			this.groupBox_Options.Controls.Add(this.linkLabel_TimeSpanFormat);
			this.groupBox_Options.Controls.Add(this.linkLabel_DateTimeFormat);
			this.groupBox_Options.Controls.Add(this.label_TimeStampFormat);
			this.groupBox_Options.Controls.Add(this.comboBox_TimeStampFormatPreset);
			this.groupBox_Options.Controls.Add(this.textBox_TimeStampFormat);
			this.groupBox_Options.Controls.Add(this.label_TimeSpanFormat);
			this.groupBox_Options.Controls.Add(this.comboBox_TimeSpanFormatPreset);
			this.groupBox_Options.Controls.Add(this.textBox_TimeSpanFormat);
			this.groupBox_Options.Controls.Add(this.label_TimeDeltaFormat);
			this.groupBox_Options.Controls.Add(this.comboBox_TimeDeltaFormatPreset);
			this.groupBox_Options.Controls.Add(this.textBox_TimeDeltaFormat);
			this.groupBox_Options.Controls.Add(this.label_TimeDurationFormat);
			this.groupBox_Options.Controls.Add(this.comboBox_TimeDurationFormatPreset);
			this.groupBox_Options.Controls.Add(this.label_InfoEnclosure);
			this.groupBox_Options.Controls.Add(this.label_InfoSeparator);
			this.groupBox_Options.Location = new System.Drawing.Point(12, 502);
			this.groupBox_Options.Name = "groupBox_Options";
			this.groupBox_Options.Size = new System.Drawing.Size(736, 216);
			this.groupBox_Options.TabIndex = 1;
			this.groupBox_Options.TabStop = false;
			this.groupBox_Options.Text = "Options";
			// 
			// comboBox_ContentSeparator
			// 
			this.comboBox_ContentSeparator.FormattingEnabled = true;
			this.comboBox_ContentSeparator.Location = new System.Drawing.Point(111, 19);
			this.comboBox_ContentSeparator.Name = "comboBox_ContentSeparator";
			this.comboBox_ContentSeparator.Size = new System.Drawing.Size(175, 21);
			this.comboBox_ContentSeparator.TabIndex = 1;
			this.toolTip.SetToolTip(this.comboBox_ContentSeparator, "The separator between adjacent content elements.");
			this.comboBox_ContentSeparator.TextChanged += new System.EventHandler(this.comboBox_ContentSeparator_TextChanged);
			this.comboBox_ContentSeparator.Validating += new System.ComponentModel.CancelEventHandler(this.comboBox_ContentSeparator_Validating);
			// 
			// label_ContentSeparator
			// 
			this.label_ContentSeparator.AutoSize = true;
			this.label_ContentSeparator.Location = new System.Drawing.Point(12, 22);
			this.label_ContentSeparator.Name = "label_ContentSeparator";
			this.label_ContentSeparator.Size = new System.Drawing.Size(96, 13);
			this.label_ContentSeparator.TabIndex = 0;
			this.label_ContentSeparator.Text = "Content &Separator:";
			// 
			// comboBox_InfoSeparator
			// 
			this.comboBox_InfoSeparator.FormattingEnabled = true;
			this.comboBox_InfoSeparator.Location = new System.Drawing.Point(111, 46);
			this.comboBox_InfoSeparator.Name = "comboBox_InfoSeparator";
			this.comboBox_InfoSeparator.Size = new System.Drawing.Size(175, 21);
			this.comboBox_InfoSeparator.TabIndex = 3;
			this.toolTip.SetToolTip(this.comboBox_InfoSeparator, "The separator between adjacent informational elements.");
			this.comboBox_InfoSeparator.TextChanged += new System.EventHandler(this.comboBox_InfoSeparator_TextChanged);
			this.comboBox_InfoSeparator.Validating += new System.ComponentModel.CancelEventHandler(this.comboBox_InfoSeparator_Validating);
			// 
			// comboBox_InfoEnclosure
			// 
			this.comboBox_InfoEnclosure.FormattingEnabled = true;
			this.comboBox_InfoEnclosure.Location = new System.Drawing.Point(111, 73);
			this.comboBox_InfoEnclosure.Name = "comboBox_InfoEnclosure";
			this.comboBox_InfoEnclosure.Size = new System.Drawing.Size(175, 21);
			this.comboBox_InfoEnclosure.TabIndex = 5;
			this.toolTip.SetToolTip(this.comboBox_InfoEnclosure, "The enclosure of informational elements.");
			this.comboBox_InfoEnclosure.TextChanged += new System.EventHandler(this.comboBox_InfoEnclosure_TextChanged);
			this.comboBox_InfoEnclosure.Validating += new System.ComponentModel.CancelEventHandler(this.comboBox_InfoEnclosure_Validating);
			// 
			// textBox_TimeDurationFormat
			// 
			this.textBox_TimeDurationFormat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox_TimeDurationFormat.Enabled = false;
			this.textBox_TimeDurationFormat.Location = new System.Drawing.Point(111, 181);
			this.textBox_TimeDurationFormat.Name = "textBox_TimeDurationFormat";
			this.textBox_TimeDurationFormat.Size = new System.Drawing.Size(275, 20);
			this.textBox_TimeDurationFormat.TabIndex = 18;
			this.textBox_TimeDurationFormat.TextChanged += new System.EventHandler(this.textBox_TimeDurationFormat_TextChanged);
			this.textBox_TimeDurationFormat.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_TimeDurationFormat_Validating);
			// 
			// checkBox_TimeStampUseUtc
			// 
			this.checkBox_TimeStampUseUtc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBox_TimeStampUseUtc.AutoSize = true;
			this.checkBox_TimeStampUseUtc.Location = new System.Drawing.Point(373, 77);
			this.checkBox_TimeStampUseUtc.Name = "checkBox_TimeStampUseUtc";
			this.checkBox_TimeStampUseUtc.Size = new System.Drawing.Size(70, 17);
			this.checkBox_TimeStampUseUtc.TabIndex = 8;
			this.checkBox_TimeStampUseUtc.Text = "Use &UTC";
			this.checkBox_TimeStampUseUtc.UseVisualStyleBackColor = true;
			this.checkBox_TimeStampUseUtc.CheckedChanged += new System.EventHandler(this.checkBox_TimeStampUseUtc_CheckedChanged);
			// 
			// label_Reference
			// 
			this.label_Reference.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label_Reference.AutoSize = true;
			this.label_Reference.Location = new System.Drawing.Point(649, 78);
			this.label_Reference.Name = "label_Reference";
			this.label_Reference.Size = new System.Drawing.Size(60, 13);
			this.label_Reference.TabIndex = 20;
			this.label_Reference.Text = "Reference:";
			// 
			// label_Presets
			// 
			this.label_Presets.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label_Presets.AutoSize = true;
			this.label_Presets.Location = new System.Drawing.Point(489, 78);
			this.label_Presets.Name = "label_Presets";
			this.label_Presets.Size = new System.Drawing.Size(45, 13);
			this.label_Presets.TabIndex = 9;
			this.label_Presets.Text = "Presets:";
			// 
			// linkLabel_TimeSpanFormat
			// 
			this.linkLabel_TimeSpanFormat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.linkLabel_TimeSpanFormat.AutoSize = true;
			this.linkLabel_TimeSpanFormat.Location = new System.Drawing.Point(636, 150);
			this.linkLabel_TimeSpanFormat.Name = "linkLabel_TimeSpanFormat";
			this.linkLabel_TimeSpanFormat.Size = new System.Drawing.Size(93, 26);
			this.linkLabel_TimeSpanFormat.TabIndex = 22;
			this.linkLabel_TimeSpanFormat.TabStop = true;
			this.linkLabel_TimeSpanFormat.Text = ".NET\r\nTime Span Format";
			this.linkLabel_TimeSpanFormat.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.linkLabel_TimeSpanFormat.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_TimeSpanFormat_LinkClicked);
			// 
			// linkLabel_DateTimeFormat
			// 
			this.linkLabel_DateTimeFormat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.linkLabel_DateTimeFormat.AutoSize = true;
			this.linkLabel_DateTimeFormat.Location = new System.Drawing.Point(636, 96);
			this.linkLabel_DateTimeFormat.Name = "linkLabel_DateTimeFormat";
			this.linkLabel_DateTimeFormat.Size = new System.Drawing.Size(93, 26);
			this.linkLabel_DateTimeFormat.TabIndex = 21;
			this.linkLabel_DateTimeFormat.TabStop = true;
			this.linkLabel_DateTimeFormat.Text = ".NET\r\nDate/Time Format";
			this.linkLabel_DateTimeFormat.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.linkLabel_DateTimeFormat.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_DateTimeFormat_LinkClicked);
			// 
			// label_TimeStampFormat
			// 
			this.label_TimeStampFormat.AutoSize = true;
			this.label_TimeStampFormat.Location = new System.Drawing.Point(12, 103);
			this.label_TimeStampFormat.Name = "label_TimeStampFormat";
			this.label_TimeStampFormat.Size = new System.Drawing.Size(66, 13);
			this.label_TimeStampFormat.TabIndex = 6;
			this.label_TimeStampFormat.Text = "T&ime Stamp:";
			// 
			// comboBox_TimeStampFormatPreset
			// 
			this.comboBox_TimeStampFormatPreset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_TimeStampFormatPreset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_TimeStampFormatPreset.FormattingEnabled = true;
			this.comboBox_TimeStampFormatPreset.Location = new System.Drawing.Point(396, 100);
			this.comboBox_TimeStampFormatPreset.Name = "comboBox_TimeStampFormatPreset";
			this.comboBox_TimeStampFormatPreset.Size = new System.Drawing.Size(232, 21);
			this.comboBox_TimeStampFormatPreset.TabIndex = 10;
			this.comboBox_TimeStampFormatPreset.SelectedIndexChanged += new System.EventHandler(this.comboBox_TimeStampFormatPreset_SelectedIndexChanged);
			// 
			// textBox_TimeStampFormat
			// 
			this.textBox_TimeStampFormat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox_TimeStampFormat.Location = new System.Drawing.Point(111, 100);
			this.textBox_TimeStampFormat.Name = "textBox_TimeStampFormat";
			this.textBox_TimeStampFormat.Size = new System.Drawing.Size(275, 20);
			this.textBox_TimeStampFormat.TabIndex = 7;
			this.textBox_TimeStampFormat.TextChanged += new System.EventHandler(this.textBox_TimeStampFormat_TextChanged);
			this.textBox_TimeStampFormat.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_TimeStampFormat_Validating);
			// 
			// label_TimeSpanFormat
			// 
			this.label_TimeSpanFormat.AutoSize = true;
			this.label_TimeSpanFormat.Location = new System.Drawing.Point(12, 130);
			this.label_TimeSpanFormat.Name = "label_TimeSpanFormat";
			this.label_TimeSpanFormat.Size = new System.Drawing.Size(61, 13);
			this.label_TimeSpanFormat.TabIndex = 11;
			this.label_TimeSpanFormat.Text = "Ti&me Span:";
			this.toolTip.SetToolTip(this.label_TimeSpanFormat, "Currently limited to the standard format. Will become available as soon as update" +
        "d to .NET 4+ which supports time span formats.");
			// 
			// comboBox_TimeSpanFormatPreset
			// 
			this.comboBox_TimeSpanFormatPreset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_TimeSpanFormatPreset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_TimeSpanFormatPreset.FormattingEnabled = true;
			this.comboBox_TimeSpanFormatPreset.Location = new System.Drawing.Point(396, 127);
			this.comboBox_TimeSpanFormatPreset.Name = "comboBox_TimeSpanFormatPreset";
			this.comboBox_TimeSpanFormatPreset.Size = new System.Drawing.Size(232, 21);
			this.comboBox_TimeSpanFormatPreset.TabIndex = 13;
			this.toolTip.SetToolTip(this.comboBox_TimeSpanFormatPreset, "Format is yet limited to a few typical presets. Contact YAT\r\nvia \"Help > Request " +
        "Feature\" to request additional presets.\r\n\r\nCustom format will become available w" +
        "ith feature request #377.");
			this.comboBox_TimeSpanFormatPreset.SelectedIndexChanged += new System.EventHandler(this.comboBox_TimeSpanFormatPreset_SelectedIndexChanged);
			// 
			// textBox_TimeSpanFormat
			// 
			this.textBox_TimeSpanFormat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox_TimeSpanFormat.Enabled = false;
			this.textBox_TimeSpanFormat.Location = new System.Drawing.Point(111, 127);
			this.textBox_TimeSpanFormat.Name = "textBox_TimeSpanFormat";
			this.textBox_TimeSpanFormat.Size = new System.Drawing.Size(275, 20);
			this.textBox_TimeSpanFormat.TabIndex = 12;
			this.textBox_TimeSpanFormat.TextChanged += new System.EventHandler(this.textBox_TimeSpanFormat_TextChanged);
			this.textBox_TimeSpanFormat.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_TimeSpanFormat_Validating);
			// 
			// label_TimeDeltaFormat
			// 
			this.label_TimeDeltaFormat.AutoSize = true;
			this.label_TimeDeltaFormat.Location = new System.Drawing.Point(12, 157);
			this.label_TimeDeltaFormat.Name = "label_TimeDeltaFormat";
			this.label_TimeDeltaFormat.Size = new System.Drawing.Size(61, 13);
			this.label_TimeDeltaFormat.TabIndex = 14;
			this.label_TimeDeltaFormat.Text = "Tim&e Delta:";
			this.toolTip.SetToolTip(this.label_TimeDeltaFormat, "Currently limited to the standard format. Will become available as soon as update" +
        "d to .NET 4+ which supports time span formats.");
			// 
			// comboBox_TimeDeltaFormatPreset
			// 
			this.comboBox_TimeDeltaFormatPreset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_TimeDeltaFormatPreset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_TimeDeltaFormatPreset.FormattingEnabled = true;
			this.comboBox_TimeDeltaFormatPreset.Location = new System.Drawing.Point(396, 154);
			this.comboBox_TimeDeltaFormatPreset.Name = "comboBox_TimeDeltaFormatPreset";
			this.comboBox_TimeDeltaFormatPreset.Size = new System.Drawing.Size(232, 21);
			this.comboBox_TimeDeltaFormatPreset.TabIndex = 16;
			this.toolTip.SetToolTip(this.comboBox_TimeDeltaFormatPreset, "Format is yet limited to a few typical presets. Contact YAT\r\nvia \"Help > Request " +
        "Feature\" to request additional presets.\r\n\r\nCustom format will become available w" +
        "ith feature request #377.");
			this.comboBox_TimeDeltaFormatPreset.SelectedIndexChanged += new System.EventHandler(this.comboBox_TimeDeltaFormatPreset_SelectedIndexChanged);
			// 
			// textBox_TimeDeltaFormat
			// 
			this.textBox_TimeDeltaFormat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox_TimeDeltaFormat.Enabled = false;
			this.textBox_TimeDeltaFormat.Location = new System.Drawing.Point(111, 154);
			this.textBox_TimeDeltaFormat.Name = "textBox_TimeDeltaFormat";
			this.textBox_TimeDeltaFormat.Size = new System.Drawing.Size(275, 20);
			this.textBox_TimeDeltaFormat.TabIndex = 15;
			this.textBox_TimeDeltaFormat.TextChanged += new System.EventHandler(this.textBox_TimeDeltaFormat_TextChanged);
			this.textBox_TimeDeltaFormat.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_TimeDeltaFormat_Validating);
			// 
			// label_TimeDurationFormat
			// 
			this.label_TimeDurationFormat.AutoSize = true;
			this.label_TimeDurationFormat.Location = new System.Drawing.Point(12, 184);
			this.label_TimeDurationFormat.Name = "label_TimeDurationFormat";
			this.label_TimeDurationFormat.Size = new System.Drawing.Size(76, 13);
			this.label_TimeDurationFormat.TabIndex = 17;
			this.label_TimeDurationFormat.Text = "Time D&uration:";
			this.toolTip.SetToolTip(this.label_TimeDurationFormat, "Currently limited to the standard format. Will become available as soon as update" +
        "d to .NET 4+ which supports time span formats.");
			// 
			// comboBox_TimeDurationFormatPreset
			// 
			this.comboBox_TimeDurationFormatPreset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_TimeDurationFormatPreset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_TimeDurationFormatPreset.FormattingEnabled = true;
			this.comboBox_TimeDurationFormatPreset.Location = new System.Drawing.Point(396, 181);
			this.comboBox_TimeDurationFormatPreset.Name = "comboBox_TimeDurationFormatPreset";
			this.comboBox_TimeDurationFormatPreset.Size = new System.Drawing.Size(232, 21);
			this.comboBox_TimeDurationFormatPreset.TabIndex = 19;
			this.toolTip.SetToolTip(this.comboBox_TimeDurationFormatPreset, "Format is yet limited to a few typical presets. Contact YAT\r\nvia \"Help > Request " +
        "Feature\" to request additional presets.\r\n\r\nCustom format will become available w" +
        "ith feature request #377.");
			this.comboBox_TimeDurationFormatPreset.SelectedIndexChanged += new System.EventHandler(this.comboBox_TimeDurationFormatPreset_SelectedIndexChanged);
			// 
			// label_InfoEnclosure
			// 
			this.label_InfoEnclosure.AutoSize = true;
			this.label_InfoEnclosure.Location = new System.Drawing.Point(12, 76);
			this.label_InfoEnclosure.Name = "label_InfoEnclosure";
			this.label_InfoEnclosure.Size = new System.Drawing.Size(78, 13);
			this.label_InfoEnclosure.TabIndex = 4;
			this.label_InfoEnclosure.Text = "Info E&nclosure:";
			// 
			// label_InfoSeparator
			// 
			this.label_InfoSeparator.AutoSize = true;
			this.label_InfoSeparator.Location = new System.Drawing.Point(12, 49);
			this.label_InfoSeparator.Name = "label_InfoSeparator";
			this.label_InfoSeparator.Size = new System.Drawing.Size(77, 13);
			this.label_InfoSeparator.TabIndex = 2;
			this.label_InfoSeparator.Text = "Info Se&parator:";
			// 
			// monitor_Example
			// 
			this.monitor_Example.ActiveConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_Example.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.monitor_Example.Location = new System.Drawing.Point(123, 733);
			this.monitor_Example.Name = "monitor_Example";
			this.monitor_Example.ShowStatusPanel = false;
			this.monitor_Example.Size = new System.Drawing.Size(625, 63);
			this.monitor_Example.TabIndex = 3;
			this.monitor_Example.TabStop = false;
			this.monitor_Example.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			// 
			// FormatSettings
			// 
			this.AcceptButton = this.button_OK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button_Cancel;
			this.ClientSize = new System.Drawing.Size(855, 811);
			this.Controls.Add(this.groupBox_Options);
			this.Controls.Add(this.monitor_Example);
			this.Controls.Add(this.label_Example);
			this.Controls.Add(this.button_Defaults);
			this.Controls.Add(this.groupBox_Elements);
			this.Controls.Add(this.button_Cancel);
			this.Controls.Add(this.button_OK);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(871, 850);
			this.Name = "FormatSettings";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Format Settings";
			this.Deactivate += new System.EventHandler(this.FormatSettings_Deactivate);
			this.Shown += new System.EventHandler(this.FormatSettings_Shown);
			this.groupBox_Elements.ResumeLayout(false);
			this.groupBox_Elements.PerformLayout();
			this.groupBox_Options.ResumeLayout(false);
			this.groupBox_Options.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox_Elements;
		private YAT.View.Controls.TextFormat textFormat_Error;
		private YAT.View.Controls.TextFormat textFormat_Length;
		private YAT.View.Controls.TextFormat textFormat_TimeSpan;
		private YAT.View.Controls.TextFormat textFormat_TimeDelta;
		private YAT.View.Controls.TextFormat textFormat_TimeDuration;
		private YAT.View.Controls.TextFormat textFormat_RxControl;
		private YAT.View.Controls.TextFormat textFormat_RxData;
		private YAT.View.Controls.TextFormat textFormat_TxControl;
		private YAT.View.Controls.TextFormat textFormat_TxData;
		private System.Windows.Forms.Label label_Example;
		private System.Windows.Forms.Button button_Font;
		private System.Windows.Forms.Button button_Defaults;
		private System.Windows.Forms.Button button_Cancel;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.Label label_TxData;
		private System.Windows.Forms.Label label_Error;
		private System.Windows.Forms.Label label_Length;
		private System.Windows.Forms.Label label_TimeSpan;
		private System.Windows.Forms.Label label_TimeDelta;
		private System.Windows.Forms.Label label_TimeDuration;
		private System.Windows.Forms.Label label_RxControl;
		private System.Windows.Forms.Label label_RxData;
		private System.Windows.Forms.Label label_TxControl;
		private YAT.View.Controls.Monitor monitor_Example;
		private YAT.View.Controls.Monitor monitor_Error;
		private YAT.View.Controls.Monitor monitor_Length;
		private YAT.View.Controls.Monitor monitor_TimeSpan;
		private YAT.View.Controls.Monitor monitor_TimeDelta;
		private YAT.View.Controls.Monitor monitor_TimeDuration;
		private YAT.View.Controls.Monitor monitor_RxControl;
		private YAT.View.Controls.Monitor monitor_RxData;
		private YAT.View.Controls.Monitor monitor_TxControl;
		private YAT.View.Controls.Monitor monitor_TxData;
		private System.Windows.Forms.Label label_Direction;
		private Controls.TextFormat textFormat_Direction;
		private Controls.Monitor monitor_Direction;
		private Controls.TextFormat textFormat_TimeStamp;
		private Controls.Monitor monitor_TimeStamp;
		private System.Windows.Forms.Label label_Date;
		private System.Windows.Forms.Button button_Background;
		private System.Windows.Forms.Label label_Remark2;
		private System.Windows.Forms.Label label_Remark1;
		private Controls.TextFormat textFormat_Device;
		private Controls.Monitor monitor_Device;
		private System.Windows.Forms.Label label_Device;
		private System.Windows.Forms.GroupBox groupBox_Options;
		private System.Windows.Forms.Label label_InfoEnclosure;
		private System.Windows.Forms.Label label_InfoSeparator;
		private MKY.Windows.Forms.ComboBoxEx comboBox_InfoEnclosure;
		private MKY.Windows.Forms.ComboBoxEx comboBox_InfoSeparator;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.ColorDialog colorDialog;
		private System.Windows.Forms.CheckBox checkBox_EnableFormatting;
		private System.Windows.Forms.Label label_Remark3;
		private MKY.Windows.Forms.TextBoxEx textBox_TimeStampFormat;
		private System.Windows.Forms.ComboBox comboBox_TimeStampFormatPreset;
		private System.Windows.Forms.Label label_TimeStampFormat;
		private MKY.Windows.Forms.TextBoxEx textBox_TimeSpanFormat;
		private System.Windows.Forms.ComboBox comboBox_TimeSpanFormatPreset;
		private System.Windows.Forms.Label label_TimeSpanFormat;
		private MKY.Windows.Forms.TextBoxEx textBox_TimeDeltaFormat;
		private System.Windows.Forms.ComboBox comboBox_TimeDeltaFormatPreset;
		private System.Windows.Forms.Label label_TimeDeltaFormat;
		private MKY.Windows.Forms.TextBoxEx textBox_TimeDurationFormat;
		private System.Windows.Forms.ComboBox comboBox_TimeDurationFormatPreset;
		private System.Windows.Forms.Label label_TimeDurationFormat;
		private System.Windows.Forms.LinkLabel linkLabel_DateTimeFormat;
		private System.Windows.Forms.LinkLabel linkLabel_TimeSpanFormat;
		private System.Windows.Forms.Label label_Reference;
		private System.Windows.Forms.Label label_Presets;
		private System.Windows.Forms.CheckBox checkBox_TimeStampUseUtc;
		private Controls.Monitor monitor_IOControl;
		private System.Windows.Forms.Label label_IOControl;
		private Controls.TextFormat textFormat_IOControl;
		private MKY.Windows.Forms.ComboBoxEx comboBox_ContentSeparator;
		private System.Windows.Forms.Label label_ContentSeparator;
		private System.Windows.Forms.Label label_Separator;
		private Controls.TextFormat textFormat_Separator;
		private Controls.Monitor monitor_Separator;
		private Controls.Monitor monitor_Warning;
		private System.Windows.Forms.Label label_Warning;
		private Controls.TextFormat textFormat_Warning;
	}
}
