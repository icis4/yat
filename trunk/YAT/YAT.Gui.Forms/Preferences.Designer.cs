namespace YAT.Gui.Forms
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.checkBox_ShowChrono = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowTerminalInfo = new System.Windows.Forms.CheckBox();
			this.groupBox_Port = new System.Windows.Forms.GroupBox();
			this.checkBox_DetectSerialPortsInUse = new System.Windows.Forms.CheckBox();
			this.groupBox_Workspace = new System.Windows.Forms.GroupBox();
			this.checkBox_UseRelativePaths = new System.Windows.Forms.CheckBox();
			this.checkBox_AutoSaveWorkspace = new System.Windows.Forms.CheckBox();
			this.checkBox_AutoOpenWorkspace = new System.Windows.Forms.CheckBox();
			this.groupBox_Preferences.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox_Port.SuspendLayout();
			this.groupBox_Workspace.SuspendLayout();
			this.SuspendLayout();
			// 
			// button_Defaults
			// 
			this.button_Defaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Defaults.Location = new System.Drawing.Point(331, 97);
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
			this.button_Cancel.Location = new System.Drawing.Point(331, 68);
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
			this.button_OK.Location = new System.Drawing.Point(331, 39);
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
			this.groupBox_Preferences.Controls.Add(this.groupBox1);
			this.groupBox_Preferences.Controls.Add(this.groupBox_Port);
			this.groupBox_Preferences.Controls.Add(this.groupBox_Workspace);
			this.groupBox_Preferences.Location = new System.Drawing.Point(12, 12);
			this.groupBox_Preferences.Name = "groupBox_Preferences";
			this.groupBox_Preferences.Size = new System.Drawing.Size(305, 248);
			this.groupBox_Preferences.TabIndex = 0;
			this.groupBox_Preferences.TabStop = false;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.checkBox_ShowChrono);
			this.groupBox1.Controls.Add(this.checkBox_ShowTerminalInfo);
			this.groupBox1.Location = new System.Drawing.Point(6, 14);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(292, 70);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Main";
			// 
			// checkBox_ShowChrono
			// 
			this.checkBox_ShowChrono.AutoSize = true;
			this.checkBox_ShowChrono.Location = new System.Drawing.Point(12, 42);
			this.checkBox_ShowChrono.Name = "checkBox_ShowChrono";
			this.checkBox_ShowChrono.Size = new System.Drawing.Size(149, 17);
			this.checkBox_ShowChrono.TabIndex = 1;
			this.checkBox_ShowChrono.Text = "Show &chrono in status bar";
			this.checkBox_ShowChrono.UseVisualStyleBackColor = true;
			this.checkBox_ShowChrono.CheckedChanged += new System.EventHandler(this.checkBox_ShowChrono_CheckedChanged);
			// 
			// checkBox_ShowTerminalInfo
			// 
			this.checkBox_ShowTerminalInfo.AutoSize = true;
			this.checkBox_ShowTerminalInfo.Location = new System.Drawing.Point(12, 19);
			this.checkBox_ShowTerminalInfo.Name = "checkBox_ShowTerminalInfo";
			this.checkBox_ShowTerminalInfo.Size = new System.Drawing.Size(188, 17);
			this.checkBox_ShowTerminalInfo.TabIndex = 0;
			this.checkBox_ShowTerminalInfo.Text = "Show &terminal indices in status bar";
			this.checkBox_ShowTerminalInfo.UseVisualStyleBackColor = true;
			this.checkBox_ShowTerminalInfo.CheckedChanged += new System.EventHandler(this.checkBox_ShowTerminalInfo_CheckedChanged);
			// 
			// groupBox_Port
			// 
			this.groupBox_Port.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Port.Controls.Add(this.checkBox_DetectSerialPortsInUse);
			this.groupBox_Port.Location = new System.Drawing.Point(6, 192);
			this.groupBox_Port.Name = "groupBox_Port";
			this.groupBox_Port.Size = new System.Drawing.Size(293, 49);
			this.groupBox_Port.TabIndex = 2;
			this.groupBox_Port.TabStop = false;
			this.groupBox_Port.Text = "Port";
			// 
			// checkBox_DetectSerialPortsInUse
			// 
			this.checkBox_DetectSerialPortsInUse.AutoSize = true;
			this.checkBox_DetectSerialPortsInUse.Location = new System.Drawing.Point(12, 19);
			this.checkBox_DetectSerialPortsInUse.Name = "checkBox_DetectSerialPortsInUse";
			this.checkBox_DetectSerialPortsInUse.Size = new System.Drawing.Size(269, 17);
			this.checkBox_DetectSerialPortsInUse.TabIndex = 0;
			this.checkBox_DetectSerialPortsInUse.Text = "When listing serial ports, &detect ports that are in use";
			this.checkBox_DetectSerialPortsInUse.UseVisualStyleBackColor = true;
			this.checkBox_DetectSerialPortsInUse.CheckedChanged += new System.EventHandler(this.checkBox_DetectSerialPortsInUse_CheckedChanged);
			// 
			// groupBox_Workspace
			// 
			this.groupBox_Workspace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Workspace.Controls.Add(this.checkBox_UseRelativePaths);
			this.groupBox_Workspace.Controls.Add(this.checkBox_AutoSaveWorkspace);
			this.groupBox_Workspace.Controls.Add(this.checkBox_AutoOpenWorkspace);
			this.groupBox_Workspace.Location = new System.Drawing.Point(6, 90);
			this.groupBox_Workspace.Name = "groupBox_Workspace";
			this.groupBox_Workspace.Size = new System.Drawing.Size(293, 96);
			this.groupBox_Workspace.TabIndex = 1;
			this.groupBox_Workspace.TabStop = false;
			this.groupBox_Workspace.Text = "Workspace";
			// 
			// checkBox_UseRelativePaths
			// 
			this.checkBox_UseRelativePaths.AutoSize = true;
			this.checkBox_UseRelativePaths.Location = new System.Drawing.Point(12, 65);
			this.checkBox_UseRelativePaths.Name = "checkBox_UseRelativePaths";
			this.checkBox_UseRelativePaths.Size = new System.Drawing.Size(229, 17);
			this.checkBox_UseRelativePaths.TabIndex = 2;
			this.checkBox_UseRelativePaths.Text = "Use &relative paths when saving workspace";
			this.checkBox_UseRelativePaths.UseVisualStyleBackColor = true;
			this.checkBox_UseRelativePaths.CheckedChanged += new System.EventHandler(this.checkBox_UseRelativePaths_CheckedChanged);
			// 
			// checkBox_AutoSaveWorkspace
			// 
			this.checkBox_AutoSaveWorkspace.AutoSize = true;
			this.checkBox_AutoSaveWorkspace.Location = new System.Drawing.Point(12, 42);
			this.checkBox_AutoSaveWorkspace.Name = "checkBox_AutoSaveWorkspace";
			this.checkBox_AutoSaveWorkspace.Size = new System.Drawing.Size(206, 17);
			this.checkBox_AutoSaveWorkspace.TabIndex = 1;
			this.checkBox_AutoSaveWorkspace.Text = "&Save current workspace automatically";
			this.checkBox_AutoSaveWorkspace.UseVisualStyleBackColor = true;
			this.checkBox_AutoSaveWorkspace.CheckedChanged += new System.EventHandler(this.checkBox_AutoSaveWorkspace_CheckedChanged);
			// 
			// checkBox_AutoOpenWorkspace
			// 
			this.checkBox_AutoOpenWorkspace.AutoSize = true;
			this.checkBox_AutoOpenWorkspace.Location = new System.Drawing.Point(12, 19);
			this.checkBox_AutoOpenWorkspace.Name = "checkBox_AutoOpenWorkspace";
			this.checkBox_AutoOpenWorkspace.Size = new System.Drawing.Size(208, 17);
			this.checkBox_AutoOpenWorkspace.TabIndex = 0;
			this.checkBox_AutoOpenWorkspace.Text = "&Open last active workspace on startup";
			this.checkBox_AutoOpenWorkspace.UseVisualStyleBackColor = true;
			this.checkBox_AutoOpenWorkspace.CheckedChanged += new System.EventHandler(this.checkBox_AutoOpenWorkspace_CheckedChanged);
			// 
			// Preferences
			// 
			this.AcceptButton = this.button_OK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button_Cancel;
			this.ClientSize = new System.Drawing.Size(418, 272);
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
			this.Shown += new System.EventHandler(this.Preferences_Shown);
			this.groupBox_Preferences.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox_Port.ResumeLayout(false);
			this.groupBox_Port.PerformLayout();
			this.groupBox_Workspace.ResumeLayout(false);
			this.groupBox_Workspace.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button button_Defaults;
		private System.Windows.Forms.Button button_Cancel;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.GroupBox groupBox_Preferences;
		private System.Windows.Forms.GroupBox groupBox_Workspace;
		private System.Windows.Forms.CheckBox checkBox_AutoSaveWorkspace;
		private System.Windows.Forms.CheckBox checkBox_AutoOpenWorkspace;
		private System.Windows.Forms.GroupBox groupBox_Port;
		private System.Windows.Forms.CheckBox checkBox_DetectSerialPortsInUse;
		private System.Windows.Forms.CheckBox checkBox_UseRelativePaths;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox checkBox_ShowChrono;
		private System.Windows.Forms.CheckBox checkBox_ShowTerminalInfo;
	}
}