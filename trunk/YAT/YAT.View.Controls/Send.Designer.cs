namespace YAT.View.Controls
{
	partial class Send
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
			this.splitContainer_Send = new System.Windows.Forms.SplitContainer();
			this.panel_SendText = new System.Windows.Forms.Panel();
			this.groupBox_SendText = new System.Windows.Forms.GroupBox();
			this.sendText = new YAT.View.Controls.SendText();
			this.panel_SendFile = new System.Windows.Forms.Panel();
			this.groupBox_SendFile = new System.Windows.Forms.GroupBox();
			this.sendFile = new YAT.View.Controls.SendFile();
			this.splitContainer_Send.Panel1.SuspendLayout();
			this.splitContainer_Send.Panel2.SuspendLayout();
			this.splitContainer_Send.SuspendLayout();
			this.panel_SendText.SuspendLayout();
			this.groupBox_SendText.SuspendLayout();
			this.panel_SendFile.SuspendLayout();
			this.groupBox_SendFile.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer_Send
			// 
			this.splitContainer_Send.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer_Send.IsSplitterFixed = true;
			this.splitContainer_Send.Location = new System.Drawing.Point(0, 0);
			this.splitContainer_Send.Name = "splitContainer_Send";
			this.splitContainer_Send.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer_Send.Panel1
			// 
			this.splitContainer_Send.Panel1.Controls.Add(this.panel_SendText);
			this.splitContainer_Send.Panel1MinSize = 46;
			// 
			// splitContainer_Send.Panel2
			// 
			this.splitContainer_Send.Panel2.Controls.Add(this.panel_SendFile);
			this.splitContainer_Send.Panel2MinSize = 46;
			this.splitContainer_Send.Size = new System.Drawing.Size(712, 93);
			this.splitContainer_Send.SplitterDistance = 46;
			this.splitContainer_Send.SplitterWidth = 1;
			this.splitContainer_Send.TabIndex = 1;
			// 
			// panel_SendText
			// 
			this.panel_SendText.Controls.Add(this.groupBox_SendText);
			this.panel_SendText.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_SendText.Location = new System.Drawing.Point(0, 0);
			this.panel_SendText.Name = "panel_SendText";
			this.panel_SendText.Size = new System.Drawing.Size(712, 46);
			this.panel_SendText.TabIndex = 1;
			// 
			// groupBox_SendText
			// 
			this.groupBox_SendText.Controls.Add(this.sendText);
			this.groupBox_SendText.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox_SendText.Location = new System.Drawing.Point(0, 0);
			this.groupBox_SendText.Name = "groupBox_SendText";
			this.groupBox_SendText.Padding = new System.Windows.Forms.Padding(3, 0, 3, 6);
			this.groupBox_SendText.Size = new System.Drawing.Size(712, 46);
			this.groupBox_SendText.TabIndex = 0;
			this.groupBox_SendText.TabStop = false;
			this.groupBox_SendText.Text = "Send T&ext";
			// 
			// sendText
			// 
			this.sendText.Dock = System.Windows.Forms.DockStyle.Fill;
			this.sendText.Location = new System.Drawing.Point(3, 13);
			this.sendText.Name = "sendText";
			this.sendText.Size = new System.Drawing.Size(706, 27);
			this.sendText.TabIndex = 0;
			this.sendText.CommandChanged += new System.EventHandler(this.sendText_CommandChanged);
			this.sendText.TextFocusedChanged += new System.EventHandler(this.sendText_TextFocusedChanged);
			this.sendText.SendCommandRequest += new System.EventHandler<YAT.View.Controls.SendTextOptionEventArgs>(this.sendText_SendCommandRequest);
			// 
			// panel_SendFile
			// 
			this.panel_SendFile.Controls.Add(this.groupBox_SendFile);
			this.panel_SendFile.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_SendFile.Location = new System.Drawing.Point(0, 0);
			this.panel_SendFile.Name = "panel_SendFile";
			this.panel_SendFile.Size = new System.Drawing.Size(712, 46);
			this.panel_SendFile.TabIndex = 2;
			// 
			// groupBox_SendFile
			// 
			this.groupBox_SendFile.Controls.Add(this.sendFile);
			this.groupBox_SendFile.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox_SendFile.Location = new System.Drawing.Point(0, 0);
			this.groupBox_SendFile.Name = "groupBox_SendFile";
			this.groupBox_SendFile.Padding = new System.Windows.Forms.Padding(3, 0, 3, 6);
			this.groupBox_SendFile.Size = new System.Drawing.Size(712, 46);
			this.groupBox_SendFile.TabIndex = 1;
			this.groupBox_SendFile.TabStop = false;
			this.groupBox_SendFile.Text = "Send F&ile";
			// 
			// sendFile
			// 
			this.sendFile.Dock = System.Windows.Forms.DockStyle.Fill;
			this.sendFile.Location = new System.Drawing.Point(3, 13);
			this.sendFile.Name = "sendFile";
			this.sendFile.Size = new System.Drawing.Size(706, 27);
			this.sendFile.TabIndex = 0;
			this.sendFile.CommandChanged += new System.EventHandler(this.sendFile_CommandChanged);
			this.sendFile.SendCommandRequest += new System.EventHandler(this.sendFile_SendCommandRequest);
			// 
			// Send
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.Controls.Add(this.splitContainer_Send);
			this.Name = "Send";
			this.Size = new System.Drawing.Size(712, 93);
			this.splitContainer_Send.Panel1.ResumeLayout(false);
			this.splitContainer_Send.Panel2.ResumeLayout(false);
			this.splitContainer_Send.ResumeLayout(false);
			this.panel_SendText.ResumeLayout(false);
			this.groupBox_SendText.ResumeLayout(false);
			this.panel_SendFile.ResumeLayout(false);
			this.groupBox_SendFile.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer_Send;
		private System.Windows.Forms.GroupBox groupBox_SendText;
		private System.Windows.Forms.GroupBox groupBox_SendFile;
		private SendText sendText;
		private SendFile sendFile;
		private System.Windows.Forms.Panel panel_SendText;
		private System.Windows.Forms.Panel panel_SendFile;
	}
}
