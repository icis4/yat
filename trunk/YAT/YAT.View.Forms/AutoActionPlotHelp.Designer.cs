using System;
using System.Collections.Generic;
using System.Text;

namespace YAT.View.Forms
{
	partial class AutoActionPlotHelp
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AutoActionPlotHelp));
			this.groupBox_ByFunction = new System.Windows.Forms.GroupBox();
			this.dataGridView_ByFunction = new System.Windows.Forms.DataGridView();
			this.bindingSource_ByFunction = new System.Windows.Forms.BindingSource(this.components);
			this.button_Close = new System.Windows.Forms.Button();
			this.groupBox_ByInput = new System.Windows.Forms.GroupBox();
			this.dataGridView_ByInput = new System.Windows.Forms.DataGridView();
			this.bindingSource_ByInput = new System.Windows.Forms.BindingSource(this.components);
			this.linkLabel_Remark = new System.Windows.Forms.LinkLabel();
			this.dataGridViewTextBoxColumn_ByFunction_Function = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn_ByFunction_Scope = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn_ByFunction_Arrow = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn_ByFunction_Modifier = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn_ByFunction_Input = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn_ByInput_Modifier = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn_ByInput_Input = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn_ByInput_Arrow = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn_ByInput_Function = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn_ByInput_Scope = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.groupBox_ByFunction.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView_ByFunction)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingSource_ByFunction)).BeginInit();
			this.groupBox_ByInput.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView_ByInput)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingSource_ByInput)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox_ByFunction
			// 
			this.groupBox_ByFunction.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox_ByFunction.Controls.Add(this.dataGridView_ByFunction);
			this.groupBox_ByFunction.Location = new System.Drawing.Point(12, 12);
			this.groupBox_ByFunction.Name = "groupBox_ByFunction";
			this.groupBox_ByFunction.Padding = new System.Windows.Forms.Padding(6);
			this.groupBox_ByFunction.Size = new System.Drawing.Size(351, 505);
			this.groupBox_ByFunction.TabIndex = 2;
			this.groupBox_ByFunction.TabStop = false;
			this.groupBox_ByFunction.Text = "By &Function";
			// 
			// dataGridView_ByFunction
			// 
			this.dataGridView_ByFunction.AllowUserToAddRows = false;
			this.dataGridView_ByFunction.AllowUserToDeleteRows = false;
			this.dataGridView_ByFunction.AllowUserToResizeColumns = false;
			this.dataGridView_ByFunction.AllowUserToResizeRows = false;
			this.dataGridView_ByFunction.AutoGenerateColumns = false;
			this.dataGridView_ByFunction.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;
			this.dataGridView_ByFunction.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
			this.dataGridView_ByFunction.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleVertical;
			this.dataGridView_ByFunction.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dataGridView_ByFunction.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dataGridView_ByFunction.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView_ByFunction.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn_ByFunction_Function,
            this.dataGridViewTextBoxColumn_ByFunction_Scope,
            this.dataGridViewTextBoxColumn_ByFunction_Arrow,
            this.dataGridViewTextBoxColumn_ByFunction_Modifier,
            this.dataGridViewTextBoxColumn_ByFunction_Input});
			this.dataGridView_ByFunction.DataSource = this.bindingSource_ByFunction;
			this.dataGridView_ByFunction.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridView_ByFunction.Location = new System.Drawing.Point(6, 19);
			this.dataGridView_ByFunction.Name = "dataGridView_ByFunction";
			this.dataGridView_ByFunction.ReadOnly = true;
			this.dataGridView_ByFunction.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
			this.dataGridView_ByFunction.RowHeadersVisible = false;
			this.dataGridView_ByFunction.Size = new System.Drawing.Size(339, 480);
			this.dataGridView_ByFunction.TabIndex = 1;
			// 
			// bindingSource_ByFunction
			// 
			this.bindingSource_ByFunction.DataMember = "ByFunction";
			this.bindingSource_ByFunction.DataSource = typeof(YAT.View.Utilities.PlotInteractionDataSet);
			this.bindingSource_ByFunction.Position = 0;
			// 
			// button_Close
			// 
			this.button_Close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Close.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button_Close.Location = new System.Drawing.Point(668, 531);
			this.button_Close.Name = "button_Close";
			this.button_Close.Size = new System.Drawing.Size(75, 23);
			this.button_Close.TabIndex = 0;
			this.button_Close.Text = "Close";
			this.button_Close.Click += new System.EventHandler(this.button_Close_Click);
			// 
			// groupBox_ByInput
			// 
			this.groupBox_ByInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_ByInput.Controls.Add(this.dataGridView_ByInput);
			this.groupBox_ByInput.Location = new System.Drawing.Point(376, 12);
			this.groupBox_ByInput.Name = "groupBox_ByInput";
			this.groupBox_ByInput.Padding = new System.Windows.Forms.Padding(6);
			this.groupBox_ByInput.Size = new System.Drawing.Size(373, 505);
			this.groupBox_ByInput.TabIndex = 3;
			this.groupBox_ByInput.TabStop = false;
			this.groupBox_ByInput.Text = "By &Input";
			// 
			// dataGridView_ByInput
			// 
			this.dataGridView_ByInput.AllowUserToAddRows = false;
			this.dataGridView_ByInput.AllowUserToDeleteRows = false;
			this.dataGridView_ByInput.AllowUserToResizeColumns = false;
			this.dataGridView_ByInput.AllowUserToResizeRows = false;
			this.dataGridView_ByInput.AutoGenerateColumns = false;
			this.dataGridView_ByInput.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;
			this.dataGridView_ByInput.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
			this.dataGridView_ByInput.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleVertical;
			this.dataGridView_ByInput.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dataGridView_ByInput.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
			this.dataGridView_ByInput.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView_ByInput.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn_ByInput_Modifier,
            this.dataGridViewTextBoxColumn_ByInput_Input,
            this.dataGridViewTextBoxColumn_ByInput_Arrow,
            this.dataGridViewTextBoxColumn_ByInput_Function,
            this.dataGridViewTextBoxColumn_ByInput_Scope});
			this.dataGridView_ByInput.DataSource = this.bindingSource_ByInput;
			this.dataGridView_ByInput.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridView_ByInput.Location = new System.Drawing.Point(6, 19);
			this.dataGridView_ByInput.Name = "dataGridView_ByInput";
			this.dataGridView_ByInput.ReadOnly = true;
			this.dataGridView_ByInput.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
			this.dataGridView_ByInput.RowHeadersVisible = false;
			this.dataGridView_ByInput.Size = new System.Drawing.Size(361, 480);
			this.dataGridView_ByInput.TabIndex = 0;
			// 
			// bindingSource_ByInput
			// 
			this.bindingSource_ByInput.DataMember = "ByInput";
			this.bindingSource_ByInput.DataSource = typeof(YAT.View.Utilities.PlotInteractionDataSet);
			this.bindingSource_ByInput.Position = 0;
			// 
			// linkLabel_Remark
			// 
			this.linkLabel_Remark.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.linkLabel_Remark.AutoSize = true;
			this.linkLabel_Remark.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_Remark.Location = new System.Drawing.Point(21, 536);
			this.linkLabel_Remark.Name = "linkLabel_Remark";
			this.linkLabel_Remark.Size = new System.Drawing.Size(540, 13);
			this.linkLabel_Remark.TabIndex = 1;
			this.linkLabel_Remark.Tag = "";
			this.linkLabel_Remark.Text = "YAT uses OxyPlot default interactions, except for [Pan → Left Mouse Button] and [" +
    "Menu → Right Mouse Button].";
			this.linkLabel_Remark.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_Remark_LinkClicked);
			// 
			// dataGridViewTextBoxColumn_ByFunction_Function
			// 
			this.dataGridViewTextBoxColumn_ByFunction_Function.DataPropertyName = "Function";
			this.dataGridViewTextBoxColumn_ByFunction_Function.HeaderText = "Function";
			this.dataGridViewTextBoxColumn_ByFunction_Function.Name = "dataGridViewTextBoxColumn_ByFunction_Function";
			this.dataGridViewTextBoxColumn_ByFunction_Function.ReadOnly = true;
			this.dataGridViewTextBoxColumn_ByFunction_Function.Width = 79;
			// 
			// dataGridViewTextBoxColumn_ByFunction_Scope
			// 
			this.dataGridViewTextBoxColumn_ByFunction_Scope.DataPropertyName = "Scope";
			this.dataGridViewTextBoxColumn_ByFunction_Scope.HeaderText = "Scope";
			this.dataGridViewTextBoxColumn_ByFunction_Scope.Name = "dataGridViewTextBoxColumn_ByFunction_Scope";
			this.dataGridViewTextBoxColumn_ByFunction_Scope.ReadOnly = true;
			this.dataGridViewTextBoxColumn_ByFunction_Scope.Width = 21;
			// 
			// dataGridViewTextBoxColumn_ByFunction_Arrow
			// 
			this.dataGridViewTextBoxColumn_ByFunction_Arrow.DataPropertyName = "Arrow";
			this.dataGridViewTextBoxColumn_ByFunction_Arrow.HeaderText = "";
			this.dataGridViewTextBoxColumn_ByFunction_Arrow.Name = "dataGridViewTextBoxColumn_ByFunction_Arrow";
			this.dataGridViewTextBoxColumn_ByFunction_Arrow.ReadOnly = true;
			this.dataGridViewTextBoxColumn_ByFunction_Arrow.Width = 21;
			// 
			// dataGridViewTextBoxColumn_ByFunction_Modifier
			// 
			this.dataGridViewTextBoxColumn_ByFunction_Modifier.DataPropertyName = "Modifier";
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
			this.dataGridViewTextBoxColumn_ByFunction_Modifier.DefaultCellStyle = dataGridViewCellStyle2;
			this.dataGridViewTextBoxColumn_ByFunction_Modifier.HeaderText = "Modifier";
			this.dataGridViewTextBoxColumn_ByFunction_Modifier.Name = "dataGridViewTextBoxColumn_ByFunction_Modifier";
			this.dataGridViewTextBoxColumn_ByFunction_Modifier.ReadOnly = true;
			this.dataGridViewTextBoxColumn_ByFunction_Modifier.Width = 21;
			// 
			// dataGridViewTextBoxColumn_ByFunction_Input
			// 
			this.dataGridViewTextBoxColumn_ByFunction_Input.DataPropertyName = "Input";
			this.dataGridViewTextBoxColumn_ByFunction_Input.HeaderText = "Input";
			this.dataGridViewTextBoxColumn_ByFunction_Input.Name = "dataGridViewTextBoxColumn_ByFunction_Input";
			this.dataGridViewTextBoxColumn_ByFunction_Input.ReadOnly = true;
			this.dataGridViewTextBoxColumn_ByFunction_Input.Width = 21;
			// 
			// dataGridViewTextBoxColumn_ByInput_Modifier
			// 
			this.dataGridViewTextBoxColumn_ByInput_Modifier.DataPropertyName = "Modifier";
			dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
			this.dataGridViewTextBoxColumn_ByInput_Modifier.DefaultCellStyle = dataGridViewCellStyle4;
			this.dataGridViewTextBoxColumn_ByInput_Modifier.HeaderText = "Modifier";
			this.dataGridViewTextBoxColumn_ByInput_Modifier.Name = "dataGridViewTextBoxColumn_ByInput_Modifier";
			this.dataGridViewTextBoxColumn_ByInput_Modifier.ReadOnly = true;
			this.dataGridViewTextBoxColumn_ByInput_Modifier.Width = 56;
			// 
			// dataGridViewTextBoxColumn_ByInput_Input
			// 
			this.dataGridViewTextBoxColumn_ByInput_Input.DataPropertyName = "Input";
			this.dataGridViewTextBoxColumn_ByInput_Input.HeaderText = "Input";
			this.dataGridViewTextBoxColumn_ByInput_Input.Name = "dataGridViewTextBoxColumn_ByInput_Input";
			this.dataGridViewTextBoxColumn_ByInput_Input.ReadOnly = true;
			this.dataGridViewTextBoxColumn_ByInput_Input.Width = 21;
			// 
			// dataGridViewTextBoxColumn_ByInput_Arrow
			// 
			this.dataGridViewTextBoxColumn_ByInput_Arrow.DataPropertyName = "Arrow";
			this.dataGridViewTextBoxColumn_ByInput_Arrow.HeaderText = "";
			this.dataGridViewTextBoxColumn_ByInput_Arrow.Name = "dataGridViewTextBoxColumn_ByInput_Arrow";
			this.dataGridViewTextBoxColumn_ByInput_Arrow.ReadOnly = true;
			this.dataGridViewTextBoxColumn_ByInput_Arrow.Width = 21;
			// 
			// dataGridViewTextBoxColumn_ByInput_Function
			// 
			this.dataGridViewTextBoxColumn_ByInput_Function.DataPropertyName = "Function";
			this.dataGridViewTextBoxColumn_ByInput_Function.HeaderText = "Function";
			this.dataGridViewTextBoxColumn_ByInput_Function.Name = "dataGridViewTextBoxColumn_ByInput_Function";
			this.dataGridViewTextBoxColumn_ByInput_Function.ReadOnly = true;
			this.dataGridViewTextBoxColumn_ByInput_Function.Width = 21;
			// 
			// dataGridViewTextBoxColumn_ByInput_Scope
			// 
			this.dataGridViewTextBoxColumn_ByInput_Scope.DataPropertyName = "Scope";
			this.dataGridViewTextBoxColumn_ByInput_Scope.HeaderText = "Scope";
			this.dataGridViewTextBoxColumn_ByInput_Scope.Name = "dataGridViewTextBoxColumn_ByInput_Scope";
			this.dataGridViewTextBoxColumn_ByInput_Scope.ReadOnly = true;
			this.dataGridViewTextBoxColumn_ByInput_Scope.Width = 21;
			// 
			// AutoActionPlotHelp
			// 
			this.AcceptButton = this.button_Close;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button_Close;
			this.ClientSize = new System.Drawing.Size(761, 566);
			this.Controls.Add(this.linkLabel_Remark);
			this.Controls.Add(this.groupBox_ByInput);
			this.Controls.Add(this.button_Close);
			this.Controls.Add(this.groupBox_ByFunction);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AutoActionPlotHelp";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "OxyPlot Interaction Help";
			this.Load += new System.EventHandler(this.AutoActionPlotHelp_Load);
			this.groupBox_ByFunction.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView_ByFunction)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingSource_ByFunction)).EndInit();
			this.groupBox_ByInput.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView_ByInput)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingSource_ByInput)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.GroupBox groupBox_ByFunction;
		private System.Windows.Forms.Button button_Close;
		private System.Windows.Forms.GroupBox groupBox_ByInput;
		private System.Windows.Forms.DataGridView dataGridView_ByInput;
		private System.Windows.Forms.BindingSource bindingSource_ByFunction;
		private System.Windows.Forms.LinkLabel linkLabel_Remark;
		private System.Windows.Forms.DataGridView dataGridView_ByFunction;
		private System.Windows.Forms.BindingSource bindingSource_ByInput;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn_ByFunction_Function;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn_ByFunction_Scope;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn_ByFunction_Arrow;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn_ByFunction_Modifier;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn_ByFunction_Input;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn_ByInput_Modifier;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn_ByInput_Input;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn_ByInput_Arrow;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn_ByInput_Function;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn_ByInput_Scope;
	}
}
