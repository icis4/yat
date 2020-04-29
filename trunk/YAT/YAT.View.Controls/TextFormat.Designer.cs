namespace YAT.View.Controls
{
	partial class TextFormat
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
				if (this.font != null) {
					this.font.Dispose();
					this.font = null;
				}
			}

			// Dispose of designer generated managed resources:
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
			this.checkBox_Italic = new System.Windows.Forms.CheckBox();
			this.checkBox_Bold = new System.Windows.Forms.CheckBox();
			this.button_Color = new System.Windows.Forms.Button();
			this.checkBox_Strikeout = new System.Windows.Forms.CheckBox();
			this.checkBox_Underline = new System.Windows.Forms.CheckBox();
			this.colorDialog = new System.Windows.Forms.ColorDialog();
			this.SuspendLayout();
			// 
			// checkBox_Italic
			// 
			this.checkBox_Italic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBox_Italic.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.checkBox_Italic.Location = new System.Drawing.Point(43, 2);
			this.checkBox_Italic.Name = "checkBox_Italic";
			this.checkBox_Italic.Size = new System.Drawing.Size(32, 21);
			this.checkBox_Italic.TabIndex = 1;
			this.checkBox_Italic.Text = "I";
			this.checkBox_Italic.CheckedChanged += new System.EventHandler(this.checkBox_Italic_CheckedChanged);
			// 
			// checkBox_Bold
			// 
			this.checkBox_Bold.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBox_Bold.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.checkBox_Bold.Location = new System.Drawing.Point(5, 2);
			this.checkBox_Bold.Name = "checkBox_Bold";
			this.checkBox_Bold.Size = new System.Drawing.Size(32, 21);
			this.checkBox_Bold.TabIndex = 0;
			this.checkBox_Bold.Text = "B";
			this.checkBox_Bold.CheckedChanged += new System.EventHandler(this.checkBox_Bold_CheckedChanged);
			// 
			// button_Color
			// 
			this.button_Color.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Color.Location = new System.Drawing.Point(157, 0);
			this.button_Color.Name = "button_Color";
			this.button_Color.Size = new System.Drawing.Size(75, 23);
			this.button_Color.TabIndex = 4;
			this.button_Color.Text = "Color...";
			this.button_Color.Click += new System.EventHandler(this.button_Color_Click);
			// 
			// checkBox_Strikeout
			// 
			this.checkBox_Strikeout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBox_Strikeout.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Strikeout, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.checkBox_Strikeout.Location = new System.Drawing.Point(119, 2);
			this.checkBox_Strikeout.Name = "checkBox_Strikeout";
			this.checkBox_Strikeout.Size = new System.Drawing.Size(32, 21);
			this.checkBox_Strikeout.TabIndex = 3;
			this.checkBox_Strikeout.Text = "S";
			this.checkBox_Strikeout.CheckedChanged += new System.EventHandler(this.checkBox_Strikeout_CheckedChanged);
			// 
			// checkBox_Underline
			// 
			this.checkBox_Underline.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBox_Underline.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.checkBox_Underline.Location = new System.Drawing.Point(81, 2);
			this.checkBox_Underline.Name = "checkBox_Underline";
			this.checkBox_Underline.Size = new System.Drawing.Size(32, 21);
			this.checkBox_Underline.TabIndex = 2;
			this.checkBox_Underline.Text = "U";
			this.checkBox_Underline.CheckedChanged += new System.EventHandler(this.checkBox_Underline_CheckedChanged);
			// 
			// TextFormat
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.Controls.Add(this.checkBox_Underline);
			this.Controls.Add(this.checkBox_Strikeout);
			this.Controls.Add(this.checkBox_Italic);
			this.Controls.Add(this.checkBox_Bold);
			this.Controls.Add(this.button_Color);
			this.Name = "TextFormat";
			this.Size = new System.Drawing.Size(232, 23);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.CheckBox checkBox_Bold;
		private System.Windows.Forms.CheckBox checkBox_Italic;
		private System.Windows.Forms.CheckBox checkBox_Underline;
		private System.Windows.Forms.CheckBox checkBox_Strikeout;
		private System.Windows.Forms.Button button_Color;
		private System.Windows.Forms.ColorDialog colorDialog;
	}
}
