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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdvancedTerminalSettings));
			this.button_Defaults = new System.Windows.Forms.Button();
			this.button_Cancel = new System.Windows.Forms.Button();
			this.button_OK = new System.Windows.Forms.Button();
			this.groupBox_User = new System.Windows.Forms.GroupBox();
			this.textBox_UserName = new MKY.Windows.Forms.TextBoxEx();
			this.label_UserName = new System.Windows.Forms.Label();
			this.groupBox_Communication = new System.Windows.Forms.GroupBox();
			this.groupBox_Communication_SerialPorts = new System.Windows.Forms.GroupBox();
			this.checkBox_IgnoreFramingErrors = new System.Windows.Forms.CheckBox();
			this.checkBox_OutputBreakModifiable = new System.Windows.Forms.CheckBox();
			this.checkBox_IndicateBreakStates = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowBreakCount = new System.Windows.Forms.CheckBox();
			this.comboBox_Endianness = new System.Windows.Forms.ComboBox();
			this.label_Endianness = new System.Windows.Forms.Label();
			this.checkBox_ShowFlowControlCount = new System.Windows.Forms.CheckBox();
			this.groupBox_Send = new System.Windows.Forms.GroupBox();
			this.checkBox_SignalXOnWhenOpened = new System.Windows.Forms.CheckBox();
			this.checkBox_AllowConcurrency = new System.Windows.Forms.CheckBox();
			this.checkBox_SkipEmptyLines = new System.Windows.Forms.CheckBox();
			this.checkBox_EnableEscapesForText = new System.Windows.Forms.CheckBox();
			this.checkBox_UseExplicitDefaultRadix = new System.Windows.Forms.CheckBox();
			this.label_SignalXOnPeriodicallyIntervalUnit = new System.Windows.Forms.Label();
			this.groupBox_Send_SerialPorts = new System.Windows.Forms.GroupBox();
			this.textBox_MaxChunkSize = new MKY.Windows.Forms.TextBoxEx();
			this.checkBox_BufferMaxBaudRate = new System.Windows.Forms.CheckBox();
			this.textBox_MaxSendRateSize = new MKY.Windows.Forms.TextBoxEx();
			this.textBox_MaxSendRateInterval = new MKY.Windows.Forms.TextBoxEx();
			this.textBox_OutputBufferSize = new MKY.Windows.Forms.TextBoxEx();
			this.checkBox_OutputBufferSize = new System.Windows.Forms.CheckBox();
			this.label_OutputBufferSizeUnit = new System.Windows.Forms.Label();
			this.checkBox_MaxChunkSizeEnable = new System.Windows.Forms.CheckBox();
			this.label_MaxChunkSizeUnit = new System.Windows.Forms.Label();
			this.checkBox_MaxSendRateEnable = new System.Windows.Forms.CheckBox();
			this.label_MaxSendRateIntervalUnit1 = new System.Windows.Forms.Label();
			this.label_MaxSendRateIntervalUnit2 = new System.Windows.Forms.Label();
			this.checkBox_NoSendOnInputBreak = new System.Windows.Forms.CheckBox();
			this.checkBox_NoSendOnOutputBreak = new System.Windows.Forms.CheckBox();
			this.textBox_SignalXOnPeriodicallyInterval = new MKY.Windows.Forms.TextBoxEx();
			this.groupBox_Send_Keywords = new System.Windows.Forms.GroupBox();
			this.label_DefaultLineIntervalUnit = new System.Windows.Forms.Label();
			this.textBox_DefaultLineInterval = new MKY.Windows.Forms.TextBoxEx();
			this.label_DefaultLineInterval = new System.Windows.Forms.Label();
			this.label_DefaultLineRepeatUnit = new System.Windows.Forms.Label();
			this.textBox_DefaultLineRepeat = new MKY.Windows.Forms.TextBoxEx();
			this.label_DefaultLineRepeat = new System.Windows.Forms.Label();
			this.label_DefaultLineDelayUnit = new System.Windows.Forms.Label();
			this.textBox_DefaultLineDelay = new MKY.Windows.Forms.TextBoxEx();
			this.label_DefaultLineDelay = new System.Windows.Forms.Label();
			this.label_DefaultDelayUnit = new System.Windows.Forms.Label();
			this.textBox_DefaultDelay = new MKY.Windows.Forms.TextBoxEx();
			this.label_DefaultDelay = new System.Windows.Forms.Label();
			this.checkBox_SignalXOnPeriodicallyEnable = new System.Windows.Forms.CheckBox();
			this.checkBox_SignalXOnBeforeEachTransmission = new System.Windows.Forms.CheckBox();
			this.checkBox_EnableEscapesForFile = new System.Windows.Forms.CheckBox();
			this.checkBox_SendImmediately = new System.Windows.Forms.CheckBox();
			this.checkBox_CopyPredefined = new System.Windows.Forms.CheckBox();
			this.checkBox_KeepSendText = new System.Windows.Forms.CheckBox();
			this.groupBox_Display = new System.Windows.Forms.GroupBox();
			this.checkBox_IncludeIOWarnings = new System.Windows.Forms.CheckBox();
			this.groupBox_Line = new System.Windows.Forms.GroupBox();
			this.label_GlueCharsOfLine = new System.Windows.Forms.Label();
			this.checkBox_GlueCharsOfLine = new System.Windows.Forms.CheckBox();
			this.label_GlueCharsOfLineTimeoutUnit = new System.Windows.Forms.Label();
			this.textBox_GlueCharsOfLineTimeout = new MKY.Windows.Forms.TextBoxEx();
			this.label_LineBreaks = new System.Windows.Forms.Label();
			this.textBox_MaxLineLength = new MKY.Windows.Forms.TextBoxEx();
			this.label_MaxLineLengthUnit = new System.Windows.Forms.Label();
			this.label_MaxLineLength = new System.Windows.Forms.Label();
			this.checkBox_DeviceLineBreak = new System.Windows.Forms.CheckBox();
			this.textBox_MaxLineCount = new MKY.Windows.Forms.TextBoxEx();
			this.checkBox_DirectionLineBreak = new System.Windows.Forms.CheckBox();
			this.label_MaxLineCountUnit = new System.Windows.Forms.Label();
			this.label_MaxLineCount = new System.Windows.Forms.Label();
			this.label_LineBreakRemark = new System.Windows.Forms.Label();
			this.comboBox_LengthSelection = new System.Windows.Forms.ComboBox();
			this.comboBox_LineNumberSelection = new System.Windows.Forms.ComboBox();
			this.checkBox_IncludeIOControl = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowDuration = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowCopyOfActiveLine = new System.Windows.Forms.CheckBox();
			this.groupBox_Display_UsbSerialHid = new System.Windows.Forms.GroupBox();
			this.checkBox_IncludeNonPayloadData = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowDevice = new System.Windows.Forms.CheckBox();
			this.groupBox_Display_Special = new System.Windows.Forms.GroupBox();
			this.checkBox_Hide0xFF = new System.Windows.Forms.CheckBox();
			this.checkBox_Hide0x00 = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowDirection = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowLineNumbers = new System.Windows.Forms.CheckBox();
			this.groupBox_Display_Space = new System.Windows.Forms.GroupBox();
			this.label_SpaceReplacementChar = new System.Windows.Forms.Label();
			this.label_ReplaceSpaceUnicode = new System.Windows.Forms.Label();
			this.checkBox_ReplaceSpace = new System.Windows.Forms.CheckBox();
			this.groupBox_Display_ControlChars = new System.Windows.Forms.GroupBox();
			this.checkBox_BeepOnBell = new System.Windows.Forms.CheckBox();
			this.label_ReplaceBackspace = new System.Windows.Forms.Label();
			this.checkBox_ReplaceBackspace = new System.Windows.Forms.CheckBox();
			this.label_ReplaceTab = new System.Windows.Forms.Label();
			this.checkBox_HideXOnXOff = new System.Windows.Forms.CheckBox();
			this.comboBox_ControlCharacterRadix = new System.Windows.Forms.ComboBox();
			this.checkBox_ReplaceTab = new System.Windows.Forms.CheckBox();
			this.checkBox_ReplaceControlCharacters = new System.Windows.Forms.CheckBox();
			this.comboBox_RxRadix = new System.Windows.Forms.ComboBox();
			this.label_RxRadix = new System.Windows.Forms.Label();
			this.checkBox_SeparateTxRxRadix = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowRadix = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowTimeStamp = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowTimeSpan = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowTimeDelta = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowConnectTime = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowCountAndRate = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowLength = new System.Windows.Forms.CheckBox();
			this.comboBox_TxRadix = new System.Windows.Forms.ComboBox();
			this.label_TxRadix = new System.Windows.Forms.Label();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.label_TextSettingsRemark = new System.Windows.Forms.Label();
			this.groupBox_User.SuspendLayout();
			this.groupBox_Communication.SuspendLayout();
			this.groupBox_Communication_SerialPorts.SuspendLayout();
			this.groupBox_Send.SuspendLayout();
			this.groupBox_Send_SerialPorts.SuspendLayout();
			this.groupBox_Send_Keywords.SuspendLayout();
			this.groupBox_Display.SuspendLayout();
			this.groupBox_Line.SuspendLayout();
			this.groupBox_Display_UsbSerialHid.SuspendLayout();
			this.groupBox_Display_Special.SuspendLayout();
			this.groupBox_Display_Space.SuspendLayout();
			this.groupBox_Display_ControlChars.SuspendLayout();
			this.SuspendLayout();
			// 
			// button_Defaults
			// 
			this.button_Defaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Defaults.Location = new System.Drawing.Point(813, 105);
			this.button_Defaults.Name = "button_Defaults";
			this.button_Defaults.Size = new System.Drawing.Size(75, 23);
			this.button_Defaults.TabIndex = 6;
			this.button_Defaults.Text = "&Defaults...";
			this.button_Defaults.UseVisualStyleBackColor = true;
			this.button_Defaults.Click += new System.EventHandler(this.button_Defaults_Click);
			// 
			// button_Cancel
			// 
			this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button_Cancel.Location = new System.Drawing.Point(813, 55);
			this.button_Cancel.Name = "button_Cancel";
			this.button_Cancel.Size = new System.Drawing.Size(75, 23);
			this.button_Cancel.TabIndex = 5;
			this.button_Cancel.Text = "Cancel";
			this.button_Cancel.UseVisualStyleBackColor = true;
			this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
			// 
			// button_OK
			// 
			this.button_OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button_OK.Location = new System.Drawing.Point(813, 28);
			this.button_OK.Name = "button_OK";
			this.button_OK.Size = new System.Drawing.Size(75, 23);
			this.button_OK.TabIndex = 4;
			this.button_OK.Text = "OK";
			this.button_OK.UseVisualStyleBackColor = true;
			this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
			// 
			// groupBox_User
			// 
			this.groupBox_User.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_User.Controls.Add(this.textBox_UserName);
			this.groupBox_User.Controls.Add(this.label_UserName);
			this.groupBox_User.Location = new System.Drawing.Point(12, 574);
			this.groupBox_User.Name = "groupBox_User";
			this.groupBox_User.Size = new System.Drawing.Size(518, 45);
			this.groupBox_User.TabIndex = 2;
			this.groupBox_User.TabStop = false;
			this.groupBox_User.Text = "User Settings";
			// 
			// textBox_UserName
			// 
			this.textBox_UserName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox_UserName.Location = new System.Drawing.Point(99, 17);
			this.textBox_UserName.Name = "textBox_UserName";
			this.textBox_UserName.Size = new System.Drawing.Size(413, 20);
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
			this.groupBox_Communication.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Communication.Controls.Add(this.groupBox_Communication_SerialPorts);
			this.groupBox_Communication.Controls.Add(this.comboBox_Endianness);
			this.groupBox_Communication.Controls.Add(this.label_Endianness);
			this.groupBox_Communication.Controls.Add(this.checkBox_ShowFlowControlCount);
			this.groupBox_Communication.Location = new System.Drawing.Point(12, 440);
			this.groupBox_Communication.Name = "groupBox_Communication";
			this.groupBox_Communication.Size = new System.Drawing.Size(518, 128);
			this.groupBox_Communication.TabIndex = 1;
			this.groupBox_Communication.TabStop = false;
			this.groupBox_Communication.Text = "&Communication Settings";
			// 
			// groupBox_Communication_SerialPorts
			// 
			this.groupBox_Communication_SerialPorts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Communication_SerialPorts.Controls.Add(this.checkBox_IgnoreFramingErrors);
			this.groupBox_Communication_SerialPorts.Controls.Add(this.checkBox_OutputBreakModifiable);
			this.groupBox_Communication_SerialPorts.Controls.Add(this.checkBox_IndicateBreakStates);
			this.groupBox_Communication_SerialPorts.Controls.Add(this.checkBox_ShowBreakCount);
			this.groupBox_Communication_SerialPorts.Location = new System.Drawing.Point(6, 19);
			this.groupBox_Communication_SerialPorts.Name = "groupBox_Communication_SerialPorts";
			this.groupBox_Communication_SerialPorts.Size = new System.Drawing.Size(506, 69);
			this.groupBox_Communication_SerialPorts.TabIndex = 0;
			this.groupBox_Communication_SerialPorts.TabStop = false;
			this.groupBox_Communication_SerialPorts.Text = "Serial COM Ports";
			// 
			// checkBox_IgnoreFramingErrors
			// 
			this.checkBox_IgnoreFramingErrors.AutoSize = true;
			this.checkBox_IgnoreFramingErrors.Location = new System.Drawing.Point(261, 19);
			this.checkBox_IgnoreFramingErrors.Name = "checkBox_IgnoreFramingErrors";
			this.checkBox_IgnoreFramingErrors.Size = new System.Drawing.Size(122, 17);
			this.checkBox_IgnoreFramingErrors.TabIndex = 2;
			this.checkBox_IgnoreFramingErrors.Text = "Ignore framing errors";
			this.checkBox_IgnoreFramingErrors.UseVisualStyleBackColor = true;
			this.checkBox_IgnoreFramingErrors.CheckedChanged += new System.EventHandler(this.checkBox_IgnoreFramingErrors_CheckedChanged);
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
			// checkBox_ShowBreakCount
			// 
			this.checkBox_ShowBreakCount.AutoSize = true;
			this.checkBox_ShowBreakCount.Location = new System.Drawing.Point(261, 42);
			this.checkBox_ShowBreakCount.Name = "checkBox_ShowBreakCount";
			this.checkBox_ShowBreakCount.Size = new System.Drawing.Size(113, 17);
			this.checkBox_ShowBreakCount.TabIndex = 3;
			this.checkBox_ShowBreakCount.Text = "Show break count";
			this.checkBox_ShowBreakCount.UseVisualStyleBackColor = true;
			this.checkBox_ShowBreakCount.CheckedChanged += new System.EventHandler(this.checkBox_ShowBreakCount_CheckedChanged);
			// 
			// comboBox_Endianness
			// 
			this.comboBox_Endianness.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_Endianness.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_Endianness.Location = new System.Drawing.Point(131, 96);
			this.comboBox_Endianness.Name = "comboBox_Endianness";
			this.comboBox_Endianness.Size = new System.Drawing.Size(121, 21);
			this.comboBox_Endianness.TabIndex = 2;
			this.comboBox_Endianness.SelectedIndexChanged += new System.EventHandler(this.comboBox_Endianness_SelectedIndexChanged);
			// 
			// label_Endianness
			// 
			this.label_Endianness.AutoSize = true;
			this.label_Endianness.Location = new System.Drawing.Point(9, 99);
			this.label_Endianness.Name = "label_Endianness";
			this.label_Endianness.Size = new System.Drawing.Size(65, 13);
			this.label_Endianness.TabIndex = 1;
			this.label_Endianness.Text = "Endianness:";
			// 
			// checkBox_ShowFlowControlCount
			// 
			this.checkBox_ShowFlowControlCount.AutoSize = true;
			this.checkBox_ShowFlowControlCount.Location = new System.Drawing.Point(267, 98);
			this.checkBox_ShowFlowControlCount.Name = "checkBox_ShowFlowControlCount";
			this.checkBox_ShowFlowControlCount.Size = new System.Drawing.Size(140, 17);
			this.checkBox_ShowFlowControlCount.TabIndex = 3;
			this.checkBox_ShowFlowControlCount.Text = "Show flow control count";
			this.checkBox_ShowFlowControlCount.UseVisualStyleBackColor = true;
			this.checkBox_ShowFlowControlCount.CheckedChanged += new System.EventHandler(this.checkBox_ShowFlowControlCount_CheckedChanged);
			// 
			// groupBox_Send
			// 
			this.groupBox_Send.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Send.Controls.Add(this.checkBox_SignalXOnWhenOpened);
			this.groupBox_Send.Controls.Add(this.checkBox_AllowConcurrency);
			this.groupBox_Send.Controls.Add(this.checkBox_SkipEmptyLines);
			this.groupBox_Send.Controls.Add(this.checkBox_EnableEscapesForText);
			this.groupBox_Send.Controls.Add(this.checkBox_UseExplicitDefaultRadix);
			this.groupBox_Send.Controls.Add(this.label_SignalXOnPeriodicallyIntervalUnit);
			this.groupBox_Send.Controls.Add(this.groupBox_Send_SerialPorts);
			this.groupBox_Send.Controls.Add(this.textBox_SignalXOnPeriodicallyInterval);
			this.groupBox_Send.Controls.Add(this.groupBox_Send_Keywords);
			this.groupBox_Send.Controls.Add(this.checkBox_SignalXOnPeriodicallyEnable);
			this.groupBox_Send.Controls.Add(this.checkBox_SignalXOnBeforeEachTransmission);
			this.groupBox_Send.Controls.Add(this.checkBox_EnableEscapesForFile);
			this.groupBox_Send.Controls.Add(this.checkBox_SendImmediately);
			this.groupBox_Send.Controls.Add(this.checkBox_CopyPredefined);
			this.groupBox_Send.Controls.Add(this.checkBox_KeepSendText);
			this.groupBox_Send.Location = new System.Drawing.Point(536, 12);
			this.groupBox_Send.Name = "groupBox_Send";
			this.groupBox_Send.Size = new System.Drawing.Size(263, 607);
			this.groupBox_Send.TabIndex = 3;
			this.groupBox_Send.TabStop = false;
			this.groupBox_Send.Text = "Send Settin&gs";
			// 
			// checkBox_SignalXOnWhenOpened
			// 
			this.checkBox_SignalXOnWhenOpened.AutoSize = true;
			this.checkBox_SignalXOnWhenOpened.Location = new System.Drawing.Point(12, 184);
			this.checkBox_SignalXOnWhenOpened.Name = "checkBox_SignalXOnWhenOpened";
			this.checkBox_SignalXOnWhenOpened.Size = new System.Drawing.Size(209, 17);
			this.checkBox_SignalXOnWhenOpened.TabIndex = 6;
			this.checkBox_SignalXOnWhenOpened.Text = "Send XOn when I/O has been opened";
			this.checkBox_SignalXOnWhenOpened.UseVisualStyleBackColor = true;
			this.checkBox_SignalXOnWhenOpened.CheckedChanged += new System.EventHandler(this.checkBox_SignalXOnWhenOpened_CheckedChanged);
			// 
			// checkBox_AllowConcurrency
			// 
			this.checkBox_AllowConcurrency.AutoSize = true;
			this.checkBox_AllowConcurrency.Location = new System.Drawing.Point(12, 42);
			this.checkBox_AllowConcurrency.Name = "checkBox_AllowConcurrency";
			this.checkBox_AllowConcurrency.Size = new System.Drawing.Size(145, 17);
			this.checkBox_AllowConcurrency.TabIndex = 1;
			this.checkBox_AllowConcurrency.Text = "Allow concurrent sending";
			this.toolTip.SetToolTip(this.checkBox_AllowConcurrency, resources.GetString("checkBox_AllowConcurrency.ToolTip"));
			this.checkBox_AllowConcurrency.UseVisualStyleBackColor = true;
			this.checkBox_AllowConcurrency.CheckedChanged += new System.EventHandler(this.checkBox_AllowConcurrency_CheckedChanged);
			// 
			// checkBox_SkipEmptyLines
			// 
			this.checkBox_SkipEmptyLines.AutoSize = true;
			this.checkBox_SkipEmptyLines.Location = new System.Drawing.Point(12, 123);
			this.checkBox_SkipEmptyLines.Name = "checkBox_SkipEmptyLines";
			this.checkBox_SkipEmptyLines.Size = new System.Drawing.Size(170, 17);
			this.checkBox_SkipEmptyLines.TabIndex = 4;
			this.checkBox_SkipEmptyLines.Text = "Skip empty lines on [Send File]";
			this.checkBox_SkipEmptyLines.UseVisualStyleBackColor = true;
			this.checkBox_SkipEmptyLines.CheckedChanged += new System.EventHandler(this.checkBox_SkipEmptyLines_CheckedChanged);
			// 
			// checkBox_EnableEscapesForText
			// 
			this.checkBox_EnableEscapesForText.AutoSize = true;
			this.checkBox_EnableEscapesForText.Location = new System.Drawing.Point(12, 436);
			this.checkBox_EnableEscapesForText.Name = "checkBox_EnableEscapesForText";
			this.checkBox_EnableEscapesForText.Size = new System.Drawing.Size(237, 17);
			this.checkBox_EnableEscapesForText.TabIndex = 12;
			this.checkBox_EnableEscapesForText.Text = "&Enable <...> and \\... escapes on [Send Text]";
			this.checkBox_EnableEscapesForText.UseVisualStyleBackColor = true;
			this.checkBox_EnableEscapesForText.CheckedChanged += new System.EventHandler(this.checkBox_EnableEscapesForText_CheckedChanged);
			// 
			// checkBox_UseExplicitDefaultRadix
			// 
			this.checkBox_UseExplicitDefaultRadix.AutoSize = true;
			this.checkBox_UseExplicitDefaultRadix.Location = new System.Drawing.Point(12, 19);
			this.checkBox_UseExplicitDefaultRadix.Name = "checkBox_UseExplicitDefaultRadix";
			this.checkBox_UseExplicitDefaultRadix.Size = new System.Drawing.Size(140, 17);
			this.checkBox_UseExplicitDefaultRadix.TabIndex = 0;
			this.checkBox_UseExplicitDefaultRadix.Text = "Use explicit default radix";
			this.toolTip.SetToolTip(this.checkBox_UseExplicitDefaultRadix, "When enabled, the default radix can explicitly be selected.\r\nWhen disabled, the d" +
        "efault radix is \'String\'.\r\nApplies to [Send Text], [Send File] as well as [Prede" +
        "fined Commands].");
			this.checkBox_UseExplicitDefaultRadix.UseVisualStyleBackColor = true;
			this.checkBox_UseExplicitDefaultRadix.CheckedChanged += new System.EventHandler(this.checkBox_UseExplicitDefaultRadix_CheckedChanged);
			// 
			// label_SignalXOnPeriodicallyIntervalUnit
			// 
			this.label_SignalXOnPeriodicallyIntervalUnit.AutoSize = true;
			this.label_SignalXOnPeriodicallyIntervalUnit.Location = new System.Drawing.Point(219, 231);
			this.label_SignalXOnPeriodicallyIntervalUnit.Name = "label_SignalXOnPeriodicallyIntervalUnit";
			this.label_SignalXOnPeriodicallyIntervalUnit.Size = new System.Drawing.Size(20, 13);
			this.label_SignalXOnPeriodicallyIntervalUnit.TabIndex = 10;
			this.label_SignalXOnPeriodicallyIntervalUnit.Text = "ms";
			this.label_SignalXOnPeriodicallyIntervalUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupBox_Send_SerialPorts
			// 
			this.groupBox_Send_SerialPorts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Send_SerialPorts.Controls.Add(this.textBox_MaxChunkSize);
			this.groupBox_Send_SerialPorts.Controls.Add(this.checkBox_BufferMaxBaudRate);
			this.groupBox_Send_SerialPorts.Controls.Add(this.textBox_MaxSendRateSize);
			this.groupBox_Send_SerialPorts.Controls.Add(this.textBox_MaxSendRateInterval);
			this.groupBox_Send_SerialPorts.Controls.Add(this.textBox_OutputBufferSize);
			this.groupBox_Send_SerialPorts.Controls.Add(this.checkBox_OutputBufferSize);
			this.groupBox_Send_SerialPorts.Controls.Add(this.label_OutputBufferSizeUnit);
			this.groupBox_Send_SerialPorts.Controls.Add(this.checkBox_MaxChunkSizeEnable);
			this.groupBox_Send_SerialPorts.Controls.Add(this.label_MaxChunkSizeUnit);
			this.groupBox_Send_SerialPorts.Controls.Add(this.checkBox_MaxSendRateEnable);
			this.groupBox_Send_SerialPorts.Controls.Add(this.label_MaxSendRateIntervalUnit1);
			this.groupBox_Send_SerialPorts.Controls.Add(this.label_MaxSendRateIntervalUnit2);
			this.groupBox_Send_SerialPorts.Controls.Add(this.checkBox_NoSendOnInputBreak);
			this.groupBox_Send_SerialPorts.Controls.Add(this.checkBox_NoSendOnOutputBreak);
			this.groupBox_Send_SerialPorts.Location = new System.Drawing.Point(6, 262);
			this.groupBox_Send_SerialPorts.Name = "groupBox_Send_SerialPorts";
			this.groupBox_Send_SerialPorts.Size = new System.Drawing.Size(251, 158);
			this.groupBox_Send_SerialPorts.TabIndex = 11;
			this.groupBox_Send_SerialPorts.TabStop = false;
			this.groupBox_Send_SerialPorts.Text = "Serial COM Ports";
			// 
			// textBox_MaxChunkSize
			// 
			this.textBox_MaxChunkSize.Location = new System.Drawing.Point(132, 63);
			this.textBox_MaxChunkSize.Name = "textBox_MaxChunkSize";
			this.textBox_MaxChunkSize.Size = new System.Drawing.Size(48, 20);
			this.textBox_MaxChunkSize.TabIndex = 5;
			this.textBox_MaxChunkSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBox_MaxChunkSize.TextChanged += new System.EventHandler(this.textBox_MaxChunkSize_TextChanged);
			this.textBox_MaxChunkSize.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_MaxChunkSize_Validating);
			// 
			// checkBox_BufferMaxBaudRate
			// 
			this.checkBox_BufferMaxBaudRate.AutoSize = true;
			this.checkBox_BufferMaxBaudRate.Location = new System.Drawing.Point(6, 42);
			this.checkBox_BufferMaxBaudRate.Name = "checkBox_BufferMaxBaudRate";
			this.checkBox_BufferMaxBaudRate.Size = new System.Drawing.Size(206, 17);
			this.checkBox_BufferMaxBaudRate.TabIndex = 3;
			this.checkBox_BufferMaxBaudRate.Text = "Buffer not more than baud rate permits";
			this.toolTip.SetToolTip(this.checkBox_BufferMaxBaudRate, resources.GetString("checkBox_BufferMaxBaudRate.ToolTip"));
			this.checkBox_BufferMaxBaudRate.UseVisualStyleBackColor = true;
			this.checkBox_BufferMaxBaudRate.CheckedChanged += new System.EventHandler(this.checkBox_BufferMaxBaudRate_CheckedChanged);
			// 
			// textBox_MaxSendRateSize
			// 
			this.textBox_MaxSendRateSize.Location = new System.Drawing.Point(78, 86);
			this.textBox_MaxSendRateSize.Name = "textBox_MaxSendRateSize";
			this.textBox_MaxSendRateSize.Size = new System.Drawing.Size(48, 20);
			this.textBox_MaxSendRateSize.TabIndex = 8;
			this.textBox_MaxSendRateSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBox_MaxSendRateSize.TextChanged += new System.EventHandler(this.textBox_MaxSendRateSize_TextChanged);
			this.textBox_MaxSendRateSize.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_MaxSendRateSize_Validating);
			// 
			// textBox_MaxSendRateInterval
			// 
			this.textBox_MaxSendRateInterval.Location = new System.Drawing.Point(178, 86);
			this.textBox_MaxSendRateInterval.Name = "textBox_MaxSendRateInterval";
			this.textBox_MaxSendRateInterval.Size = new System.Drawing.Size(48, 20);
			this.textBox_MaxSendRateInterval.TabIndex = 10;
			this.textBox_MaxSendRateInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBox_MaxSendRateInterval.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_MaxSendRateInterval_Validating);
			// 
			// textBox_OutputBufferSize
			// 
			this.textBox_OutputBufferSize.Location = new System.Drawing.Point(163, 17);
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
			this.checkBox_OutputBufferSize.Size = new System.Drawing.Size(160, 17);
			this.checkBox_OutputBufferSize.TabIndex = 0;
			this.checkBox_OutputBufferSize.Text = "Set software output buffer to";
			this.toolTip.SetToolTip(this.checkBox_OutputBufferSize, resources.GetString("checkBox_OutputBufferSize.ToolTip"));
			this.checkBox_OutputBufferSize.UseVisualStyleBackColor = true;
			this.checkBox_OutputBufferSize.CheckedChanged += new System.EventHandler(this.checkBox_OutputBufferSize_CheckedChanged);
			// 
			// label_OutputBufferSizeUnit
			// 
			this.label_OutputBufferSizeUnit.AutoSize = true;
			this.label_OutputBufferSizeUnit.Location = new System.Drawing.Point(213, 20);
			this.label_OutputBufferSizeUnit.Name = "label_OutputBufferSizeUnit";
			this.label_OutputBufferSizeUnit.Size = new System.Drawing.Size(32, 13);
			this.label_OutputBufferSizeUnit.TabIndex = 2;
			this.label_OutputBufferSizeUnit.Text = "bytes";
			this.label_OutputBufferSizeUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkBox_MaxChunkSizeEnable
			// 
			this.checkBox_MaxChunkSizeEnable.AutoSize = true;
			this.checkBox_MaxChunkSizeEnable.Location = new System.Drawing.Point(6, 65);
			this.checkBox_MaxChunkSizeEnable.Name = "checkBox_MaxChunkSizeEnable";
			this.checkBox_MaxChunkSizeEnable.Size = new System.Drawing.Size(129, 17);
			this.checkBox_MaxChunkSizeEnable.TabIndex = 4;
			this.checkBox_MaxChunkSizeEnable.Text = "Buffer chunks of max.";
			this.toolTip.SetToolTip(this.checkBox_MaxChunkSizeEnable, resources.GetString("checkBox_MaxChunkSizeEnable.ToolTip"));
			this.checkBox_MaxChunkSizeEnable.UseVisualStyleBackColor = true;
			this.checkBox_MaxChunkSizeEnable.CheckedChanged += new System.EventHandler(this.checkBox_MaxChunkSizeEnable_CheckedChanged);
			// 
			// label_MaxChunkSizeUnit
			// 
			this.label_MaxChunkSizeUnit.AutoSize = true;
			this.label_MaxChunkSizeUnit.Location = new System.Drawing.Point(182, 66);
			this.label_MaxChunkSizeUnit.Name = "label_MaxChunkSizeUnit";
			this.label_MaxChunkSizeUnit.Size = new System.Drawing.Size(32, 13);
			this.label_MaxChunkSizeUnit.TabIndex = 6;
			this.label_MaxChunkSizeUnit.Text = "bytes";
			this.label_MaxChunkSizeUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkBox_MaxSendRateEnable
			// 
			this.checkBox_MaxSendRateEnable.AutoSize = true;
			this.checkBox_MaxSendRateEnable.Location = new System.Drawing.Point(6, 88);
			this.checkBox_MaxSendRateEnable.Name = "checkBox_MaxSendRateEnable";
			this.checkBox_MaxSendRateEnable.Size = new System.Drawing.Size(76, 17);
			this.checkBox_MaxSendRateEnable.TabIndex = 7;
			this.checkBox_MaxSendRateEnable.Text = "Send max.";
			this.toolTip.SetToolTip(this.checkBox_MaxSendRateEnable, "Limiting the send rate enables use cases where a device is not\r\ncapable to proces" +
        "s more than a certain number of bytes per interval.");
			this.checkBox_MaxSendRateEnable.UseVisualStyleBackColor = true;
			this.checkBox_MaxSendRateEnable.CheckedChanged += new System.EventHandler(this.checkBox_MaxSendRateEnable_CheckedChanged);
			// 
			// label_MaxSendRateIntervalUnit1
			// 
			this.label_MaxSendRateIntervalUnit1.AutoSize = true;
			this.label_MaxSendRateIntervalUnit1.Location = new System.Drawing.Point(128, 89);
			this.label_MaxSendRateIntervalUnit1.Name = "label_MaxSendRateIntervalUnit1";
			this.label_MaxSendRateIntervalUnit1.Size = new System.Drawing.Size(50, 13);
			this.label_MaxSendRateIntervalUnit1.TabIndex = 9;
			this.label_MaxSendRateIntervalUnit1.Text = "bytes per";
			this.label_MaxSendRateIntervalUnit1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label_MaxSendRateIntervalUnit2
			// 
			this.label_MaxSendRateIntervalUnit2.AutoSize = true;
			this.label_MaxSendRateIntervalUnit2.Location = new System.Drawing.Point(228, 89);
			this.label_MaxSendRateIntervalUnit2.Name = "label_MaxSendRateIntervalUnit2";
			this.label_MaxSendRateIntervalUnit2.Size = new System.Drawing.Size(20, 13);
			this.label_MaxSendRateIntervalUnit2.TabIndex = 11;
			this.label_MaxSendRateIntervalUnit2.Text = "ms";
			this.label_MaxSendRateIntervalUnit2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkBox_NoSendOnInputBreak
			// 
			this.checkBox_NoSendOnInputBreak.AutoSize = true;
			this.checkBox_NoSendOnInputBreak.Location = new System.Drawing.Point(6, 134);
			this.checkBox_NoSendOnInputBreak.Name = "checkBox_NoSendOnInputBreak";
			this.checkBox_NoSendOnInputBreak.Size = new System.Drawing.Size(212, 17);
			this.checkBox_NoSendOnInputBreak.TabIndex = 13;
			this.checkBox_NoSendOnInputBreak.Text = "No send while in input break state (IBS)";
			this.checkBox_NoSendOnInputBreak.UseVisualStyleBackColor = true;
			this.checkBox_NoSendOnInputBreak.CheckedChanged += new System.EventHandler(this.checkBox_NoSendOnInputBreak_CheckedChanged);
			// 
			// checkBox_NoSendOnOutputBreak
			// 
			this.checkBox_NoSendOnOutputBreak.AutoSize = true;
			this.checkBox_NoSendOnOutputBreak.Location = new System.Drawing.Point(6, 111);
			this.checkBox_NoSendOnOutputBreak.Name = "checkBox_NoSendOnOutputBreak";
			this.checkBox_NoSendOnOutputBreak.Size = new System.Drawing.Size(224, 17);
			this.checkBox_NoSendOnOutputBreak.TabIndex = 12;
			this.checkBox_NoSendOnOutputBreak.Text = "No send while in output break state (OBS)";
			this.checkBox_NoSendOnOutputBreak.UseVisualStyleBackColor = true;
			this.checkBox_NoSendOnOutputBreak.CheckedChanged += new System.EventHandler(this.checkBox_NoSendOnOutputBreak_CheckedChanged);
			// 
			// textBox_SignalXOnPeriodicallyInterval
			// 
			this.textBox_SignalXOnPeriodicallyInterval.Location = new System.Drawing.Point(169, 228);
			this.textBox_SignalXOnPeriodicallyInterval.Name = "textBox_SignalXOnPeriodicallyInterval";
			this.textBox_SignalXOnPeriodicallyInterval.Size = new System.Drawing.Size(48, 20);
			this.textBox_SignalXOnPeriodicallyInterval.TabIndex = 9;
			this.textBox_SignalXOnPeriodicallyInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBox_SignalXOnPeriodicallyInterval.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_SignalXOnPeriodicallyInterval_Validating);
			// 
			// groupBox_Send_Keywords
			// 
			this.groupBox_Send_Keywords.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
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
			this.groupBox_Send_Keywords.Location = new System.Drawing.Point(6, 489);
			this.groupBox_Send_Keywords.Name = "groupBox_Send_Keywords";
			this.groupBox_Send_Keywords.Size = new System.Drawing.Size(251, 112);
			this.groupBox_Send_Keywords.TabIndex = 14;
			this.groupBox_Send_Keywords.TabStop = false;
			this.groupBox_Send_Keywords.Text = "&Keywords";
			// 
			// label_DefaultLineIntervalUnit
			// 
			this.label_DefaultLineIntervalUnit.AutoSize = true;
			this.label_DefaultLineIntervalUnit.Location = new System.Drawing.Point(197, 66);
			this.label_DefaultLineIntervalUnit.Name = "label_DefaultLineIntervalUnit";
			this.label_DefaultLineIntervalUnit.Size = new System.Drawing.Size(20, 13);
			this.label_DefaultLineIntervalUnit.TabIndex = 8;
			this.label_DefaultLineIntervalUnit.Text = "ms";
			this.label_DefaultLineIntervalUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox_DefaultLineInterval
			// 
			this.textBox_DefaultLineInterval.Location = new System.Drawing.Point(147, 63);
			this.textBox_DefaultLineInterval.Name = "textBox_DefaultLineInterval";
			this.textBox_DefaultLineInterval.Size = new System.Drawing.Size(48, 20);
			this.textBox_DefaultLineInterval.TabIndex = 7;
			this.textBox_DefaultLineInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBox_DefaultLineInterval.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_DefaultLineInterval_Validating);
			// 
			// label_DefaultLineInterval
			// 
			this.label_DefaultLineInterval.AutoSize = true;
			this.label_DefaultLineInterval.Location = new System.Drawing.Point(6, 66);
			this.label_DefaultLineInterval.Name = "label_DefaultLineInterval";
			this.label_DefaultLineInterval.Size = new System.Drawing.Size(135, 13);
			this.label_DefaultLineInterval.TabIndex = 6;
			this.label_DefaultLineInterval.Text = "Default of \\!(LineInterval) is\r\n";
			this.label_DefaultLineInterval.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label_DefaultLineRepeatUnit
			// 
			this.label_DefaultLineRepeatUnit.AutoSize = true;
			this.label_DefaultLineRepeatUnit.Location = new System.Drawing.Point(197, 89);
			this.label_DefaultLineRepeatUnit.Name = "label_DefaultLineRepeatUnit";
			this.label_DefaultLineRepeatUnit.Size = new System.Drawing.Size(31, 13);
			this.label_DefaultLineRepeatUnit.TabIndex = 11;
			this.label_DefaultLineRepeatUnit.Text = "times";
			this.label_DefaultLineRepeatUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox_DefaultLineRepeat
			// 
			this.textBox_DefaultLineRepeat.Location = new System.Drawing.Point(147, 86);
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
			this.label_DefaultLineRepeat.Location = new System.Drawing.Point(6, 89);
			this.label_DefaultLineRepeat.Name = "label_DefaultLineRepeat";
			this.label_DefaultLineRepeat.Size = new System.Drawing.Size(135, 13);
			this.label_DefaultLineRepeat.TabIndex = 9;
			this.label_DefaultLineRepeat.Text = "Default of \\!(LineRepeat) is\r\n";
			this.label_DefaultLineRepeat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label_DefaultLineDelayUnit
			// 
			this.label_DefaultLineDelayUnit.AutoSize = true;
			this.label_DefaultLineDelayUnit.Location = new System.Drawing.Point(197, 43);
			this.label_DefaultLineDelayUnit.Name = "label_DefaultLineDelayUnit";
			this.label_DefaultLineDelayUnit.Size = new System.Drawing.Size(20, 13);
			this.label_DefaultLineDelayUnit.TabIndex = 5;
			this.label_DefaultLineDelayUnit.Text = "ms";
			this.label_DefaultLineDelayUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox_DefaultLineDelay
			// 
			this.textBox_DefaultLineDelay.Location = new System.Drawing.Point(147, 40);
			this.textBox_DefaultLineDelay.Name = "textBox_DefaultLineDelay";
			this.textBox_DefaultLineDelay.Size = new System.Drawing.Size(48, 20);
			this.textBox_DefaultLineDelay.TabIndex = 4;
			this.textBox_DefaultLineDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBox_DefaultLineDelay.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_DefaultLineDelay_Validating);
			// 
			// label_DefaultLineDelay
			// 
			this.label_DefaultLineDelay.AutoSize = true;
			this.label_DefaultLineDelay.Location = new System.Drawing.Point(6, 43);
			this.label_DefaultLineDelay.Name = "label_DefaultLineDelay";
			this.label_DefaultLineDelay.Size = new System.Drawing.Size(127, 13);
			this.label_DefaultLineDelay.TabIndex = 3;
			this.label_DefaultLineDelay.Text = "Default of \\!(LineDelay) is\r\n";
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
			this.label_DefaultDelay.Text = "Default of \\!(Delay) is\r\n";
			this.label_DefaultDelay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkBox_SignalXOnPeriodicallyEnable
			// 
			this.checkBox_SignalXOnPeriodicallyEnable.AutoSize = true;
			this.checkBox_SignalXOnPeriodicallyEnable.Location = new System.Drawing.Point(12, 230);
			this.checkBox_SignalXOnPeriodicallyEnable.Name = "checkBox_SignalXOnPeriodicallyEnable";
			this.checkBox_SignalXOnPeriodicallyEnable.Size = new System.Drawing.Size(159, 17);
			this.checkBox_SignalXOnPeriodicallyEnable.TabIndex = 8;
			this.checkBox_SignalXOnPeriodicallyEnable.Text = "Send XOn periodically every";
			this.checkBox_SignalXOnPeriodicallyEnable.UseVisualStyleBackColor = true;
			this.checkBox_SignalXOnPeriodicallyEnable.CheckedChanged += new System.EventHandler(this.checkBox_SignalXOnPeriodicallyEnable_CheckedChanged);
			// 
			// checkBox_SignalXOnBeforeEachTransmission
			// 
			this.checkBox_SignalXOnBeforeEachTransmission.AutoSize = true;
			this.checkBox_SignalXOnBeforeEachTransmission.Location = new System.Drawing.Point(12, 207);
			this.checkBox_SignalXOnBeforeEachTransmission.Name = "checkBox_SignalXOnBeforeEachTransmission";
			this.checkBox_SignalXOnBeforeEachTransmission.Size = new System.Drawing.Size(195, 17);
			this.checkBox_SignalXOnBeforeEachTransmission.TabIndex = 7;
			this.checkBox_SignalXOnBeforeEachTransmission.Text = "Send XOn before each transmission";
			this.checkBox_SignalXOnBeforeEachTransmission.UseVisualStyleBackColor = true;
			this.checkBox_SignalXOnBeforeEachTransmission.CheckedChanged += new System.EventHandler(this.checkBox_SignalXOnBeforeEachTransmission_CheckedChanged);
			// 
			// checkBox_EnableEscapesForFile
			// 
			this.checkBox_EnableEscapesForFile.AutoSize = true;
			this.checkBox_EnableEscapesForFile.Location = new System.Drawing.Point(12, 459);
			this.checkBox_EnableEscapesForFile.Name = "checkBox_EnableEscapesForFile";
			this.checkBox_EnableEscapesForFile.Size = new System.Drawing.Size(232, 17);
			this.checkBox_EnableEscapesForFile.TabIndex = 13;
			this.checkBox_EnableEscapesForFile.Text = "Enable <...> and \\... escapes on [Send File]";
			this.toolTip.SetToolTip(this.checkBox_EnableEscapesForFile, "Only applies to text terminals.");
			this.checkBox_EnableEscapesForFile.UseVisualStyleBackColor = true;
			this.checkBox_EnableEscapesForFile.CheckedChanged += new System.EventHandler(this.checkBox_EnableEscapesForFile_CheckedChanged);
			// 
			// checkBox_SendImmediately
			// 
			this.checkBox_SendImmediately.AutoSize = true;
			this.checkBox_SendImmediately.Location = new System.Drawing.Point(12, 100);
			this.checkBox_SendImmediately.Name = "checkBox_SendImmediately";
			this.checkBox_SendImmediately.Size = new System.Drawing.Size(241, 17);
			this.checkBox_SendImmediately.TabIndex = 3;
			this.checkBox_SendImmediately.Text = "Send each [Send Text] character immediately";
			this.toolTip.SetToolTip(this.checkBox_SendImmediately, "Emulates a terminal/direct mode.");
			this.checkBox_SendImmediately.UseVisualStyleBackColor = true;
			this.checkBox_SendImmediately.CheckedChanged += new System.EventHandler(this.checkBox_SendImmediately_CheckedChanged);
			// 
			// checkBox_CopyPredefined
			// 
			this.checkBox_CopyPredefined.AutoSize = true;
			this.checkBox_CopyPredefined.Location = new System.Drawing.Point(12, 146);
			this.checkBox_CopyPredefined.Name = "checkBox_CopyPredefined";
			this.checkBox_CopyPredefined.Size = new System.Drawing.Size(244, 17);
			this.checkBox_CopyPredefined.TabIndex = 5;
			this.checkBox_CopyPredefined.Text = "Copy predefined to [Send Text/File] after send";
			this.checkBox_CopyPredefined.UseVisualStyleBackColor = true;
			this.checkBox_CopyPredefined.CheckedChanged += new System.EventHandler(this.checkBox_CopyPredefined_CheckedChanged);
			// 
			// checkBox_KeepSendText
			// 
			this.checkBox_KeepSendText.AutoSize = true;
			this.checkBox_KeepSendText.Location = new System.Drawing.Point(12, 77);
			this.checkBox_KeepSendText.Name = "checkBox_KeepSendText";
			this.checkBox_KeepSendText.Size = new System.Drawing.Size(159, 17);
			this.checkBox_KeepSendText.TabIndex = 2;
			this.checkBox_KeepSendText.Text = "Keep [Send Text] after send";
			this.checkBox_KeepSendText.UseVisualStyleBackColor = true;
			this.checkBox_KeepSendText.CheckedChanged += new System.EventHandler(this.checkBox_KeepSendText_CheckedChanged);
			// 
			// groupBox_Display
			// 
			this.groupBox_Display.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Display.Controls.Add(this.checkBox_ShowCountAndRate);
			this.groupBox_Display.Controls.Add(this.checkBox_IncludeIOWarnings);
			this.groupBox_Display.Controls.Add(this.groupBox_Line);
			this.groupBox_Display.Controls.Add(this.comboBox_LengthSelection);
			this.groupBox_Display.Controls.Add(this.comboBox_LineNumberSelection);
			this.groupBox_Display.Controls.Add(this.checkBox_IncludeIOControl);
			this.groupBox_Display.Controls.Add(this.checkBox_ShowDuration);
			this.groupBox_Display.Controls.Add(this.checkBox_ShowCopyOfActiveLine);
			this.groupBox_Display.Controls.Add(this.groupBox_Display_UsbSerialHid);
			this.groupBox_Display.Controls.Add(this.checkBox_ShowDevice);
			this.groupBox_Display.Controls.Add(this.groupBox_Display_Special);
			this.groupBox_Display.Controls.Add(this.checkBox_ShowDirection);
			this.groupBox_Display.Controls.Add(this.checkBox_ShowLineNumbers);
			this.groupBox_Display.Controls.Add(this.groupBox_Display_Space);
			this.groupBox_Display.Controls.Add(this.groupBox_Display_ControlChars);
			this.groupBox_Display.Controls.Add(this.comboBox_RxRadix);
			this.groupBox_Display.Controls.Add(this.label_RxRadix);
			this.groupBox_Display.Controls.Add(this.checkBox_SeparateTxRxRadix);
			this.groupBox_Display.Controls.Add(this.checkBox_ShowRadix);
			this.groupBox_Display.Controls.Add(this.checkBox_ShowTimeStamp);
			this.groupBox_Display.Controls.Add(this.checkBox_ShowTimeSpan);
			this.groupBox_Display.Controls.Add(this.checkBox_ShowTimeDelta);
			this.groupBox_Display.Controls.Add(this.checkBox_ShowConnectTime);
			this.groupBox_Display.Controls.Add(this.checkBox_ShowLength);
			this.groupBox_Display.Controls.Add(this.comboBox_TxRadix);
			this.groupBox_Display.Controls.Add(this.label_TxRadix);
			this.groupBox_Display.Location = new System.Drawing.Point(12, 12);
			this.groupBox_Display.Name = "groupBox_Display";
			this.groupBox_Display.Size = new System.Drawing.Size(518, 426);
			this.groupBox_Display.TabIndex = 0;
			this.groupBox_Display.TabStop = false;
			this.groupBox_Display.Text = "Display Settings";
			// 
			// checkBox_IncludeIOWarnings
			// 
			this.checkBox_IncludeIOWarnings.AutoSize = true;
			this.checkBox_IncludeIOWarnings.Location = new System.Drawing.Point(12, 300);
			this.checkBox_IncludeIOWarnings.Name = "checkBox_IncludeIOWarnings";
			this.checkBox_IncludeIOWarnings.Size = new System.Drawing.Size(125, 17);
			this.checkBox_IncludeIOWarnings.TabIndex = 19;
			this.checkBox_IncludeIOWarnings.Text = "Include I/O warnings";
			this.toolTip.SetToolTip(this.checkBox_IncludeIOWarnings, resources.GetString("checkBox_IncludeIOWarnings.ToolTip"));
			this.checkBox_IncludeIOWarnings.UseVisualStyleBackColor = true;
			this.checkBox_IncludeIOWarnings.CheckedChanged += new System.EventHandler(this.checkBox_IncludeIOWarnings_CheckedChanged);
			// 
			// groupBox_Line
			// 
			this.groupBox_Line.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Line.Controls.Add(this.textBox_GlueCharsOfLineTimeout);
			this.groupBox_Line.Controls.Add(this.label_GlueCharsOfLine);
			this.groupBox_Line.Controls.Add(this.checkBox_GlueCharsOfLine);
			this.groupBox_Line.Controls.Add(this.label_GlueCharsOfLineTimeoutUnit);
			this.groupBox_Line.Controls.Add(this.label_LineBreaks);
			this.groupBox_Line.Controls.Add(this.textBox_MaxLineLength);
			this.groupBox_Line.Controls.Add(this.label_MaxLineLengthUnit);
			this.groupBox_Line.Controls.Add(this.label_MaxLineLength);
			this.groupBox_Line.Controls.Add(this.checkBox_DeviceLineBreak);
			this.groupBox_Line.Controls.Add(this.textBox_MaxLineCount);
			this.groupBox_Line.Controls.Add(this.checkBox_DirectionLineBreak);
			this.groupBox_Line.Controls.Add(this.label_MaxLineCountUnit);
			this.groupBox_Line.Controls.Add(this.label_MaxLineCount);
			this.groupBox_Line.Controls.Add(this.label_LineBreakRemark);
			this.groupBox_Line.Location = new System.Drawing.Point(261, 10);
			this.groupBox_Line.Name = "groupBox_Line";
			this.groupBox_Line.Size = new System.Drawing.Size(251, 169);
			this.groupBox_Line.TabIndex = 22;
			this.groupBox_Line.TabStop = false;
			// 
			// label_GlueCharsOfLine
			// 
			this.label_GlueCharsOfLine.AutoSize = true;
			this.label_GlueCharsOfLine.Location = new System.Drawing.Point(26, 100);
			this.label_GlueCharsOfLine.Name = "label_GlueCharsOfLine";
			this.label_GlueCharsOfLine.Size = new System.Drawing.Size(141, 13);
			this.label_GlueCharsOfLine.TabIndex = 5;
			this.label_GlueCharsOfLine.Text = "a line together;  timeout after";
			this.label_GlueCharsOfLine.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkBox_GlueCharsOfLine
			// 
			this.checkBox_GlueCharsOfLine.AutoSize = true;
			this.checkBox_GlueCharsOfLine.Location = new System.Drawing.Point(9, 78);
			this.checkBox_GlueCharsOfLine.Name = "checkBox_GlueCharsOfLine";
			this.checkBox_GlueCharsOfLine.Size = new System.Drawing.Size(234, 17);
			this.checkBox_GlueCharsOfLine.TabIndex = 4;
			this.checkBox_GlueCharsOfLine.Text = "Reduce line breaks by &glueing characters of";
			this.toolTip.SetToolTip(this.checkBox_GlueCharsOfLine, resources.GetString("checkBox_GlueCharsOfLine.ToolTip"));
			this.checkBox_GlueCharsOfLine.UseVisualStyleBackColor = true;
			this.checkBox_GlueCharsOfLine.CheckedChanged += new System.EventHandler(this.checkBox_GlueCharsOfLine_CheckedChanged);
			// 
			// label_GlueCharsOfLineTimeoutUnit
			// 
			this.label_GlueCharsOfLineTimeoutUnit.AutoSize = true;
			this.label_GlueCharsOfLineTimeoutUnit.Location = new System.Drawing.Point(216, 100);
			this.label_GlueCharsOfLineTimeoutUnit.Name = "label_GlueCharsOfLineTimeoutUnit";
			this.label_GlueCharsOfLineTimeoutUnit.Size = new System.Drawing.Size(20, 13);
			this.label_GlueCharsOfLineTimeoutUnit.TabIndex = 7;
			this.label_GlueCharsOfLineTimeoutUnit.Text = "ms";
			this.label_GlueCharsOfLineTimeoutUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox_GlueCharsOfLineTimeout
			// 
			this.textBox_GlueCharsOfLineTimeout.Location = new System.Drawing.Point(167, 97);
			this.textBox_GlueCharsOfLineTimeout.Name = "textBox_GlueCharsOfLineTimeout";
			this.textBox_GlueCharsOfLineTimeout.Size = new System.Drawing.Size(48, 20);
			this.textBox_GlueCharsOfLineTimeout.TabIndex = 6;
			this.textBox_GlueCharsOfLineTimeout.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.toolTip.SetToolTip(this.textBox_GlueCharsOfLineTimeout, "When no EOL is received within this timeout, i.e.\r\nthe line is not yet complete, " +
        "a change of direction\r\n(or I/O device) will result in a line break.\r\n\r\nSet to -1" +
        " for infinite waiting for EOL.");
			this.textBox_GlueCharsOfLineTimeout.TextChanged += new System.EventHandler(this.textBox_GlueCharsOfLineTimeout_TextChanged);
			this.textBox_GlueCharsOfLineTimeout.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_GlueCharsOfLineTimeout_Validating);
			// 
			// label_LineBreaks
			// 
			this.label_LineBreaks.AutoSize = true;
			this.label_LineBreaks.Location = new System.Drawing.Point(6, 13);
			this.label_LineBreaks.Name = "label_LineBreaks";
			this.label_LineBreaks.Size = new System.Drawing.Size(68, 13);
			this.label_LineBreaks.TabIndex = 0;
			this.label_LineBreaks.Text = "&Break lines...";
			// 
			// textBox_MaxLineLength
			// 
			this.textBox_MaxLineLength.Location = new System.Drawing.Point(87, 143);
			this.textBox_MaxLineLength.Name = "textBox_MaxLineLength";
			this.textBox_MaxLineLength.Size = new System.Drawing.Size(48, 20);
			this.textBox_MaxLineLength.TabIndex = 12;
			this.textBox_MaxLineLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.toolTip.SetToolTip(this.textBox_MaxLineLength, resources.GetString("textBox_MaxLineLength.ToolTip"));
			this.textBox_MaxLineLength.TextChanged += new System.EventHandler(this.textBox_MaxLineLength_TextChanged);
			this.textBox_MaxLineLength.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_MaxLineLength_Validating);
			// 
			// label_MaxLineLengthUnit
			// 
			this.label_MaxLineLengthUnit.AutoSize = true;
			this.label_MaxLineLengthUnit.Location = new System.Drawing.Point(136, 146);
			this.label_MaxLineLengthUnit.Name = "label_MaxLineLengthUnit";
			this.label_MaxLineLengthUnit.Size = new System.Drawing.Size(94, 13);
			this.label_MaxLineLengthUnit.TabIndex = 13;
			this.label_MaxLineLengthUnit.Text = "characters per line";
			// 
			// label_MaxLineLength
			// 
			this.label_MaxLineLength.AutoSize = true;
			this.label_MaxLineLength.Location = new System.Drawing.Point(6, 146);
			this.label_MaxLineLength.Name = "label_MaxLineLength";
			this.label_MaxLineLength.Size = new System.Drawing.Size(81, 13);
			this.label_MaxLineLength.TabIndex = 11;
			this.label_MaxLineLength.Text = "Display maximal";
			// 
			// checkBox_DeviceLineBreak
			// 
			this.checkBox_DeviceLineBreak.AutoSize = true;
			this.checkBox_DeviceLineBreak.Location = new System.Drawing.Point(9, 55);
			this.checkBox_DeviceLineBreak.Name = "checkBox_DeviceLineBreak";
			this.checkBox_DeviceLineBreak.Size = new System.Drawing.Size(159, 17);
			this.checkBox_DeviceLineBreak.TabIndex = 3;
			this.checkBox_DeviceLineBreak.Text = "...when I/O device changes";
			this.toolTip.SetToolTip(this.checkBox_DeviceLineBreak, resources.GetString("checkBox_DeviceLineBreak.ToolTip"));
			this.checkBox_DeviceLineBreak.UseVisualStyleBackColor = true;
			this.checkBox_DeviceLineBreak.CheckedChanged += new System.EventHandler(this.checkBox_DeviceLineBreak_CheckedChanged);
			// 
			// textBox_MaxLineCount
			// 
			this.textBox_MaxLineCount.Location = new System.Drawing.Point(87, 120);
			this.textBox_MaxLineCount.Name = "textBox_MaxLineCount";
			this.textBox_MaxLineCount.Size = new System.Drawing.Size(48, 20);
			this.textBox_MaxLineCount.TabIndex = 9;
			this.textBox_MaxLineCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.toolTip.SetToolTip(this.textBox_MaxLineCount, "The maximal number of lines is limited in order to improve performance.");
			this.textBox_MaxLineCount.TextChanged += new System.EventHandler(this.textBox_MaxLineCount_TextChanged);
			this.textBox_MaxLineCount.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_MaxLineCount_Validating);
			// 
			// checkBox_DirectionLineBreak
			// 
			this.checkBox_DirectionLineBreak.AutoSize = true;
			this.checkBox_DirectionLineBreak.Location = new System.Drawing.Point(9, 32);
			this.checkBox_DirectionLineBreak.Name = "checkBox_DirectionLineBreak";
			this.checkBox_DirectionLineBreak.Size = new System.Drawing.Size(148, 17);
			this.checkBox_DirectionLineBreak.TabIndex = 2;
			this.checkBox_DirectionLineBreak.Text = "...when direction changes";
			this.toolTip.SetToolTip(this.checkBox_DirectionLineBreak, "When the reduce/glue option is enabled below, this\r\noption is only relevant after" +
        " the timeout has elapsed.\r\n\r\nThis option solely applies to the [Bidirectional Pa" +
        "nel].");
			this.checkBox_DirectionLineBreak.UseVisualStyleBackColor = true;
			this.checkBox_DirectionLineBreak.CheckedChanged += new System.EventHandler(this.checkBox_DirectionLineBreak_CheckedChanged);
			// 
			// label_MaxLineCountUnit
			// 
			this.label_MaxLineCountUnit.AutoSize = true;
			this.label_MaxLineCountUnit.Location = new System.Drawing.Point(136, 123);
			this.label_MaxLineCountUnit.Name = "label_MaxLineCountUnit";
			this.label_MaxLineCountUnit.Size = new System.Drawing.Size(28, 13);
			this.label_MaxLineCountUnit.TabIndex = 10;
			this.label_MaxLineCountUnit.Text = "lines";
			this.label_MaxLineCountUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label_MaxLineCount
			// 
			this.label_MaxLineCount.AutoSize = true;
			this.label_MaxLineCount.Location = new System.Drawing.Point(6, 123);
			this.label_MaxLineCount.Name = "label_MaxLineCount";
			this.label_MaxLineCount.Size = new System.Drawing.Size(81, 13);
			this.label_MaxLineCount.TabIndex = 8;
			this.label_MaxLineCount.Text = "Displa&y maximal";
			this.label_MaxLineCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label_LineBreakRemark
			// 
			this.label_LineBreakRemark.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label_LineBreakRemark.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_LineBreakRemark.ForeColor = System.Drawing.SystemColors.GrayText;
			this.label_LineBreakRemark.Location = new System.Drawing.Point(152, 11);
			this.label_LineBreakRemark.Name = "label_LineBreakRemark";
			this.label_LineBreakRemark.Size = new System.Drawing.Size(95, 44);
			this.label_LineBreakRemark.TabIndex = 1;
			this.label_LineBreakRemark.Text = "Also see\r\n[Binary Settings...]";
			this.label_LineBreakRemark.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// comboBox_LengthSelection
			// 
			this.comboBox_LengthSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_LengthSelection.Location = new System.Drawing.Point(131, 228);
			this.comboBox_LengthSelection.Name = "comboBox_LengthSelection";
			this.comboBox_LengthSelection.Size = new System.Drawing.Size(121, 21);
			this.comboBox_LengthSelection.TabIndex = 16;
			this.toolTip.SetToolTip(this.comboBox_LengthSelection, "Text terminals: Selectable, default is [Char Count].\r\nBinary terminals: Fixed to " +
        "[Byte Count].");
			this.comboBox_LengthSelection.SelectedIndexChanged += new System.EventHandler(this.comboBox_LengthSelection_SelectedIndexChanged);
			// 
			// comboBox_LineNumberSelection
			// 
			this.comboBox_LineNumberSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_LineNumberSelection.Location = new System.Drawing.Point(131, 136);
			this.comboBox_LineNumberSelection.Name = "comboBox_LineNumberSelection";
			this.comboBox_LineNumberSelection.Size = new System.Drawing.Size(121, 21);
			this.comboBox_LineNumberSelection.TabIndex = 9;
			this.comboBox_LineNumberSelection.SelectedIndexChanged += new System.EventHandler(this.comboBox_LineNumberSelection_SelectedIndexChanged);
			// 
			// checkBox_IncludeIOControl
			// 
			this.checkBox_IncludeIOControl.AutoSize = true;
			this.checkBox_IncludeIOControl.Location = new System.Drawing.Point(12, 277);
			this.checkBox_IncludeIOControl.Name = "checkBox_IncludeIOControl";
			this.checkBox_IncludeIOControl.Size = new System.Drawing.Size(150, 17);
			this.checkBox_IncludeIOControl.TabIndex = 18;
			this.checkBox_IncludeIOControl.Text = "Include I/O control events";
			this.toolTip.SetToolTip(this.checkBox_IncludeIOControl, resources.GetString("checkBox_IncludeIOControl.ToolTip"));
			this.checkBox_IncludeIOControl.UseVisualStyleBackColor = true;
			this.checkBox_IncludeIOControl.CheckedChanged += new System.EventHandler(this.checkBox_IncludeIOControl_CheckedChanged);
			// 
			// checkBox_ShowDuration
			// 
			this.checkBox_ShowDuration.AutoSize = true;
			this.checkBox_ShowDuration.Location = new System.Drawing.Point(12, 254);
			this.checkBox_ShowDuration.Name = "checkBox_ShowDuration";
			this.checkBox_ShowDuration.Size = new System.Drawing.Size(119, 17);
			this.checkBox_ShowDuration.TabIndex = 17;
			this.checkBox_ShowDuration.Text = "Show duration (line)";
			this.toolTip.SetToolTip(this.checkBox_ShowDuration, "The duration from first to last byte of the line.\r\n\r\nFormat can be configured in " +
        "[View > Format...].");
			this.checkBox_ShowDuration.UseVisualStyleBackColor = true;
			this.checkBox_ShowDuration.CheckedChanged += new System.EventHandler(this.checkBox_ShowDuration_CheckedChanged);
			// 
			// checkBox_ShowCopyOfActiveLine
			// 
			this.checkBox_ShowCopyOfActiveLine.AutoSize = true;
			this.checkBox_ShowCopyOfActiveLine.Location = new System.Drawing.Point(12, 389);
			this.checkBox_ShowCopyOfActiveLine.Name = "checkBox_ShowCopyOfActiveLine";
			this.checkBox_ShowCopyOfActiveLine.Size = new System.Drawing.Size(142, 17);
			this.checkBox_ShowCopyOfActiveLine.TabIndex = 21;
			this.checkBox_ShowCopyOfActiveLine.Text = "Show copy of active line";
			this.checkBox_ShowCopyOfActiveLine.UseVisualStyleBackColor = true;
			this.checkBox_ShowCopyOfActiveLine.CheckedChanged += new System.EventHandler(this.checkBox_ShowCopyOfActiveLine_CheckedChanged);
			// 
			// groupBox_Display_UsbSerialHid
			// 
			this.groupBox_Display_UsbSerialHid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox_Display_UsbSerialHid.Controls.Add(this.checkBox_IncludeNonPayloadData);
			this.groupBox_Display_UsbSerialHid.Location = new System.Drawing.Point(6, 328);
			this.groupBox_Display_UsbSerialHid.Name = "groupBox_Display_UsbSerialHid";
			this.groupBox_Display_UsbSerialHid.Size = new System.Drawing.Size(251, 43);
			this.groupBox_Display_UsbSerialHid.TabIndex = 20;
			this.groupBox_Display_UsbSerialHid.TabStop = false;
			this.groupBox_Display_UsbSerialHid.Text = "&USB Ser/HID";
			// 
			// checkBox_IncludeNonPayloadData
			// 
			this.checkBox_IncludeNonPayloadData.AutoSize = true;
			this.checkBox_IncludeNonPayloadData.Location = new System.Drawing.Point(6, 19);
			this.checkBox_IncludeNonPayloadData.Name = "checkBox_IncludeNonPayloadData";
			this.checkBox_IncludeNonPayloadData.Size = new System.Drawing.Size(146, 17);
			this.checkBox_IncludeNonPayloadData.TabIndex = 0;
			this.checkBox_IncludeNonPayloadData.Text = "Include non-payload data";
			this.checkBox_IncludeNonPayloadData.UseVisualStyleBackColor = true;
			this.checkBox_IncludeNonPayloadData.CheckedChanged += new System.EventHandler(this.checkBox_IncludeNonPayloadData_CheckedChanged);
			// 
			// checkBox_ShowDevice
			// 
			this.checkBox_ShowDevice.AutoSize = true;
			this.checkBox_ShowDevice.Location = new System.Drawing.Point(131, 175);
			this.checkBox_ShowDevice.Name = "checkBox_ShowDevice";
			this.checkBox_ShowDevice.Size = new System.Drawing.Size(107, 17);
			this.checkBox_ShowDevice.TabIndex = 13;
			this.checkBox_ShowDevice.Text = "Show I/O de&vice";
			this.checkBox_ShowDevice.UseVisualStyleBackColor = true;
			this.checkBox_ShowDevice.CheckedChanged += new System.EventHandler(this.checkBox_ShowDevice_CheckedChanged);
			// 
			// groupBox_Display_Special
			// 
			this.groupBox_Display_Special.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Display_Special.Controls.Add(this.checkBox_Hide0xFF);
			this.groupBox_Display_Special.Controls.Add(this.checkBox_Hide0x00);
			this.groupBox_Display_Special.Location = new System.Drawing.Point(261, 377);
			this.groupBox_Display_Special.Name = "groupBox_Display_Special";
			this.groupBox_Display_Special.Size = new System.Drawing.Size(251, 43);
			this.groupBox_Display_Special.TabIndex = 25;
			this.groupBox_Display_Special.TabStop = false;
			this.groupBox_Display_Special.Text = "Spec&ial";
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
			this.checkBox_ShowDirection.Location = new System.Drawing.Point(131, 198);
			this.checkBox_ShowDirection.Name = "checkBox_ShowDirection";
			this.checkBox_ShowDirection.Size = new System.Drawing.Size(96, 17);
			this.checkBox_ShowDirection.TabIndex = 14;
			this.checkBox_ShowDirection.Text = "Show &direction";
			this.checkBox_ShowDirection.UseVisualStyleBackColor = true;
			this.checkBox_ShowDirection.CheckedChanged += new System.EventHandler(this.checkBox_ShowDirection_CheckedChanged);
			// 
			// checkBox_ShowLineNumbers
			// 
			this.checkBox_ShowLineNumbers.AutoSize = true;
			this.checkBox_ShowLineNumbers.Location = new System.Drawing.Point(12, 139);
			this.checkBox_ShowLineNumbers.Name = "checkBox_ShowLineNumbers";
			this.checkBox_ShowLineNumbers.Size = new System.Drawing.Size(118, 17);
			this.checkBox_ShowLineNumbers.TabIndex = 8;
			this.checkBox_ShowLineNumbers.Text = "Show line &numbers:";
			this.checkBox_ShowLineNumbers.UseVisualStyleBackColor = true;
			this.checkBox_ShowLineNumbers.CheckedChanged += new System.EventHandler(this.checkBox_ShowLineNumbers_CheckedChanged);
			// 
			// groupBox_Display_Space
			// 
			this.groupBox_Display_Space.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Display_Space.Controls.Add(this.label_SpaceReplacementChar);
			this.groupBox_Display_Space.Controls.Add(this.label_ReplaceSpaceUnicode);
			this.groupBox_Display_Space.Controls.Add(this.checkBox_ReplaceSpace);
			this.groupBox_Display_Space.Location = new System.Drawing.Point(261, 328);
			this.groupBox_Display_Space.Name = "groupBox_Display_Space";
			this.groupBox_Display_Space.Size = new System.Drawing.Size(251, 43);
			this.groupBox_Display_Space.TabIndex = 24;
			this.groupBox_Display_Space.TabStop = false;
			this.groupBox_Display_Space.Text = "Spa&ce";
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
			this.checkBox_ReplaceSpace.Text = "Replace by open box character";
			this.checkBox_ReplaceSpace.UseVisualStyleBackColor = true;
			this.checkBox_ReplaceSpace.CheckedChanged += new System.EventHandler(this.checkBox_ReplaceSpace_CheckedChanged);
			// 
			// groupBox_Display_ControlChars
			// 
			this.groupBox_Display_ControlChars.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Display_ControlChars.Controls.Add(this.label_ReplaceTab);
			this.groupBox_Display_ControlChars.Controls.Add(this.label_ReplaceBackspace);
			this.groupBox_Display_ControlChars.Controls.Add(this.checkBox_BeepOnBell);
			this.groupBox_Display_ControlChars.Controls.Add(this.checkBox_ReplaceBackspace);
			this.groupBox_Display_ControlChars.Controls.Add(this.checkBox_HideXOnXOff);
			this.groupBox_Display_ControlChars.Controls.Add(this.comboBox_ControlCharacterRadix);
			this.groupBox_Display_ControlChars.Controls.Add(this.checkBox_ReplaceTab);
			this.groupBox_Display_ControlChars.Controls.Add(this.checkBox_ReplaceControlCharacters);
			this.groupBox_Display_ControlChars.Location = new System.Drawing.Point(261, 185);
			this.groupBox_Display_ControlChars.Name = "groupBox_Display_ControlChars";
			this.groupBox_Display_ControlChars.Size = new System.Drawing.Size(251, 137);
			this.groupBox_Display_ControlChars.TabIndex = 23;
			this.groupBox_Display_ControlChars.TabStop = false;
			this.groupBox_Display_ControlChars.Text = "&ASCII Control Characters (0x00..1F, 0x7F)";
			// 
			// checkBox_BeepOnBell
			// 
			this.checkBox_BeepOnBell.AutoSize = true;
			this.checkBox_BeepOnBell.Location = new System.Drawing.Point(6, 90);
			this.checkBox_BeepOnBell.Name = "checkBox_BeepOnBell";
			this.checkBox_BeepOnBell.Size = new System.Drawing.Size(117, 17);
			this.checkBox_BeepOnBell.TabIndex = 6;
			this.checkBox_BeepOnBell.Text = "Beep on bell (0x07)";
			this.toolTip.SetToolTip(this.checkBox_BeepOnBell, "Applicable to text terminals.\r\nApplicable to [String] and [Character] radix.");
			this.checkBox_BeepOnBell.UseVisualStyleBackColor = true;
			this.checkBox_BeepOnBell.CheckedChanged += new System.EventHandler(this.checkBox_BeepOnBell_CheckedChanged);
			// 
			// label_ReplaceBackspace
			// 
			this.label_ReplaceBackspace.AutoSize = true;
			this.label_ReplaceBackspace.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label_ReplaceBackspace.Font = new System.Drawing.Font("DejaVu Sans Mono", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_ReplaceBackspace.Location = new System.Drawing.Point(192, 46);
			this.label_ReplaceBackspace.Name = "label_ReplaceBackspace";
			this.label_ReplaceBackspace.Size = new System.Drawing.Size(37, 15);
			this.label_ReplaceBackspace.TabIndex = 3;
			this.label_ReplaceBackspace.Text = "<BS>";
			// 
			// checkBox_ReplaceBackspace
			// 
			this.checkBox_ReplaceBackspace.AutoSize = true;
			this.checkBox_ReplaceBackspace.Location = new System.Drawing.Point(6, 44);
			this.checkBox_ReplaceBackspace.Name = "checkBox_ReplaceBackspace";
			this.checkBox_ReplaceBackspace.Size = new System.Drawing.Size(186, 17);
			this.checkBox_ReplaceBackspace.TabIndex = 2;
			this.checkBox_ReplaceBackspace.Text = "Also replace backspace (0x08) by";
			this.toolTip.SetToolTip(this.checkBox_ReplaceBackspace, "Replaces backspace by <BS> instead of executing the backspace, i.e. removing the " +
        "previous character.\r\n\r\nApplicable to text terminals.\r\nApplicable to [String] and" +
        " [Character] radix.");
			this.checkBox_ReplaceBackspace.UseVisualStyleBackColor = true;
			this.checkBox_ReplaceBackspace.CheckedChanged += new System.EventHandler(this.checkBox_ReplaceBackspace_CheckedChanged);
			// 
			// label_ReplaceTab
			// 
			this.label_ReplaceTab.AutoSize = true;
			this.label_ReplaceTab.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label_ReplaceTab.Font = new System.Drawing.Font("DejaVu Sans Mono", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_ReplaceTab.Location = new System.Drawing.Point(202, 69);
			this.label_ReplaceTab.Name = "label_ReplaceTab";
			this.label_ReplaceTab.Size = new System.Drawing.Size(44, 15);
			this.label_ReplaceTab.TabIndex = 5;
			this.label_ReplaceTab.Text = "<TAB>";
			// 
			// checkBox_HideXOnXOff
			// 
			this.checkBox_HideXOnXOff.AutoSize = true;
			this.checkBox_HideXOnXOff.Location = new System.Drawing.Point(6, 113);
			this.checkBox_HideXOnXOff.Name = "checkBox_HideXOnXOff";
			this.checkBox_HideXOnXOff.Size = new System.Drawing.Size(147, 17);
			this.checkBox_HideXOnXOff.TabIndex = 7;
			this.checkBox_HideXOnXOff.Text = "Hide XOn/XOff (0x11/13)";
			this.checkBox_HideXOnXOff.UseVisualStyleBackColor = true;
			this.checkBox_HideXOnXOff.CheckedChanged += new System.EventHandler(this.checkBox_HideXOnXOff_CheckedChanged);
			// 
			// comboBox_ControlCharacterRadix
			// 
			this.comboBox_ControlCharacterRadix.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_ControlCharacterRadix.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_ControlCharacterRadix.Location = new System.Drawing.Point(124, 19);
			this.comboBox_ControlCharacterRadix.Name = "comboBox_ControlCharacterRadix";
			this.comboBox_ControlCharacterRadix.Size = new System.Drawing.Size(121, 21);
			this.comboBox_ControlCharacterRadix.TabIndex = 1;
			this.comboBox_ControlCharacterRadix.SelectedIndexChanged += new System.EventHandler(this.comboBox_ControlCharacterRadix_SelectedIndexChanged);
			// 
			// checkBox_ReplaceTab
			// 
			this.checkBox_ReplaceTab.AutoSize = true;
			this.checkBox_ReplaceTab.Location = new System.Drawing.Point(6, 67);
			this.checkBox_ReplaceTab.Name = "checkBox_ReplaceTab";
			this.checkBox_ReplaceTab.Size = new System.Drawing.Size(196, 17);
			this.checkBox_ReplaceTab.TabIndex = 4;
			this.checkBox_ReplaceTab.Text = "Also replace horizontal tab (0x09) by";
			this.toolTip.SetToolTip(this.checkBox_ReplaceTab, "Replaces tabulator characters by <TAB> instead of executing the tabulator, i.e. i" +
        "nserting a horizontal whitespace.\r\n\r\nApplicable to text terminals.\r\nApplicable t" +
        "o [String] and [Character] radix.");
			this.checkBox_ReplaceTab.UseVisualStyleBackColor = true;
			this.checkBox_ReplaceTab.CheckedChanged += new System.EventHandler(this.checkBox_ReplaceTab_CheckedChanged);
			// 
			// checkBox_ReplaceControlCharacters
			// 
			this.checkBox_ReplaceControlCharacters.AutoSize = true;
			this.checkBox_ReplaceControlCharacters.Location = new System.Drawing.Point(6, 21);
			this.checkBox_ReplaceControlCharacters.Name = "checkBox_ReplaceControlCharacters";
			this.checkBox_ReplaceControlCharacters.Size = new System.Drawing.Size(83, 17);
			this.checkBox_ReplaceControlCharacters.TabIndex = 0;
			this.checkBox_ReplaceControlCharacters.Text = "Replace by:";
			this.toolTip.SetToolTip(this.checkBox_ReplaceControlCharacters, "Applicable to [String] and [Character] radix.");
			this.checkBox_ReplaceControlCharacters.UseVisualStyleBackColor = true;
			this.checkBox_ReplaceControlCharacters.CheckedChanged += new System.EventHandler(this.checkBox_ReplaceControlCharacters_CheckedChanged);
			// 
			// comboBox_RxRadix
			// 
			this.comboBox_RxRadix.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_RxRadix.Location = new System.Drawing.Point(131, 90);
			this.comboBox_RxRadix.Name = "comboBox_RxRadix";
			this.comboBox_RxRadix.Size = new System.Drawing.Size(121, 21);
			this.comboBox_RxRadix.TabIndex = 6;
			this.comboBox_RxRadix.SelectedIndexChanged += new System.EventHandler(this.comboBox_RxRadix_SelectedIndexChanged);
			// 
			// label_RxRadix
			// 
			this.label_RxRadix.AutoSize = true;
			this.label_RxRadix.Location = new System.Drawing.Point(9, 93);
			this.label_RxRadix.Name = "label_RxRadix";
			this.label_RxRadix.Size = new System.Drawing.Size(53, 13);
			this.label_RxRadix.TabIndex = 5;
			this.label_RxRadix.Text = "Rx Radix:";
			// 
			// checkBox_SeparateTxRxRadix
			// 
			this.checkBox_SeparateTxRxRadix.AutoSize = true;
			this.checkBox_SeparateTxRxRadix.Location = new System.Drawing.Point(12, 68);
			this.checkBox_SeparateTxRxRadix.Name = "checkBox_SeparateTxRxRadix";
			this.checkBox_SeparateTxRxRadix.Size = new System.Drawing.Size(161, 17);
			this.checkBox_SeparateTxRxRadix.TabIndex = 4;
			this.checkBox_SeparateTxRxRadix.Text = "Separate radix for Tx and Rx";
			this.checkBox_SeparateTxRxRadix.UseVisualStyleBackColor = true;
			this.checkBox_SeparateTxRxRadix.CheckedChanged += new System.EventHandler(this.checkBox_SeparateTxRxRadix_CheckedChanged);
			// 
			// checkBox_ShowRadix
			// 
			this.checkBox_ShowRadix.AutoSize = true;
			this.checkBox_ShowRadix.Location = new System.Drawing.Point(12, 116);
			this.checkBox_ShowRadix.Name = "checkBox_ShowRadix";
			this.checkBox_ShowRadix.Size = new System.Drawing.Size(78, 17);
			this.checkBox_ShowRadix.TabIndex = 7;
			this.checkBox_ShowRadix.Text = "S&how radix";
			this.checkBox_ShowRadix.UseVisualStyleBackColor = true;
			this.checkBox_ShowRadix.CheckedChanged += new System.EventHandler(this.checkBox_ShowRadix_CheckedChanged);
			// 
			// checkBox_ShowTimeStamp
			// 
			this.checkBox_ShowTimeStamp.AutoSize = true;
			this.checkBox_ShowTimeStamp.Location = new System.Drawing.Point(12, 162);
			this.checkBox_ShowTimeStamp.Name = "checkBox_ShowTimeStamp";
			this.checkBox_ShowTimeStamp.Size = new System.Drawing.Size(106, 17);
			this.checkBox_ShowTimeStamp.TabIndex = 10;
			this.checkBox_ShowTimeStamp.Text = "Show &time stamp";
			this.toolTip.SetToolTip(this.checkBox_ShowTimeStamp, "The absolute moment in time.\r\n\r\nFormat can be configured in [View > Format...].");
			this.checkBox_ShowTimeStamp.UseVisualStyleBackColor = true;
			this.checkBox_ShowTimeStamp.CheckedChanged += new System.EventHandler(this.checkBox_ShowTimeStamp_CheckedChanged);
			// 
			// checkBox_ShowTimeSpan
			// 
			this.checkBox_ShowTimeSpan.AutoSize = true;
			this.checkBox_ShowTimeSpan.Location = new System.Drawing.Point(12, 185);
			this.checkBox_ShowTimeSpan.Name = "checkBox_ShowTimeSpan";
			this.checkBox_ShowTimeSpan.Size = new System.Drawing.Size(101, 17);
			this.checkBox_ShowTimeSpan.TabIndex = 11;
			this.checkBox_ShowTimeSpan.Text = "Show time span";
			this.toolTip.SetToolTip(this.checkBox_ShowTimeSpan, "The time that passed relative to [connect time].\r\n\r\nFormat can be configured in [" +
        "View > Format...].");
			this.checkBox_ShowTimeSpan.UseVisualStyleBackColor = true;
			this.checkBox_ShowTimeSpan.CheckedChanged += new System.EventHandler(this.checkBox_ShowTimeSpan_CheckedChanged);
			// 
			// checkBox_ShowTimeDelta
			// 
			this.checkBox_ShowTimeDelta.AutoSize = true;
			this.checkBox_ShowTimeDelta.Location = new System.Drawing.Point(12, 208);
			this.checkBox_ShowTimeDelta.Name = "checkBox_ShowTimeDelta";
			this.checkBox_ShowTimeDelta.Size = new System.Drawing.Size(101, 17);
			this.checkBox_ShowTimeDelta.TabIndex = 12;
			this.checkBox_ShowTimeDelta.Text = "Show time delta";
			this.toolTip.SetToolTip(this.checkBox_ShowTimeDelta, "The time that passed relative to the last line.\r\n\r\nFormat can be configured in [V" +
        "iew > Format...].");
			this.checkBox_ShowTimeDelta.UseVisualStyleBackColor = true;
			this.checkBox_ShowTimeDelta.CheckedChanged += new System.EventHandler(this.checkBox_ShowTimeDelta_CheckedChanged);
			// 
			// checkBox_ShowConnectTime
			// 
			this.checkBox_ShowConnectTime.AutoSize = true;
			this.checkBox_ShowConnectTime.Location = new System.Drawing.Point(12, 20);
			this.checkBox_ShowConnectTime.Name = "checkBox_ShowConnectTime";
			this.checkBox_ShowConnectTime.Size = new System.Drawing.Size(117, 17);
			this.checkBox_ShowConnectTime.TabIndex = 0;
			this.checkBox_ShowConnectTime.Text = "&Show connect time";
			this.checkBox_ShowConnectTime.UseVisualStyleBackColor = true;
			this.checkBox_ShowConnectTime.CheckedChanged += new System.EventHandler(this.checkBox_ShowConnectTime_CheckedChanged);
			// 
			// checkBox_ShowCountAndRate
			// 
			this.checkBox_ShowCountAndRate.AutoSize = true;
			this.checkBox_ShowCountAndRate.Location = new System.Drawing.Point(131, 20);
			this.checkBox_ShowCountAndRate.Name = "checkBox_ShowCountAndRate";
			this.checkBox_ShowCountAndRate.Size = new System.Drawing.Size(125, 17);
			this.checkBox_ShowCountAndRate.TabIndex = 1;
			this.checkBox_ShowCountAndRate.Text = "Show count and rate";
			this.checkBox_ShowCountAndRate.UseVisualStyleBackColor = true;
			this.checkBox_ShowCountAndRate.CheckedChanged += new System.EventHandler(this.checkBox_ShowCountAndRate_CheckedChanged);
			// 
			// checkBox_ShowLength
			// 
			this.checkBox_ShowLength.AutoSize = true;
			this.checkBox_ShowLength.Location = new System.Drawing.Point(12, 231);
			this.checkBox_ShowLength.Name = "checkBox_ShowLength";
			this.checkBox_ShowLength.Size = new System.Drawing.Size(88, 17);
			this.checkBox_ShowLength.TabIndex = 15;
			this.checkBox_ShowLength.Text = "Show &length:";
			this.checkBox_ShowLength.UseVisualStyleBackColor = true;
			this.checkBox_ShowLength.CheckedChanged += new System.EventHandler(this.checkBox_ShowLength_CheckedChanged);
			// 
			// comboBox_TxRadix
			// 
			this.comboBox_TxRadix.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_TxRadix.Location = new System.Drawing.Point(131, 42);
			this.comboBox_TxRadix.Name = "comboBox_TxRadix";
			this.comboBox_TxRadix.Size = new System.Drawing.Size(121, 21);
			this.comboBox_TxRadix.TabIndex = 3;
			this.comboBox_TxRadix.SelectedIndexChanged += new System.EventHandler(this.comboBox_TxRadix_SelectedIndexChanged);
			// 
			// label_TxRadix
			// 
			this.label_TxRadix.AutoSize = true;
			this.label_TxRadix.Location = new System.Drawing.Point(9, 45);
			this.label_TxRadix.Name = "label_TxRadix";
			this.label_TxRadix.Size = new System.Drawing.Size(37, 13);
			this.label_TxRadix.TabIndex = 2;
			this.label_TxRadix.Text = "&Radix:";
			// 
			// label_TextSettingsRemark
			// 
			this.label_TextSettingsRemark.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label_TextSettingsRemark.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_TextSettingsRemark.ForeColor = System.Drawing.SystemColors.GrayText;
			this.label_TextSettingsRemark.Location = new System.Drawing.Point(799, 131);
			this.label_TextSettingsRemark.Name = "label_TextSettingsRemark";
			this.label_TextSettingsRemark.Size = new System.Drawing.Size(103, 487);
			this.label_TextSettingsRemark.TabIndex = 7;
			this.label_TextSettingsRemark.Text = "Also see\r\n[Text Settings...]";
			this.label_TextSettingsRemark.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// AdvancedTerminalSettings
			// 
			this.AcceptButton = this.button_OK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button_Cancel;
			this.ClientSize = new System.Drawing.Size(900, 631);
			this.Controls.Add(this.groupBox_User);
			this.Controls.Add(this.groupBox_Send);
			this.Controls.Add(this.groupBox_Communication);
			this.Controls.Add(this.button_Defaults);
			this.Controls.Add(this.button_Cancel);
			this.Controls.Add(this.groupBox_Display);
			this.Controls.Add(this.button_OK);
			this.Controls.Add(this.label_TextSettingsRemark);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AdvancedTerminalSettings";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Advanced Terminal Settings";
			this.Shown += new System.EventHandler(this.AdvancedTerminalSettings_Shown);
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
			this.groupBox_Line.ResumeLayout(false);
			this.groupBox_Line.PerformLayout();
			this.groupBox_Display_UsbSerialHid.ResumeLayout(false);
			this.groupBox_Display_UsbSerialHid.PerformLayout();
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
		private System.Windows.Forms.GroupBox groupBox_Send;
		private System.Windows.Forms.GroupBox groupBox_Display;
		private System.Windows.Forms.CheckBox checkBox_KeepSendText;
		private System.Windows.Forms.CheckBox checkBox_ShowTimeStamp;
		private System.Windows.Forms.CheckBox checkBox_ShowTimeSpan;
		private System.Windows.Forms.CheckBox checkBox_ShowTimeDelta;
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
		private System.Windows.Forms.GroupBox groupBox_Display_ControlChars;
		private System.Windows.Forms.ComboBox comboBox_ControlCharacterRadix;
		private System.Windows.Forms.CheckBox checkBox_ReplaceControlCharacters;
		private System.Windows.Forms.GroupBox groupBox_Display_Space;
		private System.Windows.Forms.Label label_SpaceReplacementChar;
		private System.Windows.Forms.CheckBox checkBox_ReplaceSpace;
		private System.Windows.Forms.CheckBox checkBox_ShowConnectTime;
		private System.Windows.Forms.CheckBox checkBox_CopyPredefined;
		private System.Windows.Forms.CheckBox checkBox_ShowRadix;
		private System.Windows.Forms.CheckBox checkBox_SendImmediately;
		private System.Windows.Forms.GroupBox groupBox_User;
		private MKY.Windows.Forms.TextBoxEx textBox_UserName;
		private System.Windows.Forms.Label label_UserName;
		private System.Windows.Forms.CheckBox checkBox_NoSendOnInputBreak;
		private System.Windows.Forms.CheckBox checkBox_NoSendOnOutputBreak;
		private System.Windows.Forms.GroupBox groupBox_Communication_SerialPorts;
		private System.Windows.Forms.CheckBox checkBox_OutputBreakModifiable;
		private System.Windows.Forms.CheckBox checkBox_IndicateBreakStates;
		private System.Windows.Forms.CheckBox checkBox_ShowLineNumbers;
		private MKY.Windows.Forms.TextBoxEx textBox_OutputBufferSize;
		private System.Windows.Forms.Label label_OutputBufferSizeUnit;
		private System.Windows.Forms.CheckBox checkBox_ShowBreakCount;
		private System.Windows.Forms.CheckBox checkBox_ShowFlowControlCount;
		private System.Windows.Forms.CheckBox checkBox_HideXOnXOff;
		private System.Windows.Forms.CheckBox checkBox_EnableEscapesForFile;
		private System.Windows.Forms.GroupBox groupBox_Send_Keywords;
		private System.Windows.Forms.Label label_DefaultLineDelayUnit;
		private MKY.Windows.Forms.TextBoxEx textBox_DefaultLineDelay;
		private System.Windows.Forms.Label label_DefaultLineDelay;
		private System.Windows.Forms.Label label_DefaultDelayUnit;
		private MKY.Windows.Forms.TextBoxEx textBox_DefaultDelay;
		private System.Windows.Forms.Label label_DefaultDelay;
		private System.Windows.Forms.Label label_DefaultLineRepeatUnit;
		private MKY.Windows.Forms.TextBoxEx textBox_DefaultLineRepeat;
		private System.Windows.Forms.Label label_DefaultLineRepeat;
		private System.Windows.Forms.Label label_ReplaceTab;
		private System.Windows.Forms.CheckBox checkBox_ReplaceTab;
		private System.Windows.Forms.GroupBox groupBox_Send_SerialPorts;
		private System.Windows.Forms.CheckBox checkBox_OutputBufferSize;
		private MKY.Windows.Forms.TextBoxEx textBox_MaxChunkSize;
		private System.Windows.Forms.CheckBox checkBox_MaxChunkSizeEnable;
		private System.Windows.Forms.Label label_MaxChunkSizeUnit;
		private MKY.Windows.Forms.TextBoxEx textBox_MaxSendRateSize;
		private MKY.Windows.Forms.TextBoxEx textBox_MaxSendRateInterval;
		private System.Windows.Forms.Label label_MaxSendRateIntervalUnit1;
		private System.Windows.Forms.Label label_MaxSendRateIntervalUnit2;
		private System.Windows.Forms.CheckBox checkBox_MaxSendRateEnable;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.CheckBox checkBox_ShowDirection;
		private System.Windows.Forms.CheckBox checkBox_SignalXOnPeriodicallyEnable;
		private System.Windows.Forms.CheckBox checkBox_SignalXOnBeforeEachTransmission;
		private System.Windows.Forms.Label label_SignalXOnPeriodicallyIntervalUnit;
		private MKY.Windows.Forms.TextBoxEx textBox_SignalXOnPeriodicallyInterval;
		private System.Windows.Forms.Label label_ReplaceSpaceUnicode;
		private System.Windows.Forms.GroupBox groupBox_Display_Special;
		private System.Windows.Forms.CheckBox checkBox_Hide0x00;
		private System.Windows.Forms.CheckBox checkBox_Hide0xFF;
		private System.Windows.Forms.Label label_DefaultLineIntervalUnit;
		private MKY.Windows.Forms.TextBoxEx textBox_DefaultLineInterval;
		private System.Windows.Forms.Label label_DefaultLineInterval;
		private System.Windows.Forms.CheckBox checkBox_ShowDevice;
		private System.Windows.Forms.CheckBox checkBox_UseExplicitDefaultRadix;
		private System.Windows.Forms.CheckBox checkBox_BufferMaxBaudRate;
		private System.Windows.Forms.GroupBox groupBox_Display_UsbSerialHid;
		private System.Windows.Forms.CheckBox checkBox_IncludeNonPayloadData;
		private System.Windows.Forms.CheckBox checkBox_IgnoreFramingErrors;
		private System.Windows.Forms.CheckBox checkBox_ShowCopyOfActiveLine;
		private System.Windows.Forms.CheckBox checkBox_EnableEscapesForText;
		private System.Windows.Forms.CheckBox checkBox_SkipEmptyLines;
		private System.Windows.Forms.CheckBox checkBox_ShowDuration;
		private System.Windows.Forms.ComboBox comboBox_LineNumberSelection;
		private System.Windows.Forms.CheckBox checkBox_IncludeIOControl;
		private System.Windows.Forms.ComboBox comboBox_LengthSelection;
		private System.Windows.Forms.CheckBox checkBox_ReplaceBackspace;
		private System.Windows.Forms.Label label_ReplaceBackspace;
		private System.Windows.Forms.CheckBox checkBox_BeepOnBell;
		private System.Windows.Forms.Label label_TextSettingsRemark;
		private System.Windows.Forms.CheckBox checkBox_AllowConcurrency;
		private System.Windows.Forms.CheckBox checkBox_SignalXOnWhenOpened;
		private System.Windows.Forms.GroupBox groupBox_Line;
		private System.Windows.Forms.Label label_GlueCharsOfLine;
		private System.Windows.Forms.CheckBox checkBox_GlueCharsOfLine;
		private System.Windows.Forms.Label label_GlueCharsOfLineTimeoutUnit;
		private MKY.Windows.Forms.TextBoxEx textBox_GlueCharsOfLineTimeout;
		private System.Windows.Forms.Label label_LineBreaks;
		private MKY.Windows.Forms.TextBoxEx textBox_MaxLineLength;
		private System.Windows.Forms.Label label_MaxLineLengthUnit;
		private System.Windows.Forms.Label label_MaxLineLength;
		private System.Windows.Forms.CheckBox checkBox_DeviceLineBreak;
		private MKY.Windows.Forms.TextBoxEx textBox_MaxLineCount;
		private System.Windows.Forms.CheckBox checkBox_DirectionLineBreak;
		private System.Windows.Forms.Label label_MaxLineCountUnit;
		private System.Windows.Forms.Label label_MaxLineCount;
		private System.Windows.Forms.Label label_LineBreakRemark;
		private System.Windows.Forms.CheckBox checkBox_IncludeIOWarnings;
	}
}