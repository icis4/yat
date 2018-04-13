namespace MKY.Windows.Forms.Test
{
	partial class ListBoxTest
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
			this.button_RemoveLine = new System.Windows.Forms.Button();
			this.button_RemoveManyLines = new System.Windows.Forms.Button();
			this.fastListBox_Normal = new MKY.Windows.Forms.FastListBox();
			this.groupBox_FastListBox = new System.Windows.Forms.GroupBox();
			this.tableLayoutPanel_FastListBox = new System.Windows.Forms.TableLayoutPanel();
			this.panel_FastListBox_OwnerDrawFixed = new System.Windows.Forms.Panel();
			this.label_FastListBox_OwnerDrawFixed_Count = new System.Windows.Forms.Label();
			this.label_FastListBox_OwnerDrawFixed_Updates = new System.Windows.Forms.Label();
			this.fastListBox_OwnerDrawFixed = new MKY.Windows.Forms.FastListBox();
			this.groupBox_ListBoxEx = new System.Windows.Forms.GroupBox();
			this.tableLayoutPanel_ListBoxEx = new System.Windows.Forms.TableLayoutPanel();
			this.panel_ListBoxEx_OwnerDrawFixed = new System.Windows.Forms.Panel();
			this.label_ListBoxEx_OwnerDrawFixed_Count = new System.Windows.Forms.Label();
			this.label_ListBoxEx_OwnerDrawFixed_Updates = new System.Windows.Forms.Label();
			this.panel_ListBoxEx_Normal = new System.Windows.Forms.Panel();
			this.label_ListBoxEx_HorizontalScroll = new System.Windows.Forms.Label();
			this.label_ListBoxEx_VerticalScroll = new System.Windows.Forms.Label();
			this.label_ListBoxEx_VerticalScrollType = new System.Windows.Forms.Label();
			this.label_ListBoxEx_HorizontalScrollPositionOld = new System.Windows.Forms.Label();
			this.label_ListBoxEx_VerticalScrollPositionNew = new System.Windows.Forms.Label();
			this.label_ListBoxEx_HorizontalScrollPositionNew = new System.Windows.Forms.Label();
			this.label_ListBoxEx_VerticalScrollPositionOld = new System.Windows.Forms.Label();
			this.label_ListBoxEx_HorizontalScrollType = new System.Windows.Forms.Label();
			this.listBoxEx_Normal = new MKY.Windows.Forms.ListBoxEx();
			this.listBoxEx_OwnerDrawFixed = new MKY.Windows.Forms.ListBoxEx();
			this.button_AddSomeChars = new System.Windows.Forms.Button();
			this.button_AddManyLines = new System.Windows.Forms.Button();
			this.button_AddLine = new System.Windows.Forms.Button();
			this.button_AddChar = new System.Windows.Forms.Button();
			this.groupBox_ListBox = new System.Windows.Forms.GroupBox();
			this.tableLayoutPanel_ListBox = new System.Windows.Forms.TableLayoutPanel();
			this.listBox_Normal = new System.Windows.Forms.ListBox();
			this.listBox_OwnerDrawFixed = new System.Windows.Forms.ListBox();
			this.panel_ListBox_OwnerDrawFixed = new System.Windows.Forms.Panel();
			this.label_ListBox_OwnerDrawFixed_Count = new System.Windows.Forms.Label();
			this.label_ListBox_OwnerDrawFixed_Updates = new System.Windows.Forms.Label();
			this.button_RemoveAllLines = new System.Windows.Forms.Button();
			this.button_AddSomeLines = new System.Windows.Forms.Button();
			this.groupBox_FastListBox.SuspendLayout();
			this.tableLayoutPanel_FastListBox.SuspendLayout();
			this.panel_FastListBox_OwnerDrawFixed.SuspendLayout();
			this.groupBox_ListBoxEx.SuspendLayout();
			this.tableLayoutPanel_ListBoxEx.SuspendLayout();
			this.panel_ListBoxEx_OwnerDrawFixed.SuspendLayout();
			this.panel_ListBoxEx_Normal.SuspendLayout();
			this.groupBox_ListBox.SuspendLayout();
			this.tableLayoutPanel_ListBox.SuspendLayout();
			this.panel_ListBox_OwnerDrawFixed.SuspendLayout();
			this.SuspendLayout();
			// 
			// button_RemoveLine
			// 
			this.button_RemoveLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_RemoveLine.Location = new System.Drawing.Point(1236, 234);
			this.button_RemoveLine.Name = "button_RemoveLine";
			this.button_RemoveLine.Size = new System.Drawing.Size(100, 23);
			this.button_RemoveLine.TabIndex = 8;
			this.button_RemoveLine.Text = "&Remove";
			this.button_RemoveLine.UseVisualStyleBackColor = true;
			this.button_RemoveLine.Click += new System.EventHandler(this.button_RemoveLine_Click);
			// 
			// button_RemoveManyLines
			// 
			this.button_RemoveManyLines.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_RemoveManyLines.Location = new System.Drawing.Point(1236, 263);
			this.button_RemoveManyLines.Name = "button_RemoveManyLines";
			this.button_RemoveManyLines.Size = new System.Drawing.Size(100, 23);
			this.button_RemoveManyLines.TabIndex = 9;
			this.button_RemoveManyLines.Text = "R&emove Many";
			this.button_RemoveManyLines.UseVisualStyleBackColor = true;
			this.button_RemoveManyLines.Click += new System.EventHandler(this.button_RemoveManyLines_Click);
			// 
			// fastListBox_Normal
			// 
			this.fastListBox_Normal.Dock = System.Windows.Forms.DockStyle.Fill;
			this.fastListBox_Normal.HorizontalScrollbar = true;
			this.fastListBox_Normal.IntegralHeight = false;
			this.fastListBox_Normal.ItemHeight = 16;
			this.fastListBox_Normal.Location = new System.Drawing.Point(3, 3);
			this.fastListBox_Normal.Name = "fastListBox_Normal";
			this.fastListBox_Normal.ScrollAlwaysVisible = true;
			this.fastListBox_Normal.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.fastListBox_Normal.Size = new System.Drawing.Size(327, 300);
			this.fastListBox_Normal.TabIndex = 0;
			// 
			// groupBox_FastListBox
			// 
			this.groupBox_FastListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox_FastListBox.Controls.Add(this.tableLayoutPanel_FastListBox);
			this.groupBox_FastListBox.Location = new System.Drawing.Point(824, 12);
			this.groupBox_FastListBox.Name = "groupBox_FastListBox";
			this.groupBox_FastListBox.Size = new System.Drawing.Size(400, 631);
			this.groupBox_FastListBox.TabIndex = 2;
			this.groupBox_FastListBox.TabStop = false;
			this.groupBox_FastListBox.Text = "FastListBox";
			// 
			// tableLayoutPanel_FastListBox
			// 
			this.tableLayoutPanel_FastListBox.ColumnCount = 2;
			this.tableLayoutPanel_FastListBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 333F));
			this.tableLayoutPanel_FastListBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel_FastListBox.Controls.Add(this.panel_FastListBox_OwnerDrawFixed, 0, 1);
			this.tableLayoutPanel_FastListBox.Controls.Add(this.fastListBox_Normal, 0, 0);
			this.tableLayoutPanel_FastListBox.Controls.Add(this.fastListBox_OwnerDrawFixed, 0, 1);
			this.tableLayoutPanel_FastListBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel_FastListBox.Location = new System.Drawing.Point(3, 16);
			this.tableLayoutPanel_FastListBox.Name = "tableLayoutPanel_FastListBox";
			this.tableLayoutPanel_FastListBox.RowCount = 2;
			this.tableLayoutPanel_FastListBox.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel_FastListBox.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel_FastListBox.Size = new System.Drawing.Size(394, 612);
			this.tableLayoutPanel_FastListBox.TabIndex = 0;
			// 
			// panel_FastListBox_OwnerDrawFixed
			// 
			this.panel_FastListBox_OwnerDrawFixed.Controls.Add(this.label_FastListBox_OwnerDrawFixed_Count);
			this.panel_FastListBox_OwnerDrawFixed.Controls.Add(this.label_FastListBox_OwnerDrawFixed_Updates);
			this.panel_FastListBox_OwnerDrawFixed.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_FastListBox_OwnerDrawFixed.Location = new System.Drawing.Point(336, 309);
			this.panel_FastListBox_OwnerDrawFixed.Name = "panel_FastListBox_OwnerDrawFixed";
			this.panel_FastListBox_OwnerDrawFixed.Size = new System.Drawing.Size(55, 300);
			this.panel_FastListBox_OwnerDrawFixed.TabIndex = 4;
			// 
			// label_FastListBox_OwnerDrawFixed_Count
			// 
			this.label_FastListBox_OwnerDrawFixed_Count.AutoSize = true;
			this.label_FastListBox_OwnerDrawFixed_Count.Location = new System.Drawing.Point(3, 18);
			this.label_FastListBox_OwnerDrawFixed_Count.Name = "label_FastListBox_OwnerDrawFixed_Count";
			this.label_FastListBox_OwnerDrawFixed_Count.Size = new System.Drawing.Size(47, 13);
			this.label_FastListBox_OwnerDrawFixed_Count.TabIndex = 1;
			this.label_FastListBox_OwnerDrawFixed_Count.Text = "<Count>";
			// 
			// label_FastListBox_OwnerDrawFixed_Updates
			// 
			this.label_FastListBox_OwnerDrawFixed_Updates.AutoSize = true;
			this.label_FastListBox_OwnerDrawFixed_Updates.Location = new System.Drawing.Point(3, 5);
			this.label_FastListBox_OwnerDrawFixed_Updates.Name = "label_FastListBox_OwnerDrawFixed_Updates";
			this.label_FastListBox_OwnerDrawFixed_Updates.Size = new System.Drawing.Size(50, 13);
			this.label_FastListBox_OwnerDrawFixed_Updates.TabIndex = 0;
			this.label_FastListBox_OwnerDrawFixed_Updates.Text = "Updates:";
			// 
			// fastListBox_OwnerDrawFixed
			// 
			this.fastListBox_OwnerDrawFixed.Dock = System.Windows.Forms.DockStyle.Fill;
			this.fastListBox_OwnerDrawFixed.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.fastListBox_OwnerDrawFixed.HorizontalScrollbar = true;
			this.fastListBox_OwnerDrawFixed.IntegralHeight = false;
			this.fastListBox_OwnerDrawFixed.Location = new System.Drawing.Point(3, 309);
			this.fastListBox_OwnerDrawFixed.Name = "fastListBox_OwnerDrawFixed";
			this.fastListBox_OwnerDrawFixed.ScrollAlwaysVisible = true;
			this.fastListBox_OwnerDrawFixed.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.fastListBox_OwnerDrawFixed.Size = new System.Drawing.Size(327, 300);
			this.fastListBox_OwnerDrawFixed.TabIndex = 1;
			this.fastListBox_OwnerDrawFixed.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.fastListBox_OwnerDrawFixed_DrawItem);
			// 
			// groupBox_ListBoxEx
			// 
			this.groupBox_ListBoxEx.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox_ListBoxEx.Controls.Add(this.tableLayoutPanel_ListBoxEx);
			this.groupBox_ListBoxEx.Location = new System.Drawing.Point(418, 12);
			this.groupBox_ListBoxEx.Name = "groupBox_ListBoxEx";
			this.groupBox_ListBoxEx.Size = new System.Drawing.Size(400, 631);
			this.groupBox_ListBoxEx.TabIndex = 1;
			this.groupBox_ListBoxEx.TabStop = false;
			this.groupBox_ListBoxEx.Text = "ListBoxEx";
			// 
			// tableLayoutPanel_ListBoxEx
			// 
			this.tableLayoutPanel_ListBoxEx.ColumnCount = 2;
			this.tableLayoutPanel_ListBoxEx.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 333F));
			this.tableLayoutPanel_ListBoxEx.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel_ListBoxEx.Controls.Add(this.panel_ListBoxEx_OwnerDrawFixed, 0, 1);
			this.tableLayoutPanel_ListBoxEx.Controls.Add(this.panel_ListBoxEx_Normal, 1, 0);
			this.tableLayoutPanel_ListBoxEx.Controls.Add(this.listBoxEx_Normal, 0, 0);
			this.tableLayoutPanel_ListBoxEx.Controls.Add(this.listBoxEx_OwnerDrawFixed, 0, 1);
			this.tableLayoutPanel_ListBoxEx.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel_ListBoxEx.Location = new System.Drawing.Point(3, 16);
			this.tableLayoutPanel_ListBoxEx.Name = "tableLayoutPanel_ListBoxEx";
			this.tableLayoutPanel_ListBoxEx.RowCount = 2;
			this.tableLayoutPanel_ListBoxEx.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel_ListBoxEx.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel_ListBoxEx.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel_ListBoxEx.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel_ListBoxEx.Size = new System.Drawing.Size(394, 612);
			this.tableLayoutPanel_ListBoxEx.TabIndex = 0;
			// 
			// panel_ListBoxEx_OwnerDrawFixed
			// 
			this.panel_ListBoxEx_OwnerDrawFixed.Controls.Add(this.label_ListBoxEx_OwnerDrawFixed_Count);
			this.panel_ListBoxEx_OwnerDrawFixed.Controls.Add(this.label_ListBoxEx_OwnerDrawFixed_Updates);
			this.panel_ListBoxEx_OwnerDrawFixed.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_ListBoxEx_OwnerDrawFixed.Location = new System.Drawing.Point(336, 309);
			this.panel_ListBoxEx_OwnerDrawFixed.Name = "panel_ListBoxEx_OwnerDrawFixed";
			this.panel_ListBoxEx_OwnerDrawFixed.Size = new System.Drawing.Size(55, 300);
			this.panel_ListBoxEx_OwnerDrawFixed.TabIndex = 3;
			// 
			// label_ListBoxEx_OwnerDrawFixed_Count
			// 
			this.label_ListBoxEx_OwnerDrawFixed_Count.AutoSize = true;
			this.label_ListBoxEx_OwnerDrawFixed_Count.Location = new System.Drawing.Point(3, 18);
			this.label_ListBoxEx_OwnerDrawFixed_Count.Name = "label_ListBoxEx_OwnerDrawFixed_Count";
			this.label_ListBoxEx_OwnerDrawFixed_Count.Size = new System.Drawing.Size(47, 13);
			this.label_ListBoxEx_OwnerDrawFixed_Count.TabIndex = 1;
			this.label_ListBoxEx_OwnerDrawFixed_Count.Text = "<Count>";
			// 
			// label_ListBoxEx_OwnerDrawFixed_Updates
			// 
			this.label_ListBoxEx_OwnerDrawFixed_Updates.AutoSize = true;
			this.label_ListBoxEx_OwnerDrawFixed_Updates.Location = new System.Drawing.Point(3, 5);
			this.label_ListBoxEx_OwnerDrawFixed_Updates.Name = "label_ListBoxEx_OwnerDrawFixed_Updates";
			this.label_ListBoxEx_OwnerDrawFixed_Updates.Size = new System.Drawing.Size(50, 13);
			this.label_ListBoxEx_OwnerDrawFixed_Updates.TabIndex = 0;
			this.label_ListBoxEx_OwnerDrawFixed_Updates.Text = "Updates:";
			// 
			// panel_ListBoxEx_Normal
			// 
			this.panel_ListBoxEx_Normal.Controls.Add(this.label_ListBoxEx_HorizontalScroll);
			this.panel_ListBoxEx_Normal.Controls.Add(this.label_ListBoxEx_VerticalScroll);
			this.panel_ListBoxEx_Normal.Controls.Add(this.label_ListBoxEx_VerticalScrollType);
			this.panel_ListBoxEx_Normal.Controls.Add(this.label_ListBoxEx_HorizontalScrollPositionOld);
			this.panel_ListBoxEx_Normal.Controls.Add(this.label_ListBoxEx_VerticalScrollPositionNew);
			this.panel_ListBoxEx_Normal.Controls.Add(this.label_ListBoxEx_HorizontalScrollPositionNew);
			this.panel_ListBoxEx_Normal.Controls.Add(this.label_ListBoxEx_VerticalScrollPositionOld);
			this.panel_ListBoxEx_Normal.Controls.Add(this.label_ListBoxEx_HorizontalScrollType);
			this.panel_ListBoxEx_Normal.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_ListBoxEx_Normal.Location = new System.Drawing.Point(336, 3);
			this.panel_ListBoxEx_Normal.Name = "panel_ListBoxEx_Normal";
			this.panel_ListBoxEx_Normal.Size = new System.Drawing.Size(55, 300);
			this.panel_ListBoxEx_Normal.TabIndex = 2;
			// 
			// label_ListBoxEx_HorizontalScroll
			// 
			this.label_ListBoxEx_HorizontalScroll.AutoSize = true;
			this.label_ListBoxEx_HorizontalScroll.Location = new System.Drawing.Point(3, 5);
			this.label_ListBoxEx_HorizontalScroll.Name = "label_ListBoxEx_HorizontalScroll";
			this.label_ListBoxEx_HorizontalScroll.Size = new System.Drawing.Size(47, 13);
			this.label_ListBoxEx_HorizontalScroll.TabIndex = 0;
			this.label_ListBoxEx_HorizontalScroll.Text = "H-Scroll:";
			// 
			// label_ListBoxEx_VerticalScroll
			// 
			this.label_ListBoxEx_VerticalScroll.AutoSize = true;
			this.label_ListBoxEx_VerticalScroll.Location = new System.Drawing.Point(3, 81);
			this.label_ListBoxEx_VerticalScroll.Name = "label_ListBoxEx_VerticalScroll";
			this.label_ListBoxEx_VerticalScroll.Size = new System.Drawing.Size(46, 13);
			this.label_ListBoxEx_VerticalScroll.TabIndex = 4;
			this.label_ListBoxEx_VerticalScroll.Text = "V-Scroll:";
			// 
			// label_ListBoxEx_VerticalScrollType
			// 
			this.label_ListBoxEx_VerticalScrollType.AutoSize = true;
			this.label_ListBoxEx_VerticalScrollType.Location = new System.Drawing.Point(3, 120);
			this.label_ListBoxEx_VerticalScrollType.Name = "label_ListBoxEx_VerticalScrollType";
			this.label_ListBoxEx_VerticalScrollType.Size = new System.Drawing.Size(43, 13);
			this.label_ListBoxEx_VerticalScrollType.TabIndex = 7;
			this.label_ListBoxEx_VerticalScrollType.Text = "<Type>";
			// 
			// label_ListBoxEx_HorizontalScrollPositionOld
			// 
			this.label_ListBoxEx_HorizontalScrollPositionOld.AutoSize = true;
			this.label_ListBoxEx_HorizontalScrollPositionOld.Location = new System.Drawing.Point(3, 18);
			this.label_ListBoxEx_HorizontalScrollPositionOld.Name = "label_ListBoxEx_HorizontalScrollPositionOld";
			this.label_ListBoxEx_HorizontalScrollPositionOld.Size = new System.Drawing.Size(35, 13);
			this.label_ListBoxEx_HorizontalScrollPositionOld.TabIndex = 1;
			this.label_ListBoxEx_HorizontalScrollPositionOld.Text = "<Old>";
			// 
			// label_ListBoxEx_VerticalScrollPositionNew
			// 
			this.label_ListBoxEx_VerticalScrollPositionNew.AutoSize = true;
			this.label_ListBoxEx_VerticalScrollPositionNew.Location = new System.Drawing.Point(3, 107);
			this.label_ListBoxEx_VerticalScrollPositionNew.Name = "label_ListBoxEx_VerticalScrollPositionNew";
			this.label_ListBoxEx_VerticalScrollPositionNew.Size = new System.Drawing.Size(41, 13);
			this.label_ListBoxEx_VerticalScrollPositionNew.TabIndex = 6;
			this.label_ListBoxEx_VerticalScrollPositionNew.Text = "<New>";
			// 
			// label_ListBoxEx_HorizontalScrollPositionNew
			// 
			this.label_ListBoxEx_HorizontalScrollPositionNew.AutoSize = true;
			this.label_ListBoxEx_HorizontalScrollPositionNew.Location = new System.Drawing.Point(3, 31);
			this.label_ListBoxEx_HorizontalScrollPositionNew.Name = "label_ListBoxEx_HorizontalScrollPositionNew";
			this.label_ListBoxEx_HorizontalScrollPositionNew.Size = new System.Drawing.Size(41, 13);
			this.label_ListBoxEx_HorizontalScrollPositionNew.TabIndex = 2;
			this.label_ListBoxEx_HorizontalScrollPositionNew.Text = "<New>";
			// 
			// label_ListBoxEx_VerticalScrollPositionOld
			// 
			this.label_ListBoxEx_VerticalScrollPositionOld.AutoSize = true;
			this.label_ListBoxEx_VerticalScrollPositionOld.Location = new System.Drawing.Point(3, 94);
			this.label_ListBoxEx_VerticalScrollPositionOld.Name = "label_ListBoxEx_VerticalScrollPositionOld";
			this.label_ListBoxEx_VerticalScrollPositionOld.Size = new System.Drawing.Size(35, 13);
			this.label_ListBoxEx_VerticalScrollPositionOld.TabIndex = 5;
			this.label_ListBoxEx_VerticalScrollPositionOld.Text = "<Old>";
			// 
			// label_ListBoxEx_HorizontalScrollType
			// 
			this.label_ListBoxEx_HorizontalScrollType.AutoSize = true;
			this.label_ListBoxEx_HorizontalScrollType.Location = new System.Drawing.Point(3, 44);
			this.label_ListBoxEx_HorizontalScrollType.Name = "label_ListBoxEx_HorizontalScrollType";
			this.label_ListBoxEx_HorizontalScrollType.Size = new System.Drawing.Size(43, 13);
			this.label_ListBoxEx_HorizontalScrollType.TabIndex = 3;
			this.label_ListBoxEx_HorizontalScrollType.Text = "<Type>";
			// 
			// listBoxEx_Normal
			// 
			this.listBoxEx_Normal.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBoxEx_Normal.HorizontalScrollbar = true;
			this.listBoxEx_Normal.IntegralHeight = false;
			this.listBoxEx_Normal.Location = new System.Drawing.Point(3, 3);
			this.listBoxEx_Normal.Name = "listBoxEx_Normal";
			this.listBoxEx_Normal.ScrollAlwaysVisible = true;
			this.listBoxEx_Normal.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listBoxEx_Normal.Size = new System.Drawing.Size(327, 300);
			this.listBoxEx_Normal.TabIndex = 0;
			// 
			// listBoxEx_OwnerDrawFixed
			// 
			this.listBoxEx_OwnerDrawFixed.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBoxEx_OwnerDrawFixed.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.listBoxEx_OwnerDrawFixed.HorizontalScrollbar = true;
			this.listBoxEx_OwnerDrawFixed.IntegralHeight = false;
			this.listBoxEx_OwnerDrawFixed.Location = new System.Drawing.Point(3, 309);
			this.listBoxEx_OwnerDrawFixed.Name = "listBoxEx_OwnerDrawFixed";
			this.listBoxEx_OwnerDrawFixed.ScrollAlwaysVisible = true;
			this.listBoxEx_OwnerDrawFixed.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listBoxEx_OwnerDrawFixed.Size = new System.Drawing.Size(327, 300);
			this.listBoxEx_OwnerDrawFixed.TabIndex = 1;
			this.listBoxEx_OwnerDrawFixed.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBoxEx_OwnerDrawFixed_DrawItem);
			// 
			// button_AddSomeChars
			// 
			this.button_AddSomeChars.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_AddSomeChars.Location = new System.Drawing.Point(1236, 60);
			this.button_AddSomeChars.Name = "button_AddSomeChars";
			this.button_AddSomeChars.Size = new System.Drawing.Size(100, 23);
			this.button_AddSomeChars.TabIndex = 4;
			this.button_AddSomeChars.Text = "Add Some C&hars";
			this.button_AddSomeChars.UseVisualStyleBackColor = true;
			this.button_AddSomeChars.Click += new System.EventHandler(this.button_AddSomeChars_Click);
			// 
			// button_AddManyLines
			// 
			this.button_AddManyLines.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_AddManyLines.Location = new System.Drawing.Point(1236, 176);
			this.button_AddManyLines.Name = "button_AddManyLines";
			this.button_AddManyLines.Size = new System.Drawing.Size(100, 23);
			this.button_AddManyLines.TabIndex = 7;
			this.button_AddManyLines.Text = "Add Many Li&nes";
			this.button_AddManyLines.UseVisualStyleBackColor = true;
			this.button_AddManyLines.Click += new System.EventHandler(this.button_AddManyLines_Click);
			// 
			// button_AddLine
			// 
			this.button_AddLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_AddLine.Location = new System.Drawing.Point(1236, 118);
			this.button_AddLine.Name = "button_AddLine";
			this.button_AddLine.Size = new System.Drawing.Size(100, 23);
			this.button_AddLine.TabIndex = 5;
			this.button_AddLine.Text = "Add &Line";
			this.button_AddLine.UseVisualStyleBackColor = true;
			this.button_AddLine.Click += new System.EventHandler(this.button_AddLine_Click);
			// 
			// button_AddChar
			// 
			this.button_AddChar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_AddChar.Location = new System.Drawing.Point(1236, 31);
			this.button_AddChar.Name = "button_AddChar";
			this.button_AddChar.Size = new System.Drawing.Size(100, 23);
			this.button_AddChar.TabIndex = 3;
			this.button_AddChar.Text = "Add &Char";
			this.button_AddChar.UseVisualStyleBackColor = true;
			this.button_AddChar.Click += new System.EventHandler(this.button_AddChar_Click);
			// 
			// groupBox_ListBox
			// 
			this.groupBox_ListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox_ListBox.Controls.Add(this.tableLayoutPanel_ListBox);
			this.groupBox_ListBox.Location = new System.Drawing.Point(12, 12);
			this.groupBox_ListBox.Name = "groupBox_ListBox";
			this.groupBox_ListBox.Size = new System.Drawing.Size(400, 631);
			this.groupBox_ListBox.TabIndex = 0;
			this.groupBox_ListBox.TabStop = false;
			this.groupBox_ListBox.Text = "ListBox";
			// 
			// tableLayoutPanel_ListBox
			// 
			this.tableLayoutPanel_ListBox.ColumnCount = 2;
			this.tableLayoutPanel_ListBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 333F));
			this.tableLayoutPanel_ListBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel_ListBox.Controls.Add(this.listBox_Normal, 0, 0);
			this.tableLayoutPanel_ListBox.Controls.Add(this.listBox_OwnerDrawFixed, 0, 1);
			this.tableLayoutPanel_ListBox.Controls.Add(this.panel_ListBox_OwnerDrawFixed, 1, 1);
			this.tableLayoutPanel_ListBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel_ListBox.Location = new System.Drawing.Point(3, 16);
			this.tableLayoutPanel_ListBox.Name = "tableLayoutPanel_ListBox";
			this.tableLayoutPanel_ListBox.RowCount = 2;
			this.tableLayoutPanel_ListBox.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel_ListBox.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel_ListBox.Size = new System.Drawing.Size(394, 612);
			this.tableLayoutPanel_ListBox.TabIndex = 0;
			// 
			// listBox_Normal
			// 
			this.listBox_Normal.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBox_Normal.HorizontalScrollbar = true;
			this.listBox_Normal.IntegralHeight = false;
			this.listBox_Normal.Location = new System.Drawing.Point(3, 3);
			this.listBox_Normal.Name = "listBox_Normal";
			this.listBox_Normal.ScrollAlwaysVisible = true;
			this.listBox_Normal.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listBox_Normal.Size = new System.Drawing.Size(327, 300);
			this.listBox_Normal.TabIndex = 0;
			// 
			// listBox_OwnerDrawFixed
			// 
			this.listBox_OwnerDrawFixed.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBox_OwnerDrawFixed.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.listBox_OwnerDrawFixed.HorizontalScrollbar = true;
			this.listBox_OwnerDrawFixed.IntegralHeight = false;
			this.listBox_OwnerDrawFixed.Location = new System.Drawing.Point(3, 309);
			this.listBox_OwnerDrawFixed.Name = "listBox_OwnerDrawFixed";
			this.listBox_OwnerDrawFixed.ScrollAlwaysVisible = true;
			this.listBox_OwnerDrawFixed.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listBox_OwnerDrawFixed.Size = new System.Drawing.Size(327, 300);
			this.listBox_OwnerDrawFixed.TabIndex = 1;
			this.listBox_OwnerDrawFixed.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBox_OwnerDrawFixed_DrawItem);
			// 
			// panel_ListBox_OwnerDrawFixed
			// 
			this.panel_ListBox_OwnerDrawFixed.Controls.Add(this.label_ListBox_OwnerDrawFixed_Count);
			this.panel_ListBox_OwnerDrawFixed.Controls.Add(this.label_ListBox_OwnerDrawFixed_Updates);
			this.panel_ListBox_OwnerDrawFixed.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_ListBox_OwnerDrawFixed.Location = new System.Drawing.Point(336, 309);
			this.panel_ListBox_OwnerDrawFixed.Name = "panel_ListBox_OwnerDrawFixed";
			this.panel_ListBox_OwnerDrawFixed.Size = new System.Drawing.Size(55, 300);
			this.panel_ListBox_OwnerDrawFixed.TabIndex = 2;
			// 
			// label_ListBox_OwnerDrawFixed_Count
			// 
			this.label_ListBox_OwnerDrawFixed_Count.AutoSize = true;
			this.label_ListBox_OwnerDrawFixed_Count.Location = new System.Drawing.Point(3, 18);
			this.label_ListBox_OwnerDrawFixed_Count.Name = "label_ListBox_OwnerDrawFixed_Count";
			this.label_ListBox_OwnerDrawFixed_Count.Size = new System.Drawing.Size(47, 13);
			this.label_ListBox_OwnerDrawFixed_Count.TabIndex = 1;
			this.label_ListBox_OwnerDrawFixed_Count.Text = "<Count>";
			// 
			// label_ListBox_OwnerDrawFixed_Updates
			// 
			this.label_ListBox_OwnerDrawFixed_Updates.AutoSize = true;
			this.label_ListBox_OwnerDrawFixed_Updates.Location = new System.Drawing.Point(3, 5);
			this.label_ListBox_OwnerDrawFixed_Updates.Name = "label_ListBox_OwnerDrawFixed_Updates";
			this.label_ListBox_OwnerDrawFixed_Updates.Size = new System.Drawing.Size(50, 13);
			this.label_ListBox_OwnerDrawFixed_Updates.TabIndex = 0;
			this.label_ListBox_OwnerDrawFixed_Updates.Text = "Updates:";
			// 
			// button_RemoveAllLines
			// 
			this.button_RemoveAllLines.Location = new System.Drawing.Point(1236, 292);
			this.button_RemoveAllLines.Name = "button_RemoveAllLines";
			this.button_RemoveAllLines.Size = new System.Drawing.Size(100, 23);
			this.button_RemoveAllLines.TabIndex = 10;
			this.button_RemoveAllLines.Text = "Re&move All";
			this.button_RemoveAllLines.UseVisualStyleBackColor = true;
			this.button_RemoveAllLines.Click += new System.EventHandler(this.button_RemoveAllLines_Click);
			// 
			// button_AddSomeLines
			// 
			this.button_AddSomeLines.Location = new System.Drawing.Point(1236, 147);
			this.button_AddSomeLines.Name = "button_AddSomeLines";
			this.button_AddSomeLines.Size = new System.Drawing.Size(100, 23);
			this.button_AddSomeLines.TabIndex = 6;
			this.button_AddSomeLines.Text = "Add Some L&ines";
			this.button_AddSomeLines.UseVisualStyleBackColor = true;
			this.button_AddSomeLines.Click += new System.EventHandler(this.button_AddSomeLines_Click);
			// 
			// ListBoxTest
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1348, 655);
			this.Controls.Add(this.button_AddSomeLines);
			this.Controls.Add(this.button_RemoveAllLines);
			this.Controls.Add(this.groupBox_ListBox);
			this.Controls.Add(this.button_AddSomeChars);
			this.Controls.Add(this.button_RemoveManyLines);
			this.Controls.Add(this.groupBox_ListBoxEx);
			this.Controls.Add(this.button_RemoveLine);
			this.Controls.Add(this.groupBox_FastListBox);
			this.Controls.Add(this.button_AddChar);
			this.Controls.Add(this.button_AddLine);
			this.Controls.Add(this.button_AddManyLines);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "ListBoxTest";
			this.Text = "MKY.Windows.Forms.Test";
			this.groupBox_FastListBox.ResumeLayout(false);
			this.tableLayoutPanel_FastListBox.ResumeLayout(false);
			this.panel_FastListBox_OwnerDrawFixed.ResumeLayout(false);
			this.panel_FastListBox_OwnerDrawFixed.PerformLayout();
			this.groupBox_ListBoxEx.ResumeLayout(false);
			this.tableLayoutPanel_ListBoxEx.ResumeLayout(false);
			this.panel_ListBoxEx_OwnerDrawFixed.ResumeLayout(false);
			this.panel_ListBoxEx_OwnerDrawFixed.PerformLayout();
			this.panel_ListBoxEx_Normal.ResumeLayout(false);
			this.panel_ListBoxEx_Normal.PerformLayout();
			this.groupBox_ListBox.ResumeLayout(false);
			this.tableLayoutPanel_ListBox.ResumeLayout(false);
			this.panel_ListBox_OwnerDrawFixed.ResumeLayout(false);
			this.panel_ListBox_OwnerDrawFixed.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private FastListBox fastListBox_Normal;
		private System.Windows.Forms.Button button_RemoveLine;
		private System.Windows.Forms.Button button_RemoveManyLines;
		private System.Windows.Forms.GroupBox groupBox_FastListBox;
		private System.Windows.Forms.GroupBox groupBox_ListBoxEx;
		private ListBoxEx listBoxEx_Normal;
		private System.Windows.Forms.Button button_AddManyLines;
		private System.Windows.Forms.Button button_AddLine;
		private System.Windows.Forms.Button button_AddChar;
		private System.Windows.Forms.Label label_ListBoxEx_VerticalScroll;
		private System.Windows.Forms.Label label_ListBoxEx_HorizontalScroll;
		private System.Windows.Forms.Label label_ListBoxEx_HorizontalScrollPositionOld;
		private System.Windows.Forms.Label label_ListBoxEx_VerticalScrollType;
		private System.Windows.Forms.Label label_ListBoxEx_VerticalScrollPositionNew;
		private System.Windows.Forms.Label label_ListBoxEx_VerticalScrollPositionOld;
		private System.Windows.Forms.Label label_ListBoxEx_HorizontalScrollType;
		private System.Windows.Forms.Label label_ListBoxEx_HorizontalScrollPositionNew;
		private System.Windows.Forms.Button button_AddSomeChars;
		private System.Windows.Forms.GroupBox groupBox_ListBox;
		private System.Windows.Forms.ListBox listBox_Normal;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_ListBox;
		private System.Windows.Forms.ListBox listBox_OwnerDrawFixed;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_ListBoxEx;
		private System.Windows.Forms.Panel panel_ListBoxEx_Normal;
		private ListBoxEx listBoxEx_OwnerDrawFixed;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_FastListBox;
		private FastListBox fastListBox_OwnerDrawFixed;
		private System.Windows.Forms.Button button_RemoveAllLines;
		private System.Windows.Forms.Button button_AddSomeLines;
		private System.Windows.Forms.Panel panel_ListBox_OwnerDrawFixed;
		private System.Windows.Forms.Label label_ListBox_OwnerDrawFixed_Updates;
		private System.Windows.Forms.Label label_ListBox_OwnerDrawFixed_Count;
		private System.Windows.Forms.Panel panel_FastListBox_OwnerDrawFixed;
		private System.Windows.Forms.Label label_FastListBox_OwnerDrawFixed_Count;
		private System.Windows.Forms.Label label_FastListBox_OwnerDrawFixed_Updates;
		private System.Windows.Forms.Panel panel_ListBoxEx_OwnerDrawFixed;
		private System.Windows.Forms.Label label_ListBoxEx_OwnerDrawFixed_Count;
		private System.Windows.Forms.Label label_ListBoxEx_OwnerDrawFixed_Updates;
	}
}

