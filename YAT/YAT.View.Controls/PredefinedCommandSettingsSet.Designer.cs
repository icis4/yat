namespace YAT.View.Controls
{
	partial class PredefinedCommandSettingsSet
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PredefinedCommandSettingsSet));
			this.pathLabel_FilePath = new MKY.Windows.Forms.PathLabel();
			this.checkBox_IsFile = new System.Windows.Forms.CheckBox();
			this.button_SetMultiLineText = new System.Windows.Forms.Button();
			this.textBox_Description = new MKY.Windows.Forms.TextBoxEx();
			this.textBox_SingleLineText = new MKY.Windows.Forms.TextBoxEx();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.button_SetFile = new System.Windows.Forms.Button();
			this.button_Clear = new System.Windows.Forms.Button();
			this.comboBox_ExplicitDefaultRadix = new System.Windows.Forms.ComboBox();
			this.splitContainer_ExplicitDefaultRadix = new System.Windows.Forms.SplitContainer();
			this.panel_ExplicitDefaultRadix = new System.Windows.Forms.Panel();
			this.panel_Command = new System.Windows.Forms.Panel();
			this.splitContainer_ExplicitDefaultRadix.Panel1.SuspendLayout();
			this.splitContainer_ExplicitDefaultRadix.Panel2.SuspendLayout();
			this.splitContainer_ExplicitDefaultRadix.SuspendLayout();
			this.panel_ExplicitDefaultRadix.SuspendLayout();
			this.panel_Command.SuspendLayout();
			this.SuspendLayout();
			// 
			// pathLabel_FilePath
			// 
			this.pathLabel_FilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pathLabel_FilePath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pathLabel_FilePath.Location = new System.Drawing.Point(0, 1);
			this.pathLabel_FilePath.Name = "pathLabel_FilePath";
			this.pathLabel_FilePath.Size = new System.Drawing.Size(232, 20);
			this.pathLabel_FilePath.TabIndex = 1;
			this.pathLabel_FilePath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolTip.SetToolTip(this.pathLabel_FilePath, "File path, use [...] to browse for a file");
			this.pathLabel_FilePath.Click += new System.EventHandler(this.pathLabel_FilePath_Click);
			// 
			// checkBox_IsFile
			// 
			this.checkBox_IsFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBox_IsFile.AutoSize = true;
			this.checkBox_IsFile.Location = new System.Drawing.Point(269, 4);
			this.checkBox_IsFile.Name = "checkBox_IsFile";
			this.checkBox_IsFile.Size = new System.Drawing.Size(15, 14);
			this.checkBox_IsFile.TabIndex = 4;
			this.toolTip.SetToolTip(this.checkBox_IsFile, "Tick to set file, untick to set text");
			this.checkBox_IsFile.CheckedChanged += new System.EventHandler(this.checkBox_IsFile_CheckedChanged);
			// 
			// button_SetMultiLineText
			// 
			this.button_SetMultiLineText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_SetMultiLineText.Location = new System.Drawing.Point(238, 0);
			this.button_SetMultiLineText.Name = "button_SetMultiLineText";
			this.button_SetMultiLineText.Size = new System.Drawing.Size(25, 21);
			this.button_SetMultiLineText.TabIndex = 2;
			this.button_SetMultiLineText.Text = "...";
			this.toolTip.SetToolTip(this.button_SetMultiLineText, "Enter multi-line text.\r\n\r\nLines will be sent sequentially,\r\nby default, the EOL s" +
        "equence\r\nwill be appended to each line.");
			this.button_SetMultiLineText.Click += new System.EventHandler(this.button_SetMultiLineText_Click);
			// 
			// textBox_Description
			// 
			this.textBox_Description.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox_Description.Location = new System.Drawing.Point(289, 1);
			this.textBox_Description.Name = "textBox_Description";
			this.textBox_Description.Size = new System.Drawing.Size(181, 20);
			this.textBox_Description.TabIndex = 5;
			this.toolTip.SetToolTip(this.textBox_Description, "Command description");
			this.textBox_Description.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_Description_Validating);
			// 
			// textBox_SingleLineText
			// 
			this.textBox_SingleLineText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox_SingleLineText.Location = new System.Drawing.Point(0, 1);
			this.textBox_SingleLineText.Name = "textBox_SingleLineText";
			this.textBox_SingleLineText.Size = new System.Drawing.Size(232, 20);
			this.textBox_SingleLineText.TabIndex = 0;
			this.toolTip.SetToolTip(this.textBox_SingleLineText, "Enter text, use [...] to enter multi-line text");
			this.textBox_SingleLineText.TextChanged += new System.EventHandler(this.textBox_SingleLineText_TextChanged);
			this.textBox_SingleLineText.Enter += new System.EventHandler(this.textBox_SingleLineText_Enter);
			this.textBox_SingleLineText.Leave += new System.EventHandler(this.textBox_SingleLineText_Leave);
			this.textBox_SingleLineText.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_SingleLineText_Validating);
			// 
			// button_SetFile
			// 
			this.button_SetFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_SetFile.Location = new System.Drawing.Point(238, 0);
			this.button_SetFile.Name = "button_SetFile";
			this.button_SetFile.Size = new System.Drawing.Size(25, 21);
			this.button_SetFile.TabIndex = 3;
			this.button_SetFile.Text = "...";
			this.toolTip.SetToolTip(this.button_SetFile, resources.GetString("button_SetFile.ToolTip"));
			this.button_SetFile.Click += new System.EventHandler(this.button_SetFile_Click);
			// 
			// button_Clear
			// 
			this.button_Clear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Clear.Image = global::YAT.View.Controls.Properties.Resources.Image_Tool_lightning_16x16;
			this.button_Clear.Location = new System.Drawing.Point(476, 0);
			this.button_Clear.Name = "button_Clear";
			this.button_Clear.Size = new System.Drawing.Size(24, 21);
			this.button_Clear.TabIndex = 6;
			this.toolTip.SetToolTip(this.button_Clear, "Clear command");
			this.button_Clear.Click += new System.EventHandler(this.button_Clear_Click);
			// 
			// comboBox_ExplicitDefaultRadix
			// 
			this.comboBox_ExplicitDefaultRadix.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_ExplicitDefaultRadix.FormattingEnabled = true;
			this.comboBox_ExplicitDefaultRadix.Location = new System.Drawing.Point(0, 0);
			this.comboBox_ExplicitDefaultRadix.Name = "comboBox_ExplicitDefaultRadix";
			this.comboBox_ExplicitDefaultRadix.Size = new System.Drawing.Size(77, 21);
			this.comboBox_ExplicitDefaultRadix.TabIndex = 0;
			this.toolTip.SetToolTip(this.comboBox_ExplicitDefaultRadix, "Select the radix which is used by default,\r\ni.e. without an escape sequence.\r\n\r\nI" +
        "t applies to sending text files (incl. RTF, XML),\r\nbut not to sending binary fil" +
        "es.");
			this.comboBox_ExplicitDefaultRadix.Validating += new System.ComponentModel.CancelEventHandler(this.comboBox_ExplicitDefaultRadix_Validating);
			// 
			// splitContainer_ExplicitDefaultRadix
			// 
			this.splitContainer_ExplicitDefaultRadix.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer_ExplicitDefaultRadix.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer_ExplicitDefaultRadix.IsSplitterFixed = true;
			this.splitContainer_ExplicitDefaultRadix.Location = new System.Drawing.Point(0, 0);
			this.splitContainer_ExplicitDefaultRadix.Name = "splitContainer_ExplicitDefaultRadix";
			// 
			// splitContainer_ExplicitDefaultRadix.Panel1
			// 
			this.splitContainer_ExplicitDefaultRadix.Panel1.Controls.Add(this.panel_ExplicitDefaultRadix);
			// 
			// splitContainer_ExplicitDefaultRadix.Panel2
			// 
			this.splitContainer_ExplicitDefaultRadix.Panel2.Controls.Add(this.panel_Command);
			this.splitContainer_ExplicitDefaultRadix.Size = new System.Drawing.Size(584, 21);
			this.splitContainer_ExplicitDefaultRadix.SplitterDistance = 80;
			this.splitContainer_ExplicitDefaultRadix.SplitterWidth = 1;
			this.splitContainer_ExplicitDefaultRadix.TabIndex = 0;
			// 
			// panel_ExplicitDefaultRadix
			// 
			this.panel_ExplicitDefaultRadix.Controls.Add(this.comboBox_ExplicitDefaultRadix);
			this.panel_ExplicitDefaultRadix.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_ExplicitDefaultRadix.Location = new System.Drawing.Point(0, 0);
			this.panel_ExplicitDefaultRadix.Name = "panel_ExplicitDefaultRadix";
			this.panel_ExplicitDefaultRadix.Size = new System.Drawing.Size(80, 21);
			this.panel_ExplicitDefaultRadix.TabIndex = 1;
			// 
			// panel_Command
			// 
			this.panel_Command.Controls.Add(this.checkBox_IsFile);
			this.panel_Command.Controls.Add(this.button_Clear);
			this.panel_Command.Controls.Add(this.textBox_Description);
			this.panel_Command.Controls.Add(this.textBox_SingleLineText);
			this.panel_Command.Controls.Add(this.pathLabel_FilePath);
			this.panel_Command.Controls.Add(this.button_SetFile);
			this.panel_Command.Controls.Add(this.button_SetMultiLineText);
			this.panel_Command.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_Command.Location = new System.Drawing.Point(0, 0);
			this.panel_Command.Name = "panel_Command";
			this.panel_Command.Size = new System.Drawing.Size(503, 21);
			this.panel_Command.TabIndex = 0;
			// 
			// PredefinedCommandSettingsSet
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.Controls.Add(this.splitContainer_ExplicitDefaultRadix);
			this.Name = "PredefinedCommandSettingsSet";
			this.Size = new System.Drawing.Size(584, 21);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.PredefinedCommandSettingsSet_Paint);
			this.Enter += new System.EventHandler(this.PredefinedCommandSettingsSet_Enter);
			this.splitContainer_ExplicitDefaultRadix.Panel1.ResumeLayout(false);
			this.splitContainer_ExplicitDefaultRadix.Panel2.ResumeLayout(false);
			this.splitContainer_ExplicitDefaultRadix.ResumeLayout(false);
			this.panel_ExplicitDefaultRadix.ResumeLayout(false);
			this.panel_Command.ResumeLayout(false);
			this.panel_Command.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private MKY.Windows.Forms.PathLabel pathLabel_FilePath;
		private System.Windows.Forms.CheckBox checkBox_IsFile;
		private System.Windows.Forms.Button button_SetMultiLineText;
		private MKY.Windows.Forms.TextBoxEx textBox_Description;
		private MKY.Windows.Forms.TextBoxEx textBox_SingleLineText;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Button button_SetFile;
		private System.Windows.Forms.Button button_Clear;
		private System.Windows.Forms.SplitContainer splitContainer_ExplicitDefaultRadix;
		private System.Windows.Forms.ComboBox comboBox_ExplicitDefaultRadix;
		private System.Windows.Forms.Panel panel_ExplicitDefaultRadix;
		private System.Windows.Forms.Panel panel_Command;
	}
}
