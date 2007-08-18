namespace MKY.YAT.Gui.Controls
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
			this.button_SendFile = new System.Windows.Forms.Button();
			this.button_SetFile = new System.Windows.Forms.Button();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.pathLabel_FilePath = new MKY.Windows.Forms.PathLabel();
			this.SuspendLayout();
			// 
			// button_SendFile
			// 
			this.button_SendFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_SendFile.Enabled = false;
			this.button_SendFile.Location = new System.Drawing.Point(380, 3);
			this.button_SendFile.Name = "button_SendFile";
			this.button_SendFile.Size = new System.Drawing.Size(157, 21);
			this.button_SendFile.TabIndex = 2;
			this.button_SendFile.Text = "Send File (F4)";
			this.toolTip.SetToolTip(this.button_SendFile, "Send File");
			this.button_SendFile.Click += new System.EventHandler(this.button_SendFile_Click);
			// 
			// button_SetFile
			// 
			this.button_SetFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_SetFile.Location = new System.Drawing.Point(349, 3);
			this.button_SetFile.Name = "button_SetFile";
			this.button_SetFile.Size = new System.Drawing.Size(25, 21);
			this.button_SetFile.TabIndex = 1;
			this.button_SetFile.Text = "...";
			this.toolTip.SetToolTip(this.button_SetFile, "Choose File");
			this.button_SetFile.Click += new System.EventHandler(this.button_SetFile_Click);
			// 
			// pathLabel_FilePath
			// 
			this.pathLabel_FilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.pathLabel_FilePath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pathLabel_FilePath.Location = new System.Drawing.Point(3, 3);
			this.pathLabel_FilePath.Name = "pathLabel_FilePath";
			this.pathLabel_FilePath.Size = new System.Drawing.Size(340, 21);
			this.pathLabel_FilePath.TabIndex = 0;
			this.pathLabel_FilePath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolTip.SetToolTip(this.pathLabel_FilePath, "File Path, click or press <...> to choose file");
			this.pathLabel_FilePath.Click += new System.EventHandler(this.pathLabel_FilePath_Click);
			// 
			// SendFile
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.button_SetFile);
			this.Controls.Add(this.button_SendFile);
			this.Controls.Add(this.pathLabel_FilePath);
			this.Name = "SendFile";
			this.Size = new System.Drawing.Size(540, 27);
			this.ResumeLayout(false);

		}

		#endregion

		private MKY.Windows.Forms.PathLabel pathLabel_FilePath;
		private System.Windows.Forms.Button button_SendFile;
		private System.Windows.Forms.Button button_SetFile;
		private System.Windows.Forms.ToolTip toolTip;
	}
}
