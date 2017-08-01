//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Delta Version 1.99.80
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Windows.Forms;

using MKY;
using MKY.IO;
using MKY.Windows.Forms;

using YAT.Application.Utilities;
using YAT.Settings.Application;

#endregion

namespace YAT.View.Forms
{
	/// <summary></summary>
	public partial class LogSettings : Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;

		private Log.Settings.LogSettings settings;
		private Log.Settings.LogSettings settingsInEdit;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public LogSettings(Log.Settings.LogSettings settings)
		{
			InitializeComponent();

			KeepAndCloneAndAttachSettings(settings);
			InitializeControls();

			// SetControls() is initially called in the 'Shown' event handler.
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public Log.Settings.LogSettings SettingsResult
		{
			get { return (this.settings); }
		}

		#endregion

		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

		private void KeepAndCloneAndAttachSettings(Log.Settings.LogSettings settings)
		{
			this.settings = settings;
			this.settingsInEdit = new Log.Settings.LogSettings(settings);
			this.settingsInEdit.Changed += settings_Form_Changed;

			// Note that extensions are handled via 'ApplicationSettings'.
		}

		private void DetachAndAcceptSettings()
		{
			this.settingsInEdit.Changed -= settings_Form_Changed;
			this.settings = this.settingsInEdit;

			ApplicationSettings.LocalUserSettings.Extensions.RawLogFiles  = this.settings.RawExtension;
			ApplicationSettings.LocalUserSettings.Extensions.NeatLogFiles = this.settings.NeatExtension;
			ApplicationSettings.Save();
		}

		private void settings_Form_Changed(object sender, MKY.Settings.SettingsEventArgs e)
		{
			SetControls();
		}

		#endregion

		#region Form Event Handlers
		//==========================================================================================
		// Form Event Handlers
		//==========================================================================================

		/// <summary>
		/// Initially set controls and validate its contents where needed.
		/// </summary>
		/// <remarks>
		/// The 'Shown' event is only raised the first time a form is displayed; subsequently
		/// minimizing, maximizing, restoring, hiding, showing, or invalidating and repainting will
		/// not raise this event again.
		/// Note that the 'Shown' event is raised after the 'Load' event and will also be raised if
		/// the application is started minimized. Also note that operations called in the 'Shown'
		/// event can depend on a properly drawn form, as the 'Paint' event of this form and its
		/// child controls has been raised before this 'Shown' event.
		/// </remarks>
		private void LogSettings_Shown(object sender, EventArgs e)
		{
			SetControls();
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void pathLabel_Root_Click(object sender, EventArgs e)
		{
			ShowSetRootDirectoryDialog();
		}

		private void button_Root_Click(object sender, EventArgs e)
		{
			ShowSetRootDirectoryDialog();
		}

		[SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "YAT is not (yet) capable for RTL.")]
		private void button_RootOpen_Click(object sender, EventArgs e)
		{
			Exception ex;
			if (!DirectoryEx.TryOpen(this.settingsInEdit.RootPath, out ex))
			{
				string message = "Unable to open folder." + Environment.NewLine + Environment.NewLine +
				                 "System error message:" + Environment.NewLine + ex.Message;

				MessageBox.Show
				(
					Parent,
					message,
					"Folder Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Warning
				);
			}
		}

		private void checkBox_Raw_Tx_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsInEdit.RawLogTx = checkBox_Raw_Tx.Checked;
		}

		private void pathLabel_Raw_Tx_Click(object sender, EventArgs e)
		{
			this.settingsInEdit.RawLogTx = !this.settingsInEdit.RawLogTx;
		}

