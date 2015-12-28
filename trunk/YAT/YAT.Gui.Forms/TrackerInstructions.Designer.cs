namespace YAT.Gui.Forms
{
	partial class TrackerInstructions
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrackerInstructions));
			this.button_Close = new System.Windows.Forms.Button();
			this.groupBox_Instructions = new System.Windows.Forms.GroupBox();
			this.linkLabel_RootLinkCaption = new System.Windows.Forms.LinkLabel();
			this.linkLabel_DirectLinkCaption = new System.Windows.Forms.LinkLabel();
			this.linkLabel_DirectLink = new System.Windows.Forms.LinkLabel();
			this.linkLabel_RootLink = new System.Windows.Forms.LinkLabel();
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
			this.button_Close.Location = new System.Drawing.Point(270, 347);
			this.button_Close.Name = "button_Close";
			this.button_Close.Size = new System.Drawing.Size(75, 23);
			this.button_Close.TabIndex = 0;
			this.button_Close.Text = "Close";
			this.button_Close.Click += new System.EventHandler(this.button_Close_Click);
			// 
			// groupBox_Instructions
			// 
			this.groupBox_Instructions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Instructions.Controls.Add(this.linkLabel_RootLinkCaption);
			this.groupBox_Instructions.Controls.Add(this.linkLabel_DirectLinkCaption);
			this.groupBox_Instructions.Controls.Add(this.linkLabel_DirectLink);
			this.groupBox_Instructions.Controls.Add(this.linkLabel_RootLink);
			this.groupBox_Instructions.Controls.Add(this.linkLabel_Instructions);
			this.groupBox_Instructions.Controls.Add(this.linkLabel_SourceForgeRemark);
			this.groupBox_Instructions.Controls.Add(this.linkLabel_Intro);
			this.groupBox_Instructions.Location = new System.Drawing.Point(12, 12);
			this.groupBox_Instructions.Name = "groupBox_Instructions";
			this.groupBox_Instructions.Size = new System.Drawing.Size(333, 323);
			this.groupBox_Instructions.TabIndex = 1;
			this.groupBox_Instructions.TabStop = false;
			// 
			// linkLabel_RootLinkCaption
			// 
			this.linkLabel_RootLinkCaption.AutoSize = true;
			this.linkLabel_RootLinkCaption.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_RootLinkCaption.Location = new System.Drawing.Point(6, 162);
			this.linkLabel_RootLinkCaption.Name = "linkLabel_RootLinkCaption";
			this.linkLabel_RootLinkCaption.Size = new System.Drawing.Size(52, 13);
			this.linkLabel_RootLinkCaption.TabIndex = 4;
			this.linkLabel_RootLinkCaption.Text = "Plain link:";
			// 
			// linkLabel_DirectLinkCaption
			// 
			this.linkLabel_DirectLinkCaption.AutoSize = true;
			this.linkLabel_DirectLinkCaption.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_DirectLinkCaption.Location = new System.Drawing.Point(6, 126);
			this.linkLabel_DirectLinkCaption.Name = "linkLabel_DirectLinkCaption";
			this.linkLabel_DirectLinkCaption.Size = new System.Drawing.Size(116, 13);
			this.linkLabel_DirectLinkCaption.TabIndex = 2;
			this.linkLabel_DirectLinkCaption.Text = "Filtered and sorted link:";
			// 
			// linkLabel_DirectLink
			// 
			this.linkLabel_DirectLink.AutoEllipsis = true;
			this.linkLabel_DirectLink.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_DirectLink.Location = new System.Drawing.Point(7, 139);
			this.linkLabel_DirectLink.Name = "linkLabel_DirectLink";
			this.linkLabel_DirectLink.Size = new System.Drawing.Size(320, 13);
			this.linkLabel_DirectLink.TabIndex = 3;
			this.linkLabel_DirectLink.Text = "http://sourceforge.net/tracker/?words=tracker_browse&sort=...";
			this.linkLabel_DirectLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
			// 
			// linkLabel_RootLink
			// 
			this.linkLabel_RootLink.AutoEllipsis = true;
			this.linkLabel_RootLink.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_RootLink.Location = new System.Drawing.Point(7, 175);
			this.linkLabel_RootLink.Name = "linkLabel_RootLink";
			this.linkLabel_RootLink.Size = new System.Drawing.Size(320, 13);
			this.linkLabel_RootLink.TabIndex = 5;
			this.linkLabel_RootLink.Text = "http://sourceforge.net/tracker/?group_id=193033&&atid=943798";
			this.linkLabel_RootLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
			// 
			// linkLabel_Instructions
			// 
			this.linkLabel_Instructions.AutoSize = true;
			this.linkLabel_Instructions.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_Instructions.Location = new System.Drawing.Point(6, 198);
			this.linkLabel_Instructions.Name = "linkLabel_Instructions";
			this.linkLabel_Instructions.Size = new System.Drawing.Size(25, 117);
			this.linkLabel_Instructions.TabIndex = 6;
			this.linkLabel_Instructions.Text = "1.\r\n2.\r\n3.\r\n    >\r\n    >\r\n    >\r\n4.\r\n5.\r\n6.";
			// 
			// linkLabel_SourceForgeRemark
			// 
			this.linkLabel_SourceForgeRemark.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_SourceForgeRemark.Location = new System.Drawing.Point(6, 52);
			this.linkLabel_SourceForgeRemark.Name = "linkLabel_SourceForgeRemark";
			this.linkLabel_SourceForgeRemark.Size = new System.Drawing.Size(321, 65);
			this.linkLabel_SourceForgeRemark.TabIndex = 1;
			this.linkLabel_SourceForgeRemark.Text = resources.GetString("linkLabel_SourceForgeRemark.Text");
			// 
			// linkLabel_Intro
			// 
			this.linkLabel_Intro.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_Intro.Location = new System.Drawing.Point(6, 16);
			this.linkLabel_Intro.Name = "linkLabel_Intro";
			this.linkLabel_Intro.Size = new System.Drawing.Size(321, 26);
			this.linkLabel_Intro.TabIndex = 0;
			this.linkLabel_Intro.Text = "Support for YAT can be requested online. Follow the link below and\r\nproceed accor" +
    "ding to the instructions.";
			// 
			// TrackerInstructions
			// 
			this.AcceptButton = this.button_Close;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button_Close;
			this.ClientSize = new System.Drawing.Size(357, 382);
			this.Controls.Add(this.groupBox_Instructions);
			this.Controls.Add(this.button_Close);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TrackerInstructions";
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
		private System.Windows.Forms.LinkLabel linkLabel_DirectLink;
		private System.Windows.Forms.LinkLabel linkLabel_RootLink;
		private System.Windows.Forms.LinkLabel linkLabel_DirectLinkCaption;
		private System.Windows.Forms.LinkLabel linkLabel_RootLinkCaption;
	}
}