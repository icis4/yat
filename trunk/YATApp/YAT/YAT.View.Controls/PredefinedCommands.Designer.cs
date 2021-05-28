namespace YAT.View.Controls
{
	partial class PredefinedCommands
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
			this.comboBox_Pages = new System.Windows.Forms.ComboBox();
			this.label_Page = new System.Windows.Forms.Label();
			this.button_PagePrevious = new System.Windows.Forms.Button();
			this.button_PageNext = new System.Windows.Forms.Button();
			this.tableLayoutPanel_All = new System.Windows.Forms.TableLayoutPanel();
			this.panel_Navigation = new System.Windows.Forms.Panel();
			this.tableLayoutPanel_Subpages = new System.Windows.Forms.TableLayoutPanel();
			this.buttonSet_3C = new YAT.View.Controls.PredefinedCommandButtonSet();
			this.buttonSet_3B = new YAT.View.Controls.PredefinedCommandButtonSet();
			this.buttonSet_3A = new YAT.View.Controls.PredefinedCommandButtonSet();
			this.buttonSet_2C = new YAT.View.Controls.PredefinedCommandButtonSet();
			this.buttonSet_2B = new YAT.View.Controls.PredefinedCommandButtonSet();
			this.buttonSet_2A = new YAT.View.Controls.PredefinedCommandButtonSet();
			this.buttonSet_1C = new YAT.View.Controls.PredefinedCommandButtonSet();
			this.buttonSet_1B = new YAT.View.Controls.PredefinedCommandButtonSet();
			this.buttonSet_1A = new YAT.View.Controls.PredefinedCommandButtonSet();
			this.tableLayoutPanel_All.SuspendLayout();
			this.panel_Navigation.SuspendLayout();
			this.tableLayoutPanel_Subpages.SuspendLayout();
			this.SuspendLayout();
			// 
			// comboBox_Pages
			// 
			this.comboBox_Pages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_Pages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_Pages.FormattingEnabled = true;
			this.comboBox_Pages.Location = new System.Drawing.Point(3, 27);
			this.comboBox_Pages.Name = "comboBox_Pages";
			this.comboBox_Pages.Size = new System.Drawing.Size(426, 21);
			this.comboBox_Pages.TabIndex = 3;
			this.comboBox_Pages.SelectedIndexChanged += new System.EventHandler(this.comboBox_Pages_SelectedIndexChanged);
			// 
			// label_Page
			// 
			this.label_Page.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label_Page.Location = new System.Drawing.Point(29, 6);
			this.label_Page.Name = "label_Page";
			this.label_Page.Size = new System.Drawing.Size(374, 17);
			this.label_Page.TabIndex = 1;
			this.label_Page.Text = "Page 99/99";
			this.label_Page.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// button_PagePrevious
			// 
			this.button_PagePrevious.Location = new System.Drawing.Point(3, 3);
			this.button_PagePrevious.Name = "button_PagePrevious";
			this.button_PagePrevious.Size = new System.Drawing.Size(20, 22);
			this.button_PagePrevious.TabIndex = 0;
			this.button_PagePrevious.Text = "<";
			this.button_PagePrevious.UseVisualStyleBackColor = true;
			this.button_PagePrevious.Click += new System.EventHandler(this.button_PagePrevious_Click);
			// 
			// button_PageNext
			// 
			this.button_PageNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_PageNext.Location = new System.Drawing.Point(409, 3);
			this.button_PageNext.Name = "button_PageNext";
			this.button_PageNext.Size = new System.Drawing.Size(20, 22);
			this.button_PageNext.TabIndex = 2;
			this.button_PageNext.Text = ">";
			this.button_PageNext.UseVisualStyleBackColor = true;
			this.button_PageNext.Click += new System.EventHandler(this.button_PageNext_Click);
			// 
			// tableLayoutPanel_All
			// 
			this.tableLayoutPanel_All.ColumnCount = 1;
			this.tableLayoutPanel_All.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel_All.Controls.Add(this.panel_Navigation, 0, 1);
			this.tableLayoutPanel_All.Controls.Add(this.tableLayoutPanel_Subpages, 0, 0);
			this.tableLayoutPanel_All.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel_All.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel_All.Name = "tableLayoutPanel_All";
			this.tableLayoutPanel_All.Padding = new System.Windows.Forms.Padding(0, 14, 0, 0);
			this.tableLayoutPanel_All.RowCount = 2;
			this.tableLayoutPanel_All.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel_All.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
			this.tableLayoutPanel_All.Size = new System.Drawing.Size(432, 715);
			this.tableLayoutPanel_All.TabIndex = 0;
			// 
			// panel_Navigation
			// 
			this.panel_Navigation.Controls.Add(this.comboBox_Pages);
			this.panel_Navigation.Controls.Add(this.button_PagePrevious);
			this.panel_Navigation.Controls.Add(this.label_Page);
			this.panel_Navigation.Controls.Add(this.button_PageNext);
			this.panel_Navigation.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_Navigation.Location = new System.Drawing.Point(0, 663);
			this.panel_Navigation.Margin = new System.Windows.Forms.Padding(0);
			this.panel_Navigation.Name = "panel_Navigation";
			this.panel_Navigation.Size = new System.Drawing.Size(432, 52);
			this.panel_Navigation.TabIndex = 1;
			// 
			// tableLayoutPanel_Subpages
			// 
		////this.tableLayoutPanel_Subpages.ColumnCount = 3;
			this.tableLayoutPanel_Subpages.ColumnCount = 1;
		////this.tableLayoutPanel_Subpages.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
		////this.tableLayoutPanel_Subpages.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
		////this.tableLayoutPanel_Subpages.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel_Subpages.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
		////this.tableLayoutPanel_Subpages.Controls.Add(this.buttonSet_3C, 2, 2);
		////this.tableLayoutPanel_Subpages.Controls.Add(this.buttonSet_3B, 1, 2);
		////this.tableLayoutPanel_Subpages.Controls.Add(this.buttonSet_3A, 0, 2);
		////this.tableLayoutPanel_Subpages.Controls.Add(this.buttonSet_2C, 2, 1);
		////this.tableLayoutPanel_Subpages.Controls.Add(this.buttonSet_2B, 1, 1);
		////this.tableLayoutPanel_Subpages.Controls.Add(this.buttonSet_2A, 0, 1);
		////this.tableLayoutPanel_Subpages.Controls.Add(this.buttonSet_1C, 2, 0);
		////this.tableLayoutPanel_Subpages.Controls.Add(this.buttonSet_1B, 1, 0);
			this.tableLayoutPanel_Subpages.Controls.Add(this.buttonSet_1A, 0, 0);
			this.tableLayoutPanel_Subpages.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel_Subpages.Location = new System.Drawing.Point(3, 17);
			this.tableLayoutPanel_Subpages.Name = "tableLayoutPanel_Subpages";
		////this.tableLayoutPanel_Subpages.RowCount = 3;
			this.tableLayoutPanel_Subpages.RowCount = 1;
		////this.tableLayoutPanel_Subpages.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 292F));
		////this.tableLayoutPanel_Subpages.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 292F));
			this.tableLayoutPanel_Subpages.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel_Subpages.Size = new System.Drawing.Size(426, 643);
			this.tableLayoutPanel_Subpages.TabIndex = 0;
			// 
			// buttonSet_3C
			// 
			this.buttonSet_3C.Dock = System.Windows.Forms.DockStyle.Fill;
			this.buttonSet_3C.Location = new System.Drawing.Point(287, 584);
			this.buttonSet_3C.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
			this.buttonSet_3C.Name = "buttonSet_3C";
			this.buttonSet_3C.Size = new System.Drawing.Size(136, 59);
			this.buttonSet_3C.SubpageId = 9;
			this.buttonSet_3C.TabIndex = 8;
			this.buttonSet_3C.Visible = false;
			this.buttonSet_3C.SendCommandRequest += new System.EventHandler<YAT.Model.Types.PredefinedCommandEventArgs>(this.buttonSet_I_SendCommandRequest);
			this.buttonSet_3C.DefineCommandRequest += new System.EventHandler<YAT.Model.Types.PredefinedCommandEventArgs>(this.buttonSet_I_DefineCommandRequest);
			// 
			// buttonSet_3B
			// 
			this.buttonSet_3B.Dock = System.Windows.Forms.DockStyle.Fill;
			this.buttonSet_3B.Location = new System.Drawing.Point(145, 584);
			this.buttonSet_3B.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
			this.buttonSet_3B.Name = "buttonSet_3B";
			this.buttonSet_3B.Size = new System.Drawing.Size(136, 59);
			this.buttonSet_3B.SubpageId = 6;
			this.buttonSet_3B.TabIndex = 5;
			this.buttonSet_3B.Visible = false;
			this.buttonSet_3B.SendCommandRequest += new System.EventHandler<YAT.Model.Types.PredefinedCommandEventArgs>(this.buttonSet_I_SendCommandRequest);
			this.buttonSet_3B.DefineCommandRequest += new System.EventHandler<YAT.Model.Types.PredefinedCommandEventArgs>(this.buttonSet_I_DefineCommandRequest);
			// 
			// buttonSet_3A
			// 
			this.buttonSet_3A.Dock = System.Windows.Forms.DockStyle.Fill;
			this.buttonSet_3A.Location = new System.Drawing.Point(3, 584);
			this.buttonSet_3A.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
			this.buttonSet_3A.Name = "buttonSet_3A";
			this.buttonSet_3A.Size = new System.Drawing.Size(136, 59);
			this.buttonSet_3A.SubpageId = 3;
			this.buttonSet_3A.TabIndex = 2;
			this.buttonSet_3A.Visible = false;
			this.buttonSet_3A.SendCommandRequest += new System.EventHandler<YAT.Model.Types.PredefinedCommandEventArgs>(this.buttonSet_I_SendCommandRequest);
			this.buttonSet_3A.DefineCommandRequest += new System.EventHandler<YAT.Model.Types.PredefinedCommandEventArgs>(this.buttonSet_I_DefineCommandRequest);
			// 
			// buttonSet_2C
			// 
			this.buttonSet_2C.Dock = System.Windows.Forms.DockStyle.Fill;
			this.buttonSet_2C.Location = new System.Drawing.Point(287, 292);
			this.buttonSet_2C.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
			this.buttonSet_2C.Name = "buttonSet_2C";
			this.buttonSet_2C.Size = new System.Drawing.Size(136, 292);
			this.buttonSet_2C.SubpageId = 8;
			this.buttonSet_2C.TabIndex = 7;
			this.buttonSet_2C.Visible = false;
			this.buttonSet_2C.SendCommandRequest += new System.EventHandler<YAT.Model.Types.PredefinedCommandEventArgs>(this.buttonSet_I_SendCommandRequest);
			this.buttonSet_2C.DefineCommandRequest += new System.EventHandler<YAT.Model.Types.PredefinedCommandEventArgs>(this.buttonSet_I_DefineCommandRequest);
			// 
			// buttonSet_2B
			// 
			this.buttonSet_2B.Dock = System.Windows.Forms.DockStyle.Fill;
			this.buttonSet_2B.Location = new System.Drawing.Point(145, 292);
			this.buttonSet_2B.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
			this.buttonSet_2B.Name = "buttonSet_2B";
			this.buttonSet_2B.Size = new System.Drawing.Size(136, 292);
			this.buttonSet_2B.SubpageId = 5;
			this.buttonSet_2B.TabIndex = 4;
			this.buttonSet_2B.Visible = false;
			this.buttonSet_2B.SendCommandRequest += new System.EventHandler<YAT.Model.Types.PredefinedCommandEventArgs>(this.buttonSet_I_SendCommandRequest);
			this.buttonSet_2B.DefineCommandRequest += new System.EventHandler<YAT.Model.Types.PredefinedCommandEventArgs>(this.buttonSet_I_DefineCommandRequest);
			// 
			// buttonSet_2A
			// 
			this.buttonSet_2A.Dock = System.Windows.Forms.DockStyle.Fill;
			this.buttonSet_2A.Location = new System.Drawing.Point(3, 292);
			this.buttonSet_2A.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
			this.buttonSet_2A.Name = "buttonSet_2A";
			this.buttonSet_2A.Size = new System.Drawing.Size(136, 292);
			this.buttonSet_2A.SubpageId = 2;
			this.buttonSet_2A.TabIndex = 1;
			this.buttonSet_2A.Visible = false;
			this.buttonSet_2A.SendCommandRequest += new System.EventHandler<YAT.Model.Types.PredefinedCommandEventArgs>(this.buttonSet_I_SendCommandRequest);
			this.buttonSet_2A.DefineCommandRequest += new System.EventHandler<YAT.Model.Types.PredefinedCommandEventArgs>(this.buttonSet_I_DefineCommandRequest);
			// 
			// buttonSet_1C
			// 
			this.buttonSet_1C.Dock = System.Windows.Forms.DockStyle.Fill;
			this.buttonSet_1C.Location = new System.Drawing.Point(287, 0);
			this.buttonSet_1C.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
			this.buttonSet_1C.Name = "buttonSet_1C";
			this.buttonSet_1C.Size = new System.Drawing.Size(136, 292);
			this.buttonSet_1C.SubpageId = 7;
			this.buttonSet_1C.TabIndex = 6;
			this.buttonSet_1C.Visible = false;
			this.buttonSet_1C.SendCommandRequest += new System.EventHandler<YAT.Model.Types.PredefinedCommandEventArgs>(this.buttonSet_I_SendCommandRequest);
			this.buttonSet_1C.DefineCommandRequest += new System.EventHandler<YAT.Model.Types.PredefinedCommandEventArgs>(this.buttonSet_I_DefineCommandRequest);
			// 
			// buttonSet_1B
			// 
			this.buttonSet_1B.Dock = System.Windows.Forms.DockStyle.Fill;
			this.buttonSet_1B.Location = new System.Drawing.Point(145, 0);
			this.buttonSet_1B.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
			this.buttonSet_1B.Name = "buttonSet_1B";
			this.buttonSet_1B.Size = new System.Drawing.Size(136, 292);
			this.buttonSet_1B.SubpageId = 4;
			this.buttonSet_1B.TabIndex = 3;
			this.buttonSet_1B.Visible = false;
			this.buttonSet_1B.SendCommandRequest += new System.EventHandler<YAT.Model.Types.PredefinedCommandEventArgs>(this.buttonSet_I_SendCommandRequest);
			this.buttonSet_1B.DefineCommandRequest += new System.EventHandler<YAT.Model.Types.PredefinedCommandEventArgs>(this.buttonSet_I_DefineCommandRequest);
			// 
			// buttonSet_1A
			// 
			this.buttonSet_1A.Dock = System.Windows.Forms.DockStyle.Fill;
			this.buttonSet_1A.Location = new System.Drawing.Point(3, 0);
			this.buttonSet_1A.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
			this.buttonSet_1A.Name = "buttonSet_1A";
			this.buttonSet_1A.Size = new System.Drawing.Size(136, 292);
			this.buttonSet_1A.TabIndex = 0;
			this.buttonSet_1A.SendCommandRequest += new System.EventHandler<YAT.Model.Types.PredefinedCommandEventArgs>(this.buttonSet_I_SendCommandRequest);
			this.buttonSet_1A.DefineCommandRequest += new System.EventHandler<YAT.Model.Types.PredefinedCommandEventArgs>(this.buttonSet_I_DefineCommandRequest);
			// 
			// PredefinedCommands
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.Controls.Add(this.tableLayoutPanel_All);
			this.Name = "PredefinedCommands";
			this.Size = new System.Drawing.Size(432, 715);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.PredefinedCommands_Paint);
			this.tableLayoutPanel_All.ResumeLayout(false);
			this.panel_Navigation.ResumeLayout(false);
			this.tableLayoutPanel_Subpages.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.ComboBox comboBox_Pages;
		private System.Windows.Forms.Button button_PageNext;
		private System.Windows.Forms.Button button_PagePrevious;
		private System.Windows.Forms.Label label_Page;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_All;
		private System.Windows.Forms.Panel panel_Navigation;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Subpages;
		private PredefinedCommandButtonSet buttonSet_1A;
		private PredefinedCommandButtonSet buttonSet_1B;
		private PredefinedCommandButtonSet buttonSet_1C;
		private PredefinedCommandButtonSet buttonSet_2A;
		private PredefinedCommandButtonSet buttonSet_2B;
		private PredefinedCommandButtonSet buttonSet_2C;
		private PredefinedCommandButtonSet buttonSet_3A;
		private PredefinedCommandButtonSet buttonSet_3B;
		private PredefinedCommandButtonSet buttonSet_3C;
	}
}
