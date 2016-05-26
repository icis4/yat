namespace YAT.View.Forms
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
			this.components = new System.ComponentModel.Container();
			this.button_Defaults = new System.Windows.Forms.Button();
			this.button_Cancel = new System.Windows.Forms.Button();
			this.button_OK = new System.Windows.Forms.Button();
			this.groupBox_Settings = new System.Windows.Forms.GroupBox();
			this.groupBox_User = new System.Windows.Forms.GroupBox();
			this.textBox_UserName = new System.Windows.Forms.TextBox();
			this.label_UserName = new System.Windows.Forms.Label();
			this.groupBox_Communication = new System.Windows.Forms.GroupBox();
			this.groupBox_Communication_SerialPorts = new System.Windows.Forms.GroupBox();
			this.checkBox_OutputBreakModifiable = new System.Windows.Forms.CheckBox();
			this.checkBox_IndicateBreakStates = new System.Windows.Forms.CheckBox();
			this.comboBox_Endianness = new System.Windows.Forms.ComboBox();
			this.label_Endianness = new System.Windows.Forms.Label();
			this.groupBox_Send = new System.Windows.Forms.GroupBox();
			this.label_SignalXOnPeriodicallyIntervalUnit = new System.Windows.Forms.Label();
			this.groupBox_Send_SerialPorts = new System.Windows.Forms.GroupBox();
			this.textBox_MaxSendRateSize = new System.Windows.Forms.TextBox();
			this.textBox_MaxSendRateInterval = new System.Windows.Forms.TextBox();
			this.textBox_MaxChunkSize = new System.Windows.Forms.TextBox();
			this.textBox_OutputBufferSize = new System.Windows.Forms.TextBox();
			this.checkBox_OutputBufferSize = new System.Windows.Forms.CheckBox();
			this.label_OutputBufferSizeUnit = new System.Windows.Forms.Label();
			this.checkBox_MaxChunkSize = new System.Windows.Forms.CheckBox();
			this.label_MaxChunkSizeUnit = new System.Windows.Forms.Label();
			this.checkBox_MaxSendRateEnable = new System.Windows.Forms.CheckBox();
			this.label_MaxSendRateIntervalUnit1 = new System.Windows.Forms.Label();
			this.label_MaxSendRateIntervalUnit2 = new System.Windows.Forms.Label();
			this.checkBox_NoSendOnInputBreak = new System.Windows.Forms.CheckBox();
			this.checkBox_NoSendOnOutputBreak = new System.Windows.Forms.CheckBox();
			this.textBox_SignalXOnPeriodicallyInterval = new System.Windows.Forms.TextBox();
			this.groupBox_Send_Keywords = new System.Windows.Forms.GroupBox();
			this.label_DefaultLineIntervalUnit = new System.Windows.Forms.Label();
			this.textBox_DefaultLineInterval = new System.Windows.Forms.TextBox();
			this.label_DefaultLineInterval = new System.Windows.Forms.Label();
			this.label_DefaultLineRepeatUnit = new System.Windows.Forms.Label();
			this.textBox_DefaultLineRepeat = new System.Windows.Forms.TextBox();
			this.label_DefaultLineRepeat = new System.Windows.Forms.Label();
			this.label_DefaultLineDelayUnit = new System.Windows.Forms.Label();
			this.textBox_DefaultLineDelay = new System.Windows.Forms.TextBox();
			this.label_DefaultLineDelay = new System.Windows.Forms.Label();
			this.label_DefaultDelayUnit = new System.Windows.Forms.Label();
			this.textBox_DefaultDelay = new System.Windows.Forms.TextBox();
			this.label_DefaultDelay = new System.Windows.Forms.Label();
			this.checkBox_DisableKeywords = new System.Windows.Forms.CheckBox();
			this.checkBox_SignalXOnPeriodicallyEnable = new System.Windows.Forms.CheckBox();
			this.checkBox_SignalXOnBeforeEachTransmission = new System.Windows.Forms.CheckBox();
			this.checkBox_SendImmediately = new System.Windows.Forms.CheckBox();
			this.checkBox_CopyPredefined = new System.Windows.Forms.CheckBox();
			this.checkBox_KeepCommand = new System.Windows.Forms.CheckBox();
			this.groupBox_Display = new System.Windows.Forms.GroupBox();
			this.textBox_MaxBytePerLineCount = new System.Windows.Forms.TextBox();
			this.label_MaxBytePerLineCountUnit = new System.Windows.Forms.Label();
			this.label_MaxBytePerLineCount = new System.Windows.Forms.Label();
			this.checkBox_PortLineBreak = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowPort = new System.Windows.Forms.CheckBox();
			this.textBox_MaxLineCount = new System.Windows.Forms.TextBox();
			this.groupBox_Display_Special = new System.Windows.Forms.GroupBox();
			this.checkBox_Hide0xFF = new System.Windows.Forms.CheckBox();
			this.checkBox_Hide0x00 = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowDirection = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowBreakCount = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowFlowControlCount = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowLineNumbers = new System.Windows.Forms.CheckBox();
			this.groupBox_Display_Space = new System.Windows.Forms.GroupBox();
			this.label_SpaceReplacementChar = new System.Windows.Forms.Label();
			this.label_ReplaceSpaceUnicode = new System.Windows.Forms.Label();
			this.checkBox_ReplaceSpace = new System.Windows.Forms.CheckBox();
			this.groupBox_Display_ControlChars = new System.Windows.Forms.GroupBox();
			this.label_ReplaceTab = new System.Windows.Forms.Label();
			this.checkBox_HideXOnXOff = new System.Windows.Forms.CheckBox();
			this.comboBox_ControlCharacterRadix = new System.Windows.Forms.ComboBox();
			this.checkBox_ReplaceTab = new System.Windows.Forms.CheckBox();
			this.label_ControlCharacterRadix = new System.Windows.Forms.Label();
			this.checkBox_ReplaceControlCharacters = new System.Windows.Forms.CheckBox();
			this.checkBox_DirectionLineBreak = new System.Windows.Forms.CheckBox();
			this.comboBox_RxRadix = new System.Windows.Forms.ComboBox();
			this.label_RxRadix = new System.Windows.Forms.Label();
			this.checkBox_SeparateTxRxRadix = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowRadix = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowDate = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowTime = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowConnectTime = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowCountAndRate = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowLength = new System.Windows.Forms.CheckBox();
			this.comboBox_TxRadix = new System.Windows.Forms.ComboBox();
			this.label_TxRadix = new System.Windows.Forms.Label();
			this.label_MaxLineCountUnit = new System.Windows.Forms.Label();
			this.label_MaxLineCount = new System.Windows.Forms.Label();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.checkBox_UseExplicitDefaultRadix = new System.Windows.Forms.CheckBox();
			this.groupBox_Settings.SuspendLayout();
			this.groupBox_User.SuspendLayout();
			this.groupBox_Communication.SuspendLayout();
			this.groupBox_Communication_SerialPorts.SuspendLayout();
			this.groupBox_Send.SuspendLayout();
			this.groupBox_Send_SerialPorts.SuspendLayout();
			this.groupBox_Send_Keywords.SuspendLayout();
			this.groupBox_Display.SuspendLayout();
			this.groupBox_Display_Special.SuspendLayout();
			this.groupBox_Display_Space.SuspendLayout();
			this.groupBox_Display_ControlChars.SuspendLayout();
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
			this.groupBox_Settings.Size = new System.Drawing.Size(544, 637);
			this.groupBox_Settings.TabIndex = 0;
			this.groupBox_Settings.TabStop = false;
			// 
			// groupBox_User
			// 
			this.groupBox_User.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_User.Controls.Add(this.textBox_UserName);
			this.groupBox_User.Controls.Add(this.label_UserName);
			this.groupBox_User.Location = new System.Drawing.Point(275, 582);
			this.groupBox_User.Name = "groupBox_User";
			this.groupBox_User.Size = new System.Drawing.Size(263, 49);
			this.groupBox_User.TabIndex = 3;
			this.groupBox_User.TabStop = false;
			this.groupBox_User.Text = "User Settings";
			// 
			// textBox_UserName
			// 
			this.textBox_UserName.Location = new System.Drawing.Point(94, 17);
			this.textBox_UserName.Name = "textBox_UserName";
			this.textBox_UserName.Size = new System.Drawing.Size(157, 20);
			this.textBox_UserName.TabIndex = 1;
			this.textBox_UserName.TextChanged += new System.EventHandler(this.textBox_UserName_TextChanged);
			// 
			// label_UserName
			// 
			this.label_UserName.AutoSize = true;
			this.label_UserName.Location = new System.Drawing.Point(9, 20);
			this.label_UserName.Name = "label_UserName";
			this.label_UserName.Size = new System.Drawing.Size(79, 13);
			this.label_UserName.TabIndex = 0;
			this.label_UserName.Text = "Terminal &name:";
			// 
			// groupBox_Communication
			// 
			this.groupBox_Communication.Controls.Add(this.groupBox_Communication_SerialPorts);
			this.groupBox_Communication.Controls.Add(this.comboBox_Endianness);
			this.groupBox_Communication.Controls.Add(this.label_Endianness);
			this.groupBox_Communication.Location = new System.Drawing.Point(275, 13);
			this.groupBox_Communication.Name = "groupBox_Communication";
			this.groupBox_Communication.Size = new System.Drawing.Size(263, 120);
			this.groupBox_Communication.TabIndex = 1;
			this.groupBox_Communication.TabStop = false;
			this.groupBox_Communication.Text = "Communication Settings";
			// 
			// groupBox_Communication_SerialPorts
			// 
			this.groupBox_Communication_SerialPorts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Communication_SerialPorts.Controls.Add(this.checkBox_OutputBreakModifiable);
			this.groupBox_Communication_SerialPorts.Controls.Add(this.checkBox_IndicateBreakStates);
			this.groupBox_Communication_SerialPorts.Location = new System.Drawing.Point(6, 49);
			this.groupBox_Communication_SerialPorts.Name = "groupBox_Communication_SerialPorts";
			this.groupBox_Communication_SerialPorts.Size = new System.Drawing.Size(251, 65);
			this.groupBox_Communication_SerialPorts.TabIndex = 2;
			this.groupBox_Communication_SerialPorts.TabStop = false;
			this.groupBox_Communication_SerialPorts.Text = "Serial COM ports";
			// 
			// checkBox_OutputBreakModifiable
			// 
			this.checkBox_OutputBreakModifiable.AutoSize = true;
			this.checkBox_OutputBreakModifiable.Location = new System.Drawing.Point(6, 42);
			this.checkBox_OutputBreakModifiable.Name = "checkBox_OutputBreakModifiable";
			this.checkBox_OutputBreakModifiable.Size = new System.Drawing.Size(192, 17);
			this.checkBox_OutputBreakModifiable.TabIndex = 1;
			this.checkBox_OutputBreakModifiable.Text = "Output break state can be &modified";
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
			this.checkBox_IndicateBreakStates.Text = "Indicate brea&k states";
			this.checkBox_IndicateBreakStates.UseVisualStyleBackColor = true;
			this.checkBox_IndicateBreakStates.CheckedChanged += new System.EventHandler(this.checkBox_IndicateBreakStates_CheckedChanged);
			// 
			// comboBox_Endianness
			// 
			this.comboBox_Endianness.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_Endianness.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_Endianness.Location = new System.Drawing.Point(80, 18);
			this.comboBox_Endianness.Name = "comboBox_Endianness";
			this.comboBox_Endianness.Size = new System.Drawing.Size(172, 21);
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
			this.groupBox_Send.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Send.Controls.Add(this.checkBox_UseExplicitDefaultRadix);
			this.groupBox_Send.Controls.Add(this.label_SignalXOnPeriodicallyIntervalUnit);
			this.groupBox_Send.Controls.Add(this.groupBox_Send_SerialPorts);
			this.groupBox_Send.Controls.Add(this.textBox_SignalXOnPeriodicallyInterval);
			this.groupBox_Send.Controls.Add(this.groupBox_Send_Keywords);
			this.groupBox_Send.Controls.Add(this.checkBox_SignalXOnPeriodicallyEnable);
			this.groupBox_Send.Controls.Add(this.checkBox_SignalXOnBeforeEachTransmission);
			this.groupBox_Send.Controls.Add(this.checkBox_SendImmediately);
			this.groupBox_Send.Controls.Add(this.checkBox_CopyPredefined);
			this.groupBox_Send.Controls.Add(this.checkBox_KeepCommand);
			this.groupBox_Send.Location = new System.Drawing.Point(275, 139);
			this.groupBox_Send.Name = "groupBox_Send";
			this.groupBox_Send.Size = new System.Drawing.Size(263, 437);
			this.groupBox_Send.TabIndex = 2;
			this.groupBox_Send.TabStop = false;
			this.groupBox_Send.Text = "Send Settings";
			// 
			// label_SignalXOnPeriodicallyIntervalUnit
			// 
			this.label_SignalXOnPeriodicallyIntervalUnit.AutoSize = true;
			this.label_SignalXOnPeriodicallyIntervalUnit.Location = new System.Drawing.Point(218, 139);
			this.label_SignalXOnPeriodicallyIntervalUnit.Name = "label_SignalXOnPeriodicallyIntervalUnit";
			this.label_SignalXOnPeriodicallyIntervalUnit.Size = new System.Drawing.Size(20, 13);
			this.label_SignalXOnPeriodicallyIntervalUnit.TabIndex = 6;
			this.label_SignalXOnPeriodicallyIntervalUnit.Text = "ms";
			this.label_SignalXOnPeriodicallyIntervalUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupBox_Send_SerialPorts
			// 
			this.groupBox_Send_SerialPorts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Send_SerialPorts.Controls.Add(this.textBox_MaxSendRateSize);
			this.groupBox_Send_SerialPorts.Controls.Add(this.textBox_MaxSendRateInterval);
			this.groupBox_Send_SerialPorts.Controls.Add(this.textBox_MaxChunkSize);
			this.groupBox_Send_SerialPorts.Controls.Add(this.textBox_OutputBufferSize);
			this.groupBox_Send_SerialPorts.Controls.Add(this.checkBox_OutputBufferSize);
			this.groupBox_Send_SerialPorts.Controls.Add(this.label_OutputBufferSizeUnit);
			this.groupBox_Send_SerialPorts.Controls.Add(this.checkBox_MaxChunkSize);
			this.groupBox_Send_SerialPorts.Controls.Add(this.label_MaxChunkSizeUnit);
			this.groupBox_Send_SerialPorts.Controls.Add(this.checkBox_MaxSendRateEnable);
			this.groupBox_Send_SerialPorts.Controls.Add(this.label_MaxSendRateIntervalUnit1);
			this.groupBox_Send_SerialPorts.Controls.Add(this.label_MaxSendRateIntervalUnit2);
			this.groupBox_Send_SerialPorts.Controls.Add(this.checkBox_NoSendOnInputBreak);
			this.groupBox_Send_SerialPorts.Controls.Add(this.checkBox_NoSendOnOutputBreak);
			this.groupBox_Send_SerialPorts.Location = new System.Drawing.Point(6, 162);
			this.groupBox_Send_SerialPorts.Name = "groupBox_Send_SerialPorts";
			this.groupBox_Send_SerialPorts.Size = new System.Drawing.Size(251, 135);
			this.groupBox_Send_SerialPorts.TabIndex = 7;
			this.groupBox_Send_SerialPorts.TabStop = false;
			this.groupBox_Send_SerialPorts.Text = "Serial COM ports";
			// 
			// textBox_MaxSendRateSize
			// 
			this.textBox_MaxSendRateSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textBox_MaxSendRateSize.Location = new System.Drawing.Point(78, 63);
			this.textBox_MaxSendRateSize.Name = "textBox_MaxSendRateSize";
			this.textBox_MaxSendRateSize.Size = new System.Drawing.Size(48, 20);
			this.textBox_MaxSendRateSize.TabIndex = 7;
			this.textBox_MaxSendRateSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBox_MaxSendRateSize.TextChanged += new System.EventHandler(this.textBox_MaxSendRateSize_TextChanged);
			this.textBox_MaxSendRateSize.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_MaxSendRateSize_Validating);
			// 
			// textBox_MaxSendRateInterval
			// 
			this.textBox_MaxSendRateInterval.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textBox_MaxSendRateInterval.Location = new System.Drawing.Point(178, 63);
			this.textBox_MaxSendRateInterval.Name = "textBox_MaxSendRateInterval";
			this.textBox_MaxSendRateInterval.Size = new System.Drawing.Size(48, 20);
			this.textBox_MaxSendRateInterval.TabIndex = 9;
			this.textBox_MaxSendRateInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBox_MaxSendRateInterval.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_MaxSendRateInterval_Validating);
			// 
			// textBox_MaxChunkSize
			// 
			this.textBox_MaxChunkSize.Location = new System.Drawing.Point(141, 40);
			this.textBox_MaxChunkSize.Name = "textBox_MaxChunkSize";
			this.textBox_MaxChunkSize.Size = new System.Drawing.Size(48, 20);
			this.textBox_MaxChunkSize.TabIndex = 4;
			this.textBox_MaxChunkSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBox_MaxChunkSize.TextChanged += new System.EventHandler(this.textBox_MaxChunkSize_TextChanged);
			this.textBox_MaxChunkSize.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_MaxChunkSize_Validating);
			// 
			// textBox_OutputBufferSize
			// 
			this.textBox_OutputBufferSize.Location = new System.Drawing.Point(121, 17);
			this.textBox_OutputBufferSize.Name = "textBox_OutputBufferSize";
			this.textBox_OutputBufferSize.Size = new System.Drawing.Size(48, 20);
			this.textBox_OutputBufferSize.TabIndex = 1;
			this.textBox_OutputBufferSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBox_OutputBufferSize.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_OutputBufferSize_Validating);
			// 
			// checkBox_OutputBufferSize
			// 
			this.checkBox_OutputBufferSize.AutoSize = true;
			this.checkBox_OutputBufferSize.Location = new System.Drawing.Point(6, 19);
			this.checkBox_OutputBufferSize.Name = "checkBox_OutputBufferSize";
			this.checkBox_OutputBufferSize.Size = new System.Drawing.Size(117, 17);
			this.checkBox_OutputBufferSize.TabIndex = 0;
			this.checkBox_OutputBufferSize.Text = "&Set output buffer to";
			this.toolTip.SetToolTip(this.checkBox_OutputBufferSize, "Limiting the output buffer improves the responsiveness of hardware\r\nand/or softwa" +
        "re flow control, i.e. sending will be suspended sooner.\r\nEnlarging the output bu" +
        "ffer improves throughput.");
			this.checkBox_OutputBufferSize.UseVisualStyleBackColor = true;
			this.checkBox_OutputBufferSize.CheckedChanged += new System.EventHandler(this.checkBox_OutputBufferSize_CheckedChanged);
			// 
			// label_OutputBufferSizeUnit
			// 
			this.label_OutputBufferSizeUnit.AutoSize = true;
			this.label_OutputBufferSizeUnit.Location = new System.Drawing.Point(171, 20);
			this.label_OutputBufferSizeUnit.Name = "label_OutputBufferSizeUnit";
			this.label_OutputBufferSizeUnit.Size = new System.Drawing.Size(32, 13);
			this.label_OutputBufferSizeUnit.TabIndex = 2;
			this.label_OutputBufferSizeUnit.Text = "bytes";
			this.label_OutputBufferSizeUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkBox_MaxChunkSize
			// 
			this.checkBox_MaxChunkSize.AutoSize = true;
			this.checkBox_MaxChunkSize.Location = new System.Drawing.Point(6, 42);
			this.checkBox_MaxChunkSize.Name = "checkBox_MaxChunkSize";
			this.checkBox_MaxChunkSize.Size = new System.Drawing.Size(137, 17);
			this.checkBox_MaxChunkSize.TabIndex = 3;
			this.checkBox_MaxChunkSize.Text = "Send in &chunks of max.";
			this.toolTip.SetToolTip(this.checkBox_MaxChunkSize, "Limiting the chunk size enables use cases where a device is not\r\ncapable to proce" +
        "ss more than a certain number of bytes per paket.");
			this.checkBox_MaxChunkSize.UseVisualStyleBackColor = true;
			this.checkBox_MaxChunkSize.CheckedChanged += new System.EventHandler(this.checkBox_MaxChunkSize_CheckedChanged);
			// 
			// label_MaxChunkSizeUnit
			// 
			this.label_MaxChunkSizeUnit.AutoSize = true;
			this.label_MaxChunkSizeUnit.Location = new System.Drawing.Point(191, 43);
			this.label_MaxChunkSizeUnit.Name = "label_MaxChunkSizeUnit";
			this.label_MaxChunkSizeUnit.Size = new System.Drawing.Size(32, 13);
			this.label_MaxChunkSizeUnit.TabIndex = 5;
			this.label_MaxChunkSizeUnit.Text = "bytes";
			this.label_MaxChunkSizeUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkBox_MaxSendRateEnable
			// 
			this.checkBox_MaxSendRateEnable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkBox_MaxSendRateEnable.AutoSize = true;
			this.checkBox_MaxSendRateEnable.Location = new System.Drawing.Point(6, 65);
			this.checkBox_MaxSendRateEnable.Name = "checkBox_MaxSendRateEnable";
			this.checkBox_MaxSendRateEnable.Size = new System.Drawing.Size(76, 17);
			this.checkBox_MaxSendRateEnable.TabIndex = 6;
			this.checkBox_MaxSendRateEnable.Text = "Send ma&x.";
			this.toolTip.SetToolTip(this.checkBox_MaxSendRateEnable, "Limiting the send rate enables use cases where a device is not\r\ncapable to proces" +
        "s more than a certain number of bytes per interval.");
			this.checkBox_MaxSendRateEnable.UseVisualStyleBackColor = true;
			this.checkBox_MaxSendRateEnable.CheckedChanged += new System.EventHandler(this.checkBox_MaxSendRateEnable_CheckedChanged);
			// 
			// label_MaxSendRateIntervalUnit1
			// 
			this.label_MaxSendRateIntervalUnit1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label_MaxSendRateIntervalUnit1.AutoSize = true;
			this.label_MaxSendRateIntervalUnit1.Location = new System.Drawing.Point(128, 66);
			this.label_MaxSendRateIntervalUnit1.Name = "label_MaxSendRateIntervalUnit1";
			this.label_MaxSendRateIntervalUnit1.Size = new System.Drawing.Size(50, 13);
			this.label_MaxSendRateIntervalUnit1.TabIndex = 8;
			this.label_MaxSendRateIntervalUnit1.Text = "bytes per";
			this.label_MaxSendRateIntervalUnit1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label_MaxSendRateIntervalUnit2
			// 
			this.label_MaxSendRateIntervalUnit2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label_MaxSendRateIntervalUnit2.AutoSize = true;
			this.label_MaxSendRateIntervalUnit2.Location = new System.Drawing.Point(228, 66);
			this.label_MaxSendRateIntervalUnit2.Name = "label_MaxSendRateIntervalUnit2";
			this.label_MaxSendRateIntervalUnit2.Size = new System.Drawing.Size(20, 13);
			this.label_MaxSendRateIntervalUnit2.TabIndex = 10;
			this.label_MaxSendRateIntervalUnit2.Text = "ms";
			this.label_MaxSendRateIntervalUnit2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkBox_NoSendOnInputBreak
			// 
			this.checkBox_NoSendOnInputBreak.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkBox_NoSendOnInputBreak.AutoSize = true;
			this.checkBox_NoSendOnInputBreak.Location = new System.Drawing.Point(6, 111);
			this.checkBox_NoSendOnInputBreak.Name = "checkBox_NoSendOnInputBreak";
			this.checkBox_NoSendOnInputBreak.Size = new System.Drawing.Size(212, 17);
			this.checkBox_NoSendOnInputBreak.TabIndex = 12;
			this.checkBox_NoSendOnInputBreak.Text = "No send w&hile in input break state (IBS)";
			this.checkBox_NoSendOnInputBreak.UseVisualStyleBackColor = true;
			this.checkBox_NoSendOnInputBreak.CheckedChanged += new System.EventHandler(this.checkBox_NoSendOnInputBreak_CheckedChanged);
			// 
			// checkBox_NoSendOnOutputBreak
			// 
			this.checkBox_NoSendOnOutputBreak.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkBox_NoSendOnOutputBreak.AutoSize = true;
			this.checkBox_NoSendOnOutputBreak.Location = new System.Drawing.Point(6, 88);
			this.checkBox_NoSendOnOutputBreak.Name = "checkBox_NoSendOnOutputBreak";
			this.checkBox_NoSendOnOutputBreak.Size = new System.Drawing.Size(224, 17);
			this.checkBox_NoSendOnOutputBreak.TabIndex = 11;
			this.checkBox_NoSendOnOutputBreak.Text = "No send while in o&utput break state (OBS)";
			this.checkBox_NoSendOnOutputBreak.UseVisualStyleBackColor = true;
			this.checkBox_NoSendOnOutputBreak.CheckedChanged += new System.EventHandler(this.checkBox_NoSendOnOutputBreak_CheckedChanged);
			// 
			// textBox_SignalXOnPeriodicallyInterval
			// 
			this.textBox_SignalXOnPeriodicallyInterval.Location = new System.Drawing.Point(168, 136);
			this.textBox_SignalXOnPeriodicallyInterval.Name = "textBox_SignalXOnPeriodicallyInterval";
			this.textBox_SignalXOnPeriodicallyInterval.Size = new System.Drawing.Size(48, 20);
			this.textBox_SignalXOnPeriodicallyInterval.TabIndex = 5;
			this.textBox_SignalXOnPeriodicallyInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBox_SignalXOnPeriodicallyInterval.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_SignalXOnPeriodicallyInterval_Validating);
			// 
			// groupBox_Send_Keywords
			// 
			this.groupBox_Send_Keywords.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Send_Keywords.Controls.Add(this.label_DefaultLineIntervalUnit);
			this.groupBox_Send_Keywords.Controls.Add(this.textBox_DefaultLineInterval);
			this.groupBox_Send_Keywords.Controls.Add(this.label_DefaultLineInterval);
			this.groupBox_Send_Keywords.Controls.Add(this.label_DefaultLineRepeatUnit);
			this.groupBox_Send_Keywords.Controls.Add(this.textBox_DefaultLineRepeat);
			this.groupBox_Send_Keywords.Controls.Add(this.label_DefaultLineRepeat);
			this.groupBox_Send_Keywords.Controls.Add(this.label_DefaultLineDelayUnit);
			this.groupBox_Send_Keywords.Controls.Add(this.textBox_DefaultLineDelay);
			this.groupBox_Send_Keywords.Controls.Add(this.label_DefaultLineDelay);
			this.groupBox_Send_Keywords.Controls.Add(this.label_DefaultDelayUnit);
			this.groupBox_Send_Keywords.Controls.Add(this.textBox_DefaultDelay);
			this.groupBox_Send_Keywords.Controls.Add(this.label_DefaultDelay);
			this.groupBox_Send_Keywords.Controls.Add(this.checkBox_DisableKeywords);
			this.groupBox_Send_Keywords.Location = new System.Drawing.Point(6, 303);
			this.groupBox_Send_Keywords.Name = "groupBox_Send_Keywords";
			this.groupBox_Send_Keywords.Size = new System.Drawing.Size(251, 128);
			this.groupBox_Send_Keywords.TabIndex = 8;
			this.groupBox_Send_Keywords.TabStop = false;
			this.groupBox_Send_Keywords.Text = "Keywords";
			// 
			// label_DefaultLineIntervalUnit
			// 
			this.label_DefaultLineIntervalUnit.AutoSize = true;
			this.label_DefaultLineIntervalUnit.Location = new System.Drawing.Point(197, 62);
			this.label_DefaultLineIntervalUnit.Name = "label_DefaultLineIntervalUnit";
			this.label_DefaultLineIntervalUnit.Size = new System.Drawing.Size(20, 13);
			this.label_DefaultLineIntervalUnit.TabIndex = 8;
			this.label_DefaultLineIntervalUnit.Text = "ms";
			this.label_DefaultLineIntervalUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox_DefaultLineInterval
			// 
			this.textBox_DefaultLineInterval.Location = new System.Drawing.Point(147, 59);
			this.textBox_DefaultLineInterval.Name = "textBox_DefaultLineInterval";
			this.textBox_DefaultLineInterval.Size = new System.Drawing.Size(48, 20);
			this.textBox_DefaultLineInterval.TabIndex = 7;
			this.textBox_DefaultLineInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBox_DefaultLineInterval.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_DefaultLineInterval_Validating);
			// 
			// label_DefaultLineInterval
			// 
			this.label_DefaultLineInterval.AutoSize = true;
			this.label_DefaultLineInterval.Location = new System.Drawing.Point(6, 62);
			this.label_DefaultLineInterval.Name = "label_DefaultLineInterval";
			this.label_DefaultLineInterval.Size = new System.Drawing.Size(135, 13);
			this.label_DefaultLineInterval.TabIndex = 6;
			this.label_DefaultLineInterval.Text = "Default of \\!(&LineInterval) is\r\n";
			this.label_DefaultLineInterval.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label_DefaultLineRepeatUnit
			// 
			this.label_DefaultLineRepeatUnit.AutoSize = true;
			this.label_DefaultLineRepeatUnit.Location = new System.Drawing.Point(197, 83);
			this.label_DefaultLineRepeatUnit.Name = "label_DefaultLineRepeatUnit";
			this.label_DefaultLineRepeatUnit.Size = new System.Drawing.Size(31, 13);
			this.label_DefaultLineRepeatUnit.TabIndex = 11;
			this.label_DefaultLineRepeatUnit.Text = "times";
			this.label_DefaultLineRepeatUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox_DefaultLineRepeat
			// 
			this.textBox_DefaultLineRepeat.Location = new System.Drawing.Point(147, 80);
			this.textBox_DefaultLineRepeat.Name = "textBox_DefaultLineRepeat";
			this.textBox_DefaultLineRepeat.Size = new System.Drawing.Size(48, 20);
			this.textBox_DefaultLineRepeat.TabIndex = 10;
			this.textBox_DefaultLineRepeat.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.toolTip.SetToolTip(this.textBox_DefaultLineRepeat, "Set to -1 for infinite repeating");
			this.textBox_DefaultLineRepeat.TextChanged += new System.EventHandler(this.textBox_DefaultLineRepeat_TextChanged);
			this.textBox_DefaultLineRepeat.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_DefaultLineRepeat_Validating);
			// 
			// label_DefaultLineRepeat
			// 
			this.label_DefaultLineRepeat.AutoSize = true;
			this.label_DefaultLineRepeat.Location = new System.Drawing.Point(6, 83);
			this.label_DefaultLineRepeat.Name = "label_DefaultLineRepeat";
			this.label_DefaultLineRepeat.Size = new System.Drawing.Size(135, 13);
			this.label_DefaultLineRepeat.TabIndex = 9;
			this.label_DefaultLineRepeat.Text = "Default of \\!(Line&Repeat) is\r\n";
			this.label_DefaultLineRepeat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label_DefaultLineDelayUnit
			// 
			this.label_DefaultLineDelayUnit.AutoSize = true;
			this.label_DefaultLineDelayUnit.Location = new System.Drawing.Point(197, 41);
			this.label_DefaultLineDelayUnit.Name = "label_DefaultLineDelayUnit";
			this.label_DefaultLineDelayUnit.Size = new System.Drawing.Size(20, 13);
			this.label_DefaultLineDelayUnit.TabIndex = 5;
			this.label_DefaultLineDelayUnit.Text = "ms";
			this.label_DefaultLineDelayUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox_DefaultLineDelay
			// 
			this.textBox_DefaultLineDelay.Location = new System.Drawing.Point(147, 38);
			this.textBox_DefaultLineDelay.Name = "textBox_DefaultLineDelay";
			this.textBox_DefaultLineDelay.Size = new System.Drawing.Size(48, 20);
			this.textBox_DefaultLineDelay.TabIndex = 4;
			this.textBox_DefaultLineDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBox_DefaultLineDelay.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_DefaultLineDelay_Validating);
			// 
			// label_DefaultLineDelay
			// 
			this.label_DefaultLineDelay.AutoSize = true;
			this.label_DefaultLineDelay.Location = new System.Drawing.Point(6, 41);
			this.label_DefaultLineDelay.Name = "label_DefaultLineDelay";
			this.label_DefaultLineDelay.Size = new System.Drawing.Size(127, 13);
			this.label_DefaultLineDelay.TabIndex = 3;
			this.label_DefaultLineDelay.Text = "Default of \\!(&LineDelay) is\r\n";
			this.label_DefaultLineDelay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label_DefaultDelayUnit
			// 
			this.label_DefaultDelayUnit.AutoSize = true;
			this.label_DefaultDelayUnit.Location = new System.Drawing.Point(197, 20);
			this.label_DefaultDelayUnit.Name = "label_DefaultDelayUnit";
			this.label_DefaultDelayUnit.Size = new System.Drawing.Size(20, 13);
			this.label_DefaultDelayUnit.TabIndex = 2;
			this.label_DefaultDelayUnit.Text = "ms";
			this.label_DefaultDelayUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox_DefaultDelay
			// 
			this.textBox_DefaultDelay.Location = new System.Drawing.Point(147, 17);
			this.textBox_DefaultDelay.Name = "textBox_DefaultDelay";
			this.textBox_DefaultDelay.Size = new System.Drawing.Size(48, 20);
			this.textBox_DefaultDelay.TabIndex = 1;
			this.textBox_DefaultDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBox_DefaultDelay.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_DefaultDelay_Validating);
			// 
			// label_DefaultDelay
			// 
			this.label_DefaultDelay.AutoSize = true;
			this.label_DefaultDelay.Location = new System.Drawing.Point(6, 20);
			this.label_DefaultDelay.Name = "label_DefaultDelay";
			this.label_DefaultDelay.Size = new System.Drawing.Size(107, 13);
			this.label_DefaultDelay.TabIndex = 0;
			this.label_DefaultDelay.Text = "Default of \\!(&Delay) is\r\n";
			this.label_DefaultDelay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkBox_DisableKeywords
			// 
			this.checkBox_DisableKeywords.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkBox_DisableKeywords.AutoSize = true;
			this.checkBox_DisableKeywords.Location = new System.Drawing.Point(6, 105);
			this.checkBox_DisableKeywords.Name = "checkBox_DisableKeywords";
			this.checkBox_DisableKeywords.Size = new System.Drawing.Size(148, 17);
			this.checkBox_DisableKeywords.TabIndex = 12;
			this.checkBox_DisableKeywords.Text = "&Disable all \\!(...) keywords";
			this.checkBox_DisableKeywords.UseVisualStyleBackColor = true;
			this.checkBox_DisableKeywords.CheckedChanged += new System.EventHandler(this.checkBox_DisableKeywords_CheckedChanged);
			// 
			// checkBox_SignalXOnPeriodicallyEnable
			// 
			this.checkBox_SignalXOnPeriodicallyEnable.AutoSize = true;
			this.checkBox_SignalXOnPeriodicallyEnable.Location = new System.Drawing.Point(12, 138);
			this.checkBox_SignalXOnPeriodicallyEnable.Name = "checkBox_SignalXOnPeriodicallyEnable";
			this.checkBox_SignalXOnPeriodicallyEnable.Size = new System.Drawing.Size(159, 17);
			this.checkBox_SignalXOnPeriodicallyEnable.TabIndex = 4;
			this.checkBox_SignalXOnPeriodicallyEnable.Text = "Send XOn &periodically every";
			this.checkBox_SignalXOnPeriodicallyEnable.UseVisualStyleBackColor = true;
			this.checkBox_SignalXOnPeriodicallyEnable.CheckedChanged += new System.EventHandler(this.checkBox_SignalXOnPeriodicallyEnable_CheckedChanged);
			// 
			// checkBox_SignalXOnBeforeEachTransmission
			// 
			this.checkBox_SignalXOnBeforeEachTransmission.AutoSize = true;
			this.checkBox_SignalXOnBeforeEachTransmission.Location = new System.Drawing.Point(12, 115);
			this.checkBox_SignalXOnBeforeEachTransmission.Name = "checkBox_SignalXOnBeforeEachTransmission";
			this.checkBox_SignalXOnBeforeEachTransmission.Size = new System.Drawing.Size(195, 17);
			this.checkBox_SignalXOnBeforeEachTransmission.TabIndex = 3;
			this.checkBox_SignalXOnBeforeEachTransmission.Text = "Send XOn before each &transmission";
			this.checkBox_SignalXOnBeforeEachTransmission.UseVisualStyleBackColor = true;
			this.checkBox_SignalXOnBeforeEachTransmission.CheckedChanged += new System.EventHandler(this.checkBox_SignalXOnBeforeEachTransmission_CheckedChanged);
			// 
			// checkBox_SendImmediately
			// 
			this.checkBox_SendImmediately.AutoSize = true;
			this.checkBox_SendImmediately.Location = new System.Drawing.Point(12, 92);
			this.checkBox_SendImmediately.Name = "checkBox_SendImmediately";
			this.checkBox_SendImmediately.Size = new System.Drawing.Size(222, 17);
			this.checkBox_SendImmediately.TabIndex = 2;
			this.checkBox_SendImmediately.Text = "Send each entered character &immediately";
			this.toolTip.SetToolTip(this.checkBox_SendImmediately, "Emulates a terminal/direct mode.");
			this.checkBox_SendImmediately.UseVisualStyleBackColor = true;
			this.checkBox_SendImmediately.CheckedChanged += new System.EventHandler(this.checkBox_SendImmediately_CheckedChanged);
			// 
			// checkBox_CopyPredefined
			// 
			this.checkBox_CopyPredefined.AutoSize = true;
			this.checkBox_CopyPredefined.Location = new System.Drawing.Point(12, 69);
			this.checkBox_CopyPredefined.Name = "checkBox_CopyPredefined";
			this.checkBox_CopyPredefined.Size = new System.Drawing.Size(223, 17);
			this.checkBox_CopyPredefined.TabIndex = 1;
			this.checkBox_CopyPredefined.Text = "Cop&y predefined to [Send Text] after send";
			this.checkBox_CopyPredefined.UseVisualStyleBackColor = true;
			this.checkBox_CopyPredefined.CheckedChanged += new System.EventHandler(this.checkBox_CopyPredefined_CheckedChanged);
			// 
			// checkBox_KeepCommand
			// 
			this.checkBox_KeepCommand.AutoSize = true;
			this.checkBox_KeepCommand.Location = new System.Drawing.Point(12, 46);
			this.checkBox_KeepCommand.Name = "checkBox_KeepCommand";
			this.checkBox_KeepCommand.Size = new System.Drawing.Size(219, 17);
			this.checkBox_KeepCommand.TabIndex = 0;
			this.checkBox_KeepCommand.Text = "&Keep command in [Send Text] after send";
			this.checkBox_KeepCommand.UseVisualStyleBackColor = true;
			this.checkBox_KeepCommand.CheckedChanged += new System.EventHandler(this.checkBox_KeepCommand_CheckedChanged);
			// 
			// groupBox_Display
			// 
			this.groupBox_Display.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox_Display.Controls.Add(this.textBox_MaxBytePerLineCount);
			this.groupBox_Display.Controls.Add(this.label_MaxBytePerLineCountUnit);
			this.groupBox_Display.Controls.Add(this.label_MaxBytePerLineCount);
			this.groupBox_Display.Controls.Add(this.checkBox_PortLineBreak);
			this.groupBox_Display.Controls.Add(this.checkBox_ShowPort);
			this.groupBox_Display.Controls.Add(this.textBox_MaxLineCount);
			this.groupBox_Display.Controls.Add(this.groupBox_Display_Special);
			this.groupBox_Display.Controls.Add(this.checkBox_ShowDirection);
			this.groupBox_Display.Controls.Add(this.checkBox_ShowBreakCount);
			this.groupBox_Display.Controls.Add(this.checkBox_ShowFlowControlCount);
			this.groupBox_Display.Controls.Add(this.checkBox_ShowLineNumbers);
			this.groupBox_Display.Controls.Add(this.groupBox_Display_Space);
			this.groupBox_Display.Controls.Add(this.groupBox_Display_ControlChars);
			this.groupBox_Display.Controls.Add(this.checkBox_DirectionLineBreak);
			this.groupBox_Display.Controls.Add(this.comboBox_RxRadix);
			this.groupBox_Display.Controls.Add(this.label_RxRadix);
			this.groupBox_Display.Controls.Add(this.checkBox_SeparateTxRxRadix);
			this.groupBox_Display.Controls.Add(this.checkBox_ShowRadix);
			this.groupBox_Display.Controls.Add(this.checkBox_ShowDate);
			this.groupBox_Display.Controls.Add(this.checkBox_ShowTime);
			this.groupBox_Display.Controls.Add(this.checkBox_ShowConnectTime);
			this.groupBox_Display.Controls.Add(this.checkBox_ShowCountAndRate);
			this.groupBox_Display.Controls.Add(this.checkBox_ShowLength);
			this.groupBox_Display.Controls.Add(this.comboBox_TxRadix);
			this.groupBox_Display.Controls.Add(this.label_TxRadix);
			this.groupBox_Display.Controls.Add(this.label_MaxLineCountUnit);
			this.groupBox_Display.Controls.Add(this.label_MaxLineCount);
			this.groupBox_Display.Location = new System.Drawing.Point(6, 13);
			this.groupBox_Display.Name = "groupBox_Display";
			this.groupBox_Display.Size = new System.Drawing.Size(263, 618);
			this.groupBox_Display.TabIndex = 0;
			this.groupBox_Display.TabStop = false;
			this.groupBox_Display.Text = "Display Settings";
			// 
			// textBox_MaxBytePerLineCount
			// 
			this.textBox_MaxBytePerLineCount.Location = new System.Drawing.Point(90, 351);
			this.textBox_MaxBytePerLineCount.Name = "textBox_MaxBytePerLineCount";
			this.textBox_MaxBytePerLineCount.Size = new System.Drawing.Size(48, 20);
			this.textBox_MaxBytePerLineCount.TabIndex = 22;
			this.textBox_MaxBytePerLineCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.toolTip.SetToolTip(this.textBox_MaxBytePerLineCount, "The maximal number of bytes per line is limited in order to improve performance.");
			this.textBox_MaxBytePerLineCount.TextChanged += new System.EventHandler(this.textBox_MaxBytePerLineCount_TextChanged);
			this.textBox_MaxBytePerLineCount.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_MaxBytePerLineCount_Validating);
			// 
			// label_MaxBytePerLineCountUnit
			// 
			this.label_MaxBytePerLineCountUnit.AutoSize = true;
			this.label_MaxBytePerLineCountUnit.Location = new System.Drawing.Point(141, 354);
			this.label_MaxBytePerLineCountUnit.Name = "label_MaxBytePerLineCountUnit";
			this.label_MaxBytePerLineCountUnit.Size = new System.Drawing.Size(69, 13);
			this.label_MaxBytePerLineCountUnit.TabIndex = 23;
			this.label_MaxBytePerLineCountUnit.Text = "bytes per line";
			// 
			// label_MaxBytePerLineCount
			// 
			this.label_MaxBytePerLineCount.AutoSize = true;
			this.label_MaxBytePerLineCount.Location = new System.Drawing.Point(9, 354);
			this.label_MaxBytePerLineCount.Name = "label_MaxBytePerLineCount";
			this.label_MaxBytePerLineCount.Size = new System.Drawing.Size(81, 13);
			this.label_MaxBytePerLineCount.TabIndex = 21;
			this.label_MaxBytePerLineCount.Text = "Displa&y maximal";
			// 
			// checkBox_PortLineBreak
			// 
			this.checkBox_PortLineBreak.AutoSize = true;
			this.checkBox_PortLineBreak.Location = new System.Drawing.Point(12, 264);
			this.checkBox_PortLineBreak.Name = "checkBox_PortLineBreak";
			this.checkBox_PortLineBreak.Size = new System.Drawing.Size(172, 17);
			this.checkBox_PortLineBreak.TabIndex = 16;
			this.checkBox_PortLineBreak.Text = "Break lines when p&ort changes";
			this.checkBox_PortLineBreak.CheckedChanged += new System.EventHandler(this.checkBox_PortLineBreak_CheckedChanged);
			// 
			// checkBox_ShowPort
			// 
			this.checkBox_ShowPort.AutoSize = true;
			this.checkBox_ShowPort.Location = new System.Drawing.Point(12, 195);
			this.checkBox_ShowPort.Name = "checkBox_ShowPort";
			this.checkBox_ShowPort.Size = new System.Drawing.Size(74, 17);
			this.checkBox_ShowPort.TabIndex = 11;
			this.checkBox_ShowPort.Text = "Show &port";
			this.checkBox_ShowPort.UseVisualStyleBackColor = true;
			this.checkBox_ShowPort.CheckedChanged += new System.EventHandler(this.checkBox_ShowPort_CheckedChanged);
			// 
			// textBox_MaxLineCount
			// 
			this.textBox_MaxLineCount.Location = new System.Drawing.Point(90, 328);
			this.textBox_MaxLineCount.Name = "textBox_MaxLineCount";
			this.textBox_MaxLineCount.Size = new System.Drawing.Size(48, 20);
			this.textBox_MaxLineCount.TabIndex = 19;
			this.textBox_MaxLineCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.toolTip.SetToolTip(this.textBox_MaxLineCount, "The maximal number of lines is limited in order to improve performance.");
			this.textBox_MaxLineCount.TextChanged += new System.EventHandler(this.textBox_MaxLineCount_TextChanged);
			this.textBox_MaxLineCount.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_MaxLineCount_Validating);
			// 
			// groupBox_Display_Special
			// 
			this.groupBox_Display_Special.Controls.Add(this.checkBox_Hide0xFF);
			this.groupBox_Display_Special.Controls.Add(this.checkBox_Hide0x00);
			this.groupBox_Display_Special.Location = new System.Drawing.Point(6, 569);
			this.groupBox_Display_Special.Name = "groupBox_Display_Special";
			this.groupBox_Display_Special.Size = new System.Drawing.Size(251, 43);
			this.groupBox_Display_Special.TabIndex = 26;
			this.groupBox_Display_Special.TabStop = false;
			this.groupBox_Display_Special.Text = "Special";
			// 
			// checkBox_Hide0xFF
			// 
			this.checkBox_Hide0xFF.AutoSize = true;
			this.checkBox_Hide0xFF.Location = new System.Drawing.Point(125, 19);
			this.checkBox_Hide0xFF.Name = "checkBox_Hide0xFF";
			this.checkBox_Hide0xFF.Size = new System.Drawing.Size(74, 17);
			this.checkBox_Hide0xFF.TabIndex = 1;
			this.checkBox_Hide0xFF.Text = "Hide 0x&FF";
			this.toolTip.SetToolTip(this.checkBox_Hide0xFF, "For text terminals, this setting is only supported if encoding is single byte (e." +
        "g. ASCII or Windows code pages).\r\nFor binary terminals, this setting is always s" +
        "upported.");
			this.checkBox_Hide0xFF.UseVisualStyleBackColor = true;
			this.checkBox_Hide0xFF.CheckedChanged += new System.EventHandler(this.checkBox_Hide0xFF_CheckedChanged);
			// 
			// checkBox_Hide0x00
			// 
			this.checkBox_Hide0x00.AutoSize = true;
			this.checkBox_Hide0x00.Location = new System.Drawing.Point(6, 19);
			this.checkBox_Hide0x00.Name = "checkBox_Hide0x00";
			this.checkBox_Hide0x00.Size = new System.Drawing.Size(74, 17);
			this.checkBox_Hide0x00.TabIndex = 0;
			this.checkBox_Hide0x00.Text = "Hide 0x&00";
			this.checkBox_Hide0x00.UseVisualStyleBackColor = true;
			this.checkBox_Hide0x00.CheckedChanged += new System.EventHandler(this.checkBox_Hide0x00_CheckedChanged);
			// 
			// checkBox_ShowDirection
			// 
			this.checkBox_ShowDirection.AutoSize = true;
			this.checkBox_ShowDirection.Location = new System.Drawing.Point(131, 218);
			this.checkBox_ShowDirection.Name = "checkBox_ShowDirection";
			this.checkBox_ShowDirection.Size = new System.Drawing.Size(96, 17);
			this.checkBox_ShowDirection.TabIndex = 12;
			this.checkBox_ShowDirection.Text = "Show &direction";
			this.checkBox_ShowDirection.UseVisualStyleBackColor = true;
			this.checkBox_ShowDirection.CheckedChanged += new System.EventHandler(this.checkBox_ShowDirection_CheckedChanged);
			// 
			// checkBox_ShowBreakCount
			// 
			this.checkBox_ShowBreakCount.AutoSize = true;
			this.checkBox_ShowBreakCount.Location = new System.Drawing.Point(131, 241);
			this.checkBox_ShowBreakCount.Name = "checkBox_ShowBreakCount";
			this.checkBox_ShowBreakCount.Size = new System.Drawing.Size(113, 17);
			this.checkBox_ShowBreakCount.TabIndex = 15;
			this.checkBox_ShowBreakCount.Text = "Show brea&k count";
			this.checkBox_ShowBreakCount.UseVisualStyleBackColor = true;
			this.checkBox_ShowBreakCount.CheckedChanged += new System.EventHandler(this.checkBox_ShowBreakCount_CheckedChanged);
			// 
			// checkBox_ShowFlowControlCount
			// 
			this.checkBox_ShowFlowControlCount.AutoSize = true;
			this.checkBox_ShowFlowControlCount.Location = new System.Drawing.Point(12, 241);
			this.checkBox_ShowFlowControlCount.Name = "checkBox_ShowFlowControlCount";
			this.checkBox_ShowFlowControlCount.Size = new System.Drawing.Size(122, 17);
			this.checkBox_ShowFlowControlCount.TabIndex = 14;
			this.checkBox_ShowFlowControlCount.Text = "Show flo&w ctrl count";
			this.checkBox_ShowFlowControlCount.UseVisualStyleBackColor = true;
			this.checkBox_ShowFlowControlCount.CheckedChanged += new System.EventHandler(this.checkBox_ShowFlowControlCount_CheckedChanged);
			// 
			// checkBox_ShowLineNumbers
			// 
			this.checkBox_ShowLineNumbers.AutoSize = true;
			this.checkBox_ShowLineNumbers.Location = new System.Drawing.Point(12, 149);
			this.checkBox_ShowLineNumbers.Name = "checkBox_ShowLineNumbers";
			this.checkBox_ShowLineNumbers.Size = new System.Drawing.Size(115, 17);
			this.checkBox_ShowLineNumbers.TabIndex = 8;
			this.checkBox_ShowLineNumbers.Text = "Show line n&umbers";
			this.checkBox_ShowLineNumbers.UseVisualStyleBackColor = true;
			this.checkBox_ShowLineNumbers.CheckedChanged += new System.EventHandler(this.checkBox_ShowLineNumbers_CheckedChanged);
			// 
			// groupBox_Display_Space
			// 
			this.groupBox_Display_Space.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Display_Space.Controls.Add(this.label_SpaceReplacementChar);
			this.groupBox_Display_Space.Controls.Add(this.label_ReplaceSpaceUnicode);
			this.groupBox_Display_Space.Controls.Add(this.checkBox_ReplaceSpace);
			this.groupBox_Display_Space.Location = new System.Drawing.Point(6, 520);
			this.groupBox_Display_Space.Name = "groupBox_Display_Space";
			this.groupBox_Display_Space.Size = new System.Drawing.Size(251, 43);
			this.groupBox_Display_Space.TabIndex = 25;
			this.groupBox_Display_Space.TabStop = false;
			this.groupBox_Display_Space.Text = "Space";
			// 
			// label_SpaceReplacementChar
			// 
			this.label_SpaceReplacementChar.AutoSize = true;
			this.label_SpaceReplacementChar.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label_SpaceReplacementChar.Font = new System.Drawing.Font("DejaVu Sans Mono", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_SpaceReplacementChar.Location = new System.Drawing.Point(179, 21);
			this.label_SpaceReplacementChar.Name = "label_SpaceReplacementChar";
			this.label_SpaceReplacementChar.Size = new System.Drawing.Size(16, 15);
			this.label_SpaceReplacementChar.TabIndex = 1;
			this.label_SpaceReplacementChar.Text = "␣";
			// 
			// label_ReplaceSpaceUnicode
			// 
			this.label_ReplaceSpaceUnicode.AutoSize = true;
			this.label_ReplaceSpaceUnicode.Location = new System.Drawing.Point(196, 20);
			this.label_ReplaceSpaceUnicode.Name = "label_ReplaceSpaceUnicode";
			this.label_ReplaceSpaceUnicode.Size = new System.Drawing.Size(51, 13);
			this.label_ReplaceSpaceUnicode.TabIndex = 2;
			this.label_ReplaceSpaceUnicode.Text = "(U+2423)";
			// 
			// checkBox_ReplaceSpace
			// 
			this.checkBox_ReplaceSpace.AutoSize = true;
			this.checkBox_ReplaceSpace.Location = new System.Drawing.Point(6, 19);
			this.checkBox_ReplaceSpace.Name = "checkBox_ReplaceSpace";
			this.checkBox_ReplaceSpace.Size = new System.Drawing.Size(175, 17);
			this.checkBox_ReplaceSpace.TabIndex = 0;
			this.checkBox_ReplaceSpace.Text = "Replace by &open box character";
			this.checkBox_ReplaceSpace.UseVisualStyleBackColor = true;
			this.checkBox_ReplaceSpace.CheckedChanged += new System.EventHandler(this.checkBox_ReplaceSpace_CheckedChanged);
			// 
			// groupBox_Display_ControlChars
			// 
			this.groupBox_Display_ControlChars.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Display_ControlChars.Controls.Add(this.label_ReplaceTab);
			this.groupBox_Display_ControlChars.Controls.Add(this.checkBox_HideXOnXOff);
			this.groupBox_Display_ControlChars.Controls.Add(this.comboBox_ControlCharacterRadix);
			this.groupBox_Display_ControlChars.Controls.Add(this.checkBox_ReplaceTab);
			this.groupBox_Display_ControlChars.Controls.Add(this.label_ControlCharacterRadix);
			this.groupBox_Display_ControlChars.Controls.Add(this.checkBox_ReplaceControlCharacters);
			this.groupBox_Display_ControlChars.Location = new System.Drawing.Point(6, 402);
			this.groupBox_Display_ControlChars.Name = "groupBox_Display_ControlChars";
			this.groupBox_Display_ControlChars.Size = new System.Drawing.Size(251, 112);
			this.groupBox_Display_ControlChars.TabIndex = 24;
			this.groupBox_Display_ControlChars.TabStop = false;
			this.groupBox_Display_ControlChars.Text = "ASCII Control Characters (0x00..0x1F, 0x7F)";
			// 
			// label_ReplaceTab
			// 
			this.label_ReplaceTab.AutoSize = true;
			this.label_ReplaceTab.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label_ReplaceTab.Font = new System.Drawing.Font("DejaVu Sans Mono", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_ReplaceTab.Location = new System.Drawing.Point(179, 67);
			this.label_ReplaceTab.Name = "label_ReplaceTab";
			this.label_ReplaceTab.Size = new System.Drawing.Size(44, 15);
			this.label_ReplaceTab.TabIndex = 1;
			this.label_ReplaceTab.Text = "<TAB>";
			// 
			// checkBox_HideXOnXOff
			// 
			this.checkBox_HideXOnXOff.AutoSize = true;
			this.checkBox_HideXOnXOff.Location = new System.Drawing.Point(6, 88);
			this.checkBox_HideXOnXOff.Name = "checkBox_HideXOnXOff";
			this.checkBox_HideXOnXOff.Size = new System.Drawing.Size(158, 17);
			this.checkBox_HideXOnXOff.TabIndex = 0;
			this.checkBox_HideXOnXOff.Text = "&Hide XOn/XOff (0x11/0x13)";
			this.checkBox_HideXOnXOff.UseVisualStyleBackColor = true;
			this.checkBox_HideXOnXOff.CheckedChanged += new System.EventHandler(this.checkBox_HideXOnXOff_CheckedChanged);
			// 
			// comboBox_ControlCharacterRadix
			// 
			this.comboBox_ControlCharacterRadix.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_ControlCharacterRadix.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_ControlCharacterRadix.Location = new System.Drawing.Point(125, 40);
			this.comboBox_ControlCharacterRadix.Name = "comboBox_ControlCharacterRadix";
			this.comboBox_ControlCharacterRadix.Size = new System.Drawing.Size(121, 21);
			this.comboBox_ControlCharacterRadix.TabIndex = 2;
			this.comboBox_ControlCharacterRadix.SelectedIndexChanged += new System.EventHandler(this.comboBox_ControlCharacterRadix_SelectedIndexChanged);
			// 
			// checkBox_ReplaceTab
			// 
			this.checkBox_ReplaceTab.AutoSize = true;
			this.checkBox_ReplaceTab.Location = new System.Drawing.Point(6, 65);
			this.checkBox_ReplaceTab.Name = "checkBox_ReplaceTab";
			this.checkBox_ReplaceTab.Size = new System.Drawing.Size(178, 17);
			this.checkBox_ReplaceTab.TabIndex = 0;
			this.checkBox_ReplaceTab.Text = "Replace horizontal &tab (0x09) by";
			this.checkBox_ReplaceTab.UseVisualStyleBackColor = true;
			this.checkBox_ReplaceTab.CheckedChanged += new System.EventHandler(this.checkBox_ReplaceTab_CheckedChanged);
			// 
			// label_ControlCharacterRadix
			// 
			this.label_ControlCharacterRadix.AutoSize = true;
			this.label_ControlCharacterRadix.Location = new System.Drawing.Point(3, 43);
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
			this.checkBox_ReplaceControlCharacters.Size = new System.Drawing.Size(167, 17);
			this.checkBox_ReplaceControlCharacters.TabIndex = 0;
			this.checkBox_ReplaceControlCharacters.Text = "Re&place all control characters";
			this.checkBox_ReplaceControlCharacters.CheckedChanged += new System.EventHandler(this.checkBox_ReplaceControlCharacters_CheckedChanged);
			// 
			// checkBox_DirectionLineBreak
			// 
			this.checkBox_DirectionLineBreak.AutoSize = true;
			this.checkBox_DirectionLineBreak.Location = new System.Drawing.Point(12, 287);
			this.checkBox_DirectionLineBreak.Name = "checkBox_DirectionLineBreak";
			this.checkBox_DirectionLineBreak.Size = new System.Drawing.Size(194, 17);
			this.checkBox_DirectionLineBreak.TabIndex = 17;
			this.checkBox_DirectionLineBreak.Text = "Break lines when d&irection changes";
			this.checkBox_DirectionLineBreak.CheckedChanged += new System.EventHandler(this.checkBox_DirectionLineBreak_CheckedChanged);
			// 
			// comboBox_RxRadix
			// 
			this.comboBox_RxRadix.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_RxRadix.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_RxRadix.Location = new System.Drawing.Point(131, 95);
			this.comboBox_RxRadix.Name = "comboBox_RxRadix";
			this.comboBox_RxRadix.Size = new System.Drawing.Size(121, 21);
			this.comboBox_RxRadix.TabIndex = 6;
			this.comboBox_RxRadix.SelectedIndexChanged += new System.EventHandler(this.comboBox_RxRadix_SelectedIndexChanged);
			// 
			// label_RxRadix
			// 
			this.label_RxRadix.AutoSize = true;
			this.label_RxRadix.Location = new System.Drawing.Point(9, 98);
			this.label_RxRadix.Name = "label_RxRadix";
			this.label_RxRadix.Size = new System.Drawing.Size(53, 13);
			this.label_RxRadix.TabIndex = 5;
			this.label_RxRadix.Text = "R&x Radix:";
			// 
			// checkBox_SeparateTxRxRadix
			// 
			this.checkBox_SeparateTxRxRadix.AutoSize = true;
			this.checkBox_SeparateTxRxRadix.Location = new System.Drawing.Point(12, 72);
			this.checkBox_SeparateTxRxRadix.Name = "checkBox_SeparateTxRxRadix";
			this.checkBox_SeparateTxRxRadix.Size = new System.Drawing.Size(161, 17);
			this.checkBox_SeparateTxRxRadix.TabIndex = 4;
			this.checkBox_SeparateTxRxRadix.Text = "&Separate radix for Tx and Rx";
			this.checkBox_SeparateTxRxRadix.UseVisualStyleBackColor = true;
			this.checkBox_SeparateTxRxRadix.CheckedChanged += new System.EventHandler(this.checkBox_SeparateTxRxRadix_CheckedChanged);
			// 
			// checkBox_ShowRadix
			// 
			this.checkBox_ShowRadix.AutoSize = true;
			this.checkBox_ShowRadix.Location = new System.Drawing.Point(12, 126);
			this.checkBox_ShowRadix.Name = "checkBox_ShowRadix";
			this.checkBox_ShowRadix.Size = new System.Drawing.Size(78, 17);
			this.checkBox_ShowRadix.TabIndex = 7;
			this.checkBox_ShowRadix.Text = "Show &radix";
			this.checkBox_ShowRadix.CheckedChanged += new System.EventHandler(this.checkBox_ShowRadix_CheckedChanged);
			// 
			// checkBox_ShowDate
			// 
			this.checkBox_ShowDate.AutoSize = true;
			this.checkBox_ShowDate.Location = new System.Drawing.Point(12, 172);
			this.checkBox_ShowDate.Name = "checkBox_ShowDate";
			this.checkBox_ShowDate.Size = new System.Drawing.Size(77, 17);
			this.checkBox_ShowDate.TabIndex = 9;
			this.checkBox_ShowDate.Text = "Show d&ate";
			this.checkBox_ShowDate.CheckedChanged += new System.EventHandler(this.checkBox_ShowDate_CheckedChanged);
			// 
			// checkBox_ShowTime
			// 
			this.checkBox_ShowTime.AutoSize = true;
			this.checkBox_ShowTime.Location = new System.Drawing.Point(131, 172);
			this.checkBox_ShowTime.Name = "checkBox_ShowTime";
			this.checkBox_ShowTime.Size = new System.Drawing.Size(75, 17);
			this.checkBox_ShowTime.TabIndex = 10;
			this.checkBox_ShowTime.Text = "Show &time";
			this.checkBox_ShowTime.CheckedChanged += new System.EventHandler(this.checkBox_ShowTime_CheckedChanged);
			// 
			// checkBox_ShowConnectTime
			// 
			this.checkBox_ShowConnectTime.AutoSize = true;
			this.checkBox_ShowConnectTime.Location = new System.Drawing.Point(12, 20);
			this.checkBox_ShowConnectTime.Name = "checkBox_ShowConnectTime";
			this.checkBox_ShowConnectTime.Size = new System.Drawing.Size(117, 17);
			this.checkBox_ShowConnectTime.TabIndex = 0;
			this.checkBox_ShowConnectTime.Text = "Show connect t&ime";
			this.checkBox_ShowConnectTime.CheckedChanged += new System.EventHandler(this.checkBox_ShowConnectTime_CheckedChanged);
			// 
			// checkBox_ShowCountAndRate
			// 
			this.checkBox_ShowCountAndRate.AutoSize = true;
			this.checkBox_ShowCountAndRate.Location = new System.Drawing.Point(131, 20);
			this.checkBox_ShowCountAndRate.Name = "checkBox_ShowCountAndRate";
			this.checkBox_ShowCountAndRate.Size = new System.Drawing.Size(125, 17);
			this.checkBox_ShowCountAndRate.TabIndex = 1;
			this.checkBox_ShowCountAndRate.Text = "Show &count and rate";
			this.checkBox_ShowCountAndRate.CheckedChanged += new System.EventHandler(this.checkBox_ShowCountAndRate_CheckedChanged);
			// 
			// checkBox_ShowLength
			// 
			this.checkBox_ShowLength.AutoSize = true;
			this.checkBox_ShowLength.Location = new System.Drawing.Point(12, 218);
			this.checkBox_ShowLength.Name = "checkBox_ShowLength";
			this.checkBox_ShowLength.Size = new System.Drawing.Size(85, 17);
			this.checkBox_ShowLength.TabIndex = 13;
			this.checkBox_ShowLength.Text = "Show &length";
			this.checkBox_ShowLength.CheckedChanged += new System.EventHandler(this.checkBox_ShowLength_CheckedChanged);
			// 
			// comboBox_TxRadix
			// 
			this.comboBox_TxRadix.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_TxRadix.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_TxRadix.Location = new System.Drawing.Point(131, 45);
			this.comboBox_TxRadix.Name = "comboBox_TxRadix";
			this.comboBox_TxRadix.Size = new System.Drawing.Size(121, 21);
			this.comboBox_TxRadix.TabIndex = 3;
			this.comboBox_TxRadix.SelectedIndexChanged += new System.EventHandler(this.comboBox_TxRadix_SelectedIndexChanged);
			// 
			// label_TxRadix
			// 
			this.label_TxRadix.AutoSize = true;
			this.label_TxRadix.Location = new System.Drawing.Point(9, 48);
			this.label_TxRadix.Name = "label_TxRadix";
			this.label_TxRadix.Size = new System.Drawing.Size(37, 13);
			this.label_TxRadix.TabIndex = 2;
			this.label_TxRadix.Text = "R&adix:";
			// 
			// label_MaxLineCountUnit
			// 
			this.label_MaxLineCountUnit.AutoSize = true;
			this.label_MaxLineCountUnit.Location = new System.Drawing.Point(141, 331);
			this.label_MaxLineCountUnit.Name = "label_MaxLineCountUnit";
			this.label_MaxLineCountUnit.Size = new System.Drawing.Size(28, 13);
			this.label_MaxLineCountUnit.TabIndex = 20;
			this.label_MaxLineCountUnit.Text = "lines";
			this.label_MaxLineCountUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label_MaxLineCount
			// 
			this.label_MaxLineCount.AutoSize = true;
			this.label_MaxLineCount.Location = new System.Drawing.Point(9, 331);
			this.label_MaxLineCount.Name = "label_MaxLineCount";
			this.label_MaxLineCount.Size = new System.Drawing.Size(81, 13);
			this.label_MaxLineCount.TabIndex = 18;
			this.label_MaxLineCount.Text = "Display &maximal";
			this.label_MaxLineCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkBox_UseExplicitDefaultRadix
			// 
			this.checkBox_UseExplicitDefaultRadix.AutoSize = true;
			this.checkBox_UseExplicitDefaultRadix.Location = new System.Drawing.Point(12, 23);
			this.checkBox_UseExplicitDefaultRadix.Name = "checkBox_UseExplicitDefaultRadix";
			this.checkBox_UseExplicitDefaultRadix.Size = new System.Drawing.Size(140, 17);
			this.checkBox_UseExplicitDefaultRadix.TabIndex = 9;
			this.checkBox_UseExplicitDefaultRadix.Text = "&Use explicit default radix";
			this.toolTip.SetToolTip(this.checkBox_UseExplicitDefaultRadix, "Applies to the [Send Text] and [Send File] commands.\r\nWhen enabled, the default r" +
        "adix can explicitly be selected.\r\nWhen disabled, the default radix is \'String\'.");
			this.checkBox_UseExplicitDefaultRadix.UseVisualStyleBackColor = true;
			this.checkBox_UseExplicitDefaultRadix.CheckedChanged += new System.EventHandler(this.checkBox_UseExplicitDefaultRadix_CheckedChanged);
			// 
			// AdvancedTerminalSettings
			// 
			this.AcceptButton = this.button_OK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button_Cancel;
			this.ClientSize = new System.Drawing.Size(652, 661);
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
			this.groupBox_Communication_SerialPorts.ResumeLayout(false);
			this.groupBox_Communication_SerialPorts.PerformLayout();
			this.groupBox_Send.ResumeLayout(false);
			this.groupBox_Send.PerformLayout();
			this.groupBox_Send_SerialPorts.ResumeLayout(false);
			this.groupBox_Send_SerialPorts.PerformLayout();
			this.groupBox_Send_Keywords.ResumeLayout(false);
			this.groupBox_Send_Keywords.PerformLayout();
			this.groupBox_Display.ResumeLayout(false);
			this.groupBox_Display.PerformLayout();
			this.groupBox_Display_Special.ResumeLayout(false);
			this.groupBox_Display_Special.PerformLayout();
			this.groupBox_Display_Space.ResumeLayout(false);
			this.groupBox_Display_Space.PerformLayout();
			this.groupBox_Display_ControlChars.ResumeLayout(false);
			this.groupBox_Display_ControlChars.PerformLayout();
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
		private System.Windows.Forms.CheckBox checkBox_ShowDate;
		private System.Windows.Forms.CheckBox checkBox_ShowTime;
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
		private System.Windows.Forms.CheckBox checkBox_SendImmediately;
		private System.Windows.Forms.GroupBox groupBox_User;
		private System.Windows.Forms.TextBox textBox_UserName;
		private System.Windows.Forms.Label label_UserName;
		private System.Windows.Forms.CheckBox checkBox_NoSendOnInputBreak;
		private System.Windows.Forms.CheckBox checkBox_NoSendOnOutputBreak;
		private System.Windows.Forms.GroupBox groupBox_Communication_SerialPorts;
		private System.Windows.Forms.CheckBox checkBox_OutputBreakModifiable;
		private System.Windows.Forms.CheckBox checkBox_IndicateBreakStates;
		private System.Windows.Forms.CheckBox checkBox_ShowLineNumbers;
		private System.Windows.Forms.TextBox textBox_OutputBufferSize;
		private System.Windows.Forms.Label label_OutputBufferSizeUnit;
		private System.Windows.Forms.CheckBox checkBox_ShowBreakCount;
		private System.Windows.Forms.CheckBox checkBox_ShowFlowControlCount;
		private System.Windows.Forms.CheckBox checkBox_HideXOnXOff;
		private System.Windows.Forms.CheckBox checkBox_DisableKeywords;
		private System.Windows.Forms.GroupBox groupBox_Send_Keywords;
		private System.Windows.Forms.Label label_DefaultLineDelayUnit;
		private System.Windows.Forms.TextBox textBox_DefaultLineDelay;
		private System.Windows.Forms.Label label_DefaultLineDelay;
		private System.Windows.Forms.Label label_DefaultDelayUnit;
		private System.Windows.Forms.TextBox textBox_DefaultDelay;
		private System.Windows.Forms.Label label_DefaultDelay;
		private System.Windows.Forms.Label label_DefaultLineRepeatUnit;
		private System.Windows.Forms.TextBox textBox_DefaultLineRepeat;
		private System.Windows.Forms.Label label_DefaultLineRepeat;
		private System.Windows.Forms.Label label_ReplaceTab;
		private System.Windows.Forms.CheckBox checkBox_ReplaceTab;
		private System.Windows.Forms.GroupBox groupBox_Send_SerialPorts;
		private System.Windows.Forms.CheckBox checkBox_OutputBufferSize;
		private System.Windows.Forms.TextBox textBox_MaxChunkSize;
		private System.Windows.Forms.CheckBox checkBox_MaxChunkSize;
		private System.Windows.Forms.Label label_MaxChunkSizeUnit;
		private System.Windows.Forms.TextBox textBox_MaxSendRateSize;
		private System.Windows.Forms.TextBox textBox_MaxSendRateInterval;
		private System.Windows.Forms.Label label_MaxSendRateIntervalUnit1;
		private System.Windows.Forms.Label label_MaxSendRateIntervalUnit2;
		private System.Windows.Forms.CheckBox checkBox_MaxSendRateEnable;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.CheckBox checkBox_ShowDirection;
		private System.Windows.Forms.CheckBox checkBox_SignalXOnPeriodicallyEnable;
		private System.Windows.Forms.CheckBox checkBox_SignalXOnBeforeEachTransmission;
		private System.Windows.Forms.Label label_SignalXOnPeriodicallyIntervalUnit;
		private System.Windows.Forms.TextBox textBox_SignalXOnPeriodicallyInterval;
		private System.Windows.Forms.Label label_ReplaceSpaceUnicode;
		private System.Windows.Forms.GroupBox groupBox_Display_Special;
		private System.Windows.Forms.CheckBox checkBox_Hide0x00;
		private System.Windows.Forms.CheckBox checkBox_Hide0xFF;
		private System.Windows.Forms.Label label_DefaultLineIntervalUnit;
		private System.Windows.Forms.TextBox textBox_DefaultLineInterval;
		private System.Windows.Forms.Label label_DefaultLineInterval;
		private System.Windows.Forms.CheckBox checkBox_ShowPort;
		private System.Windows.Forms.CheckBox checkBox_PortLineBreak;
		private System.Windows.Forms.TextBox textBox_MaxBytePerLineCount;
		private System.Windows.Forms.Label label_MaxBytePerLineCountUnit;
		private System.Windows.Forms.Label label_MaxBytePerLineCount;
		private System.Windows.Forms.CheckBox checkBox_UseExplicitDefaultRadix;
	}
}