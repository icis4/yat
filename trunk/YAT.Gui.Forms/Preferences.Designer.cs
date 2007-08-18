namespace MKY.YAT.Gui.Forms
{
	partial class Preferences
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
			this.groupBox_Preferences = new System.Windows.Forms.GroupBox();
			this.checkBox_AutoOpenWorkspace = new System.Windows.Forms.CheckBox();
			this.checkBox_AutoSaveWorkspace = new System.Windows.Forms.CheckBox();
			this.checkBox_DetectSerialPortsInUse = new System.Windows.Forms.CheckBox();
			this.groupBox_Preferences.SuspendLayout();
			this.SuspendLayout();
			// 
			// button_Defaults
			// 
			this.button_Defaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Defaults.Location = new System.Drawing.Point(331, 87);
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
			this.button_Cancel.Location = new System.Drawing.Point(331, 58);
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
			this.button_OK.Location = new System.Drawing.Point(331, 29);
			this.button_OK.Name = "button_OK";
			this.button_OK.Size = new System.Drawing.Size(75, 23);
			this.button_OK.TabIndex = 1;
			this.button_OK.Text = "OK";
			this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
			// 
			// groupBox_Preferences
			// 
			this.groupBox_Preferences.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Preferences.Controls.Add(this.checkBox_DetectSerialPortsInUse);
			this.groupBox_Preferences.Controls.Add(this.checkBox_AutoSaveWorkspace);
			this.groupBox_Preferences.Controls.Add(this.checkBox_AutoOpenWorkspace);
			this.groupBox_Preferences.Location = new System.Drawing.Point(12, 12);
			this.groupBox_Preferences.Name = "groupBox_Preferences";
			this.groupBox_Preferences.Size = new System.Drawing.Size(305, 113);
			this.groupBox_Preferences.TabIndex = 0;
			this.groupBox_Preferences.TabStop = false;
			// 
			// checkBox_AutoOpenWorkspace
			// 
			this.checkBox_AutoOpenWorkspace.AutoSize = true;
			this.checkBox_AutoOpenWorkspace.Location = new System.Drawing.Point(18, 21);
			this.checkBox_AutoOpenWorkspace.Name = "checkBox_AutoOpenWorkspace";
			this.checkBox_AutoOpenWorkspace.Size = new System.Drawing.Size(208, 17);
			this.checkBox_AutoOpenWorkspace.TabIndex = 0;
			this.checkBox_AutoOpenWorkspace.Text = "&Open last active workspace on startup";
			this.checkBox_AutoOpenWorkspace.UseVisualStyleBackColor = true;
			this.checkBox_AutoOpenWorkspace.CheckedChanged += new System.EventHandler(this.checkBox_AutoOpenWorkspace_CheckedChanged);
			// 
			// checkBox_AutoSaveWorkspace
			// 
			this.checkBox_AutoSaveWorkspace.AutoSize = true;
			this.checkBox_AutoSaveWorkspace.Location = new System.Drawing.Point(18, 44);
			this.checkBox_AutoSaveWorkspace.Name = "checkBox_AutoSaveWorkspace";
			this.checkBox_AutoSaveWorkspace.Size = new System.Drawing.Size(206, 17);
			this.checkBox_AutoSaveWorkspace.TabIndex = 1;
			this.checkBox_AutoSaveWorkspace.Text = "&Save current workspace automatically";
			this.checkBox_AutoSaveWorkspace.UseVisualStyleBackColor = true;
			this.checkBox_AutoSaveWorkspace.CheckedChanged += new System.EventHandler(this.checkBox_AutoSaveWorkspace_CheckedChanged);
			// 
			// checkBox_DetectSerialPortsInUse
			// 
			this.checkBox_DetectSerialPortsInUse.AutoSize = true;
			this.checkBox_DetectSerialPortsInUse.Location = new System.Drawing.Point(18, 67);
			this.checkBox_DetectSerialPortsInUse.Name = "checkBox_DetectSerialPortsInUse";
			this.checkBox_DetectSerialPortsInUse.Size = new System.Drawing.Size(269, 17);
			this.checkBox_DetectSerialPortsInUse.TabIndex = 2;
			this.checkBox_DetectSerialPortsInUse.Text = "When listing serial ports, &detect ports that are in use";
			this.checkBox_DetectSerialPortsInUse.UseVisualStyleBackColor = true;
			this.checkBox_DetectSerialPortsInUse.CheckedChanged += new System.EventHandler(this.checkBox_DetectSerialPortsInUse_CheckedChanged);
			// 
			// Preferences
			// 
			this.AcceptButton = this.button_OK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button_Cancel;
			this.ClientSize = new System.Drawing.Size(418, 137);
			this.Controls.Add(this.groupBox_Preferences);
			this.Controls.Add(this.button_Defaults);
			this.Controls.Add(this.button_Cancel);
			this.Controls.Add(this.button_OK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Preferences";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Preferences";
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.Preferences_Paint);
			this.groupBox_Preferences.ResumeLayout(false);
			this.groupBox_Preferences.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button button_Defaults;
		private System.Windows.Forms.Button button_Cancel;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.GroupBox groupBox_Preferences;
		private System.Windows.Forms.CheckBox checkBox_DetectSerialPortsInUse;
		private System.Windows.Forms.CheckBox checkBox_AutoSaveWorkspace;
		private System.Windows.Forms.CheckBox checkBox_AutoOpenWorkspace;
	}
}