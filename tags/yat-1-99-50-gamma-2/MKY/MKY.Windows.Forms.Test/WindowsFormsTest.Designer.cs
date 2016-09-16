namespace MKY.Windows.Forms.Test
{
	partial class WindowsFormsTest
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
			this.button_FastListBox_AddLine = new System.Windows.Forms.Button();
			this.button_FastListBox_AddManyLines = new System.Windows.Forms.Button();
			this.button_FastListBox_RemoveLine = new System.Windows.Forms.Button();
			this.button_FastListBox_RemoveManyLines = new System.Windows.Forms.Button();
			this.fastListBox = new MKY.Windows.Forms.FastListBox();
			this.groupBox_FastListBox = new System.Windows.Forms.GroupBox();
			this.groupBox_ListBoxEx = new System.Windows.Forms.GroupBox();
			this.button_ListBoxEx_AddSeveralChars = new System.Windows.Forms.Button();
			this.label_ListBoxEx_VerticalScrollType = new System.Windows.Forms.Label();
			this.label_ListBoxEx_VerticalScrollPositionNew = new System.Windows.Forms.Label();
			this.label_ListBoxEx_VerticalScrollPositionOld = new System.Windows.Forms.Label();
			this.label_ListBoxEx_HorizontalScrollType = new System.Windows.Forms.Label();
			this.label_ListBoxEx_HorizontalScrollPositionNew = new System.Windows.Forms.Label();
			this.label_ListBoxEx_HorizontalScrollPositionOld = new System.Windows.Forms.Label();
			this.label_ListBoxEx_VerticalScroll = new System.Windows.Forms.Label();
			this.label_ListBoxEx_HorizontalScroll = new System.Windows.Forms.Label();
			this.button_ListBoxEx_AddManyLines = new System.Windows.Forms.Button();
			this.button_ListBoxEx_AddLine = new System.Windows.Forms.Button();
			this.button_ListBoxEx_AddChar = new System.Windows.Forms.Button();
			this.listBoxEx = new MKY.Windows.Forms.ListBoxEx();
			this.groupBox_FastListBox.SuspendLayout();
			this.groupBox_ListBoxEx.SuspendLayout();
			this.SuspendLayout();
			// 
			// button_FastListBox_AddLine
			// 
			this.button_FastListBox_AddLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_FastListBox_AddLine.Location = new System.Drawing.Point(351, 19);
			this.button_FastListBox_AddLine.Name = "button_FastListBox_AddLine";
			this.button_FastListBox_AddLine.Size = new System.Drawing.Size(100, 23);
			this.button_FastListBox_AddLine.TabIndex = 1;
			this.button_FastListBox_AddLine.Text = "&Add";
			this.button_FastListBox_AddLine.UseVisualStyleBackColor = true;
			this.button_FastListBox_AddLine.Click += new System.EventHandler(this.button_FastListBox_AddLine_Click);
			// 
			// button_FastListBox_AddManyLines
			// 
			this.button_FastListBox_AddManyLines.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_FastListBox_AddManyLines.Location = new System.Drawing.Point(351, 48);
			this.button_FastListBox_AddManyLines.Name = "button_FastListBox_AddManyLines";
			this.button_FastListBox_AddManyLines.Size = new System.Drawing.Size(100, 23);
			this.button_FastListBox_AddManyLines.TabIndex = 2;
			this.button_FastListBox_AddManyLines.Text = "A&dd Many";
			this.button_FastListBox_AddManyLines.UseVisualStyleBackColor = true;
			this.button_FastListBox_AddManyLines.Click += new System.EventHandler(this.button_FastListBox_AddManyLines_Click);
			// 
			// button_FastListBox_RemoveLine
			// 
			this.button_FastListBox_RemoveLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_FastListBox_RemoveLine.Location = new System.Drawing.Point(351, 77);
			this.button_FastListBox_RemoveLine.Name = "button_FastListBox_RemoveLine";
			this.button_FastListBox_RemoveLine.Size = new System.Drawing.Size(100, 23);
			this.button_FastListBox_RemoveLine.TabIndex = 3;
			this.button_FastListBox_RemoveLine.Text = "&Remove";
			this.button_FastListBox_RemoveLine.UseVisualStyleBackColor = true;
			this.button_FastListBox_RemoveLine.Click += new System.EventHandler(this.button_FastListBox_RemoveLine_Click);
			// 
			// button_FastListBox_RemoveManyLines
			// 
			this.button_FastListBox_RemoveManyLines.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_FastListBox_RemoveManyLines.Location = new System.Drawing.Point(351, 106);
			this.button_FastListBox_RemoveManyLines.Name = "button_FastListBox_RemoveManyLines";
			this.button_FastListBox_RemoveManyLines.Size = new System.Drawing.Size(100, 23);
			this.button_FastListBox_RemoveManyLines.TabIndex = 4;
			this.button_FastListBox_RemoveManyLines.Text = "R&emove Many";
			this.button_FastListBox_RemoveManyLines.UseVisualStyleBackColor = true;
			this.button_FastListBox_RemoveManyLines.Click += new System.EventHandler(this.button_FastListBox_RemoveManyLines_Click);
			// 
			// fastListBox
			// 
			this.fastListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.fastListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.fastListBox.HorizontalScrollbar = true;
			this.fastListBox.IntegralHeight = false;
			this.fastListBox.Location = new System.Drawing.Point(6, 19);
			this.fastListBox.Name = "fastListBox";
			this.fastListBox.ScrollAlwaysVisible = true;
			this.fastListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.fastListBox.Size = new System.Drawing.Size(339, 553);
			this.fastListBox.TabIndex = 0;
			this.fastListBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.fastListBox_DrawItem);
			// 
			// groupBox_FastListBox
			// 
			this.groupBox_FastListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_FastListBox.Controls.Add(this.fastListBox);
			this.groupBox_FastListBox.Controls.Add(this.button_FastListBox_RemoveManyLines);
			this.groupBox_FastListBox.Controls.Add(this.button_FastListBox_RemoveLine);
			this.groupBox_FastListBox.Controls.Add(this.button_FastListBox_AddLine);
			this.groupBox_FastListBox.Controls.Add(this.button_FastListBox_AddManyLines);
			this.groupBox_FastListBox.Location = new System.Drawing.Point(475, 12);
			this.groupBox_FastListBox.Name = "groupBox_FastListBox";
			this.groupBox_FastListBox.Size = new System.Drawing.Size(457, 578);
			this.groupBox_FastListBox.TabIndex = 1;
			this.groupBox_FastListBox.TabStop = false;
			this.groupBox_FastListBox.Text = "&FastListBox";
			// 
			// groupBox_ListBoxEx
			// 
			this.groupBox_ListBoxEx.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_ListBoxEx.Controls.Add(this.button_ListBoxEx_AddSeveralChars);
			this.groupBox_ListBoxEx.Controls.Add(this.label_ListBoxEx_VerticalScrollType);
			this.groupBox_ListBoxEx.Controls.Add(this.label_ListBoxEx_VerticalScrollPositionNew);
			this.groupBox_ListBoxEx.Controls.Add(this.label_ListBoxEx_VerticalScrollPositionOld);
			this.groupBox_ListBoxEx.Controls.Add(this.label_ListBoxEx_HorizontalScrollType);
			this.groupBox_ListBoxEx.Controls.Add(this.label_ListBoxEx_HorizontalScrollPositionNew);
			this.groupBox_ListBoxEx.Controls.Add(this.label_ListBoxEx_HorizontalScrollPositionOld);
			this.groupBox_ListBoxEx.Controls.Add(this.label_ListBoxEx_VerticalScroll);
			this.groupBox_ListBoxEx.Controls.Add(this.label_ListBoxEx_HorizontalScroll);
			this.groupBox_ListBoxEx.Controls.Add(this.button_ListBoxEx_AddManyLines);
			this.groupBox_ListBoxEx.Controls.Add(this.button_ListBoxEx_AddLine);
			this.groupBox_ListBoxEx.Controls.Add(this.button_ListBoxEx_AddChar);
			this.groupBox_ListBoxEx.Controls.Add(this.listBoxEx);
			this.groupBox_ListBoxEx.Location = new System.Drawing.Point(12, 12);
			this.groupBox_ListBoxEx.Name = "groupBox_ListBoxEx";
			this.groupBox_ListBoxEx.Size = new System.Drawing.Size(457, 578);
			this.groupBox_ListBoxEx.TabIndex = 0;
			this.groupBox_ListBoxEx.TabStop = false;
			this.groupBox_ListBoxEx.Text = "ListBoxEx";
			// 
			// button_ListBoxEx_AddSeveralChars
			// 
			this.button_ListBoxEx_AddSeveralChars.Location = new System.Drawing.Point(351, 48);
			this.button_ListBoxEx_AddSeveralChars.Name = "button_ListBoxEx_AddSeveralChars";
			this.button_ListBoxEx_AddSeveralChars.Size = new System.Drawing.Size(100, 23);
			this.button_ListBoxEx_AddSeveralChars.TabIndex = 2;
			this.button_ListBoxEx_AddSeveralChars.Text = "Add Some C&hars";
			this.button_ListBoxEx_AddSeveralChars.UseVisualStyleBackColor = true;
			this.button_ListBoxEx_AddSeveralChars.Click += new System.EventHandler(this.button_ListBoxEx_AddSeveralChars_Click);
			// 
			// label_ListBoxEx_VerticalScrollType
			// 
			this.label_ListBoxEx_VerticalScrollType.AutoSize = true;
			this.label_ListBoxEx_VerticalScrollType.Location = new System.Drawing.Point(351, 294);
			this.label_ListBoxEx_VerticalScrollType.Name = "label_ListBoxEx_VerticalScrollType";
			this.label_ListBoxEx_VerticalScrollType.Size = new System.Drawing.Size(43, 13);
			this.label_ListBoxEx_VerticalScrollType.TabIndex = 12;
			this.label_ListBoxEx_VerticalScrollType.Text = "<Type>";
			// 
			// label_ListBoxEx_VerticalScrollPositionNew
			// 
			this.label_ListBoxEx_VerticalScrollPositionNew.AutoSize = true;
			this.label_ListBoxEx_VerticalScrollPositionNew.Location = new System.Drawing.Point(351, 281);
			this.label_ListBoxEx_VerticalScrollPositionNew.Name = "label_ListBoxEx_VerticalScrollPositionNew";
			this.label_ListBoxEx_VerticalScrollPositionNew.Size = new System.Drawing.Size(41, 13);
			this.label_ListBoxEx_VerticalScrollPositionNew.TabIndex = 11;
			this.label_ListBoxEx_VerticalScrollPositionNew.Text = "<New>";
			// 
			// label_ListBoxEx_VerticalScrollPositionOld
			// 
			this.label_ListBoxEx_VerticalScrollPositionOld.AutoSize = true;
			this.label_ListBoxEx_VerticalScrollPositionOld.Location = new System.Drawing.Point(351, 268);
			this.label_ListBoxEx_VerticalScrollPositionOld.Name = "label_ListBoxEx_VerticalScrollPositionOld";
			this.label_ListBoxEx_VerticalScrollPositionOld.Size = new System.Drawing.Size(35, 13);
			this.label_ListBoxEx_VerticalScrollPositionOld.TabIndex = 10;
			this.label_ListBoxEx_VerticalScrollPositionOld.Text = "<Old>";
			// 
			// label_ListBoxEx_HorizontalScrollType
			// 
			this.label_ListBoxEx_HorizontalScrollType.AutoSize = true;
			this.label_ListBoxEx_HorizontalScrollType.Location = new System.Drawing.Point(351, 218);
			this.label_ListBoxEx_HorizontalScrollType.Name = "label_ListBoxEx_HorizontalScrollType";
			this.label_ListBoxEx_HorizontalScrollType.Size = new System.Drawing.Size(43, 13);
			this.label_ListBoxEx_HorizontalScrollType.TabIndex = 8;
			this.label_ListBoxEx_HorizontalScrollType.Text = "<Type>";
			// 
			// label_ListBoxEx_HorizontalScrollPositionNew
			// 
			this.label_ListBoxEx_HorizontalScrollPositionNew.AutoSize = true;
			this.label_ListBoxEx_HorizontalScrollPositionNew.Location = new System.Drawing.Point(351, 205);
			this.label_ListBoxEx_HorizontalScrollPositionNew.Name = "label_ListBoxEx_HorizontalScrollPositionNew";
			this.label_ListBoxEx_HorizontalScrollPositionNew.Size = new System.Drawing.Size(41, 13);
			this.label_ListBoxEx_HorizontalScrollPositionNew.TabIndex = 7;
			this.label_ListBoxEx_HorizontalScrollPositionNew.Text = "<New>";
			// 
			// label_ListBoxEx_HorizontalScrollPositionOld
			// 
			this.label_ListBoxEx_HorizontalScrollPositionOld.AutoSize = true;
			this.label_ListBoxEx_HorizontalScrollPositionOld.Location = new System.Drawing.Point(351, 192);
			this.label_ListBoxEx_HorizontalScrollPositionOld.Name = "label_ListBoxEx_HorizontalScrollPositionOld";
			this.label_ListBoxEx_HorizontalScrollPositionOld.Size = new System.Drawing.Size(35, 13);
			this.label_ListBoxEx_HorizontalScrollPositionOld.TabIndex = 6;
			this.label_ListBoxEx_HorizontalScrollPositionOld.Text = "<Old>";
			// 
			// label_ListBoxEx_VerticalScroll
			// 
			this.label_ListBoxEx_VerticalScroll.AutoSize = true;
			this.label_ListBoxEx_VerticalScroll.Location = new System.Drawing.Point(351, 255);
			this.label_ListBoxEx_VerticalScroll.Name = "label_ListBoxEx_VerticalScroll";
			this.label_ListBoxEx_VerticalScroll.Size = new System.Drawing.Size(46, 13);
			this.label_ListBoxEx_VerticalScroll.TabIndex = 9;
			this.label_ListBoxEx_VerticalScroll.Text = "V-Scroll:";
			// 
			// label_ListBoxEx_HorizontalScroll
			// 
			this.label_ListBoxEx_HorizontalScroll.AutoSize = true;
			this.label_ListBoxEx_HorizontalScroll.Location = new System.Drawing.Point(351, 179);
			this.label_ListBoxEx_HorizontalScroll.Name = "label_ListBoxEx_HorizontalScroll";
			this.label_ListBoxEx_HorizontalScroll.Size = new System.Drawing.Size(47, 13);
			this.label_ListBoxEx_HorizontalScroll.TabIndex = 5;
			this.label_ListBoxEx_HorizontalScroll.Text = "H-Scroll:";
			// 
			// button_ListBoxEx_AddManyLines
			// 
			this.button_ListBoxEx_AddManyLines.Location = new System.Drawing.Point(351, 106);
			this.button_ListBoxEx_AddManyLines.Name = "button_ListBoxEx_AddManyLines";
			this.button_ListBoxEx_AddManyLines.Size = new System.Drawing.Size(100, 23);
			this.button_ListBoxEx_AddManyLines.TabIndex = 4;
			this.button_ListBoxEx_AddManyLines.Text = "Add Many L&ines";
			this.button_ListBoxEx_AddManyLines.UseVisualStyleBackColor = true;
			this.button_ListBoxEx_AddManyLines.Click += new System.EventHandler(this.button_ListBoxEx_AddManyLines_Click);
			// 
			// button_ListBoxEx_AddLine
			// 
			this.button_ListBoxEx_AddLine.Location = new System.Drawing.Point(351, 77);
			this.button_ListBoxEx_AddLine.Name = "button_ListBoxEx_AddLine";
			this.button_ListBoxEx_AddLine.Size = new System.Drawing.Size(100, 23);
			this.button_ListBoxEx_AddLine.TabIndex = 3;
			this.button_ListBoxEx_AddLine.Text = "Add &Line";
			this.button_ListBoxEx_AddLine.UseVisualStyleBackColor = true;
			this.button_ListBoxEx_AddLine.Click += new System.EventHandler(this.button_ListBoxEx_AddLine_Click);
			// 
			// button_ListBoxEx_AddChar
			// 
			this.button_ListBoxEx_AddChar.Location = new System.Drawing.Point(351, 19);
			this.button_ListBoxEx_AddChar.Name = "button_ListBoxEx_AddChar";
			this.button_ListBoxEx_AddChar.Size = new System.Drawing.Size(100, 23);
			this.button_ListBoxEx_AddChar.TabIndex = 1;
			this.button_ListBoxEx_AddChar.Text = "Add &Char";
			this.button_ListBoxEx_AddChar.UseVisualStyleBackColor = true;
			this.button_ListBoxEx_AddChar.Click += new System.EventHandler(this.button_ListBoxEx_AddChar_Click);
			// 
			// listBoxEx
			// 
			this.listBoxEx.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxEx.HorizontalScrollbar = true;
			this.listBoxEx.IntegralHeight = false;
			this.listBoxEx.Location = new System.Drawing.Point(6, 19);
			this.listBoxEx.Name = "listBoxEx";
			this.listBoxEx.ScrollAlwaysVisible = true;
			this.listBoxEx.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listBoxEx.Size = new System.Drawing.Size(339, 553);
			this.listBoxEx.TabIndex = 0;