		private void checkBox_Raw_Bidir_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsInEdit.RawLogBidir = checkBox_Raw_Bidir.Checked;
		}

		private void pathLabel_Raw_Bidir_Click(object sender, EventArgs e)
		{
			this.settingsInEdit.RawLogBidir = !this.settingsInEdit.RawLogBidir;
		}

		private void checkBox_Raw_Rx_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsInEdit.RawLogRx = checkBox_Raw_Rx.Checked;
		}

		private void pathLabel_Raw_Rx_Click(object sender, EventArgs e)
		{
			this.settingsInEdit.RawLogRx = !this.settingsInEdit.RawLogRx;
		}

		private void comboBox_Raw_Extension_Validating(object sender, CancelEventArgs e)
		{
			if (!ValidateFileNamePart(comboBox_Raw_Extension.Text, "Extension"))
			{
				e.Cancel = true;
				return;
			}
			if ((this.settingsInEdit.BothRawAndNeat && (comboBox_Raw_Extension.Text == this.settingsInEdit.NeatExtension)) &&
			    (!(this.settingsInEdit.FolderFormat || this.settingsInEdit.NameFormat)))
			{
				ExtensionConflictMessage();
				e.Cancel = true;
				return;
			}
			SetControls();
		}

		private void comboBox_Raw_Extension_TextChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsInEdit.RawExtension = comboBox_Raw_Extension.Text;
		}

		private void checkBox_Neat_Tx_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsInEdit.NeatLogTx = checkBox_Neat_Tx.Checked;
		}

		private void pathLabel_Neat_Tx_Click(object sender, EventArgs e)
		{
			this.settingsInEdit.NeatLogTx = !this.settingsInEdit.NeatLogTx;
		}

		private void checkBox_Neat_Bidir_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsInEdit.NeatLogBidir = checkBox_Neat_Bidir.Checked;
		}

		private void pathLabel_Neat_Bidir_Click(object sender, EventArgs e)
		{
			this.settingsInEdit.NeatLogBidir = !this.settingsInEdit.NeatLogBidir;
		}

		private void checkBox_Neat_Rx_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsInEdit.NeatLogRx = checkBox_Neat_Rx.Checked;
		}

		private void pathLabel_Neat_Rx_Click(object sender, EventArgs e)
		{
			this.settingsInEdit.NeatLogRx = !this.settingsInEdit.NeatLogRx;
		}

		private void comboBox_Neat_Extension_Validating(object sender, CancelEventArgs e)
		{
			if (!ValidateFileNamePart(comboBox_Neat_Extension.Text, "Extension"))
			{
				e.Cancel = true;
				return;
			}
			if ((this.settingsInEdit.BothRawAndNeat && (comboBox_Neat_Extension.Text == this.settingsInEdit.RawExtension)) &&
			    (!(this.settingsInEdit.FolderFormat || this.settingsInEdit.NameFormat)))
			{
				ExtensionConflictMessage();
				e.Cancel = true;
				return;
			}
			SetControls();
		}

		private void comboBox_Neat_Extension_TextChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsInEdit.NeatExtension = comboBox_Neat_Extension.Text;
		}

		private void checkBox_Options_NameFormat_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsInEdit.NameFormat = checkBox_Options_NameFormat.Checked;
		}

		private void checkBox_Options_NameChannel_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsInEdit.NameChannel = checkBox_Options_NameChannel.Checked;
		}

		private void checkBox_Options_NameDate_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsInEdit.NameDate = checkBox_Options_NameDate.Checked;
		}

		private void checkBox_Options_NameTime_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsInEdit.NameTime = checkBox_Options_NameTime.Checked;
		}

		private void comboBox_Options_NameSeparator_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				var enclosure = (comboBox_Options_NameSeparator.SelectedItem as Log.FileNameSeparatorEx);
				if (enclosure != null)
					this.settingsInEdit.NameSeparator = enclosure;
			}
		}

		private void comboBox_Options_NameSeparator_Validating(object sender, CancelEventArgs e)
		{
			if (ValidateFileNamePart(comboBox_Options_NameSeparator.Text, "Separator"))
			{
				if (!this.isSettingControls)
					this.settingsInEdit.NameSeparator = comboBox_Options_NameSeparator.Text;
			}
			else
			{
				e.Cancel = true;
			}
		}

		private void checkBox_Options_FolderFormat_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsInEdit.FolderFormat = checkBox_Options_FolderFormat.Checked;
		}

		private void checkBox_Options_FolderChannel_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsInEdit.FolderChannel = checkBox_Options_FolderChannel.Checked;
		}

		private void radioButton_Options_ModeCreate_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls && radioButton_Options_ModeCreate.Checked)
			{
				this.settingsInEdit.WriteMode = Log.LogFileWriteMode.Create;
				this.settingsInEdit.NameDate = this.settings.NameDate;
				this.settingsInEdit.NameTime = this.settings.NameTime;
			}
		}

		private void radioButton_Options_ModeAppend_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls && radioButton_Options_ModeAppend.Checked)
			{
				this.settingsInEdit.WriteMode = Log.LogFileWriteMode.Append;
				this.settingsInEdit.NameDate = false;
				this.settingsInEdit.NameTime = false;
			}
		}

		private void radioButton_Options_TextEncodingUTF8_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls && radioButton_Options_TextEncodingUTF8.Checked)
				this.settingsInEdit.TextEncoding = Log.TextEncoding.UTF8;
		}

		private void radioButton_Options_TextEncodingTerminal_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls && radioButton_Options_TextEncodingTerminal.Checked)
				this.settingsInEdit.TextEncoding = Log.TextEncoding.Terminal;
		}

		private void checkBox_Options_EmitEncodingPreamble_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsInEdit.EmitEncodingPreamble = checkBox_Options_EmitEncodingPreamble.Checked;
		}

		private void button_OK_Click(object sender, EventArgs e)
		{
			if (ResolveNamingConflicts())
				DetachAndAcceptSettings();
			else
				DialogResult = DialogResult.None;
		}

		private void button_Cancel_Click(object sender, EventArgs e)
		{
			// Do nothing.
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void button_Defaults_Click(object sender, EventArgs e)
		{
			if (MessageBoxEx.Show
				(
				this,
				"Reset all settings to default values?",
				"Defaults?",
				MessageBoxButtons.YesNoCancel,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button3
				)
				== DialogResult.Yes)
			{
				this.settingsInEdit.SetDefaults();

				// Note that extensions are handled via 'ApplicationSettings'.
				// The 'ApplicationSettings' can only be reset to defaults via the 'Preferences' dialog.
			}
		}

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		private void InitializeControls()
		{
			this.isSettingControls.Enter();

			comboBox_Raw_Extension.Items.Clear();
			foreach (string s in ExtensionHelper.BinaryFilesWithDot)
				comboBox_Raw_Extension.Items.Add(s);

			comboBox_Neat_Extension.Items.Clear();
			foreach (string s in ExtensionHelper.TextFilesWithDot)
				comboBox_Neat_Extension.Items.Add(s);

			comboBox_Options_NameSeparator.Items.Clear();
			foreach (string s in Log.FileNameSeparatorEx.GetItems())
				comboBox_Options_NameSeparator.Items.Add(s);

			this.isSettingControls.Leave();
		}

		private void SetControls()
		{
			this.isSettingControls.Enter();

			if (!string.IsNullOrEmpty(this.settingsInEdit.RootPath))
				pathLabel_Root.Text = this.settingsInEdit.RootPath + Path.DirectorySeparatorChar + this.settingsInEdit.RootFileName;
			else
				pathLabel_Root.Text = "<Set a root file...>";

			checkBox_Raw_Tx.Checked    = this.settingsInEdit.RawLogTx;
			checkBox_Raw_Bidir.Checked = this.settingsInEdit.RawLogBidir;
			checkBox_Raw_Rx.Checked    = this.settingsInEdit.RawLogRx;

			pathLabel_Raw_Tx.Text    = this.settingsInEdit.RawTxRootRelativeFilePath;
			pathLabel_Raw_Bidir.Text = this.settingsInEdit.RawBidirRootRelativeFilePath;
			pathLabel_Raw_Rx.Text    = this.settingsInEdit.RawRxRootRelativeFilePath;

			comboBox_Raw_Extension.Text = this.settingsInEdit.RawExtension;

			checkBox_Neat_Tx.Checked    = this.settingsInEdit.NeatLogTx;
			checkBox_Neat_Bidir.Checked = this.settingsInEdit.NeatLogBidir;
			checkBox_Neat_Rx.Checked    = this.settingsInEdit.NeatLogRx;

			pathLabel_Neat_Tx.Text    = this.settingsInEdit.NeatTxRootRelativeFilePath;
			pathLabel_Neat_Bidir.Text = this.settingsInEdit.NeatBidirRootRelativeFilePath;
			pathLabel_Neat_Rx.Text    = this.settingsInEdit.NeatRxRootRelativeFilePath;

			comboBox_Neat_Extension.Text = this.settingsInEdit.NeatExtension;

			checkBox_Options_NameFormat.Checked  = this.settingsInEdit.NameFormat;
			checkBox_Options_NameChannel.Checked = this.settingsInEdit.NameChannel;
			checkBox_Options_NameDate.Checked    = this.settingsInEdit.NameDate;
			checkBox_Options_NameTime.Checked    = this.settingsInEdit.NameTime;

			Log.FileNameSeparatorEx separator = this.settingsInEdit.NameSeparator;
			SelectionHelper.Select(comboBox_Options_NameSeparator, separator, separator);

			bool dateTimeEnabled = (this.settingsInEdit.WriteMode == Log.LogFileWriteMode.Create);
			checkBox_Options_NameDate.Enabled = dateTimeEnabled;
			checkBox_Options_NameTime.Enabled = dateTimeEnabled;

			checkBox_Options_FolderFormat.Checked  = this.settingsInEdit.FolderFormat;
			checkBox_Options_FolderChannel.Checked = this.settingsInEdit.FolderChannel;

			radioButton_Options_ModeCreate.Checked = (this.settingsInEdit.WriteMode == Log.LogFileWriteMode.Create);
			radioButton_Options_ModeAppend.Checked = (this.settingsInEdit.WriteMode == Log.LogFileWriteMode.Append);

			if (this.settingsInEdit.TextEncodingIsSupported) {
				groupBox_Options_TextEncoding.Enabled            =  true;
				radioButton_Options_TextEncodingUTF8.Checked     = (this.settingsInEdit.TextEncoding == Log.TextEncoding.UTF8);
				radioButton_Options_TextEncodingTerminal.Checked = (this.settingsInEdit.TextEncoding == Log.TextEncoding.Terminal);
				checkBox_Options_EmitEncodingPreamble.Checked    =  this.settingsInEdit.EmitEncodingPreamble;
				checkBox_Options_EmitEncodingPreamble.Text       = (this.settingsInEdit.EmitEncodingPreamble ? "with BOM" : "without BOM");
			}
			else {
				groupBox_Options_TextEncoding.Enabled            = false;
				radioButton_Options_TextEncodingUTF8.Checked     = true; // Show default, XML is UTF-8 too, RTF don't care.
				radioButton_Options_TextEncodingTerminal.Checked = false;
				checkBox_Options_EmitEncodingPreamble.Checked    = true; // Show default, XML is UTF-8 too, RTF don't care.
				checkBox_Options_EmitEncodingPreamble.Text       = "with BOM";
			}

			this.isSettingControls.Leave();
		}

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "'formIsOpen' does start with a lower case letter.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private bool ShowSetRootDirectoryDialog_dialogIsOpen; // = false;

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowSetRootDirectoryDialog()
		{
			// Ensure that dialog is only shown once at a time. Because if this method is invoked
			// again while the dialog is still open (possible e.g. if this method is invoked by a
			// shortcut process in 'ProcessCmdKey()'), multiple dialogs would be shown in parallel!
			// 
			// A simple boolean flag without any interlocked or monitor protection is sufficient,
			// as this method will always have to be synchonized onto the main thread.
			// 
			// For the same reason, 'Monitor.TryEnter()' cannot be used as that would always be
			// successful on the main thread.
			if (!ShowSetRootDirectoryDialog_dialogIsOpen)
			{
				ShowSetRootDirectoryDialog_dialogIsOpen = true;
				try
				{
					DoShowSetRootDirectoryDialog();
				}
				finally
				{
					ShowSetRootDirectoryDialog_dialogIsOpen; // = false;
				}
			}
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void DoShowSetRootDirectoryDialog()
		{
			var ofd = new OpenFileDialog();
			ofd.Title = "Set Root";
			ofd.Filter = ExtensionHelper.AllFilesFilter;

			if (Directory.Exists(this.settingsInEdit.RootPath))
				ofd.InitialDirectory = this.settingsInEdit.RootPath;
			else
				ofd.InitialDirectory = ApplicationSettings.LocalUserSettings.Paths.LogFiles;

			ofd.FileName = this.settingsInEdit.RootFileName;
			ofd.CheckPathExists = false;
			ofd.CheckFileExists = false;
			if ((ofd.ShowDialog(this) == DialogResult.OK) && (!string.IsNullOrEmpty(ofd.FileName)))
			{
				Refresh();

				ApplicationSettings.LocalUserSettings.Paths.LogFiles = Path.GetDirectoryName(ofd.FileName);
				ApplicationSettings.Save();

				this.settingsInEdit.RootPath = Path.GetDirectoryName(ofd.FileName);
				this.settingsInEdit.RootFileName = Path.GetFileNameWithoutExtension(ofd.FileName);
			}
		}

		//------------------------------------------------------------------------------------------
		// Validation
		//------------------------------------------------------------------------------------------

		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Emphasize line breaks.")]
		[ModalBehavior(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private bool ValidateFileNamePart(string fileNamePart, string title)
		{
			char[] invalid = Path.GetInvalidFileNameChars();
			if (!StringEx.ContainsAny(fileNamePart, invalid))
			{
				return (true);
			}
			else
			{
				var sb = new StringBuilder(invalid.Length);
				sb.Append(title);
				sb.AppendLine(" contains invalid characters.");
				sb.AppendLine();

				foreach (char c in invalid)
				{
					if (!char.IsControl(c) && !char.IsWhiteSpace(c))
						sb.Append(c);
				}

				sb.Append(" are not allowed in file names.");

				MessageBoxEx.Show
				(
					this,
					sb.ToString(),
					"Invalid Characters",
					MessageBoxButtons.OK,
					MessageBoxIcon.Exclamation
				);

				return (false);
			}
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ExtensionConflictMessage()
		{
			string message =
				"To avoid naming conflicts, files must either be placed in format folders or named by format or have different extensions. " +
				Environment.NewLine + Environment.NewLine +
				"First select a different extension. You may then change one of the other options to resolve the naming conflict.";

			MessageBoxEx.Show
			(
				this,
				message,
				"Extension Conflict",
				MessageBoxButtons.OK,
				MessageBoxIcon.Exclamation
			);
		}

		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Message too long.")]
		[ModalBehavior(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private bool ResolveNamingConflicts()
		{
			if ((this.settingsInEdit.SameRawAndNeat) && (this.settingsInEdit.RawExtension == this.settingsInEdit.NeatExtension) &&
				(!(this.settingsInEdit.FolderFormat || this.settingsInEdit.NameFormat)))
			{
				string message =
					"To avoid naming conflicts, files must either be named by format or separated into format folders (Raw/Neat) or have different extensions." +
					Environment.NewLine + Environment.NewLine +
					"Do you want to name the files by format [Yes] or separate them into folders [No]?" +
					Environment.NewLine + Environment.NewLine +
					"You may also cancel and set different extensions, or manually change the settings.";

				switch (MessageBoxEx.Show
					(
					this,
					message,
					"Naming Conflict",
					MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Question
					))
				{
					case DialogResult.Yes: this.settingsInEdit.NameFormat   = true; break;
					case DialogResult.No:  this.settingsInEdit.FolderFormat = true; break;
					default: return (false);
				}
			}

			if ((this.settingsInEdit.MultipleRaw || this.settingsInEdit.MultipleNeat) &&
				(!(this.settingsInEdit.FolderChannel || this.settingsInEdit.NameChannel)))
			{
				string message =
					"To avoid naming conflicts, files must either be named by channel or separated into channel folders (Tx/Bidir/Rx)." +
					Environment.NewLine + Environment.NewLine +
					"Do you want to name the files by channel [Yes] or separate them into folders [No]?" +
					Environment.NewLine + Environment.NewLine +
					"You may also cancel and manually change the settings.";

				switch (MessageBoxEx.Show
					(
					this,
					message,
					"Naming Conflict",
					MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Question
					))
				{
					case DialogResult.Yes: this.settingsInEdit.NameChannel   = true; break;
					case DialogResult.No:  this.settingsInEdit.FolderChannel = true; break;
					default: return (false);
				}
			}

			return (true);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
