namespace HSR.YAT.Gui.Controls
{
	partial class Monitor
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
			this.panel_Monitor = new System.Windows.Forms.Panel();
			this.listBox_Monitor = new System.Windows.Forms.ListBox();
			this.panel_Picture = new System.Windows.Forms.Panel();
			this.pictureBox_Monitor = new System.Windows.Forms.PictureBox();
			this.label_CountStatus = new System.Windows.Forms.Label();
			this.panel_Monitor.SuspendLayout();
			this.panel_Picture.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_Monitor)).BeginInit();
			this.SuspendLayout();
			// 
			// panel_Monitor
			// 
			this.panel_Monitor.Controls.Add(this.listBox_Monitor);
			this.panel_Monitor.Controls.Add(this.panel_Picture);
			this.panel_Monitor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_Monitor.Location = new System.Drawing.Point(0, 0);
			this.panel_Monitor.Name = "panel_Monitor";
			this.panel_Monitor.Size = new System.Drawing.Size(300, 200);
			this.panel_Monitor.TabIndex = 0;
			this.panel_Monitor.Tag = "";
			// 
			// listBox_Monitor
			// 
			this.listBox_Monitor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBox_Monitor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.listBox_Monitor.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.listBox_Monitor.HorizontalScrollbar = true;
			this.listBox_Monitor.IntegralHeight = false;
			this.listBox_Monitor.Location = new System.Drawing.Point(0, 30);
			this.listBox_Monitor.Name = "listBox_Monitor";
			this.listBox_Monitor.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listBox_Monitor.Size = new System.Drawing.Size(300, 170);
			this.listBox_Monitor.TabIndex = 1;
			this.listBox_Monitor.Leave += new System.EventHandler(this.listBox_Monitor_Leave);
			this.listBox_Monitor.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBox_Monitor_DrawItem);
			// 
			// panel_Picture
			// 
			this.panel_Picture.Controls.Add(this.label_CountStatus);
			this.panel_Picture.Controls.Add(this.pictureBox_Monitor);
			this.panel_Picture.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel_Picture.Location = new System.Drawing.Point(0, 0);
			this.panel_Picture.Name = "panel_Picture";
			this.panel_Picture.Padding = new System.Windows.Forms.Padding(0, 0, 0, 4);
			this.panel_Picture.Size = new System.Drawing.Size(300, 30);
			this.panel_Picture.TabIndex = 0;
			// 
			// pictureBox_Monitor
			// 
			this.pictureBox_Monitor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox_Monitor.Image = global::HSR.YAT.Gui.Controls.Properties.Resources.Image_Monitor_Bidir_48x24;
			this.pictureBox_Monitor.Location = new System.Drawing.Point(0, 0);
			this.pictureBox_Monitor.Name = "pictureBox_Monitor";
			this.pictureBox_Monitor.Size = new System.Drawing.Size(300, 26);
			this.pictureBox_Monitor.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pictureBox_Monitor.TabIndex = 1;
			this.pictureBox_Monitor.TabStop = false;
			// 
			// label_CountStatus
			// 
			this.label_CountStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label_CountStatus.Location = new System.Drawing.Point(174, 0);
			this.label_CountStatus.Name = "label_CountStatus";
			this.label_CountStatus.Size = new System.Drawing.Size(126, 26);
			this.label_CountStatus.TabIndex = 0;
			this.label_CountStatus.Text = "888 bytes / 888 lines\r\n888 bytes / 888 lines";
			this.label_CountStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// Monitor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel_Monitor);
			this.Name = "Monitor";
			this.Size = new System.Drawing.Size(300, 200);
			this.Resize += new System.EventHandler(this.Monitor_Resize);
			this.panel_Monitor.ResumeLayout(false);
			this.panel_Picture.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_Monitor)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel_Monitor;
		private System.Windows.Forms.ListBox listBox_Monitor;
		private System.Windows.Forms.Panel panel_Picture;
		private System.Windows.Forms.PictureBox pictureBox_Monitor;
		private System.Windows.Forms.Label label_CountStatus;
	}
}
