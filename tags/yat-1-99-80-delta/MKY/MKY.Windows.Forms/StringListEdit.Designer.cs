namespace MKY.Windows.Forms
{
	partial class StringListEdit
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
			this.button_MoveDown = new System.Windows.Forms.Button();
			this.button_MoveUp = new System.Windows.Forms.Button();
			this.button_Delete = new System.Windows.Forms.Button();
			this.button_Add = new System.Windows.Forms.Button();
			this.listBox_StringList = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// button_MoveDown
			// 
			this.button_MoveDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_MoveDown.Location = new System.Drawing.Point(69, 89);
			this.button_MoveDown.Name = "button_MoveDown";
			this.button_MoveDown.Size = new System.Drawing.Size(60, 23);
			this.button_MoveDown.TabIndex = 4;
			this.button_MoveDown.Text = "Down";
			this.button_MoveDown.UseVisualStyleBackColor = true;
			this.button_MoveDown.Click += new System.EventHandler(this.button_MoveDown_Click);
			// 
			// button_MoveUp
			// 
			this.button_MoveUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_MoveUp.Location = new System.Drawing.Point(69, 60);
			this.button_MoveUp.Name = "button_MoveUp";
			this.button_MoveUp.Size = new System.Drawing.Size(60, 23);
			this.button_MoveUp.TabIndex = 3;
			this.button_MoveUp.Text = "Up";
			this.button_MoveUp.UseVisualStyleBackColor = true;
			this.button_MoveUp.Click += new System.EventHandler(this.button_MoveUp_Click);
			// 
			// button_Delete
			// 
			this.button_Delete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Delete.Location = new System.Drawing.Point(69, 31);
			this.button_Delete.Name = "button_Delete";
			this.button_Delete.Size = new System.Drawing.Size(60, 23);
			this.button_Delete.TabIndex = 2;
			this.button_Delete.Text = "Delete...";
			this.button_Delete.UseVisualStyleBackColor = true;
			this.button_Delete.Click += new System.EventHandler(this.button_Delete_Click);
			// 
			// button_Add
			// 
			this.button_Add.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Add.Location = new System.Drawing.Point(69, 2);
			this.button_Add.Name = "button_Add";
			this.button_Add.Size = new System.Drawing.Size(60, 23);
			this.button_Add.TabIndex = 1;
			this.button_Add.Text = "Add...";
			this.button_Add.UseVisualStyleBackColor = true;
			this.button_Add.Click += new System.EventHandler(this.button_Add_Click);
			// 
			// listBox_StringList
			// 
			this.listBox_StringList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listBox_StringList.FormattingEnabled = true;
			this.listBox_StringList.Location = new System.Drawing.Point(3, 3);
			this.listBox_StringList.Name = "listBox_StringList";
			this.listBox_StringList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listBox_StringList.Size = new System.Drawing.Size(60, 108);
			this.listBox_StringList.TabIndex = 0;
			this.listBox_StringList.SelectedIndexChanged += new System.EventHandler(this.listBox_StringList_SelectedIndexChanged);
			this.listBox_StringList.DoubleClick += new System.EventHandler(this.listBox_StringList_DoubleClick);
			// 
			// StringListEdit
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.Controls.Add(this.button_MoveDown);
			this.Controls.Add(this.button_MoveUp);
			this.Controls.Add(this.button_Delete);
			this.Controls.Add(this.button_Add);
			this.Controls.Add(this.listBox_StringList);
			this.MinimumSize = new System.Drawing.Size(132, 116);
			this.Name = "StringListEdit";
			this.Size = new System.Drawing.Size(132, 116);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button button_MoveDown;
		private System.Windows.Forms.Button button_MoveUp;
		private System.Windows.Forms.Button button_Delete;
		private System.Windows.Forms.Button button_Add;
		private System.Windows.Forms.ListBox listBox_StringList;
	}
}
