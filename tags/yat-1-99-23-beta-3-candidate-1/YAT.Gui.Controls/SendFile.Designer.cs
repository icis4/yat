namespace YAT.Gui.Controls
{
	partial class SendFile
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
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.pathLabel_FilePath = new MKY.Windows.Forms.PathLabel();
            this.button_SetFile = new System.Windows.Forms.Button();
            this.button_SendFile = new System.Windows.Forms.Button();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // pathLabel_FilePath
            // 
            this.pathLabel_FilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pathLabel_FilePath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pathLabel_FilePath.Location = new System.Drawing.Point(3, 3);
            this.pathLabel_FilePath.Name = "pathLabel_FilePath";
            this.pathLabel_FilePath.Size = new System.Drawing.Size(319, 21);
            this.pathLabel_FilePath.TabIndex = 1;
            this.pathLabel_FilePath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolTip.SetToolTip(this.pathLabel_FilePath, "File Path, click or press <...> to choose file");
            this.pathLabel_FilePath.Click += new System.EventHandler(this.pathLabel_FilePath_Click);
            // 
            // button_SetFile
            // 
            this.button_SetFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_SetFile.Location = new System.Drawing.Point(328, 3);
            this.button_SetFile.Name = "button_SetFile";
            this.button_SetFile.Size = new System.Drawing.Size(25, 21);
            this.button_SetFile.TabIndex = 3;
            this.button_SetFile.Text = "...";
            this.toolTip.SetToolTip(this.button_SetFile, "Choose File");
            this.button_SetFile.Click += new System.EventHandler(this.button_SetFile_Click);
            // 
            // button_SendFile
            // 
            this.button_SendFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.button_SendFile.Enabled = false;
            this.button_SendFile.Location = new System.Drawing.Point(3, 3);
            this.button_SendFile.Name = "button_SendFile";
            this.button_SendFile.Size = new System.Drawing.Size(176, 21);
            this.button_SendFile.TabIndex = 4;
            this.button_SendFile.Text = "Send File (F4)";
            this.toolTip.SetToolTip(this.button_SendFile, "Send File");
            this.button_SendFile.Click += new System.EventHandler(this.button_SendFile_Click);
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.IsSplitterFixed = true;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.button_SetFile);
            this.splitContainer.Panel1.Controls.Add(this.pathLabel_FilePath);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.button_SendFile);
            this.splitContainer.Size = new System.Drawing.Size(540, 27);
            this.splitContainer.SplitterDistance = 356;
            this.splitContainer.SplitterWidth = 1;
            this.splitContainer.TabIndex = 3;
            // 
            // SendFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Name = "SendFile";
            this.Size = new System.Drawing.Size(540, 27);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.SplitContainer splitContainer;
        private MKY.Windows.Forms.PathLabel pathLabel_FilePath;
        private System.Windows.Forms.Button button_SetFile;
        private System.Windows.Forms.Button button_SendFile;
	}
}
