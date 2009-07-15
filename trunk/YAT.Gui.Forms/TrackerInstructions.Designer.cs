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
			this.button_Close.Location = new System.Drawing.Point(282, 270);
			this.button_Close.Name = "button_Close";
			this.button_Close.Size = new System.Drawing.Size(75, 23);
			this.button_Close.TabIndex = 0;
			this.button_Close.Text = "Close";
			this.button_Close.Click += new System.EventHandler(this.button_Close_Click);
			// 
			// groupBox_Instructions
			// 
			this.groupBox_Instructions.AutoSize = true;
			this.groupBox_Instructions.Controls.Add(this.linkLabel_Link);
			this.groupBox_Instructions.Controls.Add(this.linkLabel_Instructions);
			this.groupBox_Instructions.Controls.Add(this.linkLabel_SourceForgeRemark);
			this.groupBox_Instructions.Controls.Add(this.linkLabel_Intro);
			this.groupBox_Instructions.Location = new System.Drawing.Point(12, 12);
			this.groupBox_Instructions.Name = "groupBox_Instructions";
			this.groupBox_Instructions.Size = new System.Drawing.Size(345, 246);
			this.groupBox_Instructions.TabIndex = 1;
			this.groupBox_Instructions.TabStop = false;
			// 
			// linkLabel_Link
			// 
			this.linkLabel_Link.AutoSize = true;
			this.linkLabel_Link.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_Link.Location = new System.Drawing.Point(6, 126);
			this.linkLabel_Link.Name = "linkLabel_Link";
			this.linkLabel_Link.Size = new System.Drawing.Size(309, 13);
			this.linkLabel_Link.TabIndex = 2;
			this.linkLabel_Link.Text = "http://sourceforge.net/tracker/?group_id=193033&&atid=943798";
			this.linkLabel_Link.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
			// 
			// linkLabel_Instructions
			// 
			this.linkLabel_Instructions.AutoSize = true;
			this.linkLabel_Instructions.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_Instructions.Location = new System.Drawing.Point(6, 148);
			this.linkLabel_Instructions.Name = "linkLabel_Instructions";
			this.linkLabel_Instructions.Size = new System.Drawing.Size(265, 78);
			this.linkLabel_Instructions.TabIndex = 3;
			this.linkLabel_Instructions.Text = "1. Click on \"Submit New\"\r\n2. Select a \"Category\"\r\n3. Select a \"Group\", i.e. the Y" +
				"AT version you are using\r\n4. Fill in \"Summary\"\r\n5. Fill in \"Detailed Description" +
				"\"\r\n6. Click on \"SUBMIT\"";
			// 
			// linkLabel_SourceForgeRemark
			// 
			this.linkLabel_SourceForgeRemark.AutoSize = true;
			this.linkLabel_SourceForgeRemark.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_SourceForgeRemark.Location = new System.Drawing.Point(6, 52);
			this.linkLabel_SourceForgeRemark.Name = "linkLabel_SourceForgeRemark";
			this.linkLabel_SourceForgeRemark.Size = new System.Drawing.Size(333, 65);
			this.linkLabel_SourceForgeRemark.TabIndex = 1;
			this.linkLabel_SourceForgeRemark.Text = resources.GetString("linkLabel_SourceForgeRemark.Text");
			// 
			// linkLabel_Intro
			// 
			this.linkLabel_Intro.AutoSize = true;
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
			this.AutoSize = true;
			this.CancelButton = this.button_Close;
			this.ClientSize = new System.Drawing.Size(369, 305);
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
			this.PerformLayout();

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