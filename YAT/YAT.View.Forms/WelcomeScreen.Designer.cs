namespace YAT.View.Forms
{
	partial class WelcomeScreen
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		private bool isDisposed;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			// Modified version of the designer generated Dispose() method:
			if (!this.isDisposed)
			{
				// Dispose of managed resources if requested:
				if (disposing)
				{
					if (this.finishedLoadingLock != null)
						this.finishedLoadingLock.Dispose();
				}

				// Dispose designer generated managed resources if requested:
				if (disposing && (components != null))
				{
					components.Dispose();
				}

				// Set state to disposed:
				this.finishedLoadingLock = null;
				this.isDisposed = true;
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
			this.label_Name = new System.Windows.Forms.Label();
			this.label_Version = new System.Windows.Forms.Label();
			this.label_Status = new System.Windows.Forms.Label();
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
			// label_Name
			// 
			this.label_Name.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.label_Name.AutoSize = true;
			this.label_Name.Font = new System.Drawing.Font("DejaVu Sans Mono", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_Name.Location = new System.Drawing.Point(91, 12);
			this.label_Name.Margin = new System.Windows.Forms.Padding(12, 0, 3, 0);
			this.label_Name.Name = "label_Name";
			this.label_Name.Size = new System.Drawing.Size(74, 38);
			this.label_Name.TabIndex = 1;
			this.label_Name.Text = "YAT";
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
			this.label_Version.Text = "Version 2.0.0";
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
			// WelcomeScreen
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.BackColor = System.Drawing.SystemColors.ControlDark;
			this.ClientSize = new System.Drawing.Size(280, 120);
			this.Controls.Add(this.label_Status);
			this.Controls.Add(this.label_Version);
			this.Controls.Add(this.label_Name);
			this.Controls.Add(this.pictureBox_Icon);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "WelcomeScreen";
			this.Opacity = 0.25;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "WelcomeScreen";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBox_Icon;
		private System.Windows.Forms.Label label_Name;
		private System.Windows.Forms.Label label_Version;
		private System.Windows.Forms.Label label_Status;
		private System.Windows.Forms.Timer timer_Opacity;
	}
}