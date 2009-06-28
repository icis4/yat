using System;
using System.Collections.Generic;
using System.Text;

namespace YAT.Gui.Forms
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
			this.button_Close = new System.Windows.Forms.Button();
			this.linkLabel_HHD = new System.Windows.Forms.LinkLabel();
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
			this.linkLabel_Environment = new System.Windows.Forms.LinkLabel();
			this.linkLabel_Author = new System.Windows.Forms.LinkLabel();
			this.pictureBox_License = new System.Windows.Forms.PictureBox();
			this.linkLabel_License = new System.Windows.Forms.LinkLabel();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_Product)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_License)).BeginInit();
			this.SuspendLayout();
			// 
			// button_Close
			// 
			this.button_Close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Close.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button_Close.Location = new System.Drawing.Point(527, 351);
			this.button_Close.Name = "button_Close";
			this.button_Close.Size = new System.Drawing.Size(75, 23);
			this.button_Close.TabIndex = 0;
			this.button_Close.Text = "Close";
			// 
			// linkLabel_HHD
			// 
			this.linkLabel_HHD.AutoSize = true;
			this.linkLabel_HHD.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_HHD.Location = new System.Drawing.Point(12, 160);
			this.linkLabel_HHD.Name = "linkLabel_HHD";
			this.linkLabel_HHD.Size = new System.Drawing.Size(538, 26);
			this.linkLabel_HHD.TabIndex = 8;
			this.linkLabel_HHD.Text = resources.GetString("linkLabel_HHD.Text");
			this.linkLabel_HHD.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
			// 
			// label_Separator1
			// 
			this.label_Separator1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.label_Separator1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_Separator1.Location = new System.Drawing.Point(9, 61);
			this.label_Separator1.Name = "label_Separator1";
			this.label_Separator1.Size = new System.Drawing.Size(596, 25);
			this.label_Separator1.TabIndex = 4;
			this.label_Separator1.Text = resources.GetString("label_Separator1.Text");
			this.label_Separator1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// pictureBox_Product
			// 
			this.pictureBox_Product.Image = global::YAT.Gui.Forms.Properties.Resources.Image_YAT_48x48;
			this.pictureBox_Product.Location = new System.Drawing.Point(12, 12);
			this.pictureBox_Product.Name = "pictureBox_Product";
			this.pictureBox_Product.Size = new System.Drawing.Size(48, 48);
			this.pictureBox_Product.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureBox_Product.TabIndex = 14;
			this.pictureBox_Product.TabStop = false;
			// 
			// label_Separator2
			// 
			this.label_Separator2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.label_Separator2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_Separator2.Location = new System.Drawing.Point(9, 135);
			this.label_Separator2.Name = "label_Separator2";
			this.label_Separator2.Size = new System.Drawing.Size(596, 25);
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
			this.linkLabel_Description.Size = new System.Drawing.Size(552, 26);
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
			this.linkLabel_Copyright.Size = new System.Drawing.Size(535, 26);
			this.linkLabel_Copyright.TabIndex = 2;
			this.linkLabel_Copyright.Text = "<COPYRIGHT>";
			this.linkLabel_Copyright.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
			// 
			// label_Separator3
			// 
			this.label_Separator3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.label_Separator3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_Separator3.Location = new System.Drawing.Point(9, 186);
			this.label_Separator3.Name = "label_Separator3";
			this.label_Separator3.Size = new System.Drawing.Size(596, 25);
			this.label_Separator3.TabIndex = 9;
			this.label_Separator3.Text = resources.GetString("label_Separator3.Text");
			this.label_Separator3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// linkLabel_Platform
			// 
			this.linkLabel_Platform.AutoSize = true;
			this.linkLabel_Platform.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_Platform.Location = new System.Drawing.Point(12, 122);
			this.linkLabel_Platform.Name = "linkLabel_Platform";
			this.linkLabel_Platform.Size = new System.Drawing.Size(477, 13);
			this.linkLabel_Platform.TabIndex = 6;
			this.linkLabel_Platform.Text = "For .NET <VERSION> on Windows 2000 and later. Currently running on .NET runtime <" +
				"VERSION>.";
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
			this.linkLabel_Home.Location = new System.Drawing.Point(12, 338);
			this.linkLabel_Home.Name = "linkLabel_Home";
			this.linkLabel_Home.Size = new System.Drawing.Size(322, 13);
			this.linkLabel_Home.TabIndex = 11;
			this.linkLabel_Home.Text = "Visit YAT on SourceForge.net. Tell us if you like it or why you don\'t.";
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
			// linkLabel_Environment
			// 
			this.linkLabel_Environment.AutoSize = true;
			this.linkLabel_Environment.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_Environment.Location = new System.Drawing.Point(12, 211);
			this.linkLabel_Environment.Name = "linkLabel_Environment";
			this.linkLabel_Environment.Size = new System.Drawing.Size(346, 117);
			this.linkLabel_Environment.TabIndex = 10;
			this.linkLabel_Environment.Text = resources.GetString("linkLabel_Environment.Text");
			this.linkLabel_Environment.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
			// 
			// linkLabel_Author
			// 
			this.linkLabel_Author.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.linkLabel_Author.AutoSize = true;
			this.linkLabel_Author.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_Author.Location = new System.Drawing.Point(12, 361);
			this.linkLabel_Author.Name = "linkLabel_Author";
			this.linkLabel_Author.Size = new System.Drawing.Size(65, 13);
			this.linkLabel_Author.TabIndex = 12;
			this.linkLabel_Author.Text = "<AUTHOR>";
			this.linkLabel_Author.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
			// 
			// pictureBox_License
			// 
			this.pictureBox_License.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pictureBox_License.Image = global::YAT.Gui.Forms.Properties.Resources.Image_LGPLv3_88x31;
			this.pictureBox_License.Location = new System.Drawing.Point(462, 227);
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
			this.linkLabel_License.Location = new System.Drawing.Point(414, 211);
			this.linkLabel_License.Name = "linkLabel_License";
			this.linkLabel_License.Size = new System.Drawing.Size(188, 13);
			this.linkLabel_License.TabIndex = 13;
			this.linkLabel_License.Text = "YAT is licensed under the GNU LGPL.";
			this.linkLabel_License.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
			// 
			// About
			// 
			this.AcceptButton = this.button_Close;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.button_Close;
			this.ClientSize = new System.Drawing.Size(614, 386);
			this.Controls.Add(this.linkLabel_License);
			this.Controls.Add(this.pictureBox_License);
			this.Controls.Add(this.linkLabel_Trademark);
			this.Controls.Add(this.linkLabel_Environment);
			this.Controls.Add(this.linkLabel_Author);
			this.Controls.Add(this.linkLabel_Home);
			this.Controls.Add(this.linkLabel_Title);
			this.Controls.Add(this.linkLabel_Platform);
			this.Controls.Add(this.linkLabel_Description);
			this.Controls.Add(this.label_Separator3);
			this.Controls.Add(this.label_Separator2);
			this.Controls.Add(this.label_Separator1);
			this.Controls.Add(this.linkLabel_HHD);
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
		private System.Windows.Forms.LinkLabel linkLabel_HHD;
		private System.Windows.Forms.Label label_Separator1;
		private System.Windows.Forms.Label label_Separator2;
		private System.Windows.Forms.LinkLabel linkLabel_Description;
		private System.Windows.Forms.LinkLabel linkLabel_Copyright;
		private System.Windows.Forms.Label label_Separator3;
		private System.Windows.Forms.LinkLabel linkLabel_Platform;
		private System.Windows.Forms.LinkLabel linkLabel_Title;
		private System.Windows.Forms.LinkLabel linkLabel_Home;
		private System.Windows.Forms.LinkLabel linkLabel_Trademark;
		private System.Windows.Forms.LinkLabel linkLabel_Environment;
		private System.Windows.Forms.LinkLabel linkLabel_Author;
		private System.Windows.Forms.PictureBox pictureBox_License;
		private System.Windows.Forms.LinkLabel linkLabel_License;
	}
}
