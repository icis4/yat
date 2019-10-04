namespace YAT.View.Forms
{
	partial class WelcomeScreen
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
			this.components = new System.ComponentModel.Container();
			this.pictureBox_Icon = new System.Windows.Forms.PictureBox();
			this.timer_Opacity = new System.Windows.Forms.Timer(this.components);
			this.label_Caption = new System.Windows.Forms.Label();
			this.label_Version = new System.Windows.Forms.Label();
			this.label_Status = new System.Windows.Forms.Label();
			this.backgroundWorker_LoadSettings = new System.ComponentModel.BackgroundWorker();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_Icon)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBox_Icon
			// 
			this.pictureBox_Icon.Image = global::YAT.View.Forms.Properties.Resources.Image_YAT_64x64;
			this.pictureBox_Icon.Location = new System.Drawing.Point(12, 12);
			this.pictureBox_Icon.Name = "pictureBox_Icon";
			this.pictureBox_Icon.Size = new System.Drawing.Size(64, 64);
			this.pictureBox_Icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureBox_Icon.TabIndex = 0;
			this.pictureBox_Icon.TabStop = false;
			// 
			// timer_Opacity
			// 
			this.timer_Opacity.Enabled = true;
			this.timer_Opacity.Interval = 40;
			this.timer_Opacity.Tick += new System.EventHandler(this.timer_Opacity_Tick);
			// 
			// label_Caption
			// 
			this.label_Caption.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label_Caption.AutoSize = true;
			this.label_Caption.Font = new System.Drawing.Font("DejaVu Sans Mono", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_Caption.Location = new System.Drawing.Point(91, 12);
			this.label_Caption.Margin = new System.Windows.Forms.Padding(12, 0, 3, 0);
			this.label_Caption.Name = "label_Caption";
			this.label_Caption.Size = new System.Drawing.Size(74, 38);
			this.label_Caption.TabIndex = 1;
			this.label_Caption.Text = "YAT";
			// 
			// label_Version
			// 
			this.label_Version.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label_Version.AutoSize = true;
			this.label_Version.Font = new System.Drawing.Font("DejaVu Sans Mono", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_Version.Location = new System.Drawing.Point(93, 53);
			this.label_Version.Margin = new System.Windows.Forms.Padding(12, 0, 3, 0);
			this.label_Version.Name = "label_Version";
			this.label_Version.Size = new System.Drawing.Size(179, 24);
			this.label_Version.TabIndex = 1;
			this.label_Version.Text = "Version 2.1.0";
			// 
			// label_Status
			// 
			this.label_Status.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label_Status.AutoSize = true;
			this.label_Status.Font = new System.Drawing.Font("DejaVu Sans Mono", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_Status.Location = new System.Drawing.Point(9, 91);
			this.label_Status.Margin = new System.Windows.Forms.Padding(12, 0, 3, 0);
			this.label_Status.Name = "label_Status";
			this.label_Status.Size = new System.Drawing.Size(140, 13);
			this.label_Status.TabIndex = 1;
			this.label_Status.Text = "Loading settings...";
			// 
			// backgroundWorker_LoadSettings
			// 
			this.backgroundWorker_LoadSettings.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_LoadSettings_DoWork);
			this.backgroundWorker_LoadSettings.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_LoadSettings_RunWorkerCompleted);
			// 
			// WelcomeScreen
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.BackColor = System.Drawing.SystemColors.ControlDark;
			this.ClientSize = new System.Drawing.Size(280, 120);
			this.Controls.Add(this.label_Status);
			this.Controls.Add(this.label_Version);
			this.Controls.Add(this.label_Caption);
			this.Controls.Add(this.pictureBox_Icon);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "WelcomeScreen";
			this.Opacity = 0.25D;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "WelcomeScreen";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_Icon)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBox_Icon;
		private System.Windows.Forms.Label label_Caption;
		private System.Windows.Forms.Label label_Version;
		private System.Windows.Forms.Label label_Status;
		private System.Windows.Forms.Timer timer_Opacity;
		private System.ComponentModel.BackgroundWorker backgroundWorker_LoadSettings;
	}
}