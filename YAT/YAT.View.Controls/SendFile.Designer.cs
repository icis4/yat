namespace YAT.View.Controls
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
			this.button_OpenFile = new System.Windows.Forms.Button();
			this.button_Send = new System.Windows.Forms.Button();
			this.pathComboBox_FilePath = new MKY.Windows.Forms.PathComboBox();
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// button_OpenFile
			// 
			this.button_OpenFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_OpenFile.Location = new System.Drawing.Point(328, 3);
			this.button_OpenFile.Name = "button_OpenFile";
			this.button_OpenFile.Size = new System.Drawing.Size(25, 21);
			this.button_OpenFile.TabIndex = 3;
			this.button_OpenFile.Text = "...";
			this.toolTip.SetToolTip(this.button_OpenFile, "Browse for a file");
			this.button_OpenFile.Click += new System.EventHandler(this.button_OpenFile_Click);
			// 
			// button_Send
			// 
			this.button_Send.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Send.Enabled = false;
			this.button_Send.Location = new System.Drawing.Point(3, 3);
			this.button_Send.Name = "button_Send";
			this.button_Send.Size = new System.Drawing.Size(176, 21);
			this.button_Send.TabIndex = 4;
			this.button_Send.Text = "Send File (F4)";
			this.toolTip.SetToolTip(this.button_Send, "Send file");
			this.button_Send.Click += new System.EventHandler(this.button_Send_Click);
			// 
			// pathComboBox_FilePath
			// 
			this.pathComboBox_FilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pathComboBox_FilePath.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.pathComboBox_FilePath.ForeColor = System.Drawing.SystemColors.GrayText;
			this.pathComboBox_FilePath.FormattingEnabled = true;
			this.pathComboBox_FilePath.Location = new System.Drawing.Point(3, 3);
			this.pathComboBox_FilePath.Name = "pathComboBox_FilePath";
			this.pathComboBox_FilePath.Size = new System.Drawing.Size(319, 21);
			this.pathComboBox_FilePath.TabIndex = 1;
			this.toolTip.SetToolTip(this.pathComboBox_FilePath, "<Enter> to send file, drop down\r\nfor recent files, <...> to browse for a file");
			this.pathComboBox_FilePath.SelectedIndexChanged += new System.EventHandler(this.pathComboBox_FilePath_SelectedIndexChanged);
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
			this.splitContainer.Panel1.Controls.Add(this.pathComboBox_FilePath);
			this.splitContainer.Panel1.Controls.Add(this.button_OpenFile);
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.button_Send);
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
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.SendFile_Paint);
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel2.ResumeLayout(false);
			this.splitContainer.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.SplitContainer splitContainer;
		private System.Windows.Forms.Button button_OpenFile;
		private System.Windows.Forms.Button button_Send;
		private MKY.Windows.Forms.PathComboBox pathComboBox_FilePath;
	}
}
