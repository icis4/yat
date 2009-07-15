using System;
using System.Collections.Generic;
using System.Text;

namespace YAT.Gui.Forms
{
	partial class Help
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Help));
			this.groupBox_ParserHelp = new System.Windows.Forms.GroupBox();
			this.textBox_ParserKeyword = new System.Windows.Forms.TextBox();
			this.textBox_TextTerminalKeyword = new System.Windows.Forms.TextBox();
			this.textBox_ParserFormat = new System.Windows.Forms.TextBox();
			this.label_ParserExplanations = new System.Windows.Forms.Label();
			this.button_Close = new System.Windows.Forms.Button();
			this.label_Explanations = new System.Windows.Forms.Label();
			this.groupBox_ParserHelp.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox_ParserHelp
			// 
			this.groupBox_ParserHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_ParserHelp.Controls.Add(this.textBox_ParserKeyword);
			this.groupBox_ParserHelp.Controls.Add(this.textBox_TextTerminalKeyword);
			this.groupBox_ParserHelp.Controls.Add(this.textBox_ParserFormat);
			this.groupBox_ParserHelp.Controls.Add(this.label_ParserExplanations);
			this.groupBox_ParserHelp.Location = new System.Drawing.Point(12, 48);
			this.groupBox_ParserHelp.Name = "groupBox_ParserHelp";
			this.groupBox_ParserHelp.Size = new System.Drawing.Size(476, 470);
			this.groupBox_ParserHelp.TabIndex = 1;
			this.groupBox_ParserHelp.TabStop = false;
			this.groupBox_ParserHelp.Text = "Send Format Help";
			// 
			// textBox_ParserKeyword
			// 
			this.textBox_ParserKeyword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBox_ParserKeyword.Location = new System.Drawing.Point(9, 312);
			this.textBox_ParserKeyword.Multiline = true;
			this.textBox_ParserKeyword.Name = "textBox_ParserKeyword";
			this.textBox_ParserKeyword.ReadOnly = true;
			this.textBox_ParserKeyword.Size = new System.Drawing.Size(458, 60);
			this.textBox_ParserKeyword.TabIndex = 2;
			this.textBox_ParserKeyword.Text = "<PARSER KEYWORD HELP>";
			// 
			// textBox_TextTerminalKeyword
			// 
			this.textBox_TextTerminalKeyword.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBox_TextTerminalKeyword.Location = new System.Drawing.Point(9, 378);
			this.textBox_TextTerminalKeyword.Multiline = true;
			this.textBox_TextTerminalKeyword.Name = "textBox_TextTerminalKeyword";
			this.textBox_TextTerminalKeyword.ReadOnly = true;
			this.textBox_TextTerminalKeyword.Size = new System.Drawing.Size(458, 86);
			this.textBox_TextTerminalKeyword.TabIndex = 3;
			this.textBox_TextTerminalKeyword.Text = "<TEXT TERMINAL KEYWORD HELP>";
			// 
			// textBox_ParserFormat
			// 
			this.textBox_ParserFormat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBox_ParserFormat.Location = new System.Drawing.Point(9, 51);
			this.textBox_ParserFormat.Multiline = true;
			this.textBox_ParserFormat.Name = "textBox_ParserFormat";
			this.textBox_ParserFormat.ReadOnly = true;
			this.textBox_ParserFormat.Size = new System.Drawing.Size(458, 255);
			this.textBox_ParserFormat.TabIndex = 1;
			this.textBox_ParserFormat.Text = "<PARSER FORMAT HELP>";
			// 
			// label_ParserExplanations
			// 
			this.label_ParserExplanations.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.label_ParserExplanations.Location = new System.Drawing.Point(6, 16);
			this.label_ParserExplanations.Name = "label_ParserExplanations";
			this.label_ParserExplanations.Size = new System.Drawing.Size(464, 32);
			this.label_ParserExplanations.TabIndex = 0;
			this.label_ParserExplanations.Text = "Escape sequences and ASCII mnemonics allow sending data other than strings. This " +
				"applies to commands as well as to files.";
			// 
			// button_Close
			// 
			this.button_Close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Close.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button_Close.Location = new System.Drawing.Point(413, 532);
			this.button_Close.Name = "button_Close";
			this.button_Close.Size = new System.Drawing.Size(75, 23);
			this.button_Close.TabIndex = 2;
			this.button_Close.Text = "Close";
			this.button_Close.Click += new System.EventHandler(this.button_Close_Click);
			// 
			// label_Explanations
			// 
			this.label_Explanations.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.label_Explanations.Location = new System.Drawing.Point(12, 9);
			this.label_Explanations.Name = "label_Explanations";
			this.label_Explanations.Size = new System.Drawing.Size(476, 26);
			this.label_Explanations.TabIndex = 0;
			this.label_Explanations.Text = "Since this is non-commercial software, there simply hasn\'t been resources to add " +
				"a comprehensive help yet. Maybe one day...";
			// 
			// Help
			// 
			this.AcceptButton = this.button_Close;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button_Close;
			this.ClientSize = new System.Drawing.Size(500, 567);
			this.Controls.Add(this.label_Explanations);
			this.Controls.Add(this.button_Close);
			this.Controls.Add(this.groupBox_ParserHelp);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Help";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "YAT Help";
			this.groupBox_ParserHelp.ResumeLayout(false);
			this.groupBox_ParserHelp.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.GroupBox groupBox_ParserHelp;
		private System.Windows.Forms.Button button_Close;
		private System.Windows.Forms.Label label_Explanations;
		private System.Windows.Forms.Label label_ParserExplanations;
		private System.Windows.Forms.TextBox textBox_ParserFormat;
		private System.Windows.Forms.TextBox textBox_TextTerminalKeyword;
		private System.Windows.Forms.TextBox textBox_ParserKeyword;
	}
}
