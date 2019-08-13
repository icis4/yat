﻿namespace YAT.Gui.Forms
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
			this.linkLabel_PlainLinkCaption = new System.Windows.Forms.LinkLabel();
			this.linkLabel_FilteredAndSortedLinkCaption = new System.Windows.Forms.LinkLabel();
			this.linkLabel_FilteredAndSortedLink = new System.Windows.Forms.LinkLabel();
			this.linkLabel_PlainLink = new System.Windows.Forms.LinkLabel();
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
			this.button_Close.Location = new System.Drawing.Point(282, 320);
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
			this.groupBox_Instructions.Controls.Add(this.linkLabel_PlainLinkCaption);
			this.groupBox_Instructions.Controls.Add(this.linkLabel_FilteredAndSortedLinkCaption);
			this.groupBox_Instructions.Controls.Add(this.linkLabel_FilteredAndSortedLink);
			this.groupBox_Instructions.Controls.Add(this.linkLabel_PlainLink);
			this.groupBox_Instructions.Controls.Add(this.linkLabel_Instructions);
			this.groupBox_Instructions.Controls.Add(this.linkLabel_SourceForgeRemark);
			this.groupBox_Instructions.Controls.Add(this.linkLabel_Intro);
			this.groupBox_Instructions.Location = new System.Drawing.Point(12, 12);
			this.groupBox_Instructions.Name = "groupBox_Instructions";
			this.groupBox_Instructions.Size = new System.Drawing.Size(345, 296);
			this.groupBox_Instructions.TabIndex = 1;
			this.groupBox_Instructions.TabStop = false;
			// 
			// linkLabel_PlainLinkCaption
			// 
			this.linkLabel_PlainLinkCaption.AutoSize = true;
			this.linkLabel_PlainLinkCaption.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_PlainLinkCaption.Location = new System.Drawing.Point(6, 163);
			this.linkLabel_PlainLinkCaption.Name = "linkLabel_PlainLinkCaption";
			this.linkLabel_PlainLinkCaption.Size = new System.Drawing.Size(52, 13);
			this.linkLabel_PlainLinkCaption.TabIndex = 4;
			this.linkLabel_PlainLinkCaption.Text = "Plain link:";
			// 
			// linkLabel_FilteredAndSortedLinkCaption
			// 
			this.linkLabel_FilteredAndSortedLinkCaption.AutoSize = true;
			this.linkLabel_FilteredAndSortedLinkCaption.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_FilteredAndSortedLinkCaption.Location = new System.Drawing.Point(6, 127);
			this.linkLabel_FilteredAndSortedLinkCaption.Name = "linkLabel_FilteredAndSortedLinkCaption";
			this.linkLabel_FilteredAndSortedLinkCaption.Size = new System.Drawing.Size(116, 13);
			this.linkLabel_FilteredAndSortedLinkCaption.TabIndex = 2;
			this.linkLabel_FilteredAndSortedLinkCaption.Text = "Filtered and sorted link:";
			// 
			// linkLabel_FilteredAndSortedLink
			// 
			this.linkLabel_FilteredAndSortedLink.AutoEllipsis = true;
			this.linkLabel_FilteredAndSortedLink.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_FilteredAndSortedLink.Location = new System.Drawing.Point(6, 140);
			this.linkLabel_FilteredAndSortedLink.Name = "linkLabel_FilteredAndSortedLink";
			this.linkLabel_FilteredAndSortedLink.Size = new System.Drawing.Size(333, 13);
			this.linkLabel_FilteredAndSortedLink.TabIndex = 3;
			this.linkLabel_FilteredAndSortedLink.Text = "http://sourceforge.net/tracker/?words=tracker_browse&sort=...";
			this.linkLabel_FilteredAndSortedLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
			// 
			// linkLabel_PlainLink
			// 
			this.linkLabel_PlainLink.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_PlainLink.Location = new System.Drawing.Point(6, 176);
			this.linkLabel_PlainLink.Name = "linkLabel_PlainLink";
			this.linkLabel_PlainLink.Size = new System.Drawing.Size(333, 13);
			this.linkLabel_PlainLink.TabIndex = 5;
			this.linkLabel_PlainLink.Text = "http://sourceforge.net/tracker/?group_id=193033&&atid=943798";
			this.linkLabel_PlainLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
			// 
			// linkLabel_Instructions
			// 
			this.linkLabel_Instructions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.linkLabel_Instructions.AutoSize = true;
			this.linkLabel_Instructions.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_Instructions.Location = new System.Drawing.Point(6, 198);
			this.linkLabel_Instructions.Name = "linkLabel_Instructions";
			this.linkLabel_Instructions.Size = new System.Drawing.Size(265, 78);
			this.linkLabel_Instructions.TabIndex = 6;
			this.linkLabel_Instructions.Text = "1. Choose \"Add New\"\r\n2. Select a \"Category\"\r\n3. Select a \"Group\", i.e. the YAT ve" +
				"rsion you are using\r\n4. Fill in \"Summary\"\r\n5. Fill in \"Detailed Description\"\r\n6." +
				" Choose \"Add\"";
			// 
			// linkLabel_SourceForgeRemark
			// 
			this.linkLabel_SourceForgeRemark.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_SourceForgeRemark.Location = new System.Drawing.Point(6, 52);
			this.linkLabel_SourceForgeRemark.Name = "linkLabel_SourceForgeRemark";
			this.linkLabel_SourceForgeRemark.Size = new System.Drawing.Size(333, 65);
			this.linkLabel_SourceForgeRemark.TabIndex = 1;
			this.linkLabel_SourceForgeRemark.Text = resources.GetString("linkLabel_SourceForgeRemark.Text");
			// 
			// linkLabel_Intro
			// 
			this.linkLabel_Intro.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_Intro.Location = new System.Drawing.Point(6, 16);
			this.linkLabel_Intro.Name = "linkLabel_Intro";
			this.linkLabel_Intro.Size = new System.Drawing.Size(325, 26);
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
			this.ClientSize = new System.Drawing.Size(369, 355);
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
		private System.Windows.Forms.LinkLabel linkLabel_FilteredAndSortedLink;
		private System.Windows.Forms.LinkLabel linkLabel_PlainLink;
		private System.Windows.Forms.LinkLabel linkLabel_FilteredAndSortedLinkCaption;
		private System.Windows.Forms.LinkLabel linkLabel_PlainLinkCaption;
	}
}