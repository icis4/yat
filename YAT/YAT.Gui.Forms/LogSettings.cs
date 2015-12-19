//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

using MKY;
using MKY.IO;
using MKY.Windows.Forms;

using YAT.Settings;
using YAT.Settings.Application;

#endregion

namespace YAT.Gui.Forms
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

		private Domain.TerminalType terminalType;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public LogSettings(Log.Settings.LogSettings settings, Domain.TerminalType terminalType)
		{
			InitializeComponent();

			KeepAndCloneAndAttachSettings(settings);
			InitializeControls();

			this.terminalType = terminalType;

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
			this.settingsInEdit.Changed += new EventHandler<MKY.Settings.SettingsEventArgs>(settings_Form_Changed);
		}

		private void DetachAndAcceptSettings()
		{
			this.settingsInEdit.Changed -= new EventHandler<MKY.Settings.SettingsEventArgs>(settings_Form_Changed);
			this.settings = this.settingsInEdit;
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
		/// event can depend on a properly drawn form, even when a modal dialog (e.g. a message box)
		/// is shown. This is due to the fact that the 'Paint' event will happen right after this
		/// 'Shown' event and will somehow be processed asynchronously.
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

		private void button_RootOpen_Click(object sender, EventArgs e)
		{
			Exception ex;
			if (!DirectoryEx.TryOpen(this.settingsInEdit.RootPath, out ex))
			{
				MessageBox.Show
				(
					this.Parent,
					"Unable to open folder." + Environment.NewLine + Environment.NewLine +
					"System error message:" + Environment.NewLine + ex.Message,
					"System Error",
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
			if (!ValidateFilenameChars(comboBox_Raw_Extension.Text, "Extension"))
			{
				e.Cancel = true;
				return;
			}
			if (this.settingsInEdit.BothRawAndNeat && (comboBox_Raw_Extension.Text == this.settingsInEdit.NeatExtension))
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
			if (!ValidateFilenameChars(comboBox_Neat_Extension.Text, "Extension"))
			{
				e.Cancel = true;
				return;
			}
			if (this.settingsInEdit.BothRawAndNeat && (comboBox_Neat_Extension.Text == this.settingsInEdit.RawExtension))
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

		private void comboBox_Options_NameSeparator_Validating(object sender, CancelEventArgs e)
		{
			if (!ValidateFilenameChars(comboBox_Options_NameSeparator.Text, "Separator"))
				e.Cancel = true;
		}

		private void comboBox_Options_NameSeparator_TextChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsInEdit.NameSeparator = (Log.FileNameSeparator)(comboBox_Options_NameSeparator.Text);
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

		private void radioButton_Options_EncodingUTF8_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls && radioButton_Options_EncodingUTF8.Checked)
				this.settingsInEdit.TextEncoding = Log.LogFileEncoding.UTF8;
		}

		private void radioButton_Options_EncodingTerminal_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls && radioButton_Options_EncodingTerminal.Checked)
				this.settingsInEdit.TextEncoding = Log.LogFileEncoding.Terminal;
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
			}
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void InitializeControls()
		{
			this.isSettingControls.Enter();

			comboBox_Neat_Extension.Items.Clear();
			foreach (string s in ExtensionSettings.TextFilesWithDot)
				comboBox_Neat_Extension.Items.Add(s);

			comboBox_Raw_Extension.Items.Clear();
			foreach (string s in ExtensionSettings.BinaryFilesWithDot)
				comboBox_Raw_Extension.Items.Add(s);

			comboBox_Options_NameSeparator.Items.Clear();
			foreach (string s in Log.FileNameSeparator.Items)
				comboBox_Options_NameSeparator.Items.Add(s);

			this.isSettingControls.Leave();
		}

		private void SetControls()
		{
			this.isSettingControls.Enter();

			if (this.settingsInEdit.RootPath.Length > 0)
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
			comboBox_Options_NameSeparator.Text  = this.settingsInEdit.NameSeparator;

			bool dateTimeEnabled = (this.settingsInEdit.WriteMode == Log.LogFileWriteMode.Create);
			checkBox_Options_NameDate.Enabled = dateTimeEnabled;
			checkBox_Options_NameTime.Enabled = dateTimeEnabled;

			checkBox_Options_FolderFormat.Checked  = this.settingsInEdit.FolderFormat;
			checkBox_Options_FolderChannel.Checked = this.settingsInEdit.FolderChannel;

			radioButton_Options_ModeCreate.Checked = (this.settingsInEdit.WriteMode == Log.LogFileWriteMode.Create);
			radioButton_Options_ModeAppend.Checked = (this.settingsInEdit.WriteMode == Log.LogFileWriteMode.Append);

			groupBox_Options_Encoding.Enabled = (this.terminalType == Domain.TerminalType.Text);

			if (this.terminalType == Domain.TerminalType.Text) {
				radioButton_Options_EncodingUTF8.Checked     = (this.settingsInEdit.TextEncoding == Log.LogFileEncoding.UTF8);
				radioButton_Options_EncodingTerminal.Checked = (this.settingsInEdit.TextEncoding == Log.LogFileEncoding.Terminal);
			}
			else {
				radioButton_Options_EncodingUTF8.Checked     = false; // Encoding is inactive for binary terminals.
				radioButton_Options_EncodingTerminal.Checked = false; // Encoding is inactive for binary terminals.
			}

			this.isSettingControls.Leave();
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowSetRootDirectoryDialog()
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Title = "Set Root";
			ofd.Filter = ExtensionSettings.AllFilesFilter;

			if (Directory.Exists(this.settingsInEdit.RootPath))
				ofd.InitialDirectory = this.settingsInEdit.RootPath;
			else
				ofd.InitialDirectory = ApplicationSettings.LocalUserSettings.Paths.LogFilesPath;

			ofd.FileName = this.settingsInEdit.RootFileName;
			ofd.CheckPathExists = false;
			ofd.CheckFileExists = false;
			if ((ofd.ShowDialog(this) == DialogResult.OK) && (ofd.FileName.Length > 0))
			{
				Refresh();

				ApplicationSettings.LocalUserSettings.Paths.LogFilesPath = Path.GetDirectoryName(ofd.FileName);
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
		private bool ValidateFilenameChars(string filenameChars, string title)
		{
			StringWriter invalid = new StringWriter(CultureInfo.InvariantCulture);
			invalid.Write(Path.GetInvalidPathChars());
			invalid.Write(Path.VolumeSeparatorChar);
			invalid.Write(Path.DirectorySeparatorChar);
			invalid.Write(Path.AltDirectorySeparatorChar);
			invalid.Write(Path.PathSeparator);

			if (StringEx.ContainsAny(filenameChars, invalid.ToString().ToCharArray()))
			{
				StringWriter invalidPrintable = new StringWriter(CultureInfo.InvariantCulture);
				foreach (char c in invalid.ToString().ToCharArray())
				{
					if (!char.IsControl(c))
						invalidPrintable.Write(c);
				}

				string message =
					title + " contains invalid characters." + Environment.NewLine + Environment.NewLine +
					invalidPrintable.ToString() + " are not allowed in file names.";

				MessageBoxEx.Show
				(
					this,
					message,
					"Invalid Characters",
					MessageBoxButtons.OK,
					MessageBoxIcon.Exclamation
				);

				return (false);
			}
			return (true);
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ExtensionConflictMessage()
		{
			MessageBoxEx.Show
			(
				this,
				"To avoid naming conflicts, raw and neat log files must have different extensions. Choose a different extension.",
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
				(!this.settingsInEdit.FolderFormat && !this.settingsInEdit.NameFormat))
			{
				string message =
					"To avoid naming conflicts, files must either be placed in format folders or named by format (Raw/Neat). " +
					"Do you want to place the files in folders (Yes) or name them by format (No)? You may also cancel and" +
					"set different extensions.";

				switch (MessageBoxEx.Show
					(
					this,
					message,
					"Naming Conflict",
					MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Question
					))
				{
					case DialogResult.Yes: this.settingsInEdit.FolderFormat = true; break;
					case DialogResult.No: this.settingsInEdit.NameFormat = true; break;
					default: return (false);
				}
			}

			if ((this.settingsInEdit.MultipleRaw || this.settingsInEdit.MultipleNeat) &&
				(!this.settingsInEdit.FolderChannel && !this.settingsInEdit.NameChannel))
			{
				string message =
					"To avoid naming conflicts, files must either be placed in channel " +
					"folders or named by channel (Tx/Bidir/Rx). Do you want to place " +
					"the files in folders (Yes) or name them by channel (No)?";

				switch (MessageBoxEx.Show
					(
					this,
					message,
					"Naming Conflict",
					MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Question
					))
				{
					case DialogResult.Yes: this.settingsInEdit.FolderChannel = true; break;
					case DialogResult.No: this.settingsInEdit.NameChannel = true; break;
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
