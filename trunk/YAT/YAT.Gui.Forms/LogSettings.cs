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
// YAT 2.0 Beta 4 Candidate 2 Development Version 1.99.29
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
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
using MKY.Windows.Forms;

using YAT.Settings;
using YAT.Settings.Application;

#endregion

namespace YAT.Gui.Forms
{
	/// <summary></summary>
	public partial class LogSettings : System.Windows.Forms.Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;

		private Log.Settings.LogSettings settings;
		private Log.Settings.LogSettings settings_Form;

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
		
			// SetControls() is initially called in the 'Paint' event handler.
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
			this.settings_Form = new Log.Settings.LogSettings(settings);
			this.settings_Form.Changed += new EventHandler<MKY.Settings.SettingsEventArgs>(settings_Form_Changed);
		}

		private void DetachAndAcceptSettings()
		{
			this.settings_Form.Changed -= new EventHandler<MKY.Settings.SettingsEventArgs>(settings_Form_Changed);
			this.settings = this.settings_Form;
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

		private void checkBox_Raw_Tx_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.RawLogTx = checkBox_Raw_Tx.Checked;
		}

		private void pathLabel_Raw_Tx_Click(object sender, EventArgs e)
		{
			this.settings_Form.RawLogTx = !this.settings_Form.RawLogTx;
		}

		private void checkBox_Raw_Bidir_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.RawLogBidir = checkBox_Raw_Bidir.Checked;
		}

		private void pathLabel_Raw_Bidir_Click(object sender, EventArgs e)
		{
			this.settings_Form.RawLogBidir = !this.settings_Form.RawLogBidir;
		}

		private void checkBox_Raw_Rx_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.RawLogRx = checkBox_Raw_Rx.Checked;
		}

		private void pathLabel_Raw_Rx_Click(object sender, EventArgs e)
		{
			this.settings_Form.RawLogRx = !this.settings_Form.RawLogRx;
		}

		private void comboBox_Raw_Extension_Validating(object sender, CancelEventArgs e)
		{
			if (!ValidateFilenameChars(comboBox_Raw_Extension.Text, "Extension"))
			{
				e.Cancel = true;
				return;
			}
			if (this.settings_Form.BothRawAndNeat && (comboBox_Raw_Extension.Text == this.settings_Form.NeatExtension))
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
				this.settings_Form.RawExtension = comboBox_Raw_Extension.Text;
		}

		private void checkBox_Neat_Tx_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.NeatLogTx = checkBox_Neat_Tx.Checked;
		}

		private void pathLabel_Neat_Tx_Click(object sender, EventArgs e)
		{
			this.settings_Form.NeatLogTx = !this.settings_Form.NeatLogTx;
		}

		private void checkBox_Neat_Bidir_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.NeatLogBidir = checkBox_Neat_Bidir.Checked;
		}

		private void pathLabel_Neat_Bidir_Click(object sender, EventArgs e)
		{
			this.settings_Form.NeatLogBidir = !this.settings_Form.NeatLogBidir;
		}

		private void checkBox_Neat_Rx_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.NeatLogRx = checkBox_Neat_Rx.Checked;
		}

		private void pathLabel_Neat_Rx_Click(object sender, EventArgs e)
		{
			this.settings_Form.NeatLogRx = !this.settings_Form.NeatLogRx;
		}

		private void comboBox_Neat_Extension_Validating(object sender, CancelEventArgs e)
		{
			if (!ValidateFilenameChars(comboBox_Neat_Extension.Text, "Extension"))
			{
				e.Cancel = true;
				return;
			}
			if (this.settings_Form.BothRawAndNeat && (comboBox_Neat_Extension.Text == this.settings_Form.RawExtension))
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
				this.settings_Form.NeatExtension = comboBox_Neat_Extension.Text;
		}

		private void rad_Options_ModeCreate_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls && rad_Options_ModeCreate.Checked)
			{
				this.settings_Form.WriteMode = Log.LogFileWriteMode.Create;
				this.settings_Form.NameDate = this.settings.NameDate;
				this.settings_Form.NameTime = this.settings.NameTime;
			}
		}

		private void rad_Options_ModeAppend_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls && rad_Options_ModeAppend.Checked)
			{
				this.settings_Form.WriteMode = Log.LogFileWriteMode.Append;
				this.settings_Form.NameDate = false;
				this.settings_Form.NameTime = false;
			}
		}

		private void checkBox_Options_SubdirectoriesFormat_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.SubdirectoriesFormat = checkBox_Options_SubdirectoriesFormat.Checked;
		}

		private void checkBox_Options_SubdirectoriesChannel_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.SubdirectoriesChannel = checkBox_Options_SubdirectoriesChannel.Checked;
		}

		private void checkBox_Options_NameFormat_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.NameFormat = checkBox_Options_NameFormat.Checked;
		}

		private void checkBox_Options_NameChannel_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.NameChannel = checkBox_Options_NameChannel.Checked;
		}

		private void checkBox_Options_NameDate_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.NameDate = checkBox_Options_NameDate.Checked;
		}

		private void checkBox_Options_NameTime_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.NameTime = checkBox_Options_NameTime.Checked;
		}

		private void comboBox_Options_NameSeparator_Validating(object sender, CancelEventArgs e)
		{
			if (!ValidateFilenameChars(comboBox_Options_NameSeparator.Text, "Separator"))
				e.Cancel = true;
		}

		private void comboBox_Options_NameSeparator_TextChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.NameSeparator = (Log.FileNameSeparator)(string)comboBox_Options_NameSeparator.Text;
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
				this.settings_Form.SetDefaults();
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

			if (this.settings_Form.RootPath.Length > 0)
				pathLabel_Root.Text = this.settings_Form.RootPath + Path.DirectorySeparatorChar + this.settings_Form.RootFileName;
			else
				pathLabel_Root.Text = "<Set a root file...>";

			checkBox_Raw_Tx.Checked    = this.settings_Form.RawLogTx;
			checkBox_Raw_Bidir.Checked = this.settings_Form.RawLogBidir;
			checkBox_Raw_Rx.Checked    = this.settings_Form.RawLogRx;

			pathLabel_Raw_Tx.Text    = this.settings_Form.RawTxRootRelativeFilePath;
			pathLabel_Raw_Bidir.Text = this.settings_Form.RawBidirRootRelativeFilePath;
			pathLabel_Raw_Rx.Text    = this.settings_Form.RawRxRootRelativeFilePath;

			comboBox_Raw_Extension.Text = this.settings_Form.RawExtension;

			checkBox_Neat_Tx.Checked    = this.settings_Form.NeatLogTx;
			checkBox_Neat_Bidir.Checked = this.settings_Form.NeatLogBidir;
			checkBox_Neat_Rx.Checked    = this.settings_Form.NeatLogRx;

			pathLabel_Neat_Tx.Text    = this.settings_Form.NeatTxRootRelativeFilePath;
			pathLabel_Neat_Bidir.Text = this.settings_Form.NeatBidirRootRelativeFilePath;
			pathLabel_Neat_Rx.Text    = this.settings_Form.NeatRxRootRelativeFilePath;

			comboBox_Neat_Extension.Text = this.settings_Form.NeatExtension;

			rad_Options_ModeCreate.Checked = (this.settings_Form.WriteMode == Log.LogFileWriteMode.Create);
			rad_Options_ModeAppend.Checked = (this.settings_Form.WriteMode == Log.LogFileWriteMode.Append);

			checkBox_Options_SubdirectoriesFormat.Checked  = this.settings_Form.SubdirectoriesFormat;
			checkBox_Options_SubdirectoriesChannel.Checked = this.settings_Form.SubdirectoriesChannel;

			checkBox_Options_NameFormat.Checked  = this.settings_Form.NameFormat;
			checkBox_Options_NameChannel.Checked = this.settings_Form.NameChannel;
			checkBox_Options_NameDate.Checked    = this.settings_Form.NameDate;
			checkBox_Options_NameTime.Checked    = this.settings_Form.NameTime;
			comboBox_Options_NameSeparator.Text  = this.settings_Form.NameSeparator;

			bool dateTimeEnabled = (this.settings_Form.WriteMode == Log.LogFileWriteMode.Create);
			checkBox_Options_NameDate.Enabled = dateTimeEnabled;
			checkBox_Options_NameTime.Enabled = dateTimeEnabled;

			this.isSettingControls.Leave();
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowSetRootDirectoryDialog()
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Title = "Set Root";
			ofd.Filter = ExtensionSettings.AllFilesFilter;

			if (Directory.Exists(this.settings_Form.RootPath))
				ofd.InitialDirectory = this.settings_Form.RootPath;
			else
				ofd.InitialDirectory = ApplicationSettings.LocalUserSettings.Paths.LogFilesPath;

			ofd.FileName = this.settings_Form.RootFileName;
			ofd.CheckPathExists = false;
			ofd.CheckFileExists = false;
			if ((ofd.ShowDialog(this) == DialogResult.OK) && (ofd.FileName.Length > 0))
			{
				Refresh();

				ApplicationSettings.LocalUserSettings.Paths.LogFilesPath = Path.GetDirectoryName(ofd.FileName);
				ApplicationSettings.Save();

				this.settings_Form.RootPath = Path.GetDirectoryName(ofd.FileName);
				this.settings_Form.RootFileName = Path.GetFileNameWithoutExtension(ofd.FileName);
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
			if ((this.settings_Form.SameRawAndNeat) && (this.settings_Form.RawExtension == this.settings_Form.NeatExtension) &&
				(!this.settings_Form.SubdirectoriesFormat && !this.settings_Form.NameFormat))
			{
				string message =
					"To avoid naming conflicts, files must either be placed in format subdirectories or named by format (Raw/Neat). " +
					"Do you want to place the files in subdirectories (Yes) or name them by format (No)? You can also press " +
					"Cancel and set different extensions.";

				switch (MessageBoxEx.Show
					(
					this,
					message,
					"Naming Conflict",
					MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Question
					))
				{
					case DialogResult.Yes: this.settings_Form.SubdirectoriesFormat = true; break;
					case DialogResult.No: this.settings_Form.NameFormat = true; break;
					default: return (false);
				}
			}

			if ((this.settings_Form.MultipleRaw || this.settings_Form.MultipleNeat) &&
				(!this.settings_Form.SubdirectoriesChannel && !this.settings_Form.NameChannel))
			{
				string message =
					"To avoid naming conflicts, files must either be placed in channel " +
					"subdirectories or named by channel (Tx/Bidir/Rx). Do you want to place " +
					"the files in subdirectories (Yes) or name them by channel (No)?";

				switch (MessageBoxEx.Show
					(
					this,
					message,
					"Naming Conflict",
					MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Question
					))
				{
					case DialogResult.Yes: this.settings_Form.SubdirectoriesChannel = true; break;
					case DialogResult.No: this.settings_Form.NameChannel = true; break;
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
