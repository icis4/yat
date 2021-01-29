namespace YAT.View.Forms
{
	partial class FeedbackInstructions
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FeedbackInstructions));
			this.button_Close = new System.Windows.Forms.Button();
			this.groupBox_Instructions = new System.Windows.Forms.GroupBox();
			this.linkLabel_Link = new System.Windows.Forms.LinkLabel();
			this.linkLabel_Instructions = new System.Windows.Forms.LinkLabel();
			this.linkLabel_SourceForgeRemark = new System.Windows.Forms.LinkLabel();
			this.linkLabel_Intro = new System.Windows.Forms.LinkLabel();
			this.groupBox_Instructions.SuspendLayout();
			this.SuspendLayout();
			// 
			// button_Close
			// 
			this.button_Close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Close.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button_Close.Location = new System.Drawing.Point(329, 312);
			this.button_Close.Name = "button_Close";
			this.button_Close.Size = new System.Drawing.Size(75, 23);
			this.button_Close.TabIndex = 0;
			this.button_Close.Text = "Close";
			this.button_Close.UseVisualStyleBackColor = true;
			this.button_Close.Click += new System.EventHandler(this.button_Close_Click);
			// 
			// groupBox_Instructions
			// 
			this.groupBox_Instructions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Instructions.Controls.Add(this.linkLabel_Link);
			this.groupBox_Instructions.Controls.Add(this.linkLabel_Instructions);
			this.groupBox_Instructions.Controls.Add(this.linkLabel_SourceForgeRemark);
			this.groupBox_Instructions.Controls.Add(this.linkLabel_Intro);
			this.groupBox_Instructions.Location = new System.Drawing.Point(12, 12);
			this.groupBox_Instructions.Name = "groupBox_Instructions";
			this.groupBox_Instructions.Size = new System.Drawing.Size(392, 288);
			this.groupBox_Instructions.TabIndex = 1;
			this.groupBox_Instructions.TabStop = false;
			// 
			// linkLabel_Link
			// 
			this.linkLabel_Link.AutoEllipsis = true;
			this.linkLabel_Link.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_Link.Location = new System.Drawing.Point(7, 127);
			this.linkLabel_Link.Name = "linkLabel_Link";
			this.linkLabel_Link.Size = new System.Drawing.Size(320, 13);
			this.linkLabel_Link.TabIndex = 2;
			this.linkLabel_Link.Text = "https://sourceforge.net/p/y-a-terminal/bugs/";
			this.linkLabel_Link.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
			// 
			// linkLabel_Instructions
			// 
			this.linkLabel_Instructions.AutoSize = true;
			this.linkLabel_Instructions.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_Instructions.Location = new System.Drawing.Point(6, 150);
			this.linkLabel_Instructions.Name = "linkLabel_Instructions";
			this.linkLabel_Instructions.Size = new System.Drawing.Size(25, 130);
			this.linkLabel_Instructions.TabIndex = 3;
			this.linkLabel_Instructions.Text = "1.\r\n2.\r\n3.\r\n4.\r\n5.\r\n    >\r\n    >\r\n    >\r\n6.\r\n7.";
			// 
			// linkLabel_SourceForgeRemark
			// 
			this.linkLabel_SourceForgeRemark.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_SourceForgeRemark.Location = new System.Drawing.Point(6, 52);
			this.linkLabel_SourceForgeRemark.Name = "linkLabel_SourceForgeRemark";
			this.linkLabel_SourceForgeRemark.Size = new System.Drawing.Size(321, 65);
			this.linkLabel_SourceForgeRemark.TabIndex = 1;
			this.linkLabel_SourceForgeRemark.Text = "If you have a SourceForge.net account...\r\n...\r\n...\r\nIf you don\'t have a SourceFor" +
    "ge.net account...\r\n...";
			// 
			// linkLabel_Intro
			// 
			this.linkLabel_Intro.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_Intro.Location = new System.Drawing.Point(6, 16);
			this.linkLabel_Intro.Name = "linkLabel_Intro";
			this.linkLabel_Intro.Size = new System.Drawing.Size(321, 26);
			this.linkLabel_Intro.TabIndex = 0;
			this.linkLabel_Intro.Text = "Support for YAT...\r\n...";
			// 
			// FeedbackInstructions
			// 
			this.AcceptButton = this.button_Close;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button_Close;
			this.ClientSize = new System.Drawing.Size(416, 347);
			this.Controls.Add(this.groupBox_Instructions);
			this.Controls.Add(this.button_Close);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FeedbackInstructions";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "YAT Support Request";
			this.groupBox_Instructions.ResumeLayout(false);
			this.groupBox_Instructions.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button button_Close;
		private System.Windows.Forms.GroupBox groupBox_Instructions;
		private System.Windows.Forms.LinkLabel linkLabel_Intro;
		private System.Windows.Forms.LinkLabel linkLabel_SourceForgeRemark;
		private System.Windows.Forms.LinkLabel linkLabel_Instructions;
		private System.Windows.Forms.LinkLabel linkLabel_Link;
	}
}