namespace YAT.Gui.Controls
{
	partial class SendCommand
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
			this.button_MultiLineCommand = new System.Windows.Forms.Button();
			this.button_SendCommand = new System.Windows.Forms.Button();
			this.comboBox_Command = new System.Windows.Forms.ComboBox();
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// button_MultiLineCommand
			// 
			this.button_MultiLineCommand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_MultiLineCommand.Location = new System.Drawing.Point(328, 3);
			this.button_MultiLineCommand.Name = "button_MultiLineCommand";
			this.button_MultiLineCommand.Size = new System.Drawing.Size(25, 21);
			this.button_MultiLineCommand.TabIndex = 3;
			this.button_MultiLineCommand.Text = "...";
			this.toolTip.SetToolTip(this.button_MultiLineCommand, "Multi Line Command");
			this.button_MultiLineCommand.UseVisualStyleBackColor = true;
			this.button_MultiLineCommand.Click += new System.EventHandler(this.button_MultiLineCommand_Click);
			// 
			// button_SendCommand
			// 
			this.button_SendCommand.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.button_SendCommand.Enabled = false;
			this.button_SendCommand.Location = new System.Drawing.Point(3, 3);
			this.button_SendCommand.Name = "button_SendCommand";
			this.button_SendCommand.Size = new System.Drawing.Size(177, 21);
			this.button_SendCommand.TabIndex = 4;
			this.button_SendCommand.Text = "Send Command (F3)";
			this.toolTip.SetToolTip(this.button_SendCommand, "Send Command");
			this.button_SendCommand.Click += new System.EventHandler(this.button_SendCommand_Click);
			// 
			// comboBox_Command
			// 
			this.comboBox_Command.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_Command.Location = new System.Drawing.Point(3, 3);
			this.comboBox_Command.Name = "comboBox_Command";
			this.comboBox_Command.Size = new System.Drawing.Size(319, 21);
			this.comboBox_Command.TabIndex = 1;
			this.toolTip.SetToolTip(this.comboBox_Command, "Enter Command, click arrow for recent commands, press <...> to enter multi line c" +
					"ommand");
			this.comboBox_Command.Validating += new System.ComponentModel.CancelEventHandler(this.comboBox_Command_Validating);
			this.comboBox_Command.SelectedIndexChanged += new System.EventHandler(this.comboBox_Command_SelectedIndexChanged);
			this.comboBox_Command.Leave += new System.EventHandler(this.comboBox_Command_Leave);
			this.comboBox_Command.Enter += new System.EventHandler(this.comboBox_Command_Enter);
			this.comboBox_Command.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.comboBox_Command_KeyPress);
			this.comboBox_Command.DropDown += new System.EventHandler(this.comboBox_Command_DropDown);
			this.comboBox_Command.TextChanged += new System.EventHandler(this.comboBox_Command_TextChanged);
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
			this.splitContainer.Panel1.Controls.Add(this.button_MultiLineCommand);
			this.splitContainer.Panel1.Controls.Add(this.comboBox_Command);
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.button_SendCommand);
			this.splitContainer.Size = new System.Drawing.Size(540, 27);
			this.splitContainer.SplitterDistance = 356;
			this.splitContainer.SplitterWidth = 1;
			this.splitContainer.TabIndex = 3;
			// 
			// SendCommand
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer);
			this.Name = "SendCommand";
			this.Size = new System.Drawing.Size(540, 27);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.SendCommand_Paint);
			this.Leave += new System.EventHandler(this.SendCommand_Leave);
			this.Enter += new System.EventHandler(this.SendCommand_Enter);
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel2.ResumeLayout(false);
			this.splitContainer.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.SplitContainer splitContainer;
		private System.Windows.Forms.Button button_MultiLineCommand;
		private System.Windows.Forms.Button button_SendCommand;
		private System.Windows.Forms.ComboBox comboBox_Command;
	}
}
