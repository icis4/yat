namespace YAT.Gui.Forms
{
	partial class AdvancedTerminalSettings
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
			this.button_Defaults = new System.Windows.Forms.Button();
			this.button_Cancel = new System.Windows.Forms.Button();
			this.button_OK = new System.Windows.Forms.Button();
			this.groupBox_Settings = new System.Windows.Forms.GroupBox();
			this.groupBox_User = new System.Windows.Forms.GroupBox();
			this.textBox_UserName = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox_Communication = new System.Windows.Forms.GroupBox();
			this.groupBox_Communication_Break = new System.Windows.Forms.GroupBox();
			this.checkBox_OutputBreakModifiable = new System.Windows.Forms.CheckBox();
			this.checkBox_IndicateBreakStates = new System.Windows.Forms.CheckBox();
			this.comboBox_Endianness = new System.Windows.Forms.ComboBox();
			this.label_Endianness = new System.Windows.Forms.Label();
			this.groupBox_Send = new System.Windows.Forms.GroupBox();
			this.checkBox_NoSendOnInputBreak = new System.Windows.Forms.CheckBox();
			this.label_MaxSendChunkSizeUnit = new System.Windows.Forms.Label();
			this.label_MaxSendChunkSizeRemark = new System.Windows.Forms.Label();
			this.textBox_MaxSendChunkSize = new System.Windows.Forms.TextBox();
			this.label_MaxSendChunkSize = new System.Windows.Forms.Label();
			this.checkBox_NoSendOnOutputBreak = new System.Windows.Forms.CheckBox();
			this.label_SendImmediately = new System.Windows.Forms.Label();
			this.checkBox_SendImmediately = new System.Windows.Forms.CheckBox();
			this.checkBox_CopyPredefined = new System.Windows.Forms.CheckBox();
			this.checkBox_KeepCommand = new System.Windows.Forms.CheckBox();
			this.groupBox_Display = new System.Windows.Forms.GroupBox();
			this.groupBox_Display_XOnXOff = new System.Windows.Forms.GroupBox();
			this.checkBox_HideXOnXOff = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowBreakCount = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowFlowControlCount = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowLineNumbers = new System.Windows.Forms.CheckBox();
			this.groupBox_Display_Tab = new System.Windows.Forms.GroupBox();
			this.label_ReplaceTab = new System.Windows.Forms.Label();
			this.checkBox_ReplaceTab = new System.Windows.Forms.CheckBox();
			this.groupBox_Display_Space = new System.Windows.Forms.GroupBox();
			this.label_SpaceReplacementChar = new System.Windows.Forms.Label();
			this.checkBox_ReplaceSpace = new System.Windows.Forms.CheckBox();
			this.groupBox_Display_ControlChars = new System.Windows.Forms.GroupBox();
			this.comboBox_ControlCharacterRadix = new System.Windows.Forms.ComboBox();
			this.label_ControlCharacterRadix = new System.Windows.Forms.Label();
			this.checkBox_ReplaceControlCharacters = new System.Windows.Forms.CheckBox();
			this.checkBox_DirectionLineBreak = new System.Windows.Forms.CheckBox();
			this.comboBox_RxRadix = new System.Windows.Forms.ComboBox();
			this.label_RxRadix = new System.Windows.Forms.Label();
			this.checkBox_SeparateTxRxRadix = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowRadix = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowTimeStamp = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowConnectTime = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowCountAndRate = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowLength = new System.Windows.Forms.CheckBox();
			this.comboBox_TxRadix = new System.Windows.Forms.ComboBox();
			this.label_TxRadix = new System.Windows.Forms.Label();
			this.label_MaxLineCountUnit = new System.Windows.Forms.Label();
			this.textBox_MaxLineCount = new System.Windows.Forms.TextBox();
			this.label_MaxLineCount = new System.Windows.Forms.Label();
			this.checkBox_DisableKeywords = new System.Windows.Forms.CheckBox();
			this.groupBox_Send_Keywords = new System.Windows.Forms.GroupBox();
			this.label_DefaultLineDelayUnit = new System.Windows.Forms.Label();
			this.textBox_DefaultLineDelay = new System.Windows.Forms.TextBox();
			this.label_DefaultLineDelay = new System.Windows.Forms.Label();
			this.label_DefaultDelayUnit = new System.Windows.Forms.Label();
			this.textBox_DefaultDelay = new System.Windows.Forms.TextBox();
			this.label_DefaultDelay = new System.Windows.Forms.Label();
			this.groupBox_Settings.SuspendLayout();
			this.groupBox_User.SuspendLayout();
			this.groupBox_Communication.SuspendLayout();
			this.groupBox_Communication_Break.SuspendLayout();
			this.groupBox_Send.SuspendLayout();
			this.groupBox_Display.SuspendLayout();
			this.groupBox_Display_XOnXOff.SuspendLayout();
			this.groupBox_Display_Tab.SuspendLayout();
			this.groupBox_Display_Space.SuspendLayout();
			this.groupBox_Display_ControlChars.SuspendLayout();
			this.groupBox_Send_Keywords.SuspendLayout();
			this.SuspendLayout();
			// 
			// button_Defaults
			// 
			this.button_Defaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Defaults.Location = new System.Drawing.Point(568, 136);
			this.button_Defaults.Name = "button_Defaults";
			this.button_Defaults.Size = new System.Drawing.Size(75, 23);
			this.button_Defaults.TabIndex = 3;
			this.button_Defaults.Text = "&Defaults...";
			this.button_Defaults.Click += new System.EventHandler(this.button_Defaults_Click);
			// 
			// button_Cancel
			// 
			this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button_Cancel.Location = new System.Drawing.Point(568, 72);
			this.button_Cancel.Name = "button_Cancel";
			this.button_Cancel.Size = new System.Drawing.Size(75, 23);
			this.button_Cancel.TabIndex = 2;
			this.button_Cancel.Text = "Cancel";
			this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
			// 
			// button_OK
			// 
			this.button_OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button_OK.Location = new System.Drawing.Point(568, 43);
			this.button_OK.Name = "button_OK";
			this.button_OK.Size = new System.Drawing.Size(75, 23);
			this.button_OK.TabIndex = 1;
			this.button_OK.Text = "OK";
			this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
			// 
			// groupBox_Settings
			// 
			this.groupBox_Settings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Settings.Controls.Add(this.groupBox_User);
			this.groupBox_Settings.Controls.Add(this.groupBox_Communication);
			this.groupBox_Settings.Controls.Add(this.groupBox_Send);
			this.groupBox_Settings.Controls.Add(this.groupBox_Display);
			this.groupBox_Settings.Location = new System.Drawing.Point(12, 12);
			this.groupBox_Settings.Name = "groupBox_Settings";
			this.groupBox_Settings.Size = new System.Drawing.Size(544, 489);
			this.groupBox_Settings.TabIndex = 0;
			this.groupBox_Settings.TabStop = false;
			// 
			// groupBox_User
			// 
			this.groupBox_User.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_User.Controls.Add(this.textBox_UserName);
			this.groupBox_User.Controls.Add(this.label2);
			this.groupBox_User.Location = new System.Drawing.Point(275, 431);
			this.groupBox_User.Name = "groupBox_User";
			this.groupBox_User.Size = new System.Drawing.Size(263, 52);
			this.groupBox_User.TabIndex = 4;
			this.groupBox_User.TabStop = false;
			this.groupBox_User.Text = "User";
			// 
			// textBox_UserName
			// 
			this.textBox_UserName.Location = new System.Drawing.Point(94, 20);
			this.textBox_UserName.Name = "textBox_UserName";
			this.textBox_UserName.Size = new System.Drawing.Size(157, 20);
			this.textBox_UserName.TabIndex = 1;
			this.textBox_UserName.TextChanged += new System.EventHandler(this.textBox_UserName_TextChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(9, 23);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(79, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Terminal &name:";
			// 
			// groupBox_Communication
			// 
			this.groupBox_Communication.Controls.Add(this.groupBox_Communication_Break);
			this.groupBox_Communication.Controls.Add(this.comboBox_Endianness);
			this.groupBox_Communication.Controls.Add(this.label_Endianness);
			this.groupBox_Communication.Location = new System.Drawing.Point(275, 13);
			this.groupBox_Communication.Name = "groupBox_Communication";
			this.groupBox_Communication.Size = new System.Drawing.Size(263, 118);
			this.groupBox_Communication.TabIndex = 1;
			this.groupBox_Communication.TabStop = false;
			this.groupBox_Communication.Text = "Communication Settings";
			// 
			// groupBox_Communication_Break
			// 
			this.groupBox_Communication_Break.Controls.Add(this.checkBox_OutputBreakModifiable);
			this.groupBox_Communication_Break.Controls.Add(this.checkBox_IndicateBreakStates);
			this.groupBox_Communication_Break.Location = new System.Drawing.Point(6, 45);
			this.groupBox_Communication_Break.Name = "groupBox_Communication_Break";
			this.groupBox_Communication_Break.Size = new System.Drawing.Size(251, 67);
			this.groupBox_Communication_Break.TabIndex = 2;
			this.groupBox_Communication_Break.TabStop = false;
			this.groupBox_Communication_Break.Text = "Brea&k (Applies to serial COM ports only)";
			// 
			// checkBox_OutputBreakModifiable
			// 
			this.checkBox_OutputBreakModifiable.AutoSize = true;
			this.checkBox_OutputBreakModifiable.Location = new System.Drawing.Point(6, 42);
			this.checkBox_OutputBreakModifiable.Name = "checkBox_OutputBreakModifiable";
			this.checkBox_OutputBreakModifiable.Size = new System.Drawing.Size(192, 17);
			this.checkBox_OutputBreakModifiable.TabIndex = 1;
			this.checkBox_OutputBreakModifiable.Text = "Output break state can be modified";
			this.checkBox_OutputBreakModifiable.UseVisualStyleBackColor = true;
			this.checkBox_OutputBreakModifiable.CheckedChanged += new System.EventHandler(this.checkBox_OutputBreakModifiable_CheckedChanged);
			// 
			// checkBox_IndicateBreakStates
			// 
			this.checkBox_IndicateBreakStates.AutoSize = true;
			this.checkBox_IndicateBreakStates.Location = new System.Drawing.Point(6, 19);
			this.checkBox_IndicateBreakStates.Name = "checkBox_IndicateBreakStates";
			this.checkBox_IndicateBreakStates.Size = new System.Drawing.Size(125, 17);
			this.checkBox_IndicateBreakStates.TabIndex = 0;
			this.checkBox_IndicateBreakStates.Text = "Indicate break states";
			this.checkBox_IndicateBreakStates.UseVisualStyleBackColor = true;
			this.checkBox_IndicateBreakStates.CheckedChanged += new System.EventHandler(this.checkBox_IndicateBreakStates_CheckedChanged);
			// 
			// comboBox_Endianness
			// 
			this.comboBox_Endianness.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_Endianness.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_Endianness.Location = new System.Drawing.Point(70, 18);
			this.comboBox_Endianness.Name = "comboBox_Endianness";
			this.comboBox_Endianness.Size = new System.Drawing.Size(182, 21);
			this.comboBox_Endianness.TabIndex = 1;
			this.comboBox_Endianness.SelectedIndexChanged += new System.EventHandler(this.comboBox_Endianness_SelectedIndexChanged);
			// 
			// label_Endianness
			// 
			this.label_Endianness.AutoSize = true;
			this.label_Endianness.Location = new System.Drawing.Point(9, 21);
			this.label_Endianness.Name = "label_Endianness";
			this.label_Endianness.Size = new System.Drawing.Size(65, 13);
			this.label_Endianness.TabIndex = 0;
			this.label_Endianness.Text = "&Endianness:";
			// 
			// groupBox_Send
			// 
			this.groupBox_Send.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Send.Controls.Add(this.groupBox_Send_Keywords);
			this.groupBox_Send.Controls.Add(this.checkBox_NoSendOnInputBreak);
			this.groupBox_Send.Controls.Add(this.label_MaxSendChunkSizeUnit);
			this.groupBox_Send.Controls.Add(this.label_MaxSendChunkSizeRemark);
			this.groupBox_Send.Controls.Add(this.textBox_MaxSendChunkSize);
			this.groupBox_Send.Controls.Add(this.label_MaxSendChunkSize);
			this.groupBox_Send.Controls.Add(this.checkBox_NoSendOnOutputBreak);
			this.groupBox_Send.Controls.Add(this.label_SendImmediately);
			this.groupBox_Send.Controls.Add(this.checkBox_SendImmediately);
			this.groupBox_Send.Controls.Add(this.checkBox_CopyPredefined);
			this.groupBox_Send.Controls.Add(this.checkBox_KeepCommand);
			this.groupBox_Send.Location = new System.Drawing.Point(275, 137);
			this.groupBox_Send.Name = "groupBox_Send";
			this.groupBox_Send.Size = new System.Drawing.Size(263, 288);
			this.groupBox_Send.TabIndex = 2;
			this.groupBox_Send.TabStop = false;
			this.groupBox_Send.Text = "Send Settings";
			// 
			// checkBox_NoSendOnInputBreak
			// 
			this.checkBox_NoSendOnInputBreak.AutoSize = true;
			this.checkBox_NoSendOnInputBreak.Location = new System.Drawing.Point(12, 165);
			this.checkBox_NoSendOnInputBreak.Name = "checkBox_NoSendOnInputBreak";
			this.checkBox_NoSendOnInputBreak.Size = new System.Drawing.Size(212, 17);
			this.checkBox_NoSendOnInputBreak.TabIndex = 9;
			this.checkBox_NoSendOnInputBreak.Text = "No send w&hile in input break state (IBS)";
			this.checkBox_NoSendOnInputBreak.UseVisualStyleBackColor = true;
			this.checkBox_NoSendOnInputBreak.CheckedChanged += new System.EventHandler(this.checkBox_NoSendOnInputBreak_CheckedChanged);
			// 
			// label_MaxSendChunkSizeUnit
			// 
			this.label_MaxSendChunkSizeUnit.AutoSize = true;
			this.label_MaxSendChunkSizeUnit.Location = new System.Drawing.Point(202, 119);
			this.label_MaxSendChunkSizeUnit.Name = "label_MaxSendChunkSizeUnit";
			this.label_MaxSendChunkSizeUnit.Size = new System.Drawing.Size(32, 13);
			this.label_MaxSendChunkSizeUnit.TabIndex = 7;
			this.label_MaxSendChunkSizeUnit.Text = "bytes";
			this.label_MaxSendChunkSizeUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label_MaxSendChunkSizeRemark
			// 
			this.label_MaxSendChunkSizeRemark.AutoSize = true;
			this.label_MaxSendChunkSizeRemark.Location = new System.Drawing.Point(9, 106);
			this.label_MaxSendChunkSizeRemark.Name = "label_MaxSendChunkSizeRemark";
			this.label_MaxSendChunkSizeRemark.Size = new System.Drawing.Size(110, 13);
			this.label_MaxSendChunkSizeRemark.TabIndex = 4;
			this.label_MaxSendChunkSizeRemark.Text = "In case of serial ports,";
			// 
			// textBox_MaxSendChunkSize
			// 
			this.textBox_MaxSendChunkSize.Location = new System.Drawing.Point(150, 116);
			this.textBox_MaxSendChunkSize.Name = "textBox_MaxSendChunkSize";
			this.textBox_MaxSendChunkSize.Size = new System.Drawing.Size(50, 20);
			this.textBox_MaxSendChunkSize.TabIndex = 6;
			this.textBox_MaxSendChunkSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBox_MaxSendChunkSize.TextChanged += new System.EventHandler(this.textBox_MaxSendChunkSize_TextChanged);
			this.textBox_MaxSendChunkSize.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_MaxSendChunkSize_Validating);
			// 
			// label_MaxSendChunkSize
			// 
			this.label_MaxSendChunkSize.AutoSize = true;
			this.label_MaxSendChunkSize.Location = new System.Drawing.Point(9, 119);
			this.label_MaxSendChunkSize.Name = "label_MaxSendChunkSize";
			this.label_MaxSendChunkSize.Size = new System.Drawing.Size(140, 13);
			this.label_MaxSendChunkSize.TabIndex = 5;
			this.label_MaxSendChunkSize.Text = "send data in &chunks of max.";
			// 
			// checkBox_NoSendOnOutputBreak
			// 
			this.checkBox_NoSendOnOutputBreak.AutoSize = true;
			this.checkBox_NoSendOnOutputBreak.Location = new System.Drawing.Point(12, 142);
			this.checkBox_NoSendOnOutputBreak.Name = "checkBox_NoSendOnOutputBreak";
			this.checkBox_NoSendOnOutputBreak.Size = new System.Drawing.Size(224, 17);
			this.checkBox_NoSendOnOutputBreak.TabIndex = 8;
			this.checkBox_NoSendOnOutputBreak.Text = "No send while in o&utput break state (OBS)";
			this.checkBox_NoSendOnOutputBreak.UseVisualStyleBackColor = true;
			this.checkBox_NoSendOnOutputBreak.CheckedChanged += new System.EventHandler(this.checkBox_NoSendOnOutputBreak_CheckedChanged);
			// 
			// label_SendImmediately
			// 
			this.label_SendImmediately.AutoSize = true;
			this.label_SendImmediately.Location = new System.Drawing.Point(28, 82);
			this.label_SendImmediately.Name = "label_SendImmediately";
			this.label_SendImmediately.Size = new System.Drawing.Size(164, 13);
			this.label_SendImmediately.TabIndex = 3;
			this.label_SendImmediately.Text = "(Emulates a terminal/direct mode)";
			// 
			// checkBox_SendImmediately
			// 
			this.checkBox_SendImmediately.AutoSize = true;
			this.checkBox_SendImmediately.Location = new System.Drawing.Point(12, 67);
			this.checkBox_SendImmediately.Name = "checkBox_SendImmediately";
			this.checkBox_SendImmediately.Size = new System.Drawing.Size(222, 17);
			this.checkBox_SendImmediately.TabIndex = 2;
			this.checkBox_SendImmediately.Text = "Send each entered character &immediately";
			this.checkBox_SendImmediately.UseVisualStyleBackColor = true;
			this.checkBox_SendImmediately.CheckedChanged += new System.EventHandler(this.checkBox_SendImmediately_CheckedChanged);
			// 
			// checkBox_CopyPredefined
			// 
			this.checkBox_CopyPredefined.AutoSize = true;
			this.checkBox_CopyPredefined.Location = new System.Drawing.Point(12, 44);
			this.checkBox_CopyPredefined.Name = "checkBox_CopyPredefined";
			this.checkBox_CopyPredefined.Size = new System.Drawing.Size(240, 17);
			this.checkBox_CopyPredefined.TabIndex = 1;
			this.checkBox_CopyPredefined.Text = "Cop&y predefined to send command after send";
			this.checkBox_CopyPredefined.UseVisualStyleBackColor = true;
			this.checkBox_CopyPredefined.CheckedChanged += new System.EventHandler(this.checkBox_CopyPredefined_CheckedChanged);
			// 
			// checkBox_KeepCommand
			// 
			this.checkBox_KeepCommand.AutoSize = true;
			this.checkBox_KeepCommand.Location = new System.Drawing.Point(12, 21);
			this.checkBox_KeepCommand.Name = "checkBox_KeepCommand";
			this.checkBox_KeepCommand.Size = new System.Drawing.Size(201, 17);
			this.checkBox_KeepCommand.TabIndex = 0;
			this.checkBox_KeepCommand.Text = "&Keep command in text box after send";
			this.checkBox_KeepCommand.UseVisualStyleBackColor = true;
			this.checkBox_KeepCommand.CheckedChanged += new System.EventHandler(this.checkBox_KeepCommand_CheckedChanged);
			// 
			// groupBox_Display
			// 
			this.groupBox_Display.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox_Display.Controls.Add(this.groupBox_Display_XOnXOff);
			this.groupBox_Display.Controls.Add(this.checkBox_ShowBreakCount);
			this.groupBox_Display.Controls.Add(this.checkBox_ShowFlowControlCount);
			this.groupBox_Display.Controls.Add(this.checkBox_ShowLineNumbers);
			this.groupBox_Display.Controls.Add(this.groupBox_Display_Tab);
			this.groupBox_Display.Controls.Add(this.groupBox_Display_Space);
			this.groupBox_Display.Controls.Add(this.groupBox_Display_ControlChars);
			this.groupBox_Display.Controls.Add(this.checkBox_DirectionLineBreak);
			this.groupBox_Display.Controls.Add(this.comboBox_RxRadix);
			this.groupBox_Display.Controls.Add(this.label_RxRadix);
			this.groupBox_Display.Controls.Add(this.checkBox_SeparateTxRxRadix);
			this.groupBox_Display.Controls.Add(this.checkBox_ShowRadix);
			this.groupBox_Display.Controls.Add(this.checkBox_ShowTimeStamp);
			this.groupBox_Display.Controls.Add(this.checkBox_ShowConnectTime);
			this.groupBox_Display.Controls.Add(this.checkBox_ShowCountAndRate);
			this.groupBox_Display.Controls.Add(this.checkBox_ShowLength);
			this.groupBox_Display.Controls.Add(this.comboBox_TxRadix);
			this.groupBox_Display.Controls.Add(this.label_TxRadix);
			this.groupBox_Display.Controls.Add(this.label_MaxLineCountUnit);
			this.groupBox_Display.Controls.Add(this.textBox_MaxLineCount);
			this.groupBox_Display.Controls.Add(this.label_MaxLineCount);
			this.groupBox_Display.Location = new System.Drawing.Point(6, 13);
			this.groupBox_Display.Name = "groupBox_Display";
			this.groupBox_Display.Size = new System.Drawing.Size(263, 470);
			this.groupBox_Display.TabIndex = 0;
			this.groupBox_Display.TabStop = false;
			this.groupBox_Display.Text = "Display Settings";
			// 
			// groupBox_Display_XOnXOff
			// 
			this.groupBox_Display_XOnXOff.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Display_XOnXOff.Controls.Add(this.checkBox_HideXOnXOff);
			this.groupBox_Display_XOnXOff.Location = new System.Drawing.Point(6, 418);
			this.groupBox_Display_XOnXOff.Name = "groupBox_Display_XOnXOff";
			this.groupBox_Display_XOnXOff.Size = new System.Drawing.Size(251, 46);
			this.groupBox_Display_XOnXOff.TabIndex = 20;
			this.groupBox_Display_XOnXOff.TabStop = false;
			this.groupBox_Display_XOnXOff.Text = "XOn/XOff";
			// 
			// checkBox_HideXOnXOff
			// 
			this.checkBox_HideXOnXOff.AutoSize = true;
			this.checkBox_HideXOnXOff.Location = new System.Drawing.Point(6, 19);
			this.checkBox_HideXOnXOff.Name = "checkBox_HideXOnXOff";
			this.checkBox_HideXOnXOff.Size = new System.Drawing.Size(235, 17);
			this.checkBox_HideXOnXOff.TabIndex = 0;
			this.checkBox_HideXOnXOff.Text = "&Hide XOn/XOff (at manual software flow ctrl)";
			this.checkBox_HideXOnXOff.UseVisualStyleBackColor = true;
			this.checkBox_HideXOnXOff.CheckedChanged += new System.EventHandler(this.checkBox_HideXOnXOff_CheckedChanged);
			// 
			// checkBox_ShowBreakCount
			// 
			this.checkBox_ShowBreakCount.AutoSize = true;
			this.checkBox_ShowBreakCount.Location = new System.Drawing.Point(131, 164);
			this.checkBox_ShowBreakCount.Name = "checkBox_ShowBreakCount";
			this.checkBox_ShowBreakCount.Size = new System.Drawing.Size(113, 17);
			this.checkBox_ShowBreakCount.TabIndex = 12;
			this.checkBox_ShowBreakCount.Text = "Show brea&k count";
			this.checkBox_ShowBreakCount.UseVisualStyleBackColor = true;
			this.checkBox_ShowBreakCount.CheckedChanged += new System.EventHandler(this.checkBox_ShowBreakCount_CheckedChanged);
			// 
			// checkBox_ShowFlowControlCount
			// 
			this.checkBox_ShowFlowControlCount.AutoSize = true;
			this.checkBox_ShowFlowControlCount.Location = new System.Drawing.Point(12, 164);
			this.checkBox_ShowFlowControlCount.Name = "checkBox_ShowFlowControlCount";
			this.checkBox_ShowFlowControlCount.Size = new System.Drawing.Size(122, 17);
			this.checkBox_ShowFlowControlCount.TabIndex = 11;
			this.checkBox_ShowFlowControlCount.Text = "Show flo&w ctrl count";
			this.checkBox_ShowFlowControlCount.UseVisualStyleBackColor = true;
			this.checkBox_ShowFlowControlCount.CheckedChanged += new System.EventHandler(this.checkBox_ShowFlowControlCount_CheckedChanged);
			// 
			// checkBox_ShowLineNumbers
			// 
			this.checkBox_ShowLineNumbers.AutoSize = true;
			this.checkBox_ShowLineNumbers.Location = new System.Drawing.Point(131, 95);
			this.checkBox_ShowLineNumbers.Name = "checkBox_ShowLineNumbers";
			this.checkBox_ShowLineNumbers.Size = new System.Drawing.Size(110, 17);
			this.checkBox_ShowLineNumbers.TabIndex = 6;
			this.checkBox_ShowLineNumbers.Text = "Show line n&umber";
			this.checkBox_ShowLineNumbers.UseVisualStyleBackColor = true;
			this.checkBox_ShowLineNumbers.CheckedChanged += new System.EventHandler(this.checkBox_ShowLineNumbers_CheckedChanged);
			// 
			// groupBox_Display_Tab
			// 
			this.groupBox_Display_Tab.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Display_Tab.Controls.Add(this.label_ReplaceTab);
			this.groupBox_Display_Tab.Controls.Add(this.checkBox_ReplaceTab);
			this.groupBox_Display_Tab.Location = new System.Drawing.Point(6, 314);
			this.groupBox_Display_Tab.Name = "groupBox_Display_Tab";
			this.groupBox_Display_Tab.Size = new System.Drawing.Size(251, 46);
			this.groupBox_Display_Tab.TabIndex = 18;
			this.groupBox_Display_Tab.TabStop = false;
			this.groupBox_Display_Tab.Text = "Tab";
			// 
			// label_ReplaceTab
			// 
			this.label_ReplaceTab.AutoSize = true;
			this.label_ReplaceTab.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label_ReplaceTab.Font = new System.Drawing.Font("DejaVu Sans Mono", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_ReplaceTab.Location = new System.Drawing.Point(122, 21);
			this.label_ReplaceTab.Name = "label_ReplaceTab";
			this.label_ReplaceTab.Size = new System.Drawing.Size(44, 15);
			this.label_ReplaceTab.TabIndex = 1;
			this.label_ReplaceTab.Text = "<TAB>";
			// 
			// checkBox_ReplaceTab
			// 
			this.checkBox_ReplaceTab.AutoSize = true;
			this.checkBox_ReplaceTab.Location = new System.Drawing.Point(6, 19);
			this.checkBox_ReplaceTab.Name = "checkBox_ReplaceTab";
			this.checkBox_ReplaceTab.Size = new System.Drawing.Size(118, 17);
			this.checkBox_ReplaceTab.TabIndex = 0;
			this.checkBox_ReplaceTab.Text = "Replace real &tab by";
			this.checkBox_ReplaceTab.UseVisualStyleBackColor = true;
			this.checkBox_ReplaceTab.CheckedChanged += new System.EventHandler(this.checkBox_ReplaceTab_CheckedChanged);
			// 
			// groupBox_Display_Space
			// 
			this.groupBox_Display_Space.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Display_Space.Controls.Add(this.label_SpaceReplacementChar);
			this.groupBox_Display_Space.Controls.Add(this.checkBox_ReplaceSpace);
			this.groupBox_Display_Space.Location = new System.Drawing.Point(6, 366);
			this.groupBox_Display_Space.Name = "groupBox_Display_Space";
			this.groupBox_Display_Space.Size = new System.Drawing.Size(251, 46);
			this.groupBox_Display_Space.TabIndex = 19;
			this.groupBox_Display_Space.TabStop = false;
			this.groupBox_Display_Space.Text = "Space";
			// 
			// label_SpaceReplacementChar
			// 
			this.label_SpaceReplacementChar.AutoSize = true;
			this.label_SpaceReplacementChar.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label_SpaceReplacementChar.Font = new System.Drawing.Font("DejaVu Sans Mono", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_SpaceReplacementChar.Location = new System.Drawing.Point(220, 21);
			this.label_SpaceReplacementChar.Name = "label_SpaceReplacementChar";
			this.label_SpaceReplacementChar.Size = new System.Drawing.Size(16, 15);
			this.label_SpaceReplacementChar.TabIndex = 1;
			this.label_SpaceReplacementChar.Text = "␣";
			// 
			// checkBox_ReplaceSpace
			// 
			this.checkBox_ReplaceSpace.AutoSize = true;
			this.checkBox_ReplaceSpace.Location = new System.Drawing.Point(6, 19);
			this.checkBox_ReplaceSpace.Name = "checkBox_ReplaceSpace";
			this.checkBox_ReplaceSpace.Size = new System.Drawing.Size(216, 17);
			this.checkBox_ReplaceSpace.TabIndex = 0;
			this.checkBox_ReplaceSpace.Text = "Replace by &open box character U+2423";
			this.checkBox_ReplaceSpace.UseVisualStyleBackColor = true;
			this.checkBox_ReplaceSpace.CheckedChanged += new System.EventHandler(this.checkBox_ReplaceSpace_CheckedChanged);
			// 
			// groupBox_Display_ControlChars
			// 
			this.groupBox_Display_ControlChars.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Display_ControlChars.Controls.Add(this.comboBox_ControlCharacterRadix);
			this.groupBox_Display_ControlChars.Controls.Add(this.label_ControlCharacterRadix);
			this.groupBox_Display_ControlChars.Controls.Add(this.checkBox_ReplaceControlCharacters);
			this.groupBox_Display_ControlChars.Location = new System.Drawing.Point(6, 239);
			this.groupBox_Display_ControlChars.Name = "groupBox_Display_ControlChars";
			this.groupBox_Display_ControlChars.Size = new System.Drawing.Size(251, 69);
			this.groupBox_Display_ControlChars.TabIndex = 17;
			this.groupBox_Display_ControlChars.TabStop = false;
			this.groupBox_Display_ControlChars.Text = "Control Characters \\h(00) - \\h(1F), \\h(7F)";
			// 
			// comboBox_ControlCharacterRadix
			// 
			this.comboBox_ControlCharacterRadix.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_ControlCharacterRadix.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_ControlCharacterRadix.Location = new System.Drawing.Point(125, 42);
			this.comboBox_ControlCharacterRadix.Name = "comboBox_ControlCharacterRadix";
			this.comboBox_ControlCharacterRadix.Size = new System.Drawing.Size(121, 21);
			this.comboBox_ControlCharacterRadix.TabIndex = 2;
			this.comboBox_ControlCharacterRadix.SelectedIndexChanged += new System.EventHandler(this.comboBox_ControlCharacterRadix_SelectedIndexChanged);
			// 
			// label_ControlCharacterRadix
			// 
			this.label_ControlCharacterRadix.AutoSize = true;
			this.label_ControlCharacterRadix.Location = new System.Drawing.Point(3, 45);
			this.label_ControlCharacterRadix.Name = "label_ControlCharacterRadix";
			this.label_ControlCharacterRadix.Size = new System.Drawing.Size(37, 13);
			this.label_ControlCharacterRadix.TabIndex = 1;
			this.label_ControlCharacterRadix.Text = "Radi&x:";
			// 
			// checkBox_ReplaceControlCharacters
			// 
			this.checkBox_ReplaceControlCharacters.AutoSize = true;
			this.checkBox_ReplaceControlCharacters.Location = new System.Drawing.Point(6, 19);
			this.checkBox_ReplaceControlCharacters.Name = "checkBox_ReplaceControlCharacters";
			this.checkBox_ReplaceControlCharacters.Size = new System.Drawing.Size(219, 17);
			this.checkBox_ReplaceControlCharacters.TabIndex = 0;
			this.checkBox_ReplaceControlCharacters.Text = "Re&place characters (for string/char radix)";
			this.checkBox_ReplaceControlCharacters.CheckedChanged += new System.EventHandler(this.checkBox_ReplaceControlCharacters_CheckedChanged);
			// 
			// checkBox_DirectionLineBreak
			// 
			this.checkBox_DirectionLineBreak.AutoSize = true;
			this.checkBox_DirectionLineBreak.Location = new System.Drawing.Point(12, 187);
			this.checkBox_DirectionLineBreak.Name = "checkBox_DirectionLineBreak";
			this.checkBox_DirectionLineBreak.Size = new System.Drawing.Size(194, 17);
			this.checkBox_DirectionLineBreak.TabIndex = 13;
			this.checkBox_DirectionLineBreak.Text = "&Break lines when direction changes";
			this.checkBox_DirectionLineBreak.CheckedChanged += new System.EventHandler(this.checkBox_DirectionLineBreak_CheckedChanged);
			// 
			// comboBox_RxRadix
			// 
			this.comboBox_RxRadix.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_RxRadix.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_RxRadix.Location = new System.Drawing.Point(131, 68);
			this.comboBox_RxRadix.Name = "comboBox_RxRadix";
			this.comboBox_RxRadix.Size = new System.Drawing.Size(121, 21);
			this.comboBox_RxRadix.TabIndex = 4;
			this.comboBox_RxRadix.SelectedIndexChanged += new System.EventHandler(this.comboBox_RxRadix_SelectedIndexChanged);
			// 
			// label_RxRadix
			// 
			this.label_RxRadix.AutoSize = true;
			this.label_RxRadix.Location = new System.Drawing.Point(9, 71);
			this.label_RxRadix.Name = "label_RxRadix";
			this.label_RxRadix.Size = new System.Drawing.Size(53, 13);
			this.label_RxRadix.TabIndex = 3;
			this.label_RxRadix.Text = "R&x Radix:";
			// 
			// checkBox_SeparateTxRxRadix
			// 
			this.checkBox_SeparateTxRxRadix.AutoSize = true;
			this.checkBox_SeparateTxRxRadix.Location = new System.Drawing.Point(12, 45);
			this.checkBox_SeparateTxRxRadix.Name = "checkBox_SeparateTxRxRadix";
			this.checkBox_SeparateTxRxRadix.Size = new System.Drawing.Size(161, 17);
			this.checkBox_SeparateTxRxRadix.TabIndex = 2;
			this.checkBox_SeparateTxRxRadix.Text = "&Separate radix for Tx and Rx";
			this.checkBox_SeparateTxRxRadix.UseVisualStyleBackColor = true;
			this.checkBox_SeparateTxRxRadix.CheckedChanged += new System.EventHandler(this.checkBox_SeparateTxRxRadix_CheckedChanged);
			// 
			// checkBox_ShowRadix
			// 
			this.checkBox_ShowRadix.AutoSize = true;
			this.checkBox_ShowRadix.Location = new System.Drawing.Point(12, 95);
			this.checkBox_ShowRadix.Name = "checkBox_ShowRadix";
			this.checkBox_ShowRadix.Size = new System.Drawing.Size(78, 17);
			this.checkBox_ShowRadix.TabIndex = 5;
			this.checkBox_ShowRadix.Text = "Show &radix";
			this.checkBox_ShowRadix.CheckedChanged += new System.EventHandler(this.checkBox_ShowRadix_CheckedChanged);
			// 
			// checkBox_ShowTimeStamp
			// 
			this.checkBox_ShowTimeStamp.AutoSize = true;
			this.checkBox_ShowTimeStamp.Location = new System.Drawing.Point(12, 118);
			this.checkBox_ShowTimeStamp.Name = "checkBox_ShowTimeStamp";
			this.checkBox_ShowTimeStamp.Size = new System.Drawing.Size(106, 17);
			this.checkBox_ShowTimeStamp.TabIndex = 7;
			this.checkBox_ShowTimeStamp.Text = "Show &time stamp";
			this.checkBox_ShowTimeStamp.CheckedChanged += new System.EventHandler(this.checkBox_ShowTimeStamp_CheckedChanged);
			// 
			// checkBox_ShowConnectTime
			// 
			this.checkBox_ShowConnectTime.AutoSize = true;
			this.checkBox_ShowConnectTime.Location = new System.Drawing.Point(12, 141);
			this.checkBox_ShowConnectTime.Name = "checkBox_ShowConnectTime";
			this.checkBox_ShowConnectTime.Size = new System.Drawing.Size(117, 17);
			this.checkBox_ShowConnectTime.TabIndex = 9;
			this.checkBox_ShowConnectTime.Text = "Show connect t&ime";
			this.checkBox_ShowConnectTime.CheckedChanged += new System.EventHandler(this.checkBox_ShowConnectTime_CheckedChanged);
			// 
			// checkBox_ShowCountAndRate
			// 
			this.checkBox_ShowCountAndRate.AutoSize = true;
			this.checkBox_ShowCountAndRate.Location = new System.Drawing.Point(131, 141);
			this.checkBox_ShowCountAndRate.Name = "checkBox_ShowCountAndRate";
			this.checkBox_ShowCountAndRate.Size = new System.Drawing.Size(125, 17);
			this.checkBox_ShowCountAndRate.TabIndex = 10;
			this.checkBox_ShowCountAndRate.Text = "Show &count and rate";
			this.checkBox_ShowCountAndRate.CheckedChanged += new System.EventHandler(this.checkBox_ShowCountAndRate_CheckedChanged);
			// 
			// checkBox_ShowLength
			// 
			this.checkBox_ShowLength.AutoSize = true;
			this.checkBox_ShowLength.Location = new System.Drawing.Point(131, 118);
			this.checkBox_ShowLength.Name = "checkBox_ShowLength";
			this.checkBox_ShowLength.Size = new System.Drawing.Size(104, 17);
			this.checkBox_ShowLength.TabIndex = 8;
			this.checkBox_ShowLength.Text = "Show line &length";
			this.checkBox_ShowLength.CheckedChanged += new System.EventHandler(this.checkBox_ShowLength_CheckedChanged);
			// 
			// comboBox_TxRadix
			// 
			this.comboBox_TxRadix.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_TxRadix.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_TxRadix.Location = new System.Drawing.Point(131, 18);
			this.comboBox_TxRadix.Name = "comboBox_TxRadix";
			this.comboBox_TxRadix.Size = new System.Drawing.Size(121, 21);
			this.comboBox_TxRadix.TabIndex = 1;
			this.comboBox_TxRadix.SelectedIndexChanged += new System.EventHandler(this.comboBox_TxRadix_SelectedIndexChanged);
			// 
			// label_TxRadix
			// 
			this.label_TxRadix.AutoSize = true;
			this.label_TxRadix.Location = new System.Drawing.Point(9, 21);
			this.label_TxRadix.Name = "label_TxRadix";
			this.label_TxRadix.Size = new System.Drawing.Size(37, 13);
			this.label_TxRadix.TabIndex = 0;
			this.label_TxRadix.Text = "R&adix:";
			// 
			// label_MaxLineCountUnit
			// 
			this.label_MaxLineCountUnit.AutoSize = true;
			this.label_MaxLineCountUnit.Location = new System.Drawing.Point(144, 213);
			this.label_MaxLineCountUnit.Name = "label_MaxLineCountUnit";
			this.label_MaxLineCountUnit.Size = new System.Drawing.Size(28, 13);
			this.label_MaxLineCountUnit.TabIndex = 16;
			this.label_MaxLineCountUnit.Text = "lines";
			this.label_MaxLineCountUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox_MaxLineCount
			// 
			this.textBox_MaxLineCount.Location = new System.Drawing.Point(92, 210);
			this.textBox_MaxLineCount.Name = "textBox_MaxLineCount";
			this.textBox_MaxLineCount.Size = new System.Drawing.Size(50, 20);
			this.textBox_MaxLineCount.TabIndex = 15;
			this.textBox_MaxLineCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBox_MaxLineCount.TextChanged += new System.EventHandler(this.textBox_MaxLineCount_TextChanged);
			this.textBox_MaxLineCount.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_MaxLineCount_Validating);
			// 
			// label_MaxLineCount
			// 
			this.label_MaxLineCount.AutoSize = true;
			this.label_MaxLineCount.Location = new System.Drawing.Point(9, 213);
			this.label_MaxLineCount.Name = "label_MaxLineCount";
			this.label_MaxLineCount.Size = new System.Drawing.Size(81, 13);
			this.label_MaxLineCount.TabIndex = 14;
			this.label_MaxLineCount.Text = "Display &maximal";
			this.label_MaxLineCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkBox_DisableKeywords
			// 
			this.checkBox_DisableKeywords.AutoSize = true;
			this.checkBox_DisableKeywords.Location = new System.Drawing.Point(9, 67);
			this.checkBox_DisableKeywords.Name = "checkBox_DisableKeywords";
			this.checkBox_DisableKeywords.Size = new System.Drawing.Size(148, 17);
			this.checkBox_DisableKeywords.TabIndex = 6;
			this.checkBox_DisableKeywords.Text = "&Disable all \\!(...) keywords";
			this.checkBox_DisableKeywords.UseVisualStyleBackColor = true;
			this.checkBox_DisableKeywords.CheckedChanged += new System.EventHandler(this.checkBox_DisableKeywords_CheckedChanged);
			// 
			// groupBox_Send_Keywords
			// 
			this.groupBox_Send_Keywords.Controls.Add(this.label_DefaultLineDelayUnit);
			this.groupBox_Send_Keywords.Controls.Add(this.textBox_DefaultLineDelay);
			this.groupBox_Send_Keywords.Controls.Add(this.label_DefaultLineDelay);
			this.groupBox_Send_Keywords.Controls.Add(this.label_DefaultDelayUnit);
			this.groupBox_Send_Keywords.Controls.Add(this.textBox_DefaultDelay);
			this.groupBox_Send_Keywords.Controls.Add(this.label_DefaultDelay);
			this.groupBox_Send_Keywords.Controls.Add(this.checkBox_DisableKeywords);
			this.groupBox_Send_Keywords.Location = new System.Drawing.Point(6, 190);
			this.groupBox_Send_Keywords.Name = "groupBox_Send_Keywords";
			this.groupBox_Send_Keywords.Size = new System.Drawing.Size(251, 92);
			this.groupBox_Send_Keywords.TabIndex = 10;
			this.groupBox_Send_Keywords.TabStop = false;
			this.groupBox_Send_Keywords.Text = "Keywords";
			// 
			// label_DefaultLineDelayUnit
			// 
			this.label_DefaultLineDelayUnit.AutoSize = true;
			this.label_DefaultLineDelayUnit.Location = new System.Drawing.Point(196, 42);
			this.label_DefaultLineDelayUnit.Name = "label_DefaultLineDelayUnit";
			this.label_DefaultLineDelayUnit.Size = new System.Drawing.Size(20, 13);
			this.label_DefaultLineDelayUnit.TabIndex = 5;
			this.label_DefaultLineDelayUnit.Text = "ms";
			this.label_DefaultLineDelayUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox_DefaultLineDelay
			// 
			this.textBox_DefaultLineDelay.Location = new System.Drawing.Point(144, 39);
			this.textBox_DefaultLineDelay.Name = "textBox_DefaultLineDelay";
			this.textBox_DefaultLineDelay.Size = new System.Drawing.Size(50, 20);
			this.textBox_DefaultLineDelay.TabIndex = 4;
			this.textBox_DefaultLineDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label_DefaultLineDelay
			// 
			this.label_DefaultLineDelay.AutoSize = true;
			this.label_DefaultLineDelay.Location = new System.Drawing.Point(6, 42);
			this.label_DefaultLineDelay.Name = "label_DefaultLineDelay";
			this.label_DefaultLineDelay.Size = new System.Drawing.Size(127, 13);
			this.label_DefaultLineDelay.TabIndex = 3;
			this.label_DefaultLineDelay.Text = "Default of \\!(&LineDelay) is\r\n";
			this.label_DefaultLineDelay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label_DefaultDelayUnit
			// 
			this.label_DefaultDelayUnit.AutoSize = true;
			this.label_DefaultDelayUnit.Location = new System.Drawing.Point(196, 21);
			this.label_DefaultDelayUnit.Name = "label_DefaultDelayUnit";
			this.label_DefaultDelayUnit.Size = new System.Drawing.Size(20, 13);
			this.label_DefaultDelayUnit.TabIndex = 2;
			this.label_DefaultDelayUnit.Text = "ms";
			this.label_DefaultDelayUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox_DefaultDelay
			// 
			this.textBox_DefaultDelay.Location = new System.Drawing.Point(144, 18);
			this.textBox_DefaultDelay.Name = "textBox_DefaultDelay";
			this.textBox_DefaultDelay.Size = new System.Drawing.Size(50, 20);
			this.textBox_DefaultDelay.TabIndex = 1;
			this.textBox_DefaultDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label_DefaultDelay
			// 
			this.label_DefaultDelay.AutoSize = true;
			this.label_DefaultDelay.Location = new System.Drawing.Point(6, 21);
			this.label_DefaultDelay.Name = "label_DefaultDelay";
			this.label_DefaultDelay.Size = new System.Drawing.Size(107, 13);
			this.label_DefaultDelay.TabIndex = 0;
			this.label_DefaultDelay.Text = "Default of \\!(&Delay) is\r\n";
			this.label_DefaultDelay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// AdvancedTerminalSettings
			// 
			this.AcceptButton = this.button_OK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button_Cancel;
			this.ClientSize = new System.Drawing.Size(652, 513);
			this.Controls.Add(this.groupBox_Settings);
			this.Controls.Add(this.button_Defaults);
			this.Controls.Add(this.button_Cancel);
			this.Controls.Add(this.button_OK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AdvancedTerminalSettings";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Advanced Terminal Settings";
			this.Shown += new System.EventHandler(this.AdvancedTerminalSettings_Shown);
			this.groupBox_Settings.ResumeLayout(false);
			this.groupBox_User.ResumeLayout(false);
			this.groupBox_User.PerformLayout();
			this.groupBox_Communication.ResumeLayout(false);
			this.groupBox_Communication.PerformLayout();
			this.groupBox_Communication_Break.ResumeLayout(false);
			this.groupBox_Communication_Break.PerformLayout();
			this.groupBox_Send.ResumeLayout(false);
			this.groupBox_Send.PerformLayout();
			this.groupBox_Display.ResumeLayout(false);
			this.groupBox_Display.PerformLayout();
			this.groupBox_Display_XOnXOff.ResumeLayout(false);
			this.groupBox_Display_XOnXOff.PerformLayout();
			this.groupBox_Display_Tab.ResumeLayout(false);
			this.groupBox_Display_Tab.PerformLayout();
			this.groupBox_Display_Space.ResumeLayout(false);
			this.groupBox_Display_Space.PerformLayout();
			this.groupBox_Display_ControlChars.ResumeLayout(false);
			this.groupBox_Display_ControlChars.PerformLayout();
			this.groupBox_Send_Keywords.ResumeLayout(false);
			this.groupBox_Send_Keywords.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button button_Defaults;
		private System.Windows.Forms.Button button_Cancel;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.GroupBox groupBox_Settings;
		private System.Windows.Forms.GroupBox groupBox_Send;
		private System.Windows.Forms.GroupBox groupBox_Display;
		private System.Windows.Forms.Label label_MaxLineCountUnit;
		private System.Windows.Forms.TextBox textBox_MaxLineCount;
		private System.Windows.Forms.Label label_MaxLineCount;
		private System.Windows.Forms.CheckBox checkBox_KeepCommand;
		private System.Windows.Forms.CheckBox checkBox_ShowTimeStamp;
		private System.Windows.Forms.CheckBox checkBox_ShowCountAndRate;
		private System.Windows.Forms.CheckBox checkBox_ShowLength;
		private System.Windows.Forms.ComboBox comboBox_TxRadix;
		private System.Windows.Forms.Label label_TxRadix;
		private System.Windows.Forms.GroupBox groupBox_Communication;
		private System.Windows.Forms.ComboBox comboBox_Endianness;
		private System.Windows.Forms.Label label_Endianness;
		private System.Windows.Forms.CheckBox checkBox_SeparateTxRxRadix;
		private System.Windows.Forms.ComboBox comboBox_RxRadix;
		private System.Windows.Forms.Label label_RxRadix;
		private System.Windows.Forms.CheckBox checkBox_DirectionLineBreak;
		private System.Windows.Forms.GroupBox groupBox_Display_ControlChars;
		private System.Windows.Forms.ComboBox comboBox_ControlCharacterRadix;
		private System.Windows.Forms.Label label_ControlCharacterRadix;
		private System.Windows.Forms.CheckBox checkBox_ReplaceControlCharacters;
		private System.Windows.Forms.GroupBox groupBox_Display_Space;
		private System.Windows.Forms.Label label_SpaceReplacementChar;
		private System.Windows.Forms.CheckBox checkBox_ReplaceSpace;
		private System.Windows.Forms.CheckBox checkBox_ShowConnectTime;
		private System.Windows.Forms.CheckBox checkBox_CopyPredefined;
		private System.Windows.Forms.CheckBox checkBox_ShowRadix;
		private System.Windows.Forms.GroupBox groupBox_Display_Tab;
		private System.Windows.Forms.Label label_ReplaceTab;
		private System.Windows.Forms.CheckBox checkBox_ReplaceTab;
		private System.Windows.Forms.CheckBox checkBox_SendImmediately;
		private System.Windows.Forms.Label label_SendImmediately;
		private System.Windows.Forms.GroupBox groupBox_User;
		private System.Windows.Forms.TextBox textBox_UserName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox checkBox_NoSendOnInputBreak;
		private System.Windows.Forms.CheckBox checkBox_NoSendOnOutputBreak;
		private System.Windows.Forms.GroupBox groupBox_Communication_Break;
		private System.Windows.Forms.CheckBox checkBox_OutputBreakModifiable;
		private System.Windows.Forms.CheckBox checkBox_IndicateBreakStates;
		private System.Windows.Forms.CheckBox checkBox_ShowLineNumbers;
		private System.Windows.Forms.TextBox textBox_MaxSendChunkSize;
		private System.Windows.Forms.Label label_MaxSendChunkSize;
		private System.Windows.Forms.Label label_MaxSendChunkSizeRemark;
		private System.Windows.Forms.Label label_MaxSendChunkSizeUnit;
		private System.Windows.Forms.CheckBox checkBox_ShowBreakCount;
		private System.Windows.Forms.CheckBox checkBox_ShowFlowControlCount;
		private System.Windows.Forms.GroupBox groupBox_Display_XOnXOff;
		private System.Windows.Forms.CheckBox checkBox_HideXOnXOff;
		private System.Windows.Forms.CheckBox checkBox_DisableKeywords;
		private System.Windows.Forms.GroupBox groupBox_Send_Keywords;
		private System.Windows.Forms.Label label_DefaultLineDelayUnit;
		private System.Windows.Forms.TextBox textBox_DefaultLineDelay;
		private System.Windows.Forms.Label label_DefaultLineDelay;
		private System.Windows.Forms.Label label_DefaultDelayUnit;
		private System.Windows.Forms.TextBox textBox_DefaultDelay;
		private System.Windows.Forms.Label label_DefaultDelay;
	}
}