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
			this.textFormat_Port = new YAT.View.Controls.TextFormat();
			this.monitor_Port = new YAT.View.Controls.Monitor();
			this.label_Port = new System.Windows.Forms.Label();
			this.label_FontRemark2 = new System.Windows.Forms.Label();
			this.label_FontRemark1 = new System.Windows.Forms.Label();
			this.button_Background = new System.Windows.Forms.Button();
			this.textFormat_Date = new YAT.View.Controls.TextFormat();
			this.monitor_Date = new YAT.View.Controls.Monitor();
			this.label_Date = new System.Windows.Forms.Label();
			this.textFormat_Direction = new YAT.View.Controls.TextFormat();
			this.monitor_Direction = new YAT.View.Controls.Monitor();
			this.label_Direction = new System.Windows.Forms.Label();
			this.monitor_Error = new YAT.View.Controls.Monitor();
			this.monitor_Length = new YAT.View.Controls.Monitor();
			this.monitor_Time = new YAT.View.Controls.Monitor();
			this.monitor_RxControl = new YAT.View.Controls.Monitor();
			this.monitor_RxData = new YAT.View.Controls.Monitor();
			this.monitor_TxControl = new YAT.View.Controls.Monitor();
			this.monitor_TxData = new YAT.View.Controls.Monitor();
			this.label_Error = new System.Windows.Forms.Label();
			this.label_Length = new System.Windows.Forms.Label();
			this.label_Time = new System.Windows.Forms.Label();
			this.label_RxControl = new System.Windows.Forms.Label();
			this.label_RxData = new System.Windows.Forms.Label();
			this.label_TxControl = new System.Windows.Forms.Label();
			this.label_TxData = new System.Windows.Forms.Label();
			this.textFormat_Error = new YAT.View.Controls.TextFormat();
			this.textFormat_Length = new YAT.View.Controls.TextFormat();
			this.textFormat_Time = new YAT.View.Controls.TextFormat();
			this.textFormat_RxControl = new YAT.View.Controls.TextFormat();
			this.textFormat_RxData = new YAT.View.Controls.TextFormat();
			this.textFormat_TxControl = new YAT.View.Controls.TextFormat();
			this.textFormat_TxData = new YAT.View.Controls.TextFormat();
			this.button_Font = new System.Windows.Forms.Button();
			this.button_Defaults = new System.Windows.Forms.Button();
			this.label_Example = new System.Windows.Forms.Label();
			this.monitor_Example = new YAT.View.Controls.Monitor();
			this.groupBox_Options = new System.Windows.Forms.GroupBox();
			this.label_InfoEnclosure = new System.Windows.Forms.Label();
			this.label_InfoSeparator = new System.Windows.Forms.Label();
			this.comboBox_InfoEnclosure = new System.Windows.Forms.ComboBox();
			this.comboBox_InfoSeparator = new System.Windows.Forms.ComboBox();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.groupBox_Elements.SuspendLayout();
			this.groupBox_Options.SuspendLayout();
			this.SuspendLayout();
			// 
			// button_Cancel
			// 
			this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button_Cancel.Location = new System.Drawing.Point(585, 60);
			this.button_Cancel.Name = "button_Cancel";
			this.button_Cancel.Size = new System.Drawing.Size(75, 23);
			this.button_Cancel.TabIndex = 5;
			this.button_Cancel.Text = "Cancel";
			this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
			// 
			// button_OK
			// 
			this.button_OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button_OK.Location = new System.Drawing.Point(585, 31);
			this.button_OK.Name = "button_OK";
			this.button_OK.Size = new System.Drawing.Size(75, 23);
			this.button_OK.TabIndex = 4;
			this.button_OK.Text = "OK";
			this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
			// 
			// groupBox_Elements
			// 
			this.groupBox_Elements.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Elements.Controls.Add(this.textFormat_Port);
			this.groupBox_Elements.Controls.Add(this.monitor_Port);
			this.groupBox_Elements.Controls.Add(this.label_Port);
			this.groupBox_Elements.Controls.Add(this.label_FontRemark2);
			this.groupBox_Elements.Controls.Add(this.label_FontRemark1);
			this.groupBox_Elements.Controls.Add(this.button_Background);
			this.groupBox_Elements.Controls.Add(this.textFormat_Date);
			this.groupBox_Elements.Controls.Add(this.monitor_Date);
			this.groupBox_Elements.Controls.Add(this.label_Date);
			this.groupBox_Elements.Controls.Add(this.textFormat_Direction);
			this.groupBox_Elements.Controls.Add(this.monitor_Direction);
			this.groupBox_Elements.Controls.Add(this.label_Direction);
			this.groupBox_Elements.Controls.Add(this.monitor_Error);
			this.groupBox_Elements.Controls.Add(this.monitor_Length);
			this.groupBox_Elements.Controls.Add(this.monitor_Time);
			this.groupBox_Elements.Controls.Add(this.monitor_RxControl);
			this.groupBox_Elements.Controls.Add(this.monitor_RxData);
			this.groupBox_Elements.Controls.Add(this.monitor_TxControl);
			this.groupBox_Elements.Controls.Add(this.monitor_TxData);
			this.groupBox_Elements.Controls.Add(this.label_Error);
			this.groupBox_Elements.Controls.Add(this.label_Length);
			this.groupBox_Elements.Controls.Add(this.label_Time);
			this.groupBox_Elements.Controls.Add(this.label_RxControl);
			this.groupBox_Elements.Controls.Add(this.label_RxData);
			this.groupBox_Elements.Controls.Add(this.label_TxControl);
			this.groupBox_Elements.Controls.Add(this.label_TxData);
			this.groupBox_Elements.Controls.Add(this.textFormat_Error);
			this.groupBox_Elements.Controls.Add(this.textFormat_Length);
			this.groupBox_Elements.Controls.Add(this.textFormat_Time);
			this.groupBox_Elements.Controls.Add(this.textFormat_RxControl);
			this.groupBox_Elements.Controls.Add(this.textFormat_RxData);
			this.groupBox_Elements.Controls.Add(this.textFormat_TxControl);
			this.groupBox_Elements.Controls.Add(this.textFormat_TxData);
			this.groupBox_Elements.Controls.Add(this.button_Font);
			this.groupBox_Elements.Location = new System.Drawing.Point(12, 12);
			this.groupBox_Elements.Name = "groupBox_Elements";
			this.groupBox_Elements.Size = new System.Drawing.Size(556, 341);
			this.groupBox_Elements.TabIndex = 0;
			this.groupBox_Elements.TabStop = false;
			this.groupBox_Elements.Text = "Elements";
			// 
			// textFormat_Port
			// 
			this.textFormat_Port.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textFormat_Port.FormatColor = System.Drawing.Color.Black;
			this.textFormat_Port.FormatFont = new System.Drawing.Font("DejaVu Sans Mono", 8.25F);
			this.textFormat_Port.Location = new System.Drawing.Point(216, 193);
			this.textFormat_Port.Name = "textFormat_Port";
			this.textFormat_Port.Size = new System.Drawing.Size(232, 23);
			this.textFormat_Port.TabIndex = 20;
			this.textFormat_Port.Tag = "6";
			this.textFormat_Port.FormatChanged += new System.EventHandler(this.textFormat_FormatChanged);
			// 
			// monitor_Port
			// 
			this.monitor_Port.ActiveConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_Port.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.monitor_Port.Location = new System.Drawing.Point(88, 193);
			this.monitor_Port.Name = "monitor_Port";
			this.monitor_Port.Size = new System.Drawing.Size(127, 23);
			this.monitor_Port.TabIndex = 19;
			this.monitor_Port.TabStop = false;
			this.monitor_Port.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			// 
			// label_Port
			// 
			this.label_Port.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label_Port.AutoSize = true;
			this.label_Port.Location = new System.Drawing.Point(12, 198);
			this.label_Port.Name = "label_Port";
			this.label_Port.Size = new System.Drawing.Size(29, 13);
			this.label_Port.TabIndex = 18;
			this.label_Port.Text = "&Port:";
			// 
			// label_FontRemark2
			// 
			this.label_FontRemark2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label_FontRemark2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_FontRemark2.ForeColor = System.Drawing.SystemColors.GrayText;
			this.label_FontRemark2.Location = new System.Drawing.Point(454, 231);
			this.label_FontRemark2.Name = "label_FontRemark2";
			this.label_FontRemark2.Size = new System.Drawing.Size(96, 63);
			this.label_FontRemark2.TabIndex = 33;
			this.label_FontRemark2.Text = "Style and color separately for each format.";
			this.label_FontRemark2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label_FontRemark1
			// 
			this.label_FontRemark1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_FontRemark1.ForeColor = System.Drawing.SystemColors.GrayText;
			this.label_FontRemark1.Location = new System.Drawing.Point(454, 176);
			this.label_FontRemark1.Name = "label_FontRemark1";
			this.label_FontRemark1.Size = new System.Drawing.Size(96, 26);
			this.label_FontRemark1.TabIndex = 32;
			this.label_FontRemark1.Text = "Font and size\r\nfor all formats.";
			this.label_FontRemark1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// button_Background
			// 
			this.button_Background.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.button_Background.Location = new System.Drawing.Point(312, 309);
			this.button_Background.Name = "button_Background";
			this.button_Background.Size = new System.Drawing.Size(136, 23);
			this.button_Background.TabIndex = 30;
			this.button_Background.Text = "Background Color...";
			this.button_Background.UseVisualStyleBackColor = true;
			this.button_Background.Click += new System.EventHandler(this.button_Background_Click);
			// 
			// textFormat_Date
			// 
			this.textFormat_Date.FormatColor = System.Drawing.Color.Black;
			this.textFormat_Date.FormatFont = new System.Drawing.Font("DejaVu Sans Mono", 8.25F);
			this.textFormat_Date.Location = new System.Drawing.Point(216, 135);
			this.textFormat_Date.Name = "textFormat_Date";
			this.textFormat_Date.Size = new System.Drawing.Size(232, 23);
			this.textFormat_Date.TabIndex = 14;
			this.textFormat_Date.Tag = "4";
			this.textFormat_Date.FormatChanged += new System.EventHandler(this.textFormat_FormatChanged);
			// 
			// monitor_Date
			// 
			this.monitor_Date.ActiveConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_Date.Location = new System.Drawing.Point(88, 135);
			this.monitor_Date.Name = "monitor_Date";
			this.monitor_Date.Size = new System.Drawing.Size(127, 23);
			this.monitor_Date.TabIndex = 13;
			this.monitor_Date.TabStop = false;
			this.monitor_Date.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			// 
			// label_Date
			// 
			this.label_Date.AutoSize = true;
			this.label_Date.Location = new System.Drawing.Point(12, 140);
			this.label_Date.Name = "label_Date";
			this.label_Date.Size = new System.Drawing.Size(33, 13);
			this.label_Date.TabIndex = 12;
			this.label_Date.Text = "D&ate:";
			// 
			// textFormat_Direction
			// 
			this.textFormat_Direction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textFormat_Direction.FormatColor = System.Drawing.Color.Black;
			this.textFormat_Direction.FormatFont = new System.Drawing.Font("DejaVu Sans Mono", 8.25F);
			this.textFormat_Direction.Location = new System.Drawing.Point(216, 222);
			this.textFormat_Direction.Name = "textFormat_Direction";
			this.textFormat_Direction.Size = new System.Drawing.Size(232, 23);
			this.textFormat_Direction.TabIndex = 23;
			this.textFormat_Direction.Tag = "7";
			this.textFormat_Direction.FormatChanged += new System.EventHandler(this.textFormat_FormatChanged);
			// 
			// monitor_Direction
			// 
			this.monitor_Direction.ActiveConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_Direction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.monitor_Direction.Location = new System.Drawing.Point(88, 222);
			this.monitor_Direction.Name = "monitor_Direction";
			this.monitor_Direction.Size = new System.Drawing.Size(127, 23);
			this.monitor_Direction.TabIndex = 22;
			this.monitor_Direction.TabStop = false;
			this.monitor_Direction.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			// 
			// label_Direction
			// 
			this.label_Direction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label_Direction.AutoSize = true;
			this.label_Direction.Location = new System.Drawing.Point(12, 227);
			this.label_Direction.Name = "label_Direction";
			this.label_Direction.Size = new System.Drawing.Size(52, 13);
			this.label_Direction.TabIndex = 21;
			this.label_Direction.Text = "&Direction:";
			// 
			// monitor_Error
			// 
			this.monitor_Error.ActiveConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_Error.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.monitor_Error.Location = new System.Drawing.Point(88, 280);
			this.monitor_Error.Name = "monitor_Error";
			this.monitor_Error.Size = new System.Drawing.Size(127, 23);
			this.monitor_Error.TabIndex = 28;
			this.monitor_Error.TabStop = false;
			this.monitor_Error.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			// 
			// monitor_Length
			// 
			this.monitor_Length.ActiveConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_Length.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.monitor_Length.Location = new System.Drawing.Point(88, 251);
			this.monitor_Length.Name = "monitor_Length";
			this.monitor_Length.Size = new System.Drawing.Size(127, 23);
			this.monitor_Length.TabIndex = 25;
			this.monitor_Length.TabStop = false;
			this.monitor_Length.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			// 
			// monitor_Time
			// 
			this.monitor_Time.ActiveConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_Time.Location = new System.Drawing.Point(88, 164);
			this.monitor_Time.Name = "monitor_Time";
			this.monitor_Time.Size = new System.Drawing.Size(127, 23);
			this.monitor_Time.TabIndex = 16;
			this.monitor_Time.TabStop = false;
			this.monitor_Time.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			// 
			// monitor_RxControl
			// 
			this.monitor_RxControl.ActiveConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_RxControl.Location = new System.Drawing.Point(88, 106);
			this.monitor_RxControl.Name = "monitor_RxControl";
			this.monitor_RxControl.Size = new System.Drawing.Size(127, 23);
			this.monitor_RxControl.TabIndex = 10;
			this.monitor_RxControl.TabStop = false;
			this.monitor_RxControl.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			// 
			// monitor_RxData
			// 
			this.monitor_RxData.ActiveConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_RxData.Location = new System.Drawing.Point(88, 77);
			this.monitor_RxData.Name = "monitor_RxData";
			this.monitor_RxData.Size = new System.Drawing.Size(127, 23);
			this.monitor_RxData.TabIndex = 7;
			this.monitor_RxData.TabStop = false;
			this.monitor_RxData.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			// 
			// monitor_TxControl
			// 
			this.monitor_TxControl.ActiveConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_TxControl.Location = new System.Drawing.Point(88, 48);
			this.monitor_TxControl.Name = "monitor_TxControl";
			this.monitor_TxControl.Size = new System.Drawing.Size(127, 23);
			this.monitor_TxControl.TabIndex = 4;
			this.monitor_TxControl.TabStop = false;
			this.monitor_TxControl.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			// 
			// monitor_TxData
			// 
			this.monitor_TxData.ActiveConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_TxData.Location = new System.Drawing.Point(88, 19);
			this.monitor_TxData.Name = "monitor_TxData";
			this.monitor_TxData.Size = new System.Drawing.Size(127, 23);
			this.monitor_TxData.TabIndex = 1;
			this.monitor_TxData.TabStop = false;
			this.monitor_TxData.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			// 
			// label_Error
			// 
			this.label_Error.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label_Error.AutoSize = true;
			this.label_Error.Location = new System.Drawing.Point(12, 285);
			this.label_Error.Name = "label_Error";
			this.label_Error.Size = new System.Drawing.Size(32, 13);
			this.label_Error.TabIndex = 27;
			this.label_Error.Text = "&Error:";
			// 
			// label_Length
			// 
			this.label_Length.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label_Length.AutoSize = true;
			this.label_Length.Location = new System.Drawing.Point(12, 256);
			this.label_Length.Name = "label_Length";
			this.label_Length.Size = new System.Drawing.Size(43, 13);
			this.label_Length.TabIndex = 24;
			this.label_Length.Text = "&Length:";
			// 
			// label_Time
			// 
			this.label_Time.AutoSize = true;
			this.label_Time.Location = new System.Drawing.Point(12, 169);
			this.label_Time.Name = "label_Time";
			this.label_Time.Size = new System.Drawing.Size(33, 13);
			this.label_Time.TabIndex = 15;
			this.label_Time.Text = "T&ime:";
			// 
			// label_RxControl
			// 
			this.label_RxControl.AutoSize = true;
			this.label_RxControl.Location = new System.Drawing.Point(12, 111);
			this.label_RxControl.Name = "label_RxControl";
			this.label_RxControl.Size = new System.Drawing.Size(59, 13);
			this.label_RxControl.TabIndex = 9;
			this.label_RxControl.Text = "Rx C&ontrol:";
			// 
			// label_RxData
			// 
			this.label_RxData.AutoSize = true;
			this.label_RxData.Location = new System.Drawing.Point(12, 82);
			this.label_RxData.Name = "label_RxData";
			this.label_RxData.Size = new System.Drawing.Size(49, 13);
			this.label_RxData.TabIndex = 6;
			this.label_RxData.Text = "&Rx Data:";
			// 
			// label_TxControl
			// 
			this.label_TxControl.AutoSize = true;
			this.label_TxControl.Location = new System.Drawing.Point(12, 53);
			this.label_TxControl.Name = "label_TxControl";
			this.label_TxControl.Size = new System.Drawing.Size(58, 13);
			this.label_TxControl.TabIndex = 3;
			this.label_TxControl.Text = "Tx &Control:";
			// 
			// label_TxData
			// 
			this.label_TxData.AutoSize = true;
			this.label_TxData.Location = new System.Drawing.Point(12, 24);
			this.label_TxData.Name = "label_TxData";
			this.label_TxData.Size = new System.Drawing.Size(48, 13);
			this.label_TxData.TabIndex = 0;
			this.label_TxData.Text = "&Tx Data:";
			// 
			// textFormat_Error
			// 
			this.textFormat_Error.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textFormat_Error.FormatColor = System.Drawing.Color.Black;
			this.textFormat_Error.FormatFont = new System.Drawing.Font("DejaVu Sans Mono", 8.25F);
			this.textFormat_Error.Location = new System.Drawing.Point(216, 280);
			this.textFormat_Error.Name = "textFormat_Error";
			this.textFormat_Error.Size = new System.Drawing.Size(232, 23);
			this.textFormat_Error.TabIndex = 29;
			this.textFormat_Error.Tag = "9";
			this.textFormat_Error.FormatChanged += new System.EventHandler(this.textFormat_FormatChanged);
			// 
			// textFormat_Length
			// 
			this.textFormat_Length.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textFormat_Length.FormatColor = System.Drawing.Color.Black;
			this.textFormat_Length.FormatFont = new System.Drawing.Font("DejaVu Sans Mono", 8.25F);
			this.textFormat_Length.Location = new System.Drawing.Point(216, 251);
			this.textFormat_Length.Name = "textFormat_Length";
			this.textFormat_Length.Size = new System.Drawing.Size(232, 23);
			this.textFormat_Length.TabIndex = 26;
			this.textFormat_Length.Tag = "8";
			this.textFormat_Length.FormatChanged += new System.EventHandler(this.textFormat_FormatChanged);
			// 
			// textFormat_Time
			// 
			this.textFormat_Time.FormatColor = System.Drawing.Color.Black;
			this.textFormat_Time.FormatFont = new System.Drawing.Font("DejaVu Sans Mono", 8.25F);
			this.textFormat_Time.Location = new System.Drawing.Point(216, 164);
			this.textFormat_Time.Name = "textFormat_Time";
			this.textFormat_Time.Size = new System.Drawing.Size(232, 23);
			this.textFormat_Time.TabIndex = 17;
			this.textFormat_Time.Tag = "5";
			this.textFormat_Time.FormatChanged += new System.EventHandler(this.textFormat_FormatChanged);
			// 
			// textFormat_RxControl
			// 
			this.textFormat_RxControl.FormatColor = System.Drawing.Color.Black;
			this.textFormat_RxControl.FormatFont = new System.Drawing.Font("DejaVu Sans Mono", 8.25F);
			this.textFormat_RxControl.Location = new System.Drawing.Point(216, 106);
			this.textFormat_RxControl.Name = "textFormat_RxControl";
			this.textFormat_RxControl.Size = new System.Drawing.Size(232, 23);
			this.textFormat_RxControl.TabIndex = 11;
			this.textFormat_RxControl.Tag = "3";
			this.textFormat_RxControl.FormatChanged += new System.EventHandler(this.textFormat_FormatChanged);
			// 
			// textFormat_RxData
			// 
			this.textFormat_RxData.FormatColor = System.Drawing.Color.Black;
			this.textFormat_RxData.FormatFont = new System.Drawing.Font("DejaVu Sans Mono", 8.25F);
			this.textFormat_RxData.Location = new System.Drawing.Point(216, 77);
			this.textFormat_RxData.Name = "textFormat_RxData";
			this.textFormat_RxData.Size = new System.Drawing.Size(232, 23);
			this.textFormat_RxData.TabIndex = 8;
			this.textFormat_RxData.Tag = "2";
			this.textFormat_RxData.FormatChanged += new System.EventHandler(this.textFormat_FormatChanged);
			// 
			// textFormat_TxControl
			// 
			this.textFormat_TxControl.FormatColor = System.Drawing.Color.Black;
			this.textFormat_TxControl.FormatFont = new System.Drawing.Font("DejaVu Sans Mono", 8.25F);
			this.textFormat_TxControl.Location = new System.Drawing.Point(216, 48);
			this.textFormat_TxControl.Name = "textFormat_TxControl";
			this.textFormat_TxControl.Size = new System.Drawing.Size(232, 23);
			this.textFormat_TxControl.TabIndex = 5;
			this.textFormat_TxControl.Tag = "1";
			this.textFormat_TxControl.FormatChanged += new System.EventHandler(this.textFormat_FormatChanged);
			// 
			// textFormat_TxData
			// 
			this.textFormat_TxData.FormatColor = System.Drawing.Color.Black;
			this.textFormat_TxData.FormatFont = new System.Drawing.Font("DejaVu Sans Mono", 8.25F);
			this.textFormat_TxData.Location = new System.Drawing.Point(216, 19);
			this.textFormat_TxData.Name = "textFormat_TxData";
			this.textFormat_TxData.Size = new System.Drawing.Size(232, 23);
			this.textFormat_TxData.TabIndex = 2;
			this.textFormat_TxData.Tag = "0";
			this.textFormat_TxData.FormatChanged += new System.EventHandler(this.textFormat_FormatChanged);
			// 
			// button_Font
			// 
			this.button_Font.Location = new System.Drawing.Point(463, 150);
			this.button_Font.Name = "button_Font";
			this.button_Font.Size = new System.Drawing.Size(75, 23);
			this.button_Font.TabIndex = 31;
			this.button_Font.Text = "Font...";
			this.button_Font.Click += new System.EventHandler(this.button_Font_Click);
			// 
			// button_Defaults
			// 
			this.button_Defaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Defaults.Location = new System.Drawing.Point(585, 162);
			this.button_Defaults.Name = "button_Defaults";
			this.button_Defaults.Size = new System.Drawing.Size(75, 23);
			this.button_Defaults.TabIndex = 6;
			this.button_Defaults.Text = "&Defaults...";
			this.button_Defaults.Click += new System.EventHandler(this.button_Defaults_Click);
			// 
			// label_Example
			// 
			this.label_Example.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label_Example.AutoSize = true;
			this.label_Example.Location = new System.Drawing.Point(24, 461);
			this.label_Example.Name = "label_Example";
			this.label_Example.Size = new System.Drawing.Size(50, 13);
			this.label_Example.TabIndex = 2;
			this.label_Example.Text = "Example:";
			// 
			// monitor_Example
			// 
			this.monitor_Example.ActiveConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_Example.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.monitor_Example.Location = new System.Drawing.Point(100, 456);
			this.monitor_Example.Name = "monitor_Example";
			this.monitor_Example.Size = new System.Drawing.Size(450, 63);
			this.monitor_Example.TabIndex = 3;
			this.monitor_Example.TabStop = false;
			this.monitor_Example.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			// 
			// groupBox_Options
			// 
			this.groupBox_Options.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Options.Controls.Add(this.label_InfoEnclosure);
			this.groupBox_Options.Controls.Add(this.label_InfoSeparator);
			this.groupBox_Options.Controls.Add(this.comboBox_InfoEnclosure);
			this.groupBox_Options.Controls.Add(this.comboBox_InfoSeparator);
			this.groupBox_Options.Location = new System.Drawing.Point(12, 359);
			this.groupBox_Options.Name = "groupBox_Options";
			this.groupBox_Options.Size = new System.Drawing.Size(556, 82);
			this.groupBox_Options.TabIndex = 1;
			this.groupBox_Options.TabStop = false;
			this.groupBox_Options.Text = "Options";
			// 
			// label_InfoEnclosure
			// 
			this.label_InfoEnclosure.AutoSize = true;
			this.label_InfoEnclosure.Location = new System.Drawing.Point(12, 51);
			this.label_InfoEnclosure.Name = "label_InfoEnclosure";
			this.label_InfoEnclosure.Size = new System.Drawing.Size(57, 13);
			this.label_InfoEnclosure.TabIndex = 2;
			this.label_InfoEnclosure.Text = "E&nclosure:";
			// 
			// label_InfoSeparator
			// 
			this.label_InfoSeparator.AutoSize = true;
			this.label_InfoSeparator.Location = new System.Drawing.Point(12, 22);
			this.label_InfoSeparator.Name = "label_InfoSeparator";
			this.label_InfoSeparator.Size = new System.Drawing.Size(56, 13);
			this.label_InfoSeparator.TabIndex = 0;
			this.label_InfoSeparator.Text = "&Separator:";
			// 
			// comboBox_InfoEnclosure
			// 
			this.comboBox_InfoEnclosure.FormattingEnabled = true;
			this.comboBox_InfoEnclosure.Location = new System.Drawing.Point(88, 48);
			this.comboBox_InfoEnclosure.Name = "comboBox_InfoEnclosure";
			this.comboBox_InfoEnclosure.Size = new System.Drawing.Size(162, 21);
			this.comboBox_InfoEnclosure.TabIndex = 3;
			this.toolTip.SetToolTip(this.comboBox_InfoEnclosure, "The enclosure of informational elements.");
			this.comboBox_InfoEnclosure.SelectedIndexChanged += new System.EventHandler(this.comboBox_InfoEnclosure_SelectedIndexChanged);
			this.comboBox_InfoEnclosure.Validating += new System.ComponentModel.CancelEventHandler(this.comboBox_InfoEnclosure_Validating);
			// 
			// comboBox_InfoSeparator
			// 
			this.comboBox_InfoSeparator.FormattingEnabled = true;
			this.comboBox_InfoSeparator.Location = new System.Drawing.Point(88, 19);
			this.comboBox_InfoSeparator.Name = "comboBox_InfoSeparator";
			this.comboBox_InfoSeparator.Size = new System.Drawing.Size(162, 21);
			this.comboBox_InfoSeparator.TabIndex = 1;
			this.toolTip.SetToolTip(this.comboBox_InfoSeparator, "The separator between adjacent informational elements.");
			this.comboBox_InfoSeparator.SelectedIndexChanged += new System.EventHandler(this.comboBox_InfoSeparator_SelectedIndexChanged);
			this.comboBox_InfoSeparator.Validating += new System.ComponentModel.CancelEventHandler(this.comboBox_InfoSeparator_Validating);
			// 
			// FormatSettings
			// 
			this.AcceptButton = this.button_OK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.button_Cancel;
			this.ClientSize = new System.Drawing.Size(675, 534);
			this.Controls.Add(this.groupBox_Options);
			this.Controls.Add(this.monitor_Example);
			this.Controls.Add(this.label_Example);
			this.Controls.Add(this.button_Defaults);
			this.Controls.Add(this.groupBox_Elements);
			this.Controls.Add(this.button_Cancel);
			this.Controls.Add(this.button_OK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormatSettings";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Format Settings";
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
		private YAT.View.Controls.TextFormat textFormat_Time;
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
		private System.Windows.Forms.Label label_Time;
		private System.Windows.Forms.Label label_RxControl;
		private System.Windows.Forms.Label label_RxData;
		private System.Windows.Forms.Label label_TxControl;
		private YAT.View.Controls.Monitor monitor_Example;
		private YAT.View.Controls.Monitor monitor_Error;
		private YAT.View.Controls.Monitor monitor_Length;
		private YAT.View.Controls.Monitor monitor_Time;
		private YAT.View.Controls.Monitor monitor_RxControl;
		private YAT.View.Controls.Monitor monitor_RxData;
		private YAT.View.Controls.Monitor monitor_TxControl;
		private YAT.View.Controls.Monitor monitor_TxData;
		private System.Windows.Forms.Label label_Direction;
		private Controls.TextFormat textFormat_Direction;
		private Controls.Monitor monitor_Direction;
		private Controls.TextFormat textFormat_Date;
		private Controls.Monitor monitor_Date;
		private System.Windows.Forms.Label label_Date;
		private System.Windows.Forms.Button button_Background;
		private System.Windows.Forms.Label label_FontRemark2;
		private System.Windows.Forms.Label label_FontRemark1;
		private Controls.TextFormat textFormat_Port;
		private Controls.Monitor monitor_Port;
		private System.Windows.Forms.Label label_Port;
		private System.Windows.Forms.GroupBox groupBox_Options;
		private System.Windows.Forms.Label label_InfoEnclosure;
		private System.Windows.Forms.Label label_InfoSeparator;
		private System.Windows.Forms.ComboBox comboBox_InfoEnclosure;
		private System.Windows.Forms.ComboBox comboBox_InfoSeparator;
		private System.Windows.Forms.ToolTip toolTip;
	}
}
