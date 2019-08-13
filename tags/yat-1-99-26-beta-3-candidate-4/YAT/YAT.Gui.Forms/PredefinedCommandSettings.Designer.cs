﻿namespace YAT.Gui.Forms
{
	public partial class PredefinedCommandSettings
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
			this.button_OK = new System.Windows.Forms.Button();
			this.button_Cancel = new System.Windows.Forms.Button();
			this.button_ClearPage = new System.Windows.Forms.Button();
			this.groupBox_Page = new System.Windows.Forms.GroupBox();
			this.predefinedCommandSettingsSet_12 = new YAT.Gui.Controls.PredefinedCommandSettingsSet();
			this.predefinedCommandSettingsSet_11 = new YAT.Gui.Controls.PredefinedCommandSettingsSet();
			this.predefinedCommandSettingsSet_10 = new YAT.Gui.Controls.PredefinedCommandSettingsSet();
			this.predefinedCommandSettingsSet_9 = new YAT.Gui.Controls.PredefinedCommandSettingsSet();
			this.predefinedCommandSettingsSet_8 = new YAT.Gui.Controls.PredefinedCommandSettingsSet();
			this.predefinedCommandSettingsSet_7 = new YAT.Gui.Controls.PredefinedCommandSettingsSet();
			this.predefinedCommandSettingsSet_6 = new YAT.Gui.Controls.PredefinedCommandSettingsSet();
			this.predefinedCommandSettingsSet_5 = new YAT.Gui.Controls.PredefinedCommandSettingsSet();
			this.predefinedCommandSettingsSet_4 = new YAT.Gui.Controls.PredefinedCommandSettingsSet();
			this.predefinedCommandSettingsSet_3 = new YAT.Gui.Controls.PredefinedCommandSettingsSet();
			this.predefinedCommandSettingsSet_1 = new YAT.Gui.Controls.PredefinedCommandSettingsSet();
			this.predefinedCommandSettingsSet_2 = new YAT.Gui.Controls.PredefinedCommandSettingsSet();
			this.label_File = new System.Windows.Forms.Label();
			this.label_predefinedCommandSettingsSet_12 = new System.Windows.Forms.Label();
			this.label_predefinedCommandSettingsSet_11 = new System.Windows.Forms.Label();
			this.label_predefinedCommandSettingsSet_10 = new System.Windows.Forms.Label();
			this.label_predefinedCommandSettingsSet_9 = new System.Windows.Forms.Label();
			this.label_predefinedCommandSettingsSet_8 = new System.Windows.Forms.Label();
			this.label_predefinedCommandSettingsSet_7 = new System.Windows.Forms.Label();
			this.label_predefinedCommandSettingsSet_6 = new System.Windows.Forms.Label();
			this.label_predefinedCommandSettingsSet_5 = new System.Windows.Forms.Label();
			this.label_predefinedCommandSettingsSet_4 = new System.Windows.Forms.Label();
			this.label_predefinedCommandSettingsSet_3 = new System.Windows.Forms.Label();
			this.label_predefinedCommandSettingsSet_2 = new System.Windows.Forms.Label();
			this.label_ExampleBinary_Description = new System.Windows.Forms.Label();
			this.label_ExampleBinary_Data = new System.Windows.Forms.Label();
			this.label_predefinedCommandSettingsSet_1 = new System.Windows.Forms.Label();
			this.label_Delete = new System.Windows.Forms.Label();
			this.label_Shortcut = new System.Windows.Forms.Label();
			this.label_Data = new System.Windows.Forms.Label();
			this.label_Description = new System.Windows.Forms.Label();
			this.label_ExampleText_Description = new System.Windows.Forms.Label();
			this.label_ExampleText_Data = new System.Windows.Forms.Label();
			this.label_Example = new System.Windows.Forms.Label();
			this.button_Help = new System.Windows.Forms.Button();
			this.groupBox_Predefined = new System.Windows.Forms.GroupBox();
			this.groupBox_Pages = new System.Windows.Forms.GroupBox();
			this.button_CopyPage = new System.Windows.Forms.Button();
			this.listBox_Pages = new System.Windows.Forms.ListBox();
			this.button_DeletePages = new System.Windows.Forms.Button();
			this.button_MovePageUp = new System.Windows.Forms.Button();
			this.button_MovePageDown = new System.Windows.Forms.Button();
			this.button_DeletePage = new System.Windows.Forms.Button();
			this.button_InsertPage = new System.Windows.Forms.Button();
			this.button_AddPage = new System.Windows.Forms.Button();
			this.button_NamePage = new System.Windows.Forms.Button();
			this.groupBox_Page.SuspendLayout();
			this.groupBox_Predefined.SuspendLayout();
			this.groupBox_Pages.SuspendLayout();
			this.SuspendLayout();
			// 
			// button_OK
			// 
			this.button_OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button_OK.Location = new System.Drawing.Point(617, 442);
			this.button_OK.Name = "button_OK";
			this.button_OK.Size = new System.Drawing.Size(75, 23);
			this.button_OK.TabIndex = 1;
			this.button_OK.Text = "OK";
			this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
			// 
			// button_Cancel
			// 
			this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button_Cancel.Location = new System.Drawing.Point(698, 442);
			this.button_Cancel.Name = "button_Cancel";
			this.button_Cancel.Size = new System.Drawing.Size(75, 23);
			this.button_Cancel.TabIndex = 2;
			this.button_Cancel.Text = "Cancel";
			this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
			// 
			// button_ClearPage
			// 
			this.button_ClearPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_ClearPage.Location = new System.Drawing.Point(536, 354);
			this.button_ClearPage.Name = "button_ClearPage";
			this.button_ClearPage.Size = new System.Drawing.Size(75, 23);
			this.button_ClearPage.TabIndex = 34;
			this.button_ClearPage.Text = "Dele&te All...";
			this.button_ClearPage.Click += new System.EventHandler(this.button_ClearPage_Click);
			// 
			// groupBox_Page
			// 
			this.groupBox_Page.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Page.Controls.Add(this.predefinedCommandSettingsSet_12);
			this.groupBox_Page.Controls.Add(this.predefinedCommandSettingsSet_11);
			this.groupBox_Page.Controls.Add(this.predefinedCommandSettingsSet_10);
			this.groupBox_Page.Controls.Add(this.predefinedCommandSettingsSet_9);
			this.groupBox_Page.Controls.Add(this.predefinedCommandSettingsSet_8);
			this.groupBox_Page.Controls.Add(this.predefinedCommandSettingsSet_7);
			this.groupBox_Page.Controls.Add(this.button_ClearPage);
			this.groupBox_Page.Controls.Add(this.predefinedCommandSettingsSet_6);
			this.groupBox_Page.Controls.Add(this.predefinedCommandSettingsSet_5);
			this.groupBox_Page.Controls.Add(this.predefinedCommandSettingsSet_4);
			this.groupBox_Page.Controls.Add(this.predefinedCommandSettingsSet_3);
			this.groupBox_Page.Controls.Add(this.predefinedCommandSettingsSet_1);
			this.groupBox_Page.Controls.Add(this.predefinedCommandSettingsSet_2);
			this.groupBox_Page.Controls.Add(this.label_File);
			this.groupBox_Page.Controls.Add(this.label_predefinedCommandSettingsSet_12);
			this.groupBox_Page.Controls.Add(this.label_predefinedCommandSettingsSet_11);
			this.groupBox_Page.Controls.Add(this.label_predefinedCommandSettingsSet_10);
			this.groupBox_Page.Controls.Add(this.label_predefinedCommandSettingsSet_9);
			this.groupBox_Page.Controls.Add(this.label_predefinedCommandSettingsSet_8);
			this.groupBox_Page.Controls.Add(this.label_predefinedCommandSettingsSet_7);
			this.groupBox_Page.Controls.Add(this.label_predefinedCommandSettingsSet_6);
			this.groupBox_Page.Controls.Add(this.label_predefinedCommandSettingsSet_5);
			this.groupBox_Page.Controls.Add(this.label_predefinedCommandSettingsSet_4);
			this.groupBox_Page.Controls.Add(this.label_predefinedCommandSettingsSet_3);
			this.groupBox_Page.Controls.Add(this.label_predefinedCommandSettingsSet_2);
			this.groupBox_Page.Controls.Add(this.label_ExampleBinary_Description);
			this.groupBox_Page.Controls.Add(this.label_ExampleBinary_Data);
			this.groupBox_Page.Controls.Add(this.label_predefinedCommandSettingsSet_1);
			this.groupBox_Page.Controls.Add(this.label_Delete);
			this.groupBox_Page.Controls.Add(this.label_Shortcut);
			this.groupBox_Page.Controls.Add(this.label_Data);
			this.groupBox_Page.Controls.Add(this.label_Description);
			this.groupBox_Page.Controls.Add(this.label_ExampleText_Description);
			this.groupBox_Page.Controls.Add(this.label_ExampleText_Data);
			this.groupBox_Page.Controls.Add(this.label_Example);
			this.groupBox_Page.Location = new System.Drawing.Point(231, 16);
			this.groupBox_Page.Name = "groupBox_Page";
			this.groupBox_Page.Size = new System.Drawing.Size(620, 392);
			this.groupBox_Page.TabIndex = 1;
			this.groupBox_Page.TabStop = false;
			this.groupBox_Page.Text = "<Page>";
			// 
			// predefinedCommandSettingsSet_12
			// 
			this.predefinedCommandSettingsSet_12.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.predefinedCommandSettingsSet_12.Location = new System.Drawing.Point(30, 318);
			this.predefinedCommandSettingsSet_12.Name = "predefinedCommandSettingsSet_12";
			this.predefinedCommandSettingsSet_12.ShortcutString = "Shift+F12";
			this.predefinedCommandSettingsSet_12.Size = new System.Drawing.Size(584, 20);
			this.predefinedCommandSettingsSet_12.TabIndex = 28;
			this.predefinedCommandSettingsSet_12.Tag = "12";
			this.predefinedCommandSettingsSet_12.CommandChanged += new System.EventHandler(this.predefinedCommandSettingsSet_CommandChanged);
			// 
			// predefinedCommandSettingsSet_11
			// 
			this.predefinedCommandSettingsSet_11.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.predefinedCommandSettingsSet_11.Location = new System.Drawing.Point(30, 292);
			this.predefinedCommandSettingsSet_11.Name = "predefinedCommandSettingsSet_11";
			this.predefinedCommandSettingsSet_11.ShortcutString = "Shift+F11";
			this.predefinedCommandSettingsSet_11.Size = new System.Drawing.Size(584, 20);
			this.predefinedCommandSettingsSet_11.TabIndex = 26;
			this.predefinedCommandSettingsSet_11.Tag = "11";
			this.predefinedCommandSettingsSet_11.CommandChanged += new System.EventHandler(this.predefinedCommandSettingsSet_CommandChanged);
			// 
			// predefinedCommandSettingsSet_10
			// 
			this.predefinedCommandSettingsSet_10.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.predefinedCommandSettingsSet_10.Location = new System.Drawing.Point(30, 266);
			this.predefinedCommandSettingsSet_10.Name = "predefinedCommandSettingsSet_10";
			this.predefinedCommandSettingsSet_10.ShortcutString = "Shift+F10";
			this.predefinedCommandSettingsSet_10.Size = new System.Drawing.Size(584, 20);
			this.predefinedCommandSettingsSet_10.TabIndex = 24;
			this.predefinedCommandSettingsSet_10.Tag = "10";
			this.predefinedCommandSettingsSet_10.CommandChanged += new System.EventHandler(this.predefinedCommandSettingsSet_CommandChanged);
			// 
			// predefinedCommandSettingsSet_9
			// 
			this.predefinedCommandSettingsSet_9.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.predefinedCommandSettingsSet_9.Location = new System.Drawing.Point(30, 240);
			this.predefinedCommandSettingsSet_9.Name = "predefinedCommandSettingsSet_9";
			this.predefinedCommandSettingsSet_9.ShortcutString = "Shift+F9";
			this.predefinedCommandSettingsSet_9.Size = new System.Drawing.Size(584, 20);
			this.predefinedCommandSettingsSet_9.TabIndex = 22;
			this.predefinedCommandSettingsSet_9.Tag = "9";
			this.predefinedCommandSettingsSet_9.CommandChanged += new System.EventHandler(this.predefinedCommandSettingsSet_CommandChanged);
			// 
			// predefinedCommandSettingsSet_8
			// 
			this.predefinedCommandSettingsSet_8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.predefinedCommandSettingsSet_8.Location = new System.Drawing.Point(30, 214);
			this.predefinedCommandSettingsSet_8.Name = "predefinedCommandSettingsSet_8";
			this.predefinedCommandSettingsSet_8.ShortcutString = "Shift+F8";
			this.predefinedCommandSettingsSet_8.Size = new System.Drawing.Size(584, 20);
			this.predefinedCommandSettingsSet_8.TabIndex = 20;
			this.predefinedCommandSettingsSet_8.Tag = "8";
			this.predefinedCommandSettingsSet_8.CommandChanged += new System.EventHandler(this.predefinedCommandSettingsSet_CommandChanged);
			// 
			// predefinedCommandSettingsSet_7
			// 
			this.predefinedCommandSettingsSet_7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.predefinedCommandSettingsSet_7.Location = new System.Drawing.Point(30, 188);
			this.predefinedCommandSettingsSet_7.Name = "predefinedCommandSettingsSet_7";
			this.predefinedCommandSettingsSet_7.ShortcutString = "Shift+F7";
			this.predefinedCommandSettingsSet_7.Size = new System.Drawing.Size(584, 20);
			this.predefinedCommandSettingsSet_7.TabIndex = 18;
			this.predefinedCommandSettingsSet_7.Tag = "7";
			this.predefinedCommandSettingsSet_7.CommandChanged += new System.EventHandler(this.predefinedCommandSettingsSet_CommandChanged);
			// 
			// predefinedCommandSettingsSet_6
			// 
			this.predefinedCommandSettingsSet_6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.predefinedCommandSettingsSet_6.Location = new System.Drawing.Point(30, 162);
			this.predefinedCommandSettingsSet_6.Name = "predefinedCommandSettingsSet_6";
			this.predefinedCommandSettingsSet_6.ShortcutString = "Shift+F6";
			this.predefinedCommandSettingsSet_6.Size = new System.Drawing.Size(584, 20);
			this.predefinedCommandSettingsSet_6.TabIndex = 16;
			this.predefinedCommandSettingsSet_6.Tag = "6";
			this.predefinedCommandSettingsSet_6.CommandChanged += new System.EventHandler(this.predefinedCommandSettingsSet_CommandChanged);
			// 
			// predefinedCommandSettingsSet_5
			// 
			this.predefinedCommandSettingsSet_5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.predefinedCommandSettingsSet_5.Location = new System.Drawing.Point(30, 136);
			this.predefinedCommandSettingsSet_5.Name = "predefinedCommandSettingsSet_5";
			this.predefinedCommandSettingsSet_5.ShortcutString = "Shift+F5";
			this.predefinedCommandSettingsSet_5.Size = new System.Drawing.Size(584, 20);
			this.predefinedCommandSettingsSet_5.TabIndex = 14;
			this.predefinedCommandSettingsSet_5.Tag = "5";
			this.predefinedCommandSettingsSet_5.CommandChanged += new System.EventHandler(this.predefinedCommandSettingsSet_CommandChanged);
			// 
			// predefinedCommandSettingsSet_4
			// 
			this.predefinedCommandSettingsSet_4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.predefinedCommandSettingsSet_4.Location = new System.Drawing.Point(30, 110);
			this.predefinedCommandSettingsSet_4.Name = "predefinedCommandSettingsSet_4";
			this.predefinedCommandSettingsSet_4.ShortcutString = "Shift+F4";
			this.predefinedCommandSettingsSet_4.Size = new System.Drawing.Size(584, 20);
			this.predefinedCommandSettingsSet_4.TabIndex = 12;
			this.predefinedCommandSettingsSet_4.Tag = "4";
			this.predefinedCommandSettingsSet_4.CommandChanged += new System.EventHandler(this.predefinedCommandSettingsSet_CommandChanged);
			// 
			// predefinedCommandSettingsSet_3
			// 
			this.predefinedCommandSettingsSet_3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.predefinedCommandSettingsSet_3.Location = new System.Drawing.Point(30, 84);
			this.predefinedCommandSettingsSet_3.Name = "predefinedCommandSettingsSet_3";
			this.predefinedCommandSettingsSet_3.ShortcutString = "Shift+F3";
			this.predefinedCommandSettingsSet_3.Size = new System.Drawing.Size(584, 20);
			this.predefinedCommandSettingsSet_3.TabIndex = 10;
			this.predefinedCommandSettingsSet_3.Tag = "3";
			this.predefinedCommandSettingsSet_3.CommandChanged += new System.EventHandler(this.predefinedCommandSettingsSet_CommandChanged);
			// 
			// predefinedCommandSettingsSet_1
			// 
			this.predefinedCommandSettingsSet_1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.predefinedCommandSettingsSet_1.Location = new System.Drawing.Point(30, 32);
			this.predefinedCommandSettingsSet_1.Name = "predefinedCommandSettingsSet_1";
			this.predefinedCommandSettingsSet_1.Size = new System.Drawing.Size(584, 20);
			this.predefinedCommandSettingsSet_1.TabIndex = 6;
			this.predefinedCommandSettingsSet_1.Tag = "1";
			this.predefinedCommandSettingsSet_1.CommandChanged += new System.EventHandler(this.predefinedCommandSettingsSet_CommandChanged);
			// 
			// predefinedCommandSettingsSet_2
			// 
			this.predefinedCommandSettingsSet_2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.predefinedCommandSettingsSet_2.Location = new System.Drawing.Point(30, 58);
			this.predefinedCommandSettingsSet_2.Name = "predefinedCommandSettingsSet_2";
			this.predefinedCommandSettingsSet_2.ShortcutString = "Shift+F2";
			this.predefinedCommandSettingsSet_2.Size = new System.Drawing.Size(584, 20);
			this.predefinedCommandSettingsSet_2.TabIndex = 8;
			this.predefinedCommandSettingsSet_2.Tag = "2";
			this.predefinedCommandSettingsSet_2.CommandChanged += new System.EventHandler(this.predefinedCommandSettingsSet_CommandChanged);
			// 
			// label_File
			// 
			this.label_File.AutoSize = true;
			this.label_File.Location = new System.Drawing.Point(6, 16);
			this.label_File.Name = "label_File";
			this.label_File.Size = new System.Drawing.Size(48, 13);
			this.label_File.TabIndex = 0;
			this.label_File.Text = "Use File:";
			this.label_File.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label_predefinedCommandSettingsSet_12
			// 
			this.label_predefinedCommandSettingsSet_12.AutoSize = true;
			this.label_predefinedCommandSettingsSet_12.Location = new System.Drawing.Point(6, 321);
			this.label_predefinedCommandSettingsSet_12.Name = "label_predefinedCommandSettingsSet_12";
			this.label_predefinedCommandSettingsSet_12.Size = new System.Drawing.Size(22, 13);
			this.label_predefinedCommandSettingsSet_12.TabIndex = 27;
			this.label_predefinedCommandSettingsSet_12.Text = "12:";
			// 
			// label_predefinedCommandSettingsSet_11
			// 
			this.label_predefinedCommandSettingsSet_11.AutoSize = true;
			this.label_predefinedCommandSettingsSet_11.Location = new System.Drawing.Point(6, 295);
			this.label_predefinedCommandSettingsSet_11.Name = "label_predefinedCommandSettingsSet_11";
			this.label_predefinedCommandSettingsSet_11.Size = new System.Drawing.Size(22, 13);
			this.label_predefinedCommandSettingsSet_11.TabIndex = 25;
			this.label_predefinedCommandSettingsSet_11.Text = "11:";
			// 
			// label_predefinedCommandSettingsSet_10
			// 
			this.label_predefinedCommandSettingsSet_10.AutoSize = true;
			this.label_predefinedCommandSettingsSet_10.Location = new System.Drawing.Point(6, 269);
			this.label_predefinedCommandSettingsSet_10.Name = "label_predefinedCommandSettingsSet_10";
			this.label_predefinedCommandSettingsSet_10.Size = new System.Drawing.Size(22, 13);
			this.label_predefinedCommandSettingsSet_10.TabIndex = 23;
			this.label_predefinedCommandSettingsSet_10.Text = "1&0:";
			// 
			// label_predefinedCommandSettingsSet_9
			// 
			this.label_predefinedCommandSettingsSet_9.AutoSize = true;
			this.label_predefinedCommandSettingsSet_9.Location = new System.Drawing.Point(12, 243);
			this.label_predefinedCommandSettingsSet_9.Name = "label_predefinedCommandSettingsSet_9";
			this.label_predefinedCommandSettingsSet_9.Size = new System.Drawing.Size(16, 13);
			this.label_predefinedCommandSettingsSet_9.TabIndex = 21;
			this.label_predefinedCommandSettingsSet_9.Text = "&9:";
			// 
			// label_predefinedCommandSettingsSet_8
			// 
			this.label_predefinedCommandSettingsSet_8.AutoSize = true;
			this.label_predefinedCommandSettingsSet_8.Location = new System.Drawing.Point(12, 217);
			this.label_predefinedCommandSettingsSet_8.Name = "label_predefinedCommandSettingsSet_8";
			this.label_predefinedCommandSettingsSet_8.Size = new System.Drawing.Size(16, 13);
			this.label_predefinedCommandSettingsSet_8.TabIndex = 19;
			this.label_predefinedCommandSettingsSet_8.Text = "&8:";
			// 
			// label_predefinedCommandSettingsSet_7
			// 
			this.label_predefinedCommandSettingsSet_7.AutoSize = true;
			this.label_predefinedCommandSettingsSet_7.Location = new System.Drawing.Point(12, 191);
			this.label_predefinedCommandSettingsSet_7.Name = "label_predefinedCommandSettingsSet_7";
			this.label_predefinedCommandSettingsSet_7.Size = new System.Drawing.Size(16, 13);
			this.label_predefinedCommandSettingsSet_7.TabIndex = 17;
			this.label_predefinedCommandSettingsSet_7.Text = "&7:";
			// 
			// label_predefinedCommandSettingsSet_6
			// 
			this.label_predefinedCommandSettingsSet_6.AutoSize = true;
			this.label_predefinedCommandSettingsSet_6.Location = new System.Drawing.Point(12, 165);
			this.label_predefinedCommandSettingsSet_6.Name = "label_predefinedCommandSettingsSet_6";
			this.label_predefinedCommandSettingsSet_6.Size = new System.Drawing.Size(16, 13);
			this.label_predefinedCommandSettingsSet_6.TabIndex = 15;
			this.label_predefinedCommandSettingsSet_6.Text = "&6:";
			// 
			// label_predefinedCommandSettingsSet_5
			// 
			this.label_predefinedCommandSettingsSet_5.AutoSize = true;
			this.label_predefinedCommandSettingsSet_5.Location = new System.Drawing.Point(12, 139);
			this.label_predefinedCommandSettingsSet_5.Name = "label_predefinedCommandSettingsSet_5";
			this.label_predefinedCommandSettingsSet_5.Size = new System.Drawing.Size(16, 13);
			this.label_predefinedCommandSettingsSet_5.TabIndex = 13;
			this.label_predefinedCommandSettingsSet_5.Text = "&5:";
			// 
			// label_predefinedCommandSettingsSet_4
			// 
			this.label_predefinedCommandSettingsSet_4.AutoSize = true;
			this.label_predefinedCommandSettingsSet_4.Location = new System.Drawing.Point(12, 113);
			this.label_predefinedCommandSettingsSet_4.Name = "label_predefinedCommandSettingsSet_4";
			this.label_predefinedCommandSettingsSet_4.Size = new System.Drawing.Size(16, 13);
			this.label_predefinedCommandSettingsSet_4.TabIndex = 11;
			this.label_predefinedCommandSettingsSet_4.Text = "&4:";
			// 
			// label_predefinedCommandSettingsSet_3
			// 
			this.label_predefinedCommandSettingsSet_3.AutoSize = true;
			this.label_predefinedCommandSettingsSet_3.Location = new System.Drawing.Point(12, 87);
			this.label_predefinedCommandSettingsSet_3.Name = "label_predefinedCommandSettingsSet_3";
			this.label_predefinedCommandSettingsSet_3.Size = new System.Drawing.Size(16, 13);
			this.label_predefinedCommandSettingsSet_3.TabIndex = 9;
			this.label_predefinedCommandSettingsSet_3.Text = "&3:";
			// 
			// label_predefinedCommandSettingsSet_2
			// 
			this.label_predefinedCommandSettingsSet_2.AutoSize = true;
			this.label_predefinedCommandSettingsSet_2.Location = new System.Drawing.Point(12, 61);
			this.label_predefinedCommandSettingsSet_2.Name = "label_predefinedCommandSettingsSet_2";
			this.label_predefinedCommandSettingsSet_2.Size = new System.Drawing.Size(16, 13);
			this.label_predefinedCommandSettingsSet_2.TabIndex = 7;
			this.label_predefinedCommandSettingsSet_2.Text = "&2:";
			// 
			// label_ExampleBinary_Description
			// 
			this.label_ExampleBinary_Description.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label_ExampleBinary_Description.AutoSize = true;
			this.label_ExampleBinary_Description.Location = new System.Drawing.Point(402, 368);
			this.label_ExampleBinary_Description.Name = "label_ExampleBinary_Description";
			this.label_ExampleBinary_Description.Size = new System.Drawing.Size(72, 13);
			this.label_ExampleBinary_Description.TabIndex = 33;
			this.label_ExampleBinary_Description.Text = "Reset Device";
			// 
			// label_ExampleBinary_Data
			// 
			this.label_ExampleBinary_Data.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label_ExampleBinary_Data.AutoSize = true;
			this.label_ExampleBinary_Data.Location = new System.Drawing.Point(67, 368);
			this.label_ExampleBinary_Data.Name = "label_ExampleBinary_Data";
			this.label_ExampleBinary_Data.Size = new System.Drawing.Size(66, 13);
			this.label_ExampleBinary_Data.TabIndex = 32;
			this.label_ExampleBinary_Data.Text = "\\h(52 53 54)";
			// 
			// label_predefinedCommandSettingsSet_1
			// 
			this.label_predefinedCommandSettingsSet_1.AutoSize = true;
			this.label_predefinedCommandSettingsSet_1.Location = new System.Drawing.Point(12, 35);
			this.label_predefinedCommandSettingsSet_1.Name = "label_predefinedCommandSettingsSet_1";
			this.label_predefinedCommandSettingsSet_1.Size = new System.Drawing.Size(16, 13);
			this.label_predefinedCommandSettingsSet_1.TabIndex = 5;
			this.label_predefinedCommandSettingsSet_1.Text = "&1:";
			// 
			// label_Delete
			// 
			this.label_Delete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label_Delete.AutoSize = true;
			this.label_Delete.Location = new System.Drawing.Point(583, 16);
			this.label_Delete.Name = "label_Delete";
			this.label_Delete.Size = new System.Drawing.Size(26, 13);
			this.label_Delete.TabIndex = 4;
			this.label_Delete.Text = "Del:";
			this.label_Delete.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label_Shortcut
			// 
			this.label_Shortcut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label_Shortcut.AutoSize = true;
			this.label_Shortcut.Location = new System.Drawing.Point(527, 16);
			this.label_Shortcut.Name = "label_Shortcut";
			this.label_Shortcut.Size = new System.Drawing.Size(50, 13);
			this.label_Shortcut.TabIndex = 3;
			this.label_Shortcut.Text = "Shortcut:";
			this.label_Shortcut.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label_Data
			// 
			this.label_Data.AutoSize = true;
			this.label_Data.Location = new System.Drawing.Point(54, 16);
			this.label_Data.Name = "label_Data";
			this.label_Data.Size = new System.Drawing.Size(78, 13);
			this.label_Data.TabIndex = 1;
			this.label_Data.Text = "Command/File:";
			this.label_Data.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label_Description
			// 
			this.label_Description.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label_Description.AutoSize = true;
			this.label_Description.Location = new System.Drawing.Point(402, 16);
			this.label_Description.Name = "label_Description";
			this.label_Description.Size = new System.Drawing.Size(63, 13);
			this.label_Description.TabIndex = 2;
			this.label_Description.Text = "Description:";
			this.label_Description.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label_ExampleText_Description
			// 
			this.label_ExampleText_Description.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label_ExampleText_Description.AutoSize = true;
			this.label_ExampleText_Description.Location = new System.Drawing.Point(402, 350);
			this.label_ExampleText_Description.Name = "label_ExampleText_Description";
			this.label_ExampleText_Description.Size = new System.Drawing.Size(72, 13);
			this.label_ExampleText_Description.TabIndex = 31;
			this.label_ExampleText_Description.Text = "Reset Device";
			// 
			// label_ExampleText_Data
			// 
			this.label_ExampleText_Data.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label_ExampleText_Data.AutoSize = true;
			this.label_ExampleText_Data.Location = new System.Drawing.Point(67, 350);
			this.label_ExampleText_Data.Name = "label_ExampleText_Data";
			this.label_ExampleText_Data.Size = new System.Drawing.Size(29, 13);
			this.label_ExampleText_Data.TabIndex = 30;
			this.label_ExampleText_Data.Text = "RST";
			// 
			// label_Example
			// 
			this.label_Example.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label_Example.AutoSize = true;
			this.label_Example.Location = new System.Drawing.Point(6, 359);
			this.label_Example.Name = "label_Example";
			this.label_Example.Size = new System.Drawing.Size(55, 13);
			this.label_Example.TabIndex = 29;
			this.label_Example.Text = "Examples:";
			// 
			// button_Help
			// 
			this.button_Help.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Help.Location = new System.Drawing.Point(779, 442);
			this.button_Help.Name = "button_Help";
			this.button_Help.Size = new System.Drawing.Size(75, 23);
			this.button_Help.TabIndex = 3;
			this.button_Help.Text = "Help";
			this.button_Help.Click += new System.EventHandler(this.button_Help_Click);
			// 
			// groupBox_Predefined
			// 
			this.groupBox_Predefined.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Predefined.Controls.Add(this.groupBox_Pages);
			this.groupBox_Predefined.Controls.Add(this.groupBox_Page);
			this.groupBox_Predefined.Location = new System.Drawing.Point(12, 12);
			this.groupBox_Predefined.Name = "groupBox_Predefined";
			this.groupBox_Predefined.Size = new System.Drawing.Size(857, 415);
			this.groupBox_Predefined.TabIndex = 0;
			this.groupBox_Predefined.TabStop = false;
			// 
			// groupBox_Pages
			// 
			this.groupBox_Pages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox_Pages.Controls.Add(this.button_CopyPage);
			this.groupBox_Pages.Controls.Add(this.listBox_Pages);
			this.groupBox_Pages.Controls.Add(this.button_DeletePages);
			this.groupBox_Pages.Controls.Add(this.button_MovePageUp);
			this.groupBox_Pages.Controls.Add(this.button_MovePageDown);
			this.groupBox_Pages.Controls.Add(this.button_DeletePage);
			this.groupBox_Pages.Controls.Add(this.button_InsertPage);
			this.groupBox_Pages.Controls.Add(this.button_AddPage);
			this.groupBox_Pages.Controls.Add(this.button_NamePage);
			this.groupBox_Pages.Location = new System.Drawing.Point(6, 16);
			this.groupBox_Pages.Name = "groupBox_Pages";
			this.groupBox_Pages.Size = new System.Drawing.Size(219, 392);
			this.groupBox_Pages.TabIndex = 0;
			this.groupBox_Pages.TabStop = false;
			this.groupBox_Pages.Text = "&Pages";
			// 
			// button_CopyPage
			// 
			this.button_CopyPage.Location = new System.Drawing.Point(138, 119);
			this.button_CopyPage.Name = "button_CopyPage";
			this.button_CopyPage.Size = new System.Drawing.Size(75, 23);
			this.button_CopyPage.TabIndex = 4;
			this.button_CopyPage.Text = "&Copy...";
			this.button_CopyPage.Click += new System.EventHandler(this.button_CopyPage_Click);
			// 
			// listBox_Pages
			// 
			this.listBox_Pages.FormattingEnabled = true;
			this.listBox_Pages.HorizontalScrollbar = true;
			this.listBox_Pages.Location = new System.Drawing.Point(12, 32);
			this.listBox_Pages.Name = "listBox_Pages";
			this.listBox_Pages.Size = new System.Drawing.Size(120, 303);
			this.listBox_Pages.TabIndex = 0;
			this.listBox_Pages.SelectedIndexChanged += new System.EventHandler(this.listBox_Pages_SelectedIndexChanged);
			// 
			// button_DeletePages
			// 
			this.button_DeletePages.Location = new System.Drawing.Point(12, 354);
			this.button_DeletePages.Name = "button_DeletePages";
			this.button_DeletePages.Size = new System.Drawing.Size(120, 23);
			this.button_DeletePages.TabIndex = 8;
			this.button_DeletePages.Text = "De&lete All Pages...";
			this.button_DeletePages.Click += new System.EventHandler(this.button_DeletePages_Click);
			// 
			// button_MovePageUp
			// 
			this.button_MovePageUp.Location = new System.Drawing.Point(138, 284);
			this.button_MovePageUp.Name = "button_MovePageUp";
			this.button_MovePageUp.Size = new System.Drawing.Size(75, 23);
			this.button_MovePageUp.TabIndex = 6;
			this.button_MovePageUp.Text = "&Up";
			this.button_MovePageUp.Click += new System.EventHandler(this.button_MovePageUp_Click);
			// 
			// button_MovePageDown
			// 
			this.button_MovePageDown.Location = new System.Drawing.Point(138, 313);
			this.button_MovePageDown.Name = "button_MovePageDown";
			this.button_MovePageDown.Size = new System.Drawing.Size(75, 23);
			this.button_MovePageDown.TabIndex = 7;
			this.button_MovePageDown.Text = "&Down";
			this.button_MovePageDown.Click += new System.EventHandler(this.button_MovePageDown_Click);
			// 
			// button_DeletePage
			// 
			this.button_DeletePage.Location = new System.Drawing.Point(138, 148);
			this.button_DeletePage.Name = "button_DeletePage";
			this.button_DeletePage.Size = new System.Drawing.Size(75, 23);
			this.button_DeletePage.TabIndex = 5;
			this.button_DeletePage.Text = "D&elete...";
			this.button_DeletePage.Click += new System.EventHandler(this.button_DeletePage_Click);
			// 
			// button_InsertPage
			// 
			this.button_InsertPage.Location = new System.Drawing.Point(138, 61);
			this.button_InsertPage.Name = "button_InsertPage";
			this.button_InsertPage.Size = new System.Drawing.Size(75, 23);
			this.button_InsertPage.TabIndex = 2;
			this.button_InsertPage.Text = "&Insert...";
			this.button_InsertPage.Click += new System.EventHandler(this.button_InsertPage_Click);
			// 
			// button_AddPage
			// 
			this.button_AddPage.Location = new System.Drawing.Point(138, 90);
			this.button_AddPage.Name = "button_AddPage";
			this.button_AddPage.Size = new System.Drawing.Size(75, 23);
			this.button_AddPage.TabIndex = 3;
			this.button_AddPage.Text = "&Add...";
			this.button_AddPage.Click += new System.EventHandler(this.button_AddPage_Click);
			// 
			// button_NamePage
			// 
			this.button_NamePage.Location = new System.Drawing.Point(138, 32);
			this.button_NamePage.Name = "button_NamePage";
			this.button_NamePage.Size = new System.Drawing.Size(75, 23);
			this.button_NamePage.TabIndex = 1;
			this.button_NamePage.Text = "&Name...";
			this.button_NamePage.Click += new System.EventHandler(this.button_NamePage_Click);
			// 
			// PredefinedCommandSettings
			// 
			this.AcceptButton = this.button_OK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button_Cancel;
			this.ClientSize = new System.Drawing.Size(881, 478);
			this.Controls.Add(this.groupBox_Predefined);
			this.Controls.Add(this.button_Help);
			this.Controls.Add(this.button_Cancel);
			this.Controls.Add(this.button_OK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PredefinedCommandSettings";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Predefined Commands";
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.PredefinedCommandSettings_Paint);
			this.groupBox_Page.ResumeLayout(false);
			this.groupBox_Page.PerformLayout();
			this.groupBox_Predefined.ResumeLayout(false);
			this.groupBox_Pages.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.GroupBox groupBox_Page;
		private System.Windows.Forms.Label label_Description;
		private System.Windows.Forms.Label label_Data;
		private System.Windows.Forms.Label label_Shortcut;
		private System.Windows.Forms.Label label_File;
		private System.Windows.Forms.Label label_Example;
		private System.Windows.Forms.Label label_ExampleText_Description;
		private System.Windows.Forms.Label label_ExampleText_Data;
		private System.Windows.Forms.Label label_ExampleBinary_Description;
		private System.Windows.Forms.Label label_ExampleBinary_Data;
		private System.Windows.Forms.Label label_predefinedCommandSettingsSet_1;
		private System.Windows.Forms.Label label_predefinedCommandSettingsSet_2;
		private System.Windows.Forms.Label label_predefinedCommandSettingsSet_3;
		private System.Windows.Forms.Label label_predefinedCommandSettingsSet_4;
		private System.Windows.Forms.Label label_predefinedCommandSettingsSet_5;
		private System.Windows.Forms.Label label_predefinedCommandSettingsSet_6;
		private System.Windows.Forms.Label label_predefinedCommandSettingsSet_7;
		private System.Windows.Forms.Label label_predefinedCommandSettingsSet_8;
		private System.Windows.Forms.Button button_ClearPage;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.Button button_Cancel;
		private System.Windows.Forms.Button button_Help;
		private System.Windows.Forms.GroupBox groupBox_Predefined;
		private System.Windows.Forms.Button button_DeletePages;
		private YAT.Gui.Controls.PredefinedCommandSettingsSet predefinedCommandSettingsSet_8;
		private YAT.Gui.Controls.PredefinedCommandSettingsSet predefinedCommandSettingsSet_7;
		private YAT.Gui.Controls.PredefinedCommandSettingsSet predefinedCommandSettingsSet_6;
		private YAT.Gui.Controls.PredefinedCommandSettingsSet predefinedCommandSettingsSet_5;
		private YAT.Gui.Controls.PredefinedCommandSettingsSet predefinedCommandSettingsSet_4;
		private YAT.Gui.Controls.PredefinedCommandSettingsSet predefinedCommandSettingsSet_3;
		private YAT.Gui.Controls.PredefinedCommandSettingsSet predefinedCommandSettingsSet_1;
		private YAT.Gui.Controls.PredefinedCommandSettingsSet predefinedCommandSettingsSet_2;
		private System.Windows.Forms.GroupBox groupBox_Pages;
		private System.Windows.Forms.ListBox listBox_Pages;
		private System.Windows.Forms.Button button_NamePage;
		private System.Windows.Forms.Button button_MovePageUp;
		private System.Windows.Forms.Button button_MovePageDown;
		private System.Windows.Forms.Button button_DeletePage;
		private System.Windows.Forms.Button button_AddPage;
		private System.Windows.Forms.Button button_InsertPage;
		private YAT.Gui.Controls.PredefinedCommandSettingsSet predefinedCommandSettingsSet_12;
		private YAT.Gui.Controls.PredefinedCommandSettingsSet predefinedCommandSettingsSet_11;
		private YAT.Gui.Controls.PredefinedCommandSettingsSet predefinedCommandSettingsSet_10;
		private YAT.Gui.Controls.PredefinedCommandSettingsSet predefinedCommandSettingsSet_9;
		private System.Windows.Forms.Label label_predefinedCommandSettingsSet_9;
		private System.Windows.Forms.Label label_predefinedCommandSettingsSet_12;
		private System.Windows.Forms.Label label_predefinedCommandSettingsSet_11;
		private System.Windows.Forms.Label label_predefinedCommandSettingsSet_10;
		private System.Windows.Forms.Button button_CopyPage;
		private System.Windows.Forms.Label label_Delete;
	}
}