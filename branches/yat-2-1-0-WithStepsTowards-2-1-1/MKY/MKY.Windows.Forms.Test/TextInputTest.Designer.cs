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
			this.label_Standard = new System.Windows.Forms.Label();
			this.label_Extended = new System.Windows.Forms.Label();
			this.comboBoxEx = new MKY.Windows.Forms.ComboBoxEx();
			this.textBoxEx = new MKY.Windows.Forms.TextBoxEx();
			this.button_TabStopDummy = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// textBox
			// 
			this.textBox.Location = new System.Drawing.Point(197, 44);
			this.textBox.Name = "textBox";
			this.textBox.Size = new System.Drawing.Size(100, 20);
			this.textBox.TabIndex = 3;
			this.textBox.Text = "Some text";
			// 
			// comboBox
			// 
			this.comboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox.FormattingEnabled = true;
			this.comboBox.Items.AddRange(new object[] {
            "Some item 1",
            "Some item 2",
            "Some item 3"});
			this.comboBox.Location = new System.Drawing.Point(303, 43);
			this.comboBox.Name = "comboBox";
			this.comboBox.Size = new System.Drawing.Size(121, 21);
			this.comboBox.TabIndex = 5;
			this.comboBox.Text = "Some text";
			// 
			// label_Standard
			// 
			this.label_Standard.AutoSize = true;
			this.label_Standard.Location = new System.Drawing.Point(12, 47);
			this.label_Standard.Name = "label_Standard";
			this.label_Standard.Size = new System.Drawing.Size(174, 13);
			this.label_Standard.TabIndex = 1;
			this.label_Standard.Text = "Standard (System.Windows.Forms):";
			// 
			// label_Extended
			// 
			this.label_Extended.AutoSize = true;
			this.label_Extended.Location = new System.Drawing.Point(12, 73);
			this.label_Extended.Name = "label_Extended";
			this.label_Extended.Size = new System.Drawing.Size(165, 13);
			this.label_Extended.TabIndex = 2;
			this.label_Extended.Text = "Extended (MKY.Windows.Forms):";
			// 
			// comboBoxEx
			// 
			this.comboBoxEx.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxEx.FormattingEnabled = true;
			this.comboBoxEx.Items.AddRange(new object[] {
            "Some item 1",
            "Some item 2",
            "Some item 3"});
			this.comboBoxEx.Location = new System.Drawing.Point(303, 70);
			this.comboBoxEx.Name = "comboBoxEx";
			this.comboBoxEx.Size = new System.Drawing.Size(121, 21);
			this.comboBoxEx.TabIndex = 6;
			this.comboBoxEx.Text = "Some text";
			// 
			// textBoxEx
			// 
			this.textBoxEx.Location = new System.Drawing.Point(197, 70);
			this.textBoxEx.Name = "textBoxEx";
			this.textBoxEx.Size = new System.Drawing.Size(100, 20);
			this.textBoxEx.TabIndex = 4;
			this.textBoxEx.Text = "Some text";
			// 
			// button_TabStopDummy
			// 
			this.button_TabStopDummy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.button_TabStopDummy.Location = new System.Drawing.Point(197, 12);
			this.button_TabStopDummy.Name = "button_TabStopDummy";
			this.button_TabStopDummy.Size = new System.Drawing.Size(227, 23);
			this.button_TabStopDummy.TabIndex = 0;
			this.button_TabStopDummy.Text = "TabStop Dummy";
			this.button_TabStopDummy.UseVisualStyleBackColor = true;
			// 
			// TextInputTest
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(436, 103);
			this.Controls.Add(this.button_TabStopDummy);
			this.Controls.Add(this.label_Extended);
			this.Controls.Add(this.label_Standard);
			this.Controls.Add(this.comboBoxEx);
			this.Controls.Add(this.textBoxEx);
			this.Controls.Add(this.comboBox);
			this.Controls.Add(this.textBox);
			this.MinimumSize = new System.Drawing.Size(452, 142);
			this.Name = "TextInputTest";
			this.Text = "TextInputTest";
			this.Deactivate += new System.EventHandler(this.TextInputTest_Deactivate);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBox;
		private System.Windows.Forms.ComboBox comboBox;
		private TextBoxEx textBoxEx;
		private ComboBoxEx comboBoxEx;
		private System.Windows.Forms.Label label_Standard;
		private System.Windows.Forms.Label label_Extended;
		private System.Windows.Forms.Button button_TabStopDummy;
	}
}