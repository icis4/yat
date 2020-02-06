using System;
using System.Collections.Generic;
using System.Text;

namespace YAT.View.Forms
{
	partial class Help
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
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Help));
			this.groupBox_ParserHelp = new System.Windows.Forms.GroupBox();
			this.splitContainer_Help = new System.Windows.Forms.SplitContainer();
			this.splitContainer_Upper = new System.Windows.Forms.SplitContainer();
			this.textBox_ParserFormat = new System.Windows.Forms.TextBox();
			this.textBox_ParserKeyword = new System.Windows.Forms.TextBox();
			this.splitContainer_Lower = new System.Windows.Forms.SplitContainer();
			this.textBox_TextTerminalKeyword = new System.Windows.Forms.TextBox();
			this.textBox_SerialPort = new System.Windows.Forms.TextBox();
			this.label_ParserExplanations = new System.Windows.Forms.Label();
			this.button_Close = new System.Windows.Forms.Button();
			this.label_Explanations = new System.Windows.Forms.Label();
			this.groupBox_AsciiTable = new System.Windows.Forms.GroupBox();
			this.dataGridView_AsciiTable = new System.Windows.Forms.DataGridView();
			this.dataGridViewTextBoxColumn_Dec = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn_Hex = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn_Mnemonic = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn_Esc = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn_Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.bindingSource_AsciiTable = new System.Windows.Forms.BindingSource(this.components);
			this.groupBox_ParserHelp.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer_Help)).BeginInit();
			this.splitContainer_Help.Panel1.SuspendLayout();
			this.splitContainer_Help.Panel2.SuspendLayout();
			this.splitContainer_Help.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer_Upper)).BeginInit();
			this.splitContainer_Upper.Panel1.SuspendLayout();
			this.splitContainer_Upper.Panel2.SuspendLayout();
			this.splitContainer_Upper.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer_Lower)).BeginInit();
			this.splitContainer_Lower.Panel1.SuspendLayout();
			this.splitContainer_Lower.Panel2.SuspendLayout();
			this.splitContainer_Lower.SuspendLayout();
			this.groupBox_AsciiTable.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView_AsciiTable)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingSource_AsciiTable)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox_ParserHelp
			// 
			this.groupBox_ParserHelp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox_ParserHelp.Controls.Add(this.splitContainer_Help);
			this.groupBox_ParserHelp.Controls.Add(this.label_ParserExplanations);
			this.groupBox_ParserHelp.Location = new System.Drawing.Point(12, 12);
			this.groupBox_ParserHelp.Name = "groupBox_ParserHelp";
			this.groupBox_ParserHelp.Size = new System.Drawing.Size(455, 490);
			this.groupBox_ParserHelp.TabIndex = 1;
			this.groupBox_ParserHelp.TabStop = false;
			this.groupBox_ParserHelp.Text = "Format";
			// 
			// splitContainer_Help
			// 
			this.splitContainer_Help.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer_Help.Location = new System.Drawing.Point(6, 51);
			this.splitContainer_Help.Name = "splitContainer_Help";
			this.splitContainer_Help.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer_Help.Panel1
			// 
			this.splitContainer_Help.Panel1.Controls.Add(this.splitContainer_Upper);
			this.splitContainer_Help.Panel1MinSize = 100;
			// 
			// splitContainer_Help.Panel2
			// 
			this.splitContainer_Help.Panel2.Controls.Add(this.splitContainer_Lower);
			this.splitContainer_Help.Panel2MinSize = 100;
			this.splitContainer_Help.Size = new System.Drawing.Size(443, 433);
			this.splitContainer_Help.SplitterDistance = 265;
			this.splitContainer_Help.TabIndex = 5;
			// 
			// splitContainer_Upper
			// 
			this.splitContainer_Upper.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer_Upper.Location = new System.Drawing.Point(0, 0);
			this.splitContainer_Upper.Name = "splitContainer_Upper";
			this.splitContainer_Upper.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer_Upper.Panel1
			// 
			this.splitContainer_Upper.Panel1.Controls.Add(this.textBox_ParserFormat);
			this.splitContainer_Upper.Panel1MinSize = 48;
			// 
			// splitContainer_Upper.Panel2
			// 
			this.splitContainer_Upper.Panel2.Controls.Add(this.textBox_ParserKeyword);
			this.splitContainer_Upper.Panel2MinSize = 48;
			this.splitContainer_Upper.Size = new System.Drawing.Size(443, 265);
			this.splitContainer_Upper.SplitterDistance = 181;
			this.splitContainer_Upper.TabIndex = 0;
			// 
			// textBox_ParserFormat
			// 
			this.textBox_ParserFormat.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBox_ParserFormat.Location = new System.Drawing.Point(0, 0);
			this.textBox_ParserFormat.Multiline = true;
			this.textBox_ParserFormat.Name = "textBox_ParserFormat";
			this.textBox_ParserFormat.ReadOnly = true;
			this.textBox_ParserFormat.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBox_ParserFormat.Size = new System.Drawing.Size(443, 181);
			this.textBox_ParserFormat.TabIndex = 0;
			this.textBox_ParserFormat.Text = "<PARSER FORMAT HELP>";
			// 
			// textBox_ParserKeyword
			// 
			this.textBox_ParserKeyword.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBox_ParserKeyword.Location = new System.Drawing.Point(0, 0);
			this.textBox_ParserKeyword.Multiline = true;
			this.textBox_ParserKeyword.Name = "textBox_ParserKeyword";
			this.textBox_ParserKeyword.ReadOnly = true;
			this.textBox_ParserKeyword.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBox_ParserKeyword.Size = new System.Drawing.Size(443, 80);
			this.textBox_ParserKeyword.TabIndex = 0;
			this.textBox_ParserKeyword.Text = "<PARSER KEYWORD HELP>";
			// 
			// splitContainer_Lower
			// 
			this.splitContainer_Lower.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer_Lower.Location = new System.Drawing.Point(0, 0);
			this.splitContainer_Lower.Name = "splitContainer_Lower";
			this.splitContainer_Lower.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer_Lower.Panel1
			// 
			this.splitContainer_Lower.Panel1.Controls.Add(this.textBox_TextTerminalKeyword);
			this.splitContainer_Lower.Panel1MinSize = 48;
			// 
			// splitContainer_Lower.Panel2
			// 
			this.splitContainer_Lower.Panel2.Controls.Add(this.textBox_SerialPort);
			this.splitContainer_Lower.Panel2MinSize = 48;
			this.splitContainer_Lower.Size = new System.Drawing.Size(443, 164);
			this.splitContainer_Lower.SplitterDistance = 80;
			this.splitContainer_Lower.TabIndex = 0;
			// 
			// textBox_TextTerminalKeyword
			// 
			this.textBox_TextTerminalKeyword.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBox_TextTerminalKeyword.Location = new System.Drawing.Point(0, 0);
			this.textBox_TextTerminalKeyword.Multiline = true;
			this.textBox_TextTerminalKeyword.Name = "textBox_TextTerminalKeyword";
			this.textBox_TextTerminalKeyword.ReadOnly = true;
			this.textBox_TextTerminalKeyword.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBox_TextTerminalKeyword.Size = new System.Drawing.Size(443, 80);
			this.textBox_TextTerminalKeyword.TabIndex = 0;
			this.textBox_TextTerminalKeyword.Text = "<TEXT TERMINAL KEYWORD HELP>";
			// 
			// textBox_SerialPort
			// 
			this.textBox_SerialPort.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBox_SerialPort.Location = new System.Drawing.Point(0, 0);
			this.textBox_SerialPort.Multiline = true;
			this.textBox_SerialPort.Name = "textBox_SerialPort";
			this.textBox_SerialPort.ReadOnly = true;
			this.textBox_SerialPort.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBox_SerialPort.Size = new System.Drawing.Size(443, 80);
			this.textBox_SerialPort.TabIndex = 0;
			this.textBox_SerialPort.Text = "<SERIAL COM PORT HELP>";
			// 
			// label_ParserExplanations
			// 
			this.label_ParserExplanations.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label_ParserExplanations.Location = new System.Drawing.Point(6, 16);
			this.label_ParserExplanations.Name = "label_ParserExplanations";
			this.label_ParserExplanations.Size = new System.Drawing.Size(443, 32);
			this.label_ParserExplanations.TabIndex = 0;
			this.label_ParserExplanations.Text = "Escape sequences and ASCII mnemonics allow sending data other than strings.\r\nThis" +
    " applies to text as well as to files; both configurable.";
			// 
			// button_Close
			// 
			this.button_Close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Close.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button_Close.Location = new System.Drawing.Point(693, 516);
			this.button_Close.Name = "button_Close";
			this.button_Close.Size = new System.Drawing.Size(75, 23);
			this.button_Close.TabIndex = 0;
			this.button_Close.Text = "Close";
			this.button_Close.Click += new System.EventHandler(this.button_Close_Click);
			// 
			// label_Explanations
			// 
			this.label_Explanations.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label_Explanations.AutoSize = true;
			this.label_Explanations.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_Explanations.ForeColor = System.Drawing.SystemColors.GrayText;
			this.label_Explanations.Location = new System.Drawing.Point(18, 521);
			this.label_Explanations.Name = "label_Explanations";
			this.label_Explanations.Size = new System.Drawing.Size(538, 13);
			this.label_Explanations.TabIndex = 3;
			this.label_Explanations.Text = "This is non-commercial software; there hasn\'t been resources to add a comprehensi" +
    "ve help yet; maybe one day...";
			// 
			// groupBox_AsciiTable
			// 
			this.groupBox_AsciiTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_AsciiTable.Controls.Add(this.dataGridView_AsciiTable);
			this.groupBox_AsciiTable.Location = new System.Drawing.Point(473, 12);
			this.groupBox_AsciiTable.Name = "groupBox_AsciiTable";
			this.groupBox_AsciiTable.Padding = new System.Windows.Forms.Padding(6);
			this.groupBox_AsciiTable.Size = new System.Drawing.Size(295, 490);
			this.groupBox_AsciiTable.TabIndex = 2;
			this.groupBox_AsciiTable.TabStop = false;
			this.groupBox_AsciiTable.Text = "ASCII Table";
			// 
			// dataGridView_AsciiTable
			// 
			this.dataGridView_AsciiTable.AllowUserToAddRows = false;
			this.dataGridView_AsciiTable.AllowUserToDeleteRows = false;
			this.dataGridView_AsciiTable.AllowUserToResizeColumns = false;
			this.dataGridView_AsciiTable.AllowUserToResizeRows = false;
			this.dataGridView_AsciiTable.AutoGenerateColumns = false;
			this.dataGridView_AsciiTable.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;
			this.dataGridView_AsciiTable.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
			this.dataGridView_AsciiTable.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
			this.dataGridView_AsciiTable.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dataGridView_AsciiTable.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dataGridView_AsciiTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView_AsciiTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn_Dec,
            this.dataGridViewTextBoxColumn_Hex,
            this.dataGridViewTextBoxColumn_Mnemonic,
            this.dataGridViewTextBoxColumn_Esc,
            this.dataGridViewTextBoxColumn_Description});
			this.dataGridView_AsciiTable.DataSource = this.bindingSource_AsciiTable;
			this.dataGridView_AsciiTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridView_AsciiTable.Location = new System.Drawing.Point(6, 19);
			this.dataGridView_AsciiTable.Name = "dataGridView_AsciiTable";
			this.dataGridView_AsciiTable.ReadOnly = true;
			this.dataGridView_AsciiTable.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
			this.dataGridView_AsciiTable.RowHeadersVisible = false;
			this.dataGridView_AsciiTable.Size = new System.Drawing.Size(283, 465);
			this.dataGridView_AsciiTable.TabIndex = 0;
			// 
			// dataGridViewTextBoxColumn_Dec
			// 
			this.dataGridViewTextBoxColumn_Dec.DataPropertyName = "Dec";
			this.dataGridViewTextBoxColumn_Dec.HeaderText = "D";
			this.dataGridViewTextBoxColumn_Dec.Name = "dataGridViewTextBoxColumn_Dec";
			this.dataGridViewTextBoxColumn_Dec.ReadOnly = true;
			this.dataGridViewTextBoxColumn_Dec.Width = 25;
			// 
			// dataGridViewTextBoxColumn_Hex
			// 
			this.dataGridViewTextBoxColumn_Hex.DataPropertyName = "Hex";
			this.dataGridViewTextBoxColumn_Hex.HeaderText = "H";
			this.dataGridViewTextBoxColumn_Hex.Name = "dataGridViewTextBoxColumn_Hex";
			this.dataGridViewTextBoxColumn_Hex.ReadOnly = true;
			this.dataGridViewTextBoxColumn_Hex.Width = 21;
			// 
			// dataGridViewTextBoxColumn_Mnemonic
			// 
			this.dataGridViewTextBoxColumn_Mnemonic.DataPropertyName = "Mnemonic";
			this.dataGridViewTextBoxColumn_Mnemonic.HeaderText = "Mnemonic";
			this.dataGridViewTextBoxColumn_Mnemonic.Name = "dataGridViewTextBoxColumn_Mnemonic";
			this.dataGridViewTextBoxColumn_Mnemonic.ReadOnly = true;
			this.dataGridViewTextBoxColumn_Mnemonic.Width = 60;
			// 
			// dataGridViewTextBoxColumn_Esc
			// 
			this.dataGridViewTextBoxColumn_Esc.DataPropertyName = "Esc";
			this.dataGridViewTextBoxColumn_Esc.HeaderText = "Esc";
			this.dataGridViewTextBoxColumn_Esc.Name = "dataGridViewTextBoxColumn_Esc";
			this.dataGridViewTextBoxColumn_Esc.ReadOnly = true;
			this.dataGridViewTextBoxColumn_Esc.Width = 31;
			// 
			// dataGridViewTextBoxColumn_Description
			// 
			this.dataGridViewTextBoxColumn_Description.DataPropertyName = "Description";
			this.dataGridViewTextBoxColumn_Description.HeaderText = "Description";
			this.dataGridViewTextBoxColumn_Description.Name = "dataGridViewTextBoxColumn_Description";
			this.dataGridViewTextBoxColumn_Description.ReadOnly = true;
			this.dataGridViewTextBoxColumn_Description.Width = 127;
			// 
			// bindingSource_AsciiTable
			// 
			this.bindingSource_AsciiTable.DataMember = "ASCII";
			this.bindingSource_AsciiTable.DataSource = typeof(YAT.View.Utilities.AsciiTableDataSet);
			this.bindingSource_AsciiTable.Position = 0;
			// 
			// Help
			// 
			this.AcceptButton = this.button_Close;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button_Close;
			this.ClientSize = new System.Drawing.Size(780, 551);
			this.Controls.Add(this.groupBox_AsciiTable);
			this.Controls.Add(this.label_Explanations);
			this.Controls.Add(this.button_Close);
			this.Controls.Add(this.groupBox_ParserHelp);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Help";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "YAT Help";
			this.Load += new System.EventHandler(this.Help_Load);
			this.groupBox_ParserHelp.ResumeLayout(false);
			this.splitContainer_Help.Panel1.ResumeLayout(false);
			this.splitContainer_Help.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer_Help)).EndInit();
			this.splitContainer_Help.ResumeLayout(false);
			this.splitContainer_Upper.Panel1.ResumeLayout(false);
			this.splitContainer_Upper.Panel1.PerformLayout();
			this.splitContainer_Upper.Panel2.ResumeLayout(false);
			this.splitContainer_Upper.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer_Upper)).EndInit();
			this.splitContainer_Upper.ResumeLayout(false);
			this.splitContainer_Lower.Panel1.ResumeLayout(false);
			this.splitContainer_Lower.Panel1.PerformLayout();
			this.splitContainer_Lower.Panel2.ResumeLayout(false);
			this.splitContainer_Lower.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer_Lower)).EndInit();
			this.splitContainer_Lower.ResumeLayout(false);
			this.groupBox_AsciiTable.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView_AsciiTable)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingSource_AsciiTable)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.GroupBox groupBox_ParserHelp;
		private System.Windows.Forms.Button button_Close;
		private System.Windows.Forms.Label label_Explanations;
		private System.Windows.Forms.Label label_ParserExplanations;
		private System.Windows.Forms.TextBox textBox_ParserFormat;
		private System.Windows.Forms.TextBox textBox_TextTerminalKeyword;
		private System.Windows.Forms.TextBox textBox_ParserKeyword;
		private System.Windows.Forms.GroupBox groupBox_AsciiTable;
		private System.Windows.Forms.DataGridView dataGridView_AsciiTable;
		private System.Windows.Forms.BindingSource bindingSource_AsciiTable;
		private System.Windows.Forms.TextBox textBox_SerialPort;
		private System.Windows.Forms.SplitContainer splitContainer_Help;
		private System.Windows.Forms.SplitContainer splitContainer_Upper;
		private System.Windows.Forms.SplitContainer splitContainer_Lower;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn_Dec;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn_Hex;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn_Mnemonic;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn_Esc;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn_Description;
	}
}
