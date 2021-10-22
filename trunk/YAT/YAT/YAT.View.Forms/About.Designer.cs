namespace YAT.View.Forms
{
	partial class About
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
			// Modified version of the designer generated Dispose() method:

			// Dispose of managed resources:
			if (disposing)
			{
				timer_RunManualTest3_Dispose();
			}

			// Dispose of designer generated managed resources:
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
			this.button_Close = new System.Windows.Forms.Button();
			this.linkLabel_Monitoring = new System.Windows.Forms.LinkLabel();
			this.label_Separator1 = new System.Windows.Forms.Label();
			this.pictureBox_Product = new System.Windows.Forms.PictureBox();
			this.label_Separator2 = new System.Windows.Forms.Label();
			this.linkLabel_Description = new System.Windows.Forms.LinkLabel();
			this.linkLabel_Copyright = new System.Windows.Forms.LinkLabel();
			this.label_Separator3 = new System.Windows.Forms.Label();
			this.linkLabel_Platform = new System.Windows.Forms.LinkLabel();
			this.linkLabel_Title = new System.Windows.Forms.LinkLabel();
			this.linkLabel_Home = new System.Windows.Forms.LinkLabel();
			this.linkLabel_Trademark = new System.Windows.Forms.LinkLabel();
			this.linkLabel_BasedOn = new System.Windows.Forms.LinkLabel();
			this.linkLabel_Author = new System.Windows.Forms.LinkLabel();
			this.pictureBox_License = new System.Windows.Forms.PictureBox();
			this.linkLabel_License = new System.Windows.Forms.LinkLabel();
			this.linkLabel_VirtualPorts = new System.Windows.Forms.LinkLabel();
			this.label_RunManualTest1 = new System.Windows.Forms.Label();
			this.label_RunManualTest2 = new System.Windows.Forms.Label();
			this.label_RunManualTest3 = new System.Windows.Forms.Label();
			this.timer_RunManualTest2 = new System.Windows.Forms.Timer(this.components);
			this.linkLabel_Thanks = new System.Windows.Forms.LinkLabel();
			this.linkLabel_TerminalEmulator = new System.Windows.Forms.LinkLabel();
			this.linkLabel_DevelopedWith = new System.Windows.Forms.LinkLabel();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_Product)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_License)).BeginInit();
			this.SuspendLayout();
			// 
			// button_Close
			// 
			this.button_Close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Close.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button_Close.Location = new System.Drawing.Point(537, 579);
			this.button_Close.Name = "button_Close";
			this.button_Close.Size = new System.Drawing.Size(75, 23);
			this.button_Close.TabIndex = 0;
			this.button_Close.Text = "Close";
			this.button_Close.UseVisualStyleBackColor = true;
			// 
			// linkLabel_Monitoring
			// 
			this.linkLabel_Monitoring.AutoSize = true;
			this.linkLabel_Monitoring.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_Monitoring.Location = new System.Drawing.Point(12, 173);
			this.linkLabel_Monitoring.Name = "linkLabel_Monitoring";
			this.linkLabel_Monitoring.Size = new System.Drawing.Size(573, 52);
			this.linkLabel_Monitoring.TabIndex = 8;
			this.linkLabel_Monitoring.Text = resources.GetString("linkLabel_Monitoring.Text");
			this.linkLabel_Monitoring.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
			// 
			// label_Separator1
			// 
			this.label_Separator1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label_Separator1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_Separator1.Location = new System.Drawing.Point(9, 61);
			this.label_Separator1.Name = "label_Separator1";
			this.label_Separator1.Size = new System.Drawing.Size(606, 25);
			this.label_Separator1.TabIndex = 4;
			this.label_Separator1.Text = resources.GetString("label_Separator1.Text");
			this.label_Separator1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// pictureBox_Product
			// 
		#if (!WITH_SCRIPTING)
			this.pictureBox_Product.Image = global::YAT.View.Forms.Properties.Resources.Image_YAT_48x48;
		#else
			this.pictureBox_Product.Image = global::YAT.View.Forms.Properties.Resources.Image_Albatros_64x64;
		#endif
			this.pictureBox_Product.Location = new System.Drawing.Point(12, 12);
			this.pictureBox_Product.Name = "pictureBox_Product";
			this.pictureBox_Product.Size = new System.Drawing.Size(48, 48);
		#if (!WITH_SCRIPTING)
			this.pictureBox_Product.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
		#else
			this.pictureBox_Product.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		#endif
			this.pictureBox_Product.TabIndex = 14;
			this.pictureBox_Product.TabStop = false;
			// 
			// label_Separator2
			// 
			this.label_Separator2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label_Separator2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_Separator2.Location = new System.Drawing.Point(9, 148);
			this.label_Separator2.Name = "label_Separator2";
			this.label_Separator2.Size = new System.Drawing.Size(606, 25);
			this.label_Separator2.TabIndex = 7;
			this.label_Separator2.Text = resources.GetString("label_Separator2.Text");
			this.label_Separator2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// linkLabel_Description
			// 
			this.linkLabel_Description.AutoSize = true;
			this.linkLabel_Description.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_Description.Location = new System.Drawing.Point(12, 86);
			this.linkLabel_Description.Name = "linkLabel_Description";
			this.linkLabel_Description.Size = new System.Drawing.Size(564, 26);
			this.linkLabel_Description.TabIndex = 5;
			this.linkLabel_Description.Text = resources.GetString("linkLabel_Description.Text");
			this.linkLabel_Description.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
			// 
			// linkLabel_Copyright
			// 
			this.linkLabel_Copyright.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.linkLabel_Copyright.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_Copyright.Location = new System.Drawing.Point(67, 22);
			this.linkLabel_Copyright.Name = "linkLabel_Copyright";
			this.linkLabel_Copyright.Size = new System.Drawing.Size(545, 26);
			this.linkLabel_Copyright.TabIndex = 2;
			this.linkLabel_Copyright.Text = "<COPYRIGHT>";
			this.linkLabel_Copyright.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
			// 
			// label_Separator3
			// 
			this.label_Separator3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label_Separator3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_Separator3.Location = new System.Drawing.Point(9, 297);
			this.label_Separator3.Name = "label_Separator3";
			this.label_Separator3.Size = new System.Drawing.Size(606, 25);
			this.label_Separator3.TabIndex = 11;
			this.label_Separator3.Text = resources.GetString("label_Separator3.Text");
			this.label_Separator3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// linkLabel_Platform
			// 
			this.linkLabel_Platform.AutoSize = true;
			this.linkLabel_Platform.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_Platform.Location = new System.Drawing.Point(12, 122);
			this.linkLabel_Platform.Name = "linkLabel_Platform";
			this.linkLabel_Platform.Size = new System.Drawing.Size(358, 26);
			this.linkLabel_Platform.TabIndex = 6;
			this.linkLabel_Platform.Text = "For .NET Framework <VERSION> on <WINDOWS OS> or <OTHER OS>.\r\nCurrently running on" +
    " .NET Runtime <VERSION>.";
			this.linkLabel_Platform.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
			// 
			// linkLabel_Title
			// 
			this.linkLabel_Title.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_Title.Location = new System.Drawing.Point(67, 9);
			this.linkLabel_Title.Name = "linkLabel_Title";
			this.linkLabel_Title.Size = new System.Drawing.Size(395, 13);
			this.linkLabel_Title.TabIndex = 1;
			this.linkLabel_Title.Text = "<NAME> <VERSION>";
			this.linkLabel_Title.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
			// 
			// linkLabel_Home
			// 
			this.linkLabel_Home.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.linkLabel_Home.AutoSize = true;
			this.linkLabel_Home.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_Home.Location = new System.Drawing.Point(12, 571);
			this.linkLabel_Home.Name = "linkLabel_Home";
			this.linkLabel_Home.Size = new System.Drawing.Size(479, 13);
			this.linkLabel_Home.TabIndex = 16;
			this.linkLabel_Home.Text = "Visit YAT at SourceForge.net, or contact y-a-terminal@users.sourceforge.net. Feed" +
    "back is welcome.";
			this.linkLabel_Home.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
			// 
			// linkLabel_Trademark
			// 
			this.linkLabel_Trademark.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_Trademark.Location = new System.Drawing.Point(67, 48);
			this.linkLabel_Trademark.Name = "linkLabel_Trademark";
			this.linkLabel_Trademark.Size = new System.Drawing.Size(395, 13);
			this.linkLabel_Trademark.TabIndex = 3;
			this.linkLabel_Trademark.Text = "<TRADEMARK>";
			this.linkLabel_Trademark.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
			// 
			// linkLabel_BasedOn
			// 
			this.linkLabel_BasedOn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.linkLabel_BasedOn.AutoSize = true;
			this.linkLabel_BasedOn.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_BasedOn.Location = new System.Drawing.Point(12, 322);
			this.linkLabel_BasedOn.Name = "linkLabel_BasedOn";
			this.linkLabel_BasedOn.Size = new System.Drawing.Size(377, 117);
			this.linkLabel_BasedOn.TabIndex = 12;
			this.linkLabel_BasedOn.Text = resources.GetString("linkLabel_BasedOn.Text");
			this.linkLabel_BasedOn.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
			// 
			// linkLabel_Author
			// 
			this.linkLabel_Author.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.linkLabel_Author.AutoSize = true;
			this.linkLabel_Author.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_Author.Location = new System.Drawing.Point(12, 589);
			this.linkLabel_Author.Name = "linkLabel_Author";
			this.linkLabel_Author.Size = new System.Drawing.Size(65, 13);
			this.linkLabel_Author.TabIndex = 17;
			this.linkLabel_Author.Text = "<AUTHOR>";
			this.linkLabel_Author.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
			// 
			// pictureBox_License
			// 
			this.pictureBox_License.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pictureBox_License.Image = global::YAT.View.Forms.Properties.Resources.Image_LGPLv3_88x31;
			this.pictureBox_License.Location = new System.Drawing.Point(448, 338);
			this.pictureBox_License.Name = "pictureBox_License";
			this.pictureBox_License.Size = new System.Drawing.Size(88, 31);
			this.pictureBox_License.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureBox_License.TabIndex = 15;
			this.pictureBox_License.TabStop = false;
			// 
			// linkLabel_License
			// 
			this.linkLabel_License.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.linkLabel_License.AutoSize = true;
			this.linkLabel_License.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_License.Location = new System.Drawing.Point(400, 322);
			this.linkLabel_License.Name = "linkLabel_License";
			this.linkLabel_License.Size = new System.Drawing.Size(188, 13);
			this.linkLabel_License.TabIndex = 13;
			this.linkLabel_License.Text = "YAT is licensed under the GNU LGPL.";
			this.linkLabel_License.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
			// 
			// linkLabel_VirtualPorts
			// 
			this.linkLabel_VirtualPorts.AutoSize = true;
			this.linkLabel_VirtualPorts.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_VirtualPorts.Location = new System.Drawing.Point(12, 235);
			this.linkLabel_VirtualPorts.Name = "linkLabel_VirtualPorts";
			this.linkLabel_VirtualPorts.Size = new System.Drawing.Size(579, 26);
			this.linkLabel_VirtualPorts.TabIndex = 9;
			this.linkLabel_VirtualPorts.Text = resources.GetString("linkLabel_VirtualPorts.Text");
			this.linkLabel_VirtualPorts.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
			// 
			// label_RunManualTest1
			// 
			this.label_RunManualTest1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label_RunManualTest1.Location = new System.Drawing.Point(611, 56);
			this.label_RunManualTest1.Name = "label_RunManualTest1";
			this.label_RunManualTest1.Size = new System.Drawing.Size(16, 16);
			this.label_RunManualTest1.TabIndex = 18;
			this.label_RunManualTest1.Click += new System.EventHandler(this.label_RunManualTest1_Click);
			// 
			// label_RunManualTest2
			// 
			this.label_RunManualTest2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label_RunManualTest2.Location = new System.Drawing.Point(611, 142);
			this.label_RunManualTest2.Name = "label_RunManualTest2";
			this.label_RunManualTest2.Size = new System.Drawing.Size(16, 16);
			this.label_RunManualTest2.TabIndex = 19;
			this.label_RunManualTest2.Click += new System.EventHandler(this.label_RunManualTest2_Click);
			// 
			// label_RunManualTest3
			// 
			this.label_RunManualTest3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label_RunManualTest3.Location = new System.Drawing.Point(611, 291);
			this.label_RunManualTest3.Name = "label_RunManualTest3";
			this.label_RunManualTest3.Size = new System.Drawing.Size(16, 16);
			this.label_RunManualTest3.TabIndex = 20;
			this.label_RunManualTest3.Click += new System.EventHandler(this.label_RunManualTest3_Click);
			// 
			// timer_RunManualTest2
			// 
			this.timer_RunManualTest2.Tick += new System.EventHandler(this.timer_RunManualTest2_Tick);
			// 
			// linkLabel_Thanks
			// 
			this.linkLabel_Thanks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.linkLabel_Thanks.AutoSize = true;
			this.linkLabel_Thanks.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.linkLabel_Thanks.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_Thanks.Location = new System.Drawing.Point(12, 553);
			this.linkLabel_Thanks.Name = "linkLabel_Thanks";
			this.linkLabel_Thanks.Size = new System.Drawing.Size(454, 13);
			this.linkLabel_Thanks.TabIndex = 15;
			this.linkLabel_Thanks.Text = "And a big \"Thanks!\" to everybody who helped YAT to become what it is today!";
			// 
			// linkLabel_TerminalEmulator
			// 
			this.linkLabel_TerminalEmulator.AutoSize = true;
			this.linkLabel_TerminalEmulator.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_TerminalEmulator.Location = new System.Drawing.Point(12, 271);
			this.linkLabel_TerminalEmulator.Name = "linkLabel_TerminalEmulator";
			this.linkLabel_TerminalEmulator.Size = new System.Drawing.Size(525, 26);
			this.linkLabel_TerminalEmulator.TabIndex = 10;
			this.linkLabel_TerminalEmulator.Text = "YAT is optimized for simple command sets of e.g. embedded systems. If you are loo" +
    "king for a terminal emulator,\r\nrather go for PuTTy , Tera Term, SecureCRT or the" +
    " like.";
			this.linkLabel_TerminalEmulator.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
			// 
			// linkLabel_DevelopedWith
			// 
			this.linkLabel_DevelopedWith.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.linkLabel_DevelopedWith.AutoSize = true;
			this.linkLabel_DevelopedWith.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_DevelopedWith.Location = new System.Drawing.Point(12, 444);
			this.linkLabel_DevelopedWith.Name = "linkLabel_DevelopedWith";
			this.linkLabel_DevelopedWith.Size = new System.Drawing.Size(322, 104);
			this.linkLabel_DevelopedWith.TabIndex = 14;
			this.linkLabel_DevelopedWith.Text = resources.GetString("linkLabel_DevelopedWith.Text");
			// 
			// About
			// 
			this.AcceptButton = this.button_Close;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button_Close;
			this.ClientSize = new System.Drawing.Size(624, 614);
			this.Controls.Add(this.linkLabel_DevelopedWith);
			this.Controls.Add(this.linkLabel_TerminalEmulator);
			this.Controls.Add(this.linkLabel_Thanks);
			this.Controls.Add(this.label_RunManualTest3);
			this.Controls.Add(this.label_RunManualTest2);
			this.Controls.Add(this.label_RunManualTest1);
			this.Controls.Add(this.linkLabel_License);
			this.Controls.Add(this.pictureBox_License);
			this.Controls.Add(this.linkLabel_Trademark);
			this.Controls.Add(this.linkLabel_BasedOn);
			this.Controls.Add(this.linkLabel_Author);
			this.Controls.Add(this.linkLabel_Home);
			this.Controls.Add(this.linkLabel_Title);
			this.Controls.Add(this.linkLabel_Platform);
			this.Controls.Add(this.linkLabel_Description);
			this.Controls.Add(this.label_Separator3);
			this.Controls.Add(this.label_Separator2);
			this.Controls.Add(this.label_Separator1);
			this.Controls.Add(this.linkLabel_VirtualPorts);
			this.Controls.Add(this.linkLabel_Monitoring);
			this.Controls.Add(this.linkLabel_Copyright);
			this.Controls.Add(this.pictureBox_Product);
			this.Controls.Add(this.button_Close);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "About";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "About YAT";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_Product)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_License)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button button_Close;
		private System.Windows.Forms.PictureBox pictureBox_Product;
		private System.Windows.Forms.LinkLabel linkLabel_Monitoring;
		private System.Windows.Forms.Label label_Separator1;
		private System.Windows.Forms.Label label_Separator2;
		private System.Windows.Forms.LinkLabel linkLabel_Description;
		private System.Windows.Forms.LinkLabel linkLabel_Copyright;
		private System.Windows.Forms.Label label_Separator3;
		private System.Windows.Forms.LinkLabel linkLabel_Platform;
		private System.Windows.Forms.LinkLabel linkLabel_Title;
		private System.Windows.Forms.LinkLabel linkLabel_Home;
		private System.Windows.Forms.LinkLabel linkLabel_Trademark;
		private System.Windows.Forms.LinkLabel linkLabel_BasedOn;
		private System.Windows.Forms.LinkLabel linkLabel_Author;
		private System.Windows.Forms.PictureBox pictureBox_License;
		private System.Windows.Forms.LinkLabel linkLabel_License;
		private System.Windows.Forms.LinkLabel linkLabel_VirtualPorts;
		private System.Windows.Forms.Label label_RunManualTest1;
		private System.Windows.Forms.Label label_RunManualTest2;
		private System.Windows.Forms.Label label_RunManualTest3;
		private System.Windows.Forms.Timer timer_RunManualTest2;
		private System.Windows.Forms.LinkLabel linkLabel_Thanks;
		private System.Windows.Forms.LinkLabel linkLabel_TerminalEmulator;
		private System.Windows.Forms.LinkLabel linkLabel_DevelopedWith;
	}
}
