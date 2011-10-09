namespace YAT.Gui.Controls
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
			this.components = new System.ComponentModel.Container();
			this.panel_Monitor = new System.Windows.Forms.Panel();
			this.panel_Picture = new System.Windows.Forms.Panel();
			this.label_TimeStatus = new System.Windows.Forms.Label();
			this.label_CountStatus = new System.Windows.Forms.Label();
			this.pictureBox_Monitor = new System.Windows.Forms.PictureBox();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.timer_Opacity = new System.Windows.Forms.Timer(this.components);
			this.timer_UpdateTimeout = new System.Windows.Forms.Timer(this.components);
			this.fastListBox_Monitor = new MKY.Windows.Forms.FastListBox();
			this.panel_Monitor.SuspendLayout();
			this.panel_Picture.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_Monitor)).BeginInit();
			this.SuspendLayout();
			// 
			// panel_Monitor
			// 
			this.panel_Monitor.Controls.Add(this.fastListBox_Monitor);
			this.panel_Monitor.Controls.Add(this.panel_Picture);
			this.panel_Monitor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_Monitor.Location = new System.Drawing.Point(0, 0);
			this.panel_Monitor.Name = "panel_Monitor";
			this.panel_Monitor.Size = new System.Drawing.Size(300, 200);
			this.panel_Monitor.TabIndex = 0;
			this.panel_Monitor.Tag = "";
			// 
			// panel_Picture
			// 
			this.panel_Picture.Controls.Add(this.label_TimeStatus);
			this.panel_Picture.Controls.Add(this.label_CountStatus);
			this.panel_Picture.Controls.Add(this.pictureBox_Monitor);
			this.panel_Picture.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel_Picture.Location = new System.Drawing.Point(0, 0);
			this.panel_Picture.Name = "panel_Picture";
			this.panel_Picture.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
			this.panel_Picture.Size = new System.Drawing.Size(300, 33);
			this.panel_Picture.TabIndex = 0;
			// 
			// label_TimeStatus
			// 
			this.label_TimeStatus.AutoEllipsis = true;
			this.label_TimeStatus.Location = new System.Drawing.Point(0, 0);
			this.label_TimeStatus.Name = "label_TimeStatus";
			this.label_TimeStatus.Size = new System.Drawing.Size(136, 30);
			this.label_TimeStatus.TabIndex = 0;
			this.label_TimeStatus.Text = "hh:mm:ss\r\nhh:mm:ss";
			this.label_TimeStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip.SetToolTip(this.label_TimeStatus, "Connect time (hh:mm:ss)\r\nTotal connect time (hh:mm:ss)");
			// 
			// label_CountStatus
			// 
			this.label_CountStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label_CountStatus.AutoEllipsis = true;
			this.label_CountStatus.Location = new System.Drawing.Point(164, 0);
			this.label_CountStatus.Name = "label_CountStatus";
			this.label_CountStatus.Size = new System.Drawing.Size(136, 30);
			this.label_CountStatus.TabIndex = 1;
			this.label_CountStatus.Text = "888 | 888 @ 8/s | 8/s\r\n888 | 888 @ 8/s | 8/s";
			this.label_CountStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolTip.SetToolTip(this.label_CountStatus, "Number of bytes | lines @ bytes | lines per second");
			// 
			// pictureBox_Monitor
			// 
			this.pictureBox_Monitor.BackgroundImage = global::YAT.Gui.Controls.Properties.Resources.Image_Monitor_Bidir_28x28_Grey;
			this.pictureBox_Monitor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.pictureBox_Monitor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox_Monitor.Image = global::YAT.Gui.Controls.Properties.Resources.Image_Monitor_Bidir_28x28_Green;
			this.pictureBox_Monitor.Location = new System.Drawing.Point(0, 0);
			this.pictureBox_Monitor.Name = "pictureBox_Monitor";
			this.pictureBox_Monitor.Size = new System.Drawing.Size(300, 30);
			this.pictureBox_Monitor.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pictureBox_Monitor.TabIndex = 1;
			this.pictureBox_Monitor.TabStop = false;
			// 
			// timer_Opacity
			// 
			this.timer_Opacity.Tick += new System.EventHandler(this.timer_Opacity_Tick);
			// 
			// timer_UpdateTimeout
			// 
			this.timer_UpdateTimeout.Tick += new System.EventHandler(this.timer_UpdateTimeout_Tick);
			// 
			// fastListBox_Monitor
			// 
			this.fastListBox_Monitor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.fastListBox_Monitor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.fastListBox_Monitor.Font = new System.Drawing.Font("DejaVu Sans Mono", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.fastListBox_Monitor.HorizontalScrollbar = true;
			this.fastListBox_Monitor.IntegralHeight = false;
			this.fastListBox_Monitor.Location = new System.Drawing.Point(0, 33);
			this.fastListBox_Monitor.Name = "fastListBox_Monitor";
			this.fastListBox_Monitor.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.fastListBox_Monitor.Size = new System.Drawing.Size(300, 167);
			this.fastListBox_Monitor.TabIndex = 1;
			this.fastListBox_Monitor.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.fastListBox_Monitor_DrawItem);
			this.fastListBox_Monitor.Leave += new System.EventHandler(this.fastListBox_Monitor_Leave);
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
		private MKY.Windows.Forms.FastListBox fastListBox_Monitor;
		private System.Windows.Forms.Panel panel_Picture;
		private System.Windows.Forms.PictureBox pictureBox_Monitor;
		private System.Windows.Forms.Label label_CountStatus;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Timer timer_Opacity;
		private System.Windows.Forms.Label label_TimeStatus;
		private System.Windows.Forms.Timer timer_UpdateTimeout;
	}
}
