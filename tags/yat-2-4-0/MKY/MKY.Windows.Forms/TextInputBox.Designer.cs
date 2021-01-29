namespace MKY.Windows.Forms
{
	partial class TextInputBox
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
			this.label_Text = new System.Windows.Forms.Label();
			this.button_OK = new System.Windows.Forms.Button();
			this.button_Cancel = new System.Windows.Forms.Button();
			this.textBox_InputText = new MKY.Windows.Forms.TextBoxEx();
			this.SuspendLayout();
			// 
			// label_Text
			// 
			this.label_Text.AutoSize = true;
			this.label_Text.Location = new System.Drawing.Point(22, 18);
			this.label_Text.Name = "label_Text";
			this.label_Text.Size = new System.Drawing.Size(40, 13);
			this.label_Text.TabIndex = 0;
			this.label_Text.Text = "<Text>";
			// 
			// button_OK
			// 
			this.button_OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button_OK.Location = new System.Drawing.Point(25, 80);
			this.button_OK.Name = "button_OK";
			this.button_OK.Size = new System.Drawing.Size(75, 23);
			this.button_OK.TabIndex = 2;
			this.button_OK.Text = "OK";
			this.button_OK.UseVisualStyleBackColor = true;
			this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
			// 
			// button_Cancel
			// 
			this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button_Cancel.Location = new System.Drawing.Point(106, 80);
			this.button_Cancel.Name = "button_Cancel";
			this.button_Cancel.Size = new System.Drawing.Size(75, 23);
			this.button_Cancel.TabIndex = 3;
			this.button_Cancel.Text = "Cancel";
			this.button_Cancel.UseVisualStyleBackColor = true;
			this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
			// 
			// textBox_InputText
			// 
			this.textBox_InputText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBox_InputText.Location = new System.Drawing.Point(25, 44);
			this.textBox_InputText.Name = "textBox_InputText";
			this.textBox_InputText.Size = new System.Drawing.Size(156, 20);
			this.textBox_InputText.TabIndex = 1;
			this.textBox_InputText.Text = "<InitialText>";
			this.textBox_InputText.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_InputText_Validating);
			// 
			// TextInputBox
			// 
			this.AcceptButton = this.button_OK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.CancelButton = this.button_Cancel;
			this.ClientSize = new System.Drawing.Size(205, 119);
			this.Controls.Add(this.textBox_InputText);
			this.Controls.Add(this.button_Cancel);
			this.Controls.Add(this.button_OK);
			this.Controls.Add(this.label_Text);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(211, 144);
			this.Name = "TextInputBox";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "<Caption>";
			this.Shown += new System.EventHandler(this.TextInputBox_Shown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label_Text;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.Button button_Cancel;
		private MKY.Windows.Forms.TextBoxEx textBox_InputText;
	}
}