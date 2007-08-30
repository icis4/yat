namespace MKY.YAT.Gui.Controls
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
			this.button_SendCommand = new System.Windows.Forms.Button();
			this.comboBox_Command = new System.Windows.Forms.ComboBox();
			this.button_MultiLineCommand = new System.Windows.Forms.Button();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.SuspendLayout();
			// 
			// button_SendCommand
			// 
			this.button_SendCommand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_SendCommand.Enabled = false;
			this.button_SendCommand.Location = new System.Drawing.Point(391, 3);
			this.button_SendCommand.Name = "button_SendCommand";
			this.button_SendCommand.Size = new System.Drawing.Size(146, 21);
			this.button_SendCommand.TabIndex = 2;
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
			this.comboBox_Command.Size = new System.Drawing.Size(351, 21);
			this.comboBox_Command.TabIndex = 0;
			this.toolTip.SetToolTip(this.comboBox_Command, "Enter Command, click arrow for recent commands, press <...> to enter multi line c" +
					"ommand");
			this.comboBox_Command.Leave += new System.EventHandler(this.comboBox_Command_Leave);
			this.comboBox_Command.Validating += new System.ComponentModel.CancelEventHandler(this.comboBox_Command_Validating);
			this.comboBox_Command.Enter += new System.EventHandler(this.comboBox_Command_Enter);
			this.comboBox_Command.SelectedIndexChanged += new System.EventHandler(this.comboBox_Command_SelectedIndexChanged);
			this.comboBox_Command.TextChanged += new System.EventHandler(this.comboBox_Command_TextChanged);
			this.comboBox_Command.DropDown += new System.EventHandler(this.comboBox_Command_DropDown);
			// 
			// button_MultiLineCommand
			// 
			this.button_MultiLineCommand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_MultiLineCommand.Location = new System.Drawing.Point(360, 3);
			this.button_MultiLineCommand.Name = "button_MultiLineCommand";
			this.button_MultiLineCommand.Size = new System.Drawing.Size(25, 21);
			this.button_MultiLineCommand.TabIndex = 1;
			this.button_MultiLineCommand.Text = "...";
			this.toolTip.SetToolTip(this.button_MultiLineCommand, "Multi Line Command");
			this.button_MultiLineCommand.UseVisualStyleBackColor = true;
			this.button_MultiLineCommand.Click += new System.EventHandler(this.button_MultiLineCommand_Click);
			// 
			// SendCommand
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.button_MultiLineCommand);
			this.Controls.Add(this.comboBox_Command);
			this.Controls.Add(this.button_SendCommand);
			this.Name = "SendCommand";
			this.Size = new System.Drawing.Size(540, 27);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.SendCommand_Paint);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button button_SendCommand;
		private System.Windows.Forms.ComboBox comboBox_Command;
		private System.Windows.Forms.Button button_MultiLineCommand;
		private System.Windows.Forms.ToolTip toolTip;
	}
}
