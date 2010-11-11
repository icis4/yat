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
			YAT.Model.Types.Command command1 = new YAT.Model.Types.Command();
			YAT.Model.Types.Command command2 = new YAT.Model.Types.Command();
			this.splitContainer_Send = new System.Windows.Forms.SplitContainer();
			this.groupBox_SendCommand = new System.Windows.Forms.GroupBox();
			this.sendCommand = new YAT.Gui.Controls.SendCommand();
			this.groupBox_SendFile = new System.Windows.Forms.GroupBox();
			this.sendFile = new YAT.Gui.Controls.SendFile();
			this.splitContainer_Send.Panel1.SuspendLayout();
			this.splitContainer_Send.Panel2.SuspendLayout();
			this.splitContainer_Send.SuspendLayout();
			this.groupBox_SendCommand.SuspendLayout();
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
			this.groupBox_SendCommand.Padding = new System.Windows.Forms.Padding(3, 0, 3, 6);
			this.groupBox_SendCommand.Size = new System.Drawing.Size(712, 46);
			this.groupBox_SendCommand.TabIndex = 0;
			this.groupBox_SendCommand.TabStop = false;
			this.groupBox_SendCommand.Text = "Send &Command";
			// 
			// sendCommand
			// 
			command1.CommandLines = new string[] {
		"<1 lines...> []"};
			command1.DefaultRadix = YAT.Domain.Radix.String;
			command1.Description = "<1 lines...> []";
			command1.FilePath = "";
			command1.IsFilePath = false;
			command1.MultiLineText = new string[] {
		"<1 lines...> []"};
			command1.SingleLineText = "<1 lines...> []";
			this.sendCommand.Command = command1;
			this.sendCommand.Dock = System.Windows.Forms.DockStyle.Fill;
			this.sendCommand.Location = new System.Drawing.Point(3, 13);
			this.sendCommand.Name = "sendCommand";
			this.sendCommand.Size = new System.Drawing.Size(706, 27);
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
			this.groupBox_SendFile.Padding = new System.Windows.Forms.Padding(3, 0, 3, 6);
			this.groupBox_SendFile.Size = new System.Drawing.Size(712, 46);
			this.groupBox_SendFile.TabIndex = 1;
			this.groupBox_SendFile.TabStop = false;
			this.groupBox_SendFile.Text = "Send &File";
			// 
			// sendFile
			// 
			this.sendFile.Dock = System.Windows.Forms.DockStyle.Fill;
			command2.CommandLines = new string[] {
		"<1 lines...> []"};
			command2.DefaultRadix = YAT.Domain.Radix.String;
			command2.Description = "<1 lines...> []";
			command2.FilePath = "";
			command2.IsFilePath = false;
			command2.MultiLineText = new string[] {
		"<1 lines...> []"};
			command2.SingleLineText = "<1 lines...> []";
			this.sendFile.FileCommand = command2;
			this.sendFile.Location = new System.Drawing.Point(3, 13);
			this.sendFile.Name = "sendFile";
			this.sendFile.Size = new System.Drawing.Size(706, 27);
			this.sendFile.TabIndex = 0;
			this.sendFile.SendFileCommandRequest += new System.EventHandler(this.sendFile_SendFileCommandRequest);
			this.sendFile.FileCommandChanged += new System.EventHandler(this.sendFile_FileCommandChanged);
			// 
			// Send
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer_Send);
			this.Name = "Send";
			this.Size = new System.Drawing.Size(712, 93);
			this.splitContainer_Send.Panel1.ResumeLayout(false);
			this.splitContainer_Send.Panel2.ResumeLayout(false);
			this.splitContainer_Send.ResumeLayout(false);
			this.groupBox_SendCommand.ResumeLayout(false);
			this.groupBox_SendFile.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer_Send;
		private System.Windows.Forms.GroupBox groupBox_SendCommand;
		private System.Windows.Forms.GroupBox groupBox_SendFile;
		private SendCommand sendCommand;
		private SendFile sendFile;
	}
}
