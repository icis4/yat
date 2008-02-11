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
			this.monitor_Error = new YAT.Gui.Controls.Monitor();
			this.monitor_WhiteSpace = new YAT.Gui.Controls.Monitor();
			this.monitor_Length = new YAT.Gui.Controls.Monitor();
			this.monitor_TimeStamp = new YAT.Gui.Controls.Monitor();
			this.monitor_RxControl = new YAT.Gui.Controls.Monitor();
			this.monitor_RxData = new YAT.Gui.Controls.Monitor();
			this.monitor_TxControl = new YAT.Gui.Controls.Monitor();
			this.monitor_TxData = new YAT.Gui.Controls.Monitor();
			this.label_Format_Error = new System.Windows.Forms.Label();
			this.label_Format_WhiteSpace = new System.Windows.Forms.Label();
			this.label_Format_Length = new System.Windows.Forms.Label();
			this.label_Format_TimeStamp = new System.Windows.Forms.Label();
			this.label_Format_RxControl = new System.Windows.Forms.Label();
			this.label_Format_RxData = new System.Windows.Forms.Label();
			this.label_Format_TxControl = new System.Windows.Forms.Label();
			this.label_Format_TxData = new System.Windows.Forms.Label();
			this.textFormat_Error = new YAT.Gui.Controls.TextFormat();
			this.textFormat_WhiteSpace = new YAT.Gui.Controls.TextFormat();
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
			this.button_Cancel.Location = new System.Drawing.Point(602, 60);
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
			this.button_OK.Location = new System.Drawing.Point(602, 31);
			this.button_OK.Name = "button_OK";
			this.button_OK.Size = new System.Drawing.Size(72, 23);
			this.button_OK.TabIndex = 3;
			this.button_OK.Text = "OK";
			this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
			// 
			// groupBox_Settings
			// 
			this.groupBox_Settings.Controls.Add(this.monitor_Error);
			this.groupBox_Settings.Controls.Add(this.monitor_WhiteSpace);
			this.groupBox_Settings.Controls.Add(this.monitor_Length);
			this.groupBox_Settings.Controls.Add(this.monitor_TimeStamp);
			this.groupBox_Settings.Controls.Add(this.monitor_RxControl);
			this.groupBox_Settings.Controls.Add(this.monitor_RxData);
			this.groupBox_Settings.Controls.Add(this.monitor_TxControl);
			this.groupBox_Settings.Controls.Add(this.monitor_TxData);
			this.groupBox_Settings.Controls.Add(this.label_Format_Error);
			this.groupBox_Settings.Controls.Add(this.label_Format_WhiteSpace);
			this.groupBox_Settings.Controls.Add(this.label_Format_Length);
			this.groupBox_Settings.Controls.Add(this.label_Format_TimeStamp);
			this.groupBox_Settings.Controls.Add(this.label_Format_RxControl);
			this.groupBox_Settings.Controls.Add(this.label_Format_RxData);
			this.groupBox_Settings.Controls.Add(this.label_Format_TxControl);
			this.groupBox_Settings.Controls.Add(this.label_Format_TxData);
			this.groupBox_Settings.Controls.Add(this.textFormat_Error);
			this.groupBox_Settings.Controls.Add(this.textFormat_WhiteSpace);
			this.groupBox_Settings.Controls.Add(this.textFormat_Length);
			this.groupBox_Settings.Controls.Add(this.textFormat_TimeStamp);
			this.groupBox_Settings.Controls.Add(this.textFormat_RxControl);
			this.groupBox_Settings.Controls.Add(this.textFormat_RxData);
			this.groupBox_Settings.Controls.Add(this.textFormat_TxControl);
			this.groupBox_Settings.Controls.Add(this.textFormat_TxData);
			this.groupBox_Settings.Controls.Add(this.button_Font);
			this.groupBox_Settings.Location = new System.Drawing.Point(12, 12);
			this.groupBox_Settings.Name = "groupBox_Settings";
			this.groupBox_Settings.Size = new System.Drawing.Size(573, 262);
			this.groupBox_Settings.TabIndex = 0;
			this.groupBox_Settings.TabStop = false;
			// 
			// monitor_Error
			// 
			this.monitor_Error.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.monitor_Error.Location = new System.Drawing.Point(109, 222);
			this.monitor_Error.Name = "monitor_Error";
			this.monitor_Error.Size = new System.Drawing.Size(127, 23);
			this.monitor_Error.TabIndex = 17;
			// 
			// monitor_WhiteSpace
			// 
			this.monitor_WhiteSpace.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.monitor_WhiteSpace.Location = new System.Drawing.Point(109, 193);
			this.monitor_WhiteSpace.Name = "monitor_WhiteSpace";
			this.monitor_WhiteSpace.Size = new System.Drawing.Size(127, 23);
			this.monitor_WhiteSpace.TabIndex = 17;
			// 
			// monitor_Length
			// 
			this.monitor_Length.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.monitor_Length.Location = new System.Drawing.Point(109, 164);
			this.monitor_Length.Name = "monitor_Length";
			this.monitor_Length.Size = new System.Drawing.Size(127, 23);
			this.monitor_Length.TabIndex = 17;
			// 
			// monitor_TimeStamp
			// 
			this.monitor_TimeStamp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.monitor_TimeStamp.Location = new System.Drawing.Point(109, 135);
			this.monitor_TimeStamp.Name = "monitor_TimeStamp";
			this.monitor_TimeStamp.Size = new System.Drawing.Size(127, 23);
			this.monitor_TimeStamp.TabIndex = 17;
			// 
			// monitor_RxControl
			// 
			this.monitor_RxControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.monitor_RxControl.Location = new System.Drawing.Point(109, 106);
			this.monitor_RxControl.Name = "monitor_RxControl";
			this.monitor_RxControl.Size = new System.Drawing.Size(127, 23);
			this.monitor_RxControl.TabIndex = 17;
			// 
			// monitor_RxData
			// 
			this.monitor_RxData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.monitor_RxData.Location = new System.Drawing.Point(109, 77);
			this.monitor_RxData.Name = "monitor_RxData";
			this.monitor_RxData.Size = new System.Drawing.Size(127, 23);
			this.monitor_RxData.TabIndex = 17;
			// 
			// monitor_TxControl
			// 
			this.monitor_TxControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.monitor_TxControl.Location = new System.Drawing.Point(109, 48);
			this.monitor_TxControl.Name = "monitor_TxControl";
			this.monitor_TxControl.Size = new System.Drawing.Size(127, 23);
			this.monitor_TxControl.TabIndex = 17;
			// 
			// monitor_TxData
			// 
			this.monitor_TxData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.monitor_TxData.Location = new System.Drawing.Point(109, 19);
			this.monitor_TxData.Name = "monitor_TxData";
			this.monitor_TxData.Size = new System.Drawing.Size(127, 23);
			this.monitor_TxData.TabIndex = 17;
			// 
			// label_Format_Error
			// 
			this.label_Format_Error.AutoSize = true;
			this.label_Format_Error.Location = new System.Drawing.Point(12, 227);
			this.label_Format_Error.Name = "label_Format_Error";
			this.label_Format_Error.Size = new System.Drawing.Size(32, 13);
			this.label_Format_Error.TabIndex = 14;
			this.label_Format_Error.Text = "&Error:";
			this.label_Format_Error.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label_Format_WhiteSpace
			// 
			this.label_Format_WhiteSpace.AutoSize = true;
			this.label_Format_WhiteSpace.Location = new System.Drawing.Point(12, 198);
			this.label_Format_WhiteSpace.Name = "label_Format_WhiteSpace";
			this.label_Format_WhiteSpace.Size = new System.Drawing.Size(70, 13);
			this.label_Format_WhiteSpace.TabIndex = 12;
			this.label_Format_WhiteSpace.Text = "&White space:";
			this.label_Format_WhiteSpace.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label_Format_Length
			// 
			this.label_Format_Length.AutoSize = true;
			this.label_Format_Length.Location = new System.Drawing.Point(12, 169);
			this.label_Format_Length.Name = "label_Format_Length";
			this.label_Format_Length.Size = new System.Drawing.Size(43, 13);
			this.label_Format_Length.TabIndex = 10;
			this.label_Format_Length.Text = "&Length:";
			this.label_Format_Length.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label_Format_TimeStamp
			// 
			this.label_Format_TimeStamp.AutoSize = true;
			this.label_Format_TimeStamp.Location = new System.Drawing.Point(12, 140);
			this.label_Format_TimeStamp.Name = "label_Format_TimeStamp";
			this.label_Format_TimeStamp.Size = new System.Drawing.Size(64, 13);
			this.label_Format_TimeStamp.TabIndex = 8;
			this.label_Format_TimeStamp.Text = "T&ime stamp:";
			this.label_Format_TimeStamp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label_Format_RxControl
			// 
			this.label_Format_RxControl.AutoSize = true;
			this.label_Format_RxControl.Location = new System.Drawing.Point(12, 111);
			this.label_Format_RxControl.Name = "label_Format_RxControl";
			this.label_Format_RxControl.Size = new System.Drawing.Size(59, 13);
			this.label_Format_RxControl.TabIndex = 6;
			this.label_Format_RxControl.Text = "Rx C&ontrol:";
			this.label_Format_RxControl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label_Format_RxData
			// 
			this.label_Format_RxData.AutoSize = true;
			this.label_Format_RxData.Location = new System.Drawing.Point(12, 82);
			this.label_Format_RxData.Name = "label_Format_RxData";
			this.label_Format_RxData.Size = new System.Drawing.Size(49, 13);
			this.label_Format_RxData.TabIndex = 4;
			this.label_Format_RxData.Text = "&Rx Data:";
			this.label_Format_RxData.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label_Format_TxControl
			// 
			this.label_Format_TxControl.AutoSize = true;
			this.label_Format_TxControl.Location = new System.Drawing.Point(12, 53);
			this.label_Format_TxControl.Name = "label_Format_TxControl";
			this.label_Format_TxControl.Size = new System.Drawing.Size(58, 13);
			this.label_Format_TxControl.TabIndex = 2;
			this.label_Format_TxControl.Text = "Tx &Control:";
			this.label_Format_TxControl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label_Format_TxData
			// 
			this.label_Format_TxData.AutoSize = true;
			this.label_Format_TxData.Location = new System.Drawing.Point(12, 24);
			this.label_Format_TxData.Name = "label_Format_TxData";
			this.label_Format_TxData.Size = new System.Drawing.Size(48, 13);
			this.label_Format_TxData.TabIndex = 0;
			this.label_Format_TxData.Text = "&Tx Data:";
			this.label_Format_TxData.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textFormat_Error
			// 
			this.textFormat_Error.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textFormat_Error.FormatColor = System.Drawing.Color.Black;
			this.textFormat_Error.FormatFont = new System.Drawing.Font("Courier New", 9.75F);
			this.textFormat_Error.Location = new System.Drawing.Point(238, 222);
			this.textFormat_Error.Name = "textFormat_Error";
			this.textFormat_Error.Size = new System.Drawing.Size(233, 23);
			this.textFormat_Error.TabIndex = 15;
			this.textFormat_Error.Tag = "7";
			this.textFormat_Error.FormatChanged += new System.EventHandler(this.textFormat_FormatChanged);
			// 
			// textFormat_WhiteSpace
			// 
			this.textFormat_WhiteSpace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textFormat_WhiteSpace.FormatColor = System.Drawing.Color.Black;
			this.textFormat_WhiteSpace.FormatFont = new System.Drawing.Font("Courier New", 9.75F);
			this.textFormat_WhiteSpace.Location = new System.Drawing.Point(238, 193);
			this.textFormat_WhiteSpace.Name = "textFormat_WhiteSpace";
			this.textFormat_WhiteSpace.Size = new System.Drawing.Size(233, 23);
			this.textFormat_WhiteSpace.TabIndex = 13;
			this.textFormat_WhiteSpace.Tag = "6";
			this.textFormat_WhiteSpace.FormatChanged += new System.EventHandler(this.textFormat_FormatChanged);
			// 
			// textFormat_Length
			// 
			this.textFormat_Length.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textFormat_Length.FormatColor = System.Drawing.Color.Black;
			this.textFormat_Length.FormatFont = new System.Drawing.Font("Courier New", 9.75F);
			this.textFormat_Length.Location = new System.Drawing.Point(238, 164);
			this.textFormat_Length.Name = "textFormat_Length";
			this.textFormat_Length.Size = new System.Drawing.Size(233, 23);
			this.textFormat_Length.TabIndex = 11;
			this.textFormat_Length.Tag = "5";
			this.textFormat_Length.FormatChanged += new System.EventHandler(this.textFormat_FormatChanged);
			// 
			// textFormat_TimeStamp
			// 
			this.textFormat_TimeStamp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textFormat_TimeStamp.FormatColor = System.Drawing.Color.Black;
			this.textFormat_TimeStamp.FormatFont = new System.Drawing.Font("Courier New", 9.75F);
			this.textFormat_TimeStamp.Location = new System.Drawing.Point(238, 135);
			this.textFormat_TimeStamp.Name = "textFormat_TimeStamp";
			this.textFormat_TimeStamp.Size = new System.Drawing.Size(233, 23);
			this.textFormat_TimeStamp.TabIndex = 9;
			this.textFormat_TimeStamp.Tag = "4";
			this.textFormat_TimeStamp.FormatChanged += new System.EventHandler(this.textFormat_FormatChanged);
			// 
			// textFormat_RxControl
			// 
			this.textFormat_RxControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textFormat_RxControl.FormatColor = System.Drawing.Color.Black;
			this.textFormat_RxControl.FormatFont = new System.Drawing.Font("Courier New", 9.75F);
			this.textFormat_RxControl.Location = new System.Drawing.Point(238, 106);
			this.textFormat_RxControl.Name = "textFormat_RxControl";
			this.textFormat_RxControl.Size = new System.Drawing.Size(233, 23);
			this.textFormat_RxControl.TabIndex = 7;
			this.textFormat_RxControl.Tag = "3";
			this.textFormat_RxControl.FormatChanged += new System.EventHandler(this.textFormat_FormatChanged);
			// 
			// textFormat_RxData
			// 
			this.textFormat_RxData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textFormat_RxData.FormatColor = System.Drawing.Color.Black;
			this.textFormat_RxData.FormatFont = new System.Drawing.Font("Courier New", 9.75F);
			this.textFormat_RxData.Location = new System.Drawing.Point(238, 77);
			this.textFormat_RxData.Name = "textFormat_RxData";
			this.textFormat_RxData.Size = new System.Drawing.Size(233, 23);
			this.textFormat_RxData.TabIndex = 5;
			this.textFormat_RxData.Tag = "2";
			this.textFormat_RxData.FormatChanged += new System.EventHandler(this.textFormat_FormatChanged);
			// 
			// textFormat_TxControl
			// 
			this.textFormat_TxControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textFormat_TxControl.FormatColor = System.Drawing.Color.Black;
			this.textFormat_TxControl.FormatFont = new System.Drawing.Font("Courier New", 9.75F);
			this.textFormat_TxControl.Location = new System.Drawing.Point(238, 48);
			this.textFormat_TxControl.Name = "textFormat_TxControl";
			this.textFormat_TxControl.Size = new System.Drawing.Size(233, 23);
			this.textFormat_TxControl.TabIndex = 3;
			this.textFormat_TxControl.Tag = "1";
			this.textFormat_TxControl.FormatChanged += new System.EventHandler(this.textFormat_FormatChanged);
			// 
			// textFormat_TxData
			// 
			this.textFormat_TxData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textFormat_TxData.FormatColor = System.Drawing.Color.Black;
			this.textFormat_TxData.FormatFont = new System.Drawing.Font("Courier New", 9.75F);
			this.textFormat_TxData.Location = new System.Drawing.Point(238, 19);
			this.textFormat_TxData.Name = "textFormat_TxData";
			this.textFormat_TxData.Size = new System.Drawing.Size(233, 23);
			this.textFormat_TxData.TabIndex = 1;
			this.textFormat_TxData.Tag = "0";
			this.textFormat_TxData.FormatChanged += new System.EventHandler(this.textFormat_FormatChanged);
			// 
			// button_Font
			// 
			this.button_Font.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Font.Location = new System.Drawing.Point(485, 120);
			this.button_Font.Name = "button_Font";
			this.button_Font.Size = new System.Drawing.Size(72, 23);
			this.button_Font.TabIndex = 16;
			this.button_Font.Text = "Font...";
			this.button_Font.Click += new System.EventHandler(this.button_Font_Click);
			// 
			// button_Defaults
			// 
			this.button_Defaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Defaults.Location = new System.Drawing.Point(602, 132);
			this.button_Defaults.Name = "button_Defaults";
			this.button_Defaults.Size = new System.Drawing.Size(72, 23);
			this.button_Defaults.TabIndex = 5;
			this.button_Defaults.Text = "&Defaults...";
			this.button_Defaults.Click += new System.EventHandler(this.button_Defaults_Click);
			// 
			// label_Example
			// 
			this.label_Example.AutoSize = true;
			this.label_Example.Location = new System.Drawing.Point(24, 290);
			this.label_Example.Name = "label_Example";
			this.label_Example.Size = new System.Drawing.Size(50, 13);
			this.label_Example.TabIndex = 1;
			this.label_Example.Text = "&Example:";
			this.label_Example.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// monitor_Example
			// 
			this.monitor_Example.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.monitor_Example.Location = new System.Drawing.Point(121, 290);
			this.monitor_Example.Name = "monitor_Example";
			this.monitor_Example.Size = new System.Drawing.Size(360, 58);
			this.monitor_Example.TabIndex = 2;
			// 
			// FormatSettings
			// 
			this.AcceptButton = this.button_OK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.button_Cancel;
			this.ClientSize = new System.Drawing.Size(692, 363);
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
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormatSettings_Paint);
			this.groupBox_Settings.ResumeLayout(false);
			this.groupBox_Settings.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox_Settings;
		private YAT.Gui.Controls.TextFormat textFormat_Error;
		private YAT.Gui.Controls.TextFormat textFormat_WhiteSpace;
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
		private System.Windows.Forms.Label label_Format_TxData;
		private System.Windows.Forms.Label label_Format_Error;
		private System.Windows.Forms.Label label_Format_WhiteSpace;
		private System.Windows.Forms.Label label_Format_Length;
		private System.Windows.Forms.Label label_Format_TimeStamp;
		private System.Windows.Forms.Label label_Format_RxControl;
		private System.Windows.Forms.Label label_Format_RxData;
		private System.Windows.Forms.Label label_Format_TxControl;
		private YAT.Gui.Controls.Monitor monitor_Example;
		private YAT.Gui.Controls.Monitor monitor_Error;
		private YAT.Gui.Controls.Monitor monitor_WhiteSpace;
		private YAT.Gui.Controls.Monitor monitor_Length;
		private YAT.Gui.Controls.Monitor monitor_TimeStamp;
		private YAT.Gui.Controls.Monitor monitor_RxControl;
		private YAT.Gui.Controls.Monitor monitor_RxData;
		private YAT.Gui.Controls.Monitor monitor_TxControl;
		private YAT.Gui.Controls.Monitor monitor_TxData;
	}
}
