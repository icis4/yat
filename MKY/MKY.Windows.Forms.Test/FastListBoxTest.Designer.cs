namespace MKY.Windows.Forms.Test
{
	partial class FastListBoxTest
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
			this.button_Add = new System.Windows.Forms.Button();
			this.button_AddMany = new System.Windows.Forms.Button();
			this.button_Remove = new System.Windows.Forms.Button();
			this.button_RemoveMany = new System.Windows.Forms.Button();
			this.fastListBox = new MKY.Windows.Forms.FastListBox();
			this.SuspendLayout();
			// 
			// button_Add
			// 
			this.button_Add.Location = new System.Drawing.Point(587, 12);
			this.button_Add.Name = "button_Add";
			this.button_Add.Size = new System.Drawing.Size(100, 23);
			this.button_Add.TabIndex = 1;
			this.button_Add.Text = "&Add";
			this.button_Add.UseVisualStyleBackColor = true;
			this.button_Add.Click += new System.EventHandler(this.button_Add_Click);
			// 
			// button_AddMany
			// 
			this.button_AddMany.Location = new System.Drawing.Point(587, 41);
			this.button_AddMany.Name = "button_AddMany";
			this.button_AddMany.Size = new System.Drawing.Size(100, 23);
			this.button_AddMany.TabIndex = 2;
			this.button_AddMany.Text = "Add &Many";
			this.button_AddMany.UseVisualStyleBackColor = true;
			this.button_AddMany.Click += new System.EventHandler(this.button_AddMany_Click);
			// 
			// button_Remove
			// 
			this.button_Remove.Location = new System.Drawing.Point(587, 70);
			this.button_Remove.Name = "button_Remove";
			this.button_Remove.Size = new System.Drawing.Size(100, 23);
			this.button_Remove.TabIndex = 3;
			this.button_Remove.Text = "&Remove";
			this.button_Remove.UseVisualStyleBackColor = true;
			this.button_Remove.Click += new System.EventHandler(this.button_Remove_Click);
			// 
			// button_RemoveMany
			// 
			this.button_RemoveMany.Location = new System.Drawing.Point(587, 99);
			this.button_RemoveMany.Name = "button_RemoveMany";
			this.button_RemoveMany.Size = new System.Drawing.Size(100, 23);
			this.button_RemoveMany.TabIndex = 4;
			this.button_RemoveMany.Text = "R&emove Many";
			this.button_RemoveMany.UseVisualStyleBackColor = true;
			this.button_RemoveMany.Click += new System.EventHandler(this.button_RemoveMany_Click);
			// 
			// fastListBox
			// 
			this.fastListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.fastListBox.HorizontalScrollbar = true;
			this.fastListBox.IntegralHeight = false;
			this.fastListBox.Location = new System.Drawing.Point(12, 12);
			this.fastListBox.Name = "fastListBox";
			this.fastListBox.ScrollAlwaysVisible = true;
			this.fastListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.fastListBox.Size = new System.Drawing.Size(452, 507);
			this.fastListBox.TabIndex = 0;
			this.fastListBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.fastListBox_DrawItem);
			// 
			// FastListBoxTest
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(788, 531);
			this.Controls.Add(this.button_RemoveMany);
			this.Controls.Add(this.button_Remove);
			this.Controls.Add(this.button_AddMany);
			this.Controls.Add(this.button_Add);
			this.Controls.Add(this.fastListBox);
			this.Name = "FastListBoxTest";
			this.Text = "FastListBoxTest";
			this.ResumeLayout(false);

		}

		#endregion

		private FastListBox fastListBox;
		private System.Windows.Forms.Button button_Add;
		private System.Windows.Forms.Button button_AddMany;
		private System.Windows.Forms.Button button_Remove;
		private System.Windows.Forms.Button button_RemoveMany;
	}
}

