namespace MKY.Windows.Forms.Test
{
	partial class TextInputTest
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
			this.textBox = new System.Windows.Forms.TextBox();
			this.comboBox = new System.Windows.Forms.ComboBox();
			this.textBoxEx1 = new MKY.Windows.Forms.TextBoxEx();
			this.comboBoxEx1 = new MKY.Windows.Forms.ComboBoxEx();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// textBox
			// 
			this.textBox.Location = new System.Drawing.Point(196, 12);
			this.textBox.Name = "textBox";
			this.textBox.Size = new System.Drawing.Size(100, 20);
			this.textBox.TabIndex = 1;
			this.textBox.Text = "Some text";
			// 
			// comboBox
			// 
			this.comboBox.FormattingEnabled = true;
			this.comboBox.Items.AddRange(new object[] {
            "Some item 1",
            "Some item 2",
            "Some item 3"});
			this.comboBox.Location = new System.Drawing.Point(302, 12);
			this.comboBox.Name = "comboBox";
			this.comboBox.Size = new System.Drawing.Size(121, 21);
			this.comboBox.TabIndex = 2;
			this.comboBox.Text = "Some text";
			// 
			// textBoxEx1
			// 
			this.textBoxEx1.Location = new System.Drawing.Point(196, 39);
			this.textBoxEx1.Name = "textBoxEx1";
			this.textBoxEx1.Size = new System.Drawing.Size(100, 20);
			this.textBoxEx1.TabIndex = 4;
			this.textBoxEx1.Text = "Some text";
			// 
			// comboBoxEx1
			// 
			this.comboBoxEx1.FormattingEnabled = true;
			this.comboBoxEx1.Items.AddRange(new object[] {
            "Some item 1",
            "Some item 2",
            "Some item 3"});
			this.comboBoxEx1.Location = new System.Drawing.Point(302, 39);
			this.comboBoxEx1.Name = "comboBoxEx1";
			this.comboBoxEx1.Size = new System.Drawing.Size(121, 21);
			this.comboBoxEx1.TabIndex = 5;
			this.comboBoxEx1.Text = "Some text";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(174, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Standard (System.Windows.Forms):";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 42);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(165, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Extended (MKY.Windows.Forms):";
			// 
			// TextInputTest
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(436, 71);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboBoxEx1);
			this.Controls.Add(this.textBoxEx1);
			this.Controls.Add(this.comboBox);
			this.Controls.Add(this.textBox);
			this.Name = "TextInputTest";
			this.Text = "TextInputTest";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBox;
		private System.Windows.Forms.ComboBox comboBox;
		private TextBoxEx textBoxEx1;
		private ComboBoxEx comboBoxEx1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
	}
}