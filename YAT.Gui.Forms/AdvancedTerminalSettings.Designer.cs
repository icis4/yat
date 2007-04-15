namespace HSR.YAT.Gui.Forms
{
	partial class AdvancedTerminalSettings
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
			this.button_Defaults = new System.Windows.Forms.Button();
			this.button_Cancel = new System.Windows.Forms.Button();
			this.button_OK = new System.Windows.Forms.Button();
			this.groupBox_Settings = new System.Windows.Forms.GroupBox();
			this.groupBox_ReceiveSettings = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label_ParityReplacementExample = new System.Windows.Forms.Label();
			this.textBox_ParityReplacement = new System.Windows.Forms.TextBox();
			this.groupBox_TransmitSettings = new System.Windows.Forms.GroupBox();
			this.checkBox_LocalEcho = new System.Windows.Forms.CheckBox();
			this.groupBox_DisplaySettings = new System.Windows.Forms.GroupBox();
			this.checkBox_ShowTimestamp = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowCounters = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowLength = new System.Windows.Forms.CheckBox();
			this.comboBox_Radix = new System.Windows.Forms.ComboBox();
			this.label_Radix = new System.Windows.Forms.Label();
			this.label_MaximalLineCountUnit = new System.Windows.Forms.Label();
			this.textBox_MaximalLineCount = new System.Windows.Forms.TextBox();
			this.label_MaximalLineCount = new System.Windows.Forms.Label();
			this.groupBox_Settings.SuspendLayout();
			this.groupBox_ReceiveSettings.SuspendLayout();
			this.groupBox_TransmitSettings.SuspendLayout();
			this.groupBox_DisplaySettings.SuspendLayout();
			this.SuspendLayout();
			// 
			// button_Defaults
			// 
			this.button_Defaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Defaults.Location = new System.Drawing.Point(227, 136);
			this.button_Defaults.Name = "button_Defaults";
			this.button_Defaults.Size = new System.Drawing.Size(75, 23);
			this.button_Defaults.TabIndex = 3;
			this.button_Defaults.Text = "&Defaults...";
			this.button_Defaults.Click += new System.EventHandler(this.button_Defaults_Click);
			// 
			// button_Cancel
			// 
			this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button_Cancel.Location = new System.Drawing.Point(227, 72);
			this.button_Cancel.Name = "button_Cancel";
			this.button_Cancel.Size = new System.Drawing.Size(75, 23);
			this.button_Cancel.TabIndex = 2;
			this.button_Cancel.Text = "Cancel";
			this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
			// 
			// button_OK
			// 
			this.button_OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button_OK.Location = new System.Drawing.Point(227, 43);
			this.button_OK.Name = "button_OK";
			this.button_OK.Size = new System.Drawing.Size(75, 23);
			this.button_OK.TabIndex = 1;
			this.button_OK.Text = "OK";
			this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
			// 
			// groupBox_Settings
			// 
			this.groupBox_Settings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Settings.Controls.Add(this.groupBox_ReceiveSettings);
			this.groupBox_Settings.Controls.Add(this.groupBox_TransmitSettings);
			this.groupBox_Settings.Controls.Add(this.groupBox_DisplaySettings);
			this.groupBox_Settings.Location = new System.Drawing.Point(12, 12);
			this.groupBox_Settings.Name = "groupBox_Settings";
			this.groupBox_Settings.Size = new System.Drawing.Size(203, 287);
			this.groupBox_Settings.TabIndex = 0;
			this.groupBox_Settings.TabStop = false;
			// 
			// groupBox_ReceiveSettings
			// 
			this.groupBox_ReceiveSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_ReceiveSettings.Controls.Add(this.label1);
			this.groupBox_ReceiveSettings.Controls.Add(this.label_ParityReplacementExample);
			this.groupBox_ReceiveSettings.Controls.Add(this.textBox_ParityReplacement);
			this.groupBox_ReceiveSettings.Location = new System.Drawing.Point(6, 215);
			this.groupBox_ReceiveSettings.Name = "groupBox_ReceiveSettings";
			this.groupBox_ReceiveSettings.Size = new System.Drawing.Size(190, 65);
			this.groupBox_ReceiveSettings.TabIndex = 2;
			this.groupBox_ReceiveSettings.TabStop = false;
			this.groupBox_ReceiveSettings.Text = "Receive Settings";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(118, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "&Replace parity errors by";
			// 
			// label_ParityReplacementExample
			// 
			this.label_ParityReplacementExample.AutoSize = true;
			this.label_ParityReplacementExample.Location = new System.Drawing.Point(83, 42);
			this.label_ParityReplacementExample.Name = "label_ParityReplacementExample";
			this.label_ParityReplacementExample.Size = new System.Drawing.Size(82, 13);
			this.label_ParityReplacementExample.TabIndex = 2;
			this.label_ParityReplacementExample.Text = "Example: \\h(00)";
			// 
			// textBox_ParityReplacement
			// 
			this.textBox_ParityReplacement.Location = new System.Drawing.Point(129, 19);
			this.textBox_ParityReplacement.Name = "textBox_ParityReplacement";
			this.textBox_ParityReplacement.Size = new System.Drawing.Size(50, 20);
			this.textBox_ParityReplacement.TabIndex = 1;
			this.textBox_ParityReplacement.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_ParityReplacement_Validating);
			// 
			// groupBox_TransmitSettings
			// 
			this.groupBox_TransmitSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_TransmitSettings.Controls.Add(this.checkBox_LocalEcho);
			this.groupBox_TransmitSettings.Location = new System.Drawing.Point(6, 161);
			this.groupBox_TransmitSettings.Name = "groupBox_TransmitSettings";
			this.groupBox_TransmitSettings.Size = new System.Drawing.Size(190, 48);
			this.groupBox_TransmitSettings.TabIndex = 1;
			this.groupBox_TransmitSettings.TabStop = false;
			this.groupBox_TransmitSettings.Text = "Transmit Settings";
			// 
			// checkBox_LocalEcho
			// 
			this.checkBox_LocalEcho.AutoSize = true;
			this.checkBox_LocalEcho.Location = new System.Drawing.Point(12, 21);
			this.checkBox_LocalEcho.Name = "checkBox_LocalEcho";
			this.checkBox_LocalEcho.Size = new System.Drawing.Size(133, 17);
			this.checkBox_LocalEcho.TabIndex = 0;
			this.checkBox_LocalEcho.Text = "Local &echo on transmit";
			this.checkBox_LocalEcho.UseVisualStyleBackColor = true;
			this.checkBox_LocalEcho.CheckedChanged += new System.EventHandler(this.checkBox_LocalEcho_CheckedChanged);
			// 
			// groupBox_DisplaySettings
			// 
			this.groupBox_DisplaySettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_DisplaySettings.Controls.Add(this.checkBox_ShowTimestamp);
			this.groupBox_DisplaySettings.Controls.Add(this.checkBox_ShowCounters);
			this.groupBox_DisplaySettings.Controls.Add(this.checkBox_ShowLength);
			this.groupBox_DisplaySettings.Controls.Add(this.comboBox_Radix);
			this.groupBox_DisplaySettings.Controls.Add(this.label_Radix);
			this.groupBox_DisplaySettings.Controls.Add(this.label_MaximalLineCountUnit);
			this.groupBox_DisplaySettings.Controls.Add(this.textBox_MaximalLineCount);
			this.groupBox_DisplaySettings.Controls.Add(this.label_MaximalLineCount);
			this.groupBox_DisplaySettings.Location = new System.Drawing.Point(6, 13);
			this.groupBox_DisplaySettings.Name = "groupBox_DisplaySettings";
			this.groupBox_DisplaySettings.Size = new System.Drawing.Size(190, 142);
			this.groupBox_DisplaySettings.TabIndex = 0;
			this.groupBox_DisplaySettings.TabStop = false;
			this.groupBox_DisplaySettings.Text = "Display Settings";
			// 
			// checkBox_ShowTimestamp
			// 
			this.checkBox_ShowTimestamp.AutoSize = true;
			this.checkBox_ShowTimestamp.Location = new System.Drawing.Point(12, 45);
			this.checkBox_ShowTimestamp.Name = "checkBox_ShowTimestamp";
			this.checkBox_ShowTimestamp.Size = new System.Drawing.Size(103, 17);
			this.checkBox_ShowTimestamp.TabIndex = 2;
			this.checkBox_ShowTimestamp.Text = "Show timestamp";
			this.checkBox_ShowTimestamp.CheckedChanged += new System.EventHandler(this.checkBox_ShowTimestamp_CheckedChanged);
			// 
			// checkBox_ShowCounters
			// 
			this.checkBox_ShowCounters.AutoSize = true;
			this.checkBox_ShowCounters.Location = new System.Drawing.Point(12, 91);
			this.checkBox_ShowCounters.Name = "checkBox_ShowCounters";
			this.checkBox_ShowCounters.Size = new System.Drawing.Size(97, 17);
			this.checkBox_ShowCounters.TabIndex = 4;
			this.checkBox_ShowCounters.Text = "Show counters";
			this.checkBox_ShowCounters.CheckedChanged += new System.EventHandler(this.checkBox_ShowCounters_CheckedChanged);
			// 
			// checkBox_ShowLength
			// 
			this.checkBox_ShowLength.AutoSize = true;
			this.checkBox_ShowLength.Location = new System.Drawing.Point(12, 68);
			this.checkBox_ShowLength.Name = "checkBox_ShowLength";
			this.checkBox_ShowLength.Size = new System.Drawing.Size(85, 17);
			this.checkBox_ShowLength.TabIndex = 3;
			this.checkBox_ShowLength.Text = "Show length";
			this.checkBox_ShowLength.CheckedChanged += new System.EventHandler(this.checkBox_ShowLength_CheckedChanged);
			// 
			// comboBox_Radix
			// 
			this.comboBox_Radix.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_Radix.Location = new System.Drawing.Point(52, 18);
			this.comboBox_Radix.Name = "comboBox_Radix";
			this.comboBox_Radix.Size = new System.Drawing.Size(127, 21);
			this.comboBox_Radix.TabIndex = 1;
			this.comboBox_Radix.SelectedIndexChanged += new System.EventHandler(this.comboBox_Radix_SelectedIndexChanged);
			// 
			// label_Radix
			// 
			this.label_Radix.AutoSize = true;
			this.label_Radix.Location = new System.Drawing.Point(9, 21);
			this.label_Radix.Name = "label_Radix";
			this.label_Radix.Size = new System.Drawing.Size(37, 13);
			this.label_Radix.TabIndex = 0;
			this.label_Radix.Text = "Radix:";
			// 
			// label_MaximalLineCountUnit
			// 
			this.label_MaximalLineCountUnit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label_MaximalLineCountUnit.AutoSize = true;
			this.label_MaximalLineCountUnit.Location = new System.Drawing.Point(144, 116);
			this.label_MaximalLineCountUnit.Name = "label_MaximalLineCountUnit";
			this.label_MaximalLineCountUnit.Size = new System.Drawing.Size(28, 13);
			this.label_MaximalLineCountUnit.TabIndex = 7;
			this.label_MaximalLineCountUnit.Text = "lines";
			this.label_MaximalLineCountUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox_MaximalLineCount
			// 
			this.textBox_MaximalLineCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textBox_MaximalLineCount.Location = new System.Drawing.Point(92, 113);
			this.textBox_MaximalLineCount.Name = "textBox_MaximalLineCount";
			this.textBox_MaximalLineCount.Size = new System.Drawing.Size(50, 20);
			this.textBox_MaximalLineCount.TabIndex = 6;
			this.textBox_MaximalLineCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBox_MaximalLineCount.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_MaximalLineCount_Validating);
			this.textBox_MaximalLineCount.TextChanged += new System.EventHandler(this.textBox_MaximalLineCount_TextChanged);
			// 
			// label_MaximalLineCount
			// 
			this.label_MaximalLineCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label_MaximalLineCount.AutoSize = true;
			this.label_MaximalLineCount.Location = new System.Drawing.Point(9, 116);
			this.label_MaximalLineCount.Name = "label_MaximalLineCount";
			this.label_MaximalLineCount.Size = new System.Drawing.Size(81, 13);
			this.label_MaximalLineCount.TabIndex = 5;
			this.label_MaximalLineCount.Text = "Display &maximal";
			this.label_MaximalLineCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// AdvancedTerminalSettings
			// 
			this.AcceptButton = this.button_OK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button_Cancel;
			this.ClientSize = new System.Drawing.Size(311, 311);
			this.Controls.Add(this.groupBox_Settings);
			this.Controls.Add(this.button_Defaults);
			this.Controls.Add(this.button_Cancel);
			this.Controls.Add(this.button_OK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AdvancedTerminalSettings";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Advanced Terminal Settings";
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.ExtendendTerminalSettings_Paint);
			this.groupBox_Settings.ResumeLayout(false);
			this.groupBox_ReceiveSettings.ResumeLayout(false);
			this.groupBox_ReceiveSettings.PerformLayout();
			this.groupBox_TransmitSettings.ResumeLayout(false);
			this.groupBox_TransmitSettings.PerformLayout();
			this.groupBox_DisplaySettings.ResumeLayout(false);
			this.groupBox_DisplaySettings.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button button_Defaults;
		private System.Windows.Forms.Button button_Cancel;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.GroupBox groupBox_Settings;
		private System.Windows.Forms.GroupBox groupBox_TransmitSettings;
		private System.Windows.Forms.GroupBox groupBox_DisplaySettings;
		private System.Windows.Forms.GroupBox groupBox_ReceiveSettings;
		private System.Windows.Forms.TextBox textBox_ParityReplacement;
		private System.Windows.Forms.Label label_MaximalLineCountUnit;
		private System.Windows.Forms.TextBox textBox_MaximalLineCount;
		private System.Windows.Forms.Label label_MaximalLineCount;
		private System.Windows.Forms.CheckBox checkBox_LocalEcho;
		private System.Windows.Forms.Label label_ParityReplacementExample;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox checkBox_ShowTimestamp;
		private System.Windows.Forms.CheckBox checkBox_ShowCounters;
		private System.Windows.Forms.CheckBox checkBox_ShowLength;
		private System.Windows.Forms.ComboBox comboBox_Radix;
		private System.Windows.Forms.Label label_Radix;
	}
}