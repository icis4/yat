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
			this.groupBox_CommunicationSettings = new System.Windows.Forms.GroupBox();
			this.comboBox_Endianess = new System.Windows.Forms.ComboBox();
			this.label_Endianess = new System.Windows.Forms.Label();
			this.groupBox_ReceiveSettings = new System.Windows.Forms.GroupBox();
			this.checkBox_ReplaceParityError = new System.Windows.Forms.CheckBox();
			this.label_ParityReplacementExample = new System.Windows.Forms.Label();
			this.textBox_ParityReplacement = new System.Windows.Forms.TextBox();
			this.groupBox_SendCommandSettings = new System.Windows.Forms.GroupBox();
			this.label_SendImmediately = new System.Windows.Forms.Label();
			this.checkBox_SendImmediately = new System.Windows.Forms.CheckBox();
			this.checkBox_CopyPredefined = new System.Windows.Forms.CheckBox();
			this.checkBox_KeepCommand = new System.Windows.Forms.CheckBox();
			this.groupBox_DisplaySettings = new System.Windows.Forms.GroupBox();
			this.groupBox_Display_Tab = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
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
			this.checkBox_ShowCounters = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowLength = new System.Windows.Forms.CheckBox();
			this.comboBox_TxRadix = new System.Windows.Forms.ComboBox();
			this.label_TxRadix = new System.Windows.Forms.Label();
			this.label_MaxLineCountUnit = new System.Windows.Forms.Label();
			this.textBox_MaxLineCount = new System.Windows.Forms.TextBox();
			this.label_MaxLineCount = new System.Windows.Forms.Label();
			this.groupBox_Settings.SuspendLayout();
			this.groupBox_CommunicationSettings.SuspendLayout();
			this.groupBox_ReceiveSettings.SuspendLayout();
			this.groupBox_SendCommandSettings.SuspendLayout();
			this.groupBox_DisplaySettings.SuspendLayout();
			this.groupBox_Display_Tab.SuspendLayout();
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
			this.groupBox_Settings.Controls.Add(this.groupBox_CommunicationSettings);
			this.groupBox_Settings.Controls.Add(this.groupBox_ReceiveSettings);
			this.groupBox_Settings.Controls.Add(this.groupBox_SendCommandSettings);
			this.groupBox_Settings.Controls.Add(this.groupBox_DisplaySettings);
			this.groupBox_Settings.Location = new System.Drawing.Point(12, 12);
			this.groupBox_Settings.Name = "groupBox_Settings";
			this.groupBox_Settings.Size = new System.Drawing.Size(544, 413);
			this.groupBox_Settings.TabIndex = 0;
			this.groupBox_Settings.TabStop = false;
			// 
			// groupBox_CommunicationSettings
			// 
			this.groupBox_CommunicationSettings.Controls.Add(this.comboBox_Endianess);
			this.groupBox_CommunicationSettings.Controls.Add(this.label_Endianess);
			this.groupBox_CommunicationSettings.Location = new System.Drawing.Point(275, 13);
			this.groupBox_CommunicationSettings.Name = "groupBox_CommunicationSettings";
			this.groupBox_CommunicationSettings.Size = new System.Drawing.Size(263, 48);
			this.groupBox_CommunicationSettings.TabIndex = 1;
			this.groupBox_CommunicationSettings.TabStop = false;
			this.groupBox_CommunicationSettings.Text = "Communication Settings";
			// 
			// comboBox_Endianess
			// 
			this.comboBox_Endianess.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_Endianess.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_Endianess.Location = new System.Drawing.Point(70, 18);
			this.comboBox_Endianess.Name = "comboBox_Endianess";
			this.comboBox_Endianess.Size = new System.Drawing.Size(182, 21);
			this.comboBox_Endianess.TabIndex = 1;
			this.comboBox_Endianess.SelectedIndexChanged += new System.EventHandler(this.comboBox_Endianess_SelectedIndexChanged);
			// 
			// label_Endianess
			// 
			this.label_Endianess.AutoSize = true;
			this.label_Endianess.Location = new System.Drawing.Point(9, 21);
			this.label_Endianess.Name = "label_Endianess";
			this.label_Endianess.Size = new System.Drawing.Size(59, 13);
			this.label_Endianess.TabIndex = 0;
			this.label_Endianess.Text = "&Endianess:";
			// 
			// groupBox_ReceiveSettings
			// 
			this.groupBox_ReceiveSettings.Controls.Add(this.checkBox_ReplaceParityError);
			this.groupBox_ReceiveSettings.Controls.Add(this.label_ParityReplacementExample);
			this.groupBox_ReceiveSettings.Controls.Add(this.textBox_ParityReplacement);
			this.groupBox_ReceiveSettings.Location = new System.Drawing.Point(275, 177);
			this.groupBox_ReceiveSettings.Name = "groupBox_ReceiveSettings";
			this.groupBox_ReceiveSettings.Size = new System.Drawing.Size(263, 63);
			this.groupBox_ReceiveSettings.TabIndex = 3;
			this.groupBox_ReceiveSettings.TabStop = false;
			this.groupBox_ReceiveSettings.Text = "Receive Settings";
			// 
			// checkBox_ReplaceParityError
			// 
			this.checkBox_ReplaceParityError.AutoSize = true;
			this.checkBox_ReplaceParityError.Location = new System.Drawing.Point(12, 19);
			this.checkBox_ReplaceParityError.Name = "checkBox_ReplaceParityError";
			this.checkBox_ReplaceParityError.Size = new System.Drawing.Size(137, 17);
			this.checkBox_ReplaceParityError.TabIndex = 0;
			this.checkBox_ReplaceParityError.Text = "Replace parit&y errors by";
			this.checkBox_ReplaceParityError.UseVisualStyleBackColor = true;
			this.checkBox_ReplaceParityError.CheckedChanged += new System.EventHandler(this.checkBox_ReplaceParityError_CheckedChanged);
			// 
			// label_ParityReplacementExample
			// 
			this.label_ParityReplacementExample.AutoSize = true;
			this.label_ParityReplacementExample.Location = new System.Drawing.Point(89, 40);
			this.label_ParityReplacementExample.Name = "label_ParityReplacementExample";
			this.label_ParityReplacementExample.Size = new System.Drawing.Size(129, 13);
			this.label_ParityReplacementExample.TabIndex = 2;
			this.label_ParityReplacementExample.Text = "Example: \\h(07) or <BEL>";
			// 
			// textBox_ParityReplacement
			// 
			this.textBox_ParityReplacement.Location = new System.Drawing.Point(150, 17);
			this.textBox_ParityReplacement.Name = "textBox_ParityReplacement";
			this.textBox_ParityReplacement.Size = new System.Drawing.Size(50, 20);
			this.textBox_ParityReplacement.TabIndex = 1;
			this.textBox_ParityReplacement.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_ParityReplacement_Validating);
			// 
			// groupBox_SendCommandSettings
			// 
			this.groupBox_SendCommandSettings.Controls.Add(this.label_SendImmediately);
			this.groupBox_SendCommandSettings.Controls.Add(this.checkBox_SendImmediately);
			this.groupBox_SendCommandSettings.Controls.Add(this.checkBox_CopyPredefined);
			this.groupBox_SendCommandSettings.Controls.Add(this.checkBox_KeepCommand);
			this.groupBox_SendCommandSettings.Location = new System.Drawing.Point(275, 67);
			this.groupBox_SendCommandSettings.Name = "groupBox_SendCommandSettings";
			this.groupBox_SendCommandSettings.Size = new System.Drawing.Size(263, 104);
			this.groupBox_SendCommandSettings.TabIndex = 2;
			this.groupBox_SendCommandSettings.TabStop = false;
			this.groupBox_SendCommandSettings.Text = "Send Command Settings";
			// 
			// label_SendImmediately
			// 
			this.label_SendImmediately.AutoSize = true;
			this.label_SendImmediately.Location = new System.Drawing.Point(29, 82);
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
			// groupBox_DisplaySettings
			// 
			this.groupBox_DisplaySettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox_DisplaySettings.Controls.Add(this.groupBox_Display_Tab);
			this.groupBox_DisplaySettings.Controls.Add(this.groupBox_Display_Space);
			this.groupBox_DisplaySettings.Controls.Add(this.groupBox_Display_ControlChars);
			this.groupBox_DisplaySettings.Controls.Add(this.checkBox_DirectionLineBreak);
			this.groupBox_DisplaySettings.Controls.Add(this.comboBox_RxRadix);
			this.groupBox_DisplaySettings.Controls.Add(this.label_RxRadix);
			this.groupBox_DisplaySettings.Controls.Add(this.checkBox_SeparateTxRxRadix);
			this.groupBox_DisplaySettings.Controls.Add(this.checkBox_ShowRadix);
			this.groupBox_DisplaySettings.Controls.Add(this.checkBox_ShowTimeStamp);
			this.groupBox_DisplaySettings.Controls.Add(this.checkBox_ShowConnectTime);
			this.groupBox_DisplaySettings.Controls.Add(this.checkBox_ShowCounters);
			this.groupBox_DisplaySettings.Controls.Add(this.checkBox_ShowLength);
			this.groupBox_DisplaySettings.Controls.Add(this.comboBox_TxRadix);
			this.groupBox_DisplaySettings.Controls.Add(this.label_TxRadix);
			this.groupBox_DisplaySettings.Controls.Add(this.label_MaxLineCountUnit);
			this.groupBox_DisplaySettings.Controls.Add(this.textBox_MaxLineCount);
			this.groupBox_DisplaySettings.Controls.Add(this.label_MaxLineCount);
			this.groupBox_DisplaySettings.Location = new System.Drawing.Point(6, 13);
			this.groupBox_DisplaySettings.Name = "groupBox_DisplaySettings";
			this.groupBox_DisplaySettings.Size = new System.Drawing.Size(263, 394);
			this.groupBox_DisplaySettings.TabIndex = 0;
			this.groupBox_DisplaySettings.TabStop = false;
			this.groupBox_DisplaySettings.Text = "Display Settings";
			// 
			// groupBox_Display_Tab
			// 
			this.groupBox_Display_Tab.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Display_Tab.Controls.Add(this.label1);
			this.groupBox_Display_Tab.Controls.Add(this.checkBox_ReplaceTab);
			this.groupBox_Display_Tab.Location = new System.Drawing.Point(6, 290);
			this.groupBox_Display_Tab.Name = "groupBox_Display_Tab";
			this.groupBox_Display_Tab.Size = new System.Drawing.Size(251, 46);
			this.groupBox_Display_Tab.TabIndex = 15;
			this.groupBox_Display_Tab.TabStop = false;
			this.groupBox_Display_Tab.Text = "Tab";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label1.Font = new System.Drawing.Font("DejaVu Sans Mono", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(122, 21);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(44, 15);
			this.label1.TabIndex = 1;
			this.label1.Text = "<TAB>";
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
			this.groupBox_Display_Space.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Display_Space.Controls.Add(this.label_SpaceReplacementChar);
			this.groupBox_Display_Space.Controls.Add(this.checkBox_ReplaceSpace);
			this.groupBox_Display_Space.Location = new System.Drawing.Point(6, 342);
			this.groupBox_Display_Space.Name = "groupBox_Display_Space";
			this.groupBox_Display_Space.Size = new System.Drawing.Size(251, 46);
			this.groupBox_Display_Space.TabIndex = 16;
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
			this.groupBox_Display_ControlChars.Location = new System.Drawing.Point(6, 215);
			this.groupBox_Display_ControlChars.Name = "groupBox_Display_ControlChars";
			this.groupBox_Display_ControlChars.Size = new System.Drawing.Size(251, 69);
			this.groupBox_Display_ControlChars.TabIndex = 14;
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
			this.checkBox_DirectionLineBreak.Location = new System.Drawing.Point(12, 164);
			this.checkBox_DirectionLineBreak.Name = "checkBox_DirectionLineBreak";
			this.checkBox_DirectionLineBreak.Size = new System.Drawing.Size(194, 17);
			this.checkBox_DirectionLineBreak.TabIndex = 10;
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
			this.checkBox_ShowTimeStamp.Size = new System.Drawing.Size(103, 17);
			this.checkBox_ShowTimeStamp.TabIndex = 6;
			this.checkBox_ShowTimeStamp.Text = "Show &timestamp";
			this.checkBox_ShowTimeStamp.CheckedChanged += new System.EventHandler(this.checkBox_ShowTimeStamp_CheckedChanged);
			// 
			// checkBox_ShowConnectTime
			// 
			this.checkBox_ShowConnectTime.AutoSize = true;
			this.checkBox_ShowConnectTime.Location = new System.Drawing.Point(12, 141);
			this.checkBox_ShowConnectTime.Name = "checkBox_ShowConnectTime";
			this.checkBox_ShowConnectTime.Size = new System.Drawing.Size(117, 17);
			this.checkBox_ShowConnectTime.TabIndex = 8;
			this.checkBox_ShowConnectTime.Text = "Show connect t&ime";
			this.checkBox_ShowConnectTime.CheckedChanged += new System.EventHandler(this.checkBox_ShowConnectTime_CheckedChanged);
			// 
			// checkBox_ShowCounters
			// 
			this.checkBox_ShowCounters.AutoSize = true;
			this.checkBox_ShowCounters.Location = new System.Drawing.Point(131, 141);
			this.checkBox_ShowCounters.Name = "checkBox_ShowCounters";
			this.checkBox_ShowCounters.Size = new System.Drawing.Size(97, 17);
			this.checkBox_ShowCounters.TabIndex = 9;
			this.checkBox_ShowCounters.Text = "Show &counters";
			this.checkBox_ShowCounters.CheckedChanged += new System.EventHandler(this.checkBox_ShowCounters_CheckedChanged);
			// 
			// checkBox_ShowLength
			// 
			this.checkBox_ShowLength.AutoSize = true;
			this.checkBox_ShowLength.Location = new System.Drawing.Point(131, 118);
			this.checkBox_ShowLength.Name = "checkBox_ShowLength";
			this.checkBox_ShowLength.Size = new System.Drawing.Size(85, 17);
			this.checkBox_ShowLength.TabIndex = 7;
			this.checkBox_ShowLength.Text = "Show &length";
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
			this.label_MaxLineCountUnit.Location = new System.Drawing.Point(148, 190);
			this.label_MaxLineCountUnit.Name = "label_MaxLineCountUnit";
			this.label_MaxLineCountUnit.Size = new System.Drawing.Size(28, 13);
			this.label_MaxLineCountUnit.TabIndex = 13;
			this.label_MaxLineCountUnit.Text = "lines";
			this.label_MaxLineCountUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox_MaxLineCount
			// 
			this.textBox_MaxLineCount.Location = new System.Drawing.Point(92, 187);
			this.textBox_MaxLineCount.Name = "textBox_MaxLineCount";
			this.textBox_MaxLineCount.Size = new System.Drawing.Size(50, 20);
			this.textBox_MaxLineCount.TabIndex = 12;
			this.textBox_MaxLineCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBox_MaxLineCount.TextChanged += new System.EventHandler(this.textBox_MaxLineCount_TextChanged);
			this.textBox_MaxLineCount.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_MaxLineCount_Validating);
			// 
			// label_MaxLineCount
			// 
			this.label_MaxLineCount.AutoSize = true;
			this.label_MaxLineCount.Location = new System.Drawing.Point(9, 190);
			this.label_MaxLineCount.Name = "label_MaxLineCount";
			this.label_MaxLineCount.Size = new System.Drawing.Size(81, 13);
			this.label_MaxLineCount.TabIndex = 11;
			this.label_MaxLineCount.Text = "Display &maximal";
			this.label_MaxLineCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// AdvancedTerminalSettings
			// 
			this.AcceptButton = this.button_OK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button_Cancel;
			this.ClientSize = new System.Drawing.Size(652, 437);
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
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.ExtendendTerminalSettings_Paint);
			this.groupBox_Settings.ResumeLayout(false);
			this.groupBox_CommunicationSettings.ResumeLayout(false);
			this.groupBox_CommunicationSettings.PerformLayout();
			this.groupBox_ReceiveSettings.ResumeLayout(false);
			this.groupBox_ReceiveSettings.PerformLayout();
			this.groupBox_SendCommandSettings.ResumeLayout(false);
			this.groupBox_SendCommandSettings.PerformLayout();
			this.groupBox_DisplaySettings.ResumeLayout(false);
			this.groupBox_DisplaySettings.PerformLayout();
			this.groupBox_Display_Tab.ResumeLayout(false);
			this.groupBox_Display_Tab.PerformLayout();
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
		private System.Windows.Forms.GroupBox groupBox_SendCommandSettings;
		private System.Windows.Forms.GroupBox groupBox_DisplaySettings;
		private System.Windows.Forms.GroupBox groupBox_ReceiveSettings;
		private System.Windows.Forms.TextBox textBox_ParityReplacement;
		private System.Windows.Forms.Label label_MaxLineCountUnit;
		private System.Windows.Forms.TextBox textBox_MaxLineCount;
		private System.Windows.Forms.Label label_MaxLineCount;
		private System.Windows.Forms.CheckBox checkBox_KeepCommand;
		private System.Windows.Forms.Label label_ParityReplacementExample;
		private System.Windows.Forms.CheckBox checkBox_ShowTimeStamp;
		private System.Windows.Forms.CheckBox checkBox_ShowCounters;
		private System.Windows.Forms.CheckBox checkBox_ShowLength;
		private System.Windows.Forms.ComboBox comboBox_TxRadix;
		private System.Windows.Forms.Label label_TxRadix;
		private System.Windows.Forms.GroupBox groupBox_CommunicationSettings;
		private System.Windows.Forms.ComboBox comboBox_Endianess;
		private System.Windows.Forms.Label label_Endianess;
		private System.Windows.Forms.CheckBox checkBox_SeparateTxRxRadix;
		private System.Windows.Forms.ComboBox comboBox_RxRadix;
		private System.Windows.Forms.Label label_RxRadix;
		private System.Windows.Forms.CheckBox checkBox_ReplaceParityError;
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
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox checkBox_ReplaceTab;
		private System.Windows.Forms.CheckBox checkBox_SendImmediately;
		private System.Windows.Forms.Label label_SendImmediately;
	}
}