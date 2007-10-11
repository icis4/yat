namespace YAT.Gui.Controls
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
			this.components = new System.ComponentModel.Container();
			YAT.Gui.Types.Command command7 = new YAT.Gui.Types.Command();
			YAT.Gui.Types.Command command8 = new YAT.Gui.Types.Command();
			this.splitContainer_Send = new System.Windows.Forms.SplitContainer();
			this.groupBox_SendCommand = new System.Windows.Forms.GroupBox();
			this.sendCommand = new YAT.Gui.Controls.SendCommand();
			this.groupBox_SendFile = new System.Windows.Forms.GroupBox();
			this.sendFile = new YAT.Gui.Controls.SendFile();
			this.contextMenuStrip_Send = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem_SendContextMenu_SendCommand = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_SendContextMenu_SendFile = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator_SendContextMenu_1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_SendContextMenu_Panels = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_SendContextMenu_Panels_SendCommand = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_SendContextMenu_Panels_SendFile = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_SendContextMenu_Hide = new System.Windows.Forms.ToolStripMenuItem();
			this.splitContainer_Send.Panel1.SuspendLayout();
			this.splitContainer_Send.Panel2.SuspendLayout();
			this.splitContainer_Send.SuspendLayout();
			this.groupBox_SendCommand.SuspendLayout();
			this.groupBox_SendFile.SuspendLayout();
			this.contextMenuStrip_Send.SuspendLayout();
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
			this.splitContainer_Send.Panel1.Controls.Add(this.groupBox_SendCommand);
			this.splitContainer_Send.Panel1MinSize = 46;
			// 
			// splitContainer_Send.Panel2
			// 
			this.splitContainer_Send.Panel2.Controls.Add(this.groupBox_SendFile);
			this.splitContainer_Send.Panel2MinSize = 46;
			this.splitContainer_Send.Size = new System.Drawing.Size(712, 93);
			this.splitContainer_Send.SplitterDistance = 46;
			this.splitContainer_Send.SplitterWidth = 1;
			this.splitContainer_Send.TabIndex = 1;
			// 
			// groupBox_SendCommand
			// 
			this.groupBox_SendCommand.Controls.Add(this.sendCommand);
			this.groupBox_SendCommand.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox_SendCommand.Location = new System.Drawing.Point(0, 0);
			this.groupBox_SendCommand.Name = "groupBox_SendCommand";
			this.groupBox_SendCommand.Size = new System.Drawing.Size(712, 46);
			this.groupBox_SendCommand.TabIndex = 0;
			this.groupBox_SendCommand.TabStop = false;
			this.groupBox_SendCommand.Text = "Send &Command";
			// 
			// sendCommand
			// 
			command7.CommandLines = new string[] {
        "<1 lines...> []"};
			command7.DefaultRadix = YAT.Domain.Radix.String;
			command7.Description = "<1 lines...> []";
			command7.FilePath = "";
			command7.IsFilePath = false;
			command7.MultiLineCommand = new string[] {
        "<1 lines...> []"};
			command7.SingleLineCommand = "<1 lines...> []";
			this.sendCommand.Command = command7;
			this.sendCommand.Location = new System.Drawing.Point(6, 13);
			this.sendCommand.Name = "sendCommand";
			this.sendCommand.Size = new System.Drawing.Size(700, 27);
			this.sendCommand.TabIndex = 0;
			this.sendCommand.SendCommandRequest += new System.EventHandler(this.sendCommand_SendCommandRequest);
			this.sendCommand.CommandChanged += new System.EventHandler(this.sendCommand_CommandChanged);
			// 
			// groupBox_SendFile
			// 
			this.groupBox_SendFile.Controls.Add(this.sendFile);
			this.groupBox_SendFile.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox_SendFile.Location = new System.Drawing.Point(0, 0);
			this.groupBox_SendFile.Name = "groupBox_SendFile";
			this.groupBox_SendFile.Size = new System.Drawing.Size(712, 46);
			this.groupBox_SendFile.TabIndex = 1;
			this.groupBox_SendFile.TabStop = false;
			this.groupBox_SendFile.Text = "Send &File";
			// 
			// sendFile
			// 
			command8.CommandLines = new string[] {
        "<1 lines...> []"};
			command8.DefaultRadix = YAT.Domain.Radix.String;
			command8.Description = "<1 lines...> []";
			command8.FilePath = "";
			command8.IsFilePath = false;
			command8.MultiLineCommand = new string[] {
        "<1 lines...> []"};
			command8.SingleLineCommand = "<1 lines...> []";
			this.sendFile.FileCommand = command8;
			this.sendFile.Location = new System.Drawing.Point(6, 13);
			this.sendFile.Name = "sendFile";
			this.sendFile.Size = new System.Drawing.Size(700, 27);
			this.sendFile.TabIndex = 0;
			this.sendFile.SendFileCommandRequest += new System.EventHandler(this.sendFile_SendFileCommandRequest);
			this.sendFile.FileCommandChanged += new System.EventHandler(this.sendFile_FileCommandChanged);
			// 
			// contextMenuStrip_Send
			// 
			this.contextMenuStrip_Send.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_SendContextMenu_SendCommand,
            this.toolStripMenuItem_SendContextMenu_SendFile,
            this.toolStripSeparator_SendContextMenu_1,
            this.toolStripMenuItem_SendContextMenu_Panels,
            this.toolStripMenuItem_SendContextMenu_Hide});
			this.contextMenuStrip_Send.Name = "contextMenuStrip_Send";
			this.contextMenuStrip_Send.Size = new System.Drawing.Size(160, 120);
			// 
			// toolStripMenuItem_SendContextMenu_SendCommand
			// 
			this.toolStripMenuItem_SendContextMenu_SendCommand.Name = "toolStripMenuItem_SendContextMenu_SendCommand";
			this.toolStripMenuItem_SendContextMenu_SendCommand.Size = new System.Drawing.Size(159, 22);
			this.toolStripMenuItem_SendContextMenu_SendCommand.Text = "Send Command";
			this.toolStripMenuItem_SendContextMenu_SendCommand.Click += new System.EventHandler(this.toolStripMenuItem_SendContextMenu_SendCommand_Click);
			// 
			// toolStripMenuItem_SendContextMenu_SendFile
			// 
			this.toolStripMenuItem_SendContextMenu_SendFile.Name = "toolStripMenuItem_SendContextMenu_SendFile";
			this.toolStripMenuItem_SendContextMenu_SendFile.Size = new System.Drawing.Size(159, 22);
			this.toolStripMenuItem_SendContextMenu_SendFile.Text = "Send File";
			this.toolStripMenuItem_SendContextMenu_SendFile.Click += new System.EventHandler(this.toolStripMenuItem_SendContextMenu_SendFile_Click);
			// 
			// toolStripSeparator_SendContextMenu_1
			// 
			this.toolStripSeparator_SendContextMenu_1.Name = "toolStripSeparator_SendContextMenu_1";
			this.toolStripSeparator_SendContextMenu_1.Size = new System.Drawing.Size(156, 6);
			// 
			// toolStripMenuItem_SendContextMenu_Panels
			// 
			this.toolStripMenuItem_SendContextMenu_Panels.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_SendContextMenu_Panels_SendCommand,
            this.toolStripMenuItem_SendContextMenu_Panels_SendFile});
			this.toolStripMenuItem_SendContextMenu_Panels.Name = "toolStripMenuItem_SendContextMenu_Panels";
			this.toolStripMenuItem_SendContextMenu_Panels.Size = new System.Drawing.Size(159, 22);
			this.toolStripMenuItem_SendContextMenu_Panels.Text = "Panels";
			// 
			// toolStripMenuItem_SendContextMenu_Panels_SendCommand
			// 
			this.toolStripMenuItem_SendContextMenu_Panels_SendCommand.Checked = true;
			this.toolStripMenuItem_SendContextMenu_Panels_SendCommand.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem_SendContextMenu_Panels_SendCommand.Name = "toolStripMenuItem_SendContextMenu_Panels_SendCommand";
			this.toolStripMenuItem_SendContextMenu_Panels_SendCommand.Size = new System.Drawing.Size(188, 22);
			this.toolStripMenuItem_SendContextMenu_Panels_SendCommand.Text = "Send Command Panel";
			this.toolStripMenuItem_SendContextMenu_Panels_SendCommand.Click += new System.EventHandler(this.toolStripMenuItem_SendContextMenu_Panels_SendCommand_Click);
			// 
			// toolStripMenuItem_SendContextMenu_Panels_SendFile
			// 
			this.toolStripMenuItem_SendContextMenu_Panels_SendFile.Checked = true;
			this.toolStripMenuItem_SendContextMenu_Panels_SendFile.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem_SendContextMenu_Panels_SendFile.Name = "toolStripMenuItem_SendContextMenu_Panels_SendFile";
			this.toolStripMenuItem_SendContextMenu_Panels_SendFile.Size = new System.Drawing.Size(188, 22);
			this.toolStripMenuItem_SendContextMenu_Panels_SendFile.Text = "Send File Panel";
			this.toolStripMenuItem_SendContextMenu_Panels_SendFile.Click += new System.EventHandler(this.toolStripMenuItem_SendContextMenu_Panels_SendFile_Click);
			// 
			// toolStripMenuItem_SendContextMenu_Hide
			// 
			this.toolStripMenuItem_SendContextMenu_Hide.Name = "toolStripMenuItem_SendContextMenu_Hide";
			this.toolStripMenuItem_SendContextMenu_Hide.Size = new System.Drawing.Size(159, 22);
			this.toolStripMenuItem_SendContextMenu_Hide.Text = "Hide";
			this.toolStripMenuItem_SendContextMenu_Hide.Click += new System.EventHandler(this.toolStripMenuItem_SendContextMenu_Hide_Click);
			// 
			// Send
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ContextMenuStrip = this.contextMenuStrip_Send;
			this.Controls.Add(this.splitContainer_Send);
			this.Name = "Send";
			this.Size = new System.Drawing.Size(712, 93);
			this.splitContainer_Send.Panel1.ResumeLayout(false);
			this.splitContainer_Send.Panel2.ResumeLayout(false);
			this.splitContainer_Send.ResumeLayout(false);
			this.groupBox_SendCommand.ResumeLayout(false);
			this.groupBox_SendFile.ResumeLayout(false);
			this.contextMenuStrip_Send.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer_Send;
		private System.Windows.Forms.GroupBox groupBox_SendCommand;
		private System.Windows.Forms.GroupBox groupBox_SendFile;
		private SendCommand sendCommand;
		private SendFile sendFile;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip_Send;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_SendContextMenu_SendCommand;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_SendContextMenu_SendFile;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator_SendContextMenu_1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_SendContextMenu_Panels;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_SendContextMenu_Panels_SendCommand;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_SendContextMenu_Panels_SendFile;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_SendContextMenu_Hide;

	}
}
