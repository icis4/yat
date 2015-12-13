using System;
using System.Collections.Generic;
using System.Text;

namespace YAT.Gui.Forms
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
			this.button_Cancel = new System.Windows.Forms.Button();
			this.button_OK = new System.Windows.Forms.Button();
			this.groupBox_Settings = new System.Windows.Forms.GroupBox();
			this.textFormat_Direction = new YAT.Gui.Controls.TextFormat();
			this.monitor_Direction = new YAT.Gui.Controls.Monitor();
			this.label1 = new System.Windows.Forms.Label();
			this.monitor_Error = new YAT.Gui.Controls.Monitor();
			this.monitor_Length = new YAT.Gui.Controls.Monitor();
			this.monitor_TimeStamp = new YAT.Gui.Controls.Monitor();
			this.monitor_RxControl = new YAT.Gui.Controls.Monitor();
			this.monitor_RxData = new YAT.Gui.Controls.Monitor();
			this.monitor_TxControl = new YAT.Gui.Controls.Monitor();
			this.monitor_TxData = new YAT.Gui.Controls.Monitor();
			this.label_Error = new System.Windows.Forms.Label();
			this.label_Length = new System.Windows.Forms.Label();
			this.label_TimeStamp = new System.Windows.Forms.Label();
			this.label_RxControl = new System.Windows.Forms.Label();
			this.label_RxData = new System.Windows.Forms.Label();
			this.label_TxControl = new System.Windows.Forms.Label();
			this.label_TxData = new System.Windows.Forms.Label();
			this.textFormat_Error = new YAT.Gui.Controls.TextFormat();
			this.textFormat_Length = new YAT.Gui.Controls.TextFormat();
			this.textFormat_TimeStamp = new YAT.Gui.Controls.TextFormat();
			this.textFormat_RxControl = new YAT.Gui.Controls.TextFormat();
			this.textFormat_RxData = new YAT.Gui.Controls.TextFormat();
			this.textFormat_TxControl = new YAT.Gui.Controls.TextFormat();
			this.textFormat_TxData = new YAT.Gui.Controls.TextFormat();
			this.button_Font = new System.Windows.Forms.Button();
			this.button_Defaults = new System.Windows.Forms.Button();
			this.label_Example = new System.Windows.Forms.Label();
			this.monitor_Example = new YAT.Gui.Controls.Monitor();
			this.groupBox_Settings.SuspendLayout();
			this.SuspendLayout();
			// 
			// button_Cancel
			// 
			this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button_Cancel.Location = new System.Drawing.Point(579, 60);
			this.button_Cancel.Name = "button_Cancel";
			this.button_Cancel.Size = new System.Drawing.Size(72, 23);
			this.button_Cancel.TabIndex = 4;
			this.button_Cancel.Text = "Cancel";
			this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
			// 
			// button_OK
			// 
			this.button_OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button_OK.Location = new System.Drawing.Point(579, 31);
			this.button_OK.Name = "button_OK";
			this.button_OK.Size = new System.Drawing.Size(72, 23);
			this.button_OK.TabIndex = 3;
			this.button_OK.Text = "OK";
			this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
			// 
			// groupBox_Settings
			// 
			this.groupBox_Settings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Settings.Controls.Add(this.textFormat_Direction);
			this.groupBox_Settings.Controls.Add(this.monitor_Direction);
			this.groupBox_Settings.Controls.Add(this.label1);
			this.groupBox_Settings.Controls.Add(this.monitor_Error);
			this.groupBox_Settings.Controls.Add(this.monitor_Length);
			this.groupBox_Settings.Controls.Add(this.monitor_TimeStamp);
			this.groupBox_Settings.Controls.Add(this.monitor_RxControl);
			this.groupBox_Settings.Controls.Add(this.monitor_RxData);
			this.groupBox_Settings.Controls.Add(this.monitor_TxControl);
			this.groupBox_Settings.Controls.Add(this.monitor_TxData);
			this.groupBox_Settings.Controls.Add(this.label_Error);
			this.groupBox_Settings.Controls.Add(this.label_Length);
			this.groupBox_Settings.Controls.Add(this.label_TimeStamp);
			this.groupBox_Settings.Controls.Add(this.label_RxControl);
			this.groupBox_Settings.Controls.Add(this.label_RxData);
			this.groupBox_Settings.Controls.Add(this.label_TxControl);
			this.groupBox_Settings.Controls.Add(this.label_TxData);
			this.groupBox_Settings.Controls.Add(this.textFormat_Error);
			this.groupBox_Settings.Controls.Add(this.textFormat_Length);
			this.groupBox_Settings.Controls.Add(this.textFormat_TimeStamp);
			this.groupBox_Settings.Controls.Add(this.textFormat_RxControl);
			this.groupBox_Settings.Controls.Add(this.textFormat_RxData);
			this.groupBox_Settings.Controls.Add(this.textFormat_TxControl);
			this.groupBox_Settings.Controls.Add(this.textFormat_TxData);
			this.groupBox_Settings.Controls.Add(this.button_Font);
			this.groupBox_Settings.Location = new System.Drawing.Point(12, 12);
			this.groupBox_Settings.Name = "groupBox_Settings";
			this.groupBox_Settings.Size = new System.Drawing.Size(550, 263);
			this.groupBox_Settings.TabIndex = 0;
			this.groupBox_Settings.TabStop = false;
			// 
			// textFormat_Direction
			// 
			this.textFormat_Direction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textFormat_Direction.FormatColor = System.Drawing.Color.Black;
			this.textFormat_Direction.FormatFont = new System.Drawing.Font("DejaVu Sans Mono", 8.25F);
			this.textFormat_Direction.Location = new System.Drawing.Point(216, 164);
			this.textFormat_Direction.Name = "textFormat_Direction";
			this.textFormat_Direction.Size = new System.Drawing.Size(232, 23);
			this.textFormat_Direction.TabIndex = 27;
			this.textFormat_Direction.Tag = "5";
			this.textFormat_Direction.FormatChanged += new System.EventHandler(this.textFormat_FormatChanged);
			// 
			// monitor_Direction
			// 
			this.monitor_Direction.ConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_Direction.Location = new System.Drawing.Point(88, 164);
			this.monitor_Direction.Name = "monitor_Direction";
			this.monitor_Direction.Size = new System.Drawing.Size(127, 23);
			this.monitor_Direction.TabIndex = 26;
			this.monitor_Direction.TabStop = false;
			this.monitor_Direction.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 169);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(52, 13);
			this.label1.TabIndex = 25;
			this.label1.Text = "&Direction:";
			// 
			// monitor_Error
			// 
			this.monitor_Error.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.monitor_Error.ConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_Error.Location = new System.Drawing.Point(88, 222);
			this.monitor_Error.Name = "monitor_Error";
			this.monitor_Error.Size = new System.Drawing.Size(127, 23);
			this.monitor_Error.TabIndex = 22;
			this.monitor_Error.TabStop = false;
			this.monitor_Error.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			// 
			// monitor_Length
			// 
			this.monitor_Length.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.monitor_Length.ConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_Length.Location = new System.Drawing.Point(88, 193);
			this.monitor_Length.Name = "monitor_Length";
			this.monitor_Length.Size = new System.Drawing.Size(127, 23);
			this.monitor_Length.TabIndex = 16;
			this.monitor_Length.TabStop = false;
			this.monitor_Length.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			// 
			// monitor_TimeStamp
			// 
			this.monitor_TimeStamp.ConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_TimeStamp.Location = new System.Drawing.Point(88, 135);
			this.monitor_TimeStamp.Name = "monitor_TimeStamp";
			this.monitor_TimeStamp.Size = new System.Drawing.Size(127, 23);
			this.monitor_TimeStamp.TabIndex = 13;
			this.monitor_TimeStamp.TabStop = false;
			this.monitor_TimeStamp.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			// 
			// monitor_RxControl
			// 
			this.monitor_RxControl.ConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_RxControl.Location = new System.Drawing.Point(88, 106);
			this.monitor_RxControl.Name = "monitor_RxControl";
			this.monitor_RxControl.Size = new System.Drawing.Size(127, 23);
			this.monitor_RxControl.TabIndex = 10;
			this.monitor_RxControl.TabStop = false;
			this.monitor_RxControl.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			// 
			// monitor_RxData
			// 
			this.monitor_RxData.ConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_RxData.Location = new System.Drawing.Point(88, 77);
			this.monitor_RxData.Name = "monitor_RxData";
			this.monitor_RxData.Size = new System.Drawing.Size(127, 23);
			this.monitor_RxData.TabIndex = 7;
			this.monitor_RxData.TabStop = false;
			this.monitor_RxData.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			// 
			// monitor_TxControl
			// 
			this.monitor_TxControl.ConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_TxControl.Location = new System.Drawing.Point(88, 48);
			this.monitor_TxControl.Name = "monitor_TxControl";
			this.monitor_TxControl.Size = new System.Drawing.Size(127, 23);
			this.monitor_TxControl.TabIndex = 4;
			this.monitor_TxControl.TabStop = false;
			this.monitor_TxControl.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			// 
			// monitor_TxData
			// 
			this.monitor_TxData.ConnectTime = System.TimeSpan.Parse("00:00:00");
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
			this.label_Error.Location = new System.Drawing.Point(12, 227);
			this.label_Error.Name = "label_Error";
			this.label_Error.Size = new System.Drawing.Size(32, 13);
			this.label_Error.TabIndex = 21;
			this.label_Error.Text = "&Error:";
			// 
			// label_Length
			// 
			this.label_Length.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label_Length.AutoSize = true;
			this.label_Length.Location = new System.Drawing.Point(12, 198);
			this.label_Length.Name = "label_Length";
			this.label_Length.Size = new System.Drawing.Size(43, 13);
			this.label_Length.TabIndex = 15;
			this.label_Length.Text = "&Length:";
			// 
			// label_TimeStamp
			// 
			this.label_TimeStamp.AutoSize = true;
			this.label_TimeStamp.Location = new System.Drawing.Point(12, 140);
			this.label_TimeStamp.Name = "label_TimeStamp";
			this.label_TimeStamp.Size = new System.Drawing.Size(66, 13);
			this.label_TimeStamp.TabIndex = 12;
			this.label_TimeStamp.Text = "T&ime Stamp:";
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
			this.textFormat_Error.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.textFormat_Error.FormatColor = System.Drawing.Color.Black;
			this.textFormat_Error.FormatFont = new System.Drawing.Font("DejaVu Sans Mono", 8.25F);
			this.textFormat_Error.Location = new System.Drawing.Point(216, 222);
			this.textFormat_Error.Name = "textFormat_Error";
			this.textFormat_Error.Size = new System.Drawing.Size(232, 23);
			this.textFormat_Error.TabIndex = 23;
			this.textFormat_Error.Tag = "7";
			this.textFormat_Error.FormatChanged += new System.EventHandler(this.textFormat_FormatChanged);
			// 
			// textFormat_Length
			// 
			this.textFormat_Length.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.textFormat_Length.FormatColor = System.Drawing.Color.Black;
			this.textFormat_Length.FormatFont = new System.Drawing.Font("DejaVu Sans Mono", 8.25F);
			this.textFormat_Length.Location = new System.Drawing.Point(216, 193);
			this.textFormat_Length.Name = "textFormat_Length";
			this.textFormat_Length.Size = new System.Drawing.Size(232, 23);
			this.textFormat_Length.TabIndex = 17;
			this.textFormat_Length.Tag = "6";
			this.textFormat_Length.FormatChanged += new System.EventHandler(this.textFormat_FormatChanged);
			// 
			// textFormat_TimeStamp
			// 
			this.textFormat_TimeStamp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textFormat_TimeStamp.FormatColor = System.Drawing.Color.Black;
			this.textFormat_TimeStamp.FormatFont = new System.Drawing.Font("DejaVu Sans Mono", 8.25F);
			this.textFormat_TimeStamp.Location = new System.Drawing.Point(216, 135);
			this.textFormat_TimeStamp.Name = "textFormat_TimeStamp";
			this.textFormat_TimeStamp.Size = new System.Drawing.Size(232, 23);
			this.textFormat_TimeStamp.TabIndex = 14;
			this.textFormat_TimeStamp.Tag = "4";
			this.textFormat_TimeStamp.FormatChanged += new System.EventHandler(this.textFormat_FormatChanged);
			// 
			// textFormat_RxControl
			// 
			this.textFormat_RxControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
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
			this.textFormat_RxData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
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
			this.textFormat_TxControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
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
			this.textFormat_TxData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
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
			this.button_Font.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Font.Location = new System.Drawing.Point(463, 120);
			this.button_Font.Name = "button_Font";
			this.button_Font.Size = new System.Drawing.Size(72, 23);
			this.button_Font.TabIndex = 24;
			this.button_Font.Text = "Font...";
			this.button_Font.Click += new System.EventHandler(this.button_Font_Click);
			// 
			// button_Defaults
			// 
			this.button_Defaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Defaults.Location = new System.Drawing.Point(579, 132);
			this.button_Defaults.Name = "button_Defaults";
			this.button_Defaults.Size = new System.Drawing.Size(72, 23);
			this.button_Defaults.TabIndex = 5;
			this.button_Defaults.Text = "&Defaults...";
			this.button_Defaults.Click += new System.EventHandler(this.button_Defaults_Click);
			// 
			// label_Example
			// 
			this.label_Example.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label_Example.AutoSize = true;
			this.label_Example.Location = new System.Drawing.Point(24, 291);
			this.label_Example.Name = "label_Example";
			this.label_Example.Size = new System.Drawing.Size(50, 13);
			this.label_Example.TabIndex = 1;
			this.label_Example.Text = "&Example:";
			// 
			// monitor_Example
			// 
			this.monitor_Example.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.monitor_Example.ConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_Example.Location = new System.Drawing.Point(100, 291);
			this.monitor_Example.Name = "monitor_Example";
			this.monitor_Example.Size = new System.Drawing.Size(358, 58);
			this.monitor_Example.TabIndex = 2;
			this.monitor_Example.TabStop = false;
			this.monitor_Example.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			// 
			// FormatSettings
			// 
			this.AcceptButton = this.button_OK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button_Cancel;
			this.ClientSize = new System.Drawing.Size(669, 364);
			this.Controls.Add(this.monitor_Example);
			this.Controls.Add(this.label_Example);
			this.Controls.Add(this.button_Defaults);
			this.Controls.Add(this.groupBox_Settings);
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
			this.groupBox_Settings.ResumeLayout(false);
			this.groupBox_Settings.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox_Settings;
		private YAT.Gui.Controls.TextFormat textFormat_Error;
		private YAT.Gui.Controls.TextFormat textFormat_Length;
		private YAT.Gui.Controls.TextFormat textFormat_TimeStamp;
		private YAT.Gui.Controls.TextFormat textFormat_RxControl;
		private YAT.Gui.Controls.TextFormat textFormat_RxData;
		private YAT.Gui.Controls.TextFormat textFormat_TxControl;
		private YAT.Gui.Controls.TextFormat textFormat_TxData;
		private System.Windows.Forms.Label label_Example;
		private System.Windows.Forms.Button button_Font;
		private System.Windows.Forms.Button button_Defaults;
		private System.Windows.Forms.Button button_Cancel;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.Label label_TxData;
		private System.Windows.Forms.Label label_Error;
		private System.Windows.Forms.Label label_Length;
		private System.Windows.Forms.Label label_TimeStamp;
		private System.Windows.Forms.Label label_RxControl;
		private System.Windows.Forms.Label label_RxData;
		private System.Windows.Forms.Label label_TxControl;
		private YAT.Gui.Controls.Monitor monitor_Example;
		private YAT.Gui.Controls.Monitor monitor_Error;
		private YAT.Gui.Controls.Monitor monitor_Length;
		private YAT.Gui.Controls.Monitor monitor_TimeStamp;
		private YAT.Gui.Controls.Monitor monitor_RxControl;
		private YAT.Gui.Controls.Monitor monitor_RxData;
		private YAT.Gui.Controls.Monitor monitor_TxControl;
		private YAT.Gui.Controls.Monitor monitor_TxData;
		private System.Windows.Forms.Label label1;
		private Controls.TextFormat textFormat_Direction;
		private Controls.Monitor monitor_Direction;
	}
}