//			this.listBoxEx.VerticalScrolled += new System.Windows.Forms.ScrollEventHandler(this.listBoxEx_VerticalScrolled);
//			this.listBoxEx.HorizontalScrolled += new System.Windows.Forms.ScrollEventHandler(this.listBoxEx_HorizontalScrolled);
			// 
			// WindowsFormsTest
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(944, 602);
			this.Controls.Add(this.groupBox_ListBoxEx);
			this.Controls.Add(this.groupBox_FastListBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "WindowsFormsTest";
			this.Text = "MKY.Windows.Forms.Test";
			this.groupBox_FastListBox.ResumeLayout(false);
			this.groupBox_ListBoxEx.ResumeLayout(false);
			this.groupBox_ListBoxEx.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private FastListBox fastListBox;
		private System.Windows.Forms.Button button_FastListBox_AddLine;
		private System.Windows.Forms.Button button_FastListBox_AddManyLines;
		private System.Windows.Forms.Button button_FastListBox_RemoveLine;
		private System.Windows.Forms.Button button_FastListBox_RemoveManyLines;
		private System.Windows.Forms.GroupBox groupBox_FastListBox;
		private System.Windows.Forms.GroupBox groupBox_ListBoxEx;
		private ListBoxEx listBoxEx;
		private System.Windows.Forms.Button button_ListBoxEx_AddManyLines;
		private System.Windows.Forms.Button button_ListBoxEx_AddLine;
		private System.Windows.Forms.Button button_ListBoxEx_AddChar;
		private System.Windows.Forms.Label label_ListBoxEx_VerticalScroll;
		private System.Windows.Forms.Label label_ListBoxEx_HorizontalScroll;
		private System.Windows.Forms.Label label_ListBoxEx_HorizontalScrollPositionOld;
		private System.Windows.Forms.Label label_ListBoxEx_VerticalScrollType;
		private System.Windows.Forms.Label label_ListBoxEx_VerticalScrollPositionNew;
		private System.Windows.Forms.Label label_ListBoxEx_VerticalScrollPositionOld;
		private System.Windows.Forms.Label label_ListBoxEx_HorizontalScrollType;
		private System.Windows.Forms.Label label_ListBoxEx_HorizontalScrollPositionNew;
		private System.Windows.Forms.Button button_ListBoxEx_AddSeveralChars;
	}
}

