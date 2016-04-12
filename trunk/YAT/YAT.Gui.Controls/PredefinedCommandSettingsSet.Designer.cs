namespace YAT.Gui.Controls
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
			this.pathLabel_FilePath = new MKY.Windows.Forms.PathLabel();
			this.checkBox_IsFile = new System.Windows.Forms.CheckBox();
			this.button_SetMultiLineText = new System.Windows.Forms.Button();
			this.label_Shortcut = new System.Windows.Forms.Label();
			this.textBox_Description = new System.Windows.Forms.TextBox();
			this.textBox_SingleLineText = new System.Windows.Forms.TextBox();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.button_SetFile = new System.Windows.Forms.Button();
			this.button_Delete = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// pathLabel_FilePath
			// 
			this.pathLabel_FilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pathLabel_FilePath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pathLabel_FilePath.Location = new System.Drawing.Point(21, 0);
			this.pathLabel_FilePath.Name = "pathLabel_FilePath";
			this.pathLabel_FilePath.Size = new System.Drawing.Size(313, 20);
			this.pathLabel_FilePath.TabIndex = 2;
			this.pathLabel_FilePath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolTip.SetToolTip(this.pathLabel_FilePath, "File path, click or press <...> to browse for a file");
			this.pathLabel_FilePath.Click += new System.EventHandler(this.pathLabel_FilePath_Click);
			// 
			// checkBox_IsFile
			// 
			this.checkBox_IsFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBox_IsFile.AutoSize = true;
			this.checkBox_IsFile.Location = new System.Drawing.Point(3, 3);
			this.checkBox_IsFile.Name = "checkBox_IsFile";
			this.checkBox_IsFile.Size = new System.Drawing.Size(15, 14);
			this.checkBox_IsFile.TabIndex = 0;
			this.toolTip.SetToolTip(this.checkBox_IsFile, "Tick to set file, untick to set text");
			this.checkBox_IsFile.CheckedChanged += new System.EventHandler(this.checkBox_IsFile_CheckedChanged);
			// 
			// button_SetMultiLineText
			// 
			this.button_SetMultiLineText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_SetMultiLineText.Location = new System.Drawing.Point(340, 0);
			this.button_SetMultiLineText.Name = "button_SetMultiLineText";
			this.button_SetMultiLineText.Size = new System.Drawing.Size(25, 20);
			this.button_SetMultiLineText.TabIndex = 3;
			this.button_SetMultiLineText.Text = "...";
			this.toolTip.SetToolTip(this.button_SetMultiLineText, "Multi-line text");
			this.button_SetMultiLineText.Click += new System.EventHandler(this.button_SetMultiLineText_Click);
			// 
			// label_Shortcut
			// 
			this.label_Shortcut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label_Shortcut.Location = new System.Drawing.Point(497, 0);
			this.label_Shortcut.Name = "label_Shortcut";
			this.label_Shortcut.Size = new System.Drawing.Size(55, 20);
			this.label_Shortcut.TabIndex = 6;
			this.label_Shortcut.Text = "Shift+F1";
			this.label_Shortcut.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox_Description
			// 
			this.textBox_Description.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox_Description.Location = new System.Drawing.Point(371, 0);
			this.textBox_Description.Name = "textBox_Description";
			this.textBox_Description.Size = new System.Drawing.Size(120, 20);
			this.textBox_Description.TabIndex = 5;
			this.toolTip.SetToolTip(this.textBox_Description, "Command description");
			this.textBox_Description.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_Description_Validating);
			// 
			// textBox_SingleLineText
			// 
			this.textBox_SingleLineText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox_SingleLineText.Location = new System.Drawing.Point(21, 0);
			this.textBox_SingleLineText.Name = "textBox_SingleLineText";
			this.textBox_SingleLineText.Size = new System.Drawing.Size(313, 20);
			this.textBox_SingleLineText.TabIndex = 1;
			this.toolTip.SetToolTip(this.textBox_SingleLineText, "Enter text, press <...> to enter multi-line text");
			this.textBox_SingleLineText.TextChanged += new System.EventHandler(this.textBox_SingleLineText_TextChanged);
			this.textBox_SingleLineText.Enter += new System.EventHandler(this.textBox_SingleLineText_Enter);
			this.textBox_SingleLineText.Leave += new System.EventHandler(this.textBox_SingleLineText_Leave);
			this.textBox_SingleLineText.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_SingleLineText_Validating);
			// 
			// button_SetFile
			// 
			this.button_SetFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_SetFile.Location = new System.Drawing.Point(340, 0);
			this.button_SetFile.Name = "button_SetFile";
			this.button_SetFile.Size = new System.Drawing.Size(25, 20);
			this.button_SetFile.TabIndex = 4;
			this.button_SetFile.Text = "...";
			this.toolTip.SetToolTip(this.button_SetFile, "Browse for file");
			this.button_SetFile.Click += new System.EventHandler(this.button_SetFile_Click);
			// 
			// button_Delete
			// 
			this.button_Delete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Delete.Image = global::YAT.Gui.Controls.Properties.Resources.Image_Tool_lightning_16x16;
			this.button_Delete.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.button_Delete.Location = new System.Drawing.Point(557, 0);
			this.button_Delete.Name = "button_Delete";
			this.button_Delete.Size = new System.Drawing.Size(24, 20);
			this.button_Delete.TabIndex = 7;
			this.toolTip.SetToolTip(this.button_Delete, "Clear command");
			this.button_Delete.Click += new System.EventHandler(this.button_Delete_Click);
			// 
			// PredefinedCommandSettingsSet
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.button_Delete);
			this.Controls.Add(this.textBox_SingleLineText);
			this.Controls.Add(this.pathLabel_FilePath);
			this.Controls.Add(this.checkBox_IsFile);
			this.Controls.Add(this.button_SetFile);
			this.Controls.Add(this.button_SetMultiLineText);
			this.Controls.Add(this.label_Shortcut);
			this.Controls.Add(this.textBox_Description);
			this.Name = "PredefinedCommandSettingsSet";
			this.Size = new System.Drawing.Size(584, 20);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.PredefinedCommandSettingsSet_Paint);
			this.Enter += new System.EventHandler(this.PredefinedCommandSettingsSet_Enter);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private MKY.Windows.Forms.PathLabel pathLabel_FilePath;
		private System.Windows.Forms.CheckBox checkBox_IsFile;
		private System.Windows.Forms.Button button_SetMultiLineText;
		private System.Windows.Forms.Label label_Shortcut;
		private System.Windows.Forms.TextBox textBox_Description;
		private System.Windows.Forms.TextBox textBox_SingleLineText;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Button button_SetFile;
		private System.Windows.Forms.Button button_Delete;
	}
}
